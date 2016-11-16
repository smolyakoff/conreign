#load "packages/FsLab/FsLab.fsx"

open System
open System.IO
open System.Collections.Generic

open FsLab
open FSharp.Data
open FSharp.Data.JsonExtensions

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let ConvertLogsToJsonArray (inputPaths: seq<string>) (outputPath: string) =
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

let IsDuration (log: JsonValue) =
    let props = log?Properties
    match props with
        | JsonValue.Record(props) when props |> Seq.exists (fun (k, v) -> k = "TimedOperationElapsedInMs")
            -> true
        | _ -> false

let ExtractDuration (log: JsonValue)  =
    let props = log?Properties
    seq {
        yield log?Timestamp.AsDateTime().Ticks :> Object
        yield props?InstanceId.AsString() :> Object
        yield props?TimedOperationDescription.AsString() :> Object
        yield props?TimedOperationElapsedInMs.AsInteger() :> Object
    }

let ExtractRows (headers: seq<string>) 
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

let ExtractDurations (inputPaths: seq<string>) (outputPath: string)= 
    ExtractRows ["Timestamp"; "InstanceId"; "Name"; "Ms"] IsDuration ExtractDuration inputPaths outputPath

let T4x16x16 = Directory.GetFiles("logs/loadtest-16-11-14__06-49-57")

ExtractDurations T4x16x16 "samples/4x16x16-durations.csv"


//FormatLogFileToJsonArrayFile "bot-sample-raw.log.json" "bot-sample.log.json"

ConvertLogsToJsonArray
    [@"logs\loadtest-16-11-14__06-49-57\loadtest__16-11-14__06-49-57__3-log.json"]
    "samples/4x16x16-bot.log.json"
