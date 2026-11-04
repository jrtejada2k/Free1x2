# Grupo E de filtros: plan de pruebas en la app

Pantallas: **Rentabilidad, Columnas probables (CP parcial), Control de Grupos, Signos Seguidos con figuras, Simetrías
(en espejo) y Variantes X y 2 (solo doses)**.
Oráculo: `scripts/pruebas_filtros_e.py`. Es independiente: programa cada regla con cadenas y posiciones, y
`emulacion_motor()` reprograma la lógica del motor C# (máscaras de bits y, en Rentabilidad, la aritmética **float32**
del recorrido `EncontrarDistantes1`) y comprueba en las 216 columnas de B3, más 3.000 al azar, que las dos dan lo mismo.

```
python -I scripts/pruebas_filtros_e.py                      # asserts + clave y nº de columnas esperadas (+ sensibilidad)
python -I scripts/pruebas_filtros_e.py preparar C:\Pruebas  # escribe C:\Pruebas\entrada\FE_B3.txt, FE1_apostados.txt, FE1_reales.txt
python -I scripts/pruebas_filtros_e.py verificar C:\Pruebas # compara C:\Pruebas\salida\FE*.txt
```

## 0. Preparación y reglas comunes

- **Boleto B3** (docs/PRUEBAS_WINDOWS.md:66-67): P1 `1X2` · P2 `1` · P3 `X2` · P4 `1X2` · P5 `1` · P6 `1X` · P7 `X` ·
  P8 `1X2` · P9 `12` · P10 `1` · P11 `2` · P12 `1` · P13 `X` · P14 `1`. Cabecera «Fijos: 8 - Dobles: 3 - Triples: 3»;
  sin condiciones, Calcular da Procesadas **216**. Para no teclearlo: B3 ▸ «Máscaras ▾» ▸ Guardar como `B3`; antes de
  cada prueba, **Nueva** ▸ «Máscaras ▾» ▸ `B3` (Nueva crea un `Analizador` nuevo: borra condiciones, grupos y controles).
- **Abrir cada condición desde su tarjeta de Inicio**, no desde el panel lateral: solo las tarjetas fijan el grupo que
  se edita (`AppState.GrupoEnEdicion`, `MainPageViewModel.cs:296,304,312,326`).
- **Pulsa Tab antes de «Aceptar»/«OK»** en Variantes, Signos Seguidos, Interrupciones, Simetrías y Control de Grupos
  (cajas `x:Bind … TwoWay` sin `UpdateSourceTrigger`, p. ej. `ControlGruposFrmPage.xaml:45,52`) y en las NumberBox de
  Rentabilidad. En Columnas probables y en el editor de figuras no hace falta (`PropertyChanged`).
- **Formato de los campos:** Variantes, Signos Seguidos e Interrupciones solo admiten **números con comas** (`4,5,6,7`),
  sin rangos (issue #21). Aciertos de Simetrías, Aciertos/Fallos Seguidos de CP y Fallos de Control de Grupos sí
  admiten `a-b`.
- **Calcular y grabar** (docs/PRUEBAS_WINDOWS.md:51-52): **Calcular** en la barra de Inicio ▸ «Grabar archivo» ▸
  «Seleccionar archivo...» ▸ `salida\<CLAVE>.txt` ▸ **Calcular**. Anota «Procesadas» y «Aceptadas».
- **Configuración**: comprueba antes en Configuración ▸ Parámetros LAE que siguen los de fábrica, **0,75 € y 12 %**
  (`Free1X2.WinUI/Datos/parametros.free1x2:10`), y que la jornada cargada no es de México (si lo es, Rentabilidad usa
  los del Progol, `ParametrosLAEActuales.cs:48`).

## 1. Resumen

| Clave | Pantalla (desde) | Tipo | Debe salir (de 216) | Qué prueba |
|---|---|---|---|---|
| **FE1** | Rentabilidad (panel ▸ Utilidades) | herramienta | **82** (Finalizado (82 columnas)) | EM entre 1 y 2 sobre `FE_B3.txt`, con tope del bote |
| **FE2** | Columnas probables (tarjeta) | condición | **94** | CP con 5 partidos marcados, Aciertos `3-5` |
| **FE2b** | Columnas probables | condición | **72** | FE2 + Fallos Seguidos `0,1` (los partidos sin signo no cortan la racha) |
| **FE3** | Control de Grupos (botón bajo el boleto) | condición | **82** | 2 grupos, Fallos `1` = se cumple exactamente uno |
| **FE3b** | Control de Grupos | condición | **176** | Fallos `0,1` = al menos uno |
| **FE3x** | Inicio ▸ «Grupo siguiente» (sonda) | condición | **82**. ⚠ el código predice **0** (G-1) | grupos creados desde Inicio |
| **FE4** | Signos Seguidos (tarjeta) ▸ Figuras Variantes | condición | **52** | 2 figuras de variantes + «1» `1,2` |
| **FE5** | Simetrías (tarjeta) | condición | **174** | 7 parejas en espejo (1,8)…(7,14), Aciertos `0-3` |
| **FE6** | Variantes X y 2 (tarjeta) | condición | **148** | solo «2» = `1,2,3` (Var y X vacíos) |
| **FE6b** | Variantes X y 2 | condición | **148** | igual con Var = `0,1,…,14` escrito |
| P1-a | Rentabilidad ▸ Calcular valoración de una columna | pantalla | premio ≈ 1.755.829,90 · EM ≈ 1,7289 | premio mostrado sin tope |

Ficheros de entrada: `FE_B3.txt` (las 216 columnas de B3, para FE1) y `FE1_apostados.txt` / `FE1_reales.txt` (tablas
para «Pegar desde Excel»). Orden recomendado: FE1, FE6, FE6b, FE5, FE2, FE2b, FE4, FE3, FE3b, FE3x.

---

## 2. Rentabilidad (FE1)

**Qué hace.** No es una condición de Inicio: lee columnas (14 triples o un fichero), calcula su **esperanza matemática**
y graba las que tienen la EM entre un mínimo y un máximo.
- Manual (`MANUAL_USUARIO.md:552`): EM = Premio × Probabilidad; el premio de 14 sale de los % **apostados** y la
  probabilidad de los % **reales**. La relación premio ⇄ acertantes está en `:540-543`: acertantes = prob. apostada ×
  recaudación ÷ precio, premio = recaudación × %14 ÷ acertantes. Es decir, **premio = precio × %14 ÷ prob. apostada**.
- Motor (en el VM, transcrito del original): `RentabilidadFrmViewModel.cs:217-218` (`PremioDe14 = precio × %14 / 100`,
  `PremioTope = recaudación × %14 / 100`), `:221-222` (porcentajes a tanto por uno con `Porcentajes.ValoresBase100`,
  `Porcentajes.cs:341-352`), `:265-293` (probabilidad de la `11111111111111` y cocientes `Cra`/`Crp`) y
  `EncontrarDistantes1` `:340-371`: `Premio = PremioDe14 / ProbA`, **con tope** `PremioTope` (`:357-358`),
  `Esperanza = Premio × ProbR`. Graba si `mín ≤ EM ≤ máx` (`:381-382`, `:398`), en el orden del índice (sin ordenar).
- Todo va en **float32**. El oráculo calcula la EM en doble y además emula el float32 paso a paso; comprueba que las dos
  clasifican igual y que ninguna columna de B3 está a menos de un 0,1 % de un límite ni del tope.
- Precio y %14 salen de Configuración (`ParametrosLAEActuales.Calculo`, `RentabilidadFrmViewModel.cs:165,214`), pero la **recaudación no**: la
  caja trae 8.000.000 fijo (hallazgo R-1). Por eso en la prueba se escribe a mano.

**Datos** (`FE1_apostados.txt` / `FE1_reales.txt`, % del 1, X y 2):

| Partidos | Apostados | Reales |
|---|---|---|
| 1, 5, 9, 13 | 60 · 25 · 15 | 40 · 30 · 30 |
| 2, 6, 10, 14 | 50 · 30 · 20 | 50 · 30 · 20 |
| 3, 7, 11 | 70 · 20 · 10 | 35 · 35 · 30 |
| 4, 8, 12 | 45 · 30 · 25 | 55 · 25 · 20 |

**FE1. Pasos**
1. `python -I scripts/pruebas_filtros_e.py preparar C:\Pruebas`.
2. Panel lateral ▸ Utilidades ▸ **Rentabilidad** (o menú Utilidades ▸ «Rentabilidad…»).
3. Abre `entrada\FE1_apostados.txt` en el Bloc de notas ▸ Ctrl+A ▸ Ctrl+C ▸ en la rejilla **Apostados** pulsa
   «Pegar desde Excel». Igual con `FE1_reales.txt` en la rejilla **Reales**. Comprueba la fila 3: 70/20/10 y 35/35/30.
4. Origen: **Fichero** ▸ botón de abrir ▸ `entrada\FE_B3.txt`. Salida: botón de guardar ▸ `salida\FE1.txt`.
5. **Recaudación considerada** = `14000000` ▸ Tab. **Mínimo** = `1` ▸ Tab. **Máximo** = `2` ▸ Tab (números enteros a
   propósito: así no depende de si la NumberBox espera coma o punto decimal).
6. «Ordenar columnas por EM» y «Añadir EM al fichero de salida» **apagados** ▸ **Calcular**.

**Debe salir:** estado «Finalizado (82 columnas)» y `FE1.txt` con **82** columnas. 78 de las 216 llegan al tope
(premio de 14 > 1.680.000 €). Sensibilidad: con la recaudación que trae la pantalla (8.000.000) saldrían **60**; sin
tope, 78; con máximo 3, 98.

**P1-a (pantalla).** En «Calcular Valoración de una columna» escribe `11211XX22121X1` ▸ botón calcular. Por el código:
Premio estimado de 14 ≈ **1.755.829,90** (redondeado a 2 decimales, **sin** tope, `:191`), Probabilidad real
≈ **1,029·10⁻⁶** y Esperanza matemática ≈ **1,7289** (esta **con** tope: 1.680.000 × 1,029·10⁻⁶, `:186-188`). Los
últimos decimales pueden variar (float).

---

## 3. Columnas probables parcial (FE2, FE2b)

**Qué hace.** Una CP es un segundo pronóstico **sobre un grupo de partidos**: los partidos sin ningún signo marcado no
forman parte de ella. Se cuentan los aciertos de los partidos marcados (y las rachas de aciertos y fallos entre ellos).
- Manual §7 (`MANUAL_USUARIO.md:358-397`), «segundo pronóstico sobre un grupo de partidos».
- Motor: `ColumnaProbable.cs:271-323`. Solo entra en el bucle un partido con `pronosticos[nr2] != 0`; un partido sin
  signo **ni cuenta ni corta** las rachas ACS/FS. La columna pasa si AC, ACS y FS están en sus listas (`:161-175`).
- Pantalla (desde ec6683e): una fila por partido con **tres botones 1 / X / 2** (`ColProbablesFrmPage.xaml:40-64`;
  `PartidoCPFila.Pronostico` en `ColProbablesFrmViewModel.cs:1800-1812`). Aciertos, Ac. Seguidos y Fallos Seguidos
  admiten `a-b`: se expanden antes del motor (`ExpandeRangos`, `:474-516`); vacío = 0..14 (`:530-539`).

**FE2. Pasos**
1. Nueva ▸ Máscaras ▾ ▸ B3. Tarjeta **Columnas Probables** (columna derecha, `MainPageViewModel.cs:169`), pestaña
   «Columnas». El contador debe leer 1/1 y no debe haber ningún botón encendido (si lo hay: «Borrar Pronósticos»).
2. Enciende **solo** estos botones: partido **1** ▸ `1` · partido **3** ▸ `X` · partido **4** ▸ `X` y `2` ·
   partido **8** ▸ `1` · partido **9** ▸ `2`. Los otros 9 partidos sin ningún botón.
3. **Aciertos** = `3-5`. Ac. Seguidos, Fallos Seguidos, Puntos y Tolerancias vacíos ▸ **Aceptar**. La tarjeta, verde.
4. Calcular ▸ `FE2.txt`.

**Debe salir:** **94**. Reparto de aciertos de la CP en B3: 0 → 8, 1 → 40, 2 → 74, 3 → 64, 4 → 26, 5 → 4. Con
Aciertos `3,4` saldrían 90.

**FE2b.** Reabre la tarjeta (debe mostrar los mismos botones y `3,4,5` en Aciertos) ▸ **Fallos Seguidos** = `0,1` ▸
Aceptar ▸ Calcular ▸ `FE2b.txt` ⇒ **72**. Si los partidos sin signo cortaran la racha de fallos saldrían 84: así se
comprueba que la racha salta del P4 al P8. Después: Nueva.

---

## 4. Control de fallos entre grupos (FE3, FE3b, FE3x)

**Qué hace.** Cada grupo es un conjunto de condiciones. Un **control de grupos** cuenta cuántos de los grupos indicados
**no** se cumplen, y ese número debe estar en «Fallos».
- Manual §6 «Grupos y control de fallos» (`MANUAL_USUARIO.md:353-354`): dos grupos con variantes distintas y «fallos
  entre ambos = 1 → se cumple uno u otro». Ver hallazgo CG-1: con `1` se exige **exactamente** uno.
- Motor: `ControlGrupos.AnalizaGruposConControlFallos` (`ControlGrupos.cs:61-90`) suma los grupos que fallan y mira
  `fallosPermitidos[n]` (`:78`); `PonerFallosPermitidos` (`:199-225`) marca los valores de la lista y activa el
  control. `ControladorGrupos.AnalizaColumnaSinConjuntos` (`ControladorGrupos.cs:84-95`) exige todos los controles: el
  base (grupo 0 y los grupos sueltos, todos obligatorios) y los creados en la pantalla.
- Pantalla: `ControlGruposFrmViewModel.cs`. Al pulsar OK, los grupos que no están en ningún control quedan como
  sueltos en el control base (`GuardarGruposLibres`, `:200-224`); el boleto base (0) siempre lo está.
- **Cómo se crean los grupos.** En Inicio, «Grupo siguiente» crea un `Grupo` vacío (`MainPageViewModel.cs:398-460`)
  **sin partidos activos** y no hay forma de marcarlos (hallazgo G-1). La única ruta de la app que crea grupos con los
  14 partidos es **Columnas probables ▸ Copiar Columnas ▸ Crear grupos** (`CrearGruposFrmViewModel.cs:44-49`,
  `PonerPartidosActivos("1,…,14")`). FE3 usa esa ruta; FE3x, la de Inicio, como sonda.

**Datos.** Grupo 1: Variantes `4,5,6,7` e Interrupciones Global `9,10,11` (cumplen 124 columnas). Grupo 2: Variantes
`6,7,8,9` e Interrupciones Global `8,9,10` (146). Se solapan a propósito (los dos a la vez: 94), para distinguir
«exactamente uno» de «al menos uno».

**FE3. Pasos**
1. Nueva ▸ Máscaras ▾ ▸ B3 (en el boleto base, grupo 0, sin condiciones).
2. Tarjeta **Columnas Probables** ▸ **Copiar Columnas** ▸ **Crear grupos** ▸ «Número de grupos» = `2` ▸ Tab ▸
   **Crear**. Vuelve a Copiar Columnas ▸ **Cancelar** ▸ en Columnas probables, sin marcar nada, **Aceptar** (la
   tarjeta sigue gris). En Inicio, el texto junto a los botones de grupo debe decir **«3 grupos»**.
3. **Grupo siguiente** (▶ bajo el boleto). El título debe ser «Grupo 1 / 2». **No toques el boleto** (ver G-1).
   - Tarjeta **Variantes X y 2** ▸ Borrar ▸ **Var** = `4,5,6,7` ▸ Tab ▸ Aceptar.
   - Tarjeta **Interrupciones** ▸ Borrar ▸ **Global** = `9,10,11` ▸ Tab ▸ Aceptar.
4. **Grupo siguiente** ▸ «Grupo 2 / 2».
   - Variantes ▸ **Var** = `6,7,8,9` ▸ Tab ▸ Aceptar. Interrupciones ▸ **Global** = `8,9,10` ▸ Tab ▸ Aceptar.
5. Botón **Control de Grupos** (bajo el boleto) ▸ sección «Control Grupos»: **Grupos** = `1,2` ▸ Tab ▸ **Fallos** = `1`
   ▸ Tab ▸ **OK**. El semáforo de Control de Grupos, verde (`MainPageViewModel.cs:229`).
6. **Primer grupo** (⏮) ▸ Calcular ▸ `FE3.txt`.

**Debe salir:** **82** (grupo 1 solo: 30; grupo 2 solo: 52). Sin el control (los dos grupos obligatorios) saldrían 94.

**FE3b.** Control de Grupos ▸ el contador debe leer «1/1» con `1,2` y `1` ▸ **Fallos** = `0,1` ▸ Tab ▸ OK ▸ Calcular ▸
`FE3b.txt` ⇒ **176** (al menos uno de los dos).

**FE3x. Sonda** (grupos creados desde Inicio)
1. Nueva ▸ B3 ▸ **Grupo siguiente** (crea el grupo 1) ▸ Variantes y Interrupciones como en el paso 3 de FE3 ▸ **Grupo
   siguiente** (crea el 2) ▸ como el paso 4 ▸ Control de Grupos `1,2` / `1` ▸ OK ▸ Primer grupo ▸ Calcular ▸ `FE3x.txt`.

**Debe salir:** **82**, lo mismo que FE3. **Predicción por el código: Aceptadas 0.** El grupo nuevo tiene
`partidosActivos` todo a `false` (`Grupo.cs:65-68`); `GrupoPartidos.AddGrupo` le calcula la máscara (`GrupoPartidos.cs:37-39`,
`Grupo.CalcularMascara` `Grupo.cs:69-87`), que sale **0**, y `todosPartidosActivos` queda en `false`. Al calcular,
`Grupo.AnalizaColumna(long)` aplica la máscara (`Grupo.cs:171-173`) y las condiciones ven la columna vacía: 0 variantes
y 0 interrupciones. Fallan los dos grupos, hay 2 fallos y ninguna columna pasa. Lo mismo pasa sin control de grupos: un
grupo creado desde Inicio con cualquier condición que rechace la columna vacía da 0. Hallazgo G-1.

---

## 5. Signos Seguidos con figuras (FE4)

**Qué hace.** Además de las rachas máximas (Var, 1, X, 2), cada concepto puede llevar una lista de **figuras**: la
figura es la lista de longitudes de los tramos seguidos de ese signo, **de mayor a menor**.
- Manual §2 (`MANUAL_USUARIO.md:86`): en `11X211X1X12111`, figura de `1` = `32211`, de `X` = `111`, de `2` = `11`, de
  `V` = `2111` (asserts en el oráculo). En la app se escribe con guiones: `3-2-2-1-1`.
- Motor: `FiltroSignosSeguidos.CumpleCondiciones` (`FiltroSignosSeguidos.cs:140-147`): rachas en sus listas **y**
  figuras (`AnalizaFiguras`, `:387-395`). La figura de variantes usa la máscara X|2 (`ObtenFiguraV`, `:276-280`) y
  `ObtenFiguraConMascara` (`:228-274`) ordena y empaqueta los tramos en 4 bits, el mayor arriba.
- Editor: `FigurasFiltrosFrmViewModel.Aceptar` (`:127-150`) convierte cada casilla con `ObtenerLongFiguraFromText`
  (`UtilidadesEntradasValores.cs:220-231`), que empaqueta **en el orden escrito** (ver SS-2).
- VM: las figuras solo se guardan si algún campo de texto tiene datos (`ContieneDatos`, `SignosSeguidosFrmViewModel.cs:109-113`;
  `Aceptar`, `:155-168`). Al volver del editor se recupera el VM con lo editado (`SignosSeguidosFrmPage.xaml.cs:27-44`,
  arreglo f97317a).

**FE4. Pasos**
1. Nueva ▸ B3 ▸ tarjeta **Signos Seguidos** ▸ **Borrar**.
2. Campo **1** = `1,2` ▸ Tab (Var, X y 2 vacíos).
3. **Figuras Variantes** ▸ casilla **1** = `3-2-1-1-1`, casilla **2** = `2-2-1-1-1` ▸ **Aceptar**.
4. De vuelta en Signos Seguidos: junto a «Figuras Variantes» debe verse **«Activas»** y el campo 1 seguir con `1,2` ▸
   **Aceptar**.
5. Calcular ▸ `FE4.txt`.

**Debe salir:** **52**. Solo con `1,2` saldrían 150; solo con las figuras, 60 (las dos figuras: 32 + 28 columnas).
Después: Signos Seguidos ▸ Borrar ▸ Aceptar.

**P (pantalla)**
- **P4-a** · Repite FE4 **sin** el campo 1 (todo vacío y solo las figuras) ▸ Aceptar. Según el código la condición
  **no se activa** (la tarjeta queda gris) y Calcular da 216: con los 4 campos vacíos `ContieneDatos` es falso y
  `Aceptar` desactiva el filtro aunque haya figuras. Igual en el original (`Free1X2/UI/Filtros/SignosSeguidosFrm.cs:89-99,348`).
  Hallazgo SS-1.
- **P4-b** · Escribe las figuras al revés, `1-1-1-2-3` y `1-1-1-2-2` (con «1» = `1,2`) ⇒ según el código **0**
  columnas y ningún aviso. Hallazgo SS-2.

---

## 6. Simetrías en espejo (FE5)

**Qué hace.** Cada fila es una simetría; acierta si todos sus partidos tienen el mismo signo. Se cuentan las que
aciertan y el número debe estar en «Aciertos» (admite rangos).
- Manual §6 (`MANUAL_USUARIO.md:342`): «que se cumplan solo 1 ó 2 de las simetrías impuestas».
- Motor: `FiltroSimetrias.cs:64-89`. VM: 30 filas (`SimetriasFrmViewModel.cs:63-69`); Aciertos con rangos
  (`ObtenerAciertos`, `:134-163`).

**FE5. Pasos**
1. Nueva ▸ B3 ▸ tarjeta **Simetrías** ▸ **Borrar**.
2. Filas 1 a 7: `1,8` · `2,9` · `3,10` · `4,11` · `5,12` · `6,13` · `7,14`. **Aciertos** = `0-3` ▸ Tab ▸ **Aceptar**.
3. Calcular ▸ `FE5.txt`.

**Debe salir:** **174**. Reparto en B3: 1 → 24, 2 → 72, 3 → 78, 4 → 36, 5 → 6 (la pareja 5,12 acierta siempre; 3,10 y
7,14 nunca). Con `0-2` saldrían 96. Después: Simetrías ▸ Borrar ▸ Aceptar.

---

## 7. Variantes con límite solo de doses (FE6, FE6b)

**Qué hace.** Limita a la vez el nº de variantes (X o 2), de equis y de doses. Un campo vacío vale «todos».
- Manual §6 «Variantes X y 2». Motor: `FiltroNoVariantes.AnalizaColumna(long)` (`FiltroNoVariantes.cs:103-108`) cuenta
  X y 2 con máscaras; `esColumnaValida` exige los tres.
- VM: `NoVariantesFrmViewModel.Aceptar` (`:82-117`): valida con un filtro temporal (W2, `:91`) y un campo vacío se
  rellena con `ObtenerTodosValores()` = 0..14 (`:94-107`).

**FE6. Pasos**
1. Nueva ▸ B3 ▸ tarjeta **Variantes X y 2** ▸ **Borrar**.
2. **Var** vacío · **X** vacío · **2** = `1,2,3` ▸ Tab ▸ **Aceptar**. La tarjeta, verde.
3. Calcular ▸ `FE6.txt`.

**Debe salir:** **148** (doses 1 → 16, 2 → 56, 3 → 76; P11 es un 2 fijo, así que nunca hay 0). Con `1,2` saldrían 72.

**FE6b.** Variantes ▸ **Var** = `0,1,2,3,4,5,6,7,8,9,10,11,12,13,14` (X vacío, 2 = `1,2,3`) ▸ Tab ▸ Aceptar ▸ Calcular ▸
`FE6b.txt` ⇒ **148**, las mismas columnas que FE6. Después: Borrar ▸ Aceptar.

**P6-a** · Var = `15` ▸ Aceptar ⇒ según el código, error (índice fuera de los límites: arrays de 15 casillas,
`FiltroNoVariantes.cs:52`) y la condición sin cambios (validación W2 antes de tocar el filtro). La pantalla anuncia
«Cantidades admitidas (0–15)» (`NoVariantesFrmPage.xaml:20`). Hallazgo V-1.

---

## 8. Hallazgos por gravedad

| # | Gravedad | Pantalla | Resumen | Evidencia | Prueba |
|---|---|---|---|---|---|
| G-1 | **grave** | Inicio (grupos) | Los grupos creados con «Grupo siguiente» no tienen partidos activos y no se pueden marcar: máscara 0, las condiciones del grupo ven la columna vacía. Además, en un grupo ≠ 0 el boleto que se ve y se edita es el **base** (`BoletoBaseViewModel.cs:295-302` escribe `analizador.SetPronostico`). El original toma los partidos de cada grupo del boleto (`Free1X2/UI/MainForm.cs:563-571`, `ObtenPartidosGrupos`) y no deja avanzar sin ninguno (`:1547-1553`, «No ha especificado ningún partido»). Explica también «Partidos Activos» vacío en el Listado (README, D-L3) | `MainPageViewModel.cs:448-460`; `Grupo.cs:65-87,171-173`; `GrupoPartidos.cs:37-39`; único `PonerPartidosActivos` de la app en `CrearGruposFrmViewModel.cs:47` | FE3x |
| R-1 | medio | Rentabilidad | La recaudación considerada arranca en 8.000.000 fijo; el original la toma de Configuración (14.000.000) y el VM ya usa Configuración para el precio y el %14 (y el Progol si la jornada es de México). Con 8.000.000 el tope del bote baja y FE1 daría 60 | `RentabilidadFrmViewModel.cs:63` frente a `:215-218`; original `Free1X2/UI/RentabilidadFrm.cs:96-101` (el `"8000000"` del diseñador, `:727`, se sobrescribe) | FE1 |
| CG-1 | doc | Control de Grupos | El manual dice que con fallos = 1 «se cumple uno u otro (cualquiera de los dos sistemas vale)». Es **exactamente uno**: si se cumplen los dos hay 0 fallos y la columna se rechaza. En su ejemplo no se nota porque las variantes no se solapan (5–9 y 10–12). «Al menos uno» es `0,1` | `MANUAL_USUARIO.md:354`; `ControlGrupos.cs:78` | FE3 / FE3b |
| SS-1 | heredado | Signos Seguidos | Con solo figuras (los 4 campos vacíos) la condición se desactiva al Aceptar | `SignosSeguidosFrmViewModel.cs:109-113,155,173-177`; original `SignosSeguidosFrm.cs:89-99,348` | P4-a |
| SS-2 | menor (heredado) | Figuras (Signos Seguidos) | La figura tiene que escribirse de mayor a menor; en otro orden (`1-1-1-2-3`) o sin guiones (el `32211` del manual) se guarda otra figura sin avisar y no pasa ninguna columna. La ayuda de la pantalla no lo dice. Mismo origen que G-2 de `plan_a.md` | `UtilidadesEntradasValores.cs:220-231`; `FiltroSignosSeguidos.cs:228-274`; `FigurasFiltrosFrmPage.xaml:15` | P4-b |
| V-1 | menor | Variantes, Signos Seguidos | Anuncian «(0–15)», pero los arrays tienen 15 casillas (0..14) y el 15 da error. Como D-1 de Distancias | `NoVariantesFrmPage.xaml:20`, `SignosSeguidosFrmPage.xaml:20`; `FiltroNoVariantes.cs:52`, `FiltroSignosSeguidos.cs:43` | P6-a |
| R-2 | heredado | Rentabilidad | «Premio estimado de 14» de una columna se muestra sin tope, mientras la EM sí lo aplica | `RentabilidadFrmViewModel.cs:186-191`; original `RentabilidadFrm.cs:155-159` | P1-a |
| R-3 | doc | Rentabilidad | El manual recomienda EM mínima 0,136; la pantalla trae 0,133 (como el original, `RentabilidadFrm.cs:469`) | `MANUAL_USUARIO.md:552`; `RentabilidadFrmViewModel.cs:67` | — |

Lo que no he podido comprobar sin la app (por eso FE3x y las P llevan «predicción por el código»): el texto exacto de
los mensajes, si la NumberBox de Rentabilidad acepta el decimal con coma (por eso FE1 usa enteros) y que «Aceptar» de
Columnas probables sin nada marcado no deje rastro (la tarjeta debe seguir gris).
