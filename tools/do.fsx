#r "./../packages/FAKE/tools/FakeLib.dll"

#load "vars.fsx"
#load "config.fsx"
#load "build.fsx"
#load "azure.fsx"

open System.IO
open Fake
open Fake.TargetHelper

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

Target "clean" (fun _ ->
    CleanDir BuildDir
)

Target "package-backend" (fun _ ->
    let path = BuildCloudService BackendProjectFile BackendProjectBuildDir BackendProjectVersion
    tracefn "[PACKAGE] Packed backend to %s." path
)

Target "deploy-backend" (fun _ ->
    let version = ExtractVersionFromCloudServicePackage BackendDeploymentPackagePath
    let opts = 
        {
            Credentials = azureCreds;
            PackagePath = BackendDeploymentPackagePath;
            StorageConnectionString = envOptions.StorageConnectionString;
            ServiceName = envOptions.CloudServiceName;
            ConfigurationFilePath = Vars.BackendConfigurationFile;
            Label = sprintf "conreign-api_%s" version;
            Slot = TargetSlot;
        }
    Deploy opts
)

"clean" ==> "package-backend"
"package-backend" =?> ("deploy-backend", not (File.Exists(BackendDeploymentPackagePath)))

Target "help" PrintTargets

RunTargetOrDefault "help"