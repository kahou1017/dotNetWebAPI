$ErrorActionPreference = "Stop"

$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot "..\\.dotnet"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

$solutionPath = Join-Path $PSScriptRoot "..\\Dimensions.sln"

Write-Host "Building solution..."
dotnet build $solutionPath --no-restore

Write-Host "Running tests..."
dotnet test $solutionPath --no-build
