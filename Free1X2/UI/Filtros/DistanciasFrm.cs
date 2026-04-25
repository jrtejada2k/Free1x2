// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Joan Duatis - duatis@coac.net
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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for DistanciasFrm.
    /// </summary>
    public class DistanciasFrm : Form
    {
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components;
        private Controls.OptionNumTol0_14 stdVar;
        private Controls.OptionNumTol0_14 std1;
        private Controls.OptionNumTol0_14 stdX;
        private Controls.OptionNumTol0_14 std2;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        private Grupo grupo;
        private FiltroDistancias filtro;
        private Controls.ctrlAyuda ctrlAyuda1;
        private MainForm parentFrm;
        FormulariosHelper formHelper = new FormulariosHelper();

        public DistanciasFrm(Grupo grupo, MainForm frm)
        {
            InitializeComponent();

            this.grupo = grupo;
            parentFrm=frm;
            string nombreFiltro = Filtro.Distancias.ToString();
            filtro = (FiltroDistancias)grupo.GetFiltro( nombreFiltro );
            MarcarValores();
            formHelper.Redimensionar(this);
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Distancia: número máximo de partidos\n que separan dos signos iguales";
            ctrlAyuda1.Location = new Point(Size.Width - (ctrlAyuda1.Width + 15), ctrlAyuda1.Location.Y);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        public MainForm FormPadre
        {
            get{ return parentFrm; }
        }

        protected void MarcarValores()
        {
            stdVar.Valores = filtro.GetIntVar();
            std1.Valores = filtro.GetInt1();
            stdX.Valores = filtro.GetIntX();
            std2.Valores = filtro.GetInt2();
        }

        protected void borrarValores()
        {
            stdVar.Valores = "";
            std1.Valores = "";
            stdX.Valores = "";
            std2.Valores = "";
        }

        protected bool NecesitaGuardarDatos()
        {
            bool necesitaGuardar = true;
			
            if(	stdVar.Valores == "" && 
               	std1.Valores == "" && 
               	stdX.Valores == "" && 
               	std2.Valores == "")
            {
                necesitaGuardar = false;
            }		
		
            return necesitaGuardar;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.std2 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdX = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std1 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdVar = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Var";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "1";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "X";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "2";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 120);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(643, 48);
            this.panel1.TabIndex = 37;
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(620, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 38;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // menuCondiciones1
            // 
            this.menuCondiciones1.Alineacion = Free1X2.alignment.Horizontal;
            this.menuCondiciones1.AutoSize = true;
            this.menuCondiciones1.BackColor = System.Drawing.Color.Bisque;
            this.menuCondiciones1.BotonAbrir = true;
            this.menuCondiciones1.BotonAbrirEnabled = true;
            this.menuCondiciones1.BotonBorrar = true;
            this.menuCondiciones1.BotonBorrarEnabled = true;
            this.menuCondiciones1.BotonCancelar = true;
            this.menuCondiciones1.BotonCancelarEnabled = true;
            this.menuCondiciones1.BotonCopiar = true;
            this.menuCondiciones1.BotonCopiarEnabled = true;
            this.menuCondiciones1.BotonEstadisticas = true;
            this.menuCondiciones1.BotonEstadisticasEnabled = true;
            this.menuCondiciones1.BotonGuardar = true;
            this.menuCondiciones1.BotonGuardarEnabled = true;
            this.menuCondiciones1.BotonOk = true;
            this.menuCondiciones1.BotonOkEnabled = true;
            this.menuCondiciones1.BotonPegar = true;
            this.menuCondiciones1.BotonPegarEnabled = true;
            this.menuCondiciones1.Location = new System.Drawing.Point(280, 8);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 1;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // std2
            // 
            this.std2.BackColor = System.Drawing.Color.Wheat;
            this.std2.Location = new System.Drawing.Point(46, 60);
            this.std2.Maximo = 15;
            this.std2.Minimo = 0;
            this.std2.Name = "std2";
            this.std2.Size = new System.Drawing.Size(560, 16);
            this.std2.TabIndex = 11;
            this.std2.Valores = "";
            // 
            // stdX
            // 
            this.stdX.BackColor = System.Drawing.Color.Wheat;
            this.stdX.Location = new System.Drawing.Point(46, 43);
            this.stdX.Maximo = 15;
            this.stdX.Minimo = 0;
            this.stdX.Name = "stdX";
            this.stdX.Size = new System.Drawing.Size(560, 16);
            this.stdX.TabIndex = 10;
            this.stdX.Valores = "";
            // 
            // std1
            // 
            this.std1.BackColor = System.Drawing.Color.Wheat;
            this.std1.Location = new System.Drawing.Point(46, 26);
            this.std1.Maximo = 15;
            this.std1.Minimo = 0;
            this.std1.Name = "std1";
            this.std1.Size = new System.Drawing.Size(560, 16);
            this.std1.TabIndex = 9;
            this.std1.Valores = "";
            // 
            // stdVar
            // 
            this.stdVar.BackColor = System.Drawing.Color.Wheat;
            this.stdVar.Location = new System.Drawing.Point(46, 9);
            this.stdVar.Maximo = 15;
            this.stdVar.Minimo = 0;
            this.stdVar.Name = "stdVar";
            this.stdVar.Size = new System.Drawing.Size(560, 16);
            this.stdVar.TabIndex = 8;
            this.stdVar.Valores = "";
            // 
            // DistanciasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(643, 168);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.std2);
            this.Controls.Add(this.stdX);
            this.Controls.Add(this.std1);
            this.Controls.Add(this.stdVar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(552, 176);
            this.Name = "DistanciasFrm";
            this.Text = "Distancias";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void guardarValores()
        {
            string todosValores = formHelper.ObtenerTodosValores();

            filtro.ReinicializaValores();
            if( NecesitaGuardarDatos() )
            {
                if(filtro.ContieneDatos == false)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtro.IsActive = true;				
                }
				
                filtro.ContieneDatos = true;
                if( stdVar.Valores != "" )
                {
                    filtro.SetNoIntVar( stdVar.Valores );
                }
                else
                {
                    filtro.SetNoIntVar( todosValores );
                }
				
                if( std1.Valores != "" )
                {
                    filtro.SetNoInt1( std1.Valores );
                }
                else
                {
                    filtro.SetNoInt1( todosValores );
                }
				
                if( stdX.Valores != "" )
                {
                    filtro.SetNoIntX( stdX.Valores );
                }
                else
                {
                    filtro.SetNoIntX( todosValores );
                }
				
                if( std2.Valores != "" )
                {
                    filtro.SetNoInt2( std2.Valores );
                }
                else
                {
                    filtro.SetNoInt2( todosValores );
                }
            }
            else
            {
                filtro.IsActive = false;
                filtro.ContieneDatos = false;
            }
        }
        private FiltroDistancias ObtenerFiltroTemporal ()
        {
            FiltroDistancias filtroTemp = new FiltroDistancias();
            string todosValores = formHelper.ObtenerTodosValores();

            filtroTemp.ReinicializaValores();
            if (NecesitaGuardarDatos())
            {
                if (filtroTemp.ContieneDatos == false)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtroTemp.IsActive = true;
                }

                filtroTemp.ContieneDatos = true;
                if (stdVar.Valores != "")
                {
                    filtroTemp.SetNoIntVar(stdVar.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntVar(todosValores);
                }

                if (std1.Valores != "")
                {
                    filtroTemp.SetNoInt1(std1.Valores);
                }
                else
                {
                    filtroTemp.SetNoInt1(todosValores);
                }

                if (stdX.Valores != "")
                {
                    filtroTemp.SetNoIntX(stdX.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntX(todosValores);
                }

                if (std2.Valores != "")
                {
                    filtroTemp.SetNoInt2(std2.Valores);
                }
                else
                {
                    filtroTemp.SetNoInt2(todosValores);
                }
            }
            else
            {
                filtroTemp.IsActive = false;
                filtroTemp.ContieneDatos = false;
            }
            return filtroTemp;
        }

        private void menuCondiciones1_BOk(object sender, System.EventArgs e)
        {
            guardarValores();
            Grupo g=new Grupo();
            g.Filtros.Clear();
            g.Filtros.Add(filtro);
            this.grupo.ActivaFiltro(filtro);
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, System.EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, System.EventArgs e)
        {
            guardarValores();
            MarcarValores();
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Distancias(*.dist)|*.dist|Distancias(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            guardarValores();
            MarcarValores();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Distancias(*.dist)|*.dist|Distancias(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo )==true)
            {
                grupo=archComb.LeeCondicion();
                filtro=(FiltroDistancias)grupo.GetFiltro("Distancias");
                MarcarValores();
                guardarValores();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            if(filtro.NoDistancias>0)
            {
                filtro.ContieneDatos=true;
                filtro.IsActive=true;
            }
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, System.EventArgs e)
        {
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            borrarValores();
        }

        private void menuCondiciones1_BCopiar(object sender, System.EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            guardarValores();
            MarcarValores();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.dist";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, System.EventArgs e)
        {
            guardarValores();
            MarcarValores();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.dist";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.dist"))
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }
        private void CerrarVentana()
        {
            Close();
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroDistancias filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
