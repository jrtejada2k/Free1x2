using System;
using System.IO;
using Free1X2.MotorCalculo;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W10 (DT-85): «Columnas de Inicio» en <c>Temp/ColumnasInicio.txt</c>. Mismo caso que la app
    /// Mac (<c>f52ColumnasDeInicio</c> en <c>FlujosTests.swift</c>).
    /// </summary>
    public class ColumnasInicioTests
    {
        private static Analizador Jd01()
        {
            var a = new Analizador();
            for (int i = 0; i < 14; i++)
                a.SetPronostico(i, i < 6 ? "1,X,2" : "1");
            return a;
        }

        [Fact]
        public void Jd01_Da729Columnas()
        {
            string d = MascarasTests.CarpetaTemporal();
            try
            {
                int n = ColumnasInicio.Generar(Jd01(), d, out string ruta);
                Assert.Equal(729, n);
                Assert.Equal(Path.Combine(d, "ColumnasInicio.txt"), ruta);
                Assert.Equal(729, File.ReadAllLines(ruta).Length);
                Assert.Equal(729, ColumnasInicio.ColumnasSinCondiciones(Jd01().Pronosticos));
            }
            finally { Directory.Delete(d, true); }
        }

        [Fact]
        public void SinColumnas_ErrorYSinFicheroViejo()
        {
            string d = MascarasTests.CarpetaTemporal();
            try
            {
                ColumnasInicio.Generar(Jd01(), d, out string ruta);
                Assert.True(File.Exists(ruta));

                // JD-01 tiene de 0 a 6 variantes: exigir 14 no deja ninguna columna.
                var a = Jd01();
                var f = (FiltroNoVariantes)a.GruposPartidos[0].GetFiltro(Filtro.NoVariantes.ToString());
                f.SetNoVariantes("14");
                f.SetNoEquis("0,1,2,3,4,5,6,7,8,9,10,11,12,13,14");
                f.SetNoDoses("0,1,2,3,4,5,6,7,8,9,10,11,12,13,14");
                f.IsActive = true;

                var ex = Assert.Throws<InvalidOperationException>(() => ColumnasInicio.Generar(a, d, out _));
                Assert.Equal(ColumnasInicio.MensajeSinColumnas, ex.Message);
                Assert.False(File.Exists(ruta));
            }
            finally { Directory.Delete(d, true); }
        }
    }
}
