using System;
using System.IO;
using System.Linq;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Dos fallos que venían del original y el dueño mandó corregir (pruebas I5 y O4 de
    /// <c>docs/PRUEBAS_WINDOWS.md</c>).
    /// </summary>
    public class HeredadosArregladosTests
    {
        [Fact]
        public void Filtro_LineaEnBlanco_NoEsUnaColumna()
        {
            string d = MascarasTests.CarpetaTemporal();
            try
            {
                string filtro = Path.Combine(d, "filtro_con_blanco.txt");
                File.WriteAllText(filtro, "1X21X21X21X21X\n\n2222222222222X\n   \nXXXXXXXXXXXXX1\n");
                string salida = Path.Combine(d, "salida.txt");

                var a = new Analizador();
                for (int i = 0; i < 14; i++) a.SetPronostico(i, "1,X,2");
                a.ArchivoColumnasBase = filtro;
                a.AnalizaCombinacion(salida);

                Assert.Equal(3, a.ColsAnalizadas);
                Assert.Equal(3, a.ColsAceptadas);
                string[] cols = File.ReadAllLines(salida);
                Assert.Equal(3, cols.Length);
                Assert.DoesNotContain(cols, c => c.Replace(",", "").StartsWith("11111111111111"));
            }
            finally { Directory.Delete(d, true); }
        }

        [Fact]
        public void Tramos_DiezEnCuatro_NoPierdeColumnas()
        {
            Assert.Equal(new[] { (0, 2), (2, 4), (4, 6), (6, 10) }, FraccionTramos.Limites(10, 4));
        }

        [Fact]
        public void Tramos_ComoElOriginalCuandoNoPerdia()
        {
            // 9 en 4: el resto (1) es menor que un tramo y el original ya se lo daba al último.
            Assert.Equal(new[] { (0, 2), (2, 4), (4, 6), (6, 9) }, FraccionTramos.Limites(9, 4));
            Assert.Equal(new[] { (0, 3), (3, 7) }, FraccionTramos.Limites(7, 2));
            Assert.Empty(FraccionTramos.Limites(10, 0));
        }

        [Fact]
        public void Tramos_SiempreCubrenTodasLasLineas()
        {
            for (int numcols = 0; numcols <= 60; numcols++)
                for (int qnts = 1; qnts <= 20; qnts++)
                {
                    var l = FraccionTramos.Limites(numcols, qnts);
                    Assert.Equal(qnts, l.Length);
                    Assert.Equal(0, l[0].inicio);
                    Assert.Equal(numcols, l[^1].fin);
                    for (int i = 1; i < qnts; i++) Assert.Equal(l[i - 1].fin, l[i].inicio);
                    Assert.Equal(numcols, l.Sum(t => t.fin - t.inicio));
                }
        }
    }
}
