# ESPEC 05 — Reducción, subir categoría y escrutinio

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Sigue la plantilla y las trampas T1–T12
> de [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Objetivo: reescribir en Swift **sin mirar el C#**
> y obtener resultados idénticos (M-R1). El C# es el oráculo.
>
> Alcance: `Free1X2.Domain/Reduccion/*` (9 ficheros), `Free1X2.Domain/SubirCategoria/Calculos.cs`,
> `Free1X2.Domain/Escrutinio/*` (7 ficheros) y los tests/fixtures de `Free1X2.Domain.Tests/`.

## Método y grado de confianza

- Cada algoritmo se ha leído **entero**. Además se re-implementaron en un script Python independiente (fuera del repo):
  - `JDCDobleContador` y `ReductorTM` reproducen **exactamente** los golden existentes (JDCDoble: los 5 casos, incluidos 2 SHA-256 y el de
    «externas»; ReductorTM: los 4 casos de salida exacta). El pseudocódigo de esos dos queda, por tanto, **validado contra el oráculo**.
  - `Escrutador` reproduce el histograma golden de `cols_a` (`EscrutinioReduccionFixtureTests.cs:46-53`).
  - `JDC`, `Redu1305Xfsf`, `XFSF`, `xfsfV3`, `JLPM`, `Calculos`: sin golden de salida exacta en el repo; los valores marcados **[réplica]**
    salen de la réplica y **no son oráculo** (entran como `modelo`: cada uno necesita su caso `lectura` de contraste antes de usarse como aceptación, ESPEC_12 §1).
- Toda cifra de línea (`fichero:N`) se verificó leyendo el fichero. Lo no verificado está en **«Abiertas»** (final).

**Abreviaturas de ruta** (todas relativas a la raíz del repo):

| Abrev. | Ruta |
|---|---|
| `JDC.cs`, `JDCdoble.cs` (= `JDCdobleContador.cs`), `TM.cs` (= `ReductorTM.cs`), `R1305.cs` (= `Redu1305Xfsf.cs`), `XFSF.cs`, `V3.cs` (= `xfsfV3.cs`), `JLPM.cs`, `Base.cs` (= `ReductorBase.cs`), `IRed.cs` (= `IReductor.cs`) | `Free1X2.Domain/Reduccion/<fichero>` |
| `Calculos.cs` | `Free1X2.Domain/SubirCategoria/Calculos.cs` |
| `Escrutador.cs`, `EscComb.cs` (= `EscrutadorComb.cs`), `ColPrem.cs` (= `ColumnasPremiadas.cs`), `ColPremComb.cs`, `ListaPremComb.cs`, `PPCmp.cs` (= `PosiblesPremiosComparer.cs`), `PPCont.cs` (= `PosiblesPremiosContenedor.cs`) | `Free1X2.Domain/Escrutinio/<fichero>` |
| `ACT.cs`, `Conv.cs`, `Comp.cs`, `UtilCol.cs` | `Free1X2.Domain/EntradaSalida/ArchivoColumnasTexto.cs`, `Free1X2.Domain/Utils/ConvertidorDeBases.cs`, `…/Utils/Comparador.cs`, `…/Utils/UtilColumnas.cs` |
| `T-JDCD`, `T-TM`, `T-FIX`, `T-FLU` | `Free1X2.Domain.Tests/JDCDobleContadorGoldenTests.cs`, `ReductorTMGoldenTests.cs`, `EscrutinioReduccionFixtureTests.cs`, `FlujosClaveTests.cs` |
| `VM-Red`, `VM-Esc`, `VM-EscC`, `VM-Sub` | `Free1X2.WinUI/Views/Ported/ReductorFrmViewModel.cs`, `EscrutiniosFrmViewModel.cs`, `EscrutarCombinacionesFrmViewModel.cs`, `SubirCategoriaFrmViewModel.cs` |

---

## 0. Mapa de componentes

| # | Componente (Swift propuesto) | Etiqueta en UI | Uso real | Golden existente | Prioridad port |
|---|---|---|---|---|---|
| 1 | `Base` + `IReduccion` → `protocol Reductor` | — | contrato de todos | indirecto | P1 |
| 2 | `JDC` → `ReductorJDC` | «menos tiempo» (defecto) | sí (`VM-Red:177`) | solo propiedades | P1 |
| 3 | `JDCDobleContador` → `ReductorJDCDobleContador` | «menos columnas» | sí (`VM-Red:178`) | **5 exactos** | P1 |
| 4 | `ReductorTM` | «menos columnas 2» | el `switch` lo instancia (`VM-Red:179`) pero **el combo no lo ofrece** (`VM-Red:43-44`; legacy: comentado, `Free1X2/UI/ReductorFrm.cs:147`) ⇒ hoy inalcanzable desde la UI; con tests propios | **4 exactos + 2 de comportamiento** (N-02, N-03) | P1 |
| 5 | `Redu1305Xfsf` → `ReductorGrandesArchivos` | «grandes archivos» | sí (`VM-Red:176`) | ninguno | P1 |
| 6 | `XFSF` | — | **huérfano** (0 instanciaciones) | ninguno | P4 |
| 7 | `XfsfV3` | — | **huérfano** | ninguno | P4 |
| 8 | `JLPM` | — | **huérfano**, puede no terminar | ninguno | P4 |
| 9 | `SubirCategoria.Calculos` → `SubidaCategoria` | pantalla «Subir categoría» | sí (`VM-Sub:347,270,310`) | ninguno | P2 |
| 10 | `Escrutador` | Escrutinios / Banco de pruebas | sí (`VM-Esc:584-606,732-747`) | 2 tests (histograma) | P1 |
| 11 | `EscrutadorComb` | Escrutar combinaciones | sí (`VM-EscC:430-452`) | **ninguno** y con defectos probables (**Abierta A1**) | P3 (bloqueado) |
| 12 | `ColumnasPremiadas`, `ColumnasPremiadasComb`, `ListaPremiadasComb` | listas de premiadas | DTOs | — | P1 |
| 13 | `PosiblesPremiosContenedor` + `PosiblesPremiosComparer` | Posibles premios / Mejores opciones | sí (`Free1X2/UI/PosiblesPremiosFrm.cs:3393`, `Free1X2/UI/MejoresOpcionesFrm.cs:533`, `Free1X2.WinUI/Views/Ported/PosiblesPremiosFrmViewModel.cs:247`) | ninguno | P2 |

«Huérfano» = `grep` de `new XFSF|XfsfV3|JLPM` sin resultados en `Free1X2*/`: ni la UI WinUI (`VM-Red:174-181`) ni la legacy (`Free1X2/UI/ReductorFrm.cs:66-82`) los instancian.
Propuesta (decisión del dueño, R2): **no portarlos**; quedan documentados por completitud.

---

## 1. Contratos comunes

### 1.1 `IReduccion` / `Base`

- Interfaz (`IRed.cs:22-31`): `NoColumnasIniciales`, `NoColumnasFinales`, `NoColumnasProcesadas` (Int32), `Cancelar()`,
  `ComienzaReduccion(entrada, salida, nivelReduccion, maxCol, percent)`, `Inicializa(entrada, nivelReduccion)`.
- `Base` (`Base.cs:22-49`): campos `noColumnasIniciales/Finales/Procesadas` y `bool salida`; `Cancelar()` pone `salida = true` y **nadie lo repone** salvo
  `Redu1305Xfsf` (`R1305.cs:71,115`). Plantilla `ComienzaReduccion = EntradaDeDatos → Reduce → GrabacionDeReductoras`
  (`JDC.cs:63-71`, `JDCdoble.cs:84-92`, `XFSF.cs:45-53`, `V3.cs:41-49`); `JLPM` fuerza `Reduce(nivel, 0, 100)` (`JLPM.cs:41`); `Redu1305Xfsf` bifurca (`R1305.cs:55-62`);
  `ReductorTM` es distinto (§2.3).
- `Inicializa` es **no-op** en todos salvo `ReductorTM` (`JDC.cs:255-257`, `JDCdoble.cs:349-351`, `R1305.cs:250-252`, `XFSF.cs:124-126`, `V3.cs:154-156`, `JLPM.cs:178-180`).
- **Semántica de parámetros** (común donde aplica):
  - `nivel` = aciertos mínimos que se quieren garantizar; radio de cobertura `d = noPartidos − nivel` (distancia de Hamming máxima).
  - `maxCol`: `0` = sin límite (se sustituye por `noColumnasIniciales`, `JDC.cs:81`). Corte por **igualdad** `noColumnasFinales == maxCol` (`JDC.cs:136`,
    `JDCdoble.cs:197`, `V3.cs:120`); `Redu1305Xfsf` usa `>=` (`R1305.cs:91,106,135,150`); `XFSF` usa una cuenta atrás `nfin` (`XFSF.cs:73-74,85,104`); `ReductorTM`, `JLPM` lo **ignoran**.
  - `percent`: corte cuando `noColumnasProcesadas*100/noColumnasIniciales ≥ percent` (división entera). `ReductorTM` y `JLPM` lo ignoran.
- **Cómo lo invoca la app** (`VM-Red`): `nivel ∈ {13…1}` (`:36-37`); `maxCol` por defecto 0 y `percent` por defecto 100 (`:71-77`);
  **instancia nueva por ejecución** (`:174-181`) con `externas = AdmitirColumnasExternas`; entrada validada: se debe poder leer entera y tener **exactamente 14 signos**
  (`:114-130`); se ejecuta en hilo de fondo (`:208-209`); los contadores se muestran cada 3 s (`:197-204`) con `% = procesadas*100/iniciales` (`:235`).
- **Contadores** (reutilizar una instancia **acumula** salvo donde se indica):

| Reductor | `Iniciales` | `Finales` | `Procesadas` |
|---|---|---|---|
| JDC | nº de líneas, **con duplicados** (`JDC.cs:234`) | `++` por reductora; **no se reinicia** (`:126`) | negativo `nr − N` durante la fase 1 (`:87`), `0` al empezar la fase 2 (`:91`), `++` por columna de la madre reducida y por reductora-miembro (`:128-130,202`) |
| JDCdoble | ídem (`JDCdoble.cs:327`) | `++` (`:191`), no se reinicia | negativo en fase 1 (`:117`), `0` (`:125`), `++` (`:188,295`) |
| ReductorTM | nº de líneas (`TM.cs:134`) | se reinicia a 0 (`:226`), `++` (`:272,298`) | `N − menor` (`:249`); **`= N` al final** (`:359`) |
| Redu1305Xfsf | nº de líneas (`R1305.cs:233`) | se reinicia (`:72,116`) | se reinicia; `++` (`:83,103,171,195`) |
| XFSF | `:60-64` | `++` (`:85`) | `++` por reductora y por reducida (`:85,93`) |
| V3 | `:129-133` | `++` (`:99`) | negativo en fase 1 (`:77`), `0` (`:81`), `++` (`:99,111`) |
| JLPM | `:161` | `++` (`:119`) | **nunca se asigna (siempre 0)** |

### 1.2 Codificación de columnas y E/S que usan estos módulos

(El detalle de formatos está en ESPEC_06/07; aquí solo el contrato que estos algoritmos **dependen** de cumplir.)

- **Índice base-3** (`Conv.cs:43-54`): para cada posición `i` (0 = partido 1 = dígito menos significativo): `'1'→0`, `'2'→2·3^i`, **cualquier otro carácter → 3^i** (cuenta como `X`,
  incluidos `,` y `*`). La versión con `numPartidos` explícito lee solo esos primeros caracteres (`:55-66`). Máx. 17 posiciones (tablas de 17 potencias, `:28-29`).
  El **orden numérico** del índice ordena por el **último partido primero**.
- **Inverso** (`Conv.cs:96-117`): dígito `d → "1X2"[d]` empezando por el menos significativo; relleno con `'1'` hasta `_NumPartidos`, que la
  constructora sin argumentos toma de **`VariablesGlobales.NumeroPartidos` en el momento de construir** (`:34-37`). Si el índice tiene más dígitos que `_NumPartidos`
  `Append('1', negativo)` lanza excepción (fuera de contrato). ⇒ **La anchura de las líneas de salida de JDC/JDCdoble/Redu1305 es
  `NumeroPartidos`, no el nº de signos de la entrada** (`JDC.cs:242`, `JDCdoble.cs:336`, `R1305.cs:242`; preservar: pasar `Sesion.numeroPartidos` como parámetro de formato). `Calculos.Grabar` es la excepción: usa el `noPartidos` recibido en `Calcular` (`Calculos.cs:75`, `ACT.cs:156-166`).
- **Lectura** (`ACT.cs`): `SiguienteColumna()` = `Peek() ≥ 0` (`:183-200`) ⇒ **las líneas en blanco intermedias o finales extra NO se saltan**: cada una es una columna
  de longitud 0 (en JDC/JDCdoble/Redu1305 equivale al índice 0 = `1111…`; en `ReductorTM` provoca excepción; en `Escrutador` suma un `0 aciertos`). El mensaje de la UI lo avisa
  (`VM-Red:144-145`). `LeeColumnaSinComas()` = `ReadLine().Trim().ToUpper()` (`:214-216`): **no quita comas** pese al nombre. Terminadores de línea .NET `\r\n`, `\n`, `\r`;
  UTF-8 con detección de BOM (`:191`). `ObtenNumSignos()` = longitud de la 1.ª línea ya recortada (0 si el fichero está vacío) (`:342-366`).
- **Escritura** (`ACT.cs:65-80`): el fichero **se crea al escribir la primera línea** (truncando); si no se escribe ninguna, **no se crea ni se toca** el fichero. `GuardarCols` quita comas
  y usa `WriteLine` (`Environment.NewLine`: `\r\n` en Windows, **`\n` en macOS/Linux**, T8); UTF-8 **sin BOM**.
- `UtilColumnas.ContarBitsA1` = `popcount(UInt64(b))` (`UtilCol.cs:299-309`); `UtilColumnas.ObtenNoCoincidencias(a, b)` = nº de posiciones `i < a.Length` con `a[i]==b[i]`
  (lanza si `b` es más corta) (`UtilCol.cs:348-367`).

### 1.3 Vecindario de Hamming y recorrido canónico

Todos los reductores de «3^n» recorren los vecinos de una columna `c` a distancia `1..d` con **la misma plantilla**: posiciones en orden creciente, para cada posición los dos signos
distintos al actual, y recursión con posición inicial `P+1`. Cada columna a distancia `k ≤ d` se visita **exactamente una vez**.

```
recorre(ini, pos, prof, nivel):            // pos 1-based; nivel = profundidad actual (el campo Profundidad/nivelProf de C#, equivalente a pasarla por parámetro)
    nivel += 1
    para P en pos ... noPartidos:           // "Partido < noPartidos + 1"   (¡ojo! ver peculiaridades de JDCdoble.Marcar y Redu1305: límites distintos)
        sig ← (ini / 3^(P−1)) mod 3
        para z en 0,1,2 con z ≠ sig:
            idx ← ini + 3^(P−1) · (z − sig)
            <acción sobre idx>
            si nivel < prof: recorre(idx, P+1, prof, nivel)     // (algunas acciones recursan ANTES de actuar: ver Calculos.Excepto)
```

Tamaño del vecindario para 14 partidos, `V(d) = Σ_{k=1..d} C(14,k)·2^k`: V1=28, V2=392, V3=3 304, V4=19 320, V5=83 384, V6=275 576, V7=714 872.
Profundidad efectiva = **`max(1, prof)`**: el primer nivel se visita siempre porque la comprobación `nivel < prof` va **después** de actuar (p. ej. con `nivel == noPartidos`, `prof = 0`, se comporta
como `prof = 1`; la UI no ofrece nivel 14, pero la API sí lo permite — **A8**).

### 1.4 Garantía matemática

Sea `M` la madre (conjunto de columnas del fichero), `L = nivel`, `d = n − L`. Una reducción `R` es **L-cubriente** si `∀ m ∈ M ∃ r ∈ R : aciertos(m, r) ≥ L`
(⇔ Hamming(m, r) ≤ d). Consecuencia: si el resultado real `w ∈ M`, entonces alguna `r ∈ R` tiene ≥ L aciertos con `w`. **No** garantiza categorías superiores ni cobertura si `w ∉ M`.
La garantía vale **solo si el algoritmo termina sin límites** (`maxCol=0`, `percent=100`, sin cancelar); con `maxCol/percent/Cancelar` la salida es una cobertura **parcial**.

---

## 2. Reductores

### 2.1 `JDC` — «menos tiempo» (`JDC.cs`)

**Propósito.** Cubrimiento voraz sobre el espacio 3^n: elige repetidamente la columna que más columnas aún vivas reduce, marca como reducidas todas las que están a distancia ≤ d y rebaja los
contadores de las influenciadas. Admite «columnas externas» (reductoras que no están en la madre).

**Tipos y datos.** `columnas: [Int32]` (índices; **orden del fichero, con duplicados**, más las externas añadidas al final), `Bits`/`BitsExternos`: bitset de `3^noPartidos`,
`flags: [Int32]` de `3^noPartidos`, `noPartidos: Int` (nº de signos de la 1.ª línea), `pot[0…16]`. Marcas en `flags`: vivo ≥ 0 (cuenta de «quién me reduce»), **`> 20 000 000` = muerto**,
`30 000 000` = reducida, `40 000 000` = reductora. Memoria n=14: `flags` 19 MB (Int32) + 2 bitsets de 0,6 MB (`JDC.cs:34-48,55-61`).

**Entradas / salidas.** `ComienzaReduccion(entrada, salida, nivel, maxCol, percent)`; `externas` en el constructor (`JDC.cs:50-53`). Salida: una línea por posición `nr` de `columnas`
(incluidas externas) con `flags[columnas[nr]] > 30 000 000`, **en orden de `columnas`** (orden de fichero, **no** de selección) y **repitiendo duplicados** (`:238-253`).

**Algoritmo.**
```
Entrada (:213-236):  noPartidos ← ObtenNumSignos(); alloc(3^noPartidos)
    por cada línea: num ← ConvColumnaANumero(línea); flags[num] ← 1; Bits[num] ← true; columnas.append(num)   // flags ← 1 (no suma) ; duplicados SÍ entran en la lista
    noColumnasIniciales ← columnas.count
Reduce (:73-138):    d ← noPartidos − nivel ; numExternas ← 0 ; si maxCol == 0: maxCol ← noColumnasIniciales
  // Fase 1 — ¿cuántas columnas me reducen? (recorre SOLO nr < noColumnasIniciales; las externas se añaden a `columnas` durante el recorrido)
  para nr en 0 ..< noColumnasIniciales:
      Calcula(columnas[nr], 1, d, +1) ; noColumnasProcesadas ← nr − noColumnasIniciales ; Pump() ; si salida: break
  noColumnasProcesadas ← 0 ; max ← −1 ; primera ← 0 ; ultima ← noColumnasIniciales + numExternas
  // Fase 2 — selección
  mientras verdadero:
      Pump() ; si salida: break
      nextcol ← −1 ; min ← −1
      para nr en primera ..< ultima:
          nv ← flags[columnas[nr]]
          si nv > 20_000_000: continuar
          si nv == max y nr < noColumnasIniciales: min ← nv ; nextcol ← nr ; romper     // atajo: 1.ª MIEMBRO con el máximo de la vuelta anterior (una externa no dispara el atajo)
          si nv > min: min ← nv ; nextcol ← nr                                             // estricto ⇒ gana la 1.ª posición con el máximo (0 es posible)
      si nextcol < 0: break
      max ← min
      mientras flags[columnas[primera]] > 20_000_000: primera++        // estrechar el intervalo (solo salta muertos; no altera el resultado)
      mientras flags[columnas[ultima−1]] > 20_000_000: ultima−−
      flags[columnas[nextcol]] ← 40_000_000 ; noColumnasFinales++
      si Bits[columnas[nextcol]]: noColumnasProcesadas++
      Marca(columnas[nextcol], 1, d)
      ncent ← noColumnasProcesadas·100 / noColumnasIniciales
      si noColumnasFinales == maxCol  o  ncent ≥ percent: break
Calcula(ini,pos,prof,delta) (:140-179)   // recorrido §1.3 con nivelProf
    acción sobre idx:  si Bits[idx]: flags[idx] += delta
                       si no y AdmiteExternas:  si !BitsExternos[idx]: numExternas++; BitsExternos[idx] ← true; columnas.append(idx)
                                                 flags[idx] += delta
    (la recursión continúa aunque idx no sea miembro)
Marca(ini,pos,prof) (:181-212)           // recorrido §1.3 con Profundidad (contador DISTINTO de nivelProf)
    acción sobre idx:  flags[idx] −= 1                                    // a TODOS los visitados, incluidos muertos/no miembros
                       si Bits[idx] y flags[idx] ≤ 20_000_000:           // aún no marcado
                            flags[idx] ← 30_000_000 ; noColumnasProcesadas++ ; Calcula(idx, 1, prof, −1)
    // recursión: si Profundidad < prof → Marca(idx, P+1, prof)  (bucle de P hasta noPartidos inclusive)
```

**Estado global.** Solo `UiPump.Pump` (`:88,99`) y, en la salida, `ConvertidorDeBases()` ⇒ `NumeroPartidos` (`:242`).

**Peculiaridades a preservar.**
1. **Salida en orden de fichero y con duplicados**: si una columna repetida es reductora se escribe tantas veces como aparece (**[réplica]** `cols_con_repetidas`, nivel 13: 10 líneas, `Finales=6`, `Procesadas=6`).
   Con la madre duplicada, `Iniciales` cuenta duplicados y `ncent` se calcula sobre ese total (`:135-136`).
2. En la fase 1 las columnas **duplicadas** suman dos veces a sus vecinas (cada entrada de la lista llama a `Calcula`), pero `flags[num]=1` no acumula duplicados (`:229`).
3. `Marca` decrementa `flags` de **todo** visitado (incl. ya marcados 30M/40M, que siguen `> 20M`; y no miembros ⇒ valores negativos en `flags` de no miembros). La condición `≤ 20 000 000` distingue «vivo» de «marcado»;
   no puede cruzarse por decrementos: cada columna recibe como mucho un decremento por reductora y otro por cada vecina recién reducida (≤ 2·3^14 ≈ 9,6 M < 10 M; razonamiento por conteo, no probado por ejecución).
4. El atajo `nv == max` solo cuenta para **miembros**; un candidato externo anterior con el mismo valor pierde frente a un miembro posterior.
5. `Cancelar` durante la fase 1 ⇒ ninguna reductora (`flags` 40M no puestos) ⇒ **no se crea el fichero de salida**. Cancelar en la fase 2 ⇒ se escribe la cobertura **parcial** (`GrabacionDeReductoras` corre siempre).
6. `salida` no se reinicia: una instancia cancelada queda cancelada (la app crea instancia nueva, `VM-Red:174`).
7. `noColumnasProcesadas` cuenta valores únicos (no duplicados) ⇒ con duplicados `Procesadas < Iniciales` aunque la cobertura sea total.
8. Si la madre está vacía: `noPartidos=0`, `indices=1`, `ncent` no se alcanza; sin líneas no se crea fichero. Con líneas de longitud > `noPartidos` el índice puede salirse del array (fuera de contrato: la UI exige 14 signos).

**Complejidad / puntos calientes.** Fase 1: `O(N·V(d))`. Fase 2: por vuelta `O(ultima−primera)` (barrido) + `O(V(d))` (Marca) + `O(V(d))` por cada columna recién reducida. N hasta 4,78 M, d hasta 13.
Es el motor por defecto de la UI ⇒ prioridad de rendimiento: `flags` como `UnsafeMutablePointer<Int32>`, bitsets de `UInt64`, recursión con profundidad como parámetro (no campo), sin `Array` con comprobación de límites en el bucle interno.

**Trampas Swift.**
- T1: `ncent = procesadas*100/iniciales` cabe en Int32 hasta 15 partidos (`14 348 907·100 = 1,43e9 < 2,147e9`); con 16 partidos C# **desborda** (`4,3e9`). La UI exige 14 signos; usar `Int` (64 bits) y rechazar >15 partidos, o `&*` sobre `Int32` si se quiere bit-exactitud fuera de contrato.
- T12: `Pump()` ⇒ sustituir por `Task.checkCancellation()` + progreso; `Procesadas` negativo en la fase 1 es solo cosmético (barra de progreso).
- `a ..< b` de Swift **aborta** si `a > b`; los bucles `Partido = pos … noPartidos` pueden tener `pos = noPartidos+1` (vacío válido en C#): usar `stride`/`while`.
- División por cero: C# lanza `DivideByZeroException` si `noColumnasIniciales == 0` y se llega a `ncent`; Swift aborta ⇒ proteger.
- `columnas` crece (externas) **durante** la recursión: el bucle de fase 1 usa el límite fijo `noColumnasIniciales` (no `columnas.count`). Cuidado con la exclusividad de `inout`: guardar el estado en una clase/struct con punteros.

**Casos golden propuestos.** `GR-01`, `GR-02`, `GR-03`, `GR-04`, `GR-14` (§6).

**Evidencia.** `JDC.cs:34-48` (campos), `:63-71`, `:73-138`, `:140-179`, `:181-212`, `:213-236`, `:238-253`; uso en `VM-Red:174-181`.

---

### 2.2 `JDCDobleContador` — «menos columnas» (`JDCdoble.cs`)

**Propósito.** Como JDC pero eligiendo, entre **todas** las columnas no reductoras (incluidas las ya reducidas y las externas), la que más columnas **«pobres»** (vivas con el valor de `flags` más bajo > 1) cubre.
Produce menos reductoras que JDC a costa de tiempo. (Optimización P-21: la cuenta de pobres se dispersa desde cada pobre en vez de recalcularse; resultado idéntico.)

**Tipos y datos.** Como JDC, más `Reducida`, `Reductora`, `BitsExternos`: bitsets de `3^n`; `poorN: [Int32]` de `3^n`; `pobresTocados: [Int32]`; `MenorDistHamming` se **escribe pero nunca se lee** (`:72,265,293`) ⇒ no portar.
`columnas` **se ordena ascendente** tras leer (`:329`; claves Int ⇒ resultado determinista, T5). Sin sentinelas de 20M/30M/40M: usa los bitsets.

**Entradas / salidas.** Igual que JDC. Salida: posiciones `nr` de `columnas` (**ordenadas por índice numérico**, luego externas en orden de descubrimiento) con `Reductora[columnas[nr]]` (`:332-347`); duplicados se repiten.

**Algoritmo.**
```
Entrada (:307-330): como JDC (flags[num] ← 1; Bits; columnas.append) ; columnas.sort() ascendente
Reduce (:94-200): d ← noPartidos − nivel ; numExternas ← 0 ; si maxCol == 0: maxCol ← noColumnasIniciales
  para nr en 0 ..< noColumnasIniciales:
      si !Reducida[columnas[nr]]: Calcula(columnas[nr],1,d,+1,false) sino Calcula(...,−1,false)      // Reducida está TODO a false aquí ⇒ la rama −1 es inalcanzable
      noColumnasProcesadas ← nr − noColumnasIniciales ; Pump ; si salida: break
  noColumnasProcesadas ← 0 ; primera ← 0 ; ultima ← noColumnasIniciales + numExternas
  mientras verdadero:
      max ← 0 ; Rmin ← 100000 ; Rmayor ← 0
      para nr en primera ..< noColumnasIniciales:                       // solo miembros
          v ← columnas[nr]
          si !Reducida[v]: max ← máx(max, flags[v])
          si !Reducida[v] y flags[v] > 1: Rmin ← mín(Rmin, flags[v])
      si max == 0: break                                                 // fin: ningún vivo con flags > 0
      para nr en primera ..< ultima:                                     // dispersar desde las POBRES
          vP ← columnas[nr]
          si Bits[vP] y !Reducida[vP] y flags[vP] > 1 y flags[vP] == Rmin: Dispersa(vP, 1, 14 − nivel)   // ← 14 literal, no noPartidos
      Pump ; si salida: break
      nextcol ← −1 ; min ← −1
      para nr en primera ..< ultima:                                     // CANDIDATAS: todas las no reductoras (reducidas y externas incluidas)
          v ← columnas[nr] ; nv ← flags[v]
          nnr ← poorN[v] + (nv == max ? 1 : 0)
          si Reductora[v]: continuar
          si nnr > min: min ← nnr ; nextcol ← nr ; Rmayor ← nv
          si nnr == min y nv > Rmayor: nextcol ← nr ; Rmayor ← nv       // desempate 1: mayor flags ; desempate 2: menor posición
      para t en pobresTocados: poorN[t] ← 0 ; pobresTocados.clear()
      si nextcol < 0: break
      mientras Reducida[columnas[primera]]: primera++                    // OJO: NO es solo optimización (ver peculiaridad 2)
      mientras Reducida[columnas[ultima−1]]: ultima−−
      c ← columnas[nextcol]
      si Bits[c] y !Reducida[c]: noColumnasProcesadas++
      Reductora[c] ← true ; Reducida[c] ← true ; noColumnasFinales++
      Marca(c, 1, d)
      ncent ← noColumnasProcesadas·100 / noColumnasIniciales
      si noColumnasFinales == maxCol  o  ncent ≥ percent: break
Dispersa(ini,pos,prof)  (:210-229)  // recorrido §1.3 con nivelProf: acción sobre idx: poorN[idx] += 1 ; pobresTocados.append(idx)   (no incrementa poorN de la propia pobre)
Calcula(ini,pos,prof,delta,ph) (:231-270)  // recorrido §1.3
    acción: si Bits[idx]: si !Reductora[idx]: flags[idx] += delta          // las reductoras quedan congeladas
            si no y AdmiteExternas: (alta de externa como en JDC) ; flags[idx] += delta    // para externas NO se mira Reductora
Marca(ini,pos,prof)  (:272-305)    // recorrido §1.3 con Profundidad, pero el bucle de P es  P = pos … noPartidos−1   (¡EXCLUYE el último partido!)
    acción sobre idx:  si Reducida[idx]: flags[idx] −= 1
                       si Bits[idx] y !Reducida[idx]:  Reducida[idx] ← true ; flags[idx] −= 2 ; noColumnasProcesadas++ ; Calcula(idx,1,prof,−1,true)
    // recursión: si Profundidad < prof → Marca(idx, P+1, prof)
```

**Estado global.** `UiPump.Pump` (`:118,155`); `NumeroPartidos` vía `ConvertidorDeBases()` en la salida (`:336`).

**Peculiaridades a preservar.**
1. **`Marca` excluye el último partido** (`for Partido … < noPartidos` en `:279`, mientras `Calcula`/`Dispersa` usan `< noPartidos + 1`, `:239,216`). Las columnas que difieren de la reductora **solo/también en el último partido** no se marcan como reducidas
   (siguen vivas y acabarán siendo reductoras): la cobertura se mantiene, el resultado es mayor. **No lo cubre ningún golden existente** (los universos de `T-JDCD` fijan los partidos 8–14 a «1»): según la réplica, con el recorrido inclusivo las 5 salidas completas de `T-JDCD` salen **idénticas**.
   Réplica: universo con triples en posiciones {1…6 y 14}, nivel 12 ⇒ **78** reductoras (inclusivo daría 69) **[réplica]** (`GR-06`).
2. **El estrechamiento `primera/ultima` NO es solo optimización**: las candidatas se escanean en `[primera, ultima)` y el estrechamiento salta prefijo/sufijo de columnas `Reducida` (incluidas reducidas **no reductoras**, que son candidatas legítimas hasta entonces).
   Se ejecuta **después** de elegir y **antes** de marcar la nueva, con el estado de la vuelta anterior: replicar el orden literal.
3. Desempate de la elección: (1) mayor `nnr = poorN + [flags==max]`, (2) mayor `flags`, (3) menor posición en `columnas` (miembros por índice numérico ascendente, luego externas por descubrimiento). `Rmayor` se pone a 0 en cada vuelta.
4. El `+1` de `nnr` se aplica a **toda** columna con `flags == max` (incluso reducidas/externas), no solo a vivas.
5. `Dispersa` usa **`14 − nivel` literal**, no `noPartidos − nivel`. Coinciden con la entrada de 14 signos que exige la UI; diferirían con otro nº (**A8**).
6. Salida ordenada por índice numérico (≠ orden de fichero de JDC) y con duplicados repetidos (**[réplica]** `cols_con_repetidas` nivel 13 y nivel 8: 10 líneas en ambos casos).
7. `flags` de los miembros vivos se mantiene ≥ 1 (cada decremento corresponde a una vecina distinta que antes había sumado 1; razonamiento por conteo, no probado por ejecución), por lo que `max == 0` ⇔ no queda ningún miembro vivo.
8. Cancelación: `salida` se comprueba tras la dispersión (`:156`), antes de reponer `poorN` (irrelevante, instancia desechable). Escritura parcial como JDC.

**Complejidad / puntos calientes.** Por vuelta: 3 barridos `O(N)` + `O(P·V(d))` (P = nº de pobres) + `O(V(d))` de `Marca` + `O(V(d))` por reducida nueva. Nº de vueltas = nº de reductoras. Hasta ~10–48× más rápido que la versión previa a P-21 (commit `174c2c8`).
Con N grande, `pobresTocados` puede llegar a millones de entradas (con repeticiones): reservar capacidad.

**Trampas Swift.** Las de JDC (T1, T12, rangos, exclusividad) y además: T5 `List<int>.Sort()` ⇒ cualquier `sort()` de `[Int32]` (enteros indistinguibles); el desempate del `if nnr == min && nv > Rmayor` depende del **orden de recorrido**,
no de ninguna ordenación ⇒ no usar `max(by:)`/`sorted` sobre candidatas.

**Casos golden propuestos.** Los 5 de `T-JDCD` (portarlos tal cual) + `GR-03`, `GR-05`, `GR-06`, `GR-07`, `GR-14`.

**Evidencia.** `JDCdoble.cs:35-61,68-82,94-200,210-229,231-270,272-305,307-330,332-347`; `T-JDCD:64-147`. Pseudocódigo validado con réplica contra esos 5 casos.

---

### 2.3 `ReductorTM` — «menos columnas 2» (`TM.cs`)

**Propósito.** Cobertura voraz sobre el **grafo de la propia madre** (no sobre 3^n): dos columnas son vecinas si Hamming ≤ `diferencia = 14 − nivel`. Elige siempre la de mayor número de vecinos vivos.
Siempre produce cobertura completa (no admite límites). Reconstruye la matriz por fichero (`O(N²)`), límite práctico N ≤ 32 768.

**Tipos y datos.** `columnasBaseDisponibles: [String]` (líneas ya `Trim+ToUpper`), `codigos: [[Int16]]` N×3, `reduceA: [[Int32]]` (lista de adyacencia **ordenada ascendente e incluyendo a la propia columna**),
`reduceCols: [Int32]` (= `reduceA[i].count` en todo momento), `reductoras: [String]` (orden de selección), `diferencias: UInt8[243][243]` (distancia de Hamming de 5 «trits»; **coincide con `Comparador.TDif5T`**, verificado 243×243), `diferencia: Int`,
`matrizOk: Bool`, `avisarSinEmparejamientos: Bool`, `archivoEntrada: String`.

**Entradas / salidas.** `Inicializa(entrada, nivel)` y `ComienzaReduccion(entrada, salida, nivel, maxCol, percent)`. **`maxCol`, `percent` y `nivelReduccion` dentro de `Reduce` se ignoran** (solo importa `diferencia`).
Salida: las `reductoras` **en orden de selección**, cada una tal como se leyó (sin reformatear ni rellenar) (`:362-370`).

**Algoritmo.**
```
Constructor (:36-68): columnas5[i] ← i en base 3, 5 dígitos con ceros a la izquierda (MSD primero) ; diferencias[i][j] ← nº de posiciones distintas (0 si i==j)
Inicializa(entrada, nivel) (:103-108): diferencia ← 14 − nivel ; si matrizOk: (limpia listas, matrizOk ← false) ; si entrada ≠ "": EntradaDeDatos(entrada)
ComienzaReduccion (:197-209): diferencia ← 14 − nivel                      // N-03: también aquí (la UI WinUI no llama a Inicializa)
    si !matrizOk: EntradaDeDatos(entrada)        // ⇒ si Inicializa ya cargó OTRO fichero, se usa ese y se ignora `entrada`
    Reduce(...) ; Grabacion(...)
EntradaDeDatos(entrada) (:120-195):
    columnasBase ← todas las líneas (Trim+ToUpper) ; N ← count ; noColumnasIniciales ← N
    para cada columna: c ← línea.replace("X","0") ;  cod[i] ← ( b3b10(c[0..<5]), b3b10(c[5..<10]), b3b10(c[10...]) )
        b3b10(s) = Σ (s[k]−'0')·3^(len−1−k)       // MSD primero; carácter fuera de '0'…'9' ⇒ FormatException
    reduceA[i] ← [] ; reduceCols[i] ← 0
    para i en 0..<N: para j en i..<N:
        dif ← 0
        si i ≠ j: dif += diferencias[cod_i0][cod_j0] ; si dif ≤ diferencia: dif += diferencias[cod_i1][cod_j1] ; si dif ≤ diferencia: dif += diferencias[cod_i2][cod_j2]
        si dif ≤ diferencia:  reduceA[i].append(j) ; reduceCols[i]++ ; si i ≠ j: reduceA[j].append(i) ; reduceCols[j]++
        ticks++ ; si ticks == 500: ticks ← 0 ; Pump ; si salida: break            // el break solo sale del bucle j
    matrizOk ← true
Reduce (:221-360):
    si !matrizOk: EntradaDeDatos(archivoEntrada)                       // (en la ruta normal ya está cargada)
    Finales ← 0 ; Procesadas ← 0 ; avisar ← false ; reductoras.clear() ; mayor ← −1
    mientras mayor ≠ 0:
        mayor ← reduceCols[0] ; positivos ← 0 ; hayCero ← false           // reduceCols[0] con N=0 ⇒ IndexOutOfRange
        para v en reduceCols: mayor ← máx ; si v>0: positivos++ ; sino si v==0: hayCero ← true
        menor ← hayCero ? positivos : −1
        si menor > 0: Procesadas ← N − menor
        si mayor == 0: break
        si mayor == 1:                                    // todas las vivas son aisladas
            aProcesar ← positivos   (= menor si hayCero) ; si menor < 0: avisar ← true
            cursor ← 0
            repetir aProcesar veces:
                mientras reduceCols[cursor] ≠ 1: cursor++
                reductoras.append(columnasBase[cursor]) ; Finales++ ; reduceA[cursor] ← [] ; reduceCols[cursor] ← 0
                ticks++ ; si ticks == 500: ticks ← 0 ; Pump ; si salida: break
        sino:
            numCol ← primer índice con reduceCols == mayor                 // Array.IndexOf: GANA EL PRIMERO
            lista ← reduceA[numCol] ; (si vacía: continuar)
            reduceA[numCol] ← [] ; reduceCols[numCol] ← 0 ; reductoras.append(columnasBase[numCol]) ; Finales++
            para numCol2 en lista (en orden):
                ticks++ ; si ticks == 500: ticks ← 0 ; Pump ; si salida: break
                si numCol2 > 32767: lanzar OverflowException
                si numCol2 ≠ numCol:
                    lista2 ← reduceA[numCol2] ; si vacía: continuar
                    reduceCols[numCol2] ← 0 ; reduceA[numCol2] ← []                       // numCol2 queda cubierta
                    para numCol3 en lista2:
                        si numCol3 > 32767: lanzar OverflowException
                        si numCol3 ≠ numCol2 y numCol3 ≠ numCol:
                            para b en [numCol, numCol2]: si reduceA[numCol3].eliminarPrimero(b): reduceCols[numCol3]−−
    si avisar: UserDialogs.ShowInfo("No hay más emparejamientos posibles con esta diferencia: las columnas restantes sólo se reducen a sí mismas, así que se han incluido todas en la reducción.")
    matrizOk ← false ; Procesadas ← N
```
`Grabacion`: una línea por reductora en orden de `reductoras`.

**Estado global.** `UiPump.Pump` (`:189,279,307`), `UserDialogs.ShowInfo` (`:354`).

**Peculiaridades a preservar.**
1. **Orden de salida = orden de selección** (a diferencia de JDC/JDCdoble/Redu1305, que escriben en orden de índice). Los golden fijan ese orden.
2. Desempate = primer índice (en el orden **del fichero**, no ordenado) con el máximo `reduceCols`. La lista de adyacencia queda ordenada ascendente por construcción (`i<j`); la eliminación por valor conserva el orden relativo.
3. Rama `mayor == 1`: añade **todas** las vivas, en orden de índice; si no había ningún cero (**primera pasada**, grafo sin aristas) activa el aviso `ShowInfo` (una vez). `T-TM:153-179` fija el caso de una columna.
4. **Límite Int16**: `numCol2 > 32767` o `numCol3 > 32767` lanzan `OverflowException("El valor era demasiado grande para un Int16.")` (`:313,325`) ⇒ con N > 32 768 puede fallar durante `Reduce`, no al leer (**A7**).
5. Una línea **vacía** o con < 10 caracteres lanza (`Substring`); caracteres no `1/X/2` (p. ej. `*`) lanzan `FormatException` en `b3b10` (`:78-79`); dígitos `3…9` no lanzan pero producen códigos > 242 ⇒ índice fuera de `diferencias` (fuera de contrato).
6. **La cancelación no detiene el algoritmo**: `salida` solo hace `break` de bucles internos (`:190,280,308`); el `while (mayor != 0)` sigue hasta cubrir todo (resultado válido pero posiblemente distinto). Para Swift: cancelar = abortar sin escribir, o ignorar (**A7/A9**).
7. `Inicializa(otraEntrada)` + `ComienzaReduccion(entrada)` ⇒ se usa la matriz de `Inicializa` (`:206`). Tras `Reduce`, `matrizOk=false` (`:358`): la siguiente ejecución relee.
8. El grafo incluye a cada columna como vecina de sí misma (`reduceA[i]` contiene `i`); duplicados en la madre son vecinos entre sí (`dif = 0`).

**Complejidad / puntos calientes.** Construcción: `N²/2` pares × hasta 3 consultas; memoria `Σ grado` (hasta ~N² enteros ⇒ GB si N≈32 k denso). Selección: `O(N)` por reductora (`mayor`, `IndexOf`) + eliminación `O(grado)` con `List.Remove` lineal.
Optimizaciones libres (resultado idéntico): matriz de adyacencia en bitsets o CSR, `firstIndex` incremental; mantener el **orden** de `reduceA` solo importa para la cancelación.

**Trampas Swift.** T1: `Convert.ToInt16` ⇒ comprobar rango 0…32 767 explícitamente; `UInt8` para `diferencias`. T5/T6: ninguna (listas, sin hash). T12: `ShowInfo` ⇒ evento/valor de retorno (`avisoSinEmparejamientos: Bool`) en lugar de diálogo.
La tabla `diferencias` y `Comparador.TDif5T` son **la misma** (distancia de Hamming de 5 dígitos ternarios; invariante a la asignación símbolo→dígito siempre que ambas columnas usen la misma): una sola tabla de 243×243 `UInt8` sirve a los dos.

**Casos golden propuestos.** Los 6 de `T-TM` + `GR-08`, `GR-09`, `GR-10`.

**Evidencia.** `TM.cs:15,36-68,70-101,103-118,120-195,197-209,221-360,362-370`; `T-TM:59-235`. Pseudocódigo validado con réplica (4 casos exactos).

---

### 2.4 `Redu1305Xfsf` — «grandes archivos» (`R1305.cs`)

**Propósito.** Cobertura **sin puntuación**: recorre los índices en orden numérico ascendente, toma el primer vivo como reductora, marca como reducidas las columnas a distancia ≤ d y «degrada» (marca 2) las de distancia `d+1…2d`;
cuando se agotan los vivos «1», toma las degradadas «2» en orden. Pensado para ficheros enormes (sin fase de conteo).

**Tipos y datos.** `validas`: bitset 3^n (reductoras); `flags: [Int32]` 3^n con valores `0` (no miembro), `1` (vivo), `2` (influenciada), `−1` (reducida/reductora); `Profundidad: Int16` **compartido** por las tres funciones recursivas;
`pot`; `AdmiteExternas` y `numExternas` (**nunca se incrementa; siempre 0**).

**Entradas / salidas.** Salida: bit `validas[nr]` activo, `nr` ascendente (**orden numérico, sin duplicados**), `ConvNumAColumna(nr)` (`:238-248`). `AdmiteExternas` **no cambia nada**: `ReduceConExternas` y `Reduce` tienen cuerpos idénticos (`:67-110` ≡ `:111-154`).

**Algoritmo.**
```
Entrada (:222-236): noPartidos ← ObtenNumSignos(); alloc ; por cada línea: flags[ConvColumnaANumero(línea)] ← 1 ; noColumnasIniciales++      // cuenta duplicados
Reduce (:111-154):
    salida ← false                                   // ¡reinicia la cancelación!
    Finales ← Procesadas ← 0 ; si maxCol == 0: maxCol ← noColumnasIniciales ; primera ← 0
    nsel ← primer índice ≥ 0 con flags == 1  (o `indices` si no hay)
    // Fase 1
    mientras verdadero:
        Pump ; si salida: break
        validas[nsel] ← true                          // nsel == indices (madre vacía) ⇒ IndexOutOfRange
        Finales++ ; flags[nsel] ← −1 ; Procesadas++
        MarcaR(nsel, 0, NumeroPartidos − nivel)       // ← usa la GLOBAL NumeroPartidos para la profundidad
        nsel ← primer índice ≥ primera con flags == 1 (y entonces primera ← nsel+1) ; si no hay: nsel ← indices
        si nsel == indices: break
        ncent ← Procesadas·100 / noColumnasIniciales
        si Finales ≥ maxCol  o  ncent ≥ percent: break
    // Fase 2 (SIEMPRE se ejecuta, también tras un corte por límite)
    para nr en 0 ..< indices:
        si flags[nr] == 2:
            Pump ; si salida: break
            validas[nr] ← true ; Finales++ ; flags[nr] ← −1 ; Procesadas++
            Marca1(nr, 0, NumeroPartidos − nivel)
            ncent ← …; si Finales ≥ maxCol  o  ncent ≥ percent: break            // se comprueba DESPUÉS de añadir ⇒ añade 1 reductora más aunque ya se hubiera alcanzado el límite
MarcaR(ini,pos,prof)  (:156-179)    // posiciones 0-based: P = pos … noPartidos−1 ; pot[P] ; Profundidad++ al entrar
    acción sobre idx:  si flags[idx] > 0: flags[idx] ← −1 ; Procesadas++
                       si Profundidad < prof: MarcaR(idx, P+1, prof)
                       sino:                  Influ(idx, P+1, 2·prof)           // hoja del nivel d: degrada hasta 2d
Marca1(ini,pos,prof)  (:180-201)    // igual pero sin Influ: solo marca (flags>0 → −1) hasta profundidad prof
Influ(ini,pos,prof)   (:202-220)    // acción: si flags[idx] > 0: flags[idx] ← 2 ; recursión si Profundidad < prof   (Profundidad CONTINÚA desde d: el marco entra en d+1)
```
Equivalencia de conjuntos (permite implementación no recursiva si se verifica con golden): tras elegir `r`, toda columna con `dist(r,x) ≤ d` y `flags>0` pasa a `−1` (cuenta en `Procesadas`), y toda con `d < dist ≤ 2d` y `flags==1` pasa a `2`;
el resultado final no depende del orden (cada columna se alcanza una sola vez por la regla de posiciones crecientes).

**Estado global.** **`VariablesGlobales.NumeroPartidos`** (`:84,104,128,148`) para la profundidad frente a `noPartidos` (del fichero) para los bucles ⇒ coinciden solo si ambos valen lo mismo; `UiPump.Pump`; `ConvertidorDeBases()` en salida.

**Peculiaridades a preservar.**
1. **`maxCol` se supera en 1** si quedan «2» tras la fase 1: la fase 2 añade siempre una reductora antes de comprobar el límite. **[réplica]** universo de 5 triples, nivel 12: `maxCol=1 → 2 líneas`, `maxCol=2 → 3` (`GR-12`).
2. Cancelar **antes** de `ComienzaReduccion` no sirve: `Reduce` hace `salida=false` (`:115`).
3. Profundidad con `d ≤ 1` ⇒ efectivamente 1 (§1.3); con `d=1` la «influenciada» llega solo a distancia 2.
4. `AdmiteExternas` ignorado; `numExternas` siempre 0 (la UI oculta el control para este método, `VM-Red:59-62`).
5. Salida ordenada por índice numérico y **sin duplicados** (≠ JDC).
6. Madre vacía ⇒ excepción (`validas[indices]`).

**Complejidad / puntos calientes.** Cada reductora: `O(V(d) + V(2d))` (con 14 partidos, V(10) = 3 533 688 de 4 782 968 columnas posibles, así que para d ≥ 5 la zona de influenciadas es ya la mayor parte de 3^n); barrido ascendente amortizado por `primera`; fase 2 barre 3^n. Cuello: `MarcaR` sobre vecindarios grandes. Bitsets/`Int8` para `flags`.

**Trampas Swift.** T1 (`ncent`), rangos `a ..< b`, `Int16` de `Profundidad` (irrelevante: usar `Int`), recursión con profundidad como parámetro.

**Casos golden propuestos.** `GR-11`, `GR-12`, `GR-14`.

**Evidencia.** `R1305.cs:32-65,67-154,156-220,222-248`; `VM-Red:176`.

---

### 2.5 `XFSF` (huérfano) (`XFSF.cs`)

**Propósito.** Heurística de «punto más lejano»: parte de la columna `N·4/10`, marca reducidas las que tienen ≥ `nivel` aciertos con la reductora actual y pasa a la viva con **menor suma acumulada de aciertos**.

**Tipos y datos.** `columnas: byte[3^14][3]` (cada columna = 3 bytes con trozos de 5 posiciones: `'1'→0`, `'2'→2·3^i`, otro→`3^i`, `i` 0-based dentro del trozo, `Comp.cs:288-322`), `flags: Int32[3^14]`. Requiere **≥ 14 caracteres** por línea (si no, `IndexOutOfRangeException`, `Comp.cs:296`).
`CalculaCoincidencias = 14 − Σ TDif5T` con cast a `byte` (envuelve si > 14 diferencias, p. ej. líneas de 15 signos). Salida: siempre **14** caracteres (`Conv3BytesAColumna` recorta el 15.º, `Comp.cs:376-385`).

**Algoritmo** (`XFSF.cs:70-107`; salida `:109-122` en orden de fichero, `flags == 999 999 999`).
```
Entrada: por línea: columnas[N] ← 3 bytes ; flags[N] ← 0 ; N++
Reduce: nextcol ← N·4/10 ; nfin ← maxCol == 0 ? N : maxCol
  mientras verdadero:
      Pump ; si salida: break
      flags[nextcol] ← 999_999_999 ; ref ← columnas[nextcol] ; nfin−− ; Procesadas++ ; Finales++
      min ← 999_999_999 ; ind ← 0
      para nr en 0..<N: si flags[nr] > 999_999_997: continuar
          na ← coincidencias(columnas[nr], ref)
          si na ≥ nivel: flags[nr] ← 999_999_998 ; Procesadas++
          sino: flags[nr] += na ; si flags[nr] < min: ind ← nr ; min ← flags[nr]            // estricto ⇒ 1.ª mínima
      si min > 999_999_997  o  nfin == 0  o  Procesadas·100/N ≥ percent: break
      nextcol ← ind
```
**Peculiaridades.** Los buffers de 3^14 se crean una vez y **no se reasignan** entre ejecuciones (`:31-43`); no afecta (solo se lee `[0,N)`). N=0 ⇒ `flags[0]` se toca y se sale sin salida. Sin llamadores. **Golden**: `GR-13`.
**Evidencia.** `XFSF.cs:36-43,45-122`; `Comp.cs:288-343,376-385`.

### 2.6 `XfsfV3` (huérfano) (`V3.cs`)

**Propósito.** Equivalente a JDC pero sobre **pares de la propia madre** (`O(N²)`): `flags[nr] = 1 + nº de columnas con ≥ nivel aciertos`; elige la de mayor valor, marca como reducidas las coincidentes y rebaja a sus vecinas.
**Tipos.** Igual que XFSF. Sentinelas 20M/30M/40M como JDC.

```
Fase 1: para nr en 0..<N: flags[nr]++ ; para nr3 en nr+1..<N: si coincidencias(nr,nr3) ≥ nivel: flags[nr]++ ; flags[nr3]++
        Procesadas ← nr − N ; Pump ; si salida: break           (:58-80)
        Procesadas ← 0
Fase 2: mientras verdadero:  (Pump; salida)
    nextcol ← 1.ª posición con mayor flags entre las de flags ≤ 20_000_000 (estricto >, empieza en min=−1)   (:88-94)
    si nextcol < 0: break
    flags[nextcol] ← 40_000_000 ; Finales++ ; Procesadas++
    para nr en 0..<N con flags[nr] ≤ 20_000_000 y coincidencias(nextcol,nr) ≥ nivel:
        flags[nr] ← 30_000_000 ; Procesadas++
        para nr3 en 0..<N con flags[nr3] ≤ 20_000_000:  si coincidencias(nr,nr3) ≥ nivel y flags[nr3] > 0: flags[nr3]−−
    si Finales == maxCol  o  Procesadas·100/N ≥ percent: break             (:119-120)
Salida (:139-152): orden de fichero, flags > 30_000_000
```
**Peculiaridades.** Salida con **14** caracteres; duplicados se escriben repetidos; `O(N²)` por reducida (`N³` en el peor caso). Sin llamadores. **Golden**: `GR-13`.
**Evidencia.** `V3.cs:51-152`.

### 2.7 `JLPM` (huérfano, **puede no terminar**) (`JLPM.cs`)

**Propósito.** Port de un algoritmo antiguo de JLPM sobre strings; ignora `maxCol` y `percent` (`:41`).

```
ty ← líneas (Trim+ToUpper) ; ebet ← N ; ret ← QC(ty, ty[0], nivel)        // nº de j con aciertos(ty[0],ty[j]) ≥ nivel  (N=0 ⇒ ty[0] lanza)
g0[i] ← ret  y  g1[i] ← true  para TODO i                                   // ¡el MISMO valor para todas! (bug aparente)
acu ← 0
bucle: Pump
    res ← 0 ; para i: si g0[i] ≥ 0 y g0[i] > res: res ← g0[i] ; ret ← i     // 1.º máximo
    si res == 0: break
    para i en 0..<N: si g1[i] y aciertos(ty[i],ty[ret]) ≥ nivel:
         para j: si g0[j] > 0 y aciertos(ty[i],ty[j]) ≥ nivel: g0[j]−−
         g1[i] ← false
    reductoras.append(ty[ret]) ; Finales++ ; acu += res
    si acu == ebet: break                                                   // IGUALDAD: puede saltarse
```
**Peculiaridades.** `g0` no es un recuento real (parte del valor de la columna 0) ⇒ una columna con vecindario ya cubierto puede conservar `g0>0`, volver a elegirse indefinidamente (se añade repetida) y el bucle solo se detiene por `acu == ebet` exacto o por `res == 0`; si en una vuelta no queda ninguna `g1[i]` viva cerca de `ret`, el estado no cambia y la vuelta se repite eternamente mientras `acu` crece sin igualar `ebet`:
la réplica **no termina** en `cols_a` con nivel 6 y en `cols_con_repetidas` con niveles 13, 10 y 6 **[réplica; sin test C#]**. `Procesadas` queda en 0. Complejidad `O(R·N²)`. **No portar** salvo orden expresa (**A5**).
**Evidencia.** `JLPM.cs:35-45,47-129,131-144,146-176`.

---

## 3. `SubirCategoria.Calculos` (`Calculos.cs`)

**Propósito.** «Subir categoría»: a partir de una madre genera **todas las columnas obtenibles cambiando signos** en los partidos «involucrados», con el nº de cambios en los niveles elegidos y cierta racha de cambios «seguidos»; opcionalmente restringidas a un fichero de «externas».

**Tipos y datos.** `columnas: [Int32]` (índices base-3 de la madre, **con duplicados**, leídos con `ConvColumnaANumero(línea, noSignos)` → solo los `noSignos` primeros caracteres, `:53-71`),
`Bits`, `ColumnasExternas`: bitsets de **3^15 = 14 348 907** (`:39-40`), `PartidoInvariante: [Bool]` (`noPartidos`), `ListaInvolucrados: [Int]` **tamaño fijo 29** (`:44`), `_nivel: [Bool]` (índice = nº de cambios, 0…16),
`_involucrados: [Bool]` (índice = partido 0-based), `ProfMaxima`, `Profundidad`, `noColumnas: Int`, `pot[0…15]`.

**Entradas / salidas.** `init(archivo)` ⇒ `NoColumnas = nº de líneas` («I=» en la UI). `Calcular(involucrados, niveles, seguidos, ficheroExternas, noPartidos)` ⇒ rellena `Bits`; `NoColumnas` pasa a ser el nº de columnas **distintas** generadas («F=»).
`Grabar(archivo)` ⇒ escribe `Bits` en **orden de índice ascendente** con anchura `noPartidos` (`:73-79`, `ACT.cs:156-181`); no crea fichero si no hay ninguna; `NoColumnas` no cambia. La UI: `noSignos = ObtenNumSignos()` del fichero base, `involucrados` = casillas de partido (16), `niveles` = casillas 0…16, `seguidos` ≤ nº de partidos (`VM-Sub:239-260`).

**Algoritmo.**
```
LeerExternas(f) (:165-182): ColumnasExternas ← todo false ; si f == "": todo true ; sino: para cada línea: ColumnasExternas[ConvColumnaANumero(línea)] ← true   // versión sin numPartidos: recorre TODA la línea
Calcular (:116-164):
    PartidoInvariante ← [false]·noPartidos ; numPartidos ← noPartidos ; LeerExternas
    _nivel ← niveles ; _involucrados ← involucrados ; numInvolucrados ← 0
    para i en 0..<involucrados.count: si involucrados[i]: ListaInvolucrados[numInvolucrados++] ← i
    para i en 0 ... seguidos: ListaInvolucrados[numInvolucrados + i] ← ListaInvolucrados[i]            // circular (copia secuencial)
    ProfMaxima ← noPartidos + 1 ; repetir  ProfMaxima−−  hasta _nivel[ProfMaxima]            // = mayor nivel marcado ≤ noPartidos ; ninguno ⇒ sale de rango y lanza
    noColumnas ← 0 ; Bits ← todo false
    para cada c en columnas (en orden, duplicados incluidos):
        si _nivel[0] y !Bits[c] y ColumnasExternas[c]: Bits[c] ← true ; noColumnas++
        para n en 0 ..< numInvolucrados:
            PartidoInvariante ← todo false ; PartidoInvariante[ListaInvolucrados[n]] ← true
            Seguidos(c, n, 0, seguidos)
Seguidos(ini, i, _, pmax)  (:184-220):              // Profundidad es un contador de campo, 0 al empezar
    Profundidad++ ; P ← ListaInvolucrados[i] ; PartidoInvariante[P] ← true ; sig ← (ini / 3^P) mod 3
    para z ≠ sig:  idx ← ini + 3^P·(z − sig)
        si Profundidad < pmax: Seguidos(idx, i+1, _, pmax)
        sino:  Excepto(idx, 0, _)                                  // 1.º las derivadas
               si _nivel[Profundidad] y !Bits[idx] y ColumnasExternas[idx]: Bits[idx] ← true ; noColumnas++
    Profundidad−−
Excepto(ini, pos, _)  (:221-254):
    Profundidad++
    para P en pos ..< numPartidos:  si !_involucrados[P] o PartidoInvariante[P]: continuar
        sig ← (ini / 3^P) mod 3
        para z ≠ sig:  idx ← ini + 3^P·(z − sig)
            si Profundidad < ProfMaxima: Excepto(idx, P+1, _)                  // recursión ANTES de marcar
            si _nivel[Profundidad] y !Bits[idx] y ColumnasExternas[idx]: Bits[idx] ← true ; noColumnas++
    Profundidad−−
```

**Semántica de conjuntos** (verificada por fuerza bruta contra la recursión en 40 configuraciones aleatorias, con `s ≤ nº de involucrados`): sea `s = max(1, seguidos)`. La salida es la unión, sobre cada columna `c` de la madre, de:
(a) `c` si `niveles[0]`; (b) toda columna obtenida cambiando el signo de un conjunto `S ⊆ involucrados` (cada partido de `S` a uno de los 2 signos distintos) con `|S| = k`, `niveles[k]`, y tal que `S` **contiene `s` partidos consecutivos de la lista de involucrados, en circular**;
(c) filtrada por `ColumnasExternas`. El resultado no depende del orden de visita.

**Estado global.** Ninguno (usa `ConvertidorDeBases(noPartidos)` explícito, `:120`; `Grabar` pasa `numPartidos` al fichero, `:75`).

**Peculiaridades a preservar.**
1. **`seguidos = 0` y `seguidos = 1` son equivalentes** (`Profundidad(1) < pmax` es falso en ambos) ⇒ `s = max(1, seguidos)`. Con `seguidos ≥ 2` **nunca** se generan columnas de nivel < `s` (salvo el nivel 0 de la madre).
2. `ListaInvolucrados` tiene **29** posiciones: `numInvolucrados + seguidos + 1 > 29` lanza. Es **campo de instancia**: una 2.ª llamada a `Calcular` con menos involucrados deja valores residuales (la extensión circular copia posiciones antiguas si `numInvolucrados < seguidos+1`). `Profundidad` también es campo: una excepción a medias lo deja desbalanceado.
3. Con `seguidos > nº de involucrados` la cadena **repite partido** (p. ej. A,B,A): el «nivel» pasa a ser contador de pasos, no distancia de Hamming; la equivalencia de conjuntos deja de valer (la recursión es la definición).
4. `ProfMaxima` solo limita la recursión de `Excepto`; `Seguidos` no la consulta (puede marcar `_nivel[s]` con `s > ProfMaxima`; `_nivel` necesita índice válido).
5. Duplicados en la madre repiten trabajo pero no la salida (`Bits` es un conjunto).
6. `SubirUnaCategoria` (`:80-114`) es **código muerto** (no lo llama `Calcular`) y tiene `Partido < 14` literal: no portar.
7. `Bits` mide 3^15 aunque `noPartidos=14`: `Grabar` recorre 14,3 M posiciones (anchura `noPartidos`).

**Complejidad / puntos calientes.** Por columna de la madre: `M` arranques × `2^s` cadenas × `Σ_{k} C(M−s,k)·2^k` derivadas hasta `ProfMaxima` ⇒ explosivo con muchos involucrados/niveles altos; el cuello son los accesos a `Bits`/`ColumnasExternas` (2 bitsets de 1,8 MB).
Una implementación por **conjuntos** (generar el conjunto de cambios y marcar) es admisible si el golden coincide.

**Trampas Swift.** `a ..< b` con `b < a` aborta (`Partido = pos…`); campos de profundidad ⇒ parámetros; `ListaInvolucrados` como `[Int](repeating:0,count:29)` con comprobación de límites explícita para lanzar el mismo error; `BitArray(14348907)` ⇒ `[UInt64]` de 224 k palabras; T1: sin riesgo (índices < 14,3 M).

**Casos golden propuestos.** `GS-01`, `GS-02`, `GS-03`.

**Evidencia.** `Calculos.cs:32-47,53-79,80-114,116-164,165-182,184-220,221-254,255-259`; `ACT.cs:156-181`; `VM-Sub:239-310`; `Free1X2/UI/SubirCategoriaFrm.cs:1074,1112,1144`.

---

## 4. Escrutinio

### 4.1 `Escrutador` (`Escrutador.cs`)

**Propósito.** Cuenta, para una columna ganadora (texto, admite `*` y minúsculas), el nº de aciertos de **cada línea** de un fichero de columnas; acumula el histograma de aciertos y, opcionalmente, la lista de columnas premiadas.
Variante «temporada»: repite el proceso para cada ganadora de un fichero de jornadas.

**Tipos y datos.** `premiosAceptados: [Int]` (categorías que cuentan como premio, p. ej. `[14,13,12,11,10]`, **la UI pasa un `int[]`**, `VM-Esc:584`), `premiosTotales: [Int]` tamaño `NumeroPartidos+1` (`:51,57`), `listaPremiadas: [ColumnaPremiada]` (insertion order, `ArrayList`),
`archivoCols: String`, `numJornada`, `añadirAGanadoras`, `pararEscrutinio`, `columnas: [String]` (ganadoras de la temporada), `limiteCol`. Constructora sin argumentos (`:60-62`) deja `premiosTotales=null`.

**Entradas / salidas.** `EscrutaCombConColumna(ganadora, ds[, archivo])`, `EscrutaCombConTemporada(archivoRef, ds, archivo)`, `EscrutaApuestaMultiple(apuesta: Int64, ganadora: Int64) -> Int`, `PonerPremios`, `AñadirPremiosGlobales`, `PararEscrutinio`.
Efectos: acumula `premiosTotales` (**nunca se reinicia**), `listaPremiadas` (nunca se vacía), y añade filas a la tabla `Resultados` del `DataSet` (esquema según `T-FIX:17-29`: `Seleccionado:Bool, LineaID:Int, Columna:String, Archivo:String, P0…P14:Int, "Ac. Totales":String`; el real lo crea la UI).

**Algoritmo.**
```
EscrutaColumna(gan) (:159-203):
    nc ← [0]·(NumeroPartidos+1) ; numCol ← 0 ; gan ← gan.reemplazar('x','X')          // solo 'x'→'X'; NO se recorta ni se pasa a mayúsculas el resto
    para cada línea del fichero archivoCols:                                              // semántica StreamReader (§1.2): líneas en blanco cuentan
        numCol++ ; col ← línea.Trim().ToUpper() ; m ← mín(col.count, gan.count)
        na ← nº de i en 0..<m con gan[i] == '*'  o  gan[i] == col[i]                      // comparación de unidades UTF-16
        si añadirAGanadoras: AñadeAGanadoras(col, numCol, na)
        nc[na]++                                                                          // na > NumeroPartidos ⇒ IndexOutOfRange
    devolver nc
AñadeAGanadoras (:205-219): si na ∈ premiosAceptados: listaPremiadas.append( {Columna: col, Fichero: archivoCols (ruta tal como se asignó), Jornada: numJornada, NoColumna: numCol, Premio: na, NoBoleto: ObtenBoleto(numCol)} )
ObtenBoleto(n) (:221-226): n/8 + (n mod 8 > 0 ? 1 : 0)                                    // ceil(n/8): 8 apuestas por boleto
EscrutaCombConColumna(gan, ds, archivo) (:87-102): numJornada ← 1 ; premios ← EscrutaColumna(gan) ; PonerPremios(premios, 1, gan, archivo)
EscrutaCombConTemporada(ref, ds, archivo) (:117-137):
    pararEscrutinio ← false ; LeeArchivoTemporada(ref)       // lee ganadoras: Trim+ToUpper + 'x'→'X' ; reinicia la lista
    para i en 0..<limiteCol: numJornada ← i+1 ; si pararEscrutinio: break ; Pump ; premios ← EscrutaColumna(columnas[i]) ; PonerPremios(premios, i+1, columnas[i], archivo)
PonerPremios(prems, n, col)            (:228-241): fila {LineaID: n, Columna: col, Seleccionado: false} ; para i en 1..<prems.count: P{i} ← prems[i] ; premiosTotales[i] += prems[i]     // ¡empieza en 1: P0 no se rellena ni se acumula!
PonerPremios(prems, n, col, archivo)   (:243-260): fila {LineaID: n, Columna: col, Archivo: fileName(archivo), Seleccionado: false} ; para i en 0..<prems.count: P{i} ← prems[i] ; premiosTotales[i] += prems[i] ; tot += prems[i]·i ; "Ac. Totales" ← tot
AñadirPremiosGlobales(g) (:262-276): fila {LineaID: 0, Archivo: "-------------", Columna: "----- TOTALES:", Seleccionado: false} ; P{i} ← g[i] para i en 0 ... NumeroPartidos ; "Ac. Totales" ← " " (un espacio)
EscrutaApuestaMultiple(apuesta, ganadora) (:104-108): popcount(UInt64(ganadora & apuesta))      // 3 bits por partido (2=001, X=010, 1=100): cuenta los partidos de la ganadora cubiertos por el pronóstico
```
`LeerPremios(csv)` (`:76-85`): `Split(',')` + `Convert.ToInt16` por elemento (cultura actual; admite espacios y signo; lanza `FormatException` si vacío/inválido). La UI no lo usa (pasa `int[]`).

**Estado global.** `VariablesGlobales.NumeroPartidos` en `:51,57,161,270`; `UiPump.Pump` (`:133`).

**Peculiaridades a preservar.**
1. **La sobrecarga de 3 argumentos de `PonerPremios` no rellena ni acumula `P0`** y no escribe `Archivo` ni `Ac. Totales`; la de 4 sí (`:228-260`). Las rutas reales (`VM-Esc`, `T-FIX:42`, `T-FLU:87`) usan la de 4.
2. `premiosTotales` y `listaPremiadas` **acumulan entre llamadas** de la misma instancia; la UI crea una instancia por fichero (`VM-Esc:584`) y suma sus `PremiosTotales` en `globales` (`VM-Esc:604-606`).
3. **Las líneas en blanco cuentan**: una línea vacía final extra da `nc[0]++` y aumenta `numCol` (afecta a `NoColumna` y `NoBoleto`).
4. La ganadora más corta que la columna ⇒ solo se comparan los primeros `m` signos; los demás no suman aciertos (**[réplica]** ganadora `2112` ⇒ histograma `{0:1,1:2,2:6,4:1}`).
5. `*` solo es comodín en la **ganadora** (`ch == '*'`), no en la columna.
6. En «temporada» el fichero de columnas **se relee completo por cada jornada** (`O(J·N)` E/S): un port puede cargarlo una vez si el resultado es idéntico (no cambia nada observable salvo que el fichero cambie durante la ejecución).
7. `Fichero` de `ColumnaPremiada` es la ruta completa asignada a `ArchivoColumnas`; `Archivo` de la fila es solo el nombre (`Path.GetFileName`).
8. `Ac. Totales` = `Σ prems[i]·i` (total de aciertos de la jornada) como texto.
9. `PararEscrutinio()` solo se consulta en el bucle de temporada, **no** dentro de `EscrutaColumna`.

**Complejidad.** `O(N·L)` por ganadora (L = 14). Dominado por E/S y por el parseo de líneas. Paralelizable por jornada (resultados independientes; las filas deben insertarse en orden de jornada).

**Trampas Swift.** T8 (líneas `\r\n/\n/\r`, BOM, `Trim`/`ToUpper` .NET ≠ Foundation en caracteres no ASCII; operar con `[UInt8]`/UTF-16 y no con `Character` para no alterar longitudes), T7 (`Convert.ToInt16` con cultura; `String(Int)` ASCII), T10 (`NumeroPartidos` ⇒ parámetro de sesión, **fijado al construir** como en C#), T9/T1 (`na > NumeroPartidos` ⇒ error explícito en vez de trap).

**Casos golden propuestos.** `GE-01`…`GE-06`.

**Evidencia.** `Escrutador.cs:31-62,76-85,87-137,139-157,159-203,205-226,228-276`; `T-FIX:31-58`; `T-FLU:76-97`; `VM-Esc:584-606` (tipos 1 y 2), `VM-Esc:732-747` (tipo 3).

---

### 4.2 `EscrutadorComb` (`EscComb.cs`) — **con defectos probables, ver A1–A4**

**Propósito.** Dada una columna ganadora y una lista de categorías (p. ej. 14,13,12,11,10), calcula el **conjunto de columnas que obtendrían alguna de esas categorías** (vecinos de la ganadora hasta cierta distancia, etiquetados con su nº de aciertos),
y cuenta/lista cuáles de ellas **cumple una combinación** (`.comb`: pronóstico base + condiciones).

**Tipos y datos.** `aciertos: Hashtable` (claves **mezcla de `Int64` y `Int32` boxeados**, valores `int` o `byte` boxeados), `premiosAceptados: [Int]` (se ordena in situ, ver abajo), `premiosTotales`/`premiosCombinacion`: `[Int]` de `premiosAceptados.count`,
`listaEscrutadasConPremio: ArrayList` (**null por defecto**: la UI debe asignarla si `añadirAGanadoras`, `VM-EscC:437`), `escrutinioDS` (**nunca asignado ⇒ siempre null**), `pot[0…13]` (14 potencias: solo hasta 14 partidos).

**Entradas / salidas.** `new EscrutadorComb(int[] categorias)` (el mismo array se pasa luego a `ObtenerPosiblesPremios`), `ArchivoColumnas = ruta .comb`, `AñadirAGanadoras`, `ObtenerPosiblesPremios(colGanadora, categorias)`, `EscrutarCombinacion(jornada)`.
Salida: `PremiosTotales[orden]++` donde `orden` = posición de la categoría en `Premios` **tras la ordenación descendente** (no el nº de aciertos, `VM-EscC:414-417`), y `ListaEscrutadasConPremio` de `ColumnasPremiadasComb{Columna, Fichero, Jornada, Premio}`.

**Algoritmo.**
```
ObtenerPosiblesPremios(colGan, cats) (:232-247):
    aciertos ← vacío ; Sort(cats) ; Reverse(cats)                          // EN SITIO: modifica también `Premios` si es el mismo array
    prof ← cats[0] (byte) ; col ← ConvColumnaANumero(colGan) (Int32 → Int64) ; c0 ← ConvertidorDeBases()            // anchura = NumeroPartidos
    si cats[0] == NumeroPartidos: aciertos[col (Int64)] ← (byte)cats[0] ; prof−−
    Vecinos(col, 0, ultimo = (byte)cats[última], premioActual = prof)
Vecinos(ini, pos, ultimo, premioActual) (:209-231):                          // recorrido §1.3 con P 0-based ∈ pos ..< NumeroPartidos
    para P: sig ← (ini / 3^P) mod 3 ; para z ≠ sig:  idx ← (Int32)(ini + 3^P·(z − sig))
        si aciertos.contiene(idx (Int32)): si aciertos[idx] < premioActual: aciertos[idx] ← premioActual
        sino: aciertos.añadir(idx, premioActual)
        si premioActual > ultimo: Vecinos(idx, P+1, ultimo, premioActual − 1)
EscrutarCombinacion(jornada) (:100-134):
    analizador ← Analizador() ; archComb ← abrir(archivoCols) ; cargar grupos en analizador ; pronosticos ← archComb.LeePronosticos() con comas eliminadas
    claves ← aciertos.Keys.CopyTo(long[])                                     // ORDEN = recorrido de buckets del Hashtable (de la última a la primera)
    para cada clave k en ese orden:
        si AnalizadorColumnaCombinacion.AnalizaColumna(k, analizador, pronosticos):          // k es un ÍNDICE BASE-3, no un long de 3 bits (ver A1)
            premio ← (int) aciertos[k] ; orden ← índice de premio en premiosAceptados
            si orden ≥ 0: premiosTotales[orden]++
            si añadirAGanadoras: listaEscrutadasConPremio.añadir( {Columna: k, Fichero: archivoCols, Jornada: jornada, Premio: premio} )
```
`AnalizadorColumnaCombinacion.AnalizaColumna(long, Analizador, pronósticos)` (`Free1X2.Domain/Analisis/AnalizadorColumnaCombinacion.cs:38-110`, especificado en ESPEC_01/06): rechaza si la columna no está contenida en el pronóstico base, si falla algún filtro activo / tolerancia de grupo, el control de grupos/conjuntos o el If-Then.

**Semántica pretendida** (lo que el diseño busca; lo que el C# hace hoy es **A1**): el conjunto de candidatas = ganadora (si `cats[0]==NumeroPartidos`, etiqueta `cats[0]`) + vecinos a distancia `k=1…` con etiqueta `prof − (k−1)`, hasta etiqueta `cats[última]`.
Con `cats = [14,13,12,11,10]` y 14 partidos: etiquetas `14:1`, `13:28`, `12:364`, `11:2 912`, `10:16 016` columnas (19 321 en total).

**Estado global.** `NumeroPartidos` (`:183,212,241`); `UiPump` no se usa; lee un fichero `.comb` por **cada** llamada (por jornada en el modo temporada ⇒ E/S y parseo repetidos).

**Peculiaridades a preservar (las que la lectura del código fija) y problemas abiertos.**
1. **Las etiquetas dependen de `cats[0]`, no de la distancia real**: el vecino a distancia 1 recibe `prof = cats[0]` (o `NumeroPartidos−1` si `cats[0]==NumeroPartidos`). Solo coinciden con el nº real de aciertos si `cats[0] ∈ {N, N−1}`; con `[12,11,10]` el vecino a distancia 1 se etiqueta 12 (real: 13).
2. Siempre se añaden los vecinos a distancia 1 aunque `cats[0]` solo pida 14 (la recursión es lo que se corta: `premioActual > ultimo`). Categorías intermedias ausentes (p. ej. `[14,13,10]`) producen etiquetas 12 y 11 que **no** cuentan en `PremiosTotales` (`orden = −1`) pero **sí** se listan si `añadirAGanadoras`.
3. `Sort`+`Reverse` modifican el array del llamador (que es el mismo que `Premios` ⇒ cambia el significado de los índices de `PremiosTotales`).
4. `PonerPremios`/`AñadirPremiosGlobales` lanzan `NullReferenceException` siempre (`escrutinioDS` nunca se asigna): el legado llamaba a `AñadirPremiosGlobales` (`Free1X2/UI/EscrutarCombinacionesFrm.cs:349`); la UI WinUI lo omite (`VM-EscC:403-408`).
5. Clave de la ganadora (Int64) ≠ claves de vecinos (Int32) y valor `byte` vs `int` ⇒ ver A1.
6. `pot` solo llega a 3^13 ⇒ 15/16 partidos fallan.

**Complejidad / puntos calientes.** ≈ 19 k altas en el mapa por llamada (hasta 4 categorías de profundidad); coste dominante: `AnalizaColumna` × 19 k y la carga del `.comb` por llamada. Para un port: `ContiguousArray<(clave, etiqueta)>` + diccionario solo si se necesita deduplicar (en esta recursión **no hay repetidos**: cada vecino se alcanza una sola vez).

**Trampas Swift.** **T6 (crítica)**: el orden de `ListaEscrutadasConPremio` es el de `Hashtable.Keys` (`docs/PLAN_MEJORAS.md:417,800` lo reconocen). `Dictionary` de Swift es aleatorio ⇒ decidir orden canónico (**A2**). T5: `Array.Sort`+`Reverse` de enteros ⇒ `sorted(by: >)`.
T2: la clave es índice base-3 en `Int64`; no mezclar con la codificación de 3 bits. Boxing `byte` vs `int`: en Swift no existe, pero el comportamiento observable del C# depende de él (**A1**).

**Casos golden propuestos.** `GE-07` (solo la parte de conjunto de candidatas, vía reflexión sobre `aciertos`) y `GE-08` (captura del comportamiento real de `EscrutarCombinacion`).

**Evidencia.** `EscComb.cs:31-57,89-98,100-134,145-188,209-247`; `VM-EscC:403-465`; `Analisis/AnalizadorColumnaCombinacion.cs:38-110`; ausencia de tests: `grep EscrutadorComb Free1X2.Domain.Tests` sin resultados.

---

### 4.3 DTO de premiadas (`ColPrem.cs`, `ColPremComb.cs`, `ListaPremComb.cs`)

**Propósito.** Contenedores de datos de las columnas premiadas.

| Clase | Campos (C# → Swift) | Notas |
|---|---|---|
| `ColumnasPremiadas` (`:7-52`) | `columna: String`, `fichero: String`, `jornada: Int`, `premio: Int`, `noColumna: Int`, `noBoleto: Int` | solo propiedades; las rellena `Escrutador.AñadeAGanadoras` |
| `ColumnasPremiadasComb` (`:9-56`) | `columnaTexto: String`, `columna: Int64`, `fichero`, `jornada`, `premio` | **setters acoplados**: asignar `Columna` (índice base-3) recalcula `columnaTexto = ConvNumAColumna(columna)`; asignar `ColumnaTexto` recalcula `columna = ConvColumnaANumero(texto)`; la anchura del texto es `NumeroPartidos` **al construir el objeto** (`:16`) |
| `ListaPremiadasComb` (`:9-60`) | `columnaTexto`, `columna: Int64`, `premio: Int`; 3 inits: `(String, Int)`, `(Int64, Int)`, vacío | mismo acoplamiento; solo aparece como tipo de un campo y su propiedad en `EscrutadorComb`, que nunca se rellenan ni se leen (`EscComb.cs:38,65-69`) |

**Swift.** `struct` con inicializadores que mantengan el acoplamiento (o un único campo canónico + propiedad calculada) y `anchura` explícita en lugar del global.
**Golden.** Triviales (`GE-06`); la conversión ya la cubre ESPEC_06.

### 4.4 `PosiblesPremiosContenedor` y `PosiblesPremiosComparer` (`PPCont.cs`, `PPCmp.cs`)

**Propósito.** Resumen por ganadora hipotética de las columnas que logran 10…16 aciertos y ordenación de los resúmenes de **mejor a peor**.

**Tipos y datos.** `ColGanadora: String`; `Col10…Col16: [String]` (7 listas); `ValorTotal: Double`.

**Algoritmo.**
```
Comparer.Compare(x, y) (:12-47):  null ⇒ ArgumentException ;   // descendente por nº de elementos, en cascada
    v ← y.Col16.count <=> x.Col16.count ; si v == 0: v ← y.Col15.count <=> x.Col15.count ; … Col14, Col13, Col12, Col11, Col10 ; devolver v          // sin criterio final: empate total = 0
CalcularValorTotal (:69-97):  valor ← Col10.count · 0.00001
    si Col11.count > 0: valor += Col11.count · 1 ; si Col12.count > 0: valor += Col12.count · 500
    si Col13.count > 0: valor += Col13.count + 200000        // ¡ '+' no '*' !
    si Col14.count > 0: valor += Col14.count + 6000000 ;  Col15 → + 600000000 ;  Col16 → + 600000000000 (long)     ; valorTotal ← valor
```
**Peculiaridades.** (1) **Empates totales** son habituales (muchos contenedores con los mismos recuentos) y el orden entre ellos lo decide `List<T>.Sort` (introsort **inestable** para > 16 elementos, T5): `Free1X2/UI/PosiblesPremiosFrm.cs:3393`, `Free1X2/UI/MejoresOpcionesFrm.cs:533`, `Free1X2.WinUI/Views/Ported/PosiblesPremiosFrmViewModel.cs:247`
y las primeras N se muestran (`Free1X2/UI/MejoresOpcionesFrm.cs:535-542`) ⇒ **A3**. (2) `CalcularValorTotal`/`ValorTotal` **no tienen ningún llamador** (código muerto) y mezclan `+` con `*` (parece un error; no portar salvo orden). (3) Las listas guardan **texto**; cada llamador decide su formato (`Free1X2/UI/PosiblesPremiosFrm.cs:2737-2749` añade el nº de aciertos como sufijo).
**Trampas Swift.** T5: `sort(by:)` de Swift **no** es estable garantizado; usar `sorted` con clave `(índiceOriginal)` como último criterio si se decide orden estable, o emular el introsort de .NET si se exige identidad (**A3**). T3: `Double` (no `float`) aquí.
**Golden.** `GE-09`, `GE-10`.
**Evidencia.** `PPCmp.cs:10-48`, `PPCont.cs:9-100`; consumidores arriba.

---

## 5. Qué cubren los tests golden existentes y qué falta

| Componente | Tests existentes | Qué fijan | Qué falta |
|---|---|---|---|
| `JDC` | `T-FIX:60-81` (cols_a, nivel 13); `T-FLU:57-71` (81 columnas, nivel 13) | solo propiedades: `Iniciales`, `0 < Finales ≤ Iniciales`, nº de líneas == `Finales` | **salida exacta** y contadores; niveles ≤ 12; `maxCol`/`percent`; externas; duplicados (`cols_con_repetidas`: líneas ≠ `Finales`); cancelación; anchura de salida |
| `JDCDobleContador` | `T-JDCD` (5): 7T n12/n11/n13(SHA)/8T n12(SHA)/7T n12+externas | salida completa y contadores `Iniciales/Finales/Procesadas` | **partido 14 variable** (quirk de `Marca`); `maxCol`/`percent`; columnas reales (`cols_a`/`cols_b`); duplicados; niveles ≤ 10; otro nº de partidos |
| `ReductorTM` | `T-TM` (6): cols_a n8/n10, universo n12/n11 (exactos); N-02 (una columna); N-03 (sin `Inicializa`) | orden de salida exacto; no cuelga; mismo resultado con/sin `Inicializa` | duplicados; `cols_b`; aviso `ShowInfo` (texto, 1 vez); `Inicializa(otro fichero)`; reutilización; error Int16; línea vacía; N=0 |
| `Redu1305Xfsf` | ninguno | — | todo (salida, contadores, `maxCol+1`, influenciadas) |
| `XFSF`, `XfsfV3`, `JLPM` | ninguno | — | todo (huérfanos; JLPM no termina en algunos casos) |
| `Calculos` | ninguno | — | todo (`NoColumnas`, salida, externas, `seguidos`) |
| `Escrutador` | `T-FIX:31-58` (cols_a vs ganadora); `T-FLU:76-97` (81 cols) | histograma `PremiosTotales` por la ruta de 4 args | comodín `*`, minúsculas, ganadora corta, `AñadirAGanadoras`/`NoBoleto`, temporada, `PonerPremios` 3 args, `AñadirPremiosGlobales`, `EscrutaApuestaMultiple`, línea en blanco |
| `EscrutadorComb` | ninguno | — | todo; **primero** decidir A1 |
| `PosiblesPremios*Comparer/Contenedor` | ninguno | — | orden con empates ≤16 y >16; `CalcularValorTotal` |
| Contrato `ArchivoColumnasTexto` (líneas en blanco, CRLF/LF/CR, BOM, no-creación del fichero) | otros tests de formato no inspeccionados aquí | — | casos de las peculiaridades §1.2 (ESPEC_07) |

Resumen: **15** tests tocan este alcance (5 JDCDoble + 6 TM + 2 de `T-FIX` + 2 de `T-FLU`: `Reducir_JDC` y `Escrutar`; los otros 2 de `T-FLU` ejercitan el generador y solo producen la entrada);
solo **9** fijan una salida completa no trivial (5 JDCDoble + 4 TM); **el resto** de la lógica de reducción y escrutinio no tiene oráculo de salida.

---

## 6. Casos golden propuestos (consolidado)

Entradas disponibles en `Free1X2.Domain.Tests/Fixtures/`: `cols_a.txt` (10 columnas reales distintas), `cols_b.txt` (10; 5 comparten con A), `cols_con_repetidas.txt` (10 líneas, 6 distintas), `ganadora.txt` (= 1.ª de `cols_a`: `21122X1X111122`).
«Universo kT» = fichero que genera `Analizador` con `k` triples `1,X,2` en los primeros partidos y el resto fijo `1` (3^k columnas, orden del generador), como en `T-JDCD:23-31`.
Los valores **[réplica]** entran como `modelo` y se contrastan con una traza `lectura` (ESPEC_12 §1).

| ID | Componente | Entrada y parámetros | Salida a capturar | Valor esperado **[réplica]** |
|---|---|---|---|---|
| GR-01 | JDC | `cols_a`, nivel 13/12/10/8 | líneas, `Iniciales/Finales/Procesadas` | 10 / 10 / **9** / **6** líneas; n=8: `21122X1X111122, XX12X1X1XX1122, 111X222X11XX21, 1X211111X1112X, 21XX1XX2XX1211, 22X1X1221111X1` |
| GR-02 | JDC | `cols_con_repetidas`, nivel 13 y 8 | líneas y contadores | n13: 10 líneas, `Finales=6`, `Procesadas=6`; n8: 5 líneas (`12122X1XX211X1` dos veces), `Finales=4`, `Procesadas=6` |
| GR-03 | JDC / JDCDoble / Redu1305 | universo 5T, nivel 12; `maxCol=5`; `percent=50`; sin límites | `Finales`, `Procesadas`, líneas | JDC: 5/195, 3/153, 9/243 · JDCDoble: 5/177, 4/138, 10/243 · R1305: 5/191, 4/174, 11/243 |
| GR-04 | JDC con externas | universo 7T, nivel 12, `externas=true` | líneas | capturar (comparar con el de `T-JDCD:126-147`) |
| GR-05 | JDCDoble | `cols_a`, nivel 10 y 8 | líneas | n10: 9 líneas (`1122222X11XX11, 21XX1XX2XX1211, 22X1X1221111X1, 1X1X12X1X111X1, 12122X1XX211X1, 1X211111X1112X, 2X11X11XX1122X, 21122X1X111122, XX12X1X1XX1122`); n8: 8 líneas |
| GR-06 | JDCDoble (quirk último partido) | triples en los partidos 1…6 y **14**, resto «1» (3^7 col.), nivel 12 | nº de líneas y SHA | **78** (inclusivo daría 69) |
| GR-07 | JDCDoble | `cols_con_repetidas`, nivel 13 y 8 | líneas | 10 y 10 líneas (con duplicados repetidos) |
| GR-08 | ReductorTM | `cols_con_repetidas` nivel 13/12/8; `cols_b` nivel 13 | líneas en orden de selección | 6 / 6 / 4 líneas (n13: `21122X1X111122, 1X1X12X1X111X1, 12122X1XX211X1, XX12X1X1XX1122, 111X222X11XX21, 1X211111X1112X`); `cols_b` n13: 10 |
| GR-09 | ReductorTM | `cols_a` nivel 13 | `ShowInfo` invocado 1 vez; 10 líneas en orden de índice | grafo sin aristas ⇒ aviso |
| GR-10 | ReductorTM | `Inicializa(A)` + `ComienzaReduccion(B)` | cuál de los dos ficheros se reduce | usa A (matriz de `Inicializa`) |
| GR-11 | Redu1305 | `cols_a` nivel 13/12/10/9/8; universo 5T nivel 13/12/11 | líneas (orden numérico) | 10/10/9/8/7; 50/11/3 |
| GR-12 | Redu1305 | universo 5T, nivel 12, `maxCol=1,2,3,4` | `Finales` y líneas | 2, 3, 4, 4 (límite superado en 1) |
| GR-13 | XFSF / V3 / JLPM | `cols_a` nivel 13/10/8 | líneas | XFSF y V3: 10/9/6 · JLPM: 10/9/5; JLPM en `cols_con_repetidas` no termina ⇒ test «no aplicable» |
| GR-14 | todos (propiedad) | cada caso de arriba | `∀ m ∈ madre ∃ r ∈ salida: aciertos(m,r) ≥ nivel` | verdadero (con `maxCol=0`, `percent=100`); además `Finales ≤ Iniciales` |
| GS-01 | Subir categoría | base `1111…` (14), involucrados {1,2,3}, `(niveles; seguidos)`: `({0,1,2};1)`, `({0,1,2,3};0)`, `({2,3};2)`, `({3};3)`, `({1};0)` | `NoColumnas` y fichero | 19, 27, 20, 8, 6 |
| GS-02 | Subir categoría con externas | base `1111…`, involucrados {1…5}, niveles {1,2}, seguidos 0; externas = columnas del universo 5T con `X` en el partido 1 | `NoColumnas` | 9 (sin externas: 50) |
| GS-03 | Subir categoría (bordes) | `seguidos` > nº de involucrados; `Calcular` dos veces seguidas con distinto nº de involucrados; `ListaInvolucrados` desbordada | resultado o excepción | **capturar** (no hay réplica fiable) |
| GE-01 | Escrutador | ganadora = 1.ª de `cols_a` vs `cols_a` / `cols_b` / `cols_con_repetidas` | `PremiosTotales` | a: `{14:1,8:1,7:2,6:2,5:2,4:2}` (ya en `T-FIX`); b: `{8:1,7:3,6:2,5:2,4:2}`; rep.: `{14:3,8:2,7:1,6:1,5:1,4:2}` |
| GE-02 | Escrutador | `cols_a` con ganadora `21122X1X11112*`, `21122x1x111122`, `***********122`, `2112` | `PremiosTotales` | `{14:1,9:1,8:2,7:1,6:3,5:2}`; = base; `{14:2,13:1,12:5,11:2}`; `{4:1,2:6,1:2,0:1}` |
| GE-03 | Escrutador + premiadas | `cols_a`, ganadora 1.ª, `premios={8,7}`, `AñadirAGanadoras` | `ListaPremiadas` | `(col 3, prem 8, boleto 1)`, `(5, 7, 1)`, `(10, 7, 2)`; `Fichero` = ruta asignada |
| GE-04 | Escrutador temporada | ganadoras = `cols_a` (10 jornadas), columnas = `cols_b` | filas por jornada y totales | `Ac. Totales` por jornada: 59, 51, 50, 47, 52, 60, 56, 45, 60, 60; totales `{1:3,2:6,3:11,4:20,5:25,6:11,7:9,8:5,9:3,10:1,11:1,14:5}` (suma 100) |
| GE-05 | Escrutador filas | `PonerPremios` de 3 y de 4 args; `AñadirPremiosGlobales` | contenido de las filas | 3 args sin `P0`/`Archivo`/`Ac. Totales`; global: `LineaID=0`, `"-------------"`, `"----- TOTALES:"`, `" "` |
| GE-06 | `EscrutaApuestaMultiple` | ganadora `12X` = 140; apuestas `1X,12,X2` = 238; `1X2·3` = 511; `2,1,1` = 289 | aciertos | 3, 3, 0 (cuenta de bits de `ganadora & apuesta`) |
| GE-07 | EscrutadorComb (candidatas) | ganadora 1.ª de `cols_a`, `cats=[14,13,12,11,10]` | mapa `aciertos` (clave→etiqueta), por reflexión | recuentos por etiqueta `14:1, 13:28, 12:364, 11:2 912, 10:16 016` |
| GE-08 | EscrutadorComb (real) | combinación trivial (1 doble) + misma ganadora | ¿excepción?, `PremiosTotales`, orden de `ListaEscrutadasConPremio` | **capturar** (A1/A2) |
| GE-09 | Comparer | ≤ 16 contenedores con empates; > 16 con empates | orden resultante | **capturar** (A3) |
| GE-10 | Contenedor | `Col10=3, Col13=2, Col14=1` | `ValorTotal` | `6 200 003,00003` (si se decide portar) |

---

## 7. Trampas Swift transversales (resumen aplicable)

| T | Dónde en este alcance | Regla |
|---|---|---|
| T1 | `ncent = procesadas·100/iniciales` (Int32; desborda con 16 partidos); `Convert.ToInt16`; casts `(byte)`; `byte` en `Comparador` | `Int` 64 bits + límite 15 partidos, o `&*`; comprobar rango Int16; `UInt8(truncatingIfNeeded:)` si se porta XFSF |
| T2 | índice base-3 (`Int32`) ≠ columna de 3 bits (`Int64`); `EscrutaApuestaMultiple` usa 3 bits | tipos distintos (`typealias IndiceColumna = Int32`, `ColumnaBits = UInt64`); **no** mezclar (A1) |
| T3 | solo `PosiblesPremiosContenedor.ValorTotal` (`Double`) | `Double`, no `Float` |
| T4 | no hay redondeos; sí divisiones enteras (`N·4/10`, `n/8`, `procesadas·100/iniciales`) | mantener enteros |
| T5 | `List<int>.Sort` (JDCdoble), `Array.Sort`+`Reverse` (categorías) ⇒ seguras; `resumen.Sort(PosiblesPremiosComparer)` ⇒ **no** | A3 |
| T6 | `Hashtable` de `EscrutadorComb` | A2; en el resto no hay hash (BitArray y listas son ordenadas) |
| T7 | `Convert.ToInt16`, `ToUpper()`, `ToString()` de enteros | cultura explícita (`Locale(identifier:"en_US_POSIX")`); mayúsculas por unidades ASCII |
| T8 | lectura de líneas, líneas en blanco, `Trim`, BOM, `WriteLine`, creación perezosa del fichero | §1.2; decidir fin de línea de salida (A10) |
| T9 | división/módulo con no negativos | sin riesgo |
| T10 | `NumeroPartidos` en Escrutador, Redu1305, `ConvertidorDeBases()`, `EscrutadorComb`, DTO | parámetro explícito `Sesion.numeroPartidos`, **capturado al construir** donde C# lo captura al construir |
| T12 | `Pump` en bucles; `UserDialogs.ShowInfo` (TM) | `Task.checkCancellation()` + `AsyncStream` de progreso; aviso como evento |
| Swift | `a ..< b` con `a > b` aborta; accesos a array fuera de rango abortan; recursión con campos | `stride`/`while`; errores tipados; profundidad como parámetro; punteros para el bucle caliente |

---

## Abiertas — estado y decisión de cada entrada en [`DECISIONES.md`](DECISIONES.md) §3

| ID | Tema | Qué se sabe / qué falta |
|---|---|---|
| **A1** (bloqueante para `EscrutadorComb`) | `Hashtable` con claves `Int32` e `Int64` y valores `int`/`byte`; índice base-3 pasado a un analizador de 3 bits | Por lectura: las claves de vecinos se insertan como `int` (`EscComb.cs:218-224`), la de la ganadora como `long` (`:243`) y se consultan con `long` (`:120`). `Hashtable` compara con `Equals` del objeto almacenado, y `Int32.Equals(object)` con un `Int64` devuelve falso ⇒ `aciertos[long]` devolvería `null` (`(int)null` lanza), y para la clave `long` el valor es `byte` boxeado (`(int)` sobre `byte` boxeado lanza `InvalidCastException`). Además se pasa un índice base-3 a `AnalizaColumna`, que espera un `long` de 3 bits (en el dominio no hay conversor base-3 → 3 bits; solo existe el inverso 3 bits → base-3, `Conv.cs:68`, sin ningún llamador). **No verificado por ejecución** (no hay tests ni se ha podido ejecutar .NET aquí). Hace falta: traza `lectura` de `GE-08` (ESPEC_12 §4.2) y decidir (R2) si el port replica el comportamiento (excepción/basura determinista) o implementa la semántica pretendida. |
| **A2** | Orden de `ListaEscrutadasConPremio` | Es el orden de `Hashtable.Keys.CopyTo` (buckets de mayor a menor, `docs/PLAN_MEJORAS.md:800`). Su emulación exacta exige el algoritmo interno del framework (tabla de primos, doble hashing, factor de carga): **no está en el repo ni se ha verificado**. Opciones: orden canónico (p. ej. índice ascendente) + comparar golden como conjunto, o capturar el orden C# y emularlo. Depende de A1. |
| **A3** | `PosiblesPremiosComparer` con empates | `List<T>.Sort` es inestable (para ≤ 16 elementos .NET usa inserción, para más introsort; **conocimiento del runtime, no verificado desde el repo**). Decidir: orden estable por posición original, o emular el introsort de .NET 8 y fijarlo con `GE-09`. |
| **A4** | `EscrutadorComb.PonerPremios/AñadirPremiosGlobales` | Siempre `NullReferenceException` (`escrutinioDS` nunca asignado). La UI WinUI los omite. Proponer no portarlos. |
| **A5** | Huérfanos `XFSF`, `XfsfV3`, `JLPM` | Sin llamadores. JLPM no termina en varios casos según la réplica (sin test C#). Propuesta: no portar (R2). |
| **A6** | Quirk `Partido < noPartidos` de `JDCDobleContador.Marca` | Los 5 golden existentes no lo detectan (verificado con réplica: pasan igual con el recorrido inclusivo). Confirmar con `GR-06` (réplica: 78 vs 69). |
| **A7** | `ReductorTM`: límite Int16 y cancelación | `OverflowException` si algún índice > 32 767 en listas de vecinos (`TM.cs:313,325`); `Cancelar` no detiene `Reduce`. ¿Conservar el límite y la no-cancelabilidad, o mejorar? (R2). |
| **A8** | `nivel ≥ noPartidos` y `14` literal | Con `nivel == noPartidos` JDC/JDCDoble/Redu1305 se comportan como `nivel−1` (§1.3); `Dispersa` usa `14 − nivel` fijo; `Redu1305Xfsf` usa `NumeroPartidos` global para la profundidad. Inalcanzable desde la UI (niveles 13…1, 14 signos obligatorios, `VM-Red:36-37,114-130`); decidir si la API Swift lo valida o lo replica. |
| **A9** | Salida tras `Cancelar` | JDC/JDCDoble/Redu1305/V3/XFSF escriben la cobertura **parcial** (no es una reducción válida) o no crean fichero si no hay reductoras. Decidir si el port conserva eso o descarta la salida al cancelar. |
| **A10** | Fin de línea y codificación de salida | `WriteLine` ⇒ `\r\n` en Windows, `\n` en macOS; los ficheros de usuarios Windows usan `\r\n`; los golden C# generados en Mac tendrán `\n`. Decidir en ESPEC_07 (formatos). Lectura: aceptar `\r\n`, `\n` y `\r`. |
| **A11** | Valores **[réplica]** | Salen de re-implementaciones propias (validadas solo donde hay golden: JDCDoble y TM). Entran como `modelo` con su caso `lectura` de contraste (ESPEC_12 §1). |
| **A12** | `Escrutador`: esquema real de `Resultados` | Solo se conoce el esquema de los tests (`T-FIX:17-29`); el real lo crea la UI (`VM-Esc`, `CrearDataSetResultados`), fuera de este alcance. El port debe devolver un modelo de filas propio. |
| **A13** | `Calculos`: `seguidos` > involucrados y estado residual | Comportamiento descrito por lectura; no hay réplica ni test de esos bordes (`GS-03`). |
| **A14** | `ArchivoColumnasTexto.ObtenNumCols()` | Estima `longitud del fichero / (len 1.ª línea + 2)` (supone CRLF); no lo usa ninguno de estos algoritmos, pero lo calcula el constructor (`ACT.cs:47-57,314-340`). Revisar en ESPEC_07 si sigue haciendo falta. |
