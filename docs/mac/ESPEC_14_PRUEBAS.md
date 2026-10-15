# ESPEC 14 — Estrategia de pruebas, CI y rendimiento

> Cierra H-13 de [`REVISION_SPECS.md`](REVISION_SPECS.md). Depende de [ESPEC_11](ESPEC_11_ARQUITECTURA_SWIFT.md)
> (módulos, `Entorno`) y [ESPEC_12](ESPEC_12_ORACULO.md) (golden). Decisiones técnicas DT-30…DT-39 (`DECISIONES.md`).

## 1. Niveles de prueba

| Nivel | Qué prueba | Dónde | Herramienta | Peso |
|-------|-----------|-------|-------------|------|
| **N1 Unitarias** | Cada función/tipo: ejemplos de las specs, bordes, errores | `Tests/<Módulo>Tests` | Swift Testing (`@Test`, `#expect`, `#require`) | muchos, rápidos (< 1 ms) |
| **N2 Paridad (golden)** | Igualdad exacta con los golden (esperados por lectura, ESPEC_12) | `Tests/<Módulo>Tests/Paridad/` | `@Test(arguments: Golden.casos(…))` | **criterio de aceptación** de cada componente |
| **N3 Propiedades** | Invariantes con entradas generadas (semilla fija, reproducible) | junto a N1 | `GeneradorPruebas` (SplitMix64 con semilla) + bucle de N casos | medio |
| **N4 Casos de uso** | Cada CU de ESPEC_13 sin UI, con `Entorno.pruebas` (ficheros en memoria, reloj fijo, red grabada) | `Tests/Free1X2CasosDeUsoTests` | Swift Testing | uno o más por CU |
| **N5 Modelos de pantalla** | Estado, habilitación, mensajes literales y navegación de cada `<Pantalla>Modelo` | target `Free1X2AppTests` (Xcode) | Swift Testing en `@MainActor` | uno por pantalla |
| **N6 UI** | Smoke de las 109 superficies + 5 flujos principales | target `Free1X2UITests` | XCUITest | pocos |
| **N7 Rendimiento** | Tiempos y memoria de los datasets de §6 | `Free1X2Bench` | ejecutable propio | nocturno + cierre de fase |

Ejemplos de propiedades (N3) obligatorias:
- `convLongToStr(convStrToLong(s)) == s` para columnas simples válidas (06 §1.4); `castInt32NET`/`FormatoNET` ida y vuelta donde el .NET lo garantiza.
- Reducción: `∀ m ∈ madre ∃ r ∈ salida: aciertos(m,r) ≥ nivel` y `Finales ≤ Iniciales` sin límites (05 GR-14).
- Generador: nº de columnas = producto de signos por partido (modo pronóstico); salida ordenada como el DFS (01 §2.1).
- Paralelismo (ESPEC_11 §6.3): resultado particionado == resultado secuencial, para P ∈ {1, 2, 3, 8}.
- Formatos: `leer(escribir(m)) == m` para todo modelo generado.

## 2. Dobles de prueba (target `Free1X2Pruebas`, ESPEC_11 §7)

| Protocolo | Doble | Comportamiento |
|-----------|-------|----------------|
| `Reloj` | `RelojFijo(fecha)` / `RelojManual` | devuelve la fecha dada; `avanzar(_:)` |
| `SistemaArchivos` | `ArchivosEnMemoria` | árbol en memoria con `mtime`, escrituras atómicas, fallos inyectables (permiso denegado, disco lleno) |
| `SistemaArchivos` | `ArchivosTemporales` | directorio real único por test (para probar los adaptadores de producción) |
| `ClienteHTTP` | `ClienteHTTPGrabado` | respuestas por URL: estado, cabeceras, cuerpo, demora, error de red; registra las peticiones |
| `Portapapeles` | `PortapapelesEnMemoria` | — |
| `Registro` | `RegistroEnMemoria` | líneas capturadas para `#expect` |
| `aleatorio` | `AleatorioNET(semilla:)` real | determinista por construcción |
| `Cultura` | `.esES`, `.enUS`, `.esMX` | para probar DP-01 con otras regiones |

`Entorno.pruebas(...)` compone los dobles con valores por defecto (reloj fijo 2026-01-01 12:00 UTC, cultura `es-ES`, ficheros vacíos).
Un único test de integración por adaptador de producción (`URLSession` con `URLProtocol` de prueba: timeouts de 10 s, cabecera
`Accept`, sin caché; `FileManager`; `NSPasteboard` en el target de la app).

## 3. Organización y convenciones

- Un target de tests por módulo (ESPEC_11 §3.3); ficheros `<Componente>Tests.swift`; `@Suite` por componente; nombres de test en español
  que describen el comportamiento (`@Test("ruta A asigna el veredicto del If-Then")`).
- Etiquetas: `.golden`, `.propiedad`, `.lento` (> 1 s), `.integracion`. El CI de cada PR excluye `.lento`; el nocturno lo incluye.
- Los tests no comparten estado: cada uno crea su `Entorno.pruebas()`; paralelos por defecto.
- Prohibido en tests: red real, reloj real, `sleep`, rutas absolutas del desarrollador.
- Cada peculiaridad «[PRESERVAR]» de las specs tiene al menos un test N1 o N2 que la fija, con el ID de la peculiaridad en el nombre.

## 4. Cobertura

| Módulo | Funcional (obligatoria) | Líneas (`swift test --enable-code-coverage`, llvm-cov) |
|--------|-------------------------|-----------------------------------|
| `Free1X2Base` | todas las suites `G-NET` y `base/` | ≥ 95 % |
| `Free1X2Motor` | todas las suites de motor de ESPEC_12 §4.2 + `G-D2` + `G-ERR` | ≥ 90 % |
| `Free1X2Formatos` | lectura, escritura y round-trip de todo el corpus (ESPEC_12 §6) | ≥ 90 % |
| `Free1X2Online` | parsers, caché y cliente (todos los casos de 07 §6.9-6.11) | ≥ 90 % |
| `Free1X2CasosDeUso` | los 193 CU (N4) con su oráculo O1/O2/O3 | ≥ 85 % |
| App (modelos de pantalla) | las 108 pantallas + Inicio (N5) | ≥ 70 % |

El CI falla si un módulo baja de su umbral. La cobertura funcional se comprueba con `indice.json` (nº de casos por suite) y con la tabla de ESPEC_13.

## 5. Port de los 138 tests actuales (`Free1X2.Domain.Tests`)

| Fichero C# | Casos | Destino Swift | Nota |
|------------|------:|---------------|------|
| `UtilColumnasTests.cs` | 3 + 9 | `Free1X2BaseTests/UtilColumnasTests` | |
| `ConvertidorDeBasesTests.cs` | 2 + 12 | `Free1X2BaseTests/ConvertidorDeBasesTests` | |
| `RangosHelperTests.cs`, `RangosHelperBoolTests.cs` | 1 + 5, 5 | `Free1X2MotorTests/RangosHelperTests` | |
| `SumadorCombinacionesTests.cs`, `TramoTests.cs` | 5, 6 | `Free1X2MotorTests/Utils/` | |
| `FlujosClaveTests.cs`, `ManualFuncionalTests.cs` | 4, 2 + 3 | `Free1X2MotorTests/FlujosTests` | E2E del generador y NoVariantes |
| `EscrutinioReduccionFixtureTests.cs`, `JDCDobleContadorGoldenTests.cs` | 2, 5 | `Free1X2MotorTests/Reduccion/`, `Escrutinio/` | golden existentes |
| `RelacionCP1GoldenTests.cs` | 2 | `Free1X2MotorTests/CP/` | |
| `CompresorZipTests.cs` | 1 | `Free1X2FormatosTests/Z3QTests` | |
| `JornadaQuinielaParserTests.cs`, `JornadaQuinielaParserRealBackendTests.cs`, `CatalogoEquiposParserTests.cs`, `JornadaCacheRoundTripTests.cs` | 6, 2, 8, 2 | `Free1X2OnlineTests/` | fixtures `online/*.json` copiados como recursos |
| **Total portado** | **85** | | |
| `ComparadorTests.cs`, `ComparadorBytesTests.cs`, `Comparador2Tests.cs` | 4, 6 + 15, 9 + 5 | **clases no; expectativas sí** (ESPEC_17 §1: 23 casos para `Aciertos` y `CodificacionX0MSB`) | `Comparador`/`Comparador2` del dominio no tienen uso en la app actual (solo XFSF/xfsfV3 huérfanos y `Analisis.Analizador.IteraCols` muerto; SelectorMS usa su copia local, `Free1X2.WinUI/Views/Ported/SelectorMSViewModel.cs:404-405`) → DP-08 |
| `ReductorTMGoldenTests.cs` | 6 | **no se porta** | `ReductorTM` no es seleccionable en la app actual (`Free1X2.WinUI/Views/Ported/ReductorFrmViewModel.cs:43-44`; DEC-507) → DP-08 |
| `AbstractionsTests.cs` | 8 | **sustituido** | `UiHooks` desaparece (ESPEC_11 §7); su intención (dependencias no-op por defecto) la cubren los tests de `Entorno.pruebas` |
| **Total no portado** | **53** | | 85 + 53 = 138. Revisión M1 (ESPEC_17 §1): de estos 53, 23 aportan expectativas; en total, 104 casos golden `test` |

Fixtures `cols_a.txt`, `cols_b.txt`, `cols_con_repetidas.txt`, `ganadora.txt` y `online/*.json` se copian **byte a byte** a `Sources/Free1X2Pruebas/Fixtures/` (DT-28; hecho en M1, SHA-256 en `Golden/indice.json`).

## 6. Rendimiento (cierra «igual o más rápido que Windows» sin métrica, PLAN §2)

### 6.1 Datasets fijos

| ID | Escenario | Mide |
|----|-----------|------|
| B1 | 14 triples, sin condiciones, ruta A (contar) | 4 782 969 columnas por el generador + analizador |
| B2 | 14 triples + combinación `corpus/comb/completa.comb` (todas las condiciones), ruta A | coste de los 14 filtros + CP + If-Then |
| B3 | B2 por la ruta B (grabar `.txt`) | E/S de salida |
| B4 | B2 por la ruta C (análisis) | contenedores de análisis |
| B5 | Reductores `JDC`, `JDCDobleContador`, `Redu1305Xfsf` sobre el universo 9T, nivel 12 | reducción |
| B6 | Escrutinio de 100 000 columnas contra 1 ganadora y temporada de 38 ganadoras | escrutinio |
| B7 | Lectura + escritura de un `.txt` de 1 000 000 columnas | formatos |
| B8 | 3 CU pesados de pantalla (los de mayor coste según ESPEC_13: p. ej. ordenar por probabilidad con 4,78 M entradas, 10:532) | CU |

### 6.2 Medición

- `Free1X2Bench` (Release, `-O`), reloj `ContinuousClock`; 2 ejecuciones de calentamiento + 10 medidas; se publica la **mediana** y el
  percentil 90; memoria = pico de RSS (`task_info`).
- Sin referencia C#: el original no se ejecuta (ESPEC_12). La primera medición aceptada de cada dataset fija `bench/linea-base.json`;
  se mide de nuevo al cerrar M3 y M5.

### 6.3 Presupuestos (criterio de cierre de M3)

| Regla | Umbral |
|-------|--------|
| Techo absoluto | mediana en un M1 (Release): B1 ≤ 1 s · B2 ≤ 10 s · B3 ≤ B2 + 3 s · B4 ≤ 2 × B2 · B5 ≤ 10 s · B6 ≤ 1 s · B7 ≤ 3 s · B8 ≤ 5 s por CU. Son techos de usabilidad (DT-30…39); la línea base de la primera medición solo puede endurecerlos |
| Paralelismo | B1/B2 con particiones ≥ 2× más rápido que con P = 1 en un M1 (8 núcleos) |
| Memoria | pico ≤ 64 MB en B1–B3; ≤ 128 MB en B8 (10:532 estimaba 57 MB) |
| Regresión (CI) | > 15 % más lento que `bench/linea-base.json` del mismo tipo de runner ⇒ falla el nocturno |
| Bucle caliente | 0 asignaciones por columna en B1/B2 (verificado con Instruments › Allocations en la revisión de M3) |

## 7. CI (GitHub Actions)

| Flujo | Disparador | Runner | Pasos |
|-------|-----------|--------|-------|
| `mac.yml` | PR y push a `main` | `macos-15` arm64 (o el más reciente disponible) | `swift-format lint --strict` · lint de patrones prohibidos (`fatalError`, `try!`, `as!`, desempaquetado `!`, `print(`; ESPEC_11 §5.2) · `swift build -c release` · `swift test --parallel --skip .lento --enable-code-coverage` + umbrales §4 · `xcodebuild build` + `xcodebuild test` (N5 + smoke N6) |
| `mac-nocturno.yml` | cron diario + manual | `macos` arm64 | todo `mac.yml` + `.lento` + `Free1X2Bench` en modo comparación con la línea base (§6.3) |
| `golden.yml` | — | — | **No existe**: los golden se escriben a mano por lectura y se versionan como datos (ESPEC_12 §7); no hay C# que ejecutar |

Comprobaciones obligatorias para fusionar: `mac.yml` verde. Ningún test se desactiva para hacer pasar el CI: un fallo de paridad se resuelve
en el código o, si el golden estaba mal, corrigiéndolo con la cita `file:line` que lo justifica (ESPEC_12 §1).

## 8. Definición de «hecho» para cada componente o pantalla (R3)

Un ítem del plan Mac solo se marca hecho con **todo** esto:
1. Suite golden de su componente escrita **antes** del código (ESPEC_17 §4) y 100 % verde (N2), con el número de casos de `indice.json`.
2. Tests N1/N3 (y N4/N5 si es CU o pantalla) verdes; peculiaridades `[PRESERVAR]` fijadas.
3. Umbral de cobertura del módulo cumplido.
4. Para el motor: benchmark dentro de presupuesto (§6.3).
5. `mac.yml` verde en el commit y `fichero:línea` de la implementación en la nota de cierre.

## 9. Smoke (equivalente al actual)

- **UI test** `SmokeTests.recorrerTodasLasPantallas`: para cada `Destino` (108 + inicio) navega y comprueba que la vista aparece sin
  error; escribe `SMOKE DONE total=109 ok=109 fail=0` en el log del test (mismo contrato que 10:265-270).
- **Argumento `-smoke`** de la app (sin XCUITest): recorre `Destino.allCases` cada 120 ms y escribe el mismo resultado en
  `FileManager.temporaryDirectory/free1x2_smoke.log`; inerte sin el argumento (equivale a `FREE1X2_SMOKE=1`).
