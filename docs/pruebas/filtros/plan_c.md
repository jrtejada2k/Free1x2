# Plan de pruebas de filtros, grupo C

Pantallas: Columnas probables, Grupos de equipos, If-Then (Condiciones relacionadas), Pares, Tríos y Dependencia
lineal. Oráculo: `scripts/pruebas_filtros_c.py` (misma carpeta). Formato y verificación iguales que `scripts/pruebas_windows.py`.

| Clave | Pantalla | Tipo | Debe salir |
|---|---|---|---|
| **FC1** | Columnas probables (1 CP, Aciertos + Ac. Seguidos + Fallos Seguidos) | condición | **80** de 216 |
| **FC2** | Columnas probables (2 CP + Relaciones I, suma de aciertos) | condición | **114** |
| **FC13** | Columnas probables, sonda «Todos a Triple» | condición | **216** (el código predice **0**, H3) |
| **FC3** | Grupos de equipos (1 grupo) | condición | **84** |
| **FC4** | Grupos de equipos (2 grupos + relación) | condición | **90** |
| **FC5** | If-Then, 1 relación | condición | **159** |
| **FC6** | If-Then, 2 relaciones, deben cumplirse 2 | condición | **133** |
| **FC7** | If-Then, 2 relaciones, deben cumplirse 1 | condición | **83** |
| **FC8** | If-Then + Variantes a la vez | condición | **95** (el código predice **159**, H1) |
| **FC9** | Pares | herramienta | **48** (Procesadas 216, Válidas 48) |
| **FC10** | Tríos | herramienta | **126** (Procesadas 216, Válidas 126) |
| **FC11** | Dependencia lineal, triple | herramienta | **72** |
| **FC12** | Dependencia lineal, doble 12, segunda pasada | herramienta | **108** (el código predice **36**, H9) |

## 0. Preparación

1. `python -I scripts/pruebas_filtros_c.py` corre los asserts y lista lo esperado.
2. `python -I scripts/pruebas_filtros_c.py preparar C:\Pruebas` escribe en `C:\Pruebas\entrada\` los ficheros `FC_B3.txt` (las 216 columnas
   del boleto B3), `FC_pares.par` y `FC_trios.tri`, y crea `C:\Pruebas\salida\`.
3. Cada fichero que grabe la app se guarda como `C:\Pruebas\salida\<clave>.txt`. Al final se ejecuta
   `python -I scripts/pruebas_filtros_c.py verificar C:\Pruebas`, que da OK o MAL por clave y dice lo que sobra o falta.
4. **Boleto B3** (como en C1–C4 de `docs/PRUEBAS_WINDOWS.md`): P1 `1X2` · P2 `1` · P3 `X2` · P4 `1X2` · P5 `1` · P6 `1X`
   · P7 `X` · P8 `1X2` · P9 `12` · P10 `1` · P11 `2` · P12 `1` · P13 `X` · P14 `1`. Son 216 columnas. Para no
   meterlo a mano cada vez: B3 ▸ «Máscaras ▾» ▸ Guardar como `B3`. Antes de cada prueba de condición: **Nueva** ▸
   «Máscaras ▾» ▸ `B3`. «Nueva» crea un `Analizador` nuevo (`Free1X2.WinUI/Services/AppState.cs:195-202`) y por
   tanto borra todas las condiciones y el If-Then.
5. **Calcular** (para las pruebas de condición): botón Calcular de la barra de Inicio ▸ «Grabar archivo» ▸
   «Seleccionar archivo...» ▸ `salida\<clave>.txt` ▸ Calcular. Hay que anotar «Procesadas» (siempre **216**) y
   «Aceptadas».
6. En los campos de texto de las condiciones hay que pulsar **Tab** antes de «Aceptar» (lección de C1–C4). En las
   casillas numéricas (NumberBox) también: escribir el número y pulsar Tab.

---

## 1. Columnas probables (condición `ColProbables`)

**Cómo se llega:** Inicio ▸ tarjeta «Columnas Probables» (columna derecha de «Condiciones del grupo»,
`Free1X2.WinUI/Views/MainPageViewModel.cs:169`). Es la página «Columnas Probables», con las pestañas Columnas,
Relaciones I, II y III y Control Fallos.

**Qué hace:**
- El manual (`docs/MANUAL_USUARIO.md:358-397`) la define como un segundo pronóstico sobre un grupo de partidos con un
  rango de aciertos.
- Motor, `Free1X2.Domain/MotorCalculo/ColumnaProbable.cs:271-323` (`AnalizaColumna`). Por cada partido con signo en
  la CP:
  - cuenta un acierto si el signo de la columna está entre los de la CP;
  - lleva también la racha máxima de aciertos (ACS) y la de fallos (FS);
  - los partidos sin signo en la CP no cuentan ni cortan las rachas.
- La columna pasa si AC, ACS y FS están en sus listas (`esColumnaValida`, `:161`).
- `FiltroColProbables.Analizar` (`FiltroColProbables.cs:54-92`) exige todas las CP. Después aplica Control Fallos y
  las Relaciones I, II y III.
- Relaciones I, «Suma Aciertos»: la suma de los AC de las CP indicadas tiene que estar en el rango
  (`RelacionCP1.cs:124-150`).
- Si un campo de rango se deja vacío, vale cualquier valor de 0 a 14 (`ColProbablesFrmViewModel.cs:437-445`).

**Formato de los campos** (comprobado en el código):
- **«Pronóstico (14 signos: 1 / X / 2)»**
  - Un carácter por partido, en mayúsculas, como mucho 14 (`ColProbablesFrmPage.xaml:41-45`).
  - Si se escriben menos de 14, los partidos que faltan quedan fuera de la CP.
  - No admite dobles ni triples (ver H3).
- **«Aciertos», «Ac. Seguidos» y «Fallos Seguidos»**
  - Solo admiten listas con comas (`10,11,12`). Un rango `10-12` da error (ver H2).
  - «Puntos» sí admite rangos.
- **Relaciones I**
  - «Columnas» y «Suma Aciertos» admiten `a-b` o una lista ordenada `a,b,c`, pero no las dos formas mezcladas
    (`RelacionCP1.cs:311-345`).
- Estos campos se actualizan al teclear (`UpdateSourceTrigger=PropertyChanged`), así que no hace falta Tab.

### FC1: una CP con los tres rangos

1. Nueva ▸ Máscaras ▾ ▸ B3.
2. Abre la tarjeta «Columnas Probables» y quédate en la pestaña «Columnas».
3. Rellena:
   - **Pronóstico:** `112X11X21121X1`
   - **Aciertos:** `10,11,12`
   - **Ac. Seguidos:** `5,6,7,8`
   - **Fallos Seguidos:** `0,1`
   - Deja vacíos «Puntos» y las «Tolerancias locales».
4. Pulsa **Aceptar** (abajo). La tarjeta debe quedar en verde.
5. Calcula y graba `FC1.txt`.

**Debe salir:** Procesadas **216**, Aceptadas **80**. Sin «Ac. Seguidos» saldrían 86.

### FC2: dos CP y una relación de suma

1. Nueva ▸ B3.
2. En la pestaña Columnas:
   - **Pronóstico:** `112X11X21121X1`, con los rangos vacíos.
   - Pulsa **«>»** («Columna siguiente») **dos veces**: la primera solo guarda la CP 1 (sigue en 1/1, igual que el
     original, `Free1X2/UI/Filtros/ColProbablesFrm.cs:621-641`); la segunda pasa a **2/2**.
   - **Pronóstico:** `2X1X1XX1` (8 signos, la CP solo abarca P1–P8), con los rangos vacíos.
3. En la pestaña **Relaciones I**:
   - **Columnas:** `1-2`
   - **Suma Aciertos:** `13-14`
   - Deja vacíos «Recorrido», «¿Cuántas Columnas?» y «Número de Aciertos».
4. Pulsa Aceptar, calcula y graba `FC2.txt`.

**Debe salir:** **114**. Las sumas de B3 se reparten así: 11→4, 12→24, 13→54, 14→60, 15→42, 16→24, 17→8.

### FC13: sonda de «Todos a Triple» (comprueba H3)

1. Nueva ▸ B3.
2. En la pestaña Columnas:
   - Pulsa **«Todos a Triple»**. La casilla mostrará `1X21X2…`.
   - **Aciertos:** `14`
3. Pulsa Aceptar, calcula y graba `FC13.txt`.

**Debe salir:** **216**. Una CP de 14 triples la acierta cualquier columna.

**El código predice 0:** al Aceptar la CP se convierte en `1X21X21X21X21X` (ver H3). Si de verdad salen 0, el
hallazgo queda confirmado. También se puede comprobar volviendo a abrir la tarjeta: la casilla mostrará
`1X21X21X21X21X`.

**El oráculo** reproduce los ejemplos del manual §7 (asserts):
- 4 grandes: 9 / 33 / 27.
- Restricción progresiva: 37.888 / 24.272 / 10.912.
- Las dos CP con 3–6: 27.552; con la suma entre 7 y 11: 24.288.

**Sin cubrir:**
- Puntos y «Cambiar Puntuación».
- Tolerancias locales (AC, ACS y FS Tol, «Núm Tolerancias» 0–3) y letras de tolerancia en los valores (`3A`).
- Relaciones I: Recorrido, ¿Cuántas Columnas? y Número de Aciertos.
- Relaciones II y Relaciones III (escaleras, sándwichs, agrupaciones).
- Control Fallos.
- Copiar datos y Copiar Columnas.
- Importador y Exportador CPs.
- CP con dobles: no se pueden escribir (H3).
- Navegación «<<» / «>>».

---

## 2. Grupos de equipos (condición `GruposEquipos`)

**Cómo se llega:** Inicio ▸ tarjeta «Grupos Equipos» (`MainPageViewModel.cs:162`). Es la página «Grupos de Equipos»,
con las pestañas «Grupos Equipos» y «Relaciones».

**Qué hace:**
- Manual (`docs/MANUAL_USUARIO.md:315-327`): eliges un conjunto de equipos y pones límites a sus victorias, empates,
  derrotas y suma de puntos (3-1-0). En «Relaciones» los límites se aplican a la suma de varios grupos.
- Motor, `GrupoEquipos.cs:75-149`, para cada partido con equipo elegido:
  - **Casa**: el `1` es victoria (3 puntos), la `X` es empate (1 punto) y el `2` es derrota.
  - **Fuera**: al revés.
  - **Los dos equipos del partido**: si gana uno, cuenta 1 victoria y 1 derrota (3 puntos); el empate cuenta 2
    empates (2 puntos).
- La columna pasa si cumple todos los grupos (`FiltroGruposEquipos.cs:43-54`) y todas las relaciones
  (`RelacionGE1.cs:70-119`).
- Los campos admiten número, lista o rango con guion (validación en `UtilidadesEntradasValores.SonTodosNumeros`,
  `:362`; lectura con `RangosHelper.ObtenBoolArray`).
- El enlace es `TwoWay` por defecto, que se actualiza al perder el foco: **Tab antes de Aceptar o de «Siguiente ►»**.

**Cómo se marca un equipo:** en «Equipos del grupo», cada fila N tiene dos botones.
- El de la **izquierda** es el equipo de casa y el de la **derecha** el de fuera.
- Muestran el nombre del equipo si la jornada se descargó; si no, «Equipo casa N» / «Equipo fuera N».
- Para marcar los dos equipos de un partido, se pulsan los dos botones de su fila.

### FC3: un grupo

1. Nueva ▸ B3.
2. Abre la tarjeta «Grupos Equipos». En la pestaña «Grupos Equipos» marca:
   - fila **1**: casa
   - fila **4**: casa
   - fila **6**: casa
   - fila **8**: casa **y** fuera
   - fila **11**: fuera
3. En «Objetivos del grupo»:
   - **Victorias:** `2-3`
   - **Empates:** `1-2`
   - **Derrotas:** vacío
   - **Suma Puntos:** `8-12`
4. Pulsa Tab y luego **Aceptar** (abajo a la derecha).
5. Calcula y graba `FC3.txt`.

**Debe salir:** **84**.

### FC4: dos grupos y una relación

1. Nueva ▸ B3. Abre «Grupos Equipos».
2. **Grupo 1:** los mismos equipos que en FC3 y solo **Victorias** `2-4`. Pulsa Tab y luego **«Siguiente ►»**. El
   contador debe pasar a **2/2**.
3. **Grupo 2:** marca
   - fila **3**: casa
   - fila **9**: fuera
   - fila **13**: casa

   Pon solo **Derrotas** `0-1` y pulsa Tab.
4. En la pestaña **Relaciones**:
   - **Grupos Equipos:** `1-2`
   - **Suma Puntos:** `12-15`
   - Deja vacías las demás.
5. Pulsa Tab, **Aceptar**, calcula y graba `FC4.txt`.

**Debe salir:** **90**.

**El oráculo** reproduce el ejemplo del manual §6: el grupo de 5 equipos con 3 victorias, 0 empates y 2 derrotas
suma 9 puntos y queda fuera de 10-15. También comprueba la regla de los dos equipos de un partido.

**Sin cubrir:**
- Sumas de victorias, empates y derrotas en Relaciones.
- Eliminar grupo o relación.
- Abrir, Guardar, Copiar y Pegar.
- Estadísticas.
- Varias relaciones.

---

## 3. If-Then: Condiciones relacionadas (`analizador.IfThen`)

**Cómo se llega:** Inicio ▸ botón **«Condiciones Relacionadas»**, debajo de la rejilla, junto a «Tolerancias»
(`MainPage.xaml:182-194`). Es la página «Condiciones relacionadas (if-then)», pestaña «Condiciones sencillas».

**Qué hace:**
- El manual (`docs/MANUAL_USUARIO.md:351`) lo resume como «si se cumple A, entonces exige B».
- Motor, `CondicionIfThen.cs:284-299`: una relación se cumple si A no se da o si se da B.
- «Cantidad de X: 4» es exactamente 4 equis: valor único, `Convert.ToInt16` + `SetNoEquis(int)`
  (`CondicionIfThen.cs:102-115`).
- «Condiciones que deben cumplirse» es cuántas relaciones han de cumplirse, como número exacto, lista o rango
  (`ControladorIfThen.cs:57-72` y `277-298`).
- El If-Then es global (no va por grupos) y se evalúa después de las condiciones de los grupos (`Analizador.cs:83-103`;
  ver H1).

**Campos:**
- **«Condición genérica»** y **«Condición específica»**: desplegables.
- **«Valor»**: NumberBox de 0 a 14; escribir el número y pulsar Tab.
- **«Negado»**: ToggleSwitch; se deja apagado.
- **«Añadir»**: añade la relación a la lista.
- **«Condiciones que deben cumplirse»**: se actualiza al teclear.
- **«Aceptar»**: abajo.

### FC5: una relación

1. Nueva ▸ B3. Pulsa «Condiciones Relacionadas».
2. En el bloque **Si ...**:
   - Condición genérica: `Cantidad de signos`
   - Condición específica: `Cantidad de X`
   - Valor: `4`, y Tab.
3. En el bloque **Entonces ...**:
   - Condición genérica: `Cantidad de signos`
   - Condición específica: `Cantidad de 2`
   - Valor: `2`, y Tab.
4. Pulsa **Añadir**. En la lista aparece «Cantidad de X: 4 | Cantidad de 2: 2» y «Condiciones en lista: 1».
5. **Condiciones que deben cumplirse:** `1`.
6. Pulsa **Aceptar**. El botón de Inicio queda en verde.
7. Calcula y graba `FC5.txt`.

**Debe salir:** **159**. Hay 76 columnas con 4 equis; de ellas, 19 tienen 2 doses, así que se rechazan 57.

### FC6: dos relaciones, deben cumplirse 2

1. Nueva ▸ B3. En «Condiciones Relacionadas» añade dos relaciones:
   - **Relación 1:** Si `Cantidad de X` = `4` ⇒ `Cantidad de 2` = `3`. Pulsa Añadir.
   - **Relación 2:** Si `Cantidad de X` = `3` ⇒ `Cantidad de 2` = `4`. Pulsa Añadir.
2. **Condiciones que deben cumplirse:** `2`.
3. Pulsa Aceptar, calcula y graba `FC6.txt`.

**Debe salir:** **133**.

### FC7: las mismas relaciones, deben cumplirse 1

1. Sin pulsar Nueva, vuelve a abrir «Condiciones Relacionadas». Deben aparecer las dos relaciones.
2. Cambia «Condiciones que deben cumplirse» a `1`.
3. Pulsa Aceptar, calcula y graba `FC7.txt`.

**Debe salir:** **83**. Exige que se cumpla exactamente una relación, no «al menos una».

### FC8: If-Then y Variantes a la vez (comprueba H1)

1. Nueva ▸ B3.
2. Pon la relación de FC5 con «deben cumplirse» `1` y pulsa Aceptar.
3. Abre la tarjeta «Variantes X y 2»:
   - **Var:** `7,8,9`, y Tab.
   - Pulsa Aceptar.
4. Calcula y graba `FC8.txt`.

**Debe salir según el manual:** **95**, las dos condiciones a la vez. Con solo Variantes saldrían 148 y con solo el
If-Then, 159.

**El código predice 159** (H1): el If-Then sustituye al resultado de Variantes. Si sale 159, H1 queda confirmado en la
app.

**El oráculo** comprueba la lógica con asserts. El manual no trae ejemplos con números.

**Sin cubrir:**
- «Negado» en Si y en Entonces.
- Las demás condiciones genéricas: Dibujos, Signos Seguidos, Interrupciones, Pesos numéricos, Distancias y
  Contactos.
- La pestaña «Grupos», que necesita al menos 2 grupos de partidos.
- Guardar y Abrir `.if`, Copiar y Pegar, Borrar relación.

---

## 4. Pares (`ParejasFrm`): herramienta de fichero a fichero

**Cómo se llega:**
- Inicio ▸ tarjeta **«Formatos»** ▸ tarjeta «Utilidades de formatos» ▸ botón **«Pares»**
  (`FormatosFrmPage.xaml:146-149`, `FormatosFrmPage.xaml.cs:50-54`).
- No está en el panel lateral ni en el manual (H7).

**Qué hace** (no hay manual; se sigue el código, `ParejasFrmViewModel.cs:330-361`, réplica 1:1 de
`Free1X2/UI/ParejasFrm.cs:633-658`):
- Cada una de las 14 filas tiene una pareja de partidos (P1/P2) y un nivel de 1 a 7 para cada combinación de signos
  de la pareja (`11, 1x … 22`; 0 = ninguno).
- Para cada columna se cuenta cuántas filas caen en cada nivel.
- Cada nivel tiene un mínimo y un máximo (por defecto 0 y 10).
- Graba las columnas válidas sin repetidas, en orden con X < 1 < 2 (`:246-267`).
- Las casillas son números enteros; si no se pueden leer, cuentan como 0 (`AInt`).

### FC9

1. Abre «Pares» y pulsa **«Leer condiciones»** ▸ `entrada\FC_pares.par`. Comprueba en pantalla:

   | Fila | Par (P1 / P2) | Niveles |
   |---|---|---|
   | 1 | 1 / 4 | 11 `1`, 1x `2`, 12 `2`, x1 `2`, xx `1`, x2 `2`, 21 `2`, 2x `2`, 22 `1` |
   | 2 | 8 / 1 | Los mismos que la fila 1 |
   | 3 | 3 / 9 | x2 `3`, 21 `3`, el resto `0` |
   | 4–14 | 0 / 0 | Todo `0` |

   En «Condiciones»:

   | niv | min | max |
   |---|---|---|
   | 1 | 1 | 1 |
   | 3 | 1 | 1 |
   | Los demás | 0 | 10 |

   También se puede teclear a mano en lugar de leer el fichero.
2. Pulsa **Calcular** ▸ `entrada\FC_B3.txt`.
3. Pulsa **Grabar resultado** ▸ `salida\FC9.txt`.

**Debe salir:** Procesadas **216**, Válidas **48**, y el fichero con **48** columnas.

**Comprobación en pantalla:** con «Columna Ganadora» `111X11X222X111` (el valor por defecto), **Analizar** debe dar
en los niveles 1–7: **0, 2, 0, 0, 0, 0, 0**.

**Sin cubrir:** Salvar condiciones, Cancelar, niveles 4–7 con límites distintos de los de por defecto y líneas no
válidas en la entrada.

---

## 5. Tríos (`TriosFrm`): herramienta de fichero a fichero

**Cómo se llega:**
- Inicio ▸ «Formatos» ▸ «Utilidades de formatos» ▸ **«Tríos»** (`FormatosFrmPage.xaml:150-153`).
- No está en el panel lateral ni en el manual (H7).

**Qué hace** (código `TriosFrmViewModel.cs:406-455`, igual que `Free1X2/UI/TriosFrm.cs:1133-1176`):
- Es lo mismo que Pares pero con 12 filas de tríos (P1/P2/P3) y 27 combinaciones (`111 … 222`).
- Tiene 5 niveles con límites «Mín» y «Máx» (por defecto 0 y 270).
- Graba igual que Pares: sin repetidas y con X < 1 < 2.

### FC10

1. Abre «Tríos» y pulsa **«Abrir Condiciones»** ▸ `entrada\FC_trios.tri`. Comprueba en pantalla:
   - **Fila 1:** P1/P2/P3 = `1 / 4 / 8`. Las combinaciones con los tres signos iguales tienen nivel 1, las que tienen
     dos iguales nivel 2 y las que tienen los tres distintos nivel 3.
   - **Fila 2:** `3 / 6 / 9`. Nivel 4 si la combinación contiene un `2`; si no, nivel 5 si contiene una `X`; `111`
     queda en 0.
   - **Límites:**

     | Nivel | Mín | Máx |
     |---|---|---|
     | 3 | 0 | 0 |
     | 4 | 1 | 1 |
     | 1, 2 y 5 | 0 | 270 |

   Teclear las 54 casillas a mano no es práctico; mejor el fichero.
2. Pulsa **Calcular** ▸ `entrada\FC_B3.txt`.
3. Pulsa **Grabar Resultado** ▸ `salida\FC10.txt`.

**Debe salir:** Procesadas **216**, Válidas **126**, y el fichero con **126** columnas.

**Comprobación en pantalla:** **Analizar** da en «Resultado» de los niveles 1–5: **0, 0, 1, 1, 0**. Siempre analiza
`111X11X222X111`, porque no hay casilla para cambiar la columna (H8).

**Sin cubrir:** Salvar Condiciones y Cancelar.

---

## 6. Dependencia lineal (`FrmDependenciaLineal`): herramienta de fichero a fichero

**Cómo se llega:** panel lateral ▸ Utilidades ▸ **«Dependencia lineal»** (`MainWindow.xaml.cs:511`).

**Qué hace:**
- Manual (`docs/MANUAL_USUARIO.md:876-877`): recalcula el signo del partido a tratar como combinación lineal, módulo 3
  ó 2, de los signos de los demás partidos multiplicados por sus coeficientes.
- La codificación sale del original (`Free1X2/UI/FrmDependenciaLineal.cs:1589-1672`), igual en
  `FrmDependenciaLinealViewModel.cs:216-304` y `396-428`:
  - Los signos valen 1 = 0, X = 1 y 2 = 2.
  - Partido a tratar con **triple**: suma mód 3.
  - Con **1X**: suma mód 2 (sale 1 o X).
  - Con **X2**: suma mód 2, más 1 (sale X o 2).
  - Con **12**: suma mód 2, por 2 (sale 1 o 2).
- La salida va sin repetidas y ordenada por el número en base 3, con P1 como cifra de menor peso.
- Al elegir la entrada, la pantalla propone:
  - coeficiente **1** en los partidos que tienen los tres signos (en B3: P1, P4 y P8) y 0 en los demás;
  - como partido a tratar, el primero sin triple (P2), con 1X2 (`:358-390`).
- **Ojo:** si no se elige salida, la salida es el mismo fichero de entrada y lo sobrescribe (`:151-155`, igual que el
  original `:1527-1529`).

### FC11: partido a tratar con triple

1. Abre «Dependencia lineal».
2. En «Fichero de entrada» pulsa **Seleccionar…** ▸ `entrada\FC_B3.txt`. El estado debe decir «Seleccionar partido e
   indicar coeficientes».
3. En «Fichero de salida» pulsa **Seleccionar…** ▸ `salida\FC11.txt`. **Antes de Aceptar.**
4. En la fila **P1** pulsa **«Tratar este»**. Se encienden 1, X y 2.
5. Pon los coeficientes, cada uno con Tab:
   - **P3** = `1`
   - **P4** = `2`
   - **P9** = `1`
   - **P8** se queda en `1`.
   - El resto se queda en 0. El coeficiente de P1 no cuenta, porque es el partido a tratar.
6. Pulsa **Aceptar**.

**Debe salir:** el estado «Se han grabado **72** columnas» y el fichero con **72**.

### FC12: segunda pasada con doble 12 (comprueba H9)

1. Sigue en la misma pantalla. **No** vuelvas a elegir la entrada.
2. En «Fichero de salida» pulsa Seleccionar… ▸ `salida\FC12.txt`.
3. En la fila **P6** pulsa **«Tratar este»** y apaga la **X** de P6, de modo que queden 1 y 2.
4. Deja los coeficientes como estaban: P1 `1`, P3 `1`, P4 `2`, P8 `1`, P9 `1`.
5. Pulsa **Aceptar**.

**Debe salir según el original** (que relee la entrada, `FC_B3.txt`): **108** columnas.

**El código WinUI predice 36**, porque toma `FC11.txt` como entrada (H9). Si sale 36, H9 queda confirmado.

**Sin cubrir:**
- Partido a tratar con 1X o con X2. Están en el oráculo (`o_dl`), pero no hay prueba en la app.
- La vista de porcentajes, que no está portada (`FrmDependenciaLinealViewModel.cs:296`).

---

## 7. Hallazgos

Ninguno está corregido. Lo marcado «código» sale de leer el código y queda por confirmar con la prueba indicada.

**H1. If-Then: anula las demás condiciones al calcular** (código; heredado del original). Se confirma con FC8.
- En `Free1X2.Domain/MotorCalculo/Analizador.cs:97-102`, `AnalizaColumna(long)` hace
  `columnaValida = IfThen.CompruebaPronostico(...)`.
- Eso **sustituye** el resultado de las condiciones de los grupos, en vez de combinarlo con un Y.
- La vía de análisis (`Analizador.cs:146-151`) sí hace el Y.
- El original hace lo mismo (`git show 04305c9:Free1X2/MotorCalculo/Analizador.cs`, líneas 93-99 frente a 131-136).
- El manual (`MANUAL_USUARIO.md:351`, «exige B») y `MANUAL_FLUJOS.md:131-136` y `:200` («si la columna pasó todo»)
  describen un Y.
- **Efecto:** con un If-Then activo, Calcular ignora Variantes, CP y el resto de condiciones.
- Ningún test de `Free1X2.Domain.Tests` comprueba el If-Then: solo `QuitarGrupoTests.cs:91` lo carga.
- Por R1 no se toca sin decisión del dueño.

**H2. Columnas probables: los rangos con guion dan error** (código).
- `ColumnaProbable.SetNoAciertos` (`ColumnaProbable.cs:376-400`) parte el texto solo por comas y hace `Convert.ToInt32`.
- `SetNoAciertosSeguidos`, `SetNoFallosSeguidos` y `SetACTol`/`SetACSTol`/`SetFSTol` funcionan igual.
- `ColProbablesFrmViewModel.cs:443-451` les pasa el texto tal cual, así que `10-12` lanza un error.
- Lo recoge `App.UnhandledException` (`App.xaml.cs:28`) y la CP no se aplica.
- Sin embargo, el ejemplo gris del campo dice «Ej. 0-14 ó 3,5,7» (`ColProbablesFrmPage.xaml:60`) y el manual §6
  (`MANUAL_USUARIO.md:277`) dice que se admiten rangos `2-6`.
- El original usaba casillas para marcar (`OptionNumTol0_14`, `Free1X2/UI/Filtros/ColProbablesFrm.cs:38-43`) y no
  tenía este problema.
- Para probarlo: en FC1 escribe `10-12` en «Aciertos»; debe salir un aviso de error.

**H3. Columnas probables: dobles y triples imposibles; «Todos a Triple» y las CP importadas se estropean al Aceptar**
(código). Se confirma con FC13.
- La casilla «Pronóstico» admite un signo simple por partido (`MaxLength=14`, `ColProbablesFrmPage.xaml:41-45`).
- Al mostrar una CP se concatenan los textos de sus partidos (`ColProbablesFrmViewModel.cs:345-351`): 14 triples
  se ven como `1X21X2…`.
- Al guardar se relee la casilla carácter a carácter (`:429-435`). Por eso, al Aceptar, una CP de triples o de
  dobles que esté en pantalla pasa a ser `1X21X21X21X21X`.
- Le pasa igual a una CP que llegue del Importador.
- Un partido sin signo a mitad de la CP (por ejemplo `1-1`) se ve como `11` al volver a abrirla y, si se acepta otra
  vez, queda corrido.
- El original tenía un control 1/X/2 por partido (`ColProbablesFrm.cs:225` y `323-346`).
- Los ejemplos del manual §7 con dobles (37.888, etc.) no se pueden reproducir en Windows.

**H4. Grupos de equipos: el texto de ayuda contradice al motor** (código).
- `GruposEquiposFrmPage.xaml:63` dice «Marcar ambos equivale al empate (X)», y lo mismo el comentario de
  `GruposEquiposFrmViewModel.cs:20-23`.
- El motor cuenta los dos equipos (`GrupoEquipos.cs:75-149`, caso 3): si gana uno, 1 victoria y 1 derrota; si
  empatan, 2 empates.
- FC3 y FC4 usan «los dos» en P8.

**H5. Grupos de equipos: los nombres salen de la jornada online, no del boleto de Inicio** (código; solo afecta a lo
que se ve).
- `GruposEquiposFrmViewModel.cs:264-275` toma los nombres de `AppState.JornadaActual`.
- El manual (`MANUAL_USUARIO.md:318`) dice que aparecen «si has cargado el boleto».
- El original los leía del boleto.
- Sin jornada descargada, se ve «Equipo casa N».

**H6. If-Then: «Condiciones que deben cumplirse» no está documentado** (código).
- Es un número exacto, una lista o un rango: con 2 relaciones y `1`, exige que falle exactamente una (FC7).
- Un número mayor que el de relaciones lanza un error de índice (`ControladorIfThen.cs:66-70`, `bool[Count+1]`).
- «Valor» es un número exacto, no un rango.
- El manual (`MANUAL_USUARIO.md:351`) no explica nada de esto.

**H7. Pares y Tríos no están en el manual ni en el panel lateral.**
- Solo se llega desde Inicio ▸ Formatos ▸ «Utilidades de formatos» (`FormatosFrmPage.xaml:142-153`).
- No están en `SeccionesLateral` (`MainWindow.xaml.cs:454-529`).

**H8. Tríos: «Analizar» no deja elegir la columna** (código).
- No hay casilla «Columna Ganadora» en `TriosFrmPage.xaml`, así que siempre se analiza `111X11X222X111`, el valor por
  defecto de `TriosFrmViewModel.cs:206`.
- El original la tenía (`TriosFrm.cs:248` y `1222`).
- Pares sí la tiene.

**H9. Dependencia lineal: tras grabar, toma la SALIDA como entrada** (código). Se confirma con FC12.
- `FrmDependenciaLinealViewModel.cs:293-296` llama a `LeerColumnas(_rutaSalida)`, que sustituye las columnas cargadas.
- El original relee la **entrada** (`FrmDependenciaLineal.cs:1540`, llamado desde `GrabarColumnas`, `:1691`).
- Si se acepta una segunda vez sin volver a elegir la entrada, se trabaja sobre el resultado anterior.

**H10. Nota del manual desfasada.**
- `MANUAL_USUARIO.md:879` dice que Dependencia lineal (y Multiplicador, Fraccionador, Transposición, Álgebra,
  Reductor…) tienen la lógica «todavía en proceso de migración».
- El cálculo y el grabado de Dependencia lineal están completos (`FrmDependenciaLinealViewModel.cs:216-304` y
  `431-446`).
- O1–O4 de `PRUEBAS_WINDOWS.md` ya prueban Álgebra, Transposición, Multiplicador y Fraccionador.

**H11. Relaciones I de Columnas probables: mezclar separadores da error** (código, menor, sin probar).
- En «Suma Aciertos» y «Columnas», `7,9-11` o una lista desordenada como `9,7` da error.
- `RelacionCP1.InicializarSumas` (`RelacionCP1.cs:311-345`) solo mira el primer separador y dimensiona con el último
  valor.
