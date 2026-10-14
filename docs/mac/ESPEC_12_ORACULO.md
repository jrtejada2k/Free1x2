# ESPEC 12 — Oráculo: `tools/GoldenGen` y los datos golden (fase M1)

> Cierra H-06 de [`REVISION_SPECS.md`](REVISION_SPECS.md). Arquitectura en [ESPEC_11](ESPEC_11_ARQUITECTURA_SWIFT.md),
> pruebas en [ESPEC_14](ESPEC_14_PRUEBAS.md). Citas `NN:línea` = `mac/ESPEC_NN_*.md:línea`.
>
> **Regla del dueño (2026-10-13):** «nada en .NET» se refiere a la **app nueva**. Usar C# como **herramienta aparte**
> para generar una vez los resultados de referencia está permitido (PLAN:8-10). La app Mac no contiene, no enlaza
> ni ejecuta C#; los golden son **datos** versionados.

## 1. Qué es GoldenGen

| Aspecto | Decisión (DT-20…DT-27) | Motivo |
|---------|------------------------|--------|
| Qué | Aplicación de consola C# `tools/GoldenGen/` (fuera de `Free1X2.Mac/` y de `Free1X2.sln`) | no toca la app actual (R1, R6) |
| Proyectos | `GoldenGen.Motor` (`net8.0`, `ProjectReference` a `Free1X2.Domain`) · `GoldenGen.Pantallas` (`net8.0`: compila en el Mac los **fuentes de los ViewModels enlazados en solo lectura** —`<Compile Include="../../Free1X2.WinUI/Views/Ported/XxxViewModel.cs" Link=…>`— más los servicios y controles portables y una capa de *shims* para las APIs de WinUI/WinRT, §5.1) · `GoldenGen.Comun` (serialización, normalización §1.1, manifiesto) | el dominio es portable (`Free1X2.Domain.csproj:4`; ninguna ruta con `\` literal); ningún fuente de la app se modifica |
| Dónde corre | **En el Mac del dueño (Apple Silicon)**, con el **SDK de .NET 8 x64 ejecutado por Rosetta 2**, instalado aparte en `~/.dotnet-x64` (no toca los .NET 6/10 ya instalados). Una vez por versión del oráculo; el resultado se revisa y se incorpora con un commit | la app real es .NET 8 x64 (`Free1X2.WinUI.csproj`: `win-x64`); desde .NET 9, y en arm64, la conversión `double→int` satura: solo .NET 8 x64 reproduce T13. Rosetta ejecuta el mismo JIT x64 y la misma BCL que la app. Las demás diferencias Mac/Windows se neutralizan en §1.1. Sin Windows (decisión del dueño, 2026-10-14) |
| Cultura | `Main` fija `CultureInfo.DefaultThreadCurrentCulture`, `DefaultThreadCurrentUICulture`, `CurrentCulture` y `CurrentUICulture` = `es-ES` antes de nada | oráculo `es-ES` (07:567; 06:1017; DP-01) |
| Zona horaria y reloj | Ninguna salida golden contiene la hora actual; los campos «Tiempo»/fechas de ejecución se excluyen (se listan en cada suite) | determinismo |
| Determinismo | Las entradas variadas se generan con `new Random(20261013)` **dentro de GoldenGen** y se **guardan** en el golden (Swift no necesita reproducir ese generador). Cada ejecución corre **dos veces** y compara SHA-256 de toda la salida; si difieren, falla | regeneración idéntica |
| Versión | `manifest.json` registra: commit del repo C#, versión de .NET (`RuntimeInformation`), SO, cultura, fecha, SHA-256 por fichero y nº de casos por suite | trazabilidad (R4) |

Comando (Mac):

```bash
~/.dotnet-x64/dotnet run -c Release --project tools/GoldenGen/GoldenGen.Pantallas -- --salida Free1X2.Mac/Packages/Free1X2Kit/Tests/Golden --todas
```

(`GoldenGen.Pantallas` incluye las suites de `GoldenGen.Motor`; `--suite <id>` ejecuta una sola.)

### 1.1 Diferencias Mac/Windows y cómo se neutralizan

| Diferencia | Efecto en el C# ejecutado en el Mac | Neutralización |
|------------|-------------------------------------|----------------|
| `Environment.NewLine` = `\n` | `WriteLine`, `StreamWriter`, `XmlTextWriter` (sangría) escriben LF | `GoldenGen.Comun` convierte toda salida de escritores de texto del C# a CRLF (`\n` → `\r\n`) antes de guardarla: son los bytes que escribe la app en Windows (DP-02). No se aplica a JSON de caché (sin EOL propio) ni a ZIP. Ningún dato de la app contiene un `\n` propio (columnas, XML, TSV). Igual para mensajes que concatenan `Environment.NewLine` |
| Conversión `double→int` (`(int)NaN`, fuera de rango) | arm64 y .NET 9+ saturan | .NET 8 **x64** bajo Rosetta: `Int32.MinValue` como en Windows x64 (`G-NET-CAST` lo comprueba) |
| Cultura | Windows usa ICU desde .NET 5 (Windows 10 1903+); el Mac, el ICU del sistema | `es-ES` forzado; `G-NET-PARSE/FMT` fijan cada resultado. Una diferencia de versión de ICU solo afectaría a símbolos raros (∞, NaN) que la app no escribe en ficheros |
| libm (`Math.Log`, `Exp`, `Pow`) | Darwin libm en vez de la UCRT de Windows (posible diferencia de 1 ulp) | El producto es Mac: la referencia es Darwin libm, la misma que usará Swift. Diferencia aceptada frente a Windows; como mucho afecta al último bit antes de convertir a `Float` (06 A8, DEC-606) |
| Orden de `Directory.GetFiles` | NTFS devuelve por nombre; APFS no garantiza orden | Donde la salida depende de la enumeración (estadísticas sobre `Ganadoras/`), GoldenGen reordena por nombre (ordinal sin distinguir mayúsculas = NTFS, DT-14) las entradas por fichero |
| Rutas | separador `/` | El dominio usa `/` y `Path.Combine`. El corpus incluye a propósito rutas de estilo Windows (`C:\…`) dentro de `.comb` para probar las rutas heredadas (ESPEC_11 §9) |
| Sistema de ficheros | APFS sin distinción de mayúsculas por defecto | igual que NTFS |
| `Hashtable`, ordenaciones, aritmética IEEE | código gestionado | idéntico |
| WinRT `FileIO.*` (solo en algunos ViewModels) | no existe fuera de Windows | *shim* con la regla de DEC-708/DEC-935 (UTF-8 sin BOM, CRLF); son los únicos bytes no verificables sin Windows |

**Prueba de equivalencia de plataforma** (antes de generar nada): los **138 tests** actuales (`Free1X2.Domain.Tests`, `net8.0`) se ejecutan en el Mac con
.NET 8 x64 y deben dar **138/138**. Sus valores esperados salen de ejecuciones en Windows, así que pasarlos en el Mac demuestra que el dominio
se comporta igual en el código que cubren. Si alguno falla, se investiga antes de seguir (la diferencia pasaría a §1.1).

## 2. Formato y layout de los golden

```
Free1X2.Mac/Packages/Free1X2Kit/Tests/Golden/
├─ manifest.json
├─ net/          G-NET: FormatoNET, Convert.*, T13, T14, AleatorioNET, Trim/ToUpper/ReadLine, sort, libm   (§4.1)
├─ base/         columnas y codificaciones (06 §1-2)
├─ motor/<componente>/      generador, analizador, grupos, filtros, CP, If-Then, tolerancias, análisis, reducción, escrutinio
├─ formatos/<Fnn>/          lectura (fichero → modelo) y escritura (modelo → bytes) de 07 §1
├─ online/                  parsers F18/F19, caché, cliente (respuestas HTTP sintéticas)
├─ casosdeuso/<CU-ID>/      los CU con oráculo O1/O2 (§5)
└─ corpus/                  ficheros generados por los escritores C# (§6)
```

**Casos** en JSON Lines (`<suite>.jsonl`, un caso por línea, UTF-8 sin BOM, `\n`):

```json
{"id":"G-E2E-1","componente":"Analizador","ruta":"A","entrada":{"pronosticos":["1","1",…,"1,X,2"],"combinacion":"corpus/comb/vacia.comb"},
 "salida":{"colsAnalizadas":81,"colsAceptadas":81,"columnas":{"fichero":"motor/analizador/G-E2E-1.txt","sha256":"…","lineas":81}}}
{"id":"G-ERR-07","componente":"FiltroNoVariantes","entrada":{"values":""},"error":"IndexOutOfRangeException"}
```

| Dato | Codificación en el golden | Comparación en Swift |
|------|---------------------------|----------------------|
| Entero en ±2^53 | número JSON | `==` |
| Entero fuera de ±2^53 (`Int64`/`UInt64`) | cadena decimal | `==` tras parsear |
| `double` | `{"bits":"0x3FE199999999999A","texto":"0,55"}` (`BitConverter.DoubleToInt64Bits`) | igualdad de **bits** (06:1018) |
| `float` | `{"bits":"0x3F0CCCCD","texto":"0,55"}` (`SingleToInt32Bits`) | igualdad de bits |
| Texto | cadena JSON (escapes estándar) | `==` sobre `utf16` |
| Fichero de salida ≤ 1 MiB | fichero hermano con los bytes exactos (`*.out.<ext>`, ya con CRLF por §1.1) + `sha256` | bytes idénticos (Swift escribe CRLF, DP-02) |
| Fichero de salida > 1 MiB | **no se versiona**: `sha256`, `bytes`, `lineas` y `muestra` (primeras y últimas 50 líneas) | SHA-256 de lo que escribe Swift |
| ZIP `.z3q` | bitmap descomprimido (`sha256`) + nombre de la entrada; una vez, volcado hex de cabeceras (06:1020) | contenido descomprimido en ambos sentidos |
| Error | `"error":"<TipoDeExcepción .NET>"` (+ `"mensaje"` si es un literal del código, no de .NET) | `ErrorDominio.excepcion.rawValue` (ESPEC_11 §5.2); mensaje solo si está presente |
| Avisos (`UserDialogs.Show*` en el motor) | `"avisos":[{"tipo":"info","texto":"…"}]` | `Avisos` del resultado |

Tamaño: presupuesto total del directorio ≤ 50 MB; GoldenGen falla si lo supera.

## 3. Cómo los carga Swift

```swift
// Free1X2Pruebas/Golden.swift
public struct CasoGolden<Entrada: Decodable & Sendable, Salida: Decodable & Sendable>: Decodable, Sendable, CustomTestStringConvertible {
    public let id: String
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

`manifest.json` se valida en un test previo (todos los ficheros existen y su SHA-256 coincide). Si un golden cambia,
el commit que lo cambia explica por qué (nueva versión del oráculo o corrección de la suite).

## 4. Qué cubre (suites obligatorias)

### 4.1 `G-NET` — comportamiento de la BCL de .NET que el Swift replica (cierra 06 A2, A4, A5, A8; 04 A-6; 07 A6, A12)

| Suite | Contenido | Cierra |
|-------|-----------|--------|
| `G-NET-PARSE` | ~150 cadenas límite por función: `Convert.ToInt16/ToInt32/ToDouble/ToBoolean`, `int.Parse`, `float.Parse(es-ES)`, `double.TryParse`; espacios, signos, miles, exponente, vacíos, `NaN`, `∞`, `Infinito` | T7; 06 A4; 04 A-6 |
| `G-NET-FMT` | 300 `double` y 300 `float` representativos (incl. subnormales, ±0, ∞, NaN, enteros grandes, 0,125, x,xx5) × `ToString()`, `"F0"`, `"F2"`, `"{0:f2}"`, `"0.00"`, `"#,##0.00;0.00"`, `"r"`, `"0.##"`; `TimeSpan` × `ToString()`; `DateTime` fijos × `ToShortDateString`, `ToLongTimeString`, `"dd/MM/yyyy HH:mm"` | T14; 06 A2; 07 A12 |
| `G-NET-CAST` | `(int)` y `Convert.ToInt32` sobre `double` no finitos, ±2^31, ±0,5, 2,5, 3,5 | T13 |
| `G-NET-RND` | `new Random(s)` para `s ∈ {0, 1, 42, −1, Int32.MaxValue, Int32.MinValue, 20261013}`: 1 000 valores de `Next()`, `Next(n)`, `Next(a,b)`, `NextDouble()` | `AleatorioNET` (ESPEC_16 §5); 08 A19 |
| `G-NET-TXT` | `Trim()`, `ToUpper()`, `Char.IsLetter`, `StreamReader.ReadLine` (`\n`, `\r`, `\r\n`, BOM), `Encoding.Latin1` con caracteres > U+00FF | T7, T8, T15; 06 A5 |
| `G-NET-SORT` | `List<T>.Sort`/`Array.Sort` con comparadores con empates (≤ 16 y > 16 elementos) usados por la app (`PosiblesPremiosComparer`, `FigurasComparer`, quicksorts de pantallas) y `double[]` con `NaN` | T5; 05 A3; 06 A9; 04 A-9 |
| `G-NET-HASH` | Orden de `Hashtable.Keys` en `EscrutadorComb` para las claves que produce la app | T6; 05 A2 |
| `G-NET-LIBM` | 10 000 `Math.Log`/`Math.Pow(x,2)`/`Math.Exp` sobre valores generados (bits) — Swift los compara con Darwin | 06 A8 |

### 4.2 Motor, formatos y online

Cada componente de ESPEC_01…07 tiene una sección «Casos golden propuestos»: **todas** son obligatorias y se generan con
el ID que allí figura (`G-E2E-*`, `G-GEN-*`, `G-IFTHEN-OVR`, `G-GRP-MASK`, `G-CTL-FALLOS`, `G-TOL`, `G-ERR`, `G-CFG` de 01 §6;
`COLS-A` y las de cada filtro de 02; las de cada componente de 03 y 04; `GR-*`, `GS-*`, `GE-*` de 05 §6; las de 06; las de 07 §6-7).
Reglas añadidas:

0. **No se generan** suites de los componentes que no se portan (`DECISIONES.md` §4: p. ej. `GR-08…GR-10` de `ReductorTM`, `GR-13` de XFSF/V3/JLPM).
1. Los **valores «esperado»** de las specs que salen de réplicas Python (README «Estado de la verificación»; 02:8; 03:30) son **ilustrativos**: manda el
   golden. Si difieren, se corrige la spec citando el golden (R4) y se anota en `DECISIONES.md`.
2. **`G-D2`**: un caso por cada comportamiento «por lectura» de la antigua decisión D2 y por cada «Defecto aparente» de 08/09/10;
   el golden confirma o desmiente la lectura **antes** de portar (DP-09). Lista D2 (índice M0 en `1e39740`): If-Then asigna en vez de
   combinar al contar (`Analizador.cs:100` frente a `:150`, 01); `ReinicializarVariables` no hace nada tras la 1.ª carga (01, 07);
   `aciertosMax` de Formatos 1-2-3 no se guarda (03); Dibujos vía If-Then rechaza siempre (03); `FiltroDiferencias.ObtenNoAciertosTolerancias`
   lanza (01, 03); `EscrutadorComb` falla y orden por `Hashtable` (05); `JDCDobleContador` ignora el último partido (05); `ConvStrToLong` sin
   validar caracteres (06); deduplicado de `ObtenerListaFromTxt` (04, 06); Filtro Pim ejecuta el motor de aidomnou (09 A-57-1, 10);
   Grupos de equipos no llega al analizador tras «Abrir» (09 A-62-1).
3. **`G-ERR`**: cada excepción que las specs documentan en el camino de datos (06 §1.4, 04 S4, 03 B7, 07 §0.2-3) tiene al menos un caso.
4. Los **138 tests** existentes se portan a Swift a mano (ESPEC_14 §5); sus datos se exportan también como golden para no duplicar fixtures.
5. Rutas A/B/C/D del analizador (01 §1.1) se generan para cada combinación del corpus que lo admita.
6. Online: los 6 JSON de `Free1X2.Domain.Tests/Fixtures/online/` + los casos sintéticos de 07 §6.9-6.10 (respuestas HTTP como datos: estado, cabeceras, cuerpo, demora simulada).

## 5. Casos de uso de pantalla (193 CU; cierra H-06.5)

Hoy no tienen tests ni golden (README «Estado de la verificación»). Cada CU recibe **un** nivel de oráculo; ESPEC_13 asigna el nivel de los 193.

| Nivel | Cuándo | Cómo |
|-------|--------|------|
| **O1 — dominio directo** | El CU es una orquestación fina de `Free1X2.Domain` (llama al motor, reductores, escrutinio, E/S) | `GoldenGen.Motor` llama a las mismas funciones del dominio con los mismos argumentos que el ViewModel (según el pseudocódigo de la spec) y vuelca entradas y salidas. |
| **O2 — ViewModel real** | El algoritmo vive en el ViewModel (`Lógica en el ViewModel`, 10:17-20) y es de cálculo o fichero→fichero | `GoldenGen.Pantallas` compila en el Mac el **fuente real del ViewModel** (enlazado, sin modificarlo) con los *shims* de §5.1, lo instancia, fija sus propiedades e invoca el método del CU (por reflexión si no es público), **sin selectores**: usa los métodos con ruta (`GuardarEn`, `AbrirDesde`, …) y un directorio temporal. Captura propiedades resultantes, ficheros escritos y los mensajes que recibe el *shim* de `AppServices`. Las salidas con hora se excluyen. |
| **O3 — especificación** | CU de pura presentación (navegar, rellenar listas, habilitar botones), aleatorios con semilla de reloj no separable (08 A19, 09 A-73-2), o CU cuyo ViewModel no pueda ejecutarse sin ventana (lo determina el paso 3 de §7) | Pruebas de ejemplo con los valores de la spec + **pruebas de propiedades** (ESPEC_14 §3) + revisión del dueño en M5. |

### 5.1 *Shims* de `GoldenGen.Pantallas`

Sustituyen **solo** interfaz e infraestructura de Windows; nunca lógica de negocio.

| API de Windows | *Shim* |
|----------------|--------|
| `Windows.Storage.Pickers.*` | no se invoca: el arnés llama a los métodos con ruta; si un CU solo es alcanzable por selector, devuelve la ruta preparada por el caso |
| `Windows.Storage.StorageFile`, `FileIO.Read/Write*Async` | sobre `System.IO`, con la regla de DEC-708/DEC-935 |
| `Microsoft.UI.Xaml` (`Visibility`, …), `Microsoft.UI.Dispatching.DispatcherQueue`, `UiHilo` | tipos mínimos; la ejecución «en el hilo de UI» es inmediata en el hilo actual |
| `AppServices` (`MostrarError/Info`, `ConfirmarAsync`) | registra cada mensaje (título, texto) y responde a cada confirmación con lo que fije el caso |
| `PickerHelper` | devuelve rutas fijadas por el caso |
| `AppState`, `Log`, `ColeccionUi`, `PorcentajesHelper`, `FilaPorcentaje` y demás de `Services/` y `Controls/` | **fuente real enlazado** si compila sin XAML; si no, *shim* mínimo con la misma interfaz |

Un ViewModel es apto para O2 si compila con estos *shims* **sin editar su fuente**; si no, sus CU pasan a O3 y se registra el motivo en
`DECISIONES.md` (ESPEC_13 §5).

Reglas: un CU con **cálculo masivo o escritura de ficheros** nunca queda en O3 salvo imposibilidad técnica documentada en
`DECISIONES.md`; los CU aleatorios con semilla explícita usan O1/O2 con la semilla fijada; con semilla de reloj, O3 con
`Entorno.aleatorio` inyectado y propiedades (distribución, rangos, invariantes).

## 6. Corpus de ficheros (cierra H-06.7; D6)

No hay ficheros del dueño (D6, resuelta 2026-10-12). GoldenGen genera el corpus **con los escritores C# actuales**:

- **`.comb` (F02)**: ≥ 1 por característica — cada uno de los 14 filtros con datos, grupos con partidos parciales, controles de grupos con
  fallos, conjuntos, If-Then (condiciones y grupos), tolerancias con letras, CP con las 4 relaciones, archivo-filtro, filtro parcial, comentarios
  XML — y uno «completo» con todo a la vez.
- **Condiciones (F03)**: un fichero por extensión (`.vx2 … .if`, `.xml`) y `Temp/tmp.*`.
- **Configuración (F01)**: semilla, sin nodo `salir`, `PrecioApuesta="0,55"`, valores límite (`G-CFG`).
- **Resto**: F04–F17 y F24 con contenido variado; `.dat` y equipos `.txt` con `ñ á é ü ç` y caracteres fuera de Latin-1 (`ł`, `ő`).
- **Variantes de borde** (07 §7): BOM, CRLF/LF/CR, declaración `encoding="ISO-8859-1"`, atributos con `& < > " \n`, rutas Windows absolutas.

Criterio de cierre de M4 (sustituye «round-trip con ficheros reales», PLAN §5 M4): **lectura** de todo el corpus = modelo golden;
**escritura** del modelo = bytes golden; **round-trip** lectura→escritura = bytes originales para cada fichero del corpus.

## 7. Procedimiento M1 y criterio de cierre (R3)

0. Instalar .NET 8 SDK **x64** en `~/.dotnet-x64` (script oficial `dotnet-install.sh --channel 8.0 --architecture x64 --install-dir ~/.dotnet-x64`,
   sin `sudo`; no toca los .NET instalados) y .NET 8 SDK **arm64** en `~/.dotnet8` (solo para medir tiempos nativos, ESPEC_14 §6.2).
1. **Prueba de equivalencia** (§1.1): `~/.dotnet-x64/dotnet test Free1X2.Domain.Tests` ⇒ 138/138.
2. Crear `tools/GoldenGen` (3 proyectos). Build sin avisos.
3. Implementar `G-NET` y `base/` (no dependen del motor): primera ejecución y revisión de formato.
4. **Prueba de viabilidad O2**: compilar con los *shims* de §5.1 un ViewModel de cada tipo (cálculo, fichero→fichero, condición) y luego todos.
   Resultado por ViewModel (apto / no apto y por qué) → columna «Oráculo» de ESPEC_13 y `DECISIONES.md`.
5. Suites de motor, formatos y online (§4.2), `G-D2`, `G-ERR` y corpus (§6).
6. Suites de CU O1/O2 (§5).
7. Dos ejecuciones idénticas; `manifest.json`; revisión del dueño y commit en `Tests/Golden/`.
8. Actualizar las specs donde el golden desmiente una lectura o un valor de réplica, y cerrar en `DECISIONES.md` las entradas «pendiente de oráculo».

**M1 cerrado** cuando: 138/138 en el Mac; GoldenGen compila y es determinista; están todas las suites de §4 y §6 y las O1/O2 de ESPEC_13
con su nº de casos en el manifiesto; ninguna entrada de `DECISIONES.md` queda «pendiente de oráculo».

## 8. Abiertas que este documento cierra

| Abierta | Cierre |
|---------|--------|
| 01 A1, 04 A-13, 05 A11, 06 A13, README «Estado de la verificación» | GoldenGen definido (§1-§7); valores de réplica = ilustrativos (§4.2.1) |
| 05 A10, 06 A14, 07 A1 (EOL de los golden) | GoldenGen normaliza a CRLF (§1.1): los golden tienen los bytes de Windows; Swift escribe CRLF (DP-02) |
| 06 A1 (cabeceras ZIP) | volcado hex una vez (§2, fila ZIP) |
| 06 A2, A4, A5, A8; 04 A-6; 07 A6, A12 | `G-NET` (§4.1); libm del Mac como referencia (§1.1) |
| 05 A1-A3 (EscrutadorComb, Hashtable, comparer) | `G-NET-HASH`, `G-NET-SORT`, `GE-07…GE-09` |
| 07 A2-A3, A15 (corpus, exactitud XmlTextWriter, cultura de muestras) | corpus §6 (generado con los escritores reales) |
| 08 A19, 09 A-73-2 (aleatoriedad) | `G-NET-RND` + regla de §5 |
| 09:2084 G-4, README «Estado de la verificación» (ViewModels sin oráculo) | niveles O1/O2/O3 (§5) |
