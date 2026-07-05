# Publishes the PWA and deploys ./deploy/pwa/wwwroot to Cloudflare Pages (Direct Upload).
# Requires: .NET 10 SDK, Node.js, Wrangler CLI (npx wrangler or global install).
#
# One-time setup:
#   npx wrangler login
#   npx wrangler pages project create vrt-resume-pwa
#
# Usage (from repo root or this script's directory):
#   pwsh ./VRT.Resume.Pwa/deploy-pwa-cloudflare.ps1
#   pwsh ./VRT.Resume.Pwa/deploy-pwa-cloudflare.ps1 -Branch preview-test   # Preview only (--branch creates branch aliases)
#   pwsh ./VRT.Resume.Pwa/deploy-pwa-cloudflare.ps1 -ProjectName my-pwa -SkipPublish

param(
    [string]$ProjectName = 'vrt-resume-pwa',
    [string]$Branch = '',
    [string]$PublishOutput = (Join-Path $PSScriptRoot '..' 'deploy' 'pwa'),
    [switch]$SkipPublish
)

$ErrorActionPreference = 'Stop'
$RepoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$PublishOutput = [System.IO.Path]::GetFullPath($PublishOutput)
$WebRoot = Join-Path $PublishOutput 'wwwroot'
$Csproj = Join-Path $RepoRoot 'VRT.Resume.Pwa\VRT.Resume.Pwa.csproj'

if (-not $SkipPublish) {
    Write-Host "Publishing PWA to $PublishOutput ..."
    dotnet publish $Csproj -c Release -o $PublishOutput
}

if (-not (Test-Path (Join-Path $WebRoot 'index.html'))) {
    Write-Error "Web root not found or missing index.html: $WebRoot"
}

$wranglerArgs = @(
    'pages', 'deploy', $WebRoot,
    "--project-name=$ProjectName"
)
if (-not [string]::IsNullOrWhiteSpace($Branch)) {
    $wranglerArgs += "--branch=$Branch"
}

Write-Host "Deploying $WebRoot to Cloudflare Pages project '$ProjectName' ..."
if ($Branch) {
    Write-Host "Preview deployment (branch alias): $Branch"
} else {
    Write-Host 'Production deployment (no --branch). Ensure Pages production_branch is master (Direct Upload: set via API).'
}

Push-Location $RepoRoot
try {
    & npx --yes wrangler @wranglerArgs
    if ($LASTEXITCODE -ne 0) {
        throw "wrangler pages deploy failed with exit code $LASTEXITCODE"
    }
}
finally {
    Pop-Location
}