# ESPEC 16 — Piezas transversales (`Free1X2Base` y `Free1X2Formatos`)

> Cierra H-10 de [`REVISION_SPECS.md`](REVISION_SPECS.md) y las trampas T7, T8, T13, T14 de [ESPEC_00](ESPEC_00_CONVENCIONES.md) §3.
> Son piezas que usan muchos componentes y que hoy están descritas en trozos (07 §1.0-2, 06 §1-2, 10 por pantalla). Todas se
> aceptan con las suites `G-NET` de [ESPEC_12](ESPEC_12_ORACULO.md) §4.1. Lo marcado **[.NET]** es conocimiento de la BCL que el
> golden confirma; si el golden difiere, manda el golden.

## 1. `FormatoNET` — parseo y formato con semántica .NET (Base)

### 1.1 `Cultura`

```swift
public struct Cultura: Sendable, Hashable {
    public var separadorDecimal: String          // es-ES ","   en-US "."
    public var separadorMiles: String            // es-ES "."   en-US ","
    public var signoNegativo: String             // "-"
    public var simboloNaN: String                // [.NET] es-ES "NaN"
    public var simboloInfinitoPositivo: String   // [.NET] es-ES "∞" / en-US "∞" (.NET 5+, ICU) — G-NET-FMT
    public var patronFechaCorta: String          // [.NET] es-ES "dd/MM/yyyy"
    public var patronHoraLarga: String           // [.NET] es-ES "H:mm:ss"
    public static let esES: Cultura              // cultura del oráculo y de C1–C2 (07 §2.1)
    public static let enUS: Cultura
    public static func sistema(_ locale: Locale = .autoupdatingCurrent) -> Cultura   // DP-01: «cultura actual» de C3–C13
}
```

`Cultura.sistema` toma separadores y patrones de la región del Mac (equivale a «cultura actual» del Windows del usuario). Cada
punto C1–C13 de 07 §2.1 usa `.esES` o `entorno.cultura` exactamente como el C# usa es-ES fija o la cultura actual.

### 1.2 API

| Función | Equivale a (.NET) | Reglas |
|---------|-------------------|--------|
| `convertToInt16(_:)`, `convertToInt32(_:)` `throws(ErrorDominio)` | `Convert.ToInt16/ToInt32(string)` | recorta espacios `U+0009…U+000D`, `U+0020` (y los de `Char.IsWhiteSpace` [.NET]); un signo opcional; solo dígitos ASCII; vacío/otro ⇒ `.formato`; fuera de rango ⇒ `.desbordamiento` (04 D4) |
| `convertToBoolean(_:)` | `Convert.ToBoolean(string)` | `true`/`false` sin distinguir mayúsculas, con espacios laterales; otro ⇒ `.formato` (07 §1.0) |
| `convertToDouble(_:cultura:)`, `parseFloat(_:cultura:) -> Float` | `Convert.ToDouble`, `float.Parse` | `NumberStyles.Float | AllowThousands` con los símbolos de `cultura`: miles solo tras un dígito y antes del decimal; exponente `e/E`; `NaN`/`∞` según cultura; redondeo IEEE al más cercano (= `strtod`/`strtof`) (07 §2.1 `parsearEs`) |
| `tryParseDouble(_:cultura:) -> Double?` | `double.TryParse` | igual sin lanzar |
| `toString(_ d: Double, cultura:)` | `double.ToString()` (.NET Core 3.0+) | dígitos de ida y vuelta **más cortos** (los mismos que `Double.description` de Swift: se toman de ahí y se recomponen); fija si el exponente decimal ∈ [−5, 15), si no `d,dddE+XX` / `E-XX` (mín. 2 dígitos); entero sin decimales; `-0` ⇒ `"-0"` [.NET] |
| `toString(_ f: Float, cultura:)` | `float.ToString()` | igual con `Float.description` (06 A2; 10:899, 10:1701) |
| `toStringR(_:cultura:)` | `ToString("r")` | igual que `toString` en .NET Core 3.0+ [.NET] (10:1513) |
| `toStringFijo(_:decimales:cultura:)` | `"F0"`, `"F2"`, `"{0:fN}"`, `"0.00"` | **redondeo «half away from zero» sobre el valor binario exacto** (10:1048, 10:1477): se obtiene la expansión decimal exacta del `Double` (`String(format: "%.1100f")` en Darwin es exacta) y se redondea esa cadena; nunca `String(format: "%.2f")` (que redondea a par) |
| `toStringPatron(_:patron:cultura:)` | patrones personalizados usados por la app: `"#,##0.00;0.00"` (10:782), `"0.##"` (10:283) | secciones `;` (la segunda para negativos, sin signo), agrupación de miles de 3, mismo redondeo que `toStringFijo` |
| `toString(_ t: TimeSpanNET)` | `TimeSpan.ToString()` | `[-][d.]hh:mm:ss[.fffffff]` invariante; `TimeSpanNET` = ticks de 100 ns (10:588, 10:808) |
| `fechaCorta(_:cultura:)`, `horaLarga(_:cultura:)`, `formatear(_:patron:)` | `ToShortDateString`, `ToLongTimeString`, `ToString("dd/MM/yyyy HH:mm")` | patrones de la cultura; `/` y `:` son los separadores de la cultura (07 C11) |
| `castInt32(_ d: Double) -> Int32` | `(int)d` en x64 (.NET 8) | trunca; `NaN`, `±∞` o fuera de `Int32` ⇒ `Int32.min` (T13; G-NET-CAST) |
| `convertToInt32(_ d: Double) throws(ErrorDominio) -> Int32` | `Convert.ToInt32(double)` | redondeo a par (T4); `NaN`/fuera de rango ⇒ `.desbordamiento` |
| `mayusculas(_:)` | `string.ToUpper()` | mapeo simple carácter a carácter UTF-16 (sin expansiones: `ß` se queda) (04 S7; 06 A5) |
| `recortar(_:)` | `string.Trim()` | conjunto `Char.IsWhiteSpace` [.NET] |
| `esLetra(_:)` | `Char.IsLetter` | categorías Unicode Lu, Ll, Lt, Lm, Lo de la unidad UTF-16 (02 §0.9.5) |
| `ticks(_ fecha: Date, zona: TimeZone)` | `DateTime.Now.Ticks` | intervalos de 100 ns desde 0001-01-01 en hora local; para semillas `unchecked((int)Ticks)` ⇒ `Int32(truncatingIfNeeded:)` (08:604) |

Rendimiento: nada de esto está en el camino caliente del motor; se prioriza la exactitud sobre la velocidad.

## 2. `EscritorXMLNET` y lectura XML (Formatos)

### 2.1 Escritura: réplica de `XmlTextWriter(ruta, null)` + `Formatting.Indented`

API en streaming: `iniciarElemento(_:)`, `atributo(_:_:)`, `texto(_:)`, `comentario(_:)`, `cerrarElemento()`, `declaracion()`, `cerrar() -> Data`.
Reglas (07 §1.0.1):

| Aspecto | Regla |
|---------|-------|
| Codificación | UTF-8 **sin BOM** |
| Declaración | solo si el documento la tiene (F01, F05); `.comb` y condiciones **sin** declaración |
| Sangría | 2 espacios por nivel; cada hijo en línea nueva; EOL `\r\n` (DP-02) |
| Elemento vacío | `<x a="1" />` (espacio antes de `/>`) |
| Elemento con solo texto | en una línea `<dibujo>3+2</dibujo>`; texto vacío ⇒ `<dibujo />` |
| Atributos | orden de escritura; comillas dobles; escapes `&amp; &lt; &gt; &quot;` y `&#x9; &#xD; &#xA;`; `'` y no ASCII sin escapar |
| Comentario | `<!-- texto -->` con los espacios exactos pasados |
| Final | sin salto de línea tras el último `>`; `cerrar()` cierra lo abierto |

Edición in situ (F01 `parametros.free1x2`, F05 `ColumnasProbables.xml`): se lee a un árbol, se modifica y se serializa con **este**
escritor recorriendo el árbol (mismas reglas), como hace el C# al guardar.

### 2.2 Lectura (equivalente a `XmlDocument.Load(new XmlTextReader(ruta))`)

- `Foundation.XMLDocument` con `.nodeLoadExternalEntitiesNever` (sin DTD externas, 07 A22) y conservando comentarios.
- Tras cargar, se **eliminan los nodos de texto solo-espacio** (`PreserveWhitespace = false` de .NET; 07 A23) para que `ChildNodes[i]`
  indexe igual.
- Ayudantes que replican la navegación y los modos de fallo del C# (07 §0.2.3): `primerElemento(nombre)` (= `GetElementsByTagName(n)[0]`,
  primer descendiente en orden de documento, sensible a mayúsculas), `hijo(i)` (incluye comentarios), `atributo(_:)` que lanza
  `ErrorDominio(.referenciaNula)` si falta (= `Attributes["x"].Value`), `atributoOpcional(_:)`.
- Codificación por BOM o declaración (`encoding="ISO-8859-1"` incluido); sin declaración ⇒ UTF-8.

## 3. ZIP `.z3q` (Formatos, F16)

- Contenido: **una** entrada, método Deflate, con el bitmap de columnas (06 §2.14). Lo escribe hoy SharpZipLib 1.4.2 (06 A1).
- **Lectura:** parser propio de cabecera local + directorio central (≈ 150 líneas); descompresión con `Compression.framework`
  (`COMPRESSION_ZLIB` = DEFLATE crudo); CRC-32 propio (tabla de 256 entradas) comprobado al terminar.
- **Escritura:** cabecera local, datos comprimidos con `COMPRESSION_ZLIB`, directorio central y fin de directorio; versión 20, banderas 0,
  tamaños y CRC en la cabecera local (sin descriptor), nombre de entrada igual que el C# y fecha DOS = ahora (`Entorno.reloj`).
- **Comparación** (ESPEC_12 §2): bitmap descomprimido y nombre de entrada; los bytes comprimidos no se comparan (fecha y nivel de compresión).
  Compatibilidad hacia el C#: test de integración `unzip -t` sobre lo escrito por Swift + lectura de los `.z3q` del corpus.
- Sin dependencias de terceros (ESPEC_11 §3.3).

## 4. `TextoNET` — codificaciones, BOM y fin de línea (Base)

| Pieza | Comportamiento (DP-02: igual que el C# en Windows) |
|-------|-----------------------------------------------------|
| `LectorLineas(data, codificacionPorDefecto:)` | = `StreamReader` con detección de BOM (UTF-8, UTF-16 LE/BE, UTF-32) [.NET]; `leerLinea()` corta en `\r`, `\n` o `\r\n`; **no** salta líneas en blanco; `hayMas` = `Peek() >= 0` (05 §1.2) |
| UTF-8 inválido | se sustituye por `U+FFFD` igual que `UTF8Encoding` de .NET (se confirma con `G-NET-TXT`); **no** se aplica el *fallback* a Windows-1252 propuesto en 07 A7 (DP-10: los equipos `.txt` antiguos se leen como hoy) |
| Latin-1 (`.dat`, F10) | lectura byte → `U+00xx`; escritura con `?` para caracteres > `U+00FF` |
| `EscritorLineas(url, codificacion:, eol:)` | `.utf8SinBOM` (defecto), `.utf8ConBOM` (equipos `.txt`, F17), `.latin1`; EOL `.crlf` siempre (DP-02); **creación perezosa**: el fichero se crea (truncando) al escribir la primera línea; si no se escribe ninguna, no se toca (05 §1.2); búfer de 64 KiB |
| JSON (caché, F18) | UTF-8 sin BOM; al leer se quita un `U+FEFF` inicial (07 §5.6) |

La tabla completa por formato es la columna «C# escribe (Windows)» de 07 §2.2.

## 5. `AleatorioNET` — `System.Random(int semilla)` (Base)

Réplica del generador sustractivo de Knuth que usa .NET para `new Random(semilla)` (implementación «compatible» que .NET 6+ conserva
cuando se pasa semilla) [.NET; confirmar con `G-NET-RND`]:

```
MBIG = Int32.max ; MSEED = 161803398
init(semilla):
    resta = (semilla == Int32.min) ? Int32.max : abs(semilla)
    mj = MSEED - resta ; tabla[55] = mj ; mk = 1
    para i en 1..<55: ii = (21*i) % 55 ; tabla[ii] = mk ; mk = mj - mk ; si mk < 0: mk += MBIG ; mj = tabla[ii]
    repetir 4 veces: para i en 1..<56: tabla[i] -= tabla[1 + (i+30) % 55] ; si tabla[i] < 0: tabla[i] += MBIG
    inext = 0 ; inextp = 21
muestraInterna():
    inext = inext+1 == 56 ? 1 : inext+1 ; inextp = inextp+1 == 56 ? 1 : inextp+1
    r = tabla[inext] - tabla[inextp] ; si r == MBIG: r -= 1 ; si r < 0: r += MBIG
    tabla[inext] = r ; devolver r
muestra()            = Double(muestraInterna()) * (1.0 / Double(MBIG))
next()               = muestraInterna()
next(max)            = Int32(muestra() * Double(max))                       // truncamiento
next(min, max)       = rango = Int64(max) - Int64(min)
                       rango <= Int32.max ? Int32(muestra() * Double(rango)) + min
                                          : Int32(Int64(muestraRangoGrande() * Double(rango)) + Int64(min))
muestraRangoGrande() = r = muestraInterna() ; si muestraInterna() % 2 == 0: r = -r
                       d = Double(r) + Double(Int32.max - 1) ; devolver d / Double(2 * UInt32(Int32.max) - 1)
nextDouble()         = muestra()
```

`struct AleatorioNET` (valor, `mutating`); toda la aritmética entera con `&-`/`&+` donde el C# pueda envolver (T1). Donde el C# usa
`new Random()` sin semilla, el port usa `SystemRandomNumberGenerator` vía `Entorno` (no reproducible, como hoy; sin golden).

## 6. Ayudantes únicos (Base)

| Ayudante | Sustituye | Regla |
|----------|-----------|-------|
| `CodificacionX0MSB.numero(_ columna: String, partidos: Int) -> Int32` / `.columna(_ n: Int32, partidos: Int) -> String` | `S14N/N14S`, `S2n/N2s`, `S2N/N2S`, `S1n/N1s` y `s1n` de 9 pantallas (08 §1.4; 10:578, 741, 1002, 1064, 1804) | `'1'→1`, `'2'→2`, `'X'` y cualquier otro → 0; primer partido = dígito **más** significativo (`n = n*3 + d`); inversa con `0→'X'` y longitud fija. **Distinta** de `ConvertidorDeBases` (06 §1.7: no unificar) |
| `ordenarComoQuicksortCSharp(_:por:)` | quicksort propio de 3 pantallas (10:523, 10:885, 10:1297) | Hoare recursivo con pivote `a[(izq+der)/2]` exactamente como el pseudocódigo de 10:523 (no estable; el orden de empates es parte de la salida) |
| `IntrosortNET.ordenar(_:por:)` | `List<T>.Sort` / `Array.Sort` con comparador cuando el orden de empates es observable (05 A3, 06 A9) | réplica de `ArraySortHelper<T>.IntrospectiveSort` de .NET 8: límite de profundidad `2·(⌊log2 n⌋+1)`; particiones ≤ 16 ⇒ casos 2 y 3 con `SwapIfGreater` y si no inserción; pivote mediana de tres (`lo`, `medio`, `hi`) movido a `hi−1`; *heapsort* al agotar la profundidad. Solo se usa donde `G-NET-SORT` demuestre que el orden de empates llega a la salida; si no, `sort(by:)` |
| `CRC32` | — | tabla estándar (polinomio `0xEDB88320`) para ZIP |
