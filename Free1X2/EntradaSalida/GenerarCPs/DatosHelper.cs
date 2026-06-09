using System.IO;

namespace Free1X2.EntradaSalida.GenerarCPs
{
	/// <summary>
	/// Summary description for DatosHelper.
	/// </summary>
	public class DatosHelper
	{
		protected string applicationPath = "";
		protected string xmlFile = "";

		public DatosHelper()
		{
			applicationPath = System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
			xmlFile = applicationPath + "/ColumnasProbables.xml";
		}

		public ColumnasProbables ObtenerDatos()
		{
			ColumnasProbables datosDS = new ColumnasProbables();
			//la primera vez que usamos el programa no existira el archivo.
			if( File.Exists( xmlFile ) )
			{
				datosDS.ReadXml(xmlFile, System.Data.XmlReadMode.Auto);
			}
			return datosDS;
		}

		public void GuardarDatos(ColumnasProbables datosDS)
		{
			//esto sobreescribe el archivo si existe
			datosDS.WriteXml(xmlFile);
		}
	}
}
