# Documentación histórica

Estos documentos **ya no describen el estado del proyecto**. Se conservan como registro de las
decisiones y los planes que llevaron hasta la versión actual, pero **no deben usarse como referencia**:
varios describen arquitecturas que nunca se construyeron o una app (WinForms) que hoy es solo
material de consulta.

**La documentación vigente está en la raíz y en [`docs/`](../):**
[`README.md`](../../README.md) · [`CLAUDE.md`](../../CLAUDE.md) ·
[`docs/ANALISIS_TECNICO_WINUI3.md`](../ANALISIS_TECNICO_WINUI3.md) ·
[`docs/MANUAL_USUARIO.md`](../MANUAL_USUARIO.md) · [`docs/MANUAL_FLUJOS.md`](../MANUAL_FLUJOS.md) ·
[`docs/API_CLUBPROGOL.md`](../API_CLUBPROGOL.md) · [`docs/PLAN_MEJORAS.md`](../PLAN_MEJORAS.md).

---

## Era «Web API / móvil» (septiembre 2025) — **nunca ejecutada**

Exploración de convertir Free1X2 en un servicio web con cliente móvil. No se llevó a cabo: el proyecto
siguió siendo una app de escritorio. Las carpetas `Free1X2.Shared/` y `Free1X2.WebAPI/` que quedaron en
el disco son residuos de esta etapa y **no forman parte de `Free1X2.sln`**.

| Documento | Qué era |
|-----------|---------|
| [`FREE1X2_WEBAPI_DEVELOPMENT_PLAN.md`](FREE1X2_WEBAPI_DEVELOPMENT_PLAN.md) | Plan de desarrollo de la Web API. |
| [`FREE1X2_API_REFERENCE.md`](FREE1X2_API_REFERENCE.md) | Referencia de endpoints propuesta. No confundir con [`docs/API_CLUBPROGOL.md`](../API_CLUBPROGOL.md), que sí está viva. |
| [`FREE1X2_MOBILE_INTEGRATION_GUIDE.md`](FREE1X2_MOBILE_INTEGRATION_GUIDE.md) | Guía de integración del cliente móvil. |
| [`FREE1X2_SAFE_IMPLEMENTATION_STRATEGY.md`](FREE1X2_SAFE_IMPLEMENTATION_STRATEGY.md) | Estrategia para extraer el motor sin romperlo. |
| [`FREE1X2_ARCHITECTURE_DIAGRAMS.md`](FREE1X2_ARCHITECTURE_DIAGRAMS.md) | Diagramas de la arquitectura propuesta. |
| [`FREE1X2_COMPREHENSIVE_DOCUMENTATION.md`](FREE1X2_COMPREHENSIVE_DOCUMENTATION.md) | Documentación general del sistema en aquel momento. |
| [`FREE1X2_DEVELOPER_GUIDE.md`](FREE1X2_DEVELOPER_GUIDE.md) | Guía de desarrollo (~36 KB) de la etapa WinForms. |
| [`FREE1X2_FUNCTIONALITY_MAP.md`](FREE1X2_FUNCTIONALITY_MAP.md) | Mapa de funcionalidades por pantalla. |
| [`FREE1X2_DESKTOP_OPTIMIZATION_SUMMARY.md`](FREE1X2_DESKTOP_OPTIMIZATION_SUMMARY.md) · [`FREE1X2_OPTIMIZATION_FINAL_SUMMARY.md`](FREE1X2_OPTIMIZATION_FINAL_SUMMARY.md) | Resúmenes de optimización de aquella etapa. Para el rendimiento **actual** del motor, ver [`docs/PLAN_MEJORAS.md`](../PLAN_MEJORAS.md) §4. |
| [`FREE1X2_PHASE3_COMPLETION_REPORT.md`](FREE1X2_PHASE3_COMPLETION_REPORT.md) · [`BIT_OPERATIONS_FINAL_STATUS.md`](BIT_OPERATIONS_FINAL_STATUS.md) · [`CONTINUED_ITERATION_STATUS.md`](CONTINUED_ITERATION_STATUS.md) · [`FINAL_MIGRATION_STATUS.md`](FINAL_MIGRATION_STATUS.md) · [`MIGRATION_LOG.md`](MIGRATION_LOG.md) | Informes de estado de esa iteración. |
| [`UI_MODERNIZATION_PLAN.md`](UI_MODERNIZATION_PLAN.md) | Primer borrador de modernización de UI, anterior al de 2026. |

## Era «modernización WinForms» (abril–mayo 2026) — **rama paralela, no mergeada**

Esfuerzo de dar aspecto moderno a la UI WinForms (`ModernTheme`, `NeoToolStripRenderer`). Quedó
superado por la migración a WinUI 3, que es lo que hoy está en `main`.

| Documento | Qué era |
|-----------|---------|
| [`PLAN_UI_MODERNIZACION.md`](PLAN_UI_MODERNIZACION.md) | Plan de las 7 fases de modernización WinForms. |
| [`REVISION_UI_HALLAZGOS.md`](REVISION_UI_HALLAZGOS.md) | Hallazgos de la revisión de aquella UI (Bisque/NavajoWhite/Verdana…). |
| [`PLAN_MANUAL_USUARIO.md`](PLAN_MANUAL_USUARIO.md) | Plan para escribir el manual. El manual real acabó siendo [`docs/MANUAL_USUARIO.md`](../MANUAL_USUARIO.md), que no siguió esta estructura. |

## Era «migración a WinUI 3» (mayo 2026) — **ejecutada y completada**

| Documento | Qué era |
|-----------|---------|
| [`PLAN_MIGRACION_WINUI3.md`](PLAN_MIGRACION_WINUI3.md) | Roadmap de la migración (~93 KB), estrategia *strangler-fig* en 8 fases. **Se ejecutó**: el resultado está en `main`. El estado final se resume en [`ESTADO_MIGRACION_WINUI3.md`](../../ESTADO_MIGRACION_WINUI3.md) y el detalle técnico en [`docs/ANALISIS_TECNICO_WINUI3.md`](../ANALISIS_TECNICO_WINUI3.md). |

> También es histórico [`docs/ANALISIS_TECNICO.md`](../ANALISIS_TECNICO.md) (análisis previo a la
> migración), que se conserva en `docs/` junto a su sucesor para poder compararlos.
