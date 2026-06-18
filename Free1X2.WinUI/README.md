# Free1X2.WinUI

Capa de UI en **WinUI 3** (Windows App SDK 1.6, .NET 8, self-contained win-x64) de Free1X2.
Es la **interfaz principal** del programa tras la migración desde WinForms. Detalle de
arquitectura en [`../docs/ANALISIS_TECNICO_WINUI3.md`](../docs/ANALISIS_TECNICO_WINUI3.md);
plan original en [`../PLAN_MIGRACION_WINUI3.md`](../PLAN_MIGRACION_WINUI3.md).

## Estado

La **estructura de UI está completa y cableada**: **108 pantallas** portadas desde WinForms
(registro en `Navigation/PortedPages.cs`) más la `MainPage`, todas accesibles desde los menús
y la barra de herramientas, y verificadas por el *smoke test* (`FREE1X2_SMOKE=1` → 109/109
páginas cargan). El motor de cálculo del programa original se reutiliza intacto vía
`Free1X2.Domain`.

**Pendiente:** portar a `Free1X2.Domain` la lógica de dominio (E/S de archivos y cálculo) de
**~23 pantallas** todavía marcadas como TODO en el código — lista honesta con evidencia
`file:line` en [`../docs/ANALISIS_TECNICO_WINUI3.md`](../docs/ANALISIS_TECNICO_WINUI3.md) (§11).

Características:

- **Ventana principal** con `MenuBar` (Free1x2 / Archivo / Combinación / Ver / Operaciones /
  Filtros / Utilidades / Ayuda) y **barra de herramientas** de 6 grupos (su visibilidad se
  siembra desde `parametros.free1x2`).
- **Navegación** por `Frame` a las páginas portadas (`MainWindow.Navegar`), con *handoff*
  estático productor→visor para pasar resultados entre pantallas.
- **Mica** + barra de título extendida (look Win11 nativo).
- **Sistema de diseño** Índigo & Teal en `Themes/Tokens.xaml` (`ThemeDictionaries` Light/Dark)
  y estilos en `Themes/Styles.xaml` (tarjetas, botones de filtro de 4 estados, textos).
- **Tema claro/oscuro/sistema** conmutable desde Ajustes.
- MVVM con `CommunityToolkit.Mvvm`.

Capturas de la interfaz en el [README raíz](../README.md#capturas-winui-3).

## Build

Requiere SDK .NET 8 + carga de Windows App SDK (se restaura por NuGet).

```powershell
dotnet build Free1X2.WinUI/Free1X2.WinUI.csproj -c Debug -p:Platform=x64 -r win-x64
```

## Run

```powershell
# El runtime va empaquetado (self-contained), no requiere instalar el Windows App Runtime.
.\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe
```

Publicar un build portable (instalable):

```powershell
dotnet publish Free1X2.WinUI/Free1X2.WinUI.csproj -c Release -r win-x64 --self-contained true -p:Platform=x64
```

## Estructura

```
Free1X2.WinUI/
├── App.xaml(.cs)             ← bootstrap, merge de recursos, cableado de hooks de dominio
├── MainWindow.xaml(.cs)      ← ventana principal: MenuBar + barra de herramientas + Frame de navegación
├── Themes/Tokens.xaml        ← sistema de diseño (paleta Índigo & Teal, tipografía, tokens)
├── Themes/Styles.xaml        ← estilos (tarjetas, botones de filtro, textos)
├── Navigation/PortedPages.cs ← registro de las 108 páginas portadas
└── Views/
    ├── MainPage              ← pantalla de inicio (boleto base, 14 partidos)
    └── Ported/               ← pantallas portadas desde WinForms (+ sus ViewModels)
```
