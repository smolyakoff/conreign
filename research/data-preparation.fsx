#load "./../packages/FsLab/FsLab.fsx"

open System
open System.IO
open System.Collections.Generic

open FsLab
open FSharp.Data

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let FormatLogFileToJsonArrayFile (inputPath: string) (outputPath: string) =
    use reader = new StreamReader(inputPath)
    use writer = new StreamWriter(outputPath)
    writer.WriteLine("[")
    let mutable i = 0;
    while (not reader.EndOfStream) do
        let isFirstEntry = i = 0
        let line = "  " + reader.ReadLine().TrimEnd(',')
        let isLastEntry = reader.EndOfStream
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
                    printfn "Input file is broken."
        i <- i + 1
    writer.WriteLine("]")

FormatLogFileToJsonArrayFile "bot-sample-raw.log.json" "bot-sample.log.json"
