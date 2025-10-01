using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
    public class EscaleraAciertos
    {
        private List<int> escalones;
        private OrientacionEscalera orientacion;

        public List<int> Escalones
        {
            get { return escalones; }
            set { escalones = value; }
        }
        public int NumeroEscalones
        {
            get { return Escalones.Count; }
        }
        public OrientacionEscalera Orientacion
        {
            get
            {
                if (Escalones.Count > 0)
                {
                    if (Escalones[0] > Escalones[Escalones.Count - 1])
                    {
                        return OrientacionEscalera.Descendente;
                    }
                    return OrientacionEscalera.Ascendente;
                }
                return orientacion;
            }
        }
    }
}
