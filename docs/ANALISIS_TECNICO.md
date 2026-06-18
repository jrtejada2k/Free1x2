*> Nota: este es el análisis PREVIO a la migración (monolito WinForms y riesgos). Para la arquitectura WinUI 3 ya construida ver [ANALISIS_TECNICO_WINUI3.md](ANALISIS_TECNICO_WINUI3.md).*

# Free1X2 — Análisis técnico de arquitectura

*Análisis estático del repositorio (rama `winui3-migration`). App de escritorio Windows para análisis de la Quiniela (14 partidos, signos 1/X/2). Generado por el agente de análisis de código.*

## 1. Arquitectura y estructura

### Proyectos de la solución

`Free1X2.sln` referencia **solo 4 de los 6** proyectos `.csproj` del repo:

| Proyecto | En .sln | TFM | Rol | Métricas (.cs sin bin/obj) |
|---|---|---|---|---|
| **Free1X2** | Sí | `net8.0-windows` (WinForms) | App monolítica legacy: UI + todo el dominio | **361 archivos / ~171.400 líneas** |
| **Free1X2.Domain** | Sí | `net8.0` (sin UI) | Dominio extraído (Fase 0 migración) | 9 archivos / 984 líneas |
| **Free1X2.Domain.Tests** | Sí | `net8.0` (xUnit) | Golden-master de la red de seguridad | 2 archivos / 78 líneas |
| **Free1X2.WinUI** | Sí | `net8.0-windows10.0.19041.0` (WinUI 3 / WinAppSDK 1.6) | Nueva UI (scaffold) | 6 archivos / 205 líneas |
| **Free1X2.Shared** | **No** | `net8.0` | Lógica compartida (ex-WebAPI) | 12 archivos / 1.936 líneas |
| **Free1X2.WebAPI** | **No** | `net8.0` (ASP.NET) | Backend móvil | 5 archivos / 808 líneas |

**Hallazgo:** `Free1X2.Shared` y `Free1X2.WebAPI` están **huérfanos de la solución** (no se compilan ni se referencian). Vestigios de un backend móvil previo. Candidatos a archivar.

### Dependencias entre proyectos
```
Free1X2 (WinForms) ───────► Free1X2.Domain ◄─────── Free1X2.WinUI
Free1X2.Domain.Tests ─────► Free1X2.Domain
Free1X2.WebAPI ───────────► Free1X2.Shared       (ambos fuera de la .sln)
```
El dominio real sigue **dentro de `Free1X2/`** (WinForms); `Free1X2.Domain` solo contiene piezas ya extraídas (enums + utils puros). El monolito es la fuente de verdad.

### Carpetas clave de `Free1X2/`

| Carpeta | Qué hace |
|---|---|
| `UI/` | ~115 Forms + 53 UserControls. Subcarpetas `Filtros/`, `Estadisticas/`, `Controls/`, `Modern/` (tema de la modernización previa). |
| `MotorCalculo/` | Núcleo: `Analizador`, `GeneradorColumnas`, 14 filtros `IFiltro`, `Grupo`/`GrupoPartidos`, controladores (`ControladorGrupos`, `ControladorIfThen`, relaciones CP/GE). |
| `Reduccion/` | 7 algoritmos reductores (`JDC`, `JLPM`, `XFSF`, `XfsfV3`, `ReductorTM`, `Redu1305Xfsf`, `JDCDobleContador`) sobre `Base : IReduccion`. |
| `Escrutinio/` | Cálculo de premios: `Escrutador`, `EscrutadorComb`, `ColumnasPremiadas`. |
| `EntradaSalida/` | Persistencia: `AConfiguracion` (lee `parametros.free1x2`), `ArchivoColumnasTexto`, `ArchivoCombinacion`, +14 clases `F*Data`, `GenerarCPs/`. |
| `Analisis/` | Estadística de combinaciones: un **segundo `Analizador`** (namespace `Free1X2.Analisis`), contenedores, probabilidades. |
| `Utils/` | `UtilColumnas` (codificación bit), `Porcentajes`, `CompresorZip`, `LAE`, `ConvertidorDeBases`. |
| `Infraestructura/` | `ManejadorExcepciones.cs` (**excluido del build** por tipos duplicados). |

## 2. Lógica de dominio y flujo de cálculo

**Representación núcleo:** una columna (14 signos) se codifica como un **`long`**, 3 bits por partido (mapeo `"2X1"`). En `Utils/UtilColumnas.cs`: `ConvStrToLong`/`ConvLongToStr` (`res = (res << 3) ^ (1 << "2X1".IndexOf(s[i]))`). Permite operar columnas con máscaras de bits — corazón de rendimiento del motor.

**Flujo de alto nivel:**

1. **Pronósticos** → el usuario fija 1/X/2 por partido (`UI/Controls/Pronosticos`, `Prono1X2`).
2. **Grupos + filtros** → `Grupo` agrupa un `List<IFiltro>` + tolerancias (`ControladorTol`); `ControladorGrupos` orquesta varios grupos. Los 14 filtros implementan `IFiltro` (`bool Analizar(long columna)`): `FiltroColProbables, FiltroContactos, FiltroDibujos, FiltroDiferencias, FiltroDistancias, FiltroFormatos123, FiltroFormatosSignos, FiltroGruposEquipos, FiltroInterrupciones, FiltroNoVariantes, FiltroPesosNumericos, FiltroSignosSeguidos, FiltroSimetrias, FiltroValoracionSignos`. (Los "24 filtros" de notas previas cuentan **forms de UI**, no implementaciones de motor.)
3. **Generación** → `GeneradorColumnas.GenerarColumnas()` arranca en la columna todo-1 y recorre recursivamente las variantes, pasando cada columna a `Analizador.AnalizaColumna(long)`, que la acepta/rechaza según los filtros activos. Alternativa: leer de archivo.
4. **Reducción** → `Reduccion/*` reduce el conjunto a un objetivo manteniendo garantías de acierto (`ComienzaReduccion(entrada, salida, nivel, maxCol, percent)`).
5. **Escrutinio** → `Escrutador` compara columnas contra la ganadora y reparte premios (usa `DataSet`). `Utils/Comparador` (ya en Domain) calcula coincidencias/diferencias — invariante `coincidencias + diferencias = 14`.

## 3. Modelo de datos y formatos

- **`Grupo`** (`MotorCalculo/Grupo.cs`, 616 líneas): `List<IFiltro>` + `bool[] partidosActivos` + máscara + caché `Dictionary<long,bool>`. Acopla UI vía `Application.StartupPath`.
- **`VariablesGlobales`** (singleton estático): config global (nº partidos=14, puntos CP, flags de análisis, idioma). **Usado en 69 archivos** — fuerte acoplamiento estático.
- **Formatos:** `.comb`/`.xml` (combinaciones), columnas `.txt`, config XML **`parametros.free1x2`** (`AConfiguracion`). **Sin `BinaryFormatter` ni `[Serializable]`** — bueno para portabilidad.

## 4. Calidad y deuda técnica

| Métrica | Valor |
|---|---|
| Archivos > 2.000 líneas (todos en `UI/`) | 12 (top: `TriosFrm.cs` **7.212**, `BancoPruebasFrm.cs` **4.444**, `ColProbablesFrm.cs` 4.262, `PosiblesPremiosFrm.cs` 3.755) |
| `.resx` | 168 |
| Acoplamiento dominio→WinForms | **18 archivos** usan `System.Windows.Forms`/`System.Drawing` |
| `MessageBox` en dominio | `ControlGrupos.cs` (2), `Analisis/Analizador.cs` (1), `Porcentajes.cs` (7) |
| `Application.StartupPath` | 64 ocurrencias |
| Uso de `VariablesGlobales` | 69 archivos |
| `DataSet`/`DataTable` | 19 archivos |
| TODO/FIXME/HACK | 1 |

**Code smells principales:**
- **Dominio contaminado por UI:** reductores y los dos `Analizador` referencian WinForms (`Application.DoEvents` + `MessageBox`). Bloquea el reuso desde WinUI.
- **God-forms:** `TriosFrm` (7,2k líneas), `BancoPruebasFrm` (4,4k) mezclan UI, orquestación y lógica.
- **Estado global mutable** (`VariablesGlobales` + `Application.StartupPath`) en lugar de inyección.
- **Duplicación:** dos clases `Analizador`; `ManejadorExcepciones.cs` excluido por colisión.
- **Código muerto:** `Free1X2.Shared` + `Free1X2.WebAPI` fuera de la solución; `Web References/`, `Free1X2.csproj.backup`, ~20 `.md` de status histórico.

**Tests:** mínimos pero estratégicos — `Free1X2.Domain.Tests` (10 golden-master sobre `Comparador` y `RangosHelper`).

## 5. Métricas globales

- **Total .cs (sin bin/obj):** ~176.400 líneas en 391 archivos.
- **Free1X2 (monolito):** ~171.400 líneas → ~97% del código.
- **UI:** ~115 Forms + 53 UserControls.
- **WinUI 3:** 205 líneas, 6 .cs (scaffold).

## 6. Riesgos para la migración a WinUI 3 (por dificultad)

1. **Desacoplar el dominio de WinForms.** *(En su mayor parte resuelto.)* `MotorCalculo`, `Reduccion`, `Escrutinio` y `EntradaSalida` ya están libres de `System.Windows.Forms`: las llamadas a `Application.DoEvents`/`MessageBox`/visor se enrutan por los hooks estáticos `Free1X2.Abstractions.{UiPump, UserDialogs, AnalisisUi}` (`UiHooks.cs`), cableados desde WinForms en `Program.WireDomainHooks()`. `Application.StartupPath` → `AppContext.BaseDirectory`. Quedan acoplados solo archivos UI-genuinos en `Utils` (`Grafico`/PictureBox, `Porcentajes`/MessageBox+Clipboard, `ValidadorCaracteres`/SendKeys, `ControlCompatibility`/Form) y `Analisis/AnalisisCombinacion` (TreeNode).
2. **God-forms con lógica embebida.** `TriosFrm`, `BancoPruebasFrm`, `ColProbablesFrm`, `PosiblesPremiosFrm`, `TramificarForm` (>3.000 líneas c/u). El grueso del esfuerzo.
3. **`VariablesGlobales` y `parametros.free1x2`.** 69 archivos dependen del singleton → reemplazar por config inyectada.
4. **`DataSet`/`DataTable` en escrutinio** (19 archivos) → modelos POCO + colecciones observables.
5. **Controles sin equivalente directo:** `DataVisualization` (gráficos), impresión de boletos, `DataGridView` masivos → Win2D/primitivas WinUI.
6. **Superficie enorme:** ~115 forms a portar (estrategia strangler-fig, `PLAN_MIGRACION_WINUI3.md`).

**Puntos a favor:** la codificación `long` de columnas y `Comparador`/utils son lógica pura y portable; sin serialización binaria; red de tests golden-master + scaffold WinUI que compila y renderiza Fluent nativo.

**Archivos clave:** `Free1X2/MotorCalculo/{Analizador, GeneradorColumnas, Grupo, IFiltro}.cs`, `Free1X2/Utils/UtilColumnas.cs`, `Free1X2/VariablesGlobales.cs`, `Free1X2/EntradaSalida/AConfiguracion.cs`, `Free1X2.Domain/`, `Free1X2.WinUI/MainWindow.xaml`.
