 param (
    [Parameter(Mandatory = $true)][String]$Name,
    [Int]$Instances = 1,
    [Int]$Rooms=8
 )

$appName = "Conreign.LoadTest.Supervisor"
$appPath = Join-Path $PSScriptRoot "../src/$appName/bin/Release/$appName.exe" -Resolve
$configPath = Join-Path $PSScriptRoot "../config/load-test-supervisor.secrets.json" -Resolve
$outputPath = Join-Path $PSScriptRoot "../research/logs" -Resolve
	
&$appPath --ConfigurationFileName=$configPath --OutputDirectory=$outputPath --InstanceOptions:ElasticSearchUri="" --InstanceOptions:BotOptions:RoomsCount=$Rooms --Name=$Name --Instances=$Instances 