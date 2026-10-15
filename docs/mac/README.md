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
| [11](ESPEC_11_ARQUITECTURA_SWIFT.md) | **Arquitectura Swift** | — | Módulos, modelo configuración/ejecución, errores, concurrencia, `Entorno`, `Sesion`, persistencia, rendimiento; DP/DT |
| [12](ESPEC_12_ORACULO.md) | **Oráculo** | — | Oráculo **por lectura** (el original nunca se ejecuta): origen de cada esperado, formato y carga de golden, suites `G-NET`, niveles O1/O2/O3, corpus |
| [13](ESPEC_13_CASOS_DE_USO.md) | **API de casos de uso** | — | 193 CU + 10 del shell: firma, dependencias, oráculo |
| [14](ESPEC_14_PRUEBAS.md) | **Pruebas** | — | Niveles, dobles, cobertura, port de los 138 tests, rendimiento, CI |
| [15](ESPEC_15_DISENO_UI.md) | **Diseño de UI** | — | Sistema de diseño, ventana, menús, mockups por plantilla, mapa de las 108 pantallas |
| [16](ESPEC_16_PIEZAS_TRANSVERSALES.md) | **Piezas transversales** | — | `FormatoNET`, XML, ZIP, texto, `AleatorioNET`, ayudantes únicos |
| [GLOSARIO](GLOSARIO.md) · [DECISIONES](DECISIONES.md) · [REVISION_SPECS](REVISION_SPECS.md) | Nombres e IDs · registro único de decisiones · revisión que originó 11–16 | — | |

01–10: ~13 400 líneas de comportamiento; 108 pantallas (105 ViewModels + 3 sin ViewModel). Cada espec termina en
**«Abiertas»** (dudas no verificadas).

## Estado de la verificación

- Los valores «esperado» de los ejemplos y casos golden salen de **réplicas Python** hechas al
  redactar, **no** del motor C#. Excepción: las réplicas de `JDCDobleContador`, `ReductorTM` y
  `RelacionCP1` reproducen exactamente los golden existentes. En M1 entran como `modelo` y cada
  suite lleva su caso `lectura` de contraste (ESPEC_12 §1).
- Las afirmaciones «por lectura» (no ejecutadas) están marcadas así en cada espec.
- La lógica de los ViewModels no tiene hoy tests ni golden (el proyecto de tests solo usa Domain).

## Decisiones

Todas las decisiones D1–D8 y las «Abiertas» de 01–10 están **tomadas** en [`DECISIONES.md`](DECISIONES.md)
(mandato del dueño del 2026-10-13: producto = app actual; técnica = buenas prácticas; revisión final del dueño).
D1 = DP-15 · D2 = DP-09 + `G-D2` · D3 = DP-08 · D4 = DP-01 · D5 = DP-02 · D6 = corpus `fixture` + `lectura` (ESPEC_12 §6) · D7 = DP-11 · D8 = DP-06 + DP-14.

## Siguiente fase

**M1 — Oráculo por lectura**: los golden de todos los módulos se escriben a mano leyendo el C# (nunca ejecutándolo), cada caso con
su `fuente`, y se versionan como datos en el paquete Swift. Procedimiento y
criterio de cierre en [ESPEC_12](ESPEC_12_ORACULO.md) §7.
