using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Free1X2.Mascaras;
using Free1X2.Online;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W12 (DT-88): base de ganadoras por liga con importaciones que se suman, y el histórico de
    /// clubprogol. Mismos casos que la app Mac (<c>historicoReal</c>, <c>analizarArchivoDeGanadoras</c>,
    /// <c>baseDeGanadorasPorLiga</c>, <c>descargaSeSumaALaLiga</c> en <c>MascarasTests.swift</c>).
    /// </summary>
    public class HistoricoTests
    {
        private static readonly string Unos = MascarasTests.Unos;
        private static readonly string Doses = MascarasTests.Doses;

        private static List<ResultadoHistorico> Real(string pais) =>
            HistoricoQuinielaParser.Parse(File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "Fixtures", "online", "historico-" + pais + "-2026-10-16.json")));

        [Fact]
        public void HistoricoReal()
        {
            var es = Real("es");
            var mx = Real("mx");
            Assert.Equal(19, es.Count);
            Assert.Equal(20261014, es[0].Jornada);
            Assert.Equal("11122X12112X2X", es[0].Ganadora);
            Assert.Equal(24, mx.Count);
            Assert.Equal(2331, mx[^1].Jornada);
            Assert.Equal("", mx[^1].Fecha);

            // JD-01 contra España: lo mismo que desarrollar sus 729 columnas por fuerza bruta.
            var r = CasosMascaras.Escrutar(MascarasTests.Jd01, es.Select(h => h.Ganadora).ToList());
            Assert.Equal(new Dictionary<int, long> { [11] = 3, [10] = 43 }, r.Premios);
            Assert.Equal(10, r.JornadasConPremio);
            Assert.Equal(19, r.Jornadas);
        }

        [Fact]
        public void HistoricoDelServicio_ValidacionEstricta()
        {
            var h = HistoricoQuinielaParser.Parse(
                "{\"pais\":\"ES\",\"resultados\":[{\"jornada\":20261011,\"fecha\":\"2026-10-11\",\"ganadora\":\"1x21X1121X1X21\"}," +
                "{\"jornada\":20261004,\"fecha\":\"2026-10-04\",\"ganadora\":\"11111111111111\"}]}");
            Assert.Equal(new[] { "1X21X1121X1X21", Unos }, h.Select(x => x.Ganadora));
            Assert.Equal(20261011, h[0].Jornada);
            Assert.Throws<FormatException>(() =>
                HistoricoQuinielaParser.Parse("{\"resultados\":[{\"jornada\":1,\"ganadora\":\"1X2\"}]}"));
            Assert.Throws<FormatException>(() => HistoricoQuinielaParser.Parse("{\"pais\":\"ES\"}"));
        }

        [Fact]
        public void AnalizarArchivoDeGanadoras()
        {
            var g = CasosHistorico.Analizar("2355;1x21x1121x1x211\r\n\r\n22222222222222 2-1\r\n 2356 ; 11111111111111\r\n");
            Assert.Equal(new[] { "2355", null, "2356" }, g.Select(x => x.Jornada));
            Assert.Equal(new[] { "1X21X1121X1X21", Doses, Unos }, g.Select(x => x.Ganadora));
            Assert.Empty(CasosHistorico.Analizar("\r\n"));

            foreach (string malo in new[] { "11111111111111\n1,X,2,1\n", ";11111111111111\n", "1;111X\n" })
                Assert.Throws<FormatException>(() => CasosHistorico.Analizar(malo));

            var roto = Assert.Throws<FormatException>(() => CasosHistorico.Analizar("11111111111111\n\n111X\n"));
            Assert.StartsWith("La línea 3 no es «jornada;columna»", roto.Message);
        }

        [Fact]
        public void BaseDeGanadorasPorLiga()
        {
            string d = MascarasTests.CarpetaTemporal();
            try
            {
                string liga = CasosHistorico.Crear("  Liga MX Femenil ", d);
                Assert.Equal("Liga MX Femenil", liga);
                Assert.Equal(new[] { 0 }, CasosHistorico.Ligas(d).Select(l => l.Jornadas));

                var r = CasosHistorico.Importar("1;" + Unos + "\r\n2;" + Doses + "\r\n" + Unos + "\r\n", liga, d);
                Assert.Equal((2, 0, 0, 1, 3), (r.Anadidas, r.YaEstaban, r.Actualizadas, r.SinJornada, r.Total));

                r = CasosHistorico.Importar("1;" + Unos + "\r\n2;" + Unos + "\r\n3;" + Doses + "\r\n3;" + Doses + "\r\n", liga, d);
                Assert.Equal((1, 2, 1, 0, 4), (r.Anadidas, r.YaEstaban, r.Actualizadas, r.SinJornada, r.Total));
                Assert.Equal("1 añadidas, 2 ya estaban, 1 actualizadas (otra columna para la misma jornada). " +
                             "La liga tiene ahora 4 jornadas.", r.Mensaje);

                var base_ = CasosHistorico.Leer(liga, d);
                Assert.Equal(new[] { "1", "2", null, "3" }, base_.Select(x => x.Jornada));
                Assert.Equal(new[] { Unos, Unos, Unos, Doses }, base_.Select(x => x.Ganadora));

                var m = CasosMascaras.Escrutar(MascarasTests.Jd01, base_.Select(x => x.Ganadora).ToList());
                Assert.Equal(new Dictionary<int, long> { [14] = 3, [13] = 36, [12] = 180, [11] = 480, [10] = 720 }, m.Premios);
                Assert.Equal(3, m.JornadasConPremio);
                Assert.Equal(4, m.Jornadas);

                Assert.Throws<FormatException>(() => CasosHistorico.Importar("\r\n", liga, d));
                Assert.Throws<ArgumentException>(() => CasosHistorico.Crear("a/b", d));

                CasosHistorico.Borrar(liga, d);
                Assert.Empty(CasosHistorico.Ligas(d));
            }
            finally { Directory.Delete(d, true); }
        }

        [Fact]
        public void DescargaSeSumaALaLiga()
        {
            string d = MascarasTests.CarpetaTemporal();
            try
            {
                var h = Real("es");
                Assert.Equal(19, CasosHistorico.SumarDescarga(h, "es", d).Anadidas);
                var r = CasosHistorico.SumarDescarga(h, "es", d);
                Assert.Equal((0, 19, 19), (r.Anadidas, r.YaEstaban, r.Total));
                Assert.Equal(new[] { "España" }, CasosHistorico.Ligas(d).Select(l => l.Nombre));
                Assert.StartsWith("20261014;11122X12112X2X\r\n",
                    File.ReadAllText(Path.Combine(d, "Historico", "España.txt")));
            }
            finally { Directory.Delete(d, true); }
        }

        [Fact]
        public void LigasEnOrdenNatural()
        {
            string d = MascarasTests.CarpetaTemporal();
            try
            {
                foreach (string n in new[] { "Liga 10", "liga 2", "España", "México" })
                    CasosHistorico.Crear(n, d);
                Assert.Equal(new[] { "España", "liga 2", "Liga 10", "México" }, CasosHistorico.Ligas(d).Select(l => l.Nombre));
            }
            finally { Directory.Delete(d, true); }
        }
    }
}
