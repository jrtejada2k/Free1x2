# Estado de la migración a WinUI 3 — hand-off

> **ACTUALIZACIÓN 2026-06-18 — migración completada y publicada.** La migración a WinUI 3 ya está
> **mergeada en `main`** y publicada (repositorio público, release `v0.81.2` con instalable portable).
> La UI está portada: **108 pantallas + `MainPage`**, cableadas en menús y barra de herramientas,
> con *smoke test* de carga 109/109, sobre el motor reutilizado en `Free1X2.Domain`. La lógica de
> dominio de las pantallas está **implementada y verificada** (build 0 err · smoke 109/109 · 107/107
> tests del motor · runtime UI Automation 0 crashes); los residuales son solo cosméticos. Detalle en
> [`docs/ANALISIS_TECNICO_WINUI3.md`](docs/ANALISIS_TECNICO_WINUI3.md) (§11).
> El texto inferior es el **hand-off histórico** (Fase 0) y se conserva como registro.

> Documento para retomar el trabajo exactamente donde se quedó.
> Última actualización: 2026-05-29.

## Dónde está cada cosa (ramas y PRs)

| Rama | Contenido | PR | Estado |
|------|-----------|----|--------|
| `main` | Base estable (WinForms legacy) | — | intacta |
| `ui-icons-palette` | Rediseño UI WinForms (Fluent: botones redondeados, paleta slate/índigo, iconos Segoe, reflow MainForm) | [#2](https://github.com/jrtejada2k/Free1x2/pull/2) | abierto, **pendiente verificación visual + merge** |
| `winui3-migration` | **Migración a WinUI 3** (esta rama) | [#3](https://github.com/jrtejada2k/Free1x2/pull/3) | abierto, en progreso |

> Nota: `ui-icons-palette` y `winui3-migration` son esfuerzos paralelos e independientes. La migración WinUI 3 reemplazará a la larga la UI WinForms; el rediseño WinForms (#2) es la mejora del estado actual mientras tanto.

## Validado (no es teoría)

- WinUI 3 (Windows App SDK **1.6.250108002**, TFM `net8.0-windows10.0.19041.0`) **compila, corre y renderiza Fluent nativo** en este entorno.
- **Self-contained** (`WindowsAppSDKSelfContained=true` + `SelfContained=true` + `RuntimeIdentifier=win-x64`): el runtime va empaquetado, sin instalar el Windows App Runtime a nivel sistema.
- Captura de ventanas de escritorio para verificación visual: vía PowerShell `PrintWindow(hwnd, hdc, 2)` (PW_RENDERFULLCONTENT) — funciona aunque la ventana esté detrás. Evidencia: `docs/winui3-shell.png`.

## Hecho

### Plan
- `PLAN_MIGRACION_WINUI3.md` (~93 KB): roadmap consolidado por 6 agentes (PM, Diseño, UI/UX, Tester, Performance + síntesis). Estrategia **strangler-fig**, 8 fases, 31–45 semanas-persona.
- `REVISION_UI_HALLAZGOS.md`: hallazgos previos de la UI WinForms.

### Scaffold WinUI 3 (`Free1X2.WinUI/`)
- Shell `NavigationView` + **Mica** + barra de título extendida.
- Sistema de diseño slate/índigo en `Themes/Tokens.xaml` (ThemeDictionaries Light/Dark, override de `SystemAccentColor` → `#4F46E5`).
- Vistas: `HomePage` (showcase), `BoletoPage` (slice: 14 partidos con toggles 1/X/2 + contador en vivo), `PlaceholderPage`, `SettingsPage` (tema claro/oscuro/sistema).
- MVVM con `CommunityToolkit.Mvvm 8.4.0`.

### Fase 0 — desacople de dominio (EN PROGRESO)
- **`Free1X2.Domain`** (net8.0, sin WinForms) creado con abstracciones:
  - `Abstractions/IProgressNotifier.cs` (↔ `Application.DoEvents`) + `NullProgressNotifier`.
  - `Abstractions/IAppPaths.cs` (↔ `Application.StartupPath`) + `AppPaths`.
- **Extraídos (namespaces conservados):** `Enums.cs`, `EnumFiltros.cs`, `Utils/{Combinacion, Comparador, Comparador2, RangosHelper, Tramo}.cs`.
- `Free1X2` (WinForms) y `Free1X2.WinUI` referencian `Free1X2.Domain`.
- **`Free1X2.Domain.Tests`** (xUnit): **10/10 tests** golden-master (RangosHelper, Comparador).
- Proyectos añadidos a `Free1X2.sln`.

### Fase 0b — quitar WinForms de 5 archivos (HECHO)
- `Application.StartupPath` → `AppContext.BaseDirectory` + `Path` separators; quitado `using System.Windows.Forms` en: `VariablesGlobales`, `EntradaSalida/{AConfiguracion, ArchivoIdioma, GenerarCPs/DatosHelper}`, `Utils/LAE`. Ya WinForms-free.
- Acoplamiento UI **real** que sigue en WinForms (no es solo path): `Utils/ValidadorCaracteres` (SendKeys), `Utils/Porcentajes` (MessageBox/Clipboard), `Utils/ControlCompatibility` (Form).

**Builds verdes:** Domain 0 err · WinForms 0 err · WinUI 0 err · tests 10/10.

## Próximos pasos (en orden) — retomar aquí

> Nota: `EntradaSalida` ya no usa WinForms, pero su **cierre de dependencias arrastra `MotorCalculo`** (Grupos/filtros). No se puede mover a Domain hasta desacoplar `MotorCalculo`. Por eso el siguiente paso es el refactor grande, no más mover hojas.

1. **`MotorCalculo` — el desacople grande** (es la presa):
   - `Analizador.cs`: quitar `using Free1X2.UI` + `VisorAnalisisColumnasFrm` (devolver `ContenedorAnalisisGlobal`, que la UI cree su visor); sustituir `Application.DoEvents` por `IProgressNotifier`.
   - `ControladoresImpresion.cs`: `Application.StartupPath` → `IAppPaths`.
2. **`Reduccion/`** (7 subclases con `Application.DoEvents`): mover `ReductorBase`/`IReductor`, inyectar `IProgressNotifier` (reemplazar los DoEvents).
3. **`Escrutinio/Escrutador.cs`**: reemplazar `DataSet` por DTO de dominio (`ResultadoEscrutinio`).
4. **Mover a Domain** (cuando 1–3 listos): `MotorCalculo` + `EntradaSalida` + `VariablesGlobales` + `Reduccion` + `Escrutinio` (conjunto cerrado), conservando namespaces.
5. **`Utils/Grafico.cs`** (filtros, controladores, modelos) a Domain una vez desacoplado `VariablesGlobales`. `Analizador.cs` necesita además quitar `using Free1X2.UI` (`VisorAnalisisColumnasFrm`) y usar `IProgressNotifier`.
4. **`Reduccion/`**: mover `ReductorBase`/`IReductor`, inyectar `IProgressNotifier` en las subclases (sustituir los `Application.DoEvents`).
5. **`Escrutinio/Escrutador.cs`**: reemplazar el `DataSet` por un DTO de dominio (`ResultadoEscrutinio`).
6. **`Utils/Grafico.cs`**: extraer el cálculo (Domain) y dejar el render `PictureBox` en UI.
7. Ampliar tests golden-master a cada pieza antes/después de moverla.
8. Luego: Fase 2 del plan (Boleto + primer filtro end-to-end con MVVM real sobre el dominio).

Ver detalle completo por fase en `PLAN_MIGRACION_WINUI3.md`.

## Comandos

```powershell
# Dominio
dotnet build Free1X2.Domain/Free1X2.Domain.csproj
dotnet test  Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj

# WinForms (app actual)
dotnet build Free1X2/Free1X2.csproj

# WinUI 3 (nueva UI) — requiere -p:Platform=x64 -r win-x64
dotnet build Free1X2.WinUI/Free1X2.WinUI.csproj -c Debug -p:Platform=x64 -r win-x64
.\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe
```

## Notas / gotchas

- En `Free1X2.csproj` se excluye `Infraestructura\ManejadorExcepciones.cs` (tipo duplicado con la implementación de `Debug/*`). No quitar ese `<Compile Remove>`.
- Al extraer dominio: **conservar los namespaces originales** (`Free1X2`, `Free1X2.Utils`, `Free1X2.MotorCalculo`, ...) para que los `using` de WinForms sigan resolviendo vía la `ProjectReference`, sin tocar los forms.
- Mover solo **conjuntos cerrados** de dependencias (verificar `using Free1X2.*` y referencias en el mismo namespace) y compilar tras cada lote.
