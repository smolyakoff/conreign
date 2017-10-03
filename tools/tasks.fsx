#r "System.Net.Http"
#r "./../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "./../packages/build/FAKE/tools/FakeLib.dll"
#r "./../packages/build/Hyak.Common/lib/net45/Hyak.Common.dll"
#r "./../packages/build/Microsoft.Azure.Common/lib/net45/Microsoft.Azure.Common.dll"
#r "./../packages/build/Microsoft.Azure.Common/lib/net45/Microsoft.Azure.Common.NetFramework.dll"
#r "./../packages/build/Microsoft.WindowsAzure.Management.Compute/lib/portable-net45+wp8+wpa81+win/Microsoft.WindowsAzure.Management.Compute.dll"
#r "./../packages/build/WindowsAzure.Storage/lib/net45/Microsoft.WindowsAzure.Storage.dll"
#r "./../packages/build/Microsoft.Azure.Storage.DataMovement/lib/net45/Microsoft.WindowsAzure.Storage.DataMovement.dll"
#r "./../packages/Etg.Yams/lib/net451/Etg.Yams.Core.dll"
#r "./../packages/Etg.Yams/lib/net451/Etg.Yams.AzureBlobStorageDeploymentRepository.dll"
#r "./../packages/semver/lib/net452/Semver.dll"


open Fake
open Fake.TraceHelper
open Fake.NpmHelper
open System
open System.IO
open System.Text
open System.Threading.Tasks
open System.Security.Cryptography.X509Certificates
open Newtonsoft.Json
open Hyak.Common
open Microsoft.Azure
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Blob
open Microsoft.WindowsAzure.Storage.DataMovement
open Microsoft.WindowsAzure.Management.Compute
open Microsoft.WindowsAzure.Management.Compute.Models
open Etg.Yams.Azure.Storage
open Etg.Yams.Application

System.Net.ServicePointManager.DefaultConnectionLimit <- Environment.ProcessorCount * 8;
System.Net.ServicePointManager.Expect100Continue <- false;

type PackageMetadata =
    {
        Name: string;
        Main: string;
        Version: string;
    }

type AzureCredentials = 
    {
        SubscriptionId: string;
        CertificateFilePath: string; 
        CertificatePassword: string;
    }

let private ToFormattedJson o =
    JsonConvert.SerializeObject(o, Formatting.Indented)

let private Dump o =
    ToFormattedJson o |> log  

let private ParseSlot slot = 
    Enum.Parse(typedefof<DeploymentSlot>, slot, true) :?> DeploymentSlot

let private Authenticate (creds: AzureCredentials) =
     let certificate = new X509Certificate2(creds.CertificateFilePath, creds.CertificatePassword)
     let creds = Microsoft.Azure.CertificateCloudCredentials(creds.SubscriptionId, certificate) :> SubscriptionCloudCredentials
     creds

let private CreateComputeManagementClient creds = 
    let certificate = Authenticate creds
    new ComputeManagementClient(certificate)

let private SplitStoragePath (path: string) =
    let parts = path.Split('/')
    (parts.[0], String.Join("/", parts.[1..parts.Length - 1]))

let private CreatePercentageProgress totalSizeInBytes =
    let period = TimeSpan.FromSeconds(5.0)
    let mutable lastPercentage = 0.0
    let mutable lastTime = new DateTime()
    let monitor = new Object()
    let onProgress (status: TransferStatus) =
        let percentage = ((float status.BytesTransferred) / (float totalSizeInBytes)) * 100.0
        lock monitor (fun _ ->
            let time = DateTime.Now
            if time - lastTime > period && lastPercentage <> percentage then
                tracefn "Uploaded %5.1f%%." percentage
                lastPercentage <- percentage
                lastTime <- time
        )
    new Progress<TransferStatus>(new Action<TransferStatus>(onProgress))

type AzureCloudServiceDeployment = 
    {
        ServiceName: string;
        Slot: string;
    }

type AzureCloudServiceDeploymentOptions = 
    { 
        Credentials: AzureCredentials;
        StorageConnectionString: string;
        Label: string;
        Deployment: AzureCloudServiceDeployment;
        ConfigurationFilePath: string;
        PackagePath: string;
    }

let private GetDeploymentOrNull (creds: AzureCredentials) (svc: AzureCloudServiceDeployment) = 
    let client = CreateComputeManagementClient creds
    let slot = ParseSlot svc.Slot
    let deployment = 
        try 
            client.Deployments.GetBySlot(svc.ServiceName, slot)
        with 
            | :? CloudException as ex when ex.Error.Code = "ResourceNotFound" -> null
            | ex -> raise ex
    deployment

let private GetPackageCloudPath name label = sprintf "artifacts/%s/%s.cspkg" name label

let private HandleOperation prefix name (responseTask: Task<OperationStatusResponse>) =
    sprintf "%s %s operation is started." prefix name |> trace
    let response = responseTask.Result
    match response.Status with
        | OperationStatus.Failed -> 
            sprintf "%s (%s)." response.Error.Message response.Error.Code  |> traceError
            sprintf "%s %s operation failed." prefix name |> failwith
        | OperationStatus.InProgress ->
            sprintf "%s %s operation is in progress." prefix name |> trace
        | OperationStatus.Succeeded ->
            sprintf "%s %s operation completed." prefix name |> trace
        | x -> ignore x

let private CreateCloudBlobClient connectionString =
    let account = CloudStorageAccount.Parse connectionString
    account.CreateCloudBlobClient()

let private GetContainerAndBlob (client: CloudBlobClient) path =
    let (container, path) = SplitStoragePath path
    let container = client.GetContainerReference(container)
    container.CreateIfNotExistsAsync().Result |> ignore
    let blob = container.GetBlockBlobReference(path)
    (container, blob)

let private GetContainerAndDirectory (client: CloudBlobClient) path = 
    let (container, path) = SplitStoragePath path
    let container = client.GetContainerReference(container)
    container.CreateIfNotExistsAsync().Result |> ignore
    let dir = container.GetDirectoryReference(path)
    (container, dir)

let UploadFile connectionString sourcePath destinationPath = 
    let client = CreateCloudBlobClient connectionString
    let (_, blob) = GetContainerAndBlob client destinationPath
    let absoluteSourcePath = Path.GetFullPath(sourcePath)
    let sourceInfo = new FileInfo(sourcePath)
    tracefn "Starting to upload file: %s -> %s." absoluteSourcePath destinationPath
    let transferCtx = new SingleTransferContext()
    transferCtx.ProgressHandler <- CreatePercentageProgress sourceInfo.Length
    let uploadOptions = new UploadOptions()
    TransferManager.UploadAsync(absoluteSourcePath, blob, uploadOptions, transferCtx).Wait()
    tracefn "Upload completed:  %s -> %s." absoluteSourcePath destinationPath
    blob.Uri

let DownloadFile connectionString sourcePath destinationPath =
    let client = CreateCloudBlobClient connectionString
    let (_, blob) = GetContainerAndBlob client destinationPath
    if not (blob.Exists()) then failwithf "Blob %s doesn't exist." sourcePath
    let absoluteDestinationPath = Path.GetFullPath(destinationPath)
    blob.FetchAttributes()
    let transferCtx = new SingleTransferContext()
    transferCtx.ProgressHandler <- CreatePercentageProgress blob.Properties.Length
    tracefn "Starting to download file: %s -> %s." sourcePath absoluteDestinationPath
    TransferManager.DownloadAsync(blob, destinationPath).Wait()
    destinationPath

let DownloadFileToDirectory connectionString sourcePath destinationDirectory =
    let uri = new Uri(sourcePath, UriKind.RelativeOrAbsolute)
    let fileName = Path.GetFileName(uri.LocalPath)
    let destinationPath = destinationDirectory @@ fileName
    DownloadFile connectionString sourcePath destinationPath

let CalculateDirectorySize path =
    let rec Calculate (dir: DirectoryInfo) = 
        let files = dir.GetFiles()
        let dirs = dir.GetDirectories()
        let dirsSize = 
            dirs 
            |> Seq.map Calculate
            |> Seq.sum
        let filesSize = 
            files 
            |> Seq.map (fun f -> f.Length)
            |> Seq.sum
        filesSize + dirsSize
    let dir = new DirectoryInfo(path)
    if dir.Exists then Calculate dir else 0L

let UploadDirectory (parallelConnections: option<int>) connectionString sourcePath destinationPath =
    let client = CreateCloudBlobClient connectionString
    let (_, dir) = GetContainerAndDirectory client destinationPath
    let sourceSize = CalculateDirectorySize sourcePath
    let upload (ctx: DirectoryTransferContext) =
        let getRelativeSourcePath (event: TransferEventArgs) = 
            let fullPath = event.Source.ToString()
            (ProduceRelativePath sourcePath fullPath).TrimStart([|'.'; Path.DirectorySeparatorChar|])
        let onFileFailed (event: TransferEventArgs) = 
            sprintf  
                "File transfer failed: %A. The error message was: %s" 
                (getRelativeSourcePath event)
                event.Exception.Message
            |> traceImportant
        let onFileCompleted (event: TransferEventArgs) = 
            sprintf "File transfer completed: %A." (getRelativeSourcePath event) |> log
        let onFileSkipped (event: TransferEventArgs) = 
            sprintf "File transfer skipped: %A." event.Source |> log
        ctx.FileFailed.Add(onFileFailed)
        ctx.FileTransferred.Add(onFileCompleted)
        ctx.FileSkipped.Add(onFileSkipped)
        ctx.ProgressHandler <- CreatePercentageProgress sourceSize
        let opts = new UploadDirectoryOptions()
        opts.Recursive <- true
        if parallelConnections.IsSome then
            TransferManager.Configurations.ParallelOperations <- parallelConnections.Value
        TransferManager.UploadDirectoryAsync(sourcePath, dir, opts, ctx).Result
    let attempts = 5
    let rec tryUpload (previousResult: TransferStatus) (checkpoint: TransferCheckpoint) attempt =
        match previousResult, attempt with
            | result, _ when result <> null && result.NumberOfFilesFailed = 0L -> result
            | result, attempt when (result = null || result.NumberOfFilesFailed > 0L) && attempt < attempts ->
                tracefn "Starting upload from %s to %s (attempt #%d)..." sourcePath destinationPath (attempt + 1)
                let ctx = 
                    if checkpoint = null then new DirectoryTransferContext()
                    else new DirectoryTransferContext(checkpoint)
                let result = upload ctx
                tracefn 
                    "Upload from %s to %s (attempt #%d) completed. %d files uploaded, %d skipped, %d failed. %d bytes transfered."
                    sourcePath
                    destinationPath
                    (attempt + 1)
                    result.NumberOfFilesTransferred
                    result.NumberOfFilesSkipped
                    result.NumberOfFilesFailed
                    result.BytesTransferred
                tryUpload result ctx.LastCheckpoint (attempt + 1)
            | _ ->
                failwithf "Upload failed: %d files were not uploaded." previousResult.NumberOfFilesFailed
    tryUpload null null 0 |> ignore
    destinationPath

let PackageCloudService projectPath buildOutputPath packageOutputPath label version =
    let buildProps = 
        [
            ("Configuration", "Release");
            ("TargetProfile", "Cloud");
            ("OutputPath", buildOutputPath)
            ("PublishDir", packageOutputPath)
        ]
    let buildParams (defaults: MSBuildParams) =
        {
            defaults with
                Verbosity = Some(MSBuildVerbosity.Minimal)
                Targets = ["Clean"; "Publish"]
                Properties = buildProps
        }
    build buildParams projectPath
    let path = !! (packageOutputPath @@ "*.cspkg") |> Seq.head
    let info = new FileInfo(path)
    let newPath = info.DirectoryName @@ (sprintf "%s__%s.cspkg" label version)
    Fake.FileHelper.Rename newPath path
    newPath

let private ListArtifacts storageConnectionString name label = 
    let account = CloudStorageAccount.Parse(storageConnectionString)
    let client = account.CreateCloudBlobClient()
    let container = client.GetContainerReference("artifacts")
    if container.Exists() 
        then
            let artifactsPath = sprintf "%s/%s" name label
            container.ListBlobs(artifactsPath) 
                |> Seq.filter (fun i -> i :? CloudBlockBlob)
                |> Seq.cast<CloudBlockBlob>
                |> Seq.toList 
        else []

let private IsUri path =
    Uri.IsWellFormedUriString(path, UriKind.Absolute)

let private ExtractVersionFromCloudServicePackage packagePath =
    let name = Path.GetFileNameWithoutExtension(packagePath)
    let parts = List.ofArray (name.Split([|"__"|], StringSplitOptions.RemoveEmptyEntries))
    match parts with 
      | head :: version -> String.Join("__", version)
      | _ -> sprintf "Cannot extract version from %s" packagePath |> failwith

let private DeployCloudServiceInternal (options: AzureCloudServiceDeploymentOptions) =
    let client = CreateComputeManagementClient options.Credentials
    let slot = ParseSlot options.Deployment.Slot
    let existingDeployment = GetDeploymentOrNull options.Credentials options.Deployment
    let destinationPath = GetPackageCloudPath options.Deployment.ServiceName options.Label
    let isUri = IsUri options.PackagePath
    let packageUri = 
        if isUri then new Uri(options.PackagePath, UriKind.Absolute)
        else UploadFile options.StorageConnectionString options.PackagePath destinationPath
    let logPrefix = sprintf "[DEPLOY %s (%O)]" options.Deployment.ServiceName slot
    let formatLog fmt = 
        let prepend msg = sprintf "%s %s" logPrefix msg
        Printf.ksprintf prepend fmt
    formatLog "Package uri is %A." packageUri |> trace
    let task = 
        match existingDeployment with
            | null -> 
                let createParams = new DeploymentCreateParameters()
                createParams.Configuration <- File.ReadAllText(options.ConfigurationFilePath)
                createParams.Label <- options.Label
                createParams.PackageUri <- packageUri
                createParams.Name <- options.Deployment.ServiceName
                createParams.TreatWarningsAsError <- Nullable(true)
                createParams.StartDeployment <- Nullable(true)
                client.Deployments.CreateAsync(options.Deployment.ServiceName, slot, createParams)
            | deployment ->
                let upgradeParams = new DeploymentUpgradeParameters()
                upgradeParams.Configuration <- File.ReadAllText(options.ConfigurationFilePath)
                upgradeParams.Label <- options.Label
                upgradeParams.PackageUri <- packageUri
                formatLog "Started deployment upgrade." |> trace
                client.Deployments.UpgradeBySlotAsync(options.Deployment.ServiceName, slot, upgradeParams)
    HandleOperation logPrefix "Deployment" task

let DeployCloudService (options: AzureCloudServiceDeploymentOptions) = 
    let packagePathOption = 
        if options.PackagePath = null then 
            ListArtifacts options.StorageConnectionString options.Deployment.ServiceName options.Label
                |> List.filter (fun i -> i.Properties.LastModified.HasValue)
                |> List.sortByDescending (fun i -> i.Properties.LastModified.Value)
                |> List.map (fun i -> i.StorageUri.PrimaryUri.AbsoluteUri)
                |> List.tryHead
        else Some(options.PackagePath)
    match packagePathOption with 
        | None -> 
            traceError "Existing Azure packages were not found."
            failwith "Failed to deploy to Azure Cloud Service."
        | Some(packagePath) -> 
            let version = ExtractVersionFromCloudServicePackage (packagePath)
            tracefn "[DEPLOY] Package version is %s." version
            // TODO: This looks weird, refactoring is needed
            let opts = 
                {
                    options with
                        PackagePath = packagePath;
                        Label = sprintf "%s_%s" options.Label version;
                }
            DeployCloudServiceInternal opts

type DeleteCloudServiceCommand = 
    {
        Credentials: AzureCredentials;
        Deployment: AzureCloudServiceDeployment;
    }

let DeleteCloudService (options: DeleteCloudServiceCommand) =
    let client = CreateComputeManagementClient options.Credentials
    let slot = ParseSlot options.Deployment.Slot
    let existingDeployment = GetDeploymentOrNull options.Credentials options.Deployment
    let logPrefix = sprintf "[AZURE %s (%O)]" options.Deployment.ServiceName slot
    let formatLog fmt = 
        let prepend msg = String.Join(" ", [logPrefix, msg])
        Printf.ksprintf prepend fmt
    if isNull existingDeployment then
        formatLog "Nothing to delete." |> trace
    else        
        let task = client.Deployments.DeleteBySlotAsync(options.Deployment.ServiceName, slot)
        HandleOperation logPrefix "Delete" task

type BuildOptions = 
    {
        Name: string;
        Main: option<string>;
        ProjectPath: string;
        OutputPath: string;
        Version: string;
        Clean: bool;
        Configuration: string;
        Verbosity: MSBuildVerbosity;
    }

let DefaultBuildOptions = 
    {
        Name = null;
        Main = None;
        ProjectPath = null;
        OutputPath = null;
        Version= null;
        Clean = true;
        Configuration = "Release";
        Verbosity = Minimal;
    }

let BuildProject (options: BuildOptions) =
    let props = 
        [
            ("Configuration", options.Configuration); 
            ("OutputPath", options.OutputPath)
        ]
    let targets = if options.Clean then ["Clean"; "Build"] else ["Build"]
    let setParams (defaults: MSBuildParams) = 
        {defaults with
            Targets = targets
            Properties = props
            Verbosity = Some(options.Verbosity)
        }
    build setParams options.ProjectPath
    let metadata = 
        {
            Name = options.Name;
            Main = 
                match options.Main with
                    | Some(main) -> main
                    | _ -> 
                        let projectName = Path.GetDirectoryName options.ProjectPath
                        Path.GetFileName(sprintf "%s.%s" projectName "exe");
            Version = options.Version
        }
    let metadataJson = JsonConvert.SerializeObject(metadata, Formatting.Indented);
    let packageJsonFilePath = options.OutputPath @@ "netpackage.json"
    WriteStringToFile false packageJsonFilePath metadataJson
    trace "Built .NET package:"
    trace metadataJson

let private ReadPackageMetadata packagePath = 
    let metadataPath = packagePath @@ "netpackage.json"
    let metadataJson = ReadFileAsString metadataPath
    let metadata =JsonConvert.DeserializeObject<PackageMetadata> metadataJson
    metadata

let PackageProject packagePath outputDirectory =
    let metadata = ReadPackageMetadata packagePath
    let zipName = sprintf "%s__%s.zip" metadata.Name metadata.Version
    let zipPath = outputDirectory @@ zipName
    let zipFiles = !! (packagePath @@ "**" @@ "*") |> Seq.toList
    Zip packagePath zipPath zipFiles

type YamsDeploymentOptions = {
    PackagePath: option<string>;
    AdditionalFiles: seq<FileIncludes>;
    TempDirectory: string;
    StorageConnectionString: string;
    ClusterId: string;
    Environment: string;
    ParallelConnections: option<int>
}

type YamsApplicationConfiguration = {
    ExeName: string;
    ExeArgs: string;
}

let private GenerateYamsAppConfig packagePath (metadata: PackageMetadata) (args: seq<string * string>) =
    let allArgs = 
        [
            ("ClusterId", "${ClusterId}");
            ("InstanceId", "${InstanceId}");
            ("Version", "${Version}");
        ] |> Seq.append args
    let stringArgs = allArgs |> Seq.map (fun (k, v) -> sprintf "%s=\"%s\"" k v)
    let argsLine = System.String.Join(" ", stringArgs)
    let config = 
        {
            ExeName = metadata.Main
            ExeArgs = argsLine
        }
    let configPath = packagePath @@ "AppConfig.json"
    let configJson = ToFormattedJson config
    WriteStringToFile false configPath configJson
    config

let private GetApplicationDeploymentPath (app: AppIdentity) =
    sprintf "applications/%s/%s" app.Id (app.Version.ToString())

let DeployPackageToYams (options: YamsDeploymentOptions) = 
    let repo = BlobStorageDeploymentRepository.Create(options.StorageConnectionString)
    let localPackagePath = 
        match options.PackagePath with
            | Some(packagePath) when IsUri packagePath ->
                DownloadFileToDirectory options.StorageConnectionString packagePath options.TempDirectory
            | Some(packagePath) -> packagePath
            | None -> failwith "Not implemented"
    let packageName = Path.GetFileNameWithoutExtension(localPackagePath)
    let extractedPackagePath = options.TempDirectory @@ packageName
    tracefn "Extracting package to: %s" extractedPackagePath
    Unzip extractedPackagePath localPackagePath 
    CopyWithSubfoldersTo extractedPackagePath options.AdditionalFiles
    trace "Copied additional files."
    let metadata = ReadPackageMetadata extractedPackagePath
    let app = new AppIdentity(metadata.Name, metadata.Version)
    let exists = repo.HasApplicationBinaries(app).Result
    if not exists then
        let props = [("Environment", options.Environment)]
        let appConfig = GenerateYamsAppConfig extractedPackagePath metadata props
        trace "Generated application config:"
        Dump appConfig
        trace "Ready to upload application package."
        let destinationPath = GetApplicationDeploymentPath app
        UploadDirectory 
            options.ParallelConnections
            options.StorageConnectionString 
            extractedPackagePath 
            destinationPath 
        |> ignore
        trace "Application package uploaded."
    else
        tracefn "Application \"%s\" v%s have been already uploaded." metadata.Name metadata.Version
    let previousConfig = repo.FetchDeploymentConfig().Result
    trace "Previous deployment configuration:"
    Dump previousConfig
    let existingVersions = previousConfig.ListVersions(metadata.Name, options.ClusterId) |> Seq.toList
    let currentConfig = 
        match existingVersions with
            | [appVersion] -> 
                tracefn "Previous application version: %s." appVersion
                previousConfig
                    .RemoveApplication(new AppIdentity(metadata.Name, appVersion), options.ClusterId)
                    .AddApplication(app, options.ClusterId)
            | _ -> 
                previousConfig.AddApplication(app, options.ClusterId)
    trace "Deployment configuration to publish:"
    Dump currentConfig
    repo.PublishDeploymentConfig(currentConfig).Wait()
    trace "Deployment configuration published."
    metadata

type JsProjectBuildOptions = {
    ProjectDirectory: string;
    ProjectBuildDirectory: string;
    OutputDirectory: string;
    BuildCommand: string;
}

let BuildJsProject (options: JsProjectBuildOptions) =
    tracefn "Running \"npm install\" in \"%s\"..." options.ProjectDirectory
    Npm 
        (fun p ->
            { p with
                Command = Install(Standard)
                WorkingDirectory = options.ProjectDirectory
            }
        )
    trace "Restored npm packages."
    trace "Building js project..."
    Npm 
        (fun p ->
            { p with
                Command = Run(options.BuildCommand)
                WorkingDirectory = options.ProjectDirectory
            }
        )
    tracefn "Copying build artifacts to \"%s\"..." options.OutputDirectory
    CopyDir options.OutputDirectory options.ProjectBuildDirectory (fun _ -> true)

let private MsDeployExePath = "C:\\Program Files\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe"

type MsDeployProviderOptions = {
    Name: string;
    Path: option<string>;
    Settings: list<string * option<string>>;
}

let ExecuteMsDeploy 
    verb 
    (source: MsDeployProviderOptions)
    (destination: MsDeployProviderOptions)
    (properties: list<string * option<string>>) =
    let escape value = sprintf "\"%s\"" value
    let formatPair (k, v) = 
        match v with 
            | Some(x) -> String.Join("=", [k; escape x])
            | _ -> k
    let formatProvider (provider: MsDeployProviderOptions) =
        let builder = new StringBuilder((provider.Name, provider.Path) |> formatPair);
        if not provider.Settings.IsEmpty then builder.Append(",") |> ignore
        let settings = 
            provider.Settings
            |> Seq.map formatPair
            |> String.concat ","
        builder.Append(settings).ToString()
    let args = 
        [
            (sprintf "verb:%s" verb, None);
            (sprintf "source:%s" (formatProvider source), None);
            (sprintf "dest:%s" (formatProvider destination), None);
        ] @ properties
    let argsLine = 
        args 
        |> Seq.map formatPair
        |> Seq.map (fun p -> "-" + p)
        |> String.concat " "
    let code = 
        ExecProcess
            (fun p ->
                p.FileName <- MsDeployExePath
                p.Arguments <- argsLine
            )
            (TimeSpan.FromMinutes(5.0))
    if code <> 0 then
        failwithf "MsDeploy exited with code %d." code

type StaticWebsiteDeploymentOptions = {
    SourcePath: string;
    WebsiteName: string;
    Host: string;
    Username: string;
    Password: string;
}

let DeployStaticWebsite (options: StaticWebsiteDeploymentOptions) =
    let source = 
        {
            Name = "contentPath";
            Path = Some(options.SourcePath);
            Settings = List.empty;
        }
    let computerName = sprintf "https://%s:8172/msdeploy.axd?site=%s" (options.Host.TrimEnd('/')) options.WebsiteName
    let dest = 
        {
            Name = "contentPath";
            Path = Some(options.WebsiteName);
            Settings = 
            [
                ("ComputerName", Some(computerName));
                ("Username", Some(options.Username));
                ("Password", Some(options.Password));
                ("AuthType", Some("Basic"));
            ]
        }
    let props = [("allowUntrusted", None)]
    ExecuteMsDeploy "sync" source dest props

