# ESPEC 03 — Filtros B: Distancias, No variantes, Formatos 1-2-3, Valoración, Simetrías, Dibujos, Formatos de signos, Diferencias, Grupos de equipos y Estadísticas

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Sigue la plantilla y las trampas
> T1-T12 de [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Describe **qué hace hoy el C#** (oráculo);
> las rutas son relativas a la raíz del repo y las líneas son las del código leído. Todo lo no verificado está en
> la sección final **«Abiertas»**.
>
> **Alcance (21 clases + `RangosOpciones` auxiliar, en las secciones 1-20):** `FiltroDistancias`, `FiltroNoVariantes`, `FiltroFormatos123` + `Formato123`,
> `FiltroValoracionSignos` (+ `RangosOpciones`, necesario para entenderlo), `FiltroSimetrias` + `Simetria`, `FiltroDibujos`,
> `FiltroFormatosSignos` + `FormatosSignos` + `FormatoSignos`, `FiltroDiferencias` + `Diferencia`, `FiltroGruposEquipos` +
> `GrupoEquipos`, y `MotorCalculo/Estadisticas/` (`CalculadorEstadisticas`, `Estadistica`, `Dibujos`, `DibRepes`, `StaInter`,
> `StaSigSeg`). Las rutas C# son `Free1X2.Domain/MotorCalculo/…` salvo indicación.
>
> **Fuera de alcance** (otras ESPEC): `Grupo`/tolerancias/analizador (ESPEC_01), `FiltroInterrupciones` (ESPEC_02),
> `RelacionGE1`/`ControladorRelacionesGE1` (ESPEC_04), `UtilColumnas`/`RangosHelper`/`ContenedorAnalisis` (ESPEC_06),
> formato de ficheros `.comb`/`.txt` (ESPEC_07). Aquí solo se resumen las piezas de esas ESPEC que estos filtros necesitan.

---

## 0. Piezas comunes

### 0.1 Columna, signos y columna de ejemplo

- Columna = `long` (T2). Partido `p` (1..N) ocupa los bits `3(p-1)..3(p-1)+2`; **`1`=4 (`100`), `X`=2 (`010`), `2`=1 (`001`)**;
  `0` = «asterisco» (partido fuera del grupo; `Grupo` aplica `columna & máscara`, `Grupo.cs:69-89,160-163`).
  Decodificación `"2X1"[(g & 7) >> 1]` (`Free1X2.Domain/Utils/UtilColumnas.cs:40`); texto→long con el primer carácter en los bits bajos
  (`UtilColumnas.cs:196-204`).
- **Columna de ejemplo C\*** = `1X2112X1X2211X` = `1409999751252` (0x1484A88C854). Signos por partido: 1=`1`, 2=`X`, 3=`2`, 4=`1`, 5=`1`,
  6=`2`, 7=`X`, 8=`1`, 9=`X`, 10=`2`, 11=`2`, 12=`1`, 13=`1`, 14=`X`. Recuentos: seis `1` (p 1,4,5,8,12,13), cuatro `X` (p 2,7,9,14), cuatro `2` (p 3,6,10,11).
- Tabla de columnas golden comunes (G1..G9); los valores «ref.» se calcularon con un port Python externo del C# (no versionado) y **GoldenGen debe
  regenerarlos**:

| Id | Columna | NoVar (v,x,2) | Dibujo (x+2) | Distancia (var,1,X,2) | StaSigSeg (mv,m1,mx,m2) | StaInter (ng,nv,n1,nx,n2) |
|----|---------|---------------|--------------|-----------------------|--------------------------|---------------------------|
| G1 | `11111111111111` | 0,0,0 | 0+0 | 0,1,0,0 | 0,14,0,0 | 0,0,0,0,0 |
| G2 | `XXXXXXXXXXXXXX` | 14,14,0 | 14+0 | 1,0,1,0 | 14,0,14,0 | 0,0,0,0,0 |
| G3 | `22222222222222` | 14,0,14 | 0+14 | 1,0,0,1 | 14,0,0,14 | 0,0,0,0,0 |
| G4 | `1X21X21X21X21X` | 9,5,4 | 5+4 | 2,3,3,3 | 2,1,1,1 | 13,4,5,4,4 |
| G5 = C\* | `1X2112X1X2211X` | 8,4,4 | 4+4 | 3,4,5,4 | 3,2,1,2 | 10,3,4,3,3 |
| G6 | `1111111X222222` | 7,1,6 | 1+6 | 1,1,0,1 | 7,7,1,6 | 2,0,1,1,0 |
| G7 | `X1X1X1X1X1X1X1` | 7,7,0 | 7+0 | 2,2,2,0 | 1,1,1,0 | 13,7,6,7,0 |
| G8 | `12121212121212` | 7,0,7 | 0+7 | 2,2,0,2 | 1,1,0,1 | 13,6,7,0,6 |
| G9 | `1XX2211XX2211X` | 9,5,4 | 5+4 | 3,5,5,5 | 4,2,2,2 | 7,2,3,2,2 |

  Además, para todos los filtros: (a) muestra determinista (semilla fija) de ≥20 000 columnas de las 3^14; (b) las mismas columnas con asteriscos
  aplicando máscaras de grupo `{1,2,3,5,8,9}`, `{2,4,6,8,10,12,14}` y `{1..7}`; (c) la columna `0`.

### 0.2 Contrato `IFiltro` y cómo lo invoca `Grupo`

`IFiltro` (`IFiltro.cs:25-39`): `Analizar(long)→bool`, `AnalizarFallos(long)→string[]?` (nulo = sin fallos), `IsActive`, `EsNombreFiltro(string)`,
`NombreFiltro`, `ContieneDatos`, `ObtenNoAciertosTolerancias(string letras)→int`, `UsaFiguras()`, `Figuras`, `AnalisisActivo`.
Propuesta Swift: `protocol FiltroColumna: AnyObject` (los filtros **conservan estado entre llamadas**, deben ser `final class`, T10).

Uso en `Grupo` (ESPEC_01; aquí solo lo que condiciona a estos filtros):

1. **Orden fijo de la cadena** (`Grupo.cs:94-134`): NoVariantes, ValoracionSignos, SignosSeguidos, NoInterrupciones, Dibujos, PesosNumericos,
   **Distancias**, **GruposEquipos**, Contactos, **FormatosSignos**, ColProbables, **Formatos123**, **Simetrias**, **Diferencias** (en negrita los de esta ESPEC).
2. `AnalizaColumna(long)` (`Grupo.cs:165-225`): con partidos parciales `nuevaCol = columna & máscara`; recorre la cadena evaluando **solo los `IsActive`**
   y corta en el primer `false`; si todos pasan, calcula tolerancias (`Grupo.cs:331-408`), que llaman a `ObtenNoAciertosTolerancias` de cada filtro activo.
   **Consecuencia:** cada filtro debe conservar los contadores de su última `Analizar` (la tolerancia se lee después, sobre la misma columna).
3. Variante con contenedor de análisis (`Grupo.cs:226-300`): evalúa también filtros **inactivos** con `AnalisisActivo==true` (resultado ignorado)
   y luego `ContenedorAnalisis.IncrementarContador` lee propiedades públicas post-`Analizar` (`Analisis/ContenedorAnalisis.cs:109-335`).
   **Consecuencia:** `Analizar` debe ser **total** (no fallar) con la configuración vacía de fábrica (comprobado por lectura para los 9 filtros).
4. `AnalizarFallos` solo lo usa «Analizar combinación» (`Free1X2.WinUI/Views/Ported/AnalizarCombinacionFrmViewModel.cs:253`,
   legacy `Free1X2/Analisis/AnalisisCombinacion.cs:104`). Prefijo `*` = acierto, `!` = fallo pendiente de tolerancias, otro = fallo
   (`AnalizarCombinacionFrmViewModel.cs:261-`); un elemento vacío rompe `Substring(0,1)` (`:261`) — ver Abiertas A9.

### 0.3 Resumen

| Filtro | `Filtro` (`EnumFiltros.cs:22-38`) | `AnalisisActivo` lee | Tolerancias | Estado post-`Analizar` que leen otros |
|--------|------------------------------------|----------------------|-------------|----------------------------------------|
| `FiltroNoVariantes` | NoVariantes | `AnalizarVX2` (`FiltroNoVariantes.cs:488-491`) | sí | `NoVariantes/NoEquis/NoDoses` |
| `FiltroDistancias` | Distancias | `AnalizarDistancias` (`FiltroDistancias.cs:657-660`) | sí | `NoDistanciasVariantes/Unos/Equis/Doses` |
| `FiltroFormatos123` | Formatos123 | `AnalizarFormatos123` (`FiltroFormatos123.cs:467-470`) | no (0) | — |
| `FiltroValoracionSignos` | ValoracionSignos | `AnalizarValoracion` (`FiltroValoracionSignos.cs:391-394`) | no (0) | `ValoracionResultado/Unos/Equis/Doses` |
| `FiltroSimetrias` | Simetrias | `AnalizarSimetrias` (`FiltroSimetrias.cs:202-205`) | no (0) | `AciertosSimetria` |
| `FiltroDibujos` | Dibujos | `AnalizarDibujos` (`FiltroDibujos.cs:180-183`) | no (0) | `NoEquisCol/NoDosesCol` |
| `FiltroFormatosSignos` | FormatosSignos | `AnalizarFormatos` (`FiltroFormatosSignos.cs:166-169`) | no (0) | por grupo: `NoLineasValidas/NoAparicionesGlobal` |
| `FiltroDiferencias` | Diferencias | `AnalizarSimetriasII` (`FiltroDiferencias.cs:146-149`) | **lanza** | por `Diferencia`: `NoVariantes…NoFormatos` |
| `FiltroGruposEquipos` | GruposEquipos | `AnalizarGruposEquipos` (`FiltroGruposEquipos.cs:158-161`) | no (0) | por `GrupoEquipos`: `NoSumaPuntos…` |

`aDiferencias` del fichero de configuración alimenta `AnalizarSimetriasII` (`AConfiguracion.cs:142`, orden de parámetros en `VariablesGlobales.cs:100`).
Las banderas son de solo lectura tras cargar `parametros.free1x2`; en Swift léanse de la `Sesion` (T10).

### 0.4 Piezas compartidas (referenciadas desde las secciones)

**P1 · popcount.** `ContarBitsA1(long)` = nº de bits a 1 de los 64 (`UtilColumnas.cs:299-309`) → `UInt64(bitPattern:).nonzeroBitCount`.

**P2 · signo de un partido.** `ObtenerSignoInt(L,p) = (int)((L >> ((p-1)*3)) & 7)` (`UtilColumnas.cs:66-76`). El desplazamiento de un `long` en C# se
enmascara a 6 bits (`p` fuera de 1..22 «da la vuelta»); en Swift usar `&>>` (T1). Devuelve el grupo crudo (0 si asterisco).

**P3 · compactar partidos** `IgnorarPartidos(bool[] sel, long col)` (`UtilColumnas.cs:429-444`):
```
m = 0
for i = sel.count-1 downto 1:          // el índice 0 se ignora; sel es 1-based
    if sel[i]: m = (m << 3) | signoInt(col, i)       // conserva el grupo crudo (puede ser 0)
return m                                               // el partido seleccionado de menor índice queda en los bits bajos
```
`sel` se obtiene con `ObtenerBoolArrayFromTxt(txt)` (P5b): longitud `NumeroPartidos+1`.

**P4 · `RangosHelper.ObtenBoolArray(txt)`** (`Free1X2.Domain/Utils/RangosHelper.cs:76-141`; tests `Free1X2.Domain.Tests/RangosHelperBoolTests.cs`):
`txt` = lista separada por comas de enteros o rangos `a-b`. `max` = mayor entero que aparece (en cualquier extremo); resultado `bool[max+2]`
(el último elemento siempre `false`); se marcan `true` los valores listados y los intervalos `a..b` (si `b<a` no marca nada). Cadena vacía o no numérica →
`FormatException`. **Consulta con guarda de longitud:** `arr.count > k && arr[k]` (todos los filtros lo hacen).

**P5 · listas de aciertos.**
- (a) `ObtenerListaFromTxtAciertos(txt)` (`UtilidadesEntradasValores.cs:115-181`): con comas, cada parte es `n` o `a-b`; sin comas, `n` o `a-b`;
  duplicados descartados conservando el primer orden; **cualquier excepción ⇒ lista vacía** (try/catch global).
- (b) `ObtenerBoolArrayFromTxt(txt, longitud)` (`:255-281`): `""`⇒`nil`; ordena la lista; longitud del array = `longitud+1` si `longitud > máximo`, si no `máximo+1`;
  `true` en los listados. Con lista vacía por error lanzaría al leer `lista[Count-1]`. Sobrecarga sin longitud (`:282-302`): array de `NumeroPartidos+1`
  (valores mayores se descartan en silencio). Serialización inversa `ObtenerTextoFromBool` (`:197-214`): índices `true` separados por coma, `""` si ninguno.

**P6 · parser «valores con tolerancia»** (`Set*` de NoVariantes y Distancias; p. ej. `FiltroNoVariantes.cs:231-255`):
```
for str in valores.split(","):                     // "6,7A,8B"
    if isLetter(str.last):  n = Int(str.dropLast); letra = str.last
    else:                   n = Int(str);          letra = " "
    flag[n] = true; tol[n] = letra                 // el último elemento repetido pisa la letra anterior
```
Solo enteros sueltos (no admite `a-b`, `Free1X2.Domain.Tests/ManualFuncionalTests.cs:14-15`). `""` ⇒ `str[-1]` ⇒ `IndexOutOfRange`; `n>N` o negativo ⇒ `IndexOutOfRange`; no entero ⇒ `FormatException`.
Los `Set*` **acumulan** (no limpian; `ReinicializaValores` sí). Serialización inversa `GetXxx`: índices `true` en orden ascendente como `n + letra.Trim()` unidos por coma
(`FiltroNoVariantes.cs:335-356`).

**P7 · `RangosOpciones`** (`RangosOpciones.cs:26-105`): lista de intervalos `[min,max]` de `Double`. `PonerRangos("a-b#c#d-e")`: separa por `#`, cada trozo por `-`;
1 elemento ⇒ `min=max`; si no, `min=[0]`, `max=[1]` (un 3.er elemento se ignora). `ValorEnRangoValido(v)`: `true` si algún intervalo cumple **`v <= max && v >= min`**
(cerrado); **lista vacía ⇒ siempre `false`**. No admite negativos (`"-5"` ⇒ `Convert.ToDouble("")` lanza). `Convert.ToDouble` usa la cultura actual (T7, Abiertas A1).

### 0.5 Trampas Swift transversales a esta ESPEC

- **B1 · Desplazamientos.** C# enmascara el contador (`long`: &63; `int`: &31) y `<<` no lanza por desbordamiento. Swift `<<`/`>>` con contador ≥ ancho da 0 o desplaza a la
  inversa si es negativo. Usar `&<<`/`&>>` en `FormatoSignos`, `ObtenerSignoInt`, `Formato123.ConvStrToLong` (T1). Con `<<` sobre `Int64` Swift **no** traps (descarta bits), igual que C#;
  sí trapea `*`, `+`, `-` e `Int64(x)` fuera de rango.
- **B2 · `(uint)` de −1.** `(uint)IndexOf(...)=-1` ⇒ `0xFFFFFFFF` (sin `checked`, T1) y se hace OR con un `long`: sólo los 32 bits bajos. Receta: `Int64(UInt32(bitPattern: Int32(idx)))`.
- **B3 · `1 << -1`.** `int`: contador &31 ⇒ `Int32.min`, que al combinarse con un `long` se **extiende de signo**. Receta: `Int64(Int32(1) &<< Int32(idx))`.
- **B4 · `Convert.ToInt32/ToDouble(string)`** admiten espacios, signo y (Double) separador de millares según cultura (semántica de la BCL, no del repo). `Int("3 ")` de Swift da `nil`. Escribir un parser propio y fijarlo con golden de parseo.
- **B5 · Referencias.** `GrupoEquipos`, `Diferencia`, `FormatosSignos`, `FormatoSignos`, `Formato123`, `Simetria` son **clases con semántica de referencia** (listas compartidas entre filtro, controlador y relaciones): `final class`, no `struct`.
- **B6 · `bool[,]`** ⇒ array plano `[Bool]` (`17*17`) con índice `e*17+d`. `char` ⇒ `Character`/`UInt16` (la letra de tolerancia es un `char` UTF-16; en la práctica A-Z).
- **B7 · Excepciones.** El motor no atrapa excepciones en `Grupo`/`Analizador`/`GeneradorColumnas` (sin `try/catch` en `Grupo.cs`, `Analizador.cs`, `GeneradorColumnas.cs`), así que un `IndexOutOfRange` sube hasta el llamador (la UI). En Swift los índices fuera de rango también abortan; decidir en cada caso si la configuración inválida se rechaza al cargar (propuesta) manteniendo el resultado de los casos válidos.

---

## 1. FiltroDistancias — `Free1X2.Domain/MotorCalculo/FiltroDistancias.cs`

**Propósito.** Para cada clase de signo (`1`, `X`, `2` y «variantes» = X o 2) mide la **mayor separación entre dos apariciones consecutivas** a lo largo de los partidos y exige que ese valor pertenezca a un conjunto permitido.

**Tipos y datos.**

| C# | Swift | Notas |
|----|-------|-------|
| `MascUnos=5270498306774157604`, `MascEquis=2635249153387078802`, `MascDoses=1317624576693539401`, `MascVariantes=3952873730080618203` (`:27-30`) | `Int64` | 21 grupos de 3 bits: `‹100›`, `‹010›`, `‹001›`, `‹011›` (63 bits, verificado) |
| `intVar,int1,intX,int2,intGlobal : bool[N+1]` (`:46-50,65-69`) | `[Bool]` de `N+1` | `N = NumeroPartidos` **leído en el constructor** (`:63`); defecto `false` |
| `intVarTol,int1Tol,intXTol,int2Tol,intGlobalTol : char[N+1]` (`:53-57`) | `[Character]` de `N+1` | defecto `" "` |
| `distVar,distUnos,distEquis,distDoses : int` (`:38-41`) | `Int` | estado post-`Analizar` |
| `noIntGlobal : int` (`:37`) | `Int` | **nunca se asigna** (siempre 0) |

**Entradas / salidas.** `Analizar(col)→Bool`; propiedades `NoDistanciasVariantes/Unos/Equis/Doses` (`:621-636`), `NoDistancias`=`noIntGlobal` (`:489-492`).
Setters de configuración: `SetNoIntVar/SetdistVar`, `SetNoInt1/SetdistUnos`, `SetNoIntX/SetdistEquis`, `SetNoInt2/SetdistDoses` (idénticos dos a dos, `:285-383,493-595`) y
`SetNoIntGlobales` (`:260-283`), todos con el parser P6.

**Algoritmo.**
```
analizar(col):
    distUnos  = distancia(col & MascUnos)
    distEquis = distancia(col & MascEquis)
    distDoses = distancia(col & MascDoses)
    distVar   = distancia(col & MascVariantes)
    return intVar[distVar] && int1[distUnos] && intX[distEquis] && int2[distDoses]      // :143  (intGlobal NO interviene)

distancia(c):                                   // :109-128 ; c >= 0, solo bits de una clase
    d = 0
    if c != 0:
        while (c & 7) == 0: c >>= 3             // salta hasta la 1.ª aparición
        repeat:
            c >>= 3                             // consume la aparición actual
            act = 0
            while (c & 7) == 0 && c != 0: act += 1; c >>= 3
            if c != 0: d = max(act + 1, d)      // hay otra aparición: separación = act+1
        until c == 0
    return d
```
Definición: `d` = mayor diferencia de **posición** entre dos apariciones consecutivas del conjunto; `0` si hay menos de dos apariciones.
**Ejemplo (C\*):** `1` en p 1,4,5,8,12,13 ⇒ separaciones 3,1,3,4,1 ⇒ `distUnos=4`; `X` en 2,7,9,14 ⇒ 5,2,5 ⇒ `distEquis=5`; `2` en 3,6,10,11 ⇒ 3,4,1 ⇒ `distDoses=4`;
variantes (X o 2) en 2,3,6,7,9,10,11,14 ⇒ 1,3,1,2,1,1,3 ⇒ `distVar=3`. Con `intVar[3]=int1[4]=intX[5]=int2[4]=true` ⇒ válida.

`ObtenNoAciertosTolerancias(letras)` (`:207-237`):
```
n = 0
for letra in letras:
    n += (intGlobalTol[noIntGlobal] == letra) + (intVarTol[distVar] == letra) + (int1Tol[distUnos] == letra)
       + (intXTol[distEquis] == letra) + (int2Tol[distDoses] == letra)
return n
```
`AnalizarFallos` (`:146-170,193-200`): por este orden **var, unos, equis, doses**; para cada una: si `!flag[d]` ⇒ `"Fallo en distancia de <clase>  (d)"`; si `tol[d] != ' '` ⇒ `"!Tolerancia en distancia de <clase>  (d)"`
(`<clase>` = `variantes|unos|equis|doses`, **dos espacios** antes del paréntesis). Se concatenan con `#` final, se quita el último carácter y se parte por `#`; sin texto ⇒ `nil`.

`LlenarTodosValores` (`:597-610`): arma `"0,1,…,N-1"` (**sin incluir N**) y, para cada conjunto sin ningún `true`, en el orden unos, equis, doses, var, llama a su `Set` (letra `" "`).
`ReinicializaValores` (`:242-258`) pone todo a `false`/`" "` (incluye `intGlobal`). `GetInt1/GetIntX/GetInt2/GetIntVar` (`:385-466`) serializan como P6. **No existe** `GetIntGlobales`.

**Estado global.** Lee `VariablesGlobales.NumeroPartidos` (`:63,601`) y `AnalizarDistancias` (`:659`). No escribe.

**Peculiaridades a preservar.**
1. `noIntGlobal` es siempre 0 ⇒ en tolerancias se consulta **siempre `intGlobalTol[0]`** (`:215`). `SetNoIntGlobales("0A")` suma 1 a toda columna que consulte la letra `A`.
2. `intGlobal` se rellena pero **no se evalúa** (`:143`).
3. `LlenarTodosValores` rellena `0..N-1` (NoVariantes rellena `0..N`, §2); la distancia máxima real es N-1, así que es coherente.
4. Los `Set*` fuera de rango (`n>N`) o con cadena vacía lanzan excepción (P6); `FDistanciasData` los llama con cada atributo `values` (`EntradaSalida/FDistanciasData.cs:60-81`), por lo que **un conjunto guardado vacío no se puede recargar** (`GetIntVar()` devuelve `""`).
5. `Analizar` llama a `InicializaContadores` y `AnalizaColumna` (`:186-191`); `AnalizaColumna` es pública y no reinicia.

**Trampas Swift.** B6 (letras), B4 (parseo), `max`→`max`, bucles `while c != 0` con `Int64` ≥ 0 (la `>>` aritmética no afecta). Usar `Character.isLetter` solo si se acepta equivalencia con `Char.IsLetter` (Abiertas A12).

**Casos golden propuestos.**
- D1: `SetNoIntVar("1,2,3A")`, `SetNoInt1("0,1,2,3,4B")`, `SetNoIntX("0,5C")`, `SetNoInt2("0,4")`, `SetNoIntGlobales("0A")`; columnas G1..G9 + muestra. Esperado C\*: `Analizar=true`; `ObtenNoAciertosTolerancias("A")=2` (global + var), `"B"=1`, `"C"=1`, `"ABC"=4`, `"D"=0`; `AnalizarFallos` = `["!Tolerancia en distancia de variantes  (3)","!Tolerancia en distancia de unos  (4)","!Tolerancia en distancia de equis  (5)"]` (ref. calculada a mano sobre el código).
- D2: solo `SetdistVar("1")` + `LlenarTodosValores()` (uso de If-Then, `CondicionIfThen.cs:204-215`): comprobar que el resto acepta `0..N-1`.
- D3: `Analizar` con configuración vacía (todo `false`) para todas las columnas (esperado `false`, contadores correctos).
- D4: `AnalizarFallos` con fallo real (`Fallo en distancia…`) y con tolerancia coincidente.

**Evidencia.** `FiltroDistancias.cs:27-30,37-41,60-92,102-128,143,146-170,186-200,207-237,242-258,260-383,385-466,489-595,597-619,621-636,657-660`; `FDistanciasData.cs:47-81`; `CondicionIfThen.cs:204-215`; `Grupo.cs:331-408`; `ContenedorAnalisis.cs:155-161`.

---

## 2. FiltroNoVariantes — `…/FiltroNoVariantes.cs`

**Propósito.** Cuenta cuántas `X`, cuántos `2` y cuántas «variantes» (X o 2) tiene la columna y exige que cada recuento esté en su conjunto permitido.

**Tipos y datos.** `variantes, equis, doses : bool[N+1]` y sus `…Tol : char[N+1]` (`:39-45,52-69`; `N` leído en el constructor `:50`); contadores `noVariantesCol, noEquisCol, noDosesCol : int` (`:31-33`). Máscaras en `AnalizaColumna(long)`:
`X` = `80421421917330` (**16** grupos `‹010›`), `2` = `5026338869833` (**15** grupos `‹001›`) (`:107,111`; verificado).

**Entradas / salidas.** `Analizar(long)`, `AnalizaColumna(long)` y `AnalizaColumna(String)` (públicos), `NoVariantes/NoEquis/NoDoses` (`:448-461`), `Set*` (`:218-333`), `Get*` (`:335-402`).

**Algoritmo.**
```
analizar(col):                                                    // :155-160
    noEquis     = popcount(col & 80421421917330)
    noDoses     = popcount(col & 5026338869833)
    noVariantes = noEquis + noDoses                               // :116
    return variantes[noVariantes] && equis[noEquis] && doses[noDoses]

analizaColumna(String s):                                         // :80-101 (la usa la pantalla Boleto, BoletoFrmViewModel.cs:260-268)
    reinicia contadores
    for ch in s: 'X'|'x' → noVariantes++, noEquis++ ;  '2' → noVariantes++, noDoses++ ;  otro → nada
```
`ObtenNoAciertosTolerancias(letras)` (`:176-199`): por cada `letra`, `+1` por cada coincidencia entre `letra` y `variantesTol[noVariantes]`, `equisTol[noEquis]`, `dosesTol[noDoses]`.
`AnalizarFallos` (`:126-146`): orden variantes, equis, doses; por cada una `"Fallo en número de <variantes|equis|doses>  (n)#"` si `!flag[n]` y `"!Tolerancia en número de …  (n)#"` si `tol[n]!=' '`; se quita el último `#` y se parte; sin texto ⇒ `nil`.
`LlenarTodosValores` (`:404-416`): rellena **`0..N` inclusive** los conjuntos que no tengan ningún `true` (orden variantes, equis, doses).
`SetNoVariantes(int,bool)`, `(int[])`, `(string)` y equivalentes de equis/doses (`:218-333`); `SetNoX(int,active)` fija el flag sin tocar la letra.

**Ejemplo (C\*).** `noEquis=4`, `noDoses=4`, `noVariantes=8` ⇒ con `variantes={6,7A,8}`, `equis={3,4B,5}`, `doses={4}` ⇒ válida; `ObtenNoAciertosTolerancias("B")=1`, `("A")=0`, `("AB")=1`.

**Estado global.** `NumeroPartidos` (`:50,408`), `AnalizarVX2` (`:490`).

**Peculiaridades a preservar.**
1. **Máscaras asimétricas:** `X` cuenta hasta el partido 16 y `2` solo hasta el 15 (`:107,111`). Con ≤15 partidos no se nota; hay que replicarlo (en `FiltroDibujos` ambas son de 16).
2. `LlenarTodosValores` incluye `N` (a diferencia de Distancias).
3. Los `Set*` acumulan; un valor repetido pisa la letra anterior; vacío o fuera de rango lanza (P6). `FNoVariantesData` los llama con cada `values` (`EntradaSalida/FNoVariantesData.cs:52-66`), igual que Distancias.
4. Test existente: boleto de 14 triples, `variantes="0"` ⇒ 1 columna, `"1"` ⇒ 28, `"2"` ⇒ 364 (equis y doses con `0..14`), `ManualFuncionalTests.cs:28-45`.

**Trampas Swift.** P1 (popcount sobre `UInt64`), P6/B4, B6; índices del array: con ≤14 partidos los recuentos ≤14 caben en `N+1=15`.

**Casos golden propuestos.** N1: los 3 casos de `ManualFuncionalTests` (conteos 1/28/364). N2: configuración con letras anterior + `AnalizarFallos`. N3: `AnalizaColumna(String)` con `"1X2112X1X2211X"`, `"1x2…"` (minúscula) y cadena con `*`. N4: `LlenarTodosValores` con solo `variantes` fijado. N5: columnas G1..G9 + muestra.

**Evidencia.** `FiltroNoVariantes.cs:28-78,80-117,119-160,176-199,204-333,335-425,448-461,488-491`; `FNoVariantesData.cs:52-66`; `CondicionIfThen.cs:106-115`; `BoletoFrmViewModel.cs:260-268`; `ManualFuncionalTests.cs:28-45`.

---

## 3. FiltroFormatos123 — `…/FiltroFormatos123.cs`

**Propósito.** Reordena los tres signos de cada partido según una **valoración** (1.º, 2.º y 3.º más valorado ⇒ `1`, `2`, `3`), traduce la columna a una cadena de dígitos 1-2-3 y cuenta apariciones de **formatos** (subcadenas contiguas) con rangos permitidos.

**Tipos y datos.**

| C# | Swift | Notas |
|----|-------|-------|
| `arrayFormatos : List<Formato123>` (`:28`) | `[Formato123]` | referencia |
| `arrayAciertos : List<int>` (`:29`) | `[Int]` | valores permitidos de «líneas acertadas» (se consulta con `Contains`) |
| `valores : double[N,3]` (`:31`), `valoresTransformados : byte[N,3]` (`:30`) | `[[Double]]`, `[[UInt8]]` | inicializados a 0 con el `N` del momento de construcción; la carga de fichero los sustituye por **14×3** |
| `pasoFijo : bool` (`:32`) | `Bool` | en fichero: atributo `repeticiones` |
| `aciertosMax, aciertosMin : int` (`:34-35`) | `Int` | **no se persisten** (ver peculiaridades) |
| `aciertosFiltro : string` | `String` | texto original de `ArrayAciertos` |

`Formato123` (`Formato123.cs:23-81`): `Formato:string` (al asignarlo invalida la caché), `FormatoLong:long` calculado perezosamente (`:48-59`), `AciertosMin/AciertosMax:int`.

**Entradas / salidas.** `Analizar(long)` (`:391-394`), `AnalizarFallos(long)` (`:396-405`), `Col1X2ToCol123(String)` (pantalla, `:93-96`), `ContieneDatos` ⇒ `ArrayFormatos.Count>0 || ArrayAciertos.Count>0` (setter sin efecto, `:430-434`), `ObtenNoAciertosTolerancias` ⇒ 0 (`:436-440`).

**Algoritmo.**

*Valoración→rangos (`FFormatos123Data.cs:69-122`, que es lo que se guarda en `valoresTransformados`).* Para cada partido con valores `(v1,vX,v2)`, `T[p][signo]` = bit del puesto: **1.º=4, 2.º=2, 3.º=1**, ordenando por valor **descendente** y desempatando por índice de signo ascendente (`1` antes que `X` antes que `2`):

| orden por valor | `T[p][0],T[p][1],T[p][2]` |
|-----------------|---------------------------|
| `1≥X≥2` | 4,2,1 |
| `1≥2>X` | 4,1,2 |
| `X>1`, `X≥2`, `1≥2` | 2,4,1 |
| `X>1`, `X≥2`, `2>1` | 1,4,2 |
| `2>1`, `2>X`, `1≥X` | 2,1,4 |
| `2>1`, `2>X`, `X>1` | 1,2,4 |

(equivale a la ordenación estable; comprobado exhaustivamente con valores 0..3). Si algún valor es `NaN` ningún caso se cumple y la fila queda `0,0,0`.
La versión de cadenas `TransformarValoracion` del filtro (`:332-385`) da los mismos puestos como `"1","2","3"` y solo sirve a `Col1X2ToCol123` (`:93-96,311-331`).

```
trad123(col):                                                     // :267-289
    r = 0; i = 0
    while col != 0 && i < N_actual:                               // N_actual = NumeroPartidos en este momento (:271)
        switch col & 7:  4 → r = (r << 3) | T[i][0] ;  2 → r = (r << 3) | T[i][1] ;  1 → r = (r << 3) | T[i][2] ;  otro → nada
        col >>= 3; i += 1
    return r                                  // el partido 1 queda en el grupo MÁS ALTO: se lee como cadena en orden de partidos

formatoLong(f):                               // Formato123.cs:61-70
    res = 0 ; for ch in f: res = (res << 3) ^ (1 << indexOf("321", ch))     // '3'→1, '2'→2, '1'→4 (carácter ajeno: B3)

cuenta(colf, pat):  n = 0; t = colf; while t != 0 { if (t & pat) == pat { n += 1 }; t >>= 3 }; return n    // apariciones contiguas, SOLAPADAS
```
Por tanto `cuenta` = nº de veces que el formato aparece como **subcadena** en la cadena 123 de la columna (p. ej. `"11"` en `"111"` ⇒ 2).

```
analizar(col): colf = trad123(col);  pasoFijo ? fijo(colf) : libre(colf)                          // :97-104

libre(colf):                                                      // :115-157
    ag = 0
    for f in arrayFormatos:                                       // en orden
        a = cuenta(colf, f.formatoLong)
        if a >= f.aciertosMin && a <= f.aciertosMax && ag <= self.aciertosMax: ag += 1
        else if ag > self.aciertosMax: break
    return arrayAciertos.contains(ag)

fijo(colf):                                                       // :159-181  (ignora aciertosMin/Max de cada formato)
    ag = 0
    for f in arrayFormatos: if cuenta(colf, f.formatoLong) >= 1 { ag += 1 }       // "break" interno solo corta el conteo del formato
    return arrayAciertos.contains(ag)
```
`self.aciertosMax` satura `ag` en `aciertosMax+1` como máximo (solo se incrementa mientras `ag <= aciertosMax`).

`AnalizarFallos` (`:106-113,182-256`; recalcula `formatoLong` con el `ConvStrToLong` privado `:257-266`, mismo resultado):
- libre: `texto` = **última** `"Columna Fuera de Límites (Formato <f> Apariciones: <a>)"` de un formato que no se contó; si `!arrayAciertos.contains(ag)` devuelve en su lugar `"Fallo en Líneas Acertadas (<ag>)"`; si no, devuelve `texto` (¡puede ser no vacío aunque la columna **pase**!).
- fijo: `"Fallo en Aciertos Globales (<ag>)"` si `!contains`, si no `""`. Resultado: `texto==""` ⇒ `nil`; si no `texto.split("#")`.

**Ejemplo (C\*).** Valoración `(50,30,20)` en todos los partidos salvo el 3 con `(20,30,50)`: T normal `(4,2,1)`; p3 `(1,2,4)`. Cadena 123 de C\*: `12111321233112`. Formatos: `"123"` (rango 0-2) aparece 1; `"11"` (2-3) aparece 3; `"31"` (0-0) aparece 1; `"3"` (3-3) aparece 3. Con `arrayAciertos=[2,3]`:
- libre `aciertosMax=3` ⇒ `ag=3` ⇒ **true**; libre `aciertosMax=0` ⇒ `ag=1` (salta por `break` en el 2.º) ⇒ **false**; libre `aciertosMax=1` ⇒ `ag=2` ⇒ true.
- fijo ⇒ `ag=4` (los 4 formatos aparecen ≥1) ⇒ **false**.

**Estado global.** `NumeroPartidos` en campos (`:30-31`) y en `trad123` (`:271`); `AnalizarFormatos123` (`:469`).

**Peculiaridades a preservar.**
1. **`aciertosMax/Min` no se guardan ni se cargan.** `FFormatos123Data` nunca los fija (`EntradaSalida/FFormatos123Data.cs:18-68`); sólo la pantalla los asigna al aceptar (`aciertosMax=último de ArrayAciertos ordenado`, `Free1X2.WinUI/Views/Ported/Formatos123FrmViewModel.cs:326-328`, legacy `Free1X2/UI/Filtros/Formatos123Frm.cs:756-759`). Un filtro recién cargado de `.comb` en **paso libre** tiene `aciertosMax=0` ⇒ `ag` satura en 1. **Replicar (R1)**; ver Abiertas A2.
2. Con `ValoracionTranformada` sin asignar (todo 0) la traducción da `0` ⇒ ningún formato aparece.
3. La carga fija `new byte[14,3]`/`double[14,3]` (`FFormatos123Data.cs:25,71-72`): con `N>14` `trad123` indexa fuera de rango.
4. `trad123` omite los grupos con valor distinto de 4/2/1 (asteriscos): la cadena queda **más corta** y los formatos pueden unir partidos no adyacentes.
5. `AnalizarFallos` puede devolver texto aunque `Analizar` sea `true` (libre).
6. Formato con carácter fuera de `1,2,3` ⇒ `IndexOf=-1` ⇒ B3. La pantalla restringe a 1-14 caracteres válidos (`Formatos123FrmViewModel.cs:185`).
7. `Col1X2ToCol123` convierte a mayúsculas (`ToUpper`, cultura actual) antes de traducir; un carácter que no sea `1`/`X` se trata como `2` (`:311-331`).
8. Varios `grupoformato` en el XML: los formatos se acumulan y `PasoFijo`/`AciertosFiltro`/valoración los fija el **último** (`FFormatos123Data.cs:27-66`); sólo se escribe uno (`:142`).
9. Valoración del fichero: `Convert.ToDouble` por cada valor separado por `/` (42 valores, orden partido→signo `1,X,2`), cultura actual (`:36-49`); escritura con `ToString()` de cultura (`:181-196`). Abiertas A1.

**Trampas Swift.** B1/B3 (`&<<`), B4, T3 no aplica (todo `Double`/enteros), B5 (`Formato123` clase), comparación `>=`/`>` entre `Double` idéntica; `NaN` no cumple ninguna comparación.

**Casos golden propuestos.** F1: ejemplo anterior (3 modos). F2: formatos de longitud 1, 2, 3 y 14; solapados (`"11"` sobre `1111`). F3: tabla de empates (valores iguales: `(1,1,1)`, `(1,2,2)`, `(2,2,1)`). F4: columnas con asteriscos. F5: `aciertosMax` = 0 (cargado de fichero) vs = último de `ArrayAciertos`. F6: `AnalizarFallos` libre con filtro que pasa y texto no vacío.

**Evidencia.** `FiltroFormatos123.cs:27-36,93-104,106-113,115-181,182-256,257-289,311-385,391-405,430-440,467-470`; `Formato123.cs:34-70`; `FFormatos123Data.cs:25-67,69-122,142-196`; `Formatos123FrmViewModel.cs:185,324-328`; `Free1X2/UI/Filtros/Formatos123Frm.cs:756-759`.

---

## 4. Formato123 — `…/Formato123.cs`

**Propósito.** Un formato (cadena de dígitos 1-3) con su rango `[AciertosMin, AciertosMax]` de apariciones.
**Tipos y datos.** `formato:String`, `aciertosMin/Max:Int`, `formatoLong:Int64`, `formatoLongCalculado:Bool` (`:25-32`). Defecto `""`, 0, 0.
**Algoritmo.** `FormatoLong` = `formatoLong(f)` de §3 (réplica del `ConvStrToLong` privado de `FiltroFormatos123`), calculado una vez; asignar `Formato` invalida la caché (`:34-59`).
**Estado global.** Ninguno. **Peculiaridades.** Caché solo depende del texto (P-05). Carácter fuera de `321` ⇒ B3.
**Trampas Swift.** B3, `final class`. **Golden.** `"123"→0b100_010_001=273`, `"3"→1`, `"31"→12`, `"11"→36`, `""→0`.
**Evidencia.** `Formato123.cs:25-81`.

---

## 5. FiltroValoracionSignos — `…/FiltroValoracionSignos.cs` (+ `RangosOpciones.cs`)

**Propósito.** Asigna a cada partido una puntuación por signo (`valores1`, `valoresX`, `valores2`) y exige que la **suma** (o el **producto escalado**) de las puntuaciones de la columna, global y por clase de signo, caiga en rangos permitidos.

**Tipos y datos.** `valores1, valoresX, valores2 : double[]` (`:40-42`; en el constructor de tamaño `N`, `:59-61`, pero la carga de fichero los **sustituye** por arrays de tamaño = nº de nodos `valores`, `FValoracionData.cs:40-58`); `valoracion, valoracionUnos, valoracionEquis, valoracionDoses : double` (`:44-47`);
`tipoValoracion : string` = `"suma"` por defecto (`:33`), con `esSuma/esMultiplo` precalculados al asignar (`:37-38,316-326`, comparación **ordinal exacta**); cuatro `RangosOpciones` (`:52-55`) con sus cadenas `valorGlobal/Unos/Equis/Doses` (`:48-51`, setters `:256-291` que reconstruyen los rangos: `ReinicializarRangos` y, si no es `""`, `PonerRangos`).

**Entradas / salidas.** `Analizar(long)`, `AnalizarFallos(long)`; configuración por `TipoValoracion`, `ValorGlobal/ValorUnos/ValorEquis/ValorDoses` (texto de rangos P7) y `Valores1/ValoresX/Valores2`; salida `ValoracionResultado/Unos/Equis/Doses` (`Double`). Sin ficheros ni estado global escrito.

**Algoritmo.**
```
analizar(col):                                                     // :194-199
    valoracion = valoracionUnos = valoracionEquis = valoracionDoses = 0
    if esSuma: sumas(col)  else if esMultiplo: multiplos(col)      // cualquier otro texto: no calcula (quedan 0)
    return rangosGlobales.contiene(valoracion) && rangosUnos.contiene(valoracionUnos)
        && rangosEquis.contiene(valoracionEquis) && rangosDoses.contiene(valoracionDoses)      // :162, cerrado, P7

sumas(col):                                                        // :98-125
    i = 0
    while col != 0:
        switch col & 7:  4 → valoracion += v1[i]; valoracionUnos += v1[i]
                         2 → valoracion += vX[i]; valoracionEquis += vX[i]
                         1 → valoracion += v2[i]; valoracionDoses += v2[i]
        col >>= 3; i += 1                                          // i avanza también en grupos 0 (asteriscos); orden de suma = partido ascendente

multiplos(col):                                                    // :126-158 ; arranca los cuatro acumuladores en 1
    k = 0.03420425138
    mismo recorrido con   valoracion *= (v1[i] * k)   (y el acumulador de su clase igual)       // multiplicador = producto PARCIAL v*k, luego se acumula
    al final: if valoracion == 1 { valoracion = 0 }; idem unos, equis, doses                    // igualdad EXACTA
```
**Ejemplo (C\*, suma)** con `v1[i]=i+1`, `vX[i]=0.5(i+1)`, `v2[i]=0.25(i+1)`: unos `1+4+5+8+12+13=43`; equis `0.5(2+7+9+14)=16`; doses `0.25(3+6+10+11)=7.5`; global **66.5**. Con rangos `ValorGlobal="60-70#100-120"`, `ValorUnos="40-45"`, `ValorEquis="10-16"`, `ValorDoses="0-7.4"` ⇒ `false` (7.5 > 7.4; 16 sí entra: intervalo cerrado). `AnalizarFallos` ⇒ `["Fallo en valoración de 2  (7,5)"]` (decimal según cultura, ver A1).
**Ejemplo (C\*, múltiplo)** con `v1=40`, `vX=v2=30` en los 14 partidos: global `8.06215678589256`, unos `6.559043110325511`, equis=doses `1.1086778658258967` (ref. Python, IEEE-754 doble).

`AnalizarFallos` (`:169-186,201-208`): mismos cuatro tests en el orden global, 1, X, 2: `"Fallo en valoración global  (v)#"`, `"Fallo en valoración de 1  (v)#"`, `"…de X  (v)#"`, `"…de 2  (v)#"` (el valor se interpola con `double.ToString()` de la cultura actual); se quita el último `#` y se parte; sin texto ⇒ `nil`.
`ObtenNoAciertosTolerancias` ⇒ 0 (`:215-221`). `ReinicializaValores` (`:225-233`) pone a 0 los tres arrays (lee `NumeroPartidos` actual). Salida consumida por el análisis: `ValoracionResultado/Unos/Equis/Doses` (`:350-365`); el contenedor los **castea a `float` y redondea con `Math.Round(double,0)`** (`ContenedorAnalisis.cs:267,426-429`, T3/T4; spec en ESPEC_06).

**Estado global.** `NumeroPartidos` (`:59-61,227`), `AnalizarValoracion` (`:393`).

**Peculiaridades a preservar.**
1. **Una `RangosOpciones` vacía rechaza todo** (`RangosOpciones.cs:58-74`): `valorGlobal/Unos/Equis/Doses == ""` ⇒ el filtro nunca acepta. La pantalla rellena los cuatro por defecto (`ValoracionFrmViewModel.cs:275-283`).
2. `tipoValoracion` distinto de `suma`/`multiplo` no calcula nada (todo 0); la validez depende de si cada rango contiene 0.
3. Asociatividad de coma flotante: `acc *= (v*k)` (no `acc*v*k`); Swift `valoracion *= v1[i] * k` evalúa igual; no usar `addingProduct`/FMA.
4. Se suma en orden de partido ascendente y los asteriscos (grupo 0) no aportan; la columna `0` da suma 0 / producto 1→0.
5. Índice `i` fuera del array (más grupos que valoraciones) ⇒ `IndexOutOfRange`.
6. `valoraciones` del fichero: se leen con `Replace(".",",")` + `Convert.ToDouble` (asume cultura de coma decimal) y se escriben con `ToString().Replace(",",".")` (`FValoracionData.cs:50-52,92-94`). Abiertas A1.

**Trampas Swift.** Todo `Double` (no `Float`); sumas en el mismo orden; comparaciones cerradas; `==` exacto con 1; parseo de rangos con locale explícito (A1).

**Casos golden propuestos.** V1: ejemplo suma/múltiplo anteriores. V2: rangos múltiples `a-b#c`, un solo valor, límites exactos. V3: rango vacío. V4: tipo `"otro"`. V5: columna `0` y columnas con asteriscos. V6: `valores` con decimales y notación científica (lectura).

**Evidencia.** `FiltroValoracionSignos.cs:33-61,76-125,126-158,160-208,215-233,235-291,316-326,350-365,391-394`; `RangosOpciones.cs:30-74`; `FValoracionData.cs:34-58,71-96`; `ContenedorAnalisis.cs:263-269,424-436`; `ValoracionFrmViewModel.cs:275-283`.

---

## 6. FiltroSimetrias — `…/FiltroSimetrias.cs`

**Propósito.** Cuenta cuántos **grupos de partidos «simétricos»** tienen todos sus partidos con el mismo signo y exige que ese número esté en un conjunto permitido.

**Tipos y datos.** `arraySimetrias:[Simetria]` (`:29`), `arrayAciertos:[Int]` (`:30`), `aciertosString:String=""` (`:32`), `aciertos:Int` (`:33`, expuesto como `AciertosSimetria`, `:50-54`), `figuras` (sin uso).

**Entradas / salidas.** `Analizar(long)`/`Analizar(String)`, `AnalizarFallos(long)`; configuración `ArraySimetrias`, `ArrayAciertos`; salida `AciertosSimetria`.

**Algoritmo.**
```
analizar(col):                                                     // :64-89
    aciertos = 0
    for s in arraySimetrias:
        signo1 = signoInt(col, s.partidos[0])                      // P2, grupo crudo
        ok = true
        for j in 1..<s.partidos.count: if signoInt(col, s.partidos[j]) != signo1 { ok = false; break }
        if ok { aciertos += 1 }
    return arrayAciertos.firstIndex(of: aciertos) != nil
analizar(String c) = analizar(convStrToLong(c))                    // :58-63
```
`AnalizarFallos` (`:90-136`): recalcula `ac` de la misma forma (sin tocar `aciertos`) y, si `!arrayAciertos.contains(ac)`, devuelve `["Fallo en Número de Aciertos (<ac>)"]`; si no `nil`.
`ObtenNoAciertosTolerancias` ⇒ 0 (`:176-180`).

`Simetria` (`Simetria.cs:26-54`): `Simetria(String partidos)` parte por `,` y convierte cada trozo con `Convert.ToInt32` a la lista `PartidosSimetricos` (números de partido **1-based**) y guarda el texto en `Partidos`.

**Ejemplo (C\*).** Simetrías `"1,4,5"`, `"2,7"`, `"3,6,10"`, `"1,2"` ⇒ (1,1,1) ✓, (X,X) ✓, (2,2,2) ✓, (1,X) ✗ ⇒ `aciertos=3`; con `arrayAciertos=[3]` ⇒ `true`; con `[2]` ⇒ `false` y fallo `"Fallo en Número de Aciertos (3)"`.

**Estado global.** `AnalizarSimetrias` (`:204`).

**Peculiaridades a preservar.**
1. Compara el **grupo crudo de 3 bits**: dos asteriscos (0) cuentan como iguales; una simetría de un solo partido siempre cuenta.
2. `ContieneDatos` (getter, `:161-174`) tiene **efecto lateral**: si no hay simetrías **vacía `ArrayAciertos`** y devuelve `false`; el setter no hace nada.
3. `Analizar` sí actualiza `aciertos` (estado leído por el análisis, `ContenedorAnalisis.cs:259-262`); `AnalizarFallos` no.
4. Partido ≤0 o grande ⇒ desplazamiento enmascarado (P2/B1). `Simetria("")` lanza (`Convert.ToInt32("")`).
5. Fichero: `gruposimetrias@aciertos` (vacío ⇒ `"0"`, último gana si hay varios) y `simetrias@simetria` (`EntradaSalida/FSimetriasData.cs:24-45`).

**Trampas Swift.** B1 (`&>>`), B5, lista de partidos `[Int]`.

**Casos golden propuestos.** S1: ejemplo anterior. S2: simetría de 1 partido, de 14, con partidos repetidos. S3: columnas con asteriscos. S4: `ArrayAciertos=[0]` y vacío. S5: `ContieneDatos` con lista vacía (efecto lateral).

**Evidencia.** `FiltroSimetrias.cs:28-54,58-136,161-180,202-205`; `Simetria.cs:26-54`; `FSimetriasData.cs:24-45`; `UtilColumnas.cs:66-76`.

---

## 7. Simetria — `…/Simetria.cs`

Ver §6 (se documenta junto a su filtro). **Tipos:** `partidosSimetricos:[Int]` (inicializada vacía, `:28`), `partidos:String`. **Algoritmo:** `obtenerPartidosSimetricos(parts)` = `parts.split(",").map(Convert.ToInt32)` añadidos en orden (`:45-53`); el constructor lo invoca **antes** de guardar `Partidos` (`:30-34`). **Golden:** `"1,4,5"→[1,4,5]`, `"7"→[7]`, `"1,x"` ⇒ error de formato. **Evidencia.** `Simetria.cs:26-54`.

---

## 8. FiltroDibujos — `…/FiltroDibujos.cs`

**Propósito.** Un **dibujo** es el par `(nº de X) + (nº de 2)` de la columna; el filtro acepta si el par está en la lista de dibujos permitidos.

**Tipos y datos.** `dibs : bool[17,17]` (`:37`) → `[Bool]` 289 elementos; `dibujos : ArrayList` de strings `"e+d"` (`:35`); `noEquisCol, noDosesCol : int` (`:32-33`).

**Entradas / salidas.** `Analizar(long)`, `AnalizarFallos(long)`; configuración `Dibujos` (setter) / `GetDibujos()`; salida `NoEquisCol/NoDosesCol`.

**Algoritmo.**
```
analizar(col):                                                     // :44-53,61-66
    noEquisCol = popcount(col & 80421421917330)                    // 16 grupos ‹010›
    noDosesCol = popcount(col & 40210710958665)                    // 16 grupos ‹001›  (aquí SÍ 16; NoVariantes usa 15)
    return dibs[noEquisCol, noDosesCol]

set Dibujos(lista):                                                // :113-135
    dibujos = lista
    if lista != nil && lista.count > 0:
        contieneDatos = true ; dibs = todo false
        for str in lista: (e, d) = str.split("+").map(Int) ; dibs[e, d] = true
    // lista vacía o nula: NO se reconstruye dibs ni se toca contieneDatos
```
`AnalizarFallos` (`:68-80`): si no cumple ⇒ `["Fallo en número de dibujos  (<e>+<d>)"]` (dos espacios); si no `nil`. `GetDibujos()` (`:96-111`) une las cadenas con `,`. `ObtenNoAciertosTolerancias` ⇒ 0.
**Ejemplo (C\*).** `noEquisCol=4`, `noDosesCol=4` ⇒ dibujo `4+4`; con `Dibujos=["4+4","5+3"]` ⇒ `true`; con `["5+3"]` ⇒ `false` y fallo `"Fallo en número de dibujos  (4+4)"`.

**Estado global.** `AnalizarDibujos` (`:182`).

**Peculiaridades a preservar.**
1. `dibs` solo se reconstruye **en el setter** `Dibujos`. Añadir a la lista devuelta (`filtro.Dibujos.Add(x)`) **no** actualiza `dibs`: el filtro creado por «If-Then» con una condición «Dibujo» (`CondicionIfThen.cs:121-123`) tiene `dibs` todo `false` y **rechaza siempre** (deducido por lectura; confirmar con golden, Abiertas A6).
2. Asignar una lista vacía no limpia `dibs` ni `contieneDatos`.
3. Índices > 16 en el fichero ⇒ `IndexOutOfRange`; el par `e+d` no se valida (`e+d ≤ N` no se exige).
4. `GetDibujos` hace `valores.Trim()` sin asignar (sin efecto, `:108`).
5. Fichero: cada hijo de `condicion` aporta `InnerText` `"e+d"` (`EntradaSalida/FDibujosData.cs:36-45`).

**Trampas Swift.** B6 (`bool[,]` plano), P1; el nombre `Dibujos` choca con `Estadisticas.Dibujos` (§17): renombrar en Swift (`DibujosEstadistica`).

**Casos golden propuestos.** Di1: ejemplo anterior. Di2: lista vacía tras una no vacía (no limpia). Di3: todas las combinaciones `e+d` con `e+d ≤ 14` (comprobar el contador). Di4: construcción estilo If-Then (`Dibujos.Add`) ⇒ siempre `false`.

**Evidencia.** `FiltroDibujos.cs:28-66,68-80,96-135,180-183`; `FDibujosData.cs:36-45`; `CondicionIfThen.cs:121-123`; `ControladorIfThen.cs:171-174`.

---

## 9. FiltroFormatosSignos — `…/FiltroFormatosSignos.cs`

**Propósito.** Evalúa uno o varios **grupos de formatos**. Un formato es un patrón de signos de longitud ≥1 (`1`,`X`,`2`, `V`=X o 2, `*`=cualquiera) que se busca como **subcadena contigua** en la columna **sin asteriscos**; cada grupo exige un nº de apariciones por línea y, opcionalmente, un nº de líneas válidas y de apariciones globales.

**Tipos y datos.** `arrayFormatos:[FormatosSignos]` (`:34`), `noLineasValidas`, `noAparicionesGlobal : int` (`:31-32`, = valores del **último grupo que fue válido** en la última llamada), `contieneDatos` (el setter `FormatosSignos` lo fija a `Count>0`, `:116-133`).

**Entradas / salidas.** `Analizar(long)`, `AnalizarFallos(long)`; configuración `FormatosSignos` (lista de grupos); salida `NoLineasValidas/NoAparicionesGlobal`.

**Algoritmo.**
```
analizar(col):                                                     // :41-66
    colSinAst = 0
    while col != 0: s = col & 7 ; if s != 0 { colSinAst = (colSinAst << 3) | s } ; col >>= 3     // quita asteriscos; el 1.er partido queda en el grupo MÁS ALTO
    valida = true
    for g in arrayFormatos:
        valida = g.analizar(colSinAst)
        if !valida: break                                          // "do not check any more groups"
        noLineasValidas = g.noLineasValidas ; noAparicionesGlobal = g.noAparicionesGlobal
    return valida                                                  // sin grupos ⇒ true
```
`AnalizarFallos` (`:68-94`): misma compactación; para cada grupo (numerado desde 1) concatena `g.analizar(colSinAst, numGrupo)` (§10) **sin cortar** al fallar; quita el último carácter (`#`) y parte por `#`; sin texto ⇒ `nil`. **No** actualiza `noLineasValidas/noAparicionesGlobal`. `ObtenNoAciertosTolerancias` ⇒ 0.

**Estado global.** `AnalizarFormatos` (`:168`).

**Peculiaridades a preservar.**
1. **La adyacencia se evalúa sobre la columna compactada**: con una máscara de grupo, el formato puede unir partidos no consecutivos del boleto. Ejemplo C\* con máscara `{1,2,3,5,8,9}` ⇒ compactada `1X211X` ⇒ `"11"` aparece 1 vez (p 5 y 8), `"1V"` 2 veces, `"X2"` 1 vez.
2. Al fallar un grupo, `noLineasValidas/noAparicionesGlobal` del **filtro** conservan los valores de la columna anterior; los de cada `FormatosSignos` se actualizan hasta donde se evaluó (los lee el análisis por grupo, `ContenedorAnalisis.cs:294-298`).
3. Compactar descarta grupos a 0 pero **no** reordena partidos entre sí (solo invierte su posición en el long).
4. Más de 21 grupos no nulos desbordarían el `long` (los bits sobrantes se pierden): fuera de uso (el formato ≤ N caracteres, `FormatosFrmViewModel.cs:158-166`).
5. Fichero: un hijo por grupo con `lineas`, `global` y líneas `formato`/`rango` (`EntradaSalida/FFormatosSignosData.cs:43-69`).

**Trampas Swift.** `<<` sobre `Int64` descarta bits sin trap (no usar `*`); B1; B5.

**Casos golden propuestos.** FS1: ejemplo de §10. FS2: dos grupos, el 1.º falla (estado de contadores). FS3: columnas con asteriscos (3 máscaras). FS4: `AnalizarFallos` con varios grupos. FS5: sin grupos ⇒ `true`.

**Evidencia.** `FiltroFormatosSignos.cs:31-34,41-94,116-133,166-179`; `ContenedorAnalisis.cs:285-301`; `FFormatosSignosData.cs:43-69`; `FormatosFrmViewModel.cs:158-166`.

---

## 10. FormatosSignos — `…/FormatosSignos.cs` (un grupo de formatos)

**Propósito.** Un grupo = lista de **líneas** (`FormatoSignos`) + dos restricciones opcionales: `Lineas` (cuántas líneas deben cumplirse) y `Global` (suma de apariciones de todas las líneas).

**Tipos y datos.** `lineasFormatos:[FormatoSignos]` (`:29`), `strLineas, strGlobal : String = ""` (`:30-31`), `lineas, global : [Bool]` (P4, calculados al asignar `Lineas`/`Global` si no son `""`, `:135-164`), `noLineasValidas, noAparicionesGlobal : Int` (`:32-33`).

**Algoritmo.**
```
analizar(col):                                                     // :38-85
    valida = true ; noLineasValidas = 0 ; noAparicionesGlobal = 0
    for f in lineasFormatos:
        valida = f.analiza(col)
        noAparicionesGlobal += f.noApariciones
        if !valida { if strLineas == "" && strGlobal == "" { break } }
        else        { noLineasValidas += 1 }
    if strLineas != "" || strGlobal != "":
        ok = true
        if strLineas != "": ok = lineas.count > noLineasValidas && lineas[noLineasValidas]
        if strGlobal != "" && ok: ok = global.count > noAparicionesGlobal && global[noAparicionesGlobal]
        valida = ok
    return valida
```
Modo **«Y» de líneas** (ambos textos vacíos): válida ⇔ todas las líneas válidas (corta en la primera que falla, y entonces `noAparicionesGlobal` es parcial). Modo **«contado»**: evalúa todas las líneas y decide por `Lineas`/`Global`.

`analizar(col, numGrupo) → String` (`:87-127`): igual recorrido; añade `"Fallo en Linea nº <i+1> (<formato>) en grupo <n>  (<apariciones>)#"` por cada línea que falla (**también en modo contado**, aunque el grupo pueda salir válido); luego, si hay `Lineas`/`Global`: `"Fallo en Líneas en grupo <n>  (<noLineasValidas>)#"` y `"Fallo en Global en grupo <n>  (<noAparicionesGlobal>)#"` cuando el valor queda fuera. El corte por línea fallida ocurre solo en modo «Y» (después de añadir el texto).

**Ejemplo (C\*).** Líneas `("11", rango "1-3")` y `("1V", rango "4")`: apariciones 2 y 4 ⇒ ambas válidas, `noLineasValidas=2`, `noAparicionesGlobal=6`. Con `Lineas="1-2"` (array de 4: `[F,T,T,F]`) y `Global="5-8"` (array de 10, `true` en 5..8) ⇒ **válida**.

**Estado global.** Ninguno. **Peculiaridades.** (1) `lineas`/`global` obsoletos si se reasigna a `""` (no se leen: guardadas por `strLineas != ""`). (2) Mensajes de fallo de línea aparecen aunque la columna pase en modo contado. (3) `Lineas`/`Global` no numéricos ⇒ `FormatException` al asignar.
**Trampas Swift.** P4 (longitud `max+2`, guarda de índice), B5. **Golden.** Los de FS1-FS4 con modos «Y» y «contado».
**Evidencia.** `FormatosSignos.cs:29-85,87-127,135-164`; `FFormatosSignosData.cs:48-49`.

---

## 11. FormatoSignos — `…/FormatoSignos.cs` (una línea)

**Propósito.** Cuenta las apariciones de **un patrón** en la columna compactada y comprueba que el recuento esté en un rango.

**Tipos y datos.** `formato:String` (siempre en mayúsculas), `_Longformato:Int64`, `_ValMin:Int64`, `rangoAparicion:String`, `arrayRangos:[Bool]?`, `noApariciones:Int` (`:27-32`).

**Algoritmo.**
```
set Formato(v):                                                    // :67-75
    formato = v.uppercased()                                       // ToUpper cultura actual
    _Longformato = convFormatoToLong(formato)
    _ValMin = 1 << ((formato.count - 1) * 3)                       // 1L << n : contador enmascarado &63 (B1)
convFormatoToLong(f):                                              // :77-85
    res = -1                                                       // ~0 : TODOS los bits a 1
    for ch in f: res = (res << 3) | codigo(ch)                     // ' '→0, '2'→1, 'X'→2, 'V'→3, '1'→4, '*'→7 ; otro → 0xFFFFFFFF (B2)
analiza(col):                                                      // :39-65
    noApariciones = 0
    while col != 0 && col >= _ValMin:
        if (col & _Longformato) == col { noApariciones += 1 }
        col >>= 3
    return arrayRangos.count > noApariciones && arrayRangos[noApariciones]
```
Tabla `" 2XV1  *"`: índice 0 espacio, 1 `2`, 2 `X`, 3 `V`, 4 `1`, 5-6 espacios (inalcanzables: `IndexOf` devuelve el primer espacio), 7 `*`. No hay código para «1X» o «12» (se necesita `V`/`*`).
Significado: tras `n` caracteres, `_Longformato` tiene los grupos `n-1..0` = códigos del patrón (primer carácter en el grupo más alto) y **todos los bits por encima a 1**; `(col & L) == col` exige que los `n` grupos bajos de `col` estén contenidos en los del patrón (el resto de `col` queda libre). `col >= _ValMin` exige que queden al menos `n` grupos. Es decir: nº de **posiciones** `k` en 0..(G−n) tales que el partido `k+j` encaja con `patrón[j]` (solapadas incluidas). Como los grupos a 0 se eliminaron antes, la ventana nunca contiene asteriscos.
**Ejemplo (C\* compactada = `1X2112X1X2211X`).** `"11"`⇒2, `"1V"`⇒4, `"V"`⇒8, `"*"`⇒14, `"1X"`⇒3, `"X2"`⇒2, `"2"`⇒4, `"12*"`⇒1, `"1"`⇒6, `"XV1"`⇒1 (ref. Python).
`RangoAparicion` (`:88-101`): si no es `""`, `arrayRangos = ObtenBoolArray(rango)` (P4); si es `""` **se queda `nil` y `analiza` lanza `NullReference`**.

**Peculiaridades a preservar.** (1) El desplazamiento y el OR operan en `long` con signo; `_Longformato` es negativo. (2) `ToUpper` convierte `x`→`X`, `v`→`V`. (3) Patrón vacío: `_ValMin = 1<<61` (contador −3&63) y `_Longformato=-1` ⇒ comportamiento no usado (la pantalla valida longitud 1..N). (4) Códigos 5/6 inalcanzables.
**Trampas Swift.** B1 (`&<<`), B2, `Int64` con signo, `col` ≥0. **Golden.** Tabla del ejemplo + patrones con `*`, `V`; columna con 1 solo partido; patrón más largo que la columna.
**Evidencia.** `FormatoSignos.cs:27-32,39-65,67-85,88-101`.

---

## 12. FiltroDiferencias — `…/FiltroDiferencias.cs`

**Propósito.** Contenedor de `Diferencia`s: la columna es válida si **todas** las diferencias lo son.

**Entradas / salidas.** `Analizar(long)`, `AnalizarFallos(long)`; configuración `Diferencias` (lista); salida: contadores de cada `Diferencia`.

**Algoritmo.**
```
analizar(col):  for d in diferencias: if !d.analizar(col) { return false } ; return true          // :26-36 ; vacío ⇒ true
```
`AnalizarFallos` (`:38-82`): para cada `Diferencia i` (1-based) que **no** cumple añade `"Fallos en Diferencias <i>: "` y luego, según sus banderas: si `AnalizaVX2Dib`: `"Variantes (<n>) "` si `!AcV[NoVariantes]`, `"Equis (<n>) "` si `!AcX[NoEquis]`, `"Doses (<n>) "` si `!AcDoses[NoDoses]`, `"Dibujos (<n>) "` si `!AcDib[NoDibujos]`; si `AnalizaInterrupciones`: `"Interrupciones (<n>) "` si `!AcInt[NoInterrupciones]`; si `AnalizaFormatos`: `"Formatos (<n>)#"` si `!AcFormatos[NoFormatos]`. Después `texto.split("#")` si no está vacío, si no `nil`.
- **Solo el apartado Formatos añade `#`**: los fallos de otros apartados quedan pegados sin separador y el último elemento puede ser `""` (Abiertas A9).

`ContieneDatos` (getter con efecto, `:107-122`): `Count==0` ⇒ `contieneDatos=false`, devuelve `false`; si no, lo pone a `true` y devuelve `true`. `ObtenNoAciertosTolerancias` **lanza** `Exception("The method or operation is not implemented.")` (`:124-127`); `Figuras` get/set lanzan (`:134-144`); `UsaFiguras()` ⇒ `false` (`:129-132`).
**Tipos.** `diferencia:[Diferencia]` vacío por defecto, `isActive=false` (`:9-16`).
**Estado global.** `AnalizarSimetriasII` (`:148`).

**Peculiaridades a preservar.** (1) Con una `Diferencia` activa y **al menos una tolerancia** definida en el grupo, `Grupo` llama `ObtenNoAciertosTolerancias` (`Grupo.cs:348,391`) y la excepción **no se atrapa** dentro del motor (sube al llamador): Abiertas A5. (2) El texto de `AnalizarFallos` usa los contadores que dejó `dif.Analizar` (se llama primero, `:45`).
**Trampas Swift.** `ObtenNoAciertosTolerancias` debe poder señalar error (`throws`) o documentarse como `fatalError`; `figuras` ⇒ `nil`.
**Golden.** Ver §13. **Evidencia.** `FiltroDiferencias.cs:9-36,38-82,107-149`; `Grupo.cs:348,391`.

---

## 13. Diferencia — `…/Diferencia.cs`

**Propósito.** Dado un conjunto de **sub-grupos de partidos** (típicamente del mismo tamaño), mide **cuántos valores distintos** toman, entre los sub-grupos, el nº de variantes, de X, de 2, el dibujo, las interrupciones y el propio formato (subcolumna); cada medida debe estar en su conjunto permitido.

**Tipos y datos.**

| C# | Swift | Notas |
|----|-------|-------|
| `partidosSimetricos : List<bool[]>` (`:8`) | `[[Bool]]` | cada `bool[]` 1-based de `NumeroPartidos+1` (P5b) |
| `acV, acX, acDoses, acDib, acInt, acFormatos : bool[]` (`:9`) | `[Bool]` | indexados por el **recuento de valores distintos** |
| `analizaVX2Dib, analizaFormatos, analizaInterrupciones : bool` (`:16-18`) | `Bool` | **defecto `true`** |
| `ResV, ResX, ResDoses, ResDib, ResInt, ResFormatos : int` (`:20-25`) | `Int` | expuestos `NoVariantes…NoFormatos` (`:259-282`) |
| `fI : FiltroInterrupciones`, `f : FiltroNoVariantes` (`:27-28`) | instancias propias | se reutilizan entre llamadas |
| `resultadosV/X/Doses/Int : bool[17]`, `resultadosDib : bool[17,17]`, `formatos : Dictionary<long,bool>` (`:10-15`) | `[Bool]`, `[Bool]` plano, `Set<Int64>` | solo pertenencia (T6 no aplica) |

**Algoritmo.**
```
analizar(col):                                                     // :30-119
    ResV = ResX = ResDoses = ResDib = ResInt = ResFormatos = 0 ; limpiar resultados* y formatos
    for sel in partidosSimetricos:                                 // en orden
        c = compactarPartidos(sel, col)                            // P3
        if analizaFormatos:       if c ∉ formatos { ResFormatos += 1 ; formatos.insert(c) }
        if analizaInterrupciones: fI.analizaColumna(c) ; k = fI.noInterrupciones ; if !resultadosInt[k] { ResInt += 1 ; resultadosInt[k] = true }
        if analizaVX2Dib:
            f.analizaColumna(c)                                    // popcounts de §2 (X: 16 grupos, 2: 15 grupos)
            v = f.noEquis + f.noDoses ; x = f.noEquis ; d = f.noDoses
            if !resultadosV[v]     { ResV += 1 ; resultadosV[v] = true }
            if !resultadosX[x]     { ResX += 1 ; resultadosX[x] = true }
            if !resultadosDoses[d] { ResDoses += 1 ; resultadosDoses[d] = true }
            if !resultadosDib[x, d]{ ResDib += 1 ; resultadosDib[x, d] = true }
    valida = true
    if analizaInterrupciones:              valida = acInt[ResInt]
    if analizaFormatos && valida:          valida = acFormatos[ResFormatos]
    if analizaVX2Dib && valida:            valida = acV[ResV] && acX[ResX] && acDoses[ResDoses] && acDib[ResDib]
    return valida
```
`fI.noInterrupciones` = nº de pares de grupos **no nulos consecutivos** (saltando asteriscos) con signo distinto (ESPEC_02, `FiltroInterrupciones.cs:233-337,1066-1069`).
**Ejemplo (C\*)** con sub-grupos `A={1,2,3}`, `B={4,5,6}`, `C={7,8,9}`: subcolumnas `1X2`, `112`, `X1X` (en octal `124`, `144`, `242`). (x,2,variantes,interrupciones) = A (1,1,2,2), B (0,1,1,1), C (2,0,2,2). Distintos: formatos 3, variantes {2,1}→2, X {1,0,2}→3, doses {1,0}→2, dibujos {(1,1),(0,1),(2,0)}→3, interrupciones {2,1}→2. Con `AcV="1-2"`, `AcX="3"`, `AcDoses="1-2"`, `AcDib="3"`, `AcInt="2"`, `AcFormatos="3"` ⇒ **válida**.

**Carga (`FDiferenciasData.cs:24-96`).** Un hijo `grupoDiferencias` por `Diferencia`; sus hijos `partidos@valores` (P5b sin longitud ⇒ `NumeroPartidos+1`) y un `aciertos` con seis atributos tratados con P5b **con longitud `nº de partidos-hijo leídos hasta ese momento + 1`** (el escritor emite los `partidos` antes que `aciertos`, `:134-146`). Reglas: `analizaVX2Dib = !(V,X,2,Dib todos nulos)`; si `aciertosInt`/`aciertosFormatos` es `""` ⇒ el array se rellena a `true` (longitud `N+1`) y la bandera correspondiente es `false`; los `AcV/AcX/AcDoses/AcDib` nulos se rellenan a `true`. Escritura: `:126-184` (campos desactivados ⇒ `""`).

`AnalizarFallos(long)` (`:120-203`) es público pero **sin llamadores** (`FiltroDiferencias` usa los contadores de `Analizar`): reasigna los arrays, ignora las banderas `Analiza*` y formatea `"Fallo en cantidad de Variantes<n>)#"` (**falta el `(`**), `…Equis…`, `…Doses…`, `…Dibujos…`, `…Formatos…`, `…Interrupciones…`. Portar solo por paridad.

**Peculiaridades a preservar.**
1. Las medidas son **recuentos de valores distintos entre sub-grupos**, no valores absolutos: `ResX≥1` si hay algún sub-grupo.
2. `formatos` compara el `long` compactado completo (incluye ceros ⇒ misma forma, mismas posiciones).
3. Se **limpia in situ** (`Array.Clear`) en `Analizar` (`:44-49`) pero se **reasigna** en `AnalizarFallos`.
4. Índices: `acX[ResX]` etc. caben porque la carga dimensiona con `nº sub-grupos+1`; si el XML pone `aciertos` **antes** de los `partidos` la longitud sale 1 y se indexa fuera de rango.
5. `Utils.IgnorarPartidos` ignora el índice 0 del array (P3) y no valida `sel` nulo (`valores=""` ⇒ `nil` ⇒ fallo).
6. `FiltroNoVariantes.AnalizaColumna(long)` y `FiltroInterrupciones.AnalizaColumna(long)` se reutilizan: cualquier cambio en esas máscaras afecta aquí.

**Estado global.** `NumeroPartidos` (indirecto: P5b y los filtros internos).
**Trampas Swift.** B5, `Set<Int64>` para `formatos`; array 17×17 plano; no ordenar nada.
**Casos golden propuestos.** Df1: ejemplo anterior. Df2: sub-grupos de tamaños distintos y solapados. Df3: solo VX2Dib (`aciertosInt=""`, `aciertosFormatos=""`). Df4: solo formatos. Df5: columnas con asteriscos. Df6: `AnalizarFallos` de `FiltroDiferencias` con fallo de Formatos y de otro apartado (texto y particiones).
**Evidencia.** `Diferencia.cs:8-28,30-119,120-203,205-282`; `FDiferenciasData.cs:24-96,100-110,126-184`; `UtilColumnas.cs:429-444`; `UtilidadesEntradasValores.cs:255-302`; `FiltroInterrupciones.cs:233-337`.

---

## 14. FiltroGruposEquipos — `…/FiltroGruposEquipos.cs`

**Propósito.** Cada **grupo de equipos** marca los partidos en los que juega un equipo (local, visitante, ambos o ninguno) y se evalúan **puntos, victorias, empates y derrotas** que sumaría ese equipo con los signos de la columna; luego las **relaciones** (ESPEC_04) pueden restringir sumas entre varios grupos.

**Tipos y datos.** `gEquipos:[GrupoEquipos]` (`:33`), `controlRelaciones1:ControladorRelacionesGE1` (`:34`; constructor: crea ambos y enlaza `controlRelaciones1.GruposEquipos = gEquipos`, `:36-41`).

**Entradas / salidas.** `Analizar(long)`, `AnalizarFallos(long)`; configuración `GruposEquipos` (lista) y `RelacionesGE1`; salida: contadores de cada `GrupoEquipos`.

**Algoritmo.**
```
analizar(col):                                                     // :43-54
    for ge in gEquipos: if !ge.analizar(col) { return false }      // los grupos posteriores conservan contadores obsoletos
    return controlRelaciones1.analiza()                            // ESPEC_04: relaciones sobre ge.noVictorias… del paso anterior
```
`AnalizarFallos` (`:56-77`): numera los grupos desde 1, concatena `ge.analizar(col, n)` de **todos** (sin cortar) y `controlRelaciones1.analizaRelaciones()`; quita el último carácter y parte por `#`; sin texto ⇒ `nil`. Nota: `AnalizaRelaciones()` asigna `txt = …` en cada vuelta y devuelve **solo el texto de la última relación** (`ControladorRelacionesGE1.cs:48-57`).
`set GruposEquipos(lista)` (`:85-105`): reemplaza la lista, **enlaza** `controlRelaciones1.GruposEquipos = lista` y fija `contieneDatos = lista.count > 0`.
**Estado global.** `AnalizarGruposEquipos` (`:160`).

**Peculiaridades a preservar.**
1. **Orden de carga:** `RelacionGE1.Controlador` captura la lista de grupos **en el momento de asignar `Relaciones`** (`RelacionGE1.cs:164-171`). El cargador asigna primero `GruposEquipos` y luego `Relaciones` (`FGruposEquiposData.cs:60-61`). Si se invierte el orden, las relaciones quedan apuntando a la lista antigua. En Swift: listas de **clases** compartidas por referencia y mantener ese orden.
2. Sin grupos y sin relaciones ⇒ `true`.
**Golden.** Ver §15 (+ relaciones en ESPEC_04). **Evidencia.** `FiltroGruposEquipos.cs:33-77,85-105,158-161`; `ControladorRelacionesGE1.cs:36-57,59-72`; `RelacionGE1.cs:164-171`; `FGruposEquiposData.cs:46-61`.

---

## 15. GrupoEquipos — `…/GrupoEquipos.cs`

**Propósito.** Un grupo de equipos y sus cuatro restricciones.

**Tipos y datos.**

| C# | Swift | Notas |
|----|-------|-------|
| `pronosticos : char[]` (`:28`) | `[Character]` | por partido: `'1'` equipo juega de **local**, `'2'` de **visitante**, `'3'` ambos, `'0'` ninguno, `'*'` sin dato (`GrupoEquipos.cs:301-325`; comentario de ayuda del escritor `FGruposEquiposData.cs:143-147`) |
| `longPronosticos : long` (`:29`) | `Int64` | **campo** que lee `AnalizaColumna`; se recalcula **solo** con el getter `LongPronosticos` / `CalcularLongPronosticos()` |
| `noSumaPuntos, noVictorias, noEmpates, noDerrotas : int` (`:32-35`) | `Int` | contadores |
| `strSumaPuntos/Victorias/Empates/Derrotas : string = ""` y `sumaPuntos/victorias/empates/derrotas : bool[]` (`:37-45`) | `String`, `[Bool]?` | P4; el setter pone el array a `nil` si el texto es `""` |

**Entradas / salidas.** `Analizar(long)→Bool`, `Analizar(long,Int)→String`; configuración `Pronosticos`, `SumaPuntos/Victorias/Empates/Derrotas` (texto P4); salida `NoSumaPuntos/NoVictorias/NoEmpates/NoDerrotas`.

**Algoritmo.**
```
longPronosticos(getter):                                           // :303-325
    r = 0
    for i = pronosticos.count-1 downto 0:  r <<= 3 ; r |= codigo(pronosticos[i])     // "*213***0".indexOf: '*'→0, '2'→1, '1'→2, '3'→3, '0'→7 ; otro → 0xFFFFFFFF (B2)
    longPronosticos = r ; return r                                 // ¡escribe el campo!  (partido 1 en los 3 bits bajos)

analizar(col):                                                     // :47-52,75-149
    pts = vic = emp = der = 0 ; lp = longPronosticos(campo)
    while col != 0:
        c = lp & 7                                                 // 1 = visitante, 2 = local, 3 = ambos, 7 = ninguno, 0 = sin dato
        if c != 7:
            switch col & 7:
              4 ('1'):  c==1 → der++ ;  c==2 → pts+=3, vic++ ;  c==3 → pts+=3, vic++, der++
              1 ('2'):  c==1 → vic++, pts+=3 ;  c==2 → der++ ;  c==3 → pts+=3, vic++, der++
              2 ('X'):  c==1 || c==2 → pts+=1, emp++ ;  c==3 → pts+=2, emp+=2
        col >>= 3 ; lp >>= 3
    return cumple()

cumple():                                                          // :160-192 ; cada test solo si su texto != ""
    ok = true
    if strSumaPuntos != "": ok = sumaPuntos.count >= pts+1 && sumaPuntos[pts]
    if ok && strVictorias != "": ok = victorias.count >= vic+1 && victorias[vic]
    if ok && strEmpates  != "": ok = empates.count   >= emp+1 && empates[emp]
    if ok && strDerrotas != "": ok = derrotas.count  >= der+1 && derrotas[der]
```
`analizar(col, n) → String` (`:58-63,194-218`): mismos contadores; para cada restricción activa que falle añade `"Fallo en Suma de Puntos del grupo <n>.  (<pts>)#"`, `"Fallo en Nº de Victorias del grupo <n>.  (<vic>)#"`, `"Fallo en Nº de Empates del grupo <n>.  (<emp>)#"`, `"Fallo en Nº de Derrotas del grupo <n>.  (<der>)#"` (a diferencia de `cumple`, **no** corta en el primer fallo).
Contadores públicos `NoSumaPuntos/NoVictorias/NoEmpates/NoDerrotas` (`:327-345`), leídos por `RelacionGE1.AnalizaSumas` (`RelacionGE1.cs:70-84`).

**Ejemplo (C\*)** con pronósticos `"1123"+"0"+"*"*9` (p1 `'1'` local, p2 `'1'`, p3 `'2'` visitante, p4 `'3'` ambos, p5 `'0'`): p1 signo `1` y local ⇒ +3, V; p2 signo `X`, local ⇒ +1, E; p3 signo `2`, visitante ⇒ +3, V; p4 signo `1`, ambos ⇒ +3, V, D; p5 se salta. Resultado **pts=10, V=3, E=1, D=1**.

**Peculiaridades a preservar.**
1. `AnalizaColumna` usa el **campo** `longPronosticos`, no el getter. El campo sólo se actualiza si alguien llama a `LongPronosticos`/`CalcularLongPronosticos()`: lo hacen `FGruposEquiposData.cs:125` y `GruposEquiposFrmViewModel.cs:406,523,955` tras fijar `Pronosticos`. En Swift, recalcular `longPronosticos` al asignar `pronosticos` (equivalente en todos los flujos reales); `Pronosticos` fijado sin recalcular ⇒ 0.
2. `pronosticos` por defecto es `char[N]` de `'\0'` ⇒ código −1 ⇒ B2 (basura); siempre se sustituye al cargar.
3. El bucle termina al agotar `col` (los partidos por encima del último no nulo no se miran); `'0'`(=7) salta el partido; `'*'`(=0) no cuenta (ningún `if` casa).
4. Comentarios contradictorios en el C# (`001=visitante` vs `1=eq. casa`): **manda el código** (tabla anterior).
5. Un grupo con puntos y partido 'ambos' puede sumar victoria y derrota en el mismo partido.
6. `CalcularLongPronosticos` además guarda `longPronosticosTemp`, que nadie lee (`:30,53-56`).
7. Fichero: `grupoEquipos` con hijos `equipos@value` (cadena de signos de equipo), `victorias|empates|derrotas|sumaPuntos @value` (`FGruposEquiposData.cs:97-126`).

**Estado global.** `NumeroPartidos` (`:28`).
**Trampas Swift.** B2, B5, enteros (sin `Float`). **Golden.** G-E1: ejemplo anterior; G-E2: todas las combinaciones de `c∈{0,1,2,3,7,*}` × `{1,X,2}`; G-E3: restricciones con rangos `"0-5,7"`; G-E4: `AnalizarFallos` con 4 fallos; G-E5: longitud de `pronosticos` ≠ N.
**Evidencia.** `GrupoEquipos.cs:28-63,75-149,160-218,220-294,301-345`; `RelacionGE1.cs:70-84`; `FGruposEquiposData.cs:97-126`.

---

## 16. CalculadorEstadisticas y Estadistica — `…/Estadisticas/CalculadorEstadisticas.cs`, `Estadistica.cs`

**Propósito.** Mide qué **porcentaje** de las columnas de cada fichero de una carpeta (históricos ganadores) cumple un filtro cualquiera.

**Tipos y datos.** `Estadistica { archivo:String = "", cumplimiento:Float }` (`Estadistica.cs:6-18`; **`float`**, T3).

**Algoritmo.**
```
estadisticasFiltro(filtro, directorio) → [Estadistica]:            // CalculadorEstadisticas.cs:37-52
    for f in directorio.GetFiles("*.txt"), j = 0…:
        e.archivo = f.nombre ; e.cumplimiento = obtener(filtro, directorio + f.nombre)     // concatenación textual: el directorio lleva separador final

obtener(filtro, ruta):                                             // :11-36
    total = 0 ; validas = 0
    for cada línea de ruta:  col = convStrToLong(línea.trim().uppercased()) ; total += 1 ; if filtro.analizar(col) { validas += 1 }
    if total == 0 { return 0 }
    c: Float = Float(validas) / Float(total)                       // división en 32 bits
    return c * 100                                                 // 32 bits
```
La lectura es `ArchivoColumnasTexto.SiguienteColumna/LeeColumnaSinComas` (`ArchivoColumnasTexto.cs:183-217`): `Peek()>=0` ⇒ hay más; `ReadLine().Trim().ToUpper()` (**no** quita comas pese al nombre). **Ejemplos float:** 1/3 ⇒ `33.333336`, 2/3 ⇒ `66.66667`, 7/9 ⇒ `77.77778` (ref. NumPy `float32`).

**Estado global.** Ninguno propio (el filtro lee el suyo).
**Peculiaridades a preservar.**
1. **Una línea en blanco cuenta como columna** (`ConvStrToLong("")=0`) y entra en `total`.
2. La estadística llama a `filtro.Analizar` **sin mirar `IsActive`**: se usa el filtro temporal de cada pantalla (`DistanciasFrmViewModel.cs:215-224` y análogas).
3. `ConvStrToLong` con caracteres fuera de `2X1` (p. ej. comas, `*`) ⇒ B3 (basura con signo extendido).
4. El orden de `GetFiles` y su patrón no están garantizados en macOS (Abiertas A3); `FileInfo.ToString()` se usa como nombre (Abiertas A3).
5. No se cierra el `ArchivoColumnasTexto` (fuga de manejadores en C#; en Swift cerrar siempre).
**Trampas Swift.** T3: `Float` (no `Double`), dos pasos; T8: leer aceptando `\r\n`/`\n`; T12: bucle largo ⇒ `async` con cancelación.
**Golden.** Es1: carpeta de 3 ficheros (14 columnas, vacío, con línea en blanco) con `FiltroNoVariantes` ya configurado; salida `[nombre, cumplimiento]` a nivel de bit (`Float.bitPattern`).
**Evidencia.** `CalculadorEstadisticas.cs:11-52`; `Estadistica.cs:6-18`; `ArchivoColumnasTexto.cs:183-217`; `UtilColumnas.cs:196-204`.

---

## 17. Estadisticas.Dibujos — `…/Estadisticas/Dibujos.cs` (renombrar `DibujosEstadistica`)

**Propósito.** Cuenta las `X` y los `2` de una columna **de texto** (modo «Dibujos» de Anastatics).
**Tipos.** Entrada `columna:String` ≥14 chars; salida `[Int]` de 5 posiciones (`ind[3]=nx`, `ind[4]=n2`, resto 0).
**Algoritmo.** `for nr in 0..<14: ch=columna[nr]; ch=='1' → nada; ch=='2' → n2++; otro → nx++` (`Dibujos.cs:22-35`). Cualquier carácter que no sea `1` ni `2` cuenta como `X` (incluida `x` minúscula).
**Estado global.** Ninguno (sin estado de instancia). **Peculiaridades.** Cadena <14 ⇒ `IndexOutOfRange`; solo mira los 14 primeros.
**Golden.** G1..G9 (ref.: ver tabla §0.1 columna «Dibujo»).
**Evidencia.** `Dibujos.cs:21-37`; uso `AnastaticsViewModel.cs:226-235`.

## 18. DibRepes — `…/Estadisticas/DibRepes.cs`

**Propósito.** Compara cada columna con la **anterior** (coincidencias posición a posición) y cuenta 1/X/2.
**Tipos y datos.** Estado de instancia `old:String = "00000000000000"` (14 ceros); salida `[Int]` de 5 (`ind[0]=np`, `ind[2]=n1`, `ind[3]=nx`, `ind[4]=n2`, `ind[1]=0`). Swift: `struct` con `mutating func procesar`.
**Algoritmo.**
```
procesar(col):                                                     // DibRepes.cs:24-38
    np = n1 = nx = n2 = 0
    for nr in 0..<14: ch = col[nr] ; if ch == old[nr] { np += 1 } ; ch=='1' → n1++ ; ch=='2' → n2++ ; otro → nx++
    old = col ; return [np, 0, n1, nx, n2]
```
**Peculiaridades.** La 1.ª columna se compara con ceros (`np=0`); la comparación es **exacta de carácter** (`x` ≠ `X`); una nueva instancia por pasada (`AnastaticsViewModel.cs:241`).
**Golden.** Secuencia G1,G2,G3 ⇒ `[0,0,14,0,0]`, `[0,0,0,14,0]`, `[0,0,0,0,14]`; y secuencias repetidas.
**Evidencia.** `DibRepes.cs:21-40`.

## 19. StaInter — `…/Estadisticas/StaInter.cs`

**Propósito.** Cuenta los **cambios de signo** consecutivos de una columna de texto y los clasifica.
**Algoritmo** (`StaInter.cs:22-39`):
```
ch0 = col[0] ; ng = nv = n1 = nx = n2 = 0
for nr in 1..<14:
    ch = col[nr] ; if ch == ch0 { continue }
    if ch0=='1' { n1++ } else if ch0=='2' { n2++ } else { nx++ }     // clasifica por el signo que se abandona
    ng += 1
    if ch=='1' { nv += 1 }                                           // cambios cuyo destino es '1'
    ch0 = ch
return [ng, nv, n1, nx, n2]
```
**Peculiaridades.** Solo 14 posiciones; carácter distinto de `1`/`2` ⇒ `X`. **Golden.** Tabla §0.1 «StaInter» (G1..G9; p. ej. G5 ⇒ `[10,3,4,3,3]`).
**Evidencia.** `StaInter.cs:21-41`; agregación `AnastaticsViewModel.cs:255-266`.

## 20. StaSigSeg — `…/Estadisticas/StaSigSeg.cs`

**Propósito.** Longitud de las **rachas máximas** de signos seguidos: de `1`, de `X`, de `2` y de «variantes» (X o 2 seguidos, mezclados).
**Algoritmo** fiel (máquina de estados; `StaSigSeg.cs:22-75`):
```
m1=mx=m2=mv=0 ; n1=nx=n2=nv=0 ; ch0=col[0]
if ch0=='1' n1++ else if ch0=='2' n2++ else nx++ ; if ch0 != '1' nv++
for nr in 1..<14:
    ch = col[nr]
    if ch == ch0:  suma 1 al contador de ch (n1/n2/nx) ; if ch != '1' { nv += 1 }
    else if ch0=='1': m1=max(m1,n1) ; n1=0 ; nv=1 ; if ch=='2' n2=1 else nx=1
    else if ch0=='2': m2=max(m2,n2) ; n2=0 ; if ch=='1' { mv=max(mv,nv) ; n1=1 ; nv=0 } else { nx=1 ; nv += 1 }
    else:             mx=max(mx,nx) ; nx=0 ; if ch=='2' { n2=1 ; nv += 1 } else { mv=max(mv,nv) ; n1=1 ; nv=0 }
    ch0 = ch
cierre: m1=max(m1,n1) ; mx=max(mx,nx) ; m2=max(m2,n2) ; mv=max(mv,nv)
return [0, mv, m1, mx, m2]
```
Equivale a: `m1`/`mx`/`m2` = mayor racha de `1`/`X`/`2` consecutivos; `mv` = mayor racha de caracteres **distintos de `1`** consecutivos (comprobado por prueba diferencial con 300 000 columnas aleatorias, no versionada; el port debe seguir la máquina de estados literal y verificarse contra esa definición).
**Peculiaridades.** Cualquier carácter no `1`/`2` se trata como `X`. **Golden.** Tabla §0.1 «StaSigSeg» (G5 ⇒ `[mv,m1,mx,m2]=[3,2,1,2]`).
**Evidencia.** `StaSigSeg.cs:21-77`.

### 20.1 Agregación de resultados (uso, fuera de las clases pero necesaria para reproducir la pantalla)

Matriz `rsl[15][15]` de enteros. Validación previa `VerColumna`: exactamente 14 caracteres de `12xX`, `x`→`X` (`statistics.cs:149-158`; WinUI `AnastaticsViewModel.cs:284-285`); la lectura corta a 500 000 columnas (`statistics.cs:140`, WinUI `:107`). Según el modo (`statistics.cs:71-121`; WinUI `AnastaticsViewModel.cs:222-283`):

| Modo | Acumulación por columna (`ind` = salida del procesador) |
|------|-----------------------------------------------------------|
| Dibujos | `rsl[ind[3]][ind[4]]++` |
| DibRepes | `rsl[0][ind[0]]++`; `if num1==ind[2] { rsl[1][ind[2]]++ }`; `if numx==ind[3] { rsl[2][ind[3]]++ }`; `if num2==ind[4] { rsl[3][ind[4]]++ }`; `if numx+num2 == ind[3]+ind[4] { rsl[4][numx+num2]++ }`; después `num1,numx,num2 = ind[2],ind[3],ind[4]` (inician a 0, se comparan con la columna **anterior**) |
| StaInter | `rsl[0][ind[0]]++; rsl[1][ind[2]]++; rsl[2][ind[3]]++; rsl[3][ind[4]]++; rsl[4][ind[1]]++` |
| StaSigSeg | `rsl[0][ind[2]]++; rsl[1][ind[3]]++; rsl[2][ind[4]]++; rsl[3][ind[1]]++` |

---

## 21. Abiertas — estado y decisión de cada entrada en [`DECISIONES.md`](DECISIONES.md) §3

- **A1 · Cultura de `double`.** `RangosOpciones.PonerRangos` (`RangosOpciones.cs:44-50`), `FFormatos123Data.cs:42` (lee) y `:188` (escribe), `FValoracionData.cs:50-52,92-94` y los mensajes de `FiltroValoracionSignos.AnalizarFallos` (`:174-180`) dependen de la cultura del sistema; `FValoracionData` asume coma decimal (`Replace(".",",")`) al leer y escribe con `.` (`Replace(",",".")`). Con cultura de punto decimal el valor se leería multiplicado. Decisión pendiente (ESPEC_07/dueño, R2): qué locale fija el port Swift para ficheros (propuesta: aceptar ambos separadores al leer) y para los textos de fallo.
- **A2 · `aciertosMax` de `FiltroFormatos123` no se persiste** (`FFormatos123Data.cs:18-68`): tras cargar de fichero vale 0 y el paso libre satura `ag` en 1. ¿Replicar (R1, propuesto) o derivar de `ArrayAciertos` al cargar? Cambia el resultado de los `.comb` existentes.
- **A3 · `DirectoryInfo.GetFiles("*.txt")`.** Orden de enumeración (en macOS no está garantizado) y semántica del patrón; y `FileInfo.ToString()` como nombre de fichero (`CalculadorEstadisticas.cs:44-46`). No verificable sin ejecutar .NET; propuesta: ordenar por nombre (comparación ordinal insensible a mayúsculas) y exigir directorio con separador final.
- **A4 · Semántica BCL de `Convert.ToInt32/ToDouble`** (espacios, signo, millares): aplicable a todos los `Set*`/cargadores; fijarla con golden de parseo (`" 3"`, `"+3"`, `"3 "`, `"3,5"`).
- **A5 · `FiltroDiferencias` + tolerancias.** `ObtenNoAciertosTolerancias` lanza (`FiltroDiferencias.cs:124-127`) y `Grupo` no atrapa (`Grupo.cs:348,391`). ¿La UI impide esa combinación? Si no, la excepción sube hasta el llamador; confirmar y decidir si el port replica el fallo.
- **A6 · `FiltroDibujos` construido por If-Then** (`CondicionIfThen.cs:121-123`) deja `dibs` vacío ⇒ rechaza siempre. Deducido por lectura; confirmar con un golden de `CondicionIfThen` y decidir (R1: replicar).
- **A7 · Nº de partidos ≠ 14.** `FFormatos123Data` fija 14 (`:25,71-72`); `Diferencia`/`FiltroDibujos` usan arrays de 17; `FiltroNoVariantes` usa 15 grupos para `2`. ¿Se soportan 15/16 partidos (Pleno al 15)? `ObtenNumPartidos` lee `Convert.ToInt16` (`AConfiguracion.cs:149`) sin tope visible.
- **A8 · `Diferencia.AnalizarFallos(long)`** no tiene llamadores (grep en Domain, WinUI y legacy). ¿Se porta?
- **A9 · Textos de fallo.** `FiltroDiferencias.AnalizarFallos` puede acabar con un elemento `""` (cuando termina en `#`) y la pantalla hace `Substring(0,1)` (`AnalizarCombinacionFrmViewModel.cs:261`; legacy `AnalisisCombinacion.cs:117,134`); confirmar el efecto en pantalla. Igual para el mensaje «Columna Fuera de Límites» que aparece aunque `Analizar` sea `true` (§3).
- **A10 · Líneas no válidas en históricos** (`ConvStrToLong` con caracteres ajenos y B3): la estadística no filtra líneas erróneas ni en blanco (§16).
- **A11 · `RelacionGE1`/`AnalizaRelaciones`** (sólo el último texto de relación) se especifican en ESPEC_04; aquí solo se enlaza.
- **A12 · `Char.IsLetter`** (Unicode, cualquier categoría de letra) vs `Character.isLetter` de Swift para la letra de tolerancia; en la práctica A-Z. Confirmar con `ToleranciaFiltros.LetrasTol` (ESPEC_01).
- **A13 · Cobertura de tests existente.** Para estos componentes sólo hay `ManualFuncionalTests.cs:28-45` (NoVariantes, 3 casos) y `RangosHelperBoolTests.cs`; el resto de «ref.» de este documento proviene de un port Python externo no versionado: **GoldenGen es la única fuente válida** de los valores esperados.
