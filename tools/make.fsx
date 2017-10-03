#r "./../packages/build/FAKE/tools/FakeLib.dll"

#load "config.fsx"
#load "tasks.fsx"

open System
open System.IO
open Fake
open Fake.MSBuildHelper
open Fake.TargetHelper
open Fake.ZipHelper

open Config
open Tasks

let azureCreds = 
    {
        SubscriptionId = DefaultConfig.Azure.SubscriptionId;
        CertificateFilePath = DefaultConfig.Azure.CertificateFilePath;
        CertificatePassword = DefaultConfig.Azure.CertificatePassword;
    }

let envOptions = DefaultConfig.Environments.[TargetEnvironment]

let shouldCreateApiAzurePackage = 
    [
        Uri.IsWellFormedUriString(AzureDeploymentPackagePath, UriKind.Absolute)
        File.Exists(AzureDeploymentPackagePath)
        AzureDeployLatestExisting
    ]|> List.exists id |> not

Target "clean" (fun _ ->
    CleanDir BuildDir
)

Target "build-load-test" (fun _ -> 
    let props = [("Configuration", "Release"); ("OutputPath", LoadTestBuildDir)]
    let setParams (defaults: MSBuildParams) = 
        {defaults with
            Targets = ["Build"]
            Properties = props
            Verbosity = Some(Minimal)
        }
    build setParams LoadTestProjectFile
)

Target "package-load-test"  (fun _ ->
    let files = !! (LoadTestBuildDir @@ "**/*")
    Zip LoadTestBuildDir LoadTestArtifactPath files
)

Target "build-silo" (fun _ ->
    let options = 
        {DefaultBuildOptions with
            Name = SiloLabel
            ProjectPath = SiloProjectFile
            OutputPath = SiloBuildDir
            Version = SiloVersion
        }
    BuildProject options
)

Target "package-silo" (fun _ ->
    PackageProject SiloBuildDir BuildDir
)

Target "deploy-silo" (fun _ -> 
    let options = 
        {
            PackagePath = Some(SiloArtifactPath);
            AdditionalFiles = [SiloConfigFiles];
            TempDirectory = TempDir;
            StorageConnectionString = TargetEnvironmentConfig.StorageConnectionString;
            ClusterId = DeployClusterId;
            Environment = TargetEnvironment;
            ParallelConnections = DefaultConfig.TransferParallelConnectionsOption
        }
    DeployPackageToYams options |> ignore
)

Target "build-api" (fun _ ->
    let options = 
        {DefaultBuildOptions with
            Name = ApiLabel
            ProjectPath = ApiProjectFile
            OutputPath = ApiBuildDir
            Version = ApiVersion
        }
    BuildProject options
)

Target "package-api" (fun _ ->
    PackageProject ApiBuildDir BuildDir
)

Target "deploy-api" (fun _ -> 
    let options = 
        {
            PackagePath = Some(ApiArtifactPath)
            AdditionalFiles = [ApiConfigFiles]
            TempDirectory = TempDir
            StorageConnectionString = TargetEnvironmentConfig.StorageConnectionString
            ClusterId = DeployClusterId
            Environment = TargetEnvironment
            ParallelConnections = DefaultConfig.TransferParallelConnectionsOption
        }
    DeployPackageToYams options |> ignore
)

Target "build-web" (fun _ ->
    CleanDir JsClientOutputBuildDir
    let options = 
        {
            ProjectDirectory = JsClientProjectDir
            ProjectBuildDirectory = JsClientBuildDir
            OutputDirectory = JsClientOutputBuildDir
            BuildCommand = "build"
        }
    BuildJsProject options
)

Target "deploy-web" (fun _ ->
    let options = {
        SourcePath = JsClientOutputBuildDir
        Host = TargetEnvironmentConfig.IisHost
        WebsiteName = TargetEnvironmentConfig.IisSiteName
        Username = TargetEnvironmentConfig.IisUsername
        Password = TargetEnvironmentConfig.IisPassword
    }
    DeployStaticWebsite options
)

Target "build-yams" (fun _ ->
    let options = 
        {DefaultBuildOptions with
            Name = YamsLabel
            ProjectPath = YamsProjectFile
            OutputPath = YamsBuildDir
            Version = YamsVersion
        }
    BuildProject options
)

Target "package-backend-azure" (fun _ ->
    let path = 
        PackageCloudService 
            AzureProjectFile 
            AzureProjectBuildDir
            AzureProjectPackageDir
            AzureLabel
            AzureProjectVersion
    tracefn "Created azure cloud service package at %s." path
)

Target "deploy-backend-azure" (fun _ ->
    let packagePath = if AzureDeployLatestExisting then null else AzureDeploymentPackagePath
    let label = "conreign-backend"
    let options = 
        {
            Credentials = azureCreds;
            PackagePath = packagePath;
            StorageConnectionString = envOptions.StorageConnectionString;
            Deployment =    
                {
                    ServiceName = envOptions.CloudServiceName;
                    Slot = TargetSlot;
                };
            ConfigurationFilePath = AzureConfigurationFile;
            Label = label;
        }
    DeployCloudService options  
)

Target "delete-backend-azure" (fun _ ->
    let deployment = { ServiceName = envOptions.CloudServiceName; Slot = TargetSlot }
    let opts = { Credentials = azureCreds; Deployment = deployment }
    DeleteCloudService opts
)

"build-load-test" ==> "package-load-test"
"package-backend-azure" =?> ("deploy-backend-azure", shouldCreateApiAzurePackage)
"build-silo" ==> "package-silo" ==> "deploy-silo"
"build-api" ==> "package-api" ==> "deploy-api"
"build-web" ==> "deploy-web"

Target "help" PrintTargets

RunTargetOrDefault "help"