# Preguntas frecuentes (FAQ) — Free1X2

## Sobre el programa

### ¿Qué es Free1X2?
Un programa **libre y gratuito** (licencia GPL v3) para confeccionar la **Quiniela** española (14 partidos + Pleno al 15)
y el **Progol** mexicano. No adivina resultados: genera solo las columnas que cumplen las condiciones que tú defines y
te ayuda a **reducirlas**, **escrutarlas** y **analizarlas**. Deriva del Free1X2 original de Joan Duatis.

### ¿En qué sistemas funciona?
- **Windows 10 (19041 o posterior) y Windows 11**, 64 bits. Es portable: no se instala, se copia la carpeta y se ejecuta.
- **Mac** con macOS 14 o posterior y procesador Apple (M1 o posterior).

Las dos versiones usan el mismo motor de cálculo y dan exactamente los mismos resultados.

### ¿Cuesta algo? ¿Lleva publicidad?
No. Es software libre: puedes usarlo, copiarlo y estudiar su código.

### ¿Necesito Internet?
No. Todo el cálculo se hace en tu ordenador. Internet solo se usa, si quieres, para descargar los **equipos de la
jornada**, el **catálogo de equipos** y las **ganadoras históricas**. Sin conexión el programa funciona igual.

### ¿Dónde guarda mis archivos?
En Windows, en la carpeta del programa. En Mac, en `~/Library/Application Support/Free1X2/` (en el Finder: menú
Ir → Ir a la carpeta…, y pega esa ruta).

## La jornada y los equipos

### ¿Tengo que escribir los equipos cada semana?
No. En la versión Mac, al abrir el programa se descarga sola la jornada actual (como mucho espera 3 segundos) y los 14
partidos se rellenan con los equipos local y visitante. También puedes cargarla cuando quieras con **Jornada ▾ → España**
o **México** en la barra de Inicio. En Windows, la jornada se descarga en `Archivo → Obtener Boletos Online`.

### Abrí el programa sin Internet y el boleto está en blanco, ¿es un error?
No. Si no hay conexión, el programa no muestra ningún aviso y deja el boleto en blanco para que escribas los equipos o
pulses **Jornada ▾** más tarde. Si alguna vez descargaste esa jornada, **Jornada ▾** usa la copia guardada y te dice de
qué fecha es.

### ¿Puedo traer de Internet la lista de equipos?
Sí: en **Gestión de Equipos**, elige el país y la categoría donde quieres que caigan y pulsa **«Importar equipos
online»**. Solo añade equipos nuevos; recuerda pulsar **«Guardar archivos»** para conservarlos.

## Combinaciones y condiciones

### ¿Cuántas columnas genera mi pronóstico?
3 elevado al número de triples por 2 elevado al número de dobles. Por ejemplo, 6 triples + 6 dobles + 2 fijos =
3⁶ × 2⁶ = **46.656 columnas**. Las condiciones y las reducciones bajan ese número.

### Al poner varios valores en una condición me dice «La cadena de entrada no tiene el formato correcto».
Separa los valores con **comas** (`0,1,2`) o usa rangos (`2-6`). Con espacios (`0 1 2`) no los entiende.

### «Calcular» me da 0 columnas, ¿qué pasa?
Tus condiciones son incompatibles entre sí o con el pronóstico (por ejemplo, pedir 10 variantes con 8 partidos fijos al
`1`). Abre **Listado de Condiciones** para revisarlas todas juntas y afloja la más estricta.

### ¿Qué es una columna probable (CP)?
Un segundo pronóstico sobre algunos partidos en el que pides un **rango de aciertos** en vez de acertarlo todo. Por
ejemplo, «los 4 favoritos ganan, con 3 ó 4 aciertos» deja 4 triples (81 apuestas) en 9. Es la condición más potente del
programa.

### ¿Qué es una reducción y qué nivel elijo?
Una reducción juega muchas menos columnas a cambio de **garantizar un premio menor** si el 14 estaba en tu combinación:
reducir al 13 garantiza al menos un 13; al 12, al menos un 12. Cuanto más bajo el nivel, menos columnas y menos garantía.

### ¿Tengo que guardar un archivo para poder reducir o filtrar mi boleto?
En la versión Mac, no: **Reducir** en Inicio abre el reductor con las columnas del boleto ya cargadas, y los filtros
tienen un botón **«Columnas de Inicio»**. Después guardas el resultado solo si te convence, con **«Guardar
resultado…»**. En Windows, primero se calcula y se graba el archivo de columnas.

## Máscaras e historial (versión Mac)

### ¿Qué es una máscara?
Los **signos de cada partido** de tu boleto (por ejemplo `1X` en el 1, `1` en el 2, `1X2` en el 3…) guardados con un
nombre y sin los equipos. Cada jornada eliges **Máscaras ▾** y aplicas la que quieras: los signos pasan al boleto y los
equipos de la jornada se quedan.

### ¿Qué máscara ha dado más premios?
En la pantalla **Máscaras**, pulsa **«Escrutar las máscaras»**: se comparan todas tus máscaras con las ganadoras de las
jornadas pasadas y verás, por máscara, cuántas columnas habrían tenido 14, 13, 12, 11 y 10 aciertos y en cuántas
jornadas hubo premio. Pulsa una cabecera para ordenar.

### ¿Puedo usar mi propio histórico de resultados?
Sí. En la pantalla **Máscaras**, pulsa **«Escrutar con un archivo de ganadoras…»** y elige un archivo de texto con **una
columna ganadora de 14 signos por línea** (`1`, `X` o `2`), por ejemplo:

```
1X21X1121X1X21
112X1211X1X221
```

Puede tener las jornadas que quieras (1.200, por ejemplo). Las líneas en blanco se ignoran y lo que vaya detrás de los 14
signos (como el pleno al 15) también. Si una línea está mal, el programa te dice cuál es.

### ¿Puedo escrutar solo una liga?
Puedes elegir el **juego**: Quiniela de **España** o Progol de **México**. Una liga de fútbol concreta no: cada columna
ganadora es la de una jornada completa de 14 partidos, que mezcla partidos de varias ligas.

### Si una máscara ha dado muchos premios, ¿la próxima jornada también?
No necesariamente. Cada jornada tiene partidos distintos, así que el historial de una máscara mide sobre todo cómo se
reparten los signos en general (el `1` sale en torno a la mitad de las veces). Sirve para comparar planteamientos, no
como garantía.

## Escrutinio

### ¿Cómo sé cuánto he acertado?
Abre **Escrutinio**, elige tus archivos de columnas y escribe la columna ganadora: verás cuántas columnas tienen 14, 13,
12, 11 y 10 aciertos.

### ¿Qué formato tiene un archivo de columnas?
Texto plano con **una columna de 14 signos por línea** (`1`, `X` o `2`), por ejemplo `11X211X1X12111`. Es el formato que
usan los filtros, el escrutinio y las operaciones, y el que puedes intercambiar con otros programas.

## Ayuda

### ¿Dónde pregunto o aviso de un error?
En el **foro de Free1X2**: [clubprogol.com/foro](https://clubprogol.com/foro/). Si es un error, cuenta qué hiciste, qué
esperabas y qué pasó, y si puedes adjunta la combinación (`.comb`).
