#r "./../packages/FAKE/tools/FakeLib.dll"

#load "vars.fsx"
#load "config.fsx"
#load "build.fsx"
#load "azure.fsx"

open System
open System.IO
open Fake
open Fake.MSBuildHelper
open Fake.TargetHelper
open Fake.ZipHelper

open Vars
open Config
open Build
open Azure

let azureCreds = 
    {
        SubscriptionId = DefaultConfig.Azure.SubscriptionId;
        CertificateFilePath = DefaultConfig.Azure.CertificateFilePath;
        CertificatePassword = DefaultConfig.Azure.CertificatePassword;
    }

let envOptions = DefaultConfig.Environments.[TargetEnvironment]

let shouldPackageBackend = not <| Uri.IsWellFormedUriString(BackendDeploymentPackagePath, UriKind.Absolute) || File.Exists(BackendDeploymentPackagePath)

Target "clean" (fun _ ->
    CleanDir BuildDir
)

Target "build-loadtest" (fun _ -> 
    MSBuildRelease LoadTestBuildDir "Build" [LoadTestProjectFile] |> Log "[BUILD] "
)

Target "package-loadtest"  (fun _ ->
    let files = !! (LoadTestBuildDir @@ "**/*")
    Zip LoadTestBuildDir LoadTestArtifactPath files
)

Target "package-backend" (fun _ ->
    let path = PackageCloudService BackendProjectFile BackendProjectBuildDir BackendProjectVersion
    tracefn "[PACKAGE] Packed backend to %s." path
)

Target "deploy-backend" (fun _ ->
    let version = ExtractVersionFromCloudServicePackage BackendDeploymentPackagePath
    tracefn "[DEPLOY] Package version is %s." version
    let opts = 
        {
            Credentials = azureCreds;
            PackagePath = BackendDeploymentPackagePath;
            StorageConnectionString = envOptions.StorageConnectionString;
            Deployment =    
                {
                    ServiceName = envOptions.CloudServiceName;
                    Slot = TargetSlot;
                };
            ConfigurationFilePath = Vars.BackendConfigurationFile;
            Label = sprintf "conreign-api_%s" version;
        }
    Deploy opts
)

Target "delete-backend" (fun _ ->
    let deployment = { ServiceName = envOptions.CloudServiceName; Slot = TargetSlot }
    let opts = { Credentials = azureCreds; Deployment = deployment }
    Azure.DeleteCloudService opts
)

"build-loadtest" ==> "package-loadtest"
"clean" ==> "package-backend" =?> ("deploy-backend", shouldPackageBackend)

Target "help" PrintTargets

RunTargetOrDefault "help"