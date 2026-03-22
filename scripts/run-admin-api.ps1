$ErrorActionPreference = "Stop"

$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot "..\\.dotnet"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
$env:ASPNETCORE_ENVIRONMENT = "Development"

$projectPath = Join-Path $PSScriptRoot "..\\src\\Dimensions.Admin.Api\\Dimensions.Admin.Api.csproj"

Write-Host "Starting Dimensions.Admin.Api in Development..."
dotnet run --project $projectPath
