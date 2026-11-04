# Tareas para la app móvil: arreglos que ya tiene Windows

**Para quien porte Free1X2 al móvil (Android / iOS).** Los arreglos de Windows del 1 y 2-nov-2026 están detallados para
la Mac en [`TAREAS_MAC.md`](TAREAS_MAC.md) (M1–M8, issues #17–#25). Este documento dice qué hay hoy en el móvil y qué
debe llevar desde el principio cuando tenga el motor y las pantallas de Free1X2.

## 0. Estado del móvil (3-nov-2026)

En este repositorio no hay app móvil: el proyecto Mac es solo macOS (`Free1X2.Mac/project.yml`), y ninguna rama de
`origin` tiene un proyecto Android o iOS. En el PC del dueño hay dos proyectos .NET MAUI aparte, y **ninguno tiene el
motor ni las pantallas de condiciones o filtros**:

| Proyecto | Dónde | Qué hace |
|---|---|---|
| **QuinielaGol1X2** | repo [`jrtejada2k/QuinielaGol1X2`](https://github.com/jrtejada2k/QuinielaGol1X2) (`net9.0-android;net9.0-ios`) | Cliente de una API REST: Login, Dashboard, Teams, Statistics, Analysis, Settings (`AppShell.xaml:26-57`). La API es `localhost:5001/api/` (`Services/ApiService.cs:13-15`) y no está incluida. Sin referencia a `Free1X2.Domain`. |
| **Free1X2MobileApp** | carpeta local, sin git, de 30-sep-2025 | Versión anterior del mismo cliente (las mismas pantallas más Register y Profile, `ApiBaseUrl` `https://localhost:5001` en `appsettings.json:3`). |

«Analysis» del móvil es un buscador de partidos con filtros de liga y fecha (`Views/AnalysisPage.xaml:30-81`), no las
condiciones de la Quiniela. Por eso **hoy no hay nada que corregir en el móvil**: ninguno de los fallos M1–M8 existe allí.

## 1. Si el móvil porta el motor y las pantallas

Lo más seguro es **referenciar `Free1X2.Domain`** (es .NET y no depende de la interfaz, ver `Abstractions/UiHooks.cs`)
en vez de reescribirlo. Así M2 (Pim), M3 (línea en blanco) y M4 (tramos) vienen ya arreglados, junto con sus tests. Lo
que hay que cuidar está en las pantallas: **copiar el comportamiento de Windows actual (desde `4ba81fb`), no el del port
anterior ni el de la Mac antes de M1–M8.**

Lista de comprobación, una línea por arreglo (detalle, `file:line` y pruebas en `TAREAS_MAC.md`):

| # | Debe hacer | Referencia Windows |
|---|---|---|
| M1 | «Calcular» desde cualquier sitio usa el boleto, las condiciones y el filtro de Inicio | `5bf0d52` |
| M2 | Filtro Pim con el algoritmo del original (7 rankings, recorrido = máx − mín) | `d411343`, `Free1X2.Domain/Analisis/FiltroPim.cs` |
| M3 | Las líneas en blanco del filtro de columnas no son columnas | `80fd72d` (motor) |
| M4 | Fraccionador por tramos: el último tramo llega hasta el final | `80fd72d`, `Free1X2.Domain/Utils/FraccionTramos.cs` |
| M5 | Contactos: fila vacía = 0…14; Aceptar valida; «Borrar» pone un filtro nuevo inactivo. Simetrías avisa si la entrada no vale. Distancias 0–14. Figuras sin límite de 6 caracteres | `4b882b7` |
| M6 | Volver de una pantalla auxiliar (Saca formatos, Analizador, editor de figuras…) conserva lo editado. Formatos 123 para en la primera fila no válida y el Analizador usa lo guardado. Valoración avisa con columna corta. Análisis de signos: Pleno y aspiración según el fichero, `%` con tabuladores, rejilla en `%` | `f97317a`, `Free1X2.WinUI/Services/EdicionPendiente.cs` |
| M7 | Columnas probables: pronóstico 1/X/2 **por partido** (dobles y triples) y rangos `10-12`. Grupos de equipos: nombres del boleto y ayuda correcta. If-Then: aviso si se piden más condiciones que relaciones. Tríos con «Columna Ganadora». Dependencia lineal relee la entrada | `ec6683e` |
| M8 | Filtrar por límites se aplica y trae las Difs. del original. Control de tolerancias guarda solo filas con Tolerancia y Aciertos. Listado con If-Then desactivado. GEPT con cajas G/E/P. Separador JB con la rejilla 14×7 | `a6f50cc` |

En una pantalla táctil conviene pensar el diseño de M6 (pila de navegación) y M7 (14 × 3 botones) desde el principio, pero
la decisión de diseño es del dueño (R2).

**Pendiente de decisión del dueño, igual que en Windows y Mac:** si se aceptan rangos `2-6` en Variantes, Distancias,
Contactos y Pesos (#21), y si If-Then debe sumarse a las demás condiciones en vez de sustituirlas (#16). La lista de lo
que viene del original y no se ha cambiado está en [`pruebas/filtros/README.md`](pruebas/filtros/README.md).

## 2. Cómo comprobarlo

- Los tests de `Free1X2.Domain.Tests` (209) si se referencia el motor.
- Las pruebas de la app de [`PRUEBAS_WINDOWS.md`](PRUEBAS_WINDOWS.md) (I4, F5, I5, O4) y las de filtros de
  [`pruebas/filtros/`](pruebas/filtros/README.md), con lo esperado de su README. Los verificadores
  `scripts/pruebas_windows.py` y `scripts/pruebas_filtros_*.py` preparan las entradas y comprueban las salidas.
