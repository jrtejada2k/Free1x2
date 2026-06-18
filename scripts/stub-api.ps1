# stub-api.ps1 — Servidor stub local de la API clubprogol.com para probar Free1X2 sin backend.
# Sirve los JSON de docs/ejemplos-api/ en las rutas de la spec (docs/API_CLUBPROGOL.md).
#
# Uso:
#   pwsh ./scripts/stub-api.ps1                 # escucha en http://localhost:8080
#   pwsh ./scripts/stub-api.ps1 -Port 9000      # otro puerto
#
# Luego, en OTRA terminal, antes de lanzar la app:
#   $env:FREE1X2_API_BASE = 'http://localhost:8080'
#   .\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe
#
# En la app: Descarga de boleto -> elige Espana/Mexico -> Actualizar jornada.
# (localhost no requiere permisos de admin; Ctrl+C para parar.)

param([int]$Port = 8080)

$ejemplos = Join-Path $PSScriptRoot '..\docs\ejemplos-api'
$rutas = @{
    '/api/v1/quiniela/es/actual' = 'quiniela-es-actual.json'
    '/api/v1/quiniela/mx/actual' = 'quiniela-mx-actual.json'
    '/api/v1/equipos/es'         = 'equipos-es.json'
}

$listener = [System.Net.HttpListener]::new()
$prefix = "http://localhost:$Port/"
$listener.Prefixes.Add($prefix)
$listener.Start()

Write-Host "Stub API en $prefix  (Ctrl+C para parar)" -ForegroundColor Green
Write-Host "Rutas servidas:"
$rutas.Keys | Sort-Object | ForEach-Object { Write-Host "  GET $prefix$($_.TrimStart('/'))" }
Write-Host ""
Write-Host "En la app, antes de lanzarla:" -ForegroundColor Yellow
Write-Host "  `$env:FREE1X2_API_BASE = '$($prefix.TrimEnd('/'))'"

try {
    while ($listener.IsListening) {
        $ctx  = $listener.GetContext()
        $path = $ctx.Request.Url.AbsolutePath.TrimEnd('/')
        $resp = $ctx.Response
        $resp.Headers['Content-Type'] = 'application/json; charset=utf-8'

        if ($rutas.ContainsKey($path)) {
            $file = Join-Path $ejemplos $rutas[$path]
            if (Test-Path $file) {
                $bytes = [System.IO.File]::ReadAllBytes($file)
                $resp.StatusCode = 200
                $resp.OutputStream.Write($bytes, 0, $bytes.Length)
            } else {
                $resp.StatusCode = 404
            }
        } else {
            $resp.StatusCode = 404
            $msg = [System.Text.Encoding]::UTF8.GetBytes('{"error":"not found"}')
            $resp.OutputStream.Write($msg, 0, $msg.Length)
        }

        $resp.OutputStream.Close()
        Write-Host "$($ctx.Request.HttpMethod) $path -> $($resp.StatusCode)"
    }
}
finally {
    $listener.Stop()
}
