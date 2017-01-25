#r "packages/FSharp.Collections.ParallelSeq/lib/net40/FSharp.Collections.ParallelSeq.dll"
#load "packages/FsLab/Themes/AtomChester.fsx"
#load "packages/FsLab/FsLab.fsx"

open System
open System.IO
open System.Collections.Generic
open FSharp.Collections.ParallelSeq

open FsLab
open FSharp.Data
open XPlot.Plotly
open Deedle
open MathNet.Numerics.Statistics

Environment.CurrentDirectory = __SOURCE_DIRECTORY__

module Seq =
    let toRelativeTimeSeq (s: seq<DateTime>) =
        if Seq.isEmpty s then Seq.empty
        else
            let min = Seq.min s
            s
            |> Seq.sort
            |> Seq.map (fun x -> x - min)

module Stats =
    let percentile p (s: Series<'a, float>) =
        s.Values.Percentile(p)

    let diff (s: Series<'a, float>) =
        let max = Stats.max s
        let min = Stats.min s
        match (max, min) with
            | Some(mx), Some(mi) -> mx - mi
            | _ -> 0.0

    let unwrapWith defaultValue option =
        match option with
            | Some(x) -> x
            | None -> defaultValue

    let unwrap (option: Option<'t>) =
        unwrapWith Unchecked.defaultof<'t> option

module Series =
    let normalizeSeries
        (agg: Series<int, float> -> float)
        (s: Series<'k, 'v1 * 'v2>) =
        s
        |> Series.values
        |> Seq.groupBy fst
        |> Seq.map (fun (ts, vals) ->
                        (ts, vals
                            |> Seq.map (snd >> Convert.ToDouble)
                            |> Series.ofValues
                            |> agg
                        ))
        |> Series.ofObservations
        |> Series.sortByKey

    let mergeReduce (agg: 'v -> 'v -> 'v)
                    (s1: Series<'k, 'v>)
                    (s2: Series<'k, 'v>) =
        Seq.concat [Series.observations s1; Series.observations s2]
        |> Seq.groupBy (fun (k, _) -> k)
        |> Seq.map (fun (k, v) -> k, v |> Seq.map snd |> Seq.reduce agg)
        |> Series.ofObservations
        |> Series.sortByKey

    let trim (s: Series<'k, Series<'k1, 'v>>) =
        let min =
            s
            |> Series.mapValues Series.keys
            |> Series.mapValues Seq.max
            |> Stats.min
            |> Stats.unwrap
        let mapper v = v |> Series.filter (fun k _ -> k <= min)
        s |> Series.mapValues mapper

    let normalize (norm: Series<'k, float>) (s: Series<'k, float>) =
        s
        |> Series.map (fun k v -> v / norm.Get(k))

    let toScatter (s: Series<'x, 'y>) =
            Scatter(x = Series.keys s, y = Series.values s)

    let toScatterWith (config: Scatter -> unit) (s: Series<'x, 'y>)  =
            let scatter = toScatter s
            config scatter
            scatter

module Frame =
    let private mapTsColumn index (s: ObjectSeries<int>) =
        if index = "Timestamp" then
            s
            |> Series.values
            |> Seq.map (Convert.ToInt64 >> DateTime)
            |> Seq.toRelativeTimeSeq
            |> Seq.cast<obj>
            |> Series.ofValues
            |> ObjectSeries
        else s

    let toTsFrame frame =
        frame |> Frame.mapCols mapTsColumn

let fullPath x = Path.Combine (__SOURCE_DIRECTORY__, x)

let getExperimentKey filePath =
    let name = Path.GetFileNameWithoutExtension(filePath)
    let key = name.Split('_').[0]
    key

type Counters = CsvProvider<"samples/sample_counters.csv", CacheRows = false>

type CountersStats =
    {
        Name: string;
        CountersFrame: Frame<int, string>;
        EventsStatsSeries: Series<string, float>;
        EventsProcessedFrame: Frame<string * int, string>;
        EventsProcessedSeries: Series<TimeSpan, float>;
        AvgEventsPerSecSeries: Series<TimeSpan, float>;
    }

let analyzeCounters (filePath: string) =
    let key = getExperimentKey filePath
    let countersFrame =
        Frame.ReadCsv(filePath, separators = ",")
        |> Frame.toTsFrame

    let eventsProcessedFrame =
        countersFrame
        |> Frame.filterRowsBy "Name" "Bot.ProcessedEvents"
        |> Frame.dropCol "Name"
        |> Frame.groupRowsUsing (fun k v -> v.["BotId"].ToString())

    let eventsProcessedSeries =
        eventsProcessedFrame
        |> (fun f ->
            Series.zipInner
                (f.GetColumn<TimeSpan>("Timestamp"))
                (f.GetColumn<int>("Value"))
            )
        |> Series.groupBy (fun (botId, _) _ -> botId)
        |> Series.mapValues (Series.normalizeSeries Stats.sum)
        |> Series.mapValues (Series.diff 1)
        |> Series.reduceValues (Series.mergeReduce (+))
        |> Stats.expandingSum
        |> Series.chunkDistInto (TimeSpan(0, 0, 1)) Stats.max
        |> Series.mapValues (fun x -> if x.IsSome then x.Value else Double.NaN)
        |> Series.fillMissing Direction.Backward


    let eventsPerSecSeries =
        eventsProcessedSeries
        |> Series.diff 1
        |> Series.windowDistInto (TimeSpan(0, 0, 5)) Stats.mean

    let eventsStatsSeries =
        [
            ("Events Total", Stats.unwrap (Stats.max eventsProcessedSeries))
            ("Events/Sec Min", Stats.unwrap (Stats.min eventsPerSecSeries))
            ("Events/Sec Max", Stats.unwrap (Stats.max eventsPerSecSeries))
            ("Events/Sec Mean", Stats.mean eventsPerSecSeries)
            ("Events/Sec Std Dev", Stats.stdDev eventsPerSecSeries)
            ("Events/Sec Median", Stats.median eventsPerSecSeries)
            ("Events/Sec 90th Percentile", Stats.percentile 90 eventsPerSecSeries)
        ]
        |> Series.ofObservations

    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        CountersFrame = countersFrame;
        EventsStatsSeries = eventsStatsSeries;
        EventsProcessedFrame = eventsProcessedFrame;
        EventsProcessedSeries = eventsProcessedSeries;
        AvgEventsPerSecSeries = eventsPerSecSeries;
    }

let filePath = fullPath "samples/s1b8_failures.csv"

type FailureStats =
    {
        FailuresFrame: Frame<int, string>;
        FailuresTotalSeries: Series<TimeSpan, float>;
        FailuresStatsSeries: Series<string, float>;
    }

let analyzeFailures (filePath: string) =
    let failuresFrame =
        Frame.ReadCsv(filePath)
        |> Frame.toTsFrame

    let toTsSeries frame =
        frame
        |> (fun f ->
            let length = Frame.countRows f
            Series.zipInner
                (f.GetColumn<TimeSpan>("Timestamp"))
                (Seq.replicate length 1 |> Series.ofValues)
           )
        |> Series.normalizeSeries Stats.sum

    let failuresSeries = toTsSeries failuresFrame

    let failuresTotalSeries =
        failuresSeries
        |> Stats.expandingSum
        |> Series.chunkDistInto (TimeSpan(0, 0, 1)) Stats.max
        |> Series.mapValues (fun x -> if x.IsSome then x.Value else Double.NaN)
        |> Series.fillMissing Direction.Backward

    let errorsSeries =
        failuresFrame
        |> Frame.filterRows (fun _ v -> (v.Get("Level").ToString()) = "Error")
        |> toTsSeries

    let warningSeries =
        failuresFrame
        |> Frame.filterRows (fun _ v -> (v.Get("Level").ToString()) = "Warning")
        |> toTsSeries

    let failuresStatsSeries =
        [
            ("Errors Total", Stats.sum errorsSeries)
            ("Warnings Total", Stats.sum warningSeries)
        ]
        |> Series.ofObservations

    {
        FailuresFrame = failuresFrame
        FailuresTotalSeries = failuresTotalSeries
        FailuresStatsSeries = failuresStatsSeries
    }

type DurationStats =
    {
        Name: string;
        DurationsFrame: Frame<int, string>;
        ReqsFrame: Frame<int, string>;
        ReqsStatsSeries: Series<string, float>;
        NormReqsSeries: Series<TimeSpan, float>;
        NormReqsFrame: Frame<TimeSpan, string>;
        ReqsPercentileSeries: Series<TimeSpan, float>;
        ReqsPercentileFrame: Frame<TimeSpan, string>;
        ReqsHistogramSeries: Series<int, (string * int)>;
    }

let analyzeDurations (filePath: string) =
    let key = getExperimentKey filePath

    let durationsFrame =
        Frame.ReadCsv (filePath, separators = ",")
        |> Frame.toTsFrame

    let reqsFrame =
        durationsFrame
        |> Frame.filterRowsBy "Name" "SignalR.Send"
        |> Frame.dropCol "Name"
        |> Frame.dropCol "BotId"

    let reqsDurationSeries =
        reqsFrame.GetColumn<float>("Ms")

    let totalReqs = reqsDurationSeries.KeyCount

    let reqsStatsSeries =
        [
            ("Reqs Total", float totalReqs)
            ("Latency Min", Stats.unwrap (Stats.min reqsDurationSeries))
            ("Latency Max", Stats.unwrap (Stats.max reqsDurationSeries))
            ("Latency Mean", Stats.mean reqsDurationSeries)
            ("Latency Std Dev", Stats.stdDev reqsDurationSeries)
            ("Latency Median", Stats.median reqsDurationSeries)
            ("Latency 90th Percentile", Stats.percentile 90 reqsDurationSeries)
        ]
        |> Series.ofObservations

    let normReqsSeries =
        let tsSeries = reqsFrame.GetColumn<TimeSpan>("Timestamp")
        let msSeries = reqsFrame.GetColumn<int>("Ms")
        Series.zipInner tsSeries msSeries
        |> Series.normalizeSeries (Stats.percentile 90)

    let normReqsFrame = ["Ms" => normReqsSeries] |> frame

    let reqPercentileSeries =
        normReqsSeries
        |> Series.chunkDistInto (TimeSpan(0, 0, 1)) (Stats.percentile 90)

    let reqPercentileFrame = ["Ms" => reqPercentileSeries] |> frame

    let reqHistogramSeries =
        Series.zipInner
            (reqsFrame.GetColumn<string>("CommandType"))
            (reqsFrame.GetColumn<int>("Ms"))

    {
        Name = key;
        DurationsFrame = durationsFrame;
        ReqsFrame = reqsFrame;
        ReqsStatsSeries = reqsStatsSeries;
        NormReqsSeries = normReqsSeries;
        NormReqsFrame = normReqsFrame;
        ReqsPercentileSeries = reqPercentileSeries;
        ReqsPercentileFrame = reqPercentileFrame;
        ReqsHistogramSeries = reqHistogramSeries;
    }


let sampleCounters = fullPath "samples/sample_counters.csv"
let sampleDurations = fullPath "samples/sample_durations.csv"

let s1b2Counters = fullPath "samples/s1b2_counters.csv"
let s1b2Durations = fullPath "samples/s1b2_durations.csv"

let s1b4Counters = fullPath "samples/s1b4_counters.csv"
let s1b4Durations = fullPath "samples/s1b4_durations.csv"


type CaseStats =
    {
        Name: string;
        Counters: CountersStats;
        Durations: DurationStats;
        Failures: FailureStats;
    }

let analyzeCase (case: string) =
    let countersPath = sprintf "samples/%s_counters.csv" case |> fullPath
    let durationsPath = sprintf "samples/%s_durations.csv" case |> fullPath
    let failuresPath = sprintf "samples/%s_failures.csv" case |> fullPath
    {
        Name = case
        Counters = analyzeCounters countersPath
        Durations = analyzeDurations durationsPath
        Failures = analyzeFailures failuresPath
    }

let getSeconds (ts: TimeSpan) = ts.TotalSeconds

let toNamedScatter (k: string, s: Series<'x, 'y>) =
    s
    |> Series.toScatterWith (fun sc -> sc.name <- k)

let toNamedHistogram (k: string, s: Series<'x, 'y>) =
    let x = Series.values s
    Graph.Histogram(x = x, name = k, histnorm = "percent", nbinsx = 1000)

let toNamedBar (name: string) (s: Series<'x, 'y>) =
    Bar(x = Series.keys s, y = Series.values s, name = name)

let toScatters (s: Series<string, Series<TimeSpan, float>>) =
    s
    |> Series.mapValues (Series.mapKeys getSeconds)
    |> Series.observations
    |> Seq.map toNamedScatter

let plotEventsProcessed cases =
    cases
    |> Series.mapValues (fun v -> v.Counters.EventsProcessedSeries)
    |> Series.trim
    |> toScatters
    |> Chart.Plot

let plotEventsPerSec cases =
    cases
    |> Series.mapValues (fun v -> v.Counters.AvgEventsPerSecSeries)
    |> Series.trim
    |> toScatters
    |> Chart.Plot

let plotReqPercentiles cases =
    let layout =
        Layout (
            xaxis = Xaxis(title = "Time"),
            yaxis = Yaxis(title = "90% Percentile, ms")
        )
    cases
    |> Series.mapValues (fun v -> v.Durations.ReqsPercentileSeries)
    |> Series.trim
    |> toScatters
    |> Chart.Plot
    |> Chart.WithLayout layout

let plotReqHistograms cases =
    let overlayedLayout =
        Layout(barmode = "overlay")
    cases
    |> Series.mapValues (fun v -> v.Durations.ReqsHistogramSeries)
    |> Series.mapValues (Series.mapValues snd)
    |> Series.observations
    |> Seq.map toNamedHistogram
    |> Chart.Plot
    |> Chart.WithLayout overlayedLayout

let plotStatsBars (statsSeries: Series<string, Series<string, float>>) =
    let plot name bar =
        bar
        |> Chart.Plot
        |> Chart.WithTitle name
    statsSeries
    |> Frame.ofColumns
    |> Frame.rows
    |> Series.map toNamedBar
    |> Series.map plot

type ComparisonResult =
    {
        Stats: Series<string, CaseStats>;
        StatsFrame: Frame<string, string>;
        StatsBarCharts: Series<string, PlotlyChart>;
        EventsProcessedChart: PlotlyChart;
        EventsPerSecChart: PlotlyChart;
        ReqsPercentilesChart: PlotlyChart;
        ReqsHistogramsChart: PlotlyChart;
    }

let compareCases (cases: seq<string>) =
    let stats =
        cases
        |> PSeq.map (fun c -> c, analyzeCase c)
        |> Seq.sortBy fst
        |> Series.ofObservations
    let mergeStats case =
        [
            case.Durations.ReqsStatsSeries
            case.Counters.EventsStatsSeries
            case.Failures.FailuresStatsSeries
        ]
        |> Seq.reduce Series.merge
    let statsSeries =
        stats
        |> Series.mapValues mergeStats
    let statsBarCharts = plotStatsBars statsSeries
    let combinedStatsSeries =
        statsSeries
        |> Series.mapValues (Series.normalize (statsSeries.GetAt(0)))
        |> Series.mapKeys (fun k -> k + "_norm")
        |> Series.merge statsSeries
    let statsFrame =
        combinedStatsSeries
        |> Frame.ofColumns
        |> Frame.sortRowsByKey
    {
        Stats = stats;
        StatsBarCharts = statsBarCharts;
        StatsFrame = statsFrame;
        EventsProcessedChart = plotEventsProcessed stats;
        EventsPerSecChart = plotEventsPerSec stats;
        ReqsPercentilesChart = plotReqPercentiles stats;
        ReqsHistogramsChart = plotReqHistograms stats;
    }

let all = [
    for silos in [1; 2; 4] do
    for bots in [2; 4; 8] do
        yield sprintf "s%db%d" silos bots
]
let s1 = all |> List.filter (fun s -> s.StartsWith("s1"))
let s2 = all |> List.filter (fun s -> s.StartsWith("s2"))
let s4 = all |> List.filter (fun s -> s.StartsWith("s4"))
let b2 = all |> List.filter (fun s -> s.Contains("b2"))
let b4 = all |> List.filter (fun s -> s.Contains("b4"))
let b8 = all |> List.filter (fun s -> s.Contains("b8"))
let s2s4 = s2 @ s4

let s1Stats = compareCases s1
s1Stats.StatsFrame
s1Stats.EventsProcessedChart
s1Stats.EventsPerSecChart
s1Stats.ReqsPercentilesChart
s1Stats.ReqsHistogramsChart
s1Stats.StatsBarCharts.Get("Reqs Total")

let s2Stats = compareCases s2
s2Stats.StatsFrame
s2Stats.EventsProcessedChart
s2Stats.EventsPerSecChart
s2Stats.ReqsPercentilesChart
s2Stats.ReqsHistogramsChart

let s4Stats = compareCases s4
s4Stats.StatsFrame
s4Stats.EventsProcessedChart
s4Stats.EventsPerSecChart
s4Stats.ReqsPercentilesChart
s4Stats.ReqsHistogramsChart

let s2s4Stats = compareCases s2s4
s2s4Stats.StatsFrame
s2s4Stats.EventsProcessedChart
s2s4Stats.EventsPerSecChart
s2s4Stats.ReqsPercentilesChart
s2s4Stats.ReqsHistogramsChart

let b2Stats = compareCases b2
b2Stats.StatsFrame
b2Stats.EventsProcessedChart
b2Stats.EventsPerSecChart
b2Stats.ReqsPercentilesChart
b2Stats.ReqsHistogramsChart

let b4Stats = compareCases b4
b4Stats.StatsFrame
b4Stats.EventsProcessedChart
b4Stats.EventsPerSecChart
b4Stats.ReqsPercentilesChart
b4Stats.ReqsHistogramsChart

let b8Stats = compareCases b8
b8Stats.StatsFrame
b8Stats.EventsProcessedChart
b8Stats.EventsPerSecChart
b8Stats.ReqsPercentilesChart
b8Stats.ReqsHistogramsChart

let allStats = compareCases all
allStats.StatsFrame
allStats.EventsProcessedChart
allStats.EventsPerSecChart
allStats.ReqsPercentilesChart
allStats.ReqsHistogramsChart
