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
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
    public partial class ctrlAyuda : UserControl
    {
        protected string texto = "";
        public ctrlAyuda()
        {
            InitializeComponent();
        }
        public string TextoAyuda
        {
            get { return texto; }
            set { texto = value; }
        }

        private void pctAyuda_MouseHover(object sender, EventArgs e)
        {
            if (TextoAyuda != "")
            {
                toolTip1.SetToolTip(pctAyuda, TextoAyuda);
                toolTip1.Active = true;
            }
            else
            {
                Visible = false;
                toolTip1.Active = false;
            }
        }

        private void ctrlAyuda_Load(object sender, EventArgs e)
        {
            if (TextoAyuda != "")
            {
                toolTip1.Active = true;
            }
            else
            {
                Visible = false;
                toolTip1.Active = false;
            }
        }

    }
}
