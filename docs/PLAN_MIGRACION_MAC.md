# Plan de reescritura nativa para macOS (Apple Silicon)

> Estado: **propuesta**, pendiente de aprobación del dueño (R2).
> Decisión del dueño (2026-10-11): **reescritura completa en Swift / Xcode**, sin portar código
> C#. El código actual (`Free1X2.Domain` + `Free1X2.WinUI`) pasa a ser **especificación** y
> **oráculo de pruebas**.
>
> **La app Mac es 100 % Swift nativo arm64: no contiene, no enlaza y no ejecuta C#.** El C# **solo
> se lee** (requisitos, especificación, diseño); nunca se ejecuta, ni la app ni sus tests (regla del
> dueño 2026-10-14). Los datos de referencia (golden) se escriben a mano leyendo el código y se
> versionan como ficheros JSON/TXT de test.

## 0. Principios

| # | Principio | Consecuencia |
|---|-----------|--------------|
| M-R1 | **Mismos resultados que hoy.** Se reescribe el código, no la lógica de negocio. | Cada módulo Swift se acepta solo si reproduce **byte a byte** las salidas que da el código C# según su lectura (golden, `mac/ESPEC_12_ORACULO.md`), además de los tests actuales aplicables portados (85 de 138: `mac/ESPEC_14_PRUEBAS.md` §5). |
| M-R2 | **Estructura de la app actual con convenciones nativas Mac** (decisión del dueño 2026-10-13: «lo define la app actual»). | Mismos menús, barra, pantallas y comportamiento; controles, atajos y ventanas nativos. Mockups en `mac/ESPEC_15_DISENO_UI.md`, revisados por el dueño antes de M5. |
| M-R3 | **Compatibilidad de ficheros.** | Los `.txt`, `.comb`, condiciones, `parametros.free1x2`, `.dat`, `.xml` existentes se leen y escriben igual (mismos bytes que la app Windows: DP-02). |
| M-R4 | **Sin estado global.** | `VariablesGlobales` (44 miembros `public static`) se sustituye por valores explícitos (`Sesion`). |
| M-R5 | **Evidencia** (R3/R4 vigentes). | Un ítem se cierra con CI verde en runner macOS arm64 + `file:line`. |

## 1. Qué hay hoy (inventario verificado)

| Pieza C# | Tamaño | Qué hace | Destino Swift |
|----------|--------|----------|---------------|
| `Domain/MotorCalculo` | 57 fich. · 15 603 lín. | Generador de columnas, ~20 filtros (`Filtro*`), grupos, tolerancias, If-Then, relaciones CP/GE, estadísticas | `Free1X2Core/Motor` |
| `Domain/EntradaSalida` | 28 · 6 610 | Lectura/escritura de todos los formatos de fichero | `Free1X2Formats` |
| `Domain/Utils` | 14 · 3 220 | Codificación de columnas (`long`, 3 bits/partido), comparadores, combinaciones, bases, LAE | `Free1X2Core/Columnas` |
| `Domain/Analisis` | 17 · 1 835 | Análisis de combinaciones, simetrías, valoraciones, CP | `Free1X2Core/Analisis` |
| `Domain/Reduccion` | 9 · 1 796 | Reductores JDC, JDC doble contador, JLPM, XFSF (v1/v3), 1305, TM | `Free1X2Core/Reduccion` |
| `Domain/Escrutinio` | 7 · 871 | Escrutinio contra ganadora, premios | `Free1X2Core/Escrutinio` |
| `Domain/Online` | 4 · 425 | Parsers JSON de jornada y equipos (clubprogol) | `Free1X2Online` |
| `Domain/SubirCategoria` | 1 · 262 | Cálculos de subida de categoría | `Free1X2Core/Reduccion` |
| `WinUI` ViewModels | 108 · ~41 800 | Estado de pantalla **y** orquestación de casos de uso | Casos de uso → `Free1X2Core/CasosDeUso`; estado → ViewModels SwiftUI |
| `WinUI` vistas | ~22 000 XAML + 4 400 cód. | 108 pantallas | Rediseño SwiftUI (§4) |
| `WinUI/Datos` | — | `parametros.free1x2`, `aidomnou.cfg`, `ColumnasProbables.xml`, `Equipos/*.dat`, `Idioma/en-US.lang`, `Impresion/*.cfg` | Recursos del bundle + `~/Library/Application Support/Free1X2` |

Modelo central (ver `docs/MANUAL_FLUJOS.md` §2): columna = entero de 64 bits, 3 bits por partido
(`2`=001, `X`=010, `1`=100); pertenencia a pronóstico = `(pron & col) == col`; aciertos =
popcount de `ganadora & apuesta`. Se conserva tal cual en Swift (`UInt64`, `nonzeroBitCount`).

Cobertura actual de tests: los 138 cubren reductores, escrutinio, comparadores, utilidades y
parsers. **No cubren** la mayoría de filtros de `MotorCalculo` ni `Analisis` → hay que ampliar el
oráculo antes de reescribirlos (fase M1).

## 2. Arquitectura destino

> **Definición normativa: [`mac/ESPEC_11_ARQUITECTURA_SWIFT.md`](mac/ESPEC_11_ARQUITECTURA_SWIFT.md)** (módulos, modelo de
> datos, errores, concurrencia, inyección, estado de app). Este apartado es un resumen; si discrepan, manda ESPEC_11.

```
Free1X2.Mac/                         (proyecto Xcode, en este mismo repo)
├─ Free1X2.xcodeproj                 app macOS, arm64 (+ x86_64 opcional), SwiftUI, una ventana
├─ App/                              @main, Sesion, Router, menús (Commands), barra, adaptadores de plataforma
│   ├─ Pantallas/<Menú>/             Vista + <Pantalla>Modelo (@Observable) por pantalla
│   └─ Compartido/                   Boleto, porcentajes, semáforo, gráficos (Swift Charts)
└─ Packages/Free1X2Kit/              Swift Package (sin dependencia de AppKit/SwiftUI)
    ├─ Free1X2Base       columna y codificaciones, FormatoNET, texto, errores, Entorno, progreso
    ├─ Free1X2Motor      motor completo: configuración en structs Sendable (públicos) y ejecución
    │                    en clases internas confinadas a una tarea; sin estado global
    ├─ Free1X2Formatos   E/S compatible con los ficheros actuales
    ├─ Free1X2Online     URLSession async + caché offline-first
    ├─ Free1X2CasosDeUso los 193 casos de uso (lógica que hoy vive en los ViewModels)
    └─ Tests/            Swift Testing: 138 tests portados + golden diferenciales
tools/ModeloSpec/                    FUERA de la app: modelos Python de la spec para calcular esperados (no tocan el C#)
```

Decisiones técnicas:

- **Concurrencia**: Swift 6 estricto. Cálculos largos como `async` cancelables con progreso por
  `AsyncStream` (sustituye `UiPump`/`IProgressNotifier`). Paralelismo con `TaskGroup` solo donde
  el orden de salida se preserve.
- **Rendimiento**: bucles calientes sobre `[UInt64]` contiguos, sin existenciales, closures ni
  asignaciones por columna (clases `final` con despacho estático). Objetivo medible en ESPEC_14 §6.
- **Ficheros**: `fileImporter`/`fileExporter` (o `NSOpenPanel`); carpetas de trabajo en
  `~/Library/Application Support/Free1X2` (hoy se crean junto al `.exe`, `App.xaml.cs:106-112`;
  en Mac el bundle no es escribible).
- **Idiomas**: String Catalog (`.xcstrings`) con `es` base; como hoy, solo Créditos se traduce al inglés; el `.lang` no se porta (DP-07).
- **Impresión**: `NSPrintOperation` sobre vista renderizada.
- **Online**: mismo contrato `docs/API_CLUBPROGOL.md`; caché `jornada-{es,mx}.json`.
- **Plataforma mínima**: macOS 14 Sonoma (Observation + Swift Charts maduros; soportado por M1).

## 3. Estrategia de verificación (oráculo)

1. **El original nunca se ejecuta.** Cada esperado sale de leer el código: literales de los tests C#,
   ficheros de datos del repo, trazas a mano con `file:line`, modelos de la spec contrastados con una
   traza, y el fuente público de .NET 8 para la BCL (`mac/ESPEC_12_ORACULO.md` §1).
2. Los casos se escriben como `entrada → salida` en `Packages/Free1X2Kit/Tests/Golden/`, con su `fuente`.
3. Los tests Swift leen esos golden y exigen igualdad exacta. La CI del proyecto Mac (GitHub Actions,
   runner macOS arm64) solo ejecuta `swift test` + `xcodebuild build`: **cero C# en toda la cadena**.

## 4. UI

Se conserva la estructura de la app actual (DP-11): una ventana con la barra de menús (7 menús, 53 destinos),
la barra de herramientas (6 grupos) y las pantallas apiladas con «Atrás», traducidas a convenciones Mac.
Sistema de diseño, mockups por plantilla y mapa de las 108 pantallas en `mac/ESPEC_15_DISENO_UI.md` (M0).

## 5. Fases

| Fase | Entregable | Cierre (evidencia) |
|------|-----------|--------------------|
| **M0 Especificación** | `docs/mac/ESPEC_00…16`, `GLOSARIO`, `DECISIONES`: comportamiento con `file:line`, arquitectura, oráculo, casos de uso, pruebas, diseño de UI | Revisión del dueño |
| **M1 Oráculo** | Golden de todos los módulos escritos por lectura del C# (con `fuente` por caso), versionados como datos | Golden en `Packages/Free1X2Kit/Tests/Golden/` |
| **M2 Esqueleto** | Proyecto Xcode + `Free1X2Kit` + CI macOS | CI verde (build + 0 tests) |
| **M3 Motor** | Por orden: Columnas/Utils → Escrutinio → Generador+filtros → Reducción → Análisis → Subir categoría | Golden 100 % idénticos + 85 tests portados + benchmark dentro de presupuesto (ESPEC_14 §6) |
| **M4 Formatos y Online** | Lectura/escritura de todos los ficheros; cliente online | Round-trip del corpus (ESPEC_12 §6) |
| **M5 UI** | Mockups de ESPEC_15 revisados → pantallas SwiftUI, empezando por el flujo principal | Smoke de las 109 superficies en el Mac M1 (ESPEC_14 §9) |
| **M6 Distribución** | Firma Developer ID + notarización + `.dmg` | Instalable verificado en un Mac limpio |

## 6. Requisitos del dueño

- Mac M1 con Xcode reciente (este entorno de nube es Linux: no compila Swift/macOS; la
  verificación se hace en CI macOS y en tu Mac).
- Cuenta Apple Developer (99 USD/año) solo para M6 (firma/notarización).

## 7. Decisiones

Tomadas el 2026-10-13 y registradas en [`mac/DECISIONES.md`](mac/DECISIONES.md) (revisión final del dueño):

1. Plan y arquitectura: definidos en `mac/ESPEC_11_ARQUITECTURA_SWIFT.md`.
2. Ubicación: carpeta `Free1X2.Mac/` en este repo (DP-14).
3. Distribución: fuera de la Mac App Store, sin App Sandbox (DP-06).
