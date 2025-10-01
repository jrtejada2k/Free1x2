using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Free1X2.Analisis;

namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisDiferencias_Individual : UserControl
    {
        private ContenedorDiferencias contenedor;
        public CtrlAnalisisDiferencias_Individual(ContenedorDiferencias cont, int orden)
        {
            InitializeComponent();
            this.lblNumSimetria.Text = "Conjunto " + orden.ToString();
            this.contenedor = cont;
            int noCasillasApariciones = cont.NumDiferenciasPosibles;
            LlenarNumeros(noCasillasApariciones);
            LlenarVariantes(noCasillasApariciones);
            LlenarEquis(noCasillasApariciones);
            LlenarDibujos(noCasillasApariciones);
            LlenarDoses(noCasillasApariciones);
            LlenarInterrupciones(noCasillasApariciones);
            LlenarFormatos(noCasillasApariciones);

        }

        protected void LlenarNumeros(int cuantos)
        {
            int x = lblNoAciertos.Location.X + lblNoAciertos.Size.Width + 1;
            int y = lblNoAciertos.Location.Y;

            for (int i = 1; i < cuantos; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(i.ToString());
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                this.Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }

        protected void LlenarVariantes(int cuantos)
        {
            int x = lblVar.Location.X + lblVar.Size.Width + 1;
            int y = lblVar.Location.Y;

            for (int i = 1; i < cuantos; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(contenedor.Variantes[i].ToString());
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                this.Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarFormatos(int cuantos)
        {
            int x = lblFormatos.Location.X + lblFormatos.Size.Width + 1;
            int y = lblFormatos.Location.Y;

            for (int i = 1; i < cuantos; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(contenedor.Formatos[i].ToString());
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                this.Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarEquis(int cuantos)
        {
            int x = lblEquis.Location.X + lblEquis.Size.Width + 1;
            int y = lblEquis.Location.Y;

            for (int i = 1; i < cuantos; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(contenedor.Equis[i].ToString());
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                this.Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarDoses(int cuantos)
        {
            int x = lblDoses.Location.X + lblDoses.Size.Width + 1;
            int y = lblDoses.Location.Y;

            for (int i = 1; i < cuantos; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(contenedor.Doses[i].ToString());
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                this.Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarDibujos(int cuantos)
        {
            int x = lblDibujos.Location.X + lblDibujos.Size.Width + 1;
            int y = lblDibujos.Location.Y;

            for (int i = 1; i < cuantos; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(contenedor.Dibujos[i].ToString());
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                this.Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarInterrupciones(int cuantos)
        {
            int x = lblInterrupciones.Location.X + lblInterrupciones.Size.Width + 1;
            int y = lblInterrupciones.Location.Y;

            for (int i = 1; i < cuantos; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(contenedor.Interrupciones[i].ToString());
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                this.Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }

    }
}
