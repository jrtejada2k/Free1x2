// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Free1X2.Online;

namespace Free1X2.Mascaras
{
	/// <summary>Una ganadora de la base: su jornada (null si no se conoce) y sus 14 signos.</summary>
	public sealed class GanadoraHistorica
	{
		public GanadoraHistorica(string jornada, string ganadora)
		{
			Jornada = jornada;
			Ganadora = ganadora;
		}

		public string Jornada { get; }
		public string Ganadora { get; }
	}

	/// <summary>Lo que hizo una importación en la base.</summary>
	public sealed class ResumenImportacion
	{
		public int Anadidas { get; set; }
		public int YaEstaban { get; set; }
		public int Actualizadas { get; set; }

		/// <summary>Líneas sin jornada: se añaden sin poder comprobar si ya estaban.</summary>
		public int SinJornada { get; set; }
		public int Total { get; set; }

		public string Mensaje
		{
			get
			{
				var p = new List<string> { Anadidas + " añadidas", YaEstaban + " ya estaban" };
				if (Actualizadas > 0) p.Add(Actualizadas + " actualizadas (otra columna para la misma jornada)");
				if (SinJornada > 0) p.Add(SinJornada + " sin jornada (añadidas sin comprobar si ya estaban)");
				return string.Join(", ", p) + ". La liga tiene ahora " + Total + " jornadas.";
			}
		}
	}

	/// <summary>
	/// Base de ganadoras por liga (W12, DT-88; no existe en el WinForms): <c>Historico/&lt;liga&gt;.txt</c>,
	/// una línea <c>jornada;columna</c> (o solo <c>columna</c>) por jornada, UTF-8 sin BOM y CRLF,
	/// compartido con la app Mac. Las importaciones y las descargas de clubprogol se suman sin
	/// duplicar jornadas. No usa <c>Ganadoras/</c>: esa carpeta es de Escrutador/Escrutinios.
	/// </summary>
	public static class CasosHistorico
	{
		public const string MensajeNombreNoValido =
			"El nombre de la liga no puede estar vacío ni llevar / \\ : * ? \" < > |.";

		/// <summary>Liga en la que se suman las descargas de un país: «España» (es) o «México» (mx).</summary>
		public static string Liga(string pais) =>
			string.Equals(pais, "mx", StringComparison.OrdinalIgnoreCase) ? "México" : "España";

		public static string Carpeta(string carpetaBase) => Path.Combine(carpetaBase, "Historico");

		private static string Ruta(string liga, string carpetaBase) => Path.Combine(Carpeta(carpetaBase), liga + ".txt");

		/// <summary>Las ligas de la base con su número de jornadas, por nombre (comparación natural).</summary>
		public static List<(string Nombre, int Jornadas)> Ligas(string carpetaBase)
		{
			string carpeta = Carpeta(carpetaBase);
			if (!Directory.Exists(carpeta)) return new List<(string, int)>();
			return Directory.GetFiles(carpeta, "*.txt")
				.Select(f => Path.GetFileNameWithoutExtension(f))
				.Select(n => (n, Leer(n, carpetaBase).Count))
				.OrderBy(l => l.n, ComparadorNatural.Instancia)
				.ToList();
		}

		/// <summary>Las ganadoras guardadas de la liga (vacío si no existe o no se puede leer).</summary>
		public static List<GanadoraHistorica> Leer(string liga, string carpetaBase)
		{
			try
			{
				string ruta = Ruta(liga, carpetaBase);
				return File.Exists(ruta) ? Analizar(File.ReadAllText(ruta, Encoding.UTF8)) : new List<GanadoraHistorica>();
			}
			catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is FormatException)
			{
				return new List<GanadoraHistorica>();
			}
		}

		/// <summary>
		/// Lectura de un archivo (la misma para importar y para la base). Cada línea se recorta y pasa
		/// a mayúsculas; las líneas en blanco se saltan; con <c>;</c>, lo de delante es la jornada y
		/// lo de detrás la columna; de la columna cuentan sus 14 primeros signos. Una línea mal
		/// formada detiene la lectura con <see cref="FormatException"/> y su número (desde 1,
		/// contando las líneas en blanco).
		/// </summary>
		public static List<GanadoraHistorica> Analizar(string texto)
		{
			var r = new List<GanadoraHistorica>();
			string[] lineas = (texto ?? "").Split('\n');
			for (int i = 0; i < lineas.Length; i++)
			{
				string l = lineas[i].Trim().ToUpperInvariant();
				if (l.Length == 0) continue;

				string jornada = null;
				string columna = l;
				int pc = l.IndexOf(';');
				if (pc >= 0)
				{
					jornada = l.Substring(0, pc).Trim();
					columna = l.Substring(pc + 1).Trim();
				}
				string g = columna.Length > 14 ? columna.Substring(0, 14) : columna;
				if (jornada?.Length == 0 || g.Length != 14 || g.Any(c => c != '1' && c != 'X' && c != '2'))
				{
					throw new FormatException("La línea " + (i + 1) +
						" no es «jornada;columna» ni una columna de 14 signos 1, X o 2: «" + l + "».");
				}
				r.Add(new GanadoraHistorica(jornada, g));
			}
			return r;
		}

		/// <summary>Crea la liga vacía (si no existe) para poder importar en ella; si ya existe, no la vacía.</summary>
		public static string Crear(string liga, string carpetaBase)
		{
			string n = CasosMascaras.NombreValido(liga) ?? throw new ArgumentException(MensajeNombreNoValido);
			if (!File.Exists(Ruta(n, carpetaBase))) Guardar(new List<GanadoraHistorica>(), n, carpetaBase);
			return n;
		}

		/// <summary>
		/// Suma <paramref name="nuevas"/> a la liga, en orden: sin jornada ⇒ al final; jornada nueva ⇒
		/// al final; misma jornada y columna ⇒ nada; misma jornada y otra columna ⇒ se sustituye en
		/// su sitio. Guarda la base entera (escritura atómica).
		/// </summary>
		public static ResumenImportacion Anadir(IEnumerable<GanadoraHistorica> nuevas, string liga, string carpetaBase)
		{
			var base_ = Leer(liga, carpetaBase);
			var indice = new Dictionary<string, int>();
			for (int i = 0; i < base_.Count; i++)
				if (base_[i].Jornada != null) indice[base_[i].Jornada] = i;

			var r = new ResumenImportacion();
			foreach (var g in nuevas)
			{
				if (g.Jornada == null)
				{
					base_.Add(g);
					r.SinJornada++;
				}
				else if (indice.TryGetValue(g.Jornada, out int i))
				{
					if (base_[i].Ganadora == g.Ganadora)
					{
						r.YaEstaban++;
					}
					else
					{
						base_[i] = g;
						r.Actualizadas++;
					}
				}
				else
				{
					indice[g.Jornada] = base_.Count;
					base_.Add(g);
					r.Anadidas++;
				}
			}
			Guardar(base_, liga, carpetaBase);
			r.Total = base_.Count;
			return r;
		}

		/// <summary>«Añadir ganadoras desde archivo…»: un archivo sin ninguna ganadora es un error.</summary>
		public static ResumenImportacion Importar(string texto, string liga, string carpetaBase)
		{
			var nuevas = Analizar(texto);
			if (nuevas.Count == 0) throw new FormatException("El archivo no tiene ninguna ganadora.");
			return Anadir(nuevas, liga, carpetaBase);
		}

		/// <summary>Las ganadoras descargadas de clubprogol se suman a la liga «España» o «México».</summary>
		public static ResumenImportacion SumarDescarga(IEnumerable<ResultadoHistorico> historico, string pais, string carpetaBase) =>
			Anadir(historico.Select(h => new GanadoraHistorica(h.Jornada.ToString(), h.Ganadora)), Liga(pais), carpetaBase);

		public static void Borrar(string liga, string carpetaBase) => File.Delete(Ruta(liga, carpetaBase));

		private static void Guardar(List<GanadoraHistorica> ganadoras, string liga, string carpetaBase)
		{
			Directory.CreateDirectory(Carpeta(carpetaBase));
			var sb = new StringBuilder();
			foreach (var g in ganadoras)
				sb.Append(g.Jornada != null ? g.Jornada + ";" : "").Append(g.Ganadora).Append("\r\n");
			CasosMascaras.EscribirAtomico(Ruta(liga, carpetaBase), sb.ToString());
		}

		/// <summary>Orden natural («Liga 2» antes que «Liga 10»), sin distinguir mayúsculas.</summary>
		private sealed class ComparadorNatural : IComparer<string>
		{
			public static readonly ComparadorNatural Instancia = new ComparadorNatural();

			public int Compare(string a, string b)
			{
				a ??= ""; b ??= "";
				int i = 0, j = 0;
				while (i < a.Length && j < b.Length)
				{
					if (char.IsDigit(a[i]) && char.IsDigit(b[j]))
					{
						int si = i, sj = j;
						while (i < a.Length && char.IsDigit(a[i])) i++;
						while (j < b.Length && char.IsDigit(b[j])) j++;
						string na = a.Substring(si, i - si).TrimStart('0'), nb = b.Substring(sj, j - sj).TrimStart('0');
						int c = na.Length != nb.Length ? na.Length.CompareTo(nb.Length) : string.CompareOrdinal(na, nb);
						if (c != 0) return c;
					}
					else
					{
						int c = string.Compare(a[i].ToString(), b[j].ToString(), StringComparison.CurrentCultureIgnoreCase);
						if (c != 0) return c;
						i++; j++;
					}
				}
				return (a.Length - i).CompareTo(b.Length - j);
			}
		}
	}
}
