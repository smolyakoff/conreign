#load "packages/FsLab/FsLab.fsx"
#load "packages/DotNetZip/lib/net20/DotNetZip.dll"

open System
open System.IO
open System.Collections.Generic

open FsLab
open FSharp.Data
open FSharp.Data.JsonExtensions

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let convertLogsToJsonArray (inputPaths: seq<string>) (outputPath: string) =
    use writer = new StreamWriter(outputPath)
    writer.WriteLine("[")
    let mutable recordIndex = 0
    let iterate index (inputPath: string) =
        use reader = new StreamReader(inputPath)
        while (not reader.EndOfStream) do
            let isFirstEntry = recordIndex = 0
            let line = "  " + reader.ReadLine().TrimEnd(',')
            let isLastEntry = reader.EndOfStream && index = Seq.length inputPaths - 1
            match (isFirstEntry, isLastEntry) with
                | true, false ->
                    writer.Write(line)
                | (_, false) ->
                    writer.WriteLine(",")
                    writer.Write(line)
                | (_, true) ->
                    try
                        JsonValue.Parse(line) |> ignore
                        writer.Write(",")
                        writer.WriteLine(line)
                    with _ ->
                        writer.WriteLine()
                        printfn "Input file %s is broken." inputPath
            recordIndex <- recordIndex + 1
    inputPaths |> Seq.iteri iterate
    writer.WriteLine("]")

let hasProperty (jsonVal: JsonValue) name =
    match jsonVal with
        | JsonValue.Record(props) when props |> Seq.exists (fun (k, v) -> k = "name")
            -> true
        | _ -> false

let isDuration (log: JsonValue) = hasProperty log?Properties "TimedOperationDurationInMs"
let isCounter (log: JsonValue) = hasProperty log?Properties "CounterName"
let isGauge (log: JsonValue) = hasProperty log?Properties "GaugeName"
let isError (log: JsonValue) = log?Level.AsString() = "Error"
let isWarning (log: JsonValue) = log?Level.AsString() = "Warning"

let extractDuration (log: JsonValue)  =
    let props = log?Properties
    seq {
        yield log?Timestamp.AsDateTime().Ticks :> Object
        yield props?InstanceId.AsString() :> Object
        yield props?TimedOperationDescription.AsString() :> Object
        yield match props?EventType with
                | JsonValue.String(s) -> s :> Object
                | _ -> String.Empty :> Object
        yield match props?CommandType with
                | JsonValue.String(s) -> s :> Object
                | _ -> String.Empty :> Object
        yield props?TimedOperationElapsedInMs.AsInteger() :> Object
    }

let extractCounter (log: JsonValue) =
    let props = log?Properties
    seq {
        yield log?Timestamp.AsDateTime().Ticks :> Object
        yield props?InstanceId.AsString() :> Object
        yield props?CounterName.AsString() :> Object
        yield props?CounterValue.AsInteger() :> Object
    }

let extractGauge (log: JsonValue) =
    let props = log?Properties
    seq {
        yield log?Timestamp.AsDateTime().Ticks :> Object
        yield props?InstanceId.AsString() :> Object
        yield props?GaugeName.AsString() :> Object
        yield props?GaugeValue.AsInteger() :> Object
    }

let extractFailure (log: JsonValue) =
    let props = log?Properties
    seq {
        yield log?Timestamp.AsDateTime().Ticks :> Object
        yield log?Level.AsString() :> Object
        yield props?InstanceId.AsString() :> Object
        yield log?RenderedMessage.AsString() :> Object
    }

let extractRows (headers: seq<string>)
                filter
                (mapper: JsonValue -> seq<obj>)
                (inputPaths: seq<string>)
                (outputPath: string) =
    use writer = new StreamWriter(outputPath)
    writer.WriteLine(String.Join(",", headers))
    let extract (inputPath: string) =
        use reader = new StreamReader(inputPath)
        while (not reader.EndOfStream) do
            let json = reader.ReadLine().TrimEnd(',')
            try
                let log = JsonValue.Parse(json)
                if filter log then
                    let row = mapper log
                    writer.WriteLine(String.Join(",", row))
            with e ->
                printfn "Skipped invalid json line: %O" e
    inputPaths |> Seq.iter extract

let extractDurations (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "Name"; "EventType"; "CommandType"; "Ms"]
        isDuration
        extractDuration
        inputPaths
        outputPath

let extractCounters (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "Name"; "Value"]
        isCounter
        extractCounter
        inputPaths
        outputPath

let extractGauges (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "Name"; "Value"]
        isGauge
        extractGauge
        inputPaths
        outputPath

let extractFailures (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "Level"; "Message"]
        (fun l -> isError l || isWarning l)
        extractGauge
        inputPaths
        outputPath

let extractZipToSameDirectory zipFilePath =
    failwith "Not Implemented"
    
let processBotLogs prefix logDirectoryPath outputDirectoryPath =
    let durationsPath = Path.Combine(outputDirectoryPath, prefix + "_durations.csv")
    let countersPath = Path.Combine(outputDirectoryPath, prefix + "_counters.csv")
    let gaugesPath = Path.Combine(outputDirectoryPath, prefix + "_gauges.csv")
    let failuresPath = Path.Combine(outputDirectoryPath, prefix + "_failures.csv")
    let logFiles = Directory.GetFiles(logDirectoryPath, "*.json")
    extractDurations logFiles durationsPath
    extractCounters logFiles countersPath
    extractGauges logFiles gaugesPath
    extractFailures logFiles failuresPath

let T4x16x16 = Directory.GetFiles("logs/loadtest-16-11-14__06-49-57")

ExtractDurations T4x16x16 "samples/4x16x16-durations.csv"
