using System;
using System.IO;
using System.Linq;
using Free1X2.Online;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Verifica el "round-trip" de la caché de jornadas en disco (Free1X2.WinUI/Services/JornadaCache):
    /// el JSON CRUDO guardado por el servicio se relee desde un fichero y se reparsea con el MISMO
    /// parser defensivo de dominio (<see cref="JornadaQuinielaParser"/>), tal y como hace
    /// <c>JornadaCache.TryCargar</c>. JornadaCache vive en el proyecto WinUI (no referenciable desde
    /// este proyecto net8.0), pero su comportamiento de E/S es trivial y su CORAZÓN — escribir bytes
    /// crudos y releerlos pasándolos por el parser — se cubre aquí end-to-end con la misma muestra
    /// del contrato que la app cachearía (docs/ejemplos-api → Fixtures/online).
    ///
    /// También cubre el invariante de robustez clave: una caché corrupta NO debe parsear (TryCargar
    /// devolvería false y la app caería a modo manual sin romperse).
    /// </summary>
    public class JornadaCacheRoundTripTests
    {
        private static string LeerFixture(string nombre) =>
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "online", nombre));

        [Fact]
        public void EscribirYReleerDesdeDisco_ParseaJornadaEs_Identica()
        {
            // (a) Simula JornadaCache.Guardar: el JSON crudo de la muestra ES se escribe a un
            //     fichero "jornada-es.json" en una carpeta temporal aislada por usuario.
            string raw = LeerFixture("quiniela-es-actual.json");
            string carpeta = Path.Combine(Path.GetTempPath(), "Free1X2_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, "jornada-es.json");
            try
            {
                File.WriteAllText(ruta, raw);

                // (b) Simula JornadaCache.TryCargar: releer del disco + reparsear con el parser real.
                Assert.True(File.Exists(ruta));
                string releido = File.ReadAllText(ruta);
                JornadaQuiniela jornada = JornadaQuinielaParser.Parse(releido);

                // La jornada cacheada conserva los equipos reales (14 partidos + Pleno 15 en ES).
                Assert.Equal("ES", jornada.Pais);
                Assert.Equal(38, jornada.Jornada);
                Assert.Equal(14, jornada.Partidos.Count);
                Assert.Equal(Enumerable.Range(1, 14), jornada.Partidos.Select(p => p.N));
                Assert.Equal("Real Madrid", jornada.Partidos[0].Local);
                Assert.Equal("FC Barcelona", jornada.Partidos[0].Visitante);
                Assert.NotNull(jornada.Pleno15);
                Assert.Equal(15, jornada.Pleno15.N);

                // La marca de tiempo del "guardado" = última escritura del fichero (UTC), como en
                // JornadaCache. Debe ser reciente (round-trip recién hecho).
                DateTime guardadoUtc = File.GetLastWriteTimeUtc(ruta);
                Assert.True((DateTime.UtcNow - guardadoUtc).TotalMinutes < 5);
            }
            finally
            {
                try { Directory.Delete(carpeta, recursive: true); } catch { }
            }
        }

        [Fact]
        public void CacheCorrupta_NoParsea_TratadaComoSinCache()
        {
            // Una caché corrupta en disco no debe parsear: TryCargar devolvería false (se traga la
            // FormatException) y la app caería a modo manual sin crashear.
            string carpeta = Path.Combine(Path.GetTempPath(), "Free1X2_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, "jornada-es.json");
            try
            {
                File.WriteAllText(ruta, "{ esto no es json válido ][ ");

                string releido = File.ReadAllText(ruta);
                // El parser lanza FormatException; JornadaCache.TryCargar la traga y devuelve false.
                Assert.Throws<FormatException>(() => JornadaQuinielaParser.Parse(releido));
            }
            finally
            {
                try { Directory.Delete(carpeta, recursive: true); } catch { }
            }
        }
    }
}
