# Tareas pendientes en Windows (WinUI 3)

Todo lo que la app Mac (`Free1X2.Mac/`) añadió o rediseñó el 16-oct-2026 y que el WinUI 3
(`Free1X2.WinUI/` + `Free1X2.Domain/`) todavía no tiene. El dueño pidió que valga para **Mac y
Windows**. No se tocó el C# desde el Mac porque allí no se puede compilar ni ejecutar el WinUI, y
ejecutar el C# está fuera de lo permitido en esa máquina. Cada tarea se hace en el PC Windows.

**Cómo leer cada tarea:** *Dónde* cita el WinUI actual (fichero:línea, leído el 16-oct-2026);
*Referencia Mac* es la implementación ya hecha y probada, que sirve de modelo; la decisión completa
está en `docs/mac/DECISIONES.md` (DT-xx).

**Reglas** (`docs/PLAN_MEJORAS.md` §0):

- **R1:** el motor no cambia de resultados. Siguen pasando los **138 tests** de
  `Free1X2.Domain.Tests` sin tocarlos. Las tareas nuevas añaden tests propios.
- **R2:** las decisiones de UI de abajo ya están **aprobadas por el dueño** (16-oct-2026).
- **R3:** una tarea está hecha solo con build sin errores, los tests en verde, el smoke completo y la
  cita `fichero:línea` del cambio.

**Orden sugerido:**

1. Correcciones pequeñas: W1–W5.
2. Jornada: W6–W7.
3. Navegación: W8.
4. Máscaras y columnas de Inicio: W9–W10, que comparten la generación de columnas.

---

## W1 · Ayuda de Variantes, Signos seguidos y Distancias: «comas», no «espacios» (D-01, DT-78)

- **Qué:** la ayuda dice «Separa varios valores con comas o espacios» y pone `0 1 2` de ejemplo, pero
  el parser solo acepta comas (DEC-904). Con espacios el usuario recibe «La cadena de entrada no tiene
  el formato correcto».
- **Dónde:**
  - `Views/Ported/NoVariantesFrmPage.xaml:21`
  - `Views/Ported/SignosSeguidosFrmPage.xaml:21`
  - `Views/Ported/DistanciasFrmPage.xaml:21`; los `PlaceholderText="p. ej. 0 1 2"` en `:35` y `:50`.
- **Cómo:** el texto pasa a «Separa varios valores con comas (p. ej. 0,1,2).» y los placeholders a
  `p. ej. 0,1,2`. **No** se toca el parser.
- **Aceptación:** los tres textos corregidos; los 138 tests sin cambios.

## W2 · Un valor no válido en una condición no debe dejarla activa y vacía (D-04, DT-80)

- **Qué:** con un valor no válido (p. ej. Variantes «a»), sale el error, pero el filtro queda
  **reinicializado y activo**, y Calcular da 0 columnas sin avisar.
- **Dónde:** `Views/Ported/NoVariantesFrmViewModel.cs:92-104` llama a `filtro.ReinicializaValores()` y
  pone `filtro.IsActive = true` **antes** de `SetNoVariantes/SetNoEquis/SetNoDoses`, que son los que
  lanzan. El mismo patrón (`ReinicializaValores();`) aparece en `ValoracionFrmViewModel.cs`,
  `DistanciasFrmViewModel.cs`, `SignosSeguidosFrmViewModel.cs`, `ColProbablesFrmViewModel.cs`,
  `PesosNumFrmViewModel.cs` e `InterrupcionesFrmViewModel.cs`. Revisar cada uno.
- **Cómo:** aplicar sobre una **copia** del filtro (o validar antes todas las cadenas con
  `RangosHelper`) y copiar al grupo solo si nada lanzó. Con un error, el filtro queda como estaba.
- **Referencia Mac:** `Combinacion.modificarGrupo` es atómico
  (`Packages/Free1X2Kit/Sources/Free1X2Motor/Configuracion/Combinacion.swift:83-92`).
- **Aceptación:** con «a» en Variantes, la condición no se activa y Calcular sigue dando las columnas
  de antes. Cambia solo el camino de error: los 138 tests no cambian.

## W3 · Rotación de signos: recuperar los paneles del WinForms (C2, DT-79)

- **Qué:** faltan los paneles «% COMBINACIÓN» y «Valoraciones JORNADA» (con «Abrir valoración…»), la
  propuesta automática y «Transposición automática».
- **Dónde:** `Views/Ported/RotacionDeSignosFrmPage.xaml` y `RotacionDeSignosFrmViewModel.cs`. Fuente
  legacy: `Free1X2/UI/RotacionDeSignosFrm.cs:1860-1931,2190-2292`.
- **Cómo:**
  - El X y el 2 se redondean a par y el 1 completa 100.
  - La propuesta es `PropuestaAutomatica` + `Ordenar3valores`.
  - La transposición automática ordena por intercambio exacto, con orden 0-based, y pide confirmación
    con el orden y el fichero. La salida pasa a ser la entrada.
- **Referencia Mac:** `CasosUtilidades.porcentajesRotacion`, `propuestaRotacion`,
  `TransponedorAutomatico.trasposicion` y la pantalla `App/Pantallas/Utilidades/Rotacion.swift`.
- **Aceptación:** mismos casos que `Tests/Free1X2MotorTests/TransponedorAutomaticoTests.swift`.
  Empate: orden `[12,13,8,9,10,11,0,1,2,3,4,5,6,7]`.

## W4 · Diferencias entre columnas: «CPs por diferencias…» debe abrir el generador (C3, DT-79)

- **Dónde:** `Views/Ported/DifColsViewModel.cs:248-256`: `AbrirCpsPorDiferencias()` está **vacío**.
  Legacy: `Free1X2/UI/DifCols.cs:645`.
- **Cómo:** navegar a `GeneradorCPSDiferenciasPage` (ya portada en `Views/Ported/`).
- **Aceptación:** el botón abre el generador.

## W5 · Aidomnou: quitar la nota de `aidomnou.cfg` (C5, DT-79)

- **Dónde:** `Views/Ported/aidomnouPage.xaml:208-212` promete «…aparecen editables en el fichero
  'aidomnou.cfg'», y ese fichero no se crea.
- **Cómo:** quitar la nota, o sustituirla por cómo se guardan los límites («Salvar Límites»).

---

## W6 · Inicio: botón «Jornada ▾ España / México» que rellena los equipos (DT-81)

- **Qué:** con un clic, los 14 partidos (y el 15.º con el pleno si el boleto tiene 15) toman local y
  visitante de la jornada actual de clubprogol. Sin red, se usa la jornada guardada.
- **Dónde:**
  - La descarga ya existe: `DescargaBoletoFrmViewModel.cs:88-95` publica en
    `AppState.Instancia.JornadaActual`, pero **no** rellena el boleto de Inicio.
  - `Controls/BoletoViewModel.cs:96` (`AplicarJornadaActual`) sí lo hace, pero Inicio usa
    `Controls/BoletoBaseViewModel.cs:181`, que no lo tiene.
- **Cómo:**
  1. Añadir a `BoletoBaseViewModel` un `AplicarJornada(JornadaQuiniela)` que componga
     «Local - Visitante» (vacío ⇒ `? - ?`) para los N partidos y use `pleno15` para el 15.º.
  2. Botón con menú en Inicio (ver W8) que llame a `ObtenerJornadaAsync` y luego a
     `AplicarJornada`, y avise con el mensaje del servicio.
  3. «Descarga de boleto» también aplica los nombres al boleto, como dice su texto.
- **Referencia Mac:** `CasosArchivo.equiposDeJornada`, `Sesion.cargarJornada`.
- **Aceptación:** con `real-es.json` (fixture del repo):
  - partido 1 = «Estados Unidos - Australia»;
  - pleno = «España - Arabia Saudi»;
  - México (`real-mx.json`): 14 partidos y sin pleno.

## W7 · Cargar la jornada al arrancar, 3 s como máximo (DT-83)

- **Qué:** al abrir la app se descarga la jornada actual del país más reciente de la caché (España si
  no hay).
  - Si llega en **3 s**, rellena los equipos.
  - Si tarda, falla o no hay red, el boleto queda **en blanco, sin aviso y sin usar la caché**.
  - No pisa lo que el usuario haya escrito o abierto entretanto.
- **Dónde:** `App.xaml.cs:58` (`SembrarJornadaDesdeCache()`, solo la caché). El `HttpClient` tiene 10 s
  de timeout (`Services/QuinielaOnlineService.cs:100-101`).
- **Cómo:** tras crear la ventana, una tarea en segundo plano con
  `CancellationTokenSource(TimeSpan.FromSeconds(3))` que llama a `ObtenerJornadaAsync(pais, token)`.
  En éxito, `AplicarJornada` (W6) en el hilo de UI, solo si los equipos siguen vacíos. Cualquier
  excepción se ignora (solo log).
- **Referencia Mac:** `CasosArchivo.jornadaAlArrancar` (carrera descarga/reloj), `Raiz.swift` (`.task`).
- **Aceptación:** con red, el boleto aparece con la jornada. Con la API apuntando a un puerto cerrado
  (`FREE1X2_API_BASE=http://127.0.0.1:9`), queda en blanco y sin diálogo en menos de 3 s.

## W8 · Navegación: panel lateral con nombres + barra con texto, en lugar de los 55 iconos (DT-82)

- **Qué (aprobado por el dueño):**
  - Los 55 iconos solo decían su función en un tooltip.
  - Se cambian por un **panel lateral** con las pantallas por nombre, agrupadas, y con **buscador**
    que no distingue tildes ni mayúsculas.
  - Arriba va una **barra** con las acciones frecuentes **con texto**. Cada pantalla añade las suyas.
- **Dónde:** `MainWindow.xaml:38` (`ToolbarWrapPanel x:Name="ToolbarPanel"`) y
  `MainWindow.xaml.cs:343-433` (grupos y botones). En Inicio, la tarjeta «Combinación»
  (`Views/MainPage.xaml:276-302`: Calcular, Reducir, Nueva, Abrir, Guardar).
- **Cómo:**
  1. `NavigationView` nativo de WinUI (`PaneDisplayMode="Left"`, plegable) con `AutoSuggestBox` para
     buscar.
  2. Grupos con `NavigationViewItemHeader`; las **54 entradas** son exactamente las de
     `Free1X2.Mac/App/Navegacion/Lateral.swift` (Inicio; Combinación 17 con Máscaras; Filtros 5;
     Operaciones 5; Utilidades 18; Archivo 3; Ajustes y ayuda 5). Con icono, el PNG de
     `Assets/Toolbar`.
  3. Pulsar una entrada la pone como **raíz** de la navegación (no apila).
  4. `CommandBar` con `AppBarButton` y `Label` visible:
     - Nueva, Abrir y Guardar (pueden ir solo con icono y tooltip si no cabe) y Escrutinio, en todas
       las pantallas;
     - en Inicio además Calcular, Reducir, Jornada ▾ (W6) y Máscaras ▾ (W9).
  5. La tarjeta «Combinación» de Inicio desaparece: el nombre del fichero va en la cabecera del
     Boleto Base.
  6. Se quita Ver ▸ Barras de herramientas. **`parametros.free1x2` conserva sus valores de barras sin
     tocarlos** (compatibilidad).
- **Aceptación:**
  - Buscar «algebra» deja solo «Álgebra de columnas» y la abre.
  - A 1024 px de ancho se ven todos los botones de la barra.
  - El smoke recorre todas las páginas.

---

## W9 · Máscaras: guardar, aplicar y escrutar contra el histórico (DT-84)

- **Qué (aprobado por el dueño):**
  - Una **máscara** son los **signos de cada partido** del boleto, con nombre y sin equipos.
  - Se guarda desde Inicio y se aplica a la jornada cargada: pone los signos y deja los equipos.
  - En la pantalla **«Máscaras»** se listan, aplican y borran, y se **escrutan contra las ganadoras
    históricas** que da clubprogol. Por cada máscara, la suma de columnas con **14, 13, 12, 11 y
    10** aciertos en todas las jornadas y las jornadas con premio, en una tabla **ordenable** por
    cualquier columna.
- **Formato compartido Mac/Windows** (no cambiarlo): `Mascaras/<nombre>.mas`, en la carpeta de datos
  (junto a `Equipos/`).
  - UTF-8 con **una línea por partido** con su pronóstico separado por comas (`1`, `1,X`, `1,X,2`) y
    CRLF.
  - El nombre del fichero es el de la máscara (sin `/ \ : * ? " < > |`).
  - Al listar se ignoran los que no tienen N líneas válidas.
- **Recuento sin desarrollar columnas** (igual de exacto y mucho más rápido): por cada ganadora,
  empezar con `c = [1]`. En cada partido, si el signo ganador está en el pronóstico:
  `aciertos = 1, fallos = n−1`; si no: `aciertos = 0, fallos = n`. Luego
  `nuevo[k] += c[k]·fallos` y `nuevo[k+1] += c[k]·aciertos`. `c[k]` es el número de columnas con k
  aciertos. Se usan los 14 primeros partidos.
- **Histórico propio (DT-87):** botón «Escrutar con un archivo de ganadoras…». Lee el archivo de columnas de siempre (una
  ganadora por línea): se saltan las líneas en blanco y cuentan los 14 primeros signos. Una línea que no empiece por 14
  signos 1/X/2 detiene la lectura con su número.
- **Histórico:** endpoint `GET /wp-json/clubprogol/v1/quiniela/{pais}/historico`, ya en producción. El
  contrato está en `docs/API_CLUBPROGOL.md` §4. **El dueño lo crea en el servidor.** Validación
  estricta: una ganadora sin 14 signos 1/X/2 rechaza la respuesta.
- **Dónde:** nuevo.
  - `Free1X2.Domain/Online/` (parser del histórico + modelo).
  - `Free1X2.Domain/` (casos de máscaras).
  - `Services/QuinielaOnlineService.cs` (`ObtenerHistoricoAsync`).
  - Página nueva `MascarasPage` registrada en `Navigation/PortedPages.cs`. El smoke pasa de 109 a 110.
- **Referencia Mac:** `Free1X2CasosDeUso/Mascaras.swift`, `Free1X2Online/Historico.swift`,
  `App/Pantallas/Combinacion/Mascaras.swift`.
- **Aceptación (valores calculados a mano, JD-01):** máscara de 6 triples + 8 «1» contra la columna
  de unos ⇒ `k=8..14: 64, 192, 240, 160, 60, 12, 1` (729 columnas). Contra unos + doses ⇒ premios
  `{14:1, 13:12, 12:60, 11:160, 10:240}` y 1 jornada con premio de 2.

## W10 · Reductor y filtros con las «Columnas de Inicio», sin guardar antes un fichero (DT-85)

- **Qué (aprobado por el dueño):**
  - Reductor, Combinar filtros, Diferencias entre filtros, Coincidencias, Aidomnou y Pim tienen el
    botón **«Columnas de Inicio»**. Desarrolla el boleto con sus condiciones (y el filtro de columnas
    si existe) en `Temp/ColumnasInicio.txt` (se reescribe cada vez) y lo usa de entrada.
  - **Inicio ▸ Reducir** abre el Reductor ya cargado.
  - El Reductor sin «Archivo salida» reduce a `Temp/Reducido.txt` y ofrece **«Guardar
    resultado…»**.
  - Por encima de **1 000 000** de columnas se pide confirmación.
  - Si las condiciones no dejan ninguna, error y se borra el temporal viejo.
- **Dónde:**
  - Generación: la ruta «Grabar» de `CalculaColumnasFrmViewModel.cs`.
  - Pantallas: `ReductorFrmViewModel.cs`, `CombinarFiltrosViewModel.cs`, `DiFiltrosViewModel.cs`,
    `CoincidenciasViewModel.cs`, `aidomnouViewModel.cs`, `GeneraPimViewModel.cs` (en
    `Views/Ported/`).
  - Inicio: `ReducirCommand` (`Views/MainPage.xaml:290`).
- **Referencia Mac:** `CasosCombinacion.columnasDeInicio`, `App/Compartido/ColumnasInicio.swift`,
  `Sesion.columnasDeInicio`, `Reductor.swift`.
- **Aceptación:** boleto JD-01 ▸ Reducir ⇒ Reductor con `ColumnasInicio.txt` (729 columnas) ▸ Reducir
  ⇒ entre 73 y 729 admitidas y «Guardar resultado…» activo.

---

## W11 · Enlaces de ayuda a las páginas que existen (DT-86)

- **Qué (aprobado por el dueño):** clubprogol.com es WordPress y las direcciones antiguas dan 404.
  - Manual → `https://clubprogol.com/docwk/` (en minúsculas: WordPress las normaliza así)
  - FAQ → `https://clubprogol.com/docwk/faq/` (el contenido está en `docs/FAQ.md`)
  - Foro → `https://clubprogol.com/foro/`
  - En «Recursos de ayuda» quedan solo Manual, FAQ y Foro: se quitan Artículos, Recursos, Notificaciones y Facebook, que
    solo mostraban «Online help disabled…» en inglés. En «Acerca de» se quita «Descarga».
- **Dónde:** `Views/Ported/AyudaFrmPage.xaml.cs:19` (`UrlFaq`), `:24-25` (avisos en inglés) y su `.xaml`;
  `Views/Ported/AcercaDeFrmPage.xaml.cs:22-25` (`UrlDescarga`, `UrlForo`, `UrlManual`) y su `.xaml`.
- **Referencia Mac:** `App/Pantallas/Free/Informativas.swift` (`enum Web`).
- **Aceptación:** cada enlace abre una página que existe (no 404).

