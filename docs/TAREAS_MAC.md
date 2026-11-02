# Tareas para la app Mac: arreglos que ya tiene Windows

**Para el agente que trabaja en el Mac.** Las pruebas funcionales de Windows del 1-nov-2026
([`docs/PRUEBAS_WINDOWS.md`](PRUEBAS_WINDOWS.md), resultados en [`docs/pruebas/`](pruebas/)) encontraron cuatro errores. Ya
están corregidos en Windows, y **la app Mac tiene los cuatro**: copió el comportamiento del port de WinUI, que en dos casos
era un error del port y en otros dos un error del original que las decisiones de la Mac mandaban replicar. Este documento
dice qué cambiar, dónde está en el Swift y cómo comprobarlo. El dueño ya decidió corregirlos todos; no se vuelve a preguntar.

Desde Windows **no se ha tocado** `Free1X2.Mac/`: todas las referencias Swift de abajo se leyeron, no se cambiaron.

## 0. Antes de empezar

1. **Referencia Windows:** `main` desde `80fd72d`. Commits de cada arreglo: `5bf0d52` (M1), `d411343` (M2) y `80fd72d`
   (M3 y M4). El C# dice **qué** hace; el Swift se escribe al estilo del Swift que lo rodea.
2. **Decisiones que cambian:** en `docs/mac/DECISIONES.md`, **DEC-922** (`:314`, «Filtro Pim = motor aidomnou») y
   **DEC-918** (`:310`, «tramos pierden sobrantes: replicar») quedan sin efecto, y la parte de Pim de **DT-73** (`:97`)
   también. Anótalo allí con una decisión nueva que remita a este documento.
3. **Un commit por tarea** (`fix(mac): M2 …`).

## M1 · «Calcular» desde el menú, el panel lateral o F5 debe usar el boleto de Inicio

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
  string '3-6' was not in a correct format». Hay dos salidas: corregir el manual (y `docs/FAQ.md` y `docs/web/`) o
  aceptar rangos en las dos apps.

## Pendiente de añadir

Las pruebas con datos de los filtros que faltaban están en curso en Windows. Son 20 pantallas del grupo Filtros, más
Columnas probables, Pesos numéricos y Grupos de equipos. Lo que salga y afecte a la Mac se añadirá aquí como M5, M6…

No se aplican a la Mac, porque son cosas de WinUI: el fichero vacío que deja el selector de Guardar en el Fraccionador y
los problemas de pantalla de `docs/pruebas/RESULTADOS_2026-11-01.md`.

## Dar cada tarea por hecha

Hacen falta las tres cosas:
- los tests de `Free1X2Kit` en verde, incluidos los nuevos de M2–M4;
- el CI de `.github/workflows/mac.yml` en verde;
- la prueba de la app indicada en cada tarea (I4, F5, I5, O4), con su resultado anotado.

Al terminar, marca aquí cada tarea con su commit.
