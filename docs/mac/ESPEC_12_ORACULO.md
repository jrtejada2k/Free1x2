# ESPEC 12 — Oráculo por lectura: casos de referencia (golden) y origen de cada esperado (fase M1)

> Cierra H-06 de [`REVISION_SPECS.md`](REVISION_SPECS.md). Arquitectura en [ESPEC_11](ESPEC_11_ARQUITECTURA_SWIFT.md),
> pruebas en [ESPEC_14](ESPEC_14_PRUEBAS.md). Citas `NN:línea` = `mac/ESPEC_NN_*.md:línea`.
>
> **Regla del dueño (2026-10-14, definitiva):** la app original (`Free1X2.WinUI`, `Free1X2.Domain`, `Free1X2.Domain.Tests`) **solo se
> lee** para obtener requisitos, especificación y diseño. **No se ejecuta nada de ella**: ni la app, ni sus tests, ni ninguna herramienta
> que compile o llame a su código (no existe `tools/GoldenGen`). El «oráculo» es **el código C# leído** y las specs que lo documentan
> con `file:line`. Los golden son **datos escritos a mano** (con ayuda de modelos de la spec) y versionados en el paquete Swift.

## 1. De dónde sale cada esperado

Todo caso golden lleva el campo `fuente` con **una** de estas etiquetas; no hay otra forma válida de obtener un esperado.

| `fuente` | Origen | Requisito para aceptarlo |
|----------|--------|--------------------------|
| `test` | Literal esperado de un test C# **leído** (`Free1X2.Domain.Tests/<fichero>.cs:<línea>`) | cita exacta; el literal se copia tal cual |
| `fixture` | Fichero de datos que ya está en el repo, copiado byte a byte: `Free1X2.Domain.Tests/Fixtures/**`, `Free1X2.WinUI/Datos/**`, `docs/ejemplos-api/*` | ruta de origen + SHA-256 |
| `lectura` | Traza a mano del código C#: cada paso cita `fichero:línea` | entradas pequeñas (≤ 4 partidos o ≤ 81 columnas, ≤ 20 pasos de bucle); la traza va en el campo `traza` del caso |
| `modelo` | Modelo independiente **de la spec** (script Python en `tools/ModeloSpec/`, nunca del C#), como los «esperado (modelo)» ya presentes en 02, 03, 05 y 06 | la suite contiene además ≥ 1 caso `lectura` del mismo componente con el que el modelo coincide |
| `bcl` | Comportamiento de .NET 8 fijado leyendo **su código fuente público** (`dotnet/runtime`, rama `release/8.0`, `fichero:línea`) o su documentación oficial | cita; si hay valores, los calcula un `modelo` que transcribe ese fuente |
| `propiedad` | Invariante de la spec, no un valor (p. ej. 3^k columnas, ida y vuelta, cobertura de un reductor) | se prueba en N3 (ESPEC_14 §1) |

**Discrepancias.** Si Swift y un golden no coinciden, se **relee** el código C# (`file:line`) y se corrige el que esté mal (Swift, golden o
spec) dejando la cita en el commit. Nunca se resuelve ejecutando el original.

**Instantáneas (no son oráculo).** Para salidas grandes que nadie puede trazar a mano (B1–B8 de ESPEC_14 §6), una vez que el componente pasa
sus golden se congela la salida del propio Swift como `instantanea` (SHA-256 + recuento). Sirve para detectar regresiones, no prueba equivalencia,
y se marca así en el JSON.

### 1.1 Comportamientos de plataforma fijados por lectura

| Tema | Valor fijado | `fuente` |
|------|--------------|----------|
| Fin de línea al escribir | `\r\n` (la app solo corre en Windows: `Environment.NewLine`) | `bcl`; DP-02 |
| `(int)x` con `double` no finito o fuera de rango | `Int32.MinValue` (`0x80000000`): el JIT x64 de .NET 8 emite `cvttsd2si`, que devuelve el «entero indefinido»; la saturación llegó en .NET 9 (cambio incompatible documentado). La app publica `win-x64` (`Free1X2.WinUI.csproj:26-28`) | `bcl`; T13 |
| `Convert.ToInt32(double)` | redondeo a par; `OverflowException` con `NaN`/fuera de rango | `bcl` (`Convert.cs`) |
| libm (`Math.Log`, `Exp`, `Pow`) | Swift usa la libm de Darwin. La de Windows puede diferir en 1 ulp; **no verificable sin ejecutar** y aceptado (DEC-606): solo afecta al último bit antes de convertir a `Float` o de imprimir con 2 decimales | — |
| Orden de `Directory.GetFiles` | por nombre, ordinal sin distinguir mayúsculas (orden de NTFS) | `bcl`; DT-14 |
| WinRT `FileIO.Write*Async` | UTF-8 sin BOM, CRLF (DEC-708/DEC-935) | `bcl` (documentación de WinRT) |
| Cultura | C1–C2 `es-ES`; C3–C13 la del sistema (DP-01); los golden fijan `es-ES` | DP-01 |

## 2. Formato y layout de los golden

```
Free1X2.Mac/Packages/Free1X2Kit/Tests/Golden/
├─ indice.json               suites, nº de casos por suite y por `fuente`, SHA-256 de cada fichero
├─ net/                      G-NET: FormatoNET, Convert.*, T13, T14, AleatorioNET, Trim/ToUpper/ReadLine, sort, Hashtable   (§4.1)
├─ base/                     columnas y codificaciones (06 §1-2)
├─ motor/<componente>/       generador, analizador, grupos, filtros, CP, If-Then, tolerancias, análisis, reducción, escrutinio
├─ formatos/<Fnn>/           lectura (fichero → modelo) y escritura (modelo → bytes) de 07 §1
├─ online/                   parsers F18/F19, caché, cliente (respuestas HTTP como datos)
├─ casosdeuso/<CU-ID>/       casos de los CU (§5)
└─ corpus/                   ficheros de entrada de formatos (§6)
```

**Casos** en JSON Lines (`<suite>.jsonl`, un caso por línea, UTF-8 sin BOM, `\n`):

```json
{"id":"G-GEN-1","fuente":"lectura","componente":"GeneradorColumnas","entrada":{"pronosticos":["1","1","1,X,2"]},
 "salida":{"columnas":["111","11X","112"]},"traza":["MC/GeneradorColumnas.cs:88 i=0 …","…"]}
{"id":"G-ERR-07","fuente":"lectura","componente":"FiltroNoVariantes","entrada":{"values":""},"error":"IndexOutOfRangeException",
 "traza":["MC/FiltroNoVariantes.cs:41 Split(\"\") ⇒ [\"\"] …"]}
```

| Dato | Codificación en el golden | Comparación en Swift |
|------|---------------------------|----------------------|
| Entero en ±2^53 | número JSON | `==` |
| Entero fuera de ±2^53 (`Int64`/`UInt64`) | cadena decimal | `==` tras parsear |
| `double` | `{"bits":"0x3FE199999999999A","texto":"0,55"}` | igualdad de **bits** (06:1018) |
| `float` | `{"bits":"0x3F0CCCCD","texto":"0,55"}` | igualdad de bits |
| Texto | cadena JSON (escapes estándar) | `==` sobre `utf16` |
| Fichero de salida | fichero hermano con los bytes exactos (`*.out.<ext>`, CRLF por §1.1) + `sha256` | bytes idénticos (DP-02) |
| ZIP `.z3q` | bitmap descomprimido (`sha256`) + nombre de la entrada; cabeceras según SharpZipLib 1.4.2 leído (06 §2.14) | contenido descomprimido en ambos sentidos |
| Error | `"error":"<TipoDeExcepción .NET>"` (+ `"mensaje"` si es un literal del código) | `ErrorDominio.excepcion.rawValue` (ESPEC_11 §5.2) |
| Avisos (`UserDialogs.Show*` en el motor) | `"avisos":[{"tipo":"info","texto":"…"}]` | `Avisos` del resultado |
| Instantánea (§1) | `{"fuente":"instantanea","sha256":"…","lineas":n}` | SHA-256 de lo que escribe Swift |

## 3. Cómo los carga Swift

```swift
// Free1X2Pruebas/Golden.swift
public struct CasoGolden<Entrada: Decodable & Sendable, Salida: Decodable & Sendable>: Decodable, Sendable, CustomTestStringConvertible {
    public let id: String
    public let fuente: FuenteGolden          // test, fixture, lectura, modelo, bcl, propiedad, instantanea
    public let entrada: Entrada
    public let salida: Salida?
    public let error: String?
    public var testDescription: String { id }
}
public enum Golden {
    public static func casos<E, S>(_ suite: String, _: E.Type = E.self, _: S.Type = S.self) -> [CasoGolden<E, S>]   // lee Bundle.module
    public static func bytes(_ rutaRelativa: String) -> Data
}
public struct DoubleGolden: Decodable, Sendable { public let bits: String; public var valor: Double { … } }

// Uso en un test de paridad
@Test(.tags(.golden), arguments: Golden.casos("motor/analizador/rutas", EntradaRuta.self, SalidaRuta.self))
func rutasDelAnalizador(_ caso: CasoGolden<EntradaRuta, SalidaRuta>) async throws {
    let resultado = try await Motor.ejecutar(caso.entrada.comoEntradaCalculo(), entorno: .pruebas())
    #expect(resultado == caso.salida)
}
```

`indice.json` se valida en un test previo: todos los ficheros existen, su SHA-256 coincide y ningún caso carece de `fuente` (ni de
`traza` si es `lectura`). Si un golden cambia, el commit explica por qué con la cita `file:line` que lo justifica.

## 4. Qué cubre (suites obligatorias)

### 4.1 `G-NET` — comportamiento de la BCL de .NET que el Swift replica (cierra 06 A2, A4, A5, A8; 04 A-6; 07 A6, A12)

Todas con `fuente` = `bcl` (valores calculados por un `modelo` que transcribe el fuente de `dotnet/runtime` `release/8.0`).

| Suite | Contenido | Fuente leída | Cierra |
|-------|-----------|--------------|--------|
| `G-NET-PARSE` | cadenas límite por función: `Convert.ToInt16/ToInt32/ToDouble/ToBoolean`, `int.Parse`, `float.Parse(es-ES)`, `double.TryParse`; espacios, signos, miles, exponente, vacíos, `NaN`, `∞` | `Number.Parsing.cs`, `Convert.cs` | T7; 06 A4; 04 A-6 |
| `G-NET-FMT` | `double`/`float` representativos (±0, subnormales, ∞, NaN, enteros grandes, 0,125, x,xx5) × `ToString()`, `"F0"`, `"F2"`, `"{0:f2}"`, `"0.00"`, `"#,##0.00;0.00"`, `"r"`, `"0.##"`; `TimeSpan.ToString()`; `DateTime` fijos | `Number.Formatting.cs`, `Number.Dragon4.cs`, `Number.Grisu.cs`, `TimeSpanFormat.cs` | T14; 06 A2; 07 A12 |
| `G-NET-CAST` | `(int)` y `Convert.ToInt32` sobre `double` no finitos, ±2^31, ±0,5, 2,5, 3,5 | §1.1 | T13 |
| `G-NET-RND` | `new Random(s)` para `s ∈ {0, 1, 42, −1, Int32.MaxValue, Int32.MinValue}`: secuencias de `Next()`, `Next(n)`, `Next(a,b)`, `NextDouble()` | `Random.Net5CompatImpl.cs` (algoritmo sustractivo de Knuth, ESPEC_16 §5) | `AleatorioNET`; 08 A19 |
| `G-NET-TXT` | `Trim()`, `ToUpper()`, `Char.IsLetter`, `StreamReader.ReadLine` (`\n`, `\r`, `\r\n`, BOM), `Encoding.Latin1` con caracteres > U+00FF | `String.Manipulation.cs`, `StreamReader.cs`, `Latin1Encoding.cs` | T7, T8, T15; 06 A5 |
| `G-NET-SORT` | `List<T>.Sort`/`Array.Sort` con los comparadores con empates de la app (`PosiblesPremiosComparer` y demás `Array.Sort`/`List<T>.Sort` portados; los quicksorts de pantallas van aparte, `ordenarComoQuicksortCSharp`), ≤ 16 y > 16 elementos, y `double[]` con `NaN` | `ArraySortHelper.cs` | T5; 05 A3; 06 A9; 04 A-9 |
| `G-NET-HASH` | Orden de `Hashtable.Keys` en `EscrutadorComb` para las claves que produce la app | `Hashtable.cs`, `HashHelpers.cs` | T6; 05 A2 |

### 4.2 Motor, formatos y online

Cada componente de ESPEC_01…07 tiene una sección «Casos golden propuestos»: **todas** son obligatorias con el ID que allí figura
(`G-E2E-*`, `G-GEN-*`, `G-IFTHEN-OVR`, `G-GRP-MASK`, `G-CTL-FALLOS`, `G-TOL`, `G-ERR`, `G-CFG` de 01 §6; `COLS-A` y las de cada filtro
de 02; las de cada componente de 03 y 04; `GR-*`, `GS-*`, `GE-*` de 05 §6; las de 06; las de 07 §6-7). Reglas:

0. **No hay** suites de los componentes que no se portan (`DECISIONES.md` §4: p. ej. `GR-08…GR-10` de `ReductorTM`, `GR-13` de XFSF/V3/JLPM).
1. Los valores «esperado (modelo)» o «[réplica]» de las specs entran como `modelo` y cumplen su requisito (§1: al menos un caso `lectura`
   del mismo componente coincide). Si un caso `lectura` lo desmiente, manda la lectura: se corrige la spec citando `file:line` (R4) y se
   anota en `DECISIONES.md`.
2. **`G-D2`**: un caso `lectura` por cada comportamiento «por lectura» de la antigua decisión D2 y por cada «Defecto aparente» de
   08/09/10, con la traza completa; se replica tal cual (DP-09). Lista D2 (índice M0 en `1e39740`): If-Then asigna en vez de combinar al
   contar (`Analizador.cs:100` frente a `:150`, 01); `ReinicializarVariables` no hace nada tras la 1.ª carga (01, 07); `aciertosMax` de
   Formatos 1-2-3 no se guarda (03); Dibujos vía If-Then rechaza siempre (03); `FiltroDiferencias.ObtenNoAciertosTolerancias` lanza (01, 03);
   `EscrutadorComb` falla y orden por `Hashtable` (05); `JDCDobleContador` ignora el último partido (05); `ConvStrToLong` sin validar
   caracteres (06); deduplicado de `ObtenerListaFromTxt` (04, 06); Filtro Pim ejecuta el motor de aidomnou (09 A-57-1, 10); Grupos de
   equipos no llega al analizador tras «Abrir» (09 A-62-1).
3. **`G-ERR`**: cada excepción que las specs documentan en el camino de datos (06 §1.4, 04 S4, 03 B7, 07 §0.2-3) tiene al menos un caso.
4. Los **138 tests** existentes se portan a Swift a mano (85 aplicables, ESPEC_14 §5); sus literales entran como `test` y sus ficheros
   como `fixture`.
5. Rutas A/B/C/D del analizador (01 §1.1): casos `lectura` pequeños para cada ruta, más `propiedad` (las cuatro rutas aceptan las
   mismas columnas) sobre combinaciones del corpus.
6. Online: los 6 JSON de `Free1X2.Domain.Tests/Fixtures/online/` y los 4 de `docs/ejemplos-api/` (`fixture`) + los casos sintéticos de
   07 §6.9-6.10 (`lectura`: estado, cabeceras, cuerpo, demora simulada).

## 5. Casos de uso de pantalla (193 CU; cierra H-06.5)

Hoy no tienen tests. Cada CU recibe **un** nivel; ESPEC_13 asigna el nivel de los 193.

| Nivel | Cuándo | Esperado |
|-------|--------|----------|
| **O1 — dominio directo** | El CU es una orquestación fina del dominio (motor, reductores, escrutinio, E/S) | Casos `lectura` del ViewModel (qué funciones llama y con qué argumentos, según el pseudocódigo de 08/09/10) + los golden del dominio que invoca |
| **O2 — lógica en el ViewModel** | El algoritmo vive en el ViewModel (`Lógica en el ViewModel`, 10:17-20) y es de cálculo o fichero→fichero | Casos `lectura` trazados sobre el **fuente del ViewModel** con entradas pequeñas (propiedades resultantes, ficheros escritos, mensajes de `AppServices`) + `propiedad` para tamaños reales; salidas con hora excluidas |
| **O3 — especificación** | Presentación pura (navegar, rellenar listas, habilitar botones) y aleatorios con semilla de reloj no separable (08 A19, 09 A-73-2) | Ejemplos con los valores de la spec + `propiedad` (ESPEC_14 §3) + revisión del dueño en M5 |

Reglas: un CU con **cálculo masivo o escritura de ficheros** nunca queda en O3; los CU aleatorios con semilla explícita usan O1/O2 con la
semilla fijada (`G-NET-RND`); con semilla de reloj, O3 con `Entorno.aleatorio` inyectado y propiedades (distribución, rangos, invariantes).

## 6. Corpus de ficheros (cierra H-06.7; D6)

No hay ficheros del dueño (D6). El corpus se arma con:

- **`fixture`**: todo lo que ya está en el repo — `Free1X2.WinUI/Datos/**` (`parametros.free1x2`, `aidomnou.cfg`, `ColumnasProbables.xml`,
  `Equipos/*`, `Idioma/en-US.lang`, `Impresion/*.cfg`), `Free1X2.Domain.Tests/Fixtures/**`, `docs/ejemplos-api/*`.
- **`lectura`**: ficheros escritos a mano byte a byte según el código de los escritores (07 §1), uno por característica:
  - `.comb` (F02): cada uno de los 14 filtros con datos, grupos con partidos parciales, controles de grupos con fallos, conjuntos,
    If-Then (condiciones y grupos), tolerancias con letras, CP con las 4 relaciones, archivo-filtro, filtro parcial, comentarios XML, y
    uno «completo» con todo a la vez.
  - Condiciones (F03): un fichero por extensión (`.vx2 … .if`, `.xml`) y `Temp/tmp.*`.
  - Configuración (F01): semilla, sin nodo `salir`, `PrecioApuesta="0,55"`, valores límite (`G-CFG`).
  - Resto: F04–F17 y F24; `.dat` y equipos `.txt` con `ñ á é ü ç` y caracteres fuera de Latin-1 (`ł`, `ő`).
  - Variantes de borde (07 §7): BOM, CRLF/LF/CR, declaración `encoding="ISO-8859-1"`, atributos con `& < > " \n`, rutas Windows absolutas.

Criterio de cierre de M4 (sustituye «round-trip con ficheros reales», PLAN §5 M4): **lectura** de todo el corpus = modelo esperado;
**escritura** del modelo = bytes esperados; **round-trip** lectura→escritura = bytes originales para cada fichero `fixture` y `lectura`.

## 7. Procedimiento M1 y criterio de cierre (R3)

No se instala ni se ejecuta nada de .NET.

1. Crear `Tests/Golden/` con `indice.json` y el cargador de §3 (compila en M2 con el paquete).
2. Portar los literales de los 85 tests aplicables (`test`) y copiar los `fixture`.
3. `G-NET`: leer el fuente de `dotnet/runtime` `release/8.0` citado en §4.1, transcribir los algoritmos en `tools/ModeloSpec/` y generar los valores.
4. Suites de motor, formatos y online (§4.2), `G-D2`, `G-ERR` y corpus (§6): trazas `lectura` primero, `modelo` después.
5. Casos de los CU O1/O2 (§5).
6. Revisión del dueño y commit en `Tests/Golden/`.
7. Si una traza desmiente una lectura previa o un valor de modelo, se corrige la spec y se anota en `DECISIONES.md` (entrada nueva con `file:line`).

**M1 cerrado** cuando: están todas las suites de §4 y §6 y los casos O1/O2 de ESPEC_13, con su nº de casos en `indice.json`; cada caso tiene
`fuente` válida; cada suite `modelo` tiene su caso `lectura` de contraste.

## 8. Abiertas que este documento cierra

| Abierta | Cierre |
|---------|--------|
| 01 A1, 04 A-13, 05 A11, 06 A13, README «Estado de la verificación» | Oráculo por lectura (§1-§7); valores de modelo o réplica válidos solo con su caso `lectura` de contraste (§4.2.1) |
| 05 A10, 06 A14, 07 A1 (EOL de los golden) | Los golden llevan los bytes de Windows (CRLF, §1.1); Swift escribe CRLF (DP-02) |
| 06 A1 (cabeceras ZIP) | Fijadas leyendo SharpZipLib 1.4.2 (§2, fila ZIP) |
| 06 A2, A4, A5, A8; 04 A-6; 07 A6, A12 | `G-NET` (§4.1); libm de Darwin aceptada (§1.1) |
| 05 A1-A3 (EscrutadorComb, Hashtable, comparer) | `G-NET-HASH`, `G-NET-SORT`, `GE-07…GE-09` |
| 07 A2-A3, A15 (corpus, exactitud XmlTextWriter, cultura de muestras) | corpus §6 (`fixture` + `lectura` del escritor) |
| 08 A19, 09 A-73-2 (aleatoriedad) | `G-NET-RND` + regla de §5 |
| 09:2084 G-4, README «Estado de la verificación» (ViewModels sin oráculo) | niveles O1/O2/O3 (§5) |
