# Manual de flujos y datos — Free1X2

Este documento complementa al [Manual de usuario](MANUAL_USUARIO.md). El manual de usuario describe **pantallas y menús**; este describe **los flujos funcionales y el flujo de datos** que hay detrás: qué entra, por dónde pasa, qué decide cada componente y qué sale. Todas las referencias apuntan a código real del repositorio.

> **Índice**
> 1. [Visión general del pipeline](#1-visión-general-del-pipeline)
> 2. [El modelo de datos central: la columna como `long`](#2-el-modelo-de-datos-central-la-columna-como-long)
> 3. [Flujo 1 — Cálculo de una combinación](#3-flujo-1--cálculo-de-una-combinación)
> 4. [Flujo 2 — Cálculo con archivo-filtro](#4-flujo-2--cálculo-con-archivo-filtro)
> 5. [Flujo 3 — Cadena de validación de condiciones](#5-flujo-3--cadena-de-validación-de-condiciones)
> 6. [Flujo 4 — Análisis de combinación](#6-flujo-4--análisis-de-combinación)
> 7. [Flujo 5 — Reducción](#7-flujo-5--reducción)
> 8. [Flujo 6 — Escrutinio](#8-flujo-6--escrutinio)
> 9. [Flujo 7 — Columnas Probables (CP)](#9-flujo-7--columnas-probables-cp)
> 10. [Flujo 8 — Persistencia y configuración](#10-flujo-8--persistencia-y-configuración)
> 11. [Acoplamiento con la UI y migración WinUI 3](#11-acoplamiento-con-la-ui-y-migración-winui-3)

---

## 1. Visión general del pipeline

Free1X2 es, en esencia, **un generador-validador de columnas**. Todo lo demás (filtros, CPs, grupos, tolerancias) son piezas enchufadas a un único pipeline:

```
            ┌──────────────────────────────────────────────────────────────┐
            │                        ENTRADAS                              │
            │  Boleto base (14 pronósticos)   Archivo-filtro (.txt)        │
            │  Condiciones por grupo          parametros.free1x2          │
            └──────────────┬───────────────────────────┬──────────────────┘
                           ▼                           ▼
                 GeneradorColumnas  ◄──────────  Analizador (orquestador)
                 (enumera candidatas)            (config, contadores, I/O)
                           │  long por columna
                           ▼
                 ControladorGrupos ── ControlConjuntos (fallos entre controles)
                           │
                 ControlGrupos ────── fallos permitidos entre grupos
                           │
                 Grupo (0..n) ─────── cadena de IFiltro + tolerancias + filtro parcial
                           │
                 ControladorIfThen ── reglas "si A entonces B"
                           ▼
            ┌──────────────────────────────────────────────────────────────┐
            │                        SALIDAS                               │
            │  Contadores (analizadas / aceptadas / coste)                 │
            │  Archivo de columnas .txt        ContenedorAnalisisGlobal    │
            └──────────────────────────────────────────────────────────────┘
```

Flujos posteriores consumen el `.txt` resultante: **Reducción** (menos columnas garantizando categoría), **Escrutinio** (aciertos contra la ganadora), **Operaciones** (álgebra, fraccionado, multiplicación) y **Boletos** (impresión).

---

## 2. El modelo de datos central: la columna como `long`

Todo el motor trabaja con columnas codificadas en un único `long` de 64 bits: **3 bits por partido**, hasta 16 partidos (48 bits). La conversión vive en [`UtilColumnas.cs`](../Free1X2/Utils/UtilColumnas.cs).

### Codificación de un signo (3 bits)

| Signo | Bits | Valor |
|-------|------|-------|
| `2` | `001` | 1 |
| `X` | `010` | 2 |
| `1` | `100` | 4 |

El decodificador es la cadena `"2X1"` indexada con `(bits & 7) >> 1` (`ConvLongToStr`, línea 25). El partido 1 ocupa los bits 0–2, el partido 2 los bits 3–5, etc.

### Pronósticos: la suma de bits

Un pronóstico **doble o triple es la unión (OR) de sus signos** (`ConvPartidoStrToByte`):

| Pronóstico | Bits | Valor |
|------------|------|-------|
| `1X` | `110` | 6 |
| `12` | `101` | 5 |
| `X2` | `011` | 3 |
| `1X2` (triple) | `111` | 7 |

Esto habilita la operación más importante del motor — **comprobar si una columna está contenida en un pronóstico es un solo AND**:

```csharp
// GeneradorColumnas.AnalizaNuevaColumna / Analizador.compruebaPronostico
(pronosticoBase & columna) == columna   // true ⇒ cada signo de la columna
                                        // está permitido por el pronóstico
```

Igual de barato es **contar aciertos entre dos apuestas**: `ContarBitsA1(ganadora & apuesta)` ([`Escrutador.EscrutaApuestaMultiple`](../Free1X2/Escrutinio/Escrutador.cs)).

### Constantes notables

- `columnaBase = 281474976710655` = 48 bits a 1 = "todo permitido" (cuando la fuente es un archivo y no hay pronóstico).
- `columnaInicial` por nº de partidos (p. ej. **2 513 169 434 916** para 14) = la columna "todo unos" `111…1` codificada; es la semilla del generador.

---

## 3. Flujo 1 — Cálculo de una combinación

El flujo principal del programa (`Combinación → Calcular`). Participan: [`MainForm`](../Free1X2/UI/MainForm.cs) → [`CalculaColumnas`](../Free1X2/UI/CalculaColumnas.cs) → [`Analizador`](../Free1X2/MotorCalculo/Analizador.cs) → [`GeneradorColumnas`](../Free1X2/MotorCalculo/GeneradorColumnas.cs).

### Paso a paso (con datos)

1. **Recogida desde la UI** — `MainForm.AbreCalculoColumnasFrm()`:
   - `ObtenPronosticos()` vuelca los 14 pronósticos del boleto al `Analizador` (`SetPronostico(i, "1,X" …)`).
   - `ObtenPartidosGrupos()` asigna a cada `Grupo` qué partidos le pertenecen.
   - Si el campo Filtro está activo (semáforo verde), `analizador.ArchivoColumnasBase = archivoFiltroCols` (+ `CompletarCon` si el filtro tiene menos partidos).
   - Validación: ningún partido sin pronóstico.

2. **Arranque** — el diálogo `CalculaColumnas` llama a una de estas variantes:
   - `AnalizaCombinacion(archivoResultados)` → **guardar** columnas a `.txt` (sin análisis).
   - `AnalizaCombinacion(false)` → solo **contar** (ni guarda ni analiza).
   - `AnalizaCombinacion(true)` → contar + **análisis** estadístico (Flujo 4).

3. **Inicialización** — `AnalizaCombinacion(bool)`:
   - `InicializaContadores()` → `noColsAnalizadas = noColsAceptadas = 0`.
   - `InicializaParametros()` → relee `parametros.free1x2` (vía `AConfiguracion`) y actualiza los parámetros de cada filtro de cada grupo.
   - `InicializaPronosticoBase()` → `pronosticoBase = ConvStrToLong(pronosticos)` (la máscara AND del paso 5).
   - Construye el `GeneradorColumnas` con **pronósticos** (este flujo) o con **archivo** (Flujo 2).

4. **Enumeración** — `GeneradorColumnas.GenerarColumnas()`:
   - Parte de `columnaInicial` (todo unos) y la analiza.
   - `GeneraColumnas(valor, posición, profundidad)` recorre **recursivamente** el árbol de variaciones: para cada partido desde `posicionInicial`, sustituye el signo por `X` (`… ^ 1 << (partido*3+1)`) y por `2` (`… ^ 1 << (partido*3)`), analiza cada variante y recursa sobre los partidos siguientes. Resultado: las **3^14 = 4 782 969** columnas posibles se enumeran sin generar duplicados, todo en aritmética de bits.

5. **Criba barata primero** — `AnalizaNuevaColumna(long)`:
   ```csharp
   if ((columnaBase & columna) == columna)   // ¿contenida en el pronóstico?
       analizador.AnalizaColumna(columna);   // solo entonces se valida en serio
   ```
   El AND descarta la inmensa mayoría del universo a coste casi nulo; solo las columnas compatibles con el pronóstico entran a la cadena de condiciones.

6. **Validación completa** — `Analizador.AnalizaColumna(long)` (ver Flujo 3 para el detalle):
   - `ctrlGrupos.RecalcularControladorGrupos()` y `ctrlGrupos.AnalizaColumna(columna)` → condiciones de todos los grupos.
   - Si hay reglas If-Then activas, `IfThen.CompruebaPronostico(columna, GruposPartidos)`.
   - Cada ~800 ms se llama a `UiPump.Pump()` para que la UI siga viva (ver §11).

7. **Salida**:
   - `noColsAnalizadas++` siempre; `noColsAceptadas++` si la columna pasó todo.
   - Si `guardarCols`: `archivoCols.GuardarCols(ConvLongToStr(columna))` — la columna vuelve a texto **solo** al persistir.
   - El diálogo muestra en vivo: columnas procesadas, admitidas, **coste** (`ColsAceptadas × VariablesGlobales.PrecioApuesta`) y porcentaje de aceptación.

### Diagrama de datos del flujo

```
pronósticos string[14] ──ConvStrToLong──► pronosticoBase: long (máscara)
                                                 │
columnaInicial: long ──recursión bit a bit──► candidata: long
                                                 │
                              AND con pronosticoBase (criba O(1))
                                                 │ pasa
                              Grupos → Filtros → IfThen (criba O(filtros))
                                                 │ pasa
                    contador++ ──ConvLongToStr──► "1X211X…" → .txt
```

---

## 4. Flujo 2 — Cálculo con archivo-filtro

Mismo pipeline, distinta **fuente**: en vez de enumerar 3^14 columnas, `GeneradorColumnas(archivoColsBase)` lee un `.txt` línea a línea (`UsaColumnasArchivo()`):

1. `ArchivoColumnasTexto` itera el archivo (`SiguienteColumna()` / `LeeColumnaSinComas()`).
2. **Normalización de longitud** por columna:
   - Más partidos que `VariablesGlobales.NumeroPartidos` → se **trunca** (`Substring(0, N)`).
   - Menos partidos → se **completa** con el texto de `CompletarCon` (campo de la UI).
3. Cada línea se convierte a `long` y entra por el mismo `AnalizaNuevaColumna`.

**Semántica resultante:** el resultado es siempre la **intersección** {columnas del archivo} ∩ {pronóstico} ∩ {condiciones}. Por eso el manual de usuario dice que con filtro activo "el resultado es un subconjunto del filtro".

Además del filtro global, cada **grupo** puede tener su propio **filtro parcial** (`Grupo.UsaFiltroParcial`): el archivo del grupo se carga una sola vez en memoria como mapa de bits (`InicializarColumnasFiltro`) y la columna del grupo se busca ahí (`AnalizaFiltroColumnas`).

---

## 5. Flujo 3 — Cadena de validación de condiciones

La jerarquía de validación tiene **cuatro niveles**, todos con cortocircuito (la primera condición que falla detiene la cadena) y con **memoria** (si nada cambió desde la columna anterior en ese nivel, se reutiliza el veredicto: flags `reCalcular*` + `*ValidoMemoria`).

```
ControladorGrupos                 (1 por combinación)
 ├─ sin conjuntos: TODOS los ControlGrupos deben aceptar
 └─ con conjuntos: ControlConjuntos[0] = "libres" (deben aceptar todos);
                   resto: se cuentan ControlGrupos fallados y se consulta
                   fallosPermitidos[noFallos]  ← tolerancia ENTRE controles
      │
ControlGrupos                     (agrupa grupos)
 ├─ sin control de fallos: todos sus Grupos deben aceptar
 └─ con control de fallos: cuenta grupos fallados → fallosPermitidos[noFallos]
      │
Grupo                             (boleto base = grupo 0, subsistemas = 1..n)
 │  1. si el grupo no usa todos los partidos: columna &= máscaraGrupo
 │  2. for filtro in filtros: if (filtro.IsActive && !filtro.Analizar(col)) → rechaza
 │  3. tolerancias del grupo (SonTolGrupoValidas / …ConFallos)
 │  4. filtro parcial del grupo (mapa de bits del .txt del grupo)
      │
IFiltro (cadena dentro del grupo) — cada condición de la UI es una clase:
   FiltroNoVariantes, FiltroSignosSeguidos, FiltroDibujos, FiltroInterrupciones,
   FiltroDistancias, FiltroContactos, FiltroFormatosSignos, FiltroFormatos123,
   FiltroPesosNumericos, FiltroValoracionSignos, FiltroSimetrias,
   FiltroDiferencias, FiltroGruposEquipos, FiltroColProbables
```

Tras los grupos, el `Analizador` aplica el **ControladorIfThen** (condiciones relacionadas): reglas "si se cumple A, exige B" evaluadas sobre la misma columna `long`.

**Claves de rendimiento del diseño:**
- Cada `IFiltro.Analizar(long)` opera sobre bits, sin strings.
- El orden de los filtros dentro del grupo es fijo (se crean todos en el constructor de `Grupo`; los inactivos se saltan con `IsActive`).
- Las "tolerancias" en cada nivel convierten un AND estricto en un **presupuesto de fallos**: `fallosPermitidos` es un `bool[]` indexado por número de fallos.

---

## 6. Flujo 4 — Análisis de combinación

Variante del Flujo 1 con recolección estadística (`AnalizaCombinacion(true)`), usada por "Análisis de Signos", "Analizar fichero", etc.

1. `InicializarContenedorAnalisis(esArchivo)` crea el [`ContenedorAnalisisGlobal`](../Free1X2/Analisis/ContenedorAnalisisGlobal.cs) dimensionado por nº de grupos y nº de controles, y consulta qué condiciones del grupo base están activas (`UsaSimetrias/UsaDiferencias/UsaValoraciones/UsaCPs/UsaFormatos`) para preparar sus acumuladores.
2. La validación usa las sobrecargas `AnalizaColumna(long, contenedor)`: **los filtros se evalúan aunque estén inactivos** (`filtro.AnalisisActivo`) para poder informar de su distribución, y el contenedor acumula por columna aceptada (`AnalisisGrupos.IncrementarContador(columna)`, `ColumnasPorFallosDeGrupos[n]++`, etc.).
3. Al terminar, si hubo columnas aceptadas, el motor pide a la UI que muestre el visor:
   ```csharp
   Free1X2.Abstractions.UiPump.Pump();                 // vaciar cola de eventos
   Free1X2.Abstractions.AnalisisUi.MostrarVisor(contenedor, GruposPartidos[0]);
   ```
   (en WinForms el hook abre `VisorAnalisisColumnasFrm`; ver §11).

**Dos modos:** `EsAnalisisExterno = true` cuando se analiza un archivo arbitrario (ignora `VariablesGlobales.NumeroPartidos` y usa la longitud real del archivo); `false` cuando se analiza la combinación en pantalla.

---

## 7. Flujo 5 — Reducción

Entrada y salida son **archivos de columnas `.txt`**; el motor de cálculo no participa. Contrato: [`IReduccion`](../Free1X2/Reduccion/IReductor.cs).

```csharp
void Inicializa(string entrada, int nivelReduccion);
void ComienzaReduccion(string archivoEntrada, string salida,
                       int nivelReduccion, int maxCol, int percent);
int NoColumnasIniciales / NoColumnasFinales / NoColumnasProcesadas
```

Flujo en [`ReductorFrm`](../Free1X2/UI/ReductorFrm.cs):

1. Usuario elige archivo de entrada (la "madre"), **nivel** (categoría a garantizar: reducir al 13, al 12…), método y opcionalmente `maxCol` / `percent` (reducciones porcentuales).
2. Según el método se instancia el algoritmo: [`JDC`](../Free1X2/Reduccion/JDC.cs), [`JDCdobleContador`](../Free1X2/Reduccion/JDCdobleContador.cs), [`ReductorTM`](../Free1X2/Reduccion/ReductorTM.cs), [`XFSF`](../Free1X2/Reduccion/XFSF.cs) / [`xfsfV3`](../Free1X2/Reduccion/xfsfV3.cs), [`JLPM`](../Free1X2/Reduccion/JLPM.cs), [`Redu1305Xfsf`](../Free1X2/Reduccion/Redu1305Xfsf.cs) — todos heredan de [`ReductorBase.Base`](../Free1X2/Reduccion/ReductorBase.cs) con plantilla `EntradaDeDatos → Reduce → GrabacionDeReductoras`.
3. **Invariante de los algoritmos:** las columnas elegidas ("reductoras") deben **cubrir** la madre — para cada columna de la madre debe existir una reductora a distancia de Hamming ≤ (14 − nivel). Así, si la ganadora estaba en la madre, alguna reductora tiene al menos `nivel` aciertos.
4. Salida: `.txt` con las reductoras + contadores de progreso (la UI los lee en un timer). `Cancelar()` pone un flag que corta el bucle.

**Garantía matemática, no probabilística:** reducir al 12 garantiza al menos un 12 *si la madre contenía el 14*. El precio es perder categorías superiores.

---

## 8. Flujo 6 — Escrutinio

Comprueba archivos de columnas contra el resultado real de la jornada. Núcleo: [`Escrutador`](../Free1X2/Escrutinio/Escrutador.cs).

### Datos de entrada
- **Columna ganadora** (string de 14 signos; admite `*` como comodín que siempre acierta).
- **Archivo(s) de columnas** a escrutar.
- **Premios aceptados** (qué nº de aciertos cuentan como premio, p. ej. `10,11,12,13,14`).

### Proceso (`EscrutaColumna`)

```
para cada línea del archivo:
    na = nº de posiciones donde ganadora[i] == columna[i]  (o ganadora[i]=='*')
    nc[na]++                                  ← histograma de aciertos (0..14)
    si na ∈ premiosAceptados:
        añade ColumnasPremiadas{columna, fichero, jornada, nºcolumna,
                                premio=na, boleto = ceil(nºcolumna/8)}
```

- El histograma `nc[]` se vuelca a un `DataSet` (`PonerPremios` → tabla `Resultados`: una fila por jornada/columna con columnas `Acertados-14`, `Acertados-13`…) que la UI muestra en grid.
- `nº de boleto = numCol/8` redondeado arriba: 8 apuestas por boleto físico.
- `EscrutaCombConTemporada` repite el proceso para cada jornada de un archivo histórico (banco de pruebas / simulador).
- Para apuestas múltiples codificadas como `long` existe el camino rápido: `ContarBitsA1(ganadora & apuesta)`.

Este flujo alimenta también **Premiadas**, **Estimación de Premios** y el **Banco de Pruebas** (simulación de un sistema contra temporadas completas).

---

## 9. Flujo 7 — Columnas Probables (CP)

La condición más rica del motor. Cada CP ([`ColumnaProbable`](../Free1X2/MotorCalculo/ColumnaProbable.cs)) es **un segundo pronóstico con presupuesto de fallos**, y [`FiltroColProbables`](../Free1X2/MotorCalculo/FiltroColProbables.cs) gestiona la lista.

### Datos de una CP
- Pronóstico propio por partido (puede ser doble/triple: misma codificación de bits).
- **Rangos** definidos como texto `"3#5-7#9"` → expandidos por [`RangosHelper`](../Free1X2.Domain/Utils/RangosHelper.cs) a un `bool[]` indexado por valor:
  - `noAciertos` permitidos, `aciertosSeguidos`, `fallosSeguidos`, `puntos` (sistema de puntos fijo/doble/triple de `parametros.free1x2`).
  - Tolerancias propias (`ACTol`, `ACSTol`): valores "tolerados" fuera del rango estricto.

### Evaluación por columna
`Analizar(long columna)` compara la columna candidata contra el pronóstico de la CP partido a partido (AND de bits), cuenta aciertos, rachas de aciertos y de fallos, y consulta los `bool[]`. El resultado entra al presupuesto de fallos del [`CPControlFallos`](../Free1X2/MotorCalculo/CPControlFallos.cs) / [`ControladorCPControlFallos`](../Free1X2/MotorCalculo/ControladorCPControlFallos.cs).

### Relaciones entre CPs
Tres controladores evalúan condiciones **sobre el conjunto de CPs** una vez calculados los aciertos de cada una para la columna en curso:

| Controlador | Condición |
|---|---|
| [`ControladorRelacionesCP1`](../Free1X2/MotorCalculo/ControladorRelacionesCP1.cs) | **Suma de aciertos** de varias CPs en rango |
| [`ControladorRelacionesCP2`](../Free1X2/MotorCalculo/ControladorRelacionesCP2.cs) | **Recorrido** (máx − mín aciertos) limitado |
| [`ControladorRelacionesCP3`](../Free1X2/MotorCalculo/ControladorRelacionesCP3.cs) | Relaciones lógicas "si CP A acierta ≤ x, CP B debe acertar ≥ y" |

(Análogo para grupos de equipos: `ControladorRelacionesGE1`.)

### Generación de CPs
`Utilidades → Generador CP` usa [`EntradaSalida/GenerarCPs/`](../Free1X2/EntradaSalida/GenerarCPs/) (`CPs.cs`, `ColumnasProbables.xsd`, `Valoracion.cs`): produce CPs desde valoraciones/estadísticas y las persiste en XML conforme al esquema.

---

## 10. Flujo 8 — Persistencia y configuración

### `.comb` — la combinación completa (XML)
[`ArchivoCombinacion`](../Free1X2/EntradaSalida/ArchivoCombinacion.cs) serializa/deserializa **todo el estado del sistema**:

```
.comb ─┬─ Equipos[14]                 (LeeEquipos)
       ├─ Pronósticos[14]             (LeePronosticos)
       ├─ Grupos + sus condiciones    (LeeGrupos → cada filtro con sus datos)
       │   └─ partidos activos, tolerancias, filtro parcial por grupo
       ├─ ControlesGrupos             (fallos permitidos entre grupos)
       ├─ ControlesConjuntos          (fallos entre controles)
       ├─ IfThen                      (CargaIfThen)
       └─ Ruta del archivo-filtro     (LeeFiltroColumnas)
```

Al abrir un `.comb`, `MainForm` reconstruye `Analizador` + `ControladorGrupos` desde el XML y repinta los semáforos de condiciones. Al guardar, `GuardaArchivo()` recorre la misma estructura en sentido inverso. Los datos de cada condición viajan vía las clases `F*Data` de [`EntradaSalida/`](../Free1X2/EntradaSalida/) (`FDibujosData`, `FColProbablesData`…), una por filtro (patrón DTO).

### `.txt` — columnas
[`ArchivoColumnasTexto`](../Free1X2/EntradaSalida/ArchivoColumnasTexto.cs) (implementa `IArchivoColumnas`): una columna por línea, con o sin comas. Es el **formato de intercambio** de todos los flujos (filtros, reducción, escrutinio, operaciones).

### `parametros.free1x2` — configuración global
[`AConfiguracion`](../Free1X2/EntradaSalida/AConfiguracion.cs) lee el archivo al lado del ejecutable (`System.AppContext.BaseDirectory`). [`VariablesGlobales`](../Free1X2/VariablesGlobales.cs) lo carga **una vez en su constructor estático** y expone todo como propiedades estáticas:

- `NumeroPartidos` (dimensiona el motor: máscaras, semillas, arrays de premios), `Separador`, `Desplazamiento`.
- `PuntosFijos/Dobles/Triples` (sistema de puntos de las CPs).
- `PrecioApuesta`, `Moneda`, `Porcentaje14`, `Recaudacion` (coste y estimación de premios).
- Flags `analizar*` (qué condiciones recoge el Flujo 4), toolbars visibles, idioma ([`ArchivoIdioma`](../Free1X2/EntradaSalida/ArchivoIdioma.cs) → `diccionarioIdioma`).

**Implicación de diseño:** `VariablesGlobales.NumeroPartidos` es un acoplamiento global — el `Analizador` puede recibir otro nº de partidos por constructor (análisis externo), pero el resto del motor consulta la estática. Cualquier flujo que cambie el nº de partidos debe hacerlo **antes** de instanciar el motor.

---

## 11. Acoplamiento con la UI y migración WinUI 3

El dominio fue desacoplado de WinForms mediante **tres shims estáticos** en [`Free1X2.Domain/Abstractions/UiHooks.cs`](../Free1X2.Domain/Abstractions/UiHooks.cs); el motor llama a la abstracción y cada frontend la cablea al arrancar:

| Shim | Llamado desde | WinForms cablea (en `Program.WireDomainHooks`) |
|---|---|---|
| `UiPump.Pump` | `Analizador` (cada ~800 ms), reductores, escrutador | `Application.DoEvents` |
| `UserDialogs.ShowError/ShowInfo` | `ControlGrupos`, `Analizador` | `MessageBox.Show` |
| `AnalisisUi.MostrarVisor` | fin del Flujo 4 | abre `VisorAnalisisColumnasFrm` |

Para **WinUI 3** (rama `winui3-migration`, proyecto [`Free1X2.WinUI`](../Free1X2.WinUI/)) el mismo contrato aplica:
- `UiPump.Pump` → no-op o `DispatcherQueue` yield (mejor: mover el cálculo a `Task.Run` y reportar progreso con `IProgress<T>`).
- `UserDialogs` → `ContentDialog`.
- `AnalisisUi.MostrarVisor` → navegar a `VisorAnalisisColumnasFrmPage` pasando el contenedor.

Las 111 páginas portadas (`Free1X2.WinUI/Views/Ported/`) replican las pantallas; el **cableado de estos flujos** a sus ViewModels es el trabajo pendiente de la migración: cada ViewModel debe invocar exactamente las mismas entradas de flujo descritas aquí (`Analizador.AnalizaCombinacion`, `IReduccion.ComienzaReduccion`, `Escrutador.EscrutaCombConTemporada`, `ArchivoCombinacion`…), que ya no dependen de WinForms.

---

*Documento generado a partir del análisis del código fuente (rama `winui3-migration`). Para el detalle de pantallas y uso, ver el [Manual de usuario](MANUAL_USUARIO.md); para la arquitectura técnica, [`ANALISIS_TECNICO.md`](ANALISIS_TECNICO.md).*
