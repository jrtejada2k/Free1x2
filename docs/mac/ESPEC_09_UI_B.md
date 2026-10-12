# ESPEC 09 — UI B: pantallas 41–73 (por orden alfabético de ViewModel)

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Convenciones en
> [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Describe **qué hace hoy** el código WinUI 3
> para reescribirlo en SwiftUI sin mirar el C#. Las asignaciones de «área Mac» y de tipo de
> presentación (vista principal / hoja modal / inspector) son **propuestas** (R2): las decide el dueño.
> Continúa [`ESPEC_08_UI_A.md`](ESPEC_08_UI_A.md) (pantallas 1–40); las pantallas 74–108 y el shell están en
> [`ESPEC_10_UI_C.md`](ESPEC_10_UI_C.md).

**Alcance.** Las 33 pantallas de `cd Free1X2.WinUI/Views/Ported && ls *ViewModel.cs | sed -n '41,73p'`, de `DifCols` (41) a
`ModificadorFrm` (73). Cada una se documenta con: propósito y apertura · entradas (controles, tipos, rangos, defectos, validaciones y
mensajes literales) · acciones, llamadas al motor y ficheros · **lógica de negocio que vive en el ViewModel** (pseudocódigo →
`Free1X2Core/CasosDeUso`) · salidas y formatos · peculiaridades a preservar · casos de uso `CU-Bnn` (numeración única
`CU-B01`…`CU-B41`, catálogo en §2.3) · área Mac propuesta. Al final: tabla resumen (§2) y «Abiertas» (§3).

**Cómo leer el pseudocódigo.** Igual orden de iteración y de efectos que el C# (T5/T6); `/` entre enteros es división entera (T9);
`Round` = redondeo bancario salvo que se diga lo contrario (T4); donde el C# usa `Convert.ToInt32` sobre `double` se indica (redondea
a par). «Preservar» marca peculiaridades que afectan a la salida (R1); las que parecen defectos se recogen en «Abiertas» (§3).
Los índices son 0-based salvo que se indique lo contrario. Cuando el port WinUI y el WinForms legacy difieren, **se documenta el port**
(es lo que hoy se ejecuta y donde vive la lógica) y se cita el legacy; la elección del oráculo es una decisión abierta (G-1).

---

## 0. Contexto compartido por estas pantallas

### 0.1 Convenciones de referencias y de identificadores

- Cada sección de pantalla define en **Ficheros** tres abreviaturas: `VM` (ViewModel), `XAML` (página) y `CS` (code-behind). Los tres están en
  `Free1X2.WinUI/Views/Ported/`. Una cita `VM:a-b` son líneas 1-based de ese fichero.
- Una cita con **solo nombre de fichero** (`MainWindow.xaml.cs:257`, `MainPageViewModel.cs:291-296`, `LAE.cs:50`) se resuelve por nombre, que es único en el repo:

| Nombre / prefijo | Ruta real |
|---|---|
| `*ViewModel.cs`, `*Page.xaml`, `*Page.xaml.cs`, `_Helpers/…` | `Free1X2.WinUI/Views/Ported/` |
| `MainPageViewModel.cs` (`Views/…`) | `Free1X2.WinUI/Views/` |
| `MainWindow.xaml.cs`, `Navigation/PortedPages.cs`, `Services/…`, `Controls/…` | `Free1X2.WinUI/` |
| `Domain/…` o solo nombre (`FiltroDistancias.cs`, `LAE.cs`, `Escrutador.cs`…) | `Free1X2.Domain/` (`MotorCalculo/`, `Utils/`, `Escrutinio/`, `EntradaSalida/`, `Analisis/`, `Online/`) |
| `Free1X2/UI/…` | WinForms legacy (referencia de comportamiento, congelado en 0.77.2) |
| `docs/…` | documentación del repo |

- Identificadores: `A-NN-k` = pregunta abierta nº k de la pantalla NN (§3); `G-n` = abierta global; `CU-Bnn` = caso de uso extraído (§2.3).
- Las pantallas se numeran por el orden alfabético de ViewModel, como en ESPEC_08/10. Las referencias «pantalla NN» de este documento son a ese orden.

### 0.2 Patrón de pantalla de condición (PC) en las pantallas de este alcance

El comportamiento común de las condiciones (Aceptar / Cancelar / Borrar / Estadísticas / Guardar / Abrir / Copiar / Pegar, apertura con
`AppState.GrupoEnEdicion`, filtro temporal para Estadísticas) está descrito en **ESPEC_08 §1.6**. Aquí solo se recogen los datos concretos de las
seis condiciones de este alcance (42, 43, 51, 52, 62, 69), más If-Then (65), que no edita un grupo sino el `Analizador`:

| Pantalla | Extensiones (guardar / abrir) y nombre sugerido | Temporal | Efecto de Aceptar sobre el filtro | Abrir | Borrar |
|---|---|---|---|---|---|
| 42 Diferencias | `.dif`, `.xml` · `Diferencias` (`VM:70-77`) | `Temp/tmp.rep` | `IsActive = ContieneDatos` (`VM:428-440`) | toma `Diferencias`, `ContieneDatos`, `IsActive` del leído y los escribe en el filtro vivo (`VM:478-512`) | vacía `Diferencias` del filtro vivo (`VM:517-526`) |
| 43 Distancias | `.dist`, `.xml` · `Distancias` (`VM:57-64`) | `Temp/tmp.dist` | `ReinicializaValores`; campo vacío ⇒ 0..14 (`VM:94-129`) | solo vuelca a pantalla (`VM:201-210`) | vacía los 4 campos (`VM:139-146`) |
| 51 Formatos 123 | `.123`, `.xml` · `Formatos123` (`VM:116-123`) | `Temp/tmp.123` | `IsActive = NecesitaGuardar()` (`VM:334-338`) | solo vuelca a pantalla (`VM:444-453`) | reinicia filas y aciertos; no toca el filtro (`VM:470-481`) |
| 52 Formatos | `.fmt`, `.xml` · `Formatos` (`VM:89-96`) | `Temp/tmp.fmt` | activa si hay ≥1 relación (`VM:223-233`) | copia al filtro vivo y recarga (`VM:355-370`) | vacía el filtro vivo (`VM:389-401`) |
| 62 Grupos de equipos | `.geq`, `.xml` · `Grupos de equipos` (`VM:1040,1062`); no usa la base | `Temp/tmp.geq` (`VM:123-124`) | `GuardarDatos` + relaciones + `ActivaFiltro` (`VM:823-842`) | reasigna `_grupo`/`_filtroGE` al leído (`VM:896-909`; A-62-1) | `_filtroGE` nuevo desligado hasta Aceptar (`VM:845-862`) |
| 69 Interrupciones | `.int`, `.xml` · `Interrupciones` (`VM:88-95`) | `Temp/tmp.int` | `ReinicializaValores`; campo vacío ⇒ 0..14 (`VM:137-177`) | solo vuelca a pantalla (`VM:257-266`) | vacía los 10 campos (`VM:179-193`) |
| 65 If-Then | `.if`, `.xml` · `Condiciones` (`VM:533,551`) | `Temp/tmp.if` (`VM:66-67`) | `analizador.IfThen = ifThen` (`VM:515-524`) | `Limpiar` + carga en pantalla (`VM:557-574`) | `Limpiar` (`VM:608-625`) |

En esta tabla `VM:n` es el ViewModel de la pantalla de esa fila.

Observaciones transversales: (a) las seis condiciones recargan desde el grupo en `OnNavigatedTo` salvo al volver de Estadísticas
(`NavigationMode.Back`), para no perder lo editado (`DiferenciasFrmPage.xaml.cs:33`, `DistanciasFrmPage.xaml.cs:31`, `Formatos123FrmPage.xaml.cs:30`, `FormatosFrmPage.xaml.cs:35`, `GruposEquiposFrmPage.xaml.cs:34`, `InterrupcionesFrmPage.xaml.cs:31`); en Mac, si Estadísticas es una hoja sobre la
condición, este caso desaparece. (b) El «temporal» de Copiar/Pegar de Diferencias es `tmp.rep` aunque su extensión es `.dif`
(`DiferenciasFrmViewModel.cs:73,77`; `FiltroArchivoViewModelBase.cs:44-49`). (c) Escrituras en el **filtro vivo** del grupo antes de pulsar Aceptar, que Cancelar no revierte: 42 al navegar entre conjuntos y al Abrir/Borrar
(`DiferenciasFrmViewModel.cs:345-368,478-526`); 52 al Abrir y Borrar (`FormatosFrmViewModel.cs:355-370,389-401`); 62 en Guardar, Abrir, Copiar, Pegar y Borrar,
que ejecutan antes `GuardarDatos()` (`GruposEquiposFrmViewModel.cs:852,1038,1060,1081,1100`).

### 0.3 Base común `FiltroArchivoViewModelBase` (pantallas 42, 43, 51, 52, 69)

`_Helpers/FiltroArchivoViewModelBase.cs:30-115`. Miembros específicos que cada VM aporta: `NombreSugerido`, `TiposGuardar` (pares etiqueta/extensión; el primero
fija la extensión por defecto), `ExtensionesAbrir`, `RutaTemporal`, `GuardarEn(ruta)`, `AbrirDesde(ruta)` (`:33-58`). Comandos heredados:

| Comando | Comportamiento | Evidencia |
|---|---|---|
| Guardar | `PickerHelper.GuardarAsync(NombreSugerido, TiposGuardar)`; cancelar ⇒ nada; `GuardarEn(ruta)`; error ⇒ `MostrarError("No se pudo guardar: " + msg)` | `:60-74` |
| Abrir | `PickerHelper.AbrirAsync(ExtensionesAbrir)`; `AbrirDesde(ruta)`; error ⇒ `"No se pudo abrir: " + msg` | `:76-90` |
| Copiar | crea `Temp/` si no existe y `GuardarEn(RutaTemporal)`; error ⇒ `"No se pudo copiar: " + msg` | `:92-105` |
| Pegar | si existe `RutaTemporal` ⇒ `AbrirDesde(RutaTemporal)`; **sin `try/catch`** (un fichero corrupto propaga la excepción) | `:107-114` |

### 0.4 Handoffs estáticos entre pantallas que intervienen en este alcance

(Complementa ESPEC_08 §1.3. Propuesta Swift: valores asociados en un `enum Destino` / `@State` de hoja en vez de estáticos «consume y limpia».)

| Dato | Productor → consumidor | Mecanismo | Evidencia |
|---|---|---|---|
| Grupo en edición | Inicio → condiciones 42, 43, 51, 52, 62, 69 | `AppState.GrupoEnEdicion` (estático) | `AppState.cs:73`, `MainPageViewModel.cs:291-296,299-304` |
| Estadísticas | 42, 43, 51, 52, 62, 69 → `VisorEstadisticasPage` | `VisorEstadisticasViewModel.UltimasEstadisticas` | `VisorEstadisticasViewModel.cs:49` |
| Formatos predefinidos | 51 → `AnalisisFormatos123FrmPage` | `AnalisisFormatos123FrmPage.FormatosEntrada` | `AnalisisFormatos123FrmPage.xaml.cs:33` |
| Figuras en edición | Contactos / Signos seguidos → 49 | `FigurasFiltrosFrmViewModel.FigurasEnEdicion` | `FigurasFiltrosFrmViewModel.cs:57` |
| CPs importadas | 67 → editor de CPs (pantalla 19) | `ImportadorCPsFrmViewModel.Resultado` | `ImportadorCPsFrmViewModel.cs:39`, `ColProbablesFrmViewModel.cs:746` |
| CPs a exportar | editor de CPs → 48 | `ExportadorCPsFrmViewModel.ListaParaExportar` | `ExportadorCPsFrmViewModel.cs:37` |
| Matriz de valoración | Valoración → 63 | `GuardarValoracionFrmPage.ValoresIniciales` | `GuardarValoracionFrmPage.xaml.cs:23` |
| Contexto del histórico | Tramificar → 64 | `HistoriaValoracionesFrmPage.Contexto` | `HistoriaValoracionesFrmPage.xaml.cs:19` |
| Impresora elegida | 70 → 68 | `ListaImpresorasViewModel.SeleccionResultado` | `ListaImpresorasViewModel.cs:174` |
| Ganadora y columnas jugadas | Posibles premios → 72 | parámetro de navegación `MejoresOpcionesContexto` | `MejoresOpcionesFrmViewModel.cs:21-24` |

### 0.5 Servicios y componentes de presentación reutilizados

Servicios (`PickerHelper`, `AppServices.MostrarError/MostrarInfo/ConfirmarAsync`) y patrón de tareas largas: ver **ESPEC_08 §1.2**. Particularidades de este alcance:

- Los diálogos de fichero abren en Documentos (`PickerHelper.cs:32,45,58,76`; las pantallas 44 y 45 construyen su propio selector, también en Documentos: `EscrutarCombinacionesFrmViewModel.cs:301`, `EscrutiniosFrmViewModel.cs:437`), salvo «Exportar PNG» de la pantalla 61, que abre en Imágenes (`GraficoColumnasFrmPage.xaml.cs:107`); el WinForms usaba carpetas relativas al ejecutable (p. ej. `InitialDirectory = "Condiciones\\"`,
  `Free1X2/UI/Filtros/InterrupcionesFrm.cs:744`). Ver G-2.
- `PorcentajesControl` (rejilla 14×3 de `NumberBox`, `Minimum=0`; detalle en ESPEC_08 §1.5) lo usan las pantallas 46 (`EstimadorPremiosFrmPage.xaml:331`), 50 (`FiltroPorcenJBPage.xaml:161`),
  51 (`Formatos123FrmPage.xaml:137`), 63 (`GuardarValoracionFrmPage.xaml:27`) y 64 (`HistoriaValoracionesFrmPage.xaml:75`). Sus helpers `Crear`/`AMatriz`/`CargarMatriz` están en `Controls/PorcentajesHelper.cs:15-41`.
- Tareas largas con `await Task.Run(...)` y botón Cancelar/bandera: 41 (`DifColsViewModel.cs:171,215`), 47 (`EstucolFrmViewModel.cs:146,149`), 57 (`GeneraPimViewModel.cs:171`)
  y otras citadas en su sección. En Swift: `async throws` + `Task.checkCancellation()` + progreso por `AsyncStream` (T12).

### 0.6 Leyenda de áreas Mac (PLAN §4)

**Boleto · Condiciones y filtros · Cálculo · Reducción · Escrutinio · Análisis · Columnas probables · Herramientas · Online · Ajustes.** En este alcance aparecen
siete: Columnas probables, Condiciones y filtros, Escrutinio, Herramientas, Reducción, Boleto y Análisis. «Presentación» = vista principal, hoja modal o inspector.

---

## 1. Pantallas (orden alfabético de ViewModel)

### Pantalla 41 · DifCols — «Diferencias entre columnas»

**Ficheros.** `VM` = `Free1X2.WinUI/Views/Ported/DifColsViewModel.cs` (379 lín.), `XAML` = `DifColsPage.xaml` (249), `CS` = `DifColsPage.xaml.cs` (23, sin lógica: solo `new DifColsViewModel()`, `CS:17`). Legacy: `Free1X2/UI/DifCols.cs`.

**Propósito.** Genera columnas (las 4 782 969 de 14 triples o las leídas de un fichero) y acepta cada una solo si su nº de diferencias respecto a una columna base / fichero de condiciones y respecto a las ya aceptadas cae en rangos dados (`VM:14-21`, `XAML:25-27`). Sirve sobre todo para fabricar CPs (`docs/MANUAL_USUARIO.md:302`, `:347`).

**Cómo se abre.** Menú Utilidades «Diferencias entre columnas…» (`MainWindow.xaml.cs:257`) y botón `bDiferenciasColumnas` de la barra Utilidades (`MainWindow.xaml.cs:420`). Sin parámetros ni `GrupoEnEdicion`: pantalla autónoma (no toca `AppState`).

**Entradas.**

| Control | Tipo / defecto | Evidencia |
|---|---|---|
| «Generación por»: radio «14 triples» / «Fichero» | `bool GeneraTriples=true`, `GeneraFichero=false` | `VM:33-38`, `XAML:38-55` |
| «Condicionada por»: radio «Columna Base» / «Fichero» | `CondicionColumnaBase=true`, `CondicionFichero=false` | `VM:47-52`, `XAML:79-113` |
| Columna base | texto, `MaxLength 14`, mayúsculas, defecto `1X1212X2111121` | `VM:55-56`, `XAML:86-92` |
| Diferencias s/ col. base | texto `MaxLength 20`, formato `min(-max)` separado por comas, defecto `0-5,6,7-14` | `VM:64-65`, `XAML:131-136` |
| Diferencias s/ col. result. | idem, defecto `6-14` | `VM:69-70`, `XAML:154-159` |
| Límite de columnas | `NumberBox` Min 0, defecto 0 (0 = sin límite → 4 782 969) | `VM:76-77`, `XAML:182-188`, `VM:208` |

Alfabeto válido de columna: `1 2 4 x X F` (`VM:264`); `x`/`X` se normalizan a `4`; `F` = «fijo»: ese partido no cuenta en las comparaciones (`VM:260-274`). Longitud exacta 14 (`VM:266`).

**Acciones.**

1. **Calcular** (`VM:104-194`):
   1. Construye `condis`: si «Columna Base» → `VerColumna(ColumnaBase)`; vacío ⇒ error «columna errónea» y aborta (`VM:108-117`). Si «Fichero» → abre `.txt` (diálogo Abrir, DocumentsLibrary, `PickerHelper.AbrirAsync(".txt")`, `VM:375-378`), lee todas las líneas, ignora las vacías, cada una por `VerColumna`; una errónea ⇒ «columna errónea» y aborta; I/O ⇒ `ex.Message` (`VM:118-134`). Muestra el nombre en `NombreFicheroCondiciones`.
   2. Si «Fichero» de generación → abre otro `.txt` (cancelar = salir sin hacer nada) (`VM:137-144`).
   3. Construye las máscaras `difbase`/`difresul` con `PreparaDif`; excepción ⇒ «Rango de diferencias inválido: <msg>» (`VM:146-157`).
   4. Deshabilita Calcular/Grabar, habilita Cancelar, `AdmitidasTexto="calculando..."` (`VM:163-167`); ejecuta `Procesar` en hilo de fondo.
   5. Al terminar: `AdmitidasTexto=count`, `PuedeGrabar = count>0`; si cancela, repone la cuenta de la lista **anterior** (`VM:171-181`). Error ⇒ «Error en el cálculo: <msg>» (`VM:182-185`). `TiempoTexto = (ahora−inicio).ToString().Substring(0,10)` (≈ `hh:mm:ss.f`, `VM:188`).
2. **Grabar** (`VM:200-236`): diálogo Guardar, nombre sugerido `Diferencias`, tipo «F.Salida» `.txt`, DocumentsLibrary (`VM:204`). Escribe cada aceptada con `4→X`, una por línea, hasta `limite` (`VM:208-224`); mensaje «Columnas grabadas.» / «Error al grabar: <msg>».
3. **Cancelar** (`VM:242-245`): `CancellationTokenSource.Cancel()`.
4. **«CPs por diferencias…»** (`VM:252-256`): el comando está **vacío** (no navega). Legacy abría `GeneradorCPSDiferencias` (pantalla 58).

**Lógica de negocio en el ViewModel** (toda; no hay llamadas a `Free1X2.Domain`):

```
VerColumna(s) -> (col, mask) | error:
  si len(s)!=14 -> error;  para cada char c: si c no en "124xXF" -> error
  mask[i] = 'F' si c=='F' sino 'V';  col = s con x,X -> '4'            (VM:262-274)

PreparaDif(texto) -> string de 15 chars ('A' admitido, 'F' fuera):        (VM:277-296)
  dif = "FFFFFFFFFFFFFFF"
  para cada trozo en texto.split(','):
     p = trozo.split('-');  a = Int(p[0]) acotado a [0,14]
     b = (p.count==2) ? Int(p[1]) acotado a [0,14] : a      // p.count>2 => b=a
     dif = dif[0..<a] + 'A'*(b-a+1 si b>=a, si no 0) + dif[b+1..<15]   // si a>b la cadena crece (>15)
  (Int(...) lanza FormatException con vacío/no numérico)

Compara(x, y, mask) -> nd:                                                (VM:301-310)
  nd = #{ i : mask[i]!='F'  &&  (x[i] & y[i]) == 0x30 }
  // equivale a x[i]!=y[i] siendo ambos de {'1','2','4'}; cualquier 'F' (0x46) nunca cuenta

Proceso(col, mask):                                                       (VM:319-334)
  para cada c en condis:    nd=Compara(col,c,mask);  si difbase[nd]=='F'  -> descartar
  para cada c en aceptadas: nd=Compara(col,c,mask);  si difresul[nd]=='F' -> descartar
  aceptadas.append( col con 'F' en las posiciones mask=='F' )            // sin deduplicar más allá de difresul

Procesar:
  si GeneraTriples: recorrer 14 niveles con valores '1','2','4' (orden lexicográfico 1<2<4,
     el primer partido es el bucle externo), mask = "VVVVVVVVVVVVVV"       (VM:338-358)
  si no, si hay fichero: por cada línea no vacía -> VerColumna (error => excepción «columna errónea»,
     abortando todo) -> Proceso(col, maskDeEsaLinea)                       (VM:359-370)
  comprobar cancelación en cada candidata (VM:345,364)
```

**Salidas.** Contador «Admitidas» (entero sin formato), «Tiempo» (`hh:mm:ss.f`), fichero `.txt` de columnas 14 caracteres `1/2/X/F` (las `F` se escriben literalmente si hubo máscara) (`VM:221`).

**Peculiaridades a preservar / avisos.**
- Coste cuadrático: 4 782 969 candidatas × |aceptadas| × 14 posiciones (`VM:319-334`). En Swift: representar la columna como 14 códigos de 2 bits y comparar por XOR/popcount, o `[UInt8]` contiguos; el resultado debe ser idéntico.
- `PreparaDif` con `min>max` (p. ej. `7-3`) produce una cadena de >15 caracteres desplazada (`VM:290-292`): replicar tal cual (T9: sin «arreglar»).
- `TiempoTexto`: `Substring(0,10)` lanza si el `TimeSpan` no tiene fracción (`VM:188`); en Swift formatear siempre `hh:mm:ss.f`.
- **Divergencia con el legacy** (ver «Abiertas» A-41-1/2).

**Casos de uso extraídos (CU-B01).** `ColumnasPorDiferencias.generar(origen:, condiciones:, difBase:, difResultado:)` (+ `parseRangosDiferencias`, `compararColumnasConFijos`) y `ColumnasPorDiferencias.exportarTxt(limite:)`.

**Área Mac propuesta.** *Columnas probables* (herramienta de generación). Hoja/ventana de herramienta con barra de progreso y botón Cancelar; resultado en tabla previa + «Exportar…». Propuesta; decide el dueño.

### Pantalla 42 · DiferenciasFrm — Condición «Diferencias»

**Ficheros.** `VM` = `DiferenciasFrmViewModel.cs` (542), `XAML` = `DiferenciasFrmPage.xaml` (231), `CS` = `DiferenciasFrmPage.xaml.cs` (38). Base compartida: `_Helpers/FiltroArchivoViewModelBase.cs` (ver §0.3). Legacy: `Free1X2/UI/Filtros/DiferenciasFrm.cs` (citado en `VM:107,128,166,213,309`).

**Propósito.** Condición «Diferencias»: el usuario define grupos de partidos y, por concepto (Variantes, Equis, Doses, Dibujos, Interrupciones, Formatos), cuántos valores **distintos** puede haber entre los grupos (`VM:44-55`, `XAML:14`). Cada «conjunto» es un `Diferencia` del `FiltroDiferencias` del grupo; se navega entre conjuntos.

**Cómo se abre.** Rejilla de condiciones de Inicio, botón «Diferencias» (`Views/MainPageViewModel.cs:173`, abre con `AppState.GrupoEnEdicion = grupo actual`, `MainPageViewModel.cs:291-296`). Al entrar (no al volver de Estadísticas) llama `CargarDesdeGrupo()` (`CS:28-37`). `Aceptar`/`Cancelar` hacen `Frame.GoBack()` (`CS:24`).

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| 20 líneas «Grupos de partidos» (crece si el conjunto cargado tiene más) | texto libre; partidos separados por `,` o rangos con `-` (`p. ej. 1,2-5`) | `VM:58,86-89,134-137`, `XAML:56-73` |
| Variantes / Equis / Doses / Dibujos / Interrupciones / Formatos | texto: cantidad o intervalo de valores distintos (`2`, `1-3`, `1,3`), vacío = no se analiza | `VM:241-262`, `XAML:90-165` |
| Atajo «Agrupación» | `Dúos, Tríos, Cuartetos, Quintetos, Sextetos, Septetos, Octetos` (defecto Dúos) | `VM:90-95,311-321` |
| «Paso fijo» | toggle, defecto `true` | `VM:302` |
| Navegación de conjuntos ◀ ▶ «Eliminar actual», contador `actual/guardados` | `ConjuntoActual` 1-based | `VM:267-289`, `XAML:20-41` |

**Acciones** (todas sobre `AppState.GrupoEnEdicion` → `FiltroDiferencias`, `VM:98-103`):
- **Aceptar** (`VM:428-440`): `ActualizarDatos`; `filtro.IsActive = filtro.ContieneDatos`; `grupo.ActivaFiltro(filtro)`; `AppState.NotificarCambio()` (refresca semáforos de Inicio); volver. Sin grupo ⇒ solo volver.
- **Cancelar** (`VM:443-447`): solo vuelve. **No revierte** lo ya escrito en el filtro por ◀/▶ (esas navegaciones escriben en el objeto vivo, `VM:345-368`).
- **Estadísticas** (`VM:450-463`): `ActualizarDatos`; `new CalculadorEstadisticas().EstadisticasFiltro(filtro, <exe>/Ganadoras/)` → `VisorEstadisticasViewModel.UltimasEstadisticas` → navega a `VisorEstadisticasPage` (pantalla fuera de este alcance).
- **Guardar/Abrir/Copiar/Pegar** (base §0.3): tipos `Diferencias .dif` y `Diferencias (XML) .xml`; Abrir acepta `.dif`,`.xml`; temporal `<exe>/Temp/tmp.rep` (`VM:70-77`). Guardar: `ActualizarDatos` + `ArchivoCondiciones{NombreArchivo}.GuardaArchivo(filtro)` (`VM:466-474`). Abrir: `ArchivoCondiciones.AbrirArchivoCombinacion` → `LeeCondicion()` → toma `Diferencias` y `ContieneDatos`/`IsActive` del leído (solo si `ContieneDatos`), `grupo.ActivaFiltro`, `_indice=0`, vuelca (`VM:478-512`).
- **Borrar** (`VM:517-526`): `Diferencias.Clear()`, `_indice=0`, limpia pantalla, contador 1/0. No toca `IsActive`.
- **Generar atajos** (`VM:307-335`): `cantidad` = 2..8 según atajo; `cadenas = UtilidadesEntradasValores.ObtenerGruposDeValores(cantidad, PasoFijo)`; rellena **solo las líneas vacías** en orden con cada cadena (sobrante se ignora).

**Lógica de negocio en el ViewModel.**

```
CreaDiferencia() -> Diferencia?   (try/catch: cualquier excepción => nil, sin mensaje)   (VM:167-211)
  rep = Diferencia()
  para cada línea con texto no vacío (IsNullOrWhiteSpace):
      rep.AñadirPartidosSimetricos( ObtenerBoolArrayFromTxt(texto, NumeroPartidos+1) )
  n = rep.PartidosSimetricos.count + 1
  AcV, AcX, AcDoses, AcDib, AcInt, AcFormatos = ObtenerBoolArrayFromTxt(campo, n)   // nil si campo vacío
  si PartidosSimetricos.count == 0 -> return nil
  AnalizaVX2Dib = !(AcV==nil && AcX==nil && AcDoses==nil && AcDib==nil)
  cada Ac* nil -> ActivarTodos(n)   // bool[n]: índice 0=false, 1..n-1=true
  AcInt==nil  -> ActivarTodos(n), AnalizaInterrupciones=false ; si no true
  AcFormatos==nil -> ActivarTodos(n), AnalizaFormatos=false ; si no true

ActualizarDatos(filtro):  s = CreaDiferencia()
  si filtro.Diferencias.count > indice: si s!=nil  filtro.Diferencias[indice]=s     (si nil NO borra)
  si no:                                si s!=nil  filtro.Diferencias.append(s)      (VM:214-226)

MarcarValores(rep): LimpiarPantalla; asegura líneas >= rep.PartidosSimetricos.count;
  línea[i] = ObtenerTextoFromBool(PartidosSimetricos[i])   // "i,j,k" (índices true, coma)
  si AnalizaVX2Dib: Variantes/Equis/Doses/Dibujos = TextoFromBool(AcV/AcX/AcDoses/AcDib)
  si AnalizaInterrupciones: Interrupciones;  si AnalizaFormatos: Formatos             (VM:129-157)

Siguiente (VM:338-370):                                    Anterior (VM:373-396):
  si count>indice: s=Crea(); si s!=nil filtro[indice]=s;     si indice<=0 -> return
     indice++; si count>indice mostrar[indice] sino limpiar   si count>indice: s=Crea();
  si no: s=Crea(); si s!=nil {append; indice++; limpiar}         si s!=nil {filtro[indice]=s; indice--}
  contador: actual=indice+1, guardados=count                      mostrar[indice]   // si s==nil no retrocede y repinta el actual
                                                                si no (conjunto nuevo sin guardar): indice--; mostrar[indice]
Eliminar (VM:399-425): si count>indice {RemoveAt(indice); si indice>0 indice--} sino limpiar;
  indice=0; si ContieneDatos && count>0 mostrar[0] sino limpiar; refrescar contador.
```

`ObtenerBoolArrayFromTxt(txt, n)` (`Domain/Utils/UtilidadesEntradasValores.cs:255-280`): parsea lista de enteros con `,` y `-` (`ObtenerListaFromTxtAciertos`, `:115-181`, errores ⇒ lista vacía ⇒ excepción por índice ⇒ `CreaDiferencia` devuelve nil), ordena y devuelve `bool[]` de tamaño `max(n, mayor)+1`, o `nil` si el texto es vacío. `ObtenerGruposDeValores` (`:404-424`): para i=1..N, ventana de `cantidad` partidos consecutivos escrita `"a-b"`; con paso fijo salta `i=j`.

**Salidas.** Contador `«actual/guardados»` (`VM:275`); líneas/conceptos como texto de listas separadas por coma; sin formato numérico especial.

**Peculiaridades a preservar.**
- Texto inválido en una línea o concepto ⇒ el conjunto entero no se guarda **sin avisar** (`VM:207-210`); «Aceptar» con pantalla vacía conserva el conjunto anterior (`VM:216-220`).
- `ConjuntoAnterior` no retrocede si el conjunto en pantalla no es válido (`VM:383-389`).
- `Pegar` usa `AbrirDesde` sin `try/catch` (`FiltroArchivoViewModelBase.cs:107-114`).

**Casos de uso extraídos (CU-B02).** `CondicionDiferencias.construirDesdeTexto`, `.volcarATexto`, `.navegarConjuntos`, `.generarGruposAtajo`; `CondicionDiferencias.estadisticas` (reutiliza `CalculadorEstadisticas`).

**Área Mac propuesta.** *Condiciones y filtros* → **inspector/hoja modal** sobre el grupo en edición (se abre desde la rejilla de condiciones). Propuesta; decide el dueño.

### Pantalla 43 · DistanciasFrm — Condición «Distancias»

**Ficheros.** `VM` = `DistanciasFrmViewModel.cs` (226), `XAML` = `DistanciasFrmPage.xaml` (123), `CS` = `DistanciasFrmPage.xaml.cs` (36). Patrón de condición: §0.2–0.3. Legacy: `Free1X2/UI/Filtros/DistanciasFrm.cs` (citado en los comentarios del VM: `VM:97,150,187,200,217`).

**Propósito.** Limita la «distancia» (nº máximo de partidos que separan dos signos iguales, `VM:21`) para cualquier signo (Var), `1`, `X` y `2` (`VM:22-24`, `docs/MANUAL_USUARIO.md:234`).

**Cómo se abre.** Rejilla de Inicio, botón «Distancias» (`MainPageViewModel.cs:170`) con `GrupoEnEdicion`. `OnNavigatedTo` carga el filtro salvo al volver de Estadísticas (`CS:26-35`).

**Entradas.** Cuatro textos libres «Var», «1», «X», «2» (`VM:29-42`, `XAML:25-81`), defecto vacío. Rango declarado «0–15» (`XAML:20`); el parser real (`FiltroDistancias.SetNoInt1/X/2/Var`, `Domain/MotorCalculo/FiltroDistancias.cs:493-620`) espera **valores enteros separados por coma**, cada uno con **sufijo opcional de una letra** = marca de tolerancia (`"3"`, `"4a"`); no admite rangos `a-b` ni espacios (ver A-43-1). Índice fuera del array ⇒ excepción.

**Acciones.**
- **Aceptar** (`VM:94-129`): `filtro = grupo.GetFiltro("Distancias")`; `ReinicializaValores()`; si `ContieneDatos` (algún campo no blanco, `VM:71-75`): si el filtro no tenía datos ⇒ `IsActive=true`; `ContieneDatos=true`; para cada uno de los 4 juegos, si está en blanco se rellena con `ObtenerTodosValores()` (= `0,1,…,NumeroPartidos`, `UtilidadesEntradasValores.cs:322-330`). Si no hay datos ⇒ `IsActive=false`, `ContieneDatos=false`. Después `grupo.ActivaFiltro`, `NotificarCambio()`, volver. Sin `try/catch`: un texto inválido propaga la excepción tras haber hecho `ReinicializaValores()` (el filtro queda a medio actualizar).
- **Cancelar** (`VM:132-136`), **Borrar** (vacía los 4 campos, `VM:139-146`), **Estadísticas** (`VM:215-225`: filtro temporal → `CalculadorEstadisticas.EstadisticasFiltro(filtro, <exe>/Ganadoras/)` → `VisorEstadisticasPage`).
- **Guardar/Abrir/Copiar/Pegar**: tipos `Distancias .dist` y `Distancias (XML) .xml`, temporal `Temp/tmp.dist` (`VM:57-64`). Guardar: filtro temporal; `si NoDistancias>0 { ContieneDatos=true; IsActive=true }`; `ArchivoCondiciones.GuardaArchivo` (`VM:188-198`). Abrir: **solo vuelca a pantalla** (no toca el grupo hasta Aceptar) (`VM:201-210`).

**Lógica de negocio en el ViewModel.** Solo la normalización de Aceptar/Temporal descrita arriba («campo vacío ⇒ todos los valores»; «ningún campo ⇒ condición desactivada y vacía»). `ObtenerFiltroTemporal` (`VM:152-176`) repite la misma lógica sobre un `FiltroDistancias` nuevo.

**Salidas.** Cuatro textos `v[letra],v[letra]…` ordenados ascendente (`GetIntVar` etc., `FiltroDistancias.cs:446-488`).

**Casos de uso extraídos (CU-B03).** `CondicionDistancias.aplicar(grupo, textos)`, `.volcar(grupo)`, `.estadisticas`, `.guardar/abrir/copiar/pegar(.dist)`.

**Área Mac propuesta.** *Condiciones y filtros* → inspector/hoja modal del grupo en edición. Propuesta; decide el dueño.

### Pantalla 44 · EscrutarCombinacionesFrm — «Escrutar combinaciones»

**Ficheros.** `VM` = `EscrutarCombinacionesFrmViewModel.cs` (773), `XAML` = `EscrutarCombinacionesFrmPage.xaml` (299), `CS` = `EscrutarCombinacionesFrmPage.xaml.cs` (44). Legacy: `Free1X2/UI/EscrutarCombinacionesFrm.cs` (citado en los comentarios del VM: `VM:271,333,403,533,619,644`). Motor: `Domain/Escrutinio/EscrutadorComb.cs` (spec en ESPEC_05).

**Propósito.** Escruta **ficheros de combinación** (`.comb`/`.xml`, no listas de columnas) contra una o varias columnas ganadoras: cuenta cuántas columnas «a N aciertos de la ganadora» acepta cada combinación (con sus condiciones) y opcionalmente lista las premiadas (`VM:75-83`, `XAML:16`).

**Cómo se abre.** Menú Combinación «Escrutar combinaciones…» (`MainWindow.xaml.cs:226`). El botón de la barra `bEscrutarComb` está **deshabilitado** a propósito, igual que el original (`MainWindow.xaml.cs:392`). Sale a «Posibles premios» (pantalla `PosiblesPremiosFrmPage`, `VM:649`) y «Ver Premiadas…» (`ColumnasPremiadasFrmPage`, `VM:770-771`).

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| «Fichero a escrutar» | selección múltiple `.comb`,`.xml`; reemplaza la lista (`_archivosComb` rutas, `FicherosAEscrutar` nombres) | `VM:268-282`, `XAML:28` |
| «No Aciertos» | texto, defecto `10-14`; lista con `,` y `-` (`RangosHelper.ObtenIntArray`) | `VM:196`, `Domain/Utils/RangosHelper.cs:31-59` |
| «Activar Ver Premiadas» | `bool`, defecto `false` | `VM:200` |
| Modo | combo: 0 «Escrutinio simple» (defecto), 1 «contra fichero», 2 «contra jornadas» | `VM:162-170` |
| Simple: 14 toggles 1/X/2 por partido + texto «Columna ganadora» (MaxLength 14) | sin toggle ⇒ `*`; **el texto, si no está en blanco, tiene prioridad** (`Trim().ToUpper()`) | `VM:54-73,213-217,622-639`, `XAML:73-97` |
| Fichero: «Fichero referencia» | `.txt` de columnas ganadoras; defecto `(selecciona)` | `VM:285-293,222` |
| Jornadas: plantilla de nombre | texto, botones `/t` y `/j` añaden el marcador al final; «Ver archivos» elige un fichero y copia su nombre | `VM:226,313-333`, `XAML:110-129` |
| Jornadas: dígitos temporada / jornada | toggles; temporada 4 díg. (defecto) o 2; jornada 2 díg. (defecto) o 1 | `VM:235,239,392-393` |
| Jornadas: «Carpeta…» | `FolderPicker` (DocumentsLibrary) | `VM:296-310` |
| Jornadas: lista «Temporadas» | multiselección; se llena de `<exe>/Jornadas/Resultados.txt`, orden descendente | `VM:111-154`, `CS:33-43` |

**Validación de «Escrutar!»** (`VM:336-381`), mensajes literales y orden: sin ficheros y modo≠3 → «Falta fichero a escrutar.»; modo 1 sin signos (tras quitar `*`) → «Falta columna ganadora.»; modo 2 sin referencia → «Falta fichero referencia.»; modo 3: plantilla vacía → «Falta plantilla de nombre de fichero.», sin carpeta → «Falta la carpeta de los ficheros.», sin `/t` → «No se ha puesto el indicador de temporada (/t).», sin `/j` → «No se ha puesto el indicador de jornada (/j).». `ObtenIntArray(NoAciertos)` se evalúa **fuera** del `try` (`VM:384`): texto inválido ⇒ excepción no capturada.

**Lógica de negocio en el ViewModel** (orquestación; el cálculo está en `EscrutadorComb`):

```
Escrutar:                                                                   (VM:383-510)
  colAciertos = ObtenIntArray(NoAciertos)   // p.ej. [10,11,12,13,14]
  globales = {}  (categoría -> nº)        premiadas = []
  modo 3: EscrutarContraJornadas(...)
  si no, por cada archivoComb de archivos:
      e = EscrutadorComb(colAciertos){ ArchivoColumnas=archivoComb, AñadirAGanadoras=verPremiadas }
      si verPremiadas: e.ListaEscrutadasConPremio = lista vacía
      modo 1: e.ObtenerPosiblesPremios(colGan, colAciertos); e.EscrutarCombinacion(0)
      modo 2: ganadoras = ArchivoColumnasTexto(ref).LeerTodasCols(false)
              para jorn=1..n: e.ObtenerPosiblesPremios(ganadoras[jorn-1], colAciertos); e.EscrutarCombinacion(jorn)
      AcumularPremios(globales, e)  // UNA vez tras el bucle (el legacy sumaba dentro: duplicaba en modo 2, VM:455-457)
      si verPremiadas: premiadas += e.ListaEscrutadasConPremio
  AcumularPremios: para i<min(Premios.len, PremiosTotales.len): globales[Premios[i]] += PremiosTotales[i]   (VM:518-528)
  Resultados = una fila por premiada {Columna=ColumnaTexto, Archivo=nombre fichero, Jornada, Premio}
  Histograma = claves de globales ordenadas DESC -> "{a} aciertos: {n}"                      (VM:488-494)
  finally: TiempoTexto = ("Final = " + TimeSpan).prefix(18)   // "Final = hh:mm:ss.f"       (VM:505-508)
  catch: Log + «No se pudo completar el escrutinio:\n\n<msg>»

ObtenerColumnaGanadora (VM:622-639): texto no vacío -> texto.Trim().ToUpper();
  si no, por partido 1..14: '1' si Es1, 'X' si EsX, '2' si Es2, '*' si ninguno.

EscrutarContraJornadas (VM:535-616):
  consulta = "Temp=a or Temp=b…" con las temporadas marcadas; **ninguna marcada ⇒ sin filtro ⇒ todas las jornadas** (VM:545-551)
  para cada fila (Temp,Jorn) del histórico:
      t = Temp en "0000" (si dt==2, últimos 2 dígitos); j = Jorn en "00" si dj==2, si no sin ceros
      patrón = plantilla.replace("/t",t).replace("/j",j)
      si la carpeta existe: por cada fichero que coincide con GetFiles(patrón) -> lista.append(patrón)  // se añade el PATRÓN, no el nombre real
  para cada entrada de la lista:  // se re-deduce (temporada, jornada) del texto:
      posT, posJ = índices de "/t" y "/j" en la plantilla
      si posT > posJ: jorn = nombre[posJ..posJ+2); si 2º char es dígito dj=2 sino dj=1, jorn=1er char
                      jornada = Int(jorn); temporada = Int(nombre[posT+dj-2 ..< +dt])
      si no:          temporada = Int(nombre[posT..<+dt]); jorn = nombre[posJ+dt-2..<+2); si 2º char no es dígito jorn=1er char; jornada=Int(jorn)
      colGan = Quiniela de la fila (Temp,Jorn) del histórico; si no existe, saltar
      e = EscrutadorComb(...){ArchivoColumnas = carpeta/patrón}; ObtenerPosiblesPremios(colGan); EscrutarCombinacion(jornada); acumular
```

Otros comandos: **Seleccionar/Deseleccionar todas** (`VM:652-664`); **Grabar cols** (`VM:667-708`): filas con `Seleccionado`, diálogo Guardar nombre `columnas`, tipo «Columnas» `.txt`, `ArchivoColumnasTexto.GuardarCols` por fila + `Cerrar()`; mensajes «No hay columnas seleccionadas que grabar.» / «Guardadas N columna(s) en <nombre>.»; **Ver Premiadas** (tarjeta inline, `VM:717-740`) y **Ver Premiadas…** (ventana, `VM:750-772`), ambas con aviso «No hay columnas premiadas. Activa «Ver Premiadas» antes de escrutar.» si vacío.

**Salidas.** Histograma «`N aciertos: n`» (entero sin formato), lista de premiadas `"{Columna}   {Premio} ac.   {Archivo}  j{Jornada}"` (`VM:44-46`; en modo 1 la jornada es `0`), tiempo.

**Peculiaridades a preservar / avisos.**
- `*` no es comodín real: `ConvertidorDeBases.ConvColumnaANumero` trata cualquier carácter distinto de `1`/`2` como `X` (`Domain/Utils/ConvertidorDeBases.cs:43-53`). Ver A-44-1.
- `ObtenerPosiblesPremios` ordena y invierte `colAciertos` **in situ** (`EscrutadorComb.cs:234-236`).
- En modo 3 los ficheros se localizan con `DirectoryInfo.GetFiles(patrón)` (admite `*`/`?` en Windows), pero luego se abre `carpeta/patrón` literal (`VM:563-566,599`): un comodín no funciona realmente. En Swift: buscar con coincidencia exacta de nombre.
- El histórico se lee de `<exe>/Jornadas/Resultados.txt` (TAB: `Temp`,`Jorn`,`Quiniela` ≤15 car.); la lista de temporadas supone el fichero ordenado ascendente (`VM:142-148`).
- Dependencia directa de `System.Data.DataSet/DataView` (a sustituir por un array de structs).

**Casos de uso extraídos (CU-B04, CU-B05, CU-B06).** `EscrutinioCombinaciones.escrutar(modo: .simple|.contraFichero|.contraJornadas, …)`, `HistoricoJornadas.cargar()` (+ `temporadasDisponibles`), `PlantillaJornadas.resolverFicheros(plantilla:carpeta:digitos:)` y `.deducirTemporadaJornada(nombre:)`, `EscrutinioCombinaciones.exportarColumnas(seleccion:)`.

**Área Mac propuesta.** *Escrutinio* → vista principal (pestaña «Combinaciones» junto a «Columnas»), con «Posibles premios» como hoja. Propuesta; decide el dueño.

### Pantalla 45 · EscrutiniosFrm — «Escrutinios»

**Ficheros.** `VM` = `EscrutiniosFrmViewModel.cs` (1013), `XAML` = `EscrutiniosFrmPage.xaml` (542), `CS` = `EscrutiniosFrmPage.xaml.cs` (75). Legacy: `Free1X2/UI/EscrutiniosFrm.cs` (citado en `VM:834,991` y `CS:52`). Motor: `Domain/Escrutinio/Escrutador.cs` (spec en ESPEC_05).

**Propósito.** Escruta **ficheros de columnas** `.txt` contra (1) una columna ganadora tecleada, (2) las columnas de un fichero de referencia o (3) la jornada correspondiente del histórico `Jornadas/Resultados.txt`; muestra por fichero y columna ganadora cuántas columnas tuvieron N aciertos (`VM:71-82`).

**Cómo se abre.** Menú Combinación «Escrutinios…» (`MainWindow.xaml.cs:222`) y botón `bEscrutinio` de la barra Combinación (`:394`). Sale a «Posibles premios» (`VM:978-983`) y «Ver premiadas…» (`ColumnasPremiadasFrmPage`, `VM:970-971`).

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| «Rango de aciertos a mostrar» | texto, defecto `"10-" + NumeroPartidos` = `10-14`; sintaxis `,` y `-` | `VM:108,311`, `XAML:42-47` |
| «Registrar columnas premiadas» | toggle, defecto No | `VM:112`, `XAML:49-55` |
| Pestañas (Pivot): Simple / Contra fichero / Contra jornadas | `TabSeleccionada` 0..2 ⇒ `TipoEscrutinio=Tab+1` | `VM:102,116`, `XAML:70` |
| «Seleccionar ficheros…» (derecha, deshabilitado en pestaña 3) | multiselección `.txt` (DocumentsLibrary; legacy: carpeta `Columnas`) | `VM:377-389,126`, `XAML:284-296` |
| Simple: 14 casillas (`MaxLength 1`, mayúsculas) con cabecera clicable que cicla `* → 1 → X → 2 → *` + texto «Columna ganadora» (MaxLength 14) sincronizados en ambos sentidos | alfabeto declarado `1 X 2 * S` (S = suspendido) | `VM:25-31,149-218`, `XAML:81-141` |
| Simple: «Actualizar desde Free1X2.com» | **stub**: pone `*`×14 y avisa que el servicio online no está disponible | `VM:409-425` |
| Contra fichero: «Fichero de referencia…» | `.txt`, defecto `(selecciona)` | `VM:396-403`, `XAML:160-168` |
| Contra jornadas: plantilla de nombre (con `/t` y `/j`; botones insertan **en la posición del cursor**), «?» (elige fichero y copia su nombre), «Carpeta…», combos «Dígitos de temporada» (4/2, defecto 4) y «de jornada» (2/1, defecto 2), lista multiselección «Temporadas» | igual que 44 | `VM:229-248,453-484`, `CS:43-74`, `XAML:191-262` |

**Validación y mensajes** (`VM:493-527`): sin ficheros y pestaña≠3 → «Falta fichero a escrutar.»; pestaña 2 sin referencia → «Falta fichero de referencia.»; pestaña 3: «Falta plantilla de nombre de fichero.» / «Falta la carpeta de los ficheros.» / «No se ha puesto el indicador de temporada (/t).» / «No se ha puesto el indicador de jornada (/j).». **No** valida la columna ganadora del modo simple (a diferencia de 44). `ObtenIntArray(RangoAciertos)` fuera del `try` (`VM:530`).

**Lógica de negocio en el ViewModel.**

```
Escrutar (VM:490-652):
  colAciertos = ObtenIntArray(RangoAciertos);  limpiar resultados, histograma, premiadas
  resultadosDS = tabla "Resultados": Seleccionado(bool), LineaID(int), Columna, Archivo, P0..P14(int), "Ac. Totales"(string)   (VM:813-826)
  globales = int[NumeroPartidos+1] = 0
  modo 3: EscrutarContraJornadas(...)  // misma localización/deducción de (temporada,jornada) que 44 (VM:659-752);
          cada fichero se escruta con colGan = Quiniela del histórico: EscrutaCombConColumna(colGan, ds, nombre)
  si no, por cada fichero:
      e = Escrutador(colAciertos){ArchivoColumnas=fichero, AñadirAGanadoras=verPremiadas}; _escrutadorActual = e
      modo 1: e.EscrutaCombConColumna(ColumnaGanadora, ds, nombreFichero)
      modo 2: e.EscrutaCombConTemporada(ficheroRef, ds, nombreFichero)   // una pasada por cada columna del fichero de referencia
      si verPremiadas: premiadas += e.ListaPremiadas
      globales[i] += e.PremiosTotales[i]  para i=0..NumeroPartidos
  ultimo?.AñadirPremiosGlobales(globales)      // añade fila «----- TOTALES:» (LineaID=0, Archivo «-------------», Ac.Totales « »)
  ProyectarResultados: orden = colAciertos ordenado DESC -> cabeceras P{n};
       por fila: Premios = concat("{a}: {valor}  ") de cada a con valor no vacío; Conteos[] alineados a las cabeceras
  Histograma = para k de mayor a menor en colAciertos con 0<=a<len(globales): "a -> globales[a]"      (VM:625-632)
  HayResultados = hay filas; PuedeVerPremiadas = verPremiadas && hay premiadas
  finally: Estado = ("Final = " + TimeSpan).prefix(18)   (VM:647-650)
```

Reglas del motor que condicionan la salida (citadas, no del VM): aciertos de una columna = nº de posiciones `i<min(len)` con `ganadora[i]=='*' || ganadora[i]==col[i]`, normalizando `x→X` (`Escrutador.cs:158-186`); `nc[aciertos]++`; `Ac. Totales = Σ i·nc[i]` (`:249-251`); premiada ⇔ aciertos ∈ `colAciertos` (`:205`); `NoBoleto = ceil(NoColumna/8)` (`:213-217`). `S` no tiene tratamiento especial (ver A-45-2).

Comandos auxiliares: **Seleccionar/Deseleccionar todas** (colección y tabla, `VM:854-864`); **Grabar columnas…** (filas con `Seleccionado`, sugerido `columnas`, `.txt`, `ArchivoColumnasTexto.GuardarCols` por fila, mensajes iguales a 44, `VM:870-912`); **Ver premiadas** (tarjeta inline) y **Ver premiadas…** (ventana; solo habilitado si `PuedeVerPremiadas`): fila = `{ArchivoColumnas, Jornada, Columna, Premio, NumeroColumna, NumeroBoleto = NoBoleto + " (" + orden + ")"}` con `orden = NoColumna % 8` (8 si 0) (`VM:920-972`); **Detener** (`PararEscrutinio()`, `VM:832-838`) **sin botón** en el XAML; **Cancelar** (para el escrutinio y vuelve, `VM:989-995`).

**Salidas.** Rejilla: ☐ · Archivo · Nº (LineaID) · Columna · una columna por categoría (desc.) · «Ac. Totales»; histograma `aciertos → columnas` (enteros sin formato); estado `Final = hh:mm:ss.f`.

**Peculiaridades a preservar.**
- La fila «TOTALES» es una fila más de la rejilla y, si se marca, `Grabar columnas` escribe `----- TOTALES:` en el fichero (`VM:873-876`).
- El histograma excluye categorías fuera de `colAciertos`; las columnas de la rejilla también (`VM:781-790`).
- El histórico y la localización de ficheros por plantilla son idénticos a 44 (mismas trampas).

**Casos de uso extraídos (CU-B07, CU-B08).** `EscrutinioColumnas.escrutar(modo:…)` (compartido con 44 para histórico/plantilla), `EscrutinioColumnas.proyectarResultados`, `EscrutinioColumnas.exportarSeleccion`, `ColumnasPremiadas.construirFilas` (boleto = ceil(n/8)).

**Área Mac propuesta.** *Escrutinio* → vista principal. Propuesta; decide el dueño.

### Pantalla 46 · EstimadorPremiosFrm — «Estimación de premios»

**Ficheros.** `VM` = `EstimadorPremiosFrmViewModel.cs` (430), `XAML` = `EstimadorPremiosFrmPage.xaml` (454), `CS` = `EstimadorPremiosFrmPage.xaml.cs` (19, sin lógica). Controles auxiliares: `Free1X2.WinUI/Controls/PorcentajesControl` + `PorcentajesHelper.cs`, `FilaPorcentaje.cs`. Legacy: `Free1X2/UI/EstimadorPremiosFrm.cs` (números de línea legacy citados en los comentarios del VM, p. ej. `VM:134,201,240,253,385`).

**Propósito.** Predice **acertantes y premio por categoría (14..10)** para una columna premiada, a partir de los porcentajes apostados por partido (1/X/2), la recaudación, el bote y el reparto de premios; permite además cargar/guardar el «escrutinio real» (datos L.A.E.) de la jornada (`VM:40-45`, `docs/MANUAL_USUARIO.md:415-421`).

**Cómo se abre.** Menú Utilidades «Estimación de premios…» (`MainWindow.xaml.cs:266`) y botón `bEstimacion` (`:428`). Pantalla autónoma.

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| Temporada | texto; defecto `año-1/año` si mes<7, si no `año/año+1` | `VM:55,120-125` |
| Jornada | `NumberBox` double, Min 0, defecto 0; ◀ ▶ (mín. 1 / máx. 44) | `VM:58,235-258`, `XAML:61-76` |
| Recaudación | double ≥0, defecto 13 825 260,50 | `VM:62`, `XAML:113-118` |
| Bote | double ≥0, defecto 0 | `VM:65` |
| Precio apuesta | double ≥0, defecto 0,5 | `VM:68`, `XAML:141-148` |
| «Destinado a premios» % | 0-100, defecto 55 — **no interviene en el cálculo** | `VM:76`, `XAML:192-199` |
| % para 14/13/12/11/10 | 0-100, defectos 12/8/8/8/9 | `VM:79-91`, `XAML:207-273` |
| Rejilla de porcentajes 14×3 (1/X/2 por partido) | doubles, defecto 0 | `VM:94-96`, `Controls/PorcentajesHelper.cs:15-20,23-34` |
| Columna premiada | texto 14, mayúsculas, defecto `11111111111111` | `VM:100`, `XAML:309-312` |
| Nº apuestas | double ≥0, defecto 28 000 000 | `VM:104`, `XAML:321-328` |
| Escrutinio real: acertantes/premio por categoría | `NumberBox` ≥0 (5 filas) | `VM:33-46`, `XAML:426-436` |

**Acciones.**
- **Calcular** (`VM:131-199`). Ver pseudocódigo. Columna `null` o <14 caracteres ⇒ no calcula y escribe en `NumApuestasTexto` el texto «Columna premiada inválida (14 signos)» (`VM:138-142`; no hay diálogo).
- **◀ / ▶ Jornada** (`VM:235-258`): `Jornada--` si >1 / `Jornada++` si <44 y luego `RecargarDatosJornada()` (`VM:235-258`). **Editar el número a mano no recarga** (no hay `OnJornadaChanged`; el legacy lo hacía por `TextChanged`).
- **Copiar previsión al escrutinio real** (`VM:366-374`): `Acertantes=ParseDouble(texto)`, `Premio=ParseDouble(texto)` para las 5 filas (parseo con cultura actual; falla ⇒ 0). El legacy copiaba al portapapeles (`VM:363-364`).
- **Guardar escrutinio** (`VM:382-409`): misma validación de columna; `new LAE(Temporada, Jornada.ToString(), ColumnaPremiada[0..14), Recaudacion)`; `acertantesReales[i] = (int)EscrutinioReal[i].Acertantes` (trunca); `LAE.GrabarJornada(acertantesReales)`; error ⇒ «No se ha podido grabar el escrutinio: <msg>».
- **Resultados escrutinio oficial** (`VM:414-429`): abre `https://www.loteriasyapuestas.es/es/la-quiniela` en el navegador (errores solo a log).

**Lógica de negocio en el ViewModel — Calcular** (`VM:131-199`, `VM:201-218`, `VM:220-232`):

```
pct[k]      = %Para(14,13,12,11,10)[k] / 100                      (k=0..4)
destinado[k]= Recaudacion * pct[k];   destinado[0] += Bote        // el bote solo al 14
v           = matriz double[14,3] de la rejilla
b100[i,j]   = Float( v[i,j] / Float(v[i,0]+v[i,1]+v[i,2]) )       // ValoresBase100: factor = (float)(suma en double); división en double; resultado float
signos[p]   = "1X2".IndexOf(col[p])   // -1 si carácter no válido -> se trata como 0 ('1')
probCat14   = 1.0 (double); para p=0..13: s = max(signos[p],0);
              probCat14 *= p[p,s]  (float promovido a double)
              cr[p] = Float( (1 - b100[p,s]) / b100[p,s] )        // aritmética float32
acertantes[0] = numApuestas * probCat14          // numApuestas = (int)NumApuestas
premios[0]    = Round(destinado[0] / acertantes[0], 2)           // ANTES de redondear acertantes[0]
CalcularAcertantes(acertantes, cr, acertantes[0], posInicial=0, profundidad=4):
    rec(pProb, posIni, nivel):   nivel += 1
        para partido = posIni ..< 14:
            prob = pProb * cr[partido]          // double * float
            acertantes[nivel] += prob
            si nivel < 4: rec(prob, partido+1, nivel)
    // acertantes[k] = nº esperado de columnas con exactamente 14-k aciertos (k=1..4), en este orden de sumas
para i=1..4: premios[i] = Round(destinado[i]/acertantes[i], 2);  acertantes[i] = Round(acertantes[i], 0)
acertantes[0] = Round(acertantes[0], 1)
correcciones: si premios[0] > destinado[0] -> premios[0] = destinado[0];  si premios[4] < 1 -> premios[4] = 0   (VM:189-190)
mostrar: Prevision[i].AcertantesTexto = acertantes[i].ToString();  PremioTexto = premios[i].ToString();  NumApuestasTexto = numApuestas.ToString()
```

**Lógica de negocio — RecargarDatosJornada** (`VM:260-348`). Lee `<exe>/Jornadas/InfoJornadasLAE.txt` (si no existe, no hace nada). Cada línea separada por TAB (≥9 campos): `[0]` columna premiada, `[1]` temporada, `[2]` jornada (2 dígitos), `[3]` recaudación expresada **en nº de apuestas** (coma decimal), `[4..8]` premio por categoría 14..10 **en unidades de apuesta** (así lo escribe `LAE.MontaLinea`, `Domain/Utils/LAE.cs:120-139`; el comentario del VM `:263-264` dice «acertantes» y es inexacto).

```
buscada = Int(Jornada) rellenado a 2 con '0'
recorrer líneas; primera con campos[1]==Temporada && campos[2]==buscada:
   ColumnaPremiada = campos[0]
   premiosReales[0] = (campos[4]=="0,00") ? PrecioApuesta*Parse(campos[3])*pct[0] : PrecioApuesta*Parse(campos[4])
   premiosReales[1..4] = PrecioApuesta * Parse(campos[5..8])
   Recaudacion = PrecioApuesta * Parse(campos[3])
   destinado[] recalculado con la UI actual (incluye Bote en el 14) — OJO: usa la Recaudación ANTERIOR a esta asignación
   acertantes[i] = (Parse(campos[i+4]) != 0) ? Round(destinado[i] / Parse(campos[i+4]) / PrecioApuesta, 0) : 0 ; acertantes[0]=0 si Parse(campos[4])==0
   si PrecioApuesta*Parse(campos[8]) < 1 -> premiosReales[4] = 0
   EscrutinioReal[i] = {acertantes[i], premiosReales[i]}; break
Parse = ParseEsp: vacío -> 0; double.TryParse(cultura actual, Any); si falla, reemplaza ',' por '.' con cultura invariante; si falla, 0
```

**Salidas.** Tabla «Previsión de premios» (categoría, acertantes, premio) con `double.ToString()` de la cultura actual (G15: p. ej. `0,3`, `1234,56`); acertantes del 14 con 1 decimal, resto con 0; premios con 2 decimales (`VM:177-186,193-198`). Escrutinio real editable (NumberBox).

**Trampas Swift.** T3: `float` en `b100` y `cr`; T4: `Math.Round(x,2)` es `ToEven`: en .NET Core se implementa como `(x*100).rounded(.toNearestOrEven)/100` (verificar con golden); T7: `ParseEsp` depende de la cultura (con `en-US` y `NumberStyles.Any` la coma sería separador de miles) ⇒ fijar `es-ES`; T8: la lectura/escritura de `InfoJornadasLAE.txt` usa `ArchivoColumnasTexto` (fin de línea y codificación, ESPEC_07).

**Peculiaridades a preservar.**
- Acertantes ≈ 0 ⇒ `premios[i] = destinado/0 = ∞` y se muestra `∞`; para el 14 la corrección limita a `destinado[0]` (`VM:189`).
- `GuardarEscrutinio` **ignora la columna «Premio» tecleada**: LAE recalcula los premios con precio, `%14` y recaudación de `parametros.free1x2` (`ObtenValoresLAE`, `Domain/EntradaSalida/AConfiguracion.cs:402-413`), fijando 10 % a las categorías 13..10 (`LAE.cs:50-53`), y `_Premios[i] = Round(Recaudación/Precio·%/100/acertantes + 0.005, 2)` (`LAE.cs:141-148`); acertantes 0 ⇒ ∞.
- `GrabarJornada` reescribe todo el fichero (sustituye la línea de la misma temporada/jornada o añade al final) (`LAE.cs:92-118`).

**Casos de uso extraídos (CU-B09, CU-B10).** `EstimadorPremios.estimar(porcentajes:columna:apuestas:recaudacion:bote:reparto:)` → `PrevisionPremios`, `HistoricoLAE.cargarJornada(temporada:jornada:)`, `HistoricoLAE.guardarEscrutinio(…)` (reutiliza `LAE`), `EstimadorPremios.copiarPrevision`.

**Área Mac propuesta.** *Escrutinio* (o *Análisis*) → vista principal con formulario + tabla. Propuesta; decide el dueño.

### Pantalla 47 · EstucolFrm — «EstuCol» (generador/analizador de columnas probables)

**Ficheros.** `VM` = `EstucolFrmViewModel.cs` (292), `XAML` = `EstucolFrmPage.xaml` (109), `CS` = `EstucolFrmPage.xaml.cs` (26). Legacy: `Free1X2/UI/EstucolFrm.cs` (citado en `VM:62,95,125,173`). Visor destino: `VisorAnalisisColumnasAbdonFrmPage` (otro alcance).

**Propósito.** A partir de un fichero de columnas *reducidas* (línea = columna de 14 signos) y otro de *ganadoras*, empareja las reducidas por pares (OR de bits), escruta cada columna emparejada contra cada ganadora y genera el informe ABDON de series de aciertos (`VM:17-27`, `docs/MANUAL_USUARIO.md:470-471`).

**Cómo se abre.** Menú Utilidades «EstuCol…» (`MainWindow.xaml.cs:274`; no tiene botón de barra). Al terminar navega a `VisorAnalisisColumnasAbdonFrmPage` (`CS:24`).

**Entradas.**

| Control | Tipo / defecto | Evidencia |
|---|---|---|
| «Entrada Columnas» → «Archivo…» | `.txt`/`*`; muestra nombre o «Sin archivo» | `VM:96-102,81-82`, `XAML:32-40` |
| «Entrada Columnas Ganadoras» → «Archivo…» | idem; al elegirlo cuenta **todas las líneas** (incluidas vacías) → `_noTotalGanadoras` | `VM:105-123`, `XAML:58-66` |
| «Agrupación de Columnas»: `1,2 - 3,4...` (A, defecto) / `1,2 - 2,3...` (B) / `1,3 - 2,4...` (C) | radios | `VM:66-74`, `XAML:78-91`, `Domain/Enums.cs:98` |
| «Obtener Informe» | habilitado solo si hay ambos ficheros | `VM:92-93`, `XAML:96-100` |

**Acciones.** **Obtener Informe** (`VM:128-169`): `Estado` recorre «Comprobando Entradas» → «Emparejando columnas» → «Escrutando Columnas» → «Generando Informe» → «Listo». Si falta algún fichero: «Debe especificar los dos archivos» (inalcanzable con el botón deshabilitado). Excepción ⇒ «Error al generar el informe: <msg>». Éxito ⇒ `UltimoInforme = (porCols, porGans, columnasEmparejadas)` (handoff estático) y navega al visor (`VM:35,154,168`).

**Lógica de negocio en el ViewModel.**

```
colLong(línea) = UtilColumnas.ConvStrToLong(línea)   // 3 bits/partido: '2'->001, 'X'->010, '1'->100; partido 1 en los 3 bits bajos
                                                        // (Domain/Utils/UtilColumnas.cs:196-204)
Emparejar(modo, ficheroReducidas):                                       (VM:177-220)
  emparejadas = []; colTemp = 0; colTemp2 = 0; noCol = 0
  por cada línea (sin saltar vacías):  c = colLong(línea); noCol++
     A: si noCol par:  r = colTemp | c ; si r != c  emparejadas.append(r)       ; colTemp = c          // pares (1,2),(3,4)…
     B:                r = colTemp | c ; si r != c  emparejadas.append(r)       ; colTemp = c          // pares (1,2),(2,3)…
     C: si noCol impar: r = colTemp | c ; si r != c append(r); colTemp = c                              // (1,3),(3,5)…
        si par:         r = colTemp2| c ; si r != c append(r); colTemp2 = c                             // (2,4),(4,6)…
     (en el modo A, el 1.er elemento de cada pareja solo se memoriza)
  // «r != c» descarta la pareja cuando la anterior es subconjunto de la actual (la OR no añade nada)

Escrutar(ficheroGanadoras):                                              (VM:226-251)
  si emparejadas vacío: contenedor = int[0,0]
  contenedor = int[emparejadas.count, noTotalGanadoras]
  g = 0; por cada línea del fichero de ganadoras (hasta noTotalGanadoras):
        gl = colLong(línea)
        para i en emparejadas: contenedor[i,g] = popcount(gl & emparejadas[i])       // Escrutador.EscrutaApuestaMultiple (Domain/Escrutinio/Escrutador.cs:104-108)
        g++

GenerarInforme (VM:257-283):
  porCols[i]  = InformeColumnasABDON( serie = fila i del contenedor )    // una columna, todas las ganadoras
  porGans[g]  = InformeColumnasABDON( serie = columna g del contenedor ) // una ganadora, todas las columnas
```

`InformeColumnasABDON` delega en `RelacionCP3(serie)` (`Domain/Analisis/InformeColumnasABDON_UnaColTodasGan.cs:7-14`): escaleras, sándwiches, agrupaciones, mín/máx y suma de aciertos (spec en ESPEC_04).

**Salidas.** Estado textual y, en el visor, los informes (no se formatea nada en esta pantalla).

**Peculiaridades a preservar / trampas Swift.**
- `ConvStrToLong` con un carácter fuera de `2X1` (coma, minúscula `x`, espacio, `\r`) calcula `1 << -1`; en C# el desplazamiento se enmascara a 5 bits ⇒ `1 << 31` (Int32 negativo, extendido a `long` con signo) (`UtilColumnas.cs:201`). Swift (`<<` inteligente) daría otro valor ⇒ replicar con `&<<` y `Int32` o rechazar la línea (decisión: A-47-1).
- El nº de ganadoras se fija al elegir el fichero; si cambia en disco después, se trunca/rellena con ceros (`VM:112-118,242`).
- Líneas vacías cuentan como columna `0` en el emparejamiento y como ganadora (`VM:185-188,117`).

**Casos de uso extraídos (CU-B11).** `EstuCol.emparejar(reducidas:, modo:)`, `EstuCol.escrutar(emparejadas:, ganadoras:)` → matriz de aciertos, `EstuCol.generarInformes(matriz:)` → `[InformeColumnasABDON]` ×2.

**Área Mac propuesta.** *Columnas probables* → hoja de herramienta (3 pasos: ficheros, agrupación, informe) que abre el visor ABDON en el panel principal. Propuesta; decide el dueño.

### Pantalla 48 · ExportadorCPsFrm — «Exportador de Columnas Probables»

**Ficheros.** `VM` = `ExportadorCPsFrmViewModel.cs` (173), `XAML` = `ExportadorCPsFrmPage.xaml` (65), `CS` = `ExportadorCPsFrmPage.xaml.cs` (23, solo cablea `Volver`, `CS:21`). Legacy: `Free1X2/UI/ExportadorCPsFrm.cs` y `Free1X2/UI/Filtros/ColProbablesFrm.cs:748-760` (citado en `VM:59`).

**Propósito.** Diálogo que recibe la lista de columnas probables (CP) en edición y las exporta en dos formatos: «simples» (`.txt`, solo pronósticos) o «con aciertos» (`.clm`) (`VM:17-24`).

**Cómo se abre.** Solo desde la pantalla de Columnas Probables (`ColProbablesFrmViewModel.ExportarCPs`, `ColProbablesFrmViewModel.cs:858-877`): guarda la CP en pantalla si tiene datos (`NecesitaGuardarDatos()` → `GuardarCPActual()`), crea un `FiltroColProbables` temporal con la copia de trabajo y deposita su lista en el handoff estático `ExportadorCPsFrmViewModel.ListaParaExportar` (consumido y puesto a `null` en el constructor, `VM:37,46-54`). No aparece en menús.

**Entradas.** Ninguna editable. Muestra `«N columnas para exportar.»` (singular `«1 columna para exportar.»`, `VM:168-172`) y un texto de estado (inicial «Elige el formato de exportación de las columnas probables.», `VM:41`).

**Acciones.**
- **Exportar CPs simples (sólo columnas \*.txt)** (`VM:72-109`): diálogo Guardar, sugerido `columnas`, extensión por defecto `.txt`, tipos «Columnas Simples (.txt)» y «Todos los archivos» (DocumentsLibrary). Escribe una línea por CP con `ArchivoColumnasTexto.GuardarTodasCols(columnas, conComa: true)` ⇒ cada línea es `PronosticosString` **con comas** tal cual. Estado: «Exportadas N columnas simples a <fichero>.».
- **Exportar CPs con Ac, Acs, Fs (\*.clm)** (`VM:116-153`): sugerido `columnas`, `.clm` por defecto, tipo «Columnas Con Aciertos (.clm)». `StreamWriter` directo; por CP escribe `PronosticosString#GetAciertos()#GetAciertosSeguidos()#GetFallosSeguidos()`. Estado: «Exportadas N columnas con aciertos a <fichero>.».
- **Cancelar** (`VM:161-166`): `Estado="Cancelado."` y vuelve.
- Error de escritura ⇒ log + diálogo «No se pudieron exportar las columnas … a:\n<ruta>\n\n<msg>» + `Estado = "No se pudo exportar: <msg>"` (`VM:102-107,146-151`).

**Lógica de negocio en el ViewModel.** Solo la composición de la línea `.clm` (arriba). Formatos que viven en el motor (`Domain/MotorCalculo/ColumnaProbable.cs`): `PronosticosString` = pronósticos por partido concatenados con `,` sin coma final (`:678-690`); `GetAciertos/GetAciertosSeguidos/GetFallosSeguidos` = índices con `true` en `AC/ACS/FS`, separados por `,`, cada uno con su letra de tolerancia pegada si la hay (`:605-676`). `ArchivoColumnasTexto` escribe UTF-8 **sin BOM** con buffer 64 KB (`Domain/EntradaSalida/ArchivoColumnasTexto.cs:67-77`); `GuardarColsComa` no quita comas (`:98-103`), `GuardarCols` sí (`:79-87`).

**Salidas.** Ficheros `.txt` / `.clm` (una CP por línea). Fin de línea `WriteLine` (T8: `\r\n` en Windows; decidir `\n` en Mac y que el lector acepte ambos).

**Casos de uso extraídos (CU-B12).** `ColumnasProbables.exportarSimples(lista:, destino:)`, `ColumnasProbables.exportarConAciertos(lista:, destino:)` (formato `.clm`).

**Área Mac propuesta.** *Columnas probables* → no necesita pantalla propia: menú «Exportar…» del editor de CPs (o `fileExporter` con selector de formato). Propuesta; decide el dueño.

### Pantalla 49 · FigurasFiltrosFrm — Editor de «Figuras» de un filtro

**Ficheros.** `VM` = `FigurasFiltrosFrmViewModel.cs` (218), `XAML` = `FigurasFiltrosFrmPage.xaml` (72), `CS` = `FigurasFiltrosFrmPage.xaml.cs` (31). Legacy: `Free1X2/UI/Filtros/FigurasFiltrosFrm.cs` (citado en `VM:62,83,128,168,204`).

**Propósito.** Edita la lista de «figuras» (`List<long>`) de una condición: cada figura es una secuencia de enteros (p. ej. `3-2-2-1-1`, ver `docs/MANUAL_USUARIO.md:59-61`) que el padre comparte por referencia (`VM:51-57`).

**Cómo se abre.** Solo desde otras condiciones: Contactos (`ContactosFrmViewModel.cs:191-203`, `NombreCondicion="Contactos"`) y Signos Seguidos (`SignosSeguidosFrmViewModel.cs:283-290`, `NombreCondicion="SignosSeguidos"`). Protocolo de handoff estático: el padre asigna `FigurasEnEdicion` (la misma `List<long>`) y `NombreCondicion` antes de navegar; `OnNavigatedTo` llama `CargarDesdeHandoff()` (`CS:26-30`, `VM:85-100`). Sin padre ⇒ lista local vacía.

**Entradas.** Rejilla de 12 casillas iniciales (más hasta `figuras+2`, `VM:49,90-93`), cada una un `TextBox` de `MaxLength 6` centrado con etiqueta 1-based; texto = figura como `a-b-c…` (`XAML:42-44`, `VM:21-38`). Ayuda mostrada: «Valores admitidos por casilla: 0 a 16, o el comodín \*.» (`VM:109-110`) — **no se cumple** (ver peculiaridades).

**Acciones.**
- **Aceptar** (`VM:126-150`): vacía `_figurasCondicion` (¡es la lista del padre!) y, por cada casilla con texto: `UtilidadesEntradasValores.ObtenerLongFiguraFromText`; si lanza ⇒ se omite en silencio; si `EsFiguraValida` y no está repetida ⇒ se añade; luego vuelve.
- **Abrir…** (`VM:166-199`): diálogo Abrir con extensión `.fig{Cond}` y `*` (Contactos → `.figContactos`; SignosSeguidos → `.figV1X2`; otro → `.fig`, `VM:154-163`); lee líneas, ignora vacías, convierte cada una con `ObtenerLongFiguraFromText(línea.Trim())`, **sustituye la lista** y repinta las casillas. Error ⇒ «No se pudo abrir el archivo de figuras: <msg>».
- **Borrar** (`VM:202-210`): vacía casillas y la lista del padre.
- **Cancelar** (`VM:213-217`): vuelve sin tocar nada **pero** Borrar/Abrir ya habían mutado la lista compartida.

**Lógica de negocio en el ViewModel.**

```
CargarDesdeHandoff: lista = FigurasEnEdicion ?? []; asegurar casillas >= lista.count+2;
   casilla[i].Texto = i<lista.count ? ObtenerTextoFiguraFromLong(lista[i]) : ""
Aceptar: lista.clear(); por casilla con Trim()!="":
   f = ObtenerLongFiguraFromText(texto)   // Domain/Utils/UtilidadesEntradasValores.cs:220-232
   si lanza -> continue;  si EsFigura(f) && !lista.contains(f) -> lista.append(f)
EsFiguraValida(f): mientras f!=0 { v = f & 15; si v<0||v>16 -> false; f >>= 4 } -> true   // siempre true (v ∈ 0..15)
```

Codificación (Domain): `ObtenerLongFiguraFromText("a-b-c")`: por cada trozo `fig <<= 4; fig |= (uint)Convert.ToInt32(trozo)` (`UtilidadesEntradasValores.cs:220-232`); `ObtenerTextoFiguraFromLong(fig)`: extrae nibbles de menos a más significativo y los antepone con `-`; `0` → `"0"` (`:238-251`).

**Salidas.** Lista `[Int64]` modificada en el padre; casillas con `a-b-c`.

**Peculiaridades a preservar / avisos.**
- El comodín `*` hace saltar `Convert.ToInt32` ⇒ la casilla se **descarta** sin aviso (`VM:139-142`).
- Un valor 16 (`"16"`) desborda el nibble (0x10) y corrompe la figura; `EsFiguraValida` nunca lo detecta (`v ≤ 15`). Un valor negativo o >15 se mezcla con el nibble vecino.
- Los trozos con espacios (`"3 - 2"`) → `Convert.ToInt32(" 3 ")` acepta espacios; vacío (`"3--2"`) lanza.
- Cancelar no deshace Borrar/Abrir (aliasing de la lista, `VM:87,176,209`).

**Casos de uso extraídos (CU-B13, CU-B14).** `Figuras.parsear(texto) -> Int64?`, `Figuras.formatear(Int64) -> String`, `Figuras.leerFichero(url, condicion)`, `EditorFiguras.aceptar(casillas, inout lista)`.

**Área Mac propuesta.** *Condiciones y filtros* → hoja modal secundaria abierta desde los inspectores de Contactos y Signos Seguidos. Propuesta; decide el dueño.

### Pantalla 50 · FiltroPorcenJB — «Separador Porcentajes Juan Bellas»

**Ficheros.** `VM` = `FiltroPorcenJBViewModel.cs` (570), `XAML` = `FiltroPorcenJBPage.xaml` (329), `CS` = `FiltroPorcenJBPage.xaml.cs` (16, sin lógica). Legacy: `Free1X2/UI/FiltroPorcenJB.cs` (líneas citadas en `VM:15-26`: `RecuperaPantalla` 224, `valora` 265, `Cambia` 386, `GrabaCols` 398, `Calcular` 424, `Valida` 465, `Analizar` 536, `ExporCols` 602, `s2n` 629).

**Propósito.** Clasifica los porcentajes de cada signo de los 14 partidos en **7 bandas** (por 6 límites crecientes), y filtra un fichero de columnas aceptando solo las que tienen, por banda, un nº de aciertos dentro de un rango y un «recorrido» (máx−mín entre bandas) dentro de otro (`XAML:18`, `VM:10-18`). También analiza columnas ganadoras banda a banda.

**Cómo se abre.** Sin entrada de menú ni de barra. Se llega desde el Generador de CPs (`GenerarCPsViewModel.cs:205`: `Navegar?.Invoke(typeof(FiltroPorcenJBPage))`, pantalla 59). Pantalla autónoma (no usa `AppState`).

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| «Límite 1…6» | `NumberBox` double (se trunca a Int), defectos 15, 22, 29, 36, 48, 61; deben ser **no decrecientes** | `VM:46-62,470-495`, `XAML:41-70` |
| «Banda 1…7» y «Recorrido» (rangos) | texto `min-max`, defecto `0-3` ×7 y `0-14` | `VM:64-88`, `XAML:96-127` |
| Rejilla de porcentajes 14×3 (1/X/2) | `PorcentajesControl`, doubles ≥0, defecto 0 | `VM:29-30`, `Controls/PorcentajesControl.xaml:47-52` |
| «Columna ganadora» | texto libre, defecto `COL.GANADORA` | `VM:91-92`, `XAML:257-259` |

**Acciones.**
- **Calcular** (`VM:146-203`): `RecuperaPantalla()`; falso ⇒ «error en datos de entrada». Diálogo Abrir (`.txt`,`*`, DocumentsLibrary). Lee línea a línea (en el hilo de UI, sin `await` dentro del bucle).
- **Grabar Resultado** (`VM:205-226`): sin admitidas ⇒ info «No hay columnas admitidas que grabar. Ejecute primero Calcular.»; Guardar `Resultados.txt`; escribe cada admitida con `4→X`.
- **Cancelar** (`VM:228-233`): `_salida = true`. Como `Calcular` pone `_salida=false` al empezar y su bucle no cede el hilo de UI, en la práctica **no puede interrumpirlo** (inferencia del código; ver A-50-1).
- **Salvar Rangos / Leer Rangos** (`VM:235-301`): fichero `.jb7`, nombre sugerido `RangosJB`, tipo «Condiciones»; formato de 9 líneas: `Rango1…Rango7`, `Recorrido`, `l1,l2,l3,l4,l5,l6` (enteros invariantes). Leer tolera menos líneas (solo sobrescribe lo presente); `ADouble` = `double.TryParse` cultura actual, 0 si falla.
- **Exportar** (`VM:348-374`): requiere `RecuperaPantalla`; Guardar `SignosPorBanda.txt` («Salida»); 7 líneas (una por banda) × 14 códigos separados por coma con `Cambia`.
- **Cargar Ganadoras / Anterior / Siguiente** (`VM:376-438`): `.txt`; cada línea `Trim().ToUpper()`; una línea <14 caracteres ⇒ «col.G. errónea=<línea>» y aborta (la lista ya se había vaciado); vacío ⇒ «El fichero de ganadoras está vacío.»; muestra la última (`ContadorGanadoras` = nº, `ColumnaGanadora` = esa); Anterior/Siguiente la recorren sin salir de rango.
- **Analizar** (`VM:303-346`).

**Lógica de negocio en el ViewModel** (todo reproducido del legacy, sin llamadas a `Free1X2.Domain`):

```
RecuperaRangos():                                                         (VM:470-495)
   para (Rango1..7, Recorrido) -> rks[k] = (Int(aux[0]), Int(aux[1])) con aux = texto.split('-')   // sin '-' o no numérico => error => false
   lims[0..5] = Int(Limite1..6)  (truncado);  para i=1..5: si lims[i] < lims[i-1] -> false

Valora(percent, ncol)  (ncol: 0='1', 1='X', 2='2'):                         (VM:506-518)
   banda = 0 si percent<=lims[0]; 1 si <=lims[1]; … 5 si <=lims[5]; si no 6
   bit   = (ncol==0) ? 1 : (ncol==2 ? 2 : 4)           // 1->1, 2->2, X->4

RecuperaPantalla():                                                         (VM:446-466)
   RecuperaRangos() o false;  cps[14][7] = 0
   para partido, ncol: p = Int(nvals[partido][ncol])  (TRUNCA el double);  cps[partido][Valora.banda] += Valora.bit
   scps[banda] = concatenación (partido 0..13) de los dígitos cps[partido][banda]  ('0'..'7')

Valida(col):  // col con 'X' sustituida por '4'                             (VM:535-556)
   max=0; min=98
   para banda 0..6: na = #{ i<14 : scps[banda][i] != '0' && (scps[banda][i] & col[i]) > '0'(48) }
        // equivale a (dígito & bit_del_signo) != 0
        si na fuera de [rks[banda].min, rks[banda].max] -> false;  actualiza max/min
   rec = max-min;  aceptar solo si rks[7].min <= rec <= rks[7].max

Calcular, bucle: por línea: col = Trim().ToUpper(); saltar vacías; ctini++
   si len<14: error «error longitud=<col>» y salir (se conserva lo ya aceptado, se muestran contadores)
   idx = S2n(col) con S2n = base 3: '1'->+1, '2'->+2, cualquier otro ('X','4'…)->0 (primeros 14 caracteres)   (VM:559-569)
   si idx no visto antes (HashSet; evita columnas repetidas): col = col.replace('X','4'); si Valida(col) -> validas.append, ctadm++
   al final: Procesadas=ctini, Admitidas=ctadm, Tiempo=(ahora-ini).ToString() completo (TimeSpan, 7 decimales)

Analizar:                                                                  (VM:303-346)
   RecuperaPantalla() o error «error en datos de entrada»
   col = ColumnaGanadora.replace('x','4').replace('X','4'); si len<14 -> «col.G. errónea=<col>»
   para banda 0..6: na[banda] como en Valida (sin comprobar rangos); AnalisisBandaK = na; Recorrido = max-min
Cambia(valor): 0->"-", 1->"1", 2->"2", 3->"12", 4->"X", 5->"1X", 6->"X2", 7->"1X2"      (VM:521-531)
```

**Salidas.** Contadores `Procesadas`/`Admitidas` (enteros), `Tiempo` (`hh:mm:ss.fffffff`), `Fichero` (nombre), 7 contadores de banda + `Recorrido` (enteros), ficheros `.txt` / `.jb7`.

**Peculiaridades a preservar.**
- Los porcentajes se **truncan** a entero antes de clasificar (`VM:446-466`); los límites son «≤».
- `S2n` ignora la diferencia `X` vs otros caracteres: dos columnas que difieren solo en caracteres no `1/2` en posiciones distintas de 1/2 se consideran la misma (`VM:559-569`).
- Se guarda el texto completo de la columna aceptada (aunque tenga >14 caracteres) pero solo se evalúan 14 (`VM:175-196,535-556`).
- `ParseaRango` exige exactamente `a-b` (un único guion); rangos negativos no soportados (`VM:497-502`).

**Casos de uso extraídos (CU-B15, CU-B16, CU-B17).** `SeparadorPorcentajesJB.prepararBandas(porcentajes:limites:rangos:)`, `.filtrarColumnas(ficheroEntrada:)`, `.analizarGanadora(_:)`, `.exportarSignosPorBanda()`, `RangosJB.guardar/leer(.jb7)`, `GanadorasNavegador`.

**Área Mac propuesta.** *Herramientas* (filtrado de columnas por porcentajes) → vista principal con tres tarjetas (Bandas, Filtrado, Análisis). Propuesta; decide el dueño.

### Pantalla 51 · Formatos123Frm — Condición «Formatos 123»

**Ficheros.** `VM` = `Formatos123FrmViewModel.cs` (489), `XAML` = `Formatos123FrmPage.xaml` (199), `CS` = `Formatos123FrmPage.xaml.cs` (35). Base: `FiltroArchivoViewModelBase` (§0.3). Legacy: `Free1X2/UI/Filtros/Formatos123Frm.cs` (la lógica equivalente está en `Formatos123Frm.cs`: `CompruebaFormato`, `CompruebaColumna`, `ObtenerAciertosPermitidos`, `MarcarValores`, `TransformarValoracion`, `menuCondiciones1_BOk`, `btnTraducir_Click`, `btnAnalisis_Click`; las citas de línea de este apartado son todas del port WinUI). Motor: `Domain/MotorCalculo/FiltroFormatos123.cs`, `Formato123.cs`.

**Propósito.** Condición sobre «formatos 123»: cada partido se re-etiqueta según su valoración (1 = signo más valorado, 2 = segundo, 3 = tercero) y se exige que ciertas secuencias (formatos) aparezcan un nº de veces en la columna (`XAML:19-22`, `docs/MANUAL_USUARIO.md:243`).

**Cómo se abre.** Rejilla de condiciones de Inicio, «Formatos 123» (`MainPageViewModel.cs:172`) con `GrupoEnEdicion`. Salidas: Estadísticas (`VisorEstadisticasPage`) y «Analizador de formatos» (`AnalisisFormatos123FrmPage`). `OnNavigatedTo` recarga salvo al volver (`CS:25-34`).

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| Lista de formatos (8 filas iniciales, «Añadir fila» las crece; el legacy precargaba 100) | por fila: `Formato` (≤14 car., solo `1`,`2`,`3`), `Mín`, `Máx` (enteros) | `VM:48-56,61,152-169`, `XAML:86-98` |
| «Líneas acertadas» / «Aciertos globales» | texto de lista: enteros y rangos `a-b` separados por coma, solo valores 0..40; etiqueta cambia con el toggle | `VM:74-75,223-251`, `XAML:120-125` |
| «Ignorar repeticiones» | toggle, defecto No (`PasoFijo`); ignora Mín/Máx de las filas | `VM:82-83`, `XAML:127-129` |
| Rejilla de porcentajes 14×3 | valoración 1/X/2 por partido | `VM:68-69`, `PorcentajesControl` |
| Traductor: «1X2…» → «123…» | texto 14 car.; salida solo lectura | `VM:100-107,265-283`, `XAML:147-174` |

**Acciones.** **Aceptar**, **Estadísticas**, **Guardar/Abrir/Copiar/Pegar** (`.123`, `.xml`; temporal `Temp/tmp.123`, `VM:116-123`), **Borrar**, **Cancelar**, **Traducir**, **Analizador de formatos**, **Añadir fila**.
- **Aceptar** (`VM:306-343`): vuelca a `FiltroFormatos123` del grupo: `ValoracionTranformada = TransformarValoracion(nvals)`, `Valoracion = nvals`; `ArrayFormatos`, `ArrayAciertos`; si hay aciertos, `AciertosMin/Max` = menor/mayor tras ordenar; `PasoFijo`, `AciertosFiltro` (texto); `IsActive = NecesitaGuardar()`; si `ContieneDatos==false` ⇒ `= NecesitaGuardar()` (nunca vuelve a false); `ActivaFiltro`; `NotificarCambio`; volver.
- **Estadísticas** (`VM:456-465`): filtro temporal → `CalculadorEstadisticas.EstadisticasFiltro(filtro, <exe>/Ganadoras/)` → visor.
- **Guardar** (`VM:436-441`): filtro temporal → `ArchivoCondiciones.GuardaArchivo`; **Abrir** solo vuelca a pantalla (`MarcarValores`) (`VM:444-453`).
- **Analizador de formatos** (`VM:286-303`): si no hay formatos válidos en pantalla ⇒ aviso «Guarde el Filtro Primero!!!»; si hay, `AnalisisFormatos123FrmPage.FormatosEntrada = lista` y navega.
- **Borrar** (`VM:470-481`): reinicia 8 filas vacías, aciertos vacío, `Ignorar=false`; **no** toca la rejilla de porcentajes ni el filtro del grupo.

**Lógica de negocio en el ViewModel.**

```
ConstruirFormatos():                                                     (VM:194-220)
  por fila: formato = Trim(); si vacío -> saltar; si len>14 o algún char ∉ {'1','2','3'} -> saltar (en silencio)
     si !Ignorar: min = TryParse(Trim(Mín)) y max = TryParse(Trim(Máx)) -> si alguno falla, saltar la fila
     si Ignorar: min=max=0
     lista.add(Formato123{formato,min,max})      // el «if !lista.Contains(f)» no deduplica: Formato123 no sobreescribe Equals (referencia) -> NO hay deduplicación

ConstruirAciertos():                                                     (VM:223-251)
  si texto vacío -> []; por cada parte de texto.split(','):  p = Trim(); vacío -> saltar
     sin '-' : TryParse entero v; si 0<=v<=40 -> add(v)
     con '-' : split('-') debe dar exactamente 2 partes enteras (desde,hasta) -> para j en desde...hasta con 0<=j<=40 -> add(j)  // sin deduplicar, sin validar desde<=hasta
  (valores fuera de rango o mal formados se descartan sin aviso)

TransformarValoracion(v[14][3]) -> byte[14][3]  (bit de ranking por signo [1,X,2]: 4=primero, 2=segundo, 1=tercero)   (VM:347-400)
  para cada partido con (a,b,c)=(v1,vX,v2):
     si a>=b && a>=c:   si b>=c -> (4,2,1)  sino -> (4,1,2)
     sino si b>a && b>=c: si a>=c -> (2,4,1) sino -> (1,4,2)
     sino si c>a && c>b:  si a>=b -> (2,1,4) sino -> (1,2,4)
  // empates favorecen 1 sobre X y X sobre 2 en cada rama

Traducir: entrada.ToUpper(); válida si len==NumeroPartidos y chars ∈ {1,X,2} sino Columna123 = "Error!!!!";
  si válida: FiltroFormatos123{ValoracionTranformada, Valores=nvals}.Col1X2ToCol123(entrada)   // Domain FiltroFormatos123.cs:93-96
```

Evaluación en el motor (cita, para casos de uso): `Formato123.FormatoLong` codifica el formato con alfabeto `"321"` a 3 bits por posición (`'3'→001`, `'2'→010`, `'1'→100`, desplazando 3 bits a la izquierda por carácter; `Formato123.cs:48-70`); la columna se traduce a un long «123» (`TraducirLongColumnaALong123`, `FiltroFormatos123.cs:267`) y `AnalizarColumna` elige paso fijo o libre según `PasoFijo` (`FiltroFormatos123.cs:97-104`).
- *Paso libre* (`:115-157`): por cada formato cuenta cuántas veces `(col & formato) == formato` desplazando la columna 3 bits cada vez (apariciones solapadas, hasta agotar bits); si `AciertosMin ≤ aciertos ≤ AciertosMax` **y** `aciertosGlobales ≤ filtro.AciertosMax` suma un acierto global; si no y `aciertosGlobales > filtro.AciertosMax` hace `break`. Resultado: `ArrayAciertos.Contains(aciertosGlobales)`.
- *Paso fijo* (`:159-181`): por cada formato suma 1 si aparece al menos una vez (ignora Mín/Máx); resultado: `ArrayAciertos.Contains(total)`.
- `Col1X2ToCol123` = `TraducirColumna(columna.ToUpper(), TransformarValoracion(Valores))` (`:93-96`; su `TransformarValoracion` de `:332-385` repite la misma lógica del ViewModel pero con cadenas `"1"/"2"/"3"` en vez de bits 4/2/1; con valores NaN ninguna rama se cumple y la fila queda sin asignar: `null` en cadenas, `0` en bits).

**Salidas.** Texto de columna 123, etiquetas, y los efectos en el filtro del grupo y semáforo de Inicio.

**Peculiaridades a preservar.** Filas inválidas se descartan sin mensaje (`VM:199-216`); rango de aciertos limitado a 0..40 (`VM:234,245`); etiqueta «Líneas Acertadas»/«Aciertos Globales» (`VM:85-90`); el analizador usa lo que hay en pantalla pese al mensaje «Guarde el Filtro Primero!!!».

**Casos de uso extraídos (CU-B18).** `CondicionFormatos123.aceptar(formatos:aciertos:pasoFijo:valoracion:)`, `.transformarValoracion`, `.traducirColumna`, `.estadisticas`, `.guardar/abrir/copiar/pegar`.

**Área Mac propuesta.** *Condiciones y filtros* → inspector/hoja del grupo en edición; «Analizador de formatos» como hoja secundaria. Propuesta; decide el dueño.

### Pantalla 52 · FormatosFrm — Condición «Formatos (1, X, 2, V, *)»

**Ficheros.** `VM` = `FormatosFrmViewModel.cs` (402), `XAML` = `FormatosFrmPage.xaml` (185), `CS` = `FormatosFrmPage.xaml.cs` (117; manejadores de botones y navegación a utilidades). Base: `FiltroArchivoViewModelBase` (§0.3). Legacy: `Free1X2/UI/Filtros/FormatosFrm.cs` (equivalencias citadas en los comentarios del VM: `InicializaDatos`, `GuardarDatosFormatos`, `NecesitaBorrarUltimoFormato`, `ObtenerFiltroTemporal`, `abrir`, `guardar`; las citas de línea de este apartado son del port WinUI). Motor: `FiltroFormatosSignos`, `FormatosSignos`, `FormatoSignos` (ESPEC_03).

**Propósito.** Controla la aparición/repetición de **secuencias de signos** (formatos) en las columnas: cada «relación» tiene hasta 30 líneas `formato + rango mín-máx de apariciones` más dos límites (`Líneas`, `Global`) (`VM:65-68`, `VM:40-48`, `VM:59`, `docs/MANUAL_USUARIO.md:243`).

**Cómo se abre.** Rejilla de Inicio, botón «Formatos» (`MainPageViewModel.cs:163`, `Filtro.FormatosSignos`) con `GrupoEnEdicion`. Sale a: «Saca formatos» → `CalculoFormatosFrmPage`, «Pares» → `ParejasFrmPage`, «Tríos» → `TriosFrmPage`, «Sumas pares nat.» → `AnalizadorJPMPage` (`CS:43-66`); y a `VisorEstadisticasPage`.

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| Navegador de relaciones ◀ ▶ «Eliminar actual», contador `N/Total` | ▶ más allá de la última **crea** una relación nueva | `VM:248-311`, `XAML:23-41` |
| 30 líneas por relación: «Formato» (mayúsculas, placeholder `p. ej. 1X2`) y «Mín-Máx» (`mín-máx`) | alfabeto `1 X 2 V *`, longitud ≤ `NumeroPartidos` (14) | `VM:25-32,156-166`, `XAML:85-91` |
| «Líneas» y «Global» (por relación) | texto numérico; admite lista/rangos (`SonTodosNumeros`) | `VM:43-48,190-193`, `XAML:109-121` |

**Acciones.** Aceptar, Estadísticas, Guardar/Abrir/Copiar/Pegar (`.fmt`, `.xml`; temporal `Temp/tmp.fmt`, `VM:89-96`), Borrar, Cancelar, cuatro utilidades.
- **Aceptar** (`VM:213-238`): `ConstruirFormatos()`; si el filtro no tenía datos y hay ≥1 relación ⇒ `ContieneDatos=true`, `IsActive=true`; si no hay ninguna ⇒ ambos `false`; `filtro.FormatosSignos = lista`; `ActivaFiltro`; `NotificarCambio`; volver. (Si ya tenía datos y sigue habiendo, `IsActive` no se toca.)
- **Estadísticas** (`VM:374-383`): filtro temporal (`ContieneDatos=IsActive=true` si hay relaciones) → `CalculadorEstadisticas.EstadisticasFiltro(filtro, <exe>/Ganadoras/)` → visor.
- **Guardar** (`VM:341-351`): filtro temporal + `ArchivoCondiciones.GuardaArchivo`. **Abrir** (`VM:355-370`): lee, **copia al filtro del grupo** (`FormatosSignos`, `ContieneDatos`, `IsActive` del leído) y recarga la pantalla (a diferencia de Distancias, aquí sí modifica el grupo).
- **Borrar** (`VM:389-401`): vacía el filtro del grupo (`FormatosSignos=[]`, flags false) y recarga.

**Lógica de negocio en el ViewModel.**

```
ConstruirFormatos():                                                      (VM:172-210)
  para cada relación:
     líneas = [ (formato, rango) de las 30 filas tales que
                formato = Trim().ToUpper();  rango = Trim();
                FormatoEsValido(formato)  [len <= 14 y cada char ∈ {1,X,2,V,*}]  &&  formato != ""  &&  rango != "" ]
     Lineas  = SonTodosNumeros(Trim(LimiteLineas)) ? texto : ""          // SonTodosNumeros: "" o lista/rangos de enteros (Domain/Utils/UtilidadesEntradasValores.cs:362-382)
     Global  = idem con Global
  si la ÚLTIMA relación no tiene ninguna línea válida -> se elimina (solo la última)
  (filas inválidas o sin rango se descartan en silencio; el rango mín-máx no se valida aquí)

CargarDesdeGrupo: por cada FormatosSignos del filtro -> relación con sus líneas (Formato, RangoAparicion), rellena hasta 30 filas en blanco,
                  LimiteLineas = formatos.Lineas, Global = formatos.Global; si no hay ninguna -> una relación vacía; índice = 0     (VM:113-153)
Avanzar: si índice+1 >= count -> añadir relación vacía; índice++ .   Retroceder: si índice>0 -> índice--.
EliminarActual: RemoveAt(índice); si queda vacía añadir una nueva; ajustar índice al último si se pasa.                                 (VM:289-311)
```

Observación: navegar entre relaciones **no** vuelca la relación a ningún filtro (todas viven en `Relaciones` hasta Aceptar), a diferencia de Diferencias.

**Salidas.** Contador `N/Total`; texto en líneas; efectos en el filtro del grupo.

**Peculiaridades a preservar.** Rango vacío ⇒ línea ignorada (no error) (`VM:183`); `Lineas`/`Global` no numéricos ⇒ se guardan como `""` (`VM:190-193`); `Abrir` no valida contra el alfabeto de formatos; `ToUpper()` usa la cultura actual (T7), en Swift restringir a ASCII (`VM:181`); el XAML no limita la longitud del texto (`XAML:85-91`), solo `FormatoEsValido` la comprueba al construir (`VM:160`).

**Casos de uso extraídos (CU-B19).** `CondicionFormatosSignos.construir(relaciones:)`, `.volcar(filtro)`, `.navegarRelaciones`, `.estadisticas`, `.guardar/abrir/copiar/pegar`.

**Área Mac propuesta.** *Condiciones y filtros* → inspector/hoja del grupo en edición, con las cuatro utilidades de formatos como hojas secundarias. Propuesta; decide el dueño.

### Pantalla 53 · FraccionadorFrm — «Fraccionador»

**Ficheros.** `VM` = `FraccionadorFrmViewModel.cs` (256), `XAML` = `FraccionadorFrmPage.xaml` (183), `CS` = `FraccionadorFrmPage.xaml.cs` (24, sin lógica). Legacy: `Free1X2/UI/FraccionadorFrm.cs` (`FracTrams` :150-167, `FracCols` :168-200).

**Propósito.** Carga un `.txt` de columnas y lo reparte en varios ficheros numerados (`BASE01.txt`, `BASE02.txt`…) por cuotas fijas de columnas o en N tramos iguales (`VM:14-22`).

**Cómo se abre.** Menú Operaciones «Fraccionador…» (`MainWindow.xaml.cs:247`) y botón `bFraccionador` (`:383`). Autónoma. (`docs/MANUAL_USUARIO.md:482` la marca «en migración»; el VM sí implementa el reparto, ver abajo.)

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| «Entrada…» | `.txt` (DocumentsLibrary); carga TODAS las líneas en memoria (`while Peek()>0 ReadLine`) | `VM:99-126`, `XAML:46-51` |
| Radio «Columnas» (defecto) / «Tramos» | `PorColumnas=true` | `VM:35-51`, `XAML:68-77` |
| 20 cuotas «Por columnas» | `NumberBox` enteros ≥0, defecto 0 | `VM:26-33,239-256`, `XAML:121-127` |
| «¿Cuántos tramos?» | `NumberBox` ≥0, defecto 0 | `VM:56-58`, `XAML:80-88` |

**Acciones.** **Entrada…** (`VM:99-126`: guarda la lista, `NombreArchivoEntrada`, `TotalColumnas`, habilita Fraccionar, tiempo); **Fraccionar** (`VM:133-165`, solo habilitado con entrada cargada): diálogo Guardar sugerido `BASE`, tipo «Nombre BASE salida» `.txt`; `fileBase = carpeta + nombre sin extensión`; ejecuta `FracCols` o `FracTrams` en segundo plano; muestra `Archivos` y `Tiempo`. Errores ⇒ «Error al leer el archivo: <msg>» / «Error al fraccionar: <msg>». **Cerrar**: sin efecto en el VM (`VM:228-231`).

**Lógica de negocio en el ViewModel.**

```
nombreSalida(k) = fileBase + String(format:"%02d", k) + ".txt"          // "{0:d2}" ; k=1.. (más de 99 -> 3 dígitos)

FracCols(cuotas[20]):                                                   (VM:171-189)
  acum=0; numfiles=0
  para i in 0..<20: q = cuotas[i]; si q == 0 -> break          // se detiene en la 1.ª cuota 0
      escribir nombreSalida(i+1) con columnas[acum ..< min(acum+q, N)]   // el fichero se crea aunque quede vacío
      acum += q;  numfiles = i+1
  return numfiles

FracTrams(q):                                                           (VM:195-214)
  si q <= 0 -> return 0
  part = N / q  (división entera);  acum=0; lim=part
  para k in 0..<q:
      escribir nombreSalida(k+1) con columnas[acum ..< lim]
      acum += part;  lim = acum + part
      si N - lim < part: lim = N                                  // el último recibe el resto SOLO si resto < part
  return q
```

Tiempo mostrado: `TimeSpan.ToString() + "0000000000"` recortado a 10 caracteres ⇒ `hh:mm:ss.f` (`VM:217-221`).

**Salidas.** Contadores «Columnas», «Archivos», «Tiempo»; ficheros `.txt` con `WriteLine` (UTF-8 sin BOM, T8).

**Peculiaridades a preservar (el legacy es idéntico: `UI/FraccionadorFrm.cs:150-167`).**
- Con `resto = N − part·q ≥ part` (p. ej. N=10, q=4: part=2) las columnas sobrantes **no se escriben en ningún fichero** (el último fichero cubre `[6,8)` y se pierden 8 y 9). El comentario «el último archivo recibe el resto» (`VM:192`) solo es cierto si `resto < part`.
- `q > N` ⇒ `part=0`: se crean `q` ficheros vacíos salvo el último, que recibe todas (`lim` pasa a N en la penúltima).
- Líneas vacías del fichero de entrada cuentan como columnas (`VM:113`).
- Si una cuota supera las columnas restantes, el fichero se crea (posiblemente vacío) y `acum` sigue creciendo (`VM:182-185`).
- Sin protección contra sobrescritura de ficheros existentes.

**Casos de uso extraídos (CU-B20).** `FraccionarColumnas.porCuotas(columnas:cuotas:base:)`, `FraccionarColumnas.porTramos(columnas:tramos:base:)` → `[URL]`.

**Área Mac propuesta.** *Herramientas* (operaciones sobre ficheros de columnas) → hoja modal/ventana de utilidad; salida a una carpeta elegida. Propuesta; decide el dueño.

### Pantalla 54 · FrmDependenciaLineal — «Dependencia lineal»

**Ficheros.** `VM` = `FrmDependenciaLinealViewModel.cs` (447), `XAML` = `FrmDependenciaLinealPage.xaml` (228), `CS` = `FrmDependenciaLinealPage.xaml.cs` (22, solo `Volver`). Legacy: `Free1X2/UI/FrmDependenciaLineal.cs` (métodos `LeerColumnas`, `ProponerCoeficientes`, `TestSignosPartidoATratar`, `GrabarColumnas`, citados en `VM:120,147,214,317,355,393,430`).

**Propósito.** Recalcula el signo de un «partido a tratar» como **combinación lineal módulo 3 o 2** de los signos de los demás partidos (con coeficientes 0/1/2) para cada columna de un fichero, y graba las columnas resultantes (`VM:58-63`, `docs/MANUAL_USUARIO.md:479-480`).

**Cómo se abre.** Solo botón `bDependenciaLineal` de la barra Utilidades (`MainWindow.xaml.cs:433`); sin menú. Autónoma.

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| Fichero de entrada «Seleccionar…» | `.txt`/`*` (DocumentsLibrary); lee y propone coeficientes | `VM:123-159` |
| Fichero de salida «Seleccionar…» | Guardar, sugerido `columnas`, `.txt`; **si no se elige, es el mismo que el de entrada** | `VM:166-177,151-155` |
| 14 filas `P1..P14`: toggles 1/X/2 (solo habilitados en el partido a tratar), `NumberBox` coeficiente 0..2 (paso 1), botón «Tratar este» | coef. truncado a Int y acotado a 0..2 | `VM:24-56`, `XAML:151-190`, `VM:242-249` |
| Estado (texto) | inicial «Falta seleccionar fichero de columnas» | `VM:107-108` |

**Acciones.**
- **Seleccionar entrada** (`VM:123-159`): lee el fichero (`LeerColumnas`) en segundo plano; error ⇒ «Error al leer las columnas: <msg>» (estado + diálogo). `ProponerCoeficientes`; si no hay salida, salida = entrada; estado «Seleccionar partido e indicar coeficientes»; habilita Aceptar.
- **Tratar este** (`VM:184-210`): marca ese partido como único a tratar, limpia los signos de los demás y activa los tres signos del elegido.
- **Aceptar** (`VM:217-304`): ver pseudocódigo. Mensajes de estado: «Seleccione el partido a tratar», «Debe poner un triple o un doble en el partido a tratar», «Calculando columnas...», «Se han grabado N columnas», «Error al calcular/grabar las columnas: <msg>». Al terminar **relee el fichero de salida** y sustituye `_bits` por su contenido (`VM:293-303`, `LeerColumnas` asigna `_bits`, `VM:324`).
- **Cancelar**: vuelve (`VM:310-313`).

**Lógica de negocio en el ViewModel.**

```
índice(columna) = ConvertidorDeBases.ConvColumnaANumero: dígito base 3 por partido (partido 1 = menos significativo): '1'->0, '2'->2, cualquier otro ('X'...)->1  (Domain/Utils/ConvertidorDeBases.cs:43-53)
signo(i, p) = (i / 3^p) % 3        // 0='1', 1='X', 2='2'

LeerColumnas(ruta):                                                        (VM:321-352)
  _bits = bitset[4 782 969]=false;  cnt[14][3]=0
  por cada columna del fichero (ArchivoColumnasTexto.SiguienteColumna/LeeColumnaSinComas): i = índice(col); _bits[i]=true
        para p en 0..13: cnt[p][signo(i,p)] += 1                          // se cuenta por línea, aunque la columna esté repetida
  porcentajes (solo para proponer coeficientes): suma = cnt[p][0..2]; si suma==0 -> saltar
        pct[p][2] = Round(cnt[p][2]*100/suma, 0); pct[p][1] = Round(cnt[p][1]*100/suma, 0); pct[p][0] = 100 - pct[p][2] - pct[p][1]   // Math.Round = ToEven (T4)

ProponerCoeficientes(pct):                                                  (VM:358-390)
  para p: si pct[p][0]*pct[p][1]*pct[p][2] != 0 -> Coef[p]=1 ; si no -> Coef[p]=0 y, si es el primero así, partidoATratar = p
  partidoATratar por defecto = 0; se marca como único «a tratar» con 1X2 activos.

TestSignosPartidoATratar(s1,sX,s2) -> (modulo, corrector, independiente) | inválido:   (VM:396-428)
  1X2 -> (3,1,0) ; 1X -> (2,1,0) ; X2 -> (2,1,1) ; 12 -> (2,2,0) ; cualquier otra combinación (p. ej. un solo signo) -> inválido

Aceptar:                                                                    (VM:256-281)
  cambiados = bitset[4 782 969]=false
  para i en 0..<4 782 969 con _bits[i]:
      ini  = signo(i, ata)
      suma = Σ_{p != ata} Coef[p] * signo(i, p)
      nuevo = ((suma % modulo) * corrector) + independiente           // signo 0/1/2 resultante
      j = i + 3^ata * (nuevo - ini);  cambiados[j] = true
  grabar: para j ascendente con cambiados[j]: escribir ConvNumAColumna(j); contar   (VM:431-446)
```

`ConvNumAColumna(j)` (`ConvertidorDeBases.cs:99-109`): extrae dígitos base 3 de menor a mayor con alfabeto `1,X,2`, rellena con `1` hasta `NumeroPartidos` ⇒ el **orden de salida es por índice numérico ascendente** (partido 14 es el dígito más significativo), no el del fichero de entrada. Se deduplica por construcción (bitset).

**Salidas.** Mensaje de estado con nº de columnas grabadas (entero), fichero `.txt` de columnas de 14 signos `1/X/2`, sin formato numérico especial.

**Peculiaridades a preservar.**
- Sobrescribe el fichero de entrada si no se elige otro de salida (`VM:151-155`); no hay confirmación.
- El «porcentaje» usado para proponer coeficientes está **redondeado**: un signo con <0,5 % cuenta como ausente (producto 0) (`VM:346-349,364`).
- Tras grabar, `_bits` pasa a ser el contenido del fichero de salida (segundas pulsaciones de Aceptar operan sobre el resultado) (`VM:293-303`).
- Un carácter distinto de `1`/`2` en la entrada se interpreta como `X` (`ConvertidorDeBases.cs:48-50`).

**Trampas Swift.** Bitset de 4 782 969 bits (`[UInt64]`); `%` y `/` enteros (T9); `Math.Round(..., 0)` ToEven (T4) solo para proponer coeficientes.

**Casos de uso extraídos (CU-B21).** `DependenciaLineal.leerColumnas(url)` → (`bitset`, `porcentajes`), `DependenciaLineal.proponerCoeficientes(_:)`, `DependenciaLineal.parametros(signosPartidoATratar:)`, `DependenciaLineal.aplicar(bits:partido:coeficientes:parametros:)` → `bitset`, `DependenciaLineal.exportar(bits:url)`.

**Área Mac propuesta.** *Herramientas* (transformaciones de ficheros de columnas) → ventana de utilidad con progreso. Propuesta; decide el dueño.

### Pantalla 55 · FrmReducidasPerfectas — «Reducciones perfectas»

**Ficheros.** `VM` = `FrmReducidasPerfectasViewModel.cs` (404), `XAML` = `FrmReducidasPerfectasPage.xaml` (195), `CS` = `FrmReducidasPerfectasPage.xaml.cs` (22, sin lógica). Legacy: `Free1X2/UI/FrmReducidasPerfectas.cs` (`GeneraReduccion` :1508-1583, matrices :108-112, `btGenerar_Click` :1432). No usa `Free1X2.Domain`: **toda la lógica vive en el VM**.

**Propósito.** A partir de una **columna base** (un signo por partido) y un **pronóstico** (signos jugados), genera una «reducción perfecta» de 4, 13 u 11 triples o 7 o 15 dobles (método de «Fortuna», foro1x2) y la graba en un `.txt` (`VM:74-92`, `XAML:158-160`, `docs/MANUAL_USUARIO.md:476-477`).

**Cómo se abre.** Solo botón `bRedPerfectas` de la barra Utilidades (`MainWindow.xaml.cs:432`); sin menú. Autónoma.

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| 15 filas: toggles Pronóstico 1/X/2 y botón «Base» que cicla `1→X→2→1` | `Pron1/PronX/Pron2` false; `BaseSigno` 0 («1») | `VM:26-72`, `XAML:97-121` |
| «Número de partidos» | `NumberBox` Min 1, Max 15, defecto 15 | `VM:136-137`, `XAML:136-138` |
| «Seleccionar archivo…» | Guardar, sugerido `reducida`, `.txt` (+ «Todos los archivos»); defecto «(Sin archivo seleccionado)» | `VM:140-141,157-169`, `XAML:143-146` |
| Enlace al hilo del foro (`http://www.foro1x2.com/viewtopic.php?t=4445`) | abre navegador | `VM:239-254`, `XAML:161-162` |

**Acciones.** **Generar** (`VM:176-236`), síncrono en el hilo de UI. Mensajes de `Estado` (literales): «Es necesario indicar el archivo de salida», «La columna base no está incluida en el pronóstico», «No se permite mezcla de dobles y triples», «Nº de triples incorrecto», «Nº de dobles incorrecto», «Los signos base no forman parte del pronostico», «Longitud incorrecta», «La longitud de la columna es incorrecta», «Error al grabar las columnas: <msg>» (+ diálogo), «Se han grabado N columnas» y `EstadoSalida = "Fichero de salida: <nombre>"`.

**Lógica de negocio en el ViewModel** (matrices en `VM:96-118`, tal cual el legacy):

```
M4TR13  = [[1,1],[1,2]]
M13TR13 = [[0,0,1,1,1,1,1,1,1,1],[1,1,0,0,1,1,1,2,2,2],[1,2,1,2,0,1,2,0,1,2]]
M11TR12 = [[1,0,1,2,2,2],[1,1,0,1,1,2],[1,1,2,0,2,1],[1,2,1,1,0,1],[1,2,2,2,1,0]]
M7DR13  = [[0,1,1,1],[1,0,1,1],[1,1,0,1]]
M15DR13 = [[1,0,1,1,1,0,0,0,1,1,1],[1,1,0,1,1,0,1,1,0,0,1],[1,1,1,0,1,1,0,1,0,1,0],[1,1,1,1,0,1,1,0,1,0,0]]

Generar():
  ruta vacía -> error;  TestSignos() falso -> error        // para i<LongitudColumna: Pronóstico[i][Base[i]] debe estar marcado (VM:259-270)
  ContarDoblesYtriples: por partido 0..14: n = nº de toggles marcados; n==3 -> triples++, involucrado; n==2 -> dobles++, involucrado; otro (0 o 1) -> NO involucrado   (VM:286-300)
  triples>0 && dobles>0 -> error de mezcla
  dobles==0: triples ∈ {4,13,11} -> GeneraReduccion(M4TR13|M13TR13|M11TR12, triples=true) ; otro -> «Nº de triples incorrecto»
  dobles>0 : dobles ∈ {7,15}: TestSignosDobles() [cada involucrado tiene su base en el pronóstico] -> GeneraReduccion(M7DR13|M15DR13, false) ; otro -> «Nº de dobles incorrecto»

GeneraReduccion(M, esTriples):                                           (VM:303-395)
  r = filas(M) [signos dependientes];  c = columnas(M) [independientes];  nInv = c + r
  modulo = esTriples ? 3 : 2;  maxLen = Int(LongitudColumna); si nInv > maxLen -> «La longitud de la columna es incorrecta»
  maxCol = 3^(nInv - r) = 3^c
  abrir ruta (File.CreateText: UTF-8 sin BOM) ; ncol=0
  para i en 0 ..< maxCol:
      signos = [0]*maxLen ; suma = [0]*5 ; descartar = false
      para z en 0 ..< c:
          signos[z] = Convert.ToInt32( Double(i) / pow(3, z) ) % 3        // ¡redondeo al par más próximo, NO truncado! (ver peculiaridades)
          si !esTriples && signos[z] == 2 -> descartar = true; break
          para j en 0 ..< r: suma[j] += M[j][z] * signos[z]
      si descartar: continuar
      para j en 0 ..< r: signos[c + j] = suma[j] % modulo
      columna = ""; pv = 0
      para partido en 0..14:
          b = Base[partido]
          si involucrado[partido]:
              triples: columna += "1X2"[ (b + signos[pv++]) % 3 ]
              dobles : s0 = b ; s1 = (b+1)%3 ; si !Pronóstico[partido][s1] -> s1 = (s1+1)%3 ; columna += "1X2"[ [s0,s1][ signos[pv++] ] ]
          si no: columna += "1X2"[b]
      escribir columna.prefix(maxLen) ; ncol++
```

**Salidas.** Fichero `.txt` con `ncol` líneas de `maxLen` signos `1/X/2` (sin deduplicar); estado «Se han grabado N columnas».

**Peculiaridades a preservar (idénticas en el legacy, `UI/FrmReducidasPerfectas.cs:1543`).**
- **Extracción de dígitos por redondeo:** `Convert.ToInt32(i / Math.Pow(3,z)) % 3` redondea al par más próximo (banker's), no trunca: p. ej. i=2, z=1 ⇒ `ToInt32(0,667)=1`. Para z≥1 los «dígitos» no son los de la base 3 y pueden aparecer repetidos o faltar combinaciones. En Swift: `Int((Double(i)/pow(3,Double(z))).rounded(.toNearestOrEven)) % 3` (T4).
- Los partidos con 0 o 1 signo marcados no son «involucrados» y se fijan al signo base; la comprobación `TestSignos` exige, sin embargo, que el base esté marcado en los primeros `LongitudColumna` partidos (un partido con 0 signos marcados ⇒ error).
- `Generar` bloquea el hilo de UI (hasta 3^10 = 59 049 vueltas con 13 triples; 3^11 = 177 147 con 15 dobles); en Swift pasar a `async` sin cambiar el resultado.
- Los partidos 15 se evalúan siempre en `ContarDoblesYtriples` (`VM:290`), aunque `LongitudColumna` sea menor: los partidos posteriores a la longitud pueden contar como involucrados y consumen signos virtuales, y la columna se recorta después con `Substring(0, maxLen)` (`VM:380`). `TestSignos` solo comprueba los primeros `LongitudColumna` (`VM:262`). Con `NumberBox` vacío (`NaN`) el cast `(int)LongitudColumna` (`VM:313`) no lanza excepción: devuelve un valor negativo y el flujo acaba en «La longitud de la columna es incorrecta» (`VM:321-324`).
- Iteraciones por ejecución: 3^c con c = columnas de la matriz, es decir 9 (4 triples), 59 049 (13), 729 (11), 81 (7 dobles) y 177 147 (15 dobles), y en dobles se descartan las que tengan algún dígito 2.
- El manual (`docs/MANUAL_USUARIO.md:482`) la lista como «migración pendiente»; el VM sí implementa el cálculo completo.

**Casos de uso extraídos (CU-B22).** `ReduccionPerfecta.generar(base:pronostico:longitud:)` → `[String]` (incluye `MatricesPerfectas`, `contarDoblesYTriples`, `testSignos`), `ReduccionPerfecta.exportarTxt`.

**Área Mac propuesta.** *Reducción* → herramienta (hoja o vista secundaria del área) con la cuadrícula de 15 partidos. Propuesta; decide el dueño.

### Pantalla 56 · GEPTFrm — «Columna estadística de ABDON (GEPT)»

**Ficheros.** `VM` = `GEPTFrmViewModel.cs` (131), `XAML` = `GEPTFrmPage.xaml` (140), `CS` = `GEPTFrmPage.xaml.cs` (19, sin lógica). Legacy: `Free1X2/UI/GEPTFrm.cs` (`Calcular/Verificar/Pronosticar/Traductor`, citado en `VM:94,112`).

**Propósito.** Para cada partido el usuario indica el resultado de las **dos últimas veces** que se enfrentaron (G = ganó, E = empató, P = perdió) y la pantalla deriva un pronóstico de signos por una tabla fija (`VM:75-78`, `XAML:21`).

**Cómo se abre.** Menú Utilidades «Columnas GEPT…» (`MainWindow.xaml.cs:255`) y botón `bColumnasGEPT` (`:425`). Autónoma. (`docs/MANUAL_USUARIO.md:190,302` la describe como «generador de columnas probables»; el VM solo traduce, no genera ficheros.)

**Entradas.** 14 filas (`VariablesGlobales.NumeroPartidos`, `VM:86-89`): `Nº` (dos dígitos, `numero.ToString("00")`), «Última vez» y «Penúltima vez» (`ComboBox` con `"", G, E, P`, defecto vacío) (`VM:24,31-37`, `XAML:87-102`). Al cambiar una celda se recolorea: G `GreenYellow`, E `Yellow`, P `Pink`, vacío transparente (`VM:55-71`).

**Acciones.** **Calcular** (`VM:97-108`): por fila, reasegura los colores y `Resultado = Traductor(Primera, Segunda)`. Sin validación ni mensajes; una celda vacía produce `" "`.

**Lógica de negocio en el ViewModel** (`VM:114-129`):

```
Traductor(primera, segunda): clave = (primera + segunda).uppercased()
  GG->"12"  GE->"1X"  GP->"1"
  EG->"X2"  EE->"X"   EP->"1X"
  PG->"2"   PE->"X2"  PP->"12"
  otro (incl. vacíos) -> " "
```

**Salidas.** Texto de pronóstico por fila (`1`, `X`, `2`, `12`, `1X`, `X2`) y colores de celda. No se graba ni exporta nada.

**Peculiaridades a preservar.** Valores en minúscula se normalizan a mayúscula en la clave pero el color los acepta igual (`Trim().ToUpperInvariant()`, `VM:65`); la tabla no incluye `X` para `GG`/`PP` (no hay triples).

**Casos de uso extraídos (CU-B23).** `ColumnaGEPT.traducir(primera:segunda:) -> String`, `ColumnaGEPT.calcular(filas:)`.

**Área Mac propuesta.** *Columnas probables* → utilidad ligera (hoja/popover con tabla de 14 filas); podría integrarse como ayuda en el editor de CPs. Propuesta; decide el dueño.

### Pantalla 57 · GeneraPim — «Filtro Pim»

**Ficheros.** `VM` = `GeneraPimViewModel.cs` (880), `XAML` = `GeneraPimPage.xaml` (270), `CS` = `GeneraPimPage.xaml.cs` (18, sin lógica). Legacy citado por la página: `Free1X2/UI/FiltroPim.cs`, clase `GeneraPim` (`GeneraPimPage.xaml.cs:7`); **pero el VM dice portar `aidomnou`** (`Free1X2/UI/Aidomnou.cs`, `VM:15-26`). Ver «Divergencia» abajo: es el hallazgo más importante de esta pantalla.

**Propósito (según el port).** A partir de los valores 1/X/2 de los 14 partidos construye **6 «columnas probables»** (dobles/triples/fijos por ranking), y filtra un fichero de columnas aceptando las que tienen, para cada una de las 6 columnas, un nº de aciertos dentro de su rango, más rango de suma y de recorrido (`VM:15-26`, `XAML:16`).

**Cómo se abre.** Menú Filtros «Filtro Pim…» (`MainWindow.xaml.cs:241`) y botón `bFiltroPim` (`:412`). Autónoma.

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| Rejilla «Valores por partido» 14×3 (P01..P14; 1/X/2) | `NumberBox` ≥0, defecto 0 (clase `PartidoValores`) | `VM:112,128-134,865-879`, `XAML:48-67` |
| Rangos R1..R6, R7 (suma) y «Recorr.» | texto `min-max`; defectos `3-6`, `2-6`, `2-6`, `2-6`, `2-6`, `0-1`, `0-1`, recorrido `0-14` | `VM:29-53`, `XAML:98-131` |
| «Columna ganadora» | texto, defecto `COL.GANADORA` | `VM:56-57` |
| Límites del motor `lims = {145,11,9,8,65,9,6,5}` | **fijos en código** (no editables; `aidomnou.cfg` no se porta) | `VM:114-117` |

**Acciones.**
- **Calcular** (`VM:141-218`): `RecuperaPantalla()` y `PreparaColumnas()` ANTES del selector; Abrir `.txt` (`*`); en hilo de fondo lee líneas (`Peek()>0`): `Trim()`, `ctini++`; longitud <14 ⇒ marca error y corta; `x`/`X`→`4`; `Valida`; si pasa, `idx = S2n` y se marca en un `BitArray[4 782 969]` (cuenta solo nuevas). Fin: `Procesadas=ctini`, `Admitidas=ctadm`, `Tiempo` = `TimeSpan.ToString()+"0000000000"` recortado a 10 car. Errores: «error de longitud en una columna de entrada»; «No se pudo leer el fichero de columnas:\n<ruta>\n\n<msg>».
- **Grabar resultado** (`VM:225-267`): sin cálculo ⇒ «No hay resultado calculado. Pulsa Calcular primero.»; Guardar `resultado.txt`; recorre el `BitArray` en orden ascendente escribiendo `N2s(i,14)`.
- **Salvar / Recuperar rangos** (`VM:273-347`): fichero `.cnd` (sugerido `rangos`), 8 líneas `R1..R7, Recorrido`; Recuperar solo asigna las líneas presentes.
- **Exportar columnas** (`VM:353-396`): `RecuperaPantalla`, `PreparaColumnas`; Guardar `columnas.txt` («F.Salida»); 6 líneas (una por columna probable) con 14 códigos `Cambia` separados por coma.
- **Cargar ganadoras / − / +** (`VM:403-483`): `.txt`; `VerColumna` (`VM:802-814`; ≥14 car., primeros 14, alfabeto `1 2 x X`, si no «col.G. errónea»); máx. 3000 columnas (`string[3000]`, si no excepción capturada); muestra la última; −/+ recorren.
- **Analizar** (`VM:490-526`): ver pseudocódigo.

**Lógica de negocio en el ViewModel** (transcripción literal de `aidomnou`; los índices de columna son 0..5):

```
Cambia(v): 0 "-", 1 "1", 2 "2", 3 "12", 4 "X", 5 "1X", 6 "X2", 7 "1X2"       // bits: 1='1', 2='2', 4='X'   (VM:762-774)
signo->bit: índice 0('1')->1, 1('X')->4, 2('2')->2

RecuperaPantalla: nvals[i][0..2] = Valor1, ValorX, Valor2 de la fila i ; rks[0..7] = ParseRango(R1..R7, Recorrido)   // Int(aux[0]),Int(aux[1]); fallo => excepción SIN capturar (VM:534-558)

PreparaColumnas: cps[14][6]=0; PreCol1..PreCol6; scps[c] = concat_{p} dígito cps[p][c]    (VM:561-573)

PreCol1 (col 0, VM:576-610) / PreCol4 (col 3, VM:613-647):                     // PreCol1 con tope 42 vueltas; PreCol4 `while(true)`
  pv = copia de nvals
  repetir: elegir (p,s) con MAYOR pv (comparación >= desde nv=0 => en empate gana el ÚLTIMO recorrido p, s)  ; cps[p][col] += bit(s) ; pv[p][s] = -1
           puntos = Σ_p { cps∈{3,5,6} -> 10 ; cps==7 -> 15 }   ; parar si puntos >= lims[0] (col 0: 145)  /  lims[4] (col 3: 65)
PreCol2 (col 1, VM:650-689):  lims[1]=11 veces: p con mayor nvals[p][0] entre los que cps[p][1] < 1 (>=, gana el último) -> cps += 1
                  lims[3]=8  veces: p con mayor nvals[p][2] (estricto >) entre cps[p][1] < 2  -> cps += 2
                  lims[2]=9  veces: p con mayor nvals[p][1] (estricto >) entre cps[p][1] < 4  -> cps += 4
                  (si ningún candidato supera 0, nq queda en 0 y se suma al partido 0)
PreCol5 (col 4, VM:692-731):  igual con lims[5]=9 unos (>=), lims[7]=5 doses (>), lims[6]=6 equis (>)
PreCol3 (col 2, VM:748-759) «14 dobles»: por partido, signo de MENOR valor (estricto <, init 100): '1'->6 (X2), 'X'->3 (12), '2'->5 (1X)
PreCol6 (col 5, VM:734-745) «14 fijos»:  por partido, signo de MAYOR valor (estricto >, init 0, vacío -> '1'): 1 / 4 / 2

Valida(columna) (columna con X->'4'):                                      (VM:777-799)
  para c en 0..5: na = #{ i<14 : scps[c][i] != '0' && (scps[c][i] & columna[i]) > '0' }  [≡ (dígito & bit del signo) != 0]
        si na fuera de [rks[c].min, rks[c].max] -> false ; suma += na ; min/max
  suma ∈ rks[6] y (max-min) ∈ rks[7]  -> true

Analizar: columna = ColumnaGanadora con x,X -> '4' ; Analisis1..6 = na por columna ; Analisis7 = suma ; Recorrido = max-min   (sin comprobar longitud)
S2n(s,14) (VM:817-835): base 3, '1'->1, '2'->2, cualquier otro->0 (partido 1 = dígito más significativo) ; N2s(n,14) (VM:838-858): dígitos (0->'X', 1->'1', 2->'2'), partido 1 primero
```

**Salidas.** Contadores (enteros), tiempo `hh:mm:ss.f`, 8 contadores de análisis, ficheros `.txt`/`.cnd`. Orden de salida del resultado: por `S2n` ascendente (`X < 1 < 2` posición a posición), no el del fichero de entrada.

**Divergencia con el legacy `GeneraPim` (`Free1X2/UI/FiltroPim.cs`) — verificada.** La clase WinForms «Filtro Pim» es otra cosa: genera **7** columnas (`RecuperaPantalla`, `FiltroPim.cs:221-248`: 7 rondas × 6 elecciones del mayor valor restante, estricto `>`, marcando `-1`), sin PreCol1..6 ni límites; `Valida` con 7 columnas y recorrido, **sin suma** (`:448-466`); deduplica con `repes` (BitArray) **antes** de validar y guarda las aceptadas **en orden de entrada** (`:413-446`, `:386-404`); rangos en `.jb7` de 8 líneas (`:467-508`); analiza 7 columnas (`:509-542`). El port WinUI conserva los **defectos de rangos** del legacy (`FiltroPim.cs:832,1481,1493,1505,1517,1783,2122,2206` ⇒ `3-6`, `2-6`×4, `0-1`, `0-1`, reco `0-14`) y las etiquetas R1..R7, pero **ejecuta el motor de `aidomnou`** (6 columnas + suma + límites). `aidomnouViewModel.cs` contiene el mismo motor (16 menciones a `PreCol`, ídem `GeneraPimViewModel.cs`). Ver A-57-1.

**Peculiaridades a preservar (si se sigue el port).** `Analizar` con la columna por defecto de 12 caracteres lanza `IndexOutOfRange` al acceder al índice 12 (`VM:494-511`), sin captura; rangos mal escritos lanzan sin captura (`VM:553-558`); `Valida` usa `min6` inicial 84 (`VM:780`; `Analizar` usa 99, `VM:496`).

**Casos de uso extraídos (CU-B24, CU-B25).** `FiltroPim` (decidir variante): `.prepararColumnas(valores:)`, `.filtrarFichero(url:rangos:)`, `.analizarGanadora`, `.exportarColumnas`, `RangosPim.guardar/leer`; helpers `Cambia`, `S2n`, `N2s`.

**Área Mac propuesta.** *Herramientas* (grupo «Filtros de ficheros»: Combinar, Diferencias, Coincidencias, Aidomnou, Pim) → vista principal con tarjetas Valores/Rangos/Cálculo/Análisis. Propuesta; decide el dueño.

### Pantalla 58 · GeneradorCPSDiferencias — «Generador de CPs por diferencias»

**Ficheros.** `VM` = `GeneradorCPSDiferenciasViewModel.cs` (333), `XAML` = `GeneradorCPSDiferenciasPage.xaml` (152), `CS` = `GeneradorCPSDiferenciasPage.xaml.cs` (20, sin lógica). Legacy: `Free1X2/UI/GeneradorCPSDiferencias.cs` (`button2_Click` :306 con la llamada a `combinarDobles1` en :338, definición :426; el VM declara copia literal, `VM:15-29,164`).

**Propósito.** Dadas una columna inicial y otra «alternativa» (14 signos), genera columnas probables formadas por la inicial con 1 o 2 posiciones sustituidas por la alternativa, como «fijos» o como «dobles», y las graba (`XAML:21`, `docs/MANUAL_USUARIO.md:302`).

**Cómo se abre.** Desde «Generar CPs» (`GenerarCPsViewModel.cs:215`, pantalla 59) y, en el legacy, desde el botón «CPs por diferencias…» de DifCols (en el port ese botón no navega, A-41-3). Sin menú propio.

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| Columna inicial / alternativa | texto 14, mayúsculas; alfabeto `1 X 2`, longitud exacta 14 | `VM:32-36,166-182`, `XAML:40-55` |
| «Tipo de columna» | combo `Fijos`/`Dobles`, defecto Dobles (índice 1) | `VM:41-43,53` |
| «Número de diferencias» | combo `1`/`2`, defecto 2 (índice 1) | `VM:45-47,55` |
| Archivo destino | «Examinar…» (Guardar, sugerido `doc1`, `.txt`); campo de solo lectura | `VM:61-71`, `XAML:112-122` |

**Acciones.** **Generar** (`VM:79-152`): valida en orden con mensajes (estado + diálogo): «La cadena inicial no es válida» / «La cadena alternativa no es válida» / «No se ha indicado el fichero.»; genera según tipo y nº de diferencias; escribe cada fila con `ArchivoColumnasTexto.GuardarColsComa` (la línea conserva sus comas) y `Cerrar()`; éxito ⇒ «Columnas creadas»; error ⇒ «No se pudieron generar las columnas en:\n<ruta>\n\n<msg>». **Cancelar**: solo limpia el texto de estado (`VM:159-162`).

**Lógica de negocio en el ViewModel** (todo, `VM:185-332`; `c1` inicial, `c2` alternativa, posiciones 0..13):

```
CombinarFijos1(c1,c2) -> 15 filas:      fila i (i=0..14): c1 con la posición i sustituida por c2[i]  (para i=14 no se sustituye nada => fila = c1)   cada signo separado por ","
CombinarFijos2(c1,c2) -> 92 filas:      fila 0 = c1 ; después, para i en 0..14, j en i+1..13 (91 pares, i=14 no genera): c1 con las posiciones i y j sustituidas por c2
cM(c1,c2) = concatenación, por posición i=0..13, de los signos de "1X2" que NO son c1[i] ni c2[i]
            // 1 carácter si c1[i]!=c2[i]; 2 caracteres si c1[i]==c2[i] (¡desalinea cM!, ver peculiaridades)
CombinarDobles1: como Fijos1 pero cada posición j escribe  (c2[j] o c1[j]) + cM[j]   -> "ab" por partido
CombinarDobles2: como Fijos2 con la misma regla de dobles ((c2 o c1)[k] + cM[k])
salida: cada fila = join(",", elementos) sin coma final
```

**Salidas.** Fichero `.txt` (UTF-8 sin BOM, `WriteLine`) con 15 o 92 líneas. Ejemplos: Fijos → `1,X,2,…`; Dobles → `1X,2X,…`. Sin formato numérico.

**Peculiaridades a preservar.**
- `cM` se indexa por posición (`cM.Substring(j,1)`) pero acumula 1 o 2 caracteres por posición: si `c1[i]==c2[i]` en algún partido, **todas las posiciones siguientes quedan desalineadas** y el «tercer signo» deja de corresponder a su partido (`VM:251-260,268,272`). Replicar tal cual (idéntico en legacy).
- Con `c1[i]==c2[i]` la fila «sustituida» es igual a la inicial (filas duplicadas; no se deduplican).
- La fila final de `*1` (i=14) es la inicial sin cambios; en `*2` la inicial va primero (`VM:187-204,210-218`).
- Sobrescribe el destino sin avisar.

**Casos de uso extraídos (CU-B26).** `CPsPorDiferencias.generar(inicial:alternativa:tipo:diferencias:) -> [String]`, `CPsPorDiferencias.exportar(_:url:)`.

**Área Mac propuesta.** *Columnas probables* → hoja modal pequeña abierta desde el Generador de CPs. Propuesta; decide el dueño.

### Pantalla 59 · GenerarCPs — «Generador de Columnas Probables»

**Ficheros.** `VM` = `GenerarCPsViewModel.cs` (301), `XAML` = `GenerarCPsPage.xaml` (150), `CS` = `GenerarCPsPage.xaml.cs` (19, solo cablea `Navegar`). Motor: `Domain/EntradaSalida/GenerarCPs/{CPs,ColumnasProbables,DatosHelper,IO,Valoracion}.cs` (ESPEC_04). Legacy: `Free1X2/UI/GenerarCPs.cs` (`btnOK_Click`, `btnImportarVal_Click` :271, `PonerValoracionPantalla` :295, citado en `VM:157-171`).

**Propósito.** Con la valoración 1/X/2 (en %) de los 14 partidos genera, para una jornada, **un fichero de columnas probables por cada «tipo de CP» configurado** (`ColumnasProbables.xml`) (`XAML:16`, `docs/MANUAL_USUARIO.md:288-300`).

**Cómo se abre.** Menú Utilidades «Generador CP…» (`MainWindow.xaml.cs:254`) y botón `bGeneradorCPs` (`:419`). Salidas: «Configurar columnas» → `ConfigCPsFrmPage` (`VM:192-196`), «Separador de porcentajes» → `FiltroPorcenJBPage` (`:202-206`, pantalla 50), «CPs por diferencias» → `GeneradorCPSDiferenciasPage` (`:212-216`, pantalla 58).

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| Rejilla de valoración: 14 filas (`Partido`, `1`, `X`, `2`) | `NumberBox` double ≥0 sin máximo, defecto 0 | `VM:19-40,56-63`, `XAML:65-84` |
| «Importar %» | `.txt`/`*`: `Porcentajes(ruta).valores` (detecta formato 1/3/42/43/44 valores por fila) → vuelca `valores[i,0..2]` a la rejilla | `VM:149-186`, `Domain/Utils/Porcentajes.cs` |
| «Jornada» | `NumberBox` Min 0, defecto 0; debe ser >0 | `VM:72-73,86-92`, `XAML:105-109` |
| «Generar (OK)» / botones de herramientas | — | `XAML:111-143` |

**Acciones.**
- **Generar (OK)** (`VM:84-142`): `jornada = (int)Jornada`; si ≤0 ⇒ «Especifique un número de jornada» (estado + diálogo). `CopiarValoracion()` (array `Valoracion[14]{Uno,Equis,Dos}`). En segundo plano: `DatosHelper().ObtenerDatos()` lee `<exe>/ColumnasProbables.xml` (si no existe, DataSet vacío); crea `<exe>/Condiciones/`; **por cada fila de la tabla «Tipos de CPs»**: filtra la tabla «Configuracion de CPs» con `Tipo = <Tipo>`, `CPs.CrearCPs(dv, valores)`, serializa y escribe `Condiciones/<Nombre>_j<jornada>.txt` con `IO.GuardarTexto`. Éxito ⇒ «Fichero(s) guardado(s) correctamente en la carpeta Condiciones (N)» (siempre, aunque N=0); error ⇒ «Error al generar las columnas: <msg>».
- **Importar %**: errores ⇒ «Error al importar la valoración: <msg>»; éxito ⇒ estado «Valoración importada de <fichero>.».

**Lógica de negocio en el ViewModel.**

```
LlenarDataset(matriz string[14][max]) -> tabla 14 filas × max columnas   // celdas nulas -> " "                              (VM:248-276)
LlenarTxtColumnas: para cada columna i (0..max-1): línea = join(",", [tabla[j][i] for j in 0..13]) + "\r\n"              (VM:279-300)
    // es decir: UNA LÍNEA POR CONFIGURACIÓN DE CP, 14 pronósticos separados por coma; cada celda es "1","X","12","X2","1X2", "" o " "
nombre de fichero = Nombre + "_j" + jornada + ".txt" ; el Replace(" ","_") del legacy no se asigna -> los espacios del nombre se CONSERVAN   (VM:124-125)
GuardarTexto(txt) usa WriteLine -> el fichero termina con una línea en blanco adicional (txt ya acababa en "\r\n")           (Domain IO.cs:23-30)
```

Reglas de `CPs.CrearCPs(dv, valor)` (motor, para los casos de uso; `Domain/EntradaSalida/GenerarCPs/CPs.cs:20-92`): por cada fila de configuración `j` con columnas `Desde, Hasta, ForzarFijos, NumFijos, ForzarDobles, NumDobles, ForzarTriples`: tipo = 1 si ForzarFijos, 2 si ForzarDobles (los dobles prevalecen), 3 libre. Libre: para cada partido, `"1"` si `Uno ∈ [desde,hasta]`, `"X"` si `Equis ∈ …`, `"2"` si `Dos ∈ …`, concatenados; si queda vacío y `ForzarTriples` ⇒ `"1X2"`. Fijos/Dobles con `max==14`: `ObtenerTodosFijos` (estricto mayor; empates ⇒ celda nula) / `ObtenerTodosDobles` (doble = los dos mayores: `X2` si `1` es el menor, `12` si `X`, `1X` si `2`); con `max<14`: se ordenan los 42 valores, se toman los `max` mayores (fijos) o menores (dobles), buscando el partido por igualdad de valor (`OrdenarValoracion`, `Array.Sort` ascendente; fijos usan `Array.Reverse`); en empates de valor gana el primer partido en orden de índice (T5).

**Salidas.** Ficheros `Condiciones/<Nombre>_j<N>.txt` (UTF-8 sin BOM, `\r\n`). Estado textual en pantalla.

**Peculiaridades a preservar.** Rejilla por defecto a 0 ⇒ en modo libre, un rango que contiene 0 acepta todos los signos; el mensaje de éxito no distingue N=0; la carpeta `Condiciones` se crea bajo el directorio del ejecutable (en Mac: `~/Library/Application Support/Free1X2/Condiciones`, M-R3/§2 del plan).

**Casos de uso extraídos (CU-B27).** `GeneradorCP.generar(valoracion:jornada:configuracion:) -> [FicheroCP]`, `GeneradorCP.importarPorcentajes(url:) -> [[Double]]` (reutiliza `Porcentajes`), `GeneradorCP.serializar(_:)`.

**Área Mac propuesta.** *Columnas probables* → vista principal del área (formulario de valoración + generación), con «Configurar columnas» como inspector/hoja y las otras dos herramientas como hojas. Propuesta; decide el dueño.

### Pantalla 60 · GestorEquiposFrm — «Gestión de equipos»

**Ficheros.** `VM` = `GestorEquiposFrmViewModel.cs` (372), `XAML` = `GestorEquiposFrmPage.xaml` (174), `CS` = `GestorEquiposFrmPage.xaml.cs` (24, solo cablea `Navegar`). Legacy: `Free1X2/UI/GestorEquiposFrm.cs` (`btnNuevoEquipo_Click` :120-124, citado en `VM:105`). Servicio online: `Services/QuinielaOnlineService.cs`, `Services/PaisesOnline.cs`, `Domain/Online/CatalogoEquipos.cs`.

**Propósito.** Mantiene el catálogo de equipos que se ofrece al rellenar el boleto, repartido en 4 categorías (1ª, 2ª, 2ªB, Internacionales): mover entre categorías, eliminar, dar de alta, importar el catálogo online y guardar (`VM:19-30`, `docs/MANUAL_USUARIO.md:436-447`).

**Cómo se abre.** Menú Archivo «Gestión de equipos…» (`MainWindow.xaml.cs:179`) y botón `bGestorEquipos` de la barra Archivo (`:375`). Sale a `AgregarEquipoFrmPage` («Formulario de alta…», `VM:111-114`).

**Entradas.**

| Control | Tipo / defecto | Evidencia |
|---|---|---|
| 4 listas (1ª, 2ª, 2ªB, Int), selección simple, cargadas de `<exe>/Equipos/equipos1.dat`, `equipos2.dat`, `equipos2b.dat`, `equiposInt.dat` (una línea = un equipo, **Latin-1**) | strings sin ordenar | `VM:34-46,116-123,230-251` |
| «Nombre» + «Categoría» (`1ª`,`2ª`,`2ªB`,`Int`, defecto `1ª`) + «Nuevo equipo» | alta rápida | `VM:62-69,288-311`, `XAML:21-33` |
| «País (online)» (`España`/`México`, defecto España) + «Importar equipos online» | usa la misma «Categoría» como destino | `VM:80-89,141-189`, `XAML:47-57` |
| Botones ▶◀ entre 1ª↔2ª y 2ª↔2ªB, «Eliminar» por lista, «Guardar archivos» | — | `VM:254-280,314-331`, `XAML:105-167` |

**Acciones.**
- **Carga** (constructor): por categoría, lee el fichero línea a línea con `Encoding.Latin1`; error ⇒ `Estado = "No se pudo cargar <fichero>: <msg>"` y lista vacía.
- **Nuevo equipo** (`VM:288-311`): nombre en blanco ⇒ `Estado = "Introduce un nombre de equipo."`; `destino = lista(NuevaCategoria)`; si no lo contiene (comparación `OrdinalIgnoreCase` sobre `Trim()`), **añade el nombre tal cual lo escribió** (sin `Trim`) y `Estado = Equipo "<n>" añadido a <cat>.`; si existe ⇒ `El equipo "<n>" ya existe en <cat>.`; vacía el campo.
- **Mover** (`VM:357-364`): sin selección ⇒ «Selecciona un equipo.»; si el destino no lo contiene (mismo criterio) lo añade al final; **siempre** lo quita del origen (`Remove` del primer elemento igual); `Estado = Movido "<n>".`
- **Eliminar** (`VM:366-371`): `Eliminado "<n>".` o «Selecciona un equipo.».
- **Importar equipos online** (`VM:141-189`): ver pseudocódigo; no escribe en disco.
- **Guardar archivos** (`VM:315-348`): para cada categoría, crea `Equipos/` si falta y reescribe el `.dat` completo con `StreamWriter(…, Latin1)` + `WriteLine` por equipo; éxito ⇒ «Equipos guardados.»; error ⇒ `Error al guardar: <msg>` + diálogo «No se pudieron guardar los equipos: <msg>».

**Lógica de negocio en el ViewModel.**

```
clave(n) = (n ?? "").trim()          // criterio ÚNICO de duplicados: OrdinalIgnoreCase sobre clave()      (VM:206-223)
ImportarOnline:                                                          (VM:141-189)
   si ya importando -> return ; pais = PaisSeleccionado.codigo ("es"|"mx"); Importando=true ; Estado="Conectando con el servicio online…"
   catalogo = QuinielaOnlineService.ObtenerEquiposAsync(pais)      // GET {base}/wp-json/clubprogol/v1/equipos/{pais} (docs/API_CLUBPROGOL.md)
   destino = lista(NuevaCategoria) ; yaEstan = set{ clave(e) | e ∈ destino }
   para equipo en catalogo.EquiposPlano():          // trim + dedup OrdinalIgnoreCase, ignora vacíos, orden de aparición (Domain CatalogoEquipos.cs:35-52)
        si yaEstan.add(clave(equipo)): destino.append(equipo) ; importados++
   Estado = importados>0 ? "N equipos importados (online) a <cat>. Revisa y pulsa «Guardar archivos»." : "El catálogo online no añadió equipos nuevos a <cat> (ya estaban todos)."
   error (no cancelación): log ; Estado = (QuinielaOnlineException ? ex.Message : "No se pudo importar el catálogo: " + ex.Message) + " Los equipos no se modificaron."
   finally Importando=false
```

**Salidas.** Listas y `Estado` (textos arriba); ficheros `equipos*.dat`.

**Peculiaridades a preservar / avisos.**
- Los `.dat` se leen y escriben en **Latin-1**; caracteres fuera de Latin-1 (p. ej. nombres importados de México) se perderían (`?`). Decisión de codificación en Mac: A-60-1.
- El alta rápida **no recorta** el nombre guardado, pero sí al comparar (`VM:302`).
- Mover un equipo que ya existe en el destino lo elimina del origen sin duplicar (`VM:361-362`).
- Las listas no se ordenan; el orden de guardado es el de pantalla.
- Hay dos altas: la rápida (`NuevoEquipo`) y la pantalla `AgregarEquipoFrmPage` (otro alcance), que persiste su propio `.dat`.
- Ruta de datos: junto al ejecutable (`DirEquipos`, `VM:34-40`); en Mac, `~/Library/Application Support/Free1X2/Equipos` con semilla en el bundle.

**Casos de uso extraídos (CU-B28).** `CatalogoEquiposLocal.cargar()/guardar()`, `CatalogoEquiposLocal.alta(nombre:categoria:)`, `.mover(equipo:de:a:)`, `.eliminar`, `CatalogoEquiposLocal.importarOnline(pais:destino:)` (usa `Free1X2Online`).

**Área Mac propuesta.** *Boleto* (datos del boleto) → hoja modal «Equipos» o panel de preferencias de datos. Propuesta; decide el dueño.

### Pantalla 61 · GraficoColumnasFrm — «Representación gráfica de la combinación»

**Ficheros.** `VM` = `GraficoColumnasFrmViewModel.cs` (261), `XAML` = `GraficoColumnasFrmPage.xaml` (175), `CS` = `GraficoColumnasFrmPage.xaml.cs` (202; dibujo con `Canvas`, copia de imagen y guardado PNG). Legacy: `Free1X2/UI/GraficoColumnasFrm.cs` (`GraficoColumnasFrm_Paint` :220-301, `btnGuia_Click`). Lectura: `Domain/EntradaSalida/ArchivoColumnasTexto.LeerTodasColsANumero`.

**Propósito.** Abre un fichero de columnas y dibuja la combinación como un «código de barras» sobre **10 franjas** que cubren todo el rango 0..3^14−1 de columnas (cada apuesta = una línea vertical); muestra el mínimo y el máximo y una «guía» de qué columna corresponde al inicio de cada franja (`VM:12-22`, `XAML:21`).

**Cómo se abre.** Menú Ver «Gráfico de columnas…» (`MainWindow.xaml.cs:189`) y menú Combinación «Gráfico de columnas…» (`:229`). No hay botón de barra (el `bAnalisisGrafico` está `Visible=false` en el original, `MainWindow.xaml.cs:402`). Autónoma.

**Entradas.** «Abrir combinación» (diálogo Abrir `.txt`/`*`, DocumentsLibrary; legacy: carpeta `Columnas\`), «Cancelar» (corta el bucle de líneas, `VM:190-194`), «Copiar imagen» y «Guía» (habilitados solo con combinación representada, `XAML:110,115`). La caja de ruta es editable pero no se usa para abrir (`XAML:59`).

**Acciones.**
- **Abrir combinación** (`VM:87-125`): `_para=false`, estado «Procesando...»; `ComputarGrafico(ruta)` en segundo plano; publica `Minimo`, `Maximo` (enteros sin formato), líneas y rectángulos; `EstadoTexto = "{n} apuestas representadas (mín. {min}, máx. {max})."`; error ⇒ «Error al abrir la combinación.» + diálogo «No se ha podido representar la combinación: <msg>».
- **Copiar imagen** (`VM:203-211`, `CS:63-119`): sin combinación ⇒ «No hay ninguna combinación representada que copiar.»; si no, renderiza el `Canvas` a bitmap, lo copia al portapapeles como imagen y ofrece guardarlo como PNG (`Guardar`, `PicturesLibrary`, sugerido `grafico_columnas`). Estado: «Imagen del gráfico copiada al portapapeles.» / «Imagen del gráfico guardada en <nombre>.». (Función **nueva** del port; el legacy no la tenía, `VM:196-201`.)
- **Guía** (`VM:218-229`).

**Lógica de negocio en el ViewModel** (`VM:131-185`, `VM:218-260`):

```
ComputarGrafico(ruta):
  m = LeerTodasColsANumero(ruta)          // [Int32]: índice base 3 de cada columna, '1'->0,'X'->1,'2'->2, partido 1 = dígito menos significativo (ConvertidorDeBases)
  n = m.count ; si n==0 -> líneas=[], min=max=0
  ordenar m ascendente ; minimo = m.first ; maximo = m.last ; dif = maximo - minimo
  si dif < 9566:                                                   // «combinación estrecha»
        escala = 1 ; altoRel = Int16( (dif*10) / 9580 )           // división entera
        rellenos += Rect(x: maximo+1, y: altoRel*25+50, w: 958-maximo, h: 25)      // usa «maximo» ABSOLUTO (no dif); puede salir w<=0
        para i en altoRel+1 ..< 10: rellenos += Rect(x: 9, y: i*25+50, w: 958, h: 25)
  si no: escala = dif / 9565.938
  para cada m[i] (parando si se pulsó Cancelar):
        num   = Double(m[i] - minimo) / escala
        alto  = Int16( (num/958) - 0.5 )        // Convert.ToInt16(double) = REDONDEO AL PAR (ToEven), no floor
        num  -= alto*958
        ancho = Int16( num )                    // idem redondeo al par
        línea = (x: ancho+10, y0: alto*25+51, y1: alto*25+74)
  (Int16 lanza OverflowException si |valor| > 32767; aquí no ocurre salvo datos corruptos)

Guía: dif = maximo - minimo ; para i=1..10:  v = (i-1) * (dif/10) + minimo     // división entera
      s = base3(v) con 14 dígitos (MSD primero) ; s = s.replace("1","X") luego s.replace("0","1") ; línea = "%02d)  " + s     // 0->'1', 1->'X', 2->'2'
      (el primer carácter de la cadena es el partido 14; ver peculiaridades)
```

**Salidas.** Lienzo de 977×309 px (10 franjas de 959×25 con marco desde x=9, y=50+25·k; líneas verticales de 1 px; rellenos granate `#800000`), `Mínimo`/`Máximo` (enteros), 10 líneas de guía `NN)  <14 signos>`, estado textual (`CS:31-39`, `CS:130-193`). Colores del port: marco `AppBorderBrush`, línea `AppAccentBrush` (el legacy usaba `Maroon` 1 px y `BlueViolet` 2 px).

**Peculiaridades a preservar.**
- El redondeo al par de `Convert.ToInt16(x-0,5)` produce, para `num/958` entero, `alto = x` si x es par y `x-1` si es impar (con `ancho` 0 o 958): replicar con `.rounded(.toNearestOrEven)` (T4).
- La guía muestra el número en base 3 con el **partido 14 a la izquierda** (el entero es base 3 con el partido 1 como dígito menos significativo; `DeBase10` imprime el dígito más significativo primero, `VM:232-260`).
- `Cancelar` solo detiene el bucle de líneas; el estado sigue anunciando todas las apuestas (`VM:114`).
- El rectángulo de relleno usa `maximo` absoluto: `958 - maximo` es ≤0 si `maximo ≥ 958`, y el port lo descarta al dibujar (`CS:166`); el legacy llamaba a `FillRectangle` con ancho negativo (sin efecto visible).

**Casos de uso extraídos (CU-B29).** `GraficoCombinacion.calcular(indices:) -> (lineas, rellenos, minimo, maximo)`, `GraficoCombinacion.guia(minimo:maximo:) -> [String]`, `GraficoCombinacion.exportarPNG` (UI).

**Área Mac propuesta.** *Análisis* → vista principal (gráfico con `Canvas`/Swift Charts personalizado) con exportar imagen; guía como inspector. Propuesta; decide el dueño.

### Pantalla 62 · GruposEquiposFrm — Condición «Grupos de equipos»

**Ficheros.** `VM` = `GruposEquiposFrmViewModel.cs` (1128), `XAML` = `GruposEquiposFrmPage.xaml` (298), `CS` = `GruposEquiposFrmPage.xaml.cs` (39). **No** hereda de `FiltroArchivoViewModelBase` (tiene sus propios Guardar/Abrir/Copiar/Pegar). Legacy: `Free1X2/UI/Filtros/GruposEquiposFrm.cs` (líneas citadas en cada método del VM; `abrir`/`guardar`/`BOk`/`BAbrir` :1507-1572). Motor: `FiltroGruposEquipos`, `GrupoEquipos`, `RelacionGE1` (ESPEC_03/04).

**Propósito.** Condición «futbolística»: se marcan equipos del boleto (casa y/o fuera de cada uno de los 14 partidos) y se exige un nº o rango de **victorias, empates, derrotas y suma de puntos (3-1-0)** de ese conjunto; varios «grupos» y, en la pestaña Relaciones, sumas conjuntas de varios grupos (`VM:49-69`, `docs/MANUAL_USUARIO.md:228-232`).

**Cómo se abre.** Rejilla de Inicio, «Grupos Equipos» (`MainPageViewModel.cs:162`, `Filtro.GruposEquipos`) con `GrupoEnEdicion`. El constructor ya llama `CargarDesdeGrupo()` (`VM:72-85`); la página recarga al entrar salvo al volver de Estadísticas (`CS:28-38`).

**Entradas.**

| Control | Tipo / defecto / validación | Evidencia |
|---|---|---|
| Pestaña «Grupos Equipos»: 14 filas con dos `ToggleButton` (equipo de casa = signo 1, de fuera = signo 2; ambos = X) | nombres desde `AppState.JornadaActual` (`Partido.Local/Visitante`) o, sin jornada, `"Equipo casa N"` / `"Equipo fuera N"` | `VM:27-47,74-81,264-275`, `XAML:85-95` |
| «Victorias», «Empates», «Derrotas», «Suma Puntos» | texto: enteros/rangos/listas (`SonTodosNumeros`), vacío permitido pero no los cuatro vacíos | `VM:131-142,366-379`, `XAML:122-150` |
| Navegación de grupos ◄ ► «Eliminar actual», contador `n/total` | ► solo si el grupo en pantalla tiene algún equipo marcado | `VM:144-154,729-771` |
| Pestaña «Relaciones»: «Grupos Equipos» (índices, p. ej. `1-2`), «Suma Victorias/Empates/Derrotas/Puntos» | texto; la relación solo se guarda si tiene `GruposEquipos` y al menos una suma, todas numéricas | `VM:156-170,611-624`, `XAML:208-244` |
| Navegación de relaciones ◄ ► «Eliminar actual» | ► solo si `TieneRelacion1Datos()` | `VM:593-605,775-818` |

**Acciones.** **Aceptar** (`VM:823-842`), **Cancelar** (solo vuelve), **Borrar** (`VM:845-862`: `GuardarDatos()`, `_filtroGE = new FiltroGruposEquipos()` **desligado del grupo** hasta que se pulse Aceptar, recarga; sin confirmación, el legacy la pedía solo si `ContieneDatos`, `UI/Filtros/GruposEquiposFrm.cs:1574-1587`), **Estadísticas** (`VM:1115-1127`, `ObtenerFiltroTemporal` `VM:916-993`: filtro temporal → `CalculadorEstadisticas.EstadisticasFiltro(filtro, <exe>/Ganadoras/)` → `VisorEstadisticasPage`), **Guardar** (`VM:1032-1052`: `GuardarDatos()` + diálogo `.geq`/`.xml`, sugerido `Grupos de equipos`; `GuardarEn` `VM:878-889`: si hay grupos fuerza `ContieneDatos=IsActive=true` en el filtro **real** y llama a `ArchivoCondiciones.GuardaArchivo`), **Abrir** (`VM:1054-1074`: `GuardarDatos()` + diálogo `.geq`/`.xml`; `AbrirDesde` `VM:896-909`), **Copiar/Pegar** (`VM:1076-1113`; `Temp/tmp.geq`, `VM:123-124`; Pegar no hace nada si el temporal no existe), todos con `Estado` («Condición guardada en <f>», «Condición abierta de <f>», «Condición copiada», «Condición pegada», «Datos del filtro borrados», «Hay errores en la entrada de datos del grupo»).

**Lógica de negocio en el ViewModel** (modelo de «copia de trabajo»: `_arrayGE: [GrupoEquipos]`, `_arrayRelaciones1: [RelacionGE1]`, índices `_noGEPantalla`, `_relGE1Pantalla`, 0-based):

```
Código de selección por partido i (char):  '0' ninguno, '1' casa, '2' fuera, '3' ambos        // '0'+ (casa?1:0) + (fuera?2:0)   (VM:330-344)
GrupoEquipos.Pronosticos = char[14] ; Victorias/Empates/Derrotas/SumaPuntos = string

CargarDesdeGrupo: _grupo = AppState.GrupoEnEdicion ; _filtroGE = grupo.GetFiltro("GruposEquipos")
    _arrayGE = clon de filtro.GruposEquipos (campos: Pronosticos, SumaPuntos, Victorias, Empates, Derrotas)
    _arrayRelaciones1 = clon de filtro.RelacionesGE1.Relaciones (GruposEquipos, SumaVictorias, SumaEmpates, SumaDerrotas, SumaPuntos)
    índices=0 ; pintar ; LlenaEquipos()

GuardaDatosGE(ge):                                       // pantalla -> ge
    ge.Pronosticos = selección ; ge.CalcularLongPronosticos()
    si hay algún pronóstico distinto de '\0' y '0':
        si SonEntradasValidas(): copiar V/E/D/SumaPuntos   sino Estado = «Hay errores en la entrada de datos del grupo»  (NO copia)   (VM:403-421)
GuardarGEActual: si _noGE < count -> GuardaDatosGE(_arrayGE[_noGE]) ; si no y TieneGEDatos() -> añadir nuevo y GuardaDatosGE
CambiaGESelecionado(n): GuardarGEActual ; _noGE = n ; si count < n+1 añadir GrupoEquipos() vacío ; pintar n
EliminarGrupo: si _noGE==0 { si count>0 RemoveAt(0) } sino { RemoveAt(_noGE); _noGE-- } ; si count==0 añadir uno vacío ; pintar   (VM:749-771)
GuardarDatos (copia de trabajo -> filtro REAL):                                                       (VM:483-526)
    GuardarGEActual
    si count>0 { si el ÚLTIMO no tiene ningún carácter '1','2','3' -> RemoveAt(último) } sino _filtroGE.IsActive=false
    si !filtro.ContieneDatos: si count>0 { ContieneDatos=true; IsActive=true } sino IsActive=false
    sino { ContieneDatos=true ; IsActive=true }         // ¡tras tener datos queda activo siempre!
    para cada ge: CalcularLongPronosticos ; filtro.GruposEquipos = _arrayGE      // MISMA lista (alias)
GuardarDatosRelacionesGE1 (VM:702-724): GuardarRelGE1Actual ; si count>0 y la última sin GruposEquipos o sin ninguna suma -> RemoveAt(última)
    filtro.RelacionesGE1.Relaciones = [ r | r.GruposEquipos != "" ]
Aceptar: GuardarDatos() ; GuardarDatosRelacionesGE1() ; _grupo.ActivaFiltro(_filtroGE) ; NotificarCambio ; volver   // ActivaFiltro reemplaza el filtro del mismo nombre en el grupo (Domain Grupo.cs:518-532)
```

**Salidas.** Contadores `n/total` y textos; el efecto real es la lista de `GrupoEquipos`/`RelacionGE1` del filtro del grupo.

**Peculiaridades a preservar / divergencias.**
- **Divergencia con el legacy tras «Abrir»:** `AbrirDesde` reasigna `_grupo` y `_filtroGE` al grupo leído del fichero (`VM:896-909`, igual que `UI/Filtros/GruposEquiposFrm.cs:1546-1560`), pero el legacy aplica en `BOk` `FormPadre.analizador.GruposPartidos[...]ActivaFiltro(filtroGE)` (grupo real, `:1507-1513`; en el legacy `filtroGE` es el leído del disco, así que sí llega al analizador), mientras el port aplica `_grupo.ActivaFiltro(...)` (`VM:838`) sobre el grupo leído del disco ⇒ tras Abrir + Aceptar el filtro abierto **no llega** al analizador (A-62-1).
- El legacy preguntaba «El filtro ya tiene datos introducidos. ¿Abrir igualmente?» (`:1524-1528`) y confirmaba Borrar; el port no pregunta.
- `Guardar/Abrir/Copiar/Pegar/Borrar` ejecutan `GuardarDatos()` y por tanto **modifican el filtro real del grupo** aunque luego se pulse Cancelar (alias de `_arrayGE`, `VM:525`).
- `ObtenerFiltroTemporal` (`VM:916-993`, la baja en `VM:940-943`) elimina de `_arrayGE` (la copia de trabajo) la última entrada si el temporal queda vacía: efecto lateral conservado «tal cual» del legacy.
- Los nombres de equipo se leen en cada `CargarDesdeGrupo` (constructor y entrada a la pantalla), sin suscripción a cambios de jornada mientras está abierta (`VM:264-275`; el VM y la página no contienen ninguna suscripción). Tras volver de Estadísticas no se recargan (`CS:34-37`).

**Casos de uso extraídos (CU-B30).** `CondicionGruposEquipos.cargarCopiaDeTrabajo(grupo:)`, `.guardarGrupoActual`, `.cambiarGrupo`, `.eliminarGrupo`, `.confirmar(en:)`, `.relaciones.{guardar,cambiar,eliminar,confirmar}`, `.estadisticas`, `.guardar/abrir/copiar/pegar(.geq)`, `.nombresEquipos(jornada:)`.

**Área Mac propuesta.** *Condiciones y filtros* → hoja/inspector grande con dos pestañas (Grupos, Relaciones) sobre el grupo en edición; los nombres de equipo vienen del *Boleto*. Propuesta; decide el dueño.

### Pantalla 63 · GuardarValoracionFrm — «Guardar valoración»

**Ficheros.** `VM` = `GuardarValoracionFrmViewModel.cs` (186), `XAML` = `GuardarValoracionFrmPage.xaml` (89), `CS` = `GuardarValoracionFrmPage.xaml.cs` (45). Legacy: `Free1X2/UI/GuardarValoracionFrm.cs` (`btAceptar_Click`). Motor equivalente: `Domain/Utils/Porcentajes.cs` (`GuardarValoraciones` :415-420, `Guardar` privado :354-399).

**Propósito.** Exporta una matriz de valoraciones 14×3 (1/X/2) a un `.txt` eligiendo la disposición (3 valores por línea, 42 en una línea, 1 por línea) y el separador (`VM:17-28`).

**Cómo se abre.** Desde la pantalla de Valoración (`ValoracionFrmViewModel.cs:378`: `Navegar?.Invoke(typeof(GuardarValoracionFrmPage))`), que deja la matriz en el handoff estático `GuardarValoracionFrmPage.ValoresIniciales` (consumido y puesto a `null` en `OnNavigatedTo`, `CS:23,32-43`). Sin handoff: rejilla vacía editable. Sin menú.

**Entradas.**

| Control | Tipo / defecto | Evidencia |
|---|---|---|
| Rejilla 14×3 (`PorcentajesControl`) | doubles ≥0; precargada por el llamador | `VM:69-70`, `XAML:27-31` |
| «Disposición de los valores» | `3 valores por línea` (defecto) / `42 valores en una línea` / `1 valor por línea` | `VM:35-44` |
| «Separador» | `[Tabulador]` (defecto) / `[Coma]` / `[Espacio en blanco]`; deshabilitado con «1 valor por línea» | `VM:50-63`, `XAML:60-64` |

**Acciones.**
- **Aceptar** (`VM:82-126`): formato 3/42/1 y separador (`\t`, `,`, ` `; en formato 1 el separador queda en `\t` sin uso); diálogo Guardar sugerido `valoracion`, `.txt` (+ «Todos los archivos»); escribe en segundo plano; éxito ⇒ estado `Valoración guardada en <nombre>.` y diálogo «Valoración guardada correctamente»; error ⇒ «Error al guardar la valoración: <msg>».
- **Cancelar** (`VM:129-134`): `Estado = "Cancelado."` (no cierra nada).

**Lógica de negocio en el ViewModel** (`Guardar`, `VM:140-184`; copia de `Porcentajes.Guardar`, aunque el comentario `VM:26-27` afirma que no está en el dominio, ver A-63-1):

```
n = NumeroPartidos (14) ; valor.ToString() = Double.ToString() CON LA CULTURA ACTUAL
formato 3 : por partido i: "v1<sep>vX<sep>v2"  con cada v = ToString().Replace(",", ".")      // sin separador final
formato 42: una sola línea: concatena, para i=0..13 y j=0..2:  ToString().Replace(",", ".") + sep    // ¡sep también tras el último valor!
formato 1 : 42 líneas, una por valor, en orden partido→(1,X,2): valores[i,j].ToString()          // SIN Replace: con cultura es-ES queda "12,5"
```

**Salidas.** Fichero de texto: decimales con **punto** en los formatos 3 y 42, con la coma de la cultura en el formato 1 (T7). Fin de línea de `WriteLine` (T8), UTF-8 sin BOM (`File.CreateText`). Ejemplo formato 3 (tab): `45\t30\t25`.

**Peculiaridades a preservar.**
- La inconsistencia de decimales entre formato 1 y los demás (T7); fijar el `Locale` explícitamente en Swift y documentar si el formato 1 debe usar coma (comportamiento actual con `es-ES`) o punto.
- Línea única del formato 42 acaba con el separador.
- El formato de salida es el que lee `Porcentajes(ruta)` (detecta 1/3/42/43/44 valores por fila, `Domain/Utils/Porcentajes.cs:84-100`).

**Casos de uso extraídos (CU-B31).** `Valoraciones.guardar(url:matriz:formato:separador:)` (reutilizar la misma función que `Porcentajes.GuardarValoraciones`).

**Área Mac propuesta.** *Análisis* (valoraciones) → hoja modal «Exportar valoración…» abierta desde la pantalla de Valoración (`fileExporter` + opciones). Propuesta; decide el dueño.

### Pantalla 64 · HistoriaValoracionesFrm — «Valoraciones históricas»

**Ficheros.** `VM` = `HistoriaValoracionesFrmViewModel.cs` (341), `XAML` = `HistoriaValoracionesFrmPage.xaml` (140), `CS` = `HistoriaValoracionesFrmPage.xaml.cs` (38). Legacy: `Free1X2/UI/HistoriaValoracionesFrm.cs`; motor equivalente `Domain/Utils/Porcentajes.cs` (`GuardarValoraciones(..., pTemporada, pJornada)` :422-481, `MontaLinea` :482-498).

**Propósito.** Guarda (o reemplaza) la línea de valoraciones 1X2 de una jornada/temporada en un **fichero histórico** (formato 44: `temporada`, `jornada` y 42 valores) (`VM:19-30`).

**Cómo se abre.** Solo desde Tramificar, comando «Guardar en histórico» (`TramificarFormViewModel.cs:1022-1034`): deja `HistoriaValoracionesFrmPage.Contexto = (matriz, temporada, jornada, null)` (handoff estático consumido en `OnNavigatedTo`, `CS:19,28-37`) y navega. Sin menú.

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| «Se guardará la jornada» | `NumberBox` Min 0, Max 43, defecto 1 (◄► `Jornada` ±1 dentro de 0..43) | `VM:37-39,105-117`, `XAML:32-37` |
| «de la temporada» | `NumberBox` entero, defecto 2004; a su lado `/` y `Temporada+1` (solo lectura) | `VM:34-35,49-51,91-101`, `XAML:49-61` |
| Rejilla 14×3 de valoraciones | precargada por el llamador (si no, vacía/0) | `VM:60-61,75-87` |
| «Abrir…» fichero existente (`.txt`/`*`) / «Nuevo…» (Guardar, sugerido `valoraciones_historicas`, `.txt`) | habilitan «Guardar» (`PuedeGuardar`) | `VM:120-148`, `XAML:106-113` |

**Acciones.** **Guardar** (`VM:152-179`): `temporada = Int(Temporada)`; `tempo = "{T}/{T+1}"`; `jorna = Int(Jornada)` rellenado a 2 con `0`; ejecuta `GuardarValoracionesHistorico(ruta, TAB, matriz, tempo, jorna)` en segundo plano. Éxito ⇒ `Guardada la jornada 05 (2004/2005) en <fichero>.` + diálogo «Valoraciones guardadas en el histórico»; error ⇒ «Error al guardar en el histórico: <msg>». **Cancelar** (`VM:182-187`): `Estado="Cancelado."`.

**Lógica de negocio en el ViewModel** (`VM:196-340`, réplica de `Porcentajes`):

```
GuardarHistorico(ruta, sep=TAB, V[14][3], temporada, jornada):
   si el fichero existe: formato = TestFichero(primera línea, &sep)    // sep queda en el último probado (',' -> ' ' -> TAB)
   si no existe: formato = 44
   si formato != 44 -> error «El fichero no es un fichero de Valoraciones históricas»
   líneas = []; existe = false
   si existe el fichero: por cada línea l (Trim().ToUpper()):
        campos = l.split(sep) ; si campos.count>=2 && campos[0]==temporada && campos[1]==jornada: l = MontaLinea(...) ; existe = true
        líneas.append(l)
   si !existe: líneas.append(MontaLinea(...))                       // se añade AL FINAL (sin ordenar)
   reescribir el fichero completo con las líneas (GuardarColsComa = WriteLine tal cual)

MontaLinea = temporada + sep + jornada + Π_{i<14, x<3} ( sep + V[i][x].ToString().Replace(".", ",") )       // decimales con COMA

TestSeparador(línea, sep): n = split(sep).count
   n==3 y 3 numéricos -> 3 ; n ∈ {42,45} y los 42 primeros numéricos -> 42 (si no 2)
   n ∈ {43,46} y campos[1..42] numéricos -> 43 ; n ∈ {44,47} y campos[2..43] numéricos -> 44 ; otro -> 2
TestFichero: línea = primera línea .Trim() ("" si el fichero está vacío) ; prueba sep ',' , ' ' , TAB; devuelve el primer resultado != 2 ;
             si ninguno y la línea es un número -> 1 ; si no -> 2
EsNumero(x) = Convert.ToDouble(x) sin excepción (cultura actual)
```

**Salidas.** Fichero histórico: una línea por jornada, `T/T+1<sep>JJ<sep>v…` (44 campos), valores con coma decimal. Mensajes de estado arriba.

**Peculiaridades a preservar / avisos.**
- Un fichero **existente pero vacío** se clasifica como formato 2 (primera línea `""` ⇒ `EsNumero("")` falso) ⇒ error «El fichero no es un fichero de Valoraciones históricas»; el flujo «Nuevo…» usa un `FileSavePicker` que, por comportamiento de WinRT, crea el fichero vacío antes de Guardar (inferencia, no ejecutado) ⇒ la primera grabación en «Nuevo» podría fallar (A-64-1). En Mac (`NSSavePanel`) un destino inexistente se trata como nuevo (formato 44).
- `LeerLinia` llama a `Replace` sin asignar (sin efecto) (`VM:330-331`, igual que el legacy).
- `Convert.ToDouble` depende de la cultura (T7): `"45,5"` es válido en `es-ES` y no en `en-US`; fijar el `Locale` en Swift.
- Rango de jornada 0..43; temporada sin tope (`Minimum` no definido).

**Casos de uso extraídos (CU-B32).** `HistoricoValoraciones.guardarJornada(url:valoraciones:temporada:jornada:)`, `HistoricoValoraciones.detectarFormato(_:)` (reutilizable por «Importar % / Porcentajes»).

**Área Mac propuesta.** *Análisis* → hoja modal abierta desde Tramificar («Guardar en histórico…»). Propuesta; decide el dueño.

### Pantalla 65 · IfThenFrm — «Condiciones relacionadas (if-then)»

**Ficheros.** `VM` = `IfThenFrmViewModel.cs` (644), `XAML` = `IfThenFrmPage.xaml` (339), `CS` = `IfThenFrmPage.xaml.cs` (34). Legacy: `Free1X2/UI/IfThenFrm.cs` (`btnAdd_Click`, `guardarCondicion`, `cargarDatos`, `llenarGrupos`, `cambioCombo`, `resetConds`, `limpiar`, citados en el VM). Motor: `Domain/MotorCalculo/{ControladorIfThen,CondicionIfThen,GrupoIfThen}.cs` (ESPEC_01).

**Propósito.** Define reglas «si se produce la condición A, entonces debe cumplirse la B», entre **condiciones sencillas** (de filtro) o entre **grupos de partidos**; actúa sobre toda la combinación, no sobre un grupo (`VM:40-50`, `docs/MANUAL_USUARIO.md:253`).

**Cómo se abre.** Botón «If-Then» de la pantalla Inicio (`MainPageViewModel.cs:298-304`, `AbrirIfThen`); también figura en el catálogo de páginas (`Navigation/PortedPages.cs:37`, grupo «Filtros»); no hay entrada en los menús de `MainWindow.xaml.cs`. Opera sobre `AppState.Instancia.Analizador` (campo `_analizador`, `VM:69`); `OnNavigatedTo` llama `CargarDesdeMotor()` (`CS:29-33`).

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| Pestaña «Condiciones sencillas» → bloque «Si …» y «Entonces …»: «Condición genérica» (7 opciones), «Condición específica» (dependiente), «Valor» (`NumberBox` 0..14), «Negado» | defectos: vacío/0/false | `VM:155-192`, `XAML:42-62,82-102` |
| «Condiciones que deben cumplirse» (rango, p. ej. `1` ó `1-3`) | texto; obligatorio si hay relaciones | `VM:194-198,450-454`, `XAML:179-182` |
| Lista «Relaciones» con casilla por fila + «Borrar relación» (borra las marcadas) | — | `VM:194-195,390-402` |
| Pestaña «Grupos»: «Si grupo …», «… entonces grupo …» (combos `"i - NombreGrupo"` para i=1..n−1), «Negado» ×2, rango «Condiciones que deben cumplirse» | pestaña solo útil si hay >1 grupo | `VM:95-106,203-235,406-426`, `XAML:212-226,301-304` |
| Menú: Aceptar, Guardar, Abrir, Copiar, Pegar, Borrar | — | `XAML:315-333` |

Genéricas y específicas (`VM:54-63`, `VM:237-326`; «cambioCombo»): **Cantidad de signos** → `Cantidad de Variantes`, `Cantidad de X`, `Cantidad de 2`; **Dibujos** → `Dibujo i+j` para todo `i,j ∈ 0..14` con `i+j ≤ 14` (sin valor numérico); **Signos Seguidos** → de Variantes, 1, X, 2; **Interrupciones** → Globales, de Variantes/1/X/2 y las 5 «Seguidas»; **Pesos numéricos** → Global, de Variantes/1/X/2; **Distancias** → de Variantes/1/X/2; **Contactos** → `1X,12,X2,11,XX,22,1V,XV,2V,VV`. Cambiar la genérica vacía la específica.

**Acciones.**
- **Añadir (condición)** (`VM:331-355`): compone los dos textos (ver abajo); si falta alguna genérica no hace nada (sin mensaje); `txt == txt2` o ambos contienen `"Dibujo"` ⇒ error «Las condiciones deben ser diferentes»; si no, añade `(Si, Entonces)` y resetea los combos.
- **Añadir (grupo)** (`VM:407-426`): faltan combos ⇒ nada; `GrupoIf == GrupoThen` ⇒ «Los grupos deben ser diferentes»; texto = `["(NO) "] + "i - NombreGrupo"`.
- **Aceptar** (`VM:515-524`): `GuardarCondicion()`; nulo ⇒ no cierra; `analizador.IfThen = ifThen`; `NotificarCambio()`; volver.
- **Guardar** (`VM:527-545`): `GuardarCondicion()` y si `ifThen.EsVacio` no hace nada; diálogo Guardar sugerido `Condiciones`, tipos «Condiciones relacionadas» `.if` y `(XML)` `.xml`; `ArchivoCondiciones.GuardaArchivo(ifThen)`.
- **Abrir / Pegar** (`VM:548-574`, `598-605`): `Limpiar()` y `ArchivoCondiciones.AbrirArchivoCombinacion` + `LeeIfThen()` + `CargarDatos`; **Copiar** (`VM:577-595`): `GuardarCondicion()` (no vacío) → `Temp/tmp.if` y `PuedePegar=true`; **Borrar** = `Limpiar()` (listas, rangos y combos).

**Lógica de negocio en el ViewModel.**

```
ComponerCondicion(generica, especifica, valor, usaValor, negado) -> String?:                       (VM:362-388)
   si generica vacía -> nil
   si especifica no vacía: txt = especifica ; si NO contiene "Dibujo": txt += ": "
   si no: txt = generica + ": "
   si usaValor || especifica vacía: txt += String(Int(valor))        // los «Dibujo i+j» no llevan número
   si negado: txt = "(NO) " + txt
   ej.: "Cantidad de Variantes: 5" ; "(NO) Interrupciones de 1: 3" ; "Dibujo 3+4"

GuardarCondicion() -> ControladorIfThen?:                                                           (VM:448-510)
   si RangoCondiciones vacío y hay relaciones -> error «Debe indicarse la cantidad de condiciones que se deben cumplir.» -> nil
   si RangoGrupos vacío y hay relaciones de grupos -> error «Debe indicarse la cantidad de grupos que se deben cumplir.» -> nil
   para cada relación sencilla: CondicionIfThen{CondIf = Si, CondThen = Entonces}; si hay alguna: RangoAciertoCondiciones = RangoCondiciones
   para cada relación de grupos: texto «[(NO) ]i - Nombre»: NoIf/NoThen si empieza por "(NO)" (se descartan 5 chars); i = Int16(texto hasta el primer espacio);
          GrupoIf = analizador.GruposPartidos[i] (ídem Then); si hay alguna: RangoAciertoGrupos = RangoGrupos

CargarDatos(ifThen) (VM:108-148): rellena listas; RangoCondiciones/RangoGrupos solo si hay elementos;
   grupos del fichero con índice >= GruposPartidos.count -> error «Uno de los grupos del fichero no existe en la combinación actual. Se quita de la lista.» y se omiten
   textos de grupos: «[(NO) ]{NumGrupoIf} - {GruposPartidos[NumGrupoIf].NombreGrupo}»
```

**Salidas.** Lista de relaciones (`Si`, `Entonces` como texto), contadores «Condiciones en lista» (`Count`), estado de la combinación (`IfThen` del analizador; semáforo verde/rojo en Inicio según `EsActivo`, `MainPageViewModel.cs`).

**Peculiaridades a preservar.**
- Los valores se almacenan **como texto** y el motor los vuelve a interpretar por texto (`ControladorIfThen`, ESPEC_01): el formato exacto de arriba es parte del contrato de ficheros `.if`.
- Una genérica sin específica genera `"Cantidad de signos: 3"` (válida para el VM, comportamiento del dominio sin verificar).
- El campo `Valor` se trunca a `Int` (`(int)valor`, `VM:383`) y se limita a 0..14 por la UI (`XAML:56-57,96-97`).
- `GuardarCondicion` no valida el formato del rango (`1-3`) ni que `Entonces` ≠ `Si` en grupos de distinto nombre.
- `Aceptar` no comprueba `EsVacio`: con ambas listas vacías asigna un `ControladorIfThen` vacío (`VM:518-521`); `Guardar` y `Copiar` sí lo comprueban (`VM:531,581`). `AbrirDesde` hace `Limpiar()` antes de leer, de modo que un fallo de lectura deja la pantalla vacía (`VM:560`).
- El botón Pegar se habilita si existe `Temp/tmp.if` al construir la pantalla (`VM:81`) y tras Copiar (`VM:589`).

**Casos de uso extraídos (CU-B33).** `IfThenEditor.componerCondicion(...)`, `.especificasDe(generica:)`, `.construirControlador(relaciones:grupos:rangos:analizador:)`, `.cargar(controlador:analizador:)`, `.guardar/abrir/copiar/pegar(.if)`.

**Área Mac propuesta.** *Condiciones y filtros* → vista principal secundaria (editor de reglas tipo lista, en pestañas Condiciones/Grupos) accesible desde el panel de condiciones de la combinación. Propuesta; decide el dueño.

### Pantalla 66 · ImportExportFrm — «Importar / Exportar columnas»

**Ficheros.** `VM` = `ImportExportFrmViewModel.cs` (221), `XAML` = `ImportExportFrmPage.xaml` (103), `CS` = `ImportExportFrmPage.xaml.cs` (19, sin lógica). Legacy: `Free1X2/UI/ImportExportFrm.cs` (`btnAbrirEntrada_Click`, `btnAbrirSalida_Click`, `btnOk_Click`). Lectura/escritura: `Domain/EntradaSalida/ArchivoColumnasTexto.cs`.

**Propósito.** Convierte un fichero de columnas entre **CSV** (signos separados por comas) y **TXT** (14 signos seguidos) en ambos sentidos (`VM:14-18`, `docs/MANUAL_USUARIO.md:449-450`).

**Cómo se abre.** Menú Archivo «Importar / exportar columnas…» (`MainWindow.xaml.cs:180`) y botón `bImportExport` (`:430`). Autónoma.

**Entradas.**

| Control | Tipo / defecto | Evidencia |
|---|---|---|
| «Tipo de conversión» (combo) | `0` «Separadas por comas (*.csv) a TXT (*.txt)» (defecto, legacy `conversion=2`) / `1` «TXT (*.txt) a Separadas por comas (*.csv)» (`conversion=3`) | `VM:27-34,58` |
| Fichero de entrada «Examinar…» | Abrir con extensión `.csv` (conv. 0) o `.txt` (conv. 1); la caja de ruta es editable | `VM:62-111`, `XAML:39-48` |
| Fichero de salida «Examinar…» | Guardar sugerido `Columnas`, extensión `.txt`/`.csv` según conversión | `VM:114-125`, `XAML:67-76` |
| «Hacer» / «Cancelar» | `Cancelar` no hace nada (el cierre es del host) | `VM:128-190` |

**Acciones.**
- **Abrir entrada** (`VM:63-111`): lee **todas** las líneas en segundo plano: conversión 2 ⇒ `LeerTodasColsSeparadasPorComas()` (cada línea `Trim().ToUpper()` y sin comas); conversión 3 ⇒ `LeerTodasCols(true)` (cada línea `Trim().ToUpper()` y con comas insertadas entre signos). Propone salida `ruta.ToLower()` con `.csv→.txt` (conv. 2) o `.txt→.csv` (conv. 3). Muestra `«N columnas.»`. Error ⇒ «No se ha podido leer el fichero de entrada».
- **Hacer** (`VM:129-182`), en este orden: salida vacía ⇒ no hace nada (sin mensaje); sin columnas cargadas ⇒ «No se ha cargado el fichero de entrada o no tiene columnas.»; `FicheroEntrada == FicheroSalida` ⇒ «Los archivos de entrada y salida deben ser distintos»; si la salida existe ⇒ diálogo «Ya existe el fichero de salida. ¿Sobreescribir?» (botón por defecto «Cancelar»); escribe con `GuardarTodasCols(columnas, conComa: conversion != 2)`; muestra `«N columnas.»` con `ObtenNumCols()` de la salida y «Transformación finalizada»; error ⇒ «No se ha podido guardar el fichero de salida».

**Lógica de negocio en el ViewModel.** Solo la elección de modo y la propuesta de nombres; el cambio de formato lo hace el motor:

```
CSV -> TXT: leer: línea.trim().uppercased().replacing(",", "") ; escribir GuardarCols(col) = col.replacing(",", "") + WriteLine
TXT -> CSV: leer: línea.trim().uppercased() ; DarFormatoColumna: si la línea NO contiene ',' -> signos separados por ',' ; si YA contiene ',' -> devuelve "" (cadena vacía)
            escribir GuardarColsComa(col) = col tal cual + WriteLine
ObtenNumCols(fichero) = Int( tamañoEnBytes / (longitud(1.ª línea.trim()) + 2) )      // ESTIMACIÓN que supone fin de línea CRLF (2 bytes) y líneas de igual longitud
```

**Salidas.** Fichero `.txt` (p. ej. `1X2…`) o `.csv` (`1,X,2,…`) UTF-8 sin BOM; contadores de texto `«N columnas.»`.

**Peculiaridades a preservar / avisos.**
- `DarFormatoColumna` devuelve cadena vacía si la línea ya trae comas (`ArchivoColumnasTexto.cs:290-312`): un `.txt` que ya contenga comas daría líneas vacías en TXT→CSV.
- `ObtenNumCols` es una **aproximación por tamaño** (`:314-338`): con fin de línea `\n` (macOS) subestima. La cifra «columnas» de salida debería calcularse exacta en Swift (A-66-1); la de entrada ya es exacta (`columnas.Length`).
- La ruta propuesta se pasa a minúsculas completa (`VM:98-100`), lo que cambia mayúsculas de carpetas; en Mac conviene cambiar solo la extensión.
- La conversión no valida el contenido de las columnas (longitud, alfabeto).

**Casos de uso extraídos (CU-B34).** `ConversorColumnas.csvATxt(entrada:salida:)`, `.txtACsv(entrada:salida:)` (+ `contarColumnas(url:)`).

**Área Mac propuesta.** *Herramientas* → hoja modal pequeña («Convertir columnas…»). Propuesta; decide el dueño.

### Pantalla 67 · ImportadorCPsFrm — «Importador de columnas probables»

**Ficheros.** `VM` = `ImportadorCPsFrmViewModel.cs` (224), `XAML` = `ImportadorCPsFrmPage.xaml` (80), `CS` = `ImportadorCPsFrmPage.xaml.cs` (16, solo cablea `Volver`). Legacy: `Free1X2/UI/ImportadorCPsFrm.cs` (`btnImportarSimples_Click`, `btnImportarClm_Click`, `ObtenPronostico`) y `Free1X2/UI/Filtros/ColProbablesFrm.cs:690-740` (consumo del resultado).

**Propósito.** Diálogo que lee un fichero de columnas (`.txt` simple o `.clm` con estadísticas) y devuelve al editor de Columnas Probables una lista temporal de CPs (`VM:18-25`).

**Cómo se abre.** Solo desde el editor de Columnas Probables (`ColProbablesFrmViewModel.cs:726-748`, comando `ImportarCPs`): elimina la última CP si está vacía (`NecesitaBorrarUltimaCP`, `:464-473`), pone `_importPendiente=true` (`:742`), `ImportadorCPsFrmViewModel.Resultado=null` (`:746`) y navega (`:747`). Al volver, el editor lee `Resultado` y decide entre **sustituir manteniendo rangos** (si el nº de importadas coincide con las existentes, previa confirmación), **añadir** o **reemplazar** (`AplicarImportacionSustituir/Agregar/Reemplazar`, `ColProbablesFrmViewModel.cs:797,829,846`; condición de confirmación `:750-760`). No hay menú.

**Entradas.** Ninguna editable; dos botones y «Cancelar».

**Acciones.**
- **Importar CPs Simples (sólo columnas \*.txt)** (`VM:57-111`): Abrir `.txt`/`*` (DocumentsLibrary); `todosValores = ObtenerTodosValores()` (= `0,1,…,14`); lee cada línea con `ArchivoColumnasTexto.LeeColumnaSinComas()` (`Trim().ToUpper()`); ignora líneas vacías; por línea crea `ColumnaProbable{ Pronosticos = ObtenPronostico(línea) }` y `SetNoAciertos/SetNoAciertosSeguidos/SetNoFallosSeguidos(todosValores)` (todos los valores admitidos). Cualquier excepción ⇒ «Las columnas no tienen el formato correcto» (+ estado «Error: formato de columnas incorrecto.»), sin importar nada. Éxito ⇒ `Resultado = lista`, estado «Importadas N columnas simples desde <f>.», `Volver()`.
- **Importar CPs con Ac, Acs, Fs (\*.clm)** (`VM:119-171`): Abrir `.clm`/`*`; por línea `partes = línea.split('#')`; si `partes.count == 4` crea `ColumnaProbable{ Pronosticos = ObtenPronostico(partes[0]) }`, `SetNoAciertos(partes[1])`, `SetNoAciertosSeguidos(partes[2])`, `SetNoFallosSeguidos(partes[3])`; **las líneas con otro nº de partes se ignoran en silencio**; excepción ⇒ mismo error. Ejemplo de formato: `111xxx22211xx2#1,2,3,4,5#1,2,3,4,5#1,2,3,4,5` (`VM:116`).
- **Cancelar** (`VM:178-187`): `Resultado=null`, estado «Importación cancelada.», `Volver()`.

**Lógica de negocio en el ViewModel.**

```
ObtenPronostico(línea) -> [String]:                                         (VM:199-222)
   línea = línea.uppercased()
   si contiene ','  -> línea.split(',')              // cada elemento = pronóstico de un partido: "1", "X2", "1X2"…
   si no            -> 14 elementos de un carácter: línea[0], línea[1], …, línea[13]     // línea más corta de 14 -> IndexOutOfRange -> «formato incorrecto»; más larga: sobran ignorados
SetNoAciertos(texto) etc.: el dominio interpreta texto "a,b,c[letraTol]" (tolerancias) — ColumnaProbable.cs (ESPEC_04)
```

**Salidas.** `Resultado: [ColumnaProbable]` (handoff estático), contador `«N columnas importadas.»` / singular `«1 columna importada.»` (`VM:189-193`).

**Peculiaridades a preservar.**
- El texto de ayuda de la pantalla dice que las CPs simples se importan «con los aciertos calculados sobre el histórico» (`XAML:26`), pero el código les asigna **todos** los valores `0..14` (`VM:66,84-86`).
- No se valida que cada pronóstico sea un subconjunto de `{1,X,2}` ni la longitud en la rama con comas (se acepta lo que produzca `split`).
- La importación no es transaccional: el error se detecta tras leer parcialmente, pero no hay resultado parcial (`VM:99-104`).

**Casos de uso extraídos (CU-B35).** `ImportadorCP.simples(url:) -> [ColumnaProbable]`, `ImportadorCP.conEstadisticas(url:)`, `ImportadorCP.pronosticosDeLinea(_:)`; la política sustituir/añadir/reemplazar pertenece al caso de uso del editor de CPs.

**Área Mac propuesta.** *Columnas probables* → no necesita pantalla: acción «Importar…» del editor de CPs (`fileImporter` + sheet de elección sustituir/añadir). Propuesta; decide el dueño.

### Pantalla 68 · ImprimirBoletoFrm — «Impresión de boletos»

**Ficheros.** `VM` = `ImprimirBoletoFrmViewModel.cs` (356), `XAML` = `ImprimirBoletoFrmPage.xaml` (159), `CS` = `ImprimirBoletoFrmPage.xaml.cs` (201; dibujo en `Canvas`, exportación a PNG/portapapeles). Legacy: `Free1X2/UI/imprimirBoleto.cs` (clase `ImprimirBoletoFrm`: `GetConfig` :81-99, `SetConfig` :112-128, `LeerCols` :130-151, `Preparar` desde :152, `Imprimir(PrintPageEventArgs)` :519-559, `btnVerImpresoras_Click` :564-587).

**Propósito.** Posiciona sobre el boleto oficial las marcas 1/X/2 de un fichero de columnas (8 columnas por boleto) para imprimirlas, con márgenes ajustables, rango de boletos e impresora conocida (`VM:33-40`).

**Cómo se abre.** Menú Combinación «Imprimir boletos…» (`MainWindow.xaml.cs:219`) y botón `bImprimirBoletos` (`:392`). Sale a «Impresoras conocidas» (`ListaImpresorasPage`, pantalla 70, `CS:54-58`).

**Entradas.**

| Control | Tipo / defecto | Evidencia |
|---|---|---|
| «Margen superior» / «Margen izquierdo» | `NumberBox` double, defectos 100 / 30 (unidad de impresión, ver abajo) | `VM:48-53`, `XAML:28-38` |
| «Girar boleto» | toggle, defecto No; **no altera la geometría** (solo se guarda en el `.cfg`) | `VM:62-64,115`, `XAML:41-43` |
| «Impresora seleccionada» | texto, defecto «(ninguna)» | `VM:80-81` |
| «Leer columnas» | Abrir `.txt` (DocumentsLibrary; legacy: carpeta `Columnas`); resetea fichero y nº de columnas; define el rango (1..⌈n/8⌉) | `VM:134-182` |
| «Desde el boleto» / «Hasta el boleto» | `NumberBox` Min 1, defectos 1/1 | `VM:55-60`, `XAML:119-131` |
| «Guardar configuración» / «Recuperar configuración» / «Impresoras conocidas» | — | `VM:274-342` |
| «Imprimir / Guardar como imagen» | — | `VM:184-212` |

**Acciones.**
- **Leer columnas** (`VM:135-182`): lee con `ArchivoColumnasTexto.LeeColumnaSinComas()` (`Trim().ToUpper()`); `boletos = (n-1)/8 + 1` (mínimo 1); `Desde=1`, `Hasta=boletos`; muestra `FicheroEntrada` (nombre) y `NumeroColumnas`.
- **Imprimir** (`VM:185-212`): sin columnas ⇒ «No se ha seleccionado el archivo»; `ComputarBoletos()`; vacío ⇒ «No hay columnas en el rango de boletos seleccionado.»; si hay boletos dispara `ExportacionSolicitada` ⇒ la página dibuja en `Canvas`, copia la imagen al portapapeles y ofrece guardar PNG (`PicturesLibrary`, sugerido `boleto`) (`CS:86-130`). **El port no imprime**: el legacy usaba `PrintDocument` (una página por boleto).
- **Guardar configuración** (`VM:275-294`): escribe `Impresion/imprebol.cfg` (4 líneas) y avisa «Configuración guardada». **Recuperar** (`VM:297-331`): lee las 4 líneas (`clave=valor`); fallo ⇒ «No se ha podido leer la configuración guardada»; el constructor la intenta al abrir.

**Formato de `imprebol.cfg`** (`VM:284-287,316-323`):
```
margen superior=<int>
margen izquierdo=<int>
modelo=<texto de la impresora>
girar=si|no
```

**Lógica de negocio en el ViewModel** (`ComputarBoletos`, `VM:221-272`, idéntica a `imprimirBoleto.cs:519-559`):

```
mgy = MargenSuperior(Float) ; mgx = MargenIzquierdo(Float)
minbol = max(1, Int(Desde)) ; maxbol = max(minbol, Int(Hasta))
maxcol = min(maxbol*8, cols.count) ; mincol = (minbol-1)*8 ; si mincol == maxcol { mincol -= 1 } ; si mincol < 0 { mincol = 0 }
nwcol = mincol ; actubol = minbol
mientras nwcol < maxcol:                                    // un boleto por vuelta
    p15 = cols[nwcol].count > 14 ? cols[nwcol][14] : '1'    // pleno al 15 de la 1.ª columna del boleto
    para nw8 en 0...7:
        columna = cols[nwcol] ; nwcol += 1
        para pa en 0 ..< min(14, columna.count):
            ch = columna[pa]
            cx = mgx - 29 + (14 - pa) * 19.685
            inc = ch=='1' ? 0 : (ch=='X' ? 14.764 : 29.528)          // cualquier otro carácter se trata como '2'
            cy = mgy + 83 + nw8 * 44.291 + inc
            marca(cx, cy, w: 6, h: 4)
        si nwcol == cols.count: break
        si nwcol + 2 == cols.count && nw8 == 6: break               // evita dejar un boleto final con 1 sola columna (7 + 2)
    pleno: cx = mgx - 30 ; cy = p15=='1' ? mgy+245 : (p15=='2' ? mgy+305 : mgy+275) ; marca(cx, cy, 6, 4)
    encabezado = "{fichero}   Boleto {actubol}" ; actubol += 1
```
Unidades: las constantes 19,685 / 14,764 / 29,528 / 44,291 equivalen a 5 / 3,75 / 7,5 / 11,25 mm si la unidad es la centésima de pulgada (unidad por defecto de `PrintDocument`); **inferido, no verificado** (A-68-1). El legacy dibuja el nombre del fichero en `(mgx, mgy)` y «Boleto N» en `(mgx, mgy+25)` con Arial 14 (`:525-526`); el port los une en una cabecera independiente de los márgenes.

**Salidas.** Marcas (rectángulos negros 6×4) por boleto; imagen PNG/portapapeles en el port; `imprebol.cfg`.

**Peculiaridades a preservar.** `Girar boleto` no tiene efecto en la geometría ni en el legacy (`UI/imprimirBoleto.cs:119,573`: solo cambia la imagen del formulario y se escribe en el `.cfg`). `GetConfig` del legacy devolvía siempre `false` (`:98`), por lo que `SetConfig()` se ejecutaba al abrir y **sobrescribía** el `.cfg` con los valores de pantalla (A-68-2).

**Casos de uso extraídos (CU-B36).** `ImpresionBoleto.calcularMarcas(columnas:margenes:rango:) -> [Boleto]`, `ImpresionBoleto.leerConfig/guardarConfig(imprebol.cfg)`, `ImpresionBoleto.rangoPorDefecto(nColumnas:)`.

**Área Mac propuesta.** *Boleto* → hoja de impresión (`NSPrintOperation` sobre vista con las marcas + vista previa), con las impresoras conocidas como menú desplegable en la propia hoja. Propuesta; decide el dueño.

### Pantalla 69 · InterrupcionesFrm — Condición «Interrupciones»

**Ficheros.** `VM` = `InterrupcionesFrmViewModel.cs` (288), `XAML` = `InterrupcionesFrmPage.xaml` (224), `CS` = `InterrupcionesFrmPage.xaml.cs` (36). Base: `FiltroArchivoViewModelBase` (§0.3). Legacy: `Free1X2/UI/Filtros/InterrupcionesFrm.cs` (`MarcarValores` :90-103, `ActualizarDatos` :487-717, `guardar` :762-766, `abrir` :750-758). Motor: `FiltroInterrupciones` (ESPEC_02).

**Propósito.** Limita las **interrupciones** (cada cambio de signo a lo largo de la columna) y las **interrupciones seguidas**, globales, de variantes y de cada signo (`XAML:13`, `docs/MANUAL_USUARIO.md:225-226`).

**Cómo se abre.** Rejilla de Inicio, «Interrupciones» (`MainPageViewModel.cs:161`, `Filtro.NoInterrupciones`) con `GrupoEnEdicion`. Recarga salvo al volver de Estadísticas (`CS:28-35`).

**Entradas.** Diez textos libres, dos bloques de cinco: **Interrupciones** (`Global`, `Var`, `1`, `X`, `2`) y **Seguidas** (`Global`, `Var`, `1`, `X`, `2`) (`VM:31-73`, `XAML:22,107`). Valores 0–14 separados por coma (p. ej. `0,1,2`), cada uno con sufijo opcional de letra de tolerancia; sin rangos `a-b` (parser del dominio `FiltroInterrupciones.cs:523-547`: `Split(',')`, si el último carácter es letra la separa como tolerancia, `Convert.ToInt32` del resto y marca `intGlobal[valor]`). Defecto: vacío.

**Acciones.**
- **Aceptar** (`VM:137-177`): `filtro.ReinicializaValores()`; si algún campo no está en blanco (`ContieneDatos`, `VM:101-112`): si el filtro no tenía datos ⇒ `IsActive=true`; `ContieneDatos=true`; **cada campo vacío se rellena con `0,1,…,14`** (`TodosValores`, `VM:76`) y se aplica con `SetNoIntGlobales/Var/1/X/2` y `SetNoIntGlobalSeg/VarSeg/1Seg/XSeg/2Seg`; si todos están en blanco ⇒ `IsActive=false`, `ContieneDatos=false`. `ActivaFiltro`, `NotificarCambio`, volver. Sin `try/catch`: un texto inválido propaga la excepción tras `ReinicializaValores` (`VM:146`), que ya ha puesto a `false` todos los valores del filtro real. Entradas que fallan en el dominio: elemento vacío (p. ej. coma final: `str[str.Length-1]`), rangos `a-b` o texto no numérico (`FormatException`) y valores fuera de 0..14 (índice del array) (`FiltroInterrupciones.cs:523-547`). `ReinicializaValores`: `FiltroInterrupciones.cs:493-521`.
- **Cancelar** (solo vuelve), **Borrar** (`VM:179-193`: vacía los 10 campos; **no** toca el filtro del grupo hasta Aceptar), **Estadísticas** (`VM:270-280`: filtro temporal → `CalculadorEstadisticas.EstadisticasFiltro(filtro, <exe>/Ganadoras/)` → visor).
- **Guardar/Abrir/Copiar/Pegar**: `.int` y `.xml`, temporal `Temp/tmp.int` (`VM:88-95`). Guardar: filtro temporal + `GuardaArchivo`; `GuardarEn` `VM:249-254`; Abrir solo vuelca a pantalla desde `GetFiltro("NoInterrupciones")` (`AbrirDesde` `VM:257-266`, `MarcarValores` `VM:233-246`).

**Lógica de negocio en el ViewModel.** Idéntica a Distancias (pantalla 43): «campo vacío ⇒ todos los valores 0..14»; «ningún campo ⇒ condición vacía y desactivada»; `ObtenerFiltroTemporal` (`VM:199-230`) repite la normalización sobre un filtro nuevo. `CargarDesdeGrupo` vuelca `GetIntGlobales/Var/1/X/2` y `GetIntGlobalSeg/VarSeg/1Seg/XSeg/2Seg` (`VM:118-135`).

**Salidas.** Diez textos `v[letra],…` ascendentes; efecto en el filtro del grupo.

**Peculiaridades a preservar.** El enum del filtro es `NoInterrupciones` (no `Interrupciones`) también para ficheros (`VM:78-79,263`). La ayuda promete «separa varios valores con comas» y coincide con el parser (a diferencia de Distancias, A-43-1).

**Casos de uso extraídos (CU-B37).** `CondicionInterrupciones.aplicar(grupo, textos)`, `.volcar(grupo)`, `.estadisticas`, `.guardar/abrir/copiar/pegar(.int)`.

**Área Mac propuesta.** *Condiciones y filtros* → inspector/hoja del grupo en edición. Propuesta; decide el dueño.

### Pantalla 70 · ListaImpresoras — «Lista de impresoras conocidas»

**Ficheros.** `VM` = `ListaImpresorasViewModel.cs` (198), `XAML` = `ListaImpresorasPage.xaml` (57), `CS` = `ListaImpresorasPage.xaml.cs` (34). Legacy: `Free1X2/UI/ListaImpresoras.cs`. Motor: `Domain/MotorCalculo/ControladoresImpresion.cs`, `ControladorImpresion.cs`.

**Propósito.** Muestra los modelos de impresora soportados (con márgenes y giro predefinidos) y, al hacer doble clic, aplica su configuración al formulario de impresión de boletos (`VM:13-20`).

**Cómo se abre.** Solo desde «Impresoras conocidas» de Impresión de boletos (`ImprimirBoletoFrmPage.xaml.cs:54-58`, `Frame.Navigate(typeof(ListaImpresorasPage))`). Sin menú.

**Entradas.** Lista ordenada de modelos (`ListView`, selección simple, doble clic = elegir) (`XAML:35-40`, `CS:27-33`).

**Acciones.**
- **Carga** (constructor, `VM:32-38,83-112`): `new ControladoresImpresion().Impresoras` lee `<exe>/Impresion/impresoras.cfg` (XML `<impresoras>` con hijos `modelo`, `ms`, `mi`, `girar`; sin duplicados, `ControladoresImpresion.cs:45-70`); ordena por `Modelo` con `string.Compare(…, CurrentCulture)`; lista vacía ⇒ `Mensaje="No hay impresoras"`; excepción ⇒ «No se han encontrado impresoras».
- **Selección** (selección simple): `Mensaje = "Impresora seleccionada: <modelo>"` o «Doble clic sobre una impresora de la lista» (`VM:114-131`).
- **Doble clic** (`VM:138-167`): busca el controlador por `Modelo` (si no existe, usa **el último** de la lista), copia `Modelo`, `MargenSuperior`, `MargenIzquierda`, `Rotar` al controlador activo, deja `SeleccionResultado = controladorActivo` (handoff estático) y solicita cierre (`Frame.GoBack`). El productor lo aplica en su `OnNavigatedTo` (`ImprimirBoletoFrmPage.xaml.cs:65-78`).

**Lógica de negocio.** Solo la búsqueda y copia de campos descrita (`BuscarImpresora`, `VM:185-197`). El parseo del XML de impresoras (`ms`, `mi` enteros; `girar` booleano `true/false`) vive en el dominio.

**Salidas.** `ControladorImpresion { Modelo: String, MargenSuperior: Int, MargenIzquierda: Int, Rotar: Bool }`.

**Peculiaridades a preservar.** Orden alfabético dependiente de la cultura (T11) y el comportamiento de «no encontrado ⇒ último».

**Casos de uso extraídos (CU-B38).** `ImpresorasConocidas.cargar()` (de `impresoras.cfg`) y `.buscar(modelo:)`.

**Área Mac propuesta.** *Boleto* → menú desplegable/`Picker` dentro de la hoja de impresión (no necesita pantalla propia). En Mac podrían complementarse con las impresoras reales del sistema. Propuesta; decide el dueño.

### Pantalla 71 · ListadoCondicionesFrm — «Listado de condiciones»

**Ficheros.** `VM` = `ListadoCondicionesFrmViewModel.cs` (685), `XAML` = `ListadoCondicionesFrmPage.xaml` (67), `CS` = `ListadoCondicionesFrmPage.xaml.cs` (28). Legacy: `Free1X2/UI/ListadoCondicionesFrm.cs` (`ListadoCondicionesFrm_Load` :81 con el `switch` de filtros :143-679, `ObtenerStringPronosticoCP` :61-79, `Exportar_Click` :862, `ExportarHtml_Click` :882).

**Propósito.** Muestra en un **árbol** todo lo configurado en la combinación actual (pronóstico base, filtro de columnas, grupos y sus condiciones activas, control de grupos, If-Then) y lo exporta a texto o HTML (`VM:50-60`, `docs/MANUAL_USUARIO.md:146`).

**Cómo se abre.** Menú Ver «Listado de condiciones» (`MainWindow.xaml.cs:198`); no tiene botón de barra. Solo lectura sobre `AppState.Instancia.Analizador` y `AppState.ArchivoFiltroCols`; `OnNavigatedTo` reconstruye el árbol (`CS:21-25`; el constructor del VM también lo hace, `VM:66-69`).

**Entradas.** Ninguna editable: botones «Expandir Todo», «Colapsar Todo», «Exportar a Texto», «Exportar a Html» (`XAML:27-42`) y un `TreeView` (`XAML:49-64`).

**Acciones.**
- **Expandir/Colapsar todo** (`VM:576-586`, `676-683`): pone `IsExpanded` recursivo. El nodo raíz arranca expandido.
- **Exportar a Texto** (`VM:588-595`, `GuardarYAbrirAsync` `VM:659-674`): `ExportarATexto(raíz, 0)`; diálogo Guardar sugerido `Listados`, tipo «Listados» `.txt`; escribe UTF-8 (legacy: con BOM, `StreamWriter(..., Encoding.UTF8)`, ver A-71-1) y **abre el fichero con la app asociada** (`Launcher.LaunchFileAsync`; legacy: `Process.Start` solo si el fichero existe).
- **Exportar a Html** (`VM:597-606`): `ExportarAHtml(raíz, 0)` envuelto en `<html><head><title>Listado de Condiciones</title></head><body bgcolor="#DBFEBC">…</body></html>`; `.html`; igual apertura. Error ⇒ «No se pudo exportar: <msg>».

**Lógica de negocio en el ViewModel** (construcción del árbol, `Reconstruir` `VM:75-170`; detalle por filtro `ConstruirDetalleFiltro` `VM:180-347` y sus auxiliares `VM:351-574`). Estructura (nodo → hijos):

```
Listado de Condiciones   (expandido)
├─ Pronóstico Base (VM:83-89) → "Partido {i+1}: {Pronosticos[i]}" para i en 0..<analizador.Pronosticos.count (vacío si nil)
├─ Filtro de Columnas (VM:91-94) → ArchivoFiltroCols  o  "No se usa ningún Filtro"
├─ Grupos (VM:96-120) → por grupo i (0 -> "Boleto Base", i -> "Grupo {i}"):
│    ├─ "Partidos Activos: " + grupo.ObtenPartidosActivos()
│    ├─ (si grupo.UsaFiltroParcial) "Filtro Parcial: " + grupo.ArchivoFiltroGrupo
│    └─ Condiciones → por cada filtro con IsActive: nodo = NombreFiltro.ToString() + detalle (tabla siguiente)
├─ Control de grupos (VM:122-138) → para i=1..<ControlesGrupos.count: "Control de Grupos" {"Grupos: "+ObtenGruposControlados(), "Fallos: "+ObtenFallosPermitidos()}
│                               para cada ControlesConjuntos: "Control Conjuntos" {"Conjuntos: "+ObtenCtrlGruposControladosStr(), "Fallos: "+ObtenFallosPermitidosStr()}
└─ IfThen (VM:140-169) → si IfThen != nil && EsActivo: "Condiciones Relacionadas" {por condición "Condición" {"Si se da: "+CondIf, "Entonces: "+CondThen}; "Condiciones que se cumplen: "+RangoAciertoCondiciones}
                                + si hay grupos: "Grupos Relacionados" {por grupo "Grupos" {"Si el grupo {NumGrupoIf} es {NoIf}", "Entonces el grupo: {NumGrupoThen} debe ser {NoThen}"}; "Grupos que se cumplen: "+RangoAciertoGrupos}
                                sino "IfThen no activado"      // NoIf/NoThen se imprimen como booleanos "True"/"False"
```

| Filtro (VM; legacy) | Hijos (texto literal) |
|---|---|
| NoVariantes (`VM:185-192`; legacy :146-155) | `Cantidad de V: `, `Cantidad de X: `, `Cantidad de 2: ` + getters `GetVariantes/GetEquis/GetDoses` |
| SignosSeguidos (`VM:195-203`; :158-169) | `Cantidad de V Seguidas: `, `…1 Seguidos: `, `…X Seguidas: `, `…2 Seguidos: ` |
| Dibujos (`VM:206-211`; :172-177) | `Dibujos : ` + `GetDibujos()` |
| ColProbables (`VM:214-219` y `364-448`; :180-335) | «Columnas Probables» → por CP «Columna Probable: <pronósticos separados por coma, `*` si vacío>» con `Aciertos`, `Aciertos Seguidos`, `Fallos Seguidos`, `Tolerancias …`, `Aciertos en Tolerancias`, `Puntos` (solo si no vacíos); «Relaciones» (Columnas, Cuantas, **Aciertos = CantidadCP**, Suma Aciertos, Recorrido); «Relaciones II» (dos frases «Las columnas … tendrán … que las columnas …»); «Relaciones III» (Columnas, Concepto, Sandwichs, Escaleras{Totales,Ascendentes,Descendentes}, Agrupaciones Paso Fijo/Solapadas {Número, Elementos, Aciertos}); «Control de Fallos» (Columnas, Tolerancia, Aciertos) + `Numero de Fallos de Controles: ` |
| NoInterrupciones (`VM:222-237`; :338-363) | `Interrupciones Globales: `, `…de 1: `, `…de X: `, `Interrupciones de 2 ` (**sin dos puntos**), `… Globales Seguidas: `, `…de 1/X/2 Seguidas: ` (no lista «Var») |
| PesosNumericos (`VM:240-255`; :366-396) | `Peso Global/Variantes/1/X/2: ` + «Tolerancias Peso …» y «Número de Tolerancias: » solo si no vacíos |
| ValoracionSignos (`VM:258-272`; :399-426) | «Valoración» → una línea `v1-vX-v2` por partido; `Límites Valoración Global/1/X/2: `, `Tipo de Valoración: ` |
| Distancias (`VM:275-283`; :429-442) | `Distancias de Variantes/1/X/2: ` |
| GruposEquipos (`VM:286-291` y `452-488`; :445-517) | por conjunto «Conjunto N» {«Equipos Marcados» → por partido: `Marcado - Desmarcado` (1), `Desmarcado - Marcado` (2), `Marcado - Marcado` (3), `Desmarcado - Desmarcado` (0); Victorias, Empates, Derrotas, Suma Puntos}; al final «Relaciones» {«Grupos de Equipos: x» {Suma Victorias/Empates/Derrotas/Puntos}} |
| Contactos (`VM:294-308`; :520-545) | `Contactos 1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV: ` |
| FormatosSignos (`VM:311-316` y `492-510`; :548-575) | «Conjuntos» → «Conjunto» {«Formato: f» {«Rango: r»}; `Lineas: n`; `Global: n` **(imprime `Lineas` en ambos)**} |
| Formatos123 (`VM:319-324` y `514-541`; :578-622) | «Valoración» (`a-b-c` por partido), «Formatos» → «Formato 123: f» {«Límites: min-max» si !PasoFijo}; luego `Aciertos Globales:`+`Modo: No Admitiendo Repeticiones` (PasoFijo) o `Líneas Acertadas:`+`Modo: Admitiendo Repeticiones` |
| Simetrias (`VM:327-337`; :625-639) | «Simetrías» → `Simetría: <partidos>`; `Aciertos: ` |
| Diferencias (`VM:340-345` y `545-564`; :642-678) | «Diferencia N» {`Partidos: …` por grupo; «Diferencias» {Variantes, Equis, Doses, Dibujos, Interrupciones, Formatos = `ObtenerTextoFromBool`}} |

Exportadores (recursivos, `VM:610-657`): **Texto**: por hijo, indentación = `profundidad` espacios; si tiene hijos: línea del nodo + `\r\n\r\n` + subárbol(profundidad+1) + `\r\n`; si es hoja: línea + `\r\n`. **HTML**: por hijo con hijos: en profundidad 0 `<br><b>texto</b><br>`, en 1 `<i>texto</i>`, en ≥2 solo texto; luego `<br>` + subárbol; hojas: `texto<br>`.

**Salidas.** Árbol de nodos de texto; ficheros `Listados.txt`/`.html` (T8: `\r\n` literal en el texto).

**Peculiaridades a preservar (replicadas del legacy).** `Aciertos` de Relaciones muestra `CantidadCP` (`VM:386-388`); `Global` de FormatosSignos muestra `Lineas`; falta `:` en `Interrupciones de 2 `; los nodos «Suma …» de GruposEquipos aparecen tras los conjuntos; las líneas de «Valoración» de Formatos123 usan `Valores.Length/3` (14).

**Casos de uso extraídos (CU-B39).** `ListadoCondiciones.construirArbol(analizador:archivoFiltro:)`, `ListadoCondiciones.exportarTexto(_:)`, `.exportarHtml(_:)`.

**Área Mac propuesta.** *Condiciones y filtros* → inspector lateral (outline view / `OutlineGroup`) con «Exportar…» en la barra. Propuesta; decide el dueño.

### Pantalla 72 · MejoresOpcionesFrm — «Mis mejores opciones»

**Ficheros.** `VM` = `MejoresOpcionesFrmViewModel.cs` (425), `XAML` = `MejoresOpcionesFrmPage.xaml` (116), `CS` = `MejoresOpcionesFrmPage.xaml.cs` (42). Legacy: `Free1X2/UI/MejoresOpcionesFrm.cs` (`button1_Click`, `ObtenGanadoras`, `Escrutar`, `ObtenerResumen`, `OrdenarResumen`, `MostrarResumen`, `AdaptarInterfaz`). Motor: `Domain/Escrutinio/PosiblesPremiosContenedor.cs`, `PosiblesPremiosComparer.cs`.

**Propósito.** A partir de una columna ganadora **parcialmente conocida** (con `*`) y de las columnas jugadas, el usuario marca qué partidos pueden variar; se generan todas las ganadoras posibles, se escruta el fichero contra cada una y se listan las que más premios producen (`VM:44-59`, `XAML:22`).

**Cómo se abre.** Solo desde «Posibles premios» (`PosiblesPremiosFrmViewModel.cs:355-374`): tras `ObtenGanadora()` pasa por navegación un `MejoresOpcionesContexto(columnaGanadora, columnasJugadas, contemplaPleno)` (`VM:21-24`); `OnNavigatedTo` llama `EstablecerContexto` (`CS:32-41`). Sin contexto: ganadora `**************`, sin columnas. Sin menú.

**Entradas.**

| Control | Tipo / defecto | Evidencia |
|---|---|---|
| 16 casillas «Partidos Involucrados» (1..16), visibles solo hasta la longitud de la ganadora | `bool`, defecto false | `VM:27-42,100-142`, `XAML:44-63` |
| «Mostrar los _ primeros resultados» | `NumberBox` Min 1, defecto 10; `<1` se restablece a 10 | `VM:87-92,154-159`, `XAML:72-77` |
| «Calcular» | — | `XAML:82-86` |

**Acciones.** **Calcular** (`VM:152-204`): `Resumen="Calculando..."`; en segundo plano: `ObtenGanadoras`, `ObtenerResumen` por cada ganadora, `OrdenarResumen`, `MostrarResumen`; error ⇒ diálogo «No se pudo calcular las mejores opciones:\n\n<msg>» y `Resumen="No se pudo completar el cálculo."`. El texto se publica en el hilo de UI.

**Lógica de negocio en el ViewModel** (`VM:210-407`):

```
ObtenGanadoras(ganadora, involucrados[16], pre="", p=0):              // recursivo; añade a cGanadoras
   para i en 0..<3 con signos ["1","X","2"]:
      si ganadora[p] != "*":  nuevo = pre + ganadora[p] ; (sale del bucle tras esta vuelta)
      si no, si involucrados[p]: nuevo = pre + signos[i]
      si no:                      nuevo = pre + "*"        ; (sale del bucle tras esta vuelta)
      si p < len-1: ObtenGanadoras(…, nuevo, p+1)  sino cGanadoras.append(nuevo)
   // nº de ganadoras = 3^(nº de '*' involucrados); las '*' no involucradas permanecen '*'

Escrutar(analizada, ganadora, noPartidos, contemplaPleno) -> Int:     (VM:250-281)
   aciertos=0 ; posibles=noPartidos
   para i en 0 ..< analizada.count-1:           // ¡compara todas menos la ÚLTIMA posición!
        si posibles < 10: break
        si analizada[i]==ganadora[i] o ganadora[i]=="*": aciertos++  sino posibles--
   si contemplaPleno y aciertos == analizada.count-1:
        si analizada.last == ganadora.last o ganadora.last=="*": aciertos++
   return aciertos

ObtenerResumen(ganadora): contenedor = PosiblesPremiosContenedor()                 (VM:287-348)
   para cada columna jugada: a = Escrutar(...) ; si a > 9: contenedor.ColGanadora = ganadora
        a==10: Col10.add ;  a==11: Col11.add (+ si pleno && columna.count==11: Col14.add)
        a==12: Col12.add (+ si pleno && count==12: Col14.add) ; a==13: Col13.add (+ si pleno && count==13: Col14.add)
        a==14: Col14.add (+ si pleno && count==14: Col14.add OTRA VEZ)            // doble conteo
        a==15: Col15.add (+ si pleno && count==15: Col14.add) ; a==16: Col16.add (+ si pleno && count==16: Col14.add)
   listaResumen.append(contenedor)          // se añade SIEMPRE, aunque no haya ningún acierto >9 (ColGanadora = null)

OrdenarResumen: listaResumen.Sort( PosiblesPremiosComparer )   // orden DESCENDENTE por |Col16|, |Col15|, |Col14|, |Col13|, |Col12|, |Col11|, |Col10| (Domain PosiblesPremiosComparer.cs:12-47)
                // List.Sort no es estable (T5): el orden entre contenedores con idénticos recuentos lo fija la implementación de .NET

MostrarResumen(límite, ganadora):                                                   (VM:359-407)
   "Mis mejores opciones son: \r\n------------------------\r\n\r\n"
   para los primeros `límite` contenedores: ColGanadora + ": "   (null -> "")
       si len(ganadora)>=16: "{|Col16|} de 16 + "  ; >=15: "{|Col15|} de 15 + " ; >=14: "… de 14 + " ; >=13 ; >=12 ; >=11: "… de 11 + " ; >=10: "{|Col10|} de 10"   ; + "\r\n"
```

**Salidas.** Texto multilínea de solo lectura (`TextBox`), p. ej. `1X1*X2…: 0 de 16 + 0 de 15 + 3 de 14 + 12 de 13 + … + 40 de 10` (sin formato numérico).

**Peculiaridades a preservar.**
- `Escrutar` ignora la última posición de la columna analizada salvo que `ContemplaPleno` (`VM:254,270-279`): con columnas de 14 signos y sin pleno, el máximo de aciertos contables es 13.
- La condición `cAnalizada.Length == aciertos` para volcar a `Col14` con pleno es una heurística del legacy para columnas más cortas; se replica sin interpretarla.
- Coste: `3^k` ganadoras × nº de columnas × 14 posiciones (k=14 ⇒ 4,78 M ganadoras); no hay límite ni cancelación (A-72-1).
- El `PosiblesPremiosContenedor` y el comparador están en el dominio (no se redefinen en Swift: usar los de `Free1X2Core`).

**Casos de uso extraídos (CU-B40).** `MejoresOpciones.generarGanadoras(ganadora:involucrados:)`, `.escrutar(analizada:ganadora:contemplaPleno:)`, `.resumen(ganadoras:columnas:)`, `.ordenar(_:)`, `.formatearResumen(limite:)`.

**Área Mac propuesta.** *Escrutinio* → hoja modal/inspector abierto desde «Posibles premios». Propuesta; decide el dueño.

### Pantalla 73 · ModificadorFrm — «Modificador de columnas» (modificador de porcentajes)

**Ficheros.** `VM` = `ModificadorFrmViewModel.cs` (430), `XAML` = `ModificadorFrmPage.xaml` (187), `CS` = `ModificadorFrmPage.xaml.cs` (21, sin lógica). Legacy: `Free1X2/UI/ModificadorFrm.cs` y control `Free1X2/UI/Controls/ModificadorOptions` (`AbrirClick`, `BtnOkClick`, `BtnGrabarClick`, `LeeFichero`, `ModificarFrecuencia`, `AccionModificador`, `Ordenar`). No usa clases del dominio salvo `ArchivoColumnasTexto`.

**Propósito.** Cambia la frecuencia de signos `1/X/2` de uno o más partidos en un fichero de columnas, redistribuyendo los signos entre las columnas según un modo (aleatorio / proporcional / ordenado) (`VM:51-57`, `docs/MANUAL_USUARIO.md:368-374`).

**Cómo se abre.** Menú Utilidades «Modificador %…» (`MainWindow.xaml.cs:252`) y botón `bModificadorPct` (`:418`). Autónoma.

**Entradas.**

| Control | Tipo / defecto / rango | Evidencia |
|---|---|---|
| «Abrir fichero…» | Abrir `.txt` (DocumentsLibrary); `_noPartidos = ObtenNumSignos()`, `_apuestas = (int)ObtenNumCols()` | `VM:128-159` |
| Filas de partido (14, o las que tenga el fichero): casilla de activar + `% 1`, `% X`, `% 2` | `NumberBox` 0..100; solo editables si el partido está activo | `VM:21-49,213-219`, `XAML:78-110` |
| «Modo» (Distribución de los signos) | `Aleatorio` (=1) / `Proporcional` (=2, defecto) / `Ordenado` (=3) | `VM:94-126` |
| «Ordenar signos» | toggle, defecto Sí; deshabilitado con «Aleatorio» | `VM:105-113`, `XAML:154-159` |
| «Ok» / «Grabar» / «Cancelar» | Ok y Grabar solo con fichero cargado; Cancelar sin efecto | `VM:161-209`, `XAML:166-179` |

**Acciones.**
- **Abrir** (`VM:129-159`): `LeeFichero` en segundo plano; `ReconstruirPartidos`; `EscribirFrecuencias` (porcentajes enteros por partido); `FicheroNombre`; `FicheroCargado=true`. Error ⇒ «Error al leer el archivo: <msg>».
- **Ok** (`VM:162-174`): por cada partido **activo** `ModificarFrecuencia(partido, índice)` y luego `EscribirFrecuencias()`.
- **Grabar** (`VM:177-203`): Guardar sugerido `<nombre>_modificado`, tipo «Columnas» `.txt`; `ArchivoColumnasTexto.GuardarTodasCols(columnas)` (sin comas); éxito ⇒ «Se ha grabado el archivo.»; error ⇒ «Error al grabar: <msg>».

**Lógica de negocio en el ViewModel** (`VM:233-428`):

```
LeeFichero(ruta):                                                              (VM:233-275)
   columnas = String[apuestas] ; frecuencia[nPartidos][3] = 0
   por cada línea (LeeColumnaSinComas = Trim().ToUpper()): columnas[k]=línea ; para i<nPartidos: según columna[i]: '1'->f[i][0]++, 'X'->f[i][1]++, '2'->f[i][2]++ ; k++
   porcentajes: f[i][j] = ToInt32( f[i][j]*100/Double(apuestas) + 0.4 )        // redondeo al par (ToEven) de (x+0,4)
        mientras suma < 100: f[i][0]++   ;   mientras suma > 100: si f[i][2]>0 f[i][2]-- sino si f[i][1]>0 f[i][1]--

Ordenar(pct[3]) -> "123"-orden de mayor a menor, con TRES bloques `if` consecutivos (no else): el ÚLTIMO bloque cuyo criterio se cumple SOBREESCRIBE   (VM:405-428)
   if p0>=p1 && p0>=p2: orden = '1' + (p1>=p2 ? "X2" : "2X")
   if p1>=p0 && p1>=p2: orden = 'X' + (p0>=p2 ? "12" : "21")
   if p2>=p1 && p2>=p0: orden = '2' + (p0>=p1 ? "1X" : "X1")
   // p. ej. (50,50,0) -> "X12"; todo igual -> "21X"

ModificarFrecuencia(partido, k):                                               (VM:278-371)
   pct = [Valor1, ValorX, Valor2]
   suma = Σpct ; si suma != 100 y suma != 0: pct[j] *= 100/suma                // normaliza (el legacy avisaba con MessageBox)
   inicial[j] = ToInt32( Double(apuestas) * pct[j] / 100 )                      // redondeo al par
   mientras Σinicial < apuestas: inicial[0]++ ; mientras Σinicial > apuestas: inicial[2]--
   opcion = 1|2|3 ; temporal = Ordenar(pct)
   si OrdenarSignos && opcion>1: signo = temporal (3 chars) ; total[i] = inicial[ índice del signo[i] ('1'->0,'X'->1,'2'->2) ]
   si no:                         signo = ['1','X','2'] ; total = inicial
   opcion 1 (aleatorio): para cada columna c en 0..<apuestas:  repetir a = ToInt32(pow(Double(ms_actual), 3)) % 3  hasta total[a]>0 ;
                         columnas[c] = CambiarSigno(columnas[c], k, signo[a]) ; total[a]--            // usa DateTime.Now.Millisecond: NO determinista
   opcion 2 (proporcional): grupo[i] = (total[2]==0) ? (total[1]==0 ? total[i] : total[i]/total[1]) : total[i]/total[2]   (división entera) ; ProcesarGrupos
   opcion 3 (ordenado):     grupo[i] = total[i] ; ProcesarGrupos
   frecuencia[k][j] = ToInt32(pct[j]) ; partido.Activo = false

ProcesarGrupos(total, grupo, signo, k):                                        (VM:373-392)
   procesados=0 ; mientras procesados < apuestas:
        para i en 0..2: quedan = min(grupo[i], total[i])
             mientras quedan>0 && total[i]>0: columnas[procesados] = CambiarSigno(columnas[procesados], k, signo[i]) ; quedan-- ; total[i]-- ; procesados++
CambiarSigno(col, p, s): reconstruye la columna con el carácter p sustituido por s (nPartidos caracteres)
```

**Salidas.** Filas de porcentaje (enteros 0..100) tras cada acción; fichero `.txt` con las columnas modificadas (una por línea, UTF-8 sin BOM).

**Peculiaridades a preservar / avisos.**
- **Bucle sin fin latente**: con «Proporcional» y «Ordenar signos» desactivado, si el signo `2` es el más frecuente, `grupo` de los otros signos vale 0 y `ProcesarGrupos` no avanza cuando se agota `total[2]` (p. ej. total=[10,5,20] ⇒ grupo=[0,0,1]) (`VM:350-360,377-391`). Con orden activado (defecto) `total[2]` es el menor y siempre hay progreso. Decisión del dueño sobre el guarda (A-73-1).
- `ObtenNumCols()` es una **estimación por tamaño** (`/(longitud+2)`): con fin de línea `\n` (macOS) o sin salto final el nº de apuestas se subestima y `columnas[k]` desborda ⇒ excepción capturada «Error al leer el archivo» (`VM:141-146,238,243`; `ArchivoColumnasTexto.cs:314-338`, A-66-1). En Swift contar líneas exactas.
- El modo aleatorio depende del reloj (milisegundos); `pow(ms,3) % 3 == ms % 3`. Tests deben inyectar un generador.
- El modo «Aleatorio» ignora «Ordenar signos» y siempre usa `1,X,2` (`VM:332-337`).
- Los partidos activos se procesan secuencialmente sobre las columnas ya modificadas, sin comprobar duplicados (el manual recomienda pasar luego «Álgebra → eliminar repetidas», `docs/MANUAL_USUARIO.md:374`).
- Redondeos: `Convert.ToInt32(double)` = al par más próximo (T4) en `LeeFichero`, `inicial` y `frecuencia`.

**Casos de uso extraídos (CU-B41).** `ModificadorPorcentajes.leer(url:) -> (columnas, frecuencias)`, `.modificar(columnas:partido:porcentajes:modo:ordenar:rng:)`, `.ordenarSignos(_:)`, `.exportar(url:)`.

**Área Mac propuesta.** *Herramientas* → vista principal de herramienta (tabla de 14 filas + modo + exportar). Propuesta; decide el dueño.

---

## 2. Tabla resumen

### 2.1 Pantalla → área Mac → casos de uso

Áreas y tipos de presentación son **propuestas** (R2). «CU» = casos de uso extraídos al paquete `Free1X2Core/CasosDeUso` (catálogo en §2.3). Ninguna de estas pantallas queda sin caso de uso: las que no llaman al motor tienen su lógica en el ViewModel y esa es la que se extrae.

| Nº | Pantalla WinUI (ViewModel) | Cómo se abre | Área Mac propuesta | Presentación | Casos de uso |
|---:|---|---|---|---|---|
| 41 | `DifColsViewModel` | Menú Utilidades → Diferencias entre columnas…; botón `bDiferenciasColumnas` | Columnas probables | Hoja / ventana de herramienta | CU-B01 |
| 42 | `DiferenciasFrmViewModel` | Rejilla de condiciones de Inicio → «Diferencias» | Condiciones y filtros | Inspector / hoja modal | CU-B02 |
| 43 | `DistanciasFrmViewModel` | Rejilla de Inicio → «Distancias» | Condiciones y filtros | Inspector / hoja modal | CU-B03 |
| 44 | `EscrutarCombinacionesFrmViewModel` | Menú Combinación → Escrutar combinaciones… (el botón de barra está deshabilitado) | Escrutinio | Vista principal (pestaña) | CU-B04, CU-B05, CU-B06 |
| 45 | `EscrutiniosFrmViewModel` | Menú Combinación → Escrutinios…; botón `bEscrutinio` | Escrutinio | Vista principal | CU-B07, CU-B08 |
| 46 | `EstimadorPremiosFrmViewModel` | Menú Utilidades → Estimación de premios…; botón `bEstimacion` | Escrutinio (o Análisis) | Vista principal | CU-B09, CU-B10 |
| 47 | `EstucolFrmViewModel` | Menú Utilidades → EstuCol… (sin botón) | Columnas probables | Hoja de herramienta | CU-B11 |
| 48 | `ExportadorCPsFrmViewModel` | Desde el editor de CPs (Exportar) | Columnas probables | Sin pantalla propia (menú «Exportar…») | CU-B12 |
| 49 | `FigurasFiltrosFrmViewModel` | Desde Contactos y Signos seguidos | Condiciones y filtros | Hoja modal secundaria | CU-B13, CU-B14 |
| 50 | `FiltroPorcenJBViewModel` | Desde Generar CPs (sin menú) | Herramientas | Vista principal | CU-B15, CU-B16, CU-B17 |
| 51 | `Formatos123FrmViewModel` | Rejilla de Inicio → «Formatos 123» | Condiciones y filtros | Inspector / hoja modal | CU-B18 |
| 52 | `FormatosFrmViewModel` | Rejilla de Inicio → «Formatos» | Condiciones y filtros | Inspector / hoja modal | CU-B19 |
| 53 | `FraccionadorFrmViewModel` | Menú Operaciones → Fraccionador…; botón `bFraccionador` | Herramientas | Hoja / ventana de utilidad | CU-B20 |
| 54 | `FrmDependenciaLinealViewModel` | Botón `bDependenciaLineal` (barra Utilidades; sin menú) | Herramientas | Ventana de utilidad | CU-B21 |
| 55 | `FrmReducidasPerfectasViewModel` | Botón `bRedPerfectas` (barra Utilidades; sin menú) | Reducción | Hoja o vista secundaria | CU-B22 |
| 56 | `GEPTFrmViewModel` | Menú Utilidades → Columnas GEPT…; botón `bColumnasGEPT` | Columnas probables | Hoja / popover | CU-B23 |
| 57 | `GeneraPimViewModel` | Menú Filtros → Filtro Pim…; botón `bFiltroPim` | Herramientas | Vista principal | CU-B24, CU-B25 |
| 58 | `GeneradorCPSDiferenciasViewModel` | Desde Generar CPs | Columnas probables | Hoja modal | CU-B26 |
| 59 | `GenerarCPsViewModel` | Menú Utilidades → Generador CP…; botón `bGeneradorCPs` | Columnas probables | Vista principal | CU-B27 |
| 60 | `GestorEquiposFrmViewModel` | Menú Archivo → Gestión de equipos…; botón `bGestorEquipos` | Boleto | Hoja modal | CU-B28 |
| 61 | `GraficoColumnasFrmViewModel` | Menús Ver y Combinación → Gráfico de columnas… | Análisis | Vista principal | CU-B29 |
| 62 | `GruposEquiposFrmViewModel` | Rejilla de Inicio → «Grupos Equipos» | Condiciones y filtros | Hoja / inspector grande | CU-B30 |
| 63 | `GuardarValoracionFrmViewModel` | Desde Valoración | Análisis | Hoja modal | CU-B31 |
| 64 | `HistoriaValoracionesFrmViewModel` | Desde Tramificar («Guardar en histórico») | Análisis | Hoja modal | CU-B32 |
| 65 | `IfThenFrmViewModel` | Botón «If-Then» de Inicio | Condiciones y filtros | Vista principal secundaria | CU-B33 |
| 66 | `ImportExportFrmViewModel` | Menú Archivo → Importar / exportar columnas…; botón `bImportExport` | Herramientas | Hoja modal pequeña | CU-B34 |
| 67 | `ImportadorCPsFrmViewModel` | Desde el editor de CPs (Importar) | Columnas probables | Sin pantalla propia (acción «Importar…») | CU-B35 |
| 68 | `ImprimirBoletoFrmViewModel` | Menú Combinación → Imprimir boletos…; botón `bImprimirBoletos` | Boleto | Hoja de impresión | CU-B36 |
| 69 | `InterrupcionesFrmViewModel` | Rejilla de Inicio → «Interrupciones» | Condiciones y filtros | Inspector / hoja modal | CU-B37 |
| 70 | `ListaImpresorasViewModel` | Desde Imprimir boletos («Impresoras conocidas») | Boleto | Menú desplegable en la hoja de impresión | CU-B38 |
| 71 | `ListadoCondicionesFrmViewModel` | Menú Ver → Listado de condiciones | Condiciones y filtros | Inspector lateral | CU-B39 |
| 72 | `MejoresOpcionesFrmViewModel` | Desde Posibles premios | Escrutinio | Hoja / inspector | CU-B40 |
| 73 | `ModificadorFrmViewModel` | Menú Utilidades → Modificador %…; botón `bModificadorPct` | Herramientas | Vista principal | CU-B41 |

### 2.2 Recuento por área Mac propuesta

| Área | Pantallas | Casos de uso |
|---|---:|---:|
| Columnas probables | 7 | 7 |
| Condiciones y filtros | 9 | 10 |
| Escrutinio | 4 | 8 |
| Herramientas | 6 | 9 |
| Reducción | 1 | 1 |
| Boleto | 3 | 3 |
| Análisis | 3 | 3 |
| **Total** | **33** | **41** |

La pantalla 46 figura bajo *Escrutinio* (propuesta alternativa: *Análisis*). Recuento de «casos de uso» = tipos distintos nombrados en el apartado «Casos de uso extraídos» de cada pantalla (un tipo agrupa sus operaciones), sin contar los tipos del motor que solo se reutilizan (`CalculadorEstadisticas`, `LAE`, `Porcentajes`, `Free1X2Online`).

### 2.3 Catálogo de casos de uso

Todos son funciones puras o `async` cancelables sobre valores explícitos (`Sesion`, `Configuracion`), sin acceso a UI (M-R4, T10, T12). Entradas, salidas y algoritmo: ver la sección de la pantalla indicada.

| ID | Caso de uso (nombre Swift propuesto) | Pantalla | Resumen |
|---|---|---:|---|
| CU-B01 | `ColumnasPorDiferencias` | 41 | Genera columnas (14 triples o fichero) y acepta las que cumplen rangos de diferencias contra la base/condiciones y contra las ya aceptadas; exporta `.txt`. |
| CU-B02 | `CondicionDiferencias` | 42 | Construye y vuelca el `FiltroDiferencias` desde/hacia los textos de pantalla; navegación de conjuntos; atajos de agrupación; estadísticas. |
| CU-B03 | `CondicionDistancias` | 43 | Normaliza los cuatro textos (vacío ⇒ 0..14), activa o desactiva el filtro, vuelca a pantalla; estadísticas; persistencia `.dist`. |
| CU-B04 | `EscrutinioCombinaciones` | 44 | Escruta ficheros de combinaciones (simple, contra fichero, contra jornadas) y exporta la selección. |
| CU-B05 | `HistoricoJornadas` | 44 | Carga el histórico de resultados y las temporadas disponibles. |
| CU-B06 | `PlantillaJornadas` | 44 | Resuelve ficheros por plantilla y dígitos; deduce temporada y jornada del nombre. |
| CU-B07 | `EscrutinioColumnas` | 45 | Escruta un fichero de columnas contra ganadora/histórico, proyecta resultados y exporta la selección. |
| CU-B08 | `ColumnasPremiadas` | 45 | Construye las filas de columnas premiadas por boleto (`ceil(n/8)`). |
| CU-B09 | `EstimadorPremios` | 46 | Estima acertantes y premios por categoría a partir de porcentajes, columna, apuestas, recaudación y bote. |
| CU-B10 | `HistoricoLAE` | 46 | Carga y guarda el escrutinio histórico de una jornada (reutiliza `LAE`). |
| CU-B11 | `EstuCol` | 47 | Empareja reducidas, escruta contra ganadoras y genera los informes ABDON. |
| CU-B12 | `ColumnasProbables.exportar` | 48 | Exporta CPs simples o con aciertos (`.clm`). |
| CU-B13 | `Figuras` | 49 | Parsea y formatea figuras (`Int64`) y lee ficheros de figuras. |
| CU-B14 | `EditorFiguras` | 49 | Aplica las casillas del editor a la lista de figuras en edición. |
| CU-B15 | `SeparadorPorcentajesJB` | 50 | Prepara las bandas de porcentaje, filtra columnas, analiza la ganadora y exporta signos por banda. |
| CU-B16 | `RangosJB` | 50 | Guarda y lee los rangos en `.jb7`. |
| CU-B17 | `GanadorasNavegador` | 50 | Carga y recorre las columnas ganadoras. |
| CU-B18 | `CondicionFormatos123` | 51 | Vuelca formatos y aciertos al `FiltroFormatos123`; transforma la valoración; traduce columnas; estadísticas; persistencia `.123`. |
| CU-B19 | `CondicionFormatosSignos` | 52 | Construye y vuelca las relaciones de formatos; navegación de relaciones; estadísticas; persistencia `.fmt`. |
| CU-B20 | `FraccionarColumnas` | 53 | Reparte un fichero de columnas en varios por cuotas o por tramos. |
| CU-B21 | `DependenciaLineal` | 54 | Lee columnas, propone coeficientes, aplica la dependencia lineal sobre un partido y exporta. |
| CU-B22 | `ReduccionPerfecta` | 55 | Genera la reducción perfecta (4/13/11 triples, 7/15 dobles) con las matrices fijas. |
| CU-B23 | `ColumnaGEPT` | 56 | Traduce dos columnas a la columna GEPT y calcula la tabla por partido. |
| CU-B24 | `FiltroPim` | 57 | Prepara las columnas probables por ranking, filtra un fichero, analiza la ganadora y exporta (variante a decidir, A-57-1). |
| CU-B25 | `RangosPim` | 57 | Guarda y lee los rangos de Filtro Pim (`.cnd`). |
| CU-B26 | `CPsPorDiferencias` | 58 | Genera CPs combinando dos columnas según diferencias y tipo; exporta. |
| CU-B27 | `GeneradorCP` | 59 | Genera los ficheros de CPs a partir de la valoración; importa porcentajes; serializa. |
| CU-B28 | `CatalogoEquiposLocal` | 60 | Carga, guarda, da de alta, mueve y elimina equipos; importa catálogo online. |
| CU-B29 | `GraficoCombinacion` | 61 | Calcula líneas, rellenos, mínimo y máximo de la representación gráfica y su guía. |
| CU-B30 | `CondicionGruposEquipos` | 62 | Copia de trabajo de grupos y relaciones de equipos; navegación; confirmar sobre el filtro; estadísticas; persistencia `.geq`. |
| CU-B31 | `Valoraciones.guardar` | 63 | Guarda la matriz de valoración en los formatos 1, 3 y 42. |
| CU-B32 | `HistoricoValoraciones` | 64 | Guarda la valoración de una jornada en el histórico y detecta el formato del fichero. |
| CU-B33 | `IfThenEditor` | 65 | Compone condiciones de texto, construye el `ControladorIfThen` y carga/guarda `.if`. |
| CU-B34 | `ConversorColumnas` | 66 | Convierte columnas entre CSV y TXT y cuenta columnas. |
| CU-B35 | `ImportadorCP` | 67 | Lee CPs simples o con estadísticas desde fichero. |
| CU-B36 | `ImpresionBoleto` | 68 | Calcula las marcas por boleto, lee/guarda `imprebol.cfg` y propone el rango por defecto. |
| CU-B37 | `CondicionInterrupciones` | 69 | Aplica y vuelca los diez textos de interrupciones; estadísticas; persistencia `.int`. |
| CU-B38 | `ImpresorasConocidas` | 70 | Carga `impresoras.cfg` y busca por modelo. |
| CU-B39 | `ListadoCondiciones` | 71 | Construye el árbol de condiciones del análisis y lo exporta a texto/HTML. |
| CU-B40 | `MejoresOpciones` | 72 | Genera ganadoras posibles, escruta, resume, ordena y formatea el resumen. |
| CU-B41 | `ModificadorPorcentajes` | 73 | Lee columnas y frecuencias, modifica un partido según porcentajes y modo, ordena signos y exporta. |

## 3. Abiertas

Dudas, defectos aparentes y puntos no verificados. Ninguno se resuelve aquí: los de tipo «Divergencia» o «Decisión» exigen decisión del dueño (R2) y los «No verificado» no se afirman (R4). Cada uno remite a la sección de la pantalla donde está la evidencia. Primero las 41 de pantalla (A-NN-k) y después las globales (G-n).

| ID | Pantalla | Asunto | Tipo | Detalle y evidencia |
|---|---:|---|---|---|
| A-41-1 | 41 | Máscara con `F` en modo «14 triples» | Divergencia | Con «14 triples» y columna base con `F`, el port usa máscara todo-`V` (`DifColsViewModel.cs:342`) y por tanto cuenta esas posiciones; el legacy usa la máscara de la **última `VerColumna`** (campo `mask`, `Free1X2/UI/DifCols.cs:69,176-205`): ignora esos partidos en todas las comparaciones y escribe `F` en la salida. Decidir si el oráculo es el port o el legacy. |
| A-41-2 | 41 | Cancelar conserva (legacy) o descarta (port) las aceptadas | Divergencia | Cancelar: el legacy conserva las aceptadas parciales (`UI/DifCols.cs:94,149,165,189`; solo `Clear()` al inicio de Calcular) y permite Grabar; el port descarta el parcial y deja la lista de la ejecución anterior (`DifColsViewModel.cs:171-181`). |
| A-41-3 | 41 | Botón «CPs por diferencias…» sin cablear | Defecto aparente | El botón «CPs por diferencias…» está sin cablear en el port (`DifColsViewModel.cs:252-256`; `DifColsPage.xaml.cs` no navega). Legacy abría `GeneradorCPSDiferencias`. |
| A-42-1 | 42 | Formato exacto de `.dif`/`.xml` | No verificado | No se ha verificado el formato exacto de `.dif`/`.xml` (`ArchivoCondiciones`); lo cubre ESPEC_07. |
| A-43-1 | 43 | La ayuda promete espacios; el parser no los admite | Defecto aparente | La ayuda de la pantalla dice «Separa varios valores con espacios o comas» y el placeholder es `0 1 2` (`DistanciasFrmPage.xaml:21-23,35`), pero `SetNoInt1` hace `Split(',')` + `Convert.ToInt32` (`FiltroDistancias.cs:493-519`): con espacios lanza `FormatException`. Qué hace el WinUI al propagarse la excepción (manejador global) no verificado. Decidir si la UI Mac valida/acepta espacios. |
| A-44-1 | 44 | Comodín `*` convertido a `X` | Decisión | El comodín `*` de la columna ganadora se convierte a `X` en `ConvertidorDeBases.ConvColumnaANumero` (`Domain/Utils/ConvertidorDeBases.cs:43-53`): ¿es intencional (comodín «a X») o bug heredado? En Swift se replica el comportamiento actual (tratar como `X`) salvo decisión contraria. |
| A-44-2 | 44 | Carpeta inicial del diálogo «Fichero a escrutar» | Decisión | El diálogo «Fichero a escrutar» abre en `DocumentsLibrary` (`PickerHelper.AbrirVariosAsync`, `EscrutarCombinacionesFrmViewModel.cs:272`), mientras el legacy abría en `Columnas\` (comentario `VM:271,287`). Carpeta inicial en Mac: pendiente de decidir. |
| A-45-1 | 45 | `Detener` sin control enlazado | Defecto aparente | `DetenerCommand` existe (`EscrutiniosFrmViewModel.cs:831-838`) pero ningún control del XAML lo enlaza; el legacy cambiaba el botón a «Parar escrutinio!» (`:834`). Solo «Cancelar» detiene (y sale). Decidir si la UI Mac expone «Detener». |
| A-45-2 | 45 | `S: Suspendido` anunciado y no tratado | No verificado | El XAML anuncia `S: Suspendido` (`EscrutiniosFrmPage.xaml:140`) pero `Escrutador.EscrutaColumna` solo trata `*` como comodín (`Domain/Escrutinio/Escrutador.cs:176-179`): `S` solo acierta si la columna del fichero también tiene `S`. No verificado si el legacy lo trataba distinto. |
| A-45-3 | 45 | La fila `TOTALES` es seleccionable y exportable | Decisión | La fila `----- TOTALES:` es seleccionable y se graba como si fuera una columna (`EscrutiniosFrmViewModel.cs:873-876`, `Escrutador.cs:262-275`). ¿Excluirla en Mac o replicar? |
| A-46-1 | 46 | Redondeo bancario en el Estimador | No verificado | `Calcular` y `RecargarDatosJornada` usan `Math.Round(x, n)` con `ToEven` (`EstimadorPremiosFrmViewModel.cs:177-186,326`); la réplica Swift exacta (`(x*10^n).rounded(.toNearestOrEven)/10^n`) debe confirmarse con golden (M1). No verificado en este repo. |
| A-46-2 | 46 | Acertantes cargados dependen de la recaudación previa | No verificado | En `RecargarDatosJornada`, `destinado[]` se calcula con la Recaudación *anterior* a cargar la de la jornada (`EstimadorPremiosFrmViewModel.cs:282-284` antes de `:316`): los acertantes cargados dependen de la recaudación previa de la pantalla. ¿Intencional (así era el `TextChanged` legacy)? No verificado contra `EstimadorPremiosFrm.cs`. |
| A-46-3 | 46 | «Destinado a premios» sin uso | Código muerto | El campo «Destinado a premios» (55 %) no se usa en ningún cálculo (`EstimadorPremiosFrmViewModel.cs:76`; solo se declara). |
| A-46-4 | 46 | Los premios guardados no son los tecleados | Decisión | Los premios guardados por `LAE.GrabarJornada` provienen de configuración y 10 % fijo, no de la pantalla; el usuario puede esperar que se guarde lo tecleado (`LAE.cs:50-53,141-148`). |
| A-47-1 | 47 | Líneas con caracteres distintos de `1/X/2` | Defecto aparente | Líneas con caracteres no `1/X/2` (p. ej. columnas con comas o `x` minúscula) producen en C# `1 << -1` (= `1 << 31`) en `UtilColumnas.ConvStrToLong` (`Domain/Utils/UtilColumnas.cs:201`). ¿Se replica o se valida la entrada? No verificado qué formato escribe `ArchivoColumnasTexto` (con/sin comas) — ESPEC_07. |
| A-49-1 | 49 | El editor promete 0–16 y `*` | Defecto aparente | El editor promete «0 a 16 o comodín *» (`FigurasFiltrosFrmViewModel.cs:109-110`) pero el código descarta `*` y no puede representar 16 (`:113-123,139-142`). ¿Qué valores debe aceptar la UI Mac? (Replicar el comportamiento real o validar al teclear.) Tampoco verificado si el legacy admitía `*` (`Free1X2/UI/Filtros/FigurasFiltrosFrm.cs`). |
| A-50-1 | 50 | Cálculo en hilo de UI y «Cancelar» inoperante durante el cálculo | No verificado | `CalcularAsync` ejecuta el bucle de lectura/validación sin `await` interno tras el selector de fichero (`FiltroPorcenJBViewModel.cs:163-196`); se infiere que corre en el hilo de UI y que «Cancelar» (`:228-233`) no puede ejecutarse durante el cálculo. No ejecutado/verificado. El legacy usaba `salida` con `DoEvents` (no revisado). |
| A-50-2 | 50 | Sin entrada de menú | Decisión | No hay entrada de menú; se llega desde el Generador de CPs (`GenerarCPsViewModel.cs:205`). Pendiente de decidir si en Mac es herramienta independiente. |
| A-51-1 | 51 | Dos implementaciones de `TransformarValoracion` | Decisión | Hay dos implementaciones de `TransformarValoracion`: la del dominio, con cadenas (`FiltroFormatos123.cs:332-385`, usada por `Col1X2ToCol123`, `:93-96`), y la copia del ViewModel, con bits 4/2/1 (`Formatos123FrmViewModel.cs:347-400`, que `Aceptar` guarda en `ValoracionTranformada`, `:318`). Comparadas rama a rama son equivalentes para valores no NaN. En Swift conviene una sola implementación y conservar el resultado con NaN (fila sin asignar, `null`/`0`). `Formato123` no sobreescribe `Equals` (`Domain/MotorCalculo/Formato123.cs`, sin coincidencias), así que `lista.Contains` del VM (`:217`) nunca deduplica. |
| A-53-1 | 53 | El reparto por tramos pierde columnas sobrantes | Defecto aparente | El reparto «por tramos» pierde las columnas sobrantes cuando `N − part·q ≥ part` (`FraccionadorFrmViewModel.cs:202-212`; idéntico en legacy `UI/FraccionadorFrm.cs:150-167`). Replicar (M-R1) o corregir como mejora: decisión del dueño. |
| A-54-1 | 54 | Salida por defecto = fichero de entrada | Decisión | Salida por defecto = fichero de entrada (sobrescritura sin aviso, `FrmDependenciaLinealViewModel.cs:151-155`). ¿Mantener o pedir siempre un destino en Mac? (R2). |
| A-54-2 | 54 | Comparación con el legacy incompleta | No verificado | `LeerColumnas` cuenta cada línea del fichero aunque la columna esté repetida; el legacy no se ha comparado línea a línea (`Free1X2/UI/FrmDependenciaLineal.cs` no leído al detalle). |
| A-55-1 | 55 | Dígitos por redondeo en la reducción perfecta | Defecto aparente | La extracción de «dígitos» con `Convert.ToInt32(i / Math.Pow(3, z))` (redondeo, no truncado) es idéntica en legacy y port (`FrmReducidasPerfectasViewModel.cs:344`, `UI/FrmReducidasPerfectas.cs:1543`). No se ha comprobado si las reducciones resultantes siguen siendo «perfectas» (cobertura al 13/12) ni si el autor pretendía truncar. Replicar tal cual y añadir golden en M1; el dueño decide si se corrige. |
| A-57-1 | 57 | Motor de «Filtro Pim» = motor de `aidomnou` (bloqueante) | Divergencia | **Bloqueante para la reescritura.** `GeneraPimViewModel` implementa el motor de `aidomnou` (6 columnas PreCol1..6 + suma, `GeneraPimViewModel.cs:15-26,534-799`), mientras que la clase legacy `GeneraPim` (`Free1X2/UI/FiltroPim.cs:221-248,413-466`) genera 7 columnas por rondas de 6 máximos y no tiene suma ni límites. La pantalla muestra R1..R7 y recorrido como el legacy. Decidir si el oráculo de «Filtro Pim» es el port actual (idéntico a «Filtro Aidomnou», pantalla `aidomnouPage`) o el legacy; en el segundo caso hay que reescribir la lógica desde `FiltroPim.cs` (golden nuevos) y corregir el port. Consecuencia deducida por lectura (no ejecutada): con los rangos por defecto del port, R7 (suma) `0-1` (`GeneraPimViewModel.cs:48-49`) es inalcanzable porque `Valida` exige `sum6 ∈ R7` (`:795`) y R1..R5 ya exigen una suma mínima de 3+2+2+2+2 = 11 (`:30-43`); con los valores por defecto «Calcular» admite 0 columnas. Los defectos provienen de los 7 rankings del legacy (`FiltroPim.cs:1481,1493`). |
| A-57-2 | 57 | `aidomnou.cfg` no portado | Decisión | `aidomnou.cfg` (límites 145/11,9,8/65/9,6,5) se lee/escribe en el legacy (`Free1X2/UI/Aidomnou.cs:172-226`) pero el port usa constantes (`GeneraPimViewModel.cs:114-117`). Documentado en ESPEC_07 como dato inicial; falta decidir si la UI Mac los expone. |
| A-58-1 | 58 | Desalineamiento de `cM` | Defecto aparente | El desalineamiento de `cM` cuando `c1[i]==c2[i]` (`GeneradorCPSDiferenciasViewModel.cs:251-260`) es idéntico al legacy; ¿se replica (M-R1) o se corrige? No se ha verificado el resto del legacy línea a línea (solo `combinarDobles1` inicio). |
| A-59-1 | 59 | `Porcentajes` y tablas de CPs no leídas a fondo | No verificado | `Porcentajes` y `ColumnasProbables.xml` (tablas «Tipos de CPs» / «Configuracion de CPs») se citan pero no se han leído a fondo aquí (cubre ESPEC_04/07). El comentario del VM (`GenerarCPsViewModel.cs:145-146`) dice «no portada al dominio» mientras que el cuerpo la usa del dominio (`:165`); se asume la versión del cuerpo. |
| A-60-1 | 60 | Codificación de `equipos*.dat` | Decisión | Codificación de `equipos*.dat`: Latin-1 en el port (`GestorEquiposFrmViewModel.cs:235,343`) frente a `Encoding.Default` del legacy (ANSI del Windows de cada usuario). Con catálogos online (México) pueden aparecer caracteres no Latin-1. Decidir UTF-8 vs Latin-1 en Mac y cómo migrar ficheros de usuarios (M-R3/T8). |
| A-62-1 | 62 | Tras «Abrir» el filtro no llega al analizador | Divergencia | Tras «Abrir» un `.geq`, `_grupo` pasa a ser el Grupo leído del disco (`GruposEquiposFrmViewModel.cs:896-909`) y `Aceptar` hace `_grupo.ActivaFiltro(_filtroGE)` sobre ese grupo temporal (`:838`), no sobre `AppState.GrupoEnEdicion` (legacy: `FormPadre.analizador.GruposPartidos[...]`, `UI/Filtros/GruposEquiposFrm.cs:1507-1513`). El filtro abierto no llega al analizador. Decidir: corregir (aplicar siempre al grupo real) o replicar. Efecto no ejecutado, deducido del código. |
| A-62-2 | 62 | Confirmaciones del legacy omitidas | Divergencia | Confirmaciones del legacy (Abrir con datos, Borrar) omitidas en el port; la UI Mac debería decidir si las restablece (R2). |
| A-63-1 | 63 | Escritura de valoraciones duplicada y formato 1 inconsistente | Defecto aparente | El comentario del VM (`GuardarValoracionFrmViewModel.cs:26-27`) dice que `Porcentajes.Guardar` «no está portada al dominio», pero existe en `Free1X2.Domain/Utils/Porcentajes.cs:354-420`; el VM duplica la escritura. En Swift debe haber una sola implementación. El formato 1 no sustituye la coma decimal (inconsistente con 3/42); decidir si se corrige. |
| A-64-1 | 64 | «Nuevo…» crea un fichero vacío que luego se rechaza | No verificado | Con «Nuevo…» el `FileSavePicker` de WinUI crea un fichero de 0 bytes; `GuardarValoracionesHistorico` lo detecta como existente y, al estar vacío, lo rechaza (`HistoriaValoracionesFrmViewModel.cs:200-213,274-283,323-334`). No verificado en ejecución. En Mac, tratar «inexistente o vacío» como nuevo (decisión del dueño). |
| A-64-2 | 64 | `LeeColumnaSinComas` no quita comas y usa la cultura | Defecto aparente | `LeeColumnaSinComas()` es `ReadLine().Trim().ToUpper()` y **no** quita comas (`Domain/EntradaSalida/ArchivoColumnasTexto.cs:214-217`); `ToUpper()` usa la cultura actual (T7/T11). Afecta a Dependencia lineal, Escrutinio y al histórico. Cubre ESPEC_07. |
| A-65-1 | 65 | Contrato textual de las condiciones If-Then | No verificado | El contrato textual de las condiciones (`"Cantidad de Variantes: 5"`, `"(NO) …"`, `"Dibujo i+j"`) lo interpreta `ControladorIfThen` (no leído aquí; ESPEC_01). Pendiente verificar que `"<genérica>: n"` sin específica es aceptado por el motor. |
| A-66-1 | 66 | `ObtenNumCols` es una estimación | Decisión | `ObtenNumCols()` (`Domain/EntradaSalida/ArchivoColumnasTexto.cs:314-338`) estima el nº de columnas por `tamaño/(len+2)`; el texto «N columnas.» de salida puede diferir del real. Decidir si Swift replica la estimación (M-R1, solo cosmética) o cuenta exacto. |
| A-68-1 | 68 | Unidad de las coordenadas de impresión | No verificado | Unidad de las coordenadas (19,685 etc.): inferida como centésimas de pulgada (5 mm de separación entre partidos). Comprobar contra un boleto físico/PDF de referencia antes de fijar la conversión a puntos de `NSPrintOperation`. |
| A-68-2 | 68 | `GetConfig/SetConfig` y «Girar boleto» | Divergencia | `GetConfig()` del legacy termina con `return false` (`Free1X2/UI/imprimirBoleto.cs:98`) y el constructor hace `if (!GetConfig()) SetConfig();` (`:60`): siempre re-escribe el `.cfg` con lo que haya en pantalla. El port no hace `SetConfig()` en el constructor (`ImprimirBoletoFrmViewModel.cs:124-128`). Decidir. «Girar boleto» no se aplica en la salida en ninguna de las dos versiones; ¿debe girar el boleto en Mac? |
| A-71-1 | 71 | BOM en los listados exportados | Divergencia | El legacy escribe los listados con `new StreamWriter(ruta, false, Encoding.UTF8)`, es decir **con BOM** (`Free1X2/UI/ListadoCondicionesFrm.cs:869-873,889-893`), mientras que el port usa `FileIO.WriteTextAsync(file, texto, UnicodeEncoding.Utf8)` (`ListadoCondicionesFrmViewModel.cs:666`), cuyo BOM no se ha verificado (T8). Decidir si el listado Mac lleva BOM. Tampoco se ha cruzado el resultado textual con una ejecución real; el árbol está transcrito del VM. |
| A-72-1 | 72 | Sin cancelación ni límite de ganadoras | Defecto aparente | Sin cancelación ni límite del nº de ganadoras (`3^k`); con muchos partidos involucrados el cálculo puede ser enorme en tiempo y memoria (`MejoresOpcionesFrmViewModel.cs:182-193,210-244`). En Swift añadir cancelación y aviso (no cambia resultados). `List.Sort` inestable en empates (T5). |
| A-73-1 | 73 | Bucle infinito latente en `ProcesarGrupos` | Defecto aparente | Bucle infinito latente en `ProcesarGrupos` cuando «Proporcional» y «Ordenar signos» está desactivado y el signo `2` domina (`ModificadorFrmViewModel.cs:350-360,373-392`; verificable por inspección, no ejecutado). Decidir cómo manejarlo en Swift (error, o garantizar progreso sin cambiar el resultado en los casos que terminan). |
| A-73-2 | 73 | El modo Aleatorio no es reproducible | Decisión | El modo Aleatorio no es reproducible (reloj, `DateTime.Now.Millisecond`); en Swift se necesita un RNG inyectable y definir cómo se generan los golden (probablemente sin golden para ese modo). |
| G-1 | todas | Oráculo: port WinUI o WinForms legacy | Decisión | La lógica de las 33 pantallas vive en el ViewModel del port, no en `Free1X2.Domain`, y el proyecto de tests solo referencia el Domain (`Free1X2.Domain.Tests.csproj:18`). `CLAUDE.md` define el WinForms como referencia de comportamiento y el Domain como lo que debe ser idéntico (R1). Este documento transcribe el **port** y cita el legacy cuando difiere; las divergencias encontradas (A-41-1, A-41-2, A-57-1, A-62-1, A-62-2, A-68-2, A-71-1) necesitan una decisión por caso de uso antes de generar golden. |
| G-2 | todas | Carpeta inicial de diálogos y carpetas de datos en Mac | Decisión | El port abre los diálogos en Documentos (`PickerHelper.cs:32,45,58,76`; solo la exportación PNG de la pantalla 61 usa Imágenes, `GraficoColumnasFrmPage.xaml.cs:107`); el legacy fijaba carpetas relativas al ejecutable (`Free1X2/UI/Filtros/InterrupcionesFrm.cs:744`, `Condiciones\`; A-44-2). La política de carpeta inicial, `Temp/` para Copiar/Pegar (`FiltroArchivoViewModelBase.cs:44-49`) y `Ganadoras/` queda en ESPEC_08 A20. |
| G-3 | 46, 52, 63, 64, 70 | Cultura en parseo, formato y orden | Decisión | Estas pantallas parsean o formatean con la cultura actual (T7, T11): `ParseEsp` y `double.ToString()` en 46, `ToUpper()` en 52, formato 1 con coma decimal en 63, `Convert.ToDouble` en 64, orden alfabético en 70 (ver sus secciones). Fijar `Locale` explícito en Swift y decidir qué ficheros de usuario existentes deben seguir leyéndose con coma decimal. |
| G-4 | todas | Sin golden para la lógica de los ViewModel | Decisión | El plan genera los golden ejecutando «el motor actual» con `tools/GoldenGen` (`docs/PLAN_MIGRACION_MAC.md:78-85`), pero los 41 casos de uso CU-B01…CU-B41 se extraen de código que hoy no está en el Domain, por lo que no tienen ejecutor C# aislable ni tests existentes. Hay que decidir cómo se obtiene su oráculo (p. ej. extraer su lógica a una biblioteca C# sin UI, o fijar los resultados con ejecuciones del port). |
