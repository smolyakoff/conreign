#r "System.Globalization"
#r "System.IO"
#r "./../packages/build/FAKE/tools/FakeLib.dll"
#r "./../packages/build/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "./../packages/System.ComponentModel.TypeConverter/lib/net45/System.ComponentModel.TypeConverter.dll"
#r "./../packages/Microsoft.Extensions.Primitives/lib/netstandard1.0/Microsoft.Extensions.Primitives.dll"
#r "./../packages/Microsoft.Extensions.Configuration.Abstractions/lib/netstandard1.0/Microsoft.Extensions.Configuration.Abstractions.dll"
#r "./../packages/Microsoft.Extensions.FileProviders.Abstractions/lib/netstandard1.0/Microsoft.Extensions.FileProviders.Abstractions.dll"
#r "./../packages/Microsoft.Extensions.FileProviders.Physical/lib/net451/Microsoft.Extensions.FileProviders.Physical.dll"
#r "./../packages/Microsoft.Extensions.Configuration.Binder/lib/netstandard1.1/Microsoft.Extensions.Configuration.Binder.dll"
#r "./../packages/Microsoft.Extensions.Configuration/lib/netstandard1.1/Microsoft.Extensions.Configuration.dll"
#r "./../packages/Microsoft.Extensions.Configuration.FileExtensions/lib/net451/Microsoft.Extensions.Configuration.FileExtensions.dll"
#r "./../packages/Microsoft.Extensions.Configuration.Json/lib/net451/Microsoft.Extensions.Configuration.Json.dll"

open System
open System.Collections.Generic
open Fake
open Fake.FileSystemHelper
open FSharp.Data
open Microsoft.Extensions.Configuration

type VersionsFile = JsonProvider<"../versions.json">

let ToolsDir = __SOURCE_DIRECTORY__
let SolutionDir = ToolsDir @@ ".." |> FullName
let SourceDir = SolutionDir @@ "src"
let BuildDir = SolutionDir @@ "build"
let ConfigDir = SolutionDir @@ "config"
let TempDir = SolutionDir @@ "temp"

type public AzureConfiguration() =
    member val SubscriptionId: string = null with get, set
    member val CertificateFilePath: string = ConfigDir @@ "management-certificate.secrets.pfx" with get, set
    member val CertificatePassword: string = null with get, set

type public EnvironmentConfiguration() =
    member val CloudServiceName: string = "conreign" with get, set
    member val StorageConnectionString: string = null with get, set

type public CliConfiguration() =
    member val Azure = new AzureConfiguration() with get, set
    member val Environments = new Dictionary<string, EnvironmentConfiguration>(System.StringComparer.OrdinalIgnoreCase) with get, set

let LoadConfig baseConfigPath = 
    let builder = new ConfigurationBuilder()
    let config = 
        builder.SetBasePath(baseConfigPath)
               .AddJsonFile("cli.config.json")
               .AddJsonFile("cli.secrets.json", true)
               .Build()
    let typedConfig = new CliConfiguration()
    config.Bind(typedConfig)
    typedConfig



let private FindProjectFile projectName extension =
    let pattern = sprintf "*.%s" extension
    !! (SourceDir @@ projectName @@ pattern) |> Seq.head

let private FindCsProj projectName = FindProjectFile projectName "csproj"

let DefaultConfig = LoadConfig ConfigDir
let Versions = VersionsFile.GetSample()
let VersionSeparator = "__"
let TargetEnvironment = getBuildParamOrDefault "Env" "production"
let TargetSlot = getBuildParamOrDefault "Slot" "production"
let GitHash = Fake.Git.Information.getCurrentHash()
let Timestamp = DateTime.Now.ToString("s").Replace(":", "-").Replace("T", "__")

let AzureProjectName = "Conreign.Server.Host.Azure"
let AzureLabel = "conreign-api-azure"
let AzureProjectFile = FindProjectFile AzureProjectName "ccproj"
let AzureProjectBuildDir = BuildDir @@ AzureLabel
let AzureProjectPackageDir = BuildDir @@ AzureLabel + "-package"
let AzureProjectVersion = sprintf "%s-%s__%s" Versions.AzureApi GitHash Timestamp
let AzureDefaultDeploymentPackagePath = AzureProjectBuildDir + "app.publish" @@ (sprintf "%s__%s.cspkg" AzureProjectName AzureProjectVersion)
let AzureDeploymentPackagePath = getBuildParamOrDefault "BackendDeploymentPackagePath" AzureDefaultDeploymentPackagePath
let AzureDeployLatestExisting = getEnvironmentVarAsBoolOrDefault "BackendDeployLatestExisting" false
let AzureConfigurationFile = ConfigDir @@ (sprintf "backend.%s.secrets.cscfg" TargetEnvironment)

let LoadTestProjectName = "Conreign.LoadTest"
let LoadTestLabel = "conreign-load-test"
let LoadTestProjectFile = FindCsProj LoadTestProjectName
let LoadTestProjectVersion = sprintf "%s-%s__%s" Versions.LoadTest GitHash Timestamp
let LoadTestBuildDir = BuildDir @@ LoadTestLabel
let LoadTestArtifactPath = BuildDir @@ (sprintf "%s__%s.zip" LoadTestLabel LoadTestProjectVersion)

let SiloProjectName = "Conreign.Server.Host.Console.Silo"
let SiloLabel = "conreign-silo"
let SiloProjectFile = FindCsProj SiloProjectName
let SiloVersion = sprintf "%s-%s" Versions.Silo GitHash
let SiloBuildDir = BuildDir @@ SiloLabel
let SiloArtifactPath = BuildDir @@ (sprintf "%s__%s.zip" SiloLabel SiloVersion)

let ApiProjectName = "Conreign.Server.Host.Console.Api"
let ApiLabel = "conreign-api"
let ApiProjectFile = FindCsProj ApiProjectName
let ApiVersion = sprintf "%s-%s__%s" Versions.Api GitHash Timestamp
let ApiBuildDir = BuildDir @@ ApiLabel