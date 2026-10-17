// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Free1X2.Online
{
    /// <summary>
    /// Equipos de la jornada en el boleto de Inicio (W6, DT-81) y descarga al arrancar con plazo
    /// (W7, DT-83). Lógica pura para poder probarla; mismo comportamiento que la app Mac
    /// (<c>CasosArchivo.equiposDeJornada</c>, <c>CasosInicio.aplicarEquipos</c>,
    /// <c>CasosArchivo.jornadaAlArrancar</c>).
    /// </summary>
    public static class JornadaInicio
    {
        /// <summary>Nombre de un partido sin equipos (igual que <c>Pronosticos.SetEquiposVacio()</c>).</summary>
        public const string SinEquipos = "? - ?";

        /// <summary>
        /// «Local - Visitante» de los <paramref name="partidos"/> primeros partidos de la jornada;
        /// el 15.º es el pleno, si lo hay. Local y visitante vacíos ⇒ <see cref="SinEquipos"/>.
        /// </summary>
        public static string[] Componer(JornadaQuiniela jornada, int partidos)
        {
            var todos = new List<PartidoJornada>(jornada.Partidos);
            if (jornada.Pleno15 != null) todos.Add(jornada.Pleno15);

            int n = Math.Min(partidos, todos.Count);
            var equipos = new string[n];
            for (int i = 0; i < n; i++)
            {
                string local = todos[i].Local ?? "";
                string visitante = todos[i].Visitante ?? "";
                equipos[i] = local.Trim().Length == 0 && visitante.Trim().Length == 0
                    ? SinEquipos
                    : local + " - " + visitante;
            }
            return equipos;
        }

        /// <summary>
        /// Pone <paramref name="nuevos"/> sobre los primeros partidos de <paramref name="previos"/>;
        /// los que sobran quedan como estaban (p. ej. el 15.º si la jornada no trae pleno).
        /// </summary>
        public static string[] Aplicar(string[] nuevos, string[] previos)
        {
            var r = (string[])previos.Clone();
            for (int i = 0; i < r.Length && i < nuevos.Length; i++)
            {
                r[i] = string.IsNullOrEmpty(nuevos[i]) ? SinEquipos : nuevos[i];
            }
            return r;
        }

        /// <summary>True si ningún partido tiene equipos (todo vacío o <see cref="SinEquipos"/>).</summary>
        public static bool EstanVacios(IEnumerable<string> equipos)
        {
            foreach (string e in equipos)
            {
                if (!string.IsNullOrWhiteSpace(e) && e.Trim() != SinEquipos) return false;
            }
            return true;
        }

        /// <summary>
        /// Al arrancar (DT-83): la jornada si <paramref name="obtener"/> responde antes de
        /// <paramref name="limite"/>; si tarda, falla o no hay red, <c>null</c> sin aviso (el
        /// boleto queda en blanco y no se usa la caché). Al vencer el plazo cancela la descarga.
        /// </summary>
        public static async Task<JornadaQuiniela> AlArrancarAsync(
            Func<CancellationToken, Task<JornadaQuiniela>> obtener, TimeSpan limite)
        {
            using var cts = new CancellationTokenSource(limite);
            try
            {
                Task<JornadaQuiniela> descarga = obtener(cts.Token);
                Task reloj = Task.Delay(limite);
                Task primera = await Task.WhenAny(descarga, reloj).ConfigureAwait(false);
                if (primera != descarga)
                {
                    cts.Cancel();
                    return null;
                }
                return await descarga.ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
