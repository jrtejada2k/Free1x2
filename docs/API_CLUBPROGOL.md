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
- **URL versionada**: prefijo `/api/v1/`. Cambios incompatibles → `/api/v2/`.
- Respuestas **pequeñas** (objetivo < ~50 KB). `Content-Type: application/json; charset=utf-8`.
- Recomendado: cabecera `ETag` y/o campo `actualizado` para que la app cachee y no re-descargue.
- No se necesita CORS (cliente de escritorio, no navegador).
- Errores con código HTTP correcto: `404` si no hay jornada publicada, `503` si el origen no está listo.

`{pais}` es `es` (España) o `mx` (México).

---

## 1) Jornada actual — núcleo

```
GET https://clubprogol.com/api/v1/quiniela/{pais}/actual
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

## 2) Catálogo de equipos — opcional (alimenta *Gestor de Equipos*)

```
GET https://clubprogol.com/api/v1/equipos/{pais}
```

```json
{
  "pais": "ES",
  "actualizado": "2026-06-18T10:30:00Z",
  "divisiones": [
    { "id": "1",  "nombre": "Primera", "equipos": ["Real Madrid", "FC Barcelona", "Atlético de Madrid", "..."] },
    { "id": "2",  "nombre": "Segunda", "equipos": ["Real Oviedo", "Sporting", "..."] },
    { "id": "2b", "nombre": "Primera RFEF", "equipos": ["..."] },
    { "id": "int","nombre": "Internacional", "equipos": ["..."] }
  ]
}
```
Para México, `divisiones` puede ser `[{ "id":"1", "nombre":"Liga MX", "equipos":[...] }, ...]`.
La app mapea estos `id` a sus categorías internas (`1`, `2`, `2b`, `Int`).

---

## 3) Jornada por número — opcional (histórico)

```
GET https://clubprogol.com/api/v1/quiniela/{pais}/jornada/{n}
```
Misma forma que la jornada actual. Útil para revisar jornadas pasadas. No es necesario para la v1.

---

## Cómo lo consume la app

1. Base URL **configurable** (en `parametros.free1x2` / configuración), por defecto `https://clubprogol.com`.
   Así puedes apuntarla a un stub mientras desarrollas.
2. Acción **"Actualizar jornada (online)"** (reactiva la pantalla *Descarga de boleto* del original):
   GET a `/api/v1/quiniela/{pais}/actual`, valida el JSON, y **rellena los 14 partidos** del boleto
   con `local`/`visitante`. Esos nombres se guardan en el estado compartido → *Grupos de Equipos* y
   demás pantallas muestran **equipos reales**.
3. Si no hay conexión o el JSON no valida → **mensaje claro y modo manual** (comportamiento offline
   actual, sin romper nada).
4. (Opcional) El catálogo `/equipos/{pais}` rellena las listas del *Gestor de Equipos*.

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
