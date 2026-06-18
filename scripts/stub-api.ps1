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
#
# Rutas servidas (mismo prefijo que el backend real /wp-json/clubprogol/v1/):
#   GET /wp-json/clubprogol/v1/quiniela/es/actual
#   GET /wp-json/clubprogol/v1/quiniela/mx/actual
#   GET /wp-json/clubprogol/v1/equipos/es
#   GET /wp-json/clubprogol/v1/equipos/mx
# Simular errores (para probar el manejo del cliente):
#   ?sim=404  -> 404 {"error":"sin_jornada","mensaje":"No hay jornada publicada para ES."}
#   ?sim=429  -> 429 + Retry-After: 60  {"error":"rate_limited","mensaje":"..."}

param([int]$Port = 8080)

$ejemplos = Join-Path $PSScriptRoot '..\docs\ejemplos-api'
# Rutas del backend real (WordPress REST de clubprogol.com): prefijo /wp-json/clubprogol/v1/.
$rutas = @{
    '/wp-json/clubprogol/v1/quiniela/es/actual' = 'quiniela-es-actual.json'
    '/wp-json/clubprogol/v1/quiniela/mx/actual' = 'quiniela-mx-actual.json'
    '/wp-json/clubprogol/v1/equipos/es'         = 'equipos-es.json'
    '/wp-json/clubprogol/v1/equipos/mx'         = 'equipos-mx.json'
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
        $sim  = $ctx.Request.QueryString['sim']   # ?sim=404 | ?sim=429 para probar errores
        $resp = $ctx.Response
        $resp.Headers['Content-Type'] = 'application/json; charset=utf-8'

        if ($sim -eq '429') {
            # Simula el límite de peticiones (60/min) del backend real.
            $resp.StatusCode = 429
            $resp.Headers['Retry-After'] = '60'
            $body = '{"error":"rate_limited","mensaje":"Demasiadas solicitudes (limite 60/min). Intenta de nuevo en ~60 s."}'
            $msg = [System.Text.Encoding]::UTF8.GetBytes($body)
            $resp.OutputStream.Write($msg, 0, $msg.Length)
        }
        elseif ($sim -eq '404') {
            # Simula 'no hay jornada publicada' con el cuerpo de error del contrato.
            $resp.StatusCode = 404
            $body = '{"error":"sin_jornada","mensaje":"No hay jornada publicada para ES."}'
            $msg = [System.Text.Encoding]::UTF8.GetBytes($body)
            $resp.OutputStream.Write($msg, 0, $msg.Length)
        }
        elseif ($rutas.ContainsKey($path)) {
            $file = Join-Path $ejemplos $rutas[$path]
            if (Test-Path $file) {
                $bytes = [System.IO.File]::ReadAllBytes($file)
                $resp.StatusCode = 200
                $resp.OutputStream.Write($bytes, 0, $bytes.Length)
            } else {
                $resp.StatusCode = 404
                $msg = [System.Text.Encoding]::UTF8.GetBytes('{"error":"sin_jornada","mensaje":"No hay jornada publicada."}')
                $resp.OutputStream.Write($msg, 0, $msg.Length)
            }
        } else {
            $resp.StatusCode = 404
            $msg = [System.Text.Encoding]::UTF8.GetBytes('{"error":"not_found","mensaje":"Ruta no encontrada."}')
            $resp.OutputStream.Write($msg, 0, $msg.Length)
        }

        $resp.OutputStream.Close()
        Write-Host "$($ctx.Request.HttpMethod) $path -> $($resp.StatusCode)"
    }
}
finally {
    $listener.Stop()
}
