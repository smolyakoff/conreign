#load "packages/FsLab/Themes/AtomChester.fsx"
#load "packages/FsLab/FsLab.fsx"

open System
open System.IO
open System.Collections.Generic

open FsLab
open FSharp.Data
open XPlot.Plotly
open Deedle
open MathNet.Numerics.Statistics

Environment.CurrentDirectory = __SOURCE_DIRECTORY__

module Seq =
    let toRelativeTimeSeq (s: seq<DateTime>) =
        let min = Seq.min s
        s
        |> Seq.sort
        |> Seq.map (fun x -> x - min)

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

    let toScatter (s: Series<'x, 'y>) =
            Scatter(x = Series.keys s, y = Series.values s)

    let toScatterWith (config: Scatter -> unit) (s: Series<'x, 'y>)  =
            let scatter = toScatter s
            config scatter
            scatter

module Stats =
    let public percentile p (s: Series<'a, float>) =
        s.Values.Percentile(p)

    let public diff (s: Series<'a, float>) =
        let max = Stats.max s
        let min = Stats.min s
        match (max, min) with
            | Some(mx), Some(mi) -> mx - mi
            | _ -> 0.0

module Frame =
    let mapTsColumn index (s: ObjectSeries<int>) =
        if index = "Timestamp" then
            s
            |> Series.values
            |> Seq.map (Convert.ToInt64 >> DateTime)
            |> Seq.toRelativeTimeSeq
            |> Seq.cast<obj>
            |> Series.ofValues
            |> ObjectSeries
        else s

    let public toTsFrame frame =
        frame |> Frame.mapCols mapTsColumn

let fullPath x = Path.Combine (__SOURCE_DIRECTORY__, x)

type Counters = CsvProvider<"samples/sample_counters.csv", CacheRows = false>

type CountersStats =
    {
        Name: string;
        CountersFrame: Frame<int, string>;
        EventsProcessedFrame: Frame<string * int, string>;
        EventsProcessedSeries: Series<TimeSpan, float>;
        EventsPerSecSeries: Series<TimeSpan, float>;
    }

let analyzeCounters (filePath: string) =
    let countersFrame =
        Frame.ReadCsv(filePath, separators = ";")
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
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        CountersFrame = countersFrame;
        EventsProcessedFrame = eventsProcessedFrame;
        EventsProcessedSeries = eventsProcessedSeries;
        EventsPerSecSeries = eventsPerSecSeries;
    }

type DurationStats =
    {
        Name: string;
        DurationsFrame: Frame<int, string>;
        ReqsFrame: Frame<int, string>;
        NormReqsSeries: Series<TimeSpan, float>;
        NormReqsFrame: Frame<TimeSpan, string>;
        ReqsPercentileSeries: Series<TimeSpan, float>;
        ReqsPercentileFrame: Frame<TimeSpan, string>;
        ReqsHistogramSeries: Series<int, (string * int)>;
    }

let analyzeDurations (filePath: string) =
    let durationsFrame =
        Frame.ReadCsv (filePath, separators = ";")
        |> Frame.toTsFrame

    let reqsFrame =
        durationsFrame
        |> Frame.filterRowsBy "Name" "SignalR.Send"
        |> Frame.dropCol "Name"
        |> Frame.dropCol "BotId"

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
        Name = Path.GetFileNameWithoutExtension(filePath);
        DurationsFrame = durationsFrame;
        ReqsFrame = reqsFrame;
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
        Counters: CountersStats;
        Durations: DurationStats;
    }

let analyzeCase (case: string) =
    let countersPath = sprintf "samples/%s_counters.csv" case |> fullPath
    let durationsPath = sprintf "samples/%s_durations.csv" case |> fullPath
    {
        Counters = analyzeCounters countersPath;
        Durations = analyzeDurations durationsPath;
    }

let compareCases (cases: seq<string>) =
    cases
    |> Seq.map (fun c -> c, analyzeCase c)
    |> Series.ofObservations



let getSeconds (ts: TimeSpan) = ts.TotalSeconds

let toNamedScatter (k: string, s: Series<'x, 'y>) =
    s
    |> Series.toScatterWith (fun sc -> sc.name <- k)

let toNamedHistogram nbinsx (k: string, s: Series<'x, 'y>) =
    let x = Series.values s
    Graph.Histogram(x = x, name = k, nbinsx = nbinsx)

let toScatters (s: Series<string, Series<TimeSpan, float>>) =
    s
    |> Series.mapValues (Series.mapKeys getSeconds)
    |> Series.observations
    |> Seq.map toNamedScatter

let plotEventsProcessed cases =
    cases
    |> Series.mapValues (fun v -> v.Counters.EventsProcessedSeries)
    |> toScatters
    |> Chart.Plot

let plotEventsPerSec cases =
    cases
    |> Series.mapValues (fun v -> v.Counters.EventsPerSecSeries)
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
    |> toScatters
    |> Chart.Plot
    |> Chart.WithLayout layout

let plotHistograms cases =
    let overlayedLayout =
        Layout(barmode = "overlay")
    cases
    |> Series.mapValues (fun v -> v.Durations.ReqsHistogramSeries)
    |> Series.mapValues (Series.mapValues snd)
    |> Series.observations
    |> Seq.map (toNamedHistogram 1000)
    |> Chart.Plot
    |> Chart.WithLayout overlayedLayout

let s1 = ["s1b2"; "s1b4"; "s1b8"]
let b2 = ["s1b2"; "s2b2"; "s4b2"]
let b4 = ["s1b4"; "s2b4"; "s4b4"]
let b8 = ["s1b8"; "s2b8"; "s4b8"]

let s1Stats = compareCases s1
plotEventsProcessed s1Stats
plotEventsPerSec s1Stats
plotReqPercentiles s1Stats
plotHistograms s1Stats

let b2Stats = compareCases b2
plotReqPercentiles b2Stats
plotEventsPerSec b2Stats
plotHistograms b2Stats
plotEventsProcessed b2Stats

let b4Stats = compareCases b4
plotHistograms b4Stats
plotReqPercentiles b4Stats
plotEventsPerSec b4Stats
plotEventsProcessed b4Stats

let b8Stats = compareCases b8
plotHistograms b8Stats
plotReqPercentiles b8Stats
plotEventsPerSec b8Stats
plotEventsProcessed b8Stats
