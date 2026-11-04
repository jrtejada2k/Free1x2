# Pruebas con datos de los filtros que faltaban (grupos A–D)

Segunda ronda de [`PRUEBAS_WINDOWS.md`](../../PRUEBAS_WINDOWS.md), para las pantallas del grupo Filtros que no se habían
probado con datos. Cada grupo tiene un plan con los pasos en la app y un verificador independiente en `scripts/`, que
calcula lo esperado sin usar el motor de la app:

| Grupo | Pantallas | Plan | Verificador |
|---|---|---|---|
| A | Distancias, Simetrías, Contactos, Dibujos, Diferencias, Figuras | [`plan_a.md`](plan_a.md) | `scripts/pruebas_filtros_a.py` |
| B | Formatos, Formatos 123, Pesos, Valoración, Análisis de signos, Saca formatos, Analizador | [`plan_b.md`](plan_b.md) | `scripts/pruebas_filtros_b.py` |
| C | Columnas probables, Grupos de equipos, If-Then, Pares, Tríos, Dependencia lineal | [`plan_c.md`](plan_c.md) | `scripts/pruebas_filtros_c.py` |
| D | GEPT, Control de tolerancias, Listado de condiciones, Separador JB, Tramificar ▸ Filtrar por límites | [`plan_d.md`](plan_d.md) | `scripts/pruebas_filtros_d.py` |
| E | Lo que faltaba de la guía habitual: Rentabilidad, CP parcial, control de fallos entre grupos, figuras de Signos Seguidos, simetrías en espejo, Variantes solo con doses | [`plan_e.md`](plan_e.md) | `scripts/pruebas_filtros_e.py` |

Resultados en la app: [`../RESULTADOS_2026-11-03.md`](../RESULTADOS_2026-11-03.md) (FC1, FC2, FC13, FB8, FB9 y todo el
grupo E). El resto de A–D sigue pendiente.

```bash
python -I scripts/pruebas_filtros_a.py                       # comprueba el verificador e imprime lo esperado
python -I scripts/pruebas_filtros_a.py preparar <carpeta>    # escribe los ficheros de entrada en <carpeta>\entrada
python -I scripts/pruebas_filtros_a.py verificar <carpeta>   # compara <carpeta>\salida\<CLAVE>.txt con lo esperado
```

## Estado (2-nov-2026)

**Aún no se han hecho en la app.** Los planes salen de leer el código, y al leerlo aparecieron fallos del port. Esos
fallos se corrigieron **antes** de pasar las pruebas: commits `4b882b7` (A), `f97317a` (B), `ec6683e` (C) y `a6f50cc`
(D). Los planes se escribieron antes de esos arreglos y dicen «se espera MAL» en esos puntos. **Con los arreglos, lo
esperado es esto:**

| Prueba | Antes (por el código) | Ahora |
|---|---|---|
| FA3b Contactos, una fila vacía | error y 0 columnas | **134** |
| FA6 Figuras del editor | se perdían (216) | **24**: Esc conserva lo editado |
| FB2 / FB3 tras Saca formatos o Analizador y Esc | condición vacía | se conserva: **33 / 61** |
| AF1 Analizador sin Aceptar antes | analizaba lo de pantalla | aviso «Guarde el Filtro Primero!!!» |
| V1 Valoración, «Calcula» con `11X1` | nada | aviso «Introducir columna de 14 signos para el calculo.» |
| VS0–VS2 Análisis de signos | Pleno Sí y aspiración 14 siempre | con 14 signos: Pleno No y aspiración 14; VS0 premios 10-0-0-0-0 |
| FC1 / FC2 Columnas probables | el pronóstico se escribía en una caja | **se marca con los botones 1/X/2 de cada partido**; `10-12` en Aciertos vale (= `10,11,12`) |
| FC13 CP de 14 triples | 0 | **216** |
| FC3 / FC4 Grupos de equipos | nombres «Equipo casa N» | nombres del boleto de Inicio (vacíos si no hay); resultados igual, 84 / 90 |
| FC10 Tríos | sin casilla de columna | casilla «Columna Ganadora» |
| FC12 Dependencia lineal | 36 | **108** |
| FL1 Filtrar por límites | Difs. 0–0 | 0–4, 1–3, 1–2, 1–1, 1–0, 1–0, 1–1, 1–2, 1–3, 0–4 |
| FL2 / FL3 / FL4 | no se aplicaba | **4778469 / 4782869 / 4782969** |
| G1 GEPT | desplegables | cajas de texto; «Z» da «Las letras permitidas son: G,E,P» |
| JB Separador | sin rejilla | rejilla 14×7; fila «Nº» = 4, 4, 6, 9, 8, 3, 4 |
| FC8 If-Then | **159** (anula las demás condiciones) | sin cambios: heredado del original, issue #16 |

El resto de valores de los planes no cambia.

## Lo que no se ha cambiado (viene del original)

Por R1, sin decisión del dueño no se toca:
- **If-Then sustituye a las demás condiciones** al calcular (issue #16; `plan_c.md` H1).
- **Formatos 123:** con formatos y la lista de aciertos vacía no pasa ninguna columna (`plan_b.md` H2).
- **Simetrías:** Aciertos vacío da 0 columnas (S-2). **Diferencias:** un valor no válido se ignora sin aviso (F-2).
- **Relaciones I de CP:** mezclar `7,9-11` da error (`plan_c.md` H11, en el motor).
- **Listado:** no lista Tolerancias ni «Var» de Interrupciones; «Partidos Activos» vacío (D-L1–L3).
- **Control de tolerancias:** minúscula en la condición nunca coincide; mezclas `0,2-3` dan error (D-CT2, D-CT3).
- **Filtrar por límites:** la fila 10 copia la 9; «Difs. desde» solo distingue 0 de no-0 (D-FL4, D-FL5).
- **Rangos `2-6`** en Variantes, Distancias, Contactos y Pesos: issue #21.
