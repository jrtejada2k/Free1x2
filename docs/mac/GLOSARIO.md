# Glosario — nombres canónicos, términos e identificadores

> Cierra H-03 de [`REVISION_SPECS.md`](REVISION_SPECS.md). Si un nombre de ESPEC_01…10 no coincide con la columna
> «Canónico», **manda el canónico** (ESPEC_11 §0). Citas `NN:línea` = `mac/ESPEC_NN_*.md:línea`.

## 1. Nombres Swift canónicos

| Concepto | Canónico | Variantes sustituidas | Definición |
|----------|----------|-----------------------|------------|
| Columna empaquetada (3 bits/partido) | `typealias Columna = Int64` | `long`, `ColumnaBits` (05:807), alias de 05:810 | ESPEC_11 §4.4 |
| Índice base-3 | `typealias IndiceBase3 = Int32` | `IndiceColumna` (05:807) | ESPEC_11 §4.4 |
| Identificador de filtro (enum C# `Filtro`) | `enum TipoFiltro: Int, CaseIterable` con `var id: String` (= nombre C#: `NoInterrupciones`, …) | `enum Filtro` (01:652, 07:983), `enum NombreFiltro: String` (02:63) | valores 0…13 de 01 §2.20 |
| Contrato de filtro en ejecución (`IFiltro`) | `protocol FiltroColumna: AnyObject` (interno) | `FiltroProtocol` (01:777) | ESPEC_11 §4.3 |
| Configuración de un filtro (lo que se guarda en F03) | `struct Condicion<Nombre>` (p. ej. `CondicionNoVariantes`) que cumple `protocol Condicion` | `F*Data`, «buffer» de pantalla | ESPEC_11 §4.2 |
| Las 14 condiciones de un grupo | `struct CondicionesGrupo` | `Grupo.filtros` como lista | ESPEC_11 §4.2 |
| Grupo (configuración / ejecución) | `GrupoConfig` (valor) / `Grupo` (clase interna) | `Grupo` como valor `inout` (08:123, 10:308) y como clase (01/03/04) | ESPEC_11 §4.2-4.3 |
| Combinación en pantalla (lo que guarda un `.comb`) | `struct Combinacion` | `AppState.Analizador` como estado de UI | ESPEC_11 §4.2 |
| Motor de una ejecución | `final class Analizador` (interna, `Free1X2Motor`) | — | 01 §2.2 |
| `Free1X2.Analisis.Analizador` | `AnalizadorProbabilidades` | `Analizador (Análisis)` | 06:42 |
| Configuración global (`VariablesGlobales`) | `struct Configuracion` | `Configuracion`+`RepositorioConfiguracion` (08:41-43) | 01 §3.3; ESPEC_11 §4.2 |
| Estado de la app (`AppState`) | `@MainActor @Observable final class Sesion` (target App) | `final class Sesion` de motor (01:760), `@State var sesion` (10:46), `AppState` (07:1162) | ESPEC_11 §8.1 |
| Dependencias externas | `struct Entorno` | `AnfitrionUI` (01:675,762), `Avisador` (10:261), `RutasApp` como protocolo (01:658,763) | ESPEC_11 §7 |
| Errores del dominio | `struct ErrorDominio` + `enum ExcepcionNET` | `ErrorColumnas` (06:220), `ErrorFormato` (07:1111), «sin tipo» (01:873) | ESPEC_11 §5.2 |
| Errores online | `struct ErrorOnline` | `enum ErrorOnline { case mensaje }` (07:837) | ESPEC_11 §5.2 |
| `obtenNoAciertosTolerancias` | `func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int` | con/sin `throws` (01:786, 02:64) | ESPEC_11 §4.3 |
| Progreso de operación larga | `struct Progreso` + `ReportadorProgreso` | `AsyncStream<Progreso>` suelto, `IProgressNotifier`, `UiPump` | ESPEC_11 §6.2 |
| Editor de una pantalla de condición | `protocol EditorCondicion` (App) | dos versiones (08:123, 10:308) | ESPEC_11 §8.3 |
| Modelo de una pantalla (ViewModel) | `<Pantalla>Modelo` (`@MainActor @Observable`) | `*ViewModel` | ESPEC_11 §8.3 |
| Destino de navegación | `enum Destino: Hashable, CaseIterable`-like (108 + inicio) | `enum Pantalla` (10:271) | ESPEC_11 §8.2 |
| Cliente online | `actor ClienteClubProgol` (instancia en `Entorno`) | `actor QuinielaOnlineService.shared` (07:816) | ESPEC_11 §6.1 |
| Caché de jornada | `CacheJornada` | `JornadaCache` | 07 §6.11 |
| Parseo/formato .NET | `FormatoNET` + `struct Cultura` | `parsearEs`, `formatearDouble`, `formatearFloatNET` sueltos | ESPEC_16 §1 |
| Escritor XML compatible | `EscritorXMLNET` | `DocumentoXML` (REVISION H-10) | ESPEC_16 §2 |
| `System.Random(semilla)` | `struct AleatorioNET` | «RNG inyectable» | ESPEC_16 §5 |
| Codificación «X=0, primer partido = dígito más significativo» (`S14N/N14S`…) | `CodificacionX0MSB` (un único helper) | copias en 9 pantallas (08 §1.4; 10:578,741,1002,1064,1804) | ESPEC_16 §6 |
| Quicksort propio de pantallas | `ordenarComoQuicksortCSharp` (una implementación) | 3 copias (10:523, 10:885, 10:1297) | ESPEC_16 §6 |

## 2. Identificadores de los documentos

| Prefijo | Qué identifica | Ámbito / dónde |
|---------|----------------|----------------|
| `H-nn` | Hallazgo de la revisión de specs | `REVISION_SPECS.md` |
| `Dn` | Decisión pendiente del índice M0 (D1…D8) | `README.md`; todas cerradas en `DECISIONES.md` |
| `DP-nn` / `DT-nn` | Decisión de producto (= app actual) / técnica | ESPEC_11 §2 y `DECISIONES.md` |
| `DEC-nnn` | Entrada del registro único de decisiones (fusiona las «Abiertas») | `DECISIONES.md` |
| `CU-Ann` · `CU-Bnn` · `CU-Cnn` · `CU-Snn` | Caso de uso de las pantallas 1–40 · 41–73 · 74–108 · Inicio y arranque (shell, definidos en ESPEC_13 §4.1) | ESPEC_08 · 09 · 10; firmas en ESPEC_13. **`CU-nn` sin prefijo de ESPEC_10 = `CU-Cnn`** (renombrado) |
| `Tn` | Trampa C#→Swift (catálogo único T1–T14) | ESPEC_00 §3 (incluye la equivalencia de B1–B7, S1–S12 y la tabla T de 05) |
| `Pn` | Peculiaridad crítica de CP y relaciones | 04 §0.6 |
| `Cn` | Punto dependiente de cultura | 07 §2.1 |
| `Fnn` | Familia de formato de fichero | 07 §0.1 |
| `A…` / `A-…` / `G-…` | «Abiertas» locales de cada spec (sin prefijo global) | sección final de cada ESPEC; su ID global es `DEC-nnn` |
| `G-…`, `GE-…`, `GR-…`, `GS-…`, `COLS-A` | Casos golden propuestos | 01 §6, 05 §6, 02 §0.10; formato común en ESPEC_12 |
| Pantalla `n` | Número de pantalla por orden alfabético de ViewModel (1–105) + 3 sin ViewModel | ESPEC_08/09/10 |

## 3. Abreviaturas de rutas usadas en las specs

| Alias | Ruta | Usado en |
|-------|------|----------|
| `MC/` | `Free1X2.Domain/MotorCalculo/` | 01, 04 |
| `DOM/`, `D:` | `Free1X2.Domain/` | 01, 08, 10 |
| `UT/`, `U/` | `Free1X2.Domain/Utils/` | 01, 04, 06 |
| `ES/` | `Free1X2.Domain/EntradaSalida/` | 01, 07 |
| `AN/`, `A/` | `Free1X2.Domain/Analisis/` | 01, 06 |
| `WIN/`, `P:`, `VM/` | `Free1X2.WinUI/Views/Ported/` | 01, 08, 09, 10, 07 |
| `W:` | `Free1X2.WinUI/` | 08, 10 |
| `V:`, `VW/` | `Free1X2.WinUI/Views/` | 08, 07 |
| `SV/` | `Free1X2.WinUI/Services/` | 07 |
| `L:` | `Free1X2/` (WinForms legacy; **no** es referencia, D1) | 08 |
| `TST/`, `TS/` | `Free1X2.Domain.Tests/` | 01, 07 |
| `DT/` | `Free1X2.WinUI/Datos/` | 07 |
| Alias de 2–4 letras (`PN:`, `INT:`, `CON:`, `SEG:`, `IF:`, `ACT`, `Conv.cs`…) | definidos al principio de cada spec | 02 §0.1, 05 |

## 4. Términos del dominio

| Término | Significado | Ref. |
|---------|-------------|------|
| **Partido** / **N** | Cada uno de los encuentros del boleto; `N = NumeroPartidos` (14 por defecto; hasta 16 en el generador) | 06:158-178 |
| **Signo** | Resultado de un partido: `1` (local), `X` (empate), `2` (visitante). Bits `100`, `010`, `001` | 06:67-78 |
| **Columna** / **apuesta** | Un signo por partido (apuesta simple) | 06:85-90 |
| **Pronóstico** | Signos admitidos por partido (fijo = 1 signo, doble = 2, triple = 3), como cadena `"1,X,2"` | 01:22-27 |
| **Boleto base** | Los pronósticos de los N partidos: define el universo de columnas (3^N con todo triples) | 10:278 |
| **Combinación** | Boleto base + grupos + condiciones + controles + If-Then (+ archivo-filtro); se guarda en `.comb` (F02) | 07 F02 |
| **Grupo** | Subconjunto de partidos con sus propias 14 condiciones; el grupo 0 es el boleto base | 01 §2.4 |
| **Condición** / **filtro** | Regla que acepta o rechaza una columna (14 tipos: Variantes X y 2, Signos seguidos, Dibujos, Interrupciones, Grupos de equipos, Formatos, Simetrías, Pesos numéricos, Valoración, Columnas probables, Distancias, Contactos, Formatos 123, Diferencias). «Condición» = la configuración; «filtro» = el objeto que la evalúa | 01 §4.2 |
| **Control de grupos** / **conjunto** / **fallos permitidos** | Combinan los veredictos de varios grupos admitiendo k fallos | 01 §1.2, §2.6-2.8 |
| **Tolerancia** (letra A–Z) | Marca en un valor de condición que permite aceptar la columna si el nº de «aciertos con letra» está en un rango | 02 §0.5, 01 §2.9 |
| **If-Then** | Condición «si se cumple X, entonces debe cumplirse Y» entre condiciones o grupos | 01 §2.16 |
| **Archivo-filtro** | Fichero de columnas que sustituye al boleto como entrada (recorte/relleno) | 01 §1.1 |
| **Filtro parcial** | Fichero de columnas asociado a un grupo | 01 §1.2 |
| **Columna probable (CP)** / **relaciones CP1/CP2/CP3/GE1** | Columna de referencia con puntos por acierto; las relaciones combinan sus aciertos | 04 §0.1 |
| **Figura** | Patrón de valores de una condición codificado en nibbles `"x-y-z"` | 06:203 |
| **Valoración** / **porcentajes** | Probabilidades 1/X/2 por partido (matriz N×3) | 06 §2.7 |
| **LAE** | Datos de Loterías y Apuestas del Estado: precio, % al pleno, recaudación | 01:61-64 |
| **Pleno al 15** | 15.º partido; mismo alfabeto 1/X/2 en las columnas | 06:180-195 |
| **Ganadora** | Columna con los resultados reales | 05 §4 |
| **Escrutinio** | Contar aciertos de cada columna contra la ganadora y asignar premios/categorías | 05 §4 |
| **Reducción** / **madre** / **nivel** / **reductora** | Elegir un subconjunto (reductoras) que garantice `nivel` aciertos para toda columna de la madre | 05 §1.4 |
| **Jornada** | Partidos de una semana; se descarga de clubprogol.com (F18) | 07 §5 |
| **Golden** / **oráculo** | Salidas de referencia generadas con el C# actual que el Swift debe igualar | ESPEC_12 |
