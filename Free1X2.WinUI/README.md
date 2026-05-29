# Free1X2.WinUI

Nueva capa de UI en **WinUI 3** (Windows App SDK 1.6, .NET 8) para Free1X2.
Forma parte de la migración incremental desde WinForms descrita en
[`PLAN_MIGRACION_WINUI3.md`](../PLAN_MIGRACION_WINUI3.md).

## Estado: scaffold validado (Fase 1 parcial)

Lo que ya funciona (compila, corre y renderiza Fluent nativo — ver `docs/winui3-shell.png`):

- **Shell** con `NavigationView` (Inicio, Boleto, Filtros, Operaciones, Estadísticas, Escrutinio, Ajustes).
- **Mica** + barra de título extendida (look Win11 nativo).
- **Sistema de diseño** slate/índigo en `Themes/Tokens.xaml` con `ThemeDictionaries` Light/Dark
  y override de `SystemAccentColor` → índigo `#4F46E5`.
- **Tema claro/oscuro/sistema** conmutável desde Ajustes.
- **Slice de prueba**: `BoletoPage` genera las 14 filas de partido con toggles 1/X/2 y
  contador en vivo Fijos/Dobles/Triples (patrón a formalizar con MVVM + dominio).
- MVVM con `CommunityToolkit.Mvvm`.

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

## Estructura

```
Free1X2.WinUI/
├── App.xaml(.cs)            ← bootstrap, merge de recursos
├── MainWindow.xaml(.cs)     ← shell NavigationView + Mica
├── Themes/Tokens.xaml       ← sistema de diseño (paleta, tipografía, tokens)
└── Views/
    ├── HomePage             ← landing + showcase del design system
    ├── BoletoPage           ← slice de prueba (14 partidos)
    ├── PlaceholderPage       ← secciones pendientes de migrar
    └── SettingsPage          ← tema claro/oscuro/sistema
```

## Próximos pasos

Según el roadmap: Fase 0 (extraer `Free1X2.Domain` desde el proyecto WinForms + red de
tests) → Fase 2 (boleto + primer filtro end-to-end con MVVM real sobre el dominio).
