# Free1X2 — Contexto para Claude Code

## Qué es este proyecto

**Free1X2** es una herramienta de escritorio Windows para análisis estadístico de la Quiniela
española (14 partidos de fútbol con resultado 1/X/2, más el Pleno al 15). Permite construir un
boleto, aplicar condiciones y filtros matemáticos, generar y **reducir** columnas de apuesta,
escrutar resultados contra la ganadora y analizar históricos.

Programa libre bajo **GPLv3**, derivado del Free1X2 original de Joan Duatis.

## Stack

| Dato | Valor | Evidencia |
|------|-------|-----------|
| Versión | **0.83.0** «Rarotonga» | `Free1X2.WinUI/Free1X2.WinUI.csproj:15-17` |
| UI | WinUI 3 (Windows App SDK **1.6**, `Microsoft.WindowsAppSDK 1.6.250108002`) | `Free1X2.WinUI.csproj:20,37` |
| TFM | `net8.0-windows10.0.19041.0` (mínimo `10.0.17763.0`) | `Free1X2.WinUI.csproj:5-6` |
| Empaquetado | Desempaquetado (`WindowsPackageType=None`), **self-contained win-x64** | `Free1X2.WinUI.csproj:21,26-28` |
| MVVM | `CommunityToolkit.Mvvm 8.4.0` | `Free1X2.WinUI.csproj:39` |
| SDK requerido | .NET 8 SDK, Windows 10 19041+ / 11, plataforma x64 | — |

## Estructura

`Free1X2.sln` agrupa **4** proyectos:

| Proyecto | Rol |
|----------|-----|
| **`Free1X2.WinUI/`** | **Aplicación principal.** 108 páginas en `Views/Ported/`, registradas en `Navigation/PortedPages.cs` (108 entradas). Shell en `MainWindow.xaml.cs`, inicio en `App.xaml.cs`, servicios en `Services/` (`Log`, `TemaApp`, `IdiomaApp`, `PickerHelper`, `QuinielaOnlineService`, `JornadaCache`, `PaisesOnline`, `AppServices`, `AppState`). |
| **`Free1X2.Domain/`** | **Motor completo**, libre de UI: `MotorCalculo/`, `Reduccion/`, `Escrutinio/`, `EntradaSalida/`, `Analisis/`, `Utils/`, `SubirCategoria/`, `Online/`, `VariablesGlobales.cs`. Desacople de UI vía `Abstractions/UiHooks.cs` (`UiPump`, `UserDialogs`, `AnalisisUi`). |
| **`Free1X2/`** | UI **WinForms legacy** = **referencia de comportamiento**. Congelado en **0.77.2** por diseño (`Free1X2/Free1X2.csproj:11-12`). Ya no aloja el motor: solo quedan `UI/`, `Program.cs`, `Infraestructura/`, `Analisis/AnalisisCombinacion.cs` y 4 ficheros en `Utils/` (`Grafico.cs`, `ControlCompatibility.cs`, `ValidadorCaracteres.cs`, `CompresorZip.cs`). |
| **`Free1X2.Domain.Tests/`** | **133 tests** golden-master del motor (84 `[Fact]` + 49 casos `[InlineData]`). Red de seguridad de toda optimización. |

Además, fuera de la solución: `docs/` (documentación vigente), `scripts/` (build, publish, smoke,
stub de la API), `tools/`, y carpetas `Free1X2.Shared/` y `Free1X2.WebAPI/` que **no** están
referenciadas en `Free1X2.sln`.

## Comandos

```powershell
# Build de la app principal
dotnet build Free1X2.WinUI/Free1X2.WinUI.csproj -c Debug -p:Platform=x64   # 0 errores

# Tests del motor
dotnet test  Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj              # 133/133

# Smoke de carga de las 109 superficies (108 páginas + MainPage)
$env:FREE1X2_SMOKE = '1'
.\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe
Get-Content "$env:TEMP\free1x2_smoke.log" -Tail 1   # "SMOKE DONE total=109 ok=109 fail=0"

# Publicar portable self-contained
pwsh ./scripts/publish-winui.ps1
```

El smoke está **gated** por la variable `FREE1X2_SMOKE` y es inerte sin ella
(`Free1X2.WinUI/MainWindow.xaml.cs:43,562,587`). El script `scripts/smoke-winui.ps1` automatiza
la secuencia completa.

## Integración online (opcional)

Servicio de la jornada y del catálogo de equipos contra **clubprogol.com**. Contrato completo en
[`docs/API_CLUBPROGOL.md`](docs/API_CLUBPROGOL.md).

- Cliente: `Free1X2.WinUI/Services/QuinielaOnlineService.cs`.
  Base URL por defecto `https://clubprogol.com` (`:47`) + prefijo **`/wp-json/clubprogol/v1`**
  (`:50`). `HttpClient` estático con timeout de 10 s; manejo de `429`/`Retry-After`.
- **Desarrollo:** la variable de entorno **`FREE1X2_API_BASE`** (`:53`) sobreescribe la base URL;
  `scripts/stub-api.ps1` sirve los JSON de `docs/ejemplos-api/` en las rutas de la spec.
  Es una variable de desarrollo: **no se documenta en el manual de usuario**.
- **Caché offline-first:** `Free1X2.WinUI/Services/JornadaCache.cs` guarda el JSON crudo de la
  última jornada correcta en `%LocalAppData%\Free1X2\jornada-{es,mx}.json` (`:52`), y
  `App.xaml.cs:45` (`SembrarJornadaDesdeCache`) la siembra al arrancar **sin red**. La red solo se
  toca al pulsar «Actualizar jornada».

## Reglas del proyecto

Vigentes desde [`docs/PLAN_MEJORAS.md`](docs/PLAN_MEJORAS.md) §0:

| # | Regla | Consecuencia práctica |
|---|-------|-----------------------|
| R1 | **La lógica de negocio no cambia.** El motor (`Free1X2.Domain`) debe producir resultados idénticos al WinForms original (`Free1X2/`). | Toda optimización pasa los **133 tests golden-master** sin tocarlos. |
| R2 | **Las decisiones de diseño/UI las toma el dueño.** | Los hallazgos de UI son *opciones a considerar*; ninguna se implementa sin aprobación explícita. |
| R3 | **Completitud binaria (0 o 1).** | Un ítem solo se marca hecho con evidencia: build 0 err, smoke 109/109, tests verdes y `file:line` del cambio. |
| R4 | **Sin evidencia no hay hallazgo.** | Cada afirmación cita fichero:línea leído. Lo no verificado se descarta. |
| R5 | **Trabajo en agentes de fondo, serial cuando hay build.** | Un solo `dotnet build` a la vez (contención de `obj/`). |
| R6 | **No borrar ramas ni cambiar visibilidad del repo.** | Los residuos documentales se *mueven*, no se borran, salvo orden del dueño. |

Y, además:

- **Cuota de plan**: preguntar antes de ejecutar tareas masivas (subagents, búsquedas extensas).

## Documentación

| Documento | Contenido |
|-----------|-----------|
| [`docs/ANALISIS_TECNICO_WINUI3.md`](docs/ANALISIS_TECNICO_WINUI3.md) | Arquitectura de la capa WinUI 3 y verificación de la migración (§11). |
| [`docs/API_CLUBPROGOL.md`](docs/API_CLUBPROGOL.md) | Contrato HTTP del servicio online + cómo lo consume la app. |
| [`docs/MANUAL_USUARIO.md`](docs/MANUAL_USUARIO.md) | Manual de usuario: pantallas y menús. |
| [`docs/MANUAL_FLUJOS.md`](docs/MANUAL_FLUJOS.md) | Flujos funcionales y de datos del motor, con enlaces al código. |
| [`docs/PLAN_MEJORAS.md`](docs/PLAN_MEJORAS.md) | Plan de mejoras post-v0.82.0 (fases F1–F6, con evidencia `file:line`). |
