param(
    [string]$AdminApiUrl = "http://127.0.0.1:5250",
    [string]$AdminWebUrl = "http://127.0.0.1:5260",
    [string]$LoginAccount = "admin",
    [string]$Password = "admin",
    [string]$TokenDetailPath = "/tokens/TKN000001"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$env:DOTNET_CLI_HOME = Join-Path $repoRoot ".dotnet"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

$adminApiProject = Join-Path $repoRoot "src\\Dimensions.Admin.Api"
$adminWebProject = Join-Path $repoRoot "src\\Dimensions.Admin.Web"

$apiJob = $null
$webJob = $null

try {
    $apiJob = Start-Job -ScriptBlock {
        param($projectPath, $url, $dotnetHome)
        Set-Location $projectPath
        $env:DOTNET_CLI_HOME = $dotnetHome
        $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet run --no-build --urls $url
    } -ArgumentList $adminApiProject, $AdminApiUrl, $env:DOTNET_CLI_HOME

    Start-Sleep -Seconds 8

    $webJob = Start-Job -ScriptBlock {
        param($projectPath, $url, $dotnetHome)
        Set-Location $projectPath
        $env:DOTNET_CLI_HOME = $dotnetHome
        $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet run --no-build --urls $url
    } -ArgumentList $adminWebProject, $AdminWebUrl, $env:DOTNET_CLI_HOME

    Start-Sleep -Seconds 8

    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $loginPage = Invoke-WebRequest -UseBasicParsing -Uri "$AdminWebUrl/Account/Login" -WebSession $session
    $antiForgeryToken = [regex]::Match($loginPage.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"').Groups[1].Value

    if ([string]::IsNullOrWhiteSpace($antiForgeryToken)) {
        throw "Missing anti-forgery token on login page."
    }

    $loginBody = @{
        LoginAccount = $LoginAccount
        Password = $Password
        __RequestVerificationToken = $antiForgeryToken
    }

    try {
        Invoke-WebRequest -UseBasicParsing -Uri "$AdminWebUrl/Account/Login" -Method Post -Body $loginBody -WebSession $session -MaximumRedirection 0 -ErrorAction Stop | Out-Null
    }
    catch {
        # MVC login success usually returns 302.
    }

    $dashboard = Invoke-WebRequest -UseBasicParsing -Uri "$AdminWebUrl/" -WebSession $session
    $tokenDetail = Invoke-WebRequest -UseBasicParsing -Uri "$AdminWebUrl$TokenDetailPath" -WebSession $session

    [pscustomobject]@{
        login_page_ok       = ($loginPage.StatusCode -eq 200)
        dashboard_ok        = ($dashboard.StatusCode -eq 200)
        token_detail_ok     = ($tokenDetail.StatusCode -eq 200)
        has_loading_overlay = ($dashboard.Content -like "*global-loading-overlay*")
        has_confirm_dialog  = ($tokenDetail.Content -like "*global-confirm-dialog*")
        has_dashboard       = ($dashboard.Content -like "*Admin.Web / Admin.Api*")
        has_token_usage     = ($tokenDetail.Content -like "*Token Usage*")
    } | ConvertTo-Json -Compress
}
finally {
    if ($webJob) {
        Stop-Job $webJob -ErrorAction SilentlyContinue | Out-Null
        Remove-Job $webJob -Force -ErrorAction SilentlyContinue | Out-Null
    }

    if ($apiJob) {
        Stop-Job $apiJob -ErrorAction SilentlyContinue | Out-Null
        Remove-Job $apiJob -Force -ErrorAction SilentlyContinue | Out-Null
    }
}
