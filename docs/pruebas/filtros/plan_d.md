# Plan de pruebas — grupo D (GEPT, Control Tol, Separador JB, Filtrar por límites, Listado de condiciones)

Oráculo: `scripts/pruebas_filtros_d.py` (misma carpeta). Reimplementa cada regla en Python sin llamar a C#; usa el arnés
`scripts/pruebas_windows.py` (`pw`) para desarrollar boletos, rachas, leer/escribir y verificar.

```
python -I scripts/pruebas_filtros_d.py                      # asserts + lista de esperados (ficheros y pantallas)
python -I scripts/pruebas_filtros_d.py preparar C:\Pruebas  # escribe entrada\FD_JB_entrada.txt y entrada\FD_JB_porcentajes.txt
python -I scripts/pruebas_filtros_d.py verificar C:\Pruebas # compara salida\FD1..FD6.txt (usa pw.verificar con estos esperados)
```

Para FL hace falta además `entrada\val_50_30_20.txt` del arnés (`python scripts/pruebas_windows.py preparar C:\Pruebas`).
Los «Pegar desde Excel»: abrir el .txt en el Bloc de notas, Ctrl+A, Ctrl+C y pulsar «Pegar desde Excel» en la tabla.

| Clave | Pantalla | Graba | Esperado |
|---|---|---|---|
| G1 | Columnas GEPT | — (pantalla) | tabla de pronósticos (abajo) |
| **FD1** | Inicio ▸ Tolerancias | `FD1.txt` | Procesadas 216, Aceptadas **166** |
| **FD2** | Inicio ▸ Tolerancias (+ Fallos Controles) | `FD2.txt` | Procesadas 216, Aceptadas **134** |
| **FD3** | Listado de condiciones ▸ Exportar a Texto | `FD3.txt` | 56 líneas (árbol de FD2) |
| **FD6** | Separador JB ▸ Exportar | `FD6.txt` | 7 líneas (signos por banda) |
| **FD4** | Separador JB, valores por defecto | `FD4.txt` | Procesadas **310**, Admitidas **99** |
| JB-A | Separador JB ▸ Analizar | — (pantalla) | Bandas 1,3,1,4,3,0,2 · Recorrido 4 |
| **FD5** | Separador JB, límites y rangos cambiados | `FD5.txt` | Procesadas **310**, Admitidas **22** |
| JB-B | Separador JB ▸ Analizar (límites FD5) | — (pantalla) | Bandas 0,3,2,5,2,0,2 · Recorrido 5 |
| FL1–FL4 | Tramificar ▸ Filtrar (diálogo «Filtrar por límites») | — (pantalla) | ver §5 (se espera MAL: hallazgo D-FL1) |

Ninguna de estas cinco pantallas está en el panel lateral salvo GEPT y Listado: Control Tol se abre desde Inicio, el
Separador JB desde el Generador de CPs y Filtrar por límites desde Tramificar (`MainWindow.xaml.cs:454-529`; el registro
`Navigation/PortedPages.cs` solo lo usa el smoke).

---

## 1. Columnas GEPT (`GEPTFrm`)

**Qué hace.** No filtra columnas: calcula un pronóstico por partido a partir de dos letras G/E/P.
- Manual (`docs/MANUAL_USUARIO.md:594-603`): 1.ª columna = último resultado de liga del **local**, 2.ª = el del
  **visitante**; «Calcular» da el pronóstico en la 3.ª.
- Original: `Free1X2/UI/GEPTFrm.cs:138-198` (`Calcular` = `Verificar` + `Pronosticar`; tabla `Traductor` 183-198:
  GG 12 · GE 1X · GP 1 · EG X2 · EE X · EP 1X · PG 2 · PE X2 · PP 12; cualquier otra cosa, un espacio).
- Port: `Free1X2.WinUI/Views/Ported/GEPTFrmViewModel.cs:140-178` (misma tabla). La lógica coincide.
- Oráculo: `gept()` deduce el pronóstico del fútbol (el 1 entra si el local ganó o el visitante perdió, etc.) y un
  assert comprueba que coincide con la tabla del original en las 9 combinaciones y que es simétrica (local⇄visitante =
  1⇄2).

**Cómo llegar.** Panel lateral ▸ Utilidades ▸ «Columnas GEPT» (o menú Utilidades ▸ «Columnas GEPT…»).

**Prueba G1.** En cada fila hay dos desplegables («Última vez», «Penúltima vez») con `—`, `G`, `E`, `P`. Elegir:

| Fila | 1.ª | 2.ª | Pronóstico esperado |
|---|---|---|---|
| 01 | G | G | 12 |
| 02 | G | E | 1X |
| 03 | G | P | 1 |
| 04 | E | G | X2 |
| 05 | E | E | X |
| 06 | E | P | 1X |
| 07 | P | G | 2 |
| 08 | P | E | X2 |
| 09 | P | P | 12 |
| 10 | G | — | (vacío) |
| 11 | — | P | (vacío) |
| 12-14 | sin tocar | | (vacío) |

Pulsar **Calcular**. Además: las celdas G se ven verde-amarillo, E amarillo, P rosa (`GEPTFrm.cs:168-174`).

**Hallazgos.**
- **D-G1 (texto de UI).** El port titula las columnas «Última vez» / «Penúltima vez» (`GEPTFrmPage.xaml:49,53`) y dice
  «las dos últimas veces que se repitió el enfrentamiento» (`GEPTFrmPage.xaml:21`, también `GEPTFrmViewModel.cs:16-17`).
  El manual (`MANUAL_USUARIO.md:596-597`) dice local / visitante, y el original tiene una sola etiqueta «Última vez» encima
  de las dos columnas (`GEPTFrm.cs:228-234`, label1 en x=10 de 100 px; cajas en x=40 y x=75, `GEPTFrm.cs:40-41`). La tabla
  (GP = 1, PG = 2) solo tiene sentido con local/visitante. Decisión de UI: del dueño (R2).
- D-G2 (menor). El original usa cajas de texto que rechazan otra letra con «Las letras permitidas son: G,E,P»
  (`GEPTFrm.cs:276-309`); el port usa desplegables. Mismo resultado.

---

## 2. Control Tol — Tolerancias del grupo (`ControlTolFrm`)

**Qué hace.** Edita el `ControladorTol` del grupo en pantalla: filas «Tolerancia» (letras) / «Aciertos» y «Fallos
Controles». Actúa como condición al calcular desde Inicio.
- Manual: una línea (`MANUAL_USUARIO.md:350`: permite que un nº limitado de condiciones se cumpla en un valor
  «tolerado»); ejemplo con CPs en `MANUAL_USUARIO.md:377,389`. No explica cómo se marcan los valores tolerados.
- Regla (motor, igual en original y port):
  1. En una condición, un valor con letra (`8A`) es un valor **admitido** marcado con la letra A
     (`FiltroNoVariantes.cs:231-252, 270, 309`; `FiltroSignosSeguidos.cs:414-440`).
  2. Si la columna pasa todas las condiciones activas, por cada fila de tolerancia se cuentan las condiciones cuyo valor
     lleva una de sus letras (`FiltroNoVariantes.cs:176-199`, `FiltroSignosSeguidos.cs:319-347`, suma en
     `Grupo.cs:331-365` y `367-406`); la fila se cumple si ese nº está en «Aciertos» (`ToleranciaFiltros.cs:43-59`).
  3. «Fallos Controles» vacío ⇒ deben cumplirse todas (`Grupo.cs:194-197`). Con valor ⇒ el nº de filas falladas debe
     estar en esa lista (`ControladorTol.cs:43-81, 109-125`): **«1» es exactamente una**, no «hasta una».
- Original `Free1X2/UI/Filtros/ControlTolFrm.cs:285-309`; port `ControlTolFrmViewModel.cs:94-117`.
- Oráculo `cumple_tol()`; los asserts reproducen el ejemplo del manual §7: 46.656 columnas, dos CPs a 3–6 ⇒ **27.552** y a
  4–5 con 3 y 6 tolerados y como mucho uno ⇒ **22.848** (`MANUAL_USUARIO.md:388-389`), y 37.888 / 24.272 / 10.912 de la
  «restricción progresiva» (`:368-371`).

**Cómo llegar.** Inicio ▸ botón **«Tolerancias»** bajo la rejilla de condiciones (`MainPage.xaml:174-179`,
`MainPageViewModel.cs:324-328`).

**Prueba FD1** (boleto B3 del arnés: P1 `1X2` · P2 `1` · P3 `X2` · P4 `1X2` · P5 `1` · P6 `1X` · P7 `X` · P8 `1X2` ·
P9 `12` · P10 `1` · P11 `2` · P12 `1` · P13 `X` · P14 `1` = 216 columnas):
1. Inicio ▸ **Nueva combinación** ▸ boleto B3 (cabecera «Fijos: 8 - Dobles: 3 - Triples: 3»).
2. Tarjeta **«Variantes X y 2»**: Var `5,6,7,8A,9A` · X vacío · 2 `1,2,3,4A,5A,6A` (letras en **mayúscula**) ▸ Tab ▸
   **Aceptar**.
3. Tarjeta **«Signos Seguidos»**: 1 `1,2,3A` (Var, X, 2 vacíos) ▸ Tab ▸ **Aceptar**.
4. **Tolerancias**: fila 1 «Tolerancia» `A`, «Aciertos» `0,1`; «Fallos Controles» vacío ▸ **Ok**.
5. **Calcular** (barra de Inicio) ▸ «Grabar archivo» ▸ «Seleccionar archivo...» `salida\FD1.txt` ▸ Calcular.

Esperado: Procesadas **216**, Aceptadas **166** (A marca «Var 8 ó 9», «2 de 4 a 6» y «1 seguidos = 3»; se admiten las
columnas con como mucho una de las tres). Sin la tolerancia saldrían 214: la tolerancia quita 48.

**Prueba FD2** (sigue de FD1):
6. «Variantes X y 2»: X `2,3,4B,5B,6B,7B` (Var y 2 como estaban) ▸ Tab ▸ Aceptar.
7. Tolerancias: fila 2 `B` / `0`; «Fallos Controles» `1` ▸ Ok.
8. Calcular ▸ Grabar `salida\FD2.txt`.

Esperado: Procesadas **216**, Aceptadas **134** (exactamente una de las dos filas falla). Diagnóstico si sale otra cosa:
185 = «Fallos» tomado como «0,1»; 51 = «Fallos» ignorado; 214 = tolerancias ignoradas.

**Hallazgos.**
- **D-CT1 (port ≠ original).** El original solo guarda filas con «Tolerancia» no vacía (`ControlTolFrm.cs:137`) y
  luego con «Aciertos» no vacío (`:299`); el port guarda toda fila con «Aciertos», aunque no tenga letra
  (`ControlTolFrmViewModel.cs:101`). Una fila «(vacío) / 1» hace que el port rechace **todas** las columnas (cuenta 0
  siempre); el original la ignora. A la inversa, una fila con letra y sin aciertos hace fallar al original
  (`Convert.ToInt32("")` en `ToleranciaFiltros.cs:61-114`) y el port la ignora.
- D-CT2 (heredado, sin aviso). Una letra en minúscula en la condición (`8a`) nunca coincide: la de la tolerancia se pasa a
  mayúsculas (`ToleranciaFiltros.cs:148-152`) y la de la condición no (`FiltroNoVariantes.cs:231-252`).
- D-CT3 (heredado). «Aciertos» y «Fallos Controles» aceptan `0,1` o `0-1`, pero no mezclas (`0,2-3` ⇒ excepción) y la lista
  debe acabar en el mayor: el array se dimensiona con el último elemento (`ToleranciaFiltros.cs:61-80`,
  `ControladorTol.cs:50-58`), p. ej. `3,1` ⇒ índice fuera de rango.
- D-CT4 (documentación). El manual no explica la sintaxis `valor+letra` ni que «Fallos Controles 1» significa exactamente
  uno (`MANUAL_USUARIO.md:350`).

---

## 3. Listado de condiciones (`ListadoCondicionesFrm`)

**Qué hace.** No filtra: muestra en árbol la combinación actual (Pronóstico Base, Filtro de Columnas, Grupos con sus
condiciones activas, Control de grupos, IfThen), con Expandir/Colapsar y Exportar a texto/HTML
(`MANUAL_USUARIO.md:204`). Original `Free1X2/UI/ListadoCondicionesFrm.cs:80-776`, exportación `:777-806`; port
`ListadoCondicionesFrmViewModel.cs:72-170` (árbol), `:610-632` (texto).

**Cómo llegar.** Panel lateral ▸ Ajustes y ayuda ▸ «Listado de condiciones» (o menú Ver ▸ «Listado de condiciones»).

**Prueba FD3** (con el estado de FD2, sin tocar nada más):
1. Abrir «Listado de condiciones» ▸ **Expandir Todo**. Comprobar en pantalla:
   - Pronóstico Base: «Partido 1: 1,X,2» … «Partido 9: 1,2» … «Partido 14: 1».
   - Grupos ▸ Boleto Base ▸ Condiciones ▸ NoVariantes: «Cantidad de V: 5,6,7,8A,9A», «Cantidad de X: 2,3,4B,5B,6B,7B»,
     «Cantidad de 2: 1,2,3,4A,5A,6A»; SignosSeguidos: «Cantidad de 1 Seguidos: 1,2,3A» y las otras tres
     `0,1,…,14`.
   - Control de grupos ▸ Control Conjuntos: «Conjuntos: 0», «Fallos: 0». IfThen ▸ «IfThen no activado».
2. **Exportar a Texto** ▸ guardar `salida\FD3.txt` (la app abre después el fichero con el Bloc de notas: ciérralo).

Esperado: el fichero de 56 líneas que da el oráculo (se compara línea a línea, con las vacías). Derivado del código, no
ejecutado: lo menos seguro es «Partidos Activos:» vacío (ver D-L3); si solo difiere esa línea, el resto vale.

**Hallazgos.**
- **D-L1 (heredado).** No lista las **Tolerancias** del grupo ni «Fallos Controles» (ni el original): tras FD2 el listado
  no dice nada de A/B ni del «1».
- D-L2 (heredado). De Interrupciones lista Global, 1, X, 2 y sus seguidas, pero **no «Var» ni «Var seguidas»**
  (`ListadoCondicionesFrmViewModel.cs:228-235`, original `ListadoCondicionesFrm.cs:343-351`) aunque el filtro los tiene
  (`FiltroInterrupciones.cs:746, 991`). Comprobable con la C1 del arnés (Seguidas Var `2,3` no aparece). La etiqueta
  «Interrupciones de 2 » no lleva dos puntos (igual en el original, `:346`).
- D-L3 (heredado). «Partidos Activos:» del Boleto Base sale vacío en una combinación nueva: el grupo base solo marca
  `todosPartidosActivos` (`Analizador.cs:65-67`, `Grupo.cs:565-578`) y el listado lee el array `partidosActivos`
  (`Grupo.cs:433-450`), que nadie rellena.
- D-L4 (port ≠ original, menor). Con un IfThen existente pero desactivado, el port escribe «IfThen no activado»
  (`ListadoCondicionesFrmViewModel.cs:143,168`); el original deja «IfThen» sin hijos y solo pone ese texto si no hay
  IfThen (`ListadoCondicionesFrm.cs:722,769`).

---

## 4. Separador de porcentajes Juan Bellas (`FiltroPorcenJB`)

**Qué hace.** Clasifica cada signo de cada partido en 7 bandas por su porcentaje (límites «<=», por defecto 15, 22, 29,
36, 48, 61; por encima, banda 7). Para cada columna de un fichero cuenta, por banda, cuántos partidos tienen su signo en
esa banda; la admite si cada cuenta está en el rango «min-max» de su banda y el **recorrido** (máx − mín de las 7
cuentas) en el suyo. Quita repetidas (se queda con la primera). Graba las admitidas en el orden del fichero.
- El manual solo la nombra (`MANUAL_USUARIO.md:413`). Oráculo según el original `Free1X2/UI/FiltroPorcenJB.cs`:
  RecuperaPantalla 224-237, valora 265-277, Calcular 424-464, Valida 465-482, Analizar 536-568, ExporCols 602-618,
  s2n 629-637. Port `FiltroPorcenJBViewModel.cs`: Calcular 147-204, Grabar 206-226, Analizar 304-347, Exportar 349-374,
  RecuperaPantalla 446-468, Valora 506-518, Valida 535-556. La regla coincide.
- Asserts: suma de bandas = 14 en toda columna, fronteras «<=» (15→1, 22→2, 29→3, 36→4, 48→5, 61→6, 62→7), un caso a mano
  y coherencia Exportar ⇔ cuentas.

**Cómo llegar.** Panel lateral ▸ Utilidades ▸ «Generador de CPs» ▸ botón **«Separador de porcentajes»**
(`GenerarCPsPage.xaml:141-143`).

**Entradas** (`scripts/pruebas_filtros_d.py preparar`): `FD_JB_entrada.txt` = 300 columnas al azar (semilla 2026) + 10 repetidas (4 en
minúsculas) = 310 líneas; `FD_JB_porcentajes.txt` = 14 filas 1/X/2 (con valores justo en los límites 15, 22, 29, 36, 48,
61).

**Pasos y esperado.**
1. Tarjeta «Filtrado de columnas» ▸ tabla de porcentajes ▸ **Pegar desde Excel** con `FD_JB_porcentajes.txt` (P1 =
   45/30/25 … P14 = 75/15/10).
2. **FD6**: **Exportar** ▸ `salida\FD6.txt`. Esperado (7 líneas):
   ```
   -,2,-,-,2,-,-,-,X,X2,-,2,-,X2
   -,X,1,-,X,X2,-,-,-,-,-,-,-,-
   2,-,-,2,-,-,2,-,-,-,1X,X,2,-
   X,-,X,X,-,-,1,X2,2,-,-,-,1,-
   1,-,2,1,-,-,X,1,-,-,2,-,X,-
   -,-,-,-,-,1,-,-,1,-,-,1,-,-
   -,1,-,-,1,-,-,-,-,1,-,-,-,1
   ```
3. **FD4**: límites y rangos por defecto (Bandas 1-7 `0-3`, Recorrido `0-14`) ▸ **Calcular** ▸ elegir
   `entrada\FD_JB_entrada.txt` ▸ «Procesadas: **310**», «Admitidas: **99**» ▸ **Grabar Resultado** ▸ `salida\FD4.txt`.
4. **JB-A**: «Columna ganadora» `1X21X21X21X211` ▸ Tab ▸ **Analizar** ⇒ Banda 1..7 = **1, 3, 1, 4, 3, 0, 2**, Recorrido
   **4**.
5. **FD5**: Límite 1..6 = `10`, `20`, `30`, `40`, `50`, `60` (Tab tras cada uno) · Banda 1..7 = `1-3`, `2-4`, `1-4`,
   `1-5`, `1-3`, `1-4`, `0-2` · Recorrido `3-5` (Tab) ▸ Calcular ▸ `FD_JB_entrada.txt` ⇒ Procesadas **310**, Admitidas
   **22** ▸ Grabar Resultado `salida\FD5.txt`. (Cada rango y el recorrido quitan alguna columna: probado en el oráculo.)
6. **JB-B**: Analizar otra vez `1X21X21X21X211` ⇒ **0, 3, 2, 5, 2, 0, 2**, Recorrido **5**.

**Hallazgos.**
- **D-JB1 (UI que falta).** El original pinta una rejilla de 14 × 7 con los signos de cada partido en cada banda y el nº
  de partidos por banda (`PintaPantalla`, `FiltroPorcenJB.cs:278-385`, llamada en Calcular `:438`, Analizar `:544` y al
  cambiar porcentajes `:2832-2837`; etiquetas `l011…`, `lq1…lq7`, `:138-140`). El port no la tiene (solo sale en el fichero
  de «Exportar»). Decisión del dueño (R2).
- D-JB2 (port ≠ original, benigno). Una línea vacía en el fichero: el original la cuenta y se para con «error
  longitud=» (`FiltroPorcenJB.cs:448-450`); el port la salta sin contarla (`FiltroPorcenJBViewModel.cs:179`).
- D-JB3 (documentación). El manual no describe la pantalla (bandas, rangos, recorrido, Analizar, Exportar).

---

## 5. Filtrar por límites (`DialogoFiltrarPorLimitesFrm`, desde Tramificar ▸ Filtrar)

**Qué hace.** Tras «Tramificar» (las 4.782.969 columnas ordenadas por valoración y escrutadas), «Filtrar» pasa al
diálogo 10 tramos de **posiciones** calculados de «Mínima»/«Máxima» de cada categoría (`ObtenerExtremos`,
`TramificarFormViewModel.cs:751-784`). En cada tramo: «Difs. desde» = 0 elimina las columnas del propio tramo;
«Difs. hasta» = n > 0 elimina además las que están a 1..n signos de cada una. Al Aceptar, Tramificar muestra «Nº columnas
filtro» (las que quedan) y «Grabar filtro» las graba (`TramificarFormViewModel.cs:681-749`; original
`TramificarForm.cs:3015-3052`). Manual `MANUAL_USUARIO.md:633-637`: por defecto 0–4 bajo 10, 1–3 de 10 a 11, «1 a 0» no
elimina nada, «0 a 0» quita solo el intervalo.

**Por qué la prueba no depende de la valoración.** Con todos los «hasta» a 0, lo eliminado es la unión de tramos de
posiciones: aritmética pura. Los Mínima/Máxima se teclean a mano (son cajas editables).

**Cómo llegar.** Panel lateral ▸ Utilidades ▸ «Tramificar».

**Pasos.**
1. Valoración por partido ▸ Pegar desde Excel `entrada\val_50_30_20.txt` ▸ **Tramificar** (tarda: 4,8 millones).
2. En «Posiciones mínimas y máximas» escribir (Tab tras cada una): Mínima 14 = `501`, 13 = `401`, 12 = `301`, 11 = `201`,
   10 = `101`; Máxima 14 = `1000`, 13 = `2000`, 12 = `3000`, 11 = `4000`, 10 = `5000`.
3. **Filtrar** ⇒ abre «Filtrar por límites».

| Prueba | Qué hacer en el diálogo | Esperado |
|---|---|---|
| **FL1** | Mirar la columna «Posiciones» y las «Difs.» al abrir | Posiciones: `0 a 100`, `101 a 200`, `201 a 300`, `301 a 400`, `401 a 500`, `1001 a 2000`, `2001 a 3000`, `3001 a 4000`, `4001 a 5000`, `5001 a 4782968`. Difs. (original y manual): 0–4, 1–3, 1–2, 1–1, 1–0, 1–0, 1–1, 1–2, 1–3, 0–4. **Por el código del port se verán todas 0–0 (D-FL2)** |
| **FL2** | Todas las filas «Difs. desde» 0 y «hasta» 0 ▸ Aceptar | «Nº columnas filtro» = **501** desde el arreglo D-FL4 (en el original, 4778469) |
| **FL3** | Fila 1 (`< 10`, 0 a 100) 0–0; las demás 1–0 ▸ Aceptar | **4782869** (quita las 100 del tramo) |
| FL4 | Solo la fila 10 a 0–0; las demás 1–0 ▸ Aceptar | **5001** desde el arreglo D-FL4 (en el original la fila 10 no hacía nada: 4782969) |

Antes de cada FL2–FL4 hay que repetir 1–3 si Tramificar ha perdido los datos (ver D-FL1).

**Hallazgos.**
- **D-FL1 (grave, por lectura de código; confirmar con FL2).** Al Aceptar, el diálogo vuelve con `Frame.GoBack()`
  (`DialogoFiltrarPorLimitesFrmPage.xaml.cs:47-55`); como ninguna página usa `NavigationCacheMode` (lo dicen
  `AppState.cs:68-69` y `MainPageViewModel.cs:124-126`), se crea un `TramificarFormPage` **nuevo** con un ViewModel nuevo
  (`TramificarFormPage.xaml.cs:18`), cuyo `Ap14T` está vacío (`TramificarFormViewModel.cs:312`).
  `AplicarFiltroConExtremos` sale con «Primero hay que tramificar para escrutar las columnas.»
  (`TramificarFormViewModel.cs:708-713`): el filtro **nunca se aplica** y se pierden la tramificación y los datos
  tecleados. Esperado en la app: FL2/FL3 sin «Nº columnas filtro» y ese mensaje en el estado.
- **D-FL2 (port ≠ original y manual).** Al abrir desde Tramificar, las «Difs.» salen 0–0 en todas las filas: `Inicializar`
  las toma de la matriz recibida (`DialogoFiltrarPorLimitesFrmViewModel.cs:98`), que `ObtenerExtremos` deja a 0
  (`TramificarFormViewModel.cs:751-784`). El original no las carga de la matriz: usa las del diseñador (0–4, 1–3, 1–2,
  1–1, 1–0, 1–0, 1–1, 1–2, 1–3, 0–4; p. ej. `DialogoFiltrarPorLimitesFrm.cs:954, 1027, 1037`), como dice el manual
  (`:635-637`). Aceptar sin tocar elimina todas las columnas de cada tramo en vez de sus vecinas.
- D-FL3 (port ≠ original). Los valores por defecto del constructor del port para las filas 9 y 10 son 0–0 y 0–0
  (`DialogoFiltrarPorLimitesFrmViewModel.cs:67-68`); en el original, 1–3 y 0–4. Y las etiquetas están corridas desde la
  fila 6: falta «de <14 a 13» (original label42, `DialogoFiltrarPorLimitesFrm.cs:994`) y la fila 10 se llama «fila 10» en
  vez de «< 10» (`DialogoFiltrarPorLimitesFrmViewModel.cs:49-58`; original label26 `:755`).
- **D-FL4 (heredado).** La fila 10 (las posiciones peores que la máxima de 10) no se aplica nunca: al aceptar copia la
  fila 9 (original `DialogoFiltrarPorLimitesFrm.cs:1217-1220`, port `DialogoFiltrarPorLimitesFrmViewModel.cs:141-146`).
  El «por debajo de 10, 0–4» del manual solo actúa en el tramo de arriba.
- D-FL5 (heredado). «Difs. desde» solo distingue 0 de no-0: con «hasta» > 0 siempre elimina desde 1 diferencia
  (`TramificarFormViewModel.cs:741-742, 787-803`); «2 a 3» quita también las de 1.

---

## Cómo se comprueba

`python -I scripts/pruebas_filtros_d.py verificar C:\Pruebas` saca OK / MAL (con lo que sobra o falta) / «sin salida» para FD1–FD6; las
de pantalla (G1, JB-A, JB-B, FL1–FL4, recuentos «Procesadas/Aceptadas») se apuntan a mano con captura.
