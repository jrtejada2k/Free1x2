using System;
using System.IO;
using Free1X2.Reduccion;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de <see cref="ReductorTM"/> ("menos columnas 2").
    /// El algoritmo elige reductoras por el PRIMER índice del máximo
    /// (<c>Array.IndexOf(reduceCols, mayor)</c>), así que el orden de selección
    /// determina el contenido y el orden del fichero de salida. Estos tests fijan la
    /// salida COMPLETA (todas las líneas, en su orden exacto) más los tres contadores
    /// públicos, para poder optimizar el interior del reductor sin cambiar resultados.
    ///
    /// Los valores esperados se capturaron del código original (v0.82.0, antes de
    /// cualquier optimización de la fase F3).
    ///
    /// Nota: se usan niveles de reducción que el algoritmo original sabe terminar. Con
    /// `diferencia = 14 - nivel = 1` sobre columnas que no se emparejan, el bucle
    /// `while (mayor != 0)` del original no termina (todos los contadores valen 1 y
    /// `Array.IndexOf(matrizTemporal, 0)` devuelve -1); ese caso queda fuera del test
    /// porque ya cuelga en el código de referencia.
    /// </summary>
    public class ReductorTMGoldenTests
    {
        // Ejecuta el reductor como lo hace la UI (Inicializa + ComienzaReduccion) y
        // devuelve TODAS las líneas del fichero de salida en su orden exacto.
        private static string[] Reducir(string entrada, int nivel, out int ini, out int fin, out int proc)
        {
            string salida = Path.Combine(Path.GetTempPath(),
                "free1x2_tm_golden_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                var reductor = new ReductorTM();
                reductor.Inicializa(entrada, nivel);
                reductor.ComienzaReduccion(entrada, salida, nivel, 0, 100);

                ini = reductor.NoColumnasIniciales;
                fin = reductor.NoColumnasFinales;
                proc = reductor.NoColumnasProcesadas;
                return File.ReadAllLines(salida);
            }
            finally { if (File.Exists(salida)) File.Delete(salida); }
        }

        // Universo determinista de 3^7 = 2187 columnas: 7 triples + 7 fijos a "1".
        private static string GenerarUniverso2187()
        {
            string ruta = Path.Combine(Path.GetTempPath(),
                "free1x2_tm_golden_in_" + Guid.NewGuid().ToString("N") + ".txt");
            var a = new Free1X2.MotorCalculo.Analizador();
            for (int i = 0; i < 14; i++) a.SetPronostico(i, i < 7 ? "1,X,2" : "1");
            a.AnalizaCombinacion(ruta);
            return ruta;
        }

        [Fact]
        public void ReductorTM_ColumnasReales_Nivel8_SalidaCompletaExacta()
        {
            int ini, fin, proc;
            string[] salida = Reducir(FixturePaths.Ruta(FixturePaths.ColsA), 8, out ini, out fin, out proc);

            Assert.Equal(10, ini);
            Assert.Equal(6, fin);
            Assert.Equal(10, proc);
            Assert.Equal(new[]
            {
                "1X211111X1112X",
                "21122X1X111122",
                "111X222X11XX21",
                "XX12X1X1XX1122",
                "21XX1XX2XX1211",
                "22X1X1221111X1",
            }, salida);
        }

        [Fact]
        public void ReductorTM_ColumnasReales_Nivel10_SalidaCompletaExacta()
        {
            int ini, fin, proc;
            string[] salida = Reducir(FixturePaths.Ruta(FixturePaths.ColsA), 10, out ini, out fin, out proc);

            Assert.Equal(10, ini);
            Assert.Equal(9, fin);
            Assert.Equal(10, proc);
            Assert.Equal(new[]
            {
                "111X222X11XX21",
                "21122X1X111122",
                "1X1X12X1X111X1",
                "12122X1XX211X1",
                "XX12X1X1XX1122",
                "1X211111X1112X",
                "21XX1XX2XX1211",
                "22X1X1221111X1",
                "2X11X11XX1122X",
            }, salida);
        }

        [Fact]
        public void ReductorTM_Universo2187_Nivel12_SalidaCompletaExacta()
        {
            string entrada = GenerarUniverso2187();
            try
            {
                int ini, fin, proc;
                string[] salida = Reducir(entrada, 12, out ini, out fin, out proc);

                Assert.Equal(2187, ini);
                Assert.Equal(56, fin);
                Assert.Equal(2187, proc);
                Assert.Equal(new[]
                {
                    "11111111111111", "XXXXX111111111", "XX222XX1111111", "X2X21221111111",
                    "2X21X221111111", "22X12X11111111", "222X11X1111111", "121XXX21111111",
                    "11XX22X1111111", "21122121111111", "XX111XX1111111", "X122XX11111111",
                    "1X122211111111", "1XX2X1X1111111", "X211X2X1111111", "211X1211111111",
                    "X1212121111111", "1X2X1X21111111", "21X2XXX1111111", "12212111111111",
                    "2X1X21X1111111", "11X11221111111", "2X221211111111", "X2XX2X21111111",
                    "122XX2X1111111", "X212X121111111", "2XX11X21111111", "12121XX1111111",
                    "X12X2211111111", "11X1XX11111111", "X1XX11X1111111", "22X2X111111111",
                    "2XX122X1111111", "1X222121111111", "212XXX21111111", "11211XX1111111",
                    "XX1X2X11111111", "X2X11111111111", "22222221111111", "XX1X1221111111",
                    "11XXX121111111", "2X11X111111111", "1112X2X1111111", "11112X21111111",
                    "X1X22111111111", "12XX1X11111111", "XXX2XX21111111", "X221XX21111111",
                    "XX21X1X1111111", "22111221111111", "XX121111111111", "X1X12XX1111111",
                    "X12212X1111111", "X11XXXX1111111", "X1121X21111111", "22XXX221111111",
                }, salida);
            }
            finally { if (File.Exists(entrada)) File.Delete(entrada); }
        }

        [Fact]
        public void ReductorTM_Universo2187_Nivel11_SalidaCompletaExacta()
        {
            string entrada = GenerarUniverso2187();
            try
            {
                int ini, fin, proc;
                string[] salida = Reducir(entrada, 11, out ini, out fin, out proc);

                Assert.Equal(2187, ini);
                Assert.Equal(19, fin);
                Assert.Equal(2187, proc);
                Assert.Equal(new[]
                {
                    "11111111111111", "XXXXXXX1111111", "22222221111111", "XXX22111111111",
                    "22211XX1111111", "111XX221111111", "XX2X1221111111", "22X1X111111111",
                    "X1122XX1111111", "12XX2X21111111", "1X22X2X1111111", "2X1X2X11111111",
                    "21X212X1111111", "X121X121111111", "X2121211111111", "X22X21X1111111",
                    "2X12X121111111", "2122XX11111111", "121122X1111111",
                }, salida);
            }
            finally { if (File.Exists(entrada)) File.Delete(entrada); }
        }
    }
}
