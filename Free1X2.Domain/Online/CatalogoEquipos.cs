// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;

namespace Free1X2.Online
{
    /// <summary>
    /// Modelo de dominio del CATÁLOGO de equipos descargado del servicio online
    /// (clubprogol.com). Representa la lista de equipos vistos en jornadas recientes de un país,
    /// organizada en una o más "divisiones". El backend actual sirve una ÚNICA división con
    /// <c>id = "all"</c> ("Equipos (jornadas recientes)"), pero el modelo admite varias para
    /// no atarse a esa forma concreta (futuro multi-división).
    ///
    /// Es un tipo PURO de dominio (sin WinForms/WinUI): se obtiene parseando el JSON del
    /// endpoint <c>GET {base}/wp-json/clubprogol/v1/equipos/{pais}</c> mediante
    /// <see cref="CatalogoEquiposParser"/>. La respuesta de red SIEMPRE se trata como DATOS:
    /// se valida y se parsea a la defensiva; nunca se ejecuta nada del cuerpo.
    ///
    /// Contrato: docs/API_CLUBPROGOL.md (ejemplos: docs/ejemplos-api/equipos-es.json, equipos-mx.json).
    /// </summary>
    public sealed class CatalogoEquipos
    {
        /// <summary>Código de país: "ES" (España) o "MX" (México).</summary>
        public string Pais { get; set; } = "";

        /// <summary>Divisiones del catálogo (al menos una). El backend actual trae solo "all".</summary>
        public IReadOnlyList<DivisionEquipos> Divisiones { get; set; } = new List<DivisionEquipos>();

        /// <summary>
        /// Devuelve la lista PLANA de equipos de TODAS las divisiones, sin duplicados, preservando el
        /// orden de primera aparición. Cubre tanto el caso de una sola división "all" como cualquier
        /// catálogo multi-división futuro. La comparación de duplicados ignora mayúsculas y espacios
        /// extremos (p. ej. "Real Madrid" y "real madrid " se consideran el mismo equipo).
        /// </summary>
        public IReadOnlyList<string> EquiposPlano()
        {
            var resultado = new List<string>();
            var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DivisionEquipos division in Divisiones)
            {
                foreach (string equipo in division.Equipos)
                {
                    if (string.IsNullOrWhiteSpace(equipo)) continue;
                    string clave = equipo.Trim();
                    if (vistos.Add(clave))
                    {
                        resultado.Add(clave);
                    }
                }
            }
            return resultado;
        }
    }

    /// <summary>
    /// Una división del catálogo: identificador, nombre legible y la lista de nombres de equipos.
    /// </summary>
    public sealed class DivisionEquipos
    {
        /// <summary>Identificador de la división ("all", "1", "2", …).</summary>
        public string Id { get; set; } = "";

        /// <summary>Nombre legible de la división ("Equipos (jornadas recientes)", "Primera", …).</summary>
        public string Nombre { get; set; } = "";

        /// <summary>Nombres de los equipos de la división (al menos uno, no vacíos).</summary>
        public IReadOnlyList<string> Equipos { get; set; } = new List<string>();
    }
}
