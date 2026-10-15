# Registro único de decisiones (app Mac)

> Cierra H-07 y H-08 de [`REVISION_SPECS.md`](REVISION_SPECS.md). Fusiona las ≈196 «Abiertas» de ESPEC_01…10, las decisiones
> D1–D8 del índice y las de PLAN §7. Cada «Abierta» de una spec tiene aquí su estado; **si una spec dice «decisión pendiente»,
> manda este registro**.
>
> **Mandato del dueño (2026-10-13):** (1) las decisiones de producto las define la app actual («usar eso»); (2) las técnicas las
> toma Claude con buenas prácticas de velocidad, mantenimiento, arquitectura y pruebas; (3) el dueño revisa todo al final.
>
> **Cierre de M0 (dueño, 2026-10-14):** el dueño delegó en Claude cerrar todo lo pendiente con buenas prácticas. Todas las entradas
> están **cerradas** (D, o D con candidata R). El valor exacto de cada «replicar» lo fija su caso `lectura` en M1 (ESPEC_12): es
> trabajo de M1, no una decisión abierta.

Estados: **D** decidida (se aplica) · **R** se replica hoy; **candidata a corrección después de la paridad** (solo con
orden expresa del dueño; no se implementa).

## 1. Decisiones de producto (= app actual) — detalle en ESPEC_11 §2

| ID | Tema | Decisión | Fusiona |
|----|------|----------|---------|
| DP-01 | Cultura numérica | C1–C2 `es-ES` fija; C3–C13 cultura del sistema; golden con `es-ES` | D4; 01 A9; 03 A1; 05 §7 T7; 06 A3; 07 A15; 09 G-3; 10 A-15 (formato: G-NET) |
| DP-02 | EOL, BOM, codificación | mismos bytes que hoy en Windows (CRLF; BOM y Latin-1 por formato); lectura tolerante como el C# | D5; 05 A10, A14; 06 A14; 07 A1, A7; 09 A-60-1, A-71-1 (este: UTF-8 sin BOM con CRLF, DEC-708) |
| DP-03 | Nº de partidos | `NumeroPartidos` de configuración (defecto 14); límites por componente como hoy; superarlos = mismo error tipado | 02 A2; 03 A7; 04 A-2, A-11; 06 A15; 10 A-14 |
| DP-04 | Recarga de configuración | lectura única al arrancar; guardar no recarga (salvo puntos CP y N de `Grupo`) | 01 A8; 08 A6 |
| DP-05 | Ventana/documento | una ventana, una `Sesion`, sin modelo de documento | 10 A-01 |
| DP-06 | Distribución | fuera de la Mac App Store, sin sandbox, Developer ID + notarización; rutas absolutas como hoy | D8; PLAN §7.3; 07 A16 |
| DP-07 | Localización | UI en español; selector es/en que solo traduce Créditos; `.lang` no se porta | PLAN §2 «Idiomas»; 07 A8; 10 A-04 |
| DP-08 | Código sin uso | no se porta lo inalcanzable desde la app actual (lista en §4) | D3; 01 A11; 03 A8; 04 A-10; 05 A4, A5, A7; 06 A7, A9, A10; 08 A9; 09 A-46-3 (el campo se muestra, no calcula); 10 A-11, A-20 |
| DP-09 | Defectos aparentes | se replican; cada uno con su caso `lectura` (`G-D2`, ESPEC_12 §4.2) | D2 y todos los «Defecto aparente» (§3) |
| DP-10 | Mejoras de UX | ninguna; la arquitectura deja preparada la cancelación | 10 A-16; 09 A-50-1, A-72-1; 05 A9 |
| DP-11 | Navegación y áreas | estructura actual de 7 menús y 6 grupos de barra, traducida a convenciones Mac (ESPEC_15) | D7; PLAN §4 |
| DP-12 | Preferencias | tema/idioma con las opciones de hoy en `UserDefaults`; barras en `parametros.free1x2` | 10 A-03 |
| DP-13 | Carpetas | las 10 de hoy bajo `~/Library/Application Support/Free1X2/`; selectores en Documentos; `Ganadoras/` se llena a mano | 08 A20; 09 G-2, A-44-2; 10 A-06 |
| DP-14 | Ubicación del proyecto | carpeta `Free1X2.Mac/` en este repo (junto al código original, que solo se lee) | PLAN §7.2 |
| DP-15 | Referencia de comportamiento | **solo la app actual** (WinUI + `Free1X2.Domain`); toda divergencia con el WinForms legacy se resuelve a favor del port | D1; 09 G-1, A-41-1, A-41-2, A-57-1, A-62-1, A-62-2, A-68-2; 08 A2, A5, A17; 10 A-10 (manda el código, no el manual), A-22 |

## 2. Decisiones técnicas — detalle en el documento indicado

| ID | Decisión | Dónde |
|----|----------|-------|
| DT-01 | Módulos `Free1X2Base`/`Motor`/`Formatos`/`Online`/`CasosDeUso` + app; grafo acíclico; solo `swift-collections` | ESPEC_11 §3 |
| DT-02 | Configuración = `struct` `Sendable`; ejecución = `final class` internas por ejecución | ESPEC_11 §4 |
| DT-02b | Paridad definida por ejecución desde estado limpio (no se arrastra el memo de un `Grupo` no controlado entre cálculos) | ESPEC_11 §4.3 |
| DT-03 | Errores tipados que replican cada excepción C# en el mismo punto; nunca abortar | ESPEC_11 §5; cierra 01 A7; 04 A-1 (se descarta su propuesta de validar al fijar), S4; 03 B7, A5; 06 §1.8 |
| DT-04 | Concurrencia Swift 6, operaciones largas `@concurrent`, progreso limitado, cancelación cooperativa, paralelismo solo donde no cambia el resultado | ESPEC_11 §6 |
| DT-05 | `Entorno` inyectado por parámetro (reloj, aleatorio, archivos, rutas, red, portapapeles, registro, cultura) | ESPEC_11 §7 |
| DT-06 | Persistencia y rutas (semillas que no se sobrescriben, escrituras atómicas) | ESPEC_11 §9; 07 A13 |
| DT-07 | `Sesion` única, `Router` con `Destino`, modelos de pantalla sin lógica | ESPEC_11 §8 |
| DT-08 | Bucles que en C# **no terminan** (09 A-73-1; 10:1341 `ColumnasPorTramo ≤ 0`) ⇒ se detecta la falta de progreso y se lanza `ErrorDominio(.operacionInvalida)` con un mensaje; los casos que terminan dan el mismo resultado | — |
| DT-09 | Reglas de rendimiento del camino caliente | ESPEC_11 §10 |
| DT-10 | macOS 14+, arm64, Swift 6.2+, Swift Testing | ESPEC_11 §12 |
| DT-11 | XML con `Foundation.XMLDocument` + `EscritorXMLNET`; ZIP con `Compression.framework` sin terceros | ESPEC_16 §2-3; 06 A18 (DEC-616) |
| DT-12 | Convenciones de código y trazabilidad `// C#: fichero:línea` | ESPEC_11 §11 |
| DT-13 | Mayúsculas ASCII invariantes para letras de tolerancia y columnas (la cultura turca no se emula) | 02 A5; 06 A5 |
| DT-14 | Orden de ficheros de `DirectoryInfo.GetFiles`: por nombre, ordinal sin distinguir mayúsculas (orden de NTFS); fijado por lectura (`bcl`, ESPEC_12 §1.1) | 03 A3; 10 A-19 |
| DT-15 | Ordenaciones: **toda** `Array.Sort`/`List<T>.Sort` del C# se porta con `IntrosortNET` (mismo algoritmo de .NET 8, con la pasada previa que pone los `NaN` primero en claves `double`/`float`); los quicksorts escritos a mano en pantallas se transcriben tal cual. Regla única, sin depender de si el orden llega a la salida (el C# no usa `OrderBy`) | 04 A-9; 05 A3; 06 A9 |
| DT-16 | Cliente HTTP: `URLSession` efímera, timeout 10 s, `Accept: application/json`, `User-Agent: Free1X2/<versión> (macOS)`; redirecciones y descompresión por defecto | 07 A10 |
| DT-17 | Mensaje genérico HTTP con los nombres de `HttpStatusCode` de .NET 8 (tabla de los códigos estándar) | 07 A11 |
| DT-18 | Selector «Nuevo…» de valoraciones: se aplica la lógica C# al estado real del fichero; como `NSSavePanel` no crea un fichero vacío (a diferencia de WinUI), «inexistente» ⇒ nuevo | 09 A-64-1 |
| DT-19 | Ayuda: el menú Ayuda que macOS crea siempre abre la página «Recursos de ayuda» actual (mismo contenido, avisos en inglés incluidos) además de `F1`/`⌘?` | 10 A-02, A-05 |
| DT-20…27 | Oráculo **por lectura** (decisión del dueño 2026-10-14: el original solo se lee, nunca se ejecuta; no hay GoldenGen): golden JSONL escritos a mano con `fuente` por caso (`test`, `fixture`, `lectura`, `modelo`, `bcl`, `propiedad`), plataforma fijada por lectura (CRLF, T13, `es-ES`), niveles O1/O2/O3, corpus `fixture` + `lectura`, instantáneas solo para regresión | ESPEC_12 §1, §1.1, §2, §5, §6 |
| DT-28 | Golden y fixtures como recursos de `Free1X2Pruebas` (`Sources/Free1X2Pruebas/{Golden,Fixtures}`): SwiftPM no admite recursos fuera de su target (`.copy("../Golden")` de ESPEC_11 §3.3 no compila); el cargador lee `Bundle.module` | ESPEC_12 §2; ESPEC_11 §3 |
| DT-29 | M1 = catálogo funcional (ESPEC_17) + 104 casos heredados + infraestructura; cada componente escribe su suite JSONL **antes** de su código (*test-first*) | ESPEC_17 §4; ESPEC_12 §7 |
| DT-30…39 | Pruebas, cobertura, CI y presupuestos de rendimiento | ESPEC_14 |
| DT-40 | API de casos de uso (5 formas, `enum` por menú, unificaciones) | ESPEC_13 |
| DT-42 | Proyecto Xcode como código: `Free1X2.Mac/project.yml` (XcodeGen) es la fuente; `Free1X2.xcodeproj` se genera y se versiona para abrirlo sin herramientas | ESPEC_11 §3.1 |
| DT-43 | El producto `Free1X2Kit` expone `Free1X2CasosDeUso` y `Free1X2Base`; la app no puede importar Motor, Formatos ni Online | ESPEC_11 §3.2-3.3 |
| DT-44 | Dependencias y targets se añaden con su primer uso: `swift-collections` y `Free1X2Bench` en M3; `aleatorio` y `cultura` del `Entorno` llegan con `AleatorioNET` y `Cultura` | ESPEC_11 §3.3, §7 |
| DT-45 | `ClienteHTTP` es transporte (`throws` sin tipar, `URLError`); el cliente online lo traduce a `ErrorOnline` con los textos del C#. `Portapapeles` es asíncrono y la app lo implementa con `NSPasteboard` en el hilo principal | ESPEC_11 §7 |
| DT-46 | `ReportadorProgreso` limita por tiempo (≥ 100 ms entre eventos; el final siempre sale); el bucle caliente informa cada 65 536 columnas | ESPEC_11 §6.2; ESPEC_17 NF-06 |
| DT-47 | Versión `0.1.0` (build 1), solo `arm64`, firma ad hoc y runtime endurecido hasta M6; CI en `macos-26` (Swift ≥ 6.2) | `project.yml`; ESPEC_14 §7 |
| DT-48 | El motor no lee ficheros: recibe las líneas ya leídas (`ArchivoColumnasTexto.columnas`) y devuelve filas como valores (`FilaEscrutinio`) en lugar de escribir en un `DataSet`; el escrutinio por temporada lee una vez el fichero que el C# relee por jornada (ESPEC_05 §4.1.6) | ESPEC_05 §4.1; ESPEC_11 §3.3 |
| DT-49 | Valores esperados de `IntrosortNET` y `HashtableNET` (`net/colecciones`, GE-07, GE-09): fuente `modelo`, transcripción en Python de `ArraySortHelper.cs` y `Hashtable.cs` de .NET 8 leídos, independiente del código Swift | ESPEC_12 §1; ESPEC_16 §6 |
| DT-50 | Iconos de la barra: SF Symbols en lugar de los PNG de 16×16 px (mismo tooltip y posición) | ESPEC_15 §1.4 |
| DT-51 | Las acciones de la barra «Archivo» (Nueva, Abrir, Guardar, Guardar como, equipos, online, borrar) también en el menú Archivo, donde macOS pone los atajos `⌘N/⌘O/⌘S` (como ya indica el manual) | ESPEC_15 §2.2; 10:140-142 |
| DT-52 | Cambiar de idioma no recrea la página (la recarga de WinUI era una limitación técnica) | ESPEC_15 §2.4; 10:66 |
| DT-53 | Los contadores de progreso se refrescan con el progreso limitado (≤ 100 ms) en vez de un temporizador de 3 s; mismos campos y valores finales | ESPEC_15 T3; 10:808 |
| DT-54 | CP y grupos de equipos: la configuración son `struct` (`CondicionColProbables`, `CondicionGruposEquipos`) y en cada cálculo se construyen las clases de ejecución; las relaciones reciben la lista de CPs/grupos de **ese** filtro, así que el orden de montaje P24 deja de importar (equivale al C# cuando se respeta). `RelacionCP2Config` guarda los textos y las listas D3 se calculan al construir; `GrupoEquiposConfig` recalcula `longPronosticos` al asignar `pronosticos` | ESPEC_04 §0.2; ESPEC_03 §15 P1 |
| DT-55 | `RelacionCP3` con más índices (K) que CPs: el C# lanza `ArgumentOutOfRange` al cargar; el cargador Swift (M4) debe hacer lo mismo y, si llega al motor, se lanza al primer uso de la serie | ESPEC_04 §15 P12 |
| DT-56 | `Free1X2Bench` (ESPEC_14 §6) mide ya B1, B5 y B6 (línea base en `Free1X2.Mac/bench/linea-base.json`, M1 Max); B2–B4, B7 y B8 se añaden con el cargador `.comb` y los formatos (M4). Las particiones de §6.3 de ESPEC_11 se implementan cuando B2 sea medible y solo si la mediana secuencial no cumple el techo: B1 secuencial = 0,13 s frente a 1 s | ESPEC_14 §6.3; ESPEC_11 §6.3, §10 |
| DT-57 | `Free1X2CasosDeUso` reexporta (`@_exported import`) Base, Motor, Formatos y Online: la app ve los tipos de valor (`Combinacion`, `CondicionX`, `JornadaQuiniela`…) importando solo el producto `Free1X2Kit`; la lógica sigue pasando por los casos de uso | ESPEC_11 §3.2 |
| DT-58 | Editor de figuras (pantalla 49): edita una **copia** de la lista y la devuelve al aceptar; el C# la compartía por referencia, de modo que Borrar/Abrir + Cancelar mutaban la condición (peculiaridad que 09 §49 y 10 §88 dejan al dueño). Pantallas de condición: los «handoffs» estáticos (`GrupoEnEdicion`, `UltimasEstadisticas`, `FigurasEnEdicion`) viven en `Sesion` y `Destino` es un `enum` sin valores asociados (smoke `allCases`) | ESPEC_15 §2.4; 10 §1.8 |
| DT-59 | Grupos de equipos (pantalla 62): la copia de trabajo y el filtro son valores; se replica que `GuardarDatos` (Guardar/Abrir/Copiar/Pegar/Borrar) escriba en la condición del grupo mientras el filtro es el suyo, y que tras Abrir Aceptar no llegue a la combinación (DEC-927), pero no el alias de lista por el que cambios posteriores sin Aceptar también llegaban al filtro | 09 §62 |
| DT-60 | Columnas probables (pantalla 19) y satélites 29/30/48/67: el editor es un valor en la vista y nada llega al filtro hasta Aceptar (como DT-59, no se replica el alias por el que Relaciones II/III cambiaban el filtro al navegar). Copiar datos e Importador devuelven por `Sesion.traspasoCP` y al volver no se recarga del grupo, como especifican ESPEC_08 §19.1 y CU-A46/A47: en el port la página se recrea al volver (sin `NavigationCacheMode`) y esos resultados nunca se aplicaban. Relaciones I «<» deshabilitado en la primera (§19.2); `MaxLength=14` del pronóstico con `onChange` | 08 §19, §29, §30; 09 §48, §67 |
| DT-61 | Escrutinios (pantalla 45): «Insertar /t» y «Insertar /j» añaden al final (SwiftUI de macOS 14 no da la posición del cursor; el port ya hacía eso sin cursor válido); `GetFiles(plantilla)` se resuelve con `fnmatch` sin distinguir mayúsculas (`*`, `?`); los ficheros se leen una vez por escrutinio (ESPEC_05 §4.1.6); el botón Escrutar se deshabilita mientras corre (ESPEC_15 §4) | 09 §45; 08 §20 |
| DT-62 | Compresor (pantalla 22): el `.z3q` se escribe con `compress2` de la zlib del SDK (sin terceros) quitando cabecera y Adler-32, así «Nivel de compresión» 0–9 cambia el resultado como `SetLevel` del C# (sustituye el nivel fijo de ESPEC_16 §3; la lectura sigue con Compression.framework). Importar/Exportar (66): la salida propuesta es el **nombre** en minúsculas con `.csv`↔`.txt` en la misma carpeta (el `ToLower()` de la ruta completa solo es inocuo en NTFS). Histórico de valoraciones (64): se sigue la réplica viva del VM (`Length >= 2`); la de `Porcentajes` no tiene llamadores | 08 §22; 09 §64, §66; ESPEC_16 §3 |
| DT-63 | Análisis de signos (97): cada casilla de la columna ganadora es un selector «-/1/X/2» (el `TextBox` de 1 carácter admitía otros caracteres, que el C# trata como comodín igual que «-»). Diferencias entre columnas (41): columnas empaquetadas en 14 grupos de 3 bits (1, 2, 4 one-hot; F = 111) y «diferencia» = grupo con AND 0, equivalente a `(x & y) == '0'`; el universo de triples se recorre con dos tablas de 3^7 en el mismo orden que la recursión. El tiempo se rellena con ceros antes de cortar a 10 (el `Substring(0,10)` del C# lanzaría con 0 *ticks*). Subir categoría (92): el motor vuelve a la pantalla también tras un error, para conservar el estado residual como el C# (DEC-513) | 09 §41; 10 §92, §97 |
| DT-64 | Añadir P15 (1): con error a mitad no se graba nada (el `StreamWriter` del C# queda sin cerrar y su contenido depende del búfer). Gráfico de columnas (61): las líneas se guardan sin repetir (el C# añade una por apuesta, hasta 4,7 M elementos, y se ven igual); «Copiar imagen» usa `ImageRenderer` y ofrece el PNG en Imágenes | 08 §1; 09 §61 |
| DT-65 | EstuCol (47): emparejar, escrutar e informar van en un solo caso de uso; el estado muestra «Comprobando Entradas», «Emparejando columnas» y «Listo» (los intermedios «Escrutando Columnas»/«Generando Informe» del C# duran milisegundos). Visor ABDON (101): el informe llega por `Sesion.informeAbdon`, que el visor consume y vacía como el `UltimoInforme` estático; la lógica del VM (contenedores `List<int>[,]`, textos y condición) vive en `VisorAbdon` del paquete | 09 §47; 10 §101 |
| DT-66 | Impresión de boletos (68): como el port, genera la imagen (portapapeles + PNG en Imágenes) y no abre la impresión del sistema; `impresoras.cfg` se lee de los recursos (`Datos/Impresion/`) y `imprebol.cfg` de la carpeta base. Impresoras conocidas (70): doble clic = acción primaria de la lista; la elegida pasa por `Sesion.impresoraSeleccionada`; el orden es `compare(_:locale: .current)` (T11) | 09 §68, §70 |
| DT-67 | Selector JuanM (86): «por valor» agrupa por los tres códigos de grupo (la columna ↔ sus códigos) y comprueba el total con conjuntos de sumas de posiciones en bits, en vez de comparar cada columna con las 243·243·81 posiciones; mismo conjunto y mismos contadores (si se cancela, lo parcial sale en otro orden). Los botones no se deshabilitan durante el cálculo, como en el C# (DP-10). Selector MarioSan (87): los vecinos a distancia exacta 1, 2 o 3 se enumeran directamente (28/364/2912) en vez de los caminos con `repes`; mismo recuento. Una fila Q fuera de la tabla al analizar se ignora (el C# lanzaría) | 10 §86, §87 |
| DT-68 | Separador Juan Bellas (50): el cálculo va en segundo plano y «Cancelar» sigue sin efecto (DEC-915). Diferencias entre filtros (33): `Valida` cuenta los vecinos a distancia exacta 1/2/3 con salida temprana (mismo resultado que los caminos ordenados del C#); el estado `validas`/contador sobrevive entre «Iniciar» como los campos del VM; una columna fuera del espacio de 14 signos se informa como error (el C# lanzaba sin capturar) | 08 §33; 09 §50 |
| DT-69 | Listado de condiciones (71): el árbol sale de `Sesion.combinacion` (el `Analizador` vivo del C#) con los textos literales; la columna probable lista sus N pronósticos (`textoPronosticos`; el C# podía añadir un N+1 vacío según cómo se creó); listas nulas del C# (agrupaciones, partidos de una diferencia) se tratan como vacías en vez de lanzar | 09 §71 |
| DT-70 | Banco de pruebas (10) y Grabar apuestas (36): las 3^14 columnas guardan solo diferencia y columna (`Float`, `Int32`; la probabilidad no se lee) y se ordenan con `OrdenacionPropia`; «Fichero» como origen de las aleatorias y «Seleccionar jornadas…» no hacen nada, como en el C#; `Desviación Típica deseada` y `LN mínimo` no se portan (DEC-809); Calcular y Analizar se inhabilitan mientras calculan (`Ocupado` del C#); el diálogo 36 es hoja del Banco y, como destino suelto, solo cambia su estado | 08 §10, §36 |
| DT-71 | Tramificar (93, 94, 34, 35, 37): `Ap14T`, `Bits`, `MaxiMin` y los tramos viven en `EstadoTramificar` (valor); `OrdenacionPropia` pasa a clave `Comparable` (aquí `Double`). «Filtrar por límites» es hoja de Tramificar, así el filtro se aplica sobre lo tramificado (en WinUI la vuelta atrás recrea la página sin `NavigationCacheMode` y el filtro llega a un VM vacío); abierto desde Tramificar sus difs empiezan en 0 (`ObtenerExtremos`), los defectos solo salen como destino suelto, que vuelve atrás al aceptar o cancelar. «Columnas / tramo» ≤ 0 con rango no vacío da error en vez del bucle infinito del C#. «Tramificar» se inhabilita mientras calcula (`AsyncRelayCommand`). Gráficas: `Canvas` con la geometría de `GraficoLineasControl`. 34 y 37 sin consumidor (DEC-815); leer un `.lst` con una línea corta deja lo leído y avisa con un texto propio (el de la excepción .NET no se reproduce) | 10 §93, §94; 08 §34, §35, §37 |
| DT-72 | Coincidencias (17): la radiografía trabaja sobre las unidades UTF-16 de los 14 primeros signos (cualquier otro carácter cae en la rama del C#); la entrada es un `ConjuntoBits` y de las válidas se guarda solo índice y coincidencias de la última condición (en vez de `agrabarCOL` de 3^14). Los errores de lectura replican lo parcial del C# (entrada a medias; ganadoras que pisan las primeras y cambian el límite sin mover la actual); el desbordamiento de 3000 ganadoras y el Excel con más de 100 condiciones avisan con el texto de la excepción .NET (`Index was outside the bounds of the array.`), y el Excel no llega a escribir el fichero parcial. «Calcular» se inhabilita mientras calcula (`AsyncRelayCommand`); «Cancelar» lo corta y deja lo procesado. Analizar sin ganadora actual (lectura fallida) da error inesperado en vez de cerrar la app | 08 §17 |
| DT-41 | Piezas transversales `FormatoNET`, `TextoNET`, `AleatorioNET`, ayudantes únicos | ESPEC_16 |

## 3. Registro de las «Abiertas» por spec

Formato: `origen` → decisión (estado). Las que solo remitían a otra spec se cierran con la referencia.

### ESPEC_01 (motor núcleo)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-101 | A1 GoldenGen inexistente | ESPEC_12 (D) |
| DEC-102 | A2 `EsGrupoBase` tras cargar | replicar el bucle tal cual (`ES/ArchivoCombinacion.cs:221-232`): el índice `i` cuenta también los comentarios XML, así que `EsGrupoBase` queda `true` solo si el grupo tiene exactamente un hijo (D) |
| DEC-103 | A3 `VaciarInformacion` descarta lo del control 0 | replicar: `VaciarInformacion()` antes de cada control libre `j>0` (`MC/ControladorGrupos.cs:224`) (D) |
| DEC-104 | A4 Diferencias + tolerancias lanza | `throw new Exception("The method or operation is not implemented.")` (`MC/FiltroDiferencias.cs:124-127`) ⇒ `ErrorDominio(.general)` con ese mensaje, en el mismo punto (D; DT-03) |
| DEC-105 | A5 formato de `Valor*` con negativos | replicar (`MC/Analizador.cs:234-249`): un valor negativo contiene `-` y se trata como intervalo; `Split('-')` deja un trozo vacío y `Convert.ToDouble("")` ⇒ `ErrorDominio(.formato)`; los extremos de intervalo se redondean a par (D) |
| DEC-106 | A6 tamaños de `ColumnasPorFallos*` | replicar los índices literales; un `nFallados` fuera de rango ⇒ `ErrorDominio(.indiceFueraDeRango)` en el mismo punto (D) |
| DEC-107 | A7 estructura de errores | DT-03 (D) |
| DEC-108 | A8 reinicializar = no-op | DP-04 (D) |
| DEC-109 | A9 cultura | DP-01 (D) |
| DEC-110 | A10 `ObtenNumCols` supone CRLF | replicar; coherente con DP-02 (D) |
| DEC-111 | A11 `Signo`, `ReduccionTipo`, `AlgoritmoReduccion`, `IAppPaths`, `IProgressNotifier` sin uso | DP-08 (D) |
| DEC-112 | A12 alcance de lectura de `ContenedorAnalisis` | cubierto por ESPEC_06 §3.5 (D) |

### ESPEC_02 (filtros A)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-201 | A1 celdas multi-signo | replicar el `switch` literal para cualquier valor de celda, incluidas las multi-signo (D) |
| DEC-202 | A2 rango de N | DP-03 (D) |
| DEC-203 | A3 If-Then con Peso numérico siempre `false` | replicar (D; R) |
| DEC-204 | A4 `values=""` no se relee | replicar: el lector lanza ⇒ `ErrorDominio(.indiceFueraDeRango)` al abrir, que llega al mismo `catch` que en C# (D) |
| DEC-205 | A5 letras de tolerancia y `ToUpper` | DT-13 (D) |
| DEC-206 | A6 `isLetter` vs `IsLetter` | `FormatoNET.esLetra` con categorías Unicode (D; `G-NET-TXT`) |
| DEC-207 | A7 máscara del grupo base en análisis | replicar (`GRP:39`, `GRP:232-233`) (D) |
| DEC-208 | A8 caracteres ≠ `1/X/2/*` en Interrupciones | replicar (rama «X») (D) |
| DEC-209 | A9 figuras ≥ 16 · A10 duplicados en figuras · A11 tolerancia sin 0 | replicar lo que permita la UI actual (D) |

### ESPEC_03 (filtros B)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-301 | A1 cultura de `double` | DP-01 (D) |
| DEC-302 | A2 `aciertosMax` no se persiste | replicar (D; R) |
| DEC-303 | A3 orden de `GetFiles` | DT-14 (D) |
| DEC-304 | A4 `Convert.ToInt32/ToDouble` | `FormatoNET` con la semántica de .NET 8 (`bcl`) + `G-NET-PARSE` (D) |
| DEC-305 | A5 Diferencias + tolerancias | = DEC-104 |
| DEC-306 | A6 Dibujos vía If-Then rechaza siempre | replicar (D; R) |
| DEC-307 | A7 N ≠ 14 | DP-03 (D) |
| DEC-308 | A8 `Diferencia.AnalizarFallos` sin llamadores | DP-08 (D) |
| DEC-309 | A9 elemento `""` en textos de fallo / «Columna Fuera de Límites» | replicar; `Substring(0,1)` sobre vacío ⇒ error tipado como el C# (D) |
| DEC-310 | A10 líneas no válidas en históricos | replicar (D) |
| DEC-311 | A11 `RelacionGE1` · A12 `IsLetter` · A13 cobertura | ESPEC_04 · DEC-206 · ESPEC_12 (D) |

### ESPEC_04 (CP y relaciones)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-401 | A-1 política de errores | DT-03: se replican los fallos del C# (no se valida al fijar) (D) |
| DEC-402 | A-2 N = 15 en RelacionCP1/CP3 | DP-03: con `noAC = 15`, `ErrorDominio(.indiceFueraDeRango)` como el C# (D) |
| DEC-403 | A-3, A-4 `values=""` en CP y tolerancias | replicar: el lector lanza `IndexOutOfRange` (`str[-1]`) ⇒ error tipado (D) |
| DEC-404 | A-5 caché obsoleta de RelacionCP3 (P16) | replicar (D; R) |
| DEC-405 | A-6 BCL | `G-NET` (`bcl`) (D) |
| DEC-406 | A-7 RelacionCP3 desde el informe ABDON | replicar el ViewModel (`Columnas = cols`) (O2) (D) |
| DEC-407 | A-8 unidades de `impresoras.cfg` | centésimas de pulgada → puntos ×0,72 (exacto: 1 pt = 1/72 in); el boleto impreso por el dueño en M5 es prueba de aceptación, no una decisión (D) |
| DEC-408 | A-9 `NaN` en `OrdenarValoracion` | DT-15: `IntrosortNET` con los `NaN` primero (D) |
| DEC-409 | A-10 `IO`/`GCP` muertos | DP-08 (D) |
| DEC-410 | A-11 N cambia durante la vida de una CP | imposible: `Configuracion` inmutable por ejecución (D) |
| DEC-411 | A-12 `Relaciones.Add` directo | replicar (solo serializa) (D) |
| DEC-412 | A-13 GoldenGen · A-14 `RelacionGE1` | ESPEC_12 · ESPEC_03 (D) |

### ESPEC_05 (reducción y escrutinio)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-501 | A1 `EscrutadorComb` (Hashtable, índice base-3) | replicar (`Escrutinio/EscrutadorComb.cs:114,120,224,243`): las claves de vecinos son `Int32` y la de la ganadora `Int64` con valor `byte`; `Keys.CopyTo(long[])` ensancha, pero la búsqueda con clave `long` no encuentra las `Int32` (`(int)null` ⇒ `NullReferenceException`) y en la ganadora `(int)` sobre un `byte` encajado ⇒ `InvalidCastException`. La primera columna que acepte `AnalizaColumna` (que recibe el índice base 3 como si fuera de 3 bits) lanza; si ninguna la acepta, los premios quedan a 0. El port recorre las claves en el orden de `HashtableNET` (`G-NET-HASH`) y lanza `ErrorDominio(.referenciaNula)` o `(.conversionInvalida)`, que la pantalla muestra como «No se pudo completar el escrutinio:…» (`EscrutarCombinacionesFrmViewModel.cs:496-499`) (D; R) |
| DEC-502 | A2 orden de `ListaEscrutadasConPremio` | `ListaEscrutadasConPremio` nunca recibe elementos (el `Add` va tras el cast que siempre falla, DEC-501): siempre vacía; su orden no aplica (D) |
| DEC-503 | A3 `PosiblesPremiosComparer` con empates | DT-15 (D) |
| DEC-504 | A4 `PonerPremios`/`AñadirPremiosGlobales` | DP-08 (D) |
| DEC-505 | A5 XFSF, XfsfV3, JLPM | DP-08 (D) |
| DEC-506 | A6 último partido de `JDCDobleContador.Marca` | replicar `Partido < noPartidos` (`GR-06`) (D) |
| DEC-507 | A7 `ReductorTM` | **no se porta**: la app no lo ofrece (`Free1X2.WinUI/Views/Ported/ReductorFrmViewModel.cs:43-44`; 10 A-11) (DP-08) |
| DEC-508 | A8 `nivel ≥ noPartidos` | replicar (inalcanzable desde la UI) (D) |
| DEC-509 | A9 salida tras cancelar | replicar la salida parcial (DP-09/DP-10) (D) |
| DEC-510 | A10 EOL | DP-02 (D) |
| DEC-511 | A11 valores de réplica | ESPEC_12 §4.2 regla 1 (D) |
| DEC-512 | A12 esquema de `Resultados` | modelo de filas propio del port (O2) (D) |
| DEC-513 | A13 bordes de `Calculos` (`GS-03`) | replicar (D) |
| DEC-514 | A14 `ObtenNumCols` | replicar (D) |

### ESPEC_06 (utils y análisis)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-601 | A1 cabeceras SharpZipLib | no se exigen los bytes de SharpZipLib: escritor estándar y lector tolerante (ESPEC_16 §3); se compara el contenido descomprimido en ambos sentidos (D) |
| DEC-602 | A2 `ToString()` e infinito/NaN · A4 parser numérico · A5 funciones de cadena | `G-NET-FMT`, `G-NET-PARSE`, `G-NET-TXT` (`bcl`) (D) |
| DEC-603 | A3 cultura | DP-01 (D) |
| DEC-604 | A6 `AnalizadorColumnaCombinacion` corta al primer fallo | replicar (D; R) |
| DEC-605 | A7 código muerto | DP-08 (D) |
| DEC-606 | A8 libm | la referencia es la libm de Darwin (la del producto Mac, la misma que usa Swift); diferencia de hasta 1 ulp frente a la UCRT de Windows aceptada (ESPEC_12 §1.1) (D) |
| DEC-607 | A9 `FigurasComparer` | solo WinForms ⇒ DP-08 (D) |
| DEC-608 | A10 `Comparador` con 15 signos | `Comparador` del dominio sin uso en la app ⇒ DP-08 (D) |
| DEC-609 | A11 rangos de propiedades de `ContenedorAnalisis` | replicar; si el C# lanza, error tipado (D) |
| DEC-610 | A12 `InfoJornadasLAE.txt` | corpus generado (ESPEC_12 §6) (D) |
| DEC-611 | A13 GoldenGen | ESPEC_12 (D) |
| DEC-612 | A14 EOL | DP-02 (D) |
| DEC-613 | A15 `Porcentajes` con N ≠ 14 | replicar (D) |
| DEC-614 | A16 `ConvLongToStr` con dobles/triples | replicar (error tipado) (D) |
| DEC-615 | A17 pleno al 15 real | replicar lo que hace hoy el boleto con la jornada online (O2) (D) |
| DEC-616 | A18 DEFLATE en Swift | ESPEC_16 §3: DEFLATE crudo con `Compression.framework` (`COMPRESSION_ZLIB`, RFC 1951) y contenedor ZIP propio; lector tolerante (método 0/8, descriptor de datos, campos extra y Zip64) (D) |
| DEC-617 | A19 desfase de documentación | sin efecto en la app Mac (D) |

### ESPEC_07 (formatos y online)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-701 | A1 EOL/BOM | DP-02 (D) |
| DEC-702 | A2 sin `.comb` reales · A3 exactitud de `XmlTextWriter` | corpus `fixture` + `lectura` (ESPEC_12 §6); exactitud de `XmlTextWriter` según `EscritorXMLNET` (ESPEC_16 §2) (D) |
| DEC-703 | A4 gramáticas delegadas | ESPEC_01–04 (D) |
| DEC-704 | A5 propiedades JSON duplicadas | gana la **última** definición, como `JsonElement.TryGetProperty` de .NET 8 (`Online/JornadaQuinielaParser.cs:63,87,129,146`; `bcl`); el parser Swift igual, con caso en la suite online (D) |
| DEC-705 | A6 enteros JSON y `Enum.TryParse` | `TryGetInt32` rechaza `38.0`, `1e2` y valores fuera de `Int32` (`Online/JornadaQuinielaParser.cs:154`); `Enum.TryParse(…, ignoreCase: true)` acepta cifras no definidas (`"99"`) (`WinUI/Services/TemaApp.cs:76`, `IdiomaApp.cs:77`) (`bcl`) (D) |
| DEC-706 | A7 equipos `.txt` antiguos | DP-02: lectura UTF-8 como hoy, sin *fallback* cp1252 (D) |
| DEC-707 | A8 `.lang` | DP-07: no se porta (D) |
| DEC-708 | A9 `FileIO.WriteLinesAsync` | sin Windows no se puede capturar: el port escribe UTF-8 sin BOM con CRLF (regla general de DP-02); los lectores aceptan cualquier variante (D) |
| DEC-709 | A10 valores por defecto de `HttpClient` | DT-16 (D) |
| DEC-710 | A11 nombres de `HttpStatusCode` | DT-17 (D) |
| DEC-711 | A12 `ToString("0.00")` | `G-NET-FMT` (`bcl`) (D) |
| DEC-712 | A13 semillas ya editadas | sin fusión: copia solo si falta, como hoy (D) |
| DEC-713 | A14 rutas absolutas heredadas | se conservan; semáforo rojo si no existen (ESPEC_11 §9) (D) |
| DEC-714 | A15 cultura de las muestras | DP-01 + corpus en `es-ES` (D) |
| DEC-715 | A16 sandbox | DP-06 (D) |
| DEC-716 | A17 impresión | se conservan `imprebol.cfg`/`impresoras.cfg` (los usa la app) con `NSPrintOperation` (D; DEC-407) |
| DEC-717 | A18 `Resultados.txt` | lector de las pantallas (O2) (D) |
| DEC-718 | A19 `DataSet.ReadXml` | `DataSet.ReadXml` con esquema ignora elementos y atributos que no están en él (`bcl`); el lector Swift lee el esquema conocido y se prueba con `Free1X2.WinUI/Datos/ColumnasProbables.xml` (`fixture`) (D) |
| DEC-719 | A20 desfase de `API_CLUBPROGOL.md` | fuera del alcance Mac (D) |
| DEC-720 | A21 carpeta `Idioma/` ausente | no aplica (`.lang` no se porta) (D) |
| DEC-721 | A22 DTD · A23 espacios en blanco | ESPEC_16 §2.2 (D) |
| DEC-722 | A24 NoVariantes `values=""` | replicar: error tipado (D) |

### ESPEC_08 (pantallas 1–40)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-801 | A1 modos combinados de Pleno al 15 | replicar (D; R) |
| DEC-802 | A2 `Grabar` de Análisis de grupos | port (DP-15) (D) |
| DEC-803 | A3 índices fuera de rango en Formatos 123 | replicar: error tipado (D) |
| DEC-804 | A4 estado «Pronóstico» pisado | replicar (D; R) |
| DEC-805 | A5 Banco de pruebas LN↔Premio | port (DP-15) (D) |
| DEC-806 | A6 puntos CP tras guardar | DP-04 (D) |
| DEC-807 | A7 peculiaridades de Coincidencias | replicar (D) |
| DEC-808 | A8 `IncluirPleno` sin efecto | replicar: se muestra y no actúa (R) |
| DEC-809 | A9 propiedades sin uso | no se portan las propiedades; nada visible cambia (DP-08) (D) |
| DEC-810 | A10 pronósticos CP de un carácter | replicar (D; R) |
| DEC-811 | A11 parseo de tolerancias/aciertos CP | replicar: error tipado (D) |
| DEC-812 | A12 `N2s` con ≠ 14 | replicar (D) |
| DEC-813 | A13 ZIP | DEC-601/DEC-616 (D) |
| DEC-814 | A14 ruta `.lst` en mayúsculas | replicar: en macOS (APFS sin distinción de mayúsculas por defecto) la ruta sigue resolviéndose (D) |
| DEC-815 | A15 diálogos 34/37 sin consumidor | replicar (R) |
| DEC-816 | A16 fila 10 de «Filtrar por límites» | replicar (D; R) |
| DEC-817 | A17 rejilla de Dibujos con 15 partidos | port: 15 fijo (DP-15) (D) |
| DEC-818 | A18 formatos de condición y TSV de 44 columnas | ESPEC_07 + corpus (D) |
| DEC-819 | A19 `System.Random` | `AleatorioNET` + O3 para semillas de reloj (D) |
| DEC-820 | A20 carpetas y portapapeles de condiciones | DP-13 + fichero `Temp/tmp.<ext>` como hoy (D) |
| DEC-821 | A21 productor de `ArrayFormatos` | payload de navegación desde la pantalla 51 (ESPEC_11 §8.2) (D) |
| DEC-822 | A22 rama «en pantalla» inalcanzable | replicar el mensaje (R) |

### ESPEC_09 (pantallas 41–73)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-901 | A-41-1, A-41-2 divergencias con el legacy | port (DP-15) (D) |
| DEC-902 | A-41-3 botón «CPs por diferencias…» sin cablear | replicar (R) |
| DEC-903 | A-42-1 formato `.dif` | ESPEC_07 + corpus (D) |
| DEC-904 | A-43-1 la ayuda promete espacios | replicar: con espacios, error inesperado como hoy (R) |
| DEC-905 | A-44-1 comodín `*` → `X` | replicar (D) |
| DEC-906 | A-44-2 carpeta inicial | Documentos, como el port (D) |
| DEC-907 | A-45-1 `Detener` sin control | no se expone, como hoy (D) |
| DEC-908 | A-45-2 `S: Suspendido` | replicar (D) |
| DEC-909 | A-45-3 fila TOTALES exportable | replicar (R) |
| DEC-910 | A-46-1 redondeo bancario · A-46-2 dependencia de la recaudación previa | replicar: `Math.Round(x, n)` de .NET 8 = `x·10ⁿ` redondeado a par ÷ `10ⁿ` (`bcl`, `Math.cs`); la dependencia de la recaudación previa se replica (D) |
| DEC-911 | A-46-3 «Destinado a premios» sin uso | se muestra y no calcula (D) |
| DEC-912 | A-46-4 premios guardados ≠ tecleados | replicar (R) |
| DEC-913 | A-47-1 caracteres ≠ `1/X/2` | replicar con la receta T1 (D) |
| DEC-914 | A-49-1 editor de figuras (0–16, `*`) | replicar el comportamiento real (R) |
| DEC-915 | A-50-1 «Cancelar» inoperante | DP-10: igual que hoy (D) |
| DEC-916 | A-50-2 sin entrada de menú | igual que hoy: se abre desde el Generador de CPs (D) |
| DEC-917 | A-51-1 dos `TransformarValoracion` | una implementación, mismo resultado (NaN incluido) (ESPEC_13 §3) (D) |
| DEC-918 | A-53-1 tramos pierden sobrantes | replicar (D; R) |
| DEC-919 | A-54-1 salida por defecto = entrada | replicar (R) |
| DEC-920 | A-54-2 comparación con legacy | no aplica (DP-15) (D) |
| DEC-921 | A-55-1 dígitos por redondeo | replicar (D; R) |
| DEC-922 | A-57-1 Filtro Pim = motor aidomnou | port: mismo motor (DP-15; ESPEC_13 §3) (D) |
| DEC-923 | A-57-2 `aidomnou.cfg` | constantes como el port; no se expone (D) |
| DEC-924 | A-58-1 desalineamiento de `cM` | replicar (D; R) |
| DEC-925 | A-59-1 `Porcentajes`/tablas CP | versión del cuerpo del VM (O2) (D) |
| DEC-926 | A-60-1 codificación de `equipos*.dat` | Latin-1 como el port (DP-02) (D) |
| DEC-927 | A-62-1 «Abrir» no llega al analizador · A-62-2 confirmaciones omitidas | port (DP-15) (R) |
| DEC-928 | A-63-1 escritura de valoraciones doble y formato 1 | una implementación; inconsistencia del formato 1 replicada (D; R) |
| DEC-929 | A-64-1 «Nuevo…» crea fichero vacío | DT-18 (D) |
| DEC-930 | A-64-2 `LeeColumnaSinComas` | replicar (D) |
| DEC-931 | A-65-1 contrato textual If-Then | replicar el parser de `ControladorIfThen` tal cual (ESPEC_01) (D) |
| DEC-932 | A-66-1 `ObtenNumCols` estimado | replicar (D) |
| DEC-933 | A-68-1 unidades de impresión | DEC-407 (D) |
| DEC-934 | A-68-2 `SetConfig` y «Girar boleto» | port: sin `SetConfig` inicial; girar sin efecto (DP-15) (R) |
| DEC-935 | A-71-1 BOM de listados | = DEC-708: UTF-8 sin BOM, CRLF (D) |
| DEC-936 | A-72-1 sin cancelación en Mejores opciones | DP-10 (D) |
| DEC-937 | A-73-1 bucle infinito latente | DT-08 (D) |
| DEC-938 | A-73-2 modo aleatorio | `Entorno.aleatorio` + O3 (D) |
| DEC-939 | G-1 oráculo port/legacy · G-2 carpetas · G-3 cultura · G-4 golden de ViewModels | DP-15 · DP-13 · DP-01 · ESPEC_12 §5 (D) |

### ESPEC_10 (pantallas 74–108 y shell)

| DEC | Origen | Decisión |
|-----|--------|----------|
| DEC-1001 | A-01 multiventana | DP-05 (D) |
| DEC-1002 | A-02 menú Ayuda · A-05 enlaces de ayuda | DT-19 (D) |
| DEC-1003 | A-03 preferencias | DP-12, sin migración (D) |
| DEC-1004 | A-04 localización | DP-07 (D) |
| DEC-1005 | A-06 `Ganadoras/` | DP-13 (D) |
| DEC-1006 | A-07 smoke | ESPEC_14 §9 (D) |
| DEC-1007 | A-08 el visor pierde el análisis al salir | replicar (R) |
| DEC-1008 | A-09 14 secciones del visor | replicar (D) |
| DEC-1009 | A-10 manual vs código | el código (DP-15) (D) |
| DEC-1010 | A-11 `ReductorTM` no seleccionable | DP-08 (= DEC-507) (D) |
| DEC-1011 | A-12 ganadora fija en Tríos | replicar (R) |
| DEC-1012 | A-13 estado sobrescrito en Rotación | replicar (R) |
| DEC-1013 | A-14 límite de 15 signos y niveles | replicar (D) |
| DEC-1014 | A-15 formato .NET | `G-NET-FMT` (`bcl`) (D) |
| DEC-1015 | A-16 cancelación | DP-10 (D) |
| DEC-1016 | A-17 boletos +1 con múltiplos de 8 | replicar (D; R) |
| DEC-1017 | A-18 «Valor» sobre el filtro vivo | replicar (CU-C46) (D) |
| DEC-1018 | A-19 formato y orden en Estadísticas | `Float.ToString()` como hoy (ida y vuelta más corta, `bcl`) + orden DT-14 (D) |
| DEC-1019 | A-20 `CalcularValorTotal` | DP-08 (D) |
| DEC-1020 | A-21 `aidomnou.cfg` | = DEC-923 (D) |
| DEC-1021 | A-22 diferencias port/legacy | DP-15 (D) |
| DEC-1022 | A-23 motor aidomnou duplicado | = DEC-922 (D) |
| DEC-1023 | A-24 dos visores | se mantienen separados (D) |
| DEC-1024 | A-25 recuento 105/108 | informativo (D) |

## 4. Componentes que no se portan (DP-08)

`XFSF`, `XfsfV3`, `JLPM`, **`ReductorTM`** (no seleccionable), `EscrutadorComb.PonerPremios/AñadirPremiosGlobales`,
`PosiblesPremiosContenedor.CalcularValorTotal`, `Comparador`, `Comparador2`, `FigurasComparer`, `TransponedorAutomatico`, `clAnalisisColumnas`,
`ContenedorValoraciones`, `Analisis.Analizador.IteraCols`, `Diferencia.AnalizarFallos`, `IO`/`GCP` muertos (04 A-10), funciones de `UtilColumnas`
con 0 consumidores (06 §1.4 y A7), `IAppPaths`, `IProgressNotifier`, `UiHooks` (sustituido por `Entorno`), `Signo`/`ReduccionTipo`/`AlgoritmoReduccion`,
`ArchivoIdioma`/`.lang`, `aidomnou.cfg`, `Comunicaciones/`, propiedades sin lectura de 08 A9. Sus tests y suites golden no se portan ni generan.

## 5. Candidatas a corrección después de la paridad (estado R)

Se replican tal cual; el dueño decide si alguna se corrige **después** de alcanzar la paridad (cada corrección = cambio de golden explicado):
DEC-203, 302, 306, 404, 604, 801, 804, 808, 810, 815, 816, 822, 902, 904, 909, 912, 914, 918, 919, 921, 924, 927, 928, 934, 1007,
1011, 1012, 1016; y la mejora de cancelación de DP-10.

## 6. Decisiones del índice M0 y del plan

| Origen | Estado |
|--------|--------|
| README D1 | resuelta (2026-10-12) = DP-15 |
| README D2 | DP-09 + suite `G-D2` (D) |
| README D3 · D4 · D5 · D7 · D8 | DP-08 · DP-01 · DP-02 · DP-11 · DP-06 |
| README D6 | resuelta (2026-10-12; 2026-10-14): corpus = ficheros del repo + ficheros escritos a mano leyendo los escritores (ESPEC_12 §6) |
| PLAN §7.1 aprobar plan y arquitectura | arquitectura en ESPEC_11; revisión final del dueño |
| PLAN §7.2 ubicación | DP-14 |
| PLAN §7.3 distribución | DP-06 |
