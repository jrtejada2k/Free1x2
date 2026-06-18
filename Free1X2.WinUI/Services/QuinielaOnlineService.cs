// Free1X2 · WinUI 3 — WIN3
using System;
using System.Net.Http;
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
/// (<c>GET {base}/api/v1/quiniela/{pais}/actual</c>) y la parsea con el parser PURO de
/// dominio (<see cref="JornadaQuinielaParser"/>).
///
/// Principios (docs/API_CLUBPROGOL.md):
///   · La respuesta de red SIEMPRE se trata como DATOS: se valida/parsea a la defensiva,
///     nunca se ejecuta nada del cuerpo.
///   · Offline-first: cualquier fallo (sin conexión, timeout, HTTP de error, JSON inválido)
///     se traduce en una <see cref="QuinielaOnlineException"/> clara; el llamador cae a modo
///     manual sin romper nada.
///   · Base URL: variable de entorno <c>FREE1X2_API_BASE</c> si está definida (permite
///     apuntar a un stub local mientras no exista el backend), si no el valor por defecto
///     <c>https://clubprogol.com</c>. Sin secretos ni autenticación (datos públicos).
/// </summary>
public sealed class QuinielaOnlineService
{
    /// <summary>Base URL por defecto del servicio (contrato: docs/API_CLUBPROGOL.md).</summary>
    public const string BaseUrlPorDefecto = "https://clubprogol.com";

    /// <summary>Nombre de la variable de entorno que permite sobreescribir la base URL.</summary>
    public const string EnvBaseUrl = "FREE1X2_API_BASE";

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

        string url = BaseUrl.TrimEnd('/') + "/api/v1/quiniela/" + paisNorm + "/actual";

        string cuerpo;
        try
        {
            using HttpResponseMessage resp = await _http
                .GetAsync(url, HttpCompletionOption.ResponseContentRead, ct)
                .ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                throw new QuinielaOnlineException(
                    "El servicio respondió con código " + (int)resp.StatusCode +
                    " (" + resp.StatusCode + ").");
            }

            cuerpo = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
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
}
