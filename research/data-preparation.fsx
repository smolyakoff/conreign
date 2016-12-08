#r "packages/DotNetZip/lib/net20/DotNetZip.dll"

#load "packages/FsLab/FsLab.fsx"

open System
open System.IO
open System.Collections.Generic

open FSharp.Data
open FSharp.Data.JsonExtensions
open Ionic.Zip

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let hasProperty (jsonVal: JsonValue) name =
    match jsonVal with
        | JsonValue.Record(props) when props |> Seq.exists (fun (k, v) -> k = name)
            -> true
        | _ -> false

let hasPropertiesProperty (log: JsonValue) = hasProperty log "Properties"
let isDuration (log: JsonValue) =
    hasPropertiesProperty log && hasProperty log?Properties "TimedOperationElapsedInMs"
let isCounter (log: JsonValue) =
    hasPropertiesProperty log && hasProperty log?Properties "CounterName"
let isGauge (log: JsonValue) =
    hasPropertiesProperty log && hasProperty log?Properties "GaugeName"
let isError (log: JsonValue) =
    let level = log?Level.AsString()
    level = "Error" || level = "Fatal"
let isWarning (log: JsonValue) = log?Level.AsString() = "Warning"

let extractValue (jsonValue: JsonValue) (name: string) defaultValue =
    match jsonValue.TryGetProperty(name) with
        | Some(prop) -> prop.ToString()
        | None -> defaultValue

let extractValueOrEmpty (jsonValue: JsonValue) (name: string) =
    extractValue jsonValue name String.Empty
let extractDuration (log: JsonValue)  =
    let props = log?Properties
    let ts = log?Timestamp.AsDateTime().Ticks
    let items = seq {
        yield log?Timestamp.AsDateTime().Ticks.ToString()
        yield extractValueOrEmpty props "InstanceId"
        yield extractValueOrEmpty props "BotId"
        yield props?TimedOperationDescription.AsString()
        yield extractValueOrEmpty props "EventType"
        yield extractValueOrEmpty props "CommandType"
        yield props?TimedOperationElapsedInMs.AsInteger().ToString()
    }
    (ts, items)

let extractCounter (log: JsonValue) =
    let props = log?Properties
    let ts = log?Timestamp.AsDateTime().Ticks
    let items = seq {
        yield log?Timestamp.AsDateTime().Ticks.ToString()
        yield extractValueOrEmpty props "InstanceId"
        yield extractValueOrEmpty props "BotId"
        yield props?CounterName.AsString()
        yield props?CounterValue.AsInteger().ToString()
    }
    (ts, items)

let extractGauge (log: JsonValue) =
    let props = log?Properties
    let ts = log?Timestamp.AsDateTime().Ticks
    let items = seq {
        yield ts.ToString()
        yield extractValueOrEmpty props "InstanceId"
        yield extractValueOrEmpty props "BotId"
        yield props?GaugeName.AsString()
        yield props?GaugeValue.AsInteger().ToString()
    }
    (ts, items)

let extractFailure (log: JsonValue) =
    let props = log?Properties
    let ts = log?Timestamp.AsDateTime().Ticks
    let items = seq {
        yield ts.ToString()
        yield extractValueOrEmpty props "InstanceId"
        yield log?Level.AsString()
        yield log?RenderedMessage.AsString()
    }
    (ts, items)

let extractRows (headers: seq<string>)
                filter
                (mapper: JsonValue -> int64 * seq<string>)
                (inputPaths: seq<string>)
                (outputPath: string) =
    use writer = new StreamWriter(outputPath)
    writer.WriteLine(String.Join(",", headers))
    let parse (index: int) (line: string) =
        let json = line.TrimEnd(',')
        try
            let log = JsonValue.Parse(json)
            if filter log
                then
                    let (ts, values) = mapper log
                    Some(index, ts, values)
                else None
        with e ->
            printfn "Skipped invalid json line: %O" e
            None
    let read (readers: list<StreamReader>)
             (rows: list<int * int64 * seq<string>>)
             (nextReaderIndex: option<int>) =
        let allRows =
            match nextReaderIndex with
                | None ->
                    readers
                    |> List.mapi
                        (fun i r ->
                            if r.EndOfStream then None
                            else parse i (r.ReadLine()))
                    |> List.filter (fun l -> l.IsSome)
                    |> List.map (fun l -> l.Value)
                | Some(i) ->
                    let rowOption = readers.[i].ReadLine() |> parse i
                    match rowOption with
                        | None -> rows
                        | Some(row) -> row :: rows
        // Find min row
        let mapTs row =
            let (i, ts, vals) = row
            ts
        let minTsRowOption = allRows |> List.sortBy mapTs |> Seq.tryHead
        let outRows =
            match minTsRowOption with
                | Some(minRow) -> allRows |> List.filter (fun row -> row <> minRow)
                | None -> allRows
        let outReaderIndex =
            match minTsRowOption with
                | Some(i, ts, value) when not readers.[i].EndOfStream -> Some(i)
                | _ -> None
        (minTsRowOption, outRows, outReaderIndex)
    let iterate (readers: list<StreamReader>) =
        let mutable currentIndex = None
        let mutable currentRows = []
        while (List.exists (fun (r: StreamReader) -> not r.EndOfStream) readers) do
            let (minRowOption, outRows, outIndex) =
                read (Seq.toList readers) currentRows currentIndex
            match minRowOption with
                | Some(_, _, vals) ->
                    writer.WriteLine(String.Join(",", vals))
                | _ -> ()
            currentRows <- outRows
            currentIndex <- outIndex
        readers
    inputPaths
    |> Seq.map (fun p -> new StreamReader(p))
    |> Seq.toList
    |> iterate
    |> List.iter (fun r -> r.Dispose())

let extractDurations (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "BotId"; "Name"; "EventType"; "CommandType"; "Ms"]
        isDuration
        extractDuration
        inputPaths
        outputPath

let extractCounters (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "BotId"; "Name"; "Value"]
        isCounter
        extractCounter
        inputPaths
        outputPath

let extractGauges (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "BotId"; "Name"; "Value"]
        isGauge
        extractGauge
        inputPaths
        outputPath

let extractFailures (inputPaths: seq<string>) (outputPath: string) =
    extractRows
        ["Timestamp"; "InstanceId"; "Level"; "Message"]
        (fun l -> isError l || isWarning l)
        extractFailure
        inputPaths
        outputPath

let extractZip (zipFilePath: string) =
    let directory = Path.GetDirectoryName(zipFilePath);
    use zipFile = ZipFile.Read(zipFilePath)
    let outputDirectory =
        Path.Combine(directory, Path.GetFileNameWithoutExtension(zipFilePath))
    zipFile.ExtractAll(outputDirectory, ExtractExistingFileAction.OverwriteSilently)
    outputDirectory

let processBotLogs prefix logDirectoryPath outputDirectoryPath =
    let durationsPath = Path.Combine(outputDirectoryPath, prefix + "_durations.csv")
    let countersPath = Path.Combine(outputDirectoryPath, prefix + "_counters.csv")
    let gaugesPath = Path.Combine(outputDirectoryPath, prefix + "_gauges.csv")
    let failuresPath = Path.Combine(outputDirectoryPath, prefix + "_failures.csv")
    let zipFiles = Directory.GetFiles(logDirectoryPath, "*.zip")
    let outputDirs = zipFiles |> Seq.toList |> List.map extractZip
    let logFiles = Directory.GetFiles(logDirectoryPath, "*.json", SearchOption.AllDirectories)
    extractDurations logFiles durationsPath
    extractCounters logFiles countersPath
    extractGauges logFiles gaugesPath
    extractFailures logFiles failuresPath
    outputDirs |> List.iter (fun d -> Directory.Delete(d, true))

let replaceInFile (source: string) (replacement: string) filePath =
    let lines = File.ReadLines(filePath)
    use ms = new MemoryStream()
    use writer = new StreamWriter(ms)
    lines
    |> Seq.map (fun line -> line.Replace(source, replacement))
    |> Seq.iter writer.WriteLine
    writer.Flush()
    File.WriteAllBytes(filePath, ms.ToArray())

let SMPL0 = "logs/loadtest-sample-0"
let S1B2 = "logs/loadtest-s1b2_a"
let S1B4 = "logs/loadtest-s1b4_1"
let S1B8 = "logs/loadtest-s1b8_a"
let S2B2 = "logs/loadtest-s2b2_a"
let S2B4 = "logs/loadtest-s2b4_1"
let S2B8 = "logs/loadtest-s2b8_a"
let S4B2 = "logs/loadtest-s4b2_1"
let S4B4 = "logs/loadtest-s4b4_1"
let S4B8 = "logs/loadtest-s4b8_1"

//processBotLogs "sample" SMPL0 "samples"
processBotLogs "s1b2" S1B2 "samples"
processBotLogs "s1b4" S1B4 "samples"
processBotLogs "s1b8" S1B8 "samples"
processBotLogs "s2b2" S2B2 "samples"
processBotLogs "s2b4" S2B4 "samples"
processBotLogs "s2b8" S2B8 "samples"
processBotLogs "s4b2" S4B2 "samples"
processBotLogs "s4b4" S4B4 "samples"
processBotLogs "s4b8" S4B8 "samples"

processBotLogs "s2b8_2" "logs/loadtest-s2b8"
