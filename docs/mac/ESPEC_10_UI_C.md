# ESPEC 10 — UI C: shell, controles transversales y pantallas 74–105 (+ Acerca de, Ayuda, Créditos)

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Convenciones en
> [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Describe **qué hace hoy** el código WinUI 3
> para reescribirlo en SwiftUI sin mirar el C#. Las decisiones de diseño y de área Mac son
> **propuestas** (R2): las decide el dueño.

**Prefijos de ruta** (para abreviar `fichero:línea`; todas relativas a la raíz del repo):
`W:` = `Free1X2.WinUI/` · `P:` = `Free1X2.WinUI/Views/Ported/` · `D:` = `Free1X2.Domain/`.

**Alcance.** (1) Shell y lo transversal: `App`, `MainWindow`, `Navigation`, `Controls/`, `Services/`,
`Themes/`, `Strings/`. (2) Las pantallas 74–108 por orden alfabético de ViewModel (`ls *ViewModel.cs | sed -n '74,108p'`).
La lista solo tiene **105 ViewModels** (para 108 páginas): el rango 74–108 devuelve **32 pantallas**, de `MultiplicadorFrm` (74) a
`aidomnou` (105). Las tres páginas sin ViewModel (`AcercaDeFrm`, `AyudaFrm`, `CreditosFrm`) pertenecen al shell y se documentan en §1.11.
Total documentado: **32 pantallas con ViewModel + 3 páginas de shell = 35**.

**Cómo leer «lógica de negocio en el ViewModel».** Muchas pantallas WinUI no llaman al motor: el
algoritmo vive dentro del propio `*ViewModel.cs`. Cada pantalla lo lista en el apartado
**Lógica en el ViewModel (→ `Free1X2Core/CasosDeUso`)** con pseudocódigo; cada bloque lleva un
identificador `CU-Cnn` (único en este documento, `CU-C01`…`CU-C62`) que se recoge en §3. Al final: §3 tabla resumen, §4 «Abiertas», §5 recuento.

---

## 1. Shell y componentes transversales

### 1.1 Secuencia de arranque

Orden real (todo en el hilo de UI, sin `await`):

| # | Paso | Qué hace | Evidencia |
|---|------|----------|-----------|
| 1 | `App()` | Registra `UnhandledException`: marca `Handled=true`, **siempre** escribe `Log.Error`, y si no está ya avisando (`_enManejadorDeExcepciones`) muestra «Se produjo un error inesperado:\n\n{mensaje}» con `AppServices.MostrarError`. | `W:App.xaml.cs:21,28-47` |
| 2 | `AsegurarCarpetasDeTrabajo` | Crea **junto al exe** 10 carpetas: `Columnas, Combinaciones, Condiciones, Filtros, Ganadoras, Informes, Jornadas, Temp, Comunicaciones, Documentacion`. Un fallo solo se registra. | `W:App.xaml.cs:102-120` |
| 3 | `IdiomaApp.Aplicar()` | Lee `idioma.json`, fija `PrimaryLanguageOverride` ("es"/"en") **antes** de crear la ventana. | `W:App.xaml.cs:57`, `W:Services/IdiomaApp.cs:99-103` |
| 4 | `SembrarJornadaDesdeCache` | Si existe `jornada-{es,mx}.json`, toma el **más reciente por fecha de escritura** (`JornadaCache.PaisMasReciente`), lo reparsea (`JornadaCache.TryCargar` → `JornadaQuinielaParser.Parse`) y lo deja en `AppState.JornadaActual`. **Sin red.** Sin caché → no-op (arranque manual). | `W:App.xaml.cs:76-93`, `W:Services/JornadaCache.cs:119-146,153-180` |
| 5 | `new MainWindow()` | `InitializeComponent`; título «Free1X2»; `TemaApp.Aplicar(RaizVentana)` (tema antes de pintar); tamaño 1020×760 + icono + mínimo 900×600; `ConstruirToolbar()`; `ConstruirMenus()`; atajos; `ContentFrame.Navigate(MainPage)`; engancha `Closed → GuardarBarrasHerramientas`; si `FREE1X2_SMOKE=1` arranca el smoke. | `W:MainWindow.xaml.cs:22-53` |
| 6 | `AppServices.Inicializar(MainWindow)` | Guarda ventana y `DispatcherQueue` (necesarios para diálogos y pickers). | `W:App.xaml.cs:60`, `W:Services/AppServices.cs:46-50` |
| 7 | `CablearHooksDominio` | Conecta los hooks del dominio (ver 1.7). | `W:App.xaml.cs:61,127-208` |
| 8 | `MainWindow.Activate()` | Muestra la ventana. | `W:App.xaml.cs:62` |

Notas: (a) la `MainPage` se construye en el paso 5, **antes** de los pasos 6-7 (los hooks aún son no-op
durante su constructor). (b) `App.xaml` **no** fija `RequestedTheme` (lo decide `TemaApp`): `W:App.xaml:6-10`.
(c) `App.xaml` solo fusiona `XamlControlsResources`, `Tokens.xaml` y `Styles.xaml` (`W:App.xaml:14-16`).

**Propuesta macOS.** `@main struct Free1X2App: App` con una `WindowGroup` (una sola ventana de documento
lógico: ver «Abiertas» sobre multiventana), `@State var sesion = Sesion()` inyectada con `.environment`.
`AppDelegate` (`NSApplicationDelegateAdaptor`) para `applicationDidFinishLaunching`: crear
`~/Library/Application Support/Free1X2/{Columnas,Combinaciones,Condiciones,Filtros,Ganadoras,Informes,Jornadas,Temp,Comunicaciones,Documentacion}`
(hoy van junto al exe, `W:App.xaml.cs:102-120`; el bundle de macOS no es escribible, PLAN §2), sembrar
`JornadaActual` desde la caché y registrar el manejador global de errores (`NSSetUncaughtExceptionHandler` solo
cubre ObjC; en Swift los errores van por `throws` → `AlertaError` + log). El tema y el idioma se aplican antes de
mostrar la primera vista (`.preferredColorScheme`, `.environment(\.locale)`).

### 1.2 Ventana, navegación y estado compartido

| Aspecto | Comportamiento actual | Evidencia |
|---------|----------------------|-----------|
| Estructura | `Grid` de 3 filas: `MenuBar` · panel de herramientas (alto mín. 63) · `Frame` de contenido. No hay `NavigationView`: el `PortedPagesRegistry` (108 entradas, categorías Filtros 27, Columnas y combinación 21, Boleto e impresión 15, Análisis 15, Otras 9, Cuadros CPs 7, Estadísticas 5, Equipos 3, Ajustes 3, Ayuda 3; todos con `Glyph` vacío) **solo lo usa el smoke**. | `W:MainWindow.xaml:241-273`, `W:Navigation/PortedPages.cs:9-137`, `W:MainWindow.xaml.cs:826` |
| Barra de título | Estándar de Windows (no se extiende el contenido). | `W:MainWindow.xaml.cs:29` |
| Tamaño | Inicial 1020×760; mínimo 900×600 (en DIP, escalado por DPI vía `WM_GETMINMAXINFO`). | `W:MainWindow.xaml.cs:63,99-100,112-126` |
| Navegar | `Navegar(page)`: solo navega si `CurrentSourcePageType != page` (no apila duplicados). `NavegarA(page)`: **siempre** navega (lo usa el visor de análisis). | `W:MainWindow.xaml.cs:738-742,752` |
| Atrás | `Esc` → `Frame.GoBack()` si `CanGoBack`. Cada pantalla de condición tiene además su `Volver` (`Frame.GoBack`). | `W:MainWindow.xaml.cs:773-774`, `P:NoVariantesFrmPage.xaml.cs:20` |
| Estado compartido | Singleton `AppState`: `Analizador`, `GrupoPantalla`, `NombreArchivoComb`, `ArchivoFiltroCols`, `JornadaActual`; eventos `Cambiado` y `JornadaCambiada`. | `W:Services/AppState.cs:24-166` |
| «Handoff» estático | Estáticos que un productor deja y el consumidor limpia: `AppState.GrupoEnEdicion` (grupo que edita una página de condición), `AppState.UsarAnalizadorCompartido` (cálculo usa el analizador de Inicio), `VisorAnalisisColumnasFrmViewModel.UltimoContenedor/UltimoGrupo`, `VisorEstadisticasViewModel.UltimasEstadisticas`. | `W:Services/AppState.cs:73,84`, `W:App.xaml.cs:187-190`, `P:NoVariantesFrmViewModel.cs:199` |
| Acciones sobre Inicio | `NavegarConAccion(AccionInicio)`: si ya estás en `MainPage` ejecuta el comando sobre la **instancia viva** (conserva el boleto en edición); si no, navega a `MainPage` pasando el token y `OnNavigatedTo` lo ejecuta. Siempre dentro de `EjecutarObservandoErrores`. | `W:MainWindow.xaml.cs:794-808`, `W:Views/MainPage.xaml.cs:44-52` |
| Idioma en vivo | Cambiar idioma persiste y **recrea la página actual** (re-navega y quita la duplicada de la pila): pierde el estado en edición. | `W:MainWindow.xaml.cs:698-712` |

`AccionInicio` = `{Ninguna, NuevaCombinacion, AbrirCombinacion, GuardarCombinacion, GuardarCombinacionComo,
GuardarEquipos, AbrirEquipos, BorrarTemporales, BorrarInformes}` (`W:Views/MainPageViewModel.cs:25-36`).
Efecto de cada una (`W:Views/MainPageViewModel.cs:567-788`):

| Acción | Qué hace |
|--------|----------|
| Nueva combinación | `AppState.NuevaCombinacion()` (nuevo `Analizador`, `GrupoPantalla=0`, rutas vacías) + `Boleto.VaciarEquipos()` (todos «? - ?») + `Boleto.ReiniciarTriples()` (14 × «1,X,2»). **Sin confirmación** (aunque `AppServices.ConfirmarAsync` cita «Nueva/Abrir» como usos, `W:Services/AppServices.cs:52-57`). |
| Abrir combinación | Picker `.comb`/`.xml` → `ArchivoCombinacion.AbrirArchivoCombinacion` → `LeeEquipos`, `LeeFiltroColumnas`, `CargaControladorGrupos(analizador.CtrlGrupos)`, `analizador.IfThen = CargaIfThen()`, `LeePronosticos` → `SetPronostico(i,…)`; reemplaza `AppState.Analizador`, `ArchivoFiltroCols`, `NombreArchivoComb`, `GrupoPantalla=0`; `Boleto.SetEquipos`. Error → «No se pudo abrir la combinación: …». |
| Guardar combinación | `Boleto.VolcarPronosticosAlMotor()`; si no hay `NombreArchivoComb` picker «Combinacion» `.comb`; `ArchivoCombinacion{Equipos=Boleto.DevolverEquipos(), Pronosticos, ArchivoColumnasFiltro, Grupos, CtrlGrupos, IfThen}.GuardaArchivo()`. |
| Guardar como | Siempre picker; fija `NombreArchivoComb` y reutiliza Guardar. |
| Guardar equipos | Picker `.txt` «Equipos»; escribe una línea por partido (`DevolverEquipos`) en UTF-8 (`File.WriteAllLines`). |
| Abrir equipos | Picker `.txt`; lee `NumeroPartidos` líneas (faltantes → «? - ?») → `Boleto.SetEquipos`. |
| Borrar temporales | Confirma «¿Borrar las combinaciones temporales?»; borra de `Temp/` los ficheros cuyo nombre contenga `_tmp.comb`. |
| Borrar informes | Confirma «¿Borrar todos los Informes generados?»; borra de `Informes/` los ficheros cuyo nombre empiece por «Informe» (longitud > 7). |

**Propuesta macOS.** `NavigationSplitView` con **barra lateral de las 10 áreas** (Boleto · Condiciones y filtros ·
Cálculo · Reducción · Escrutinio · Análisis · Columnas probables · Herramientas · Online · Ajustes), un `detail`
con `NavigationStack` por área (la pila sustituye a `Frame.BackStack`; `⌘[` / Esc = atrás), y las pantallas de
condición como **hoja modal** (`.sheet`) o **inspector** (`.inspector`) sobre el boleto. Los «handoffs»
estáticos desaparecen: la navegación lleva su payload (`path.append(.visorAnalisis(contenedor, grupo))`) o lo
lee de `Sesion` (`@Observable`, M-R4). `AppState` → `Sesion` con `analizador`, `grupoPantalla`, `rutaCombinacion`,
`rutaFiltro`, `jornadaActual`. Documento: usar `DocumentGroup`/`FileDocument` para `.comb` da gratis
Nueva/Abrir/Guardar/Guardar como/Reciente y el marcador «edited»; alternativa (más fiel) `WindowGroup` + comandos propios.

### 1.3 Árbol de menús completo

Solo hay 7 menús de nivel superior, en este orden (el mismo que el `MainForm`, `W:MainWindow.xaml.cs:157-163`).
Los items **no llevan atajo de teclado** (no hay `KeyboardAcceleratorTextOverride`); los únicos atajos son los de 1.4.
Cada item navega con `Navegar(page)`. Columna «Mac» = propuesta de comando nativo.

| Menú | Texto del item | Pantalla destino (clase) | Mac (propuesta) |
|------|----------------|--------------------------|-----------------|
| **Free1x2** (`:165-174`) | Inicio | `MainPage` | Ventana principal / `⌘1` Boleto |
| | Configuración… | `ConfiguracionFrmPage` | App menu «Ajustes…» `⌘,` (escena `Settings`) |
| | Configurar análisis… | `ConfiguracionAnalisisFrmPage` | Pestaña de `Settings` |
| | Acerca de… | `AcercaDeFrmPage` | App menu «Acerca de Free1X2» (`orderFrontStandardAboutPanel`) |
| | Créditos… | `CreditosFrmPage` | Hoja desde «Acerca de» |
| | Salir | cierra la ventana (`Close()`, **sin confirmación**) | `⌘Q` estándar |
| **Archivo** (`:176-180`) | Boleto / combinación (Inicio) | `MainPage` | — (redundante con Inicio) |
| | Gestión de equipos… | `GestorEquiposFrmPage` | Área Boleto / Online |
| | Importar / exportar columnas… | `ImportExportFrmPage` | Archivo ▸ Importar/Exportar… |
| **Ver** (`:186-208`) | Inicio | `MainPage` | — |
| | Ver boletos… | `VerBoletosPage` | Área Escrutinio/Boleto |
| | Gráfico de columnas… | `GraficoColumnasFrmPage` | Área Análisis |
| | Estadísticas… | `AnastaticsPage` | Área Análisis |
| | Configuración… | `ConfiguracionFrmPage` | `⌘,` |
| | *(separador)* ▸ **Barras de Herramientas** (submenú, 6 `ToggleMenuFlyoutItem`: Filtros, Free1X2, Operaciones, Utilidades, Combinación, Archivo; marcado = visible) | alterna grupo de la barra (`:570-608`) | «Personalizar barra de herramientas…» nativo |
| | Listado de condiciones | `ListadoCondicionesFrmPage` | Inspector del boleto |
| | *(separador)* ▸ **Tema** (3 `RadioMenuFlyoutItem`: Claro, Oscuro, Sistema) | `TemaApp.Cambiar` (`:618-646`) | Ajustes ▸ Apariencia |
| | *(separador)* ▸ **Idioma** (2 radios: Español, English) | `IdiomaApp.Cambiar` + recarga (`:656-687`) | Ajustes ▸ Idioma |
| **Combinación** (`:210-233`) | Calcular… | `CalculaColumnasFrmPage` | Área Cálculo `⌘R` |
| | Calcular varias… | `CalculaColumnasMultipleFrmPage` | Cálculo |
| | Ver boletos… | `VerBoletosPage` | Escrutinio |
| | Ver boletos en editor de texto… | `VerBoletosEnEditorFrmPage` | Herramientas |
| | Imprimir boletos… | `ImprimirBoletoFrmPage` | Archivo ▸ Imprimir `⌘P` |
| | Reducir… | `ReductorFrmPage` | Área Reducción |
| | Escrutinios… | `EscrutiniosFrmPage` | Área Escrutinio |
| | Escrutar combinaciones… | `EscrutarCombinacionesFrmPage` | Escrutinio |
| | Analizar combinación… | `AnalizarCombinacionFrmPage` | Análisis |
| | Gráfico de columnas… | `GraficoColumnasFrmPage` | Análisis |
| | Probabilidades… | `ProbabilidadPremiosPage` | Análisis |
| | Estadísticas… | `AnastaticsPage` | Análisis |
| | Añadir Pleno al 15… | `AgregaP15FrmPage` | Herramientas |
| **Filtros** (`:235-241`) | Combinar filtros… · Diferencias entre filtros… · Filtro Coincidencias… · Filtro Aidomnou… · Filtro Pim… | `CombinarFiltrosPage` · `DiFiltrosPage` · `CoincidenciasPage` · `aidomnouPage` · `GeneraPimPage` | Condiciones y filtros / Herramientas |
| **Operaciones** (`:243-248`) | Álgebra de columnas… · Transposición… · Multiplicador… · Fraccionador… · Rotación de signos… | `AlgebraColumnasFrmPage` · `TransposicionFrmPage` · `MultiplicadorFrmPage` · `FraccionadorFrmPage` · `RotacionDeSignosFrmPage` | Herramientas |
| **Utilidades** (`:250-274`) | Sube categoría… · Modificador %… · *(sep.)* Generador CP… · Columnas GEPT… · *(sep.)* Diferencias entre columnas… · Ordenar por probabilidad… · *(sep.)* Selector JuanM… · Selector MarioSan… · *(sep.)* Rentabilidad… · Tramificar… · Premiadas… · Estimación de premios… · Banco de pruebas… · *(sep.)* Compresor z3q… · EstuCol… | `SubirCategoriaFrmPage` · `ModificadorFrmPage` · `GenerarCPsPage` · `GEPTFrmPage` · `DifColsPage` · `OrdenarPorProbabilidadFrmPage` · `SelecJMPage` · `SelectorMSPage` · `RentabilidadFrmPage` · `TramificarFormPage` · `PremiadasFrmPage` · `EstimadorPremiosFrmPage` · `BancoPruebasFrmPage` · `CompresorPage` · `EstucolFrmPage` | Herramientas / Columnas probables / Análisis |

Recuento: **7 menús, 53 items de navegación + 11 items conmutables** (6 barras + 3 tema + 2 idioma) = **64 entradas**
(más 3 submenús). Los iconos son glifos «Segoe Fluent Icons» (códigos hex en cada línea; el título superior no lleva
icono, `:277-281`) → en Mac, SF Symbols.

**Diferencias menú ↔ barra ↔ manual** (relevantes para el rediseño):
- El menú **Archivo** de WinUI solo tiene 3 items; Nueva / Abrir / Guardar / Guardar como combinación, Abrir/Guardar equipos,
  Obtener boletos online, Borrar temporales/informes **solo existen en la barra y en atajos** (`W:MainWindow.xaml.cs:366-375`).
  El manual los sitúa en el menú Archivo (`docs/MANUAL_USUARIO.md:125-135`). En Mac deben ir a **Archivo**.
- **Ayuda** (`AyudaFrmPage`) solo en barra y `F1` (`:357,771`); no hay menú Ayuda (el README del proyecto lo cita, `W:README.md:24`: ver «Abiertas»).
- Solo en barra: Análisis de columnas (`AnalizarFicheroFrmPage`), Análisis de fallos (`ColGanadoraFrmPage`), Análisis de signos (`VSignosFrmPage`),
  Análisis de grupos (`AnaCombiPage`), Reducciones perfectas (`FrmReducidasPerfectasPage`), Dependencia lineal (`FrmDependenciaLinealPage`) (`:397-400,431-433`).
- `bEscrutarComb` está **deshabilitado** en la barra («handler vacío en el original») pero el menú sí abre `EscrutarCombinacionesFrmPage` (`:396` vs `:226`).
- `bAnalisisGrafico` se omite a propósito (`Visible=false` en el original) (`:399`).

### 1.4 Barra de herramientas y atajos

6 grupos (`GrupoBarra`, orden visual `:343,350-433`), icono PNG por botón (`Assets/Toolbar/{botón}.png`, 16×16 en
botón 26×26, `:461-492`), tooltip = nombre accesible. **55 botones** (47 de navegación + 8 `HerramientaAccion`).
Panel propio `ToolbarWrapPanel` que salta de fila cuando no caben (`W:Controls/ToolbarWrapPanel.cs:25-110`).

| Grupo (separador al final) | Botones (tooltip → destino/acción) |
|----------------------------|-------------------------------------|
| **Free** `:353-358` | Salir (cierra) · Configuración · Configurar Análisis · Ayuda · Acerca de |
| **Archivo** `:365-375` | *Acciones sobre Inicio:* Guardar equipos · Nueva combinación · Abrir combinación · Guardar combinación · Guardar combinación como · Borrar archivos temporales · Abrir equipos · Borrar Informes de Error. *Navegan:* Obtener Boletos Online (`DescargaBoletoFrmPage`) · Gestión de Equipos (`GestorEquiposFrmPage`) |
| **Operaciones** `:379-384` | Algebra · Transposición · Multiplicación · Fraccionar · Rotación de signos |
| **Combinación** `:388-403` | Calcular combinación · Calcular múltiples · Ver boletos · Imprimir boletos · Reducir · Escrutinio · *(Escrutar combinaciones, deshabilitado)* · Análisis de columnas · Análisis de fallos · Análisis de signos · Probabilidades · Estadísticas · Añadir pleno al 15 |
| **Filtros** `:407-412` | Combinar filtros · Diferencias de filtros · Filtro de coincidencias · Filtro Aidomnou · Filtro Pim |
| **Utilidades** `:416-433` | Subir categoría · Modificador de porcentajes · Generador de CPs · Diferencias entre columnas · Ordenar por probabilidad · Selector JuanM · Selector MarioSan · Rentabilidad · Columnas GEPT · Tramificar · Premiadas · Estimación de premios · Banco de pruebas · Importar / Exportar · Análisis de grupos · Reducciones perfectas · Dependencia lineal |

- **Visibilidad por grupo**: se siembra de `VariablesGlobales.MostrarTsFree/Archivo/Operaciones/Combinacion/Filtros/Utilidades`
  (cargadas de `parametros.free1x2`, `:559-568`); el grupo **Filtros se fuerza siempre visible al arrancar** (`:445`) y el check
  del menú también (`:594`). Al **cerrar la ventana** se persisten con
  `new AConfiguracion().GuardarToolBarsVisibles(Free, Filtros, Combinacion, Operaciones, Archivo, Utilidades)` (orden de argumentos
  exacto, `:717-729`); un fallo solo se registra. Formato del fichero → ESPEC_07.
- **Atajos de ventana** (a nivel de `RaizVentana`, funcionan en cualquier página; «funcionalidad nueva, decisión del dueño»; `:759-787`):

| Atajo | Acción |
|-------|--------|
| `Ctrl+N` | `NavegarConAccion(NuevaCombinacion)` |
| `Ctrl+O` | `NavegarConAccion(AbrirCombinacion)` |
| `Ctrl+S` | `NavegarConAccion(GuardarCombinacion)` |
| `F5` | Navega a `CalculaColumnasFrmPage` (sin volcar el boleto ni activar el analizador compartido) |
| `F1` | Navega a `AyudaFrmPage` |
| `Esc` | Atrás (`GoBack` si es posible) |

**Propuesta macOS.** Una única `.toolbar(id: "principal") { ToolbarItem(id:…) }` **personalizable** (el sistema persiste qué
items se ven → sustituye al submenú «Barras de Herramientas» y a `GuardarToolBarsVisibles`; no se replican 6 barras ni los 55
iconos). Atajos: `⌘N`, `⌘O`, `⌘S`, `⇧⌘S`, `⌘R` (Calcular; `F5` queda como alternativa), `⌘?` (Ayuda → `NSHelpManager`/ventana de ayuda), `⌘[` atrás.
Recomendación: Salir/Configuración/Acerca de/Ayuda **no** van a la barra (ya están en el menú de la app).

### 1.5 Temas, tipografía e idioma

**Tema** (`W:Services/TemaApp.cs:30-175`):
- Opciones `Sistema=0` (por defecto), `Claro=1`, `Oscuro=2` (`:33-43`). `Sistema` = `ElementTheme.Default`.
- Persistencia: `%LocalAppData%\Free1X2\tema.json` = `{"tema":"Claro|Oscuro|Sistema"}`; lectura tolerante (ausente/ilegible/valor
  desconocido → `Sistema` + `Log`), escritura **atómica** (`.tmp` + `File.Move(overwrite)`) (`:46-49,65-92,158-175`).
- Se aplica con `RequestedTheme` sobre la raíz del contenido, **en vivo** (`:97-117`); los `ContentDialog` no heredan tema y se les
  pasa `TemaDeElemento` (`W:Services/AppServices.cs:93`).
- Paleta («Índigo & Teal» claro / oscuro) en `W:Themes/Tokens.xaml:14-116`:

| Token | Claro | Oscuro |
|-------|-------|--------|
| Acento (`SystemAccentColor`) | `#3D5A80` | `#818CF8` |
| Fondo / Superficie / Superficie alt. | `#F4F6F8` / `#FFFFFF` / `#E6ECF2` | `#0B1120` / `#0F172A` / `#1E293B` |
| Borde | `#C9D4DF` | `#334155` |
| Texto / Texto secundario | `#293241` / `#5C6B7A` | `#F1F5F9` / `#94A3B8` |
| Éxito / Aviso / Error | `#2E7D32` / `#EE6C4D` / `#C0392B` | `#16A34A` / `#F59E0B` / `#EF4444` |
| Secundario (teal) / Terciario (coral) | `#2A9D8F` / `#EE6C4D` | `#2DD4BF` / `#FB7185` |
| Tinte acento / secundario (opacidad) | 0,12 | 0,18 |

  Signos marcados del boleto en verde `#2E7D32` (hover `#357A38`, pressed `#27692B`) en claro (`:39-44`). Fuente `Segoe UI Variable`
  (`:128-135`); radio de esquina 8 (controles) y 12 (tarjetas) (`:138-140`); densidad compacta (`:152-155`).
- Estilos (`W:Themes/Styles.xaml`): tarjeta `AppCardStyle` (borde 1, radio 12, padding 10,7, sombra `ThemeShadow` Z=16, `:141-153`),
  `SignoToggleButtonStyle` (`:155-170`) y **botón de filtro de 4 estados** `FiltroButton{Inactivo,Neutro,Activo,Error}Style` (`:172-215`):
  Inactivo = superficie neutra; Neutro = filtro presente sin evaluar (ámbar, texto y borde `AppWarning`); Activo = verde con texto
  `AppOnSuccess`; Error = rojo con texto `AppOnError` (equivale a `SetBotonEstado` del MainForm).

**Idioma** (`W:Services/IdiomaApp.cs:30-157`): `Espanol=0` (defecto) / `Ingles=1`; `idioma.json` = `{"idioma":"Espanol|Ingles"}` con las
mismas reglas que el tema; `CodigoBcp47` = "es"/"en"; el override de recursos solo se aplica al **cargar** una página (de ahí la
recarga forzada). **Solo la pantalla Créditos está localizada** (7 recursos en `W:Strings/{es,en-US}/Resources.resw`: `CreditosTituloEquipo`,
`CreditosWebsiteLink` (accesibilidad), `CreditosWebUrl`, `CreditosColaboradores`, `CreditosGraciasNota`, `CreditosGracias`); el resto del texto está
embebido en español (`W:Free1X2.WinUI.csproj:85-93`). La fuente de traducciones completa es `Datos/Idioma/en-US.lang` (→ ESPEC_07).

**Propuesta macOS.** Tema: `@AppStorage("tema")` con `enum Tema { sistema, claro, oscuro }` y
`.preferredColorScheme(nil|.light|.dark)`; **colores en el Asset Catalog** (Color Set con variante Any/Dark: `Fondo`, `Superficie`, `SuperficieAlt`,
`Borde`, `Texto`, `TextoSecundario`, `Exito`, `Aviso`, `Error`, `Secundario`, `Terciario`; el acento = `AccentColor` del catálogo, no el del
sistema salvo decisión contraria). Se respeta que «Sistema» sea el valor por defecto. Idioma: `.xcstrings` (es base + en) con **todas** las cadenas
(hoy solo 7 están localizadas); cambio de idioma en Ajustes (`AppleLanguages` requiere reinicio; alternativa `.environment(\.locale, …)` en vivo sin
perder el estado, evitando la recarga destructiva de WinUI). Mantener `tema.json`/`idioma.json` no es necesario (no hay compatibilidad de fichero
que preservar para preferencias de UI): usar `UserDefaults` (decisión a confirmar, ver «Abiertas»).

### 1.6 Diálogos, picker de ficheros y log

**`AppServices`** (`W:Services/AppServices.cs`):
- `MostrarError(msg)` → diálogo título «Error», botón «Aceptar»; `MostrarInfo(msg)` → título «Free1X2» (`:138-141,188-196`). Ambos **se encolan**
  (`Queue<(titulo,mensaje)>`) y se muestran de uno en uno; sin hilo de UI solo se escribe `Log.Warn` (`:143-162`). Sin `XamlRoot` los mensajes
  esperan en cola (`:170-176`).
- `ConfirmarAsync(msg, titulo="Free1X2", botonPorDefecto=Close)` → Sí/No; devuelve `true` solo con «Sí»; **sin ventana o ante error devuelve `false`**
  (nunca hace nada destructivo sin confirmación explícita) (`:58-114`). Todos los diálogos pasan por una **puerta única** (`SemaphoreSlim(1,1)`) porque
  WinUI solo permite un `ContentDialog` por raíz (`:37,74`).
- `EjecutarObservandoErrores(Func<Task>, contexto)` ejecuta una acción async «dispara y olvida» y, si falla, `Log.Error` + «No se pudo completar la acción:\n\n{msg}» (`:123-135`).

**`PickerHelper`** (`W:Services/PickerHelper.cs:30-94`): `AbrirAsync(ext…)` (un fichero), `AbrirVariosAsync(ext…)`, `GuardarAsync(nombreSugerido, (etiqueta, ext)…)`,
`GuardarAsync(ubicación, …)` (p. ej. `PicturesLibrary` para imágenes), `GuardarConExtensionPorDefectoAsync`. Ubicación inicial siempre `DocumentsLibrary`.
Las extensiones llevan punto (`".txt"`, `".comb"`, `"*"` para todos).

**`Log`** (`W:Services/Log.cs`): `%LocalAppData%\Free1X2\log.txt`, línea `yyyy-MM-dd HH:mm:ss.fffZ [NIVEL] mensaje` (UTC, invariante), niveles ERROR/WARN/INFO;
rota a `log.1.txt` al superar 1 MiB (`:30-36,86-133`); thread-safe (`lock`); **nunca lanza**; las excepciones se aplanan a tipo + mensaje (cadena de `InnerException`
hasta 5 niveles) + pila.

**Propuesta macOS.**
- `AlertaCentral` (`@Observable`): cola de `Alerta { titulo, mensaje, tipo: .error/.info }` presentada con `.alert(item:)` desde la raíz; SwiftUI ya serializa
  alertas, pero se mantiene la **cola** para no perder mensajes. `confirmar(_:titulo:)` con `async -> Bool` mediante `withCheckedContinuation` y `.confirmationDialog`
  (botón por defecto = el no destructivo).
- Ficheros: `fileImporter(allowedContentTypes:)` / `fileExporter` / `NSOpenPanel`/`NSSavePanel` con `allowedFileTypes` por extensión (`.txt`, `.comb`, `.xml`, `.vx2`…
  definidos como `UTType` exportados); directorio inicial = Documentos. Si se adopta App Sandbox (decisión pendiente PLAN §7.3) hay que usar *security-scoped bookmarks*
  para reabrir rutas guardadas (p. ej. `ArchivoFiltroCols`).
- Log: `os.Logger` + fichero rotativo en `~/Library/Logs/Free1X2/` (opcional); mantener formato de línea si se quiere compatibilidad con soporte.

### 1.7 Hooks del dominio (puentes UI ↔ motor)

`CablearHooksDominio` (`W:App.xaml.cs:127-208`) asigna los delegados estáticos de `D:Abstractions/UiHooks.cs`:

| Hook | Contrato (`UiHooks.cs`) | Implementación WinUI | Propuesta Swift |
|------|--------------------------|----------------------|-----------------|
| `UiPump.Pump` | `Action` llamada en bucles largos | **No-op** (el motor corre en `Task.Run`; no se bombea el dispatcher desde hilos de trabajo) `:131` | Eliminar: `async` + `Task.checkCancellation()` + progreso por `AsyncStream` (T12) |
| `UserDialogs.ShowError/ShowInfo` | `Action<string>` | `AppServices.MostrarError/MostrarInfo` `:133-134` | Protocolo `Avisador` inyectado en los casos de uso (`func error(_:)`, `func info(_:)`) |
| `Clipboard.Write/Read` | `Action<string>` / `Func<string>` | `DataPackage.SetText` / `GetTextAsync().GetResult()` (síncrono; error → cadena vacía + `Log.Error`) `:139-162` | `NSPasteboard.general` (`clearContents` + `setString(_,forType:.string)`; `string(forType:)`) tras un protocolo `Portapapeles` |
| `AnalisisUi.MostrarVisor(contenedor, grupo)` | `Action<object,object>` | Encola en el dispatcher: asigna `VisorAnalisisColumnasFrmViewModel.UltimoContenedor/UltimoGrupo` **dentro** del turno de UI (B-07: evita que dos visores seguidos se pisen) y `MainWindow.NavegarA(VisorAnalisisColumnasFrmPage)` `:170-208` | El caso de uso **devuelve** el `ContenedorAnalisis`; la vista navega con `path.append(.visorAnalisis(contenedor, grupo))` |

### 1.8 Smoke test

Gated por la variable de entorno `FREE1X2_SMOKE=1` (inerte sin ella). `IniciarSmokeTest`: ruta `%TEMP%\free1x2_smoke.log`, escribe «SMOKE START»; lista = `MainPage` +
las 108 de `PortedPagesRegistry.All`; un `DispatcherQueueTimer` cada **120 ms** navega a la siguiente (éxito si `Navigate` no lanza); cada fallo añade
`FAIL {tipo}: {excepción}: {mensaje}`; al terminar escribe `SMOKE DONE total={n} ok={n} fail={n}` y `Application.Exit()` (`W:MainWindow.xaml.cs:810-869`).
Esperado: `total=109 ok=109 fail=0` (CLAUDE.md).
**Propuesta macOS:** enum `Pantalla: CaseIterable` con todas las vistas; argumento de lanzamiento `-smoke` que recorre `allCases` instanciando cada vista y escribe el mismo
formato de log; verificación equivalente en CI con `xcodebuild test` (UI test que recorre la barra lateral).

### 1.9 Controles reutilizables

| Control WinUI | Qué muestra | Entradas (propiedades) | Eventos / efectos | Usado en | Propuesta SwiftUI |
|---------------|-------------|------------------------|-------------------|----------|--------------------|
| `BoletoBaseControl` + `BoletoBaseViewModel` (`W:Controls/BoletoBaseControl.xaml`, `BoletoBaseViewModel.cs:181-379`) | Tarjeta «Boleto Base» editable: cabecera con título (rojo), resumen «Fijos: N - Dobles: N - Triples: N» y botón reiniciar; `NumeroPartidos` filas (nº `00`, combo editable Local, combo editable Visitante, 3 `ToggleButton` 1/X/2). | `Partidos[i].Local/Visitante/Equipos` (formato «LOCAL - VISITANTE», placeholder «? - ?»), `Signo1/SignoX/Signo2` (defecto `true,true,true` = triple). `Marcados` = nº de signos (1=fijo, 2=doble, 3=triple). | Cada cambio de signo: `VolcarPronosticosAlMotor()` (`analizador.SetPronostico(i, "1,X,2")` con los marcados en orden 1,X,2 separados por coma, `Pronostico` `:142-152`) + recalcular resumen. `CargarDesdeMotor` lee `analizador.Pronosticos[i]` (vacío → «1,X,2»; usa `Contains("1")` etc.) `:233-258`. `DevolverEquipos()` normaliza (recorta cada parte; «? - ?» si ambas vacías) `:120-124,280-288`. **El setter de Local/Visitante no recorta** para permitir escribir «Real Madrid» (B-03) `:46-67`. `Reiniciar` → 14 triples `:343-361`. | `MainPage` (`W:Views/MainPage.xaml`) | `BoletoBaseView(modelo: BoletoBase)`: `Table`/`Grid` con `TextField` + `Toggle(.button)`. Regla: recortar solo al volcar al motor/disco. `Local`/`Visitante` con sugerencias (`.searchSuggestions`) desde el catálogo de equipos. |
| `BoletoControl` + `BoletoViewModel` (`W:Controls/BoletoControl.xaml`, `BoletoViewModel.cs:63-171`) | Lista de 14 partidos con equipos de muestra (Madrid–Barcelona, … Eibar–Huesca, `:147-163`) y signos; botón Limpiar; si hay `JornadaActual` pisa los equipos con los reales (`AplicarJornadaActual` `:96-107`) y se suscribe a `JornadaCambiada` (`:85`). | `Partidos` (14 fijos: `NumPartidos=14`, `:65`) | `Limpiar` desmarca todo; resumen «Fijos: n · Dobles: n · Triples: n». | Solo `BoletoFrmPage` | Datos de muestra **no** deben portarse; usar el modelo real. |
| `BoletoMatrizControl` + `BoletoMatrizViewModel` (`W:Controls/BoletoMatrizViewModel.cs:17-211`, `BoletoMatrizControl.xaml`) | **Visor** de solo lectura del boleto oficial: 8 columnas × 16 apuestas; cada apuesta = 3 ranuras (1, X, 2), solo la signada muestra el texto; cabecera = nº de columna en la combinación; pie = aciertos. | `Llenar(signos[8], numerosColumna?[8], aciertos?[8])`. Cadena vacía → columna inexistente (nº 0, filas ocultas). | `ColumnaBoletoViewModel.Llenar(signos, num, aciertos)`: limpia; para j<16: si `j < len(signos)` marca `signo=='1'`/`'X'|'x'`/`'2'` y fila visible, si no **fila oculta** `:112-144`. Aciertos: texto vacío si ≤0; **resaltado si ≥ 10** `:94-105`. Separación extra de 3 px **tras los partidos 4, 8, 11 y 14** (agrupamiento clásico) `:25-28`. `Limpiar()` `:203-210`. | `VerBoletosPage`, `BoletoFrmPage` | `BoletoMatrizView`: `HStack` de 8 `ColumnaBoletoView` con `ScrollView(.horizontal)`; constantes `numApuestas=16`, `numColumnas=8`. Cabecera con el acento, número de columna. |
| `PorcentajesControl` + `FilaPorcentaje` + `PorcentajesHelper` (`W:Controls/PorcentajesControl.xaml`, `FilaPorcentaje.cs`, `PorcentajesHelper.cs`) | Rejilla editable «Part. / 1 / X / 2» con una fila por partido; altura máx. 260 con scroll interno (`PorcentajesControl.xaml:12`). `NumberBox` con `Minimum=0`, sin spin, `SmallChange=1` (**sin máximo**) `PorcentajesControl.xaml:47-52`. | `Filas: ObservableCollection<FilaPorcentaje>` (`Partido:int`, `Uno/Equis/Dos: double`). | `PorcentajesHelper.Crear(n)` (n filas a 0), `AMatriz(filas)` → `double[N,3]` (col 0=1, 1=X, 2=2), `CargarMatriz(destino, m)` (reemplaza; partido = i+1) `PorcentajesHelper.cs:15-41`. Es el contrato `valors.RetVals()`/`ControlPorcentajes.Valores` del WinForms. | 14 pantallas (Valoración, Ordenar por probabilidad, Rentabilidad, SelecJM, Tramificar, Banco de pruebas, Estimador, Formatos123, FiltroPorcenJB, GuardarValoración, HistoriaValoraciones, BuscaLims, AnalisisFormatos123, aidomnou) | `PorcentajesGrid(valores: Binding<[[Double]]>)` con `TextField(value:format:)`; `Double` (no `Float`: aquí es `double`); matriz `[[Double]]` Nx3. |
| `SemaforoControl` (`W:Controls/SemaforoControl.xaml(.cs)`) | Punto de 14 px + halo de 22 px en un lienzo 24×24. Estados `EstadoSemaforo { Neutro, Verde, Rojo }` (`:12-20`). | `Estado` (defecto `Neutro`). | Neutro: relleno `AppBorder`, opacidad 0,55, sin halo. Verde: `AppSuccess`, opacidad 1, halo 0,35. Rojo: `AppError`, igual. Nombre accesible: «Filtro vacío» / «Filtro activo» / «Filtro con error» `:50-55,63-91`. | `MainPage`, `ContactosFrmPage`, `AnalizarCombinacionFrmPage` | `SemaforoView(estado:)` con `Circle` + `.accessibilityLabel`. |
| `GraficoLineasControl` + `CurvaGrafico` (`W:Controls/GraficoLineasControl.xaml.cs:42-346`) | Lienzo con rejilla, ejes, 4 etiquetas de extremos y una polilínea por curva. | `Curvas: ObservableCollection<CurvaGrafico(Nombre, Color, [(x,y)])>`; `IntervaloGrid` (defecto 50 px, mín. efectivo >4, `:130,215`). | Márgenes izq 44, inf 24, sup 12, der 12 (`:49-52`). Escala común: **min/max de todos los puntos de todas las curvas**; si `xMax-xMin<1e-9` fuerza `xMax=xMin+1` (idem Y) `:203-204`; sin datos: ejes 0..1. `px = 44 + (x-xMin)/(xMax-xMin)*anchoÚtil`; `py = 12 + (yMax-y)/(yMax-yMin)*altoÚtil` (Y invertida) `:271-275`. Marcadores (elipse 5 px) solo si la curva tiene **≤ 64 puntos** `:280`. Etiquetas: entero si `|v-round(v)|<1e-6`, si no `"0.##"` (¡cultura actual!) `:316-317`. Polilínea recta (el legacy usaba `DrawCurve` tensión 0,1). | `TramificarGraficasFrmPage` | **Swift Charts**: `Chart { ForEach(curvas) { LineMark(x,y).foregroundStyle(color) ; PointMark si ≤64 } }` con `.chartXScale/.chartYScale` calculados con la misma regla; formateo con `Locale` explícito. |
| `MatrizAnalisisView` + `MatrizAnalisis/FilaMatrizAnalisis/CeldaAnalisis/ColumnaValoracion/EntradaValorAnalisis` (`W:Controls/Analisis/*`) | Rejilla de análisis: por fila una etiqueta (ancho 150) + celdas de 34 px en fuente monoespaciada; celdas de cabecera con fondo `AppSurfaceAlt`. | `Matriz: MatrizAnalisis(Filas[])`; `FilaMatrizAnalisis(Etiqueta, Celdas, EsCabecera)`; `CeldaAnalisis(Valor:String, EsCabecera)`. `ColumnaValoracion(Titulo, Entradas[(Clave, Columnas)])` solo con entradas de columnas ≠ 0. | Solo presentación: **todos los valores vienen del `ContenedorAnalisis` del dominio, nunca se fabrican** (`ModelosAnalisis.cs:6-15`). Reproduce los `CtrlAnalisis*` del WinForms (VX2, SSeguidos, Interrupciones, Contactos, Pesos, Distancias, Simetrías, Diferencias, CPs, Formatos). | `VisorAnalisisColumnasFrmPage` | `Grid`/`Table` con `Text(.monospacedDigit())`; modelo `MatrizAnalisis` como `struct` en Core (formateado a `String` ya hecho por el caso de uso). |
| `ToolbarWrapPanel` / `CardElevation` | Layout de la barra con salto de línea / sombra de tarjeta. | — | Específicos de WinUI. | `MainWindow` / estilos | No se portan (toolbar nativa, `GroupBox`/`.background(.regularMaterial)`). |

### 1.10 Patrón común de las pantallas de condición («diálogo de filtro»)

Casi todas las pantallas de **condición** (NoVariantes, PesosNum, SignosSeguidos, Simetrías, Valoración, Parejas, Trios, StaInter, StaSS, VSignos, …) comparten estructura y botones **Aceptar · Estadísticas · Guardar · Abrir · Copiar · Pegar · Borrar · Cancelar**
(equivalen al `menuCondiciones` del WinForms, `P:NoVariantesFrmPage.xaml` sección «Acciones»). Las 4 últimas de fichero están en la base
`FiltroArchivoViewModelBase` (`P:_Helpers/FiltroArchivoViewModelBase.cs:30-115`):

- **Guardar**: picker de guardado (`NombreSugerido`, `TiposGuardar`); `GuardarEn(ruta)` del VM; excepción → «No se pudo guardar: {msg}».
- **Abrir**: picker (`ExtensionesAbrir`); `AbrirDesde(ruta)`; excepción → «No se pudo abrir: {msg}».
- **Copiar**: crea la carpeta de `RutaTemporal` y `GuardarEn(RutaTemporal)` (el «portapapeles» de condiciones es un **fichero** en `Temp/tmp.<ext>`); excepción → «No se pudo copiar: {msg}».
- **Pegar**: si existe `RutaTemporal`, `AbrirDesde(RutaTemporal)`; si no, **nada** (sin mensaje; **sin try/catch**).
- **Aceptar** (patrón): leer `AppState.GrupoEnEdicion`; si es `null` → volver; `filtro = (FiltroX)grupo.GetFiltro(Filtro.X.ToString())`; `filtro.ReinicializaValores()`; si hay datos → `IsActive = true` (solo si antes no tenía datos), `ContieneDatos = true`, setear valores; si no → `IsActive=false`, `ContieneDatos=false`; `grupo.ActivaFiltro(filtro)`; `AppState.NotificarCambio()`; volver.
- **Estadísticas**: construye un **filtro temporal** con lo de pantalla (misma lógica que Aceptar pero sobre `new FiltroX()`), `CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, <Ganadoras/>)` → `VisorEstadisticasViewModel.UltimasEstadisticas = lista` → navega a `VisorEstadisticasPage`. Al volver (`NavigationMode.Back`) **no** se recarga desde el grupo para no perder lo editado.
- **Guardar/Abrir condición** usan `ArchivoCondiciones { NombreArchivo }.GuardaArchivo(filtroTemp)` / `AbrirArchivoCombinacion(ruta)` + `LeeCondicion()` → `Grupo` (formatos → ESPEC_07).

**Cómo se abre una pantalla de condición.** Desde la rejilla de 14 condiciones de `MainPage` (izquierda: Variantes X y 2, Signos Seguidos, Dibujos,
Interrupciones, Grupos Equipos, Formatos, Simetrías; derecha: Pesos Numéricos, Valoración, Columnas Probables, Distancias, Contactos, Formatos 123,
Diferencias; `W:Views/MainPageViewModel.cs:155-196`). `AbrirCondicion` hace `AppState.GrupoEnEdicion = AppState.GrupoActual` y navega (`:290-297`);
If-Then, Control de grupos y Tolerancias abren igual (`:299-326`). **Semáforo de cada botón** (`CondicionItem.RefrescarDesdeGrupo`, `W:Views/CondicionItem.cs:61-80`):
`filtro.IsActive` → Verde; si no, `filtro.ContieneDatos` → Rojo (tiene datos pero está desactivada); si no → Neutro. Control de grupos: Verde si
`CtrlGrupos.ControlesGrupos.Count > 1`; If-Then: Verde si activo, Rojo si no vacío e inactivo, Neutro si vacío (`W:Views/MainPageViewModel.cs:228-241`).

**Propuesta macOS.** `protocol EditorCondicion { associatedtype Filtro; var borrador: Borrador {get set}; func aceptar(en grupo: inout Grupo) ; func filtroTemporal() -> Filtro; func estadisticas() async -> [Estadistica] }`
y una `CondicionSheet` genérica que aporta los 8 botones (Aceptar/Cancelar en la barra de la hoja; Guardar/Abrir/Copiar/Pegar en un menú «···»; Borrar; Estadísticas abre un inspector).
**Copiar/Pegar**: usar `NSPasteboard` con un tipo propio (`com.free1x2.condicion`) en lugar del fichero `Temp/tmp.ext` (la compatibilidad de ese fichero temporal no es un requisito: se regenera en cada Copiar).

---

### 1.11 Páginas sin ViewModel: Acerca de, Ayuda y Créditos

Son las **tres páginas de `Views/Ported/` sin `*ViewModel.cs`** (por eso la lista 74–108 de los ViewModels solo llega a 105: hay 105 ViewModels para 108 páginas). Son estáticas, sin entradas ni estado, y la lógica vive en el code-behind. Las tres están en `PortedPages` (`W:Navigation/PortedPages.cs:124-126`, categoría «Ayuda»).

| Página | Cómo se abre | Contenido y comportamiento |
|--------|--------------|----------------------------|
| **`AcercaDeFrmPage`** «Acerca de» | Menú Free1x2 ▸ «Acerca de…» (`W:MainWindow.xaml.cs:171`) y botón «Acerca de ...» (`:358`) | Cabecera «Acerca de» + lema «El primer programa de quinielas libre y gratuito» (`P:AcercaDeFrmPage.xaml:17-18`); tarjeta con logo (`Assets/logo.jpg`, JPEG 110×110 extraído del `.resx` legacy, `P:AcercaDeFrmPage.xaml.cs:41-47`), «Free1X2», «Versión {v} Rarotonga» y enlace «www.clubprogol.com» (`:45-52`); tarjeta «Enlaces» con 6 `HyperlinkButton` (`:69-99`) |
| **`AyudaFrmPage`** «Recursos de ayuda» | Tecla **F1** (`W:MainWindow.xaml.cs:771-772`) y botón «Ayuda» (`:357`); **no** hay entrada de menú | Tarjetas «Documentación» (Manual, Artículos, FAQ, Recursos) y «Comunidad» (Foro de Free1X2, Notificaciones, Free1X2 en Facebook) (`P:AyudaFrmPage.xaml:27-76`) |
| **`CreditosFrmPage`** «Créditos» | Menú Free1x2 ▸ «Créditos…» (`W:MainWindow.xaml.cs:172`) y enlace «Créditos» de Acerca de (`P:AcercaDeFrmPage.xaml.cs:133-139`, `Frame.Navigate`) | Cabecera «Equipo Free1X2» + `www.clubprogol.com` (enlace, `P:CreditosFrmPage.xaml.cs:45-49`), tarjeta «Colaboradores» (9 entradas), nota de agradecimiento (`P:CreditosFrmPage.xaml:19-85`) |

**Acciones de «Acerca de» (`P:AcercaDeFrmPage.xaml.cs`).**
- *Versión* (`VersionProducto` `:66-95`): `"Versión " + v + " Rarotonga"` (la palabra «Rarotonga» es literal, `:39`). `v` se resuelve en cascada: `AssemblyInformationalVersion` recortado antes de un `+` (`<Version>` del csproj, hoy `0.83.0`, `W:Free1X2.WinUI.csproj:15-17`) → `FileVersionInfo.ProductVersion` del .exe → versión de 4 partes del ensamblado → «(desconocida)» (nunca un literal). En macOS: `CFBundleShortVersionString` del `Info.plist`.
- *Enlaces externos* (`:22-25`): «Descarga» `https://clubprogol.com/Descargas/Descargas.aspx`, «Foro sobre el programa» `https://clubprogol.com/foros/index.php`, «www.clubprogol.com» `https://clubprogol.com`, «Manual» `https://clubprogol.com/DocWK`; se abren con `Launcher.LaunchUriAsync` (`:141-147`). Mac: `NSWorkspace.shared.open`.
- *Documentos locales*: «Información de licencia» → `Documentacion/licencia.txt` y «Sobre el software libre» → `Documentacion/GPL.txt` (ambos existen en `W:Datos/Documentacion/` y se copian junto al exe por `W:Free1X2.WinUI.csproj:78`); se abren con la app asociada y, si falta el fichero o falla, se abre `https://clubprogol.com` (`:97-111,155-174`). Mac: recursos del bundle (`Bundle.main.url`) abiertos con `NSWorkspace`.
- «Créditos» navega a `CreditosFrmPage` (no es un diálogo modal como en el WinForms).

**Acciones de «Ayuda» (`P:AyudaFrmPage.xaml.cs`).** «FAQ» abre `https://clubprogol.com/DocWK/index.php?title=FAQ` (`:19,32-36`). **Los otros cinco enlaces no abren nada**: muestran un aviso informativo (`AppServices.MostrarInfo`) con textos fijos **en inglés**: «Online help disabled for offline operation.» + salto de línea + «Please refer to local documentation.» (Manual, Artículos, Recursos, Foro y Facebook, `:23-24,38-72`) o «Notifications system disabled for performance.» (Notificaciones, `:25,62-66`). Es el comportamiento heredado del WinForms (comentarios `:21-22,40-54`); ver «Abiertas».

**Créditos (`P:CreditosFrmPage.xaml.cs:24-38`).** Lista fija, en este orden: Luis Fernández, Morrison (sin contacto), Toni Moreno, Joan Duatis, Indeciso, JVallespin, Xfsf, ArDoC y Juan Tejada. Los contactos de terceros son correos **ofuscados** (`usuario [at] dominio [dot] tld`) mostrados como **texto plano no clicable**; solo el propietario del repositorio es un enlace `mailto:` funcional (`:26-37,88-89`). La nota final y los títulos usan recursos localizados (`x:Uid`: `CreditosTituloEquipo`, `CreditosWebsiteLink`, `CreditosWebUrl`, `CreditosColaboradores`, `CreditosGraciasNota`, `CreditosGracias`; `P:CreditosFrmPage.xaml:21-85`): es/en en `W:Strings/{es,en-US}/Resources.resw` («Equipo Free1X2» / «Free1X2 Team», «Colaboradores» / «Contributors», «¡Gracias!» / «Thank you!»…). La lista de nombres y el resto de páginas **no** están localizados (§1.5).

**Propuesta macOS.** Acerca de → panel estándar «Acerca de Free1X2» (`NSApplication.orderFrontStandardAboutPanel` con `credits` en RTF, logo y versión del bundle) más una hoja «Créditos y licencia» con los enlaces a documentos; Ayuda → menú **Ayuda** nativo (con `⌘?`/`F1`) que abre el manual local (`docs/MANUAL_USUARIO.md` empaquetado como Help Book o PDF) y la FAQ; sustituir los 5 avisos «disabled for offline operation» por enlaces reales o por la entrada «Ayuda de Free1X2» (decisión del dueño). Créditos con `.xcstrings` para es/en.
**Evidencia.** `P:AcercaDeFrmPage.xaml.cs:19-175`, `P:AcercaDeFrmPage.xaml:15-99`, `P:AyudaFrmPage.xaml.cs:16-81`, `P:AyudaFrmPage.xaml:15-76`, `P:CreditosFrmPage.xaml.cs:21-114`, `P:CreditosFrmPage.xaml:19-85`, `W:Strings/es/Resources.resw:68-90`, `W:Strings/en-US/Resources.resw:68-90`, `W:Navigation/PortedPages.cs:124-126`.

---

## 2. Pantallas 74–105 (ViewModels)

**Plantilla por pantalla**: Nombre y propósito · Cómo se abre · Entradas · Acciones · **Lógica en el ViewModel (`CU-nn`)** · Salidas y formato · Área Mac propuesta · Evidencia.
Áreas Mac: Boleto · Condiciones y filtros · Cálculo · Reducción · Escrutinio · Análisis · Columnas probables · Herramientas · Online · Ajustes.
Tipo: *vista principal* (ocupa el detalle), *hoja modal* (`.sheet`), *inspector* (`.inspector`).

### 74. `MultiplicadorFrm` — Multiplicador

**Propósito.** Producto cartesiano de dos ficheros de columnas de 14 signos: concatena cada columna de la Entrada 1 con cada una de la Entrada 2 (28 cifras) y reordena según una plantilla de 14 índices (1..28), lo que permite transponer a la vez. **Se abre** desde Operaciones ▸ Multiplicador… o botón «Multiplicación» (`W:MainWindow.xaml.cs:246,382`).

**Entradas** (`P:MultiplicadorFrmPage.xaml`):

| Control | Tipo / rango | Defecto | Notas |
|---------|--------------|---------|-------|
| «Seleccionar archivo…» (Entrada Comb-1, «columnas numeradas de 1 a 14») `:45-50` | picker `.txt` | «(selecciona)», 0 columnas | Carga en segundo plano (`Task.Run`); deshabilita Seleccionar/Multiplicar mientras tanto. |
| «Seleccionar archivo…» (Entrada Comb-2, «de 15 a 28») `:87-92` | picker `.txt` | idem | |
| 14 `NumberBox` «Índice de plantilla 1..14» `:141-197` | entero, `Minimum=1`, `Maximum=28`, spin compacto | 1, 2, 3, 4, 5, 15, 16, 17, 18, 19, 20, 21, **13, 13** (`P:MultiplicadorFrmViewModel.cs:37-64`) | `Value` es `double`; el VM lo trunca con `(int)`. |
| «Multiplicar» (accent) `:237-242`, «Grabar resultado» `:232-236` | botones | Grabar deshabilitado hasta que haya resultado | |

**Validaciones y mensajes.** Cualquier índice fuera de 1..28 → `Mensaje="error en plantilla"` **y** diálogo de error con el mismo texto; no se calcula (`:161-169`). No se comprueba que haya entradas cargadas (con 0 columnas el resultado es 0 y Grabar sigue deshabilitado). Excepciones en cálculo/grabado → `Mensaje="Error: {msg}"` (en rojo).

**Acciones paso a paso.**
1. *Seleccionar Entrada n*: picker `.txt` → `CargarColumnas(ruta, lista)` (en `Task.Run`) → muestra solo el nombre de fichero y `Columnas: {n}` (`:103-145`).
2. *Multiplicar*: valida plantilla → `Mensaje=""` → calcula en `Task.Run` → `ColumnasResultadoTexto = n`, `PuedeGrabar = n>0` (`:151-204`).
3. *Grabar resultado*: picker «Fichero resultados» `.txt` (nombre sugerido «Resultado») → escribe una línea por columna (`:210-238`) → `Mensaje="Resultado grabado."`.

**Lógica en el ViewModel.** No llama al motor (`Free1X2.Domain`); todo vive aquí.

*CU-C01 `MultiplicarColumnas`* (`P:MultiplicadorFrmViewModel.cs:152-204,241-250`):
```
cargar(ruta): para cada línea (mientras haya caracteres por leer):
    col = (línea + "11111111111111").prefix(14).trim()   // línea vacía → 14 unos; línea corta se rellena con '1'
multiplicar(e1, e2, indices[14]):                        // indices en 1...28 (validado antes)
    para c1 en e1 (orden de fichero):
      para c2 en e2:
        s = c1 + c2                                      // 28 caracteres
        salida.append( String( (0..<14).map { s[indices[$0]-1] } ) )
```
Orden de salida: **E1 externo, E2 interno** (no ordena ni deduplica). Resultado: `[String]` de 14 caracteres.

**Peculiaridades a preservar / trampas.**
- `Trim()` se aplica *después* del `Substring(0,14)`: una línea con espacios o tabuladores dentro de los 14 primeros caracteres queda más corta, y luego `scol3[indices-1]` puede salirse de rango → `IndexOutOfRangeException` capturada como «Error: …» (`:247-248`). Replicar (o validar y avisar) pero **sin** `fatalError` en Swift: usar acceso seguro.
- `(int)Indice` con `NaN` (NumberBox vacío) da un valor fuera de 1..28 en C# → «error en plantilla»; en Swift `Int(Double.nan)` **aborta**: convertir con `Int(exactly:)` y tratar `nil` como inválido.
- La lectura usa `sr.Peek() > 0` (para en EOF) y la escritura `StreamWriter` UTF-8 sin BOM con `WriteLine` (T8): en Mac leer `\r\n` y `\n`; escribir `\n` (decidir y documentar: compatibilidad con Windows).
- Defecto de plantilla 13 y 13 en las posiciones 13 y 14: es el valor actual (no un error de transcripción de esta espec.).

**Salidas.** Contadores de columnas como enteros sin formato (`Count.ToString()`); fichero de texto con una columna por línea.
**Área Mac propuesta.** **Herramientas** · vista principal (operación sobre ficheros) con previsualización de las primeras N columnas resultantes y progreso cancelable; la plantilla como 14 `Stepper`/`TextField` numéricos.
**Evidencia.** `P:MultiplicadorFrmViewModel.cs:21-257`, `P:MultiplicadorFrmPage.xaml:1-246`, `P:MultiplicadorFrmPage.xaml.cs:1-22`.

---

### 75. `NoVariantesFrm` — Número de Variantes (condición «Variantes, X y 2»)

**Propósito.** Condición que limita las **cantidades admitidas** de variantes (X+2), de signos X y de signos 2 por columna. Escribe sobre `FiltroNoVariantes` del grupo en edición. **Se abre** desde el botón «Variantes X y 2» de `MainPage` (`W:Views/MainPageViewModel.cs:158`), patrón §1.10.

**Entradas** (`P:NoVariantesFrmPage.xaml`): 3 `TextBox` de texto libre — «Var» (`:34`), «X» (`:49`), «2» (`:64`) — con *placeholder* «p. ej. 0 1 2», cabecera «Cantidades admitidas (0–15)» y ayuda «Separa varios valores con comas o espacios» (`:20-21`). Defecto: vacío. Botones: Aceptar, Estadísticas, Guardar, Abrir, Copiar, Pegar, Borrar, Cancelar (`:77-99`).

**Acciones.**
- *Al entrar* (no al volver de Estadísticas): `CargarDesdeGrupo`: `filtro = (FiltroNoVariantes)AppState.GrupoEnEdicion.GetFiltro("NoVariantes")`; `Variantes=filtro.GetVariantes()`, `Equis=GetEquis()`, `Doses=GetDoses()` (`P:NoVariantesFrmViewModel.cs:70-79`, `P:NoVariantesFrmPage.xaml.cs:24-33`).
- *Aceptar*, *Estadísticas*, *Borrar* (vacía los tres textos, `:123-130`), *Cancelar* (vuelve sin aplicar, `:116-121`), y el cuarteto de fichero (`§1.10`).
- Guardar: nombre sugerido «CantidadSignos»; tipos «Cantidad de signos V, X y 2» `.vx2` y `(XML)` `.xml`; Abrir acepta `.vx2`, `.xml`; temporal `Temp/tmp.vx2` (`:47-54`).

**Lógica en el ViewModel.**

*CU-C02 `AplicarCondicionNoVariantes`* (`P:NoVariantesFrmViewModel.cs:82-114`, mismo patrón en `ObtenerFiltroTemporal` `:136-159`):
```
todos = UtilidadesEntradasValores.ObtenerTodosValores()      // "0,1,...,NumeroPartidos" (D:Utils/UtilidadesEntradasValores.cs:322-330)
filtro.ReinicializaValores()                                 // los 3 bool[N+1] a false y las 3 tolerancias a ' '
hayDatos = !blank(Variantes) || !blank(Equis) || !blank(Doses)
si hayDatos:
    si !filtro.ContieneDatos { filtro.IsActive = true }      // solo se activa la 1ª vez que tiene datos
    filtro.ContieneDatos = true
    filtro.SetNoVariantes( blank(Variantes) ? todos : Variantes )   // un campo vacío = "todos los valores admitidos"
    filtro.SetNoEquis(     blank(Equis)     ? todos : Equis )
    filtro.SetNoDoses(     blank(Doses)     ? todos : Doses )
si no: filtro.IsActive = false; filtro.ContieneDatos = false
grupo.ActivaFiltro(filtro); AppState.NotificarCambio(); volver
```
Nota: `ReinicializaValores` borra antes, de modo que al reaceptar con datos **conserva** el estado `IsActive` previo del filtro (no lo reactiva si el usuario lo había desactivado desde Inicio).

*CU-C03 `EstadisticasDeFiltro`* (`:192-201`): `filtroTemp = ObtenerFiltroTemporal()`; `lista = CalculadorEstadisticas().EstadisticasFiltro(filtroTemp, <AppDir>/Ganadoras/)`; entrega `lista` al visor (pantalla 103). Genérico para todas las condiciones.

*CU-C04 `GuardarCondicion` / `AbrirCondicion`* (`:170-187`): `ArchivoCondiciones{NombreArchivo}.GuardaArchivo(filtroTemp)`; abrir: si `AbrirArchivoCombinacion(ruta)` devuelve true → `g = LeeCondicion()`; `MarcarValores((FiltroNoVariantes)g.GetFiltro("NoVariantes"))` (pone los tres textos). Si devuelve false, **no hace nada ni avisa**.

**Formato de entrada realmente aceptado (importante).** El VM pasa el texto crudo a `FiltroNoVariantes.SetNoXxx(string)` (`D:MotorCalculo/FiltroNoVariantes.cs:231-251,270-290,309-329`), que **divide solo por `,`**; cada token es `N` o `N<letra>` (la letra final = **grupo de tolerancia**, ver pantalla `ControlTol`), con `N` en `0…NumeroPartidos` (arrays de tamaño `NumeroPartidos+1`, `:41-54`). `GetVariantes()` devuelve `"2,3a,5"` (valores activos en orden ascendente, letra de tolerancia pegada, `:335-356`).
⇒ La indicación de la UI «comas **o espacios**» y el *placeholder* «0 1 2» **no son ciertos**: un token con espacios hace que `Convert.ToInt32` lance `FormatException`, y un valor > `NumeroPartidos` lanza `IndexOutOfRange`; el VM no captura (excepción al manejador global de la app, §1.1). En Swift: parsear con un tokenizador tolerante (`,`/espacios) **o** editor de chips con selector de tolerancia, devolviendo error de validación en lugar de abortar; documentar la decisión (preservar solo si el dueño lo exige).

**Salidas.** Tres textos con formato `"v,v,v"` (+ letra de tolerancia). Estado del semáforo en Inicio actualizado vía `AppState.Cambiado`.
**Área Mac propuesta.** **Condiciones y filtros** · hoja modal sobre el boleto (editor de condición genérico §1.10) con chips numéricos 0…14 y picker de tolerancia.
**Evidencia.** `P:NoVariantesFrmViewModel.cs:26-202`, `P:NoVariantesFrmPage.xaml:12-99`, `P:NoVariantesFrmPage.xaml.cs:13-34`, `P:_Helpers/FiltroArchivoViewModelBase.cs:30-115`.

---

### 76. `OrdenarPorProbabilidadFrm` — Ordenación de columnas por probabilidad

**Propósito.** Ordena las 4 782 969 columnas («14 triples») o las de un fichero por **proximidad a un valor central** (probabilidad en logaritmo neperiano, o suma de valoraciones) y escribe las primeras N a un fichero, opcionalmente con probabilidad acumulada y premio de 14. **Se abre** desde Utilidades ▸ Ordenar por probabilidad… / botón «Ordenar por probabilidad» (`W:MainWindow.xaml.cs:258,421`).

**Entradas** (`P:OrdenarPorProbabilidadFrmPage.xaml`):

| Control | Tipo / rango | Defecto (VM) |
|---------|--------------|--------------|
| Radio «14 Triples» / «Fichero» `:32,37` | exclusivos | 14 Triples |
| «Seleccionar archivo…» entrada `:43` (solo habilitado con Fichero) | picker `.txt` | «(falta selección)» |
| «Seleccionar archivo…» salida `:60` | picker guardar `.txt` (sugerido «ColumnasOrdenadas», tipo «Columnas») | «(falta selección)» |
| «Nº máximo de columnas» `:68` | `NumberBox` ≥ 1 | 4 782 969 (`VM:59`) |
| «Añadir probabilidad acumulada» `:74` / «Límite de la prob. acumulada» `:78` (≥ 0) | check / número | no / 1 (`VM:63,67`) |
| «Añadir Premio de 14 aciertos» `:84` | check | no |
| Rejilla de valoraciones (`PorcentajesControl`, §1.9) `:97` | `NumeroPartidos` filas × (1,X,2) `double` ≥ 0 | todo 0 (`PorcentajesHelper.Crear`, `VM:188-189`) |
| Pestañas (`Pivot`) `:107`: 0 «Por productos», 1 «Por Sumas», 2 «Multiple» | índice | 0 |
| Pestaña 0 — radios «Nº acertantes» / «Premio» / «Probabilidad» / «Log. neperiano» / «Columna» `:129-171` (solo el `TextBox` del radio activo está habilitado) | texto | acertantes `"1,5"`, resto vacío |
| …Columna: `TextBox` mayúsculas, `MaxLength=14` `:171`; combo de aciertos (solo con LN) `:161` con `0,10,11,12,13,14` (def. `14`) | | el combo **no se usa** en ningún cálculo (`VM:129-135`) |
| …Configuración L.A.E. `:181-187`: Recaudación, Precio de una apuesta, % para los 14 aciertos | texto | `VariablesGlobales.Recaudacion/PrecioApuesta/Porcentaje14` convertidos con `ToString()` (cultura actual, `VM:195-197`; el valor de campo por defecto sería `"15000000"`, `"0,5"`, `"15"`) |
| Pestaña 1 — «Valor central de ordenación» `:197` | número | 0 |
| Pestaña 2 — radios «Por probabilidad (LN)» / «Por sumas» `:214-216`; «LN mínimo» `:231`, «LN máximo» `:235`, «Nº puntos centrales» (≥ 1) `:239`, «Nº columnas por punto» (≥ 0) `:243` | | LN por probabilidad; 0; 0; 1; 0 |
| Botones «Ordenar» `:277`, «Cancelar» `:273` (→ `Frame.GoBack`, `P:…Page.xaml.cs:22-26`) | | |

**Estados / mensajes** (barra `Estado`): «Faltan datos» (inicial y si falta salida o —sin 14 triples— entrada), «Preparado», «{i} Calculando probabilidades ...», «{i} Ordenando apuestas ...», «{i} Guardando apuestas ...» (el prefijo `{i} ` solo en modo Multiple), «Finalizado {c} columnas», «Finalizado {c} columnas Pr. acum. ={100·acum}» (solo pestaña 0), «Error: {msg}», «Los LN deben ser valores negativos» (`VM:253-258,298,321,472,591-638,769-773`). `Ordenar` está deshabilitado mientras `Calculando`.

**Acciones.** *Ordenar* (`VM:250-304`): comprueba ficheros; toma una *instantánea* de L.A.E. con `TryParseEsp` (fallo → 0), `MaxColumnasMotor = min(Int(MaxColumnas), 4 782 969)`; calcula el valor central (CU-C05); ejecuta en segundo plano según la pestaña: 0→`OrdenaPorProductos`, 1→`OrdenaPorSumas`, 2→`OrdenacionMultiple`.

**Lógica en el ViewModel** (transcrita «literalmente» de `OrdenarPorProbabilidadFrm.cs`, `VM:31-33`; opera sobre un array de 4 782 969 `ApuestaProbableCentral {Probabilidad: Float, ProbabilidadDiferencial: Float, Columna: Int32}` = *struct* `D:Analisis/ApuestaProbableCentral.cs:24-29`).

*CU-C05 `ValorCentralDeOrdenacion`* (`VM:367-434`) — según el radio activo (todos con `precio, pct, recaud` ya parseados; `TryParseEsp(t) = Double.parse(t.trim().replacing(".", ","), es-ES, NumberStyles.Any)`, vacío/inválido → no cambia el campo anterior):
```
columna : P = Π_i  (c[i]=='1' ? v[i,0] : c[i]=='2' ? v[i,2] : v[i,1]) / 100        // cualquier otro char cuenta como X
          LN = ln(P); Premio = precio*pct/P/100; Acertantes = recaud/precio*P
LN      : LN = parse(Ln); P = exp(LN);   Premio = precio*pct/P/100; Acertantes = recaud/precio*P
Prob.   : P  = parse(Probabilidad); LN = ln(P); Premio = precio*pct/P/100; Acertantes = recaud/precio*P
Premio  : Premio = parse(...); P = precio*pct/Premio/100; LN = ln(P); Acertantes = recaud/precio*P
Acertantes (defecto): A = parse(...); Premio = recaud*pct/A/100; P = precio*pct/Premio/100; LN = ln(P)
```
Todo en `Double`. Efecto: si el texto no parsea, se arrastra el valor anterior (0 en la primera ejecución → `Infinity`/`NaN` sin protección).

*CU-C06 `OrdenarPorProductosOSumas`* (`VM:521-644,656-716`):
```
productos: P = (_Probabilidad>0) ? ln(_Probabilidad) : -99999 → _LN
           p = Porcentajes(v).ValoresNeperianos()        // Float[14,3]: ln( v[i,j] / (v[i,0]+v[i,1]+v[i,2]) ) en Float (D:Utils/Porcentajes.cs:329-353)
sumas:     _LN = ValorCentralSumas;  p[i,j] = Float(v[i,j])
comun:     Prob = Σ_i p[i,0]  (Float);  Cr[i,1] = p[i,1]-p[i,0];  Cr[i,2] = p[i,2]-p[i,0]       // Float
           inicializar Ap14T[k] = {Columna:k, Prob:0, Dif:0}  para k en 0..<3^14
14 triples: Bits = todo true; NumApuestas = 4 782 969
fichero   : NumApuestas = ObtenNumCols(); Bits = LeerTodasColsABitArray(14)   // columnas del fichero (sin duplicados)
EncontrarDistantes1(Prob, 0, 0, 14)                      // DFS que visita cada columna con ≥1 dígito no nulo
Ap14T[0] (columna "todo 1"): Dif=|Prob-Float(_LN)|, Prob=Prob, Columna=0      // en fichero: solo si Bits[0], si no Dif=Float(9e10)
ordena(0, 4 782 968)                                     // quicksort propio (ver peculiaridades)
si !CalculoMultiple: GrabacionColumnas() sino marcar Admitidas[Ap14T[nr].Columna] para nr<MaxColumnasMotor
EncontrarDistantes1(pProb, indIni, posIni, prof):  Profundidad++
   para partido en posIni..<14, z en 1...2:
       idx = indIni + 3^partido * z;  prob = pProb + Cr[partido,z]            // Float, suma acumulada por la ruta
       si Bits[idx]: Ap14T[idx] = {Columna:idx, Dif:|prob-Float(_LN)|, Prob:prob}
       si no:        Ap14T[idx].Dif = Float(9e10); Ap14T[idx].Prob = prob     // las ausentes del fichero van al final
       si Profundidad < prof: EncontrarDistantes1(prob, idx, partido+1, prof)
   Profundidad--
```
`pot = [1,3,9,27,…,1 594 323]` (`VM:337`). **Dígito 0 = «1», 1 = «X», 2 = «2»**, y el dígito menos significativo es el **primer carácter** (partido 1): `ConvNumAColumna` añade `SignoQuini[n%3]` y divide por 3, rellenando con '1' hasta `NumPartidos` (`D:Utils/ConvertidorDeBases.cs:31,96-106`; detalle → ESPEC_06).

*CU-C07 `GrabarColumnasOrdenadas`* (`VM:718-775`):
```
pr = Float(precio*pct/100);  premioMax = Float(recaud*pct/100);  limite = Float(LimiteProbAcumulada)
n = min(NumApuestas, MaxColumnasMotor);  acum = 0 (Float);  c = 0
para nr en 0..<n:
    linea = ConvNumAColumna(Ap14T[nr].Columna)
    si pestaña==0: prob = Float(exp(Ap14T[nr].Probabilidad)); acum += prob; si acum > limite: BREAK   // la columna que supera el límite NO se escribe
    si no:         prob = Ap14T[nr].Probabilidad; acum = prob
    si AnadirProbabilidadAcumulada: linea += TAB + acum.ToString()
    si AnadirPremio14: premio = pr/prob; si premio > premioMax {premio = premioMax}; linea += TAB + premio.ToString()
    escribir linea (GuardarColsComa = WriteLine); c += 1
```
*CU-C08 `OrdenacionMultiple`* (`VM:453-515,778-836`):
```
(a,b) = (LnMinimo, LnMaximo); si b<a intercambiar
si MultiplePorProbabilidad y b>0: Estado="Los LN deben ser valores negativos"; return
paso = (b-a)/(NumPuntos-1)                                    // Double; NumPuntos=1 → ±∞/NaN; a==b con NumPuntos>1 → paso 0 → BUCLE INFINITO
maxcol = MaxColumnasMotor; MaxColumnasMotor = Int(NumColumnasPorTramo); Admitidas = todo false; i = 0
para prCentral = a; prCentral <= b; prCentral += paso:        // acumulación en Double (no multiplicar)
    i += 1; numIter = "\(i) "
    si PorProbabilidad: _Probabilidad = exp(prCentral) sino ValorCentralSumas = prCentral
    si i==1: OrdenaPorProductos() / OrdenaPorSumas()          // con CalculoMultiple=true → deja Admitidas del tramo 1
    si no: para x en todas: si Ap14T[x].Dif == Float(9e10) continuar; Dif = |Prob - Float(prCentral)|
           ordena(0,4 782 968); para nr<MaxColumnasMotor: Admitidas[Ap14T[nr].Columna] = true
GrabarAdmitidasMultiples()  // recorre 0..<3^14 EN ORDEN DE COLUMNA (no de ordenación); mismo formato que CU-C07; PorProbabilidad: acum += exp(Prob), break si > límite
MaxColumnasMotor = maxcol
```
*`ordena(izq,der)`* — quicksort recursivo propio (Hoare, pivote `Ap14T[(izq+der)/2]`, ordena **ascendente por `ProbabilidadDiferencial`**, inestable): bucles `while Ap14T[i].Dif < x.Dif && j <= der {i++}`; `while x.Dif < Ap14T[j].Dif && j > izq {j--}`; si `i<=j` intercambia y avanza; recursión `if izq<j …; if i<der …` (`VM:694-715`). **Replicar verbatim**: el orden entre empates depende de esta implementación (T5).

**Peculiaridades a preservar / trampas Swift.**
- `Float` (32 bits) en `p`, `Cr`, `Prob`, `Dif`, acumulados (T3); `Double` en el valor central y L.A.E. `Math.Abs(float-float)` es `Float`.
- `Float(9e10)` como centinela (`== (float)9E+10`). Sumas y restas en el **mismo orden** (suma acumulada por ruta DFS).
- Texto de salida: `linea.Append(float)` usa `float.ToString()` con la **cultura actual** (coma decimal en es-ES; notación científica según .NET) → fijar `Locale` y escribir un formateador .NET-compatible de `Float` (T7); decidir y probar con golden.
- Duplicados en el fichero de entrada: `NumApuestas` cuenta líneas, `Bits` columnas únicas ⇒ se pueden escribir columnas **no pertenecientes** al fichero al final (preservar o avisar).
- `Math.Log(Math.Exp(x))` ≠ `x` exactamente: reproducir las llamadas (`_LN = ln(exp(prCentral))`).
- El combo de aciertos y los demás valores centrales **no se recalculan en vivo** (el manual dice que sí, `docs/MANUAL_USUARIO.md:377`): ver «Abiertas».
- 4,78 M elementos × 12 B ≈ 57 MB: usar `[ApuestaProbableCentral]` contiguo y `async` cancelable con progreso (T12); `BitArray` → `[Bool]` o bitset.
- Sin guardas para `Origen = Fichero` sin lectura válida, `precio = 0`, etc.

**Salidas.** Fichero `.txt` con una columna de 14 caracteres por línea (+ `\t` valor …). Estado en barra. Sin tabla en pantalla.
**Área Mac propuesta.** **Análisis** (o Herramientas) · vista principal con 3 pestañas y barra de progreso/cancelar; la rejilla de valoraciones como inspector. El motor (CU-C05..08) en `Free1X2Core/Analisis`.
**Evidencia.** `P:OrdenarPorProbabilidadFrmViewModel.cs:35-837`, `P:OrdenarPorProbabilidadFrmPage.xaml:16-277`, `P:OrdenarPorProbabilidadFrmPage.xaml.cs:13-27`, `D:Analisis/ApuestaProbableCentral.cs:24-29`, `D:Utils/Porcentajes.cs:329-353`, `D:EntradaSalida/ArchivoColumnasTexto.cs:92-102`.

---

### 77. `ParejasFrm` — Pares (herramienta fichero → fichero)

**Propósito.** Filtra un fichero de columnas por el **resultado de parejas de partidos**: para cada una de 14 filas se elige una pareja (P1, P2) y se asigna a cada una de las 9 combinaciones (11, 1x, 12, x1, xx, x2, 21, 2x, 22) un **nivel** 1..7; después, por nivel, se exige un número de apariciones dentro de [min, max]. Las columnas que cumplen se acumulan y se graban. **No edita un `Grupo` del motor** (`P:ParejasFrmViewModel.cs:23-27`). **Se abre** desde la pantalla *Formatos* (`P:FormatosFrmPage.xaml.cs:53`); no figura en menús ni barra.

**Entradas** (`P:ParejasFrmPage.xaml`):

| Control | Tipo | Defecto | Notas |
|---------|------|---------|-------|
| Rejilla «Niveles» `:26-110`: 14 filas × [`#`, «Par (P1 / P2)» = 2 `TextBox`, 9 `TextBox` de nivel] | texto → entero | todo `"0"` (`VM:416-448`) | P1/P2 = posiciones 1..14; nivel 1..7 (0 = ignora esa combinación) |
| Rejilla «Condiciones» `:116-155`: 7 filas [`niv`, `min`, `max`] | texto → entero | min `"0"`, max `"10"` (`VM:48-51`) | |
| «Columna Ganadora» `:170` + «Analizar» `:175` | texto | `"111X11X222X111"` (`VM:62`) | |
| Recuento por nivel 1..7 `:183-211` | solo lectura | `"-"` | |
| «Procesadas» / «Válidas» / «Tiempo» `:216-236` | solo lectura | `"0"` | se actualizan **solo al terminar** el cálculo |
| «Leer condiciones» `:244`, «Salvar condiciones» `:248`, «Grabar resultado» `:252` (solo con `PuedeGrabar`), «Cancelar» `:257`, «Calcular» `:261` (deshabilitado mientras corre) | botones | | |

**Acciones.**
- *Calcular* (`VM:190-238`): picker `.txt`/`*` → `RecuperarPantalla()` → en segundo plano lee línea a línea; por cada línea `ctproc++`; si `Valida(línea)` → `bits[S1n(línea)] = true; ctval++`. Al acabar muestra contadores y habilita Grabar. Errores → «Error al procesar el fichero de columnas: …».
- *Grabar resultado* (`:246-267`): picker «Resultados» `.txt` (sugerido «Resultados»); recorre los 4 782 969 bits y escribe `N1s(idx)` por cada bit activo (**orden por índice, sin duplicados**, no el del fichero origen).
- *Analizar* (`:274-285`): `RecuperarPantalla()`; `Valida(ColumnaGanadora)`; muestra `rks[n,2]` (recuento por nivel) de la última validación.
- *Leer / Salvar condiciones* (`:112-183`): fichero `.par` (picker `.par`+`*`, sugerido «Pares», tipo «Condiciones»): **14 líneas** `P1,P2,v11,v1X,v12,vX1,vXX,vX2,v21,v2X,v22` y **7 líneas** `min,max` (separador `,`). Al leer, una línea con menos campos se salta (se consume); EOF anticipado corta. Errores → «No se ha podido leer/guardar el fichero de condiciones: …».
- *Cancelar* (`:290-295`): solo pone `_salida = true` (aborta el bucle de Calcular en curso); **no navega** (no hay `GoBack`).

**Lógica en el ViewModel.**

*CU-C09 `FiltrarColumnasPorParejas`* (`VM:303-389`):
```
recuperar(): nivells[14][11] = Int(P1,P2,v11…v22)  ; rks[7][2] = Int(min,max)       // AInt: int.TryParse (cultura actual); inválido → 0
valida(col):                                                                          // devuelve Bool y deja rks[n][2] = recuento por nivel
    rks[*][2] = 0 ; col = col.uppercased()
    para fila en 0..<14:
        n1 = P1-1 ; n2 = P2-1
        si n1<0 || n2<0 || n1>=col.count || n2>=col.count : continuar
        par = col[n1] + col[n2]
        nv  = tabla[par]  con  11→v11, 1X→v1X, 12→v12, X1→vX1, XX→vXX, X2→vX2, 21→v21, 2X→v2X, 22→v22 ; otro → 0
        si nv > 0 : rks[nv-1][2] += 1                    // ¡nv > 7 provoca IndexOutOfRange!
    para n en 0..<7 : si rks[n][2] < rks[n][0] || rks[n][2] > rks[n][1] : devolver false
    devolver true
s1n(col)  : n = 0 ; para i en 0..<14 { n = n*3 + (col[i]=="1" ? 1 : col[i]=="2" ? 2 : 0) }       // X (y cualquier otro) = 0 ; MSB primero
n1s(n)    : repetir 14 veces { d = n%3 ; n /= 3 ; s = (d==1 ? "1" : d==2 ? "2" : "X") + s }       // inverso de s1n (antepone)
```
**Esta codificación base-3 es propia de la pantalla** (X=0, 1=1, 2=2, MSB primero) y **no** la de `ConvertidorDeBases` (1=0, X=1, 2=2, LSB primero): no unificar.

**Peculiaridades a preservar / trampas Swift.**
- `Valida` mata la ejecución si algún nivel vale > 7 (el C# lanza `IndexOutOfRangeException` dentro de `Task.Run`, que acaba en el diálogo «Error al procesar…»). En Swift: validar niveles 0..7 al editar y tratar > 7 como error.
- `S1n` exige ≥ 14 caracteres (`Substring(nr,1)` lanza con líneas cortas o vacías → mismo diálogo de error y se pierde lo ya acumulado en pantalla, aunque el BitArray conserva lo marcado). `Valida` sí acepta columnas más cortas/largas.
- `ToUpper()` solo en `Valida`; `S1n` compara con "1"/"2" sin normalizar.
- El BitArray (`new BitArray(4782969)`) es de instancia y se reinicia en cada Calcular; si se cancela, `PuedeGrabar` queda activo con resultados parciales.
- `Tiempo` = `(DateTime.Now - t0).ToString()` → formato `TimeSpan` constante `[d.]hh:mm:ss[.fffffff]` (invariante).
- Escritura `StreamWriter` UTF-8 sin BOM y `\r\n` (T8). Los contadores se muestran como enteros sin separador de miles.

**Área Mac propuesta.** **Condiciones y filtros** (herramienta sobre ficheros; no pertenece al boleto) · vista principal con rejilla `Table` editable y panel de resultados; botón Cancelar real (cancelación cooperativa) y progreso en vivo.
**Evidencia.** `P:ParejasFrmViewModel.cs:29-476`, `P:ParejasFrmPage.xaml:26-261`, `P:ParejasFrmPage.xaml.cs:15-23`.

---

### 78. `PesosNumFrm` — Pesos Numéricos (condición)

**Propósito.** Condición sobre el **peso numérico** de la columna (representación numérica global y por signos Variantes/1/X/2), con tolerancias por conjunto, número de tolerancias admitidas (0..5) y **figuras** permitidas. Escribe sobre `FiltroPesosNumericos` del grupo en edición. **Se abre** desde el botón «Pesos Numéricos» de `MainPage` (`W:Views/MainPageViewModel.cs:167`); patrón §1.10.

**Entradas** (`P:PesosNumFrmPage.xaml`):

| Control | Tipo | Defecto | Notas |
|---------|------|---------|-------|
| 5 `TextBox` «Global», «Var (variantes)», «1», «X», «2» `:29-45` | texto «p. ej. 0,1,2,3» (lista separada por **comas**) | vacío | La UI dice «dígitos 0 a 9» (`:25`), pero el filtro admite **0…`NumeroPartidos`** (arrays `NumeroPartidos+1`, `D:MotorCalculo/FiltroPesosNumericos.cs:63-73`); actualiza al teclear (`UpdateSourceTrigger=PropertyChanged`). |
| 5 `TextBox` de «Tolerancias» (mismos nombres) `:58-74` | idem | vacío | Conjunto de valores tolerados por cada peso. |
| «Núm. Tolerancias» `:90-98`: 6 `ToggleButton` «0»…«5» | multi-selección | ninguno marcado | |
| «Figuras» `:118-126`: 5 `ToggleButton` «3-2», «3-1-1», «2-2-1», «2-1-1-1», «1-1-1-1-1» | multi-selección | ninguno | Nota: «No se incluyen las figuras 5 y 4-1, que darían 0 columnas.» (`VM:217-218`). Texto de ayuda de cabecera: «El Peso Numérico de una columna es una representación numérica de la columna…» (`VM:221-223`). |
| Aceptar · Estadísticas · Guardar… · Abrir… · Copiar · Pegar · Borrar · Cancelar `:139-161` | botones | | §1.10 |

Guardar: sugerido «PesosNumericos»; tipos «Pesos Numéricos» `.pes` y «(XML)» `.xml`; temporal `Temp/tmp.pes` (`VM:76-83`).

**Acciones.** Al entrar (no al volver): `MarcarValores(filtro)` (`VM:93-128`): `PesoGlobal=GetPNGlobal()`, `PesoVariantes=GetPNVariantes()`, `PesoUnos/Equis/Doses` y las 5 `…Tol` con `GetPN*Tol()`; las tolerancias seleccionadas = `Set(GetTolerancias().Split(','))` (p. ej. «0,2,5»); las figuras seleccionadas = las casillas cuyo código (`ObtenerLongFiguraFromText`) está en `filtro.Figuras`. *Borrar* (`:332-349`) vacía los 10 textos, desmarca todo y `_figuras = []`. Resto de botones: §1.10.

**Lógica en el ViewModel.**

*CU-C10 `AplicarCondicionPesosNumericos`* (`VolcarA`, `P:PesosNumFrmViewModel.cs:229-272`; Aceptar `:283-296`; temporal `:275-280`):
```
figuras = casillas marcadas → [Int64]  con  fig("a-b-c") = ((a<<4 | b)<<4) | c      // 4 bits por grupo (D:Utils/UtilidadesEntradasValores.cs:220-231): "3-2"=0x32, "3-1-1"=0x311, "2-2-1"=0x221, "2-1-1-1"=0x2111, "1-1-1-1-1"=0x11111
todos = "0,1,…,NumeroPartidos"
filtro.ReinicializaValores()
hayDatos = algún texto de Global/Var/1/X/2 no vacío  ||  figuras.count > 0                // NO cuenta las tolerancias ni el nº de tolerancias
si hayDatos:
    si !filtro.ContieneDatos { filtro.IsActive = true } ; filtro.ContieneDatos = true
    SetPNGlobal(Global.vacío ? todos : Global) ; SetPNVar / SetPNUnos / SetPNEquis / SetPNDoses  igual     // vacío = sin restricción
    si hayTol (algún texto …Tol no vacío):
         SetPNGlobalTol(texto) … (5 llamadas con el texto tal cual, "" si vacío)
         tol = números marcados 0..5 unidos por ","   ;  filtro.PonerTolerancia( tol.vacío ? "0" : tol )
    si no: filtro.PonerTolerancia("0")
    filtro.Figuras = figuras
si no: filtro.IsActive = false ; filtro.ContieneDatos = false
grupo.ActivaFiltro(filtro) ; NotificarCambio ; volver
```
Estadísticas = CU-C03 con `FiltroPesosNumericos` temporal. Guardar/Abrir: `ArchivoCondiciones` (`:299-316`); `g.GetFiltro("PesosNumericos")`.

**Peculiaridades a preservar.**
- Las cadenas se pasan al filtro, que hace `Split(',')` + `Convert.ToInt32` (`D:MotorCalculo/FiltroPesosNumericos.cs:549-557`): texto con espacios o valores > `NumeroPartidos` lanza excepción (sin captura en `Aceptar`/`Estadisticas`). Cadena vacía se ignora.
- Las **figuras no listadas** que trajera el filtro (p. ej. de un fichero) **se pierden** al re-Aceptar porque `ConstruirFiguras` lee solo las 5 casillas (el campo `_figuras` solo se usa en Borrar/Marcar) (`:66-67,122-127,142-153`).
- `PonerTolerancia("0")` se aplica **siempre** que haya datos: el nº de tolerancias nunca queda vacío.

**Salidas.** Cadenas «v,v,v» (con letra/tolerancia según el filtro); casillas verdes. Semáforo en Inicio.
**Área Mac propuesta.** **Condiciones y filtros** · hoja modal del editor genérico; los 10 textos como *token fields* numéricos, las figuras y tolerancias como `Toggle(.button)` en un `Grid`.
**Evidencia.** `P:PesosNumFrmViewModel.cs:25-357`, `P:PesosNumFrmPage.xaml:11-161`, `P:PesosNumFrmPage.xaml.cs:15-36`, `D:MotorCalculo/FiltroPesosNumericos.cs:44-73,513-557,996-1014`.

---

### 79. `PosiblesPremiosFrm` — Posibles premios

**Propósito.** Con una **columna ganadora parcial** (signos 1/X/2 o «*» = aún no definitivo) y un fichero de boletos/columnas jugadas, calcula a cuántos premios (16/15/14/13/12/11/10) opta cada columna, navega por las columnas premiadas de 8 en 8 (un «boleto») y genera un resumen por cada combinación concreta de los «*». **Se abre** desde las pantallas de Escrutinios y Escrutar combinaciones (`P:EscrutiniosFrmViewModel.cs:982`, `P:EscrutarCombinacionesFrmViewModel.cs:649`).

**Entradas** (`P:PosiblesPremiosFrmPage.xaml`):

| Control | Tipo | Defecto | Notas |
|---------|------|---------|-------|
| 16 `ComboBox` «Ganadora» (uno por partido) `:81` | opciones `""`, `1`, `X`, `2`, `*` (`VM:114`) | `""` | Fila `Part.` + signos de las ≤8 columnas jugadas (solo lectura) `:88-103` |
| «Archivo…» `:136` | picker `.txt`, `.cols` | — | Etiqueta con el nombre del fichero |
| `◀` `:140` / `▶` `:144` | navegación entre boletos | | |
| «Considerar último partido como pleno» `:159` | check | no | |
| «Generar resumen» `:162` | check | no | |
| «Mis mejores opciones» `:206`, «Ver» `:210`, «Copiar» `:214`, «Guardar Resumen» `:218`, «Calcular» `:222` | botones | | |

**Salidas** (`:175-189`, `VM:134-157`): «Optando a 16: n» … «Optando a 10: n» y «Columnas con premio: n» (o «No hay premios :(»).
**Mensajes**: «Error leyendo columnas», «Primero abre un fichero de columnas.», «No hay resumen que mostrar. Marca 'Generar resumen' y calcula.», «No hay resumen que guardar. Marca 'Generar resumen' y calcula.».

**Acciones y lógica en el ViewModel.**

*CU-C11 `CargarColumnasJugadas`* (`VM:163-199`): `ac = ArchivoColumnasTexto(ruta)`; `N = ac.ObtenNumSignos()`; para cada `SiguienteColumna()`: `c = LeeColumnaSinComas()`; si `c.count != N` → error «Error leyendo columnas», vaciar y abortar; si no `columnas.append(c.prefix(N).uppercased())`. Al terminar: nombre de fichero, `AdaptarInterfaz(N)` (vacía el signo ganador de los partidos ≥ N), `premiadas=[]`, `boleto=1`, `nBoletos=0`, rejilla vacía.

*CU-C12 `EscrutarPosiblesPremios`* (`Escrutar`, `VM:407-446`) — columna jugada `a`, ganadora `g` (con «*»), `N` = nº de partidos del fichero, `pleno` = «Considerar último partido como pleno»:
```
aciertos = 0 ; posibles = N
para i en 0..<(N-1):
    si posibles < 10 : romper
    si a[i]==g[i] || g[i]=="*" : aciertos += 1  si no : posibles -= 1
si pleno : si aciertos == N-1 && (a[N-1]==g[N-1] || g[N-1]=="*") : aciertos += 1
si no    : si a.last==g.last || g[N-1]=="*" : aciertos += 1          // el último partido suma siempre que coincida
devolver aciertos
```
*CU-C13 `CalcularPosiblesPremios`* (`Calcular` `VM:223-264`, `ObtenGanadora` `:382-404`):
```
(si no hay columnas → error "Primero abre un fichero de columnas.")
vaciar col16…col10 y premiadas
ganadora = ""; noDefinitivos = 0
para i en 0..<16 (partidos):                                   // lee los 16 combos
    s = signo[i]; si s ∈ {"1","x","X","2"}: ganadora += s.uppercased()
    sino si i < N: ganadora += "*"; signo[i] = "*"; noDefinitivos += 1      // los vacíos visibles se marcan '*' en pantalla
para cada columna c: a = Escrutar(c, ganadora); si a > 9 : col[a].append(c + String(a))      // sufijo con el nº de aciertos; a ∈ 10…16
premiadas = col16 + col15 + col14 + col13 + col12 + col11 + col10      // orden de categorías, dentro de cada una orden de fichero
nBoletos = ceil(premiadas.count / 8)
si GenerarResumen: resumen = []; Expandir(ganadora, "", 0) ; si resumen no vacío : ordenar con PosiblesPremiosComparer
si premiadas.count > 0 : boleto = 1 ; mostrar boleto 1 ; "Columnas con premio: n" ; MostrarOpciones
sino : "No hay premios :(" ; MostrarOpciones ; limpiar rejilla
```
*`Expandir(pre, partido)`* (`AnalizarColumnasResumen` `:475-500`): para cada signo `s ∈ [1,X,2]`: si `ganadora[partido]=="*"` → `nuevo = pre + s`; si no → `nuevo = pre + ganadora[partido]` y se fuerza **una sola** iteración; si `partido < ganadora.count-1 && partido < N-1` recursión con `partido+1`, si no `ObtenerResumen(nuevo)`. `ObtenerResumen(g)` escruta **todas** las columnas con `g` y, si alguna categoría tiene columnas, añade un contenedor `{ColGanadora: g, Col16…Col10}` (listas con sufijo de aciertos).
Orden del resumen (`D:Escrutinio/PosiblesPremiosComparer.cs:12-47`): **descendente** por nº de columnas de Col16, luego Col15, 14, 13, 12, 11, 10 (`List.Sort` no estable: los empates quedan en orden indefinido para n > 16; T5).

*`MostrarOpciones`* (`:606-628`): `p16..p10` = tamaños de `col16..col10`; **si `pleno` entonces `p14 += p15`** (¡el 15 se suma al 14 en pantalla!); textos «Optando a k: n».
*`LlenarColumnas(boleto)`* (`:577-594`): limpia la rejilla; `desde = boleto*8-8`; muestra hasta 8 columnas `premiadas[desde+c].prefix(N)` signo a signo en la fila de su partido.
*Navegación*: `▶` solo si `boleto < nBoletos`; `◀` solo si `boleto > 1` (`:202-217`).
*Copiar* (`:287-304`): texto `"Posibles premios a falta de {noDefinitivos} partidos:"` + `\r\n` + «Optando a 16: n» (solo si N ≥ 16) + «Optando a 15: n» (solo si N ≥ 15) + 14, 13, 12, 11, 10 (todas con `\r\n` explícito) al portapapeles (`DataPackage` directo, no el hook).
*Guardar resumen* (`:307-349`): picker «Resumen» `.txt`; contenido: `Posibles Premios a falta de {noDefinitivos} partidos` / por cada contenedor: línea de 46 guiones, `Columna Ganadora: {g}`, 46 guiones, y para las categorías **14, 13, 12, 11, 10** (no 16/15) una línea `"{14 primeros signos} {n} aciertos"` por columna (`prefix(min(14,len))`), 46 guiones, línea `" "`. Saltos de línea con `AppendLine` (= `\r\n` en Windows); escrito con `FileIO.WriteTextAsync` (UTF-8).
*Ver* (`:267-284`): copia `resumen` a `VisorPosiblesPremiosViewModel.UltimoResumen` y navega a `VisorPosiblesPremiosPage` (pantalla 104); sin resumen → aviso.
*Mis mejores opciones* (`:352-375`): `ObtenGanadora()`; navega a `MejoresOpcionesFrmPage` con `MejoresOpcionesContexto(ganadora, copia(columnas), pleno)`.

**Peculiaridades a preservar.** (a) El bucle de `Escrutar` se detiene si ya no pueden alcanzarse 10 aciertos (`posibles < 10`), y el último partido se evalúa después (T9: solo enteros). (b) Con `pleno=false` y N=15/16 el último partido cuenta igual que los demás; con `pleno=true` solo si los anteriores son todos aciertos. (c) El sufijo numérico se concatena al final de la columna (`c + "14"`), y todo el código posterior asume `prefix(N)`. (d) El «*» escrito en pantalla es un efecto lateral de `ObtenGanadora`. (e) Los `ToUpper()` y `==` son ordinales. (f) Complejidad del resumen: 3^k combinaciones × columnas (k = nº de «*»); sin límite ni cancelación en el C#.
**Área Mac propuesta.** **Escrutinio** · vista principal (rejilla 16×(ganadora + 8 columnas), barra lateral con resultados); «Mis mejores opciones» como hoja; el resumen en un `Table` y exportación `.txt`.
**Evidencia.** `P:PosiblesPremiosFrmViewModel.cs:22-642`, `P:PosiblesPremiosFrmPage.xaml:35-224`, `P:PosiblesPremiosFrmPage.xaml.cs:16-36`, `D:Escrutinio/PosiblesPremiosComparer.cs:10-48`.

---

### 80. `PremiadasFrm` — Análisis Premiadas

**Propósito.** A partir de un fichero de **columnas ganadoras** (una por jornada), cuenta para cada una de las 4 782 969 columnas del espacio cuántas veces habría sido premiada con 13/12/11/10 aciertos (distancia 1/2/3/4 signos respecto a una ganadora), muestra la distribución de frecuencias («784 = 1 veces»), permite grabar las columnas de una frecuencia y ver en qué jornadas ocurre. **Se abre** desde Utilidades ▸ Premiadas… / botón «Premiadas» (`W:MainWindow.xaml.cs:265,427`).

**Entradas** (`P:PremiadasFrmPage.xaml`): «Seleccionar fichero» `:50` (picker `.txt`/`*`; muestra solo el nombre; defecto «Seleccionar Previamente», `VM:52`); `ComboBox` «Premios a estudiar» `:64` con «13 aciertos», «12 aciertos», «11 aciertos», «10 aciertos» (defecto índice 3 = **10**, `VM:60,73-79`); «Calcular» `:78`; lista «Frecuencias» `:128` (selección única, índice → `FrecuenciaSeleccionadaIndex`); lista «Jornadas» `:151`; «Grabar Selección» `:171`, «Analiza Selección» `:175`. Salidas: «Procesadas» `:90` (defecto `"0"`) y «Tiempo» `:100` (defecto `"00:00:00.0"`).

**Mensajes**: «Selecciona primero un fichero de columnas ganadoras.», «Selecciona una frecuencia en la lista.», «Calcula primero las frecuencias.», «No se pudo calcular sobre el fichero de columnas ganadoras:\n{ruta}\n\n{msg}», «Columnas grabadas en {nombre}.», errores de grabación/análisis análogos (`VM:132-136,221-230,255,260,283-286,320`).

**Lógica en el ViewModel** (réplica de `PremiadasFrm.Genera/Trasvasa/Examina/s1n/n1s`, `VM:25-27`):

*CU-C14 `FrecuenciasDePremiadas`* (`Calcular` `VM:130-211`, `Genera` `:358-413`):
```
premio ∈ {13,12,11,10}  (índice 0…3);  validas = [Int32](repeating:0, count: 3^14);  nmax = 0;  procesadas = 0
para cada línea del fichero (UTF-8, en orden):
    si línea.count < 14 : continuar                     // se salta sin contar
    idx = s1n(línea)                                    // ver abajo
    procesadas += 1 ;  genera(idx)
genera(nsel):                                           // pot[k] = 3^k, k = 0…13
  para nr en 0..<14:   s1 = (nsel / pot[nr]) % 3
    para z1 en 0..<3 donde z1 != s1:  col1 = nsel + pot[nr]*(z1 - s1)
       si premio==13: contar(col1)
       para nr2 en (nr+1)..<14:  s2 = (col1 / pot[nr2]) % 3
         para z2 != s2:  col2 = col1 + pot[nr2]*(z2 - s2)
            si premio==12: contar(col2)
            para nr3 en (nr2+1)..<14: … col3 … si premio==11: contar(col3)
                para nr4 en (nr3+1)..<14: … col4 … si premio==10: contar(col4)
contar(c): validas[c] += 1 ; nmax = max(nmax, validas[c])
qdc = [Int](repeating:0, count: nmax) ; para n en validas donde n>0 : qdc[n-1] += 1
frecuencias[nr] = "\(qdc[nr]) = \(nr+1) veces"   para nr en 0..<nmax                  // formato {0:d} = {1:d} veces
```
Cada ganadora aporta +1 a las columnas a distancia **exacta** d (d=1…4); la propia ganadora (14 aciertos) no se cuenta. Una columna a distancia d de varias ganadoras acumula.
`s1n(línea)`: `n = 0; para i en 0..<14: n = n*3 + (línea[i]=="1" ? 1 : línea[i]=="2" ? 2 : 0)` (X o cualquier otro = 0; MSB primero). `n1s(n)` (`NumeroASignos`): 14 veces `d = n%3; n/=3; s = (d==1 ? "1" : d==2 ? "2" : "X") + s` (**misma codificación propia que la pantalla 77**, no la de `ConvertidorDeBases`).
*CU-C15 `JornadasDeFrecuencia`* (`Analizar` `VM:270-324`, `Examina` `:417-471`): recorre el fichero con el mismo enumerador; `jornada` se incrementa por línea válida (≥ 14 caracteres); por cada `col` generado con `validas[col] == n` (n = índice seleccionado + 1) añade `"sem.\(jornada)"`; el mismo `sem.k` puede repetirse varias veces; orden = orden de enumeración.
*CU-C16 `GrabarColumnasConFrecuencia`* (`Grabar` `VM:218-263`): picker `.txt` (sugerido «columnas», tipo «Cols.Salida»); para `nr` en `0..<3^14` con `validas[nr] == n` escribe `n1s(nr)` (orden ascendente del índice) en UTF-8 sin BOM; luego aviso «Columnas grabadas en {nombre}.».

**Peculiaridades / trampas Swift.**
- `Tiempo` = `(ahora - inicio).ToString() + "0000000000"` truncado a 10 caracteres: típicamente `hh:mm:ss.f` (p. ej. `00:00:03.1`); con un intervalo de segundos exactos daría `00:00:0300` (replicar solo si se desea paridad exacta) (`VM:207-209`).
- `validas` es `Int32[4 782 969]` (≈19 MB); en Swift `[Int32]` y bucles sin `Array` anidados; el cálculo con 10 aciertos es 16 016 incrementos por ganadora → usar `Task.detached` y progreso opcional (T12). Sin overflow posible en la práctica (contadores ≤ nº de líneas) pero usar `&+=` si se documenta T1.
- Si la lista de frecuencias queda vacía (`nmax = 0`) no hay selección posible; la selección persiste entre cálculos solo por índice (se limpian `Frecuencias` y `Jornadas` al calcular).
- Los textos de frecuencias van sin separador de miles.

**Área Mac propuesta.** **Análisis** · vista principal con `Table` de frecuencias + lista de jornadas; botones «Grabar»/«Analizar» sobre la fila seleccionada; progreso y cancelación.
**Evidencia.** `P:PremiadasFrmViewModel.cs:29-481`, `P:PremiadasFrmPage.xaml:27-175`, `P:PremiadasFrmPage.xaml.cs:17-35`.

---

### 81. `ProbabilidadPremios` — Probabilidades de premios (madre → hijas)

**Propósito.** Compara un fichero de columnas **«madre»** (base) con otro de **«hijas»** y calcula, para cada categoría de aciertos del rango indicado, la probabilidad de que una columna de la madre dé premio dentro de las hijas. **Se abre** desde Combinación ▸ Probabilidades… / botón «Probabilidades» (`W:MainWindow.xaml.cs:230,401`).

**Entradas** (`P:ProbabilidadPremiosPage.xaml`): «Seleccionar…» para madre `:50` y hija `:68` (picker `.txt`/`*`; muestran el nombre sin extensión, defecto «(seleccionar)», `VM:47-51,79,90`); `TextBox` «Rango de aciertos» `:88` (placeholder «Ej. 10-14»; ayuda «Si se deja vacío se usa el rango por defecto 10-14.» `:95`); «Calcular» `:103`, **habilitado solo con ambos ficheros elegidos** (`VM:67`). Salida: lista «Categoría / Probabilidad» `:125-137`.

**Lógica en el ViewModel.**
*CU-C17 `ProbabilidadDePremios`* (`P:ProbabilidadPremiosViewModel.cs:100-163`):
```
(min, max) = (10, 14)
si Rango no vacío:
    intentar: lista = UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(Rango)    // admite "10-14", "10,12,14", "10-12,14"; lista sin repetidos; en error devuelve lista vacía
              lista.sort() ; min = lista.first ; max = lista.last                        // lista vacía → excepción → por defecto
    en cualquier excepción: (min, max) = (10, 14)
resultados = []
analizador = Analizador(min)                                                          // D:Analisis/Analizador.cs
analizador.ComparaCombinaciones(madre, hija)
para i en min...max:
    p = analizador.ObtenProbabilidadPremios(i)                                         // Double (D:Analisis/ProbabilidadesPremios)
    resultados.append( ("Premio de \(i)", formato(p, "#,##0.00;0.00") + " %") )
errores → diálogo "Error al calcular las probabilidades: {msg}"
```
Sin lógica propia más allá del rango y el formato. `ComparaCombinaciones` (→ ESPEC_06): marca en un `BitArray` las hijas; para cada base: si está en las hijas cuenta un premio de `N` aciertos directo, si no explora variantes a distancia `≤ N - premioMinimo` (`D:Analisis/Analizador.cs:73-131`). Si los ficheros tienen distinto nº de signos muestra «Los Dos archivos tienen un número distinto de signos» (`UserDialogs.ShowError`) y sale **sin** calcular.

**Peculiaridades / trampas Swift.**
- El rango «14-10» (min > max) produce lista vacía → se usa 10..14; un solo valor «12» da una única fila.
- El formato `#,##0.00;0.00` es **cultura actual** (separador de miles y decimal del sistema, p. ej. `1.234,56 %` en es-ES) (T7). El 2º tramo (`0.00`) rige para negativos (sin signo menos). Usar `NumberFormatter` con `Locale` explícito, 2 decimales, agrupación, y mismo redondeo (T4: .NET formatea con redondeo «away from zero» en `ToString("0.00")`, no el bancario de `Math.Round`; confirmar con golden).
- El cálculo se ejecuta en `Task.Run`; no hay cancelación ni progreso.
- La lista de resultados solo se rellena si el cálculo termina correctamente (`Resultados.Clear()` antes, `:128`).

**Área Mac propuesta.** **Análisis** · vista principal pequeña (dos selectores de fichero, campo de rango, `Table` de resultados).
**Evidencia.** `P:ProbabilidadPremiosViewModel.cs:35-164`, `P:ProbabilidadPremiosPage.xaml:22-137`, `D:Utils/UtilidadesEntradasValores.cs:115-160`, `D:Analisis/Analizador.cs:73-131,207-210`.

---

### 82. `ReductorFrm` — Reductor

**Propósito.** Reduce un fichero de columnas a otro con menos columnas garantizando un premio de cierto nivel, eligiendo el algoritmo (`IReduccion`). **Se abre** desde Combinación ▸ Reducir… / botón «Reducir» (`W:MainWindow.xaml.cs:221,393`) o el botón «Reducir» de Inicio (que antes vuelca los pronósticos al motor: `W:Views/MainPageViewModel.cs:559-564`; el reductor no usa luego ese estado).

**Entradas** (`P:ReductorFrmPage.xaml`):

| Control | Tipo / rango | Defecto | Notas |
|---------|--------------|---------|-------|
| «Archivo entrada» `:43` | picker `.txt` | «(falta selección)» | Se valida al elegir (CU-C18) |
| «Archivo salida» `:54` | picker guardar (sugerido «Reducido», tipo «Columnas» `.txt`) | «(falta selección)» | |
| «Nivel Reducción» `:82` | combo `13,12,…,1` (fijo, `VM:36-37`) | `13` | El legacy lo llenaba con `NumeroPartidos-1 … 1`; aquí está fijo a 14 partidos |
| «Método Reducción» `:96` | combo «menos tiempo», «menos columnas», «grandes archivos» (`VM:43-44`) | «menos tiempo» | |
| «Admitir columnas externas» `:104` | `ToggleSwitch` Sí/No | No | **Oculto** (`Collapsed`) con «grandes archivos» y «menos columnas 2» (`VM:59-62`) |
| «Máx. Columnas» `:126` | `NumberBox` ≥ 0 | 0 | 0 = sin límite (semántica del algoritmo → ESPEC_05) |
| «Máx. %» `:134` | `NumberBox` 0…100 | 100 | |
| «Reducir» `:209` (habilitado con entrada y salida y sin reducción en curso, `VM:109-112`), «Cancelar» `:214` | botones | | |

**Salidas** (`:150-200`): «Col. Iniciales», «Procesadas», «Admitidas», «Porcentaje», «Tiempo»; valor inicial «-». Formato: `" = {n}"` para los tres contadores; `Porcentaje = " = " + (procesadas*100/iniciales)` (**división entera**, o «-» si iniciales = 0); `Tiempo = (ahora - inicio).ToString()` (formato `TimeSpan` `hh:mm:ss.fffffff`) (`VM:225-237`). Se refrescan **cada 3000 ms** con un temporizador de UI y una última vez al terminar.

**Lógica en el ViewModel.**
*CU-C18 `ValidarFicheroDeEntradaReductor`* (`VM:115-130`): lee todas las columnas como número (`LeerTodasColsANumero`) y reabre para `ObtenNumSignos()`; válido si `== 14` y sin excepciones. Si falla: «El archivo de entrada debe tener 14 partidos.\nCompruebe además que no hay líneas en blanco adicionales al final del archivo.» (no se asigna el fichero).
*CU-C19 `ReducirColumnas`* (`VM:168-222`):
```
si ya reduciendo: return
reductor = switch método:  "grandes archivos" → Redu1305Xfsf(externas) ; "menos tiempo" → JDC(externas)
                           "menos columnas" → JDCDobleContador(externas) ; "menos columnas 2" → ReductorTM() ; otro → JDC(externas)
nivel = Int(NivelSeleccionado) ; maxCol = Int(MaxColumnas) (trunca) ; percent = Int(MaxPorcentaje) (trunca)
contadores = "-" ; arrancar temporizador 3 s → refrescarContadores
en segundo plano: reductor.ComienzaReduccion(entrada, salida, nivel, maxCol, percent)
errores → diálogo "Error en la reducción: {msg}"
finally: parar temporizador ; refrescarContadores ; Reduciendo = false
```
*Cancelar* (`:240-247`): `reductor?.Cancelar()`; pone `Reduciendo = false` **de inmediato** (la tarea sigue hasta que el algoritmo observe la cancelación, y mientras tanto se podría lanzar otra reducción; `finally` de la primera repetirá el refresco). **No cierra ni navega** (a diferencia del legacy `BtnCancelClick`).
Interfaz del dominio (`D:Reduccion/IReductor.cs:22-31`): `NoColumnasIniciales/Finales/Procesadas`, `Cancelar()`, `ComienzaReduccion(entrada, salida, nivel, maxCol, percent)`, `Inicializa(entrada, nivel)`. Los algoritmos están en ESPEC_05.

**Peculiaridades a preservar / decisiones.**
- `ReductorTM` («menos columnas 2») **no es seleccionable** desde la UI actual (no está en la lista), aunque el `switch` y la visibilidad lo contemplan (`VM:43-44,60,179`) → ver «Abiertas».
- El cálculo y el temporizador comparten contadores sin sincronización (lecturas de `int` en el hilo de UI); en Swift exponer un `AsyncStream<Progreso>` en vez de sondear.
- `(int)NumberBox.Value` trunca; `NaN` → valor basura en C#, abortaría en Swift (`Int(exactly:)`).

**Área Mac propuesta.** **Reducción** · vista principal (formulario + panel de progreso con `ProgressView`, botón «Cancelar» real y resumen final); reutilizable desde Inicio con la combinación actual como entrada.
**Evidencia.** `P:ReductorFrmViewModel.cs:21-248`, `P:ReductorFrmPage.xaml:27-214`, `P:ReductorFrmPage.xaml.cs:11-19`, `D:Reduccion/IReductor.cs:22-31`.

---

### 83. `RentabilidadFrm` — Rentabilidad (esperanza matemática)

**Propósito.** Calcula la **esperanza matemática (EM)** de premio de cada columna (premio de 14 estimado a partir de lo apostado × probabilidad real) para las 14 triples o para un fichero, y graba las que estén entre dos límites de EM (opcionalmente ordenadas y con la EM añadida). También valora **una sola columna**. **Se abre** desde Utilidades ▸ Rentabilidad… / botón «Rentabilidad» (`W:MainWindow.xaml.cs:263,424`).

**Entradas** (`P:RentabilidadFrmPage.xaml`):

| Control | Tipo / rango | Defecto |
|---------|--------------|---------|
| Radios «14 Triples» / «Fichero» `:31,36` | exclusivos | 14 Triples |
| `TextBox` fichero entrada + botón carpeta `:48,54` (solo con Fichero) | picker `.txt` | vacío |
| `TextBox` fichero salida + botón guardar `:72,77` | picker guardar (sugerido «Rentabilidad», tipo «Columnas» `.txt`) | vacío |
| «Recaudación considerada» `:99` | `NumberBox` ≥ 0 | 8 000 000 (`VM:63`) |
| «Mínimo» `:129` / «Máximo» `:144` (límites EM) | `NumberBox` | 0,133 / 50 (`VM:67,70`) |
| «Ordenar columnas por EM» `:161`, «Añadir EM al fichero de salida» `:165` | `ToggleSwitch` | no / no |
| Rejillas de porcentajes **«Apostados»** y **«Reales»** (`PorcentajesControl`) `:277,283` | `NumeroPartidos × 3` `double` ≥ 0 | todo 0 |
| «Columna» `:209` (máx. 14, mayúsculas, placeholder «14 signos (1 X 2)») + botón calcular `:216` | texto | vacío; el botón solo se habilita con **exactamente 14** caracteres (`VM:104,159`) |
| «Calcular» `:174` (deshabilitado mientras `Calculando`, `VM:315`) | | |

**Salidas.** Estado `:179` («Listo» inicial; «Faltan datos»; «Inicializando…»; «Leyendo columnas…»; «Calculando…»; «Ordenando…»; «Grabando…»; «Finalizado (N columnas)»; «Error: {msg}»; «Valoración calculada», `VM:96-97,208,235-302`); «Premio estimado de 14» `:229`, «Probabilidad real» `:241`, «Esperanza matemática» `:253` (defecto «-»).

**Lógica en el ViewModel** (motor transcrito de `RentabilidadFrm.cs`, `VM:26-27`). Constantes del dominio: `precio = VariablesGlobales.PrecioApuesta`, `pct = VariablesGlobales.Porcentaje14` (ambos `double`, → ESPEC_01/07). Todo el cálculo masivo en **`Float`**.

*CU-C20 `ValorarColumnaRentabilidad`* (`VM:160-194`):
```
premioDe14 = Float(precio)*Float(pct)/100 ; premioTope = Float(Recaudacion)*Float(pct)/100          // todo en Float
pa = Porcentajes(apostados).ValoresBase100() ; pr = Porcentajes(reales).ValoresBase100()             // Float[N,3] = v/(suma de la fila)
p14A = 1 ; p14R = 1
para i en 0..<14: según columna[i]: '1'→col 0 ; '2'→col 2 ; otro→col 1 :  p14A *= pa[i,c] ; p14R *= pr[i,c]
premio = premioDe14 / p14A ; si premio > premioTope : premio = premioTope ;  esperanza = premio * p14R
mostrar:  PremioEstimado14 = Math.Round(precio*pct/100/p14A, 2).ToString()      // Double, redondeo bancario, SIN tope
          ProbabilidadReal = p14R.ToString()                                    // Float, cultura actual
          EsperanzaMatematica = esperanza.ToString()                            // Float, cultura actual
```
*CU-C21 `CalcularRentabilidad`* (`Calcular` `VM:202-313`, `EncontrarDistantes1` `:338-370`, `ordena` `:373-402`, `GrabacionColumnas` `:405-429`):
```
validar: salida no vacía y (si Fichero) entrada no vacía → si no "Faltan datos" y return
premioDe14, premioTope, pa, pr  como en CU-C20
Ap14T[k] = {Columna:k, Probabilidad:0}  para k<3^14      // struct {Probabilidad: Float, Columna: Int32} (D:Analisis/ApuestaProbable.cs:24-29)
Bits = Fichero ? LeerTodasColsABitArray(14) del fichero : todo true
p14A = Π pa[i,0] ; p14R = Π pr[i,0]
Cra[i,0] = pa[i,1]/pa[i,0] ; Cra[i,1] = pa[i,2]/pa[i,0] ; Crp[i,0] = pr[i,1]/pr[i,0] ; Crp[i,1] = pr[i,2]/pr[i,0]       // razones respecto al signo '1'
premio = min(premioDe14/p14A, premioTope) ; esperanza = premio*p14R
si Bits[0] : Ap14T[0] = {Columna:0, Probabilidad: -esperanza} sino Ap14T[0].Probabilidad = Float(3e7)
EncontrarDistantes1(p14A, p14R, indIni:0, posIni:0, prof:14):                          // DFS idéntico al de la pantalla 76 con z∈{0,1}
   para partido en posIni..<14, z en 0...1:
        idx = indIni + 3^partido*(z+1) ; pA = pProbA*Cra[partido,z] ; pR = pProbR*Crp[partido,z]
        premio = min(premioDe14/pA, premioTope) ; esperanza = premio*pR           // ¡campos de instancia, no locales!
        si Bits[idx] : Ap14T[idx] = {Columna:idx, Probabilidad:-esperanza}
        si Profundidad < prof : recursión(pA, pR, idx, partido+1)
si Ordenar : ordena(0, 4 782 968)                         // el MISMO quicksort propio que en CU-C06 pero por `Probabilidad` ascendente (= EM descendente)
grabar:  emMin' = -EmMin ; emMax' = -EmMax                // EmMin/EmMax son los NumberBox
   para nr en 0..<3^14: a = Ap14T[nr]
       si Bits[a.Columna] && a.Probabilidad >= emMax' && a.Probabilidad <= emMin':
           con EM : GuardarColsComa( ConvNumAColumna(a.Columna) + TAB + (-a.Probabilidad).ToString() )
           sin EM : GuardarCols( ConvNumAColumna(a.Columna) )              // GuardarCols elimina comas
           NumCols += 1
estado final "Finalizado (\(NumCols) columnas)"
```
Es decir, se graban las columnas con `EmMin ≤ EM ≤ EmMax` (comparación `Float` contra `Double`).
Orden de salida: **sin ordenar → índice ascendente** (`Columna == nr` por la inicialización); **ordenar → EM descendente** con el desempate que dicte el quicksort propio (T5; copiar `ordena` de CU-C06).

**Peculiaridades / trampas Swift.**
- `Float` en `pa, pr, Cra, Crp, premio, esperanza` (T3); el orden de multiplicación sigue la ruta DFS. `Premio`/`Esperanza` son **campos** mutados en la recursión (en Swift pasarlos como locales: mismo resultado porque se recalculan antes de usarse).
- `PremioEstimado14` (solo ficha de columna) **no** se recorta con `premioTope`, mientras que la EM sí; usa `Math.Round(x, 2)` → `ToEven` (T4) y `Double.ToString()` de la cultura actual (T7). `Float.ToString()` en probabilidad/EM: formato .NET (notación científica para valores muy pequeños, p. ej. `1,234E-07`); implementar un `formatearFloatNET`.
- Las columnas fuera del fichero conservan `Probabilidad = 0` y se descartan por `Bits`; con Fichero, `Ap14T[0]` si no está en el fichero recibe `3e7`.
- Defecto `EmMin = 0,133` vs. recomendación del manual 0,136 (`docs/MANUAL_USUARIO.md:382`): es el valor del código; el manual se escribió con otro precio.
- Salida binaria del legacy (`FilterIndex==2`) no existe en la versión WinUI (`VM:145`).
- 57 MB de `Ap14T` y recorrido completo: cálculo asíncrono cancelable; mantener reutilización del array entre ejecuciones solo si se reinicializa (el C# lo recrea cada vez).

**Área Mac propuesta.** **Análisis** (o Herramientas) · vista principal con 3 bloques (Entrada/Salida, Parámetros y rejillas, Valoración de una columna) y barra de estado; las dos rejillas de porcentajes en un inspector con pestañas «Apostados/Reales».
**Evidencia.** `P:RentabilidadFrmViewModel.cs:29-430`, `P:RentabilidadFrmPage.xaml:23-283`, `P:RentabilidadFrmPage.xaml.cs:12-20`, `D:Analisis/ApuestaProbable.cs:24-29`, `D:EntradaSalida/ArchivoColumnasTexto.cs:70-102`, `D:Utils/Porcentajes.cs:341-353`.

---

### 84. `ResultadosCalculoMultipleFrm` — Resultados de la Generación Múltiple

**Propósito.** Pantalla **solo de lectura** con el resumen de un cálculo por lotes: una fila por fichero de combinación calculado. **Se abre** únicamente desde «Calcular varias combinaciones» al terminar el lote (`P:CalculaColumnasMultipleFrmViewModel.cs:264-269`); no figura en menús ni barra (solo en el registro del smoke).

**Entradas.** Ninguna. No hay botones ni acciones (`P:ResultadosCalculoMultipleFrmPage.xaml:15-89`).

**Datos y handoff.** El productor deja `ResultadosCalculoMultipleFrmViewModel.UltimosResultados` (copia de su lista) y navega; el VM lo **consume y borra** en su constructor (`P:ResultadosCalculoMultipleFrmViewModel.cs:47,56-67`). Sin handoff (apertura directa) la rejilla queda vacía.

**Salidas.** `ListView` de 5 columnas (`XAML:45-57`) — todas las celdas ya son `string`:

| Columna | Origen (en el productor, `P:CalculaColumnasMultipleFrmViewModel.cs:232-239`) |
|---------|------------------------------------------------------------------------------|
| Arch. Combinación | `Path.GetFileName(ficheroOrigen)` |
| Arch. Columnas | `Path.GetFileName(ficheroDestino)` |
| Cols. Analizadas | `analizador.ColsAnalizadas.ToString()` (entero sin separador de miles) |
| Cols. Aceptadas | `analizador.ColsAceptadas.ToString()` |
| Tiempo | `(ahora - inicioDeEsteFichero).ToString()` → `TimeSpan` `hh:mm:ss.fffffff` (invariante) |

**Lógica en el ViewModel.** Ninguna (solo `Cargar(IEnumerable)`, `:75-82`). En Swift basta un `struct ResultadoLote { archivoCombinacion, archivoColumnas, colsAnalizadas: Int, colsAceptadas: Int, tiempo: Duration }` devuelto por el caso de uso de cálculo múltiple y una `Table`; el formateo (miles, `Duration`) pasa a la vista con `Locale` explícito.
**Área Mac propuesta.** **Cálculo** · hoja modal (o panel inferior) que aparece al terminar el lote, con botón «Exportar resumen…» (CSV) como mejora opcional (decisión del dueño).
**Evidencia.** `P:ResultadosCalculoMultipleFrmViewModel.cs:14-83`, `P:ResultadosCalculoMultipleFrmPage.xaml:15-89`, `P:ResultadosCalculoMultipleFrmPage.xaml.cs:13-21`.

---

### 85. `RotacionDeSignosFrm` — Rotación de signos

**Propósito.** Transforma los signos de un fichero de columnas según una tabla de cambios por partido (qué signo destino recibe cada 1, X y 2) y escribe el resultado deduplicado y ordenado por índice. Con «Corrección de fallos» rota cada partido por separado (genera variantes). **Se abre** desde Operaciones ▸ Rotación de signos… / botón «Rotación de signos» (`W:MainWindow.xaml.cs:248,384`).

**Entradas** (`P:RotacionDeSignosFrmPage.xaml`):

| Control | Tipo | Defecto | Notas |
|---------|------|---------|-------|
| «Examinar…» entrada `:44` (fichero de solo lectura) | picker `.txt` | «(falta selección)» | Al elegir: `ObtenNumSignos()` → nº de partidos (si ≤ 0 → 14); reconstruye las filas; lee la **primera columna** y la muestra en «Base» (`VM:100-138`) |
| «Examinar…» salida `:64` | picker guardar (sugerido «ColumnasRotadas», tipo «Columnas» `.txt`) | «(falta selección)» | |
| Tabla «Cambios de signo» `:99-133`: por partido (14 por defecto, o `partidos`): número, «Base» (combo 1/X/2), «1 →», «X →», «2 →» (combos 1/X/2) | 3 combos por fila | `1→1`, `X→X`, `2→2` (rotación identidad, `VM:30-36`) | «Base» es **solo informativa**: no interviene en el cálculo |
| «Corrección de fallos» (`ToggleSwitch` Activada/Desactivada) `:150` | bool | No | |
| «Transponer» `:155` | botón | | |
| «Aceptar» `:181` (habilitado con entrada y salida), «Cancelar» `:188` | botones | | `Cancelar` **no hace nada** (`VM:206-210`; la Page no lo cablea) |

Texto informativo `:159`: «Nota: las valoraciones de JORNADA y COMBINACIÓN (ControlPorcentajes legacy) se portarán como control propio en una fase posterior.» (no implementado).
**Estados**: «Faltan datos», «Preparado», «Procesando...», «Se han grabado {n} columnas», «Error: {msg}», «Error al leer el archivo: {msg}» (diálogo) (`VM:91,170,189,194,198,134`).

**Lógica en el ViewModel.**
*CU-C22 `RotarSignosDeColumnas`* (`VM:166-204` y núcleo `Rotar` `:216-282`):
```
nuevos[p][s] = índice("1X2".IndexOf(texto)) de la fila p: s=0 → NuevoUno, s=1 → NuevaX, s=2 → NuevoDos      // texto desconocido → 0
bits[0..<3^15] = false ; cambiados[0..<3^15] = false                 // 14 348 907 posiciones: máximo 15 partidos (con 16 el índice se sale de rango)
para cada columna del fichero:  bits[ConvColumnaANumero(col)] = true       // '1'→0, 'X' (y otro)→3^i, '2'→2·3^i; carácter i = dígito i (LSB primero)
si CorreccionDeFallos:                                               // "giros": cada partido por separado
    para partido en 0..<partidos:
        para i con bits[i]: s = (i / 3^partido) % 3 ; z = nuevos[partido][s] ; cambiados[ i + 3^partido*(z - s) ] = true
si no:                                                               // rotación completa de la columna
    para i con bits[i]: idx = i
        para partido en 0..<partidos: s = (idx / 3^partido) % 3 ; z = nuevos[partido][s] ; idx += 3^partido*(z - s)
        cambiados[idx] = true
para i en 0..<3^15 con cambiados[i]: salida.GuardarCols( ConvertidorDeBases(partidos).ConvNumAColumna(i) ) ; c += 1      // orden por índice ascendente, sin duplicados
devolver c
```
*Transponer* (`VM:152-163`): para cada fila `NuevoUno = sig(NuevoUno)`, `NuevaX = sig(NuevaX)`, `NuevoDos = sig(NuevoDos)` con `sig(1)=X, sig(X)=2, sig(otro)=1`.
*Reconstrucción de filas* (`:284-289`): `partidos` filas con los 3 combos en identidad; la primera columna del fichero rellena «Base».

**Peculiaridades a preservar / defectos.**
- El `finally` de `Aceptar` llama a `ActualizarPuedeCalcular()`, que **reescribe `Estado` con «Preparado»/«Faltan datos»** inmediatamente después de poner «Se han grabado n columnas» o «Error…» (`VM:194-203,306-312`): el mensaje de resultado apenas se llega a ver. Decidir si se replica o se corrige (Swift: mostrar el resultado).
- En modo normal el resultado es **una columna por columna de entrada** (puede haber colisiones → menos columnas); en modo «Corrección de fallos» el resultado es la unión de las variantes de **un solo partido** rotado (la columna sin rotar solo aparece si algún partido tiene identidad para su signo).
- `bits`/`cambiados` ocupan 2 × 14,3 M bits; usar bitset (`[UInt64]`) y bucles `Int`; T9 (`/` y `%` enteros).
- `IndiceSigno` usa `"1X2".IndexOf(signo, Ordinal)`; no hay validación de que la tabla sea una permutación (se permite mapear dos signos al mismo).
- Escritura con `GuardarCols` (elimina comas), UTF-8 sin BOM, `WriteLine` (T8).

**Área Mac propuesta.** **Herramientas** · vista principal: dos selectores de fichero, tabla de cambios (un `Picker` por celda; botón «Transponer»), opción de corrección de fallos y resultado en pie con progreso.
**Evidencia.** `P:RotacionDeSignosFrmViewModel.cs:22-313`, `P:RotacionDeSignosFrmPage.xaml:15-188`, `P:RotacionDeSignosFrmPage.xaml.cs:9-17`, `D:Utils/ConvertidorDeBases.cs:31-45,96-106`.

---

### 86. `SelecJM` — Selección 5+5+4 de JuanM

**Propósito.** Reparte los 14 partidos en 3 grupos (5 + 5 + 4), construye para cada grupo la tabla de sus 243/243/81 combinaciones **ordenada por producto de valoraciones (de mayor a menor)**, aplica límites de posición por grupo y por suma de posiciones, y marca como válidas las columnas que los cumplen («por corte» o «por valor»). También ubica columnas ganadoras en esos rangos. **Se abre** desde Utilidades ▸ Selector JuanM… / botón «Selector JuanM» (`W:MainWindow.xaml.cs:260,422`).

**Entradas** (`P:SelecJMPage.xaml`):

| Control | Tipo / rango | Defecto |
|---------|--------------|---------|
| Rejilla de valoraciones 1/X/2 (`PorcentajesControl`) `:26` | `NumeroPartidos × 3` `double` ≥ 0 | 0 (un **0 se sustituye por 0,1** al calcular) |
| 14 `TextBox` «Partido n» (grupo, `MaxLength=1`) `:54-70` | "1", "2" ó "3" | 1×5, 2×5, 3×4 (`VM:59-72`) |
| Límites: «Grupo 1 (1-243)» `:96`, «Grupo 2 (1-243)» `:99`, «Grupo 3 (1-81)» `:102`, «Total (1-567)» `:105` | texto «a-b, c, d-e» | `1-243`, `1-243`, `1-81`, `1-567` (`VM:75-78`) |
| `ComboBox` «Examinar» `:131` | «Por corte» / «Por valor» | Por corte |
| «Leer condiciones» `:139` / «Salvar condiciones» `:142` | `.cnd`/`.txt` | «(ninguno)» |
| «Calcular» `:157`, «Cancelar» `:161`, «Grabar columnas» `:164` | botones | |
| «Cargar ganadoras» `:206`, «Columna ganadora» (14) `:222`, «-» `:231` / «+» `:235`, «Analizar» `:241` (solo con ganadoras cargadas, `VM:110`) | | |

**Salidas.** «% Grupo 1/2/3» `:118-126` (defecto «%1», «%2», «%3»); «Procesadas», «Válidas», «Tiempo», «Fichero» `:179-191`; «Rango gr.1/2/3» y «Rango total» `:257-269` (defecto «-»).
**Mensajes**: «error en grupos», «error en limites», «fichero de condiciones erróneo», «col.G. errónea», «col.G. errónea={n}» (`VM:207,210,268,290,365,372`).

**Lógica en el ViewModel** (`P:SelecJMViewModel.cs`; codificación de columna **propia**: `s2n(c, n)` = base 3 MSB-primero con X=0, '1'=1, '2'=2; `n2s(x, n)` su inversa, `VM:422-446`).

*CU-C23 `PrepararTablasJM`* (`RecuperaPantalla` `:346-373`, `LlenarTablim` `:376-403`, `CalgrCinco/Cuatro` `:454-530`, `mxsort` `:406-419`):
```
nvals = valoraciones (Double[14][3]) con 0 → 0.1
ngrups[i] = Int(texto grupo i, 0 si no parsea)  ; si #(grupo==1) != 5 || #(==2) != 5 || #(==3) != 4 → "error en grupos"
tablim = máscaras Bool de 243, 243, 81 y 567 posiciones desde los textos de límites:
    para cada trozo separado por ',' ; trozo.split('-'):
        1 número: n = clamp(n,1,max) ; tablim[n-1] = true
        2 números: (a,b) = ordenados ; a = clamp(a,1,max) ; b = clamp(b,1,max) ; tablim[a-1 ... b-1] = true
        otro nº de partes, o no entero (incluye texto vacío y negativos) → error → "error en limites"
para g en {1,2,3}:                                                      // g=1,2 → 5 partidos; g=3 → 4 partidos
    w[k][0] = nvals[partido_k][1] (X) ; w[k][1] = nvals[partido_k][0] ('1') ; w[k][2] = nvals[partido_k][2] ('2')     // k = orden de aparición del partido en el grupo
    para a,b,c,d,(e) en 0..<3 (a más externo):   pg[idx] = (((w0[a]*w1[b])*w2[c])*w3[d])*w4[e] ; id[idx] = idx ; idx += 1     // Double; idx = código base 3 con X=0
    mxsort(pg, id, 0, n-1)           // orden DESCENDENTE por pg (ver abajo)
    porcentaje_g = formato("f2", Σ_{pos: tablim[pos]} pg[pos] / (g==3 ? 1e6 : 1e8))        // cultura actual, 2 decimales
mxsort(ls1, ls2, inf, sup):          // selección con '>' estricto; se queda con el PRIMER máximo y lo intercambia con la posición i
    para i en inf..<sup: ref = ls1[i] ; s = i ; para nr en i+1...sup: si ls1[nr] > ref { s = nr ; ref = ls1[nr] }
                         ls1[s] = ls1[i](antes) ; ls1[i] = ref ; intercambiar ls2[i], ls2[s]
```
*CU-C24 `SeleccionJMPorCorte`* (`BuscaCorte` `:534-571`):
```
valida = Bool[3^14] = false ; ctval = ctproc = 0
para r1 en 0..<243: si salida: romper
    si tablim1[r1]:
       col1 = n2s(id1[r1], 5)
       para r2 en 0..<243:
          si tablim2[r2]: col2 = n2s(id2[r2],5)
             para r3 en 0..<81:
                si tablim3[r3] && tablimTotal[r1+r2+r3+2]:                         // rango total (base 1) = r1+r2+r3+3
                    col3 = n2s(id3[r3],4) ; col14 = para p en 0..<14: siguiente carácter del string de su grupo (ngrups[p])
                    valida[s2n(col14,14)] = true ; ctval += 1
                ctproc += 1
          si no: ctproc += 81
    si no: ctproc += 19683
```
*CU-C25 `SeleccionJMPorValor`* (`BuscaValor` `:575-619`): para cada `x` en `0..<3^14` (`salida` se comprueba por columna): `cadena = n2s(x,14)`; `n1,n2,n3` = productos (Double, empezando en 1, en orden de partido) de `nvals[p][0|1|2]` según el signo ('1'→0, '2'→2, otro→1) acumulados en el producto de su grupo; para todo `(r1,r2,r3)` con `pg1[r1] == n1 && tablim1[r1]`, `pg2[r2] == n2 && tablim2[r2]`, `pg3[r3] == n3 && tablim3[r3]` y `tablimTotal[r1+r2+r3+2]`: si `!valida[x]` → marcar y `ctval += 1`; `ctproc += 1`. **Igualdad exacta de `Double`** (hay que reproducir la misma secuencia de multiplicaciones).
*CU-C26 `UbicarGanadoraJM`* (`Analizar` `:283-302`, `AnaCorte` `:622-641`, `AnaValor` `:644-667`): requiere `columna.count ≥ 14` (si no «col.G. errónea»); ejecuta `PrepararTablasJM` (actualiza los % mostrados); *por corte*: separa la columna en las 3 cadenas de grupo, `i_g = s2n(...)`, `pos_g` = posición de `i_g` en `id_g` (índice 0-based, `RangoGrupo = pos+1`), `RangoTotal = pos1+pos2+pos3+3`; *por valor*: producto `n_g` con `nv==0 → 1`, `pos_g` = primera posición con `pg_g[pos] == n_g` (0 si no hay), mismas salidas.
*CU-C27 `CondicionesJM`* (`:198-252`): fichero de 6 líneas — `grupos` (14 dígitos), `limite1`, `limite2`, `limite3`, `total`, `modo` (`"1"`=corte, `"0"`=valor). Leer: <6 líneas o `grupos.count<14` → «fichero de condiciones erróneo»; `modo == "0"` → «Por valor», cualquier otro → «Por corte». Escribir con `File.WriteAllLines` (picker «Condiciones», tipo «F.Condiciones» `.cnd`).
*Ganadoras* (`:254-326`): carga `.txt`; por línea `trim().uppercased()`, vacía se salta, `count < 14` → «col.G. errónea={nº+1}» y se detiene (conserva las ya leídas); si hay ≥1 la **última** queda seleccionada (`indice = count`), `+`/`-` navegan (`nrf < count`/`nrf > 1`).
*Grabar columnas* (`:174-195`): picker «ResultadosJM» `.txt`; escribe `n2s(x,14)` para cada `valida[x]` en orden ascendente de `x` (código con X=0); la etiqueta «Fichero» muestra el nombre. (El legacy también grababa las tablas de productos; esta versión no.)
*Calcular* (`:114-164`): borra contadores a `" "`, lee rejilla/grupos/límites en el hilo de UI, trabaja en `Task.Run`; tras terminar: `% Grupo n`, `Válidas`, `Procesadas` (`ToString()`), `Tiempo = (elapsed.ToString() + "0000000000")` truncado a 10 caracteres (`:157-158`). Errores de validación → diálogo, sin tocar contadores.

**Peculiaridades / trampas Swift.**
- Complejidad: `Por valor` ≈ 4,78 M × hasta 243·243·81 comparaciones de `Double` — insoportable sin optimizar (el C# hace exactamente eso); en Swift indexar por valor (diccionario `Double → [posiciones]`) **sin cambiar el resultado** (ojo a valores repetidos: ties).
- Cancelación: `Cancelar` solo pone `salida = true`; `BuscaCorte` la lee en el bucle externo, `BuscaValor` en cada columna. El botón «Calcular» no se desactiva durante el cálculo (se pueden lanzar dos).
- `"{0:f2}"` con la cultura actual: .NET Core 3+ redondea el valor binario exacto «half away from zero»; `String(format:"%.2f")` hace «ties-to-even» sobre el exacto: puede diferir en empates exactos → implementar con `Decimal` o probar con golden (T4/T7).
- `int.TryParse` de los grupos usa la cultura actual; admite espacios de borde (`.Trim()`).
- Memoria: `Bool[3^14]` ≈ 4,8 MB; tablas pequeñas.

**Área Mac propuesta.** **Herramientas** (selección de columnas) · vista principal con rejilla de valoraciones, selector de grupos (14 `Picker`), campos de límites y `Table` de resultados; análisis de ganadoras en un inspector.
**Evidencia.** `P:SelecJMViewModel.cs:29-668`, `P:SelecJMPage.xaml:19-269`, `P:SelecJMPage.xaml.cs:14-22`.

---

### 87. `SelectorMS` — Selector MarioSan

**Propósito.** Para cada columna de un fichero, cuenta cuántas **otras columnas del mismo fichero** están a distancia exacta 1, 2 ó 3 signos (aspectos 13/12/11); agrupa las columnas por ese contador (tabla de distribución «Q»), permite analizar la distribución contra una ganadora (cuántas columnas de cada grupo darían 14…10) y grabar las columnas de un grupo. **Se abre** desde Utilidades ▸ Selector MarioSan… / botón «Selector MarioSan» (`W:MainWindow.xaml.cs:261,423`).

**Entradas** (`P:SelectorMSPage.xaml`): `ComboBox` «Aspecto» `:36` (11/12/13, defecto **13**) con `+`/`−` `:43,47` (acotan 11..13, `VM:80-92`); «Iniciar» `:56` (abre picker `.txt` y calcula), «Cancelar» `:60`; lista «Distribución» `:134` (selección única; columnas Q · cols. · 14 · 13 · 12 · 11 · 10); «Sumar» `:193`, «Grabar» `:196`; «Cargar ganadoras…» `:219`, `TextBox` «Columna» (14) `:243`, `+`/`−` `:249,253`, «Analizar» `:260` (solo con ganadoras, `VM:78`).
**Salidas** (`:80-94,180-187,222-227`): «Fichero de entrada», «Columnas leídas» (nº de columnas **distintas**), «Tiempo» (`TimeSpan.ToString()` completo), «Suma de selección», «Fichero grabado», fichero e índice de ganadoras. Todas las celdas de la tabla son `string` (`FilaDistribucion {Q,C,P14,P13,P12,P11,P10}`, `VM:495-504`).

**Lógica en el ViewModel** (`P:SelectorMSViewModel.cs`; codificación propia `S2N`/`N2S`: base 3 MSB-primero, X=0, '1'=1, '2'=2; `S2N` solo mira los 14 primeros caracteres, `:457-468`; `N2S` `:446-455`; `Pot = 3^k`, k=0…13).

*CU-C28 `DistribucionDeVecinosMS`* (`Iniciar` `:95-130`, `LeerYCalcular` `:254-284`, `Calcular13/12/11` `:286-402`):
```
limite = Int(Aspecto) ∈ {11,12,13} ; limsh = limite==13 ? 29 : limite==12 ? 365 : 2913        // nº de filas Q de la tabla
leer: para cada columna del fichero (LeeColumnaSinComas): nx = S2N(col)
        si !existe[nx]: tabla[conta] = (columna: nx, vecinos: 0) ; existe[nx] = true ; conta += 1        // SIN duplicados, orden de fichero
13: para cada i<conta: para partido p en 0..<14: s = (nx/Pot[p])%3 ; para z != s : si existe[nx + Pot[p]*(z-s)] : vecinos[i] += 1
12: repes = false; repes[nx] = true ; marcar los 28 vecinos a distancia 1 ;
    para cada vecino c1 (partido p, z): para p2 en 0..<14, z2 != dígito: c2 = c1 ± … :
        si existe[c2] && !repes[c2] : repes[c2] = true ; vecinos[i] += 1                         // cuenta existentes a distancia EXACTA 2
11: repes marca nx y todos los de distancia 1 y 2 ; luego triple bucle (p,p2,p3 sobre 0..<14 completos) cuenta existentes a distancia EXACTA 3 no marcados
lbtab = [Int](repeating:0, count:2913) ; para i<conta: lbtab[vecinos[i]] += 1
tabla de pantalla: fila q en 0..<limsh : Q=q, C=lbtab[q], P14…P10 = 0 (hasta analizar)
```
Se comprueba el token de cancelación una vez por columna leída y una vez por columna calculada (`OperationCanceledException` silenciosa); el `finally` fija `Tiempo`. Se usan `BitArray(4 782 969)` + `int[4 782 969, 2]` (≈38 MB); en Calcular12/11 se hace `repes.SetAll(false)` por columna (muy caro: 600 KB × nº de columnas) → en Swift limpiar solo lo marcado.
*CU-C29 `AnalizarDistribucionMS`* (`Analizar` `:228-247`, `Comparador2` `:405-420`, `Neq` `:422-426`): requiere `conta>0` y `ColumnaGanadora.count ≥ 14` (si no, retorna sin aviso); `resuls = Int[limsh][5]`; `ncg = S2N(ganadora)`; `c7p = ncg/2187`, `c7u = ncg%2187`; para cada columna `i`: `na = comtab[c7p][nx/2187] + comtab[c7u][nx%2187]` (nº de signos iguales entre ambas; `comtab[a][b]` = nº de posiciones iguales entre los códigos de 7 signos `a` y `b`, `comtab[a][a]=7`); si `na > 9`: `resuls[vecinos[i]][14-na] += 1` → columnas «14»…«10» de la fila Q del grupo; luego refresca la tabla. (`comtab` es 2187×2187 `Int32`, ≈19 MB, calculado una vez, perezoso.)
*Sumar* (`:135-145`): `SumaSeleccion` = columna `C` de la **fila seleccionada** (el legacy sumaba varias; aquí solo una; sin selección → "0").
*Grabar* (`:147-174`): si `conta == 0` o sin fila → no hace nada (sin mensaje); picker «SeleccionMS» `.txt`; escribe `N2S(tabla[i].columna)` para las `i` con `vecinos[i] == FilaSeleccionada` en orden de fichero (`GuardarCols`).
*Ganadoras* (`:176-226`): como en la pantalla 86 pero al detectar una línea corta muestra «col.G. errónea=<texto de la línea>» y **retorna dejando ya cargadas las anteriores sin actualizar el índice**; `trim().uppercased()`, vacías se saltan; tras cargar queda seleccionada la última.

**Peculiaridades / trampas Swift.**
- `int[4 782 969, 2]` y `BitArray` por ejecución: usar `[Int32]` planos; paralelizar por columna es posible (el resultado `vecinos[i]` es independiente) manteniendo el orden de `tabla`.
- Distancias: «a distancia exacta d» con vecinos ya contados una vez (`repes`); no es el nº de caminos.
- Mensaje de error de lectura de columnas cortas: `S2N` lanzaría con < 14 caracteres (capturado → «Error: …»).
- Valores `Q` fuera del rango (`vecinos[i] ≥ limsh`) no pueden darse (máx. 28/364/2912).

**Área Mac propuesta.** **Herramientas** (selección de columnas) o **Análisis** · vista principal con `Table` de distribución y panel lateral de análisis de ganadoras; botón «Grabar grupo».
**Evidencia.** `P:SelectorMSViewModel.cs:26-504`, `P:SelectorMSPage.xaml:20-260`, `P:SelectorMSPage.xaml.cs:19-27`.

---

### 88. `SignosSeguidosFrm` — Signos Seguidos (condición)

**Propósito.** Condición sobre la **racha máxima de signos iguales consecutivos**: cantidades admitidas de variantes seguidas, de 1 seguidos, de X seguidos y de 2 seguidos, más listas de «figuras» (patrones de rachas) por concepto. Escribe sobre `FiltroSignosSeguidos` del grupo en edición. **Se abre** desde el botón «Signos Seguidos» de `MainPage` (`W:Views/MainPageViewModel.cs:159`); patrón §1.10.

**Entradas** (`P:SignosSeguidosFrmPage.xaml`): 4 `TextBox` «Var» `:34`, «1» `:49`, «X» `:64`, «2» `:79` (texto «p. ej. 0 1 2», defecto vacío; título «Cantidades admitidas (0–15)» `:20`); 4 botones «Figuras Variantes/Unos/Equis/Doses» `:101,118,135,152` con un indicador «Activas» visible cuando el concepto tiene figuras (`:105,122,139,156`, `VM:48-76`); Aceptar/Estadísticas/Guardar/Abrir/Copiar/Pegar/Borrar/Cancelar `:170-192`. Guardar: sugerido «SignosSeguidos»; tipos «Signos seguidos» `.seg` y «(XML)» `.xml`; temporal `Temp/tmp.seg` (`VM:95-102`).

**Acciones.**
- *Entrar* (`P:SignosSeguidosFrmPage.xaml.cs:26-38`): si **no** es `NavigationMode.Back` → `CargarDesdeGrupo()` (`VM:119-138`: `Variantes=GetVariantes()`, `Unos=GetUnos()`, `Equis=GetEquis()`, `Doses=GetDoses()`, y referencias `_figurasV/_figuras1/_figurasX/_figuras2 = filtro.FigurasV/Figuras1/FigurasX/Figuras2`; indicadores = `lista.Count > 0`); si es `Back` (vuelta de Estadísticas o del editor de figuras) solo `RefrescarFiguras()` (recalcula los 4 indicadores).
- *Figuras …* (`:283-290,306-332`): `lista ??= []`; `FigurasFiltrosFrmViewModel.FigurasEnEdicion = lista` (**por referencia**); `NombreCondicion = "SignosSeguidos"`; navega a `FigurasFiltrosFrmPage` (en esa pantalla `NombreCondicion` fija la extensión `.fig{cond}`; para Signos Seguidos "V1X2").
- *Borrar* (`:187-200`): vacía los 4 textos y pone a `null` las 4 listas locales (indicadores a falso); **no** vacía las listas del filtro del grupo.

**Lógica en el ViewModel.**
*CU-C30 `AplicarCondicionSignosSeguidos`* (`Aceptar` `VM:141-178`; temporal `:206-234`) — mismo patrón que CU-C02 con estas diferencias:
```
todos = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14"       // CONSTANTE de 15 valores (no depende de NumeroPartidos)
filtro.ReinicializaValores()                        // limpia valores y tolerancias; NO toca las listas de figuras
hayDatos = !blank(Variantes) || !blank(Unos) || !blank(Equis) || !blank(Doses)        // las figuras NO cuentan
si hayDatos: IsActive = true (solo si no tenía datos) ; ContieneDatos = true
        SetNoVariantes / SetNoUnos / SetNoEquis / SetNoDoses( texto o todos )
        para cada lista de figuras _figX: si no nula y count>0 → filtro.FigurasX = _figX      // si está vacía se deja la que ya tuviera el filtro
si no: IsActive = false ; ContieneDatos = false
grupo.ActivaFiltro(filtro) ; NotificarCambio ; volver
```
Estadísticas = CU-C03 con `FiltroSignosSeguidos` temporal; Guardar/Abrir = CU-C04 (`GetFiltro("SignosSeguidos")`, `VM:264-281`). El formato de cada texto es el de CU-C02 (tokens separados por **coma**, `N` ó `N<letra de tolerancia>`; la indicación «comas o espacios» de la UI no es cierta, `XAML:21`).

**Peculiaridades a preservar.**
- Las listas de figuras se comparten por **referencia** con el filtro del grupo: editarlas en la pantalla de figuras modifica el filtro real inmediatamente, **aunque luego se pulse Cancelar**; y «Borrar + Aceptar» no las elimina (se ignoran las listas vacías). En Swift: decidir si se corrige (copia-al-editar) o se preserva (documentar).
- `TodosValores` fijo (0…14) frente a `ObtenerTodosValores()` (0…`NumeroPartidos`) de Variantes/PesosNum.

**Área Mac propuesta.** **Condiciones y filtros** · hoja modal del editor genérico; las figuras como sub-hoja con la rejilla de casillas (pantalla `FigurasFiltrosFrm`, otra espec.).
**Evidencia.** `P:SignosSeguidosFrmViewModel.cs:28-333`, `P:SignosSeguidosFrmPage.xaml:10-192`, `P:SignosSeguidosFrmPage.xaml.cs:15-40`, `P:FigurasFiltrosFrmViewModel.cs:44-80`, `D:MotorCalculo/FiltroSignosSeguidos.cs:397-411`.

---

### 89. `SimetriasFrm` — Simetrías (condición)

**Propósito.** Condición que obliga a que ciertos grupos de partidos tengan el **mismo signo** («simetrías») y limita cuántas simetrías deben cumplirse («Aciertos»). Escribe sobre `FiltroSimetrias`. **Se abre** desde el botón «Simetrías» de `MainPage` (`W:Views/MainPageViewModel.cs:164`); patrón §1.10.

**Entradas** (`P:SimetriasFrmPage.xaml`): lista de filas (30 iniciales, `VM:62-69`): número + `TextBox` «p. ej. 1,2-4» `:42` (partidos separados por comas, guiones o mezcla «a,b-c»); botón «Añadir fila» `:51` (añade una al final; el legacy lo hacía al pulsar Intro en la última); `TextBox` «Aciertos» «p. ej. 1,3,5-7» `:64` (valores y rangos separados por comas); botones Aceptar/Estadísticas/Guardar/Abrir/Copiar/Pegar/Borrar/Cancelar `:75-97`. Guardar: sugerido «Simetrias»; tipos «Simetrias» `.sim` y «(XML)» `.xml`; temporal `Temp/tmp.sim` (`VM:49-56`).

**Acciones.**
- *Entrar* (`P:SimetriasFrmPage.xaml.cs:25-34`, `VM:75-93`): si no es `Back`: `filtro = (FiltroSimetrias)grupo.GetFiltro("Simetrias")`; **si `!filtro.ContieneDatos` no hace nada**; si no, amplía filas hasta `ArraySimetrias.Count`, `fila[i].Partidos = ArraySimetrias[i].Partidos` y `Aciertos = filtro.Aciertos`.
- *Aceptar* (`:188-209`): CU-C31. Si la entrada es inválida **vuelve sin mensaje y sin cerrar** (el legacy mostraba error).
- *Estadísticas* / *Guardar*: `ObtenerFiltroTemporal()` (`:216-236`); si es inválido muestra «Verifique que ha introducido una simetría correcta: (a,b) ó (a-b) ó (a,b-c), y que ha introducido un número válido de aciertos» y no sigue.
- *Borrar* (`:291-303`): vacía filas y aciertos **y** hace `grupo.ActivaFiltro(new FiltroSimetrias())` **inmediatamente** (un filtro vacío): «Borrar + Cancelar» ya ha eliminado la condición del grupo.
- *Abrir/Copiar/Pegar*: §1.10; `MarcarValores` limpia las filas y reescribe con `ArraySimetrias[i].Partidos` y `Aciertos` (`:239-252`).

**Lógica en el ViewModel.**
*CU-C31 `AplicarCondicionSimetrias`* (`VM:95-209`):
```
entradaValida(t):  t no vacío ; para cada trozo de t.split(','): para cada n de trozo.split('-'): v = Int(n.trim()) ; si v<=0 || v>NumeroPartidos → false ; alguno parsea → true ; cualquier excepción (vacío, texto) → false
obtenerSimetrias(): para cada fila no vacía: si !entradaValida → devolver nil ;  lista.append( Simetria( ObtenerValoresSeparadosPorComas(t) ) )
      // ObtenerValoresSeparadosPorComas("1,2-4") = "1,2,3,4" (expande a-b con ObtenerTodosValores(a,b)); si a>b da "" y Substring(0,-1) lanza → no capturado
      // Simetria(partidos) guarda el texto EXPANDIDO ; PartidosSimetricos = ints
obtenerAciertos(): vacío → []; para cada trozo de t.split(',') (trim): sin '-' → v=Int(trozo) ; si 0<=v<=40 → añadir (fuera de rango: se ignora)
                   con '-' → para j de Int(a) a Int(b) (inclusive): si 0<=j<=40 añadir ; cualquier excepción (p. ej. "-3", "a") → nil
aceptar(): si simetrias==nil || aciertos==nil : return (no cierra)
           filtro.ArraySimetrias = simetrias ; filtro.ArrayAciertos = aciertos
           filtro.Aciertos = blank(texto) ? "0" : texto            // el TEXTO original (sin expandir)
           filtro.IsActive = filtro.ContieneDatos                  // ContieneDatos = ArraySimetrias.count>0 (y limpia ArrayAciertos si no hay)
           grupo.ActivaFiltro(filtro) ; NotificarCambio ; volver
```
El filtro evalúa «aciertos» = nº de simetrías cumplidas y exige `ArrayAciertos.Contains(...)` (→ ESPEC_03); `Simetria.Partidos` se serializa expandido (`D:MotorCalculo/Simetria.cs:24-46`).
*Estadísticas*: CU-C03 con `FiltroSimetrias` temporal (`:276-287`). *Guardar/Abrir*: CU-C04 (`GetFiltro("Simetrias")`).

**Peculiaridades a preservar.**
- A diferencia de las demás condiciones, **`Aceptar` siempre reactiva** (`IsActive = ContieneDatos`) aunque el usuario la hubiera desactivado desde Inicio.
- Con «a-b» siendo a > b: `EntradaValida` lo acepta (ambos en rango) y la expansión lanza `ArgumentOutOfRangeException` sin captura (global → diálogo «Se produjo un error inesperado…»). En Swift, validar `a <= b`.
- Valores de «Aciertos» fuera de 0…40 se descartan en silencio.
- Las filas en blanco no cuentan; el orden de las filas se conserva en `ArraySimetrias`.

**Área Mac propuesta.** **Condiciones y filtros** · hoja modal del editor genérico con lista editable de simetrías (cada una como *token field* de partidos 1…14 con validación en vivo) y campo de aciertos.
**Evidencia.** `P:SimetriasFrmViewModel.cs:29-311`, `P:SimetriasFrmPage.xaml:19-97`, `P:SimetriasFrmPage.xaml.cs:14-35`, `P:FilaSimetria.cs:69-85`, `D:MotorCalculo/FiltroSimetrias.cs:36-60,134-175`, `D:MotorCalculo/Simetria.cs:24-54`, `D:Utils/UtilidadesEntradasValores.cs:341-361`.

---

### 90. `StaInterFrm` — Interrupciones (visor de estadística 5 × 14)

**Propósito.** Muestra la matriz de **interrupciones** calculada por la herramienta *Anastatics* (`Estadísticas`, modo «Interrupciones»): 5 agregados (globales, cant. 1, cant. X, cant. 2, cant. V) × 14 partidos, como porcentaje o recuento. **Se abre** desde *Anastatics* → «Mostrar» (modo 2) (`P:AnastaticsViewModel.cs:190`); no figura en menús.

**Entradas.** `ComboBox` «Valores» (`porcentajes` | `columnas`; índice por defecto 0) `P:StaInterFrmPage.xaml:24`. Sin más acciones.
**Datos de entrada (handoff).** `StaInterFrmViewModel.MatrizEntrada: Int32[15,15]` y `NumColEntrada` (nº total de columnas analizadas) — los deja *Anastatics* (clon de su matriz `rsl`, `P:AnastaticsViewModel.cs:186-195`); el ctor los vuelca si existen; usa solo filas 0…4 y columnas 0…13 (`P:StaInterFrmViewModel.cs:42-45,95-118`).
**Salidas.** Cabecera de columnas «0»…«13»; filas «globales», «cant. 1», «cant. X», «cant. 2», «cant. V» (`VM:48-60`); «Nº columnas» = `NumColumnas.ToString()` (`XAML:30-31`); celdas `string` (inicial «-», `VM:90`).

**Lógica en el ViewModel.**
*CU-C32 `FormatearMatrizEstadistica`* (`Repintar` `P:StaInterFrmViewModel.cs:125-149`):
```
divisor = (numColumnas == 0) ? 1 : numColumnas                       // el legacy asumía numcol>0
si modo == "porcentajes":  celda = ( (rsl[f][c] * 10000) / divisor ) / 100.0     // `rsl*10000` en Int32 con WRAP; `/divisor` entero (trunca); luego Double
                           texto = celda.ToString()                  // cultura actual: "12,34" (es-ES), sin ceros finales: 12.5 → "12,5"; 12 → "12"
si no:                     texto = String(rsl[f][c])
```
**Peculiaridad crítica (T1).** `rsl[f,c] * 10000` es `int * int` **sin `checked`**: con `rsl > 214 748` desborda y da porcentajes erróneos (negativos o absurdos) en reducciones grandes. Para paridad de bytes usar `Int32(truncatingIfNeeded: Int64(r) &* 10000)` (`&*` sobre `Int32`); decidir con el dueño si se corrige (usar `Int64`). Idéntico en `StaSSForm` y probablemente en `DibForm`/`DibRepFrm` (otras espec.).
**Área Mac propuesta.** **Análisis** · inspector/hoja con `Grid` de 5×14 y selector «porcentajes/columnas» (`Picker(.segmented)`), abierta desde la pantalla de Estadísticas.
**Evidencia.** `P:StaInterFrmViewModel.cs:30-150`, `P:StaInterFrmPage.xaml:12-172`, `P:StaInterFrmPage.xaml.cs:15-23`, `P:AnastaticsViewModel.cs:149-195`.

---

### 91. `StaSSForm` — Signos seguidos (visor de estadística 4 × 15)

**Propósito.** Visor de la estadística *Anastatics* modo «Signos seguidos»: para cada tipo (1, X, 2, V) cuántas columnas tienen una racha de longitud `0…14`. **Se abre** desde *Anastatics* → «Mostrar» (modo 3) (`P:AnastaticsViewModel.cs:198`).

**Entradas.** `ComboBox` «Modo de visualización» (`porcentajes` | `columnas`, defecto 0) `P:StaSSFormPage.xaml:37`. **Handoff:** `StaSSFormViewModel.MatrizEntrada` (`Int32[15,15]`, filas 0…3 = 1/X/2/V, columnas 0…14) y `NumColEntrada` (`P:StaSSFormViewModel.cs:56-59,81-84`).
**Salidas.** Cabecera «0»…«14» (`VM:44,68-71`); filas «cant. 1», «cant. X», «cant. 2», «cant. V» (`:47`); «Columnas analizadas» = `numcol.ToString()` (defecto «-», `:102`); celdas `string`.
**Lógica en el ViewModel.** Idéntica a CU-C32 (porcentaje = `((rsl*10000)/divisor)/100.0` con `divisor=1` si `numcol==0`, o recuento bruto; `Recalcular` `:131-154`) — mismo riesgo de desbordamiento `Int32`. Se reconstruye la lista `Filas` completa al cambiar de modo (`:104-108`).
**Área Mac propuesta.** **Análisis** · mismo visor que la pantalla 90 parametrizado (`MatrizEstadisticaView(filas:, columnas:, etiquetas:)`).
**Evidencia.** `P:StaSSFormViewModel.cs:14-155`, `P:StaSSFormPage.xaml:17-130`, `P:StaSSFormPage.xaml.cs:13-21`.

---

### 92. `SubirCategoriaFrm` — Subir categoría

**Propósito.** Toma un fichero de columnas y añade todas las que difieren en 1…n signos de alguna (en los partidos «involucrados»), según los **niveles** marcados, eliminando repetidas; sube la categoría de premios de una combinación. **Se abre** desde Utilidades ▸ Sube categoría… / botón «Subir categoría» (`W:MainWindow.xaml.cs:251,417`).

**Entradas** (`P:SubirCategoriaFrmPage.xaml`):

| Control | Tipo / rango | Defecto / habilitación |
|---------|--------------|------------------------|
| «Origen (txt)» `:55` (picker `.txt`, `OnSeleccionarOrigen` en el code-behind `P:SubirCategoriaFrmPage.xaml.cs:26-34`) | fichero | siempre activo |
| «Salida (txt)» `:67` (picker guardar `.txt`, sugerido «columnas», tipo «Columnas», `:37-44,67-74`) | fichero | habilitado tras elegir origen (`PuedeElegirSalida`) |
| 16 `CheckBox` «Partidos involucrados» 1…16 `:100` («desmarcando alguno, se arriesga la garantía» `:94`) | bool | deshabilitadas al inicio; al cargar origen con `N` signos se habilitan y marcan las `N` primeras (`VM:127-137`) |
| «Seguidos:» `NumberBox` 0…16 `:112` | entero | 0; en «Calcular» se recorta a `numPartidos` |
| «Usar combinación» `:124` + etiqueta (fichero de columnas externas, picker `.txt`) | fichero opcional | vacío = sin restricción |
| 17 `CheckBox` «Niveles» mostrados de 16 a 0 `:143` | bool | nivel 0 siempre habilitado; luego uno por cada partido marcado (nivel k habilitado si hay ≥ k marcados), **sin marcar** |
| «Calcular» `:152` (con `PuedeCalcular`), «Grabar» `:157` (con `PuedeGrabar`) | botones | |

**Salidas.** `ConteoColumnas` `:161` (`"leyendo..."` → `"I=<n>"` tras cargar; `"F=<n>"` tras calcular; `"grabando..."` → `"G=<n>"`) y `Estado` `:165`: «Selecciona un archivo de origen» (inicial), «Leyendo...», «Archivo de origen cargado», «Error al leer el archivo» (+ diálogo «Error al leer el archivo: {msg}»), «Listo para calcular», «No se ha seleccionado ningún nivel», «Calculando...», «Calculado», «Falta el archivo de salida», «Grabando...», «Grabado», «Error: {msg}» (`VM:110,230-235,264,275,295,305,314,338-362,371`).

**Lógica en el ViewModel** (el cálculo real está en `D:SubirCategoria/Calculos.cs`, ESPEC_05):
*CU-C33 `PrepararSubirCategoria`* (`OnArchivoOrigenSeleccionadoAsync` `VM:335-364`): en segundo plano `aCol = ArchivoColumnasTexto(ruta)`; `s = ObtenNumSignos()`; `motor = Calculos(ruta)` (lee todas las columnas a índices con `ConvColumnaANumero(col, s)`; `motor.NoColumnas` = nº de líneas leídas, duplicados incluidos); `noSignos = numPartidos = s`; `ConteoColumnas = "I=" + motor.NoColumnas`; `AdaptarInterfaz(s)`; `PuedeElegirSalida = true`, `PuedeCalcular = PuedeGrabar = false`.
*Regla de habilitación de niveles* (`AdaptarInterfaz/AdaptarNiveles` `:127-163`, `Partido_PropertyChanged` `:200-207`):
```
AdaptarNiveles(): todos los niveles → {habilitada:false, marcada:false}      // ¡borra las marcas del usuario!
                  nivel[0].habilitada = true ; k = 1 ; para cada partido marcado: nivel[k].habilitada = true ; k += 1
al cambiar la marca de CUALQUIER partido → AdaptarNiveles()                  // por tanto cambiar un partido desmarca todos los niveles
```
*CU-C34 `CalcularSubirCategoria`* (`Calcular` `:226-285`):
```
si motor == nil → Estado "Selecciona un archivo de origen" ; si ningún nivel marcado → "No se ha seleccionado ningún nivel"
si Seguidos > numPartidos { Seguidos = numPartidos }
involucrados = Bool[16] (marca de cada partido) ; niveles = Bool[17] indexado 0…16 (la colección está invertida)
PuedeCalcular = PuedeGrabar = false ; Estado = "Calculando..."
en segundo plano: motor.Calcular(involucrados, niveles, Int(Seguidos), ArchivoExternas, noSignos) ; n = motor.NoColumnas
ConteoColumnas = "F=\(n)" ; PuedeGrabar = true ; Estado = "Calculado"       // finally: PuedeCalcular = true
```
*CU-C35 `GrabarSubirCategoria`* (`Grabar` `:291-325`): si no hay motor o ruta de salida → «Falta el archivo de salida»; `motor.Grabar(ruta)` (`ArchivoColumnasTexto(ruta, numPartidos).GuardarTodasCols(Bits)` → una línea por columna marcada, orden ascendente de índice); `ConteoColumnas = "G=\(motor.NoColumnas)"`; `Estado = "Grabado"`.

**Peculiaridades a preservar / decisiones.**
- Límite del motor: los `BitArray` son de `3^15` (14 348 907) ⇒ **máximo 15 signos** por fichero (el VM ofrece 16 casillas).
- Los niveles se pierden al tocar cualquier casilla de partido (efecto lateral de `AdaptarNiveles`); mantenerlo o recordar la selección es una decisión de UX (R2).
- `ArchivoExternas = ""` ⇒ el motor marca todas las columnas como externas (sin restricción).
- Los selectores de fichero de esta página **no usan `PickerHelper`** (usan `App.MainWindow` directo, `:56-86`): mismas extensiones y ubicación `DocumentsLibrary`.

**Área Mac propuesta.** **Reducción** (o Herramientas) · vista principal con asistente de 3 pasos (Origen → Parámetros → Salida) y resumen «I/F/G»; la rejilla de partidos/niveles como `Toggle` en `LazyVGrid`.
**Evidencia.** `P:SubirCategoriaFrmViewModel.cs:19-379`, `P:SubirCategoriaFrmPage.xaml:8-165`, `P:SubirCategoriaFrmPage.xaml.cs:16-87`, `D:SubirCategoria/Calculos.cs` (completo).

---

### 93. `TramificarForm` — Tramificar

**Propósito.** Ordena las 4 782 969 columnas por proximidad a un valor central de valoración (LN central), las reparte en **tramos** de igual nº de columnas y cuenta en cada tramo cuántas columnas premiadas hay (14…10 aciertos frente a una columna ganadora de una jornada), con importes y balance. Permite filtrar por posiciones mínimas/máximas de cada categoría, grabar el filtro, navegar a columnas concretas, guardar la jornada L.A.E. y ver gráficas. **Se abre** desde Utilidades ▸ Tramificar… / botón «Tramificar» (`W:MainWindow.xaml.cs:264,426`).

**Entradas** (`P:TramificarFormPage.xaml`):

| Bloque | Control | Tipo / rango | Defecto |
|--------|---------|--------------|---------|
| Definición del tramo `:41-62` | «Columna inicial» / «Columna final» / «Columnas / tramo» (`NumberBox`) | `double` | 0 / 4 782 969 / 531 441 |
| | «Nº de tramos» (combo) | `9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441` (`VM:133-136`) | `"9"` |
| Datos L.A.E. `:91-111` | «Columna premiada» (mayúsculas, máx. 14) | texto | `1X1222X2X12121` |
| | «Temporada» (máx. 14) / temporada fin (solo lectura, **fija**) | texto | `2004` / `2005` |
| | «Jornada» | `NumberBox` 1…70 | 7 |
| | «Recaudación» | texto | `7768049` |
| Premios `:141-180` | importes 14…10 (texto con coma decimal) + interruptor «Bloq.» por categoría | texto / bool | `1165207,35`, `29877,11`, `1753,51`, `165,38`, `22,79` (los interruptores **no se usan** en ningún cálculo) |
| «Guardar jornada L.A.E.» `:185` | botón | | |
| Valoración por partido `:197` | `PorcentajesControl` | `NumeroPartidos×3` | 0 |
| «Guardar en histórico de valoraciones» `:202` | botón | | |
| Posiciones mínimas y máximas `:240-283` | `Min14/Max14 … Min10/Max10` | texto (admite `%`) | vacío; se rellenan tras tramificar |
| | «Nº columnas filtro» (solo lectura) `:295` | texto | `"0"` |
| | «Filtrar» `:300`, «Grabar filtro» `:303`, «Leer límites» `:306`, «Guardar límites» `:309` | botones | |
| Ver columna `:335-368` | «Columna» (editable, mayúsculas), «Aciertos» (solo lectura), «Aciertos a buscar» (combo 14,13,12,11,10), «Valoración / LN» (solo lectura), «Anterior», «Siguiente», «Buscar columna» | | aciertos a buscar `"14"` |
| Valoración `:387-396` | «LN central» (texto), interruptor «Ordenar por productos (logaritmos neperianos)» / «Ordenar por sumas», interruptor «Acumular resultados de jornadas distintas» | | `"0"`; productos; no |
| Resultados `:426-482` | lista de tramos: Nº, Col. Inf., Col. Sup., Nº Cols., Prob. Max., 14, 13, 12, 11, 10, NºPremios, Imp.Premios, Ingresos-Gastos | solo lectura | |
| Acciones `:489-508` | «Ver gráficas», «Análisis múltiple», «Tramificar», «Grabar tramos», «Cancelar» | | |

**Estados** (barra `:18`): «Faltan datos» (inicial), «Calculando ...», «Escrutando ...», «Calculando probabilidades ...», «Ordenando apuestas ...», «Tramificando apuestas ...», «Finalizado», «Error en el cálculo de tramos.», «Primero hay que tramificar para escrutar las columnas.», «Primero hay que tramificar.», «Primero hay que tramificar y filtrar.», «Eliminando columnas ...», «Guardando columnas ...», «Grabadas {n} columnas», «Límites guardados.», «Límites cargados.», «Datos de la jornada L.A.E. guardados.», «No hay tramos calculados que representar.».

**Lógica en el ViewModel** (motor «transcrito literalmente» de `TramificarForm.cs`, `VM:75-79`). Estructuras del dominio: `ApuestaProbEscrutada {Probabilidad: Double, Columna: Int32, Aciertos: UInt8}` (`D:Analisis/ApuestaProbEscrutada.cs:24-29`), `Tramo` (`D:Utils/Tramo.cs:24-123`). **Aquí `Probabilidad` y el acumulado del DFS son `Double`** (a diferencia de las pantallas 76 y 83, que usan `Float`).

*Reacciones de la UI* (`VM:111,138-166,278-293`): cambiar «Nº de tramos» → `ColumnasPorTramo = floor(ColumnaFinal / nTramos)` y `Acumular = false`; cambiar «Columna final» → `ColumnasPorTramo = floor(ColumnaFinal / nTramos)`; editar cualquier valoración o la columna premiada → `escrutado = false`; «Aciertos a buscar» → `numAciertosABuscar`; «LN central» → `_LN = parse(texto)` (si `≠ "-"`; **no** invalida el escrutinio).
*CU-C36 `TramificarColumnas`* (`Tramificar` `:345-372`, `Tramifica` `:375-399`, `OrdenaPorProductos/Sumas` `:402-442`, `Calcula14Triples` `:445-467`):
```
ColumnaFinal = min(ColumnaFinal, 4 782 969)                                  // (dispara la recomputación de columnas/tramo)
si Acumular: escrutado = false ; afegir = true  sino: afegir = false ; MaxiMin[i] = (max:0, min:4 782 969) para i<5
productos: v = valoraciones ; p = Porcentajes(v).ValoresNeperianos()  (Float[14,3]) ; Prob = Σ p[i,0] (Float)  ;  Cr[i,z] = Double(p[i,z] - p[i,0]) z∈{1,2}   // resta en Float, luego a Double
sumas:     p[i,j] = Float(v[i,j]) ;  Prob = Σ p[i,0] ; Cr[i,z] = Double(p[i,z]-p[i,0])
Calcula14Triples(Double(Prob)):
   si !escrutado:
        Escrutar(): Ap14T[k] = {Aciertos:0, Columna:k, Prob:0} ∀k ; num = ConvColumnaANumero(ColumnaPremiada) ; Ap14T[num].Aciertos = 14 ;
                    ColumnasADistancia1(num, 0, 4): DFS por partidos crecientes cambiando 1 signo por nivel: Aciertos[col] = 14 - profundidad (13,12,11,10) ; escrutado = true
        Profundidad = 0 ; EncontrarDistantes1(Prob, 0, 0, 14)     // Prob_acum = pProb + Cr[partido][z] (z∈{1,2}) en Double ; Ap14T[idx] = {Columna:idx, Probabilidad: |Prob_acum - _LN|}
        Ap14T[0] = {Columna:0, Probabilidad: |Prob - _LN|}
        ordena(0, 4 782 968)                                      // el MISMO quicksort propio de CU-C06, ascendente por Probabilidad (|valor - LN|)
   (EliminaColumnas() es un no-op en la versión portada)
   si afegir: TramificarAdd() sino TramificarTramos()
tras terminar (en UI): PonerMaximosiMinimos: MinK = String(1 + MaxiMin[k][1]) ; MaxK = String(1 + MaxiMin[k][0])  (K=14…10 ↔ k=0…4) ; refrescar rejilla ; cursor "Ver columna" = 1 ; Estado "Finalizado"
```
*`TramificarTramos`* (`:563-620`):
```
premios[0…4] = ToDouble(Premio14…Premio10)      // ToDouble: vacío→0 ; parse con cultura actual (NumberStyles.Any) ; si falla, parse invariante de texto con ',' → '.' ; si falla → 0
tramos = [] ; para nr = Int(Round-even(ColumnaInicial)); nr < Int(Round-even(ColumnaFinal)); nr += Int(Round-even(ColumnasPorTramo)):      // Convert.ToInt32(double) = redondeo bancario ; incremento 0 → BUCLE INFINITO
    tr = Tramo(premios) ; c = 0 ; c2 = 0 ; tr.ValorIzquierda = nr ; aciertos = [0]*5
    para nc = nr; nc < nr + incremento (comparación en Double); nc += 1:
        si nc > 4 782 968 : romper
        acumProb = -Ap14T[nc].Probabilidad                                   // se queda con el de la ÚLTIMA columna del tramo (aunque esté eliminada)
        si !Bits[Ap14T[nc].Columna]:                                         // Bits = columnas eliminadas por el filtro
            c += 1 ; si Ap14T[nc].Aciertos > 9 : k = 14 - Aciertos ; aciertos[k] += 1 ; tr.ColumnasPremiadas += 1 ; MaxiMin[k][0] = max(MaxiMin[k][0], nc) ; MaxiMin[k][1] = min(MaxiMin[k][1], nc)
        sino c2 += 1
    tr.ProbAcumulada = acumProb ; tr.NumColumnasTramo = c ; tr.ValorDerecha += tr.ValorIzquierda + c + c2 - 1       // (ValorDerecha empieza en 0)
    tr.PonerAciertos(aciertos) ; tr.NumeroDeTramo = tramos.count + 1 ; tr.CalculaTotalImportePremios() ; tramos.append(tr)
Tramo.CalculaTotalImportePremios(): premiosAcum = Σ_{i<5} aciertos[i] * premios[i] ; balance = premiosAcum - numColumnasTramo * 0.5      // coste fijo 0,5 € (D:Utils/Tramo.cs:112-118)
```
*`TramificarAdd`* (`:623-682`): igual pero (a) `acumProb` y `c++` solo se actualizan para columnas **no eliminadas**; (b) tras `CalculaTotalImportePremios`, `if numTramo < previos.count { tr.AddTramo(previos[numTramo]) }` (suma aciertos P14…P10, `ColumnasPremiadas`, importe y balance del tramo homónimo previo, `D:Utils/Tramo.cs:51-61`); `NumeroDeTramo = tramos.count + 1`; no reinicia `MaxiMin`.
*Presentación de la fila* (`TramoFilaViewModel` `:28-61`): todos los campos `ToString(CultureInfo.CurrentCulture)` — enteros sin separadores; `Prob. Max.`, `Imp.Premios` e `Ingresos-Gastos` son `Double` con formato por defecto (ida y vuelta más corta, **sin redondeo**).
*CU-C37 `FiltrarPorLimites`* (`Filtrar` `:704-724`, `ObtenerExtremos` `:774-807`, `EjecutarFiltro` `:755-771`, `EliminarColumnas` `:810-826`): requiere `Ap14T` cargado (si no → «Primero hay que tramificar para escrutar las columnas.»). `Bits.SetAll(true)`; `Bits[Ap14T[x].Columna] = false ∀x` (nada eliminado); `extremos[10][4]`:
```
[0]=(0, Min10-1) [1]=(Min10-1, Min11-1) [2]=(Min11-1, Min12-1) [3]=(Min12-1, Min13-1) [4]=(Min13-1, Min14-1)
[5]=(Max14, Max13) [6]=(Max13, Max12) [7]=(Max12, Max11) [8]=(Max11, Max10) [9]=(Max10, 4 782 968)          // columnas 2 y 3 (difMin, difMax) las rellena DialogoFiltrarPorLimitesFrm
ToInt(texto) = vacío→0 ; quitar "%" ; Int.parse (cultura actual, NumberStyles.Any) ; si no, Int(Double.parse) ; si no 0
```
y navega a la hoja `DialogoFiltrarPorLimitesFrmPage` (otra espec.); si el usuario acepta → `EjecutarFiltro(extremos)`:
```
para i en 0..<10:  si extremos[i][0] > 0 { extremos[i][0] -= 1 }                    // muta el array
   para x en extremos[i][0] ..< extremos[i][1]:
        si extremos[i][2] == 0 : Bits[Ap14T[x].Columna] = true                       // eliminar la propia columna de la posición x
        si extremos[i][3] > 0  : EliminarColumnas(Ap14T[x].Columna, 0, extremos[i][3])    // marca Bits=true en todas las columnas a distancia 1…difMax (DFS por partidos crecientes, `Profundidad` compartida)
total = nº de nr<3^14 con Bits[nr] == false → «Nº columnas filtro»
```
*Ver columna* (`:874-957`): `Siguiente`: `desde = posicionActual` (0-based = siguiente posición); para `i` desde `desde` hasta `N-1`: saltar si `Bits[Ap14T[i].Columna]`; si `Ap14T[i].Aciertos >= numAciertosABuscar` → `posicionActual = i+1`, actualizar, parar (si no hay, no se mueve). `Anterior`: `i` desde `posicionActual-2` hacia abajo mientras `i > 0` (el índice 0 no se explora), misma condición. `Buscar columna`: `num = ConvColumnaANumero(texto)`; primera `i` con `Ap14T[i].Columna == num` → `posicionActual = i+1`. `ActualizarColumnaEnPosicion`: `columna = ConvNumAColumna(Ap14T[pos].Columna)`; `Valoración/LN = (-Ap14T[pos].Probabilidad).ToString()` (Double, cultura); `Aciertos` = nº de posiciones donde `ColumnaPremiada[i] == columna[i]` (calculado de nuevo, no es `Ap14T.Aciertos`).
*Grabar filtro* (`:829-871`): picker «Columnas» `.txt`; escribe `ConvNumAColumna(nr)` (`GuardarCols`) para `nr` ascendente con `Bits[nr] == false` → «Grabadas {n} columnas».
*Guardar/Leer límites* (`:960-1017`): `.txt` de 10 líneas `Min14, Max14, Min13, Max13, Min12, Max12, Min11, Max11, Min10, Max10`; al leer, línea ausente → cadena vacía.
*CU-C38 `GuardarJornadaLAE`* (`:1037-1093`, `MontaLinea` `:1199-1231`): fichero `<AppDir>/Jornadas/InfoJornadasLAE.txt`; `jornada = Int(Jornada)` rellena a 2 dígitos (`"07"`); `temporada = Temporada + "/" + TemporadaFin`; lee las líneas existentes (separadas por TAB, `LeeColumnaSinComas`); si una línea tiene > 2 campos y `campo[1] == temporada && campo[2] == jornada` la **reemplaza**; si no, añade al final; reescribe todo el fichero (`GuardarColsComa` = `WriteLine`). Línea nueva (separador `\t`): `ColumnaPremiada` · `temporada` · `jornada` · `Recaudacion/0.5` · `Premio14/0.5` · `Premio13/0.5` · `Premio12/0.5` · `Premio11/0.5` · `Premio10/0.5`, cada importe con `Double.ToString(cultura).Replace(".", ",")`; `Recaudacion` vacío → `14000000`; `0.5` = constante `PrecioApuesta` (`:84`) ⇒ importe por apuesta de 1 €/columnas apostadas (→ ESPEC_07 formato `InfoJornadasLAE`).
*Navegación auxiliar*: «Ver gráficas» → handoff `TramificarGraficasFrmViewModel.TramosAnalizados = tramos` (pantalla 94; sin tramos → aviso); «Grabar tramos» → `DialogoGrabarTramosFrmPage` (sin datos; la selección de tramos de la rejilla no se transmite); «Análisis múltiple» → `DialogoAnalisisMultipleDeTramosFrmPage`; «Guardar en histórico» → `HistoriaValoracionesFrmPage.Contexto = (valoraciones, temporada(Int, 0 si inválida), jornada, nil)`; «Cancelar» → `Volver`.

**Peculiaridades a preservar / trampas Swift.**
- `escrutado` no se invalida con «LN central» ni con el interruptor productos/sumas: repetir «Tramificar» tras cambiarlos **reutiliza el orden anterior** (idéntico al legacy `TramificarForm.cs:3425-3428`).
- `Convert.ToInt32(Double)` = redondeo bancario y `Int(Double.nan)` aborta en Swift; guardar contra `ColumnasPorTramo ≤ 0` (bucle infinito en el C#).
- La barra «Prob. Max.» es **el valor de la última columna del tramo**; en acumulación solo de columnas no eliminadas.
- `Tramo.Balance` usa `0.5` fijo, no el precio de apuesta configurado (`D:Utils/Tramo.cs:117`).
- `MaxiMin` queda en `(0, 4 782 969)` para categorías sin aciertos ⇒ `Max=1`, `Min=4 782 970` tras `PonerMaximosiMinimos`.
- Memoria: `Ap14T` ≈ 4,78 M × 16 B; `Bits` y la tabla de DFS: usar buffers planos y `async` cancelable con progreso (T12). `TemporadaFin` es un literal («2005») que no sigue a «Temporada».
- Cultura: parseo de importes con la cultura actual y *fallback* invariante; escritura de `InfoJornadasLAE.txt` con coma decimal forzada (T7).

**Área Mac propuesta.** **Análisis** · vista principal con 3 paneles (Parámetros, Resultados `Table`, Ver columna) más hojas para Filtrar/Grabar tramos/Análisis múltiple; las gráficas en inspector (pantalla 94).
**Evidencia.** `P:TramificarFormViewModel.cs:28-1260`, `P:TramificarFormPage.xaml:12-508`, `P:TramificarFormPage.xaml.cs:16-52`, `D:Utils/Tramo.cs:24-123`, `D:Analisis/ApuestaProbEscrutada.cs:24-29`, `Free1X2/UI/TramificarForm.cs:3425-3428`.

---

### 94. `TramificarGraficasFrm` — Gráficas de los resultados del análisis de tramos

**Propósito.** Dibuja, sobre un lienzo común, las curvas de la lista de tramos calculada en *Tramificar*: probabilidad acumulada, aciertos 14…10, columnas premiadas, importe de premios y balance. **Se abre** con «Ver gráficas» de Tramificar (`P:TramificarFormViewModel.cs:1118`).

**Entradas** (`P:TramificarGraficasFrmPage.xaml`): `ComboBox` «Tipo de curva» `:34` (9 opciones, defecto «Probabilidad acumulada», `VM:37,89-100`); `NumberBox` «Tamaño del grid» 10…200 `:41` (defecto 50, alimenta `IntervaloGrid` del lienzo); «Dibujar curva» `:51`, «Limpiar imagen» `:56`, «Copiar al portapapeles» `:60`; leyenda de colores `:82-121`; lienzo `GraficoLineasControl` `:129`.
**Datos de entrada (handoff).** `TramificarGraficasFrmViewModel.TramosAnalizados: [Tramo]` (lista de solo lectura dejada por Tramificar; **no se borra** al consumirla) (`P:TramificarGraficasFrmViewModel.cs:32`).

**Lógica en el ViewModel.**
*CU-C39 `PuntosDeCurvaDeTramos`* (`Dibujar` `VM:103-152`): sin tramos → «No hay tramos analizados que representar.». Para cada tramo `tr` (en orden): `x = tr.NumeroDeTramo - 1` y `y` **entero truncado**:
| Curva | `y` |
|-------|-----|
| Probabilidad acumulada | `Int(35 + tr.ProbAcumulada)` (trunca hacia 0) |
| 14 / 13 / 12 / 11 / 10 aciertos | `tr.P14 … tr.P10` |
| Nº de aciertos / columnas premiadas | `tr.ColumnasPremiadas` |
| Importe de premios | `Int(tr.TotalImportePremios)` |
| Balance (ingresos - gastos) | `Int(tr.Balance)` |
La curva se **añade** a `Curvas` (si ya existía una del mismo nombre se sustituye; el legacy superponía hasta «Limpiar»). Estado: `"Curva '{nombre}': {n} puntos dibujados."`.
Colores de trazo (`ColorDeCurva` `:72-84`): Probabilidad `#000000`; 14 `#1E90FF` (DodgerBlue); 13 `#A52A2A` (Brown); 12 `#00FFFF` (Cyan); 11 `#008000` (Green); 10 `#FF0000` (Red); Columnas premiadas `#5F9EA0` (CadetBlue); Importe `#228B22` (ForestGreen); Balance `#FFA500` (Orange). **No siguen el tema** (identifican la curva).
*Limpiar* (`:155-164`): vacía puntos y curvas; estado «Imagen limpiada. Selecciona una curva para volver a dibujar.».
*Copiar* (`:166-180`, `P:TramificarGraficasFrmPage.xaml.cs:36-92`): sin curvas → «No hay ninguna curva dibujada que copiar.»; si no, renderiza el lienzo a bitmap BGRA8, lo codifica a PNG, lo pone en el portapapeles (`SetBitmap`) → «Imagen de la gráfica copiada al portapapeles.»; **además** ofrece guardarlo (picker en *Imágenes*, sugerido `grafica_tramos`, tipo «Imagen PNG» `.png`) → «Imagen de la gráfica guardada en {nombre}.». Error → «No se ha podido copiar la imagen de la gráfica: {msg}».
El dibujo de ejes/escala/marcadores lo hace `GraficoLineasControl` (§1.9): escala común a todas las curvas, Y invertida, marcadores si ≤ 64 puntos.

**Peculiaridades / trampas Swift.** `(int)` de `Double` no finito es indefinido en C# y aborta en Swift (`Int(exactly:)`/saturar); la truncación (no redondeo) es parte del comportamiento. Con la curva «Probabilidad acumulada» el `+35` es un desplazamiento fijo del legacy (los valores son LN negativos).
**Área Mac propuesta.** **Análisis** · inspector/hoja con **Swift Charts** (`LineMark` por curva, `PointMark` si ≤ 64, `ChartProxy` para exportar) y `ImageRenderer` para copiar/guardar PNG (`NSPasteboard` + `NSSavePanel`).
**Evidencia.** `P:TramificarGraficasFrmViewModel.cs:24-182`, `P:TramificarGraficasFrmPage.xaml:17-129`, `P:TramificarGraficasFrmPage.xaml.cs:20-93`, `W:Controls/GraficoLineasControl.xaml.cs:42-346`.

---

### 95. `TransposicionFrm` — Transposición de columnas

**Propósito.** Reordena los 14 signos de cada columna de un fichero según una **permutación** (para cada posición de salida, de qué posición de la entrada se toma el signo). **Se abre** desde Operaciones ▸ Transposición… / botón «Transposición» (`W:MainWindow.xaml.cs:245,381`).

**Entradas** (`P:TransposicionFrmPage.xaml`): «Seleccionar…» entrada `:56` (picker `.txt`) y salida `:82` (picker guardar `.txt`, sugerido «ColumnasTranspuestas», tipo «Columnas»); 14 `NumberBox` «Salida 1…14» `:107-120`, `Minimum=1`, `Maximum=14`, defecto = identidad `1,2,…,14` (`VM:37-63`); «Cancelar» `:144` (**no hace nada**, `VM:186-191`); «Transponer» `:148` (deshabilitado mientras corre).
**Mensajes** (campo «Estado» `:131`): «Error en condiciones» (también diálogo), «Seleccione los archivos de entrada y salida.», «Procesando...», «Transposición completada.», «Error: {msg}».

**Lógica en el ViewModel.**
*CU-C40 `TransponerColumnas`* (`Transponer` `P:TransposicionFrmViewModel.cs:113-181`):
```
orde[i] = Int(Pos_i) - 1                                   // truncado; fuera de 0…13 → inválido
valido = todos en 0…13  &&  los 14 valores distintos (cada 1…14 exactamente una vez)
si !valido: Estado = "Error en condiciones" ; diálogo "Error en condiciones" ; return                   // se valida ANTES que los ficheros
si falta entrada o salida: Estado = "Seleccione los archivos de entrada y salida." ; return              // sin diálogo
PuedeTransponer = false ; Estado = "Procesando..."
en segundo plano: para cada línea del fichero de entrada (mientras queden caracteres):
        columna = línea ;  salida = String( (0..<14).map { columna[orde[$0]] } )  ;  escribir salida + fin de línea
Estado = "Transposición completada."  (errores → "Error: {msg}")  ; PuedeTransponer = true
```
**Peculiaridades / trampas Swift.** Una línea con < 14 caracteres (p. ej. vacía) lanza `IndexOutOfRange` y **aborta la transposición** dejando el fichero de salida parcial («Error: …»); una línea con > 14 caracteres se trunca a las 14 posiciones elegidas (los extras se pierden). El resultado conserva el orden de las líneas, no deduplica ni cambia el signo. `(int)NumberBox.Value` con `NaN` debe tratarse como inválido (`Int(exactly:)`). Escritura `StreamWriter` UTF-8 sin BOM, `\r\n` (T8).
**Área Mac propuesta.** **Herramientas** · vista principal con dos selectores de fichero y una rejilla 2×7 de `Stepper`/`Picker` (con validación de permutación en vivo y botón «Invertir»/«Identidad» opcionales).
**Evidencia.** `P:TransposicionFrmViewModel.cs:27-192`, `P:TransposicionFrmPage.xaml:20-148`, `P:TransposicionFrmPage.xaml.cs:12-20`.

---

### 96. `TriosFrm` — Tríos (herramienta fichero → fichero)

**Propósito.** Variante de la pantalla 77 (*Pares*) con **tríos** de partidos: 12 filas, cada una con 3 posiciones (P1,P2,P3) y un nivel 1..5 para cada una de las 27 combinaciones de signos del trío; por nivel se exige un número de apariciones en [min, max]. Filtra un fichero de columnas y graba las válidas. **Se abre** desde la pantalla *Formatos* (`P:FormatosFrmPage.xaml.cs:59`); no figura en menús ni barra.

**Entradas** (`P:TriosFrmPage.xaml`):
- Tabla «Límites» `:44-137`: 5 niveles × (`Mín`, `Máx` en `NumberBox`) + columna «Resultado» (solo lectura, «-» inicial); defecto **0 / 270** en todos (`P:TriosFrmViewModel.cs:150-173`).
- Matriz «Niveles» `:160-226`: 12 filas × [`P1`,`P2`,`P3`, 27 `TextBox` con cabeceras `111, 11X, 112, 1X1, 1XX, 1X2, 121, 12X, 122, X11, X1X, X12, XX1, XXX, XX2, X21, X2X, X22, 211, 21X, 212, 2X1, 2XX, 2X2, 221, 22X, 222`] (texto, defecto `"0"`; `P:TriosFrmViewModel.cs:24-58`). Nota: la ayuda dice «nivel (1-5)».
- Botones «Calcular» `:241`, «Analizar» `:245`, «Grabar Resultado» `:248`, «Abrir Condiciones» `:253`, «Salvar Condiciones» `:256`, «Cancelar» `:259` (solo aborta el cálculo en curso, no navega). Indicadores «Procesadas», «Válidas», «Tiempo» `:278-291` (defecto `"0"`).
- `ColumnaGanadora` (defecto `"111X11X222X111"`, `VM:204-206`) **no tiene control en el XAML** (no hay `TextBox` enlazado): «Analizar» siempre analiza esa columna fija (en *Pares* sí hay campo, `P:ParejasFrmPage.xaml:170`). Discrepancia con el legacy (`tCG`) → «Abiertas».

**Acciones y lógica en el ViewModel.**
- *Calcular* (`:211-253`): picker `.txt`/`*` → `RecuperarPantalla()` → en segundo plano recorre las líneas (comprobando `_salida`); `ctproc++`; si `Valida(línea)` → `bits[S1n(línea)] = true; ctval++`. Al final muestra contadores y `Tiempo = (ahora - t0).ToString()`. Errores → «Error al procesar el fichero de columnas: {msg}». *No habilita/deshabilita botones* y no hay botón «Grabar» condicionado.
- *Analizar* (`:256-266`): `RecuperarPantalla()`; `Valida(ColumnaGanadora)`; muestra `rks[n][2]` (n=0…4).
- *Grabar Resultado* (`:269-291`): picker «Resultados» `.txt`; recorre los 4 782 969 bits y escribe `N1s(idx)` (orden por índice, sin duplicados).
- *Abrir Condiciones* (`:294-337`): picker `.tri`/`*`; `File.ReadAllLines`; para `r<12`: `aux = linea.split(",")`, `celda c<30 = Int(aux[c].trim()) ?? 0`; los límites se leen de las **últimas 5 líneas** del fichero (`inicio = max(0, count-5)`), `min,max` con `Double.TryParse` (cultura actual; inválido → 0). Error → «No se ha podido leer el fichero de condiciones: {msg}».
- *Salvar Condiciones* (`:340-369`): picker «Trios» `.tri` (tipo «Condiciones»); `RecuperarPantalla()`; escribe **12 líneas** de 30 enteros separados por coma (`P1,P2,P3,v111…v222`) y **5 líneas** `min,max` (enteros truncados). Error → «No se ha podido guardar el fichero de condiciones: {msg}».

*CU-C41 `FiltrarColumnasPorTrios`* (`RecuperarPantalla` `:385-403`, `Valida` `:406-455`) — análogo a CU-C09:
```
rks[n] = (Int(NivelnMin), Int(NivelnMax), 0)     // (int) trunca el NumberBox
nivells[r][c] = AInt(celda c de la fila r)       // AInt = int.TryParse (cultura) o 0 ; c: 0,1,2 = P1,P2,P3 ; 3…29 = niveles de las 27 combinaciones (orden 111,11X,112,1X1,…,222)
valida(col):  rks[*][2] = 0 ; col = col.uppercased()
    para fila en 0..<12:  n1,n2,n3 = P1-1,P2-1,P3-1 ; si algún ni<0 o ni>=col.count : continuar
        trio = col[n1]+col[n2]+col[n3] ; nv = nivells[fila][3 + índice de `trio` en la lista de 27] (otra combinación → 0)
        si nv > 0 : rks[nv-1][2] += 1                // ¡nv > 5 provoca IndexOutOfRange!
    devolver  ∀n<5 : rks[n][0] <= rks[n][2] <= rks[n][1]
```
Codificación `S1n`/`N1s`: la propia de las pantallas 77/80 (X=0, MSB primero); `S1n` exige ≥ 14 caracteres.
**Peculiaridades.** Mismas que la pantalla 77 (nivel > 5 lanza; línea corta aborta con diálogo; BitArray reutilizable e incrementos no atómicos); `Cancelar` no navega; el orden de lectura de límites depende del final del fichero (un `.tri` con < 17 líneas mezcla niveles y límites).
**Área Mac propuesta.** **Condiciones y filtros** (herramienta sobre ficheros, como *Pares*; comparten un `EditorDeNivelesPorGrupo` parametrizado por tamaño de grupo 2 ó 3) · vista principal con tabla editable y panel de resultados.
**Evidencia.** `P:TriosFrmViewModel.cs:24-486`, `P:TriosFrmPage.xaml:11-291`, `P:TriosFrmPage.xaml.cs:14-22`.

---

### 97. `VSignosFrm` — Análisis de signos

**Propósito.** Contabiliza, partido a partido, cuántas veces aparece cada signo (1, X, 2) en un fichero de columnas, con una **columna ganadora parcial** (escrutinio de tardes de sábado/domingo) y un nivel de **aspiración** que filtra qué columnas se contabilizan; resume premios «remanentes» y graba porcentajes o las columnas aceptadas. **Se abre** únicamente desde el botón de barra «Análisis de signos» (`W:MainWindow.xaml.cs:400`); no hay entrada de menú.

**Entradas** (`P:VSignosFrmPage.xaml`):

| Control | Tipo / rango | Defecto |
|---------|--------------|---------|
| `TextBox` ruta + «Examinar…» `:139,144` | picker `.txt`/`*`; al elegirlo calcula **automáticamente** (`VM:168-201`) | «Fichero a procesar» |
| `ComboBox` «Formato» `:173` | «% enteros» / «% con decimales» / «columnas» (solo afecta a **Grabar**) | «% enteros» |
| `ToggleSwitch` «Último partido como Pleno» `:182` | bool | **Sí** |
| `NumberBox` «Aspirando a…» `:193` ≥ 0 + «−» `:201` / «+» `:206` | nº mínimo de aciertos | 14 (`+` solo si `< NumeroPartidos`, `−` solo si `> 0`, `VM:389-408`) |
| «Columna ganadora» `:226-245`: una casilla por partido (1 carácter, mayúsculas) | `1` / `X` / `2` / `-` (comodín) | `-` |
| «Calcular» `:264`, «Grabar» `:269`, «Limpiar» `:273` (vuelve la columna ganadora a `-`) | botones | |
| Rejilla «Recuento por partido» `:69-105`: filas [Part., celda 1, celda X, celda 2]; **pulsar una celda fija ese signo en la columna ganadora del partido** (`FijarUno/FijarEquis/FijarDos`, `VM:359-386`) | enteros | vacía hasta calcular |

**Salidas** (`:281-289`): «Procesadas: n» (defecto «Procesadas: -»), «Tiempo: hh:mm:ss.f» (defecto «Tiempo: -»), «Premios remanentes: a-b-c…» (defecto «Premios remanentes: -»). La rejilla siempre muestra **recuentos** (los modos de porcentaje solo se aplican al grabar).
**Mensajes**: «Seleccione primero un fichero de columnas.», «No se ha podido leer el fichero de columnas: {msg}», «Error al procesar el fichero: {msg}», «Calcule primero el análisis antes de grabar.», «Error al grabar el resultado: {msg}».

**Lógica en el ViewModel.**
*CU-C42 `AnalizarSignosDeFichero`* (`Calcular` `VM:205-289`, `AcCb` `:430-462`, `RecuperaColumnaGanadora` `:414-423`):
```
abrir fichero: N = ObtenNumSignos() ; si N < 10 { N = 15 } ; NumeroPartidos = N ; vals = Int[N][3] ; casillas de ganadora = N (todas "-") ; luego Calcular()
calcular:  part[i] = (casilla i vacía) ? '-' : primer carácter ; limac = Int(Aspiracion)
    premis = Int[ max(1, N-9) ] ; vals = Int[N][3] ; aceptadas = [] ; ctproc = 0
    para cada línea: col = línea.trim().uppercased() ; si col.count < N : continuar ; ctproc += 1
        a = AcCb(col)
        si a >= limac : para i<N: vals[i][ col[i]=='1' ? 0 : col[i]=='2' ? 2 : 1 ] += 1 ; aceptadas.append(col)
        si a > 9 && a <= N : idx = N - a ; si idx < premis.count { premis[idx] += 1 }          // idx 0 = todos los aciertos
    "Premios remanentes: " + premis.joined("-")
AcCb(col):  r = 0
    para nr en 0..<(N-1): si part[nr] == col[nr] { r += 1 } sino si part[nr] ∈ {1,X,2} { nada (fallo) } sino { r += 1 }       // cualquier otro carácter = comodín
    u = part.last ;  c = col.last                                         // ¡el ÚLTIMO CARÁCTER de la columna, aunque tenga más de N!
    si pleno:  si r == N-1 { si u == c { r += 1 } sino si u ∉ {1,X,2} { r += 1 } }
    sino:      si u == c { r += 1 } sino si u ∉ {1,X,2} { r += 1 }
```
*CU-C43 `GrabarSignos`* (`Grabar` `:293-341`): sin resultado previo → aviso. Modo «columnas»: picker «Columnas» `.txt`, una línea por columna aceptada. Modo porcentajes: picker «Valoracion» (tipo «Valoración» `.txt`); por cada partido `suma = vals[0]+vals[1]+vals[2]` (si 0 → 1); `v = vals[j] / suma * 100`; línea `"{v1}, {vx}, {v2}"` formateada con `InvariantCulture` y `"f0"` (enteros) o `"f2"` (2 decimales), separador `", "`.
**Peculiaridades / trampas Swift.** (a) `"F0"/"F2"` de .NET Core ≥ 3.0 redondean el valor binario exacto **«half away from zero»** (12,5 → `13`); `String(format:"%.0f")` da `12` en empates exactos: usar `Decimal`/`NumberFormatter(.halfUp)` y probar con golden (T4). (b) La columna ganadora/aspiración solo filtran la **contabilización**; el contador «Procesadas» cuenta todas las columnas con longitud suficiente. (c) `AcCb` usa el último carácter de la columna real (no el partido `N-1`), igual que `PosiblesPremios`. (d) El bucle se ejecuta en `Task.Run` sin cancelación ni progreso. (e) Un fichero con `N<10` se trata como 15 partidos.
**Área Mac propuesta.** **Análisis** · vista principal con la rejilla de recuentos a la izquierda y panel de parámetros a la derecha (inspector); clic en celda fija el signo de la ganadora.
**Evidencia.** `P:VSignosFrmViewModel.cs:20-463`, `P:VSignosFrmPage.xaml:33-289`, `P:VSignosFrmPage.xaml.cs:12-20`.

---

### 98. `ValoracionFrm` — Valoración de signos (condición)

**Propósito.** Condición que asigna a cada partido unas valoraciones 1/X/2 (rejilla de porcentajes) y exige que la valoración **Global** de la columna y la de sus **unos, equis y doses** caigan en rangos dados, en modo «suma» o «producto». Escribe sobre `FiltroValoracionSignos`. **Se abre** desde el botón «Valoración» de `MainPage` (`W:Views/MainPageViewModel.cs:168`); patrón §1.10.

**Entradas** (`P:ValoracionFrmPage.xaml`): rejilla `PorcentajesControl` `:26` (`NumeroPartidos × 3`, defecto 0) + «Guardar valoración» `:33`; `ComboBox` «Modo» `:45` («Por sumas» | «Por productos x 3E7», defecto «Por sumas»; **cambiarlo borra los 4 rangos**, `VM:54-63`); «Mín-Máx Sumas» `:64` y «Mín-Máx Productos» `:72` (solo lectura); rangos «Global» `:103` (placeholder `100-200#200-600`), «1» `:111`, «X» `:119`, «2» `:127` (placeholder `0-100`; **`#` separa varios rangos**, `:84`); «Columna» `:148` (mayúsculas) + «Calcula» `:168` + «Valor» `:164`; «Buscar límite» `:183`; «Columnas base» `:186` + cuadro multilínea `:191`; botones Aceptar/Estadísticas/Guardar/Abrir/Copiar/Pegar/Borrar/Cancelar `:205-227`. Guardar: sugerido «Valoracion»; tipos «Valoración de Signos» `.valor` y «(XML)» `.xml`; temporal `Temp/tmp.valor` (`VM:122-129`).

**Acciones.** *Entrar*: si no es `Back` y `filtro.ContieneDatos` → `InicializarPantalla` (tipo según `filtro.TipoValoracion == "suma"`, rejilla ← `Valores1/ValoresX/Valores2`, rangos ← `ValorGlobal/Unos/Equis/Doses`; `VM:143-168`). *Buscar límite* → navega a `BuscaLimsFrmPage` sin datos (`:358-366`). *Guardar valoración* → `GuardarValoracionFrmPage.ValoresIniciales = matriz` y navega (`:368-379`). Resto: §1.10.

**Lógica en el ViewModel.**
*CU-C44 `PrepararValoracion`* (`PrepararValores` `VM:225-290`) — siempre con `NumeroPartidos` filas y `Double`:
```
v1[i], vX[i], v2[i] = rejilla[i][0], [1], [2]
sumaMin = Σ min(v1,vX,v2) ; sumaMax = Σ max(v1,vX,v2) ; suma1 = Σ v1 ; sumaX = Σ vX ; suma2 = Σ v2
K = 0.03420425138                                                  // = 1/29.236 ; "productos x 3E7"
prodMax = Π_{i: v1+vX+v2 > 0} max(...)*K ; prodMin = Π_{i: ...} min(...)*K                       // producto inicial 1
prod1 = Π_{i: v1[i] > 29.236} v1[i]*K ; prodX = Π_{vX>29.236} vX*K ; prod2 = Π_{v2>29.236} v2*K     // solo valores > 29,236 aumentan
prodMax = Round(prodMax + 0.000999999, 3) ; prodMin = Round(prodMin - 0.000999999, 3) ; si prodMin < 0 { prodMin = 0 }        // Math.Round(x,3): ToEven
MinMaxSumas = "\(sumaMin)-\(sumaMax)" ; MinMaxProductos = "\(prodMin)-\(prodMax)"                  // Double.ToString() de la cultura actual
rangos vacíos se RELLENAN:   Global ← esSuma ? "\(sumaMin)-\(sumaMax)" : "\(prodMin)-\(prodMax)"
                             Unos   ← esSuma ? "0-\(suma1)" : "0-\(Round(prod1 + 0.000999999, 3))"      (X con sumaX/prodX ; Doses con suma2/prod2)
```
*CU-C45 `AplicarCondicionValoracion`* (`VolcarA` `:388-406`, `Aceptar` `:417-430`):
```
PrepararValores()                                       // (rellena rangos vacíos y refresca Mín-Máx)
filtro.ReinicializaValores()
filtro.TipoValoracion = esSuma ? "suma" : "multiplo" ; filtro.Valores1/ValoresX/Valores2 = arrays ; filtro.ValorGlobal/Unos/Equis/Doses = textos de los rangos
si !filtro.ContieneDatos { filtro.IsActive = true } ; filtro.ContieneDatos = true                // SIEMPRE hay datos: no existe la rama "vacío"
grupo.ActivaFiltro(filtro) ; NotificarCambio ; volver
```
Estadísticas (CU-C03) y Guardar/Abrir (CU-C04, `GetFiltro("ValoracionSignos")`) usan el mismo `VolcarA` sobre un filtro temporal.
*CU-C46 `ValorarColumna`* (`Calcular` `:171-196`): si `Columna.count < NumeroPartidos` solo ejecuta `PrepararValores()` (refresca Mín-Máx y rellena rangos; **sin aviso**); si no: `PrepararValores()`; escribe **en el filtro real del grupo** `Valores1/X/2` y `TipoValoracion` (efecto lateral aunque luego se cancele); `filtro.Analizar(ConvStrToLong(Columna))`; `Resultado = filtro.ValoracionResultado.ToString("r")` (Double, ida y vuelta, cultura actual).
*CU-C47 `ColumnasBase`* (`ObtenerColsBase` `:293-314`, `ordenarValoracionPartido` `:317-339`): por partido, ordena los tres signos por valoración descendente con estos desempates: si `v1 >= v2 && v1 >= vX` → mayor `1`, y `vX >= v2 ? (X,2) : (2,X)`; si no, si `vX >= v2` → mayor `X`, y `v1 >= v2 ? (1,2) : (2,1)`; si no → mayor `2`, y `vX > v1 ? (X,1) : (1,X)`. Las 3 columnas base son: columna 0 = «mayor» de cada partido, columna 1 = «medio», columna 2 = «menor»; se muestran `col0 + "\r\n" + col1 + "\r\n" + col2`.
*Borrar* (`:466-477`): vacía los 4 rangos y los dos Mín-Máx y anula los arrays internos (la rejilla **no** se borra).

**Peculiaridades / trampas Swift.** (a) `Math.Round` (bancario) y `ToString()` de `Double` con cultura: el texto de rangos/Mín-Máx cambia con la configuración regional (T4, T7); para el golden fijar `es-ES` o la cultura que se decida. (b) `29.236` y `0.03420425138` son literales de dominio (comparación `>` estricta). (c) El filtro del dominio parsea los rangos `a-b#c-d` (→ ESPEC_03). (d) «Valor» se calcula sobre el filtro vivo del grupo, no sobre un temporal (R1: preservar o corregir con decisión del dueño). (e) `Aceptar` nunca desactiva el filtro.
**Área Mac propuesta.** **Condiciones y filtros** · hoja modal; rejilla de porcentajes en pestaña lateral; calculadora de valoración de columna y «Columnas base» como secciones del propio panel.
**Evidencia.** `P:ValoracionFrmViewModel.cs:31-485`, `P:ValoracionFrmPage.xaml:11-227`, `P:ValoracionFrmPage.xaml.cs:15-36`.

---

### 99. `VerBoletosEnEditorFrm` — Ver boletos en editor de texto

**Propósito.** Muestra las columnas de un fichero formateadas como **boletos verticales** en texto monoespaciado de solo lectura (8 columnas por boleto), y permite exportar ese texto como imagen. **Se abre** desde Combinación ▸ Ver boletos en editor de texto… (`W:MainWindow.xaml.cs:218`); no tiene botón de barra.

**Entradas** (`P:VerBoletosEnEditorFrmPage.xaml`): «Cargar columnas…» `:58` (picker `.txt`/`*`) y «Imprimir / Exportar imagen» `:62`; `TextBox` multilínea de solo lectura `:38` (en el WinForms la lista de columnas llegaba por constructor; aquí se pide por botón, `P:…Page.xaml.cs:32-35`).
**Mensajes**: «No se ha podido leer el fichero de columnas: {msg}», «No se ha podido generar la imagen de los boletos.», «No se ha podido exportar los boletos: {msg}».

**Lógica en el ViewModel.**
*CU-C48 `FormatearBoletosEnTexto`* (`CargarColumnas` `P:VerBoletosEnEditorFrmViewModel.cs:73-128`, `TransformarColumnas` `:131-146`):
```
cols = ArchivoColumnasTexto(ruta).LeerTodasCols(false)           // [String] sin comas ; vacío → texto ""
long = cols[0].count                                              // la longitud de la PRIMERA columna fija la matriz (una columna más larga lanza)
numBoletos = cols.count / 8 (división entera) ; si numBoletos < cols.count { numBoletos += 1 }      // ¡siempre suma 1! : n=8 → 2 boletos (el 2º vacío) ; idéntico al legacy (Free1X2/UI/VerBoletosEnEditorFrm.cs:17-18)
NL = fin de línea (Environment.NewLine = "\r\n" en Windows) ; sep = VariablesGlobales.Separador            // p. ej. ["5","9","12"] de parametros.free1x2 (D:VariablesGlobales.cs:224-227, `Datos/parametros.free1x2:17`); por defecto ["1","X","2"] (`VariablesGlobales.cs:157`)
para i en 0..<numBoletos: n = i+1
    texto += (n==1 ? "Boleto 1" : NL+NL+"Boleto n") + NL + "__________________"           // 18 guiones bajos
    para z en 0..<long:  texto += NL ; si String(z+1) ∈ {sep[0], sep[1], sep[2]} { texto += NL }       // línea en blanco ANTES de la fila z+1 (si hay ≥3 separadores)
        para j en (i*8)..<(n*8): si j < cols.count { texto += cols[j][z] + " " }                        // un signo por columna, separados por espacio (con espacio final)
```
*Imprimir / Exportar* (`:155-163`, `P:…Page.xaml.cs:45-107`): sin texto no hace nada; si no, la Page dibuja el texto en un `TextBlock` Consolas 13, negro sobre blanco fijo (fondo «papel», sin tema), lo renderiza a bitmap BGRA8, lo copia al portapapeles como imagen y ofrece guardarlo (picker en *Imágenes*, sugerido «boletos», tipo «Imagen PNG»).
**Peculiaridades / trampas Swift.** El número de boletos tiene un exceso de 1 cuando `n` es múltiplo de 8 (y siempre `⌊n/8⌋+1`); una columna posterior más larga que la primera provoca `IndexOutOfRange` (en Swift: acceso seguro); `Separador` puede tener menos de 3 elementos (entonces no se insertan blancos). Saltos de línea `\r\n` vs `\n` (T8). Equivale funcionalmente a `ImageRenderer(content: Text(...).font(.system(.body, design: .monospaced)))` + `NSPasteboard`/`NSSavePanel`; la impresión nativa (`NSPrintOperation`) sería la evolución natural.
**Área Mac propuesta.** **Escrutinio** (o Boleto) · vista principal con `TextEditor` de solo lectura monoespaciado y botón «Imprimir…» (`⌘P`) / «Exportar imagen…».
**Evidencia.** `P:VerBoletosEnEditorFrmViewModel.cs:22-165`, `P:VerBoletosEnEditorFrmPage.xaml:14-78`, `P:VerBoletosEnEditorFrmPage.xaml.cs:23-125`.

---

### 100. `VerBoletos` — Ver boletos (selector de fichero y orden)

**Propósito.** Elegir un fichero de columnas y un criterio/sentido de ordenación y **mostrar las columnas dispuestas en boletos** (8 columnas por boleto, visor `BoletoMatrizControl`, §1.9). **Se abre** desde Ver ▸ Ver boletos…, Combinación ▸ Ver boletos… o el botón «Ver boletos» (`W:MainWindow.xaml.cs:188,214,391`).

**Entradas** (`P:VerBoletosPage.xaml`): «Seleccionar archivo…» `:33` (picker `.txt`, `.cols`, `*`, `VM:123`; defecto «(falta selección)»); radios «Ordenar por …» `:68-91`: «No ordenar» (defecto, `OrdenarMatriz.Signo`), «Variantes», «X», «2», «Interrupciones» y «Signos seguidos» (**deshabilitado**); radios «Tipo de ordenamiento» `:104-107`: «ascendente» (defecto) / «descendente»; «Ok» `:183` (habilitado tras elegir fichero), «Cancelar» `:178` (`Frame.GoBack`, `P:VerBoletosPage.xaml.cs:44-49`); navegación embebida `|<`, `<`, «Boleto nº» (`NumberBox` ≥ 1) + «Ir», `de {total}`, `>`, `>|` `:124-151` y visor `:156` (solo visibles con `BoletoVisible`).
**Estados**: «Seleccione un fichero de columnas», «Fichero seleccionado», «Mostrando boleto», «Cancelado», «Error al seleccionar el fichero: {msg}» (+ diálogo «No se pudo seleccionar el fichero: {msg}»), y después el `Estado` del `BoletoFrmViewModel`.

**Lógica en el ViewModel.**
*Resolución de orden* (`P:VerBoletosViewModel.cs:96-112`): criterio = primero marcado entre Variantes→`OrdenarMatriz.Variantes`, X→`Equis`, 2→`Doses`, Interrupciones→`Interrupciones`, Signos seguidos→`SignosSeguidos`, si ninguno `Signo`; sentido = `desc` si «descendente», si no `asc`.
*`Ok`* (`MostrarBoletoAsync` `:143-166`): si no hay fichero → «Seleccione un fichero de columnas»; configura el `BoletoFrmViewModel` embebido (`ArchivoCombinacion/OrdenarPor/TipoOrden`), dispara `AbrirBoletoSolicitado(fichero, orden, tipo)` y ejecuta `Boleto.CargarCommand`. **La Page responde navegando a `BoletoFrmPage`** con `BoletoFrmPage.ParametrosBoleto = (fichero, orden, tipo)` (`P:VerBoletosPage.xaml.cs:33-37`), de modo que en la práctica `Ok` **abre la pantalla Boleto** y el visor embebido de esta página nunca llega a verse (la página se sustituye; al volver con «atrás» se recrea sin estado) → «Abiertas».
*CU-C49 `CargarYOrdenarBoletos`* (lógica real, en `P:BoletoFrmViewModel.cs:91-160,235-310`, común a esta pantalla y a `BoletoFrm`):
```
n = ObtenNumCols() ; cols = las n columnas del fichero (LeeColumnaSinComas) ; línea ausente → ""
totalBoletos = (n % 8 == 0) ? n/8 : Int(n/8 + 0.5000001) (= ceil(n/8)) ; mínimo 1 ; Estado = "{nombre}: {total} boleto(s)."
ordenar(cols, orden, sentido):
   Signo:  Array.Sort(cols) (comparación de cadenas) ; si desc → invertir                    // ¡orden de .NET por cultura! (T11)
   si no:  clave[i] = Variantes→FiltroNoVariantes.AnalizaColumna(c).NoVariantes ; Equis→.NoEquis ; Doses→.NoDoses ; Interrupciones→FiltroInterrupciones.AnalizaColumna(c).NoInterrupciones ; SignosSeguidos→0 (cálculo desactivado)
           salida = cubos por clave: asc: k = 0…longitud(cols[0]) ; desc: k = longitud…0 ; dentro de cada cubo, orden de aparición en el fichero (ordenación estable por conteo)
           (columnas cuya clave quede fuera de 0…longitud no se emiten → quedan nulas)
boleto b (1-based) = cols[(b-1)*8 ..< b*8] → BoletoMatrizViewModel.Llenar(signos[8], numerosColumna[8]) donde número de columna = posición+1 en el orden ya aplicado
```
Navegación de boletos: `|<`, `<`, `>`, `>|` y «Ir» (`IrABoleto` entre 1 y `Total`, si no «El número de boleto está fuera de rango.»).
**Peculiaridades.** (a) El orden «Signo» usa el comparador de cadenas de .NET (cultura) — en Swift usar comparación ordinal si el contenido es solo `1X2` (equivalente), documentarlo. (b) Cubos ⇒ ordenación **estable** por clave; en Swift `sorted(by:)` no es estable garantizado: usar conteo o `enumerated()` con desempate por índice. (c) `SignosSeguidos` está deshabilitado en la UI y desactivado en el cálculo.
**Área Mac propuesta.** **Escrutinio** (o Boleto) · vista principal: selector de fichero + `Picker` de orden + `BoletoMatrizView` con paginación (`⌘←/⌘→`); fusionar con la pantalla *Boleto* (`BoletoFrm`) en un solo visor.
**Evidencia.** `P:VerBoletosViewModel.cs:29-175`, `P:VerBoletosPage.xaml:16-183`, `P:VerBoletosPage.xaml.cs:13-50`, `P:BoletoFrmViewModel.cs:91-160,235-310`.

---

### 101. `VisorAnalisisColumnasAbdonFrm` — EstuCol · Visor de informe (ABDON)

**Propósito.** Visor del informe que genera la pantalla *EstuCol*: análisis ABDON de las **columnas generadas** frente a las **columnas ganadoras** del fichero. Dos modos (**Global**: rangos sobre todas las ganadoras; **Individual**: una ganadora cada vez), con rango de aciertos, agrupaciones *paso fijo* y *solapadas*, escaleras, sandwichs y suma total de aciertos; permite generar un fichero de condición de columnas probables. **Se abre** desde *EstuCol* (`P:EstucolFrmPage.xaml.cs:24`).

**Datos de entrada (handoff).** `EstucolFrmViewModel.UltimoInforme` = `(porCols: [InformeColumnasABDON], porGans: [InformeColumnasABDON], columnas: [Int64])`; el ctor lo **consume y borra** (sin informe → estado vacío) (`P:VisorAnalisisColumnasAbdonFrmViewModel.cs:186-190`). `InformeColumnasABDON` (dominio, `D:Analisis/InformeColumnasABDON_UnaColTodasGan.cs:6-62`, envuelve `RelacionCP3(serie)`): `MinimoAciertos`, `MaximoAciertos`, `SumaTotalDeAciertos`, `AgrupacionesPasoFijo[,]`, `AgrupacionesSolapadas[,]` (Int32[elementos, aciertos]), `Escaleras`, `NumeroDeEscalerasAscendentes/Descendentes/NumeroDeEscaleras`, `Sandwichs`, `SerieAciertos[]` (cálculo → ESPEC_04).

**Entradas** (`P:VisorAnalisisColumnasAbdonFrmPage.xaml`): botón de modo «Cambiar a modo individual / global» `:31`; `◀ ▶` columna generada `:57,69` y ganadora (solo Individual) `:116,130`; `NumberBox` ≥ 0 «Mostrar sólo agrupaciones de» + «Mostrar» para paso fijo `:213,221` y solapadas `:325,333` (0 = todas); `TextBox` «Suma de Aciertos de las columnas» (placeholder `1-5,8`, solo Individual) + «Sumar» `:416,423`; «Generar Condición» `:486` (solo Global).
**Salidas.** «n / total» de columna `:62` y de ganadora `:122`; «Mín»/«Máx» de aciertos `:86,95`; listas de agrupaciones (columnas «Número · Elementos · Aciertos») `:178,290`; «Hay {x} agrupaciones de {tipo} elementos» `:241-256,353-368`; «Suma Total de la serie de Aciertos» `:395`; «Escaleras Ascendentes/Descendentes/Totales», «Sandwichs» `:450-477`. Valores por defecto «-» o vacío.

**Lógica en el ViewModel.**
*Presentación por columna* (`MostrarInformePorCols` `VM:211-239`): con informes: `ColumnaTexto = "\(n+1) / \(total)"`, Mín/Máx = `informe.Minimo/MaximoAciertos`; sin informes: `"- / -"`, `"-"`. Botones `◀ ▶` acotan `0…total-1` (`:599-619`).
*Presentación por ganadora* (`MostrarInformePorGans` `:243-261`): sin informes `"- / -"`; **Individual**: `GanadoraTexto = "\(g+1) / \(total)"` y `ObtenInfoPorGans` = agrupaciones (tipo 0) paso fijo y solapadas, suma total (`informe.SumaTotalDeAciertos`), escaleras (`NumeroDeEscalerasDescendentes/Ascendentes/NumeroDeEscaleras`) y sandwichs (`Sandwichs.Count`); **Global**: ver abajo. Cambiar de ganadora recrea los contenedores individuales (`new List<Int>[columnas+1, 15]`) y repite (`:621-646`).
*CU-C50 `ResumenGlobalAbdon`* (`MostrarEscalerasGlobal` `:314-337`, `MostrarSandwichsGlobal` `:339-353`, `MostrarSumaAciertosGlobal` `:355-367`):
```
escaleras{Asc,Desc,Tot} = "\(min)-\(max)" sobre todas las ganadoras (aunque min==max, p. ej. "3-3" ; sin informes → "0-0")
sandwichs               = "\(min)-\(max)" del Count de Sandwichs ; sumaTotal = ordenar(sumas) → "\(primero)-\(último)" ; sin informes "-"
```
*CU-C51 `AgrupacionesAbdon`* (`ObtenerListaAgrupacionesPasoFijo/Solapadas[Global]` `:409-548`, `ObtenTextoAgrupaciones` `:550-577`):
```
contenedor[elementos][aciertos] : lista de enteros (distintos) ; dimensión [columnas+1][15]
INDIVIDUAL(tipo): inf = porGans[g] ; para j en filas de inf.Agrup (j==tipo || tipo==0 ; desde j=0) , k en columnas:
        num = inf.Agrup[j][k] ; si no está en contenedor[j][k]: noAgrup += num ; añadir        // devuelve noAgrup
GLOBAL(tipo):     para cada ganadora i: para j = 1..<filas (j==tipo||tipo==0), k: num = inf.Agrup[j][k] ; noAgrup += num ; añadir si distinta ;
                  para j desde filas(inf) hasta filas(contenedor): garantizar que contenedor[j][k] contiene 0 ;
                  valores.append(noAgrup) ; noAgrup = 0                                       // un total por ganadora
                  devuelve ObtenerRangos(valores) = [min, max]  (ordena la lista ; vacía → [0,0])
texto(contenedor): para i = 1..<filas, j = 0..<15: si lista != nil :
        si lista.count > 1 { ordenar ; "\(primero)-\(último)+\(i)+\(j)" } sino si lista[0] != 0 { "\(lista[0])+\(i)+\(j)" }
fila mostrada: partes = texto.split("+") → Número = partes[0] (cantidad o rango), Elementos = i, Aciertos = j
```
«Mostrar» (`:652-694`): recrea el contenedor del modo activo; `tipo = Int(NumberBox)`; Individual → `Hay = noAgrup.ToString()`; Global → `Hay = "\(r0)-\(r1)"` (o vacío) y `NumElementos = tipo`.
*Suma opcional* (`CalcularSumaOpcional` `:700-715`): `columnas = ObtenerListaFromTxt(texto)` (admite `a-b`, `a,b`; **entrada 1-based → índices 0-based**, error → lista vacía); `suma = Σ SerieAciertos[i] ∀ i ∈ columnas`.
*Cambiar modo* (`CambiarModo` `:585-597`): invierte `ModoGlobal`, borra «Hay…/Nº elementos» y los NumberBox de tipo (a 0) y repite `MostrarInformePorGans()` (sin reiniciar contenedores).
*CU-C52 `GenerarCondicionColumnasProbables`* (`GenerarCondicion` `:722-802`): requiere `columnas.count > 0 && porCols.count >= columnas.count` (si no, «No hay informe de columnas para generar la condición.»); picker «Condicion» (tipos «Columnas probables» `.cps` y «(XML)» `.xml`); construye `FiltroColProbables`:
```
todos = ObtenerTodosValores() ("0…NumeroPartidos")
para i en columnas: cp = ColumnaProbable{ Pronosticos = ObtenerStringFromLongApuestaMultiple(columnas[i]).split(","),
        NoAciertos = ObtenerTodosValores(porCols[i].Min, porCols[i].Max), NoAciertosSeguidos = todos, NoFallosSeguidos = todos, Puntos = "" } ; filtro.ColProbables.append(cp)
RelacionCP1{ Columnas: "1-n", SumaAciertos: SumaTotalAciertos(texto mostrado), Recorridos: "", CantidadCP: "", CuantosAC: "" }
RelacionCP3{ ColumnasImplicadasString: "1-n", Columnas: cps, Concepto: "AC", EscalerasAsc/Desc/Totales (texto + ObtenerBoolArrayFromTxt(texto, n/3)), Sandwichs (texto + bool[](texto, n/4)),
             AgrupacionesPasoFijoPermitidasString = texto(contenedorPasoFijoGlobal), AgrupacionesSolapadasPermitidasString = texto(contenedorSolapadasGlobal) }
filtro.ContieneDatos = true ; ArchivoCondiciones.GuardaArchivo(filtro) → aviso "Filtro guardado"       // errores: "No se pudo generar la condición: {msg}"
```
Usa **siempre valores globales** (textos «min-max» del modo Global y contenedores globales; los de Individual no intervienen). Si algún texto es «-» o inválido, `ObtenerBoolArrayFromTxt` lanza (captura general → diálogo de error).
**Peculiaridades / trampas Swift.** `List<int>[,]` con celdas `null` → en Swift `[[ [Int]? ]]`; `Sort()` de enteros (T5 no afecta); fila 0 (elementos = 0) se recorre en Individual pero no en Global ni al generar texto; los `.ToString()` de enteros sin cultura relevante; `Contains` lineal. `Sandwichs`/`Escaleras` son listas de objetos del dominio cuyo cálculo vive en `RelacionCP3` (ESPEC_04).
**Área Mac propuesta.** **Columnas probables** · vista principal con segmented «Global/Individual», inspector de rangos y `Table` de agrupaciones; «Generar condición…» como acción de exportación (`.cps`).
**Evidencia.** `P:VisorAnalisisColumnasAbdonFrmViewModel.cs:23-803`, `P:VisorAnalisisColumnasAbdonFrmPage.xaml:12-486`, `P:VisorAnalisisColumnasAbdonFrmPage.xaml.cs:12-20`, `D:Analisis/InformeColumnasABDON_UnaColTodasGan.cs`, `D:Utils/UtilidadesEntradasValores.cs:42-106,255-290,383-392`.

---

### 102. `VisorAnalisisColumnasFrm` — Visor de análisis de columnas (por secciones)

**Propósito.** Visor **de solo lectura** con una lista de secciones («pestañas» del WinForms) que vuelca el `ContenedorAnalisisGlobal` producido al **calcular con análisis** (VX2, signos seguidos, dibujos, interrupciones, contactos, pesos, distancias, control de grupos/conjuntos, simetrías, diferencias, valoración, columnas probables y formatos). **Se abre solo desde el motor**, nunca desde menú ni barra: `Analizador.AnalizaCombinacion(bool)` (`D:MotorCalculo/Analizador.cs:256,296-301`; la sobrecarga `(int)` de `:196-200` delega en ella con `true`) y `AnalizaCombinacion(bool,bool)` (`:304,341-346`), cuando `ContenedorAnalisisColumnasGlobal != null && noColsAceptadas > 0`, llaman al gancho `AnalisisUi.MostrarVisor(contenedor, GruposPartidos[0])` (`D:Abstractions/UiHooks.cs:35`); la app lo cablea en `W:App.xaml.cs:170-205` (ver §1.7). Se dispara, p. ej., desde CalculaColumnas en modo *Analizar* (`P:CalculaColumnasFrmViewModel.cs:330`) y AnalizarFichero (`P:AnalizarFicheroFrmViewModel.cs:128`).

**Datos de entrada (handoff).** `VisorAnalisisColumnasFrmViewModel.UltimoContenedor`/`UltimoGrupo` (`P:VisorAnalisisColumnasFrmViewModel.cs:103-104`). El hook los publica **en el hilo de UI justo antes de navegar** (`W:App.xaml.cs:185-196`; la cola de UI evita que un segundo visor pise el payload del primero). El ctor del VM **consume y borra** ambos (`VM:112-114`); `UltimoGrupo` no se usa para nada más. Abrir el visor sin análisis previo (no hay entrada de menú; solo por navegación directa/smoke) muestra el estado vacío. En Swift: `AnalisisColumnasVista(contenedor:)` recibido por parámetro/`Sesion`, sin estáticos.

**Entradas** (`P:VisorAnalisisColumnasFrmPage.xaml`): lista «Secciones» `ListView` de selección única `:44-62` (cada fila = título + marcador «●» si la sección está activa, `VM:77`); botón «Refrescar» `:71`. Sin campos editables, sin validaciones.

**Salidas.** Cabecera «Análisis / Visor de análisis de columnas por secciones» `:17-18`; a la derecha título y texto de detalle de la sección (`DetalleTitulo`/`DetalleTexto` `:89,94`, `VM:128-131`) y, según `Tipo`, el presentador de **matrices** (`ItemsControl` de bloques con título opcional + `MatrizAnalisisView` `:110-121`, §1.9) o el de **valoración** (4 columnas «Valor / Columnas» `:128-185`). El título de bloque se oculta si está vacío (`P:VisorAnalisisColumnasFrmPage.xaml.cs:28-29`). Todas las celdas son **enteros con `ToString()`** (nº de columnas) sin formato adicional.

**Lógica en el ViewModel.**
*CU-C53 `ConstruirSeccionesDeAnalisis`* (`ConstruirSecciones` `VM:180-270` y constructores `:276-593`). Las secciones se añaden **en este orden** y solo si el flag global correspondiente está activo (`VariablesGlobales.Analizar*`, leídos de `Datos/parametros.free1x2` en `D:VariablesGlobales.cs:100`, getters `:266-338`):
```
c = contenedor?.AnalisisGrupos (ContenedorAnalisis)
1 VX2              (AnalizarVX2)         matriz c.VX2 [3][N+1]            filas "Variantes","Equis","Doses"
2 Signos Seguidos  (AnalizarSeguidos)    c.Seguidos [4][N+1]              "Variantes seguidas","Unos seguidos","Equis seguidas","Doses seguidos"
3 Dibujos          (AnalizarDibujos)     c.Dibujos [maxAciertos][N+1]     etiqueta "Fila i" (sin etiquetas)
4 Interrupciones   (AnalizarInterrupciones) c.Interrupciones [10][N+1]    "Globales","Variantes","Unos","Equis","Doses" + las mismas "… seguidas/seguidos"
5 Contactos        (AnalizarContactos)   c.Contactos [10][N]              "1X","12","X2","11","XX","22","1V","XV","2V","VV"
6 Pesos Numéricos  (AnalizarPesos)       c.Pesos [5][10]                  "Global","Variantes","Unos","Equis","Doses"
7 Distancias       (AnalizarDistancias)  c.Distancias [4][N]              "Variantes","Unos","Equis","Doses"
8 Control Grupos   (AnalizarControlGrupos)    vector contenedor.ColumnasPorFallosDeGrupos   filas "Fallos"(cabecera 0…n-1)/"Columnas"
9 Control Conjuntos(AnalizarControlConjuntos) vector ColumnasPorFallosDeConjuntos
10 Simetrías       (AnalizarSimetrias)   c.ContenedorSim
11 Diferencias     (AnalizarSimetriasII) c.ContenedorDiferencias
12 Valoración      (AnalizarValoracion)  c.ValoracionesGlobales/Unos/Equis/Doses
13 Columnas Probables (AnalizarCPs)      c.ContColumnasProbables
14 Formatos        (AnalizarFormatos)    c.ContenedorFormatosSignos
si no se añadió ninguna → una sección "Sin análisis activos" (detalle: contenedor nulo → "No se ha recibido ningún análisis. Ejecuta un análisis para ver los datos."; si no → "No hay ningún tipo de análisis activo …")
```
(dimensiones de las matrices en `D:Analisis/ContenedorAnalisis.cs:354-364`; `N = NumeroPartidos`.) Los flags `AnalizarTodo`, `AnalizarFigurasV1X2`, `AnalizarFormatos123`, `AnalizarFigurasContactos/Pesos` y `AnalizarGruposEquipos` **no** crean sección en este visor.
*Matriz genérica* (`SeccionMatriz`/`ConstruirMatriz` `VM:276-321`): si la matriz es nula → sección `Vacia` con detalle «{título}: no hay datos para analizar»; si no, detalle «{título}: {filas} conceptos × {cols} valores (cada celda = nº de columnas).» y **una** matriz = fila de cabecera (etiqueta vacía, celdas `0…cols-1`, estilo cabecera) + una fila por concepto (etiqueta o «Fila i», celdas `matriz[i][j]`).
*Vector* (`SeccionVector` `:324-348`): dos filas: «Fallos»/«Columnas», cabecera `0…len-1` y valores; nulo → «…: no hay datos para analizar».
*Simetrías* (`:352-379`): lista vacía → «Simetrías: No hay datos para analizar»; si no, matriz de 2 filas: «Nº Aciertos» (cabecera = `Aciertos` de cada `ContenedorSimetrias`) y «Columnas» (= `Columnas`).
*Diferencias* (`:384-433`): un bloque «Conjunto n» (n 1-based) por `ContenedorDiferencias`; `cuantos = NumDiferenciasPosibles`; fila de cabecera «Nº» con `1…cuantos-1` y seis filas «Variantes, Equis, Dibujos, Doses, Interrupciones, Formatos» con `array[i]` para `i = 1 ..< min(cuantos, array.count)` (el índice 0 **no** se muestra; array nulo → fila vacía).
*Valoración* (`:437-478`): requiere `c != nil && c.UsaValoraciones` (si no «Valoración: No hay datos para analizar»). Detalle: «Valoración: distribución de columnas por valor ({tipo}).» con `tipo = "Por Sumas"` si `TipoValoracion == "suma"`, `"Por Productos"` si `"multiplo"`, si no el texto tal cual. Cuatro columnas (Global, Unos, Equis, Doses): `inicio = max(0, noValores.first ?? 0)`; para `i = inicio ..< valores.count` con `valores[i] != 0` → entrada (`Clave = String(i)`, `Columnas = String(valores[i])`).
*Columnas probables* (`:482-532`): requiere `UsaCPs`; tres bloques «Aciertos», «Aciertos seguidos», «Fallos seguidos» (= `NoAC`, `NoACS`, `NoFS` de cada `ContenedorColumnasProbables`); cada matriz tiene cabecera «Nº» con `0…NumeroPartidos` (inclusive) y una fila «CP i» (i 1-based) con `datos[j]` (si el array es nulo o corto → `"0"`). Detalle «Columnas probables: {n} columnas (aciertos / aciertos seguidos / fallos seguidos).».
*Formatos* (`:536-593`): requiere `UsaFormatos`; por cada grupo `i` (1-based): si `AciertosFormatosSignos != nil` → bloque «Grupo i — Líneas» (cabecera «Líneas» `0…len-1`, fila «Columnas»); si `AciertosGlobalesFormatos` (SortedList clave→nº) no está vacío → bloque «Grupo i — Globales» (cabecera «Global» con el **índice de posición** `0…count-1`, **no** la clave; valores = `globales[claves[j]]` en orden de clave). Detalle «Formatos: {n} grupos de formatos.».
*CU-C54 `RefrescarVisorAnalisis`* (`Refrescar` `VM:160-174`): guarda la clave de la sección seleccionada, vuelve a ejecutar CU-C53 sobre **el mismo** contenedor (los datos no cambian; solo se releen los flags `Analizar*`), vuelve a seleccionar la sección con esa clave y, si ya no existe, la primera. La selección inicial es la primera sección (`VM:119`).
**Peculiaridades / trampas Swift.** (a) El texto de detalle y los títulos son literales en español fijos (no localizados). (b) Una sección con datos nulos sigue **listada** pero muestra solo el mensaje (Tipo `Vacia`). (c) `Seleccion = null` + reasignación en Refrescar dispara dos notificaciones (sin efecto visible). (d) La cabecera de Diferencias y Formatos/Globales usa índices de posición, no claves de dominio: replicar tal cual. (e) `SortedList` de Formatos → en Swift un diccionario ordenado por clave (`[Int: Int]` + `keys.sorted()`).
**Área Mac propuesta.** **Cálculo** (resultado de «Calcular con análisis») · vista principal tras el cálculo (no es una pantalla de menú): `List` de secciones en una barra lateral secundaria + detalle con `Grid`/`Table` por bloque (`MatrizAnalisisView`) y botón «Actualizar» (`⌘R`); posible exportación a CSV como añadido (decisión del dueño).
**Evidencia.** `P:VisorAnalisisColumnasFrmViewModel.cs:30-595`, `P:VisorAnalisisColumnasFrmPage.xaml:15-185`, `P:VisorAnalisisColumnasFrmPage.xaml.cs:11-30`, `W:App.xaml.cs:170-205`, `D:MotorCalculo/Analizador.cs:296-301,346`, `D:Analisis/ContenedorAnalisis.cs:354-364,450-678`, `D:Analisis/ContenedorAnalisisGlobal.cs:20-62`, `Free1X2/UI/VisorAnalisisColumnasFrm.cs:67-195` (mismos 14 flags `Analizar*` en el WinForms de referencia).

---

### 103. `VisorEstadisticas` — Estadísticas de cumplimiento por fichero

**Propósito.** Visor de **solo lectura** que muestra, para el filtro que se está editando, el **porcentaje de cumplimiento** sobre cada fichero de columnas ganadoras de la carpeta `Ganadoras/`. **Se abre** únicamente desde el botón «Estadísticas» de las pantallas de condición (§1.10): 13 productores fijan `VisorEstadisticasViewModel.UltimasEstadisticas = lista` y navegan a `VisorEstadisticasPage` — NoVariantes (`P:NoVariantesFrmViewModel.cs:195-200`), PesosNum (`:326-327`), SignosSeguidos (`:302-303`), Simetrías (`:285-286`), Valoración (`:460-461`), Contactos (`:275-276`), Distancias (`:223-224`), Interrupciones (`:278-279`), Dibujos (`:297-298`), Diferencias (`:461-462`), Formatos (`:381-382`), Formatos123 (`:463-464`) y GruposEquipos (`:1125-1126`). Tiene entrada propia en `PortedPages` (`W:Navigation/PortedPages.cs:84`, «Visor Estadisticas») pero ninguna de menú ni de barra.

**Datos de entrada (handoff).** `VisorEstadisticasViewModel.UltimasEstadisticas: List<Estadistica>?` (`P:VisorEstadisticasViewModel.cs:49`), leída en el constructor y **no borrada** (a diferencia de otros visores). `Estadistica` = `{Archivo: String, Cumplimiento: Float32}` (`D:MotorCalculo/Estadisticas/Estadistica.cs:4-19`). En Swift: pasar `[EstadisticaFichero]` como parámetro de la hoja, sin estático.

**Entradas.** Ninguna: sin controles editables ni acciones (`P:VisorEstadisticasPage.xaml:15-91`).

**Salidas** (`P:VisorEstadisticasPage.xaml`): cabecera «Estadísticas» + «Porcentaje de cumplimiento de cada fichero analizado.» `:17-20`; tabla con columnas «Archivo» (3*) · barra de progreso 0–100 (2*, `:86`) · «Cumplimiento» con sufijo «%» (`:43-50,91`); estado vacío «No hay estadísticas para mostrar.» cuando la lista es nula o vacía (`:57`, `VM:62,86`).

**Lógica.**
*CU-C55 `EstadisticasDeCumplimiento`* (producida en el ViewModel de cada condición con el motor, p. ej. NoVariantes `P:NoVariantesFrmViewModel.cs:192-201`; ver CU-C03):
```
filtroTemp = ObtenerFiltroTemporal()                          // filtro con lo que hay en pantalla, sin tocar el Grupo
para cada fichero f de <AppDir>/Ganadoras/ que cumpla "*.txt" (orden de DirectoryInfo.GetFiles):
    total = 0 ; validas = 0
    para cada columna de f (ArchivoColumnasTexto.SiguienteColumna/LeeColumnaSinComas) :
        total += 1 ; si filtroTemp.Analizar(ConvStrToLong(col)) { validas += 1 }
    cumplimiento = (total == 0) ? 0 : (Float32(validas) / Float32(total)) * 100          // Float 32 bits
    resultado.append(Estadistica(Archivo: nombre de f, Cumplimiento: cumplimiento))
```
(`D:MotorCalculo/Estadisticas/CalculadorEstadisticas.cs:11-52`; `Analizar` de cada filtro está en ESPEC_03/04.) La carpeta es `Path.Combine(AppContext.BaseDirectory,"Ganadoras") + separador` (`P:NoVariantesFrmViewModel.cs:57-58`; igual en los otros 12), y la ruta del fichero se arma por **concatenación** `directorio + nombre` (`CalculadorEstadisticas.cs:44`), por lo que el directorio debe acabar en separador. En la reescritura la carpeta pasa a `Application Support/Free1X2/Ganadoras/` (decisión del dueño; ver ESPEC_00).
*CU-C56 `PrepararFilasEstadisticas`* (ctor `P:VisorEstadisticasViewModel.cs:64-87`):
```
para cada e en lista (en orden de entrada):
    valor = e.Cumplimiento (Double) ; acotar a [0, 100]                    // solo para la barra
    fila = { Archivo: e.Archivo, Cumplimiento: e.Cumplimiento + "%", CumplimientoValor: valor }
SinDatos = (filas.count == 0)
```
**Formato numérico (trampa).** `Cumplimiento + "%"` concatena un `Float32` con `ToString()` **sin cultura ni formato** (`VM:81`): da la representación más corta que reconstruye el `Float` (.NET Core ≥ 3.0) y con la cultura del usuario (coma decimal en es-ES), p. ej. `33,333336%` o `100%`, sin redondeo a 1 o 2 decimales. Es el comportamiento a replicar si se exige igualdad textual con la app actual (T7); en Swift `Float.description` produce `33.333336` (punto) → reemplazar el separador según el `Locale`. No hay ordenación en el visor: el orden es el que devuelve `GetFiles` (dependiente del sistema de ficheros; en macOS fijar orden alfabético por nombre, ver «Abiertas»).
**Área Mac propuesta.** **Condiciones y filtros** · hoja modal (o panel inspector) abierta desde «Estadísticas» del editor de condición: `Table` con columnas Archivo / `ProgressView` / Cumplimiento.
**Evidencia.** `P:VisorEstadisticasViewModel.cs:15-88`, `P:VisorEstadisticasPage.xaml:15-91`, `P:VisorEstadisticasPage.xaml.cs:12-20`, `D:MotorCalculo/Estadisticas/CalculadorEstadisticas.cs:9-54`, `D:MotorCalculo/Estadisticas/Estadistica.cs:4-19`, `W:Navigation/PortedPages.cs:84`.

---

### 104. `VisorPosiblesPremios` — Visor de posibles premios (resumen por grupos)

**Propósito.** Visor de **solo lectura** del **resumen** generado por *Posibles premios* (pantalla 79): para cada **columna ganadora completa** posible (todas las formas de sustituir los «*» por 1/X/2) muestra esa ganadora en vertical y, a su lado, las columnas jugadas que obtendrían premio (16/15/14/13/12/11/10 aciertos), con los signos fallados marcados en rojo; navegación entre grupos con contador «X de N». **Se abre** solo desde el botón «Ver» de `PosiblesPremiosFrm` (`P:PosiblesPremiosFrmViewModel.cs:267-284`, `Navegar(typeof(VisorPosiblesPremiosPage), null)`); sin resumen muestra el aviso «No hay resumen que mostrar. Marca 'Generar resumen' y calcula.». No tiene entrada de menú ni de barra.

**Datos de entrada (handoff).** `VisorPosiblesPremiosViewModel.UltimoResumen: List<PosiblesPremiosContenedor>?` (`P:VisorPosiblesPremiosViewModel.cs:82`) con una **copia** de la lista `resumen` de la pantalla 79 (`P:PosiblesPremiosFrmViewModel.cs:281`); no se borra al consumirlo. `PosiblesPremiosContenedor` = `{ColGanadora: String, Col16…Col10: [String], ValorTotal}` (`D:Escrutinio/PosiblesPremiosContenedor.cs:9-30`); cada elemento de `ColNN` es la columna jugada **seguida de dos dígitos con la categoría** (`columna + "14"`, `P:PosiblesPremiosFrmViewModel.cs:514-520`), y la lista llega ya ordenada por `PosiblesPremiosComparer` (descendente por tamaño de Col16, Col15, …; CU-C13).

**Entradas.** Sólo `◀` / `▶` (`P:VisorPosiblesPremiosPage.xaml:31,42`); sin campos editables ni validaciones.

**Salidas** (`P:VisorPosiblesPremiosPage.xaml`): cabecera «Visor de Posibles Premios» + descripción `:19-22`; contador «X de N» (`:35`, `VM:224-230`; «0 de 0» si no hay grupos); panel «Columna ganadora (CG)» con un recuadro 28×28 por signo, en vertical (`:62-80`); panel «Columnas con premio» (`:96-100`) con, por cada columna premiada, una tarjeta horizontal: etiqueta de aciertos (`AciertosTexto`, tooltip «Aciertos de la columna», `:125`), categoría (`:133`) y un recuadro 24×24 por signo (`:138-157`), con fondo normal (`AppSurfaceAltBrush`) si acierta o **rojo de error** (`AppErrorBrush`) con texto blanco si falla (`P:VisorPosiblesPremiosPage.xaml.cs:30-50`).

**Lógica en el ViewModel.**
*CU-C57 `MostrarGrupoDePremios`* (ctor y `MostrarGrupo` `P:VisorPosiblesPremiosViewModel.cs:87-102,157-222`):
```
resumen = UltimoResumen ?? [] ; total = resumen.count ; indice = 0
si resumen no vacío: MostrarGrupo(0) sino Contador = "0 de 0"
MostrarGrupo(g):
    vaciar ColumnaGanadora y ColumnasPremiadas ; si g fuera de rango → solo actualizar contador
    cont = resumen[g] ; gan = cont.ColGanadora ?? ""
    ColumnaGanadora = [ SignoGanador(c) para c en gan ]                      // un recuadro por carácter
    para (lista, categoria) en [(Col16,16),(Col15,15),(Col14,14),(Col13,13),(Col12,12),(Col11,11),(Col10,10)]:   // ese orden
        para columna en lista (nula → "" ):
            aciertosTexto = columna.count >= 2 ? últimos 2 caracteres de columna : ""
            signos = [ (signo: columna[s], esAcierto: columna[s] == gan[s]) para s en 0 ..< min(gan.count, columna.count) ]   // comparación de caracteres, sensible a mayúsculas
            ColumnasPremiadas.append({ Signos: signos, Categoria: categoria, AciertosTexto: aciertosTexto })
    Contador = total == 0 ? "0 de 0" : "\(indice+1) de \(total)"
```
*Navegación* (`Adelante`/`Atras` `:132-152`): `▶` incrementa `indice` solo si `indice < total-1`; `◀` lo decrementa solo si `indice > 0`; en ambos casos llama a `MostrarGrupo(indice)`. Los botones **no se deshabilitan** en los extremos (simplemente no hacen nada).
**Peculiaridades / trampas Swift.** (a) `AciertosTexto` es una subcadena del final del string almacenado, no se recalcula (siempre coincide con la categoría porque el sufijo se añadió en pantalla 79). (b) Un grupo contiene **todas** las columnas premiadas (no se pagina por 8 aquí). (c) El resumen puede ser enorme (producto cartesiano de los «*»; ver CU-C13) y aquí se materializa un `ColumnaPremioItem` con N hijos por cada columna de un grupo: en SwiftUI usar `LazyVGrid`/`LazyVStack`. (d) Los signos se comparan como caracteres exactos; la pantalla 79 guarda columnas y ganadora en mayúsculas (`ToUpper()` en `P:PosiblesPremiosFrmViewModel.cs:186,391`). (e) `ValorTotal` **no** se muestra ni se calcula: `CalcularValorTotal` (`D:Escrutinio/PosiblesPremiosContenedor.cs:67-95`, con `+` donde cabría `*` en las categorías 13–16) no tiene ningún llamante; **código muerto, no portar**.
**Área Mac propuesta.** **Escrutinio** · hoja modal (o panel secundario) abierta desde «Ver» en *Posibles premios*, con `Stepper`/`◀ ▶` y contador; mantiene el aviso de falta de resumen.
**Evidencia.** `P:VisorPosiblesPremiosViewModel.cs:14-231`, `P:VisorPosiblesPremiosPage.xaml:17-157`, `P:VisorPosiblesPremiosPage.xaml.cs:17-51`, `P:PosiblesPremiosFrmViewModel.cs:267-284,502-530`, `D:Escrutinio/PosiblesPremiosContenedor.cs:9-97`.

---

### 105. `aidomnou` — Filtro Aidomnou (6 columnas probables + filtrado de un fichero)

**Propósito.** A partir de la **valoración de cada signo (1/X/2) de los 14 partidos** genera **6 columnas probables** (cada una con una estrategia distinta, con signos simples o dobles/triples), y filtra un **fichero de columnas** dejando solo las que cumplen unos límites de aciertos contra esas 6 columnas (por columna, suma y recorrido); además analiza una columna ganadora contra las 6. Es una herramienta **independiente** (no usa `Grupo`, `Analizador` ni `AppState`; no toca el boleto). **Se abre** desde Filtros ▸ «Filtro Aidomnou…» (`W:MainWindow.xaml.cs:240`) o el botón «Filtro Aidomnou» de la barra (`:411`); también figura en `PortedPages` (`W:Navigation/PortedPages.cs:129`).

**Entradas** (`P:aidomnouPage.xaml`):

| Control | Tipo | Defecto | Notas |
|---------|------|---------|-------|
| Rejilla «Valoraciones (1 / X / 2)» `:46` (`PorcentajesControl`, §1.9) | `NumeroPartidos` filas × (1,X,2) `double` ≥ 0 | todo 0 (`PorcentajesHelper.Crear(VariablesGlobales.NumeroPartidos)`, `VM:47-48`) | el algoritmo usa **solo las 14 primeras filas** |
| Límites por columna «lím:» ×6 `:139-149` | `TextBox` texto `a-b`, `MaxLength=14` | `0-14` (`VM:79-94`) | |
| «suma:» `:158` y «recorrido:» `:163` | `TextBox` `a-b` | `0-84` / `0-14` (`VM:97,100`) | |
| «Salvar Límites» `:223` / «Recuperar Límites» `:224` | botones (picker `.cnd`) | etiqueta «Fichero» (`VM:104`) | |
| «Calcular» `:235` / «Cancelar» `:251` | botones | | |
| «Exportar Columnas» `:258` / «Grabar Resultado» `:259` | botones | etiqueta «Fichero» (`VM:123`) | |
| «Fichero Ganadoras» `:275`, «-» `:281`, «+» `:288` | botones | «Fichero Ganadoras», «-» (`VM:133,137`) | |
| «Columna a analizar (14 signos: 1 / X / 2)» `:291` | `TextBox` `MaxLength=14`, mayúsculas, placeholder `2222XXXX222XXX` | `2222XXXX222XXX` (`VM:129`) | |
| «Analizar» `:298` | botón | | |

**Salidas.** Matriz de solo lectura «Columnas Probables» 14 × 6 (cabecera `P`, `C1…C6`; fila «01»…«14») `:65-117` y lista de 6 líneas «Col n: …» (`VM:621-648`); fila de aciertos «ac:» ×6 (`Rk1…Rk6`) + «suma:» + «recorrido:» `:170-204`; «Procesadas», «Válidas», «Tiempo» `:240-249`; nombre del fichero de resultado `:260`, de límites `:225`, de ganadoras `:276` y posición «n» `:282`. Nota informativa fija sobre `aidomnou.cfg` `:209` (ver «Abiertas»: **la app no lee ni escribe ese fichero**). Todos los contadores se muestran como `Int.ToString()`.

**Lógica en el ViewModel** (todo está aquí; no se llama al motor):
*Códigos de signo.* Cada celda de `cps[partido][columna]` es una **máscara de bits**: `1` = signo «1», `4` = «X», `2` = «2». Texto (`Cambia` `VM:651-661`): `0→"-"`, `1→"1"`, `2→"2"`, `3→"12"`, `4→"X"`, `5→"1X"`, `6→"X2"`, cualquier otro (7 o más)`→"1X2"`. Esta codificación y la de `S2n`/`N2s` (más abajo) son **locales** de esta pantalla, no la de `ConvertidorDeBases` (`D:Utils/ConvertidorDeBases.cs:31,96-106`).

*CU-C58 `GenerarSeisColumnasProbables`* (`PreparaColumnas` `VM:487-500`, `PreCol1..6` `:503-618`). `lims = [145,11,9,8,65,9,6,5]` **constantes** (`VM:66`); `nvals = Valoraciones` como `Double[14][3]` (col 0=1, 1=X, 2=2; `PorcentajesHelper.AMatriz`, `W:Controls/PorcentajesHelper.cs:23-34`); `cps` se pone a 0 y se ejecutan, en este orden, las seis estrategias; después `scps[c]` = concatenación de los 14 enteros `cps[p][c]` como dígitos (`"…".join`):
```
código(s)  = s==0 ? 1 : (s==1 ? 4 : 2)                        // s = índice de signo 0=1, 1=X, 2=2
puntos(v)  = v ∈ {3,5,6} ? 10 : (v == 7 ? 15 : 0)              // dobles 10, triple 15, simples 0
PreCol1 (c=0, tope=lims[0]=145) y PreCol4 (c=3, tope=lims[4]=65) — «por puntos»:
    pvals = copia de nvals
    repetir (Col1: como máximo 42 veces; Col4: sin límite) :
        (q1,q2,nv) = (0,0,0)
        para p en 0..<14, s en 0..<3 : si pvals[p][s] >= nv { nv = pvals[p][s]; q1 = p; q2 = s }      // ">=": en empate gana el ÚLTIMO recorrido
        cps[q1][c] += código(q2) ; pvals[q1][q2] = -1
        si Σ_p puntos(cps[p][c]) >= tope → parar
PreCol2 (c=1; n1=lims[1]=11 unos, n2=lims[2]=9 equis, n3=lims[3]=8 doses) y PreCol5 (c=4; n1=lims[5]=9, n2=lims[6]=6, n3=lims[7]=5) — «por cantidades»:
    repetir n1 veces : (q,nv)=(0,0) ; para p : si cps[p][c] < 1 && nvals[p][0] >= nv { nv = nvals[p][0]; q = p } ; cps[q][c] += 1      // ">=" (último)
    repetir n3 veces : (q,nv)=(0,0) ; para p : si cps[p][c] < 2 && nvals[p][2] >  nv { nv = nvals[p][2]; q = p } ; cps[q][c] += 2      // ">" (primero)
    repetir n2 veces : (q,nv)=(0,0) ; para p : si cps[p][c] < 4 && nvals[p][1] >  nv { nv = nvals[p][1]; q = p } ; cps[q][c] += 4
PreCol3 (c=2) — «14 dobles»: para cada partido p : s* = argmin_s nvals[p][s] (inicio nv=100, «<»: gana el primero) ;
    cps[p][2] = s*==0 ? 6 (X2) : (s*==1 ? 3 (12) : 5 (1X))                         // excluye el signo menos valorado
PreCol6 (c=5) — «14 fijos»: para cada partido p : s* = argmax_s nvals[p][s] (inicio nv=0, «>»: primero; todo 0 → s*=0) ; cps[p][5] = código(s*)
```
**Trampas (replicar tal cual).** (a) El orden de los tres bucles de PreCol2/5 es **unos → doses → equis**, aunque `lims` esté en orden unos/equis/doses (`lims[2]` es equis, `lims[3]` doses). (b) Si ninguna casilla es elegible (p. ej. todos los valores a 0, ya que `nv` arranca en 0 y `>` exige estricto mayor) se usa **el partido 0 sin comprobar elegibilidad**, de modo que `cps[0]` puede recibir dos veces el mismo bit y salir de 0…7 (entonces `Cambia` da «1X2» y `scps` pierde la alineación de 14 caracteres). (c) La valoración a 0 es el estado inicial de la pantalla: los comparadores estrictos `>` no encuentran candidato y se aplica la regla (b) (resultado degenerado, no es un error). (d) Si `NumeroPartidos < 14` las lecturas `nvals[0..13]` desbordan (excepción no capturada por el ViewModel; la `Rejilla` se crea con `NumeroPartidos` filas, `VM:47-48`).

*CU-C59 `ParsearRangosAidomnou`* (`RecuperaPantalla` `VM:458-484`): `nvals = AMatriz(Valoraciones)`; para cada uno de los 8 textos (6 columnas, suma, recorrido): `aux = texto.split("-")`; `rks[i][0] = Int(aux[0])`, `rks[i][1] = Int(aux[1])` (`Convert.ToInt32`: admite espacios y signo `+`; **no** hay comprobación de `min ≤ max` ni de rango). Cualquier excepción (sin guion, cadena vacía, no numérica; un texto con signo menos como `-3-5` da `aux[0]=""`) → «error en datos de entrada» (Error) y se aborta. Con más de un guion (`3-5-7`) se ignoran los trozos sobrantes.

*CU-C60 `FiltrarFicheroAidomnou`* (`Calcular` `VM:170-225`, `Valida` `:665-688`, `S2n` `:691-702`):
```
_salida = false ; Procesadas = Válidas = Tiempo = " " ; validas = {}       // SortedSet<Int> (legacy: BitArray de 3^14 posiciones)
si !CU-C59 → error ; CU-C58 ; pintar pantalla
fichero = picker (.txt, *) ; si cancela → fin                               // los contadores quedan en " "
ctini = ctadm = 0 ; t0 = ahora
para cada línea (StreamReader.ReadLine) :
    si _salida → parar
    col = línea.Trim() ; si col vacío → continuar (no cuenta)               // ¡el legacy NO salta vacías: cuenta y da error de longitud (`Free1X2/UI/Aidomnou.cs:581-585`)!
    ctini += 1
    si col.count < 14 → error "error de longitud=" + col ; parar            // los contadores se muestran igualmente tras parar
    col = col.replace('x','4').replace('X','4')
    si Valida(col) : idx = S2n(col, 14) ; si validas.insert(idx) { ctadm += 1 }     // duplicados no suman
Procesadas = String(ctini) ; Válidas = String(ctadm) ; Tiempo = (ahora - t0).ToString()     // errores de lectura → "No se ha podido leer el fichero de entrada: {msg}" y fin sin contadores
Valida(col): suma=0 ; max=0 ; min=84
    para c en 0..<6 : na = #{ p en 0..<14 : banda=scps[c][p] ; banda != '0' && (banda & col[p]) > 48 }        // AND de los códigos de carácter: ≡ «el signo de col[p] está en la banda»
         si na < rks[c][0] || na > rks[c][1] → false ; suma += na ; min = mín(min,na) ; max = máx(max,na)
    si suma fuera de rks[6] → false ; si (max - min) fuera de rks[7] → false ; true
S2n(col,14): nx = 0 ; para p en 0..<14 : nx = nx*3 + (col[p]=='1' ? 1 : col[p]=='2' ? 2 : 0)             // cualquier otro carácter (X→'4') vale 0 ; MSB primero
```
`banda & col[p]` es la operación de bits sobre los **códigos ASCII**: con `banda='0'+código` y `col[p]` en `'1','2','4'` equivale a `(código & bit) != 0`; para otros caracteres (`'3'`, letras…) el resultado es el de la aritmética de caracteres y **no se validan**: replicar con `UInt16` de unidades UTF-16, no con `Character`.
*Cancelar* (`VM:282-287`): pone `_salida = true`. **No puede tener efecto**: el bucle de `Calcular` es síncrono en el hilo de UI, sin `await` entre líneas, de modo que el botón no se atiende hasta que acaba (el WinForms legacy llamaba a `Application.DoEvents()` por línea, `Free1X2/UI/Aidomnou.cs:595`). En Swift: tarea cancelable con `Task.isCancelled` por línea.
*Grabar resultado* (`GrabarResultadoAsync` `VM:230-249`): sin válidas → Info «No hay columnas válidas que grabar. Ejecute primero Calcular.»; picker «Resultados» `.txt`; escribe `N2s(idx,14)` por cada índice **en orden ascendente** (SortedSet), con `N2s`: `nx2 = nx % 3; nx /= 3; ax = (nx2==1 ? "1" : nx2==2 ? "2" : "X") + ax` (14 dígitos, MSB primero, **0 ⇒ «X»**); `FicheroResultado = nombre`. Texto UTF-8 sin BOM, `\r\n` (T8).
*Exportar columnas* (`VM:253-279`): CU-C59 + CU-C58 + pintar; picker «ColumnasProbables» (tipo «Salida» `.txt`); escribe **6 líneas** (una por columna) con los 14 `Cambia(cps[p][c])` separados por `,` (p. ej. `1,X,12,…`; `-` para 0).
*Salvar / recuperar límites* (`VM:291-339`): `.cnd` de **8 líneas** de texto (`LimColumna1…6`, `LimSuma`, `LimRecorrido`, tal cual escritos); al recuperar se copia cada línea existente (hasta 8) **sin validar**; `FicheroRangos = nombre`; errores «No se han podido guardar/leer los límites: {msg}».

*CU-C61 `AnalizarColumnaContraSeis`* (`Analizar` `VM:408-452`):
```
CU-C59 (si falla → "error en datos de entrada") ; CU-C58 ; pintar
columna = ColumnaResultado.replace('x','4').replace('X','4') ; si count < 14 → error "col. errónea=" + columna
min = 99 ; max = 0 ; suma = 0
para c en 0..<6 : na = mismo conteo que en Valida ; Rk[c] = String(na) ; suma += na ; actualizar min/max
SumaAnalisis = String(suma) ; RecorridoAnalisis = String(max - min)
```
*CU-C62 `CargarGanadorasYNavegar`* (`AbrirGanadorasAsync` `VM:343-379`, `VerColumna` `:719-730`, `GanadoraAnterior/Siguiente` `:383-403`):
```
VerColumna(l): x = l.Trim() ; si x.count < 14 → "" ; x = prefijo 14 ; si algún carácter ∉ "12xX" → "" ; devolver x       // conserva x/X tal cual
picker (.txt, *) ; colgs = [] ; para cada línea : t = VerColumna(línea) ; si "" → error "col.G. errónea" y salir ; colgs.append(t)     // una línea en blanco también es error
si colgs vacío → error "El fichero de ganadoras está vacío."
n = colgs.count ; FicheroGanadoras = nombre ; PaginacionGanadoras = String(n) ; ColumnaResultado = colgs[n-1]        // empieza por la ÚLTIMA ganadora
«-» : si n > 1 { n -= 1 ; mostrar } ; «+» : si n < colgs.count { n += 1 ; mostrar }       // mostrar: Paginacion = String(n) ; ColumnaResultado = colgs[n-1]
```
Caso límite: si la carga falla a medias, `colgs` queda con la lectura parcial pero `n` conserva el valor anterior → «+/-» pueden desbordar (excepción) — corregir en Swift (no vaciar hasta tener lectura correcta).
**Peculiaridades / trampas Swift.** (a) `Tiempo` = `TimeSpan.ToString()` (formato constante `[d.]hh:mm:ss[.fffffff]`, sin fracción si es 0); el legacy lo recortaba a 10 caracteres (`Aidomnou.cs:556-557`). (b) `Calcular` reabre siempre un picker (no recuerda el fichero) y sustituye `validas` por completo en cada ejecución. (c) El rango de límites por defecto `0-14`/`0-84` se corresponde con el máximo posible (6 × 14), no hay validación de que `a ≤ b`. (d) El tamaño máximo del conjunto de índices es 3^14 = 4 782 969 (`BitArray` en el legacy; en Swift un `[UInt64]` de bits ahorra memoria frente a `Set<Int>`). (e) Ningún `Double` interviene en los resultados salvo la comparación de valoraciones (no hay formateo numérico de decimales).
**Área Mac propuesta.** **Columnas probables** (herramienta independiente; en el menú actual cuelga de «Filtros», decisión del dueño) · vista principal con tres paneles (Valoraciones · Matriz 14×6 + límites + análisis · Acciones/Resultado); picker de ficheros con `.fileImporter/.fileExporter`; cálculo en `Task` con progreso y cancelación real.
**Evidencia.** `P:aidomnouViewModel.cs:20-731`, `P:aidomnouPage.xaml:16-298`, `P:aidomnouPage.xaml.cs:12-20`, `W:Controls/PorcentajesHelper.cs:15-43`, `W:MainWindow.xaml.cs:240,411`, `W:Datos/aidomnou.cfg:1-10` (formato legacy), `Free1X2/UI/Aidomnou.cs:156,181-222,556-597` (comportamiento WinForms de referencia).

---

## 3. Tabla resumen: pantalla → área Mac → casos de uso

Propuesta (R2: la decide el dueño). Las áreas son las del PLAN: Boleto · Condiciones y filtros · Cálculo · Reducción · Escrutinio · Análisis · Columnas probables · Herramientas · Online · Ajustes. «Tipo» = vista principal, hoja modal o inspector. Las filas `shell` no tienen ViewModel (§1.11).

| # | Pantalla WinUI | Área Mac (propuesta) | Tipo | Casos de uso |
|---|----------------|----------------------|------|--------------|
| 74 | `MultiplicadorFrm` | Herramientas | vista principal | CU-C01 |
| 75 | `NoVariantesFrm` | Condiciones y filtros | hoja modal | CU-C02, 03, 04 |
| 76 | `OrdenarPorProbabilidadFrm` | Análisis (o Herramientas) | vista principal | CU-C05 a 08 |
| 77 | `ParejasFrm` | Condiciones y filtros (herramienta de ficheros) | vista principal | CU-C09 |
| 78 | `PesosNumFrm` | Condiciones y filtros | hoja modal | CU-C10 (+ 03, 04) |
| 79 | `PosiblesPremiosFrm` | Escrutinio | vista principal (+ hoja) | CU-C11, 12, 13 |
| 80 | `PremiadasFrm` | Análisis | vista principal | CU-C14, 15, 16 |
| 81 | `ProbabilidadPremios` | Análisis | vista principal | CU-C17 |
| 82 | `ReductorFrm` | Reducción | vista principal | CU-C18, 19 |
| 83 | `RentabilidadFrm` | Análisis (o Herramientas) | vista principal | CU-C20, 21 |
| 84 | `ResultadosCalculoMultipleFrm` | Cálculo | hoja modal | — (solo presentación) |
| 85 | `RotacionDeSignosFrm` | Herramientas | vista principal | CU-C22 |
| 86 | `SelecJM` | Herramientas | vista principal | CU-C23 a 27 |
| 87 | `SelectorMS` | Herramientas (o Análisis) | vista principal | CU-C28, 29 |
| 88 | `SignosSeguidosFrm` | Condiciones y filtros | hoja modal | CU-C30 (+ 02, 03, 04) |
| 89 | `SimetriasFrm` | Condiciones y filtros | hoja modal | CU-C31 (+ 03, 04) |
| 90 | `StaInterFrm` | Análisis | inspector / hoja | CU-C32 |
| 91 | `StaSSForm` | Análisis | inspector / hoja | CU-C32 (misma lógica) |
| 92 | `SubirCategoriaFrm` | Reducción (o Herramientas) | vista principal | CU-C33, 34, 35 |
| 93 | `TramificarForm` | Análisis | vista principal (+ hojas) | CU-C36, 37, 38 |
| 94 | `TramificarGraficasFrm` | Análisis | inspector / hoja | CU-C39 |
| 95 | `TransposicionFrm` | Herramientas | vista principal | CU-C40 |
| 96 | `TriosFrm` | Condiciones y filtros (herramienta de ficheros) | vista principal | CU-C41 |
| 97 | `VSignosFrm` | Análisis | vista principal | CU-C42, 43 |
| 98 | `ValoracionFrm` | Condiciones y filtros | hoja modal | CU-C44 a 47 |
| 99 | `VerBoletosEnEditorFrm` | Escrutinio (o Boleto) | vista principal | CU-C48 |
| 100 | `VerBoletos` | Escrutinio (o Boleto) | vista principal | CU-C49 |
| 101 | `VisorAnalisisColumnasAbdonFrm` | Columnas probables | vista principal | CU-C50, 51, 52 |
| 102 | `VisorAnalisisColumnasFrm` | Cálculo | vista principal (tras calcular) | CU-C53, 54 |
| 103 | `VisorEstadisticas` | Condiciones y filtros | hoja / inspector | CU-C55, 56 |
| 104 | `VisorPosiblesPremios` | Escrutinio | hoja modal | CU-C57 |
| 105 | `aidomnou` | Columnas probables | vista principal | CU-C58 a 62 |
| shell | `AcercaDeFrm` | Ajustes / menú de la app | panel estándar «Acerca de» | — |
| shell | `AyudaFrm` | Ayuda (menú nativo) | enlaces al manual | — |
| shell | `CreditosFrm` | Ajustes / menú de la app | hoja desde «Acerca de» | — |

Casos de uso compartidos que consumen varias pantallas: **CU-C03** (`EstadisticasDeFiltro`) y **CU-C04** (`GuardarCondicion`) sirven a todas las condiciones (§1.10); **CU-C32** a las dos pantallas de estadística 5×14 / 4×15; **CU-C58/59** (`aidomnou`) son el mismo motor que reimplementa `GeneraPimViewModel` (ver A-23).

### 3.1 Índice de casos de uso (→ `Free1X2Core/CasosDeUso`)

| CU | Nombre propuesto (tipo Swift) | Pantalla |
|----|-------------------------------|----------|
| CU-C01 | `MultiplicarColumnas` | 74 `MultiplicadorFrm` |
| CU-C02 | `AplicarCondicionNoVariantes` | 75 `NoVariantesFrm` |
| CU-C03 | `EstadisticasDeFiltro` | 75 `NoVariantesFrm` |
| CU-C04 | `GuardarCondicion` | 75 `NoVariantesFrm` |
| CU-C05 | `ValorCentralDeOrdenacion` | 76 `OrdenarPorProbabilidadFrm` |
| CU-C06 | `OrdenarPorProductosOSumas` | 76 `OrdenarPorProbabilidadFrm` |
| CU-C07 | `GrabarColumnasOrdenadas` | 76 `OrdenarPorProbabilidadFrm` |
| CU-C08 | `OrdenacionMultiple` | 76 `OrdenarPorProbabilidadFrm` |
| CU-C09 | `FiltrarColumnasPorParejas` | 77 `ParejasFrm` |
| CU-C10 | `AplicarCondicionPesosNumericos` | 78 `PesosNumFrm` |
| CU-C11 | `CargarColumnasJugadas` | 79 `PosiblesPremiosFrm` |
| CU-C12 | `EscrutarPosiblesPremios` | 79 `PosiblesPremiosFrm` |
| CU-C13 | `CalcularPosiblesPremios` | 79 `PosiblesPremiosFrm` |
| CU-C14 | `FrecuenciasDePremiadas` | 80 `PremiadasFrm` |
| CU-C15 | `JornadasDeFrecuencia` | 80 `PremiadasFrm` |
| CU-C16 | `GrabarColumnasConFrecuencia` | 80 `PremiadasFrm` |
| CU-C17 | `ProbabilidadDePremios` | 81 `ProbabilidadPremios` |
| CU-C18 | `ValidarFicheroDeEntradaReductor` | 82 `ReductorFrm` |
| CU-C19 | `ReducirColumnas` | 82 `ReductorFrm` |
| CU-C20 | `ValorarColumnaRentabilidad` | 83 `RentabilidadFrm` |
| CU-C21 | `CalcularRentabilidad` | 83 `RentabilidadFrm` |
| CU-C22 | `RotarSignosDeColumnas` | 85 `RotacionDeSignosFrm` |
| CU-C23 | `PrepararTablasJM` | 86 `SelecJM` |
| CU-C24 | `SeleccionJMPorCorte` | 86 `SelecJM` |
| CU-C25 | `SeleccionJMPorValor` | 86 `SelecJM` |
| CU-C26 | `UbicarGanadoraJM` | 86 `SelecJM` |
| CU-C27 | `CondicionesJM` | 86 `SelecJM` |
| CU-C28 | `DistribucionDeVecinosMS` | 87 `SelectorMS` |
| CU-C29 | `AnalizarDistribucionMS` | 87 `SelectorMS` |
| CU-C30 | `AplicarCondicionSignosSeguidos` | 88 `SignosSeguidosFrm` |
| CU-C31 | `AplicarCondicionSimetrias` | 89 `SimetriasFrm` |
| CU-C32 | `FormatearMatrizEstadistica` | 90 `StaInterFrm` |
| CU-C33 | `PrepararSubirCategoria` | 92 `SubirCategoriaFrm` |
| CU-C34 | `CalcularSubirCategoria` | 92 `SubirCategoriaFrm` |
| CU-C35 | `GrabarSubirCategoria` | 92 `SubirCategoriaFrm` |
| CU-C36 | `TramificarColumnas` | 93 `TramificarForm` |
| CU-C37 | `FiltrarPorLimites` | 93 `TramificarForm` |
| CU-C38 | `GuardarJornadaLAE` | 93 `TramificarForm` |
| CU-C39 | `PuntosDeCurvaDeTramos` | 94 `TramificarGraficasFrm` |
| CU-C40 | `TransponerColumnas` | 95 `TransposicionFrm` |
| CU-C41 | `FiltrarColumnasPorTrios` | 96 `TriosFrm` |
| CU-C42 | `AnalizarSignosDeFichero` | 97 `VSignosFrm` |
| CU-C43 | `GrabarSignos` | 97 `VSignosFrm` |
| CU-C44 | `PrepararValoracion` | 98 `ValoracionFrm` |
| CU-C45 | `AplicarCondicionValoracion` | 98 `ValoracionFrm` |
| CU-C46 | `ValorarColumna` | 98 `ValoracionFrm` |
| CU-C47 | `ColumnasBase` | 98 `ValoracionFrm` |
| CU-C48 | `FormatearBoletosEnTexto` | 99 `VerBoletosEnEditorFrm` |
| CU-C49 | `CargarYOrdenarBoletos` | 100 `VerBoletos` |
| CU-C50 | `ResumenGlobalAbdon` | 101 `VisorAnalisisColumnasAbdonFrm` |
| CU-C51 | `AgrupacionesAbdon` | 101 `VisorAnalisisColumnasAbdonFrm` |
| CU-C52 | `GenerarCondicionColumnasProbables` | 101 `VisorAnalisisColumnasAbdonFrm` |
| CU-C53 | `ConstruirSeccionesDeAnalisis` | 102 `VisorAnalisisColumnasFrm` |
| CU-C54 | `RefrescarVisorAnalisis` | 102 `VisorAnalisisColumnasFrm` |
| CU-C55 | `EstadisticasDeCumplimiento` | 103 `VisorEstadisticas` |
| CU-C56 | `PrepararFilasEstadisticas` | 103 `VisorEstadisticas` |
| CU-C57 | `MostrarGrupoDePremios` | 104 `VisorPosiblesPremios` |
| CU-C58 | `GenerarSeisColumnasProbables` | 105 `aidomnou` |
| CU-C59 | `ParsearRangosAidomnou` | 105 `aidomnou` |
| CU-C60 | `FiltrarFicheroAidomnou` | 105 `aidomnou` |
| CU-C61 | `AnalizarColumnaContraSeis` | 105 `aidomnou` |
| CU-C62 | `CargarGanadorasYNavegar` | 105 `aidomnou` |

---

## 4. Abiertas — estado y decisión de cada entrada en [`DECISIONES.md`](DECISIONES.md) §3

Puntos que no se han podido verificar con el código o que requieren decisión del dueño (R2). Se citan con `fichero:línea` cuando hay evidencia; «no verificado» = no comprobado (regla de esta fase: sin builds ni ejecución).

**Shell y transversal**

| ID | Tema | Detalle y evidencia | Decisión / acción |
|----|------|---------------------|-------------------|
| A-01 | Multiventana | WinUI es una **única ventana** con un `Frame` y pila de retroceso (`W:MainWindow.xaml.cs:738-752,773-774`) y estado global en `AppState` (§1.2). Una `WindowGroup` de macOS permite varias ventanas | ¿Una ventana (`Window`) o documentos múltiples, cada uno con su `Sesion`? |
| A-02 | Menú Ayuda | El README del proyecto lista un menú «Ayuda» (`W:README.md:24`) que **no existe** en el código (7 menús, `W:MainWindow.xaml.cs:157-163`); la ayuda solo cuelga de `F1` y del botón (`:357,771-772`) | Crear menú Ayuda nativo (propuesto en §1.11) o limitarse a lo actual |
| A-03 | Preferencias de UI | `tema.json` e `idioma.json` en `%LocalAppData%\Free1X2` (§1.5). No hay requisito de compatibilidad de fichero | Confirmar `UserDefaults` y que no se migra nada |
| A-04 | Alcance de la localización | Solo `CreditosFrmPage` usa recursos `x:Uid` es/en (`W:Strings/*/Resources.resw`); todo lo demás son literales en español (§1.5) | ¿Localizar toda la UI en Mac (`.xcstrings`) o solo los créditos? |
| A-05 | Enlaces de ayuda | En «Ayuda» solo la FAQ abre una URL; el resto muestra avisos fijos **en inglés** («disabled for offline operation», `P:AyudaFrmPage.xaml.cs:23-25,38-72`), mientras «Acerca de ▸ Manual» sí abre `https://clubprogol.com/DocWK` (`P:AcercaDeFrmPage.xaml.cs:25,118-121`). La vigencia de las URLs externas **no está verificada** (sin red) | Decidir el destino real de cada enlace (manual local, web o quitar) |
| A-06 | Carpeta `Ganadoras/` | La crea vacía `AsegurarCarpetasDeTrabajo` (`W:App.xaml.cs:102-120`); `W:Datos/` no incluye datos semilla de ganadoras. De ella leen *Estadísticas* (pantalla 103, 13 condiciones) y otras. **No verificado** qué flujo de la app la alimenta (¿copia manual del usuario?) | Definir carpeta y alta de ganadoras en Application Support |
| A-07 | Smoke test | `FREE1X2_SMOKE=1` abre las 109 superficies y registra «SMOKE DONE total=109 ok=109 fail=0» (§1.8). No hay equivalente definido | Decidir si en Mac es un `XCUITest`/argumento de línea de comandos |
| A-08 | Visor de análisis sin acceso | `VisorAnalisisColumnasFrm` solo se abre por el gancho del motor y **consume y borra** su payload (`P:VisorAnalisisColumnasFrmViewModel.cs:112-114`): si el usuario sale del visor pierde el resultado hasta recalcular | ¿Retener el último análisis en la `Sesion` y ofrecer «Ver último análisis»? |
| A-09 | Banderas de análisis | Los 14 flags `Analizar*` salen de `Datos/parametros.free1x2` (`D:VariablesGlobales.cs:100`); `AnalizarTodo`, `AnalizarFormatos123`, `AnalizarFigurasV1X2/Contactos/Pesos` y `AnalizarGruposEquipos` no producen sección (igual en el WinForms, `Free1X2/UI/VisorAnalisisColumnasFrm.cs:67-195`). Pantalla que los edita: `ConfiguracionAnalisisFrm` (otra espec.) | Confirmar que el visor Mac debe mantener esas 14 secciones |

**Pantallas**

| ID | Pantalla | Detalle y evidencia | Decisión / acción |
|----|----------|---------------------|-------------------|
| A-10 | 76, 83 | El manual dice que el combo de aciertos y los valores centrales se recalculan en vivo (`docs/MANUAL_USUARIO.md:377`); el código no. Para Rentabilidad el manual recomienda EmMin 0,136 (`:382`) y el código parte de 0,133 | ¿Se corrige el manual o el comportamiento? |
| A-11 | 82 | `ReductorTM` («menos columnas 2») no es seleccionable aunque el `switch` lo contempla (`P:ReductorFrmViewModel.cs:43-44,60,179`) | ¿Portar la rama muerta? |
| A-12 | 96 | `ColumnaGanadora` de Tríos es fija (`"111X11X222X111"`) y no tiene control, a diferencia de Pares y del legacy (`tCG`) (`P:TriosFrmViewModel.cs:204-206`) | ¿Añadir el campo? |
| A-13 | 85 | El estado «Se han grabado n columnas» se sobrescribe de inmediato con «Preparado» (`P:RotacionDeSignosFrmViewModel.cs:194-203,306-312`) | ¿Replicar o corregir? |
| A-14 | 92 | Los `BitArray` del motor son de 3^15: máx. 15 signos por fichero aunque la pantalla ofrece 16 casillas; los niveles se pierden al tocar casillas | Decidir límite y política de niveles |
| A-15 | 79, 83, 86 y otras | Redondeo y formato numérico de .NET (T4, T7: `Math.Round` bancario, `F2` «away from zero», `Float.ToString()` más corto) se han descrito por conocimiento de la plataforma, **no ejecutando código** | Validar con los golden-master antes de fijar el formateador |
| A-16 | 77, 80, 83, 86, 97, 105 | Varios cálculos corren sin cancelación o con «Cancelar» ineficaz (p. ej. `aidomnou`: bucle síncrono en el hilo de UI, `P:aidomnouViewModel.cs:191-214`, frente a `DoEvents` del legacy `Free1X2/UI/Aidomnou.cs:595`) | Cancelación cooperativa + progreso en Mac (T12): ¿se acepta como mejora? |
| A-17 | 99 | El nº de boletos suma 1 también cuando `n` es múltiplo de 8 (`P:VerBoletosEnEditorFrmViewModel.cs:73-128`), igual que el legacy (`Free1X2/UI/VerBoletosEnEditorFrm.cs:17-18`) | R1: preservar (golden) o corregir con visto bueno del dueño |
| A-18 | 98 | «Valor» se calcula sobre el filtro vivo del grupo, no sobre un temporal | Preservar o corregir |
| A-19 | 103 | `Estadistica.Cumplimiento` (Float32) se muestra con `ToString()` sin formato ni cultura (`P:VisorEstadisticasViewModel.cs:81`) y el orden de los ficheros es el de `GetFiles` (`D:MotorCalculo/Estadisticas/CalculadorEstadisticas.cs:40-49`) | Fijar formato (¿2 decimales?) y orden alfabético en Mac |
| A-20 | 104 | `PosiblesPremiosContenedor.CalcularValorTotal` no tiene llamantes y usa `+` donde cabría `*` (`D:Escrutinio/PosiblesPremiosContenedor.cs:67-95`) | No portar |
| A-21 | 105 | `aidomnou.cfg` **no se lee ni se escribe** en el port: los límites `145/11,9,8/65/9,6,5` son constantes (`P:aidomnouViewModel.cs:66`) aunque la pantalla anuncia que el fichero es editable (`P:aidomnouPage.xaml:209`). El legacy lo leía y reescribía al abrir (`Free1X2/UI/Aidomnou.cs:156,181-222`; `GetConfig` devuelve siempre `false`, así que `SetConfig` siempre vuelve a escribirlo) | ¿Hacer los límites editables en Mac (Ajustes) o dejarlos fijos? |
| A-22 | 105 | Diferencias port ↔ legacy: el port omite las líneas vacías del fichero de entrada (legacy: cuentan y dan «error de longitud», `Free1X2/UI/Aidomnou.cs:581-585`) y muestra `Tiempo` completo (legacy: 10 caracteres, `:556-557`); con `NumeroPartidos < 14` la generación desborda; si la carga de ganadoras falla a medias, `_colgsR` queda a medias con el índice anterior (`P:aidomnouViewModel.cs:343-379`) | Decidir cuál es la referencia (R1) |
| A-23 | 105 / `FiltroPim` | `GeneraPimViewModel` reimplementa el mismo motor de 6 columnas (`P:GeneraPimViewModel.cs:114-117`, comentarios `:17-24`) | Una sola implementación en `Free1X2Core` compartida por ambas pantallas |
| A-24 | 102 | `VisorAnalisisColumnasFrm` y `VisorAnalisisColumnasAbdonFrm` son dos visores distintos (secciones por tipo de análisis frente a informe ABDON de EstuCol); la espec. los trata por separado | Confirmar que no se fusionan |
| A-25 | 74–105 | La lista pedida (`ls *ViewModel.cs | sed -n '74,108p'`) contiene solo 32 elementos porque hay 105 ViewModels para 108 páginas | Las 3 páginas sin ViewModel se documentan en §1.11 |

---

## 5. Recuento

| Concepto | Cantidad |
|----------|----------|
| Pantallas con ViewModel documentadas (74–105) | **32** |
| Páginas de shell sin ViewModel documentadas (Acerca de, Ayuda, Créditos) | **3** |
| Casos de uso extraídos (`CU-C01`…`CU-C62`) | **62** |
| Entradas de menú (7 menús: 53 de navegación + 11 conmutables) | **64** |
| Botones de la barra de herramientas | 55 (§1.4) |
| Entradas abiertas | 25 |
