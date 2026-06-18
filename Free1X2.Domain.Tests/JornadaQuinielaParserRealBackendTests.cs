using System;
using System.IO;
using System.Linq;
using Free1X2.Online;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Tests del parser PURO (<see cref="JornadaQuinielaParser"/>) contra las RESPUESTAS REALES
    /// capturadas del backend EN PRODUCCIÓN (clubprogol.com,
    /// <c>GET /wp-json/clubprogol/v1/quiniela/{pais}/actual</c>) el 2026-06-18.
    ///
    /// Prueban que el JSON que de verdad sirve el servidor live parsea de punta a punta con el
    /// cliente de la app: 14 partidos, numeración 1..14 en orden y nombres no vacíos. MX no trae
    /// Pleno al 15; ES sí. Las muestras están versionadas en Fixtures/online/real-*.json.
    /// </summary>
    public class JornadaQuinielaParserRealBackendTests
    {
        private static string LeerFixture(string nombre) =>
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "online", nombre));

        [Fact]
        public void Real_Mx_Live_Parsea_14Partidos_SinPleno15()
        {
            string json = LeerFixture("real-mx.json");

            JornadaQuiniela jornada = JornadaQuinielaParser.Parse(json);

            Assert.Equal("MX", jornada.Pais);
            Assert.Equal(14, jornada.Partidos.Count);

            // Numeración correlativa 1..14 en orden (contrato).
            Assert.Equal(Enumerable.Range(1, 14), jornada.Partidos.Select(p => p.N));

            // Todos los nombres reales no vacíos.
            Assert.All(jornada.Partidos, p =>
            {
                Assert.False(string.IsNullOrWhiteSpace(p.Local));
                Assert.False(string.IsNullOrWhiteSpace(p.Visitante));
            });

            // México no trae Pleno al 15.
            Assert.Null(jornada.Pleno15);
        }

        [Fact]
        public void Real_Es_Live_Parsea_14Partidos_ConPleno15_Y_Acentos()
        {
            string json = LeerFixture("real-es.json");

            JornadaQuiniela jornada = JornadaQuinielaParser.Parse(json);

            Assert.Equal("ES", jornada.Pais);
            Assert.Equal(14, jornada.Partidos.Count);
            Assert.Equal(Enumerable.Range(1, 14), jornada.Partidos.Select(p => p.N));

            Assert.All(jornada.Partidos, p =>
            {
                Assert.False(string.IsNullOrWhiteSpace(p.Local));
                Assert.False(string.IsNullOrWhiteSpace(p.Visitante));
            });

            // España trae Pleno al 15 (n=15), con acentos UTF-8 preservados ("España").
            Assert.NotNull(jornada.Pleno15);
            Assert.Equal(15, jornada.Pleno15.N);
            Assert.Equal("España", jornada.Pleno15.Local);
            Assert.False(string.IsNullOrWhiteSpace(jornada.Pleno15.Visitante));
        }
    }
}
