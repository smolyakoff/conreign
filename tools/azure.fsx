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

type DeploymentOptions = 
    { 
        Credentials: AzureCredentials;
        StorageConnectionString: string;
        Label: string;
        ServiceName: string; 
        Slot: string;
        ConfigurationFilePath: string;
        PackagePath: string;
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
    let onProgress (status: TransferStatus) =
        let percentage = ((float status.BytesTransferred) / (float sourceInfo.Length)) * 100.0
        tracefn "[STORAGE] Uploadeded %5.1f..." percentage
    let transferCtx = new TransferContext()
    transferCtx.ProgressHandler <- new Progress<TransferStatus>(new Action<TransferStatus>(onProgress))
    let uploadOptions = new UploadOptions()
    TransferManager.UploadAsync(absoluteSourcePath, blob, uploadOptions, transferCtx).Wait()
    tracefn "[STORAGE] Upload completed:  %s -> %s." absoluteSourcePath destinationPath
    blob.Uri

let GetDeploymentOrNull (options: DeploymentOptions) = 
    let client = CreateComputeManagementClient options.Credentials
    let slot = ParseSlot options.Slot
    let deployment = 
        try 
            client.Deployments.GetBySlot(options.ServiceName, slot)
        with 
            | :? CloudException as ex when ex.Error.Code = "ResourceNotFound" -> null
            | ex -> raise ex
    deployment

let GetPackageCloudPath name label = sprintf "artifacts/%s/%s.cspkg" name label

let Deploy (options: DeploymentOptions) =
    let client = CreateComputeManagementClient options.Credentials
    let slot = ParseSlot options.Slot
    let existingDeployment = GetDeploymentOrNull options
    let destinationPath = GetPackageCloudPath options.ServiceName options.Label
    let packageUri = UploadFile options.StorageConnectionString options.PackagePath destinationPath
    let formatLog fmt = 
        let prepend msg = sprintf "[DEPLOY %s(%O)-%s] %s" options.ServiceName slot options.Label msg
        Printf.ksprintf prepend fmt
    match existingDeployment with
        | null -> 
            raise (new NotImplementedException("Service provisioning is not implemented"))
        | deployment ->
            let upgradeParams = new DeploymentUpgradeParameters()
            upgradeParams.Configuration <- File.ReadAllText(options.ConfigurationFilePath)
            upgradeParams.Label <- options.Label
            upgradeParams.PackageUri <- packageUri
            formatLog "Started deployment upgrade." |> trace
            let response = client.Deployments.UpgradeBySlot(options.ServiceName, slot, upgradeParams)
            match response.Status with
                | OperationStatus.Failed -> 
                    formatLog "%s (%s)." response.Error.Message response.Error.Code |> traceError
                    failwith "Deployment failed."
                | OperationStatus.InProgress ->
                    formatLog "Deployment upgrade is in progress." |> trace
                | OperationStatus.Succeeded ->
                    formatLog "Deployment upgrade completed." |> trace
                | x -> ignore x
