# Plan de Modernización UI - Free1X2

**Tag de referencia**: `v0.77.2-legacy-ui` (estado antes de este trabajo)  
**Rama de trabajo**: `ui-modernization`  
**Merge a main**: solo cuando todas las fases estén verificadas y aprobadas por el usuario

---

## Diagnóstico del estado actual

| Problema | Alcance |
|----------|---------|
| Fuente "Microsoft Sans Serif 8.25pt" | 220+ archivos UI |
| Colores Bisque/NavajoWhite hardcoded | 80% de controles |
| Posicionamiento manual en píxeles | mayoría de forms |
| DataGrid legacy (no DataGridView) | BancoPruebasFrm + varios |
| Sin tema centralizado activo | ModernTheme.cs existe pero no aplicado |
| Sin DPI awareness real | toda la app |
| Sin modo oscuro/claro | toda la app |
| Botones estilo Windows 95 | toda la app |
| Iconos desactualizados | barraIconos, toolstrips |

## Estrategia

**Framework: mantener WinForms** (no migrar a WPF/MAUI, demasiado riesgo, app funcional compleja).  
**Objetivo visual**: Windows 11 / Material Design adaptado a WinForms.  
**Paleta**: blancos, grises neutros, azul acento `#0078D4` (Fluent UI / Windows 11).  
**Fuente**: Segoe UI 9pt (o Segoe UI Variable si Win11 detectado).  
**Aprovechar**: ModernTheme.cs y ModernFormBase.cs ya iniciados, expandirlos.

---

## FASE 1 — Sistema de Temas (Fundación)
*Prerequisito de todas las fases. No tocar forms hasta que esto esté completo y verificado.*

### Tarea 1.1 — Finalizar ModernTheme.cs
**Archivo**: `Free1X2/UI/Modern/Theming/ModernTheme.cs`  
**Acción**: Expandir con todos los tokens necesarios.  
**Debe incluir**:
```csharp
// Colores - Surface
ColorBackground        = Color.FromArgb(243, 243, 243)  // fondo app
ColorSurface           = Color.White                     // cards/panels
ColorSurfaceAlt        = Color.FromArgb(249, 249, 249)  // filas alternas grids

// Colores - Acento
ColorPrimary           = Color.FromArgb(0, 120, 212)    // azul Windows 11
ColorPrimaryHover      = Color.FromArgb(0, 102, 180)
ColorPrimaryPressed    = Color.FromArgb(0, 84, 153)

// Colores - Texto
ColorTextPrimary       = Color.FromArgb(26, 26, 26)
ColorTextSecondary     = Color.FromArgb(97, 97, 97)
ColorTextDisabled      = Color.FromArgb(161, 161, 161)

// Colores - Borde
ColorBorder            = Color.FromArgb(213, 213, 213)
ColorBorderFocus       = Color.FromArgb(0, 120, 212)

// Colores - Estado
ColorSuccess           = Color.FromArgb(16, 124, 16)
ColorWarning           = Color.FromArgb(193, 100, 0)
ColorError             = Color.FromArgb(196, 43, 28)
ColorInfo              = Color.FromArgb(0, 120, 212)

// Fuentes
FontDefault            = new Font("Segoe UI", 9f, FontStyle.Regular)
FontSmall              = new Font("Segoe UI", 8f, FontStyle.Regular)
FontBold               = new Font("Segoe UI", 9f, FontStyle.Bold)
FontHeader             = new Font("Segoe UI", 11f, FontStyle.SemiBold)
FontTitle              = new Font("Segoe UI", 14f, FontStyle.SemiBold)

// Tamaños
ControlHeight = 28
ButtonHeight  = 32
ToolbarHeight = 40
GridRowHeight = 24
BorderRadius  = 4
```
**Criterio de aceptación**: archivo compila sin errores, todos los tokens definidos como `static readonly`.

---

### Tarea 1.2 — Crear ThemeApplier.cs
**Archivo nuevo**: `Free1X2/UI/Modern/Theming/ThemeApplier.cs`  
**Propósito**: Métodos estáticos para aplicar tema a cualquier tipo de control recursivamente.  
**Debe implementar**:
```csharp
public static class ThemeApplier
{
    public static void ApplyToForm(Form form)
    public static void ApplyToControl(Control control)
    public static void ApplyToMenuStrip(MenuStrip menu)
    public static void ApplyToToolStrip(ToolStrip toolbar)
    public static void ApplyToDataGridView(DataGridView grid)
    public static void ApplyToGroupBox(GroupBox gb)
    public static void ApplyToButton(Button btn)
    public static void ApplyToTextBox(TextBox tb)
    public static void ApplyToLabel(Label lbl)
    public static void ApplyToTabControl(TabControl tab)
    public static void ApplyToListView(ListView lv)
    public static void ApplyToComboBox(ComboBox cb)
}
```
- `ApplyToForm` debe llamar `ApplyToControl` recursivamente en todos los hijos.
- `ApplyToControl` debe hacer dispatch según tipo del control.
- Respetar controles con `Tag = "no-theme"` (skip).  

**Criterio de aceptación**: compila, método `ApplyToForm` aplicable a cualquier Form sin excepciones.

---

### Tarea 1.3 — Finalizar ModernFormBase.cs
**Archivo**: `Free1X2/UI/Modern/ModernFormBase.cs`  
**Acción**: Asegurarse de que:
- Llama `ThemeApplier.ApplyToForm(this)` en `OnLoad`.
- Activa DPI awareness: `AutoScaleMode = AutoScaleMode.Dpi`.
- Asigna `Font = ModernTheme.FontDefault` en constructor.
- Tiene método `protected virtual void ApplyTheme()` sobreescribible.
- Compatible con Designer de Visual Studio (sin romper herencia).

**Criterio de aceptación**: clase abstracta compila, una form de prueba heredando de ella muestra tema al ejecutar.

---

### Tarea 1.4 — Form de prueba visual (ThemeTestForm)
**Archivo nuevo temporal**: `Free1X2/UI/Modern/ThemeTestForm.cs`  
**Propósito**: Form con todos los tipos de control para verificar tema visualmente.  
**Debe contener**: Button, Label, TextBox, DataGridView (5 filas dummy), ComboBox, GroupBox, TabControl, MenuStrip, ToolStrip, ListView.  
**Criterio de aceptación**: al ejecutar, todos los controles se ven modernos, sin Bisque/NavajoWhite, fuente Segoe UI visible.  
**Nota**: este archivo se elimina al finalizar la fase 1.

---

## FASE 2 — Shell Principal (MainForm)
*Prerequisito: Fase 1 completa.*

### Tarea 2.1 — MainForm hereda ModernFormBase
**Archivo**: `Free1X2/UI/MainForm.cs`  
**Acción**: 
- Cambiar herencia de `Form` a `ModernFormBase`.
- Verificar que Designer siga funcionando.
- Ajustar constructor si es necesario.

**Criterio de aceptación**: app arranca, MainForm visible con fondo gris claro, fuente Segoe UI.

---

### Tarea 2.2 — Modernizar MenuStrip
**Archivo**: `Free1X2/UI/MainForm.cs` + `MainForm.Designer.cs`  
**Acción**:
- `menuStrip.BackColor = ModernTheme.ColorBackground`
- `menuStrip.ForeColor = ModernTheme.ColorTextPrimary`
- `menuStrip.Font = ModernTheme.FontDefault`
- Implementar `ToolStripProfessionalRenderer` con colores personalizados.
- Hover color: `ModernTheme.ColorPrimary` con texto blanco.

**Criterio de aceptación**: menú principal se ve moderno, hover visible, sin gradientes Windows XP.

---

### Tarea 2.3 — Modernizar los 6 ToolStrips
**Archivos**: `MainForm.cs` / `MainForm.Designer.cs`  
**ToolStrips afectados**: `tsFree`, `tsArchivo`, `tsOperaciones`, `tsCombinacion`, `tsFiltros`, `tsUtilidades`  
**Acción**:
- Aplicar mismo renderer que MenuStrip.
- `toolbar.BackColor = ModernTheme.ColorBackground`
- Botones: `DisplayStyle = ImageAndText` o `ImageOnly` donde corresponda.
- Altura: `ModernTheme.ToolbarHeight` (40px).
- Separadores: `ForeColor = ModernTheme.ColorBorder`.

**Criterio de aceptación**: los 6 toolbars se ven flat y modernos, sin relieve 3D.

---

### Tarea 2.4 — StatusBar / StatusStrip
**Archivo**: `MainForm.cs`  
**Acción**:
- Reemplazar `StatusBar` legacy por `StatusStrip` si no existe ya.
- `statusStrip.BackColor = ModernTheme.ColorPrimary`
- `statusStrip.ForeColor = Color.White`
- Fuente: `ModernTheme.FontSmall`

**Criterio de aceptación**: barra de estado visible con fondo azul Windows 11, texto blanco legible.

---

### Tarea 2.5 — GroupBoxes y fondo MainForm
**Archivo**: `MainForm.cs` + `MainForm.Designer.cs`  
**Acción**:
- `this.BackColor = ModernTheme.ColorBackground`
- Todos los GroupBox: `BackColor = ModernTheme.ColorSurface`, `ForeColor = ModernTheme.ColorTextPrimary`, `Font = ModernTheme.FontDefault`
- Labels: `ForeColor = ModernTheme.ColorTextPrimary`

**Criterio de aceptación**: fondo de MainForm gris claro (#F3F3F3), GroupBoxes blancos, cero Bisque/NavajoWhite.

---

## FASE 3 — Forms Principales
*Prerequisito: Fase 2 completa y aprobada visualmente.*

### Tarea 3.1 — BancoPruebasFrm (mayor prioridad, 216KB)
**Archivo**: `Free1X2/UI/BancoPruebasFrm.cs`  
**Acciones**:
1. Heredar `ModernFormBase`.
2. Reemplazar `DataGrid` legacy por `DataGridView` + `ThemeApplier.ApplyToDataGridView`.
3. `TabControl`: `tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed`, implementar `DrawItem` con colores modernos.
4. `StatusBar` → `StatusStrip`.
5. Eliminar hardcodes Bisque/NavajoWhite/Khaki.
6. Aplicar `ThemeApplier.ApplyToForm(this)` en `OnLoad`.

**Criterio de aceptación**: BancoPruebasFrm abre sin excepciones, tabs visibles, grids con filas blancas/grises alternas, cero colores legacy.

---

### Tarea 3.2 — ConfiguracionFrm
**Archivo**: `Free1X2/UI/ConfiguracionFrm.cs`  
**Acciones**: heredar ModernFormBase, ThemeApplier.ApplyToForm en OnLoad, verificar funcionalidad de configuración intacta.

---

### Tarea 3.3 — AyudaFrm
**Archivo**: `Free1X2/UI/AyudaFrm.cs`  
**Acciones**: heredar ModernFormBase, ThemeApplier.ApplyToForm, verificar que el contenido de ayuda se muestre correctamente.

---

### Tarea 3.4 — GestorEquiposFrm
**Archivo**: `Free1X2/UI/GestorEquiposFrm.cs`  
**Acciones**: heredar ModernFormBase, reemplazar DataGrid si existe, ThemeApplier.ApplyToForm.

---

### Tarea 3.5 — AgregarEquipoFrm
**Archivo**: `Free1X2/UI/AgregarEquipoFrm.cs`  
**Acciones**: heredar ModernFormBase, ThemeApplier.ApplyToForm.

---

### Tarea 3.6 — BoletoFrm
**Archivo**: `Free1X2/UI/BoletoFrm.cs`  
**Acciones**: heredar ModernFormBase, ThemeApplier.ApplyToForm, verificar que el boleto/ticket se muestre correctamente.

---

### Tarea 3.7 — TramificarForm
**Archivo**: `Free1X2/UI/TramificarForm.cs`  
**Acciones**: heredar ModernFormBase, ThemeApplier.ApplyToForm.

---

### Tarea 3.8 — Resto de forms principales (batch)
**Archivos**: todos los `*.cs` en `Free1X2/UI/` que hereden `Form` y no sean filtros ni controles.  
**Acción por cada form**:
1. Heredar `ModernFormBase` (en lugar de `Form`).
2. Agregar `ThemeApplier.ApplyToForm(this)` al final de `InitializeComponent()` o en `OnLoad`.
3. Verificar que la form abre sin excepciones.
4. Verificar que su funcionalidad principal es operativa.

---

## FASE 4 — Forms de Filtros (24 forms)
*Prerequisito: Fase 3 completa.*

### Tarea 4.0 — FilterFormBase
**Archivo nuevo**: `Free1X2/UI/Filtros/FilterFormBase.cs`  
**Propósito**: base común para los 24 filtros.  
```csharp
public abstract class FilterFormBase : ModernFormBase
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // layout estándar para filtros: GroupBox con parámetros, botones OK/Cancel estandarizados
    }
}
```

---

### Tarea 4.1 a 4.24 — Un filtro por tarea
**Archivos** (uno por tarea):
- `Filtros/ContactosFrm.cs`
- `Filtros/DiferenciasFrm.cs`
- `Filtros/FigurasFiltrosFrm.cs`
- `Filtros/SimetriasFrm.cs`
- `Filtros/DistanciasFrm.cs`
- `Filtros/InterrupcionesFrm.cs`
- (los 18 restantes en `Free1X2/UI/Filtros/`)

**Acción estándar por cada uno**:
1. Heredar `FilterFormBase`.
2. `ThemeApplier.ApplyToForm(this)` en `OnLoad`.
3. Eliminar hardcodes de color.
4. Verificar que el filtro funciona (se puede abrir, aplicar, cancelar).

---

## FASE 5 — Controles de Análisis (83 controles)
*Prerequisito: Fase 4 completa.*

### Tarea 5.0 — AnalysisControlBase
**Archivo nuevo**: `Free1X2/UI/Controls/Analisis/AnalysisControlBase.cs`  
```csharp
public abstract class AnalysisControlBase : UserControl
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ThemeApplier.ApplyToControl(this);
    }
}
```

### Tarea 5.1 — CtrlAnalisisContactos
**Archivo**: `Free1X2/UI/Controls/Analisis/CtrlAnalisisContactos.cs`  
**Acción**: heredar AnalysisControlBase, reemplazar labels con BackColor NavajoWhite → ModernTheme.ColorSurface, borders → ModernTheme.ColorBorder.

### Tarea 5.2 — CtrlFiguras
**Acción**: heredar AnalysisControlBase, BackColor Bisque → ModernTheme.ColorBackground.

### Tarea 5.3 — CtrlAnalisisDiferencias
**Acción**: heredar AnalysisControlBase, modernizar DataGridView con ThemeApplier.

### Tarea 5.4 a 5.20 — Controles restantes (batch)
**Acción estándar**: heredar AnalysisControlBase (o aplicar ThemeApplier.ApplyToControl en OnLoad), eliminar colores Bisque/NavajoWhite/Khaki hardcoded.

### Tarea 5.21 — Controles de Boleto
**Archivos**: `ControlBoleto.cs`, `PartidoBoleto.cs`, `EquipoBoleto.cs`, `Prono1X2.cs`, `SignoBoletoBase.cs`  
**Acción**: aplicar tema, asegurar que el boleto se ve legible con nueva paleta.

---

## FASE 6 — Estadísticas y Visualización
*Prerequisito: Fase 5 completa.*

### Tarea 6.1 — VisorEstadisticas
**Archivo**: `Free1X2/UI/Estadisticas/VisorEstadisticas.cs`  
**Acción**: heredar ModernFormBase, aplicar tema, actualizar colores del Chart (ejes, fondo, series).

### Tarea 6.2 — DibForm y DibRepFrm
**Acción**: heredar ModernFormBase, aplicar tema.

### Tarea 6.3 — PremiadasFrm y EscrutiniosFrm
**Acción**: heredar ModernFormBase, reemplazar DataGrid por DataGridView con tema.

---

## FASE 7 — Pulido Final
*Prerequisito: Todas las fases anteriores completas.*

### Tarea 7.1 — Iconos y imágenes
**Objetivo**: auditar ImageLists (`ilUtilidades`, `ilArchivo`, `ilFree`, etc.) y reemplazar iconos de 16x16 pixelados por iconos modernos de 24x24 o SVG convertidos.  
**Fuente sugerida**: Segoe MDL2 Assets (incluido en Windows 10/11) o icons de Fluent UI (MIT license).

### Tarea 7.2 — Verificación DPI
**Acción**: ejecutar app a 100%, 125%, 150% y 200% de escala y verificar que todos los controles se ven bien. Ajustar `AutoScaleMode` y `AutoScaleDimensions` donde fallen.

### Tarea 7.3 — Navegación por teclado
**Acción**: verificar que todos los forms tienen TabIndex correcto, AcceptButton y CancelButton definidos, y que se puede navegar sin ratón.

### Tarea 7.4 — Consistencia visual final
**Acción**: abrir cada form, capturar screenshot, comparar con lista de checklist:
- [ ] Fuente Segoe UI visible
- [ ] Sin colores Bisque/NavajoWhite
- [ ] Botones modernos (sin relieve 3D)
- [ ] Grids con filas blancas/grises alternas
- [ ] Labels legibles con contraste suficiente

### Tarea 7.5 — Limpieza de código
**Acción**: buscar y eliminar todos los residuos de colores legacy:
```
grep -rn "Bisque\|NavajoWhite\|Khaki\|Microsoft Sans Serif" Free1X2/UI/
```
Cada resultado debe ser eliminado o reemplazado con tokens de ModernTheme.

---

## Orden de ejecución por fases

```
FASE 1 (Fundación) → revisión y aprobación usuario
    ↓
FASE 2 (MainForm) → revisión y aprobación usuario
    ↓
FASE 3 (Forms principales) → revisión y aprobación usuario
    ↓
FASE 4 (Filtros) → revisión y aprobación usuario
    ↓
FASE 5 (Controles análisis) → revisión y aprobación usuario
    ↓
FASE 6 (Estadísticas) → revisión y aprobación usuario
    ↓
FASE 7 (Pulido) → revisión y aprobación usuario
    ↓
MERGE a main
```

---

## Instrucciones para el agente ejecutor

1. **Nunca cambiar lógica de negocio**. Solo modificar: colores, fuentes, tamaños, herencia de form, aplicación de tema.
2. **Siempre verificar que compila** después de cada tarea antes de continuar.
3. **Nunca mezclar tareas de distintas fases** en un mismo commit.
4. **Commit por tarea** con mensaje: `feat(ui): [Tarea X.Y] descripción breve`
5. Si una form tiene comportamiento de color dinámico (cambiar color según estado), **no eliminar esa lógica**, solo reemplazar los colores hardcoded con tokens de ModernTheme.
6. Si al heredar ModernFormBase el Designer se rompe, usar la alternativa: mantener herencia Form y llamar ThemeApplier.ApplyToForm en OnLoad.
7. **Preguntar al usuario** antes de cambiar cualquier control que afecte funcionalidad (DataGrid → DataGridView puede romper bindings).

---

## Estado de progreso

| Fase | Estado |
|------|--------|
| 1 - Fundación | Pendiente |
| 2 - MainForm | Pendiente |
| 3 - Forms Principales | Pendiente |
| 4 - Filtros | Pendiente |
| 5 - Análisis Controls | Pendiente |
| 6 - Estadísticas | Pendiente |
| 7 - Pulido | Pendiente |
