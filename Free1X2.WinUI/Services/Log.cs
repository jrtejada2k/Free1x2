// Free1X2 · WinUI 3 — WIN3
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Log MÍNIMO de diagnóstico a fichero, para que ningún error se pierda en silencio.
/// Antes de esto, todos los <c>catch</c> de la capa WinUI se tragaban la excepción sin dejar
/// rastro: cuando el usuario reportaba un fallo no había NADA que mirar.
///
/// Diseño (deliberadamente simple: es una red de seguridad, no una infraestructura):
///   · Destino: <c>%LocalAppData%\Free1X2\log.txt</c> — la MISMA carpeta que
///     <see cref="JornadaCache"/> (se reutiliza <see cref="JornadaCache.Carpeta"/>), siempre
///     escribible por el usuario aunque la app esté instalada en una ruta de solo lectura.
///   · Rotación simple: al superar ~1 MB, el fichero pasa a <c>log.1.txt</c> (sobrescribiendo
///     el anterior) y se empieza uno nuevo. Nunca crece sin control ni necesita mantenimiento.
///   · Thread-safe: toda la E/S va dentro de un <c>lock</c> (el motor corre en hilos de fondo).
///   · NUNCA lanza: cada operación está envuelta en try/catch. Un fallo del log (permisos,
///     disco lleno, fichero bloqueado) jamás puede romper la app ni alterar el flujo del
///     llamador; el log es SIEMPRE un añadido a un comportamiento que ya funciona.
///   · Además de al fichero, escribe a <see cref="System.Diagnostics.Debug"/> para verlo en
///     vivo con el depurador adjunto.
/// </summary>
public static class Log
{
    /// <summary>Nombre del fichero activo dentro de la carpeta de datos de la app.</summary>
    private const string NombreFichero = "log.txt";

    /// <summary>Nombre del fichero rotado (el anterior). Se sobrescribe en cada rotación.</summary>
    private const string NombreFicheroRotado = "log.1.txt";

    /// <summary>Tamaño a partir del cual se rota (~1 MB).</summary>
    private const long TamanoMaximoBytes = 1024 * 1024;

    /// <summary>Serializa la escritura: el motor y la UI escriben desde hilos distintos.</summary>
    private static readonly object _candado = new();

    /// <summary>Ruta del fichero activo (misma carpeta base que la caché de jornada).</summary>
    public static string Ruta => Path.Combine(JornadaCache.Carpeta, NombreFichero);

    /// <summary>Registra un error con su contexto y la excepción (tipo, mensaje y pila).</summary>
    public static void Error(string contexto, Exception? ex)
    {
        Escribir("ERROR", ex is null ? contexto : contexto + " :: " + Describir(ex));
    }

    /// <summary>Registra un error sin excepción asociada (fallo detectado por código propio).</summary>
    public static void Error(string mensaje) => Escribir("ERROR", mensaje);

    /// <summary>Registra una advertencia (algo no fue como se esperaba, pero hay fallback).</summary>
    public static void Warn(string mensaje) => Escribir("WARN", mensaje);

    /// <summary>Registra información de traza (arranque, acciones relevantes).</summary>
    public static void Info(string mensaje) => Escribir("INFO", mensaje);

    /// <summary>
    /// Aplana una excepción a una línea legible: tipo, mensaje, pila y la cadena de
    /// <c>InnerException</c> (la causa real suele estar en el inner, no en el exterior).
    /// </summary>
    private static string Describir(Exception ex)
    {
        var sb = new StringBuilder();
        Exception? actual = ex;
        int nivel = 0;
        while (actual is not null && nivel < 5) // tope: evita ciclos/cadenas absurdas
        {
            if (nivel > 0) sb.Append(" <- ");
            sb.Append(actual.GetType().FullName).Append(": ").Append(actual.Message);
            actual = actual.InnerException;
            nivel++;
        }
        if (!string.IsNullOrEmpty(ex.StackTrace))
        {
            sb.Append(Environment.NewLine).Append(ex.StackTrace);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Núcleo de escritura. Todo va en try/catch: este método NO puede lanzar bajo ninguna
    /// circunstancia (lo llaman catches cuya única misión es no romper el flujo del usuario).
    /// </summary>
    private static void Escribir(string nivel, string mensaje)
    {
        string linea =
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) +
            "Z [" + nivel + "] " + (mensaje ?? "");

        try { System.Diagnostics.Debug.WriteLine("[Free1X2] " + linea); } catch { }

        try
        {
            lock (_candado)
            {
                string carpeta = JornadaCache.Carpeta;
                Directory.CreateDirectory(carpeta);

                string ruta = Path.Combine(carpeta, NombreFichero);
                RotarSiProcede(carpeta, ruta);

                File.AppendAllText(ruta, linea + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Permisos, disco lleno, fichero bloqueado… El log es best-effort por definición:
            // si no se puede escribir, se pierde la traza pero la app sigue exactamente igual.
        }
    }

    /// <summary>
    /// Rotación simple: si el fichero activo supera el tamaño máximo, se mueve a
    /// <c>log.1.txt</c> (sobrescribiendo el rotado anterior) y el siguiente append crea uno nuevo.
    /// Se invoca SIEMPRE dentro del <c>lock</c>. No lanza: si la rotación falla se sigue
    /// escribiendo en el fichero actual (peor tener un log grande que no tener log).
    /// </summary>
    private static void RotarSiProcede(string carpeta, string ruta)
    {
        try
        {
            var info = new FileInfo(ruta);
            if (!info.Exists || info.Length < TamanoMaximoBytes) return;

            File.Move(ruta, Path.Combine(carpeta, NombreFicheroRotado), overwrite: true);
        }
        catch
        {
            // Fichero en uso por otro proceso, permisos… se continúa sin rotar.
        }
    }
}
