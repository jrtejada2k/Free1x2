// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;

namespace Free1X2.Online
{
    /// <summary>
    /// Modelo de dominio de una jornada de la Quiniela descargada del servicio online
    /// (clubprogol.com). Representa los 14 partidos de la semana con sus equipos reales y,
    /// opcionalmente, el Pleno al 15 (solo España).
    ///
    /// Es un tipo PURO de dominio (sin WinForms/WinUI): se obtiene parseando el JSON del
    /// endpoint <c>GET {base}/wp-json/clubprogol/v1/quiniela/{pais}/actual</c> mediante
    /// <see cref="JornadaQuinielaParser"/>. La respuesta de red SIEMPRE se trata como DATOS:
    /// se valida y se parsea a la defensiva; nunca se ejecuta nada del cuerpo.
    ///
    /// Contrato: docs/API_CLUBPROGOL.md.
    /// </summary>
    public sealed class JornadaQuiniela
    {
        /// <summary>Código de país: "ES" (España) o "MX" (México).</summary>
        public string Pais { get; set; } = "";

        /// <summary>Temporada en formato libre, p. ej. "2025/26" o "2026".</summary>
        public string Temporada { get; set; } = "";

        /// <summary>Número de jornada (1..n).</summary>
        public int Jornada { get; set; }

        /// <summary>Fecha de la jornada (YYYY-MM-DD). Puede venir vacía.</summary>
        public string Fecha { get; set; } = "";

        /// <summary>Los 14 partidos en orden (n = 1..14).</summary>
        public IReadOnlyList<PartidoJornada> Partidos { get; set; } = new List<PartidoJornada>();

        /// <summary>Pleno al 15, opcional (solo España; null si no aplica).</summary>
        public PartidoJornada Pleno15 { get; set; }
    }

    /// <summary>
    /// Un partido de la jornada: número de posición (1..14, o 15 para el Pleno) y los
    /// nombres reales de los equipos local y visitante tal y como deben mostrarse.
    /// </summary>
    public sealed class PartidoJornada
    {
        /// <summary>Posición del partido (1..14; 15 para el Pleno al 15).</summary>
        public int N { get; set; }

        /// <summary>Nombre del equipo local (no vacío).</summary>
        public string Local { get; set; } = "";

        /// <summary>Nombre del equipo visitante (no vacío).</summary>
        public string Visitante { get; set; } = "";
    }
}
