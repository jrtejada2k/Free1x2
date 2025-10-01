using System;
using System.Xml;

namespace Free1X2
{
    public class GestorPublicidad
    {
        public bool PublicidadActivadaEnMainForm()
        {
            // Desactivada publicidad bajo Mono hasta corregir bug de cambio demasiado rapido de imagen
            if (VariablesGlobales.FuncionaBajoMono){ return false; }

            return EstaAnuncioActivo();
        }

        public bool PublicidadActivadaEnImprimirBoletos()
        {
            return EstaAnuncioActivo();
        }

        private bool EstaAnuncioActivo()
        {
            //desactivado por ahora para evitar conectar con la web cada vez que se ejecute el Free1x2
            //try
            //{
            //    DateTime fechaFinAnuncio = ObtenFechaFinPublicidad();
            //    if (DateTime.Today > fechaFinAnuncio)
            //    {
            //        return false;
            //    }
            //}
            //catch (Exception){ }

            return true;
        }

        private static DateTime ObtenFechaFinPublicidad()
        {
            const string xmlLocation = "http://www.free1x2.com/Free1x2/DatosFree1X2.xml";
            var datos = new XmlDocument();
            datos.Load(xmlLocation);

            var nodes = datos.GetElementsByTagName("FechaFinPublicidad");
            return DateTime.Parse(nodes[0].InnerText);
        }
    }
}
