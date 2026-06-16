<#
.SYNOPSIS
    Publica Free1X2.WinUI como app autocontenida y desempaquetada (WinUI 3 unpackaged).

.DESCRIPTION
    Ejecuta 'dotnet publish' sobre Free1X2.WinUI con configuracion Release, plataforma
    x64 y RID win-x64. El csproj ya define WindowsPackageType=None, SelfContained=true y
    WindowsAppSDKSelfContained=true, por lo que el resultado es una carpeta autonoma con
    Free1X2.WinUI.exe, el Windows App Runtime incrustado y los ficheros semilla de Datos/.

    Al terminar imprime la ruta de la carpeta de publicacion, el tamano del exe y el
    tamano total de la carpeta, listo para empaquetar/distribuir.

.PARAMETER Configuration
    Configuracion de build (Debug | Release). Por defecto: Release.

.EXAMPLE
    pwsh ./scripts/publish-winui.ps1

.EXAMPLE
    pwsh ./scripts/publish-winui.ps1 -Configuration Debug
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Continue'

# --- Resolver la raiz del repo: el script vive en <repo>/scripts ------------
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot  = Split-Path -Parent $ScriptDir
Set-Location $RepoRoot

$Project   = 'Free1X2.WinUI/Free1X2.WinUI.csproj'
$Rid       = 'win-x64'

Write-Host ''
Write-Host '==================================================================' -ForegroundColor Cyan
Write-Host " Free1X2.WinUI :: publish  (Configuration=$Configuration, RID=$Rid)" -ForegroundColor Cyan
Write-Host " Repo: $RepoRoot" -ForegroundColor Cyan
Write-Host '==================================================================' -ForegroundColor Cyan

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Host "ERROR: no se encontro 'dotnet' en el PATH." -ForegroundColor Red
    exit 127
}
Write-Host ("dotnet SDK: " + (dotnet --version)) -ForegroundColor DarkGray

$publishArgs = @(
    'publish', $Project,
    '-c', $Configuration,
    '-p:Platform=x64',
    '-r', $Rid,
    '--nologo'
)

Write-Host ''
Write-Host ("> dotnet " + ($publishArgs -join ' ')) -ForegroundColor DarkGray
& dotnet @publishArgs
$code = $LASTEXITCODE

if ($code -ne 0) {
    Write-Host ''
    Write-Host "[FAIL] publish fallo (exit $code)." -ForegroundColor Red
    exit $code
}

# --- Carpeta de salida estandar del publish ---------------------------------
$OutDir = Join-Path $RepoRoot "Free1X2.WinUI/bin/x64/$Configuration/net8.0-windows10.0.19041.0/$Rid/publish"
$Exe    = Join-Path $OutDir 'Free1X2.WinUI.exe'

Write-Host ''
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow
Write-Host ' RESULTADO' -ForegroundColor Yellow
Write-Host '------------------------------------------------------------------' -ForegroundColor Yellow

if (-not (Test-Path $OutDir)) {
    Write-Host "[FAIL] No se encontro la carpeta de publicacion esperada:" -ForegroundColor Red
    Write-Host "       $OutDir" -ForegroundColor Red
    exit 1
}

Write-Host ("Carpeta publish : {0}" -f $OutDir) -ForegroundColor Green

if (Test-Path $Exe) {
    $exeMB = [math]::Round((Get-Item $Exe).Length / 1MB, 2)
    Write-Host ("Ejecutable      : {0}  ({1} MB)" -f $Exe, $exeMB) -ForegroundColor Green
}
else {
    Write-Host "[AVISO] No se encontro Free1X2.WinUI.exe en la carpeta de publicacion." -ForegroundColor DarkYellow
}

$totalBytes = (Get-ChildItem -Path $OutDir -Recurse -File | Measure-Object -Property Length -Sum).Sum
$totalMB    = [math]::Round($totalBytes / 1MB, 2)
$fileCount  = (Get-ChildItem -Path $OutDir -Recurse -File | Measure-Object).Count
Write-Host ("Tamano total    : {0} MB  ({1} ficheros)" -f $totalMB, $fileCount) -ForegroundColor Green

# Comprobacion rapida de que los datos semilla se copiaron junto al exe.
$datosDir = Join-Path $OutDir 'Datos'
$parametros = Join-Path $OutDir 'parametros.free1x2'
if ((Test-Path $datosDir) -or (Test-Path $parametros)) {
    Write-Host "Datos semilla   : OK (Datos/ y/o parametros.free1x2 presentes)" -ForegroundColor Green
}
else {
    Write-Host "Datos semilla   : AVISO - no se encontraron ficheros de Datos en la salida." -ForegroundColor DarkYellow
}

Write-Host ''
Write-Host '[OK] Publish completado.' -ForegroundColor Green
exit 0
