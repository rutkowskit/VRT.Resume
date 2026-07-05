# Publishes the PWA and serves it locally for manual testing (OPFS / offline).
#
# Usage:
#   pwsh ./VRT.Resume.Pwa/publish-serve.ps1
#   pwsh ./VRT.Resume.Pwa/publish-serve.ps1 -SkipPublish -Port 8080

param(
    [int]$Port = 8080,
    [switch]$SkipPublish,
    [string]$PublishOutput = (Join-Path $PSScriptRoot '..' 'deploy' 'pwa')
)

$ErrorActionPreference = 'Stop'
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$PublishOutput = [System.IO.Path]::GetFullPath($PublishOutput)
$csproj = Join-Path $repoRoot 'VRT.Resume.Pwa\VRT.Resume.Pwa.csproj'

if (-not $SkipPublish) {
    Write-Host "Publishing PWA to $PublishOutput ..."
    dotnet publish $csproj -c Release -o $PublishOutput
}

& (Join-Path $PSScriptRoot 'serve-published.ps1') -Port $Port