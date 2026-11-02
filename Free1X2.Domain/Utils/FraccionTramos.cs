// Free1X2 · WinUI 3 — WIN3
using System;

namespace Free1X2.Utils
{
	/// <summary>
	/// Fraccionador por tramos: reparte las líneas en <c>qnts</c> ficheros de <c>numcols / qnts</c> y el último recibe
	/// además el resto. El original (<c>FracTrams</c> de <c>Free1X2/UI/FraccionadorFrm.cs</c>) solo se lo daba si era
	/// menor que un tramo: 10 líneas en 4 tramos grababa 2+2+2+2 y perdía 2. En los demás casos da lo mismo.
	/// </summary>
	public static class FraccionTramos
	{
		/// <summary>Inicio (incluido) y fin (excluido) de cada tramo; ninguno si <paramref name="qnts"/> ≤ 0.</summary>
		public static (int inicio, int fin)[] Limites(int numcols, int qnts)
		{
			if (qnts <= 0) return Array.Empty<(int, int)>();
			int part = numcols / qnts;
			var limites = new (int inicio, int fin)[qnts];
			for (int i = 0; i < qnts; i++)
				limites[i] = (i * part, i == qnts - 1 ? numcols : (i + 1) * part);
			return limites;
		}
	}
}
