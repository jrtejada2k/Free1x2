using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Free1X2.Online;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W6 (DT-81): los equipos del boleto de Inicio salen de la jornada real; el 15.º es el pleno.
    /// W7 (DT-83): al arrancar, la jornada solo si llega a tiempo. Mismos casos que la app Mac
    /// (<c>equiposDeJornadaReal</c> y <c>jornadaAlArrancar</c> en <c>EquiposTests.swift</c>).
    /// </summary>
    public class JornadaTests
    {
        private static JornadaQuiniela Jornada(string pais) =>
            JornadaQuinielaParser.Parse(File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "Fixtures", "online", "real-" + pais + ".json")));

        [Fact]
        public void EquiposDeJornadaReal_Espana()
        {
            var es = Jornada("es");
            Assert.Equal(14, JornadaInicio.Componer(es, 14).Length);

            var es15 = JornadaInicio.Componer(es, 15);
            Assert.Equal(15, es15.Length);
            Assert.Equal("Estados Unidos - Australia", es15[0]);
            Assert.Equal("Finalista 2 Playoff - Finalista 1 Playoff", es15[13]);
            Assert.Equal("España - Arabia Saudi", es15[14]);
        }

        [Fact]
        public void EquiposDeJornadaReal_MexicoSinPleno()
        {
            var mx = JornadaInicio.Componer(Jornada("mx"), 15);
            Assert.Equal(14, mx.Length);
            Assert.Equal("Chequia - Mexico", mx[0]);
            Assert.Equal("Argelia - Austria", mx[13]);

            // El 15.º queda como estaba: México no trae pleno.
            var previos = Enumerable.Repeat("a - b", 15).ToArray();
            Assert.Equal("a - b", JornadaInicio.Aplicar(mx, previos)[14]);
        }

        [Fact]
        public void EstanVacios()
        {
            Assert.True(JornadaInicio.EstanVacios(new[] { "? - ?", "", " ? - ? " }));
            Assert.False(JornadaInicio.EstanVacios(new[] { "? - ?", "Betis - Sevilla" }));
        }

        [Fact]
        public async Task AlArrancar_ConRed_DevuelveLaJornada()
        {
            var es = Jornada("es");
            var r = await JornadaInicio.AlArrancarAsync(_ => Task.FromResult(es), TimeSpan.FromSeconds(3));
            Assert.Same(es, r);
            Assert.Equal("Estados Unidos", r.Partidos[0].Local);
        }

        [Fact]
        public async Task AlArrancar_SinRespuesta_NullEnPlazo()
        {
            var reloj = Stopwatch.StartNew();
            var r = await JornadaInicio.AlArrancarAsync(
                async ct => { await Task.Delay(Timeout.Infinite, ct); return null; }, TimeSpan.FromSeconds(3));
            reloj.Stop();
            Assert.Null(r);
            Assert.True(reloj.Elapsed < TimeSpan.FromSeconds(4), "tardó " + reloj.Elapsed);
        }

        [Fact]
        public async Task AlArrancar_Error_Null()
        {
            var r = await JornadaInicio.AlArrancarAsync(
                _ => Task.FromException<JornadaQuiniela>(new IOException("sin red")), TimeSpan.FromSeconds(3));
            Assert.Null(r);
        }
    }
}
