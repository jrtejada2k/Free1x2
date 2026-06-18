<#
.SYNOPSIS
    Compila todos los proyectos de Free1X2 y ejecuta la suite de tests del dominio.

.DESCRIPTION
    Mecanismo repetible de CI/build local. Compila en orden de dependencias:
        1. Free1X2.Domain            (logica de negocio pura, net8.0)
        2. Free1X2.Domain.Tests      (xUnit, net8.0)
        3. Free1X2                    (WinForms, net8.0-windows)
        4. Free1X2.WinUI             (WinUI 3, net8.0-windows + x64/win-x64)
    Luego corre 'dotnet test' sobre Free1X2.Domain.Tests.
    Al final imprime un resumen PASS/FAIL por proyecto y el total de tests,
    devolviendo un exit code != 0 si cualquier paso falla.

.PARAMETER Configuration
    Configuracion de build (Debug | Release). Por defecto: Release.

.PARAMETER SkipWinUI
    Omite la compilacion de Free1X2.WinUI (util en runners headless o sin
    workload de WinUI). Por defecto: $false.

.EXAMPLE
    pwsh ./scripts/build-and-test.ps1

.EXAMPLE
    pwsh ./scripts/build-and-test.ps1 -Configuration Debug -SkipWinUI
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [switch]$SkipWinUI
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Continue'

# --- Resolver la raiz del repo: el script vive en <repo>/scripts ------------
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot  = Split-Path -Parent $ScriptDir
Set-Location $RepoRoot

Write-Host ''
Write-Host '==================================================================' -ForegroundColor Cyan
Write-Host " Free1X2 :: build-and-test  (Configuration=$Configuration)" -ForegroundColor Cyan
Write-Host " Repo: $RepoRoot" -ForegroundColor Cyan
Write-Host '==================================================================' -ForegroundColor Cyan

# --- Comprobar que dotnet esta disponible -----------------------------------
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Host "ERROR: no se encontro 'dotnet' en el PATH." -ForegroundColor Red
    exit 127
}
Write-Host ("dotnet SDK: " + (dotnet --version)) -ForegroundColor DarkGray

# --- Acumulador de resultados ----------------------------------------------
$results = [System.Collections.Generic.List[object]]::new()

function Add-Result {
    param(
        [string]$Name,
        [string]$Status,   # PASS | FAIL | SKIP
        [string]$Detail = ''
    )
    $results.Add([pscustomobject]@{
        Name   = $Name
        Status = $Status
        Detail = $Detail
    })
}

# --- Helper de build --------------------------------------------------------
function Invoke-Build {
    param(
        [string]$Name,
        [string]$ProjectPath,
        [string[]]$ExtraArgs = @()
    )

    Write-Host ''
    Write-Host "------------------------------------------------------------------" -ForegroundColor Yellow
    Write-Host " BUILD: $Name" -ForegroundColor Yellow
    Write-Host "------------------------------------------------------------------" -ForegroundColor Yellow

    $args = @(
        'build', $ProjectPath,
        '-c', $Configuration,
        '--nologo',
        '-v', 'minimal'
    ) + $ExtraArgs

    Write-Host ("> dotnet " + ($args -join ' ')) -ForegroundColor DarkGray
    & dotnet @args
    $code = $LASTEXITCODE

    if ($code -eq 0) {
        Write-Host "[OK]   $Name compilo correctamente." -ForegroundColor Green
        Add-Result -Name $Name -Status 'PASS'
    }
    else {
        Write-Host "[FAIL] $Name fallo (exit $code)." -ForegroundColor Red
        Add-Result -Name $Name -Status 'FAIL' -Detail "build exit $code"
    }
    return $code
}

# --- 1. Free1X2.Domain ------------------------------------------------------
[void](Invoke-Build -Name 'Free1X2.Domain' `
    -ProjectPath 'Free1X2.Domain/Free1X2.Domain.csproj')

# --- 2. Free1X2.Domain.Tests (build) ----------------------------------------
[void](Invoke-Build -Name 'Free1X2.Domain.Tests' `
    -ProjectPath 'Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj')

# --- 3. Free1X2 (WinForms) --------------------------------------------------
[void](Invoke-Build -Name 'Free1X2 (WinForms)' `
    -ProjectPath 'Free1X2/Free1X2.csproj')

# --- 4. Free1X2.WinUI (x64 / win-x64) ---------------------------------------
if ($SkipWinUI) {
    Write-Host ''
    Write-Host '[SKIP] Free1X2.WinUI omitido (-SkipWinUI).' -ForegroundColor DarkYellow
    Add-Result -Name 'Free1X2.WinUI' -Status 'SKIP' -Detail 'omitido por parametro'
}
else {
    [void](Invoke-Build -Name 'Free1X2.WinUI' `
        -ProjectPath 'Free1X2.WinUI/Free1X2.WinUI.csproj' `
        -ExtraArgs @('-p:Platform=x64', '-r', 'win-x64'))
}

# --- 5. Tests del dominio ---------------------------------------------------
Write-Host ''
Write-Host "------------------------------------------------------------------" -ForegroundColor Yellow
Write-Host " TEST: Free1X2.Domain.Tests" -ForegroundColor Yellow
Write-Host "------------------------------------------------------------------" -ForegroundColor Yellow

$testProject = 'Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj'
$trxName     = "domain-tests.trx"
$trxDir      = Join-Path $RepoRoot 'artifacts/test-results'
New-Item -ItemType Directory -Force -Path $trxDir | Out-Null

$testArgs = @(
    'test', $testProject,
    '-c', $Configuration,
    '--nologo',
    '--no-build',
    '-v', 'minimal',
    '--logger', "trx;LogFileName=$trxName",
    '--results-directory', $trxDir
)
Write-Host ("> dotnet " + ($testArgs -join ' ')) -ForegroundColor DarkGray
& dotnet @testArgs
$testCode = $LASTEXITCODE

# --- Parsear el conteo de tests desde el .trx (si existe) -------------------
$testTotal   = $null
$testPassed  = $null
$testFailed  = $null
$trxPath = Join-Path $trxDir $trxName
if (Test-Path $trxPath) {
    try {
        [xml]$trx = Get-Content -Raw -Path $trxPath
        $counters = $trx.TestRun.ResultSummary.Counters
        if ($counters) {
            $testTotal  = [int]$counters.total
            $testPassed = [int]$counters.passed
            $testFailed = [int]$counters.failed
        }
    }
    catch {
        Write-Host "AVISO: no se pudo parsear el .trx ($($_.Exception.Message))." -ForegroundColor DarkYellow
    }
}

if ($testCode -eq 0) {
    $detail = if ($null -ne $testTotal) { "$testPassed/$testTotal passed" } else { 'ok' }
    Write-Host "[OK]   Tests PASARON ($detail)." -ForegroundColor Green
    Add-Result -Name 'TESTS: Free1X2.Domain.Tests' -Status 'PASS' -Detail $detail
}
else {
    $detail = if ($null -ne $testTotal) { "$testFailed failed de $testTotal" } else { "test exit $testCode" }
    Write-Host "[FAIL] Tests FALLARON ($detail)." -ForegroundColor Red
    Add-Result -Name 'TESTS: Free1X2.Domain.Tests' -Status 'FAIL' -Detail $detail
}

# --- Resumen final ----------------------------------------------------------
Write-Host ''
Write-Host '==================================================================' -ForegroundColor Cyan
Write-Host ' RESUMEN' -ForegroundColor Cyan
Write-Host '==================================================================' -ForegroundColor Cyan

foreach ($r in $results) {
    $color = switch ($r.Status) {
        'PASS' { 'Green' }
        'FAIL' { 'Red' }
        'SKIP' { 'DarkYellow' }
        default { 'Gray' }
    }
    $line = '  {0,-6} {1,-32} {2}' -f $r.Status, $r.Name, $r.Detail
    Write-Host $line -ForegroundColor $color
}

if ($null -ne $testTotal) {
    Write-Host ''
    Write-Host ("  Total tests: {0}  (passed: {1}, failed: {2})" -f $testTotal, $testPassed, $testFailed) -ForegroundColor Cyan
}

$failed = @($results | Where-Object { $_.Status -eq 'FAIL' })
Write-Host '------------------------------------------------------------------' -ForegroundColor Cyan
if ($failed.Count -gt 0) {
    Write-Host (" RESULTADO GLOBAL: FAIL  ($($failed.Count) paso(s) con error)") -ForegroundColor Red
    exit 1
}
else {
    Write-Host ' RESULTADO GLOBAL: PASS' -ForegroundColor Green
    exit 0
}
