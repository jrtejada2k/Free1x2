# Plan de pruebas de filtros, grupo B

Pantallas: **Formatos**, **Formatos 123**, **Analizador Formatos123**, **Saca Formatos** (CalculoFormatosFrm),
**Pesos Numéricos**, **Valoración** (filtro `ValoracionSignos`) y **Análisis de signos** (VSignosFrm).

Oráculo: `scripts/pruebas_filtros_b.py` (esta carpeta). Reimplementa cada regla con cadenas y fuerza bruta, sin llamar al motor.

```
python -I scripts/pruebas_filtros_b.py                       # asserts, clave + nº de líneas, números de pantalla
python -I scripts/pruebas_filtros_b.py preparar C:\Pruebas   # escribe entrada\FB_val_mixta.txt, FB_vs_entrada.txt, FB_af123.txt
python -I scripts/pruebas_filtros_b.py verificar C:\Pruebas  # compara salida\FB1.txt … FB13.txt (mismo criterio que pw.verificar)
```

> **Aviso sobre el encargo.** «VSignos» no es la condición `ValoracionSignos`. `VSignosFrmPage` es la herramienta
> **«Análisis de signos»** del panel lateral (`MainWindow.xaml.cs:471`), que lee un fichero sin usar el motor. La
> condición `ValoracionSignos` es la tarjeta **«Valoración»** de Inicio, que abre `ValoracionFrmPage`
> (`MainPageViewModel.cs:167`). Este plan cubre las dos (§6 y §7).

## Resumen

| Clave | Pantalla | Qué se prueba | Debe salir |
|---|---|---|---|
| **FB1** | Formatos | B3 · X1 `2,3` · 11 `1-3` · X\*1 `1,2` | **92** de 216 |
| **FB2** | Formatos | FB1 + Líneas `1,2` + Global `5-7` | **33** |
| **FB3** | Formatos 123 | B3 · valoración mixta · 11 6–8 · 3 1–1 · 21 2–3 · Líneas acertadas `2` | **61** |
| **FB4** | Formatos 123 | B3 · Ignorar repeticiones · 13 · 31 · 22 · Aciertos globales `1` | **76** |
| **FB5** | Pesos Numéricos | B3 · Global `2,5,8` · 1 `1,2,4` | **56** |
| **FB6** | Pesos Numéricos | B3 · Global `2,5,8` · X `1,3,5,7,9` · Tol. Global `1,4` · Tol. X `2,4` · Núm. tol. 0 y 1 | **110** |
| **FB7** | Pesos Numéricos | FB6 + figuras 2-2-1 y 2-1-1-1 | **22** |
| **FB8** | Valoración | B3 · valoración mixta · por sumas · Global `590-610#640-660` | **82** |
| **FB9** | Valoración | FB8 + 1 `300-380` | **59** |
| **FB10** | Análisis de signos | FB_vs_entrada · ganadora `1 X 2 - …` · aspirando a 13 · Pleno No · «columnas» | **6** columnas |
| **FB11** | Análisis de signos | lo mismo en «% enteros» | **14** líneas |
| **FB12** | Saca Formatos | columna `11X211X1X12111` | informe de **55** líneas |
| **FB13** | Saca Formatos | columna `11XX2212X11111` (9 pares distintos) | informe de **58** líneas |
| T1, AF1, AF2, V1, V2, VS0–VS2 | pantalla | números en pantalla | ver cada sección |

Cada prueba de condición se hizo sensible a un fallo creíble de la app. Los asserts de `comprobar()` lo verifican:

| Fallo | Saldría |
|---|---|
| Formatos leídos al revés | 147 en vez de 92 |
| Formatos 123 sin valoración | 8 en vez de 61 |
| Formatos 123 con la valoración al revés | 0 en vez de 61 |
| Pesos con los partidos al revés | 12 en vez de 56 |
| Pesos sin tolerancias | 72 en vez de 110 |
| Valoración con solo el primer rango de `#` | 32 en vez de 82 |
| Análisis de signos con Pleno Sí | 2 en vez de 6 |

---

## 0. Preparación

1. `python -I scripts/pruebas_filtros_b.py preparar C:\Pruebas`. Si se usa el arnés general, sus `entrada\` y `salida\` son las mismas.
2. **Boleto B3** (`docs/PRUEBAS_WINDOWS.md:64-75`): P1 `1X2` · P2 `1` · P3 `X2` · P4 `1X2` · P5 `1` · P6 `1X` · P7 `X` ·
   P8 `1X2` · P9 `12` · P10 `1` · P11 `2` · P12 `1` · P13 `X` · P14 `1`. Cabecera: «Fijos: 8 - Dobles: 3 - Triples: 3»,
   216 columnas. Atajo opcional: guárdalo una vez con «Máscaras ▾» ▸ Guardar como `B3` y, en cada prueba, usa
   **Nueva combinación** (Ctrl+N) ▸ «Máscaras ▾» ▸ `B3`.
3. **Pegar desde Excel.** Abre `entrada\FB_val_mixta.txt` con el Bloc de notas, pulsa Ctrl+A y Ctrl+C, y luego el botón
   «Pegar desde Excel» de la rejilla. Comprueba P1 = 50 / 30 / 20, P7 = 30 / 40 / 30 y P14 = 55 / 30 / 15.
4. **Calcular y grabar**, igual que en C1–C4. En la barra de Inicio pulsa **Calcular** ▸ «Grabar archivo» ▸
   «Seleccionar archivo...» ▸ `salida\FBn.txt` ▸ **Calcular**. Debe salir «Procesadas» **216** y en «Aceptadas» el
   número de la tabla.
5. **Pulsa Tab** después del último campo antes de «Aceptar»: los TextBox guardan el valor al perder el foco.
   (Los de Pesos guardan con cada tecla, `PesosNumFrmPage.xaml:31`, pero Tab no molesta.)
6. Empieza cada grupo de condiciones (Formatos, Formatos 123, Pesos, Valoración) con **Nueva combinación + B3**. Así
   no queda activa la condición del grupo anterior. Cuidado con Valoración: «Borrar» no la desactiva, porque al
   «Aceptar» rellena los rangos vacíos (`ValoracionFrmViewModel.cs:275-289`, `401-405`).
7. ⚠ **No pulses «Aceptar» en una condición a la que has vuelto con Esc** desde Estadísticas, Saca formatos o el
   Analizador: aparece vacía (hallazgo **H1**). Sal con «Cancelar».

---

## 1. Formatos (tarjeta «Formatos», filtro `FormatosSignos`)

**Qué hace.** El manual dice: «controlan la aparición o repetición de secuencias de signos… En `11X211X1X12111` el
formato `1X` aparece 3 veces, `1X2` una vez. También puede pedirse que aparezcan N formatos distintos»
(`docs/MANUAL_USUARIO.md:341`). Así lo hace el código:

- Cada línea es un formato con su rango de apariciones. Signos posibles: `1`, `X`, `2`, `V` (X o 2) y `*` (cualquiera)
  (`FormatosFrmViewModel.cs:155-166`; codificación en `FormatoSignos.cs:77-85`).
- Se cuentan las apariciones leyendo de izquierda a derecha y **con solapes** (`FormatoSignos.cs:45-52`;
  `FiltroFormatosSignos.cs:45-51` reordena la columna). El ejemplo del manual sale igual: assert en `comprobar()`.
- Una relación sin «Líneas» ni «Global» vale si **todas** sus líneas están en rango (`FormatosSignos.cs:44-60`).
- Con «Líneas» y/o «Global» (`FormatosSignos.cs:62-83`), las líneas dejan de exigirse una a una:
  - **Líneas**: cuántas líneas están en su rango.
  - **Global**: suma de las apariciones de todas las líneas.
  - Cada campo rellenado debe contener su número.
- Varias relaciones («Relación siguiente») se combinan con Y (`FiltroFormatosSignos.cs:52-63`).
- **Formato de los campos.** «Mín-Máx», «Líneas» y «Global» admiten `2,3`, `1-3` y `1,3-5`
  (`RangosHelper.cs:76-141`; para Líneas y Global, `UtilidadesEntradasValores.cs:362-382`).
  - Un «Mín-Máx» no numérico salta como «Se produjo un error inesperado…» y el filtro no cambia, porque la excepción
    llega antes de la asignación (`FormatosFrmViewModel.cs:221` frente a `:233`).
  - Una «Líneas» o «Global» no numérica se descarta en silencio (`FormatosFrmViewModel.cs:190-193`).

**FB1.** Nueva + B3 ▸ tarjeta **«Formatos»** ▸ en «Líneas de formato» escribe, columna «Formato» / «Mín-Máx»:
fila 1 `X1` / `2,3`; fila 2 `11` / `1-3`; fila 3 `X*1` / `1,2` ▸ Tab ▸ **Aceptar**. La tarjeta debe quedar en verde.
Calcular ▸ Grabar `FB1.txt`.
- Esperado: **Aceptadas 92**. Primeras columnas: `11X111X12121X1`, `11X111XX2121X1`.

**FB2.** Tarjeta «Formatos». Recarga las 3 líneas y el contador muestra «1/1» ▸ «Líneas» `1,2` ▸ «Global» `5-7` ▸ Tab
▸ Aceptar ▸ Calcular ▸ Grabar `FB2.txt`.
- Esperado: **33**.
- Diagnóstico si falla: solo con Líneas `1,2` saldrían 123; solo con Global `5-7`, 107.

**Hallazgos.** H1 (vuelta con Esc) y H6 (al manual le falta explicar Líneas y Global).

## 2. Formatos 123 (tarjeta «Formatos 123», filtro `Formatos123`)

**Qué hace.** El texto de la pantalla dice: «el 1 corresponde al signo más valorado, el 2 al segundo más valorado y el
3 al menos valorado» (`Formatos123FrmPage.xaml:22`). El manual solo lo nombra junto a Formatos (`:341`). Así lo hace el
código:

- **Traducción a 1/2/3.** Cada signo se traduce según la valoración de su partido (rejilla «Porcentajes»). En caso de
  empate, el 1 gana a X y a 2 si es `>=`, y la X gana al 2 si es `>=` (`Formatos123FrmViewModel.cs:347-400`). Sin
  valoración (todo 0), la traducción queda 1→1, X→2, 2→3.
- **Recuento.** Cada formato (solo dígitos `1`, `2`, `3`, hasta 14; `:183-191`) se cuenta con solapes en la columna
  traducida, leída en orden natural (`FiltroFormatos123.cs:115-157`, `267-289`; `Formato123.cs:61-70`).
- **Sin «Ignorar repeticiones».**
  - «Líneas Acertadas» es cuántos formatos tienen sus apariciones entre su «Mín» y su «Máx».
  - Una fila con Mín o Máx no entero se salta en silencio (`Formatos123FrmViewModel.cs:204-211`).
- **Con «Ignorar repeticiones»** (la etiqueta pasa a «Aciertos Globales»), se cuentan los formatos que aparecen al
  menos una vez y Mín/Máx no cuentan (`FiltroFormatos123.cs:159-181`).
- **Decisión final.** La columna vale si ese número está en la lista: `2`, `1,3` o `1-3`, entre 0 y 40
  (`Formatos123FrmViewModel.cs:223-251`; `FiltroFormatos123.cs:155`, `180`).
  - El tope `aciertosGlobales <= AciertosMax` de `:138` no cambia el resultado, porque el número que se pasa del tope
    nunca está en la lista.
  - Con la lista vacía **no vale ninguna columna** (hallazgo H2).

**FB3.**
1. Nueva + B3 ▸ tarjeta **«Formatos 123»** ▸ tarjeta «Porcentajes» ▸ **«Pegar desde Excel»** con `FB_val_mixta.txt`.
2. **T1 (pantalla).** En «Utilidades» ▸ «Traducir columna 1X2 a 123» escribe `11X111X11121X1` ▸ `›`. Debe salir
   **`11231212211111`**.
3. Lista «Formatos», columnas Nº / Formato / Mín / Máx: fila 1 `11` / `6` / `8`; fila 2 `3` / `1` / `1`; fila 3 `21` /
   `2` / `3`. En «Aciertos» ▸ «Líneas Acertadas» escribe `2` ▸ Tab ▸ **Aceptar** ▸ Calcular ▸ Grabar `FB3.txt`.

- Esperado: **61**.
- Diagnóstico: con `2,3` saldrían 114; con `3`, 53; con «Líneas Acertadas» vacía, **0**; sin pegar la valoración, 8.

**FB4.**
1. Tarjeta «Formatos 123». Recarga formatos y valoración; comprueba P1 50/30/20 en «Porcentajes» o vuelve a pegar.
2. **Borrar**. Limpia la lista, «Líneas Acertadas» y el interruptor, pero no la rejilla (`:469-481`).
3. Formatos `13`, `31`, `22` con Mín/Máx vacíos ▸ «Ignorar repeticiones» = **Activado** (la etiqueta cambia a
   «Aciertos Globales») ▸ escribe `1` ▸ Tab ▸ **Aceptar** ▸ Calcular ▸ Grabar `FB4.txt`.

- Esperado: **76**. Con `2` saldrían 90.

**Hallazgos.** H1, H2 y H3.

## 3. Analizador Formatos123 (solo pantalla)

**Cómo se llega.** Solo desde Formatos 123 ▸ «Analizador de formatos». No está en el panel lateral: `PortedPages.cs:20`
solo lo usan el smoke y la captura (`MainWindow.xaml.cs:769`).

**Qué hace.**
- Carga un fichero de columnas: líneas de más de 16 signos lo anulan (`AnalisisFormatos123FrmViewModel.cs:142-177`).
- Tiene **su propia** rejilla de valoración, como el original con `valors1`.
- **«Analizar»** cuenta, en la columna actual traducida a 1/2/3, las apariciones de los formatos que le pasa Formatos 123
  (`:184-236`).
- **«Mostrar todos»** lista todos los trozos de la columna traducida, por longitud y posición, agrupando los iguales en
  su primera aparición (`:243-292`).

**AF1 / AF2.**
1. **Después de FB3** (filtro guardado), abre la tarjeta «Formatos 123». Recarga 11 / 3 / 21 ▸ **«Analizador de
   formatos»**.
2. «Archivo de columnas» ▸ `entrada\FB_af123.txt`. El contador debe decir **1/3**.
3. «Valoración de los partidos» ▸ «Pegar desde Excel» con `FB_val_mixta.txt`.
4. **Analizar**. Repite con «Columna siguiente» (`›`) en las columnas 2 y 3.

| Columna | Traducida (T1) | 11 | 3 | 21 | ¿Está en FB3.txt? |
|---|---|---|---|---|---|
| 1/3 `11X111X11121X1` | `11231212211111` | **5** | **1** | **2** | sí (2 líneas en rango) |
| 2/3 `X1X211X21121X1` | `21221211211111` | **5** | **0** | **4** | no |
| 3/3 `21221XX22121X1` | `31121111111111` | **10** | **1** | **1** | no |

- **AF2.** Vuelve a 1/3 ▸ **Mostrar todos**. Deben salir **84 filas**. Las 8 primeras: `1` 9 · `2` 4 · `3` 1 · `11` 5 ·
  `12` 3 · `23` 1 · `31` 1 · `21` 2. La última: `11231212211111` 1.
- Al terminar, pulsa Esc: Formatos 123 aparecerá **vacía** (H1). Pulsa «Cancelar», no «Aceptar».

**Hallazgos.** H4 (le pasa los formatos de pantalla, no los guardados).

## 4. Saca Formatos (CalculoFormatosFrm)

**Cómo se llega.** Formatos ▸ «Utilidades de formatos» ▸ **«Saca formatos»** (`FormatosFrmPage.xaml.cs:43-48`).
Título: «Saca Formatos (Indeciso)».

**Qué hace.** Es una herramienta, no filtra.
- Para una columna escribe un informe (`CalculoFormatosFrmViewModel.cs:59-237`, igual que el original
  `Free1X2/UI/CalculoFormatosFrm.cs:185-466`):
  - de pares, tríos, cuartetos y quintetos: en orden de primera aparición, con apariciones y solapes, y «… distintos = n»;
  - de contactos: pares de partidos seguidos sin orden, donde 1V = 1X + 12, XV = X2 + XX, 2V = X2 + 22 y
    VV = X2 + XX + 22 (`:247-277`).
- Graba con `File.WriteAllText` y avisa «Ya».

**FB12.**
1. Nueva (Formatos vacío) ▸ tarjeta «Formatos» ▸ «Saca formatos».
2. «Seleccionar fichero…» ▸ `salida\FB12.txt`.
3. «Columna (14 signos: 1 / X / 2)» = `11X211X1X12111` ▸ **Sacar Formato** ▸ aviso «Ya».

Esperado: informe de **55 líneas**. Primeras: `* Pares que aparecen`, `11-4`, `1X-3` (el `1X` ×3 del manual), `X2-1`,
`21-2`, `X1-2`, `12-1`, `Pares distintos = 6`. Resumen: Trios distintos = 9, Cuartetos distintos = 11, Quintetos
distintos = 10. Contactos: `1X-5`, `12-3`, `X2-1`, `11-4`, `XX-0`, `22-0`, `1V-8`, `XV-1`, `2V-1`, `VV-1`.

**FB13.** Igual, con `salida\FB13.txt` y la columna `11XX2212X11111`. Esperado: **58 líneas**, con
«Pares distintos = 9»; es la rama de los 9 pares, `j == 8`.

Al volver con Esc, Formatos aparece vacía (H1): pulsa «Cancelar».

`verificar` compara en mayúsculas, como `pw.leer`, así que la clave lleva el texto en mayúsculas.

## 5. Pesos Numéricos (tarjeta «Pesos Numéricos», filtro `PesosNumericos`)

**Qué hace.** El manual solo dice «distribuye numéricamente (del 0 al 9) las columnas posibles… se usa sobre todo para
evitar columnas cuyo peso coincida con el de la jornada anterior» (`MANUAL_USUARIO.md:344`). La regla sale del código:

- **Pesos.**
  - Cada signo suma un número según su partido. Las tablas de 1, X y 2 están en `FiltroPesosNumericos.cs:58-60`: para
    el 1 empiezan 7,5,3,1,8,…; para la X, 5,1,6,2,7,…; para el 2, 1,2,3,…
  - Peso de 1, X y 2: suma de cifras repetida hasta una cifra; es 0 solo si no hay ese signo (`:226-241`).
  - Var = raíz(X + 2) y Global = raíz(Var + 1) (`:113-163`).
- **Comprobación.** Cada peso debe estar en su conjunto; un campo vacío vale 0..14, o sea todo
  (`PesosNumFrmViewModel.cs:243-247`).
- **Tolerancias.**
  - Un peso fuera de su conjunto es un «fallo». Solo se perdona si está en la «Tolerancia» del mismo campo.
  - El nº de fallos debe estar en «Núm. Tolerancias» (`FiltroPesosNumericos.cs:331-425`).
  - Sin ninguna tolerancia escrita, solo vale 0 fallos (`PesosNumFrmViewModel.cs:249-263`).
  - Si escribes tolerancias y no marcas el 0, se **rechazan** las columnas sin fallos.
- **Figuras** (`:164-224`). Si hay figuras marcadas, la figura de los 5 pesos debe ser una de ellas. Con las 5 marcadas
  vale todo.
  - Se comprobó sobre las 4.782.969 columnas que las figuras 5 y 4-1 no salen nunca, como dice la nota de la pantalla
    (`PesosNumFrmViewModel.cs:217-218`). Por eso da igual que el código confunda 4-1 con 2-2-1 (`:204-222`, producto 4).
- **Formato de los campos.** Solo dígitos separados por comas (`:549-557`, `Convert.ToInt32` por trozo).
  - `2-6` da «Se produjo un error inesperado: La cadena de entrada no tiene el formato correcto» (`App.xaml.cs:28-41`)
    y el filtro no cambia (`PesosNumFrmViewModel.cs:290-292`). Ver H5.

**FB5.** Nueva + B3 ▸ tarjeta **«Pesos Numéricos»** ▸ «Pesos Numéricos»: Global `2,5,8`, 1 `1,2,4` ▸ **Aceptar** ▸
Calcular ▸ Grabar `FB5.txt`.
- Esperado: **56**. Primeras: `11X111X11121X1`, `11X111X12121X1`.
- La columna `11X111X11121X1` tiene pesos 1=2, X=7, 2=2, Var=9, Global=2.

**FB6.**
1. Tarjeta «Pesos Numéricos» ▸ **Borrar**.
2. «Pesos Numéricos»: Global `2,5,8`, X `1,3,5,7,9`.
3. «Tolerancias»: Global `1,4`, X `2,4`.
4. «Núm. Tolerancias»: pulsa **0** y **1**.
5. **Aceptar** ▸ Calcular ▸ Grabar `FB6.txt`.

- Esperado: **110**.
- Diagnóstico: sin tolerancias saldrían 72; con solo «1» marcado, 38.

**FB7.** Tarjeta «Pesos Numéricos». Recarga; Var, 1 y 2 muestran `0,1,…,14`, que significa todos, y los botones 0 y 1
siguen pulsados ▸ «Figuras»: pulsa **2-2-1** y **2-1-1-1** ▸ Aceptar ▸ Calcular ▸ Grabar `FB7.txt`.
- Esperado: **22**.

**Hallazgos.** H1 y H5.

## 6. Valoración (tarjeta «Valoración», filtro `ValoracionSignos`)

**Qué hace.** El manual dice: «asigna un valor… a cada signo de cada partido y exige que la suma de los valores de los
signos elegidos en la columna esté en un intervalo». Su ejemplo: dos triples 50/30/20 con 60–100 dejan fuera X2, 2X y
22 (`MANUAL_USUARIO.md:332-336`); está como assert. Así lo hace el código:

- **Por sumas.** Global es la suma de los valores de los signos de la columna, y «1», «X» y «2» la de los de ese signo
  (`FiltroValoracionSignos.cs:98-125`). Los cuatro deben estar en rango (`:160-167`).
- **Rangos.** `a-b`, y varios separados por `#` (`RangosOpciones.cs:30-56`). Un rango vacío no admitiría nada
  (`:58-74`), pero «Aceptar» y «Calcula» rellenan los vacíos con todo el intervalo posible
  (`ValoracionFrmViewModel.cs:225-290`).
- «Por productos x 3E7» no se prueba: da números en coma flotante.

**FB8.**
1. Nueva + B3 ▸ tarjeta **«Valoración»** ▸ «Porcentajes» ▸ **«Pegar desde Excel»** con `FB_val_mixta.txt`.
2. «Modo» = **Por sumas**. Cambiar de modo borra los rangos (`:54-63`).
3. «Rangos de filtrado» ▸ Global `590-610#640-660` ▸ Tab.
4. **V1/V2 (pantalla).** «Calcular valoración» ▸ Columna `11X111X11121X1` ▸ **Calcula**.
   - Valor **623**.
   - «Mín-Máx Sumas» = **303-684**.
   - Los campos 1, X y 2 se rellenan con **0-528**, **0-463** y **0-409**.
   - Global sigue en `590-610#640-660`. **Escribe Global antes de pulsar Calcula**: si está vacío, se rellena con
     303-684.
5. **Aceptar** ▸ Calcular ▸ Grabar `FB8.txt`.

- Esperado: **82**. Con solo `590-610` saldrían 32.

**FB9.** Tarjeta «Valoración». Recarga todo ▸ campo «1» = `300-380` (sustituye `0-528`) ▸ Tab ▸ Aceptar ▸ Calcular ▸
Grabar `FB9.txt`.
- Esperado: **59**. Con solo el rango del 1 saldrían 158.

**Hallazgos.** H1, H7 y H8.

## 7. Análisis de signos (VSignosFrm)

**Cómo se llega.** Panel lateral ▸ Combinación ▸ **«Análisis de signos»** (`MainWindow.xaml.cs:471`). El manual lo
resume como «seguir la combinación mientras se juega la jornada» (`MANUAL_USUARIO.md:217`).

**Qué hace.** Es una herramienta y no usa el motor (`VSignosFrmViewModel.cs:205-289`).
- **Lectura.** Lee el fichero; las líneas más cortas que la primera no cuentan.
- **Aciertos** contra la «Columna ganadora» (`AcCb`, `:430-462`, igual que `Free1X2/UI/VSignosFrm.cs:269-316`):
  - `-` es comodín y siempre acierta.
  - Con «Último partido como Pleno» = Sí, el último partido solo suma si se acertaron todos los anteriores.
- **Recuento y premios.**
  - Las columnas con aciertos ≥ «Aspirando a…» se cuentan por partido y signo (rejilla «Recuento por partido») y se
    guardan.
  - «Premios remanentes» = nº de columnas con 14-13-12-11-10 aciertos, de todas, sin mirar la aspiración.
- **Grabar** según el «Formato» (`:293-341`):
  - «columnas»: las aceptadas, en orden de fichero y en mayúsculas;
  - «% enteros»: una línea por partido, `%1, %X, %2`.

**Pasos.**
1. **Examinar…** ▸ `entrada\FB_vs_entrada.txt`. Calcula solo, con todo `-`, aspirando a 14 y Pleno Sí.
   **VS0:** «Procesadas: 10», «Premios remanentes: 10-0-0-0-0», recuento P1 = 5/3/2, P2 = 2/6/2, P3 = 3/1/6.
2. Columna ganadora: partido 1 = `1`, partido 2 = `X`, partido 3 = `2`, el resto `-`. Puedes pulsar la celda 1 de la
   fila 1, la X de la fila 2 y el 2 de la fila 3 de la rejilla (`FijarSigno`, `:380-386`), o seleccionar el `-` y
   escribir. Hazlo **después** de abrir el fichero, porque abrirlo reinicia la columna (`:197`).
3. «Último partido como Pleno» = **No**. «Aspirando a…» = **13**: pulsa «−» una vez.
4. **Calcular.** **VS1:**
   - «Procesadas: 10», «Premios remanentes: **2-4-3-1-0**».
   - Recuento 1/X/2 por partido: P1 5/1/0 · P2 1/5/0 · P3 1/1/4 · P4 2/2/2 · P5 2/2/2 · P6 1/2/3 · P7 3/1/2 ·
     P8 1/3/2 · P9 3/1/2 · P10 3/2/1 · P11 3/2/1 · P12 2/2/2 · P13 4/1/1 · P14 1/3/2.
5. «Formato» = **columnas** ▸ **Grabar** ▸ `salida\FB10.txt`. Esperado: **6** columnas: `1X21X21X21X21X`,
   `1X111111111111`, `112XXXXXXXXXXX`, `XX222222222222`, `1X222222111112`, `1XXX1X1X1X1X1X`. La última está en
   minúsculas en la entrada y debe salir en mayúsculas.
6. «Formato» = **% enteros** ▸ **Grabar** ▸ `salida\FB11.txt`. Esperado: 14 líneas; las 3 primeras son `83, 17, 0`,
   `17, 83, 0` y `17, 17, 67`. No hay mitades exactas: N = 6 (assert).
7. **VS2, opcional, para H8.** «Último partido como Pleno» = Sí ▸ Calcular: «Premios remanentes: **2-0-4-3-1**» y solo
   2 columnas en el recuento (P1 = 2/0/0). El original pone Pleno = No al abrir un fichero de 14 signos.

**Hallazgos.** H8, H9 y H10.

---

## Hallazgos

Ninguno está arreglado. Los de UI son opciones para el dueño (R2). Ninguno se ha visto en ejecución: salen del código
leído y de la regla de WinUI de que una `Page` sin `NavigationCacheMode` se crea de nuevo en cada visita.

| # | Dónde | Qué | Evidencia |
|---|---|---|---|
| **H1** | Formatos, Formatos 123, Valoración, Pesos (y otras 9 condiciones) | Al volver con Esc desde Estadísticas, Saca formatos, el Analizador o Buscar límite, la condición aparece **vacía**. No se recarga el grupo y la página es una instancia nueva. Si entonces se pulsa «Aceptar», se escribe el filtro vacío y se desactiva; en Formatos 123 también se pierde la valoración. El comentario «para no perder los formatos en edición» no se cumple. Las otras 9 están en `grep -l "NavigationMode.Back" Views/Ported/*.cs`. | `FormatosFrmPage.xaml.cs:30-39`, `Formatos123FrmPage.xaml.cs:25-33`, `ValoracionFrmPage.xaml.cs:26-35`, `PesosNumFrmPage.xaml.cs:26-35`. Sin `NavigationCacheMode` en todo `Free1X2.WinUI` (lo dicen `AppState.cs:69` y `MainPageViewModel.cs:125`). Esc = `ContentFrame.GoBack()` (`MainWindow.xaml.cs:716-717`) |
| **H2** | Formatos 123 (manual) | Con formatos y «Líneas Acertadas / Aciertos Globales» vacía no pasa **ninguna** columna (`Contains` sobre una lista vacía) y la tarjeta queda verde. El original hace lo mismo; el manual no lo dice. | `FiltroFormatos123.cs:155`, `:180`; `Formatos123FrmViewModel.cs:322-334`; original `Free1X2/UI/Filtros/Formatos123Frm.cs:744-772`; `MANUAL_USUARIO.md:341` |
| **H3** | Formatos 123 | Una fila no válida (Mín o Máx no entero, o formato con otros dígitos) se **salta** y se siguen leyendo las demás. El original paraba en la primera fila no válida y descartaba las siguientes. | `Formatos123FrmViewModel.cs:194-220` (`continue`) frente a `Formatos123Frm.cs:116-151` (`break`) |
| **H4** | Formatos 123 ▸ Analizador | WinUI le pasa los formatos **de pantalla** sin guardar. El original pasaba los del filtro guardado y avisaba «Guarde el Filtro Primero!!!». | `Formatos123FrmViewModel.cs:286-303` frente a `Formatos123Frm.cs:925-937` |
| **H5** | Pesos (manual) | El manual dice que los valores van «con comas: `0,1,2` o rangos como `2-6`». En Pesos un rango da «error inesperado»; la pantalla sí avisa «separados por comas». | `MANUAL_USUARIO.md:277` frente a `FiltroPesosNumericos.cs:549-557`; `PesosNumFrmPage.xaml:25` |
| **H6** | Formatos (manual) | El manual («también puede pedirse que aparezcan N formatos distintos») no explica Líneas = nº de líneas en rango ni Global = suma de apariciones. Tampoco dice que, con cualquiera de los dos, las líneas dejan de exigirse una a una. | `MANUAL_USUARIO.md:341` frente a `FormatosSignos.cs:38-85` |
| **H7** | Valoración (manual) | Los rangos se escriben `a-b` y se separan con `#`, no con comas; el manual §6 no lo dice y su nota general habla de comas. | `MANUAL_USUARIO.md:277`, `:332-336` frente a `RangosOpciones.cs:30-56` y `ValoracionFrmPage.xaml:84` |
| **H8** | Valoración | «Calcula» con una columna de menos de 14 signos no avisa; el original mostraba «Introducir columna de 14 signos para el calculo.». Además, «Calcula» escribe la valoración en el filtro del grupo sin «Aceptar», como el original. | `ValoracionFrmViewModel.cs:179-196` frente a `Free1X2/UI/Filtros/ValoracionFrm.cs:385-404` |
| **H9** | Análisis de signos | Al abrir un fichero, el original pone «Pleno» = Sí solo con 15 o más signos y la aspiración = nº de signos. WinUI deja Pleno = **Sí** y aspiración = 14 siempre. Con 14 signos el resultado cambia: VS1 frente a VS2 da 6 frente a 2 aceptadas, y premios 2-4-3-1-0 frente a 2-0-4-3-1. | `VSignosFrmViewModel.cs:115`, `:126`, `:169-201` frente a `Free1X2/UI/VSignosFrm.cs:131-148` |
| **H10** | Análisis de signos | Otras diferencias con el original. **(a)** «% con decimales» graba `a, b, c` con punto; el original, `a<TAB>b<TAB>c` con la cultura del sistema (`VSignosFrmViewModel.cs:321-332` frente a `VSignosFrm.cs:199-205`). **(b)** La rejilla siempre muestra recuentos; el original mostraba % en los modos % (`:271-276` frente a `VSignosFrm.cs:333-372`; UI, R2). **(c)** `MANUAL_FLUJOS.md:211` dice que «Análisis de Signos» usa el Flujo 4 del motor, pero la herramienta no llama al motor. **(d)** Mejora: el original grababa en el directorio actual con solo el nombre del fichero (`VSignosFrm.cs:184-185`); WinUI graba en la ruta elegida. | citados en cada punto |

**Comprobado, sin hallazgo.**
- El orden de lectura de los formatos coincide con el ejemplo del manual.
- La nota de las figuras 5 y 4-1 de Pesos es cierta para las 3^14 columnas.
- Saca Formatos y el Analizador replican el original línea a línea.
