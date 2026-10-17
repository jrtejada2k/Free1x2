using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Free1X2.Mascaras;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W9 (DT-84): máscaras guardadas y su escrutinio. Mismos casos que la app Mac
    /// (<c>MascarasTests.swift</c>). JD-01 = partidos 1–6 «1,X,2» y 7–14 «1» (729 columnas).
    /// </summary>
    public class MascarasTests
    {
        internal static readonly Mascara Jd01 = new Mascara("JD-01",
            Enumerable.Repeat("1,X,2", 6).Concat(Enumerable.Repeat("1", 8)).ToList());

        internal static readonly string Unos = new string('1', 14);
        internal static readonly string Doses = new string('2', 14);

        internal static string CarpetaTemporal()
        {
            string d = Path.Combine(Path.GetTempPath(), "free1x2_mas_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(d);
            return d;
        }

        [Fact]
        public void AciertosSinDesarrollar()
        {
            long[] a = CasosMascaras.Aciertos(Jd01.Pronosticos, Unos);
            Assert.Equal(new long[] { 64, 192, 240, 160, 60, 12, 1 }, a.Skip(8).Take(7));
            Assert.Equal(729, a.Sum());
            Assert.Equal(729, Jd01.Columnas);
            Assert.Equal((8, 0, 6), (Jd01.Fijos, Jd01.Dobles, Jd01.Triples));
        }

        [Fact]
        public void EscrutarContraUnosYDoses()
        {
            var r = CasosMascaras.Escrutar(Jd01, new[] { Unos, Doses });
            Assert.Equal(new Dictionary<int, long> { [14] = 1, [13] = 12, [12] = 60, [11] = 160, [10] = 240 }, r.Premios);
            Assert.Equal(1, r.JornadasConPremio);
            Assert.Equal(2, r.Jornadas);
            Assert.Equal(729, r.Columnas);
        }

        [Fact]
        public void GuardarListarYBorrar()
        {
            string d = CarpetaTemporal();
            try
            {
                CasosMascaras.Guardar(new Mascara("  Mi 4D-2T ", Jd01.Pronosticos), d);
                Assert.True(CasosMascaras.Existe("Mi 4D-2T", d));

                var lista = CasosMascaras.Listar(14, d);
                Assert.Single(lista);
                Assert.Equal("Mi 4D-2T", lista[0].Nombre);
                Assert.Equal(Jd01.Pronosticos, lista[0].Pronosticos);
                Assert.Empty(CasosMascaras.Listar(15, d));

                // Formato compartido con el Mac: UTF-8 sin BOM, CRLF también tras la última línea.
                byte[] bytes = File.ReadAllBytes(Path.Combine(d, "Mascaras", "Mi 4D-2T.mas"));
                Assert.NotEqual(0xEF, bytes[0]);
                Assert.EndsWith("1\r\n1\r\n", System.Text.Encoding.UTF8.GetString(bytes));

                Assert.Throws<ArgumentException>(() => CasosMascaras.Guardar(new Mascara("a/b", new string[0]), d));
                Assert.Null(CasosMascaras.NombreValido("   "));

                CasosMascaras.Borrar("Mi 4D-2T", d);
                Assert.Empty(CasosMascaras.Listar(14, d));
            }
            finally { Directory.Delete(d, true); }
        }

        [Fact]
        public void NoSeListanLasMalFormadas()
        {
            string d = CarpetaTemporal();
            try
            {
                string carpeta = Path.Combine(d, "Mascaras");
                Directory.CreateDirectory(carpeta);
                File.WriteAllText(Path.Combine(carpeta, "trece.mas"), string.Join("\r\n", Enumerable.Repeat("1", 13)) + "\r\n");
                File.WriteAllText(Path.Combine(carpeta, "repetido.mas"), "1,1\r\n" + string.Join("\r\n", Enumerable.Repeat("1", 13)) + "\r\n");
                File.WriteAllText(Path.Combine(carpeta, "conblancos.mas"), "\r\n" + string.Join("\r\n\r\n", Enumerable.Repeat("X", 14)) + "\r\n");
                var lista = CasosMascaras.Listar(14, d);
                Assert.Equal(new[] { "conblancos" }, lista.Select(m => m.Nombre));
            }
            finally { Directory.Delete(d, true); }
        }
    }
}
