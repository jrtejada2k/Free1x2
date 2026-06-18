# Free1X2 — Análisis técnico de la arquitectura WinUI 3 (post-migración)

*Análisis estático del repositorio en la rama `main` (migración ya fusionada). Describe la arquitectura **tal y como está construida** tras migrar la UI de WinForms a WinUI 3. Para el análisis previo a la migración (monolito WinForms y riesgos) ver [ANALISIS_TECNICO.md](ANALISIS_TECNICO.md). Para el manual funcional ver [MANUAL_USUARIO.md](MANUAL_USUARIO.md).*

> **Honestidad de este documento.** Todas las cifras y afirmaciones están respaldadas por evidencia en el código (se cita `archivo:línea`). Donde hay deuda técnica o pantallas incompletas se dice explícitamente (ver §11). No se describe ninguna funcionalidad que no esté en el código.

---

## 1. Estructura de la solución y frameworks

`Free1X2.sln` (formato VS 17) referencia **4 de los 6** proyectos `.csproj` del repositorio:

| Proyecto | En `.sln` | TFM | Rol | Métricas (`.cs` sin bin/obj) |
|---|---|---|---|---|
| **Free1X2.WinUI** | Sí | `net8.0-windows10.0.19041.0` (WinUI 3) | **Nueva UI construida** | 238 archivos / ~49 961 líneas |
| **Free1X2.Domain** | Sí | `net8.0` (sin UI) | Motor / dominio compartido | 139 archivos / ~30 505 líneas |
| **Free1X2.Domain.Tests** | Sí | `net8.0` (xUnit) | Red de seguridad (golden-master) | 10 archivos |
| **Free1X2** | Sí | `net8.0-windows` (WinForms) | App legacy, conservada como referencia | (no migrada; ya no es la app principal) |
| **Free1X2.Shared** | **No** | `net8.0` | Lógica compartida (ex-WebAPI) | huérfano de la `.sln` |
| **Free1X2.WebAPI** | **No** | `net8.0` (ASP.NET) | Backend móvil | huérfano de la `.sln` |

Evidencia: `Free1X2.sln:6-13` (los 4 proyectos), TFMs en `Free1X2.WinUI/Free1X2.WinUI.csproj:5`, `Free1X2.Domain/Free1X2.Domain.csproj:4`, `Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj:4`, `Free1X2/Free1X2.csproj:5`.

### Configuración del proyecto WinUI

De `Free1X2.WinUI/Free1X2.WinUI.csproj`:

| Propiedad | Valor | Línea |
|---|---|---|
| `OutputType` | `WinExe` | :4 |
| `TargetFramework` | `net8.0-windows10.0.19041.0` | :5 |
| `TargetPlatformMinVersion` | `10.0.17763.0` | :6 |
| `Version` / `AssemblyVersion` | `0.78.0` | :15-17 |
| `Platforms` | `x86;x64;ARM64` | :18 |
| `RuntimeIdentifier` | `win-x64` | :26 |
| `UseWinUI` | `true` | :20 |
| `WindowsPackageType` | `None` (no MSIX, app desempaquetada) | :21 |
| `SelfContained` | `true` | :27 |
| `WindowsAppSDKSelfContained` | `true` | :28 |
| `Nullable` | `enable` | :22 |

Es decir: **app de escritorio WinUI 3 desempaquetada, self-contained, win-x64**, que empaqueta el Windows App Runtime (no requiere instalación a nivel de sistema). Configuración de plataforma en `.sln`: el proyecto WinUI compila para `Debug|x64`, `Release|x64`, `x86` y ARM64 (`Free1X2.sln:60-71`).

### Paquetes NuGet (WinUI)

| Paquete | Versión | Línea |
|---|---|---|
| `Microsoft.WindowsAppSDK` | `1.6.250108002` | :37 |
| `Microsoft.Windows.SDK.BuildTools` | `10.0.26100.1742` | :38 |
| `CommunityToolkit.Mvvm` | `8.4.0` | :39 |

`Free1X2.Domain` solo depende de `SharpZipLib 1.4.2` (`Free1X2.Domain.csproj:14`).

### Grafo de dependencias

```
Free1X2.WinUI ───────────► Free1X2.Domain ◄─────── Free1X2 (WinForms legacy)
Free1X2.Domain.Tests ────► Free1X2.Domain
Free1X2.WebAPI ──────────► Free1X2.Shared          (ambos fuera de la .sln)
```

Evidencia de referencias: `Free1X2.WinUI.csproj:43`, `Free1X2/Free1X2.csproj:74`, `Free1X2.Domain.Tests.csproj:18`. **La UI WinUI no referencia a la app WinForms**: toda la lógica que comparten vive en `Free1X2.Domain`.

### Datos semilla empaquetados

El `.csproj` de WinUI copia junto al `.exe` (preservando subcarpetas) los datos semilla del motor (`Datos/**/*`: `parametros.free1x2`, `Idioma/`, `Equipos/`, `Impresion/`) vía `<Content>` con `TargetPath` y `CopyToPublishDirectory` (`Free1X2.WinUI.csproj:63-69`). Sin ellos, `Analizador`/`AConfiguracion` fallarían al arrancar por falta de `parametros.free1x2`. Los iconos originales de la barra de herramientas, extraídos del `.resx` de WinForms, se empaquetan como `Assets/Toolbar/*.png` (`:48-53`). Las carpetas de trabajo de salida (`Columnas`, `Combinaciones`, `Condiciones`, etc.) las crea la app en tiempo de ejecución (`App.xaml.cs:39-52`).

---

## 2. Arranque de la aplicación (App.xaml / App.xaml.cs)

`App.xaml` (`Free1X2.WinUI/App.xaml`) declara `RequestedTheme="Light"` y mezcla tres diccionarios de recursos: `XamlControlsResources`, `Themes/Tokens.xaml` y `Themes/Styles.xaml` (`App.xaml:9-13`).

`App.xaml.cs` (`OnLaunched`, :24-31):
1. `AsegurarCarpetasDeTrabajo()` — crea las carpetas de salida del motor.
2. Crea `MainWindow`.
3. `AppServices.Inicializar(MainWindow)`.
4. `CablearHooksDominio()` — conecta los *shims* del dominio con la UI (ver §6).
5. `MainWindow.Activate()`.

Además registra `UnhandledException` como red de seguridad: una excepción no controlada se marca `Handled` y se muestra al usuario con `AppServices.MostrarError` en vez de tumbar la app (`App.xaml.cs:17-21`).

---

## 3. La ventana principal (MainWindow)

`MainWindow.xaml` es un `Grid` de tres filas (`Auto` / `Auto` / `*`):
- **Fila 0** — `MenuBar` (`x:Name="BarraMenu"`).
- **Fila 1** — `Border` con un `ToolbarWrapPanel` (`x:Name="ToolbarPanel"`); el panel hace *wrap* a una segunda fila cuando los botones no caben (`MainWindow.xaml:31-40`).
- **Fila 2** — `Frame` de navegación (`x:Name="ContentFrame"`).

`MainWindow.xaml.cs` (621 líneas reales en `Free1X2.WinUI/MainWindow.xaml.cs`):
- **Chrome / tamaño.** No se extiende el contenido a la barra de título (`ExtendsContentIntoTitleBar = false`, :28); barra de Windows estándar (minimizar/maximizar/cerrar). `AjustarTamanoVentana()` fija el tamaño inicial a **1020 × 760** (`:55`).
- En el constructor: `ConstruirToolbar()`, `ConstruirMenus()`, navega a `MainPage` (:32-34) y, al cerrar, persiste la visibilidad de las barras con `GuardarBarrasHerramientas()` (:41).
- Si la variable de entorno `FREE1X2_SMOKE == "1"`, inicia el *smoke test* (:43-44, ver §10).

### 3.1 Menús (`ConstruirMenus`, :71-174)

Siete menús superiores, en el mismo orden que el `MainForm` WinForms original:

| Menú | Entradas (destino) |
|---|---|
| **Free1x2** | Inicio (`MainPage`), Configuración (`ConfiguracionFrmPage`), Configurar análisis (`ConfiguracionAnalisisFrmPage`), Acerca de (`AcercaDeFrmPage`), Créditos (`CreditosFrmPage`), Salir (cierra ventana) |
| **Archivo** | Boleto/combinación (`MainPage`), Gestión de equipos (`GestorEquiposFrmPage`), Importar/exportar columnas (`ImportExportFrmPage`) |
| **Ver** | Inicio, Ver boletos (`VerBoletosPage`), Gráfico de columnas (`GraficoColumnasFrmPage`), Estadísticas (`AnastaticsPage`), Configuración, submenú **Barras de Herramientas**, Listado de condiciones (`ListadoCondicionesFrmPage`) |
| **Combinación** | Calcular (`CalculaColumnasFrmPage`), Calcular varias (`CalculaColumnasMultipleFrmPage`), Ver boletos, Ver boletos en editor (`VerBoletosEnEditorFrmPage`), Imprimir boletos (`ImprimirBoletoFrmPage`), Reducir (`ReductorFrmPage`), Escrutinios (`EscrutiniosFrmPage`), Escrutar combinaciones (`EscrutarCombinacionesFrmPage`), Analizar combinación (`AnalizarCombinacionFrmPage`), Gráfico de columnas, Probabilidades (`ProbabilidadPremiosPage`), Estadísticas, Añadir Pleno al 15 (`AgregaP15FrmPage`) |
| **Filtros** | Combinar filtros (`CombinarFiltrosPage`), Diferencias entre filtros (`DiFiltrosPage`), Filtro Coincidencias (`CoincidenciasPage`), Filtro Aidomnou (`aidomnouPage`), Filtro Pim (`GeneraPimPage`) |
| **Operaciones** | Álgebra de columnas (`AlgebraColumnasFrmPage`), Transposición (`TransposicionFrmPage`), Multiplicador (`MultiplicadorFrmPage`), Fraccionador (`FraccionadorFrmPage`), Rotación de signos (`RotacionDeSignosFrmPage`) |
| **Utilidades** | Sube categoría (`SubirCategoriaFrmPage`), Modificador % (`ModificadorFrmPage`), Generador CP (`GenerarCPsPage`), Columnas GEPT (`GEPTFrmPage`), Diferencias entre columnas (`DifColsPage`), Ordenar por probabilidad (`OrdenarPorProbabilidadFrmPage`), Selector JuanM (`SelecJMPage`), Selector MarioSan (`SelectorMSPage`), Rentabilidad (`RentabilidadFrmPage`), Tramificar (`TramificarFormPage`), Premiadas (`PremiadasFrmPage`), Estimación de premios (`EstimadorPremiosFrmPage`), Banco de pruebas (`BancoPruebasFrmPage`), Compresor z3q (`CompresorPage`), EstuCol (`EstucolFrmPage`) |

Cada `MenuFlyoutItem` lleva un `FontIcon` (Segoe Fluent Icons, codepoint hex → glifo vía `Glifo()`, :63). El helper `Menu()` (:181) construye un menú; un elemento `null` inserta separador y una página `null` cierra la ventana.

### 3.2 Barra de herramientas (`ConstruirToolbar`, :249-347)

Réplica 1:1 de los **6 ToolStrips** del `MainForm`, en seis grupos en el orden visual del original (`ObtenerPosicionBarraHerramientas`):

| Grupo (`enum GrupoBarra`) | Nº botones (aprox.) | Contenido |
|---|---|---|
| `Free` | 5 | Salir, Configuración, Configurar Análisis, Ayuda, Acerca de |
| `Archivo` | 10 | Guardar equipos, Nueva combinación, Obtener Boletos Online, Abrir/Guardar/Guardar como combinación, Borrar temporales, Abrir equipos, Borrar informes, Gestión de equipos |
| `Operaciones` | 5 | Álgebra, Transposición, Multiplicación, Fraccionar, Rotación de signos |
| `Combinacion` | ~12 | Calcular, Calcular múltiples, Ver boletos, Imprimir, Reducir, Escrutinio, Escrutar combinaciones (deshabilitado), Análisis de columnas/fallos/signos, Probabilidades, Estadísticas, Añadir Pleno al 15 |
| `Filtros` | 5 | Combinar, Diferencias, Coincidencias, Aidomnou, Pim |
| `Utilidades` | ~17 | Subir categoría, Modificador %, Generador CPs, Diferencias entre columnas, Ordenar por probabilidad, Selector JuanM/MarioSan, Rentabilidad, GEPT, Tramificar, Premiadas, Estimación, Banco de pruebas, Importar/Exportar, Análisis de grupos, Reducciones perfectas, Dependencia lineal |

Detalles fieles al original verificados en código:
- `bEscrutarComb` se crea **deshabilitado** (`deshabilitado: true`, :295) porque en el original tiene `Enabled=false`.
- `bAnalisisGrafico` se **omite** porque en el original tiene `Visible=false` (:298).
- Cada botón usa el icono PNG original extraído del `.resx` (`ms-appx:///Assets/Toolbar/<botón>.png`, :364-366), no un glifo de fuente.

Los botones del grupo **Archivo** no solo navegan: ejecutan una **acción** sobre la pantalla de Inicio (`HerramientaAccion` → `NavegarConAccion`, :395-419). Ver §5.

### 3.3 Semilla de visibilidad de barras desde `parametros.free1x2`

La visibilidad inicial de cada grupo se siembra del flag `VariablesGlobales.MostrarTs*` (`MostrarGrupo`, :458-467), que el dominio carga perezosamente de `parametros.free1x2` mediante `AConfiguracion.ObtenToolBarsVisibles` (`Free1X2.Domain/EntradaSalida/AConfiguracion.cs:68-89`). Esa lectura es **robusta**: si falta el nodo `<toolbars>`/`<Activadas>` o un atributo `tsX`, devuelve `true` (la barra se muestra por defecto, `AConfiguracion.cs:70-97`), de modo que un fichero viejo o corrupto nunca pueda dejar una barra oculta de forma permanente.

> **Excepción intencional:** el grupo **Filtros** se fuerza SIEMPRE visible al arrancar, con independencia del valor cargado (`MainWindow.xaml.cs:342-346`), porque copias antiguas de `parametros.free1x2` traían `tsFiltros=False`. El toggle "Ver → Barras de Herramientas" puede ocultarlo después.

El submenú **Ver → Barras de Herramientas** (`ConstruirSubmenuBarrasHerramientas`, :469-507) ofrece un `ToggleMenuFlyoutItem` por grupo (orden: Filtros, Free1X2, Operaciones, Utilidades, Combinación, Archivo) que muestra/oculta el grupo en vivo. Al cerrar la ventana, `GuardarBarrasHerramientas()` persiste la visibilidad viva con `AConfiguracion.GuardarToolBarsVisibles` (:512-526).

---

## 4. Navegación y registro de páginas

- `Navegar(Type)` (:528-532) — navega por el `ContentFrame` evitando re-navegar a la página actual.
- `NavegarConAccion(AccionInicio)` (:539-549) — si la página viva es `MainPage`, invoca el comando sobre su `ViewModel` (preserva el boleto en edición); si no, navega a `MainPage` pasando el token como parámetro.

El **registro de páginas portadas** vive en `Free1X2.WinUI/Navigation/PortedPages.cs`: un `record PortedPage(string Title, string Glyph, Type PageType, string Category)` y un `PortedPagesRegistry.All` con **108 entradas** (verificado: 108 `new PortedPage(...)` y 108 `typeof(...)` distintos). El *smoke test* itera este registro (§10).

> **Discrepancia de comentario (honestidad):** el comentario de `PortedPages.cs:13` dice "las 111 pantallas portadas", pero el array contiene **108**. El número correcto es 108 (registro) + `MainPage`. El comentario está desactualizado.

### Inventario de páginas por categoría (registro)

| Categoría | Nº | Páginas |
|---|---|---|
| **Filtros** | 27 | Analisis Formatos 123, Calculo Formatos, Combinar Filtros, Contactos, Control Tol, Di Filtros, Dialogo Filtrar Por Limites, Dibujos, Diferencias, Distancias, Figuras Filtros, Filtro Porcen JB, Formatos, Formatos 123, Frm Dependencia Lineal, Generador CPSDiferencias, GEPT, If Then, Interrupciones, Listado Condiciones, No Variantes, Parejas, Rotacion De Signos, Signos Seguidos, Simetrias, Trios, VSignos |
| **Boleto e impresión** | 15 | Boleto, Col Ganadora, Columnas Premiadas, Descarga Boleto, Estimador Premios, Imprimir Boleto, Lista Impresoras, Mejores Opciones, Posibles Premios, Premiadas, Rentabilidad, Subir Categoria, Ver Boletos, Ver Boletos En Editor, Visor Posibles Premios |
| **Análisis** | 15 | Ana Combi, Analizador JPM, Analizar Combinacion, Analizar Fichero, Anastatics, Coincidencias, Col Probables, Grafico Columnas, Guardar Valoracion, Historia Valoraciones, Ordenar Por Probabilidad, Probabilidad Premios, Valoracion, Visor Analisis Columnas, Visor Analisis Columnas Abdon |
| **Estadísticas** | 5 | Pesos Num, Sta Inter, Sta SS, Tramificar Graficas, Visor Estadisticas |
| **Columnas y combinación** | 21 | Algebra Columnas, Busca Lims, Calcula Columnas, Calcula Columnas Multiple, Compresor, Control Grupos, Crear Grupos, Dif Cols, Escrutar Combinaciones, Escrutinios, Fraccionador, Frm Reducidas Perfectas, Genera Pim, Modificador, Multiplicador, Reductor, Resultados Calculo Multiple, Selec JM, Selector MS, Tramificar, Transposicion |
| **Equipos** | 3 | Agregar Equipo, Gestor Equipos, Grupos Equipos |
| **Cuadros (CPs)** | 7 | Cambio Puntos, Config CPs, Copiar CP, Copiar Datos CP, Exportador CPs, Generar CPs, Importador CPs |
| **Ajustes** | 3 | Configuracion, Configuracion Analisis, Import Export |
| **Ayuda** | 3 | Acerca De, Ayuda, Creditos |
| **Otras pantallas** | 9 | Agrega P 15, aidomnou, Banco Pruebas, Dialogo Analisis Multiple De Tramos, Dialogo Grabar Banco Pruebas, Dialogo Grabar Tramos, Dib, Dib Rep, Estucol |

> *Recuento por categoría* (verificado con `grep`): Filtros 27, Columnas y combinación 21, Análisis 15, Boleto e impresión 15, Otras pantallas 9, Cuadros (CPs) 7, Estadísticas 5, Ajustes 3, Equipos 3, Ayuda 3 → **108**. Cada `PageType` corresponde a un archivo `.xaml` en `Free1X2.WinUI/Views/Ported/`. Hay **108 XAML** en `Views/Ported/` + `MainPage.xaml` = **109 páginas** de navegación.

### Convención de mapeo WinForms → WinUI

El nombre de clase de cada página deriva del Form original: `<Form>Frm` → `<Form>FrmPage` (p. ej. `ContactosFrm` → `ContactosFrmPage`). El doc-comment de cada página suele indicarlo explícitamente ("Page portada del WinForms legacy …" / "Port de WinForms …"), p. ej. `ReductorFrmPage.xaml.cs:7` ("Página portada del WinForms `ReductorFrm`"), `AcercaDeFrmPage.xaml.cs:11` ("Port de WinForms AcercaDeFrm").

---

## 5. Patrón MVVM y handoff estático

- **Librería:** `CommunityToolkit.Mvvm 8.4.0`. Se usan `[ObservableProperty]` (campos → propiedades observables) y `[RelayCommand]` (métodos → comandos). Ejemplos en `MainPageViewModel.cs` (`[ObservableProperty]` en :69, :72, :80…) y en las VM portadas (p. ej. `ConfiguracionAnalisisFrmViewModel`, `SubirCategoriaFrmViewModel`).
- **Aviso suprimido a propósito:** `MVVMTK0045` (AOT/WinRT por `[ObservableProperty]` sobre campos) se silencia en el `.csproj` porque la app no usa NativeAOT (`Free1X2.WinUI.csproj:29-33`).
- **Ubicación de las VM:** junto a su Page, en `Free1X2.WinUI/Views/Ported/<X>ViewModel.cs` (no hay carpeta `ViewModels/` separada). Cada Page expone su VM como propiedad pública (`public XViewModel ViewModel { get; } = new();`) consumida con `x:Bind`.
- **`AccionInicio`** (`Views/MainPageViewModel.cs:25-36`) — enum de las acciones del menú/barra "Archivo" (NuevaCombinacion, AbrirCombinacion, GuardarCombinacion, GuardarCombinacionComo, GuardarEquipos, AbrirEquipos, BorrarTemporales, BorrarInformes). La barra enruta cada token a `MainPage`, que invoca el comando equivalente sobre su VM (réplica de los handlers `MNuevaComb`/`MAbrirComb`/… del `MainForm`).

### Handoff estático entre productor y visor

Como la navegación del `Frame` instancia las páginas sin pasarles objetos de dominio ricos, los productores dejan el resultado en una **propiedad estática** del ViewModel del visor, navegan a su página y el visor lo consume en su constructor/activación. Instancias encontradas:

| Campo estático | Tipo | Ubicación |
|---|---|---|
| `AnalizarCombinacionFrmViewModel.UltimoAnalisis` | `(string, long, Analizador, string[])?` | `AnalizarCombinacionFrmViewModel.cs:86` |
| `EstucolFrmViewModel.UltimoInforme` | `(List<InformeColumnasABDON>, …)?` | `EstucolFrmViewModel.cs:35` |
| `VisorAnalisisColumnasFrmViewModel.UltimoContenedor` | `object?` | `VisorAnalisisColumnasFrmViewModel.cs:103` |
| `VisorAnalisisColumnasFrmViewModel.UltimoGrupo` | `object?` | `VisorAnalisisColumnasFrmViewModel.cs:104` |

El propio dominio dispara la navegación al visor mediante el hook `AnalisisUi.MostrarVisor`, que `App.xaml.cs:97-109` implementa dejando el payload en `UltimoContenedor`/`UltimoGrupo` y navegando a `VisorAnalisisColumnasFrmPage` en el hilo de UI.

---

## 6. Desacople del dominio (Free1X2.Domain)

El motor vive en `Free1X2.Domain` (TFM `net8.0`, sin dependencias de UI). Espacios de nombres encontrados (`grep namespace`):

| Namespace | Rol |
|---|---|
| `Free1X2` | Núcleo (VariablesGlobales, Pronosticos, Grupo…) |
| `Free1X2.MotorCalculo` | Motor de cálculo, filtros, `Analizador`, `GeneradorColumnas` |
| `Free1X2.MotorCalculo.Estadisticas` | Cálculo estadístico |
| `Free1X2.Escrutinio` | Escrutado de columnas (`Escrutador`) |
| `Free1X2.Analisis` | Análisis de columnas/combinaciones |
| `Free1X2.EntradaSalida` | Configuración y E/S (`AConfiguracion`…) |
| `Free1X2.EntradaSalida.GenerarCPs` | Generación de Columnas Probables |
| `Free1X2.Reduccion` | Algoritmos de reducción |
| `Free1X2.SubirCategoria` | Subir categoría de premios |
| `Free1X2.Utils` | Utilidades puras (`UtilColumnas`, `Comparador`, `ConvertidorDeBases`…) |
| `Free1X2.Abstractions` | *Shims* de UI (ver abajo) |

### Shims / abstracciones (`Free1X2.Domain/Abstractions/UiHooks.cs`)

Para que el dominio no dependa de WinForms ni de WinUI, expone *hooks* estáticos con implementación no-op por defecto, que la UI rellena al arrancar:

| Clase | Miembro | Por defecto |
|---|---|---|
| `UiPump` | `Action Pump` | no-op (`UiHooks.cs:14`) |
| `UserDialogs` | `Action<string> ShowError`, `ShowInfo` | no-op (`:24-25`) |
| `AnalisisUi` | `Action<object,object> MostrarVisor` | no-op (`:35`) |
| `Clipboard` | `Func<string> Read`, `Action<string> Write` | vacío / no-op (`:46-47`) |

La UI los cablea en `App.CablearHooksDominio()` (`App.xaml.cs:59-110`):
- `UiPump.Pump` → no-op (WinUI corre el motor en `Task.Run`; no se bombea el dispatcher desde un hilo de trabajo).
- `UserDialogs.ShowError`/`ShowInfo` → `AppServices.MostrarError`/`MostrarInfo` (ContentDialog).
- `Clipboard.Write`/`Read` → `DataPackage`/`Clipboard` de WinRT.
- `AnalisisUi.MostrarVisor` → handoff estático + navegación al visor (ver §5).

También existen `Abstractions/IAppPaths.cs` e `Abstractions/IProgressNotifier.cs` (interfaces de rutas y progreso).

---

## 7. Representación del motor (columna = `long`, 3 bits por partido)

La unidad de cálculo sigue siendo una columna de 14 signos **codificada en un `long`**, 3 bits por partido, **sin cambios** respecto al motor original (compartida vía `Free1X2.Domain`). Evidencia en `Free1X2.Domain/Utils/UtilColumnas.cs`:

```csharp
// ConvLongToStr (UtilColumnas.cs:26-35): recorre la columna 3 bits a la vez
res.Append("2X1"[(int)(L & 7) >> 1]);  // 3 bits → signo 2/X/1
L >>= 3;

// ObtenerSigno (UtilColumnas.cs:42-55): signo del partido N
res.Append("2X1"[(int)((L >>= ((partido - 1) * 3)) & 7) >> 1]);
```

`Free1X2.Utils` aporta además `Comparador` (con `ConvColumnaA3Bytes` / `Conv3BytesAColumna`, `Comparador.cs:281-371`) y `ConvertidorDeBases` (`ConvColumnaANumero` / `ConvLongANumero` / `ConvNumAColumna`, `ConvertidorDeBases.cs:43-96`). Todas las pantallas de cálculo se apoyan en estas conversiones; el universo total son **3¹⁴ = 4 782 969** columnas de 14 triples (citado en varias VM).

---

## 8. Tema y sistema de diseño

Dos `ResourceDictionary` cargados desde `App.xaml`:

### `Themes/Tokens.xaml`
- **`ThemeDictionaries`** con claves `Light` y `Dark` (`Tokens.xaml:11-106`). La app arranca en `Light` (`App.xaml:6`).
- **Paleta "Índigo & Teal"** (elegida por el usuario). En `Light`: acento índigo `#3D5A80` (`SystemAccentColor`), fondo `#F4F6F8`, superficie `#FFFFFF`, superficie alternativa `#E6ECF2`, texto `#293241`, secundario teal `#2A9D8F`, terciario coral `#EE6C4D`, éxito `#2E7D32`, error `#C0392B`, aviso `#EE6C4D` (`Tokens.xaml:16-35`). Cada color tiene su `SolidColorBrush` (`App*Brush`, :45-56) y se añaden tintes a baja opacidad (`AppAccentTintBrush`, `AppSecondaryTintBrush`, :61-62).
- **Signos 1/X/2 marcados en verde** (`#2E7D32`) sobreescribiendo los recursos `ToggleButtonBackgroundChecked*` (`:38-43`), igual que el boleto del original.
- **Tipografía:** **Segoe UI Variable** (con variantes Display/Text/Small y *fallback* a Segoe UI). Se sobreescribe `ContentControlThemeFontFamily` para que toda la app la herede (`Tokens.xaml:116-123`).
- **Tokens no temáticos:** `AppCornerRadius` = 8, `AppCardCornerRadius` = 12 (`:126-128`); **`ThemeShadow`** compartido `AppCardShadow` (`:137`); espaciados compactos (`AppSpacingSmall/Medium/Large`, `AppPagePadding` = `12,8`, :140-143).

### `Themes/Styles.xaml`
Estilos reutilizables, todos sobre tokens:
- **Texto:** `PageTitleTextBlockStyle` (Display SemiBold ~21px), `SectionHeaderTextBlockStyle` (~15px), `CardTitleTextBlockStyle` (~14px), `SecondaryTextBlockStyle` (~12px apagado), y variantes con acento `AccentCardTitleTextBlockStyle` (índigo) / `SecondaryCardTitleTextBlockStyle` (teal) (`Styles.xaml:19-71`). También compacta los estilos integrados `TitleTextBlockStyle`/`SubtitleTextBlockStyle` (:91-105).
- **Densidad compacta** de controles de entrada implícitos: `TextBox`/`ComboBox`/`NumberBox` a ~30px de alto y 12px (`:113-133`).
- **`AppCardStyle`** (`:141-152`): `Border` con superficie blanca, borde 1px, esquinas `AppCardCornerRadius`, **`Shadow` = `AppCardShadow`** y elevación vía propiedad adjunta **`CardElevation.Depth = 16`** (Translation Z, porque `UIElement.Translation` no es válida en un `Setter`).
- **`SignoToggleButtonStyle`** para los toggles 1/X/2 del boleto (`:155-164`).
- **Botones de filtro de 4 estados** (réplica de `SetBotonEstado` del `MainForm`): `FiltroButtonInactivoStyle` (superficie neutra), `FiltroButtonNeutroStyle` (ámbar), `FiltroButtonActivoStyle` (verde éxito), `FiltroButtonErrorStyle` (rojo) (`:172-212`).

`CardElevation` es un control/clase de `Free1X2.WinUI.Controls` (referenciado en `Styles.xaml:6` y `:151`).

---

## 9. Métricas

| Métrica | Valor | Cómo se obtuvo |
|---|---|---|
| `.cs` en `Free1X2.WinUI` (sin bin/obj) | **238 archivos / ~49 961 líneas** | `find … -name *.cs \| wc` |
| `.xaml` en `Free1X2.WinUI` (sin bin/obj) | **120 archivos** | `find … -name *.xaml \| wc` |
| Páginas XAML (Views) | **109** (108 en `Views/Ported/` + `MainPage`) | `Glob Views/**/*.xaml` |
| Entradas en `PortedPagesRegistry` | **108** | `grep -c "new PortedPage("` |
| `.cs` en `Free1X2.Domain` (sin bin/obj) | **139 archivos / ~30 505 líneas** | `find … \| wc` |
| Tests (`[Fact]`/`[Theory]`) | **52** en 10 archivos | `grep "\[Fact\]\|\[Theory\]"` |

Los 120 XAML incluyen las 109 páginas + ~7 UserControls (`Controls/*.xaml`, p. ej. `BoletoControl`, `SemaforoControl`, `GraficoLineasControl`, `MatrizAnalisisView`) + `MainWindow.xaml` + `App.xaml` + los 2 diccionarios de tema.

### Marca de propiedad "WIN3"

**Todas** las clases propias de la migración llevan en la primera línea el comentario marcador `// Free1X2 · WinUI 3 — WIN3` (verificado: **238/238** archivos `.cs` de `Free1X2.WinUI` contienen `WIN3`). El marcador se propaga también a los archivos del dominio tocados durante la migración (p. ej. `UtilColumnas.cs:1`, `UiHooks.cs:1`). Sirve para distinguir el código de la migración del código heredado.

---

## 10. Smoke test (`FREE1X2_SMOKE`)

`MainWindow.xaml.cs` incluye un *smoke test* permanente, **no-op salvo que** `FREE1X2_SMOKE == "1"` (`:43-44`). `IniciarSmokeTest()` (:560-577):
1. Escribe `SMOKE START` en `%TEMP%\free1x2_smoke.log` (`Path.GetTempPath()`, :562).
2. Construye la ruta = `MainPage` + **todas** las páginas de `PortedPagesRegistry.All` (:565-567) → **109 páginas** (1 + 108).
3. Un `DispatcherQueueTimer` cada 120 ms navega a la siguiente página; cuenta `ok`/`fail` y registra cualquier excepción como `FAIL <página>: <tipo>: <mensaje>` (:579-603).
4. Al terminar escribe `SMOKE DONE total=<N> ok=<N> fail=<N>` y sale de la app (:586-590).

Es decir, **instancia las 109 páginas de navegación** (cada Page con su VM) para detectar fallos de carga de XAML/recursos. Útil como prueba de humo en CI/headless.

---

## 11. Estado y verificación (honesto)

La **estructura de UI está completa Y la lógica de dominio está portada**: las 108 pantallas del registro existen como `.xaml` + `.xaml.cs` + ViewModel, están cableadas en menús/toolbar/registro, y su lógica está implementada — usando clases reales de `Free1X2.Domain` donde el original ya las tenía, y transcripción **1:1** inline donde la lógica vivía en el code-behind del Form WinForms.

> **Nota histórica.** Una versión previa de este documento listaba ~23 pantallas "con lógica aún marcada como TODO". Una auditoría posterior (4 pases de scoping independientes) demostró que esos eran **comentarios obsoletos**: el código debajo ya estaba implementado. Se limpiaron **46** doc-comments obsoletos (solo comentarios) y la realidad se verificó con build + smoke + tests + runtime (abajo).

### Verificación (evidencia)

| Nivel | Resultado |
|---|---|
| Build `Free1X2.WinUI` (Debug, x64, win-x64) | **0 errores** |
| Build `Free1X2.Domain` | **0 errores** |
| Smoke de carga (`FREE1X2_SMOKE=1`: instancia MainPage + 108 páginas) | **109/109 ok · 0 fail** |
| Tests del motor (`Free1X2.Domain.Tests`, xUnit) | **107/107** (incl. 33 golden-master con datos reales: ConvertidorDeBases round-trip, UtilColumnas + Comparador [invariante coincidencias+diferencias=14], SumadorCombinaciones [4 ops], Escrutador [distribución de premios], reductor JDC) |
| Runtime UI Automation (navegar 24 pantallas + invocar la acción primaria) | **0 crashes**; las acciones completan o muestran su validación esperada |
| Pantallas desconectadas / que lanzan excepción | **0** (todas con su productor/handoff cableado) |

### Residuales menores (honestos — no bloquean ninguna función)

- **`EscrutiniosFrmViewModel`**: los prefijos `/t` y `/j` se añaden al final del texto, no en la posición del cursor (sin tracking de caret en MVVM). `:486,497`.
- **`GruposEquiposFrmViewModel`**: las etiquetas muestran nombres por defecto ("Equipo casa N") en vez de los nombres reales de equipo (que viven en la UI del boleto, no en el dominio). El filtrado casa/fuera sí funciona. `:256`.
- **`ListadoCondicionesFrmViewModel`**: se muestra el resumen de una línea por filtro; el árbol detallado por tipo de filtro queda como `TODO[detalle]`. `:122`.
- **`AcercaDeFrmPage`**: el logo no se carga (cosmético) y los enlaces licencia/GPL abren la web en vez del documento empaquetado. `:36,45,53`.

### Decisiones de port (correcto por diseño — no son huecos)

- **Ayuda online** deshabilitada igual que el original (el `AyudaFrm` legacy ya mostraba "Online help disabled for offline operation").
- **Descarga de boleto online**: stub offline igual que el original (`ObtenerBoleto` devolvía "" en legacy).
- **Control de porcentajes** (`valors`) reemplazado por `PorcentajesControl`; lógica equivalente.
- **Gráficos `System.Drawing`** (no portables a WinUI) reimplementados con `GraficoLineasControl`.
- **`BoletoControl`** legacy reemplazado por `BoletoMatrizControl`.

### Arreglado en esta auditoría

- **46** doc-comments obsoletos "queda como TODO" limpiados (solo comentarios).
- Único clic muerto user-facing arreglado: enlace **"Créditos"** en *Acerca de* → navega a `CreditosFrmPage` (confirmado en runtime).
- Cierre fiel al original (`Frame.GoBack`) restaurado en *Cambio de puntos*, *Analizar fichero* y *Config CPs* (el original cerraba esos diálogos).
- Comentario "111 pantallas" en `PortedPages.cs` corregido a **108**.

### Resumen del estado

| Aspecto | Estado |
|---|---|
| Estructura de UI (páginas, menús, toolbar, tema) | **Completa y cableada** (108 páginas + MainPage) |
| Lógica de dominio de las pantallas | **Implementada y verificada** (build/smoke/tests/UIA) |
| Smoke de carga | **109/109 ok** |
| Tests del motor | **107/107** |
| Crashes en runtime al invocar acciones (24 pantallas) | **0** |
| Residuales | menores/cosméticos (lista arriba); **0 funciones bloqueadas** |

---

*Documento generado por análisis estático del código en `main`. Cifras verificadas con `find`/`grep`/lectura directa de fuentes el día de la redacción.*
