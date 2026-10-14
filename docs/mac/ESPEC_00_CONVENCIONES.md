# ESPEC 00 — Convenciones de la especificación para la reescritura Swift

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Estas especificaciones
> describen **qué hace hoy** el código C# con el detalle suficiente para reescribirlo en Swift
> **sin mirar el C#** y obtener resultados idénticos (M-R1). El C# sigue siendo el oráculo.

## 1. Índice de especificaciones

| Fichero | Alcance |
|---------|---------|
| `ESPEC_01_MOTOR_NUCLEO.md` | Generador de columnas, Analizador, grupos, tolerancias, If-Then, estado global, enums |
| `ESPEC_02_FILTROS_A.md` | Pesos numéricos, Interrupciones, Contactos, Signos seguidos |
| `ESPEC_03_FILTROS_B.md` | Distancias, No variantes, Formatos 1-2-3, Valoración, Simetrías, Dibujos, Formatos de signos, Diferencias, Grupos de equipos, Estadísticas |
| `ESPEC_04_CP_RELACIONES.md` | Columnas probables, control de fallos CP, relaciones CP1/CP2/CP3/GE1, impresión |
| `ESPEC_05_REDUCCION_ESCRUTINIO.md` | Reductores, subir categoría, escrutinio y premios |
| `ESPEC_06_UTILS_ANALISIS.md` | Codificación de columnas, comparadores, combinaciones, bases, LAE, porcentajes, análisis |
| `ESPEC_07_FORMATOS_ONLINE.md` | Todos los formatos de fichero, datos iniciales, idioma, online y caché |
| `ESPEC_08_UI_A.md` · `09_UI_B` · `10_UI_C` | Las 108 pantallas: comportamiento, casos de uso, área Mac propuesta; shell |
| `ESPEC_11_ARQUITECTURA_SWIFT.md` | **Cómo** se construye: módulos, modelo de datos, errores, concurrencia, inyección, estado de app, persistencia, rendimiento |
| `ESPEC_12_ORACULO.md` | GoldenGen: qué genera, formato y layout de los golden, cómo los carga Swift, casos de uso y aleatoriedad |
| `ESPEC_13_CASOS_DE_USO.md` | Firma Swift de los 193 casos de uso (entradas, salidas, errores, progreso, dependencias) |
| `ESPEC_14_PRUEBAS.md` | Pirámide de pruebas, dobles, cobertura, CI y presupuesto de rendimiento |
| `ESPEC_15_DISENO_UI.md` | Sistema de diseño, estructura de ventana, mockups por tipo de pantalla, estados, accesibilidad, atajos |
| `ESPEC_16_PIEZAS_TRANSVERSALES.md` | `FormatoNET`, `EscritorXMLNET`, ZIP `.z3q`, codificaciones de texto, `AleatorioNET`, helpers únicos |
| `GLOSARIO.md` · `DECISIONES.md` | Nombres canónicos e IDs · registro único de decisiones (cierra todas las «Abiertas») |

**Precedencia.** Comportamiento: ESPEC_01–10 (y el oráculo). Estructura, tipos, errores, concurrencia y nombres:
ESPEC_11 + GLOSARIO (sustituyen las «propuestas Swift» de 01–10 cuando choquen). Decisiones: `DECISIONES.md`.

## 2. Plantilla obligatoria por componente

```
### <Componente> — `<ruta C#>`
**Propósito.** Una o dos frases.
**Tipos y datos.** Campos con tipo C# → tipo Swift exacto (Int32/Int64/Float/Double/UInt8…),
rangos, invariantes, valores por defecto.
**Entradas / salidas.** Parámetros, retorno, efectos (ficheros, estado global).
**Algoritmo.** Pseudocódigo fiel, en el mismo orden de iteración que el C#. Incluir
desempates, redondeos, divisiones enteras, condiciones de corte y contadores.
**Estado global.** Miembros de `VariablesGlobales` leídos/escritos.
**Peculiaridades a preservar.** Comportamientos raros o aparentes bugs que afectan a la salida
(se replican; se anotan como «preservar»).
**Trampas Swift.** Las de §3 que aplican aquí, concretas.
**Diseño Swift.** Módulo (ESPEC_11 §3), capa (configuración `struct` / ejecución `final class`, §4),
visibilidad, casos de `ExcepcionNET` que lanza (§5) y dependencias de `Entorno` que necesita (§7).
**Casos golden propuestos.** Entradas concretas para que `tools/GoldenGen` genere la salida (formato: ESPEC_12).
**Evidencia.** `fichero:línea` de cada afirmación (R4).
```

Los componentes ya escritos en 01–10 sin el apartado «Diseño Swift» lo heredan de las reglas generales de ESPEC_11
(motor = ejecución interna; `F*Data`/condiciones = configuración pública; servicios = `Entorno`).

## 3. Trampas C# → Swift — catálogo único (aplica a todas las specs)

| # | Diferencia | Evidencia | Regla Swift |
|---|-----------|-----------|-------------|
| T1 | **Overflow y desplazamientos**: C# envuelve en silencio (no hay `checked`) y enmascara el contador de desplazamiento (`int` &31, `long` &63); Swift aborta en `+ - *` y en `<<`/`>>` con contador variable se comporta distinto. Casos: `(uint)` de −1 ⇒ `0xFFFFFFFF`; `1 << -1` ⇒ `Int32.min` que se **extiende de signo** al combinarse con `long`. | 0 usos de `checked`/`unchecked` en `Free1X2.Domain`; 03 §0.5 B1-B3; 06 §1.8; 05 §7 T1 | `&+ &- &* &<< &>>` donde el C# pueda desbordar o el contador no sea constante; `Int64(UInt32(bitPattern: Int32(idx)))` para `(uint)`; `Int64(Int32(1) &<< Int32(idx))` para `1 << idx`. Sumas que nunca desbordan (p. ej. puntos CP ≤ 300, 04 S9): aritmética normal. |
| T2 | **Columna = `long`** 3 bits/partido; el índice base-3 es otro tipo. | `Utils/UtilColumnas.cs`, `docs/MANUAL_FLUJOS.md` §2; 05 §7 T2 | `Columna = Int64` (signo necesario en rutas con carácter inválido); `IndiceBase3 = Int32`; no se mezclan (GLOSARIO). `>>` aritmético como en `long`. |
| T3 | **`float` (32 bits)** en porcentajes, valoraciones, probabilidades. | `Utils/Porcentajes.cs:329-349`, `Analisis/ApuestaProbable.cs:26`, `MotorCalculo/Estadisticas/Estadistica.cs:7` | `Float`, nunca `Double`, donde el C# usa `float`; mismo orden de operaciones y mismos casts intermedios. |
| T4 | **`Math.Round` = redondeo bancario** (ToEven) por defecto. | `Analisis/ContenedorAnalisis.cs:426-429`, `Utils/LAE.cs:146`, `MotorCalculo/Analizador.cs:246` | `.rounded(.toNearestOrEven)`; `Math.Round(x, 2)` requiere réplica exacta. **Ojo:** el *formateo* `ToString("F2")` NO redondea igual (T14). |
| T5 | **Ordenación inestable** (`List.Sort`/`Array.Sort` = introsort; inserción con ≤ 16 elementos). Con claves enteras puras es determinista; con objetos y empates no; `NaN` va primero en `Array.Sort` de `double`. | `Utils/UtilidadesEntradasValores.cs:261,288,388`, `Reduccion/JDCdoble.cs:33,329`, `MotorCalculo/Analizador.cs:253`, `EntradaSalida/GenerarCPs/CPs.cs:216`; 05 A3; 04 A-9 | Enteros: cualquier sort. Con empates: el orden lo fija el oráculo (ESPEC_12) y se replica con `IntrosortNET` (ESPEC_16 §6) solo donde el golden lo exige. |
| T6 | **Orden de iteración de colecciones hash**: `Hashtable` sin orden garantizado; `Dictionary` .NET conserva inserción si no hay borrados; `Dictionary` Swift es aleatorio. | `Escrutinio/EscrutadorComb.cs:42,235`, `MotorCalculo/Grupo.cs:44,318`, `MotorCalculo/Diferencia.cs:15` | Si la salida depende del orden: `OrderedDictionary` (swift-collections) o array ordenado, documentando el orden real (oráculo). |
| T7 | **Cultura y parseo**: unos puntos fijan `es-ES`, otros usan la cultura actual (C1–C13). `Convert.ToInt32/ToDouble/ToInt16/ToBoolean(string)` ≠ `Int(_:)`/`Double(_:)` (espacios, signo, miles, excepciones). `ToUpper()` sin cultura usa la actual, carácter a carácter. | `EntradaSalida/AConfiguracion.cs:373-410`; 07 §2.1; 03 B4; 04 D4, S2, S7; 02 §0.9.4; 05 §7 T7 | Todo parseo/formato numérico pasa por `FormatoNET` con `Cultura` explícita (ESPEC_16 §1; DP-01). `convertirAInt32(_:) throws(ErrorDominio)` replica `Convert.ToInt32`. Mayúsculas por carácter ASCII (las letras de tolerancia son A–Z), nunca `uppercased()` (`ß`→`SS`). |
| T8 | **Texto**: `Encoding.Default` (.NET 8) = UTF-8; `StreamWriter` sin BOM; `WriteLine` = `\r\n` en Windows; `.dat`/`.lang` en Latin-1; las líneas en blanco **no** se saltan; el fichero de salida se crea al escribir la primera línea. | `EntradaSalida/ArchivoIdioma.cs:25`, `EntradaSalida/ArchivoColumnasTexto.cs:60`; 07 §2.2; 05 §1.2 | Leer aceptando `\r\n`, `\n`, `\r` y BOM; escribir los mismos bytes que hoy en Windows (DP-02) con `TextoNET` (ESPEC_16 §4). |
| T9 | **División entera** y `%` con negativos: misma semántica (trunca hacia 0). | 04 S10; 05 §7 T9 | Mantener tipos enteros; no promover a `Double`. |
| T10 | **Estado global** (`VariablesGlobales`, 44 miembros estáticos; `AppState`; hooks estáticos). | `VariablesGlobales.cs`; 01 §3; 10 §1.7 | `Configuracion` (valor) + `Sesion` (app) + `Entorno` (ESPEC_11 §4, §7, §8). Valores que el C# **captura al construir** (p. ej. `NumeroPartidos` en `ConvertidorDeBases()`, 05 §1.2) se pasan al construir. |
| T11 | **Comparación de cadenas**: `CompareTo`/`string.Compare` culturales; `==`, `switch`, `IndexOf(char)` ordinales sobre UTF-16; Swift `==` usa equivalencia canónica Unicode y cuenta `Character`. | 8 usos de `CompareTo`/`string.Compare`; 04 S8; 06:216-218 | Documentar ordinal/cultural en cada caso; índices y longitudes con `utf16`; literales con tilde precompuesta (NFC). |
| T12 | **Bombeo de UI** (`UiPump.Pump`, `IProgressNotifier`) y diálogos dentro de bucles largos. | `Abstractions/UiHooks.cs:12-14`, `Abstractions/IProgressNotifier.cs` | `ReportadorProgreso` + cancelación cooperativa (ESPEC_11 §6.2); avisos devueltos en el resultado (§5.1). |
| **T13** | **`Double` no finito o fuera de rango → entero.** `(int)x` con `NaN`/`±∞`/fuera de rango es «indefinido» en C#; en el runtime x64 de .NET 8 (el de la app, `win-x64`) da `Int32.MinValue` (`0x80000000`). `Convert.ToInt32(double)` redondea a par (T4) y **lanza `OverflowException`** con `NaN`/fuera de rango. Swift `Int(x)` aborta. Casos reales: `NumberBox` vacío entrega `NaN` (08 §1.5; 10:381, 10:829, 10:1341, 10:1375). | 10:381, 10:829, 10:1341, 10:1375; 08 §1.5 | `FormatoNET.castInt32NET(_:)` (= `(int)`: trunca; no finito o fuera de rango ⇒ `Int32.min`) y `FormatoNET.convertToInt32NET(_:) throws(ErrorDominio)` (= `Convert.ToInt32(double)`). El valor de `castInt32NET` lo confirma GoldenGen en x64 (ESPEC_12 G-NET). Nunca `Int(_:)` ni `Int(exactly:)` sobre datos de usuario. |
| **T14** | **Formato numérico y de tiempo de .NET más allá de la cultura**: `double.ToString()`/`float.ToString()` = representación de ida y vuelta **más corta** (.NET Core 3.0+), científica fuera de `[1e-5, 1e15)` con `E+XX`; `"F0"`, `"F2"`, `"{0:f2}"`, `"0.00"` redondean el valor binario exacto **«half away from zero»** (no ToEven); patrones con secciones `#,##0.00;0.00`; `"r"`; `TimeSpan.ToString()` = `[-][d.]hh:mm:ss[.fffffff]`; `ToLongTimeString()`/`ToShortDateString()` dependen de la cultura. | 07 §2.1 (`formatearDouble`); 10:588, 10:782, 10:808, 10:899, 10:1048, 10:1477, 10:1513, 10:1701 | Implementar en `FormatoNET` (ESPEC_16 §1) con tabla golden propia (ESPEC_12 G-NET); prohibido `String(format:)`, `description` y `formatted()` para texto que compara con la app actual. |
| **T15** | **API de colecciones y cadenas**: `string.Split(',')` conserva vacíos (Swift `split` los omite); `char[]` sin inicializar = `'\0'`; `Char.IsLetter` = categorías L*; `a ..< b` con `a > b` aborta; índices fuera de rango abortan; `bool[]` puede ser `null`; `bool[,]`; parámetros mutados (`columna >>= 3`); `switch` sin `default`; listas con duplicados significativos. | 04 S1, S5, S6, S11; 03 B6; 02 §0.9.1-3, 5-6; 05 §7 fila «Swift» | `split(separator:omittingEmptySubsequences: false)`; centinela `"\0"` solo donde el C# no inicializa; categoría Unicode explícita; `stride`/`while` en vez de rangos con extremos de datos; `[Bool]?`; array plano `[Bool]` con `e*ancho+d`; `var c = columna`; `default: break`; `[Int]`, nunca `Set`, cuando hay duplicados. |
| **T16** | **Semántica de referencia y estado por instancia**: filtros, CP, `GrupoEquipos`, `Diferencia`, `FormatoSignos`, `Simetria` comparten referencias y guardan contadores de su último `Analizar`; no son reentrantes. | 03 B5; 04 S3, §0.2-0.3; 02 §0.3; 01 §4.1 regla 8 | Capa de ejecución de ESPEC_11 §4.3: `final class` internas, un grafo por ejecución/tarea; la configuración es valor (§4.2). |
| **T17** | **Excepciones**: el C# lanza `FormatException`/`IndexOutOfRange`/`NullReference`/`Overflow`/`InvalidCast` sin capturarlas en el motor; Swift abortaría. | 03 B7; 04 S4; 06 §1.8; 02 §0.9.7 | `throw ErrorDominio(<ExcepcionNET>)` en el mismo punto lógico (ESPEC_11 §5); nada de `fatalError`/`!`. |
| **T18** | **Literales visibles**: mensajes con dobles espacios, tildes, `ª`, `Nº`, textos en inglés heredados; forman parte de la salida (golden y tests). | 04 S12; 02 §0.9.10; 10:330 | Copiar byte a byte; no localizar (DP-07). |

### 3.1 Equivalencias con los catálogos locales (las referencias antiguas siguen siendo válidas)

| Catálogo local | → Catálogo único |
|----------------|------------------|
| 03 §0.5: B1, B2, B3 · B4 · B5 · B6 · B7 | T1 · T7 · T16 · T15 · T17 |
| 04 §0.7: S1 · S2 · S3 · S4 · S5 · S6 · S7 · S8 · S9 · S10 · S11 · S12 | T15 · T7 · T16 · T17 · T15 · T15 · T7 · T11 · T1 · T9 · T15 · T18 |
| 05 §7: filas T1…T12 · fila «Swift» | mismo número · T15 + T17 (+ «profundidad como parámetro»: ESPEC_11 §4.3) |
| 02 §0.9: 1, 2, 3 · 4 · 5, 6 · 7 · 8 · 9 · 10 | T15 · T7 · T15 · T17 · ESPEC_11 §10 · T9/T5 · T18 |

## 4. Reglas de redacción

- Español, conciso, tablas y pseudocódigo; sin pegar bloques largos de C#.
- Toda afirmación con `fichero:línea` (rutas relativas a la raíz del repo).
- Lo no verificado no se escribe (R4). Las dudas van a una sección final **«Abiertas»** y se registran en `DECISIONES.md`.
- Nombres Swift propuestos en `UpperCamelCase` para tipos y `lowerCamelCase` para miembros,
  conservando el término de dominio en español (p. ej. `FiltroContactos`, `columnaBase`); si hay duda, manda `GLOSARIO.md`.
