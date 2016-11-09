#r "./../packages/FAKE/tools/FakeLib.dll"
#r "./../packages/System.ComponentModel.TypeConverter/lib/net45/System.ComponentModel.TypeConverter.dll"
#r "./../packages/Microsoft.Extensions.Primitives/lib/netstandard1.0/Microsoft.Extensions.Primitives.dll"
#r "./../packages/Microsoft.Extensions.Configuration.Abstractions/lib/netstandard1.0/Microsoft.Extensions.Configuration.Abstractions.dll"
#r "./../packages/Microsoft.Extensions.FileProviders.Abstractions/lib/netstandard1.0/Microsoft.Extensions.FileProviders.Abstractions.dll"
#r "./../packages/Microsoft.Extensions.FileProviders.Physical/lib/net451/Microsoft.Extensions.FileProviders.Physical.dll"
#r "./../packages/Microsoft.Extensions.Configuration.Binder/lib/netstandard1.1/Microsoft.Extensions.Configuration.Binder.dll"
#r "./../packages/Microsoft.Extensions.Configuration/lib/netstandard1.1/Microsoft.Extensions.Configuration.dll"
#r "./../packages/Microsoft.Extensions.Configuration.FileExtensions/lib/net451/Microsoft.Extensions.Configuration.FileExtensions.dll"
#r "./../packages/Microsoft.Extensions.Configuration.Json/lib/net451/Microsoft.Extensions.Configuration.Json.dll"

open System.IO
open System.Collections.Generic
open Fake
open Microsoft.Extensions.Configuration

let DefaultConfigBasePath = Path.GetFullPath(__SOURCE_DIRECTORY__ @@ ".." @@ "config")

type public AzureConfiguration() =
    member val SubscriptionId: string = null with get, set
    member val CertificateFilePath: string = DefaultConfigBasePath @@ "management-certificate.secrets.pfx" with get, set
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

let DefaultConfig = LoadConfig DefaultConfigBasePath