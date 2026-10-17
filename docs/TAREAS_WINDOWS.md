# Tareas para el WinUI 3: igualar Windows con la app Mac

**Para el agente que trabaja en el PC Windows.** Este documento es la especificación completa: qué cambiar en el programa
Windows (`Free1X2.WinUI/` + `Free1X2.Domain/`), con qué textos, qué pruebas añadir y cómo dar cada tarea por hecha. Recoge
todo lo que la app Mac (`Free1X2.Mac/`) añadió o cambió entre el 16 y el 17-oct-2026 y que Windows aún no tiene. Hazlo de
principio a fin y en el orden de abajo. Lo que aquí está decidido no se vuelve a preguntar; al dueño solo se le pregunta lo
que este documento no resuelva.

## 0. Antes de empezar

1. **Traer el documento y el código de referencia.** Están en la rama del Mac, no en `main`:

   ```powershell
   git fetch origin
   git checkout claude/kind-hawking-1lz017
   ```

   Esta rama tiene el C# de `main` sin cambios y, además, `Free1X2.Mac/`, que es el modelo de cada tarea.
2. **Rama de trabajo:** la que indique el dueño. Si no indica ninguna, crea `windows/igualar-mac` desde esta rama y haz
   **un commit por tarea** (`feat(winui): W6 …`). **No** hagas push a `main` ni abras un PR sin su orden.
3. **Lee primero:** `CLAUDE.md` (comandos y reglas), `docs/PLAN_MEJORAS.md` §0 y, para cada tarea, el fichero de
   *Referencia Mac* y su decisión en `docs/mac/DECISIONES.md` (DT-xx). El Swift dice **qué** hace; el C# se escribe al
   estilo del C# que lo rodea (MVVM con `CommunityToolkit.Mvvm`, servicios en `Services/`, lógica en `Free1X2.Domain`).
4. **No toques** `Free1X2/` (WinForms, congelado), `Free1X2.Mac/` ni los 138 tests que ya existen en
   `Free1X2.Domain.Tests`.

**Cómo leer cada tarea.**
- *Dónde* cita el WinUI actual (fichero:línea, leído el 16-oct-2026). Si una línea se movió, búscala por el nombre citado.
- *Referencia Mac* es la implementación ya hecha y probada (rutas desde la raíz del repo).
- *Pruebas* son los tests nuevos que hay que añadir.
- *Aceptación* es lo que se comprueba antes de dar la tarea por hecha.

**Reglas** (`docs/PLAN_MEJORAS.md` §0):

- **R1:** el motor no cambia de resultados. Los **138 tests** existentes siguen en verde sin tocarlos. Los tests nuevos
  van en ficheros nuevos de `Free1X2.Domain.Tests/`.
- **R2:** las decisiones de UI de abajo ya están **aprobadas por el dueño** (16 y 17-oct-2026). Los textos entre «» son
  los de la app Mac y se copian tal cual, para que las dos apps digan lo mismo.
- **R3:** una tarea está hecha solo con build sin errores, todos los tests en verde, el smoke completo y la cita
  `fichero:línea` del cambio.
- **R5:** un solo `dotnet build` a la vez.

**Comandos** (de `CLAUDE.md`):

```powershell
dotnet build Free1X2.WinUI/Free1X2.WinUI.csproj -c Debug -p:Platform=x64   # 0 errores
dotnet test  Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj              # 138 + los nuevos
$env:FREE1X2_SMOKE = '1'
.\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe
Get-Content "$env:TEMP\free1x2_smoke.log" -Tail 1   # hoy total=109; tras W9, total=110
```

Borra `free1x2_smoke.log` antes de cada smoke: un log viejo puede dar por bueno un smoke que no terminó.

**Formatos compartidos con el Mac (no cambiarlos).** `Mascaras/<nombre>.mas` (W9) e `Historico/<liga>.txt` (W12) van en
la carpeta de datos, junto a `Equipos/`, en UTF-8 **sin BOM** (`new UTF8Encoding(false)`) y con CRLF. Un archivo hecho en
un sistema se usa tal cual en el otro.

**Orden:**

1. Correcciones pequeñas: W1–W5.
2. Jornada: W6–W7.
3. Navegación: W8.
4. Máscaras, columnas de Inicio y base de ganadoras: W9, W10 y W12. Comparten la generación de columnas y la pantalla
   «Máscaras».
5. Enlaces de ayuda (W11) e icono (W13, ya sustituido: solo verificar).
6. Cierre (§ «Cierre»): documentación, `CLAUDE.md` e informe al dueño.

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
- **Pruebas:** si la validación previa queda en `Free1X2.Domain` (recomendable), un test nuevo: con «a» en Variantes,
  el filtro y el grupo quedan idénticos a antes del intento. Si queda en el ViewModel, prueba manual. Mac:
  `f05CondicionNoValidaNoAltera` en `Free1X2.Mac/Packages/Free1X2Kit/Tests/Free1X2CasosDeUsoTests/FlujosTests.swift`.
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
- **Pruebas:** nuevo `Free1X2.Domain.Tests/RotacionTests.cs` con los casos de
  `Free1X2.Mac/Packages/Free1X2Kit/Tests/Free1X2MotorTests/TransponedorAutomaticoTests.swift` y de `f30Transponer`
  (`FlujosTests.swift`).
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
- **Pruebas:** la composición de nombres va en `Free1X2.Domain` (no en el ViewModel) para poder probarla. Nuevo
  `Free1X2.Domain.Tests/JornadaTests.cs` con `Fixtures/online/real-es.json` y `real-mx.json` (ya están en
  `Free1X2.Domain.Tests/Fixtures/online/`). Mac: `equiposDeJornadaReal` en `EquiposTests.swift`.
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
- **Pruebas:** la carrera descarga/reloj va en una función que devuelve la jornada o `null`. Test con un cliente que
  no responde nunca ⇒ `null` en menos de 3 s (más un margen de 1 s), y con uno que responde ⇒ la jornada. Mac:
  `jornadaAlArrancar` en `EquiposTests.swift`.
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

## W9 · Máscaras: guardar, aplicar y escrutar (DT-84)

- **Qué (aprobado por el dueño):**
  - Una **máscara** son los **signos de cada partido** del boleto, con nombre y sin equipos.
  - Se guarda desde Inicio y se aplica a la jornada cargada: pone los signos y deja los equipos.
  - Pantalla nueva **«Máscaras»** (grupo Combinación del panel de W8): lista, aplica y borra las máscaras y las
    **escruta contra la base de ganadoras de una liga** (W12). Por cada máscara muestra la suma de columnas con **14, 13,
    12, 11 y 10** aciertos en todas las jornadas de la liga y las jornadas con premio, en una tabla **ordenable** por
    cualquier columna.
- **Formato `.mas`** (compartido con el Mac): `Mascaras/<nombre>.mas` en la carpeta de datos.
  - Una línea por partido con su pronóstico, signos separados por comas (`1`, `1,X`, `1,X,2`), CRLF, también tras la
    última línea.
  - Nombre válido: se recorta; no puede quedar vacío ni llevar `/ \ : * ? " < > |`. Es el nombre del fichero.
  - Al listar se ignoran los `.mas` que, quitando las líneas en blanco, no tienen exactamente N líneas (N = partidos del
    boleto) o tienen un signo que no es `1`, `X` o `2`, o repetido.
- **Recuento sin desarrollar columnas** (igual de exacto y mucho más rápido): por cada ganadora, empezar con `c = [1]`.
  En cada uno de los 14 primeros partidos, si el signo ganador está en el pronóstico, `aciertos = 1` y `fallos = n−1`; si
  no, `aciertos = 0` y `fallos = n` (n = signos del pronóstico). Luego `nuevo[k] += c[k]·fallos` y
  `nuevo[k+1] += c[k]·aciertos`. Al final `c[k]` es el número de columnas con k aciertos. Premios = `c[10..14]`; una
  jornada «tiene premio» si alguno es mayor que 0.
- **Textos y controles** (iguales que en el Mac):
  - **Inicio, barra:** menú **«Máscaras ▾»** con:
    - «Guardar boleto como máscara…»;
    - una entrada por máscara, `«<nombre>  ·  <columnas> col.»` (dos espacios a cada lado del punto), que la aplica;
    - «Máscaras y escrutinio histórico…», que abre la pantalla.
  - **Guardar:** diálogo «Guardar boleto como máscara» con un campo «Nombre», los botones «Guardar» y «Cancelar», y el
    texto «Se guardan los signos de los N partidos, sin los equipos.».
    - Si ya existe: «Ya hay una máscara «X». ¿Reemplazarla?».
    - Al guardar: «Máscara «X» guardada.».
    - Nombre no válido: «El nombre de la máscara no puede estar vacío ni llevar / \ : * ? " < > |.».
  - **Pantalla, cabecera:** «Una máscara guarda los signos de cada partido, sin equipos. Aplícala a la jornada cargada o
    compárala con las ganadoras históricas para ver cuántas columnas de 10 a 14 aciertos habría tenido.».
  - **Tarjeta «Máscaras guardadas»:**
    - Cada fila: nombre; signos sin comas y separados por espacios; «F n · D n · T n · N col.»; y los botones «Aplicar al
      boleto» (aplica y vuelve a Inicio) y «Borrar» (confirma «¿Borrar la máscara «X»?»).
    - Sin máscaras: «Aún no hay máscaras: marca los signos en Inicio y pulsa «Guardar boleto como máscara…».».
    - Botón «Guardar boleto como máscara…».
  - **Tarjeta «Escrutinio histórico»:** ver W12.
  - **Tabla de resultados:** columnas «Máscara», «Columnas», «14», «13», «12», «11», «10» y «Jornadas con premio»
    (`x de y`). Orden inicial: «10» descendente.
    - Encima: «N jornadas escrutadas (liga «L»).».
    - Debajo: «Pulsa una cabecera para ordenar. Cada cifra suma las columnas premiadas de todas las jornadas.».
- **Dónde:** nuevo.
  - `Free1X2.Domain/` (casos de máscaras: nombre válido, existe, guardar, listar, borrar, aciertos, escrutar).
  - Página nueva `MascarasPage` (+ ViewModel) en `Views/Ported/`, registrada en `Navigation/PortedPages.cs` (de 108 a
    109 entradas). El smoke pasa de 109 a 110.
  - Inicio: el menú «Máscaras ▾» de la barra de W8.
- **Referencia Mac:**
  - `Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2CasosDeUso/Mascaras.swift` (`Mascara`, `CasosMascaras`).
  - `Free1X2.Mac/App/Pantallas/Combinacion/Mascaras.swift` (pantalla y diálogo `GuardarMascara`).
  - `Free1X2.Mac/App/Arranque/Sesion.swift` (`mascara`, `aplicarMascara`).
  - `Free1X2.Mac/App/Pantallas/Inicio/VistaInicio.swift` (menú «Máscaras»).
- **Pruebas** (nuevo `Free1X2.Domain.Tests/MascarasTests.cs`; en el Mac, `MascarasTests.swift`). JD-01 = partidos 1–6
  `1,X,2` y 7–14 `1` (729 columnas; F 8 · D 0 · T 6).
  - JD-01 contra la columna de catorce unos ⇒ aciertos `k=8..14`: 64, 192, 240, 160, 60, 12, 1 (suma 729).
  - JD-01 contra [unos, doses] ⇒ premios `{14:1, 13:12, 12:60, 11:160, 10:240}`, 1 jornada con premio de 2.
  - Guardar «  Mi 4D-2T » ⇒ existe «Mi 4D-2T» y listar la devuelve con los mismos pronósticos. Borrar la quita. Un `.mas`
    con 13 líneas, o con `1,1`, no se lista.
- **Aceptación:** los tests en verde. A mano: guardar JD-01 como «JD01» ▸ Nueva ▸ Máscaras ▾ «JD01  ·  729 col.» ⇒
  «Fijos: 8 - Dobles: 0 - Triples: 6» y los equipos intactos.

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
- **Pruebas:** la generación de `Temp/ColumnasInicio.txt` va en `Free1X2.Domain`. Test nuevo con JD-01 (W9) ⇒ 729
  columnas, y con condiciones que no dejan ninguna ⇒ error y sin fichero viejo. Mac: `f52ColumnasDeInicio` en
  `FlujosTests.swift`.
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

---

## W12 · Base de ganadoras por liga, con importaciones que se suman (DT-88)

- **Qué (aprobado por el dueño, 17-oct-2026):**
  - El programa guarda su **propia base de ganadoras**, separada por **liga**. Cada liga es una lista de jornadas pasadas
    con su columna ganadora.
  - Las importaciones se **suman**: si hoy se suben 1 500 jornadas y otro día 10 más, solo entran las 10.
  - Hay ligas «España» y «México» (las crea y alimenta la descarga de clubprogol) y **las que cree el usuario** para sus
    propios históricos.
  - El escrutinio de máscaras (W9) usa la liga elegida en un **selector**.
- **Formato `Historico/<liga>.txt`** (compartido con el Mac), en la carpeta de datos. **No** usar `Ganadoras/`: esa carpeta
  ya la usan Escrutador, Escrutinios y Posibles premios para otra cosa.
  - Una línea por jornada: `jornada;columna`, o solo `columna` si no se conoce la jornada. UTF-8 sin BOM, CRLF.
  - El nombre del fichero es el de la liga, con las mismas reglas de nombre que las máscaras (W9).
  - Un fichero vacío es una liga sin jornadas.
- **Lectura de un archivo** (la misma para importar y para leer la base). Cada línea se recorta y pasa a mayúsculas (como
  `ArchivoColumnasTexto`):
  - Las líneas en blanco se saltan.
  - Si hay `;`, lo de delante (recortado) es la jornada y lo de detrás (recortado) la columna. Una jornada vacía
    (`;111…`) es un error.
  - De la columna cuentan sus **14 primeros signos**; lo de detrás (el pleno, «2-1»…) se ignora. Deben ser 14 signos
    `1`, `X` o `2`.
  - Una línea mal formada **detiene la lectura** con el mensaje «La línea N no es «jornada;columna» ni una columna de 14
    signos 1, X o 2: «<línea>».». N cuenta desde 1 e incluye las líneas en blanco.
- **Suma a la liga** (`añadir`). Se recorre lo nuevo en orden:
  - Sin jornada: se añade al final (`sinJornada`).
  - Jornada que no está en la base: se añade al final (`añadidas`).
  - Jornada que ya está con la misma columna: no cambia nada (`yaEstaban`).
  - Jornada que ya está con otra columna: se sustituye en su sitio (`actualizadas`).
  - Un mismo archivo con la misma jornada dos veces se trata igual: la segunda cuenta como «ya estaba» o «actualizada».
  - Se guarda la base entera (escritura atómica) y `total` = jornadas de la liga.
- **Importar un archivo:** leer + sumar. Un archivo sin ninguna ganadora (solo líneas en blanco) es un error: «El archivo
  no tiene ninguna ganadora.».
- **Descarga:** `GET /wp-json/clubprogol/v1/quiniela/{es|mx}/historico`, ya en producción (contrato en
  `docs/API_CLUBPROGOL.md` §4). La jornada (número) pasa a texto y se suma a la liga «España» (`es`) o «México» (`mx`).
  Validación estricta: si una ganadora no tiene 14 signos 1/X/2, se rechaza la respuesta entera. `fecha` puede faltar o
  ser `null`.
- **Mensaje del resumen**, en este orden, separado por comas:
  - «A añadidas», «B ya estaban»;
  - si hay actualizadas, «C actualizadas (otra columna para la misma jornada)»;
  - si hay líneas sin jornada, «D sin jornada (añadidas sin comprobar si ya estaban)»;
  - y al final «. La liga tiene ahora T jornadas.».
  - Ejemplo: «1 añadidas, 2 ya estaban, 1 actualizadas (otra columna para la misma jornada). La liga tiene ahora 4
    jornadas.».
- **Tarjeta «Escrutinio histórico»** de la pantalla «Máscaras» (textos iguales que en el Mac):
  - Sin ligas: «Aún no hay ganadoras guardadas: descarga las de España o México, o crea una liga y añade su archivo.».
  - Con ligas: selector **«Liga»** con una entrada por liga, `«<liga>  ·  <n> jornadas»`, ordenadas por nombre
    (comparación natural), y el botón principal **«Escrutar las máscaras»**. El botón se desactiva si no hay máscaras o
    la liga tiene 0 jornadas.
    - Al abrir la pantalla se elige la liga del país de la última jornada en caché («España» si no hay); si no existe,
      la primera.
  - Botones:
    - **«Descargar de clubprogol ▾»** con «España» y «México». Mientras descarga: «Descargando el histórico…». Al
      terminar selecciona esa liga y muestra «clubprogol ▸ «España»: <resumen>». Con error: «No se pudo descargar el
      histórico: <motivo>».
    - **«Nueva liga…»**: diálogo «Nueva liga» con un campo «Nombre de la liga», los botones «Crear» y «Cancelar», y el
      texto «Después añade sus ganadoras desde un archivo.». Si la liga ya existe, se selecciona sin vaciarla. Al crear:
      «Liga «X» creada: añade sus ganadoras desde un archivo.». Nombre no válido: «El nombre de la liga no puede estar
      vacío ni llevar / \ : * ? " < > |.».
    - **«Añadir ganadoras desde archivo…»** (abre un `.txt`): suma a la liga elegida y muestra ««<archivo>» ▸
      «<liga>»: <resumen>». Con error: «No se pudo añadir el archivo de ganadoras: <motivo>».
    - **«Borrar liga…»**: confirma «¿Borrar la liga «X» y sus N jornadas guardadas?» y borra el fichero. Después:
      «Liga «X» borrada.».
    - «Añadir…» y «Borrar liga…» se desactivan sin liga elegida.
  - Ayuda bajo los botones: «Archivo: una jornada por línea, «jornada;columna» (p. ej. 2355;1X21X1121X1X21). Una jornada
    que ya está no se duplica; con otra columna, se actualiza.».
  - «Escrutar las máscaras» escruta todas las máscaras contra las columnas de la liga (W9) y pone encima de la tabla
    «N jornadas escrutadas (liga «L»).».
- **Dónde:** nuevo.
  - `Free1X2.Domain/Online/` (parser del histórico + modelo `ResultadoHistorico`: jornada, fecha, ganadora).
  - `Free1X2.Domain/` (casos de la base: ligas, leer, analizar, crear, añadir, importar, sumar descarga, borrar).
  - `Services/QuinielaOnlineService.cs` (`ObtenerHistoricoAsync(pais, token)`, con el mismo manejo de errores, 429 y
    timeout que `ObtenerJornadaAsync`).
  - `MascarasPage` (W9).
- **Referencia Mac:**
  - `Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2CasosDeUso/Mascaras.swift` (`GanadoraHistorica`,
    `ResumenImportacion`, `CasosHistorico`).
  - `Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Online/Historico.swift` y `ServicioQuiniela.obtenerHistorico`.
  - `Free1X2.Mac/App/Pantallas/Combinacion/Mascaras.swift` (tarjeta «Escrutinio histórico»).
- **Pruebas** (nuevo `Free1X2.Domain.Tests/HistoricoTests.cs`; en el Mac, `MascarasTests.swift`: `historicoReal`,
  `analizarArchivoDeGanadoras`, `baseDeGanadorasPorLiga`, `descargaSeSumaALaLiga`):
  - **Fixtures:** copiar `historico-es-2026-10-16.json` y `historico-mx-2026-10-16.json` de
    `Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Fixtures/online/` a `Free1X2.Domain.Tests/Fixtures/online/`.
  - **Histórico real:** España ⇒ 19 jornadas, la primera 20261014 con «11122X12112X2X». México ⇒ 24, la última 2331 con
    `fecha: null`. JD-01 (W9) contra España ⇒ `{11:3, 10:43}` en 10 de 19 jornadas (comprobado aparte con fuerza bruta
    sobre sus 729 columnas).
  - **Lectura:** `"2355;1x21x1121x1x211\r\n\r\n22222222222222 2-1\r\n 2356 ; 11111111111111\r\n"` ⇒
    [(2355, 1X21X1121X1X21), (sin jornada, 22222222222222), (2356, 11111111111111)]. `"\r\n"` ⇒ lista vacía (no es error al
    leer). Son error: `1,X,2,1`, `;11111111111111` y `1;111X`. `"11111111111111\n\n111X\n"` ⇒ el mensaje empieza por
    «La línea 3 no es «jornada;columna»».
  - **Suma** (U = catorce unos, D = catorce doses):
    - `crear("  Liga MX Femenil ")` ⇒ «Liga MX Femenil» con 0 jornadas.
    - Importar `1;U`, `2;D`, `U` ⇒ añadidas 2, ya estaban 0, actualizadas 0, sin jornada 1, total 3.
    - Importar `1;U`, `2;U`, `3;D`, `3;D` ⇒ añadidas 1, ya estaban 2, actualizadas 1, sin jornada 0, total 4, y el mensaje
      del ejemplo de arriba.
    - La base queda: jornadas [1, 2, —, 3] con columnas [U, U, U, D].
    - JD-01 contra esa base ⇒ `{14:3, 13:36, 12:180, 11:480, 10:720}`, 3 de 4 jornadas con premio.
    - Importar `"\r\n"` ⇒ error. `crear("a/b")` ⇒ error. Borrar la liga ⇒ no queda ninguna.
  - **Descarga sumada:** el histórico de España sumado dos veces ⇒ la 1.ª vez 19 añadidas; la 2.ª, 0 añadidas, 19 ya
    estaban, total 19. La única liga es «España» y su primera línea es `20261014;11122X12112X2X`.
- **Aceptación:** los tests en verde. A mano (el flujo F51 del Mac):
  - «Nueva liga…» «Mi liga».
  - Añadir un `.txt` con `1;` + catorce unos y `2;` + catorce doses ⇒ ««ganadoras.txt» ▸ «Mi liga»: 2 añadidas, 0 ya
    estaban. La liga tiene ahora 2 jornadas.».
  - Añadirlo otra vez ⇒ «… 0 añadidas, 2 ya estaban. …».
  - «Escrutar las máscaras» con la máscara JD01 ⇒ «2 jornadas escrutadas (liga «Mi liga»).» y 1 · 12 · 60 · 160 · 240,
    «1 de 2».
  - Copiar ese `Historico/Mi liga.txt` a la carpeta de datos del Mac (o al revés) ⇒ la otra app lo ve igual.

---

## W13 · Icono nuevo de la app (DT-90)

- **Qué (aprobado por el dueño, 17-oct-2026):** el mismo icono que la app Mac («FREE 1X2» azul neón, red naranja, marco
  plateado sobre base azul).
- **Ya hecho desde el Mac:** `Free1X2.WinUI/Assets/app.ico` se sustituyó por uno de 7 tamaños (16, 24, 32, 48, 64, 128 y
  256 px, 32 bits; el de 256 en PNG). Mismo nombre y ruta, así que **no** cambia código ni el `.csproj`: lo usan
  `<ApplicationIcon>` (icono del `.exe`) y `AppWindow.SetIcon` (ventana y barra de tareas). Se genera con
  `swift Free1X2.Mac/scripts/icono.swift …` (ver su cabecera); en Windows no hace falta regenerarlo.
- **No se toca** el `.ico` del WinForms (`Free1X2/`), que está congelado.
- **Pendiente en Windows (no se pudo compilar desde el Mac):** build, smoke y comprobar a la vista el icono del `.exe` en
  el Explorador, el de la ventana y el de la barra de tareas; después `pwsh ./scripts/publish-winui.ps1`. Si el Explorador
  sigue mostrando el icono viejo, es su caché: `ie4uinit.exe -show`.
- **Aceptación:** los tres sitios muestran el icono nuevo; build 0 errores y smoke completo.

---

## Lo que cambió en el Mac y **no** se lleva a Windows

| Cambio en el Mac | Por qué no aplica |
|------------------|-------------------|
| DT-87 «Escrutar con un archivo de ganadoras…» (archivo suelto) | Lo sustituye W12 (DT-88); no se implementa. |
| DT-89 autoría de la app Mac (créditos con un solo autor) | Decisión del dueño solo para el Mac: los créditos de Windows se quedan como están. |
| Etiquetas de «Condiciones del grupo» a dos líneas, en dos columnas o en una según el ancho | Arreglo de maquetación de SwiftUI; en el WinUI no estaban cortadas. |
| Lista de premiadas del Escrutinio como tabla virtual | Arreglo de rendimiento de SwiftUI; el WinUI ya virtualiza sus listas. |
| `.github/workflows/mac.yml`, tests de UI del Mac (`Free1X2.Mac/UITests/`) | Son del Mac. Windows se prueba con el smoke y los tests del dominio. |

---

## Cierre (después de W1–W13)

1. **Verificación completa:** build 0 errores, `dotnet test` con 138 + los nuevos en verde, smoke `total=110 ok=110
   fail=0` con el log borrado antes.
2. **`CLAUDE.md`:** al día con los números nuevos: páginas (`PortedPages.cs` 109 entradas), superficies del smoke (110),
   tests (138 + los nuevos).
3. **Manual y FAQ** (`docs/MANUAL_USUARIO.md`, `docs/FAQ.md`): quitar las marcas **[Mac]** y los «En la versión Mac…» de lo
   que Windows ya tenga (jornada al arrancar, Jornada ▾, panel lateral, Máscaras, base de ganadoras, Columnas de Inicio,
   `.mas`, `Historico/`). Luego `python scripts/md-a-wordpress.py`, que regenera `docs/web/manual.html` y
   `docs/web/faq.html`, y avisar al dueño de que hay que volver a subirlos a `clubprogol.com/docwk/` y
   `clubprogol.com/docwk/faq/`.
4. **Informe al dueño:** una tabla por tarea (W1–W13) con estado (hecha / no hecha y por qué), commit, `fichero:línea`
   del cambio y los tests añadidos. Lo que no se pudo hacer se dice, no se da por hecho.
