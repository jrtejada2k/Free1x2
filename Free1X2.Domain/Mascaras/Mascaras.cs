// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Free1X2.Mascaras
{
	/// <summary>
	/// Una máscara (W9, DT-84; no existe en el WinForms): el signo o signos de cada partido del
	/// boleto, con un nombre y sin equipos. Se aplica a la jornada cargada y se escruta contra la
	/// base de ganadoras de una liga (<see cref="CasosHistorico"/>). Formato compartido con la app
	/// Mac: <c>Mascaras/&lt;nombre&gt;.mas</c>, una línea por partido (<c>1</c>, <c>1,X</c>,
	/// <c>1,X,2</c>), UTF-8 sin BOM y CRLF.
	/// </summary>
	public sealed class Mascara
	{
		public Mascara(string nombre, IReadOnlyList<string> pronosticos)
		{
			Nombre = nombre;
			Pronosticos = pronosticos;
		}

		public string Nombre { get; }

		/// <summary>Un pronóstico por partido en el formato del boleto («1», «1,X», «1,X,2»).</summary>
		public IReadOnlyList<string> Pronosticos { get; }

		/// <summary>Columnas que desarrolla: el producto de los signos de cada partido.</summary>
		public long Columnas => Pronosticos.Aggregate(1L, (c, p) => c * CasosMascaras.NumSignos(p));

		public int Fijos => Pronosticos.Count(p => CasosMascaras.NumSignos(p) == 1);
		public int Dobles => Pronosticos.Count(p => CasosMascaras.NumSignos(p) == 2);
		public int Triples => Pronosticos.Count(p => CasosMascaras.NumSignos(p) == 3);
	}

	/// <summary>
	/// Lo que una máscara habría acertado en el histórico: por cada nº de aciertos premiado (10…14),
	/// las columnas que lo lograron sumando todas las jornadas.
	/// </summary>
	public sealed class ResultadoMascara
	{
		public string Nombre { get; set; } = "";
		public long Columnas { get; set; }

		/// <summary><c>Premios[k]</c> = columnas con k aciertos (k = 10…14).</summary>
		public Dictionary<int, long> Premios { get; } = new Dictionary<int, long>();

		/// <summary>Jornadas en las que al menos una columna tuvo premio.</summary>
		public int JornadasConPremio { get; set; }
		public int Jornadas { get; set; }

		public long Premio(int aciertos) => Premios.TryGetValue(aciertos, out long n) ? n : 0;
	}

	public static class CasosMascaras
	{
		public const int MinAciertosPremiados = 10;
		public const int MaxAciertosPremiados = 14;

		private const string Prohibidos = "/\\:*?\"<>|";
		private static readonly string[] SignosValidos = { "1", "X", "2" };
		private static readonly Encoding Utf8SinBom = new UTF8Encoding(false);

		public const string MensajeNombreNoValido =
			"El nombre de la máscara no puede estar vacío ni llevar / \\ : * ? \" < > |.";

		public static string Carpeta(string carpetaBase) => Path.Combine(carpetaBase, "Mascaras");

		private static string Ruta(string nombre, string carpetaBase) => Path.Combine(Carpeta(carpetaBase), nombre + ".mas");

		/// <summary>Nombre recortado, no vacío y válido como nombre de fichero en Windows y macOS; si no, null.</summary>
		public static string NombreValido(string nombre)
		{
			string n = (nombre ?? "").Trim();
			return n.Length == 0 || n.IndexOfAny(Prohibidos.ToCharArray()) >= 0 ? null : n;
		}

		/// <summary>Signos marcados de un pronóstico («1,X» ⇒ 2). Vacío ⇒ triple, como el boleto.</summary>
		public static int NumSignos(string pronostico)
		{
			string p = string.IsNullOrEmpty(pronostico) ? "1,X,2" : pronostico;
			return SignosValidos.Count(s => p.Contains(s));
		}

		public static bool Existe(string nombre, string carpetaBase) => File.Exists(Ruta(nombre, carpetaBase));

		/// <summary>Guarda (o sobrescribe) <c>Mascaras/&lt;nombre&gt;.mas</c>. Nombre no válido ⇒ ArgumentException.</summary>
		public static void Guardar(Mascara m, string carpetaBase)
		{
			string nombre = NombreValido(m.Nombre) ?? throw new ArgumentException(MensajeNombreNoValido);
			Directory.CreateDirectory(Carpeta(carpetaBase));
			EscribirAtomico(Ruta(nombre, carpetaBase), string.Join("\r\n", m.Pronosticos) + "\r\n");
		}

		/// <summary>
		/// Las guardadas para <paramref name="numeroPartidos"/> partidos, por nombre. Se saltan las que,
		/// quitando las líneas en blanco, no tienen exactamente N líneas o tienen un signo que no es
		/// 1, X o 2, o repetido.
		/// </summary>
		public static List<Mascara> Listar(int numeroPartidos, string carpetaBase)
		{
			var r = new List<Mascara>();
			string carpeta = Carpeta(carpetaBase);
			if (!Directory.Exists(carpeta)) return r;

			foreach (string fichero in Directory.GetFiles(carpeta, "*.mas").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
			{
				string[] lineas;
				try
				{
					lineas = File.ReadAllText(fichero, Encoding.UTF8)
						.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).ToArray();
				}
				catch (IOException) { continue; }
				catch (UnauthorizedAccessException) { continue; }

				bool validos = lineas.All(x =>
				{
					string[] s = x.Split(',');
					return s.Length > 0 && s.Distinct().Count() == s.Length && s.All(c => SignosValidos.Contains(c));
				});
				if (lineas.Length == numeroPartidos && validos)
					r.Add(new Mascara(Path.GetFileNameWithoutExtension(fichero), lineas));
			}
			return r;
		}

		public static void Borrar(string nombre, string carpetaBase) => File.Delete(Ruta(nombre, carpetaBase));

		/// <summary>
		/// Columnas de la máscara con k aciertos (k = 0…partidos) frente a <paramref name="ganadora"/>,
		/// sin desarrollarlas: empezando con c = [1], cada partido hace
		/// <c>nuevo[k] += c[k]·fallos</c> y <c>nuevo[k+1] += c[k]·aciertos</c>.
		/// </summary>
		public static long[] Aciertos(IReadOnlyList<string> pronosticos, string ganadora)
		{
			var cuenta = new long[] { 1 };
			int n = Math.Min(pronosticos.Count, ganadora.Length);
			for (int i = 0; i < n; i++)
			{
				string p = string.IsNullOrEmpty(pronosticos[i]) ? "1,X,2" : pronosticos[i];
				int signos = NumSignos(p);
				bool acierta = p.Contains(ganadora[i]);
				int fallos = signos - (acierta ? 1 : 0);

				var nueva = new long[cuenta.Length + 1];
				for (int k = 0; k < cuenta.Length; k++)
				{
					nueva[k] += cuenta[k] * fallos;
					if (acierta) nueva[k + 1] += cuenta[k];
				}
				cuenta = nueva;
			}
			return cuenta;
		}

		/// <summary>Escruta la máscara contra cada ganadora (los 14 primeros partidos) y suma las premiadas de 10 a 14.</summary>
		public static ResultadoMascara Escrutar(Mascara m, IReadOnlyList<string> ganadoras)
		{
			var r = new ResultadoMascara { Nombre = m.Nombre, Columnas = m.Columnas, Jornadas = ganadoras.Count };
			var catorce = m.Pronosticos.Take(14).ToList();
			foreach (string g in ganadoras)
			{
				long[] a = Aciertos(catorce, g);
				bool premiada = false;
				for (int k = MinAciertosPremiados; k <= MaxAciertosPremiados; k++)
				{
					if (k < a.Length && a[k] > 0)
					{
						r.Premios[k] = r.Premio(k) + a[k];
						premiada = true;
					}
				}
				if (premiada) r.JornadasConPremio++;
			}
			return r;
		}

		/// <summary>Escritura atómica en UTF-8 sin BOM (fichero temporal y reemplazo).</summary>
		internal static void EscribirAtomico(string ruta, string texto)
		{
			string tmp = ruta + ".tmp";
			File.WriteAllText(tmp, texto, Utf8SinBom);
			File.Move(tmp, ruta, overwrite: true);
		}
	}
}
