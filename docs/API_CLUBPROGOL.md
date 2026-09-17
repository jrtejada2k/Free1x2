# API clubprogol.com — contrato para Free1X2

Especificación del servicio HTTP que **clubprogol.com** expone para que la aplicación de
escritorio **Free1X2** (WinUI 3) consulte la **jornada de la semana** (España y México) y el
**catálogo de equipos**. Con esto la app puede auto-rellenar el boleto con los **equipos reales**
de la jornada (resuelve que la pantalla *Grupos de Equipos* y el boleto muestren nombres reales).

> **Naturaleza de la integración (decidido con el dueño):** función **online opcional** ("Actualizar
> jornada"), con **fallback total a modo offline/manual** si no hay conexión. La app trata la
> respuesta como **datos** (se validan y parsean a la defensiva); nunca ejecuta nada del cuerpo.

## Principios

- **HTTPS** obligatorio. Método **GET**. Cuerpo **JSON, UTF-8** (acentos correctos: "Atlético", "Peñarol").
- **Solo lectura** y **datos públicos** → **sin autenticación** (sin tokens ni claves).
- **URL versionada**: prefijo `/wp-json/clubprogol/v1/` (backend WordPress REST). Cambios incompatibles → `/wp-json/clubprogol/v2/`.
- Respuestas **pequeñas** (objetivo < ~50 KB). `Content-Type: application/json; charset=utf-8`.
- Recomendado: cabecera `ETag` y/o campo `actualizado` para que la app cachee y no re-descargue.
- No se necesita CORS (cliente de escritorio, no navegador).
- Errores con código HTTP correcto: `404` si no hay jornada publicada, `429` si se supera el límite de
  peticiones, `503` si el origen no está listo. Los errores traen un **cuerpo JSON** (ver más abajo).

`{pais}` es `es` (España) o `mx` (México).

### Límite de peticiones (HTTP 429)

El backend aplica un límite de **60 peticiones por minuto**. Al superarlo responde **`HTTP 429 Too
Many Requests`** con la cabecera **`Retry-After: 60`** (segundos a esperar). La app lo maneja con
gracia: si el `Retry-After` es corto reintenta una vez; si pide esperar ~60 s **no bloquea la UI** y
muestra un mensaje claro (cae a modo manual). No conviene "machacar" el endpoint en bucle.

### Cuerpos de error (JSON)

Todos los errores devuelven el mismo shape, con un `mensaje` legible que la app propaga al usuario:

```json
{ "error": "sin_jornada",   "mensaje": "No hay jornada publicada para ES." }   // 404
{ "error": "rate_limited",  "mensaje": "Demasiadas solicitudes (límite 60/min). Intenta de nuevo en ~60 s." }  // 429
```

| Código | `error` | Significado |
|--------|---------|-------------|
| `404` | `sin_jornada` | No hay jornada publicada para ese país. |
| `429` | `rate_limited` | Se superó el límite de 60 peticiones/min (`Retry-After: 60`). |
| `503` | `no_listo` | El origen no está listo todavía. |

---

## 1) Jornada actual — núcleo

```
GET https://clubprogol.com/wp-json/clubprogol/v1/quiniela/{pais}/actual
```

Devuelve la jornada vigente (los 14 partidos con sus equipos). Es lo único imprescindible para
auto-rellenar el boleto.

**Respuesta (España):**
```json
{
  "pais": "ES",
  "temporada": "2025/26",
  "jornada": 38,
  "fecha": "2026-06-21",
  "actualizado": "2026-06-18T10:30:00Z",
  "partidos": [
    { "n": 1,  "local": "Real Madrid",      "visitante": "FC Barcelona" },
    { "n": 2,  "local": "Atlético de Madrid","visitante": "Athletic Club" },
    { "n": 3,  "local": "Sevilla",          "visitante": "Real Betis" },
    { "n": 4,  "local": "Valencia",         "visitante": "Villarreal" },
    { "n": 5,  "local": "Real Sociedad",    "visitante": "Osasuna" },
    { "n": 6,  "local": "Celta",            "visitante": "Rayo Vallecano" },
    { "n": 7,  "local": "Getafe",           "visitante": "Mallorca" },
    { "n": 8,  "local": "Girona",           "visitante": "Las Palmas" },
    { "n": 9,  "local": "Alavés",           "visitante": "Leganés" },
    { "n": 10, "local": "Espanyol",         "visitante": "Valladolid" },
    { "n": 11, "local": "Levante",          "visitante": "Elche" },
    { "n": 12, "local": "Real Oviedo",      "visitante": "Sporting" },
    { "n": 13, "local": "Racing",           "visitante": "Eibar" },
    { "n": 14, "local": "Almería",          "visitante": "Granada" }
  ],
  "pleno15": { "n": 15, "local": "Cádiz", "visitante": "Tenerife" }
}
```

**Respuesta (México / Progol):** idéntica forma; `pleno15` puede omitirse (no aplica). Opcional:
añadir `revancha` (7 partidos) más adelante si se quiere; el núcleo de 14 es lo que usa la app.
```json
{
  "pais": "MX",
  "temporada": "2026",
  "jornada": 24,
  "fecha": "2026-06-21",
  "actualizado": "2026-06-18T10:30:00Z",
  "partidos": [
    { "n": 1, "local": "América",     "visitante": "Guadalajara" },
    { "n": 2, "local": "Cruz Azul",   "visitante": "Pumas UNAM" }
    /* … 14 en total … */
  ]
}
```

### Campos
| Campo | Tipo | Obligatorio | Notas |
|-------|------|-------------|-------|
| `pais` | string | sí | `"ES"` o `"MX"`. |
| `temporada` | string | sí | Libre, p. ej. `"2025/26"` o `"2026"`. |
| `jornada` | entero | sí | Nº de jornada. La app lo usa para cachear/mostrar. |
| `fecha` | string `YYYY-MM-DD` | recomendado | Fecha de la jornada. |
| `actualizado` | string ISO-8601 UTC | recomendado | Para cache. |
| `partidos` | array | sí | **Exactamente 14** objetos, `n` de 1 a 14, en orden. |
| `partidos[].n` | entero | sí | Posición 1–14. |
| `partidos[].local` | string | sí | Nombre del equipo local, tal cual debe mostrarse. |
| `partidos[].visitante` | string | sí | Nombre del equipo visitante. |
| `pleno15` | objeto | opcional | Solo España; mismo shape con `n:15`. |

---

## 2) Catálogo de equipos — implementado (alimenta *Gestor de Equipos*)

```
GET https://clubprogol.com/wp-json/clubprogol/v1/equipos/{pais}
```

El backend real devuelve **una sola división** con `id` = `"all"` (los equipos que han aparecido en
jornadas recientes), no las divisiones separadas.

**La app SÍ consume este endpoint**, en *Gestión de Equipos → «Importar equipos online»*
(`Free1X2.WinUI/Views/Ported/GestorEquiposFrmViewModel.cs:149-192`):

1. El usuario elige **país** en el selector «País (online)» (España / México → `es` / `mx`).
2. La app hace el `GET` y parsea el catálogo con el parser defensivo `CatalogoEquiposParser`
   (`QuinielaOnlineService.ObtenerEquiposAsync`, `:168-190`).
3. Aplana **todas las divisiones** del JSON (hoy, la única `"all"`) y **fusiona** la lista en la
   **categoría destino** que el usuario tenga elegida en el selector «Categoría» (1ª / 2ª / 2ªB /
   Int). El backend no separa por división, así que es el usuario quien decide dónde caen.
4. La fusión es **no destructiva**: solo añade los que falten (dedup ignorando mayúsculas/minúsculas
   y espacios extremos, `:208-218`); no borra, no reordena y **no escribe en disco**.
5. El cambio queda **solo en memoria**: hay que pulsar **«Guardar archivos»** (`Guardar()`,
   `:314-331`) para persistirlo en los `.dat` de cada categoría.
6. Sin conexión / `429` / JSON inválido → mensaje claro y **no se modifica nada** (`:180-187`).

A diferencia de la jornada, el catálogo **no se cachea** en disco: es una acción explícita y
puntual del usuario (`QuinielaOnlineService.cs:162-164`).

```json
{
  "pais": "ES",
  "actualizado": "2026-06-18T10:30:00Z",
  "divisiones": [
    {
      "id": "all",
      "nombre": "Equipos (jornadas recientes)",
      "equipos": ["Real Madrid", "FC Barcelona", "Atlético de Madrid", "Real Oviedo", "Sporting", "..."]
    }
  ]
}
```
México tiene la misma forma (una división `"all"`). Ejemplos completos en
`docs/ejemplos-api/equipos-es.json` y `docs/ejemplos-api/equipos-mx.json`.

---

## 3) Jornada por número — opcional (histórico)

```
GET https://clubprogol.com/wp-json/clubprogol/v1/quiniela/{pais}/jornada/{n}
```
Misma forma que la jornada actual. Útil para revisar jornadas pasadas. No es necesario para la v1.

---

## Cómo lo consume la app

1. Base URL **configurable** = **raíz del host** (en `parametros.free1x2` / configuración o vía la
   variable de entorno `FREE1X2_API_BASE`), por defecto `https://clubprogol.com`. La app le antepone
   el prefijo `/wp-json/clubprogol/v1/` a cada ruta. Así puedes apuntarla a un stub mientras desarrollas.
2. Acción **"Actualizar jornada (online)"** (reactiva la pantalla *Descarga de boleto* del original):
   GET a `/wp-json/clubprogol/v1/quiniela/{pais}/actual`, valida el JSON, y **rellena los 14 partidos**
   del boleto con `local`/`visitante`. Esos nombres se guardan en el estado compartido → *Grupos de
   Equipos* y demás pantallas muestran **equipos reales**.
3. Si no hay conexión, hay `429`/`404`, o el JSON no valida → **mensaje claro y modo manual**
   (comportamiento offline actual, sin romper nada). En `404`/`429` se muestra el `mensaje` del servidor.
4. **Caché local de la jornada (offline-first).** Tras cada descarga correcta, la app guarda el
   **JSON crudo** —los bytes ya validados— en `%LocalAppData%\Free1X2\jornada-{es,mx}.json`, un
   fichero por país (`Free1X2.WinUI/Services/JornadaCache.cs:52,66-81`;
   `QuinielaOnlineService.cs:152`). Consecuencias:
   - **Al arrancar**, la app siembra la jornada guardada más reciente **sin tocar la red**
     (`App.xaml.cs:45-61`, `JornadaCache.PaisMasReciente()`): boleto y *Grupos de Equipos* ya
     muestran equipos reales offline. El **único** punto de red es el botón «Actualizar jornada».
   - La caché se re-parsea con el **mismo parser defensivo** que la respuesta HTTP: se trata como
     dato no confiable. Una caché corrupta o ilegible se descarta en silencio (= «no hay caché»)
     y la app cae a modo manual (`JornadaCache.cs:89-114`).
   - **Guarda anti-doble-petición:** si el mismo país se pidió hace **< 60 s** y hay caché válida,
     se devuelve la caché **sin salir a la red** (`QuinielaOnlineService.cs:67,116-126`). Evita que
     dobles clics rápidos tropiecen con el límite de 60/min.
   - Si la red falla, la pantalla cae a la jornada guardada e indica la fecha de última
     actualización (`DescargaBoletoFrmViewModel.cs:117-135`).
5. El catálogo `/wp-json/clubprogol/v1/equipos/{pais}` alimenta el *Gestor de Equipos*
   (**implementado**, ver §2): importa la lista de equipos y la fusiona en la categoría elegida.
   Este endpoint **no se cachea**.

> **Revancha (México).** A fecha de 2026-09 el servidor **no** publica el bloque `revancha`, y la
> app tampoco lo consume: el parser lee solo las propiedades que conoce
> (`JornadaQuinielaParser.cs:63,87`), por lo que **tolera campos extra** sin romperse. Cuando el
> backend lo sirva, se extenderá el parser sin cambiar el contrato actual de los 14 partidos.

### Para probar localmente (stub)

La app lee la base URL de la variable de entorno **`FREE1X2_API_BASE`** (si no existe, usa
`https://clubprogol.com`). Para probar sin backend real, sirve los JSON de `docs/ejemplos-api/` con el
stub incluido `scripts/stub-api.ps1`:

```powershell
# Terminal 1 — levanta el stub (sirve los ejemplos en las rutas de la spec)
pwsh ./scripts/stub-api.ps1            # http://localhost:8080  (usa -Port para otro puerto)
```
```powershell
# Terminal 2 — apunta la app al stub y lánzala
$env:FREE1X2_API_BASE = 'http://localhost:8080'
.\Free1X2.WinUI\bin\x64\Debug\net8.0-windows10.0.19041.0\win-x64\Free1X2.WinUI.exe
```

En la app: **Descarga de boleto → elige España/México → Actualizar jornada**. El boleto y la pantalla
*Grupos de Equipos* se rellenan con los equipos reales del JSON. Cuando tu backend esté en producción,
quita la variable (o ponla a `https://clubprogol.com`) y apuntará al servicio real.

---

*Contrato mínimo y estable. Si necesitas ajustar nombres de campos, dilo y la app se adapta — basta
con que el shape sea consistente.*
