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
using System.IO;

using Free1X2.MotorCalculo;
using Free1X2.UI.Controls;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    public class FigurasFiltrosFrm : Form
    {
        List<long> figurasCondicion;
        protected string[] valoresPermitidos = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "*" };

        private Free1X2.UI.Controls.CtrlFiguras ctrlFiguras1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancelar;
        private Button btnAbrir;
        private System.Windows.Forms.Button btnBorrar;
        IFiltro _filtro;
        public FigurasFiltrosFrm(List<long> figuras, int longitudFigura, IFiltro filtro)
        {
            this._filtro = filtro;
            if (figuras != null)
            {
                if (figuras.Count > 0)
                {
                    this.ctrlFiguras1 = new CtrlFiguras(figuras);
                }
                else
                {
                    this.ctrlFiguras1 = new CtrlFiguras();
                }
            }
            else
            {
                this.ctrlFiguras1 = new CtrlFiguras();
            }
            ctrlFiguras1.Location = new Point(0, 2);
            this.Controls.Add(ctrlFiguras1);
            InitializeComponent();
            this.figurasCondicion = figuras;
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

        protected void ObtenerFiguras()
        {
            this.figurasCondicion.Clear();
            for (int i = 0; i < this.ctrlFiguras1.contControl.Controls.Count; i++)
            {
                CtrlCasillaFigura casilla = (CtrlCasillaFigura)this.ctrlFiguras1.contControl.Controls[i];
                long figura;
                if (casilla.Casilla.Text != "")
                {
                    figura = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(casilla.Casilla.Text);
                    if (EsFiguraValida(figura) && (!this.figurasCondicion.Contains(figura)))
                    {
                        this.figurasCondicion.Add(figura);
                    }
                }

            }
        }
        public bool EsFiguraValida(long figura)
        {
            long figuraTemp = figura;
            bool esValida = true;
            while (figuraTemp != 0)
            {
                int valor = (int)figura & 15;
                if (Array.IndexOf(valoresPermitidos, valor.ToString()) == -1)
                {
                    esValida = false;
                    break;
                }
                figuraTemp >>= 4;
            }

            return esValida;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ObtenerFiguras();
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            BorrarTodo();
        }
        protected void BorrarTodo()
        {
            for (int i = 0; i < this.ctrlFiguras1.contControl.Controls.Count; i++)
            {
                CtrlCasillaFigura ctrl = (CtrlCasillaFigura) this.ctrlFiguras1.contControl.Controls[i];
                ctrl.Casilla.Text = "";
                this.figurasCondicion.Clear();
            }
        }
        protected void BorrarCasillas()
        {
            for (int i = 0; i < this.ctrlFiguras1.contControl.Controls.Count; i++)
            {
                CtrlCasillaFigura ctrl = (CtrlCasillaFigura)this.ctrlFiguras1.contControl.Controls[i];
                ctrl.Casilla.Text = "";
            }
        }

        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FigurasFiltrosFrm));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnBorrar = new System.Windows.Forms.Button();
            this.btnAbrir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.LightSalmon;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.Location = new System.Drawing.Point(26, 246);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(24, 20);
            this.btnOk.TabIndex = 1;
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.Location = new System.Drawing.Point(52, 246);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(24, 20);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnBorrar
            // 
            this.btnBorrar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnBorrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBorrar.Image = ((System.Drawing.Image)(resources.GetObject("btnBorrar.Image")));
            this.btnBorrar.Location = new System.Drawing.Point(104, 246);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(24, 20);
            this.btnBorrar.TabIndex = 3;
            this.btnBorrar.UseVisualStyleBackColor = false;
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // btnAbrir
            // 
            this.btnAbrir.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbrir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbrir.Image = ((System.Drawing.Image)(resources.GetObject("btnAbrir.Image")));
            this.btnAbrir.Location = new System.Drawing.Point(78, 246);
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.Size = new System.Drawing.Size(24, 20);
            this.btnAbrir.TabIndex = 4;
            this.btnAbrir.UseVisualStyleBackColor = false;
            this.btnAbrir.Click += new System.EventHandler(this.btnAbrir_Click);
            // 
            // FigurasFiltrosFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(165, 275);
            this.Controls.Add(this.btnAbrir);
            this.Controls.Add(this.btnBorrar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FigurasFiltrosFrm";
            this.ShowInTaskbar = false;
            this.Text = "Figuras";
            this.ResumeLayout(false);

        }

        #endregion
        protected string DeterminarCondicion()
        {
            string condicion = "";
            switch (_filtro.NombreFiltro)
            {
                case Filtro.Contactos:
                    condicion = "Contactos";
                    break;
                case Filtro.SignosSeguidos:
                    condicion = "V1X2";
                    break;
            }
            return condicion;
        }
        private void btnAbrir_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Application.StartupPath + "/Condiciones";
            string condicion = DeterminarCondicion();
            string extension = "(*.fig" + condicion + ")|*.fig" + condicion + "|Todos los archivos (*.*)|*.*";

            openFile.Filter = "Figuras de " + condicion + extension;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                this.figurasCondicion.Clear();

                string nombre = openFile.FileName;
                StreamReader sr = new StreamReader(nombre);
                while (sr.Peek() != -1)
                {
                    long figuraTemp;
                    figuraTemp = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(sr.ReadLine());
                    figurasCondicion.Add(figuraTemp);
                }
                sr.Close();

            }
            BorrarCasillas();
            this.Controls.Remove(this.ctrlFiguras1);
            this.ctrlFiguras1 = new CtrlFiguras(this.figurasCondicion);
            this.Controls.Add(ctrlFiguras1);
        }


    }
}
