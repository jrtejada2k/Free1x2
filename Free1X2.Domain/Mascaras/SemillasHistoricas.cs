// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Free1X2.Mascaras
{
	/// <summary>
	/// Datos de fábrica que traía el Free1X2 original y que se habían perdido (W20, DT-96), la liga «México» del Progol
	/// (W21, DT-97) y las jornadas de España de 2007/08 a 2026/27 (W22, DT-98). Van en <c>Semillas/</c> junto al exe (la
	/// carpeta del exe es también la de los datos: si fueran a <c>Jornadas/</c>, una actualización pisaría los del
	/// usuario) y se copian a la carpeta de datos <b>solo si allí no existen</b>. Mismo comportamiento que la app Mac
	/// (<c>CasosInicio.arrancar</c>, <c>jornadasLAEAlPrecio</c>, <c>ganadorasDeResultados</c>, <c>ganadorasDeProgol</c>).
	/// </summary>
	public static class SemillasHistoricas
	{
		private static readonly CultureInfo Es = new CultureInfo("es-ES");

		/// <summary>Históricos que se copian tal cual: resultados de 1970 a 2006, porcentajes y ganadoras por temporada.</summary>
		public static readonly string[] Historicas =
			new[] { "Resultados", "HistoricoConcurso", "HistoricoMarca", "HistoricoQuinielista.com", "HistoricoQuinielista" }
				.Select(n => "Jornadas/" + n + ".txt")
				.Concat(Enumerable.Range(1983, 26).Select(a => "Ganadoras/" + a + "-" + ((a + 1) % 100).ToString("00") + ".txt"))
				.ToArray();

		/// <summary>Jornadas L.A.E. de 1988/89 a 2006/07, con los importes en apuestas de 0,50 € (ver <see cref="JornadasLAEAlPrecio"/>).</summary>
		public const string JornadasLAE = "Jornadas/InfoJornadasLAE.txt";

		/// <summary>Concursos del Progol de Lotería Nacional (767 a 2354), CSV oficial.</summary>
		public const string Progol = "Jornadas/Progol.csv";

		/// <summary>Jornadas de España de 2007/08 a 2026/27, ya en el formato de la liga (<c>jornada;columna</c>).</summary>
		public const string EspanaWeb = "Jornadas/EspanaDesde2007.txt";

		private static string Ruta(string carpeta, string relativa) =>
			Path.Combine(new[] { carpeta }.Concat(relativa.Split('/')).ToArray());

		/// <summary>
		/// Las jornadas L.A.E. guardan recaudación y premios en apuestas de 0,50 €; como el fichero se lee × el precio de
		/// Configuración (W16), al sembrarlo cada importe distinto de 0 pasa a <c>Math.Round(valor × 0,50 ÷ precio, 2)</c>
		/// (es-ES, sin separador de miles). Un 0 se deja tal cual. Con precio 0,50 no cambia nada. CRLF.
		/// </summary>
		public static string JornadasLAEAlPrecio(string texto, double precio)
		{
			if (!(precio > 0) || precio == 0.5) return texto;
			string[] lineas = texto.Split('\n');
			for (int n = 0; n < lineas.Length; n++)
			{
				string l = lineas[n].TrimEnd('\r');
				string[] v = l.Split('\t');
				if (v.Length >= 9)
				{
					for (int i = 3; i <= 8; i++)
					{
						if (!double.TryParse(v[i], NumberStyles.Float, Es, out double x) || x == 0) continue;
						v[i] = Math.Round(x * 0.5 / precio, 2).ToString(Es);
					}
					l = string.Join("\t", v);
				}
				lineas[n] = l;
			}
			return string.Join("\r\n", lineas);
		}

		/// <summary>
		/// Liga «España» desde <c>Resultados.txt</c> (<c>temporada⇥jornada⇥quiniela</c>, 1970–2006): las filas con 14
		/// signos 1, X o 2, con la jornada como «1970/1971-01» (no choca con la clave de clubprogol, la fecha).
		/// </summary>
		public static List<GanadoraHistorica> GanadorasDeResultados(string texto)
		{
			var r = new List<GanadoraHistorica>();
			foreach (string linea in texto.Split('\n'))
			{
				string[] v = linea.Trim().Split('\t');
				if (v.Length < 3 || !int.TryParse(v[0].Trim(), out int temporada)) continue;
				string g = v[2].Trim().ToUpperInvariant();
				if (g.Length > 14) g = g.Substring(0, 14);
				if (g.Length != 14 || g.Any(c => c != '1' && c != 'X' && c != '2')) continue;
				string j = v[1].Trim();
				if (j.Length < 2) j = j.PadLeft(2, '0');
				r.Add(new GanadoraHistorica(temporada + "/" + (temporada + 1) + "-" + j, g));
			}
			return r;
		}

		/// <summary>
		/// Liga «México» desde el CSV de Lotería Nacional (<c>NPRODUCTO,CONCURSO,R1…R14,BOLSA,FECHA</c>; L = 1, E = X, V = 2):
		/// <c>concurso;14 signos</c> por fila válida, del concurso más antiguo al más reciente. La cabecera y las filas sin
		/// 14 signos se saltan. El nº de concurso es la clave de la descarga de clubprogol, así que no se duplican.
		/// </summary>
		public static List<GanadoraHistorica> GanadorasDeProgol(string texto)
		{
			var signo = new Dictionary<string, string> { ["L"] = "1", ["E"] = "X", ["V"] = "2" };
			var r = new List<(int Concurso, GanadoraHistorica G)>();
			foreach (string linea in texto.Split('\n'))
			{
				string[] v = linea.Split(',').Select(s => s.Trim().ToUpperInvariant()).ToArray();
				if (v.Length < 16 || !int.TryParse(v[1], out int concurso)) continue;
				string g = string.Concat(v.Skip(2).Take(14).Select(s => signo.TryGetValue(s, out string x) ? x : ""));
				if (g.Length == 14) r.Add((concurso, new GanadoraHistorica(concurso.ToString(), g)));
			}
			return r.OrderBy(x => x.Concurso).Select(x => x.G).ToList();
		}

		/// <summary>
		/// Siembra de arranque. <paramref name="sembrarLigas"/> = la carpeta <c>Historico/</c> no existía al arrancar (nunca
		/// se ha guardado una liga): solo entonces se crean «España» (Resultados.txt + EspanaDesde2007.txt) y «México»
		/// (Progol.csv); si el usuario las borra, no se vuelven a sembrar. Nunca lanza: devuelve los errores para el log.
		/// </summary>
		public static List<string> Sembrar(string carpetaSemillas, string carpetaDatos, double precio, bool sembrarLigas)
		{
			var errores = new List<string>();

			foreach (string s in Historicas)
			{
				try
				{
					string origen = Ruta(carpetaSemillas, s), destino = Ruta(carpetaDatos, s);
					if (File.Exists(destino) || !File.Exists(origen)) continue;
					Directory.CreateDirectory(Path.GetDirectoryName(destino));
					File.Copy(origen, destino);
				}
				catch (Exception e) { errores.Add("Semilla " + s + ": " + e.Message); }
			}

			try
			{
				string origen = Ruta(carpetaSemillas, JornadasLAE), destino = Ruta(carpetaDatos, JornadasLAE);
				if (!File.Exists(destino) && File.Exists(origen))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(destino));
					string texto = File.ReadAllText(origen, Encoding.Latin1);
					File.WriteAllText(destino, JornadasLAEAlPrecio(texto, precio), Encoding.Latin1);
				}
			}
			catch (Exception e) { errores.Add("Semilla " + JornadasLAE + ": " + e.Message); }

			if (!sembrarLigas) return errores;

			string Leer(string s)
			{
				string ruta = Ruta(carpetaSemillas, s);
				return File.Exists(ruta) ? File.ReadAllText(ruta, Encoding.UTF8) : null;
			}

			try
			{
				var espana = new List<GanadoraHistorica>();
				string resultados = Leer("Jornadas/Resultados.txt");
				if (resultados != null) espana.AddRange(GanadorasDeResultados(resultados));
				string web = Leer(EspanaWeb);
				if (web != null) espana.AddRange(CasosHistorico.Analizar(web));
				if (espana.Count > 0) CasosHistorico.Guardar(espana, CasosHistorico.Liga("es"), carpetaDatos);
			}
			catch (Exception e) { errores.Add("Liga España: " + e.Message); }

			try
			{
				string progol = Leer(Progol);
				var mexico = progol != null ? GanadorasDeProgol(progol) : new List<GanadoraHistorica>();
				if (mexico.Count > 0) CasosHistorico.Guardar(mexico, CasosHistorico.Liga("mx"), carpetaDatos);
			}
			catch (Exception e) { errores.Add("Liga México: " + e.Message); }

			return errores;
		}
	}
}
