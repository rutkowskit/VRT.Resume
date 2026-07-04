# Serves published PWA static files for local offline/OPFS testing.
# SqliteWasmBlazor requires:
#   - Secure context: use http://127.0.0.1 (NOT LAN IP like http://192.168.x.x)
#   - COOP/COEP headers for SharedArrayBuffer in the SQLite worker
#   - Only one browser tab open (OPFS single-writer lock)
#
# Usage:
#   dotnet publish VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Release -o ./deploy/pwa
#   ./VRT.Resume.Pwa/serve-published.ps1
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

$listener = [System.Net.HttpListener]::new()
$listener.Prefixes.Add("http://127.0.0.1:$Port/")
$listener.Start()

Write-Host "Serving PWA at http://127.0.0.1:$Port/"
Write-Host "Root: $WebRoot"
Write-Host "Use 127.0.0.1 (not LAN IP). Close other tabs with this app open."
Write-Host "Press Ctrl+C to stop."

try {
    while ($listener.IsListening) {
        $context = $listener.GetContext()
        $request = $context.Request
        $response = $context.Response

        $response.Headers['Cross-Origin-Opener-Policy'] = 'same-origin'
        $response.Headers['Cross-Origin-Embedder-Policy'] = 'require-corp'
        $response.Headers['Cross-Origin-Resource-Policy'] = 'same-origin'

        $relativePath = [System.Uri]::UnescapeDataString($request.Url.AbsolutePath.TrimStart('/'))
        if ([string]::IsNullOrWhiteSpace($relativePath)) {
            $relativePath = 'index.html'
        }

        $fullPath = [System.IO.Path]::GetFullPath((Join-Path $WebRoot $relativePath))
        if (-not $fullPath.StartsWith($WebRoot, [StringComparison]::OrdinalIgnoreCase)) {
            Send-Response $response 403 'Forbidden'
            continue
        }

        if (-not (Test-Path $fullPath -PathType Leaf)) {
            if ($request.HttpMethod -eq 'GET' -and -not $relativePath.Contains('.')) {
                $fullPath = Join-Path $WebRoot 'index.html'
            }
            elseif (-not (Test-Path $fullPath -PathType Leaf)) {
                Send-Response $response 404 "Not found: $relativePath"
                continue
            }
        }

        $bytes = [System.IO.File]::ReadAllBytes($fullPath)
        $response.StatusCode = 200
        $response.ContentType = Get-ContentType $fullPath
        $response.ContentLength64 = $bytes.Length
        $response.OutputStream.Write($bytes, 0, $bytes.Length)
        $response.Close()
    }
}
finally {
    $listener.Stop()
    $listener.Close()
}