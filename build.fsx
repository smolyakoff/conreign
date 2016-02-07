#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/FAKE.SQL.X64/tools/FAKE.SQL.dll"
#r "packages/FAKE.SQL.X64/tools/Microsoft.SqlServer.Smo.dll"
#r "packages/FAKE.SQL.X64/tools/Microsoft.SqlServer.Management.Sdk.Sfc.dll"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open Fake
open Fake.SQL.SqlServer
open FSharp.Data

type BuildConfig = JsonProvider<"config.json">

let SolutionDir = __SOURCE_DIRECTORY__ 
let ConfigPath = SolutionDir @@ "config.json"
let OrleansDatabaseSchemaPath = SolutionDir @@ "packages/Microsoft.Orleans.OrleansSqlUtils/lib/net451/SQLServer/CreateOrleansTables_SqlServer.sql"

let config = BuildConfig.Load(ConfigPath)

Target "ProvisionOrleansDb" (fun _ ->
    let serverInfo = getServerInfo config.Db
    let exists = intitialCatalogExistsOnServer serverInfo
    if not exists then
        CreateDb serverInfo |> ignore
        runScript serverInfo OrleansDatabaseSchemaPath
    else
        let db = getDatabase serverInfo
        use results = db.ExecuteWithResults("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'OrleansMembershipTable'")
        let initialized = results.Tables.["TABLES"].Rows.Count > 0
        if not initialized then runScript serverInfo OrleansDatabaseSchemaPath
)

Target "Default" (fun _ ->
    PrintTargets()
)

RunTargetOrDefault "Default"