using System;
using Free1X2.Analisis;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// #13: <see cref="OrdenacionPropia"/> (las dos mitades a la vez) deja exactamente el mismo orden que el
    /// <c>ordena</c> de un hilo de las pantallas (aquí copiado tal cual), con empates, 9E+10 y NaN.
    /// 600.000 elementos son varios niveles en paralelo (65.536 como mínimo).
    /// </summary>
    public class OrdenacionPropiaTests
    {
        private const int N = 600_000;

        [Fact]
        public void Central_PorDiferencial_MismoOrden()
        {
            var r = new Random(1);
            var a = new ApuestaProbableCentral[N];
            for (int i = 0; i < N; i++)
            {
                a[i].Columna = i;
                a[i].Probabilidad = r.Next(1000) / 7f;
                a[i].ProbabilidadDiferencial = i % 5 == 0 ? (float)9E+10 : i % 977 == 0 ? float.NaN : r.Next(5000) / 13f;
            }
            var b = (ApuestaProbableCentral[])a.Clone();
            OrdenaCentral(a, 0, N - 1);
            OrdenacionPropia.Ordenar<ApuestaProbableCentral, PorProbabilidadDiferencial>(b, 0, N - 1);
            for (int i = 0; i < N; i++) Assert.Equal(a[i].Columna, b[i].Columna);
        }

        [Fact]
        public void Probable_PorProbabilidad_MismoOrden()
        {
            var r = new Random(2);
            var a = new ApuestaProbable[N];
            for (int i = 0; i < N; i++) { a[i].Columna = i; a[i].Probabilidad = r.Next(3000) / 11f; }
            var b = (ApuestaProbable[])a.Clone();
            OrdenaProbable(a, 0, N - 1);
            OrdenacionPropia.Ordenar<ApuestaProbable, PorProbabilidad>(b, 0, N - 1);
            for (int i = 0; i < N; i++) Assert.Equal(a[i].Columna, b[i].Columna);
        }

        [Fact]
        public void Escrutada_PorProbabilidad_MismoOrden()
        {
            var r = new Random(3);
            var a = new ApuestaProbEscrutada[N];
            for (int i = 0; i < N; i++) { a[i].Columna = i; a[i].Probabilidad = -r.Next(4000) / 3.0; a[i].Aciertos = (byte)(i % 15); }
            var b = (ApuestaProbEscrutada[])a.Clone();
            OrdenaEscrutada(a, 0, N - 1);
            OrdenacionPropia.Ordenar<ApuestaProbEscrutada, PorProbabilidad>(b, 0, N - 1);
            for (int i = 0; i < N; i++) { Assert.Equal(a[i].Columna, b[i].Columna); Assert.Equal(a[i].Aciertos, b[i].Aciertos); }
        }

        // ---- Los ordena de un hilo de las pantallas, antes de #13 ----

        private static void OrdenaCentral(ApuestaProbableCentral[] Ap14T, int izq, int der)
        {
            int i = izq, j = der;
            ApuestaProbableCentral x = Ap14T[(izq + der) / 2];
            do
            {
                while (Ap14T[i].ProbabilidadDiferencial < x.ProbabilidadDiferencial && j <= der) i++;
                while (x.ProbabilidadDiferencial < Ap14T[j].ProbabilidadDiferencial && j > izq) j--;
                if (i <= j) { var aux = Ap14T[i]; Ap14T[i] = Ap14T[j]; Ap14T[j] = aux; i++; j--; }
            } while (i <= j);
            if (izq < j) OrdenaCentral(Ap14T, izq, j);
            if (i < der) OrdenaCentral(Ap14T, i, der);
        }

        private static void OrdenaProbable(ApuestaProbable[] Ap14T, int izq, int der)
        {
            int i = izq, j = der;
            ApuestaProbable x = Ap14T[(izq + der) / 2];
            do
            {
                while ((Ap14T[i].Probabilidad < x.Probabilidad) && (j <= der)) i++;
                while ((x.Probabilidad < Ap14T[j].Probabilidad) && (j > izq)) j--;
                if (i <= j) { var aux = Ap14T[i]; Ap14T[i] = Ap14T[j]; Ap14T[j] = aux; i++; j--; }
            } while (i <= j);
            if (izq < j) OrdenaProbable(Ap14T, izq, j);
            if (i < der) OrdenaProbable(Ap14T, i, der);
        }

        private static void OrdenaEscrutada(ApuestaProbEscrutada[] Ap14T, int izq, int der)
        {
            int i = izq, j = der;
            ApuestaProbEscrutada x = Ap14T[(izq + der) / 2];
            do
            {
                while (Ap14T[i].Probabilidad < x.Probabilidad && j <= der) i++;
                while (x.Probabilidad < Ap14T[j].Probabilidad && j > izq) j--;
                if (i <= j) { var aux = Ap14T[i]; Ap14T[i] = Ap14T[j]; Ap14T[j] = aux; i++; j--; }
            } while (i <= j);
            if (izq < j) OrdenaEscrutada(Ap14T, izq, j);
            if (i < der) OrdenaEscrutada(Ap14T, i, der);
        }
    }
}
