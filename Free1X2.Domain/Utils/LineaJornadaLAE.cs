// Free1X2 · WinUI 3 — WIN3
using System.Globalization;
using System.Text;

namespace Free1X2.Utils
{
	/// <summary>
	/// Línea de <c>Jornadas/InfoJornadasLAE.txt</c> que escribe «Tramificar ▸ Guardar jornada L.A.E.» (legacy
	/// <c>TramificarForm.MontaLinea</c>, Free1X2/UI/TramificarForm.cs:2982): columna premiada, temporada, jornada,
	/// recaudación y premios de 14 a 10, separados por tabuladores, con los importes ÷ precio y coma decimal.
	/// W16 (DT-93): el original dividía por 0,50 fijo y usaba 14.000.000 si la recaudación venía vacía; ahora el precio
	/// y la recaudación por defecto son los de Configuración, así el fichero queda en las mismas unidades que escribe
	/// Escrutinio y que lee Estimación de premios (× su precio).
	/// </summary>
	public static class LineaJornadaLAE
	{
		public static string Componer(string columnaPremiada, string temporada, string jornada, string recaudacion,
			string[] premios14a10, double precio, double recaudacionPorDefecto, CultureInfo ci)
		{
			const char sep = (char)9;
			double rec = string.IsNullOrEmpty(recaudacion) ? recaudacionPorDefecto : ADouble(recaudacion, ci);

			var linea = new StringBuilder();
			linea.Append(columnaPremiada).Append(sep);
			linea.Append(temporada).Append(sep);
			linea.Append(jornada).Append(sep);
			linea.Append(Importe(rec / precio, ci));
			for (int i = 0; i < 5; i++)
			{
				string p = premios14a10 != null && i < premios14a10.Length ? premios14a10[i] : "";
				linea.Append(sep).Append(Importe(ADouble(p, ci) / precio, ci));
			}
			return linea.ToString();
		}

		private static string Importe(double v, CultureInfo ci) => v.ToString(ci).Replace(".", ",");

		/// <summary>Importe de texto (coma o punto decimal) a double sin reventar por la cultura; vacío o ilegible ⇒ 0.</summary>
		public static double ADouble(string valor, CultureInfo ci)
		{
			if (string.IsNullOrWhiteSpace(valor)) return 0;
			if (double.TryParse(valor, NumberStyles.Any, ci, out double r)) return r;
			if (double.TryParse(valor.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out r)) return r;
			return 0;
		}
	}
}
