using System.Collections.Generic;


namespace Free1X2.MotorCalculo
{
    public class ControladorRelacionesCP3
    {
        private List<RelacionCP3> relaciones;

        public ControladorRelacionesCP3()
        {
            relaciones = new List<RelacionCP3>();
        }

        public List<RelacionCP3> Relaciones
        {
            get { return relaciones; }
            set { relaciones = value; }
        }
        public bool AnalizaRelaciones()
        {
            for (int i = 0; i < Relaciones.Count; i++)
            {
                Relaciones[i].ActualizaValores();
                if(!Relaciones[i].Analiza())
                {
                    return false;
                }

            }
            return true;
        }

        public void Analiza(ref string txt)
        {
            txt = "";
            for (int i = 0; i < Relaciones.Count; i++)
            {
                RelacionCP3 rel = Relaciones[i];
                txt += rel.Analiza(i + 1);
            }
        }

    }
}
