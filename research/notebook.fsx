#load "packages/FsLab/Themes/AtomChester.fsx"
#load "packages/FsLab/FsLab.fsx"

open System
open System.IO
open System.Collections.Generic

open FsLab
open FSharp.Data
open XPlot.GoogleCharts
open Deedle
open MathNet.Numerics.Statistics

type Log = JsonProvider<"D:/Personal/Conreign/research/bot-sample.log.json">

let percentile p (s: Series<'a, float>) =
    s.Values.Percentile(p)

let IsDuration (log: Log.Root) = log.Properties.TimedOperationElapsedInMs.IsSome
let IsCounter (log: Log.Root) = log.Properties.CounterName.IsSome

let MapDurationName (log: Log.Root) = log.Properties.TimedOperationDescription.Value

let ExtractDurationSeries (items: seq<Log.Root>)  =
    items |> Seq.filter IsDuration
          |> Seq.mapi (fun i x -> i, x.Properties.TimedOperationElapsedInMs.Value)
          |> series

let ExtractCounterSeries (items: seq<Log.Root>) =
    items |> Seq.filter IsCounter
          |> Seq.distinctBy (fun i -> i.Timestamp)
          |> Seq.map (fun i -> i.Timestamp, i.Properties.CounterValue.Value)
          |> series

let logs = Log.Load(@"D:\Personal\Conreign\research\samples\4x16x16-bot.log.json")

let durations =
    logs |> Seq.filter IsDuration
         |> Seq.groupBy MapDurationName
         |> Seq.map (fun (name, values) -> (name, ExtractDurationSeries values))
         |> Frame.ofColumns

let durationNames = durations.ColumnKeys |> Seq.toList

let signalRSeries =
    durations.GetColumn<float>("SignalR.Send")
        |> Series.dropMissing
        |> Series.windowInto 200 (percentile 95)

let botHandleSeries =
    durations.GetColumn<float>("Bot.Handle")
        |> Series.dropMissing
        |> Series.windowInto 100 (percentile 95)

botHandleSeries |> Chart.Line |> Chart.WithSize (1600, 900)

    
