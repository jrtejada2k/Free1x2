# Pruebas funcionales de Free1X2 para Windows

Lista para probar la app **como un usuario**, con columnas y resultados concretos, y comprobar que cada pantalla da la
salida correcta. Cubre lo básico (Inicio, condiciones, filtro, Calcular, Reducir, Escrutar), las pantallas de
filtros y de operaciones, y todo lo añadido para igualar la app Mac (W1–W22 de [`TAREAS_WINDOWS.md`](TAREAS_WINDOWS.md))
y los arreglos de octubre (#4–#15 en GitHub).

**Cómo se sabe qué debe salir.** El script [`scripts/pruebas_windows.py`](../scripts/pruebas_windows.py) calcula cada
resultado por su cuenta, en Python y por fuerza bruta (desarrolla el boleto y aplica las reglas del manual, o las del
WinForms original donde el manual no llega). No usa el motor de la app. Si la app graba lo mismo que el script, la
prueba pasa.

## 0. Preparación

1. Usa una carpeta de la app **recién descomprimida** (para que arranque como una instalación nueva) y una carpeta de
   trabajo aparte, por ejemplo `C:\Pruebas`.
2. Crea las entradas:

   ```bash
   python scripts/pruebas_windows.py preparar C:\Pruebas
   ```

   Deja los ficheros en `C:\Pruebas\entrada\` (también están en [`docs/pruebas/entrada/`](pruebas/entrada), menos el grande
   `E3_once_triples.txt`, que solo crea este comando) y crea `C:\Pruebas\salida\`.
3. **Cada vez que la app grabe un fichero**, guárdalo en `C:\Pruebas\salida\` con el **nombre de la prueba** (`I1.txt`,
   `F2a.txt`…). Al final, o cuando quieras:

   ```bash
   python scripts/pruebas_windows.py verificar C:\Pruebas
   ```

   Saca una línea por prueba: `OK`, `MAL` (con lo que sobra o falta) o «sin salida».
4. `python scripts/pruebas_windows.py esperado` lista lo que debe salir en cada prueba.
5. Los valores de **«Pegar desde Excel»** están en `entrada\val_60_25_15.txt`, `val_50_30_20.txt`, `val_34_33_33.txt`,
   `esperanza_apostados.txt` y `esperanza_reales.txt`: ábrelos con el Bloc de notas, **Ctrl+A, Ctrl+C**, y pulsa
   «Pegar desde Excel» en la tabla.

**Boletos de las pruebas** (P = partido):

| Boleto | Pronóstico | Columnas |
|---|---|---|
| **B1** | P1–P10 `1` · P11 `1X` · P12 `X2` · P13 `1X2` · P14 `1` | 12 |
| **B2** | P1–P6 `1X2` · P7 `1X` · P8 `X2` · P9–P14 `1` | 2.916 |
| **JD-01** | P1–P6 `1X2` · P7–P14 `1` | 729 |

---

## 1. Inicio: boleto, condiciones, filtro y Calcular

En Inicio, cada partido tiene los botones `1`, `X`, `2`. Las condiciones se abren con su tarjeta en «Condiciones del
grupo» y se cierran con **Aceptar**. **Calcular** es el botón de la **barra de arriba de Inicio**. En la pantalla de
cálculo, elige **«Grabar archivo»** ▸ «Seleccionar archivo...» ▸ `salida\<prueba>.txt` ▸ **Calcular**.

| Prueba | Pasos | Debe salir |
|---|---|---|
| **I1** | Nueva combinación ▸ boleto **B1** (cabecera «Fijos: 11 - Dobles: 2 - Triples: 1») ▸ «Variantes X y 2»: Var `1,2` ▸ «Signos Seguidos»: 1 = `10` ▸ Calcular ▸ Grabar `I1.txt` | Procesadas **12**, Aceptadas **2**: `1111111111XX11`, `1111111111X211` |
| **I2** | Nueva ▸ boleto **B2** ▸ Variantes: Var `3,4,5,6`, X `1,2,3,4` ▸ Signos Seguidos: Var `1,2,3` (los rangos con guion no se admiten aquí) ▸ Calcular ▸ Grabar `I2.txt` | Procesadas **2.916**, Aceptadas **1.435** |
| **I3** | I2 + tarjeta «Filtro de columnas» ▸ «Filtro…» ▸ `entrada\filtro_medio.txt` (semáforo verde) ▸ Calcular (barra de Inicio) ▸ Grabar `I3.txt` | Procesadas **500**, Aceptadas **136** (las repetidas del filtro se conservan) |
| **I4** | Igual que I3 pero abriendo **«Calcular columnas» desde el panel lateral** (o el menú Combinación ▸ Calcular…, o F5) ▸ Grabar `I4.txt` | Lo mismo que I3: **136**. (Hasta el 1-nov-2026 estas vías calculaban los 4.782.969 de 14 triples sin boleto, condiciones ni filtro.) |
| **I5** | Nueva (14 triples, sin condiciones) ▸ Filtro…: `entrada\filtro_con_blanco.txt` (3 columnas y una línea en blanco) ▸ Calcular ▸ Grabar `I5.txt` | **3** columnas. ⚠ Revisión: la línea en blanco podría convertirse en `11111111111111` (`GeneradorColumnas.cs:289`) |
| I6 | Con el filtro cargado, pulsa otra vez «Filtro…» | Debe abrir el selector. ⚠ Revisión: lo **quita** sin avisar (`MainPageViewModel.cs:470-475`) |
| I7 | En «Variantes X y 2» escribe `a` en Var ▸ Aceptar (W2) | Mensaje de error; la tarjeta **no** queda activa y vacía |

### Las condiciones filtran (Interrupciones y Signos Seguidos)

Boleto **B3**: P1 `1X2` · P2 `1` · P3 `X2` · P4 `1X2` · P5 `1` · P6 `1X` · P7 `X` · P8 `1X2` · P9 `12` · P10 `1` · P11 `2` ·
P12 `1` · P13 `X` · P14 `1` (3 triples, 3 dobles, 8 fijos: **216** columnas). Calcula con «Grabar archivo». El script
comprueba, además del recuento, que **cada** columna grabada cumple sus condiciones y que no falta ninguna que las cumpla.

| Prueba | Condiciones | Debe salir |
|---|---|---|
| **C1** | «Interrupciones»: Global `9,10` · «2» `2,3` · Seguidas Global `4,5,6` · Seguidas Var `2,3` | **120** de 216 |
| **C2** | Solo «Signos Seguidos»: «1» `1,2` · Var `2,3` (Interrupciones borradas con «Borrar») | **114** |
| **C3** | Las dos a la vez | **54** |
| C4 | C3 + «Filtro…» = `salida\C1.txt`, calculando desde el panel lateral | Procesadas **120**, aceptadas **54** (las de C3) |

Qué es cada cosa (manual §6 y el WinForms original): una **interrupción** es cada cambio de signo entre dos partidos
seguidos. «1», «X», «2» cuentan los que salen de ese signo y «Var» los que van de X o 2 a 1. «Seguidas» es la racha más
larga de cambios consecutivos. En **Signos Seguidos**, cada campo limita la racha más larga de ese signo (o de
variantes, X o 2).

## 2. Reductor (Inicio ▸ Reducir, W10)

| Prueba | Pasos | Debe salir |
|---|---|---|
| **R1a/b/c** | Panel ▸ Reducir ▸ «Archivo entrada»: `entrada\R1_9.txt` (9 columnas, 2 triples) ▸ Nivel **13** ▸ Método **menos tiempo** (a), **menos columnas** (b), **grandes archivos** (c) ▸ Reducir ▸ «Guardar resultado…» `R1a.txt`/`R1b.txt`/`R1c.txt` | «Col. Iniciales = 9», «Admitidas = 3». El script comprueba que **toda** columna de entrada tiene ≥ 13 aciertos con alguna de la salida |
| **R2** | Boleto **B2** con las condiciones de I2 ▸ **Reducir** en la barra de Inicio ▸ el Reductor abre con las Columnas de Inicio (1.435) ▸ nivel **12**, menos tiempo ▸ Reducir ▸ Guardar resultado `R2.txt` | Garantía al 12 sobre las 1.435; salida ⊆ entrada |
| **R3** | Boleto **JD-01** ▸ Reducir ▸ nivel **13**, menos columnas ▸ Reducir ▸ Guardar `R3.txt` | Entre **73 y 729** admitidas; garantía al 13 sobre las 729 |

## 3. Filtros

| Prueba | Pasos | Debe salir |
|---|---|---|
| **F1a–e** Combinar filtros | «Cargar Filtro» ▸ `F1_A.txt`, `F1_B.txt`, `F1_C.txt` ▸ Iniciar Proceso (Cols. = 4, 3, 3) ▸ Mínimo/Máximo ▸ «Sumar cols.» ▸ «Grabar cols.» | **a** 2–3 ⇒ 3 · **b** 1–1 ⇒ 2 · **c** 3–3 ⇒ 1 · **d** 2–99 ⇒ 3 · **e** desmarca B, Iniciar otra vez, 2–3 ⇒ 1 |
| **F2a–d** Diferencias entre filtros | Cargar `F2_1.txt` y `F2_2.txt` ▸ en la fila 2: Difs y Mín. ▸ Iniciar Proceso ▸ Grabar | **a** Difs `1100` Mín 1 ⇒ 3 · **b** `0010` ⇒ 2 · **c** `1000` ⇒ 1 · **d** a + `F2_3.txt` con `0100` Mín 2 ⇒ 1. Con d: «Fichero…» `F2_G.txt` ▸ Analizar ⇒ (14, 13) = (1,2) (1,2) (1,0) |
| **F3a–c** Coincidencias | «Seleccionar fichero…» `F3_E.txt` ▸ Ganadora anterior: `F3_GA.txt` ▸ condiciones ▸ Calcular ▸ Grabar resultado | **a** `1-9` / `0-9` ⇒ 5 · **b** `1-9` / `5-9` ⇒ 4 · **c** b + `1-3` / `0-3` ⇒ 4. Ganadora reciente `F3_GR.txt` ▸ Analizar con `1-9;0-9` ⇒ Rsl. **5** |
| **F4** Aidomnou | Valoraciones: pegar `val_60_25_15.txt` ▸ límites C1–C5 `0-14`, C6 `10-14`, suma `60-84`, recorrido `0-14` ▸ Calcular ▸ `F4_entrada.txt` ▸ Grabar Resultado `F4.txt` | Procesadas **5**, Válidas **2**: `11111111111111`, `1111111111111X` |
| **F5** Pim | Pegar `val_60_25_15.txt` ▸ rangos por defecto ▸ Calcular ▸ `F5_entrada.txt` ▸ Grabar `F5.txt` | Admitidas **1**: `111XXX11X1XX12` (aciertos por ranking 3,3,2,3,2,0,1; recorrido 3). (Hasta el 1-nov-2026 aplicaba el algoritmo de Aidomnou y daba 0.) |

## 4. Escrutinios y premios

| Prueba | Pasos | Debe salir |
|---|---|---|
| **E1** | Escrutinios ▸ «Escrutinio simple» ▸ Columna ganadora `1X21X21X21X21X` ▸ Registrar premiadas: Sí ▸ «Seleccionar ficheros…» `E1_A.txt` ▸ Escrutar ▸ «Ver premiadas…» ▸ Guardar Todas `E1.txt` | Fila: 14 = **1**, 13 = **1**, Ac. Totales **32** · premiadas: la 1 (14) y la 3 (13) |
| E2 | «Contra fichero» ▸ referencia `E2_R.txt` ▸ ficheros `E1_A.txt` ▸ Escrutar | Fila 1: 14 = 1, 13 = 1 · Fila 2: 14 = 1 · TOTALES: 14 = 2, 13 = 1 |
| **E3** | Simple ▸ ganadora `1X21X21X21X111` ▸ fichero `E3_once_triples.txt` (177.147 columnas) ▸ Escrutar ▸ Ver premiadas… ▸ Guardar Todas `E3.txt` | 14: **1** · 13: **22** · 12: **220** · 11: **1.320** · 10: **5.280** (6.843 premiadas; prueba los arreglos #5 y #9) |
| E4 (W17) | Con la ganadora de E1 escrita ▸ «Posibles premios» | La pantalla abre con `1 X 2 1 X 2 …` en las casillas |

## 5. Operaciones

| Prueba | Pasos | Debe salir |
|---|---|---|
| **O1-0…3** Álgebra | Combinación 1 `O1_F1.txt`, Combinación 2 `O1_F2.txt` ▸ cada tipo de operación ▸ Resultado `O1-0.txt`… ▸ Calcular | 0 ⇒ 3 · 1 ⇒ 4 · 2 ⇒ `2…2` dos veces (como el original) · 3 ⇒ `1…1` y `X…X` |
| **O2** Transposición | Entrada `O2_entrada.txt` ▸ Salida 1…14 = `14,13,…,1` ▸ Transponer ▸ `O2.txt` | `2X1111111112X1` |
| **O3** Multiplicador | Comb-1 `O3_E1.txt`, Comb-2 `O3_E2.txt` ▸ plantilla por defecto ▸ Multiplicar ▸ Grabar `O3.txt` | 2 columnas: `11111XXXXXXX11`, `22222XXXXXXX22` |
| **O4** Fraccionador | Entrada `O4_diez.txt` ▸ Por **tramos**, 4 ▸ Fraccionar ▸ base `O4_` | `O4_01…04.txt` juntos = las **10** líneas. ⚠ Revisión: se perderían 2 (`FraccionadorFrmViewModel.cs:195-214`, heredado del original) |
| **O5a/b** Subir categoría | Origen `O5_origen.txt` ▸ todos los partidos ▸ Seguidos 0 ▸ niveles **1** (a) / **0 y 1** (b) ▸ Calcular ▸ Grabar | a: **28** · b: **29** |
| **O6a/b** Rotación de signos | Entrada `O6_entrada.txt` ▸ partido 1: «1 →» `X`, el resto igual ▸ corrección Desactivada (a) / Activada (b) ▸ Aceptar | a: `X111…`, `XX11…` · b: 4 columnas |

## 6. Lo añadido para igualar la Mac (W1–W22)

| Prueba | Pasos | Debe salir |
|---|---|---|
| W7 | Arrancar la app con Internet | Inicio con los equipos de la jornada (en 3 s como máximo) |
| W6 | Inicio ▸ «Jornada ▾» ▸ México, luego España | Los equipos cambian; los signos no |
| W8 | Panel lateral ▸ buscar `algebra` | Aparece «Álgebra de columnas» |
| **W9 / W20 / W22** | Boleto **JD-01** ▸ «Máscaras ▾» ▸ Guardar como `JD01` ▸ Nueva ▸ Máscaras ▾ «JD01 · 729 col.» ▸ panel Máscaras ▸ liga **España** ▸ Escrutar | Al aplicar: «Fijos: 8 - Dobles: 0 - Triples: 6». La liga España trae **2.659** jornadas (México **1.579**). Premios: los que da `python scripts/pruebas_windows.py mascara <app>\Historico\España.txt` |
| **W14 / W15** Esperanza | Panel ▸ Esperanza matemática ▸ columna `1121121221112X` ▸ Recaudación `3153494,25`, Precio **1** ▸ pegar `esperanza_apostados.txt` y `esperanza_reales.txt` ▸ Calcular | EM 14 **0,3394** · 13 **0,2600** · 12 **0,1998** · 11 **0,1451** · 10 **0,1296** · «EM total: 1,0739 € … el **107,4 %**». Con precio 0,75: **119,2 %** y 4.204.659 apuestas |
| W15 | Configuración ▸ Parámetros LAE | Precio apuesta **0,75**. Estimación de premios abre con 0,75 |
| **W18** | Ordenar por probabilidad ▸ entrada `W18_entrada.txt` ▸ pegar `val_50_30_20.txt` ▸ recaudación 15.000.000, precio 0,5, %14 15, «Por productos» LN central 0 ▸ «Obtener intervalo» 1.000–2.000 ▸ grabar `W18a.txt`; 2.000.000–3.000.000 ▸ `W18b.txt`; 0–1.000 ▸ ninguna | a: `11111111111111` · b: `2222222222222X` · c: 0 columnas |
| **W19** | Banco de pruebas ▸ `W19_banco.txt` ▸ pegar `val_34_33_33.txt` (apostados y reales) ▸ paso 3: origen «Fichero» = `W19_banco.txt` ▸ «Generar columnas» ▸ paso 4: tipo «Autoescrutinio», 14 y 13 marcados ▸ Analizar ▸ pestaña de la tabla ▸ ordenar por Nº13 (la 3.ª al final; otra pulsación, al principio) ▸ «Filtrar…» Nº de veces 13 entre 1 y 1 ▸ Grabar seleccionadas `W19.txt` | Nºacumulado 2, 2, 1 · «2 filas seleccionadas» · `11111111111111`, `X1111111111111` |
| W21 | Configuración ▸ «Parámetros Progol (México)» | 15 · 33 · 15.000.000 · `$`. Con «Jornada ▾ México», el coste de Calcular va en `$` a 15 por columna |
| W3 | Rotación de signos | Paneles «% Combinación», «Valoraciones jornada» y «Transposición automática» |
| W1 / W5 / W11 / W4 | Ayuda de Variantes (dice «comas») · Aidomnou (sin nota de `aidomnou.cfg`) · enlaces de ayuda · Diferencias entre columnas ▸ «CPs por diferencias…» | Cada uno como dice; «CPs por diferencias…» abre el generador |
| W13 | Barra de tareas y ventana | Icono azul «FREE 1X2» |

## 7. Arreglos de octubre (GitHub #4–#15)

| Prueba | Pasos | Debe salir |
|---|---|---|
| #4 / #10 | R3 con «menos columnas» | Termina sin agotar la memoria; garantía cumplida |
| #5 / #9 | E3 | Premiadas a disco (más de 1.000), totales exactos |
| #6 | Jornada ▾ España | El aviso dice la fecha de la jornada, no «Jornada 2026…» |
| #11 / #12 / #13 | Probabilidades, Diferencias entre filtros, Coincidencias, Rentabilidad / Ordenar / Banco / Tramificar | Mismos resultados que arriba (F2, F3, W18, W19) |
| #15 | Inicio ▸ crea un 2.º grupo ▸ Control de grupos que lo nombra ▸ borra ese grupo ▸ Calcular | Calcula sin error y sin apuntar a otro grupo |

## 8. Cómo se registra

Para cada prueba: **OK** o **MAL**, el fichero de `salida\` (si graba) y una captura de pantalla si muestra números.
El informe de una ronda de pruebas va en `docs/pruebas/RESULTADOS_<fecha>.md`. Primera ronda:
[`RESULTADOS_2026-11-01.md`](pruebas/RESULTADOS_2026-11-01.md).
