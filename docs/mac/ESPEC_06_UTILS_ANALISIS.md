# ESPEC 06 — Utils y Análisis (modelo de columna, comparadores, bases, LAE, porcentajes, análisis)

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Plantilla y trampas T1–T12 en
> [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Modelo conceptual en
> [`../MANUAL_FLUJOS.md`](../MANUAL_FLUJOS.md) §2 (columna como `long`) y §6 (análisis).
> Alcance: `Free1X2.Domain/Utils/*.cs` (14 ficheros, 3 220 líneas) y `Free1X2.Domain/Analisis/*.cs`
> (17 ficheros, 1 835 líneas). Todo lo que sigue describe **lo que hace hoy el C#** (el C# es el oráculo, M-R1).

**Abreviaturas de ruta** (todas relativas a la raíz del repo): `U/` = `Free1X2.Domain/Utils/` ·
`A/` = `Free1X2.Domain/Analisis/` · `MC/` = `Free1X2.Domain/MotorCalculo/` ·
`T/` = `Free1X2.Domain.Tests/` · `ACT` = `Free1X2.Domain/EntradaSalida/ArchivoColumnasTexto.cs`.

**Convención de lectura.** «Preservar» = comportamiento raro que se replica. «EXC» = el C# lanza excepción
(en Swift: `throws` con error tipado, nunca `fatalError`, salvo que se indique). Los valores «esperado
(modelo)» de los casos golden los calculó un modelo Python de esta espec, **no** el oráculo C# (aún no existe
`tools/GoldenGen`): el oráculo manda y cualquier discrepancia se resuelve a favor del C#.

## 0. Mapa de componentes

31 componentes (14 Utils + 17 Analisis). «Vivo» = tiene consumidores fuera de su fichero en
`Free1X2.Domain`, `Free1X2.WinUI` o `Free1X2` (verificado con búsqueda de símbolos); «solo tests» /
«muerto» = ningún consumidor (se especifica igualmente, marcado, para que el dueño decida si se porta).

| # | Componente | Fichero (líneas) | Uso | Módulo Swift propuesto |
|---|-----------|------------------|-----|------------------------|
| U1 | `UtilColumnas` | `U/UtilColumnas.cs` (489) | vivo; 7 miembros muertos (ver §2.1) | `Columnas/UtilColumnas` |
| U2 | `Comparador` | `U/Comparador.cs` (390) | vivo (XFSF, xfsfV3, `A/Analizador.IteraCols` muerto) | `Columnas/Comparador` |
| U3 | `Comparador2` | `U/Comparador2.cs` (98) | solo tests (`SelectorMSViewModel.cs:405` tiene copia propia) | `Columnas/Comparador2` |
| U4 | `ConvertidorDeBases` | `U/ConvertidorDeBases.cs` (186) | vivo (Escrutinio, Reducción, Sumador, Compresor…) | `Columnas/ConvertidorDeBases` |
| U5 | `RangosHelper` | `U/RangosHelper.cs` (143) | vivo (27 refs) | `Columnas/RangosHelper` |
| U6 | `UtilidadesEntradasValores` | `U/UtilidadesEntradasValores.cs` (458) | vivo (UI y `*Data`) | `Columnas/UtilidadesEntradasValores` |
| U7 | `Porcentajes` | `U/Porcentajes.cs` (549) | vivo (`ValoresBase100/Neperianos`); E/S de ficheros solo en WinForms | `Columnas/Porcentajes` |
| U8 | `LAE` | `U/LAE.cs` (151) | vivo (`EstimadorPremiosFrmViewModel.cs:394`) | `Columnas/LAE` |
| U9 | `SumadorCombinaciones` | `U/SumadorCombinaciones.cs` (333) | vivo (`AlgebraColumnasFrmViewModel.cs:180`) | `Columnas/SumadorCombinaciones` |
| U10 | `Tramo` | `U/Tramo.cs` (123) | vivo (Tramificar) | `Columnas/Tramo` |
| U11 | `TablaDeSeleccion` | `U/TablaDeSeleccion.cs` (58) | vivo (diálogo banco de pruebas) | `Columnas/TablaDeSeleccion` |
| U12 | `Combinacion` | `U/Combinacion.cs` (52) | vivo (análisis múltiple de tramos) | `Columnas/Combinacion` |
| U13 | `Pral` | `U/Pral.cs` (41) | vivo (CombinarFiltros) | `Columnas/Pral` |
| U14 | `CompresorZip` | `U/CompresorZip.cs` (149) | vivo (`CompresorViewModel.cs:137,174`); `CreateZip/ExtractZip` muertos | `Columnas/CompresorZ3Q` |
| A1 | `Analizador` (Análisis) | `A/Analizador.cs` (213) | vivo (`ProbabilidadPremiosViewModel.cs:138`) | `Analisis/AnalizadorProbabilidades` |
| A2 | `ProbabilidadesPremios` | `A/ProbabilidadesPremios.cs` (76) | vivo (solo vía A1) | `Analisis/ProbabilidadesPremios` |
| A3 | `AnalizadorColumnaCombinacion` | `A/AnalizadorColumnaCombinacion.cs` (162) | vivo (`EscrutadorComb.cs:112`) | `Analisis/AnalizadorColumnaCombinacion` |
| A4 | `ContenedorAnalisisGlobal` | `A/ContenedorAnalisisGlobal.cs` (79) | vivo (MC `Analizador`, `ControlGrupos`, `ControladorGrupos`) | `Analisis/ContenedorAnalisisGlobal` |
| A5 | `ContenedorAnalisis` | `A/ContenedorAnalisis.cs` (684) | vivo | `Analisis/ContenedorAnalisis` |
| A6 | `ContenedorColumnasProbables` | `A/ContenedorColumnasProbables.cs` (69) | vivo | `Analisis/ContenedoresSimples` |
| A7 | `ContenedorDiferencias` | `A/ContenedorDiferencias.cs` (55) | vivo | ídem |
| A8 | `ContenedorFormatos` | `A/ContenedorFormatos.cs` (38) | vivo | ídem |
| A9 | `ContenedorSimetrias` | `A/ContenedorSimetrias.cs` (36) | vivo | ídem |
| A10 | `ContenedorValoraciones` | `A/ContenedorValoraciones.cs` (36) | **muerto** (0 refs) | ídem (opcional) |
| A11 | `ApuestaProbable` | `A/ApuestaProbable.cs` (29) | vivo (`RentabilidadFrmViewModel.cs:116`) | `Analisis/Apuestas` |
| A12 | `ApuestaProbableCentral` | `A/ApuestaProbableCentral.cs` (30) | vivo (`BancoPruebasFrmViewModel.cs:537`) | ídem |
| A13 | `ApuestaProbEscrutada` | `A/ApuestaProbEscrutada.cs` (32) | vivo (`TramificarFormViewModel.cs:315`) | ídem |
| A14 | `FigurasComparer` | `A/FigurasComparer.cs` (35) | solo WinForms (`VisorAnalisisColumnasFrm.cs:397-482`) | `Analisis/FigurasComparer` |
| A15 | `InformeColumnasABDON` | `A/InformeColumnasABDON_UnaColTodasGan.cs` (62) | vivo (`EstucolFrmViewModel.cs:35`) | `Analisis/InformeColumnasABDON` |
| A16 | `TransponedorAutomatico` | `A/TransponedorAutomatico.cs` (90) | solo WinForms (`RotacionDeSignosFrm.cs`) | `Analisis/TransponedorAutomatico` |
| A17 | `clAnalisisColumnas` | `A/clAnalisisColumnas.cs` (109) | **muerto** (0 refs) | `Analisis/ResumenCondicion` (opcional) |

---

## 1. Modelo de columna (base de todo el Swift)

### 1.1 Codificación de un partido: 3 bits

Una **columna** es un entero de 64 bits con signo (`long`) que empaqueta un signo por partido en 3 bits.
El partido `k` (numeración **1-based**) ocupa los bits `[3(k−1), 3(k−1)+2]`; el **partido 1 está en los
bits menos significativos**. Cada bit del slot es un signo posible: bit0 (valor 1) = `2`, bit1 (valor 2) =
`X`, bit2 (valor 4) = `1`. El orden de bits sale de la cadena `"2X1"` indexada con `IndexOf`
(`U/UtilColumnas.cs:87,178,192,201,212`).

| Slot | Bits | Significado (apuesta/pronóstico) | `ConvLongToStr` (`:40`) | `ConvPartidoByteToStr` (`:134-167`) | `ConvPartidoStrToByte` acepta (`:94-132`) |
|------|------|----------------------------------|--------------------------|--------------------------------------|-------------------------------------------|
| 0 | 000 | vacío / partido fuera de grupo | `'2'` si hay slot superior ≠ 0; **omitido** si es el último (el bucle termina cuando el resto es 0) | `""` | cualquier otra cadena → 0 |
| 1 | 001 | `2` | `'2'` | `"2"` | `"2"` |
| 2 | 010 | `X` | `'X'` | `"X"` | `"X"` |
| 3 | 011 | `X2` (doble) | `'X'` (**pierde el 2**) | `"X2"` | `"X2"`, `"X,2"` |
| 4 | 100 | `1` | `'1'` | `"1"` | `"1"` |
| 5 | 101 | `12` (doble) | `'1'` (**pierde el 2**) | `"12"` | `"12"`, `"1,2"` |
| 6 | 110 | `1X` (doble) | **EXC** (`"2X1"[3]` fuera de rango) | `"1X"` | `"1X"`, `"1,X"` |
| 7 | 111 | `1X2` (triple) | **EXC** | `"1X2"` | `"1X2"`, `"1,X,2"` |

`ConvPartidoStrToByte` es **sensible a mayúsculas y al orden**: `"x"`, `"2X"`, `"21"`, `"X1"`, `"2,1"` → 0.

Familias de valores según uso (mismo formato, distinto contenido de los slots):

| Familia | Slots válidos | Dónde |
|---------|---------------|-------|
| Apuesta simple («columna») | solo `{1,2,4}` | generador, filtros, escrutinio |
| Pronóstico / apuesta múltiple | `1…7` (OR de signos) | `pronosticoBase`, `ConvStrToLong(string[])` (`:78-92`), `ObtenerLongFromTxtApuestaMultiple` (`:369-378`) |
| Máscara de partidos/grupo | `{0,7}` | `Grupo.ColumnaGrupo = columna & mascara` (`MC/Grupo.cs:160-163`), `ObtenerMascaraParcial` (`:445-461`) |
| Columna parcial | slot 0 en los partidos no marcados | `ConvStrToLong(string, bool[])` (`:205-220`) |

Operaciones núcleo (ya documentadas en MANUAL_FLUJOS §2; aquí solo la regla exacta):
- **Pertenencia a pronóstico:** `(pron & col) == col`.
- **Aciertos entre dos apuestas:** `ContarBitsA1(ganadora & apuesta)` = popcount de 64 bits (`:299-309`) → Swift `nonzeroBitCount`.
- **Contar signos por tipo:** `popcount(col & máscara_signo)` con máscaras `FiltroNoVariantes.cs:107,111`, `FiltroDibujos.cs:46-47` (ver §1.3).

### 1.2 Reparto de conceptos «columna / texto / índice»

| Concepto | Tipo C# | Base de índice | Orden | Notas |
|----------|---------|----------------|-------|-------|
| Columna empaquetada | `long` | partido 1 = bits 0-2 | LSB = primer partido | `ObtenerSignoInt(L, partido)` desplaza `(partido−1)*3` (`:75`) |
| Texto de columna | `string` `"1X2…"` | char 0 = partido 1 | izquierda→derecha | mayúsculas: `1`, `X`, `2` (los ficheros se normalizan a mayúsculas+trim en `ACT:214-217`; `UtilColumnas` **no** normaliza) |
| Pronósticos | `string[N]` | elemento 0 = partido 1 | — | cada elemento `"1,X,2"`/`"1X2"`/`"1"`… |
| `bool[] partidos` de `IgnorarPartidos`, `ObtenerMascaraParcial`, `ObtenerBoolArrayFromTxt(txt)` | `bool[]` | **1-based con el índice 0 sin usar** (longitud N+1) | — | `IgnorarPartidos` itera `i = len−1 … 1` (`:433`); `ObtenerBoolArrayFromTxt(txt)` crea `N+1` (`U/UtilidadesEntradasValores.cs:282-302`) |
| `bool[] partidosMarcados` de `ConvStrToLong(s, bool[])` y `Grupo.partidosActivos` | `bool[]` | **0-based**, alineado con el texto | — | `MC/Grupo.cs:67,321` |
| Número base-3 | `int` | partido 1 = dígito menos significativo (`ConvertidorDeBases`) | — | ver §1.8 |

### 1.3 Constantes

| Constante | Valor | Derivación | Evidencia |
|-----------|-------|------------|-----------|
| Bits por partido | 3 | — | `UtilColumnas.cs:41` (`>>= 3`) |
| `columnaBase` (archivo, sin pronóstico) | `281474976710655` | `2^48 − 1` (16 slots a 7) | `MC/GeneradorColumnas.cs:30` |
| `columnaInicial(N)` = «todo `1`» | N=1…16: 4 · 36 · 292 · 2340 · 18724 · 149796 · 1198372 · 9586980 · 76695844 · 613566756 · 4908534052 · 39268272420 · 314146179364 · **2513169434916 (N=14)** · 20105355479332 (15) · 160842843834660 (16) | `4·(8^N−1)/7` (verificado para los 16 valores) | `MC/GeneradorColumnas.cs:36-91` (por `NumeroPartidos`, `switch` en `:38`) y `:92-143` (por parámetro; el constructor de archivo lo llama con `ObtenNumSignos()`, `:33`); **N fuera de 1…16 → 0** (no hay `default`) |
| Máscara signo `2` (N partidos) | `m2(N) = (8^N−1)/7` | `40210710958665` (N=16), `5026338869833` (N=15) | `FiltroDibujos.cs:47`, `FiltroNoVariantes.cs:111` |
| Máscara signo `X` | `2·m2(N)` | `80421421917330` (N=16) | `FiltroDibujos.cs:46`, `FiltroNoVariantes.cs:107` |
| Máscara signo `1` | `4·m2(N)` | = `columnaInicial(N)` | — |
| Potencias de 3 (0…16) | 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, **4782969 (3^14)**, **14348907 (3^15)**, 43046721 (3^16) | — | `U/ConvertidorDeBases.cs:28`, `A/Analizador.cs:40` |
| Universo de columnas | `3^N` | 14→4 782 969 · 15→14 348 907 · 16→43 046 721 | `T/CompresorZipTests.cs:20` |

Los filtros usan máscaras de 15/16 partidos aunque N=14: los bits por encima de N son 0 en columnas válidas,
así que el resultado no cambia (no replicar la inconsistencia: derivar `m2(N)` del `N` activo está permitido
**solo si** el golden de cada filtro lo confirma; por defecto copiar las constantes).

### 1.4 Conversiones string ↔ long (todas en `U/UtilColumnas.cs`)

Notación: `w(c)` = peso de un carácter: `'2'→1`, `'X'→2`, `'1'→4`; **cualquier otro carácter** (minúsculas,
`,`, `*`, espacio…) → `IndexOf = −1` → `1 << −1`: C# enmascara el contador de desplazamiento de `int` a 5 bits,
así que vale `1 << 31 = Int32.min`. Ese `int` se **extiende con signo** al combinarse con `long` (`:178,192,201,212`):
contribución `Int64(Int32.min) = −2147483648` (XOR, no OR) → columna negativa y basura en los bits altos.
**Preservar** (no es un caso funcional; solo importa que el Swift no aborte y dé el mismo valor).

| Función (línea) | Algoritmo | Casos límite |
|-----------------|-----------|--------------|
| `ConvStrToLong(string s)` `:196-204` | `res = 0; for i = len−1 … 0: res = (res &<< 3) ^ w(s[i])` | `""→0`; >21 caracteres: los bits altos se pierden sin error; carácter inválido: ver arriba |
| `ConvStrToLong(string[] Pbase)` `:78-92` | para `i = n−1 … 0`: `b = 0; for j = 0, 2, 4… < len(Pbase[i]): b \|= (1 << idx(Pbase[i][j]))` ; `L = (L << 3) \| UInt32(b)` (**zero-extend**: `(uint)b`) | lee **solo las posiciones pares**: `"1,X,2"→7`, pero `"1X"→4` (solo la `1`) y `"12"→4`; elemento `""→0`; carácter inválido en posición par → bit 31 (`0x80000000`, positivo) |
| `ConvStrToLong(s, string relleno)` `:168-181` | `relleno == ""` → `ConvStrToLong(s, NumeroPartidos − s.Length)` (la sobrecarga **int**); si no, `s += relleno` y bucle de `:196` | con `relleno==""` y `s.Length ≥ NumeroPartidos` → EXC (índice −1); se usa en `MC/GeneradorColumnas.cs:289` con `CompletarCon` |
| `ConvStrToLong(s, int faltan)` `:186-195` | `s += "1"×faltan` (tabla `signosRelleno`, 15 entradas, `:184`) y bucle | `faltan ∉ 1…15` → EXC |
| `ConvStrToLong(s, bool[] marcados)` `:205-220` | bucle `i = len−1 … 0`; si `marcados[i]` suma `w(s[i])`, si no `res <<= 3` (slot 0) | `marcados` más corto que `s` → EXC |
| `ConvLongToStr(long L)` `:27-44` | `while L ≠ 0: out.append("2X1"[(L & 7) >> 1]); L >>= 3` | longitud = posición del último slot ≠ 0; slots 6/7 → EXC; `L<0` nunca llega a 0 (`>>` aritmético): EXC al 23.º carácter (`stackalloc char[22]`, `:36`) |
| `ConvPartidoStrToByte` / `ByteToStr` `:94-167` | tablas de §1.1 | — |
| `ObtenerLongFromTxtApuestaMultiple(string[])` `:369-378` | `for i = n−1 … 0: L = (L << 3) \| ConvPartidoStrToByte(p[i])` | 0 consumidores |
| `ObtenerStringFromLongApuestaMultiple(long)` `:379-388` | `while L ≠ 0: s += ByteToStr(L & 7) + ","; L >>= 3`; devuelve `s` sin la última coma | `L == 0` → EXC (`Substring(0,−1)`); slot 0 intermedio → elemento vacío (`"1,,X"`); `L<0` → bucle infinito (precondición `L ≥ 0`) |
| `ObtenerSignoInt(long L, int partido)` `:66-76` | `Int((L &>> ((partido−1)*3)) & 7)` | C# enmascara el contador a 6 bits (`partido ≤ 0` o `> 22` ⇒ desplazamiento módulo 64) |
| `ObtenerSigno(L, partido)` `:51-64` | `"2X1"[((L >> (partido−1)*3) & 7) >> 1]` como string de 1 char | slots 6/7 → EXC; 0 consumidores |
| `CambiarSignoPartido(L, partido, signo, partidos)` `:405-427` | `for i = partidos … 1: col = (col << 3) \| (i == partido ? signo : ObtenerSignoInt(L, i))` | 0 consumidores; `B` no se usa |
| `IgnorarPartidos(bool[] p, long col)` `:429-444` | `m = 0; for i = len−1 … 1: if p[i] { m = (m << 3) \| ObtenerSignoInt(col, i) }` — **compacta** los partidos marcados (el de menor índice queda en los 3 bits bajos) | `p[0]` se ignora; usado en `MC/Diferencia.cs:53,140` |
| `ObtenerMascaraParcial(bool[] p)` `:445-461` | `for i = len−1 … 1: m = (m << 3) \| (p[i] ? 7 : 0)` | partido `i` en el slot `i−1`; 0 consumidores |
| `PonerPartidoATriple(bool[] p, long col)` `:390-403` | `for i = len−1 … 1: if !p[i] { c \|= 7 }; c <<= 3`; `return c \| col` | **el slot de `p[i]` queda en `3i` (un slot por encima del estándar `3(i−1)`)**: inconsistente con `ObtenerMascaraParcial`; 0 consumidores → portar solo si se decide, tal cual |
| `ConvStrToLongDosBits(string)` `:463-487` | `ToUpper()` y `res = (res << 3) ^ {'1'→2,'X'→3,'2'→1,'-'→0}` (otro char: solo desplaza) | codificación **distinta** (2 bits útiles en slot de 3); 0 consumidores; `ToUpper()` usa cultura actual |
| `ConvStrToInt32(string)` `:222-297` | compresión experimental en nibbles: pares `11→1,1X→2,12→3,X1→4,XX→5,X2→6,21→7,2X→8,22→9`; resto impar `1→10,X→11,2→12`; `res = (res << 4) ^ v` en `int` (desbordamiento silencioso → `&<<`) | **el último par se pierde si `len` es par** (el bucle sale con `colTemp.Length == len` antes de procesar `temp`); par desconocido: se descarta sin desplazar; 0 consumidores |

### 1.5 Número de partidos variable (14 / 15 / 16)

- `NumeroPartidos` vive en `parametros.free1x2` (`EntradaSalida/AConfiguracion.cs:145-150`, `Convert.ToInt16`) con
  defecto 14 (`VariablesGlobales.cs:152`) y se expone como estática (`:199-202`). El generador soporta 1…16 (§1.3).
  Cualquier flujo que lo cambie debe hacerlo **antes** de crear el motor (MANUAL_FLUJOS §10).
- En Swift (M-R4) pasa a un valor explícito `numeroPartidos: Int` en `Sesion`/`ConfiguracionBoleto` inyectado a cada
  componente que hoy lo lee (§0 de cada ficha lo indica).
- Capacidad por componente (si el N activo la supera, el C# lanza o desborda; el Swift debe lanzar el mismo error):

| Componente | N máximo | Evidencia |
|------------|----------|-----------|
| `ConvLongToStr` | 21 partidos con signo positivo (buffer 22) | `UtilColumnas.cs:36` |
| `ConvStrToLong(s, int)` | relleno 1…15 | `:184` |
| `Comparador` | 15 (3 bytes × 5 tritos); N=14 se completa con `'1'` | `Comparador.cs:274-299` |
| `Comparador2` | **14 fijo** (2 mitades × 7) | `Comparador2.cs:46-47` |
| `ConvertidorDeBases` | 16 (17 potencias, índices 0…16) | `ConvertidorDeBases.cs:28-29` |
| `SumadorCombinaciones` | 15 (`BitArray(3^15)`) | `SumadorCombinaciones.cs:41` |
| `CompresorZip` | 14 o 15 | `CompresorZip.cs:90,123-127` |
| `A/Analizador` | 16 (`pot` de 17) | `Analizador.cs:40,63` |
| `TransponedorAutomatico` | 14 fijo | `TransponedorAutomatico.cs:29-32` |
| `ContenedorAnalisis` | parametrizado (`numPartidos`) | `ContenedorAnalisis.cs:352-365` |

### 1.6 Pleno al 15

El Pleno al 15 **no tiene codificación propia**: es el 15.º partido, con el mismo alfabeto `1/X/2`. En los `.txt` es
el carácter 15 de la línea (`WinUI/Views/Ported/AgregaP15FrmViewModel.cs:247,300`: `columna.Substring(14,1)` con casos
`"1"`, `"x"/"X"`, `"2"`) y en la columna empaquetada es el slot 15 (bits 42-44) cuando `NumeroPartidos = 15`
(`columnaInicial(15)`, §1.3). El pleno **real** de la Quiniela (marcador 0/1/2/M por equipo) no se modela en Utils/Análisis;
la jornada online lo transporta aparte (`Online/JornadaQuiniela.cs:35-36`). Efectos al pasar columnas de 15 signos:

| Componente | Comportamiento con 15 signos |
|------------|------------------------------|
| `ConvStrToLong` / `ConvLongToStr` | 45 bits, sin problema |
| `ConvertidorDeBases.ConvColumnaANumero(columna)` | usa `columna.Length` → número < 3^15 (§2.4) |
| `Comparador` | el 15.º signo **sí cuenta** en las diferencias (§2.2): `coincidencias = 14 − difs15`, puede dar < 0 → byte 255 |
| `Comparador2.s1n` | ignora el 15.º (solo `Substring(0,7)` y `Substring(7,7)`) |
| `CompresorZip` | acepta 14 o 15 signos (`CompresorViewModel.cs:89`); BitArray 3^15 |
| `SumadorCombinaciones` | `Bits` ya es 3^15 |

### 1.7 Otras codificaciones presentes (NO unificar)

| Codificación | Mapa de dígitos | Orden | Dónde |
|--------------|-----------------|-------|-------|
| **Base-3 «ConvertidorDeBases»** | `'1'→0, 'X'→1, '2'→2` (cualquier otro carácter ⇒ `X`) | partido 1 = dígito **menos** significativo; texto leído de izquierda a derecha con peso `3^i` | `ConvertidorDeBases.cs:43-54`; índice de los `BitArray` de columnas (Sumador, Compresor, A/Analizador) |
| **Tritos de `Comparador`** | igual que la anterior (`'1'→0, 'X'→1, '2'→2`, otro⇒X) | 3 bytes × 5 tritos; trito `i` pesa `3^i`; byte 1 = partidos 1-5, byte 2 = 6-10, byte 3 = 11-15 | `Comparador.cs:301-314` |
| **Mitades de `Comparador2`** | `'X'→0, '1'→1, '2'→2` (otro⇒0); **distinto** | 2 mitades de 7; el **primer** carácter es el dígito **más** significativo; `n = nx1·2187 + nx2` | `Comparador2.cs:70-89` |
| **Figuras** | un nibble por valor | `"x-y-z"` → `(x<<8)\|(y<<4)\|z` | `UtilidadesEntradasValores.cs:220-253` |
| **DosBits** / **Int32 comprimido** | ver §1.4 | — | muertos |

### 1.8 Reglas Swift del modelo

- `typealias Columna = Int64` (T2). Los bucles calientes pueden usar `UInt64` vía `bitPattern` sin cambiar resultados
  (todo valor válido es `< 2^48`); las rutas con carácter inválido o `L < 0` exigen `Int64` para reproducir el signo.
- `<<` y `>>` de Swift **no abortan** (desplazamiento «inteligente»), pero con contador **variable** difieren de C#
  (que enmascara a 6 bits en `long` y 5 en `int`): usar `&<<`/`&>>` siempre que el contador no sea constante
  (`ObtenerSignoInt`, `1 << idx`). Aritmética (`+ - *`) con riesgo de desbordar: `&+ &- &*` (T1).
- `Int32(1) &<< Int32(idx)` reproduce `1 << idx` también para `idx = −1`.
- `(uint)b` ⇒ `Int64(UInt32(bitPattern: b))`; `(byte)x` ⇒ `UInt8(truncatingIfNeeded: x)`.
- Índices y longitudes de texto: C# cuenta unidades UTF-16; usar `Array(s.utf16)` en cualquier función que indexe por
  posición (`Comparador`, `UtilColumnas.Compara*`, `ConvertidorDeBases`), no `Character`.
- `string.IndexOf(char)` es ordinal y distingue mayúsculas; `ToUpper()` (sin cultura) usa la cultura actual (T7, T11).
- Las EXC de este modelo son errores de **programación/entrada inválida** del propio C#: modelarlas con
  `enum ErrorColumnas: Error { case indiceFueraDeRango, formato, argumentoFueraDeRango }` y cubrirlas en golden
  como «espera error» (no necesitan coincidir en el mensaje).

### 1.9 Vectores de referencia del modelo (esperado calculado por el modelo; el oráculo manda)

| Entrada | `ConvStrToLong` | Vuelta `ConvLongToStr` |
|---------|-----------------|-------------------------|
| `"1"` / `"X"` / `"2"` | 4 / 2 / 1 | `"1"` / `"X"` / `"2"` |
| `"1X2"` | 84 (`0x54`) | `"1X2"` |
| `"11111111111111"` (14×`1`) | 2513169434916 | igual |
| `"22222222222222"` / `"XXXXXXXXXXXXXX"` | 628292358729 / 1256584717458 | igual |
| `"1X21X21X21X21X"` | 1385685887060 | igual |
| `"112211221122XX"` | 1247220744804 | igual |
| `""` | 0 | `""` |
| `"1X2x"` (minúscula) | −1099511627692 | EXC (negativo) |
| `"1,X"` (coma) | −17179869052 | EXC (negativo) |
| `ConvStrToLong(["1,X","1","X,2"])` | 230 (6 + 4·8 + 3·64) | `ConvPartidoByteToStr` de cada slot: `"1X","1","X2"` |
| `ConvStrToLong(["1X"])` / `(["x"])` | 4 / 2147483648 | — |

---

## 2. Utils — fichas por componente

### 2.1 UtilColumnas — `Free1X2.Domain/Utils/UtilColumnas.cs`

**Propósito.** Conversión texto↔long y utilidades de bajo nivel sobre columnas (§1). Más tres comparadores de
cadenas (métodos de **instancia**) usados por el reductor JLPM.

**Tipos y datos.** Clase sin estado salvo la tabla `signosRelleno` (`:184`). Swift: `enum UtilColumnas` (funciones
estáticas, §1.4 da firma y algoritmo de cada una) y tres funciones libres de cadenas.

**Entradas / salidas.** Ver tabla §1.4. Sin efectos externos.

**Algoritmo.** §1.4 (conversiones) más:
- `Compara(base, col)` `:311-329`: `n = 0; for i in 0..<base.length: if base[i] != col[i] { n += 1 }`. Solo recorre `base.length`; `col` más corta → EXC, más larga → el sobrante se ignora.
- `Compara(base, col, max)` `:331-346`: ídem con `n += 1; if n > max { return n }` (devuelve `max+1` en el primer exceso).
- `ObtenNoCoincidencias(base, col)` `:348-367`: cuenta posiciones **iguales**. **Único usado** (`Reduccion/JLPM.cs:103,107,137`); `Compara` y `Compara(max)` no tienen consumidores.
- `ContarBitsA1(long)` `:299-309` = `nonzeroBitCount` de los 64 bits (también para negativos).

**Estado global.** `VariablesGlobales.NumeroPartidos` solo en `ConvStrToLong(s, "")` (`:172`).

**Peculiaridades a preservar.** (1) Todo lo de §1.4 (`"1X"` → 4, carácter inválido → `Int32.min` con signo, relleno por 15 máximo, slots 6/7 → EXC en `ConvLongToStr`). (2) `ConvLongToStr` omite los slots 0 finales: la longitud del texto depende del último partido no vacío. (3) Miembros **sin consumidores** (no son parte del contrato; portar solo si el dueño lo pide): `ObtenerSigno`, `ConvStrToInt32`, `PonerPartidoATriple`, `ObtenerMascaraParcial`, `CambiarSignoPartido`, `ConvStrToLongDosBits`, `ObtenerLongFromTxtApuestaMultiple`, `Compara`, `Compara(max)`.

**Trampas Swift.** T1 (`&<<` en `1 << idx` y en `ObtenerSignoInt`; `res << 3` nunca aborta pero pierde bits como C#), T2 (`Int64`, `>>` aritmético), T7/T11 (`ToUpper` de `ConvStrToLongDosBits` usa cultura; `IndexOf(char)` es ordinal), UTF-16 para índices (§1.8), `(uint)b` ⇒ `UInt32(bitPattern:)`.

**Casos golden propuestos.** Tabla §1.9 completa; además: `ConvStrToLong("1", "")` con N=14 (relleno 13 unos → `columnaInicial(14)`... EXC si `s.Length==14`), `ConvStrToLong("1X", 3)` (relleno `"111"`), `ConvStrToLong("1X2", [true,false,true])` (= 4 + 0 + 1·64 = 68), `ConvLongToStr(3)` (`"X"`), `ConvLongToStr(6)` (EXC), `ConvLongToStr(260)` (`"1" + "2" + "1"` = `"121"`: el slot 0 interior da `'2'`), `ObtenerSignoInt(84, 1..3)` = 4,2,1, `ObtenerSignoInt(84, 0)` (desplaza −3 ⇒ módulo 64 ⇒ 0), `IgnorarPartidos([_,true,false,true], 84)` (= 12: partido 3 aporta `1`, partido 1 aporta `4` ⇒ `(1<<3)|4`), `ContarBitsA1(-1)` = 64, `ObtenNoCoincidencias("1X2","1X1")` = 2.

**Evidencia.** `U/UtilColumnas.cs:27-487` (líneas por función en §1.4), `T/UtilColumnasTests.cs` (round-trip y pesos de 3 bits), `Reduccion/JLPM.cs:33,103,107,137`, `MC/Diferencia.cs:53,140`, `MC/FiltroSimetrias.cs:72-106`, `MC/Analizador.cs:115`.

---

### 2.2 Comparador — `Free1X2.Domain/Utils/Comparador.cs`

**Propósito.** Coincidencias/diferencias entre dos columnas de ≤ 15 signos mediante 3 bytes de 5 tritos y una tabla de distancias 243×243 (núcleo de XFSF/xfsfV3 y, en teoría, de `A/Analizador.IteraCols`).

**Tipos y datos.**
- `ConvNum1X2: [String](243)` (`:25-26`), `TDif5T: [[UInt8]](243×243)` (`:29-272`), `Pt3 = [1,3,9,27,81]`, `DosPt3 = [2,6,18,54,162]` (`:274-275`), `byteCol1`, `byteCol2: [UInt8](3)` públicos (`:277-278`), buffer `bufColumna` de 15 caracteres (`:286`).
- **Las dos tablas se regeneran**: verificado con un script sobre el fuente (243/243 cadenas y 59 049/59 049 celdas): `ConvNum1X2[n]` = 5 caracteres, el carácter `i` es el dígito base-3 `i`-ésimo de `n` (menos significativo primero) con `0→'1', 1→'X', 2→'2'`; `TDif5T[a][b]` = nº de dígitos base-3 distintos entre `a` y `b` (Hamming sobre 5 tritos). **No copiar los literales** (11 000 números): generar en `static let`.

**Entradas / salidas.** `col1`/`col2` (setter: texto → bytes; getter: bytes → texto de 14 caracteres); `CalculaCoincidencias()` y `CalculaDiferencias()` sobre `byteCol1`×`byteCol2`; variantes `(byte[] b)` (contra `byteCol1`), `ConCol2(b1,b2,b3)` (contra `byteCol2`), `CalculaCoincidencias(string, string)` (fija ambas columnas como efecto lateral y devuelve `CalculaCoincidencias()`), `Conv3BytesAColumna`. Retornos `byte`.

**Algoritmo.**
```
ConvColumnaA3Bytes(s):                                          // :288-315
  if s.length < 14 { EXC IndexOutOfRange }                      // :296
  c = primeros 15 caracteres de s; si s.length < 15, c[14] = '1'  // :297-299
  b1 = b2 = b3 = 0
  for i in 0..<5:
     b1 += w(c[i],   i);  b2 += w(c[i+5], i);  b3 += w(c[i+10], i)
  w(ch,i) = (ch == '1') ? 0 : (ch == '2') ? 2*3^i : 3^i         // cualquier otro char cuenta como X
CalculaCoincidencias   = UInt8(truncatingIfNeeded: 14 − T[a1][b1] − T[a2][b2] − T[a3][b3])   // :334-337
CalculaDiferencias     = UInt8(truncatingIfNeeded: T[a1][b1] + T[a2][b2] + T[a3][b3])         // :359-362
Conv3BytesAColumna(b1,b2,b3) = N1X2[b1] + N1X2[b2] + N1X2[b3].prefix(4)                       // 14 caracteres, :376-385
```
(`T` es simétrica, así que el orden de operandos de las sobrecargas no altera el valor.)

**Estado global.** Ninguno (estado de instancia: `byteCol1/2`, `bufColumna`).

**Peculiaridades a preservar.**
1. `< 14` caracteres ⇒ EXC; `== 14` ⇒ completa con `'1'`; `≥ 15` ⇒ solo mira los 15 primeros.
2. Con columnas de 15 signos, el 15.º **entra en la tabla**: `coincidencias = 14 − difs(15 partidos)`; si los 15 difieren ⇒ `14−15 = −1` ⇒ **255** (`(byte)` sin `checked`, T1). El invariante «coincidencias + diferencias = 14» solo vale con 14 signos (o con el 15.º igual).
3. Cualquier carácter que no sea `'1'`/`'2'` (incl. `'x'`, `'*'`, espacio) cuenta como `X`.
4. `col1`/`col2` getter devuelve 14 caracteres (descarta el 15.º).
5. Un `byte ≥ 243` en `Conv3BytesAColumna` o en la tabla ⇒ EXC (no hay validación).
6. `bufColumna` es estado compartido de instancia (no reentrante): en Swift usar un array local por llamada.

**Trampas Swift.** T1 (`UInt8(truncatingIfNeeded:)`), UTF-16 para `Length`/índices (§1.8), tablas como `static let` planas `[UInt8]` (`a*243+b`), `struct Comparador` con `mutating` o `final class` (los reductores lo mutan por columna).

**Casos golden propuestos.** (esperado = modelo)
| Caso | Esperado |
|------|----------|
| `ConvColumnaA3Bytes("1X21X21X21X21X")` | bytes `(102, 65, 34)` |
| `ConvColumnaA3Bytes("11111111111111")` | `(0,0,0)`; `Conv3BytesAColumna` vuelve a 14 unos |
| `ConvColumnaA3Bytes("111111111111112")` (15, pleno `2`) | `(0,0,162)`; `col1` getter ⇒ 14 unos |
| `CalculaCoincidencias("11111111111111","1111111111111X")` | 13 |
| `…("1X21X21X21X21X","112211221122XX")` | 5; `CalculaDiferencias()` = 9 |
| `…("111111111111112","11111111111111")` | **13** (el pleno cuenta) |
| `…("222222222222222","111111111111111")` | **255** |
| `…("1111","1111")` | EXC |
| round-trip de las 6 columnas de `ComparadorBytesTests` | idéntico |
Más todos los casos de `T/ComparadorTests.cs`, `T/ComparadorBytesTests.cs` y los dos tests de `Comparador` en `T/UtilColumnasTests.cs` (usan los fixtures `cols_a.txt`/`ganadora.txt`).

**Evidencia.** `U/Comparador.cs:25-385`; `T/ComparadorBytesTests.cs` (nota de la cabecera sobre el `"1"` fijo de la posición 15); `Reduccion/XFSF.cs:59,75,111`, `Reduccion/xfsfV3.cs:128,142`, `A/Analizador.cs:164`.

---

### 2.3 Comparador2 — `Free1X2.Domain/Utils/Comparador2.cs`

**Propósito.** Coincidencias entre columnas de **14** signos con una tabla 2187×2187 (mitades de 7). Sin consumidores en Domain/WinUI (solo tests; `SelectorMSViewModel.cs:405` mantiene su propia copia) — se especifica por completitud.

**Tipos y datos.** `comtab: int[2187,2187]` (`:21`) — sus valores son 0…7: Swift `[UInt8]` plano de 4 782 969 bytes. Estado: `c7p`, `c7u` (mitades de la columna de referencia), `n7p`, `n7u` (`:22`).

**Contenido de la tabla** (`:24-36`): `comtab[a][b]` = nº de posiciones iguales entre `n2s(a)` y `n2s(b)` (7 posiciones); `comtab[a][a] = 7` (incluido `[0][0]`). Forma cerrada: nº de dígitos base-3 coincidentes entre `a` y `b` en 7 dígitos.

**Algoritmo.**
```
n2s7(nx): s = ""; repetir 7 veces: d = nx % 3; nx /= 3; s = ((d==1) ? "1" : (d==2) ? "2" : "X") + s     // :70-79 (prepend ⇒ el último dígito queda a la izquierda)
s2n7(s):  nx = 0; for ch in s: nx = nx*3 + (ch=="1" ? 1 : ch=="2" ? 2 : 0)                            // :80-89 (cualquier otro, incl. 'X' ⇒ 0)
s1n(col): s2n7(col[0..<7]) * 2187 + s2n7(col[7..<14])                                                    // :65-69, :45-48 (Substring(0,7), Substring(7,7))
n1s(nx):  14 iteraciones de la misma regla que n2s7, prepend                                             // :55-64
e1c(colnum): c7p = colnum / 2187; c7u = colnum % 2187      e1c(a,b): c7p = a; c7u = b                    // :49-54
neq(colnum): n7p = colnum / 2187; n7u = colnum % 2187; return comtab[c7p][n7p] + comtab[c7u][n7u]       // :41-44
neq(a,b):    comtab[c7p][a] + comtab[c7u][b]                                                              // :38-40
n2s(a,b) público = n2s7(a) + n2s7(b)                                                                      // :37
```
**Estado global.** Ninguno.

**Peculiaridades.** (1) `neq` sin `e1c` previo compara contra `c7p = c7u = 0` (14×`X`). (2) `s1n` exige ≥ 14 caracteres (EXC) e ignora el 15.º. (3) `n1s(nx)` con `nx ≥ 3^14` trunca la parte alta (siempre 14 caracteres). (4) `neq(colnum)` con `colnum ≥ 3^14` ⇒ `colnum/2187 ≥ 2187` ⇒ EXC. (5) **Mapa de dígitos distinto** al de `ConvertidorDeBases`/`Comparador`: aquí `X=0, 1=1, 2=2` y el primer carácter es el más significativo (§1.7).

**Trampas Swift.** Construir la tabla perezosamente (el ctor C# tarda ~16 M operaciones); `UInt8`; no mezclar con los números de `ConvertidorDeBases`.

**Casos golden.** Los de `T/Comparador2Tests.cs`: `s1n("11111111111111") = 2391484`, `s1n("XXXXXXXXXXXXXX") = 0`, `n1s(0) = "XXXXXXXXXXXXXX"`, `s2n("11111112222222") → (1093, 2186)`, `neq` idénticas = 14, opuestas = 0, una diferencia = 13, round-trip `n2s(s2n(c))` para `"1122XX1122XX11"`; añadido (modelo): `s1n("22222222222222") = 4782968`.

**Evidencia.** `U/Comparador2.cs:21-96`; `T/Comparador2Tests.cs` (cabecera: convenciones caracterizadas).

---

### 2.4 ConvertidorDeBases — `Free1X2.Domain/Utils/ConvertidorDeBases.cs`

**Propósito.** Columna (texto o long) ↔ número base-3 que indexa los `BitArray` de 3^N columnas (§1.7).

**Tipos y datos.** `PotDe3`, `DosPotDe3: [Int](17)` (`:28-29`; `DosPotDe3[i] = 2·PotDe3[i]`), `SignoQuini = ['1','X','2',' ']` (`:31`), `_NumPartidos: UInt8` (`:32`): ctor sin argumentos = `(byte)VariablesGlobales.NumeroPartidos` (`:34-37`); ctor `(byte)` explícito (`:38-41`). Swift: `struct ConvertidorDeBases { let numPartidos: Int }`.

**Algoritmo.**
```
ConvColumnaANumero(col):            b = 0; for i in 0..<col.length: if col[i]=='1' {} elif col[i]=='2' { b += 2·3^i } else { b += 3^i }      // :43-54
ConvColumnaANumero(col, n):         igual con i en 0..<n (lee col[i] ⇒ EXC si col.length < n)                                                    // :55-66
ConvLongANumero(L):                 b = 0; p = 0; while L != 0 { bt = L & 7; 4 → +0; 1 → +2·3^p; 2 → +3^p; otro → b += 7 ; L >>= 3; p += 1 }   // :68-94
ConvNumAColumna(n) (int y long):    s = ""; repetir { s += SignoQuini[n % 3]; n /= 3 } mientras n != 0; s += "1" × (numPartidos − s.length)   // :96-117
ObtenTamañoBitArray(len)            = PotDe3[len]                                                                                                  // :118-121
ObtenNumeroPartidosColBin(len)      = k si len == 3^k (k = 0…16); en otro caso 14                                                                  // :122-…
```
**Estado global.** `NumeroPartidos` en el ctor sin argumentos.

**Peculiaridades a preservar.**
1. Cualquier carácter distinto de `'1'`/`'2'` pesa como `X` (`,`, `x`, `*`, espacio…). No normaliza a mayúsculas (lo hace `ACT:216` al leer).
2. `PotDe3` tiene 17 entradas: columna con un carácter ≠ `'1'` en posición ≥ 17 ⇒ EXC (los `'1'` no acceden a la tabla). Una línea con comas (`"1,X,2,…"`, 27 caracteres) lanza EXC.
3. `ConvLongANumero`: un slot que no sea 1/2/4 (0 interior, dobles, triples) **suma 7** (literal, no un peso); 0 consumidores. Precondición `L ≥ 0` (con `L < 0` el bucle no termina).
4. `ConvNumAColumna`: el relleno `'1'` va a la **derecha** (partido 1 primero); si el número necesita más de `numPartidos` dígitos ⇒ EXC (`Append('1', negativo)`); `n < 0` ⇒ EXC (`SignoQuini[−1]`); `n = 0` ⇒ `"1…1"`.
5. El contador `i` de `ConvColumnaANumero` es `byte`: sin sentido con > 255 caracteres (irrelevante).

**Trampas Swift.** `n % 3` con negativos (misma semántica que C#, T9, pero el índice negativo debe lanzar); `Int` 64 bits sin riesgo de desbordamiento (3^16 < 2^31); UTF-16.

**Casos golden propuestos** (esperado = modelo, N=14): `"11111111111111"→0`, `"X1111111111111"→1`, `"21111111111111"→2`, `"1X111111111111"→3`, `"XX211111111111"→22`, `"1X21X21X21X21X"→2023563`, `"22222222222222"→4782968`; `ConvNumAColumna(0)` = 14 unos, `(5)` = `"2X111111111111"`, `(4782968)` = 14 doses, `(4782969)` = EXC; `ConvLongANumero(84)` (= `"1X2"`) = 21, `ConvLongANumero(260)` = 7 (slot 0 interior), `ObtenNumeroPartidosColBin(4782969)` = 14, `(14348907)` = 15, `(43046721)` = 16, `(100)` = 14, `ObtenTamañoBitArray(14)` = 4782969. Más `T/ConvertidorDeBasesTests.cs`.

**Evidencia.** `U/ConvertidorDeBases.cs:28-186`; `ACT:216`; `T/ConvertidorDeBasesTests.cs`.

---

### 2.5 RangosHelper — `Free1X2.Domain/Utils/RangosHelper.cs`

**Propósito.** Parser de rangos `"1,3-5,7"` a `[Int]` o a `[Bool]` indexado por valor (condiciones de la UI).

**Tipos y datos.** Instancia con un `ArrayList tempBoolArray` (`:28`) como estado de trabajo (no reentrante). `int(...)` en el pseudocódigo = `Convert.ToInt32(string)`: base 10, admite espacios en blanco iniciales/finales y un signo `+`/`-` inicial; `""` ⇒ EXC `FormatException`; > `Int32` ⇒ EXC `OverflowException`. Como `-` es el separador de rango, **no hay negativos** (`"-1"` ⇒ partes `["","1"]` ⇒ EXC).

**Algoritmo.**
```
ObtenIntArray(texto):                                                          // :32-74
  for parte in texto.split(","):                       // split literal, sin eliminar vacíos
     t = parte.split("-")
     if t.count == 2 { a = int(t[0]); b = int(t[1]); for i in 0..<(b−a+1) { out.append(a+i) } }   // b<a ⇒ nada
     else            { out.append(int(t[0])) }          // 1 parte, o 3+ partes ("1-2-3" ⇒ [1])
ObtenBoolArray(texto):                                                         // :76-141
  partes = texto.split(",")
  max = 0; for p in partes { for q in p.split("-") { max = Swift.max(max, int(q)) } }   // TODAS las subpartes
  arr = [false] × (max + 2)                                                     // el último elemento queda siempre false
  for p in partes:
     if p contiene "-" { t = p.split("-"); v1 = int(t[0]); v2 = int(t[1]); for i in 0..<(v2−v1+1) { arr[v1+i] = true } }   // subpartes ≥3 se ignoran
     else               { arr[int(p)] = true }
```
**Estado global.** Ninguno.

**Peculiaridades a preservar.**
1. Longitud `max+2` (el test `RangosHelperBoolTests` lo bloquea); índice 0 siempre existe.
2. `"1-2-3"`: `ObtenIntArray → [1]`, `ObtenBoolArray → {1,2}` (más `max` = 3 ⇒ longitud 5): las dos rutas **no coinciden**.
3. Rango invertido `"5-3"`: `ObtenIntArray → []`; `ObtenBoolArray` ⇒ todo `false`, longitud 7.
4. Sin deduplicado en `ObtenIntArray` (`"3,3"` ⇒ `[3,3]`); espacios admitidos: `" 2 - 4"` ⇒ 2…4.
5. Entradas inválidas (`""`, `"a"`, `"1,,2"`, `"3-"`, `"-1"`) ⇒ EXC; los llamadores de la UI las capturan.
6. Memoria: `"999999999"` pide un `bool[10^9]` (en C# `OutOfMemoryException`): Swift debe lanzar error en vez de reservar sin límite razonable.

**Trampas Swift.** Parseo estilo `Convert.ToInt32` (recortar solo espacios en blanco Unicode ASCII `9–13, 32`, un signo, dígitos); `Int32` y overflow; no usar `split` de Swift con `omittingEmptySubsequences` por defecto (`true`): usar `omittingEmptySubsequences: false`.

**Casos golden propuestos.** Los de `T/RangosHelperTests.cs` y `RangosHelperBoolTests.cs`, más: `"1-2-3"`, `"5-3"`, `"3,3"`, `" 2 - 4"`, `"0"` (Bool: `[T,F]`), `"0-14"`, `""`, `"a"`, `"1,,2"`, `"3-"`, `"-1"`, `"2147483648"` (overflow).

**Evidencia.** `U/RangosHelper.cs:28-141`; `T/RangosHelperTests.cs`, `T/RangosHelperBoolTests.cs` (cabecera: longitud `max+2`).

---

### 2.6 UtilidadesEntradasValores — `Free1X2.Domain/Utils/UtilidadesEntradasValores.cs`

**Propósito.** Conversión texto↔lista/bool[]/figuras para los campos de condiciones de la UI y los `F*Data`.

**Tipos y datos.** Clase estática. `int(x)` = `Convert.ToInt32` (ver §2.5). `List<int>` ⇒ `[Int]`. Salidas `bool[]` ⇒ `[Bool]?` (algunas devuelven `null`).

**Algoritmo / comportamiento por función** (todas `static`; líneas en `U/UtilidadesEntradasValores.cs`):

| Función (línea) | Algoritmo | Particularidades |
|-----------------|-----------|------------------|
| `ObtenerArrayAciertos(s)` `:26-36` | `s.split(",").map(int)` | sin `try`: EXC si un elemento es inválido; sin deduplicar |
| `ObtenerListaFromTxt(s)` `:42-113` | `try`: si `s` contiene `,` ⇒ por cada parte; si no, `s` como única parte. Parte con `-` ⇒ `min=int(p[0]); max=int(p[1])`; `for j in min…max: if !lista.contains(j) { lista.append(j−1) }`. Parte sin `-` ⇒ `v=int(p); if !lista.contains(v) { lista.append(v−1) }`. `catch` ⇒ `lista.clear()` | **índices 0-based (`valor−1`) pero el filtro de duplicados compara contra `valor` sin restar** (ver peculiaridades) |
| `ObtenerListaFromTxtAciertos(s)` `:115-181` | mismo árbol que la anterior pero añade `j` / `v` (valor tal cual) y deduplica correctamente | `catch` ⇒ lista vacía |
| `ObtenerTextoFromListAciertos(lista)` `:183-195` | `lista.map(String).joined(",")` | `ToString()` de `int` (usar invariante) |
| `ObtenerTextoFromBool(arr)` `:197-212` | índices con `true`, unidos por `,`; ninguno ⇒ `""` | — |
| `ObtenerLongFiguraFromText(t)` `:220-231` | `fig = 0; for v in t.split("-"): fig = (fig << 4) \| UInt32(bitPattern: int(v))` | valor > 15 solapa nibbles anteriores; vacío ⇒ EXC; más de 16 nibbles ⇒ se pierden los altos |
| `ObtenerTextoFiguraFromLong(fig)` `:238-253` | `s = ""; while fig != 0 { s = "-" + String(fig & 15) + s; fig >>= 4 }`; devuelve `s.dropFirst()` o `"0"` si `fig == 0` | los nibbles 0 **a la izquierda** se pierden: `"0-1"` → 1 → `"1"` (no es biyectiva); `fig < 0` no termina (precondición) |
| `ObtenerBoolArrayFromTxt(txt, longitud)` `:255-280` | `txt == ""` ⇒ `nil`. `lista = ObtenerListaFromTxtAciertos(txt)` ordenada; `n = longitud > lista.last ? longitud+1 : lista.last+1`; `arr[i] = lista.contains(i)` | **`lista` vacía (texto inválido) ⇒ EXC** (`lista[−1]`) |
| `ObtenerBoolArrayFromTxt(txt)` `:282-302` | `txt == ""` ⇒ `nil`; tamaño `NumeroPartidos+1`; `arr[i] = lista.contains(i)` | valores > N se descartan; lista vacía ⇒ todo `false` (sin EXC); **usa `NumeroPartidos`** |
| `ValidarCamposDeTexto(entradas, max)` `:304-320` | `true` si `#{e == ""} <= max` | sin consumidores; no recorta espacios |
| `ObtenerTodosValores()` `:322-330` | `"0,1,…,N"` con `N = NumeroPartidos` | **usa `NumeroPartidos`** |
| `ObtenerTodosValores(min, max)` `:332-340` | `"min,…,max"` | `min > max` ⇒ EXC (`Substring(0,−1)`) |
| `ObtenerValoresSeparadosPorComas(s)` `:341-361` | por parte de `s.split(",")`: con `-` ⇒ `ObtenerTodosValores(int(p[0]), int(p[1]))`; sin `-` ⇒ la parte tal cual (sin validar); unidas por `,` | rango invertido ⇒ EXC |
| `SonTodosNumeros(s)` `:362-382` | `s == ""` ⇒ `true`; `try`: expandir con la anterior y `int(x)` de cada elemento; `catch` ⇒ `false` | `"1,,2"` ⇒ `false` |
| `ObtenerRangos(lista)` `:383-393` | **ordena `lista` in situ**; `[lista.first, lista.last]`; vacía ⇒ `[0,0]` | efecto lateral sobre el argumento (Swift: `inout`) |
| `ObtenerRangosSeparadosPorGuion(s)` `:394-403` | `v = s.split(",")`; si `v.count > 1` ⇒ `v.first + "-" + v.last`, si no `""` | **un solo valor ⇒ `""`**; sin consumidores |
| `ObtenerGruposDeValores(cant, pasoFijo)` `:404-426` | `for i in 1…N: for j in i+1…N: if j−i == cant−1 { lista.append("\(i)-\(j)"); if pasoFijo { i = j }; break }` (el `i++` del bucle externo sigue aplicándose) | **usa `NumeroPartidos`**; `cant ≤ 1` ⇒ lista vacía; con `pasoFijo` los grupos son disjuntos |
| `PonerEnMayuscula(p)` `:428-455` | si `p.length > 1`: si `p.split([".", " "]).count > 1` ⇒ para cada parte `PonerEnMayuscula(parte) + " "` concatenado (**con espacio final**); si no ⇒ `p[0].ToUpper() + p.dropFirst()`. Si `p.length ≤ 1` ⇒ `p.ToUpper()` | `"r. madrid"` ⇒ partes `["r","","madrid"]` ⇒ `"R  Madrid "` (doble espacio, espacio final); `ToUpper()` usa cultura actual y mapeo **simple** 1:1 |

**Estado global.** `VariablesGlobales.NumeroPartidos` en `ObtenerBoolArrayFromTxt(txt)`, `ObtenerTodosValores()`, `ObtenerGruposDeValores`.

**Peculiaridades a preservar.**
1. **Bug de deduplicado en `ObtenerListaFromTxt`**: `"2,1"` ⇒ `[1]` (el `1` se descarta porque `lista` ya contiene el valor `1` = índice del partido 2); `"2-4,1-2"` ⇒ `[1,2,3]` (faltan los índices 0); `"1-3,3"` ⇒ `[0,1,2,2]` (duplicado). Replicar tal cual: la variante `…Aciertos` es la correcta.
2. Cualquier excepción en las dos variantes `ObtenerListaFromTxt*` se traga y devuelve `[]` (no `throws`), mientras que `ObtenerBoolArrayFromTxt(txt, longitud)` lanza si el resultado queda vacío.
3. Rango con más de un `-` (`"1-3-5"`): usa solo las dos primeras partes (1 y 3).
4. `for (int j = min; j <= max; j++)` con `max = Int32.MaxValue` no termina en C# (`j` da la vuelta y la lista crece hasta agotar memoria). En Swift (`Int` de 64 bits) el rango terminaría pero con un coste absurdo: rechazar con error los rangos de más de 10^6 elementos.
5. `ObtenerRangos` muta la lista del llamador.
6. `ObtenerTextoFiguraFromLong`: el `Convert.ToString(long)` es invariante (sin cultura).

**Trampas Swift.** `Convert.ToInt32` (recorte, signo, `Int32`), `Int32` ↔ `UInt32(bitPattern:)` en `(uint)Convert.ToInt32(...)`, `split(omittingEmptySubsequences: false)`, `ToUpper` simple (usar `String(c).uppercased()` solo si el resultado tiene un único `Character`; si no, dejar `c`: así `ß` queda `ß` como en .NET), cultura invariante en `ToString()`.

**Casos golden propuestos.** `ObtenerListaFromTxt`: `"1-3"` (→ `[0,1,2]`), `"2,1"` (→ `[1]`), `"2-4,1-2"` (→ `[1,2,3]`), `"1-3,3"` (→ `[0,1,2,2]`), `"1,2"`, `"3-1"`, `""`, `"a"`, `"1-3-5"`; `…Aciertos`: `"0,2-4,3"` (→ `[0,2,3,4]`); `ObtenerLongFiguraFromText("1-2-3")` = 291, `("16-1")` = 257; `ObtenerTextoFiguraFromLong(291)` = `"1-2-3"`, `(257)` = `"1-0-1"`, `(1)` = `"1"`, `(0)` = `"0"`; `ObtenerBoolArrayFromTxt("1,3-4", 2)` = `[F,T,F,T,T]`, `("1,3-4", 14)` (longitud 15), `("x", 14)` (EXC), `("")` (nil), `("0,2")` con N=14; `ObtenerTodosValores(3,5)` = `"3,4,5"`, `(5,3)` (EXC); `ObtenerValoresSeparadosPorComas("1,3-5,9")` = `"1,3,4,5,9"`; `SonTodosNumeros("1,,2")` = false; `ObtenerRangosSeparadosPorGuion("3,4,5")` = `"3-5"`, `("7")` = `""`; `ObtenerGruposDeValores(3, false)` N=14 ⇒ `"1-3","2-4",…,"12-14"`, `(3, true)` ⇒ `"1-3","4-6","7-9","10-12"`, `(2, true)` ⇒ 7 grupos, `(1, false)` ⇒ `[]`; `PonerEnMayuscula`: `"real madrid"` (→ `"Real Madrid "`), `"r. madrid"`, `"a"`, `""`, `"ab"`, `"ñandú"`.

**Evidencia.** `U/UtilidadesEntradasValores.cs:26-456`; consumidores: `MC/FiltroPesosNumericos.cs:315`, `MC/FiltroSignosSeguidos.cs:105-129`, `EntradaSalida/FContactosData.cs:131`, `FPesosNumericosData.cs:148`, `FDiferenciasData.cs:87`, `MC/Analizador.cs` (`ObtenerArrayAciertos`), `DiferenciasFrmViewModel.cs:324`.

---

### 2.7 Porcentajes — `Free1X2.Domain/Utils/Porcentajes.cs`

**Propósito.** Matriz `N×3` de valoraciones/porcentajes (1,X,2 por partido): normalización numérica (`ValoresBase100`, `ValoresNeperianos`, usados por banco de pruebas, estimador y tramificar), lectura/escritura de los ficheros de valoraciones y puente con el portapapeles.

**Tipos y datos.** `valores: [[Double]]` `N×3` (`new double[NumeroPartidos,3]` al construir, `:32`; el ctor `Porcentajes(double[,] p)` **guarda la referencia** `:50-53`), `JornadaEncontrada: Bool = true` (`:33`), `FormatoFichero: Int16` (`:34`), `sep: Character = ' '` (`:35`), `_DescripcionValoracion: [String]?`, `_DescripcionBuscada = ""`, `_Temporada`, `_Jornada`. Salida numérica `Float[N][3]` (T3).

**A) Numéricos** (`:329-352`; sin I/O):
```
ValoresBase100():    para cada partido i:
    factor = Float( v[i][0] + v[i][1] + v[i][2] )          // suma en Double, luego a Float (pierde precisión)
    para j: out[i][j] = Float( v[i][j] / Double(factor) )   // división en Double con el factor ya redondeado a Float
ValoresNeperianos(): out = ValoresBase100(); out[i][j] = Float( log( Double(out[i][j]) ) )     // log natural, resultado a Float
```
`log(0)` = −∞, `log(<0)` = NaN, `factor == 0` ⇒ NaN/∞ (sin protección). Itera `i < NumeroPartidos` (filas extra de `valores` se ignoran; filas de menos ⇒ EXC).

**B) Lectura** (`Leer` `:81-100`). Helpers:
- `LeerLinia` `:312-322`: sin datos ⇒ `""`; si no `ReadLine().Trim()`. Los dos `.Replace` de `:318-319` descartan su resultado (**no-ops**): las tabulaciones y los dobles espacios **no** se normalizan.
- `num(s)` = `Convert.ToDouble(s)` con la cultura actual; las lecturas hacen antes `s.Replace(".", ",")` (y `.Trim()` salvo el formato 1) ⇒ asumen cultura de **coma decimal** (es-ES). Spec Swift: tras reemplazar `.`→`,` aceptar `^\s*[+-]?(\d+(,\d*)?|,\d+)([eE][+-]?\d+)?\s*$` (decimal = `,`); cualquier otra cosa ⇒ fallo (`FormatException`).
- `EsNumero(s)` `:324-327` = `Convert.ToDouble(s)` **sin** `Replace`: en es-ES el `.` es separador de millares ⇒ `"1.5"` ⇒ 15 (válido). Spec Swift: eliminar los `.` entre dígitos y aplicar `num`. (Ver Abiertas A4.)

```
TestFichero(sr):                                                 // :288-310
   linea = LeerLinia(sr)
   for s in [',', ' ', '\t']: sep = s; f = TestSeparador(linea); if f != 2 { return f }
   return EsNumero(linea) ? 1 : 2                                // `sep` queda en '\t'
TestSeparador(linea):                                            // :248-286
   aux = linea.split(sep)
   if aux.count == 3 && los 3 EsNumero → 3
   if aux.count ∈ {42,45}: 42 si EsNumero(aux[0..<42]) todos, si no 2
   if aux.count ∈ {43,46}: 43 si EsNumero(aux[1..<43]) todos, si no 2
   if aux.count ∈ {44,47}: 44 si EsNumero(aux[2..<44]) todos, si no 2
   else 2
Leer():   formato = TestFichero(archivo); reabrir; switch formato:
   1 → LeeUnValorPorFila;  3 → LeeTresValoresPorFila;  42|45 → Lee42;  43|46 → JornadaEncontrada = Lee43;  44|47 → JornadaEncontrada = Lee44;
   otro → ShowError("Fichero Incorrecto")                         // TestFichero solo devuelve 1,2,3,42,43,44
```
Lectores (cada uno cierra el lector al final):
- **Formato 1** `:102-119`: `for i in 0..<N: v[i][0]=num(ReadLine().Replace(".",",")); v[i][1]=…; v[i][2]=…` — 3 líneas por partido, **sin `Trim`**, `ReadLine()` nulo ⇒ NRE; todo en `try` ⇒ `ShowError("El fichero seleccionado no es de valoraciones o está corrupto")` y se conservan los valores ya asignados.
- **Formato 3** `:121-147`: `i = 0; mientras haya datos: partes = LeerLinia().split(sep); si partes.count == 3 { try { v[i][k] = num(partes[k].Replace(".",",").Trim()) } catch { si i != N { ShowError("…corrupto"); break } } }; i += 1`. **`i` avanza con cada línea** (también vacías o de otra longitud). La excepción por `i ≥ N` se traga en silencio solo si `i == N` (una línea sobrante); a partir de la segunda sobrante ⇒ error + corte.
- **Formato 42/45** `:149-175`: líneas con `count ∈ {42,45}`: `try { for i in stride(0,42,3) { v[indice][k] = num(partes[i+k]…); indice += 1 } } catch { ShowError; break }`. **Solo lee 14 partidos aunque haya 45 campos** (los 3 últimos se ignoran). `indice` no se reinicia entre líneas: la 2.ª línea válida desborda (N=14) ⇒ error + corte. Líneas con otro nº de campos se saltan sin aviso.
- **Formato 43/46** `:177-211`: líneas con `count ∈ {43,46}`: `_DescripcionValoracion.append(partes[0])` (siempre); si `partes[0] == _DescripcionBuscada` (ordinal) ⇒ lee `i in stride(1,43,3)` y `encontrado = true`; errores ⇒ `ShowError` + corte. Devuelve `encontrado`.
- **Formato 44/47** `:213-246`: `count ∈ {44,47}`; si `partes[0] == _Temporada && partes[1] == _Jornada` ⇒ `i in stride(2,44,3)`; devuelve `encontrado`.

**C) Escritura** (`Guardar` `:354-399`; UTF-8 **sin BOM**, `WriteLine` ⇒ `Environment.NewLine`):
- formato 3: una línea por partido: los 3 valores `ToString().Replace(",", ".")` unidos por `sep` (sin separador final).
- formato 1: un valor por línea, `ToString()` **sin** Replace (con es-ES queda coma).
- formato 42: **una** línea con cada valor `ToString().Replace(",", ".")` seguido de `sep` (separador final incluido).
- otro formato: fichero vacío.
- `GuardarValoraciones(nombre, formato, sep, valores1[], valoresX[], valores2[])` `:401-413` copia las tres columnas en `valores[i][0..2]`; la de `double[,]` `:415-420` toma la referencia; ambas fijan `sep`, `FormatoFichero` y llaman a `Guardar`.
- **Histórico** `GuardarValoraciones(nombre, sep, valores, temp, jor)` `:422-480`: `FormatoFichero = existe ? TestFichero : 44`; si ≠ 44 ⇒ `ShowError("El fichero no es un fichero de Valoraciones históricas")` y fin. Si 44: lee **todas** las líneas con `LeeColumnaSinComas` (`Trim().ToUpper()`, `ACT:214-217`), `split(sep)` (aquí `sep` es el **detectado** por `TestFichero` si el fichero existía, el del parámetro si era nuevo); la línea cuyo `[0]==temp && [1]==jor` se sustituye por `MontaLinea`; si no existía la jornada se añade al final; reescribe el fichero entero con `GuardarColsComa` (sin quitar comas).
- `MontaLinea` `:482-499`: `temp + sep + jor` y, por cada partido y signo, `sep + v.ToString().Replace(".", ",")` (**decimales con coma**, al contrario que `Guardar`), con el campo `sep` vigente (el **detectado** si el fichero existía, el del parámetro si es nuevo: `:459,478`).

**D) Portapapeles.** `FormatearParaPortapapeles()` `:505-523`: `v[0][0] + TAB + v[0][1] + TAB + v[0][2]` y para cada partido siguiente `CR LF` + lo mismo; sin salto final; números con `ToString()` de la cultura actual (coma en es-ES). `CargarDesdeCadena(cadena)` `:525-537`: `""`/`null` ⇒ `false`; escribe la cadena en `Path.GetTempFileName()` (UTF-8 sin BOM, no se borra) y llama a `Leer()`; Swift: ejecutar la misma máquina de estados sobre las líneas de la cadena, sin fichero temporal. `PonerPorcentajesEnElPortapapeles/RecuperarPorcentajesDelPortapapeles` solo delegan en `Clipboard` (`:539-547`).

**Estado global.** `NumeroPartidos` (dimensión y todos los bucles), `UserDialogs.ShowError` (cuatro textos: `"Fichero Incorrecto"`, `"El fichero seleccionado no es de valoraciones o está corrupto"`, `"El fichero no es un fichero de Valoraciones históricas"`), `Clipboard`. Swift: `LecturaPorcentajes { valores, formato, jornadaEncontrada, descripciones, errores: [String] }` en vez de diálogos.

**Peculiaridades a preservar.** (1) `Float`/`Double` exactos de A). (2) `LeerLinia` no normaliza. (3) `i` consume líneas inválidas (formato 3). (4) 15.º partido ignorado en 42/43/44 (45/46/47 campos). (5) `ReadLine` de .NET separa en `\n`, `\r` y `\r\n` (T8): usar un divisor equivalente; el texto se decodifica como UTF-8 con detección de BOM. (6) Mezcla de decimales: `Guardar` escribe punto, el histórico escribe coma. (7) El histórico reescribe el fichero en mayúsculas y recortado.

**Trampas Swift.** T3 (`Float`/`Double`), T7 (`ToString()` y `Convert.ToDouble` dependen de la cultura: fijar es-ES; ver formateo de dobles en §2.8), T8 (BOM/fin de línea), `Foundation.log` vs `Math.Log` (diferencias de 1 ulp en `Double` casi siempre desaparecen al pasar a `Float`; golden con 10⁴ valores aleatorios), cultura al generar golden (Abiertas A3).

**Casos golden propuestos.** `ValoresBase100` de `[[2,3,5],[1,1,1],[0.5,0.25,0.25]…]` (N=14), con una fila `[0,0,0]` (NaN), `ValoresNeperianos` con un cero (−∞); un fichero por formato: 1 (42 líneas), 3 (14 líneas `"a b c"`, separador `' '`, `','` y `\t`), 42 (1 línea de 42 y otra de 45 números), 43 (con 3 descripciones, buscar la 2.ª y una inexistente), 44 (2 jornadas; buscar existente e inexistente), fichero inválido, línea sobrante (15 líneas en formato 3), línea vacía intercalada, decimales con `.` y con `,`; guardado en cada formato + histórico (nuevo, existente con jornada repetida, existente sin la jornada, formato incorrecto, fichero vacío existente); portapapeles ida y vuelta.

**Evidencia.** `U/Porcentajes.cs:32-547`; `ACT:214-217`; consumidores `BancoPruebasFrmViewModel.cs:327,765,836,920`, `EstimadorPremiosFrmViewModel.cs:154`, `TramificarFormViewModel.cs:409`; el resto de E/S solo en WinForms (`HistoriaValoracionesFrm.cs:341`, `GuardarValoracionFrm.cs:307`, `ControlPorcentajes.cs:397,472`) y réplicas WinUI (`GuardarValoracionFrmViewModel.cs:137`, `HistoriaValoracionesFrmViewModel.cs:192`).

---

### 2.8 LAE — `Free1X2.Domain/Utils/LAE.cs`

**Propósito.** Calcula los importes de premio por categoría (14…10) a partir de la recaudación y de los acertantes de una jornada y los **graba** en `Jornadas/InfoJornadasLAE.txt` (historial de Loterías y Apuestas del Estado).

**Tipos y datos.** `_Temporada`, `_Jornada = numJornada.PadLeft(2,'0')` (`:43`), `_ColumnaPremiada`, `_Acertantes: [Int](5)`, `_Recaudacion`, `_PrecioApuesta: Double`, `_PorcentajePremioCategoria: [Double](5)`, `_Premios: [Double](5)`, `moneda` (leída y **no usada**).

**Constructor** (`:41-57`): `AConfiguracion(BaseDirectory).ObtenValoresLAE(&precio, &pct[0], &recaud, &moneda)`; luego `pct[1…4] = 10`; `if recaudacion > 0 { _Recaudacion = recaudacion }`.
**Importante (T3):** `ObtenValoresLAE` parsea los tres números con `float.Parse(texto, CultureInfo("es-ES"))` y los asigna a `double` (`EntradaSalida/AConfiguracion.cs:407-410`) ⇒ en Swift `Double(Float(parseEs(texto)))`: `0,6` queda `0.6000000238418579`, no `0.6`. El parámetro `recaudacion` del ctor (double) **no** pasa por `Float`.

**Algoritmo.**
```
CalculaPremios():                                                  // :141-148
  for i in 0..<5:
     premios[i] = _Recaudacion / _PrecioApuesta * pct[i] / 100 / Double(_Acertantes[i])      // izquierda→derecha
     premios[i] = round2( premios[i] + 0.005 )
round2(x) = (|x| < 1e16) ? ( (x * 100).rounded(.toNearestOrEven) / 100 ) : x                  // Math.Round(x, 2), ToEven (T4)
MontaLinea():                                                      // :120-139
  columnaPremiada \t temporada \t jornada \t fmt(_Recaudacion/_PrecioApuesta) \t fmt(premios[0]) … \t fmt(premios[4])
  fmt(d) = d.ToString().Replace(".", ",")                          // cadena .NET "G" más corta de ida y vuelta, cultura actual; el Replace fuerza coma
GrabarJornada(acertantes):                                         // :77-118
  _Acertantes = acertantes; CalculaPremios()
  fichero = BaseDirectory + "/Jornadas/InfoJornadasLAE.txt"
  lineas = []; existe = false
  para cada línea L del fichero (ACT.LeeColumnaSinComas ⇒ Trim().ToUpper()):
       v = L.split("\t")
       si v[1] == _Temporada && v[2] == _Jornada { existe = true; L = MontaLinea() }
       lineas.append(L)
  si !existe { lineas.append(MontaLinea()) }
  reescribir el fichero completo con lineas (UTF-8 sin BOM, una por WriteLine)
```
**Estado global.** Lee `parametros.free1x2` (sección LAE) y `AppContext.BaseDirectory`; escribe el fichero de jornadas. Swift: inyectar `ConfigLAE { precioApuesta, porcentaje14, recaudacion }` y la ruta del fichero.

**Peculiaridades a preservar.**
1. `Math.Round(x + 0.005, 2)` **no es «redondear hacia arriba»**: es `(x+0.005)·100` redondeado a par; el resultado depende del error binario de la suma (p. ej. `300000.005·100`).
2. `Acertantes[i] == 0` ⇒ división real por cero ⇒ `+∞`; `Round(∞)` = ∞; `ToString()` produce el símbolo de infinito de la cultura (Abiertas A2).
3. El valor de la 4.ª columna es `Recaudación/PrecioApuesta` (**nº de apuestas**, no euros) pese al nombre local `Recaudacion`.
4. El fichero se relee y reescribe **en mayúsculas y recortado** (`LeeColumnaSinComas`); la comparación `v[1] == _Temporada` es ordinal contra ese texto en mayúsculas ⇒ si temporada/jornada contienen minúsculas nunca se detecta la jornada existente.
5. El fichero debe existir (`SiguienteColumna` crea el `StreamReader` sin comprobarlo ⇒ `FileNotFoundException`/`DirectoryNotFoundException`); el repo no incluye `Jornadas/InfoJornadasLAE.txt` (se distribuye con la app).
6. Línea sin tabuladores ⇒ `v[1]` EXC (incl. línea vacía en medio del fichero).
7. Pct de categorías 13…10 fijo en 10 %; el de la 14 viene de la configuración (`PorcentajeDestinadoAl14`, en tanto por ciento).

**Trampas Swift.** T3 (`Double(Float(...))`), T4 (`.rounded(.toNearestOrEven)`; no usar `Foundation.round`, que es *away from zero*), T7 (formato `double.ToString()`: implementar `formatoNet(Double)` con los dígitos más cortos de ida y vuelta —`Double.description` de Swift los da— y la notación de .NET: **fija** si el exponente decimal está en `[−5, 15)`, **científica** `d.dddE+XX` fuera de ese rango; sin sufijo `.0`), T8 (fin de línea y UTF-8 sin BOM), escritura atómica.

**Casos golden propuestos** (esperado = modelo; el oráculo manda): `recaudacion=1000000, precio=0.5, pct=[15,10,10,10,10], acertantes=[1,10,100,1000,10000]` ⇒ premios `[300000, 20000, 2000, 200, 20]`; `2500000, 0.5, [12,10,10,10,10], [3,57,1500,14000,120000]` ⇒ `[200000, 8771.93, 333.34, 35.72, 4.17]`; `acertantes[2]=0` (∞); `precio=0.6` leído de config (float widening); fichero con la jornada ya existente, sin ella, con minúsculas, con línea vacía, inexistente; `jornada="7"` ⇒ `"07"`.

**Evidencia.** `U/LAE.cs:41-148`; `EntradaSalida/AConfiguracion.cs:402-413`; `ACT:214-217,70-80,92-99`; `EstimadorPremiosFrmViewModel.cs:394-409`.

---

### 2.9 SumadorCombinaciones — `Free1X2.Domain/Utils/SumadorCombinaciones.cs`

**Propósito.** Álgebra de ficheros de columnas (pantalla «Álgebra»): eliminar repetidas, unión, intersección, resta.

**Tipos y datos.** `archivoCols1/2/Final: String`, `noColumnasFinal`, `noColsRepetidas: Int`, `mensajeFinCalculo: String`, `noPartidos` (ctor `:45-48`), `Bits`: `BitArray(14348907)` = 3^15 por instancia (`:41`). Enum `AlgebraCombTipo { EliminaRepetidas, SumaEliminaRepetidas, SumaSoloComunes, RestaSegunda }` (`Enums.cs:45-51`). Swift: `Bits` como `[UInt64]` de 224 202 palabras; índices fuera ⇒ lanzar.

**Convenciones comunes** (todas las operaciones): `Bits.SetAll(false)`; `conv = ConvertidorDeBases(noPartidos)`; cada línea se lee con `LeeColumnaSinComas` = `Trim().ToUpper()` (**a pesar del nombre NO quita las comas**, `ACT:214-217`); `num = conv.ConvColumnaANumero(linea)` (usa `linea.Length`, no `noPartidos`); la línea en blanco ⇒ `""` ⇒ `num = 0` (equivale a la columna «todo `1`»); se escribe con `GuardarCols(string)`, que **elimina las comas** y escribe `WriteLine` (`ACT:70-80`); el escritor se crea en la **primera** escritura ⇒ si no sale ninguna columna el fichero de salida **no se crea ni se trunca** (`ACT:70-75`).

**Algoritmo** (`Calcula(tipo)` `:50-71` despacha y después `CrearMensajeFinCalculo`):
```
EliminaRepetidas  :244-281   for L in f1: n = num(L); si !Bits[n] { Bits[n] = true; escribir(L); final += 1 } sino { rep += 1 }
SumaEliminaRepetidas :123-189   igual sobre f1 y después sobre f2 con los mismos Bits (orden: f1 luego f2; se escribe el texto original de cada línea)
SumaSoloComunes   :191-242   for L in f1: Bits[num(L)] = true (sin escribir) ; for L in f2: si Bits[num(L)] { escribir(L); final += 1; rep += 1 }   // NO desmarca ⇒ las repetidas de f2 que estén en f1 se escriben cada vez
RestaSegunda      :73-121    for L in f1: n = num(L); si !Bits[n] { Bits[n]=true; final += 1 } sino { rep += 1 }
                             for L in f2: n = num(L); si Bits[n] { Bits[n]=false; final −= 1; rep += 1 }
                             for nr in 0..<Bits.count: si Bits[nr] { escribir( conv.ConvNumAColumna(nr) ) }       // ascendente por número base-3, texto NORMALIZADO (relleno '1' hasta noPartidos)
Mensajes (:283-302; "\n" literal):
   EliminaRepetidas, SumaEliminaRepetidas: "Nº columnas final: {final}\nNº col. repetidas: {rep}"
   SumaSoloComunes, RestaSegunda:          "Nº columnas final: {final}"
```
**Estado global.** Ninguno (`noPartidos` por ctor; los tests pasan `VariablesGlobales.NumeroPartidos`).

**Peculiaridades a preservar.** (1) En `SumaSoloComunes`, `final == rep` y las repeticiones de f2 se escriben duplicadas. (2) `RestaSegunda` pierde el orden y el formato original de las líneas (salida canónica) y su `rep` cuenta repetidas de f1 **más** columnas eliminadas por f2. (3) Línea en blanco = columna 0 (se escribiría como `""` en las otras tres operaciones). (4) Una línea con comas o de > 17 caracteres no-`1` lanza EXC en `ConvColumnaANumero`; una de > 15 signos con carácter ≠ `1` en posición ≥ 15 desborda `Bits` (EXC `ArgumentOutOfRange`). (5) Se leen los ficheros por orden: f1 completo y después f2. (6) `Bits` se reserva en el constructor (1,8 MB).

**Trampas Swift.** Lectura línea a línea con `ReadLine` de .NET (separadores `\n`, `\r`, `\r\n`; `Peek() >= 0` no devuelve línea vacía final si el fichero acaba en salto), `Trim()` .NET (todo espacio Unicode, incluido ` `), `ToUpper()` cultural, escritura UTF-8 sin BOM con `\n` o `\r\n` (T8: decidir y documentar; los lectores de ambos mundos aceptan los dos), no cargar f1 entero en memoria si es enorme (3^15 líneas) — procesar en streaming.

**Casos golden propuestos.** Los 5 de `T/SumadorCombinacionesTests.cs` (fixtures `cols_a/cols_b/cols_con_repetidas`: unión 15, intersección 5, resta 5, dedup 10→6, relaciones de conjuntos) comparando **ficheros byte a byte** (no solo recuentos) y los mensajes; añadir: f2 con una columna de f1 repetida 2 veces (SoloComunes ⇒ 2 líneas), línea en blanco al final, minúsculas, espacios alrededor, `noPartidos=15`, resultado vacío con fichero de salida preexistente (no se toca).

**Evidencia.** `U/SumadorCombinaciones.cs:25-333`; `ACT:70-80,183-217`; `AlgebraColumnasFrmViewModel.cs:180`; `T/SumadorCombinacionesTests.cs`.

---

### 2.10 Tramo — `Free1X2.Domain/Utils/Tramo.cs`

**Propósito.** Acumulador de un tramo de columnas del escrutinio de «Tramificar»: aciertos por categoría 14…10, columnas premiadas, importes y balance.

**Tipos y datos.** `_NumTramo`, `_ValorIzquierda`, `_ValorDerecha`, `_NumColumnasTramo`, `_ColumnasPremiadas: Int`; `_ProbAcumulada`, `_PremiosAcumulados`, `_Balance: Double`; `_Aciertos: [Int](5)` (índice 0=14 … 4=10), `premios: [Double](5)`. Swift: `final class Tramo` (los `Tramo` se guardan en listas y se mutan con `AddTramo`).

**Algoritmo.**
```
init(premios): self.premios = premios                       // por referencia en C# (alias)
PonerAciertos(a): _Aciertos = a                             // por referencia: AddTramo muta el array del llamador
P14…P10 = _Aciertos[0…4]
CalculaTotalImportePremios():                               // :112-118
   acc = 0; for i in 0..<5 { acc += Double(_Aciertos[i]) * premios[i] }
   _PremiosAcumulados = acc
   _Balance = acc − Double(_NumColumnasTramo) * 0.5        // coste FIJO 0,5 por columna (no usa PrecioApuesta)
AddTramo(tr):                                               // :51-59
   _Aciertos[k] += tr.P(k) para k = 0…4;  _ColumnasPremiadas += tr.ColumnasPremiadas
   _PremiosAcumulados += tr.TotalImportePremios;  _Balance += tr.Balance
   // NO suma NumColumnasTramo ni recalcula nada
```
**Estado global.** Ninguno.

**Peculiaridades.** (1) El `0.5` es constante (coste de apuesta de Loterías). (2) `AddTramo` suma importes ya calculados; el orden de uso en WinUI es `PonerAciertos → CalculaTotalImportePremios → AddTramo` (`TramificarFormViewModel.cs:673-675`). (3) Alias de `premios` y `_Aciertos`: en Swift copiar al construir/poner es equivalente mientras el llamador no los modifique después (el ViewModel no lo hace, `TramificarFormViewModel.cs:582-617`). (4) `P14…P10`, `TotalImportePremios`, `Balance` son solo lectura.

**Trampas Swift.** `Double(Int)*Double` en el mismo orden `i = 0…4` (la suma de punto flotante no es asociativa); `_NumColumnasTramo * 0.5` en `Double`.

**Casos golden.** Los 6 de `T/TramoTests.cs` (premios `{1000,100,10,5,1}`: `[1,2,3,4,5]` ⇒ 1255; balance con 100 columnas 950; `[0,…]` con 10 columnas ⇒ −5; `AddTramo` ⇒ aciertos `[11,22,33,44,55]`, importes 1255).

**Evidencia.** `U/Tramo.cs:25-121`; `T/TramoTests.cs`.

---

### 2.11 TablaDeSeleccion — `Free1X2.Domain/Utils/TablaDeSeleccion.cs`

**Propósito.** Fila de una tabla de mínimos/máximos (diálogo de selección del banco de pruebas): `Concepto` (solo lectura), `Minimo`, `Maximo`, `Checked`.
**Tipos y datos.** `_Concepto: String`, `_Min`, `_Max: Double`, `_chec: Bool` (`:25-28`). Constructor `(concepto, min, max)` asigna los campos **sin** marcar `Checked` (queda `false`, `:31-36`).
**Algoritmo.** Los setters de `Minimo` y de `Maximo` ponen **`Checked = true`** además de guardar el valor (`:47-56`); `Checked` tiene setter propio. Swift: `struct` con `didSet`, o `final class` si la UI la enlaza por referencia.
**Estado global.** Ninguno. **Peculiaridad.** Asignar `Minimo`/`Maximo` desde la UI siempre marca la fila.
**Golden.** `init("x",1,5)` ⇒ `Checked=false`; tras `Minimo=2` ⇒ `true`. **Evidencia.** `U/TablaDeSeleccion.cs:21-57`, `Free1X2/UI/DialogoSeleccionBancoPruebasFrm.cs:52-57`.

### 2.12 Combinacion — `Free1X2.Domain/Utils/Combinacion.cs`

DTO inmutable en la práctica: `Temporada`, `Jornada`, `Path` (`String`), ctor `(temporada, jornada, path)` (`:30-35`). Sin lógica ni estado global. Swift: `struct Combinacion { var temporada, jornada, path: String }`. Uso: lista de combinaciones del análisis múltiple de tramos (`DialogoAnalisisMultipleDeTramosFrmViewModel.cs:312,393`). Golden: ninguno (round-trip de propiedades).

### 2.13 Pral — `Free1X2.Domain/Utils/Pral.cs`

**Propósito.** `Normaliza(columna)`: deja solo los signos `1`, `2`, `X` de una línea.
**Algoritmo** (`:26-37`): `c = columna.ToUpper(); out = ""; for ch in c: if "12X".contains(ch) { out += ch }`. Swift: recorrer, convertir solo `x→X` y conservar `1`,`2`,`X`; equivalente a `ToUpper()` de cualquier cultura porque ningún otro carácter se transforma en `1/2/X`. Quita comas, espacios, `*`, etc.; `null` ⇒ NRE (en Swift `String` no opcional).
**Estado global.** Ninguno. **Golden.** `"1,x,2 *"` ⇒ `"1X2"`, `"abc"` ⇒ `""`, `" 1X2\t"` ⇒ `"1X2"`. **Evidencia.** `U/Pral.cs:26-37`; `CombinarFiltrosViewModel.cs:291`.

---

### 2.14 CompresorZip — `Free1X2.Domain/Utils/CompresorZip.cs`

**Propósito.** Guardar/leer un conjunto de columnas (bitmap de 3^N bits) como fichero `.z3q`: un ZIP con una única entrada Deflate. Basado en el compresor `*.z2q` de Charlie (`:2-4`). Dependencia: `SharpZipLib 1.4.2` (`Free1X2.Domain.csproj:14`).

**Tipos y datos.** `Comprimir(BitArray bs, string fichZip, string fichBin, int tamano, int nivel) → long` (`:41-85`); `Descomprimir(string fichComprimido) → BitArray` (`:88-136`). Swift: `Bitmap` = `[UInt64]` + `count`; `comprimir(_ bits: Bitmap, a url: URL, nombreEntrada: String, tamano: Int, nivel: Int) throws -> Int`; `descomprimir(_ url: URL) throws -> Bitmap`. Constantes: `BUFFER = 2048` (`:39`), 14 348 907 = 3^15 (`:90,124,126`), 4 782 969 = 3^14 (`:123`).

**Formato `.z3q` (lo observable en el código):**
1. **Contenedor ZIP** (PKWARE) con **una sola entrada** creada con `ZipOutputStream` (`:43,67-77`).
2. **Método** `Deflated` (`:69`); **nivel** `SetLevel(nivel)` (`:75`) con `nivel` 0–9 (la UI usa 5 por defecto, `CompresorViewModel.cs:46`).
3. **Nombre de la entrada** = parámetro `fichBin`. WinUI pasa la **ruta absoluta del propio `.z3q`** (`CompresorViewModel.cs:137-138`), WinForms el nombre elegido (`Free1X2/UI/Compresor.cs:101`); el lector **ignora el nombre** (`GetNextEntry`, `:96`). El comentario de `:67` pide «.bin y sin path». **Swift:** escribir un nombre corto (`"columnas.bin"`) y aceptar cualquiera.
4. **Metadatos fijados antes de `PutNextEntry`:** `entry.Size = matrizBits.Length`, `entry.Crc = crc.Value` (`:71,73`) ⇒ cabecera con tamaños y CRC reales.
5. **Carga útil descomprimida:** `matrizBits` de **`⌊tamano/8⌋ + 1` bytes** (`:45`; siempre un byte de más); el bit del **número de columna `i`** (índice base-3 de `ConvertidorDeBases`, §1.7) es el bit `i % 8` (LSB primero, máscara `0x01 << (i%8)`) del byte `i / 8` (`:46-62`); los bits `≥ bs.Length` no existen (el indexador lanza y el `catch {}` vacío `:59-61` los deja a 0).
6. **CRC-32** del ZIP (polinomio reflejado `0xEDB88320`) sobre `matrizBits` (`:64-65`).
7. **Valor devuelto** por `Comprimir` = `entry.Size` = longitud **sin comprimir** (`:80`), no el tamaño en disco (`T/CompresorZipTests.cs`: `(Length/8)+1`).

**Lectura** (`Descomprimir` `:88-136`): `ZipInputStream`, **primera** entrada (`GetNextEntry`; zip sin entradas ⇒ NRE; si es directorio no se lee nada); lee en bloques de 2048; para cada byte `car` y `p in 0..<8`, si `car & (1<<p)` ⇒ `bs.Set(count + i*8 + p)` sobre un `BitArray(14348907)` (EXC si un bit a 1 cae ≥ 3^15); `count += numBytes*8` por bloque. Tamaño del resultado: `tamanoReal = (count > 14348907) ? 14348907 : 4782969`; devuelve los primeros `tamanoReal` bits. Consecuencia: archivo de 14 partidos (597 872 bytes ⇒ 4 782 976 bits) ⇒ 3^14; de 15 (1 793 614 bytes ⇒ 14 348 912 bits) ⇒ 3^15; **cualquier otro tamaño ⇒ 3^14** (p. ej. un bitmap 3^13 o 3^16 se lee como 3^14).

**Qué debe cumplir el Swift** (compatibilidad **funcional** en ambos sentidos; igualdad byte a byte imposible porque la fecha DOS de la entrada es la hora actual):
- *Escritor:* ZIP estándar de 1 entrada, método 8 (o 0 si el nivel es 0 y se prefiere), tamaños y CRC reales en la cabecera local (sin bit 3), directorio central y registro de fin estándar, sin comentario. El `ZipInputStream` de SharpZipLib lee ese resultado.
- *Lector:* localizar `PK\x05\x06` desde el final, usar el directorio central (no los tamaños de la cabecera local), tomar la primera entrada que no sea directorio, método 8 ⇒ inflate crudo, método 0 ⇒ copia; tolerar el bit 3 (descriptor de datos); verificar el CRC.
- Reproducir el cálculo de `tamanoReal` y la longitud `⌊tamano/8⌋+1`.

**Alternativa Swift (recomendada).** (a) Contenedor ZIP mínimo propio (~120 líneas: cabecera local, directorio central, EOCD) + **DEFLATE crudo** con `libz` del sistema (`deflateInit2(level, Z_DEFLATED, −15, 8, Z_DEFAULT_STRATEGY)` / `inflateInit2(−15)` y `crc32`; target `systemLibrary` en SPM, `libz` viene con macOS) ⇒ respeta `nivel` 0–9. (b) Sin enlazar `libz`: `Compression.framework` con `COMPRESSION_ZLIB` (DEFLATE crudo, **nivel fijo**) ⇒ formato idéntico, el parámetro `nivel` deja de influir en el tamaño. (c) `ZIPFoundation` (MIT): API de archivo ZIP, sin control de nivel. No portar `CreateZip/ExtractZip` (`FastZip`, `:139-147`): 0 consumidores.

**Estado global.** Ninguno. **Peculiaridades a preservar.** Longitud `⌊tamano/8⌋+1`; valor devuelto = bytes sin comprimir; regla de `tamanoReal`; lector que ignora el nombre; `Comprimir` no cierra el flujo si falla (irrelevante en Swift).

**Trampas Swift.** Bitset como `[UInt64]` (14,3 M bits = 1,8 MB); orden de bits LSB-primero dentro del byte (no mezclar con el orden de `UInt64` little-endian); cabeceras ZIP en little-endian explícito; fecha DOS (año−1980, 2 s): usar la fecha actual o fija (propuesta: fecha fija 1980-01-01 para reproducibilidad, el lector C# no la usa).

**Casos golden propuestos.** (1) Patrón de `CompresorZipTests`: bits `{0,1,7,8,9,1234,100000,2391484,4782968}` sobre 3^14, niveles 0, 1, 5, 9: C# comprime → Swift descomprime y compara; Swift comprime → C# descomprime. (2) Bitmap vacío y bitmap lleno (3^14); (3) bitmap 3^15 con el bit 14 348 906; (4) columnas reales de `cols_a.txt` (10) y de un fichero de ≥ 10⁵ columnas; (5) fichero con nombre de entrada = ruta absoluta de Windows con barras invertidas; (6) zip sin entradas (error).

**Evidencia.** `U/CompresorZip.cs:25-147`; `Free1X2.Domain.csproj:14`; `T/CompresorZipTests.cs`; `CompresorViewModel.cs:46,82,89,137-138,174,208`.

---

