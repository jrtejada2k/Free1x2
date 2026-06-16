<#
.SYNOPSIS
    Smoke test de runtime para Free1X2.WinUI: arranca la app y navega a todas las pantallas.

.DESCRIPTION
    1. Compila Free1X2.WinUI en Debug x64 (RID win-x64).
    2. Lanza Free1X2.WinUI.exe con la variable de entorno FREE1X2_SMOKE=1.
       En ese modo, MainWindow recorre con try/catch todas las pantallas portadas
       (PortedPagesRegistry.All) mas MainPage/HomePage, registra los fallos en
       %TEMP%\free1x2_smoke.log, escribe 'SMOKE DONE total=.. ok=.. fail=..' y cierra
       la app llamando a Application.Current.Exit().
    3. Espera hasta 60s a que el proceso termine.
    4. Comprueba que el proceso salio con exit code 0 y que el log contiene una linea
       'SMOKE DONE ... fail=0'. Devuelve exit 0 si todo paso; != 0 en caso contrario.

    El arnes esta GATED por FREE1X2_SMOKE: en ejecucion normal de la app es un no-op.

.PARAMETER TimeoutSeconds
    Segundos maximos de espera a que la app termine el recorrido. Por defecto: 60.

.EXAMPLE
    pwsh ./scripts/smoke-winui.ps1
#>
[CmdletBinding()]
param(
    [int]$TimeoutSeconds = 60
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Continue'

# --- Resolver la raiz del repo: el script vive en <repo>/scripts ------------
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot  = Split-Path -Parent $ScriptDir
Set-Location $RepoRoot

$Project       = 'Free1X2.WinUI/Free1X2.WinUI.csproj'
$Configuration = 'Debug'
$Rid           = 'win-x64'
$LogPath       = Join-Path $env:TEMP 'free1x2_smoke.log'

Write-Host ''
Write-Host '==================================================================' -ForegroundColor Cyan
Write-Host ' Free1X2.WinUI :: smoke test (FREE1X2_SMOKE=1)' -ForegroundColor Cyan
Write-Host " Repo: $RepoRoot" -ForegroundColor Cyan
Write-Host '==================================================================' -ForegroundColor Cyan

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Host "ERROR: no se encontro 'dotnet' en el PATH." -ForegroundColor Red
    exit 127
}
Write-Host ("dotnet SDK: " + (dotnet --version)) -ForegroundColor DarkGray

# --- 1. Build Debug x64 -----------------------------------------------------
Write-Host ''
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow
Write-Host " BUILD: Free1X2.WinUI ($Configuration / $Rid)" -ForegroundColor Yellow
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow

$buildArgs = @('build', $Project, '-c', $Configuration, '-p:Platform=x64', '-r', $Rid, '--nologo', '-v', 'minimal')
Write-Host ("> dotnet " + ($buildArgs -join ' ')) -ForegroundColor DarkGray
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Host "[FAIL] La compilacion fallo (exit $LASTEXITCODE)." -ForegroundColor Red
    exit $LASTEXITCODE
}

# --- 2. Localizar el exe ----------------------------------------------------
$ExeDir = Join-Path $RepoRoot "Free1X2.WinUI/bin/x64/$Configuration/net8.0-windows10.0.19041.0/$Rid"
$Exe    = Join-Path $ExeDir 'Free1X2.WinUI.exe'
if (-not (Test-Path $Exe)) {
    Write-Host "[FAIL] No se encontro el ejecutable:" -ForegroundColor Red
    Write-Host "       $Exe" -ForegroundColor Red
    exit 1
}
Write-Host ("Ejecutable: {0}" -f $Exe) -ForegroundColor DarkGray

# --- 3. Lanzar la app con FREE1X2_SMOKE=1 -----------------------------------
if (Test-Path $LogPath) { Remove-Item $LogPath -Force -ErrorAction SilentlyContinue }

Write-Host ''
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow
Write-Host ' SMOKE: lanzando la app...' -ForegroundColor Yellow
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow

$env:FREE1X2_SMOKE = '1'
try {
    $proc = Start-Process -FilePath $Exe -PassThru -WorkingDirectory $ExeDir
}
finally {
    Remove-Item Env:\FREE1X2_SMOKE -ErrorAction SilentlyContinue
}

if (-not $proc) {
    Write-Host "[FAIL] No se pudo arrancar el proceso." -ForegroundColor Red
    exit 1
}

$exited = $proc.WaitForExit($TimeoutSeconds * 1000)
if (-not $exited) {
    Write-Host "[FAIL] La app no termino en ${TimeoutSeconds}s. Matando proceso $($proc.Id)." -ForegroundColor Red
    try { $proc.Kill($true) } catch { }
    if (Test-Path $LogPath) {
        Write-Host '--- contenido del log ---' -ForegroundColor DarkGray
        Get-Content $LogPath | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }
    }
    exit 1
}

$exitCode = $proc.ExitCode

# --- 4. Validar resultado ---------------------------------------------------
Write-Host ''
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow
Write-Host ' RESULTADO' -ForegroundColor Yellow
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow
Write-Host ("Exit code de la app : {0}" -f $exitCode) -ForegroundColor DarkGray

if (-not (Test-Path $LogPath)) {
    Write-Host "[FAIL] No se genero el log $LogPath." -ForegroundColor Red
    exit 1
}

$logLines = Get-Content $LogPath
Write-Host '--- log ---' -ForegroundColor DarkGray
$logLines | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }

$doneLine = $logLines | Where-Object { $_ -match '^SMOKE DONE ' } | Select-Object -Last 1
if (-not $doneLine) {
    Write-Host "[FAIL] El log no contiene una linea 'SMOKE DONE'." -ForegroundColor Red
    exit 1
}

# Parsear total/ok/fail
$total = $null; $ok = $null; $fail = $null
if ($doneLine -match 'total=(\d+)') { $total = [int]$Matches[1] }
if ($doneLine -match 'ok=(\d+)')    { $ok    = [int]$Matches[1] }
if ($doneLine -match 'fail=(\d+)')  { $fail  = [int]$Matches[1] }

Write-Host ''
Write-Host ("Pantallas: total={0} ok={1} fail={2}" -f $total, $ok, $fail) -ForegroundColor Cyan

$problemas = $false
if ($exitCode -ne 0) {
    Write-Host "[FAIL] La app no salio con exit 0 (exit $exitCode)." -ForegroundColor Red
    $problemas = $true
}
if ($null -eq $fail -or $fail -ne 0) {
    Write-Host "[FAIL] El recorrido tuvo fallos (fail=$fail). Ver el log arriba." -ForegroundColor Red
    $problemas = $true
}

Write-Host '------------------------------------------------------------------' -ForegroundColor Cyan
if ($problemas) {
    Write-Host ' RESULTADO GLOBAL: FAIL' -ForegroundColor Red
    exit 1
}
Write-Host (" RESULTADO GLOBAL: PASS  ({0}/{0} pantallas OK)" -f $ok) -ForegroundColor Green
exit 0
