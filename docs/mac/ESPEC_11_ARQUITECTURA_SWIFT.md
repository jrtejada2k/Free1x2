# ESPEC 11 — Arquitectura Swift (el «cómo»)

> Cierre de la fase **M0** de [`../PLAN_MIGRACION_MAC.md`](../PLAN_MIGRACION_MAC.md). ESPEC_01…10 describen **qué hace hoy**
> la app (WinUI + `Free1X2.Domain`); este documento fija **cómo se construye** la app Mac. Cierra los hallazgos
> H-01, H-02, H-04, H-05 y H-14 de [`REVISION_SPECS.md`](REVISION_SPECS.md) y parte de H-11/H-13 (detalle en ESPEC_13/14).
>
> Citas: `NN:línea` = `mac/ESPEC_NN_*.md:línea`; `PLAN:línea` = `docs/PLAN_MIGRACION_MAC.md:línea`.
> Nombres canónicos en [`GLOSARIO.md`](GLOSARIO.md); trampas C#→Swift en [`ESPEC_00`](ESPEC_00_CONVENCIONES.md) §3.

## 0. Cómo se lee y qué manda

1. **Comportamiento**: mandan ESPEC_01…10 (y en M1, el oráculo: ESPEC_12).
2. **Estructura, tipos, errores, concurrencia y nombres**: manda **este documento** + `GLOSARIO.md`. Las «propuestas Swift»
   repartidas en ESPEC_01…10 (p. ej. `final class Sesion` en 01:760, `FiltroProtocol` en 01:777, `EditorCondicion` en 08:123 y 10:308,
   `actor QuinielaOnlineService.shared` en 07:816) quedan **sustituidas** por lo que aquí se fija cuando choquen.
3. Dos tipos de decisión, todas en el registro [`DECISIONES.md`](DECISIONES.md):
   - **DP-nn (producto)**: el dueño ordenó (2026-10-13) «lo define la app actual: usar eso». Cada DP replica el comportamiento actual y cita la evidencia.
   - **DT-nn (técnica)**: el dueño delegó (2026-10-13) aplicar buenas prácticas de velocidad, mantenimiento, arquitectura y pruebas; revisa al final.

---

## 1. Principios

| # | Principio | Consecuencia |
|---|-----------|--------------|
| A1 | **Paridad antes que elegancia** (M-R1). | Ninguna decisión de este documento puede cambiar una salida observable de la app actual; si una buena práctica la cambiaría, se replica el comportamiento y se anota en `DECISIONES.md`. |
| A2 | **Configuración = valores; ejecución = clases** (DT-02). | Lo que el usuario edita y se guarda en ficheros son `struct` `Sendable`/`Equatable`. El estado mutable del cálculo (contadores, memos, referencias compartidas) vive en `final class` internas que nacen y mueren dentro de una ejecución. |
| A3 | **Núcleo sin UI ni E/S implícita.** | `Free1X2Kit` no importa AppKit/SwiftUI. Reloj, azar, ficheros, red, portapapeles y log entran por `Entorno` (§7). |
| A4 | **Sin estado global** (M-R4). | Ni `static var` mutables ni singletons. `VariablesGlobales` → `Configuracion` (valor); `AppState` → `Sesion` (app); hooks estáticos (`UiHooks`) → resultados devueltos o `Entorno`. |
| A5 | **Nunca abortar el proceso por un dato.** | Donde el C# lanza una excepción, Swift lanza un error tipado en el mismo punto (§5). Prohibidos `fatalError`, `!`, `try!` y subíndices sin comprobar en código alcanzable por datos de usuario. |
| A6 | **Una definición por concepto.** | Un solo `Sesion`, un solo `Configuracion`, un solo protocolo de filtro, un esquema de IDs (GLOSARIO). |
| A7 | **Rapidez medida, no supuesta.** | El camino caliente sigue las reglas de §10; cada optimización se acepta solo con golden idénticos y benchmark (ESPEC_14). |
| A8 | **Todo probable sin UI.** | Cada caso de uso es una función `async` sobre valores + `Entorno`; los modelos de pantalla se prueban sin vistas (ESPEC_14). |

---

## 2. Decisiones de producto (DP) — replican la app actual

| ID | Tema | Decisión (= comportamiento actual) | Evidencia |
|----|------|------------------------------------|-----------|
| DP-01 | Cultura numérica (D4) | **Igual que hoy, punto por punto**: C1–C2 con `es-ES` **fija**; C3–C13 con la **cultura del sistema** (en Mac: región del usuario, vía `Cultura.sistema`). Los golden se generan y comparan con `es-ES` (cultura del oráculo). Nunca se usa `Locale` de Foundation directamente: todo pasa por `FormatoNET` con una `Cultura` explícita (ESPEC_16 §1). | 07:565-587; 01 §7 (A9) |
| DP-02 | Fin de línea y codificación al escribir (D5) | **Los mismos bytes que escribe hoy la app en Windows**: columna «C# escribe (Windows)» de 07 §2.2 (CRLF; UTF-8 sin BOM salvo equipos `.txt` con BOM; `.dat` en Latin-1; XML sin salto final). Lectura: acepta `\r\n`, `\n` y `\r` y BOM, como el C#. | 07:589-606; 05:837 (A10) |
| DP-03 | Nº de partidos | **Igual que hoy**: `NumeroPartidos` de `parametros.free1x2` (defecto 14); el motor admite lo que admite hoy (1…16 en el generador) con los **límites por componente** de 06 §1.5; superar un límite produce el mismo error que el C# (§5). | 06:158-178; 01 §3.1 |
| DP-04 | Recarga de configuración | **Igual que hoy**: `parametros.free1x2` se lee **una vez** al arrancar (todo-o-nada); guardar desde Ajustes escribe el fichero pero no recarga (aviso «Algunos cambios pueden requerir reiniciar»). Excepción conservada: puntos CP y `NumeroPartidos` de `Grupo` se releen del fichero en cada cálculo. | 01 §3.2 (puntos 1-2), 01 §7 (A8) |
| DP-05 | Ventana y documento | **Igual que hoy**: una sola ventana, un solo estado de sesión (`Sesion` ≡ `AppState`), sin modelo de documento (sin marca «editado», «Nueva» sin confirmación). Escena `Window` única. | 10:56-81 (§1.2) |
| DP-06 | Distribución / sandbox (D8) | **Igual que hoy** (app portable con acceso libre a ficheros): **fuera de la Mac App Store, sin App Sandbox**, firma Developer ID + notarización (PLAN M6). Sin *security-scoped bookmarks*: las rutas se guardan absolutas, como hoy. | PLAN §5 (M6), §7.3; 07:1210 (A16) |
| DP-07 | Localización | **Igual que hoy**: interfaz en español; selector Español/English que solo traduce Créditos (6 claves). Infraestructura `.xcstrings` con `es` base para crecer sin rehacer. El `.lang` no se aplica hoy → no se porta. | 10:212-216; 07:660-669 |
| DP-08 | Código sin uso (D3) | **No se porta lo que la app actual no alcanza** (resultado idéntico): reductores `XFSF`, `XfsfV3`, `JLPM` y `ReductorTM` (no seleccionable: `Free1X2.WinUI/Views/Ported/ReductorFrmViewModel.cs:43-44`); `.lang`; `IAppPaths`, `IProgressNotifier`; `aidomnou.cfg`; `Comunicaciones/`; `EscrutadorComb.PonerPremios/AñadirPremiosGlobales`; funciones con «0 consumidores» de 06 §1.4. | 05:831-832 (A4-A5); 01 §2.21-2.22; 06:130-156 |
| DP-09 | Defectos aparentes (D2 y «Defecto aparente» de 08/09/10) | **Se replican** (motor y pantallas), tras confirmarlos con el oráculo (ESPEC_12). Los que en C# acaban en el manejador global se replican como **error tipado con el mismo efecto visible** (diálogo «Se produjo un error inesperado:…»), nunca como cierre de la app (A5). | README «Decisiones» (D2); 10:32 |
| DP-10 | Mejoras de UX (cancelar real, validación en vivo, confirmaciones) | **Ninguna**: cada pantalla conserva sus botones, mensajes y efectos. Donde hoy «Cancelar» no detiene el cálculo (p. ej. aidomnou, 10:1976 A-16), el port tampoco lo hace; la arquitectura queda preparada (§6) para activarlo con una línea si el dueño lo pide. | 09:2056; 10:1976 |
| DP-11 | Navegación y áreas (D7) | **La estructura actual**: 7 menús con sus 53 destinos y 6 grupos de barra; páginas que se apilan en la ventana con «Atrás». Se traduce a convenciones Mac (barra de menús, barra de herramientas personalizable, atajos `⌘`) sin cambiar qué hay ni dónde está. Detalle en ESPEC_15. | 10:92-183 |
| DP-12 | Preferencias | Tema `Sistema/Claro/Oscuro` (defecto Sistema) e idioma `Español/English` (defecto Español) como hoy; visibilidad de grupos de barra se sigue leyendo/escribiendo en `parametros.free1x2` (`GuardarToolBarsVisibles`), con el grupo Filtros siempre visible al arrancar. | 10:164-168,187-216 |
| DP-13 | Carpetas de trabajo | Las **10 carpetas de hoy**, creadas al arrancar, bajo **una sola carpeta base** (hoy, la del `.exe`). En Mac la base es `~/Library/Application Support/Free1X2/` (el bundle no es escribible). Los selectores de fichero empiezan en Documentos, como hoy. | 10:33; 07:612-638 |
| DP-14 | Ubicación del proyecto | Carpeta `Free1X2.Mac/` en este repo, junto al oráculo. | PLAN §7.2 |
| DP-15 | Referencia de comportamiento | **Solo la app actual** (WinUI + `Free1X2.Domain`); toda divergencia con el WinForms legacy se resuelve a favor del port. Lista completa en `DECISIONES.md`. | README D1 |

---

## 3. Módulos (cierra H-01)

### 3.1 Estructura del repositorio

```
Free1X2.Mac/                               (carpeta en este repo; PLAN §7.2)
├─ Free1X2.xcodeproj                       app macOS (arm64), SwiftUI, escena `Window` única
├─ App/                                    capa de presentación (único sitio con SwiftUI/AppKit)
│   ├─ Arranque/      Free1X2App, AppDelegate, Sesion, Router, Comandos (menús + barra)
│   ├─ Plataforma/    adaptadores de Entorno: Portapapeles (NSPasteboard), paneles de fichero, alertas, log
│   ├─ Compartido/    controles reutilizables (10 §1.9): BoletoBase, Porcentajes, Semaforo, MatrizAnalisis…
│   └─ Pantallas/<Menú>/   una carpeta por menú actual (DP-11); Vista + ModeloPantalla por pantalla
├─ Packages/Free1X2Kit/                    Swift Package; cero AppKit/SwiftUI
│   ├─ Sources/Free1X2Base/                Columna y codificaciones, FormatoNET, Texto, AleatorioNET, Errores, Entorno, Progreso
│   ├─ Sources/Free1X2Motor/               generador, analizador, grupos, 14 filtros, CP, If-Then, tolerancias,
│   │                                      análisis, reducción, escrutinio, subir categoría
│   ├─ Sources/Free1X2Formatos/            F01–F24 (07 §1), EscritorXMLNET, ZIP `.z3q`
│   ├─ Sources/Free1X2Online/              cliente clubprogol, parsers JSON, caché
│   ├─ Sources/Free1X2CasosDeUso/          los 193 casos de uso (ESPEC_13) + Configuracion/arranque
│   ├─ Sources/Free1X2Pruebas/             dobles de prueba y cargador de golden (solo para tests)
│   ├─ Tests/<Módulo>Tests/                Swift Testing (ESPEC_14)
│   ├─ Tests/Golden/                       datos del oráculo (ESPEC_12)
│   └─ Sources/Free1X2Bench/               ejecutable de benchmarks (ESPEC_14 §6)
└─ tools/GoldenGen/  (en la raíz del repo, fuera de Free1X2.Mac)  C#, se ejecuta una vez (ESPEC_12)
```

### 3.2 Grafo de dependencias (acíclico; una flecha = «importa»)

```
App ──► Free1X2CasosDeUso ──► Free1X2Formatos ──► Free1X2Motor ──► Free1X2Base
                         ├──► Free1X2Online ───────────────────────► Free1X2Base
                         └──► Free1X2Motor
```

| Módulo | Contiene | Puede importar | Prohibido |
|--------|----------|----------------|-----------|
| `Free1X2Base` | `Columna`/`IndiceBase3` y las 5 codificaciones (06 §1, sin unificarlas), `FormatoNET` (parseo/formato .NET por `Cultura`), `TextoNET` (codificaciones, EOL, BOM), `AleatorioNET`, `ExcepcionNET`/`ErrorDominio`, protocolos de `Entorno`, `Progreso` | Foundation, `Collections` (swift-collections) | todo lo demás |
| `Free1X2Motor` | configuración (valores públicos, §4.2) + ejecución (clases internas, §4.3) de todo `MotorCalculo`, `Analisis`, `Reduccion`, `Escrutinio`, `SubirCategoria`, `Utils` de cálculo | Base | Foundation de E/S (`FileManager`, `URLSession`) |
| `Free1X2Formatos` | lectores/escritores de 07 §1 y ESPEC_16 §2-4; mapea ficheros ↔ valores del Motor | Base, Motor | red |
| `Free1X2Online` | `ClienteClubProgol`, parsers F18/F19, `CacheJornada` | Base | Motor, Formatos |
| `Free1X2CasosDeUso` | API de casos de uso (ESPEC_13), carga de `Configuracion`, carpetas de trabajo | Base, Motor, Formatos, Online | SwiftUI/AppKit |
| App | vistas, modelos de pantalla, `Sesion`, `Router`, adaptadores de plataforma | Kit (solo el producto `Free1X2Kit` = CasosDeUso + tipos públicos reexportados) | importar `Free1X2Motor` directamente |

Reglas de acceso: `public` solo en la fachada de cada módulo; `package` para lo que comparten módulos del mismo paquete
(p. ej. las clases del motor que necesitan los tests de paridad); `internal`/`private` el resto. La app solo ve tipos de
valor y funciones de casos de uso.

### 3.3 `Package.swift` (esqueleto normativo)

```swift
// swift-tools-version: 6.2
import PackageDescription

let package = Package(
    name: "Free1X2Kit",
    platforms: [.macOS(.v14)],
    products: [.library(name: "Free1X2Kit", targets: ["Free1X2CasosDeUso"])],
    dependencies: [.package(url: "https://github.com/apple/swift-collections", from: "1.1.0")],
    targets: [
        .target(name: "Free1X2Base", dependencies: [.product(name: "Collections", package: "swift-collections")]),
        .target(name: "Free1X2Motor", dependencies: ["Free1X2Base", .product(name: "Collections", package: "swift-collections")]),
        .target(name: "Free1X2Formatos", dependencies: ["Free1X2Base", "Free1X2Motor"]),
        .target(name: "Free1X2Online", dependencies: ["Free1X2Base"]),
        .target(name: "Free1X2CasosDeUso", dependencies: ["Free1X2Motor", "Free1X2Formatos", "Free1X2Online"]),
        .target(name: "Free1X2Pruebas", dependencies: ["Free1X2CasosDeUso"], path: "Sources/Free1X2Pruebas"),
        .executableTarget(name: "Free1X2Bench", dependencies: ["Free1X2CasosDeUso", "Free1X2Pruebas"]),
        .testTarget(name: "Free1X2BaseTests", dependencies: ["Free1X2Base", "Free1X2Pruebas"]),
        .testTarget(name: "Free1X2MotorTests", dependencies: ["Free1X2Motor", "Free1X2Pruebas"], resources: [.copy("../Golden")]),
        .testTarget(name: "Free1X2FormatosTests", dependencies: ["Free1X2Formatos", "Free1X2Pruebas"]),
        .testTarget(name: "Free1X2OnlineTests", dependencies: ["Free1X2Online", "Free1X2Pruebas"]),
        .testTarget(name: "Free1X2CasosDeUsoTests", dependencies: ["Free1X2CasosDeUso", "Free1X2Pruebas"]),
    ],
    swiftLanguageModes: [.v6]
)
```

Ajustes de compilación (todos los targets): modo Swift 6 (concurrencia estricta completa), `-warnings-as-errors` en CI,
`-enable-upcoming-feature ExistentialAny`, optimización `-O` + `-enforce-exclusivity=checked` en Release (no se desactiva
la comprobación de exclusividad salvo en ficheros del camino caliente medidos, §10). Dependencias de terceros: **solo**
`swift-collections` (Apple), para `OrderedDictionary`/`Deque` donde el C# depende del orden de inserción (T6). ZIP con
`Compression.framework`; XML de lectura con `Foundation.XMLDocument` (ESPEC_16).

---

## 4. Modelo de datos (cierra H-02)

### 4.1 Por qué dos capas

- Las specs exigen **estado mutable compartido** durante el cálculo: cada filtro guarda los contadores de su último
  `analizar` que leen tolerancias, `ContenedorAnalisis` y «Analizar combinación» (02 §0.3, 03 §0.2); las CP se comparten
  por referencia con las relaciones (04 §0.2); hay memos por columna (01 §1.4). Un núcleo «solo structs» rompería la paridad.
- El plan exigía «structs `Sendable`, sin clases ni existenciales» (PLAN §2, texto anterior a la fase A) para poder probar y paralelizar.
- **Solución (DT-02):** lo que se **edita, guarda y compara** es un valor; lo que **calcula** es un grafo de clases que se
  construye a partir de ese valor al empezar cada ejecución, vive dentro de una sola tarea y se descarta al terminar.
  El grafo nunca cruza dominios de aislamiento: no necesita `Sendable`, ni locks.

### 4.2 Capa de configuración (pública, `struct` `Sendable, Hashable, Codable`)

```swift
public struct Configuracion: Sendable, Hashable { … }        // 01 §3.3: boleto, puntosCP, lae, analisis (20 flags), interfaz
public struct Combinacion: Sendable, Hashable {               // lo que hoy guarda un .comb (F02) + estado de Inicio
    public var pronosticos: [String]                         // N cadenas "1,X,2" (01:22-27)
    public var equipos: [String]                             // "LOCAL - VISITANTE"
    public var grupos: [GrupoConfig]                         // grupos[0] = boleto base
    public var controles: ControlGruposConfig                // controles, conjuntos, fallos permitidos
    public var ifThen: IfThenConfig?
    public var filtroColumnas: FiltroColumnasConfig?         // ruta del archivo-filtro + recorte/relleno
}
public struct GrupoConfig: Sendable, Hashable {
    public var partidosActivos: [Bool]                       // 0-based (06:109)
    public var condiciones: CondicionesGrupo                 // las 14, en el orden fijo de Grupo.filtros (01 §4.2)
    public var tolerancias: [ToleranciaConfig]
    public var fallosPermitidosTolerancias: String           // cadena opaca, como en F02
    public var filtroParcial: String?                        // ruta del archivo del grupo
}
public struct CondicionesGrupo: Sendable, Hashable {
    public var noVariantes = CondicionNoVariantes()
    public var valoracionSignos = CondicionValoracionSignos()
    // … una propiedad por filtro, en el orden de 01 §4.2 …
    public var diferencias = CondicionDiferencias()
}
public protocol Condicion: Sendable, Hashable {
    static var tipo: TipoFiltro { get }
    var activa: Bool { get set }                             // IsActive
    var contieneDatos: Bool { get set }                      // ContieneDatos
}
```

Reglas:
- Cada `CondicionX` guarda **todo** el estado configurable del filtro C# y reproduce la semántica de sus setters de texto
  (`Set*` acumulativos, `ReinicializaValores`, `Get*` canónico; p. ej. 03 §0.4 P6) como métodos `mutating … throws(ErrorDominio)`.
  El cargador de `.comb`/condiciones (Formatos) y las pantallas usan **las mismas** funciones: un solo camino de parseo.
- Sin contadores ni cachés en esta capa: son de ejecución.
- `Equatable` sirve para tests (round-trip F02/F03) y para saber si la UI tiene cambios.

### 4.3 Capa de ejecución (interna, `final class`)

```swift
final class Analizador {                                      // 01 §2.2; una instancia por ejecución
    init(_ c: Combinacion, _ cfg: Configuracion, puntosCP: PuntosCP, archivos: ArchivosEjecucion) throws(ErrorDominio)
}
final class Grupo {                                           // 01 §2.4
    let noVariantes: FiltroNoVariantes                        // 14 propiedades tipadas, orden fijo:
    let valoracionSignos: FiltroValoracionSignos              // despacho estático en el bucle caliente (§10),
    …                                                         // sin [any FiltroColumna]
    func filtro(_ t: TipoFiltro) -> any FiltroColumna         // solo para usos por nombre (GetFiltro/IfThen/análisis)
}
protocol FiltroColumna: AnyObject {                           // contrato IFiltro (01 §4.1) con el nombre canónico
    var tipo: TipoFiltro { get }
    var activa: Bool { get }
    var analisisActivo: Bool { get }
    func analizar(_ c: Columna) -> Bool
    func analizarFallos(_ c: Columna) -> [String]?
    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int   // Diferencias lanza (03 §0.3)
}
```

- **Construcción** = el «contrato de construcción» de 01 §1.7 ejecutado dentro de `Analizador.init`, en ese orden, a partir
  de la `Combinacion`. Las referencias compartidas (CP ↔ relaciones, 04 §0.2) se cablean ahí, en el orden P24.
- **Ciclo de vida:** `init` → ejecución (ruta A/B/C/D, 01 §1.1) → resultado (valores) → el grafo se libera. Nada del grafo
  escapa al llamador; los resultados que la UI necesita (contadores, `ContenedorAnalisis`, estadísticas) se **copian a
  `struct`** antes de devolver (p. ej. `MatrizAnalisis`, 10:284).
- **Desviación conocida y aceptada (DT-02b):** el C# reutiliza el mismo `Analizador` entre cálculos (`AppState.Analizador`,
  07:1160), así que el memo de un `Grupo` no controlado (01:127, «nunca se invalida») puede arrastrarse de una ejecución a
  la siguiente. Con grafo por ejecución eso no ocurre. El oráculo (ESPEC_12) define la paridad **por ejecución desde estado
  limpio**, que es como trabajan los 138 tests y GoldenGen. Anotada en `DECISIONES.md` para revisión.
- `ContenedorAnalisis` deduplica filtros por identidad (01 §4.1, regla 4): con clases internas se mantiene `ObjectIdentifier`.

### 4.4 Tipos base

| Concepto | Tipo | Nota |
|----------|------|------|
| Columna de 3 bits | `typealias Columna = Int64` | 06:207-221. Bucles calientes pueden operar en `UInt64(bitPattern:)` (todo valor válido < 2^48). |
| Índice base-3 | `typealias IndiceBase3 = Int32` | 05 §7 (T2); distinto de `Columna`; no se mezclan. |
| Columna de texto | `String` + acceso por `utf16` | 06:216 |
| Pronóstico de un partido | `String` (`"1,X,2"`) | 01:24-27 (`"1X"` sin coma = solo `1`, [PRESERVAR]) |
| Porcentajes de pantalla | `[[Double]]` N×3 | 10:281 (en el motor, `Float` donde el C# usa `float`: T3) |

Las otras codificaciones de 06 §1.7 y 08 §1.4 (base-3 de `ConvertidorDeBases`, tritos de `Comparador`, mitades de `Comparador2`,
«X=0, MSB primero» de 5 pantallas, rangos de Formatos 123) viven en `Free1X2Base/Codificaciones/` como **tipos con nombre
propio**, una implementación cada una (sin duplicados por pantalla: 10:578,741,1002,1064,1804 → `CodificacionX0MSB`).

---

## 5. Errores (cierra H-04)

### 5.1 Política (DT-03)

**Mixta y fiel:** el núcleo **reproduce** cada excepción C# como error tipado **en el mismo punto lógico** (para poder
compararla con el oráculo); la app **muestra** el error por el mismo canal y con el mismo texto que hoy. Nunca se aborta (A5).

| Situación en C# | Swift |
|-----------------|-------|
| Excepción que el C# lanza y **un llamador captura** (p. ej. `ObtenerListaFromTxtAciertos` ⇒ lista vacía, 04 §0.5 D3) | Se replica la captura: la función devuelve lo mismo que el C#; ningún error sale. |
| Excepción que **sube a la pantalla** y esta la convierte en mensaje (`"No se pudo abrir: " + msg`, 08:118) | `throw ErrorDominio`; el modelo de pantalla hace `catch` y muestra el **mismo literal** + `mensajeUsuario`. |
| Excepción **no capturada** que hoy acaba en el manejador global (`App.UnhandledException`, 10:32) | `throw` hasta el modelo de pantalla, que lo entrega a `Alertas.errorInesperado(_:)`: «Se produjo un error inesperado:\n\n{mensaje}» + log. Mismo efecto visible, la app sigue viva. |
| `UserDialogs.ShowError/ShowInfo` dentro del motor (01 §2.23) | El motor **no** muestra nada: devuelve el aviso en el resultado (`avisos: [Aviso]`) o lo emite como evento de progreso; la app lo presenta igual. |
| Comportamiento indefinido en C# sin excepción (`(int)NaN`, 10:381) | Se fija el valor que da el C# en x64 (`Int32.min`), documentado en T13; si no es conocido, el oráculo lo fija. |
| Bucle que en C# **no termina** (09 A-73-1; 10:1341) | Se detecta la falta de progreso y se lanza `ErrorDominio(.operacionInvalida)`; en los casos que terminan, mismo resultado (DT-08). |

### 5.2 Tipos

```swift
public enum ExcepcionNET: String, Sendable, Codable {         // el TIPO de excepción .NET que el oráculo registra
    case formato = "FormatException", indiceFueraDeRango = "IndexOutOfRangeException"
    case argumentoFueraDeRango = "ArgumentOutOfRangeException", argumento = "ArgumentException"
    case argumentoNulo = "ArgumentNullException", referenciaNula = "NullReferenceException"
    case desbordamiento = "OverflowException", conversionInvalida = "InvalidCastException"
    case operacionInvalida = "InvalidOperationException", divisionPorCero = "DivideByZeroException"
    case archivoNoEncontrado = "FileNotFoundException", directorioNoEncontrado = "DirectoryNotFoundException"
    case entradaSalida = "IOException", accesoDenegado = "UnauthorizedAccessException"
    case xml = "XmlException", operacionCancelada = "OperationCanceledException"
}
public struct ErrorDominio: Error, Sendable, Equatable {
    public let excepcion: ExcepcionNET
    public let mensajeUsuario: String          // texto que hoy ve el usuario cuando el C# lo fija; si .NET lo genera, equivalente en español
    public let origen: String                  // "Free1X2.Domain/MotorCalculo/FiltroNoVariantes.cs:231" (trazabilidad, log)
}
public struct ErrorOnline: Error, Sendable, Equatable { public let mensaje: String }   // = QuinielaOnlineException (07:1123-1133)
```

- Firmas: `throws(ErrorDominio)` en Base, Motor, Formatos y CasosDeUso; `throws(ErrorOnline)` en el cliente online; los parsers
  JSON lanzan `ErrorDominio(.formato, …)` con los textos exigidos por los tests (07:1112-1118).
- Cancelación: `ErrorDominio(.operacionCancelada, …)`; nunca se muestra al usuario (como la `OperationCanceledException` silenciosa de 10:1079).
- Golden de error: `{"error":"<ExcepcionNET.rawValue>"}` (06:1015); se compara el **tipo**, no el mensaje de .NET (06 §1.8).
- Lint (CI): `fatalError`, `preconditionFailure`, `try!`, `as!` y `!` de desempaquetado prohibidos en `Sources/` salvo con comentario
  `// invariante: <por qué es inalcanzable>` revisado.

---

## 6. Concurrencia, progreso y cancelación (cierra H-05)

### 6.1 Aislamiento

| Pieza | Aislamiento | Por qué |
|-------|-------------|---------|
| Vistas, modelos de pantalla, `Sesion`, `Router`, alertas | `@MainActor` | estado de UI |
| Casos de uso largos (cálculo, reducción, escrutinio, análisis, herramientas de fichero) | `@concurrent nonisolated func … async throws(ErrorDominio)` | corren fuera del hilo principal; reciben **valores** y devuelven **valores** |
| Grafo del motor (§4.3) | confinado a la tarea que lo crea | no `Sendable`, sin locks |
| `ClienteClubProgol` | `actor` (instancia en `Entorno`, no `static shared`) | guarda anti-doble-petición en memoria (07:750-763) |
| `Registro` (log) | `final class` `Sendable` con `OSAllocatedUnfairLock` | escrituras desde cualquier tarea, nunca lanza (07:1166-1173) |

### 6.2 Contrato único de operación larga

```swift
public struct Progreso: Sendable, Equatable {
    public var procesadas: Int; public var total: Int?; public var aceptadas: Int?
    public var fase: String?                     // texto de estado que hoy muestra la pantalla, si lo hay
}
public struct Avisos: Sendable { … }            // mensajes no fatales que hoy salen por UserDialogs

// Forma de TODO caso de uso largo:
@concurrent public func calcularColumnas(
    _ entrada: EntradaCalculo,                   // valores (Combinacion, Configuracion, rutas…)
    entorno: Entorno,
    progreso: ReportadorProgreso                 // no-op por defecto; la UI pasa uno ligado a un AsyncStream
) async throws(ErrorDominio) -> ResultadoCalculo
```

- **Progreso:** `ReportadorProgreso` (`Sendable`) envuelve un `AsyncStream<Progreso>.Continuation` y **limita la frecuencia**
  (como mucho cada 100 ms o cada 65 536 columnas, lo que ocurra después). El bucle caliente solo incrementa un entero local.
  Sustituye `UiPump`/`IProgressNotifier` (T12) y los temporizadores que hoy sondean contadores sin sincronizar (10:808, 10:828).
- **Cancelación cooperativa:** comprobación de `Task.isCancelled` cada 4 096 iteraciones del bucle externo; al detectarla,
  `throw ErrorDominio(.operacionCancelada)` **o**, donde el C# deja salida parcial (05:836, A9), se cierra el fichero con lo
  escrito hasta ese punto y se devuelve el resultado parcial, igual que hoy (DP-09/DP-10). Donde el C# no es cancelable
  (p. ej. aidomnou, 10:1976 A-16) no se comprueba (DP-10); activar la cancelación es cambiar un parámetro, sin rediseño.
- **Salida a fichero durante el cálculo** (ruta B, reductores): escritura secuencial en la propia tarea con búfer de 64 KiB;
  se crea el fichero al escribir la primera línea, como el C# (05 §1.2).

### 6.3 Paralelismo (DT-04)

Solo donde **no cambia el resultado**, con orden de salida preservado (PLAN §2):

- **Generación en modo pronóstico (rutas A/B/C):** el espacio de columnas se parte por prefijos del DFS (01 §2.1) en `P`
  particiones (`P` = núcleos de rendimiento); cada partición construye **su propio** `Analizador` (grafo independiente) y
  produce contadores + columnas aceptadas; se concatenan en orden de partición. Válido porque el memo solo sirve dentro de una
  columna (01:120-123). **No** se particiona si hay archivo-filtro (riesgo de memo obsoleta entre columnas, 01:124-127) ni si un
  If-Then usa un grupo no controlado; en esos casos, una sola tarea (paridad).
- **Reductores, escrutinio y herramientas de fichero:** secuenciales salvo que su ficha de ESPEC_13 diga lo contrario con golden.
- Contenedores de análisis (rutas C/D): por partición y fusión en orden; la fusión debe reproducir el orden de inserción (T6).

### 6.4 Memoria

Conjuntos de 3^N columnas como bitset `[UInt64]` (3^14 → 598 KB; 3^15 → 1,8 MB), no `[Bool]` ni `BitArray` (10:1079);
limpiar solo lo marcado en lugar de `SetAll(false)` por columna (10:1079). Arrays de structs contiguos para tablas de
millones de entradas (10:532: 57 MB → `ContiguousArray` de structs de 12 bytes sin padding).

---

## 7. Inyección de dependencias (DT-05)

Inyección **por parámetro** de un único valor `Entorno`; sin contenedor ni framework.

```swift
public struct Entorno: Sendable {
    public var reloj: any Reloj                     // ahora(), monotónico; «Tiempo» de ≥6 pantallas, semillas de reloj (08:604)
    public var aleatorio: @Sendable (Int32) -> AleatorioNET   // System.Random(semilla) de .NET (ESPEC_16 §5)
    public var archivos: any SistemaArchivos        // leer/escribir/atómico/listar/borrar/existe/crearCarpeta/mtime
    public var rutas: RutasApp                      // carpeta base y las 10 carpetas (DP-13), bundle de solo lectura
    public var red: any ClienteHTTP                 // data(for:) → (Data, HTTPURLResponse); URLSession en producción
    public var portapapeles: any Portapapeles       // leer/escribir texto (01 §2.23)
    public var registro: any Registro               // log (07:1166)
    public var cultura: Cultura                     // DP-01: cultura «del sistema» para C3–C13
}
extension Entorno { public static func produccion() -> Entorno }        // en Free1X2CasosDeUso
// Free1X2Pruebas: Entorno.pruebas(reloj: .fijo(…), archivos: .enMemoria(…), red: .grabada(…), cultura: .esES)
```

Reglas: el bucle caliente no toca `Entorno` (se lee antes de empezar); cada protocolo tiene exactamente dos implementaciones
(producción + doble de prueba), lo que justifica el protocolo; `Entorno` se pasa a los casos de uso, nunca a las clases del motor
(que reciben valores ya leídos: ficheros cargados a `[Columna]`/`[String]`, puntos CP, N).

---

## 8. Estado de la app y presentación

### 8.1 `Sesion` (definición única; sustituye a 01:760, 07:1162, 08:41-43 y 10:46)

```swift
@MainActor @Observable public final class Sesion {           // ≡ AppState (07 §6.15), en el target App
    public let configuracion: Configuracion                  // DP-04: leída una vez al arrancar
    public var combinacion: Combinacion                      // la combinación en pantalla (≡ AppState.Analizador)
    public var grupoPantalla: Int                            // getter `grupoActual` reajusta a 0 fuera de rango (07:1162)
    public var rutaCombinacion: URL?                         // NombreArchivoComb
    public var jornadaActual: JornadaQuiniela?
    public let entorno: Entorno
    public func nuevaCombinacion()                           // 10:74; no toca jornadaActual
}
```

Los eventos `Cambiado`/`JornadaCambiada` desaparecen: `@Observable` notifica a las vistas que leen cada propiedad. Los
«handoffs» estáticos (08 §1.3, 10:64) se sustituyen por payload de navegación (§8.2) o por el valor devuelto por una hoja.

### 8.2 Navegación y comandos (DP-11)

```swift
enum Destino: Hashable { case inicio, calcularColumnas, condicion(TipoFiltro, grupo: Int), visorAnalisis(MatrizAnalisisID), … }   // 108 + inicio
@MainActor @Observable final class Router { var pila: [Destino] = [] ; func navegar(_:) ; func navegarSiempre(_:) ; func atras() }
enum Comando { case nuevaCombinacion, abrirCombinacion, guardarCombinacion, guardarComo, abrirEquipos, guardarEquipos,
               borrarTemporales, borrarInformes, navegar(Destino) }                         // = AccionInicio + menús (10:68-81)
```

- `navegar` no apila duplicados; `navegarSiempre` sí (10:61). `Esc`/`⌘[` = atrás.
- Menús, barra y atajos emiten `Comando`; un único despachador en `Sesion`/`Router` los ejecuta (equivale a `NavegarConAccion`, 10:65).
- Smoke: `Destino.allCases` (108 + inicio) recorrido por un UI test y por el argumento `-smoke` (10:265-272).

### 8.3 Modelo de pantalla

```swift
@MainActor @Observable final class CalcularColumnasModelo {   // uno por pantalla; nombre = <Pantalla>Modelo
    init(sesion: Sesion, router: Router)
    // estado editable de la pantalla (valores), estado de ejecución (progreso, ocupado), textos ya formateados
    func calcular() async                                     // llama al caso de uso; nunca contiene lógica de negocio
}
```

- Toda la lógica de los ViewModels WinUI (los 193 CU) va a `Free1X2CasosDeUso`; el modelo de pantalla solo traduce
  controles ↔ valores, lanza el caso de uso y presenta resultados/errores (con los literales de hoy).
- Formateo de números para mostrar: en el caso de uso (con `FormatoNET`), para que sea probable sin UI.
- **Editor de condición** (sustituye 08:123 y 10:308; ahora coherente con `Grupo` como valor):

```swift
@MainActor protocol EditorCondicion: AnyObject, Observable {
    associatedtype C: Condicion
    var borrador: C { get set }
    func aceptar(en grupo: inout GrupoConfig) throws(ErrorDominio)   // patrón «Aceptar» de 10:297
    func estadisticas() async throws(ErrorDominio) -> [Estadistica] // filtro temporal + Ganadoras/ (10:298)
}
```
  Una vista genérica `CondicionPantalla<E: EditorCondicion>` aporta los 8 botones (10:289); Guardar/Abrir/Copiar/Pegar usan
  `Formatos` y el **fichero temporal** `Temp/tmp.<ext>` como hoy (DP-10; 10:295).

### 8.4 Alertas, ficheros, log

- `Alertas` (`@MainActor @Observable`): cola FIFO de `error`/`info` mostrada de una en una (10:229-231) y `confirmar(_:) async -> Bool`
  (sin ventana ⇒ `false`, 10:232).
- Selectores: `NSOpenPanel`/`NSSavePanel` envueltos en `SelectorFicheros` (async), carpeta inicial Documentos, extensiones como hoy (10:237-239).
- Log: `os.Logger` (subsistema `com.free1x2.app`) + fichero `~/Library/Logs/Free1X2/log.txt` con el formato y la rotación de 07:1166-1173.

---

## 9. Persistencia, rutas y datos de usuario (cierra H-14)

| Dato | Dónde | Regla |
|------|-------|-------|
| Carpeta base (DP-13) | `~/Library/Application Support/Free1X2/` | Al arrancar: crear las 10 carpetas de 10:33 si faltan (fallo ⇒ solo log). |
| Semillas editables (`parametros.free1x2`, `ColumnasProbables.xml`, `Equipos/*.dat`, `Jornadas/InfoJornadasLAE.txt`, `Impresion/imprebol.cfg`) | carpeta base | Copia desde `Resources/Datos/` **solo si no existe**; nunca se sobrescribe un fichero del usuario. |
| Recursos de solo lectura (`Impresion/impresoras.cfg`, `Documentacion/*.txt`, logo) | bundle | `Bundle.main` vía `RutasApp`. |
| `Ganadoras/` | carpeta base | Hoy se crea vacía y el usuario la llena a mano (07 §3.1); igual. Menú «Mostrar carpeta de datos» **no** se añade (DP-10). |
| Caché de jornada | carpeta base, `jornada-{es,mx}.json` | Escritura atómica (07:1135-1143). |
| Tema, idioma | `UserDefaults` | No son formatos de intercambio (07 §3.2); mismas opciones y defectos (DP-12). |
| Log | `~/Library/Logs/Free1X2/` | 07 §3.2. |

**Ficheros que vienen de Windows** (un usuario que copia su carpeta de la app actual): se leen exactamente como los lee hoy el
C# (DP-02): CRLF/LF/CR, BOM, `.dat` Latin-1, coma decimal según cultura (DP-01). Rutas absolutas de Windows dentro de `.comb`/`.lst`
(07 §3.1, «Rutas absolutas»): se conservan tal cual al leer y al reescribir (como hoy); si no existen, el semáforo queda rojo como hoy cuando falta el
fichero (07 §3.1; `File.Exists`), sin heurísticas nuevas. No hay asistente de migración: basta copiar la carpeta de datos a la
carpeta base (instrucción para el manual de usuario Mac).

---

## 10. Rendimiento (DT-09)

Objetivo y medición en ESPEC_14 §6. Reglas de construcción del camino caliente (generador → analizador → grupos → filtros):

1. Despacho estático: `Grupo` con 14 propiedades tipadas y llamadas directas en el orden fijo; nada de `[any FiltroColumna]`,
   closures ni genéricos no especializados por columna. `final class` + `@inline(__always)` en `analizar` pequeños.
2. Cero asignaciones por columna: búferes y contadores preasignados en `init`; `ContiguousArray`/tuplas de tamaño fijo;
   bitsets `UInt32`/`UInt64` para conjuntos de índices ≤ 64 (02 §0.9, punto 8).
3. Aritmética de bits con operadores con máscara (`&<<`, `&>>`, `&+`) donde el C# envuelve (T1) — además son más rápidos.
4. Sin `String` en el bucle: las columnas viajan como `Columna`; el texto se genera solo al escribir (ruta B) con una tabla precalculada.
5. E/S con búfer (64 KiB) y lectura del archivo-filtro completa a `[Columna]` antes del bucle.
6. Exclusividad dinámica: se desactiva (`-enforce-exclusivity=unchecked`) **solo** en los ficheros del bucle caliente y solo si el
   benchmark lo justifica y los golden siguen idénticos.
7. Paralelismo de §6.3.

---

## 11. Convenciones de código (DT-12)

- Nombres: términos de dominio en español (ESPEC_00 §4); tipos `UpperCamelCase`, miembros `lowerCamelCase`; nombre canónico del GLOSARIO.
  Un tipo C# portado conserva su nombre (salvo choque: `Analisis.Analizador` → `AnalizadorProbabilidades`, 06:42).
- Un fichero por tipo principal; ≤ 600 líneas (los filtros grandes se parten en extensiones por responsabilidad: config, análisis, texto).
- Cada función portada lleva un comentario de una línea con su origen: `// C#: Free1X2.Domain/MotorCalculo/Grupo.cs:165-225`
  (trazabilidad R4; facilita la revisión contra el oráculo). Las peculiaridades replicadas se marcan `// PRESERVAR: <motivo>`.
- Formato: `swift-format` (configuración en el repo) aplicado en CI; sin reglas propias adicionales.
- Literales visibles por el usuario: en el código del caso de uso o pantalla tal como hoy (con dobles espacios y tildes, 04 §0.7 S12);
  solo Créditos pasa por `.xcstrings` (DP-07).

---

## 12. Plataforma y herramientas (DT-10, DT-11)

| Tema | Decisión | Motivo |
|------|----------|--------|
| macOS mínimo | **14 Sonoma** (PLAN §2) | `@Observable`; corre en el M1 del dueño. Nada que exija macOS 15+ (p. ej. `Synchronization.Mutex` → `OSAllocatedUnfairLock`). |
| Arquitectura | `arm64` (x86_64 opcional sin coste: no hay código específico) | PLAN §2 |
| Swift / Xcode | Swift 6.2+ (Xcode 26+), modo de lenguaje 6 | concurrencia estricta, `throws(E)`, `@concurrent` |
| Pruebas | Swift Testing (`@Test`, `#expect`) + XCTest solo para UI tests | ESPEC_14 |
| CI | GitHub Actions, runner macOS arm64: `swift build`, `swift test`, `xcodebuild test` (UI smoke), lint, benchmarks en modo comparación | PLAN §3; ESPEC_14 §7 |
| Terceros | solo `swift-collections` | §3.3 |

---

## 13. Trazabilidad

| Hallazgo | Dónde se cierra |
|----------|-----------------|
| H-01 arquitectura | §1–§12 |
| H-02 modelo de datos | §4 (DT-02); PLAN §2 corregido |
| H-03 nombres | `GLOSARIO.md` (+ §3-§8 de aquí usan los canónicos) |
| H-04 errores | §5 |
| H-05 concurrencia | §6 |
| H-11 casos de uso | forma común §6.2 y §8.3; tabla completa en ESPEC_13 |
| H-13 pruebas | §7 (inyección); resto en ESPEC_14 |
| H-14 datos de usuario | §9 |
| D4, D5, D7, D8, D3, D2 | DP-01, DP-02, DP-11, DP-06, DP-08, DP-09 |
