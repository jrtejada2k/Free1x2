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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Free1X2.EntradaSalida;

namespace Free1X2.UI
{
    public partial class ConfiguracionAnalisisFrm : Form
    {
        private bool analizarTodo;
        private bool analizarVX2;
        private bool analizarSeguidos;
        private bool analizarDibujos;
        private bool analizarInterrupciones;
        private bool analizarFormatos;
        private bool analizarFormatos123;
        private bool analizarContactos;
        private bool analizarFigurasContactos;
        private bool analizarFigurasV1X2;
        private bool analizarSimetrias;
        private bool analizarGruposEquipos;
        private bool analizarPesos;
        private bool analizarFigurasPesos;
        private bool analizarValoracion;
        private bool analizarDistancias;
        private bool analizarCPs;
        private bool analizarControlGrupos;
        private bool analizarControlConjuntos;
        private bool analizarDiferencias;
        public ConfiguracionAnalisisFrm()
        {
            InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        protected void MarcarTodo()
        {
            chkVX2.Checked = true;
            chkSeguidos.Checked = true;
            chkFigurasV1X2.Checked = true;
            chkDibujos.Checked = true;
            chkInterrupciones.Checked = true;
            chkValoracion.Checked = true;
            chkFormatos.Checked = true;
            chkContactos.Checked = true;
            chkFigContactos.Checked = true;
            chkFormatos123.Checked = true;
            chkSimetrias.Checked = true;
            chkDiferencias.Checked = true;
            chkDistancias.Checked = true;
            chkPesos.Checked = true;
            chkFigPesos.Checked = true;
            chkCPs.Checked = true;
            chkGruposEquipos.Checked = true;
            chkControlGrupos.Checked = true;
            chkControlConjuntos.Checked = true;
            chkNada.Checked = false;

        }
        protected void DesmarcarTodo()
        {
            chkVX2.Checked = false;
            chkSeguidos.Checked = false;
            chkFigurasV1X2.Checked = false;
            chkDibujos.Checked = false;
            chkInterrupciones.Checked = false;
            chkValoracion.Checked = false;
            chkFormatos.Checked = false;
            chkContactos.Checked = false;
            chkFigContactos.Checked = false;
            chkFormatos123.Checked = false;
            chkSimetrias.Checked = false;
            chkDiferencias.Checked = false;
            chkDistancias.Checked = false;
            chkPesos.Checked = false;
            chkFigPesos.Checked = false;
            chkCPs.Checked = false;
            chkGruposEquipos.Checked = false;
            chkControlGrupos.Checked = false;
            chkControlConjuntos.Checked = false;
        }
        private void chkTodo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTodo.Checked)
            {
                MarcarTodo();
            }

        }

        private void chkContactos_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkContactos.Checked)
            {
                chkFigContactos.Checked = false;
                chkFigContactos.Enabled = false;
                chkTodo.Checked = false;
            }
            else
            {
                chkFigContactos.Enabled = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            AConfiguracion ac = new AConfiguracion(Application.StartupPath);
            ac.GuardarConfiguracionAnalisis(chkTodo.Checked, chkVX2.Checked, chkSeguidos.Checked, chkFigurasV1X2.Checked, chkInterrupciones.Checked, chkDibujos.Checked, chkSimetrias.Checked, chkFormatos.Checked, chkFormatos123.Checked, chkDistancias.Checked, chkContactos.Checked, chkFigContactos.Checked, chkPesos.Checked, chkFigPesos.Checked, chkValoracion.Checked, chkCPs.Checked, chkGruposEquipos.Checked, chkControlGrupos.Checked, chkControlConjuntos.Checked, chkDiferencias.Checked);
            VariablesGlobales.ReinicializarVariables();
            this.Close();
        }

        private void ConfiguracionAnalisisFrm_Load(object sender, EventArgs e)
        {
            AConfiguracion ac = new AConfiguracion(Application.StartupPath);
            ac.ObtenConfiguracionAnalisis(ref analizarTodo, ref analizarVX2, ref analizarSeguidos, ref analizarFigurasV1X2, ref analizarInterrupciones, ref analizarDibujos, ref analizarSimetrias, ref analizarFormatos, ref analizarFormatos123, ref analizarDistancias, ref analizarContactos, ref analizarFigurasContactos, ref analizarPesos, ref analizarFigurasPesos, ref analizarValoracion, ref analizarCPs, ref analizarGruposEquipos, ref analizarControlGrupos, ref analizarControlConjuntos, ref analizarDiferencias);

            chkTodo.Checked = analizarTodo;
            chkVX2.Checked = analizarVX2;
            chkSeguidos.Checked = analizarSeguidos;
            chkFigurasV1X2.Checked = analizarFigurasV1X2;
            chkDibujos.Checked = analizarDibujos;
            chkInterrupciones.Checked = analizarInterrupciones;
            chkValoracion.Checked = analizarValoracion;
            chkFormatos.Checked = analizarFormatos;
            chkContactos.Checked = analizarContactos;
            chkFigContactos.Checked = analizarFigurasContactos;
            chkFormatos123.Checked = analizarFormatos123;
            chkSimetrias.Checked = analizarSimetrias;
            chkDiferencias.Checked = analizarDiferencias;
            chkDistancias.Checked = analizarDistancias;
            chkPesos.Checked = analizarPesos;
            chkCPs.Checked = analizarCPs;
            chkGruposEquipos.Checked = analizarGruposEquipos;
            chkControlGrupos.Checked = analizarControlGrupos;
            chkControlConjuntos.Checked = analizarControlConjuntos;
        }

        private void chkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;
            if (!chkBox.Checked)
            {
                this.chkTodo.Checked = false;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNada.Checked)
            {
                DesmarcarTodo();
            }
        }

        private void chkSeguidos_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkSeguidos.Checked)
            {
                chkFigurasV1X2.Checked = false;
                chkFigurasV1X2.Enabled = false;
                chkTodo.Checked = false;
            }
            else
            {
                chkFigurasV1X2.Enabled = true;
            }
        }

        private void chkPesos_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkPesos.Checked)
            {
                chkFigPesos.Checked = false;
                chkFigPesos.Enabled = false;
                chkTodo.Checked = false;
            }
            else
            {
                chkFigPesos.Enabled = true;
            }
        }
    }
}