// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Free1X2.Online
{
    /// <summary>La columna ganadora de una jornada pasada (W12, DT-88).</summary>
    public sealed class ResultadoHistorico
    {
        public int Jornada { get; set; }

        /// <summary>«AAAA-MM-DD»; puede faltar o ser null (vacío).</summary>
        public string Fecha { get; set; } = "";

        /// <summary>Los 14 signos ganadores (1, X o 2), en mayúsculas.</summary>
        public string Ganadora { get; set; } = "";
    }

    /// <summary>
    /// Parser PURO del JSON de <c>GET {base}/wp-json/clubprogol/v1/quiniela/{pais}/historico</c>
    /// (contrato: docs/API_CLUBPROGOL.md §4). Validación estricta: si UNA ganadora no tiene 14
    /// signos 1/X/2 se rechaza la respuesta entera (mejor avisar que escrutar con datos rotos).
    /// Mismo comportamiento que la app Mac (<c>HistoricoQuiniela.analizar</c>).
    /// </summary>
    public static class HistoricoQuinielaParser
    {
        public static List<ResultadoHistorico> Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new FormatException("La respuesta del histórico está vacía.");
            }

            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new FormatException("La respuesta del histórico no es un JSON válido: " + ex.Message, ex);
            }

            using (doc)
            {
                JsonElement raiz = doc.RootElement;
                if (raiz.ValueKind != JsonValueKind.Object ||
                    !raiz.TryGetProperty("resultados", out JsonElement lista) ||
                    lista.ValueKind != JsonValueKind.Array)
                {
                    throw new FormatException("Falta el array 'resultados' en el histórico.");
                }

                var r = new List<ResultadoHistorico>();
                foreach (JsonElement v in lista.EnumerateArray())
                {
                    if (v.ValueKind != JsonValueKind.Object)
                    {
                        throw new FormatException("Un resultado del histórico no es un objeto JSON.");
                    }

                    int jornada = LeerEntero(v, "jornada");
                    string ganadora = LeerCadena(v, "ganadora", obligatorio: true).ToUpperInvariant();
                    if (ganadora.Length != 14 || ganadora.Any(c => c != '1' && c != 'X' && c != '2'))
                    {
                        throw new FormatException("La ganadora de la jornada " + jornada +
                                                  " debe tener 14 signos 1, X o 2: «" + ganadora + "».");
                    }
                    r.Add(new ResultadoHistorico
                    {
                        Jornada = jornada,
                        Fecha = LeerCadena(v, "fecha", obligatorio: false),
                        Ganadora = ganadora,
                    });
                }
                return r;
            }
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

        private static int LeerEntero(JsonElement obj, string nombre)
        {
            if (!obj.TryGetProperty(nombre, out JsonElement el) || el.ValueKind == JsonValueKind.Null)
            {
                throw new FormatException("Falta el campo obligatorio '" + nombre + "'.");
            }
            if (el.ValueKind != JsonValueKind.Number || !el.TryGetInt32(out int valor))
            {
                throw new FormatException("El campo '" + nombre + "' debe ser un número entero.");
            }
            return valor;
        }
    }
}
