# ESPEC 15 — Diseño de la interfaz (sistema de diseño, ventana y mockups)

> Cierra H-12 de [`REVISION_SPECS.md`](REVISION_SPECS.md). Decisiones de producto: DP-05, DP-10, DP-11 y DP-12 ([ESPEC_11](ESPEC_11_ARQUITECTURA_SWIFT.md) §2):
> **la estructura es la de la app actual** (una ventana, 7 menús, barra de 6 grupos, páginas apiladas con «Atrás»), traducida a
> convenciones nativas de macOS. Las «Áreas Mac» y «Presentación» propuestas en 08 §3.1, 09 §2.1 y 10 §3 quedan **sustituidas** por este documento.
> Las medidas exactas de cada pantalla se toman de su `*.xaml` actual al implementarla (M5).

## 1. Sistema de diseño

### 1.1 Colores (Asset Catalog, variante Any/Dark; valores de hoy: `Free1X2.WinUI/Themes/Tokens.xaml:14-116`, 10 §1.5)

| Color Set | Claro | Oscuro | Uso |
|-----------|-------|--------|-----|
| `AccentColor` | `#3D5A80` | `#818CF8` | acento de la app (como hoy, no el del sistema) |
| `Fondo` · `Superficie` · `SuperficieAlt` | `#F4F6F8` · `#FFFFFF` · `#E6ECF2` | `#0B1120` · `#0F172A` · `#1E293B` | fondo de ventana · tarjetas · cabeceras de rejilla |
| `Borde` | `#C9D4DF` | `#334155` | bordes de tarjeta y rejilla |
| `Texto` · `TextoSecundario` | `#293241` · `#5C6B7A` | `#F1F5F9` · `#94A3B8` | |
| `Exito` · `Aviso` · `Error` | `#2E7D32` · `#EE6C4D` · `#C0392B` | `#16A34A` · `#F59E0B` · `#EF4444` | semáforo, botón de condición, mensajes |
| `Secundario` · `Terciario` | `#2A9D8F` · `#EE6C4D` | `#2DD4BF` · `#FB7185` | gráficos y series |
| `SignoMarcado` (+ hover/pressed) | `#2E7D32` (`#357A38`/`#27692B`) | `Exito` oscuro | signos 1/X/2 marcados del boleto |

Tema: `Sistema` (defecto) · `Claro` · `Oscuro` aplicado con `NSApp.appearance` (`nil`/`.aqua`/`.darkAqua`), en vivo, como hoy (DP-12).

### 1.2 Tipografía, espaciado y forma

| Token | Valor | Origen |
|-------|-------|--------|
| Fuente | fuente del sistema (SF Pro) en tamaños de macOS: cuerpo 13 pt, secundario 11 pt, título de página 17 pt semibold | sustituye a `Segoe UI Variable` (10:206) |
| Columnas, signos y matrices | `.monospacedDigit()` / `.monospaced()` | `MatrizAnalisisView` monoespaciada (10:284) |
| Radio | 8 (controles) · 12 (tarjetas) | 10:206 |
| Tarjeta | fondo `Superficie`, borde 1 `Borde`, radio 12, relleno 10×7, sombra suave | `AppCardStyle` (10:207) |
| Densidad | compacta: `controlSize(.small)` en rejillas, `.regular` en formularios | 10:206 |
| Separación | 4 / 8 / 12 / 16 pt | — |

### 1.3 Controles (WinUI → SwiftUI/AppKit)

| Hoy (WinUI) | Mac | Nota |
|-------------|-----|------|
| `Button` | `Button` (`.bordered`; el principal de cada pantalla `.borderedProminent` + `keyboardShortcut(.defaultAction)`) | |
| `ToggleButton` de signo 1/X/2 | `Toggle` con `.toggleStyle(.button)` y `SignoMarcado` | 10:278 |
| Botón de condición de 4 estados | `BotonCondicion(estado:)` Inactivo (neutro) · Neutro (borde y texto `Aviso`) · Activo (`Exito`) · Error (`Error`) | `FiltroButton*Style` (10:208-210) |
| `SemaforoControl` | `SemaforoView` (círculo 14 pt + halo 22 pt; etiqueta accesible) | 10:282 |
| `TextBox` | `TextField` (sin recorte al escribir; se recorta al volcar, B-03) | 10:278 |
| `NumberBox` | `TextField(value:format:)` con `FormatoNET` (vacío ⇒ `NaN` como hoy, tratado según T13) | 08 §1.5 |
| `ComboBox` editable | `TextField` + `.textInputSuggestions` / `Picker(.menu)` si no es editable | |
| `ListView`/`DataGrid` | `Table` (ordenable solo donde hoy lo es) | |
| `TreeView` (árbol de fallos, listado de condiciones) | `OutlineGroup`/`List(children:)` | |
| `ProgressBar` | `ProgressView(value:)` | |
| Lienzo de gráfico | Swift Charts | 10:283 |
| `ContentDialog` | `.alert` / `.confirmationDialog` (cola de `Alertas`) | 10:229-235 |
| Selectores de fichero | `NSOpenPanel`/`NSSavePanel` (carpeta inicial Documentos) | 10:237-239 |

### 1.4 Iconos

Los iconos actuales son PNG de 16×16 px (`Free1X2.WinUI/Assets/Toolbar/*.png`, 56 ficheros) y se verían borrosos en pantallas Retina.
**DT-50:** se sustituyen por **SF Symbols** con el mismo texto de ayuda (tooltip = nombre accesible) y en la misma posición. Mapa propuesto
(se valida en M5 con la app SF Symbols; un símbolo inexistente se cambia por el más cercano):

| Grupo | Botón → SF Symbol |
|-------|-------------------|
| Free | Salir `rectangle.portrait.and.arrow.right` · Configuración `gearshape` · Configurar análisis `slider.horizontal.3` · Ayuda `questionmark.circle` · Acerca de `info.circle` |
| Archivo | Guardar equipos `person.2.badge.gearshape` · Nueva combinación `doc.badge.plus` · Abrir combinación `folder` · Guardar combinación `square.and.arrow.down` · Guardar como `square.and.arrow.down.on.square` · Borrar temporales `trash` · Abrir equipos `person.2` · Borrar informes `doc.badge.ellipsis` · Obtener boletos online `icloud.and.arrow.down` · Gestión de equipos `person.3` |
| Operaciones | Álgebra `plus.forwardslash.minus` · Transposición `arrow.left.arrow.right` · Multiplicación `multiply` · Fraccionar `square.split.2x2` · Rotación de signos `arrow.triangle.2.circlepath` |
| Combinación | Calcular `play.fill` · Calcular múltiples `play.square.stack` · Ver boletos `list.bullet.rectangle` · Imprimir boletos `printer` · Reducir `arrow.down.right.and.arrow.up.left` · Escrutinio `checkmark.seal` · Escrutar combinaciones (deshabilitado, como hoy) `checkmark.seal.fill` · Análisis de columnas `tablecells` · Análisis de fallos `exclamationmark.triangle` · Análisis de signos `textformat.123` · Probabilidades `percent` · Estadísticas `chart.bar` · Añadir pleno al 15 `15.circle` |
| Filtros | Combinar `line.3.horizontal.decrease.circle` · Diferencias de filtros `minus.circle` · Coincidencias `equal.circle` · Aidomnou `6.circle` · Pim `p.circle` |
| Utilidades | Subir categoría `arrow.up.circle` · Modificador % `slider.horizontal.below.square.and.square.filled` · Generador de CPs `wand.and.stars` · Diferencias entre columnas `arrow.left.and.right.square` · Ordenar por probabilidad `arrow.up.arrow.down` · Selector JuanM `j.circle` · Selector MarioSan `m.circle` · Rentabilidad `eurosign.circle` · Columnas GEPT `g.circle` · Tramificar `chart.bar.xaxis` · Premiadas `trophy` · Estimación de premios `chart.line.uptrend.xyaxis` · Banco de pruebas `testtube.2` · Importar/Exportar `arrow.up.arrow.down.square` · Análisis de grupos `square.grid.3x3` · Reducciones perfectas `checkmark.circle` · Dependencia lineal `function` |

Logo de «Acerca de»: `Assets/logo.jpg` actual. Icono de la app: se genera a partir de `Assets/app.ico` en las resoluciones de macOS.

## 2. Ventana, menús, barra y navegación

### 2.1 Ventana (DP-05)

Escena `Window` única «Free1X2»; tamaño inicial 1020×760 y mínimo 900×600 (10:60); cerrar la ventana termina la app
(`applicationShouldTerminateAfterLastWindowClosed = true`), como hoy (`Salir` sin confirmación, 10:105). El contenido es una
`NavigationStack(path:)` sobre `[Destino]` (ESPEC_11 §8.2): cada pantalla es una **página** que se apila, como en el `Frame` actual;
«Atrás» = `Esc` y `⌘[` (10:62). Las páginas que hoy devuelven datos a la anterior (diálogos 30, 34-37, importador/exportador de CP…) se
apilan igual y devuelven su valor al cerrar (plantilla T7). No hay barra lateral (no existe hoy).

### 2.2 Barra de menús (traducción de los 7 menús, 10 §1.3)

| Menú macOS | Ítems (orden de hoy; atajos en §5) |
|------------|------------------------------------|
| **Free1X2** (menú de la app) | Acerca de Free1X2 · Créditos… · — · Ajustes… `⌘,` (= «Configuración…», abre la página de configuración) · Configurar análisis… · — · Servicios · Ocultar/Mostrar (sistema) · — · Salir `⌘Q` |
| **Archivo** | Nueva combinación `⌘N` · Abrir combinación… `⌘O` · Guardar combinación `⌘S` · Guardar combinación como… `⇧⌘S` · — · Abrir equipos… · Guardar equipos… · — · Boleto / combinación (Inicio) · Gestión de equipos… · Importar / exportar columnas… · Obtener boletos online… · — · Borrar temporales… · Borrar informes… |
| **Edición** | menú estándar del sistema (cortar, copiar, pegar en campos de texto) |
| **Ver** | Inicio · Ver boletos… · Gráfico de columnas… · Estadísticas… · Configuración… · — · Barras de herramientas ▸ (Filtros, Free1X2, Operaciones, Utilidades, Combinación, Archivo) · Listado de condiciones · — · Tema ▸ (Claro, Oscuro, Sistema) · Idioma ▸ (Español, English) |
| **Combinación** | los 13 ítems de hoy (Calcular… `F5`, Calcular varias…, Ver boletos…, Ver boletos en editor de texto…, Imprimir boletos…, Reducir…, Escrutinios…, Escrutar combinaciones…, Analizar combinación…, Gráfico de columnas…, Probabilidades…, Estadísticas…, Añadir Pleno al 15…) |
| **Filtros** | Combinar filtros… · Diferencias entre filtros… · Filtro Coincidencias… · Filtro Aidomnou… · Filtro Pim… |
| **Operaciones** | Álgebra de columnas… · Transposición… · Multiplicador… · Fraccionador… · Rotación de signos… |
| **Utilidades** | los 15 ítems de hoy con sus separadores (10:133) |
| **Ventana** | estándar del sistema |
| **Ayuda** | Recursos de ayuda `⌘?` (también `F1`): abre la página de ayuda actual (DT-19) |

**DT-51:** Nueva/Abrir/Guardar/Guardar como, Abrir/Guardar equipos, Obtener boletos online y Borrar temporales/informes, que hoy solo
están en la barra y en atajos (10:140-141), se añaden al menú **Archivo**: en macOS los atajos `⌘N/⌘O/⌘S` viven en el menú, y el manual
de usuario ya los sitúa ahí (`docs/MANUAL_USUARIO.md:125-135`). No se añade ninguna funcionalidad.

### 2.3 Barra de herramientas (DP-11, DP-12)

- Una barra con los **6 grupos de hoy** en su orden (10 §1.4) y separadores entre grupos; 55 botones con los iconos de §1.4.
- Visibilidad por grupo desde **Ver ▸ Barras de herramientas**, sembrada y guardada en `parametros.free1x2` como hoy; el grupo
  Filtros siempre visible al arrancar (10:164-168). No se usa la personalización del sistema (guardaría en otro sitio).
- Lo que no cabe va al menú de desbordamiento `»` de macOS (sustituye al salto de fila de `ToolbarWrapPanel`).
- «Escrutar combinaciones» deshabilitado y «Análisis gráfico» ausente, como hoy (10:146-147).

### 2.4 Navegación

| Hoy | Mac |
|-----|-----|
| `Navegar(page)` no apila duplicados; `NavegarA` siempre apila (10:61) | `Router.navegar` / `Router.navegarSiempre` |
| `NavegarConAccion(AccionInicio)` (10:65) | `Comando` despachado sobre la `Sesion`; si la página activa no es Inicio, se navega a Inicio y se ejecuta |
| Cambio de idioma recrea la página (pierde lo editado, 10:66) | el idioma se aplica con `.environment(\.locale, …)` **sin** recrear la página (DT-52: la recarga destructiva es una limitación técnica de WinUI, no un comportamiento) |
| Pantallas de condición se abren desde la rejilla de Inicio con el grupo en edición (10:301-306) | `Destino.condicion(tipo, grupo:)` |

## 3. Plantillas de pantalla y mockups

Diez plantillas cubren las 109 superficies (§6). Cada una es una vista genérica de SwiftUI con huecos; el contenido concreto viene de la
sección de su pantalla en 08/09/10. Los mockups son esquemas (no a escala).

### T0 — Inicio (`MainPage`)

```
┌ Free1X2 ─────────────────────────────────────────────────────────────────────────────────────┐
│ [Free: ⏻ ⚙ ≡ ? i] │ [Archivo: …10] │ [Operaciones: …5] │ [Combinación: …13] │ [Filtros: …5] │ » │
├──────────────────────────────────────────────┬───────────────────────────────────────────────┤
│ ┌ Boleto Base ─────── Fijos 0 - Dobles 0 - Triples 14 [↺] ┐ │ ┌ Combinación ───────────────────────────┐ │
│ │ 01  [Local        ▾] [Visitante    ▾]  (1)(X)(2) │ │ [Nueva] [Abrir…] [Guardar…]  jornada38.comb  │ │
│ │ 02  [Local        ▾] [Visitante    ▾]  (1)(X)(2) │ │ └────────────────────────────────────────┘ │
│ │ …                                                │ │ ┌ Filtro de columnas ────────────────────┐ │
│ │ 14  [? ]            [? ]               (1)(X)(2) │ │ [Seleccionar…] [Filtro…]          ● rojo │ │
│ └──────────────────────────────────────────────────┘ │ └────────────────────────────────────────┘ │
│                                                │ Grupo  [⏮][◀]  0 (boleto base)  [▶][⏭] [Eliminar] │
│                                                │ ┌ Condiciones del grupo ─────────────────┐ │
│                                                │ │ ● Variantes X y 2    ● Pesos numéricos  │ │
│                                                │ │ ● Signos seguidos    ● Valoración       │ │
│                                                │ │ ● Dibujos            ● Columnas prob.   │ │
│                                                │ │ ● Interrupciones     ● Distancias       │ │
│                                                │ │ ● Grupos equipos     ● Contactos        │ │
│                                                │ │ ● Formatos           ● Formatos 123     │ │
│                                                │ │ ● Simetrías          ● Diferencias      │ │
│                                                │ └────────────────────────────────────────┘ │
│                                                │ ┌ Condiciones relacionadas ──────────────┐ │
│                                                │ │ ● If-Then  ● Control de grupos  ● Tolerancias │
│                                                │ └────────────────────────────────────────┘ │
│                                                │ Filtro parcial del grupo [Seleccionar…] ● │
│                                                │                       [Reducir] [Calcular] │
└──────────────────────────────────────────────┴───────────────────────────────────────────────┘
```
Cada `●` es un `BotonCondicion` con semáforo (verde = activa, rojo = con datos e inactiva, neutro = vacía; 10:304-306).

### T1 — Condición (17 pantallas; patrón PC, 10 §1.10)

```
‹ Atrás    Variantes X y 2 · Grupo 0                                           ● activa
┌ Valores ─────────────────────────────────────────────────────────────────────────────┐
│ Variantes  [0,1,2A          ]   Equis  [                ]   Doses  [                ] │
│ (texto de ayuda de la pantalla actual, literal)                                       │
│ [contenido específico: rejilla de Dibujos, 10 parejas de Contactos, relaciones…]      │
└───────────────────────────────────────────────────────────────────────────────────────┘
[Aceptar]  [Estadísticas]      [Guardar…] [Abrir…] [Copiar] [Pegar]      [Borrar]  [Cancelar]
```
Botones y efectos idénticos a hoy (Aceptar vuelve; Copiar/Pegar usan `Temp/tmp.<ext>`). Las variantes con navegación interna (CP, grupos
de equipos, formatos, diferencias, If-Then, control de grupos, tolerancias) añaden su barra de navegación bajo el título.

### T2 — Herramienta de ficheros (28 pantallas)

```
‹ Atrás    Fraccionador
┌ Entrada ─────────────────────────────────────────────────────────────┐
│ Fichero de columnas [/Users/…/Columnas/jornada38.txt     ] [Seleccionar…] │
└──────────────────────────────────────────────────────────────────────┘
┌ Parámetros ──────────────────────────────────────────────────────────┐
│ Modo ( ) por cuotas  (•) por tramos     Partes [ 4 ]                  │
└──────────────────────────────────────────────────────────────────────┘
┌ Salida ──────────────────────────────────────────────────────────────┐
│ Carpeta [/Users/…/Columnas                                ] [Seleccionar…] │
└──────────────────────────────────────────────────────────────────────┘
[Calcular]  [Cancelar]        ▓▓▓▓▓▓▓▓▓░░░░░ 62 %   Procesadas = 2 965 000
Estado: «Se han grabado 4 ficheros» (literal de hoy)
```
«Cancelar» solo aparece y actúa donde hoy existe y funciona (DP-10).

### T3 — Cálculo y análisis con resultados (21 pantallas)

```
‹ Atrás    Calcular columnas
┌ Modo ───────────────────────────┐  ┌ Estadísticas ─────────────────────────────┐
│ (•) Contar  ( ) Contar y analizar │  │ Col. iniciales  = 4 782 969              │
│ ( ) Grabar en [fichero…]          │  │ Procesadas      = 1 203 004              │
└───────────────────────────────────┘  │ Admitidas       = 18 211                 │
[Calcular]  [Cancelar]                 │ Porcentaje      = 25                     │
▓▓▓▓▓░░░░░░░░░░░░ 25 %                 │ Tiempo          = 00:00:03.1234567       │
                                       │ Importe         = 9 105,50 €             │
                                       └──────────────────────────────────────────┘
┌ Resultado (tabla, si la pantalla la tiene) ─────────────────────────────────────┐
│ Columna          │ Aciertos │ …                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```
Los textos de contadores conservan el formato de hoy (`" = {n}"`, `TimeSpan`, cultura; T14). Se refrescan con el progreso limitado de
ESPEC_11 §6.2 en lugar del temporizador de 3 s (10:808): mismos campos y valores finales.

### T4 — Visor de resultados (15 pantallas)

```
‹ Atrás    Columnas premiadas                              [Exportar todas…] [Exportar selección…]
┌──────────┬─────────┬──────────┬───────────┐
│ Boleto   │ Columna │ Premio   │ Fichero   │   ← Table (selección múltiple donde hoy la hay)
├──────────┼─────────┼──────────┼───────────┤
│ 1        │ 3       │ 8        │ …         │
└──────────┴─────────┴──────────┴───────────┘
```
Variantes: árbol (análisis de fallos, listado de condiciones), matriz de boletos 8×16 (`BoletoMatrizView`, 10:280), matrices de
estadística (`MatrizAnalisisView`).

### T5 — Lista o catálogo (4 pantallas)

```
‹ Atrás    Gestión de equipos         Categoría [Primera ▾]        [Importar online…]
┌ Equipos ──────────────────┐  [Añadir…] [Mover ▸] [Eliminar]
│ Athletic Club              │
│ Atlético de Madrid         │
│ …                          │  [Guardar]  [Cancelar]
└────────────────────────────┘
```

### T6 — Formulario de configuración (5 pantallas)

```
‹ Atrás    Configuración
Boleto        Nº de partidos [14]   Separador JB [5] [9] [12] …
Puntuación CP Fijos [6]  Dobles [3]  Triples [1]
L.A.E.        Precio [0,50]  % al 14 [12]  Recaudación [14 000 000]  Moneda [€]
Inicio        [ ] Actualizar al inicio   [x] Confirmar al salir
[Guardar]  [Cancelar]        «Algunos cambios pueden requerir reiniciar» (literal, DP-04)
```

### T7 — Página-diálogo con resultado (13 pantallas)

```
‹ Atrás    Copiar datos
Desde [ 1 ]   Hasta [ 7 ]
[Aceptar]  [Cancelar]          → al aceptar, la página devuelve (Desde, Hasta) a la que la abrió
```

### T8 — Gráfico (2 pantallas)

```
‹ Atrás    Gráfico de columnas                              [Exportar imagen…]
┌──────────────────────────────────────────────────────────────┐
│  y ▲            ╱╲        (Swift Charts: LineMark por curva;   │
│    │   ╱╲      ╱  ╲        PointMark si ≤ 64 puntos; escala     │
│    │  ╱  ╲____╱    ╲       común min/max, 10:283)               │
│    └──────────────────────────────────────────▶ x             │
└──────────────────────────────────────────────────────────────┘
Leyenda: ■ Curva 1  ■ Curva 2          Rejilla [50]
```

### T9 — Informativa (Acerca de, Ayuda, Créditos)

```
‹ Atrás    Acerca de
[logo]  Free1X2 · Versión 0.83.0 Rarotonga · www.clubprogol.com
        «El primer programa de quinielas libre y gratuito»
Enlaces: Descarga · Foro · www.clubprogol.com · Manual · Información de licencia · Sobre el software libre · Créditos
```
Textos, enlaces y avisos (incluidos los avisos en inglés de Ayuda) exactamente como hoy (10 §1.11).

## 4. Estados

| Estado | Presentación | Regla |
|--------|--------------|-------|
| Vacío (sin fichero, sin datos) | campos vacíos y `-` en contadores, como hoy | sin textos nuevos |
| Ocupado | `ProgressView` + contadores; botón de acción deshabilitado mientras corre | el resto de la ventana sigue respondiendo (el cálculo no bloquea la UI) |
| Error | `.alert` con título «Error» y el literal de hoy; errores inesperados: «Se produjo un error inesperado:\n\n{mensaje}» | ESPEC_11 §5 |
| Información | `.alert` con título «Free1X2» | cola de `Alertas` (10:229-231) |
| Confirmación | `.confirmationDialog` Sí/No, botón por defecto «No» | solo donde hoy se confirma (Borrar temporales/informes…) |
| Éxito | texto de estado de la pantalla (p. ej. «Se han grabado n columnas»), con sus peculiaridades (10 A-13) | |

## 5. Atajos de teclado

| Hoy | Mac | Acción |
|-----|-----|--------|
| `Ctrl+N` · `Ctrl+O` · `Ctrl+S` | `⌘N` · `⌘O` · `⌘S` | Nueva · Abrir · Guardar combinación |
| — | `⇧⌘S` | Guardar combinación como (convención del menú Archivo; DT-51) |
| `F5` | `F5` | Calcular (navega sin volcar el boleto, como hoy, 10:176) |
| `F1` | `F1` y `⌘?` | Ayuda |
| `Esc` | `Esc` y `⌘[` | Atrás |
| Salir (sin atajo) | `⌘Q` | Salir sin confirmación |
| Configuración (sin atajo) | `⌘,` | Página de configuración |
| — | `⌘H`, `⌘M`, `⌘W` (cierra = sale) | del sistema |

`Return` activa el botón principal de cada página; `Esc` en una página-diálogo (T7) equivale a Cancelar.

## 6. Accesibilidad

- Todo botón solo-icono lleva `accessibilityLabel` = su tooltip actual; los semáforos, «Filtro vacío/activo/con error» (10:282).
- Boleto: cada fila es un elemento con etiqueta «Partido 03: Real Madrid - Sevilla; signos marcados: 1 y X».
- Navegación completa por teclado (`Tab`, flechas en rejillas y tablas, `Espacio` para alternar signos).
- Contraste: la paleta de §1.1 se comprueba (WCAG AA para texto) al implementar M5; donde no llegue, se ajusta el tono **solo** en el modo afectado.
- VoiceOver lee las tablas con cabeceras de columna; las matrices de análisis exponen fila/columna.

## 7. Mapa de las 109 superficies

`Destino` = caso de navegación (ESPEC_11 §8.2). Plantillas de §3. Cómo se abre cada pantalla: su sección en 08/09/10 y el árbol de menús de §2.2.

| Nº | Pantalla (ViewModel) | Plantilla | `Destino` |
|---:|---|---|---|
| — | `MainPage` (Inicio) | T0 | `.inicio` |
| 1 | `AgregaP15Frm` | T2 | `.agregaP15` |
| 2 | `AgregarEquipoFrm` | T7 | `.agregarEquipo` |
| 3 | `AlgebraColumnasFrm` | T2 | `.algebraColumnas` |
| 4 | `AnaCombi` | T3 | `.anaCombi` |
| 5 | `AnalisisFormatos123Frm` | T3 | `.analisisFormatos123` |
| 6 | `AnalizadorJPM` | T3 | `.analizadorJPM` |
| 7 | `AnalizarCombinacionFrm` | T4 | `.analizarCombinacion` |
| 8 | `AnalizarFicheroFrm` | T3 | `.analizarFichero` |
| 9 | `Anastatics` | T3 | `.anastatics` |
| 10 | `BancoPruebasFrm` | T3 | `.bancoPruebas` |
| 11 | `BoletoFrm` | T4 | `.boleto` |
| 12 | `BuscaLimsFrm` | T3 | `.buscaLims` |
| 13 | `CalculaColumnasFrm` | T3 | `.calculaColumnas` |
| 14 | `CalculaColumnasMultipleFrm` | T3 | `.calculaColumnasMultiple` |
| 15 | `CalculoFormatosFrm` | T3 | `.calculoFormatos` |
| 16 | `CambioPuntosFrm` | T6 | `.cambioPuntos` |
| 17 | `Coincidencias` | T2 | `.coincidencias` |
| 18 | `ColGanadoraFrm` | T7 | `.colGanadora` |
| 19 | `ColProbablesFrm` | T1 | `.colProbables` |
| 20 | `ColumnasPremiadasFrm` | T4 | `.columnasPremiadas` |
| 21 | `CombinarFiltros` | T2 | `.combinarFiltros` |
| 22 | `Compresor` | T2 | `.compresor` |
| 23 | `ConfigCPsFrm` | T6 | `.configCPs` |
| 24 | `ConfiguracionAnalisisFrm` | T6 | `.configuracionAnalisis` |
| 25 | `ConfiguracionFrm` | T6 | `.configuracion` |
| 26 | `ContactosFrm` | T1 | `.contactos` |
| 27 | `ControlGruposFrm` | T1 | `.controlGrupos` |
| 28 | `ControlTolFrm` | T1 | `.controlTol` |
| 29 | `CopiarCPFrm` | T7 | `.copiarCP` |
| 30 | `CopiarDatosCPFrm` | T7 | `.copiarDatosCP` |
| 31 | `CrearGruposFrm` | T7 | `.crearGrupos` |
| 32 | `DescargaBoletoFrm` | T5 | `.descargaBoleto` |
| 33 | `DiFiltros` | T2 | `.diFiltros` |
| 34 | `DialogoAnalisisMultipleDeTramosFrm` | T5 | `.dialogoAnalisisMultipleDeTramos` |
| 35 | `DialogoFiltrarPorLimitesFrm` | T7 | `.dialogoFiltrarPorLimites` |
| 36 | `DialogoGrabarBancoPruebasFrm` | T7 | `.dialogoGrabarBancoPruebas` |
| 37 | `DialogoGrabarTramosFrm` | T7 | `.dialogoGrabarTramos` |
| 38 | `DibForm` | T4 | `.dib` |
| 39 | `DibRepFrm` | T4 | `.dibRep` |
| 40 | `DibujosFrm` | T1 | `.dibujos` |
| 41 | `DifCols` | T2 | `.difCols` |
| 42 | `DiferenciasFrm` | T1 | `.diferencias` |
| 43 | `DistanciasFrm` | T1 | `.distancias` |
| 44 | `EscrutarCombinacionesFrm` | T3 | `.escrutarCombinaciones` |
| 45 | `EscrutiniosFrm` | T3 | `.escrutinios` |
| 46 | `EstimadorPremiosFrm` | T3 | `.estimadorPremios` |
| 47 | `EstucolFrm` | T2 | `.estucol` |
| 48 | `ExportadorCPsFrm` | T7 | `.exportadorCPs` |
| 49 | `FigurasFiltrosFrm` | T7 | `.figurasFiltros` |
| 50 | `FiltroPorcenJB` | T2 | `.filtroPorcenJB` |
| 51 | `Formatos123Frm` | T1 | `.formatos123` |
| 52 | `FormatosFrm` | T1 | `.formatos` |
| 53 | `FraccionadorFrm` | T2 | `.fraccionador` |
| 54 | `FrmDependenciaLineal` | T2 | `.dependenciaLineal` |
| 55 | `FrmReducidasPerfectas` | T2 | `.reducidasPerfectas` |
| 56 | `GEPTFrm` | T3 | `.gEPT` |
| 57 | `GeneraPim` | T2 | `.generaPim` |
| 58 | `GeneradorCPSDiferencias` | T2 | `.generadorCPSDiferencias` |
| 59 | `GenerarCPs` | T2 | `.generarCPs` |
| 60 | `GestorEquiposFrm` | T5 | `.gestorEquipos` |
| 61 | `GraficoColumnasFrm` | T8 | `.graficoColumnas` |
| 62 | `GruposEquiposFrm` | T1 | `.gruposEquipos` |
| 63 | `GuardarValoracionFrm` | T7 | `.guardarValoracion` |
| 64 | `HistoriaValoracionesFrm` | T7 | `.historiaValoraciones` |
| 65 | `IfThenFrm` | T1 | `.ifThen` |
| 66 | `ImportExportFrm` | T2 | `.importExport` |
| 67 | `ImportadorCPsFrm` | T7 | `.importadorCPs` |
| 68 | `ImprimirBoletoFrm` | T6 | `.imprimirBoleto` |
| 69 | `InterrupcionesFrm` | T1 | `.interrupciones` |
| 70 | `ListaImpresoras` | T5 | `.listaImpresoras` |
| 71 | `ListadoCondicionesFrm` | T4 | `.listadoCondiciones` |
| 72 | `MejoresOpcionesFrm` | T3 | `.mejoresOpciones` |
| 73 | `ModificadorFrm` | T2 | `.modificador` |
| 74 | `MultiplicadorFrm` | T2 | `.multiplicador` |
| 75 | `NoVariantesFrm` | T1 | `.noVariantes` |
| 76 | `OrdenarPorProbabilidadFrm` | T2 | `.ordenarPorProbabilidad` |
| 77 | `ParejasFrm` | T2 | `.parejas` |
| 78 | `PesosNumFrm` | T1 | `.pesosNum` |
| 79 | `PosiblesPremiosFrm` | T3 | `.posiblesPremios` |
| 80 | `PremiadasFrm` | T3 | `.premiadas` |
| 81 | `ProbabilidadPremios` | T3 | `.probabilidadPremios` |
| 82 | `ReductorFrm` | T2 | `.reductor` |
| 83 | `RentabilidadFrm` | T3 | `.rentabilidad` |
| 84 | `ResultadosCalculoMultipleFrm` | T4 | `.resultadosCalculoMultiple` |
| 85 | `RotacionDeSignosFrm` | T2 | `.rotacionDeSignos` |
| 86 | `SelecJM` | T2 | `.selecJM` |
| 87 | `SelectorMS` | T2 | `.selectorMS` |
| 88 | `SignosSeguidosFrm` | T1 | `.signosSeguidos` |
| 89 | `SimetriasFrm` | T1 | `.simetrias` |
| 90 | `StaInterFrm` | T4 | `.staInter` |
| 91 | `StaSSForm` | T4 | `.staSS` |
| 92 | `SubirCategoriaFrm` | T2 | `.subirCategoria` |
| 93 | `TramificarForm` | T3 | `.tramificar` |
| 94 | `TramificarGraficasFrm` | T8 | `.tramificarGraficas` |
| 95 | `TransposicionFrm` | T2 | `.transposicion` |
| 96 | `TriosFrm` | T2 | `.trios` |
| 97 | `VSignosFrm` | T3 | `.vSignos` |
| 98 | `ValoracionFrm` | T1 | `.valoracion` |
| 99 | `VerBoletos` | T4 | `.verBoletos` |
| 100 | `VerBoletosEnEditorFrm` | T4 | `.verBoletosEnEditor` |
| 101 | `VisorAnalisisColumnasAbdonFrm` | T4 | `.visorAnalisisColumnasAbdon` |
| 102 | `VisorAnalisisColumnasFrm` | T4 | `.visorAnalisisColumnas` |
| 103 | `VisorEstadisticas` | T4 | `.visorEstadisticas` |
| 104 | `VisorPosiblesPremios` | T4 | `.visorPosiblesPremios` |
| 105 | `aidomnou` | T2 | `.aidomnou` |
| shell | `AcercaDeFrm` (sin ViewModel) | T9 | `.acercaDe` |
| shell | `AyudaFrm` (sin ViewModel) | T9 | `.ayuda` |
| shell | `CreditosFrm` (sin ViewModel) | T9 | `.creditos` |

Recuento: T1 17 · T2 28 · T3 21 · T4 15 · T5 4 · T6 5 · T7 13 · T8 2 · T9 3 · T0 1 → 109 superficies
