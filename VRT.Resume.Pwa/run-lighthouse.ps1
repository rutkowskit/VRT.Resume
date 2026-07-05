# Runs a Lighthouse PWA audit against the published static app.
# Requires Node.js + Google Chrome or Microsoft Edge.
#
# Usage:
#   pwsh ./VRT.Resume.Pwa/run-lighthouse.ps1
#   pwsh ./VRT.Resume.Pwa/run-lighthouse.ps1 -SkipPublish

param(
    [int]$Port = 8080,
    [switch]$SkipPublish,
    [string]$OutputPath = (Join-Path $PSScriptRoot '..' 'deploy' 'pwa' 'lighthouse-pwa.json')
)

$ErrorActionPreference = 'Stop'
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$publishRoot = Join-Path $repoRoot 'deploy' 'pwa'
$webRoot = Join-Path $publishRoot 'wwwroot'
$url = "http://127.0.0.1:$Port/"

function Resolve-BrowserPath {
    $candidates = @(
        ${env:CHROME_PATH},
        (Join-Path ${env:ProgramFiles} 'Google\Chrome\Application\chrome.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Google\Chrome\Application\chrome.exe'),
        (Join-Path ${env:ProgramFiles} 'Microsoft\Edge\Application\msedge.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Microsoft\Edge\Application\msedge.exe')
    ) | Where-Object { $_ -and (Test-Path $_) }

    return $candidates | Select-Object -First 1
}

if (-not $SkipPublish) {
    Write-Host 'Publishing PWA (Release)...'
    dotnet publish (Join-Path $repoRoot 'VRT.Resume.Pwa\VRT.Resume.Pwa.csproj') -c Release -o $publishRoot
}

if (-not (Test-Path $webRoot)) {
    Write-Error "Published web root not found: $webRoot"
}

$browser = Resolve-BrowserPath
if ($browser) {
    $env:CHROME_PATH = $browser
    Write-Host "Using browser: $browser"
}
else {
    Write-Warning 'Chrome/Edge not found. Set CHROME_PATH if Lighthouse cannot launch a browser.'
}

Write-Host "Starting static server on $url"
$serverJob = Start-Job -ScriptBlock {
    param($ScriptPath, $Port)
    & $ScriptPath -Port $Port
} -ArgumentList (Join-Path $PSScriptRoot 'serve-published.ps1'), $Port

Start-Sleep -Seconds 2

try {
    Write-Host 'Running Lighthouse (PWA category only)...'
    npx --yes lighthouse $url `
        --only-categories=pwa `
        --chrome-flags='--headless=new --no-sandbox' `
        --max-wait-for-load=180000 `
        --output=json `
        --output-path=$OutputPath

    $report = Get-Content $OutputPath -Raw | ConvertFrom-Json
    $score = [math]::Round(100 * $report.categories.pwa.score)
    Write-Host "PWA score: $score / 100"
    Write-Host "Report: $OutputPath"

    if ($score -lt 90) {
        Write-Warning 'Score is below 90. Open the URL in Chrome DevTools > Lighthouse for detailed audits.'
        exit 1
    }
}
finally {
    Stop-Job $serverJob -ErrorAction SilentlyContinue
    Remove-Job $serverJob -Force -ErrorAction SilentlyContinue
}