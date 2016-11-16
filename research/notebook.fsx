#load "packages/FsLab/Themes/AtomChester.fsx"
#load "packages/FsLab/FsLab.fsx"

open System
open System.IO
open System.Collections.Generic

open FsLab
open FSharp.Data
//open XPlot.GoogleCharts
open FSharp.Charting
open Deedle
open MathNet.Numerics.Statistics

let percentile p (s: Series<'a, float>) =
    s.Values.Percentile(p)

//type Log = JsonProvider<"D:/Personal/Conreign/research/bot-sample.log.json">
type DurationLog = CsvProvider<"D:/Personal/Conreign/research/samples/sample-durations.csv">
let FullPath path = Path.Combine(__SOURCE_DIRECTORY__, path)

let durationLog = DurationLog.Load(FullPath "samples/4x16x16-durations.csv")

let histograms =
    durationLog.Rows
    |> Seq.map (fun x -> x.Name, x.Ms)
    |> Seq.groupBy fst
    //|> Seq.map (fun (k, vs) ->  vs |> Chart.Histogram |> Chart.WithTitle k)
    |> Seq.toList

histograms.[0]
histograms.[1]
histograms.[2]

let createDurationSeries (agg: seq<float> -> float) (logs: seq<DurationLog.Row>)  =
    logs
    |> Seq.map (fun x -> x.Timestamp, x.Ms)
    |> Seq.groupBy fst
    |> Seq.map (fun (t, vs) -> new DateTime(t), vs |> Seq.map (snd >> float) |> agg)
    |> Seq.sortBy fst
    |> Series.ofObservations

let durationSeries =
    durationLog.Rows
    |> Seq.groupBy (fun x -> x.Name)
    |> Series.ofObservations
    |> Series.mapValues (createDurationSeries (fun p -> p.Percentile(95)))

let signalR = durationSeries |> Series.get "SignalR.Send";

signalR
|> Series.observations
|> Chart.FastLine

signalR
|> Series.windowDistInto (TimeSpan(0, 0, 10)) (percentile 90)
|> Series.observations
|> Chart.FastLine



//percentiles
//percentiles |> Frame.ofColumns |> Chart.Line

let durations =
    Frame.ReadCsv (FullPath "samples/4x16x16-durations.csv")
    |> Frame.sortRows "Timestamp"

durations



//let percentile p (s: Series<'a, float>) =
//    s.Values.Percentile(p)
//
//let IsDuration (log: Log.Root) = log.Properties.TimedOperationElapsedInMs.IsSome
//let IsCounter (log: Log.Root) = log.Properties.CounterName.IsSome
//
//let MapDurationName (log: Log.Root) = log.Properties.TimedOperationDescription.Value
//let MapDurationValue (log: Log.Root) = log.Properties.TimedOperationElapsedInMs.Value
//
//let ExtractDurationSeries (items: seq<Log.Root>)  =
//    items |> Seq.filter IsDuration
//          |> Seq.groupBy (fun l -> l.Timestamp)
//          |> Seq.map (fun (t, values) -> t, values |> Seq.map MapDurationValue |> Seq.map float |> (fun s -> s.Percentile(95)))
//          |> Seq.sortBy fst
//          |> series
//
//let ExtractCounterSeries (items: seq<Log.Root>) =
//    items |> Seq.filter IsCounter
//          |> Seq.distinctBy (fun i -> i.Timestamp)
//          |> Seq.map (fun i -> i.Timestamp, i.Properties.CounterValue.Value)
//          |> series
//
//let logs = Log.Load(@"D:\Personal\Conreign\research\samples\4x16x16-bot.log.json")
//
//let durations =
//    logs |> Seq.filter IsDuration
//         |> Seq.groupBy MapDurationName
//         |> Seq.map (fun (name, values) -> (name, ExtractDurationSeries values))
//         |> Frame.ofColumns
//
//let durationNames = durations.ColumnKeys |> Seq.toList
//
//let signalRSeries =
//    durations.GetColumn<float>("SignalR.Send")
//        |> Series.dropMissing
//        |> Series.windowInto 200 (percentile 95)
//
//let botHandleSeries =
//    durations.GetColumn<float>("Bot.Handle")
//        |> Series.dropMissing
//        |> Series.windowInto 1000 (percentile 95)
//
//botHandleSeries |> Chart.Line
//
//logs |> Seq.filter IsDuration
//     |> Seq.mapi (fun i l -> (i.ToString()), MapDurationValue l)
//     |> Seq.filter (fun (k, v) -> v < 1800)
//     |> Series.ofObservations
//     |> Chart.Histogram
