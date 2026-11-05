# Tareas para la app Mac: arreglos que ya tiene Windows

**Para el agente que trabaja en el Mac.** Las pruebas funcionales de Windows del 1-nov-2026
([`docs/PRUEBAS_WINDOWS.md`](PRUEBAS_WINDOWS.md), resultados en [`docs/pruebas/`](pruebas/)) encontraron cuatro errores. Ya
están corregidos en Windows, y **la app Mac tiene los cuatro**: copió el comportamiento del port de WinUI, que en dos casos
era un error del port y en otros dos un error del original que las decisiones de la Mac mandaban replicar. Este documento
dice qué cambiar, dónde está en el Swift y cómo comprobarlo. M5–M8 (2-nov-2026) son las diferencias de las pantallas
de filtros que se encontraron después. El dueño ya decidió corregirlos todos; no se vuelve a preguntar.

Desde Windows **no se ha tocado** `Free1X2.Mac/`: todas las referencias Swift de abajo se leyeron, no se cambiaron.

## 0. Antes de empezar

1. **Referencia Windows:** `main` desde `80fd72d`. Commits de cada arreglo: `5bf0d52` (M1), `d411343` (M2) y `80fd72d`
   (M3 y M4). El C# dice **qué** hace; el Swift se escribe al estilo del Swift que lo rodea.
2. **Decisiones que cambian:** en `docs/mac/DECISIONES.md`, **DEC-922** (`:314`, «Filtro Pim = motor aidomnou») y
   **DEC-918** (`:310`, «tramos pierden sobrantes: replicar») quedan sin efecto, y la parte de Pim de **DT-73** (`:97`)
   también. Anótalo allí con una decisión nueva que remita a este documento.
3. **Un commit por tarea** (`fix(mac): M2 …`).

## M1 · «Calcular» desde el menú, el panel lateral o F5 debe usar el boleto de Inicio

Issue [#17](https://github.com/jrtejada2k/Free1x2/issues/17).

**Qué pasa.** Solo el botón «Calcular» de Inicio pone `sesion.usarCombinacionCompartida = true`
(`App/Pantallas/Inicio/VistaInicio.swift:262`). El menú «Calcular…» con F5 (`App/Navegacion/Menus.swift:71`) y el panel
lateral (`App/Navegacion/Lateral.swift:24`) abren la pantalla sin esa marca. Entonces `cargar()` crea una combinación
nueva de 14 triples y no usa el boleto, las condiciones ni el filtro (`App/Pantallas/Combinacion/CalculaColumnas.swift:31-41`).
En Windows pasaba lo mismo: calculaba los **4.782.969** de 14 triples.

**Qué hacer.** La pantalla usa siempre `sesion.combinacion`, con sus condiciones y su filtro de columnas, se abra desde
donde se abra. Referencia: `Free1X2.WinUI/Views/Ported/CalculaColumnasFrmViewModel.cs` (constructor), commit `5bf0d52`.

**Cómo comprobarlo.** Es la prueba **I4** de `docs/PRUEBAS_WINDOWS.md`: el boleto y las condiciones de I3 más el filtro
`filtro_medio.txt`, abriendo «Calcular» desde el panel y con F5. Debe dar **500 procesadas / 136 aceptadas**, lo mismo que
desde el botón de Inicio.

## M2 · Filtro Pim: el algoritmo del original, no el de Aidomnou

Issue [#18](https://github.com/jrtejada2k/Free1x2/issues/18).

**Qué pasa.** Pim usa el motor de Aidomnou (`columnasAidomnou` / `filtrarAidomnou`) por DEC-922
(`Packages/Free1X2Kit/Sources/Free1X2CasosDeUso/Utilidades/Aidomnou.swift:5-6,123-127`;
`App/Pantallas/Utilidades/Aidomnou.swift:9`). El original es otro algoritmo (`Free1X2/UI/FiltroPim.cs:221-248,448-466`):
7 rankings de 6 casillas, **sin suma**, y el recorrido es el máximo menos el mínimo de los 7. Con el caso de la prueba F5,
Windows antes daba **Admitidas 0** y «Analizar» R1–R7 = 14, 8, 13, 10, 7, 7, 59.

**Qué hacer.** Portar `Free1X2.Domain/Analisis/FiltroPim.cs`, que es corto y libre de UI:
- `CalculaRankings`: en cada ranking se eligen las 6 casillas de más valor que quedan, con «mayor estricto». Los empates
  van al primero en orden P1..P14 y, dentro del partido, 1, X, 2.
- `Aciertos`: la «x» minúscula cuenta como X, y una columna de menos de 14 signos da error.
- `Recorrido` y `Admite` (8 rangos: R1–R7 y el recorrido).

Después, ajustar la pantalla como en `Free1X2.WinUI/Views/Ported/GeneraPimViewModel.cs` (commit `d411343`):
- Pasar la entrada a mayúsculas.
- Una línea de menos de 14 caracteres da error de longitud y para la lectura.
- Quitar las repetidas antes de validar.
- Las admitidas se graban en el orden de entrada.
- «Exportar» graba los 7 rankings.
- «Analizar» da 7 valores; R7 ya no es una suma.

**Cómo comprobarlo.**
- **Tests** (los de `Free1X2.Domain.Tests/FiltroPimTests.cs`), con valores 60/25/15 en los 14 partidos:
  - R1 = `1` en P1–P6, R2 = `1` en P7–P12, R3 = `X` en P1–P4 y `1` en P13–P14, R4 = `X` en P5–P10, R5 = `2` en P1–P2 y
    `X` en P11–P14, R6 = `2` en P3–P8, R7 = `2` en P9–P14.
  - `111XXX11X1XX12` da aciertos **3, 3, 2, 3, 2, 0, 1** y recorrido 3. Con los rangos por defecto (R1 3-6, R2–R5 2-6,
    R6 0-1, R7 0-1, recorrido 0-14) se **admite**. Con el recorrido 4-14, no.
  - `11111111111111` da 6, 6, 2, 0, 0, 0, 0 y **no** se admite.
  - Una columna de 4 signos da error.
- **En la app:** la prueba **F5** debe dar **Admitidas 1** (`111XXX11X1XX12`) y «Analizar» 3, 3, 2, 3, 2, 0, 1.

## M3 · Una línea en blanco del fichero de filtro no es una columna

Issue [#19](https://github.com/jrtejada2k/Free1x2/issues/19).

**Qué pasa.** Con un filtro de columnas, `usaColumnasArchivo` analiza cada línea, también las vacías
(`Packages/Free1X2Kit/Sources/Free1X2Motor/Calculo/Analizador.swift:291-307`). Una línea vacía tiene menos de 14
caracteres y se rellena con `completarCon` (`:301-302`, y `Free1X2Base/Codificaciones/UtilColumnas.swift:38-40`). En
Windows eso la convertía en la columna `11111111111111`, un error que venía del original. En la Mac **no se ha
comprobado qué columna sale**, porque con `completarCon` vacío va por otro camino (`convStrToLong(_, faltan:)`). Pero la
línea no se salta, y el test de abajo lo dirá.

**Qué hacer.** Saltar las líneas vacías o hechas solo de espacios en las tres rutas del filtro (recortar, rellenar y
externa). Referencia: `Free1X2.Domain/MotorCalculo/GeneradorColumnas.cs`, en `UsaColumnasArchivo()`,
`UsaColumnasArchivo(bool)` y `UsaColumnasArchivoExterno` (commit `80fd72d`).

**Cómo comprobarlo.** Es el test `Filtro_LineaEnBlanco_NoEsUnaColumna` de `Free1X2.Domain.Tests/HeredadosArregladosTests.cs`:
14 triples y un filtro con estas líneas:

```
1X21X21X21X21X
(línea vacía)
2222222222222X
(tres espacios)
XXXXXXXXXXXXX1
```

Debe dar **3 analizadas y 3 aceptadas**, y ninguna columna `11111111111111`. En Windows, sin el arreglo, daba 5. En la app
es la prueba **I5**.

## M4 · Fraccionador por tramos: no perder columnas

Issue [#20](https://github.com/jrtejada2k/Free1x2/issues/20).

**Qué pasa.** `fraccionar` solo da el resto al último fichero si es menor que un tramo
(`Packages/Free1X2Kit/Sources/Free1X2CasosDeUso/Utilidades/Utilidades.swift:195-196,216-224`, por DEC-918). Por ejemplo,
10 líneas en 4 tramos se graban como 2+2+2+2 y **se pierden 2**.

**Qué hacer.** El último fichero va siempre hasta el final. El tramo `i` empieza en `i·parte` y termina en `(i+1)·parte`,
salvo el último, que termina en el total. En los casos que el original no perdía, el resultado es el mismo. Referencia:
`Free1X2.Domain/Utils/FraccionTramos.cs` (`Limites`), commit `80fd72d`.

**Cómo comprobarlo.** Son los tests `Tramos_*` de `HeredadosArregladosTests.cs`:
- 10 en 4 ⇒ (0,2) (2,4) (4,6) (6,10);
- 9 en 4 ⇒ … (6,9);
- 7 en 2 ⇒ (0,3) (3,7);
- 0 tramos ⇒ ninguno;
- para todo total de 0 a 60 y de 1 a 20 tramos, los tramos son contiguos y suman el total.

En la app es la prueba **O4**: `O4_diez.txt` en 4 tramos ⇒ los 4 ficheros juntos tienen las **10** líneas.

## Común a las dos apps (decisión del dueño)

- **Rangos en las condiciones.** El manual dice que valen rangos como `2-6` (`docs/MANUAL_USUARIO.md:271,277`), pero ni
  Windows ni la Mac los aceptan: hay que escribir `2,3,4,5,6`. En Windows, además, el error sale en inglés: «The input
  string '3-6' was not in a correct format». Issue [#21](https://github.com/jrtejada2k/Free1x2/issues/21).

  **Por qué no los aceptan.** En el original, Variantes no tenía cajas de texto: era una fila de botones 0–14
  (`Free1X2/UI/Filtros/NoVariantesFrm.cs:95`, control `OptionNumTol0_14`), y el control montaba la lista `0,3,5` para
  el motor (`:255`). El motor solo entiende esa lista: parte por comas y convierte cada trozo con `Convert.ToInt32`
  (`Free1X2.Domain/MotorCalculo/FiltroNoVariantes.cs:231-243`). El port cambió los botones por cajas de texto
  (`NoVariantesFrmPage.xaml:34,49,64`) que pasan lo escrito tal cual, y la Mac lo copió. El manual generalizó a partir
  de las pantallas que sí aceptan rangos porque usan `RangosHelper` (Columnas probables, If-Then, Formatos, Grupos de
  equipos y Escrutinios).

  Hay dos salidas. Una es corregir el manual (y `docs/FAQ.md` y `docs/web/`). La otra es aceptar rangos en las dos apps:
  la pantalla expande `2-6` a `2,3,4,5,6` antes de pasarlo al motor, y así el motor no cambia.

## M5–M8 · Pantallas de filtros: lo que el port hacía distinto del original (2-nov-2026)

Al preparar las pruebas con datos de los filtros que faltaban ([`docs/pruebas/filtros/`](pruebas/filtros/README.md)) se
leyó cada pantalla contra el original WinForms y aparecieron diferencias del port. En Windows están corregidas
(commits `4b882b7`, `f97317a`, `ec6683e`, `a6f50cc`), y la Mac tiene casi todas, porque copió el port. Las rutas Swift
son de una lectura, no se han ejecutado. Arreglar = hacer lo que hacía el original: el C# de cada commit dice qué; los
textos de los avisos, los del original.

Abreviaturas: **Kit** = `Packages/Free1X2Kit/Sources/Free1X2CasosDeUso/`; **App** = `App/Pantallas/`.

### M5 · Grupo A (Distancias, Simetrías, Contactos, Figuras) — commit `4b882b7`

Issue [#22](https://github.com/jrtejada2k/Free1x2/issues/22).

- **Contactos, fila vacía:** «todos» debe ser 0…14 (`numeroPartidos`), no 0…15. Kit `Condiciones/ContactosDibujos.swift:16`.
  Prueba FA3b ⇒ **134**.
- **Contactos, «Borrar»:** debe poner un filtro nuevo (vacío) en el grupo, no solo vaciar la pantalla.
  App `Condiciones/ContactosDibujos.swift:31-34`. En Windows queda **inactivo** (el original lo dejaba activo y entonces
  «Borrar» + «Cancelar» rechazaba todas las columnas).
- **Simetrías, Aceptar con una entrada no válida:** mostrar el aviso del original («Verifique que ha introducido una
  simetria correcta…»). Kit `Condiciones/SignosSimetrias.swift:96` devuelve false en silencio; App `Condiciones/Simetrias.swift:39-41`.
- **Distancias:** el rótulo dice «(0–15)» y el 15 no vale: «(0–14)». App `Condiciones/DistanciasInterrupciones.swift:24`.
- **Editor de figuras:** no cortar a 6 caracteres (`9-5-4-2-2-2-1-1-1` es válida) y la ayuda no debe hablar de comodín.
  App `Condiciones/SignosSeguidos.swift:83,92,96`.

### M6 · Grupo B (Formatos 123, Valoración, Análisis de signos) — commit `f97317a`

Issue [#23](https://github.com/jrtejada2k/Free1x2/issues/23).

- **Formatos 123, fila no válida:** el original **para** en la primera fila no válida y no lee las siguientes; la Mac la
  salta. Kit `Condiciones/Formatos123.swift:24-31` (`compactMap`).
- **Formatos 123 ▸ Analizador:** debe usar los formatos **guardados** del grupo; sin guardar, aviso «Guarde el Filtro
  Primero!!!». App `Condiciones/Formatos.swift:95-99` usa los de pantalla.
- **Valoración ▸ «Calcula»** con una columna de menos de 14 signos: aviso «Introducir columna de 14 signos para el
  calculo.». App `Condiciones/Valoracion.swift:107-120`, Kit `Condiciones/Valoracion.swift:83`.
- **Análisis de signos, al abrir un fichero:** Pleno = Sí solo con 15 o más signos, y aspiración = nº de signos.
  App `Utilidades/Signos.swift:9-10,20-30`.
- **Análisis de signos, «% con decimales»:** el original graba `a<TAB>b<TAB>c` con la cultura del sistema (`83,33`),
  no `a, b, c`. Kit `Utilidades/Signos.swift:72-78`.
- **Análisis de signos, rejilla:** en los modos % muestra %, no recuentos. App `Utilidades/Signos.swift:65`.
- La vuelta atrás que en Windows vaciaba las condiciones (H1) **parece** correcta en la Mac (`@State` en la pila de
  navegación); compruébalo con FB2: Saca formatos ▸ volver ▸ la condición sigue ahí.

### M7 · Grupo C (Columnas probables, Grupos de equipos, If-Then, Tríos, Dependencia lineal) — commit `ec6683e`

Issue [#24](https://github.com/jrtejada2k/Free1x2/issues/24).

- **Columnas probables, pronóstico (el más grave):** una caja de 14 caracteres no puede guardar dobles ni triples, y al
  Aceptar una CP de triples pasa a ser `1X21X21X21X21X`. El original tenía un control 1/X/2 **por partido**.
  App `Condiciones/ColProbables.swift:108-111,119`; Kit `Condiciones/ColumnasProbables.swift:140` (une) y `:152-156` (relee).
  Prueba FC13 (CP de 14 triples) ⇒ **216**.
- **Columnas probables, rangos:** la pista promete `0-14`, pero `10-12` da error. Expandir rangos en la pantalla
  antes del motor. Kit `Condiciones/ColumnasProbables.swift:160-165`; pista App `Condiciones/ColProbables.swift:123-125`.
- **Grupos de equipos, ayuda:** «Marcar ambos equivale al empate» es falso: el motor cuenta los dos equipos.
  App `Condiciones/GruposEquipos.swift:80`.
- **Grupos de equipos, nombres:** del boleto de Inicio (vacíos si no hay), no de la jornada online.
  Kit `Condiciones/GruposEquipos.swift:48-51`.
- **If-Then:** «condiciones que deben cumplirse» mayor que el nº de relaciones: aviso claro antes del motor.
  `Combinacion.swift:140-144` hoy da un error de dominio poco claro.
- **Tríos ▸ Analizar:** falta la casilla «Columna Ganadora». App `Utilidades/Grupos.swift:168-169,244`.
- **Dependencia lineal:** tras grabar debe releer la **entrada**, no la salida. App `Utilidades/Dependencia.swift:69`.
  Prueba FC12 ⇒ **108**.

### M8 · Grupo D (Tramificar, Control de tolerancias, Listado, GEPT, Separador JB) — commit `a6f50cc`

Issue [#25](https://github.com/jrtejada2k/Free1x2/issues/25).

- **Filtrar por límites:** las Difs. por defecto son las del original (0–4, 1–3, 1–2, 1–1, 1–0, 1–0, 1–1, 1–2, 1–3,
  0–4), no las de la matriz; filas 9 y 10 a 1–3 y 0–4. Kit `Utilidades/Tramificar.swift:331-334,344,352-353`.
  (El diálogo en sí ya funciona en la Mac: es una hoja, App `Utilidades/Tramificar.swift:338-343`.)
- **Control de tolerancias:** solo se guardan las filas con «Tolerancia» **y** «Aciertos». Kit `Condiciones/ControlGrupos.swift:181`.
- **Listado:** con un If-Then que existe pero está desactivado, el nodo «IfThen» queda vacío; «IfThen no activado» solo
  si no hay If-Then. Kit `Ver/Listado.swift:65,82-83`.
- **GEPT:** una sola cabecera «Última vez» sobre local/visitante, y cajas de texto con el aviso «Las letras permitidas
  son: G,E,P». App `Utilidades/GEPT.swift:22,36-37`.
- **Separador de porcentajes JB:** falta la rejilla 14×7 del original con los signos por banda y la fila «Nº».
  App `Utilidades/Filtros.swift:98-160`.

## M9 · Grupos de Inicio, Rentabilidad y rótulos «0–15» (3-nov-2026)

Issue [#26](https://github.com/jrtejada2k/Free1x2/issues/26).

Salen de las pruebas en la app del 3-nov ([`pruebas/RESULTADOS_2026-11-03.md`](pruebas/RESULTADOS_2026-11-03.md),
grupo E de [`pruebas/filtros/plan_e.md`](pruebas/filtros/plan_e.md)). Windows: commits `558912c`, `8a2db0a`, `6da50b0`.

- **Grupos creados desde Inicio sin partidos (el más grave).** «Grupo siguiente» crea un grupo vacío y nadie le pone
  partidos, así que cualquier condición en él ve la columna vacía y da 0 columnas (FE3x: 0 en vez de 82).
  Kit `Free1X2Motor/Configuracion/GrupoConfig.swift:60`, `Free1X2CasosDeUso/Inicio.swift:234`; «Grupo siguiente» en
  App `Inicio/VistaInicio.swift:114-117`. **Qué hacer**, como el original (`Free1X2/UI/Controls/Pronosticos.cs:682-711,780-805`,
  `MainForm.cs:563-571,1543-1559`) y Windows `558912c`: en un grupo distinto del base, cada partido se activa o
  desactiva (en Windows, clic en su número; doble clic en el título, todos); las filas fuera del grupo salen atenuadas;
  los partidos se guardan en el grupo; y «Grupo siguiente» desde un grupo sin partidos avisa «No ha especificado ningún
  partido, especifique al menos un partido para seguir» (Atención) y no avanza. La forma de marcarlos en la Mac la
  decide el dueño. **Prueba:** FE3x de `plan_e.md` (con los partidos activados) ⇒ **82**.
- **Rentabilidad arranca con recaudación 8.000.000** fija; debe ser la de Configuración (la de Progol con jornada de
  México), como el original (`RentabilidadFrm.cs:96-101`). App `Utilidades/Premios.swift:176`. **Prueba:** FE1 sin
  tocar la recaudación ⇒ **82** (con 8.000.000 da 60).
- **Rótulos «(0–15)»** en Variantes y Signos Seguidos (además del de Distancias de M5): deben decir «(0–14)», y un 15 al
  Aceptar debe dar un aviso claro. App `Condiciones/NoVariantes.swift:31`, `Condiciones/SignosSeguidos.swift:38`.
- **Manual:** con «Fallos = 1» el control de grupos exige que falle exactamente uno (FE3 = 82, FE3b con `0,1` = 176).

No se aplican a la Mac (comprobado al leer el Swift): «Nueva» repetido al volver a Inicio (`App/Arranque/Sesion.swift:190-201`
ejecuta la acción en el momento) y el primer «Calcula» de Valoración (`Valoracion.swift:87` vuelca al teclear).

## M10 · Signos bloqueados en los grupos y botones «Todos / Ninguno» (4-nov-2026)

Issue [#27](https://github.com/jrtejada2k/Free1x2/issues/27). Decisión del dueño; Windows commit `c46d570`.

Un grupo no tiene signos propios: el motor genera las columnas con los signos del boleto base y cada grupo solo guarda
qué partidos incluye (`Free1X2/UI/MainForm.cs:556-571`, `Grupo.cs:165-175`). El original dejaba cambiar los signos
estando en un grupo, y ese cambio tocaba todo el boleto sin que se notara. Ahora:

- En un grupo distinto del base, los signos 1/X/2 **se ven pero no se pueden pulsar**, y el botón «Reiniciar a 14
  triples» está deshabilitado. En el boleto base, todo igual que antes.
- Encima del boleto, solo en un grupo: el texto «Los signos se cambian en el Boleto Base. Pulse el número para meter o
  sacar el partido del grupo.» y dos botones **«Todos»** y **«Ninguno»** que meten o sacan todos los partidos del grupo
  (el doble clic en el título sigue funcionando).
- Las columnas generadas no cambian: es solo cómo se edita.

Depende de M9 (los partidos de cada grupo). **Prueba:** en un grupo, pulsar una X no cambia «Triples: 14»; «Ninguno»
atenúa las 14 filas y deshabilita las condiciones; clic en el número 3 lo activa; «Todos» activa los 14; de vuelta al
boleto base, los signos se pueden pulsar y la barra no se ve.

## M11 · Fallos heredados del original que el dueño decidió arreglar (4-nov-2026)

Issue [#28](https://github.com/jrtejada2k/Free1x2/issues/28). Windows: commits `479c386` (#16), `da1751d` (#21), `4e237b5` (D-CT2/D-CT3/H11), `5d78f50` (el resto) y
`7ccb723` (tamaños). Hasta ahora la regla era «igual que el original»; estos puntos los cambia el dueño a propósito, así
que en la Mac se quitan las marcas `[PRESERVAR]` que los protegían. Lo esperado de las pruebas está en
[`pruebas/filtros/README.md`](pruebas/filtros/README.md) y los planes.

| Punto | Qué hacer | Mac |
|---|---|---|
| **#16 If-Then** | Al Generar, el If-Then se suma a las condiciones (AND), como al analizar columnas | Kit `Free1X2Motor/Calculo/Analizador.swift:403-404` ⇒ `if !(try ifThen.comprueba(col, grupos)) { valida = false }` solo si `valida` |
| **#21 Rangos** | En Variantes, Signos seguidos, Interrupciones, Distancias, Contactos, Pesos y CP, `2-6` ⇒ `2,3,4,5,6` y `2-4a` ⇒ `2a,3a,4a` antes de partir por comas; un rango al revés vale | Kit `Free1X2Motor/Configuracion/ValoresConTolerancia.swift:29-30` (el `poner(String)` común) y los `set…` de `CondicionColProbables.swift:35-43` |
| **D-CT2** | La letra de tolerancia de una condición (`8a`) se pasa a mayúsculas | ídem, `ValoresConTolerancia.swift:35` |
| **D-CT3 / H11** | Aciertos y Fallos de Control de tolerancias, y Suma aciertos / Columnas de Relaciones I: expandir rangos, ordenar y quitar repetidos antes de dimensionar (`0,2-3` y `3,1` valen) | Kit `Configuracion/Tolerancias.swift:41-50` (`ArrayAciertosTol.parsear`) y `CondicionColProbables.swift:291` |
| **S-2 Simetrías** | Con simetrías y Aciertos vacío, aviso «Indique los aciertos de las simetrías (por ejemplo 0-3).» y no se guarda | App `Pantallas/Condiciones/Simetrias.swift` |
| **H2 Formatos 123** | Con formatos y la lista vacía, aviso «Indique las Líneas Acertadas (por ejemplo 1-3).» (o «Aciertos Globales» con Ignorar repeticiones) | App `Pantallas/Condiciones/Formatos.swift` |
| **F-2 Diferencias** | Un valor no válido avisa «Hay un valor no válido. Escriba los partidos y las cantidades con números, separados por comas o en rangos (1-3).» y no cierra | App `Pantallas/Condiciones/Diferencias.swift` |
| **SS-1 Signos seguidos** | Con solo figuras (los 4 campos vacíos) la condición se activa | App `Pantallas/Condiciones/SignosSeguidos.swift` |
| **SS-2 Figuras de Signos seguidos** | `32211` (sin guiones) y `1-1-2-2-3` (otro orden) se guardan como `3-2-2-1-1`; ayuda «Cada figura son los tramos de signos seguidos, por ejemplo 3-2-2-1-1 o 32211 (se ordenan de mayor a menor).» | Editor de figuras de `SignosSeguidos.swift` |
| **D-L1–L3 Listado** | Añadir por grupo «Tolerancias» (`Tolerancia A: aciertos …` y `Fallos Controles: …`), «Interrupciones de Variantes» y «… Seguidas», «Interrupciones de 2:» con dos puntos, y «Partidos Activos: Todos» en el boleto base | Kit `Free1X2CasosDeUso/Ver/Listado.swift` |
| **D-FL4 Filtrar por límites** | La fila 10 usa sus propios valores (no copia la 9) | Kit `Utilidades/Tramificar.swift:357-360` (`e[9] = e[8]` fuera) |
| **D-FL5** | «Difs. desde» n > 0 elimina desde n diferencias: se recorren las más cercanas sin marcarlas | Kit `Utilidades/Tramificar.swift:208` (`eliminar`) |
| **Tamaños** | Botones 1/X/2 de Columnas probables 44×30 con letra 14; casillas de figuras 96 de ancho, letra 15 | App `Pantallas/Condiciones/ColProbables.swift`, editor de figuras |

**Pruebas:** FC8 ⇒ **95**; FL2 ⇒ **501**, FL4 ⇒ **5001**; Variantes `4-7` igual que `4,5,6,7`; FE4 sin el campo 1 ⇒ **60**
(solo figuras); `IfThenAndTests` y `RangosListaTests` de Windows como tests del Kit.

## Otras notas


No se aplican a la Mac, porque son cosas de WinUI: el fichero vacío que deja el selector de Guardar en el Fraccionador y
los problemas de pantalla de `docs/pruebas/RESULTADOS_2026-11-01.md`.

## Dar cada tarea por hecha

Hacen falta las tres cosas:
- los tests de `Free1X2Kit` en verde, incluidos los nuevos de M2–M4;
- el CI de `.github/workflows/mac.yml` en verde;
- la prueba de la app indicada en cada tarea (I4, F5, I5, O4; para M5–M11, las de `docs/pruebas/filtros/plan_*.md` con
  lo esperado de su `README.md`), con su resultado anotado.

Al terminar, marca aquí cada tarea con su commit.
