# Especificación para la reescritura Swift (fase M0) — índice

Fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). La app Mac es **100 % Swift
nativo arm64**; estos documentos describen qué hace hoy el código C# para reescribirlo sin mirarlo.
Convenciones, plantilla y trampas C#→Swift en [`ESPEC_00_CONVENCIONES.md`](ESPEC_00_CONVENCIONES.md).

| Espec | Alcance | Líneas | Contenido clave |
|-------|---------|-------:|-----------------|
| [01](ESPEC_01_MOTOR_NUCLEO.md) | Motor núcleo | 878 | 24 componentes, pipeline por ruta, 44 miembros de `VariablesGlobales` → `Configuracion`/`Sesion`, protocolo de filtro |
| [02](ESPEC_02_FILTROS_A.md) | Filtros A | 1 005 | Pesos numéricos, Interrupciones, Contactos, Signos seguidos (citas con alias `XX:línea`) |
| [03](ESPEC_03_FILTROS_B.md) | Filtros B | 914 | 21 clases: Distancias, No variantes, Formatos, Valoración, Simetrías, Dibujos, Diferencias, Grupos de equipos, Estadísticas |
| [04](ESPEC_04_CP_RELACIONES.md) | CP y relaciones | 1 313 | 16 componentes, 24 peculiaridades |
| [05](ESPEC_05_REDUCCION_ESCRUTINIO.md) | Reducción y escrutinio | 841 | 13 componentes, cobertura de tests, 33 casos golden |
| [06](ESPEC_06_UTILS_ANALISIS.md) | Utils y análisis | 1 048 | Modelo de columna, 31 componentes |
| [07](ESPEC_07_FORMATOS_ONLINE.md) | Formatos y online | 1 218 | 24 familias / 52 variantes de fichero, rutas macOS, idioma, API |
| [08](ESPEC_08_UI_A.md) | Pantallas 1–40 | 2 058 | 40 pantallas, 90 casos de uso |
| [09](ESPEC_09_UI_B.md) | Pantallas 41–73 | 2 084 | 33 pantallas, 41 casos de uso |
| [10](ESPEC_10_UI_C.md) | Pantallas 74–105 + shell | 1 998 | 35 pantallas (incl. Acerca de/Ayuda/Créditos), 62 casos de uso, menú (64 entradas) |

Total: ~13 400 líneas; 108 pantallas (105 ViewModels + 3 sin ViewModel). Cada espec termina en
**«Abiertas»** (dudas no verificadas).

## Estado de la verificación

- Los valores «esperado» de los ejemplos y casos golden salen de **réplicas Python** hechas al
  redactar, **no** del motor C#. Excepción: las réplicas de `JDCDobleContador`, `ReductorTM` y
  `RelacionCP1` reproducen exactamente los golden existentes. **Todos se regeneran en M1** con
  `tools/GoldenGen`.
- Las afirmaciones «por lectura» (no ejecutadas) están marcadas así en cada espec.
- La lógica de los ViewModels no tiene hoy tests ni golden (el proyecto de tests solo usa Domain).

## Decisiones pendientes del dueño (R2)

| # | Decisión | Dónde |
|---|----------|-------|
| ~~D1~~ | **Resuelta (2026-10-12):** la referencia es **solo la app actual** (WinUI + `Free1X2.Domain`). El WinForms legacy (`Free1X2/`) **no se usa** como referencia; las divergencias con él citadas en 06, 08 y 09 se ignoran. | — |
| D2 | **Comportamientos observados SOLO por lectura de código, sin ejecutar (no son errores confirmados; los 138 tests pasan y no cubren estas rutas).** Se verifican ejecutando en M1 antes de cualquier decisión. Lista: If-Then asigna en vez de combinar al contar, `Analizador.cs:100` vs `:150` (01); `ReinicializarVariables` no hace nada tras la 1.ª carga (01, 07); `aciertosMax` de Formatos 1-2-3 no se guarda (03); Dibujos vía If-Then rechaza siempre (03); `FiltroDiferencias.ObtenNoAciertosTolerancias` lanza (01, 03); `EscrutadorComb` falla y orden por Hashtable (05); JDCDobleContador ignora el último partido (05); `ConvStrToLong` sin validar caracteres (06); deduplicado de `ObtenerListaFromTxt` (04, 06); Filtro Pim ejecuta el motor de aidomnou y `aidomnou.cfg` no se lee (09 A-57-1, 10); Grupos de equipos no llega al analizador tras «Abrir» (09 A-62-1) | ver espec. |
| D3 | **Código sin uso**: reductores XFSF, XfsfV3, JLPM; fichero `.lang`; ~15 miembros sin consumidores | 05, 06 (A7), 07 |
| D4 | **Cultura numérica** de referencia (propuesta: es-ES, decimal con coma) | 07 |
| D5 | **Fin de línea** al escribir en Mac: CRLF (compatible con ficheros Windows) o LF | 07 (A1) |
| D6 | **Muestras reales** de `.comb` y condiciones desde Windows (no hay ninguna en el repo) | 07 (A2) |
| D7 | **Áreas y presentación Mac** de cada pantalla (propuestas en las tablas resumen) | 08 §3, 09 §2, 10 §3 |
| D8 | Repo (`Free1X2.Mac/` aquí o repo aparte) y distribución (fuera/dentro de Mac App Store) | Plan §7 |

## Siguiente fase

**M1 — Oráculo**: `tools/GoldenGen` exporta una vez los golden de todos los módulos (en Windows,
para fijar cultura y CRLF reales) y se versionan como datos en el paquete Swift.
