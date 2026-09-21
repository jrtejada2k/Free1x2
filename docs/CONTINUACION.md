# Continuación — Free1X2 mejoras (handoff)

> Resumen para retomar el trabajo sin perder contexto. Fecha: 2026-09-20.
> Detalle completo con evidencia `file:line` en [`PLAN_MEJORAS.md`](PLAN_MEJORAS.md).

## Dónde estamos

- Rama **`mejoras-0.83`** (creada desde `main` = `e77a5ac`, release v0.82.0). **23 commits nuevos**, sin pushear a `origin` todavía, working tree limpio.
- Calidad actual: **build 0 errores · `dotnet test` 133/133 · igualdad del motor verificada por SHA-256**. El **smoke NO se ha corrido** desde el arranque del plan (abre ventana → regla R7).
- Regla nueva **R7**: no lanzar la app / smoke / capturas / input sintético **sin permiso explícito del dueño cada vez** (roba el foco). Solo `dotnet build` y `dotnet test` corren libres. Ver [[feedback-never-steal-focus]].

## Ya HECHO (código, verificado con build+tests, sin abrir la app)

| Área | Qué | Commits |
|------|-----|---------|
| F1 bugs | 12 bugs de UI + logging (`Services/Log.cs`); cola de diálogos; nombres compuestos boleto; guarda anti-doble; cálculo múltiple COMException; **6 clicks muertos en Ayuda** | `9109823` `8c9ffb5` `12cfe8c` |
| F2 docs | CLAUDE.md/README/MANUAL_* al día; **20 históricos → `docs/historico/`**; 0 enlaces rotos | `1586c8f` `c8946c8` `af5db88` |
| F3 motor | P-01…P-17 optimizaciones (salida idéntica, SHA-256); +6 tests de igualdad | `526b05f` |
| F4 calidad | ObservableCollection, StringBuilder, dedup, UiHilo, etc. | `8c9ffb5` `12cfe8c` |
| T1 motor | **ReductorTM ya no se cuelga** (N-02) + N-03 (`Inicializa`) + bombeo UI (P-20) | `078b10d` |
| T2 | refactor **168 file pickers → `Services/PickerHelper.cs`** (C-17) | `f69fdea` |
| T3 | refactor cuarteto Guardar/Abrir/Copiar/Pegar → `FiltroArchivoViewModelBase` (12/16, C-18) | `d273395` |
| T5 | **ventana mínima 900×600** (WM_GETMINMAXINFO) + **atajos** Ctrl+N/O/S, F5, F1, Esc (U-04/U-05) | `e1c561e` |
| Tema | **toggle Claro/Oscuro/Sistema** en menú Ver, persistido (U-01/U-02) | `c0a9591` |
| U-09 | orden de botones de diálogo unificado a Aceptar→Cancelar | `ed11361` |
| U-12 | **123 `AutomationProperties.Name`** en 20 páginas (accesibilidad) | `58e4844` |
| U-10 | **localización: infra + piloto** `CreditosFrmPage` bilingüe + menú Ver→Idioma (`Services/IdiomaApp.cs`, `Strings/{es,en-US}/Resources.resw`) | `e1b2371` |
| U-03/U-13 | verificados en estático: colores de leyenda = datos; fondo blanco de exportación = intencional | (en `c0a9591`) |

## PENDIENTE

### P-A · Decisión del dueño — U-11 (restos de patrón manual)
Convertirlos **cambia comportamiento**, choca con la regla 1:1:
- `FormatosFrmPage` 2 `TextBox` numéricos → `LimiteLineas`/`Global` son **strings** con vacío=«sin límite» (`FormatosFrmViewModel.cs:190-193`); un `NumberBox` forzaría `double`.
- `ComboBox` del boleto (Local/Visitante) → el original era ComboBox; `AutoSuggestBox` arriesga la corrección B-03.

**Recomendación: dejar ambos** (1:1). Falta que el dueño diga «dejar» o «cambiar a sabiendas».

### P-B · Tanda 4 — verificación CON LA APP ABIERTA (⚠ requiere permiso, roba foco)
Hacer todo de una vez en un rato sin usar el equipo. Pasos:
1. **Smoke** (build x64 primero):
   ```powershell
   dotnet build Free1X2.WinUI/Free1X2.WinUI.csproj -c Debug -p:Platform=x64
   Remove-Item "$env:TEMP\free1x2_smoke.log" -ErrorAction SilentlyContinue
   $env:FREE1X2_SMOKE='1'
   .\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe | Out-Null
   Remove-Item Env:\FREE1X2_SMOKE
   Get-Content "$env:TEMP\free1x2_smoke.log" -Tail 1   # esperado: SMOKE DONE total=109 ok=109 fail=0
   ```
2. **Tema** (menú Ver→Tema): Claro/Oscuro/Sistema cambia en vivo y **persiste** al reabrir (`%LocalAppData%\Free1X2\tema.json`). En oscuro: revisar que no haya texto ilegible.
3. **Idioma** (menú Ver→Idioma): Español↔English cambia `CreditosFrmPage`; persiste (`%LocalAppData%\Free1X2\idioma.json`).
4. **Ventana mínima**: no encoge por debajo de 900×600.
5. **Atajos**: Ctrl+N/O/S (nueva/abrir/guardar combinación), F5 (calcular), F1 (ayuda), Esc (volver).
6. **U-06 icono**: ¿la barra de título muestra el icono de la app? Si no → `AppWindow.SetIcon` en `MainWindow`.
7. **U-07 DPI**: capturas a 100/125/150 % de las 11 páginas sin `ScrollViewer` (lista en PLAN_MEJORAS §7ter tanda 4); arreglar SOLO donde se recorte.
8. Al terminar: matar todo proceso `Free1X2.WinUI`.

### P-C · Tanda 6 — cierre y publicación (no abre ventana, salvo el smoke de P-B)
1. Bump `Free1X2.WinUI/Free1X2.WinUI.csproj` `AssemblyVersion`/`FileVersion`/`Version` → **0.83.0** (conservar «Rarotonga»).
2. `CLAUDE.md`: versión 0.82.0→0.83.0, tests 125→**133**, añadir `Services/{Log,TemaApp,IdiomaApp,PaisesOnline,PickerHelper}.cs`.
3. `docs/ANALISIS_TECNICO_WINUI3.md` §11: añadir lo cerrado en F1-F5.
4. `grep -E "0\.8[0-2]\.[0-9]" *.md docs/*.md` → que no queden versiones viejas.
5. `pwsh ./scripts/publish-winui.ps1` (self-contained win-x64); verificar `FileVersion 0.83.0`, datos semilla, `Assets/logo.jpg`, `Documentacion/licencia.txt`.
6. Merge `mejoras-0.83` → `main` **sin borrar la rama** (R6). Tag `v0.83.0`. Release con:
   `& "C:\Program Files\GitHub CLI\gh.exe" release create v0.83.0 --title "..." --notes-file <notas> <zip>` (gh NO está en PATH).

### Opcional / futuro (documentado, no bloquea)
- Extender localización a las 108 páginas (esfuerzo L; menú+toolbar tienen textos hardcodeados en `MainWindow.xaml.cs` que necesitan `ResourceLoader`, no bastan `x:Uid`).
- Fuera por decisión del dueño: C-07 (borrar BoletoControl), N-04 (límite 32767), P-18/P-19 (motor, sin test de igualdad), N-05 (HashSet figuras).

## Reglas vigentes (resumen)
R1 lógica del motor no cambia (125→133 tests verdes). R2 diseño lo decide el dueño. R3 completitud binaria (0/1, con evidencia). R4 sin evidencia no hay hallazgo. R5 un solo `dotnet build` a la vez. R6 no borrar ramas ni cambiar visibilidad. **R7 no abrir la app / smoke / capturas sin permiso del dueño.**
