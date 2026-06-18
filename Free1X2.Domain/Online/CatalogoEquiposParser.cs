// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Free1X2.Online
{
    /// <summary>
    /// Parser PURO (sin red, sin UI) que convierte el JSON del endpoint
    /// <c>GET {base}/wp-json/clubprogol/v1/equipos/{pais}</c> en un <see cref="CatalogoEquipos"/>.
    ///
    /// La respuesta de red se trata SIEMPRE como datos no confiables: se valida a la defensiva
    /// (al menos una división, cada división con al menos un equipo de nombre no vacío) y se lanza
    /// un <see cref="FormatException"/> claro ante cualquier dato inválido o JSON malformado.
    /// No se ejecuta nada del cuerpo; solo se leen campos conocidos.
    ///
    /// El backend actual sirve una ÚNICA división con <c>id = "all"</c>, pero el parser admite
    /// varias divisiones sin cambios (futuro multi-división).
    ///
    /// Contrato: docs/API_CLUBPROGOL.md (ejemplos: docs/ejemplos-api/equipos-es.json, equipos-mx.json).
    /// </summary>
    public static class CatalogoEquiposParser
    {
        /// <summary>
        /// Parsea el JSON del catálogo de equipos. Lanza <see cref="FormatException"/> con un
        /// mensaje claro si el JSON está malformado o no cumple el contrato (sin divisiones,
        /// división sin equipos, nombres vacíos, etc.).
        /// </summary>
        /// <param name="json">Cuerpo de la respuesta HTTP (UTF-8 ya decodificado a string).</param>
        public static CatalogoEquipos Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new FormatException("La respuesta del catálogo de equipos está vacía.");
            }

            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new FormatException(
                    "La respuesta del catálogo de equipos no es un JSON válido: " + ex.Message, ex);
            }

            using (doc)
            {
                JsonElement raiz = doc.RootElement;
                if (raiz.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException("La respuesta del catálogo de equipos no es un objeto JSON.");
                }

                var catalogo = new CatalogoEquipos
                {
                    Pais = LeerCadena(raiz, "pais", obligatorio: true),
                };

                if (!raiz.TryGetProperty("divisiones", out JsonElement divisionesEl) ||
                    divisionesEl.ValueKind != JsonValueKind.Array)
                {
                    throw new FormatException("Falta el array 'divisiones' en el catálogo de equipos.");
                }

                var divisiones = new List<DivisionEquipos>();
                foreach (JsonElement divEl in divisionesEl.EnumerateArray())
                {
                    divisiones.Add(LeerDivision(divEl));
                }

                if (divisiones.Count == 0)
                {
                    throw new FormatException("El catálogo de equipos debe tener al menos una división.");
                }

                catalogo.Divisiones = divisiones;
                return catalogo;
            }
        }

        private static DivisionEquipos LeerDivision(JsonElement el)
        {
            if (el.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException("Una división del catálogo no es un objeto JSON.");
            }

            string id = LeerCadena(el, "id", obligatorio: true);
            string nombre = LeerCadena(el, "nombre", obligatorio: false);

            if (!el.TryGetProperty("equipos", out JsonElement equiposEl) ||
                equiposEl.ValueKind != JsonValueKind.Array)
            {
                throw new FormatException("Falta el array 'equipos' en la división '" + id + "'.");
            }

            var equipos = new List<string>();
            foreach (JsonElement equipoEl in equiposEl.EnumerateArray())
            {
                if (equipoEl.ValueKind != JsonValueKind.String)
                {
                    throw new FormatException(
                        "Los equipos de la división '" + id + "' deben ser texto.");
                }
                string nombreEquipo = equipoEl.GetString() ?? "";
                if (string.IsNullOrWhiteSpace(nombreEquipo))
                {
                    throw new FormatException(
                        "La división '" + id + "' contiene un nombre de equipo vacío.");
                }
                equipos.Add(nombreEquipo);
            }

            if (equipos.Count == 0)
            {
                throw new FormatException("La división '" + id + "' no tiene equipos.");
            }

            return new DivisionEquipos { Id = id, Nombre = nombre, Equipos = equipos };
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
    }
}
