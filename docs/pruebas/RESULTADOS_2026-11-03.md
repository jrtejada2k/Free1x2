# Resultados de las pruebas funcionales · 3-nov-2026

App probada: **Free1X2 1.1.0+3ffa8b9** (la `main` de ese día, con los arreglos de filtros del 2-nov), publicada en una
carpeta aparte y manejada con el ratón y el teclado como un usuario. Cada fichero que grabó la app se comparó con lo que
calculan, por su cuenta, los verificadores `scripts/pruebas_filtros_{b,c,e}.py` (salida en
[`evidencias/verificar-2026-11-03.txt`](evidencias/verificar-2026-11-03.txt)).

Esta ronda cubre lo que describe la guía habitual de Free1X2 que repasó el dueño: columnas probables, variantes, «columna
base», coeficientes (Valoración), simetrías en espejo, figuras, esperanza matemática (Rentabilidad) y grupos de partidos
con control de fallos. Las pruebas nuevas son el grupo E ([`filtros/plan_e.md`](filtros/plan_e.md)).

**Resumen: 14 pruebas con fichero bien, 1 mal** (FE3x).

| Prueba | Qué | Esperado | App |
|---|---|---|---|
| FC1 | Columna probable de 14 signos con aciertos 10–12 (= 2 a 4 diferencias con la columna base) | 80 | **80** OK |
| FC2 | Dos CP y suma de aciertos (Relaciones I) | 114 | **114** OK |
| FC13 | CP de 14 triples («Todos a Triple») | 216 | **216** OK: el arreglo `ec6683e` (1/X/2 por partido) funciona; antes daba 0 |
| FE2 / FE2b | CP solo con 5 partidos, aciertos `3-5` (/ y fallos seguidos 0–1) | 94 / 72 | **94 / 72** OK; el rango `3-5` ya se acepta |
| FE6 / FE6b | Variantes con solo el límite de doses | 148 / 148 | **148 / 148** OK |
| FB8 / FB9 | Valoración por sumas (coeficientes) | 82 / 59 | **82 / 59** OK; «Calcula» da 623, 303-684 y 0-528 / 0-463 / 0-409 |
| FE5 | Simetrías en espejo (1,8 … 7,14), aciertos 0–3 | 174 | **174** OK |
| FE4 | Signos Seguidos con figuras de variantes | 52 | **52** OK; al volver del editor se ve «Activas» y el campo 1 se conserva (arreglo `f97317a`) |
| FE1 | Rentabilidad: esperanza entre 1 y 2 | 82 | **82** OK, escribiendo la recaudación a mano (ver R-1) |
| FE3 / FE3b | Dos grupos con control de fallos 1 (/ 0–1), creados desde Columnas probables | 82 / 176 | **82 / 176** OK |
| FE3x | Los mismos grupos creados con «Grupo siguiente» en Inicio | 82 | **0 MAL** (G-1) |

## Fallos encontrados

| # | Qué pasa | Dónde |
|---|---|---|
| N-1 | **«Nueva» pulsado fuera de Inicio se repite al volver a Inicio** (Aceptar de una condición, Esc): se crea otra combinación nueva y se pierden el boleto y las condiciones. Se vio dos veces: Calcular dio 4.782.969 columnas en vez de 216. | `MainWindow.xaml.cs:749` navega con la acción como parámetro; `MainPage.xaml.cs:44` la ejecuta en cada navegación |
| G-1 | **Un grupo creado desde Inicio no tiene partidos** y no hay forma de marcárselos: cualquier condición en él da 0 columnas. El original deja activar los partidos de cada grupo en el boleto y no deja avanzar sin ninguno. | `MainPageViewModel.cs:448-460`; original `Free1X2/UI/Controls/Pronosticos.cs:682-711,780-805`, `MainForm.cs:563-571,1543-1553` |
| R-1 | **Rentabilidad arranca con recaudación 8.000.000** fija; el original la toma de Configuración. Con 8.000.000, FE1 da 60. | `RentabilidadFrmViewModel.cs:63`; original `RentabilidadFrm.cs:96-101` |
| V-1 | **Variantes y Signos Seguidos dicen «0–15»** y el 15 da «Se produjo un error inesperado. Index was outside the bounds of the array.» | `NoVariantesFrmPage.xaml:20` |
| V-3 | **Valoración ▸ «Calcula»**: el primer clic tras escribir la columna da «Introducir columna de 14 signos para el calculo.»; al segundo, el valor. La casilla aún no había pasado el texto. | `ValoracionFrmPage.xaml` (casilla «Columna») |

**No es fallo:** en Columnas probables, con la combinación sin ninguna CP, el primer «>» solo guarda la CP 1 y hay que
pulsarlo otra vez para pasar a 2/2. El original hace lo mismo (`Free1X2/UI/Filtros/ColProbablesFrm.cs:621-641`); el
plan C decía lo contrario y está corregido.

**Para el diseño (R2, decide el dueño):** los botones 1/X/2 por partido de Columnas probables y las casillas del editor de
figuras son muy pequeños para acertar con el ratón.

**Manual:** con «Fallos = 1» el control de grupos exige que falle **exactamente** uno, no «uno u otro» (FE3 = 82
frente a FE3b = 176). Ver hallazgo CG-1 de `plan_e.md`.

## Tras los arreglos (4-nov-2026)

App **1.1.0+35ac2ee**, con los commits `0b29f4a` (N-1), `558912c` (G-1), `8a2db0a` (R-1), `6da50b0` (V-1) y `7f0854f`
(V-3). Build 0 errores, tests 209/209, smoke 111/111. Comprobado en la app:

| Fallo | Comprobación | Resultado |
|---|---|---|
| N-1 | Calcular ▸ Nueva ▸ Máscaras ▸ B3 ▸ Variantes ▸ Aceptar ▸ Calcular | Máximo **216** (antes 4.782.969) |
| G-1 | FE3x con los partidos activados (doble clic en el título del grupo) | **82** OK (antes 0); el grupo nuevo sale con las filas atenuadas y las condiciones deshabilitadas |
| R-1 | Abrir Rentabilidad | Recaudación **14000000**, la de Configuración |
| V-1 | Variantes ▸ Var `15` ▸ Aceptar | Rótulo «(0–14)» y aviso «Las cantidades admitidas van de 0 a 14, separadas por comas.» |
| V-3 | Valoración ▸ columna `11X111X11121X1` ▸ un solo «Calcula» | Valor **623** |

El grupo E queda **10 bien, 0 mal** ([`evidencias/verificar-2026-11-04.txt`](evidencias/verificar-2026-11-04.txt): 15 de 15).

### Signos de solo lectura en los grupos (decisión del dueño, 4-nov-2026)

Commit `c46d570`. Build 0 errores, tests 209/209, smoke 111/111. Comprobado en la app (grupo 1, combinación nueva):

| Paso | Resultado |
|---|---|
| Pulsar la X del partido 3 | No cambia: sigue «Triples: 14» |
| «Ninguno» | Las 14 filas atenuadas |
| Clic en el número 3 | Solo la fila 3 normal |
| «Todos» | Las 14 activas |
| Volver al Boleto Base | Barra oculta y signos pulsables |
