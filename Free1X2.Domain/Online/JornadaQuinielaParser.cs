// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Free1X2.Online
{
    /// <summary>
    /// Parser PURO (sin red, sin UI) que convierte el JSON del endpoint
    /// <c>GET {base}/wp-json/clubprogol/v1/quiniela/{pais}/actual</c> en un <see cref="JornadaQuiniela"/>.
    ///
    /// La respuesta de red se trata SIEMPRE como datos no confiables: se valida a la
    /// defensiva (14 partidos, números 1..14 en orden, nombres no vacíos) y se lanza un
    /// <see cref="FormatException"/> claro ante cualquier dato inválido o JSON malformado.
    /// No se ejecuta nada del cuerpo; solo se leen campos conocidos.
    ///
    /// Contrato: docs/API_CLUBPROGOL.md.
    /// </summary>
    public static class JornadaQuinielaParser
    {
        /// <summary>Número de partidos obligatorios de una quiniela.</summary>
        public const int NumeroPartidos = 14;

        /// <summary>
        /// Parsea el JSON de una jornada actual. Lanza <see cref="FormatException"/> con un
        /// mensaje claro si el JSON está malformado o no cumple el contrato (faltan campos,
        /// número de partidos ≠ 14, nombres vacíos, etc.).
        /// </summary>
        /// <param name="json">Cuerpo de la respuesta HTTP (UTF-8 ya decodificado a string).</param>
        public static JornadaQuiniela Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new FormatException("La respuesta de la jornada está vacía.");
            }

            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new FormatException("La respuesta de la jornada no es un JSON válido: " + ex.Message, ex);
            }

            using (doc)
            {
                JsonElement raiz = doc.RootElement;
                if (raiz.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException("La respuesta de la jornada no es un objeto JSON.");
                }

                var jornada = new JornadaQuiniela
                {
                    Pais = LeerCadena(raiz, "pais", obligatorio: true),
                    Temporada = LeerCadena(raiz, "temporada", obligatorio: true),
                    Jornada = LeerEntero(raiz, "jornada", obligatorio: true),
                    Fecha = LeerCadena(raiz, "fecha", obligatorio: false),
                };

                if (!raiz.TryGetProperty("partidos", out JsonElement partidosEl) ||
                    partidosEl.ValueKind != JsonValueKind.Array)
                {
                    throw new FormatException("Falta el array 'partidos' en la jornada.");
                }

                var partidos = new List<PartidoJornada>();
                int posicionEsperada = 1;
                foreach (JsonElement partidoEl in partidosEl.EnumerateArray())
                {
                    partidos.Add(LeerPartido(partidoEl, posicionEsperada));
                    posicionEsperada++;
                }

                if (partidos.Count != NumeroPartidos)
                {
                    throw new FormatException(
                        "La jornada debe tener exactamente " + NumeroPartidos +
                        " partidos; se recibieron " + partidos.Count + ".");
                }

                jornada.Partidos = partidos;

                // Pleno al 15: opcional (solo España). Si está presente se valida igual.
                if (raiz.TryGetProperty("pleno15", out JsonElement plenoEl) &&
                    plenoEl.ValueKind == JsonValueKind.Object)
                {
                    jornada.Pleno15 = LeerPartido(plenoEl, posicionEsperada: 15);
                }

                return jornada;
            }
        }

        private static PartidoJornada LeerPartido(JsonElement el, int posicionEsperada)
        {
            if (el.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException("El partido nº " + posicionEsperada + " no es un objeto JSON.");
            }

            int n = LeerEntero(el, "n", obligatorio: true);
            if (n != posicionEsperada)
            {
                throw new FormatException(
                    "Los partidos deben venir en orden: se esperaba n=" + posicionEsperada +
                    " y se recibió n=" + n + ".");
            }

            string local = LeerCadena(el, "local", obligatorio: true);
            string visitante = LeerCadena(el, "visitante", obligatorio: true);

            if (string.IsNullOrWhiteSpace(local))
            {
                throw new FormatException("El equipo local del partido nº " + n + " está vacío.");
            }
            if (string.IsNullOrWhiteSpace(visitante))
            {
                throw new FormatException("El equipo visitante del partido nº " + n + " está vacío.");
            }

            return new PartidoJornada { N = n, Local = local, Visitante = visitante };
        }

        private static string LeerCadena(JsonElement obj, string nombre, bool obligatorio)
        {
            if (!obj.TryGetProperty(nombre, out JsonElement el) || el.ValueKind == JsonValueKind.Null)
            {
                if (obligatorio)
                {
                    throw new FormatException("Falta el campo obligatorio '" + nombre + "'.");
                }
                return "";
            }
            if (el.ValueKind != JsonValueKind.String)
            {
                throw new FormatException("El campo '" + nombre + "' debe ser texto.");
            }
            return el.GetString() ?? "";
        }

        private static int LeerEntero(JsonElement obj, string nombre, bool obligatorio)
        {
            if (!obj.TryGetProperty(nombre, out JsonElement el) || el.ValueKind == JsonValueKind.Null)
            {
                if (obligatorio)
                {
                    throw new FormatException("Falta el campo obligatorio '" + nombre + "'.");
                }
                return 0;
            }
            if (el.ValueKind != JsonValueKind.Number || !el.TryGetInt32(out int valor))
            {
                throw new FormatException("El campo '" + nombre + "' debe ser un número entero.");
            }
            return valor;
        }
    }
}
