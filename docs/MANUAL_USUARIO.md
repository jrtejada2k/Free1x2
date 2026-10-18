# Manual de usuario — Free1X2

Manual de uso del programa **Free1X2** para el análisis y la generación de combinaciones de la Quiniela. Reconstruido a partir del manual oficial del programa y de [quinielaticas.com](https://quinielaticas.com/programa-free1x2/).

> **Windows y Mac.** Free1X2 tiene dos versiones con el mismo motor de cálculo (dan exactamente los mismos resultados):
> **Windows** (WinUI 3, Windows 10 19041+ / Windows 11, x64, portable) y **Mac** (macOS 14 o posterior, Apple Silicon).
> Lo marcado **[Mac]** son novedades de octubre de 2026 que ya están en la versión Mac y llegan a Windows en la próxima
> versión. Lo marcado **[Windows]** solo existe allí (el diseño anterior). Lo que no lleva marca vale para las dos.

> **Índice**
> 1. [Qué es Free1X2](#1-qué-es-free1x2)
> 2. [Conceptos básicos de la quiniela](#2-conceptos-básicos-de-la-quiniela)
> 3. [La pantalla principal](#3-la-pantalla-principal)
> 4. [Flujo de trabajo paso a paso](#4-flujo-de-trabajo-paso-a-paso)
> 5. [Referencia de menús](#5-referencia-de-menús)
> 6. [Las condiciones](#6-las-condiciones)
> 7. [Columnas Probables (CP)](#7-columnas-probables-cp)
> 8. [Filtros](#8-filtros)
> 9. [Reducción de columnas](#9-reducción-de-columnas)
> 10. [Utilidades](#10-utilidades)
> 11. [Formatos de archivo](#11-formatos-de-archivo)
> 12. [Consejos y errores frecuentes](#12-consejos-y-errores-frecuentes)
> 13. [Recetas de jugadores](#13-recetas-de-jugadores)

---

## 1. Qué es Free1X2

Free1X2 es un programa libre (GPL) para Windows y Mac que ayuda a confeccionar la **Quiniela** española (14 partidos + Pleno al 15) y el **Progol** mexicano. No "adivina" resultados: aplica **las condiciones matemáticas que tú defines** sobre el universo de columnas posibles, generando solo aquellas que cumplen tus criterios, y ofrece herramientas para **reducir** el número de columnas, **escrutar** resultados y **analizar** combinaciones.

La idea central: partes de un pronóstico (que multiplica columnas con cada doble y triple) y vas **acotando** con condiciones y filtros hasta llegar a un número de columnas asumible para tu presupuesto.

---

## 2. Conceptos básicos de la quiniela

**Los signos.** Cada partido se pronostica con:
- **`1`** — gana el equipo local.
- **`X`** — empate.
- **`2`** — gana el visitante.

**Variante (V).** Es todo signo que **no** es `1`, es decir, una `X` o un `2`.

**Tipos de pronóstico por partido:**
- **Fijo** — un solo signo. No multiplica columnas.
- **Doble** — dos signos. Multiplica el total de columnas por 2.
- **Triple** — los tres signos. Multiplica por 3.

Una combinación de *t* triples y *d* dobles genera **3^t × 2^d** columnas. Por ejemplo, 6 triples + 6 dobles + 2 fijos = 3⁶ × 2⁶ = **46 656 columnas**.

**Por qué cuestan tanto los triples.** Sin saber nada de los equipos, un triple acierta siempre, un doble cubre 2 de los
3 resultados posibles (≈66 %) y un fijo 1 de 3 (≈33 %). Más triples dan más opciones de premio, pero cada doble duplica las
columnas y cada triple las triplica.

**Cómo se desarrollan.** Cada signo de un partido se combina con todos los de los demás partidos. *Ej.:* un triple en el
partido 1 y una `X` fija en el 2 dan 3 columnas (`1X`, `XX`, `2X`); un triple y dos dobles `1X` dan 3 × 2 × 2 = 12
(`111`, `11X`, `1X1`, `1XX`, `X11`, `X1X`, `XX1`, `XXX`, `211`, `21X`, `2X1`, `2XX`). El conjunto de columnas que juegas
es la **combinación**.

**Columna.** Una apuesta concreta: una cadena de 14 signos, p. ej. `11X211X1X12111`.

**Coste.** Cada columna tiene un precio (configurable; 0,75 € en la actualidad). Reducir columnas = reducir coste.

**Vocabulario.**

| Término | Significado |
|---------|-------------|
| **Condición** | Regla que debe cumplir una columna para jugarse; las que no la cumplen se descartan. |
| **Filtro** | Archivo de columnas que hace de base: solo se aceptan las columnas que están en él (ver §8). |
| **Sistema** | El conjunto de condiciones y filtros que lleva de la combinación inicial a la que juegas. |
| **Boleto base** | Los partidos del boleto: 14 más el Pleno al 15. |
| **Grupo de partidos** | Cualquier subconjunto de los partidos del boleto. |
| **Grupo de equipos** | Cualquier subconjunto de los 28 equipos del boleto (2 por partido). |
| **Rebotes** | Premios de categorías inferiores que aún cobras cuando fallas una condición (fallarla te hace perder al menos una categoría). |

**Otras definiciones útiles** (se usan en las condiciones). Para fijar ideas, se ilustran todas sobre la misma columna de ejemplo **`11X211X1X12111`**:

| Concepto | Definición | En `11X211X1X12111` |
|----------|-----------|----------------------|
| **Interrupción** | Cada cambio de signo a lo largo de la columna. | **9** interrupciones: `11`@`X`@`2`@`11`@`X`@`1`@`X`@`1`@`2`@`111`. |
| **Interrupción de signo** | Cada vez que tras un signo concreto aparece otro distinto. | **4** del signo `1`. |
| **Interrupciones seguidas (global)** | Cuando tras una interrupción el siguiente signo también interrumpe; se limita la racha máxima. | **6** seguidas. |
| **Interrupciones seguidas de un signo** | Racha de interrupciones de un mismo signo en la que cada nueva aparición de ese signo es un signo suelto que vuelve a interrumpir. | **3** del `1` (los `1` de las posiciones 6, 8 y 10). |
| **Signos seguidos** | Máximo de signos iguales consecutivos. | **3** unos (el tramo final `111`), 1 equis, 1 dos y 2 variantes seguidas (`X2`). |
| **Distancia** | Mayor separación entre dos apariciones del mismo signo. | `1`→3, `X`→4, `2`→7, `V`→3. |
| **Dibujo** | La representación `X+Y` (nº de equis + nº de doses). | 3 equis + 2 doses = dibujo `3+2`. |
| **Figura de signos** | Forma en que se agrupan consecutivamente las apariciones de un signo, ordenadas de mayor a menor grupo. | Figura de `1` = `32211`; de `X` = `111`; de `2` = `11`; de `V` = `2111`. |

**Figuras de zonas.** Además de la figura global, se puede dividir el boleto en zonas y mirar cómo se reparten los signos en cada una. Las zonas habituales son **7+7** (los 7 primeros partidos frente a los 7 últimos) y **Par-Impar** (partidos en posición par frente a impar). En el ejemplo, dividiendo en 7+7 las mitades son `11X211X` y `1X12111`, con figura de variantes `3+2`; dividiendo en par/impar quedan `1211111` y `1X1XX21`. También puede condicionarse el número de *coincidencias* entre zonas para el mismo signo (suele darse 0, 1 ó 3, rara vez 2).

Otras particiones posibles: por bloques del boleto (4+4+3+3) o por categoría de los partidos (p. ej. 10 de 1.ª y 4 de 2.ª).
La figura de zonas se escribe de mayor a menor: en par/impar, la columna de ejemplo tiene 1 variante en los pares
(`1211111`) y 4 en los impares (`1X1XX21`), figura `4+1`. Las figuras de zonas del `1` casi no se usan porque son lo
contrario de las de variantes.

**Suma de aciertos / Recorrido.** Cuando se trabaja con varias columnas probables (ver §7), la **suma de aciertos** es el total acumulado de aciertos de un conjunto de CPs, y el **recorrido** es la diferencia entre la CP con más aciertos y la que menos. Sobre rangos de aciertos `4,6,5,8,7,7` el recorrido es `8−4 = 4`.

---

## 3. La pantalla principal

La ventana principal se divide en zonas:

### Navegación **[Mac]** — panel lateral y barra de la ventana
- **Panel lateral** (izquierda): todas las pantallas **por su nombre**, agrupadas en *Combinación, Filtros, Operaciones,
  Utilidades, Archivo* y *Ajustes y ayuda*, con un **buscador** arriba (no distingue tildes ni mayúsculas: «algebra»
  encuentra «Álgebra de columnas»). La pantalla en la que estás aparece resaltada. Se oculta y se muestra con el botón
  de la esquina superior.
- **Barra de la ventana** (arriba): **Nueva**, **Abrir** y **Guardar** combinación (iconos; al dejar el ratón encima se
  ve su nombre) y **Escrutinio**, en todas las pantallas. En Inicio se añaden **Calcular**, **Reducir**, **Jornada ▾** y
  **Máscaras ▾**. Cada pantalla tiene además sus botones dentro.
- Los **menús** de siempre siguen arriba en la barra de menús de macOS, con los mismos nombres.

### Zona superior **[Windows]** — Menú y barras de herramientas
Menús y accesos directos a todas las utilidades del programa (ver [Referencia de menús](#5-referencia-de-menús)). Las
barras de iconos se muestran u ocultan desde `Ver → Barras de Herramientas`.

### Campo Pronósticos (el boleto base)
Contiene los 14 partidos. Para cargarlo:
- **Desde archivo:** `Archivo → Abrir Partidos Boleto`.
- **Manualmente:** junto a cada `?` eliges el equipo (1ª, 2ª, 2ªB o selección internacional) y marcas el/los signo(s) `1`/`X`/`2` de cada partido.

Puedes guardarlo con `Archivo → Guardar Partidos Boleto`.

**[Mac]** Los nombres se escriben directamente en las casillas *Local* y *Visitante* de cada partido, o se rellenan solos
con la **jornada actual** (ver [Obtener Boletos Online](#obtener-boletos-online)): al **abrir el programa** se intenta
descargar, y en cualquier momento con **Jornada ▾ → España / México** en la barra. En la cabecera del boleto aparece el
nombre del fichero de la combinación («combinación nueva» si aún no se ha guardado).

Debajo están los **controles de grupos** (crear/navegar grupos de condiciones — ver más adelante).

### Campo Condiciones
Una rejilla de botones, uno por condición. Cada botón tiene **dos indicadores (semáforo)** que muestran su estado:
- **Ambos en blanco** — la condición no tiene datos.
- **Indicador inferior activo** — la condición tiene datos **y se aplicará** al calcular.
- **Indicador superior activo** — la condición tiene datos **pero está desactivada** (no se tendrá en cuenta).

Puedes activar/desactivar una condición pulsando sus indicadores, sin perder los datos introducidos.

Bajo la rejilla están **Tolerancias** y **Condiciones Relacionadas**.

**[Mac]** Si la ventana es estrecha (por ejemplo con el panel lateral abierto), los botones de las condiciones pasan a
una sola columna para que sus nombres se lean enteros.

### Campo Filtro
Permite seleccionar un archivo de columnas que actúa como **base/filtro** de la combinación (ver [Filtros](#8-filtros)). Muestra indicadores de estado (cargado / activo) y el nombre del filtro seleccionado.

---

## 4. Flujo de trabajo paso a paso

1. **Configura** el programa (`Free1x2 → Configuración`): precio de la apuesta, nº de partidos, etc. En la versión Mac el
   precio de partida es **0,75 €** (Configuración ▸ Parámetros LAE ▸ Precio apuesta); cámbialo si la apuesta cuesta otra cosa.
   **[Mac]** El Progol tiene sus propios parámetros (**Configuración ▸ Parámetros Progol (México)**: 15 $ la quiniela
   sencilla, 15.000.000 $ de recaudación y 33 % para el 14; los dos últimos son estimados, ajústalos si los conoces). Se
   usan cuando la jornada cargada es de México; si no, los de España. Estimación de premios y lo que se guarda en
   `Jornadas/InfoJornadasLAE.txt` son de L.A.E. y usan siempre los de España.
2. **Carga el boleto base** (equipos + pronóstico inicial de fijos/dobles/triples). **[Mac]** Los equipos de la jornada
   se cargan solos al abrir el programa (o con **Jornada ▾**), y el pronóstico puede salir de una **máscara** guardada
   (**Máscaras ▾**, ver §10).
3. **Aplica condiciones** en el campo Condiciones (variantes, columnas probables, etc.). Cada condición acota el universo de columnas.
4. *(Opcional)* **Selecciona un filtro** si quieres partir de un archivo de columnas concreto.
5. **Calcula** (`Combinación → Calcular`). El programa genera todas las columnas que cumplen y te dice **cuántas** son y su **coste**.
6. Si son demasiadas, **reduce**: cambia el pronóstico, aprieta condiciones o usa el **reductor** (`Combinación → Reducir`).
   **[Mac]** «Reducir» en Inicio abre el reductor **ya cargado** con las columnas del boleto, sin guardar antes ningún
   archivo (ver §9).
7. **Revisa**: análisis de fallos, gráfico, probabilidades, estadísticas.
8. **Guarda** la combinación (`.comb`) y/o **exporta** las columnas (`.txt`).
9. **Ver/Imprimir boletos** (`Combinación → Ver Boletos` / `Imprimir Boletos`).
10. Tras la jornada, **escruta** (`Combinación → Escrutinios`) para ver los aciertos y premios.

---

## 5. Referencia de menús

**[Mac]** Todas las pantallas de estos menús están también en el **panel lateral**, con los mismos grupos.

### Free1x2
| Opción | Función |
|--------|---------|
| Inicio | Vuelve a la pantalla principal (boleto + condiciones). |
| Configuración | Parámetros del programa. |
| Configurar análisis | Activa/desactiva los **tipos de análisis estadístico** que el programa tendrá en cuenta (variantes, seguidos, figuras, interrupciones, dibujos, simetrías, formatos, distancias, contactos, pesos, valoración, CPs, grupos de equipos, control de grupos, diferencias…). Botones para marcar/desmarcar todos. |
| Acerca de | Información, licencia GPL, web, foros, manual. |
| Créditos | Lista de autores y colaboradores del programa. |
| Salir | Cierra el programa (pide confirmación). |

### Archivo
| Opción | Función |
|--------|---------|
| Abrir / Guardar Partidos Boleto | Carga/guarda el boleto base (equipos). |
| Nueva Combinación | Reinicia para una combinación nueva. |
| Abrir / Guardar / Guardar Como Combinación | Gestiona archivos `.comb`. |
| Borrar Combinaciones Temporales | Elimina los backups temporales. |
| Borrar Informes de Error | Elimina los informes de error generados por el programa. |
| Obtener Boletos Online | Descarga la **jornada de la semana** (España o México) desde el servicio online (ver §10). **[Mac]** Rellena el boleto de Inicio con los equipos reales. |
| Gestión de Equipos | Abre el **gestor de equipos** (ver §10). |
| Importar / Exportar Columnas | Convierte ficheros de columnas entre **CSV y TXT** en ambos sentidos. |

### Ver
| Opción | Función |
|--------|---------|
| Inicio | Vuelve a la pantalla principal. |
| Ver Boletos | Muestra las columnas dispuestas sobre boletos. |
| Gráfico de Columnas | Representación gráfica de un archivo de columnas. |
| Estadísticas | Distribución de un archivo por condiciones. |
| Configuración | Parámetros del programa. |
| **Barras de Herramientas** **[Windows]** | Submenú con un interruptor por cada una de las **6 barras** (Filtros, Free1X2, Operaciones, Utilidades, Combinación, Archivo). Permite mostrar/ocultar cada barra; la elección se conserva al cerrar el programa. En Mac no existe: las barras de iconos se sustituyeron por el panel lateral. |
| Listado de Condiciones | Muestra en **árbol** todas las condiciones/filtros configurados en la combinación actual; se puede expandir/colapsar y exportar a texto/HTML. |

### Combinación
| Opción | Función |
|--------|---------|
| Calcular | Genera la combinación. |
| Calcular Varias Combinaciones | Cálculo por lotes. |
| Ver / Imprimir Boletos | Muestra / imprime en boletos oficiales. |
| Ver Boletos en Editor de Texto | Vuelca las columnas de un fichero a un editor de texto para revisarlas/copiarlas. |
| Reducir | Abre el reductor. |
| Escrutinios | Escruta archivos de columnas. Su botón **Posibles premios** abre esa pantalla con la columna ganadora ya puesta en las casillas (las que falten, con `*`) **[Mac]**. |
| Escrutar Combinaciones | Escrutinio orientado a combinaciones (no solo a ficheros de columnas sueltas). |
| Analizar Combinación | Analiza una combinación concreta (su columna, su analizador y sus pronósticos asociados). |
| Análisis de Fallos / Gráfico / de Signos | Fallos: qué condiciones acierta o falla la ganadora. Gráfico: representación de la combinación. Signos: seguir la combinación mientras se juega la jornada. |
| Probabilidades | Probabilidad de una "hija" respecto a la "madre". |
| Estadísticas | Distribución de un archivo por condiciones. |
| Añadir Pleno al 15 | Incorpora el signo del 15. |
| **Máscaras** **[Mac]** | En el panel lateral (grupo Combinación): guarda y aplica máscaras y las compara contra las ganadoras históricas (ver §10). |

### Operaciones
| Opción | Función |
|--------|---------|
| Álgebra | Suma/resta/elimina duplicados entre archivos de columnas. |
| Transposición | Utilidad de transposición. |
| Multiplicador | Multiplicador de columnas. |
| Fraccionador | Divide combinaciones en trozos. |
| Rotación de Signos | Cambia signos según un criterio. |

### Filtros
| Opción | Función |
|--------|---------|
| Combinar Filtros | Combina dos o más filtros. |
| Diferencias entre Filtros | Compara filtros. |
| Filtro Coincidencias / "Aidomnou" / "Pim" | Filtros específicos. |

### Utilidades
| Opción | Función |
|--------|---------|
| SubeCategoría | Sube la categoría de premios de una combinación. |
| Modificador % | Modifica la distribución de signos. |
| Generador CP | Generador de columnas probables. |
| Diferencias entre columnas | Compara columnas. |
| Ordenar por Probabilidad | Ordena columnas por probabilidad a partir de una valoración. |
| Selector (JuanM) / (MarioSan) | Selección de columnas (por nº de 13s, etc.). |
| Rentabilidad | Calcula la rentabilidad de una combinación. |
| Esperanza matemática **[Mac]** | Esperanza matemática de una columna por categoría (14 a 10), con precio, reparto, retención y mínimos configurables (ver §«Esperanza matemática»). |
| Columnas GEPT | Generador de columnas probables. |
| Tramificar | Separa columnas por tramos. |
| Premiadas | Utilidad de premiadas. |
| Estimación de Premios | Predice la cuantía de premios dada la ganadora. |
| Banco de Pruebas | Simulador de escrutinios (ver §10). |
| Compresor *.z3q | Comprime/descomprime archivos de columnas. |
| EstuCol | Generador/analizador de columnas probables a partir de reducidas y ganadoras. |

> **Pegar desde Excel [Mac]:** todas las tablas de porcentajes 1 / X / 2 (Esperanza matemática, Rentabilidad, Estimación
> de premios, Valoración, Banco de pruebas…) tienen el botón **«Pegar desde Excel»**. Copia en tu hoja las filas de
> porcentajes, una por partido, y púlsalo. De cada fila cuentan los **tres últimos números** (1, X y 2), así que puedes
> copiar también las columnas del nº de partido o de los equipos. Valen tabuladores, punto y coma o espacios, coma o punto
> decimal, y con o sin «%». Los encabezados y las filas vacías se saltan; si copias menos filas que partidos, el resto se
> queda como estaba y el programa te lo dice.

> **Otras herramientas** que no figuran en este menú: **Análisis de grupos**, **Reducciones perfectas** y **Dependencia lineal** (ver §10). En Windows están en la barra de herramientas (grupo Utilidades); en Mac, en el panel lateral (grupo Utilidades).

---

## 6. Las condiciones

Cada condición se abre con su botón en el campo Condiciones. Cada pantalla de condición permite introducir un **valor concreto o un rango** (mínimo–máximo) por cada signo. Tras introducir datos, se **activa** con el botón de aceptar; el botón de salir descarta sin guardar. La forma de rellenar es muy parecida en casi todas las condiciones.

**Botones comunes.** Todas las ventanas de condición tienen la misma botonera: **Aceptar** (✓, guarda y activa),
**Salir** (✗, descarta), **Abrir** y **Guardar** (cargan o guardan los valores de la condición en un archivo) y **Copiar** y
**Pegar** (pasan los valores de un grupo a otro).

> **Varios valores se separan con comas:** `0,1,2` o rangos como `2-6`. Con espacios (`0 1 2`) el programa responde
> «La cadena de entrada no tiene el formato correcto». **[Mac]** Si un valor no es válido, la condición se queda como
> estaba (no se activa vacía), así que «Calcular» no da 0 columnas por un error de escritura.

Las principales:

### Variantes, X y 2 (botón "Variantes, X y 2")
Limita el número de **variantes** (X + 2), y opcionalmente de **equis** y **doses**, mediante valores o rangos.

No hay casilla para los unos porque no hace falta: con 14 partidos, los unos son 14 menos las variantes, así que limitar
las variantes ya limita los unos. Con solo **Var = 9** vale cualquier reparto que sume 9 (3 equis + 6 doses, 0 + 9…); para
fijar el reparto exacto usa **Dibujos**.

> Otro ejemplo: con 6–10 variantes, 2–4 equis y 2–5 doses, la columna `11X211X1X12111` (3 equis y 2 doses) cumple las
> equis y los doses, pero se **rechaza**: sus 5 variantes no llegan al mínimo de 6.

> Ejemplo (según el manual oficial): mín. 4 / máx. 9 variantes, mín. 2 / máx. 6 equis, mín. 2 / máx. 6 doses.
> - Columna con **3 equis + 3 doses** → aceptada (6 variantes, todo en rango).
> - Columna con **5 equis + 6 doses** → **rechazada**: aunque equis y doses están en sus rangos, suman **11 variantes**, por encima del máximo de 9.

### Signos Seguidos (botón "S.Seguidos")
Limita el **máximo de signos iguales consecutivos** para `1`, `X`, `2` o variantes. En la columna `1XX11XXX22XX11` el análisis es: 2 unos seguidos, **3 equis** seguidas (hay un tramo de 3), 2 doses seguidos y 7 variantes seguidas.

Caso especial de los valores 0 y 1:
- Marcar **`1`** unos seguidos → como mucho hay *un* `1` aislado en cada tramo (pueden existir varios unos en la columna, pero nunca dos juntos). Ej.: `1XX12X1X22XX21`.
- Marcar **`0`** unos seguidos → fuerza que **no haya ningún** `1` en la columna. Ej.: `X2XX222XXX22X2`.

Como en las demás, se admiten rangos (p. ej. 2–10 variantes, 1–5 unos, 1–5 equis, 2–6 doses) o valores aislados.

### Dibujos (botón "Dibujos")
Forma **más exacta** de condicionar variantes: marcas los **dibujos** `X+Y` concretos permitidos (nº de equis + nº de doses), entre todos los posibles para 14 partidos. Mientras "Variantes, X y 2" con 7 variantes admite cualquier reparto (3+4, 2+5, 4+3…), en Dibujos eliges exactamente cuáles. Botones **Marcar Todos** / **Desmarcar Todos**. Otras condiciones activas pueden, a su vez, descartar algunos de los dibujos marcados.

*Ej.:* con «Variantes, X y 2» en 2–9 variantes y 2–6 equis y doses, una columna de 7 variantes puede ser 3+4, 2+5, 4+3…
Si en Dibujos marcas solo `4+3` y `5+2`, una columna con 3 equis y 4 doses queda fuera aunque también tenga 7 variantes.

### Interrupciones (botón "Interrupciones")
Condiciona por **cambios de signo**: interrupciones totales, por signo concreto, y seguidas (globales o por signo). Marcas el número o rango esperado; la condición acierta si las interrupciones de la columna ganadora caen dentro de lo indicado. (Recuerda: en `11X211X1X12111` hay 9 interrupciones totales, 4 del signo `1`, 6 seguidas globales y 3 seguidas de `1`.)

### Grupos de Equipos (botón "Grupos Equipos")
Condición "futbolística", organizada en **dos pestañas**:

- **Pestaña principal (Grupos Equipos).** Si has cargado el boleto, aparecen los nombres de los 28 equipos. Marcas un conjunto de equipos y exiges un nº o rango de **Victorias, Empates, Derrotas y Suma de puntos** (sistema español **3-1-0**). El color de los equipos solo indica su posición en el boleto, no aporta significado. *Ejemplo:* marcas 5 equipos y pides "al menos 3 victorias, 0 ó 1 empate, como máximo 2 derrotas, suma de puntos entre 10 y 15". Con los botones de navegación de grupos puedes crear un **segundo grupo** de equipos (p. ej. 7 equipos) con sus propios límites; el panel indica "Grupo 2 de 2".
- **Pestaña Relaciones.** Permite relacionar varios grupos entre sí. En el cuadro "Grupos Equipos" indicas qué grupos entran en la relación (p. ej. `1-2`) y defines la suma de victorias/empates/derrotas/puntos **conjunta** de esos grupos. Tiene su propio panel de navegación para crear tantas relaciones como quieras. Botones para eliminar el grupo de equipos en pantalla o borrar relaciones.
- **Cómo se rellenan.** Cada casilla admite un número o un rango con guion (`3-5`, `0-1`, `10-15`). Ejemplo completo:
  grupo 1 (5 equipos): Victorias `3-5`, Empates `0-1`, Derrotas `0-2`, Puntos `10-15`; grupo 2 (7 equipos): `3-5`,
  `0-4`, `1-4`, `10-14`; en **Relaciones**, Grupos `1-2` con suma de victorias `5-11`, de empates `3-7`, de derrotas
  `3-5` y de puntos `20-34`. Los puntos acotan más de lo que parece: en el grupo 1, «3 victorias, 0 empates y 2 derrotas»
  cumple las tres primeras casillas, pero suma 9 puntos y queda fuera.
- **Ideas de uso.** Que los 10 primeros de la clasificación sumen más puntos que los 10 últimos, jueguen donde jueguen; o
  que los 4 favoritos de tu CP saquen entre 5 y 10 puntos (como poco 1 victoria y 2 empates, como mucho 3 victorias y
  1 empate).

### Distancias (botón "Distancias")
Limita la **distancia** (mayor separación entre dos apariciones del mismo signo) para `1`, `X`, `2` y variantes, con valores o rangos.

### Valoración de signos
Asigna un **valor** (normalmente en %) a cada signo de cada partido y exige que la **suma** de los valores de los signos elegidos en la columna esté en un intervalo. Sirve para sesgar la combinación hacia los signos que crees más probables, en vez del reparto matemático al 50%/33%. *Ejemplo:* dos partidos a triple valorados `50,30,20`; con intervalo de suma 60–100 quedan excluidas las combinaciones `X2`, `2X` y `22` (suman 50, 50 y 40), pero sí entra la sorpresa `2` si el otro partido aporta suficiente valor.

También sirve para **inclinar un doble**: si juegas `1X` pero crees que el `1` es mucho más probable, valóralo 75/25 en
lugar del 50/50 que daría el reparto matemático.

### Otras condiciones disponibles
Según la versión, el campo Condiciones incluye además:
- **Figuras de signos / Figuras de zonas** — condicionan la *forma* de agrupación de los signos (figura global, como `32211`) y su reparto por zonas 7+7 o Par-Impar (ver §2).
- **Formatos de Signos / Formatos 123** — controlan la aparición o repetición de **secuencias** de signos de tamaño 2 (parejas), 3 (tríos), 4 (cuartetos)… En `11X211X1X12111` el formato `1X` aparece 3 veces, `1X2` una vez. También puede pedirse que aparezcan *N* formatos distintos.
- **Simetrías** — obliga a que determinados partidos (o equipos) tengan el **mismo** signo. Suele combinarse con "que se cumplan solo 1 ó 2 de las simetrías impuestas" para no arriesgar de más; por sí sola puede ahorrar ~35% de apuestas.
- **Repeticiones / Diferencias** — definen grupos de partidos y exigen que tengan la **misma** cantidad de variantes/equis/doses, el mismo dibujo, formato o interrupciones (sin importar el valor concreto, solo que coincida).
- **Pesos Numéricos** — distribuye numéricamente (del 0 al 9) las columnas posibles, de forma global o por aparición de signos. El reparto suele ser equitativo, así que se usa sobre todo para **evitar** columnas cuyo peso coincida con el de la jornada anterior u otros controles estadísticos. (Condición originaria del programa Premium©.)
- **Contactos** — condición específica de contactos entre signos/partidos.

Todas siguen el mismo patrón: abrir → introducir valores/rangos → activar.

### Tolerancias y Condiciones Relacionadas
- **Tolerancias** — permite que un nº limitado de condiciones se cumpla en un valor "tolerado" fuera del rango estricto.
- **Condiciones Relacionadas (If-Then)** — reglas lógicas del tipo "si se cumple A, entonces exige B".

### Grupos y control de fallos
Las condiciones se organizan en **grupos**. Un grupo es una agrupación independiente de condiciones que forman un todo; puedes crear varios e ir navegando entre ellos para definir y combinar subsistemas dentro de una misma combinación. Su utilidad principal es **condicionar unos grupos contra otros mediante el control de fallos**: p. ej. poner en un grupo "5–9 variantes y 4–9 interrupciones" y en otro "10–12 variantes y 6–10 interrupciones", y pedir que el nº de fallos entre ambos grupos sea 1 → así se cumple **uno u otro** (cualquiera de los dos sistemas vale). A nivel avanzado, los conjuntos de grupos pueden usarse como una condición más sobre los aciertos de los controles de grupos.

---

## 7. Columnas Probables (CP)

La condición **más potente y versátil** (la "condición de condiciones"). Una CP es un **segundo pronóstico** sobre un grupo de partidos en el que defines un **rango de aciertos** deseado (en vez de exigir acertarlo todo).

**Ejemplo de los 4 grandes (paso a paso).** Cuatro "grandes" (Madrid, Barça, Valencia, Deportivo) juegan en casa. En vez de gastar 4 triples —que por sí solos son **81 apuestas**— por si alguno pincha:
- Creas una CP donde los 4 ganan y pides **3 ó 4 aciertos** (1 o ningún fallo): la combinación de esos 4 partidos baja a **9 apuestas**. Si el Madrid pierde en casa pero los otros 3 ganan, lo tienes cubierto.
- Si admites que fallen **hasta 2** (de 2 a 4 aciertos): **33 apuestas** (un 60% menos que las 81 originales).
- Y como crees difícil que dos grandes *pierdan* en casa el mismo día, añades una **segunda CP** con los 4 doses pidiendo **0 ó 1 aciertos**: la combinación queda en **27 apuestas**.

**Ejemplo de restricción progresiva.** Partiendo de 6 triples + 6 dobles + 2 fijos = **46 656 columnas**:
- CP nº 1: convierte los 6 triples en 6 dobles (los signos más probables) y pide **3–5 aciertos** → 37 888 columnas (~20% menos).
- CP nº 2: convierte los 6 dobles originales en 6 "fijos" y pide también 3–5 → 24 272 columnas (~48% menos).
- Si en ambas hubieras pedido **4–6**, te quedarías en 10 912 columnas (como "2 dobles de más" por el mismo precio).

Puedes poner **decenas, cientos o miles de CPs**, cada una con sus condiciones, y **relacionarlas** entre sí. Conceptos de relación entre CPs:

| Relación | Qué hace | Ejemplo |
|----------|----------|---------|
| **Aciertos seguidos / fallos seguidos** | Limita el máximo de aciertos (o fallos) consecutivos en una CP. | CP de 11 partidos: aciertos seguidos de 3 a 6. Útil con columnas de especialistas. |
| **Tolerancias (fallos permitidos)** | Permite que solo *n* CPs salgan a un valor "tolerado" fuera del rango estricto. | Dos CPs en 4–5 aciertos con 3 y 6 marcados como tolerancia, permitiendo que **solo 1** salga a un valor tolerado. |
| **Suma de aciertos** | Condiciona la **suma** de aciertos de varias CPs. | Dos CPs a 3–6 aciertos cada una, exigiendo que la suma esté entre 7 y 11. |
| **Recorridos** | Limita la diferencia entre la CP con más aciertos y la de menos. | Rangos `4,6,5,8,7…` → recorrido 4 (8−4). Bueno con grupos de CPs de una misma base. |
| **Grupos de columnas** | Exige que un nº limitado de CPs tenga un intervalo de aciertos más cerrado. | De 10 CPs, "entre 2 y 3 con exactamente 4 aciertos" (no se sabe cuáles). |
| **Coincidencias** | Agrupa CPs por nº de aciertos *coincidente* sin acotar el grado. | "2–3 grupos con 2 coincidencias, 1 grupo con 3, 1–2 aciertos aislados". |
| **Relaciones lógicas** | Reglas "si–entonces" entre CPs. | "Si la CP A acierta hasta 3, la CP B debe acertar al menos 5"; o "la columna de B siempre con más aciertos que la de A". |

**Las relaciones en números.** Partiendo de 6 triples + 6 dobles + 2 fijos y de las dos CPs del ejemplo anterior:

| Planteamiento | Columnas |
|---------------|---------:|
| Las dos CPs con 3–6 aciertos | 27.552 |
| Las dos con 4–5, marcando 3 y 6 como tolerancia y aceptando que **como mucho una** caiga en un valor tolerado | 22.848 (≈4.700 menos) |
| Las dos con 3–6 y la **suma** de aciertos entre 7 y 11 | 24.288 (≈3.300 menos, más de un 10 %) |

La suma solo descarta los extremos (3+3 y 6+6): ahorra bastante con poco riesgo.

*Recorridos y coincidencias, ejemplo completo:* si un grupo de 10 CPs dio en una semana `4,6,5,8,7,7,6,4,5,6` aciertos y en
la siguiente `4,2,3,5,7,4,5,2,4,3`, los recorridos fueron 4 (8−4) y 5 (7−2), y todos los valores cayeron entre 2 y 8. En
las dos semanas hubo tres parejas de columnas con los mismos aciertos, un trío y una columna aislada: ese reparto repetido es
lo que se condiciona con **Coincidencias**.

> Las CPs permiten **dirigir** la combinación hacia los signos que crees más probables y jugar "por encima de tu presupuesto" controlando el riesgo. Conviene incluir siempre, al menos, una CP con un pronóstico arriesgado que admita algún fallo.

### Generador CP (menú Utilidades)
Genera columnas probables a partir de una **valoración** que tú proporcionas. Procedimiento:

1. **Configurar las columnas.** Pulsa el botón correspondiente; en la tabla que se abre indicas, de entrada, solo un **nº de orden** y un **nombre** (ese nombre se usará para el fichero `nombre_xxxxx.txt` que se cree).
2. **Definir la creación.** Pulsa el `+` a la izquierda del nº de orden → se despliega "Configuración"; al pulsarla se abre una tabla con estos campos:
   - **Desde / Hasta** — intervalo de porcentajes para aceptar un signo. Si la valoración de un signo cae entre ambos, el signo entra. *Ej.:* de 20 a 30, con valoración `50,30,20`, genera el pronóstico `X2`.
   - **Forzar fijos / Nº de fijos** — genera solo columnas de fijos: los *N* partidos más fijos según la valoración.
   - **Forzar dobles / Nº de dobles** — análogo, con los *N* partidos más dobles.
   - **Forzar triples** — evita pronósticos vacíos: si ningún signo de un partido cae en el rango Desde–Hasta, se genera un triple, asegurando 14 partidos con signo.
   - La definición se separa en 3 bloques: **pronóstico libre** (Desde/Hasta + forzar triples), **fijos** y **dobles**. Si fuerzas fijos o dobles, Desde/Hasta se ignoran (aun así se recomienda dejarlos en 0–100).
   - Ejemplos de configuración (`Desde,Hasta,ForzarFijos,NºFijos,ForzarDobles,NºDobles,ForzarTriples`): `0,100,sí,14,…` = 14 fijos · `0,100,…,sí,14,…` = 14 dobles · `0,100,sí,6,…` = 6 fijos · `20,30,…` = todos los signos con valoración entre 20 y 30.
   - La ventana tiene 15 filas de valoración (incluye el Pleno al 15), un campo **Jornada**, **Importar %** y accesos a
     **Separador de porcentajes** y **CPs por diferencias**.
   - *Forzar fijos:* con 7 fijos, la columna lleva los 7 partidos más claros según la valoración. *Forzar triples:* con
     Desde 20 – Hasta 30 y un partido valorado 34/33/33 no entra ningún signo; con la casilla marcada, ese partido sale
     `1X2`.
   - Más ejemplos: `0,100,…,sí,8,…` = 8 dobles; `20,30,…,sí` (forzar triples) = signos entre 20 y 30 y triple en los
     partidos que queden vacíos. Con las seis configuraciones de ejemplo se obtiene un `nombre_xxxxx.txt` de **6
     columnas**, una por configuración.
3. Pulsa **Guardar**.
4. **Generar.** Rellena las valoraciones (a mano o **Importar** desde fichero) y pulsa **OK**: se generan todas las columnas definidas según la valoración indicada.

**Columnas GEPT** y **Diferencias entre columnas** son vías alternativas para crear columnas probables (esta última se usa principalmente para CPs, no para las columnas finales a sellar: parte de una columna base y busca columnas con un nº mínimo–máximo de diferencias respecto a ella y entre sí; usa la letra `F` en un partido para que no se tenga en cuenta).

---

## 8. Filtros

Un **filtro** es un archivo de columnas que tiene una cierta cantidad de aciertos y que usas como **base de partida** (o como paso intermedio para obtener otro). Lo interesante de un filtro no es solo que contenga el 14 con frecuencia, sino que sea **regular** (que suela contener 13, 12… aciertos); a partir de un filtro regular puedes obtener otros con las utilidades de Free1X2.

Cuando hay un filtro activo, una columna solo se acepta si **además** de cumplir todas tus condiciones, **está contenida** en el archivo del filtro. Es decir, el resultado es siempre un **subconjunto** del filtro.

**Cómo usarlo:** en el campo Filtro, pulsa el icono de carpeta y elige el archivo de columnas. El icono pasa a "X" y el indicador derecho se enciende (filtro activo). Puedes:
- **Desactivarlo** (clic en el cuadro izquierdo): sigue cargado pero no se aplica.
- **Borrarlo** (icono "X"): lo descarga; para volver a usar uno hay que abrirlo de nuevo.

**De dónde sacar filtros (según el manual oficial).** En la web de la comunidad de Free1X2 hay numerosos filtros creados por los usuarios, distinguiendo entre **filtros Genéricos** (válidos para cualquier jornada) y **filtros Semanales** (publicados semana a semana, válidos solo para la jornada en curso). También suele haber un seguimiento del rendimiento de los filtros publicados.

**Utilidades de filtros** (menú Filtros): combinar filtros, diferencias entre filtros, y filtros específicos (Coincidencias, "Aidomnou", "Pim").

**[Mac] Columnas de Inicio.** Combinar filtros, Diferencias entre filtros y Coincidencias tienen un botón **«Columnas de
Inicio»** junto a la carga de ficheros; Aidomnou y Pim, **«Calcular con columnas de Inicio»**. El programa desarrolla el
boleto de Inicio con sus condiciones (y el filtro de columnas, si hay uno) y lo usa como fichero de entrada, sin que
tengas que calcularlo y guardarlo antes. El resultado se guarda después, si quieres, con el botón de grabar de cada
pantalla.

---

## 9. Reducción de columnas

Cuando el cálculo da más columnas de las que quieres jugar, hay **tres formas** de reducir:

1. **Cambiar el pronóstico** (lo más efectivo). Reducir triples y dobles:
   - Triple → doble: divide las columnas por **1,5**.
   - Doble → fijo: divide por **2**.
   - Triple → fijo: divide por **3**.
   Arriesga más en el pronóstico, pero es el mejor método.
2. **Añadir/apretar condiciones.** Terreno delicado: cada condición extra es una opción de fallo. Mejor apretar los rangos de tus condiciones que añadir otras nuevas que no tengas estudiadas.
   *Ej.:* si de 600 columnas quitar las de 12 variantes solo ahorra 10, no compensa. Si de 600 hay 300 con 9 variantes y
   las quitas para jugar 300, asume que si salen 9 variantes tu techo será el 13.
3. **Jugar una reducción** (`Combinación → Reducir`). Algoritmos que, partiendo de un conjunto mayor, garantizan **como mínimo un premio de la categoría elegida** con muchas menos columnas (p. ej., reducir al 12 garantiza al menos un 12 si la madre tenía el 14). Algunas reducciones garantizan un **porcentaje** del premio a cambio de menos columnas. Free1X2 incluye varios algoritmos (JDC, JLPM, XFSF, TM…).
   Como orden de magnitud, una reducida al 12 puede quedarse en torno al 10 % de las columnas de la madre, y una que
   garantiza el 13 «al 95 %» puede tener un 15 % menos de columnas que la del 100 %.

**[Mac] Reducir sin guardar antes un archivo.**
- En Inicio, **Reducir** desarrolla el boleto (con sus condiciones) y abre el reductor **con esas columnas ya cargadas**
  como entrada. En el reductor, el botón **«Columnas de Inicio»** hace lo mismo.
- Si no eliges «Archivo salida», la reducción va a un archivo temporal y, al terminar, **«Guardar resultado…»** la
  guarda donde quieras. Puedes probar varios niveles y métodos y guardar solo la que te convenza.
- Si el boleto pasa del **millón de columnas** (14 triples son 4.782.969), el programa avisa antes, porque desarrollarlo
  tarda (del orden de medio minuto) y ocupa mucho.

> Regla de oro: **comprueba siempre el ahorro y el efecto en caso de fallo** de cada decisión. No quites variantes que apenas ahorran columnas, ni te cargues el sistema por reducir a ciegas.

---

## 10. Utilidades

Resumen rápido (el detalle de las más completas viene a continuación):

| Utilidad | Para qué sirve |
|----------|----------------|
| **SubeCategoría** | Toma cada columna de un `.txt` y añade todas las que se diferencian en un solo signo (eliminando repetidas). Sube la categoría de premios. |
| **Modificador %** | Cambia la frecuencia de signos de uno o más partidos de un archivo de columnas. |
| **Generador CP / Columnas GEPT** | Generan columnas probables (ver §7). |
| **Diferencias entre columnas** | Genera columnas a cierta distancia (diferencias) de una columna/fichero base. Se usa sobre todo para crear CPs. |
| **Ordenar por Probabilidad** | Ordena las columnas según una valoración de probabilidad / nº de acertantes. |
| **Rentabilidad** | Calcula la esperanza matemática (rentabilidad) de cada columna. |
| **Selector (JuanM)** | Escoge apuestas por probabilidad en 3 grupos (5+5+4 partidos) y los multiplica. |
| **Selector (MarioSan)** | Escoge columnas en función del nº de 13s que las acompañarían. |
| **Tramificar** | Divide las columnas en tramos de igual densidad y analiza dónde caen los premios; incluye Gráficas. |
| **Premiadas** | Analiza un fichero de ganadoras contra los 14 triples: veces que cada columna fue premiada. |
| **Estimación de Premios** | Predice acertantes y cuantía de cada premio dada la ganadora y las valoraciones apostadas. |
| **Simulador de Escrutinios (Banco de Pruebas)** | Prueba/compara combinaciones contra 14s reales o simulados. |
| **Álgebra / Transposición / Multiplicador / Fraccionador / Rotación** | Operaciones sobre archivos de columnas. |
| **Gestión de Equipos** | Mantiene los equipos de las 4 categorías (1ª, 2ª, 2ªB e Internacionales). |
| **Importar / Exportar Columnas** | Convierte ficheros de columnas entre CSV y TXT. |
| **Compresor *.z3q** | Comprime/descomprime archivos de columnas a un formato propio. |
| **EstuCol** | Genera/analiza columnas probables a partir de reducidas y ganadoras. |
| **Análisis de grupos** | Cuenta cuántas columnas de un fichero encajan en cada combinación de un grupo/patrón. |
| **Reducciones perfectas** | Genera reducciones "perfectas" (4/13/11 triples ó 7/15 dobles) a partir de una base. |
| **Dependencia lineal** | Recalcula el signo de un partido como combinación lineal de los demás. |
| **Máscaras** **[Mac]** | Guarda los signos del boleto para reutilizarlos cada jornada y compara tus máscaras contra las ganadoras históricas. |

### SubeCategoría
Coge cada columna de un fichero `.txt` y añade todas las que se le diferencian en un solo signo, eliminando repetidas. Opera sobre los 14 partidos por defecto, pero puedes restringir los **partidos involucrados** (solo esos cambiarán de signo). En **Niveles** indicas cuántas categorías subir: el archivo final contiene las columnas de los niveles marcados (marca el **Nivel 0** para incluir también las columnas originales). Entre "Calcular" y "Grabar" hay una casilla informativa que muestra el nº de columnas de origen (`I=`), resultantes (`F=`) y grabadas (`G=`).

- Si desmarcas un **partido involucrado**, su signo no cambia y **se pierde la garantía** cuando ese partido esté fallado.
- Se pueden marcar varios **niveles** a la vez: `1` y `2` graban las columnas que suben 1 o 2 categorías; solo `2`, las
  que suben 2. Marca también el `0` para conservar las originales: partiendo de una sola columna, el nivel 1 da **28**
  (14 partidos × 2 signos alternativos) y los niveles 0+1, **29**.
- *Ej. de la casilla informativa:* `I=8604` al elegir el origen, `F=151000` tras Calcular y `G=151000` tras Grabar.
- Se puede grabar con otro nombre, y el título de la ventana muestra la combinación con la que trabajas.

### Modificador % (modificador de columnas)
Cambia la **frecuencia de signos** de uno o más partidos de un archivo. *Ej.:* un partido apostado `30,20,50` que crees que infravalora la `X` lo pasas a `25,30,45`. Procedimiento: abrir el `.txt`, seleccionar partido(s), poner los nuevos porcentajes (si no suman 100 se ajustan proporcionalmente), elegir el **tipo de inserción** y aceptar:
- **Aleatorio** — reparte al azar hasta el tope de cada frecuencia.
- **Proporcional** — rellena en grupos proporcionales (10 unos, 20 equis, 50 doses → grupos de 1/2/5).
- **Ordenado** — todos los 1 seguidos, luego las X, luego los 2.

Con "Ordenar signos" (no válido para aleatorio) coloca primero el signo más probable. Es un editor automático; después conviene pasar el resultado por `Operaciones → Álgebra → eliminar columnas repetidas`, porque el modificador no comprueba duplicados.

- Sin «Ordenar signos», los bloques van siempre en el orden `1`, `X`, `2`. La combinación recomendada es
  **Proporcional + Ordenar signos**, que deja al principio y juntos los signos más probables.
- **Qué esperar** (pruebas de los autores con jornadas reales): si aciertas el nuevo reparto, sueles ganar alguna columna
  premiada sin jugar más (p. ej. pasar de 2 a 3 columnas de 10); si lo fallas, puedes quedarte igual o perder premios. En
  torno a la mitad de los aciertos hubo **subida de categoría**: en un caso documentado, una jugada de 1.138 columnas pasó
  de 10 columnas de 10 aciertos a 3 de 11 y 8 de 10 cambiando un solo partido a 15/35/50.

### Ordenar por Probabilidad
Ordena un fichero (o los 14 triples) por proximidad a un **valor central de ordenación**: las columnas más próximas quedan al principio del fichero de salida. El valor central se puede expresar de 4 formas equivalentes —**nº de acertantes**, **importe del premio de 14**, **probabilidad** (producto de los tantos por uno de cada signo) o **logaritmo neperiano** de la probabilidad (para usar las estadísticas de PacoHH)— y al fijar una, las otras tres se recalculan. Pasos: elegir 14 triples o fichero → importar/escribir valoraciones → indicar fichero de salida → definir el valor central → "Ordenar". Con "añadir valor de ordenación" el fichero de salida lleva ese valor junto a cada columna.

- **Opciones de salida:** nº máximo de columnas, límite de probabilidad acumulada y casillas para añadir a cada columna la
  probabilidad acumulada o el premio de 14. Pestañas **Por productos** (probabilidad), **Por sumas** y **Múltiple**.
- **Obtener intervalo: premio de 14 entre … y …** **[Mac]**: con la casilla marcada solo se graban las columnas cuyo
  premio de 14 estimado (con la Configuración L.A.E. de la ventana) cae entre los dos importes; las demás se saltan, pero
  la probabilidad acumulada y su límite siguen contándolas. Desmarcada, todo queda como siempre. No se aplica a la pestaña
  Múltiple. *Ej.:* con 15.000.000 €, 0,5 € y 15 %, la columna `11111111111111` con 50 % para el 1 en los 14 partidos da
  0,075 ÷ 0,5¹⁴ ≈ **1.228,8 €**: entra en un intervalo de 1.000 a 2.000 €.
- **Configuración L.A.E.** (recaudación, precio de la apuesta y % del 14): con ella se pasa de una forma del valor central a
  las demás. El nº de acertantes es la probabilidad × (recaudación ÷ precio), y el premio de 14 es la recaudación × % del 14
  ÷ acertantes. *Ej.:* con 13.000.000 €, 0,5 € y 15 %, **1,5 acertantes** = premio de **1.300.000 €** = probabilidad
  **5,77·10⁻⁸** = logaritmo **−16,67**.
- Si eliges el logaritmo, una lista ofrece los valores más probables según las estadísticas reales de PacoHH.
- El **valor de ordenación** que se añade a cada columna es la diferencia, en valor absoluto, entre el logaritmo del valor
  central y el de la probabilidad de la columna.

> **Nº de acertantes < 1.** Cuando el nº teórico de acertantes baja de 1, el premio supera el 15% de la recaudación; se mantiene como premio teórico para conservar la relación con el nº de acertantes, y aparece un aviso en la barra de estado. La recaudación, el precio de la apuesta y el % del 14 salen de **Configuración L.A.E.**; en una instalación nueva de la
versión Mac son 14.000.000 €, 0,75 € y 12 %.

### Rentabilidad
Una apuesta es **rentable** si `CosteApuesta < Premio × Probabilidad`. Ese producto es la **esperanza matemática (EM)**. La utilidad calcula, columna a columna, el premio de 14 (a partir de los % apostados) y la probabilidad (a partir de las valoraciones probables), y de ahí la EM. Como solo considera el premio de 14, el umbral de rentabilidad no es el coste íntegro: se recomienda una **EM mínima de 0,136 €** (el 27,27% del coste de 0,5 €, precio vigente cuando se escribió el manual oficial; con el precio actual de 0,75 € recalcula este umbral proporcionalmente). Procedimiento: introducir % apostados y % de probabilidad de los 14 partidos → elegir 14 triples o fichero → fichero de salida → fijar EM mínima (0,136) y máxima → "Calcular". Puedes ordenar de mayor a menor EM y analizar una única columna escribiendo sus 14 signos.

*Ej.:* una columna con probabilidad 0,000001 sale una vez de cada millón; jugarla un millón de veces cuesta 500.000 €
(a 0,5 €), así que solo es rentable si su premio supera esa cifra. Si al 14 solo le pidieras recuperar su parte de la
recaudación (15 %), el umbral sería 0,075 € por apuesta; el 0,136 € recomendado sale de repartir esa parte en proporción a
lo que va a premios (15/55).
**La ventana** pide valoraciones **apostadas** (izquierda) y **reales** (derecha), el origen (14 triples o fichero), el
fichero de salida, la **recaudación considerada** y la **EM mínima y máxima**, con las opciones «Ordenar columnas por EM» y
«Añadir EM al fichero salida». En «Calcular valoración de una columna» escribes 14 signos y ves el premio estimado de 14, la
probabilidad real y la EM. Los botones de cálculo se activan cuando están todos los datos.

### Diferencias entre columnas
Genera columnas que están a una distancia controlada (nº de signos distintos) de una referencia y entre sí. Se usa sobre
todo para fabricar CPs, no las columnas que vas a sellar.
- **Generación por:** los 14 triples o un fichero. De ahí salen las candidatas, en ese orden.
- **Condicionada por:** una **columna base** o un **fichero** de columnas base.
- **s/col.Base:** diferencias mínimas y máximas de cada columna con la base (p. ej. `0-5,6,7-14`). `0-14` = la base no
  cuenta.
- **s/col.result.:** diferencias de cada columna con las ya aceptadas (p. ej. `6-14`).
- Una **`F`** en un partido de la base lo deja fuera de la cuenta: `111F1111F1111F`.
- Bajo **Calcular** aparecen las columnas admitidas y el tiempo; después, **Grabar** y **CPs por diferencias…**.

*Ej.:* con los 14 triples y la base `11111111111111` a 2 diferencias, la primera admitida es `111111111111XX`. La
siguiente candidata, `111111111111X2`, se rechaza: está a 2 de la base, pero solo a 1 de la anterior.
Con los 14 triples la primera candidata es siempre `11111111111111`, así que unos mismos rangos dan siempre el mismo
resultado; para obtener otro, usa como origen un fichero con los 14 triples desordenados. Contra muchas columnas base el
resultado suele ser 0 columnas.

### Selector (JuanM)
Escoge apuestas de mayor a menor probabilidad en **tres grupos** (dos de 5 partidos = 243 columnas cada uno, y uno de 4 = 81 columnas) y los **multiplica**. Pasos: (1) introducir valoraciones; (2) asignar cada partido al grupo 1, 2 ó 3 (cinco al 1, cinco al 2, cuatro al 3); (3) poner el **límite** mín–máx de columnas de cada grupo. *Ej.:* grupo 1 → 60, grupo 2 → 40, grupo 3 → 27 da `60 × 40 × 27 = 64 800` columnas (las 60 más probables del grupo 1, las 40 del 2 y las 27 del 3).

Los límites por defecto son 1–243, 1–243 y 1–81 (todas las columnas de cada grupo) y hay un límite **total** (1–567).
La ventana tiene también dos modos de examinar (**por corte** / **por valor**), **leer/salvar condiciones** y un bloque de
**análisis de resultados** contra un fichero de ganadoras o una columna ganadora.

### Selector (MarioSan)
Parte de una combinación final que, según tus estadísticas, suele encerrar el 14 con un nº de 13s en un rango estable. Califica cada columna con el **nº de 13s** que la acompañarían si fuera ganadora, repartiéndolas en **29 grupos** (de 0 a 28 treces). Pasos: "Iniciar" → elegir fichero → el programa muestra la distribución → marcar las filas deseadas (con `Ctrl` para varias) → "Grabar". *Nota:* la selección solo garantiza el 14, no los treces.

*Ej.:* un fichero de 2.292.535 columnas se clasificó en unos 33 s. La tabla «distribución» indica cuántas columnas hay con
cada nº de treces (p. ej. 1.989 con 11 … 446.794 con 28). Seleccionando las filas 11 a 16 y pulsando **Sumar** se ve que se
grabarían 98.455 columnas.

### Columnas GEPT
Calcula automáticamente la **columna estadística de Abdon** (José Carlos), que él publicaba junto a sus columnas habituales.
1. En la 1.ª columna escribe, para cada equipo **local** de la jornada, su último resultado: `G` (ganó), `E` (empató) o
   `P` (perdió).
2. En la 2.ª, lo mismo para cada equipo **visitante**.
3. Pulsa **Calcular**: la 3.ª columna da el pronóstico.

Cuenta el último partido **de liga**, no los de copa ni los europeos. Si el partido de un equipo se aplazó, espera a que se
juegue o usa el último que se disputó. Según las estadísticas de su autor, con los 14 partidos suele acertar entre 5 y 10;
lleva tus propias estadísticas, sobre todo si solo la aplicas a una parte del boleto (p. ej. los 10 de 1.ª).

### Tramificar y Gráficas por tramos
Idea: en vez de repartir las 4.782.969 columnas por rangos de probabilidad, se ordenan por probabilidad y se dividen en **tramos con el mismo nº de columnas** (igual densidad), para descubrir en qué tramos se concentran los premios de 14, 13, 12, 11 y 10. Recuerda que cada jornada salen 28 columnas con 13, 364 con 12, 2.912 con 11 y 16.016 con 10.

**Datos:** valoraciones de los 14 partidos (a mano, de fichero o del portapapeles) y la **definición del tramo** (Columna inicial / Columna final —para los 14 triples, 0 y 4782969—, y **Cols. por tramo** o **Nº de tramos**). Para que todos los tramos tengan igual densidad conviene usar un nº de tramos potencia de 3 (9, 27, 81, 243…). **LN central** fija el valor central de ordenación (0 = de más a menos probable). Para analizar una jornada pasada se añaden: columna premiada, temporada, jornada, premios de 14/13/12/11/10 y recaudación (recuperables automáticamente desde el menú L.A.E. si la jornada ya se jugó).

**Tabla de resultados** (una fila por tramo):

| Columna | Significado |
|---------|-------------|
| **Nº** | Número de tramo. |
| **Col. Inf. / Col. Sup.** | Primera y última columna del tramo. |
| **Nº Col.** | Columnas que contiene el tramo (el último puede tener menos). |
| **Prob. Max.** | Probabilidad (o valoración por sumas) de la columna superior del tramo. |
| **14, 13, 12, 11, 10** | Columnas premiadas en cada categoría dentro del tramo. |
| **Nº Premios** | Total de columnas premiadas del tramo (sin distinguir categoría). |
| **Imp. Premios** | Importe en euros de los premios del tramo. |
| **Ingresos-Gastos** | Balance: premios obtenidos − coste de jugar todas las columnas del tramo. |

Se puede **Grabar Tramo** (uno o varios) a un fichero, **acumular** resultados de varias jornadas, analizar **jornadas múltiples** (14 triples o un fichero por jornada) y **filtrar** por los límites de posición de cada categoría (eliminando columnas y las que se les diferencian en *n* signos sin riesgo de perder el 14). El "Resumen de posiciones máximas y mínimas" indica el ancho de existencia de cada categoría de premio.

**Premios de referencia.** Para comparar jornadas puedes fijar tus propios premios (p. ej. 2.401, 343, 49, 7 y 1 € para el
14…10, cada categoría 7 veces la siguiente) y bloquearlos con el candado de cada casilla para que no cambien al cambiar de
jornada. Los datos de una jornada se guardan con `L.A.E. → Guardar Jornada`; ese menú tiene un enlace a los sorteos
oficiales. La valoración (menú *Valoraciones*) puede ser **Por productos** (probabilidad, por defecto) o **Por sumas**.

**Grabar Tramo:** uno o varios tramos a un fichero, con la posición inicial y final y la opción de grabar todas las
columnas o alternas (por defecto, todas).

**Filtrar** (con varias jornadas analizadas): los límites de posición de cada categoría permiten eliminar columnas sin
riesgo. *Ej.:* si en ninguna jornada hubo un 10 entre las posiciones 1 y 5, puedes quitar esas columnas y las que estén a
4 signos o menos de ellas sin perder nunca el 14. Valores por defecto: por debajo de 10 aciertos, eliminar las que estén a
0–4 diferencias; de 10 a 11, a 1–3 (las del propio intervalo se quedan); de 13 a 14, «1 a 0» (no elimina nada). Para
quitar solo las columnas del intervalo, «0 a 0». Al Aceptar se ve cuántas quedan y **Guardar filtro** las graba; desde ese
momento los tramos solo contienen las filtradas. Los límites se pueden guardar, abrir, copiar y pegar (p. ej. a una hoja
de cálculo).

**Ver columna:** muestra la columna de una posición (y sus aciertos si fue premiada); avanza de una en una o salta a la
siguiente o anterior premiada de la categoría mínima elegida; **Buscar** da la posición de una columna escrita, y se
muestra su logaritmo. Si analizas un fichero sobre los 14 triples, las columnas que no están en el fichero salen en rojo.

**Jornadas múltiples** (`Análisis → Jornadas múltiples`): pestaña **14 Triples** (eliges temporadas; hay botones para
marcarlas o desmarcarlas todas) o **Ficheros** (lista Temporada–Jornada–Fichero con Añadir, Eliminar, Guardar y Leer; la
jornada sube sola al añadir). Necesita un fichero de valoraciones **históricas** y permite elegir las jornadas por premios o
recaudación. Con ficheros de distinto tamaño conviene marcar «Analizar ficheros dentro del desarrollo de los 14 triples».
Aceptar → Tramificar.

**Gráficas.** Desde el menú "Gráficas" se visualizan los resultados; hay 9 gráficas (probabilidades; posiciones de los 14/13/12/11/10; nº de columnas premiadas por tramo; importe de premios por tramo; beneficio por tramo). En el eje X van los tramos y se pueden superponer varias.

### Premiadas
Elige antes la categoría que quieres estudiar (13, 12, 11 o 10) y el fichero de ganadoras.
Escruta un fichero de columnas **ganadoras** contra los 14 triples y cuenta cuántas veces cada columna de los 14 triples resultó premiada. En "Frecuencias" se lee, p. ej., `784 = 1 veces` → hay 784 columnas que salieron premiadas 1 vez. Con "Analiza selección" se ve en qué jornadas ocurrió (para estudiar regularidad) y con "Grabar selección" se vuelcan esas 784 columnas a un fichero para escrutarlas.

### Estimación de Premios (con Escrutinio Real)
Predice acertantes y cuantía de cada premio. Lo básico: introducir las **valoraciones apostadas** de la jornada y definir la **columna ganadora** → aparece la estimación.
- **Valoraciones:** a mano (si la suma de un partido no está entre 99 y 101 se marca en rojo) o desde fichero (varios formatos, incluido histórico). Con histórico, al cambiar de jornada se cargan valoración, ganadora y premios reales.
- **Columna ganadora:** clic sobre cada casilla cicla `1 → X → 2 → 1`; cada cambio recalcula los premios.
- **Resultados ("Previsión de premios"):** el nº de acertantes es entero salvo el de 14, que se muestra **decimal** porque cuando hay pocos acertantes interesa distinguir 1,1 de 1,8, o 0,3 de 0,9. Para acertantes **< 1**, el premio no supera el máximo que marcan la recaudación y el % asignado al 14.
- **Recaudación / bote / % a premios:** ajustables; el bote solo afecta al 15 (no contemplado en la utilidad).
- **Precio de la apuesta [Mac]:** arranca con el de **Configuración ▸ Parámetros LAE** y se puede cambiar para el cálculo.
- **Escrutinio Real:** muestra los datos reales de la jornada (leídos del fichero de información de jornadas del L.A.E.); se pueden introducir/actualizar a mano y guardar con el botón del disquete asociándolos a la jornada del selector. Hay un enlace directo a la página oficial de resultados del L.A.E.
- La **recaudación no cambia el premio estimado**, pero sí el **nº de acertantes**: ajústala para que la previsión de
  acertantes se parezca a la jornada real.
- **Ficheros de valoraciones:** 3 valores por fila, 1 por fila o todos en una fila, separados por comas, espacios o
  tabuladores. También admite ficheros de **valoraciones múltiples** (eliges una de la lista) e **históricas**. Si las
  escribes a mano, sal de la última celda para que el valor se acepte.
- Si la jornada no está guardada, la ganadora empieza en `11111111111111`. El selector de jornadas abre la temporada en
  curso en la jornada 0.
- **Reparto por categorías** (valores de 2005/06 del manual original): 12 % al 14 y 8 %, 8 %, 8 % y 9 % al 13, 12, 11 y
  10. *Ej.:* con 14.000.000 € (28.000.000 de apuestas a 0,5 €) quedan 1.680.000 € para el 14, 1.120.000 € para cada una de
  13, 12 y 11 y 1.260.000 € para el 10. Solo se guarda el % del 14.
- *Ej. real (jornada 10 de 2005/06, recaudación 12.891.267,50 €):* la previsión dio 10,1 / 372 / 5.528 / 46.204 / 247.435
  acertantes y el escrutinio real fue 6 / 316 / 5.481 / 46.792 / 245.808.
- El botón de la esquina de «Previsión de premios» copia el resultado al portapapeles.

### Simulador de Escrutinios (Banco de Pruebas)
Tres aplicaciones: (1) **comparar dos combinaciones** para elegir una; (2) **comparar las columnas de una combinación** para seleccionar un grupo; (3) **testar una combinación contra sí misma** (autoescrutinio). Se organiza en **4 pasos** (pestañas); la primera vez se hacen en orden, luego se puede volver a cualquiera:

1. **Combinación a analizar** — abrir el fichero a analizar.
2. **Valoraciones reales y apostadas** — a la izquierda las **apostadas** (cómo crees que apuesta la gente); a la derecha las **reales/naturales** (probabilidad real de cada signo). Hay un botón que calcula las naturales a partir de las apostadas (se recomienda no tocar parámetros y "Calcular"; para afinar, variar el premio medio de 14, entre 150.000 y 200.000 €). El nº de "columnas a considerar" (por defecto 50.000) toma esas primeras columnas de los 14 triples ordenados para contar la aparición de cada signo.
3. **Generación de columnas de 14 aciertos** — los "14 virtuales" de la semana: leídos de un fichero (p. ej. los últimos 500 catorces reales) o **generados aleatoriamente** a partir de las valoraciones naturales (recomendado; p. ej. 100.000 columnas). *No es imprescindible* si en el paso 4 vas a hacer Autoescrutinio.
4. **Resultados de la simulación** — marcar los "Premios acumulados que se consideran", elegir el tipo de análisis y "Analizar":
   - **Combinación** — resultados globales (veces que se obtienen 0…14 aciertos, premios totales y medios). Ideal para comparar dos combinaciones del mismo nº de columnas.
   - **Columnas** — para cada apuesta, todos los aciertos/premios obtenidos contra los 14 virtuales. Revela qué apuestas cubren columnas más probables.
   - **Autoescrutinio** — considera cada apuesta como si fuera el 14 premiado y mide qué premios darían el resto de apuestas (marca automáticamente todos los premios).

   **Tabla de resultados** (análisis "Columnas" o "Autoescrutinio"): una fila por apuesta, con las columnas **Nº14, Nº13, Nº12, Nº11, Nº10** (veces que esa apuesta obtuvo cada premio en todos los escrutinios), **Premio 14 … Premio 10** (importe en euros), **Premio Ac** (acumulado de los premios marcados; en análisis "Columnas" no conviene contar el 14 por la aleatoriedad) y el **% de recuperación**. Se puede ordenar por cualquier cabecera, filtrar por rangos de cualquier concepto y grabar las apuestas seleccionadas (con `Shift`/`Ctrl`).

   **[Mac]** La tabla tiene las columnas **Nºcol.**, **Columna** (los signos), **Nº14…Nº10**, **Nºacumulado** (veces con
   las categorías marcadas), **Premio 14…10**, **Premio Acumulado**, **14 medio…10 medio** (premio ÷ veces) y
   **Recuperación**. Al pulsar una cabecera se ordena por ella: «Nºcol.» de menor a mayor y el resto de mayor a menor (otra
   pulsación invierte el orden). Las filas se seleccionan con `⌘` o `⇧`, y esa selección es la que usan **Grabar** y
   **Simplificar**. El botón **Filtrar…** abre el filtro de selección de filas (abajo). En Windows, la tabla todavía es
   una lista sin ordenar ni filtrar.

Utilidad aportada por **PacoHH**.
- **Paso 2, de apostadas a reales:** puedes fijar el **premio medio de 14** (el parámetro más intuitivo; por defecto
  152.755 €, la media de las últimas 550 semanas) o su equivalente, el **logaritmo de la probabilidad media del 14**
  (−14,75), y el **nº de columnas a considerar** (50.000). La utilidad ordena los 14 triples por cercanía a ese valor y
  cuenta los signos de las primeras 50.000: si en el partido 1 la `X` aparece en 15.000 de ellas, su valoración real es 30.
  Subir el premio aleja las reales de las apostadas.
- **Paso 3:** al generar las columnas aleatorias se muestran el **logaritmo de probabilidad medio** y la **desviación
  típica** obtenidos. Puede haber columnas repetidas, a propósito: las más probables se repiten más. Se pueden grabar con el
  disquete para reutilizarlas o escrutarlas con el 14 real.
- **Resultados globales** (análisis Combinación): nº de columnas, gasto total (columnas × catorces simulados × precio),
  premio total, veces premiada, veces con beneficio y % recuperado (premio total ÷ gasto). Para comparar otra combinación,
  vuelve al paso 1, cambia el fichero y analiza sin tocar nada más: se usan las mismas valoraciones y los mismos catorces.
- **Filtro de selección de filas** **[Mac]**: la ventana «Selección de columnas» tiene un mínimo y un máximo por concepto:
  veces con 14…10, veces con premio, premio total, acumulado y medio por categoría, y % de recuperación. Arranca con el
  mínimo y el máximo que hay en la tabla y sin nada marcado. Marcas el concepto, ajustas el rango y al Aceptar quedan
  seleccionadas las filas que cumplen todos los conceptos marcados; las demás se deseleccionan. Se indica cuántas quedan
  (p. ej. «28 filas seleccionadas»).
- **Grabar apuestas:** fila inicial, fila final, nº máximo de apuestas y la casilla «sólo si están seleccionadas»
  (marcada por defecto). Sin selección previa, desmarca la casilla e indica las filas a mano.

### Gestión de Equipos
Mantiene la base de **equipos** que se ofrecen al rellenar el boleto. Muestra los equipos repartidos en sus **cuatro categorías** (1ª, 2ª, 2ªB e Internacionales) y permite **moverlos** de una categoría a otra, **eliminarlos**, dar de **alta** nuevos (pantalla "Agregar equipo": nombre + categoría) y **guardar** los cambios con **«Guardar archivos»**.

**Importar equipos online.** En vez de teclearlos uno a uno, puedes traer de Internet los equipos que han aparecido en las jornadas recientes:

1. Elige el **país** en el selector **«País (online)»** (España o México).
2. Elige en **«Categoría»** (1ª / 2ª / 2ªB / Int) **dónde quieres que caigan** los equipos importados: el servicio devuelve una **única lista**, sin separar por división, así que eres tú quien decide la categoría destino.
3. Pulsa **«Importar equipos online»**.

La importación **solo añade**: los equipos que ya tenías se conservan tal cual, y los repetidos se descartan (no distingue mayúsculas de minúsculas). Un mensaje indica cuántos se han añadido, o que no había ninguno nuevo.

> **Importante:** la importación se queda **en pantalla**; para que los cambios se conserven hay que pulsar **«Guardar archivos»**. Si no hay conexión o el servicio falla, **no se modifica nada** y se muestra el motivo.

### Importar / Exportar Columnas
Convierte ficheros de columnas entre **CSV** y **TXT** (en ambos sentidos), para intercambiar columnas con otras herramientas u hojas de cálculo.

### Obtener Boletos Online
Descarga la **jornada de la semana** (España o México) para no teclear los equipos. Es una función **opcional**: el
programa funciona igual sin conexión. Hay tres formas de usarla:

- **[Mac] Al abrir el programa.** Se descarga sola la jornada actual del país que usaste la última vez (España si nunca
  descargaste nada) y los 14 partidos de Inicio se rellenan con los equipos local y visitante. Espera **3 segundos como
  máximo**: si no hay red o el servicio tarda, el boleto se queda **en blanco y sin ningún aviso**, y puedes escribir los
  equipos o pulsar **Jornada ▾** más tarde. Si mientras tanto ya escribiste o abriste algo, no se toca.
- **[Mac] Inicio → Jornada ▾ → España / México.** Rellena los equipos en cualquier momento. Un aviso indica la jornada
  cargada. Si el boleto tiene 15 partidos, el 15.º se rellena con el **Pleno al 15** (España).
- **`Archivo → Obtener Boletos Online`.** Eliges el **país** (el programa propone el último descargado) y pulsas
  **«Actualizar jornada»**. Si la descarga va bien aparece un resumen del tipo *«Jornada 38 · 14 partidos cargados ·
  Última actualización: 21/06/2026 10:30»*. En Mac los equipos pasan además al boleto de Inicio. En las dos versiones
  quedan disponibles para el resto de pantallas (por ejemplo **Grupos de Equipos**).

**Qué pasa sin conexión.** El programa guarda en tu equipo la última jornada descargada con éxito de cada país:

- Si pulsas «Actualizar jornada» o **Jornada ▾** y **no hay conexión**, se usa la jornada guardada y verás *«Sin
  conexión: mostrando la jornada guardada (actualizada dd/mm/aaaa hh:mm)»*, para que sepas de cuándo son los datos.
- **[Windows]** Al abrir el programa, la jornada guardada se carga sin conectarse y queda disponible para pantallas como
  Grupos de Equipos.
- Si **nunca** se ha descargado nada para ese país, el mensaje explica el fallo y el boleto **sigue en modo manual**
  (puedes escribir los equipos tú mismo).

**Si el servidor responde con un error**, el programa muestra el mensaje que envía el servicio: por ejemplo *«No hay jornada publicada para ES»* cuando todavía no se ha publicado la jornada, o un aviso de *demasiadas solicitudes* si se ha pulsado el botón muchas veces seguidas (en ese caso, espera un minuto). En ningún caso se pierde lo que ya tenías: el boleto conserva los últimos equipos cargados.

### Máscaras **[Mac]**
Una **máscara** guarda los **signos de cada partido** del boleto (por ejemplo `1X` en el 1, `1` en el 2, `1X2` en el
3…), con un nombre y **sin los equipos**. Sirve para reutilizar tus planteamientos habituales cada jornada sin volver a
marcar los signos.

- **Guardar:** marca los signos en Inicio y elige **Máscaras ▾ → Guardar boleto como máscara…**. Escribe el nombre; si
  ya existe una con ese nombre, el programa pregunta antes de reemplazarla.
- **Aplicar:** **Máscaras ▾** muestra tus máscaras con sus columnas (p. ej. *«Mi 4D-2T · 144 col.»*). Al elegir una, sus
  signos pasan al boleto y **los equipos de la jornada cargada se conservan**.
- **Pantalla «Máscaras»** (panel lateral, grupo Combinación; o **Máscaras ▾ → Máscaras y escrutinio histórico…**): lista
  cada máscara con sus signos, sus fijos/dobles/triples y sus columnas, y permite **aplicarla** o **borrarla**.

**Escrutinio histórico.** El programa guarda su propia **base de ganadoras**, separada por **liga**: cada liga es una lista
de jornadas pasadas con su columna ganadora. Las importaciones se van **sumando**: si hoy añades 1.500 jornadas y otro
día 10 más, no hace falta volver a subir las 1.500.

- **Elegir la liga:** el selector **«Liga»** muestra cada liga con sus jornadas guardadas (p. ej. *«España · 19
  jornadas»*). **«Escrutar las máscaras»** compara tus máscaras con las ganadoras de esa liga.
- **Descargar de clubprogol ▾ → España** o **México:** baja las jornadas publicadas en clubprogol.com y las suma a la liga
  «España» (Quiniela) o «México» (Progol). Si las descargas otra vez, solo entran las nuevas.
- En una instalación nueva, la liga **«España»** ya trae **2.659 jornadas de 1970/71 a 2026/27**: las 1.454 de
  `Jornadas/Resultados.txt` (1970/71 a 2006/07, con sus 14 signos completos), con la jornada escrita como `1970/1971-01`,
  y 1.205 de 2007/08 a 2026/27 sacadas de quinielafutbol.info, con la fecha como jornada (`20261011`, igual que
  clubprogol) o, si no se conoce, `2019/2020-01`. Así una descarga de clubprogol no repite las que ya están. La liga
  **«México»** trae los **1.579 concursos del Progol del 767 al 2354** (de 1993 a 2026, Lotería Nacional), con el nº de
  concurso como jornada. Las descargas de clubprogol se suman a ellas sin duplicar (en España, la misma fecha; en
  México, el mismo nº de concurso). Si borras una liga, no vuelve a aparecer.
- **Nueva liga…:** crea una liga vacía con el nombre que quieras (por ejemplo «Liga MX 2010-2020») para cargar en ella tu
  propio histórico.
- **Añadir ganadoras desde archivo…:** suma a la liga elegida un `.txt` con **una jornada por línea** (formato debajo).
- **Borrar liga…:** borra la liga y todas sus jornadas, tras confirmarlo.

El archivo de ganadoras lleva en cada línea el número de jornada, un punto y coma y la columna ganadora:

```
2355;1X21X1121X1X21
2356;112X1211X1X221
```

Una jornada que **ya está** no se duplica; si trae **otra columna**, se actualiza. También vale una línea con solo la
columna, pero entonces no se puede comprobar si ya estaba y se añade siempre. Las líneas en blanco se ignoran, da igual
`x` o `X`, y lo que haya detrás de los 14 signos (el pleno al 15, por ejemplo) no cuenta. Si una línea está mal, el
programa dice **qué línea** es y no añade nada. Al terminar indica cuántas jornadas se añadieron, cuántas ya estaban y
cuántas se actualizaron.

Cada máscara se compara con cada ganadora. La tabla muestra, por máscara:

| Columna | Significado |
|---------|-------------|
| **Columnas** | Columnas que desarrolla la máscara (lo que costaría jugarla entera). |
| **14, 13, 12, 11, 10** | Columnas que habrían tenido ese número de aciertos, **sumando todas las jornadas**. |
| **Jornadas con premio** | En cuántas jornadas de la liga tuvo al menos una columna de 10 o más aciertos. |

Pulsa una cabecera para **ordenar** (por ejemplo por la columna «10» para ver qué máscara da más premios de 10). El
cálculo es instantáneo aunque la máscara tenga miles de columnas: cuenta los aciertos sin desarrollarlas, con el mismo
resultado que escrutarlas una a una. Para descargar las ganadoras se necesita conexión; sin ella aparece el motivo y no se
hace nada. Encima de la tabla se indica cuántas jornadas se escrutaron y de qué liga.

> Una máscara fija signos **por posición** (partido 1, 2…), pero cada jornada trae partidos distintos: su historial
> refleja sobre todo cómo se reparten los signos en general (en las jornadas de octubre de 2026 el `1` salió el 45 % de
> las veces en España y el 54 % en México), no qué partidos acertarás. Úsalo para comparar planteamientos, no como garantía.

Las máscaras se guardan como archivos `.mas` (ver §11), con el mismo formato en Mac y en Windows.

### Esperanza matemática **[Mac]**
Calcula cuánto **devuelve de media** una columna de 14 signos por cada apuesta: su **esperanza matemática (EM)**, categoría
por categoría (14, 13, 12, 11 y 10 aciertos). Está en el panel lateral, grupo Utilidades, y en el menú de Utilidades.

Datos que pide:

- **Columna:** los 14 signos (`1`, `X` o `2`).
- **Valoraciones:** los porcentajes de cada partido en dos pestañas. **Apostados** es lo que juega la gente (sirve para
  estimar cuántos acertantes habría); **Reales** es tu estimación de lo que va a pasar (sirve para la probabilidad de cada
  resultado). Si una fila no suma 100, se toma sobre su suma. Con **«Pegar desde Excel»** se rellenan de una vez (ver la
  nota de Utilidades).
- **Jornada:** la **recaudación** y el **precio de la apuesta**. Las dos salen de **Configuración ▸ Parámetros LAE**, donde
  se guardan, y aquí se pueden cambiar para un cálculo. El número de apuestas es recaudación ÷ precio.
- **Reparto del fondo:** el % de la recaudación para cada categoría. El selector trae dos repartos (el del Excel
  «EM Categorias»: 16 · 7,5 · 7,5 · 7,5 · 9, y el de Estimación de premios: 12 · 8 · 8 · 8 · 9) y se puede editar a mano.
- **Cobro:** la **retención** (por defecto, el 20 % de lo que pase de 40.000 €), los **mínimos** por debajo de los que no se
  cobra (1 € de 14 a 11 aciertos y 1,5 € con 10) y **«Contarme como un acertante más (+1)»**: si aciertas, eres un
  acertante más de los estimados (es lo más realista y viene activado).

El cálculo es **exacto**: recorre los 19.321 resultados que darían premio a la columna (el propio, los 28 que cambian un
signo, los 364 que cambian dos, los 2.912 que cambian tres y los 16.016 que cambian cuatro). En cada uno estima los
acertantes con los porcentajes apostados, calcula el premio neto y lo pondera con su probabilidad real.

La tabla de resultado muestra, por categoría:

| Columna | Significado |
|---------|-------------|
| **Probabilidad real** | Probabilidad de que la columna saque exactamente esos aciertos (y «1 entre N»). |
| **Apostado con esos aciertos** | Parte de las apuestas que tendría esos aciertos si tu columna fuera la ganadora. |
| **Premio neto medio** | Lo que cobrarías de media al sacar esa categoría, tras la retención y los mínimos. |
| **EM** | Lo que aporta esa categoría a lo que devuelve la columna por apuesta. |

Debajo, la **EM total** y qué **% de lo apostado** supone: por encima del 100 %, la columna devuelve de media más de lo que
cuesta según tus porcentajes reales.

> **El precio cambia los premios, no las columnas.** Con la misma recaudación, un precio mayor significa menos apuestas,
> menos acertantes y premios más altos. Con el ejemplo del Excel «EM Categorias» (columna `1121121221112X`, recaudación
> 3.153.494,25 €), la EM es de 1,07 € por apuesta a 1 € (el 107 % de lo apostado) y de 0,89 € por apuesta a 0,75 € (el
> 119 %). El precio no afecta a las condiciones, al cálculo de columnas, a las reducciones ni al escrutinio.

### Compresor *.z3q
Comprime un archivo de columnas (`.txt`) a un **formato propio comprimido `.z3q`** (con un nivel de compresión 0–9) y lo descomprime de vuelta a `.txt`. Útil para almacenar o intercambiar ficheros de columnas grandes ocupando menos espacio.

### EstuCol
**Generador / analizador de columnas probables.** Selecciona un archivo de columnas **reducidas** y otro de **ganadoras**, elige el modo de agrupación/emparejamiento de columnas y genera un informe de escrutinio (que se abre en el visor de análisis de columnas).

### Análisis de grupos
Define un **grupo o patrón** sobre los 14 partidos, carga un fichero de columnas y cuenta **cuántas columnas encajan** en cada combinación del grupo; permite grabar la selección a un fichero.

### Reducciones perfectas
Genera una **"reducción perfecta"** a partir de una **columna base** (un signo por partido) y un **pronóstico** (qué signos juega cada partido): produce reducciones de tipo 4/13/11 triples ó 7/15 dobles y las graba en un archivo de columnas. (Método descrito por "Fortuna" en el foro de la comunidad.)

### Dependencia lineal
Recalcula el signo de un **partido a tratar** como **combinación lineal** (módulo 3 ó 2) de los signos del resto de partidos, ponderados por unos coeficientes que defines, y reescribe el archivo de columnas con el resultado.

> **Nota sobre el estado de algunas utilidades (Windows).** En la versión WinUI 3 actual, varias de estas pantallas (entre ellas Multiplicador, Fraccionador, Transposición, Álgebra, Reductor, Compresor, Reducciones perfectas, Dependencia lineal y Generador CP) tienen la **interfaz completa** pero parte de su **lógica de cálculo/guardado todavía en proceso de migración**. Si una acción no produce el archivo esperado, es por este motivo (pendiente de detallar/completar), no por un error de uso. En la versión **Mac** estas pantallas tienen ya su lógica completa.

---

## 11. Formatos de archivo

| Extensión | Contenido |
|-----------|-----------|
| `.comb` / `.xml` | Combinación completa: equipos, pronósticos, grupos, condiciones y filtro asociado. |
| `.grupos` | Un grupo de condiciones exportado, para reutilizar. |
| `.txt` | Archivo de columnas: una columna de 14 signos por línea. Es el formato de intercambio (filtros, escrutinio, operaciones). |
| `parametros.free1x2` | Configuración del programa: puntos de CP, nº de partidos, separador, idioma, datos ONLAE (precio, recaudación). **[Mac]** Los del Progol van en `<Progol>`, junto a `<LAE>`. |
| `.mas` **[Mac]** | Una máscara (carpeta `Mascaras/`): texto con una línea por partido y sus signos separados por comas (`1`, `1,X`, `1,X,2`). El nombre del archivo es el de la máscara. |
| `Historico/<liga>.txt` **[Mac]** | La base de ganadoras de una liga: una línea `jornada;columna` por jornada. El nombre del archivo es el de la liga. Se gestiona desde la pantalla «Máscaras»; no hace falta editarlo a mano. |
| `.z3q` | Archivo de columnas comprimido con el Compresor. |
| `Jornadas/InfoJornadasLAE.txt` | Jornadas L.A.E. (columna, temporada, jornada, recaudación y premios de 14 a 10), divididos por el precio de la apuesta. Lo leen Estimación de premios y Tramificar. **[Mac]** Una instalación nueva trae las de **1988/89 a 2006/07**, ya pasadas al precio de Configuración. |
| `Jornadas/Resultados.txt` | Ganadoras por temporada y jornada (`1970⇥1⇥X11121212X11X1`), para el escrutinio histórico. **[Mac]** Trae las de **1970 a 2006**. |
| `Jornadas/Historico*.txt` | Porcentajes de jornadas pasadas (temporada, jornada y 42 porcentajes): `HistoricoQuinielista.txt`, `HistoricoQuinielista.com.txt`, `HistoricoConcurso.txt` y `HistoricoMarca.txt`. **[Mac]** Se incluyen los de la distribución original: Quinielista de 1994/95 a 2006/07, Quinielista.com de 1994/95 a 2004/05, Concurso de 2000/01 a 2006/07 y Marca de 2004/05 a 2006/07. |
| `Ganadoras/<temporada>.txt` | Una columna ganadora por línea. **[Mac]** Se incluyen las temporadas **1983-84 a 2008-09**; sirven, por ejemplo, para **Premiadas** o para añadirlas a una liga de Máscaras. |

Las combinaciones temporales se guardan como backups y se pueden limpiar desde `Archivo → Borrar Combinaciones Temporales`.

**[Mac]** Las «Columnas de Inicio» y las reducciones sin archivo de salida van a la carpeta `Temp/` (`ColumnasInicio.txt`
y `Reducido.txt`) y se reescriben cada vez: guarda lo que quieras conservar con «Guardar resultado…» o el botón de
grabar de cada pantalla.

**Dónde están tus datos.** En **Windows**, junto al programa (es portable: se copia la carpeta y listo). En **Mac**, en
`~/Library/Application Support/Free1X2/` (en el Finder: menú Ir → Ir a la carpeta…, y pega esa ruta).

---

## 12. Consejos y errores frecuentes

- **Centra el objetivo según tu presupuesto.** A menos columnas, más riesgo (vía condiciones o pronóstico). No pretendas el 14 con 20 columnas.
- **No te dejes llevar por lo fácil que reduce el programa.** Partir de demasiadas columnas destroza la quiniela.
- **No abuses de las condiciones.** Cada condición añadida es una opción de fallar. Juntar 4 condiciones del 95% equivale a una del **81%**.
- **Conoce lo que aplicas.** No uses condiciones o sistemas ajenos sin entender su comportamiento.
- **No tomes condiciones de alto cumplimiento como dogma.** Una que se cumple el 95% también falla.
- **Comprueba el ahorro.** No apliques una condición que solo elimina 5 columnas (tentación de fallo). Ojo también con cegarte por la rentabilidad.
- **Usa columnas probables**, casi indispensables hoy: al menos una con un pronóstico arriesgado permitiendo algún fallo.
- **Sé coherente y ten paciencia.** Un sistema que debería entrar el 10% de las veces puede pasar 15 jornadas sin acertar y luego entrar dos seguidas.
- **El programa no da premios.** Solo aplica lo que tú le indicas. Y al final, siempre hay un componente de **suerte**.
- **El historial no es una garantía.** Que una máscara o una condición haya funcionado en las jornadas pasadas no asegura
  que funcione la próxima; compara planteamientos, pero sin perder de vista lo anterior.
- **Mismo efecto, distinto camino.** Con 4 partidos a triple, jugar 2 de ellos como fijos (9 columnas) cuesta lo mismo que
  una CP de 4 fijos que admita 1 fallo (también 9). Elige qué riesgo prefieres.
- **Mide cada recorte.** Pasar un triple a doble ahorra un 33 % y un doble a fijo un 50 %; quitar 5 variantes, apenas un
  5–10 %. Con 14 triples, prohibir exactamente 10 variantes ahorra ≈21 % de columnas, pero eso pasa ≈7 % de las jornadas
  y entonces los premios son altos; prohibir 4 variantes solo ahorra ≈0,3 %.
- **20 columnas** son el 0,0004 % de las 4.782.969 posibles: con tan pocas no se juega al 14, se juega a la suerte.

## 13. Recetas de jugadores

Métodos que jugadores veteranos publicaron en el foro, traducidos a los pasos de Free1X2. Son ideas para probar y comparar,
no garantías (ver la sección anterior).

### PacoHH: los 14 triples por tramos y a distancia 6

**Idea.** Diversificar mucho el pronóstico para acertar la mayoría de las jornadas (regularidad en el acierto) en vez de
buscar el golpe. Se ordenan los 14 triples de más a menos apostado con los porcentajes de L.A.E. de la jornada, se parten
en **9 tramos** iguales y del **tramo 1**, las **531.441** columnas más apostadas (4.782.969 ÷ 9), se juegan columnas que se
diferencien entre sí en **6 signos o más**. Según PacoHH, a distancia 6 salen entre **75 y 150 columnas**, según los
porcentajes de cada jornada. En la lista que publicó de la temporada 2009/10, el 14 de las doce primeras jornadas quedó
entre las posiciones 2.383 y 526.026, dentro del tramo 1; el de la jornada 13, en la 2.084.273, fuera de él.

**Pasos.**
1. **Ordenar por probabilidad:** origen **14 Triples**; los porcentajes de L.A.E. de la jornada (en la versión Mac,
   **Pegar desde Excel**); pestaña **Por productos** con el valor central **Log. neperiano = 0**, que ordena de más a menos
   probable; **Nº máximo de columnas = 531441**; las casillas de añadir, sin marcar; un fichero de salida, p. ej.
   `tramo1.txt` → **Ordenar**.
2. **Diferencias entre columnas:** generación por **Fichero** (`tramo1.txt`); condicionada por **Columna Base** con
   cualquier columna (p. ej. `11111111111111`) y **s/ col. base = `0-14`**, para que la base no cuente;
   **s/ col. result. = `6-14`** → **Calcular** → **Grabar**. Las candidatas se recorren en el orden del fichero, así que
   entran primero las más apostadas.
3. Si quieres un número concreto de apuestas, pon el **Límite de columnas**: se graban las primeras admitidas, que son las
   más apostadas. PacoHH usaba el tramificador de otro programa, que permite pedir ese número directamente.

> Para ver en qué tramo cayó el 14 de jornadas pasadas, usa **Tramificar** con **Nº de tramos = 9** y la columna
> premiada: la tabla dice en qué tramo cae cada premio. **Grabar tramos** no graba nada, como en el programa original;
> por eso el tramo 1 se saca con Ordenar por probabilidad.

### PacoHH: elegir columnas con el Banco de pruebas **[Mac]**

**Idea.** Simular muchas jornadas y quedarse con las columnas que, a la vez, aciertan a menudo y cobran bien. Los consejos
son de PacoHH, el autor del Banco de pruebas. Hace falta ordenar la tabla del Banco, que de momento solo se puede en la
versión Mac.

**Pasos.**
1. **Paso 1:** abre la combinación que quieres estudiar.
2. **Paso 2:** a la izquierda, los porcentajes **apostados** (PacoHH recomienda los de L.A.E. del jueves, que se publican el
   viernes); a la derecha, los **reales** (por ejemplo, los de Betfair del mismo viernes).
3. **Paso 3:** genera las columnas aleatorias. Con unas **100.000** basta para estudiar el 13, 12, 11 y 10; para que el 14
   cuente haría falta del orden de 100.000.000.
4. **Las más probables (paso 4):** análisis **Columnas** con todas las categorías marcadas en «Premios acumulados que se
   consideran» → **Analizar** → ordena por **Nºacumulado** (las veces con premio).
5. **Las más rentables:** desmarca el 14 (salvo que simules millones de jornadas), **Analizar** otra vez y ordena por
   **Premio Acumulado**. Las columnas aleatorias del paso 3 son las mismas.
6. PacoHH se queda con el **66 % más probable** y con el **66 % más rentable**, y juega las columnas que están en los dos
   grupos (entre el 33 % y el 67 % del total). En Free1X2: tras el paso 4, selecciona con `⇧` las dos primeras terceras
   partes de la tabla y **Grabar**; tras el paso 5, lo mismo en otro fichero; luego **Álgebra de columnas** con «Suma
   Combinaciones: selecciona columnas comunes».
7. Con el **Filtro de selección de filas** puedes hacerlo por importes: por ejemplo, un % de recuperación mínimo.

**Cómo leer el resultado** (los totales del paso 4):
- **Veces beneficio** es lo más importante: intenta que pase del **10 %** de las jornadas simuladas.
- **% recuperado** también cuenta, con cuidado: el 14 influye mucho y es poco fiable.
- Una combinación rentable, para PacoHH, recupera más o menos esto en cada categoría:

  | Categoría | Recuperación | Reparto de entonces |
  |-----------|--------------|---------------------|
  | 10 | 12 % | 9 % |
  | 11 | 13 % | 8 % |
  | 12 | 16 % | 8 % |
  | 13 | 21 % | 8 % |
  | 14 | 45 % | 12 % |

  Suman un **107 %**, sin contar el Pleno al 15. Según PacoHH, si recuperas un 12 % con el 10, el resto llega solo.
- Otros consejos suyos: pide en cada categoría una rentabilidad mayor que su parte del reparto, sobre todo en el 10 y el
  11. Para maximizar los arrastres, ordena por las veces con premio.

**¿Dudas?** Pregunta en el **foro de Free1X2**: [clubprogol.com/foro](https://clubprogol.com/foro/).

---

*Manual reconstruido para la versión actual de Free1X2. Para el detalle de **flujos funcionales y de datos** (cómo viaja una columna por el motor), ver [`MANUAL_FLUJOS.md`](MANUAL_FLUJOS.md). Para la arquitectura técnica, ver [`../README.md`](../README.md) y [`ANALISIS_TECNICO_WINUI3.md`](ANALISIS_TECNICO_WINUI3.md).*
