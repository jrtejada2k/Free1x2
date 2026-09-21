# Plan de Migración a WinUI 3 — Free1X2

> Generado por un equipo de agentes (PM, Diseño, UI/UX, Tester, Performance) que analizaron el repositorio real. Fundación técnica ya validada: el proyecto `Free1X2.WinUI` compila, corre y renderiza Fluent nativo (self-contained).

> Fecha: 2026-05-29 · Rama: `winui3-migration`


---

## 0. Resumen Ejecutivo y Roadmap

### 0.1 Veredicto

**Migración VIABLE y recomendada.** La condición técnica que hace viable el proyecto ya está verificada en el repo: el andamiaje WinUI 3 existe y está validado (`Free1X2.WinUI.csproj` con Windows App SDK `1.6.250108002`, `net8.0-windows10.0.19041.0`, `CommunityToolkit.Mvvm 8.4.0`, self-contained `win-x64` que compila, corre y renderiza Fluent nativo), y el dominio está **débilmente acoplado** a WinForms: solo 15 archivos referencian `System.Windows.Forms` (uso concentrado en `Application.DoEvents()`, `Application.StartupPath`, `MessageBox`) y **0 referencias a `System.Drawing`** en la lógica de cálculo. El desacople es plomería de abstracción, no reescritura algorítmica.

El factor de viabilidad decisivo es que **el motor de dominio se reutiliza intacto** (regla del proyecto: no cambiar la lógica de negocio). Las dos UIs (WinForms vivo + WinUI nueva) referenciarán el mismo `Free1X2.Domain`, lo que garantiza paridad funcional por construcción.

**Esfuerzo total estimado: 31–45 semanas-persona.**
- 1 dev senior con experiencia WinUI/MVVM: **~7–11 meses** de calendario.
- 2 devs en paralelo desde la Fase 2 (las olas de forms homogéneos — filtros, análisis — son paralelizables): **~4–6 meses**.
- Recomendación: arrancar con 1 dev hasta consolidar el patrón end-to-end (Fases 0–2), luego escalar a 2.

Alcance: **109 superficies de UI** a migrar (65 `Form` + 44 `UserControl`), un `MainForm` con menú de ~294 ítems que es el árbol de navegación de toda la app, y grids combinatorios de hasta **4.782.969 columnas** (3¹⁴) que hoy no tienen virtualización y son el principal reto de performance.

### 0.2 Roadmap consolidado por fases

Las cinco secciones (PM, Diseño, UX, Tester, Performance) coinciden en una estrategia **incremental tipo strangler-fig** (no big-bang) con olas por dominio funcional. El roadmap unifica sus numeraciones. Las duraciones son para 1 dev senior; las fases marcadas *(∥)* se acortan a la mitad con 2 devs.

| Fase | Nombre | Contenido integrado (PM + Diseño + UX + Tester + Perf) | Duración |
|---|---|---|---|
| **0** | **Fundación: desacople de dominio + red de seguridad** | Crear `Free1X2.Domain` (`net8.0`, sin `UseWindowsForms`); mover `MotorCalculo/`, `Reduccion/`, `Escrutinio/`, `EntradaSalida/`, `Utils/`, `VariablesGlobales`. Reemplazar `Application.DoEvents`→`IProgress<T>`/async; `Application.StartupPath`→`IAppPaths`; `MessageBox`→`IUserDialogs`. Sustituir el `DataSet` de `Escrutador` por POCO `ResultadoEscrutinio`. **EN PARALELO (va primero, gobierna el refactor):** proyecto `Free1X2.Domain.Tests` (xUnit + FluentAssertions) con characterization tests; meta ≥80% cobertura en motor/reducción/escrutinio y 100% en `UtilColumnas`; **golden master** del motor capturado desde WinForms. | **3–5 sem** |
| **1** | **Shell WinUI + sistema de diseño + MVVM base** | `MainWindow` con `NavigationView` (mapea el menú de 294 ítems a ~10 categorías), `SystemBackdrop=Mica`, barra de título extendida. Sistema de diseño Fluent declarativo: `Themes/Colors.xaml` (ThemeDictionaries Light/Dark/HighContrast con la paleta slate/indigo), `Typography.xaml`, `Sizes.xaml`, override de `SystemAccentColor`→indigo. DI + `INavigationService` + `ViewModelLocator`. `Semaforo` WinUI (UserControl con `InfoBadge`/Ellipse + DP `Estado`) y estilos de botón de filtro. **Spikes de riesgo R2/R3** (DataGrid, gráficas, PrintManager, control de boleto compuesto). Navegación perezosa + R2R para proteger el arranque. Smoke test de arranque como gate de CI. | **3–4 sem** |
| **2** | **Slice núcleo: Boleto + primer filtro end-to-end** *(∥ a partir de aquí)* | Página de boleto (`BoletoFrm` + 6 controles compuestos `ControlBoleto`/`PartidoBoleto`/`SignoBoletoBase` rediseñados como `ItemsRepeater`+`DataTemplate` con `x:Bind`), contador en vivo Fijos/Dobles/Triples, alta de equipos/P15. Patrón `SplitView`: filtro configurable a la derecha con el boleto siempre visible. Tests de ViewModel del boleto y del primer filtro. **Valida el patrón más difícil (control compuesto custom) temprano para eliminar incertidumbre.** | **3–5 sem** |
| **3** | **Filtros (28 superficies)** *(∥)* | 21 forms de `Filtros/` + combinadores (`CombinarFiltros`, `DiFiltros`, `FiltroPim`, `FiltroPorcenJB`) + `CtrSemaforo`. Paneles deslizantes (no modales) con aplicación en vivo del semáforo y contador de "columnas que pasan". Forms homogéneos → economía de escala con plantillas. Tests parametrizados sobre los 24 `IFiltro`. | **5–7 sem** |
| **4** | **Columnas / Reducción (12)** *(∥)* | `CalculaColumnas`, `AlgebraColumnasFrm`, `FraccionadorFrm`, `FrmReducidasPerfectas`, `Compresor`, etc. Página dedicada con grid virtualizado. **Aquí entra la pieza crítica de performance:** colección `ISupportIncrementalLoading` que envuelve `GeneradorColumnas.ObtenerRango()` sin materializar las 4,78M columnas, `ListView`/`ItemsRepeater` virtualizado con `x:Bind Mode=OneTime`, cálculo siempre en `Task.Run`. Golden-file tests de reducción byte-a-byte. | **3–4 sem** |
| **5** | **Análisis (26)** *(∥)* | 19 UserControls de `Controls/Analisis/` + 7 forms de análisis. Volumen homogéneo, se ejecuta con el patrón UserControl→WinUI ya consolidado. `DataGrid` del Toolkit solo para grids pequeños/medios; `ItemsRepeater` virtualizado para visores de alto volumen. | **5–7 sem** |
| **6** | **Escrutinio + Estadísticas + Impresión (19)** | 6 forms de escrutinio, 5 de estadísticas (sustituir `System.Windows.Forms.DataVisualization` por LiveCharts2/Win2D), 8 de impresión/E-S (`PrintManager`/render a PDF en vez de `PrintDocument`). Riesgo técnico alto → se ataca con el equipo ya experto. | **5–7 sem** |
| **7** | **Auxiliares + banco de pruebas + corte** | Banco de pruebas/tramos (6), ~15 diálogos auxiliares, `ConfiguracionFrm`→`SettingsItem`, ayuda/acerca-de. **Inversión del arranque:** `Program.cs` lanza la `MainWindow` WinUI como shell principal. Retirada de `Free1X2.csproj` (WinForms) y de `UI/Modern/**`. Empaquetado self-contained final validado. | **4–6 sem** |

**Coexistencia durante la transición:** WinForms sigue siendo la app shippable y el "oráculo vivo" para comparación lado a lado. WinUI 3 **no hospeda WinForms** de forma robusta, así que la coexistencia es por **ventanas hermanas** (un menú "Probar nueva UI" en MainForm lanza la ventana WinUI; ambas comparten `Free1X2.Domain`), no por XAML Islands. El arranque se invierte solo cuando el flujo crítico (boleto + filtros + columnas) está migrado.

### 0.3 Decisiones arquitectónicas clave

| Decisión | Resolución | Justificación |
|---|---|---|
| **TFM WinUI** | `net8.0-windows10.0.19041.0` | Ya fijado y validado en el andamiaje; Segoe UI Variable y Mica disponibles nativamente. |
| **TFM dominio** | `net8.0` puro (sin `UseWindowsForms`) | Permite que tanto WinForms como WinUI lo referencien; headless y testeable. |
| **Framework MVVM** | `CommunityToolkit.Mvvm 8.4.0` | Ya en el `.csproj`; `[ObservableProperty]`/`[RelayCommand]` con source generators (sin reflexión), ideal para los ViewModels testeables. |
| **DI** | `Microsoft.Extensions.DependencyInjection` + `INavigationService` + `ViewModelLocator` | Inyecta los servicios de dominio (`IAnalysisService`, `IFilterService`, etc. ya extraídos en `Free1X2.Shared/Services/`) en los VM; permite mockear en tests. |
| **Estructura de solución** | `Free1X2.Domain` (nuevo, net8.0), `Free1X2.Shared` (contratos, existente), `Free1X2.WinUI` (UI nueva), `Free1X2.WebAPI` (existente), `Free1X2` (WinForms, se retira en Fase 7) + 2 proyectos de test: `Free1X2.Domain.Tests` y `Free1X2.WinUI.Tests` | `Free1X2.Shared` como única fuente de contratos; consolidar modelos duplicados ahí en Fase 0. |
| **Cómo separar el dominio** | Mover físicamente las carpetas de cálculo a `Free1X2.Domain`; abstraer las 3 dependencias UI (`DoEvents`→`IProgress`/async, `StartupPath`→`IAppPaths`, `MessageBox`→`IUserDialogs`); aislar el estado global `VariablesGlobales` con helper de fijación determinista en tests | Solo 15 archivos tocados, 0 referencias a `System.Drawing` → desacople de bajo riesgo si va respaldado por los characterization tests. |
| **Sistema de diseño** | ResourceDictionaries declarativos con `ThemeDictionaries` (Light/Dark/HighContrast), reemplazando el reskin runtime de `ModernTheme.cs`. Override de `SystemAccentColor`→`#4F46E5`. Mica Base en ventana; superficies de datos sólidas (no Acrylic, por legibilidad de celdas 1/X/2) | Tema nativo conmutable por SO; elimina `ModernTheme.cs`/`NeoToolStripRenderer`/`UI/Modern/Icons` (→ `FontIcon`/`SymbolIcon`). |
| **Grids masivos** | NO `DataGrid` del Toolkit para los combinatorios; `ListView`/`ItemsRepeater` virtualizado + `ISupportIncrementalLoading` + `x:Bind Mode=OneTime`, datos perezosos desde `GeneradorColumnas`. `DataGrid` solo para grids pequeños/medios | El patrón actual (`DataTable`/`DataSet` materializado, 0 virtualización en el repo) no escala a 4,78M filas. |
| **Empaquetado** | Self-contained, unpackaged (sin MSIX), `WindowsAppSDKSelfContained=true`, R2R activado, **sin** Native AOT ni Trimming en la primera entrega | Usuarios finales no técnicos (comunidad de quinielas); conserva el modelo ".exe suelto" actual; R2R mitiga el arranque más lento de WinUI; AOT no soportado por WinUI 3, trimming demasiado frágil con el patrón DataSet/reflexión. |

### 0.4 Primera slice recomendada (empezar YA)

**Slice 0 (bloqueante, no negociable): `UtilColumnas` + característica de un `IFiltro` con tests, y extracción mínima del dominio.** Antes de cualquier UI, escribir los characterization tests de `Free1X2/Utils/UtilColumnas.cs` (round-trip `ConvStrToLong`↔`ConvLongToStr`, `ObtenerSigno`, `ContarBitsA1`) y de `FiltroContactos.Analizar(long)` — son lógica de bits pura, sin UI, el cimiento de todo. Esto establece la red de seguridad y el golden master.

**Primera slice de UI end-to-end: la página de Boleto (`BoletoFrm` + `ControlBoleto`).**

Por qué el boleto y no un filtro simple:
1. **Es el control más difícil de toda la UI** (control compuesto custom con dibujo propio: `ControlBoleto`/`ControlColumnaBoleto`/`PartidoBoleto`/`SignoBoletoBase`, evento `PronosticoChanged`). Portarlo primero **descubre las incógnitas de rediseño** (`ItemsRepeater`+`DataTemplate`+binding) cuando aún hay margen para ajustar el patrón, en lugar de chocar con ellas en la Ola 5.
2. **Es la entrada de datos sin la cual nada funciona** — máximo uso, valor inmediato visible.
3. **Ejercita el stack completo end-to-end**: dominio extraído → servicio → ViewModel (`CommunityToolkit.Mvvm`) → vista XAML con `x:Bind` → tema Fluent → test de ViewModel. Valida que toda la arquitectura está conectada antes de invertir en 100+ superficies más.
4. Da un artefacto demostrable temprano (boleto de 14 partidos editable con contador en vivo) para validar la IA de navegación con el usuario.

Si se prefiere un riesgo menor para el primerísimo paso de UI, portar **un filtro de pocos campos** (p.ej. `DistanciasFrm` o `ContactosFrm`) como warm-up del patrón VM→vista→tema, e inmediatamente después el boleto. Pero el boleto debe ser la primera slice "real", porque es el descubridor de riesgo R3.

### 0.5 Top 5 riesgos con mitigación

| # | Riesgo | Impacto | Mitigación |
|---|---|---|---|
| **R1** | **Dominio acoplado al proyecto WinForms** (`DoEvents` ×20, `StartupPath`, `MessageBox` en 15 archivos; estado global static `VariablesGlobales` leído en casi todos los constructores) | Alto | Fase 0 dedicada: `Free1X2.Domain` + abstracciones `IProgress`/`IAppPaths`/`IUserDialogs`; convertir bucles `DoEvents` a `async`/`Task`. Aislar `VariablesGlobales` con helper de fijación determinista y colecciones de test en serie. **Cada paso del refactor con los characterization tests en verde.** |
| **R2** | **Grids combinatorios de 4,78M filas sin virtualización** — el patrón actual (`DataTable` materializado, 0 `VirtualMode` en el repo) colapsa en RAM/GDI+ | Alto | `ISupportIncrementalLoading` envolviendo `GeneradorColumnas.ObtenerRango()`; `ListView`/`ItemsRepeater` virtualizado + `x:Bind Mode=OneTime`; cálculo en `Task.Run`. Validar que el working set queda **plano** al hacer scroll (señal de virtualización viva). Spike en Fase 1. |
| **R3** | **Controles compuestos custom de boleto** (dibujo + lógica + `PronosticoChanged`) sin equivalente directo WinUI | Alto | Rediseño como `UserControl` XAML + `ItemsRepeater`/`DataTemplate` con binding; **abordarlo en la primera slice (Fase 2)** para descubrir incógnitas pronto. |
| **R4** | **Paridad funcional difícil de verificar en 109 superficies** | Alto | **Golden master del motor** capturado desde WinForms y reproducido byte-a-byte por WinUI en cada PR (gate duro de merge); checklist de paridad por form; WinForms vivo como oráculo de comparación lado a lado. |
| **R5** | **Arranque de WinUI más lento que WinForms** (carga del Windows App Runtime + parsing XAML de ~166 vistas) degradaría la base actual (~0,8–1,5 s cold) | Medio-Alto | R2R activado (mayor win de arranque); navegación perezosa (instanciar cada `Page` solo al navegar, `NavigationCacheMode=Disabled` salvo 2–3 vistas calientes); shell inicial mínimo; gate de CI que mide cold/warm start (mediana de N≥10) y falla si supera los topes (cold ≤2,5 s, warm ≤1,2 s). |

### 0.6 Definición de "terminado" de la migración

La migración se considera **completa** cuando se cumplen TODOS estos criterios:

1. **Cobertura funcional total:** 0 superficies pendientes en WinForms; las 109 superficies tienen su equivalente WinUI 3 navegable y funcional.
2. **Paridad funcional verificada (gate duro):** dado idéntico input de dominio, `salida_WinUI == salida_WinForms` para generación, reducción, escrutinio y estadísticas, demostrado por el golden master automatizado en CI. Cobertura de dominio ≥80% líneas (100% en `UtilColumnas` y filtros).
3. **Retirada del legacy:** `Free1X2.csproj` (WinForms) y `Free1X2/UI/Modern/**` (reskin runtime, `ModernTheme.cs`, `NeoToolStripRenderer`, iconos legacy) eliminados del repo; arranque invertido a la `MainWindow` WinUI.
4. **Arquitectura limpia:** todo el cálculo en `Free1X2.Domain` (net8.0, 0 referencias a `System.Windows.Forms`/`System.Drawing`); ViewModels sin código WinForms; contratos consolidados en `Free1X2.Shared`.
5. **Calidad de presentación 2026:** Fluent nativo con Mica, tema Light/Dark conmutable por SO, paleta slate/indigo aplicada vía ThemeResources, accesibilidad AA verificada (contraste, foco/teclado, `AutomationProperties`, estados no-solo-color en el semáforo).
6. **Performance dentro de presupuesto:** cold start ≤2,5 s, warm ≤1,2 s, memoria en reposo ≤180 MB; grids grandes con working set plano al hacer scroll (virtualización funcional); 60 fps en navegación y scroll. Gate de performance en CI en verde.
7. **Red de pruebas completa:** suite de dominio + ViewModel + smoke de arranque + regresión visual de tokens en verde; 3–5 E2E de humo de los flujos críticos pasando.
8. **Empaquetado entregable:** build self-contained unpackaged `win-x64` con R2R, producido por CI, validado (compila/corre/Fluent nativo) y distribuible como ".exe suelto" a usuarios no técnicos.


---


I have solid real data. Writing the plan.

## 1. Plan de Proyecto (PM)

### 1.1 Inventario real de la UI

Conteo verificado con Grep/Glob sobre `Free1X2/UI/**` (excluyendo `*.Designer.cs`):

- **65** clases que heredan de `Form` (`: Form`)
- **44** clases que heredan de `UserControl` (`: UserControl`)
- **109** superficies de UI totales a migrar

Reparto por carpeta física:
- `Free1X2/UI/*.cs` (raíz): **86** archivos (mezcla de forms, controles base de boleto, helpers e impresión)
- `Free1X2/UI/Filtros/*.cs`: **21** forms de filtros
- `Free1X2/UI/Estadisticas/*.cs`: **6** (5 forms + `statistics.cs`)
- `Free1X2/UI/Controls/Analisis/*.cs`: **19** UserControls de análisis
- `Free1X2/UI/Controls/*.cs` (raíz): **35** controles (incluye boleto + barra de iconos + figuras)
- `Free1X2/UI/Modern/**`: capa de reskin runtime (NO se migra; se descarta en WinUI)

**Hallazgo clave**: ya existen tres proyectos en la solución `Free1X2.sln`:
- `Free1X2.WinUI/Free1X2.WinUI.csproj` — **ya creado y validado** (Windows App SDK `1.6.250108002`, `net8.0-windows10.0.19041.0`, `CommunityToolkit.Mvvm 8.4.0`, self-contained `win-x64`). Hoy solo contiene `App.xaml`, `App.xaml.cs` y `Themes/Tokens.xaml`. El andamiaje base está hecho.
- `Free1X2.Shared/` — 12 archivos con servicios e interfaces ya extraídos (`IAnalysisService`, `IFilterService`, `IStatisticsService`, `ITeamService`, modelos `Match/Team`, `AppConfiguration`). **No tiene dependencias de WinForms ni System.Drawing** (verificado: 0 referencias).
- `Free1X2.WebAPI/` — backend opcional que ya referencia `Free1X2.Shared`.

**Agrupación funcional por dominio** (basada en nombres reales de archivo):

| Dominio | Superficies representativas | Vol. aprox. |
|---|---|---|
| **Shell / entrada principal** | `MainForm` (294 `ToolStripMenuItem` en el Designer → menú gigante), `AcercaDeFrm`, `CreditosFrm`, `SalirFrm`, `ConfiguracionFrm`, `ConfiguracionAnalisisFrm` | ~6 |
| **Entrada de boleto** | `BoletoFrm`, `ControlBoleto`, `ControlColumnaBoleto`, `ControlApuestaBoleto`, `PartidoBoleto`, `EquipoBoleto`, `SignoBoletoBase`, `AgregaP15Frm`, `AgregarEquipoFrm`, `GestorEquiposFrm` | ~12 |
| **Filtros** | 21 forms en `Filtros/` (`ContactosFrm`, `DistanciasFrm`, `DiferenciasFrm`, `SimetriasFrm`, `FormatosFrm`, `Formatos123Frm`, `ValoracionFrm`, `PesosNumFrm`, `InterrupcionesFrm`, `SignosSeguidosFrm`, `ControlGruposFrm`, `CrearGruposFrm`, `DibujosFrm`, `FigurasFiltrosFrm`, etc.) + `CombinarFiltros`, `DiFiltros`, `FiltroPim`, `FiltroPorcenJB`, `CtrSemaforo` | ~28 |
| **Operaciones de columnas / reducción** | `CalculaColumnas`, `CalculaColumnasMultipleFrm`, `AlgebraColumnasFrm`, `FraccionadorFrm`, `FrmReducidasPerfectas`, `FrmDependenciaLineal`, `ColProbablesFrm`, `CalculoFormatosFrm`, `DifCols`, `Compresor` | ~12 |
| **Análisis** | 19 UserControls en `Controls/Analisis/` (`CtrlAnalisisContactos`, `CtrlAnalisisDistancias`, `CtrlAnalisisDiferencias`, `CtrlAnalisisSimetrias`, `CtrlAnalisisVX2`, `CtrlAnalisisPesos`, etc.) + `AnaCombi`, `AnalizarCombinacionFrm`, `AnalizarFicheroFrm`, `AnalizadorJPMFrm`, `AnalisisFormatos123Frm`, `VisorAnalisisColumnasFrm`, `VisorAnalisisColumnasAbdonFrm` | ~26 |
| **Estadísticas** | `VisorEstadisticas`, `StaInterFrm`, `StaSSForm`, `DibForm`, `DibRepFrm` (`statistics.cs` es lógica de soporte) | ~5 |
| **Escrutinio** | `EscrutiniosFrm`, `EscrutarCombinacionesFrm`, `ColGanadoraFrm`, `ColumnasPremiadasFrm`, `EstimadorPremiosFrm`, `Coincidencias` | ~6 |
| **Boletos imprimibles / E-S** | `imprimirBoleto`, `VerBoletos`, `VerBoletosEnEditorFrm`, `DescargaBoletoFrm`, `ListaImpresoras`, `ImportadorCPsFrm`, `ExportadorCPsFrm`, `ConfigurarCPs` | ~8 |
| **Banco de pruebas / tramos** | `BancoPruebasFrm`, `DialogoGrabarBancoPruebasFrm`, `DialogoSeleccionBancoPruebasFrm`, `DialogoGrabarTramosFrm`, `DialogoAnalisisMultipleDeTramosFrm`, `DialogoFiltrarPorLimitesFrm` | ~6 |
| **Diálogos auxiliares / utilidades** | `AyudaFrm`, `BuscaLimsFrm`, `ListadoCondicionesFrm`, `EstucolFrm`, `Aidomnou`, controles base (`NumTextBox`, `Vertical_Label`, `BotonValorNum`, `CtrlFiguras`, `CtrlFormato123`, barra de iconos) | ~15 |

### 1.2 Estrategia de migración recomendada: **incremental (strangler fig), NO big-bang**

Justificación con datos del repo:

1. **Volumen y riesgo**: 109 superficies, una de ellas (`MainForm`) con un menú de ~294 entradas que es el árbol de navegación de toda la app. Un big-bang reescribiría todo a la vez sin poder validar valor incremental → riesgo de regresión enorme e imposible de probar por partes.
2. **La lógica de dominio NO está separada del proyecto WinForms** (vive en `Free1X2/MotorCalculo/`, `Reduccion/`, `Escrutinio/`, `EntradaSalida/`). Eso obliga primero a extraer/abstraer dominio antes de poder consumirlo desde WinUI. Esto es trabajo de plomería que solo el patrón strangler permite hacer de forma segura y gradual.
3. **Acoplamiento UI→dominio es real pero superficial**: solo 15 archivos de dominio referencian `System.Windows.Forms`, y el uso es `Application.DoEvents()` (20 ocurrencias en `MotorCalculo`/`Reduccion`/`Escrutinio`), `Application.StartupPath` (en `AConfiguracion.cs`, `Analizador.cs`) y `MessageBox`. **System.Drawing = 0 referencias en dominio**. Es eliminable con abstracciones (`IProgressReporter`, `IAppPaths`, `IUserDialogs`), no requiere reescritura.
4. **El andamiaje WinUI ya existe y está validado** (compila/corre/renderiza Fluent). Empezar incremental capitaliza ese trabajo sin botarlo.

**Cómo coexisten WinForms y WinUI durante la transición:**

- Durante toda la migración, **WinForms (`Free1X2.csproj`) sigue siendo la app shippable**. Es el "host vivo" que se va vaciando.
- WinUI 3 **no soporta hospedar WinForms dentro de su ventana de forma robusta** (XAML Islands al revés no es viable). Por tanto el host de coexistencia es a la inversa: durante las primeras olas, **WinForms es el shell** y se puede hospedar contenido WinUI vía `DesktopWindowXamlSource` (XAML Islands) SOLO si se necesita probar piezas WinUI temprano. En la práctica recomiendo evitar islas y, en su lugar:
  - **Lanzar la nueva ventana WinUI como ventana hermana** desde un menú de MainForm ("Probar nueva UI"), ambos procesos/ventanas conviviendo, compartiendo `Free1X2.Shared` + dominio extraído. El usuario alterna mientras se completan olas.
  - Una vez la cobertura WinUI alcanza el flujo crítico (boleto + filtros + análisis), se **invierte el arranque**: `Program.cs` lanza la `MainWindow` WinUI y deja MainForm WinForms accesible solo como fallback temporal para forms aún no migrados.
- **Contrato compartido**: todo lo que se migra consume exclusivamente `Free1X2.Shared` + un nuevo `Free1X2.Domain` (ver Fase 0). Cero código WinForms en los ViewModels.

### 1.3 Fases (entregables, secuencia, criterios de hecho)

**Fase 0 — Descoplamiento de dominio (fundación)**
- Entregables: nuevo proyecto `Free1X2.Domain` (`net8.0`, sin `UseWindowsForms`) que absorbe `MotorCalculo`, `Reduccion`, `Escrutinio`, `EntradaSalida`. Reemplazar `Application.DoEvents` → `IProgressReporter`/async-await; `Application.StartupPath` → `IAppPaths`; `MessageBox` → `IUserDialogs`. Mover modelos/servicios duplicados a `Free1X2.Shared`.
- DoD: `Free1X2.Domain` compila sin referencia a `System.Windows.Forms`; los tests existentes/nuevos de cálculo pasan; WinForms sigue corriendo consumiendo `Free1X2.Domain`.

**Fase 1 — Shell WinUI + MVVM base**
- Entregables: `MainWindow` con `NavigationView` que refleja la taxonomía del menú de 294 ítems agrupada en las ~10 categorías del inventario; sistema de temas Fluent en `Themes/Tokens.xaml` mapeando la paleta slate/indigo existente (Background `#F5F7FA`, Surface `#FFFFFF`, Primary `#4F46E5`, Text `#0F172A`, Success `#16A34A`, Warning `#D97706`, Error `#DC2626`); infraestructura DI + `INavigationService` + `ViewModelLocator` (CommunityToolkit.Mvvm); equivalente WinUI del `CtrSemaforo` (`InfoBadge`/`InfoBar` con estados) y botones de filtro con estado.
- DoD: shell arranca, navega entre páginas placeholder, tema Fluent nativo correcto en claro/oscuro.

**Fase 2 — Flujo núcleo: Entrada de boleto + Filtros**
- Entregables: página de boleto (`BoletoFrm` + 6 controles de boleto) en WinUI/MVVM; los 21 filtros + semáforo + combinadores migrados.
- DoD: un usuario puede cargar/editar un boleto de 14 partidos, aplicar/combinar filtros y ver el conteo de columnas, todo en WinUI consumiendo `Free1X2.Domain`. Paridad funcional con WinForms verificada caso a caso.

**Fase 3 — Operaciones de columnas + Análisis**
- Entregables: ~12 forms de columnas/reducción + 19 UserControls de análisis + 7 forms de análisis migrados. Reemplazar `DataGridView` por `DataGrid` (CommunityToolkit) o `ListView` virtualizado.
- DoD: generación/reducción de columnas y todos los análisis producen resultados idénticos a WinForms.

**Fase 4 — Escrutinio + Estadísticas + Boletos imprimibles**
- Entregables: 6 forms de escrutinio, 5 de estadísticas (sustituir `System.Windows.Forms.DataVisualization` por gráficos WinUI/`LiveCharts`/`Win2D`), 8 de impresión/E-S de boletos.
- DoD: escrutinio y premios correctos; impresión de boleto funcional vía `PrintManager`/PDF; gráficas estadísticas renderizadas.

**Fase 5 — Auxiliares, banco de pruebas, pulido y corte**
- Entregables: banco de pruebas/tramos (6), diálogos auxiliares y utilidades (~15), `ConfiguracionFrm`, ayuda, acerca-de. Inversión del arranque a WinUI como shell principal; retirada del proyecto WinForms y de `UI/Modern/**`.
- DoD: 0 superficies pendientes en WinForms; `Free1X2.csproj` y `UI/Modern` eliminados; distribución self-contained empaquetada y validada.

### 1.4 Work Breakdown — olas de migración

Priorización por **valor/uso × riesgo técnico** (lo más usado y menos riesgoso primero; lo más complejo cuando el patrón ya está maduro):

- **Ola 0 (fundación)** — extracción de dominio. *Por qué*: bloquea todo lo demás; sin esto no hay nada que consumir desde WinUI.
- **Ola 1 (shell)** — `MainWindow`/NavigationView, temas, DI, semáforo. *Por qué*: marco de navegación y patrón de referencia para el resto.
- **Ola 2 (boleto)** — `BoletoFrm` + 6 controles + alta de equipos/P15. *Por qué*: es la entrada de datos sin la cual nada funciona; máximo uso; valida el patrón de control compuesto custom (lo más difícil de UI) temprano para reducir incertidumbre.
- **Ola 3 (filtros)** — 21 filtros + combinadores + semáforo. *Por qué*: corazón de la propuesta de valor (reducir columnas), alto uso, forms relativamente homogéneos → buena economía de escala.
- **Ola 4 (columnas/reducción)** — cálculo/álgebra/reducidas/compresor. *Por qué*: depende de boleto+filtros ya migrados; riesgo medio.
- **Ola 5 (análisis)** — 19 UserControls + 7 forms. *Por qué*: gran volumen homogéneo; se hace cuando el patrón de UserControl→UserControl-WinUI ya está consolidado.
- **Ola 6 (escrutinio + estadísticas + impresión)** — *Por qué*: dependen de columnas; estadísticas e impresión tienen riesgo técnico alto (gráficas + `PrintManager`) → se atacan con el equipo ya experto.
- **Ola 7 (auxiliares + banco de pruebas + corte)** — todo lo restante de menor uso + inversión de arranque + limpieza.

### 1.5 Registro de riesgos

| # | Riesgo | Tipo | Impacto | Mitigación |
|---|---|---|---|---|
| R1 | Lógica de dominio acoplada al proyecto WinForms (15 archivos con `System.Windows.Forms`, 20 `Application.DoEvents`, `Application.StartupPath`, `MessageBox`) | Técnico | Alto | Fase 0 dedicada: `Free1X2.Domain` + abstracciones `IProgressReporter`/`IAppPaths`/`IUserDialogs`; convertir bucles con `DoEvents` a `async`/`Task` con reporte de progreso |
| R2 | Controles WinForms sin equivalente directo: `DataGridView` (en CtrlDataGridView*), `System.Windows.Forms.DataVisualization` (gráficas), `PrintDocument` (impresión de boletos) | Técnico | Alto | DataGrid de CommunityToolkit / ListView virtualizado; gráficas → Win2D o LiveCharts2; impresión → `PrintManager`/render a PDF. Hacer spikes en Ola 1 |
| R3 | Controles compuestos custom de boleto (`ControlBoleto`, `PartidoBoleto`, `SignoBoletoBase`, eventos `PronosticoChanged`) con dibujo y lógica propia | Técnico | Alto | Rediseñar como `UserControl` XAML + `ItemsControl`/`DataTemplate` con binding; abordar temprano (Ola 2) para descubrir incógnitas |
| R4 | `MainForm` con ~294 ítems de menú → taxonomía de navegación no trivial | Proyecto/UX | Medio | Mapear el menú a las ~10 categorías del inventario en Ola 1; validar IA de navegación con el usuario antes de Ola 2 |
| R5 | Paridad funcional difícil de verificar en 109 superficies | Proyecto | Alto | Tests de regresión sobre `Free1X2.Domain` (cálculo determinista); checklist de paridad por form; mantener WinForms vivo para comparación lado a lado |
| R6 | WinUI 3 no hospeda WinForms; coexistencia limitada | Técnico | Medio | Coexistencia por ventanas hermanas compartiendo dominio, no por islas; invertir arranque solo cuando el flujo crítico esté migrado |
| R7 | Distribución/empaquetado self-contained de Windows App SDK | Técnico | Bajo | Ya validado en este entorno (compila/corre/Fluent nativo); fijar versión `1.6.250108002`; CI que produzca el bundle cada ola |
| R8 | Duplicación entre `Free1X2.Shared` y dominio aún embebido | Proyecto | Medio | En Fase 0 consolidar modelos/servicios; `Free1X2.Shared` como única fuente de contratos |
| R9 | Pérdida de funcionalidad de nicho poco usada en forms auxiliares | Proyecto | Bajo | Inventario de uso real; auxiliares al final (Ola 7); decidir explícitamente qué se deprecia |
| R10 | Recursos/iconos legacy (`UI/Modern/Icons`, Resources) no reutilizables | Técnico | Bajo | Adoptar Segoe Fluent Icons nativos de WinUI; descartar `UI/Modern/**` |

### 1.6 Estimación de esfuerzo (semanas-persona)

**Supuestos explícitos:**
- 1 dev senior con experiencia previa en WinUI 3/MVVM (curva de aprendizaje absorbida en Ola 0–1).
- Andamiaje WinUI ya existe y está validado (no se estima creación de proyecto).
- "No cambiar lógica de negocio": el dominio se reusa; se estima solo abstracción/migración, no reescritura algorítmica.
- Forms homogéneos (filtros, análisis) se benefician de plantillas/economía de escala una vez establecido el patrón.
- Incluye paridad funcional y pruebas básicas; excluye QA formal externo y manual de usuario.

| Fase / Ola | Alcance | Rango (sem-persona) |
|---|---|---|
| Fase 0 — Descoplamiento dominio | `Free1X2.Domain`, abstracciones, async progress, tests | 3 – 5 |
| Fase 1 — Shell + MVVM + temas | NavigationView, DI, tokens Fluent, semáforo, spikes R2/R3 | 3 – 4 |
| Fase 2 — Boleto (12 superficies, R3) | Control compuesto + alta equipos | 3 – 5 |
| Fase 3 — Filtros (28) | 21 filtros + combinadores + semáforo | 5 – 7 |
| Fase 4 — Columnas/Reducción (12) | cálculo, álgebra, reducidas, compresor | 3 – 4 |
| Fase 5 — Análisis (26) | 19 UserControls + 7 forms (homogéneo) | 5 – 7 |
| Fase 6 — Escrutinio+Estadísticas+Impresión (19, R2) | gráficas + PrintManager (riesgo alto) | 5 – 7 |
| Fase 7 — Auxiliares+Banco+corte (21) | utilidades, banco de pruebas, inversión arranque, limpieza | 4 – 6 |
| **Total** | **109 superficies** | **31 – 45 sem-persona** |

Con 1 dev: ~7–11 meses. Con 2 devs en paralelo a partir de la Fase 2 (olas de forms homogéneos son paralelizables): **~4–6 meses** de calendario. Recomendación: 2 devs tras consolidar el patrón en Ola 1–2.

Archivos clave referenciados (rutas absolutas):
- `C:\Users\jr_te\Free1x2\Free1X2.WinUI\Free1X2.WinUI.csproj` (andamiaje WinUI existente)
- `C:\Users\jr_te\Free1x2\Free1X2.WinUI\Themes\Tokens.xaml`
- `C:\Users\jr_te\Free1x2\Free1X2.Shared\Services\` (contratos ya extraídos)
- `C:\Users\jr_te\Free1x2\Free1X2\UI\MainForm.Designer.cs` (menú de 294 ítems → IA de navegación)
- `C:\Users\jr_te\Free1x2\Free1X2\MotorCalculo\Analizador.cs`, `Free1X2\EntradaSalida\AConfiguracion.cs` (foco de descoplamiento Fase 0)


---


I have everything I need. Writing the section.

## 2. Sistema de Diseño Fluent

El sistema de diseño WinUI 3 reemplaza el reskin en runtime de `Free1X2/UI/Modern/Theming/ModernTheme.cs` (clases `Colors`, `Fonts`, `Sizes`) por **ResourceDictionaries declarativos** con soporte nativo Light/Dark, override del `AccentColor` del sistema y los efectos de material Fluent (Mica/Acrylic). La paleta objetivo de CLAUDE.md (slate/indigo `#4F46E5`) sustituye al azul Windows-11 `#0078D4` que aún tiene hardcodeado el `ModernTheme.cs` actual (línea 20: `Color.FromArgb(0, 120, 212)`).

### 2.1 Paleta → ResourceDictionary con ThemeResources

Crear `Free1X2.WinUI/Themes/Colors.xaml`. La estructura clave es un `ResourceDictionary.ThemeDictionaries` con tres llaves (`Light`, `Dark`, `HighContrast`) para que WinUI conmute automáticamente con el tema del SO. Los `Color` van en `<ResourceDictionary x:Key="...">` y los `SolidColorBrush` los consumen.

```xml
<!-- Free1X2.WinUI/Themes/Colors.xaml -->
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <ResourceDictionary.ThemeDictionaries>

        <!-- ================= LIGHT ================= -->
        <ResourceDictionary x:Key="Light">
            <!-- Slate / Indigo (mapeo desde ModernTheme.Colors) -->
            <Color x:Key="AppBackgroundColor">#F5F7FA</Color>   <!-- Colors.Background -->
            <Color x:Key="AppSurfaceColor">#FFFFFF</Color>      <!-- Colors.Surface -->
            <Color x:Key="AppSurfaceAltColor">#EEF2F6</Color>   <!-- Colors.SurfaceAlt / grid alt -->
            <Color x:Key="AppPrimaryColor">#4F46E5</Color>      <!-- Colors.Primary (indigo-600) -->
            <Color x:Key="AppPrimaryHoverColor">#4338CA</Color>
            <Color x:Key="AppPrimaryPressColor">#3730A3</Color>
            <Color x:Key="AppTextColor">#0F172A</Color>         <!-- Colors.Text (slate-900) -->
            <Color x:Key="AppTextSecondaryColor">#475569</Color>
            <Color x:Key="AppBorderColor">#CBD5E1</Color>       <!-- Colors.Border (slate-300) -->
            <Color x:Key="AppSuccessColor">#16A34A</Color>      <!-- Colors.Success -->
            <Color x:Key="AppWarningColor">#D97706</Color>      <!-- Colors.Warning -->
            <Color x:Key="AppErrorColor">#DC2626</Color>        <!-- Colors.Error -->

            <SolidColorBrush x:Key="AppBackgroundBrush"  Color="{StaticResource AppBackgroundColor}" />
            <SolidColorBrush x:Key="AppSurfaceBrush"     Color="{StaticResource AppSurfaceColor}" />
            <SolidColorBrush x:Key="AppSurfaceAltBrush"  Color="{StaticResource AppSurfaceAltColor}" />
            <SolidColorBrush x:Key="AppPrimaryBrush"     Color="{StaticResource AppPrimaryColor}" />
            <SolidColorBrush x:Key="AppTextBrush"        Color="{StaticResource AppTextColor}" />
            <SolidColorBrush x:Key="AppTextSecondaryBrush" Color="{StaticResource AppTextSecondaryColor}" />
            <SolidColorBrush x:Key="AppBorderBrush"      Color="{StaticResource AppBorderColor}" />
            <SolidColorBrush x:Key="AppSuccessBrush"     Color="{StaticResource AppSuccessColor}" />
            <SolidColorBrush x:Key="AppWarningBrush"     Color="{StaticResource AppWarningColor}" />
            <SolidColorBrush x:Key="AppErrorBrush"       Color="{StaticResource AppErrorColor}" />
        </ResourceDictionary>

        <!-- ================= DARK ================= -->
        <ResourceDictionary x:Key="Dark">
            <Color x:Key="AppBackgroundColor">#0F172A</Color>   <!-- slate-900 como fondo -->
            <Color x:Key="AppSurfaceColor">#1E293B</Color>      <!-- slate-800 -->
            <Color x:Key="AppSurfaceAltColor">#334155</Color>   <!-- slate-700 -->
            <Color x:Key="AppPrimaryColor">#818CF8</Color>      <!-- indigo-400, más luminoso en dark -->
            <Color x:Key="AppPrimaryHoverColor">#A5B4FC</Color>
            <Color x:Key="AppPrimaryPressColor">#6366F1</Color>
            <Color x:Key="AppTextColor">#F1F5F9</Color>         <!-- slate-100 -->
            <Color x:Key="AppTextSecondaryColor">#94A3B8</Color>
            <Color x:Key="AppBorderColor">#475569</Color>
            <Color x:Key="AppSuccessColor">#22C55E</Color>
            <Color x:Key="AppWarningColor">#F59E0B</Color>
            <Color x:Key="AppErrorColor">#EF4444</Color>

            <SolidColorBrush x:Key="AppBackgroundBrush"  Color="{StaticResource AppBackgroundColor}" />
            <SolidColorBrush x:Key="AppSurfaceBrush"     Color="{StaticResource AppSurfaceColor}" />
            <SolidColorBrush x:Key="AppSurfaceAltBrush"  Color="{StaticResource AppSurfaceAltColor}" />
            <SolidColorBrush x:Key="AppPrimaryBrush"     Color="{StaticResource AppPrimaryColor}" />
            <SolidColorBrush x:Key="AppTextBrush"        Color="{StaticResource AppTextColor}" />
            <SolidColorBrush x:Key="AppTextSecondaryBrush" Color="{StaticResource AppTextSecondaryColor}" />
            <SolidColorBrush x:Key="AppBorderBrush"      Color="{StaticResource AppBorderColor}" />
            <SolidColorBrush x:Key="AppSuccessBrush"     Color="{StaticResource AppSuccessColor}" />
            <SolidColorBrush x:Key="AppWarningBrush"     Color="{StaticResource AppWarningColor}" />
            <SolidColorBrush x:Key="AppErrorBrush"       Color="{StaticResource AppErrorColor}" />
        </ResourceDictionary>

    </ResourceDictionary.ThemeDictionaries>

    <!-- Tokens neutros (no dependen del tema): radios, espaciado, fuentes -->
</ResourceDictionary>
```

### 2.2 Override del AccentColor del sistema con indigo

WinUI deriva docenas de brushes (`AccentButtonBackground`, `NavigationViewSelectionIndicatorForeground`, `ToggleSwitchOnColor`, selección de `ListView`, etc.) de la familia `SystemAccentColor` + sus variantes `Light1/2/3` y `Dark1/2/3`. Para forzar el indigo `#4F46E5` de marca en lugar del acento del usuario, **sobrescribir esas llaves en `App.Resources`** (en `App.xaml`, fuera de `ThemeDictionaries` porque son los mismos hex en ambos temas, salvo que se quiera afinar):

```xml
<!-- App.xaml -->
<Application
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <XamlControlsResources xmlns="using:Microsoft.UI.Xaml.Controls" />
                <ResourceDictionary Source="ms-appx:///Themes/Colors.xaml" />
                <ResourceDictionary Source="ms-appx:///Themes/Typography.xaml" />
                <ResourceDictionary Source="ms-appx:///Themes/Sizes.xaml" />
            </ResourceDictionary.MergedDictionaries>

            <!-- Marca: indigo como acento global (sustituye SystemAccentColor del usuario) -->
            <Color x:Key="SystemAccentColor">#4F46E5</Color>
            <Color x:Key="SystemAccentColorLight1">#6366F1</Color>
            <Color x:Key="SystemAccentColorLight2">#818CF8</Color>
            <Color x:Key="SystemAccentColorLight3">#A5B4FC</Color>
            <Color x:Key="SystemAccentColorDark1">#4338CA</Color>
            <Color x:Key="SystemAccentColorDark2">#3730A3</Color>
            <Color x:Key="SystemAccentColorDark3">#312E81</Color>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Nota: para que el acento se aplique también a la barra de título/Mica del SO, además se setea `Application.Current.Resources` en `App.OnLaunched` o se respeta el del usuario; recomendación abajo en §2.7.

### 2.3 Tipografía → estilos Fluent + Segoe UI Variable

WinUI 3 usa **Segoe UI Variable** por defecto (`ContentControlThemeFontFamily`), que es la evolución de la "Segoe UI" que `ModernTheme.Fonts` instanciaba a mano (líneas 51–66). No hay que cargar la fuente: ya viene en Win10 19041+ (nuestro TFM `net8.0-windows10.0.19041.0`). El mapeo de los tokens de fuente actuales a estilos Fluent integrados:

| `ModernTheme.Fonts.*` (pt) | Estilo Fluent WinUI | Tamaño Fluent |
|---|---|---|
| `Title` (14pt bold) → líneas 64 | `TitleTextBlockStyle` | 28px SemiBold |
| `Header` (11pt bold) → 63 | `SubtitleTextBlockStyle` | 20px SemiBold |
| `Bold` (9pt bold) → 62 | `BodyStrongTextBlockStyle` | 14px SemiBold |
| `Large` (10pt) → 61 | `BodyTextBlockStyle` | 14px |
| `Default` (9pt) → 59 | `BodyTextBlockStyle` (default control) | 14px |
| `Small`/`Caption` (8pt) → 60/65 | `CaptionTextBlockStyle` | 12px |

`Free1X2.WinUI/Themes/Typography.xaml` solo necesita un alias y, si se quiere, fijar la familia para datos numéricos tabulares (columnas 1/X/2, conteos de boleto):

```xml
<!-- Free1X2.WinUI/Themes/Typography.xaml -->
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Familia base = Segoe UI Variable (default de WinUI; explícito por claridad) -->
    <FontFamily x:Key="AppFontFamily">Segoe UI Variable Text</FontFamily>
    <FontFamily x:Key="AppDisplayFontFamily">Segoe UI Variable Display</FontFamily>

    <!-- Mono tabular para columnas de quiniela / contadores (Fijos-Dobles-Triples) -->
    <FontFamily x:Key="AppNumericFontFamily">Cascadia Mono, Consolas</FontFamily>

    <!-- Estilo para celdas de signo 1/X/2 en grids de columnas -->
    <Style x:Key="SignoCellStyle" TargetType="TextBlock" BasedOn="{StaticResource BodyStrongTextBlockStyle}">
        <Setter Property="FontFamily" Value="{StaticResource AppNumericFontFamily}" />
        <Setter Property="TextAlignment" Value="Center" />
        <Setter Property="HorizontalAlignment" Value="Stretch" />
    </Style>
</ResourceDictionary>
```

Uso: `<TextBlock Text="Análisis de columnas" Style="{StaticResource TitleTextBlockStyle}" />` — los estilos `*TextBlockStyle` ya están en `XamlControlsResources`.

### 2.4 Tokens de espaciado / radio / elevación → `Sizes.xaml`

Mapeo 1:1 de `ModernTheme.Sizes` (líneas 72–88). En WinForms los gaps eran `int`; en WinUI se expresan como `x:Double`/`Thickness`/`CornerRadius`. WinUI 11 usa radio 4 en controles y 8 en cards — coincide con el `BorderRadius=4` actual (línea 84).

```xml
<!-- Free1X2.WinUI/Themes/Sizes.xaml -->
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Espaciado (= Sizes.Spacing*) -->
    <x:Double x:Key="SpacingXSmall">4</x:Double>   <!-- SpacingXSmall = 4 -->
    <x:Double x:Key="SpacingSmall">8</x:Double>    <!-- SpacingSmall  = 8 -->
    <x:Double x:Key="SpacingMedium">12</x:Double>  <!-- SpacingMedium = 12 -->
    <x:Double x:Key="SpacingLarge">16</x:Double>   <!-- SpacingLarge  = 16 -->
    <x:Double x:Key="SpacingXLarge">24</x:Double>  <!-- SpacingXLarge = 24 -->

    <Thickness x:Key="PageMargin">24,16,24,16</Thickness>
    <Thickness x:Key="CardPadding">16</Thickness>
    <Thickness x:Key="GroupBoxPadding">12</Thickness>

    <!-- Radios -->
    <CornerRadius x:Key="ControlRadius">4</CornerRadius>   <!-- Sizes.BorderRadius = 4 -->
    <CornerRadius x:Key="CardRadius">8</CornerRadius>

    <!-- Alturas (= Sizes.*Height) -->
    <x:Double x:Key="ControlHeight">32</x:Double>   <!-- ControlHeight=28 → 32 estándar WinUI -->
    <x:Double x:Key="GridRowHeight">32</x:Double>   <!-- GridRowHeight=24 → 32 (touch target) -->

    <!-- Elevación: usar los ThemeShadow integrados; offset para cards -->
    <x:Double x:Key="CardShadowDepth">8</x:Double>
</ResourceDictionary>
```

**Elevación**: WinForms no tenía sombras; en WinUI usar `ThemeShadow` (o el `CardBackgroundFillColorDefaultBrush` + borde para cards "planas"). Para los paneles tipo card de los filtros, envolver en `<Border Background="{ThemeResource CardBackgroundFillColorDefaultBrush}" CornerRadius="{StaticResource CardRadius}" BorderBrush="{ThemeResource CardStrokeColorDefaultBrush}" BorderThickness="1">`.

### 2.5 Mapa de componentes WinForms → WinUI

Inventario real: `MainForm.cs`, 22 forms de filtro en `Free1X2/UI/Filtros/`, 7 de estadística en `Free1X2/UI/Estadisticas/`, 19 controles en `Free1X2/UI/Controls/Analisis/`, y diálogos sueltos en `Free1X2/UI/`.

| WinForms (origen real) | WinUI 3 | Notas de migración |
|---|---|---|
| `Button` (flat, `StyleButton`) | `Button` con `DefaultButtonStyle` | Acción primaria → `AccentButtonStyle`. |
| Botón de filtro con `SetBotonEstado` (`BotonEstado.Activo/Error/Neutro/Inactivo`, `MainForm.cs:729`) | `ToggleButton` + `VisualStateManager` o `Style` por estado | Ver §2.6: estilos `FilterButtonActivo/Error/Neutro`. |
| `DataGridView` (`StyleDataGrid`, usado en `CtrlDataGridViewCPs`, `CtrlDataGridViewFiguras`, columnas premiadas) | **`CommunityToolkit.WinUI.UI.Controls.DataGrid`** | Recomendado para grids editables/ordenables con columnas tipadas (columnas de quiniela, históricos). Es el reemplazo directo con `Columns`, `AlternatingRowBackground`, selección de fila. |
| Grids de solo-lectura con muchas filas (visores de análisis) | `ItemsRepeater` dentro de `ScrollViewer`, o `ListView` virtualizado | Si la rejilla es solo lectura y de alto volumen (miles de columnas reducidas), `ItemsRepeater` rinde mejor que el DataGrid del Toolkit. |
| `TabControl` (`StyleTabControl`, p.ej. `BancoPruebasFrm`) | **Navegación de nivel app → `NavigationView`; tabs dentro de página → `Pivot` o `TabView`** | Las secciones grandes (Filtros, Estadísticas, Boleto) van como ítems del `NavigationView` del shell. Sub-pestañas dentro de un módulo → `Pivot`. Pestañas cerrables/dinámicas → `TabView`. |
| `MenuStrip` (`StyleMenuStrip` + `NeoColorTable`) | `MenuBar` + `MenuBarItem`/`MenuFlyout` | El acelerador y submenús se mapean directo. |
| `StatusStrip` (índigo, `StyleStatusStrip`, `MainForm`) | `Grid` custom anclado abajo + `InfoBar` para mensajes transitorios | No hay StatusStrip nativo. Barra fija con `AppPrimaryBrush` para estado permanente (versión, conteos); `InfoBar` (Severity Informational/Success/Error) para avisos. |
| `GroupBox` (`StyleGroupBox`) | `Border` con header (caja estática) **o `Expander`** (colapsable) | Los paneles de opciones de filtro densos ganan con `Expander`. |
| `NumericUpDown` (`StyleNumericUpDown`, abundante en filtros: pesos, tolerancias, distancias) | **`NumberBox`** | `SpinButtonPlacementMode="Inline"`, `Minimum/Maximum/SmallChange`, `ValidationMode="InvalidInputOverwritten"`. Mapeo directo. |
| `ComboBox` (`StyleComboBox`) | `ComboBox` | Para listas editables grandes → `AutoSuggestBox`. |
| `CheckBox` / `RadioButton` | `CheckBox` / `RadioButton` (o `RadioButtons` agrupado) | Directo. |
| `ListView` / `ListBox` | `ListView` | Directo, con virtualización. |
| `RichTextBox` (ayuda/resultados) | `RichTextBlock` (solo lectura) o `RichEditBox` (editable) | La mayoría son solo-lectura → `RichTextBlock`. |
| `SplitContainer` (`StyleSplitContainer`) | `Grid` con `GridSplitter` (CommunityToolkit) | El Toolkit aporta `GridSplitter`. |
| `Panel` | `Grid` / `StackPanel` / `Border` | Según layout. |
| `ToolStrip`/`ToolBarManager` (`Controls/barraIconos/`) | `CommandBar` + `AppBarButton` | Iconos Fluent vía `FontIcon`/`SymbolIcon` (reemplaza `SegoeIcons.cs`). |
| `CtrSemaforo` (`Controls/CtrSemaforo.cs`) | UserControl WinUI nuevo, ver §2.6 | 3 `Ellipse` enlazadas a un enum. |

### 2.6 Botones de filtro y CtrSemaforo (propuestas concretas)

**Botones de estado** (`SetBotonEstado`, `MainForm.cs:711-729`): definir estilos en un `ResourceDictionary` y enlazar el estilo por binding/converter desde el ViewModel. Estilos:

```xml
<!-- Themes/FilterStates.xaml -->
<Style x:Key="FilterButtonActivo" TargetType="Button" BasedOn="{StaticResource DefaultButtonStyle}">
    <Setter Property="Background" Value="{ThemeResource AppSuccessBrush}" />
    <Setter Property="Foreground" Value="White" />
</Style>
<Style x:Key="FilterButtonError" TargetType="Button" BasedOn="{StaticResource DefaultButtonStyle}">
    <Setter Property="Background" Value="{ThemeResource AppErrorBrush}" />
    <Setter Property="Foreground" Value="White" />
</Style>
<Style x:Key="FilterButtonNeutro" TargetType="Button" BasedOn="{StaticResource DefaultButtonStyle}">
    <Setter Property="Background" Value="{ThemeResource AppWarningBrush}" />
    <Setter Property="Foreground" Value="White" />
</Style>
<!-- Inactivo = DefaultButtonStyle (Surface) -->
```

El enum `BotonEstado` se preserva en el ViewModel; un `BotonEstadoToStyleConverter` lo mapea a estos estilos. (Mejor que tocar `Background` por código: queda theme-aware.)

**CtrSemaforo** → reemplazar el UserControl de 3 `Button` 10×10 (`CtrSemaforo.cs:134-169`, con `FlatStyle.Popup` legacy) por un UserControl WinUI con 3 `Ellipse` y una DP `Estado` (enum `Neutro/Rojo/Amarillo/Verde`). Sin colores hardcodeados: usa los brushes de tema. Los colores "dim" (líneas 253-258) → versiones con opacidad 0.30.

```xml
<!-- Controls/Semaforo.xaml -->
<UserControl x:Class="Free1X2.WinUI.Controls.Semaforo" ...>
    <StackPanel Orientation="Horizontal" Spacing="4">
        <Ellipse x:Name="DotRojo"     Width="12" Height="12" Fill="{ThemeResource AppErrorBrush}"   Opacity="0.30"/>
        <Ellipse x:Name="DotAmarillo" Width="12" Height="12" Fill="{ThemeResource AppWarningBrush}" Opacity="0.30"/>
        <Ellipse x:Name="DotVerde"    Width="12" Height="12" Fill="{ThemeResource AppSuccessBrush}" Opacity="0.30"/>
    </StackPanel>
</UserControl>
```

El code-behind sube a `Opacity=1.0` el dot activo según `Estado` (equivalente a `CambiaColor()`), expone el evento `BotonPulsado` y mantiene la semántica de habilitado/deshabilitado por `PointerPressed`. Soporta `Orientation` (Horizontal/Vertical, sustituye `alignment`) y `NumLuces` (Dos/Tres → ocultar `DotAmarillo`).

### 2.7 Light/Dark + Mica/Acrylic — recomendación

1. **Tema**: dejar `RequestedTheme` sin fijar → la app sigue el SO automáticamente vía los `ThemeDictionaries` de §2.1. Añadir un toggle en Ajustes (`ElementTheme.Default/Light/Dark` sobre `FrameworkElement.RequestedTheme` del root) persistido en `ApplicationData.LocalSettings`.

2. **Material de fondo**: **Mica** en la ventana principal (shell con `NavigationView`). Es el material recomendado por Microsoft para la superficie base de apps de larga duración y es barato en GPU. Aplicar con `SystemBackdrop="Mica"` (Mica Base) en la `Window`, dejando el `NavigationView` y el contenido sobre fondo transparente para que el material se vea:

```xml
<!-- MainWindow.xaml -->
<Window ...>
    <Window.SystemBackdrop>
        <MicaBackdrop Kind="Base" />
    </Window.SystemBackdrop>
    <NavigationView x:Name="Nav" PaneDisplayMode="Left"
                    IsBackButtonVisible="Collapsed"
                    Background="Transparent">
        <Frame x:Name="ContentFrame" />
    </NavigationView>
</Window>
```

3. **Acrylic**: reservarlo para superficies **transitorias/flotantes** (flyouts, panes de `NavigationView` en modo overlay, diálogos). No usar Acrylic en superficies de contenido grandes con datos (los grids de columnas y los visores de análisis): el ruido reduce legibilidad de las celdas 1/X/2. Esas superficies van con `AppSurfaceBrush` sólido y borde de card (§2.4).

4. **Mica Alt**: opcional para `TabView`/títulos si se adopta `TabView` en algún módulo; para esta app, Mica Base en ventana + superficies sólidas para datos es la combinación correcta.

5. **Barra de título**: extender el contenido bajo la barra (`ExtendsContentIntoTitleBar = true`) para que Mica fluya hasta arriba; setear `AppWindow.TitleBar` botones transparentes para que respeten el material.

**Resumen de archivos nuevos a crear**: `Free1X2.WinUI/App.xaml` (merge dictionaries + override de acento), `Themes/Colors.xaml` (ThemeDictionaries Light/Dark), `Themes/Typography.xaml`, `Themes/Sizes.xaml`, `Themes/FilterStates.xaml`, `Controls/Semaforo.xaml`. Todos sustituyen funcionalmente a `Free1X2/UI/Modern/Theming/ModernTheme.cs`, `NeoToolStripRenderer`, `NeoColorTable` y `Free1X2/UI/Modern/Icons/` (este último → `FontIcon`/`SymbolIcon` nativos).


---


I have enough grounding data. The menu structure gives me: Free1X2 (config/help), Archivo (file/boletos online/equipos), Combinación (calcular/reducir/escrutar/análisis/probabilidades/estadísticas/imprimir), Grupos, Filtros (24), Estadísticas (5), Utilidades. Plus the central pane with boleto entry, Condiciones (filter buttons), Pronósticos, and CtrSemaforo per filter.

## 3. Arquitectura UX e Interacción

### 3.1 Diagnóstico de la UI actual (datos reales)

La app hoy es un único `MainForm` (`Free1X2/UI/MainForm.cs` + `MainForm.Designer.cs`) que concentra: una barra de menú (`mainMenu`), seis toolbars flotantes (`tsFree`, `tsArchivo`, `tsOperaciones`, `tsCombinacion`, `tsFiltros`, `tsUtilidades`, reposicionadas a mano en `ObtenerPosicionBarraHerramientas`), un panel central con la entrada de boleto (`ControlBoleto`), un `groupBox` "Condiciones" con ~18 botones de filtro (`btnDiferencias`, `btnSimetrias`, `btnContactos`, `btnDistancias`, `btnValoracion`, `btnPesosNum`, `btnCP`, `btnDibujos`, `btnSignosSeguidos`, `btnNoVariantes`, `btnControlGrupos`, etc.), un `groupBox2` "Pronósticos", y dos secciones de filtro (`gbFiltroGeneral`, `gbFiltroParcial`). Cada filtro abre un **diálogo modal** (24 forms en `Free1X2/UI/Filtros/`: `DiferenciasFrm`, `ContactosFrm`, `DistanciasFrm`, `ValoracionFrm`, …) y el estado de cada uno se refleja con un `CtrSemaforo` (`Free1X2/UI/Controls/CtrSemaforo.cs`, estados Neutro/Rojo/Amarillo/Verde). Hay ~166 forms/UserControls en total, la inmensa mayoría diálogos lanzados desde el menú "Combinación" (Calcular, Reducir, Escrutar, Análisis de Columnas/Fallos/Gráfico/Signos, Probabilidades, Estadísticas) y "Utilidades".

El problema UX no es estético sino **estructural**: el modelo "menú + N diálogos modales" obliga a abrir/cerrar ventanas para una tarea que es inherentemente un pipeline (boleto → condiciones → cálculo → resultados). La migración a WinUI 3 debe convertir ese árbol de menús en navegación persistente y los diálogos en superficies contextuales.

### 3.2 Modelo de navegación: `NavigationView` + flujo central

Mapeo el menú/toolbars actuales a un `NavigationView` (modo `Left`, con `PaneDisplayMode="Auto"` para colapsar a `LeftCompact`/`LeftMinimal` en ventanas estrechas). Las secciones top-level se derivan directamente de los menús reales `menuArchivo`, `menuCombinacion`, `menuGrupo`, `filtrosToolStripMenuItem`, `menuItem49 (Estadísticas)`, `utilidadesToolStripMenuItem`:

```
NavigationView
├── [Header / título de archivo en uso]  (= PonerNombrePrograma → muestra combinación activa)
│
├─ 🏠  Inicio / Boleto            ← MenuItem      (ControlBoleto + entrada 14 partidos + P15)
│       └─ sub-flujo: Abrir/Guardar Partidos Boleto, Obtener Boletos Online
│
├─ 🎯  Condiciones (Filtros)      ← MenuItem expandible  (los 24 Filtros/)
│       ├─ Diferencias        ├─ Simetrías          ├─ Formatos / Formatos 123
│       ├─ Contactos          ├─ Distancias          ├─ Interrupciones
│       ├─ Valoración         ├─ Tolerancias         ├─ Pesos Numéricos
│       ├─ Columnas Probables ├─ Dibujos             ├─ Signos Seguidos
│       ├─ Variantes X y 2    ├─ Control de Grupos   ├─ Grupos Equipos
│       └─ Condiciones Relacionadas (IfThen) · Combinar Filtros · Listado de Condiciones
│
├─ ⚙️  Cálculo                    ← MenuItem      (menuCombinacion: Calcular, Calcular Varias,
│       │                                          Reducir, Escrutar Columnas/Combinaciones)
│       └─ Banco de Pruebas (BancoPruebasFrm)
│
├─ 📊  Resultados / Análisis      ← MenuItem expandible
│       ├─ Análisis de Columnas (VisorAnalisisColumnasFrm)
│       ├─ Análisis de Fallos    ├─ Análisis Gráfico (GraficoColumnasFrm)
│       ├─ Análisis de Signos    ├─ Probabilidades (ProbabilidadPremios)
│       └─ Ver Boletos / Boleto imprimible (BoletoFrm)
│
├─ 📈  Estadísticas               ← MenuItem      (Estadisticas/: VisorEstadisticas,
│                                                  StaInterFrm, StaSSForm, DibForm, DibRepFrm)
│
├─ 👥  Grupos                     ← MenuItem      (menuGrupo: Abrir/Guardar/Copiar/Pegar/Insertar/Eliminar)
│
├─ 🛠️  Utilidades                 ← MenuItem expandible  (tsUtilidades + menú Utilidades)
│       (Álgebra de Columnas, Fraccionador, Multiplicador, Transponedor, Modificador,
│        Import/Export, Compresor, CPs: Generar/Importar/Exportar/Configurar…)
│
└── [Footer del NavigationView]
    ├─ 🏟️  Gestión de Equipos     ← FooterMenuItem  (GestorEquiposFrm)
    ├─ ⚙️  Configuración          ← SettingsItem    (ConfiguracionFrm + ConfiguracionAnalisisFrm)
    └─ ❓  Ayuda / Acerca de       ← FooterMenuItem  (AyudaFrm, AcercaDeFrm, CreditosFrm,
                                                      Comprobar Actualizaciones)
```

Decisiones clave de mapeo:
- **El menú "Combinación" actual se parte en dos secciones** ("Cálculo" y "Resultados/Análisis") porque mezcla acciones (calcular, reducir) con vistas (análisis, ver boletos). Separarlas hace explícito el pipeline.
- **Configuración** va al `SettingsItem` nativo del `NavigationView` (esquina inferior), no a un diálogo, siguiendo la convención Fluent.
- **Gestión de Equipos** y **Ayuda** van al `FooterMenuItem` por ser tareas auxiliares de baja frecuencia.
- Las seis toolbars flotantes desaparecen: sus comandos de alta frecuencia (Nueva/Abrir/Guardar combinación, Calcular, Reducir) pasan a un **`CommandBar` superior contextual por página**, no a una barra global ad-hoc.

### 3.3 Regla de decisión para los diálogos modales actuales

Tengo ~140 diálogos. No todos deben ser `ContentDialog`. Regla por número de campos y rol:

| Caso | Patrón WinUI | Ejemplos reales |
|------|-------------|-----------------|
| **Confirmación / mensaje / 1–2 inputs** (sin grid, decisión binaria) | `ContentDialog` | `SalirFrm`, `GuardarValoracionFrm`, `DialogoGrabarTramosFrm`, `ListaImpresoras`, `SalirFrm` |
| **Configuración de un filtro/condición** (varios parámetros, sin tabla grande) | **Panel deslizante** (`SplitView` pane derecho o `Flyout` anclado al ítem del filtro) sobre la página "Condiciones" | `DiferenciasFrm`, `ContactosFrm`, `DistanciasFrm`, `SimetriasFrm`, `PesosNumFrm`, `ValoracionFrm`, `SignosSeguidosFrm` |
| **Herramienta con grids grandes / múltiples controles / resultados tabulares** | **Página dedicada** (navegación, no modal) | `BancoPruebasFrm`, `VisorAnalisisColumnasFrm`, `VisorEstadisticas`, `EscrutiniosFrm`, `ReductorFrm`, `BoletoFrm`, `GraficoColumnasFrm` |
| **Wizard / proceso multipaso** | Página con `Stepper` (PivotHeaders o breadcrumbs) | `ImportExportFrm`, `GeneradorCPSDiferencias`, `FraccionadorFrm`, `TramificarForm` |

Regla en una frase: **si el usuario necesita ver el boleto/resultado mientras configura, es panel (no modal); si produce o consume tablas grandes, es página; si solo decide "sí/no/un valor", es `ContentDialog`.** Esto elimina el patrón actual de "abrir filtro → cerrar → ver semáforo → reabrir para ajustar".

### 3.4 Layout responsivo y densidad

La app es densa por naturaleza (grids de columnas de 14 signos, tablas de análisis, decenas de parámetros). Recomendaciones:

- **Densidad "compacta" por defecto**: aplicar el override de recursos `Control.CompactDensity` de WinUI (alturas de fila ~32 px en `DataGrid`, espaciado reducido) en los grids de resultados (`ControlBoleto`, `VisorAnalisisColumnasFrm`, `CtrlDataGridViewCPs`, `CtrlDataGridViewFiguras`). Ofrecer un toggle "Densidad cómoda/compacta" en Configuración.
- **Grids grandes**: usar **`CommunityToolkit.WinUI.UI.Controls.DataGrid`** con columnas congeladas (la columna de partido/nº fija a la izquierda), virtualización (esencial: las combinaciones pueden ser miles de filas), y `Grid.RowDefinitions` con `*` para que el grid coma el espacio sobrante en lugar de scroll de ventana.
- **VisualStateManager con `AdaptiveTrigger`**: tres breakpoints — `< 640 px` (pane `LeftMinimal`, paneles de filtro a pantalla completa como `ContentDialog`), `640–1007 px` (`LeftCompact`, panel de filtro como `SplitView` overlay), `≥ 1008 px` (`Left` expandido, panel de filtro como `SplitView` inline empujando el contenido).
- **Boleto de 14 partidos**: layout en `Grid` de 14 filas (no flow), cada fila con `Match | 1 | X | 2 | semáforo`. Usar `ItemsRepeater` enlazado a la colección de partidos para que escale a la variante P15 (`AgregaP15Frm`) sin recodificar el layout.
- **Parámetros de filtros**: agrupar en `Expander` (Community Toolkit / WinUI nativo) por categoría dentro del panel, para que un filtro con 20 parámetros (p.ej. `ValoracionFrm`) no sea un muro de controles.

### 3.5 Flujo principal mejorado

Flujo actual (fricción): abrir MainForm → escribir boleto en el panel central → clicar botón de filtro en "Condiciones" → se abre diálogo modal → configurar → cerrar → mirar el `CtrSemaforo` → menú Combinación → Calcular → se abre otro form de resultados → volver atrás para ajustar.

Flujo WinUI propuesto (pipeline visible, sin ventanas que tapan):

1. **Entrada de boleto** (sección Inicio/Boleto): grid de 14 partidos con autocompletado de equipos desde `GestorEquiposFrm`, marcado 1/X/2, contador en vivo "Fijos / Dobles / Triples" (ya existe la lógica en `Pronosticos.ActualizarContador`). Importar boleto online inline.
2. **Condiciones** (misma ventana, `SplitView`): lista de los 24 filtros a la izquierda con su **semáforo Fluent** (badge de color: gris Neutro `#94A3B8` / rojo Error `#DC2626` / ámbar Warning `#D97706` / verde Success `#16A34A`) como `InfoBadge`. Al seleccionar un filtro, su panel de parámetros aparece a la derecha — **el boleto sigue visible**. Cambios se aplican en vivo; el semáforo y un contador de "columnas que pasan" se actualizan sin cerrar nada.
3. **Cálculo** (sección Cálculo, o botón primario `Calcular` en el `CommandBar`): barra de progreso determinada (`ProgressBar`) con `InfoBar` de estado; al terminar, navegación automática a Resultados.
4. **Resultados** (página): grid virtualizado con columnas reducidas, acciones `Reducir`, `Escrutar`, `Ver Boleto`/imprimir (`BoletoFrm`), exportar. `Breadcrumb` superior "Boleto › Condiciones › Resultados" para volver a cualquier etapa conservando estado.

Mejora central: **el ciclo configurar↔ver pasa de "abrir/cerrar diálogos" a "ajustar parámetro y ver el efecto en vivo"**, con el estado del pipeline siempre presente en el `NavigationView` y el `Breadcrumb`.

### 3.6 Accesibilidad 2026 — checklist concreto

Foco y teclado:
- [ ] `TabIndex` lógico fila-a-fila en el boleto (partido → 1 → X → 2), `Enter` confirma y baja a la siguiente fila.
- [ ] Aceleradores con `AccessKey` en comandos del `CommandBar` (Calcular = Alt+C, Reducir = Alt+R) replicando los atajos de menú actuales.
- [ ] `XYFocusKeyboardNavigation="Enabled"` en los grids para navegación direccional con flechas.
- [ ] Foco visible nunca suprimido; usar `FocusVisualKind="Reveal"`.
- [ ] `Escape` cierra paneles de filtro y `ContentDialog`; foco vuelve al elemento invocador.

Lectores de pantalla (`AutomationProperties`):
- [ ] `AutomationProperties.Name` en cada celda de signo ("Partido 3, opción X, no seleccionada") y en cada filtro de la lista.
- [ ] El `CtrSemaforo` → `InfoBadge` debe exponer estado por texto, no solo color: `AutomationProperties.Name="Filtro Diferencias: activo"` (cumple criterio "no depender solo del color").
- [ ] `LiveSetting="Polite"` en el contador de pronósticos y en el de "columnas que pasan" para anunciar cambios.
- [ ] `LabeledBy` enlazando cada input de parámetro de filtro a su etiqueta; `HeadingLevel` en los headers de sección del `NavigationView`.
- [ ] `InfoBar`/`ProgressBar` de cálculo con `AutomationProperties.Name` para anunciar inicio/fin.

Contraste:
- [ ] Verificar la paleta slate/indigo objetivo contra WCAG AA (4.5:1 texto): Text `#0F172A` sobre Surface `#FFFFFF` ✔; revisar Primary `#4F46E5` como fondo de texto blanco (≈ 6.3:1 ✔) y los tonos de semáforo sobre fondo claro.
- [ ] Respetar tema de sistema (claro/oscuro/alto contraste) vía `ThemeResource`; definir overrides de `HighContrast` para los colores de semáforo (Windows fuerza la paleta del SO en alto contraste — no hardcodear).
- [ ] Estados de filtro distinguibles por icono + texto + color, nunca solo color.

Tamaños táctiles y objetivo:
- [ ] Objetivos interactivos ≥ 40×40 px (recomendación Fluent táctil) en botones de signo 1/X/2 y comandos primarios, aun en densidad compacta.
- [ ] Separación mínima entre celdas de signo para evitar errores de toque en el boleto.
- [ ] Soporte de zoom de texto del SO sin recortes: layouts en `*`/`Auto`, nada de tamaños fijos en píxeles para contenedores de texto.

---

Archivos clave referenciados (rutas absolutas): `C:\Users\jr_te\Free1x2\Free1X2\UI\MainForm.cs`, `C:\Users\jr_te\Free1x2\Free1X2\UI\MainForm.Designer.cs`, `C:\Users\jr_te\Free1x2\Free1X2\UI\Filtros\` (24 forms), `C:\Users\jr_te\Free1x2\Free1X2\UI\Estadisticas\`, `C:\Users\jr_te\Free1x2\Free1X2\UI\Controls\ControlBoleto.cs`, `C:\Users\jr_te\Free1x2\Free1X2\UI\Controls\CtrSemaforo.cs`, `C:\Users\jr_te\Free1x2\Free1X2\UI\BancoPruebasFrm.cs`, `C:\Users\jr_te\Free1x2\Free1X2\UI\VisorAnalisisColumnasFrm.cs`, `C:\Users\jr_te\Free1x2\Free1X2\UI\BoletoFrm.cs`.


---


I now have all the concrete data I need. Producing the testing strategy section.

## 4. Estrategia de Pruebas

### 4.0. Diagnóstico de partida (datos reales del repo)

- **No existe ningún proyecto de pruebas** en la solución (`Glob **/*Test*.cs` → 0 resultados). Los únicos `.csproj` son `Free1X2/Free1X2.csproj` (WinForms, lógica + UI mezcladas), `Free1X2.Shared/`, `Free1X2.WebAPI/` y `Free1X2.WinUI/` (este último es un cascarón: solo `app.manifest` + `.csproj`, sin código ni XAML). Partimos de cobertura cero.
- **La lógica de dominio vive DENTRO del proyecto WinForms** (`OutputType=WinExe`, `UseWindowsForms=true`). `MotorCalculo/`, `Reduccion/`, `Escrutinio/`, `EntradaSalida/` no son ensamblados independientes. Un proyecto de test que referencie `Free1X2.csproj` arrastra todo WinForms — funciona en `net8.0-windows` pero acopla los tests al desktop.
- **Contaminación UI dentro del dominio** (confirmada por grep): `Analizador.cs` (4 usos de `System.Windows.Forms`, incl. `Application.DoEvents()` en `AnalizaColumna`), `Escrutador.cs` (`Application.DoEvents()` + `DataSet escrutinioDS` como contenedor de resultados), `ControlGrupos.cs`, `Grupo.cs`, `ControladoresImpresion.cs`. Esto significa que el motor **no es headless puro** hoy.
- **Estado global estático**: `VariablesGlobales` (clase static, `NumeroPartidos`, +20 flags `analizar*`, diccionario de idioma). Casi toda la lógica lee `VariablesGlobales.NumeroPartidos` en sus constructores (`FiltroContactos`, `Escrutador`, `GeneradorColumnas.InicializarColumnaInicial`). **Esto es el mayor riesgo de testabilidad**: los tests deben fijar este estado y aislarse entre sí.
- **Núcleo puro y altamente testeable**: `Utils/UtilColumnas.cs` (conversiones bit↔signo `ConvLongToStr`/`ConvStrToLong`/`ObtenerSigno`/`ContarBitsA1`) y `MotorCalculo/FiltroContactos.cs` (la lógica `Analizar(long)` es aritmética de bits pura, sin dependencias de UI). Son el punto de entrada ideal para la primera red de seguridad.

**Decisión transversal:** las pruebas son la red para garantizar **paridad funcional** entre la app WinForms actual y la WinUI 3 nueva. El contrato a blindar es: *misma entrada de dominio → misma salida de dominio*, independientemente de la capa de presentación.

---

### 4.1. Red de seguridad PRE-migración: pruebas unitarias sobre la lógica de dominio

Esto va **primero**, antes de tocar la UI. Objetivo: capturar el comportamiento *actual* (incluidos bugs — los congelamos como "characterization tests") para detectar cualquier regresión que la migración introduzca.

**Proyecto de tests a crear:** `Free1X2.Domain.Tests` (`net8.0-windows`, xUnit + FluentAssertions), `ProjectReference` a `Free1X2.csproj`. Se usa `net8.0-windows` porque el dominio aún arrastra `System.Windows.Forms`; cuando se extraiga el dominio (ver más abajo) se podrá bajar a `net8.0`.

**Clases críticas a cubrir, por prioridad (todas exploradas en este repo):**

| Prioridad | Clase / archivo | Por qué es crítica | Qué probar |
|-----------|-----------------|--------------------|------------|
| P0 | `Utils/UtilColumnas.cs` | Base de TODO el motor (codificación 3-bits por partido: 1/X/2). Un error aquí corrompe todo. Es pura. | `ConvStrToLong`↔`ConvLongToStr` round-trip; `ObtenerSigno(L,partido)`, `ObtenerSignoInt`, `ContarBitsA1`, `ConvPartidoStrToByte`. Tabla de casos por nº de partidos. |
| P0 | `MotorCalculo/FiltroContactos.cs` (y los otros 20 `Filtro*.cs` que implementan `IFiltro`) | Lógica matemática de filtrado pura. `Analizar(long)` no toca UI. | Para cada filtro: dada una columna `long` conocida y una config `SetNum*`, `Analizar()` devuelve el bool esperado; `AnalizarFallos()` devuelve los strings esperados. El contrato `IFiltro` (`Analizar`, `AnalizarFallos`, `ObtenNoAciertosTolerancias`, `UsaFiguras`, `Figuras`) es el mismo para los 24 → tests parametrizados sobre la lista de implementaciones. |
| P0 | `MotorCalculo/GeneradorColumnas.cs` | Genera el universo de columnas (recursivo, bit-twiddling). Define cuántas/cuáles columnas entran al análisis. | Con un pronóstico base pequeño (p.ej. 4 partidos), contar/listar columnas generadas y comparar contra valor esperado. Validar `columnaInicial` por cada `NumeroPartidos` (la tabla switch 1–16). |
| P1 | `MotorCalculo/Analizador.cs` | Orquesta filtros + grupos + IfThen; cuenta `noColsAceptadas`. **Contaminado con `Application.DoEvents()`** → ver §4.6. | Tras refactor headless: dado un set de filtros y pronóstico base, `noColsAnalizadas`/`noColsAceptadas` esperados. |
| P1 | `Escrutinio/Escrutador.cs` + `EscrutadorComb.cs` | Cuenta aciertos/premios de columnas vs ganadora. **Acoplado a `DataSet` y `DoEvents`.** | `EscrutaApuestaMultiple(apuesta,ganadora)` es pura → test directo. Para `EscrutaColumna` (privado), extraer el conteo de aciertos a método público/internal y testear matriz de premios `nc[]`. |
| P1 | `Reduccion/` (`ReductorBase`/`Base`, `JDC`, `XFSF`, `xfsfV3`, `ReductorTM`, `Redu1305Xfsf`) | Algoritmos de reducción de columnas — núcleo del valor del producto. Trabajan sobre ficheros (`ComienzaReduccion(archivoEntrada, sal, ...)`). | Golden-file tests: fichero de columnas de entrada fijo → comparar `NoColumnasFinales` y el contenido del fichero de salida byte-a-byte contra un *snapshot* aprobado. |
| P2 | `MotorCalculo/Estadisticas/CalculadorEstadisticas.cs` y `Estadistica.cs` | Histogramas/estadísticas mostradas en UI. | Dado un fichero de columnas conocido, los conteos estadísticos esperados. |
| P2 | `EntradaSalida/Archivo*.cs`, `F*Data.cs` (serialización de filtros) | Persistencia de configuración de filtros (round-trip con la UI). | Serializar→deserializar cada `F*Data` y comprobar igualdad; leer ficheros de ejemplo reales. |

**Manejo del estado global en tests:** crear un helper `EntornoQuiniela.Fijar(numPartidos)` que inicialice `VariablesGlobales` de forma determinista en el `constructor`/`IClassFixture` de cada clase de test, y ejecutar las colecciones que dependen de `NumeroPartidos` en **serie** (`[Collection("EstadoGlobal")]`) para evitar contaminación cruzada por el estado static.

**Meta de la ola 0:** ≥80 % de cobertura de líneas en `MotorCalculo` + `Reduccion` + `Escrutinio`, y 100 % en `UtilColumnas`, **antes de migrar la primera vista**.

---

### 4.2. Refactor de extracción de dominio (habilitador, no opcional)

La migración a WinUI 3 obliga a sacar el dominio del `WinExe` WinForms (WinUI no referencia `System.Windows.Forms`). Por tanto:

1. Crear `Free1X2.Domain` (`net8.0`, sin UI) y mover `MotorCalculo/`, `Reduccion/`, `Escrutinio/`, `EntradaSalida/`, `Utils/`, `VariablesGlobales`.
2. **Eliminar la contaminación UI del dominio** (los 4 ficheros del grep): sustituir `Application.DoEvents()` por un patrón `IProgress<T>`/`CancellationToken`; reemplazar el `DataSet` de `Escrutador` por un POCO (`ResultadoEscrutinio`) que la capa de UI mapea a su grid.
3. **Cada paso de este refactor se hace con la red de seguridad de §4.1 en verde** — los tests escritos contra el comportamiento WinForms actual deben seguir pasando tras mover el código. Ese es precisamente el valor de escribirlos primero.

`Free1X2.csproj` (WinForms) y `Free1X2.WinUI.csproj` referencian ambos a `Free1X2.Domain` → garantiza por construcción que las dos UIs ejecutan exactamente el mismo motor (paridad funcional gratis).

---

### 4.3. Pruebas de UI automatizadas para WinUI 3

Evaluación de opciones (estado real del ecosistema 2026):

| Opción | Veredicto | Motivo |
|--------|-----------|--------|
| **WinAppDriver** (Microsoft) | ❌ No recomendado | Prácticamente sin mantenimiento; soporte WinUI 3 frágil; sigue el protocolo WebDriver clásico. Solo como último recurso. |
| **Appium + appium-windows-driver** (sobre WinAppDriver) | ⚠️ Solo E2E mínimos | Hereda la fragilidad de WinAppDriver, pero estandarizado y CI-friendly. Útil para 2-3 smoke E2E, no como base. |
| **Microsoft.Windows.Apps.Test (WinAppSDK UI Test) / WinUI test harness con UITestControl** | ✅ Recomendado para component tests | Es el camino soportado por el equipo de WinUI; corre dentro del proceso de la app, accede al árbol XAML real, estable para ViewModels+vistas. |
| **Pruebas de ViewModel puras (sin UI)** | ✅✅ Base de la pirámide UI | Con MVVM + `CommunityToolkit.Mvvm` (ya está en el `.csproj` WinUI), los ViewModels son clases testeables con xUnit sin renderizar nada. **Aquí debe ir el grueso del testing de presentación.** |

**Recomendación concreta:**
1. **ViewModel tests (mayoría):** proyecto `Free1X2.WinUI.Tests` (`net8.0-windows10.0.19041.0`). Cada `*ViewModel` (NavigationView shell, vistas de filtros, escrutinio, boleto) se prueba en aislamiento: comandos `[RelayCommand]`, propiedades observables, validación, mapeo dominio→VM. El dominio se inyecta vía interfaces (`IAnalysisService`, `IFilterService`, `IStatisticsService` — ya existen en `Free1X2.Shared/Services/`), mockeadas con NSubstitute/Moq.
2. **E2E delgados (pocos):** Appium-Windows para 3-5 flujos end-to-end de humo (arranque → navegar a Filtros → aplicar filtro → generar → ver resultados). No replicar lógica de dominio aquí; solo verificar que la cadena UI↔VM↔dominio está conectada.

---

### 4.4. Pruebas de regresión visual (capturas comparadas)

Viable porque este entorno puede arrancar la app WinUI 3 y screenshotear ventanas (validado: renderiza Fluent nativo). Propuesta:

1. **Modo "galería de capturas":** un arranque especial de `Free1X2.WinUI` (flag `--screenshot-mode`) que instancia cada vista con datos sintéticos fijos (un fixture determinista de pronóstico/filtros), las renderiza una a una en tamaño y tema fijos (Light, escala 100 %, locale es-ES), y captura cada `Window`/`Page` a PNG en `artifacts/visual/`.
2. **Baseline aprobado:** primera ejecución → set de PNG baseline revisado y commiteado bajo `tests/visual/baseline/`.
3. **Comparación en CI:** en cada PR, regenerar y comparar contra baseline con diff perceptual (umbral de tolerancia, p.ej. ImageMagick `compare -metric AE` o `Verify.ImageMagick`/`Verify.Xunit` con su comparador de imágenes). Diferencias > umbral → fallo + PNG de diff publicado como artefacto.
4. **Paridad visual de migración (puente WinForms→WinUI):** capturar también las ventanas WinForms equivalentes (la app actual ya corre) y mantener una rejilla lado-a-lado WinForms vs WinUI por cada vista migrada. No es comparación pixel-exact (las paletas difieren a propósito — slate/indigo), sino checklist visual humano + captura archivada como evidencia por ola.
5. **Tokens de tema como aserción:** test unitario que verifica que los recursos de tema WinUI exponen los hex objetivo (`#F5F7FA`, `#FFFFFF`, `#4F46E5`, `#0F172A`, `#16A34A`, `#D97706`, `#DC2626`) y que el semáforo (rojo/amarillo/verde) y los estados de botón de filtro usan esos tokens — barato y atrapa drift de paleta sin screenshots.

Recomendación de robustez: **fijar fuente, escala DPI, idioma y datos** antes de capturar; los flakes de regresión visual casi siempre vienen de no fijar estos cuatro.

---

### 4.5. Pirámide de pruebas y smoke test

```
            /\        E2E (Appium-Windows)      ~3-5 flujos        — lentos, frágiles
           /  \       Smoke de arranque         1                  — gate de CI
          /----\      Regresión visual          1 PNG por vista    — paridad de tema
         /      \     UI component / ViewModel   por cada VM       — MVVM headless
        /--------\    Integración (dominio+IO)   reductores, escrutinio, golden files
       /          \   Unitarias de dominio       UtilColumnas, 24 filtros, generador  ← MAYORÍA
      /------------\
```

**Qué se prueba en cada nivel:**
- **Unitarias de dominio (base, la mayoría):** `UtilColumnas`, los 24 `Filtro*` (`IFiltro.Analizar/AnalizarFallos`), `GeneradorColumnas`, conteo de `Analizador`/`Escrutador`. Rápidas, sin IO, sin UI.
- **Integración:** algoritmos que tocan ficheros — `Reduccion/*` (`ComienzaReduccion` entrada→salida), `Escrutinio` sobre temporada, `EntradaSalida/Archivo*` round-trip, lectura de `F*Data`. Golden files.
- **ViewModel/UI component:** comandos y bindings de cada `*ViewModel` WinUI con dominio mockeado.
- **Regresión visual:** una captura por vista, comparada contra baseline.
- **E2E:** los flujos críticos completos a través de la UI real.

**Smoke test de arranque (gate obligatorio de CI):** lanzar `Free1X2.WinUI.exe` self-contained, esperar a que la ventana principal (shell con NavigationView) reporte `Loaded`, navegar a cada ítem del NavigationView de forma programática y confirmar que ninguna página lanza excepción ni queda en blanco; cerrar con código 0. Es la primera línea de defensa contra "compila pero crashea al abrir vista X" (riesgo alto dado que se migran ~166 forms). Tiempo objetivo < 30 s.

---

### 4.6. Criterios de aceptación por ola y gate de paridad funcional

**Olas de migración sugeridas** (UI por dominio funcional, reutilizando el mismo motor):
- **Ola 0** — Extracción de `Free1X2.Domain` + red de seguridad §4.1 (sin UI nueva todavía).
- **Ola 1** — Shell WinUI (NavigationView, tema, MainForm equivalente).
- **Ola 2** — Filtros (24 vistas).
- **Ola 3** — Generación/Reducción de columnas.
- **Ola 4** — Escrutinio + Estadísticas.
- **Ola 5** — Boleto imprimible + diálogos restantes.

**Criterios de aceptación por ola (todos deben cumplirse para cerrar la ola):**
1. **Build:** solución completa compila, 0 warnings nuevos como errores; smoke de arranque (§4.5) en verde.
2. **Paridad funcional (gate duro):** para cada función migrada en la ola, existe un test de dominio (§4.1) que demuestra **misma salida que la app WinForms** ante la misma entrada. Concretamente, el *gate de no romper paridad* es:
   - **Golden master del motor:** un set fijo de N escenarios (pronóstico base + config de filtros + acción) ejecutados contra el dominio producen un fichero de salida canónico (columnas generadas / columnas reducidas / matriz de premios / estadísticas). Ese golden master se captura una vez desde la app WinForms actual y se commitea. Tras cada ola, **la WinUI debe reproducir byte-a-byte ese mismo golden master** (vía sus ViewModels/servicios). Cualquier divergencia bloquea el merge.
   - Cobertura de dominio de la ola ≥ 80 % líneas; `UtilColumnas` y los filtros tocados al 100 %.
3. **ViewModel:** cada VM nuevo de la ola con tests de comandos y bindings en verde.
4. **Regresión visual:** baseline PNG aprobado para cada vista nueva; diffs posteriores dentro de tolerancia. Verificación de tokens de tema (§4.4.5) en verde.
5. **E2E:** si la ola añade un flujo crítico, su smoke E2E pasa.
6. **No regresión:** toda la suite de olas anteriores sigue verde (la WinForms original puede mantenerse como oráculo vivo hasta completar la migración).

**Definición operativa de "no romper paridad funcional":** dado idéntico input de dominio, `salida_WinUI == salida_WinForms` para generación, reducción, escrutinio y estadísticas, verificado por el golden master automatizado en cada PR. La presentación (paleta slate/indigo, Fluent nativo) cambia deliberadamente y **no** está sujeta a paridad pixel — solo a aprobación visual y a la verificación de tokens.

---

### Archivos clave referenciados (rutas absolutas)
- Núcleo puro / P0: `C:\Users\jr_te\Free1x2\Free1X2\Utils\UtilColumnas.cs`, `C:\Users\jr_te\Free1x2\Free1X2\MotorCalculo\FiltroContactos.cs`, `C:\Users\jr_te\Free1x2\Free1X2\MotorCalculo\GeneradorColumnas.cs`, `C:\Users\jr_te\Free1x2\Free1X2\MotorCalculo\IFiltro.cs`
- Contaminados con UI (refactor §4.2): `C:\Users\jr_te\Free1x2\Free1X2\MotorCalculo\Analizador.cs` (`Application.DoEvents()`), `C:\Users\jr_te\Free1x2\Free1X2\Escrutinio\Escrutador.cs` (`DataSet` + `DoEvents`), `C:\Users\jr_te\Free1x2\Free1X2\MotorCalculo\ControlGrupos.cs`, `C:\Users\jr_te\Free1x2\Free1X2\MotorCalculo\Grupo.cs`
- Estado global a aislar: `C:\Users\jr_te\Free1x2\Free1X2\VariablesGlobales.cs`
- Reducción (golden files): `C:\Users\jr_te\Free1x2\Free1X2\Reduccion\` (`ReductorBase.cs`, `JDC.cs`, `XFSF.cs`, `xfsfV3.cs`, `ReductorTM.cs`, `Redu1305Xfsf.cs`)
- Servicios ya abstraídos (mockeables para VM tests): `C:\Users\jr_te\Free1x2\Free1X2.Shared\Services\` (`IAnalysisService.cs`, `IFilterService.cs`, `IStatisticsService.cs`, `ITeamService.cs`)
- Destino UI (hoy vacío): `C:\Users\jr_te\Free1x2\Free1X2.WinUI\Free1X2.WinUI.csproj`


---


I have all the concrete data I need: the combinatorial scale (3^14 = 4.78M columns), the DataGridView/DataTable binding pattern used in 21+ files, the ListView results form, the absence of virtualization (no VirtualMode anywhere except one helper), and the current R2R/TieredPGO settings. Here is my section.

## 5. Performance y Empaquetado

### 5.1 Línea base y presupuesto de performance

La app WinForms actual ya viene optimizada en `Free1X2/Free1X2.csproj` (Release): `ReadyToRun=true`, `TieredCompilation=true`, `TieredPGO=true`, GC workstation concurrente. Eso fija el listón: la versión WinUI 3 **no debe degradar** el arranque ni la fluidez frente a WinForms, que es el framework de arranque más rápido del ecosistema .NET de escritorio. Esto es el riesgo central de esta migración: WinUI 3 arranca más lento que WinForms por diseño (carga del Windows App Runtime + parsing XAML).

Metas concretas (medidas en frío y en caliente, ver 5.5):

| Métrica | WinForms actual (referencia) | Meta WinUI 3 | Tope inaceptable |
|---|---|---|---|
| Cold start (primer arranque tras boot) | ~0.8–1.5 s | ≤ 2.5 s | > 4 s |
| Warm start (segundo arranque) | ~0.4–0.8 s | ≤ 1.2 s | > 2 s |
| Memoria en reposo (MainForm/Shell cargado) | ~80–120 MB | ≤ 180 MB | > 300 MB |
| Memoria con grid de 1M+ filas abierto | (hoy se cuelga, ver 5.2) | ≤ 400 MB con virtualización | OOM |
| Frame time navegación entre páginas | n/a | ≤ 16 ms (60 fps) | janks visibles |
| Scroll en grid grande | hoy bloqueante | 60 fps sostenido | < 30 fps |

La meta de fluidez es alcanzable y debería **superar** a WinForms: WinUI 3 compone en GPU (DirectComposition), mientras los `DataGridView`/`ListView` GDI+ actuales repintan en CPU. El reto es exclusivamente el arranque y el primer render.

### 5.2 Grids grandes: el problema real y su solución en WinUI 3

Este es el punto crítico de performance de la app. El dominio es combinatorio: `GeneradorColumnas` (`Free1X2/MotorCalculo/GeneradorColumnas.cs`) opera sobre 3^14 = **4.782.969** columnas para 14 partidos (y `columnaBase = 281474976710655` para 16+). Los grids de resultados pueden tener cientos de miles a millones de filas.

El patrón actual es **anti-performante** y debe NO portarse tal cual. En `Free1X2/UI/Controls/Analisis/CtrlDataGridViewCPs.cs` (y replicado en ~21 archivos con `DataGridView`, 117 ocurrencias de `Rows.Add`/`DataSource`), cada fila se materializa en un `DataTable` en memoria y se hace `dataGridView1.DataSource = dsFiguras.Tables["CPs"]`. No existe virtualización en ningún sitio del repo: `Grep` de `VirtualMode`/`VirtualListSize` devuelve **cero** usos reales (la única coincidencia es `ModernControls.cs`). `ResultadosCalculoMultipleFrm.cs` usa un `ListView` en modo Details sin `VirtualMode`. Con millones de filas esto materializa todo en RAM y satura GDI+.

**Recomendación WinUI 3:**

1. **No usar `DataGrid` del Community Toolkit para los grids enormes.** Es funcional pero su virtualización por filas no escala bien a millones de elementos y su rendimiento de scroll degrada. Reservarlo solo para grids pequeños/medios (filtros, estadísticas con decenas/cientos de filas).

2. **Para los grids combinatorios masivos: `ListView` (o `ItemsRepeater` dentro de `ScrollViewer`) con virtualización + `ISupportIncrementalLoading` + datos calculados perezosamente.** No materializar las 4,78M de combinaciones; exponerlas a través de una colección virtual que las genere/lea por página bajo demanda desde `GeneradorColumnas`/los archivos de columnas.

3. **`x:Bind` compilado, no `Binding`.** En plantillas que se reciclan millones de veces la diferencia es enorme: `x:Bind` resuelve en compilación (sin reflexión, sin `INotifyPropertyChanged` por defecto), reduce tiempo de creación de cada celda y elimina warnings de binding en runtime. Usar `Mode=OneTime` para celdas de solo lectura (la mayoría de estos grids).

Ejemplo de la colección incremental (envuelve el generador de dominio sin tocarlo):

```csharp
public sealed class ColumnasVirtuales : ObservableCollection<ColumnaVm>, ISupportIncrementalLoading
{
    private readonly GeneradorColumnas _gen;   // lógica de dominio reutilizada
    private long _cargadas;
    private readonly long _total;              // p.ej. 4_782_969

    public bool HasMoreItems => _cargadas < _total;

    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        => LoadPageAsync(count).AsAsyncOperation();

    private async Task<LoadMoreItemsResult> LoadPageAsync(uint count)
    {
        var pagina = await Task.Run(() => _gen.ObtenerRango(_cargadas, count)); // off-UI-thread
        foreach (var c in pagina) Add(new ColumnaVm(c));
        _cargadas += pagina.Count;
        return new LoadMoreItemsResult { Count = (uint)pagina.Count };
    }
}
```

```xml
<ListView ItemsSource="{x:Bind Vm.Columnas}">  <!-- virtualización por defecto -->
  <ListView.ItemTemplate>
    <DataTemplate x:DataType="vm:ColumnaVm">
      <TextBlock Text="{x:Bind Signos, Mode=OneTime}"/>  <!-- x:Bind, OneTime -->
    </DataTemplate>
  </ListView.ItemTemplate>
</ListView>
```

Notas: mantener el `ItemsStackPanel` virtualizado (no envolver el `ListView` en un `StackPanel`/`ScrollViewer` que mate la virtualización — error #1 de perf en WinUI). Para celdas tipo matriz (14+ columnas por fila, como en `CtrlDataGridViewCPs`), usar un `ItemTemplate` con `ItemsRepeater` horizontal en lugar de DataGrid, o un único `TextBlock` con la fila formateada para minimizar elementos visuales por fila. El cálculo (`Task.Run`) **siempre fuera del UI thread**: la lógica de `MotorCalculo` es síncrona y bloquearía la composición.

### 5.3 Self-contained vs framework-dependent (Windows App Runtime)

Hay dos ejes independientes que decidir: (a) el .NET runtime, (b) el Windows App SDK runtime.

| Estrategia | Tamaño aprox. | Pros | Contras |
|---|---|---|---|
| **Framework-dependent + WinAppSDK compartido** | ~10–30 MB | distribución mínima | requiere instalar .NET 8 Desktop Runtime **y** Windows App Runtime en cada máquina; soporte frágil para usuarios no técnicos |
| **Self-contained + WinAppSDK packaged (unpackaged)** | ~150–250 MB | un solo paquete, cero dependencias previas, "descomprime y ejecuta" | distribución grande |

**Recomendación: self-contained, unpackaged (sin MSIX), con Windows App SDK en modo *self-contained*** (`<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>` + `<SelfContained>true</SelfContained>`). Justificación concreta para Free1X2: es una herramienta de hobby/análisis distribuida a usuarios finales no técnicos de la comunidad de quinielas (hoy se distribuye como `.exe` WinExe directo, sin instalador ni Store). Pedirles instalar dos runtimes por separado es inviable. El brief además confirma que self-contained "compila, corre y renderiza Fluent nativo en este entorno". El coste (150–250 MB) es aceptable para escritorio descargado una vez.

Empaquetado: **unpackaged** (no MSIX) para conservar el modelo "ejecutable suelto" actual. Si más adelante se quiere actualización automática o Store, MSIX se puede añadir sin reescribir la app.

TFM objetivo confirmado: `net8.0-windows10.0.19041.0`.

**AOT / ReadyToRun / Trimming — evaluación para WinUI 3:**

- **Native AOT: NO aplica.** WinUI 3 / Windows App SDK 1.6 no soporta Native AOT de forma completa (XAML compilado y la generación de tipos COM/WinRT no son AOT-safe). Descartar.
- **ReadyToRun (R2R): SÍ, recomendado.** Reduce JIT en el arranque precompilando a código nativo. La app WinForms ya lo usa (`<ReadyToRun>true</ReadyToRun>`); mantenerlo en WinUI mitiga directamente el arranque más lento del runtime. Es el mayor "win" de arranque disponible para WinUI 3.
- **Trimming: NO en la primera entrega.** WinUI 3 + reflexión + el patrón actual (DataTable/DataSet en `MotorCalculo`, `System.ServiceModel`, `Microsoft.Windows.Compatibility`) son propensos a romperse con trimming. El riesgo de fallos en runtime difíciles de diagnosticar supera el ahorro de tamaño. Reconsiderar más tarde con `PublishTrimmed` + análisis de warnings, solo si el tamaño se vuelve un problema real.
- **TieredPGO: mantener** (`true`), heredado del csproj actual; mejora el estado estable sin coste.

Config recomendada de publicación:
```xml
<SelfContained>true</SelfContained>
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
<RuntimeIdentifier>win-x64</RuntimeIdentifier>
<PublishReadyToRun>true</PublishReadyToRun>
<PublishTrimmed>false</PublishTrimmed>
<TieredPGO>true</TieredPGO>
```

### 5.4 Riesgos de performance específicos de WinUI 3 y mitigaciones

1. **Arranque del Windows App Runtime + COM/WinRT activation** (el principal sobrecoste vs WinForms). Mitigación: R2R activado; mantener `App.xaml`/recursos del arranque mínimos; **no** cargar todo el árbol de navegación al inicio.
2. **XAML parsing en el primer render.** Con ~166 vistas a portar, cargar todas de golpe mataría el arranque. Mitigación: **navegación perezosa** en `NavigationView` — instanciar cada `Page` solo al navegar a ella (no precrear); usar `NavigationCacheMode=Disabled` para vistas pesadas/raras y `Required` solo para las 2–3 más usadas. Mantener el shell inicial (solo `NavigationView` + página de inicio) lo más ligero posible.
3. **`{Binding}` con reflexión en plantillas recicladas** → ralentiza scroll en grids grandes. Mitigación: `x:Bind` (ver 5.2).
4. **Diccionarios de recursos grandes / `ResourceDictionary` monolítico.** Mitigación: dividir por feature; cargar bajo demanda; no abusar de estilos implícitos globales costosos.
5. **Romper la virtualización accidentalmente** (envolver listas en `StackPanel`/`ScrollViewer`, alturas no acotadas, `ItemsRepeater` sin layout virtualizado). Mitigación: revisión obligatoria de cada vista con grid; verificar que el panel usado es `ItemsStackPanel`/`StackLayout` virtualizado.
6. **Cálculo de dominio síncrono bloqueando el UI thread.** `MotorCalculo` es CPU-intensivo y todo síncrono; en WinForms congela la ventana, en WinUI congela la composición (peor percepción). Mitigación: todo cálculo en `Task.Run` + `IProgress<T>` para barras de progreso; nunca tocar `MotorCalculo` desde el hilo de UI.
7. **DataTable/DataSet como modelo de datos** (patrón omnipresente en los `CtrlDataGridView*`). Es memoria-pesado para millones de filas. Mitigación: en la capa WinUI, sustituir el binding a `DataTable` por VMs ligeros calculados perezosamente (5.2); el `MotorCalculo` puede seguir produciendo lo que produce, pero la UI no lo materializa entero.

### 5.5 Telemetría y medición repetible

Objetivo: medir arranque y memoria de forma reproducible para comparar contra la base WinForms y validar las metas de 5.1.

**Arranque (cold/warm):**
- Instrumentar en código el momento de "primer frame interactivo": registrar `Stopwatch` desde el inicio de `App.OnLaunched` hasta el `Loaded` de la primera página, y emitir vía `EventSource`/ETW (no `Console`).
- Medición externa repetible con **PerfView** o **WPR/WPA** (Windows Performance Recorder): perfil de arranque, separar cold (limpiar working set / primer arranque tras reinicio) vs warm. Repetir N≥10 ejecuciones y reportar mediana + p95, no la media (el primer JIT distorsiona).
- Script de automatización: bucle que lanza el `.exe`, espera al evento ETW de "first frame", registra el delta, cierra y repite. Limpiar caché de ficheros entre cold runs.

**Memoria:**
- `dotnet-counters monitor -p <pid>` para `Working Set`, `GC Heap Size`, `Gen 0/1/2`, `LOH` en vivo durante escenarios (reposo, grid grande abierto, tras cerrar grid → verificar que se libera, sin fugas).
- `dotnet-trace` / `dotnet-gcdump` para snapshots de heap antes/después de abrir y cerrar un grid de 1M+ filas; comparar dumps para detectar retención (clave porque hoy todo se materializa en `DataTable`).
- Validar el presupuesto de virtualización: working set con grid grande abierto debe quedar **plano** al hacer scroll (señal de que la virtualización funciona); si crece con el scroll, la virtualización está rota.

**Fluidez:**
- WPA con el perfil GPU/composición para frame time y janks; objetivo 60 fps en navegación y scroll (5.1).

**CI/regresión:** integrar la medición de cold/warm start (mediana de N runs) como gate en el pipeline de release, fallando si supera los topes de 5.1, para evitar que la app WinUI degrade silenciosamente por debajo de la base WinForms.


---
