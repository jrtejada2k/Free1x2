// Free1X2 · WinUI 3 — WIN3
namespace Free1X2.Utils
{
	/// <summary>
	/// «Ordenar por probabilidad ▸ Obtener intervalo: premio de 14 entre … y …» (W18, DT-95; era de la 0.77.2). Misma
	/// cuenta que «Añadir Premio de 14» del original (OrdenarPorProbabilidadFrm.GrabacionColumnas), en <c>float</c>.
	/// </summary>
	public static class IntervaloPremio14
	{
		/// <summary>premio = (precio × %14 ÷ 100) ÷ probabilidad, con tope recaudación × %14 ÷ 100.</summary>
		public static float Premio(double precio, double porcentaje14, double recaudacion, float probabilidad)
		{
			float pr = (float)(precio * porcentaje14 / 100);
			float premioMax = (float)(recaudacion * porcentaje14 / 100);
			float premio = pr / probabilidad;
			if (premio > premioMax) premio = premioMax;
			return premio;
		}

		/// <summary>Si la columna se graba: su premio está entre el mínimo y el máximo (ambos incluidos).</summary>
		public static bool Admite(float premio, double minimo, double maximo) => premio >= minimo && premio <= maximo;
	}
}
