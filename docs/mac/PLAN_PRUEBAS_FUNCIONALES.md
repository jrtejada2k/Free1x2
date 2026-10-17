# Plan de pruebas funcionales — Free1X2 para Mac

> Estado: **borrador para ejecutar** (2026-10-16). Objetivo: poder afirmar con evidencia que la app Mac es
> **funcional** pantalla por pantalla y flujo por flujo, y completar lo que nunca se construyó.
> Regla de oráculo (M-R1, memoria del dueño): **el C# no se ejecuta**. Los resultados esperados salen de
> cálculo a mano, de un oráculo independiente escrito desde las specs, de los golden ya existentes o de
> propiedades (ida y vuelta, invariantes).

## 0. Respuesta corta: ¿es funcional hoy?

| Pregunta | Respuesta | Evidencia |
|----------|-----------|-----------|
| ¿Pasan las pruebas? | Sí, las que existen: 230 tests (N1–N4) + 104 casos golden + smoke 109/109 + CI verde | `swift test` 2026-10-16; run 38908692051 |
| ¿Qué prueban? | Motor, formatos, online y casos de uso **sin UI**, con ficheros en memoria (`Entorno.pruebas`) | ESPEC_14 §1 N1–N4 |
| ¿Se probó la app de punta a punta? | **No.** El smoke solo abre cada pantalla. Nadie ha pulsado «Calcular», «Reducir», «Escrutar»… en la app con ficheros reales | ESPEC_17 UI-10 sin ejecutar |
| ¿Existen los niveles N5 y N6 de ESPEC_14? | **No se construyeron**: `project.yml` solo tiene el target `application`; faltan `Free1X2AppTests` (N5) y `Free1X2UITests` (N6) | `ESPEC_14_PRUEBAS.md:14-15`; `Free1X2.Mac/project.yml:23` |
| ¿Se midió la cobertura (≥ 85 % CU, ≥ 70 % app)? | No | `ESPEC_14_PRUEBAS.md:59-60` |
| ¿Se pueden crear ficheros 1X2 de prueba? | Sí: columnas = texto, una columna de 14 signos `1/X/2` por línea; `.comb` = XML con el escritor de la app; `Resultados.txt` = `Temp⇥Jorn⇥Quiniela` | 07 F09; `ArchivoResultados` |

**Conclusión:** hoy se puede decir «el motor es correcto según las specs»; **no** se puede decir «la app es
funcional». Este plan cierra ese hueco y añade lo que falta construir (§6).

## 1. Alcance

- Las 109 superficies (Inicio + 108 pantallas; `App/Navegacion/Destino.swift`).
- Los flujos de usuario de `docs/MANUAL_FLUJOS.md` §3–§10 y los 5 de ESPEC_17 UI-10.
- Compatibilidad con ficheros creados por la app de Windows (CRLF, Latin-1, UTF-8 con BOM).
- Fuera de alcance: firma y notarización (M6, dueño); las 28 peculiaridades replicadas de
  `DECISIONES.md` §5 se prueban **tal cual** (su corrección es decisión del dueño).

## 2. Requisitos previos

| ID | Requisito | Quién | Estado |
|----|-----------|-------|--------|
| P1 | macOS 14+ arm64, Xcode con Swift ≥ 6.2, `xcodegen`, `swift-format` | — | hecho |
| P2 | Build Debug y Release sin avisos; smoke 109/109 | Desarrollo | hecho (`dd616eb`) |
| P3 | **Carpeta de datos aislada**: argumento de desarrollo `-datos <carpeta>` (inerte sin él, como `-smoke`) para que las pruebas no toquen `~/Library/Application Support/Free1X2/` | Desarrollo | hecho (`RutasApp.produccion`; probado con el smoke) |
| P4 | **Juego de datos JD** (§3) generado en código, reproducible (`FlujosTests.seisTriples`, `sinX`…) | Desarrollo | hecho para la Fase 1 |
| P5 | **Oráculo independiente** `scripts/oraculo.py`: fuerza bruta sobre las 729 columnas de JD-01, escrito desde ESPEC_02/03/05/06 sin mirar el Swift ni ejecutar C# | Desarrollo | **por construir** |
| P6 | Targets de prueba en `project.yml`: `Free1X2UITests` (XCUITest) | Desarrollo | **por construir** |
| P7 | Permiso de macOS para que Xcode controle la app en las UI tests (aviso de Accesibilidad/«Automation Mode» la 1.ª vez; puede pedir contraseña de administrador) | **Dueño** | pendiente |
| P8 | Servidor de prueba del servicio online: `python3 -m http.server` sobre `docs/ejemplos-api/` + `FREE1X2_API_BASE` | Desarrollo | por construir |
| P9 | Para la sesión manual (§5.4): 2–3 ficheros reales del dueño (`.comb` y columnas `.txt` hechos con la app de Windows) y, si los tiene, los recuentos que la app de Windows mostró con ellos | **Dueño** | pendiente |

## 3. Juego de datos (JD)

Todos se generan con `scripts/generar-datos-prueba.sh` (o el helper Swift equivalente) y van con CRLF salvo
que se diga otra cosa. «Partidos variables» = 1–6; los partidos 7–14 valen siempre `1`.

| ID | Fichero | Contenido | Propiedad conocida |
|----|---------|-----------|--------------------|
| JD-01 | `seis_triples.txt` | las 3⁶ = **729** columnas con partidos 1–6 ∈ {1,X,2} | sin repetidas |
| JD-02 | `sin_x.txt` | las 2⁶ = **64** de JD-01 sin ninguna `X` | JD-02 ⊂ JD-01 |
| JD-03 | `repetidas.txt` | JD-01 + JD-02 | 793 líneas, 729 distintas |
| JD-04 | `mal_formadas.txt` | una línea de 13 signos, una con `Z`, una vacía | debe dar error controlado |
| JD-05 | `ganadora.txt` | `11111111111111` | — |
| JD-06 | `ganadoras.txt` | `11111111111111`, `22222222222222`, `1X21X21X21X21X` | — |
| JD-07 | `Jornadas/Resultados.txt` | `2024⇥1⇥11111111111111`, `2024⇥2⇥22222222222222` | 2 jornadas, 1 temporada |
| JD-08 | `seis_triples.comb` | boleto con partidos 1–6 a triple y 7–14 fijos a `1`, sin condiciones | lo escribe la app (F06) |
| JD-09 | `completa.comb` | 14 condiciones activas, 3 grupos, controles, If-Then | ya existe (`Fixtures/corpus/comb/`) |
| JD-10 | `valoracion_50_30_20.txt` | 14 filas `50 30 20` | lo escribe la app (F17) |
| JD-11 | `seis_triples_lf.txt` | JD-01 con LF y sin salto final | compatibilidad |
| JD-12 | `latin1.dat` | equipos con `ñ`, `á` en Latin-1 | compatibilidad Windows |

### 3.1 Resultados esperados calculados a mano (sobre JD-01)

Aciertos frente a `11111111111111` = 8 + nº de `1` en los partidos 1–6. Con *k* signos distintos de `1`:
C(6,k)·2ᵏ columnas.

| Aciertos | 14 | 13 | 12 | 11 | 10 | 9 | 8 | Total |
|----------|---:|---:|---:|---:|---:|--:|--:|------:|
| JD-01 (729) | 1 | 12 | 60 | 160 | 240 | 192 | 64 | 729 |
| JD-02 (64) | 1 | 6 | 15 | 20 | 15 | 6 | 1 | 64 |

- Premiadas en el rango 10–14: **473** (JD-01) y **57** (JD-02).
- Frente a `22222222222222` y `1X21X21X21X21X`: máximo 6 y 9 aciertos ⇒ **0** premiadas.
- Variantes (X + 2) ≤ 2: 1 + 12 + 60 = **73**. Sin X: **64**. Sin X ni 2: **1**.
- Álgebra: JD-01 ∩ JD-02 = **64**; JD-01 − JD-02 = **665**; JD-01 ∪ JD-02 = **729**; repetidas de JD-03 = **729**.
- Universo completo: 3¹⁴ = **4 782 969**; Tramificar en 9 tramos = **531 441** cada uno.
- Reducción a 13 sobre JD-01: cota inferior **73** (código de recubrimiento ternario K₃(6,1)); toda columna de
  JD-01 debe tener ≥ 13 aciertos frente a alguna reducida (garantía).
- Valoración 50/30/20 por sumas: columna de unos = 14·50 = **700**; de doses = 14·20 = **280**.
  Probabilidad de la columna de unos = 0,5¹⁴ ≈ **6,1035·10⁻⁵**.

Lo que no sale a mano (condiciones complejas, probabilidades, CPs) lo da el oráculo P5 desde la fórmula de la
spec citada en cada flujo.

## 4. Flujos y resultado esperado

Formato: **ID · pantallas · datos → pasos → esperado**. «Nivel»: I = integración sin UI (Swift Testing con
ficheros reales en carpeta temporal), U = XCUITest, M = manual del dueño.

### 4.1 Combinación (núcleo)

| ID | Flujo | Datos | Pasos | Esperado | Nivel |
|----|-------|-------|-------|----------|-------|
| F01 | Primer arranque | carpeta `-datos` vacía | abrir la app | crea `Columnas/ Combinaciones/ … Temp/`, copia semillas (`parametros.free1x2`, `Equipos/*.dat`, `Impresion/*.cfg`); Inicio con 14 triples, «Fijos 0 - Dobles 0 - Triples 14» | I, U |
| F02 | Boleto | — | marcar/desmarcar signos; «Reiniciar» | contadores Fijos/Dobles/Triples exactos; reiniciar ⇒ 14 triples | U |
| F03 | Calcular sin condiciones | JD-08 | Inicio ▸ Calcular ▸ «Grabar archivo» | Procesadas 729, Aceptadas 729; fichero con 729 líneas = JD-01 (como conjunto) | I, U |
| F04 | Calcular con condición | JD-08 + Variantes `Var = 0,1,2` | Condición ▸ Aceptar ▸ Calcular | semáforo verde; Aceptadas **73**; con `X = 0` **64**; con `X = 0` y `2 = 0` **1**; `0 1 2` (espacios) ⇒ error de formato (DEC-904) | I, U |
| F05 | Condición inválida y listado | JD-08 | Variantes `Var = a` | mensaje de error, semáforo rojo, no calcula; Listado de condiciones refleja lo activo | I, U |
| F06 | Guardar / abrir combinación | JD-08, JD-09 | Guardar como ▸ Nueva ▸ Abrir | `.comb` releído = mismo estado; abrir JD-09 ⇒ 14 semáforos verdes, 3 grupos, If-Then; guardar JD-09 sin cambios ⇒ bytes idénticos | I, U |
| F07 | Grupos y control de grupos | JD-08 | crear grupo 1 (partidos 1–2), filtro parcial, Control de Grupos | navegación ◀ ▶, «2 grupos», máscara de partidos; control `0,1` guardado y releído | I, U |
| F08 | Cálculo múltiple | 2 `.comb` | Calcular múltiples | una fila por `.comb` en «Resultados de la Generación Múltiple», ficheros en `Columnas/` | I |
| F09 | Cancelar cálculo largo | 14 triples | Calcular ▸ Cancelar | se detiene ≤ 1 s, mensaje de cancelado, la app sigue viva | U |
| F10 | Rendimiento | 14 triples | Calcular | 4 782 969 aceptadas dentro del techo B1 (`bench/linea-base.json`) | I |

### 4.2 Reducción y escrutinio

| ID | Flujo | Datos | Pasos | Esperado | Nivel |
|----|-------|-------|-------|----------|-------|
| F11 | Reducir | JD-01 | Reductor, nivel 13, «menos tiempo» | 73 ≤ N ≤ 729 = líneas del fichero; garantía de §3.1 comprobada por el oráculo | I, U |
| F12 | Subir categoría | salida de F11 | Subir categoría | fichero válido; garantía del nivel pedido (oráculo) | I |
| F13 | Escrutinio simple | JD-01 + JD-05 | Escrutinios, rango 10–14 | 473 filas; 1/12/60/160/240 por categoría | I, U |
| F14 | Escrutinio contra fichero | JD-01 + JD-06 | «Contra fichero» | 3 × 729 escrutadas; 10–14: 240/160/60/12/1 (solo premia la de unos); 473 premiadas | I |
| F15 | Escrutinio contra jornadas | JD-07 + `q2024_01.txt`, `q2024_02.txt` (= JD-01) | plantilla `q/t_/j.txt`, temporada 2024 | jornada 1: 473; jornada 2: 0 | I |
| F16 | Columnas premiadas | resultado de F13 | Ver premiadas ▸ Guardar todas | fichero con 473 líneas | I, U |
| F17 | Posibles premios / Mis mejores opciones | JD-02 + JD-05 | Calcular | «Optando a 14: 1 … a 10: 15» (de §3.1) | I |
| F18 | Escrutar combinaciones | JD-08 + JD-07 | Escrutar | mismo recuento que F15 | I |
| F19 | Análisis de fallos / Analizar combinación | JD-09 + JD-05 | Analizar | lista de condiciones acertadas/falladas según el oráculo | I |

### 4.3 Condiciones del grupo (14 + auxiliares)

| ID | Flujo | Esperado | Nivel |
|----|-------|----------|-------|
| F20 | Cada una de las 14 condiciones con un caso que acepta y otro que rechaza sobre JD-08 (Variantes, Signos seguidos, Dibujos, Interrupciones, Grupos de equipos, Formatos, Simetrías, Pesos numéricos, Valoración, CPs, Distancias, Contactos, Formatos 123, Diferencias) | Aceptadas = oráculo P5 para ese valor; Guardar/Abrir/Copiar/Pegar/Borrar de la condición = ida y vuelta | I (14) + U (3) |
| F21 | Estadísticas de condición (Interrupciones, Signos seguidos 0.1, Dibujos, Coincidencias de dibujos) | tablas = recuentos del oráculo sobre JD-01 | I |
| F22 | If-Then y Tolerancias | relación «si Variantes 0 entonces …» filtra lo que dice el oráculo; tolerancia 1 acepta las que fallan 1 condición | I |
| F23 | Figuras, Formatos 123 (analizador, saca formatos), Pares, Tríos, Buscador de límites, Valoración (guardar, histórico) | resultados del oráculo; ficheros guardados releídos iguales | I |

### 4.4 Columnas probables

| ID | Flujo | Esperado | Nivel |
|----|-------|----------|-------|
| F24 | Generar CPs desde JD-10, configurar, copiar, cambiar puntuación | CPs de la regla de 04 §… (oráculo) | I |
| F25 | Exportar / importar CPs (`.txt`, `.clm`) | ida y vuelta idéntica | I |
| F26 | Generador por diferencias, EstuCol, Separador JB, Sumas pares JPM | salidas del oráculo | I |

### 4.5 Filtros, operaciones y utilidades

| ID | Flujo | Datos | Esperado | Nivel |
|----|-------|-------|----------|-------|
| F27 | Combinar filtros | JD-01, JD-02 | 64 en 2 filtros, 665 en 1; mín = máx = 2 ⇒ 64 | I |
| F28 | Diferencias de filtros, Coincidencias, Aidomnou, Pim | JD-01, JD-05, JD-10 | oráculo | I |
| F29 | Álgebra | JD-01, JD-02, JD-03 | 729 / 729 / 64 / 665 (§3.1) | I, U |
| F30 | Transposición | JD-01 | orden identidad ⇒ mismas columnas; 1↔7 ⇒ 729 columnas con `1` en el partido 1 | I |
| F31 | Multiplicador | JD-02 × JD-05 | 64 columnas de 28 cifras | I |
| F32 | Fraccionador | JD-01, 3 tramos | 3 ficheros de 243 | I |
| F33 | Rotación de signos | JD-01, base = identidad | salida = entrada | I |
| F34 | Compresor `.z3q` | JD-01 | comprimir + descomprimir = mismas 729 | I |
| F35 | Importar / Exportar (`.csv` ↔ `.txt`) | JD-01 | ida y vuelta | I |
| F36 | Ordenar por probabilidad, Modificador, Rentabilidad, Estimación, Probabilidades de premios | JD-01, JD-10 | columna de unos: 0,5¹⁴; resto oráculo | I |
| F37 | Tramificar (+ gráficas, filtrar por límites, grabar tramos, análisis múltiple) | 14 triples | 9 tramos × 531 441; Σ = 4 782 969 | I |
| F38 | Banco de pruebas (4 pasos + grabar apuestas) | JD-10 | recuentos del oráculo; fichero de apuestas con el nº pedido | I |
| F39 | Selectores JuanM y MarioSan, GEPT, Dependencia lineal, Reducciones perfectas, Análisis de grupos, Diferencias entre columnas | JD-01 | oráculo | I |
| F40 | Ver boletos, editor, Impresión (PNG) | JD-02 | nº de boletos según 09 §68 (incluida DEC-1016); PNG creado | I, U |

### 4.6 Archivo, configuración y online

| ID | Flujo | Esperado | Nivel |
|----|-------|----------|-------|
| F41 | Actualizar jornada (stub P8) | 14 equipos de `quiniela-es-actual.json` en el boleto; reiniciar sin red ⇒ siguen (caché `jornada-es.json`) | I, U |
| F42 | Sin red | mensaje «No se pudo contactar…», la app sigue | I |
| F43 | Gestión de equipos / Añadir equipo | alta en 1.ª ▸ Guardar ▸ reiniciar ⇒ presente en `equipos1.dat` (Latin-1, JD-12) | I, U |
| F44 | Configuración y Configurar análisis | cambiar «Fijos» 6→7 ▸ Guardar ▸ reiniciar ⇒ 7 en `parametros.free1x2` | I, U |
| F45 | Panel lateral (DT-82) | buscar «algebra» ⇒ solo «Álgebra de columnas»; pulsarla la abre; «Inicio» vuelve | U |
| F46 | Tema e idioma | Claro/Oscuro/Sistema; inglés solo en Créditos | U |
| F47 | Borrar temporales / informes | confirmación; carpetas vacías | I |
| F48 | Errores | abrir JD-04 en Escrutinios, un `.comb` truncado y un `.z3q` corrupto ⇒ mensaje, sin cierre | I, U |
| F49 | Cargar jornada en Inicio | «Jornada» ▸ España ⇒ local y visitante de los 14 partidos con los de la jornada (sin red, la guardada) | I, U |
| F50 | Jornada al arrancar (DT-83) | con red ⇒ boleto con los equipos de la jornada; sin red o > 3 s ⇒ en blanco y sin aviso | I, manual |
| F51 | Máscaras (DT-84, DT-88) | guardar el boleto JD-01 como máscara ▸ combinación nueva ▸ aplicarla ⇒ «Fijos: 8 - Dobles: 0 - Triples: 6»; «Nueva liga…» «Mi liga» ▸ añadir `1;unos` y `2;doses` ⇒ «2 añadidas, 0 ya estaban»; otra vez ⇒ «0 añadidas, 2 ya estaban»; escrutar ⇒ 1/12/60/160/240, 1 de 2 jornadas | I, U |
| F52 | Reducir y filtrar sin fichero (DT-85) | boleto JD-01 ▸ Inicio ▸ Reducir ⇒ Reductor con «ColumnasInicio.txt» (729 columnas) ▸ Reducir ⇒ 73…729 admitidas y «Guardar resultado…» activo | I, U |

## 5. Cómo se ejecuta

### 5.1 Fase 0 — Preparación (P3–P6, P8)
`-datos`, generador JD, oráculo, target `Free1X2UITests`, stub online. Sale: `generar-datos-prueba` reproducible
(mismos bytes en dos ejecuciones) y el oráculo validado con las cifras a mano de §3.1.

### 5.2 Fase 1 — Integración sin UI (nivel I)
Suite `Free1X2CasosDeUsoTests/Flujos/` con ficheros reales en una carpeta temporal (no en memoria) y el
`Entorno` de producción apuntado a ella. Cubre todos los flujos con «I». Corre en la CI (`mac.yml`).

### 5.3 Fase 2 — UI de punta a punta (nivel U)
`Free1X2UITests` (XCUITest) con `-datos` temporal y JD: F01–F07, F09, F11, F13, F16, F20 (3), F29, F40, F41,
F43–F46, F48. Localiza controles por `accessibilityLabel`. Requiere P7.
**Nota:** el N5 de ESPEC_14 («modelos de pantalla») supone un `<Pantalla>Modelo` por pantalla que el port no
tiene (el estado vive en la vista); se sustituye por Fase 1 + Fase 2 (se registrará como DT).

### 5.4 Fase 3 — Sesión manual del dueño (nivel M, ~1 h)
Con P9: abrir sus `.comb` y ficheros de Windows, calcular, reducir y escrutar, comparar con los recuentos que
recuerde de la app de Windows y revisar el aspecto. Lista de comprobación en el informe de Fase 4.

### 5.5 Fase 4 — Informe
Tabla flujo → resultado (OK / fallo / bloqueado) con `file:line` de cada test, defectos abiertos y CI verde.

## 6. Funcionalidad que nunca se construyó (se completa en este plan)

| ID | Hueco | Referencia | Trabajo | Prueba |
|----|-------|------------|---------|--------|
| C1 | Niveles de prueba N5/N6 y flujos UI-10 | ESPEC_14 §1; ESPEC_17 UI-10 | Fases 0–2 | este plan |
| C2 | Rotación de signos: paneles «% COMBINACIÓN» (porcentajes de signos del fichero de entrada) y «Valoraciones JORNADA», y la transposición automática | legacy `Free1X2/UI/RotacionDeSignosFrm.cs:1868-1931,2224-2292`; `TransponedorAutomatico` en ESPEC_06 §3.10 (con casos golden propuestos) | **hecho**: `Free1X2Motor/Analisis/TransponedorAutomatico.swift`, `porcentajesRotacion`, `propuestaRotacion`; paneles y «Transposición automática» en `App/Pantallas/Utilidades/Rotacion.swift` | `TransponedorAutomaticoTests` (4 casos a mano, incluido el empate no estable) + F33 |
| C3 | Botón «CPs por diferencias…» de Diferencias entre columnas sin cablear | ESPEC_09 A-41-3; legacy `UI/DifCols.cs:645` abre `GeneradorCPSDiferencias` | **hecho** (`App/Pantallas/Utilidades/Signos.swift`) | U |
| C4 | «Incluir pleno al 15» de Analizar fichero no actúa | ESPEC_08 A8 (`:488`, `:2044`) | **no es hueco**: en el legacy tampoco actúa (`Free1X2/UI/AnalizarFicheroFrm.cs:265`) y en Calcular `AnalizaCombinacion(15)` solo cambia un valor que el análisis normal no lee (`ESPEC_01_MOTOR_NUCLEO.md:303`) | — |
| C5 | Nota de Aidomnou promete `aidomnou.cfg` editable, que no existe en el port (límites fijos, DEC-923) | `App/Pantallas/Utilidades/Aidomnou.swift:353` | **hecho**: «son fijas; no se leen de ningún fichero» | U |
| C6 | Cobertura medida | ESPEC_14 §3 | `swift test --enable-code-coverage` en la CI con umbral | CI |

Se mantienen **como en el original** (no son huecos del port): «Escrutar combinaciones» deshabilitado en la
barra (el legacy también: `UI/MainForm.cs:833` vacío) y «Girar boleto» sin efecto geométrico (legacy igual,
ESPEC_09 A-68-2). Las 28 peculiaridades de `DECISIONES.md` §5 siguen replicadas hasta que el dueño decida.

## 7. Criterios de salida («la app es funcional»)

1. Fases 1 y 2 en verde en la CI; cada flujo de §4 con al menos un test que lo cubre.
2. Las 109 superficies cubiertas (matriz §8) por smoke **y** por al menos un flujo.
3. C2–C6 hechos con sus pruebas.
4. Sesión manual del dueño sin defectos graves abiertos.
5. Sin defectos de gravedad 1 (pérdida de datos, resultado erróneo, cierre) abiertos.

**Gravedad:** 1 = resultado numérico erróneo, pérdida de datos o cierre; 2 = función no disponible o mensaje
erróneo; 3 = aspecto. Un fallo frente al oráculo se investiga contra la spec citada; si la spec y el oráculo
coinciden, se corrige el Swift (R1: sin tocar golden).

## 8. Matriz de cobertura de las 109 superficies

| Área | Superficies (`Destino`) | Flujos |
|------|------------------------|--------|
| Inicio y combinación | `inicio`, `calculaColumnas`, `calculaColumnasMultiple`, `resultadosCalculoMultiple`, `listadoCondiciones`, `crearGrupos`, `controlGrupos` | F01–F10 |
| Reducción y escrutinio | `reductor`, `subirCategoria`, `escrutinios`, `columnasPremiadas`, `escrutarCombinaciones`, `posiblesPremios`, `visorPosiblesPremios`, `mejoresOpciones`, `premiadas` | F11–F18 |
| Análisis | `analizarFichero`, `analizarCombinacion`, `visorAnalisisColumnas`, `visorAnalisisColumnasAbdon`, `colGanadora`, `graficoColumnas`, `vSignos`, `anastatics`, `visorEstadisticas`, `agregaP15` | F19, F36, C4 |
| Condiciones | `noVariantes`, `signosSeguidos`, `dibujos`, `interrupciones`, `gruposEquipos`, `formatos`, `simetrias`, `pesosNum`, `valoracion`, `colProbables`, `distancias`, `contactos`, `formatos123`, `diferencias`, `ifThen`, `controlTol`, `figurasFiltros` | F20, F22 |
| Estadísticas de condición | `staInter`, `staSS`, `dib`, `dibRep` | F21 |
| Auxiliares de condición | `analisisFormatos123`, `calculoFormatos`, `parejas`, `trios`, `buscaLims`, `guardarValoracion`, `historiaValoraciones` | F23 |
| Columnas probables | `configCPs`, `copiarCP`, `copiarDatosCP`, `cambioPuntos`, `importadorCPs`, `exportadorCPs`, `generadorCPSDiferencias`, `generarCPs`, `estucol`, `filtroPorcenJB`, `analizadorJPM` | F24–F26, C3 |
| Filtros | `combinarFiltros`, `diFiltros`, `coincidencias`, `aidomnou`, `generaPim` | F27, F28, C5 |
| Operaciones | `algebraColumnas`, `transposicion`, `multiplicador`, `fraccionador`, `rotacionDeSignos`, `compresor`, `importExport` | F29–F35, C2 |
| Utilidades | `modificador`, `difCols`, `ordenarPorProbabilidad`, `selecJM`, `selectorMS`, `rentabilidad`, `gEPT`, `estimadorPremios`, `probabilidadPremios`, `bancoPruebas`, `dialogoGrabarBancoPruebas`, `anaCombi`, `reducidasPerfectas`, `dependenciaLineal` | F36, F38, F39 |
| Tramificar | `tramificar`, `tramificarGraficas`, `dialogoFiltrarPorLimites`, `dialogoGrabarTramos`, `dialogoAnalisisMultipleDeTramos` | F37 |
| Boletos e impresión | `boleto`, `verBoletos`, `verBoletosEnEditor`, `imprimirBoleto`, `listaImpresoras` | F40 |
| Archivo y online | `descargaBoleto`, `gestorEquipos`, `agregarEquipo` | F41–F43 |
| Free1X2 | `configuracion`, `configuracionAnalisis`, `ayuda`, `acercaDe`, `creditos` | F44–F46 |

## 9. Avance

| Fecha | Hecho | Hallazgos |
|-------|-------|-----------|
| 2026-10-16 | P3 `-datos`; Fase 1, lote 1: F01, F03, F04, F06, F11, F13, F14, F15, F27, F29, F30, F31, F32, F34, F48 (`Tests/Free1X2CasosDeUsoTests/FlujosTests.swift`, 13 tests verdes) | **D-01** (gravedad 2): Variantes, Signos seguidos y Distancias decían «separa con comas o espacios» y ponían `0 1 2` de ejemplo, pero el parser solo acepta comas (DEC-904) ⇒ el usuario que seguía la ayuda recibía «La cadena de entrada no tiene el formato correcto». Corregida la ayuda a comas (DT-78), sin tocar el motor. |
| 2026-10-16 | C2 (Rotación: % combinación, valoraciones jornada, propuesta y transposición automática), C3, C5; C4 cerrado como comportamiento original; F33 | — |
| 2026-10-16 | Fase 1, lote 2: F07, F16, F17, F20 (Interrupciones, Distancias, Signos seguidos, Simetrías, Dibujos con oráculo de fuerza bruta desde ESPEC_02/03), F35, F41, F42, F43, F44, F47 (FlujosTests: 23 tests). F37 (Tramificar 9 × 531 441) ya lo cubre `TramificarTests`. Fase 2: target `Free1X2UITests` con F02+F03; en local falta P7 (modo de automatización), en la CI lo activa el runner | — |
| 2026-10-16 | Fase 2 en la CI: la sesión de automatización arranca y la app se lanza con `-datos`; F04 por UI; C6: puerta de cobertura en `mac.yml` (`scripts/cobertura.py`; casos de uso 88,3 % ≥ 85 %) | **D-02** (gravedad 2, accesibilidad): «Reiniciar a 14 triples» y las flechas de Formatos se anunciaban con el nombre del icono (`arrow.counterclockwise`, `chevron.left`); ahora tienen etiqueta. **D-03** (gravedad 3): los botones `◀`/`▶`/`|<`/`<<`/`>`… de Diferencias, Premios, Posibles premios, EstuCol, Columnas probables, Analizador de formatos y Boletos se leían como símbolos; ahora tienen etiqueta |
| 2026-10-16 | CI run 38916790093 verde: F02+F03 (729) y F04 (73) pulsando la app; cobertura 88,3 %. Lote UI 2: F07, F44, F45, F46 | — |
| 2026-10-16 | Fase 2 en la CI (run 38926368108): **12 flujos de UI verdes** pulsando la app real, con los paneles de abrir/guardar (⇧⌘G + ruta): F02+F03 (729), F04 (73), F06 (guardar ▸ nueva ▸ abrir), F07, F11 (Reductor: 73…729 con garantía de 13), F13 (histograma 1/12/60/160/240), F20 Distancias (127) y Dibujos (31), F43, F44, F45, F46. Añadidos F05, F16 y F48. **No se automatizan por la UI**: F09 (14 triples sin condiciones termina en ~0,13 s, antes de poder cancelar; la cancelación la cubren los casos de uso), F41 (necesitaría un servidor de prueba en la CI; el flujo lo cubren F41/F42 de la Fase 1), F29 y F40 (cubiertos en la Fase 1 y por `ImpresionTests`) | — |
| 2026-10-16 | F48 por la UI verde (aviso de error y la app sigue). F05 por la UI destapó D-04 | **D-04** (gravedad 1, resultado erróneo): con un valor no válido en una condición (p. ej. Variantes «a») salía el error, pero el grupo se quedaba con la condición **activa y sin valores** y Calcular daba 0 columnas en silencio. Causa: `modificarGrupo` editaba el grupo por `inout` y Swift devuelve lo ya tocado aunque se lance; el original WinUI hace lo mismo (`NoVariantesFrmViewModel.cs:90-102` reinicia y activa el filtro antes de `SetNoVariantes`). Corregido en el punto común: `modificarGrupo` y `controlGrupoLibre` son atómicos (DT-80); prueba `f05CondicionNoValidaNoAltera` |

## 10. Informe (Fase 4)

Leyenda: **I** = `Tests/Free1X2CasosDeUsoTests/FlujosTests.swift` (ficheros reales); **U** = `UITests/FlujosUITests.swift`
(la app pulsada en la CI, `mac.yml`); **N1/N4** = suites existentes del paquete; ✔ = verde.

| Flujo | Cobertura | Estado |
|-------|-----------|--------|
| F01 primer arranque | I `f01PrimerArranque`; U implícito (cada prueba de UI arranca con `-datos` vacío) | ✔ |
| F02 boleto | U `testF02F03BoletoYCalcular` | ✔ |
| F03 calcular | I `f03f04Calcular`; U `testF02F03…` (729) | ✔ |
| F04 condición | I `f03f04Calcular` (73 / 64 / 1); U `testF04…` (73) | ✔ |
| F05 condición no válida | I `f05CondicionNoValidaNoAltera`; U `testF05…` (error y 729) | ✔ tras D-04 |
| F06 guardar / abrir | I `f06GuardarYAbrir` (`completa.comb` byte a byte); U `testF06GuardarNuevaAbrir` | ✔ |
| F07 grupos | I `f07Grupos`; U `testF07Grupos` | ✔ |
| F08 cálculo múltiple | N4 `LoteTests` | ✔ |
| F09 cancelar | N1 `ReduccionTests` (cancelación del motor); por la UI no es medible (§9) | ✔ parcial |
| F10 rendimiento | `Free1X2Bench` B1 frente a `bench/linea-base.json` | ✔ |
| F11 reducir | I `f11Reducir`; U `testF11Reducir` (73…729 con garantía de 13) | ✔ |
| F12 subir categoría | N1 `ReduccionTests` | ✔ |
| F13 escrutinio simple | I `f13EscrutinioSimple`; U `testF13Escrutinio` (1/12/60/160/240) | ✔ |
| F14 contra fichero | I `f14EscrutinioContraFichero` | ✔ |
| F15 contra jornadas | I `f15EscrutinioContraJornadas` | ✔ |
| F16 columnas premiadas | I `f16ColumnasPremiadas` (473); U `testF16GuardarPremiadas` | ✔ |
| F17 posibles premios | I `f17PosiblesPremios` (57) | ✔ |
| F18 escrutar combinaciones | N4 `EscrutarCombinacionesTests` | ✔ |
| F19 análisis de fallos | N4 `AnalisisFallosTests`, `VisorAnalisisTests` | ✔ |
| F20 condiciones | I `f20Condiciones` (Interrupciones, Distancias, Signos seguidos 486, Simetrías 243, Dibujos 31) + Variantes; U Distancias 127, Dibujos 31; N4 `CondicionesTests` | ✔ |
| F21 estadísticas de condición | N4 `EstadisticasTests`, `SignosTests` | ✔ |
| F22 If-Then y tolerancias | N4 `CondicionesTests`, `ListadoTests`; `CondicionesXMLTests` | ✔ |
| F23 auxiliares de condición | N4 `CondicionesTests`, `FormatosPremiadasTests`, `HerramientasTests` | ✔ |
| F24–F26 columnas probables | N4 `ColumnasProbablesTests`, `EstuColTests`; N1 `ColProbablesTests`, `GeneradorCPsTests` | ✔ |
| F27 combinar filtros | I `f27CombinarFiltros` | ✔ |
| F28 filtros (diferencias, coincidencias, Aidomnou, Pim) | N4 `FiltrosTests`, `CoincidenciasTests`, `AidomnouTests`, `ModificadorFiltrosTests` | ✔ |
| F29 álgebra | I `f29Algebra` | ✔ |
| F30 transposición | I `f30Transponer` | ✔ |
| F31–F32 multiplicar, fraccionar | I `f31f32MultiplicarYFraccionar` | ✔ |
| F33 rotación (C2) | I `f33RotacionDeSignos`; N1 `TransponedorAutomaticoTests` | ✔ |
| F34 compresor | I `f34Compresor` | ✔ |
| F35 importar / exportar | I `f35ImportarExportar` | ✔ |
| F36 probabilidades, rentabilidad, estimación | N4 `ProbabilidadTransposicionTests`, `PremiosTests` | ✔ |
| F37 tramificar | N4 `TramificarTests` (9 × 531 441) | ✔ |
| F38 banco de pruebas | N4 `BancoPruebasTests` | ✔ |
| F39 selectores, GEPT, dependencia, perfectas, grupos | N4 `SelectoresTests`, `DependenciaTests`, `RotacionGruposTests`, `HerramientasTests` | ✔ |
| F40 boletos e impresión | N4 `BoletosTests`, `ImpresionTests` | ✔ |
| F41–F42 online | I `f41f42Online`; N4 `EquiposTests`; N1 `OnlineTests` | ✔ |
| F43 equipos | I `f43Equipos` (Latin-1); U `testF43EquipoPersiste` | ✔ |
| F44 configuración | I `f44Configuracion`; U `testF44ConfiguracionPersiste` | ✔ |
| F45 panel lateral | U `testF45PanelLateral` | ✔ |
| F46 tema e idioma | U `testF46Idioma` | ✔ |
| F47 borrar temporales | I `f47BorrarTemporales` | ✔ |
| F48 errores | I `f48Errores`; U `testF48CombinacionCorrupta` | ✔ |
| F49 cargar jornada | N4 `equiposDeJornadaReal` (capturas reales ES con pleno y MX); U `testF49CargarJornada` | ✔ |
| F50 jornada al arrancar | N4 `jornadaAlArrancar` (a tiempo, lenta, error 500); comprobado en la app con la API real (Villarreal - Nápoles) y sin red (en blanco) | ✔ |
| F51 máscaras | N4 `MascarasTests` (recuento sin desarrollar = valores a mano de JD-01; guardar/listar/borrar; histórico del servicio; base por liga: archivo `jornada;columna`, suma sin duplicar, descarga sumada a «España»); U `testF51Mascaras` | ✔ |
| F52 sin fichero | I `f52ColumnasDeInicio` (729 = JD-01; 0 columnas ⇒ error y sin fichero viejo); U `testF52ReducirSinFichero`; en local, 14 triples ⇒ Reductor con las columnas en 41 s | ✔ |

**CI:** run 39006120618 verde (19 min): 265 tests del paquete, cobertura de casos de uso 88,5 % y **18 tests de UI** (F02–F52; F16 escruta JD-02 para que «Ver premiadas» no tenga 473 filas, que hacen cada búsqueda de XCUITest de 9-13 s).

**Defectos encontrados:** D-01 (ayuda que pedía espacios), D-02 y D-03 (accesibilidad), **D-04** (condición no válida que
dejaba el cálculo a 0). Los cuatro corregidos, con su prueba. **Huecos completados:** C2, C3, C5, C6 (C4 no era hueco).

**Pendiente para declarar «funcional»** (§7): la sesión manual del dueño con sus ficheros de Windows (Fase 3, P9). Para
ejecutar las pruebas de UI en su Mac, una vez: `sudo automationmodetool enable-automationmode-without-authentication` (P7).

## 11. Orden de trabajo y consumo

1. Fase 0 (P3–P6, P8) → 2. Fase 1 → 3. C2–C5 → 4. Fase 2 (necesita P7) → 5. C6 → 6. Fase 3 (dueño) → 7. Fase 4.
Sin subagentes; un `xcodebuild` a la vez (R5); commit por fase.
