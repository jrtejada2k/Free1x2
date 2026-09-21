#| 2026-09-19 | **N-02** Cuelgue de ReductorTM | **Arreglarlo y avisar al usuario** | Salir del bucle en vez de colgarse y mostrar un mensaje explicando que no hay emparejamientos posibles. Los golden-master de ReductorTM deben seguir verdes. |
| 2026-09-19 | **P-20** Bombeo de UI | **Arreglarlo** | `.Milliseconds` → `.TotalMilliseconds` y comprobar cada N columnas con `Environment.TickCount64`. No afecta a resultados; la UI responde de forma regular en análisis largos. |
| 2026-09-19 | **U-04…U-07** Opciones de UI | **Todas** (con énfasis en U-05) | U-05 tamaño mínimo de ventana · U-06 icono en la barra de título · U-04 atajos de teclado (funcionalidad nueva: se propone un juego estándar de Windows y el dueño lo ajusta) · U-07 revisión de recorte a 125/150 % DPI con capturas, arreglando solo donde se recorte de verdad. |
| 2026-09-19 | **C-17 y C-18** Refactors | **Ambos** | C-17: 163 pickers duplicados en 67 ficheros → helper. C-18: cuarteto Guardar/Abrir/Copiar/Pegar en 16 ViewModels (48 métodos) → clase base. Verificación V0 completa tras cada uno. |
| 2026-09-19 | **C-07** Borrar BoletoControl | **NO** | El dueño prefiere no borrar código. Se mantiene; se le añade la desuscripción para cerrar la fuga latente. |
| 2026-09-19 | **N-04** Límite de 32 767 columnas en ReductorTM | **NO subirlo** | Se conserva el comportamiento actual con la comprobación explícita ya añadida. Documentar como limitación conocida. |
| 2026-09-20 | **U-08** FontSize 11 boleto | **Dejar** | Densidad intencional del boleto; no se toca. |
| 2026-09-20 | **U-09** Orden de botones | **Unificar** (HECHO `ed11361`) | Aceptar→Cancelar en ambos diálogos, fiel al original y a la convención Windows. |
| 2026-09-20 | **U-11** Restos de patrón manual | **Dejar ambos** (recomendación) | Convertirlos CAMBIA comportamiento: `LimiteLineas`/`Global` son strings con vacío=«sin límite» (`FormatosFrmViewModel.cs:190-193`), un NumberBox forzaría double; y el ComboBox del boleto era ComboBox en el original (cambiar a AutoSuggestBox arriesga B-03). Choca con la regla 1:1. Se dejan salvo que el dueño pida el cambio a sabiendas. |
| 2026-09-20 | **U-12** Accesibilidad | **Aplicar** (HECHO `58e4844`) | 123 `AutomationProperties.Name` en las 20 páginas que faltaban. |
| 2026-09-20 | **U-10** Localización | **Infraestructura + 1 pantalla piloto** | Montar recursos `.resw` ES/EN + selector de idioma persistido, y migrar UNA pantalla como patrón probado. El resto de las 108 queda para después de que el dueño vea el piloto. |
 Plan de mejoras — Free1X2 WinUI 3 (post v0.82.0 «Rarotonga»)

> **Estado: borrador para tu revisión.** Base: `main` = `e77a5ac`, release `v0.82.0`, fecha 2026-09-16.
> Producido por **5 revisiones independientes en solo lectura** (infra WinUI · 108 páginas portadas · motor `Free1X2.Domain` ·
> XAML/UI · documentación). **No se ha modificado ni una línea de código.** Solo entra aquí lo que tenga **evidencia `file:line`**.
> Nada se ejecuta sin tu ☑ (casillas ☐ en cada ítem).

### Resumen ejecutivo

| Fase | Qué es | Ítems | Lo más grave |
|------|--------|-------|--------------|
| **F1** Bugs | Defectos reales de la capa WinUI | 12 | **B-11** el cálculo múltiple **aborta** (`COMException`) · **B-03** no se pueden teclear nombres compuestos («Real Madrid») · **B-01** bucle de diálogos · **B-05** «jornada fresca» falsa tras fallo de red |
| **F2** Docs | Documentos que contradicen el código | 9 | **D-01** `CLAUDE.md` describe WinForms 0.77.2 · **D-04** 18 enlaces rotos en `MANUAL_FLUJOS.md` |
| **F3** Motor | Rendimiento sin cambiar resultados | 20 + menores | **P-01** filtro reconstruido en **cada** una de los 4 782 969 columnas |
| **F4** Calidad WinUI | Fugas, repintados, duplicación | 29 | **C-12** cero logging en toda la app · **C-01** ~2000 notificaciones por boleto · **C-17** 163 pickers duplicados |
| **F5** UI | **Opciones**, las decides tú | 13 | **U-01** tema oscuro construido y apagado |

**Salud general:** la base es sólida. **0** `.Result`/`.Wait()`, **0** fugas de eventos estáticos en las páginas portadas,
**31/31** `GoBack` con guard, **37/37** destinos de navegación existen, `x:Bind` en el 99 % de los bindings, motor con
125 tests golden-master. Los hallazgos son bordes, no estructura.

**Lo que yo haría primero, si me dices que sí:** C-12 (logging) → **B-11** → B-03 → B-05 → B-06 → B-12 →
D-01/D-02/D-04 → Lote A de F3.
**Lo que NO tocaría sin tu palabra:** todo F5 · P-16/P-19 (riesgo de alterar salidas) · B-10 · C-07 y C-28 (borrar código) ·
D-03 (mover docs) · C-17/C-18 (refactor grande de 67 y 16 ficheros).

## 0. Reglas que gobiernan todo el plan

| # | Regla | Consecuencia práctica |
|---|-------|-----------------------|
| R1 | **La lógica de negocio no cambia.** El motor (`Free1X2.Domain`) debe producir resultados idénticos al WinForms original (`Free1X2\`). | Toda optimización pasa los **125 tests golden-master** (`Free1X2.Domain.Tests`) sin tocarlos. |
| R2 | **Las decisiones de diseño/UI las toma el dueño.** | Los hallazgos de UI son *opciones a considerar*; ninguna se implementa sin ☑ explícita. |
| R3 | **Completitud binaria (0 o 1).** | Un ítem solo se marca hecho con evidencia: build 0 err, smoke 109/109, tests verdes, y `file:line` del cambio. |
| R4 | **Sin evidencia no hay hallazgo.** | Cada fila cita fichero:línea leído. Lo no verificado se descarta. |
| R5 | **Trabajo en agentes de fondo, serial cuando hay build.** | Un solo `dotnet build` a la vez (contención de `obj/`). |
| R6 | **No borrar ramas ni cambiar visibilidad del repo.** | Los residuos documentales se *mueven*, no se borran, salvo orden del dueño. |
| R7 | **No lanzar la app ni capturar pantalla sin permiso explícito del dueño, cada vez.** | Lanzar `Free1X2.WinUI.exe` abre una ventana que **roba el foco**, y las capturas usan ratón y teclado sintéticos que caen sobre lo que el dueño esté haciendo. Ocurrió el 2026-09-19: un agente relanzó la app en bucle sobre un juego a pantalla completa y dejó al dueño sin poder escribir. **Incluye el smoke test.** Antes de ejecutar la app: pedir permiso; al terminar, matar todo proceso `Free1X2.WinUI`. Las verificaciones que no abren ventana (`dotnet build`, `dotnet test`) no necesitan permiso. |

**Verificación estándar (V0)** que cierra cualquier ítem de código:
```powershell
# OJO: -p:Platform=x64 es OBLIGATORIO. Sin el, el build escribe en bin\Debug\ y el smoke
# de abajo se ejecutaria sobre el binario VIEJO de bin\x64\Debug\ -> pasaria en falso.
dotnet build Free1X2.WinUI/Free1X2.WinUI.csproj -c Debug -p:Platform=x64   # 0 errores (13 warnings = baseline)
dotnet test  Free1X2.Domain.Tests/Free1X2.Domain.Tests.csproj              # 125/125
Remove-Item "$env:TEMP\free1x2_smoke.log" -ErrorAction SilentlyContinue
$env:FREE1X2_SMOKE = '1'
.\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe | Out-Null
Remove-Item Env:\FREE1X2_SMOKE
Get-Content "$env:TEMP\free1x2_smoke.log" -Tail 1   # "SMOKE DONE total=109 ok=109 fail=0"
```
> **Por qué el borrado previo del log:** si el smoke no llega a arrancar, sin borrarlo se leería el
> resultado de la ejecución anterior y se daría por bueno. Comprobar además la fecha del `.exe`.

## 1. Fases

| Fase | Contenido | Fuente | Estado | Commit |
|------|-----------|--------|--------|--------|
| F0 | Rama `mejoras-0.83` + baseline V0 (0 err · 125/125 · 109/109) | — | ☑ | `7072cae` |
| F1 | **Bugs** B-01…B-12 + **N-01** (6 clicks muertos en Ayuda) | §3, §7bis | ☑ | `9109823`, `8c9ffb5`, `12cfe8c` |
| F2 | **Documentación** D-01…D-09 (incluido D-03: 20 históricos archivados) | §2 | ☑ | `1586c8f`, `c8946c8`, `af5db88` |
| F3 | **Rendimiento del motor** P-01…P-17 + menores | §4 | ☑ · P-18/P-19 descartados con motivo · P-20 pendiente | `526b05f` |
| F4 | **Calidad WinUI** C-01…C-16, C-22…C-29 | §5 | ☑ · C-17/C-18 no hechos (refactor grande) · C-28 pendiente | `8c9ffb5`, `12cfe8c` |
| F5 | **UI** — U-01 toggle de tema + U-02 (decisión tomada) | §6 | en curso | — |
| F6 | Cierre: bump `0.83.0`, docs, release con zip fresco | §7 | ☐ | — |

**Evidencia acumulada tras F1-F4:** build **0 errores, 0 advertencias** · **131/131** tests
(125 originales **sin modificar** + 6 nuevos de igualdad) · smoke **109/109** · igualdad de salidas del
motor verificada por **SHA-256** contra la DLL publicada v0.82.0.

Orden recomendado: F0 → F1 → F2 → F3 → F4 → F5 (solo lo aprobado) → F6. F1 va primero porque un bug
confirmado pesa más que cualquier optimización; F3 antes que F4 porque toca el motor y necesita la red
de tests intacta antes de refactorizar la capa UI.

---

## 2. Fase F2 — Documentación (hallazgos CONFIRMADOS con evidencia)

### D-01 · 🔴 `CLAUDE.md` describe un proyecto que ya no existe

**Evidencia** (`CLAUDE.md`):
- `:7` `Versión: 0.77.2 "Rarotonga"` → real: `Free1X2.WinUI/Free1X2.WinUI.csproj:17` `<Version>0.82.0</Version>`.
- `:8` `Stack: WinForms .NET 8.0-windows` → real: WinUI 3 / WinAppSDK 1.6 (`Free1X2.WinUI.csproj:20,37`).
- `:10,47` `dotnet build Free1X2/Free1X2.csproj` como build principal → la app es `Free1X2.WinUI/Free1X2.WinUI.csproj`.
- `:14-16,44,149,155-161` rama `ui-modernization` y tags `v0.78.0-ui-modernization-ready` → todo está en `main`, release `v0.82.0`.
- `:32` sección «Lo que falta» (prueba visual, merge a main, manual) → todo hecho (`docs/MANUAL_USUARIO.md`, `docs/ANALISIS_TECNICO_WINUI3.md §11`).
- `:77,132` `ModernTheme.cs` como «TODO el sistema de temas» → es de la capa WinForms legacy, no de la app actual.

**Impacto:** cualquier sesión nueva de Claude Code arranca con contexto falso (build equivocado, rama equivocada, tareas «pendientes» ya cerradas).

**Pasos:**
1. ☐ Reescribir `CLAUDE.md` con la realidad WinUI: qué es, versión desde csproj, estructura (`Free1X2.WinUI`, `Free1X2.Domain`, `Free1X2` legacy = referencia, `Free1X2.Domain.Tests`), comandos V0, smoke `FREE1X2_SMOKE=1`, env `FREE1X2_API_BASE` + `scripts/stub-api.ps1`.
2. ☐ Trasladar las reglas vigentes: R1–R6 de este plan + «Cuota de plan: preguntar antes de tareas masivas» (única regla del CLAUDE.md actual que sigue viva, `:150`).
3. ☐ Eliminar «Estado actual de la rama», «Lo que falta», «Cómo continuar en Windows», «Arquitectura del sistema de temas», «Historial relevante de esta sesión» (todo era del WinForms 2026-04).
4. ☐ Enlazar a `docs/ANALISIS_TECNICO_WINUI3.md`, `docs/API_CLUBPROGOL.md`, `docs/MANUAL_USUARIO.md`.
5. Verificación: leer el fichero completo; cada afirmación debe tener respaldo en código o docs actuales.

**Esfuerzo:** S (1 fichero). **Riesgo:** nulo.

### D-02 · 🔴 `docs/API_CLUBPROGOL.md` contradice el código en «equipos» y no documenta la cache

**Evidencia:**
- `docs/API_CLUBPROGOL.md:126-127` «La app **no consume este endpoint todavía**; queda documentado por si se usa más adelante para el *Gestor de Equipos*» → real: `Free1X2.WinUI/Views/Ported/GestorEquiposFrmViewModel.cs` («Importar equipos online», commit `b9c1044`) y `QuinielaOnlineService.ObtenerEquiposAsync`.
- `docs/API_CLUBPROGOL.md:167` «(Opcional) El catálogo … rellena las listas del *Gestor de Equipos*» → ya implementado, no «opcional».
- `docs/API_CLUBPROGOL.md:18` «Recomendado: `ETag` y/o `actualizado` para que la app cachee» → la app **ya cachea** en `%LocalAppData%\Free1X2\jornada-{es,mx}.json` (`Free1X2.WinUI/Services/JornadaCache.cs`, commit `dd1870f`) y siembra al arranque (`App.xaml.cs` `SembrarJornadaDesdeCache`). No está descrito en «Cómo lo consume la app».

**Pasos:**
1. ☐ §2 «Catálogo de equipos»: sustituir `:125-127` por «La app lo consume en *Gestor de Equipos → Importar equipos online*: descarga la lista `all`, la fusiona sin duplicados en la categoría elegida».
2. ☐ §«Cómo lo consume la app» `:156-167`: añadir punto «Cache: la última jornada por país se guarda en `%LocalAppData%\Free1X2\jornada-{es,mx}.json`; al abrir la app se muestra sin red; el guard anti-doble-petición (<60 s) evita el 429».
3. ☐ Cambiar `:167` de «(Opcional)» a «Implementado».
4. ☐ Añadir nota «Revancha (MX): no disponible en el servidor a 2026-09; el parser ignora campos extra, se extenderá cuando exista».
5. Verificación: contrastar cada frase nueva con `QuinielaOnlineService.cs` / `JornadaCache.cs` (`file:line`).

**Esfuerzo:** S. **Riesgo:** nulo.

### D-03 · 🟠 Residuos documentales en la raíz (19 ficheros de eras anteriores)

**Evidencia** (`git log -1 --date=short`, todos trackeados):

| Fichero | Último commit | Era |
|---------|---------------|-----|
| `BIT_OPERATIONS_FINAL_STATUS.md`, `CONTINUED_ITERATION_STATUS.md`, `FINAL_MIGRATION_STATUS.md`, `MIGRATION_LOG.md`, `UI_MODERNIZATION_PLAN.md` | 2025-09-30 | pre-WinUI |
| `FREE1X2_API_REFERENCE.md`, `FREE1X2_ARCHITECTURE_DIAGRAMS.md`, `FREE1X2_COMPREHENSIVE_DOCUMENTATION.md`, `FREE1X2_DESKTOP_OPTIMIZATION_SUMMARY.md`, `FREE1X2_DEVELOPER_GUIDE.md` (36 KB), `FREE1X2_FUNCTIONALITY_MAP.md`, `FREE1X2_MOBILE_INTEGRATION_GUIDE.md`, `FREE1X2_OPTIMIZATION_FINAL_SUMMARY.md`, `FREE1X2_PHASE3_COMPLETION_REPORT.md`, `FREE1X2_SAFE_IMPLEMENTATION_STRATEGY.md`, `FREE1X2_WEBAPI_DEVELOPMENT_PLAN.md` | 2025-09-30 | plan Web API / móvil nunca ejecutado |
| `PLAN_UI_MODERNIZACION.md`, `PLAN_MANUAL_USUARIO.md` | 2026-04-24 | WinForms modernización |
| `REVISION_UI_HALLAZGOS.md` | 2026-05-28 | WinForms modernización |
| `PLAN_MIGRACION_WINUI3.md` (93 KB) | 2026-05-29 | plan de la migración ya ejecutada |

**Impacto:** la raíz del repo público muestra 24 `.md`; un visitante no distingue lo vigente
(`README.md`, `SECURITY.md`, `ESTADO_MIGRACION_WINUI3.md`, `docs/`) de lo histórico.

**Decisión del dueño: Opción A** (2026-09-16). ✅ **HECHO.**

1. ☑ `git mv` de **20** ficheros (el plan decía 19; el recuento real es 20) a `docs/historico/`. Nada borrado.
2. ☑ Enlaces corregidos: `README.md:12` (sustituido por punteros a `docs/PLAN_MEJORAS.md` y `docs/historico/`),
   `ESTADO_MIGRACION_WINUI3.md:34,35,75`, `docs/ANALISIS_TECNICO.md:99`, `docs/MANUAL_USUARIO.md:513`.
3. ☑ `docs/historico/README.md` escrito, agrupando los 20 por época (Web API/móvil nunca ejecutada ·
   modernización WinForms no mergeada · migración WinUI 3 ejecutada) y avisando de que no son referencia.
4. ☑ Verificado: la raíz queda con `README.md`, `CLAUDE.md`, `SECURITY.md`, `ESTADO_MIGRACION_WINUI3.md`.

### D-04 · 🔴 `docs/MANUAL_FLUJOS.md`: 18 enlaces rotos + habla de la migración como pendiente

**Evidencia** (verificada con Grep + `Test-Path`):
- 18 enlaces con prefijo `../Free1X2/{MotorCalculo,Reduccion,Escrutinio,EntradaSalida,Utils,Analisis,VariablesGlobales}` (líneas 56, 87, 98, 213, 228, 240, 279, 288, 295-297, 302, 309, 322, 325, 328, 333). `Test-Path Free1X2\MotorCalculo` = **False** (ídem Reduccion/Escrutinio/EntradaSalida); `Free1X2.Domain\MotorCalculo` = True.
- `:349` «Para WinUI 3 (rama `winui3-migration`) …» → la rama no existe; todo está en `main`.
- `:354` «Las 111 páginas portadas … el cableado de estos flujos a sus ViewModels es el trabajo pendiente de la migración» → `PortedPages.cs` = **108** páginas; cableado verificado (smoke 109/109, `ANALISIS_TECNICO_WINUI3.md §11`).
- `:358` pie «rama `winui3-migration`».

**Pasos:**
1. ☐ Sustitución en bloque `../Free1X2/` → `../Free1X2.Domain/` en los 18 enlaces; comprobar cada destino con `Test-Path` (los 2 enlaces a `RangosHelper.cs:283` y `UiHooks.cs:341` ya apuntan bien — no tocar).
2. ☐ Reescribir §11 (`:349-354`): migración completada en `main`; 108 páginas + MainPage; los ViewModels ya invocan las entradas de flujo (citar 2-3 ejemplos `file:line`).
3. ☐ Corregir pie `:358`.
4. Verificación: script que extrae todos los `](../…)` del doc y falla si alguno no existe.

**Esfuerzo:** S. **Riesgo:** nulo.

### D-05 · 🔴 `README.md:67-70` sitúa el motor en carpetas que ya no existen

**Evidencia:** `README.md:67-70` lista `MotorCalculo/`, `Reduccion/`, `Escrutinio/`, `EntradaSalida/` como carpetas dentro de `Free1X2/`. `Test-Path` de las 4 en `Free1X2\` = **False**; viven en `Free1X2.Domain/` (MotorCalculo 52 ficheros, EntradaSalida 24, Reduccion 9, Escrutinio 7). En `Free1X2/` solo quedan `Analisis/AnalisisCombinacion.cs` y 4 sueltos en `Utils/` (`Grafico.cs`, `ControlCompatibility.cs`, `ValidadorCaracteres.cs`, `CompresorZip.cs`).

**Pasos:**
1. ☐ Reescribir la tabla de carpetas: motor → `Free1X2.Domain/…`; `Free1X2/` = UI WinForms legacy + remanentes acoplados (listar los 5).
2. ☐ Añadir línea: «`Free1X2/Free1X2.csproj` queda congelado en `0.77.2` por diseño (referencia de comportamiento)» (`Free1X2/Free1X2.csproj:11`).
3. Verificación: cada carpeta nombrada existe (`Test-Path`).

**Esfuerzo:** S. **Riesgo:** nulo.

### D-06 · 🟠 `docs/MANUAL_USUARIO.md` no describe el flujo online real

**Evidencia:**
- `:133` y `:442-443` «Descarga el boleto oficial de una **jornada y temporada** concretas … si el servicio no responde avisa» → real: `DescargaBoletoFrmViewModel.cs:41-48` selector **España/México**, `:94-141` botón **«Actualizar jornada»** con fallback a cache + mensaje con fecha de última actualización.
- `:436-437` Gestión de Equipos sin mención online → real: `GestorEquiposFrmViewModel.cs:149-192` «Importar equipos online» (país, fusión sin duplicados, requiere «Guardar»).
- `:460` «la persistencia del Gestor de Equipos … todavía en proceso de migración» → real: `GestorEquiposFrmViewModel.Guardar()` `:314-331` hace I/O completo; `ANALISIS_TECNICO_WINUI3.md §11` no lo lista como pendiente.

**Pasos:**
1. ☐ Reescribir «Obtener Boletos Online» (`:133`, `:442-443`): elegir país → «Actualizar jornada» → boleto con equipos reales; sin red → última jornada guardada + fecha; mensaje del servidor si 404/429.
2. ☐ Añadir párrafo en Gestión de Equipos (`:436-437`): «Importar equipos online» paso a paso.
3. ☐ Retirar Gestor de Equipos de la nota `:460`; si la nota queda vacía, eliminarla.
4. ☐ **NO** documentar `FREE1X2_API_BASE` (variable de desarrollo; ya correcto).
5. Verificación: recorrer las dos pantallas en la app y contrastar cada frase con la UI real.

**Esfuerzo:** S-M. **Riesgo:** nulo.

### D-07 · 🟠 Versiones residuales `0.78.0` / `0.81.2` en docs vigentes

**Evidencia:**
- `ESTADO_MIGRACION_WINUI3.md:3` «release `v0.81.2`» → real `v0.82.0`.
- `docs/ANALISIS_TECNICO_WINUI3.md:33` tabla de evidencia «Version → 0.78.0 (líneas :15-17)» → `Free1X2.WinUI.csproj:15-17` = `0.82.0`.
- `README.md:105-108` tabla de tags solo `v0.81.x-winui3` y `v0.80.3-winforms` → faltan `v0.81.1-emails-ofuscados`, `v0.81.2`, **`v0.82.0`**.

**Pasos:**
1. ☐ Actualizar los 3 puntos a `0.82.0` (o, en F6, directamente a `0.83.0`).
2. ☐ Regla nueva para F6: «grep `0\.8[0-9]\.[0-9]` en `*.md` antes de cada release».
3. Verificación: `Grep "0\.78\.0|0\.81\.2" --glob *.md` = 0 hits fuera de históricos.

**Esfuerzo:** S. **Riesgo:** nulo.

### D-08 · 🟡 Imagen huérfana `docs/img/winui3-01-inicio.png`

**Evidencia:** existe en disco (`Test-Path` = True); Grep «01-inicio» en `*.md` = 0 hits; README usa 02–10.
**Decisión del dueño:** ☐ añadirla como primera captura «Inicio» en la galería del README · ☐ eliminarla.
**Esfuerzo:** S.

### D-09 · 🟠 `PLAN_UI_MODERNIZACION.md` / `PLAN_MANUAL_USUARIO.md` se presentan como planes activos
Ya cubierto por **D-03** (mover a `docs/historico/`); si se elige Opción B, añadirles la nota de «histórico» como la de `docs/ANALISIS_TECNICO.md:1`.

### Verificado CORRECTO (constancia, Agente 5)
`docs/ANALISIS_TECNICO.md` (nota de obsoleto `:1` presente) · contrato HTTP de `API_CLUBPROGOL.md` (rutas, 404/429/503, 60/min) vs `QuinielaOnlineService.cs` · imágenes referenciadas en README (02–10 + `winui3-shell.png`) existen · conteo 108 coherente en README, README WinUI y ANALISIS_TECNICO_WINUI3 vs `PortedPages.cs` · comandos build/publish y requisito Win10 19041+ coherentes con csproj · `docs/ejemplos-api/*.json`, `scripts/stub-api.ps1`, `scripts/publish-winui.ps1` existen · `LICENSE`, `SECURITY.md` existen.

### Orden de ejecución F2
D-01 → D-02 → D-04 → D-05 → D-06 → D-07 → D-03 (tras decisión) → D-08 (tras decisión). Un commit por documento. Sin build necesario; verificación = scripts de enlaces + lectura completa.

---

## 3. Fase F1 — Bugs de corrección (infra WinUI)

> Los 6 primeros son **defectos de corrección**, no preferencias. Ninguno toca la lógica del motor.
> Cierre de cada uno = **V0** (build + 125/125 + smoke 109/109) más la reproducción manual indicada.

### B-01 · 🔴 Cola de diálogos: posible bucle y pérdida del mensaje
**Evidencia verificada:**
- `Free1X2.WinUI/Services/AppServices.cs:42-57` — `ConfirmarAsync` crea su `ContentDialog` y hace `await dlg.ShowAsync()` **sin tocar `_dialogoAbierto`** ni pasar por la cola.
- `AppServices.cs:77-99` — `private static async void ProcesarColaDialogos()`: `_dialogoAbierto = true;` → `var (titulo, mensaje) = _colaDialogos.Dequeue();` → `await dlg.ShowAsync();` **sin try/catch alrededor del `ShowAsync`**.
- `Free1X2.WinUI/App.xaml.cs:17-21` — `UnhandledException` → `e.Handled = true; MostrarError(...)`.

**Fallo concreto:** con un `ConfirmarAsync` (o cualquier diálogo de página) ya abierto, el `ShowAsync` de la cola lanza
`InvalidOperationException` (WinUI solo admite un `ContentDialog` a la vez). Al ser `async void` la excepción llega a
`UnhandledException` → `MostrarError` → **re-encola** → el `finally` ya dejó `_dialogoAbierto = false` → vuelve a lanzar.
Bucle mientras el otro diálogo siga abierto. Y el mensaje original ya se hizo `Dequeue` → **se pierde**.

**Pasos:**
1. ☐ `ConfirmarAsync` pasa por la misma puerta: respetar/poner `_dialogoAbierto` (o encolarse).
2. ☐ Envolver el `ShowAsync` de `ProcesarColaDialogos` en try/catch: re-encolar al **frente** o registrar en log; nunca dejar escapar.
3. ☐ En `App.UnhandledException`, bandera de corte para no reentrar si el origen es el propio código de diálogos.
4. **Reproducción:** abrir un diálogo de confirmación y, con él abierto, provocar un error (p. ej. cargar un fichero inválido).
**Esf.:** M.

### B-02 · 🔴 Fuga: `AppState.Cambiado` sin desuscripción → VMs zombis
**Evidencia:** `Free1X2.WinUI/Views/MainPageViewModel.cs:113` `_estado.Cambiado += (_, _) => RefrescarPantalla();` —
**lambda**, nunca `-=`. `Views/MainPage.xaml.cs:15,26` `public MainPageViewModel ViewModel { get; } = new();` y
`ViewModel.Navegar = tipo => Frame?.Navigate(tipo);`. No hay `NavigationCacheMode` en todo el proyecto.

**Fallo:** cada vuelta a Inicio crea MainPage+VM nuevos; el singleton `AppState` **retiene todos los VMs anteriores**
(y la propia página, vía la closure `Navegar`). Cada `Cambiado`/`JornadaActual` ejecuta `RefrescarPantalla` en **todos los
zombis** (`Boleto?.CargarDesdeMotor()` + bucle de `Condiciones`). Las escrituras al motor están protegidas por `_cargando`
→ **fuga de memoria + CPU lineal en el nº de visitas**, no corrupción de datos.

**Pasos:** 1. ☐ Handler **nombrado**. 2. ☐ Suscribir en `OnNavigatedTo`, desuscribir en `OnNavigatedFrom` (o `WeakEventListener`).
3. **Reproducción:** ir y volver a Inicio 20 veces; medir memoria y la latencia del refresco.
**Esf.:** S.

### B-03 · 🔴 No se pueden teclear nombres de equipo compuestos
**Evidencia:** `Free1X2.WinUI/Controls/BoletoBaseViewModel.cs:41-50,79-91` — el setter hace
`local = (local ?? "").Trim();` y notifica; `Controls/BoletoBaseControl.xaml:112-126`
`Text="{x:Bind Local, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"`.

**Fallo:** al teclear `"Real "` el setter normaliza a `"Real"` y notifica → el TwoWay reescribe en el ComboBox → **el espacio
desaparece** → no se puede escribir "Real Madrid" a mano.
**Pasos:** 1. ☐ **Reproducir primero** (teclear "Real Madrid" en Local). 2. ☐ Si se confirma: no hacer `Trim()` en cada
pulsación — recortar solo en `DevolverEquipos`/al volcar al motor; o no notificar si el normalizado == entrante; o
`UpdateSourceTrigger=LostFocus`.
**Esf.:** S. **Prioridad alta** si se reproduce: afecta a la entrada manual del boleto.

### B-04 · 🔴 Portapapeles: bloqueo sincrónico + fallo silencioso
**Evidencia:** `Free1X2.WinUI/App.xaml.cs:107-119` — `Clipboard.GetContent()` +
`AsTask(contenido.GetTextAsync()).GetAwaiter().GetResult()` dentro de un `catch { }`. Llamador:
`Free1X2.Domain/Utils/Porcentajes.cs:545`.
**Fallo:** bloqueo sincrónico de una operación WinRT — en hilo UI congela y puede **interbloquear** (formatos con render
diferido); desde hilo de fondo `GetContent()` exige hilo UI → excepción → `catch {}` → devuelve `""` → **el pegado no hace
nada, en silencio**.
**Pasos:** 1. ☐ Hacer el shim `async`, o pre-leer el texto en el comando del VM (hilo UI) y pasarlo al dominio.
2. ☐ Como mínimo, registrar en log en el catch. 3. **Reproducción:** pegar porcentajes desde Excel.
**Esf.:** M.

### B-05 · 🔴 La guarda anti-doble se marca ANTES del GET → «jornada fresca» falsa
**Evidencia verificada:** `Free1X2.WinUI/Services/QuinielaOnlineService.cs:131-135`
```csharp
// Se marca el inicio de la descarga ANTES de salir a la red…
_ultimaDescargaUtc[paisNorm] = DateTime.UtcNow;
string cuerpo = await DescargarConReintentoAsync(url, ct).ConfigureAwait(false);
```
Contraste: `Views/Ported/DescargaBoletoFrmViewModel.cs:87-88,114-115` documenta que «SIEMPRE intenta la red».
**Fallo:** tras un fallo (timeout / 429 / sin red) la marca **queda puesta** → durante 60 s «Actualizar jornada» devuelve la
caché **sin tocar la red** aunque ya haya conexión, y el VM muestra «Jornada N · 14 partidos cargados» **como si fuese fresco**.
**Nota:** el doble clic ya está cubierto por 3 capas (ver «LIMPIO»), así que esta 4ª capa solo aporta el efecto secundario.
**Pasos:** 1. ☐ Marcar **solo tras éxito** (o borrar la marca en el `catch`). 2. ☐ Devolver un flag `desdeCache` para que el
VM redacte el mensaje correcto. 3. **Reproducción:** desconectar red → Actualizar (falla) → reconectar → Actualizar dentro de 60 s.
**Esf.:** S.

### B-06 · 🔴 Excepciones no observadas en acciones de menú/toolbar
**Evidencia:** `Free1X2.WinUI/MainWindow.xaml.cs:543` `_ = paginaViva.ViewModel.EjecutarAccionAsync(accion);` y
`Views/MainPage.xaml.cs:42` `_ = ViewModel.EjecutarAccionAsync(accion);`.
**Fallo:** Task descartada → una excepción en Abrir/Guardar combinación (etc.) **no llega** a `App.UnhandledException`:
la acción falla **en silencio**, sin mensaje al usuario.
**Pasos:** 1. ☐ Wrapper `async void` con try/catch → `AppServices.MostrarError` (o `ContinueWith(OnlyOnFaulted)` en el
scheduler de UI). 2. **Reproducción:** abrir una combinación con un fichero corrupto.
**Esf.:** S.

### B-07 · 🟡 Carrera en el handoff estático productor→visor
**Evidencia:** `App.xaml.cs:127-139` — estáticos asignados en hilo de dominio y después `TryEnqueue(...Navigate...)`;
`Views/Ported/VisorAnalisisColumnasFrmViewModel.cs:112-114` — el ctor hace `UltimoContenedor = null; UltimoGrupo = null;`.
**Fallo:** si `MostrarVisor` se invoca **dos veces** antes de que la UI procese la 1ª, el primer visor consume y anula → el
**segundo abre vacío** (last-wins + consume-and-clear). Además `fe.FindName("ContentFrame")` acopla por string.
**Pasos:** 1. ☐ Mover las dos asignaciones **dentro** de la lambda de `TryEnqueue` (capturando `contenedor`/`grupo`), o pasar
el payload por `Navigate(type, parameter)`. 2. ☐ Exponer `MainWindow.NavegarA(Type)` en vez de `FindName`.
**Esf.:** S.

### B-08 · 🟡 Escritura de caché no atómica
**Evidencia:** `Free1X2.WinUI/Services/JornadaCache.cs:74-75` `Directory.CreateDirectory(Carpeta); File.WriteAllText(ruta, rawJson);`
**Fallo:** un corte a mitad deja JSON truncado → al arrancar se descarta (robusto) pero **se pierde la última caché buena**.
**Pasos:** ☐ escribir a `ruta + ".tmp"` y `File.Move(tmp, ruta, overwrite: true)`. **Esf.:** S.

### B-09 · 🟡 Fallback de versión desactualizado en «Acerca de»
**Evidencia:** `Views/Ported/AcercaDeFrmPage.xaml.cs:38`
`Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.81.2"` — literal obsoleto; además `GetName().Version`
da 4 partes («0.82.0.0»), distinto del `ProductVersion` del legacy.
**Pasos:** ☐ usar `AssemblyInformationalVersionAttribute`/`FileVersionInfo` y **eliminar el literal**. **Esf.:** S.

### B-10 · 🟡 Escritura bajo `AppContext.BaseDirectory` — **decisión del dueño**
**Evidencia:** `App.xaml.cs:69-82` `Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, d))` dentro de `catch {}`;
`GestorEquiposFrmViewModel.cs:34-40,336-348` escribe los `.dat` bajo `BaseDirectory`.
**Hoy:** el zip portable se descomprime donde el usuario quiera → normalmente funciona. Si se instalara bajo `Program Files`,
el directorio es de **solo lectura** → las carpetas no se crean (en silencio) y «Guardar archivos» **falla siempre**.
**Opciones:** ☐ (a) dejarlo (la app es portable por diseño) · ☐ (b) detectar BaseDirectory no escribible y caer a
`%LocalAppData%\Free1X2`, como ya hace `JornadaCache`. **Esf.:** M.

### Revisado y **LIMPIO** (constancia — infra WinUI)
`QuinielaOnlineService`: `HttpClient` único estático (`:76`) con `Timeout = 10 s` (`:83`); 429 con `Retry-After ≤ 3 s` → un solo
reintento, si no fast-fail (`:219-236`); cancelación del llamador distinguida del timeout (`:248-258`); `MensajeDelCuerpo` tolera
cuerpo no-JSON (`:314-317`); **sin `.Result`/`.Wait()`**; `ConfigureAwait(false)` en servicio y `ConfigureAwait(true)` en los VMs
antes de tocar `AppState`/bindings (`DescargaBoletoFrmViewModel.cs:106`, `GestorEquiposFrmViewModel.cs:161`) → **acceso a UI correcto**.
`JornadaCache`: crea la carpeta antes de escribir; caché corrupta → `FormatException` capturada → «sin caché»; `PaisMasReciente`
tolera ficheros inaccesibles. Parsers: `using (doc)` libera el `JsonDocument`; validan 14 partidos/orden/nombres; **no ejecutan nada
del cuerpo**. **Doble clic en «Actualizar jornada»: 3 capas** (`DescargaBoletoFrmPage.xaml.cs:26-34` `IsEnabled=false`,
`AsyncRelayCommand` sin concurrencia, `if (Descargando) return;` en `:97`). `async void` **solo** en event handlers (salvo B-01).
`VisorAnalisisColumnasFrmViewModel:112-114` no retiene el `ContenedorAnalisisGlobal`. `CardElevation.cs:52-53` handler estático
`-=/+=` → sin fuga. `AppState`: eventos en el hilo que asigna; todos los setters actuales se invocan en hilo UI. Tokens de tema
resueltos vía `ThemeDictionaries` (`App.xaml:11`). `PortedPages.cs`: **108** entradas, coincide con el comentario.
`MainWindow.xaml.cs:530` evita re-navegar a la misma página; smoke inerte sin `FREE1X2_SMOKE=1`.

### B-11 · 🔴 Cálculo múltiple: escritura a propiedad enlazada desde hilo de fondo
**Evidencia verificada:** `Views/Ported/CalculaColumnasMultipleFrmViewModel.cs:233` — dentro de
`await Task.Run(() => { … ActualizaColumnasPrevistas(); … }, _cts.Token)`; ese método asigna en `:319` y `:321`:
```csharp
ColumnasMaximas = _colsMaximas.ToString("#,##0;0");
CosteMaximas = costeMaximo.ToString(...);
```
Ambas son `[ObservableProperty]` (`:86-87`) enlazadas **OneWay** en `CalculaColumnasMultipleFrmPage.xaml:147,150`.
**Fallo:** `PropertyChanged` desde hilo de fondo → x:Bind actualiza `TextBlock.Text` fuera del hilo UI →
**`COMException RPC_E_WRONG_THREAD`**, capturada en `:255` → **el cálculo en lote aborta en el primer fichero cuyo valor cambie**.
**Pasos:** 1. ☐ Calcular `_colsMaximas` en fondo; mover las asignaciones `:319-321` al hilo UI tras el `await`
(o `AppServices.UiDispatcher.TryEnqueue`). 2. **Reproducción:** cálculo múltiple con ≥2 ficheros de distinto nº de columnas.
**Esf.:** S. **Nota:** `:229-230` llama `LeeFiltroColumnas()` **dos veces** (la primera se descarta) — limpiar de paso.

### B-12 · 🟠 15+ operaciones de fichero en `Task.Run` sin `try/catch`
**Evidencia:** `GeneraPimViewModel.cs:169,241,281,311,356,396`; `PremiadasFrmViewModel.cs:154,228,273`;
`MejoresOpcionesFrmViewModel.cs:177`; `ExportadorCPsFrmViewModel.cs:93,134`; `GeneradorCPSDiferenciasViewModel.cs:119`;
`BancoPruebasFrmViewModel.cs:1194`; `EscrutiniosFrmViewModel.cs:575-627,891`; `EscrutarCombinacionesFrmViewModel.cs:429-485,696`;
`CombinarFiltrosViewModel.cs:279-323` (tiene `finally` pero no `catch`).
GeneraPim, Premiadas, MejoresOpciones, ExportadorCPs y GeneradorCPSDiferencias tienen **0 `catch` en todo el fichero**.
**Fallo:** cualquier `IOException` (fichero bloqueado, disco lleno, ruta de red caída) llega al handler global →
mensaje genérico, y los indicadores quedan **colgados**: `Estado="Calculando..."` (`EscrutiniosFrmViewModel.cs:561`),
`TiempoTexto` (`:418`), `ColumnasProcesadas` (`GeneraPimViewModel.cs:160-162`), `ProcesadasTexto` (`PremiadasFrmViewModel.cs:151`).
**Pasos:** 1. ☐ Helper `AppServices.EjecutarConErrorAsync(Func<Task>, string contexto)` con try/catch/finally.
2. ☐ Aplicarlo en los 15 sitios (patrón de referencia ya correcto: `CalculaColumnasFrmViewModel.cs:325-361`).
3. ☐ Restablecer los indicadores en `finally`. **Esf.:** M.

### Orden de ejecución F1
B-11 → B-03 (reproducir primero) → B-05 → B-06 → B-12 → B-02 → B-01 → B-07 → B-08 → B-09 → B-04 → B-10 (tras decisión).
Un commit por bug, V0 tras cada uno.

## 4. Fase F3 — Rendimiento del motor

**Impacto**: Alto = dentro del bucle de hasta **4 782 969** columnas · Medio = miles · Bajo = una vez.
**Riesgo** = probabilidad de alterar resultados. **Regla dura (R1):** cierre = `dotnet test` **125/125 sin tocar los tests**.
**Regla extra:** todo ítem de riesgo *Medio* exige, ANTES de aplicarse, un test de igualdad byte a byte
(fichero de salida antes vs después sobre un fixture real de `Free1X2.Domain.Tests/Fixtures`).

### Lote A — riesgo Nulo/Bajo, alto beneficio (recomendado empezar aquí)

| # | Imp. | Riesgo | Fichero:línea | Problema | Optimización | Esf. |
|---|------|--------|---------------|----------|--------------|------|
| P-01 | Alto | Bajo | `MotorCalculo/CondicionIfThen.cs:236,243` | **Verificado:** `CompruebaPronostico(long)` llama `getFiltro(CondIf)` y `getFiltro(CondThen)` **por cada columna** → `new` del filtro + re-parseo del texto + `LlenarTodosValores()` que concatena strings (`FiltroNoVariantes.cs:404-416`). Los campos `filtroIf`/`filtroThen` existen y **no se usan**. | Construir el filtro una vez en los setters de `CondIf`/`CondThen` y reutilizarlo. Supuesto único: `VariablesGlobales.NumeroPartidos` no cambia durante la ejecución. | S |
| P-02 | Alto | Nulo | `MotorCalculo/Diferencia.cs:41-45` | `new bool[17]` + `new bool[17,17]` ×5 (~360 B) por columna. | `Array.Clear` sobre los arrays ya existentes. | S |
| P-03 | Alto | Nulo | `MotorCalculo/FiltroContactos.cs:301,964-992,1073` | `ObtenerFiguraLong()`: `new FiguraCondicion()` + `new List<int>()` + 10 `Add` + `Sort()` por columna; luego `List<long>.Contains` O(n). | Buffer fijo (`Span<int>` de 10 + orden por inserción), instancia `FiguraCondicion` reutilizada, `HashSet<long>` paralelo para el `Contains`. | M |
| P-04 | Alto | Nulo | `MotorCalculo/FiltroSignosSeguidos.cs:216-355,415-442` | Hasta **4** listas + 4 `Sort()` + 4 `Contains` O(n) por columna. Mismo patrón en `Analisis/ContenedorAnalisis.cs:199-232`. | Igual que P-03. El `long` resultante es idéntico. | M |
| P-05 | Alto | Nulo | `MotorCalculo/FiltroFormatos123.cs:125,164,254-263` | `ConvStrToLong(formato.Formato)` por columna y por formato, con un `Substring` por carácter. | Precalcular el `long` en `Formato123` / cachear al asignar `ArrayFormatos`. | S |
| P-06 | Alto | Nulo | `Reduccion/JLPM.cs:103,107,137` → `Utils/UtilColumnas.cs:348-349,308-309,327-328` | `ToCharArray()` ×2 por comparación, dentro de bucles O(n²)/O(n³). | Indexar el string directamente (`ColumnaBase[i] == Columna[i]`). | S |
| P-07 | Alto | Nulo | `Reduccion/JDC.cs:30,99-223` · `JDCdobleContador.cs:31-306` | `ArrayList` → enteros **boxeados**; unboxing en bucles O(n²); `Sort()` sobre boxed. | `List<int>` (mismo orden de `Sort`). | S |
| P-08 | Medio | Nulo | `Utils/UtilColumnas.cs:287-300` (usado en `FiltroNoVariantes.cs:107,111`, `FiltroDibujos.cs:46-47`, `Escrutador.cs:107`) | Popcount por bucle (hasta 14 vueltas), 2× por columna. | `System.Numerics.BitOperations.PopCount` (instrucción hardware en .NET 8). | S |
| P-09 | Medio | Nulo | `MotorCalculo/Grupo.cs:194-201,367-403` | `SonTolGrupoValidas()` recorre 14 filtros aunque `Tolerancias.Count == 0` (caso habitual). | `if (ctrlTolerancias.Tolerancias.Count == 0) return true;` — resultado idéntico (bucle vacío deja `true`). | S |
| P-10 | Medio | Nulo | `MotorCalculo/FiltroPesosNumericos.cs:167-175` | `new FiguraCondicion()` + `new int[10]` por columna. | Reutilizar + `Array.Clear`. | S |
| P-11 | Medio | Nulo | `Reduccion/XFSF.cs:31-32`, `xfsfV3.cs:30-31`, `JDC.cs:31-33`, `JDCdobleContador.cs:32-37`, `Redu1305Xfsf.cs:30-31`, `Analisis/Analizador.cs:36`, `Escrutinio/Escrutador.cs:46-47` | **20–38 MB en LOH** asignados en inicializadores de campo y **reasignados** al conocer `noPartidos`. En `JDCdobleContador.cs:58` un `new BitArray(indices)` se descarta sin asignar. | Inicialización perezosa en `InicializarNumeroDePartidos()`; `List<string>` en `Escrutador`. | S |
| P-12 | Medio | Nulo | `Reduccion/ReductorTM.cs:52-61,118-120` | base3→base10 con `Math.Pow` + `Substring` por dígito. | Tabla de potencias de 3 (int) + comparación de `char`. | S |
| P-13 | Medio | Nulo | `MotorCalculo/FiltroValoracionSignos.cs:82-90` | `switch` sobre string por columna. | `bool esSuma` precalculado en el setter. | S |
| P-14 | Medio | Bajo | `MotorCalculo/Grupo.cs:255` | `FiltrosTemp.Contains(filtro)` O(n) por filtro y columna en modo análisis (~100 comparaciones/columna). | `bool[]`/flag por filtro, limpiado en `FiltrosTemp.Clear()`; se conserva el orden de inserción. | S |
| P-15 | Medio | Nulo | `MotorCalculo/Analizador.cs:116` + `EntradaSalida/ArchivoColumnasTexto.cs:63,67,177` | StringBuilder+string+`Replace` por columna aceptada; buffers de 4 KB por defecto para ficheros de millones de líneas. | `string.Create`/`char[]` reutilizable + `sw.Write(char[],0,n)`; abrir con buffer `1<<16` y **la misma codificación** que `File.CreateText`. | S |

**Pasos del Lote A:** 1. ☐ Rama `perf-motor`. 2. ☐ Un commit por ítem (P-01 primero). 3. ☐ `dotnet test` tras **cada** commit — si un test cae, revertir ese commit y marcar el ítem como no viable. 4. ☐ Medición antes/después con un caso real del dueño (cronometrar un análisis completo de 4,78 M).

### Lote B — requieren validación extra (NO tocar sin decisión)

| # | Imp. | Riesgo | Fichero:línea | Problema | Por qué es delicado |
|---|------|--------|---------------|----------|---------------------|
| P-16 | Alto | **Medio** | `Reduccion/ReductorTM.cs:142-147,190-272` | Listas de adyacencia como **strings CSV**: O(n²) concatenaciones + `Split`/`Convert`/`Replace`/`IndexOf` por vecino + `Sort`+`Reverse`+`IndexOf` en cada vuelta del `while`. Es la optimización de **mayor beneficio** del motor. | El **orden de selección define la salida**. Hay que replicar exactamente `Array.IndexOf(reduceCols, mayor)` (primer índice del máximo), incluido el bucle de `mayor==1`. Exige test de igualdad previo. **Esf. L.** |
| P-17 | Alto | Bajo | `MotorCalculo/RelacionCP1.cs:126-257` | Índices guardados como CSV (`+= i + ","`) y re-parseados con `Split`+`Convert.ToInt32` por columna. | `List<int>[15]` reutilizadas preservando el orden de recorrido. **Esf. M.** |
| P-18 | Alto | Bajo-**Medio** | `ControladorRelacionesCP3.cs:25` → `RelacionCP3.cs:204-503` | `ActualizaValores()` anula y recalcula: `new int[columnas.Count]`, `new int[1,15]`+copias, `Clone()`, `new List<int>()`, `new EscaleraAciertos()` → decenas de allocs por columna con Relaciones III activas. | Riesgo por **complejidad del código**, no por semántica. **Esf. M.** |
| P-19 | Medio | **Medio** | `Escrutinio/EscrutadorComb.cs:42,113-224` | `Hashtable` → boxing de clave y valor por columna. | `Dictionary<int,int>` **cambia el orden de `Keys`**, y ese orden fija `listaEscrutadasConPremio`. Solo viable si la salida se ordena después. **Verificar primero.** |

### P-20 · Medio · Bombeo de UI errático — **decisión del dueño**
**Evidencia verificada** (`MotorCalculo/Analizador.cs:100`):
```csharp
if (DateTime.Now.Subtract(dt1).Milliseconds > 800)
```
`.Milliseconds` es la **componente 0–999**, no `.TotalMilliseconds`. Si pasan 1,2 s la componente vale 200 → **no** bombea;
la UI responde de forma irregular durante análisis largos. Además `DateTime.Now` se llama **4,78 M de veces**.
**No afecta a los resultados**, solo a la fluidez de la UI. **Es así en el original** → cambiarlo es apartarse del 1:1.
**Opciones:** ☐ (a) dejarlo (paridad exacta) · ☐ (b) corregir a `TotalMilliseconds` + comprobar cada N columnas (`(n & 4095) == 0`) con `Environment.TickCount64` → UI más fluida y menos coste.

### Paralelismo — analizado y **descartado** como primer paso
`GeneradorColumnas.GeneraColumnas:205` genera 14 subárboles independientes, **pero** `Analizador→Grupo→IFiltro` es un
grafo **mutable compartido** (contadores por columna) y la escritura de `archivoCols` es secuencial. Paralelizar exigiría
clonar el grafo por hilo y bufferizar la salida por subárbol en orden. **Esf. L, riesgo Medio. No recomendado ahora.**
Obstáculo adicional (solo señalado, no propuesto): `VariablesGlobales` estático leído en constructores
(`FiltroNoVariantes.cs:50`, `FiltroContactos.cs:77`, `ColumnaProbable.cs:27`, `GrupoEquipos.cs:28`, `FiltroFormatos123.cs:30-31`)
y `Grupo.cs:61` (carga XML de configuración en **cada** `new Grupo()`).

### Menores (Bajo, riesgo Nulo) — opcional
`Analisis/Analizador.cs:65,108` `new int[17]` por columna base → `Array.Clear` · `Utils/Comparador.cs:286` `columna1 += "1"`
por columna en carga · `Utils/UtilColumnas.cs:175` recrea la tabla `string[] signos` en cada llamada · `SubirCategoria/Calculos.cs:32,143-158` `ArrayList` boxeado.

### Rutas calientes revisadas y **correctas** (constancia)
`GeneradorColumnas.GeneraColumnas/AnalizaNuevaColumna:199-258,348-364` (solo bit-ops, sin allocs) ·
`Analizador.compruebaPronostico:156-159` y la cadena `ControladorGrupos→ControlGrupos→Grupo.AnalizaColumna` (con memo `reCalcular*`) ·
`AnalizaColumna(long)` de FiltroNoVariantes, FiltroContactos, FiltroSignosSeguidos, FiltroInterrupciones, FiltroDistancias,
FiltroPesosNumericos, FiltroDibujos, FormatoSignos/FormatosSignos, FiltroFormatosSignos, GrupoEquipos, ColumnaProbable,
FiltroSimetrias, FiltroValoracionSignos, RangosOpciones, RelacionGE1, RelacionCP2, CPControlFallos, ControladorTol ·
`Utils/Comparador.cs` (tablas TDif5T) y núcleos de XFSF/XfsfV3/Redu1305Xfsf · `ArchivoColumnasTexto` (streaming, **no** `ReadAllLines`) ·
`SumadorCombinaciones` · **`Online/JornadaQuinielaParser.cs` y `CatalogoEquiposParser.cs`: `JsonDocument` con `using`, sin opciones recreadas, sin serializer reflexivo — limpios** ·
`Escrutador.EscrutaColumna:155-199`.

## 5. Fase F4 — Calidad y rendimiento de la capa WinUI

Nada aquí cambia comportamiento visible; son fugas menores, repintados innecesarios y duplicación.

| # | Sev | Fichero:línea | Problema | Fix | Esf. |
|---|-----|---------------|----------|-----|------|
| C-01 | 🟠 | `Controls/BoletoMatrizViewModel.cs:112-144` | `foreach (var ap in Apuestas) ap.Limpiar();` (4 sets × 2 notificaciones) y acto seguido se reasignan los 4 → hasta **~2000 `PropertyChanged` por boleto** (8 col × 16 filas × doble pasada) con `x:Bind` en 128 filas, **en cada paso de boleto**. | Quitar la pasada `Limpiar`; en el `else` poner `Es1=EsX=Es2=false; FilaVisible=false`. | S |
| C-02 | 🟠 | `Controls/GraficoLineasControl.xaml.cs:174-178,247-255` | `new SolidColorBrush(color)` por cada línea de rejilla/eje **en cada `Redibujar`** (y también en cada `SizeChanged`). | Cachear brushes por color. | S |
| C-03 | 🟠 | `Controls/GraficoLineasControl.xaml.cs:92-95` | `nuevo.CollectionChanged += control.Curvas_CollectionChanged` **sin `-=`** en `Unloaded`: la colección es del VM → retiene el control si el VM sobrevive. | Desenganchar en `Unloaded`. | S |
| C-04 | 🟠 | `Services/QuinielaOnlineService.cs:120-126` | `JornadaCache.TryCargar(...)` **antes del primer `await`** → `File.ReadAllText` + `JsonDocument.Parse` **sincrónicos en el hilo UI**. | En la guarda comprobar solo `File.GetLastWriteTimeUtc`; leer el cuerpo tras un `Task.Run`/yield. | S |
| C-05 | 🟠 | `Views/Ported/DescargaBoletoFrmViewModel.cs:113,121,147-154` | Tras **cada** éxito se relee y re-parsea el JSON completo (hilo UI) **solo para obtener el timestamp**; en el fallback se vuelve a parsear lo que el servicio ya pudo haber parseado. | `JornadaCache.FechaGuardado(pais)` (solo `GetLastWriteTimeUtc`), o que el servicio devuelva `(jornada, guardadoUtc)`. | S |
| C-06 | 🟡 | `Views/Ported/GestorEquiposFrmViewModel.cs:292-300,360` vs `:195-201,208-218` | **Dedup inconsistente**: el alta manual/mover usa `destino.Contains(NuevoNombre)` (**case-sensitive**) mientras la importación online usa `ContieneEquipo` (OrdinalIgnoreCase + Trim) → «Real Madrid» y «real madrid» pueden convivir. Además `ContieneEquipo` es O(n·m) con `Trim()` por comparación. Y `var destino = NuevaCategoria switch {...}` duplica `ListaDeCategoria`. | Un **único** comparador; usar `ListaDeCategoria` en `NuevoEquipo`; `HashSet<string>(OrdinalIgnoreCase)` construido una vez por importación. | S |
| C-07 | 🟡 | `Controls/BoletoViewModel.cs:85,147-163` + `Controls/BoletoControl.xaml.cs:12` | `AppState.Instancia.JornadaCambiada += OnJornadaCambiada;` **sin `-=`**. Pero `BoletoControl` **no aparece en ningún XAML** (solo un comentario en `BoletoFrmPage.xaml.cs:16`; MainPage usa `BoletoBaseControl`) → **código muerto con fuga latente**. Contiene además equipos de muestra hardcodeados. | ☐ eliminar `BoletoControl`/`BoletoViewModel` · ☐ o añadir desuscripción (`Unloaded`/`IDisposable`). **Decisión del dueño** (borrar código). | S |
| C-08 | 🟡 | `Services/QuinielaOnlineService.cs:44,73,76` + `DescargaBoletoFrmViewModel.cs:32`, `GestorEquiposFrmViewModel.cs:73` | Todo el estado es `static` (`_http`, `_ultimaDescargaUtc`) pero **cada VM hace `new QuinielaOnlineService()`**: singleton de facto disfrazado de instancia. | `static class` o instancia única compartida. | S |
| C-09 | 🟡 | `Services/QuinielaOnlineService.cs:271-286` | `LeerRetryAfter` solo trata `ra.Delta` y enteros → un `Retry-After` en **formato HTTP-date** se ignora y cae al genérico «~60 s». | Contemplar `ra.Date` (`ra.Date.Value - DateTimeOffset.UtcNow`). | S |
| C-10 | 🟡 | `DescargaBoletoFrmViewModel.cs:35-45,117` vs `GestorEquiposFrmViewModel.cs:76-89,180` | `sealed record OpcionPais` + lista `Paises` **duplicados** en dos VMs. El filtro `catch (Exception ex) when (ex is QuinielaOnlineException \|\| ex is not OperationCanceledException)` es **redundante** (la 1ª cláusula está subsumida por la 2ª). | `Services/PaisesOnline.cs` (record + lista estática); filtro `when (ex is not OperationCanceledException)`. | S |
| C-11 | 🟡 | `MainWindow.xaml.cs:360-391` vs `:395-419`; `:181-204` vs `:208-217` | `Herramienta`/`HerramientaAccion` repiten ~20 líneas (Image+Button+ToolTip+Automation); `Menu` reimplementa lo que ya hace `ItemFlyout`. | `CrearBotonBarra(icono, tooltip)`; `Menu` delega en `ItemFlyout`. | S |

### C-12 · 🟡 **Sin logging en ninguna parte** (transversal)
**Evidencia:** `App.xaml.cs:17-21` (`e.Handled = true` para **toda** excepción), `:57-60,81,117`;
`MainWindow.xaml.cs:57,525,563,607`; `JornadaCache.cs:77-80,107-113,140-143`; `AcercaDeFrmPage.xaml.cs:47-51,124-127`;
`AppServices.cs:68` (`if (disp is null) return;`) — **todos los `catch { }` del alcance sin traza alguna**.
**Consecuencia:** la app sigue en estado desconocido tras cualquier excepción y **no queda rastro**; antes de
`AppServices.Inicializar` cualquier error desaparece sin más. Cuando reportes un fallo, no hay nada que mirar.
**Propuesta:** logger mínimo a `%LocalAppData%\Free1X2\log.txt` (con rotación simple) escrito desde `UnhandledException` y
desde los catches; `Handled = true` **solo** para tipos recuperables.
**Esf.:** M. **Recomendación:** hacerlo **antes** que F1, así los bugs B-01…B-10 dejan rastro al reproducirlos.

### Escaneo de las 108 páginas portadas (215 `.cs`, 108 `.xaml`)

**Rendimiento**

| # | Sev | Fichero:línea | Problema | Fix | Esf. |
|---|-----|---------------|----------|-----|------|
| C-13 | 🟠 | `EscrutiniosFrmViewModel.cs:759,912-927` · `EscrutarCombinacionesFrmViewModel.cs:488-500,720-730` · `BancoPruebasFrmViewModel.cs:1092-1096` · `ColumnasPremiadasFrmPage.xaml.cs:40-44` · `PremiadasFrmViewModel.cs:287-290` · `AnaCombiViewModel.cs:173,185-188` | `X.Clear(); foreach … X.Add(...)` sobre `ObservableCollection` **enlazadas a ListView** (`EscrutiniosFrmPage.xaml:397,488`, `EscrutarCombinacionesFrmPage.xaml:225,279`, `BancoPruebasFrmPage.xaml:345`, `ColumnasPremiadasFrmPage.xaml:71`, `AnaCombiPage.xaml:181`) → **un `CollectionChanged` por ítem** con cientos/miles de premiadas. | Construir `List<T>` en el hilo de fondo y **sustituir la colección completa** (propiedad `IReadOnlyList<T>` + `OnPropertyChanged`), o una `ObservableCollection` con `AddRange` que emita un único `Reset`. | M |
| C-14 | 🟠 | `GenerarCPsViewModel.cs:287-304` | `txt += …` dentro de `for i (Columns.Count) × for j (NumeroPartidos)` → **O(n²) en memoria** por cada tipo de CP. | `StringBuilder` con la misma secuencia de `Append` (salida idéntica). | S |
| C-15 | 🟠 | `AnaCombiViewModel.cs:282-294` (`Contabiliza`) | `xcol += columna[_grup[nr]]` ×14 por **cada línea** del fichero (`:234-249`) → millones de strings. | `stackalloc char[14]` / `string.Create` → mismo `S14N(xcol)`. | S |
| C-16 | 🟡 | `AnalisisFormatos123FrmViewModel.cs:60,173-174` | `ObservableCollection<string> Columnas` con **todas** las columnas del fichero, pero **no está enlazada** (el XAML solo tiene un `TextBlock` literal, `:24`) → notificaciones sin oyentes. | `List<string>`. | S |

**Duplicación** (nada urgente; reduce mantenimiento)

| # | Fichero:línea | Problema | Fix | Esf. |
|---|---------------|----------|-----|------|
| C-17 | 67 ficheros — p. ej. `ContactosFrmViewModel.cs:277-286,303-308`, `DistanciasFrmViewModel.cs:210-219,236-241`, `EscrutiniosFrmViewModel.cs:880-889` | **163** `new FileOpenPicker/FileSavePicker` + **322** `InitializeWithWindow.Initialize(picker, AppServices.WindowHandle)` → ~800-1000 líneas de boilerplate idéntico. | `PickerHelper.AbrirAsync(params string[] ext)` / `GuardarAsync(nombre, (etiqueta, ext)[])` en `Services`. | M |
| C-18 | 16 VM de filtros (`Contactos:274-344` ≡ `Distancias:207-274`; también Dibujos, Diferencias, FigurasFiltros, Formatos, Formatos123, GruposEquipos, IfThen, Interrupciones, NoVariantes, PesosNum, Simetrias, SignosSeguidos, Valoracion, Modificador) | Cuarteto `Guardar/Abrir/Copiar/Pegar` idéntico salvo extensión y nombre sugerido = **48 métodos duplicados**. | Clase base `FiltroArchivoViewModelBase(ext, nombre)`; `GuardarEn/AbrirDesde` siguen en cada VM. | M |
| C-19 | `GraficoColumnasFrmPage.xaml.cs:63-127` · `ImprimirBoletoFrmPage.xaml.cs:188-208` · `TramificarGraficasFrmPage.xaml.cs:78-100` · `VerBoletosEnEditorFrmPage.xaml.cs:112-132` | `GuardarComoPngAsync` ×4 idéntico y `CopiarImagenAsync` ×4 (`paquete.SetBitmap` en `:88/:120/:61/:93`). | `ImagenHelper.CopiarYGuardarAsync(UIElement, nombre)`; cada página fija su `EstadoTexto`. | S |
| C-20 | 27 páginas: `Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };` + ~25 `Navegar = tipo => Frame?.Navigate(tipo)` (p. ej. `TramificarFormPage.xaml.cs:23-24`) | Lambda idéntica en 27 constructores; el `bool` de `Navigate` se ignora en los **47** sitios. | Extensiones `Page.VolverSiPuede()` / `Page.NavegarA(Type)` con log si devuelve `false`. | S |
| C-21 | `aidomnouViewModel.cs:699` · `FiltroPorcenJBViewModel.cs:569` · `GeneraPimViewModel.cs:730` | `static string Cambia(int)` ×3 (dos `switch`, uno `if/else`) con la misma tabla 0→"-",1→"1",2→"2",3→"12",4→"X"… | Mover a `Free1X2.Shared` / `SignosHelper`. | S |

**Coherencia**

| # | Sev | Fichero:línea | Problema | Fix | Esf. |
|---|-----|---------------|----------|-----|------|
| C-22 | 🟡 | `ColProbablesFrmPage.xaml.cs:97-109` · `CalculaColumnasMultipleFrmViewModel.cs:367-383` · `ImportExportFrmViewModel.cs:205-221` | Tres `ContentDialog` Sí/No propios pese a existir `AppServices.ConfirmarAsync` (`Services/AppServices.cs:42-57`). **Y la semántica headless es opuesta**: las copias devuelven `true` (`:370`, `:208`), el helper devuelve `false` (`:45`). | `ConfirmarAsync(mensaje, titulo, textoSi, textoNo, valorHeadless)` y usarlo en los 3. **Relacionado con B-01.** | S |
| C-23 | 🟡 | `CalculaColumnasFrmViewModel.cs:302,348` · `ReductorFrmViewModel.cs:228` · `EscrutiniosFrmViewModel.cs:536,541,901` (21 hits / 7 ficheros) | `Free1X2.Abstractions.UserDialogs.Show*` conviviendo con `AppServices.MostrarError/Info` (177+ hits) → **dos APIs de mensaje**. | Unificar en una sola vía. | S |
| C-24 | 🟡 | `AcercaDeFrmPage.xaml.cs:48,124` · `ConfiguracionAnalisisFrmViewModel.cs:216` · `ConfiguracionFrmViewModel.cs:151` · `DescargaBoletoFrmViewModel.cs:79` · `EstimadorPremiosFrmViewModel.cs:422` · `FrmReducidasPerfectasViewModel.cs:257` · `FrmDependenciaLinealViewModel.cs:315` | 8 `catch { /* comentario */ }` sin variable ni traza. (De 212 catches totales, **0** son `catch (Exception) { }` vacío — el resto fija fallback o `return`.) | `catch (Exception ex) { Debug.WriteLine(ex); }` manteniendo el fallback. **Depende de C-12.** | S |
| C-25 | 🟡 | `AnaCombiViewModel.cs:177` · `CoincidenciasViewModel.cs:486` | `DispatcherQueue dispatcher = AppServices.UiDispatcher!;` — si es null (headless/tests) → NRE dentro del `Task.Run` → mensaje engañoso «Error al calcular: Object reference…». | Fallback como `OrdenarPorProbabilidadFrmViewModel.cs:337-342` (`if (disp is null) { acción directa }`). | S |
| C-26 | 🟡 | 16 `TryEnqueue` sin comprobar retorno (`AnaCombi:255,265`, `BancoPruebas:587`, `Coincidencias:541`, `CombinarFiltros:292,316`, `DiFiltros:306,312,328,333,378,443`, `MejoresOpciones:193`, `OrdenarPorProbabilidad:341`, `Rentabilidad:334`, `TramificarForm:1212`) | Devuelve `false` con la cola cerrada (cierre de app). Impacto bajo. | Centralizar en `AppServices.EnUi(Action)` que loguee el `false` (ya existe `EnUi` local en `BancoPruebasFrmViewModel.cs:583-588`). | S |
| C-27 | 🟡 | `ColumnasPremiadasFrmViewModel.cs:71` · `SubirCategoriaFrmViewModel.cs:331` | `public async void` en VM (no son handlers), llamados desde code-behind. Hoy protegidos por try/catch interno. | Devolver `Task` y hacer `await` en el handler `async void` de la página. | S |
| C-28 | 🟡 | `GenerarCPsViewModel.cs:125` | `fichero.Replace(" ", "_");` — **resultado descartado**, sentencia muerta (el comentario reconoce que replica al legacy). | ☐ dejar (paridad 1:1 con el legacy) · ☐ eliminar y anotar. **Decisión del dueño.** | S |
| C-29 | 🟡 | `AyudaFrmPage.xaml.cs:35,42,48,54,62,68` · `ColProbablesFrmViewModel.cs:23` · `ConfiguracionAnalisisFrmPage.xaml.cs:22` · `CreditosFrmPage.xaml.cs:18` · `EscrutiniosFrmViewModel.cs:78` · `TramificarGraficasFrmViewModel.cs:21` | **12 `TODO` restantes** en 6 ficheros. | Revisar uno a uno: cerrar, convertir en nota de diseño o borrar si ya no aplica. | S |

### Revisado y **LIMPIO** (constancia — 108 páginas portadas)
**`.Result` / `.Wait()` / `GetAwaiter().GetResult()`: 0 hits.** · **`#pragma warning disable`: 0.** ·
**LINQ en getters enlazados: 0** (3 regex distintas). · **Suscripciones a estáticos (`AppState`/`VariablesGlobales`/`UiHooks`/`AppServices`) sin `-=`: 0**;
las 14 suscripciones Page→VM son sobre VM `{ get; } = new()` por página → mueren con la página; único `-=` necesario ya está
(`BancoPruebasFrmPage.xaml.cs:59`). · **Timers parados en `finally`** (`CalculaColumnas:352`, `CalculaColumnasMultiple:261`, `Reductor:232`). ·
**Navegación: 31/31 `GoBack` con guard `CanGoBack`**; los **37 tipos destino** de los 47 `Navigate` **existen todos** en `PortedPages.cs`. ·
**`{Binding}` solo 16** (vs 2111 `x:Bind`) y **todos** son `Command="{Binding ViewModel.X, ElementName=PageRoot}"` dentro de `DataTemplate` (patrón legítimo). ·
`async void`: 19 + 4 lambdas, **15 son handlers `_Click`**, 2 `OnNavigatedTo`, y los 2 de VM (C-27) están protegidos por try/catch interno. ·
`Task.Run` con UI: **un solo caso real** (B-11); el resto usa `Disp?.TryEnqueue`/`EnUi`/snapshots previos correctamente.

## 6. Fase F5 — UI: opciones para que decida el dueño (R2)

> **NINGUNA de estas se implementa sin tu ☑.** Son hallazgos con evidencia, no decisiones.
> Varias son **funcionalidad nueva** (no existía en el WinForms original) → marcadas **[NUEVO]**: aceptarlas
> significa apartarse del 1:1. Las marcadas **[PARIDAD]** corrigen algo que el original sí tenía o que el
> propio sistema de temas ya construido espera.

### Estado de partida (verificado, para constancia)
Bueno de serie: `x:Bind` **2181** vs `{Binding}` 17 (y esos 17 son `ElementName` deliberados) · **963**
`AutomationProperties.Name` en 85/108 páginas · **0** botones solo-icono sin nombre · `NumberBox` nativo ya en
40 páginas (173 usos) · virtualización correcta (`ListView`/`ItemsRepeater` donde hay listas; los 41 `ItemsControl`
con `StackPanel` son listas fijas de 14-16 ítems) · estilos ya centralizados en `Themes/Styles.xaml` (sin duplicados
inline ≥3). **No hay deuda estructural de XAML.**

### U-01 · Alto · [PARIDAD] Tema oscuro construido pero desactivado
**Evidencia:** `Free1X2.WinUI/App.xaml:6` `RequestedTheme="Light"` fijo, mientras `Themes/Tokens.xaml:66-104`
define la paleta **Dark completa**.
**Hoy:** el usuario con Windows en modo oscuro ve la app siempre clara; el trabajo de tokens oscuros no se usa.
**Opciones:** ☐ (a) quitar `RequestedTheme` → sigue el tema del sistema · ☐ (b) añadir toggle Claro/Oscuro/Sistema en el menú **Ver** · ☐ (c) dejar fijo en claro (decisión consciente, documentarla).
**Esfuerzo:** S (a) / M (b). **Nota:** si se elige (a) o (b), U-02 pasa a ser obligatorio.

### U-02 · Medio · [PARIDAD] Colores literales fuera del sistema de tokens
**Evidencia:** `Themes/Styles.xaml:202,210` `Foreground="White"` literal en `FiltroButtonActivoStyle`/`FiltroButtonErrorStyle`
— y **ya existe** el token `AppOnSuccess` sin usar en `Tokens.xaml:30`.
**Hoy:** en tema oscuro/alto contraste ese blanco no se adapta.
**Opciones:** ☐ usar `AppOnSuccessBrush`/`AppOnErrorBrush` · ☐ dejarlo.
**Esfuerzo:** S. Solo relevante si U-01 = (a)/(b).

### U-03 · Medio · Swatches de leyenda con colores fijos
**Evidencia:** `Views/Ported/TramificarGraficasFrmPage.xaml:79-113` — 8 `Background="Black/DodgerBlue/Brown/Cyan/Green/Red/CadetBlue/ForestGreen/Orange"`.
**Hoy:** son colores **de datos** (deben coincidir con las curvas de `GraficoLineasControl`), no de tema.
**Pasos:** 1. ☐ Verificar que coinciden 1:1 con las curvas dibujadas. 2. ☐ Si coinciden: añadir comentario «colores de datos, intencionalmente fuera del tema» y cerrar. 3. ☐ Si no coinciden: **es un bug** → mover a F1.
**Esfuerzo:** S.

### U-04 · Alto · [NUEVO] Sin atajos de teclado
**Evidencia:** `Grep KeyboardAccelerator` en `Free1X2.WinUI` = **0 hits**. Contraste verificado: el WinForms original
**tampoco tenía** (`Free1X2/UI/MainForm.Designer.cs` sin `ShortcutKeys`; único hit en todo `Free1X2/UI` es
`Modern/ModernMainForm.cs:186`, de la rama de modernización abandonada).
**Hoy:** ~90 comandos de menú, todos solo con ratón. **Es paridad exacta con el original.**
**Opción:** ☐ añadir `KeyboardAccelerator` a las acciones frecuentes (¿cuáles? — lo decides tú: Guardar, Nueva combinación, Calcular, F1 Ayuda…) · ☐ no añadir (mantener 1:1).
**Esfuerzo:** M. **Si sí:** necesito que me digas la lista exacta tecla→acción.

### U-05 · Medio · Ventana sin tamaño mínimo
**Evidencia:** `MainWindow.xaml.cs:51-58` solo `AppWindow.Resize(1020,760)`; sin `MinWidth`/`MinHeight`.
La toolbar tiene ~55 botones a 2 filas (`MainWindow.xaml:32`, `MinHeight="63"`).
**Hoy:** al encoger mucho la ventana no hay tope; la toolbar se recorta.
**Opción:** ☐ `OverlappedPresenter` con `PreferredMinimumWidth/Height` (¿qué mínimo? p. ej. 900×600 — decides tú) · ☐ dejarlo.
**Esfuerzo:** S.

### U-06 · Medio · Icono de ventana
**Evidencia:** sin `AppWindow.SetIcon` en `MainWindow.xaml.cs`; el `.ico` sí está incrustado en el exe (`Free1X2.WinUI.csproj:10` `<ApplicationIcon>Assets\app.ico`).
**Hoy:** el exe tiene icono correcto en el Explorador; la barra de título de la ventana puede mostrar el genérico de WinUI.
**Pasos:** 1. ☐ Comprobar en la app corriendo si el titlebar ya muestra el icono. 2. ☐ Si no: `AppWindow.SetIcon`.
**Esfuerzo:** S.

### U-07 · Bajo · 19 páginas sin `ScrollViewer` propio — verificar a DPI alto
**Evidencia:** 19/108 sin `ScrollViewer`; **8** tienen `ListView`/`DataGrid` con scroll interno → sin riesgo.
Quedan **11** a verificar: `AnaCombiPage`, `AnalizarFicheroFrmPage`, `AnastaticsPage`, `CambioPuntosFrmPage`,
`ColumnasPremiadasFrmPage`, `CombinarFiltrosPage`, `ControlTolFrmPage`, `CrearGruposFrmPage`, `DescargaBoletoFrmPage`,
`DialogoFiltrarPorLimitesFrmPage`, `GEPTFrmPage`, `ListaImpresorasPage`, `ListadoCondicionesFrmPage`,
`MejoresOpcionesFrmPage`, `PremiadasFrmPage`, `ReductorFrmPage`, `ResultadosCalculoMultipleFrmPage`,
`VerBoletosEnEditorFrmPage`, `VisorEstadisticasPage`.
**Hoy:** con filas `Height="*"` a **150 % DPI** o fuente de sistema grande el contenido podría recortarse sin aviso.
**Pasos:** 1. ☐ Pasada de capturas a 100 %, 125 % y 150 % de las 11 páginas. 2. ☐ Solo donde se recorte realmente: envolver en `ScrollViewer` (fix puntual, sin rediseño).
**Esfuerzo:** M (la verificación; el fix es S por página).

### U-08 · Bajo · `FontSize="11"` (por debajo de 12)
**Evidencia:** 24 hits en 4 ficheros: `Controls/BoletoBaseControl.xaml:60-72` (cabeceras Nº/Local/Visitante/1/X/2) y `:105-147`,
`BoletoControl.xaml`, `BoletoMatrizControl.xaml`, `Views/Ported/EscrutiniosFrmPage.xaml:102`.
**Hoy:** densidad alta — probablemente **réplica 1:1** del boleto WinForms (filas de 26 px).
**Opción:** ☐ subir a 12 donde quepa · ☐ dejar como está y documentar «densidad intencional del boleto».
**Esfuerzo:** S. **Recomendación:** es tu decisión de diseño; el boleto imita papel real.

### U-09 · Alto · Orden de botones inconsistente en diálogos
**Evidencia:** `Views/Ported/DialogoAnalisisMultipleDeTramosFrmPage.xaml:405-411` = **Cancelar, Aceptar**;
`DialogoGrabarBancoPruebasFrmPage.xaml:88-94` = **Aceptar, Cancelar** (y este segundo sin `HorizontalAlignment="Right"`).
**Pasos:** 1. ☐ Contrastar con los diálogos WinForms originales (si allí también diferían, es 1:1 fiel → cerrar). 2. ☐ Si en el original eran iguales: unificar al orden del original. 3. ☐ Barrido al resto de `Dialogo*FrmPage`.
**Esfuerzo:** S. **Nota:** el orden lo dicta el original, no una convención mía.

### U-10 · Medio · [NUEVO] Localización: `en-US.lang` heredado sin conectar
**Evidencia:** 0 hits de `x:Uid`/`.resw` en las 108 páginas; todo el texto en español hardcodeado.
Existe `Datos/Idioma/en-US.lang` (del WinForms) **no conectado a nada** en WinUI.
**Opciones:** ☐ (a) dejarlo como está (la app es en español) · ☐ (b) retirar `en-US.lang` del paquete si no se va a usar · ☐ (c) retomar localización → extraer a `.resw` (**L**, 108 páginas).
**Esfuerzo:** S (a,b) / L (c). **Recomendación:** (a) o (b); (c) es un proyecto aparte.

### U-11 · Bajo · Dos restos de patrón manual
- `Views/Ported/FormatosFrmPage.xaml:114,122` `TextBox InputScope="Number"` → único resto frente a `NumberBox` usado en 40 páginas. ☐ unificar.
- `Controls/BoletoBaseControl.xaml:112-126` `ComboBox IsEditable="True"` (Local/Visitante) → `AutoSuggestBox` daría filtrado incremental. ☐ evaluar · ☐ dejar (el original era ComboBox).
**Esfuerzo:** S / M.

### U-12 · Bajo · `AutomationProperties.Name` ausente en 20 páginas
**Evidencia:** 20 ficheros con `<Button>` y 0 `AutomationProperties.Name`; verificado en 3 de ellos que **todos los botones tienen `Content` de texto legible** → el lector de pantalla sí anuncia. Es **inconsistencia**, no bloqueo.
**Opción:** ☐ añadirlos por consistencia con las otras 85 páginas · ☐ dejarlo.
**Esfuerzo:** S (mecánico).

### U-13 · Bajo · Fondo blanco fijo en host de exportación
**Evidencia:** `Views/Ported/VerBoletosEnEditorFrmPage.xaml:70-79` `Border x:Name="HostExportacion" Background="White" Opacity="0"`.
**Hoy:** parece **intencional** (fondo «papel» del PNG exportado), no un descuido de tema.
**Pasos:** ☐ confirmar y añadir comentario en el XAML.
**Esfuerzo:** S.

### Resumen F5
Lo único que yo llamaría **defecto** y no preferencia: **U-01** (tema oscuro construido y apagado) y **U-03** si los
colores de leyenda no casan con las curvas. **U-04** (atajos) y **U-10c** (localización) son **funcionalidad nueva** —
el original no los tenía. El resto son detalles de consistencia. Ninguna toca la lógica.

## 7. Fase F6 — Cierre

1. ☐ Bump `Free1X2.WinUI.csproj` `AssemblyVersion/FileVersion/Version` → `0.83.0`.
2. ☐ Actualizar `docs/ANALISIS_TECNICO_WINUI3.md §11` («Arreglado en esta auditoría») con los ítems cerrados.
3. ☐ V0 completo + pasada UIA (0 crashes) como en v0.82.0.
4. ☐ `dotnet publish` self-contained win-x64, verificar `FileVersion`, datos semilla, `Assets/logo.jpg`, `Documentacion/licencia.txt`.
5. ☐ Tag `v0.83.0` + Release con `& "C:\Program Files\GitHub CLI\gh.exe" release create …` (gh no está en PATH).

---

## 7 bis. Hallazgos NUEVOS surgidos al ejecutar el plan

Aparecieron al trabajar, no estaban en la revisión inicial. Cada uno con evidencia verificada.

### N-01 · 🔴 RESUELTO — 6 clicks muertos en la pantalla *Ayuda*
**Evidencia:** `Free1X2.WinUI/Views/Ported/AyudaFrmPage.xaml.cs` — los handlers `ManualLink_Click`,
`ArticulosLink_Click`, `RecursosLink_Click`, `ForoLink_Click`, `NotificacionesLink_Click` y
`FacebookLink_Click` tenían **cuerpo vacío** (solo un `TODO`). El legacy `Free1X2/UI/AyudaFrm.cs:21-54`
mostraba un `MessageBox` informativo en cada uno.
**Fallo:** el usuario pulsaba 6 de las 7 opciones de la pantalla y **no ocurría nada**.
**Estado:** ☑ arreglado (replican el mensaje del original). Es exactamente el tipo de «pantalla
desconectada» que el dueño marcó como inadmisible, y la revisión inicial no lo detectó porque
buscaba `Frame.Navigate` ausentes, no handlers vacíos.
**Lección para el futuro:** un escaneo de *handlers de evento con cuerpo vacío o solo comentarios*
en las 108 páginas encontraría más casos como éste. **Pendiente de decisión del dueño** si se hace.

### N-02 · 🔴 `ReductorTM` se cuelga (bucle infinito) con `diferencia == 1`
**Evidencia:** `Free1X2.Domain/Reduccion/ReductorTM.cs` — en `Reduce`, si ninguna columna se empareja,
`menor = Array.IndexOf(matrizTemporal, 0)` devuelve **-1**, el `for (i = 0; i < menor)` no itera y el
`while (mayor != 0)` **nunca termina**. **Preexistente**: reproducido con la DLL publicada v0.82.0, así
que no lo introdujo ninguna optimización. Por eso los golden-master de `ReductorTM` usan niveles
8/10/11/12 y no 13.
**Fallo:** la app se queda colgada (no cierra, no responde) al reducir con ese parámetro.
**Decisión del dueño:** ☐ (a) arreglarlo (salir del bucle y avisar al usuario) · ☐ (b) dejarlo como
está por paridad 1:1 con el original · ☐ (c) primero quiero reproducirlo yo.
**Nota:** una app que se cuelga es peor que una que avisa; recomiendo (a), pero es tu llamada.

### N-03 · 🟠 `ReductorTM.ComienzaReduccion` no llama a `Inicializa`
**Evidencia:** `Free1X2.Domain/Reduccion/ReductorTM.cs` — `diferencia` queda en 0 si el llamante no
invoca `Inicializa` antes. La UI legacy sí lo hacía (`Free1X2/UI/ReductorFrm.cs:618-619`). **Preexistente.**
**Riesgo:** si alguna ruta de la UI WinUI no replica esa llamada, la reducción corre con `diferencia = 0`.
**Pasos:** 1. ☐ Comprobar en `Free1X2.WinUI/Views/Ported/ReductorFrmViewModel.cs` si llama a `Inicializa`.
2. ☐ Si no lo hace, es un bug de comportamiento → arreglar. 3. ☐ Si lo hace, documentar la precondición.

### N-04 · 🟡 `ReductorTM.Reduce` tiene un límite duro de 32 767 columnas
**Evidencia:** los `Convert.ToInt16` sobre índices desbordan por encima de 32 767 columnas
(`OverflowException`). **Preexistente**, conservado a propósito en la optimización P-16 para no cambiar
el comportamiento; ahora con una comprobación explícita en vez de un desbordamiento crudo.
**Decisión:** ☐ subir el límite a `int` · ☐ dejarlo y documentarlo en el manual.

### N-05 · 🟡 `HashSet` paralelo a `Figuras` no es implementable con seguridad (P-03/P-04)
**Evidencia:** `Free1X2.WinUI/Views/Ported/FigurasFiltrosFrmViewModel.cs:129-146` hace `Clear()` + `Add()`
**sobre la misma referencia de lista** que tiene el filtro (handoff documentado en
`ContactosFrmViewModel.cs:190-193`), **sin pasar por el setter**. Una edición que deje el mismo `Count`
con contenido distinto desincronizaría el set en silencio.
**Estado:** el resto de P-03/P-04 sí se aplicó (buffers sin asignaciones); solo el `HashSet` queda fuera.
Requeriría un hook desde la capa WinUI. **Ganancia restante pequeña; no recomiendo tocarlo.**

### N-06 · 🟡 V0 del plan permitía un smoke en falso — RESUELTO
`dotnet build -c Debug` **sin** `-p:Platform=x64` escribe en `bin\Debug\`, no en `bind\Debug\`, que es
de donde el smoke coge el `.exe`. Un smoke podía pasar sobre un binario viejo. ☑ Corregido en §0
(añadido `-p:Platform=x64`, borrado previo del log y comprobación de la fecha del `.exe`).

## 7 ter. Trabajo restante — plan de ejecución

Estado a 2026-09-19. Todo lo de abajo está **decidido** por el dueño; lo que falta es ejecutarlo.
Las tandas están ordenadas para que **nada requiera abrir la app hasta la tanda 4**.

### Tanda 1 — Motor (no abre ninguna ventana) · ☑ HECHA (commit `078b10d`)

| # | Qué | Fichero | Verificación |
|---|-----|---------|--------------|
| **N-02** | Cuelgue de `ReductorTM` con `diferencia == 1`: `menor = Array.IndexOf(matrizTemporal, 0)` devuelve `-1`, el `for` no itera y el `while (mayor != 0)` no termina nunca. Salir del bucle y avisar al usuario de que no hay emparejamientos posibles. | `Free1X2.Domain/Reduccion/ReductorTM.cs` | Los 6 golden-master de ReductorTM/RelacionCP1 **deben seguir verdes** + un test nuevo que fije el caso `diferencia == 1` (antes: cuelgue; después: mensaje). |
| **P-20** | Bombeo de UI errático: `Analizador.cs:100` usa `.Milliseconds` (componente 0-999) en vez de `.TotalMilliseconds`, así que si pasan 1,2 s la componente vale 200 y **no** bombea. Además llama a `DateTime.Now` 4,78 M de veces. Corregir y comprobar cada N columnas con `Environment.TickCount64`. | `Free1X2.Domain/MotorCalculo/Analizador.cs:100` | 131/131. No altera resultados, solo la cadencia del refresco. |
| **N-03** | Comprobar si `Free1X2.WinUI/Views/Ported/ReductorFrmViewModel.cs` llama a `Inicializa` antes de `ComienzaReduccion` (la UI legacy sí lo hacía, `Free1X2/UI/ReductorFrm.cs:618-619`). Si no lo hace, `diferencia` queda en 0 → **es un bug de comportamiento**. | `ReductorFrmViewModel.cs` | Si hay que arreglarlo: golden-master del caso. |

**Cierre de tanda:** `dotnet test` 131+/131+ y `dotnet build` 0 errores. **Sin smoke** (no hace falta abrir la app para cambios de Domain cubiertos por tests).

### Tanda 2 — Refactor C-17: pickers de fichero (no abre ninguna ventana) · ☑ HECHA (commit `f69fdea`, 168 pickers)

163 `new FileOpenPicker`/`FileSavePicker` + 322 `InitializeWithWindow.Initialize(picker, AppServices.WindowHandle)`
repartidos por **67 ficheros** (~800-1000 líneas de *boilerplate* idéntico).

1. Crear `Free1X2.WinUI/Services/PickerHelper.cs`: `AbrirAsync(params string[] extensiones)` y
   `GuardarAsync(string nombreSugerido, params (string etiqueta, string extension)[] tipos)`, encapsulando
   el `InitializeWithWindow` y devolviendo `StorageFile?`.
2. Sustituir sitio por sitio, **en tandas de ~10 ficheros**, compilando entre tandas. Conservar
   **exactamente** las extensiones, el nombre sugerido y la ubicación inicial de cada picker: un cambio ahí
   altera lo que el usuario ve en el diálogo.
3. **No** cambiar el flujo posterior (qué se hace con el fichero elegido).

**Cierre:** build 0 errores · 131/131 · **el smoke queda para la tanda 4**.

### Tanda 3 — Refactor C-18: cuarteto de los filtros (no abre ninguna ventana) · ☑ HECHA (commit `d273395`, 12/16)

`Guardar`/`Abrir`/`Copiar`/`Pegar` idénticos salvo extensión y nombre sugerido en **16 ViewModels** de filtro
(**48 métodos**): Contactos, Distancias, Dibujos, Diferencias, FigurasFiltros, Formatos, Formatos123,
GruposEquipos, IfThen, Interrupciones, NoVariantes, PesosNum, Simetrias, SignosSeguidos, Valoracion, Modificador.
Referencia de equivalencia: `ContactosFrmViewModel.cs:274-344` ≡ `DistanciasFrmViewModel.cs:207-274`.

1. Clase base `FiltroArchivoViewModelBase(extension, nombreSugerido)` con los 4 comandos, apoyada en el
   `PickerHelper` de la tanda 2. `GuardarEn`/`AbrirDesde` (lo específico de cada filtro) **siguen en cada VM**
   como métodos abstractos.
2. Migrar **de a un ViewModel**, compilando después de cada uno. Cualquier diferencia real entre dos VMs
   (aunque parezca un despiste) se **conserva** y se anota: puede ser intencional del original.

**Cierre:** build 0 errores · 131/131.

### Tanda 4 — Verificación con la app abierta · ⏸ ESPERANDO PERMISO DEL DUEÑO (R7)
> **U-03 y U-13 ya verificados en estático** (sin abrir la app): los colores de la leyenda de `TramificarGraficasFrmPage` coinciden 1:1 con `ColorDeCurva` (son datos, no tema); el `Background="White"` de `HostExportacion` es el fondo «papel» del PNG exportado. Ambos correctos, no deben seguir el tema.

Aquí es donde se abre `Free1X2.WinUI.exe`. **No se ejecuta nada de esto hasta que el dueño diga que puede.**
Conviene hacerlo todo de una vez, en una sola ventana de tiempo en que no esté usando el ordenador.

1. **Smoke** de las tandas 1-3: `SMOKE DONE total=109 ok=109 fail=0`.
2. **U-01 pendiente de verificar en ejecución** (el código ya está escrito y compila): que el submenú
   **Ver → Tema** cambie el tema **en vivo**, que «Sistema» siga a Windows, y que la elección **persista**
   tras cerrar y reabrir (comprobar `%LocalAppData%\Free1X2\tema.json`).
3. **U-02/U-03/U-13** en tema oscuro: que no quede texto ilegible; veredicto sobre los 8 *swatches* de
   leyenda de `TramificarGraficasFrmPage.xaml:79-113` (¿coinciden con las curvas → son colores de datos?) y
   sobre el `Background="White"` de `HostExportacion` en `VerBoletosEnEditorFrmPage.xaml:70-79` (¿es el
   fondo «papel» del PNG exportado?).
4. **U-07**: capturas a **100 %, 125 % y 150 %** de las 19 páginas sin `ScrollViewer` propio
   (`AnaCombiPage`, `AnalizarFicheroFrmPage`, `AnastaticsPage`, `CambioPuntosFrmPage`, `ColumnasPremiadasFrmPage`,
   `CombinarFiltrosPage`, `ControlTolFrmPage`, `CrearGruposFrmPage`, `DescargaBoletoFrmPage`,
   `DialogoFiltrarPorLimitesFrmPage`, `GEPTFrmPage`, `ListaImpresorasPage`, `ListadoCondicionesFrmPage`,
   `MejoresOpcionesFrmPage`, `PremiadasFrmPage`, `ReductorFrmPage`, `ResultadosCalculoMultipleFrmPage`,
   `VerBoletosEnEditorFrmPage`, `VisorEstadisticasPage`) — 8 de ellas tienen scroll interno propio, así que
   el riesgo real está en las otras 11. **Arreglar solo donde se recorte de verdad.**
5. **U-06**: comprobar si la barra de título ya muestra el icono de la app; si no, `AppWindow.SetIcon`.
6. **Al terminar: matar todo proceso `Free1X2.WinUI`.**

### Tanda 5 — U-05 y U-04 · ☑ IMPLEMENTADA (commit `e1c561e`; falta comprobar en vivo → tanda 4)

- **U-05 tamaño mínimo de ventana** (el dueño lo destacó). Hoy `MainWindow.xaml.cs` solo hace
  `AppWindow.Resize(1020,760)`, sin tope: al encoger, la barra de ~55 botones a 2 filas se recorta.
  Implementar con `OverlappedPresenter.PreferredMinimumWidth/Height`. **Falta que el dueño diga el mínimo**
  (propuesta: **900 × 600**; se aplica salvo que diga otro).
- **U-04 atajos de teclado.** Es **funcionalidad nueva**: el WinForms original **no tenía ninguno**
  (`Free1X2/UI/MainForm.Designer.cs` sin `ShortcutKeys`). Propuesta de juego mínimo y estándar de Windows,
  **a confirmar o cambiar por el dueño** antes de implementar:

  | Atajo | Acción |
  |-------|--------|
  | `Ctrl+N` | Nueva combinación |
  | `Ctrl+O` | Abrir combinación |
  | `Ctrl+S` | Guardar combinación |
  | `F5` | Calcular / Analizar |
  | `F1` | Ayuda |
  | `Esc` | Volver (donde hoy hay botón Volver/Cancelar) |

  Se añaden como `KeyboardAccelerator` en el `MenuBar`, sin tocar la disposición.

### Tanda 6 — F6 Cierre

1. Bump `Free1X2.WinUI.csproj` `AssemblyVersion`/`FileVersion`/`Version` → **`0.83.0`** (el nombre «Rarotonga» se conserva).
2. Comprobar que no quedan versiones viejas en los `.md`.
3. Actualizar `docs/ANALISIS_TECNICO_WINUI3.md` §11 con lo cerrado en F1-F5 y las cifras nuevas.
4. Actualizar `CLAUDE.md` (versión, conteo de tests, `Services/TemaApp.cs`, `Services/Log.cs`, `PickerHelper`).
5. V0 completo **con permiso (R7)** + `dotnet publish` self-contained win-x64; verificar `FileVersion`,
   datos semilla, `Assets/logo.jpg`, `Documentacion/licencia.txt`.
6. Merge de `mejoras-0.83` a `main` (**sin borrar la rama**, R6), tag `v0.83.0` y Release adjuntando el zip.
   Recordatorio: `gh` **no** está en el PATH, hay que llamarlo por su ruta completa.

### Lo que queda FUERA por decisión expresa del dueño

| Ítem | Motivo |
|------|--------|
| **C-07** borrar `BoletoControl`/`BoletoViewModel` | El dueño prefiere no borrar código. Se mantiene; solo se le añade la desuscripción del evento para cerrar la fuga latente. |
| **N-04** subir el límite de 32 767 columnas de `ReductorTM` | Se conserva el comportamiento actual, con la comprobación explícita ya añadida. Documentar como limitación conocida. |
| **P-18** buffers de `RelacionCP3` | Sus getters públicos devuelven los buffers internos: reutilizarlos aliasearía estado a los consumidores. Sin test de igualdad fiable → no se toca (regla del dueño). |
| **P-19** `Hashtable` → `Dictionary` en `EscrutadorComb` | `Keys.CopyTo` recorre los buckets **en orden inverso** y ese orden fija la lista que la UI muestra **sin ordenar**. Cambiarlo alteraría lo que ve el usuario. |
| **N-05** `HashSet` paralelo a `Figuras` | `FigurasFiltrosFrmViewModel.cs:129-146` muta la lista **por referencia** sin pasar por el setter: el set se desincronizaría en silencio. Ganancia restante pequeña. |
| **U-08…U-12** (FontSize 11 del boleto, orden de botones en 2 diálogos, localización, `AutomationProperties` en 20 páginas, `NumberBox`/`AutoSuggestBox`) | No decididos aún; quedan documentados en §6 a la espera de que el dueño los quiera o no. |

## 8. Registro de decisiones del dueño

| Fecha | Ítem | Decisión | Nota |
|-------|------|----------|------|
| 2026-09-16 | **U-01** Tema oscuro | **Toggle Claro/Oscuro/Sistema en el menú Ver** | Quitar `RequestedTheme="Light"` de `App.xaml:6`, añadir entrada en el menú **Ver** con las 3 opciones y **persistir la elección**. Arrastra **U-02** (los `Foreground="White"` literales de `Themes/Styles.xaml:202,210` pasan a los tokens `AppOnSuccess`/`AppOnError`) → obligatorio. Revisar también U-03 (swatches de leyenda) y U-13 (fondo de exportación) para que no rompan en oscuro. |
| 2026-09-16 | **D-03** Docs residuales | **Mover los 19 `.md` a `docs/historico/`** (Opción A) | `git mv`, más `docs/historico/README.md` con una línea por fichero. **No se borra nada.** Después: grep de enlaces rotos en todos los `.md`. La raíz queda con `README.md`, `CLAUDE.md`, `SECURITY.md`, `ESTADO_MIGRACION_WINUI3.md`. |
| 2026-09-16 | **F3** Alcance del motor | **Lote A + Lote B, con tests de igualdad previos** | Lote A (P-01…P-15, riesgo nulo/bajo) directo con los 125 tests. Lote B (**P-16** ReductorTM, **P-17** RelacionCP1, **P-18** RelacionCP3, **P-19** EscrutadorComb): **primero** escribir tests de igualdad byte a byte sobre ficheros reales de `Free1X2.Domain.Tests/Fixtures`, y solo entonces optimizar. Si un test de igualdad no se puede construir para un ítem, ese ítem **no se toca**. |
| 2026-09-16 | **P-20** Bombeo de UI | *(pendiente)* | Se decide al llegar a F3: dejar `.Milliseconds` (paridad exacta con el original) o corregir a `TotalMilliseconds` para que la UI responda de forma regular en análisis largos. |
