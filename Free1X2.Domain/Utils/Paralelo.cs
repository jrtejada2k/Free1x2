// Free1X2 · WinUI 3 — WIN3
using System;
using System.Threading.Tasks;

namespace Free1X2.Utils
{
    /// <summary>
    /// Reparto de un bucle en tramos para hacerlo en varios núcleos y juntar los resultados después
    /// como si fuera uno (mismo criterio que la Mac, <c>Paralelo.swift</c>).
    /// </summary>
    public static class Paralelo
    {
        /// <summary>
        /// Límites de los tramos para <paramref name="n"/> elementos: cuatro por núcleo (para repartir
        /// bien), con al menos <paramref name="minimo"/> elementos cada uno; con menos, uno.
        /// El tramo k es [t[k], t[k+1]).
        /// </summary>
        public static int[] Tramos(int n, int minimo = 32768)
        {
            if (n < 0) n = 0;
            int k = Math.Max(1, Math.Min(Environment.ProcessorCount * 4, n / minimo));
            var t = new int[k + 1];
            for (int i = 0; i <= k; i++) t[i] = (int)((long)n * i / k);
            return t;
        }

        /// <summary>
        /// <c>trabajo(0..n-1)</c> en todos los núcleos, cada resultado en su casilla y en orden.
        /// Con <paramref name="n"/> ≤ 1, en el hilo que llama.
        /// </summary>
        public static T[] EnParalelo<T>(int n, Func<int, T> trabajo)
        {
            var r = new T[n];
            if (n <= 1)
            {
                for (int i = 0; i < n; i++) r[i] = trabajo(i);
                return r;
            }
            Parallel.For(0, n, i => r[i] = trabajo(i));
            return r;
        }
    }
}
