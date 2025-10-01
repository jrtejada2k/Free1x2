// created on 13/09/2003 at 12:23
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
// 
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
using System.Collections.Generic;

using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;

namespace Free1X2.UI.Filtros
{
    public class DibujosFrm : Form
    {
        private Button btnDesmarcarTodos;
        private Button btnMarcarTodos;
        private GroupBox groupBox;
        private Controls.GridDibujosCentral gridDibujosCentral;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        private MainForm parentFrm;
        private Controls.ctrlAyuda ctrlAyuda1;
        private FormulariosHelper formHelper = new FormulariosHelper();
        private Grupo grupo;
		
        public DibujosFrm( Grupo grupo, MainForm form )
        {
            InitializeComponent();
            this.grupo = grupo;
            MarcarValores();
            parentFrm = form;
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Un dibujo es la figura formada por el número\nde X y el número de 2, por ejemplo:\n2 equis y 3 doses forman el dibujo 2+3";
            formHelper.Redimensionar(this);
        }
		
        public MainForm FormPadre
        {
            get{ return parentFrm; }
        }

        protected void MarcarValores()
        {
            string nombreFiltro = Filtro.Dibujos.ToString();
            FiltroDibujos filtro = (FiltroDibujos)grupo.GetFiltro( nombreFiltro );
            gridDibujosCentral.Dibujos = filtro.Dibujos;		
        }
				
		
				
		
        void BtnMarcarTodosClick(object sender, System.EventArgs e)
        {
            gridDibujosCentral.SeleccionaTodos();
        }			
		
        void BtnDesmarcarTodosClick(object sender, System.EventArgs e)
        {
            gridDibujosCentral.DeseleccionaTodos();
        }
			
        void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DibujosFrm));
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.btnDesmarcarTodos = new System.Windows.Forms.Button();
            this.btnMarcarTodos = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.gridDibujosCentral = new Free1X2.UI.Controls.GridDibujosCentral();
            this.groupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.BackColor = System.Drawing.Color.Transparent;
            this.groupBox.Controls.Add(this.btnDesmarcarTodos);
            this.groupBox.Controls.Add(this.btnMarcarTodos);
            this.groupBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox.Location = new System.Drawing.Point(445, 193);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(175, 96);
            this.groupBox.TabIndex = 4;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Marcar";
            // 
            // btnDesmarcarTodos
            // 
            this.btnDesmarcarTodos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnDesmarcarTodos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDesmarcarTodos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDesmarcarTodos.ForeColor = System.Drawing.Color.Black;
            this.btnDesmarcarTodos.Image = ((System.Drawing.Image)(resources.GetObject("btnDesmarcarTodos.Image")));
            this.btnDesmarcarTodos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDesmarcarTodos.Location = new System.Drawing.Point(20, 56);
            this.btnDesmarcarTodos.Name = "btnDesmarcarTodos";
            this.btnDesmarcarTodos.Size = new System.Drawing.Size(134, 23);
            this.btnDesmarcarTodos.TabIndex = 1;
            this.btnDesmarcarTodos.Text = "Desmarcar Todos";
            this.btnDesmarcarTodos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDesmarcarTodos.UseVisualStyleBackColor = false;
            this.btnDesmarcarTodos.Click += new System.EventHandler(this.BtnDesmarcarTodosClick);
            // 
            // btnMarcarTodos
            // 
            this.btnMarcarTodos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnMarcarTodos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnMarcarTodos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMarcarTodos.ForeColor = System.Drawing.Color.Black;
            this.btnMarcarTodos.Image = ((System.Drawing.Image)(resources.GetObject("btnMarcarTodos.Image")));
            this.btnMarcarTodos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMarcarTodos.Location = new System.Drawing.Point(20, 24);
            this.btnMarcarTodos.Name = "btnMarcarTodos";
            this.btnMarcarTodos.Size = new System.Drawing.Size(134, 23);
            this.btnMarcarTodos.TabIndex = 0;
            this.btnMarcarTodos.Text = "Marcar Todos";
            this.btnMarcarTodos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMarcarTodos.UseVisualStyleBackColor = false;
            this.btnMarcarTodos.Click += new System.EventHandler(this.BtnMarcarTodosClick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 310);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(627, 48);
            this.panel1.TabIndex = 36;
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlAyuda1.Location = new System.Drawing.Point(605, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 37;
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
            this.menuCondiciones1.Location = new System.Drawing.Point(228, 8);
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
            // gridDibujosCentral
            // 
            this.gridDibujosCentral.AutoSize = true;
            this.gridDibujosCentral.BackColor = System.Drawing.Color.Bisque;
            this.gridDibujosCentral.Dibujos = ((System.Collections.ArrayList)(resources.GetObject("gridDibujosCentral.Dibujos")));
            this.gridDibujosCentral.Location = new System.Drawing.Point(1, 2);
            this.gridDibujosCentral.Name = "gridDibujosCentral";
            this.gridDibujosCentral.Size = new System.Drawing.Size(626, 300);
            this.gridDibujosCentral.TabIndex = 0;
            // 
            // DibujosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(627, 358);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.gridDibujosCentral);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DibujosFrm";
            this.Text = "Dibujos X+2";
            this.groupBox.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private FiltroDibujos ObtenerFiltroTemporal()
        {
            FiltroDibujos filtroTemp = new FiltroDibujos();

            if (gridDibujosCentral.Dibujos.Count > 0)
            {
                if (filtroTemp.ContieneDatos == false)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtroTemp.ContieneDatos = true;
                }
                filtroTemp.IsActive = true;
                filtroTemp.Dibujos = gridDibujosCentral.Dibujos;
            }
            else
            {
                filtroTemp.IsActive = false;
                filtroTemp.ContieneDatos = false;
            }
            return filtroTemp;
        }
        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            string nombreFiltro = Filtro.Dibujos.ToString();
            FiltroDibujos filtro = (FiltroDibujos)grupo.GetFiltro( nombreFiltro );
				
            if( gridDibujosCentral.Dibujos.Count > 0 )
            {
                if(!filtro.ContieneDatos)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtro.ContieneDatos = true;
                }
                filtro.IsActive = true;
                filtro.Dibujos = gridDibujosCentral.Dibujos;
            }
            else
            {
                filtro.IsActive = false;
                filtro.ContieneDatos = false;			
            }
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            gridDibujosCentral.GetValores();
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            MarcarValores();
            if(gridDibujosCentral.Dibujos.Count>0)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Dibujos(*.dbj)|*.dbj|Dibujos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            gridDibujosCentral.GetValores();
            MarcarValores();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Dibujos(*.dbj)|*.dbj|Dibujos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo ))
            {
                //Borrar Dibujos antes de pegar
                gridDibujosCentral.DeseleccionaTodos();

                grupo=archComb.LeeCondicion();
                gridDibujosCentral.GetValores();
                MarcarValores();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            FiltroDibujos filtro=(FiltroDibujos)grupo.GetFiltro("Dibujos");
            if(filtro.Dibujos.Count>0)
            {
                filtro.ContieneDatos=true;
                filtro.IsActive=true;
            }
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                return;
            Grupo g=new Grupo();
            grupo.ActivaFiltro(g,"Dibujos",true);
            gridDibujosCentral.DeseleccionaTodos();
            MarcarValores();
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            gridDibujosCentral.GetValores();
            MarcarValores();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.dbj";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            if(gridDibujosCentral.Dibujos.Count>0)
            {
                if (MessageBox.Show("El filtro ya tiene datos introducidos. ¿Pegar igualmente?", "Pegar condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.dbj";
            abrir(nombreFichero);		
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable

            if (formHelper.ExisteFicheroTemporal("tmp.dbj"))
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
            FiltroDibujos filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
