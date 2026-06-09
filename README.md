# Free1X2

**Free1X2** es una herramienta de escritorio para Windows de análisis y generación de combinaciones para la **Quiniela** española (14 partidos de fútbol con resultado 1 / X / 2, más el Pleno al 15). Permite construir un boleto, aplicar condiciones y filtros matemáticos, generar y **reducir** columnas de apuesta, escrutar resultados y analizar históricos.

Programa libre bajo licencia **GPL** (proyecto original de Joan Duatis, 2005).

- 📖 **Manual de usuario**: [`docs/MANUAL_USUARIO.md`](docs/MANUAL_USUARIO.md)
- 🗺️ **Plan de migración a WinUI 3**: [`PLAN_MIGRACION_WINUI3.md`](PLAN_MIGRACION_WINUI3.md)
- 📌 **Estado de la migración**: [`ESTADO_MIGRACION_WINUI3.md`](ESTADO_MIGRACION_WINUI3.md)

![Free1X2 WinUI 3](docs/winui3-shell.png)

---

## Qué hace

| Área | Descripción |
|------|-------------|
| **Boleto** | Carga/edición del boleto base (14 partidos), equipos, fijos/dobles/triples, grupos. |
| **Condiciones** | 14+ condiciones matemáticas para filtrar columnas: variantes, signos seguidos, dibujos, interrupciones, distancias, grupos de equipos, columnas probables (CP), simetrías, formatos, etc. |
| **Filtros** | Usar un archivo de columnas como base/filtro; combinar, comparar y derivar filtros. |
| **Cálculo** | Genera todas las columnas que cumplen las condiciones; muestra número y coste. |
| **Reducción** | Algoritmos de reducción (JDC, JLPM, XFSF, TM…) para garantizar premios de una categoría con menos columnas. |
| **Escrutinio** | Escruta archivos de columnas contra la ganadora; calcula premios. |
| **Análisis** | Análisis de fallos, gráfico, signos, probabilidades, estadísticas, rentabilidad. |
| **Operaciones** | Álgebra de columnas, transposición, multiplicador, fraccionador, rotación de signos. |
| **Utilidades** | SubeCategoría, generador de CPs, ordenación por probabilidad, tramificar, estimación de premios, banco de pruebas, etc. |

Detalle completo de cada función en el [manual de usuario](docs/MANUAL_USUARIO.md).

---

## Stack y estructura de la solución

`Free1X2.sln` agrupa:

| Proyecto | TFM | Rol |
|----------|-----|-----|
| **`Free1X2`** | `net8.0-windows` (WinForms) | Aplicación actual (UI + parte de la lógica). |
| **`Free1X2.Domain`** | `net8.0` | Lógica de dominio desacoplada de la UI (en migración). Abstracciones `IProgressNotifier`, `IAppPaths`. |
| **`Free1X2.WinUI`** | `net8.0-windows10.0.19041.0` (WinUI 3) | Nueva UI Fluent (Windows App SDK 1.6, self-contained). En construcción. |
| **`Free1X2.Domain.Tests`** | `net8.0` (xUnit) | Red de tests golden-master del dominio. |
| **`Free1X2.Shared`** | `net8.0` | Contratos/servicios compartidos. |

### Carpetas de dominio dentro de `Free1X2/`

- `MotorCalculo/` — motor de análisis, generación de columnas y los filtros (`IFiltro`).
- `Reduccion/` — algoritmos de reducción.
- `Escrutinio/` — escrutinio y cálculo de premiadas.
- `EntradaSalida/` — lectura/escritura de archivos (`.comb`, `.grupos`, columnas, configuración).
- `Analisis/` — contenedores y analizadores de resultados.
- `Utils/` — matemática combinatoria y utilidades.
- `UI/` — formularios WinForms (MainForm, filtros, estadísticas, controles).

---

## Compilar y ejecutar

Requiere **.NET 8 SDK** en Windows.

```powershell
# App actual (WinForms)
dotnet build Free1X2/Free1X2.csproj
dotnet run   --project Free1X2/Free1X2.csproj

# Lógica de dominio + tests
dotnet build Free1X2.Domain/Free1X2.Domain.csproj
dotnet test  Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj

# Nueva UI WinUI 3 (requiere plataforma x64; runtime empaquetado, self-contained)
dotnet build Free1X2.WinUI/Free1X2.WinUI.csproj -c Debug -p:Platform=x64 -r win-x64
.\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe
```

---

## Estado de la migración a WinUI 3

La UI se está migrando de WinForms a **WinUI 3** (Fluent nativo) de forma incremental (*strangler-fig*), reutilizando la lógica de dominio. Resumen:

- ✅ Viabilidad validada (WinUI 3 compila, corre y renderiza Fluent nativo, self-contained).
- ✅ Plan completo y scaffold (shell `NavigationView` + Mica + sistema de diseño slate/índigo).
- 🔄 Fase 0 en curso: desacople de `Free1X2.Domain` + red de tests.
- ⏳ Pendiente: portar las ~109 superficies de UI por olas.

Ver [`ESTADO_MIGRACION_WINUI3.md`](ESTADO_MIGRACION_WINUI3.md) para el detalle y los próximos pasos.

---

## Formatos de archivo

| Extensión | Contenido |
|-----------|-----------|
| `.comb` / `.xml` | Combinación completa (equipos, pronósticos, grupos, condiciones, filtro). |
| `.grupos` | Un grupo de condiciones exportado. |
| `.txt` | Archivo de columnas (una columna de 14 signos por línea). |
| `parametros.free1x2` | Configuración del programa (puntos CP, nº de partidos, idioma, ONLAE…). |

---

## Créditos y fuentes

- Proyecto original **Free1X2** — Joan Duatis (GPL).
- Manual de usuario reconstruido a partir del manual oficial y de
  [quinielaticas.com/programa-free1x2](https://quinielaticas.com/programa-free1x2/).
- Conceptos básicos de quiniela basados en el *Mini-Manual Básico* de Toni Moreno.
