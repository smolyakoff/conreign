#r "./../packages/FAKE/tools/FakeLib.dll"

open System
open System.IO
open Fake
open Fake.MSBuild
open Fake.TraceHelper

let BuildCloudService projectPath outputPath version =
    let props = 
        [
            ("Configuration", "Release");
            ("TargetProfile", "Cloud")
        ]
    MSBuild outputPath "Publish" props [projectPath] |> Log "[BUILD] "
    let path = !! (outputPath + "app.publish" @@ "*.cspkg") |> Seq.head
    let info = new FileInfo(path)
    let newPath = info.DirectoryName @@ (sprintf "%s__%s.cspkg" (Path.GetFileNameWithoutExtension(info.Name)) version)
    Fake.FileHelper.Rename newPath path
    newPath

let ExtractVersionFromCloudServicePackage packagePath =
    let name = Path.GetFileNameWithoutExtension(packagePath)
    let parts = List.ofArray (name.Split([|"__"|], StringSplitOptions.RemoveEmptyEntries))
    match parts with 
      | head :: version -> String.Join("__", version)
      | _ -> sprintf "Cannot extract version from %s" packagePath |> failwith