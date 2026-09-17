// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Free1X2.Online;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Excepción de fallo de la descarga online de la jornada. Envuelve cualquier error de
/// red, timeout, código HTTP de error o JSON inválido en un mensaje en español que el
/// llamador puede mostrar directamente al usuario (modo "online opcional con fallback").
/// </summary>
public sealed class QuinielaOnlineException : Exception
{
    public QuinielaOnlineException(string mensaje, Exception? inner = null)
        : base(mensaje, inner) { }
}

/// <summary>
/// Resultado de una petición de jornada: el dato y su PROCEDENCIA.
/// <paramref name="DesdeCache"/> es true cuando la jornada NO viene de la red sino del fichero
/// local (guarda anti-doble-descarga), y <paramref name="GuardadoUtc"/> es la fecha de esa copia
/// local (null si no se pudo consultar). El ViewModel lo usa para redactar el mensaje correcto
/// ("jornada actualizada" vs. "mostrando la última guardada") sin releer ni re-parsear nada.
/// </summary>
public sealed record ResultadoJornada(JornadaQuiniela Jornada, bool DesdeCache, DateTime? GuardadoUtc);

/// <summary>
/// Cliente HTTP de la integración OPCIONAL con clubprogol.com. Descarga la jornada vigente
/// (<c>GET {base}/wp-json/clubprogol/v1/quiniela/{pais}/actual</c>) y la parsea con el parser
/// PURO de dominio (<see cref="JornadaQuinielaParser"/>).
///
/// Principios (docs/API_CLUBPROGOL.md):
///   · La respuesta de red SIEMPRE se trata como DATOS: se valida/parsea a la defensiva,
///     nunca se ejecuta nada del cuerpo.
///   · Offline-first: cualquier fallo (sin conexión, timeout, HTTP de error, JSON inválido)
///     se traduce en una <see cref="QuinielaOnlineException"/> clara; el llamador cae a modo
///     manual sin romper nada.
///   · Base URL: es la RAÍZ del host (p. ej. <c>https://clubprogol.com</c>). El servicio le
///     añade el prefijo del backend WordPress (<c>/wp-json/clubprogol/v1/...</c>). Se puede
///     sobreescribir con la variable de entorno <c>FREE1X2_API_BASE</c> (p. ej. apuntar a un
///     stub local <c>http://localhost:8080</c>). Sin secretos ni autenticación (datos públicos).
///   · Límite de peticiones: el backend aplica 60 req/min y responde <c>HTTP 429</c> con
///     <c>Retry-After: 60</c>. Si el reintento es largo no se bloquea la UI: se lanza una
///     <see cref="QuinielaOnlineException"/> amigable. Los errores (404, 429, …) traen un
///     cuerpo JSON <c>{ "error": "...", "mensaje": "..." }</c> cuyo <c>mensaje</c> se propaga.
/// </summary>
public sealed class QuinielaOnlineService
{
    /// <summary>
    /// Instancia ÚNICA compartida (C-08). Todo el estado del servicio ya era <c>static</c>
    /// (el <c>HttpClient</c> y la marca anti-doble-descarga), así que crear una instancia por
    /// ViewModel era un singleton disfrazado. El constructor es privado para que no haya dudas.
    /// </summary>
    public static QuinielaOnlineService Instancia { get; } = new();

    private QuinielaOnlineService() { }

    /// <summary>Base URL por defecto del servicio: raíz del host (contrato: docs/API_CLUBPROGOL.md).</summary>
    public const string BaseUrlPorDefecto = "https://clubprogol.com";

    /// <summary>Prefijo del backend (WordPress REST de clubprogol). Se antepone a cada ruta.</summary>
    public const string PrefijoApi = "/wp-json/clubprogol/v1";

    /// <summary>Nombre de la variable de entorno que permite sobreescribir la base URL (raíz del host).</summary>
    public const string EnvBaseUrl = "FREE1X2_API_BASE";

    /// <summary>
    /// Umbral (segundos) de <c>Retry-After</c> por debajo del cual SÍ se hace un único reintento
    /// acotado. Si el servidor pide esperar más, NO se bloquea la UI: se falla con mensaje claro.
    /// </summary>
    private const int RetryAfterMaxSegundos = 3;

    /// <summary>
    /// Ventana anti-doble-descarga (segundos): si se pide la jornada del MISMO país de nuevo dentro
    /// de este intervalo Y ya existe caché, se devuelve la caché SIN tocar la red. Evita que dobles
    /// clics accidentales en "Actualizar jornada" disparen ráfagas que rocen el 429 del backend
    /// (60 req/min). Una acción explícita posterior (>60 s) vuelve a contactar el servicio.
    /// </summary>
    private const int VentanaAntiDobleSegundos = 60;

    /// <summary>
    /// Última hora (UTC) de inicio de una descarga por país, para la guarda anti-doble-descarga.
    /// Concurrente porque podría consultarse desde distintos contextos de sincronización.
    /// </summary>
    private static readonly ConcurrentDictionary<string, DateTime> _ultimaDescargaUtc = new();

    // Un único HttpClient reutilizado (best practice: evita el agotamiento de sockets).
    private static readonly HttpClient _http = CrearHttpClient();

    private static HttpClient CrearHttpClient()
    {
        var http = new HttpClient
        {
            // Timeout razonable: la app no debe quedarse colgada si el origen no responde.
            Timeout = TimeSpan.FromSeconds(10),
        };
        http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        return http;
    }

    /// <summary>
    /// Base URL efectiva: <c>FREE1X2_API_BASE</c> si está definida y no vacía; si no, el
    /// valor por defecto. (Cuando exista una clave de configuración persistente para esto,
    /// solo habría que leerla aquí; el resto del flujo no cambia.)
    /// </summary>
    public static string BaseUrl
    {
        get
        {
            string? env = Environment.GetEnvironmentVariable(EnvBaseUrl);
            return string.IsNullOrWhiteSpace(env) ? BaseUrlPorDefecto : env!.Trim();
        }
    }

    /// <summary>
    /// Descarga y parsea la jornada vigente del país indicado ("es" o "mx").
    /// Lanza <see cref="QuinielaOnlineException"/> con un mensaje claro ante cualquier error
    /// de red/HTTP/parseo (el llamador lo convierte en el mensaje offline amigable).
    ///
    /// Devuelve, además de la jornada, de DÓNDE salió (<see cref="ResultadoJornada.DesdeCache"/>)
    /// y cuándo se guardó, para que el ViewModel pueda distinguir "jornada actualizada" de
    /// "mostrando la última guardada" sin volver a leer ni parsear el fichero de caché.
    /// </summary>
    public async Task<ResultadoJornada> ObtenerJornadaAsync(string pais, CancellationToken ct = default)
    {
        string paisNorm = (pais ?? "").Trim().ToLowerInvariant();
        if (paisNorm != "es" && paisNorm != "mx")
        {
            throw new QuinielaOnlineException("País no soportado: '" + pais + "' (use 'es' o 'mx').");
        }

        // Guarda anti-doble-descarga: si el mismo país se DESCARGÓ CON ÉXITO hace
        // < VentanaAntiDobleSegundos Y hay caché, se devuelve la caché SIN red. Evita que dobles
        // clics rápidos tropiecen con el límite 60/min del backend. (Sin caché => sí se contacta
        // la red, que es el caso de la primera descarga.)
        //
        // C-04: la comprobación mira SOLO la marca de tiempo del fichero (sin ReadAllText ni
        // JsonDocument.Parse); el cuerpo, si hace falta, se lee y parsea en un hilo de fondo.
        // Esta guarda es previa a cualquier await, y este método se invoca desde el hilo de UI.
        if (_ultimaDescargaUtc.TryGetValue(paisNorm, out DateTime ultima) &&
            (DateTime.UtcNow - ultima).TotalSeconds < VentanaAntiDobleSegundos &&
            JornadaCache.FechaGuardado(paisNorm) is not null)
        {
            ResultadoJornada? deCache = await Task.Run(() =>
            {
                return JornadaCache.TryCargar(paisNorm, out JornadaQuiniela? c, out DateTime g) && c is not null
                    ? new ResultadoJornada(c, true, g)
                    : null;
            }, ct).ConfigureAwait(false);

            // Si la caché resultó ilegible pese a existir el fichero, se sigue a la red
            // (mismo comportamiento que antes: la guarda solo corta si hay caché VÁLIDA).
            if (deCache is not null) return deCache;
        }

        // Raíz del host + prefijo WordPress del backend + recurso.
        string url = BaseUrl.TrimEnd('/') + PrefijoApi + "/quiniela/" + paisNorm + "/actual";

        string cuerpo = await DescargarConReintentoAsync(url, ct).ConfigureAwait(false);

        // El cuerpo es DATO no confiable: el parser valida/parsea a la defensiva.
        JornadaQuiniela jornada;
        try
        {
            jornada = JornadaQuinielaParser.Parse(cuerpo);
        }
        catch (FormatException ex)
        {
            throw new QuinielaOnlineException(
                "La respuesta del servicio no es una jornada válida: " + ex.Message, ex);
        }

        // Descarga + parseo correctos: persiste los bytes CRUDOS en la caché local para que el
        // próximo arranque muestre los equipos reales offline sin reconsultar. Es best-effort:
        // JornadaCache.Guardar nunca lanza, así que un fallo de E/S no afecta a la descarga.
        JornadaCache.Guardar(paisNorm, cuerpo);

        // B-05: la marca anti-doble se pone SOLO tras un éxito. Marcarla antes del GET hacía que
        // un fallo de red (timeout / 429 / sin conexión) bloqueara el siguiente intento durante
        // 60 s devolviendo la caché COMO SI fuese fresca, aunque ya hubiese vuelto la conexión.
        // El doble clic sigue cubierto por las 3 capas de la pantalla (botón deshabilitado,
        // AsyncRelayCommand sin concurrencia y el guard `if (Descargando) return;`).
        _ultimaDescargaUtc[paisNorm] = DateTime.UtcNow;

        return new ResultadoJornada(jornada, false, JornadaCache.FechaGuardado(paisNorm));
    }

    /// <summary>
    /// Descarga y parsea el CATÁLOGO de equipos del país indicado ("es" o "mx") desde
    /// <c>GET {base}/wp-json/clubprogol/v1/equipos/{pais}</c>. Reutiliza el mismo HttpClient,
    /// base URL, prefijo, manejo de 429 y traducción de errores que <see cref="ObtenerJornadaAsync"/>.
    ///
    /// El cuerpo se trata SIEMPRE como dato no confiable: se valida/parsea con el parser PURO
    /// (<see cref="CatalogoEquiposParser"/>). No se cachea (a diferencia de la jornada): la importación
    /// del catálogo es una acción explícita y puntual del usuario en "Gestión de Equipos".
    /// Lanza <see cref="QuinielaOnlineException"/> con un mensaje claro ante cualquier error de
    /// red/HTTP/parseo (el llamador lo convierte en el mensaje offline amigable).
    /// </summary>
    public async Task<CatalogoEquipos> ObtenerEquiposAsync(string pais, CancellationToken ct = default)
    {
        string paisNorm = (pais ?? "").Trim().ToLowerInvariant();
        if (paisNorm != "es" && paisNorm != "mx")
        {
            throw new QuinielaOnlineException("País no soportado: '" + pais + "' (use 'es' o 'mx').");
        }

        // Raíz del host + prefijo WordPress del backend + recurso.
        string url = BaseUrl.TrimEnd('/') + PrefijoApi + "/equipos/" + paisNorm;

        string cuerpo = await DescargarConReintentoAsync(url, ct).ConfigureAwait(false);

        try
        {
            return CatalogoEquiposParser.Parse(cuerpo);
        }
        catch (FormatException ex)
        {
            throw new QuinielaOnlineException(
                "La respuesta del servicio no es un catálogo de equipos válido: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// Hace el GET y devuelve el cuerpo si es 2xx. En 429 con <c>Retry-After</c> corto
    /// (≤ <see cref="RetryAfterMaxSegundos"/> s) reintenta UNA vez; si no, falla rápido sin
    /// bloquear la UI. Cualquier otro no-2xx se traduce en <see cref="QuinielaOnlineException"/>
    /// propagando el <c>mensaje</c> del cuerpo de error JSON cuando existe.
    /// </summary>
    private static async Task<string> DescargarConReintentoAsync(string url, CancellationToken ct)
    {
        bool reintentoUsado = false;

        while (true)
        {
            try
            {
                using HttpResponseMessage resp = await _http
                    .GetAsync(url, HttpCompletionOption.ResponseContentRead, ct)
                    .ConfigureAwait(false);

                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                }

                // Cuerpo de error: lo leemos como DATO para extraer { error, mensaje }.
                string cuerpoError = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

                // 429: límite de peticiones (60/min). Retry-After indica los segundos a esperar.
                if (resp.StatusCode == (HttpStatusCode)429)
                {
                    int? retryAfter = LeerRetryAfter(resp);

                    // Reintento acotado SOLO si la espera es corta y no lo hemos usado ya.
                    if (!reintentoUsado && retryAfter.HasValue && retryAfter.Value > 0 &&
                        retryAfter.Value <= RetryAfterMaxSegundos)
                    {
                        reintentoUsado = true;
                        await Task.Delay(TimeSpan.FromSeconds(retryAfter.Value), ct).ConfigureAwait(false);
                        continue; // un único reintento
                    }

                    int espera = retryAfter ?? 60;
                    throw new QuinielaOnlineException(
                        MensajeDelCuerpo(cuerpoError) ??
                        "Demasiadas solicitudes (límite 60/min). Intenta de nuevo en ~" + espera + " s.");
                }

                // Resto de no-2xx (404 sin_jornada, 503, …): propagamos el mensaje del servidor.
                throw new QuinielaOnlineException(
                    MensajeDelCuerpo(cuerpoError) ??
                    "El servicio respondió con código " + (int)resp.StatusCode +
                    " (" + resp.StatusCode + ").");
            }
            catch (QuinielaOnlineException)
            {
                throw;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Cancelación explícita del llamador: se propaga tal cual (no es un error de red).
                throw;
            }
            catch (TaskCanceledException ex)
            {
                // Timeout del HttpClient (cancelación NO solicitada por el llamador).
                throw new QuinielaOnlineException(
                    "Se agotó el tiempo de espera al contactar con el servicio online.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new QuinielaOnlineException(
                    "No se pudo contactar con el servicio online (sin conexión o servidor no disponible).", ex);
            }
        }
    }

    /// <summary>
    /// Lee la cabecera <c>Retry-After</c> y la traduce a SEGUNDOS de espera. Soporta los dos
    /// formatos del RFC: delta-seconds (numérico) y HTTP-date (C-09; antes se ignoraba y se caía
    /// al genérico "~60 s"). Devuelve null si no está presente o no es interpretable.
    /// </summary>
    private static int? LeerRetryAfter(HttpResponseMessage resp)
    {
        var ra = resp.Headers.RetryAfter;
        if (ra?.Delta is TimeSpan delta)
        {
            return (int)Math.Ceiling(delta.TotalSeconds);
        }
        if (ra?.Date is DateTimeOffset fecha)
        {
            // HTTP-date: la espera es la diferencia con "ahora". Una fecha ya pasada (o un reloj
            // desfasado) significa "puedes reintentar ya": se devuelve 1 s, no 0, para que siga
            // cayendo en la rama del reintento acotado en vez de parecer "sin cabecera".
            double segundos = (fecha - DateTimeOffset.UtcNow).TotalSeconds;
            return segundos <= 1 ? 1 : (int)Math.Ceiling(segundos);
        }
        if (resp.Headers.TryGetValues("Retry-After", out var valores))
        {
            foreach (string v in valores)
            {
                if (int.TryParse((v ?? "").Trim(), out int seg)) return seg;
            }
        }
        return null;
    }

    /// <summary>
    /// Intenta extraer el campo <c>mensaje</c> (o, en su defecto, <c>error</c>) del cuerpo de
    /// error JSON <c>{ "error": "...", "mensaje": "..." }</c>. Devuelve null si el cuerpo no es
    /// JSON o no trae esos campos (el llamador usa entonces un mensaje genérico).
    /// </summary>
    private static string? MensajeDelCuerpo(string cuerpo)
    {
        if (string.IsNullOrWhiteSpace(cuerpo)) return null;
        try
        {
            using JsonDocument doc = JsonDocument.Parse(cuerpo);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;

            if (doc.RootElement.TryGetProperty("mensaje", out JsonElement m) &&
                m.ValueKind == JsonValueKind.String)
            {
                string s = m.GetString() ?? "";
                if (!string.IsNullOrWhiteSpace(s)) return s;
            }
            if (doc.RootElement.TryGetProperty("error", out JsonElement e) &&
                e.ValueKind == JsonValueKind.String)
            {
                string s = e.GetString() ?? "";
                if (!string.IsNullOrWhiteSpace(s)) return s;
            }
        }
        catch (JsonException)
        {
            // Cuerpo de error no JSON (HTML de error, vacío, …): el llamador usa el genérico.
        }
        return null;
    }
}
