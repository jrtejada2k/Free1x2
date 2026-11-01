// Free1X2 · WinUI 3 — WIN3
using System;

namespace Free1X2.Analisis
{
	/// <summary>
	/// Filtro Pim del original (<c>Free1X2/UI/FiltroPim.cs</c>): con los valores 1 / X / 2 de los 14 partidos forma 7
	/// rankings de las 6 casillas de más valor que quedan, y una columna pasa si sus aciertos con cada ranking y el
	/// recorrido (máximo − mínimo de los 7) caen en sus rangos. No tiene suma: esa es del filtro Aidomnou, que el port
	/// de WinUI aplicaba aquí por error.
	/// </summary>
	public static class FiltroPim
	{
		public const int Rankings = 7;
		public const int CasillasPorRanking = 6;

		/// <summary>
		/// Los 7 rankings, como el <c>RecuperaPantalla</c> del original (FiltroPim.cs:221-248): [partido, ranking] con
		/// la suma de los signos elegidos en ese partido (1 = «1», 4 = «X», 2 = «2»). Cada casilla elegida sale de las
		/// que quedan con el valor más alto; en un empate, la primera recorriendo P1..P14 y en cada partido 1, X, 2.
		/// </summary>
		public static int[,] CalculaRankings(double[,] valores)
		{
			if (valores.GetLength(0) < 14 || valores.GetLength(1) < 3)
				throw new ArgumentException("Hacen falta los valores 1 / X / 2 de los 14 partidos.", nameof(valores));

			var quedan = new double[14, 3];
			for (int p = 0; p < 14; p++)
				for (int s = 0; s < 3; s++) quedan[p, s] = valores[p, s];

			var rankings = new int[14, Rankings];
			for (int r = 0; r < Rankings; r++)
			{
				for (int n = 0; n < CasillasPorRanking; n++)
				{
					double max = -1; int partido = 0, signo = 0;
					for (int p = 0; p < 14; p++)
						for (int s = 0; s < 3; s++)
							if (quedan[p, s] > max) { max = quedan[p, s]; partido = p; signo = s; }
					quedan[partido, signo] = -1;
					rankings[partido, r] += signo == 0 ? 1 : signo == 1 ? 4 : 2;
				}
			}
			return rankings;
		}

		/// <summary>Aciertos de la columna con cada ranking (Valida y Analizar del original). La «x» vale como «X».</summary>
		public static int[] Aciertos(int[,] rankings, string columna)
		{
			if (columna == null || columna.Length < 14)
				throw new ArgumentException("La columna debe tener 14 signos.", nameof(columna));

			var aciertos = new int[Rankings];
			for (int r = 0; r < Rankings; r++)
				for (int p = 0; p < 14; p++)
					if ((rankings[p, r] & Bit(columna[p])) != 0) aciertos[r]++;
			return aciertos;
		}

		/// <summary>Recorrido de los aciertos: el mayor menos el menor de los 7.</summary>
		public static int Recorrido(int[] aciertos)
		{
			int min = int.MaxValue, max = int.MinValue;
			foreach (int a in aciertos) { if (a < min) min = a; if (a > max) max = a; }
			return max - min;
		}

		/// <summary>
		/// Si la columna pasa: <paramref name="rangos"/> es [8, 2] con el mínimo y el máximo de R1..R7 y, en la fila 7,
		/// los del recorrido (FiltroPim.cs:448-466).
		/// </summary>
		public static bool Admite(int[] aciertos, int[,] rangos)
		{
			for (int r = 0; r < Rankings; r++)
				if (aciertos[r] < rangos[r, 0] || aciertos[r] > rangos[r, 1]) return false;
			int recorrido = Recorrido(aciertos);
			return recorrido >= rangos[Rankings, 0] && recorrido <= rangos[Rankings, 1];
		}

		/// <summary>Signos de un ranking en un partido, como los pinta el original (Cambia): «-», «1», «X2»…</summary>
		public static string Signos(int codigo) => codigo switch
		{
			0 => "-",
			1 => "1",
			2 => "2",
			3 => "12",
			4 => "X",
			5 => "1X",
			6 => "X2",
			_ => "1X2",
		};

		private static int Bit(char signo) => signo switch
		{
			'1' => 1,
			'2' => 2,
			'X' or 'x' => 4,
			_ => 0,
		};
	}
}
