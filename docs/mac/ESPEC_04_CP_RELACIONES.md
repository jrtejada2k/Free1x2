# ESPEC 04 — Columnas probables, control de fallos, relaciones CP1/CP2/CP3/GE1 e impresión

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Plantilla de
> [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md) §2; trampas T1-T12 de su §3 (aquí se
> concretan como S1-S12 en §0.7). El C# es el oráculo (M-R1).
>
> **Abreviaturas de ruta** (todas relativas a la raíz del repo):
> `MC` = `Free1X2.Domain/MotorCalculo` · `ES` = `Free1X2.Domain/EntradaSalida` ·
> `UT` = `Free1X2.Domain/Utils` · `GCP` = `Free1X2.Domain/EntradaSalida/GenerarCPs` ·
> `TST` = `Free1X2.Domain.Tests` · `WUI` = `Free1X2.WinUI/Views/Ported`.
>
> **Origen de los valores esperados** en los casos golden: `TST` = golden vigente en el repo
> (autoritativo); `SIM` = cálculo por simulación Python independiente escrita desde este
> pseudocódigo (que además reproduce exactamente las dos cadenas de validez de
> `TST/RelacionCP1GoldenTests.cs:140,158`); aún **no** ejecutada contra el C#, debe ratificarse
> con GoldenGen; `CALC` = aritmética directa.

## Índice de componentes (16)

| # | Componente | Fichero C# | Sección |
|---|-----------|------------|---------|
| 1 | `ColumnaProbable` | `MC/ColumnaProbable.cs` (750 l.) | §1 |
| 2 | `FiltroColProbables` | `MC/FiltroColProbables.cs` | §2 |
| 3 | `CPControlFallos` | `MC/CPControlFallos.cs` | §3 |
| 4 | `ControladorCPControlFallos` | `MC/ControladorCPControlFallos.cs` | §4 |
| 5 | `RelacionCP1` | `MC/RelacionCP1.cs` | §5 |
| 6 | `ControladorRelacionesCP1` | `MC/ControladorRelacionesCP1.cs` | §6 |
| 7 | `RelacionCP2` | `MC/RelacionCP2.cs` | §7 |
| 8 | `ControladorRelacionesCP2` | `MC/ControladorRelacionesCP2.cs` | §8 |
| 9 | `RelacionCP3` (+ `EscaleraAciertos`, `SandwichAciertos`, `AgrupacionColumnas`) | `MC/RelacionCP3.cs` y 3 ficheros auxiliares | §9 |
| 10 | `ControladorRelacionesCP3` | `MC/ControladorRelacionesCP3.cs` | §10 |
| 11 | `RelacionGE1` | `MC/RelacionGE1.cs` | §11 |
| 12 | `ControladorRelacionesGE1` | `MC/ControladorRelacionesGE1.cs` | §12 |
| 13 | `ControladorImpresion` | `MC/ControladorImpresion.cs` | §13 |
| 14 | `ControladoresImpresion` | `MC/ControladoresImpresion.cs` | §14 |
| 15 | `FColProbablesData` (carga/guardado XML de la condición CP) | `ES/FColProbablesData.cs` | §15 |
| 16 | Generador de CPs (`CPs`, `Valoracion`, `IO`, `DatosHelper`, `ColumnasProbables` DataSet) | `GCP/*` | §16 |

---

## 0. Visión de conjunto

### 0.1 Qué es una columna probable (CP) — definición con ejemplo

Una CP es un **segundo pronóstico** por partido (`∅`, `1`, `X`, `2`, `1X`, `12`, `X2`, `1X2`; mismos
3 bits que el boleto, `∅` = byte 0 = «partido ignorado») con un **presupuesto de condiciones**. Para
una columna candidata (`Int64`, 3 bits/partido, partido 1 en los bits 0-2) la CP calcula cuatro
magnitudes sobre los `N` partidos (`N = VariablesGlobales.NumeroPartidos`, 14 por defecto):

| Magnitud | Significado |
|----------|-------------|
| `noAC` | partidos con pronóstico ≠ ∅ cuya máscara intersecta el signo de la columna |
| `noACS` | racha máxima de aciertos consecutivos |
| `noFS` | racha máxima de fallos consecutivos |
| `noPuntos` | suma por acierto: fijo (pron. de 1 signo) / doble (6,5,3) / triple (7) |

La CP es válida si `AC[noAC] ∧ ACS[noACS] ∧ FS[noFS]` (∧ `puntos[noPuntos]` si hay rango de puntos),
con una variante de **tolerancias locales** (§1).

**Ejemplo trabajado** (SIM): CP con pronóstico `p1=1, p2=∅, p3=1, p4=X, p5=1X2, p6..p14=∅`, columna
`1 2 1 1 X 1 1 1 1 1 1 1 1 1`:

```
p1 '1' vs 1  -> acierto  (noAC=1, racha A=1)           puntos += fijo
p2  ∅        -> IGNORADO (no rompe ni reinicia rachas)
p3 '1' vs 1  -> acierto  (noAC=2, racha A=2 -> noACS=2) puntos += fijo
p4 'X' vs 1  -> fallo    (racha F=1 -> noFS=1)
p5 '1X2' vs X-> acierto  (noAC=3)                       puntos += triple
=> noAC=3, noACS=2, noFS=1, noPuntos = 6+6+1 = 13  (puntos 6/3/1)   ó  10+10+20 = 40  (10/15/20)
```

### 0.2 Modelo de objetos y cableado (contrato de referencias)

```
FiltroColProbables (IFiltro)                 MC/FiltroColProbables.cs:24-52
 ├─ colProbables       : [ColumnaProbable]   (instancias COMPARTIDAS por referencia)
 ├─ controlFallosCP    : ControladorCPControlFallos ── [CPControlFallos]      -> lee colProbables
 ├─ controlRelaciones1 : ControladorRelacionesCP1   ── [RelacionCP1]          -> lee colProbables
 ├─ controlRelaciones2 : ControladorRelacionesCP2   ── [RelacionCP2]          -> lee colProbables
 └─ controlRelaciones3 : ControladorRelacionesCP3   ── [RelacionCP3]          -> cada una guarda SU lista `columnas`
```

Las relaciones **no copian** las CPs: leen `noAC/noACS/noFS` de las mismas instancias que el filtro
acaba de evaluar. En Swift `ColumnaProbable` debe ser `final class` (S3).

**Las referencias a la lista de CPs se capturan en el momento de la asignación** (no hay
indirección): `RelacionCP1.Controlador` (`MC/RelacionCP1.cs:490-497`), `CPControlFallos.Controlador`
(`MC/CPControlFallos.cs:217-224`) y `RelacionGE1.Controlador` (`MC/RelacionGE1.cs:164-171`) copian
`controlador.ColumnasProbables`/`GruposEquipos` al asignarse. Por eso el orden de montaje es parte
del contrato (P24), y es el que siguen el cargador XML (`ES/FColProbablesData.cs:87-93`) y la UI
(`Free1X2/UI/Filtros/ColProbablesFrm.cs:438`, `1072`, `1273-1274`, `3700-3706`; `WUI/ColProbablesFrmViewModel.cs:598-606`):

1. `filtro.ColProbables = lista` (fija además `contieneDatos = lista.Count > 0`, `MC/FiltroColProbables.cs:159-181`);
2. `RelacionesCP1.Relaciones = …` (asigna `Controlador` a cada una);
3. `RelacionesCP2.Relaciones2 = …` y `RelacionesCP2.ColumnasProbables = lista`;
4. `RelacionesCP3.Relaciones = …` (cada `RelacionCP3` ya trae su `columnas`);
5. `ControlFallosCP.ControlesFallos = …` (asigna `Controlador`) y después `FallosPermitidos`.

**Recomendación Swift:** que cada relación resuelva las CPs a través del filtro en cada evaluación
(`unowned let filtro`) produce resultados idénticos **siempre que se respete ese orden**; si no se
respeta, el C# usa listas obsoletas (comportamiento a no replicar; fuera de golden).

### 0.3 Orden de evaluación por columna

`Grupo.AnalizaColumna` recorre los filtros en orden fijo y llama a `filtro.Analizar(col)` solo si
`IsActive` (`MC/Grupo.cs:169-190`); con análisis (`AnalisisActivo`) lo llama también inactivo
(`MC/Grupo.cs:230-262`). Dentro de `FiltroColProbables.Analizar` (`MC/FiltroColProbables.cs:54-92`):

```
1. para cada CP en orden: si !cp.Analizar(col) -> return false          (corta; las CPs siguientes NO se recalculan)
2. si !controlFallosCP.Analiza()        -> false
3. si !controlRelaciones1.Analiza()     -> false
4. si Relaciones2.Count > 0: AND de todas (corta en la 1ª falsa)
5. si Relaciones3.Count > 0: return controlRelaciones3.AnalizaRelaciones()   // último; su resultado es el final
6. return true
```

`Grupo` reutiliza el veredicto en memoria (`reCalcularGrupo`/`grupoValidoMemoria`,
`MC/Grupo.cs:41-42,169-221`); por tanto `NoAC/NoACS/NoFS` reflejan la **última columna realmente
evaluada**. El `ContenedorAnalisis` los lee tras evaluar (`Free1X2.Domain/Analisis/ContenedorAnalisis.cs:271-283`)
y `Grupo.SonTolGrupoValidas*` llama a `ObtenNoAciertosTolerancias(letras)` tras `Analizar`
(`MC/Grupo.cs:348,391`). Todo el estado es mutable por instancia: **no reentrante**, un filtro por tarea.

### 0.4 Parámetro de puntos CP

`ColumnaProbable` nace con puntos fijo/doble/triple = **6/3/1** (`MC/ColumnaProbable.cs:50-52`). Se
sobrescriben en cada cálculo: `Analizador.InicializaParametros` (`MC/Analizador.cs:272,320,424-435`)
→ `Grupo.ActualizaParametrosFiltros` → `Grupo.InicializaPuntosCP` (`MC/Grupo.cs:140-157`) →
`FiltroColProbables.InicializaPuntosCP` (`MC/FiltroColProbables.cs:150-157`), con los valores del
nodo `<CP><puntos fijos= dobles= triples=/></CP>` de `parametros.free1x2`
(`ES/AConfiguracion.cs:58-67`; fichero distribuido: 6/3/1, `Free1X2.WinUI/Datos/parametros.free1x2:4-6`).
`VariablesGlobales.PuntosFijos/Dobles/Triples` (10/15/20 por defecto, `Free1X2.Domain/VariablesGlobales.cs:153-155`)
**no** los lee la CP. Solo afectan a las CPs existentes en ese momento (las creadas después usan 6/3/1 hasta el siguiente cálculo).

### 0.5 Dependencias mínimas (contratos replicados; fuente canónica ESPEC_06)

**D1 · Pronóstico ↔ byte** (`UT/UtilColumnas.cs:94-132`). `ConvPartidoStrToByte`: `"1"→4, "X"→2, "2"→1`,
`"1X"|"1,X"→6`, `"12"|"1,2"→5`, `"X2"|"X,2"→3`, `"1X2"|"1,X,2"→7`, **cualquier otra cadena (incluidas `""`,
`" "`, `null`, `"X1"`, `"21"`) → 0**. `ConvPartidoByteToStr` (`UT/UtilColumnas.cs:134-167`): 4→`"1"`, 2→`"X"`,
1→`"2"`, 6→`"1X"`, 5→`"12"`, 3→`"X2"`, 7→`"1X2"`, otro→`""`.

**D2 · `RangosHelper.ObtenBoolArray(s)`** (`UT/RangosHelper.cs:76-143`): `s.Split(',')`; `max` = mayor entero
de todos los tokens (también los extremos de rangos `a-b`); devuelve `bool[max+2]` con `true` en cada
valor y en cada `a..b`. Longitud siempre `max+2` ⇒ los dos últimos índices son `false`. Cadena vacía →
`Convert.ToInt32("")` lanza `FormatException` (los llamadores protegen con `!= ""`).
`ObtenIntArray(s)` (`UT/RangosHelper.cs:32-74`): lista de enteros en orden de escritura, rangos
expandidos `a..b` (si `b<a` no añade nada), **sin deduplicar**.

**D3 · `UtilidadesEntradasValores`** (`UT/UtilidadesEntradasValores.cs`):

- `ObtenerListaFromTxtAciertos(s)` (`:115-181`): admite `"a,b-c,d"`, `"a-b"`, `"a"`; expande rangos
  ascendentes; **deduplica correctamente** conservando primer orden de aparición; cualquier error de
  parseo (token vacío, no numérico, rango con extremo vacío como `"1-"`, **valores negativos** porque `-` es separador)
  ⇒ `catch` ⇒ **lista vacía**. Nunca lanza.
- `ObtenerListaFromTxt(s)` (`:42-108`): mismo parseo, pero **convierte a base 0** (`valor-1`) y su
  deduplicación es defectuosa: antes de añadir `v` comprueba `indices.Contains(v)` contra una lista que
  contiene valores ya restados. Resultado: `v` se **descarta si `v+1` apareció antes**, y se **duplica
  si `v` se repite**. SIM: `"2,1"→[1]`, `"3,2"→[2]`, `"1,1"→[0,0]`, `"1-3,2"→[0,1,2]`, `"1-2,2-3"→[0,1,1,2]`,
  `"1,2,3"→[0,1,2]`, `""→[]`, `"3-1"→[]`, `"1-"→[]`.
- `ObtenerBoolArrayFromTxt(s, L)` (`:255-281`): `""`→`null`; si no, `lista = ObtenerListaFromTxtAciertos(s)` ordenada;
  longitud = `(L > lista.max ? L : lista.max) + 1`, `true` en cada valor de la lista. Lista vacía por error ⇒
  `ArgumentOutOfRangeException` (`lista[lista.Count-1]`).

**D4 · `Convert.ToInt32(string)`** (BCL, no está en el repo; ver Abiertas A-6): equivale a `int.Parse` con
`NumberStyles.Integer` y cultura actual: admite espacios en blanco inicial/final y un signo `+`/`-`,
solo dígitos, sin separadores de miles; vacío/ilegible ⇒ `FormatException`; fuera de rango ⇒
`OverflowException`. Swift: helper `convertToInt32(_:) throws`, **no** `Int32(_:)` a secas (S2).

**D5 · Listas/rangos locales a CPControlFallos y RelacionCP1** — se describen donde se usan (§3, §5).

### 0.6 Catálogo de peculiaridades críticas (a preservar)

| Id | Resumen | Sección |
|----|---------|---------|
| P1 | Partidos con pronóstico 0 se saltan **sin romper ni reiniciar** rachas | §1 |
| P2 | Con tolerancias locales activas el veredicto es solo `tolLocales[nFallosTolerados]`: si `tolLocales[0]=false` se **rechaza incluso una columna que cumple todo** | §1 |
| P3 | Los puntos se comprueban antes que las tolerancias y son excluyentes | §1 |
| P4 | `esColumnaValida` evalúa AC, ACS y FS **sin cortocircuito** (fija flags) | §1 |
| P5 | Setters acumulativos; `ReinicializaValores` parcial; `Pronosticos` set no limpia el resto | §1 |
| P6 | Token de pronóstico en orden no canónico (`X1`, `21`) → byte 0 (partido ignorado) | §1, §15 |
| P7 | `FiltroColProbables` corta en la 1ª CP fallida; las siguientes conservan `NoAC` de la columna anterior | §2 |
| P8 | RelacionCP1: si ninguna CP cumple «cuántos AC», suma/recorrido se calculan sobre **todas** las CPs relacionadas | §5 |
| P9 | RelacionCP1: búferes de 15 posiciones ⇒ `noAC ≤ 14`; `cuantosAC` nulo con `cantidadCP` no vacío ⇒ NullReference | §5 |
| P10 | RelacionCP2: índices 0-based con deduplicación defectuosa (D3); las diferencias negativas nunca se pueden permitir | §7 |
| P11 | RelacionCP2: `Concepto`/`Cantidad` desconocidos ⇒ globales `true`; individuales usan diferencia 0 | §7 |
| P12 | RelacionCP3: «columnas implicadas» = las **primeras K** CPs (K = nº de índices), se ignoran los índices concretos | §9, §15 |
| P13 | RelacionCP3: `AnalizaAgrupacionesSolapadas` **nunca rechaza** | §9 |
| P14 | RelacionCP3: sándwiches — tras completar uno se **descarta el elemento siguiente**; pueden salir sándwiches de 4 valores iguales | §9 |
| P15 | RelacionCP3: la matriz solapada no multiplica por el nº de rachas | §9 |
| P16 | RelacionCP3: el camino de texto usa cachés **obsoletos**; solo `AnalizaRelaciones` llama a `ActualizaValores` | §9, §10 |
| P17 | CPControlFallos: lista de aciertos debe ir ascendente; letras repetidas cuentan doble; `ToUpper` | §3 |
| P18 | ControladorCPControlFallos: con `fallosPermitidos` no se emiten líneas «aceptada» y se evalúan todos los controles | §4 |
| P19 | Protocolo de texto de `AnalizarFallos`: prefijo `*` acierto, `!` tolerancia, resto fallo; `null` si vacío | §2 |
| P20 | ControladorRelacionesGE1.AnalizaRelaciones **sobrescribe** el texto (solo queda la última relación) | §12 |
| P21 | ControladoresImpresion: `Contains` por referencia ⇒ jamás deduplica | §14 |
| P22 | Generador: asignaciones que se sobrescriben, huecos `null`, empates por menor índice, centinela 99.99 | §16 |
| P23 | Carga XML: `ContieneDatos` acaba siendo `Count>0`; ids ignorados; nombres de nodo raíz sin distinguir mayúsculas, hijos sí | §15 |
| P24 | Referencias capturadas en el setter (orden de montaje obligatorio) | §0.2 |

### 0.7 Trampas Swift transversales (S1-S12)

| Id | Trampa | Regla |
|----|--------|-------|
| S1 | `String.split(separator:)` de Swift **omite subsecuencias vacías** por defecto; `string.Split(',')` de C# las conserva (`"1X,,,X"` → 4 tokens: `1X`, vacío, vacío, `X`). Los vacíos son significativos (partido sin pronóstico) | usar `split(separator:omittingEmptySubsequences:false)`; `""` → `[""]` en ambos |
| S2 | `Convert.ToInt32` ≠ `Int(String)` (espacios, signo, excepciones) | helper `convertToInt32` según D4 |
| S3 | Semántica de referencia: relaciones leen contadores de CPs vivas | `final class`; `[ColumnaProbable]` de referencias; nunca structs |
| S4 | C# lanza `FormatException`/`IndexOutOfRange`/`NullReference`/`Overflow`; Swift aborta | validar en *set-time* con `throws`; en evaluación solo accesos seguros donde el C# ya comprueba límites (`idx < arr.count ∧ arr[idx]`); el resto, `precondition` documentada. Decisión del dueño: Abiertas A-1 |
| S5 | `bool[]` anulable y `.Length` | `[Bool]?` y `.count` |
| S6 | `char` C# (UTF-16) con `' '` = «sin letra de tolerancia» | `[Character]` con `" "`; `Char.IsLetter` ↔ `Character.isLetter` (idénticos para A-Z) |
| S7 | `string.ToUpper()` (carácter a carácter, cultura actual) vs `uppercased()` (puede expandir `ß`→`SS`) | mapear por carácter; las letras de tolerancia son A-Z |
| S8 | `switch` de cadenas C# es ordinal (`"AC"`, `"Más"`); `==` Swift usa equivalencia canónica | iguales en la práctica (NFC); literales exactos con tilde precompuesta |
| S9 | Enteros: `int`→`Int32`, `byte`→`UInt8`, `long`→`Int64`. Ninguna suma puede desbordar (máx. `15×20 = 300` puntos) | `Int32` normal; sin `&+` |
| S10 | División entera `Count/3`, `Count/4` | `/` de `Int` trunca igual (T9) |
| S11 | Listas de índices con **duplicados significativos** (CP2) | `[Int]`, nunca `Set` |
| S12 | Mensajes con caracteres no ASCII (`Relación`, `Puntuación`, `ª`, `Más`, `Nº`) y doble espacio antes de `(` | literales exactos, byte a byte (golden `TST` los fija) |

---

## 1. `ColumnaProbable` — `MC/ColumnaProbable.cs`

**Propósito.** Evaluar una columna contra un pronóstico propio con rangos permitidos de aciertos,
aciertos seguidos, fallos seguidos y puntos, con tolerancias globales (letras) y locales (presupuesto
de fallos tolerados).

**Tipos y datos.**

| Campo C# | Swift | Invariante / valor inicial | Línea |
|----------|-------|----------------------------|-------|
| `byte[] pronosticos` (N+1) | `[UInt8]` | 0…7; 0 = ignorado; posición `N` solo la toca `TodosATriple` (=7) y nunca se evalúa | :27,328-336 |
| `int noPartidos` | `let n: Int32` | = `NumeroPartidos` **al construir**; ver Abiertas A-11 | :30,70 |
| `bool[] AC, ACS, FS` (N+1) | `[Bool]` | índice = valor permitido (0…N); todo `false` | :32-34,72-74 |
| `char[] ACTol, ACSTol, FSTol` (N+1) | `[Character]` | letra de tolerancia del valor; `" "` = sin letra | :40-42,76-78 |
| `string strPuntos`, `bool[] puntos` | `String`, `[Bool]?` | `""`/`nil`; `puntos` = D2 de `strPuntos` | :36-37 |
| `noAC, noACS, noFS, noPuntos` | `Int32` | resultado de la última `Analizar`; 0 | :45-48 |
| `noPuntosFijos/Dobles/Triples` | `Int32` | 6 / 3 / 1 | :50-52 |
| `bool tolLocalActiva` | `Bool` | `false`; se pone `true` con `SetACTol/ACSTol/FSTol` o por la propiedad | :55,472,484,495,736-740 |
| `bool[] ACTolLocal, ACSTolLocal, FSTolLocal` (N+1) | `[Bool]` | valores tolerados fuera del rango estricto | :56-58 |
| `bool[4] tolLocales` | `[Bool]` (4) | índice = nº de fallos tolerados (0…3) permitido | :60 |
| `falloACBase, falloACSBase, falloFSBase`, `noFallosActual` | `Bool`×3, `Int32` | efecto lateral de la validación | :62-65 |

**Entradas / salidas.**

- `Analizar(col: Int64) -> Bool` (`:362-367`) y `Analizar(col, numCol) -> String` (`:369-374`, texto de fallos, §2/P19).
- Lectura: `NoAC`, `NoACS`, `NoFS` (`:723-734`), `ToleranciaLocalActiva`, `Pronosticos` (`[String]` de N, `:695-709`),
  `PronosticosString` (CSV, `:678-692`), `GetAciertos/GetAciertosSeguidos/GetFallosSeguidos` (`:605-672`),
  `GetPuntos`, `GetTolerancias/GetACTol/GetACSTol/GetFSTol` (`:513-603`).
- Escritura: `SetNoAciertos/SetNoAciertosSeguidos/SetNoFallosSeguidos(String)`, `SetPuntos`, `SetACTol/SetACSTol/SetFSTol`,
  `SetTolerancias`, `Pronosticos = [String]`, `BorraPronosticos()`, `TodosATriple()`, `ReinicializaValores()`,
  `InicializaPuntosCP(f,d,t)`, `ObtenNoAciertosTolerancias(letras) -> Int`.
- Sin ficheros. Sin I/O.

**Algoritmo.**

```
Analizar(col: Int64) -> Bool:
  noAC = noACS = noFS = noPuntos = 0; falloACBase = falloACSBase = falloFSBase = false     // :259-269
  calcular(col)
  return cumpleCondiciones()

calcular(col):                                                  // :271-323
  tACS = 0; tFS = 0
  for i in 0..<N:                       // N = VariablesGlobales.NumeroPartidos EN VIVO (:276)
     p = pronosticos[i]
     if p != 0:
        if (Int32(p) & Int32(col & 7)) > 0:                     // acierto (cualquier solape de bits)
            noAC += 1; tACS += 1; tFS = 0
            noPuntos += (p == 6 or p == 5 or p == 3) ? noPuntosDobles
                       : (p == 7 ? noPuntosTriples : noPuntosFijos)
        else:
            tFS += 1; tACS = 0
        if noACS < tACS { noACS = tACS }
        if noFS  < tFS  { noFS  = tFS  }
     col >>= 3                                                   // SIEMPRE, también si p == 0  (P1)

cumpleCondiciones() -> Bool:                                     // :130-148
  cumple = esColumnaValida()
  if strPuntos != "":
      if puntos.count < noPuntos + 1 or not puntos[noPuntos]: return false      // P3: antes que tolerancias
  if tolLocalActiva: return toleranciasValidas()                                // P2
  return cumple

esColumnaValida() -> Bool:                                       // :161-183  (P4: evalúa los tres)
  ok = true
  if not AC[noAC]:  ok = false; falloACBase  = true
  if not ACS[noACS]: ok = false; falloACSBase = true
  if not FS[noFS]:  ok = false; falloFSBase  = true
  return ok

toleranciasValidas() -> Bool:                                    // :215-257
  noFallosActual = 0
  if falloACBase:  if ACTolLocal[noAC]:   noFallosActual += 1 else return false
  if falloACSBase: if ACSTolLocal[noACS]: noFallosActual += 1 else return false
  if falloFSBase:  if FSTolLocal[noFS]:   noFallosActual += 1 else return false
  return tolLocales[noFallosActual]            // 0…3; tolLocales[0] == false rechaza una columna perfecta (P2)

ObtenNoAciertosTolerancias(letras: String) -> Int:               // :337-360 (usa noAC/noACS/noFS de la ÚLTIMA Analizar)
  n = 0
  for letra in letras:                          // cada carácter, repetidos incluidos
     if ACTol[noAC]   == letra { n += 1 }
     if ACSTol[noACS] == letra { n += 1 }
     if FSTol[noFS]   == letra { n += 1 }
  return n
```

*Texto* `Analizar(col, numCol)` (`:150-213`): tras calcular, concatena (nunca corta):

```
si !AC[noAC]:     "Fallo en Aciertos en la columna {numCol}  ({noAC})#"          ; falloACBase = true
si ACTol[noAC] != " ":  "!Tolerancias en Aciertos en la columna {numCol}  ({noAC})#"
si !ACS[noACS]:   "Fallo en Aciertos Seguidos en la columna {numCol}  ({noACS})#"; falloACSBase = true
si ACSTol[noACS] != " ": "!Tolerancias en Aciertos Seguidos en la columna {numCol}  ({noACS})#"
si !FS[noFS]:     "Fallo en Fallos Seguidos en la columna {numCol}  ({noFS})#"    ; falloFSBase = true
si FSTol[noFS] != " ":  "!Tolerancias en Fallos Seguidos en la columna {numCol}  ({noFS})#"
si strPuntos != "" y (puntos.count < noPuntos+1 o !puntos[noPuntos]):
                  "Fallo en Puntuación de la columna {numCol}  ({noPuntos})#"
```

(Dos espacios antes de `(`. El texto **ignora** las tolerancias locales.)

*Setters de listas* `SetNoAciertos/SetNoAciertosSeguidos/SetNoFallosSeguidos(valores)` (`:376-452`, idénticos salvo destino):

```
for str in valores.split(",", omitEmpty: false):          // S1
   // str vacío: el C# hace str[str.Length-1] -> IndexOutOfRange  => error de configuración
   last = str.last
   if last.isLetter:  v = convertToInt32(str.dropLast()); letra = last
   else:              v = convertToInt32(str);              letra = " "
   destino[v] = true;  destinoTol[v] = letra                // v fuera de 0...N => error (IndexOutOfRange)
```

No limpia: **acumula** (P5); un valor repetido sin letra borra la letra previa. `SetPuntos(s)` (`:454-467`):
`strPuntos = s; puntos = s != "" ? D2(s) : nil`. `SetACTol/SetACSTol/SetFSTol(s)` (`:470-501`):
`tolLocalActiva = true` **antes** de parsear; cada token `convertToInt32` → `…TolLocal[v] = true`. `SetTolerancias(s)`
(`:503-511`): cada token → `tolLocales[v] = true` (v ∈ 0…3; no activa `tolLocalActiva`).

`Get*` (`:513-672`): índices con `true` en orden ascendente unidos por `,`; `GetAciertos/…Seguidos/…FallosSeguidos`
añaden la letra (`counter + ACTol[counter].Trim()` → `"3A"`, `"3"`); `GetPuntos` devuelve `strPuntos`.

`Pronosticos` set (`:711-720`): `for i in 0..<value.count: pronosticos[i] = D1(value[i])` (P5: no limpia el resto; `value.count > N+1` ⇒
IndexOutOfRange; elementos `nil`/inválidos ⇒ 0). Get (`:695-709`): `[D1⁻¹(pronosticos[i]) for i in 0..<N]`.
`BorraPronosticos()` = set con `N+1` `nil` (todo a 0). `TodosATriple()` = set con `N+1` × `"1X2"` (`:324-336`).
`PronosticosString` = `Pronosticos.joined(",")` (`:678-692`).
`ReinicializaValores()` (`:105-128`): pone a `false/' '` AC, ACS, FS, sus `*Tol`, `*TolLocal`, `tolLocales`, `tolLocalActiva=false`;
**no** toca `pronosticos`, `strPuntos`/`puntos`, puntos fijo/doble/triple ni contadores (P5).
`InicializaPuntosCP(f,d,t)` (`:742-748`) asigna los tres.

**Estado global.** Lee `VariablesGlobales.NumeroPartidos` en construcción (`:27,70`) y en vivo en `calcular`, `Pronosticos`,
`BorraPronosticos`, `TodosATriple` (`:276,326,330,697-704`). No escribe estado global.

**Peculiaridades a preservar.** P1 (rachas atraviesan huecos), P2, P3, P4, P5, P6 (D1), `tolLocales[0]`;
`noPuntos` se calcula siempre; `ObtenNoAciertosTolerancias` cuenta cada carácter de `letras` contra las
tres familias y los **repetidos cuentan doble** (si `letras` contiene un espacio coincide con todo valor sin letra).

**Trampas Swift.** S1 (setters con vacíos), S2, S3, S4 (índices fuera de rango y `str` vacío ⇒ error de configuración
en *set-time*), S5, S6 (`" "` como «sin letra»), S9; `col >>= 3` sobre `Int64` (T2; nunca negativa, 42 bits);
`(p & Int32(col & 7)) > 0` con `p` promovido a `Int32`.

**Casos golden propuestos.**

| Id | Entrada | Esperado | Origen |
|----|---------|----------|--------|
| G04-CP-01 | §0.1 (pron. `1,∅,1,X,1X2`; col `12 1 1 X 1…`; AC/ACS/FS = todo permitido) | `noAC=3 noACS=2 noFS=1 noPuntos=13`; con `InicializaPuntosCP(10,15,20)` → 40 | SIM |
| G04-CP-02 | CP-01 con AC=`{3}`, ACS=`{2}`, FS=`{1}`, sin tolerancias locales | `true` | SIM |
| G04-CP-03 | CP-02 + `SetACTol("2")` + `SetTolerancias("1")` | `false` (P2: `tolLocales[0]=false`) | SIM |
| G04-CP-04 | CP-03 + `SetTolerancias("0")` (→ `tolLocales={0,1}`) | `true` | SIM |
| G04-CP-05 | AC=`{4}` (falla), `SetACTol("3")`, `SetTolerancias("1")`, ACS/FS ok | `true`; con `tolLocales={0}` → `false` | SIM |
| G04-CP-06 | AC=`{4}`, ACS=`{3}` (fallan ambos), `ACTolLocal{3}`, `ACSTolLocal{2}`, `tolLocales={1}` | `false`; añadiendo `tolLocales[2]` → `true` | SIM |
| G04-CP-07 | CP-02 + `SetPuntos("13")` / `SetPuntos("12")` | `true` / `false` (con puntos fallando y tolerancia activa, también `false`) | SIM |
| G04-CP-08 | AC=`"3A,4"`, ACS=`"2B"`, FS=`"1A"` y columna de CP-01: `ObtenNoAciertosTolerancias` de `"A"`,`"B"`,`"AB"`,`"AA"`,`"C"`,`""` | 2, 1, 3, 4, 0, 0 | CALC |
| G04-CP-09 | Texto `Analizar(col,1)` sobre la columna de CP-01 con AC=`"3A"`, ACS=`"9"`, FS=`"1"` | `!Tolerancias en Aciertos en la columna 1  (3)#Fallo en Aciertos Seguidos en la columna 1  (2)#` | CALC |
| G04-CP-10 | `Pronosticos = ["X1","21","1X",nil,"1,X"]` | bytes `[0,0,6,0,6]`, resto sin tocar | CALC |
| G04-CP-11 | `SetNoAciertos("3,,4")` y `SetNoAciertos("")` | error de configuración (C#: `IndexOutOfRange`) | CALC |

**Evidencia.** `MC/ColumnaProbable.cs:27-103` (campos/ctor), `:105-128`, `:130-148`, `:150-213`, `:215-257`, `:259-323`,
`:324-360`, `:362-374`, `:376-511`, `:513-692`, `:693-748`; `UT/UtilColumnas.cs:94-167`; `MC/Grupo.cs:348,391`;
`TST/RelacionCP1GoldenTests.cs:28-44` (uso real: `Pronosticos`, `SetNoAciertos(Todos)`…).

---

## 2. `FiltroColProbables` — `MC/FiltroColProbables.cs`

**Propósito.** Implementación de `IFiltro` (`MC/IFiltro.cs:25-40`) de la condición «Columnas probables»:
gestiona la lista de CPs, el control de fallos y las relaciones CP1/CP2/CP3.

**Tipos y datos.** `isActive: Bool`, `contieneDatos: Bool`, `figuras: [Int64]?` (ningún llamador la asigna para este filtro: `.Figuras =` solo aparece para PesosNum/Contactos/análisis VX2;
`UsaFiguras() = figuras != nil ∧ !figuras.isEmpty`, `:250-260`), `colProbables: [ColumnaProbable]`,
`controlRelaciones1/2/3`, `controlFallosCP` (todos creados en el constructor, `:38-52`; solo CP1, fallos y CP2
reciben la lista, `:42-49`). `NombreFiltro = Filtro.ColProbables` (`:204-207`; `Free1X2.Domain/EnumFiltros.cs:27`);
`EsNombreFiltro(n) = n == "ColProbables"` (`:209-221`); `AnalisisActivo = VariablesGlobales.AnalizarCPs` (`:274-277`).

**Entradas / salidas.** `Analizar(Int64) -> Bool`; `AnalizarFallos(Int64) -> [String]?`;
`ObtenNoAciertosTolerancias(letras) -> Int` = suma sobre las CPs (`:223-233`); `InicializaPuntosCP` (`:150-157`);
propiedad `ColProbables` (setter ver §0.2).

**Algoritmo.** `Analizar`: §0.3. `AnalizarRelaciones2` (`:93-105`): recorre `Relaciones2`, `esValida = rel.Analizar()`, corta en la
primera falsa (AND). `AnalizarRelaciones3` (`:106-113`): `controlRelaciones3.AnalizaRelaciones()`.

`AnalizarFallos(col)` (`:114-148`):

```
texto = ""; numCol = 0
for cp in colProbables: numCol += 1; texto += cp.Analizar(col, numCol)          // todas, sin cortar
controlFallosCP.Analiza(&txt, texto.count > 0); texto += txt                    // el bool se ignora (`MC/ControladorCPControlFallos.cs:75-76`)
controlRelaciones1.Analiza(&txt); texto += txt                                  // todas las relaciones, sin cortar
controlRelaciones2.Analiza(&txt); texto += txt
controlRelaciones3.Analiza(&txt); texto += txt                                  // OJO P16
if texto.count > 0: texto = texto.dropLast(); return texto.split("#", omitEmpty: false)   // quita el '#' final
return nil
```

**Protocolo de texto (P19).** Cada elemento es un mensaje; el consumidor (`WUI/AnalizarCombinacionFrmViewModel.cs:253-300`)
clasifica por su **primer carácter**: `*` = condición cumplida (se muestra sin el `*`), `!` = cumplida por
tolerancia, cualquier otro = fallo. Devuelve `nil` solo si no hay ningún mensaje; como `ControladorCPControlFallos`
emite líneas `*…aceptada` (§4), el resultado es no nulo siempre que haya controles de fallos.

**Estado global.** `VariablesGlobales.AnalizarCPs` (`:276`).

**Peculiaridades.** P7: al cortar en la 1ª CP fallida las siguientes conservan `noAC` de la columna previa
(no se leen porque se devuelve `false`). `controlRelaciones2/3` solo se consultan si `Count > 0` (`:77,86`); el
resultado de CP3 es el resultado final. `ColProbables` setter pone `contieneDatos = (lista != nil ∧ count > 0)` (`:172-179`),
pero **no** lo hace `ColProbables.Add(cp)` (sigue en `false`), p. ej. `TST/ManualFuncionalTests.cs:68`.
`RelacionesCP3` no recibe lista de CPs en el setter (cada `RelacionCP3` lleva la suya).

**Trampas Swift.** S3 (lista compartida; el `Add` externo debe verse en todas las relaciones, §0.2), S1 (`texto.split("#")` con
vacíos), `Optional<[String]>` para `null`, T12 no aplica (sin bombeo).

**Casos golden propuestos.**

| Id | Entrada | Esperado | Origen |
|----|---------|----------|--------|
| G04-FCP-01 | 10 fijos `1` + 4 triples, CP pron. `∅×10,1,1,1,1`, AC=`{3,4}`, ACS/FS todo permitido | 9 columnas aceptadas de 81 | TST (`TST/ManualFuncionalTests.cs:52-77`) |
| G04-FCP-02 | Filtro sin CPs ni relaciones, cualquier columna | `Analizar = true`; `AnalizarFallos = nil` | CALC |
| G04-FCP-03 | CP1 falla en la 1ª CP | `false` sin evaluar relaciones; `NoAC` de CP2 no cambia | CALC |
| G04-FCP-04 | `AnalizarFallos` con CPs que cumplen todo y sin letras de tolerancia, y un único `CPControlFallos` aceptado (`FallosPermitidos=""`) | `["*1ª linea de Tolerancias Locales aceptada."]` (no nulo aunque no haya fallos) | CALC |

**Evidencia.** `MC/FiltroColProbables.cs:26-281`; `MC/IFiltro.cs:25-40`; `MC/Grupo.cs:169-262`; `WUI/AnalizarCombinacionFrmViewModel.cs:253-300`.

---

## 3. `CPControlFallos` — `MC/CPControlFallos.cs`

**Propósito.** Una «línea de tolerancias locales»: suma cuántas letras de tolerancia de las CPs indicadas
quedan «gastadas» en la columna actual y exige que esa suma esté en un conjunto permitido.

**Tipos y datos.** `strColumnas`, `strTolerancias`, `strAciertos` (`String`, `""`), `noToleranciasAcumulados: Int32`,
`columnasRelacionadas: [Int32]` (números de CP **1-based**), `aciertosPermitidos: [Bool]`, `controlador` (referencia
al padre), `columnasProbables: [ColumnaProbable]` (copiada de `controlador.ColumnasProbables` al asignar `Controlador`, `:217-224`).

**Entradas / salidas.** Propiedades `Columnas`, `Aciertos` (parsean al asignar), `Tolerancias`
(`strTolerancias = value.ToUpper()`, `:214`), `ToleranciasAcumuladas` (lectura/escritura), `Controlador` (set).
`Analiza() -> Bool`.

**Algoritmo.**

```
Analiza() -> Bool:                                               // :41-64
  noToleranciasAcumulados = 0
  for n in columnasRelacionadas:
      noToleranciasAcumulados += columnasProbables[n-1].ObtenNoAciertosTolerancias(strTolerancias)
  if aciertosPermitidos.count - 1 < noToleranciasAcumulados: return false
  return aciertosPermitidos[noToleranciasAcumulados]
```

`Columnas = s` → `InicializarColumnas` (`:66-103`; igual que `RelacionCP1.InicializarColumnasRelacionadas`):
`sep` = primer carácter de `s` que sea `,` o `-` (`:160-183`), `""` si ninguno;

```
sep == ",": columnasRelacionadas = s.split(",").map(convertToInt32)
sep == "-": a = ToInt32(t[0]); b = ToInt32(t[1]) (t = s.split("-"), tokens extra ignorados);
            n = b - a + 1; columnasRelacionadas = [a, a+1, …, a+n-1]   (n == 0 -> []; n < 0 -> Overflow en C#)
otro:       [convertToInt32(s)]                                   ("" -> FormatException)
```

`Aciertos = s` → `InicializarAciertos` (`:105-158`):

```
sep == ",": t = s.split(","); cap = ToInt32(t.last) + 2; arr = [Bool](cap); arr[ToInt32(x)] = true para cada x   // requiere orden ascendente
sep == "-": a = ToInt32(t[0]); b = ToInt32(t[1]); cap = b + 2; arr[a ... b] = true
otro:       v = ToInt32(s); cap = v + 2; arr[v] = true
```

Equivale a D2 (`bool[max+2]`) para listas ascendentes y rangos `a≤b`; **no admite formas mixtas** (`"1-3,5"`:
el primer separador es `-` ⇒ `ToInt32("3,5")` ⇒ `FormatException`).

**Estado global.** Ninguno.

**Peculiaridades.** P17: (a) lista de aciertos no ascendente (`"5,3"`) ⇒ `cap` calculado con el último ⇒ `IndexOutOfRange`;
(b) `Tolerancias` se pasa a mayúsculas y cada carácter cuenta contra ACTol/ACSTol/FSTol de **cada** CP: `"AA"` cuenta doble;
(c) la suma usa las letras de la tabla «global» de la CP (`ACTol…`), **no** las tolerancias locales; (d) la comprobación
`count-1 < acum` equivale a «índice fuera ⇒ falso».

**Trampas Swift.** S1, S2, S4, S5, S7 (`ToUpper`), S11 (no `Set`).

**Casos golden propuestos.** Sobre CP de G04-CP-08 (`noAC=3 noACS=2 noFS=1`, `ACTol[3]='A'`, `ACSTol[2]='B'`, `FSTol[1]='A'`),
`Columnas="1"`, `Tolerancias="a"` (→ `"A"`, `acum=2`):

| Id | `Aciertos` | `arr.count` | Esperado | Origen |
|----|-----------|-------------|----------|--------|
| G04-CPF-01 | `"0-1"` | 3 | `false` | SIM |
| G04-CPF-02 | `"2"` | 4 | `true` | SIM |
| G04-CPF-03 | `"0,1"` | 3 | `false` | SIM |
| G04-CPF-04 | `"1-3"` | 5 | `true` | SIM |
| G04-CPF-05 | `Columnas="1-1"`, `Aciertos="2"` | — | `true`; `Columnas="2-1"` ⇒ `[]`, `acum=0` | CALC |

**Evidencia.** `MC/CPControlFallos.cs:26-226`; `MC/ColumnaProbable.cs:337-360`.

---

## 4. `ControladorCPControlFallos` — `MC/ControladorCPControlFallos.cs`

**Propósito.** Agrupa las líneas `CPControlFallos` y decide si la columna es válida, bien exigiendo que todas
pasen, bien permitiendo cierto nº de líneas falladas.

**Tipos y datos.** `controlesFallos: [CPControlFallos]`, `columnasProbables`, `strFallosPermitidos: String` (`""`),
`fallosPermitidos: [Bool]?` (D2 de `strFallosPermitidos`; `nil` si `""`, `:136-152`).

**Entradas / salidas.** `Analiza() -> Bool`, `Analiza(&txt, tieneFallos)` (texto), `ControlesFallos` (set asigna `ctrl.Controlador = self`
a cada línea, `:115-128`), `FallosPermitidos` (set), `ColumnasProbables`.

**Algoritmo.**

```
Analiza() -> Bool:                                               // :40-71
  if strFallosPermitidos == "":
      for c in controlesFallos: if !c.Analiza() { return false }          // corta
      return true
  noFallos = 0
  for c in controlesFallos: if !c.Analiza() { noFallos += 1 }              // evalúa TODAS (P18)
  return !(fallosPermitidos.count < noFallos + 1 or !fallosPermitidos[noFallos])

Analiza(&txt, tieneFallos):                                      // :73-107   (el 2º parámetro se ignora)
  txt = ""
  if strFallosPermitidos == "":
      for (i, c) in controlesFallos.enumerated():
          if !c.Analiza(): txt += "Fallo en {i+1}ª linea de Tolerancias Locales  ({c.ToleranciasAcumuladas})#"
          else:            txt += "*{i+1}ª linea de Tolerancias Locales aceptada.#"
  else:
      noFallos = nº de c con !c.Analiza()
      if fallosPermitidos.count < noFallos+1 or !fallosPermitidos[noFallos]:
          txt += "Fallo en Fallos de Controles permitidos  ({noFallos})#"      // si es válido NO hay línea (P18)
```

Sin controles: con `""` ⇒ `true`; con `fallosPermitidos` ⇒ `fallosPermitidos[0]`.

**Estado global.** Ninguno. **Peculiaridades.** P18; `ª` = U+00AA; «linea» sin tilde.

**Trampas Swift.** S4 (`fallosPermitidos` nulo con `strFallosPermitidos != ""` es imposible: el setter lo construye), S12.

**Casos golden propuestos.** Dos controles sobre la CP de G04-CP-08: C1 (`Columnas="1"`, `Tolerancias="A"`, `Aciertos="2"` ⇒ acepta, `acum=2`) y
C2 (`Columnas="1"`, `Tolerancias="B"`, `Aciertos="0"` ⇒ falla, `acum=1`):

| Id | `FallosPermitidos` | `Analiza()` | Texto | Origen |
|----|-------------------|-------------|-------|--------|
| G04-CCF-01 | `""` | `false` | `*1ª linea de Tolerancias Locales aceptada.#Fallo en 2ª linea de Tolerancias Locales  (1)#` | SIM |
| G04-CCF-02 | `"1"` | `true` | `""` | SIM |
| G04-CCF-03 | `"0"` | `false` | `Fallo en Fallos de Controles permitidos  (1)#` | SIM |

**Evidencia.** `MC/ControladorCPControlFallos.cs:27-153`.

---

## 5. `RelacionCP1` — `MC/RelacionCP1.cs`

**Propósito.** Relación entre un conjunto de CPs: (a) **cuántas** de ellas tienen un nº de aciertos dentro de un conjunto, (b) **suma** de
aciertos y (c) **recorrido** (máx − mín) permitidos.

**Tipos y datos.**

| Campo | Swift | Notas | Línea |
|-------|-------|-------|-------|
| `strColumnas` / `columnasRelacionadas: int[]` | `String` / `[Int32]` | nº de CP **1-based** | :29,55,272-309 |
| `strSumas` / `sumas: bool[]` | `String` / `[Bool]` | solo si `strSumas != ""` | :30,58,311-351 |
| `strRecorrido` / `recorridos: bool[15]` | `String` / `[Bool]` (15) | **siempre** se asigna 15 posiciones | :31,59,353-393 |
| `strCantidadCP` / `cantidadCP: bool[]?` | `String` / `[Bool]?` | D2; `nil` si `""` | :34,61,450-468 |
| `strCuantosAC` / `cuantosAC: bool[]?` | `String` / `[Bool]?` | D2; `nil` si `""` | :35,62,470-488 |
| `listaCPs: List<int>` | `[Int32]` | **posiciones** (0-based) dentro de `columnasRelacionadas` | :42 |
| `noAciertosPorCP: int[15]`, `cpsPorNoAciertos: List<int>[15]` | `[Int32]`(15), `[[Int32]]`(15) | búferes reutilizados | :45-46 |
| `noColConMismosAC`, `recorrido` (públicos), `sumaTemp` | `Int32` | `recorrido` no se reinicia | :68-70 |

**Entradas / salidas.** `Columnas`, `SumaAciertos`, `Recorridos`, `CantidadCP`, `CuantosAC` (setters que parsean), `Controlador` (set copia la lista).
`Analiza() -> Bool`; `Analiza(numRel) -> String`.

**Algoritmo.**

```
Analiza() -> Bool:                                               // :72-89
  listaCPs.clear(); sumaTemp = 0; noColConMismosAC = 0           // InicializaValores :117-122
  ok = true
  if strCantidadCP != "": ok = analizaGrupos()
  if ok and strSumas != "": ok = analizaSumas()
  if ok and strRecorrido != "": ok = analizaRecorridos()
  return ok

analizaGrupos():                                                 // :233-270
  noAciertosPorCP = [0]*15; cpsPorNoAciertos = [[]]*15
  for (i, n) in columnasRelacionadas.enumerated():
      cp = columnasProbables[n-1]
      noAciertosPorCP[cp.NoAC] += 1;  cpsPorNoAciertos[cp.NoAC].append(i)          // cp.NoAC > 14 => IndexOutOfRange (P9)
  for i in 0..<cuantosAC.count:                    // cuantosAC == nil => NullReference (P9)
      if cuantosAC[i]:
          noColConMismosAC += noAciertosPorCP[i]                                    // i >= 15 con true => IndexOutOfRange
          listaCPs += cpsPorNoAciertos[i]          // orden: nº de aciertos creciente, y dentro de cada uno el orden de columnasRelacionadas
  return noColConMismosAC < cantidadCP.count and cantidadCP[noColConMismosAC]

analizaSumas():                                                  // :124-161
  if listaCPs.isEmpty:  sumaTemp = Σ columnasProbables[n-1].NoAC  para n en columnasRelacionadas      // P8: TODAS
  else:                 sumaTemp = Σ columnasProbables[columnasRelacionadas[pos]-1].NoAC  para pos en listaCPs
  return !(sumas.count < sumaTemp + 1) and sumas[sumaTemp]

analizaRecorridos():                                             // :163-224
  maxAC = 0; minAC = 14
  fuente = listaCPs.isEmpty ? todas las columnasRelacionadas : las de listaCPs      // P8
  for cp in fuente: minAC = min(minAC, cp.NoAC); maxAC = max(maxAC, cp.NoAC)
  recorrido = maxAC - minAC
  return recorridos[recorrido]                                   // 15 posiciones; sin comprobación de límites
```

*Parseo de textos.* `Columnas` = §3 (`InicializarColumnasRelacionadas`, `:272-309`). `SumaAciertos` (`:311-351`):
`""` ⇒ no se toca `sumas`; `,` ⇒ `cap = ToInt32(últimoToken)+2`; `-` ⇒ `cap = b+2`; otro ⇒ `cap = v+2` (idéntico a §3
`Aciertos`; mixtos ⇒ `FormatException`). `Recorridos` (`:353-393`): siempre `recorridos = [false]*15`; luego `,` ⇒ marca
cada valor; `-` ⇒ marca `a…b`; otro ⇒ marca `v` (valor ≥ 15 ⇒ `IndexOutOfRange`). `CantidadCP`/`CuantosAC`: D2 si `!= ""`, si no `nil`.

*Texto* `Analiza(numRel)` (`:91-115`): mismos pasos y mismo encadenado por `ok` (tras el primer fallo no se evalúa el resto), por lo que el texto contiene **como máximo un mensaje**:

```
si cantidad falla: "Fallo en Cantidad de CPs de la Relación de Columnas {numRel}  ({noColConMismosAC})#"
si suma falla:     "Fallo en Suma de Aciertos de la Relación de Columnas {numRel}  ({sumaTemp})#"
si recorrido falla:"Fallo en Recorrido de la Relación de Columnas {numRel}  ({recorrido})#"
```

**Estado global.** Ninguno (el N máximo es el literal 15/14: `:45,236,355,168`).

**Peculiaridades.** P8 (leído del código, `MC/RelacionCP1.cs:130-137,174-192`; **no** lo ejercita el golden vigente: la relación 1 de `TST`
usa `CuantosAC="2,3,4,5"`/`CantidadCP="0-6"` pero en las 243 columnas siempre hay alguna CP con 2-5 aciertos, SIM; cubrirlo con G04-R1-03); P9; `Columnas=""` ⇒
`FormatException`; el orden de `listaCPs` no afecta a suma/máx/mín pero sí es el orden de inserción documentado; `recorrido`
y `noColConMismosAC` son públicos y quedan con el último valor calculado.

**Trampas Swift.** S1, S2, S4, S5, S3 (leer `cp.noAC` en vivo), búferes de 15 posiciones: reproducir el límite (o validar `noAC ≤ 14`).

**Casos golden propuestos.**

| Id | Entrada | Esperado | Origen |
|----|---------|----------|--------|
| G04-R1-01 | **Golden existente** `TST/RelacionCP1GoldenTests.cs` (6 CPs sintéticas, 243 columnas, 3 relaciones, variantes «suma estrecha» y «recorrido estrecho») | cadenas de validez, SHA-256 y textos de `:140-150` y `:158-164` | TST |
| G04-R1-02 | `NoAC=[5,3,3,7]`; `Columnas="1-4"`, `CuantosAC="3"`, `CantidadCP="2"`, `SumaAciertos="6"`, `Recorridos="0"` | `true`; `noColConMismosAC=2`, `listaCPs=[1,2]`, `sumaTemp=6`, `recorrido=0` | SIM |
| G04-R1-03 | mismas CPs; `CuantosAC="9"`, `CantidadCP="0"`, `SumaAciertos="18"`, `Recorridos="4"` | `true` (P8: `listaCPs` vacía ⇒ suma 18 y recorrido 7−3=4 sobre las 4 CPs; caso **no cubierto** por el golden vigente) | SIM |
| G04-R1-04 | igual que 03 con `SumaAciertos="6"` | `false`; texto `Fallo en Suma de Aciertos de la Relación de Columnas 1  (18)#` | SIM |
| G04-R1-05 | `Columnas=""`; `CantidadCP="1"` con `CuantosAC=""` | error de configuración (C#: `FormatException` / `NullReference` al analizar) | CALC |

**Evidencia.** `MC/RelacionCP1.cs:27-499`; `TST/RelacionCP1GoldenTests.cs:23-165`.

---

## 6. `ControladorRelacionesCP1` — `MC/ControladorRelacionesCP1.cs`

**Propósito.** Lista de `RelacionCP1`; todas deben cumplirse.

**Tipos y datos.** `relaciones: [RelacionCP1]`, `columnasProbables`.

**Entradas / salidas.** `PonerRelacion(rel)` (añade y `rel.Controlador = self`, `:36-40`); `Relaciones` set (asigna `Controlador` a cada una, `:70-83`);
`Analiza() -> Bool`; `Analiza(&txt)`.

**Algoritmo.**

```
Analiza() -> Bool:        for rel in relaciones: if !rel.Analiza() { return false };  return true        // :42-58 (corta)
Analiza(&txt):            txt = ""; for (i, rel): txt += rel.Analiza(i+1)                                // :60-68 (todas, sin cortar)
```

**Estado global.** Ninguno. **Peculiaridades.** `ColumnasProbables` set no se propaga a relaciones ya asignadas (P24);
`Relaciones.Add(rel)` directo **no** asigna `Controlador` (`columnasProbables` nulo ⇒ NullReference al evaluar).

**Trampas Swift.** S3, S4.

**Casos golden propuestos.** G04-R1-01 (golden vigente, usa `PonerRelacion` + `Analiza()` + `Analiza(ref txt)`). G04-RC1-01: dos relaciones, la 1ª falla ⇒ `Analiza()=false` y texto = concatenación de los fallos de ambas en orden. Origen: CALC.

**Evidencia.** `MC/ControladorRelacionesCP1.cs:25-91`.

---

## 7. `RelacionCP2` — `MC/RelacionCP2.cs`

**Propósito.** Comparar el total (o cada par) de aciertos / aciertos seguidos / fallos seguidos entre dos grupos de CPs (A y B):
«A tiene `Más`/`Menos` que B» en una cantidad permitida. Dos bloques independientes: **globales** (suma de A − suma de B) e **individuales** (cada A2 contra cada B2).

**Tipos y datos.**

| Campo | Swift | Notas | Línea |
|-------|-------|-------|-------|
| `columnasA`, `columnasB`, `columnasA2`, `columnasB2` | `[Int32]` | índices **0-based** a `columnasProbables` (vienen de D3 `ObtenerListaFromTxt`, el texto es 1-based) | :24-28 |
| `arrayAciertos`, `arrayAciertos2` | `[Int32]` | valores permitidos (D3 `…Aciertos`, no negativos) | :30-31 |
| `concepto`, `cantidad`, `concepto2`, `cantidad2` | `String` | `"AC"`/`"ACS"`/`"FS"`; `"Más"`/`"Menos"`; defecto `""` | :33-37 |
| `AC_A, AC_B, ACS_A, ACS_B, FS_A, FS_B` | `Int32` | acumuladores | :39 |
| `columnasProbables` | `[ColumnaProbable]` | asignada por quien carga (§0.2) | :40 |
| `strColsA/B/Aciertos`, `strColsA2/B2/Aciertos2` | `String` | texto original (persistencia) | :42-44 |

`ActivaGlobales = A.count>0 ∧ B.count>0 ∧ Aciertos.count>0` (`:306-312`); `ActivaIndividuales` análogo con `…2` (`:314-320`).

**Entradas / salidas.** Propiedades de lectura/escritura; `Analizar() -> Bool`; `Analiza(numRel) -> String`.

**Algoritmo.**

```
Analizar() -> Bool:                                              // :152-166
  ok = true
  if ActivaGlobales:                  ok = analizaGlobales()
  if ActivaIndividuales and ok:       ok = analizaIndividuales()
  return ok

analizaGlobales():                                               // :46-91, 188-213
  AC_A=ACS_A=FS_A=AC_B=ACS_B=FS_B = 0
  for i in columnasA: cp = columnasProbables[i]; AC_A += cp.NoAC; ACS_A += cp.NoACS; FS_A += cp.NoFS
  for i in columnasB: (idem para _B)
  ok = true                                          // P11: cualquier otra combinación deja ok = true
  switch (cantidad, concepto):
     ("Más","AC")  : ok = arrayAciertos.contains(AC_A  - AC_B)     ("ACS","FS" análogos)
     ("Menos","AC"): ok = arrayAciertos.contains(AC_B  - AC_A)     ("ACS","FS" análogos)
  return ok

analizaIndividuales():                                           // :92-150
  ok = true
  for i in columnasA2:                      // el bucle exterior comprueba !ok al inicio de cada vuelta
      if !ok { break }
      cp = columnasProbables[i]
      for j in columnasB2:
          cp2 = columnasProbables[j]
          d = 0
          ("Más", "AC"/"ACS"/"FS")  : d = cp.X  - cp2.X
          ("Menos","AC"/"ACS"/"FS") : d = cp2.X - cp.X           // otro valor desconocido: d = 0 (P11)
          if !arrayAciertos2.contains(d) { ok = false; break }
  return ok
```

*Texto* `Analiza(numRel)` (`:168-186`): si `ActivaGlobales` y `!analizaGlobales()` ⇒ `"Fallo en Relaciones II - Globales#"`; si
`ActivaIndividuales` y `!analizaIndividuales()` ⇒ `"Fallo en Relaciones II - Individuales#"` (los dos bloques se evalúan siempre).

**Estado global.** Ninguno.

**Peculiaridades.** P10: `"2,1"` produce un único índice `[1]` (CP2) y pierde la CP1; `"1,1"` cuenta dos veces la CP1 en las sumas; las
diferencias negativas jamás pertenecen a `Aciertos` (D3 las convierte en lista vacía ⇒ bloque inactivo). P11. Un índice fuera de
`columnasProbables` ⇒ `ArgumentOutOfRange` (el texto del usuario puede nombrar una CP inexistente). `StrColsA`… solo se usan al guardar.

**Trampas Swift.** S1, S11 (duplicados), S8 (`"Más"`/`"Menos"` exactos, con tilde precompuesta), S4, S3.

**Casos golden propuestos.** CPs con `NoAC=[8,6,7,5]`:

| Id | Entrada | Esperado | Origen |
|----|---------|----------|--------|
| G04-R2-01 | A=`"1-2"`→`[0,1]`, B=`"3-4"`→`[2,3]`, `AC`, `Más`, Aciertos=`"0-3"` | `AC_A=14, AC_B=12`, dif=2 ⇒ `true` | CALC |
| G04-R2-02 | igual con `Menos` | dif=−2 ⇒ `false` | CALC |
| G04-R2-03 | solo individuales: A2=`"1"`, B2=`"2-3"`, `AC`, `Más`, Aciertos2=`"0-3"` | pares (8−6=2, 8−7=1) ⇒ `true`; con Aciertos2=`"2"` ⇒ `false` | CALC |
| G04-R2-04 | columnas `"2,1"` | lista `[1]` | SIM |
| G04-R2-05 | `Concepto="XX"` con globales activas | `true` | CALC |

**Evidencia.** `MC/RelacionCP2.cs:22-322`; `UT/UtilidadesEntradasValores.cs:42-181`; `ES/FColProbablesData.cs:154-205`.

---

## 8. `ControladorRelacionesCP2` — `MC/ControladorRelacionesCP2.cs`

**Propósito.** Lista de `RelacionCP2`; todas deben cumplirse.

**Tipos y datos.** `relaciones2: [RelacionCP2]` (`Relaciones2` get/set), `columnasProbables` (get/set; **no** se propaga a las relaciones: el cargador
asigna `rel.ColumnasProbables` a cada una, `ES/FColProbablesData.cs:77,90`).

**Algoritmo.** `Analizar() -> Bool` (`:27-38`): AND con corte. `Analiza(&txt)` (`:39-47`): `txt = ""`; concatena `rel.Analiza(i+1)` para todas.

**Estado global / Peculiaridades.** Ninguno / `Relaciones2` set no toca el `Controlador` (no existe). `FiltroColProbables` solo llama a
`Analizar` si `Count>0` y reimplementa el AND en `AnalizarRelaciones2` (`MC/FiltroColProbables.cs:77-83,93-105`) llamando directamente a
`Relaciones2[i].Analizar()`.

**Trampas Swift.** S3. **Casos golden.** G04-R2-01…05 a través del controlador; G04-RC2-01: dos relaciones, texto = concatenación (Origen: CALC).

**Evidencia.** `MC/ControladorRelacionesCP2.cs:22-58`.

---

## 9. `RelacionCP3` (+ tipos auxiliares) — `MC/RelacionCP3.cs`

**Propósito.** Analizar la **serie** de valores `AC`/`ACS`/`FS` de las CPs (en el orden de la lista) buscando *escaleras*
(secuencias ±1 de ≥3 valores distintos), *sándwiches* (patrón `a b b a`), *agrupaciones paso fijo* (rachas de valores iguales) y *agrupaciones solapadas*.
También la usa el informe ABDON con una serie explícita (`Free1X2.Domain/Analisis/InformeColumnasABDON_UnaColTodasGan.cs:11-14,19-53`).

**Tipos y datos.**

| Campo | Swift | Notas | Línea |
|-------|-------|-------|-------|
| `columnas: List<ColumnaProbable>` | `[ColumnaProbable]?` | **las primeras K CPs** (P12); `nil` con el constructor sin parámetros | :51,54-66 |
| `concepto: string` | `String?` | `nil` por defecto: `"AC"`/`"ACS"`/`"FS"` (otro ⇒ serie de ceros) | :33,233-261 |
| `serieAciertos: int[]` | `[Int32]?` | **caché**; `SerieAciertos` set la fija | :49,548-553 |
| `escaleras`, `sandwichs`, `agrupacionesPasoFijo: int[,]`, `agrupacionesSolapadas: int[,]` | cachés `?` | se anulan con `ActualizaValores` | :46-50,204-212 |
| `noEscalerasAscPermitidas`, `…Desc…`, `…Totales…`, `noSandwichsPermitidos` | `[Bool]?` | `nil` = sin restricción; construidos con D3 `ObtenerBoolArrayFromTxt(texto, L)` | :27-30,590-609 |
| `agrupacionesPasoFijoPermitidas`, `agrupacionesSolapadasPermitidas` | `[AgrupacionColumnas]?` | `nil` = sin restricción | :31-32,610-619 |
| `*String` (7) | `String?` | texto original (persistencia) | :37-44,632-674 |

Auxiliares: `EscaleraAciertos { escalones: [Int32]; orientacion = escalones[0] > escalones.last ? .descendente : .ascendente }`
(`MC/EscaleraAciertos.cs:6-35`; enum `OrientacionEscalera { Ascendente=1, Descendente=2 }`, `Free1X2.Domain/Enums.cs:105-109`);
`SandwichAciertos { sandwich: [Int32] }` (`MC/SandwichAciertos.cs:6-15`);
`AgrupacionColumnas { numero: [Int32] (nº de rachas permitido), noElementos: Int32 (longitud de racha), noAciertos: [Int32] (valores) }`
(`MC/AgrupacionColumnas.cs:6-30`).

**Entradas / salidas.** Constructores: `()`, `(cps)` (calcula serie con `Concepto` aún `nil` ⇒ ceros), `(serie)` (serie explícita). `Analiza() -> Bool` (`:68-83`);
`Analiza(numRel) -> String` (`:173-202`); `ActualizaValores()`; propiedades derivadas `SerieAciertos`, `Min/Max/SumaTotalDeAciertos`, `Escaleras`,
`Sandwichs`, `NumeroDeEscaleras`, `NumeroDeEscalerasAscendentes`, `NumeroDeEscalerasDescendentes`, `AgrupacionesPasoFijo`, `AgrupacionesSolapadas`.

**Algoritmo.**

```
ActualizaValores():                                              // :204-212
  serieAciertos = nil; obtenerSerie()      // se recalcula YA desde `columnas` (NullReference si columnas == nil)
  agrupacionesPasoFijo = nil; agrupacionesSolapadas = nil; escaleras = nil; sandwichs = nil

obtenerSerie():                                                  // :233-261  (solo si serieAciertos == nil)
  serie = [0] * columnas.count
  switch concepto: "AC": serie[i] = columnas[i].NoAC;  "ACS": .NoACS;  "FS": .NoFS;  otro/nil: ceros      // ordinal, mayúsculas
  (cachea)

Analiza() -> Bool:                                               // :68-83 (cortocircuito en este orden)
  return analizaEscaleras() and analizaSandwichs() and analizaPasoFijo() and analizaSolapadas()

analizaEscaleras():                                              // :135-162
  if noEscalerasAscPermitidas  != nil and !noEscalerasAscPermitidas[nAsc]:   return false
  if noEscalerasDescPermitidas != nil and !noEscalerasDescPermitidas[nDesc]: return false
  if noEscalerasTotalesPermitidas != nil and !noEscalerasTotalesPermitidas[nTotal]: return false
  return true                         // nAsc = nº escaleras ascendentes; nDesc = nTotal - nAsc; sin comprobación de límites
analizaSandwichs(): return noSandwichsPermitidos == nil ? true : noSandwichsPermitidos[sandwichs.count]    // :163-170

analizaPasoFijo():                                               // :85-109
  ok = true
  if permitidasPF != nil:
     for ag in permitidasPF:
        n = 0
        if matrizPF.rows > ag.noElementos:                 // si no, n = 0
            for v in ag.noAciertos: n += matrizPF[ag.noElementos][v]       // v >= 15 => IndexOutOfRange
        ok = ag.numero.contains(n)
        if !ok { break }
  return ok

analizaSolapadas():                                              // :110-133  (P13)
  recorre las permitidas con la misma fórmula sobre matrizSolapada, hace `break` si falla,
  pero NUNCA asigna `ok = false`  => devuelve SIEMPRE true (solo puede lanzar por índice fuera de rango)
```

**Escaleras** (`ObtenerEscaleras`, `:383-430`), `minimoEscalones = 3`:

```
escaleras = []; T = []
if serie.count > 0:
   prev = serie[0]; T = [prev]
   for i in 1..<serie.count:
      if abs(serie[i] - prev) == 1 and !T.contains(serie[i]): T.append(serie[i])
      else:
         if T.count >= 3 { escaleras.append(T) }          // si no, se descarta
         T = [serie[i]]
      prev = serie[i]
if T.count >= 3: escaleras.append(T)
```

Como no se repite ningún valor y cada paso es ±1, toda escalera es **monótona**; la que termina por un cambio de sentido cede su último valor como primero de la siguiente.
`NumeroDeEscalerasAscendentes` cuenta las que no son `Descendente` (`:521-536`; `Escalones[0] <= Escalones.last`, en la práctica `<`).

**Sándwiches** (`ObtenerSandwichs`, `:431-520`) — máquina de estados `capa` (1…5) con lista `T` (P14):

```
capa = 1; T = []; sandwichs = []
for i in 0..<serie.count:
   s = serie[i]
   switch capa:
     1: T.append(s); capa = 2
     2: if s != T[0] { T.append(s); capa = 3 } else { T.removeFirst(); T.append(s) }          // T = [s], capa sigue 2
     3: if s == T[1] { T.append(s); capa = 4 } else { T = [serie[i-1], s] }                  // capa sigue 3
     4: if s == T[0] { T.append(s); capa = 5 } else { T = [serie[i-1], s]; capa = 3 }
     5: if T.count == 4 { sandwichs.append(T) };  T = []; capa = 1                           // el elemento `s` se DESCARTA
fin: if T.count == 4 { sandwichs.append(T) }                                                 // cubre el sándwich terminado en el último elemento
```

Observaciones: `T=[a,b,b,a]`, `a≠b` en el camino normal; los caminos de reinicio de 3/4 pueden formar `a==b` (SIM: `[1,2,3,3,3,3,3,4]` → `[[3,3,3,3]]`).
SIM: `[2,3,3,2,9,2,3,3,2,2,3,3,2,1]` → 2 sándwiches `[2,3,3,2]` (el `9` y el segundo `2` tras cada uno se descartan).

**Agrupaciones paso fijo** (`ObtenerAgrupaciones`, `:291-348`): matriz `M[longitudRacha][valor]` = nº de rachas de exactamente esa longitud de ese valor:

```
M = matriz de 1 fila x 15 columnas a 0; run = 1
if serie.count > 0:
   actual = serie[0]
   for i in 1..<serie.count:
      if serie[i] == actual { run += 1 }
      else { asegurar M.rows > run (crecer hasta run+1 filas); M[run][actual] += 1; run = 1 }
      actual = serie[i]
   asegurar M.rows > run; M[run][actual] += 1
```

Resultado: `rows = longitudMáximaDeRacha + 1` (o 1 si la serie es vacía), 15 columnas (valor ≥ 15 ⇒ `IndexOutOfRange`; `NumeroPartidos=15` con 15 aciertos, P9).
**Solapadas** (`ObtenerAgrupacionesSolapadas`, `:349-382`, P15): `S = copia de M`; para cada `(i, j)` con `M[i][j] > 0`: para `k` desde `i-1` hasta `2` (descendiendo): `suma` empieza en 1
y se incrementa antes de cada uso: `S[k][j] += suma` (suma = 2, 3, …) ⇒ `S[k][j] += i-k+1` (nº de ventanas de longitud `k` dentro de una racha de `i`), **sin multiplicar por `M[i][j]`**; las filas 0 y 1 no cambian. No influye en `Analiza()` (P13) pero sí en los informes.

**Min/Max/Suma** (`:216-290`): sobre `SerieAciertos`; `0` si vacía.

*Texto* `Analiza(numRel)` (`:173-202`):

```
si !analizaEscaleras():  "Fallo en Escaleras ({nTotal})#"
sino si !analizaSandwichs(): "Fallo en Sandwichs ({sandwichs.count})#"
sino si !analizaPasoFijo():  "Fallo en Agrupaciones Paso Fijo#"
sino si !analizaSolapadas(): "Fallo en Agrupaciones Solapadas#"      // inalcanzable (P13)
```

**Estado global.** Ninguno.

**Peculiaridades.** P12, P13, P14, P15, P16: `ControladorRelacionesCP3.AnalizaRelaciones` llama a `ActualizaValores()` antes de cada `Analiza()` (§10), pero el camino de **texto** (`Analiza(numRel)`) no:
usa las cachés de la última evaluación (o las calcula la primera vez y las **congela**). Con la instancia `RelacionCP3(serie)` nunca debe llamarse a `ActualizaValores` (anularía la serie y fallaría con `columnas==nil`).
Los arrays permitidos creados desde el cargador/UI tienen longitud `max(L, máx)+1` con `L = K/3` (escaleras) o `K/4` (sándwiches) (`ES/FColProbablesData.cs:210,229,236-244`), suficiente para el máximo posible de escaleras (`≤ K/3`) y sándwiches (`≤ K/4`).
`ObtenArrayAgrupaciones` no valida los valores de `noAciertos` (≥15 ⇒ excepción en evaluación) ni `noElementos` negativos.

**Trampas Swift.** S3 (caché + `NoAC` vivo), S4, S5, S10, S11; la matriz como `[[Int32]]` con crecimiento por filas; **orden de condiciones en `Analiza` fijo** (el corte importa solo para excepciones).
No usar `Set` en `Temporales.Contains` (listas pequeñas; basta `contains`).

**Casos golden propuestos** (`SerieAciertos` fijada; instancia nueva por serie; salidas SIM):

| Id | Serie | Escaleras (orientación) | Sándwiches | Matriz paso fijo (no ceros `[longitud][valor]`) |
|----|-------|-------------------------|------------|-------------------------------------------------|
| G04-R3-01 | `3,4,5,5,2,7,7,3,3,3,0,1,2,3` | `[3,4,5]` A, `[0,1,2,3]` A | ninguno | `[1][0]=1 [1][1]=1 [1][2]=2 [1][3]=2 [1][4]=1 [2][5]=1 [2][7]=1 [3][3]=1`; solapada añade `[2][3]=2` |
| G04-R3-02 | `5,4,3,4,5,6,7` | `[5,4,3]` D, `[4,5,6,7]` A | ninguno | solo fila 1: `[1][3]=1 [1][4]=2 [1][5]=2 [1][6]=1 [1][7]=1` |
| G04-R3-03 | `2,3,3,2,9,2,3,3,2,2,3,3,2,1` | `[3,2,1]` D | `[2,3,3,2]`×2 | `[1][1]=1 [1][2]=4 [1][9]=1 [2][2]=1 [2][3]=3` |
| G04-R3-04 | `1,2,3,3,3,3,3,4` | `[1,2,3]` A | `[3,3,3,3]` | `[1][1]=1 [1][2]=1 [1][4]=1 [5][3]=1`; solapada `[2][3]=4 [3][3]=3 [4][3]=2` |
| G04-R3-05 | catorce `6` | ninguna | ninguno | `rows=15`, `[14][6]=1`; solapada `[k][6]=15-k` para `k=2…13` |
| G04-R3-06 | G04-R3-01 con asc=`"2"`, desc=`"0"`, total=`"2"` (L=4), sándwiches=`"0"` | `Analiza()=true` | — | — |
| G04-R3-07 | G04-R3-01 con paso fijo `"2+2+5,7"` / `"1+2+5,7"` | `true` / `false` (suma de `[2][5]+[2][7]` = 2) | — | — |
| G04-R3-08 | G04-R3-01 con solapadas `"99+3+3"` (`S[3][3]=1`, `99` no contiene 1) | `Analiza()` sigue `true` (P13) | — | — |

**Evidencia.** `MC/RelacionCP3.cs:24-676`; `MC/EscaleraAciertos.cs`, `MC/SandwichAciertos.cs`, `MC/AgrupacionColumnas.cs`; `ES/FColProbablesData.cs:206-262,714-751`;
`Free1X2/UI/Filtros/ColProbablesFrm.cs:3298-3340,4098-4106` (la UI replica el patrón «primeras K CPs»).

---

## 10. `ControladorRelacionesCP3` — `MC/ControladorRelacionesCP3.cs`

**Propósito.** Lista de `RelacionCP3`; todas deben cumplirse.

**Tipos y datos.** `relaciones: [RelacionCP3]` (`Relaciones` get/set). No guarda lista de CPs.

**Algoritmo.**

```
AnalizaRelaciones() -> Bool:                                     // :21-33
  for rel in relaciones: rel.ActualizaValores(); if !rel.Analiza() { return false }; return true
Analiza(&txt):                                                   // :35-43   -- SIN ActualizaValores (P16)
  txt = ""; for (i, rel): txt += rel.Analiza(i+1)
```

**Estado global.** Ninguno. **Peculiaridades.** P16. `FiltroColProbables.Analizar` solo lo invoca con `Count>0`; en el camino de texto
siempre se recorre.

**Trampas Swift.** S3 (reproducir exactamente quién invalida las cachés), S4.

**Casos golden propuestos.** G04-RC3-01: dos columnas distintas evaluadas en este orden con `AnalizaRelaciones` ⇒ las series se refrescan cada vez. G04-RC3-02: `Analiza(&txt)` llamado dos veces con CPs distintas
sin `AnalizaRelaciones` entre medias ⇒ el segundo texto usa la serie de la **primera** llamada (Origen: CALC; ratificar con GoldenGen, Abiertas A-5).

**Evidencia.** `MC/ControladorRelacionesCP3.cs:7-45`; `MC/RelacionCP3.cs:204-212`.

---

## 11. `RelacionGE1` — `MC/RelacionGE1.cs`

**Propósito.** Relación entre grupos de equipos (`GrupoEquipos`): la **suma** de victorias, empates, derrotas y puntos de los grupos indicados debe estar en rangos permitidos.
(El cálculo de cada `NoVictorias/NoEmpates/NoDerrotas/NoSumaPuntos` pertenece a `GrupoEquipos`, ESPEC_03; aquí solo se consumen.)

**Tipos y datos.** `strGEquipos` + `geRelacionados: [Int32]` (números de grupo **1-based**, por `RangosHelper.ObtenIntArray`, D2, **sin deduplicar**, `:173-183`);
`strSumaVictorias/Empates/Derrotas/Puntos` + `sumaVictorias/Empates/Derrotas/Puntos: [Bool]?` (D2 si `!= ""`, si no `nil`, `:185-259`);
`gruposEquipos: [GrupoEquipos]` (copiada de `controlador.GruposEquipos` al asignar `Controlador`, `:164-171`).

**Entradas / salidas.** Propiedades `GruposEquipos`, `SumaVictorias`, `SumaEmpates`, `SumaDerrotas`, `SumaPuntos` (parsean al asignar); `Analiza() -> Bool`; `Analiza(numRelacion) -> String`.

**Algoritmo.**

```
Analiza() -> Bool:                                               // :46-57
  if todas las cuatro cadenas == "": return true
  return analizaSumas()

analizaSumas() -> Bool:                                          // :70-119
  v = e = d = p = 0
  for n in geRelacionados: ge = gruposEquipos[n-1]; v += ge.NoVictorias; e += ge.NoEmpates; d += ge.NoDerrotas; p += ge.NoSumaPuntos
  if strSumaPuntos   != "" and (sumaPuntos.count   < p+1 or !sumaPuntos[p])   { return false }     // orden: puntos,
  if strSumaVictorias!= "" and (sumaVictorias.count < v+1 or !sumaVictorias[v]) { return false }   // victorias,
  if strSumaEmpates  != "" and (sumaEmpates.count  < e+1 or !sumaEmpates[e])   { return false }     // empates,
  if strSumaDerrotas != "" and (sumaDerrotas.count < d+1 or !sumaDerrotas[d])  { return false }     // derrotas
  return true
```

*Texto* `Analiza(numRelacion)` (`:59-68`, `:121-162`): mismas comprobaciones **sin cortar**, en el mismo orden, cada fallo añade:

```
"Fallo en Suma de Puntos de la relación {n}.  ({p})#"
"Fallo en Nº de Victorias de la relación {n}.  ({v})#"
"Fallo en Nº de Empates de la relación {n}.  ({e})#"
"Fallo en Nº de Derrotas de la relación {n}.  ({d})#"
```

(punto final tras `{n}` y dos espacios antes del paréntesis.)

**Estado global.** Ninguno. **Peculiaridades.** `GruposEquipos = ""` ⇒ `FormatException` (D2). Un nº de grupo inexistente ⇒ `ArgumentOutOfRange`. Persistencia: nodo `Relacion1` con hijos `ge`, `victorias`, `empates`,
`derrotas`, `puntos`, atributo `values` (`ES/FGruposEquiposData.cs:65-95,211-235`).

**Trampas Swift.** S4, S5, S11 (duplicados en `geRelacionados` suman dos veces).

**Casos golden propuestos.** GE1: `ge1=(v2,e1,d0,p7)`, `ge2=(v1,e0,d1,p3)`, `GruposEquipos="1-2"` (CALC):

| Id | Config | Esperado |
|----|--------|----------|
| G04-GE1-01 | `SumaVictorias="3"`, `SumaPuntos="10"` | `true` (v=3, p=10) |
| G04-GE1-02 | `SumaPuntos="9"` | `false`; texto `Fallo en Suma de Puntos de la relación 1.  (10)#` |
| G04-GE1-03 | todas vacías | `true`; texto `""` |
| G04-GE1-04 | `SumaPuntos="9"` y `SumaDerrotas="0"` | texto `Fallo en Suma de Puntos de la relación 1.  (10)#Fallo en Nº de Derrotas de la relación 1.  (1)#` |

**Evidencia.** `MC/RelacionGE1.cs:26-260`; `MC/FiltroGruposEquipos.cs:34-72`; `ES/FGruposEquiposData.cs:30-95`.

---

## 12. `ControladorRelacionesGE1` — `MC/ControladorRelacionesGE1.cs`

**Propósito.** Lista de `RelacionGE1`; todas deben cumplirse. Lo consume `FiltroGruposEquipos` (`MC/FiltroGruposEquipos.cs:36-60`).

**Tipos y datos.** `relaciones: [RelacionGE1]`, `gruposEquipos: [GrupoEquipos]` (get/set).

**Algoritmo.**

```
Analiza() -> Bool:      for rel in relaciones: if !rel.Analiza() { return false }; return true         // :36-46
AnalizaRelaciones() -> String:                                                                       // :48-57
  txt = ""; for (i, rel): txt = rel.Analiza(i+1)        // ASIGNA, no concatena  (P20): solo queda el texto de la ÚLTIMA relación
  return txt
Relaciones (set): asigna rel.Controlador = self a cada una                                          // :59-72
```

**Estado global.** Ninguno. **Peculiaridades.** P20; igual que §0.2: `filtro.GruposEquipos = lista` debe ejecutarse antes de `RelacionesGE1.Relaciones = …` (`ES/FGruposEquiposData.cs:60-61`).

**Trampas Swift.** S3, S4. **Casos golden.** G04-GE1-04 con dos relaciones donde la 1ª falla y la 2ª pasa ⇒ texto `""` (se pierde el fallo de la 1ª) (CALC).

**Evidencia.** `MC/ControladorRelacionesGE1.cs:26-79`.

---

## 13. `ControladorImpresion` — `MC/ControladorImpresion.cs`

**Propósito.** Registro de configuración de una impresora de boletos.

**Tipos y datos.** `margenSuperior: Int32` (0), `margenIzquierda: Int32` (0), `modelo: String?` (`nil`), `rotar: Bool` (`false`). Propiedades get/set (`:22-46`). Sin lógica ni igualdad de valor (`Equals` por referencia).
Consumo: `Free1X2/UI/imprimirBoleto.cs:53,119,568-573`; `WUI/ImprimirBoletoFrmViewModel.cs:43,112-115,224-225`.

**Entradas / salidas / algoritmo / estado global.** Nada: DTO. **Peculiaridades.** `controlador != nil ∧ modelo != nil` es la condición que usa la UI para saber si hay impresora elegida (`imprimirBoleto.cs:568`).

**Trampas Swift.** Si se usa `struct`, la comparación por valor cambia el comportamiento de `Contains` en §14 (P21): ver allí.

**Casos golden propuestos.** Ver §14. **Evidencia.** `MC/ControladorImpresion.cs:20-47`.

---

## 14. `ControladoresImpresion` — `MC/ControladoresImpresion.cs`

**Propósito.** Leer el catálogo de impresoras soportadas de `Impresion/impresoras.cfg`.

**Tipos y datos.** `configFile: XmlDocument` (cargado una vez en el constructor), `archConfig: String` = `{AppContext.BaseDirectory sin separador final}/Impresion/impresoras.cfg` (`:29-44`).

**Entradas / salidas.** `Impresoras -> [ControladorImpresion]` (cada lectura reconstruye la lista desde el documento en memoria, `:64-70`). Excepción de E/S si falta el fichero; la UI WinUI la captura (`WUI/ListaImpresorasViewModel.cs:80-90`) y además **ordena** la lista por `Modelo` con
`string.Compare(…, CurrentCulture)` (T11; fuera de `ControladoresImpresion`, el orden de salida de la clase es el del documento).

**Algoritmo.** `ObtenListaImpresorasSoportadas` (`:45-63`):

```
nodo = primer elemento llamado "impresoras" (GetElementsByTagName("impresoras")[0])
for hijo in nodo.ChildNodes (en orden):                          // el nombre del hijo no se comprueba ("Controladores")
   c.modelo = hijo.atributo["modelo"]; c.margenSuperior = ToInt32(hijo["ms"]); c.margenIzquierda = ToInt32(hijo["mi"])
   c.rotar = Convert.ToBoolean(hijo["girar"])        // "true"/"false" sin distinguir mayúsculas (cualquier otro valor: FormatException)
   if !lista.contains(c) { lista.append(c) }         // igualdad por referencia => SIEMPRE se añade (P21)
```

Un hijo no elemento (comentario) provoca `NullReference` (`Attributes` nulo). Fichero distribuido: 18 entradas, la 1ª `HP OFFICEJET 5510 ms=102 mi=32 girar=false`, la última
`Canon Fax-L295 Super G3 ms=98 mi=255 girar=true` (`Free1X2.WinUI/Datos/Impresion/impresoras.cfg:6-23`).

**Estado global.** `AppContext.BaseDirectory` (ruta). **Peculiaridades.** P21: no hay deduplicación; los duplicados del cfg se conservan; el orden es el del documento.

**Trampas Swift.** `XMLParser`/`XMLDocument` no equivalente: hay que ignorar nodos no elemento (el C# los rechaza) y leer los atributos con los mismos nombres; `Convert.ToBoolean` = comparación sin mayúsculas con `"true"/"false"` (S2 análogo);
`rotar`/`ms`/`mi` requeridos. Qué hace la versión Mac con el catálogo de impresoras (reutilizarlo o sustituirlo por la impresión nativa) es decisión del dueño (Abiertas A-8).

**Casos golden propuestos.** G04-IMP-01: parsear el `impresoras.cfg` distribuido ⇒ 18 entradas con los valores indicados (CALC con `grep -c Controladores`). G04-IMP-02: cfg con dos entradas idénticas ⇒ 2 elementos.

**Evidencia.** `MC/ControladoresImpresion.cs:24-71`; `Free1X2/UI/ListaImpresoras.cs:30-60`.

---

## 15. `FColProbablesData` — `ES/FColProbablesData.cs`

**Propósito.** Cargar y guardar la condición «Columnas probables» (nodo `<condicion id="ColProbables">` del `.comb`) hacia/desde `FiltroColProbables`.

**Tipos y datos.** Hereda de `FiltroDatosBase` (`PonerCondicionesFiltro(filtro, nodo)` / `EscribirCondicionesFiltros(filtro, writer)`).

**Estructura XML.**

```
<condicion id="ColProbables" activa="True|False">
  <cp id="0">                                   (uno por CP; el id se IGNORA al leer)
    <pronostico values="1,,1,X,1X2,,,,,,,,," />          14 tokens separados por coma; "" = ignorado
    <standard>
      <aciertos values="3A,4" />  <aciertosSeguidos values="0,1,2" />  <fallosSeguidos values="0,1" />  <puntos values="" />
    </standard>
    <tolerancias noTolerancias="0,1">           (solo si ToleranciaLocalActiva)
      <aciertosTol values="3" /> <aciertosSeguidosTol values="2" /> <fallosSeguidosTol values="1" />
    </tolerancias>
  </cp>
  <relacion1 id="0"> <columnas values="1-6"/> <sumas values="5-15"/> <recorridos values="0-3"/> <gruposCP cantidadCP="0-6" noAC="2,3,4,5"/> </relacion1>
  <relacion2 id="0"> columnasA, columnasB, aciertos (values) · concepto, cantidad (value)
                     columnasA2, columnasB2, aciertos2 (values) · concepto2, cantidad2 (value) </relacion2>
  <fallosCP id="0"> <columnas values=""/> <tolerancias values=""/> <aciertos values=""/> </fallosCP>
  <controlFallos noFallos="" />
  <relacion3 id="0"> columnas, sandwichs, escalerasAsc, escalerasDesc, escaleras (values) · concepto (values) ·
                     agrupacionesPasoFijo, agrupacionesSolapadas (values, separadas por '#') </relacion3>
</condicion>
```

Orden de escritura: `cp*`, `relacion1*`, `relacion2*`, `fallosCP*` + `controlFallos`, `relacion3*` (`:383-387`). Ojo al contraste: en `relacion2` `concepto/cantidad` usan el atributo **`value`**, en `relacion3` `concepto` usa **`values`**.

**Entradas / salidas.** Lectura: nodo XML → rellena el filtro. Escritura: `ContieneDatos` ⇒ escribe (`:377`). Sin ficheros propios.

**Algoritmo de lectura** (`:32-95`):

```
filtro.IsActive = ToBoolean(attr "activa"); filtro.ContieneDatos = true
arrayCP = []; rel1 = []; rel2 = []; rel3 = []; fallos = []; controlFallos = ""
for hijo in nodo.hijos (orden de documento):
   switch hijo.Name.ToUpper():                    // raíz: sin distinguir mayúsculas; los nombres de nodo hijo SÍ distinguen
     "CP":           arrayCP.append(obtenCP(hijo))
     "RELACION1":    rel1.append(obtenRelacionCP1(hijo))
     "FALLOSCP":     fallos.append(obtenCPCtrlFallos(hijo))
     "CONTROLFALLOS":controlFallos = attr "noFallos"
     "RELACION2":    r = obtenRelacionCP2(hijo); r.ColumnasProbables = arrayCP; rel2.append(r)      // lista viva
     "RELACION3":    rel3.append(obtenRelacionCP3(hijo, arrayCP))                                    // usa arrayCP TAL COMO ESTÁ (los CP deben ir antes)
filtro.ColProbables = arrayCP              // recalcula contieneDatos = arrayCP.count > 0   (P23: pisa el `true` inicial)
filtro.RelacionesCP1.Relaciones = rel1;  filtro.RelacionesCP2.Relaciones2 = rel2; filtro.RelacionesCP2.ColumnasProbables = filtro.ColProbables
filtro.RelacionesCP3.Relaciones = rel3;  filtro.ControlFallosCP.ControlesFallos = fallos; filtro.ControlFallosCP.FallosPermitidos = controlFallos
```

`obtenCP` (`:97-121`, `:292-340`): `pronostico@values` → `Pronosticos = obtenPronosticosCP(values)`; `standard` → `aciertos`→`SetNoAciertos`, `aciertosSeguidos`→`SetNoAciertosSeguidos`,
`fallosSeguidos`→`SetNoFallosSeguidos`, `puntos`→`SetPuntos`; `tolerancias` → `SetTolerancias(@noTolerancias)` y luego `aciertosTol`→`SetACTol`, `aciertosSeguidosTol`→`SetACSTol`,
`fallosSeguidosTol`→`SetFSTol` (comparación de nombre exacta, sensible a mayúsculas).
`obtenPronosticosCP` (`:342-370`): `values.split(",")` (**con vacíos**, S1); cada token se reescribe insertando una coma entre sus caracteres (`"1X"→"1,X"`, `""→""`) y se pasa a D1 (P6: un token
no canónico como `"X1"` → `"X,1"` → byte 0).
`obtenRelacionCP1` (`:123-153`): `columnas`→`Columnas`, `sumas`→`SumaAciertos`, `recorridos`→`Recorridos`, `gruposCP`→`CantidadCP = @cantidadCP; CuantosAC = @noAC`.
`obtenRelacionCP2` (`:154-205`): `columnasA/B/A2/B2` → `StrCols* = values` y `ColumnasX = D3 ObtenerListaFromTxt(values)`; `aciertos/aciertos2` → `Aciertos* = D3 ObtenerListaFromTxtAciertos(values)`; `concepto/cantidad/concepto2/cantidad2` → `@value`.
`obtenRelacionCP3(nodo, grupoCP)` (`:206-262`):

```
L = grupoCP.count / 3                                   // fijado al empezar (nº de CP leídas hasta ahora)
columnas:  lista = obtenerColumnasImplicadas(values, grupoCP); rel.Columnas = lista; rel.ColumnasImplicadasString = values
sandwichs: si lista != nil: rel.NumeroSandwichsPermitidosString = values; rel.NumeroSandwichsPermitidos = D3.BoolArray(values, lista.count / 4)
escalerasAsc/escalerasDesc/escaleras: D3.BoolArray(values, L)                      // "" => nil
concepto:  rel.Concepto = rel.ConceptoString = @values
agrupacionesPasoFijo / agrupacionesSolapadas: *String = values.split("#"); *Permitidas = obtenArrayAgrupaciones(strings, grupoCP.count + 1)
```

`obtenerColumnasImplicadasRelCP3` (`:714-723`, **P12**): `indices = D3 ObtenerListaFromTxt(values)`; devuelve `[grupoCP[0], grupoCP[1], …, grupoCP[indices.count-1]]` — **usa `i`, no `indices[i]`**
(ArgumentOutOfRange si hay menos CPs). `obtenArrayAgrupaciones(datos, max)` (`:724-751`): por cada token, `partes = token.split("+", omitEmpty: true)`; si `partes.count == 3`:
`numero = D3.ListaAciertos(partes[0])`, `elementos = ToInt32(partes[1])`, `aciertos = D3.ListaAciertos(partes[2])`; se añade `AgrupacionColumnas(numero, elementos, aciertos)` solo si `elementos < max ∧ aciertos.count <= 15`;
si no queda ninguna ⇒ `nil`.
`obtenCPCrtlFallos` (`:264-290`): `columnas`→`Columnas`, `tolerancias`→`Tolerancias`, `aciertos`→`Aciertos` (todos `@values`; cada uno parsea al asignar).

**Algoritmo de escritura** (`:372-712`): CP: `<cp id=i>` + `pronostico` (`Pronosticos` con comas eliminadas, unidos por `,`) + `standard` (`GetAciertos/GetAciertosSeguidos/GetFallosSeguidos/GetPuntos`) + `tolerancias`
(solo si `ToleranciaLocalActiva`: `GetTolerancias`, `GetACTol`, `GetACSTol`, `GetFSTol`). Relaciones y `fallosCP`: atributos tomados de las propiedades de texto (`Columnas`, `SumaAciertos`, `Recorridos`, `CantidadCP`,
`CuantosAC`, `StrCols*`, `StrAciertos*`, `Concepto*`, `Cantidad*`; en `relacion3` las `*String`: `ColumnasImplicadasString`, `NumeroSandwichsPermitidosString`, `NumeroEscalerasASCPermitidasString`, etc.); `controlFallos@noFallos = FallosPermitidos`. `agrupaciones*`: cadenas no nulas unidas con `#`; si la matriz es nula, `values=""`.

**Estado global.** `NumeroPartidos` indirectamente (tamaños de los arrays de `ColumnaProbable`).

**Peculiaridades.** P6, P12, P23, P24. Errores de lectura que el C# no captura aquí: atributo obligatorio ausente (`NullReference`); `SetNoAciertos("")`, `SetACTol("")`, `SetTolerancias("")`, `Columnas=""` ⇒ excepción
(el escritor *puede* emitir `values=""`; ver Abiertas A-3/A-4). `fallosCP` o `relacion1` con `columnas=""` no deberían guardarse (la UI filtra `rel.Columnas != ""`, `Free1X2/UI/Filtros/ColProbablesFrm.cs:1066,3250`).

**Trampas Swift.** S1 (clave), S2, S3 (la `arrayCP` viva), S4, S8 (comparar `Name.uppercased()` solo para el nivel raíz), orden de montaje §0.2. Escritura: ver T8 (finales de línea del serializador XML, ESPEC_07).

**Casos golden propuestos.**

| Id | Entrada | Esperado | Origen |
|----|---------|----------|--------|
| G04-XML-01 | El XML del recuadro superior (1 CP + relacion1) | CP0: `pronosticos=[4,0,4,2,7,0×9]`, `AC{3 'A', 4 ' '}`, `ACS{0,1,2}`, `FS{0,1}`; `filtro.ContieneDatos=true` | CALC |
| G04-XML-02 | XML sin ningún `<cp>` pero con `activa="True"` | `IsActive=true`, `ContieneDatos=false` (P23) | CALC |
| G04-XML-03 | `relacion3` con `columnas values="3,1"` y 6 CPs | `Columnas` = las CPs 0 y 1 (P12) | CALC |
| G04-XML-04 | `agrupacionesPasoFijo values="1,2+3+4,5#bad#2+2+7"` con 14 CPs | 2 agrupaciones: `([1,2],3,[4,5])`, `([2],2,[7])` | CALC |
| G04-XML-05 | `pronostico values="X1,21,1X"` | bytes `[0,0,6]` | CALC |
| G04-XML-06 | ida y vuelta (leer → escribir) de G04-XML-01 | idéntico salvo normalización ascendente de las listas | CALC |

**Evidencia.** `ES/FColProbablesData.cs:28-755`; `ES/FiltroDatosBase.cs:27-47`; `UT/UtilidadesEntradasValores.cs:42-281`.

---

## 16. Generador de CPs — `GCP/*`

**Propósito.** A partir de una **valoración** (probabilidad o valor por signo y partido) y de una **configuración** de tipos de CP, producir las columnas probables como matrices de pronósticos por partido, y escribirlas en `Condiciones/{tipo}_j{jornada}.txt`.
(Pantalla: `Free1X2/UI/GenerarCPs.cs:345-383`, `WUI/GenerarCPsViewModel.cs:85-135`.)

**Tipos y datos.**

- `Valoracion { uno, equis, dos: Double }` (`GCP/Valoracion.cs:7-31`). `valores: [Valoracion]` de `N` elementos (por partido).
- Configuración (DataSet tipado `ColumnasProbables`, `GCP/ColumnasProbables.cs`; esquema `GCP/ColumnasProbables.xsd`): tabla `Tipos de CPs` (`Tipo: Int32` PK, `Nombre: String`) y tabla `Configuracion de CPs`
  (`Tipo: Int32` FK, `Desde: Int32`, `Hasta: Int32`, `Forzar Fijos: Bool`, `Num Fijos: Int32`, `Forzar Dobles: Bool`, `Num Dobles: Int32`, `Forzar Triples: Bool`; orden de columnas = índices 0…7, `ColumnasProbables.cs:278-280,560-574`),
  persistida en `{BaseDirectory}/ColumnasProbables.xml` (`GCP/DatosHelper.cs:16-35`): se lee con `ReadXml(Auto)` **solo si existe**, se escribe sobrescribiendo; atributos con espacios codificados `_x0020_` y espacio de nombres `http://tempuri.org/ColumnasProbables.xsd`.
  Fichero distribuido (`Free1X2.WinUI/Datos/ColumnasProbables.xml`): tipo 1 «Normal» y **dos** filas de configuración; solo la 2ª tiene `Tipo=1` (`Desde=1 Hasta=100 Forzar Fijos Num Fijos=14`), la 1ª carece de `Tipo` y el filtro `Tipo = 1` no la incluye.

**Entradas / salidas.** `CPs.CrearCPs(vista, valores) -> String?[N][M]` (fila = partido, columna = CP; `M` = filas de la vista); `ObtenerFijos`, `ObtenerDobles`, `ObtenerTodosFijos`, `ObtenerTodosDobles`, `OrdenarValoracion`.
Los pasos de la pantalla (`LlenarDataset`, `LlenarTxtColumnas`, `IO.GuardarTexto`) están en §«Salida a fichero».

**Algoritmo.**

```
CrearCPs(vista, valor):                                          // GCP/CPs.cs:16-90
  M = vista.count;  columnas = String?[N][M] (todo nil)
  for j in 0..<M:
     fila = vista[j]        // en el orden de las filas de la tabla filtradas por «Tipo = x» (sin ordenar)
     tipo = 3
     if fila[3].texto != "" and ToBoolean(fila[3]): tipo = 1            // Forzar Fijos
     if fila[5].texto != "" and ToBoolean(fila[5]): tipo = 2            // Forzar Dobles (pisa a Fijos)
     desde = Int16(fila[1]); hasta = Int16(fila[2])
     switch tipo:
       1: tmp = ObtenerFijos(Int16(fila[4]), valor);  columnas[k][j] = tmp[k]  para k en 0..<tmp.count
       2: tmp = ObtenerDobles(Int16(fila[6]), valor); columnas[k][j] = tmp[k]
       3: for k in 0..<valor.count:
             s = ""
             if desde <= valor[k].uno   <= hasta { s += "1" }
             if desde <= valor[k].equis <= hasta { s += "X" }
             if desde <= valor[k].dos   <= hasta { s += "2" }
             triple = fila[7].texto != "" ? ToBoolean(fila[7]) : false
             if s == "" and triple { s = "1X2" }
             columnas[k][j] = s                                          // "1X","12","X2","1X2": orden canónico de D1
```

`OrdenarValoracion(valor)` (`:207-218`): `m = [Double](N*3)`; `m[3i]=uno, m[3i+1]=equis, m[3i+2]=dos`; ordena ascendente (`Array.Sort`; los empates son valores iguales, indistinguibles ⇒ el orden de empate no importa; el comportamiento de `Array.Sort` con `NaN` es BCL, no verificado: Abiertas A-6/A-9).

```
ObtenerTodosFijos(valor):                                        // :138-148
  signos = String?[N] (nil)
  para i: if uno > equis ∧ uno > dos { "1" }; if equis > uno ∧ equis > dos { "X" }; if dos > equis ∧ dos > uno { "2" }   // estrictos: empate de máximos => nil
ObtenerTodosDobles(valor):                                       // :150-160
  para i: if uno < equis ∧ uno < dos { "X2" }; if equis < uno ∧ equis < dos { "12" }; if dos < equis ∧ dos < uno { "1X" }   // el signo estrictamente MENOR se excluye; empate => nil

ObtenerFijos(max, valor):                                        // :92-136
  si max == N: return ObtenerTodosFijos(valor)
  tmp = copia profunda de valor[0..<N]                            // valor.count < N => IndexOutOfRange
  m = OrdenarValoracion(tmp) invertido (DESCENDENTE)
  signos = String?[N] (nil)
  for i in 0..<max:
     for j in 0..<N:                                              // primer partido (índice menor) que contenga el valor m[i] en cualquiera de sus 3 casillas
        if tmp[j].uno   == m[i] { signos[j] = "1"; tmp[j].uno   = 0; break }
        if tmp[j].equis == m[i] { signos[j] = "X"; tmp[j].equis = 0; break }
        if tmp[j].dos   == m[i] { signos[j] = "2"; tmp[j].dos   = 0; break }
  return signos                                                   // un partido puede ser reasignado por un valor posterior (P22)

ObtenerDobles(max, valor):                                       // :162-205
  si max == N: return ObtenerTodosDobles(valor)
  tmp = copia profunda; m = OrdenarValoracion(tmp) ASCENDENTE
  for i in 0..<max: for j in 0..<N:
        if tmp[j].uno   == m[i] { signos[j] = "X2"; tmp[j].uno   = 99.99; break }
        if tmp[j].equis == m[i] { signos[j] = "12"; tmp[j].equis = 99.99; break }
        if tmp[j].dos   == m[i] { signos[j] = "1X"; tmp[j].dos   = 99.99; break }
```

**Salida a fichero** (WinForms `GenerarCPs.cs:345-383,392-452`; WinUI `GenerarCPsViewModel.cs:85-135,239-330`, misma lógica): para cada fila de `Tipos de CPs`: filtra `Configuracion de CPs` por `Tipo = x`, `CrearCPs`, y escribe un
fichero con **una línea por CP** (`columnas[0][j],…,columnas[N-1][j]`, `nil` ⇒ `" "`), cada línea terminada en `\r\n`; el contenido completo se escribe con `StreamWriter.WriteLine`, que **añade un terminador de línea final adicional** (`GCP/IO.cs:23-31`).
Ruta `{BaseDirectory}/Condiciones/{Nombre}_j{jornada}.txt`; el `fichero.Replace(" ", "_")` descarta su resultado (no reasigna), de modo que **los espacios del nombre se conservan** (P22). Jornada ≤ 0 aborta (`btnOK_Click`).

**Estado global.** `VariablesGlobales.NumeroPartidos` (tamaños y límites: `GCP/CPs.cs:32-33,94,101,140,152,164,171,209`).

**Peculiaridades (P22).** (a) En `ObtenerFijos`/`ObtenerDobles` el mismo valor numérico puede estar en varios partidos: gana el de menor índice; (b) un partido puede recibir varias asignaciones (la última sobrescribe) y por tanto
quedar `max` o menos partidos asignados; (c) las casillas ya consumidas se ponen a `0` (fijos) o `99.99` (dobles) y un valor real igual a esos centinelas se confunde con ellas; (d) los partidos sin asignar son `nil`, no `""`;
(e) el tipo «Dobles» tiene prioridad sobre «Fijos» si ambos están marcados; (f) `desde/hasta` son `Int16` pero se comparan como `Double`; (g) `miDataView[j][c]` con `DBNull` en `Desde/Hasta/NumFijos` ⇒ `InvalidCastException`.
Código muerto sin llamadores: `IO.LeeColumna`, `DarFormatoColumna`, `ColumnaAPartidos`, `ContarValoracion`, `LeeLineaValoracion` (búsqueda en el repo: solo se usan `GuardarTexto` y `Cerrar`).

**Trampas Swift.** T3 no aplica (todo `Double`); T5: `sort()` de `[Double]` con empates no distingue elementos (seguro) pero **no** admite NaN; T7: la valoración se obtiene con `Convert.ToDouble`/`Replace(".", ",")`
dependiente de cultura en `Porcentajes` (ESPEC_06) — fijar `Locale` al importar; S1; `99.99` y `0` como `Double` literales; copia profunda de `tmp` (clase vs struct: usar `struct Valoracion`).

**Casos golden propuestos** (N = 4 por legibilidad; el algoritmo no depende de N; valoración `p0=(50,30,20) p1=(40,45,15) p2=(10,20,70) p3=(33,33,34)`; SIM):

| Id | Llamada | Esperado |
|----|---------|----------|
| G04-GEN-01 | `ObtenerFijos(2)` | `["1", nil, "2", nil]` |
| G04-GEN-02 | `ObtenerFijos(3)` | `["1","X","2", nil]` |
| G04-GEN-03 | `ObtenerFijos(4)` (todos) | `["1","X","2","2"]` |
| G04-GEN-04 | `ObtenerDobles(1)`, `(2)`, `(3)` | `[nil,nil,"X2",nil]`, `[nil,"1X","X2",nil]`, `["1X","1X","X2",nil]` |
| G04-GEN-05 | `ObtenerDobles(4)` (todos) | `["1X","1X","X2", nil]` (p3 empata ⇒ nil) |
| G04-GEN-06 | Rango `Desde=30 Hasta=45`, sin/con `Forzar Triples` | `["X","1X","","1X2"]` / `["X","1X","1X2","1X2"]` |
| G04-GEN-07 | Sobrescritura: `p0=(60,55,5) p1=(10,20,80) p2=(30,40,30) p3=(34,33,33)`, `ObtenerFijos(3)` | `["X","2",nil,nil]` (p0 recibe «1» y luego «X») |
| G04-GEN-08 | Configuración distribuida (`Tipo 1`, 14 fijos) con N=14 | 1 CP = `ObtenerTodosFijos` |
| G04-GEN-09 | Fichero de salida con una CP `["1",nil,"X2"]` (N=3 ilustrativo) | `1, ,X2\r\n` + terminador final de `WriteLine` |

**Evidencia.** `GCP/CPs.cs:10-220`; `GCP/Valoracion.cs`; `GCP/IO.cs:11-165`; `GCP/DatosHelper.cs:9-36`; `GCP/ColumnasProbables.cs:278-280,560-574`; `GCP/ColumnasProbables.xsd`;
`Free1X2/UI/GenerarCPs.cs:317-332,345-452`; `WUI/GenerarCPsViewModel.cs:85-135,239-330`; `Free1X2.WinUI/Datos/ColumnasProbables.xml`.

---

## Abiertas

| Id | Pregunta / riesgo | Referencia |
|----|-------------------|------------|
| A-1 | **Política de errores** (R2, decisión del dueño): el C# lanza excepciones en configuraciones inválidas (listas vacías, `Columnas=""`, índices fuera de rango, rangos mixtos). Propuesta: validar en *set-time* con `throws` y, en evaluación, acceso seguro. Con ella Swift **no** reproduce los fallos del C# (solo diverge en entradas que el C# rechaza). | §0.7 S4 |
| A-2 | `NumeroPartidos = 15` (valor leído de `parametros.free1x2`, `ES/AConfiguracion.cs:153-159`; `UT/UtilColumnas.cs:184` prevé hasta 15): `RelacionCP1` (búferes de 15, `recorridos[15]`) y `RelacionCP3` (matriz de 15 columnas) fallan con `noAC = 15`. ¿Se soporta 15 en la versión Mac? | P9, §5, §9 |
| A-3 | El escritor puede emitir `values=""` en `standard/aciertos`, `aciertosSeguidos` o `fallosSeguidos` y el lector lanza (`SetNoAciertos("")` ⇒ `str[-1]`). No verificado si la UI impide guardar una CP sin AC/ACS/FS ni si `ArchivoCombinacion` captura la excepción. | `ES/FColProbablesData.cs:292-316,661-688` |
| A-4 | Idem para `tolerancias`: `SetACTol("")`/`SetTolerancias("")` lanzan, pero el escritor emite siempre los tres hijos (`:690-712`). | `MC/ColumnaProbable.cs:470-511` |
| A-5 | Caché obsoleta de `RelacionCP3` en el camino de texto (P16): se documenta como se comporta hoy; ¿replicar (golden R1) o corregir llamando a `ActualizaValores` (decisión del dueño)? | §9, §10 |
| A-6 | Comportamiento BCL no verificable en el repo y que condiciona el parseo: `Convert.ToInt32` (espacios, signo, cultura), `Char.IsLetter`, `string.ToUpper()`, `Convert.ToBoolean`, `Array.Sort` con NaN. Proponer casos GoldenGen que fijen cada uno. | D4, S2, S6, S7 |
| A-7 | `RelacionCP3` generada por la UI/informe ABDON: `Columnas = cols` se asigna directamente (no `grupoCP[i]`) en `VisorAnalisisColumnasAbdonFrm.cs:602-606` y su ViewModel (`:765`); no se ha comprobado si ahí la lista puede no ser el prefijo de las CPs. Afecta solo a quien construya relaciones fuera de la carga XML. | P12 |
| A-8 | Unidad de `ms`/`mi` en `impresoras.cfg` (la geometría de `ImprimirBoletoFrmViewModel.cs:224-250` usa constantes como 19,685 y 44,291, compatibles con centésimas de pulgada, **no verificado**). Decidir si macOS reutiliza el catálogo de impresoras o lo sustituye por `NSPrintInfo`. | §13, §14 |
| A-9 | `OrdenarValoracion` con valoraciones `NaN` (p. ej. una celda con «NaN» importada): el orden de `Array.Sort` de C# (NaN primero) no es reproducible con `sort()` de Swift sin comparador total. ¿Se acepta rechazar NaN al importar? | §16 |
| A-10 | `IO.LeeLineaValoracion` y los demás métodos de `IO`/`GCP` listados son código muerto. Si se portan por completitud: su parseo supone separador `", "` (el `Replace(".", ",")` no tiene efecto) y depende de la cultura. | `GCP/IO.cs:114-158` |
| A-11 | `NumeroPartidos` puede cambiar tras construir una `ColumnaProbable`: sus arrays usan el valor de construcción pero `Analizar`/`Pronosticos` leen el valor vivo (`MC/ColumnaProbable.cs:27,70,276,697`). ¿Se garantiza que no cambia en la vida del objeto? (`docs/MANUAL_FLUJOS.md:335` exige cambiarlo antes de instanciar el motor.) | §1 |
| A-12 | `ControladorRelacionesCP1.Relaciones.Add(rel)` directo (p. ej. `VisorAnalisisColumnasAbdonFrm.cs:598`) deja `columnasProbables` sin asignar en la relación: solo se usa para serializar. Confirmar que nunca se evalúa. | §6 |
| A-13 | `tools/GoldenGen` (citado en ESPEC_00 §2) no existe aún en el repo: `tools/` solo contiene `apply_theme_batch.py` y `compact_winui_pages.py`. Los esperados marcados `SIM`/`CALC` deben generarse con él antes de darse por buenos. | cabecera |
| A-14 | `RelacionGE1` depende de `GrupoEquipos.NoVictorias/NoEmpates/NoDerrotas/NoSumaPuntos` (`MC/GrupoEquipos.cs:55-160`, `:327-340`), especificado en ESPEC_03; no se ha reverificado aquí su cálculo. | §11 |
