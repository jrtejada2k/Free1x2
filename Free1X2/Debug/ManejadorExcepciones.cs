using System;
using System.IO;
using System.Windows.Forms;

namespace Free1X2.Debug
{
    public class ManejadorExcepciones
    {
        protected string path = Application.StartupPath + "/Informes/";

        public void GuardarInformeErrorATxt(Exception e, string nombreInforme)
        {
            string nombreArchivo = path + nombreInforme;
            string claseDesencadenante = e.TargetSite.DeclaringType.Name;
            StreamWriter sw = new StreamWriter(nombreArchivo);
            sw.WriteLine("Error producido el " + DateTime.Now);
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("Objeto: " + claseDesencadenante);
            sw.WriteLine("");
            sw.WriteLine("\n");
            sw.WriteLine("SO: " + VariablesGlobales.SistemaOperativo);
            sw.WriteLine("");
            sw.WriteLine("\n");
            sw.WriteLine("Versión de Free1x2: " + Application.ProductVersion);
            sw.WriteLine("");
            sw.WriteLine("\n");
            sw.WriteLine("Número de Partidos en la Configuración: " + VariablesGlobales.NumeroPartidos);
            sw.WriteLine("");
            sw.WriteLine("\n");
            sw.WriteLine("Mensaje de Error:");
            sw.WriteLine("");
            sw.Write(e.Message);
            sw.WriteLine("");
            sw.WriteLine("\n");
            sw.WriteLine("Información de Trazado:");
            sw.WriteLine("");
            sw.Write(e.StackTrace);
            
            sw.Close();
        }
    }
}
