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
#r "./../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

open System
open System.Collections.Generic
open Fake
open Fake.FileSystemHelper
open FSharp.Data
open Microsoft.Extensions.Configuration
open Newtonsoft.Json

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
    member val IisSiteName: string = "Conreign" with get, set
    member val IisUsername: string = "conreign" with get, set
    member val IisPassword: string = null with get, set
    member val IisHost: string = null with get, set

type public CliConfiguration() =
    member val Azure = new AzureConfiguration() with get, set
    member val StorageConnectionString: string = null with get, set
    member val TransferParallelConnections: Nullable<int> = Nullable() with get, set
    member this.TransferParallelConnectionsOption 
        with get() : option<int> = 
            if this.TransferParallelConnections.HasValue then Some(this.TransferParallelConnections.Value) else None
    member val Environments = new Dictionary<string, EnvironmentConfiguration>(System.StringComparer.OrdinalIgnoreCase) with get, set

let LoadConfig baseConfigPath = 
    let builder = new ConfigurationBuilder()
    let config = 
        builder.SetBasePath(baseConfigPath)
               .AddJsonFile("cli.config.json")
               .AddJsonFile("cli.user.config.json", true)
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
let TargetEnvironmentConfig = DefaultConfig.Environments.[TargetEnvironment]
let TargetSlot = getBuildParamOrDefault "Slot" "production"
let GitHash = Fake.Git.Information.getCurrentHash()
let Timestamp = DateTime.Now.ToString("s").Replace(":", "-").Replace("T", "--")
let private IsCleanWorkingCopy = Fake.Git.Information.isCleanWorkingCopy SolutionDir
let BuildVersion = 
    if IsCleanWorkingCopy then  GitHash
    else sprintf "%s.%s" GitHash Timestamp

let private FormatVersion version =
    sprintf "%s+build.%s" version BuildVersion

let private FormatArtifactFileName extension projectName version =
    sprintf "%s__%s%s" projectName version extension

let private FormatZipArtifactFileName = FormatArtifactFileName ".zip"

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
let SiloVersion = FormatVersion Versions.Silo
let SiloBuildDir = BuildDir @@ SiloLabel
let SiloArtifactPath = BuildDir @@ FormatZipArtifactFileName SiloLabel SiloVersion
let SiloConfigFiles = SetBaseDir ConfigDir (!!(ConfigDir @@ "silo*"))

let ApiProjectName = "Conreign.Server.Host.Console.Api"
let ApiLabel = "conreign-api"
let ApiProjectFile = FindCsProj ApiProjectName
let ApiVersion = FormatVersion Versions.Api
let ApiBuildDir = BuildDir @@ ApiLabel
let ApiArtifactPath = BuildDir @@ FormatZipArtifactFileName ApiLabel ApiVersion
let ApiConfigFiles = SetBaseDir ConfigDir (!!(ConfigDir @@ "api*"))

let YamsProjectName = "Conreign.Server.Host.Yams"
let YamsLabel = "conreign-yams"
let YamsProjectFile = FindCsProj YamsProjectName
let YamsVersion = FormatVersion Versions.Yams
let YamsBuildDir = BuildDir @@ YamsLabel

let JsClientLabel = "conreign-web"
let JsClientProjectName = "Conreign.Client.Web"
let JsClientProjectDir = SourceDir @@ JsClientProjectName
let JsClientBuildDir = JsClientProjectDir @@ "build"
let JsClientOutputBuildDir = BuildDir @@ JsClientLabel

let DeployClusterId = String.Join("-", [| "conreign"; TargetEnvironment |])