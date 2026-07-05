# Serves published PWA static files for local offline/OPFS testing.
# SqliteWasmBlazor requires:
#   - Secure context: use http://127.0.0.1 (NOT LAN IP like http://192.168.x.x)
#   - COOP/COEP headers for SharedArrayBuffer in the SQLite worker
#   - Only one browser tab open (OPFS single-writer lock)
#
# Usage:
#   dotnet publish VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Release -o ./deploy/pwa
#   pwsh ./VRT.Resume.Pwa/serve-published.ps1
#   Open http://127.0.0.1:8080/

param(
    [int]$Port = 8080,
    [string]$WebRoot = (Join-Path $PSScriptRoot '..' 'deploy' 'pwa' 'wwwroot')
)

$ErrorActionPreference = 'Stop'
$WebRoot = [System.IO.Path]::GetFullPath($WebRoot)

if (-not (Test-Path $WebRoot)) {
    Write-Error "Web root not found: $WebRoot. Run 'dotnet publish VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Release -o ./deploy/pwa' first."
}

$mime = @{
    '.html' = 'text/html; charset=utf-8'
    '.js' = 'application/javascript; charset=utf-8'
    '.css' = 'text/css; charset=utf-8'
    '.json' = 'application/json; charset=utf-8'
    '.webmanifest' = 'application/manifest+json; charset=utf-8'
    '.wasm' = 'application/wasm'
    '.png' = 'image/png'
    '.ico' = 'image/png'
    '.map' = 'application/json; charset=utf-8'
    '.dat' = 'application/octet-stream'
    '.br' = 'application/octet-stream'
    '.gz' = 'application/octet-stream'
}

function Get-ContentType([string]$path) {
    $ext = [System.IO.Path]::GetExtension($path).ToLowerInvariant()
    if ($mime.ContainsKey($ext)) { return $mime[$ext] }
    return 'application/octet-stream'
}

function Send-Response([System.Net.HttpListenerResponse]$response, [int]$status, [string]$message) {
    $response.StatusCode = $status
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($message)
    $response.ContentType = 'text/plain; charset=utf-8'
    $response.ContentLength64 = $bytes.Length
    $response.OutputStream.Write($bytes, 0, $bytes.Length)
    $response.Close()
}

function Add-SecurityHeaders([System.Net.HttpListenerResponse]$response, [string]$relativePath) {
    $response.Headers['Cross-Origin-Opener-Policy'] = 'same-origin'
    $response.Headers['Cross-Origin-Embedder-Policy'] = 'require-corp'
    $response.Headers['Cross-Origin-Resource-Policy'] = 'same-origin'

    if ($relativePath -eq 'service-worker.js' -or $relativePath -eq 'service-worker-assets.js') {
        $response.Headers['Cache-Control'] = 'no-cache, no-store, must-revalidate'
    }
}

function Get-SpaFallbackPath([string]$requestPath) {
    $redirectsPath = Join-Path $WebRoot '_redirects'
    if (-not (Test-Path $redirectsPath)) {
        return $null
    }

    $normalized = '/' + $requestPath.Trim('/')
    foreach ($line in Get-Content $redirectsPath) {
        $trimmed = $line.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmed) -or $trimmed.StartsWith('#')) {
            continue
        }

        $parts = $trimmed -split '\s+', 3
        if ($parts.Count -lt 2) {
            continue
        }

        $from = $parts[0]
        if ($from.EndsWith('/*')) {
            $prefix = $from.Substring(0, $from.Length - 1)
            if ($normalized.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) {
                return Join-Path $WebRoot 'index.html'
            }
        }
        elseif ($normalized.Equals($from, [StringComparison]::OrdinalIgnoreCase)) {
            return Join-Path $WebRoot 'index.html'
        }
    }

    return $null
}

function Resolve-RequestPath([System.Net.HttpListenerRequest]$request) {
    $relativePath = [System.Uri]::UnescapeDataString($request.Url.AbsolutePath.TrimStart('/'))
    if ([string]::IsNullOrWhiteSpace($relativePath)) {
        $relativePath = 'index.html'
    }

    $fullPath = [System.IO.Path]::GetFullPath((Join-Path $WebRoot $relativePath))
    if (-not $fullPath.StartsWith($WebRoot, [StringComparison]::OrdinalIgnoreCase)) {
        return @{ Status = 403; Path = $null; RelativePath = $relativePath }
    }

    if (Test-Path $fullPath -PathType Leaf) {
        return @{ Status = 200; Path = $fullPath; RelativePath = $relativePath }
    }

    if ($request.HttpMethod -eq 'GET' -and -not $relativePath.Contains('.')) {
        $spaFallback = Get-SpaFallbackPath $relativePath
        if ($spaFallback -and (Test-Path $spaFallback -PathType Leaf)) {
            return @{ Status = 200; Path = $spaFallback; RelativePath = 'index.html' }
        }

        $indexPath = Join-Path $WebRoot 'index.html'
        if (Test-Path $indexPath -PathType Leaf) {
            return @{ Status = 200; Path = $indexPath; RelativePath = 'index.html' }
        }
    }

    return @{ Status = 404; Path = $null; RelativePath = $relativePath }
}

$listener = [System.Net.HttpListener]::new()
$listener.Prefixes.Add("http://127.0.0.1:$Port/")

try {
    $listener.Start()
}
catch [System.Net.HttpListenerException] {
    Write-Error "Port $Port is already in use. Stop the other server or pass -Port <number>."
}

Write-Host "Serving PWA at http://127.0.0.1:$Port/"
Write-Host "Root: $WebRoot"
Write-Host "Use 127.0.0.1 (not LAN IP). Close other tabs with this app open."
Write-Host "Press Ctrl+C to stop."

try {
    while ($listener.IsListening) {
        $context = $listener.GetContext()
        $request = $context.Request
        $response = $context.Response

        try {
            $resolved = Resolve-RequestPath $request
            if ($resolved.Status -ne 200) {
                Send-Response $response $resolved.Status $(if ($resolved.Status -eq 403) { 'Forbidden' } else { "Not found: $($resolved.RelativePath)" })
                continue
            }

            Add-SecurityHeaders $response $resolved.RelativePath
            $bytes = [System.IO.File]::ReadAllBytes($resolved.Path)
            $response.StatusCode = 200
            $response.ContentType = Get-ContentType $resolved.Path
            $response.ContentLength64 = $bytes.Length
            $response.OutputStream.Write($bytes, 0, $bytes.Length)
            $response.Close()
        }
        catch {
            try {
                Send-Response $response 500 "Internal server error: $($_.Exception.Message)"
            }
            catch {
                # Response may already be closed.
            }
        }
    }
}
finally {
    $listener.Stop()
    $listener.Close()
}