# Plan de reescritura nativa para macOS (Apple Silicon)

> Estado: **propuesta**, pendiente de aprobación del dueño (R2).
> Decisión del dueño (2026-10-11): **reescritura completa en Swift / Xcode**, sin portar código
> C#. El código actual (`Free1X2.Domain` + `Free1X2.WinUI`) pasa a ser **especificación** y
> **oráculo de pruebas**.
>
> **La app Mac es 100 % Swift nativo arm64: no contiene, no enlaza y no ejecuta C#.** El C# solo
> se usa fuera del proyecto Mac para exportar una vez los datos de referencia (golden) que se
> versionan como ficheros JSON/TXT de test.

## 0. Principios

| # | Principio | Consecuencia |
|---|-----------|--------------|
| M-R1 | **Mismos resultados que hoy.** Se reescribe el código, no la lógica de negocio. | Cada módulo Swift se acepta solo si reproduce **byte a byte** las salidas del motor C# (golden files), además de los 138 tests actuales portados. |
| M-R2 | **Diseño nativo Mac**, no calco de las 108 ventanas Windows. | Mockups aprobados por el dueño antes de escribir cada pantalla (R2). |
| M-R3 | **Compatibilidad de ficheros.** | Los `.txt`, `.comb`, condiciones, `parametros.free1x2`, `.dat`, `.lang`, `.xml` existentes se leen y escriben igual. |
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

```
Free1X2.Mac/                         (proyecto Xcode, en este mismo repo)
├─ Free1X2.xcodeproj                 app macOS, arm64 (+ x86_64 opcional), SwiftUI
├─ App/                              @main, ventanas, menús (Commands), ajustes, idiomas
│   ├─ Funciones/<Área>/             Vista + ViewModel (@Observable) por área funcional
│   └─ Compartido/                   Boleto, porcentajes, semáforo, gráficos (Swift Charts)
└─ Packages/Free1X2Kit/              Swift Package (sin dependencia de AppKit/SwiftUI)
    ├─ Free1X2Core      motor: Columnas, Motor (generador+filtros), Reduccion,
    │                   Escrutinio, Analisis, CasosDeUso. Structs Sendable, sin estado global.
    ├─ Free1X2Formats   E/S compatible con los ficheros actuales
    ├─ Free1X2Online    URLSession async + caché offline-first
    └─ Tests/           Swift Testing: 138 tests portados + golden diferenciales
tools/GoldenGen/                     FUERA de la app: exporta los golden una vez (no se distribuye)
```

Decisiones técnicas:

- **Concurrencia**: Swift 6 estricto. Cálculos largos como `async` cancelables con progreso por
  `AsyncStream` (sustituye `UiPump`/`IProgressNotifier`). Paralelismo con `TaskGroup` solo donde
  el orden de salida se preserve.
- **Rendimiento**: bucles calientes sobre `[UInt64]` contiguos / `UnsafeBufferPointer`, sin
  clases ni existenciales en el camino crítico. Objetivo: igual o más rápido que la versión Windows.
- **Ficheros**: `fileImporter`/`fileExporter` (o `NSOpenPanel`); carpetas de trabajo en
  `~/Library/Application Support/Free1X2` (hoy se crean junto al `.exe`, `App.xaml.cs:106-112`;
  en Mac el bundle no es escribible).
- **Idiomas**: String Catalog (`.xcstrings`) es/en; `en-US.lang` como fuente de traducciones.
- **Impresión**: `NSPrintOperation` sobre vista renderizada.
- **Online**: mismo contrato `docs/API_CLUBPROGOL.md`; caché `jornada-{es,mx}.json`.
- **Plataforma mínima**: macOS 14 Sonoma (Observation + Swift Charts maduros; soportado por M1).

## 3. Estrategia de verificación (oráculo)

1. `tools/GoldenGen` (herramienta de datos, fuera del proyecto Mac) ejecuta una vez el motor
   actual con entradas variadas y deterministas (semilla fija)
   por módulo y escribe `entrada → salida` en `Packages/Free1X2Kit/Tests/Golden/`.
2. Los tests Swift leen esos golden y exigen igualdad exacta.
3. Los golden se versionan como datos. La CI del proyecto Mac (GitHub Actions, runner macOS
   arm64) solo ejecuta `swift test` + `xcodebuild build`: **cero C# en la cadena Mac**.

## 4. UI

No se replican las 108 ventanas: se agrupan en áreas con barra lateral (`NavigationSplitView`),
inspector y hojas modales, p. ej.: **Boleto · Condiciones y filtros · Cálculo · Reducción ·
Escrutinio · Análisis · Columnas probables · Herramientas · Online**. El mapa
«ventana Windows → área Mac» y los mockups se entregan en M0/M4 para aprobación.

## 5. Fases

| Fase | Entregable | Cierre (evidencia) |
|------|-----------|--------------------|
| **M0 Especificación** | `docs/mac/ESPEC_*.md`: por módulo, entradas/salidas/algoritmo con `file:line`; mapa de las 108 pantallas a áreas | Revisión del dueño |
| **M1 Oráculo** | `tools/GoldenGen` + golden de todos los módulos versionados como datos | Golden en `Packages/Free1X2Kit/Tests/Golden/` |
| **M2 Esqueleto** | Proyecto Xcode + `Free1X2Kit` + CI macOS | CI verde (build + 0 tests) |
| **M3 Motor** | Por orden: Columnas/Utils → Escrutinio → Generador+filtros → Reducción → Análisis → Subir categoría | Golden 100 % idénticos + 138 tests portados + benchmark |
| **M4 Formatos y Online** | Lectura/escritura de todos los ficheros; cliente online | Round-trip con ficheros reales |
| **M5 UI** | Mockups aprobados → áreas SwiftUI, empezando por el flujo principal | Smoke de todas las áreas en M1 |
| **M6 Distribución** | Firma Developer ID + notarización + `.dmg` | Instalable verificado en un Mac limpio |

## 6. Requisitos del dueño

- Mac M1 con Xcode reciente (este entorno de nube es Linux: no compila Swift/macOS; la
  verificación se hace en CI macOS y en tu Mac).
- Cuenta Apple Developer (99 USD/año) solo para M6 (firma/notarización).

## 7. Decisiones pendientes (R2)

1. Aprobar este plan y la arquitectura §2.
2. Ubicación: carpeta `Free1X2.Mac/` en este repo (propuesto, junto al oráculo) o repo aparte.
3. Distribución: fuera de la Mac App Store (propuesto) o dentro (exige App Sandbox).
