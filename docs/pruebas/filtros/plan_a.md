# Grupo A de filtros: plan de pruebas en la app

Pantallas: **Distancias, Simetrías, Contactos, Dibujos, Diferencias y Figuras Filtros** (las figuras de Contactos).
Oráculo: `scripts/pruebas_filtros_a.py`, en esta misma carpeta. Es independiente: programa cada regla con posiciones y cadenas.
Además, `emulacion_motor()` reprograma la lógica de bits del motor C# y comprueba, en las 216 columnas de B3 más 3.000 al
azar, que la regla del oráculo y la del motor son la misma.

```
python -I scripts/pruebas_filtros_a.py                      # asserts + clave y nº de columnas esperadas (+ sensibilidad)
python -I scripts/pruebas_filtros_a.py preparar C:\Pruebas  # escribe C:\Pruebas\entrada\FA_contactos_figuras.cont
python -I scripts/pruebas_filtros_a.py verificar C:\Pruebas # compara C:\Pruebas\salida\FA*.txt
```

## 0. Preparación y reglas comunes

- **Boleto B3** (docs/PRUEBAS_WINDOWS.md:66-67): P1 `1X2` · P2 `1` · P3 `X2` · P4 `1X2` · P5 `1` · P6 `1X` · P7 `X` ·
  P8 `1X2` · P9 `12` · P10 `1` · P11 `2` · P12 `1` · P13 `X` · P14 `1`. Cabecera esperada «Fijos: 8 - Dobles: 3 - Triples: 3».
  Con las condiciones de este grupo apagadas, Calcular da Procesadas **216**.
- **Abrir cada condición desde su tarjeta en Inicio** («Condiciones del grupo»: Distancias, Simetrías, Contactos,
  Dibujos, Diferencias), **no desde el panel lateral**. Solo las tarjetas fijan el grupo que se edita
  (`AppState.GrupoEnEdicion` se asigna únicamente en `MainPageViewModel.cs:296,304,312,326`). Si la página se abre
  desde el panel lateral y no hay grupo, «Aceptar» vuelve sin aplicar nada (p. ej. `DistanciasFrmViewModel.cs:99`).
  Si queda un grupo de una apertura anterior, la página edita ese.
- **Pulsa Tab antes de «Aceptar»** en Distancias, Simetrías y Diferencias. Sus cajas usan `x:Bind … TwoWay` y guardan
  el valor al perder el foco. En Contactos y en el editor de figuras no hace falta (`UpdateSourceTrigger=PropertyChanged`).
- **Calcular y grabar** (como en docs/PRUEBAS_WINDOWS.md:51-52): **Calcular** en la barra de Inicio ▸ «Grabar archivo» ▸
  «Seleccionar archivo...» ▸ `salida\<CLAVE>.txt` ▸ **Calcular**. Anota «Procesadas» y «Aceptadas».
- **Apagar la condición entre pruebas**: abre su tarjeta ▸ **Borrar** ▸ **Aceptar**. La siguiente prueba debe volver a
  dar Procesadas 216. En Contactos, «Borrar» solo vacía la pantalla (ver hallazgo C-4). Al reabrir pueden reaparecer
  los valores de la prueba anterior: pulsa siempre «Borrar» antes de escribir los nuevos.
- **Orden recomendado**: FA1, FA1b, FA2, FA3, FA3b, FA6, FA7, FA8, FA4, FA5, FA5b. FA6 necesita un Contactos que nunca
  haya tenido figuras, así que va antes de FA7.

## 1. Resumen

| Clave | Pantalla | Tipo | Debe salir (de 216) | Qué prueba |
|---|---|---|---|---|
| **FA1** | Distancias | condición | **71** | los 4 campos (Var, 1, X, 2) |
| **FA1b** | Distancias | condición | **168** | campos vacíos = todos |
| **FA2** | Simetrías | condición | **102** | 4 simetrías con rangos, Aciertos `2-3` |
| **FA3** | Contactos | condición | **71** | 4 parejas limitadas, 6 sin límite |
| **FA3b** | Contactos | condición | **134** | filas vacías = todos. ⚠ el código predice error y **0** (C-1) |
| **FA4** | Dibujos | condición | **68** | 3 dibujos marcados |
| **FA5** | Diferencias | condición | **67** | atajo Tríos + 4 conceptos (uno con rango) |
| **FA5b** | Diferencias | condición | **28** | 2 conjuntos a la vez |
| **FA6** | Figuras Filtros (desde Contactos) | condición | **24** | figuras nuevas con el editor. ⚠ el código predice **216** (C-3) |
| **FA7** | Contactos ▸ Abrir `.cont` | condición | **24** | lógica de figuras del motor, sin el editor |
| **FA8** | Figuras Filtros (editar las existentes) | condición | **12** | quitar una figura ya guardada |

Todas son condiciones del cálculo desde Inicio. En este grupo no hay herramientas que procesen ficheros. El único
fichero de entrada es `FA_contactos_figuras.cont`, para FA7. Las comprobaciones que solo se ven en pantalla van al final
de cada sección (marcadas «P»).

---

## 2. Distancias

**Qué hace.** Para cada signo (`1`, `X`, `2`) y para las variantes (`Var` = X o 2), calcula la **mayor separación entre
dos apariciones seguidas** de ese signo (diferencia de posiciones). Con 0 o 1 apariciones vale 0. La columna pasa si
cada distancia está en la lista de su campo.
- Manual: `MANUAL_USUARIO.md:84` define la distancia y trae el ejemplo `11X211X1X12111`: `1`→3, `X`→4, `2`→7, `V`→3.
  El oráculo lo comprueba con asserts. El §6 (`:329-330`) dice que se aplica a «`1`, `X`, `2` y variantes».
- Motor: `FiltroDistancias.cs:102-108` aplica las máscaras por signo. `MascVariantes` (`:30`) vale `011` en cada
  partido, es decir X o 2. `Distancia()` está en `:109-127`. La condición es `:143` (`intVar[distVar] && int1[…] && …`).
- VM: `DistanciasFrmViewModel.cs:93-133`. Valida antes con un filtro temporal (W2, `:103`). Un campo vacío se rellena
  con `ObtenerTodosValores()` = 0..14 (`:119-122`). Cada valor pasa por `Convert.ToInt32` tras partir por comas
  (`FiltroDistancias.cs:571-595`), así que **no admite rangos** `a-b`.

**FA1. Pasos**
1. Inicio ▸ Nueva ▸ boleto B3 ▸ tarjeta **Distancias**.
2. Tarjeta «Valores admitidos (0–15)»: **Var** = `2,3` · **1** = `3,4` · **X** = `6` · **2** = `0,2,3,4,5,6,7` ▸ Tab ▸ **Aceptar**.
3. Calcular ▸ Grabar `salida\FA1.txt`.

**Debe salir:** Procesadas 216, **Aceptadas 71**. Quitando cualquiera de los 4 campos el recuento cambia (102 / 89 /
98 / 74), así que cada campo se prueba de verdad.

**FA1b.** Distancias ▸ Borrar ▸ solo **1** = `3,4` (Var, X y 2 vacíos) ▸ Tab ▸ Aceptar ▸ Calcular ▸ `FA1b.txt` ⇒ **168**.
Prueba que un campo vacío equivale a «todos». Después: Borrar ▸ Aceptar.

**P (pantalla)**
- **P1-a** · **1** = `3,15` ▸ Aceptar. La pantalla anuncia 0–15, pero los arrays tienen 15 casillas (índices 0..14,
  `FiltroDistancias.cs:66-70`). Según el código sale «Se produjo un error inesperado» (índice fuera de los límites) y la
  condición queda como estaba (la validación W2 va antes de tocar el filtro, `DistanciasFrmViewModel.cs:103` frente a
  `:108`). Hallazgo D-1.
- **P1-b** · **1** = `3-4` ▸ Aceptar. Debe salir «La cadena de entrada no tiene el formato correcto» y la condición sin
  cambios. El manual §6 (`MANUAL_USUARIO.md:277`) dice que se admiten «rangos como `2-6`»; aquí no. Hallazgo D-2.

**Hallazgos**
- **D-1 (menor).** La pantalla dice «Valores admitidos (0–15)» (`DistanciasFrmPage.xaml:20`, y el comentario del VM
  `:23`), pero 15 lanza una excepción. La distancia máxima posible es 13.
- **D-2 (doc).** El manual promete rangos `2-6` en todas las condiciones (`MANUAL_USUARIO.md:277`). Distancias solo
  admite comas, y la propia pantalla así lo indica (`DistanciasFrmPage.xaml:21`).
- **D-3 (menor, accesibilidad).** El nombre accesible de «Var» es «Distancias para cualquier signo (Var)»
  (`DistanciasFrmPage.xaml:37`). Los comentarios del VM (`:22`, `:28`) dicen lo mismo. El motor usa variantes, X o 2
  (`FiltroDistancias.cs:30,107`), igual que el manual (`:84`). La etiqueta visible, «Var», es correcta.

---

## 3. Simetrías

**Qué hace.** Cada fila es una simetría: una lista de partidos (`a,b`, `a-b` o `a,b-c`). Una simetría **acierta** si
todos sus partidos llevan el mismo signo. Se cuentan las que aciertan y ese número debe estar en «Aciertos», que admite
valores y rangos.
- Manual: `MANUAL_USUARIO.md:342` («obliga a que determinados partidos tengan el mismo signo… que se cumplan solo 1 ó 2
  de las simetrías»). Coincide con el código.
- Motor: `FiltroSimetrias.cs:64-89`. Para cada simetría compara el signo de cada partido con el del primero (`:71-80`),
  cuenta los aciertos (`:85`) y exige `ArrayAciertos.IndexOf(aciertos) != -1` (`:88`).
- VM: las filas pasan por `EntradaValida` (`SimetriasFrmViewModel.cs:96-117`) y por `ObtenerValoresSeparadosPorComas`
  (`:128`). «Aciertos» se lee en `:134-163`: admite rangos y descarta valores fuera de 0..40.

**FA2. Pasos**
1. Inicio (B3, sin otras condiciones del grupo) ▸ tarjeta **Simetrías**.
2. Filas: **1** = `1,4` · **2** = `6-7` · **3** = `3,11` · **4** = `8-10`. **Aciertos** = `2-3` ▸ Tab ▸ **Aceptar**.
3. Calcular ▸ `FA2.txt`.

**Debe salir:** **102**. Reparto de aciertos en B3: 0 → 30, 1 → 81, 2 → 75, 3 → 27, 4 → 3. Con Aciertos `2` saldrían
75, y sin la fila `8-10`, 90. Después: Borrar ▸ Aceptar. «Borrar» ya pone un filtro vacío en el grupo,
`SimetriasFrmViewModel.cs:302`.

**P (pantalla)**
- **P2-a** · Fila 1 = `0,4` ▸ Aceptar. Según el código no ocurre nada: no cierra y **no avisa**
  (`SimetriasFrmViewModel.cs:198`). El original muestra «Verifique que ha introducido una simetria correcta…»
  (`Free1X2/UI/Filtros/SimetriasFrm.cs:431-434`). Hallazgo S-1.
- **P2-b** · Filas de FA2 con **Aciertos vacío** ▸ Aceptar ▸ Calcular ⇒ según el código, **Aceptadas 0**: la lista de
  aciertos queda vacía (`:137`) y ninguna columna pasa (`FiltroSimetrias.cs:88`). Al reabrir, Aciertos muestra `0`
  (`:203`), que significa otra cosa. Hallazgo S-2.

**Hallazgos**
- **S-1 (menor, regresión de UI).** Con una entrada no válida, «Aceptar» no muestra el error que sí muestra el original.
  El comentario de `:197` lo menciona, pero el mensaje solo existe en `ObtenerFiltroTemporal` (`:222`), que Aceptar no usa.
- **S-2 (heredado).** Aciertos vacío ⇒ 0 columnas, guardado como «0». El original hace lo mismo
  (`SimetriasFrm.cs:420-425` y `ObtenerAciertos`).

---

## 4. Contactos

**Qué hace.** Mira las 13 parejas de partidos seguidos. Un contacto `ab` es cada pareja con los signos a y b, en
cualquier orden. Cuenta `1X`, `12`, `X2`, `11`, `XX` y `22`, y deriva **1V = 1X+12**, **XV = XX+X2**, **2V = 22+X2** y
**VV = XX+X2+22**. Cada recuento debe estar en su fila. Opcionalmente, la «figura» de contactos debe estar en la lista
de figuras (sección 7).
- Manual: solo dice «condición específica de contactos entre signos/partidos» (`MANUAL_USUARIO.md:345`). La definición
  sale de la pantalla («Se da un contacto entre dos signos cuando aparecen juntos…», `ContactosFrmPage.xaml`) y del
  motor. Hallazgo C-5.
- Motor: `FiltroContactos.cs:155-202`. Recorre las parejas con `switch (signo1|signo2)` (`:176-194`) y calcula las
  derivadas en `:198-201`. Condición en `:204-223`, figuras en `:282-303`.
- VM: 10 filas en el orden 1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV (`ContactosFrmViewModel.cs:56-68`). Una fila vacía se
  sustituye por `ObtenerTodosValores(0, 15)` (`:139`, `:150-159`). Cada valor pasa por `Convert.ToInt32`, sin rangos.
  No hay validación previa.

**FA3. Pasos**
1. Inicio ▸ tarjeta **Contactos** ▸ **Borrar**.
2. En «Número de contactos por pareja de signos» rellena **las 10 filas**: **1X** = `4,5,6` · **11** = `1,2` ·
   **2V** = `1,2` · **VV** = `2,3`. Las otras seis (**12, X2, XX, 22, 1V, XV**) = `0,1,2,3,4,5,6,7,8,9,10,11,12,13`
   (pégalo). Ninguna fila puede quedar vacía (C-1) ▸ **Aceptar**.
3. Calcular ▸ `FA3.txt`.

**Debe salir:** **71**. Sin 1X / 11 / 2V / VV saldrían 83 / 89 / 86 / 84. Ejemplo de contactos a mano: en
`11X211X1X12111` salen 11 = 4, 1X = 5, X2 = 1, 12 = 3, 1V = 8, XV = 1, 2V = 1, VV = 1 (assert en el oráculo).

**FA3b. Pasos** (prueba que una fila vacía equivale a «todos», como en el original)
1. Contactos ▸ **Borrar** ▸ solo **11** = `1,2`, las otras 9 vacías ▸ **Aceptar**.
2. Anota lo que pasa. Si sigue en Contactos con un error, pulsa **Cancelar**. Luego Calcular ▸ `FA3b.txt`.

**Debe salir:** **134** (11 ∈ {1,2}: 64 + 70). **Predicción por el código: error «Se produjo un error inesperado»
(índice fuera de los límites), la pantalla no se cierra y después Calcular da Aceptadas 0.** El motivo:
`ObtenerTodosValores(0, 15)` incluye el 15 y los arrays tienen 15 casillas (`FiltroContactos.cs:79-88`, índices 0..14).
`SetNum1X` marca 0..14 y falla en el 15. Para entonces `GuardarDatos` ya activó el filtro y lo dejó a medias
(`ContactosFrmViewModel.cs:143-150`): 1X con todo marcado y el resto con nada. Hallazgo C-1.

**Después (limpieza):** Contactos ▸ Borrar ▸ Aceptar. Comprueba que Calcular vuelve a dar 216.

**P3-a (opcional)** · Una fila con `3-6` ▸ Aceptar ⇒ «La cadena de entrada no tiene el formato correcto». Según el
código, aquí **sí** se estropea el filtro: no hay validación W2, al contrario que en Distancias. Calcular ⇒ 0. Esto
contradice `MANUAL_USUARIO.md:278-279` («la condición se queda como estaba»). Hallazgo C-2.

**Hallazgos**
- **C-1 (grave, solo en WinUI).** Toda fila vacía rompe la condición: hay que escribir las 10 siempre.
  `ContactosFrmViewModel.cs:139` usa `ObtenerTodosValores(0, 15)`, con el comentario «el form legacy construye todos
  sobre 0..15». En el original, `OptionNumTol0_14.Maximo` **devuelve `VariablesGlobales.NumeroPartidos` (14)** e ignora
  el 15 del diseñador (`Free1X2/UI/Controls/OptionNumTol0_14.cs:87-104`), así que allí «todos» es 0..14 y no falla.
- **C-2 (medio).** Sin validación previa: cualquier error de escritura deja la condición activa y a medias. Distancias
  sí valida (`DistanciasFrmViewModel.cs:103`). El manual promete que «la condición se queda como estaba».
- **C-3 (grave).** Las figuras nuevas creadas con «Editar figuras…» se pierden. Ver la sección 7 y FA6.
- **C-4 (menor).** «Borrar» solo vacía la pantalla (`ContactosFrmViewModel.cs:282-291`). Con «Aceptar» se desactiva,
  pero el filtro del grupo conserva los valores y las figuras, que reaparecen al reabrir. El original sustituye el
  filtro por uno nuevo (`Free1X2/UI/Filtros/ContactosFrm.cs:700-707`).
- **C-5 (doc).** El manual no define Contactos: ni las parejas, ni V = X o 2, ni 1V/XV/2V/VV, ni la figura. La pantalla
  dice «de 0 a 14» (`ContactosFrmPage.xaml:27`), pero el máximo real es 13 y el 15 provoca el fallo C-1.

---

## 5. Dibujos

**Qué hace.** Marcas los dibujos `X+Y` permitidos (nº de equis + nº de doses). La columna pasa si su dibujo está marcado.
- Manual: `MANUAL_USUARIO.md:85` (ejemplo `3+2`, comprobado en el oráculo) y `:306-310`. Coincide con el código.
- Motor: `FiltroDibujos.cs:44-53`. Cuenta X y 2 con máscaras y consulta `dibs[x,2]`. La matriz se construye con
  `Split('+')` en `:113-134`.
- VM: `DibujosFrmViewModel.cs:177-209` pasa las etiquetas marcadas («4+3»…).

**FA4. Pasos**
1. Inicio ▸ tarjeta **Dibujos** ▸ **Desmarcar todos**.
2. Marca las casillas **`4+3`**, **`3+4`** y **`5+2`**. Debe leerse «Dibujos seleccionados: 3» ▸ **Aceptar**.
3. Calcular ▸ `FA4.txt`.

**Debe salir:** **68** (4+3 → 30, 3+4 → 19, 5+2 → 19). Después: Desmarcar todos ▸ Aceptar.

**P4-a** · Mira la malla. Según el código hay **136** casillas, entre ellas imposibles con 14 partidos como `15+0`,
`0+15` o `7+8`. El original tiene 120. Hallazgo B-1.

**Hallazgos**
- **B-1 (menor, UI).** `DibujosFrmViewModel.cs:63` fija `NumPartidos = 15`. El original usa
  `VariablesGlobales.NumeroPartidos` (`Free1X2/UI/Controls/GridDibujosCentral.cs:18`) = 14 (`parametros.free1x2`:
  `numero="14"`). No afecta al filtrado, porque esos dibujos nunca salen.

---

## 6. Diferencias (condición `Filtro.Diferencias`, `Free1X2.Domain/EnumFiltros.cs:37`)

**Qué hace.** Cada línea es un **grupo de partidos**. Para cada grupo se toma su subcolumna (los signos de esos
partidos, en orden) y se calculan sus variantes, equis, doses, dibujo (X,2), interrupciones y formato (la secuencia de
signos). Por cada concepto se cuenta **cuántos valores distintos** salen entre los grupos, y ese número debe estar en
la lista del concepto. Se admiten rangos. Un concepto vacío no limita nada. Si hay varios **conjuntos** (◀ ▶), deben
cumplirse todos.
- Pantalla: «indica la cantidad o el intervalo de valores DISTINTOS entre los grupos» (`DiferenciasFrmPage.xaml:14`).
- Manual: «exigen que tengan la **misma** cantidad de variantes/…» (`MANUAL_USUARIO.md:343`). Eso es solo el caso
  «1 valor distinto». Hallazgo F-1.
- Motor: `Diferencia.cs:30-119`. Saca la subcolumna con `IgnorarPartidos`, que pese al nombre conserva los partidos
  marcados (`Utils/UtilColumnas.cs:429-444`). Cuenta valores distintos en `:57-103` y comprueba en `:105-118`.
  `FiltroDiferencias.cs:26-37` exige que todos los conjuntos se cumplan (Y).
- VM: `CreaDiferencia` (`DiferenciasFrmViewModel.cs:167-211`). Las líneas y los conceptos se leen con
  `ObtenerBoolArrayFromTxt` / `ObtenerListaFromTxtAciertos`, que admiten `a,b-c`. Atajos en `:306-335` (Tríos + Paso fijo
  ⇒ `1-3`, `4-6`, `7-9`, `10-12`, según `UtilidadesEntradasValores.cs:404-426`).

**FA5. Pasos**
1. Inicio ▸ tarjeta **Diferencias**. El contador debe leer `1/0`.
2. Tarjeta **Atajos**: Agrupación = **Tríos**, **Paso fijo** activado ▸ **Generar**. Comprueba que las líneas 1–4 son
   `1-3`, `4-6`, `7-9`, `10-12` (si no, escríbelas a mano).
3. «Valores distintos por concepto»: **Equis** = `2` · **Doses** = `2-3` · **Interrupciones** = `2` · **Formatos** = `4`
   (Variantes y Dibujos vacíos) ▸ Tab ▸ **Aceptar**.
4. Calcular ▸ `FA5.txt`.

**Debe salir:** **67**. Sin Equis / Doses / Interrupciones / Formatos saldrían 136 / 75 / 89 / 83.

**FA5b. Pasos** (justo después de FA5)
1. Tarjeta **Diferencias**. Muestra el conjunto 1 (`1/1`) ▸ **▶**. El contador pasa a `2/1` y la pantalla queda vacía.
2. Línea 1 = `1-7`, línea 2 = `8-14` · **Variantes** = `1` (las dos mitades con el mismo nº de variantes) ▸ Tab ▸ **Aceptar**.
3. Calcular ▸ `FA5b.txt`.

**Debe salir:** **28**. Solo el conjunto 2 daría 64. Después: Diferencias ▸ **Borrar** ▸ Aceptar.

**P5-a** · Variantes = `a` ▸ Aceptar ⇒ según el código vuelve a Inicio **sin avisar** y sin guardar el conjunto
(`CreaDiferencia` devuelve null en `:207-210`). Igual que en el original (`Free1X2/UI/Filtros/DiferenciasFrm.cs:253-256,
280-292`). Hallazgo F-2.

**Hallazgos**
- **F-1 (doc).** El manual (`:343`) describe solo el caso «misma cantidad». La condición real limita el **nº de valores
  distintos** (1 = todos iguales, 2, 3…) y admite intervalos, como dice la pantalla.
- **F-2 (heredado).** Un valor no válido se ignora en silencio: Aceptar cierra y la condición no se guarda.

---

## 7. Figuras Filtros (editor de figuras de Contactos)

**Qué es.** No es una condición por sí sola. Es el editor de la lista de **figuras** de Contactos (y de Signos
Seguidos). La figura de Contactos de una columna son sus 10 recuentos (1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV)
**distintos de cero, de mayor a menor, separados por «-»**. Por ejemplo, `11X211X1X12111` ⇒ `8-5-4-3-1-1-1-1` (assert).
Con figuras, la columna pasa solo si su figura está en la lista.
- Motor: `FiltroContactos.cs:282-303` (`Figuras.Contains(ObtenerFiguraLongValor())`) y `:982-1015` (ordena y empaqueta
  en grupos de 4 bits). Texto ⇄ figura en `UtilidadesEntradasValores.cs:220-253`, con el formato **`9-5-4-2-…`**.
- Editor: `FigurasFiltrosFrmViewModel.cs`. `CargarDesdeHandoff` (`:85-100`) toma la lista estática `FigurasEnEdicion`.
  `Aceptar` (`:125-150`) la vacía y la rellena, y lo que no se puede leer, como `*`, se descarta (`:136-142`).
- Manual: no documenta las figuras de Contactos. Solo menciona las figuras de signos y de zonas (`MANUAL_USUARIO.md:340`).

**FA6. Pasos** (figuras nuevas con el editor; Contactos sin figuras previas)
1. Inicio ▸ tarjeta **Contactos** ▸ **Borrar** ▸ **Editar figuras…**.
2. «Casillas de figuras»: casilla **1** = `9-5-4-2-2-2-1-1-1`, casilla **2** = `8-5-3-3-2-2-1-1-1` ▸ **Aceptar**.
3. De vuelta en Contactos, **anota**: ¿el semáforo de «Figuras» está verde? ¿qué tienen las filas? Asegúrate de que
   las **10 filas** tienen `0,1,2,3,4,5,6,7,8,9,10,11,12,13` (pégalo donde falte) ▸ **Aceptar**.
4. Calcular ▸ `FA6.txt`.

**Debe salir:** **24** (12 columnas por figura). **Predicción por el código: 216**, con el semáforo de Figuras gris y
las 10 filas ya rellenas con 0..13. No hay `NavigationCacheMode` en ninguna página, así que al volver del editor
(`Frame.GoBack`) se crea un `ContactosFrmPage` nuevo con un VM nuevo (`ContactosFrmPage.xaml.cs:19`). En modo Back
solo se llama a `RefrescarFiguras()` (`:38-41`) y el nuevo `_figuras` es null (`ContactosFrmViewModel.cs:52`). La lista
que editó el editor (`:198-199`) ya no la tiene nadie. Hallazgo C-3.

**FA7. Pasos** (la lógica de figuras del motor, sin el editor)
1. `python -I scripts/pruebas_filtros_a.py preparar C:\Pruebas` (crea `entrada\FA_contactos_figuras.cont`: 10 filas 0..13 y las 2 figuras de FA6).
2. Contactos ▸ **Abrir** ▸ `entrada\FA_contactos_figuras.cont`. Deben verse las 10 filas `0,…,13` y el semáforo de
   Figuras **verde** ▸ **Aceptar**.
3. Calcular ▸ `FA7.txt`.

**Debe salir:** **24**, las mismas columnas que FA6. «Abrir» pasa por `MarcarValores`, que sí copia las figuras
(`ContactosFrmViewModel.cs:187`), y «Aceptar» las guarda (`:161-164`).

**FA8. Pasos** (editar figuras que ya tiene el filtro; justo después de FA7)
1. Contactos. Al abrir, el semáforo de Figuras debe estar verde ▸ **Editar figuras…**. Deben aparecer las 2 figuras.
2. Vacía la casilla 2 (`8-5-3-3-2-2-1-1-1`) ▸ **Aceptar**.
3. En Contactos, revisa que las 10 filas tienen 0..13 ▸ **Aceptar** ▸ Calcular ▸ `FA8.txt`.

**Debe salir:** **12**. Por el código también sale 12, pero el semáforo se verá **gris**. Al abrir, `_figuras` apunta a
la misma lista que `filtro.Figuras` (`ContactosFrmViewModel.cs:128`), así que el editor la cambia en el propio filtro
del grupo. Es más: la cambia aunque luego se pulse «Cancelar» en Contactos. Después: Contactos ▸ Borrar ▸ Aceptar.

**P (pantalla)**
- **P7-a** · Panel lateral ▸ Filtros ▸ **Figuras Filtros** (`Navigation/PortedPages.cs:30`) ▸ escribe `1-1` ▸ Aceptar.
  Según el código no se aplica a ninguna condición: `FigurasEnEdicion` es estático y no se reinicia
  (`FigurasFiltrosFrmViewModel.cs:57`). Si ya se había abierto el editor desde Contactos o Signos Seguidos, modifica
  aquella lista vieja. Hallazgo G-1.
- **P7-b** · En el editor, el texto de ayuda dice «escribe sus dígitos o usa el comodín» y «0 a 16, o el comodín *»
  (`FigurasFiltrosFrmPage.xaml:15`, `FigurasFiltrosFrmViewModel.cs:110`). Hallazgo G-2.

**Hallazgos**
- **G-1 (medio).** «Figuras Filtros» aparece en el panel lateral como si fuera una pantalla de filtro, pero sin una
  condición que la abra no tiene destino (lista estática `FigurasEnEdicion`, `:57`, leída en `:87`).
- **G-2 (medio, UI/doc).** La ayuda no dice que el separador es **«-»**. Si escribes `4321`, se lee como un solo número
  y se parte en grupos de 4 bits: la figura queda `1-0-14-1`, sin ningún error (`UtilidadesEntradasValores.cs:220-231`).
  El comodín `*` que anuncia se descarta en silencio (`:141`). El 16 no cabe en 4 bits. Y `EsFiguraValida`
  (`:113-121`) compara cada grupo de 4 bits con 0..16, así que nunca rechaza nada.
- **C-3 (grave, ya citado).** Las figuras nuevas creadas en el editor no llegan a Contactos (FA6). Por el mismo patrón
  de página, probablemente pasa lo mismo en **Signos Seguidos** (`SignosSeguidosFrmPage.xaml.cs:31-38`,
  `SignosSeguidosFrmViewModel.cs:288-294`). No lo he verificado en esa pantalla.

---

## 8. Hallazgos por gravedad

| # | Gravedad | Pantalla | Resumen | Evidencia | Prueba |
|---|---|---|---|---|---|
| C-1 | **grave** | Contactos | Una fila vacía ⇒ excepción y condición activa a medias ⇒ 0 columnas | `ContactosFrmViewModel.cs:139`; `FiltroContactos.cs:79-88`; original `OptionNumTol0_14.cs:87-104` | FA3b |
| C-3 | **grave** | Contactos / Figuras Filtros | Figuras nuevas del editor perdidas al volver (página recreada) | `ContactosFrmPage.xaml.cs:19,38-41`; `ContactosFrmViewModel.cs:52,198-199`; sin `NavigationCacheMode` | FA6 |
| C-2 | medio | Contactos | Sin validación W2: un error de escritura deja la condición rota (contra el manual `:278-279`) | `ContactosFrmViewModel.cs:141-159` frente a `DistanciasFrmViewModel.cs:103` | P3-a |
| G-1 | medio | Figuras Filtros | Entrada en el panel lateral sin destino (lista estática) | `FigurasFiltrosFrmViewModel.cs:57,87`; `PortedPages.cs:30` | P7-a |
| G-2 | medio | Figuras Filtros | Ayuda engañosa: no dice el separador «-»; el `*` se descarta; validación inútil | `FigurasFiltrosFrmPage.xaml:15`; VM `:110,113-121,141` | P7-b |
| — | medio | todas | Abiertas desde el panel lateral editan `GrupoEnEdicion` (null o el último abierto desde Inicio) | `MainPageViewModel.cs:296`; `DistanciasFrmViewModel.cs:99` | — |
| S-1 | menor | Simetrías | Aceptar con entrada no válida: sin mensaje (el original avisa) | `SimetriasFrmViewModel.cs:198`; original `SimetriasFrm.cs:431-434` | P2-a |
| C-4 | menor | Contactos | «Borrar» no reinicia el filtro del grupo (el original sí) | `ContactosFrmViewModel.cs:282-291`; original `ContactosFrm.cs:700-707` | — |
| D-1 | menor | Distancias | Dice 0–15; el 15 lanza una excepción | `DistanciasFrmPage.xaml:20`; `FiltroDistancias.cs:66-70` | P1-a |
| D-3 | menor | Distancias | Nombre accesible «Var = cualquier signo»; es X o 2 | `DistanciasFrmPage.xaml:37`; `FiltroDistancias.cs:30` | — |
| B-1 | menor | Dibujos | Malla de 136 dibujos (15 partidos) en vez de 120 | `DibujosFrmViewModel.cs:63`; original `GridDibujosCentral.cs:18` | P4-a |
| S-2 | heredado | Simetrías | Aciertos vacío ⇒ 0 columnas, guardado como «0» | `SimetriasFrmViewModel.cs:137,203`; `FiltroSimetrias.cs:88` | P2-b |
| F-2 | heredado | Diferencias | Valor no válido ignorado en silencio | `DiferenciasFrmViewModel.cs:207-210` | P5-a |
| D-2 | doc | Distancias (y Contactos) | El manual promete rangos `2-6`; solo hay comas | `MANUAL_USUARIO.md:277`; `FiltroDistancias.cs:571-595` | P1-b |
| F-1 | doc | Diferencias | El manual dice «misma cantidad»; la condición limita el nº de valores distintos | `MANUAL_USUARIO.md:343`; `DiferenciasFrmPage.xaml:14` | — |
| C-5 | doc | Contactos / Figuras | El manual no define Contactos ni su figura | `MANUAL_USUARIO.md:345` | — |

Lo que no he podido comprobar sin la app (por eso FA3b, FA6 y las P llevan «predicción por el código»): el texto exacto
de los mensajes de error y que los `TextBox` de estas páginas guarden el valor al pulsar el botón sin Tab.
