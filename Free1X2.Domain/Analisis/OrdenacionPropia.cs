// Free1X2 · WinUI 3 — WIN3
using System.Threading.Tasks;

namespace Free1X2.Analisis
{
	/// <summary>La clave por la que ordena <see cref="OrdenacionPropia"/>; un struct, para que el JIT la meta en línea.</summary>
	public interface IClaveOrden<T>
	{
		double De(in T x);
	}

	/// <summary>Ordenar por probabilidad y Banco de pruebas.</summary>
	public struct PorProbabilidadDiferencial : IClaveOrden<ApuestaProbableCentral>
	{
		public double De(in ApuestaProbableCentral x) => x.ProbabilidadDiferencial;
	}

	/// <summary>Rentabilidad y Tramificar. De float a double es exacto: las comparaciones no cambian.</summary>
	public struct PorProbabilidad : IClaveOrden<ApuestaProbable>, IClaveOrden<ApuestaProbEscrutada>
	{
		public double De(in ApuestaProbable x) => x.Probabilidad;
		public double De(in ApuestaProbEscrutada x) => x.Probabilidad;
	}

	/// <summary>
	/// El quicksort propio <c>ordena(izq, der)</c> de Ordenar por probabilidad, Banco de pruebas, Rentabilidad y
	/// Tramificar: ascendente, pivote central y las guardas <c>j &lt;= der</c> / <c>j &gt; izq</c> tal cual; no estable,
	/// así que no vale otra ordenación (los empates quedarían en otro orden). #13: tras cada partición las dos mitades
	/// no se tocan y se ordenan a la vez, lo que deja el mismo orden que una tras otra.
	/// </summary>
	public static class OrdenacionPropia
	{
		public static void Ordenar<T, K>(T[] a, int izq, int der) where K : struct, IClaveOrden<T>
		{
			Ordenar<T, K>(a, izq, der, 6);
		}

		private static void Ordenar<T, K>(T[] a, int izq, int der, int niveles) where K : struct, IClaveOrden<T>
		{
			K k = default;
			int i = izq, j = der;
			double x = k.De(a[(izq + der) / 2]);
			do
			{
				while (k.De(a[i]) < x && j <= der) i++;
				while (x < k.De(a[j]) && j > izq) j--;
				if (i <= j)
				{
					T aux = a[i];
					a[i] = a[j];
					a[j] = aux;
					i++; j--;
				}
			} while (i <= j);
			// ponytail: 6 niveles (64 trozos) y en un hilo por debajo de 65.536; subir si hay más núcleos.
			if (niveles > 0 && der - izq >= 65536)
			{
				Parallel.Invoke(
					() => { if (izq < j) Ordenar<T, K>(a, izq, j, niveles - 1); },
					() => { if (i < der) Ordenar<T, K>(a, i, der, niveles - 1); });
			}
			else
			{
				if (izq < j) Ordenar<T, K>(a, izq, j, 0);
				if (i < der) Ordenar<T, K>(a, i, der, 0);
			}
		}
	}
}
