// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.IO;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Analisis
{
	/// <summary>
	/// Paneles «% COMBINACIÓN» y «Valoraciones JORNADA» de la Rotación de signos (W3, C2, DT-79),
	/// recuperados del WinForms (<c>Free1X2/UI/RotacionDeSignosFrm.cs:1860-1931,2190-2292</c>).
	/// Lógica pura, sin UI, para poder probarla. Mismo comportamiento que la app Mac
	/// (<c>CasosUtilidades.porcentajesRotacion</c>, <c>propuestaRotacion</c>).
	/// </summary>
	public static class RotacionSignos
	{
		private static readonly int[] Pot =
		{
			1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907,
		};

		/// <summary>
		/// «% COMBINACIÓN» de un fichero de columnas (legacy <c>LeerColumnas</c> + <c>ContarSignos</c>).
		/// </summary>
		public static double[,] Porcentajes(string rutaEntrada, int partidos)
		{
			var columnas = new List<string>();
			IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(rutaEntrada);
			while (comBaseCols.SiguienteColumna())
			{
				columnas.Add(comBaseCols.LeeColumnaSinComas());
			}
			comBaseCols.Cerrar();
			return Porcentajes(columnas, partidos);
		}

		/// <summary>
		/// Porcentaje de 1/X/2 de cada partido en las columnas (legacy <c>ConvertirAPorcentaje</c>):
		/// X y 2 con <c>Math.Round</c> (a par) y el 1 completa 100. Sin columnas, ceros (el legacy
		/// daba NaN), como la app Mac.
		/// </summary>
		public static double[,] Porcentajes(IEnumerable<string> columnas, int partidos)
		{
			var porcentajes = new double[partidos, 3];
			var col = new ConvertidorDeBases();
			foreach (string columna in columnas)
			{
				int num = col.ConvColumnaANumero(columna);
				for (int partido = 0; partido < partidos; partido++)
				{
					porcentajes[partido, (num / Pot[partido]) % 3]++;
				}
			}

			for (int i = 0; i < partidos; i++)
			{
				double suma = porcentajes[i, 0] + porcentajes[i, 1] + porcentajes[i, 2];
				if (suma == 0) continue;
				porcentajes[i, 2] = Math.Round(porcentajes[i, 2] * 100 / suma, 0);
				porcentajes[i, 1] = Math.Round(porcentajes[i, 1] * 100 / suma, 0);
				porcentajes[i, 0] = 100 - porcentajes[i, 2] - porcentajes[i, 1];
			}
			return porcentajes;
		}

		/// <summary>
		/// Legacy <c>Ordenar3valores</c>: índices 0/1/2 de mayor a menor valor, con los desempates
		/// de ±0,001 del original (si aún coinciden, gana el último índice comprobado).
		/// </summary>
		public static int[] Ordenar3valores(double v1, double vX, double v2)
		{
			double[] x = new double[3];
			double[] originalValues = new double[3];
			int[] resultado = new int[3];

			if (v1 == vX) v1 += 0.001;
			if (v2 == vX) v2 -= 0.001;
			if (v1 == v2) v1 += 0.001;

			x[0] = -v1;
			x[1] = -vX;
			x[2] = -v2;

			originalValues[0] = v1;
			originalValues[1] = vX;
			originalValues[2] = v2;

			Array.Sort(x);

			for (int i = 0; i < 3; i++)
			{
				if (originalValues[0] == -x[i]) resultado[i] = 0;
				if (originalValues[1] == -x[i]) resultado[i] = 1;
				if (originalValues[2] == -x[i]) resultado[i] = 2;
			}

			return resultado;
		}

		/// <summary>
		/// Legacy <c>PropuestaAutomatica</c>: en cada partido, el k-ésimo signo más frecuente de la
		/// combinación pasa al k-ésimo más valorado de la jornada. Devuelve
		/// <c>nuevos[partido, signoOriginal] = signoDestino</c> (0/1/2).
		/// </summary>
		public static int[,] PropuestaAutomatica(double[,] combinacion, double[,] jornada)
		{
			int partidos = Math.Min(combinacion.GetLength(0), jornada.GetLength(0));
			var nuevos = new int[partidos, 3];
			for (int i = 0; i < partidos; i++)
			{
				int[] x = Ordenar3valores(combinacion[i, 0], combinacion[i, 1], combinacion[i, 2]);
				int[] y = Ordenar3valores(jornada[i, 0], jornada[i, 1], jornada[i, 2]);
				for (int s = 0; s < 3; s++)
				{
					nuevos[i, x[s]] = y[s];
				}
			}
			return nuevos;
		}

		/// <summary>
		/// Orden 0-based de la «Transposición automática» (legacy <c>button3_Click</c> con
		/// <see cref="TransponedorAutomatico"/>). <c>null</c> si la valoración o la combinación no
		/// tienen 14 partidos (el transponedor trabaja con 14 fijos).
		/// </summary>
		public static int[] TrasposicionAutomatica(double[,] jornada, double[,] combinacion)
		{
			if (jornada.GetLength(0) < 14 || combinacion.GetLength(0) < 14) return null;
			return new TransponedorAutomatico(jornada, combinacion).Trasposicion();
		}

		/// <summary>
		/// Graba en <paramref name="rutaSalida"/> cada columna de la entrada con sus 14 signos
		/// reordenados: <c>salida[i] = entrada[orden[i]]</c> (legacy <c>TransposicionFrm.Transponer</c>).
		/// </summary>
		public static void TransponerColumnas(string rutaEntrada, string rutaSalida, int[] orden)
		{
			using var sr = new StreamReader(rutaEntrada);
			using var sw = new StreamWriter(rutaSalida);
			while (sr.Peek() > 0)
			{
				string columna = sr.ReadLine() ?? string.Empty;
				var aux = new char[14];
				for (int nr = 0; nr < 14; nr++) aux[nr] = columna[orden[nr]];
				sw.WriteLine(new string(aux));
			}
		}
	}
}
