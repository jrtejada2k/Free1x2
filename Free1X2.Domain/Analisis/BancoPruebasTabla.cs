// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Linq;

namespace Free1X2.Analisis
{
	/// <summary>
	/// Una fila del resultado por columnas del Banco de pruebas (modos «Columnas» y «Autoescrutinio»), con los valores
	/// del WinForms <c>ResultadosPorColumna</c> (Free1X2/UI/BancoPruebasFrm.cs:336) que el port WinUI había perdido y que
	/// usan la ordenación y el «Filtro de selección de filas» (W19, DT-95). Mismo modelo que la app Mac
	/// (<c>FilaBancoColumna</c>).
	/// </summary>
	public sealed class FilaBancoColumna
	{
		public FilaBancoColumna(int indice, string columna, int[] veces14a10, double[] premios14a10, bool[] acumula14a10,
			int numEscrutinios, double precio)
		{
			Indice = indice;
			Columna = columna;
			Veces = (int[])veces14a10.Clone();
			Premios = (double[])premios14a10.Clone();
			double suma = 0;
			int veces = 0;
			for (int k = 0; k < 5; k++)
			{
				if (!acumula14a10[k]) continue;
				suma += Premios[k];
				veces += Veces[k];
			}
			VecesAcumulado = veces;
			PremioAcumulado = Math.Round(suma, 1);
			PorcentajeRecuperacion = numEscrutinios > 0 ? Math.Round(suma * 100 / numEscrutinios / precio, 0) : 0;
		}

		/// <summary>Posición original (0-based): lo que muestra «Nºcol.» menos uno.</summary>
		public int Indice { get; }
		public string Columna { get; }

		/// <summary>Veces con 14…10 aciertos.</summary>
		public int[] Veces { get; }

		/// <summary>Importes de 14…10 sin redondear.</summary>
		public double[] Premios { get; }

		/// <summary>«Nºacumulado»: veces con las categorías marcadas en «Premios acumulados que se consideran».</summary>
		public int VecesAcumulado { get; }

		/// <summary>«Premio Acumulado»: importe de las categorías marcadas, redondeado a 1 decimal.</summary>
		public double PremioAcumulado { get; }

		/// <summary>% de recuperación: acumulado × 100 ÷ escrutinios ÷ precio, redondeado a entero.</summary>
		public double PorcentajeRecuperacion { get; }

		/// <summary>«Premio 14»…«Premio 10» de la tabla: <c>Math.Round(importe)</c>.</summary>
		public double PremioRedondeado(int k) => Math.Round(Premios[k]);

		/// <summary>«14 medio»…«10 medio»: <c>Math.Round(importe ÷ veces)</c>; 0 sin veces.</summary>
		public double PremioMedio(int k) => Veces[k] > 0 ? Math.Round(Premios[k] / Veces[k]) : 0;

		/// <summary>
		/// Valor de la cabecera <paramref name="c"/> de la tabla para ordenar (WinForms <c>ComparaCon</c>): 0 «Nºcol.»,
		/// 1…5 «Nº14»…«Nº10», 6 «Nºacumulado», 7…11 «Premio 14»…«Premio 10» (sin redondear), 12 «Premio Acumulado»,
		/// 13…17 «14 medio»…«10 medio», 18 «Recuperación» (el premio acumulado).
		/// </summary>
		public double ValorCabecera(int c)
		{
			if (c >= 1 && c <= 5) return Veces[c - 1];
			if (c == 6) return VecesAcumulado;
			if (c >= 7 && c <= 11) return Premios[c - 7];
			if (c >= 13 && c <= 17) return PremioMedio(c - 13);
			if (c == 12 || c == 18) return PremioAcumulado;
			return Indice;
		}

		/// <summary>Valor del concepto <paramref name="c"/> del filtro (el orden de <see cref="BancoPruebasTabla.Conceptos"/>).</summary>
		public double ValorConcepto(int c)
		{
			if (c >= 0 && c <= 4) return Veces[c];
			if (c == 5) return VecesAcumulado;
			if (c >= 6 && c <= 10) return Premios[c - 6];
			if (c == 11) return PremioAcumulado;
			if (c >= 12 && c <= 16) return PremioMedio(c - 12);
			return PorcentajeRecuperacion;
		}
	}

	/// <summary>Ordenación, límites y «Filtro de selección de filas» de la tabla del Banco de pruebas (W19).</summary>
	public static class BancoPruebasTabla
	{
		/// <summary>Cabeceras de la tabla en el orden de <see cref="FilaBancoColumna.ValorCabecera"/>.</summary>
		public static readonly string[] Cabeceras =
		{
			"Nºcol.", "Nº14", "Nº13", "Nº12", "Nº11", "Nº10", "Nºacumulado",
			"Premio 14", "Premio 13", "Premio 12", "Premio 11", "Premio 10", "Premio Acumulado",
			"14 medio", "13 medio", "12 medio", "11 medio", "10 medio", "Recuperación",
		};

		/// <summary>Conceptos del «Filtro de selección de filas» (WinForms <c>DialogoSeleccionBancoPruebasFrm</c>).</summary>
		public static readonly string[] Conceptos =
			new[] { 14, 13, 12, 11, 10 }.Select(k => "Nº de veces " + k + " aciertos")
			.Concat(new[] { "Nº de veces con premio" })
			.Concat(new[] { 14, 13, 12, 11, 10 }.Select(k => "Premio total de " + k))
			.Concat(new[] { "Premio acumulado" })
			.Concat(new[] { 14, 13, 12, 11, 10 }.Select(k => "Premio medio de " + k))
			.Concat(new[] { "% de recuperación" })
			.ToArray();

		/// <summary>Primera pulsación de una cabecera: «Nºcol.» de menor a mayor, el resto de mayor a menor.</summary>
		public static bool DescendentePorDefecto(int cabecera) => cabecera != 0;

		/// <summary>Filas ordenadas por la cabecera (orden estable).</summary>
		public static List<T> Ordenar<T>(IEnumerable<T> filas, Func<T, FilaBancoColumna> fila, int cabecera, bool descendente) =>
			descendente
				? filas.OrderByDescending(f => fila(f).ValorCabecera(cabecera)).ToList()
				: filas.OrderBy(f => fila(f).ValorCabecera(cabecera)).ToList();

		/// <summary>
		/// Mínimo y máximo de cada concepto entre las filas, valores iniciales del filtro (WinForms <c>MaxMin</c>). Como en
		/// el original, los premios totales salen redondeados a euros aunque el filtro compare el importe sin redondear.
		/// </summary>
		public static (double Minimo, double Maximo)[] Limites(IReadOnlyList<FilaBancoColumna> filas) =>
			Enumerable.Range(0, Conceptos.Length).Select(c =>
			{
				var v = filas.Select(f => c >= 6 && c <= 10 ? Math.Round(f.ValorConcepto(c)) : f.ValorConcepto(c)).ToList();
				return v.Count == 0 ? (0d, 0d) : (v.Min(), v.Max());
			}).ToArray();

		/// <summary>
		/// WinForms <c>btFiltrar_Click</c>: en cada fila, el primer concepto marcado que queda fuera de su mínimo y máximo
		/// la desmarca y corta; si los cumple todos, queda marcada. Sin conceptos marcados no cambia nada. Devuelve la
		/// selección resultante, una por fila.
		/// </summary>
		public static bool[] Seleccionar(IReadOnlyList<FilaBancoColumna> filas, bool[] seleccionadas,
			IReadOnlyList<(int Concepto, double Minimo, double Maximo)> criterios)
		{
			var r = (bool[])seleccionadas.Clone();
			for (int i = 0; i < filas.Count; i++)
			{
				foreach (var c in criterios)
				{
					double v = filas[i].ValorConcepto(c.Concepto);
					r[i] = !(v < c.Minimo || v > c.Maximo);
					if (!r[i]) break;
				}
			}
			return r;
		}
	}
}
