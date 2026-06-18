// Free1X2 · WinUI 3 — WIN3
namespace Free1X2.MotorCalculo.Estadisticas
{
    public class Estadistica
    {
        protected string archivo = "";
        protected float cumplimiento;

        public string Archivo
        {
            get { return archivo; }
            set { archivo = value; }
        }
        public float Cumplimiento
        {
            get { return cumplimiento; }
            set { cumplimiento = value; }
        }

    }
}
