# Continuación — Free1X2 mejoras (handoff)

> Estado tras el cierre autónomo del plan de mejoras. Fecha: 2026-09-21.
> Detalle completo con evidencia `file:line` en [`PLAN_MEJORAS.md`](PLAN_MEJORAS.md).

## Dónde estamos — v0.83.0 PUBLICADA ✅

- **Release `v0.83.0` «Rarotonga» publicada**: <https://github.com/jrtejada2k/Free1x2/releases/tag/v0.83.0>
  (no-draft, *latest*, asset `Free1X2-WinUI3-win-x64.zip` 63,5 MB con el exe FileVersion `0.83.0`).
- `mejoras-0.83` **mergeada en `main`** (`--no-ff`, commit `e26e850`) y **pusheada** a `origin`. Tag
  `v0.83.0` anotado en `origin`. Rama `mejoras-0.83` **conservada** (R6, no borrar).
- Calidad del release: **build 0 errores · `dotnet test` 133/133 · smoke 109/109** · igualdad del
  motor verificada por SHA-256 contra la DLL de v0.82.0.

## Hecho en el cierre autónomo (sobre el handoff anterior)

- **Smoke 109/109** ejecutado y verde (P-B: gate runtime que sí se podía automatizar por log).
- **U-06 icono** de barra de título aplicado (`AppWindow.SetIcon(Assets\app.ico)` en
  `MainWindow.xaml.cs`; `app.ico` añadido como `Content`). Build 0 err + smoke 109/109 tras el cambio. Commit `57ef6ad`.
- **Bump a 0.83.0** + docs al día (CLAUDE.md, README.md, ESTADO_MIGRACION, ANALISIS_TECNICO §11,
  MANUAL_FLUJOS): versión, **tests 125→133** (84 `[Fact]` + 49 `[InlineData]`), lista de `Services/`.
  Commit `198fdac`.
- **Publish** self-contained win-x64 verificado (FileVersion 0.83.0, `Documentacion/{GPL,licencia}.txt`,
  `Assets/logo.jpg`+`app.ico`, `resources.pri`, Datos semilla) y **release** creado con `gh`.

## PENDIENTE (no bloquea; decisión o pasada visual del dueño)

### P-A · U-11 — restos de patrón manual (decisión del dueño) — SE DEJÓ 1:1
Convertirlos **cambia comportamiento** (choca con R1/R2), así que **se dejaron como están** por
defecto (regla 1:1), pendiente de que el dueño diga «cambiar a sabiendas»:
- `FormatosFrmPage`: 2 `TextBox` numéricos → `LimiteLineas`/`Global` son **strings** con vacío =
  «sin límite» (`FormatosFrmViewModel.cs:190-193`); un `NumberBox` forzaría `double`.
- `ComboBox` del boleto (Local/Visitante): el original era ComboBox; `AutoSuggestBox` arriesga la
  corrección B-03.

### P-B-visual · Pasada VISUAL con la app abierta (no se pudo hacer dormido)
El **smoke 109/109** confirma que las 109 superficies instancian sin fallo. Lo que necesita **ojo
humano** (no se hizo: requería capturas y aprobar `request_access` de computer-use con el dueño
presente) queda para una revisión tranquila:
- **U-07 DPI**: revisar a 125 %/150 % las páginas **sin `ScrollViewer`** (lista en
  `PLAN_MEJORAS.md` §7ter tanda 4); envolver en `ScrollViewer` **solo** donde se recorte. Es un
  posible detalle cosmético preexistente, no una regresión de 0.83.0.
- **Tema oscuro**: confirmar que ningún texto queda ilegible en alguna página.
- **Icono/atajos/idioma/mínimo**: confirmación visual en vivo (código verificado + smoke verde).

### Opcional / futuro (documentado, no bloquea)
- Extender la localización a las 108 páginas (esfuerzo L; menú+toolbar tienen textos hardcodeados en
  `MainWindow.xaml.cs` que necesitan `ResourceLoader`, no bastan `x:Uid`).
- Fuera por decisión del dueño: C-07 (borrar BoletoControl), N-04 (límite 32767), P-18/P-19 (motor,
  sin test de igualdad), N-05 (HashSet figuras).

## Reglas vigentes (resumen)
R1 lógica del motor no cambia (133 tests verdes). R2 diseño lo decide el dueño. R3 completitud
binaria (0/1, con evidencia). R4 sin evidencia no hay hallazgo. R5 un solo `dotnet build` a la vez.
R6 no borrar ramas ni cambiar visibilidad. **R7 no abrir la app / smoke / capturas sin permiso del
dueño** (esta sesión sí tuvo permiso explícito para el smoke).
