// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// «Columnas de Inicio» (W10, DT-85; no existe en el WinForms): el boleto de Inicio con sus
	/// condiciones (y el filtro de columnas, si el Analizador lo trae en ArchivoColumnasBase),
	/// desarrollado como «Calcular ▸ Grabar» en <c>Temp/ColumnasInicio.txt</c>, que se reescribe
	/// cada vez. El Reductor y los filtros lo usan de entrada sin que el usuario guarde antes un
	/// fichero. Mismo comportamiento que la app Mac (<c>CasosCombinacion.columnasDeInicio</c>).
	/// </summary>
	public static class ColumnasInicio
	{
		public const string Fichero = "ColumnasInicio.txt";

		/// <summary>Por encima de este número de columnas (antes de las condiciones) se pide confirmación.</summary>
		public const long LimiteSinAviso = 1_000_000;

		public const string MensajeSinColumnas = "La combinación de Inicio no da ninguna columna con sus condiciones.";

		/// <summary>Columnas que desarrolla el boleto antes de aplicar las condiciones.</summary>
		public static long ColumnasSinCondiciones(IEnumerable<string> pronosticos) =>
			pronosticos.Aggregate(1L, (c, p) => c * Math.Max(1, NumSignos(p)));

		private static int NumSignos(string p)
		{
			string s = string.IsNullOrEmpty(p) ? "1,X,2" : p;
			return (s.Contains('1') ? 1 : 0) + (s.Contains('X') ? 1 : 0) + (s.Contains('2') ? 1 : 0);
		}

		/// <summary>
		/// Graba las columnas aceptadas en <c>&lt;carpetaTemp&gt;/ColumnasInicio.txt</c> y devuelve
		/// cuántas son. Si las condiciones no dejan ninguna, lanza <see cref="InvalidOperationException"/>
		/// y no queda fichero (tampoco el de la vez anterior).
		/// </summary>
		public static int Generar(Analizador analizador, string carpetaTemp, out string ruta)
		{
			Directory.CreateDirectory(carpetaTemp);
			ruta = Path.Combine(carpetaTemp, Fichero);
			if (File.Exists(ruta)) File.Delete(ruta);

			analizador.AnalizaCombinacion(ruta);

			if (analizador.ColsAceptadas == 0)
			{
				if (File.Exists(ruta)) File.Delete(ruta);
				throw new InvalidOperationException(MensajeSinColumnas);
			}
			return analizador.ColsAceptadas;
		}
	}
}
