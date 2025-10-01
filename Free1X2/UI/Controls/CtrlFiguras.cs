// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
    public partial class CtrlFiguras : UserControl
    {

        public CtrlFiguras()
        {
            InitializeComponent();
            LlenarCasillas();
        }
        public CtrlFiguras(List<long> figuras)
        {
            InitializeComponent();
            if (figuras.Count > 0)
            {
                LlenarCasillas(figuras);
            }
            else
            {
                LlenarCasillas();
            }
        }
        protected void LlenarCasillas()
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i <= 20; i++)
            {
                CtrlCasillaFigura ctrl = new CtrlCasillaFigura();
                ctrl.Location = new Point(x, y);
                
                y += ctrl.Height + 1;
                if (i == 20)
                {
                    ctrl.Casilla.Enter += Añadir_Enter;
                }
                contControl.Controls.Add(ctrl);
            }
        }
        protected void LlenarCasillas(List<long> figurasCondicion)
        {
            const int x = 0;
            int y = 0;
            for (int i = 0; i < figurasCondicion.Count; i++)
            {
                CtrlCasillaFigura ctrl = new CtrlCasillaFigura();
                ctrl.Location = new Point(x, y);
                ctrl.Casilla.Text = ObtenerTextoFromLong( figurasCondicion[i]);
                contControl.Controls.Add(ctrl);
                y += ctrl.Height + 1;
            }
            //Añadir hasta 20
            for (int i = 0; i <= 20 - figurasCondicion.Count; i++)
            {
                CtrlCasillaFigura ctrl = new CtrlCasillaFigura();
                ctrl.Location = new Point(x, y);
                if (i == 20 - figurasCondicion.Count)
                {
                    ctrl.Casilla.Enter += Añadir_Enter;
                }
                contControl.Controls.Add(ctrl);
                y += ctrl.Height + 1;
            }
        }
        protected string ObtenerTextoFromLong(long fig)
        {
            return Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(fig);
        }
        private void Añadir_Enter(object sender, EventArgs e)
        {
            //Obtener el último control, su número y su posicion
            CtrlCasillaFigura ctrl = (CtrlCasillaFigura)contControl.Controls[contControl.Controls.Count - 1];

            int posicionY = ctrl.Location.Y;
            //Eliminar el evento en el último control
            ctrl.Casilla.Enter -= Añadir_Enter;
            CtrlCasillaFigura ctrl2 = new CtrlCasillaFigura();
            AñadirControl(ctrl2, 0, posicionY + ctrl2.Height + 1);
        }
        private void AñadirControl(CtrlCasillaFigura ctrl, int posicionX, int posicionY)
        {
            ctrl.Location = new Point(posicionX, posicionY);

            ctrl.Casilla.Enter += Añadir_Enter;

            int indicePre = contControl.Controls.Count - 1;
            if (indicePre >= 0)
            {
                CtrlCasillaFigura ctrl2 = (CtrlCasillaFigura)contControl.Controls[indicePre];
                ctrl2.Casilla.Enter -= Añadir_Enter;
            }
            contControl.Controls.Add(ctrl);

        }

    }
}
