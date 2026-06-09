# Free1X2 — Contexto para Claude Code

## Qué es este proyecto

**Free1X2** es una herramienta de escritorio Windows para análisis estadístico de la Quiniela española (apuestas de fútbol, sistema 14 partidos con resultado 1/X/2). Permite generar y reducir columnas de apuestas, aplicar filtros matemáticos, analizar históricos y generar boletos imprimibles.

- **Versión**: 0.77.2 "Rarotonga"
- **Stack**: WinForms .NET 8.0-windows (solo Windows)
- **SDK requerido**: .NET 8+ (probado con .NET 10 SDK en Mac)
- **Build**: `dotnet build Free1X2/Free1X2.csproj`

## Estado actual de la rama

**Rama activa**: `ui-modernization`  
**Tag de referencia**: `v0.78.0-ui-modernization-ready`  
**Tag del estado original** (antes de modernizar): `v0.77.2-legacy-ui`

### Lo que está completo

Modernización completa de la UI — todas las fases ejecutadas:

| Fase | Descripción | Estado |
|------|-------------|--------|
| 1 | ModernTheme.cs + NeoToolStripRenderer + ModernFormBase | ✅ |
| 2 | MainForm — menus, toolbars, status bar, botones de filtro | ✅ |
| 3 | 111 Forms (batch via tools/apply_theme_batch.py) | ✅ |
| 4 | 24 Filtros (incluídos en batch Fase 3) | ✅ |
| 5 | 53 UserControls (batch extendido a UserControl) | ✅ |
| 6 | Estadísticas (incluídas en batch Fase 3) | ✅ |
| 7 | Pulido — CtrSemaforo, sweep colores, limpieza | ✅ |

### Lo que falta

1. **Prueba visual en Windows** — verificar que todos los forms se ven correctos al ejecutar
2. **Correcciones post-prueba** — ajustar lo que no se vea bien
3. **Merge a main** — cuando prueba en Windows sea aprobada por el usuario
4. **Manual de usuario** — PLAN_MANUAL_USUARIO.md (diferido hasta después del merge)

## Cómo continuar en Windows

```bash
# Clonar o hacer pull de la rama
git clone https://github.com/jrtejada2k/Free1x2
git checkout ui-modernization

# Compilar
dotnet build Free1X2/Free1X2.csproj

# Ejecutar y probar visualmente
dotnet run --project Free1X2/Free1X2.csproj
```

### Qué verificar al probar

Abrir cada sección y confirmar visualmente:

- [ ] MainForm: fondo slate #F5F7FA, toolbars flat sin gradientes, status bar índigo, iconos Fluent
- [ ] Fuente Segoe UI visible en todos los controles (no "Microsoft Sans Serif")
- [ ] Sin colores Bisque/NavajoWhite/DarkSalmon en ningún form
- [ ] Botones filtro en MainForm: verde/rojo/blanco según estado (no LightGreen/Tomato/DarkSalmon)
- [ ] CtrSemaforo (dots de semáforo): rojo/amarillo/verde modernos sin bordes 3D
- [ ] BancoPruebasFrm: tabs, grids, groupboxes con tema moderno
- [ ] Cualquier form de filtro (Contactos, Distancias, etc.): fondo blanco, fuente Segoe UI
- [ ] Boleto: controles legibles con nueva paleta

### Proceso de corrección

Si algo no se ve bien:
1. Identificar el archivo (form o control)
2. Verificar si tiene `OnLoad` con `ModernTheme.ApplyToForm/ApplyToControl`
3. Si tiene colores dinámicos en código (no Designer), actualizarlos a tokens de ModernTheme
4. Compilar y verificar

## Arquitectura del sistema de temas

```
Free1X2/UI/Modern/Theming/ModernTheme.cs   ← TODO el sistema de temas
Free1X2/UI/Modern/ModernFormBase.cs         ← Base class para forms modernos
```

### Cómo funciona

1. Cada Form/UserControl tiene `OnLoad` que llama `ModernTheme.ApplyToForm(this)` o `ApplyToControl(this)`
2. `ApplyToForm` recorre recursivamente todos los controles hijos
3. Cada tipo de control tiene su propio handler: `StyleButton`, `StyleTextBox`, `StyleDataGrid`, etc.
4. Los colores legacy del Designer (Bisque, DarkSalmon, etc.) se sobreescriben en runtime

### Paleta de colores

Paleta **Slate / Indigo** (más contraste que el gris Windows-11 original).

| Token | Color | Hex | Uso |
|-------|-------|-----|-----|
| `Colors.Background` | Slate-50 | `#F5F7FA` | Fondo forms/panels |
| `Colors.Surface` | Blanco | `#FFFFFF` | Cards, inputs, grids |
| `Colors.Primary` | Índigo-600 | `#4F46E5` | Acento, status bar |
| `Colors.Text` | Slate-900 | `#0F172A` | Texto principal |
| `Colors.Border` | Slate-300 | `#CBD5E1` | Bordes controles |
| `Colors.Success` | Verde-600 | `#16A34A` | Filtro activo |
| `Colors.Error` | Rojo-600 | `#DC2626` | Filtro con error |
| `Colors.Warning` | Ámbar-600 | `#D97706` | Estado neutro/advertencia |

### Botones de filtro (MainForm)

Los botones de filtro usan `SetBotonEstado(btn, BotonEstado)` en `MainForm.cs`:
- `BotonEstado.Activo` → verde Success
- `BotonEstado.Error` → rojo Error
- `BotonEstado.Neutro` → ámbar claro
- `BotonEstado.Inactivo` → Surface blanco (default)

### CtrSemaforo (control semáforo)

Ubicado en `Free1X2/UI/Controls/CtrSemaforo.cs`.  
Los 3 mini-botones tienen `Tag = "no-theme"` para excluirlos del tema general.  
Sus colores se definen internamente con constantes `ActiveRed/ActiveYellow/ActiveGreen`.

### Iconos Segoe Fluent

```
Free1X2/UI/Modern/Icons/SegoeIcons.cs    ← renderiza glifos Fluent como Bitmap
Free1X2/UI/Modern/Icons/IconReplacer.cs  ← mapea control.Name → glifo y reemplaza imágenes
```

- `SegoeIcons.Render(glyph, size, color)` dibuja un glifo de **Segoe Fluent Icons**
  (fallback a *Segoe MDL2 Assets* en Win10). Devuelve `null` si no hay fuente → se
  conservan los iconos legacy, sin crash.
- `IconReplacer.Apply(root)` recorre el árbol de controles y reemplaza las imágenes de
  toolbars/menús/botones cuyo `Name` está en el diccionario `Map`. Es idempotente.
- `MainForm.OnLoad` llama `IconReplacer.Apply(this)` después de aplicar el tema.

### Contador de pronósticos (Pronosticos.cs)

`Prono1X2` expone el evento `PronosticoChanged`, propagado por `PartidoBoleto`.
`Pronosticos.ActualizarContador()` escucha esos eventos y muestra en `lblTitulo`
el conteo en vivo: **"Fijos: X - Dobles: Y - Triples: Z"**.

## Estructura de archivos clave

```
Free1x2/
├── CLAUDE.md                          ← este archivo
├── PLAN_UI_MODERNIZACION.md           ← plan completo (referencia)
├── PLAN_MANUAL_USUARIO.md             ← plan manual (diferido post-merge)
├── Free1X2/
│   ├── Free1X2.csproj                 ← proyecto principal
│   ├── Program.cs                     ← entry point
│   ├── Infraestructura/
│   │   └── ManejadorExcepciones.cs    ← stubs de error handler (recreados)
│   └── UI/
│       ├── MainForm.cs                ← form principal (modernizado manualmente)
│       ├── Modern/
│       │   ├── ModernFormBase.cs      ← base class
│       │   ├── Icons/
│       │   │   ├── SegoeIcons.cs       ← render de glifos Fluent
│       │   │   └── IconReplacer.cs     ← reemplazo de iconos por Name
│       │   └── Theming/
│       │       └── ModernTheme.cs     ← sistema de temas completo
│       ├── Controls/
│       │   ├── CtrSemaforo.cs        ← semáforo de filtros (modificado)
│       │   └── Analisis/              ← 19 controles de análisis
│       ├── Filtros/                   ← 24 forms de filtros
│       └── Estadisticas/              ← 5 forms de estadísticas
├── Free1X2.Shared/                    ← lógica compartida
└── tools/
    └── apply_theme_batch.py           ← script que modernizó 164 archivos
```

## Reglas para este proyecto

- **No cambiar lógica de negocio** — solo UI (colores, fuentes, temas)
- **No tocar Designer.cs** — los colores allí se sobreescriben en runtime por el tema
- **Commits por funcionalidad** — no batches masivos sin descripción
- **Compilar antes de commitear** — 0 errores siempre
- **Rama ui-modernization** hasta aprobación visual → luego merge a main
- **Cuota de plan**: preguntar antes de ejecutar tareas masivas (subagents, búsquedas extensas)

## Historial relevante de esta sesión

1. Análisis de UI legacy → encontrado: Bisque/NavajoWhite/Verdana/FlatStyle.Popup en 220+ archivos
2. Tag `v0.77.2-legacy-ui` en main → snapshot del estado original
3. Rama `ui-modernization` creada
4. Fase 1: ModernTheme.cs con Segoe UI, paleta Windows 11, NeoToolStripRenderer
5. Fase 2: MainForm con OnLoad + SetBotonEstado para botones semánticos
6. Fase 3-6: batch Python modernizó 164 forms/controles en una pasada
7. Fase 7: CtrSemaforo colores modernos + cleanup
8. Tag `v0.78.0-ui-modernization-ready` en rama actual
9. Push a GitHub → listo para prueba en Windows
