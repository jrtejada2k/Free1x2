# scripts/

Utilidades de build y verificacion para Free1X2.

## `build-and-test.ps1`

Mecanismo repetible para comprobar que "no se rompio nada": compila todos los
proyectos y ejecuta la suite de tests del dominio, con un resumen PASS/FAIL.

### Que hace

1. Compila en orden de dependencias:
   - `Free1X2.Domain` (logica de negocio, net8.0)
   - `Free1X2.Domain.Tests` (xUnit, net8.0)
   - `Free1X2` (WinForms, net8.0-windows)
   - `Free1X2.WinUI` (WinUI 3, net8.0-windows, con `-p:Platform=x64 -r win-x64`)
2. Ejecuta `dotnet test Free1X2.Domain.Tests`.
3. Imprime un resumen PASS/FAIL por proyecto + total de tests.
4. Devuelve **exit code != 0** si cualquier paso falla (apto para CI/hooks).

Los resultados de test se guardan en `artifacts/test-results/domain-tests.trx`.

### Uso

Desde la raiz del repo (o cualquier carpeta; el script resuelve la raiz solo):

```powershell
# Build Release + tests (por defecto)
pwsh ./scripts/build-and-test.ps1

# Build en Debug
pwsh ./scripts/build-and-test.ps1 -Configuration Debug

# Omitir WinUI (util en maquinas sin el workload de WinUI / runners headless)
pwsh ./scripts/build-and-test.ps1 -SkipWinUI
```

### Requisitos

- PowerShell 7+ (`pwsh`)
- .NET SDK 8.0 o superior (`dotnet --version`)
- En Windows con el workload de WinUI instalado para compilar `Free1X2.WinUI`.

### Nota sobre `Free1X2.WinUI`

La compilacion de WinUI 3 usa el compilador de XAML de Windows App SDK
(`XamlCompiler.exe`). En entornos sin el workload completo de WinUI o sin los
Windows SDK Build Tools adecuados, ese paso puede fallar (`error MSB3073`)
aunque el resto del codigo este sano. Si solo quieres validar dominio + tests
+ WinForms, usa `-SkipWinUI`. En CI (GitHub Actions) este paso esta marcado
como `continue-on-error` por el mismo motivo; ver `.github/workflows/ci.yml`.

## CI

El workflow `.github/workflows/ci.yml` ejecuta el mismo build + test en
`windows-latest` con .NET 8 en cada push/PR a `winui3-migration` y `main`.
