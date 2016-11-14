#r "./../packages/build/FAKE/tools/FakeLib.dll"
#r "./../packages/WindowsAzure.Storage.7.2.0/lib/net40/Microsoft.WindowsAzure.Storage.dll"

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
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Blob

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

let shouldPackageBackend = 
    [
        Uri.IsWellFormedUriString(BackendDeploymentPackagePath, UriKind.Absolute)
        File.Exists(BackendDeploymentPackagePath)
        BackendDeployLatestExisting
    ]|> List.filter (fun x -> x) |> List.isEmpty


let ListExistingBackendPackages() = 
    let account = CloudStorageAccount.Parse(envOptions.StorageConnectionString)
    let client = account.CreateCloudBlobClient()
    let container = client.GetContainerReference("artifacts")
    if container.Exists() 
        then container.ListBlobs("conreign/conreign-api") 
            |> Seq.filter (fun i -> i :? CloudBlockBlob)
            |> Seq.cast<CloudBlockBlob>
            |> Seq.toList 
        else []

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
    let packagePathOption = 
        if BackendDeployLatestExisting then 
            ListExistingBackendPackages()
                |> List.filter (fun i -> i.Properties.LastModified.HasValue)
                |> List.sortByDescending (fun i -> i.Properties.LastModified.Value)
                |> List.map (fun i -> i.StorageUri.PrimaryUri.AbsoluteUri)
                |> List.tryHead
        else Some(BackendDeploymentPackagePath)
    match packagePathOption with 
        | None -> 
            traceError "Existing backend packages were not found."
            failwith "Failed to deploy backend."
        | Some(packagePath) -> 
            let version = ExtractVersionFromCloudServicePackage (packagePath)
            tracefn "[DEPLOY] Package version is %s." version
            let opts = 
                {
                    Credentials = azureCreds;
                    PackagePath = packagePath;
                    StorageConnectionString = envOptions.StorageConnectionString;
                    Deployment =    
                        {
                            ServiceName = envOptions.CloudServiceName;
                            Slot = TargetSlot;
                        };
                    ConfigurationFilePath = Vars.BackendConfigurationFile;
                    Label = sprintf "conreign-api_%s" version;
                }
            DeployCloudService opts
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