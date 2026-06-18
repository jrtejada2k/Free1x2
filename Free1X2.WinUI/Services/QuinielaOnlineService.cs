// Free1X2 · WinUI 3 — WIN3
using System;
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
    /// </summary>
    public async Task<JornadaQuiniela> ObtenerJornadaAsync(string pais, CancellationToken ct = default)
    {
        string paisNorm = (pais ?? "").Trim().ToLowerInvariant();
        if (paisNorm != "es" && paisNorm != "mx")
        {
            throw new QuinielaOnlineException("País no soportado: '" + pais + "' (use 'es' o 'mx').");
        }

        // Raíz del host + prefijo WordPress del backend + recurso.
        string url = BaseUrl.TrimEnd('/') + PrefijoApi + "/quiniela/" + paisNorm + "/actual";

        string cuerpo = await DescargarConReintentoAsync(url, ct).ConfigureAwait(false);

        // El cuerpo es DATO no confiable: el parser valida/parsea a la defensiva.
        try
        {
            return JornadaQuinielaParser.Parse(cuerpo);
        }
        catch (FormatException ex)
        {
            throw new QuinielaOnlineException(
                "La respuesta del servicio no es una jornada válida: " + ex.Message, ex);
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
    /// Lee la cabecera <c>Retry-After</c> (segundos). Soporta el formato numérico (delta-seconds);
    /// devuelve null si no está presente o no es un entero de segundos.
    /// </summary>
    private static int? LeerRetryAfter(HttpResponseMessage resp)
    {
        var ra = resp.Headers.RetryAfter;
        if (ra?.Delta is TimeSpan delta)
        {
            return (int)Math.Ceiling(delta.TotalSeconds);
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
