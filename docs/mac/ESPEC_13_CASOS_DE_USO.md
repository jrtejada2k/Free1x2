# ESPEC 13 — API de los casos de uso (`Free1X2CasosDeUso`)

> Cierra H-11 de [`REVISION_SPECS.md`](REVISION_SPECS.md). Forma común en [ESPEC_11](ESPEC_11_ARQUITECTURA_SWIFT.md) §6.2 y §8.3;
> oráculo O1/O2/O3 en [ESPEC_12](ESPEC_12_ORACULO.md) §5. El **comportamiento** de cada CU (pseudocódigo, mensajes, bordes) sigue en la
> sección de su pantalla (ESPEC_08/09/10); aquí se fija su **contrato Swift**.

## 1. Plantilla de firma

Cinco formas; cada CU usa exactamente una (columna «T» de §4).

```swift
// P — pura y síncrona: valores → valores, sin E/S
public static func traducirColumnaARangos(_ columna: String, valoracion: [[Double]]) throws(ErrorDominio) -> String

// N — presentación: formatea resultados para mostrar (texto ya formateado con FormatoNET; probable sin UI)
public static func formatearMatrizDibujos(_ m: MatrizEstadistica, modo: ModoMatriz, cultura: Cultura) -> MatrizTexto

// F — fichero corto: lee/escribe con Entorno.archivos, sin progreso
public static func guardarPuntosCP(_ p: PuntosCP, entorno: Entorno) async throws(ErrorDominio)

// L — larga: fuera del hilo principal, progreso limitado y cancelación según la pantalla (DP-10)
@concurrent public static func reducirColumnas(_ e: EntradaReduccion, entorno: Entorno,
                                               progreso: ReportadorProgreso = .ninguno) async throws(ErrorDominio) -> ResultadoReduccion

// E — edición de configuración (valores de ESPEC_11 §4.2): transforma el borrador de una pantalla
public static func aplicarCondicion<C: Condicion>(_ borrador: C, en grupo: inout GrupoConfig) throws(ErrorDominio)
```

Reglas:
1. **Entradas y salidas son valores** (`Sendable`, `Equatable`); rutas como `URL`; nunca `Sesion`, vistas ni modelos de pantalla.
   Más de 3 parámetros ⇒ `struct <Nombre>Entrada`; salida compuesta ⇒ `struct <Nombre>Resultado`.
2. **Mensajes** que hoy la pantalla muestra como resultado van en el `Resultado` (`avisos`); los de error, en `ErrorDominio.mensajeUsuario`
   con el literal de la spec (ESPEC_11 §5.1). El caso de uso nunca presenta nada.
3. **Configuración** necesaria (N, LAE, flags de análisis, puntos CP) llega como parámetro (`Configuracion` o el campo concreto), no global.
4. **Cancelación** (solo L): la función la admite siempre; la pantalla la conecta solo donde hoy funciona (DP-10). Si el C# deja salida parcial al
   cancelar, el CU también (05:836).
5. **Nombres**: `verboObjeto` en `lowerCamelCase`, agrupados en un `enum` por menú (`CasosCombinacion.calcularCombinacion`). Sustituye los tres
   estilos de 08/09/10 (H-03); el nombre original de la spec se conserva en la tabla para trazabilidad.

Dependencias de `Entorno` (columna «Dep.»): **A** archivos/rutas · **R** reloj · **Z** aleatorio · **H** red · **C** cultura del sistema (DP-01) · **K** portapapeles.

## 2. Organización (un `enum` por menú actual, DP-11)

| `enum` | Menú / origen | Fichero(s) |
|--------|---------------|------------|
| `CasosInicio` | Inicio, barra «Archivo», arranque (10 §1.1-1.2) | `Inicio.swift` |
| `CasosAjustes` | Free1x2 ▸ Configuración, Configurar análisis; Cambiar puntuación | `Ajustes.swift` |
| `CasosArchivo` | Archivo ▸ Gestión de equipos, Importar/exportar; Obtener boletos online | `Archivo.swift` |
| `CasosVer` | Ver ▸ boletos, gráfico, estadísticas, listado de condiciones; visores | `Ver.swift` |
| `CasosCombinacion` | Combinación ▸ calcular, reducir, escrutar, analizar, imprimir, P15; barra Combinación | `Combinacion/*.swift` |
| `CasosCondiciones` | rejilla de condiciones de Inicio, CP, control de grupos, tolerancias, If-Then, figuras, valoraciones | `Condiciones/*.swift` |
| `CasosFiltros` | Filtros ▸ combinar, diferencias, coincidencias, aidomnou, Pim | `Filtros/*.swift` |
| `CasosOperaciones` | Operaciones ▸ álgebra, transposición, multiplicador, fraccionador, rotación | `Operaciones.swift` |
| `CasosUtilidades` | Utilidades ▸ (17 entradas) y herramientas de la barra Utilidades | `Utilidades/*.swift` |

## 3. Unificaciones (una implementación para lo que hoy está repetido; cierra la parte de H-10 que toca a los CU)

| Repetición actual | Implementación única | CU afectados |
|-------------------|----------------------|--------------|
| Patrón de pantalla de condición (Aceptar · Guardar/Copiar · Abrir/Pegar · Estadísticas) repetido en 14 condiciones | `aplicarCondicion`, `exportarCondicion`, `importarCondicion`, `estadisticasDeCondicion` genéricos sobre `Condicion` (+ editor específico solo para la conversión pantalla ↔ condición) | A67-A71, A90, B02, B03, B18, B19, B30, B37, C02-C04, C10, C30, C31, C45 |
| Motor de 6 columnas de aidomnou reimplementado en Filtro Pim (09 A-57-1; 10 A-23) | `MotorAidomnou` (Motor) usado por ambos con sus parámetros | B24, C58-C61 |
| Carga y navegación de columnas ganadoras | `cargarColumnasGanadoras(_:)` (Formatos F04) | B17, C62 y las estadísticas de condición sobre `Ganadoras/` |
| Codificación «X=0, MSB primero» en 9 pantallas (08 §1.4; 10:578,741,1002,1064,1804) | `CodificacionX0MSB` (Base, ESPEC_16 §6) | A04-A05, A09-A11, A38-A41, A53-A55 (pantallas 4, 6, 17, 21: 08 §1.4), C09, C14-C16, C24-C25, C28-C29, C58-C61 |
| Quicksort propio (10:523, 10:885, 10:1297) | `ordenarComoQuicksortCSharp` (Base, ESPEC_16 §6) | C05-C08, C20-C21, C36 |
| `TransformarValoracion` doble (09:2058 A-51-1) y escritura de valoraciones doble (09:2070 A-63-1) | `Porcentajes.transformar` y `EscritorValoraciones` (Formatos F07) | B18, B31, B32, C44 |
| `CU-C03` ≡ `CU-A71`, `CU-C04` ≡ `CU-A69` (mismo patrón documentado dos veces) | alias de la función genérica | C03, C04 |

## 4. Catálogo (T = forma §1; Or. = oráculo ESPEC_12 §5)

### 4.1 `CasosInicio` (nuevos IDs `CU-Snn`: lógica de `MainPageViewModel`/`App`, sin ID en 08-10)

| ID | Función Swift | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|------:|------------------|---|------|-----|
| CU-S01 | `arrancar` | shell | `Entorno` → `EstadoInicial` (crea las 10 carpetas, copia semillas, carga `Configuracion` todo-o-nada, jornada desde caché) | F | A | O1 |
| CU-S02 | `nuevaCombinacion` | Inicio | `Configuracion` → `Combinacion` (N triples, equipos «? - ?») | P | — | O3 |
| CU-S03 | `abrirCombinacion` | Inicio | `URL` → `CombinacionAbierta` (combinación, equipos, ruta del filtro, aviso de error con el literal de 10:75) | F | A | O1 |
| CU-S04 | `guardarCombinacion` | Inicio | `Combinacion`, equipos, `URL` → () | F | A | O1 |
| CU-S05 | `abrirEquipos` | Inicio | `URL`, N → `[String]` (faltantes «? - ?») | F | A | O2 |
| CU-S06 | `guardarEquipos` | Inicio | `[String]`, `URL` → () (UTF-8 con BOM, CRLF) | F | A | O2 |
| CU-S07 | `borrarTemporales` | Inicio | `Entorno` → nº borrados (`*_tmp.comb` de `Temp/`) | F | A | O3 |
| CU-S08 | `borrarInformes` | Inicio | `Entorno` → nº borrados (`Informe*` de `Informes/`) | F | A | O3 |
| CU-S09 | `estadoDeCondiciones` | Inicio | `GrupoConfig`, `ControlGruposConfig`, `IfThenConfig?` → semáforo de las 17 entradas de la rejilla (10:304-306) | P | — | O3 |
| CU-S10 | `guardarBarrasVisibles` / `leerBarrasVisibles` | shell | 6 `Bool` ↔ `parametros.free1x2` (orden de 10:166-167) | F | A | O1 |

### 4.2 `CasosAjustes`

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A36 | `leerPuntosCP` | `LeerPuntosCP` | 16 | → `PuntosCP` | F | A | O1 |
| CU-A37 | `guardarPuntosCP` | `GuardarPuntosCP` | 16 | `PuntosCP` → () | F | A | O1 |
| CU-A62 | `leerOpcionesAnalisis` | `LeerOpcionesAnalisis` | 24 | → `OpcionesAnalisis` (20 flags) | F | A | O1 |
| CU-A63 | `guardarOpcionesAnalisis` | `GuardarOpcionesAnalisis` | 24 | `OpcionesAnalisis` → () (sin recarga, DP-04) | F | A | O1 |
| CU-A64 | `leerConfiguracionEditable` | `LeerConfiguracion` | 25 | → `ConfiguracionEditable` | F | A | O1 |
| CU-A65 | `guardarConfiguracion` | `GuardarConfiguracion` | 25 | `ConfiguracionEditable` → () | F | A | O2 |
| CU-A66 | `validarSeparadorJB` | `ValidarSeparadorJB` | 25 | 6 límites → () o error con el literal | P | — | O2 |

### 4.3 `CasosArchivo`

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A02 | `anadirEquipo` | `AnadirEquipo` | 2 | nombre, categoría → () (`.dat` Latin-1) | F | A | O2 |
| CU-B28 | `cargarCatalogoEquipos` · `guardarCatalogoEquipos` · `importarCatalogoOnline` | `CatalogoEquiposLocal` | 60 | categoría ↔ `[String]`; país → `CatalogoEquipos` (alta/mover/eliminar = operaciones de lista en el modelo de pantalla) | F | A, H | O2 |
| CU-B34 | `convertirColumnas` · `contarColumnas` | `ConversorColumnas` | 66 | `URL` CSV↔TXT → nº de columnas | L | A | O2 |
| CU-A78 | `actualizarJornadaOnline` | `ActualizarJornadaOnline` | 32 | país → `ResultadoJornada` (desde caché o red; 07 §5.3) | F | A, H, R | O1 |
| CU-A79 | `resolverPaisInicial` | `ResolverPaisInicial` | 32 | → `OpcionPais` | F | A | O1 |

### 4.4 `CasosVer`

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A25 | `prepararBoletos` | `PrepararBoletos` | 11 | `URL`, orden, tipo → `[Boleto]` (8 apuestas) | F | A | O2 |
| CU-A26 | `ordenarColumnasDeBoleto` | `OrdenarColumnasPorCriterio` | 11 | `[String]`, `OrdenarMatriz`, `TipoOrden` → `[String]` | P | — | O2 |
| CU-C49 | `cargarYOrdenarBoletos` | `CargarYOrdenarBoletos` | 100 | `URL`, orden → parámetros para A25 (reutiliza A26) | F | A | O2 |
| CU-C48 | `formatearBoletosEnTexto` | `FormatearBoletosEnTexto` | 99 | `[Boleto]` → `String` | N | C | O2 |
| CU-B29 | `calcularGraficoCombinacion` | `GraficoCombinacion` | 61 | columnas, opciones → `DatosGrafico` (líneas, rellenos, mín./máx., guía) | P | — | O2 |
| CU-A15 | `calcularEstadisticasDeFichero` | `CalcularEstadisticasDeFichero` | 9 | `URL`, tipo (4) → `MatrizEstadistica` 15×15 | L | A | O2 |
| CU-A88 | `formatearMatrizDibujos` | `FormatearMatrizDibujos` | 38 | matriz, modo → `MatrizTexto` | N | C | O2 |
| CU-A89 | `formatearMatrizCoincidencias` | `FormatearMatrizCoincidencias` | 39 | matriz 5×15, modo → `MatrizTexto` | N | C | O2 |
| CU-C32 | `formatearMatrizEstadistica` | `FormatearMatrizEstadistica` | 90 | matriz 5×14 / 4×15, modo → `MatrizTexto` | N | C | O2 |
| CU-B39 | `construirListadoCondiciones` · `exportarListadoCondiciones` | `ListadoCondiciones` | 71 | `Combinacion` → árbol; árbol, formato (texto/HTML), `URL` → () | F | A | O2 |
| CU-C53 | `construirSeccionesDeAnalisis` | `ConstruirSeccionesDeAnalisis` | 102 | `MatrizAnalisis` → `[SeccionAnalisis]` | N | C | O2 |
| CU-C54 | `refrescarVisorAnalisis` | `RefrescarVisorAnalisis` | 102 | secciones, filtro de vista → secciones visibles | N | — | O2 |
| CU-C55 | `estadisticasDeCumplimiento` | `EstadisticasDeCumplimiento` | 103 | `[Estadistica]` → filas con `Float.ToString()` (T14) | N | C | O2 |
| CU-C56 | `prepararFilasEstadisticas` | `PrepararFilasEstadisticas` | 103 | `[Estadistica]` → filas | N | C | O2 |
| CU-C57 | `filasDeGrupoDePremios` | `MostrarGrupoDePremios` | 104 | resultado de C13, grupo → filas | N | C | O2 |
| CU-C50 | `resumenGlobalAbdon` | `ResumenGlobalAbdon` | 101 | informe ABDON → texto | N | C | O2 |
| CU-C51 | `agrupacionesAbdon` | `AgrupacionesAbdon` | 101 | informe ABDON → agrupaciones | P | — | O2 |
| CU-C52 | `generarCondicionColumnasProbables` | `GenerarCondicionColumnasProbables` | 101 | informe ABDON → `CondicionColProbables` | P | — | O2 |

### 4.5 `CasosCombinacion`

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A28 | `calcularCombinacion` | `CalcularCombinacion` | 13 | `EntradaCalculo` (combinación, configuración, modo contar/analizar/grabar) → `ResultadoCalculo` | L | A | O1 |
| CU-A29 | `validarArchivoResultados` | `ValidarArchivoResultados` | 13 | `URL?`, `Combinacion` → () o error literal | P | — | O2 |
| CU-A30 | `estimarColumnasMaximas` | `EstimarColumnasMaximas` | 13 | `Combinacion` → `Int64` | F | A | O2 |
| CU-A31 | `estadisticasDeCalculo` | `EstadisticasDeCalculo` | 13 | `Progreso`, LAE → textos (%, importes) | N | C | O2 |
| CU-A32 | `calcularLoteDeCombinaciones` | `CalcularLoteDeCombinaciones` | 14 | `[URL]`, carpeta destino → `[ResumenLote]` | L | A | O1 |
| CU-A33 | `nombreSalidaDeCombinacion` | `NombreSalidaDeCombinacion` | 14 | `URL`, carpeta → `URL` | P | — | O2 |
| CU-C18 | `validarFicheroEntradaReductor` | `ValidarFicheroDeEntradaReductor` | 82 | `URL` → () o error (14 signos exactos) | F | A | O2 |
| CU-C19 | `reducirColumnas` | `ReducirColumnas` | 82 | `EntradaReduccion` (algoritmo: JDC «menos tiempo» · JDCDobleContador «menos columnas» · Redu1305Xfsf «grandes archivos»; nivel, maxCol, percent, externas) → `ResultadoReduccion` | L | A | O1 |
| CU-B07 | `escrutarColumnas` | `EscrutinioColumnas` | 45 | `EntradaEscrutinio` (columnas, ganadora o histórico) → `ResultadoEscrutinio` | L | A | O1 |
| CU-B08 | `columnasPremiadasPorBoleto` | `ColumnasPremiadas` | 45 | resultado → filas (`ceil(n/8)` boletos) | P | — | O2 |
| CU-A52 | `exportarColumnasPremiadas` | `ExportarColumnasPremiadas` | 20 | filas, selección, `URL` → () | F | A | O2 |
| CU-B04 | `escrutarCombinaciones` | `EscrutinioCombinaciones` | 44 | modo (simple / contra fichero / contra jornadas) → filas | L | A | O1 |
| CU-B05 | `cargarHistoricoJornadas` | `HistoricoJornadas` | 44 | `URL` → histórico + temporadas | F | A | O2 |
| CU-B06 | `resolverPlantillaJornadas` | `PlantillaJornadas` | 44 | plantilla, dígitos → `[ArchivoJornada]` | F | A | O2 |
| CU-A12 | `analizarFallosCombinacion` | `AnalizarFallosCombinacion` | 7 | columna, `Combinacion` → árbol Verde/Ámbar/Rojo | P | — | O2 |
| CU-A13 | `evaluarPronosticosBase` | `EvaluarPronosticosBase` | 7 | columna, pronósticos → partidos fallados | P | — | O2 |
| CU-A43 | `prepararAnalisisDeFallos` | `PrepararAnalisisDeFallos` | 18 | texto de ganadora, `Combinacion` → entrada de A12 o error | P | — | O2 |
| CU-C17 | `calcularProbabilidadDePremios` | `ProbabilidadDePremios` | 81 | madre, valoración → tabla | L | A | O1 |
| CU-A01 | `agregarPleno15` | `AgregarPleno15` | 1 | `URL` entrada, modo (4), `URL` salida → () | L | A | O2 |
| CU-A14 | `analizarFicheroDeColumnas` | `AnalizarFicheroDeColumnas` | 8 | `URL`, N, combinación → `MatrizAnalisis` (ruta D) | L | A | O1 |
| CU-C42 | `analizarSignosDeFichero` | `AnalizarSignosDeFichero` | 97 | `URL` → `AnalisisSignos` | L | A | O2 |
| CU-C43 | `grabarSignos` | `GrabarSignos` | 97 | análisis, selección, `URL` → () | F | A | O2 |
| CU-A04 | `analizarGruposEnFichero` | `AnalizarGruposEnFichero` | 4 | `URL`, N partidos, fallos → filas por índice base 3 | L | A | O2 |
| CU-A05 | `grabarGruposSeleccionados` | `GrabarGruposSeleccionados` | 4 | `URL`, grupos marcados, `URL` salida → () | L | A | O2 |
| CU-B22 | `generarReduccionPerfecta` | `ReduccionPerfecta` | 55 | tipo (4/13/11 triples, 7/15 dobles), `URL` → () | F | A | O2 |
| CU-B21 | `proponerCoeficientesDependencia` · `aplicarDependenciaLineal` | `DependenciaLineal` | 54 | columnas → coeficientes; coeficientes, partido, `URL` → () | L | A | O2 |
| CU-B36 | `calcularMarcasBoleto` · `leerImprebol` · `guardarImprebol` · `rangoPorDefecto` | `ImpresionBoleto` | 68 | columnas, config. de impresión → marcas por boleto | F | A | O2 |
| CU-B38 | `cargarImpresorasConocidas` · `buscarImpresora` | `ImpresorasConocidas` | 70 | bundle → catálogo; modelo → impresora | F | A | O2 |

### 4.6 `CasosCondiciones`

Funciones genéricas del patrón de condición (§3):

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A68 | `aplicarCondicion` | `AplicarCondicion` | 26 (y todas) | borrador `C`, `inout GrupoConfig` (reglas `activa`/`contieneDatos` de 10:297) | E | — | O2 |
| CU-A69 | `exportarCondicion` (≡ CU-C04) | `ExportarCondicion` / `GuardarCondicion` | 26, 75 | `C`, `URL` (incluye `Temp/tmp.<ext>` de Copiar) → () | F | A | O1 |
| CU-A70 | `importarCondicion` | `ImportarCondicion` | 26 (y todas) | `URL` → `C` | F | A | O1 |
| CU-A71 | `estadisticasDeCondicion` (≡ CU-C03) | `CalcularEstadisticasCondicion` / `EstadisticasDeFiltro` | 26, 75 | `C`, carpeta `Ganadoras/` → `[Estadistica]` | L | A | O1 |
| CU-C03 | alias de `estadisticasDeCondicion` | `EstadisticasDeFiltro` | 75 | = CU-A71 | L | A | O1 |
| CU-C04 | alias de `exportarCondicion` | `GuardarCondicion` | 75 | = CU-A69 | F | A | O1 |

Editores específicos (conversión pantalla ↔ `Condicion`; puros):

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A67 | `EditorContactos.condicion(desde:)` / `.textos(de:)` | `EditarCondicionContactos` | 26 | 10 parejas + figuras ↔ `CondicionContactos` | E | — | O2 |
| CU-A90 | `EditorDibujos…` | `EditarCondicionDibujos` | 40 | rejilla triangular ↔ `CondicionDibujos` | E | — | O2 |
| CU-B02 | `EditorDiferencias…` | `CondicionDiferencias` | 42 | textos, conjuntos, atajos ↔ `CondicionDiferencias` | E | — | O2 |
| CU-B03 | `EditorDistancias…` | `CondicionDistancias` | 43 | 4 textos (vacío ⇒ 0..14) ↔ `CondicionDistancias` | E | — | O2 |
| CU-B18 | `EditorFormatos123…` · `traducirColumnasFormatos123` | `CondicionFormatos123` | 51 | formatos, aciertos, valoración ↔ `CondicionFormatos123` | E | — | O2 |
| CU-B19 | `EditorFormatosSignos…` | `CondicionFormatosSignos` | 52 | relaciones de formatos ↔ `CondicionFormatosSignos` | E | — | O2 |
| CU-B30 | `EditorGruposEquipos…` | `CondicionGruposEquipos` | 62 | grupos y relaciones ↔ `CondicionGruposEquipos` | E | — | O2 |
| CU-B37 | `EditorInterrupciones…` | `CondicionInterrupciones` | 69 | 10 textos ↔ `CondicionInterrupciones` | E | — | O2 |
| CU-C02 | `EditorNoVariantes…` | `AplicarCondicionNoVariantes` | 75 | textos ↔ `CondicionNoVariantes` | E | — | O2 |
| CU-C10 | `EditorPesosNumericos…` | `AplicarCondicionPesosNumericos` | 78 | textos ↔ `CondicionPesosNumericos` | E | — | O2 |
| CU-C30 | `EditorSignosSeguidos…` | `AplicarCondicionSignosSeguidos` | 88 | textos ↔ `CondicionSignosSeguidos` | E | — | O2 |
| CU-C31 | `EditorSimetrias…` | `AplicarCondicionSimetrias` | 89 | textos ↔ `CondicionSimetrias` | E | — | O2 |
| CU-C45 | `EditorValoracion…` | `AplicarCondicionValoracion` | 98 | valores 1/X/2, tipo ↔ `CondicionValoracionSignos` | E | — | O2 |
| CU-C44 | `prepararValoracion` | `PrepararValoracion` | 98 | matriz → mín.-máx. y rangos | P | C | O2 |
| CU-C46 | `valorarColumna` | `ValorarColumna` | 98 | columna, condición → resultado `"r"` (efecto sobre la condición del grupo como hoy, 10:1513) | E | C | O2 |
| CU-C47 | `columnasBase` | `ColumnasBase` | 98 | valoración → columnas base | P | — | O2 |
| CU-B13 | `parsearFiguras` · `formatearFiguras` · `leerFicheroFiguras` | `Figuras` | 49 | texto ↔ `[Int64]` (nibbles, 06:203) | P | A | O1 |
| CU-B14 | `aplicarCasillasFiguras` | `EditorFiguras` | 49 | casillas, `[Int64]` → `[Int64]` | E | — | O2 |
| CU-A44 | `EditorColumnasProbables…` (cargar, guardar CP en edición, confirmar) | `EditarColumnasProbables` | 19 | buffer ↔ `CondicionColProbables` | E | — | O2 |
| CU-A45 | `navegarColumnaProbable` | `NavegarColumnaProbable` | 19 | buffer, índice → buffer (añade CP vacía si hace falta) | E | — | O2 |
| CU-A46 | `copiarRangosAColumnas` | `CopiarRangosAColumnas` | 19, 30 | CP actual, desde, hasta → CP | E | — | O2 |
| CU-A47 | `fusionarColumnasImportadas` | `FusionarColumnasImportadas` | 19 | CP, importadas, modo (sustituir/añadir/reemplazar) → CP | E | — | O2 |
| CU-A48 | `EditorRelacionesCP1…` | `EditarRelacionesCP1` | 19 | ↔ `[RelacionCP1Config]` | E | — | O2 |
| CU-A49 | `EditorRelacionesCP2…` | `EditarRelacionesCP2` | 19 | ↔ `[RelacionCP2Config]` | E | — | O2 |
| CU-A50 | `EditorRelacionesCP3…` | `EditarRelacionesCP3` | 19 | ↔ `[RelacionCP3Config]` | E | — | O2 |
| CU-A51 | `EditorControlFallosCP…` | `EditarControlFallosCP` | 19 | ↔ `ControlFallosCPConfig` | E | — | O2 |
| CU-A75 | `copiarColumnasProbablesAGrupos` | `CopiarColumnasProbablesAGrupos` | 29 | CP seleccionadas, grupos destino, `inout Combinacion` | E | — | O2 |
| CU-A76 | `parsearIndices` | `ParsearIndices` | 29 | texto → `[Int]` | P | — | O2 |
| CU-B12 | `exportarColumnasProbables` | `ColumnasProbables.exportar` | 48 | CP, con/sin aciertos, `URL` (`.clm`) → () | F | A | O2 |
| CU-B35 | `importarColumnasProbables` | `ImportadorCP` | 67 | `URL` → CP simples o con estadísticas | F | A | O2 |
| CU-A72 | `EditorControlGrupos…` · `calcularLibres` | `EditarControlGrupos` | 27 | ↔ `ControlGruposConfig` | E | — | O2 |
| CU-A73 | `EditorControlConjuntos…` | `EditarControlConjuntos` | 27 | ↔ `ControlGruposConfig.conjuntos` | E | — | O2 |
| CU-A74 | `EditorTolerancias…` | `EditarTolerancias` | 28 | filas + fallos ↔ `[ToleranciaConfig]` | E | — | O2 |
| CU-A77 | `crearGrupos` | `CrearGrupos` | 31 | n, `inout Combinacion` | E | — | O2 |
| CU-B33 | `EditorIfThen…` · `leerIfThen` · `guardarIfThen` | `IfThenEditor` | 65 | textos ↔ `IfThenConfig`; `.if` | E | A | O1 |
| CU-B31 | `guardarValoracion` | `Valoraciones.guardar` | 63 | matriz, formato (1/3/42), `URL` → () | F | A, C | O1 |
| CU-B32 | `guardarEnHistoricoValoraciones` · `detectarFormatoValoracion` | `HistoricoValoraciones` | 64 | jornada, matriz, `URL` → () | F | A, C | O1 |

### 4.7 `CasosFiltros`

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A53 | `combinarFiltrosContar` | `CombinarFiltrosContar` | 21 | lista de filtros de fichero → conteos | L | A | O2 |
| CU-A54 | `sumarColumnasPorRango` | `SumarColumnasPorRango` | 21 | lista, rango → suma | L | A | O2 |
| CU-A55 | `grabarColumnasPorRango` | `GrabarColumnasPorRango` | 21 | lista, rango, `URL` → () | L | A | O2 |
| CU-A56 | `guardarListaFiltros` | `GuardarListaFiltros` | 21 | lista, `URL` (`.lst`) → () | F | A | O2 |
| CU-A57 | `cargarListaFiltros` | `CargarListaFiltros` | 21 | `URL` → lista | F | A | O2 |
| CU-A80 | `diferenciaEntreFiltros` | `DiferenciaEntreFiltros` | 33 | base, filtros, máscara, mínimo, `URL` → nº | L | A | O2 |
| CU-A81 | `escrutarGanadoraSobreResultado` | `EscrutarGanadoraSobreResultado` | 33 | ganadora, `URL` → (14, 13) | F | A | O2 |
| CU-A82 | `guardarListaDiferencias` | `GuardarListaDiferencias` | 33 | lista, `URL` (`.lst` con `;`) → () | F | A | O2 |
| CU-A83 | `cargarListaDiferencias` | `CargarListaDiferencias` | 33 | `URL` → lista | F | A | O2 |
| CU-A38 | `calcularRadiografiaColumna` | `CalcularRadiografiaColumna` | 17 | columna → 108 características | P | — | O2 |
| CU-A39 | `parsearCondicionCoincidencias` | `ParsearCondicionCoincidencias` | 17 | filas `grupos;rango` → condiciones (máx. 100) | P | — | O2 |
| CU-A40 | `filtrarPorCoincidencias` | `FiltrarPorCoincidencias` | 17 | `URL`, ganadora anterior, condiciones → selección | L | A | O2 |
| CU-A41 | `analizarCoincidenciasEntreGanadoras` | `AnalizarCoincidenciasEntreGanadoras` | 17 | ganadoras (máx. 3000) → distribución | L | A | O2 |
| CU-A42 | `exportarDistribuciones` | `ExportarDistribuciones` | 17 | resultados, `URL` → () (TSV) | F | A, C | O2 |
| CU-C58 | `generarSeisColumnasProbables` | `GenerarSeisColumnasProbables` | 105 | valoración → 6 CP (`MotorAidomnou`) | P | — | O2 |
| CU-C59 | `parsearRangosAidomnou` | `ParsearRangosAidomnou` | 105 | texto → rangos | P | — | O2 |
| CU-C60 | `filtrarFicheroAidomnou` | `FiltrarFicheroAidomnou` | 105 | `URL`, CP, rangos, `URL` salida → nº | L | A | O2 |
| CU-C61 | `analizarColumnaContraSeis` | `AnalizarColumnaContraSeis` | 105 | columna, CP → filas | P | — | O2 |
| CU-C62 | `cargarColumnasGanadoras` | `CargarGanadorasYNavegar` | 105 | `URL` → `[String]` (§3) | F | A | O2 |
| CU-B24 | `filtroPim` | `FiltroPim` | 57 | parámetros Pim → resultado (`MotorAidomnou`, 09 A-57-1) | L | A | O2 |
| CU-B25 | `guardarRangosPim` · `leerRangosPim` | `RangosPim` | 57 | rangos ↔ `.cnd` | F | A | O2 |

### 4.8 `CasosOperaciones`

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-A03 | `algebraDeColumnas` | `AlgebraDeColumnas` | 3 | operación, `URL` A, `URL` B, `URL` salida → nº | L | A | O2 |
| CU-C40 | `transponerColumnas` | `TransponerColumnas` | 95 | `URL`, transposición, `URL` salida → () | L | A | O2 |
| CU-C01 | `multiplicarColumnas` | `MultiplicarColumnas` | 74 | `URL`s, `URL` salida → nº | L | A | O2 |
| CU-B20 | `fraccionarColumnas` | `FraccionarColumnas` | 53 | `URL`, modo (cuotas/tramos), carpeta → `[URL]` | L | A | O2 |
| CU-C22 | `rotarSignosDeColumnas` | `RotarSignosDeColumnas` | 85 | `URL`, rotación, `URL` salida → () | L | A | O2 |

### 4.9 `CasosUtilidades`

| ID | Función Swift | Nombre en spec | Pant. | Entrada → Salida | T | Dep. | Or. |
|----|---------------|----------------|------:|------------------|---|------|-----|
| CU-C33 | `prepararSubirCategoria` | `PrepararSubirCategoria` | 92 | textos → parámetros | P | — | O2 |
| CU-C34 | `calcularSubirCategoria` | `CalcularSubirCategoria` | 92 | parámetros → `NoColumnas` + columnas | L | A | O1 |
| CU-C35 | `grabarSubirCategoria` | `GrabarSubirCategoria` | 92 | columnas, `URL` → () | F | A | O1 |
| CU-B41 | `leerColumnasYFrecuencias` · `modificarPorcentajes` · `exportarModificadas` | `ModificadorPorcentajes` | 73 | `URL`, partido, %, modo → columnas | L | A | O2 |
| CU-B27 | `generarFicherosCP` · `importarPorcentajesCP` | `GeneradorCP` | 59 | valoración, configuración (F05) → ficheros F06 | F | A | O1 |
| CU-A60 | `leerConfiguracionGeneradorCP` | `LeerConfiguracionGeneradorCP` | 23 | tipo → filas de «Configuracion de CPs» (`ColumnasProbables.xml`, F05) | F | A | O1 |
| CU-A61 | `guardarConfiguracionGeneradorCP` | `GuardarConfiguracionGeneradorCP` | 23 | tipo, filas (Desde, Hasta, Num*, Forzar*) → () | F | A | O1 |
| CU-B23 | `calcularColumnaGEPT` | `ColumnaGEPT` | 56 | dos columnas → columna GEPT + tabla | P | — | O2 |
| CU-B01 | `columnasPorDiferencias` | `ColumnasPorDiferencias` | 41 | origen (triples o `URL`), rangos → `URL` salida | L | A | O2 |
| CU-C05 | `valorCentralDeOrdenacion` | `ValorCentralDeOrdenacion` | 76 | valoración → valor central | P | — | O2 |
| CU-C06 | `ordenarPorProductosOSumas` | `OrdenarPorProductosOSumas` | 76 | `URL`, valoración, criterio → columnas ordenadas | L | A | O2 |
| CU-C07 | `grabarColumnasOrdenadas` | `GrabarColumnasOrdenadas` | 76 | columnas, `URL` → () | F | A | O2 |
| CU-C08 | `ordenacionMultiple` | `OrdenacionMultiple` | 76 | criterios → columnas | L | A | O2 |
| CU-C23 | `prepararTablasJM` | `PrepararTablasJM` | 86 | valoración → tablas | P | — | O2 |
| CU-C24 | `seleccionJMPorCorte` | `SeleccionJMPorCorte` | 86 | tablas, corte, `URL` → () | L | A | O2 |
| CU-C25 | `seleccionJMPorValor` | `SeleccionJMPorValor` | 86 | tablas, valor, `URL` → () | L | A | O2 |
| CU-C26 | `ubicarGanadoraJM` | `UbicarGanadoraJM` | 86 | ganadora, tablas → posición | P | — | O2 |
| CU-C27 | `condicionesJM` | `CondicionesJM` | 86 | textos ↔ condiciones | P | — | O2 |
| CU-C28 | `distribucionDeVecinosMS` | `DistribucionDeVecinosMS` | 87 | parámetros → distribución | L | — | O2 |
| CU-C29 | `analizarDistribucionMS` | `AnalizarDistribucionMS` | 87 | `URL`, parámetros → análisis | L | A | O2 |
| CU-C20 | `valorarColumnaRentabilidad` | `ValorarColumnaRentabilidad` | 83 | columna, valoración, LAE → ficha | P | C | O2 |
| CU-C21 | `calcularRentabilidad` | `CalcularRentabilidad` | 83 | `URL`, valoración, LAE → esperanza matemática | L | A, C | O2 |
| CU-C36 | `tramificarColumnas` | `TramificarColumnas` | 93 | `URL`, valoración, tramos → resultado | L | A | O2 |
| CU-C37 | `filtrarPorLimites` | `FiltrarPorLimites` | 93 | resultado, límites (A87) → columnas | L | A | O2 |
| CU-C38 | `guardarJornadaLAE` | `GuardarJornadaLAE` | 93 | jornada → `InfoJornadasLAE.txt` (F08) | F | A, C | O1 |
| CU-C39 | `puntosDeCurvaDeTramos` | `PuntosDeCurvaDeTramos` | 94 | resultado → `[CurvaGrafico]` | P | — | O2 |
| CU-A84 | `leerTemporadasDeValoraciones` | `LeerTemporadasDeValoraciones` | 34 | `URL` (TSV 44 col.) → temporadas | F | A | O2 |
| CU-A85 | `validarFicheroDeColumnas` | `ValidarFicheroDeColumnas` | 34 | `URL` → () o error | F | A | O2 |
| CU-A86 | `gestionarListaCombinacionesPorJornada` | `GestionarListaCombinacionesPorJornada` | 34 | lista ↔ `.lst` | F | A | O2 |
| CU-A87 | `filtrarTramosPorLimites` | `FiltrarTramosPorLimites` | 35 | extremos 10×4, tramos → columnas | P | — | O2 |
| CU-C14 | `frecuenciasDePremiadas` | `FrecuenciasDePremiadas` | 80 | `URL`, ganadoras → frecuencias | L | A | O2 |
| CU-C15 | `jornadasDeFrecuencia` | `JornadasDeFrecuencia` | 80 | frecuencias, filtro → jornadas | P | — | O2 |
| CU-C16 | `grabarColumnasConFrecuencia` | `GrabarColumnasConFrecuencia` | 80 | columnas, `URL` → () | F | A | O2 |
| CU-B09 | `estimarPremios` | `EstimadorPremios` | 46 | %, columna, apuestas, recaudación, bote → premios | P | C | O2 |
| CU-B10 | `cargarHistoricoLAE` · `guardarHistoricoLAE` | `HistoricoLAE` | 46 | jornada ↔ F08 | F | A, C | O1 |
| CU-A16 | `valoracionRealDesdeApostada` | `ObtenerValoracionRealDesdeApostada` | 10 | % apostados, nº columnas → matriz «reales» | P | — | O2 |
| CU-A17 | `generarColumnasAleatorias14` | `GenerarColumnasAleatorias14` | 10 | n, «reales» → columnas, media y desviación LN | L | Z, R | O3 |
| CU-A18 | `premiosJornadaVirtual` | `CalcularPremiosJornadaVirtual` | 10 | columna, «reales» → premios 14..10 | P | — | O2 |
| CU-A19 | `escrutarCombinacionBanco` | `EscrutarCombinacionBanco` | 10 | `EntradaBanco` → resultado | L | A, Z, R | O3 |
| CU-A20 | `escrutarColumnasBanco` | `EscrutarColumnasBanco` | 10 | `EntradaBanco` → resultado por columna | L | A, Z, R | O3 |
| CU-A21 | `autoescrutinioBanco` | `AutoescrutinioBanco` | 10 | `URL` → resultado | L | A | O2 |
| CU-A22 | `escrutarPorJornadasBanco` | `EscrutarPorJornadas` | 10 | `EntradaBanco` → saldo por jornada | L | A, Z, R | O3 |
| CU-A23 | `simplificarFilasPorDiferencias` | `SimplificarFilasPorDiferencias` | 10 | filas, referencia, dif → selección | P | — | O2 |
| CU-A24 | `grabarColumnasSeleccionadas` | `GrabarColumnasSeleccionadas` | 10, 36 | filas, rango, tope, solo selec., `URL` → () | F | A | O2 |
| CU-A58 | `comprimirColumnasAZ3q` | `ComprimirColumnasAZ3q` | 22 | `URL` `.txt` → `URL` `.z3q` | L | A | O1 |
| CU-A59 | `descomprimirZ3qAColumnas` | `DescomprimirZ3qAColumnas` | 22 | `URL` `.z3q` → `URL` `.txt` | L | A | O1 |
| CU-B11 | `emparejarReducidas` · `escrutarEstuCol` · `informesAbdon` | `EstuCol` | 47 | reducidas, ganadoras → informes ABDON | L | A | O2 |
| CU-C11 | `cargarColumnasJugadas` | `CargarColumnasJugadas` | 79 | `URL` → columnas | F | A | O2 |
| CU-C12 | `escrutarPosiblesPremios` | `EscrutarPosiblesPremios` | 79 | columnas, ganadoras posibles → escrutinio | L | A | O2 |
| CU-C13 | `calcularPosiblesPremios` | `CalcularPosiblesPremios` | 79 | escrutinio → contenedores ordenados (T5) | L | — | O2 |
| CU-A09 | `clasificarColumnasPorSumaPares` | `ClasificarColumnasPorSumaPares` | 6 | `URL` → 36 casillas | L | A | O2 |
| CU-A10 | `grabarCasillasSuma` | `GrabarCasillasSuma` | 6 | casillas marcadas, `URL` → () | F | A | O2 |
| CU-A11 | `contarPremiosPorCasillas` | `ContarPremiosPorCasillas` | 6 | ganadora, casillas → premios 14..10 | L | — | O2 |
| CU-B15 | `separarPorcentajesJB` | `SeparadorPorcentajesJB` | 50 | bandas, `URL`, ganadora → signos por banda + exportación | L | A | O2 |
| CU-B16 | `guardarRangosJB` · `leerRangosJB` | `RangosJB` | 50 | rangos ↔ `.jb7` | F | A | O2 |
| CU-B17 | `cargarColumnasGanadoras` (≡ C62) | `GanadorasNavegador` | 50 | `URL` → `[String]` | F | A | O2 |
| CU-B40 | `mejoresOpciones` | `MejoresOpciones` | 72 | parámetros → resumen ordenado y formateado | L | A, C | O2 |
| CU-B26 | `cpsPorDiferencias` | `CPsPorDiferencias` | 58 | dos columnas, diferencias, tipo, `URL` → () | L | A | O2 |
| CU-C09 | `filtrarColumnasPorParejas` | `FiltrarColumnasPorParejas` | 77 | `URL`, condiciones, `URL` salida → nº | L | A | O2 |
| CU-C41 | `filtrarColumnasPorTrios` | `FiltrarColumnasPorTrios` | 96 | `URL`, condiciones, `URL` salida → nº | L | A | O2 |
| CU-A06 | `traducirColumnaARangos` | `TraducirColumnaARangos` | 5 | columna, valoración → texto | P | — | O2 |
| CU-A07 | `contarAparicionesFormato123` | `ContarAparicionesFormato123` | 5 | `URL`, formatos → cuentas | L | A | O2 |
| CU-A08 | `listarTodosFormatos123` | `ListarTodosFormatos123` | 5 | `URL` → subsecuencias con cuenta | L | A | O2 |
| CU-A34 | `extraerFormatosDeColumna` | `ExtraerFormatosDeColumna` | 15 | columna → pares, tríos, cuartetos, quintetos, contactos | P | — | O2 |
| CU-A35 | `formatearInformeFormatos` | `FormatearInformeFormatos` | 15 | formatos → texto | N | — | O2 |
| CU-A27 | `buscarLimitesValoracion` | `BuscarLimitesValoracion` | 12 | columna base, rango de aciertos, valoración → mín./máx. de suma y producto (recorre 3^N) | L | — | O2 |

## 5. Recuento y cobertura

| Serie | CU | O1 | O2 | O3 |
|-------|---:|---:|---:|---:|
| A (08) | 90 | 17 | 69 | 4 |
| B (09) | 41 | 8 | 33 | 0 |
| C (10) | 62 | 7 | 55 | 0 |
| **Total spec** | **193** | **32** | **157** | **4** |
| S (shell, nuevos) | 10 | 4 | 2 | 4 |

- O3 solo en CU con azar sembrado por el reloj (A17, A19, A20, A22: 08 A19) y en cuatro acciones triviales del shell (S02, S07, S08, S09); ninguno escribe ficheros
  con cálculo masivo sin propiedad que lo cubra (ESPEC_12 §5).
- Si la prueba de viabilidad O2 (ESPEC_12 §7 paso 3) descarta un ViewModel, sus CU pasan a O3 **con registro en `DECISIONES.md`**
  (motivo + propiedades que los cubren).
- Cada CU tiene al menos un test N4 (ESPEC_14) aunque su oráculo sea O1/O2.
