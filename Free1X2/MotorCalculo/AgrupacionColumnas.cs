using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
    public class AgrupacionColumnas
    {
        private int noElementos;
        private List<int> noAciertos;
        private List<int> numero;
        public AgrupacionColumnas(List<int> num, int elementos, List<int> aciertos)
        {
            noElementos = elementos;
            noAciertos = aciertos;
            numero = num;
        }
        public List<int> Numero
        {
            get { return numero; }
        }
        public int NoElementos
        {
            get { return noElementos; }
        }
        public List<int> NoAciertos
        {
            get { return noAciertos; }
        }

    }
}
