#r "System.Globalization"
#r "System.IO"
#r "./../packages/build/FAKE/tools/FakeLib.dll"
#r "./../packages/build/FSharp.Data/lib/net40/FSharp.Data.dll"

open FSharp.Data
open System
open System.IO
open Fake
open Fake.FileSystemHelper

type JsonConfigFile = JsonProvider<"../versions.json">

let ToolsDir = __SOURCE_DIRECTORY__
let SolutionDir = ToolsDir @@ ".." |> FullName
let SourceDir = SolutionDir @@ "src"
let BuildDir = SolutionDir @@ "build"
let ConfigDir = SolutionDir @@ "config"

let Versions = JsonConfigFile.GetSample()
let VersionSeparator = "__"
let TargetEnvironment = getBuildParamOrDefault "Env" "production"
let TargetSlot = getBuildParamOrDefault "Slot" "production"
let GitHash = Fake.Git.Information.getCurrentSHA1(SolutionDir)
let Timestamp = DateTime.Now.ToString("s").Replace(":", "-").Replace("T", "__")

let BackendProjectName = "Conreign.Server.Host.Azure"
let BackendProjectFile = !! (SourceDir @@ BackendProjectName @@ "*.ccproj") |> Seq.head
let BackendProjectBuildDir = BuildDir @@ BackendProjectName
let BackendProjectVersion = sprintf "%s-%s__%s" Versions.Api GitHash Timestamp
let BackendDefaultDeploymentPackagePath = BackendProjectBuildDir + "app.publish" @@ (sprintf "%s__%s.cspkg" BackendProjectName BackendProjectVersion)
let BackendDeploymentPackagePath = getBuildParamOrDefault "BackendDeploymentPackagePath" BackendDefaultDeploymentPackagePath
let BackendDeployLatestExisting = getEnvironmentVarAsBoolOrDefault "BackendDeployLatestExisting" false
let BackendConfigurationFile = ConfigDir @@ (sprintf "backend.%s.secrets.cscfg" TargetEnvironment)

let LoadTestProjectName = "Conreign.LoadTest"
let LoadTestProjectFile = !! (SourceDir @@ LoadTestProjectName @@ "*.csproj") |> Seq.head
let LoadTestProjectVersion = sprintf "%s-%s__%s" Versions.Loadtest GitHash Timestamp
let LoadTestBuildDir = BuildDir @@ LoadTestProjectName
let LoadTestArtifactPath = BuildDir @@ (sprintf "%s__%s.zip" LoadTestProjectName LoadTestProjectVersion)