# ESPEC 00 — Convenciones de la especificación para la reescritura Swift

> Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). Estas especificaciones
> describen **qué hace hoy** el código C# con el detalle suficiente para reescribirlo en Swift
> **sin mirar el C#** y obtener resultados idénticos (M-R1). El C# sigue siendo el oráculo.

## 1. Índice de especificaciones

| Fichero | Alcance |
|---------|---------|
| `ESPEC_01_MOTOR_NUCLEO.md` | Generador de columnas, Analizador, grupos, tolerancias, If-Then, estado global, enums |
| `ESPEC_02_FILTROS_A.md` | Pesos numéricos, Interrupciones, Contactos, Signos seguidos |
| `ESPEC_03_FILTROS_B.md` | Distancias, No variantes, Formatos 1-2-3, Valoración, Simetrías, Dibujos, Formatos de signos, Diferencias, Grupos de equipos, Estadísticas |
| `ESPEC_04_CP_RELACIONES.md` | Columnas probables, control de fallos CP, relaciones CP1/CP2/CP3/GE1, impresión |
| `ESPEC_05_REDUCCION_ESCRUTINIO.md` | Reductores, subir categoría, escrutinio y premios |
| `ESPEC_06_UTILS_ANALISIS.md` | Codificación de columnas, comparadores, combinaciones, bases, LAE, porcentajes, análisis |
| `ESPEC_07_FORMATOS_ONLINE.md` | Todos los formatos de fichero, datos iniciales, idioma, online y caché |
| `ESPEC_08_UI_A.md` · `09_UI_B` · `10_UI_C` | Las 108 pantallas: comportamiento, casos de uso, área Mac propuesta; shell |

## 2. Plantilla obligatoria por componente

```
### <Componente> — `<ruta C#>`
**Propósito.** Una o dos frases.
**Tipos y datos.** Campos con tipo C# → tipo Swift exacto (Int32/Int64/Float/Double/UInt8…),
rangos, invariantes, valores por defecto.
**Entradas / salidas.** Parámetros, retorno, efectos (ficheros, estado global).
**Algoritmo.** Pseudocódigo fiel, en el mismo orden de iteración que el C#. Incluir
desempates, redondeos, divisiones enteras, condiciones de corte y contadores.
**Estado global.** Miembros de `VariablesGlobales` leídos/escritos.
**Peculiaridades a preservar.** Comportamientos raros o aparentes bugs que afectan a la salida
(se replican; se anotan como «preservar»).
**Trampas Swift.** Las de §3 que aplican aquí, concretas.
**Casos golden propuestos.** Entradas concretas para que `tools/GoldenGen` genere la salida.
**Evidencia.** `fichero:línea` de cada afirmación (R4).
```

## 3. Trampas C# → Swift detectadas en el código (aplican a todas las espec.)

| # | Diferencia | Evidencia | Regla Swift |
|---|-----------|-----------|-------------|
| T1 | **Overflow**: C# envuelve en silencio (no hay `checked`); Swift aborta. | 0 usos de `checked`/`unchecked` en `Free1X2.Domain` | Usar `&+ &- &* &<<` donde el C# pueda desbordar; documentar dónde. |
| T2 | **Columna = `long`** 3 bits/partido. | `Utils/UtilColumnas.cs`, `docs/MANUAL_FLUJOS.md` §2 | `Int64` si hay signo/negativos en juego, si no `UInt64`; respetar la misma semántica de `>>` (aritmético en `long`). |
| T3 | **`float` (32 bits)** en porcentajes, valoraciones, probabilidades. | `Utils/Porcentajes.cs:329-349`, `Analisis/ApuestaProbable.cs:26`, `MotorCalculo/Estadisticas/Estadistica.cs:7` | Usar `Float`, nunca `Double`, donde el C# usa `float`; mismo orden de operaciones y mismos casts intermedios. |
| T4 | **`Math.Round` = redondeo bancario** (ToEven) por defecto. | `Analisis/ContenedorAnalisis.cs:426-429`, `Utils/LAE.cs:146`, `MotorCalculo/Analizador.cs:246` | `.rounded(.toNearestOrEven)`; `Math.Round(x, 2)` requiere réplica exacta. |
| T5 | **Ordenación inestable** (`List.Sort`/`Array.Sort` = introsort). Con claves enteras puras es determinista; con objetos y empates no. | `Utils/UtilidadesEntradasValores.cs:261,288,388`, `Reduccion/JDCdobleContador.cs:33,329`, `MotorCalculo/Analizador.cs:253`, `EntradaSalida/GenerarCPs/CPs.cs:216` | Para enteros: cualquier sort. Con empates de objetos: documentar el orden resultante y replicarlo explícitamente. |
| T6 | **Orden de iteración de colecciones hash**: `Hashtable` no garantiza orden; `Dictionary` conserva inserción si no hay borrados; `Dictionary` Swift es aleatorio. | `Escrutinio/EscrutadorComb.cs:42,235`, `MotorCalculo/Grupo.cs:44,318`, `MotorCalculo/Diferencia.cs:15` | Si la salida depende del orden de iteración, usar array ordenado o `OrderedDictionary` (swift-collections) y documentar el orden real. |
| T7 | **Cultura** en parseo/formato: algunos sitios fijan `es-ES`/`en-US`; los que no, usan la cultura actual. | `EntradaSalida/AConfiguracion.cs:373-410`, `EntradaSalida/GenerarCPs/ColumnasProbables.cs:140` | Fijar siempre el `Locale` explícito equivalente; señalar cada `Parse`/`ToString` sin cultura. |
| T8 | **Texto**: `Encoding.Default` en .NET 8 = UTF-8; `StreamWriter` sin BOM; fin de línea según `WriteLine` (`\r\n` en Windows). | `EntradaSalida/ArchivoIdioma.cs:25`, `EntradaSalida/ArchivoColumnasTexto.cs:60` | Leer aceptando `\r\n` y `\n`; al escribir, decidir y documentar (compatibilidad con ficheros de usuarios Windows). |
| T9 | **División entera** y `%` con negativos: misma semántica en C# y Swift (trunca hacia 0). | — | Mantener tipos enteros; no promover a Double. |
| T10 | **Estado global** (`VariablesGlobales`, 44 miembros estáticos). | `VariablesGlobales.cs` | Pasar a un valor explícito `Sesion`/`Configuracion`; documentar cada lectura/escritura. |
| T11 | **`string.CompareTo`** depende de cultura; `==` es ordinal. | 8 usos de `CompareTo`/`string.Compare` | Documentar si es ordinal o cultural en cada caso. |
| T12 | **Bombeo de UI** (`UiPump.Pump`, `IProgressNotifier`) dentro de bucles largos. | `Abstractions/UiHooks.cs:12-14`, `Abstractions/IProgressNotifier.cs` | `async` + `Task.checkCancellation()` + progreso por `AsyncStream`. |

## 4. Reglas de redacción

- Español, conciso, tablas y pseudocódigo; sin pegar bloques largos de C#.
- Toda afirmación con `fichero:línea` (rutas relativas a la raíz del repo).
- Lo no verificado no se escribe (R4). Las dudas van a una sección final **«Abiertas»**.
- Nombres Swift propuestos en `UpperCamelCase` para tipos y `lowerCamelCase` para miembros,
  conservando el término de dominio en español (p. ej. `FiltroContactos`, `columnaBase`).
