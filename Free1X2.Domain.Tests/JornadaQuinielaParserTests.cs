using System;
using System.IO;
using System.Linq;
using Free1X2.Online;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Tests del parser PURO de la jornada online (<see cref="JornadaQuinielaParser"/>) contra
    /// las MUESTRAS REALES versionadas del contrato (docs/ejemplos-api/), copiadas a
    /// Fixtures/online/ junto al binario de pruebas (CopyToOutputDirectory).
    /// Verifican que la app puede parsear de punta a punta antes de tener el backend real.
    /// </summary>
    public class JornadaQuinielaParserTests
    {
        private static string LeerFixture(string nombre) =>
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "online", nombre));

        [Fact]
        public void Es_Muestra_Tiene14Partidos_NombresReales_Y_Pleno15()
        {
            string json = LeerFixture("quiniela-es-actual.json");

            JornadaQuiniela jornada = JornadaQuinielaParser.Parse(json);

            Assert.Equal("ES", jornada.Pais);
            Assert.Equal("2025/26", jornada.Temporada);
            Assert.Equal(38, jornada.Jornada);
            Assert.Equal(14, jornada.Partidos.Count);

            // Numeración correlativa 1..14 en orden.
            Assert.Equal(Enumerable.Range(1, 14), jornada.Partidos.Select(p => p.N));

            // Nombres reales tal cual el contrato.
            Assert.Equal("Real Madrid", jornada.Partidos[0].Local);
            Assert.Equal("FC Barcelona", jornada.Partidos[0].Visitante);

            // Acentos preservados (UTF-8) en nombres como "Alavés"/"Leganés", "Almería".
            Assert.Equal("Alavés", jornada.Partidos[8].Local);
            Assert.Equal("Almería", jornada.Partidos[13].Local);

            // Pleno al 15 presente (solo España).
            Assert.NotNull(jornada.Pleno15);
            Assert.Equal(15, jornada.Pleno15.N);
            Assert.Equal("Cádiz", jornada.Pleno15.Local);
            Assert.Equal("Tenerife", jornada.Pleno15.Visitante);
        }

        [Fact]
        public void Mx_Muestra_Tiene14Partidos_ConAcentos_Y_SinPleno15()
        {
            string json = LeerFixture("quiniela-mx-actual.json");

            JornadaQuiniela jornada = JornadaQuinielaParser.Parse(json);

            Assert.Equal("MX", jornada.Pais);
            Assert.Equal(24, jornada.Jornada);
            Assert.Equal(14, jornada.Partidos.Count);

            // "América" con acento (UTF-8 correcto).
            Assert.Equal("América", jornada.Partidos[0].Local);
            Assert.Equal("Guadalajara", jornada.Partidos[0].Visitante);

            // México no trae Pleno al 15.
            Assert.Null(jornada.Pleno15);
        }

        [Fact]
        public void JsonMalformado_LanzaFormatException()
        {
            string basura = "{ esto no es json válido ][ ";

            FormatException ex = Assert.Throws<FormatException>(
                () => JornadaQuinielaParser.Parse(basura));
            Assert.Contains("JSON", ex.Message);
        }

        [Fact]
        public void JsonVacio_LanzaFormatException()
        {
            Assert.Throws<FormatException>(() => JornadaQuinielaParser.Parse(""));
        }

        [Fact]
        public void MenosDe14Partidos_LanzaFormatException()
        {
            // JSON sintácticamente válido pero con solo 2 partidos: viola el contrato (14).
            string json =
                "{ \"pais\":\"ES\", \"temporada\":\"2025/26\", \"jornada\":1, \"partidos\":[" +
                "{ \"n\":1, \"local\":\"A\", \"visitante\":\"B\" }," +
                "{ \"n\":2, \"local\":\"C\", \"visitante\":\"D\" } ] }";

            FormatException ex = Assert.Throws<FormatException>(
                () => JornadaQuinielaParser.Parse(json));
            Assert.Contains("14", ex.Message);
        }

        [Fact]
        public void EquipoVacio_LanzaFormatException()
        {
            // 14 partidos pero el local del primero está vacío: dato inválido.
            var sb = new System.Text.StringBuilder();
            sb.Append("{ \"pais\":\"ES\", \"temporada\":\"2025/26\", \"jornada\":1, \"partidos\":[");
            for (int i = 1; i <= 14; i++)
            {
                if (i > 1) sb.Append(',');
                string local = i == 1 ? "" : "Local" + i;
                sb.Append("{ \"n\":" + i + ", \"local\":\"" + local + "\", \"visitante\":\"Visitante" + i + "\" }");
            }
            sb.Append("] }");

            Assert.Throws<FormatException>(() => JornadaQuinielaParser.Parse(sb.ToString()));
        }
    }
}
