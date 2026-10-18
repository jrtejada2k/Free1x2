// Free1X2 · WinUI 3 — WIN3
namespace Free1X2.Escrutinio
{
	/// <summary>
	/// W17 (DT-95; lo hacía la 0.77.2): la columna ganadora de Escrutinios repartida en las 16 casillas de «Posibles
	/// premios». Mismo comportamiento que la app Mac (<c>CasosCombinacion.signosGanadoraImportada</c>).
	/// </summary>
	public static class GanadoraImportada
	{
		/// <summary>1, X o 2 donde los haya (una «x» minúscula pasa a X) y «*» en lo demás; vacía ⇒ 16 «*».</summary>
		public static string[] SignosGanadoraImportada(string ganadora)
		{
			string g = (ganadora ?? "").ToUpperInvariant();
			var r = new string[16];
			for (int i = 0; i < 16; i++)
				r[i] = i < g.Length && "1X2".IndexOf(g[i]) >= 0 ? g[i].ToString() : "*";
			return r;
		}
	}
}
