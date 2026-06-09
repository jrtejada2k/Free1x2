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

**Coste.** Cada columna tiene un precio (configurable, p. ej. 0,75 €). Reducir columnas = reducir coste.

**Otras definiciones útiles** (se usan en las condiciones):
- **Interrupción** — cada cambio de signo a lo largo de la columna. En `11X211X1X12111` hay 9.
- **Signos seguidos** — máximo de signos iguales consecutivos.
- **Distancia** — mayor separación entre dos signos iguales.
- **Dibujo** — la representación `X+Y` (X equis, Y doses) de una columna.

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
| Salir | Cierra el programa (pide confirmación). |
| Configuración | Parámetros del programa. |
| Acerca de | Información, licencia GPL, web, foros, manual. |

### Archivo
| Opción | Función |
|--------|---------|
| Abrir / Guardar Partidos Boleto | Carga/guarda el boleto base. |
| Nueva Combinación | Reinicia para una combinación nueva. |
| Abrir / Guardar / Guardar Como Combinación | Gestiona archivos `.comb`. |
| Borrar Combinaciones Temporales | Elimina los backups temporales. |

### Combinación
| Opción | Función |
|--------|---------|
| Calcular | Genera la combinación. |
| Calcular Varias Combinaciones | Cálculo por lotes. |
| Ver / Imprimir Boletos | Muestra / imprime en boletos oficiales. |
| Reducir | Abre el reductor. |
| Escrutinios | Escruta archivos de columnas. |
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
| Simulador de Escrutinios | Banco de pruebas. |

---

## 6. Las condiciones

Cada condición se abre con su botón en el campo Condiciones. Tras introducir datos, se **activa** con el botón de aceptar (o se descarta sin guardar). Las principales:

### Variantes, X y 2
Limita el número de **variantes** (X + 2), y opcionalmente de **equis** y **doses**, mediante valores o rangos. Ejemplo: 4–9 variantes, 2–6 equis, 2–6 doses. Una columna con 5 equis + 6 doses (11 variantes) se descartaría por exceder el máximo de variantes, aunque equis y doses estén en rango.

### Signos Seguidos
Limita el **máximo de signos iguales consecutivos** para `1`, `X`, `2` o variantes. Ejemplo: en `1XX11XXX22XX11` hay 3 equis seguidas. Marcar `0` para un signo fuerza que **no aparezca ningún** signo de ese tipo seguido.

### Dibujos
Forma exacta de condicionar variantes: marcas los **dibujos** `X+Y` concretos permitidos (nº de equis + nº de doses). Más preciso que "Variantes, X y 2" porque fija la combinación exacta. Botones *Marcar/Desmarcar Todos*.

### Interrupciones
Condiciona por **cambios de signo**: interrupciones totales, por signo, y seguidas (globales o por signo). Marcas el número o rango esperado.

### Grupos de Equipos
Condición "futbolística": marcas un conjunto de equipos del boleto y exiges un nº/rango de **victorias, empates, derrotas y suma de puntos** (sistema 3-1-0). Pestaña **Relaciones** para relacionar varios grupos entre sí. Soporta varios grupos con su panel de navegación.

### Distancias
Limita la **distancia** (mayor separación entre dos signos iguales) para `1`, `X`, `2` y variantes.

### Otras condiciones disponibles
Según la versión, el campo Condiciones incluye además: **Pesos Numéricos**, **Valoración de Signos**, **Contactos**, **Formatos de Signos** y **Formatos 123**, **Simetrías** y **Diferencias**. Todas siguen el mismo patrón: abrir → introducir valores/rangos → activar.

### Tolerancias y Condiciones Relacionadas
- **Tolerancias** — permite que un nº limitado de condiciones se cumpla en un valor "tolerado" fuera del rango estricto.
- **Condiciones Relacionadas (If-Then)** — reglas lógicas del tipo "si se cumple A, entonces exige B".

### Grupos
Las condiciones se organizan en **grupos**. Puedes crear varios grupos de condiciones e ir navegando entre ellos; sirven para definir y combinar subsistemas de condiciones dentro de una misma combinación.

---

## 7. Columnas Probables (CP)

La condición **más potente y versátil**. Una CP es un **segundo pronóstico** sobre un grupo de partidos en el que defines un **rango de aciertos** deseado (en vez de exigir acertarlo todo).

**Ejemplo.** Cuatro "grandes" juegan en casa. En vez de gastar 4 triples (81 apuestas) por si alguno pincha, creas una CP donde los 4 ganan y pides **3 ó 4 aciertos** (1 o ningún fallo): solo 9 apuestas. Si el Madrid pierde en casa pero los otros 3 ganan, lo tienes cubierto.

Puedes poner **decenas, cientos o miles de CPs**, cada una con sus condiciones, y **relacionarlas** entre sí. Conceptos de relación entre CPs:

| Relación | Qué hace |
|----------|----------|
| **Aciertos seguidos / fallos seguidos** | Limita el máximo de aciertos (o fallos) consecutivos en una CP. |
| **Tolerancias (fallos permitidos)** | Permite que solo *n* CPs salgan a un valor tolerado fuera del rango. |
| **Suma de aciertos** | Condiciona la **suma** de aciertos de varias CPs (p. ej. entre 7 y 11). |
| **Recorridos** | Limita la diferencia entre la CP con más aciertos y la de menos. |
| **Grupos de columnas** | Exige que un nº limitado de CPs tenga un rango de aciertos más cerrado. |
| **Coincidencias** | Agrupa CPs por nº de aciertos coincidentes. |
| **Relaciones lógicas** | "Si la CP A acierta hasta 3, la CP B debe acertar al menos 5", etc. |

> Las CPs permiten **dirigir** la combinación hacia los signos que crees más probables y jugar "por encima de tu presupuesto" controlando el riesgo.

El **Generador CP** (menú Utilidades) y **Columnas GEPT** ayudan a crear columnas probables.

---

## 8. Filtros

Un **filtro** es un archivo de columnas que usas como **base de partida**. Cuando hay un filtro activo, una columna solo se acepta si **además** de cumplir todas tus condiciones, **está contenida** en el archivo del filtro. Es decir, el resultado es siempre un **subconjunto** del filtro.

**Cómo usarlo:** en el campo Filtro, pulsa el icono de carpeta y elige el archivo de columnas. El icono pasa a "X" y el indicador derecho se enciende (filtro activo). Puedes:
- **Desactivarlo** (clic en el cuadro izquierdo): sigue cargado pero no se aplica.
- **Borrarlo** (icono "X"): lo descarga.

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

| Utilidad | Para qué sirve |
|----------|----------------|
| **SubeCategoría** | Toma cada columna de un `.txt` y añade todas las que se diferencian en un solo signo (eliminando repetidas). Sube la categoría de premios. |
| **Diferencias entre columnas** | Compara dos archivos de columnas. |
| **Ordenar por Probabilidad** | Ordena las columnas según una valoración de probabilidad. |
| **Rentabilidad** | Calcula la rentabilidad (cumplimiento vs. ahorro) de una combinación. |
| **Selector (JuanM / MarioSan)** | Escoge columnas según criterios (p. ej. nº de 13s). |
| **Tramificar** | Separa las columnas por tramos. |
| **Premiadas** | Analiza columnas premiadas. |
| **Estimación de Premios** | Predice la cuantía de los premios sabiendo la ganadora (fiabilidad según la valoración introducida). |
| **Simulador de Escrutinios (Banco de Pruebas)** | Prueba combinaciones contra históricos. |
| **Álgebra / Transposición / Multiplicador / Fraccionador / Rotación** | Operaciones sobre archivos de columnas. |

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

*Manual reconstruido para la versión actual de Free1X2. Para la arquitectura técnica y el estado de la migración a WinUI 3, ver [`../README.md`](../README.md) y [`../PLAN_MIGRACION_WINUI3.md`](../PLAN_MIGRACION_WINUI3.md).*
