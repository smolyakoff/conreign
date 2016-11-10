#r "System.Threading.Tasks"
#r "System.Net.Http"
#r "./../packages/Microsoft.Bcl.Async/lib/portable-net45+win8+wpa81/Microsoft.Threading.Tasks.dll"
#r "./../packages/System.IO.FileSystem.Primitives/lib/net46/System.IO.FileSystem.Primitives.dll"
#r "./../packages/System.IO.FileSystem/lib/net46/System.IO.FileSystem.dll"
#r "./../packages/System.Security.Cryptography.Algorithms/lib/net46/System.Security.Cryptography.Algorithms.dll"
#r "./../packages/System.Security.Cryptography.Primitives/lib/net46/System.Security.Cryptography.Primitives.dll"
#r "./../packages/FAKE/tools/FakeLib.dll"
#r "./../packages/Hyak.Common/lib/net45/Hyak.Common.dll"
#r "./../packages/Microsoft.Azure.Common/lib/net45/Microsoft.Azure.Common.dll"
#r "./../packages/Microsoft.Azure.Common/lib/net45/Microsoft.Azure.Common.NetFramework.dll"
#r "./../packages/WindowsAzure.Storage.7.2.0/lib/net40/Microsoft.WindowsAzure.Storage.dll"
#r "./../packages/Microsoft.Azure.Storage.DataMovement/lib/net45/Microsoft.WindowsAzure.Storage.DataMovement.dll"
#r "./../packages/Microsoft.IdentityModel.Clients.ActiveDirectory/lib/net45/Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
#r "./../packages/Microsoft.IdentityModel.Clients.ActiveDirectory/lib/net45/Microsoft.IdentityModel.Clients.ActiveDirectory.Platform.dll"
#r "./../packages/Microsoft.WindowsAzure.Management.Compute/lib/portable-net45+wp8+wpa81+win/Microsoft.WindowsAzure.Management.Compute.dll"

open Fake
open Fake.TraceHelper
open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Security.Cryptography.X509Certificates
open Hyak.Common
open Microsoft.Azure
open Microsoft.IdentityModel.Clients.ActiveDirectory
open Microsoft.IdentityModel.Clients.ActiveDirectory.Native
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Blob
open Microsoft.WindowsAzure.Storage.DataMovement
open Microsoft.WindowsAzure.Management.Compute
open Microsoft.WindowsAzure.Management.Compute.Models

System.Net.ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount * 8;
System.Net.ServicePointManager.Expect100Continue = false;

type AzureCredentials = 
    {
        SubscriptionId: string;
        CertificateFilePath: string; 
        CertificatePassword: string;
    }

type Deployment = 
    {
        ServiceName: string;
        Slot: string;
    }

type DeploymentOptions = 
    { 
        Credentials: AzureCredentials;
        StorageConnectionString: string;
        Label: string;
        Deployment: Deployment;
        ConfigurationFilePath: string;
        PackagePath: string;
    }

type StopOptions = 
    {
        Credentials: AzureCredentials;
        Deployment: Deployment;
    }

let ParseSlot slot = 
    Enum.Parse(typedefof<DeploymentSlot>, slot, true) :?> DeploymentSlot

let Authenticate (creds: AzureCredentials) =
    let certificate = new X509Certificate2(creds.CertificateFilePath, creds.CertificatePassword)
    let creds = new Microsoft.Azure.CertificateCloudCredentials(creds.SubscriptionId, certificate) :> SubscriptionCloudCredentials
    creds

let CreateComputeManagementClient creds = 
    let certificate = Authenticate creds
    new ComputeManagementClient(certificate)

let SplitStoragePath (path: string) =
    let parts = path.Split('/')
    (parts.[0], String.Join("/", parts.[1..parts.Length - 1]))

let CreatePercentageProgress totalSizeInBytes =
    let threshold = 0.1;
    let mutable lastValue = 0.0
    let monitor = new Object()
    let onProgress (status: TransferStatus) =
        let percentage = ((float status.BytesTransferred) / (float totalSizeInBytes)) * 100.0
        lock monitor (fun _ ->
            if percentage - lastValue > threshold then
                tracefn "[STORAGE] Uploaded %5.1f%%." percentage
            lastValue <- percentage
        )
    new Progress<TransferStatus>(new Action<TransferStatus>(onProgress))

let UploadFile connectionString sourcePath destinationPath = 
    let (container, path) = SplitStoragePath destinationPath
    let account = CloudStorageAccount.Parse connectionString
    let client = account.CreateCloudBlobClient()
    let container = client.GetContainerReference(container)
    container.CreateIfNotExistsAsync().Result |> ignore
    let blob = container.GetBlockBlobReference(path)
    let absoluteSourcePath = Path.GetFullPath(sourcePath)
    let sourceInfo = new FileInfo(sourcePath)
    tracefn "[STORAGE] Started to upload file: %s -> %s." absoluteSourcePath destinationPath
    let transferCtx = new TransferContext()
    transferCtx.ProgressHandler <- CreatePercentageProgress sourceInfo.Length
    let uploadOptions = new UploadOptions()
    TransferManager.UploadAsync(absoluteSourcePath, blob, uploadOptions, transferCtx).Wait()
    tracefn "[STORAGE] Upload completed:  %s -> %s." absoluteSourcePath destinationPath
    blob.Uri

let GetDeploymentOrNull (creds: AzureCredentials) (svc: Deployment) = 
    let client = CreateComputeManagementClient creds
    let slot = ParseSlot svc.Slot
    let deployment = 
        try 
            client.Deployments.GetBySlot(svc.ServiceName, slot)
        with 
            | :? CloudException as ex when ex.Error.Code = "ResourceNotFound" -> null
            | ex -> raise ex
    deployment

let GetPackageCloudPath name label = sprintf "artifacts/%s/%s.cspkg" name label

let HandleOperationResponse prefix name (response: OperationStatusResponse) =
    match response.Status with
        | OperationStatus.Failed -> 
            sprintf "%s (%s)." response.Error.Message response.Error.Code  |> traceError
            sprintf "%s %s operation failed." prefix name |> failwith
        | OperationStatus.InProgress ->
            sprintf "%s %s operation is in progress." prefix name |> trace
        | OperationStatus.Succeeded ->
            sprintf "%s %s operation completed." prefix name |> trace
        | x -> ignore x

let Deploy (options: DeploymentOptions) =
    let client = CreateComputeManagementClient options.Credentials
    let slot = ParseSlot options.Deployment.Slot
    let existingDeployment = GetDeploymentOrNull options.Credentials options.Deployment
    let destinationPath = GetPackageCloudPath options.Deployment.ServiceName options.Label
    let isUri = Uri.IsWellFormedUriString(options.PackagePath, UriKind.Absolute)
    let packageUri = 
        if isUri then new Uri(options.PackagePath, UriKind.Absolute)
        else UploadFile options.StorageConnectionString options.PackagePath destinationPath
    let logPrefix = sprintf "[DEPLOY %s (%O)]" options.Deployment.ServiceName slot
    let formatLog fmt = 
        let prepend msg = sprintf "%s %s" logPrefix msg
        Printf.ksprintf prepend fmt
    formatLog "Package uri is %A." packageUri |> trace
    let response = 
        match existingDeployment with
            | null -> 
                let createParams = new DeploymentCreateParameters()
                createParams.Configuration <- File.ReadAllText(options.ConfigurationFilePath)
                createParams.Label <- options.Label
                createParams.PackageUri <- packageUri
                createParams.Name <- options.Deployment.ServiceName
                createParams.TreatWarningsAsError <- Nullable(true)
                createParams.StartDeployment <- Nullable(true)
                client.Deployments.Create(options.Deployment.ServiceName, slot, createParams)
            | deployment ->
                let upgradeParams = new DeploymentUpgradeParameters()
                upgradeParams.Configuration <- File.ReadAllText(options.ConfigurationFilePath)
                upgradeParams.Label <- options.Label
                upgradeParams.PackageUri <- packageUri
                formatLog "Started deployment upgrade." |> trace
                client.Deployments.UpgradeBySlot(options.Deployment.ServiceName, slot, upgradeParams)
    HandleOperationResponse logPrefix "Deployment" response

let DeleteCloudService (options: StopOptions) =
    let client = CreateComputeManagementClient options.Credentials
    let slot = ParseSlot options.Deployment.Slot
    let existingDeployment = GetDeploymentOrNull options.Credentials options.Deployment
    let logPrefix = sprintf "[AZURE %s (%O)]" options.Deployment.ServiceName slot
    let formatLog fmt = 
        let prepend msg = String.Join(" ", [logPrefix, msg])
        Printf.ksprintf prepend fmt
    if existingDeployment <> null then
        let response = client.Deployments.DeleteBySlot(options.Deployment.ServiceName, slot)
        HandleOperationResponse logPrefix "Delete" response
