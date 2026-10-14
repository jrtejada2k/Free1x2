# Revisión de las especificaciones M0 para la reescritura Swift

> Rama `claude/kind-hawking-1lz017` @ `1e39740` · revisión del 2026-10-12.
> Alcance: **solo la app Mac (Swift/Xcode)**. No se toca nada de .NET; el C# solo se cita como
> referencia de comportamiento (D1). Documento de **hallazgos y opciones**: ninguna decisión se
> toma aquí (R2). Nada se ejecutó; cada afirmación cita `fichero:línea` (R4).

**Material revisado (lectura íntegra):** `docs/PLAN_MIGRACION_MAC.md` (116 líneas), `docs/mac/README.md`
y `ESPEC_00` … `ESPEC_10` (~13 400 líneas). Rutas de este documento relativas a `docs/` salvo que se diga otra cosa;
`NN:línea` abrevia `mac/ESPEC_NN_*.md:línea`.

---

## 1. Veredicto

| Pregunta | Estado | Severidad |
|----------|--------|-----------|
| ¿Está especificado **qué** hace hoy la app (motor, formatos, 108 pantallas)? | **Sí, con mucho detalle.** Cobertura nominal 143/143 ficheros del Domain y 108/108 páginas. | — |
| ¿Están **verificados** los valores esperados? | **No.** Salen de réplicas Python; solo 3 componentes coinciden con golden reales (README:25-28). Se regeneran en M1. | Alta |
| ¿Está diseñada la **arquitectura Swift** (el **cómo**)? | **No.** El plan le dedica ~30 líneas (PLAN:50-73); las specs proponen piezas sueltas y contradictorias entre sí y con el plan. | **Bloqueante** |
| ¿Es **fácil de mantener y probar** lo propuesto? | **No definido.** No hay estrategia de pruebas, puntos de inyección, CI ni presupuesto de rendimiento. | **Bloqueante** |
| ¿Está listo el **oráculo** (M1)? | **No.** `tools/GoldenGen` no existe ni tiene spec propia; los ~193 casos de uso de ViewModels no tienen oráculo; choca con «nada en .NET». | **Bloqueante** |
| ¿Está **diseñada la UI** (M-R2)? | **Parcial.** ESPEC_10 §1 propone shell, navegación, comandos y paleta; **no hay ni un mockup**, aunque el plan los exige en M0 (PLAN:17,92). | Alta |
| ¿Están **cerradas las decisiones** del dueño? | **No.** ~200 «Abiertas» repartidas en 10 documentos, con temas repetidos hasta 7 veces y sin registro único. | Alta |

**Conclusión:** M0 está completa como **especificación de comportamiento**, pero **no** como base de
diseño. Antes de escribir código Swift (M2) faltan: un documento de arquitectura, uno de oráculo/pruebas,
la API de casos de uso, el registro único de decisiones y los mockups (§4).

---

## 2. Fortalezas (conservar)

- **Precisión de comportamiento.** Pseudocódigo en el orden real de llamadas, `fichero:línea` en cada afirmación,
  marcas `[PRESERVAR]`, «Peculiaridades a preservar» y «Abiertas» al final de cada spec.
- **Trampas C#→Swift** catalogadas (T1–T12, 00:42-53): overflow, desplazamientos, `Float` vs `Double`,
  redondeo bancario, ordenación inestable, cultura, EOL/BOM, división entera, estado global, bombeo de UI.
- **ESPEC_04** etiqueta el origen de cada valor esperado (TST/SIM/CALC): buena práctica que conviene extender.
- **ESPEC_05 §5** mide la cobertura real de los 138 tests (qué está fijado y qué no).
- **ESPEC_06 §1** modela la columna (3 bits, familias de codificación, capacidades por N) y advierte de no unificar codificaciones distintas.
- **ESPEC_07** verifica formatos sobre bytes reales, inventaría los 13 puntos dependientes de cultura (C1–C13) y la política EOL/BOM por formato.
- **ESPEC_10 §1** ya contiene el embrión de la arquitectura de app: arranque, navegación (`NavigationSplitView`, 10 áreas),
  menús → comandos nativos, barra personalizable, paleta de tokens claro/oscuro, alertas, paneles de fichero, log, hooks → protocolos, smoke.

---

## 3. Hallazgos

Severidad: **B** = bloqueante para empezar M2 · **A** = alta (afecta diseño o pruebas) · **M** = media (consistencia) · **b** = baja (redacción).

### H-01 · B · No existe especificación de arquitectura Swift

- El plan resume la arquitectura en un árbol de carpetas y 6 viñetas (PLAN:50-73). Ninguna spec cubre:
  `Package.swift` y grafo de módulos (`Free1X2Core`, `Free1X2Formats`, `Free1X2Online`, app), **API pública** de cada módulo
  y niveles de acceso, modelo de datos (valor vs referencia), modelo de errores, modelo de concurrencia/cancelación/progreso,
  inyección de dependencias, persistencia y modelo de documento, navegación, logging, nombres canónicos.
- Búsqueda en las 10 specs + plan: `Package.swift` 0 apariciones; «inyección/DI» 0; `XCTest` 0; `@Test` 0; *snapshot* 0.
- Las decisiones de diseño existentes están **dispersas** dentro de specs de comportamiento (p. ej. `unowned let filtro` en 04:100,
  `Configuracion`/`Sesion` en 01:752-770, `EditorCondicion` en 08:123 y 10:308, `actor QuinielaOnlineService` en 07:816).
- **Impacto:** cada fase M2–M5 reinventaría la estructura; no hay contrato contra el que revisar ni probar.

### H-02 · B · El plan y las specs se contradicen en el modelo de datos

| Plan | Specs |
|------|-------|
| «Structs `Sendable`, sin estado global» (PLAN:56) | Filtros como clases con estado: `protocol FiltroProtocol: AnyObject` (01:777), `protocol FiltroColumna: AnyObject` + «deben ser `final class`» (03:52) |
| «Sin clases ni existenciales en el camino crítico» (PLAN:69) | `[any FiltroProtocol]` (01:332) recorrido por columna (01:372); relaciones CP «`final class` … **nunca structs**» (04:206); `weak var` para romper ciclos del grafo C# (01:174) |

- Las specs tienen razón en que el **comportamiento** depende de estado mutable compartido (contadores que deja `analizar`
  y leen otros, 01:793; listas compartidas por referencia, 04:206). El plan tiene razón en que eso es difícil de probar y de paralelizar.
- **Opciones (decide el dueño):** (a) núcleo fiel con `final class` internas detrás de una **fachada pública de valores `Sendable`**
  (entrada: configuración + columnas; salida: resultados/contadores), y corregir PLAN:56,69; (b) rediseño funcional puro
  (cada filtro devuelve un `struct` de contadores), más testeable pero con más riesgo de romper la paridad byte a byte (02 §0.3 lo desaconseja);
  (c) dejar el plan como está (no recomendado: contradice las specs que lo implementan).

### H-03 · M · Nombres y tipos incoherentes entre specs

| Concepto | Variantes encontradas |
|----------|----------------------|
| Protocolo de filtro | `FiltroProtocol` (01:777) · `FiltroColumna` (03:52) |
| Enum de filtros | `enum Filtro` (01:652; 07:983) · `enum NombreFiltro: String` (02:63) |
| `obtenNoAciertosTolerancias` | `throws -> Int` (01:786) · `-> Int` sin `throws` (02:64) |
| `Sesion` | `final class` de motor con `configuracion`/`anfitrion`/`rutas` (01:760) · `@MainActor @Observable final class` = `AppState` (07:1162) · `@Observable final class` + `struct Configuracion` + `RepositorioConfiguracion` (08:41-43) · `@State var sesion = Sesion()` en la `App` (10:46) |
| Editor de condición | `EditorCondicion { cargar(desde:), aplicar(a: inout Grupo), exportar, importar }` (08:123) · `EditorCondicion { associatedtype Filtro; borrador; aceptar(en: inout Grupo); filtroTemporal(); estadisticas() async }` (10:308). Ambos tratan `Grupo` como **valor** (`inout`) mientras 01/03/04 lo modelan como clase con referencias |
| Columna | `typealias Columna = Int64` (06:209) · alias propios de índice/bits en 05:810 |
| Errores | `enum ErrorColumnas` (06:220) · `throws(ErrorFormato)` (07:1112) · sin tipo global (01:873) |
| ID de casos de uso | `CU-A01…A90` (08:17) · `CU-B01…B41` (09:1990-2032) · `CU-01…CU-62` **sin prefijo** (10:20,1883-1944) |
| Estilo de nombre de CU | tipo-sustantivo (`AgregarPleno15`, 08:1938) · tipo + métodos (`MejoresOpciones.generarGanadoras`, 09:1850) · verbo+objeto (`MultiplicarColumnas`, 10:1883) |

- Referencia cruzada rota: 10:1877 remite a «A-24» para el motor aidomnou/Pim; el asunto está en **A-23** (10:1983).
- **Propuesta:** `GLOSARIO.md` con un nombre canónico por concepto y renombrar `CU-nn` de ESPEC_10 a `CU-Cnn`.

### H-04 · B · Política de errores sin decidir y contradictoria

- Motor: «replicar la excepción» (`[PRESERVAR]` en 01/02) · «rechazar la configuración inválida al cargar» (03:138, B7) ·
  «validar con `throws` al fijar; Swift **no** reproduce los fallos del C#» (04:1300, A-1) · abierta (01:873, A7).
- UI: «sin `fatalError`, acceso seguro» (10:380) · «error de validación en lugar de abortar» (10:425) · replicar excepciones no capturadas que hoy acaban en el diálogo global (10:32).
- Oráculo: 06 §4 pide serializar errores como `{"error":"<Tipo>"}` (06:1015), lo que implica **replicarlos**.
- **Impacto:** cada componente lo resolverá distinto; los golden de error no se pueden definir.
- **Opciones:** (a) errores tipados por módulo que **reproducen** cada excepción C# (paridad total, golden de error);
  (b) validación en la frontera (UI/carga) + `precondition` interno, sin golden de error; (c) mixto: motor replica, UI valida antes.

### H-05 · A · Concurrencia, progreso y cancelación sin modelo común

- Solo hay reglas genéricas: `AsyncStream` + `TaskGroup` preservando orden (PLAN:66), T12 (00:53, 05:819).
- Casos sin resolver: salida **parcial** al cancelar (05:836, A9); `ReductorTM` no cancelable (05:834); «Cancelar» inoperante en
  varias pantallas (09:2056 A-50-1; 10:1807; 10:1976 A-16); cálculos de 3^k sin límite (09:2078 A-72-1; 10:704);
  contadores compartidos sin sincronización (10:828); arrays de 4 782 969 elementos (≈57 MB, 10:532; 38 MB, 10:1079).
- **Falta:** un contrato único para operaciones largas (firma `async`, tipo `Progreso`, semántica de cancelación y de salida parcial,
  actor/aislamiento del motor) y la decisión de si «cancelar de verdad» es una mejora aceptada (10:1976).

### H-06 · B · Oráculo M1 incompleto

1. **GoldenGen no existe** (04:1312; 06:1042) y su única especificación está enterrada en 06 §4 (06:1015). Falta un documento propio:
   formato de los golden (JSON/binario), layout en `Packages/Free1X2Kit/Tests/Golden/` (PLAN:99), nombres, cómo los carga Swift,
   comparación (bytes tras normalizar EOL; ZIP por contenido), versionado y regeneración.
2. **Choca con «nada en .NET».** M1 exige escribir y ejecutar C# una vez (PLAN:80). Decisión del dueño: hacerlo como excepción acotada
   (herramienta de datos fuera de la app) o sustituirlo. `Free1X2.Domain` es `net8.0` portable (`Free1X2.Domain/Free1X2.Domain.csproj:4`),
   así que podría correr en este Mac con el runtime .NET 8 o en Windows.
3. **Dónde corre, contradictorio:** «en Windows, para fijar cultura y CRLF» (README:47-48) frente a «en Mac, normalizando EOL» (05:837 A10; 07:1195 A1; 06:1037 A8 pide ambos para `libm`).
4. **Valores esperados sin verificar:** réplicas Python (README:25-28; 02:8; 03:30) y ese port de verificación **no está versionado**.
5. **Casos de uso de ViewModels sin oráculo:** 90 + 41 + 62 = **193 CU** viven en ViewModels WinUI (`net8.0-windows`), sin tests ni golden
   (README:30; 09:2084 G-4). Opciones: (a) extraer su lógica a una biblioteca C# sin UI (es trabajo .NET); (b) capturar golden ejecutando
   la app WinUI en Windows; (c) aceptar la spec como oráculo + revisión del dueño + pruebas de propiedades (sin paridad de bytes).
6. **Aleatoriedad y reloj:** `System.Random` con semilla de reloj (08:2055 A19) y modo aleatorio por milisegundos (09:2080 A-73-2) → RNG y reloj inyectables y decidir qué se compara.
7. **Corpus:** no hay ningún `.comb` real (07:1196 A2); D6 se resolvió «GoldenGen los genera» (README:41), pero el cierre de M4 sigue siendo
   «round-trip con ficheros reales» (PLAN:102) → redefinir el criterio de M4.

### H-07 · A · Decisiones del dueño repetidas y sin registro único

Mismo tema preguntado en varios sitios, a veces con propuestas distintas:

| Tema | Dónde aparece | Conflicto |
|------|---------------|-----------|
| **Cultura** (D4) | 01:875 A9 · 03:902 A1 · 05:815 · 06:1032 A3 · 07:1209 A15 · 09:2083 G-3 · 10:1975 A-15 | README:39 propone `es-ES`; 05:815 propone `en_US_POSIX` |
| **Nº de partidos** (14/15/16) | 02:996 A2 · 03:908 A7 · 04:1301 A-2 · 06:1039 A10 · 06:1044 A15 · 10:1974 A-14 (3^15) · 10:1244 | Sin rango soportado definido; varios componentes fijan 14 o 3^15 |
| **EOL/BOM** (D5) | 05:837 A10 · 07:1195 A1 · 09:2077 A-71-1 · 10:382 | — |
| **Sandbox/distribución** (D8) | PLAN:112-116 · 07:1210 A16 · 10:250 | Condiciona rutas, *bookmarks* y diseño de ficheros |
| **Ventana / documento** | 10:89-90 · 10:1956 A-01 | `DocumentGroup` vs `WindowGroup`; una o varias `Sesion` |
| **Recarga de configuración** | 01:874 A8 | — |
| **Alcance de localización** | PLAN:73 · 07:679 · 10:1959 A-04 | PLAN:73 dice «`en-US.lang` como fuente», pero hoy no se aplica (07:679) y solo Créditos está localizado (10:214) |
| **Código muerto** (D3) | README:38 · 06:1036 A7 · 10:1980 A-20 · 10:1971 A-11 | — |

- Recuento de «Abiertas»: 01=12 · 02=10 · 03=13 · 04=13 · 05=14 · 06≈18 · 07=24 · 08=22 · 09=45 · 10=25 → **≈196**.
- **Propuesta:** `DECISIONES.md` con ID global, temas fusionados, estado (abierta/decidida), decisión y fecha; las specs enlazan al ID.

### H-08 · M · Texto desfasado respecto a decisiones ya tomadas

- **D1** (solo WinUI + Domain, README:36) no se propagó: 09:20 y 09:2081 (G-1) siguen diciendo «decisión abierta»; 10:1982 (A-22) «decidir cuál es la referencia»;
  divergencias con el legacy aún planteadas como pendientes en 09:2040-2041, 09:2063, 09:2069, 09:2076, 09:2077, 10:1972.
- **D6** (sin muestras del dueño, README:41) frente a 07:1196 (A2) y PLAN:102.
- **M0 «completa»** (commit `52589c9`) aunque PLAN:92 dice que los mockups se entregan en M0/M4.

### H-09 · M · Trampas duplicadas y dos que faltan

- Cuatro catálogos solapados: T1–T12 (00:42-53), B1–B7 (03:130-138), S1–S12 (desde 04:204), tabla T de 05:809-819. Conviene uno solo en ESPEC_00.
- **Falta T13 — `Double` no finito → entero:** `Int(Double.nan)` aborta en Swift; en C# da un valor basura que hoy produce un mensaje (10:381, 10:829, 10:1341, 10:1375).
- **Falta T14 — formatos numéricos de .NET** más allá de la cultura: `F0/F2` redondea «half away from zero» (10:1048, 10:1477), `#,##0.00;0.00` (10:782),
  `Float.ToString()` más corto (10:1701, 10:899), `"r"` (10:1513), `TimeSpan.ToString()` (10:588, 10:808). 10:1975 (A-15) admite que está descrito «por conocimiento», sin ejecutar.

### H-10 · A · Piezas transversales sin spec propia (y duplicaciones a evitar)

- **`FormatoNET`** (parseo/formato .NET-compatible: cultura, T13, T14) — inventario en 07 (C1–C13) pero sin API ni tabla golden propia.
- **`DocumentoXML`** — réplica byte a byte de `XmlTextWriter` (orden de atributos, comentarios): pieza grande descrita solo dentro de 07.
- **ZIP/DEFLATE `.z3q`** — dependencia por decidir (libz, `Compression.framework`, ZIPFoundation; 06 §2.14).
- **Texto Latin-1/cp1252** — `equipos*.dat` (09:2067 A-60-1), equipos antiguos (07:1201 A7).
- **Duplicaciones en el C# que no deben pasar a Swift:**
  - Codificación base-3 local «X=0, MSB primero», idéntica en 5 pantallas: Pares (10:578), Premiadas (10:741), SelecJM (10:1002), SelectorMS (10:1064), aidomnou (10:1804) → un único helper (distinto de `ConvertidorDeBases`, que no se unifica, 06 §1).
  - Quicksort propio idéntico en 3 pantallas (10:523, 10:885, 10:1297) → una implementación.
  - Motor aidomnou duplicado en Filtro Pim (09:2063 A-57-1; 10:1983 A-23).
  - `TransformarValoracion` doble (09:2058 A-51-1); escritura de valoraciones doble (09:2070 A-63-1).

### H-11 · B · API de casos de uso no especificada

- Los catálogos son nombre + una línea (08:1938-2027; 09:1990-2032; 10:1881-1944). No hay firma Swift, tipos de entrada/salida, errores,
  progreso/cancelación ni dependencias (ficheros, reloj, RNG, portapapeles).
- 09:1988 afirma que todos son «funciones puras o `async` cancelables sobre `Sesion`, `Configuracion`», pero eso no está definido en ningún sitio,
  y 10:261 propone inyectar un `Avisador` (UI) en los casos de uso.
- Falta también el patrón de presentación (cómo una vista/modelo de vista consume un CU y cómo se prueba sin UI).

### H-12 · A · Diseño de UI incompleto

- **Existe** (10 §1): arranque (10:45-52), `NavigationSplitView` con 10 áreas (10:83-90), menú → comandos Mac (10:98-133), barra personalizable y atajos (10:180-183),
  paleta de tokens (10:193-206), controles reutilizables (10:276-285), editor de condición genérico (10:308-310).
- **Falta:** mockups (exigidos por M-R2, PLAN:17,92), decisión D7 de área/presentación para 108 pantallas (README:42), catálogo de estados vacío/cargando/error,
  accesibilidad (solo el semáforo tiene etiquetas, 10:282), mapa completo de atajos, modelo ventana/documento (10:1956), y qué «mejoras» de UX se aceptan
  (cancelar real, validación en vivo, confirmaciones) frente a replicar el comportamiento actual.

### H-13 · B · Estrategia de pruebas y mantenibilidad sin definir

- Solo existe: «CI macOS: `swift test` + `xcodebuild`» (PLAN §3) y una propuesta de smoke (10:271, abierta en 10:1962 A-07).
- **Falta:** pirámide de pruebas (golden por bytes, unitarias, propiedades, modelos de vista, UI), **puntos de inyección** (sistema de ficheros,
  reloj —hay «Tiempo» en ≥6 pantallas—, RNG, `URLSession` del `actor … .shared` de 07:816, portapapeles 10:262, avisos), criterios de cobertura por módulo,
  y **presupuesto de rendimiento** medible: el plan pide «igual o más rápido que Windows» (PLAN:69) sin conjunto de datos, máquina ni umbral.

### H-14 · A · Datos de usuario y migración desde Windows

- Carpetas Mac propuestas (07; 10:48) sin decisión; `Ganadoras/` se crea vacía y no se sabe qué la alimenta (10:1961 A-06), aunque 13 condiciones calculan estadísticas sobre ella (10:1674).
- Ficheros Windows existentes: CRLF, cp1252/Latin-1, rutas absolutas Windows dentro de `.comb` (07:1208 A14), coma decimal (07:1209). Falta una política de importación.
- Preferencias de UI: `UserDefaults` sin compatibilidad (10:1958 A-03) — pendiente de confirmar.

---

## 4. Documentos que faltan (propuesta; el dueño decide cuáles y en qué orden)

| Doc propuesto | Contenido mínimo | Cierra |
|---------------|------------------|--------|
| `ESPEC_11_ARQUITECTURA_SWIFT.md` | Módulos y `Package.swift`; dependencias permitidas; API pública por módulo; modelo de datos (H-02); errores (H-04); concurrencia/progreso/cancelación (H-05); `Sesion`/`Configuracion` únicas; puntos de inyección; persistencia y documento; navegación; logging | H-01, H-02, H-04, H-05 |
| `ESPEC_12_ORACULO.md` | GoldenGen (o su sustituto): dónde y cómo corre, formato, layout, carga en Swift, regeneración; estrategia para los 193 CU; RNG/reloj; corpus | H-06 |
| `ESPEC_13_CASOS_DE_USO.md` | Plantilla de firma + tabla de los 193 CU con entradas, salidas, errores, `async`/progreso, dependencias | H-11 |
| `ESPEC_14_PRUEBAS.md` | Pirámide, dobles de prueba, CI, cobertura, benchmarks y memoria | H-13 |
| `ESPEC_15_DISENO_UI.md` + mockups | Sistema de diseño (partiendo de 10:193-206), mockups por área, estados, accesibilidad, atajos | H-12 |
| `DECISIONES.md` | Registro único de las ≈196 Abiertas, fusionadas | H-07, H-08 |
| `GLOSARIO.md` | Nombre canónico por concepto e ID de CU | H-03 |
| ESPEC_00 ampliada | Catálogo único de trampas + T13/T14; plantilla «diseño Swift» por componente | H-09 |
| `ESPEC_07` (apartados) o docs propios | `FormatoNET`, `DocumentoXML`, ZIP, codificaciones de texto | H-10 |

Correcciones menores a specs existentes (solo con tu OK): actualizar textos desfasados (H-08), la referencia rota de 10:1877 y PLAN:56,69,73,92,102 según lo que decidas.

---

## 5. Decisiones bloqueantes (orden sugerido)

Opciones resumidas; la recomendación es solo una sugerencia (R2).

| # | Decisión | Opciones | Sugerencia |
|---|----------|----------|------------|
| 1 | **Oráculo y «nada en .NET»** (H-06.1-3) | Ejecutar GoldenGen una vez como excepción (Windows o Mac con .NET 8) · sin C#: oráculo = spec + revisión | Excepción acotada, fuera de la app, ejecutada una sola vez |
| 2 | **Oráculo de los 193 CU de ViewModels** (H-06.5) | Extraer a C# · capturar en la app WinUI · spec + propiedades | Decidir por CU según riesgo (los de cálculo masivo con golden) |
| 3 | **Política de errores** (H-04) | Replicar · validar en frontera · mixto | Mixto |
| 4 | **Modelo de datos** (H-02) | Clases internas + fachada de valores · funcional puro · plan tal cual | Clases internas + fachada; corregir PLAN:56,69 |
| 5 | **Cultura** (D4, H-07) | `es-ES` · `en_US_POSIX` · por formato | Una tabla por formato en `FormatoNET` |
| 6 | **EOL/BOM al escribir** (D5) | CRLF · LF · por formato | — |
| 7 | **Nº de partidos soportado** | Solo 14 · 14 y 15 · variable | — |
| 8 | **Distribución / sandbox** (D8) | Fuera de la App Store sin sandbox · con sandbox | — |
| 9 | **Ventana/documento** (10:1956) | `DocumentGroup` · una ventana | — |
| 10 | **Replicar o corregir defectos** (D2 y los «Defecto aparente» de 08/09/10) | Replicar todo (M-R1) · corregir tras paridad · caso a caso | Replicar en motor; política explícita para UI |
| 11 | **Mejoras de UX** (cancelar real, validación en vivo) | Aceptar en bloque · caso a caso | — |
| 12 | **Localización** | Solo es · es + en completo | — |
| 13 | **Código muerto** (D3) | No portar · portar | — |
| 14 | **Áreas y mockups** (D7, M-R2) | — | Requisito previo a M5 |

---

## 6. Qué no cubre esta revisión

- No se ejecutó nada (ni C# ni Python): las observaciones «por lectura» de las specs siguen sin verificar.
- No se comprobó contra el código C# cada `fichero:línea` citado por las specs; solo la coherencia entre documentos y con el plan.
- No se evaluó la viabilidad del calendario por fases.

---

## 7. Seguimiento (2026-10-13)

Por mandato del dueño (producto = app actual; técnica = buenas prácticas; revisión final del dueño) se cerraron todos los hallazgos:

| Hallazgo | Cerrado en |
|----------|-----------|
| H-01 arquitectura · H-02 modelo de datos · H-04 errores · H-05 concurrencia · H-14 datos de usuario | `ESPEC_11_ARQUITECTURA_SWIFT.md` (+ PLAN §2 corregido) |
| H-03 nombres | `GLOSARIO.md` (+ `CU-nn` → `CU-Cnn` en ESPEC_10; referencia A-24 → A-23) |
| H-06 oráculo | `ESPEC_12_ORACULO.md` |
| H-07 decisiones repetidas · H-08 texto desfasado | `DECISIONES.md` (+ encabezados de «Abiertas», README, PLAN) |
| H-09 trampas | `ESPEC_00_CONVENCIONES.md` §3 (T1–T18 y equivalencias) |
| H-10 piezas transversales | `ESPEC_16_PIEZAS_TRANSVERSALES.md` (+ unificaciones de ESPEC_13 §3) |
| H-11 casos de uso | `ESPEC_13_CASOS_DE_USO.md` |
| H-12 diseño de UI | `ESPEC_15_DISENO_UI.md` |
| H-13 pruebas | `ESPEC_14_PRUEBAS.md` |

Las 14 decisiones de §5 quedan tomadas en `DECISIONES.md` (1 = ESPEC_12 + regla del dueño; 2 = ESPEC_12 §5; 3 = DT-03; 4 = DT-02;
5 = DP-01; 6 = DP-02; 7 = DP-03; 8 = DP-06; 9 = DP-05; 10 = DP-09; 11 = DP-10; 12 = DP-07; 13 = DP-08; 14 = DP-11 + ESPEC_15).
