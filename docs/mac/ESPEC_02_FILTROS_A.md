# ESPEC 02 — Filtros A: Pesos numéricos · Interrupciones · Contactos · Signos seguidos

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Sigue la plantilla (§2) y las
> trampas T1–T12 de [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md). Objetivo: reescribir en Swift
> **sin mirar el C#** y obtener resultados idénticos (M-R1).
>
> **Cómo se ha verificado.** Todo `alias:línea` sale de lectura directa del C# (R4). Los números de los
> ejemplos y de las tablas golden salen de un *port de verificación* en Python escrito para este documento
> (transcripción literal de los algoritmos; la lectura «por cadenas» de las rachas de Interrupciones se
> contrastó de forma exhaustiva contra el port con todas las columnas de hasta 10 partidos). **El oráculo
> es `tools/GoldenGen` sobre el C#**: si un número de este documento discrepa de GoldenGen, manda GoldenGen
> y se corrige el documento. No existen tests actuales que toquen estos cuatro filtros
> (`Free1X2.Domain.Tests` solo usa `FiltroNoVariantes` y `FiltroColProbables`: `Free1X2.Domain.Tests/ManualFuncionalTests.cs:38,59`).

---

## 0. Base común a los cuatro filtros

### 0.1 Alias de rutas (todas relativas a la raíz del repo)

| Alias | Fichero |
|-------|---------|
| `IF` | `Free1X2.Domain/MotorCalculo/IFiltro.cs` |
| `PN` | `Free1X2.Domain/MotorCalculo/FiltroPesosNumericos.cs` |
| `INT` | `Free1X2.Domain/MotorCalculo/FiltroInterrupciones.cs` |
| `CON` | `Free1X2.Domain/MotorCalculo/FiltroContactos.cs` |
| `SEG` | `Free1X2.Domain/MotorCalculo/FiltroSignosSeguidos.cs` |
| `PNd` · `INTd` · `CONd` · `SEGd` | `Free1X2.Domain/EntradaSalida/FPesosNumericosData.cs` · `FInterrupcionesData.cs` · `FContactosData.cs` · `FSignosSeguidosData.cs` |
| `FDB` | `Free1X2.Domain/EntradaSalida/FiltroDatosBase.cs` |
| `GRP` | `Free1X2.Domain/MotorCalculo/Grupo.cs` |
| `TOL` | `Free1X2.Domain/MotorCalculo/ToleranciaFiltros.cs` |
| `IFT` | `Free1X2.Domain/MotorCalculo/CondicionIfThen.cs` |
| `CAI` | `Free1X2.Domain/Analisis/ContenedorAnalisis.cs` |
| `UEV` | `Free1X2.Domain/Utils/UtilidadesEntradasValores.cs` |
| `UC` | `Free1X2.Domain/Utils/UtilColumnas.cs` |
| `ENUM` | `Free1X2.Domain/EnumFiltros.cs` |
| `VG` | `Free1X2.Domain/VariablesGlobales.cs` |
| `ACV` | `Free1X2.WinUI/Views/Ported/AnalizarCombinacionFrmViewModel.cs` |
| `BOL` | `Free1X2.WinUI/Views/Ported/BoletoFrmViewModel.cs` |
| `PNvm` · `CONvm` | `Free1X2.WinUI/Views/Ported/PesosNumFrmViewModel.cs` · `ContactosFrmViewModel.cs` |

### 0.2 Notación de columna

- Columna = `Int64` ≥ 0, 3 bits por partido; **celda** `i` (0 = partido 1) = `(c >> 3i) & 7`.
  Códigos: `4` = «1», `2` = «X», `1` = «2», `0` = **hueco** (`*` o partido fuera del grupo)
  (`UC:25-37` decodifica con `"2X1"`; `UC:196-204` y `UC:205-220` codifican, este último con máscara de partidos).
- Los cuatro filtros recorren de la celda menos significativa (partido 1) a la más significativa con
  `while (c != 0)`: **las celdas altas a 0 no se visitan** (`PN:127-145`, `INT:242-331`, `CON:160-165,168-197`, `SEG:157-205`).
- *Columna pura* = cada celda ∈ {0,1,2,4}. Las celdas multi-signo (3,5,6,7) se documentan tal cual
  las trata cada `switch` literal, pero no se garantiza que el motor las ejerza (Abiertas A1).
- «Variante» = X o 2 (no-1). «V» en los nombres = variante.
- `N` = `VariablesGlobales.NumeroPartidos` leído **en el constructor** de cada filtro
  (`PN:55`, `INT:77`, `CON:77`, `SEG:41`); por defecto 14. Los `bool[]`/`char[]` miden `N+1`.

### 0.3 Contrato `IFiltro` (`IF:25-40`) y propuesta Swift

| Miembro C# | Semántica común | Swift propuesto |
|-----------|-----------------|-----------------|
| `bool Analizar(long)` | Reinicia contadores, recalcula, devuelve veredicto. **Sin memoria entre llamadas** (`IFT:257-259`). | `func analizar(_ c: Int64) -> Bool` |
| `string[] AnalizarFallos(long)` | `nil` = nada que mostrar; ver §0.7 | `func analizarFallos(_ c: Int64) -> [String]?` |
| `IsActive` | por defecto `false` (`PN:9`) | `var isActive = false` |
| `ContieneDatos` | por defecto `false`; lo pone a `true` el cargador (`PNd:37`, `INTd:39`, `CONd:39`, `SEGd:36`); el escritor solo emite filtros con datos (`PNd:161`, `INTd:140`, `CONd:143`, `SEGd:171`) | `var contieneDatos = false` |
| `Filtro NombreFiltro` / `EsNombreFiltro(string)` | enum `ENUM:22-38`; comparación **ordinal** con `NombreFiltro.ToString()`: `PesosNumericos` (`PN:1019-1022,1042-1049`), `NoInterrupciones` (`INT:431-434,1027-1034`), `Contactos` (`CON:322-325,913-920`), `SignosSeguidos` (`SEG:308-311,610-617`) | `enum NombreFiltro: String` con esos *raw values* |
| `ObtenNoAciertosTolerancias(string)` | cuenta de condiciones «con letra»; §0.5 | `func obtenNoAciertosTolerancias(_ letras: String) -> Int` |
| `UsaFiguras()` / `Figuras` | específico de cada filtro | ver cada sección |
| `AnalisisActivo` | lee un flag de `VariablesGlobales` (§0.8) | `var analisisActivo: Bool` (inyectado) |

**Requisito de diseño Swift.** Los contadores calculados por el último `analizar` deben quedar como
**estado almacenado** del objeto: los consumen `obtenNoAciertosTolerancias`, `ContenedorAnalisis`
(`CAI:124-254`: getters `NoInterrupciones…`, `NoContactos…`, `PesoGlobal…`, `NoVariantesSeguidas…`) y la
pantalla «Analizar combinación» (`ACV:253`). Una versión puramente funcional rompería a esos consumidores.
Un filtro no es *thread-safe*: una instancia por hilo de cálculo.

### 0.4 Dónde se invoca (Grupo)

- Los filtros viven en la lista del `Grupo`; **orden fijo** (`GRP:94-134`): NoVariantes, ValoracionSignos,
  **SignosSeguidos (3.º)**, **Interrupciones (4.º)**, Dibujos, **PesosNumericos (6.º)**, Distancias,
  GruposEquipos, **Contactos (9.º)**, FormatosSignos, ColProbables, Formatos123, Simetrias, Diferencias.
- Modo normal (`GRP:165-225`): si `!todosPartidosActivos` la columna se enmascara `c & mascara` (`GRP:171-174`,
  `GRP:160-163`); recorre filtros y **solo llama a `Analizar` si `IsActive`**; el primero que falla corta
  (`GRP:176-189`). Después, tolerancias de grupo (`GRP:194-201`) y, si procede, filtro parcial
  (`GRP:203-212`, consulta de pertenencia a un diccionario `long→bool`, `GRP:311-314`). **Los filtros no
  conocen ni la máscara ni el filtro parcial**: solo ven la columna ya enmascarada.
- Modo análisis (`GRP:226-300`): máscara **siempre** y, si el resultado es 0, se usa la columna completa
  (`GRP:232-233`); `Analizar` se llama también en filtros **no activos** cuando `AnalisisActivo` es `true`
  (`GRP:238-254`), ignorando su resultado, para que el contenedor lea sus contadores.
- `GetFiltro(nombre)` devuelve **el último filtro de la lista** (Diferencias) si el nombre no coincide con
  ninguno (`GRP:415-431`): buscar `"Interrupciones"` en vez de `"NoInterrupciones"` no falla, devuelve otro filtro.
  Nombres reales: `ENUM:22-38` (UI legacy usa el literal `"NoInterrupciones"`: `Free1X2/UI/Filtros/InterrupcionesFrm.cs:69`).

### 0.5 Valores con letra de tolerancia (Interrupciones, Contactos, Signos seguidos)

Estos tres filtros guardan, por cada contador, un `bool[N+1]` («valor permitido») y un `char[N+1]` («letra de
tolerancia», `' '` = sin letra). Los *setters* son **acumulativos** (nunca limpian; `ReinicializaValores`
sí: `INT:493-521`, `CON:381-409`, `SEG:397-411`).

**Gramática de `SetXxx(string valores)`** (idéntica en los 3 filtros; ej. `INT:523-547`, `CON:411-435`, `SEG:414-438`):

```
valores := item { "," item }          // Split(',') conserva vacíos
item    := entero | entero letra      // la letra es el ÚLTIMO carácter y debe cumplir Char.IsLetter
para cada item:
   si Char.IsLetter(item.último):  v = Convert.ToInt32(item[0..<último]);  L = item.último
   si no:                          v = Convert.ToInt32(item);               L = ' '
   permitido[v] = true;  letra[v] = L          // v fuera de 0...N → IndexOutOfRangeException
```

Errores reales a replicar (o convertir en `throw` equivalente): item vacío (`""` ⇒ `item[-1]`) →
`IndexOutOfRangeException`; prefijo no numérico → `FormatException`; `v` > N o < 0 → `IndexOutOfRangeException`.
Una lista vacía guardada por el escritor (`Get*` devuelve `""` si no hay valores) **no puede releerse** con
estos setters (`INTd:67-77`, `CONd:65-71`, `SEGd:69-74` no comprueban vacío; Abiertas A4).

**`GetXxx()`**: recorre el array en orden creciente; por cada `true` emite `v` seguido de la letra recortada
(`.Trim()`), separados por `,` (`INT:654-675`, `CON:541-562`, `SEG:518-539`). Ej.: `"0,3A,5"`.

**`ObtenNoAciertosTolerancias(letras)`**: para **cada carácter** `l` de `letras` (en orden, duplicados
cuentan doble) y para cada contador del filtro: `+1` si `letra[contadorActual] == l` (comparación exacta,
sensible a mayúsculas). Usa los contadores del **último `Analizar`** (`INT:436-488`, `CON:327-376`, `SEG:319-347`).

**Consumo en el Grupo** (a especificar en ESPEC_01; aquí solo el contrato): por cada `ToleranciaFiltros`,
acumula `Σ filtros activos de ObtenNoAciertosTolerancias(tol.LetrasTol)` (`GRP:344-351` con fallos,
`GRP:387-394` sin fallos) y exige `aciertosPermitidos[acumulado]` (`TOL:43-59`); `LetrasTol` se guarda con
`ToUpper()` (`TOL:148-152`), de modo que una letra minúscula guardada en el filtro **nunca** coincide.
Sin tolerancias definidas se sale antes con `true` (`GRP:372`; solo en la variante sin lista de fallos).
**`Analizar` no usa las letras**: un valor con letra es simplemente un valor permitido.

### 0.6 Figuras (Pesos, Contactos, Signos seguidos; **no** Interrupciones)

Una *figura* es un `Int64` que empaqueta nibbles (4 bits) de un multiconjunto de enteros.
Conversión texto ⇄ long (compartida, `UEV:220-231` y `UEV:238-253`):

- `ObtenerLongFiguraFromText("a-b-c")`: `f = 0; por cada parte: f <<= 4; f |= UInt32(Convert.ToInt32(parte))` →
  `"10-6-4"` = `0xA64`. No valida que cada parte sea < 16 (si no, se solapan nibbles).
- `ObtenerTextoFiguraFromLong(f)`: `mientras f != 0: prepend "-" + decimal(f & 15); f >>= 4`; quita el `-` inicial;
  `0` → `"0"`. Ej.: `0xA64` → `"10-6-4"`; `0x10` → `"1-0"`.
- Las figuras se guardan en XML como texto (`PNd:252-267`, `CONd:223-238`, `SEGd:214-284`) y se comparan
  como `Int64` exacto con `List<long>.Contains`: `"6-10"` y `"10-6"` son figuras distintas; los filtros solo
  generan el orden **descendente** (mayor en el nibble alto).

### 0.7 Protocolo de `AnalizarFallos`

Devuelve `nil` o un array de cadenas. Prefijo de cada cadena (lo interpreta la pantalla de análisis,
`ACV:261-301`): sin prefijo = **fallo**; `!` = fallo/observación **aceptada por tolerancia** (naranja);
`*` = nota informativa sin fallo. Los textos están **escritos en el código** (no pasan por el diccionario de
idioma) y llevan **dos espacios** antes del paréntesis; se construyen concatenando `"…#"` y luego se corta
con `Split('#')` tras quitar el último `#` (p. ej. `PN:319-323`, `INT:402-406`). Ojo: **`nil` no equivale a
«la columna pasa»** ni «no-nil» a «falla» (§1.7, §2.7, §3.7).

### 0.8 Estado global leído (ninguno escrito)

| Miembro | Dónde | Para qué |
|---------|-------|----------|
| `VariablesGlobales.NumeroPartidos` | ctor de los 4 (`PN:55`, `INT:77`, `CON:77`, `SEG:41`); `LlenarTodosValores` (`INT:1040`, `CON:926`, `SEG:623`) | tamaño de arrays y rango de `LlenarTodosValores` |
| `VariablesGlobales.AnalizarPesos` / `AnalizarInterrupciones` / `AnalizarContactos` / `AnalizarSeguidos` | `AnalisisActivo` (`PN:1133-1136`, `INT:1130-1133`, `CON:1101-1104`, `SEG:711-714`) | modo análisis (`GRP:238`); valores de `parametros.free1x2` (`VG:100`, `Free1X2.Domain/EntradaSalida/AConfiguracion.cs:123-142`) |
| `VariablesGlobales.AnalizarFigurasPesos` / `AnalizarFigurasContactos` / `AnalizarFigurasV1X2` | **no** en los filtros: los lee `CAI:138,184,206` | decide si el contenedor calcula figuras |

Swift (T10): pasar `numeroPartidos` y los 4 flags `analizar*` por el inicializador (o un objeto `Sesion`).

### 0.9 Trampas Swift comunes (concretas)

1. **Parámetros mutados.** El C# hace `columna >>= 3` sobre el parámetro (`PN:144`, `INT:247,330`, `CON:164`,
   `SEG:204`); en Swift copiar a `var c = columna` (T2). Con `Int64` negativo el `while (c != 0)` no
   termina (shift aritmético): exigir `columna >= 0`.
2. **`switch` sin `default`** en C# (`PN:129-143`, `SEG:159-182`): en Swift añadir `default: break`; las
   celdas fuera de {4,2,1} se ignoran sin tocar ningún contador.
3. **`Split` conserva vacíos** en C#; `String.split` de Swift los omite por defecto → usar
   `split(separator:omittingEmptySubsequences:false)` o `components(separatedBy:)` en `Set*`, `Get*` y en el
   corte de mensajes (T7).
4. **`Convert.ToInt32(string)`** acepta espacios y signo `+/-` y lanza `FormatException`; `Int(_:)` de Swift
   devuelve `nil` con espacios. Recortar espacios y rechazar lo demás con error explícito (T7).
5. **`Char.IsLetter`** = categoría Unicode L* (Lu, Ll, Lt, Lm, Lo). No asumir que `Character.isLetter` de
   Swift sea idéntico; usar `unicodeScalars.last?.properties.generalCategory` ∈ {…Letter} (T11).
6. **`char[]` no inicializado = `'\0'`** (Contactos, §3.7-C4). En Swift modelar la letra como `Character`
   con centinela `" "` y, solo donde el C# no inicializa, `"\0"`.
7. **Índices fuera de rango**: C# lanza; Swift aborta. Validar y lanzar un error propio en `Set*`.
8. **Rendimiento (hot path).** `Analizar` se ejecuta hasta 3^14 = 4 782 969 veces por cálculo
   (`docs/MANUAL_FLUJOS.md` §3). El C# ya eliminó asignaciones por columna (`PN:14-16,174-224`,
   `CON:973-981`, `SEG:216-225`): en Swift usar arrays de tamaño fijo / `ContiguousArray` / bitsets
   (`UInt32` por conjunto, N+1 ≤ 17 bits) y `@inline(__always)`; el resultado debe ser idéntico (R1).
9. **Aritmética.** Los cuatro filtros usan solo enteros: no hay coma flotante, redondeos, desempates ni colecciones
   hash; la única división es la entera de `raíz` (`PN:232-233`) y las ordenaciones (`CON:996-1006`, `SEG:256-266`) son
   de enteros (cualquier orden estable o no da lo mismo, T5).
10. **Mensajes**: copiar literalmente (dobles espacios, acentos como en `número`, `Tolerancia  en` con 2 espacios en SEG) y **no
   localizar**.

### 0.10 Columnas golden compartidas (COLS-A)

14 partidos; `*` = hueco (en GoldenGen: `UtilColumnas.ConvStrToLong(s, partidosMarcados)` con
`marcados[i] = (s[i] != '*')`, `UC:205-220`). Los `long` permiten verificar la codificación.

| id | columna (partido 1 → 14) | `long` |
|---|---|---|
| c1 | `1X21X1121X1X21` | 2289556400212 |
| c2 | `11111111111111` | 2513169434916 |
| c3 | `XXXXXXXXXXXXXX` | 1256584717458 |
| c4 | `22222222222222` | 628292358729 |
| c5 | `12X1X1211X12X1` | 2349691447436 |
| c6 | `1X2X1X2X1X2X1X` | 1392983229524 |
| c7 | `1112211X1X2X21` | 2286337266468 |
| c8 | `12121212121212` | 837723144972 |
| c9 | `1XX211122XX1X2` | 723990495892 |
| m0 | `**************` | 0 |
| m1 | `1*X*2*1*X*2*1*` | 275986256004 |
| m2 | `**1X2*****1X2*` | 90194318592 |
| m3 | `*1*1*1*1*1*1*1` | 2233928386592 |
| m4 | `X*X*X*1*2*2*2*` | 69811052674 |
| m5 | `1*1*1*1*X*X*X1` | 2338644312324 |

Observación: `c5` es el reverso de `c1`; Contactos y Signos seguidos dan los mismos contadores, Interrupciones
coincide salvo `VarSeg`/`1Seg` (2,3 ↔ 3,2) y **los pesos numéricos difieren** (dependen de la posición).

---

## 1. `FiltroPesosNumericos` — `Free1X2.Domain/MotorCalculo/FiltroPesosNumericos.cs`

### 1.1 Propósito y definición matemática

Mide una «huella» módulo-9 de la columna: asigna a cada partido un valor según **su signo y su posición**,
suma por signo y reduce cada suma a un dígito 0–9 (raíz digital). Compara cinco pesos (1, X, 2, Variantes,
Global) con conjuntos permitidos.

Sea `k` el **ordinal 0-based entre las celdas no nulas de un solo signo** (4, 2 o 1), recorridas desde el
partido 1 (`PN:120,127-145`). Tablas de 16 posiciones (`PN:58-60`):

```
IU (unos)  = [7,5,3,1,8,6,4,2,9,7,5,3,1,8,6,4]
IE (equis) = [5,1,6,2,7,3,8,4,9,5,1,6,2,7,3,8]
ID (doses) = [1,2,3,4,5,6,7,8,9,1,2,3,4,5,6,7]
```

```
Σ1 = Σ_{k: celda k = '1'} IU[k]     ΣX = Σ_{k: 'X'} IE[k]     Σ2 = Σ_{k: '2'} ID[k]
raíz(v):  mientras v > 9: v = ⌊v/10⌋ + (v − 10·⌊v/10⌋)          // PN:226-241
pesoUnos  = raíz(Σ1)      pesoEquis = raíz(ΣX)      pesoDoses = raíz(Σ2)       // PN:148-154
pesoVar    = raíz(pesoEquis + pesoDoses)        // usa PESOS, no sumas      PN:157-158
pesoGlobal = raíz(pesoVar + pesoUnos)                                        PN:161-162
```

`raíz(0) = 0`, `raíz(9) = 9`, `raíz(10) = 1`, `raíz(18) = 9` (no es «mod 9»: el 9 es estable y solo 0 da 0).
**Compactación (preservar):** el índice `k` avanza únicamente en celdas {4,2,1}; los huecos (0) y las celdas
multi-signo no consumen índice, así que un grupo con partidos 1,3,5… calcula sus pesos «como si estuvieran
consecutivos» (comentario del autor en `PN:122-125`).

**Ejemplo trabajado** — `c1 = 1X21X1121X1X21` (`long` 2289556400212):

| partido | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| signo | 1 | X | 2 | 1 | X | 1 | 1 | 2 | 1 | X | 1 | X | 2 | 1 |
| k | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 |

- Unos (k = 0,3,5,6,8,10,13): `IU` = 7+1+6+4+9+5+8 → **Σ1 = 40** → pesoUnos = raíz(40) = 4.
- Equis (k = 1,4,9,11): `IE` = 1+7+5+6 → **ΣX = 19** → 1+9 = 10 → 1+0 = 1 → pesoEquis = 1.
- Doses (k = 2,7,12): `ID` = 3+8+4 → **Σ2 = 15** → pesoDoses = 6.
- pesoVar = raíz(1+6) = **7**; pesoGlobal = raíz(7+4) = raíz(11) = **2**.
- Multiconjunto de pesos {4,1,6,7,2}: cinco valores distintos → figura `1-1-1-1-1` (69905).

### 1.2 Tipos y datos

| Campo C# | Swift | Rango / defecto | Evidencia |
|---|---|---|---|
| `pesoGlobal/Unos/Var/Equis/Doses` | `Int32` (estado, expuesto como `pesoGlobal`, `pesoUnos`, `pesoVariantes`, `pesoEquis`, `pesoDoses`) | 0…9 | `PN:18-22,1081-1100` |
| `pnGlobal, pnVariantes, pnUnos, pnEquis, pnDoses` | `[Bool]` (N+1) | pesos *permitidos*; defecto todo `false` | `PN:32-36,63-67` |
| `pnGlobalTol … pnDosesTol` | `[Bool]` (N+1) | pesos *tolerados*; defecto `false` | `PN:38-42,69-73` |
| `tolerancias` | `[Bool]` (6) | `tolerancias[k]` = «se admiten k fallos tolerados», k = 0…5 | `PN:44,90-95` |
| `falloPNGlobalBase … falloPNDosesBase` | `Bool` ×5 | marcas de qué pesos están fuera del conjunto permitido | `PN:47-51` |
| `noFallosActual` | `Int32` | 0…5 | `PN:46` |
| `figuras` | `[Int64]` | defecto `[]` (no nulo) | `PN:11` |
| `figuraPesos` | `Int64` (`figura`) | se asigna en `ObtenerFiguraLong` | `PN:13,223` |

Los arrays miden `N+1` pero **solo se indexan 0…9** (los pesos), de modo que con N < 9 el C# lanzaría
`IndexOutOfRange` (Abiertas A2). Configuración: tipos XML `global/variantes/unos/equis/doses` →
`SetPNGlobal/SetPNVar/SetPNUnos/SetPNEquis/SetPNDoses` (`PNd:72-91`); sección `tolerancias` con atributo
`noTolerancias` → `PonerTolerancia`, y sub-`valores` → `SetPN*Tol` (`PNd:97-135`); sección `figuras` →
`Figuras` (`PNd:137-153`). Las tres sobrecargas de cada setter (`int,bool` / `int[]` / `string` con
enteros simples **sin letras**) están en `PN:513-764`; la de `string` hace `Split(',')` y **sale sin hacer nada
si la cadena está vacía** (`PN:528-529,551-552` …). `GetPN*()` y `GetTolerancias()` devuelven los índices
`true` en orden creciente separados por coma (`PN:766-1017`). `ReinicializaValores` borra los 10 arrays y las 6
tolerancias, **no** las figuras (`PN:487-511`). `LlenarTodosValores` pone `0…9` en cada uno de los **10**
arrays (permitidos y tolerados) que esté totalmente vacío; **no toca `tolerancias`** (`PN:1051-1070`).

### 1.3 Entradas / salidas

`analizar(columna) -> Bool`; efectos: actualiza los 5 pesos, las marcas `falloXBase`, `noFallosActual` y
`figuraPesos`. `analizarFallos(columna) -> [String]?` (§1.7). `ObtenNoAciertosTolerancias` devuelve **siempre 0**
(`PN:1036-1040`: «este filtro no usa tolerancias de grupo»). `obtenerFiguraLong()` es público y lo llama
`ContenedorAnalisis` antes de leer `FiguraPesos.Figura` (`CAI:184-189`).

### 1.4 Algoritmo

```
analizar(col):                                                          // PN:447-452
  inicializaContadores()           // pesos=0, noFallosActual=0, falloXBase=false     PN:98-112
  analizaColumna(col)              // arriba, PN:113-163
  return cumpleCondiciones()

cumpleCondiciones():                                                    // PN:397-425
  valida = false
  si esColumnaValida():            // SIN cortocircuito: evalúa los 5 y marca TODOS los que fallan
      si tolerancias[0]: valida = true
  si !valida: valida = sonToleranciasValidas()
  si valida y usaFiguras(): valida = analizaFiguras()
  return valida

esColumnaValida():                                                      // PN:243-278
  ok = true
  si !pnGlobal[pesoGlobal]    { falloGlobal = true; ok = false }
  si !pnVariantes[pesoVar]    { falloVar    = true; ok = false }
  si !pnUnos[pesoUnos]        { falloUnos   = true; ok = false }
  si !pnEquis[pesoEquis]      { falloEquis  = true; ok = false }
  si !pnDoses[pesoDoses]      { falloDoses  = true; ok = false }
  return ok

sonToleranciasValidas():                                                // PN:331-395   (orden G, V, U, E, D)
  si falloGlobal: si pnGlobalTol[pesoGlobal]   { noFallosActual += 1 } si no { return false }
  si falloVar:    si pnVariantesTol[pesoVar]   { noFallosActual += 1 } si no { return false }
  si falloUnos:   si pnUnosTol[pesoUnos]       { noFallosActual += 1 } si no { return false }
  si falloEquis:  si pnEquisTol[pesoEquis]     { noFallosActual += 1 } si no { return false }
  si falloDoses:  si pnDosesTol[pesoDoses]     { noFallosActual += 1 } si no { return false }
  return tolerancias[noFallosActual]

analizaFiguras():                                                       // PN:164-172
  si figuras.count == 5: return true          // «están las 5 figuras»: atajo, SIN calcular
  obtenerFiguraLong()
  return figuras.contains(figuraPesos)

obtenerFiguraLong():                                                    // PN:174-224
  cnt[0…9] = 0;  cnt[pesoUnos]++; cnt[pesoEquis]++; cnt[pesoDoses]++; cnt[pesoVar]++; cnt[pesoGlobal]++
  prod = Π cnt[i] sobre los cnt[i] ≠ 0
  figura = {1:69905, 2:8465, 3:785, 4:545, 6:50}[prod]  ?? 0       // cualquier otro producto → 0
```

Forma cerrada equivalente de `analizar` (sin figuras): con `F` = conjunto de categorías con
`pnX[peso]==false`: **acepta ⇔ ∀ f∈F: pnXTol[peso_f] ∧ tolerancias[|F|]**. Con `F = ∅` exige `tolerancias[0]`.

Códigos de figura (nibbles descendentes del multiconjunto de repeticiones de los 5 pesos):

| Repeticiones de pesos | Producto | Figura (long) | Texto | Nota |
|---|---|---|---|---|
| 1,1,1,1,1 | 1 | 69905 = 0x11111 | `1-1-1-1-1` | |
| 2,1,1,1 | 2 | 8465 = 0x2111 | `2-1-1-1` | |
| 3,1,1 | 3 | 785 = 0x311 | `3-1-1` | |
| 2,2,1 | 4 | 545 = 0x221 | `2-2-1` | |
| **4,1** | 4 | 545 | `2-2-1` | **colisiona con 2,2,1** |
| 3,2 | 6 | 50 = 0x32 | `3-2` | |
| **5** (todos iguales) | 5 | **0** | `0` | sin `case` → 0 |

Alcanzabilidad (port, exhaustivo): en columnas de **14 partidos completos** solo aparecen los patrones
(2,1,1,1), (1,1,1,1,1), (3,1,1), (2,2,1), (3,2) — 1 778 117 / 1 755 297 / 600 116 / 416 810 / 232 629 columnas
de las 3^14; (4,1) y (5) solo con columnas **cortas o enmascaradas** (p. ej. `m5`, `m0`).

### 1.5 Interacción con tolerancias / filtro parcial

- **Tolerancias propias** del filtro (arrays `pn*Tol` + `tolerancias[]`), independientes de las letras de grupo.
- `ObtenNoAciertosTolerancias = 0`: nunca suma a las tolerancias de grupo (§0.5).
- La columna llega enmascarada por el grupo (§0.4): los pesos se recalculan con compactación (§1.1).
- El filtro parcial es posterior y ajeno al filtro.

### 1.6 Estado global
Lectura de `NumeroPartidos` (ctor) y `AnalizarPesos` (`AnalisisActivo`). Nada se escribe. Ver §0.8.

### 1.7 Peculiaridades a preservar

1. **`tolerancias[0]` es obligatoria para aceptar columnas sin fallos** (`PN:402-414,394`): un filtro con todos
   los `pn*` bien puestos pero sin `PonerTolerancia(0)` **rechaza todas las columnas**. La UI pasa `"0"` cuando no hay
   tolerancias definidas (`PNvm:258,262`) y, si las hay, el texto de `ToleranciaValores()` (`PNvm:257-258`; su contenido no se ha verificado, A11).
2. **`analizarFallos` y `analizar` discrepan**: con `tolerancias={1}` y una columna sin fallos, `analizar` →
   `false` pero `analizarFallos` → `nil` (`PN:397-425,427-445,454-485`; caso PN-G).
3. **If-Then con «Peso numérico» nunca se cumple**: `IFT:179-198` construye el filtro, llama a
   `LlenarTodosValores` (no toca `tolerancias`, `PN:1051-1070`) y nunca a `PonerTolerancia` (único uso en
   `PNd:100` y `PNvm:258,262`); con `tolerancias` todo `false`, `Analizar` devuelve siempre `false`
   (`PN:405-414,394`). Confirmar con golden (Abiertas A3). Si es así, un `(NO) Peso numérico…` se cumple siempre.
4. `esColumnaValida()` **marca todas** las categorías fallidas (sin `return` temprano) porque
   `sonToleranciasValidas` depende de esas marcas (`PN:243-278,331-395`): no refactorizar a cortocircuito.
5. `sonToleranciasValidas` **muta** `noFallosActual` y corta en la primera categoría no tolerada (orden G,V,U,E,D).
6. El atajo `figuras.count == 5` cuenta elementos, no valores distintos (`PN:166`); acepta incluso la figura 0.
7. Colisión 4,1 ↔ 2,2,1 y «5 iguales» → 0 (`PN:204-222`): replicar tal cual, también al **mostrar**
   `Fallo en Figuras  (2-2-1)` / `(0)`.
8. `PonerValoresTolerancias` llama a `PonerTolerancia(int[])` **sin comprobar `null`**: un `noTolerancias=""`
   (lo que escribe `GetTolerancias()` sin tolerancias) provoca `NullReferenceException` (`PNd:99-100`,
   `FDB:33-50` devuelve `null` si `""`). Los `SetPN*(int[])` sí comprueban `null` (`PNd:70`).
9. Los textos de `analizarFallos` tienen dos espacios antes de `(` y no se traducen.

**`analizarFallos` (exacto, `PN:280-329,427-445,454-485`)**

```
arrayFallos = nil;  arrayFiguras = nil
inicializaContadores(); analizaColumna(col)
texto = ""
si !pnGlobal[pesoGlobal]: falloGlobal=true; texto += "Fallo en peso global  (\(pesoGlobal))#"
si !pnVariantes[pesoVar]: falloVar=true;    texto += "Fallo en peso de variantes  (\(pesoVar))#"
si !pnUnos[pesoUnos]:     falloUnos=true;   texto += "Fallo en peso de unos  (\(pesoUnos))#"
si !pnEquis[pesoEquis]:   falloEquis=true;  texto += "Fallo en peso de equis  (\(pesoEquis))#"
si !pnDoses[pesoDoses]:   falloDoses=true;  texto += "Fallo en peso de doses  (\(pesoDoses))#"
si usaFiguras() y !analizaFiguras(): textoFig = "Fallo en Figuras  (\(textoFigura(figuraPesos)))#"
arrayFallos  = texto.vacío ? nil : split(texto.dropLast(), "#")           // misma lógica para textoFig
si arrayFallos != nil y sonToleranciasValidas():                          // OJO: muta noFallosActual
     arrayFallos = arrayFallos.map { "!" + $0 } + ["*\(noFallosActual) tolerancias aceptadas."]
resultado = (arrayFallos, arrayFiguras) según cuáles sean nil:
     ambos nil → nil;  solo figuras → copia de figuras;  solo fallos → fallos;  ambos → fallos + figuras
```

Con `arrayFallos == nil` **no** se mira `tolerancias[0]` (ver peculiaridad 2).

### 1.8 Trampas Swift específicas

- `figuraPesos` puede ser `nil` hasta el primer `obtenerFiguraLong()` (`PN:13,180`): exponer `Int64` con 0 inicial
  no cambia nada porque siempre se asigna antes de leerse.
- `raíz(v)` con `Int32`: usar división entera (`/`), sin pasar por `Double` (T9).
- El `switch` sobre `prod` debe tener `default → 0` (el C# deja `l = 0` inicial, `PN:203`).
- `cnt` indexado por peso 0…9 (10 posiciones); los pesos nunca superan 9.
- `indices*` tienen 16 entradas: con > 16 celdas no nulas el C# lanzaría (solo N > 16; A2).

### 1.9 Casos golden propuestos

**(a) Contadores sobre COLS-A** (`obtenerFiguraLong` incluido; salida = los 5 pesos + figura):

| id | Σ1 | ΣX | Σ2 | peso1 | pesoX | peso2 | pesoV | pesoG | figura (long / texto) |
|---|---|---|---|---|---|---|---|---|---|
| c1 | 40 | 19 | 15 | 4 | 1 | 6 | 7 | 2 | 69905 (`1-1-1-1-1`) |
| c2 | 69 | 0 | 0 | 6 | 0 | 0 | 0 | 6 | 50 (`3-2`) |
| c3 | 0 | 66 | 0 | 0 | 3 | 0 | 3 | 3 | 50 (`3-2`) |
| c4 | 0 | 0 | 60 | 0 | 0 | 6 | 6 | 6 | 50 (`3-2`) |
| c5 | 38 | 20 | 12 | 2 | 2 | 3 | 5 | 7 | 8465 (`2-1-1-1`) |
| c6 | 25 | 28 | 12 | 7 | 1 | 3 | 4 | 2 | 69905 (`1-1-1-1-1`) |
| c7 | 42 | 15 | 15 | 6 | 6 | 6 | 3 | 9 | 785 (`3-1-1`) |
| c8 | 37 | 0 | 29 | 1 | 0 | 2 | 2 | 3 | 8465 (`2-1-1-1`) |
| c9 | 28 | 15 | 26 | 1 | 6 | 8 | 5 | 6 | 8465 (`2-1-1-1`) |
| m0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 (`0`) |
| m1 | 12 | 8 | 9 | 3 | 8 | 9 | 8 | 2 | 8465 (`2-1-1-1`) |
| m2 | 8 | 8 | 9 | 8 | 8 | 9 | 8 | 7 | 785 (`3-1-1`) |
| m3 | 34 | 0 | 0 | 7 | 0 | 0 | 0 | 7 | 50 (`3-2`) |
| m4 | 1 | 12 | 18 | 1 | 3 | 9 | 3 | 4 | 8465 (`2-1-1-1`) |
| m5 | 18 | 18 | 0 | 9 | 9 | 0 | 9 | 9 | 545 (`2-2-1`) |

**(b) Configuraciones** (sobre `c1`; `Σ` como en 1.1; `pn…` = listas pasadas a `SetPN…(string)`;
`tols` = `PonerTolerancia(string)`). Esperado = port de verificación.

| id | configuración | `analizar(c1)` | `analizarFallos(c1)` |
|---|---|---|---|
| PN-A | G{2,3} V{7} U{4} E{1} D{6}; tols "0" | `true` | `nil` |
| PN-B | como A pero D{5} | `false` | `["Fallo en peso de doses  (6)"]` |
| PN-C | como A pero D{5}, tolD{6}; tols "0,1" | `true` | `["!Fallo en peso de doses  (6)", "*1 tolerancias aceptadas."]` |
| PN-D | como C pero tols "0" | `false` | `["Fallo en peso de doses  (6)"]` |
| PN-E | como A + figuras {8465} | `false` | `["Fallo en Figuras  (1-1-1-1-1)"]` |
| PN-F | como A + figuras {69905,8465,785,545,50} (5 elementos) | `true` | `nil` |
| PN-G | como A pero tols "1" (sin 0) | `false` | `nil` |
| PN-H | G{2,3} V{8} U{4} E{1} D{5}, tolV{7}, tolD{6}; tols "0,2" | `true` | `["!Fallo en peso de variantes  (7)", "!Fallo en peso de doses  (6)", "*2 tolerancias aceptadas."]` |
| PN-I | como H pero tols "0,1" | `false` | `["Fallo en peso de variantes  (7)", "Fallo en peso de doses  (6)"]` |
| PN-J | como A + figuras {69905} | `true` | `nil` |

Sobre `m5 = 1*1*1*1*X*X*X1` (pesos 9,9,0,9,9; patrón 4,1): PN-K1 G{9} V{9} U{9} E{9} D{0}, tols "0", figuras
{545} → `true`/`nil`; PN-K2 igual con figuras {8465} → `false`/`["Fallo en Figuras  (2-2-1)"]` (colisión).
Sobre `m0` (columna 0, pesos 0,0,0,0,0; «5 iguales» → figura 0): PN-L1 todos {0}, tols "0", figuras {0} →
`true`; PN-L2 figuras {50} → `false`/`["Fallo en Figuras  (0)"]`; PN-L3 con 5 figuras cualesquiera → `true`.

**(c) Errores** (registrar excepción en GoldenGen): `PonerTolerancia(6,true)` → `IndexOutOfRange`;
`SetPNUnos("10")` con N = 14 → válido (índice ≤ N) pero inalcanzable; `SetPNUnos("15")` → `IndexOutOfRange`;
carga XML con `noTolerancias=""` → `NullReferenceException`; `SetPNGlobal("")` → sin efecto.

### 1.10 Evidencia
Algoritmo `PN:113-163,164-241,243-278,331-425,447-452`; fallos `PN:280-329,427-445,454-485`; API `PN:487-1070`;
tablas `PN:58-60`; figuras `PN:164-224`; propiedades `PN:1081-1100,1106-1142`; config `PNd:32-153`;
consumidor `CAI:177-199`; UI de configuración `PNvm:231-267`.

---

## 2. `FiltroInterrupciones` — `Free1X2.Domain/MotorCalculo/FiltroInterrupciones.cs`

### 2.1 Propósito y definición matemática

Cuenta **cambios de signo entre partidos consecutivos** («interrupciones») y las **rachas de cambios**
(«interrupciones seguidas»). Diez contadores:

| Contador (getter) | Definición |
|---|---|
| `NoInterrupciones` (Glob) | nº de pares consecutivos con signo distinto |
| `NoInterrupcionesUnos` | cambios que **salen de «1»** (1→X, 1→2) |
| `NoInterrupcionesEquis` | cambios que salen de «X» (X→1, X→2) |
| `NoInterrupcionesDoses` | cambios que salen de «2» (2→1, 2→X) |
| `NoInterrupcionesVariantes` (Var) | cambios **variante → 1** (X→1, 2→1). **No** cuenta X↔2 ni 1→variante |
| `…GlobalesSeguidas` | mayor nº de cambios consecutivos sin ninguna repetición intermedia |
| `…UnosSeguidos` / `…VariantesSeguidas` | ver cadenas abajo |
| `…EquisSeguidas`, `…DosesSeguidas` | ver cadenas abajo |

Sea `s_0…s_{m-1}` la secuencia de celdas **no nulas** (huecos saltados), `T` = índices `j` con `s_j ≠ s_{j+1}`.
El algoritmo literal es la máquina de estados de §2.4. Lectura equivalente por cadenas (informativa,
verificada exhaustivamente hasta 10 partidos): un **segmento** es un tramo maximal de transiciones sin pares
repetidos; dentro de él, para un signo `s` una **cadena de s** es un tramo maximal de transiciones **todas
las cuales tocan a `s`** (como origen o destino). Entonces:
`GlobSeg` = longitud máxima de segmento; `1Seg` = máx. nº de transiciones *con origen 1* en una cadena de 1;
`VarSeg` = máx. nº de transiciones *con destino 1* en una cadena de 1; `XSeg` = máx. nº de transiciones con
origen X en una cadena de X; `2Seg` = ídem con 2. (Una transición X↔2 rompe las cadenas de 1; 1↔2 rompe las de X;
1↔X rompe las de 2.)

**Ejemplo trabajado** — `c1 = 1X21X1121X1X21`; estado tras cada par (pares 13; `ngs,nvs,nus,nxs,nds` son
las rachas en curso; las columnas finales son los máximos acumulados):

| paso | par | evento | Glob | Var | 1 | X | 2 | ngs | nvs | nus | nxs | nds | GlobSeg | VarSeg | 1Seg | XSeg | 2Seg |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 1→X | cambia | 1 | 0 | 1 | 0 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | X→2 | cambia | 2 | 0 | 1 | 1 | 0 | 2 | 0 | 0 | 1 | 0 | 0 | 0 | 1 | 0 | 0 |
| 3 | 2→1 | cambia | 3 | 1 | 1 | 1 | 1 | 3 | 1 | 0 | 0 | 1 | 0 | 0 | 1 | 1 | 0 |
| 4 | 1→X | cambia | 4 | 1 | 2 | 1 | 1 | 4 | 1 | 1 | 0 | 0 | 0 | 0 | 1 | 1 | 1 |
| 5 | X→1 | cambia | 5 | 2 | 2 | 2 | 1 | 5 | 2 | 1 | 1 | 0 | 0 | 0 | 1 | 1 | 1 |
| 6 | 1→1 | repite | 5 | 2 | 2 | 2 | 1 | 0 | 0 | 0 | 0 | 0 | 5 | 2 | 1 | 1 | 1 |
| 7 | 1→2 | cambia | 6 | 2 | 3 | 2 | 1 | 1 | 0 | 1 | 0 | 0 | 5 | 2 | 1 | 1 | 1 |
| 8 | 2→1 | cambia | 7 | 3 | 3 | 2 | 2 | 2 | 1 | 1 | 0 | 1 | 5 | 2 | 1 | 1 | 1 |
| 9 | 1→X | cambia | 8 | 3 | 4 | 2 | 2 | 3 | 1 | 2 | 0 | 0 | 5 | 2 | 1 | 1 | 1 |
| 10 | X→1 | cambia | 9 | 4 | 4 | 3 | 2 | 4 | 2 | 2 | 1 | 0 | 5 | 2 | 1 | 1 | 1 |
| 11 | 1→X | cambia | 10 | 4 | 5 | 3 | 2 | 5 | 2 | 3 | 1 | 0 | 5 | 2 | 1 | 1 | 1 |
| 12 | X→2 | cambia | 11 | 4 | 5 | 4 | 2 | 6 | 0 | 0 | 2 | 0 | 5 | 2 | 3 | 1 | 1 |
| 13 | 2→1 | cambia | 12 | 5 | 5 | 4 | 3 | 7 | 1 | 0 | 0 | 1 | 5 | 2 | 3 | 2 | 1 |

Cierre (tras el bucle se vuelve a hacer `max` con las rachas vivas): **GlobSeg 7, VarSeg 2, 1Seg 3, XSeg 2, 2Seg 1**.
Resultado: `Glob=12, Var=5, 1=5, X=4, 2=3 | seguidas 7,2,3,2,1`.

### 2.2 Tipos y datos

| Campo C# | Swift | Rango / defecto | Evidencia |
|---|---|---|---|
| `noIntGlobal, noIntVar, noInt1, noIntX, noInt2` | `Int32` | 0…N−1 | `INT:33-37` |
| `noIntGlobalSeg, noIntVarSeg, noInt1Seg, noIntXSeg, noInt2Seg` | `Int32` | 0…N−1 | `INT:40-44` |
| `intGlobal, intVar, int1, intX, int2` + `…Seg` (10) | `[Bool]` (N+1) | permitidos; defecto `false` | `INT:49-59,79-89` |
| `intGlobalTol … int2SegTol` (10) | `[Character]` (N+1) | defecto `' '` (**sí** inicializado) | `INT:62-72,91-127` |
| `figuras` | `[Int64]?` | **nunca asignada → `nil`**; `UsaFiguras()` = `false` | `INT:30,1110-1129` |
| `byte` locales (`ngs…nds`, `ant`, `act`) | `Int32`/`UInt8` | ≤ N | `INT:237-239` |

Configuración (`INTd:75-121`): tipos XML `global,var,unos,equis,doses` → `SetNoIntGlobales/SetNoIntVar/SetNoInt1/SetNoIntX/SetNoInt2`;
`globalSeg,varSeg,unosSeg,equisSeg,dosesSeg` → `SetNoIntGlobalSeg/…VarSeg/…1Seg/…XSeg/…2Seg`. Valores con letra (§0.5).
**Compatibilidad 0.23 (preservar):** si el XML no trae **ninguna** sección `*Seg`, se activan las 5 seguidas con
`"0,1,…,14"` sin letras (`INTd:58-61,125-133`); exige N ≥ 14 (si no, `IndexOutOfRange`; A2).
`LlenarTodosValores` rellena `0…N` **inclusive** en cada uno de los 10 conjuntos vacíos (`INT:1036-1055`).
`GetIntGlobales/GetIntVar/GetInt1/GetIntX/GetInt2/GetIntGlobalSeg/…` según §0.5 (`INT:654-767,899-1012`).

### 2.3 Entradas / salidas

`analizar(Int64) -> Bool`; `analizaColumna(Int64)` y `analizaColumna(String)` **públicos** (la UI del boleto usa
la de `String` para ordenar columnas por `NoInterrupciones`: `BOL:275-276`); getters de los 10 contadores
(`INT:1066-1105`); `analizarFallos`; `obtenNoAciertosTolerancias` (§0.5).

### 2.4 Algoritmo (versión `long`, la que usa `Analizar`: `INT:233-337`)

```
analizaColumna(col):
  reset 10 contadores a 0                                              // INT:132-145,235
  var c = col; var ant: UInt8 = 0
  var ngs = 0, nvs = 0, nus = 0, nxs = 0, nds = 0
  mientras c != 0:
     mientras ant == 0:  ant = UInt8(c & 7); c >>= 3                  // primera celda ≠ 0
     act = UInt8(c & 7)
     si act == 0 { c >>= 3; continue }                                // hueco: se omite sin tocar nada
     si act == ant:
         máx(ngs→GlobSeg); máx(nus→1Seg); máx(nds→2Seg); máx(nxs→XSeg); máx(nvs→VarSeg)
         ngs = nus = nds = nxs = nvs = 0
     si no:
         Glob += 1; ngs += 1
         según ant:
           4 ('1'):  noInt1 += 1; nus += 1
                     si act == 1:  máx(nxs→XSeg); nxs = 0              // 1→2
                     si no:        máx(nds→2Seg); nds = 0              // 1→X (y cualquier otro)
           1 ('2'):  noInt2 += 1; nds += 1
                     si act == 4:  Var += 1; nvs += 1; máx(nxs→XSeg); nxs = 0       // 2→1
                     si no:        máx(nus→1Seg); máx(nvs→VarSeg); nus = 0; nvs = 0 // 2→X
           otro ('X'): noIntX += 1; nxs += 1
                     si act == 4:  Var += 1; nvs += 1; máx(nds→2Seg); nds = 0        // X→1
                     si no:        máx(nus→1Seg); máx(nvs→VarSeg); nus = 0; nvs = 0  // X→2
     ant = act; c >>= 3
  máx(ngs→GlobSeg); máx(nus→1Seg); máx(nds→2Seg); máx(nxs→XSeg); máx(nvs→VarSeg)      // cierre

esColumnaValida():   intGlobal[Glob] ∧ intVar[Var] ∧ int1[n1] ∧ intX[nX] ∧ int2[n2]
                     ∧ intGlobalSeg[GlobSeg] ∧ intVarSeg[VarSeg] ∧ int1Seg[1Seg] ∧ intXSeg[XSeg] ∧ int2Seg[2Seg]
                     // INT:338-355 (un único && encadenado; resultado = AND de los 10)
analizar(col) = reset; analizaColumna(col); return esColumnaValida()      // INT:415-420
```

`máx(a→B)` = `si a > B { B = a }`. La versión `String` (`INT:147-231`) es equivalente: quita los `*`
(`columna.Replace("*","")`), usa `ant='0'` inicial, ignora el primer carácter para contar (`if (i > 0)`) y
distingue `'1'`, `'2'`, resto (= X). Verificado exhaustivamente igual a la de `long` para columnas puras.

### 2.5 Interacción con tolerancias / filtro parcial

Letras de tolerancia por valor (§0.5): `obtenNoAciertosTolerancias(letras)` suma **hasta 10 por letra**, una por
contador cuyo valor actual lleve esa letra (`INT:436-488`). `Analizar` ignora las letras. Filtro parcial y
máscara: igual que §1.5.

### 2.6 Estado global
`NumeroPartidos` (ctor y `LlenarTodosValores`), `AnalizarInterrupciones`. Nada se escribe.

### 2.7 Peculiaridades a preservar

1. **Definiciones asimétricas** (tabla 2.1): `Var` solo cuenta variante→1; `1/X/2` cuentan por **origen**.
2. **Rachas = cadenas** (2.1), no «rachas de signo»; la repetición de signo lo resetea todo.
3. Para `ant`, el `default` trata como **X** cualquier código distinto de 4 y 1 (`INT:307-326`); el `switch (act)`
   interno solo distingue `case 1`/`case 4` y deja el resto en `default`. Con columnas puras es transparente.
4. **Huecos (celdas 0) se saltan** sin cortar rachas (`INT:249-254`); la versión `String` elimina `*`.
5. **Sin figuras**: aunque `IFiltro` las expone, `Analizar` no las usa y `figuras == nil` (`INT:30,409-412,1110-1129`).
6. `AnalizarFallos` **no es `nil` aunque la columna sea válida** si algún valor actual lleva letra: emite
   `!Tolerancia…` (`INT:361-400`); y no emite nada de «tolerancia» cuando el valor falla y no tiene letra.
7. Compatibilidad 0.23 de `INTd` (2.2). `INTd:79-102` encadena `if`/`else if` con dos `if` sueltos; el efecto es
   idéntico a un `switch` por `tipo`.
8. Letras **sensibles a mayúsculas** frente al `ToUpper` de `LetrasTol` (§0.5).
9. `Analizar` hace `InicializaContadores` dos veces (`INT:417,235`); inocuo.

**`analizarFallos` (`INT:357-407,422-429`)**: para cada contador, en este orden — Glob, Var, 1, X, 2, GlobSeg, VarSeg, 1Seg,
XSeg, 2Seg — añade, si el valor actual **no** está permitido, `"Fallo en <nombre>  (<valor>)#"` y, si su letra ≠ `' '`,
`"!Tolerancia en <nombre>  (<valor>)#"`. Nombres: `número de interrupciones`, `interrupciones de variantes`,
`interrupciones de unos`, `interrupciones de equis`, `interrupciones de doses`, `número de interrupciones seguidas`,
`interrupciones de variantes seguidas`, `interrupciones de unos seguidas`, `interrupciones de equis seguidas`,
`interrupciones de doses seguidas`. Se corta en `#` (§0.7); `nil` si no hay texto.

### 2.8 Trampas Swift específicas

- Traducir el bucle `while (ant == 0)` interno sin reordenar: lee y **consume** la primera celda no nula y, en la
  misma iteración externa, lee la siguiente como `act` (si la columna tiene una sola celda no nula, `act = 0` y
  termina).
- Contadores `byte` en C#: en Swift usar `Int` (un `UInt8` aborta al pasar de 255; irrelevante aquí, pero no
  usar `&+` por reflejo).
- `Char.IsLetter` y `Split` (§0.9-3/5). `Replace("*","")` → `replacingOccurrences`.
- `switch (act)` anidado con `case 1:`/`case 4:`/`default:` → `switch act { case 1…; case 4…; default… }`.

### 2.9 Casos golden propuestos

**(a) Contadores sobre COLS-A:**

| id | Glob | Var | 1 | X | 2 | GlobSeg | VarSeg | 1Seg | XSeg | 2Seg |
|---|---|---|---|---|---|---|---|---|---|---|
| c1 | 12 | 5 | 5 | 4 | 3 | 7 | 2 | 3 | 2 | 1 |
| c2 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| c3 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| c4 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| c5 | 12 | 5 | 5 | 4 | 3 | 7 | 3 | 2 | 2 | 1 |
| c6 | 13 | 3 | 4 | 6 | 3 | 13 | 1 | 1 | 6 | 1 |
| c7 | 9 | 3 | 3 | 3 | 3 | 7 | 1 | 2 | 3 | 2 |
| c8 | 13 | 6 | 7 | 0 | 6 | 13 | 6 | 7 | 0 | 6 |
| c9 | 8 | 2 | 3 | 3 | 2 | 3 | 1 | 1 | 2 | 1 |
| m0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| m1 | 6 | 2 | 2 | 2 | 2 | 6 | 1 | 1 | 1 | 1 |
| m2 | 5 | 1 | 2 | 2 | 1 | 5 | 1 | 1 | 1 | 1 |
| m3 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |
| m4 | 2 | 1 | 1 | 1 | 0 | 2 | 1 | 1 | 1 | 0 |
| m5 | 2 | 1 | 1 | 1 | 0 | 1 | 1 | 1 | 1 | 0 |

**(b) Configuraciones** — «base» = `Glob "12"`, `Var "5"`, `1 "5"`, `X "4"`, `2 "3"`, `GlobSeg "7"`, `VarSeg "2"`,
`1Seg "3"`, `XSeg "2"`, `2Seg "1"` (los valores exactos de `c1`):

| id | configuración | `analizar(c1)` | `analizarFallos(c1)` | `obtenNoAciertos("A","B","AB","AA","")` |
|---|---|---|---|---|
| INT-A | base | `true` | `nil` | 0,0,0,0,0 |
| INT-B | base con `1 "5A"` | `true` | `["!Tolerancia en interrupciones de unos  (5)"]` | 1,0,1,2,0 |
| INT-C | base con `Var "4"` | `false` | `["Fallo en interrupciones de variantes  (5)"]` | — |
| INT-D | base con `Var "4,5B"`, `GlobSeg "6A,7B"` | `true` | `["!Tolerancia en interrupciones de variantes  (5)", "!Tolerancia en número de interrupciones seguidas  (7)"]` | A→0, B→2, AB→2, C→0 |
| INT-E | base con `Var "4"`, `Glob "12A"` | `false` | `["!Tolerancia en número de interrupciones  (12)", "Fallo en interrupciones de variantes  (5)"]` | A→1 |

Más: columnas `m1` (huecos) y `m4` de COLS-A con la configuración «todo permitido» (`LlenarTodosValores`) para
fijar los contadores; y la versión `String` de `analizaColumna` sobre las 9 columnas `c*` (debe coincidir con
la de `long`) y con `"1*X*2*1*X*2*1*"` (el `*` se elimina).

**(c) Errores:** `SetNoInt1("")` → `IndexOutOfRange`; `SetNoInt1("3AB")` → `FormatException`; `SetNoInt1("15")`
(N=14) → `IndexOutOfRange`; XML sin secciones `*Seg` → Seg `0…14` activadas.

### 2.10 Evidencia
`INT:147-231` (string), `233-337` (long), `338-429`, `436-488`, `493-1055`, `1066-1133`; datos `INTd:35-134`;
consumidores `CAI:162-176`, `BOL:275-276`, `ACV:253`.

---

## 3. `FiltroContactos` — `Free1X2.Domain/MotorCalculo/FiltroContactos.cs`

### 3.1 Propósito y definición matemática

Cuenta **pares de partidos consecutivos** (tras descartar huecos) por tipo de pareja, sin distinguir el orden
del par, y deriva cuatro agregados con «variante» (V = X o 2).

Sea `s_0…s_{m-1}` la secuencia de celdas no nulas; para cada `j` el par `{s_j, s_{j+1}}` se clasifica por
`s_j | s_{j+1}` (OR de códigos) (`CON:174-194`):

| OR | Par | Contador |
|---|---|---|
| 6 | 1 con X | `Num1X` |
| 5 | 1 con 2 | `Num12` |
| 3 | X con 2 | `NumX2` |
| 4 | 1 con 1 | `Num11` |
| 2 | X con X | `NumXX` |
| 1 | 2 con 2 | `Num22` |
| otro (7…) | — | no cuenta (solo con celdas multi-signo) |

Derivados (`CON:198-201`): `Num1V = Num1X + Num12`; `NumVV = NumXX + NumX2 + Num22`; `Num2V = Num22 + NumX2`;
`NumXV = NumXX + NumX2`. (`Num2V` y `NumXV` cuentan los pares XX/22 **una vez**; `X2` cuenta en ambos.)

**Ejemplo trabajado** — `c1 = 1X21X1121X1X21`: pares (1X)(X2)(21)(1X)(X1)(11)(12)(21)(1X)(X1)(1X)(X2)(21) →
`1X=6` (pasos 1,4,5,9,10,11), `X2=2` (2,12), `12=4` (3,7,8,13), `11=1` (6), `XX=0`, `22=0` (suma 13 = N−1);
`1V=6+4=10`, `VV=0+2+0=2`, `2V=0+2=2`, `XV=0+2=2`.
**Figura** (3.4): valores `[1X,12,X2,11,XX,22,1V,XV,2V,VV] = [6,4,2,1,0,0,10,2,2,2]` → positivos en orden
descendente `10,6,4,2,2,2,2,1` → nibbles → `0xA6422221` = **2789351969**, texto `10-6-4-2-2-2-2-1`.

### 3.2 Tipos y datos

| Campo C# | Swift | Rango / defecto | Evidencia |
|---|---|---|---|
| `Num1X, Num12, NumX2, Num11, NumXX, Num22, Num1V, NumXV, Num2V, NumVV` | `Int32` | 0…N−1 | `CON:33-42` |
| `cont1X, cont12, contX2, cont11, contXX, cont22, cont1V, contXV, cont2V, contVV` | `[Bool]` (N+1) | permitidos; defecto `false` | `CON:47-58,79-89` |
| `cont…Tol` (10) | `[Character]` (N+1) | **defecto `'\0'`** tras el ctor (ver C4); `' '` tras `Inicializa()` / `ReinicializaValores()` | `CON:61-72,91-101,110-138,381-409` |
| `figuras` | `[Int64]?` | `nil` hasta que el cargador la asigne; `UsaFiguras()` ⇔ no nula y no vacía | `CON:29,1065-1084` |
| `figurasLong` | — | declarada y **sin uso**; no portar | `CON:30,1085-1095` |

Configuración (`CONd:57-117`): tipos XML `1X,12,X2,11,XX,22,1V,XV,2V,VV` → `SetNum1X/SetNum12/SetNumX2/SetNum11/SetNumXX/SetNum22/SetNum1V/SetNumXV/SetNum2V/SetNumVV`
(valores con letra, §0.5); sección `figuras` (`CONd:118-137`). `LlenarTodosValores` rellena **`0…N−1`** (no N) los
conjuntos vacíos (`CON:922-941`). `GetNum*` según §0.5 (`CON:541-899`). `Inicializa()` = limpiar matrices +
contadores (`CON:104-138`); lo llama la UI al guardar (`CONvm:148`).

### 3.3 Entradas / salidas

`analizar(Int64) -> Bool`; `analizaColumna(Int64)` público; 10 getters `NoContactos11 … NoContactosVV`
(`CON:1020-1059`); `obtenerFiguraLongValor() -> Int64` (público, sin asignaciones) y `obtenerFiguraLong()`/
`figuraContactos` que lo envuelven (`CON:966-971,982-1018,1097-1100`); `analizarFallos`; `obtenNoAciertosTolerancias`.

### 3.4 Algoritmo

```
analizaColumna(col):                                                   // CON:155-202
  reset 10 contadores
  // 1) compactar quitando huecos Y reordenando: lee de partido 1 a N, empuja por la derecha
  sin = 0;  c = col
  mientras c != 0: s = c & 7; si s != 0 { sin = (sin << 3) | s };  c >>= 3
  //    → `sin` contiene las celdas no nulas en orden INVERSO (partido más alto en el nibble bajo)
  signo1 = sin & 7
  mientras sin != 0:
     sin >>= 3;  signo2 = sin & 7
     si signo2 > 0:
        switch (signo1 | signo2): 6→Num1X++ ; 5→Num12++ ; 3→NumX2++ ; 4→Num11++ ; 2→NumXX++ ; 1→Num22++ ; default: nada
        signo1 = signo2
  Num1V = Num1X + Num12;  NumVV = NumXX + NumX2 + Num22;  Num2V = Num22 + NumX2;  NumXV = NumXX + NumX2

esColumnaValida() = cont1X[Num1X] ∧ cont12[Num12] ∧ contX2[NumX2] ∧ cont11[Num11] ∧ contXX[NumXX]
                  ∧ cont22[Num22] ∧ cont1V[Num1V] ∧ contXV[NumXV] ∧ cont2V[Num2V] ∧ contVV[NumVV]   // CON:204-223
analizar(col):                                                         // CON:307-312, 282-297
  reset; analizaColumna(col)
  si !esColumnaValida() return false
  si usaFiguras() return figuras.contains(obtenerFiguraLongValor())   // CON:299-304
  return true

obtenerFiguraLongValor():                                              // CON:982-1018
  v = [Num1X, Num12, NumX2, Num11, NumXX, Num22, Num1V, NumXV, Num2V, NumVV]
  ordenar v ascendente
  t = 0;  para i desde 9 hasta 0: si v[i] > 0 { t = (t << 4) | v[i] }   // el mayor queda en el nibble alto
  return t                                                            // todos 0 → 0
```

El orden inverso del paso 1 no altera los contadores: la clasificación usa OR (conmutativa) y se recorren los
mismos pares consecutivos. Forma simplificada equivalente: iterar las celdas no nulas en orden de partido y
clasificar cada par adyacente por `a | b`.

### 3.5 Interacción con tolerancias / filtro parcial
Letras por valor (§0.5): hasta 10 aciertos por letra (`CON:327-376`). `Analizar` las ignora. Máscara de grupo:
el filtro ve la columna con huecos y los **compacta** (contacta partidos separados por huecos, §3.7-C1).

### 3.6 Estado global
`NumeroPartidos` (ctor y `LlenarTodosValores`), `AnalizarContactos`. `AnalizarFigurasContactos` lo lee el
contenedor (`CAI:138`).

### 3.7 Peculiaridades a preservar

- **C1.** Los huecos no separan: «1 * X» cuenta como contacto 1X (compactación, `CON:160-166`).
- **C2.** Agregados con doble conteo intencionado (3.1): p. ej. `X2` suma en `Num2V`, `NumXV` y `NumVV`.
- **C3.** Figura: nibbles de los valores **> 0** en orden **descendente**; los ceros desaparecen; sin atajo de
  «todas las figuras» (a diferencia de Pesos, `CON:290-293,299-304`). Un valor ≥ 16 solaparía nibbles (solo N > 16).
- **C4.** **El constructor no inicializa los `char[]` de tolerancia** (quedan `'\0'`); solo `Inicializa()` y
  `ReinicializaValores()` los dejan en `' '` (`CON:74-102` frente a `110-138,381-409`). En el flujo «cargar XML»
  (`CONd`) nadie los llama (grep: solo `CONvm:148` y `Free1X2/UI/Filtros/ContactosFrm.cs:529`). Efecto
  observable solo en `analizarFallos`: para un valor **no permitido y nunca asignado**, `tol == '\0' ≠ ' '` y se
  añade un `!Tolerancia…` espurio tras el `Fallo…` (caso CT-B). `Analizar` y `obtenNoAciertosTolerancias` no se
  ven afectados (una letra real nunca es `'\0'`). Preservar o documentar la decisión.
- **C5.** `LlenarTodosValores` usa `0…N−1` mientras Interrupciones/Seguidos usan `0…N`.
- **C6.** `ObtenerFiguraLongB` (marcador `1<<…`) es código muerto: no portar (`CON:951-965`).
- **C7.** Si el XML no trae `figuras`, `figuras` queda `nil`; si trae la sección vacía, `[]` → ambos ⇒ `UsaFiguras()=false`.
- **C8.** `analizarFallos` no es `nil` aunque la columna sea válida si algún valor actual lleva letra (`!Tolerancia…`).
- **C9.** `CONd:73-114` mezcla `if`/`else if` sueltos; equivalente a `switch` por `tipo`.

**`analizarFallos` (`CON:225-280,314-320`)** — orden **11, 1X, 12, 1V, XX, X2, XV, 22, 2V, VV**; por cada uno:
si no permitido `"Fallo en contactos <par>  (<n>)#"`, y si `tol ≠ ' '` `"!Tolerancia en contactos <par>  (<n>)#"`.
Al final, si `UsaFiguras()` y la figura no está: `"Fallo en Figuras  (<texto de figura>)#"` **en la misma lista**
(no hay array aparte). No llama a `InicializaContadores` fuera de `analizaColumna` (ya lo hace).

### 3.8 Trampas Swift específicas

- `colSinAsteriscos << 3 | signo1` en C# = `(col << 3) | signo` (el `<<` liga más que `|`); en Swift usar paréntesis.
- Con ≤ 16 celdas no nulas (48 bits) no hay desbordamiento; el `<<` de Swift no aborta de todos modos.
- `switch (signo1 | signo2)` sobre `UInt8` promovido a `int`: en Swift `Int`, con `default: break`.
- Ordenar 10 enteros: cualquier ordenación estable o no (enteros indistinguibles, T5); el C# usa inserción.
- `(uint)valores[i]` → conversión explícita a `Int64`, valores 0…15.
- `char[]` con `'\0'` (C4): el centinela `" "` solo después de `inicializa()`.

### 3.9 Casos golden propuestos

**(a) Contadores y figura sobre COLS-A:**

| id | 1X | 12 | X2 | 11 | XX | 22 | 1V | XV | 2V | VV | figura (long / texto) |
|---|---|---|---|---|---|---|---|---|---|---|---|
| c1 | 6 | 4 | 2 | 1 | 0 | 0 | 10 | 2 | 2 | 2 | 2789351969 (`10-6-4-2-2-2-2-1`) |
| c2 | 0 | 0 | 0 | 13 | 0 | 0 | 0 | 0 | 0 | 0 | 13 (`13`) |
| c3 | 0 | 0 | 0 | 0 | 13 | 0 | 0 | 13 | 0 | 13 | 3549 (`13-13-13`) |
| c4 | 0 | 0 | 0 | 0 | 0 | 13 | 0 | 0 | 13 | 13 | 3549 (`13-13-13`) |
| c5 | 6 | 4 | 2 | 1 | 0 | 0 | 10 | 2 | 2 | 2 | 2789351969 (`10-6-4-2-2-2-2-1`) |
| c6 | 7 | 0 | 6 | 0 | 0 | 0 | 7 | 6 | 6 | 6 | 7824998 (`7-7-6-6-6-6`) |
| c7 | 3 | 3 | 3 | 3 | 0 | 1 | 6 | 3 | 4 | 4 | 26914009905 (`6-4-4-3-3-3-3-3-1`) |
| c8 | 0 | 13 | 0 | 0 | 0 | 0 | 13 | 0 | 0 | 0 | 221 (`13-13`) |
| c9 | 3 | 2 | 3 | 2 | 2 | 1 | 5 | 5 | 4 | 6 | 435204334113 (`6-5-5-4-3-3-2-2-2-1`) |
| m0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 (`0`) |
| m1 | 2 | 2 | 2 | 0 | 0 | 0 | 4 | 2 | 2 | 2 | 69345826 (`4-2-2-2-2-2-2`) |
| m2 | 2 | 1 | 2 | 0 | 0 | 0 | 3 | 2 | 2 | 2 | 52568609 (`3-2-2-2-2-2-1`) |
| m3 | 0 | 0 | 0 | 6 | 0 | 0 | 0 | 0 | 0 | 0 | 6 (`6`) |
| m4 | 1 | 1 | 0 | 0 | 2 | 2 | 2 | 2 | 2 | 4 | 1109533201 (`4-2-2-2-2-2-1-1`) |
| m5 | 2 | 0 | 0 | 3 | 2 | 0 | 2 | 2 | 0 | 2 | 3285538 (`3-2-2-2-2-2`) |

**(b) Configuraciones** — «base» = `1X "6"`, `12 "4"`, `X2 "2"`, `11 "1"`, `XX "0"`, `22 "0"`, `1V "10"`, `XV "2"`, `2V "2"`, `VV "2"`
(valores exactos de `c1`); filtro **recién construido** (sin `Inicializa`) salvo indicación:

| id | configuración | `analizar(c1)` | `analizarFallos(c1)` |
|---|---|---|---|
| CT-A | base | `true` | `nil` |
| CT-B | base con `11 "2"` (recién construido) | `false` | `["Fallo en contactos 11  (1)", "!Tolerancia en contactos 11  (1)"]` |
| CT-B′ | CT-B tras llamar a `Inicializa()` y repoblar | `false` | `["Fallo en contactos 11  (1)"]` |
| CT-C | base + figuras {2789351969} | `true` | `nil` |
| CT-D | base + figuras {13} | `false` | `["Fallo en Figuras  (10-6-4-2-2-2-2-1)"]` |
| CT-E | base con `1X "6A"` | `true` | `["!Tolerancia en contactos 1X  (6)"]` |

`obtenNoAciertos("A")` en CT-E = 1. Más: `m0` (columna 0, todo 0, figura 0) con `LlenarTodosValores` → `true`;
`m3` (siete «1» aislados por huecos → `11 = 6`).

**(c) Errores:** `SetNum11("")` → `IndexOutOfRange`; `SetNum11("14")` con N = 14 es válido (el array mide 15) aunque
inalcanzable (máximo 13 contactos); `SetNum11("15")` → `IndexOutOfRange`.

### 3.10 Evidencia
`CON:74-202,204-376,381-1018,1020-1104`; datos `CONd:35-137`; consumidor `CAI:124-154`; UI `CONvm:148`.

---

## 4. `FiltroSignosSeguidos` — `Free1X2.Domain/MotorCalculo/FiltroSignosSeguidos.cs`

### 4.1 Propósito y definición matemática

Mide la **racha más larga** de cada clase de signo y, opcionalmente, la «figura» de rachas (multiconjunto de
longitudes de **todas** las rachas):

- `V` (variantes): racha máxima de signos consecutivos distintos de «1» (X o 2, mezclados).
- `1`, `X`, `2`: racha máxima de ese signo.

Rachas (`SEG:150-206`): se recorre de partido 1 a N y, para cada celda: `4` → `r1++; rV=rX=r2=0`;
`2` → `rX++; rV++; r1=r2=0`; `1` → `r2++; rV++; r1=rX=0`; cualquier otro valor (incl. **0**) **no cambia ninguna
racha**. Tras cada celda, `noX = máx(noX, rX)` para las 4 clases.

Figura de una clase (`SEG:226-275`): se aplica la máscara de la clase (constantes: V = `3333…`₈ = 120632132875995,
1 = `4444…`₈ = 160842843834660, X = `2222…`₈ = 80421421917330, 2 = `1111…`₈ = 40210710958665, 16 celdas),
se recorren las celdas: una celda ≠ 0 alarga la racha en curso; una celda = 0 **la cierra** (si `> 0`, se guarda
su longitud). Se ordenan las longitudes ascendentes y se empaquetan **de mayor a menor** en nibbles
(`t = (t<<4)|len` desde la mayor). Sin rachas → 0.

**Ejemplo trabajado** — `c1 = 1X21X1121X1X21`:
- Rachas de «1»: posiciones 1 | 4 | 6-7 | 9 | 11 | 14 → longitudes 1,1,2,1,1,1 → **`1`=2**; figura `2-1-1-1-1-1` (2167057).
- Rachas de X: 2,5,10,12 → todas 1 → **`X`=1**; figura `1-1-1-1` (4369).
- Rachas de «2»: 3, 8, 13 → **`2`=1**; figura `1-1-1` (273).
- Rachas de variantes (X o 2): (2,3) (5) (8) (10) (12,13) → 2,1,1,1,2 → **`V`=2**; figura `2-2-1-1-1` (139537).

### 4.2 Tipos y datos

| Campo C# | Swift | Rango / defecto | Evidencia |
|---|---|---|---|
| `noVarSeguidosCol, noUnosSeguidosCol, noEquisSeguidosCol, noDosesSeguidosCol` | `Int32` | 0…N | `SEG:19-22` |
| `variantes, unos, equis, doses` | `[Bool]` (N+1) | permitidos; defecto `false` | `SEG:27-30,43-46` |
| `variantesTol, unosTol, equisTol, dosesTol` | `[Character]` (N+1) | defecto `' '` | `SEG:32-35,48-65` |
| `figurasV, figuras1, figurasX, figuras2` | `[Int64]` | **no nulas**, defecto `[]` | `SEG:12-15` |
| `figuras` (la de `IFiltro`) | `[Int64]?` | `nil`, sin uso | `SEG:11,695-705` |
| `bufFigura int[16]` | local `[Int32]`×16 | rachas ≤ 8 con N ≤ 16 | `SEG:226` |

Configuración (`SEGd:63-94,96-164`): tipos `var,unos,equis,doses` → `SetNoVariantes/SetNoUnos/SetNoEquis/SetNoDoses` (valores con
letra, §0.5); secciones `figurasVariantes/figurasUnos/figurasEquis/figurasDoses` → `FigurasV/Figuras1/FigurasX/Figuras2`.
`LlenarTodosValores` rellena `0…N` los conjuntos vacíos (`SEG:619-632`). `UsaFiguras()` ⇔ **alguna** de las 4
listas no vacía (`SEG:690-693`).

### 4.3 Entradas / salidas
`analizar(Int64) -> Bool`; getters `NoVariantesSeguidas/NoUnosSeguidos/NoEquisSeguidas/NoDosesSeguidos`
(`SEG:649-663`); `obtenFiguraV/1/X/2(Int64) -> Int64` **públicos**, que reciben **la columna** y no usan el estado
(`SEG:276-295`; el contenedor los llama con la columna completa, `CAI:206-253`); `analizarFallos`;
`obtenNoAciertosTolerancias`.

### 4.4 Algoritmo

```
analizar(col):                                                         // SEG:209-214, 140-147
  reset 4 contadores; analizaColumna(col)                              // reglas de racha de 4.1
  si !(variantes[V] ∧ unos[U] ∧ equis[X] ∧ doses[D]) return false      // SEG:77-80
  return analizaFiguras(col)

analizaFiguras(col):                                                   // SEG:387-394
  si !usaFiguras() return true
  return analizaFigurasV(col) ∧ analizaFiguras1(col) ∧ analizaFigurasX(col) ∧ analizaFiguras2(col)
analizaFigurasK(col) = lista_K.vacía ? true : lista_K.contains(obtenFiguraK(col))     // SEG:351-385

obtenFiguraConMascara(col, mascara):                                   // SEG:228-275
  c = col & mascara;  vals = [];  cuantas = 0
  mientras c != 0:
     si (c & 7) != 0: cuantas += 1
     si no: si cuantas > 0 { vals.añade(cuantas); cuantas = 0 }
     c >>= 3
  si cuantas > 0 { vals.añade(cuantas) }
  ordenar vals ascendente;  t = 0
  para i desde vals.count-1 hasta 0: t = (t << 4) | vals[i]
  return t
```
`obtenFiguraV = …(col, 120632132875995)`, `Figura1 = …(col, 160842843834660)`, `FiguraX = …(col, 80421421917330)`,
`Figura2 = …(col, 40210710958665)` (`SEG:276-295`).

### 4.5 Interacción con tolerancias / filtro parcial
Letras por valor (§0.5): hasta 4 aciertos por letra (`SEG:319-347`). Las figuras no tienen tolerancia. Máscara
de grupo: la racha ignora huecos pero la figura los usa como separadores (4.7-S2).

### 4.6 Estado global
`NumeroPartidos` (ctor y `LlenarTodosValores`), `AnalizarSeguidos`. `AnalizarFigurasV1X2` lo lee el contenedor (`CAI:206`).

### 4.7 Peculiaridades a preservar

- **S1.** «V» = racha de **no-1** mezclando X y 2 (`SEG:168-181`).
- **S2.** **Inconsistencia hueco**: en `analizaColumna` las celdas 0/multi-signo son transparentes (la racha continúa
  a través de ellas); en `obtenFigura*` una celda 0 **corta** la racha. Con `m3 = *1*1*1*1*1*1*1`: `1`=7 pero figura de unos
  = `1-1-1-1-1-1-1`. Con columnas completas coinciden.
- **S3.** Cuatro listas de figuras **independientes**; lista vacía = sin restricción; basta que una esté
  no vacía para `UsaFiguras()`. Una clase sin ninguna racha da figura `0` y solo pasa si la lista contiene `0`
  (texto `"0"`).
- **S4.** `figuras` (de `IFiltro`) no se usa; `Figuras` devuelve `nil`.
- **S5.** `Tolerancia  en variantes seguidas` lleva **dos espacios** tras «Tolerancia» (`SEG:88`); los otros tres no.
- **S6.** `AnalizarFallos` evalúa las 4 figuras siempre (vacías → `true`) y no depende de `UsaFiguras()`.
- **S7.** Longitud de racha ≥ 16 (solo N ≥ 16 y una clase completa) desbordaría el nibble; ignorado (A2).
- **S8.** `analizarFallos` no es `nil` aunque la columna sea válida si algún valor actual lleva letra.

**`analizarFallos` (`SEG:82-138,298-305`)** — orden: variantes, unos, equis, doses (cada uno: `"Fallo en …"` y/o `"!Tolerancia …"`)
con los textos `variantes seguidas` / `unos seguidos` / `equis seguidas` / `doses seguidos` (`"Fallo en variantes seguidas  (n)"`,
`"!Tolerancia  en variantes seguidas  (n)"`, `"Fallo en unos seguidos  (n)"`, `"!Tolerancia en unos seguidos  (n)"`, …);
después, por este orden V, 1, X, 2, si la figura no está permitida: `"Fallo en Figuras de Variantes|Unos|Equis|Doses  (<texto>)#"`
(texto `"0"` si la figura es 0).

### 4.8 Trampas Swift específicas

- Racha con `switch (columna & 7)` sin `default` en C# (`SEG:159-182`): `default: break` y **no** salir del bucle
  (las celdas ignoradas siguen actualizando los máximos con los valores vivos).
- El `max` se actualiza **tras cada celda**, incluidas las ignoradas (`SEG:184-204`); equivale a máximo global.
- Las máscaras son constantes de 48 bits: declararlas como `Int64` (no `Int32`).
- Insertion sort de ≤ 8 elementos: cualquier orden ascendente (T5). `(uint)valores[i]` → `Int64(valor)`.

### 4.9 Casos golden propuestos

**(a) Rachas y figuras sobre COLS-A:**

| id | V | 1 | X | 2 | figV | fig1 | figX | fig2 |
|---|---|---|---|---|---|---|---|---|
| c1 | 2 | 2 | 1 | 1 | 139537 (`2-2-1-1-1`) | 2167057 (`2-1-1-1-1-1`) | 4369 (`1-1-1-1`) | 273 (`1-1-1`) |
| c2 | 0 | 14 | 0 | 0 | 0 (`0`) | 14 (`14`) | 0 (`0`) | 0 (`0`) |
| c3 | 14 | 0 | 14 | 0 | 14 (`14`) | 0 (`0`) | 14 (`14`) | 0 (`0`) |
| c4 | 14 | 0 | 0 | 14 | 14 (`14`) | 0 (`0`) | 0 (`0`) | 14 (`14`) |
| c5 | 2 | 2 | 1 | 1 | 139537 (`2-2-1-1-1`) | 2167057 (`2-1-1-1-1-1`) | 4369 (`1-1-1-1`) | 273 (`1-1-1`) |
| c6 | 3 | 1 | 1 | 1 | 13105 (`3-3-3-1`) | 4369 (`1-1-1-1`) | 17895697 (`1-1-1-1-1-1-1`) | 273 (`1-1-1`) |
| c7 | 4 | 3 | 1 | 2 | 1057 (`4-2-1`) | 12817 (`3-2-1-1`) | 273 (`1-1-1`) | 529 (`2-1-1`) |
| c8 | 1 | 1 | 0 | 1 | 17895697 (`1-1-1-1-1-1-1`) | 17895697 (`1-1-1-1-1-1-1`) | 0 (`0`) | 17895697 (`1-1-1-1-1-1-1`) |
| c9 | 4 | 3 | 2 | 2 | 1074 (`4-3-2`) | 785 (`3-1-1`) | 545 (`2-2-1`) | 529 (`2-1-1`) |
| m0 | 0 | 0 | 0 | 0 | 0 (`0`) | 0 (`0`) | 0 (`0`) | 0 (`0`) |
| m1 | 2 | 1 | 1 | 1 | 4369 (`1-1-1-1`) | 273 (`1-1-1`) | 17 (`1-1`) | 17 (`1-1`) |
| m2 | 2 | 1 | 1 | 1 | 34 (`2-2`) | 17 (`1-1`) | 17 (`1-1`) | 17 (`1-1`) |
| m3 | 0 | 7 | 0 | 0 | 0 (`0`) | 17895697 (`1-1-1-1-1-1-1`) | 0 (`0`) | 0 (`0`) |
| m4 | 3 | 1 | 3 | 3 | 1118481 (`1-1-1-1-1-1`) | 1 (`1`) | 273 (`1-1-1`) | 273 (`1-1-1`) |
| m5 | 3 | 4 | 3 | 0 | 273 (`1-1-1`) | 69905 (`1-1-1-1-1`) | 273 (`1-1-1`) | 0 (`0`) |

**(b) Configuraciones** — «base» = `Var "2"`, `1 "2"`, `X "1"`, `2 "1"`:

| id | configuración | `analizar(c1)` | `analizarFallos(c1)` |
|---|---|---|---|
| SS-A | base | `true` | `nil` |
| SS-B | base con `2 "0"` | `false` | `["Fallo en doses seguidos  (1)"]` |
| SS-C | base con `1 "2A"`, `2 "1,2B"` | `true` | `["!Tolerancia en unos seguidos  (2)"]` |
| SS-D | base + `FigurasV={139537}`, `Figuras1={2167057}` | `true` | `nil` |
| SS-E | base + `FigurasV={34}`, `Figuras2={17}` | `false` | `["Fallo en Figuras de Variantes  (2-2-1-1-1)", "Fallo en Figuras de Doses  (1-1-1)"]` |

`obtenNoAciertos("A")` en SS-C = 1; `obtenNoAciertos("B")` = 0 (el valor actual de «2» es 1, sin letra).
Más: `m3` con `LlenarTodosValores` para fijar S2; `obtenFiguraV/1/X/2` aisladas sobre `m1`, `m2`, `m4`.

**(c) Errores:** `SetNoUnos("")` → `IndexOutOfRange`; `SetNoUnos("15")` (N=14) → `IndexOutOfRange`.

### 4.10 Evidencia
`SEG:19-80,82-147,150-206,209-214,226-305,319-347,351-394,397-717`; datos `SEGd:32-298`; consumidor `CAI:200-254`.

---

## 5. Abiertas

| # | Pregunta | Por qué no se cierra aquí |
|---|----------|---------------------------|
| A1 | ¿Pasa alguna vez el motor celdas **multi-signo** (3,5,6,7) a `Analizar` de estos filtros (p. ej. análisis de pronósticos)? Los `switch` las ignoran (Pesos, Seguidos), las tratan como X (Interrupciones, rama `default`) o las dejan fuera de la tabla de OR (Contactos, OR=7). | Habría que leer `GeneradorColumnas`/`Analizador` (ESPEC_01). |
| A2 | Rango real de `NumeroPartidos`: `Free1X2.Domain/EntradaSalida/AConfiguracion.cs:149` lo lee como `Int16` sin validar. Con N < 9 Pesos lanza (índices 0…9); con N < 14 el fallback 0.23 de `INTd` lanza; con N > 16 `IU/IE/ID` (16) y los nibbles (≤ 15) se desbordan. | No se ha verificado qué valores admite la UI. Hipótesis de trabajo: 14 (también 15 con Pleno). |
| A3 | **If-Then con «Peso numérico» siempre `false`** (§1.7-3): deducido por lectura; confirmar con un golden del `ControladorIfThen` y decidir si es defecto a preservar. | Pertenece a ESPEC_01 (If-Then). |
| A4 | Un filtro con un conjunto vacío se serializa con `values=""` y no puede releerse (§0.5): ¿lo evita la UI o el cargador captura la excepción? | Formato/carga en ESPEC_07; aquí solo el comportamiento del setter. |
| A5 | Qué letras de tolerancia genera la UI (¿solo A–Z?) y si alguna vez guarda minúsculas (sin efecto por `ToUpper`, `TOL:151`). `ToUpper()` usa la **cultura actual** (p. ej. turco: `i` → `İ`). | `ToleranciaFiltros` es de ESPEC_01; fijar `Locale` invariante allí. |
| A6 | Equivalencia exacta `Character.isLetter` (Swift) ↔ `Char.IsLetter` (C#) para letras no ASCII. | Se propone usar `generalCategory`; sin prueba empírica. |
| A7 | Máscara efectiva del **grupo base** en modo análisis: `GRP:39` inicializa `mascara = 7` y `GRP:232-233` enmascara siempre; depende de que `Partidos` (`GRP:555-562`) se haya asignado. | Pertenece a ESPEC_01. |
| A8 | Comportamiento de `analizaColumna(String)` de Interrupciones con caracteres ≠ `1/X/2/*` (p. ej. minúsculas): cae en la rama «X». No se ha leído qué envía `InterrupcionesFrm`/`ControlBoleto`. | UI fuera de alcance. |
| A9 | Figuras con valores ≥ 16 en el texto (no validado en `UEV:220-231`): ¿puede producirlas la UI? | UI fuera de alcance. |
| A10 | `Pesos`: atajo `figuras.Count == 5` con duplicados (¿lo impide la UI?). | UI fuera de alcance. |
| A11 | `PesosNumFrmViewModel.ToleranciaValores()`: ¿puede el usuario dejar fuera el 0 de `noTolerancias` (lo que haría rechazar todas las columnas, §1.7-1)? | UI fuera de alcance. |
