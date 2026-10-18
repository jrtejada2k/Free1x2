using System;
using System.IO;
using System.Linq;
using System.Xml;
using Free1X2.EntradaSalida;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// W21 (DT-97): parámetros del Progol en <c>parametros.free1x2</c>. Mismos casos que la app Mac
    /// (<c>ParametrosTests.laeMexico</c>).
    /// </summary>
    public class ParametrosProgolTests
    {
        private const string SinProgol =
            "<?xml version=\"1.0\"?>\r\n<xml>\r\n  <configuracionONLAE>\r\n    <LAE>\r\n" +
            "      <OrdenarApuestas PrecioApuesta=\"0,75\" PorcentajeDestinadoAl14=\"12\" Recaudacion=\"14000000\" Moneda=\"€\" />\r\n" +
            "    </LAE>\r\n  </configuracionONLAE>\r\n</xml>\r\n";

        private static string Carpeta(string contenido)
        {
            string d = Path.Combine(Path.GetTempPath(), "free1x2_par_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(d);
            File.WriteAllText(Path.Combine(d, "parametros.free1x2"), contenido);
            return d;
        }

        [Fact]
        public void SinNodo_LosDeFabrica()
        {
            string d = Carpeta(SinProgol);
            try
            {
                new AConfiguracion(d).ObtenValoresProgol(out double precio, out double pct, out double rec, out string moneda);
                Assert.Equal((15d, 33d, 15000000d, "$"), (precio, pct, rec, moneda));
            }
            finally { Directory.Delete(d, true); }
        }

        [Fact]
        public void Guardar_LoAnadeTrasLAE_YEspanaNoCambia()
        {
            string d = Carpeta(SinProgol);
            try
            {
                new AConfiguracion(d).GuardarValoresProgol(20, 30.5, 18000000, "$");

                var doc = new XmlDocument();
                doc.Load(Path.Combine(d, "parametros.free1x2"));
                XmlNode padre = doc.GetElementsByTagName("configuracionONLAE")[0];
                Assert.Equal(new[] { "LAE", "Progol" }, padre.ChildNodes.Cast<XmlNode>().Select(n => n.Name));
                var progol = (XmlElement)doc.GetElementsByTagName("Progol")[0].ChildNodes[0];
                Assert.Equal("20,00", progol.GetAttribute("PrecioApuesta"));
                Assert.Equal("30,5", progol.GetAttribute("PorcentajeDestinadoAl14"));
                Assert.Equal("18000000", progol.GetAttribute("Recaudacion"));
                Assert.Equal("$", progol.GetAttribute("Moneda"));

                double precio = 0, pct = 0, rec = 0; string moneda = "";
                new AConfiguracion(d).ObtenValoresLAE(ref precio, ref pct, ref rec, ref moneda);
                Assert.Equal((0.75, 12d, 14000000d, "€"), (Math.Round(precio, 2), pct, rec, moneda));

                // Guardarlo otra vez cambia el mismo nodo: sigue habiendo un solo <Progol>.
                new AConfiguracion(d).GuardarValoresProgol(21, 30.5, 18000000, "MX$");
                doc.Load(Path.Combine(d, "parametros.free1x2"));
                Assert.Equal(1, doc.GetElementsByTagName("Progol").Count);
                new AConfiguracion(d).ObtenValoresProgol(out double p2, out _, out _, out string m2);
                Assert.Equal((21d, "MX$"), (p2, m2));
            }
            finally { Directory.Delete(d, true); }
        }
    }
}
