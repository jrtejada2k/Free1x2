# Resultados de las pruebas funcionales · 1-nov-2026

App probada: **Free1X2 1.1.0+8745e5f** (la `main` de ese día), recién descomprimida en una carpeta aparte, manejada con
el ratón y el teclado como un usuario, siguiendo [`docs/PRUEBAS_WINDOWS.md`](../PRUEBAS_WINDOWS.md). Cada fichero que
grabó la app se comparó con lo que calcula, por su cuenta, [`scripts/pruebas_windows.py`](../../scripts/pruebas_windows.py)
(salida completa en [`evidencias/verificar-2026-11-01.txt`](evidencias/verificar-2026-11-01.txt)).

**Resumen: 36 pruebas con fichero bien, 4 mal.** Además, todas las pruebas de pantalla (números que se leen, no se graban)
salieron como se esperaba, salvo las anotadas abajo.

## Segunda ronda (mismo día, tras los arreglos)

Salida completa en [`evidencias/verificar-2026-11-01-ronda2.txt`](evidencias/verificar-2026-11-01-ronda2.txt):
**41 bien, 2 mal** (I5 y O4, los dos heredados del original, pendientes de decisión del dueño por la regla R1).

- **Arreglado · «Calcular» desde el panel lateral, el menú o F5** (`CalculaColumnasFrmViewModel.cs`): usa siempre la
  combinación de Inicio y su filtro de columnas. I4 da ya **500 procesadas / 136 aceptadas**
  ([captura](evidencias/I4-arreglado.jpg)), igual que desde la barra de Inicio; C1 y C2 se calcularon desde el panel y
  C3 con **F5**.
- **Arreglado · Filtro Pim** (`Free1X2.Domain/Analisis/FiltroPim.cs` + `GeneraPimViewModel.cs`, tests
  `FiltroPimTests`): 7 rankings sin suma, como el original. F5 da **1 admitida** (`111XXX11X1XX12`) y «Analizar»
  R1–R7 = 3, 3, 2, 3, 2, 0, 1, recorrido 3 ([captura](evidencias/F5-pim-arreglado.jpg)).
- **Las condiciones filtran** (C1–C3 de la lista): boleto de 3 triples, 3 dobles y 8 fijos (216 columnas).
  Interrupciones ⇒ **120**, Signos Seguidos ⇒ **114**, las dos ⇒ **54**. El script comprobó columna a columna que
  **ninguna** grabada incumple sus condiciones y que **no falta ninguna** que las cumpla. Con C1.txt como filtro de
  columnas y las dos condiciones, desde el panel: 120 procesadas y las mismas 54 (C4).
- Calidad: build 0 errores · **205/205** tests (138 golden-master intactos) · smoke **111/111**.

## Fallos confirmados en la app

| # | Prueba | Qué pasa | Esperado | Dónde (código) |
|---|---|---|---|---|
| 1 | **I4** | «Calcular» desde el **panel lateral**, el menú Combinación o **F5** ignora el boleto, las condiciones y el filtro de Inicio: calcula y graba los **4.782.969** de 14 triples ([captura](evidencias/I4-calcular-desde-panel.jpg)). | 136 (como desde el botón «Calcular» de la barra de Inicio) | `MainWindow.xaml.cs:221,459,713` navegan sin pasar el boleto; solo `MainPageViewModel.cs:524` lo pasa; la página usa entonces 14 triples propios (`CalculaColumnasFrmViewModel.cs:149-159`) |
| 2 | **F5** | **Filtro Pim** usa el algoritmo de Aidomnou: Admitidas **0**; su «Analizar» da R1–R7 = 14, 8, 13, 10, 7, 7, 59 ([captura](evidencias/F5-pim-admitidas-0.jpg)). | Admitidas 1 (`111XXX11X1XX12`); R1–R7 = 3, 3, 2, 3, 2, 0, 1 | `GeneraPimViewModel.cs:15` («clase legacy aidomnou») frente a `Free1X2/UI/FiltroPim.cs:221-248,448-466`. La Mac copió lo mismo. |
| 3 | **I5** | Una **línea en blanco** en el fichero de filtro se convierte en la columna `11111111111111`. | 3 columnas | `ArchivoColumnasTexto.cs:216` + `GeneradorColumnas.cs:289` |
| 4 | **O4** | **Fraccionador por tramos** (10 líneas en 4 tramos) graba 4 ficheros de 2: **se pierden 2 columnas**, y la pantalla dice «Columnas: 10». También deja un `O4_.txt` vacío. | 10 líneas entre los 4 ficheros | `FraccionadorFrmViewModel.cs:195-214` (heredado del original, `FraccionadorFrm.cs:153-163`) |

## Otros problemas vistos (menores o de pantalla)

- **«Filtro…» con un filtro cargado lo quita** sin avisar ni abrir el selector; el botón sigue diciendo «Filtro…» (I6, `MainPageViewModel.cs:470-475`).
- **Rangos en las condiciones:** el manual (§6) dice que vale `2-6`, pero Variantes responde «The input string '3-6' was not in a correct format» (en inglés). Hay que escribir `3,4,5,6`. Ni Windows ni la Mac aceptan rangos ahí: **el manual está mal**.
- **Selección múltiple de ficheros:** una vez, al elegir 3 ficheros de la carpeta temporal en «Combinar filtros», el selector lanzó una COMException (`PickerHelper.cs:48`) y el botón dejó de responder hasta reiniciar. Con 2 ficheros de otra carpeta funcionó. No reproducido del todo.
- **Coincidencias:** «Índice AMC» solo ofrece «1» con dos condiciones (el original permitía elegir el grupo).
- **Aidomnou:** la tarjeta «Columnas Probables» es estrecha y **corta los límites de C5 y C6** (solo se llega con Tab).
- **Calcular columnas:** las etiquetas de «Resultados» se parten letra a letra («Pro ces ad as») por falta de ancho.
- **Nueva combinación (Ctrl+N)** borra los equipos, pero la pantalla no lo muestra hasta volver a Inicio.
- **Escrutinios → Posibles premios → volver:** se pierden la ganadora y los resultados (en el original era otra ventana).
- **Grupos:** el pie dice «1 grupo» al volver de Control de grupos aunque se acababa de crear el 2.º; el grupo se borra sin pedir confirmación.
- **Banco de pruebas:** el análisis pide «Genera primero las columnas aleatorias (paso 3)» aunque el origen sea «Fichero»; en autoescrutinio, «Premio total», «% recuperado» y «Veces premiada» salen en blanco.
- **Clic muy rápido en «Aceptar»** de una condición (sin salir antes del campo) puede no guardar lo escrito: con un clic de persona sí se guarda.

## Lo que funciona bien (comprobado con datos)

| Área | Pruebas | Resultado |
|---|---|---|
| Inicio + condiciones + filtro (botón de la barra) | I1, I2, I3 | Mismas columnas que el cálculo independiente (2 · 1.435 · 136) |
| W2 valor no válido | `a` en Variantes | Error y la condición queda como estaba |
| Reductor (3 métodos) y W10 | R1a/b/c, R2, R3 | Garantía cumplida; «Columnas de Inicio» aplica las condiciones (1.435) |
| Máscaras e históricos (W9, W12, W20, W22) | JD01 vs España | 2.659 jornadas; 14: 16 · 13: 264 · 12: 2.071 · 11: 10.374 · 10: 37.243 · 1.568 con premio: **idéntico** al cálculo independiente |
| Combinar filtros | F1a–e | 5/5 |
| Diferencias entre filtros | F2a–d + Analizar | 4/4; (14,13) = (1,2) (1,2) (1,0) |
| Coincidencias | F3a–c | 3/3, con s/AC y AMC correctos |
| Aidomnou | F4 | Válidas 2; rejilla C1–C6 idéntica a la esperada |
| Escrutinios (#5, #9) | E1, E3, W17 | 177.147 columnas: 1 · 22 · 220 · 1.320 · 5.280 y las 6.843 premiadas guardadas; Posibles premios abre con la ganadora |
| Operaciones | O1 (4 tipos), O2, O3, O5a/b, O6a/b | 10/10 |
| Esperanza (W14) + Pegar desde Excel (W15) | ejemplo del Excel | EM 0,3394 · 0,2600 · 0,1998 · 0,1451 · 0,1296 · total 1,0739 € · **107,4 %** ([captura](evidencias/W14-esperanza-107-4.jpg)) |
| Precio configurable (W15, W16) | Configuración, Calcular, Ordenar, Banco | 0,75 € por defecto; costes y gasto con 0,75 |
| Ordenar por probabilidad (W18) | W18a/b | 1.000–2.000 ⇒ `1…1`; 2.000.000–3.000.000 ⇒ `2…2X` |
| Banco de pruebas (W19) | autoescrutinio, orden, filtro, grabar | Nºacumulado 2,2,1; orden por Nº13 en los dos sentidos; «2 filas seleccionadas»; graba las 2 |
| Progol (W21) y Jornada (W6, W7, #6) | México / España | Jornada 2357 de México; coste a 15 $ (71.744.535 $); aviso «Jornada del 01/11/2026» |
| Otros | W3, W4, W5, W8, #15 | Paneles de Rotación; «CPs por diferencias…» abre el generador; nota de Aidomnou sin `aidomnou.cfg`; buscador del panel; borrar grupo controlado no da error |

No probado a mano: W1 y W11 (enlaces de ayuda, abren el navegador) y W13 (icono, ya comprobado dentro del exe en 1.1.0).
