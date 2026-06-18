// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;
using Free1X2.MotorCalculo;

namespace Free1X2.Analisis
{
    public class InformeColumnasABDON
    {
        RelacionCP3 relacionCP3;

        public InformeColumnasABDON(int[] serie)
        {
            relacionCP3 = new RelacionCP3(serie);
        }

        public int[,] AgrupacionesPasoFijo
        {
            get { return relacionCP3.AgrupacionesPasoFijo; }
        }
        public int[,] AgrupacionesSolapadas
        {
            get { return relacionCP3.AgrupacionesSolapadas; }
        }

        public int MinimoAciertos
        {
            get { return relacionCP3.MinimoAciertos; }
        }
        public int MaximoAciertos
        {
            get { return relacionCP3.MaximoAciertos; }
        }
        public int SumaTotalDeAciertos
        {
            get{return relacionCP3.SumaTotalDeAciertos;}
        }
        public List<EscaleraAciertos> Escaleras
        {
            get { return relacionCP3.Escaleras; }
        }
        public List<SandwichAciertos> Sandwichs
        {
            get { return relacionCP3.Sandwichs; }
        }
        public int[] SerieAciertos
        {
            get { return relacionCP3.SerieAciertos; }
        }
        public int NumeroDeEscaleras
        {
            get { return relacionCP3.NumeroDeEscaleras; }
        }
        public int NumeroDeEscalerasAscendentes
        {
            get { return relacionCP3.NumeroDeEscalerasAscendentes; }
        }
        public int NumeroDeEscalerasDescendentes
        {
            get { return relacionCP3.NumeroDeEscalerasDescendentes; }
        }
    }
}
