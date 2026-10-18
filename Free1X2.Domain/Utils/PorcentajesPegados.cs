// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Free1X2.Utils
{
	/// <summary>
	/// «Pegar desde Excel» en los editores de porcentajes 1 / X / 2 (W15, DT-92; no existe en el WinForms). Mismo
	/// comportamiento que la app Mac (<c>CasosUtilidades.porcentajesPegados</c>).
	/// </summary>
	public static class PorcentajesPegados
	{
		public const string MensajeVacio = "El portapapeles está vacío: copia antes en Excel las filas de porcentajes.";
		public const string MensajeSinFilas = "No hay ninguna fila con tres porcentajes (1, X y 2) en lo copiado.";

		/// <summary>
		/// Filas de porcentajes copiadas de una hoja de cálculo: de cada línea cuentan sus <b>tres últimos números</b>
		/// (1, X y 2), separados por tabuladores, punto y coma o espacios, con coma o punto decimal y con o sin «%». Así se
		/// pueden copiar también las columnas del nº de partido o de los equipos. Las líneas con menos de tres números
		/// (encabezados, vacías) se saltan; se devuelven como mucho <paramref name="filas"/>. Un número negativo, infinito
		/// o NaN, o ninguna fila válida ⇒ <see cref="FormatException"/>.
		/// </summary>
		public static List<double[]> Leer(string texto, int filas)
		{
			var r = new List<double[]>();
			foreach (string linea in (texto ?? "").Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (r.Count >= filas) break;
				var numeros = new List<double>();
				foreach (string t in linea.Split(new[] { '\t', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
				{
					string limpio = t.Trim().Replace("%", "").Replace(",", ".");
					if (double.TryParse(limpio, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
						numeros.Add(v);
				}
				if (numeros.Count < 3) continue;

				var tres = numeros.GetRange(numeros.Count - 3, 3).ToArray();
				foreach (double v in tres)
				{
					if (v < 0 || double.IsNaN(v) || double.IsInfinity(v))
						throw new FormatException("La fila «" + linea + "» tiene un porcentaje no válido.");
				}
				r.Add(tres);
			}
			if (r.Count == 0) throw new FormatException(MensajeSinFilas);
			return r;
		}
	}
}
