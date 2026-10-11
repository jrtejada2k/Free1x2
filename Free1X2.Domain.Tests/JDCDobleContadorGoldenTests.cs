using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Free1X2.Reduccion;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de <see cref="JDCDobleContador"/> ("menos columnas"). Fija la salida
    /// COMPLETA (todas las líneas, en su orden exacto) y los tres contadores públicos, para
    /// poder optimizar el interior del reductor sin cambiar resultados (R1).
    ///
    /// Los valores se capturaron del código tal cual estaba antes de la optimización
    /// (pump fuera de la recursión + recuento incremental de "columnas pobres"). El reductor
    /// se invoca como la UI WinUI (ReductorFrmViewModel): ComienzaReduccion directo, sin
    /// Inicializa previo (su Inicializa es no-op). Universo determinista: 'triples' triples +
    /// resto fijo "1" = 3^triples columnas.
    /// </summary>
    public class JDCDobleContadorGoldenTests
    {
        private static string GenerarUniverso(int triples)
        {
            string ruta = Path.Combine(Path.GetTempPath(),
                "free1x2_jdc_in_" + Guid.NewGuid().ToString("N") + ".txt");
            var a = new Free1X2.MotorCalculo.Analizador();
            for (int i = 0; i < 14; i++) a.SetPronostico(i, i < triples ? "1,X,2" : "1");
            a.AnalizaCombinacion(ruta);
            return ruta;
        }

        private static string[] Reducir(int triples, int nivel, bool externas,
            out int ini, out int fin, out int proc)
        {
            string entrada = GenerarUniverso(triples);
            string salida = Path.Combine(Path.GetTempPath(),
                "free1x2_jdc_out_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                var r = new JDCDobleContador(externas);
                r.ComienzaReduccion(entrada, salida, nivel, 0, 100);
                ini = r.NoColumnasIniciales;
                fin = r.NoColumnasFinales;
                proc = r.NoColumnasProcesadas;
                return File.ReadAllLines(salida);
            }
            finally
            {
                if (File.Exists(entrada)) File.Delete(entrada);
                if (File.Exists(salida)) File.Delete(salida);
            }
        }

        private static string Sha(string[] lineas)
        {
            using var sha = SHA256.Create();
            byte[] h = sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", lineas)));
            var sb = new StringBuilder();
            foreach (var b in h) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        [Fact]
        public void JDCDoble_7Triples_Nivel12_SalidaCompletaExacta()
        {
            string[] salida = Reducir(7, 12, false, out int ini, out int fin, out int proc);
            Assert.Equal(2187, ini);
            Assert.Equal(69, fin);
            Assert.Equal(2187, proc);
            Assert.Equal(new[]
            {
                "11111111111111", "X1111111111111", "11211111111111", "X1211111111111", "21211111111111", "211X1111111111",
                "1XXX1111111111", "2111X111111111", "211XX111111111", "22XXX111111111", "XX2XX111111111", "X112X111111111",
                "X1X2X111111111", "X1212111111111", "X1XX2111111111", "21XX2111111111", "X12X2111111111", "122X2111111111",
                "21X22111111111", "1221XX11111111", "12X22X11111111", "22222X11111111", "1XX2X211111111", "1X1X2211111111",
                "12X2X1X1111111", "2X111XX1111111", "XXX11XX1111111", "1X211XX1111111", "XX1X1XX1111111", "XXXX1XX1111111",
                "XX121XX1111111", "2X121XX1111111", "2XX21XX1111111", "2X221XX1111111", "2XX1XXX1111111", "2X21XXX1111111",
                "XX1XXXX1111111", "X12XXXX1111111", "2X112XX1111111", "2XX12XX1111111", "2X212XX1111111", "XX1X2XX1111111",
                "XX122XX1111111", "XXX22XX1111111", "XX222XX1111111", "2112X2X1111111", "11X2X2X1111111", "111122X1111111",
                "112X22X1111111", "1X22X121111111", "1X112121111111", "11X2XX21111111", "111X2X21111111", "22X11221111111",
                "22211221111111", "121X1221111111", "22XX1221111111", "X22X1221111111", "222X1221111111", "X2X21221111111",
                "X2221221111111", "X211X221111111", "X2X1X221111111", "X221X221111111", "X2112221111111", "X2X12221111111",
                "221X2221111111", "222X2221111111", "22122221111111",
            }, salida);
        }

        [Fact]
        public void JDCDoble_7Triples_Nivel11_SalidaCompletaExacta()
        {
            string[] salida = Reducir(7, 11, false, out int ini, out int fin, out int proc);
            Assert.Equal(2187, ini);
            Assert.Equal(17, fin);
            Assert.Equal(2187, proc);
            Assert.Equal(new[]
            {
                "11111111111111", "X1111111111111", "212X1111111111", "21X21111111111", "11XXX111111111", "1X12XX11111111",
                "1X122X11111111", "X11X21X1111111", "XXXX1XX1111111", "2X22XXX1111111", "2XX12XX1111111", "XX212XX1111111",
                "221X1221111111", "1211X221111111", "X2X2X221111111", "X222X221111111", "12222221111111",
            }, salida);
        }

        [Fact]
        public void JDCDoble_7Triples_Nivel13_SalidaCompletaExacta()
        {
            string[] salida = Reducir(7, 13, false, out int ini, out int fin, out int proc);
            Assert.Equal(2187, ini);
            Assert.Equal(256, fin);
            Assert.Equal(2187, proc);
            Assert.Equal(256, salida.Length);
            // Salida grande (nivel 13 = diferencia 1): se fija por SHA-256 del contenido en orden.
            Assert.Equal("c51a75cd9ffb8d8e0c197b28b3840bda5eb035654f81ebbf43fb64b5c199ba82", Sha(salida));
        }

        [Fact]
        public void JDCDoble_8Triples_Nivel12_ContadoresYHash()
        {
            string[] salida = Reducir(8, 12, false, out int ini, out int fin, out int proc);
            Assert.Equal(6561, ini);
            Assert.Equal(133, fin);
            Assert.Equal(6561, proc);
            Assert.Equal(133, salida.Length);
            Assert.Equal("3412170198d2d97921b90de5f501d29257a6fe4921ebad367898acdb9a775e92", Sha(salida));
        }

        [Fact]
        public void JDCDoble_7Triples_Nivel12_ConExternas_SalidaCompletaExacta()
        {
            string[] salida = Reducir(7, 12, true, out int ini, out int fin, out int proc);
            Assert.Equal(2187, ini);
            Assert.Equal(64, fin);
            Assert.Equal(2187, proc);
            Assert.Equal(new[]
            {
                "11111111111111", "X1111111111111", "11211111111111", "X1211111111111", "21211111111111", "211X1111111111",
                "21X21111111111", "2111X111111111", "21X1X111111111", "21XXX111111111", "212XX111111111", "21X2X111111111",
                "XX22X111111111", "X1XX2111111111", "11122111111111", "21X22111111111", "XXX21X11111111", "X222XX11111111",
                "1XXX2X11111111", "122X1211111111", "1211X211111111", "X21X2211111111", "12X1X1X1111111", "X22XX1X1111111",
                "XXX121X1111111", "X21221X1111111", "2X111XX1111111", "1XX11XX1111111", "2X2X1XX1111111", "2X121XX1111111",
                "X1X21XX1111111", "2X221XX1111111", "2X21XXX1111111", "1X1XXXX1111111", "XXXXXXX1111111", "2X22XXX1111111",
                "2X112XX1111111", "2XX12XX1111111", "2X212XX1111111", "2X2X2XX1111111", "12222XX1111111", "X11XX2X1111111",
                "112222X1111111", "X21X1121111111", "11XX1121111111", "1X212121111111", "XX111X21111111", "X2X1XX21111111",
                "1112XX21111111", "X1222X21111111", "22X11221111111", "X2211221111111", "22XX1221111111", "12121221111111",
                "221XX221111111", "222XX221111111", "2212X221111111", "12X2X221111111", "11112221111111", "XXX12221111111",
                "221X2221111111", "22XX2221111111", "22122221111111", "22222221111111",
            }, salida);
        }
    }
}
