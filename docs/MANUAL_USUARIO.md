# Manual de usuario — Free1X2

Manual de uso del programa **Free1X2** para el análisis y la generación de combinaciones de la Quiniela. Reconstruido a partir del manual oficial del programa y de [quinielaticas.com](https://quinielaticas.com/programa-free1x2/).

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

---

## 1. Qué es Free1X2

Free1X2 es un programa libre (GPL) para Windows que ayuda a confeccionar la **Quiniela** (14 partidos + Pleno al 15). No "adivina" resultados: aplica **las condiciones matemáticas que tú defines** sobre el universo de columnas posibles, generando solo aquellas que cumplen tus criterios, y ofrece herramientas para **reducir** el número de columnas, **escrutar** resultados y **analizar** combinaciones.

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

**Columna.** Una apuesta concreta: una cadena de 14 signos, p. ej. `11X211X1X12111`.

**Coste.** Cada columna tiene un precio (configurable; 0,75 € en la actualidad). Reducir columnas = reducir coste.

**Otras definiciones útiles** (se usan en las condiciones). Para fijar ideas, se ilustran todas sobre la misma columna de ejemplo **`11X211X1X12111`**:

| Concepto | Definición | En `11X211X1X12111` |
|----------|-----------|----------------------|
| **Interrupción** | Cada cambio de signo a lo largo de la columna. | **9** interrupciones: `11`@`X`@`2`@`11`@`X`@`1`@`X`@`1`@`2`@`111`. |
| **Interrupción de signo** | Cada vez que tras un signo concreto aparece otro distinto. | **4** del signo `1`. |
| **Interrupciones seguidas (global)** | Cuando tras una interrupción el siguiente signo también interrumpe; se limita la racha máxima. | **6** seguidas. |
| **Signos seguidos** | Máximo de signos iguales consecutivos. | 2 unos, 1 equis (la racha máx. de `X` es 1), etc. |
| **Distancia** | Mayor separación entre dos apariciones del mismo signo. | `1`→3, `X`→4, `2`→7, `V`→3. |
| **Dibujo** | La representación `X+Y` (nº de equis + nº de doses). | 3 equis + 2 doses = dibujo `3+2`. |
| **Figura de signos** | Forma en que se agrupan consecutivamente las apariciones de un signo, ordenadas de mayor a menor grupo. | Figura de `1` = `32211`; de `X` = `111`; de `2` = `11`; de `V` = `2111`. |

**Figuras de zonas.** Además de la figura global, se puede dividir el boleto en zonas y mirar cómo se reparten los signos en cada una. Las zonas habituales son **7+7** (los 7 primeros partidos frente a los 7 últimos) y **Par-Impar** (partidos en posición par frente a impar). En el ejemplo, dividiendo en 7+7 las mitades son `11X211X` y `1X12111`, con figura de variantes `3+2`; dividiendo en par/impar quedan `1211111` y `1X1XX21`. También puede condicionarse el número de *coincidencias* entre zonas para el mismo signo (suele darse 0, 1 ó 3, rara vez 2).

**Suma de aciertos / Recorrido.** Cuando se trabaja con varias columnas probables (ver §7), la **suma de aciertos** es el total acumulado de aciertos de un conjunto de CPs, y el **recorrido** es la diferencia entre la CP con más aciertos y la que menos. Sobre rangos de aciertos `4,6,5,8,7,7` el recorrido es `8−4 = 4`.

---

## 3. La pantalla principal

La ventana principal se divide en zonas:

### Zona superior — Menú y barras de herramientas
Menús y accesos directos a todas las utilidades del programa (ver [Referencia de menús](#5-referencia-de-menús)).

### Campo Pronósticos (el boleto base)
Contiene los 14 partidos. Para cargarlo:
- **Desde archivo:** `Archivo → Abrir Partidos Boleto`.
- **Manualmente:** junto a cada `?` eliges el equipo (1ª, 2ª, 2ªB o selección internacional) y marcas el/los signo(s) `1`/`X`/`2` de cada partido.

Puedes guardarlo con `Archivo → Guardar Partidos Boleto`.

Debajo están los **controles de grupos** (crear/navegar grupos de condiciones — ver más adelante).

### Campo Condiciones
Una rejilla de botones, uno por condición. Cada botón tiene **dos indicadores (semáforo)** que muestran su estado:
- **Ambos en blanco** — la condición no tiene datos.
- **Indicador inferior activo** — la condición tiene datos **y se aplicará** al calcular.
- **Indicador superior activo** — la condición tiene datos **pero está desactivada** (no se tendrá en cuenta).

Puedes activar/desactivar una condición pulsando sus indicadores, sin perder los datos introducidos.

Bajo la rejilla están **Tolerancias** y **Condiciones Relacionadas**.

### Campo Filtro
Permite seleccionar un archivo de columnas que actúa como **base/filtro** de la combinación (ver [Filtros](#8-filtros)). Muestra indicadores de estado (cargado / activo) y el nombre del filtro seleccionado.

---

## 4. Flujo de trabajo paso a paso

1. **Configura** el programa (`Free1x2 → Configuración`): precio de la apuesta, nº de partidos, etc.
2. **Carga el boleto base** (equipos + pronóstico inicial de fijos/dobles/triples).
3. **Aplica condiciones** en el campo Condiciones (variantes, columnas probables, etc.). Cada condición acota el universo de columnas.
4. *(Opcional)* **Selecciona un filtro** si quieres partir de un archivo de columnas concreto.
5. **Calcula** (`Combinación → Calcular`). El programa genera todas las columnas que cumplen y te dice **cuántas** son y su **coste**.
6. Si son demasiadas, **reduce**: cambia el pronóstico, aprieta condiciones o usa el **reductor** (`Combinación → Reducir`).
7. **Revisa**: análisis de fallos, gráfico, probabilidades, estadísticas.
8. **Guarda** la combinación (`.comb`) y/o **exporta** las columnas (`.txt`).
9. **Ver/Imprimir boletos** (`Combinación → Ver Boletos` / `Imprimir Boletos`).
10. Tras la jornada, **escruta** (`Combinación → Escrutinios`) para ver los aciertos y premios.

---

## 5. Referencia de menús

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
| Obtener Boletos Online | Descarga el boleto oficial de una **jornada y temporada** concretas desde el servicio online del programa, para cargarlo como boleto base. |
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
| **Barras de Herramientas** | Submenú con un interruptor por cada una de las **6 barras** (Filtros, Free1X2, Operaciones, Utilidades, Combinación, Archivo). Permite mostrar/ocultar cada barra; la elección se conserva al cerrar el programa. |
| Listado de Condiciones | Muestra en **árbol** todas las condiciones/filtros configurados en la combinación actual; se puede expandir/colapsar y exportar a texto/HTML. |

### Combinación
| Opción | Función |
|--------|---------|
| Calcular | Genera la combinación. |
| Calcular Varias Combinaciones | Cálculo por lotes. |
| Ver / Imprimir Boletos | Muestra / imprime en boletos oficiales. |
| Ver Boletos en Editor de Texto | Vuelca las columnas de un fichero a un editor de texto para revisarlas/copiarlas. |
| Reducir | Abre el reductor. |
| Escrutinios | Escruta archivos de columnas. |
| Escrutar Combinaciones | Escrutinio orientado a combinaciones (no solo a ficheros de columnas sueltas). |
| Analizar Combinación | Analiza una combinación concreta (su columna, su analizador y sus pronósticos asociados). |
| Análisis de Fallos / Gráfico / de Signos | Herramientas de análisis. |
| Probabilidades | Probabilidad de una "hija" respecto a la "madre". |
| Estadísticas | Distribución de un archivo por condiciones. |
| Añadir Pleno al 15 | Incorpora el signo del 15. |

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
| Columnas GEPT | Generador de columnas probables. |
| Tramificar | Separa columnas por tramos. |
| Premiadas | Utilidad de premiadas. |
| Estimación de Premios | Predice la cuantía de premios dada la ganadora. |
| Banco de Pruebas | Simulador de escrutinios (ver §10). |
| Compresor *.z3q | Comprime/descomprime archivos de columnas. |
| EstuCol | Generador/analizador de columnas probables a partir de reducidas y ganadoras. |

> **Otras herramientas accesibles desde la barra de herramientas** (grupo Utilidades), aunque no figuran en este menú: **Análisis de grupos**, **Reducciones perfectas** y **Dependencia lineal** (ver §10).

---

## 6. Las condiciones

Cada condición se abre con su botón en el campo Condiciones. Cada pantalla de condición permite introducir un **valor concreto o un rango** (mínimo–máximo) por cada signo. Tras introducir datos, se **activa** con el botón de aceptar; el botón de salir descarta sin guardar. La forma de rellenar es muy parecida en casi todas las condiciones. Las principales:

### Variantes, X y 2 (botón "Variantes, X y 2")
Limita el número de **variantes** (X + 2), y opcionalmente de **equis** y **doses**, mediante valores o rangos.

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

### Interrupciones (botón "Interrupciones")
Condiciona por **cambios de signo**: interrupciones totales, por signo concreto, y seguidas (globales o por signo). Marcas el número o rango esperado; la condición acierta si las interrupciones de la columna ganadora caen dentro de lo indicado. (Recuerda: en `11X211X1X12111` hay 9 interrupciones totales, 4 del signo `1`, 6 seguidas globales y 3 seguidas de `1`.)

### Grupos de Equipos (botón "Grupos Equipos")
Condición "futbolística", organizada en **dos pestañas**:

- **Pestaña principal (Grupos Equipos).** Si has cargado el boleto, aparecen los nombres de los 28 equipos. Marcas un conjunto de equipos y exiges un nº o rango de **Victorias, Empates, Derrotas y Suma de puntos** (sistema español **3-1-0**). El color de los equipos solo indica su posición en el boleto, no aporta significado. *Ejemplo:* marcas 5 equipos y pides "al menos 3 victorias, 0 ó 1 empate, como máximo 2 derrotas, suma de puntos entre 10 y 15". Con los botones de navegación de grupos puedes crear un **segundo grupo** de equipos (p. ej. 7 equipos) con sus propios límites; el panel indica "Grupo 2 de 2".
- **Pestaña Relaciones.** Permite relacionar varios grupos entre sí. En el cuadro "Grupos Equipos" indicas qué grupos entran en la relación (p. ej. `1-2`) y defines la suma de victorias/empates/derrotas/puntos **conjunta** de esos grupos. Tiene su propio panel de navegación para crear tantas relaciones como quieras. Botones para eliminar el grupo de equipos en pantalla o borrar relaciones.

### Distancias (botón "Distancias")
Limita la **distancia** (mayor separación entre dos apariciones del mismo signo) para `1`, `X`, `2` y variantes, con valores o rangos.

### Valoración de signos
Asigna un **valor** (normalmente en %) a cada signo de cada partido y exige que la **suma** de los valores de los signos elegidos en la columna esté en un intervalo. Sirve para sesgar la combinación hacia los signos que crees más probables, en vez del reparto matemático al 50%/33%. *Ejemplo:* dos partidos a triple valorados `50,30,20`; con intervalo de suma 60–100 quedan excluidas las combinaciones `X2`, `2X` y `22` (suman 50, 50 y 40), pero sí entra la sorpresa `2` si el otro partido aporta suficiente valor.

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

---

## 9. Reducción de columnas

Cuando el cálculo da más columnas de las que quieres jugar, hay **tres formas** de reducir:

1. **Cambiar el pronóstico** (lo más efectivo). Reducir triples y dobles:
   - Triple → doble: divide las columnas por **1,5**.
   - Doble → fijo: divide por **2**.
   - Triple → fijo: divide por **3**.
   Arriesga más en el pronóstico, pero es el mejor método.
2. **Añadir/apretar condiciones.** Terreno delicado: cada condición extra es una opción de fallo. Mejor apretar los rangos de tus condiciones que añadir otras nuevas que no tengas estudiadas.
3. **Jugar una reducción** (`Combinación → Reducir`). Algoritmos que, partiendo de un conjunto mayor, garantizan **como mínimo un premio de la categoría elegida** con muchas menos columnas (p. ej., reducir al 12 garantiza al menos un 12 si la madre tenía el 14). Algunas reducciones garantizan un **porcentaje** del premio a cambio de menos columnas. Free1X2 incluye varios algoritmos (JDC, JLPM, XFSF, TM…).

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

### SubeCategoría
Coge cada columna de un fichero `.txt` y añade todas las que se le diferencian en un solo signo, eliminando repetidas. Opera sobre los 14 partidos por defecto, pero puedes restringir los **partidos involucrados** (solo esos cambiarán de signo). En **Niveles** indicas cuántas categorías subir: el archivo final contiene las columnas de los niveles marcados (marca el **Nivel 0** para incluir también las columnas originales). Entre "Calcular" y "Grabar" hay una casilla informativa que muestra el nº de columnas de origen (`I=`), resultantes (`F=`) y grabadas (`G=`).

### Modificador % (modificador de columnas)
Cambia la **frecuencia de signos** de uno o más partidos de un archivo. *Ej.:* un partido apostado `30,20,50` que crees que infravalora la `X` lo pasas a `25,30,45`. Procedimiento: abrir el `.txt`, seleccionar partido(s), poner los nuevos porcentajes (si no suman 100 se ajustan proporcionalmente), elegir el **tipo de inserción** y aceptar:
- **Aleatorio** — reparte al azar hasta el tope de cada frecuencia.
- **Proporcional** — rellena en grupos proporcionales (10 unos, 20 equis, 50 doses → grupos de 1/2/5).
- **Ordenado** — todos los 1 seguidos, luego las X, luego los 2.

Con "Ordenar signos" (no válido para aleatorio) coloca primero el signo más probable. Es un editor automático; después conviene pasar el resultado por `Operaciones → Álgebra → eliminar columnas repetidas`, porque el modificador no comprueba duplicados.

### Ordenar por Probabilidad
Ordena un fichero (o los 14 triples) por proximidad a un **valor central de ordenación**: las columnas más próximas quedan al principio del fichero de salida. El valor central se puede expresar de 4 formas equivalentes —**nº de acertantes**, **importe del premio de 14**, **probabilidad** (producto de los tantos por uno de cada signo) o **logaritmo neperiano** de la probabilidad (para usar las estadísticas de PacoHH)— y al fijar una, las otras tres se recalculan. Pasos: elegir 14 triples o fichero → importar/escribir valoraciones → indicar fichero de salida → definir el valor central → "Ordenar". Con "añadir valor de ordenación" el fichero de salida lleva ese valor junto a cada columna.

> **Nº de acertantes < 1.** Cuando el nº teórico de acertantes baja de 1, el premio supera el 15% de la recaudación; se mantiene como premio teórico para conservar la relación con el nº de acertantes, y aparece un aviso en la barra de estado. Por defecto la recaudación es de 15.000.000 € (30.000.000 de columnas apostadas).

### Rentabilidad
Una apuesta es **rentable** si `CosteApuesta < Premio × Probabilidad`. Ese producto es la **esperanza matemática (EM)**. La utilidad calcula, columna a columna, el premio de 14 (a partir de los % apostados) y la probabilidad (a partir de las valoraciones probables), y de ahí la EM. Como solo considera el premio de 14, el umbral de rentabilidad no es el coste íntegro: se recomienda una **EM mínima de 0,136 €** (el 27,27% del coste de 0,5 €, precio vigente cuando se escribió el manual oficial; con el precio actual de 0,75 € recalcula este umbral proporcionalmente). Procedimiento: introducir % apostados y % de probabilidad de los 14 partidos → elegir 14 triples o fichero → fichero de salida → fijar EM mínima (0,136) y máxima → "Calcular". Puedes ordenar de mayor a menor EM y analizar una única columna escribiendo sus 14 signos.

### Selector (JuanM)
Escoge apuestas de mayor a menor probabilidad en **tres grupos** (dos de 5 partidos = 243 columnas cada uno, y uno de 4 = 81 columnas) y los **multiplica**. Pasos: (1) introducir valoraciones; (2) asignar cada partido al grupo 1, 2 ó 3 (cinco al 1, cinco al 2, cuatro al 3); (3) poner el **límite** mín–máx de columnas de cada grupo. *Ej.:* grupo 1 → 60, grupo 2 → 40, grupo 3 → 27 da `60 × 40 × 27 = 64 800` columnas (las 60 más probables del grupo 1, las 40 del 2 y las 27 del 3).

### Selector (MarioSan)
Parte de una combinación final que, según tus estadísticas, suele encerrar el 14 con un nº de 13s en un rango estable. Califica cada columna con el **nº de 13s** que la acompañarían si fuera ganadora, repartiéndolas en **29 grupos** (de 0 a 28 treces). Pasos: "Iniciar" → elegir fichero → el programa muestra la distribución → marcar las filas deseadas (con `Ctrl` para varias) → "Grabar". *Nota:* la selección solo garantiza el 14, no los treces.

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

**Gráficas.** Desde el menú "Gráficas" se visualizan los resultados; hay 9 gráficas (probabilidades; posiciones de los 14/13/12/11/10; nº de columnas premiadas por tramo; importe de premios por tramo; beneficio por tramo). En el eje X van los tramos y se pueden superponer varias.

### Premiadas
Escruta un fichero de columnas **ganadoras** contra los 14 triples y cuenta cuántas veces cada columna de los 14 triples resultó premiada. En "Frecuencias" se lee, p. ej., `784 = 1 veces` → hay 784 columnas que salieron premiadas 1 vez. Con "Analiza selección" se ve en qué jornadas ocurrió (para estudiar regularidad) y con "Grabar selección" se vuelcan esas 784 columnas a un fichero para escrutarlas.

### Estimación de Premios (con Escrutinio Real)
Predice acertantes y cuantía de cada premio. Lo básico: introducir las **valoraciones apostadas** de la jornada y definir la **columna ganadora** → aparece la estimación.
- **Valoraciones:** a mano (si la suma de un partido no está entre 99 y 101 se marca en rojo) o desde fichero (varios formatos, incluido histórico). Con histórico, al cambiar de jornada se cargan valoración, ganadora y premios reales.
- **Columna ganadora:** clic sobre cada casilla cicla `1 → X → 2 → 1`; cada cambio recalcula los premios.
- **Resultados ("Previsión de premios"):** el nº de acertantes es entero salvo el de 14, que se muestra **decimal** porque cuando hay pocos acertantes interesa distinguir 1,1 de 1,8, o 0,3 de 0,9. Para acertantes **< 1**, el premio no supera el máximo que marcan la recaudación y el % asignado al 14.
- **Recaudación / bote / % a premios:** ajustables; el bote solo afecta al 15 (no contemplado en la utilidad).
- **Escrutinio Real:** muestra los datos reales de la jornada (leídos del fichero de información de jornadas del L.A.E.); se pueden introducir/actualizar a mano y guardar con el botón del disquete asociándolos a la jornada del selector. Hay un enlace directo a la página oficial de resultados del L.A.E.

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

### Gestión de Equipos
Mantiene la base de **equipos** que se ofrecen al rellenar el boleto. Muestra los equipos repartidos en sus **cuatro categorías** (1ª, 2ª, 2ªB e Internacionales) y permite **moverlos** de una categoría a otra, **eliminarlos**, dar de **alta** nuevos (pantalla "Agregar equipo": nombre + categoría) y **guardar** los cambios.

### Importar / Exportar Columnas
Convierte ficheros de columnas entre **CSV** y **TXT** (en ambos sentidos), para intercambiar columnas con otras herramientas u hojas de cálculo.

### Obtener Boletos Online
Descarga el **boleto oficial** de la jornada eligiendo **jornada** y **temporada**; el boleto descargado se puede usar como boleto base. *(Requiere conexión; si el servicio no responde, el programa avisa de que el boleto no está disponible.)*

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

> **Nota sobre el estado de algunas utilidades.** En la versión WinUI 3 actual, varias de estas pantallas (entre ellas Multiplicador, Fraccionador, Transposición, Álgebra, Reductor, Compresor, Reducciones perfectas, Dependencia lineal, Generador CP y la persistencia del Gestor de Equipos) tienen la **interfaz completa** pero parte de su **lógica de cálculo/guardado todavía en proceso de migración**. Si una acción no produce el archivo esperado, es por este motivo (pendiente de detallar/completar), no por un error de uso.

---

## 11. Formatos de archivo

| Extensión | Contenido |
|-----------|-----------|
| `.comb` / `.xml` | Combinación completa: equipos, pronósticos, grupos, condiciones y filtro asociado. |
| `.grupos` | Un grupo de condiciones exportado, para reutilizar. |
| `.txt` | Archivo de columnas: una columna de 14 signos por línea. Es el formato de intercambio (filtros, escrutinio, operaciones). |
| `parametros.free1x2` | Configuración del programa: puntos de CP, nº de partidos, separador, idioma, datos ONLAE (precio, recaudación). |

Las combinaciones temporales se guardan como backups y se pueden limpiar desde `Archivo → Borrar Combinaciones Temporales`.

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

---

*Manual reconstruido para la versión actual de Free1X2. Para el detalle de **flujos funcionales y de datos** (cómo viaja una columna por el motor), ver [`MANUAL_FLUJOS.md`](MANUAL_FLUJOS.md). Para la arquitectura técnica y el estado de la migración a WinUI 3, ver [`../README.md`](../README.md) y [`../PLAN_MIGRACION_WINUI3.md`](../PLAN_MIGRACION_WINUI3.md).*
