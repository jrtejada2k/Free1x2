# ESPEC 01 — Motor núcleo (generador, analizador, grupos, tolerancias, If-Then, estado global)

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Convenciones y trampas T1–T12 en
> [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Este documento describe **qué hace hoy** el C#
> (oráculo) para reescribirlo en Swift sin mirarlo. Los filtros concretos los documentan ESPEC_02/03/04.
>
> **Abreviaturas de ruta** (todas relativas a la raíz del repo): `MC/` = `Free1X2.Domain/MotorCalculo/` ·
> `DOM/` = `Free1X2.Domain/` · `UT/` = `Free1X2.Domain/Utils/` · `ES/` = `Free1X2.Domain/EntradaSalida/` ·
> `AN/` = `Free1X2.Domain/Analisis/` · `WIN/` = `Free1X2.WinUI/Views/Ported/` · `TST/` = `Free1X2.Domain.Tests/`.
> Marca **[PRESERVAR]** = comportamiento raro/aparente bug que afecta a la salida y se replica.
> Las trazas «derivado por lectura» no se han ejecutado: `tools/GoldenGen` (aún inexistente) debe confirmarlas.

**Ficheros cubiertos (24):** `MC/{GeneradorColumnas, Analizador, IFiltro, Grupo, GrupoPartidos, ControlGrupos,
ControladorGrupos, ControlConjuntos, ToleranciaFiltros, ControladorTol, RangosOpciones, FiguraCondicion,
CondicionIfThen, ControladorIfThen, GrupoIfThen, AgrupacionColumnas, EscaleraAciertos, SandwichAciertos}.cs` ·
`DOM/{VariablesGlobales, Enums, EnumFiltros}.cs` · `DOM/Abstractions/{IAppPaths, IProgressNotifier, UiHooks}.cs`.

---

## 0. Modelo de datos mínimo (recordatorio)

- `Columna = Int64` (`long`), 3 bits por partido, partido 0 en los bits 0-2: `'2'`=`001`(1), `'X'`=`010`(2),
  `'1'`=`100`(4); pronóstico doble/triple = OR de los signos (`1X`=6, `12`=5, `X2`=3, `1X2`=7)
  (`UT/UtilColumnas.cs:94-132`, `docs/MANUAL_FLUJOS.md` §2).
- Test de pertenencia al pronóstico: `(base & col) == col` (`MC/GeneradorColumnas.cs:351,360`, `MC/Analizador.cs:168`).
- Formato de pronóstico por partido que consume el motor: **cadena con comas** `"1"`, `"1,X"`, `"1,X,2"`
  (`UT/UtilColumnas.cs:85-88` lee solo las posiciones pares del texto; `Free1X2.WinUI/Controls/BoletoBaseViewModel.cs:142-150`
  produce `string.Join(",", …)`). **[PRESERVAR]** `"1X"` sin coma se leería como solo `1`.

---

## 1. Visión general del pipeline de validación de una columna

### 1.1 Cuatro rutas de entrada (todas pasan por `Analizador.AnalizaCombinacion`)

| Ruta | Llamada de la UI | Generador | Entrada de columnas | Columna → motor | Contenedor análisis | Guarda `.txt` |
|------|------------------|-----------|---------------------|-----------------|---------------------|---------------|
| **A contar** | `AnalizaCombinacion(false)` (`MC/Analizador.cs:256`) | `GenerarColumnas()` (`MC/GeneradorColumnas.cs:149`) | pronósticos (DFS) o archivo con recorte/relleno (`UsaColumnasArchivo()` `:272`) | `Analizador.AnalizaColumna(long)` (`:84`) | `null` (`:268`) | no |
| **B grabar** | `AnalizaCombinacion(string ruta)` (`:182`) → `guardarCols=true` → ruta A | igual que A | igual que A | igual que A | `null` | sí, `GuardarCols(ConvLongToStr(col))` por columna aceptada (`:113-116`) |
| **C analizar** | `AnalizaCombinacion(int n)` (`:196`) → `AnalizaCombinacion(true)` (`:256`) | `GenerarColumnas(true,false)` (`:170`) | pronósticos (DFS) o archivo con **recorte pero sin relleno** (`UsaColumnasArchivo(bool)` `:300`) | `AnalizaColumna(long,bool)` (`:137`) | `InicializarContenedorAnalisis(false)` (`:264`) | no |
| **D analizar externo** | `AnalizaCombinacion(true,true)` (`:304`) | `GenerarColumnas(true,true)` | pronósticos (DFS) o archivo **sin recorte ni relleno** (`UsaColumnasArchivoExterno` `:325`) | `AnalizaColumna(long,bool)` | `InicializarContenedorAnalisis(true)` (`:312`) | no |

`AnalizaCombinacion(false, x)` (2 args, `analizarColumnas=false`) cae en el generador de la ruta A. Los ViewModels usan
`AnalizaCombinacion(ruta)`, `(14|15)`, `(false)` y `(true,true)` (`WIN/CalculaColumnasFrmViewModel.cs:326,330,334`,
`WIN/AnalizarFicheroFrmViewModel.cs:128`). **[PRESERVAR]** el overload de 2 args pasa **siempre** `esAnalisisExterno=true`
al generador (`MC/Analizador.cs:337`) aunque `esArchivo` sea `false`; el de 1 arg pasa **siempre** `false` (`:289`).

### 1.2 Orden exacto de llamadas por columna — ruta A/B (sin análisis)

```
GeneradorColumnas.analizaNueva(col)                              MC/GeneradorColumnas.cs:357-364
 └ si (columnaBase & col) == col:                                (cortocircuito 0: barato, descarta casi todo en modo pronóstico)
    Analizador.analizaColumna(col)                               MC/Analizador.cs:84-119
     1. valida = false
     2. si (pronosticoBase & col) == col:                        (cortocircuito 1; en modo pronóstico siempre true: columnaBase == pronosticoBase, ver §2.2)
          ctrlGrupos.recalcularControladorGrupos()               MC/ControladorGrupos.cs:270-278  → pone a true reCalcular* de:
                                                                   ControladorGrupos, cada ControlGrupos, y cada Grupo listado en gruposControlados
                                                                   (MC/ControlGrupos.cs:283-291)
          valida = ctrlGrupos.analizaColumna(col)                MC/ControladorGrupos.cs:64-81
     3. si ifThen != nil && !ifThen.esVacio && ifThen.esActivo:
          valida = ifThen.compruebaPronostico(col, grupos)       ← ASIGNA, no hace AND  [PRESERVAR] MC/Analizador.cs:97-103
     4. bombearUiSiToca()                                        solo UI; no afecta al resultado (MC/Analizador.cs:127-136)
     5. noColsAnalizadas += 1
     6. si valida: noColsAceptadas += 1; si guardarCols: archivo.guardarCols(convLongToStr(col))
```

Dentro de `ctrlGrupos.analizaColumna(col)` (memoizado por `reCalcularControladorGrupo`, `MC/ControladorGrupos.cs:68-80`):

```
si usaControlConjuntos → AnalizaColumnaConConjuntos        MC/ControladorGrupos.cs:95-153
    a. conjunto[0] («libres»): para j en orden: controlesGrupos[idx_j].analiza(col); el primero que falla → INVÁLIDA y corte
    b. si pasó (a): para i = 1..<conjuntos.count: cuenta TODOS los ControlGrupos del conjunto que fallan (sin corte interno);
       inválida si  fallosPermitidos.count < nFallos+1  o  fallosPermitidos[nFallos] == false  → corte entre conjuntos
sino → AnalizaColumnaSinConjuntos                          MC/ControladorGrupos.cs:83-94
    para cada ControlGrupos en orden: si falla → false (corte); si todos pasan → true
```

`ControlGrupos.analiza(col)` (`MC/ControlGrupos.cs:43-50`):

```
si usaControlGrupos (hay «fallos permitidos»):                 MC/ControlGrupos.cs:61-90  (memo reCalcularControlGrupo)
    nFallos = nº de grupos controlados cuyo Grupo.analiza(col) es false      (SIN corte: se evalúan todos)
    valida = fallosPermitidos[nFallos]
sino:                                                           MC/ControlGrupos.cs:92-103  (sin memo propio)
    todos los grupos controlados deben aceptar; corte en el primero que falla
```

`Grupo.analiza(col)` (`MC/Grupo.cs:165-225`, memo `reCalcularGrupo`):

```
si reCalcular:
   nuevaCol = todosPartidosActivos ? col : col & mascara
   para f en filtros (orden fijo de 14, §2.4): si f.isActive y !f.analizar(nuevaCol) → valida=false; break
   si valida: tolerancias del grupo (SonTolGrupoValidas / …ConFallos)           MC/Grupo.cs:192-201
   si valida y usaFiltroParcial: nuevaCol ∈ bits (archivo del grupo, cargado una vez)   MC/Grupo.cs:203-212
   grupoValidoMemoria = valida; reCalcular = false
si no: valida = grupoValidoMemoria
```

### 1.3 Orden exacto — rutas C/D (con análisis)

```
GeneradorColumnas.analizaNueva(col, true)                         MC/GeneradorColumnas.cs:348-355   (mismo test (columnaBase & col)==col)
 └ Analizador.analizaColumna(col, _)                              MC/Analizador.cs:137-165          (el 2º parámetro NO se usa)
     1. ctrlGrupos.recalcularControladorGrupos()                  SIN comprobar pronosticoBase (solo pasa el AND de 48 bits o del pronóstico del generador)
     2. valida = ctrlGrupos.analizaColumna(col, contenedor)       MC/ControladorGrupos.cs:155-179
     3. si IfThen activo y no vacío:  si !ifThen.compruebaPronostico(...) → valida = false   ← AND  [PRESERVAR; difiere de la ruta A]
     4. bombear; noColsAnalizadas += 1
     5. si valida: contenedor.analisisGrupos.incrementarContador(col); noColsAceptadas += 1
```

Diferencias respecto a A: (i) `Grupo.analiza(col, contenedor)` (`MC/Grupo.cs:226-300`) evalúa también filtros **inactivos** con `AnalisisActivo`
y registra los del grupo base en `contenedor.FiltrosTemp`; (ii) **ignora el filtro parcial** del grupo; (iii) usa `nuevaCol = col & mascara`
siempre, con el parche `si nuevaCol==0 → nuevaCol=col` (`:232-233`); (iv) `ControlGrupos` con «fallos permitidos» evalúa sus grupos con el
overload **sin** análisis (`MC/ControlGrupos.cs:119`) — los filtros no se registran en el contenedor por esa vía; (v) `incrementarContador`
consume los filtros registrados (`AN/ContenedorAnalisis.cs:109-…`) y vacía `FiltrosTemp` al final.

### 1.4 Memoización

Tres niveles con el mismo patrón `reCalcular*` + `*ValidoMemoria`: `ControladorGrupos` (`MC/ControladorGrupos.cs:35-36`),
`ControlGrupos` (`MC/ControlGrupos.cs:40-41`) y `Grupo` (`MC/Grupo.cs:41-42`). El flag se activa **una vez por columna** desde
`Analizador` (`:91`, `:141`), de modo que la memoria solo sirve **dentro de una columna** (un grupo evaluado desde varios controles/If-Then).
`ControlGrupos` sin fallos permitidos no tiene memo propio (`MC/ControlGrupos.cs:92-103`); depende del memo de `Grupo`.
**[PRESERVAR] riesgo de memo obsoleta:** en la ruta A, si `(pronosticoBase & col) != col` el `Analizador` **no** llama a
`recalcularControladorGrupos` (`:89-94`) y aun así evalúa `IfThen` (`:97-103`); `GrupoIfThen` llama a `Grupo.analiza` y recibiría el veredicto
**memoizado de la columna anterior** (`MC/GrupoIfThen.cs:67-81`). Solo ocurre con archivo-filtro (en modo pronóstico el AND del generador ya
garantiza `compruebaPronostico`). Idem para un `Grupo` no listado en ningún `ControlGrupos.gruposControlados`: nunca se invalida.

### 1.5 Contadores y detalles de conteo

- `noColsAnalizadas` cuenta **solo** columnas que superan `(columnaBase & col)==col` en el generador. En modo pronóstico = nº de combinaciones del
  boleto (test: 10 fijos + 4 triples → 81, `TST/FlujosClaveTests.cs:23-30`); en modo archivo `columnaBase` es `2^48-1` (`MC/GeneradorColumnas.cs:30`),
  luego cuenta todas las líneas del archivo (salvo basura con bits ≥48) aunque no estén en el pronóstico.
- `noColsAceptadas ≤ noColsAnalizadas`. Ambos `Int32` en C# (≤ 3^16 = 43 046 721 en modo pronóstico; no desbordan). En Swift `Int`.
- Los dos contadores se ponen a 0 en `InicializaContadores` (`MC/Analizador.cs:457-461`) al empezar cada `AnalizaCombinacion(bool)` (`:271`, `:319`); `ticksUltimoPump` no se reinicia.

### 1.6 Cortocircuitos (resumen)

1. Generador: AND con `columnaBase` (descarta antes de llamar al motor).
2. Analizador (A): AND con `pronosticoBase`.
3. `ControladorGrupos`: primer `ControlGrupos` que falla (sin conjuntos) o primer libre que falla (con conjuntos).
4. `ControlGrupos` sin fallos: primer grupo que falla. Con fallos: **sin** corte, cuenta fallos.
5. `Grupo`: primer filtro activo que falla; las tolerancias y el filtro parcial solo se miran si todos los filtros pasaron.
6. `CondicionIfThen`: si el «If» no se cumple, el «Then» no se evalúa y la condición **se cumple** (`MC/CondicionIfThen.cs:291`).

### 1.7 Contrato de construcción (orden obligatorio)

1. `Configuracion` (nº de partidos N) cargada **antes** de crear cualquier `Grupo`/filtro (los filtros dimensionan arrays con N: p. ej. `MC/FiltroNoVariantes.cs:50-52`).
2. `ControladorGrupos` y `GrupoPartidos` enlazados (`gp.CtrlGrupos=ctrl`, `ctrl.GruposPartidos=gp`) **antes** de `AddGrupo`
   (`MC/GrupoPartidos.cs:44` desreferencia `ctrlGrupos.ControlesGrupos[0]`; ver `MC/Analizador.cs:59-67`).
3. `Grupo.PonerPartidosActivos` **antes** de `AddGrupo` (la máscara se calcula en `AddGrupo`, `MC/GrupoPartidos.cs:39`; `PonerPartidosActivos` no la recalcula).
4. `ControlGrupos.PonerGruposControlados` **antes** de `PonerFallosPermitidos` (usa `gruposControlados.Length`, `MC/ControlGrupos.cs:210`).
5. `ControlConjuntos.PonerCtrlGruposControlados` **antes** de `PonerFallosPermitidos` (`MC/ControlConjuntos.cs:38`).
6. `ControladorIfThen`: añadir **todas** las condiciones/grupos y **después** fijar `RangoAciertoCondiciones/Grupos` (el `bool[]` se dimensiona con `Count+1` en ese instante, `MC/ControladorIfThen.cs:66,83`). Así lo hace el cargador (`ES/ArchivoCondiciones.cs:104-141`).
7. `ToleranciaFiltros`: `Aciertos` (parsea) y `LetrasTol`; luego `ControladorTol.Tolerancias=lista` y por último `FallosPermitidos` (`ES/ArchivoCombinacion.cs:276-288`).

---

## 2. Componentes

### 2.1 GeneradorColumnas — `MC/GeneradorColumnas.cs`

**Propósito.** Enumera las columnas candidatas (DFS bit a bit sobre las 3^N variaciones o líneas de un archivo), las criba con un AND y las entrega a `Analizador`.

**Tipos y datos.**

| Campo C# | Swift | Notas |
|----------|-------|-------|
| `long columnaBase` (`:10`) | `Int64` | pronóstico: `ConvStrToLong(Pronosticos)` (`:23`); archivo: `281474976710655` = 2^48−1 (`:30`) |
| `long columnaInicial` (`:11`) | `Int64` | «todo unos» = `4·(8^N−1)/7`; tabla N=1..16 en `:36-91` (N=14 → 2513169434916, 15 → 20105355479332, 16 → 160842843834660). **Sin `default`:** N∉1…16 deja 0 |
| `string archColsBase` (`:12`) | `String` | `""` ⇒ modo pronóstico |
| `int noPartidos` (`:14`) | `Int` | pronóstico: `Pronosticos.Length` (`:22`); ctor de archivo **no lo asigna** (0) |
| `bool generacionEnMarcha` (`:16`) | atómico `Bool` | escrito desde otro hilo por `PararGeneracionCols` (`:338-341`) |
| `Analizador analizador` (`:18`) | `weak var` | el `Analizador` es dueño del generador (`MC/Analizador.cs:30`); evitar ciclo de retención |

**Entradas/salidas.** `init(pronosticos:[String])` / `init(archivo:String)`; `generarColumnas()`; `generarColumnas(generandoAnalisis:esAnalisisExterno:)`; `pararGeneracionCols()`; setter `analizador`. Efecto: llama a `Analizador.analizaColumna` (§2.2). Lee el archivo-filtro en streaming.

**Algoritmo.**

```
generarColumnas()                                   // rutas A/B  (:149-169)
  enMarcha = true
  si archColsBase != "": usaColumnasArchivo()
  si no: analizaNueva(columnaInicial); genera(columnaInicial, 0, 1)      // la raíz se analiza SIEMPRE, sin mirar enMarcha
  enMarcha = false

genera(v, pos, prof):                               // (:199-228) ; versión con flag análisis idéntica salvo analizaNueva(_, flag)
  si !enMarcha: return
  mascara = 7 << (3*pos)
  para p desde pos hasta noPartidos-1:
     x = (v & ~mascara) ^ (1 << (3*p+1))     // signo 'X'
     analizaNueva(x);  si prof < noPartidos: genera(x, p+1, prof+1)
     d = (v & ~mascara) ^ (1 << (3*p))       // signo '2'
     analizaNueva(d);  si prof < noPartidos: genera(d, p+1, prof+1)
     mascara <<= 3
```

`prof` vale siempre `pos+1`, luego `prof < noPartidos` ⇔ `pos ≤ noPartidos−2`; omitir esa recursión (pos = N−1) solo evita una llamada vacía: se puede recursar siempre. **Orden de emisión = preorden**: raíz; luego, por cada posición `p` ascendente, primero la variante `X` (con todo su subárbol) y después la `2`. Equivale a ordenar las columnas por la secuencia de pares `(posición,signo)` de sus signos distintos de `1`, comparando pares con `posición` ascendente y `X<2`, y el prefijo antes que sus extensiones. Traza para tres posiciones libres (las otras fijas a `1`), derivada por lectura:
`111, X11, XX1, XXX, XX2, X21, X2X, X22, X1X, X12, 211, 2X1, 2XX, 2X2, 221, 22X, 222, 21X, 212, 1X1, 1XX, 1X2, 121, 12X, 122, 11X, 112` (27 = 3^3). Con la máscara de pronóstico `1,X` ×3 quedan, en este orden: `111, X11, XX1, XXX, X1X, 1X1, 1XX, 11X`. El orden es **observable** (orden del `.txt` de salida) y se preserva aunque se optimice la enumeración (se puede generar solo columnas contenidas en el pronóstico siempre que se conserve la subsecuencia: los contadores solo cuentan las contenidas).

```
analizaNueva(col)          : si (columnaBase & col) == col: analizador.analizaColumna(col)            (:357-364)
analizaNueva(col, flag)    : si (columnaBase & col) == col: analizador.analizaColumna(col, flag)      (:348-355)

usaColumnasArchivo()                      // rutas A/B (:272-299)
  mientras enMarcha y hay línea:
     s = línea.trim().mayúsculas()                                   // ES/ArchivoColumnasTexto.cs:214-217 (NO quita comas pese al nombre)
     N = Configuracion.numeroPartidos
     si s.count > N : col = convStrToLong(prefix(s,N))
     si s.count < N : col = convStrToLong(s, completarCon)           // "" ⇒ rellena con '1' hasta N (UT/UtilColumnas.cs:168-195)
     si no          : col = convStrToLong(s)
     analizaNueva(col)
usaColumnasArchivo(flag)                  // ruta C (:300-324): recorta si count>N; NO rellena si count<N; analizaNueva(col, TRUE)
usaColumnasArchivoExterno(flag)           // ruta D (:325-336): sin recorte ni relleno;                  analizaNueva(col, TRUE)
```

**Estado global.** Lee `VariablesGlobales.NumeroPartidos` en `InicializarColumnaInicial()` (`:38`, solo ctor de pronósticos) y en `UsaColumnasArchivo*` (`:281,284,287,309,312`). No escribe nada.

**Peculiaridades a preservar.**

1. El flag que se pasa a `AnalizaColumna(col, flag)` es el literal `true` en `UsaColumnasArchivo(bool)` y `Externo` (`:321,333`); en el DFS se pasa `generandoAnalisis` (`:189,239,248`). El parámetro nunca se usa aguas abajo (`MC/Analizador.cs:137`), así que es inocuo.
2. Ruta A recorta/rellena y la C solo recorta; la D no toca la longitud. `CompletarCon` solo influye en la ruta A/B. WinUI **nunca** asigna `CompletarCon` (solo WinForms, `Free1X2/UI/MainForm.cs:536-538`): en WinUI queda `""` ⇒ relleno con `'1'`.
3. `noPartidos` (= `Pronosticos.Length`) y `columnaInicial` (según `VariablesGlobales.NumeroPartidos`) pueden divergir: si `Pronosticos.Length` > N de config, las posiciones extra parten de `000` y nunca reciben `'1'` ⇒ enumeración incompleta. En todos los flujos UI coinciden (`WIN/CalculaColumnasFrmViewModel.cs:154-158`). Replicar la misma derivación (no «arreglarla»).
4. Línea vacía: ruta A ⇒ `convStrToLong("", "")` = todo unos (relleno de N '1'); rutas C/D ⇒ columna `0` (se analiza, `(base & 0)==0`). Carácter ajeno a `1X2` ⇒ `1 << −1` (ver T1 abajo) ⇒ XOR con `Int32.min` extendido a 64 bits que activa bits 31…63 ⇒ casi siempre rechazada por el AND de 48 bits; si el carácter está en el índice ≥11 (base 0) de la línea, los 33+ desplazamientos posteriores expulsan los bits basura y la columna **se acepta con ese partido a `000`**. Derivado por lectura de `UT/UtilColumnas.cs:196-204`.
5. `PararGeneracionCols` solo baja el flag: los marcos de recursión ya activos **siguen** recorriendo su `for` y analizando las variantes restantes sin recursar (el `return` está al entrar, `:201`), así que tras «parar» se analizan hasta ~2·N columnas más. No determinista ⇒ no entra en golden.
6. Código muerto, **no portar**: `UsaColumnasArchivoB` (`:260-271`) y `AnalizaNuevaColumna(string)` vacío (`:343-346`). El ctor de archivo abre el archivo solo para `ObtenNumSignos()` y calcular un `columnaInicial` que nunca se usa en modo archivo (`:31-34`).

**Trampas Swift.**
- T2/T1: todo en `Int64`; `Int64(1) << Int64(3*p+1)` es seguro (p ≤ 15). Paréntesis explícitos: en C# `<<` > `&` > `^`; en Swift `<<` > `&` (precedencia de multiplicación) > `^` (adición): misma agrupación `(v & ~m) ^ (1<<k)`, pero no confiar en ello con `==`.
- **T1 (desplazamiento enmascarado):** `1 << "2X1".IndexOf(c)` con `IndexOf=−1` en C# enmascara el contador a 5 bits ⇒ `Int32.min`. En Swift `1 << -1` (desplazamiento «inteligente») da 0 o `1>>1`; usar `Int32(1) &<< Int32(idx)` y extender con signo a `Int64` en el XOR.
- `String.Split(',')` de C# conserva vacíos; `Substring/Split` de Swift no: `split(separator:omittingEmptySubsequences:false)`.
- Lectura de líneas estilo `StreamReader.ReadLine` (`\n`, `\r\n`, `\r`; un `\n` final no genera línea vacía extra). `Trim()` .NET = `Char.IsWhiteSpace`; `ToUpper()` usa la cultura actual (inocuo para ASCII; usar `uppercased()` sin locale).
- T12: `async` + cancelación cooperativa; `generacionEnMarcha` atómico (`Atomic<Bool>`/`os_unfair_lock`), no `Task.isCancelled` solo (el contrato C# conserva contadores parciales y sigue hasta el visor, §2.2).

**Casos golden propuestos.** G-GEN-1: N=14, pronósticos `"1"`×11 + `"1,X,2"`×3 ⇒ volcar las 27 columnas en orden (comparar con la traza). G-GEN-2: `"1,X"`×3 + `"1"`×11 ⇒ 8 columnas (traza arriba). G-GEN-3: archivo con líneas `1X2` (len 3), `` (vacía), `1X2…` (len 16), `xx11…` minúsculas, línea con coma; volcar `col` por línea en rutas A/C/D y contadores. Valor de referencia: `"1X2"` rellena con 11 `'1'` ⇒ 2513169434708 (= 2513169434916 − 292 + 84, derivado). Existentes: 81 y 46656 (`TST/FlujosClaveTests.cs:23-30`, `TST/ManualFuncionalTests.cs:78-88`).

**Evidencia.** `MC/GeneradorColumnas.cs:10-18,20-34,36-91,149-197,199-258,272-336,338-369`; `ES/ArchivoColumnasTexto.cs:183-217,342-367`; `UT/UtilColumnas.cs:168-204`.

---

### 2.2 Analizador — `MC/Analizador.cs`

**Propósito.** Orquestador: guarda pronósticos/archivo-filtro/If-Then/grupos, inicializa contadores y parámetros, lanza el generador y valida cada columna (§1.2-1.3).

**Tipos y datos.**

| Campo | Swift | Inicial / notas |
|-------|-------|-----------------|
| `string[] pronosticos` (`:32`) | `[String]?` | `nil` hasta el primer `SetPronostico`; entonces `[String](repeating:…, count: numPartidos)` con elementos `nil` (`:439-443`) |
| `long pronosticoBase` (`:33`) | `Int64` | `ConvStrToLong(Pronosticos)` en cada `AnalizaCombinacion(bool)` (`:409-412`) |
| `archColumnasBase`, `completarCon` (`:35-36`) | `String` | `""` |
| `int noColsAnalizadas/Aceptadas` (`:37-38`) | `Int` | 0 |
| `int numPartidos` (`:39`) | `Int` | `VariablesGlobales.NumeroPartidos` al construir; `AnalizaCombinacion(int)` y `Analizador(int)` lo cambian |
| `ContenedorAnalisisGlobal contenedorAnalisisGlobal` (`:40`) | `ContenedorAnalisisGlobal?` | `nil` en rutas A/B |
| `ControladorGrupos ctrlGrupos` (`:42`) | `ControladorGrupos` | creado en ctor |
| `bool guardarCols, analizarCols` (`:44-45`) | `Bool` | false |
| `IArchivoColumnas archivoCols` (`:46`) | escritor de columnas | solo en ruta B |
| `ControladorIfThen ifThen` (`:47`) | `ControladorIfThen?` | `nil` |
| `long ticksUltimoPump` (`:55`) | `Int64` | `Environment.TickCount64` al construir; solo UI |
| `GeneradorColumnas gc` (`:30`) | `GeneradorColumnas?` | se recrea en cada `AnalizaCombinacion(bool)` |

**Constructores** (`:57-82`): ambos crean `ControladorGrupos`, un `GrupoPartidos` enlazado (`CtrlGrupos`↔`GruposPartidos`), un `Grupo` base con `EsGrupoBase=true` y lo añaden con `AddGrupo` (⇒ máscara 0, control libre `"0"`, §2.4/2.6). `Analizador(int)` además fija `numPartidos`.

**Entradas/salidas.** `setPronostico(i, str)`; propiedades `pronosticos`, `archivoColumnasBase`, `completarCon`, `ifThen`, `gruposPartidos` (setter reenlaza `CtrlGrupos`, `:491-500`), `ctrlGrupos`, `colsAnalizadas`, `colsAceptadas`, `hayDatos`, `contenedorAnalisisColumnasGlobal`; `analizaCombinacion(...)` (4 overloads + 1), `pararAnalisis()`. Efectos: fichero de columnas (ruta B), hooks `UiPump.Pump`/`AnalisisUi.MostrarVisor` (§2.24).

**Algoritmo.** Cadena por columna en §1.2/§1.3. Resto:

```
analizaCombinacion(rutaSalida):                                   :182-194
   guardarCols = true; archivoCols = ArchivoColumnasTexto(rutaSalida)
   analizaCombinacion(false)                                      // el fichero se crea PEREZOSAMENTE en la 1ª columna aceptada
   archivoCols.cerrar(); archivoCols = nil; guardarCols = false   // sin try/finally en C#: ante excepción guardarCols queda true

analizaCombinacion(n: Int):  numPartidos = n; analizaCombinacion(true)          :196-200

analizaCombinacion(analizar: Bool)                                :256-303   (y (analizar, esArchivo) :304-349)
   analizarCols = analizar
   si analizar: InicializarContenedorAnalisis(esArchivo)  // 1 arg: false ; 2 args: el parámetro
   si no: contenedor = nil
   inicializaContadores(); inicializaParametros(); inicializaPronosticoBase()
   gc = archColumnasBase != "" ? GeneradorColumnas(archivo: archColumnasBase) : GeneradorColumnas(pronosticos)
   gc.analizador = self
   si analizar: gc.generarColumnas(true, esAnalisisExterno: false /*1 arg*/ ó true /*2 args*/)  si no: gc.generarColumnas()
   si contenedor != nil y noColsAceptadas > 0:
        (solo 1 arg) UiPump.Pump()                                // «fidelidad legacy» :299-300
        AnalisisUi.MostrarVisor(contenedor, gruposPartidos[0])
```

`InicializarContenedorAnalisis(esArchivo)` (`:351-408`):
- `esArchivo=false`: `contenedor = ContenedorAnalisisGlobal(gruposPartidos.count, ctrlGrupos.controlesGrupos.count, VariablesGlobales.NumeroPartidos)` (el 2º argumento se llama `noConjuntos` pero recibe nº de **controles de grupos**). Toma del grupo 0 `Simetrias`, `Diferencias`, `ValoracionSignos`, `ColProbables`, `FormatosSignos` (por nombre, §2.4) y copia `UsaX = filtro.IsActive`; si `UsaSimetrias`: `NoAciertosSimetrias = filtroSim.ArrayAciertos` + `InicializarSimetrias()`; si `UsaDiferencias`: un `ContenedorDiferencias(dif.PartidosSimetricos.Count)` por diferencia; si `UsaValoraciones`: `NoValoresValoracion{Global,Unos,Equis,Doses} = ObtenerArrayAciertos(filtroVal.Valor{Global,Unos,Equis,Doses})` + `InicializarValoraciones()`; si `UsaCPs`: `InicializarCPs(filtroCPs.ColProbables)`; si `UsaFormatos`: `InicializaFormatosSignos([lineasFormatos.count por FormatosSignos])`; `EsAnalisisExterno=false`.
- `esArchivo=true`: `ContenedorAnalisisGlobal(grupos.count, controles.count, numPartidos)` con `EsAnalisisExterno=true`; no consulta filtros.
- `ObtenerArrayAciertos(s)` (`:229-255`): `s==""` ⇒ `[]`. Trocea por `#`; cada trozo sin `-` ⇒ `Convert.ToInt32`; con `-` ⇒ `a…b` con `a=Round(ToDouble(p0))`, `b=Round(ToDouble(p1))` (`Math.Round` bancario, `Convert.ToDouble` con **cultura actual**), todos los enteros de `a` a `b`; al final `Sort()` (enteros ⇒ determinista; **conserva duplicados**).

`inicializaParametros()` (`:424-435`): construye `AConfiguracion` desde el directorio de la app (lanza si falta `parametros.free1x2`) y, para cada grupo, `ActualizaParametrosFiltros` ⇒ `InicializaPuntosCP` ⇒ lee `<CP><puntos fijos dobles triples>` **del fichero en cada ejecución** y los propaga a `FiltroColProbables`/`ColumnaProbable` (`MC/Grupo.cs:147-158`, `MC/FiltroColProbables.cs:150-157`). No pasa por `VariablesGlobales` (sus `PuntosFijos/Dobles/Triples` no tienen lectores, §3).

`hayDatos` (`:463-479`): `true` si hay archivo base; o `ControlesGrupos.Count>1`; o `GruposPartidos.Count>1`; o grupo 0 con algún filtro activo; o `ifThen` no vacío; o algún `pronosticos[i] != "1,X,2"`. Si `pronosticos==nil` ⇒ NRE.

**Estado global.** Lee `VariablesGlobales.NumeroPartidos` (`:39,356`). Lee `parametros.free1x2` (`:426`, `MC/Grupo.cs:61`). No escribe globales. Hooks: `UiPump.Pump` (`:133,300`), `AnalisisUi.MostrarVisor` (`:301,346`).

**Peculiaridades a preservar.**
1. **Asignación vs AND de If-Then:** ruta A/B `valida = ifThen.compruebaPronostico(...)` (`:97-103`) — con If-Then activo y no vacío el veredicto final es **solo** el del If-Then (ignora pronóstico y grupos); ruta C/D hace AND (`:146-152`). Resultado: la misma combinación puede dar distinto nº de aceptadas contando (A) que analizando (C). Ver G-IFTHEN-OVR.
2. `compruebaPronostico` (AND con `pronosticoBase`) existe **solo** en la ruta A/B; en C/D no se comprueba (`:137-165`); en modo archivo + análisis las columnas fuera del pronóstico se analizan.
3. Overloads `(bool)` / `(bool,bool)`: el segundo no llama `UiPump.Pump()` antes del visor y fuerza `esAnalisisExterno=true` (§1.1). `AnalizaCombinacion(int n)` solo cambia `numPartidos` (afecta a la longitud del array de `SetPronostico` si aún es `nil` y a `InicializarContenedorAnalisis(true)`); **no** cambia el nº de partidos del motor ni del contenedor no-externo.
4. Ruta B: si no se acepta ninguna columna el archivo **no se crea ni se sobrescribe** (escritor perezoso, `ES/ArchivoColumnasTexto.cs:70-80`; `Cerrar` solo cierra si existe). Escritura UTF-8 sin BOM, `WriteLine` ⇒ `\r\n` en Windows (T8; decidir en ESPEC_07). Texto de cada línea = `ConvLongToStr(col)` (`UT/UtilColumnas.cs:27-44`: emite desde el partido 0 y se detiene cuando el resto de bits es 0 ⇒ **recorta partidos finales con `000`**; con un doble/triple lanzaría `IndexOutOfRange` — no ocurre con columnas simples).
5. `PararAnalisis()` solo llama a `gc.PararGeneracionCols()` si `gc != nil`; tras parar, `AnalizaCombinacion` **continúa** y muestra el visor si `noColsAceptadas>0`.
6. `BombearUiSiToca` se evalúa **antes** de incrementar `noColsAnalizadas` y muestrea cada 4096 columnas (`(n & 0xFFF)==0`); solo emite `Pump` si pasaron >800 ms (`:127-136`). Sin efecto en resultados; en Swift sustituir por progreso periódico.
7. `AnalizaColumna(col, flag)` desreferencia `ContenedorAnalisisColumnasGlobal` (`:143,161`) ⇒ NRE si se llama sin contenedor.
8. Excepciones C# que terminan la ejecución y llegan al ViewModel (`Error en el cálculo: …`, `WIN/CalculaColumnasFrmViewModel.cs:340`): config ausente, `pronosticos==nil`, `Pronosticos[i]==nil`, etc. En Swift: `throws`.

**Trampas Swift.** T4 `Math.Round` ⇒ `.rounded(.toNearestOrEven)`; T7 `Convert.ToDouble/ToInt32` cultura actual y toleran espacios y signo (`Convert.ToInt32("")` lanza): usar un parser propio con locale explícito y documentado; `a-b` con negativos no soportado (el `-` inicial rompe `Split`); T5 `List<int>.Sort` de enteros es determinista; T10 estado global ⇒ `Configuracion`; T12 `UiPump` ⇒ progreso `AsyncStream`; el motor debe ejecutarse fuera del hilo principal; contadores `Int` (T1 sin riesgo).

**Casos golden propuestos.** Existentes: `TST/FlujosClaveTests.cs:23-30` (81/81), `TST/ManualFuncionalTests.cs:30-88` (NoVariantes `0/1/2` ⇒ 1/28/364; CP «4 grandes» ⇒ 9; 6 triples+6 dobles ⇒ 46656). Nuevos: G-AN-1 `(analizadas, aceptadas)` en rutas A y C con idénticas condiciones y un If-Then (comparar); G-AN-2 ruta B con 0 aceptadas sobre fichero preexistente (debe quedar intacto); G-AN-3 `ObtenerArrayAciertos` con `"1-3#5#2-2#3"` ⇒ `[1,2,2,3,3,5]`, `"2.5-4"` (cultura) y `""`; G-AN-4 `hayDatos` en cada rama.

**Evidencia.** `MC/Analizador.cs:30-82,84-119,127-136,137-165,166-180,182-200,229-255,256-349,351-422,424-446,448-479,481-538`; `WIN/CalculaColumnasFrmViewModel.cs:326-340`.

---

### 2.3 IFiltro — `MC/IFiltro.cs`

Contrato en §4. Resumen: 10 miembros (`Analizar`, `AnalizarFallos`, `IsActive`, `EsNombreFiltro`, `NombreFiltro`, `ContieneDatos`, `ObtenNoAciertosTolerancias`, `UsaFiguras`, `Figuras`, `AnalisisActivo`) en `MC/IFiltro.cs:25-40`.

---

### 2.4 Grupo — `MC/Grupo.cs`

**Propósito.** Subconjunto de partidos (boleto base = grupo 0, subsistemas 1..n) con su cadena fija de 14 filtros, tolerancias entre filtros y filtro parcial por archivo.

**Tipos y datos.**

| Campo | Swift | Notas |
|-------|-------|-------|
| `List<IFiltro> filtros` (`:29`) | `[any FiltroProtocol]` | 14 elementos, **orden fijo** (abajo); el orden decide el corte y el acumulado de tolerancias |
| `ControladorTol ctrlTolerancias` (`:31`) | `ControladorTol` (clase) | `new` en ctor |
| `bool esGrupoBase`, `string nombreGrupo` | `Bool`, `String` | setter de `EsGrupoBase=true` además pone `todosPartidosActivos=true` (`:565-578`); `false` **no** lo revierte |
| `bool[] partidosActivos` (`:35`) | `[Bool]` | longitud = `VariablesGlobales.NumeroPartidos` en el ctor (`:51,67`) |
| `bool todosPartidosActivos` (`:36`) | `Bool` | false por defecto |
| `int numPartidos` (`:38`) | `Int` | init = `VariablesGlobales.NumeroPartidos`, luego **sobrescrito** releyendo `parametros.free1x2` (`configuraBoleto`, `:57-63`) |
| `long mascara = 7` (`:39`) | `Int64` | **[PRESERVAR]** valor inicial 7 (solo partido 0) hasta `CalcularMascara` |
| `reCalcularGrupo=true`, `grupoValidoMemoria=true` (`:41-42`) | `Bool` | memo (§1.4) |
| `Dictionary<long,bool> bits` (`:44`) | `Set<Int64>?` | filtro parcial; solo se usa `ContainsKey`; `nil` ⇒ aún no cargado |
| `string archivoFiltro` (`:47`) | `String` | `""` ⇒ sin filtro parcial (`UsaFiltroParcial`, `:610-620`) |

**Orden de los 14 filtros** (`:94-134`): 0 `NoVariantes`, 1 `ValoracionSignos`, 2 `SignosSeguidos`, 3 `Interrupciones`, 4 `Dibujos`, 5 `PesosNumericos`, 6 `Distancias`, 7 `GruposEquipos`, 8 `Contactos`, 9 `FormatosSignos`, 10 `ColProbables`, 11 `Formatos123`, 12 `Simetrias`, 13 `Diferencias` (distinto del orden del enum `Filtro`, §2.20).

**Algoritmo.**

```
calcularMascara():  m = 0; para i desde N-1 hasta 0 (descendente): m = (m << 3) | (partidosActivos[i] ? 7 : 0); mascara = m     // :69-89 → partido i en bits 3i…3i+2
columnaGrupo(col) = col & mascara                                                                                               // :160-163

analiza(col)   // :165-225  → ver §1.2 (memo, enmascarado condicional, filtros, tolerancias, filtro parcial)
   tolerancias: si ctrlTolerancias.fallosPermitidos(str) == "" → solTolGrupoValidas() si no → solTolGrupoValidasConFallos()
   filtro parcial (solo si valida): si bits == nil: bits = Set( convStrToLong(línea, partidosActivos) para línea en leerTodasCols(false) ); valida = bits.contains(nuevaCol)

analiza(col, contenedor)   // :226-300  (rutas C/D)
   si reCalcular:
      nuevaCol = col & mascara; si nuevaCol == 0: nuevaCol = col                  // base (máscara 0) y grupos sin partidos usan la columna entera
      valida = true
      para f en filtros:
         si f.analisisActivo:
             si f.isActive: valida = f.analizar(nuevaCol); si !valida: break (antes de registrar f)
             si no:         f.analizar(nuevaCol)                                   // resultado ignorado; solo para estadística
             si esGrupoBase: contenedor.añadirFiltroTemp(f)                         // deduplicado por identidad, conserva orden de inserción
         si no, si f.isActive: valida = f.analizar(nuevaCol); si !valida: break
      si valida: tolerancias (igual que arriba)
      memo...
   // NO aplica filtro parcial

solTolGrupoValidas():   // :367-408
   si tolerancias.count == 0: return true                                             // atajo P-09
   reset acumulados de todas las tolerancias
   para f en filtros (orden) con f.isActive: para t en tolerancias (orden): t.suma(f.obtenNoAciertosTolerancias(t.letrasTol))
   return todas t.cumpleTolerancia()           (corte en la primera que no)
solTolGrupoValidasConFallos():  // :331-365  (sin atajo P-09)
   igual acumulado; nFallados = nº de t con !cumple; return ctrlTolerancias.sonFallosPermitidosValidos(nFallados)
```

Otros miembros: `GetFiltro(nombre)` (`:415-431`): recorre `filtros` y devuelve el primero con `EsNombreFiltro(nombre)`; **si ninguno coincide devuelve el último (`Diferencias`)**. `PonerPartidosActivos(str)` (`:470-493`): `Split(',')`; `todosPartidosActivos = (partes.count == numPartidos)` (cuenta, no valores); por cada parte `Convert.ToInt32` y, si `n <= partidosActivos.Length`, `partidosActivos[n-1] = true` (no limpia lo previo; `n=0` o no numérico lanza); **no recalcula la máscara**. `ObtenPartidosActivos()` (`:433-453`) devuelve `"1,3,…"`. `HayPartidosActivos()` (`:454-468`). `EsActivo` = algún filtro activo (`:534-542`). `Partidos` setter ⇒ `partidosActivos=v; calcularMascara()` (`:555-563`). `RecalcularGrupo()` (`:591-594`). `ReinicializaVariablesFiltroParcial()` pone `bits=nil` (`:595-603`).

`ActivaFiltro(IFiltro f)` (`:518-532`): reemplaza en la lista el filtro del mismo `NombreFiltro` por el objeto dado (el antiguo se descarta; referencias externas al viejo quedan obsoletas). `ActivaFiltro(Grupo g, string nombre, bool v)` (`:495-516`): toma de `g.Filtros` el que cumpla `NombreFiltro.ToString()==nombre` (si ninguno, el último), lo coloca en la posición equivalente de **esta** lista, y le pone `IsActive=ContieneDatos=v` — con `g != this` el mismo objeto queda **compartido** por dos grupos. Solo WinForms la usa (`Free1X2/UI/Filtros/DibujosFrm.cs:345`, `ContactosFrm.cs:705`); WinUI usa la de 1 argumento (18 llamadas).

**Estado global.** `VariablesGlobales.NumeroPartidos` (`:38`), `parametros.free1x2` vía `AConfiguracion.ObtenNumPartidos` (`:61-62`) y `ObtenPuntosCP` (`:153`); `filtro.AnalisisActivo` (flags `Analizar*`, §3.2).

**Peculiaridades a preservar.**
1. Máscara: `AddGrupo` la calcula (`MC/GrupoPartidos.cs:39`). El grupo base (todos `false`) queda con **máscara 0** y `todosPartidosActivos=true`; por eso la ruta A no enmascara y la C/D usa el parche `nuevaCol==0→col`. Un grupo no base con `todosPartidosActivos=false` y máscara 0 (sin partidos activos) ve la columna **0** en la ruta A.
2. `InsertarGrupo` no recalcula la máscara (`MC/GrupoPartidos.cs:48-54`); `PonerPartidosActivos` tampoco. Orden de uso correcto: `new Grupo(); PonerPartidosActivos(…); AddGrupo(g)` (`WIN/CrearGruposFrmViewModel.cs:45-48`).
3. Las tolerancias dependen del orden de iteración de los filtros y de que `ObtenNoAciertosTolerancias` use el **estado dejado por el último `Analizar`** de ese filtro: las tolerancias solo se calculan si todos los filtros activos pasaron, luego todos tienen estado de la columna actual. `FiltroDiferencias.ObtenNoAciertosTolerancias` lanza `Exception("not implemented")` (`MC/FiltroDiferencias.cs:124-127`): con ≥1 tolerancia y Diferencias activo ⇒ excepción (con 0 tolerancias el atajo P-09 o el bucle interno vacío lo evitan).
4. Ruta C/D: un filtro **activo con `AnalisisActivo` que falla** rompe el bucle **antes** de `añadirFiltroTemp`; los posteriores no se registran. Filtros inactivos con `AnalisisActivo` se analizan (estado interno actualizado) aunque su resultado se ignore.
5. Filtro parcial: `LeerTodasCols(false)` hace `Trim().ToUpper()` por línea sin quitar comas; `ConvStrToLong(línea, partidosActivos)` pone `000` en partidos no marcados y lanza si la línea es más larga que `partidosActivos`; el stream `aCol` nunca se cierra (cerrar en Swift). `bits` se carga una sola vez hasta `ReinicializaVariablesFiltroParcial`.
6. `GetFiltro` con nombre desconocido devuelve `Diferencias`; `Analizador.UsaSimetrias()` etc. castean el resultado (`MC/Analizador.cs:201-228`) ⇒ `InvalidCast` si los nombres no casan.
7. `configuraBoleto` relee el XML en **cada** `new Grupo()` (I/O + excepción si falta). En Swift: usar `Configuracion.numeroPartidos` (único origen) — equivalente mientras la configuración no cambie en caliente (§3.3).

**Trampas Swift.** `ToleranciaFiltros` y `ControladorTol` deben ser `class` (mutan `noAciertosAcumulados` mientras se itera la lista compartida). Los filtros son objetos con identidad (`HashSet<IFiltro>` por referencia, `AN/ContenedorAnalisis.cs:41-49`) ⇒ `ObjectIdentifier`. `Dictionary<long,bool>` ⇒ `Set<Int64>` (solo pertenencia; sin T6). `mascaraTemp <<= 3` con N ≤ 16 no desborda (48 bits). Evitar copiar el array `filtros` (value semantics) al sustituir con `ActivaFiltro`.

**Casos golden propuestos.** G-GR-1: grupo 0 con `NoVariantes` activo y subgrupo `{1,2,3,4}` con `NoVariantes`; volcar aceptadas por ruta A y C (máscara, parche 0, filtros no registrados). G-GR-2: `GetFiltro("Foo")` ⇒ nombre de clase devuelto (`FiltroDiferencias`). G-GR-3: `PonerPartidosActivos("1,2,3")` + `AddGrupo` ⇒ `mascara == 0b111111111` (511) y 1,2,3 activos; `"0"` ⇒ excepción. G-GR-4: dos tolerancias con letras distintas + `FallosPermitidos` `""` / `"1"` (una por rama).

**Evidencia.** `MC/Grupo.cs:29-63,65-89,90-138,140-158,160-163,165-225,226-300,302-309,311-330,331-408,415-431,433-493,495-542,544-620`; `MC/GrupoPartidos.cs:37-59`; `ES/ArchivoCombinacion.cs:217-261`.

---

### 2.5 GrupoPartidos — `MC/GrupoPartidos.cs`

**Propósito.** Lista de `Grupo` (índice 0 = boleto base) con enlace al `ControladorGrupos`.

**Tipos.** `List<Grupo> grupos` ⇒ `[Grupo]`; `ControladorGrupos ctrlGrupos` ⇒ `weak`. Implementa `IEnumerable` (no genérico).

**Algoritmo.**
- `AddGrupo(g)` (`:37-46`): `g.CalcularMascara(); grupos.append(g); ctrlGrupos.ControlesGrupos[0].PonerGrupo(grupos.count-1)` (añade el grupo al control «libre»).
- `InsertarGrupo(g, pos)` (`:48-54`): `grupos.insert(g, pos)` y `ControlesGrupos[0].PonerGrupo(pos)` (añade **`pos`**, no renumera los demás; no calcula la máscara).
- `BorrarGrupo(pos)` (`:56-59`): solo `RemoveAt`; **no** reindexa los `ControlGrupos` (lo hace la UI: `Free1X2.WinUI/Views/MainPageViewModel.cs:434` y siguientes).
- Indexador `[i]` y `Count`.

**Estado global.** Ninguno. **Peculiaridades.** `PonerGrupo(0)` **reinicia** el control libre a `"0"` (`MC/ControlGrupos.cs:265-268`): insertar en la posición 0 lo resetea. Índices fuera de rango lanzan (`List`); en Swift comprobar y lanzar `ErrorMotor`.
**Trampas.** Ciclo de retención `GrupoPartidos`↔`ControladorGrupos` (usar `weak`). **Golden.** Añadir 3 grupos ⇒ `ControlesGrupos[0].ObtenGruposControlados() == "0,1,2"`; `InsertarGrupo(g,0)` ⇒ `"0"`. **Evidencia.** `MC/GrupoPartidos.cs:25-82`.

---

### 2.6 ControladorGrupos — `MC/ControladorGrupos.cs`

**Propósito.** Raíz de la validación por grupos: lista de `ControlGrupos`, lista de `ControlConjuntos` y memo de columna.

**Tipos y datos.** `List<ControlGrupos> controlesGrupos` (`[ControlGrupos]`), `List<ControlConjuntos> controlesConjuntos`, `GrupoPartidos gruposPartidos`, `bool usaControlConjuntos` (false), `reCalcularControladorGrupo=true`, `controladorGrupoValidoMemoria=true` (`:28-36`).

**Constructor** (`:38-62`): crea `ControlGrupos` #0 con `gruposControlados=[0]`, `UsaControlGrupos=false`, añadido con `AddControlGrupo` (que fija `ctrlGrupo.CtrlGrupos=this`, `:280-284`); crea `ControlConjuntos` #0 con `PonerCtrlGruposControlados("0")` y `PonerFallosPermitidos("0")` ⇒ `ctrlGruposControlados=[0]`, `fallosPermitidos=bool[2]{true,false}`.

**Algoritmo.** Cadenas en §1.2 (sin análisis) y:
- `AnalizaColumna(col)` (`:64-81`): si `reCalcular`: pone `reCalcular=false` **antes** de evaluar; memoiza `usaControlConjuntos ? ConConjuntos : SinConjuntos`.
- Análisis (`:155-179`, `:181-200`, `:201-269`): memo igual (pone `reCalcular=false` **después**). `SinConjuntos(contenedor)`: `valida = ctrl.analiza(col, contenedor)`; corte si `false`. `ConConjuntos(contenedor)`: libres: `j==0` ⇒ `ctrl.analiza(col, contenedor)`; `j>0` ⇒ **antes** `contenedor.VaciarInformacion()` (vacía `FiltrosTemp`) y luego `ctrl.analiza(col, contenedor)`; corte en el primer fallo. Resto de conjuntos (i≥1): se usa el overload **sin** análisis `ctrl.analiza(col)`; si no pasa `fallosPermitidos` ⇒ inválida; si pasa: `ColumnasPorFallosDeConjuntos[nFallados]++` y `ColumnasPorFallosDeGrupos[nFallados]++` (`:263-264`).
- `RecalcularControladorGrupos()` (`:270-278`): flag propio + `RecalcularControlGrupos()` de cada control.
- Setter `ControlesConjuntos` (`:298-309`): asigna y, si `Count>1`, `usaControlConjuntos=true` (nunca lo pone a false). `UsaControlConjuntos` tiene setter público (`:311-315`).

**Estado global.** Ninguno. **Peculiaridades.** (1) Los `ControlGrupos` que no figuran en ningún `ControlConjuntos` **nunca se evalúan** cuando `usaControlConjuntos` es true (`:95-153`). (2) Con ≥2 controles «libres» y análisis, `VaciarInformacion` descarta lo registrado por el primer control (incluido el grupo base) — efecto sobre las estadísticas, no sobre el veredicto (no ejecutado; ver Abiertas A3). (3) `ColumnasPorFallosDeGrupos` mide `GruposPartidos.Count` y `ColumnasPorFallosDeConjuntos` mide `ControlesGrupos.Count` (`AN/ContenedorAnalisisGlobal.cs:30-38`, `MC/Analizador.cs:356`): un índice `nFallados` igual al tamaño lanza `IndexOutOfRange` en C# (todos los grupos de un control fallan y está permitido). En Swift comprobar y lanzar el mismo error, no crashear. (4) El memo se marca `false` antes de evaluar en el overload sin análisis: si una excepción interrumpe, queda memo obsoleta (irrelevante: la ejecución termina).
**Trampas.** T12 (el `Pump` no está aquí). Colecciones por referencia. **Golden.** G-CG-1: 3 controles, 2 conjuntos (`"1,2"` con fallos `"0,1"`), pronóstico con 4 triples y NoVariantes distintos por grupo ⇒ aceptadas A y C. **Evidencia.** `MC/ControladorGrupos.cs:26-318`; `ES/ArchivoCombinacion.cs:109-145`.

---

### 2.7 ControlGrupos — `MC/ControlGrupos.cs`

**Propósito.** Agrupa grupos y, opcionalmente, aplica un presupuesto de «fallos permitidos» entre ellos.

**Tipos y datos.** `bool usaControlGrupos`, `bool[] fallosPermitidos` (`[Bool]?`), `int[] gruposControlados` (`[Int]`, índices en `GrupoPartidos`), `strFallosPermitidos`, `strGruposControlados` (`""`), `noFallosEnAnalisis`, `noGruposControladosTemp`, `ControladorGrupos ctrlGrupos` (weak), `reCalcularControlGrupo=true`, `controlGrupoValidoMemoria=true` (`:27-41`).

**Algoritmo.** Evaluación en §1.2/§1.3 (cuatro variantes `Con/Sin`×`long/contenedor`, `:43-161`). `ControlaFallo(ok)`: `noFallos += ok?0:1; noGruposControladosTemp += 1` (`:164-172`). Análisis con fallos: marca `GruposPartidos[0].EsGrupoBase = true`, evalúa **todos** los grupos con `Grupo.analiza(col)` (sin contenedor), y si `!fallosPermitidos[nFallos]` ⇒ inválida + `contenedor.VaciarInformacion()`, si no `ColumnasPorFallosDeGrupos[nFallos]++` (`:105-143`). Análisis sin fallos: `BBase.EsGrupoBase=true` en cada llamada y `Grupo.analiza(col, contenedor.AnalisisGrupos)` con corte (`:145-161`).

Configuración:
- `PonerGruposControlados(str)` (`:179-197`): `strGruposControlados=str`; `parsed = ObtenerValoresSeparadosPorComas(str)` (**fuera** del `try`; expande `a-b`; puede lanzar); `gruposControlados = new int[parsed.Split(',').count]` (asignado antes del `try`); `Convert.ToInt32` de cada parte dentro del `try`; ante error `UserDialogs.ShowError("Hay errores en los valores introducidos")` y `return` dejando el array **parcial** (ceros).
- `PonerFallosPermitidos(str)` (`:199-232`): `strFallosPermitidos=str`; si `str != ""`: `fallosPermitidos = new bool[gruposControlados.Length+1]`, para cada valor expandido `fallosPermitidos[Convert.ToInt32(v)] = true` dentro de `try` (un valor negativo o > Length lanza ⇒ mensaje de error y `return` **sin** poner `usaControlGrupos=true`, dejando el array parcial); si todo bien `usaControlGrupos=true`. Si `str==""`: `usaControlGrupos=false` (`fallosPermitidos` anterior queda).
- `PonerGrupo(n)` (`:262-281`): `n==0` ⇒ `strGruposControlados="0"`; si no, añade `","+n`; llama `PonerGruposControlados(str)`.
- `RecalcularControlGrupos()` (`:283-291`): flag + `GruposPartidos[idx].RecalcularGrupo()` de cada grupo controlado. `ContieneGrupo(n)` (`:234-249`).

**Estado global.** `UserDialogs.ShowError` (hook). **Peculiaridades.** Ver arriba (estado parcial ante error). `fallosPermitidos[noFallos]` sin comprobar rango en `:78,:124` (el array mide `gruposControlados.Length+1`, salvo que la UI asigne `FallosPermitidos` con otro tamaño por el setter `:311-314`). El mensaje de error está en español fijo.
**Trampas.** `Convert.ToInt32` (espacios/signo/vacío), `Split` con vacíos (§2.1). `bool[]` ⇒ `[Bool]` con acceso protegido para replicar la excepción. **Golden.** G-CtlG-1: `PonerGruposControlados("0,2-4")` ⇒ `[0,2,3,4]`; `PonerFallosPermitidos("5")` con 4 grupos ⇒ error, `usaControlGrupos` sin cambios; `""` ⇒ false. G-CtlG-2: control `[1,2,3]` fallos `"0,1"`: tabla de verdad según nº de grupos que fallan (0,1,2,3 ⇒ T,T,F,F). **Evidencia.** `MC/ControlGrupos.cs:25-322`.

---

### 2.8 ControlConjuntos — `MC/ControlConjuntos.cs`

**Propósito.** Conjunto de `ControlGrupos` con presupuesto de fallos entre controles (el conjunto 0 son los «libres»: deben pasar todos).

**Tipos y datos.** `bool[] fallosPermitidos` (`[Bool]?`), `int[] ctrlGruposControlados` (`[Int]`, índices en `controlesGrupos`), `strFallosPermitidos`, `strCtrlGruposControlados` (`""`) (`:28-32`).

**Algoritmo.** `PonerCtrlGruposControlados(str)` (`:41-48`): `RangosHelper.ObtenIntArray(str)` ⇒ `Split(',')`, cada trozo con `-` y exactamente 2 partes ⇒ rango ascendente inclusivo (descendente ⇒ vacío), si no `Convert.ToInt32(parte[0])`; sin deduplicar (`UT/RangosHelper.cs:32-74`). `PonerFallosPermitidos(str)` (`:34-39`): `ObtenerBoolArrayFromTxt(str, ctrlGruposControlados.Length)`: `""` ⇒ `nil`; si no `ObtenerListaFromTxtAciertos` (acepta `a,b`, `a-b`, `a,b-c`; **ante cualquier error devuelve lista vacía**) ⇒ ordenar ⇒ `Length = max(longitud, mayor)+1` (lanza si la lista queda vacía: `lista[lista.Count-1]`) (`UT/UtilidadesEntradasValores.cs:115-181,255-281`). `ContieneConjunto(n)`.

**Estado global.** Ninguno. **Peculiaridades.** `PonerFallosPermitidos("")` ⇒ `nil` ⇒ NRE al evaluar conjuntos i≥1 (`MC/ControladorGrupos.cs:144`). **Trampas.** `Split` con vacíos, `Convert.ToInt32`, `List.Contains` O(n) (irrelevante). **Golden.** `"0"`⇒`[0]`, fallos `"0"`⇒`[T,F]`; `"1,3-5"`⇒`[1,3,4,5]`; fallos `"1"` con 2 controles ⇒ `[F,T,F]`; `"a"` ⇒ lista vacía ⇒ excepción. **Evidencia.** `MC/ControlConjuntos.cs:26-85`.

---

### 2.9 ToleranciaFiltros — `MC/ToleranciaFiltros.cs`

**Propósito.** Una «tolerancia»: letras (A,B,C…) que marcan valores tolerados en los filtros, y un rango de aciertos permitidos sobre la suma de coincidencias de todos los filtros activos del grupo.

**Tipos y datos.** `string letrasTol` (setter `value.ToUpper()`, cultura actual), `string strAciertos`, `bool[] aciertosPermitidos`, `int noAciertosAcumulados` (`:26-31,148-162`) ⇒ `class ToleranciaFiltros` con `String`, `String`, `[Bool]`, `Int`.

**Algoritmo.**

```
set Aciertos(s): strAciertos = s; inicializaArrayAciertos()
inicializaArrayAciertos():                                                  :61-114
   sep = primer carácter de s que sea ',' o '-' (EncuentraSeparador :116-140; si no hay ⇒ "")
   ',' : partes = s.Split(','); max = Int(partes.last) + 2;  a = bool[max]; para p: a[Int(p)] = true       // "último = mayor" implícito
   '-' : partes = s.Split('-'); lo=Int(p[0]); hi=Int(p[1]); a = bool[hi+2]; para i en 0..<(hi-lo+1): a[lo+i]=true
   si no: n = Int(s); a = bool[n+2]; a[n] = true
cumpleTolerancia(): n = noAciertosAcumulados; si a.count-1 < n ⇒ false; si no ⇒ a[n]         :43-59
```

**Estado global.** Ninguno. **Peculiaridades [PRESERVAR].** El array mide `máximo+2`, así que el valor `máximo+1` y mayores siempre fallan. Lista `"5,2"` (no creciente) lanza `IndexOutOfRange`; `"1-3,5"` ⇒ separador `-` ⇒ `"3,5"` no numérico ⇒ `FormatException`; `"-1"` ⇒ `Split` da `["","1"]` ⇒ `FormatException`; rango `lo>hi` ⇒ todo `false`. `aciertosPermitidos==nil` si nunca se asignó `Aciertos` (NRE al evaluar). Se crea siempre con `Aciertos` antes de `LetrasTol` (`ES/ArchivoCombinacion.cs:276-277`).
**Trampas.** `Convert.ToInt32` (espacios/`+`), `Split` con vacíos, `ToUpper` con cultura (usar `uppercased()` sin locale), clase por referencia. **Golden.** `"1,3,5"` ⇒ `[F,T,F,T,F,T,F]`; `"2-4"` ⇒ `[F,F,T,T,T,F]`; `"3"` ⇒ `[F,F,F,T,F]`; `cumple(4)` para `"3"` ⇒ false (índice = `a.count-1`); errores `"5,2"`, `"1-3,5"`, `""`. **Evidencia.** `MC/ToleranciaFiltros.cs:24-164`.

---

### 2.10 ControladorTol — `MC/ControladorTol.cs`

**Propósito.** Lista de `ToleranciaFiltros` de un grupo y presupuesto de tolerancias falladas permitidas.

**Tipos y datos.** `List<ToleranciaFiltros> tolerancias` (`[ToleranciaFiltros]`, inicial vacía), `string strFallosPermitidos=""`, `bool[] fallosPermitidos` (`[Bool]?`) (`:27-30`).

**Algoritmo.**

```
set FallosPermitidos(s): str = s; si s != "": inicializarFallos()                 :127-139   (con "" NO borra el array anterior)
inicializarFallos():                                                              :43-81
   ',' : a = bool[Int(last)+2]; para p en partes: a[Int(p)] = true
   '-' : lo,hi = partes; a = bool[hi+2]; para i en 0..<(hi-lo+1): a[lo+i] = true
   si no: n = Int(s); a = bool[n+2]; a[n] = true
sonFallosPermitidosValidos(n): si a.count <= n ⇒ false; si no a[n]               :109-125
```

**Peculiaridades.** Mismas que `ToleranciaFiltros` (array `máx+2`, errores por orden/formato). `Grupo` solo consulta `FallosPermitidos` (string) `== ""` para elegir la rama (`MC/Grupo.cs:194`), y `PonerTolerancia` no se usa en el cargador (asigna la lista entera, `ES/ArchivoCombinacion.cs:287`). **Trampas.** Igual que 2.9. **Golden.** `"0"`⇒`[T,F]` (válido(0)=T, (1)=F, (2)=F); `"0-2"`⇒`[T,T,T,F]`; `"1,3"`⇒`[F,T,F,T,F]`. **Evidencia.** `MC/ControladorTol.cs:25-146`.

---

### 2.11 RangosOpciones — `MC/RangosOpciones.cs`

**Propósito.** Conjunto de intervalos `[min,max]` de `Double` sobre los que se comprueba un valor (usado por `FiltroValoracionSignos`: 4 instancias, `MC/FiltroValoracionSignos.cs:52-66,162,173-179,237-253`).

**Tipos y datos.** `List<Rango>` con `Rango{min:Double,max:Double}` (clase interna `protected`) ⇒ `struct Rango`, `[Rango]`.

**Algoritmo.** `PonerRangos(s)` (`:30-56`): `s.Split('#')`; cada trozo `Split('-')`: 1 parte ⇒ `min=max=ToDouble(p0)`; si no `min=ToDouble(p0)`, `max=ToDouble(p1)` (partes extra se ignoran); **añade** (no limpia). `ValorEnRangoValido(v)` (`:58-74`): `true` si algún rango cumple `v <= max && v >= min`. `ReinicializarRangos()` limpia; `Count`.

**Peculiaridades [PRESERVAR].** `Convert.ToDouble` usa la **cultura actual** (con `es-ES`, `"1.5"` ⇒ 15 y `"1,5"` ⇒ 1,5); negativos (`"-3"`) no soportados: `Split('-')` produce `["","3"]` y `ToDouble("")` lanza. Comparaciones `Double` con `NaN` ⇒ false. Los llamadores ignoran `""` (`if (valores != "")`, `MC/FiltroValoracionSignos.cs:238`), por lo que una instancia vacía rechaza todo valor (`ValorEnRangoValido` ⇒ false).
**Trampas.** T7 (fijar locale; decidir con el dueño si se respeta `es-ES` o `invariant`, R2), `Double` (no `Float`) aquí. **Golden.** `"0-5#10#20-30"` con 0, 5, 7, 10, 25 ⇒ T,T,F,T,T. **Evidencia.** `MC/RangosOpciones.cs:26-105`.

---

### 2.12 FiguraCondicion — `MC/FiguraCondicion.cs`

DTO: `long figura` ⇒ `Int64`, `int apariciones` ⇒ `Int` (`:20-36`). Lo producen `FiltroContactos.ObtenerFiguraLong*` (`MC/FiltroContactos.cs:951-953`) y `FiltroPesosNumericos` (`:13,180`) y lo ordena `AN/FigurasComparer.cs:24-27`. Sin lógica. Golden: n/a (cubierto por los filtros).

### 2.13 AgrupacionColumnas — `MC/AgrupacionColumnas.cs`

DTO inmutable: `List<int> numero`, `int noElementos`, `List<int> noAciertos` ⇒ `struct AgrupacionColumnas { numero:[Int]; noElementos:Int; noAciertos:[Int] }` (`:6-30`); constructor `(num, elementos, aciertos)`. Lo usa `RelacionCP3` (`MC/RelacionCP3.cs:31-32,92,117`; ESPEC_04). Sin lógica.

### 2.14 EscaleraAciertos — `MC/EscaleraAciertos.cs`

`List<int> escalones` (get/set, nace `null`), `NumeroEscalones = Escalones.Count`, `Orientacion` calculada (`:20-34`): `Count>0` ⇒ `Descendente` si `primero > último`, si no `Ascendente` (igualdad ⇒ ascendente); `Count==0` ⇒ devuelve el campo `orientacion` que **nunca se asigna** = `(OrientacionEscalera)0`, valor **sin miembro** en el enum (`Ascendente=1`, `Descendente=2`, `DOM/Enums.cs:105-109`). `RelacionCP3` solo crea escaleras con ≥3 escalones (`MC/RelacionCP3.cs:411-426`), así que la rama vacía no se ejecuta. Swift: `struct EscaleraAciertos { var escalones:[Int] }` con `orientacion` ⇒ `precondition(!escalones.isEmpty)` (o `Optional`). Golden: `[1,2,3]`⇒asc, `[3,2,1]`⇒desc, `[2,2,2]`⇒asc. Evidencia: `MC/EscaleraAciertos.cs:6-35`.

### 2.15 SandwichAciertos — `MC/SandwichAciertos.cs`

DTO: `List<int> sandwich` (get/set, `null` inicial) ⇒ `struct SandwichAciertos { var sandwich:[Int] }` (`:6-15`). Lo construye `RelacionCP3.ObtenerSandwichs` (`MC/RelacionCP3.cs:431-…`). Sin lógica.

---

### 2.16 CondicionIfThen — `MC/CondicionIfThen.cs`

**Propósito.** Una condición «Si A entonces B» entre dos filtros descritos como **texto** (p. ej. `"(NO) Cantidad de Variantes: 3"`), evaluada sobre la columna entera (sin máscara de grupo).

**Tipos y datos.** `IFiltro filtroIf/filtroThen` (caché, `(any FiltroProtocol)?`), `int numPartidosFiltroIf/Then=-1`, `string condIf/condThen`, `bool noIf/noThen` (`:26-35`).

**Algoritmo.**

```
set CondIf(s):  condIf = s; noIf = s contiene "(NO)"; filtroIf = nil; numPartidosFiltroIf = -1      :57-68  (idem Then :70-81)
set FiltroIf(f): filtroIf = f; numPartidosFiltroIf = N actual                                         :37-45
obtenFiltroIf(): si filtroIf == nil o numPartidosFiltroIf != N: filtroIf = getFiltro(condIf); numPartidos... = N; return filtroIf    :263-271
compruebaPronostico(col):                                                                             :284-298
   v = obtenFiltroIf().analizar(col); si noIf: v = !v
   si !v: return true                       // «If» no se cumple ⇒ la condición se cumple
   w = obtenFiltroThen().analizar(col); si noThen: w = !w
   return w
```

`getFiltro(texto)` (`:95-250`) — ramas **en este orden** (la primera que contenga la subcadena, `IndexOf` sensible a mayúsculas y a la cultura); `posicion = LastIndexOf(" ")`, `valor = Substring(posicion).Trim()` (última palabra), `nombreCond = Substring(0, posicion-1)` (quita el `:` final):

| Contiene | Filtro creado | Configuración |
|----------|---------------|---------------|
| `Cantidad` | `FiltroNoVariantes` | si `Variantes`: `SetNoVariantes(Int16(valor), true)`; si no `X`: `SetNoEquis(…)`; si no `2`: `SetNoDoses(…)`; `LlenarTodosValores()` (la comprobación usa `texto` completo, valor incluido) |
| `Dibujo` | `FiltroDibujos` | `Dibujos.Add(valor)` (valor = última palabra, p. ej. `"3+4"`); sin `LlenarTodosValores` |
| `Signos Seguidos` | `FiltroSignosSeguidos` | `nombreCond` contiene (en orden) `Variantes`→`SetNoVariantes(valor)`, `1`→`SetNoUnos`, `X`→`SetNoEquis`, `2`→`SetNoDoses`; `LlenarTodosValores()` |
| `Interrupciones` | `FiltroInterrupciones` | si `Seguidas`: `Globales`→`SetNoIntGlobalSeg`, `Variantes`→`SetNoIntVarSeg`, `1`→`SetNoInt1Seg`, `X`→`SetNoIntXSeg`, `2`→`SetNoInt2Seg`; si no: `SetNoIntGlobales`, `SetNoIntVar`, `SetNoInt1`, `SetNoIntX`, `SetNoInt2`; `LlenarTodosValores()` |
| `Peso numérico` (n minúscula, é) | `FiltroPesosNumericos` | `Global`→`SetPNGlobal`, `Variantes`→`SetPNVar`, `1`→`SetPNUnos`, `X`→`SetPNEquis`, `2`→`SetPNDoses`; `LlenarTodosValores()` |
| `Distancia` | `FiltroDistancias` | `Variantes`→`SetdistVar`, `1`→`SetdistUnos`, `X`→`SetdistEquis`, `2`→`SetdistDoses`; `LlenarTodosValores()` |
| `Contactos` | `FiltroContactos` | en orden: `1X`→`SetNum1X`, `12`→`SetNum12`, `X2`→`SetNumX2`, `11`→`SetNum11`, `XX`→`SetNumXX`, `22`→`SetNum22`, `1V`→`SetNum1V`, `XV`→`SetNumXV`, `2V`→`SetNum2V`, `VV`→`SetNumVV`; `LlenarTodosValores()` |

Al final `f.IsActive=true; f.ContieneDatos=true` (`:247-248`). Textos reales que genera la UI: `"[(NO) ]<específica>: <int>"` (`<específica>` p. ej. `Cantidad de Variantes`, `Interrupciones Seguidas de X`, `Peso numérico de 1`, `Contactos de XV`; `Dibujo i+j` sin `:`) — `WIN/IfThenFrmViewModel.cs:262-318,359-390`; 3 + 120 (N=14) + 4 + 10 + 5 + 4 + 10 textos.

**Estado global.** `VariablesGlobales.NumeroPartidos` (`:43,53,265,268,276,279`).

**Peculiaridades [PRESERVAR].**
1. Texto no reconocido ⇒ `f` queda `nil` ⇒ NRE en `:247`; la construcción es **perezosa** (en la primera columna) para no adelantar excepciones (comentario P-01, `:252-262`).
2. Un filtro cacheado se reutiliza para todas las columnas; es válido porque los siete filtros construidos reinician sus contadores en cada `Analizar` (`InicializaContadores` en `MC/FiltroNoVariantes.cs:157`, `FiltroDibujos.cs:63`, `FiltroSignosSeguidos.cs:211`, `FiltroInterrupciones.cs:417`, `FiltroPesosNumericos.cs:449`, `FiltroDistancias.cs:188`, `FiltroContactos.cs:309`). La caché se invalida si cambia el texto o `NumeroPartidos`.
3. Asignar `NoIf` y después `CondIf` pierde el `NoIf` (el setter de texto lo recalcula). `"(NO)"` se detecta en **cualquier** posición.
4. `Convert.ToInt16(valor)` lanza ante no numérico/fuera de rango. `texto.IndexOf("Peso numérico")` no casa con `"Peso Numérico"`, que es lo que emite `ControladorIfThen.getLineaTexto` (código sin llamadas, §2.17).
5. El filtro opera sobre la columna **completa** (ningún grupo, ninguna máscara).

**Trampas Swift.** `IndexOf(string)` es sensible a cultura (T11): usar comparación literal tras normalizar a NFC (`"é"` precompuesta en la UI). `Substring(posicion)` con `posicion=-1` lanza. Los setters `Set*(String)` pertenecen a ESPEC_02/03: se llaman con la **cadena de valor** (`"3"`), no con enteros.
**Casos golden.** Para cada uno de los textos de la UI (≈156): construir el filtro y volcar `analizar` sobre 200 columnas deterministas; textos erróneos (`"Foo: 3"`, `"Peso Numérico de 1: 3"`) ⇒ error; `compruebaPronostico` con `noIf/noThen` en las 4 combinaciones. **Evidencia.** `MC/CondicionIfThen.cs:24-300`.

---

### 2.17 ControladorIfThen — `MC/ControladorIfThen.cs`

**Propósito.** Conjunto de condiciones y de grupos «Si-Entonces» con un rango de cuántas deben cumplirse.

**Tipos y datos.** `ArrayList controlesCondiciones/controlesGrupos` ⇒ `[CondicionIfThen]`, `[GrupoIfThen]`; `string rangoAciertoCondiciones/Grupos`; `bool[] aciertoCondiciones/aciertoGrupos`; `bool activo=true` (`:29-37`).

**Algoritmo.**

```
set RangoAciertoCondiciones(s): r = s; tmp = RangosHelper.obtenIntArray(s)
      aciertoCondiciones = bool[controlesCondiciones.count + 1]; para v en tmp: aciertoCondiciones[v] = true    :57-72  (v > count ⇒ IndexOutOfRange)
   (idéntico para RangoAciertoGrupos con controlesGrupos :74-89)
esVacio = controlesCondiciones.count == 0 && controlesGrupos.count == 0                                  :107-115

compruebaPronostico(col, grupos):                                                                         :277-298
   fallos = 0; para c en condiciones (TODAS, sin corte): si !c.compruebaPronostico(col): fallos += 1
   si condiciones.count > 0 y !aciertoCondiciones[condiciones.count - fallos]: return false
   fallos = 0; para g en gruposIfThen (TODOS): si !g.compruebaPronostico(col, grupos): fallos += 1
   si grupos.count > 0: return aciertoGrupos[grupos.count - fallos]
   return true
```

`compruebaErrores(col)` (`:300-325`) y `(col, grupos)` (`:327-352`): para cada condición/grupo fallado añaden `"!Fallo en condición relacionada N"` / `"!Fallo en grupo relacionado N"` (N 1-based); si hubo fallos añaden `"Rango de aciertos de condiciones fallado"` o `"*Rango de aciertos de condiciones acertado"` (idem `grupos`) según `aciertoX[count - nFallos]`; sin fallos devuelven array vacío. `AnalizaColumna(string)` devuelve `true` (`:117-120`). `AddGrupos/AddCondiciones/QuitaGrupos(i)/QuitaCondiciones(i)/Limpia*`. `getLineaTexto(IFiltro)` (`:158-263`) y `getLineaTexto(Grupo,Analizador)` (`:265-275`): **sin llamadas en todo el repo** (grep) — no portar salvo que lo pida la UI; `getLineaTexto(Grupo,…)` recorre `1…Count` **inclusive** y lanza si no encuentra el nombre.

**Estado global.** Ninguno. **Peculiaridades [PRESERVAR].** (1) `aciertoCondiciones` se dimensiona con el `Count` **del momento de asignar el rango**; añadir condiciones después deja el array corto ⇒ `IndexOutOfRange` al evaluar. (2) `aciertoX==nil` si nunca se fijó el rango y hay elementos ⇒ NRE. (3) `AciertoCondiciones[…]==false ⇒ return false` corta antes de mirar los grupos. (4) La invocación desde `Analizador` y sus consecuencias (asignar vs AND, memo de grupos) en §1.2-1.4.
**Trampas.** `ArrayList` ⇒ arrays tipados; índices calculados `count - fallos` ∈ [0,count] siempre, el riesgo es el tamaño del `bool[]`. **Golden.** 3 condiciones con rango `"2-3"` ⇒ `[F,F,T,T]` y tabla (fallos 0,1,2,3 ⇒ T,T,F,F); rango `"0-4"` con 3 ⇒ excepción; `compruebaErrores` con 2 fallos de 3. **Evidencia.** `MC/ControladorIfThen.cs:27-352`; `ES/ArchivoCondiciones.cs:85-145`.

---

### 2.18 GrupoIfThen — `MC/GrupoIfThen.cs`

**Propósito.** «Si el grupo A se cumple entonces el grupo B» (por número de grupo).

**Tipos y datos.** `Grupo grupoIf/grupoThen` (**no usados** en la evaluación), `int numGrupoIf/numGrupoThen` ⇒ `Int`, `bool noIf/noThen` (`:24-29`). El cargador lee los números con `Convert.ToInt16` y los `no-if/no-then` con `Convert.ToBoolean` (`ES/ArchivoCondiciones.cs:137-140`).

**Algoritmo** (`:67-81`): `v = grupos[numGrupoIf].analiza(col)` (overload **sin** análisis, con su memo); `si noIf: v=!v`; `si !v: return true`; `w = grupos[numGrupoThen].analiza(col)`; `si noThen: w=!w`; `return w`.

**Peculiaridades.** El veredicto del grupo viene del memo de la columna en curso (§1.4). El grupo evaluado aquí **también** aplica su filtro parcial y tolerancias (es el `Grupo.analiza(long)` completo). Índices fuera de rango lanzan. **Trampas.** Ninguna específica. **Golden.** If=grupo 1, Then=grupo 2 con y sin `noIf/noThen`. **Evidencia.** `MC/GrupoIfThen.cs:22-82`.

---

### 2.19 Enums — `DOM/Enums.cs`

| Enum C# | Miembros (valor) | Uso verificado | Swift |
|---------|------------------|----------------|-------|
| `Signo` (`:22-28`) | NoSigno 0, Uno 1, Equis 2, Dos 3 | sin usos en el repo (grep) | `enum Signo: Int` (opcional) |
| `ReduccionTipo` (`:30-36`) | Categoria13 0 … Categoria10 3 | sin usos | opcional |
| `AlgoritmoReduccion` (`:38-43`) | XFSF 0, JLPM 1, ToniMoreno 2 | sin usos | opcional |
| `AlgebraCombTipo` (`:45-51`) | EliminaRepetidas 0, SumaEliminaRepetidas 1, SumaSoloComunes 2, RestaSegunda 3 | `UT/SumadorCombinaciones.cs:50-64,283` | `enum AlgebraCombTipo: Int` |
| `AccionControlModificador` (`:53-57`) | Modificar 0, Escribir 1 | solo WinForms (`Free1X2/UI/ModificadorFrm.cs`) | UI Mac |
| `OrdenarMatriz` (`:59-67`) | Signo 0, Variantes 1, Equis 2, Doses 3, SignosSeguidos 4, Interrupciones 5 | `WIN/BoletoFrmViewModel.cs:37,235-240` | `enum: Int` |
| `TipoOrden` (`:69-73`) | asc 0, desc 1 | `WIN/BoletoFrmViewModel.cs:38` | `enum: Int` |
| `TipoValidacionString` (`:74-84`) | SoloNumeros 0 … Solo1X2 6 | solo WinForms (`Free1X2/Utils/ValidadorCaracteres.cs`) | UI Mac |
| `alignment` (`:86-90`) | Vertical 0, Horizontal 1 | solo WinForms | UI Mac |
| `vistaAnalisis` (`:92-96`) | Columnas 0, Porcentaje 1 | solo WinForms (`CtrPorcentaje.cs`) | UI Mac |
| `ModoEmparejamientoColumnasABDON` (`:98-103`) | A 1, B 2, C 3 | `WIN/EstucolFrmViewModel.cs:62,139…` | `enum: Int` |
| `OrientacionEscalera` (`:105-109`) | Ascendente 1, Descendente 2 | `MC/EscaleraAciertos.cs`, `MC/RelacionCP3.cs:528` | `enum: Int` |

Conservar los **valores enteros explícitos** (por si se persisten o comparan). No hay enums `[Flags]`. `Signo`/`ReduccionTipo`/`AlgoritmoReduccion` no se referencian: portarlos solo si algún ESPEC posterior los necesita.

### 2.20 Filtro (enum) — `DOM/EnumFiltros.cs`

`NoVariantes 0, SignosSeguidos 1, Dibujos 2, ColProbables 3, PesosNumericos 4, ValoracionSignos 5, NoInterrupciones 6, Distancias 7, GruposEquipos 8, Contactos 9, FormatosSignos 10, Formatos123 11, Simetrias 12, Diferencias 13` (`:22-38`). Swift: `enum Filtro: Int, CaseIterable` con una propiedad `id: String` ≡ nombre del caso (se usa `ToString()` como **id** en búsquedas, XML y `ActivaFiltro`): el miembro de `FiltroInterrupciones` es **`NoInterrupciones`** (`MC/FiltroInterrupciones.cs:433,1029`), no `Interrupciones`. Su orden **no** es el de `Grupo.filtros` (§2.4).

---

### 2.21 IAppPaths / AppPaths — `DOM/Abstractions/IAppPaths.cs`

`protocol` con `StartupPath:String` (equivalente a `Application.StartupPath`); `AppPaths` por defecto = `AppContext.BaseDirectory` (`:8-23`). **Sin usos** en Domain/WinUI/Tests (grep): la ruta real de `parametros.free1x2` se resuelve en `VariablesGlobales.GetConfigPath` (`DOM/VariablesGlobales.cs:119-147`: prueba 5 carpetas, cae a `BaseDirectory`), `MC/Analizador.cs:426`, `MC/Grupo.cs:61` y `ES/AConfiguracion.cs:33-55` (`dir + "/parametros.free1x2"`). Swift: `protocol RutasApp { var rutaInicio: URL }`, con el fichero en Application Support (decisión de ESPEC_07). Golden: n/a.

### 2.22 IProgressNotifier / NullProgressNotifier — `DOM/Abstractions/IProgressNotifier.cs`

`AllowUIToProcess()` y `Report(percent:Int, message:String?)` (`:9-15`); `NullProgressNotifier.Instance` no hace nada (`:19-23`). **Sin usos** (grep). Swift: no portar o `protocol ProgresoMotor` con `report` y reemplazar por `AsyncStream<Progreso>` (T12).

### 2.23 UiHooks — `DOM/Abstractions/UiHooks.cs`

Cuatro shims estáticos mutables, por defecto no-op (`:12-47`):

| Shim | Firma | Llamado desde (núcleo) | Swift |
|------|-------|------------------------|-------|
| `UiPump.Pump` (`:14`) | `Action` | `MC/Analizador.cs:133,300`; reductores (`DOM/Reduccion/*.cs`), `DOM/Escrutinio/Escrutador.cs:133` | `func bombear()` ⇒ mejor: progreso asíncrono + cancelación cooperativa |
| `UserDialogs.ShowError/ShowInfo` (`:24-25`) | `Action<string>` | `MC/ControlGrupos.cs:194,222` (`"Hay errores en los valores introducidos"`), `UT/Porcentajes.cs`, `DOM/Reduccion/ReductorTM.cs:354`, `AN/Analizador.cs:90` | `func mostrarError(_:)`, `mostrarInfo(_:)` |
| `AnalisisUi.MostrarVisor` (`:35`) | `Action<object,object>` (contenedor, grupo 0) | `MC/Analizador.cs:301,346` | `func mostrarVisorAnalisis(_ c: ContenedorAnalisisGlobal, grupoBase: Grupo)` |
| `Clipboard.Read/Write` (`:46-47`) | `Func<string>` / `Action<string>` | `UT/Porcentajes.cs:541,545` | `func leerPortapapeles() -> String`, `escribirPortapapeles(_:)` |

En WinUI `Pump` es no-op deliberado (`docs/MANUAL_FLUJOS.md` §11; `Free1X2.WinUI/App.xaml.cs:131`) y `MostrarVisor` encola la navegación al hilo de UI (`Free1X2.WinUI/App.xaml.cs:170-204`). Swift (Swift 6): **no** usar globales mutables; inyectar `protocol AnfitrionUI: Sendable` en `Sesion` con implementación por defecto vacía. Caracterización existente: `TST/AbstractionsTests.cs:20-85` (por defecto no lanzan; reasignables).

### 2.24 VariablesGlobales — `DOM/VariablesGlobales.cs`

Detalle completo en §3 (tabla de 44 miembros) y propuesta de structs (§3.3). Comportamiento a preservar: (a) estático e inmutable salvo el constructor estático; (b) `ReinicializarVariables()` es **no-op** una vez inicializado (§3.2).

---

## 3. `VariablesGlobales` — estado global

### 3.1 Tabla de los 44 miembros (42 campos + 2 calculados)

Escritor único: `InicializarVariables()` (`DOM/VariablesGlobales.cs:76-117`) vía `AConfiguracion.Obten*` por `ref`, o `SetDefaultValues()` (`:149-175`) si algo lanza. **No hay ningún setter público** ni asignaciones externas (grep). `Inicial` = valor del campo antes de leer el XML; `Default` = tras `SetDefaultValues`; `Shipped` = `Free1X2.WinUI/Datos/parametros.free1x2`.

**a) Boleto, CP y LAE**

| Miembro (campo / propiedad) | Tipo C# → Swift | Inicial | Default | Shipped | Escribe (fichero:línea) | Lee (fichero:línea) |
|------|------|------|------|------|------|------|
| `numPartidos` / `NumeroPartidos` (`:14,199`) | `int` → `Int` | 0 | 14 (`:152`) | 14 (`param:17`) | `:94` ← `ES/AConfiguracion.cs:145-151` (`Convert.ToInt16`) | ~280 usos. Núcleo: `MC/Analizador.cs:39,356`; `MC/GeneradorColumnas.cs:38,281-312`; `MC/Grupo.cs:38`; `MC/CondicionIfThen.cs:43,53,265-279`; `UT/UtilColumnas.cs:172`; `UT/UtilidadesEntradasValores.cs:290,325,407,410`; los 14 filtros al construirse (p. ej. `MC/FiltroNoVariantes.cs:50`, `FiltroValoracionSignos.cs:59-61,227`, `FiltroDistancias.cs:63,601`, `FiltroInterrupciones.cs:77,1040`, `FiltroContactos.cs:77,926`, `FiltroPesosNumericos.cs:55`, `FiltroSignosSeguidos.cs:41,623`, `FiltroFormatos123.cs:30,271,334`); `MC/ColumnaProbable.cs:27,70,276,326,697-704`; `MC/GrupoEquipos.cs:28`; `DOM/Escrutinio/*`, `DOM/Reduccion/Redu1305Xfsf.cs`, `UT/Porcentajes.cs`; WinUI (p. ej. `Free1X2.WinUI/Views/MainPageViewModel.cs:335,371`, `Free1X2.WinUI/Controls/BoletoBaseViewModel.cs:177,187`) |
| `puntosFijos/Dobles/Triples` / `Puntos*` (`:15-17,204-217`) | `int` ×3 → `Int` | 0 | 10 / 15 / 20 (`:153-155`) | 6 / 3 / 1 (`param:5`) | `:90` ← `AConfiguracion.cs:58-66` (`Convert.ToInt32`) | **ninguno** vía `VariablesGlobales`. Los CP leen el fichero en cada ejecución: `MC/Analizador.cs:426-434` → `MC/Grupo.cs:147-158` → `MC/FiltroColProbables.cs:150-157` → `MC/ColumnaProbable.cs:742-744` |
| `desplazamiento` / `Desplazamiento` (`:18,219`) | `int` → `Int` | 0 | 0 (`:156`) | 3 (`param:20`) | `:92` ← `AConfiguracion.cs:160-169` (`ToInt16`; **excepciones tragadas**, conserva el valor previo) | `WIN/ColProbablesFrmViewModel.cs:504,522` |
| `separador` / `Separador` (`:19,224`) | `string[]` → `[String]` | `nil` | `["1","X","2"]` (`:157`) | `["5","9","12"]` (`param:17`) | `:94` ← `AConfiguracion.cs:145-151` (`Split(',')`) | `WIN/VerBoletosEnEditorFrmViewModel.cs:99,103` |
| `precioApuesta` / `PrecioApuesta` (`:21,182`) | `double` → `Double` | 0 | 0.75 (`:158`) | `"0,50"` → 0.5 (`param:10`) | `:102` ← `AConfiguracion.cs:402-413`: **`float.Parse(…, es-ES)` ensanchado a `double`** | `WIN/RentabilidadFrmViewModel.cs:165,213`; `CalculaColumnasFrmViewModel.cs:221,372`; `OrdenarPorProbabilidadFrmViewModel.cs:196`; `CalculaColumnasMultipleFrmViewModel.cs:28,321,354` |
| `porcentaje14` / `Porcentaje14` (`:22,186`) | `double` | 0 | 0.55 (`:159`) | 12 (`param:10`) | idem (`float.Parse`) | `WIN/RentabilidadFrmViewModel.cs:166,214`; `OrdenarPorProbabilidadFrmViewModel.cs:197` |
| `recaudacion` / `Recaudacion` (`:23,190`) | `double` | 0 | 1000000 (`:160`) | 14000000 (`param:10`) | idem | `WIN/OrdenarPorProbabilidadFrmViewModel.cs:195` |
| `moneda` / `Moneda` (`:24,194`) | `string` → `String` | `nil` | `"€"` (`:161`) | `"€"` | `:102` ← `AConfiguracion.cs:412` | `WIN/CalculaColumnasFrmViewModel.cs:222,373`; `CalculaColumnasMultipleFrmViewModel.cs:323,355` |

**b) Interfaz y preferencias**

| Miembro | Tipo | Inicial | Default | Shipped | Escribe | Lee |
|------|------|------|------|------|------|------|
| `mostrarTsFree/Filtros/Combinacion/Operaciones/Archivo/Utilidades` / `MostrarTs*` (`:26-31,228-251`) | `bool` ×6 | false | true ×6 (`:163-168`) | True ×6 (`param:23`) | `:96` ← `AConfiguracion.cs:68-87` (lectura robusta: falta/ilegible ⇒ true) | `Free1X2.WinUI/MainWindow.xaml.cs:561-566` |
| `actualizarAlInicio` / `ActualizarAlInicio` (`:54,252`) | `bool` | false | false (`:170`) | False | `:98` ← `AConfiguracion.cs:99-106` (`Convert.ToBoolean`) | ninguno |
| `mostrarConfirmacionSalir` / `PedirConfirmacionAlSalir` (`:56,256`) | `bool` | false | true (`:171`) | True | `:106` ← `AConfiguracion.cs:108-115` | solo WinForms `Free1X2/UI/MainForm.cs:160` |
| `fechaUltimaComprobacionNotificaciones` / … (`:57,362`) | `DateTime` → `Date` | `0001-01-01` | `DateTime.Now` (`:172`) | `"03/02/2010"` | `:108` ← `AConfiguracion.cs:198-208` (`Convert.ToDateTime` **cultura actual**; errores tragados) | ninguno |
| `diccionarioIdioma` / `DiccionarioIdioma` (`:58,346`) | `Dictionary<string,string>` → `[String:String]` | vacío (`:58`) | vacío (`:173`) | — | `:104` ← `AConfiguracion.cs:178-182` → `ArchivoIdioma.ObtenerIdioma` | solo WinForms `Free1X2/UI/FormulariosHelper.cs:34-113` |
| `idioma` / `Idioma` (`:59,342`) | `string` | `""` | `"es-ES"` (`:162`) | `es-ES` (`param:32`) | `:88` ← `AConfiguracion.cs:172-176` | solo WinForms `FormulariosHelper.cs:32` |
| `inicializado` (`:60`) | `bool` | false | true (`:174`) | — | `:110,174` | `:78` (guarda de `InicializarVariables`) |

**c) Flags de análisis** (todos `bool`, inicial `false`, default `false` salvo `analizarTodo=true` `:169`, shipped `True` ×20, `param:29`). Escritos en bloque por `:100` ← `AConfiguracion.cs:117-143` (`Convert.ToBoolean` de los atributos `aXxx`; el último, `aDiferencias`, alimenta `analizarSimetriasII`). Lectores:

| Campo / propiedad (`VG:líneas`) | Atributo XML | Lee (`AnalisisActivo` del filtro u otro) |
|------|------|------|
| `analizarTodo` `:33,262` | `aTodo` | **ninguno** |
| `analizarVX2` `:34,266` | `aVX2` | `MC/FiltroNoVariantes.cs:490`; UI `WIN/VisorAnalisisColumnasFrmViewModel.cs:187` |
| `analizarSeguidos` `:35,270` | `aSeguidos` | `MC/FiltroSignosSeguidos.cs:713` |
| `analizarDibujos` `:36,274` | `aDibujos` | `MC/FiltroDibujos.cs:182` |
| `analizarInterrupciones` `:37,278` | `aInterrupciones` | `MC/FiltroInterrupciones.cs:1132` |
| `analizarFormatos` `:38,314` | `aFormatos` | `MC/FiltroFormatosSignos.cs:168` |
| `analizarFormatos123` `:39,318` | `aFormatos123` | `MC/FiltroFormatos123.cs:469` |
| `analizarContactos` `:40,290` | `aContactos` | `MC/FiltroContactos.cs:1103` |
| `analizarFigurasContactos` `:41,294` | `aFigurasContactos` | `AN/ContenedorAnalisis.cs:138` |
| `analizarFigurasV1X2` `:42,298` | `aFigurasV1X2` | `AN/ContenedorAnalisis.cs:206` |
| `analizarFigurasPesos` `:43,302` | `aFigurasPesos` | `AN/ContenedorAnalisis.cs:184` |
| `analizarSimetrias` `:44,282` | `aSimetrias` | `MC/FiltroSimetrias.cs:204` |
| `analizarSimetriasII` `:45,286` | **`aDiferencias`** | `MC/FiltroDiferencias.cs:148` |
| `analizarGruposEquipos` `:46,322` | `aGruposEquipos` | `MC/FiltroGruposEquipos.cs:160` |
| `analizarPesos` `:47,326` | `aPesos` | `MC/FiltroPesosNumericos.cs:1135` |
| `analizarValoracion` `:48,310` | `aValoracion` | `MC/FiltroValoracionSignos.cs:393` |
| `analizarDistancias` `:49,330` | `aDistancias` | `MC/FiltroDistancias.cs:659` |
| `analizarCPs` `:50,306` | `aCPs` | `MC/FiltroColProbables.cs:276` |
| `analizarControlGrupos` `:51,334` | `aControlGrupos` | solo UI (`WIN/VisorAnalisisColumnasFrmViewModel.cs:231`) |
| `analizarControlConjuntos` `:52,338` | `aControlConjuntos` | solo UI (`WIN/VisorAnalisisColumnasFrmViewModel.cs:235`) |

**d) Calculados:** `SistemaOperativo` = `Environment.OSVersion.ToString()` (`:351-354`; lee `Free1X2/Debug/ManejadorExcepciones.cs:22`); `FuncionaBajoMono` = `Type.GetType("Mono.Runtime") != null` (`:355-361`; lee solo WinForms `MainForm.cs:151,190`) ⇒ en macOS/Swift constante `false` (no portar o `ProcessInfo`).

### 3.2 Comportamientos a preservar

1. **Carga única y todo-o-nada:** el constructor estático (`:62-74`) llama a `InicializarVariables` (`:76-117`), que lee en este orden: idioma → puntos CP → desplazamiento → partidos/separador → barras → actualizador → análisis → LAE → diccionario → salir → fecha. Si **cualquier** paso lanza (p. ej. falta el nodo `salir`) ⇒ `SetDefaultValues()` **sobrescribe todo** (incluso lo ya leído bien) y `inicializado=true`. `SetDefaultValues` **no** resetea los otros 19 flags `analizar*` (solo fija `analizarTodo=true`, `:169`): si el fallo ocurre tras leerlos (el nodo `analisis` es anterior a `LAE`, `diccionario` y `salir`), conservan el valor del XML; si falla antes o dentro de la lectura de análisis, quedan en `false`.
2. **`ReinicializarVariables()` es no-op** tras la primera inicialización: `InicializarVariables` empieza con `if (inicializado) return;` (`:78`). Las pantallas de configuración llaman a `ReinicializarVariables()` tras guardar (`WIN/ConfiguracionFrmViewModel.cs:199`, `WIN/ConfiguracionAnalisisFrmViewModel.cs:241`, `Free1X2/UI/ConfiguracionFrm.cs:738`), pero **no recargan nada**: nº de partidos, flags de análisis, LAE, etc. solo cambian al reiniciar la app (el aviso lo admite: «Algunos cambios pueden requerir reiniciar», `WIN/ConfiguracionFrmViewModel.cs:201-202`). Excepción: los puntos CP y el número de partidos que lee `Grupo` se releen del fichero (§2.2, §2.4).
3. **`float` en LAE:** `PrecioApuesta/Porcentaje14/Recaudacion` pasan por `float.Parse` con `es-ES` y se ensanchan a `double` (T3, T7): `"0,55"` ⇒ `Double(Float(0.55))` = 0.550000011920929…, **no** 0.55. Los defaults (`SetDefaultValues`) son `double` exactos. Swift: `Double(Float(texto-es-ES))`.
4. `GetConfigPath` (`:119-147`) busca `parametros.free1x2` en 5 carpetas (carpeta del ensamblado, `BaseDirectory`, `CurrentDirectory`, `CurrentDirectory/bin`, carpeta de `argv[0]`) y cae a `BaseDirectory`; pero `Analizador`, `Grupo` y `AConfiguracion()` usan **solo** `BaseDirectory` (`MC/Analizador.cs:426`, `MC/Grupo.cs:61`, `ES/AConfiguracion.cs:33-55`).
5. Convertir con `Convert.ToInt16/ToInt32/ToBoolean/ToDateTime` es estricto (excepción ante formato inválido); `ObtenToolBarsVisibles` y `ObtenDesplazamiento`/`Fecha` son las únicas lecturas tolerantes.

### 3.3 Propuesta de agrupación en Swift

```swift
struct Configuracion: Sendable {              // inmutable; se construye UNA vez al arrancar (réplica del ctor estático)
    var boleto: ConfigBoleto                  // numeroPartidos: Int, separador: [String], desplazamiento: Int
    var puntosCP: PuntosCP                    // fijos, dobles, triples: Int          (ver nota: el motor los relee del fichero)
    var lae: ConfigLAE                        // precioApuesta, porcentaje14, recaudacion: Double (vía Float), moneda: String
    var analisis: OpcionesAnalisis            // 20 Bool (nombres en español, mapeo XML aXxx; `diferencias` ← aDiferencias)
    var interfaz: PreferenciasInterfaz        // barras (6 Bool), actualizarAlInicio, confirmarAlSalir, idioma, fechaUltimaComprobacion
    static let porDefecto: Configuracion      // = SetDefaultValues (con la salvedad de analizar* de §3.2.1)
}
final class Sesion {                          // estado de ejecución inyectado a todo el motor (sustituye a las estáticas)
    let configuracion: Configuracion
    var anfitrion: any AnfitrionUI            // UiPump/UserDialogs/AnalisisUi/Clipboard (§2.23)
    let rutas: any RutasApp                   // §2.21
    // + Analizador compartido, caché de jornada… (otros ESPEC)
}
```

Reglas: los filtros reciben `Sesion`/`Configuracion` en `init` (leen N y `analisis.<flag>` para `analisisActivo`); `diccionarioIdioma` e `idioma` pertenecen a un servicio de localización (ESPEC_07), no a `Configuracion`. Reproducir (a) lectura única y (b) «Reinicializar = no-op» **es una decisión del dueño** (R2): si la app Mac recarga en caliente cambiará el comportamiento respecto al C#.

---

## 4. Contrato `IFiltro` en Swift y encaje de los filtros concretos

### 4.1 Protocolo propuesto

```swift
protocol FiltroProtocol: AnyObject {                       // clases con identidad (ContenedorAnalisis las guarda por referencia)
    var nombreFiltro: Filtro { get }                       // MC/IFiltro.cs:34
    var isActive: Bool { get set }                         // :32
    var contieneDatos: Bool { get set }                    // :35  (Diferencias lo calcula: Count>0)
    var analisisActivo: Bool { get }                       // :39  = flag Analizar* de Configuracion (tabla 4.2)
    var figuras: [Int64] { get set }                       // :38  (Diferencias: getter y setter LANZAN)
    func analizar(_ columna: Int64) -> Bool                // :29
    func analizarFallos(_ columna: Int64) -> [String]?     // :31  nil = sin fallos
    func esNombreFiltro(_ nombre: String) -> Bool          // :33  ≡ nombre == nombreFiltro.id (comparación ordinal exacta)
    func obtenNoAciertosTolerancias(_ letras: String) throws -> Int   // :36  (Diferencias lanza)
    func usaFiguras() -> Bool                              // :37  = figuras != nil && !figuras.isEmpty (Diferencias: false)
}
```

**Reglas del contrato (verificadas en el núcleo):**

1. **`analizar` fija el estado interno de la columna.** Tras `analizar(c)`, los contadores del filtro describen `c`; de ahí leen `obtenNoAciertosTolerancias` (`MC/Grupo.cs:348,391`) y los contenedores de análisis (`AN/ContenedorAnalisis.cs:109-…`, que castean cada filtro a su clase). Los filtros que construye `CondicionIfThen` reinician contadores en cada llamada (§2.16 punto 2).
2. **Entrada ya enmascarada:** `Grupo` pasa `col & mascara` (o `col` si `todosPartidosActivos`); `IfThen` pasa la columna entera; el filtro nunca conoce el grupo.
3. **Cuándo se llama:** ruta A: solo si `isActive`. Rutas C/D: si `analisisActivo` también cuando `!isActive` (resultado ignorado, solo para estadística). `analizarFallos`: solo la pantalla «Analizar combinación» (`WIN/AnalizarCombinacionFrmViewModel.cs:253`); por convención cada cadena del array empieza por `*` (acierto, no es fallo), `!` (fallo pendiente de tolerancias ⇒ estado «aceptada por tolerancias») o cualquier otro carácter (fallo) (`WIN/AnalizarCombinacionFrmViewModel.cs:262-300`); el array se construye troceando por `#` un texto de fallos (`FiltroValoracionSignos`, `MC/FiltroValoracionSignos.cs:169-186`; `FiltroDiferencias`, `MC/FiltroDiferencias.cs:38-82`).
4. **Identidad y orden:** `Grupo.filtros` mantiene los 14 objetos en el orden de §2.4; `ActivaFiltro` puede sustituir un objeto (referencias antiguas obsoletas). `ContenedorAnalisis` deduplica por identidad (`HashSet<IFiltro>`), conserva orden de inserción.
5. **`esNombreFiltro`:** todos los filtros hacen `nombre.Equals(Filtro.X.ToString())` (ordinal, sensible a mayúsculas). Ids: `NoVariantes, SignosSeguidos, Dibujos, ColProbables, PesosNumericos, ValoracionSignos, NoInterrupciones, Distancias, GruposEquipos, Contactos, FormatosSignos, Formatos123, Simetrias, Diferencias`.
6. **`ObtenNoAciertosTolerancias(letras)`:** `letras` ya en mayúsculas; el filtro suma, por cada letra y por cada contador propio (`variantesTol[noVariantesCol]`, `equisTol[...]`, `dosesTol[...]` en `FiltroNoVariantes`, `MC/FiltroNoVariantes.cs:176-198`), 1 si la marca de tolerancia del valor actual es esa letra. El resultado se suma entre filtros activos (`MC/Grupo.cs:340-352`). Semántica concreta por filtro: ESPEC_02/03/04.
7. **Construcción:** `init(sesion:)` lee N; arrays `bool[N+1]` etc. (p. ej. `MC/FiltroNoVariantes.cs:46-70`). Un `Grupo` crea siempre los 14, inactivos por defecto (`isActive` sin inicializador en los 14, p. ej. `MC/FiltroNoVariantes.cs:28`, `MC/FiltroDiferencias.cs:11`).
8. **Hilos:** los filtros son mutables y no thread-safe; un `Analizador` = un hilo de ejecución.

### 4.2 Tabla de encaje (14 filtros)

| Pos. en `Grupo.filtros` (`MC/Grupo.cs`) | Clase | Miembro `Filtro` (`NombreFiltro`) | `AnalisisActivo` ← flag | Lo documenta |
|---|---|---|---|---|
| 0 (`:94`) | `FiltroNoVariantes` (`:171`) | `NoVariantes` | `AnalizarVX2` (`:490`) | ESPEC_03 |
| 1 (`:97`) | `FiltroValoracionSignos` (`:210`) | `ValoracionSignos` | `AnalizarValoracion` (`:393`) | ESPEC_03 |
| 2 (`:100`) | `FiltroSignosSeguidos` (`:308`) | `SignosSeguidos` | `AnalizarSeguidos` (`:713`) | ESPEC_02 |
| 3 (`:103`) | `FiltroInterrupciones` (`:431`) | **`NoInterrupciones`** | `AnalizarInterrupciones` (`:1132`) | ESPEC_02 |
| 4 (`:106`) | `FiltroDibujos` (`:82`) | `Dibujos` | `AnalizarDibujos` (`:182`) | ESPEC_03 |
| 5 (`:109`) | `FiltroPesosNumericos` (`:1019`) | `PesosNumericos` | `AnalizarPesos` (`:1135`) | ESPEC_02 |
| 6 (`:112`) | `FiltroDistancias` (`:202`) | `Distancias` | `AnalizarDistancias` (`:659`) | ESPEC_03 |
| 7 (`:115`) | `FiltroGruposEquipos` (`:113`) | `GruposEquipos` | `AnalizarGruposEquipos` (`:160`) | ESPEC_03 |
| 8 (`:118`) | `FiltroContactos` (`:322`) | `Contactos` | `AnalizarContactos` (`:1103`) | ESPEC_02 |
| 9 (`:121`) | `FiltroFormatosSignos` (`:102`) | `FormatosSignos` | `AnalizarFormatos` (`:168`) | ESPEC_03 |
| 10 (`:124`) | `FiltroColProbables` (`:204`) | `ColProbables` | `AnalizarCPs` (`:276`) | ESPEC_04 |
| 11 (`:127`) | `FiltroFormatos123` (`:422`) | `Formatos123` | `AnalizarFormatos123` (`:469`) | ESPEC_03 |
| 12 (`:130`) | `FiltroSimetrias` (`:153`) | `Simetrias` | `AnalizarSimetrias` (`:204`) | ESPEC_03 |
| 13 (`:133`) | `FiltroDiferencias` (`:99`) | `Diferencias` | `AnalizarSimetriasII` (`:148`) | ESPEC_03 |

(Líneas de `NombreFiltro` y `AnalisisActivo` en el fichero `MC/<Clase>.cs` de cada fila.)

### 4.3 Requisitos cruzados que el núcleo impone a los ESPEC de filtros

- **Setters de texto usados por `CondicionIfThen.getFiltro`** (deben quedar especificados con firma y semántica): `FiltroNoVariantes.SetNoVariantes(Int, Bool)`/`SetNoEquis`/`SetNoDoses` y `LlenarTodosValores()`; `FiltroDibujos.Dibujos.Add(String)`; `FiltroSignosSeguidos.SetNoVariantes/SetNoUnos/SetNoEquis/SetNoDoses(String)`; `FiltroInterrupciones.SetNoIntGlobales/SetNoIntVar/SetNoInt1/SetNoIntX/SetNoInt2` y `…GlobalSeg/VarSeg/1Seg/XSeg/2Seg(String)`; `FiltroPesosNumericos.SetPNGlobal/SetPNVar/SetPNUnos/SetPNEquis/SetPNDoses(String)`; `FiltroDistancias.SetdistVar/Unos/Equis/Doses(String)`; `FiltroContactos.SetNum1X/12/X2/11/XX/22/1V/XV/2V/VV(String)` (`MC/CondicionIfThen.cs:102-246`).
- **Propiedades que consume `Analizador.InicializarContenedorAnalisis`:** `FiltroSimetrias.ArrayAciertos`, `FiltroDiferencias.Diferencias[i].PartidosSimetricos.Count`, `FiltroValoracionSignos.ValorGlobal/ValorUnos/ValorEquis/ValorDoses` (cadenas con `#`/`-`), `FiltroColProbables.ColProbables`, `FiltroFormatosSignos.FormatosSignos[i].LineasFormatos.Count` (`MC/Analizador.cs:357-399`).
- **Contadores públicos que lee `ContenedorAnalisis.IncrementarContador`** (p. ej. `FiltroNoVariantes.NoEquis/NoDoses`) — ESPEC_06.
- **Serialización:** `ContieneDatos` es el criterio de las clases `F*Data` para escribir cada condición (`ES/F*Data.cs`, p. ej. `FNoVariantesData.cs:75`) — ESPEC_07.
- **`FiltroDiferencias`:** debe documentar explícitamente que `ObtenNoAciertosTolerancias`, `Figuras.get/set` lanzan (`MC/FiltroDiferencias.cs:124-143`).

---

## 5. Dependencias externas cuyo comportamiento condiciona el núcleo (resumen; fuente de verdad en ESPEC_06/07)

| Función | Comportamiento relevante | Evidencia |
|---------|--------------------------|-----------|
| `UtilColumnas.ConvStrToLong(string[])` | por partido: recorre el texto en pasos de 2 (`j += 2`), acumula `b = b OR (1 << "2X1".IndexOf(c))`; luego `L = (L << 3) OR UInt32(b)` (OR bit a bit); recorre del último partido al primero; carácter desconocido ⇒ `1 << -1` = `Int32.min` ⇒ `(uint)` ⇒ solo el bit 31 (sin extensión de signo) | `UT/UtilColumnas.cs:78-92` |
| `ConvStrToLong(string)` / `(s, int)` / `(s, string)` / `(s, bool[])` | `res = (res <<= 3) ^ (1 << idx)` desde el último carácter; el `XOR` con `int` negativo **extiende signo a 64 bits**; `(s,int)` añade `signosRelleno[n-1]` (lanza si `n∉1…15`); `(s,"")` ⇒ `(s, N−len)`; `(s, bool[])` pone `000` en no marcados | `UT/UtilColumnas.cs:168-220` |
| `ConvLongToStr` | emite `"2X1"[(L&7)>>1]` por partido hasta que `L==0`; no termina con `L<0`; con bits `110/101/011/111` (dobles/triples) el índice es 3 o más ⇒ excepción en el caso `110` | `UT/UtilColumnas.cs:27-44` |
| `ObtenerValoresSeparadosPorComas` | expande `a-b` (solo 2 partes) a `a,a+1,…,b`; `a>b` ⇒ `Substring(0,-1)` lanza | `UT/UtilidadesEntradasValores.cs:332-361` |
| `ObtenerListaFromTxtAciertos` | acepta `a`, `a,b`, `a-b`, `a,b-c`; deduplica; **cualquier error ⇒ lista vacía** | `UT/UtilidadesEntradasValores.cs:115-181` |
| `RangosHelper.ObtenIntArray` | `Split(',')`, trozo con exactamente un `-` ⇒ rango ascendente; si no `Convert.ToInt32(trozo.split("-")[0])` | `UT/RangosHelper.cs:32-74` |
| `ArchivoColumnasTexto` | `SiguienteColumna` ⇔ `Peek()>=0`; `LeeColumnaSinComas` = `ReadLine().Trim().ToUpper()` (no quita comas); `GuardarCols(string)` escribe `col.Replace(",","")` + `WriteLine`, escritor perezoso UTF-8 sin BOM; `ObtenNumCols` estima `tamañoFichero/(len1+2)` (supone CRLF) | `ES/ArchivoColumnasTexto.cs:70-80,183-217,314-340` |
| `AConfiguracion.Obten*` | ver §3.1 | `ES/AConfiguracion.cs:58-208,402-413` |

---

## 6. Casos golden transversales (para `tools/GoldenGen`)

| Id | Escenario | Qué volcar |
|----|-----------|------------|
| G-E2E-1 | 10 fijos + 4 triples, sin condiciones (ya en `TST/FlujosClaveTests.cs:23-30`) | `colsAnalizadas=81`, `colsAceptadas=81`, y las 81 columnas de la ruta B en orden |
| G-E2E-2 | NoVariantes `0/1/2` sobre 14 triples (ya en `TST/ManualFuncionalTests.cs:30-50`) | aceptadas 1 / 28 / 364 |
| G-E2E-3 | 6 triples + 6 dobles + 2 fijos (ya en `TST/ManualFuncionalTests.cs:78-88`) | 46 656 |
| G-GEN-1/2/3 | §2.1 | listas ordenadas de columnas y valores `Int64` |
| **G-IFTHEN-OVR** | archivo de 3 líneas `11111111111111`, `XXXXXXXXXXXXXX`, `22222222222222`; pronósticos `"1"`×14; grupo 0 con `NoVariantes` activo, variantes `0` y equis/doses «cualquier valor» (como `TST/ManualFuncionalTests.cs:34-50`); If-Then con una condición `If="Cantidad de Variantes: 14"`, `Then="Cantidad de X: 14"` y rango `1`. Derivado por lectura: ruta A ⇒ analizadas 3, aceptadas **2** (la línea `X…X` se acepta por la asignación de If-Then pese a violar pronóstico y grupo); ruta C ⇒ aceptadas **1** (AND) | `(analizadas, aceptadas)` en A, B (fichero), C y D |
| G-GRP-MASK | grupo 1 con partidos `1,2,3,4` y `NoVariantes` distinto del base | aceptadas en A y C; verifica máscara/parche/filtro parcial ignorado en C |
| G-CTL-FALLOS | 3 grupos, control `[1,2,3]` con fallos `"0,1"`; control de conjuntos `[1]` con fallos `"0"` | tabla de verdad por nº de fallos + contadores `ColumnasPorFallos*` en análisis |
| G-TOL | tolerancias `A` con aciertos `"1,3"` y `B` con `"0-1"`, con `FallosPermitidos` `""` y `"1"`; filtros NoVariantes con marcas `A`,`B` | aceptadas en ambas ramas |
| G-ERR | cada formato inválido de §2.7-2.10, 2.17 | tipo de excepción y, si procede, mensaje de `UserDialogs.ShowError` |
| G-CFG | `parametros.free1x2` shipped, uno sin nodo `salir`, uno con `PrecioApuesta="0,55"` | todos los campos de §3.1 tras la carga (verifica todo-o-nada y `Float→Double`) |

---

## 7. Abiertas (dudas no verificadas) — estado y decisión de cada entrada en [`DECISIONES.md`](DECISIONES.md) §3

- **A1.** `tools/GoldenGen` no existe aún (solo `tools/*.py`): las trazas marcadas «derivado por lectura» (orden de las 27 columnas, `2513169434708`, G-IFTHEN-OVR) están sin ejecutar.
- **A2.** `ES/ArchivoCombinacion.cs:221-232`: tras `GetGrupoFromXmlNode`, `EsGrupoBase` queda `true` solo si el nodo del grupo tiene **exactamente un** hijo (`i==0` es la última iteración) y `false` en caso contrario; además `todosPartidosActivos` ya quedó a `true` en `i==0`. No se verificó el efecto en el guardado (`ES/ArchivoCombinacion.cs:386` usa `EsGrupoBase`) ni en estadísticas (un subgrupo con un único hijo registraría filtros en `FiltrosTemp`). Tampoco si el índice `i` cuenta comentarios XML (`:195-211`: el primer hijo no comentario podría no ser tratado como control base).
- **A3.** `ControladorGrupos.AnalizaColumnaConConjuntos(contenedor)` llama a `VaciarInformacion()` antes de cada control libre `j>0` (`MC/ControladorGrupos.cs:224`). Lectura: descarta lo registrado por el control 0 (que contiene el grupo base). No verificado si la UI permite más de un control en el conjunto libre.
- **A4.** `FiltroDiferencias.ObtenNoAciertosTolerancias` lanza (`MC/FiltroDiferencias.cs:124-127`): no se comprobó si la UI permite definir tolerancias con Diferencias activo (provocaría excepción en el cálculo).
- **A5.** Formato de `FiltroValoracionSignos.Valor*` que consume `ObtenerArrayAciertos` (¿decimales, negativos?). `Convert.ToDouble` + `Round` y el `LastIndexOf('-')` fallarían con negativos (`MC/Analizador.cs:237-246`). Pendiente de ESPEC_03.
- **A6.** Tamaños de `ColumnasPorFallosDeGrupos/Conjuntos` (`AN/ContenedorAnalisisGlobal.cs:36-37`) frente a los índices `nFallados` (`MC/ControlGrupos.cs:131`, `MC/ControladorGrupos.cs:263-264`): posible `IndexOutOfRange` real en C#; no reproducido.
- **A7.** Qué estructura de errores adoptar en Swift para las excepciones C# del camino caliente (índices fuera de rango, NRE por configuración incompleta): `throws` en `analizaCombinacion` vs `precondition`. **Decisión del dueño (R2).**
- **A8.** «Reinicializar = no-op» y lectura repetida del XML en `Grupo`/`Analizador`/`AConfiguracion` (§3.2): ¿se conserva el requisito de reinicio para cambiar N/flags en la versión Mac? **Decisión del dueño (R2).**
- **A9.** `Convert.ToDouble/ToInt32/ToDateTime` con cultura actual: no está fijada la cultura del proceso en el repo (el ejecutable WinUI usa la del usuario). Para golden, fijar `es-ES`; para producción Mac decidir (T7, R2).
- **A10.** `ObtenNumCols` asume CRLF (`ES/ArchivoColumnasTexto.cs:314-340`) y alimenta «columnas máximas» y el ctor `ArchivoColumnasTexto(path)` usado en ruta B; en ficheros con `\n` el resultado difiere (ESPEC_07).
- **A11.** Uso real de `Signo`, `ReduccionTipo`, `AlgoritmoReduccion` (grep sin referencias) y de `IAppPaths`/`IProgressNotifier` (sin referencias): no se miró código generado en `obj/` ni reflexión/XAML.
- **A12.** No se leyó el cuerpo de `ContenedorAnalisis` más allá de `IncrementarContador`/`FiltrosTemp` (ESPEC_06); las afirmaciones sobre sus efectos se limitan a las líneas citadas.
