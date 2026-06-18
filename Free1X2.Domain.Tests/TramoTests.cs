using Free1X2.Utils;
using Xunit;

namespace Free1X2.Domain.Tests
{
    /// <summary>
    /// Golden-master de <see cref="Tramo"/>: acumulador de un tramo del escrutinio
    /// (aciertos por categoria 14..10, columnas premiadas, importes y balance).
    ///
    /// Comportamiento caracterizado:
    ///  - PonerAciertos(int[5]) mapea: [0]=P14, [1]=P13, [2]=P12, [3]=P11, [4]=P10.
    ///  - CalculaTotalImportePremios: TotalImportePremios = sum(aciertos[i]*premios[i]);
    ///    Balance = TotalImportePremios - NumColumnasTramo * 0.5 (coste 0,5 por columna).
    ///  - AddTramo suma aciertos, columnas premiadas, importes y balance de otro tramo.
    /// </summary>
    public class TramoTests
    {
        private static readonly double[] Premios = { 1000.0, 100.0, 10.0, 5.0, 1.0 };

        [Fact]
        public void PonerAciertos_ExponePropiedadesP14aP10()
        {
            var tr = new Tramo(Premios);
            tr.PonerAciertos(new[] { 5, 4, 3, 2, 1 });

            Assert.Equal(5, tr.P14);
            Assert.Equal(4, tr.P13);
            Assert.Equal(3, tr.P12);
            Assert.Equal(2, tr.P11);
            Assert.Equal(1, tr.P10);
        }

        [Fact]
        public void CalculaTotalImportePremios_SumaPonderadaPorPremios()
        {
            var tr = new Tramo(Premios);
            tr.PonerAciertos(new[] { 1, 2, 3, 4, 5 });
            tr.NumColumnasTramo = 0;

            tr.CalculaTotalImportePremios();

            // 1*1000 + 2*100 + 3*10 + 4*5 + 5*1 = 1000+200+30+20+5 = 1255.
            Assert.Equal(1255.0, tr.TotalImportePremios);
        }

        [Fact]
        public void CalculaTotalImportePremios_BalanceDescuentaMedioPorColumna()
        {
            var tr = new Tramo(Premios);
            tr.PonerAciertos(new[] { 1, 0, 0, 0, 0 }); // un pleno al 14 = 1000
            tr.NumColumnasTramo = 100;                 // coste 100 * 0,5 = 50

            tr.CalculaTotalImportePremios();

            Assert.Equal(1000.0, tr.TotalImportePremios);
            Assert.Equal(950.0, tr.Balance); // 1000 - 50
        }

        [Fact]
        public void CalculaTotalImportePremios_SinPremios_BalanceNegativo()
        {
            var tr = new Tramo(Premios);
            tr.PonerAciertos(new[] { 0, 0, 0, 0, 0 });
            tr.NumColumnasTramo = 10; // coste 10 * 0,5 = 5

            tr.CalculaTotalImportePremios();

            Assert.Equal(0.0, tr.TotalImportePremios);
            Assert.Equal(-5.0, tr.Balance);
        }

        [Fact]
        public void Propiedades_ValoresSeConservan()
        {
            var tr = new Tramo(Premios)
            {
                NumeroDeTramo = 7,
                ValorIzquierda = 3,
                ValorDerecha = 9,
                NumColumnasTramo = 42,
                ProbAcumulada = 0.25,
                ColumnasPremiadas = 11
            };

            Assert.Equal(7, tr.NumeroDeTramo);
            Assert.Equal(3, tr.ValorIzquierda);
            Assert.Equal(9, tr.ValorDerecha);
            Assert.Equal(42, tr.NumColumnasTramo);
            Assert.Equal(0.25, tr.ProbAcumulada);
            Assert.Equal(11, tr.ColumnasPremiadas);
        }

        [Fact]
        public void AddTramo_AcumulaAciertosImportesYBalance()
        {
            var origen = new Tramo(Premios);
            origen.PonerAciertos(new[] { 1, 2, 3, 4, 5 });
            origen.NumColumnasTramo = 0;
            origen.ColumnasPremiadas = 6;
            origen.CalculaTotalImportePremios(); // TotalImportePremios = 1255, Balance = 1255

            var destino = new Tramo(Premios);
            destino.PonerAciertos(new[] { 10, 20, 30, 40, 50 });

            destino.AddTramo(origen);

            // aciertos sumados
            Assert.Equal(11, destino.P14);
            Assert.Equal(22, destino.P13);
            Assert.Equal(33, destino.P12);
            Assert.Equal(44, destino.P11);
            Assert.Equal(55, destino.P10);

            // columnas premiadas, importes y balance acumulados desde origen
            Assert.Equal(6, destino.ColumnasPremiadas);
            Assert.Equal(1255.0, destino.TotalImportePremios);
            Assert.Equal(1255.0, destino.Balance);
        }
    }
}
