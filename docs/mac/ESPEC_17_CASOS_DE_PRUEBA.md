# ESPEC 17 — Casos de prueba: catálogo funcional (fase M1)

> Qué se prueba y de dónde sale cada esperado. Complementa [ESPEC_12](ESPEC_12_ORACULO.md) (origen y formato de los esperados),
> [ESPEC_13](ESPEC_13_CASOS_DE_USO.md) (casos de uso) y [ESPEC_14](ESPEC_14_PRUEBAS.md) (niveles, CI, rendimiento).
> **Regla del dueño (2026-10-14):** el programa original solo se **lee**; nada de él se ejecuta. Citas `NN §x` = `mac/ESPEC_NN_*.md`.

## 0. Cómo se lee

- **ID** `CP-<ÁREA>-<nn>[-<k>]`. Áreas: `COL` columnas y codificaciones · `RNG` rangos · `TRA` tramos · `SUM` álgebra de columnas ·
  `GEN` generador y analizador · `FIL` condiciones · `CP` columnas probables · `RED`/`SUB` reducción y subir categoría · `ESC` escrutinio ·
  `ANA` análisis · `FMT`/`Z3Q` ficheros · `ONL` online · `NET` comportamiento .NET replicado · `CU` casos de uso · `UI` pantallas ·
  `NF` no funcionales · `INF` infraestructura de pruebas.
- **Fuente** del esperado: `test`, `fixture`, `lectura`, `modelo`, `bcl`, `propiedad` (ESPEC_12 §1). **Nivel**: N1–N7 (ESPEC_14 §1).
- **Datos**: **D** = ya escrito en `Free1X2.Mac/Packages/Free1X2Kit/Sources/Free1X2Pruebas/Golden/` · **C** = definido aquí; su JSONL
  se escribe al empezar el componente, **antes del código** (§4).
- Un componente se cierra cuando todos sus CP están en verde (ESPEC_14 §8).

## 1. Tests heredados: qué sirve

Se leyeron los 21 ficheros de `Free1X2.Domain.Tests` (138 casos: 89 `[Fact]` + 49 `[InlineData]`). De cada uno se conserva la
**expectativa funcional** y se descarta la forma C# (`DataSet` «Resultados», `BitArray`, rutas temporales, API de 3 bytes).
Resultado: **104 casos golden** con fuente `test` y cita `fichero:línea`, más intenciones convertidas en propiedades.

| Fichero | Casos | Qué prueba | Aprovechamiento | CP |
|---------|------:|------------|-----------------|----|
| `UtilColumnasTests.cs` | 12 | columna de 3 bits (`1`=4, `X`=2, `2`=1), ida y vuelta, aciertos + fallos = 14 con datos reales | todo; los 2 `[Fact]` sobre `Comparador` se reescriben sobre `Aciertos` de Base | COL-01…03, 10 |
| `ConvertidorDeBasesTests.cs` | 14 | columna ↔ número en base 3 (`1`=0, `X`=1, `2`=2; partido 1 = dígito menos significativo) | todo | COL-04…07 |
| `RangosHelperTests.cs`, `RangosHelperBoolTests.cs` | 6, 5 | texto de rangos → enteros y → `bool[]` de longitud máx + 2 | todo | RNG-01, 02 |
| `SumadorCombinacionesTests.cs` | 5 | unión 15, intersección 5, resta 5, deduplicado 10 → 6 | todo (pantalla 3, CU-A03) | SUM-01…05 |
| `TramoTests.cs` | 6 | acumulador de tramo: P14…P10, importe, balance (0,5 por columna), suma de tramos | todo | TRA-01…06 |
| `FlujosClaveTests.cs` | 4 | calcular 81 → grabar 81 → reducir JDC → escrutar | todo | GEN-01, 02 · RED-06 · ESC-01 |
| `ManualFuncionalTests.cs` | 5 | ejemplos del manual: NoVariantes 1/28/364, CP «4 grandes» 81 → 9, 46 656 | todo | GEN-03, 04 · CP-01 |
| `EscrutinioReduccionFixtureTests.cs` | 2 | escrutinio de 10 columnas reales; JDC sobre fichero real | todo | ESC-02 · RED-07 |
| `JDCDobleContadorGoldenTests.cs` | 5 | salida **completa** del reductor «menos columnas» (7 y 8 triples, niveles 11–13, externas) | todo; son los golden más fuertes | RED-01…05 |
| `RelacionCP1GoldenTests.cs` | 2 | relaciones I entre CP: validez de 243 columnas, SHA-256 del volcado, textos de fallo | todo | CP-02, 03 |
| `CompresorZipTests.cs` | 1 | ida y vuelta `.z3q` de 3^14 bits; 597 872 bytes de datos | todo | Z3Q-01 |
| `JornadaQuinielaParserTests.cs` | 6 | jornada ES/MX de muestra; JSON roto, vacío, < 14 partidos, equipo vacío | todo | ONL-01…06 |
| `JornadaQuinielaParserRealBackendTests.cs` | 2 | respuestas reales del backend (2026-06-18) | todo | ONL-07, 08 |
| `CatalogoEquiposParserTests.cs` | 8 | catálogo ES/MX, deduplicado sin mayúsculas ni espacios, 5 errores | todo | ONL-09…16 |
| `JornadaCacheRoundTripTests.cs` | 2 | caché: escribir → releer → parsear; caché corrupta = sin caché | todo | ONL-17, 18 |
| `ComparadorTests.cs` | 4 | coincidencias entre columnas | **sí**: `Comparador` no se porta (DP-08), pero la función «aciertos entre dos columnas» sí existe | COL-08 |
| `ComparadorBytesTests.cs` | 21 | API de 3 bytes + coincidencias | 9 (coincidencias e invariante); los 12 de la API de 3 bytes no (DP-08) | COL-08, 09 |
| `Comparador2Tests.cs` | 14 | codificación «`X`=0, partido 1 = dígito más significativo» y coincidencias por tabla | 10 para `CodificacionX0MSB` (la usan 9 pantallas, ESPEC_16 §6); los 4 de `neq` son las mismas coincidencias de COL-08 (copia local de SelectorMS) | COL-11 |
| `AbstractionsTests.cs` | 8 | ganchos de UI no-op por defecto | intención → `Entorno.pruebas` determinista | INF-03 |
| `ReductorTMGoldenTests.cs` | 6 | `ReductorTM` | no (no seleccionable, DP-08) | — |

Hallazgos de la lectura que fijan casos nuevos:
- `Escrutador.PremiosTotales` se indexa por **nº de aciertos** (0…14) (`EscrutinioReduccionFixtureTests.cs:44-53`); el de
  `EscrutadorComb`, por la **posición de la categoría** tras ordenarlas descendente (`EscrutarCombinacionesFrmViewModel.cs:414-416`) → ESC-06.
- `ConvertidorDeBases` (`X`=1, partido 1 menos significativo) y `CodificacionX0MSB` (`X`=0, partido 1 más significativo) dan números
  distintos para la misma columna; no se mezclan (ESPEC_11 §4.4) → COL-16.

## 2. Catálogo por área

### 2.1 `CP-COL` — columnas y codificaciones (`Free1X2Base`; 06 §1-2; 16 §6)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| COL-01…03 | codificación de un signo; ida y vuelta de 6 columnas y de `cols_a.txt` | `1`→4, `X`→2, `2`→1; identidad | test | N2 | D |
| COL-04…07 | `ConvertidorDeBases`: 4 valores, 8 idas y vueltas, máximo, `cols_a.txt` | `"X1111111111111"`→1 … `4782968`→`"22222222222222"` | test | N2 | D |
| COL-08 | aciertos entre dos columnas | idénticas 14; una diferencia 13; tres 11; contra todo `X` 0; contra todo `2` 0 | test | N2 | D |
| COL-09, 10 | aciertos + fallos = 14 (4 pares y `cols_a` contra la ganadora); 1.ª de `cols_a` = ganadora ⇒ 14 | 14 | test | N2 | D |
| COL-11 | `CodificacionX0MSB`: 5 idas y vueltas; `"XXXXXXXXXXXXXX"`→0; `"11111111111111"`→2 391 484; mitades de 7 → 1093 y 2186 | literales | test | N2 | D |
| COL-12 | ida y vuelta texto ↔ `Columna` para columnas simples generadas (semilla fija) | identidad | propiedad | N3 | C |
| COL-13 | vectores de 06 §1.9; `ConvStrToLong("1", "")` con N = 14 rellena 13 unos ⇒ 2 513 169 434 916 | 06 §2.1 | modelo + lectura | N2 | C |
| COL-14 | `ConvStrToLong` no valida caracteres (G-D2) | 06 §2.1 | lectura | N2 | C |
| COL-15 | `aciertos(a, b)` = bits comunes de `a & b`; aciertos + fallos = N para pares generados | identidad | propiedad | N3 | C |
| COL-16 | `"X1111111111111"`: `ConvertidorDeBases` → 1; `CodificacionX0MSB` → 797 161 (= (3¹³ − 1)/2) | dos valores distintos | lectura | N1 | C |

### 2.2 `CP-RNG` — rangos y listas de valores (`Free1X2Motor`; 06 §2.5-2.6)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| RNG-01, 02 | `obtenIntArray` (6 casos) y `obtenBoolArray` (5 casos, longitud máx + 2) | literales | test | N2 | D |
| RNG-03 | bordes `"1-2-3"`, `"5-3"`, `"3,3"`, `" 2 - 4"`, `"0"` | 06 §2.5 | lectura | N2 | C |
| RNG-04 | `ObtenerListaFromTxt`: `"1-3"`→`[0,1,2]`, `"2,1"`→`[1]`, `"2-4,1-2"`→`[1,2,3]`, `"1-3,3"`→`[0,1,2,2]` (deduplicado de G-D2) | 06 §2.6 | lectura | N2 | C |

### 2.3 `CP-GEN` — generador y analizador (`Free1X2Motor`; 01)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| GEN-01, 02 | 10 fijos + 4 triples, rutas A y B | 81 analizadas, 81 aceptadas; fichero de 81 líneas | test | N2 | D |
| GEN-03 | 14 triples con NoVariantes `0`/`1`/`2` | 1 / 28 / 364 | test | N2 | D |
| GEN-04 | 6 triples + 6 dobles + 2 fijos | 46 656 | test | N2 | D |
| GEN-05 | G-GEN-1/2/3: orden exacto de las columnas del DFS | 01 §2.1 | lectura | N2 | C |
| GEN-06 | sin condiciones, aceptadas = producto de signos por partido | ∏ | propiedad | N3 | C |
| GEN-07 | las rutas A/B/C/D aceptan las mismas columnas | iguales | propiedad | N3 | C |
| GEN-08 | G-AN-1…4 (analizador), G-GR-1…4 y G-GRP-MASK (grupos), G-CG-1 y G-CTL-FALLOS (controles), G-TOL (tolerancias) | 01 §2.2-2.6, §6 | lectura | N2 | C |
| GEN-09 | G-IFTHEN-OVR y los ≈156 textos de If-Then sobre 200 columnas | 01 §2.16, §6 | lectura | N2 | C |
| GEN-10 | G-ERR del núcleo (formatos inválidos) | 01 §2.7-2.10, 2.17 | lectura | N2 | C |
| GEN-11 | G-CFG: `parametros.free1x2` de la semilla, sin nodo `salir`, `PrecioApuesta="0,55"` | 01 §3.1 | fixture + lectura | N2 | C |
| GEN-12 | pronóstico `"1X"` (sin coma) = solo `1` [PRESERVAR] | 1 columna con `"1X"` + 13 × `"1"` | lectura | N1 | C |
| GEN-13 | resultado particionado = secuencial para P ∈ {1, 2, 3, 8}; sin partición con archivo-filtro | iguales | propiedad | N3 | C |

### 2.4 `CP-FIL` — condiciones (`Free1X2Motor`; 02, 03)

Cada fila es la sección «Casos golden propuestos» del filtro; los valores están allí con `file:line`. Todas: fuente `lectura` (y `modelo`
donde la spec lo marca, con su caso `lectura` de contraste), nivel N2, datos C.

| ID | Condición | Spec |
|----|-----------|------|
| FIL-01 | Pesos numéricos (columnas COLS-A) | 02 §1.9, §0.10 |
| FIL-02 | Interrupciones | 02 §2.9 |
| FIL-03 | Contactos | 02 §3.9 |
| FIL-04 | Signos seguidos | 02 §4.9 |
| FIL-05 | Distancias | 03 §1 |
| FIL-06 | NoVariantes (N1 = GEN-03) | 03 §2 |
| FIL-07 | Formatos 1-2-3 (F1–F3) | 03 §3 |
| FIL-08 | Valoración de signos (V1–V4) | 03 §5 |
| FIL-09 | Simetrías (S1–S4) | 03 §6 |
| FIL-10 | Dibujos (Di1–Di3) | 03 §8 |
| FIL-11 | Formatos de signos (FS1–FS4) | 03 §9 |
| FIL-12 | Diferencias (Df1–Df3) | 03 §13 |
| FIL-13 | Columnas comunes G1–G9 | 03 §0.1 |
| FIL-14 | Grupos de equipos: G-E1…G-E5 (+ RelacionGE1, CP-11) | 03 §15; 04 §11-12 |
| FIL-15 | Letras de tolerancia y filtro parcial | 02 §0.5 |
| FIL-16 | G-D2: If-Then asigna en vez de combinar; `aciertosMax` de Formatos 1-2-3 no se guarda; Dibujos vía If-Then rechaza siempre; Peso numérico vía If-Then siempre `false` (DEC-203) | ESPEC_12 §4.2.2 |
| FIL-17 | Diferencias + tolerancias ⇒ `ErrorDominio(.general)` «The method or operation is not implemented.» (DEC-104) | `MC/FiltroDiferencias.cs:124-127` |

### 2.5 `CP-CP` — columnas probables y relaciones (`Free1X2Motor`; 04)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| CP-01 | «4 grandes»: 4 triples + CP que pide 3 ó 4 aciertos | 81 → 9 | test | N2 | D |
| CP-02, 03 | relaciones I con suma estrecha y con recorrido estrecho (6 CP, 3 relaciones, 243 columnas) | cadena de validez, SHA-256 y textos de fallo exactos | test | N2 | D |
| CP-04 | `ColumnaProbable` | 04 §1 | lectura | N2 | C |
| CP-05 | `FiltroColProbables` | 04 §2 | lectura | N2 | C |
| CP-06, 07 | control de fallos: CP G04-CP-08, controles C1/C2 | 04 §3-4 | lectura | N2 | C |
| CP-08 | `RelacionCP1` y su controlador (G04-R1-01, G04-RC1-01) | 04 §5-6 | lectura | N2 | C |
| CP-09 | `RelacionCP2` (G04-R2-01…05, G04-RC2-01) | 04 §7-8 | lectura | N2 | C |
| CP-10 | `RelacionCP3` (G04-RC3-01: series refrescadas) | 04 §9-10 | lectura | N2 | C |
| CP-11 | `RelacionGE1` (GE1; G04-GE1-04: se pierde el fallo de la 1.ª) | 04 §11-12 | lectura | N2 | C |
| CP-12 | `impresoras.cfg` de la semilla | 18 entradas (G04-IMP-01) | fixture | N2 | C |
| CP-13 | `FColProbablesData` | 04 §15 | lectura | N2 | C |
| CP-14 | generador de CPs (N = 4) | 04 §16 | lectura | N2 | C |
| CP-15 | N = 15 en RelacionCP1/CP3 | `ErrorDominio(.indiceFueraDeRango)` (DEC-402) | lectura | N1 | C |
| CP-16 | `values=""` en CP o tolerancias al leer | error tipado (DEC-403) | lectura | N1 | C |

### 2.6 `CP-RED` / `CP-SUB` — reducción y subir categoría (`Free1X2Motor`; 05 §2-3, §6)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| RED-01…05 | `JDCDobleContador` sobre universos de 7 y 8 triples | contadores + salida exacta (o su SHA-256) | test | N2 | D |
| RED-06, 07 | `JDC` sobre 81 columnas y sobre `cols_a.txt` | iniciales 81 / 10; 0 < finales ≤ iniciales; líneas = finales | test | N2 | D |
| RED-08 | `JDC`: GR-01…04, GR-14 | 05 §6 | lectura | N2 | C |
| RED-09 | `JDCDobleContador`: GR-03, 05, 06 (`Partido < noPartidos`, DEC-506), 07, 14 | 05 §6 | lectura + modelo | N2 | C |
| RED-10 | `Redu1305Xfsf`: GR-11, 12, 14 | 05 §6 | lectura | N2 | C |
| RED-11 | cobertura: ∀ m ∈ madre ∃ r ∈ salida con aciertos(m, r) ≥ nivel; finales ≤ iniciales | cierto | propiedad | N3 | C |
| RED-12 | cancelación: salida parcial donde el C# la deja (05:836) | ESPEC_11 §6.2 | lectura | N4 | C |
| SUB-01 | `SubirCategoria.Calculos`: GS-01…03 | 05 §3 | lectura | N2 | C |

### 2.7 `CP-ESC` — escrutinio (`Free1X2Motor`; 05 §4)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| ESC-01 | 81 columnas contra la 1.ª | P14 = 1; 81 puntuadas | test | N2 | D |
| ESC-02 | `cols_a.txt` contra la ganadora | 14→1, 8→1, 7→2, 6→2, 5→2, 4→2; total 10 | test | N2 | D |
| ESC-03 | `Escrutador`: GE-01…06 | 05 §6 | lectura | N2 | C |
| ESC-04 | `S` (suspendido) solo acierta contra `S`; `*` es comodín | DEC-908 | lectura | N1 | C |
| ESC-05 | `EscrutadorComb` (DEC-501/502): sin columna aceptada ⇒ premios 0 y lista vacía; 1.ª aceptada vecina ⇒ `.referenciaNula`; 1.ª aceptada = ganadora ⇒ `.conversionInvalida`; recorrido en el orden de `HashtableNET`; GE-07…10 | DEC-501 | lectura | N2 | C |
| ESC-06 | índice de `PremiosTotales`: aciertos (`Escrutador`) frente a posición de categoría (`EscrutadorComb`) | §1 | lectura | N1 | C |
| TRA-01…06 | `Tramo` | 1255; balance 950; −5; propiedades; suma de tramos | test | N2 | D |

### 2.8 `CP-ANA` / `CP-SUM` — análisis y álgebra de columnas (`Free1X2Motor`; 06 §2.7-3)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| SUM-01…05 | unión, intersección, resta, deduplicado y relaciones de conjuntos | 15, 5, 5, 6; \|A∪B\| = \|A\|+\|B\|−\|A∩B\| | test | N2 | D |
| ANA-01 | `Porcentajes` (fila de ceros ⇒ NaN; N ≠ 14, DEC-613) | 06 §2.7 | lectura | N2 | C |
| ANA-02 | `LAE` | 06 §2.8 | modelo + lectura | N2 | C |
| ANA-03 | analizador de análisis | 06 §3.1 | lectura | N2 | C |
| ANA-04 | `AnalizadorColumnaCombinacion` (corta al primer fallo, DEC-604) | 06 §3.3 | lectura | N2 | C |
| ANA-05 | `ContenedorAnalisis`: las 3 combinaciones del manual + cada filtro activo, con las 3 banderas | 06 §3.5 | lectura | N2 | C |

### 2.9 `CP-FMT` / `CP-Z3Q` — ficheros (`Free1X2Formatos`; 07; 16 §2-4)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| Z3Q-01 | ida y vuelta de 3^14 bits con 9 activos | 597 872 bytes de datos; mismos bits | test | N2 | D |
| Z3Q-02 | lector tolerante: método 0/8, descriptor de datos, extras Zip64; CRC distinto ⇒ `.entradaSalida` | DEC-601/616 | lectura | N1 | C |
| Z3Q-03 | escritor: 1 entrada, versión 20, banderas 0, CRC y tamaños en la cabecera local | 16 §3 | lectura | N1 | C |
| FMT-01 | `AConfiguracion` (G-CFG) | 07 §6.1 | fixture + lectura | N2 | C |
| FMT-02 | `.comb`: lectura/escritura; `EsGrupoBase` cuenta comentarios (DEC-102) | 07 §6.2 | lectura | N2 | C |
| FMT-03 | condiciones: un fichero por filtro (14), If-Then vacío y con rangos | 07 §6.3 | lectura | N2 | C |
| FMT-04 | datos de los 14 filtros: extremos y lectura → escritura | 07 §6.4 | lectura | N2 | C |
| FMT-05 | `ArchivoColumnasTexto` con `cols_a.txt` en LF y en CRLF | 07 §6.5 | fixture + lectura | N2 | C |
| FMT-06 | `ColumnasProbables.xml` de la semilla: 1 tipo, 2 configuraciones; leer → escribir idéntico | 07 §6.7 | fixture | N2 | C |
| FMT-07 | corpus: lectura = modelo, escritura = bytes (CRLF, BOM por formato), ida y vuelta = bytes originales | ESPEC_12 §6 | fixture + lectura | N2 | C |
| FMT-08 | ficheros de Windows: CRLF/LF/CR, BOM, `.dat` Latin-1; rutas `C:\…` en `.comb` se conservan | ESPEC_11 §9 | lectura | N2 | C |

### 2.10 `CP-ONL` — online (`Free1X2Online`; 07 §6.9-6.14; `docs/API_CLUBPROGOL.md`)

| ID | Qué se comprueba | Esperado | Fuente | Nivel | Datos |
|----|------------------|----------|--------|-------|-------|
| ONL-01…08 | parser de jornada: muestras ES/MX, reales ES/MX y 4 errores | campos exactos; `FormatException` con «JSON» / «14» | test | N2 | D |
| ONL-09…16 | parser del catálogo: muestras, deduplicado, 5 errores | `["Real Madrid","Sevilla","Eibar"]`; `FormatException` | test | N2 | D |
| ONL-17, 18 | caché: escribir → releer → parsear; corrupta ⇒ sin caché | `jornada-es.json`; `cargada = false` | test | N4 | D |
| ONL-19 | cliente: 200; 404 `{"error":"sin_jornada",…}`; 429 con `Retry-After`; timeout 10 s; sin red | 07 §6.10 | lectura | N4 | C |
| ONL-20 | anti-doble petición del `actor` cliente | 07:750-763 | lectura | N4 | C |
| ONL-21 | propiedad JSON duplicada | gana la última (DEC-704) | bcl | N1 | C |
| ONL-22 | enteros `38.0`, `1e2`, > `Int32` | rechazados (DEC-705) | bcl | N1 | C |
| ONL-23 | caché de los dos países con `mtime` distinto; escritura atómica | 07 §6.11 | lectura | N4 | C |
| ONL-24 | tema/idioma: `"Ingles"`, `"ingles"`, `"1"`, `"99"`, `3`, `{}`, `[]`, basura, vacío | 07 §6.13-6.14; DP-12 | bcl + lectura | N4 | C |

### 2.11 `CP-NET` — comportamiento .NET replicado (`Free1X2Base`; ESPEC_12 §4.1; 00 T13-T14; 16 §1)

Fuente `bcl` (código de `dotnet/runtime` `release/8.0`), nivel N1, datos C. Cultura `es-ES`.

| ID | Entrada | Esperado |
|----|---------|----------|
| NET-01 | `castInt32NET(NaN)`, `(±∞)`, `(3e9)` | −2 147 483 648 (T13) |
| NET-02 | `castInt32NET(2.9)`, `(−2.9)` | 2, −2 (trunca) |
| NET-03 | `convertToInt32NET(2.5)`, `(3.5)`, `(−2.5)`; `(NaN)` | 2, 4, −2 (a par); `.desbordamiento` |
| NET-04 | `(0.1 + 0.2).ToString()` | `"0,30000000000000004"` |
| NET-05 | `1e14`, `1e15`, `0.0001`, `0.00001` con `ToString()` | `"100000000000000"`, `"1E+15"`, `"0,0001"`, `"1E-05"` |
| NET-06 | `0.5`, `1.5`, `2.5` con `"F0"`; `0.125` con `"F2"` | `"1"`, `"2"`, `"3"`; `"0,13"` (mitad lejos de cero, T14) |
| NET-07 | `−0.0`, `NaN`, `+∞`, `−∞` con `ToString()` | `"-0"`, `"NaN"`, `"∞"`, `"-∞"` |
| NET-08 | `TimeSpan` (1 h 2 min 3 s), (+4 ms), (1 d 2 h 3 min 4 s) | `"01:02:03"`, `"01:02:03.0040000"`, `"1.02:03:04"` |
| NET-09 | `Convert.ToInt32` de `" 3"`, `"+3"`, `"3 "`; de `"3,5"` y `""` | 3, 3, 3; `.formato` |
| NET-10 | `Convert.ToDouble` de `"3,5"`, `"3.5"`, `"1e2"` | 3,5; 35 (punto = miles); 100 |
| NET-11 | `Convert.ToBoolean` de `"True"`, `"true"`, `" true "`; de `"1"` | `true`; `.formato` |
| NET-12 | `Array.Sort` de `[3, NaN, 1]` | `[NaN, 1, 3]` (DT-15) |
| NET-13 | `ReadLine` sobre `\r\n`, `\n`, `\r`; BOM UTF-8 | separa en los tres; BOM descartado |
| NET-14 | `new Random(s)` para s ∈ {0, 1, 42, −1, Int32.max, Int32.min}: `Next()`, `Next(n)`, `Next(a, b)`, `NextDouble()` | modelo que transcribe `Random.Net5CompatImpl.cs` (ESPEC_16 §5) |
| NET-15 | orden de `Hashtable.Keys` con las claves de `EscrutadorComb` | modelo que transcribe `Hashtable.cs` (ESPEC_16 §6) |

### 2.12 `CP-CU` — casos de uso (`Free1X2CasosDeUso`; ESPEC_13; 08-10)

**Regla:** cada uno de los 203 CU (193 + 10 del shell) tiene al menos un caso N4 con su nivel O1/O2/O3 (ESPEC_13 §5); el JSONL se
escribe con el CU (datos C). Casos ya fijados:

| ID | CU | Qué se comprueba | Esperado / fuente |
|----|----|------------------|-------------------|
| CU-01 | CU-S01 `arrancar` | crea las 10 carpetas si faltan; copia semillas solo si no existen; `Configuracion` todo-o-nada; jornada desde caché sin red | ESPEC_11 §9; ESPEC_13:74 |
| CU-02 | CU-S06 `guardarEquipos` | UTF-8 con BOM y CRLF | ESPEC_13:79 |
| CU-03 | CU-S07, S08 | borra `*_tmp.comb` de `Temp/` e `Informe*` de `Informes/`; devuelve cuántos | ESPEC_13:80-81 |
| CU-04 | CU-A28 `calcularCombinacion` | = GEN-01…04 por el caso de uso | test |
| CU-05 | CU-A03 `algebraDeColumnas` | = SUM-01…05 con ficheros | test |
| CU-06 | CU-A58, A59 (comprimir/descomprimir) | = Z3Q-01 | test |
| CU-07 | CU-C19 `reducirColumnas` | = RED-01…07 | test |
| CU-08 | CU-B07 `escrutarColumnas` | = ESC-01, 02 | test |
| CU-09 | CU-B04 `escrutarCombinaciones` | ESC-05 y el mensaje «No se pudo completar el escrutinio:\n\n…» | `EscrutarCombinacionesFrmViewModel.cs:496-499` |
| CU-10 | CU-B09 `estimarPremios` | `Math.Round` a par; acertantes según la recaudación previa | DEC-910 |
| CU-11 | CU-A39…A41 (coincidencias) | máx. 100 condiciones y 3000 ganadoras; usa la ganadora del visor | DEC-807 |
| CU-12 | CU-A87 (tramos por límites) | la fila 10 usa los extremos de la fila 9 | DEC-816 |
| CU-13 | CU-A53 (combinar filtros) | `N2s` con ≠ 14 partidos: el desbordamiento sale como error tipado | DEC-812 |
| CU-14 | CU-A17, A19, A20, A22 (aleatorios con semilla de reloj) | O3: distribución y rangos con `Entorno.aleatorio` inyectado | ESPEC_12 §5 |

### 2.13 `CP-UI` — pantallas y shell (`App`; ESPEC_15; ESPEC_14 §9)

| ID | Qué se comprueba | Esperado | Nivel |
|----|------------------|----------|-------|
| UI-01 | smoke de las 109 superficies | `SMOKE DONE total=109 ok=109 fail=0` | N6 |
| UI-02 | argumento `-smoke` | mismo resultado en `free1x2_smoke.log` del directorio temporal; inerte sin el argumento | N6 |
| UI-03 | menús y barra | 7 menús, 53 destinos; 6 grupos, 55 botones con `accessibilityLabel` = tooltip | N5 |
| UI-04 | ventana | única, 1020 × 760, mínimo 900 × 600; cerrarla cierra la app | N6 |
| UI-05 | navegación | `navegar` no duplica; `navegarSiempre` sí; `Esc`/`⌘[` = atrás | N5 |
| UI-06 | alertas | cola FIFO de una en una; `confirmar` sin ventana ⇒ `false` | N5 |
| UI-07 | error no capturado | «Se produjo un error inesperado:\n\n{mensaje}»; la app sigue viva | N5 |
| UI-08 | modo claro y oscuro | tokens de ESPEC_15 §1 | N6 |
| UI-09 | idioma | solo «Créditos» en inglés (DP-07) | N5 |
| UI-10 | flujos principales | calcular, reducir, escrutar, abrir/guardar combinación, actualizar jornada | N6 |

### 2.14 `CP-NF` — no funcionales

| ID | Qué se comprueba | Esperado | Nivel |
|----|------------------|----------|-------|
| NF-01 | tiempos B1–B8 | techos de ESPEC_14 §6.3 | N7 |
| NF-02 | asignaciones en el bucle caliente | 0 por columna en B1/B2 | N7 |
| NF-03 | paralelismo | B1/B2 ≥ 2× con P ≥ 2 | N7 |
| NF-04 | memoria | pico ≤ 64 MB (B1–B3), ≤ 128 MB (B8) | N7 |
| NF-05 | cancelación | termina en ≤ 4 096 iteraciones del bucle externo con `.operacionCancelada`, o deja la salida parcial donde el C# la deja | N4 |
| NF-06 | progreso | como mucho un evento cada 100 ms, más el final; el bucle caliente informa cada 65 536 columnas (DT-46) | N1 |
| NF-07 | sin abortos | lint: sin `fatalError`, `preconditionFailure`, `try!`, `as!` ni `!` de desempaquetado en `Sources/` | CI |
| NF-08 | determinismo | dos ejecuciones con la misma entrada ⇒ bytes idénticos | N3 |
| NF-09 | compilación | Swift 6 estricto; avisos = errores en CI | CI |

### 2.15 `CP-INF` — infraestructura de pruebas (ejecutable desde el esqueleto, M2)

| ID | Qué se comprueba | Esperado |
|----|------------------|----------|
| INF-01 | `indice.json` | cada suite existe y tiene el nº de casos indicado; todo `.jsonl` está listado; IDs únicos; cada caso tiene `fuente` válida, `salida` o `error`, `cita` si es `test` y `traza` si es `lectura` |
| INF-02 | fixtures | SHA-256 = el de su origen en `Free1X2.Domain.Tests/Fixtures/` (copia byte a byte) |
| INF-03 | `Entorno.pruebas` | determinista: reloj fijo 2026-01-01 12:00 UTC; ficheros en memoria; red sin respuestas ⇒ error |
| INF-04 | CI | `mac.yml` verde |

## 3. Recuento

| Área | Casos con datos (D) | Filas definidas (C) |
|------|-------------------:|--------------------:|
| COL | 45 | 5 |
| RNG | 11 | 2 |
| TRA, SUM | 11 | — |
| GEN, CP-01 | 7 | 9 + 15 (CP) |
| FIL | — | 17 |
| RED, SUB | 7 | 6 |
| ESC | 2 | 4 |
| ANA | — | 5 |
| FMT, Z3Q | 1 | 10 |
| ONL | 18 | 6 |
| NET | — | 15 |
| CU | — | 14 + 203 CU (regla) |
| UI, NF, INF | — | 10 + 9 + 4 |
| **Total** | **104** (`test`) + 10 fixtures | — |

## 4. Cuándo se escribe cada dato (DT-29)

- **M1 (cerrado con este documento):** catálogo, 104 casos `test`, 10 fixtures copiados byte a byte e infraestructura (INF-01…03).
- **Al empezar cada componente** (M3, M4 y M5), antes de su código: su JSONL con los CP «C» de su área, con `traza` y citas `file:line`.
  El componente se cierra cuando su suite está en verde (ESPEC_14 §8).
- Por qué: los esperados ya están fijados en las specs con `file:line`. Transcribirlos junto al componente (*test-first*) evita datos huérfanos
  y hace que los revise quien implementa.
