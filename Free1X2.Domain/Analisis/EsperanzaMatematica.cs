// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Linq;

namespace Free1X2.Analisis
{
	/// <summary>Parámetros de la jornada y del cobro de premios (W14, DT-91).</summary>
	public sealed class ParametrosEsperanza
	{
		/// <summary>Reparto del Excel del dueño: % de la recaudación para 14, 13, 12, 11 y 10 aciertos.</summary>
		public static readonly double[] RepartoExcel = { 16, 7.5, 7.5, 7.5, 9 };

		/// <summary>Reparto que trae «Estimación de premios».</summary>
		public static readonly double[] RepartoFree1X2 = { 12, 8, 8, 8, 9 };

		public ParametrosEsperanza(double recaudacion, double precio, double[] reparto = null)
		{
			Recaudacion = recaudacion;
			Precio = precio;
			Reparto = (double[])(reparto ?? RepartoExcel).Clone();
		}

		public double Recaudacion { get; set; }

		/// <summary>Precio de una apuesta: nº de apuestas = recaudación ÷ precio.</summary>
		public double Precio { get; set; }

		/// <summary>% de la recaudación para 14, 13, 12, 11 y 10 aciertos.</summary>
		public double[] Reparto { get; set; }

		/// <summary>% que se retiene de la parte del premio que pasa de <see cref="Exento"/>.</summary>
		public double Retencion { get; set; } = 20;
		public double Exento { get; set; } = 40000;

		/// <summary>Por debajo de estos importes la categoría no se cobra (de 14 a 11 y con 10).</summary>
		public double Minimo { get; set; } = 1;
		public double Minimo10 { get; set; } = 1.5;

		/// <summary>«+1 AC» del Excel: si aciertas, eres un acertante más de los estimados.</summary>
		public bool ContarmeComoAcertante { get; set; } = true;
	}

	/// <summary>Resultado de una categoría (14…10 aciertos).</summary>
	public sealed class CategoriaEsperanza
	{
		public int Aciertos { get; set; }

		/// <summary>Probabilidad real de que la columna saque exactamente estos aciertos.</summary>
		public double ProbabilidadReal { get; set; }

		/// <summary>Fracción de lo apostado que tendría estos aciertos si la columna fuera la ganadora.</summary>
		public double ProbabilidadApostada { get; set; }

		/// <summary>Premio neto medio cuando la columna saca esta categoría (EM ÷ probabilidad real).</summary>
		public double PremioMedio { get; set; }

		public double Esperanza { get; set; }
	}

	public sealed class ResultadoEsperanza
	{
		public IReadOnlyList<CategoriaEsperanza> Categorias { get; set; }
		public double Apuestas { get; set; }
		public double Precio { get; set; }
		public double Total => Categorias.Sum(c => c.Esperanza);

		/// <summary>EM por cada unidad apostada (1 = se recupera lo apostado).</summary>
		public double Rendimiento => Total / Precio;
	}

	/// <summary>
	/// Esperanza matemática (EM) exacta de una columna por categoría (W14, DT-91; no existe en el WinForms), como el Excel
	/// «EM Categorias.xlsm» del dueño. Mismo cálculo que la app Mac (<c>CasosEsperanza.calcular</c>).
	/// </summary>
	public static class EsperanzaMatematica
	{
		/// <summary>Probabilidad de exactamente k aciertos (k = 0…n) si el partido i se acierta con probabilidad p[i].</summary>
		public static double[] Distribucion(IReadOnlyList<double> p)
		{
			var d = new double[] { 1 };
			foreach (double q in p)
			{
				var n = new double[d.Length + 1];
				for (int k = 0; k < d.Length; k++)
				{
					n[k] += d[k] * (1 - q);
					n[k + 1] += d[k] * q;
				}
				d = n;
			}
			return d;
		}

		/// <summary>
		/// Probabilidad de 1, X y 2 por partido: cada fila dividida por su suma. Se usan los 14 primeros partidos.
		/// Una fila que sume 0 o tenga negativos ⇒ ArgumentException «Faltan los porcentajes {nombre} del partido N.».
		/// </summary>
		public static double[][] Normalizar(double[,] v, string nombre)
		{
			if (v.GetLength(0) < 14 || v.GetLength(1) < 3)
				throw new ArgumentException("Faltan los porcentajes " + nombre + " de los 14 partidos.");
			var r = new double[14][];
			for (int i = 0; i < 14; i++)
			{
				double a = v[i, 0], b = v[i, 1], c = v[i, 2];
				double s = a + b + c;
				if (!(s > 0) || a < 0 || b < 0 || c < 0)
					throw new ArgumentException("Faltan los porcentajes " + nombre + " del partido " + (i + 1) + ".");
				r[i] = new[] { a / s, b / s, c / s };
			}
			return r;
		}

		/// <summary>
		/// EM exacta de <paramref name="columna"/>: recorre los 19.321 resultados con premio (el propio, los 28 a un signo,
		/// 364 a dos, 2.912 a tres y 16.016 a cuatro). En cada uno: acertantes = fracción apostada con esos aciertos × nº de
		/// apuestas (+1 si te cuentas; como mínimo 1), premio bruto = fondo de la categoría ÷ acertantes, neto tras el
		/// mínimo y la retención, y se suma probabilidad real del resultado × neto.
		/// </summary>
		public static ResultadoEsperanza Calcular(string columna, double[,] apostados, double[,] reales, ParametrosEsperanza p)
		{
			string c = (columna ?? "").ToUpperInvariant();
			if (c.Length != 14 || c.Any(s => s != '1' && s != 'X' && s != '2'))
				throw new ArgumentException("La columna debe tener 14 signos 1, X o 2.");
			if (!(p.Recaudacion > 0) || !(p.Precio > 0) || p.Reparto == null || p.Reparto.Length != 5)
				throw new ArgumentException("La recaudación y el precio de la apuesta deben ser mayores que 0.");

			int[] col = c.Select(s => "1X2".IndexOf(s)).ToArray();
			double[][] ap = Normalizar(apostados, "apostados");
			double[][] re = Normalizar(reales, "reales");
			double apuestas = p.Recaudacion / p.Precio;

			double Neto(double bruto, int aciertos)
			{
				if (bruto < (aciertos == 10 ? p.Minimo10 : p.Minimo)) return 0;
				return bruto > p.Exento ? p.Exento + (bruto - p.Exento) * (1 - p.Retencion / 100) : bruto;
			}

			var esperanza = new double[5];
			var probabilidad = new double[5];
			var w = (int[])col.Clone();
			var pAp = new double[14];

			void Visitar(int desde, int cambios)
			{
				int aciertos = 14 - cambios;
				for (int i = 0; i < 14; i++) pAp[i] = ap[i][w[i]];
				double x = Distribucion(pAp)[aciertos] * apuestas;
				double acertantes = Math.Max(1, p.ContarmeComoAcertante
					? Math.Floor(x + 1.5)
					: Math.Round(x, MidpointRounding.AwayFromZero));
				double real = 1;
				for (int i = 0; i < 14; i++) real *= re[i][w[i]];
				esperanza[cambios] += real * Neto(p.Reparto[cambios] / 100 * p.Recaudacion / acertantes, aciertos);
				probabilidad[cambios] += real;
				if (cambios >= 4) return;
				for (int i = desde; i < 14; i++)
				{
					int original = w[i];
					for (int d = 1; d <= 2; d++)
					{
						w[i] = (original + d) % 3;
						Visitar(i + 1, cambios + 1);
					}
					w[i] = original;
				}
			}

			Visitar(0, 0);

			double[] propia = Distribucion(Enumerable.Range(0, 14).Select(i => ap[i][col[i]]).ToArray());
			var categorias = Enumerable.Range(0, 5).Select(m => new CategoriaEsperanza
			{
				Aciertos = 14 - m,
				ProbabilidadReal = probabilidad[m],
				ProbabilidadApostada = propia[14 - m],
				PremioMedio = probabilidad[m] > 0 ? esperanza[m] / probabilidad[m] : 0,
				Esperanza = esperanza[m],
			}).ToList();

			return new ResultadoEsperanza { Categorias = categorias, Apuestas = apuestas, Precio = p.Precio };
		}
	}
}
