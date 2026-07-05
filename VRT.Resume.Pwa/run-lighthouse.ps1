# PWA Lighthouse helper.
#
# Headless Lighthouse on Blazor WASM often hangs or crashes on Windows (Chrome temp EPERM).
# Default mode opens the app in your browser and prints DevTools steps — fast and reliable.
# Use -Automated only if you want to try headless CLI (hard timeout, may still fail).
#
# Usage:
#   pwsh ./VRT.Resume.Pwa/run-lighthouse.ps1              # production URL, manual DevTools
#   pwsh ./VRT.Resume.Pwa/run-lighthouse.ps1 -Local         # publish + serve + manual DevTools
#   pwsh ./VRT.Resume.Pwa/run-lighthouse.ps1 -Automated     # try headless CLI (90s cap)

param(
    [string]$Url = 'https://cv.rutus.work/',
    [switch]$Local,
    [switch]$Automated,
    [int]$Port = 8080,
    [int]$TimeoutSeconds = 90,
    [int]$MaxWaitForLoadMs = 30000,
    [switch]$SkipPublish,
    [string]$OutputPath = (Join-Path $PSScriptRoot '..' 'deploy' 'pwa' 'lighthouse-pwa.json')
)

$ErrorActionPreference = 'Stop'
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$publishRoot = Join-Path $repoRoot 'deploy' 'pwa'
$webRoot = Join-Path $publishRoot 'wwwroot'
$serveScript = Join-Path $PSScriptRoot 'serve-published.ps1'
$publishServeScript = Join-Path $PSScriptRoot 'publish-serve.ps1'

if ($Local) {
    $Url = "http://127.0.0.1:$Port/"
}

function Wait-ForServer([string]$targetUrl, [int]$timeoutSeconds = 15) {
    $deadline = [datetime]::UtcNow.AddSeconds($timeoutSeconds)
    while ([datetime]::UtcNow -lt $deadline) {
        try {
            $response = Invoke-WebRequest -Uri $targetUrl -UseBasicParsing -TimeoutSec 3
            if ($response.StatusCode -eq 200) {
                return
            }
        }
        catch {
            Start-Sleep -Milliseconds 400
        }
    }

    throw "Static server did not respond at $targetUrl within ${timeoutSeconds}s."
}

function Stop-ServerProcess([System.Diagnostics.Process]$process) {
    if (-not $process -or $process.HasExited) {
        return
    }

    try {
        Stop-Process -Id $process.Id -Force -ErrorAction Stop
    }
    catch {
        Write-Warning "Could not stop static server (PID $($process.Id))."
    }
}

function Show-ManualSteps([string]$targetUrl) {
    Write-Host ''
    Write-Host 'Manual Lighthouse (recommended for Blazor WASM):'
    Write-Host "  1. Open $targetUrl in Chrome or Edge"
    Write-Host '  2. Wait until the app fully loads (first visit caches the WASM shell)'
    Write-Host '  3. DevTools (F12) → Lighthouse → Categories: Progressive Web App → Analyze'
    Write-Host '  4. Target score: PWA >= 90'
    Write-Host ''
    Write-Host 'Local published build:'
    Write-Host '  pwsh ./VRT.Resume.Pwa/publish-serve.ps1'
    Write-Host '  then open http://127.0.0.1:8080'
    Write-Host ''
}

function Invoke-AutomatedLighthouse(
    [string]$targetUrl,
    [string]$reportPath,
    [int]$timeoutSeconds,
    [int]$maxWaitForLoadMs
) {
    $browser = @(
        ${env:CHROME_PATH},
        (Join-Path ${env:ProgramFiles(x86)} 'Google\Chrome\Application\chrome.exe'),
        (Join-Path ${env:ProgramFiles} 'Google\Chrome\Application\chrome.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Microsoft\Edge\Application\msedge.exe')
    ) | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1

    if (-not $browser) {
        Write-Error 'Chrome/Edge not found for automated Lighthouse.'
    }

    $tempDir = Join-Path (Split-Path $reportPath -Parent) '.lighthouse-tmp'
    New-Item -ItemType Directory -Force -Path $tempDir | Out-Null
    $outputPathArg = [System.IO.Path]::GetFullPath($reportPath)
    if (Test-Path $outputPathArg) {
        Remove-Item $outputPathArg -Force
    }

    Write-Host "Automated Lighthouse (browser: $browser, timeout ${timeoutSeconds}s)..."

    $job = Start-Job -ScriptBlock {
        param($Url, $Out, $Browser, $TempDir, $MaxWait)
        $env:CHROME_PATH = $Browser
        $env:TEMP = $TempDir
        $env:TMP = $TempDir
        & npx --yes lighthouse $Url `
            --only-categories=pwa `
            --chrome-flags='--headless=new --no-sandbox --disable-gpu --disable-dev-shm-usage' `
            --max-wait-for-load=$MaxWait `
            --output=json `
            --output-path=$Out
        return (Test-Path $Out)
    } -ArgumentList $targetUrl, $outputPathArg, $browser, $tempDir, $maxWaitForLoadMs

    $started = [datetime]::UtcNow
    $nextProgress = $started.AddSeconds(10)
    $timedOut = $false

    while ((Get-Job -Id $job.Id).State -eq 'Running') {
        if ([datetime]::UtcNow -gt $started.AddSeconds($timeoutSeconds)) {
            $timedOut = $true
            Stop-Job $job -ErrorAction SilentlyContinue
            Get-Process -Name chrome, msedge -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
            break
        }

        if ([datetime]::UtcNow -ge $nextProgress) {
            $elapsed = [int]([datetime]::UtcNow - $started).TotalSeconds
            Write-Host "  ...still running (${elapsed}s / ${timeoutSeconds}s)"
            $nextProgress = $nextProgress.AddSeconds(10)
        }

        Start-Sleep -Milliseconds 500
    }

    $reportWritten = $false
    if (-not $timedOut -and (Get-Job -Id $job.Id).State -eq 'Completed') {
        $reportWritten = [bool](Receive-Job $job)
    }

    Remove-Job $job -Force -ErrorAction SilentlyContinue
    if (-not $reportWritten) {
        $reportWritten = Test-Path $outputPathArg
    }

    return $reportWritten
}

$serverProcess = $null

try {
    if ($Local) {
        if (-not $SkipPublish) {
            Write-Host 'Publishing PWA (Release)...'
            dotnet publish (Join-Path $repoRoot 'VRT.Resume.Pwa\VRT.Resume.Pwa.csproj') -c Release -o $publishRoot
        }

        if (-not (Test-Path $webRoot)) {
            Write-Error "Published web root not found: $webRoot. Run publish-serve.ps1 first."
        }

        Write-Host "Starting local static server on $Url"
        $serverProcess = Start-Process `
            -FilePath (Get-Command pwsh).Source `
            -ArgumentList @('-NoProfile', '-File', $serveScript, '-Port', $Port) `
            -PassThru `
            -WindowStyle Hidden

        Wait-ForServer $Url
    }

    if ($Automated) {
        if (Invoke-AutomatedLighthouse $Url $OutputPath $TimeoutSeconds $MaxWaitForLoadMs) {
            $report = Get-Content $OutputPath -Raw | ConvertFrom-Json
            $score = [math]::Round(100 * $report.categories.pwa.score)
            Write-Host "PWA score: $score / 100"
            Write-Host "Report: $OutputPath"
            if ($score -lt 90) { exit 1 }
            return
        }

        Write-Warning 'Automated Lighthouse failed or timed out. Use manual DevTools instead.'
    }

    Write-Host "Opening $Url in the default browser..."
    Start-Process $Url
    Show-ManualSteps $Url
}
finally {
    if ($Local -and -not $Automated -and $serverProcess -and -not $serverProcess.HasExited) {
        Write-Host "Local server left running at $Url (PID $($serverProcess.Id))."
        Write-Host "When finished: Stop-Process -Id $($serverProcess.Id) -Force"
        Write-Host 'Or use publish-serve.ps1 next time (single terminal, Ctrl+C stops server).'
    }
    elseif ($Automated -or -not $Local) {
        Stop-ServerProcess $serverProcess
    }
}