# Revisión UI — Hallazgos (para continuar en Windows)

> Revisión hecha desde Mac (solo análisis de código — WinForms no se puede **ejecutar** en Mac, y la máquina de revisión no tenía .NET 8 SDK). La verificación visual real queda pendiente en Windows.
> Fecha: 2026-05-28

---

## Resumen ejecutivo

La modernización de UI es un **reskin en runtime**, no un rediseño. El resultado se ve **limpio y consistente** (flat, Segoe UI, paleta Windows 11, status bar azul) — claro salto desde el legacy Bisque/Verdana. Pero **no** llega a "elegante moderno premium" estilo Win11/Fluent porque faltan esquinas redondeadas, sombras y trabajo de layout, y hay gaps de cobertura.

**Conclusión corta:** *limpio y consistente, sí. Elegante/moderno premium, todavía no.*

---

## Cómo funciona el tema (importante)

Override **en tiempo de ejecución**:

1. Cada Form/UserControl tiene `OnLoad → ModernTheme.ApplyToForm(this)` / `ApplyToControl(this)`.
2. `ApplyToForm` recorre recursivamente el árbol de controles y **pisa** colores/fuentes legacy.
3. Los `*.Designer.cs` **nunca se tocaron** (regla del proyecto) → el código fuente sigue lleno de legacy, pero en pantalla se renderiza moderno.

### Cobertura
- **166 forms/controles** con `OnLoad → ApplyTo*` (de 167 totales: 114 forms + 53 UserControls). Casi total.
- Patrón de inyección limpio y correcto:
  ```csharp
  protected override void OnLoad(System.EventArgs e)
  {
      base.OnLoad(e);
      ModernTheme.ApplyToForm(this);
  }
  ```

### Legacy que sigue en Designer (pisado en runtime)
Conteos dentro de `Free1X2/`:
- **2469** refs a `Verdana`
- **921** refs a `Microsoft Sans Serif`
- **592** refs a `FlatStyle.Popup`
- **2170** refs a colores legacy (`Bisque`, `NavajoWhite`, `DarkSalmon`, `Khaki`, etc.)

No es un bug en sí (se sobreescriben al cargar), pero significa que el "estado real" del Designer es legacy y todo depende de que el override alcance cada control.

---

## Hallazgos / gaps que impiden el look "elegante moderno"

### 1. Sin esquinas redondeadas, sombras ni acrylic
`Sizes.BorderRadius = 4` está **definido pero nunca usado** (0 pintura custom: ni `GraphicsPath`, ni `Region`, ni `FillRoundedRectangle`). Los botones son rectángulos flat con borde de 1px. Look "flat limpio" tipo Win7, **no** Fluent/Win11.
- Archivo: `Free1X2/UI/Modern/Theming/ModernTheme.cs:84`

### 2. Layout intacto (gap más grande para "elegancia")
La modernización cambió **solo color y fuente**. Espaciado, alineación, padding y tamaños densos del legacy siguen igual. La elegancia visual viene sobre todo de whitespace + layout, y eso no se tocó. Los tokens `Sizes.Spacing*` existen pero casi no se aplican.

### 3. Bug de remapeo en `StylePanel`
`StylePanel` solo cambia el fondo si es `SystemColors.Control`:
```csharp
private static void StylePanel(Panel p)
{
    if (p.BackColor == SystemColors.Control)   // <-- solo este caso
        p.BackColor = Colors.Background;
    p.ForeColor = Colors.Text;
}
```
Un Panel con `Bisque`/`Khaki`/`NavajoWhite` puesto en el Designer **sobrevive sin cambiar**. En cambio `StyleUserControl` **sí** remapea esos colores. Inconsistencia → posibles islas de color viejo en paneles.
- `StylePanel`: `Free1X2/UI/Modern/Theming/ModernTheme.cs:313`
- `StyleUserControl` (referencia de cómo debería hacerse): `Free1X2/UI/Modern/Theming/ModernTheme.cs:209`

### 4. Botones forzados a blanco Surface
`StyleButton` pone todos los botones en `Surface` (blanco) flat. El color semántico (verde/rojo/ámbar) solo aparece donde se llama `SetBotonEstado` — eso solo está en `MainForm`. Botones de color en otros forms quedan en blanco plano.
- `Free1X2/UI/Modern/Theming/ModernTheme.cs:153`

### 5. Tipos de control sin handler (gaps menores)
`DispatchControl` no estiliza: `PictureBox` (3 archivos), `TreeView` (1 archivo). Pocos → impacto bajo. No hay `Chart`, `TableLayoutPanel`, `TrackBar`, etc., así que no preocupa.

---

## Lo que sí está bien

- Paleta Windows 11 coherente (`#0078D4` azul, `#F3F3F3` fondo).
- `Segoe UI` global con fallback seguro.
- `NeoToolStripRenderer` + `NeoColorTable` eliminan gradientes feos de menús/toolbars.
- Status bar azul, grids limpios con header diferenciado y filas alternas.
- `AdaptFont` swap de fuente legacy → Segoe UI preservando bold/italic.

---

## Estado del repo (no-UI)

1. **`main` == `ui-modernization`** — el PR #1 ya se mergeó (commit `38a757a`). Cero diff entre ramas.
2. **`CLAUDE.md` desactualizado** — dice "rama activa ui-modernization, falta merge a main". Ya está mergeado. Actualizar líneas 14, 32-36.
3. **Proyectos huérfanos** — `Free1X2.WebAPI` y `Free1X2.Shared` existen en disco con código real (controllers/services/models) pero **no están en `Free1X2.sln`**. Un doc dice "WebAPI removed" → contradicción. Decidir: agregar a la solución o borrar.
4. **`Free1X2/Free1X2.csproj.backup`** (72 KB) commiteado — basura, borrar.
5. **Versiones inconsistentes** — `Free1X2.csproj` = `0.77.2`; Shared/WebAPI = `0.77.1`; tag = `v0.78.0`.
6. **~17 archivos `.md`** de plan/status en la raíz, muy solapados → candidatos a consolidar.

---

## Qué verificar visualmente en Windows

```bash
git pull
dotnet build Free1X2/Free1X2.csproj
dotnet run --project Free1X2/Free1X2.csproj
```

Checklist de render:
- [ ] MainForm: fondo `#F3F3F3`, toolbars flat sin gradientes, status bar azul.
- [ ] Segoe UI en TODOS los controles (no "Microsoft Sans Serif" ni "Verdana").
- [ ] Sin Bisque/NavajoWhite/DarkSalmon visibles en ningún form.
- [ ] **Paneles** con color viejo (ver bug #3) — buscar islas Bisque/Khaki que no cambiaron.
- [ ] Botones de filtro MainForm: verde/rojo/blanco según estado.
- [ ] Botones de color en OTROS forms (ver #4) — confirmar si quedan en blanco plano.
- [ ] CtrSemaforo: rojo/amarillo/verde modernos sin bordes 3D.
- [ ] Grids (BancoPruebas, etc.): header gris, filas alternas, selección azul.
- [ ] PictureBox / TreeView (ver #5) — confirmar si desentonan.

---

## Mejoras recomendadas (priorizadas)

| Prioridad | Mejora | Riesgo | Notas |
|-----------|--------|--------|-------|
| Alta | Fix bug `StylePanel` — remapear todos los colores legacy (igual que `StyleUserControl`) | Bajo | 1 método |
| Alta | Verificación visual en Windows + capturas | — | Bloquea lo demás |
| Media | Esquinas redondeadas + sombra sutil en botones/cards (custom paint usando `BorderRadius`) | Medio | Sube de "limpio" a "Fluent" |
| Media | Aplicar tokens `Sizes.Spacing*` para padding/whitespace en forms clave | Medio | Elegancia real = layout |
| Baja | Decidir WebAPI/Shared (agregar a .sln o borrar) | Bajo | Limpieza |
| Baja | Borrar `Free1X2.csproj.backup`, consolidar docs, sincronizar `CLAUDE.md` | Bajo | Higiene repo |

---

## Archivos clave

```
Free1X2/UI/Modern/Theming/ModernTheme.cs   ← todo el sistema de temas (490 líneas)
Free1X2/UI/Modern/ModernFormBase.cs         ← base class
Free1X2/UI/MainForm.cs                       ← SetBotonEstado (botones semánticos)
Free1X2/UI/Controls/CtrSemaforo.cs           ← semáforo
tools/apply_theme_batch.py                   ← script que inyectó OnLoad en 164 archivos
```
