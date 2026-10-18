using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Free1X2.Mascaras;
using Free1X2.Online;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W20 (DT-96), W21 (DT-97) y W22 (DT-98): datos de fábrica. Mismos casos que la app Mac
    /// (<c>InicioTests.jornadasLAEAlPrecio</c> y <c>arrancarSiembraHistoricos</c>).
    /// </summary>
    public class SemillasHistoricasTests
    {
        private const string L1 = "111111XX211221\t1988/1989\t01\t13894332,67\t1042074,97\t18946,83\t1053,67\t150,52\t21,50";
        private const string L2 = "21X121XXX22X2X\t1995/1996\t23\t47426967,00\t0,00\t278982,15\t14503,65\t1097,85\t156,84";

        [Fact]
        public void JornadasLAEAlPrecio_A075()
        {
            string r = SemillasHistoricas.JornadasLAEAlPrecio(L1 + "\r\n" + L2 + "\r\n", 0.75);
            string[] l = r.Split("\r\n");
            Assert.Equal("111111XX211221\t1988/1989\t01\t9262888,45\t694716,65\t12631,22\t702,45\t100,35\t14,33", l[0]);
            Assert.Equal("21X121XXX22X2X\t1995/1996\t23\t31617978\t0,00\t185988,1\t9669,1\t731,9\t104,56", l[1]);
            Assert.Equal("", l[2]);

            // Leída con Estimación a 0,75 € (× precio): 1995/96-23 da 23.713.483,5 € de recaudación (0,50 × 47.426.967).
            double rec = double.Parse(l[1].Split('\t')[3], new CultureInfo("es-ES")) * 0.75;
            Assert.Equal(23713483.5, rec, 6);
        }

        [Fact]
        public void JornadasLAEAlPrecio_A050_SinCambios()
        {
            string t = L1 + "\r\n" + L2 + "\r\n";
            Assert.Same(t, SemillasHistoricas.JornadasLAEAlPrecio(t, 0.5));
        }

        [Fact]
        public void GanadorasDeResultados()
        {
            var g = SemillasHistoricas.GanadorasDeResultados("1970\t1\tX11121212X11X1\r\n2006\t38\t1X21X21X21X21X1\r\n1980\t5\t1X2-1X21X21X21\r\n");
            Assert.Equal(new[] { "1970/1971-01", "2006/2007-38" }, g.Select(x => x.Jornada));
            Assert.Equal(new[] { "X11121212X11X1", "1X21X21X21X21X" }, g.Select(x => x.Ganadora));
        }

        [Fact]
        public void GanadorasDeProgol()
        {
            string csv = "NPRODUCTO,CONCURSO,R1,R2,R3,R4,R5,R6,R7,R8,R9,R10,R11,R12,R13,R14,BOLSA,FECHA\r\n" +
                         "10,2354,L,L,E,E,E,E,E,L,V,L,E,L,L,L,8000000,12/10/2026\r\n" +
                         "10,767,E,L,V,V,L,V,L,V,E,E,E,L,E,L,0,15/08/1993\r\n" +
                         "10,1000,L,L,E\r\n";
            var g = SemillasHistoricas.GanadorasDeProgol(csv);
            Assert.Equal(new[] { "767;X1221212XXX1X1", "2354;11XXXXX121X111" }, g.Select(x => x.Jornada + ";" + x.Ganadora));
        }

        private static string Carpeta()
        {
            string d = Path.Combine(Path.GetTempPath(), "free1x2_sem_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(d);
            return d;
        }

        [Fact]
        public void Sembrar_LigasHistoricosYNoPisa()
        {
            string semillas = Carpeta(), datos = Carpeta();
            try
            {
                Directory.CreateDirectory(Path.Combine(semillas, "Jornadas"));
                Directory.CreateDirectory(Path.Combine(semillas, "Ganadoras"));
                File.WriteAllText(Path.Combine(semillas, "Jornadas", "Resultados.txt"), "1970\t1\tX11121212X11X1\r\n2006\t38\t1X21X21X21X21X1\r\n");
                File.WriteAllText(Path.Combine(semillas, "Jornadas", "EspanaDesde2007.txt"), "2019/2020-01;1X21X21X21X21X\r\n20261011;X21X11XX111121\r\n");
                File.WriteAllText(Path.Combine(semillas, "Jornadas", "InfoJornadasLAE.txt"), L2 + "\r\n");
                File.WriteAllText(Path.Combine(semillas, "Ganadoras", "1983-84.txt"), "X11121212X11X1\r\n");

                var errores = SemillasHistoricas.Sembrar(semillas, datos, 0.75, sembrarLigas: true);
                Assert.Empty(errores);

                // W20 + W22: «España» con Resultados.txt y detrás la semilla web, en ese orden.
                var espana = CasosHistorico.Leer("España", datos);
                Assert.Equal(new[] { "1970/1971-01", "2006/2007-38", "2019/2020-01", "20261011" }, espana.Select(x => x.Jornada));
                // La descarga de clubprogol no repite la que ya trae.
                var r = CasosHistorico.SumarDescarga(new[] { new ResultadoHistorico { Jornada = 20261011, Ganadora = "X21X11XX111121" } }, "es", datos);
                Assert.Equal((1, 4), (r.YaEstaban, r.Total));

                // Históricos copiados y InfoJornadasLAE al precio.
                Assert.True(File.Exists(Path.Combine(datos, "Jornadas", "Resultados.txt")));
                Assert.True(File.Exists(Path.Combine(datos, "Ganadoras", "1983-84.txt")));
                Assert.StartsWith("21X121XXX22X2X\t1995/1996\t23\t31617978\t",
                    File.ReadAllText(Path.Combine(datos, "Jornadas", "InfoJornadasLAE.txt")));

                // Un InfoJornadasLAE.txt ya existente no se pisa.
                File.WriteAllText(Path.Combine(datos, "Jornadas", "InfoJornadasLAE.txt"), "mio");
                SemillasHistoricas.Sembrar(semillas, datos, 0.75, sembrarLigas: false);
                Assert.Equal("mio", File.ReadAllText(Path.Combine(datos, "Jornadas", "InfoJornadasLAE.txt")));

                // Tras borrar la liga y volver a arrancar (Historico/ ya existe), sigue sin ella.
                CasosHistorico.Borrar("España", datos);
                SemillasHistoricas.Sembrar(semillas, datos, 0.75, sembrarLigas: false);
                Assert.Empty(CasosHistorico.Leer("España", datos));
            }
            finally
            {
                Directory.Delete(semillas, true);
                Directory.Delete(datos, true);
            }
        }

        [Fact]
        public void SemillasReales_TotalesDelPlan()
        {
            // Los ficheros reales de Free1X2.WinUI/Datos/Semillas: 1.454 + 1.205 = 2.659 de España y 1.579 de México.
            string raiz = AppContext.BaseDirectory;
            while (raiz != null && !Directory.Exists(Path.Combine(raiz, "Free1X2.WinUI"))) raiz = Path.GetDirectoryName(raiz);
            Assert.NotNull(raiz);
            string sem = Path.Combine(raiz, "Free1X2.WinUI", "Datos", "Semillas", "Jornadas");

            var resultados = SemillasHistoricas.GanadorasDeResultados(File.ReadAllText(Path.Combine(sem, "Resultados.txt")));
            var web = CasosHistorico.Analizar(File.ReadAllText(Path.Combine(sem, "EspanaDesde2007.txt")));
            var progol = SemillasHistoricas.GanadorasDeProgol(File.ReadAllText(Path.Combine(sem, "Progol.csv")));
            Assert.Equal(1454, resultados.Count);
            Assert.Equal(2659, resultados.Count + web.Count);
            Assert.Equal(1579, progol.Count);
            Assert.Equal("767;X1221212XXX1X1", progol[0].Jornada + ";" + progol[0].Ganadora);
            Assert.Equal("2354;11XXXXX121X111", progol[^1].Jornada + ";" + progol[^1].Ganadora);
        }
    }
}
