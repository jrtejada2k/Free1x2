# ESPEC 08 — UI A: pantallas 1–40 (por orden alfabético de ViewModel)

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Convenciones en
> [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Describe **qué hace hoy** el código WinUI 3
> para reescribirlo en SwiftUI sin mirar el C#. Las asignaciones de «área Mac» y de tipo de
> presentación (vista principal / hoja modal / inspector) son **propuestas** (R2): las decide el dueño.

**Prefijos de ruta** (para abreviar `fichero:línea`; todas relativas a la raíz del repo):
`P:` = `Free1X2.WinUI/Views/Ported/` · `V:` = `Free1X2.WinUI/Views/` · `W:` = `Free1X2.WinUI/` ·
`D:` = `Free1X2.Domain/` · `L:` = `Free1X2/` (WinForms legacy, referencia de comportamiento).
Una referencia `:NNN` sin fichero es del último fichero citado en el mismo apartado.

**Alcance.** Las 40 primeras pantallas de `cd Free1X2.WinUI/Views/Ported && ls *ViewModel.cs | sed -n '1,40p'`, de `AgregaP15Frm` a
`DibujosFrm`. Cada una se documenta con: propósito y apertura · entradas (controles, rangos, defectos,
validaciones y mensajes literales) · acciones y llamadas al motor · **lógica de negocio que vive en el
ViewModel** (pseudocódigo → `Free1X2Core/CasosDeUso`) · ficheros y estado global · salidas · área Mac
propuesta · casos de uso `CU-Axx` (numeración única `CU-A01`…`CU-A90`, catálogo en §3.3). Al final: tabla resumen (§3) y «Abiertas» (§4). Las pantallas vecinas fuera de este alcance (41–108) se documentan en ESPEC_09 y ESPEC_10.

**Cómo leer el pseudocódigo.** Igual orden de iteración y de efectos que el C# (T5/T6); `/` entre enteros
es división entera (T9); `Round` = redondeo bancario salvo que se diga lo contrario (T4); donde el C#
concatena cadenas o usa `Convert.ToInt32` sobre `double` se indica. «Preservar» marca peculiaridades
que afectan a la salida (R1); las que parecen defectos se recogen en «Abiertas».

---

## 1. Contexto compartido por estas pantallas

### 1.1 Estado global que tocan (`AppState`, `VariablesGlobales`)

| Elemento | Qué es | Pantallas de este alcance que lo usan | Evidencia |
|---|---|---|---|
| `AppState.Instancia.Analizador` | El `Analizador` de la combinación en pantalla (boleto base, grupos, condiciones, If-Then, `CtrlGrupos`). Singleton de proceso. | 7, 8 (no: crea uno propio), 13, 18, 27, 28, 29, 31 | `W:Services/AppState.cs:94-102` |
| `AppState.GrupoPantalla` / `GrupoActual` | Índice y objeto del grupo visible en Inicio (0 = boleto base). `GrupoActual` reajusta a 0 si el índice sale de rango. | 28, 29 | `W:Services/AppState.cs:105-114,139-150` |
| `AppState.GrupoEnEdicion` (estático) | «Handoff» del grupo cuya condición se edita; lo fija Inicio antes de navegar. | 19, 26, 29, 40 | `W:Services/AppState.cs:73`, `V:MainPageViewModel.cs:288-296` |
| `AppState.UsarAnalizadorCompartido` (estático) | Bandera de un solo uso para que «Calcular» use el analizador compartido. | 13 | `W:Services/AppState.cs:84`, `V:MainPageViewModel.cs:544-557` |
| `AppState.JornadaActual` + eventos `JornadaCambiada`, `Cambiado` | Jornada online cargada; `Cambiado` refresca semáforos de Inicio. | 32 (escribe); 7, 19, 27, 28, 29, 31 (`NotificarCambio`) | `W:Services/AppState.cs:47-56,63,91,166` |
| `VariablesGlobales` | 44 miembros estáticos de configuración (T10). En estas pantallas: `NumeroPartidos`, `PrecioApuesta`, `Moneda`, `Desplazamiento`, `PuntosFijos/Dobles/Triples`; `ReinicializarVariables()` tras guardar configuración. | 5, 10, 12, 13, 14, 19, 24, 25, 40 | `D:VariablesGlobales.cs:177-223` |

Nota: «`Analizador`» en todo este documento es `Free1X2.MotorCalculo.Analizador` (`D:MotorCalculo/Analizador.cs:28`, el de `W:Services/AppState.cs:3,94`). Existe otra clase homónima `Free1X2.Analisis.Analizador` (`D:Analisis/Analizador.cs:30`, constructor con `minimoPremio`); de los ViewModels de este alcance solo `BancoPruebasFrmViewModel` importa `Free1X2.Analisis` y no nombra `Analizador`.

**Propuesta Swift.** Un `@Observable final class Sesion` (analizador, grupo en pantalla, nombre de `.comb`,
filtro de columnas, jornada) inyectado por `.environment`, y un `struct Configuracion: Sendable` leído/escrito
por un `RepositorioConfiguracion` (reemplaza `VariablesGlobales`). Los «handoffs estáticos» se sustituyen por
parámetros tipados de la navegación o por valores devueltos por la hoja (ver §1.3).

### 1.2 Servicios de UI que usan los ViewModels

| Servicio | Comportamiento | Equivalente Swift propuesto | Evidencia |
|---|---|---|---|
| `PickerHelper.AbrirAsync/AbrirVariosAsync(exts…)` | Selector de fichero; carpeta inicial = Documentos; filtros por extensión en el orden dado; devuelve `null` si se cancela. | `fileImporter(allowedContentTypes:)` / `NSOpenPanel` | `W:Services/PickerHelper.cs:30-49` |
| `PickerHelper.GuardarAsync(nombre, (etiqueta, ext)…)` | Guardar con nombre sugerido y tipos; carpeta inicial Documentos. | `fileExporter` / `NSSavePanel` | `W:Services/PickerHelper.cs:56-94` |
| `AppServices.MostrarError/MostrarInfo(msg)` | Diálogo modal con título «Error» / «Free1X2» y botón «Aceptar», **encolados** de uno en uno; sin ventana → solo log. | `alert` con cola serie | `W:Services/AppServices.cs:138-162` |
| `AppServices.ConfirmarAsync(msg, título, botónPorDefecto)` | Sí/No (principal «Sí», cierre «No»); sin ventana → `false`. | `confirmationDialog` | `W:Services/AppServices.cs:58-114` |
| `AppServices.EjecutarObservandoErrores` | Lanza una `Task` y, si falla, log + «No se pudo completar la acción:\n\n…». | `Task` con `do/catch` + alerta | `W:Services/AppServices.cs:123-135` |
| `UiHilo.Ejecutar` | Marshal al hilo de UI; sin UI ejecuta en el actual. | `@MainActor` / `AsyncStream` de progreso (T12) | `P:_Helpers/UiHilo.cs:32-48` |
| `ColeccionUi<T>.ReemplazarTodo` | Reemplazo en bloque con un solo `Reset`. | asignar un array a una propiedad `@Observable` | `P:_Helpers/ColeccionUi.cs:34-52` |

Patrón de tareas largas: `await Task.Run(...)` + `UiHilo` para progreso + bandera volátil `_salida`/`Cancelar`
(Anastatics, AnaCombi, Coincidencias, DiFiltros, CombinarFiltros, AnalizadorJPM, CalculaColumnas*). En Swift:
funciones `async throws` con `Task.checkCancellation()` y progreso por `AsyncStream<Progreso>`.

### 1.3 Handoffs estáticos entre pantallas (productor → consumidor)

| Dato | Productor → consumidor | Mecanismo actual | Evidencia |
|---|---|---|---|
| Formatos predefinidos | `Formatos123Frm` → pantalla 5 | `AnalisisFormatos123FrmPage.FormatosEntrada` | `P:AnalisisFormatos123FrmPage.xaml.cs:33` |
| Columna, analizador, pronósticos | pantalla 18 → pantalla 7 | `AnalizarCombinacionFrmViewModel.UltimoAnalisis` | `P:AnalizarCombinacionFrmViewModel.cs:86` |
| Matriz y nº de columnas | pantalla 9 → 38, 39 (y `StaInter`, `StaSS`) | `MatrizEntrada` / `NumColEntrada` | `P:AnastaticsViewModel.cs:172-199` |
| Fichero, orden, tipo | `VerBoletos` → pantalla 11 | `BoletoFrmPage.ParametrosBoleto` | `P:BoletoFrmPage.xaml.cs:27` |
| Filas premiadas | `Escrutinios`/`EscrutarCombinaciones` → pantalla 20 | `ColumnasPremiadasFrmPage.Entrada` | `P:ColumnasPremiadasFrmPage.xaml.cs:23` |
| Rango (Desde, Hasta) | pantalla 19 ↔ 30 | parámetro de navegación + `CopiarDatosCPFrmViewModel.Resultado` | `P:CopiarDatosCPFrmViewModel.cs:24` |
| CP importadas / a exportar | pantalla 19 ↔ `ImportadorCPsFrm` / `ExportadorCPsFrm` | `ImportadorCPsFrmViewModel.Resultado`, `ExportadorCPsFrmViewModel.ListaParaExportar` | `P:ColProbablesFrmViewModel.cs:746,875` |
| Extremos 10×4 | `TramificarForm` ↔ pantalla 35 | `ExtremosEntrada` / `Resultado` | `P:DialogoFiltrarPorLimitesFrmPage.xaml.cs:19,26` |
| Figuras / estadísticas | pantallas 26, 40 → `FigurasFiltrosFrm`, `VisorEstadisticas` | `FigurasEnEdicion`+`NombreCondicion`, `UltimasEstadisticas` | `P:ContactosFrmViewModel.cs:199-201,275` |
| Contenedor de análisis | pantalla 8 (vía hook de dominio) → `VisorAnalisisColumnasFrm` | `AnalisisUi.MostrarVisor` → `UltimoContenedor`/`UltimoGrupo` | `W:App.xaml.cs:170-206` |

**Propuesta Swift.** Modelar la navegación con un `enum Destino: Hashable` con valores asociados (p. ej.
`.visorAnalisis(ContenedorAnalisis)`, `.boleto(fichero:orden:tipo:)`) y las hojas con resultado como
`@State var resultado: ResultadoHoja?` + cierre. Quita el riesgo de «consume y limpia» y de datos obsoletos.

### 1.4 Cuatro codificaciones de «columna → entero» distintas (¡no mezclar!)

| Código en | Dígito de un partido | Orden de partidos | Se usa en |
|---|---|---|---|
| `S14N/N14S`, `S2n/N2s`, `S2N/N2S`, `S1n/N1s` | `'1'→1`, `'2'→2`, **`'X'` y cualquier otro → 0** | **primer partido = dígito más significativo** (`nx = nx*3 + d`); `N14S` genera siempre 14 dígitos, `0→'X'` | pantallas 4, 6, 17, 21 (`P:AnaCombiViewModel.cs:409-440`, `P:CombinarFiltrosViewModel.cs:110-133`) |
| `ConvertidorDeBases` | **`'1'→0`, `'2'→2`, cualquier otro (X) → 1** | **primer partido = dígito menos significativo** (`PotDe3[i]`); `ConvNumAColumna` añade `'1'` hasta `NumeroPartidos` | pantallas 10, 22, 33 (`D:Utils/ConvertidorDeBases.cs:43-52,96-106`) |
| `UtilColumnas.ConvStrToLong` | 3 bits por partido: `'2'→0b001`, `'X'→0b010`, `'1'→0b100`; otro → `1<<-1` (basura, T1) | primer carácter = bits bajos | pantallas 7, 12, 18 (`D:Utils/UtilColumnas.cs:196-204`) |
| `ConvStrToLong` local de Formatos 123 | rangos: `'3'→0b001`, `'2'→0b010`, `'1'→0b100` | primer carácter = bits bajos | pantalla 5 (`P:AnalisisFormatos123FrmViewModel.cs:212-221`) |

El orden de las filas generadas desde un índice depende de la codificación (p. ej. `Grabar` de 6 y 33 recorre
por índice ascendente de su propia codificación). Cualquier golden debe fijar la función usada.

### 1.5 Componentes de presentación reutilizados

- **`PorcentajesControl`** (pantallas 5, 10, 12): rejilla `N × 3` (`N = NumeroPartidos`) con columnas «Part.», «1», «X», «2»; una
  `NumberBox` por celda, `Mínimo=0`, sin máximo ni botones de paso; modelo `FilaPorcentaje{partido:Int, uno,equis,dos: Double}`;
  `PorcentajesHelper.AMatriz` ↔ `Double[N][3]` (índices 0=«1», 1=«X», 2=«2») y `CargarMatriz`. Altura máx. 260 px con scroll.
  Evidencia: `W:Controls/FilaPorcentaje.cs:11-26`, `W:Controls/PorcentajesControl.xaml:19-57`, `W:Controls/PorcentajesHelper.cs:23-41`.
  En SwiftUI: `Grid`/`Table` de `TextField(value:format:)` con `Double` y `Locale` fijo (T7).
- **`NumberBox`**: entrega `double`; las VMs hacen `(int)` (**trunca**) o `Convert.ToInt32` (**redondeo a par**, p. ej. 2.5→2; solo en pantalla 6).
  Un `NumberBox` vacío entrega `NaN`; `(int)NaN` es indefinido en C#. En Swift validar y rechazar.
- **`SemaforoControl`** (`EstadoSemaforo{Neutro, Verde, Rojo}`): indicador de 3 estados; en el árbol de fallos «Neutro» = ámbar (aceptada por tolerancias).
  Evidencia: `W:Controls/SemaforoControl.xaml.cs:12-20`.
- **Tiempos**: varias pantallas muestran `TimeSpan.ToString()` (+ `"0000000000"` recortado a 10/11 caracteres) o `DateTime.ToLongTimeString()`;
  son formatos dependientes de la cultura (T7). Propuesta: `Duration.formatted` con formato fijo documentado y mismo recorte.

### 1.6 Patrón de pantalla de condición (**PC**) — pantallas 26 y 40 (y, por analogía, las demás condiciones en ESPEC_09/10)

Todas las condiciones se abren desde la rejilla de Inicio con `AppState.GrupoEnEdicion` fijado, trabajan sobre un
**buffer** y aplican al grupo solo en «Aceptar» (salvo las excepciones señaladas), y comparten estos botones:

| Botón | Comportamiento común | Evidencia |
|---|---|---|
| **Aceptar** | Vuelca el buffer al filtro del grupo (`ContieneDatos`/`IsActive` según haya datos), `UsaFiguras()` si procede, `grupo.ActivaFiltro(filtro)`, `AppState.NotificarCambio()`, `Volver`. | `P:ContactosFrmViewModel.cs:214-227`, `P:DibujosFrmViewModel.cs:178-209` |
| **Cancelar** | `Volver` sin aplicar. | `P:ContactosFrmViewModel.cs:233-237` |
| **Borrar** | Vacía el buffer (no toca el grupo hasta Aceptar). | `P:ContactosFrmViewModel.cs:282-291` |
| **Estadísticas** | Construye un filtro temporal con el buffer y llama a `CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, <base>/Ganadoras/)`; navega a `VisorEstadisticas`. | `P:ContactosFrmViewModel.cs:264-277` |
| **Guardar / Abrir** | Base `FiltroArchivoViewModelBase`: guardar con `PickerHelper.GuardarAsync(NombreSugerido, TiposGuardar)` → `GuardarEn(ruta)`; error → `MostrarError("No se pudo guardar: " + msg)`. Abrir con `AbrirAsync(ExtensionesAbrir)` → `AbrirDesde(ruta)`; error → `"No se pudo abrir: " + msg`. | `P:_Helpers/FiltroArchivoViewModelBase.cs:61-90` |
| **Copiar / Pegar** | «Portapapeles» = **fichero temporal** `<base>/Temp/tmp.<ext>` (crea la carpeta): Copiar = `GuardarEn(tmp)`; Pegar = `AbrirDesde(tmp)` solo si existe (si no, no hace nada). Errores: `"No se pudo copiar: …"`. | `P:_Helpers/FiltroArchivoViewModelBase.cs:93-114` |

Formato de los ficheros de condición (`ArchivoCondiciones`, `.cont`, `.dbj`, `.xml`…) en ESPEC_07. Extensiones y nombres sugeridos por condición:
`Contactos` → `.cont`/`.xml`, `Contactos`, tmp `.cont`; `Dibujos` → `.dbj`/`.xml`, `Dibujos`, tmp `.dbj`.
**Propuesta Swift.** Un protocolo `EditorCondicion` (`cargar(desde: Grupo)`, `aplicar(a: inout Grupo)`, `exportar(a: URL)`, `importar(de: URL)`)
y el «portapapeles» como `NSPasteboard` con tipo propio **o** conservar el fichero en `Application Support/.../Temp` (decisión del dueño → A20).

### 1.7 Carpetas de datos que nombran estas pantallas

Hoy todas son `AppContext.BaseDirectory + "/…"` (junto al `.exe`): `Equipos/` (pantalla 2), `Columnas/` (pantalla 14),
`Temp/` (26, 40), `Ganadoras/` (26, 40), `W:Datos/ColumnasProbables.xml` (23), `W:Datos/parametros.free1x2` (16, 24, 25) y
`Comunicaciones/` (no usada aquí). En Mac el bundle no es escribible (PLAN §2): propuesta
`~/Library/Application Support/Free1X2/<carpeta>` con copia inicial desde el bundle (decisión del dueño → A20).

---

## 2. Pantallas (orden alfabético de ViewModel)

### Pantalla 1 · AgregaP15Frm — «Añade P15 (Indeciso)»

**Propósito.** Convierte un fichero de columnas de 14 signos en otro de 15 signos (o al revés) decidiendo el Pleno al 15 con uno de cuatro modos. **Apertura.** Menú *Combinación → Añadir Pleno al 15…* y botón `bP15` de la barra Combinación (`W:MainWindow.xaml.cs:233,403`). No se abre desde otra pantalla.

**Entradas** (`P:AgregaP15FrmViewModel.cs:33-91`, `P:AgregaP15FrmPage.xaml:38-147`)
| Control | Tipo / rango | Defecto | Notas |
|---|---|---|---|
| Fichero entrada | ruta `.txt` | `""` | abrir, carpeta inicial Documentos (`:97`, `W:Services/PickerHelper.cs:32`) |
| Fichero salida | ruta `.txt` | `""` | guardar, nombre sugerido `ColumnasP15` (`:105`) |
| Modo 1 «Partido 15 fijo a» + signo | bool + {1,X,2} | off / `""` | |
| Modo 2 «Si partido N = s entonces P15 = a; si no b» | N: NumberBox 1..14 (`xaml:106`), s/a/b ∈ {1,X,2} | N=1, signos `""` | |
| Modo 3 «Elimina partido 15» | bool | off | muestra «Nº Columnas Repetidas» |
| Modo 4 «Resultado P15 igual que partido N» | N: 1..14 | 1 | |
| Estado | texto | «Preparado» → «Calculando» → «Terminado»/«Error» | |

**Validaciones** (`ComprobarEntradas`, `:113-132`), mensajes literales: «Uno de los archivos necesarios no ha sido especificado\n» (alguno vacío) / «El archivo de salida debe ser distinto al de entrada\n» (misma ruta, comparación ordinal exacta) / «No se ha especificado qué hacer con el Pleno al 15» (ningún modo). Se concatenan; el resultado (`Trim()`) va a `Estado`, sin diálogo. **No** se valida que los signos de los combos estén elegidos ni que las columnas tengan 14 signos.

**Acción `Calcular`** (`:145-188`, `EjecutarAgregaP15` `:194-353`), en hilo de fondo (`Task.Run`). Excepción → `MostrarError("Error al añadir el P15: " + msg)` y `Estado="Error"`. Si termina: `ColumnasRepetidas = noColsRepetidas`, `Estado="Terminado"`.

**Lógica de negocio en el VM (pseudocódigo)**
```
buf1[8], bufX[8], buf2[8] = ""; c1=cX=c2=0; repetidas=0; vistas = BitArray(3^14 = 4 782 969)
ent = ArchivoColumnasTexto(entrada); sal = ArchivoColumnasTexto(salida)   // sal crea/trunca al primer GuardarCols
mientras ent.SiguienteColumna():
  col = ent.LeeColumnaSinComas()            // Trim + ToUpper (D:EntradaSalida/ArchivoColumnasTexto.cs:214-217)
  si modoFijo:   sal.Guardar(col + signoFijo)     // los demás modos se IGNORAN
  si no:
    si modoCondicional:
        col = col + (col[N-1] == char.Parse(signoCond) ? signoSi : signoNo)   // char.Parse("") lanza
        encolar(col)                       // ver «encolar»
    si modoEliminar:
        col = col[0..14)                   // opera sobre el valor de col ya modificado por el modo 2 si coincidían
        n = ConvColumnaANumero(col)        // base 3
        si !vistas[n]: vistas[n]=true; sal.Guardar(col) sino repetidas++
    si modoCopiar:
        col = col + col[M-1]; encolar(col)
encolar(col): según col[14] ∈ {"1","x"/"X","2"} → bufS[cS++]=col ; (otro carácter: la columna se PIERDE ; si col no tiene 15.º carácter, p. ej. signoSi vacío → ArgumentOutOfRange → «Error al añadir el P15: …»)
              si c1==8: volcar buf1 (8 líneas), c1=0 | else si cX==8: ... | else si c2==8: ...
al terminar: volcar restos buf1[0..c1), bufX[0..cX), buf2[0..c2) en ese orden (1, X, 2); cerrar.
```
- **Orden de salida** en modos 2 y 4: NO es el de entrada; las columnas salen **agrupadas en bloques de 8 por signo de P15** y los restos al final en orden 1, X, 2 (`:263-277,337-348`). Preservar (golden obligatorio).
- Combinaciones de modos (2+3, 2+4, …) se ejecutan **en secuencia sobre la misma `col`** y pueden duplicar o corromper líneas (p. ej. 2+4 añade un 16.º carácter y `Substring(14,1)` sigue leyendo el P15 del modo 2). Preservar o restringir a un solo modo (decisión del dueño → Abiertas A1).
- Modo 3 detecta repetidas con 14 primeros signos; las repetidas **no** se escriben; `noColumnasFinal` se calcula pero no se muestra (`:213,288`).
- `Signos` de los combos son `1`,`X`,`2`; el modo fijo con signo vacío añade cadena vacía.

**Ficheros.** Lee/escribe `.txt` de columnas (una por línea; escritura UTF-8 sin BOM, `WriteLine`, comas eliminadas: `D:EntradaSalida/ArchivoColumnasTexto.cs:70-80`). Estado global: ninguno.

**Salidas.** Texto de estado y contador entero «Nº Columnas Repetidas» (`ToString()` entero).

**Área Mac propuesta.** Herramientas → *Operaciones sobre columnas*; **hoja modal** (formulario corto con selector de modo exclusivo). Casos de uso: CU-A01 `AgregarPleno15`. Evidencia adicional: `W:MainWindow.xaml.cs:233,403`; `P:AgregaP15FrmPage.xaml.cs:43-73` (pickers duplicados, equivalentes a `PickerHelper`).

---

### Pantalla 2 · AgregarEquipoFrm — «Añadir Equipo»

**Propósito.** Da de alta un equipo (nombre + categoría 1ª/2ª/2ªB/Int) directamente en el `.dat` de la categoría. **Apertura.** Desde `GestorEquiposFrm` (botón «nuevo equipo», `P:GestorEquiposFrmViewModel.cs:113`); no está en menús.

**Entradas** (`P:AgregarEquipoFrmViewModel.cs:37-50`): `NombreEquipo` (texto libre, `""`), `Categoria` ∈ {«1ª»,«2ª»,«2ªB»,«Int»} (defecto «1ª»), `Estado` («Preparado»).

**Acción `Anadir`** (`:57-99`):
```
si NombreEquipo vacío o solo espacios → Estado="Introduce un nombre de equipo"; fin
archivo = {1ª: W:Datos/Equipos/equipos1.dat, 2ª: W:Datos/Equipos/equipos2.dat, 2ªB: W:Datos/Equipos/equipos2b.dat, Int: W:Datos/Equipos/equiposInt.dat}  en <base>/Equipos/
equipos = leerLineas(archivo, Latin1) (si no existe → [])
si equipos.Contains(NombreEquipo)  // List<string>.Contains = comparación ORDINAL, sensible a mayúsculas
    → Estado = «El equipo "X" ya existe en <cat>.» ; fin
equipos.Add(NombreEquipo)  // SIN Trim, al final de la lista
escribir todas las líneas (Latin1, WriteLine, crea el directorio si falta)
Estado = «Equipo "X" añadido a <cat>.» ; NombreEquipo = ""
excepción → Estado = «Error al añadir el equipo: …» + MostrarError(«No se pudo añadir el equipo: …»)
```
- Codificación **Latin1** (ISO-8859-1) en lectura y escritura (`:112,132`): los acentos del nombre fuera de Latin1 se pierden; en Swift usar `String.Encoding.isoLatin1` (T8).
- La ruta es `AppContext.BaseDirectory/Equipos/…` (`:27-28`): en Mac → `~/Library/Application Support/Free1X2/Equipos/`.
- **Peculiaridad**: no refresca el gestor abierto; el gestor tiene sus propias listas en memoria y sus propios botones de guardado (ver pantalla de Gestor Equipos en ESPEC_09). Dos fuentes de verdad sobre el mismo `.dat` → en Swift, un único `RepositorioEquipos`.

**Salidas.** Solo mensaje de estado. **Área Mac propuesta.** Boleto → *Equipos* (gestión de equipos); **hoja modal** sobre el gestor. Casos de uso: CU-A02 `AnadirEquipo` (reutilizable por el gestor). Evidencia: `P:AgregarEquipoFrmViewModel.cs:57-137`, `P:AgregarEquipoFrmPage.xaml:25-45`.

---

### Pantalla 3 · AlgebraColumnasFrm — «Álgebra»

**Propósito.** Operaciones de conjuntos sobre ficheros de columnas: eliminar repetidas, unir, intersectar, restar. **Apertura.** Menú *Operaciones → Álgebra de columnas…* y botón `bAlgebra` (`W:MainWindow.xaml.cs:244,380`).

**Entradas** (`P:AlgebraColumnasFrmViewModel.cs:35-73`, `P:AlgebraColumnasFrmPage.xaml:37-148`)
| Control | Detalle |
|---|---|
| Combinación 1 / 2 | selector de abrir `.txt` (carpeta inicial Documentos: `PickerHelper.AbrirAsync(".txt")`, `:214`); muestra nombre sin extensión; defecto «(selecciona)» |
| Resultado | selector de guardar `.txt`, nombre sugerido `ColumnasResultado` (`:119`) |
| Tipo de operación | desplegable índice 0..3 = enum `AlgebraCombTipo` (`D:Enums.cs:45`): 0 «Elimina columnas repetidas de Combinación 1», 1 «Suma Combinaciones: elimina columnas repetidas», 2 «Suma Combinaciones: selecciona columnas comunes», 3 «Resta combinaciones: (1)-(2)» (`:35-41`); defecto 0 |
| Detalle | «Combinación 1: N columnas.» / «Combinación 2: N columnas.» tras elegir fichero (`:91,109`) |
| Botón Calcular | deshabilitado mientras corre (`PuedeCalcular`) |

**Al elegir un fichero** (`:80-110`): `noSignosK = ArchivoColumnasTexto.ObtenNumSignos()` = longitud (tras `Trim`) de la **primera línea** del fichero (`D:EntradaSalida/ArchivoColumnasTexto.cs:342-357`); `NumColumnas` = nº de líneas (`ObtenNumCols`).

**Acción `Calcular`** (`:131-199`) — lógica de validación en el VM:
```
tipo = Operaciones[idx]
rutasOk = (tipo == EliminaRepetidas) ? (r1 != "" && rFinal != "") : (r1 != "" && r2 != "" && rFinal != "")
si !rutasOk → Resultado = "No ha seleccionado los archivos"; fin
partidosOk = (tipo == EliminaRepetidas) ? noSignos1 != 0 : (noSignos1 == noSignos2 && noSignos1 != 0)
si !partidosOk → limpia los tres nombres a "(selecciona)" y las tres rutas; MostrarError("Los archivos tienen distinto número de partidos"); fin
noPartidos = (EliminaRepetidas) ? noSignos1 : noSignos2
Task.Run: SumadorCombinaciones(noPartidos){ArchivoCols1, ArchivoCols2, ArchivoColsFinal}.Calcula(tipo); Resultado = MensajeFinCalculo
excepción → Resultado = "Error: " + msg ; finally PuedeCalcular = true
```
Nota: `noSignos` se calcula al **elegir** el fichero; si el fichero se edita después no se recalcula. Un fichero vacío da `noSignos = 0` y dispara el mismo mensaje «distinto número de partidos».

**Motor.** `SumadorCombinaciones` (`D:Utils/SumadorCombinaciones.cs:50` `Calcula`, `:283` `CrearMensajeFinCalculo`, `:327` `MensajeFinCalculo`); formato de fichero y detalle en ESPEC_06.

**Salidas.** Texto verde de resultado (`MensajeFinCalculo`), nº de columnas de cada entrada. **Botón Cancelar:** vacío en el VM (`:205-209`), no hace nada → en Mac basta cerrar la hoja.

**Área Mac propuesta.** Herramientas → *Operaciones sobre columnas*; **hoja modal**. Casos de uso: CU-A03 `AlgebraDeColumnas` (validación + invocación). Evidencia: `P:AlgebraColumnasFrmViewModel.cs:131-199`.

---

### Pantalla 4 · AnaCombi — «Análisis de grupos en combinación»

**Propósito.** Dado un fichero de columnas, un conjunto de partidos «grupo» (G) y un patrón de signos fijos con fallos admitidos, cuenta cuántas columnas caen en cada combinación posible de signos del grupo y permite grabar las de las combinaciones marcadas. **Apertura.** Botón `bAnalisisGrupos` de la barra Utilidades (`W:MainWindow.xaml.cs:431`); no figura en los menús.

**Entradas** (`P:AnaCombiViewModel.cs:20-37,75-97`, `P:AnaCombiPage.xaml:32-160`)
| Control | Detalle |
|---|---|
| Fichero de entrada | abrir con filtros `.txt` y `*` (`:135`); muestra solo el nombre; defecto «Fichero a procesar» |
| 14 combos (uno por partido) | opciones `-`, `1`, `X`, `2`, `G` (`:28`); defecto `-`. `1/X/2` fija el signo, `G` añade al grupo, `-` ignora |
| Fallos admitidos | NumberBox `Minimum=0`, paso 1 (`xaml:101-108`), defecto 0; se trunca `(int)` (`:392`) |
| Limpiar patrón | los 14 combos a `-` (`:120-126`); **no** reinicia fallos ni resultados |
| Calcular / Cancelar | `Cancelar` pone `_salida=true` (volátil); el bucle lo comprueba por línea (`:229`) |
| Contadores | «Procesadas» (cada 1024 líneas: `(ctproc & 0x3FF)==0`, `:244`) y «Tiempo» |
| Resultados | lista (casilla, Grupo en Consolas, Columnas) con casilla por fila |
| Grabar resultados | habilitado solo si hay ≥1 fila (`PuedeGrabar`, `:183`) |

**Validaciones/mensajes.** `MostrarError("Falta grupo")` si no hay ningún `G` (`:151-155`); `MostrarError("Falta fichero")` si no hay ruta o no existe (`:156-160`); línea con <14 caracteres válidos → excepción «Columna errónea = n» (n = nº de línea contada, `:232-235`) que sale como `MostrarError("Error al calcular: …")` (`:187`); Grabar sin ninguna casilla → `MostrarInfo("No hay ningún grupo marcado para grabar.")` (`:326`); éxito `MostrarInfo("Resultados grabados en <nombre>")`; fallo «No se pudieron grabar los resultados: …».

**Lógica de negocio en el VM (autocontenida, no usa el motor)**
```
RecuperaGrupo(): para i=0..13: s=Seleccion[i]
    patron[i] = s si s∈{1,X,2} sino '-' ; si s=="G": grup[ctgrup++] = i ; admfal = (int)FallosAdmitidos
S14N(c14): n=0; para k=0..13: n=n*3 + (c[k]=='1'?1 : c[k]=='2'?2 : 0)         // 'X' y cualquier otro = 0
N14S(n):  14 dígitos base 3, menos significativo al final; 0→'X', 1→'1', 2→'2'
Normaliza(l): ToUpper y conserva SOLO los caracteres {1,2,X}                     // descarta separadores/comas/espacios

Calcular:
  comptes[3^14] = -1
  // inicializa los 3^ctgrup grupos posibles (con relleno de '1')
  idx0 = S14N( N14S(0).suffix(ctgrup) + "1"*(14-ctgrup) )  ; comptes[idx0]=0
  para nr=1..3^14-1: t = N14S(nr).suffix(ctgrup)+"11111111111111" → primeros 14; idxn=S14N(t); si idxn==idx0 → break; comptes[idxn]=0
  para cada línea l del fichero (hasta _salida):
      c = Normaliza(l); ctproc++ ; si len(c)<14 → error "Columna errónea = ctproc"
      ctfal = nº de i∈0..13 con patron[i]!='-' y c[i]!=patron[i]
      si ctfal <= admfal:  g = c[grup[0]]…c[grup[ctgrup-1]] + '1'*(14-ctgrup) ; comptes[S14N(g)]++   // valides[S14N(c)] = idxg solo se escribe
  filas = para nr ascendente con comptes[nr]>=0: (N14S(nr).prefix(ctgrup), comptes[nr])
```
- **Orden de las filas**: ascendente por índice base-3 del grupo ⇒ orden lexicográfico por la cadena de grupo con `X < 1 < 2` (no alfabético). Incluye filas con 0 columnas (todas las 3^ctgrup combinaciones).
- Tamaño: dos `int[4 782 969]` (`:199-200`); `valides` se escribe pero **nunca se lee** en el VM (en Swift se puede omitir).
- El grupo compara solo los primeros 14 caracteres; columnas de 15+ (con P15) son válidas.
- **Grabar** (`:309-363`): `grupos = {Grupo de filas con casilla marcada}`; pide guardar (`.txt`, sugerido `Resultados`, `:330`); relee el fichero **en su orden**, `c=Normaliza(l)`, salta `len<14`, escribe `c` (Normalizada, **sin truncar a 14**) si `grupoDe(c)∈grupos`. **No reaplica el patrón ni los fallos admitidos** y conserva duplicados.
  - Divergencia con el original WinForms: `L:UI/AnaCombi.cs:93-123` recorría `idxc=0..3^14-1` (columnas únicas, ordenadas por índice base-3) usando `valides[idxc]` (solo las que pasaron el filtro de fallos) y escribía `n14s(idxc)` (14 signos). → «Abiertas» A2.
- `FormatearTiempo` (`:443-447`): `TimeSpan.ToString()` + `"0000000000"`, truncado a 10 caracteres (p. ej. `00:00:01.2`; con fracción 0 sale `00:00:0100`, peculiaridad). 

**Fichero.** Lee y escribe `.txt` (UTF-8, `StreamReader/Writer` por defecto, sin Trim previo). Estado global: ninguno.

**Salidas.** Tabla «Grupo / Columnas» (cadena de ctgrup signos / entero), contadores de progreso. 

**Área Mac propuesta.** Análisis → *Análisis de grupos* (vista principal con inspector lateral para el patrón de 14 partidos y tabla de resultados con casillas); cálculo `async` cancelable con progreso. Casos de uso: CU-A04 `AnalizarGruposEnFichero`, CU-A05 `GrabarGruposSeleccionados`. Evidencia: `P:AnaCombiViewModel.cs:146-293,309-363,377-447`.

---

### Pantalla 5 · AnalisisFormatos123Frm — «Analizador Formatos123»

**Propósito.** Carga un fichero de columnas, navega columna a columna y, con una valoración 1/X/2 por partido, traduce cada columna a notación de rango (1 = signo más valorado, 3 = menos) y cuenta apariciones de cada «formato» (subsecuencia). **Apertura.** Botón «Analizar» de `Formatos123Frm` (`P:Formatos123FrmViewModel.cs:290-302`, entrega los formatos por el estático `AnalisisFormatos123FrmPage.FormatosEntrada`, `P:AnalisisFormatos123FrmPage.xaml.cs:33,42-51`; si la lista está vacía allí se avisa «Guarde el Filtro Primero!!!»). Está también en el registro (`W:Navigation/PortedPages.cs:20`) pero **no** en menús ni barras.

El productor de la lista (`Formatos123Frm`) queda fuera de este alcance: ver «Abiertas» A21.

**Entradas** (`P:AnalisisFormatos123FrmViewModel.cs:65-103`, `P:AnalisisFormatos123FrmPage.xaml:30-96`)
| Control | Detalle |
|---|---|
| «Archivo de columnas» | abrir con filtros `.txt`, `.cols`, `*` (`:144`) |
| Navegación < > y contador «N/Total» (`0/0` si vacío) | `IndiceColumna` 0..Count-1, empieza en 0 tras cargar |
| Columna actual | `TextBox` solo lectura, Consolas |
| Rejilla de valoración | `PorcentajesControl` con `VariablesGlobales.NumeroPartidos` filas (1/X/2, `double`, `Minimum=0`, sin máximo; valores iniciales 0; ver §1.5) |
| Botones «Analizar» y «Mostrar todos» | |
| `ArrayFormatos` | lista de `Formato123` entregada por el productor (no editable aquí) |

**Carga** (`:142-177`): `Task.Run`: por cada columna `LeeColumnaSinComas()`; si `Length > 16` → **lista vacía y corta, sin mensaje** (el comentario habla de «Error leyendo columnas» pero no se muestra, `:157-162`); si no `Add(columna.ToUpper())`. Excepción → `MostrarError("Error leyendo columnas: …")`. Tras cargar: `IndiceColumna=0`.

**Lógica de negocio en el VM (pseudocódigo)**
```
Rango(valoracion[i][0..2]) → val[i][0..2] cadenas "1".."3"   // TransformarValoracion :310-333
   a=v0,b=v1,c=v2
   si a>=b y a>=c:   b>=c ? [1,2,3] : [1,3,2]
   sino si b>a y b>=c: a>=c ? [2,1,3] : [3,1,2]
   sino si c>a y c>b:  a>=b ? [2,3,1] : [3,2,1]
   // empates: el '1' gana a 'X' y '2'; la 'X' gana al '2'  (comparaciones >= / > exactas, orden preservar)
TraducirColumna(col, val): concatena por posición i: col[i]=='1' → val[i][0]; =='X' → val[i][1]; cualquier otro → val[i][2]
ConvStrToLong(s):  res=0; para i=len-1…0: res = (res<<3) ^ (1 << idx(s[i] en "321"))      // '3'→1, '2'→2, '1'→4 ; carácter 0 = bits bajos
Apariciones(colBin, fmtBin): n=0; mientras colBin!=0 { si (colBin & fmtBin)==fmtBin → n++ ; colBin >>= 3 }   // ocurrencias SOLAPADAS de la subcadena
Analizar:  si no hay columnas → nada ; si ArrayFormatos vacío → MostrarInfo("No hay formatos predefinidos para analizar. Abra el análisis desde Formatos 123 o use \"Mostrar todos\".")
           informe = [ (f.Formato, Apariciones(ConvStrToLong(columnaTraducida), ConvStrToLong(f.Formato))) para f en ArrayFormatos ]  // orden de ArrayFormatos
MostrarTodos: lista = para L=1..len(col): para pos=0..len-L: columnaTraducida[pos..pos+L)      // longitud-mayor-al-final, posición ascendente
              agrupa iguales conservando la PRIMERA aparición y su orden; cuenta = nº de repeticiones (O(n²) con RemoveAt)
```
- Los índices `val[i,…]` usan `i < len(columna)`; con columnas de 15–16 signos y rejilla de `NumeroPartidos` filas puede salir de rango (excepción no capturada) → «Abiertas» A3.
- `ColumnaActual` se muestra tal cual (mayúsculas), `IndiceColumna` fuera de rango → `""`.

**Salidas.** Informe de dos columnas «Formato» (Consolas) y «Apariciones» (entero). Sin formato numérico especial.

**Área Mac propuesta.** Análisis → *Formatos 123* (inspector con la rejilla de valoración + tabla). Casos de uso: CU-A06 `TraducirColumnaARangos`, CU-A07 `ContarAparicionesFormato123`, CU-A08 `ListarTodosFormatos123`. Evidencia: `P:AnalisisFormatos123FrmViewModel.cs:142-346`.

---

### Pantalla 6 · AnalizadorJPM — «Sumas Pares Naturales (JPM)»

**Propósito.** Puntúa cada columna sumando un valor (0–5) por cada uno de los 7 pares consecutivos de signos, clasifica las columnas únicas de un fichero en 36 casillas de suma (0..35), permite sumar/grabar las casillas marcadas y, para una columna ganadora, cuenta cuántas columnas a distancia ≤4 de ella caen en casillas marcadas (premios 14..10). **Apertura.** Desde `Formatos` (`P:FormatosFrmPage.xaml.cs:65`, botón de esa pantalla); en el registro (`W:Navigation/PortedPages.cs:65`, categoría Análisis); no está en menús ni barras.

**Entradas** (`P:AnalizadorJPMViewModel.cs:77-150,152-161`, `P:AnalizadorJPMPage.xaml`)
| Control | Detalle |
|---|---|
| 9 valores (11,1X,12,X1,XX,X2,21,2X,22) | `NumberBox` Mín 0, Máx 5, paso 1 (`xaml:61-107`); defecto `11=0, 1X=1, 12=2, X1=1, XX=3, X2=4, 21=2, 2X=4, 22=5` (`:155-157`). Se convierte con `Convert.ToInt32(double)` (**redondeo a par**, T4) y se recorta a [0,5] (`:495-507`) |
| Iniciar | abre `.txt`/`*` (`:170`) y procesa |
| Grabar | guarda `.txt`, nombre sugerido `ColumnasSalida` (`:245`) |
| Cancelar | `_salida = true` |
| 36 casillas | casilla marcable + contador (`Casillas[0..35]`, ListView de 260 px) |
| Sumar | suma los contadores de las casillas marcadas → «Suma:» |
| Cargar ganadoras, `-`, `+` | abre `.txt`/`*`; navega por la lista (índice 1-based) |
| «Columna ganadora» | TextBox editable (defecto «COL.GANADORA») |
| Analizar | calcula premios |
Etiquetas: «Fichero entrada/salida» (`-`), «Total columnas» (`0`), «Tiempo» (`-`), «Fichero ganadoras» (`-`), «Índice» (`0`), «Puntuación» (`-`), «14…10 aciertos» (`0`).

**Lógica de negocio en el VM (pseudocódigo)**
```
Puntuar(col): s=0; para nr=0,2,4,…,12: par=col[nr..nr+2) ; s += vals[tabla{11:0,1X:1,12:2,X1:3,XX:4,X2:5,21:6,2X:7,22:8}[par]]  // sensible a mayúsculas: 'x' minúscula suma 0
                                                                    // rango real 0..35 (7 pares × máx 5)
S1n(c) = Σ dígitos base 3 (posición 0 = más significativo; '1'→1,'2'→2, resto→0) ; N1s(n) inversa con 'X' para 0   // = S14N/N14S de AnaCombi
Iniciar:
  validas[3^14] = -1 ; lbtab[36]=0 ; repes = bitset(3^14) ; conta = 0 ; RecuperaValores()
  para cada línea (mientras haya, hasta _salida): col = línea.Trim()           // NO se pasa a mayúsculas
      si len(col) < 14: error "error de longitud=" + col ; parar bucle (conserva lo contado)
      nx = S1n(col)
      si !repes[nx]: p = Puntuar(col) ; validas[nx]=p ; lbtab[p]++ ; conta++ ; repes[nx]=true     // deduplica columnas
  // se actualizan SIEMPRE las 36 casillas, el total y «Tiempo» (TimeSpan.ToString()+"00000000000" recortado a 11)
Sumar: Σ lbtab[i] de casillas marcadas (int)
Grabar: si validas==null → MostrarError("Inicia primero el proceso de columnas.")
        para nr=0..3^14-1 ascendente: si 0<=validas[nr]<36 y casilla[validas[nr]] marcada → escribe N1s(nr)   // ORDEN base-3 (X<1<2 lexicográfico), no el del fichero
        FicheroSalida = nombre
CargarGanadoras: lista = VerColumna(línea) por línea (Trim; si len<14 → ""; 14 primeros; todos ∈ {1,2,x,X} o ""); primera "" → MostrarError("col.G. errónea") y no se carga nada
        nrfCGR = Count ; muestra el ÚLTIMO elemento (índice nrfCGR) ; «Índice» = nrfCGR
Siguiente/Anterior: nrfCGR ∈ [1, Count] (Anterior no baja de 1; Siguiente no pasa de Count) → ColumnaGanadora = lista[nrfCGR-1]
Analizar:
  col = ColumnaGanadora.Trim().ToUpper() ; si len<14 → MostrarError("error de longitud=" + col)
  colprs[3^14]=0 ; nprs[0..4]=0 ; ncol=S1n(col) ; nx2=Puntuar(col)
  si marcas[nx2]: colprs[ncol]=14 ; nprs[0]++
  BuscaPremios(ncol, 13)
  BuscaPremios(c0, nprof):                       // DFS, ORDEN IMPORTANTE (el conteo depende de él)
     para nr0=0..13 (Pot=[1,3,9,…,3^13], nr0=0 es el ÚLTIMO partido): x1=Pot[nr0]; s=(c0/x1)%3
        para z1=0..2: c1 = c0 + x1*(z1 - s)      // cambia ese partido al signo z1 (0='X',1='1',2='2'); z1==s deja c1==c0
           si colprs[c1] < nprof: colprs[c1]=nprof ; si marcas[Puntuar(N1s(c1))] → nprs[14-nprof]++
           si nprof > 10: BuscaPremios(c1, nprof-1)       // recursión incondicional (incluso con c1==c0)
  muestra: Puntuación = nx2 ; 14 = nprs[0] ; 13 = nprs[1] ; 12 = nprs[2]-nprs[1] ; 11 = nprs[3]-nprs[2] ; 10 = nprs[4]-nprs[3]
```
- Una columna puede contarse en varios niveles si se la alcanza primero por un camino más profundo y luego por uno más corto; la resta de acumulados de la visualización **no** garantiza conteos exactos por categoría. Preservar el orden de iteración (T5/T6) y la resta tal cual; la rejilla de 36 casillas y el cálculo usan solo `Int32`.
- `_salida` se comprueba en la entrada de `BuscaPremios` y en el bucle de lectura; `Cancelar` durante `Analizar` deja contadores parciales visibles.
- **Memoria**: `validas` (19 MB) se conserva entre Iniciar y Grabar; `colprs` se crea en cada Analizar.

**Ficheros.** Lee `.txt` (UTF-8, `StreamReader`, `Peek() > 0`); escribe `.txt` con `StreamWriter` por defecto, 14 signos en mayúsculas por línea. Estado global: ninguno.

**Salidas.** Contadores enteros (`ToString()`), tiempo `hh:mm:ss.f…` truncado.

**Área Mac propuesta.** Análisis → *Sumas de pares (JPM)*; vista principal con inspector. Casos de uso: CU-A09 `ClasificarColumnasPorSumaPares`, CU-A10 `GrabarCasillasSuma`, CU-A11 `ContarPremiosPorCasillas`. Evidencia: `P:AnalizadorJPMViewModel.cs:168-415,420-525`.

---

### Pantalla 7 · AnalizarCombinacionFrm — «Análisis de fallos» (árbol de condiciones evaluadas)

**Propósito.** Visor de solo lectura: evalúa una columna (la «ganadora») contra todas las condiciones/filtros/tolerancias/controles de grupos/If-Then del `Analizador` y muestra un árbol con tres estados (acertada / aceptada por tolerancias / fallada). **Apertura.** (a) desde `ColGanadoraFrm` → `Analizar` (`P:ColGanadoraFrmViewModel.cs:165`), que deja el estático `AnalizarCombinacionFrmViewModel.UltimoAnalisis` (`:86`); (b) menú *Combinación → Analizar combinación…* (`W:MainWindow.xaml.cs:228`), en cuyo caso se analiza el boleto en pantalla (`:117-136`). Nota: el botón `bAnalisisFallos` de la barra abre `ColGanadoraFrm`, no esta página (`:398`).

**Entradas.** Ninguna directa. Datos: `(nombre, columna:long, analizador, pronosticos[])`. Modo independiente (`AnalizarCombinacionActual`, `:146-181`): `analizador = AppState.Instancia.Analizador`; `pronosticos = analizador.Pronosticos` o, si vacío, `NumeroPartidos × "1,X,2"` (`:154-163`); `columna = UtilColumnas.ConvStrToLong(pronosticos)` (la columna «todo lo pronosticado», `:166`); `pronosticosBase[i] = pronosticos[i].Replace(",", "")`; nombre = `NombreArchivoComb` o «Combinación en pantalla».

**Lógica de negocio en el VM (pseudocódigo del árbol)** — `AnalizarCombinacion` `:188-467`. Estados: `A`=Acertada(verde), `T`=AceptadaPorTolerancias(neutro/ámbar), `F`=Fallada(rojo).
```
título ventana = "Análisis de fallos: " + nombreArchivo(sin ruta)
raíz = Nodo("Combinación", A)
para i en 0..grupos-1:
   texto = i==0 ? "Boleto base" : "Grupo i" + (NombreGrupo no vacío ? " (nombre)" : "")
   P = Nodo(texto, A) hijo de raíz
   si i==0:
      S = Nodo("Pronóstico de partidos")
      fallos = EvaluarPronosticos(columna, pronosticosBase)          // nil o lista de nº de partido
      si fallos != nil: S=F ; raíz=F ; P=F ; P += S ; para cada n: S += Nodo("Partido n  (" + pronosticosBase[n-1] + ")", F)   // 2 espacios
      sino: S=A ; P += S
   colG = grupo.ColumnaGrupo(columna)
   para cada filtro f del grupo con f.IsActive:
      N = Nodo(NombreVisible(f.NombreFiltro), A) ; ev = f.AnalizarFallos(colG) ; P += N
      si ev != nil:
         N=A ; P=A        // ← el port pone P=A aquí: pisa un F anterior de P (ver «Abiertas» A4)
         para cada línea m en ev:
            "*texto" → N += Nodo(texto,A) ; si f∈{PesosNumericos,ColProbables} y N==T → N=A
            "!texto" → N += Nodo(texto,T) ; si N!=F → N=T
            otro     → N += Nodo(m,F) ; N=F ; P=F ; raíz=F
   si grupo.ControladorTolerancias.Tolerancias.Count>0 y !grupo.EsGrupoBase:
      TG = Nodo("Tolerancias Globales")
      si grupo.AnalizaToleranciasGrupo(colG): P=A ; TG=A (no se añade al árbol)
      sino: P=F ; TG=F ; TG.título += " - Fallo en tolerancias Globales" ; P += TG
   grupos[i] = (P != F) ; hayFallos |= (P == F)
si hayFallos: raíz=F
// Control de grupos
si ControlesGrupos.Count > 1:
   CG = Nodo("Control de grupos", A) (segunda raíz) ; hayFallos=false
   para i=1..n-1: c=ControlesGrupos[i]
      nf = nº de g∈c.GruposControlados con !grupos[g] ; ok = c.FallosPermitidos[nf]
      si !ok: conjuntos[i]=false ; hayFallos=true ; estado=F
      sino:   conjuntos[i]=true ; estado=A ; para g∈c.GruposControlados: si raíz.Hijos[g]==F → T
      CG += Nodo("Control de Grupos i", estado)
   si hayFallos: CG=F  sino si grupos[0]: raíz=T
   // Control de conjuntos (solo si ControlesGrupos.Count>1)
   si ControlesConjuntos.Count > 1:
      CC = Nodo("Control de conjuntos", A) (tercera raíz) ; hayFallos=false
      para i=1..: cc=ControlesConjuntos[i] ; permitidos=cc.ObtenFallosPermitidos() ; conj=cc.ObtenCtrolGruposConjunto()
         nf = nº de k∈conj con !conjuntos[k] ; ok = permitidos[nf]            // usa conjuntos[] (control de grupos), no conjuntos2
         si !ok: conjuntos2[i]=false ; hayFallos=true ; F
         sino:   conjuntos2[i]=true ; A ; para k∈conj: si CG.Hijos[k-1]==F → T
         CC += Nodo("Control de Conjuntos i", estado)
      si hayFallos: CC=F  sino { si grupos[0]: raíz=T ; CG=T }
// If-Then
si IfThen!=nil && !IfThen.EsVacio && IfThen.EsActivo:
   IT = Nodo("Condiciones relacionadas", A)
   si ControlesCondiciones.Count>0: Q=Nodo("Condiciones sencillas",A) ; ev=IfThen.CompruebaErrores(columna) ; hijos por prefijo (*=A, !=T, otro=F con texto completo) ; si algún F: Q=F ; IT=F
   si ControlesGrupos.Count>0:     Q=Nodo("Grupos relacionados",A)   ; ev=IfThen.CompruebaErrores(columna, GruposPartidos) ; igual
EvaluarPronosticos(cg, pron): fallos=""; partido=0; mientras cg!=0: signo=(int)cg & 7 ; p=ConvPartidoStrToByte(pron[partido]) ;
      si (signo & p) != signo: fallos = "#"+(partido+1) + fallos         // se ANTEPONE ⇒ nº de partido en orden DESCENDENTE
      cg >>= 3 ; partido++
   si len(fallos)>=2: devuelve fallos[1…].Split('#') sino nil
NombreVisible: ColProbables→"Columnas probables", FormatosSignos→"Formatos de Signos", GruposEquipos→"Grupos de Equipos", NoInterrupciones→"Interrupciones", NoVariantes→"Variantes, X y 2", PesosNumericos→"Pesos Numericos"(sin tilde), SignosSeguidos→"Signos Seguidos", ValoracionSignos→"Valoracion de Signos"(sin tilde), resto→nombre del enum
```
- **Pistas T6/T5**: ningún orden de colección hash; todo depende del orden de `Grupos`, `Filtros` (lista del grupo) y de cada `AnalizarFallos`.
- El bucle del pronóstico termina cuando `cg==0` (partidos finales sin bits se ignoran); `Convert.ToInt16` de los nº.

**Salidas.** Árbol expandido (`IsExpanded=True`), cada nodo con semáforo (Verde/Neutro/Rojo) y título; leyenda de 3 estados; texto «Sin resultados todavía. El árbol se rellena al analizar una combinación.» si el árbol está vacío (`P:AnalizarCombinacionFrmPage.xaml:50-54`). Sin números.

**Área Mac propuesta.** Análisis → *Análisis de fallos*; **vista principal** con `OutlineGroup`/`List` jerárquica (o inspector de la columna cargada). Casos de uso: CU-A12 `AnalizarFallosCombinacion` (devuelve un árbol `NodoAnalisis{titulo, estado, hijos}`) y CU-A13 `EvaluarPronosticosBase`. Evidencia: `P:AnalizarCombinacionFrmViewModel.cs:78-561`, legacy `L:Analisis/AnalisisCombinacion.cs:95-330`.

---

### Pantalla 8 · AnalizarFicheroFrm — «Analizar fichero de columnas»

**Propósito.** Lanza el análisis estadístico completo (`Analizador.AnalizaCombinacion`) sobre un fichero de columnas externo y abre el visor de análisis. **Apertura.** Botón `bAnalisisColumnas` de la barra Combinación (`W:MainWindow.xaml.cs:397`); no está en menús.

**Entradas** (`P:AnalizarFicheroFrmViewModel.cs:25-48`, `P:AnalizarFicheroFrmPage.xaml`)
| Control | Detalle |
|---|---|
| Ruta del fichero | TextBox editable (TwoWay) + botón «Examinar…» (abrir `.txt`, `.cols`, `*`, `:58`). **Editar la ruta a mano no relee el fichero** (solo `AbrirFichero` carga `_columnas`) |
| Resumen | «N columnas.» (`:70`) |
| «Incluir pleno al 15» | `ToggleSwitch` Sí/No; habilitado solo si `columnas[0].Length == 15` (`:84`); **su valor no se lee en `Analizar`** (`IncluirPleno` no se usa en `:99-135`; «Abiertas» A8) |
| Cancelar | `Frame.GoBack()` si se puede (`P:AnalizarFicheroFrmPage.xaml.cs:20-24`) |
| Analizar | habilitado si hay ≥1 columna |

**Lógica de negocio en el VM**
```
AbrirFichero: cols = ArchivoColumnasTexto(ruta).LeerTodasCols(false)   // Trim+ToUpper por línea, sin formato
   ok → PuedeAnalizar = (n>0) ; PuedeIncluirPleno = (n>0 && cols[0].Length==15) ; si n==0 → IncluirPleno=false
   excepción → MostrarError("No se ha podido leer el fichero de columnas.") ; columnas=[]
Analizar: si ruta=="" → nada ; si columnas vacío → MostrarError("No se ha cargado el fichero de entrada o no tiene columnas.")
   Task.Run:
     partidos = ArchivoColumnasTexto(ruta).ObtenNumSignos()           // longitud de la 1.ª línea
     a = new Analizador(partidos) ; a.ArchivoColumnasBase = ruta
     para i<partidos: a.SetPronostico(i, "1,X,2")
     a.AnalizaCombinacion(analizarColumnas:true, esArchivo:true)
        → InicializarContenedorAnalisis(true); GeneradorColumnas(archColumnasBase).GenerarColumnas(true,true);
        → si ContenedorAnalisisColumnasGlobal != null && noColsAceptadas>0: UiPump.Pump(); AnalisisUi.MostrarVisor(contenedor, GruposPartidos[0])
   excepción → MostrarError("Error al analizar el fichero: " + msg)
```
- El **hook** `AnalisisUi.MostrarVisor` (`W:App.xaml.cs:170-206`) deja `VisorAnalisisColumnasFrmViewModel.UltimoContenedor/UltimoGrupo` y navega a `VisorAnalisisColumnasFrmPage` en el hilo de UI. En Swift: el caso de uso devuelve un `ContenedorAnalisis` y la UI decide mostrarlo (sustituye el hook global).
- Motor: `Analizador.AnalizaCombinacion(bool,bool)` `D:MotorCalculo/Analizador.cs:304` (versión sin `esArchivo` en `:256`); contenedor/visor en ESPEC_06 y pantallas Visor* (ESPEC_09/10).

**Ficheros.** Lee `.txt`/`.cols` (una columna por línea). No escribe. Estado global: ninguno (usa un `Analizador` propio, no el compartido).

**Salidas.** Texto «N columnas.» y, tras analizar, navegación al visor de análisis.

**Área Mac propuesta.** Análisis → *Analizar fichero*; **hoja modal** de selección de fichero que termina abriendo el visor como vista principal. Casos de uso: CU-A14 `AnalizarFicheroDeColumnas` (crea `Analizador` con N partidos y devuelve el contenedor). Evidencia: `P:AnalizarFicheroFrmViewModel.cs:55-135`.

---

### Pantalla 9 · Anastatics — «Estadísticas 0.3.4»

**Propósito.** Calcula, sobre un fichero de columnas de 14 signos, una de 4 estadísticas (dibujos, coincidencias con la columna anterior, interrupciones, signos seguidos) en una matriz 15×15 y luego abre la pantalla de resultados de ese modo. **Apertura.** Menú *Ver → Estadísticas…* y *Combinación → Estadísticas…*, botón `bEstadisticas` (`W:MainWindow.xaml.cs:190,231,402`).

**Entradas** (`P:AnastaticsViewModel.cs:44-70`, `P:AnastaticsPage.xaml`)
| Control | Detalle |
|---|---|
| Modo de análisis | lista de 4: 0 «Variantes, X, 2» (Dibujos), 1 «Sus coincidencias» (DibRepes), 2 «Interrupciones» (StaInter), 3 «Signos seguidos» (StaSigSeg); defecto 0 |
| «Selección origen» | abrir `.txt`/`*` (`:86`); muestra solo el nombre (defecto «Fichero origen») y «Columnas: N» (defecto «Columnas») |
| Calcular | habilitado tras origen válido (`OrigenSeleccionado`) |
| Mostrar resultados | habilitado tras calcular (`ResultadosListos`) |

**Lógica de negocio en el VM**
```
SeleccionarOrigen: resetea OrigenSeleccionado y ResultadosListos ; lee con ArchivoColumnasTexto.LeeColumnaSinComas (Trim+ToUpper)
   VerColumna(c): exige len==14 EXACTO y todos ∈ {1,2,x,X}, devuelve c con 'x'→'X' ; si no "" (error)
   para cada línea: v=VerColumna ; si v=="" → error en índice 0-based = lista.Count → MostrarError("columna errónea = " + idx) y abandona
                    añade ; si lista.Count == 500000 → break  (TRUNCA SILENCIOSAMENTE a 500 000 columnas)
   ok → numcol=lista.Count ; «Columnas» = numcol ; OrigenSeleccionado=true
   excepción → MostrarError("No se ha podido leer el fichero: " + msg)
Calcular: si numcol==0 → MostrarError("No hay columnas que analizar.") ; rsl = Proceso(modo) (Task.Run) → _rsl (15×15 int) ; ResultadosListos=true
Proceso(modo): rsl[15][15]=0
  0 Dibujos:   para cada col: ind=Dibujos.Procesar(col) ; rsl[ind[3]][ind[4]]++         // [nº de X][nº de 2]
  1 DibRepes:  num1=numx=num2=0 ; DibRepes (con estado «columna anterior», inicial "00000000000000")
               para cada col: ind=Procesar(col) // ind0=coincidencias posición a posición con la anterior; 2=#1; 3=#X; 4=#2
                  rsl[0][ind0]++
                  si num1==ind2: rsl[1][ind2]++ ; si numx==ind3: rsl[2][ind3]++ ; si num2==ind4: rsl[3][ind4]++
                  si (numx+num2)==(ind3+ind4): rsl[4][numx+num2]++
                  num1,numx,num2 = ind2,ind3,ind4        // previos; la 1.ª columna se compara con ceros
  2 StaInter:  rsl[0][ind0]++ (nº interrupciones) ; rsl[1][ind2]++ ; rsl[2][ind3]++ ; rsl[3][ind4]++ ; rsl[4][ind1]++
  3 StaSigSeg: rsl[0][ind2]++ (máx. 1 seguidos) ; rsl[1][ind3]++ (máx. X) ; rsl[2][ind4]++ (máx. 2) ; rsl[3][ind1]++ (máx. variantes seguidas)
MostrarResultados (solo si ResultadosListos): handoff por estáticos + navegar:
  0 → DibFormViewModel.MatrizEntrada = clon(rsl 15×15), NumColEntrada=numcol → DibFormPage
  1 → DibRepFrmViewModel.MatrizEntrada = primeras 5 filas × 15, NumColEntrada → DibRepFrmPage
  2 → StaInterFrmViewModel.MatrizEntrada/NumColEntrada + CargarDatos(rsl,numcol) → StaInterFrmPage
  3 → StaSSFormViewModel idem → StaSSFormPage
```
- Clases de motor (`Free1X2.Domain/MotorCalculo/Estadisticas/`): `Dibujos.Procesar` (`D:MotorCalculo/Estadisticas/Dibujos.cs:22`), `DibRepes.Procesar` (`D:MotorCalculo/Estadisticas/DibRepes.cs:24`, estado `old` por instancia), `StaInter.Procesar` (`D:MotorCalculo/Estadisticas/StaInter.cs:22`: `ind0`=nº cambios, `ind1`=cambios que acaban en `1`, `ind2/3/4`=cambios que parten de 1/X/2), `StaSigSeg.Procesar` (`D:MotorCalculo/Estadisticas/StaSigSeg.cs:21`: `ind1`=racha máx. de variantes, `ind2/3/4`=racha máx. de 1/X/2).
- Los índices usados (hasta 14) caben en 15×15; en modo 1 la matriz tiene 5 filas útiles.

**Ficheros.** Lee `.txt`. No escribe. Estado global: ninguno.

**Salidas.** Contador entero y navegación a la pantalla del modo (ver pantallas 38 `DibForm`, 39 `DibRepFrm` y StaInter/StaSS en ESPEC_09/10).

**Área Mac propuesta.** Análisis → *Estadísticas de fichero* (vista principal con selector de modo y resultados en el panel detalle en lugar de navegar). Casos de uso: CU-A15 `CalcularEstadisticasDeFichero` (devuelve `MatrizEstadistica{filas:[[Int]], numColumnas}`). Evidencia: `P:AnastaticsViewModel.cs:83-201,221-295`.

---

### Pantalla 10 · BancoPruebasFrm — «Simulador de escrutinios (Banco de Pruebas)»

**Propósito.** Asistente de 4 pasos para comparar combinaciones/columnas frente a «14 virtuales» (generados al azar según una valoración real) o frente a sí mismas, estimando premios (categorías 14..10) con el modelo de reparto del L.A.E. **Apertura.** Menú *Utilidades → Banco de pruebas…* y botón `bBancoPruebas` (`W:MainWindow.xaml.cs:267,429`). El botón «Grabar» abre `DialogoGrabarBancoPruebasFrm` (pantalla 36) como `ContentDialog` (`P:BancoPruebasFrmPage.xaml.cs:36-64`).

**Entradas por paso** (`P:BancoPruebasFrmViewModel.cs:106-257`, `P:BancoPruebasFrmPage.xaml`)
| Paso | Controles |
|---|---|
| 1 Ficheros | ruta (TextBox editable; **solo el botón «Examinar…» lee** el fichero, abrir `.txt`, `:301`); «Columnas leídas: N» |
| 2 Valoraciones | dos `PorcentajesControl` (apostados / reales, 14×3, ver §1.5); radio «LN de la Prob. media del 14» (`UsarLN` defecto sí) con NumberBox `LnProbMedia14` (defecto **-14.7**); radio «Premio medio de 14» (`PremioMedio14`, defecto 0); «Nº columnas a considerar» (`NumColumnasAConsiderar`, defecto 50000); botón «Calcular» |
| 3 Simular 14's | radio «Columnas aleatorias» (defecto) / «Fichero» (`FicheroAleatorias`); «Número de columnas» (defecto 1000); «Desviación Típica deseada» (1.965561); «LN mínimo» (-11); «Generar columnas»; «Guardar columnas»; resultados «Desviación Típica obtenida» y «LN Probabilidad Media obtenida» (defecto «0») |
| 4 Escrutinios | «Tipo de análisis» ∈ {Combinación, Columnas, Jornadas, Autoescrutinio} (defecto «Columnas»); casillas «Premios acumulados» 10..14 (defecto solo 14); «Analizar», «Seleccionar jornadas…» (sin efecto), «Grabar»; «Resultados globales»: Nº columnas, Gasto total, Premio total, % recuperado, Veces premiada, Veces beneficio (todas `"0"`); tres listas (Combinación / Columnas+Autoescrutinio con casilla por fila / Jornadas); «Eliminar las apuestas con menos de [2] diferencias», radio «Sólo respecto de la fila actual» (defecto) / «Respecto a cada fila seleccionada», «Fila Nº» (0), «Simplificar» |
Botón inferior «Cancelar» = `Frame.GoBack()` (`:66-70`). No hay barra de progreso ni cancelación en el port (el original tenía `progressBar1` y `salida`).

**Constantes del modelo L.A.E. cableadas en el VM** (`:543-547`): `Recaudacion=14 000 000`, `PrecioApuesta=0.5`, `PctDestinadoAPremiosCategoria={0.12,0.08,0.08,0.08,0.09}` (categorías 14,13,12,11,10), `DestinadoAPremiosCategoria[i]=Recaudacion*Pct[i]`. 3^14 = 4 782 969; `PotDe3`/`DosPotDe3` (`:138-139`).

**Codificación de columna a entero** (`ConvertidorDeBases`, `D:Utils/ConvertidorDeBases.cs:43-52,96-106`): partido 0 = dígito menos significativo; `'1'→0, 'X'→1 (cualquier carácter no '1'/'2'), '2'→2`; `ConvNumAColumna(n)` → `1,X,2` con relleno de `'1'` hasta `NumeroPartidos`. **Distinta** de la de `AnaCombi`/`AnalizadorJPM` (primer partido más significativo, `X=0`).

**Lógica de negocio en el VM (pseudocódigo)**
```
LeerColumnas: columnasLeidas = [ConvColumnaANumero(LeeColumnaSinComas())…]  // sin validar longitud/caracteres ; NumColumnasLeidasTexto=count
CalcularReales (Task.Run, deshabilitado si Ocupado):
   v = matriz(apostados) (double[14,3]) ; va = Porcentajes(v).ValoresNeperianos()  // float: ln( v[i][j] / Σ_j v[i][j] )  (D:Utils/Porcentajes.cs:329-349)
   Prob(float)=Σ_p va[p][0] ; Cra[p][1]=(float)(va[p][1]-va[p][0]) ; Cra[p][2]=(float)(va[p][2]-va[p][0])   // Cra[p][0]=0
   LN = LnProbMedia14 ; numCol=(int)NumColumnasAConsiderar     // UsarLN y PremioMedio14 NO se consultan (ver A5)
   Calcula14Triples:
      Ap14T = [3^14] de {Probabilidad:float, ProbabilidadDiferencial:float, Columna:int} ; Bits = todo true
      Encontrar(Prob, indice=0, posIni=0, prof=14): profundidad++ ;
          para p=posIni..13, z=1..2: idx = indiceIni + PotDe3[p]*z ; pr = pProb + Cra[p][z]
              Ap14T[idx] = {Columna=idx, Probabilidad=pr, ProbabilidadDiferencial=|pr − (float)LN|}      // (rama Bits[idx]==false inalcanzable)
              si profundidad < prof: Encontrar(pr, idx, p+1, prof)
          profundidad--
      Ap14T[0] = {Columna=0, Probabilidad=Prob, ProbabilidadDiferencial=|Prob − (float)LN|}
      ordena(0, 3^14−1)   // quicksort propio ASCENDENTE por ProbabilidadDiferencial (float), pivote = elemento central (izq+der)/2,
                          // esquema Hoare con guardas «&& j<=der» / «&& j>izq»; NO estable: con empates el orden depende del algoritmo → copiar tal cual
      p[14][3]=0 ; para i<numCol: para cada partido: p[partido][(Ap14T[i].Columna / PotDe3[partido]) % 3]++
      p[partido][k] /= (numCol / 100)      // (numCol/100) es DIVISIÓN ENTERA; con numCol<100 divide por 0 (→ ∞/NaN, sin excepción)
      → matriz reales (14×3, en %), se carga en la rejilla «reales»
GenerarAleatorias (SÍNCRONO en hilo de UI):
   p=matriz(reales) ; v=matriz(apostados) ; rng = Random(semilla = unchecked((int)DateTime.Now.Ticks))   // P:BancoPruebasFrmViewModel.cs:368; semilla de reloj, no reproducible (A19) ; n=(int)NumAleatorias
   para z<n: prob=1 ; b=0 ; para j<14: u=rng.NextDouble()
        si u < p[j][0]/100: prob*=v[j][0]/100
        sino si u < (p[j][0]+p[j][1])/100: prob*=v[j][1]/100 ; b+=PotDe3[j]
        sino: prob*=v[j][2]/100 ; b+=DosPotDe3[j]
      mpx=(mpx*z+prob)/(z+1) ; ln=log(prob) ; lnm=log(mpx) ; lnMedia=(lnMedia*z+ln)/(z+1)
      lnVari=(lnVari*z+(ln−lnm)²)/(z+1) ; lnDT=sqrt(lnVari) ; aleatorias.add(b)
   muestra lnDT y lnMedia con double.ToString() (cultura actual)      // `DesviacionTipicaDeseada` (def. 1.965561) y `LnMinimo` (def. -11) se declaran en `P:BancoPruebasFrmViewModel.cs:172,175` y no se leen en ningún punto (A9)
GuardarAleatorias: si vacío → MostrarInfo("No hay columnas aleatorias generadas que guardar.") ; guardar `.txt` (sugerido «ColumnasAleatorias») con GuardarCols(int)→ConvNumAColumna
CalcularPremios(ganadora) (por cada 14 virtual):
   pa = Porcentajes(v).ValoresBase100() (float: v/Σ) ; prob14 = Π_p pa[p][signo_p] (double) ; Cr[p] = (float)((1−pa[p][signo])/pa[p][signo])
   SumaProb[0..4]=0 ; Premios[0] = Round(Pct[0]*0.5 / prob14, 2)          // Math.Round(x,2): bancario
   SumaProbabilidades(prob14, pos=0, prof=4): prof++ ; para p=pos..13: pr = pProb*Cr[p] ; SumaProb[prof] += pr ; si prof<4: recursión(pr, p+1) ; prof--
   para i=1..4: Premios[i] = Round(Pct[i]*0.5 / SumaProb[i], 2)
   Correcciones: Premios[i] = min(Premios[i], Recaudacion*Pct[i]) ; si Premios[4] < 1 → Premios[4]=0
Aciertos(a,b) = nº de partidos p<14 con dígito_p(a)==dígito_p(b)
Analizar: sin fichero → MostrarInfo("Selecciona primero el fichero de columnas (paso 1).") ; tipo≠Autoescrutinio y sin aleatorias → MostrarInfo("Genera primero las columnas aleatorias (paso 3).") ; excepción → MostrarError("Error en el escrutinio: …")
 • Combinación: cols=fichero ; coste=cols.Count*0.5 ; gasto=coste*nAleatorias
     para cada aleatoria i: CalcularPremios(i) ; beneficio=−coste ; premiada=false
        para cada j∈cols: n=Aciertos(i,j) ; aciertos[n]++ ; si n>9: ingresos[14−n]+=Premios[14−n] ; premiada=true ; beneficio+=Premios[14−n]
        premiada→vecesPremiada++ ; beneficio>0→vecesBeneficio++
     filas de texto i=0..14: "{i} aciertos\tveces: {aciertos[i]}\tpremios: {Round(ingresos[14−i],0) si i≥10 sino 0}\tmedia: {aciertos[i]>0 ? Round(premios/aciertos[i],2) : 0}"
     premioTotal=Round(Σ_{i=10..14} ingresos[14−i], 0) ; recuperación=Round(100*premioTotal/gasto, 0) → «N%»  (gasto=0 ⇒ NaN/∞ en texto)
 • Columnas: resCol[j] por columna del fichero ; por cada aleatoria i: CalcularPremios(i) ; para cada j: n=Aciertos(i,j) ; aciertos[n]++ ; si n>9: resCol[j].veces[14−n]++, resCol[j].premio[14−n]+=Premios[14−n]
     gasto=coste*nAleatorias ; premioTotal=Round(Σ_j Σ_k Round(premio_j[k]),0) ; recuperación = gasto>0 ? Round(100*premioTotal/gasto,0) : 0
     fila resumen «SUMAS » (aciertos globales, Premios[k]=Σ_j Round(premio_j[k])) se calcula pero NO se muestra
     por fila: Premio = Round(Σ_{k acumulable} premio_j[k], 1) ; Recuperación = Round(Suma*100/nAleatorias/0.5, 0) + " %" (Suma sin redondear)
 • Autoescrutinio: igual que Columnas pero el 14 es CADA columna i del fichero y se escruta contra todo el fichero (incluida ella misma, n=14):
     los premios/veces se acumulan en resCol[i] (la columna «ganadora»), gasto=coste (sin × aleatorias), nEscrutinios=nApuestas ; no se muestra PremioTotal ni %, solo «Veces beneficio»
 • Jornadas: para cada aleatoria i (jornada k=1..): CalcularPremios(i) ; aciertos[5], ingresos[5] sobre todo el fichero (solo n>9)
     saldo_k = saldo_{k−1} − coste + Σ ingresos ; fila: Jornada, #14..#10, Importe=Round(Σ ingresos,2), Saldo=Round(saldo_k,2)
Grabar: requiere resCol (modos Columnas/Autoescrutinio) ; c1=primera fila marcada, c2=última, c=nº marcadas (todas 1-based tras ++) ;
   diálogo(c1,c2,c) → (aceptado, filaIni, filaFin, numMax, soloSel) ; c1=filaIni ; c2=filaFin+1 ; si c2>nApuestas−c1: c2=nApuestas−c1 ; c=numMax ; c1-- ; c2--
   guardar `.txt` (sugerido «columnas») : filas[c1 .. min(nApuestas, c2+1)) ; si soloSel y no marcada → salta ; GuardarCols(columna) ; conta++ ; si numMax>0 y conta==numMax → fin
   MostrarInfo("Grabadas N columna(s) en <fichero>.") ; error → MostrarError("No se pudieron grabar las columnas en:\n<ruta>\n\n<msg>")
Simplificar: requiere resultado por columnas (MostrarInfo "No hay resultado por columnas que simplificar…")
   dif = (byte)(14 − (int)DiferenciasMinimas)
   «Sólo fila actual»: fila=(int)NumFila (índice 0-BASADO en la lista ; fuera de rango → MostrarInfo "El nº de fila está fuera de rango.") ; c=número(fila.Columna)
        Deselecciona(c, 0, dif) ; fila.Seleccionado = true
   «Cada fila seleccionada»: c=0 (COLUMNA «todo 1», variable nunca reasignada — preservar) ; para cada fila i seleccionada: Deselecciona(0, i, dif)
   Deselecciona(c, ini, dif): para i≥ini seleccionada: si c==fila_i.ColumnaNumero → continúa ; si Aciertos(c,fila_i) > dif → fila_i.Seleccionado=false
```
- `CalcularReales`/`Analizar` bloquean mutuamente con `Ocupado`. «Seleccionar jornadas…» no hace nada (el botón original estaba oculto, `:508-515`).
- Divergencias con el original (`L:UI/BancoPruebasFrm.cs`): sincronización LN↔Premio (`:4082-4108`: `LN=ln(0.5*0.12/Premio)`, `Premio=Round(0.5*0.12/exp(LN),0)`), lectura del fichero de aleatorias (`:3258-3262`), fila SUMAS visible, barra de progreso/cancelar; defecto LN `-14,75` (`:543`) frente a `-14.7` del port → «Abiertas» A5.

**Ficheros.** Lee/escribe `.txt` de columnas (14 signos). Estado global: ninguno (campos privados del VM: `v, p, Cr, pa, Cra, Ap14T` = ≈115 MB de `ApuestaProbableCentral` 12 B × 4,78 M).

**Salidas.** Resultados globales (texto, `double.ToString()` sin formato fijo; cultura actual → fijar `en_US`/`es_ES` explícito, T7); listas por modo. Fila de columna: `"{Columna}    14:{v14}  13:{v13}  12:{v12}  11:{v11}  10:{v10}    premio:{Premio}  {Recuperación}"` (`:54-56`); fila de jornada: `"Jornada {n}    14:… 10:…    premios:{Importe}  saldo:{Saldo}"` (`:85-87`).

**Área Mac propuesta.** Herramientas → *Simulador de escrutinios*; vista principal con `TabView`/pasos y tabla `Table` ordenable con selección múltiple; el rango de grabación como hoja modal. Casos de uso: CU-A16 `ObtenerValoracionRealDesdeApostada`, CU-A17 `GenerarColumnasAleatorias14`, CU-A18 `CalcularPremiosJornadaVirtual`, CU-A19 `EscrutarCombinacionBanco`, CU-A20 `EscrutarColumnasBanco`, CU-A21 `AutoescrutinioBanco`, CU-A22 `EscrutarPorJornadas`, CU-A23 `SimplificarFilasPorDiferencias`, CU-A24 `GrabarColumnasSeleccionadas`. Evidencia: `P:BancoPruebasFrmViewModel.cs:298-1376`.

---

### Pantalla 11 · BoletoFrm — «Boleto» (visor de boletos imprimibles)

**Propósito.** Muestra, de 8 en 8, las columnas de un fichero dispuestas sobre el boleto oficial (8 columnas × 16 apuestas) y permite navegar entre boletos. **Apertura.** Desde `VerBoletos` (pantalla de ESPEC_09/10) vía el estático `BoletoFrmPage.ParametrosBoleto = (fichero, orden, tipo)` (`P:BoletoFrmPage.xaml.cs:27,46-58`, productor `P:VerBoletosPage.xaml.cs:36`); también de forma autónoma (botón «Abrir archivo»). No aparece directamente en menús (los menús abren `VerBoletosPage`, `W:MainWindow.xaml.cs:188,214`).

**Entradas** (`P:BoletoFrmViewModel.cs:37-85`, `P:BoletoFrmPage.xaml`)
| Control | Detalle |
|---|---|
| «Abrir archivo» | abrir `.txt` (`P:BoletoFrmPage.xaml.cs:85-91`), solo fija `ArchivoCombinacion` |
| «Cargar boletos» | ejecuta la carga |
| `OrdenarPor` ∈ `OrdenarMatriz{Signo, Variantes, Equis, Doses, SignosSeguidos, Interrupciones}` (`D:Enums.cs:59-67`) y `TipoOrden{asc, desc}` (`:69-73`) | no editables en esta pantalla; llegan del handoff; defecto `Signo`/`asc` |
| Navegación `|<  <  [Boleto nº] Ir de N  >  >|` | NumberBox `Mínimo=1` (`xaml:66-70`), defecto 1 |

**Lógica de negocio en el VM**
```
Cargar: ruta vacía → Estado="No se ha seleccionado ningún archivo." ; Estado="Cargando boletos..."
   Task.Run:
     capacidad = ObtenNumCols() (nº de líneas) ; cols = string[capacidad]
     nBoletos = capacidad%8==0 ? capacidad/8 : Convert.ToInt32(capacidad/8 + 0.5000001)   // = ceil(capacidad/8)
     para i<capacidad: cols[i] = SiguienteColumna ? LeeColumnaSinComas() : ""            // Trim + ToUpper
     OrdenarMatrizColumnas(cols, orden, tipo)
   TotalBoletos = nBoletos<=0 ? 1 : nBoletos ; LlenarBoleto(0) ; Estado = "<nombre>: N boleto(s)."
   excepción → Estado="Error al cargar el boleto: …" + MostrarError("No se pudo cargar el boleto: …")
OrdenarMatrizColumnas(cols, orden, tipo):
   Signo:  Array.Sort(cols) [comparador de cadenas de .NET: sensible a cultura; para '1','2','X' mayúsculas equivale a '1'<'2'<'X'] ; si desc → Reverse
   resto: m[i] = métrica(cols[i]):  Variantes→FiltroNoVariantes.NoVariantes ; Equis→.NoEquis ; Doses→.NoDoses ; Interrupciones→FiltroInterrupciones.NoInterrupciones ;
                SignosSeguidos→ NO calculado (m[i]=0 para todas ⇒ orden de fichero)
          asc : para v=0..len(cols[0]): añade, en orden de fichero, las cols con m==v
          desc: para v=len..0  (m == len−i): añade, en orden de fichero, las cols con m==v      // NO invierte el orden interno del grupo
          copia sobre cols (si alguna m queda fuera de 0..len, se pierde y queda null)
LlenarBoleto(b0): b0 se acota a [0, TotalBoletos−1] ; para i=0..7: idx=b0*8+i ; col = idx<apuestas ? cols[idx] : "" ; nº columna = col vacía ? 0 : idx+1
   emite BoletoCambiado(signos[8], numeros[8]) → BoletoMatrizControl.Llenar ; BoletoActual=b0+1 ; IrABoleto=BoletoActual
Primero=LlenarBoleto(0) ; Anterior=LlenarBoleto(BoletoActual−2) ; Siguiente=LlenarBoleto(BoletoActual) ; Ultimo=LlenarBoleto(Total−1)
Ir: d=(int)IrABoleto (trunca) ; d<1 o d>Total → Estado="El número de boleto está fuera de rango." sino LlenarBoleto(d−1)
```
- Métricas con `fVar.AnalizaColumna(c)`/`fInt.AnalizaColumna(c)`: ver ESPEC_02/03 (`D:MotorCalculo/FiltroNoVariantes.cs`, `D:MotorCalculo/FiltroInterrupciones.cs`).
- Una línea vacía/erróneamente corta puede romper `AnalizaColumna`; el port no valida (el legacy tampoco).

**Componente compartido `BoletoMatrizControl`** (`W:Controls/BoletoMatrizViewModel.cs`): 8 `ColumnaBoletoViewModel`, cada una con cabecera = nº de columna (vacío si 0), 16 `ApuestaBoletoViewModel` y pie de aciertos (vacío si ≤0; resaltado si ≥10, `:96-105`). Cada apuesta marca la ranura `1`/`X`/`2` (`x`/`X` equivalen); las filas más allá de la longitud de la columna se ocultan (`:112-143`); separación extra tras los partidos 4, 8, 11 y 14 (`:27`). El visor pasa `aciertos=0`.

**Ficheros.** Solo lectura de `.txt`. Estado global: ninguno.

**Salidas.** Boleto visual, «Boleto nº [k] de N», mensaje de estado (`Estado`).

**Área Mac propuesta.** Cálculo → *Boletos* (vista principal con `Canvas`/`Grid` del boleto, atajos ←/→); la impresión (pantalla de impresión, ESPEC_09/10) como acción de la misma vista. Casos de uso: CU-A25 `PrepararBoletos` (leer + ordenar + trocear de 8 en 8) y CU-A26 `OrdenarColumnasPorCriterio`. Evidencia: `P:BoletoFrmViewModel.cs:91-309`.

---

### Pantalla 12 · BuscaLimsFrm — «Buscador límites»

**Propósito.** Con una valoración 1/X/2 por partido, recorre las 3^N columnas posibles y, para las que coinciden con la «columna base» (el signo mejor valorado por partido) en un número de aciertos dentro de un rango, calcula el mínimo y máximo de la suma y del producto de valoraciones (límites a usar en la condición Valoración). **Apertura.** Desde la pantalla de la condición *Valoración* (`P:ValoracionFrmViewModel.cs:365`); en el registro (`W:Navigation/PortedPages.cs:87`), sin menú propio.

**Entradas** (`P:BuscaLimsFrmViewModel.cs:26-71`, `P:BuscaLimsFrmPage.xaml`)
| Control | Detalle |
|---|---|
| Rejilla de valoraciones | `PorcentajesControl` N×3 (ver §1.5), `double`, Mín 0 |
| «Rango» | texto, defecto `4-9`, placeholder «p. ej. 4-9» (aciertos permitidos respecto a la columna base) |
| «C.B.» | columna base calculada (solo lectura) |
| «Calcular Límites» | deshabilitado mientras corre |
| Resultados | Sumas Mín/Máx y Productos Mín/Máx (TextBox solo lectura, vacíos al empezar); Procesadas, Admitidas (`"0"`), Tiempo |

**Lógica de negocio en el VM (pseudocódigo)**
```
Calcular (Task.Run; PuedeCalcular=false; limpia las 4 cajas):
  N = VariablesGlobales.NumeroPartidos (si <=0 → 14) ; dv = matriz(rejilla) (double[N,3])
  min=-1 ; max=0 ; minProd=-1 ; maxProd=0 ; nca=0 ; ncv=0
  columnaBase = CalculoCB():  para cada partido i: 
        si dv[i][0] >= dv[i][1] && dv[i][0] >= dv[0][2]   → "1"      // ← usa dv[0][2] (fila 0), no dv[i][2]: «réplica exacta de la condición legacy» → PRESERVAR
        sino si dv[i][2] >= dv[i][0] && dv[i][2] >= dv[i][1] → "2"
        sino "X"           ; baseLong = ConvStrToLong(columnaBase)
  rangos = BitArray(N+1) ← UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(rango): lista de enteros, admite "a-b", "a,b", "a-b,c" ; valores fuera de [0,N] se ignoran
  AnalizarColumnas("", 0): recursión pos=0..N−1 con signos en el orden "1","X","2" ; en la hoja (columna completa):
        nca++ ; aciertos = ContarBitsA1(baseLong & ConvStrToLong(col)) ; si rangos[aciertos]: ncv++ ; ObtenerValoracion(col)
  ObtenerValoracion(col): suma=0 ; prod=1 ; para i: signo s∈{1→0, X→1, 2→2}: suma += dv[i][s] ; prod *= dv[i][s] * 0.03420425138   // orden izquierda→derecha, double
        si min>suma o min==-1: min=suma ; si max<suma: max=suma ; si minProd>prod o minProd==-1: minProd=prod ; si maxProd<prod: maxProd=prod
  Presentación: columna base ; SumaMin si min>-1 ; SumaMax si max>0 ; ProductoMin si minProd>-1 ; ProductoMax si maxProd>0   (double.ToString(): cultura actual)
     Procesadas=nca ; Admitidas=ncv ; Tiempo = (TimeSpan.ToString()+"00000000000").Substring(0,11)
  excepción → Tiempo = "Error: " + mensaje
```
- Si ninguna columna entra en rango, las 4 cajas quedan vacías. Un rango sin números válidos o con texto lanza dentro de `ObtenerListaFromTxtAciertos` (el try/catch interno devuelve lo acumulado; ver ESPEC_06).
- Coste: 3^14 = 4 782 969 hojas; el port concatena cadenas en cada nivel (en Swift usar enteros base 3 / `UInt64` con bits 3-por-partido, T2).

**Ficheros.** Ninguno. Estado global: lee `VariablesGlobales.NumeroPartidos`.

**Salidas.** Tabla 2×2 Mín/Máx × Sumas/Productos; contadores enteros; tiempo.

**Área Mac propuesta.** Condiciones y filtros → *Valoración* → acción «Buscar límites» como **hoja modal** (inspector con la rejilla heredada de la condición). Casos de uso: CU-A27 `BuscarLimitesValoracion`. Evidencia: `P:BuscaLimsFrmViewModel.cs:87-256`.

---

### Pantalla 13 · CalculaColumnasFrm — «Calcular columnas»

**Propósito.** Lanza el motor sobre la combinación en pantalla (boleto base + grupos + condiciones + If-Then + filtro de columnas) en uno de tres modos —contar, contar y analizar, grabar a fichero— y muestra contadores, costes, % de aceptación y tiempos en vivo. **Apertura.** Botón «Calcular» de Inicio (`V:MainPageViewModel.cs:540-556`: vuelca el boleto al motor, fija `analizador.ArchivoColumnasBase = filtro verde ? archivoFiltro : ""`, `AppState.UsarAnalizadorCompartido = true`), menú *Combinación → Calcular…*, botón `bCalcular`, tecla **F5** (`W:MainWindow.xaml.cs:211,389,769-770`). Desde menú/barra/F5 **no** activa el handoff ⇒ usa un `Analizador` propio (ver abajo).

**Entradas** (`P:CalculaColumnasFrmViewModel.cs:49-128`, `P:CalculaColumnasFrmPage.xaml`)
| Control | Detalle |
|---|---|
| Modo (radio) | «Calcular» (defecto) / «Calcular y analizar» / «Grabar archivo» |
| «Incluir pleno al 15» | casilla; habilitada solo en «Calcular y analizar»; al salir de ese modo se desmarca (`:165-170`) |
| «Seleccionar archivo…» + nombre | solo en modo Grabar; guardar `.txt`, nombre sugerido `Columnas` (`:267`); defecto «(Seleccionar)» |
| Cancelar | `PararAnalisis()` + `CancellationToken` + vuelve atrás (`P:CalculaColumnasFrmPage.xaml.cs:21-25`, `:358-365`) |
| Calcular | arranca; ignorado si ya calcula |
| `Pronosticos` | colección de 14 cadenas `"1,X,2"` en modo independiente (no hay control para editarla en la página actual) |

**Origen del `Analizador`** (`:130-161`): con `AppState.UsarAnalizadorCompartido` (se consume y se reinicia a `false`) → `AppState.Instancia.Analizador` (ya poblado); si no → `new Analizador()` con los `NumeroPartidos` pronósticos a `"1,X,2"` (3^14 columnas, sin condiciones).

**Lógica de negocio en el VM (pseudocódigo)**
```
RecalcularMaximas():
   si analizador.ArchivoColumnasBase != "": colsMax = nº de líneas de ese fichero (ObtenNumCols) (excepción → 0)
   sino: dobles = nº de pronósticos con 2 signos (tras quitar comas) ; triples = con 3 ; (en modo independiente aplica SetPronostico(i, texto))
         colsMax = (long)(2^dobles * 3^triples)   // pow en double → Int64
   ColsMaximo = colsMax.ToString("#,##0;0") ; CosteMaximo = (colsMax * PrecioApuesta).ToString(Moneda + "#,##0.00;0.0")
Calcular:
   RecalcularMaximas()
   si ModoGrabar y archivo vacío → MostrarError("Seleccione un archivo de resultados.")
   si ModoGrabar y HayConflictos → MostrarError("No puede usar como archivo de resultados un archivo usado ya en la combinación")
        HayConflictos: para cada grupo g: g no base y g.UsaFiltroParcial y archivo == g.ArchivoFiltroGrupo → true ; g base y archivo == analizador.ArchivoColumnasBase → true   // comparación de cadenas exacta, sin normalizar rutas
   Calculando=true ; Estado="Calculando..." ; HoraComienzo = hora larga ; temporizador UI cada 500 ms → ActualizaDatosCalculo()
   Task.Run: ModoGrabar → analizador.AnalizaCombinacion(archivo)            // grabar; NO analiza columnas
             ModoAnalizar → analizador.AnalizaCombinacion(IncluirPleno ? 15 : 14)   // asigna numPartidos y analiza; abre el visor al terminar
             si no       → analizador.AnalizaCombinacion(false)
   excepción → MostrarError("Error en el cálculo: " + msg) ; finally: parar temporizador, HoraFinal, TiempoTranscurrido = TimeSpan.ToString(), ActualizaDatosCalculo(), Estado="Listo"
Cancelar → Estado="Cancelado" (el finally de Calcular vuelve a poner «Listo»)
ActualizaDatosCalculo():
   P = analizador.ColsAnalizadas ; A = ColsAceptadas ; precio = PrecioApuesta ; moneda = Moneda
   ColsProcesadas = P.ToString("#,##0;0") ; CosteProcesadas = (P*precio).ToString(moneda+"#,##0.00;0.0")
   Progreso = colsMax>0 ? min(100, P*100.0/colsMax) : 0
   ColsAceptadas = A... ; CosteAceptadas = (A*precio)...
   pct = P>0 ? A*100.0/P : 0 ; Porcentaje = pct.ToString("#,##0.00;0.00") + " %"
   est = Math.Round(colsMax*pct/100, 0)  // redondeo bancario (T4)
   ColsEstimadas = est.ToString("#,##0;0") ; CosteEstimadas = (est*precio).ToString(moneda+"#,##0.00;0.0")
```
- Los formatos son .NET personalizados; el separador de miles/decimal y el símbolo `Moneda` (defecto «€», `D:VariablesGlobales.cs:161,194`) dependen de la cultura actual: fijar `Locale` explícito (T7). `PrecioApuesta` es 0.75 solo si falla la carga de configuración (`D:VariablesGlobales.cs:158`); el `W:Datos/parametros.free1x2` entregado trae `PrecioApuesta="0,50"`, `Moneda="€"` (`W:Datos/parametros.free1x2:10`).
- `Analizador.AnalizaCombinacion(bool|string|int)`: `D:MotorCalculo/Analizador.cs:182-201,256,304`; contadores `ColsAnalizadas`/`ColsAceptadas` `:481-489`; `PararAnalisis` `:448-455`. En «Calcular y analizar» el visor se abre vía el hook `AnalisisUi.MostrarVisor` si hay columnas aceptadas (`D:MotorCalculo/Analizador.cs:295-299`).
- La actualización cada 500 ms es solo presentación: en Swift usar `AsyncStream` de progreso (T12).

**Ficheros.** Modo Grabar: escribe `.txt` de columnas mediante el motor (`archivoCols`); el modo con filtro de columnas base lee ese fichero (`ArchivoColumnasBase`). Estado global: `AppState.Analizador`, `AppState.UsarAnalizadorCompartido`, `VariablesGlobales.PrecioApuesta/Moneda/NumeroPartidos`.

**Salidas.** Tabla Procesadas / Aceptadas / % aceptación / Estimadas / Máximo (columnas y coste), barra de progreso 0–100, Comienzo/Final (`DateTime.ToLongTimeString()`), Transcurrido (`TimeSpan.ToString()`), estado «Preparado»/«Calculando...»/«Cancelado»/«Listo».

**Área Mac propuesta.** Cálculo → *Calcular* (vista principal con panel de progreso; acción también desde la barra de herramientas y ⌘R/F5). Casos de uso: CU-A28 `CalcularCombinacion(modo: contar|analizar(pleno)|grabar(archivo))`, CU-A29 `ValidarArchivoResultados`, CU-A30 `EstimarColumnasMaximas`, CU-A31 `EstadisticasDeCalculo`. Evidencia: `P:CalculaColumnasFrmViewModel.cs:187-391`.

---

### Pantalla 14 · CalculaColumnasMultipleFrm — «Calcular Múltiples Combinaciones»

**Propósito.** Procesa en lote varios ficheros de combinación (`.comb`/`.xml`), genera las columnas de cada uno en la carpeta «Columnas» como `.txt` y al terminar abre el resumen (`ResultadosCalculoMultipleFrm`). **Apertura.** Menú *Combinación → Calcular varias…* y botón `bCalcularM` (`W:MainWindow.xaml.cs:212,390`). Navega a `ResultadosCalculoMultipleFrmPage` al acabar (`P:CalculaColumnasMultipleFrmViewModel.cs:267-269`).

**Entradas** (`:54-102,106-131`, `P:CalculaColumnasMultipleFrmPage.xaml`)
| Control | Detalle |
|---|---|
| «Seleccionar archivos…» | selección múltiple, filtros `.comb`, `.xml`, `*` (`PickerHelper.AbrirVariosAsync`, `:112`); sustituye la lista; ordena rutas con `StringComparer.Ordinal` (`:123`) y muestra solo el nombre |
| Lista de ficheros | `SelectedIndex` se mueve sola durante el proceso (`FicheroSeleccionado = i`) |
| Calcular | habilitado si hay ≥1 fichero y no hay proceso (`PuedeCalcular`) |
| Cancelar | `PararAnalisis()` + token + `ProcesoEnMarcha=false` (`:273-280`) |
| Estadísticas | Procesadas/Aceptadas/% /Estimadas/Máximo (columnas y coste del **archivo en curso**), Comienzo/Final/Tiempo total, dos barras (archivos y columnas del archivo) |

**Lógica de negocio en el VM (pseudocódigo)**
```
Calcular (ignora si ya en marcha o lista vacía):
   destino = <BaseDirectory>/Columnas ; crear (error → MostrarError("No se pudo acceder a la carpeta de salida: …"))
   salida[i] = destino/ nombreOrigen.Replace(".comb",".txt").Replace(".xml",".txt")      // Replace sensible a mayúsculas y a TODAS las apariciones; «a.comb» y «a.xml» colisionan en «a.txt»
   si algún destino existe → diálogo único «Algunos archivos de columnas ya existen. ¿Deseas reemplazarlos?» [Reemplazar (defecto) / Cancelar] ; Cancelar → no hace nada
        (el original WinForms preguntaba por cada fichero, L:UI/CalculaColumnasMultipleFrm.cs)
   ProcesoEnMarcha=true ; Resultados.Clear() ; HoraComienzo ; temporizador UI 500 ms → ActualizaDatosCalculo()
   para i en orden:  (corta si se canceló)
        ProgresoArchivos = (i+1)*100.0/N
        Task.Run: arch = ArchivoCombinacion(); arch.AbrirArchivoCombinacion(origen) ; analizador = new Analizador()
                  arch.CargaControladorGrupos(analizador.CtrlGrupos)
                  analizador.ArchivoColumnasBase = arch.LeeFiltroColumnas() ; analizador.Pronosticos = arch.LeePronosticos() ; arch.Pronosticos = analizador.Pronosticos
                  ActualizaColumnasPrevistas() ; analizador.AnalizaCombinacion(rutaSalida)          // GRABA (no analiza)
                  // no se llama a nada de If-Then en este flujo (a diferencia de la pantalla Calcular)
        Resultados += {ArchivoCombinacion: nombre, ArchivoColumnas: nombre salida, ColumnasAnalizadas, ColumnasAceptadas, Tiempo: (ahora − t_anterior).ToString()}
   catch cancelación → sale limpio ; catch → MostrarError("Error en el cálculo en lote: " + msg)
   finally: parar temporizador, HoraFinal, TiempoTotal=TimeSpan.ToString(), ActualizaDatosCalculo(), ProcesoEnMarcha=false
   ResultadosCalculoMultipleFrmViewModel.UltimosResultados = copia(Resultados) ; navegar a ResultadosCalculoMultipleFrmPage   // incluso tras cancelar/error
ActualizaColumnasPrevistas: si ArchivoColumnasBase != "": existe → colsMax = ObtenNumCols ; no existe → MostrarInfo("No se ha encontrado el filtro <ruta>. Se calculan las columnas sin filtro.") ; ArchivoColumnasBase="" ; colsMax = 2^dobles·3^triples
                            si no: colsMax = (long)(2^dobles · 3^triples) (dobles = pronósticos con 2 signos sin comas; triples = 3 signos)
                            texto máximo = colsMax.ToString("#,##0;0") ; coste = (colsMax·PrecioApuesta).ToString(Moneda+"#,##0.00;0.0")
ActualizaDatosCalculo: idéntico al de CalculaColumnasFrm (pantalla 13): formatos "#,##0;0", Moneda+"#,##0.00;0.0", "#,##0.00;0.00"+" %", estimadas = Math.Round(colsMax·pct/100, 0)
```
- Valores iniciales mostrados: `"0"`, `"€ 0,00"`, `"0,00 %"`, `"0,0"`, `"00:00:00"` (literales con coma decimal española, `:73-92`), distintos del formato que luego produce `ToString(...)` en la cultura vigente.
- `ObtenNumCols`/`ArchivoCombinacion`: `D:EntradaSalida/ArchivoCombinacion.cs:64,76,97`; formato `.comb` en ESPEC_07.

**Ficheros.** Lee `.comb`/`.xml` (+ el fichero de filtro de columnas de cada uno si existe). Escribe `<BaseDirectory>/Columnas/<nombre>.txt` (mac: `~/Library/Application Support/Free1X2/Columnas/`, propuesta). Estado global: `VariablesGlobales.PrecioApuesta/Moneda`; **no** toca `AppState`.

**Salidas.** Contadores y costes del archivo en curso, progreso doble, y tras el lote la tabla de `ResultadosCalculoMultipleFrm` (ESPEC_09/10).

**Área Mac propuesta.** Cálculo → *Cálculo por lotes* (vista principal con lista de ficheros arrastrables + panel de progreso; diálogo de reemplazo como `confirmationDialog`). Casos de uso: CU-A32 `CalcularLoteDeCombinaciones(ficheros, carpetaDestino)` (devuelve `[ResumenCalculo]`), CU-A33 `NombreSalidaDeCombinacion`. Evidencia: `P:CalculaColumnasMultipleFrmViewModel.cs:134-373`.

---

### Pantalla 15 · CalculoFormatosFrm — «Saca Formatos (Indeciso)»

**Propósito.** Dada una columna de 14 signos, lista los «formatos» (pares, tríos, cuartetos y quintetos consecutivos distintos y cuántas veces aparece cada uno) y los contactos entre signos, y vuelca el informe a un fichero de texto. **Apertura.** Botón de la pantalla de la condición *Formatos* (`P:FormatosFrmPage.xaml.cs:47`); en el registro (`W:Navigation/PortedPages.cs:21`), sin menú.

**Entradas** (`P:CalculoFormatosFrmViewModel.cs:25-52`, `P:CalculoFormatosFrmPage.xaml`)
| Control | Detalle |
|---|---|
| «Seleccionar fichero…» | guardar `.txt`, nombre sugerido `Informe` (`:46`); texto «Falta Fichero Salida» por defecto |
| «Columna (14 signos: 1 / X / 2)» | TextBox `MaxLength=14`, placeholder «Ej. 1X21X2111XX221», actualiza al teclear. El WinForms forzaba mayúsculas (`CharacterCasing.Upper`); el port **no** |
| «Sacar Formato» | calcula y guarda |
| Informe | TextBox multilínea solo lectura |

**Validaciones** (`:62-72`): sin fichero de salida → `MostrarError("Falta seleccionar archivo de salida")` y aborta; longitud ≠ 14 → `MostrarError("numero de caracteres en la columna distinto de 14")` pero **continúa**. No se validan caracteres.

**Lógica de negocio en el VM**
```
col = Columna
para cada posición (hasta agotar col, recortando 1 carácter por la izquierda en cada vuelta):
   para k en {2,3,4,5} con capacidades {9,12,11,10}: si len(col) >= k:
        v = col[0..k) ; si v ya está en la lista(k) → cuenta[v]++ ; si no → añade en el primer hueco con cuenta 0 (orden de PRIMERA aparición)
   col = col[1...]
informe (líneas con AppendLine; fin de línea del SO):
   "* Pares que aparecen"   → "<par>-<n>" por entrada en orden de 1.ª aparición ; al encontrar hueco vacío en la posición j: "Pares distintos = j" y parar ;
                              si se llenan todas las posiciones (j == capacidad−1 con valor) → además "Pares distintos = <capacidad>"
   "* Trios que aparecen"   (sin tilde) , "* Cuartetos que aparecen" , "* Quintetos que aparecen" → igual con "Trios/Cuartetos/Quintetos distintos = n"
   "* Contactos" → líneas "1X-n","12-n","X2-n","11-n","XX-n","22-n","1V-n","XV-n","2V-n","VV-n"
Contactos (AnalizaColumna sobre la columna original, tras quitar '*'): para i=0..len−2, par = c[i..i+2):
   1X|X1|1x|x1 → n1X++, n1V++ ;  12|21 → n12++, n1V++ ;  X2|2X|x2|2x → nX2++, nVV++, nXV++, n2V++ ;
   11 → n11++ ;  XX|xx → nXX++, nVV++, nXV++ ;  22 → n22++, nVV++, n2V++      // «Xx»/«xX» no cuentan; 'V' = variante (X o 2)
Escritura: File.WriteAllText(ruta, informe) (UTF-8 sin BOM) → MostrarInfo("Ya") ; excepción → MostrarError("No se ha podido guardar el informe: " + msg)
```
- Con ≤14 caracteres las capacidades 9/12/11/10 nunca se desbordan (14−k+1 ventanas); en Swift usar arrays dinámicos con el mismo orden de primera aparición.
- Las comparaciones de ventanas son exactas (sensibles a mayúsculas): `1x2` ≠ `1X2`.

**Ficheros.** Escribe un `.txt` de informe. Estado global: ninguno.

**Salidas.** Informe de texto (mostrado y guardado), formato `"<formato>-<veces>"` y enteros sin relleno.

**Área Mac propuesta.** Condiciones y filtros → *Formatos* → acción «Sacar formato» como **hoja modal**; el informe puede mostrarse en la hoja con botón «Guardar…» (en vez de exigir primero un fichero). Casos de uso: CU-A34 `ExtraerFormatosDeColumna` (→ `InformeFormatos{pares,trios,cuartetos,quintetos,contactos}`) y CU-A35 `FormatearInformeFormatos`. Evidencia: `P:CalculoFormatosFrmViewModel.cs:59-277`.

---

### Pantalla 16 · CambioPuntosFrm — «Cambiar Puntuación»

**Propósito.** Edita los puntos que una columna probable (CP) otorga por partido fijo, doble y triple al evaluar columnas. **Apertura.** Botón «Cambiar puntuación» de `ColProbablesFrm` (`P:ColProbablesFrmViewModel.cs:706-710`); en el registro (`W:Navigation/PortedPages.cs:112`), sin menú.

**Entradas** (`P:CambioPuntosFrmViewModel.cs:17-29`, `P:CambioPuntosFrmPage.xaml:42-111`): tres `NumberBox` «Valor Fijos / Valor Dobles / Valor Triples», `Mínimo=0`, sin máximo, paso por defecto; botones «Aceptar» y «Cancelar» (ambos vuelven atrás con `Frame.GoBack()`, `P:CambioPuntosFrmPage.xaml.cs:19-32`).

**Lógica de negocio en el VM**
```
Cargar (al abrir): (fijos,dobles,triples) = AConfiguracion.ObtenPuntosCP()  // atributos fijos/dobles/triples del primer hijo de <CP> en W:Datos/parametros.free1x2 (D:EntradaSalida/AConfiguracion.cs:58-67)
                   cualquier excepción → los tres a 0   // (VariablesGlobales usa 10/15/20 si falla la config, D:VariablesGlobales.cs:153-155; el fichero entregado trae fijos=6 dobles=3 triples=1, W:Datos/parametros.free1x2:5)
Aceptar:           AConfiguracion.GuardarPuntosCP((int)fijos,(int)dobles,(int)triples) → reescribe el XML con sangría (D:EntradaSalida/AConfiguracion.cs:209-221)
                   éxito → MostrarInfo("Puntuación guardada.") ; error → MostrarError("No se pudo guardar la puntuación: " + msg) ; después GoBack siempre
```
- `(int)` trunca los decimales que admita el NumberBox. Valor vacío/NaN no se controla (`(int)NaN` indefinido en C#; en Swift validar y rechazar).
- Efecto en el motor: los puntos se leen del fichero al crear el filtro CP de un grupo (`D:MotorCalculo/Grupo.cs:149-156` → `FiltroColProbables.InicializaPuntosCP`, `D:MotorCalculo/FiltroColProbables.cs:150`), y `VariablesGlobales.PuntosFijos/Dobles/Triples` se cargan al arrancar (`D:VariablesGlobales.cs:90`). No se refresca `VariablesGlobales` tras guardar (no verificado si algún consumidor lo relee → «Abiertas» A6).

**Ficheros.** Lee y escribe `W:Datos/parametros.free1x2` (XML; ver ESPEC_07). Estado global: ninguno directo.

**Salidas.** Mensaje «Puntuación guardada.».

**Área Mac propuesta.** Columnas probables → «Puntuación» (también accesible desde Ajustes); **hoja modal** con 3 `Stepper`/`TextField` enteros ≥ 0. Casos de uso: CU-A36 `LeerPuntosCP`, CU-A37 `GuardarPuntosCP` (repositorio de configuración). Evidencia: `P:CambioPuntosFrmViewModel.cs:35-70`.

---

### Pantalla 17 · Coincidencias — «Filtro de coincidencias» (radiografía de 108 características)

**Propósito.** Filtra un fichero de columnas comparando la «radiografía» de 108 características de cada columna con la de una columna ganadora anterior: una columna es válida si, para cada condición (grupo de características + rango de coincidencias admitidas), el nº de características que coinciden está en el rango. Además calcula distribuciones y permite grabar las columnas válidas. **Apertura.** Menú *Filtros → Filtro Coincidencias…* y botón `bFiltroCoincidencias` (`W:MainWindow.xaml.cs:239,410`).

**Entradas** (`P:CoincidenciasViewModel.cs:127-223`, `P:CoincidenciasPage.xaml`)
| Control | Detalle |
|---|---|
| Fichero de entrada | abrir `.txt`/`*`; muestra nombre (defecto «(ningún fichero)») y «Columnas leídas» (`"0"`) |
| Ganadora anterior / reciente | «Fichero…» (abrir `.txt`/`*`), botones `−`/`+` para navegar (índice 1-based), columna actual (defecto `··············`); **máximo 3000 columnas** por fichero (`:732-733`) |
| Condiciones | filas editables «Grupos» (ej. `1-9,33`) y «Rango» (ej. `0-3`), «Rsl.» (solo lectura); defecto una fila `1-9` / `0-9`; «Añadir», «Quitar» (la última), «Leer…» / «Guardar…» (`.cnd`) |
| «Calcular», «Cancelar», «Analizar», «Calcular radiografía» | |
| Contadores | Procesadas, Válidas, Tiempo (defecto `00:00:00.0`, `FormatearTiempo` = `TimeSpan.ToString()+"0000000000"` a 10 car.) |
| Radiografía | 108 filas «C. / Descripción / Valor» |
| Distribución de aceptadas | combo «Índice AMC» (**solo "1"**, `:147`) y 109 filas (C=0..108, casilla S=true por defecto, s/AC, s/GR, AMC) |
| «Grabar resultado», «Grabar distribuciones en Excel», «Imprimir radiografía» | |

**Codificación**: `S2N`/`N2S` = índice base-3 con primer partido más significativo, `X=0`, `1=1`, `2=2` (`:1050-1075`; la misma que `AnaCombi`, ≠ `ConvertidorDeBases`). `_entrada`/`_validas` son bitsets de 3^14.

**Lógica de negocio en el VM (pseudocódigo)**
```
LeerFicheroEntrada: _entrada.limpia ; para cada línea: c = Trim+ToUpper ; len<14 → excepción «Error longitud=<c>» ; ctproc++ ; _entrada[S2N(c)] = true
     // dedupica y ORDENA por índice; ColumnasLeidas = ctproc (líneas, con duplicados) ; xfil=1 ; error → MostrarError("No se pudo leer el fichero de entrada: …")
Ganadoras (A y R): lim=0 ; por línea: t=Trim+ToUpper ; len<14 → MostrarError("col.G. errónea=" + t) y sale (lim queda en lo leído, sin fijar nrf) ; col[lim++]=t[0..14)
     nrf = lim (se muestra la ÚLTIMA) ; índice = nrf ; xgan=1 ; (>3000 líneas → IndexOutOfRange → MostrarError("No se pudo leer el fichero de ganadoras …: …"))
     Siguiente/Anterior: nrf ∈ [1, lim]
Condiciones .cnd: línea = "grupos;rango" (Split(';'): [0], [1] opcional) ; guardar: grupos + ";" + rango por fila (StreamWriter por defecto, UTF-8)
InitConds(): limGrN = nº de filas (0 → false) ; para fila nl:
     grupos.Split(','): "n" → grN[nl][n−1]=true ; "a-b" → grN[nl][a−1 … b−1]=true ; >2 partes → false
     rango.Split(','): "n" → rks[nl][n]=true ; "a-b" → rks[nl][a … b]=true ; >2 partes → false
     cualquier excepción (texto no numérico, n=0, n>109 en grupos, índice>108 en rango, >100 filas) → false → MostrarError("Error en condiciones")
Calcondis(col14): rsls[0..107] = 0 ; CalVX2 ; CalSS ; CalDist ; CalContac ; CalPN ; CalInterr         // radiografía
   Segmentos: «14» = columna entera ; «7P» = partidos 1-7 (Substring(0,7)) ; «7U» = partidos 8-14 (Substring(7))
   [0..8]   Variantes V (≠'1'), X, D('2'): totales 14 → [0,1,2] ; 7P → [3,4,5] ; 7U → [6,7,8]
   [9..20]  Signos seguidos (racha máxima) V,U('1'),X,D por segmento: 14→9..12 ; 7P→13..16 ; 7U→17..20
   [21..32] Distancia (máx. diferencia de posición entre dos apariciones consecutivas del mismo signo; V = X y 2 fundidos) V,U,X,D: 14→21..24 ; 7P→25..28 ; 7U→29..32 (0 si <2 apariciones)
   [33..62] Contactos por segmento (14: 33..42, 7P: 43..52, 7U: 53..62), orden 1X,12,X2,11,XX,22,1V,XV,2V,VV  (reglas de contacto como en CalculoFormatosFrm, solo mayúsculas)
   [63..77] Pesos numéricos por segmento (14: 63..67, 7P: 68..72, 7U: 73..77), orden G(lobal),V,U,X,D:
        sumas: nSuma1 += idxUnos[i] ('1' en posición i) con idxUnos=[7,5,3,1,8,6,4,2,9,7,5,3,1,8] ; nSumaX += idxEquis[i] con [5,1,6,2,7,3,8,4,9,5,1,6,2,7] ; nSuma2 += idxDoses[i] con [1,2,3,4,5,6,7,8,9,1,2,3,4,5]
        (i = posición dentro del segmento: para 7U se reinicia en 0)
        peso(v): mientras v>9: v = v/10 + v%10
        U=peso(nSuma1) ; X=peso(nSumaX) ; D=peso(nSuma2) ; V=peso(X+D) ; G=peso(V+U)
   [78..92] Nº interrupciones por segmento (14: 78..82, 7P: 83..87, 7U: 88..92), orden G,V,U,X,D
   [93..107] Interrupciones SEGUIDAS (racha máxima) con la misma agrupación (14: 93..97, 7P: 98..102, 7U: 103..107)
   CalInterr(segmento tmp): ant=tmp[0]; para i=1..: act=tmp[i]
        act==ant → cierra todas las rachas: maxSeg_k = max(maxSeg_k, n_k) y n_k=0 (k = g,u,d,x,v)
        act!=ant → noIntG++ ; ngs++ ; según (ant→act):
            1→2: noInt1++ ; nus++ ; cierra racha X (maxX = max(maxX,nxs) ; nxs=0)
            1→X: noInt1++ ; nus++ ; cierra racha 2 (maxD, nds=0)
            2→1: noInt2++ ; noIntV++ ; nds++ ; nvs++ ; cierra racha X
            2→X: noInt2++ ; nds++ ; cierra racha 1 y racha V (nus=nvs=0)
            X→1: noIntX++ ; noIntV++ ; nxs++ ; nvs++ ; cierra racha 2
            X→2: noIntX++ ; nxs++ ; cierra racha 1 y racha V
        ant=act ; al terminar: cierra todas con max(.)
   Descripciones (108): V14,X14,D14,V7P,X7P,D7P,V7U,X7U,D7U,SSV14…SSD7U,disV14…disD7U,con(1x)14…con(vv)7U,PNG14…PND7U,IG14…ID7U,ISG14…ISD7U (lista completa en P:CoincidenciasViewModel.cs:1098-1111)
CalcularRadiografia: si nrfCGA<1 → MostrarError("Faltan ganadoras anteriores.") ; condis = Calcondis(ganadoraA[nrfCGA]) con 'x'→'X' ; rellena la tabla «Radiografía»
Calcular:
   limGrN = nº filas ; xcon = limGrN>0 ; si xfil*xgan1*xcon == 0 → MostrarError("Faltan entradas (fichero, ganadora anterior y condiciones).") ; si nrfCGA<1 → MostrarError("Faltan ganadoras anteriores.")
   InitConds() falso → MostrarError("Error en condiciones") ; condis = radiografía(ganadoraA actual) ; _validas=∅ ; agrabarAC[109], agrabarGR[109], agrabarAMC[100][109], agrabarCOL[3^14] = 0 ; ctadm=0
   para idx = 0..3^14−1 ascendente con _entrada[idx] (hasta _salida): col=N2S(idx) ; procesadas++ ; Valida(col, idx) ; cada 4096 procesadas publica progreso
   Valida(col, idx): rsls=Calcondis(col) ; coin[k] = (condis[k] == rsls[k]) k<108
        para cada condición ng: nc = |{k : grN[ng][k] ∧ coin[k]}| ; si !rks[ng][nc] → descarta y TERMINA
        // al aceptar: nc conserva el valor de la ÚLTIMA condición
        validas[idx]=true ; ctadm++ ; agrabarCOL[idx]=nc ; agrabarAC[nc]++ ; para k<108 con coin[k]: agrabarGR[k+1]++ ;
        para cada ng: agrabarAMC[ng][|{k: grN[ng][k] ∧ coin[k]}|]++
   Fin: «Distribución» fila n: s/AC = agrabarAC[n] ; s/GR = agrabarGR[n] ; AMC = agrabarAMC[índiceAMC−1][n] ; Procesadas = ctproc ; Válidas = ctadm ; Tiempo
Analizar: requiere ganadoras A y R (MostrarError "Faltan ganadoras") y condiciones válidas ; condis = radiografía(A actual) ; rsls = radiografía(R actual) ; coin = igualdad por característica ;
        «Rsl.» de cada condición = |{k : grN[nr][k] ∧ coin[k]}|
GrabarResultado: guardar `.txt` (sugerido «Resultados») ; para nr ascendente con validas[nr] y Distribucion[agrabarCOL[nr]].Seleccionada: escribe N2S(nr) (14 signos) ; ColumnasGrabadas = n
ImprimirRadiografia: guardar `.txt` (sugerido «radiografia»): "Radiografia de la columna = " + índiceGanadoraA ; luego 108 líneas "<descripción> = <valor>" ; MostrarInfo("Radiografía guardada en <nombre>")
GrabarExcel: guardar «.xls» (sugerido «Resultados»), TEXTO tabulado (no Excel real): cabecera "N\ts/AC\tN\ts/GR" + ("\tN\tAMC{k}" por condición) ; 109 filas "n\tAC[n]\tn\tGR[n]" + ("\tn\tAMC[ng][n]" por condición)
```
- Divergencias/peculiaridades a preservar o decidir: (1) la ganadora que usa `Calcular` es la anterior **actual** (la que muestra el visor), no necesariamente la última; (2) `Calcular` ignora la ganadora reciente; (3) `agrabarCOL` guarda `nc` de la última condición; (4) límite 100 condiciones y 3000 ganadoras; (5) el combo AMC solo ofrece «1» (el original tenía botones +/− `bAMC1/bAMC2`, no verificado en el port) → «Abiertas» A7.
- Rendimiento: `Calcondis` por columna con cadenas; en Swift trabajar con `[UInt8]` y tablas por posición.

**Ficheros.** Lee `.txt` (entrada, ganadoras), `.cnd` (condiciones). Escribe `.cnd`, `.txt`, `.xls` (texto). Estado global: ninguno.

**Salidas.** Tres tablas (condiciones con resultado, radiografía 108×3, distribución 109×5) y contadores enteros.

**Área Mac propuesta.** Condiciones y filtros → *Filtros de fichero → Coincidencias* (vista principal de tres paneles); exportación «distribuciones» como CSV/TSV. Casos de uso: CU-A38 `CalcularRadiografiaColumna` (108 valores), CU-A39 `ParsearCondicionCoincidencias`, CU-A40 `FiltrarPorCoincidencias`, CU-A41 `AnalizarCoincidenciasEntreGanadoras`, CU-A42 `ExportarDistribuciones`. Evidencia: `P:CoincidenciasViewModel.cs:247-1111`.

---

### Pantalla 18 · ColGanadoraFrm — «Análisis de fallos en combinación»

**Propósito.** Pide la columna ganadora y la combinación a analizar (la de pantalla o una `.comb`/`.xml`) y abre el árbol de fallos (`AnalizarCombinacionFrm`). **Apertura.** Botón `bAnalisisFallos` («Análisis de fallos») de la barra Combinación (`W:MainWindow.xaml.cs:398`), sin parámetros de navegación; en el WinForms original lo abría el menú de la pantalla principal pasándole el contexto.

**Entradas** (`P:ColGanadoraFrmViewModel.cs:41-72`, `P:ColGanadoraFrmPage.xaml:23-63`)
| Control | Detalle |
|---|---|
| Columna ganadora | `TextBox` `MaxLength=16`, `CharacterCasing=Upper`, actualiza al teclear; texto auxiliar «La columna debe tener exactamente N signos.» (N = `_numPartidos`, defecto 14) |
| Origen (radio) | «Analizar combinación en pantalla» (defecto) / «Abrir combinación para analizar» |
| Analizar | `ViewModel.AnalizarCommand` |

**Contexto de partida.** `ColGanadoraContexto(NumPartidos, NombreComb, Analizador, ListaPronosticos)` llega por `e.Parameter` (`P:ColGanadoraFrmPage.xaml.cs:32-40`). **Ningún código del proyecto WinUI lo construye** (búsqueda de `ColGanadoraContexto` solo en su propia definición y receptor): abierta desde la barra, la rama «en pantalla» siempre falla con el mensaje de abajo.

**Lógica de negocio en el VM**
```
Analizar:
   si len(ColumnaGanadora) != _numPartidos → MostrarError("La columna ganadora debe tener " + VariablesGlobales.NumeroPartidos + " signos.") ; fin
   si «en pantalla»:
        si no hay contexto → MostrarError("No hay una combinación en pantalla disponible para analizar. Selecciona \"Abrir combinación para analizar\".") ; fin
        archivo=NombreComb ; analizador=contexto.Analizador ; pronosticos=contexto.ListaPronosticos
   si «abrir»: abrir `.comb`/`.xml` (cancelar → fin) ; archivo=ruta
        arch=ArchivoCombinacion(); arch.AbrirArchivoCombinacion(ruta) ; analizador=new Analizador() ; arch.CargaControladorGrupos(analizador.CtrlGrupos)
        pronosticos = arch.LeePronosticos() con Replace(",","") por elemento     // NO se cargan filtro de columnas ni If-Then
   columna = UtilColumnas.ConvStrToLong(ColumnaGanadora)     // 3 bits/partido: '2'→1,'X'→2,'1'→4 ; primer carácter = partido 1 = bits bajos ; carácter no válido ⇒ 1 << -1 = 1<<31 (basura): solo se evita por CharacterCasing/longitud
   AnalizarCombinacionFrmViewModel.UltimoAnalisis = (archivo, columna, analizador, pronosticos) ; navegar a AnalizarCombinacionFrmPage      (ver pantalla 7)
```
- **No hay validación de caracteres** (solo longitud y mayúsculas). Una columna con `0`,`3`… produce una máscara inválida. En Swift validar `{1,X,2}` y rechazar con el mismo mensaje.
- La longitud exigida es el nº de partidos del contexto, pero el mensaje imprime `VariablesGlobales.NumeroPartidos` (pueden diferir con 15 partidos).

**Ficheros.** Abre `.comb`/`.xml` (sin carpeta inicial propia; `PickerHelper.AbrirAsync(".comb", ".xml")`, `:139`; el original arrancaba en `Combinaciones\`). Estado global: ninguno directo (handoff estático hacia pantalla 7).

**Salidas.** Navegación al árbol de fallos.

**Área Mac propuesta.** Análisis → *Análisis de fallos*: **hoja modal** de entrada (campo de columna con validación en vivo + selector «pantalla/archivo») que abre la vista principal del árbol (pantalla 7). Casos de uso: CU-A43 `PrepararAnalisisDeFallos(columna, origen)`; reutiliza la conversión del motor `UtilColumnas.ConvStrToLong` (ESPEC_06; no es caso de uso). La rama «en pantalla» no es alcanzable desde la barra (ver «Abiertas» A22). Evidencia: `P:ColGanadoraFrmViewModel.cs:102-166`, legacy `L:UI/ColGanadoraFrm.cs:158-208`.

---

### Pantalla 19 · ColProbablesFrm — «Columnas Probables» (editor del filtro CP)

**Propósito.** Editor del filtro «Columnas Probables» del grupo en pantalla: lista de columnas probables (CP) con su pronóstico, rangos de aciertos / aciertos seguidos / fallos seguidos, puntos y tolerancias locales, más tres tipos de relaciones entre CP (I, II, III) y el control de fallos. **Apertura.** Botón «Columnas Probables» de la rejilla de condiciones de Inicio (`V:MainPageViewModel.cs:169,291-296`: `AppState.GrupoEnEdicion = grupo en pantalla` y navega). No está en menús. Vuelve con `Frame.GoBack` (`P:ColProbablesFrmPage.xaml.cs:23`).

**Estructura de la pantalla** (`P:ColProbablesFrmPage.xaml`): 5 pestañas — *Columnas*, *Relaciones I*, *Relaciones II*, *Relaciones III*, *Control Fallos* — y un único botón «Aceptar» (`:592`); **no hay «Cancelar»** (salir con atrás descarta las ediciones).

#### 19.1 Pestaña Columnas
| Control | Detalle |
|---|---|
| Pronóstico | `TextBox` `MaxLength=14`, un carácter por partido (placeholder «Ej. 1X21X2111XX221») |
| Puntos | texto libre |
| Rangos (Aciertos, Ac. Seguidos, Fallos Seguidos) | texto, valores 0..14 (placeholder «Ej. 0-14 ó 3,5,7») |
| Tolerancias locales (Aciertos, Ac. Seguidos, Fallos Seguidos) | texto + casillas «Núm Tolerancias» 0,1,2,3 |
| Navegación | `|<  <<  <  n/total  >  >>  >|` (`<<`/`>>` saltan `VariablesGlobales.Desplazamiento`, 3 en el `W:Datos/parametros.free1x2` entregado, `W:Datos/parametros.free1x2:19-21`) |
| Acciones | «Todos a Triple», «Borrar Pronósticos», «Eliminar actual», «Copiar datos», «Copiar Columnas», «Cambiar Puntuación», «Importador CPs», «Exportador CPs» |

**Lógica en el VM (`P:ColProbablesFrmViewModel.cs`)**
```
Cargar (OnNavigatedTo, salvo retorno de Copiar/Importar): grupo=AppState.GrupoEnEdicion (nil → todo vacío) ; filtroCP = grupo.GetFiltro("ColProbables")
   grupoCP = clon de cada ColumnaProbable del filtro (pronósticos, AC, ACS, FS, puntos y —solo si ToleranciaLocalActiva— ACTol/ACSTol/FSTol/tolerancias)   // ObtenCopiaCP :295-319
   cpPantalla=0 ; ActualizaDatosPantalla(0) ; carga Relaciones I/II/III y Control Fallos
ActualizaDatosPantalla(n): cp = grupoCP[n] (o vacía si no hay) ; paginación "(n+1)/Count" (vacío: "1/1")
   Pronostico = concatenación de cp.Pronosticos[i] (¡un partido doble/triple aporta 2–3 caracteres!) ; textos = cp.GetAciertos()/GetAciertosSeguidos()/GetFallosSeguidos()/GetPuntos()/GetACTol()/GetACSTol()/GetFSTol() ;
   casillas Tolerancia0..3 desde "0,1,.." (cp.GetTolerancias())
   GetAciertos() devuelve la lista "0,3,5…" de índices activos (no rangos)
GuardaDatosCP(cp):  cp.ReinicializaValores() ; cp.Pronosticos[i] = (i<len(Pronostico)) ? Pronostico[i] (UN carácter) : ""     // ← PIERDE dobles/triples al guardar (A10)
   todos = "0,1,…,NumeroPartidos"  (UtilidadesEntradasValores.ObtenerTodosValores)
   si Pronostico != "": cp.SetPuntos(Puntos) ; SetNoAciertos(Aciertos != "" ? Aciertos : todos) ; SetNoAciertosSeguidos(...) ; SetNoFallosSeguidos(...) ;
        si algún campo Tol != "": SetACTol/SetACSTol/SetFSTol(valor o todos) ; SetTolerancias(casillas.join(",") o "0")
   sino si cpPantalla < Count: grupoCP.RemoveAt(cpPantalla)
   // SetNoAciertos(csv): cada token = entero o entero+LETRA (letra = tolerancia asignada, D:MotorCalculo/ColumnaProbable.cs:376-400); un rango «0-14» lanza FormatException (`Convert.ToInt32`) y un token vacío (p. ej. coma final) lanza IndexOutOfRangeException (`str[str.Length-1]`); el campo entero vacío se sustituye por «todos» (el WinForms usaba controles OptionNum, no texto) → A11
GuardarCPActual(): si cpPantalla<Count → GuardaDatosCP(grupoCP[cpPantalla]) ; sino si hay pronóstico → añade nueva CP y guarda
CambiaCPSeleccionado(n): n=clamp(n,0,Count) ; GuardarCPActual() ; cpPantalla=n ; si Count < n+1 → añade CP vacía ; ActualizaDatosPantalla(n)
   // PRESERVAR: «>» en la última crea una CP nueva vacía; si GuardarCPActual elimina la actual (pronóstico vacío) los índices se desplazan y se crea otra CP vacía al final
Primera=Cambia(0) ; Atrás3=Cambia(cp−Desplaz) ; Anterior=Cambia(cp−1) ; Siguiente=Cambia(cp+1) ; Adelante3=Cambia(cp+Desplaz) ; Última=Cambia(Count>0 ? Count−1 : 0)
EliminarActual: grupoCP.RemoveAt(cpPantalla) ; cpPantalla=min(cpPantalla, Count−1) ; refresca
TodosATriple: (crea CP si no hay) cp.TodosATriple() → pronósticos "1X2" en NumeroPartidos+1 posiciones ; BorrarPronosticos: cp.BorraPronosticos()
CopiarDatos (si Count==0 no hace nada): captura AC/ACS/FS de pantalla (vacío→todos) ; CopiarDatosCPFrmViewModel.Resultado=nil ; navega a CopiarDatosCPFrm con (Desde=cpPantalla+1, Hasta=Count) ; al volver AplicarCopiaDatos():
        min=Desde−1 (<0 → cancelado) ; max=Hasta ; para i∈[min,max) ∩ [0,Count): cp.ReinicializaValores() ; SetNoAciertos/SetNoAciertosSeguidos/SetNoFallosSeguidos(valores capturados)
        si cpPantalla fuera de [min,max] y existe: lo mismo para la actual ; refresca     // los pronósticos y puntos de las CP de destino se pierden (ReinicializaValores) → ver pantalla 30
CopiarColumnas → navega a CopiarCPFrm (pantalla 29; origen AppState.GrupoEnEdicion) ; CambiarPuntuacion → CambioPuntosFrm (pantalla 16)
ImportarCPs: si Count>0 y la última CP no tiene pronósticos → se borra ; _importPendiente ; navega a ImportadorCPsFrm ; al volver (página):
        sin resultado → fin ; si importadas.Count==grupoCP.Count>0 → ContentDialog «¿Sustituir las columnas existentes por las importadas (manteniendo los rangos de aciertos y tolerancias)?» → Sí: SustituirPronosticos (conserva rangos)
        si no, si grupoCP.Count>0 → «¿Añadir las columnas importadas al final de las existentes? (Si se selecciona No, se sustituirán TODAS las columnas y sus rangos por las columnas importadas)» → Sí: Añadir ; No: Reemplazar todo
        sin existentes → Reemplazar ; (Sí es el botón por defecto)
ExportarCPs: si hay AC/ACS/FS en pantalla → GuardarCPActual() ; ExportadorCPsFrmViewModel.ListaParaExportar = grupoCP ; navega a ExportadorCPsFrm
Aceptar: (filtro nil → solo volver) GuardarCPActual() ; si la última CP no tiene pronósticos → se borra ;
        si !filtro.ContieneDatos && Count>0: filtro.ContieneDatos=true ; filtro.IsActive=true ;   (la 1.ª vez con datos activa la condición)
        filtro.ColProbables = grupoCP ;
        GuardarDatosRelacionesCP1/2/3 + GuardarDatosControlFallos ; AppState.NotificarCambio() ; volver
```

#### 19.2 Relaciones I (`RelacionCP1`, `ControladorRelacionesCP1`)
Campos: Columnas, Suma Aciertos, Recorrido, «¿Cuántas Columnas?» (`CantidadCP`), «Número de Aciertos» (`CuantosAC`), todos texto; navegación `<`/`>`, «Eliminar Actual», paginación «k/total» (`:1013-1077`).
```
Cargar: copia de filtro.RelacionesCP1.Relaciones (5 campos) ; mostrar la 0
TieneDatos = Columnas != "" && !(SumaAciertos=="" && Recorrido=="" && CuantasCP=="")
Anterior: Cambia(rel−1) (SIN límite inferior: en la 0 pasa a −1 y el siguiente guardado lanza ArgumentOutOfRange; el WinForms deshabilitaba el botón → en Swift deshabilitar) ; Siguiente: solo si TieneDatos → Cambia(rel+1) (crea RelacionCP1 vacía al final) ; Cambia(n): GuardarRelActual() ; rel=n ; si Count<n+1 añade vacía ; refresca
GuardarRelActual: si rel<Count → copia campos solo si TieneDatos ; sino si TieneDatos → añade
Eliminar: rel==0 → quita la 0 (si hay) ; si no → quita rel y rel−− ; si queda vacío añade una vacía
Aceptar: GuardarRelActual() ; si la última no tiene datos → se quita ; Relaciones = [r | r.Columnas != ""]
```
#### 19.3 Relaciones II (`RelacionCP2`) — «Sumas de AC/ACS/FS» + «AC/ACS/FS individuales»
Por bloque: «Las columnas» A, «Concepto» ∈ {AC, ACS, FS}, «Tendrán (nº aciertos)», «Más / Menos» ∈ {Más, Menos}, «Que las columnas» B. Navegación `<`/`>` con índice **1-based** (`indiceNavRel2`), paginación «k / total», «Eliminar Actual» (`:1079-1256`).
```
Pantalla→RelacionCP2: Aciertos=ObtenerListaFromTxtAciertos(txt) ; ColumnasA/B=ObtenerListaFromTxt(txt) ; Concepto ; Cantidad (Más/Menos) ; + cadenas originales (StrAciertos, StrColsA, StrColsB) ; idem sufijo 2 para el bloque individual ; ColumnasProbables = grupoCP
Comprobar: válida si NO(( A vacío ∨ B vacío ∨ Aciertos vacío ) ∧ ( A2 vacío ∨ B2 vacío ∨ Aciertos2 vacío ))
Guardar: si Count ≥ índice → sustituye la actual ; si no → añade ; Anterior (solo si índice>1): guarda, índice−− ; Siguiente: guarda, índice++ (muestra vacía si no existe)
Eliminar: si Count>0 y Count ≥ índice → quita la actual y la re-muestra ; si Count==0 → reinicializa ; la lista de trabajo ES la del filtro (no copia) → los cambios afectan al filtro aunque no se pulse Aceptar
Aceptar: guarda la pantalla ; filtro.RelacionesCP2.ColumnasProbables = grupoCP ; Relaciones2 = lista
```
#### 19.4 Relaciones III (`RelacionCP3`) — escaleras, sándwiches, agrupaciones
Campos: «Columnas implicadas», «Concepto» ∈ {AC,ACS,FS}, Escaleras Total / Ascendentes / Descendentes, Sándwichs Total, tablas «Agrupaciones Paso Fijo» y «Agrupaciones Solapadas» (filas Número / Elementos / Aciertos, botones «Añadir agrupación» y «Eliminar»). Navegación 1-based y «Eliminar Actual» (`:1258-1576`).
```
Cargar fila: textos desde la RelacionCP3 ; agrupaciones: cada cadena "Número+Elementos+Aciertos" con Split('+', sin vacíos) de EXACTAMENTE 3 partes → una fila
ObtenerColumnasImplicadas: indices=ObtenerListaFromTxt(txt) ; lista = [grupoCP[i] para i<indices.Count y i<Count]    // usa el ÍNDICE POSICIONAL i, no el valor del índice (réplica del WinForms)
GuardarPantalla: longEscaleras = lista.Count/3 (div. entera) ; longSandwichs = lista.Count/4
     NumeroEscalerasASC/DESC/Totales = ObtenerBoolArrayFromTxt(txt, longEscaleras) ; NumeroSandwichs = ObtenerBoolArrayFromTxt(txt, longSandwichs) (+ cadenas originales)
     Agrupaciones*: por cada fila con las 3 celdas no vacías: valores=ObtenerListaFromTxtAciertos(Número) ; elementos=Convert.ToInt32(Elementos) ; ac=ObtenerListaFromTxtAciertos(Aciertos) ;
          si elementos < grupoCP.Count+1 y ac.Count ≤ 15 → AgrupacionColumnas(valores, elementos, ac) ; ninguna → nil
          cadena = "Número+Elementos+Aciertos" por fila con Número != "" (nil si ninguna)
     Comprobar: inválida (no se guarda) si TODOS: AgrupPasoFijo==nil, AgrupSolapadas==nil, ColumnasImplicadasString=="", ASC==nil, DESC==nil, Totales==nil, Sandwichs==nil
     (los arrays Escaleras/Sandwichs no son nil si el campo tiene texto)
Mismo patrón de navegación/guardado/eliminación que Relaciones II ; lista de trabajo = la del filtro
```
#### 19.5 Control Fallos (`ControladorCPControlFallos`, `CPControlFallos`)
Campo «Número de Fallos de Controles permitidos» (placeholder «Ej. 0-2 ó 1,2») y tabla de controles (Columnas «1-3 ó 1,2», Tolerancias «AC, ACS, FS», Aciertos «0-2 ó 1»), «Añadir control», «Eliminar» por fila (`:1578-1647`).
```
Cargar: NumeroFallosControles = filtro.ControlFallosCP.FallosPermitidos ; filas = controles (Columnas, Tolerancias, Aciertos)
Aceptar: ControlesFallos = [filas con Columnas no vacías] ; FallosPermitidos = texto
```
**Pistas Swift.** Todo el flujo es un *editor con buffer de trabajo* (`EditorColumnasProbables`) + commit al filtro; las claves de formato textual (`a-b`, `a,b`, `Más/Menos`, letras de tolerancia) pertenecen al motor/ficheros (ESPEC_04 y ESPEC_07). El pronóstico por partido debe poder ser 1, 2 o 3 signos (la `TextBox` del port no lo permite, A10).

**Ficheros.** Ninguno directo (copia de datos y de columnas son las pantallas 30 y 29; importador/exportador en ESPEC_09/10). Estado global: `AppState.GrupoEnEdicion` (entrada), `AppState.NotificarCambio()` (salida), `VariablesGlobales.Desplazamiento/NumeroPartidos`.

**Salidas.** Valores del filtro actualizados; paginaciones «n/total» y «k / total».

**Área Mac propuesta.** Columnas probables (área propia): **vista principal** con lista de CP a la izquierda (`List` seleccionable) y detalle/inspector con pestañas Rangos · Tolerancias · Relaciones · Control de fallos; el importador/exportador/copia como hojas modales. Casos de uso: CU-A44 `EditarColumnasProbables` (buffer + `confirmar()`), CU-A45 `NavegarColumnaProbable`, CU-A46 `CopiarRangosAColumnas`, CU-A47 `FusionarColumnasImportadas(modo: sustituir|añadir|reemplazar)`, CU-A48 `EditarRelacionesCP1`, CU-A49 `EditarRelacionesCP2`, CU-A50 `EditarRelacionesCP3`, CU-A51 `EditarControlFallosCP`. Evidencia: `P:ColProbablesFrmViewModel.cs:256-1648`.

---

### Pantalla 20 · ColumnasPremiadasFrm — «Columnas Premiadas»

**Propósito.** Muestra el resumen de las columnas premiadas encontradas por un escrutinio (fichero, jornada, columna, premio, nº de columna, nº de boleto) y las exporta a un `.txt`. **Apertura.** Botón «Ver premiadas» de `EscrutiniosFrm` (`P:EscrutiniosFrmViewModel.cs:971`) y de `EscrutarCombinacionesFrm` (`P:EscrutarCombinacionesFrmViewModel.cs:771`), que dejan las filas en el estático `ColumnasPremiadasFrmPage.Entrada` (`P:ColumnasPremiadasFrmPage.xaml.cs:23,32-45`). En el registro (`W:Navigation/PortedPages.cs:50`), sin menú.

**Entradas.** Solo la lista de filas recibida (`ColumnaPremiadaItem`: ArchivoColumnas, Jornada, Columna, Premio, NumeroColumna, NumeroBoleto; todo `string`, `P:ColumnasPremiadasFrmViewModel.cs:21-40`). Selección múltiple en la lista (`ListaResumen.SelectedItems`). Botones «Guardar Todas» y «Guardar Seleccionadas» (`P:ColumnasPremiadasFrmPage.xaml:115-121`).

**Lógica de negocio en el VM**
```
Guardar(filas):  filas vacías/nil → MostrarInfo("No hay columnas que guardar.") ; fin
   guardar `.txt`, nombre sugerido «columnas» (`PickerHelper.GuardarAsync("columnas", ("Columnas", ".txt"))`)  (cancelar → fin)
   Task.Run: StreamWriter por defecto → una línea por fila con SOLO el campo Columna (texto tal cual, sin cabecera ni más campos), en el orden de la lista (o de selección)
   éxito → MostrarInfo("Guardadas N columna(s) en <fichero>.") ; excepción → MostrarError("No se pudieron guardar las columnas: …")
Seleccionadas: se recogen los ítems de SelectedItems (orden de la colección de selección del control, no necesariamente el visual)
```
- La ordenación y el contenido de las filas las decide el productor (`Escrutinios`/`EscrutarCombinaciones`, ESPEC_09/10), no esta pantalla.

**Ficheros.** Escribe `.txt` (una columna por línea). Estado global: ninguno.

**Salidas.** Tabla de 6 columnas de texto; mensaje con el nº de columnas guardadas.

**Área Mac propuesta.** Escrutinio → *Columnas premiadas*: **hoja modal/panel de detalle** del resultado de un escrutinio con `Table` multiselección y botones «Exportar todas / seleccionadas» (`fileExporter`). Casos de uso: CU-A52 `ExportarColumnasPremiadas(filas)`. Evidencia: `P:ColumnasPremiadasFrmViewModel.cs:61-120`.

---

### Pantalla 21 · CombinarFiltros — «Combinación de Filtros»

**Propósito.** Cuenta, para cada columna posible, en cuántos de varios ficheros de columnas («filtros») aparece, y permite sumar o grabar las columnas cuyo recuento está en un rango [mín, máx]. **Apertura.** Menú *Filtros → Combinar filtros…* y botón `bCombinarFiltros` (`W:MainWindow.xaml.cs:236,408`).

**Entradas** (`P:CombinarFiltrosViewModel.cs:25-98`, `P:CombinarFiltrosPage.xaml`)
| Control | Detalle |
|---|---|
| «Cargar Filtro» | abrir `.txt` **múltiple** (`PickerHelper.AbrirVariosAsync(".txt")`, `:147`) |
| Lista «N / Filtro / Activo / Cols.» | N (orden), nombre, casilla Activo (defecto sí), columnas contadas (0) |
| «Activa / Desactiva» | `ToggleSwitch` defecto on; al conmutar aplica su valor a todas las casillas (`MarcarDesmarcar`) |
| «Salvar Lista» / «Cargar Lista» | `.lst` (una ruta completa por línea) |
| «Iniciar Proceso» / «Cancelar» / «Reiniciar Todo» | |
| «Filtros acertados» Mínimo / Máximo | `NumberBox` `Minimum=1`, defectos 1 y 1 |
| «Sumar cols.» → «Total columnas» (`0`), «Grabar cols.» → «Archivo de salida» | |
Texto fijo: «Cualquier variación en el cuadro "Filtros" requiere repetir el proceso antes de sumar o grabar columnas.»

**Codificación** `S2n`/`N2s` propias (`:110-133`): primer partido más significativo, `X=0`, `1=1`, `2=2` (no es `ConvertidorDeBases`); `N2s` produce siempre **14** signos.

**Lógica de negocio en el VM (pseudocódigo)**
```
estado: htCols:int[3^partidosEnJuego] (contador por columna) ; partidosEnJuego (0 = sin definir) ; salida
CargarFiltro: para cada fichero elegido: numP = ObtenNumSignos() (longitud de la 1.ª línea) ; (error al abrir → se ignora y sigue)
      si partidosEnJuego==0 → = numP ; sino si ≠ → MostrarError("Los filtros deben tener el mismo número de partidos") y SALE (los ya añadidos quedan; htCols no se redimensiona)
      añade fila {N = Count+1, Nombre = nombre de fichero, Ruta, Activo=true, Columnas=0}
   al terminar el lote: htCols = new int[3^partidosEnJuego]      // se pierde cualquier proceso previo
CargarLista(.lst): Filtros.Clear ; partidosEnJuego=0 ; para cada línea no vacía: ruta ; numP ; primera línea fija el nº ; distinto → MostrarError("Los archivos no tienen el mismo número de partidos") y SALE ; añade fila ; luego htCols = new int[3^p] ; error → MostrarError("No se pudo cargar la lista: …")
SalvarLista: guarda `.lst` (sugerido «Lista»), una línea por fila con la RUTA COMPLETA (incluye inactivas)
Iniciar (si Filtros.Count==0 no hace nada): htCols[*]=0 ; para cada fila en orden (hasta salida):
      fila.Columnas = 0 ; si !Activo → continúa
      repes = BitArray(3^p) limpio ; ctcols=0
      para cada línea del fichero: c = Normaliza(línea) [ToUpper y solo {1,2,X}] ; ctcols++ ; si len(c) < 14 → DEJA de leer ese fichero ; i = S2n(c) ; si !repes[i]: htCols[i]++ ; repes[i]=true
      fila.Columnas = ctcols            // incluye duplicados y la línea corta que cortó la lectura
   error → MostrarError("No se pudo combinar los filtros:\n\n" + msg) ; Cancelar: salida=true, Procesando=false
Rango efectivo (Sumar y Grabar): min = max(1,(int)Mínimo) ; max = min((int)Máximo, Filtros.Count)      // Filtros.Count incluye inactivos
Sumar: Total = |{i < 3^p : min ≤ htCols[i] ≤ max}|
Grabar: guardar `.txt` (sugerido «Columnas») ; para i = 0..3^p−1 ascendente con htCols[i] en [min,max]: GuardarCols(N2s(i)) ; Total = nº grabadas ; ArchivoSalida = nombre   // orden por índice (X<1<2); error → MostrarError("No se pudo grabar: …")
Reiniciar: Filtros.Clear ; Total=0 ; ArchivoSalida="" ; partidosEnJuego=0 ; htCols vacío
```
- Un filtro con columnas más largas que `partidosEnJuego` desborda `htCols` (excepción → mensaje genérico). `N2s` con ≠14 partidos escribe columnas de 14 (no replicar el defecto sin decisión → «Abiertas» A12).

**Ficheros.** Lee `.txt` (filtros) y `.lst`; escribe `.lst` y `.txt` (columnas, UTF-8 sin BOM, `\n` del SO). Estado global: ninguno.

**Salidas.** Tabla de filtros con columnas leídas; «Total columnas» entero; nombre del fichero de salida.

**Área Mac propuesta.** Condiciones y filtros → *Filtros de fichero → Combinar*; vista principal con `Table` reordenable y panel de rango. Casos de uso: CU-A53 `CombinarFiltrosContar`, CU-A54 `SumarColumnasPorRango`, CU-A55 `GrabarColumnasPorRango`, CU-A56 `GuardarListaFiltros`/CU-A57 `CargarListaFiltros`. Evidencia: `P:CombinarFiltrosViewModel.cs:143-415`.

---

### Pantalla 22 · Compresor — «Compresor *.z3q»

**Propósito.** Comprime un fichero de columnas `.txt` (sin repetidas, ordenado por índice) al formato propio `.z3q` y lo descomprime de vuelta. **Apertura.** Menú *Utilidades → Compresor z3q…* (`W:MainWindow.xaml.cs:273`); no hay botón de barra.

**Entradas** (`P:CompresorViewModel.cs:39-58`, `P:CompresorPage.xaml`)
| Control | Detalle |
|---|---|
| «Comprimir» → «Archivo…» | abrir `.txt`/`*`; muestra el nombre (defecto «(ningún archivo)») |
| «Nivel de compresión» | desplegable «0»..«9» (el **índice** = nivel), defecto 5 |
| «Descomprimir» → «Archivo…» | abrir `.z3q`/`*` |
| «Estado» | «Preparado» → «Guardando» / «Descomprimiendo…» → mensajes finales |
| «Cerrar» | `Frame.GoBack()` |

**Formato `.z3q`** (`D:Utils/CompresorZip.cs:20-110`): ZIP (SharpZipLib) con **una única entrada Deflated**; los bytes de la entrada empaquetan el `BitArray`: bit *i* → byte `i/8`, bit `i%8` (LSB primero), longitud de datos `(tamano/8)+1`; CRC32 de esos bytes; nombre de entrada = ruta de salida recibida (`ZipEntry(fichBin)` con `fichBin == fichZip == rutaFinal`, `:48-50`); nivel de deflate 0..9 (`SetLevel`). La descompresión lee **solo la primera entrada**, ignora su nombre y construye un `BitArray` de `14 348 907` bits (3^15), recortado a `4 782 969` (3^14) salvo que se hayan leído más de `14 348 907` bits (`:80-125`). El índice *i* usa la codificación de `ConvertidorDeBases` (partido 0 = dígito menos significativo; `'1'→0, 'X'→1, '2'→2`).

**Lógica de negocio en el VM**
```
Comprimir:
   abrir `.txt` ; primera línea → longitud L = len(LeeColumnaSinComas) ; array = BitArray(3^L)         // ObtenTamañoBitArray(L) = PotDe3[L]
   relee todo: para cada línea c (Trim+ToUpper): si len(c)>15 o len(c)<14 → array=∅ ; MostrarError("Error leyendo columnas") ; nombre=«(ningún archivo)» ; fin
        array[ConvColumnaANumero(c)] = true          // un fichero con líneas más largas que la 1.ª puede desbordar el array → "Error al leer el archivo de columnas: …"
   Estado="Guardando" ; guardar «Columnas Comprimidas» `.z3q`, nombre sugerido = nombre del `.txt` sin extensión (cancelar → "Preparado")
   rutaFinal = carpeta + nombreSinExtensión + ".z3q" (fuerza la extensión) ; tamDatos = CompresorZip.Comprimir(array, rutaFinal, rutaFinal, 3^L, nivel)
   Estado = "Guardado: <nombre> (<tamDatos:N0> bytes de datos, <tamFichero:N0> bytes en disco, nivel <n>)." ; MostrarInfo("Archivo comprimido correctamente:\n<ruta>\n\nDatos: <N0> bytes — Archivo: <N0> bytes.")
   error → Estado="Error al comprimir." ; MostrarError("Error al comprimir el archivo: …")
Descomprimir:
   Estado="Descomprimiendo…" ; array = CompresorZip.Descomprimir(ruta) (error → nombre «(ningún archivo)», Estado="Error al descomprimir.", MostrarError("Error al descomprimir el archivo: …"))
   guardar `.txt` (sugerido «Columnas») ; numPartidos = ObtenNumeroPartidosColBin(array.Count) (4 782 969→14, 14 348 907→15)
   para i=0..Count−1 ascendente con array[i]: GuardarCols(ConvNumAColumna(i))     // columnas de numPartidos signos en orden de índice
   Estado = "Guardado: <nombre> (<n:N0> columnas)." ; MostrarInfo("Archivo descomprimido correctamente:\n<ruta>\n\n<N0> columnas escritas.") ; error → Estado="Error al guardar las columnas." + MostrarError("Error al guardar las columnas descomprimidas: …")
```
- La compresión **deduplica y reordena** (es un conjunto de columnas), por lo que `txt → z3q → txt` no conserva orden ni repetidas.
- En Swift hace falta un compresor/descompresor ZIP-deflate compatible (SharpZipLib 1.4.x, sin cifrado); verificar con ficheros reales (Abiertas A13).
- `:N0` = separador de miles de la cultura actual.

**Ficheros.** Lee `.txt` / `.z3q`; escribe `.z3q` / `.txt`. Estado global: ninguno (campo privado `BitArray` de 4,78 M).

**Salidas.** Mensajes de estado y diálogos informativos con tamaños.

**Área Mac propuesta.** Herramientas → *Compresor z3q* (**hoja modal** con dos acciones). Casos de uso: CU-A58 `ComprimirColumnasAZ3q(nivel)`, CU-A59 `DescomprimirZ3qAColumnas`. Evidencia: `P:CompresorViewModel.cs:65-232`.

---

### Pantalla 23 · ConfigCPsFrm — «Configurar Columnas Probables» (reglas del Generador CP)

**Propósito.** Edita, por «tipo de CP», los tramos de configuración (rango de valoración y reglas de forzado de fijos/dobles/triples) que usa el generador de columnas probables. **Apertura.** Desde `GenerarCPs` (botón «Configurar columnas», `P:GenerarCPsViewModel.cs:195`); en el registro (`W:Navigation/PortedPages.cs:113`), sin menú propio.

**Entradas** (`P:ConfigCPsFrmViewModel.cs:78-139`, `P:ConfigCPsFrmPage.xaml`)
| Control | Detalle |
|---|---|
| «Tipo de CP» | desplegable con los nombres de la tabla «Tipos de CPs» (primer tipo seleccionado al abrir) |
| Filas del tipo | por fila: «Desde», «Hasta» (NumberBox ≥0), «Forzar Fijos» (Sí/No), «Nº Fijos» (≥0), «Forzar Dobles», «Nº Dobles» (≥0), «Forzar Triples» |
| «Guardar» / «Volver» | `Volver` = `Frame.GoBack()`; no hay botones de añadir/borrar filas ni tipos |
| Leyenda | textos fijos; «Si se hacen columnas de sólo fijos o dobles, se recomienda poner Desde=0 y Hasta=100.» |

**Lógica de negocio en el VM**
```
Abrir: ds = DatosHelper().ObtenerDatos()   // lee <BaseDirectory>/ColumnasProbables.xml (DataSet tipado) si existe; si no, DataSet vacío (D:EntradaSalida/GenerarCPs/DatosHelper.cs:14-29)
   tipos = filas de «Tipos de CPs» (Nombre→Tipo:int), sin nombres repetidos ; tipoSeleccionado = primero
Cambiar tipo: filas = filas de «Configuracion de CPs» con Tipo == tipo (RowFilter "Tipo = n"), con Desde,Hasta,Num*:double (nulo→0) y Forzar*:bool (nulo→false)
Guardar: sobre las filas EXISTENTES del tipo y en orden: row[Desde]=(int)Desde ; row[Hasta]=(int)Hasta ; row[Forzar Fijos] ; row[Num Fijos]=(int) ; row[Forzar Dobles] ; row[Num Dobles]=(int) ; row[Forzar Triples]
         n = min(filas del dataset, filas en pantalla) ; ds.AcceptChanges() ; DatosHelper.GuardarDatos(ds) → ds.WriteXml(<BaseDirectory>/ColumnasProbables.xml) (sobrescribe)   // el flag HayCambiosSinGuardar solo se pone a false, nunca a true
```
- El XML entregado (`W:Datos/ColumnasProbables.xml`) tiene 1 tipo «Normal» (Tipo=1) y 2 filas de configuración (una sin atributo `Tipo`); una fila sin `Tipo` no aparece en ningún tipo (RowFilter `Tipo = 1`).
- Los truncamientos `(int)` descartan decimales; la lógica de interpretación de estas reglas está en el generador (`D:EntradaSalida/GenerarCPs/CPs.cs`, ESPEC_04), no aquí.

**Ficheros.** Lee/escribe `W:Datos/ColumnasProbables.xml` (XML de DataSet, namespace `http://tempuri.org/ColumnasProbables.xsd`). En Mac: `~/Library/Application Support/Free1X2/ColumnasProbables.xml` (con copia inicial desde el bundle). Estado global: ninguno.

**Salidas.** Ninguna (persistencia).

**Área Mac propuesta.** Columnas probables → *Generador* → «Reglas de generación»: **hoja modal** con `Table` editable por tipo. Casos de uso: CU-A60 `LeerConfiguracionGeneradorCP`, CU-A61 `GuardarConfiguracionGeneradorCP`. Evidencia: `P:ConfigCPsFrmViewModel.cs:141-204`.

---

### Pantalla 24 · ConfiguracionAnalisisFrm — «Configurar Análisis»

**Propósito.** Activa/desactiva los 20 tipos de análisis estadístico que el programa calcula al analizar columnas. **Apertura.** Menú *Free1x2 → Configurar análisis…* y botón `bConfAnalisis` (`W:MainWindow.xaml.cs:169,356`).

**Entradas** (`P:ConfiguracionAnalisisFrmViewModel.cs:28-92`, `P:ConfiguracionAnalisisFrmPage.xaml`): 20 interruptores (`ToggleSwitch`), agrupados en la página como
- *Signos y secuencias*: VX2, Signos seguidos, **Figuras V1X2** (solo si Signos seguidos), Dibujos, Interrupciones, Simetrías, Diferencias.
- *Formatos y grupos*: Grupos equipos, Formatos, Formatos 123, Control de grupos, Control de conjuntos.
- *Pesos y valoración*: Pesos numéricos, **Figuras de pesos numéricos** (solo si Pesos), Valoración, Columnas probables, Distancias.
- *Contactos*: Contactos, **Figuras de contactos** (solo si Contactos).
Más «Marcar todo», «Desmarcar todo», «Guardar» y «Salir». Existe además un flag persistido `aTodo` sin control propio (`_aTodo`, `:26`).

**Lógica de negocio en el VM**
```
Dependencias: Seguidos→(FigurasV1X2Habilitado = valor ; si !valor FigurasV1X2=false) ; Contactos→FigurasContactos ; Pesos→FigurasPesos   (mismo patrón)
MarcarTodo: los 20 = true ; habilita los tres «Figuras…» ; _aTodo=true        DesmarcarTodo: los 20 = false ; _aTodo=false
Cargar (al abrir): AConfiguracion.ObtenConfiguracionAnalisis → 20 booleanos (nodo <analisis><Opciones …> de W:Datos/parametros.free1x2: aTodo, aVX2, aSeguidos, aFigurasV1X2, aDibujos, aInterrupciones, aSimetrias, aDiferencias, aFormatos, aFormatos123, aDistancias, aContactos, aFigurasContactos, aPesos, aFigurasPesos, aValoracion, aCPs, aGruposEquipos, aControlGrupos, aControlConjuntos)
        orden de asignación: VX2, Seguidos, FigurasV1X2, Dibujos, Interrupciones, Valoracion, Formatos, Contactos, FigurasContactos, Formatos123, Simetrias, Diferencias, Distancias, Pesos, FigurasPesos, ColumnasProbables, GruposEquipos, ControlGrupos, ControlConjuntos
        (los «Figuras…» se asignan DESPUÉS de su padre: el valor guardado manda aunque el padre esté apagado; solo queda deshabilitado)
        excepción → solo log (Log.Error) y valores por defecto (false)
Guardar: AConfiguracion.GuardarConfiguracionAnalisis(_aTodo, 19 flags en el orden de arriba…) ; VariablesGlobales.ReinicializarVariables() (relee la config) ;
        error → MostrarError("No se pudo guardar la configuración de análisis: " + msg) ; la página SIEMPRE vuelve atrás después (Click tras Command)
```
- Valores por defecto del fichero entregado: todos `True` (`W:Datos/parametros.free1x2:28-30`); `VariablesGlobales.SetDefaultValues` pone `analizarTodo=true` (`D:VariablesGlobales.cs:169`).
- La forma exacta de leer/escribir el XML (atributos, formato `True/False`) está en `D:EntradaSalida/AConfiguracion.cs` (ESPEC_07). El efecto de cada flag lo consume el contenedor de análisis (ESPEC_06).

**Ficheros.** Lee/escribe `W:Datos/parametros.free1x2`. Estado global: `VariablesGlobales` (se reinicializa tras guardar).

**Salidas.** Ninguna (persistencia).

**Área Mac propuesta.** Ajustes → *Análisis* (panel de ajustes `Settings` con `Toggle`s agrupados y dependencias de habilitación). Casos de uso: CU-A62 `LeerOpcionesAnalisis`, CU-A63 `GuardarOpcionesAnalisis` (repositorio de configuración, valor `OpcionesAnalisis` con 20 booleanos). Evidencia: `P:ConfiguracionAnalisisFrmViewModel.cs:101-248`.

---

### Pantalla 25 · ConfiguracionFrm — «Configuración»

**Propósito.** Edita los parámetros persistentes del programa (`W:Datos/parametros.free1x2`): boleto, desplazamiento, puntuación CP, separador JB, parámetros L.A.E., actualizador, idioma y confirmación al salir. **Apertura.** Menús *Free1x2 → Configuración…* y *Ver → Configuración…*, botón `bConfig` (`W:MainWindow.xaml.cs:168,192,355`).

**Entradas** (`P:ConfiguracionFrmViewModel.cs:25-90`, `P:ConfiguracionFrmPage.xaml`)
| Grupo | Campo | Tipo / rango | Defecto (antes de cargar) |
|---|---|---|---|
| Boleto | «Nº de partidos en boleto» | NumberBox 1..15 | 14 |
| | «Separador de partidos» | texto (`"5,9,12"`: partidos tras los que se pinta separación) | `5,9,12` |
| Desplazamiento | «Desplazamiento de grupo/condición» | NumberBox 0..99 | 3 |
| Puntuación en CPs | Fijos / Dobles / Triples | NumberBox (sin rango) | 0 |
| Separador % JB | 6 límites | NumberBox, estrictamente crecientes | 15, 22, 29, 36, 48, 61 |
| Parámetros L.A.E. | Recaudación | NumberBox | 15 000 000 |
| | Símbolo moneda | TextBox `MaxLength=3` | `€` |
| | Precio apuesta | NumberBox | 0.50 |
| | «% al 14» | NumberBox 0..100 | 15 |
| Preferencias | «Comprobar actualizaciones al inicio» | interruptor | off |
| | «Pedir confirmación al salir» | interruptor | off |
| | Idioma | desplegable; solo `es-ES` más el valor leído de la config (`:87-90`) | `es-ES` |
Botones «Guardar» y «Volver» (`Frame.GoBack()`). El menú *Ver → Tema/Idioma* (`W:MainWindow.xaml.cs:195-207`, preferencias de tema e idioma de la interfaz) es independiente de esta pantalla.

**Lógica de negocio en el VM**
```
Cargar (al abrir; cualquier excepción → solo log y se quedan los defectos):
   cfg = AConfiguracion()   // <BaseDirectory>/parametros.free1x2 (XML)
   JB = ObtenValoresUtilSeparadorJB().Split(',') ; si ≥6 elementos → Jb1..Jb6 = ParseInv(texto)  // double.TryParse invariante, si no es-ES, si no 0
   (precio, pct, recaudación, moneda) = ObtenValoresLAE()      // float.Parse con cultura es-ES → asignado a double (precisión float, T3)
   (fijos, dobles, triples) = ObtenPuntosCP() ; desplazamiento = ObtenDesplazamiento() ; (nº partidos, separador) = ObtenNumPartidos()
   actualizar = ObtenConfiguracionActualizador() ; confirmarSalir = ObtenConfiguracionAdvertenciaSalir() ; idioma = ObtenInfoIdioma() (se añade a la lista si falta)
Guardar:
   validación: para i=1..5: si jb[i−1] >= jb[i] → MostrarError("El valor del campo " + i + " no puede ser mayor o igual que el siguiente.") y NO guarda nada
   GuardarValoresSeparadorJB([(int)jb_i invariante]) ; GuardarValoresLAE(precio, pct, recaudación, moneda) ;
        → atributos PrecioApuesta = precio.ToString("0.00", es-ES) (coma decimal), PorcentajeDestinadoAl14 y Recaudacion = ToString(es-ES), Moneda   (D:EntradaSalida/AConfiguracion.cs:367-384)
   GuardarPuntosCP((int),(int),(int)) ; GuardarConfiguracionBoleto((int)nº partidos, separador) ; GuardarDesplazamiento(((int)valor).ToString(invariante)) ;
   GuardarConfiguracionActualizador(bool) ; si idioma no vacío → GuardarIdioma(idioma) ; GuardarConfiguracionAdvertenciaSalir(bool)
   VariablesGlobales.ReinicializarVariables() ; MostrarInfo("Configuración guardada. Algunos cambios pueden requerir reiniciar la aplicación.")
   error → MostrarError("No se pudo guardar la configuración: " + msg)       // cada Guardar* reescribe el XML completo (no es transaccional)
```
- La pantalla **no** vuelve atrás al guardar (solo «Volver»). No valida `separador`, precio ≥ 0, ni coherencia nº de partidos/separador.
- Efecto: `VariablesGlobales` (44 miembros) se recarga; en Swift pasa a un valor `Configuracion` inyectable (T10).
- `Moneda`, `PrecioApuesta`, `NumeroPartidos` los usan Calcular (pantalla 13), Banco de Pruebas (constantes propias), etc.

**Ficheros.** Lee/escribe `W:Datos/parametros.free1x2` (ver ESPEC_07). Estado global: `VariablesGlobales.ReinicializarVariables()`.

**Salidas.** Diálogos de información/error.

**Área Mac propuesta.** Ajustes (ventana `Settings` ⌘, con pestañas Boleto · LAE · Columnas probables · Preferencias); ajustes de tema/idioma en la misma ventana. Casos de uso: CU-A64 `LeerConfiguracion`, CU-A65 `GuardarConfiguracion(valores)`, CU-A66 `ValidarSeparadorJB`. Evidencia: `P:ConfiguracionFrmViewModel.cs:98-208`.

---

### Pantalla 26 · ContactosFrm — condición «Contactos»

**Propósito.** Editor de la condición *Contactos* del grupo en pantalla: por cada una de 10 parejas de signos, qué números de contactos (0–14) admite la columna, más una condición opcional de figuras. Sigue el **patrón de pantalla de condición (PC)** descrito en §1.6. **Apertura.** Botón «Contactos» de la rejilla de condiciones de Inicio (`V:MainPageViewModel.cs:171,291-296`, `AppState.GrupoEnEdicion`); menú: no. En el registro `W:Navigation/PortedPages.cs:23`.

**Entradas** (`P:ContactosFrmViewModel.cs:49-95`, `P:ContactosFrmPage.xaml`)
| Control | Detalle |
|---|---|
| 10 filas «Pareja → Valores» | parejas `1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV` (V = X ó 2); texto de números separados por comas, placeholder «p.ej. 0,1,2,3»; defecto al crear la VM `0,1,…,13` (`:49`) |
| «Editar figuras…» + semáforo | verde si hay figuras (`TieneFiguras`) |
| Botones PC | Aceptar · Guardar · Abrir · Copiar · Pegar · Estadísticas · Borrar · Cancelar |

**Lógica de negocio en el VM**
```
Cargar (entrada normal; NO al volver de Figuras): filtro = grupo.GetFiltro("Contactos") ; Filas[i] = filtro.GetNum1X()/12/X2/11/XX/22/1V/XV/2V/VV ; figuras = filtro.Figuras ; TieneFiguras = figuras.Count>0
GuardarDatos(filtro):
   todos = "0,1,…,15"    (ObtenerTodosValores(0,15))
   ContieneDatos = alguna fila no vacía/espacios  ∨  figuras no vacías
   si ContieneDatos: si !filtro.ContieneDatos → filtro.IsActive=true ; filtro.ContieneDatos=true ; filtro.Inicializa() ;
        filtro.SetNum1X(fila0 o todos) … SetNumVV(fila9 o todos) ; si hay figuras → filtro.Figuras = figuras
   sino: filtro.IsActive=false ; filtro.ContieneDatos=false
Aceptar: (grupo nil → solo volver) filtro=…GetFiltro("Contactos") ; GuardarDatos(filtro) ; filtro.UsaFiguras() ; grupo.ActivaFiltro(filtro) ; AppState.NotificarCambio() ; volver
Cancelar: volver sin aplicar
Borrar: todas las filas "" ; figuras=nil ; TieneFiguras=false          // no toca el grupo hasta Aceptar
EditarFiguras: figuras ??= [] ; FigurasFiltrosFrmViewModel.FigurasEnEdicion = figuras (MISMA referencia, el editor la muta) ; NombreCondicion="Contactos" ; navegar a FigurasFiltrosFrm ; al volver (NavigationMode.Back) solo refresca el semáforo
Estadísticas: filtroTemp = new FiltroContactos() ; GuardarDatos(filtroTemp) ; UsaFiguras() ; lista = CalculadorEstadisticas().EstadisticasFiltro(filtroTemp, <BaseDirectory>/Ganadoras/) ; VisorEstadisticasViewModel.UltimasEstadisticas = lista ; navegar a VisorEstadisticas
Guardar/Abrir/Copiar/Pegar: ver PC. Extensiones: guardar `.cont` («Contactos») y `.xml` («Contactos (XML)»), nombre sugerido «Contactos» ; abrir `.cont`/`.xml` ; temporal `Temp/tmp.cont`
   GuardarEn(ruta): GuardarDatos(filtro REAL del grupo) → ArchivoCondiciones{NombreArchivo}.GuardaArchivo(filtro)         // ← Guardar y Copiar MODIFICAN el filtro del grupo aunque luego se pulse Cancelar
   AbrirDesde(ruta): ArchivoCondiciones.AbrirArchivoCombinacion(ruta) → LeeCondicion() → grupo temporal → filtro Contactos → rellena filas y figuras (no toca el grupo hasta Aceptar)
```
- Interpretación de los números (contacto = pareja adyacente en cualquier orden) y `FiltroContactos` en ESPEC_02; el rango 0–15 de «todos» frente a «0–14» del texto de ayuda es un detalle a preservar (`todos` incluye 15).

**Ficheros.** `.cont`/`.xml` de condición (ESPEC_07), `Temp/tmp.cont`; lee `<BaseDirectory>/Ganadoras/` para estadísticas. Estado global: `AppState.GrupoEnEdicion` (entrada), `NotificarCambio` (salida), handoff estáticos de figuras y estadísticas.

**Salidas.** Valores del filtro; semáforo de figuras; navegación al visor de estadísticas.

**Área Mac propuesta.** Condiciones y filtros → *Contactos*: **inspector** (panel derecho) sobre la lista de condiciones del grupo, con tabla de 10 filas y botones de archivo en un menú «⋯». Casos de uso: CU-A67 `EditarCondicionContactos` (+ patrón PC: CU-A68 `AplicarCondicion`, CU-A69 `ExportarCondicion`, CU-A70 `ImportarCondicion`, CU-A71 `CalcularEstadisticasCondicion`). Evidencia: `P:ContactosFrmViewModel.cs:111-291`.

---

### Pantalla 27 · ControlGruposFrm — «Control de Grupos»

**Propósito.** Define los *controles de fallos* entre grupos de condiciones («Control Grupos»: qué grupos se controlan juntos y cuántos fallos se permiten) y entre esos controles («Control Conjuntos»), trabajando sobre copias y confirmando con OK. **Apertura.** Botón «Control de grupos» de Inicio (`V:MainPageViewModel.cs:306-312`: `AppState.GrupoEnEdicion = grupo` y navega). No está en menús.

**Entradas** (`P:ControlGruposFrmViewModel.cs:45-75`, `P:ControlGruposFrmPage.xaml`)
| Sección | Controles |
|---|---|
| Control Grupos | `Anterior` · «actual/total» (defecto `1/1`) · `Siguiente` · «Eliminar Actual»; «Grupos» (placeholder «Grupos controlados, p. ej. 1,3,5»); «Fallos» («Fallos permitidos») |
| Control Conjuntos | igual: `Anterior` · contador · `Siguiente` · «Eliminar Actual»; «Conjuntos» («Conjuntos controlados, p. ej. 1,2»); «Fallos» |
| Pie | `OK`, `Cancelar`, texto `Estado` («Preparado» / «Guardado» / «Cancelado») |
Los textos admiten listas `a,b,c` y rangos `a-b` (los expande `UtilidadesEntradasValores.ObtenerValoresSeparadosPorComas`, `D:MotorCalculo/ControlGrupos.cs:181-197`); un valor no numérico dispara `UserDialogs.ShowError("Hay errores en los valores introducidos")` (`D:MotorCalculo/ControlGrupos.cs:194`).

**Lógica de negocio en el VM (pseudocódigo)** — dos secciones idénticas en estructura (G = grupos, C = conjuntos); el control **0** es la base y no se muestra; la navegación empieza en 1.
```
CargarDesdeMotor (OnNavigatedTo): ctrl = AppState.Analizador.CtrlGrupos ; cgPantalla = cConjPantalla = 1
   copiasG = clon de ctrl.ControlesGrupos (GruposControlados texto, FallosPermitidos texto, CtrlGrupos, UsaControlGrupos)
   copiasC = clon de ctrl.ControlesConjuntos (CtrlGruposControlados texto, FallosPermitidos texto)
Pantalla(n): si copias.Count > 1: textos = copias[n] ; contador = "n/(Count−1)" ; sino textos "" y "1/1"
TieneDatos = Grupos != "" && FallosGrupos != ""            (idem Conjuntos)
GuardarActual: si pantalla < Count: si TieneDatos → copias[pantalla] ← textos
               sino si TieneDatos → nueva ControlGrupos{CtrlGrupos=ctrl} añadida y rellenada
Cambia(n): GuardarActual() ; pantalla=n ; si Count < n+1 → añade una vacía ; Pantalla(n)
Anterior: si pantalla ≤ 1 no hace nada ; Cambia(pantalla−1)         Siguiente: solo si TieneDatos → Cambia(pantalla+1)
EliminarActual: si pantalla==1: quita copias[1] solo si Count>1 ; sino: quita copias[pantalla] y pantalla−− ; Pantalla(pantalla)   // RemoveAt sin comprobar rango en el caso pantalla>1
NecesitaBorrarUltima: Count>1 y la última tiene Grupos=="" o Fallos==""        
GruposLibres (G): libres = "0" + ",k" para cada grupo k=1..GruposPartidos.Count−1 que NO aparezca en ningún control i≥1 (ContieneGrupo(k)) ; base[0].PonerGruposControlados(libres) ; base[0].UsaControlGrupos=false
ConjuntosLibres (C): libres = "0" + ",k" para k=1..ctrl.ControlesGrupos.Count−1 sin control i≥1 que lo contenga (ContieneConjunto(k)) ; base[0].PonerCtrlGruposControlados(libres) ; base[0].PonerFallosPermitidos("0")
GuardarDatos (G): GuardarActual() ; si Count>0 y NecesitaBorrarUltima → quita la última ; GruposLibres() ; ctrl.ControlesGrupos = copiasG
GuardarDatosCConj: lo mismo con conjuntos ; ctrl.ControlesConjuntos = copiasC          // se ejecuta DESPUÉS de G, así que usa el nuevo recuento de controles de grupos
Aceptar: GuardarDatos() ; GuardarDatosCConj() ; AppState.NotificarCambio() ; Estado="Guardado" ; volver
Cancelar: Estado="Cancelado" ; volver (las copias se descartan)
```
- Este árbol (grupo → control de grupos → control de conjuntos) es el que evalúa el motor y el árbol de fallos (pantalla 7 usa `FallosPermitidos[nFallos]` y `ObtenCtrolGruposConjunto`); su semántica detallada en ESPEC_01.

**Ficheros.** Ninguno directo (la combinación se guarda desde Inicio). Estado global: `AppState.Analizador.CtrlGrupos` (lectura/escritura), `NotificarCambio`.

**Salidas.** Contadores «k/n» y textos editables.

**Área Mac propuesta.** Condiciones y filtros → *Grupos → Control de grupos*: **hoja modal** con dos listas (controles de grupos / de conjuntos) con alta/baja explícita (en vez de navegar con `<`/`>`). Casos de uso: CU-A72 `EditarControlGrupos` (buffer + confirmar, cálculo de «libres»), CU-A73 `EditarControlConjuntos`. Evidencia: `P:ControlGruposFrmViewModel.cs:82-453`.

---

### Pantalla 28 · ControlTolFrm — «Control Tolerancias»

**Propósito.** Edita las *tolerancias globales* del grupo en pantalla (pares «letras de tolerancia → aciertos permitidos») y el nº de fallos permitidos en esos controles. **Apertura.** Botón «Tolerancias» de Inicio (`V:MainPageViewModel.cs:313-326`, equivale a `BtnTolGrupoClick`); en el registro (`W:Navigation/PortedPages.cs:24`).

**Entradas** (`P:ControlTolFrmViewModel.cs:44-47`, `P:ControlTolFrmPage.xaml`)
| Control | Detalle |
|---|---|
| «Fallos Controles» | texto, placeholder «Fallos permitidos» («Número de fallos permitidos en los controles.») |
| Tabla «Tolerancia / Aciertos» | siempre **al menos 30 filas** (`LineasMinimas=30`, `:36`), cada una con dos TextBox: letras de tolerancia y aciertos (placeholders «Tolerancia», «Aciertos») |
| `Ok` / `Cancelar` | volver con `Frame.GoBack()` |

**Lógica de negocio en el VM**
```
Cargar (OnNavigatedTo): ctrl = AppState.GrupoActual.ControladorTolerancias           // grupo en pantalla, no GrupoEnEdicion
   filas = ctrl.Tolerancias (LetrasTol, Aciertos) rellenadas hasta max(Count, 30) con filas en blanco ; FallosPermitidos = ctrl.FallosPermitidos
Ok: tolerancias = [ ToleranciaFiltros{LetrasTol, Aciertos} para cada fila con Aciertos != "" ]      // el setter de Aciertos inicializa el array interno y puede lanzar con formato inválido
    ctrl.Tolerancias = tolerancias ; ctrl.FallosPermitidos = FallosPermitidos ; AppState.NotificarCambio() ; volver
Cancelar: volver sin guardar
```
- Formato de `Aciertos`/`FallosPermitidos` (`D:MotorCalculo/ToleranciaFiltros.cs`, `D:MotorCalculo/ControladorTol.cs:43-75`): lista ascendente `a,b,c` (el último valor +2 fija el tamaño) o rango `a-b`, o un solo valor. No hay validación en la UI; un texto inválido provoca excepción no controlada al pulsar Ok (el comando no es async ni captura) → en Swift validar y mostrar error.
- `LetrasTol` son letras que se asignan a los valores de AC/ACS/FS (en las CP, `ColumnaProbable.SetNoAciertos` acepta `3a`, etc.): ver ESPEC_01/04.
- Una fila con letras pero sin aciertos se **descarta** silenciosamente.

**Ficheros.** Ninguno. Estado global: `AppState.GrupoActual.ControladorTolerancias`, `NotificarCambio`.

**Salidas.** Ninguna (el semáforo de «Tolerancias» de Inicio se refresca vía `Cambiado`).

**Área Mac propuesta.** Condiciones y filtros → *Grupos → Tolerancias*: **hoja modal** con tabla editable de filas dinámicas (+/−) y un campo de fallos. Casos de uso: CU-A74 `EditarTolerancias(grupo)`. Evidencia: `P:ControlTolFrmViewModel.cs:60-117`.

---

### Pantalla 29 · CopiarCPFrm — «Copiar Columnas» (CP a otros grupos)

**Propósito.** Copia columnas probables (por índice) del filtro CP del grupo en edición a uno o varios grupos destino del boleto. **Apertura.** Botón «Copiar Columnas» de `ColProbablesFrm` (`P:ColProbablesFrmViewModel.cs:697-703`, que usa `AppState.GrupoEnEdicion`); en el registro (`W:Navigation/PortedPages.cs:114`).

**Entradas** (`P:CopiarCPFrmViewModel.cs:33-61`, `P:CopiarCPFrmPage.xaml`)
| Control | Detalle |
|---|---|
| «Columnas» | TextBox; un índice `5`, lista `1,3,5` o rango `1-5` (placeholder «Ej.: 5  |  1,3,5  |  1-5»); índices **1-based** |
| «Grupos destino» | `ListView` multiselección con «Boleto Base» (grupo 0) y «1», «2», … (`:80-83`); sincroniza `GruposSeleccionados` por nombre (`P:CopiarCPFrmPage.xaml.cs:40-53`) |
| «Copiar» · «Crear grupos» · «Cancelar» | `Crear grupos` navega a `CrearGruposFrm` (pantalla 31) y, al volver, recarga la lista (`OnNavigatedTo`) |
| `Estado` | defecto «Indica las columnas y el grupo destino.» |

**Lógica de negocio en el VM**
```
Cargar: grupos = AppState.Analizador.GruposPartidos ; destinos = ["Boleto Base", "1", …, "n-1"] ; grupoPantalla = AppState.GrupoPantalla
   origen = AppState.GrupoEnEdicion ?? grupos[grupoPantalla] ; _grupoCP = origen.GetFiltro("ColProbables").ColProbables     // la lista CONFIRMADA del filtro (por referencia), no el buffer sin aceptar de ColProbablesFrm
ObtenCP(texto): separador = primer ',' o '-' hallado en el texto (el primero que aparezca) ; ',' → enteros del Split ; '-' → rango a..b inclusive ; si no → un entero      // Convert.ToInt32: excepción si no numérico
IndicesValidos: todo i en [1, _grupoCP.Count]
Copiar:
   Columnas vacío → Estado="Introduce al menos una columna a copiar." ; sin destinos → "Selecciona al menos un grupo destino." ; sin origen → "No hay columnas de origen (abra primero el editor de Columnas Probables)."
   parseo falla → "Expresión de columnas no válida (usa \"5\", \"1,3,5\" o \"1-5\")." ; índice fuera de rango → "Algún índice supera el número de columnas disponibles (N)."
   para cada grupo destino seleccionado (en orden de grupo), para cada índice en el orden dado:
        cpCopia = new ColumnaProbable{Pronosticos = cp.Pronosticos} ; SetNoAciertos(GetAciertos) ; SetNoAciertosSeguidos(...) ; SetNoFallosSeguidos(...)        // NO copia los puntos
        si cp.ToleranciaLocalActiva: SetACTol/ACSTol/FSTol/Tolerancias
        si destino == grupo en pantalla: _grupoCP.Add(cpCopia)        // (la lista del filtro del grupo en edición; ContieneDatos/IsActive no se tocan)
        sino: filtroDestino.ColProbables.Add(cpCopia) ; filtroDestino.ContieneDatos=true ; filtroDestino.IsActive=true
        copiadas++
   NotificarCambio() ; Estado = "Copiadas N columna(s) a G grupo(s)." ; volver
Cancelar: Estado="Cancelado." ; volver
```
- Copiar a un destino que también sea el origen añade duplicados al final; las copias se añaden **al final** del filtro destino (no sustituyen).
- Al volver, `ColProbablesFrm` recarga desde el grupo (`OnNavigatedTo` → `CargarDesdeGrupo`), por lo que las ediciones no aceptadas de esa pantalla se pierden (cf. pantalla 19).

**Ficheros.** Ninguno. Estado global: `AppState.Analizador`, `GrupoPantalla`, `GrupoEnEdicion`, `NotificarCambio`.

**Salidas.** Mensaje de estado con nº de columnas y grupos.

**Área Mac propuesta.** Columnas probables → acción «Copiar a grupos…» como **hoja modal** (selección de columnas en la propia lista y grupos con casillas). Casos de uso: CU-A75 `CopiarColumnasProbablesAGrupos(indices, grupos)`, CU-A76 `ParsearIndices`. Evidencia: `P:CopiarCPFrmViewModel.cs:74-203,208-279`.

---

### Pantalla 30 · CopiarDatosCPFrm — «Copiar Datos» (rango de columnas)

**Propósito.** Diálogo mínimo para elegir el rango [Desde, Hasta] de columnas probables a las que se copiarán los rangos AC/ACS/FS de la columna en pantalla. **Apertura.** Botón «Copiar datos» de `ColProbablesFrm`, con parámetro `(double Desde = cpPantalla+1, double Max = Count)` (`P:ColProbablesFrmViewModel.cs:629-650`, `P:CopiarDatosCPFrmPage.xaml.cs:27-37`).

**Entradas** (`P:CopiarDatosCPFrmViewModel.cs:28-49`, `P:CopiarDatosCPFrmPage.xaml`): dos `NumberBox` «desde» y «hasta» **encadenados**: `Desde ∈ [DesdeMinimo=1, DesdeMaximo=Hasta]`, `Hasta ∈ [HastaMinimo=Desde, HastaMaximo=Max]` (`:83-92`). Botones «Copiar» y «Cancelar», texto `Estado` («Preparado»).

**Lógica de negocio en el VM**
```
Inicializar(desde, max): Desde=desde ; Hasta=max ; HastaMaximo=max ; HastaMinimo=desde ; DesdeMaximo=max          // sin parámetro de navegación quedan en 1/1/1
Desde cambia → HastaMinimo = Desde ; Hasta cambia → DesdeMaximo = Hasta
Copiar: Resultado = (Confirmado:true, (int)Desde, (int)Hasta) [estático] ; Estado = "Copiar columnas d a h" ; volver
Cancelar: Resultado = (false, −1, (int)Hasta) ; DesdeMinimo = −1 ; Desde = −1 ; Estado="Cancelado" ; volver        // −1 = bandera de cancelación del WinForms
```
- El consumidor (`ColProbablesFrm.AplicarCopiaDatos`, pantalla 19) trata `Desde−1 < 0` como cancelado y aplica a las CP `[Desde−1, Hasta)`.
- Si se vuelve con el botón «atrás» (sin Copiar/Cancelar) `Resultado` es `nil` y no se copia nada.

**Ficheros.** Ninguno. **Estado global:** el estático `Resultado`.

**Salidas.** Rango (Desde, Hasta) enteros 1-based.

**Área Mac propuesta.** Columnas probables → **popover/hoja** con dos `Stepper` («Copiar rangos a las columnas de N a M»). Casos de uso: ninguno propio (parámetro de CU-A46 `CopiarRangosAColumnas`, ver pantalla 19). Evidencia: `P:CopiarDatosCPFrmViewModel.cs:73-126`.

---

### Pantalla 31 · CrearGruposFrm — «Crear nuevos grupos»

**Propósito.** Crea N grupos nuevos (con los 14 partidos activos) en la combinación actual. **Apertura.** Botón «Crear grupos» de `CopiarCPFrm` (`P:CopiarCPFrmViewModel.cs:286-296`); en el registro (`W:Navigation/PortedPages.cs:92`), sin menú.

**Entradas** (`P:CrearGruposFrmViewModel.cs:22-33`, `P:CrearGruposFrmPage.xaml`): `NumberBox` «Número de grupos» `Mín 0, Máx 9999`, defecto 0; «Crear» (habilitado si `NumGrupos > 0`) y «Cancelar».

**Lógica de negocio en el VM**
```
Crear: n=(int)NumGrupos ; grupos = AppState.Analizador.GruposPartidos
   repetir n veces: g = new Grupo() ; g.PonerPartidosActivos("1,2,3,4,5,6,7,8,9,10,11,12,13,14") ; grupos.AddGrupo(g)      // lista fija de 14, aunque NumeroPartidos sea otro
   AppState.NotificarCambio() ; NumGrupos=0 ; volver
Cancelar: NumGrupos=0 ; volver
```
- No cambia `GrupoPantalla`; los grupos nuevos nacen sin condiciones. Ver también la creación de grupos desde Inicio (ESPEC_10, controles de grupo de `MainPage`).

**Ficheros.** Ninguno. Estado global: `AppState.Analizador.GruposPartidos`, `NotificarCambio`.

**Salidas.** Ninguna.

**Área Mac propuesta.** Condiciones y filtros → *Grupos* → «Añadir grupos…» (popover con `Stepper`). Casos de uso: CU-A77 `CrearGrupos(n, partidosActivos)`. Evidencia: `P:CrearGruposFrmViewModel.cs:36-54`.

---

### Pantalla 32 · DescargaBoletoFrm — «Descarga de boleto» (jornada online)

**Propósito.** Integración **opcional** con `clubprogol.com`: descarga la jornada vigente de España o México y deja los equipos reales disponibles para el boleto y para «Grupos de Equipos». Sin red cae a la última jornada guardada. **Apertura.** Botón `bObtenerBoletos` («Obtener Boletos Online») de la barra Archivo (`W:MainWindow.xaml.cs:368`); no hay menú (el manual lo cita en *Archivo*).

**Entradas** (`P:DescargaBoletoFrmViewModel.cs:36-46`, `P:DescargaBoletoFrmPage.xaml`): selector «País» (`España`=`es`, `México`=`mx`, `PaisesOnline.Todos`, `W:Services/PaisesOnline.cs:25-29`), botón «Actualizar jornada» (se deshabilita mientras descarga), texto de mensaje. **País inicial** = país de la jornada cacheada más reciente (`JornadaCache.PaisMasReciente()`), o España si no hay caché o falla (`:56-68`).

**Lógica de negocio en el VM**
```
Descargar (ignora si Descargando): pais = PaisSeleccionado.Codigo ("es" por defecto) ; Mensaje = "Conectando con el servicio online…"
   r = QuinielaOnlineService.ObtenerJornadaAsync(pais)     // (ver abajo) → {Jornada, DesdeCache, GuardadoUtc}
   AppState.JornadaActual = r.Jornada          // dispara JornadaCambiada/Cambiado → el boleto y Grupos de Equipos refrescan nombres
   resumen = "Jornada " + J.Jornada + " · " + J.Partidos.Count + " partidos cargados"
   Mensaje = r.DesdeCache ? resumen + " — mostrando la última guardada" + [" (" + fecha + ")"] + "."
                          : resumen + [" · Última actualización: " + fecha]
   fecha = GuardadoUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm")  (omitida si no se conoce)
   excepción distinta de cancelación:
       si JornadaCache.TryCargar(pais) → AppState.JornadaActual = cacheada ; Mensaje = "Sin conexión: mostrando la jornada guardada (actualizada " + fecha + ")."
       sino → Mensaje = (ex es QuinielaOnlineException ? ex.Message : "No se pudo descargar la jornada: " + ex.Message) + " El boleto sigue en modo manual."
ObtenerJornadaAsync(pais):
   pais normalizado Trim+minúsculas ∉ {es,mx} → QuinielaOnlineException("País no soportado: '…' (use 'es' o 'mx').")
   guarda anti-doble: si el mismo país se descargó CON ÉXITO hace <60 s Y existe caché válida → devuelve la caché (DesdeCache=true) sin red
   GET {base}/wp-json/clubprogol/v1/quiniela/{pais}/actual   (base = FREE1X2_API_BASE o https://clubprogol.com ; timeout 10 s ; Accept: application/json ; 429 con Retry-After ≤3 s → un reintento, si más → excepción amigable ; errores JSON {error, mensaje} propagan «mensaje»)
   jornada = JornadaQuinielaParser.Parse(cuerpo) (FormatException → QuinielaOnlineException("La respuesta del servicio no es una jornada válida: …"))
   JornadaCache.Guardar(pais, cuerpo CRUDO) (best-effort) ; marca anti-doble = ahora (solo tras éxito) ; devuelve {jornada, false, JornadaCache.FechaGuardado}
```
- Contrato HTTP y parser: `docs/API_CLUBPROGOL.md`, `Free1X2.Domain/Online/` y caché `%LocalAppData%\Free1X2\jornada-{es,mx}.json` → `~/Library/Application Support/Free1X2/` en Mac (ESPEC_07).
- Al arrancar la app se siembra `AppState.JornadaActual` desde la caché sin red (`W:App.xaml.cs:58,76-93`); esta pantalla es la única que sale a Internet.
- El resumen se muestra también si `Partidos.Count != 14`; no se valida el nº de partidos aquí.

**Ficheros.** Lee/escribe la caché de jornada (JSON crudo). Estado global: `AppState.JornadaActual` (+ eventos `JornadaCambiada`, `Cambiado`).

**Salidas.** Mensaje de resumen/estado (texto plano en español, fecha `dd/MM/yyyy HH:mm` en hora local).

**Área Mac propuesta.** Online → *Jornada* (vista principal con selector de país, botón «Actualizar» y resumen; también acción rápida en la barra). Casos de uso: CU-A78 `ActualizarJornadaOnline(pais)` (con fallback a caché) y CU-A79 `ResolverPaisInicial`. Evidencia: `P:DescargaBoletoFrmViewModel.cs:79-135`; `W:Services/QuinielaOnlineService.cs:88-170`.

---

### Pantalla 33 · DiFiltros — «Diferencias entre filtros»

**Propósito.** Toma el primer fichero de columnas como base y, para cada filtro siguiente, **elimina de la base** las columnas que tengan algún vecino (a distancia 0–3) dentro de ese filtro, según una máscara de niveles y un mínimo; permite grabar el resultado y analizar cuántos 14/13 de una columna ganadora sobreviven. **Apertura.** Menú *Filtros → Diferencias entre filtros…* y botón `bDiferenciasFiltros` (`W:MainWindow.xaml.cs:237,409`).

**Entradas** (`P:DiFiltrosViewModel.cs:25-87,124-164`, `P:DiFiltrosPage.xaml`)
| Control | Detalle |
|---|---|
| «Cargar Filtro» | abrir `.txt` múltiple; añade filas {N, nombre, Activo=sí, Difs=`1111`, Mín=1} (acumula, no limpia) |
| Lista | por fila: N, Filtro, **Activo** (casilla), Cols., **Difs** (TextBox `MaxLength=4`, máscara de niveles 0..3 con `1`/otro), **Mín.** (NumberBox 1..3305, defecto 1), Adm., 14, 13 |
| «Activa / Desactiva» | `CheckBox` defecto marcado; al (des)marcar aplica a todas las filas |
| «Salvar Lista» / «Cargar Lista» | `.lst`, línea `ruta;nombre;Activo;Difs;Mín` (separador `;`, `Activo` como `True`/`False`, Mín entero) |
| «Iniciar Proceso» / «Cancelar» / «Grabar» | `Tiempo` y «Fichero» de salida (solo lectura) |
| «Análisis resultados» | «Fichero…» (abrir `.txt` de ganadoras), columna ganadora (`MaxLength=14`), `+`/`−`, índice, «Analizar» (habilitado tras cargar ganadoras) |

**Codificación**: `ConvertidorDeBases` (partido 0 = dígito menos significativo, `'1'→0`, `'X'`/otro `→1`, `'2'→2`) con `Pot[]` = potencias de 3; espacio 3^14 con `_filtro2` y `_validas` como bitsets (`:105-114,175-180`).

**Lógica de negocio en el VM (pseudocódigo)**
```
CargarLista(.lst): Filtros.Clear ; por línea no vacía: aux=Split(';') ; si <5 campos → ignora ; {ruta=aux0, nombre=aux1, Activo = aux2∈{"True"(sin may/min),"1"}, Difs=aux3, Mín = TryParse(aux4) ?? 1}
Iniciar (si Filtros.Count == 0 no hace nada): validas=∅ ; t0 ; para nf = 0..N−1 en orden (hasta salida):
      fila.Columnas = 0 ; si !fila.Activo → continúa
      abrir fila.Ruta (error → log, fila.Activo=false, continúa) ; filtro2=∅ ; ctcols=0
      para cada línea c (Trim+ToUpper): ctcols++ ; si len(c)<14 → DEJA de leer ; i=S2n(c) ; nf==0 ? validas[i]=true : filtro2[i]=true
      fila.Columnas = ctcols
      si nf==0: ctFR = ctcols ; fila.Admitidas = ctcols          // = nº de LÍNEAS (con duplicados), no de distintas
      sino: Calcular(fila)
      si modo análisis: Escrutar(fila)
   Tiempo = (TimeSpan.ToString()+"0000000000").Substring(0,10)
   // si la fila 0 está inactiva o no abre, validas queda vacío y los siguientes filtros no tienen nada que descartar
Calcular(fila): mincol = clamp((int)Mín, 1, 3305) ; tmp = Difs (si len<4 se rellena con "0" hasta 4) ; tst_k = (tmp[k]=='1') k=0..3
      para nc = 0..3^14−1 ascendente (hasta salida): si validas[nc] && !Valida(nc) → validas[nc]=false ; ctFR−−
      fila.Admitidas = ctFR
Valida(n):  si filtro2[n] && tst0 → true
      si tst1∨tst2∨tst3: na13=na12=na11=0
          para nr=0..13: s1=dígito_nr(n) ; para z1∈{0,1,2}, z1≠s1: col1 = n con dígito nr = z1 ; si filtro2[col1] ∧ tst1: na13++
              si tst2∨tst3: para nr2=nr+1..13: para z2≠dígito_nr2(col1): col2 = col1 con nr2=z2 ; si filtro2[col2] ∧ tst2: na12++
                    si tst3: para nr3=nr2+1..13, z3≠dígito: col3 ; si filtro2[col3]: na11++      // (na11 solo cuenta distancia 3 si tst3)
              y se devuelve true en cuanto na11+na12+na13 >= mincol (comprobaciones a distintos niveles de anidación)
      → false
      Resultado equivalente: n sobrevive si (tst0 ∧ n∈filtro2)  ∨  (nº de columnas de filtro2 a distancia 1 [tst1] + distancia 2 [tst2] + distancia 3 [tst3] de n) ≥ mincol
      (las comprobaciones intermedias solo aceleran: los contadores solo crecen y hay una comprobación tras cada z1 completo)
Escrutar(fila): fila.Acierta14 = validas[prm14] ? 1 : 0 ; fila.Aciertos13 = |{k<28 : validas[prm13[k]]}|
Analizar: prm14 = S2n(ColumnaGanadora) ; prm13 = las 28 columnas a distancia 1 de prm14 en el orden (nr=0..13, z1=0..2 con z1≠dígito) ; modo análisis ; Iniciar() ; modo análisis=false
CargarGanadoras (abrir `.txt`): por línea: VerColumna (Trim, len≥14, 14 primeros, todos ∈ {1,2,x,X}) ; inválida → MostrarError("col.G. errónea") y fin ; máx 3000 ; vacío → nada ;
        nrf=Count (última) ; IndiceGanadora=nrf ; ColumnaGanadora = última ; PuedeAnalizar=true ; +/−: nrf ∈ [1,Count]
Grabar: guardar `.txt` (sugerido «Columnas») ; ArchivoColumnasTexto.GuardarTodasCols(validas): una línea por bit activo, ascendente, ConvNumAColumna(i) ; error → MostrarError("No se pudo grabar: …")
```
- `Cancelar` solo activa `_salida`. «Cualquier cambio» en la lista requiere repetir «Iniciar».
- Los `Difs` aceptan cualquier carácter: solo `'1'` activa el nivel; las posiciones tras la 4.ª se ignoran.

**Ficheros.** Lee `.txt`, `.lst`; escribe `.lst` y `.txt`. Estado global: ninguno.

**Salidas.** Tabla por fila (Cols., Adm., 14, 13), tiempo y nombre de salida; enteros sin formato.

**Área Mac propuesta.** Condiciones y filtros → *Filtros de fichero → Diferencias*; vista principal con `Table` y panel de análisis. Casos de uso: CU-A80 `DiferenciaEntreFiltros` (con `NivelesDiferencia`, `MinimoVecinos`), CU-A81 `EscrutarGanadoraSobreResultado`, CU-A82 `GuardarListaDiferencias`/CU-A83 `CargarListaDiferencias`. Evidencia: `P:DiFiltrosViewModel.cs:272-441`.

---

### Pantalla 34 · DialogoAnalisisMultipleDeTramosFrm — «Definición múltiple de jornadas y ficheros»

**Propósito.** Diálogo de configuración del *análisis múltiple de tramos* (pantalla Tramificar, ESPEC_09/10): fichero de valoraciones históricas, qué combinación analizar (misma combinación sobre varias temporadas, o una por jornada) y criterio de selección de jornadas por recaudación e importes de premios. **Apertura.** Desde `TramificarForm` (`P:TramificarFormViewModel.cs:1125-1126`, solo navega); en el registro (`W:Navigation/PortedPages.cs:131`).

**Entradas** (`P:DialogoAnalisisMultipleDeTramosFrmViewModel.cs:79-160`, `…Page.xaml`)
| Zona | Controles |
|---|---|
| Valoraciones | texto solo lectura + «Seleccionar» (abrir `.txt`/`*`) |
| Pestaña 0 «Combinación única» | interruptor «Usar el desarrollo de 14 triples» (defecto sí: «14 triples» / «Fichero de combinación»), fichero de combinación + «Seleccionar» (abrir `.txt`; al elegir fuerza «fichero» y `Es14Triples=1`), lista de **temporadas** con casillas, «Todas» / «Ninguna» |
| Pestaña 1 «Una combinación por jornada» | `Temporada` NumberBox 1900..2100 (defecto 2004) con < > (±1) y texto «AAAA/AAAA+1»; `Jornada` NumberBox 0..43 (defecto 1) con ◀▶ (deshabilitados en los extremos); «Añadir», «Eliminar», «Guardar», «Leer»; casilla «Analizar ficheros dentro del desarrollo de 14 triples»; lista «Temporada / Jor. / Fichero» (multiselección) |
| Criterio de selección | Recaudación mín/máx (0 / 25 000 000) e importe de premio mín/máx de 14 (0 / 6 000 000), 13, 12, 11, 10 (0 / 3 000 000 cada uno); NumberBox ≥0 |
| `Aceptar` / `Cancelar` | |

**Lógica de negocio en el VM**
```
CargarListaDeTemporadas(rutaValoraciones):   // al seleccionar fichero o por Inicializar(ruta)
   temporadas=[] ; ant="" ; por cada línea: campos = línea(Trim+ToUpper).Split(TAB) ; si campos.Count != 44 → fichero inválido
        si campos[0] != ant → ant=campos[0] ; añade temporada ant (sin seleccionar)         // agrupa temporadas CONSECUTIVAS iguales
   ok → Temporada = Int(ant[0..4]) (si parsea) ; Jornada=1
   no ok (o excepción) → rutaValoraciones="" ; Temporadas.Clear ; MostrarError("El fichero no es de valoraciones históricas")
Añadir: abrir `.txt` ; EsFicheroDeColumnas(ruta) falso → MostrarError("El fichero seleccionado no es un fichero de columnas válido")
        EsFicheroDeColumnas: la 1.ª línea (Trim) tiene ≥14 caracteres y los 14 primeros ∈ "1xX2"
        jornadaPad = ((int)Jornada).ToString().PadLeft(2,'0') ; fila = Combinacion(temporada="AAAA/AAAA+1", jornadaPad, ruta) ; Jornada += 1 (auto-avance)
Eliminar: borra las filas multiseleccionadas (más la «actual») recorriendo en orden inverso
Guardar (.lst, sugerido «Lista»): una línea por fila: Temporada + " " + Jornada + " " + Ruta        // GuardarCols elimina comas de la línea
Leer (.lst): limpia ; por línea (LeeColumnaSinComas = Trim + ToUpper): temporada = línea[0..9) ; jornada = línea[10..12) ; ruta = línea[13..]       // ← la ruta queda en MAYÚSCULAS (A14)
Aceptar:
   Es14Triples = pestaña 0 ? (Usar14Triples ? 0 : 1) : 2
   pestaña 0: válido si hay fichero de valoraciones y ≥1 temporada marcada ; pestaña 1: válido si hay fichero de valoraciones (la lista puede estar vacía)
   inválido → MostrarError("No se ha introducido toda la información necesaria") ; válido → Aceptado=true ; evento CierreSolicitado
Cancelar: rutaValoraciones="" ; FicheroValoraciones="" ; Aceptado=false ; CierreSolicitado
```
- **Contrato de salida** (propiedades públicas del VM, equivalentes a los campos del form WinForms que leía `MainForm`): `FicheroValoracionesHistoricas`, `FicheroCombinacionRuta`, `Es14Triples` (0 = 14 triples, 1 = fichero, 2 = una por jornada), `ListaCombinaciones` (`Free1X2.Utils.Combinacion{Temporada, Jornada, Path}`), `Aceptado`, y los 12 umbrales. En la página **no hay ningún suscriptor** de `CierreSolicitado` ni consumidor de estas propiedades (búsqueda en `Free1X2.WinUI`): el diálogo del port no se cierra ni entrega datos → «Abiertas» A15. En Swift modelarlo como `ConfiguracionAnalisisMultipleTramos` devuelto por una hoja.
- `ListaCombinaciones` (dominio) y `Combinaciones` (vista) se mantienen con el mismo índice.

**Ficheros.** Lee `.txt` de valoraciones históricas (TSV de 44 columnas; formato en ESPEC_07, «Abiertas» A18), `.txt` de combinación y `.lst`; escribe `.lst`. Estado global: ninguno.

**Salidas.** Listas y textos; resultado del diálogo.

**Área Mac propuesta.** Análisis → *Tramificar* → hoja modal «Análisis múltiple» (`TabView`/`Picker` segmentado con las dos modalidades). Casos de uso: CU-A84 `LeerTemporadasDeValoraciones`, CU-A85 `ValidarFicheroDeColumnas`, CU-A86 `GestionarListaCombinacionesPorJornada` (guardar/leer `.lst`). Evidencia: `P:DialogoAnalisisMultipleDeTramosFrmViewModel.cs:296-541`.

---

### Pantalla 35 · DialogoFiltrarPorLimitesFrm — «Filtrar por límites»

**Propósito.** Diálogo de `Tramificar`: por cada tramo de posiciones (ordenadas por probabilidad) se eligen los números mínimo y máximo de diferencias; las columnas de esos tramos y las que se diferencian en ≤ *máx* signos se **eliminan** del conjunto tramificado. **Apertura.** Botón «Filtrar» de `TramificarForm` (`P:TramificarFormPage.xaml.cs:20-33`: deja `DialogoFiltrarPorLimitesFrmPage.ExtremosEntrada = extremos` y navega); en el registro (`W:Navigation/PortedPages.cs:26`). Resultado de vuelta por el estático `DialogoFiltrarPorLimitesFrmPage.Resultado = (aceptado, extremos)` (`P:DialogoFiltrarPorLimitesFrmPage.xaml.cs:26,47-63`); al volver (modo Back) `TramificarForm` llama a `AplicarFiltroConExtremos` si `aceptado` (`P:TramificarFormViewModel.cs:731-751`).

**Entradas** (`P:DialogoFiltrarPorLimitesFrmViewModel.cs:29-76`, `P:FilaLimite.cs`, `…Page.xaml`): tabla de **10 filas** «Rango de aciertos | Posiciones | Difs. desde | Difs. hasta». Etiquetas (`:47-59`): `< 10`, `de 10 a < 11`, `de 11 a < 12`, `de 12 a < 13`, `de 13 a < 14`, `de < 13 a 12`, `de < 12 a 11`, `de < 11 a 10`, `< 10`, `fila 10`. Posiciones = texto solo lectura «a a b»; **editables**: «Difs. desde/hasta» (NumberBox ≥0) con defectos `(0,4) (1,3) (1,2) (1,1) (1,0) (1,0) (1,1) (1,2) (0,0) (0,0)` (`:65-69`). Botones «Aceptar» / «Cancelar».

**Lógica de negocio**
```
Matriz extremos int[10][4]: [i][0]=posición inicial, [i][1]=posición final, [i][2]=difs mín, [i][3]=difs máx
Los extremos de posición los calcula Tramificar (ObtenerExtremos, P:TramificarFormViewModel.cs:774-810) a partir de Min10..Min14 / Max14..Max10 y del nº de columnas iniciales:
   fila0=[0, Min10−1] · fila1=[Min10−1, Min11−1] · fila2=[Min11−1, Min12−1] · fila3=[Min12−1, Min13−1] · fila4=[Min13−1, Min14−1]
   fila5=[Max14, Max13] · fila6=[Max13, Max12] · fila7=[Max12, Max11] · fila8=[Max11, Max10] · fila9=[Max10, nColumnasIniciales−1]
Inicializar(extremos): Coherenciar() ; fila i muestra posIni = (i==0 ? e[i][0] : e[i][0]+1) , posFin = e[i][1] ; texto «posIni a posFin» ; difs = e[i][2], e[i][3]
Coherenciar(): para i=0..8: si e[i+1][0] < e[i][0]+1 → e[i+1][0] = e[i][0]+1
Aceptar: PonerTextoEnVariables(): para i<10: e[i][0]=fila.PosInicial (valor MOSTRADO, ya con +1 salvo i=0) ; e[i][1]=fila.PosFinal ; e[i][2]=(int)DifMin ; e[i][3]=(int)DifMax
            y después e[9] ← COPIA de e[8] (pos y difs; réplica del WinForms, que reutilizaba los controles de la fila 9)  → Coherenciar() → ValoresAceptados=true
Cancelar: ValoresAceptados=false
Consumidor (EjecutarFiltro, P:TramificarFormViewModel.cs:755-770): para i<10: si e[i][0]>0 → e[i][0]−− (deshace el +1 mostrado) ;
      para x en [e[i][0], e[i][1]):  si e[i][2]==0 → Bits[Ap14T[x].Columna]=true (marca la columna de la posición x como ELIMINADA)
                                      si e[i][3]>0 → EliminarColumnas(Ap14T[x].Columna, 0, e[i][3])   // marca todas las columnas a distancia 1..e[i][3] de esa columna
      total = nº de columnas con Bits==false (las que quedan)
```
- Efecto práctico de la réplica: la fila 10 (`e[9]`) nunca usa sus propios extremos; coincide con la fila 9 (solapadas). Preservar o corregir con aprobación del dueño (→ «Abiertas» A16).
- `difs desde == 0` elimina la columna de la posición; `difs desde > 0` no la elimina pero sí sus vecinos hasta *máx* (el «desde» solo se compara con 0).

**Ficheros.** Ninguno. **Estado global:** los estáticos de entrada y resultado.

**Salidas.** Matriz 10×4 de enteros.

**Área Mac propuesta.** Análisis → *Tramificar* → hoja modal «Filtrar por límites» con `Table` de 10 filas (columnas de posición no editables). Casos de uso: CU-A87 `FiltrarTramosPorLimites(extremos)` (la parte `Coherenciar` + eliminación se extrae a Core). Evidencia: `P:DialogoFiltrarPorLimitesFrmViewModel.cs:86-158`.

---

### Pantalla 36 · DialogoGrabarBancoPruebasFrm — «Grabar apuestas» (rango del Banco de Pruebas)

**Propósito.** Diálogo recolector de parámetros para grabar a `.txt` las apuestas del resultado por columnas del Banco de Pruebas: fila inicial/final, nº máximo y «solo seleccionadas». No graba: lo hace el llamador. **Apertura.** Botón «Grabar» del Banco de Pruebas (pantalla 10), hospedado en un `ContentDialog` (`P:BancoPruebasFrmPage.xaml.cs:36-64`); en el registro (`W:Navigation/PortedPages.cs:132`).

**Entradas** (`P:DialogoGrabarBancoPruebasFrmViewModel.cs:25-46`, `…Page.xaml`): «Fila de la apuesta inicial» (NumberBox ≥0), «Fila de la apuesta final» (≥0), «Nº máximo de apuestas» (≥0; 0 = sin tope), interruptor «Grabar solo si están seleccionadas» (defecto **sí**), `Estado`. Valores iniciales = `(c1, c2, c)` calculados por el llamador (primera fila marcada 1-based, última marcada 1-based, nº de marcadas).

**Lógica de negocio**
```
Grabar: si FilaInicial > FilaFinal → MostrarError("La fila inicial no puede ser mayor que la fila final.") y sigue abierto
        resultado = ((int)FilaInicial, (int)FilaFinal, (int)NumMaxColumnas, SoloSeleccionadas) ; Aceptado=true ; Estado="Aceptado" ; CierreSolicitado
Cancelar: Cancelado=true ; Aceptado=false ; Estado="Cancelado" ; CierreSolicitado
Uso del resultado (pantalla 10, GrabarAsync): c1=FilaInicial ; c2=FilaFinal+1 ; si c2 > nApuestas − c1 → c2 = nApuestas − c1 ; c=NumMax ; c1−− ; c2−− ; filas c1..min(n, c2+1) ; salta no marcadas si SoloSeleccionadas ; corta al llegar a NumMax (>0)
```
- La comprobación `FilaInicial > FilaFinal` es una añadida del port (el WinForms no la tenía, `L:UI/DialogoGrabarBancoPruebasFrm.cs`); el cómputo de `c2` mezcla filas 1-based con `nApuestas − c1` (se replica tal cual).

**Ficheros.** Ninguno (el llamador guarda `.txt`). Estado global: ninguno.

**Salidas.** Tupla `(aceptado, filaInicial, filaFinal, numMax, soloSeleccionadas)`.

**Área Mac propuesta.** Herramientas → *Simulador* → **hoja modal** (`Form` con `Stepper`s y `Toggle`). Casos de uso: parte de CU-A24 `GrabarColumnasSeleccionadas` (pantalla 10). Evidencia: `P:DialogoGrabarBancoPruebasFrmViewModel.cs:72-116`.

---

### Pantalla 37 · DialogoGrabarTramosFrm — «Grabar Tramos»

**Propósito.** Recoge el rango de columnas y el patrón «grabar N columnas de cada P columnas» con que `Tramificar` volcará los tramos seleccionados a un `.txt`. **Apertura.** Botón «Grabar tramos» de `TramificarForm` (`P:TramificarFormViewModel.cs:683-693`, solo navega); en el registro (`W:Navigation/PortedPages.cs:133`).

**Entradas** (`P:DialogoGrabarTramosFrmViewModel.cs:22-35`, `…Page.xaml`): «Columna inicial» y «Columna final» (NumberBox ≥0; los inicializa el llamador con el rango de los tramos seleccionados), «Columnas a grabar (N)» (≥1, defecto 1), «De cada (P) columnas» (≥1, defecto 1). Botón «Aceptar» (no hay «Cancelar»).

**Lógica de negocio**
```
Grabar: ColumnaInicial=(int)…, ColumnaFinal=(int)…, NumColsPorPaso=(int)N, Paso=(int)P (trunca decimales) ; Aceptado=true ; CierreSolicitado
```
- Consumidor (legacy `TramificarForm.button1_Click`/`DialogoGuardar`, `L:UI/TramificarForm.cs:2627-2740`): sin tramos seleccionados → «No se ha seleccionado ningún tramo de la tabla»; `c1` = columna inferior del primer tramo seleccionado, `c2` = columna superior del último + 1; tras el diálogo se pide el fichero (`Columnas\`, `.txt`) y, si `Paso > noColumnasIniciales/NumIntervalos`, se avisa «No se pueden definir saltos mayores al nº de columnas por tramo»; si no, para cada tramo seleccionado se recorre `nr = c1..c2` de `Paso` en `Paso` y se graban `NumColsPorPaso` columnas consecutivas (descartando las eliminadas por el filtro, `Bits[...]`). 
- En el port **nadie consume** el resultado ni suscribe `CierreSolicitado` (la página no se cierra y `P:TramificarFormViewModel.cs:856` usa `Paso=1`) → «Abiertas» A15.
- El diálogo no valida `inicial ≤ final`.

**Ficheros.** Ninguno. Estado global: ninguno.

**Salidas.** `(ColumnaInicial, ColumnaFinal, NumColsPorPaso, Paso)`.

**Área Mac propuesta.** Análisis → *Tramificar* → **hoja modal** «Exportar tramos». Casos de uso: `ExportarTramosSeleccionados(rango, n, p)` (propiedad de Tramificar, ESPEC_09/10). Evidencia: `P:DialogoGrabarTramosFrmViewModel.cs:72-84`.

---

### Pantalla 38 · DibForm — «Dibujos» (resultado de Estadísticas, modo Variantes X 2)

**Propósito.** Visor de solo presentación de la matriz 15×15 de *dibujos* (nº de X en filas × nº de 2 en columnas) calculada por `Anastatics`; muestra porcentajes o recuentos con totales por fila, por columna y por nº de variantes. **Apertura.** «Mostrar resultados» de `Anastatics` en modo 0 (`P:AnastaticsViewModel.cs:172-176`, deja `DibFormViewModel.MatrizEntrada` y `NumColEntrada`); en el registro (`W:Navigation/PortedPages.cs:134`).

**Entradas** (`P:DibFormViewModel.cs:56-100`, `P:DibFormPage.xaml`): solo el interruptor «Modo de visualización» (`Porcentajes` por defecto / `Columnas`), y la matriz entregada por el estático (si no hay → ceros).

**Lógica de negocio en el VM**
```
Repintar(): NumColTexto = numcol (invariante)
   para nf=0..14: fila = [ celda(nf,nc) para nc=0..14 ] ; visible solo si nf+nc < 15 (matriz triangular; las demás celdas vacías) ; sumaFila = Σ_nc rsl[nf][nc] (long, incluye celdas no visibles)
   TotalesColumna[nc] = Σ_nf rsl[nf][nc] ; TotalesVariantes[v] = Σ_{nf+nc==v} rsl[nf][nc], v=0..14        (numvars de 29 posiciones; solo se muestran 0..14)
   Formatear(valor: Int64):
        modo Columnas → valor.ToString(invariante)                    // entero sin separadores
        modo Porcentajes → divisor = numcol==0 ? 1 : numcol ; pct = Double( (valor*10000 / divisor) ) / 100.0     // división ENTERA de Int64 antes de /1E2 (2 decimales truncados)
                           texto = pct.ToString(invariante) → «0», «12.34», «100» (representación mínima de .NET: sin ".0" final)
```
- En Swift: `Int64`; formatear con 0–2 decimales eliminando ceros finales de forma equivalente a `Double.ToString()` de .NET (p. ej. `5` y no `5.0`). Probar con el golden de `Anastatics`.

**Ficheros.** Ninguno. **Estado global:** estáticos `MatrizEntrada`, `NumColEntrada`.

**Salidas.** Tabla triangular 15×15 + «Σ fila», «Σ col», «Variantes (X + 2)»; «Columnas analizadas: N».

**Área Mac propuesta.** Análisis → *Estadísticas de fichero*: **panel de detalle** de la vista principal (pantalla 9) con `Picker` Porcentajes/Columnas. Casos de uso: CU-A88 `FormatearMatrizDibujos(modo)` (presentación, trivial). Evidencia: `P:DibFormViewModel.cs:124-188`.

---

### Pantalla 39 · DibRepFrm — «Coincidencias» (resultado de Estadísticas, modo Sus coincidencias)

**Propósito.** Visor de solo presentación de la matriz 5×15 de *coincidencias con la columna anterior* y repeticiones de cantidades (1, X, 2, variantes). **Apertura.** «Mostrar resultados» de `Anastatics` en modo 1 (`P:AnastaticsViewModel.cs:177-183`: entrega las 5 primeras filas de `rsl` y `numcol`); en el registro (`W:Navigation/PortedPages.cs:135`).

**Entradas** (`P:DibRepFrmViewModel.cs:34-66`, `P:DibRepFrmPage.xaml`): interruptor «Modo de visualización» (`Porcentajes` por defecto, `Columnas (conteo)`); matriz por estático (si no hay → `int[5,15]` a ceros).

**Lógica de negocio en el VM**
```
Etiquetas de fila: «posiciones», «cant. 1», «cant. X», «cant. 2», «cant. V»  ; cabeceras de columna «0»…«14»
Repintar(): divisor = numcol==0 ? 1 : numcol ; para f<filas, c<cols: valor=rsl[f][c] (Int32)
      modo Porcentajes → texto = ( (valor*10000 / divisor) / 1E2 ).ToString(invariante)      // ← aquí `valor*10000` es Int32: DESBORDA y envuelve si valor > 214 748 (T1; Anastatics admite hasta 500 000 columnas) → usar &* en Swift para replicar
      modo Columnas    → valor.ToString(invariante)
```
- Contraste con `DibForm` (pantalla 38), que usa `Int64` y no desborda. Preservar el desbordamiento exige test golden con `numcol` grande.
- Significado de filas (`AnastaticsViewModel.Proceso` modo 1, pantalla 9): fila 0 = histograma de coincidencias posición a posición con la columna anterior; filas 1–4 = nº de columnas cuya cantidad de 1 / X / 2 / variantes iguala la de la columna anterior, indexadas por esa cantidad.

**Ficheros.** Ninguno. **Estado global:** estáticos `MatrizEntrada`, `NumColEntrada`.

**Salidas.** Tabla 5×15 de texto; «Columnas totales: N».

**Área Mac propuesta.** Análisis → *Estadísticas de fichero*: panel de detalle (pantalla 9). Casos de uso: CU-A89 `FormatearMatrizCoincidencias(modo)`. Evidencia: `P:DibRepFrmViewModel.cs:89-114`.

---

### Pantalla 40 · DibujosFrm — condición «Dibujos X+2»

**Propósito.** Editor de la condición *Dibujos* del grupo en pantalla: se marcan los *dibujos* `X+Y` (nº de equis + nº de doses) que debe cumplir la columna. Sigue el **patrón de pantalla de condición (PC)**. **Apertura.** Botón «Dibujos» de la rejilla de condiciones de Inicio (`V:MainPageViewModel.cs:160,291-296`); en el registro `W:Navigation/PortedPages.cs:27`.

**Entradas** (`P:DibujosFrmViewModel.cs:59-94`, `P:DibujosFrmPage.xaml`)
| Control | Detalle |
|---|---|
| Malla de casillas | una `CheckBox` por dibujo, etiqueta `X+Y` (ej. `2+3`), orden: `X=0..N`, para cada X `Y=0..N−X` (`GenerarCasillas`, `:134-146`) |
| «Marcar todos» / «Desmarcar todos» | |
| Resumen | «Dibujos seleccionados: n» |
| Botones PC | Estadísticas · Abrir · Guardar · Copiar · Pegar · Borrar · Cancelar · Aceptar |
**Divergencia:** el port fija `NumPartidos = 15` (`const`, `:63`, el comentario habla de `VariablesGlobales.NumeroPartidos` «con respaldo 15») ⇒ 136 casillas; el WinForms usaba `VariablesGlobales.NumeroPartidos` (14 por defecto ⇒ 120 casillas) con respaldo 15 solo ante excepción (`L:UI/Controls/GridDibujosCentral.cs:13-27`). La spec Swift debe usar `Configuracion.numeroPartidos` (→ «Abiertas» A17).

**Lógica de negocio en el VM**
```
Cargar (entrada normal; no al volver de Estadísticas): filtro = grupo.GetFiltro("Dibujos") ; marcadas = { s.Trim() : s ∈ filtro.Dibujos (ArrayList de "X+Y") } ; casilla.Seleccionado = marcadas.contains(etiqueta)
Aceptar: (grupo nil → volver) lista = etiquetas marcadas, en el orden de la malla
     si lista no vacía: filtro.ContieneDatos=true ; filtro.IsActive=true ; filtro.Dibujos=lista       // el setter reconstruye la matriz interna
     sino: filtro.IsActive=false ; filtro.ContieneDatos=false ; filtro.Dibujos=[]
     grupo.ActivaFiltro(filtro) ; AppState.NotificarCambio() ; volver
Cancelar: volver sin aplicar            Borrar: desmarca toda la malla (no toca el filtro del grupo)
ObtenerFiltroTemporal: new FiltroDibujos con la selección (mismas reglas ContieneDatos/IsActive)
Estadísticas: lista = CalculadorEstadisticas().EstadisticasFiltro(filtroTemp, <BaseDirectory>/Ganadoras/) ; VisorEstadisticasViewModel.UltimasEstadisticas = lista ; navegar a VisorEstadisticas
Guardar/Abrir/Copiar/Pegar (ver PC): guardar `.dbj` («Dibujos») o `.xml` («Dibujos (XML)»), nombre sugerido «Dibujos» ; abrir `.dbj`/`.xml` ; temporal `Temp/tmp.dbj`
    GuardarEn(ruta): filtroTemp (selección en pantalla; si hay dibujos → ContieneDatos=IsActive=true) → ArchivoCondiciones{ruta}.GuardaArchivo(filtroTemp)       // NO modifica el filtro real (a diferencia de Contactos)
    AbrirDesde(ruta): ArchivoCondiciones.AbrirArchivoCombinacion → si ok: DesmarcarTodos() ; LeeCondicion() → grupo temporal → filtro "Dibujos" → marca sus etiquetas
```
- Formato del fichero `.dbj`/`.xml` de condición y semántica del filtro (matriz X×2 de dibujos permitidos): ESPEC_03.
- La selección completa (todas marcadas) y la vacía no son equivalentes: vacía **desactiva** el filtro.

**Ficheros.** `.dbj`/`.xml` de condición, `Temp/tmp.dbj`, lee `Ganadoras/` para estadísticas. Estado global: `AppState.GrupoEnEdicion`, `NotificarCambio`, estático de estadísticas.

**Salidas.** Contador de seleccionados; navegación a estadísticas.

**Área Mac propuesta.** Condiciones y filtros → *Dibujos*: **inspector** con rejilla triangular de casillas (`LazyVGrid`) y acciones de archivo en menú «⋯». Casos de uso: CU-A90 `EditarCondicionDibujos` (+ patrón PC: CU-A68, CU-A69, CU-A70, CU-A71). Evidencia: `P:DibujosFrmViewModel.cs:100-310`.

---

## 3. Tabla resumen

### 3.1 Pantalla → área Mac → casos de uso

Áreas y tipos de presentación son **propuestas** (R2). «CU» = casos de uso extraídos al paquete `Free1X2Core/CasosDeUso`; las pantallas 30, 36 y 37 no aportan uno propio (son parámetros de otro caso de uso, ver sus secciones).

| Nº | Pantalla WinUI (ViewModel) | Cómo se abre | Área Mac propuesta | Presentación | Casos de uso |
|---:|---|---|---|---|---|
| 1 | `AgregaP15FrmViewModel` | Menú Combinación → Añadir Pleno al 15…; botón `bP15` | Herramientas · Operaciones sobre columnas | Hoja modal | CU-A01 |
| 2 | `AgregarEquipoFrmViewModel` | Desde el gestor de equipos | Boleto · Equipos | Hoja modal | CU-A02 |
| 3 | `AlgebraColumnasFrmViewModel` | Menú Operaciones → Álgebra de columnas…; botón `bAlgebra` | Herramientas · Operaciones sobre columnas | Hoja modal | CU-A03 |
| 4 | `AnaCombiViewModel` | Botón `bAnalisisGrupos` (barra Utilidades) | Análisis · Análisis de grupos | Vista principal + inspector | CU-A04, CU-A05 |
| 5 | `AnalisisFormatos123FrmViewModel` | Botón «Analizar» de Formatos123 | Análisis · Formatos 123 | Inspector + tabla | CU-A06, CU-A07, CU-A08 |
| 6 | `AnalizadorJPMViewModel` | Desde Formatos | Análisis · Sumas de pares (JPM) | Vista principal + inspector | CU-A09, CU-A10, CU-A11 |
| 7 | `AnalizarCombinacionFrmViewModel` | Desde ColGanadoraFrm; menú Combinación → Analizar combinación… | Análisis · Análisis de fallos | Vista principal | CU-A12, CU-A13 |
| 8 | `AnalizarFicheroFrmViewModel` | Botón `bAnalisisColumnas` (barra Combinación) | Análisis · Analizar fichero | Hoja modal → visor | CU-A14 |
| 9 | `AnastaticsViewModel` | Menús Ver / Combinación → Estadísticas…; botón `bEstadisticas` | Análisis · Estadísticas de fichero | Vista principal | CU-A15 |
| 10 | `BancoPruebasFrmViewModel` | Menú Utilidades → Banco de pruebas…; botón `bBancoPruebas` | Herramientas · Simulador de escrutinios | Vista principal | CU-A16, CU-A17, CU-A18, CU-A19, CU-A20, CU-A21, CU-A22, CU-A23, CU-A24 |
| 11 | `BoletoFrmViewModel` | Desde VerBoletos (ESPEC_09/10) | Cálculo · Boletos | Vista principal | CU-A25, CU-A26 |
| 12 | `BuscaLimsFrmViewModel` | Desde la condición Valoración | Condiciones y filtros · Valoración | Hoja modal | CU-A27 |
| 13 | `CalculaColumnasFrmViewModel` | Botón «Calcular» de Inicio; menú Combinación | Cálculo · Calcular | Vista principal | CU-A28, CU-A29, CU-A30, CU-A31 |
| 14 | `CalculaColumnasMultipleFrmViewModel` | Menú Combinación → Calcular varias…; botón `bCalcularM` | Cálculo · Cálculo por lotes | Vista principal | CU-A32, CU-A33 |
| 15 | `CalculoFormatosFrmViewModel` | Desde la condición Formatos | Condiciones y filtros · Formatos | Hoja modal | CU-A34, CU-A35 |
| 16 | `CambioPuntosFrmViewModel` | Botón de ColProbablesFrm | Columnas probables · Puntuación | Hoja modal | CU-A36, CU-A37 |
| 17 | `CoincidenciasViewModel` | Menú Filtros → Filtro Coincidencias…; botón `bFiltroCoincidencias` | Condiciones y filtros · Filtros de fichero | Vista principal (3 paneles) | CU-A38, CU-A39, CU-A40, CU-A41, CU-A42 |
| 18 | `ColGanadoraFrmViewModel` | Botón `bAnalisisFallos` (barra Combinación) | Análisis · Análisis de fallos | Hoja modal | CU-A43 |
| 19 | `ColProbablesFrmViewModel` | Botón «Columnas Probables» de la rejilla de Inicio | Columnas probables | Vista principal + inspector | CU-A44, CU-A45, CU-A46, CU-A47, CU-A48, CU-A49, CU-A50, CU-A51 |
| 20 | `ColumnasPremiadasFrmViewModel` | «Ver premiadas» desde Escrutinios / EscrutarCombinaciones | Escrutinio · Columnas premiadas | Hoja / panel de detalle | CU-A52 |
| 21 | `CombinarFiltrosViewModel` | Menú Filtros → Combinar filtros…; botón `bCombinarFiltros` | Condiciones y filtros · Filtros de fichero | Vista principal | CU-A53, CU-A54, CU-A55, CU-A56, CU-A57 |
| 22 | `CompresorViewModel` | Menú Utilidades → Compresor z3q… | Herramientas · Compresor z3q | Hoja modal | CU-A58, CU-A59 |
| 23 | `ConfigCPsFrmViewModel` | Botón «Configurar columnas» de GenerarCPs | Columnas probables · Generador | Hoja modal | CU-A60, CU-A61 |
| 24 | `ConfiguracionAnalisisFrmViewModel` | Menú Free1x2 → Configurar análisis…; botón `bConfAnalisis` | Ajustes · Análisis | Panel de Ajustes | CU-A62, CU-A63 |
| 25 | `ConfiguracionFrmViewModel` | Menús Free1x2 / Ver → Configuración…; botón `bConfig` | Ajustes | Ventana Settings (⌘,) | CU-A64, CU-A65, CU-A66 |
| 26 | `ContactosFrmViewModel` | Botón «Contactos» de la rejilla de Inicio | Condiciones y filtros · Contactos | Inspector | CU-A67, CU-A68, CU-A69, CU-A70, CU-A71 |
| 27 | `ControlGruposFrmViewModel` | Botón «Control de grupos» de Inicio | Condiciones y filtros · Grupos | Hoja modal | CU-A72, CU-A73 |
| 28 | `ControlTolFrmViewModel` | Botón «Tolerancias» de Inicio | Condiciones y filtros · Grupos | Hoja modal | CU-A74 |
| 29 | `CopiarCPFrmViewModel` | Botón «Copiar Columnas» de ColProbablesFrm | Columnas probables | Hoja modal | CU-A75, CU-A76 |
| 30 | `CopiarDatosCPFrmViewModel` | Botón «Copiar datos» de ColProbablesFrm | Columnas probables | Popover / hoja | — (parámetro de CU-A46) |
| 31 | `CrearGruposFrmViewModel` | Botón «Crear grupos» de CopiarCPFrm | Condiciones y filtros · Grupos | Popover | CU-A77 |
| 32 | `DescargaBoletoFrmViewModel` | Botón `bObtenerBoletos` (barra Archivo) | Online · Jornada | Vista principal | CU-A78, CU-A79 |
| 33 | `DiFiltrosViewModel` | Menú Filtros → Diferencias entre filtros…; botón `bDiferenciasFiltros` | Condiciones y filtros · Filtros de fichero | Vista principal | CU-A80, CU-A81, CU-A82, CU-A83 |
| 34 | `DialogoAnalisisMultipleDeTramosFrmViewModel` | Desde TramificarForm | Análisis · Tramificar | Hoja modal | CU-A84, CU-A85, CU-A86 |
| 35 | `DialogoFiltrarPorLimitesFrmViewModel` | Botón «Filtrar» de TramificarForm | Análisis · Tramificar | Hoja modal | CU-A87 |
| 36 | `DialogoGrabarBancoPruebasFrmViewModel` | Botón «Grabar» del Banco de Pruebas | Herramientas · Simulador de escrutinios | Hoja modal | — (parte de CU-A24) |
| 37 | `DialogoGrabarTramosFrmViewModel` | Botón «Grabar tramos» de TramificarForm | Análisis · Tramificar | Hoja modal | — (`ExportarTramosSeleccionados`, propiedad de Tramificar; ESPEC_09/10) |
| 38 | `DibFormViewModel` | «Mostrar resultados» de Anastatics (modo 0) | Análisis · Estadísticas de fichero | Panel de detalle | CU-A88 |
| 39 | `DibRepFrmViewModel` | «Mostrar resultados» de Anastatics (modo 1) | Análisis · Estadísticas de fichero | Panel de detalle | CU-A89 |
| 40 | `DibujosFrmViewModel` | Botón «Dibujos» de la rejilla de Inicio | Condiciones y filtros · Dibujos | Inspector | CU-A90 |

### 3.2 Recuento por área Mac propuesta

| Área | Pantallas | Casos de uso |
|---|---:|---:|
| Herramientas | 5 | 13 |
| Boleto | 1 | 1 |
| Análisis | 12 | 19 |
| Cálculo | 3 | 8 |
| Condiciones y filtros | 10 | 27 |
| Columnas probables | 5 | 14 |
| Escrutinio | 1 | 1 |
| Ajustes | 2 | 5 |
| Online | 1 | 2 |
| **Total** | **40** | **90** |

### 3.3 Catálogo de casos de uso

Firma lógica de cada caso de uso (entradas y salidas detalladas en la sección de su pantalla). Todos son funciones puras o `async` cancelables sobre valores explícitos (`Sesion`, `Configuracion`), sin acceso a UI (M-R4, T10, T12).

| ID | Caso de uso | Pantalla | Qué hace |
|---|---|---:|---|
| CU-A01 | `AgregarPleno15` | 1 | Convierte un fichero de columnas de 14 signos a 15 (o lo inverso) con 4 modos de Pleno al 15; orden de salida por bloques de 8 por signo. |
| CU-A02 | `AnadirEquipo` | 2 | Alta de un equipo (nombre + categoría) en el `.dat` de su categoría. |
| CU-A03 | `AlgebraDeColumnas` | 3 | Operaciones de conjuntos sobre ficheros de columnas (eliminar repetidas, unir, intersectar, restar) con `SumadorCombinaciones`. |
| CU-A04 | `AnalizarGruposEnFichero` | 4 | Cuenta columnas por patrón de grupo (primeros N partidos) sobre un fichero, con filtro de fallos; devuelve filas ordenadas por índice base 3. |
| CU-A05 | `GrabarGruposSeleccionados` | 4 | Relee el fichero y escribe las columnas cuyo grupo está marcado. |
| CU-A06 | `TraducirColumnaARangos` | 5 | Traduce una columna a notación de rango (1 = signo más valorado) según la valoración 1/X/2. |
| CU-A07 | `ContarAparicionesFormato123` | 5 | Cuenta apariciones de cada formato predefinido en las columnas del fichero. |
| CU-A08 | `ListarTodosFormatos123` | 5 | Lista todas las subsecuencias (longitud 1..N) con su cuenta, conservando la primera aparición. |
| CU-A09 | `ClasificarColumnasPorSumaPares` | 6 | Puntúa cada columna con la suma de valores de sus 7 pares consecutivos y clasifica las columnas únicas en 36 casillas de suma (0..35). |
| CU-A10 | `GrabarCasillasSuma` | 6 | Escribe las columnas de las casillas de suma marcadas. |
| CU-A11 | `ContarPremiosPorCasillas` | 6 | Para una ganadora, cuenta las columnas a distancia ≤ 4 que caen en casillas marcadas (premios 14..10). |
| CU-A12 | `AnalizarFallosCombinacion` | 7 | Evalúa una columna contra el analizador y devuelve un árbol de nodos con estado Verde/Ámbar/Rojo. |
| CU-A13 | `EvaluarPronosticosBase` | 7 | Comprueba la columna contra los pronósticos del boleto; devuelve los partidos fallados. |
| CU-A14 | `AnalizarFicheroDeColumnas` | 8 | Crea un `Analizador` con N partidos desde un fichero y devuelve el contenedor de análisis. |
| CU-A15 | `CalcularEstadisticasDeFichero` | 9 | Una de 4 estadísticas (dibujos, coincidencias con la columna anterior, interrupciones, signos seguidos) en una matriz 15×15. |
| CU-A16 | `ObtenerValoracionRealDesdeApostada` | 10 | Desde los porcentajes apostados calcula la matriz «reales» (14×3 en %) con las `NumColumnasAConsiderar` columnas más próximas a LN (quicksort propio no estable). |
| CU-A17 | `GenerarColumnasAleatorias14` | 10 | Genera n columnas de 14 al azar según «reales» y acumula media y desviación típica de LN. |
| CU-A18 | `CalcularPremiosJornadaVirtual` | 10 | Premios de las categorías 14..10 de una columna 14 virtual con el modelo de reparto del L.A.E. (porcentajes de premio × 0.5 / probabilidad). |
| CU-A19 | `EscrutarCombinacionBanco` | 10 | Modo «Combinación»: escruta cada aleatoria contra las columnas del fichero; veces, premios, beneficio y recuperación. |
| CU-A20 | `EscrutarColumnasBanco` | 10 | Modo «Columnas»: igual, pero acumula veces y premios por cada columna del fichero. |
| CU-A21 | `AutoescrutinioBanco` | 10 | Modo «Autoescrutinio»: cada columna del fichero actúa de 14 y se escruta contra todo el fichero. |
| CU-A22 | `EscrutarPorJornadas` | 10 | Modo «Jornadas»: una aleatoria por jornada, con saldo acumulado. |
| CU-A23 | `SimplificarFilasPorDiferencias` | 10 | Deselecciona las filas que están a más de `dif` diferencias de la fila de referencia (`Deselecciona`). |
| CU-A24 | `GrabarColumnasSeleccionadas` | 10 | Escribe a `.txt` el rango de filas elegido, con tope y opción «solo seleccionadas» del diálogo de la pantalla 36. |
| CU-A25 | `PrepararBoletos` | 11 | Lee el fichero de columnas, lo ordena y lo trocea en boletos de 8 apuestas. |
| CU-A26 | `OrdenarColumnasPorCriterio` | 11 | `OrdenarMatrizColumnas(cols, orden, tipo)`: ordena las columnas del boleto según criterio y tipo. |
| CU-A27 | `BuscarLimitesValoracion` | 12 | Recorre las 3^N columnas; para las que coinciden con la columna base en un nº de aciertos dentro del rango, devuelve mínimo y máximo de la suma y del producto de valoraciones. |
| CU-A28 | `CalcularCombinacion` | 13 | Ejecuta el motor sobre la combinación en pantalla en uno de tres modos: contar, contar y analizar, o grabar a fichero. |
| CU-A29 | `ValidarArchivoResultados` | 13 | Modo grabar: error si el archivo está vacío o coincide con un archivo ya usado en la combinación (`HayConflictos`). |
| CU-A30 | `EstimarColumnasMaximas` | 13 | Máximo de columnas: líneas del fichero de columnas base o 2^dobles · 3^triples de los pronósticos. |
| CU-A31 | `EstadisticasDeCalculo` | 13 | Procesadas, aceptadas, % de aceptación, estimadas y costes (importe × `PrecioApuesta`) y progreso 0–100. |
| CU-A32 | `CalcularLoteDeCombinaciones` | 14 | Calcula en lote varias combinaciones `.comb`/`.xml`, una salida `.txt` por fichero; devuelve un resumen por combinación. |
| CU-A33 | `NombreSalidaDeCombinacion` | 14 | Nombre del `.txt` de salida: sustituye `.comb` y `.xml` por `.txt` y lo coloca en la carpeta de destino. |
| CU-A34 | `ExtraerFormatosDeColumna` | 15 | Extrae pares, tríos, cuartetos, quintetos y contactos de una columna (con capacidades 9/12/11/10). |
| CU-A35 | `FormatearInformeFormatos` | 15 | Da formato de texto al informe de formatos. |
| CU-A36 | `LeerPuntosCP` | 16 | Lee los puntos CP (fijos, dobles, triples) de `parametros.free1x2`. |
| CU-A37 | `GuardarPuntosCP` | 16 | Reescribe los puntos CP en `parametros.free1x2`. |
| CU-A38 | `CalcularRadiografiaColumna` | 17 | Radiografía de 108 características de una columna (`Calcondis`: VX2, SS, distancias, contactos, PN, interrupciones). |
| CU-A39 | `ParsearCondicionCoincidencias` | 17 | Interpreta las filas «grupos;rango» (`.cnd`) como condiciones (límite 100). |
| CU-A40 | `FiltrarPorCoincidencias` | 17 | Marca las columnas del fichero cuya radiografía coincide con la de la ganadora anterior dentro de los rangos de las condiciones. |
| CU-A41 | `AnalizarCoincidenciasEntreGanadoras` | 17 | Distribución de coincidencias entre ganadora anterior y reciente a lo largo de una lista de ganadoras (límite 3000). |
| CU-A42 | `ExportarDistribuciones` | 17 | Exporta resultados, radiografía y distribuciones a texto tabulado. |
| CU-A43 | `PrepararAnalisisDeFallos` | 18 | Valida la columna ganadora y reúne archivo, columna (3 bits), analizador y pronósticos para la pantalla 7. |
| CU-A44 | `EditarColumnasProbables` | 19 | Editor con buffer del filtro CP: carga, guarda la CP en edición y confirma sobre el grupo. |
| CU-A45 | `NavegarColumnaProbable` | 19 | Cambia la CP seleccionada guardando el buffer y añadiendo una CP vacía si hace falta. |
| CU-A46 | `CopiarRangosAColumnas` | 19 | Copia los datos de la CP actual a las CP Desde..Hasta (parámetros de la pantalla 30). |
| CU-A47 | `FusionarColumnasImportadas` | 19 | Incorpora CP importadas con modo sustituir, añadir o reemplazar. |
| CU-A48 | `EditarRelacionesCP1` | 19 | Editor de Relaciones I (`RelacionCP1`). |
| CU-A49 | `EditarRelacionesCP2` | 19 | Editor de Relaciones II (`RelacionCP2`). |
| CU-A50 | `EditarRelacionesCP3` | 19 | Editor de Relaciones III (`RelacionCP3`). |
| CU-A51 | `EditarControlFallosCP` | 19 | Editor del control de fallos CP. |
| CU-A52 | `ExportarColumnasPremiadas` | 20 | Exporta todas o las seleccionadas de las columnas premiadas. |
| CU-A53 | `CombinarFiltrosContar` | 21 | Cuenta columnas combinando listas de filtros de fichero. |
| CU-A54 | `SumarColumnasPorRango` | 21 | Suma de columnas por rango de la combinación de filtros. |
| CU-A55 | `GrabarColumnasPorRango` | 21 | Graba las columnas de un rango de la combinación. |
| CU-A56 | `GuardarListaFiltros` | 21 | Guarda la lista de filtros a fichero. |
| CU-A57 | `CargarListaFiltros` | 21 | Carga la lista de filtros desde fichero. |
| CU-A58 | `ComprimirColumnasAZ3q` | 22 | Comprime un `.txt` de columnas a `.z3q` (ZIP con una entrada Deflated; bitset LSB-first; deduplica y ordena). |
| CU-A59 | `DescomprimirZ3qAColumnas` | 22 | Descomprime un `.z3q` a `.txt` de columnas. |
| CU-A60 | `LeerConfiguracionGeneradorCP` | 23 | Lee las filas de «Configuracion de CPs» por tipo desde `ColumnasProbables.xml`. |
| CU-A61 | `GuardarConfiguracionGeneradorCP` | 23 | Escribe Desde, Hasta, Num* y Forzar* sobre las filas existentes del tipo. |
| CU-A62 | `LeerOpcionesAnalisis` | 24 | Lee los 20 booleanos de análisis de `parametros.free1x2`. |
| CU-A63 | `GuardarOpcionesAnalisis` | 24 | Escribe los 20 booleanos y llama a `ReinicializarVariables()`. |
| CU-A64 | `LeerConfiguracion` | 25 | Lee boleto, desplazamiento, puntuación CP, separador JB, L.A.E., actualizador, idioma y confirmación al salir. |
| CU-A65 | `GuardarConfiguracion` | 25 | Valida y escribe la configuración y reinicializa las variables globales. |
| CU-A66 | `ValidarSeparadorJB` | 25 | Comprueba que los 6 límites del separador % JB son estrictamente crecientes; si no, error y no guarda nada. |
| CU-A67 | `EditarCondicionContactos` | 26 | Editor con buffer de la condición Contactos (10 parejas + figuras). |
| CU-A68 | `AplicarCondicion` | 26 | Patrón PC (Aceptar): vuelca el buffer al filtro del grupo, `ActivaFiltro` y notifica. |
| CU-A69 | `ExportarCondicion` | 26 | Patrón PC (Guardar/Copiar): escribe la condición a fichero. |
| CU-A70 | `ImportarCondicion` | 26 | Patrón PC (Abrir/Pegar): lee la condición de fichero al buffer. |
| CU-A71 | `CalcularEstadisticasCondicion` | 26 | Patrón PC (Estadísticas): estadísticas de un filtro temporal sobre las ganadoras de `Ganadoras/`. |
| CU-A72 | `EditarControlGrupos` | 27 | Editor del control de grupos (buffer, cálculo de «libres»). |
| CU-A73 | `EditarControlConjuntos` | 27 | Editor del control de conjuntos. |
| CU-A74 | `EditarTolerancias` | 28 | Editor de tolerancias del grupo (filas dinámicas + fallos). |
| CU-A75 | `CopiarColumnasProbablesAGrupos` | 29 | Copia CP seleccionadas a otros grupos. |
| CU-A76 | `ParsearIndices` | 29 | Interpreta una lista de índices CP (`a,b,c` o rango `a-b`, el primer separador hallado). |
| CU-A77 | `CrearGrupos` | 31 | Crea n grupos nuevos con la lista fija de 14 partidos activos (`PonerPartidosActivos`). |
| CU-A78 | `ActualizarJornadaOnline` | 32 | Descarga la jornada del país elegido, con caché offline-first, y fija `AppState.JornadaActual`. |
| CU-A79 | `ResolverPaisInicial` | 32 | País inicial del selector: el de la jornada cacheada más reciente (`JornadaCache.PaisMasReciente()`) o España. |
| CU-A80 | `DiferenciaEntreFiltros` | 33 | Elimina de la base las columnas con algún vecino a distancia 0–3 en el filtro, según máscara de niveles y mínimo. |
| CU-A81 | `EscrutarGanadoraSobreResultado` | 33 | Cuenta cuántos 14 y 13 de una ganadora hay en el resultado. |
| CU-A82 | `GuardarListaDiferencias` | 33 | Guarda la lista de filtros (`;` como separador, 5 campos) en `.lst`. |
| CU-A83 | `CargarListaDiferencias` | 33 | Carga la lista de filtros desde `.lst` (ignora líneas con menos de 5 campos). |
| CU-A84 | `LeerTemporadasDeValoraciones` | 34 | Lee las temporadas del fichero de valoraciones históricas (TSV de 44 columnas). |
| CU-A85 | `ValidarFicheroDeColumnas` | 34 | Comprueba que la primera línea tiene ≥ 14 caracteres en `1xX2`. |
| CU-A86 | `GestionarListaCombinacionesPorJornada` | 34 | Alta, baja y lectura/escritura `.lst` de la lista temporada-jornada-ruta. |
| CU-A87 | `FiltrarTramosPorLimites` | 35 | Coherencia de extremos 10×4 y eliminación de columnas por tramo y diferencias. |
| CU-A88 | `FormatearMatrizDibujos` | 38 | Presenta la matriz 15×15 de dibujos como porcentajes o recuentos con totales. |
| CU-A89 | `FormatearMatrizCoincidencias` | 39 | Presenta la matriz 5×15 de coincidencias como porcentajes o recuentos. |
| CU-A90 | `EditarCondicionDibujos` | 40 | Editor con buffer de la condición Dibujos (rejilla triangular de casillas). |

---

## 4. Abiertas

Dudas, defectos aparentes y puntos no verificados. Ninguno se resuelve aquí: los de tipo «divergencia» exigen decisión del dueño (R2) y los de tipo «no verificado» no se afirman (R4). Cada uno remite a la sección de la pantalla donde está la evidencia.

| ID | Pantalla | Asunto | Tipo | Detalle y evidencia |
|---|---:|---|---|---|
| A1 | 1 | Combinaciones de modos de Pleno al 15 | Decisión | Los modos 2, 3 y 4 se ejecutan en secuencia sobre la misma `col` y pueden duplicar o corromper líneas (p. ej. 2+4 añade un 16.º carácter). Preservar tal cual o restringir a un único modo (`P:AgregaP15FrmViewModel.cs:194-353`). |
| A2 | 4 | `Grabar` de Análisis de grupos | Divergencia | El port relee el fichero y escribe las columnas completas cuyo grupo está marcado, conservando duplicados; el WinForms recorría los 3^14 índices y escribía columnas únicas de 14 signos filtradas por `valides` (`P:AnaCombiViewModel.cs:309-363` frente a `L:UI/AnaCombi.cs:93-123`). Decidir cuál es la referencia. |
| A3 | 5 | Índices fuera de rango en Formatos 123 | Defecto aparente | Con columnas de 15–16 signos y rejilla de `NumeroPartidos` filas se indexa `val[i,…]` fuera de rango sin capturar la excepción (`P:AnalisisFormatos123FrmViewModel.cs:142-346`). En Swift validar longitud. |
| A4 | 7 | Estado de «Pronóstico» pisado en el árbol de fallos | Defecto aparente | Al procesar un filtro con fallos el port asigna `P=A`, lo que borra un `F` previo del nodo padre `P` (`P:AnalizarCombinacionFrmViewModel.cs:78-561`; compárese con `L:Analisis/AnalisisCombinacion.cs:95-330`). Decidir si se replica. |
| A5 | 10 | Banco de pruebas: LN↔Premio y defecto de LN | Divergencia | El original sincroniza `LN=ln(0.5*0.12/Premio)` y `Premio=Round(0.5*0.12/exp(LN),0)` (`L:UI/BancoPruebasFrm.cs:4082-4108`), lee el fichero de aleatorias de otra forma (`:3258-3262`) y tiene LN por defecto −14,75 (`:543`); el port usa −14.7 y el cálculo lee siempre `LnProbMedia14` (`P:BancoPruebasFrmViewModel.cs:337`): `UsarLN` solo conmuta la UI (`:147,277`) y `PremioMedio14` (`:153`) no se lee. |
| A6 | 16 | ¿Relee algún consumidor `VariablesGlobales.Puntos*` tras guardar? | No verificado | `GuardarPuntosCP` escribe `parametros.free1x2` pero no refresca `VariablesGlobales` (`D:VariablesGlobales.cs:90` solo en el arranque). No se ha comprobado si algún consumidor lo lee después sin reiniciar (ESPEC_01/04). |
| A7 | 17 | Peculiaridades de Coincidencias | Decisión | (1) `Calcular` usa la ganadora actual del visor, no la última; (2) ignora la ganadora reciente; (3) `agrabarCOL` guarda `nc` de la última condición; (4) límite de 100 condiciones y 3000 ganadoras; (5) el combo AMC solo ofrece «1» (el original tenía botones +/−; no verificado en el port) (`P:CoincidenciasViewModel.cs:247-1111`). |
| A8 | 8 | `IncluirPleno` sin efecto | Defecto aparente | El interruptor «Incluir pleno al 15» se muestra pero su valor no se lee en `Analizar` (`P:AnalizarFicheroFrmViewModel.cs:99-135`). Decidir si se omite en Mac. |
| A9 | 10 | `DesviacionTipicaDeseada` y `LnMinimo` sin uso | Código muerto | Declaradas con defectos 1.965561 y −11 y nunca leídas (`P:BancoPruebasFrmViewModel.cs:172,175`). |
| A10 | 19 | Guardado de pronósticos CP pierde dobles/triples | Defecto aparente | `GuardaDatosCP` guarda un único carácter por partido; la `TextBox` del port no admite 2 o 3 signos (`P:ColProbablesFrmViewModel.cs:424-434`). La spec Swift debe admitir 1, 2 o 3 signos por partido. |
| A11 | 19 | Parseo de tolerancias/aciertos | Defecto aparente | En `SetNoAciertos` un rango «0-14» lanza `FormatException` (`Convert.ToInt32`) y un token vacío (p. ej. coma final) lanza `IndexOutOfRangeException`; el campo entero vacío se sustituye por «todos» (el WinForms usaba controles numéricos). Definir validación previa (`P:ColProbablesFrmViewModel.cs:443-444`, `D:MotorCalculo/ColumnaProbable.cs:376-400`). |
| A12 | 21 | `N2s` con ≠ 14 partidos | Defecto aparente | Escribe columnas de 14 y un filtro con columnas más largas que `partidosEnJuego` desborda `htCols` (`P:CombinarFiltrosViewModel.cs:122,137-138,297`). No replicar el defecto sin decisión. |
| A13 | 22 | Compatibilidad ZIP de `.z3q` | No verificado | Hace falta un ZIP-deflate compatible (SharpZipLib 1.4.x, sin cifrado); verificar con ficheros reales (`P:CompresorViewModel.cs:127-137`, `D:Utils/CompresorZip.cs:20-110`). |
| A14 | 34 | Ruta en mayúsculas al leer `.lst` | Defecto aparente | `LeeColumnaSinComas` aplica `Trim + ToUpper` y la ruta queda en mayúsculas; en un sistema de ficheros que distinga mayúsculas la ruta leída no coincidiría con la guardada (`P:DialogoAnalisisMultipleDeTramosFrmViewModel.cs:377-396`). |
| A15 | 34, 37 | Diálogos sin consumidor | Defecto aparente | Ningún suscriptor de `CierreSolicitado` ni consumidor de las propiedades de salida; el diálogo no se cierra ni entrega datos (`CierreSolicitado` se declara en `P:DialogoAnalisisMultipleDeTramosFrmViewModel.cs:187` y se dispara en `:449,:459`; en `P:DialogoGrabarTramosFrmViewModel.cs:68,83`; a diferencia del diálogo del Banco, que sí lo escucha en `P:BancoPruebasFrmPage.xaml.cs:48-59`; `P:TramificarFormViewModel.cs:856` usa `Paso=1`). En Swift modelar la hoja con valor de retorno. |
| A16 | 35 | Fila 10 de «Filtrar por límites» | Defecto aparente | Por la réplica del original la fila 10 (`e[9]`) nunca usa sus propios extremos y coincide con la fila 9. Preservar o corregir con aprobación (`P:DialogoFiltrarPorLimitesFrmViewModel.cs:86-158`; consumidor `P:TramificarFormViewModel.cs:755-770`). |
| A17 | 40 | Nº de partidos de la rejilla Dibujos | Divergencia | El port fija `NumPartidos = 15` (136 casillas) frente al WinForms que usa `NumeroPartidos` (14 ⇒ 120 casillas) (`P:DibujosFrmViewModel.cs:63`, `L:UI/Controls/GridDibujosCentral.cs:13-27`). La spec Swift usa `Configuracion.numeroPartidos`. |
| A18 | 26, 34, 40 | Formato exacto de ficheros de condición y de valoraciones históricas | No verificado | Los formatos de `.cont`, `.dbj`, `.xml` (`ArchivoCondiciones`) y el TSV de 44 columnas de valoraciones históricas no se describen aquí: se remiten a ESPEC_07 y no se han cotejado aquí. |
| A19 | 10 | `System.Random` y reproducibilidad | No verificado | La semilla es de reloj (`unchecked((int)DateTime.Now.Ticks)`, `P:BancoPruebasFrmViewModel.cs:368`), por lo que la salida no es reproducible. Para tests golden con semilla fija habría que replicar el algoritmo de `System.Random(seed)` de .NET 8 (BCL, no leído en este repo). |
| A20 | todas | Carpetas de datos, «portapapeles» de condiciones y ficheros temporales en Mac | Decisión | Hoy todo vive junto al `.exe` (`Equipos/`, `Columnas/`, `Temp/`, `Ganadoras/`, `ColumnasProbables.xml`, `parametros.free1x2`). En Mac el bundle no es escribible (PLAN §2): decidir `Application Support/Free1X2/…` con copia inicial desde el bundle, y si Copiar/Pegar de condiciones usa `NSPasteboard` o conserva el fichero `Temp/tmp.<ext>` (§1.6, §1.7). |
| A21 | 5 | Productor de `ArrayFormatos` | No verificado | La lista de formatos predefinidos llega desde `Formatos123Frm` por un estático (`P:AnalisisFormatos123FrmPage.xaml.cs:33`); esa pantalla está fuera de este alcance (ESPEC_09/10) y no se ha leído aquí cómo se construye `ArrayFormatos`. |
| A22 | 18 | Rama «en pantalla» inalcanzable | Defecto aparente | `ColGanadoraContexto` no la construye ningún código del proyecto WinUI; abierta desde la barra, «Analizar combinación en pantalla» siempre falla con «No hay una combinación en pantalla disponible…» (`P:ColGanadoraFrmViewModel.cs:102-166`, `P:ColGanadoraFrmPage.xaml.cs:32-40`). En Mac debe alimentarse con la `Sesion`. |
