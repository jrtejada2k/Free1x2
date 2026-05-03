// created on 03/09/2003 at 22:24
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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
using System.Drawing;

using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    public class SignosSeguidosFrm : Form
    {
        private Label label;
        private Controls.OptionNumTol0_14 stdVar;
        private Controls.OptionNumTol0_14 std2;
        private Label label3;
        private Label label2;
        private Controls.OptionNumTol0_14 stdX;
        private Label label4;
        private Controls.OptionNumTol0_14 std1;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        FiltroSignosSeguidos filtro;
        private Controls.ctrlAyuda ctrlAyuda1;
        private Button btnFigurasV;
        private Button btnFiguras1;
        private Button btnFigurasX;
        private Button btnFiguras2;
        private Grupo grupo;
        protected List<long> figurasV;
        protected List<long> figuras1;
        protected List<long> figurasX;
        protected List<long> figuras2;
        private FormulariosHelper formHelper = new FormulariosHelper();

        public SignosSeguidosFrm( Grupo grupo )
        {
            InitializeComponent();
            this.grupo = grupo;
            string nombreFiltro = Filtro.SignosSeguidos.ToString();
            filtro = (FiltroSignosSeguidos)grupo.GetFiltro( nombreFiltro );
            MarcarValores();
            compruebaPegar();
            formHelper.Redimensionar(this);
            ctrlAyuda1.TextoAyuda = "Especificar la cantidad de Variantes\nseguidas, 1 seguidos, X seguidas\ny 2 seguidos";
            ctrlAyuda1.Location = new Point(Size.Width - (ctrlAyuda1.Width + 15), ctrlAyuda1.Location.Y);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
        protected void MarcarValores()
        {
            stdVar.Valores = filtro.GetVariantes();
            std1.Valores = filtro.GetUnos();
            stdX.Valores = filtro.GetEquis();
            std2.Valores = filtro.GetDoses();

            this.figurasV = filtro.FigurasV;
            this.figuras1 = filtro.Figuras1;
            this.figurasX = filtro.FigurasX;
            this.figuras2 = filtro.Figuras2;
            IndicarCondicionFiguras();
        }
		
        protected bool NecesitaGuardarDatos()
        {
            bool necesitaGuardar = true;
			
            if(stdVar.Valores == "" && std1.Valores == "" && stdX.Valores == "" && std2.Valores == "" )
            {
                necesitaGuardar = false;
            }		
		
            return necesitaGuardar;
        }
					
		
		
		
        void InitializeComponent() {
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.btnFigurasV = new System.Windows.Forms.Button();
            this.btnFiguras1 = new System.Windows.Forms.Button();
            this.btnFigurasX = new System.Windows.Forms.Button();
            this.btnFiguras2 = new System.Windows.Forms.Button();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.std2 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdX = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std1 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdVar = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "2";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "1";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "X";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(16, 16);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(32, 16);
            this.label.TabIndex = 0;
            this.label.Text = "Var";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 130);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(616, 48);
            this.panel1.TabIndex = 37;
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
            this.menuCondiciones1.BotonPegarEnabled = false;
            this.menuCondiciones1.Location = new System.Drawing.Point(288, 8);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 3;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // btnFigurasV
            // 
            this.btnFigurasV.BackColor = System.Drawing.Color.Wheat;
            this.btnFigurasV.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFigurasV.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFigurasV.Location = new System.Drawing.Point(67, 94);
            this.btnFigurasV.Name = "btnFigurasV";
            this.btnFigurasV.Size = new System.Drawing.Size(120, 23);
            this.btnFigurasV.TabIndex = 39;
            this.btnFigurasV.Text = "Figuras Variantes";
            this.btnFigurasV.UseVisualStyleBackColor = false;
            this.btnFigurasV.Click += new System.EventHandler(this.btnFigurasV_Click);
            // 
            // btnFiguras1
            // 
            this.btnFiguras1.BackColor = System.Drawing.Color.Wheat;
            this.btnFiguras1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFiguras1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFiguras1.Location = new System.Drawing.Point(188, 94);
            this.btnFiguras1.Name = "btnFiguras1";
            this.btnFiguras1.Size = new System.Drawing.Size(120, 23);
            this.btnFiguras1.TabIndex = 40;
            this.btnFiguras1.Text = "Figuras Unos";
            this.btnFiguras1.UseVisualStyleBackColor = false;
            this.btnFiguras1.Click += new System.EventHandler(this.btnFiguras1_Click);
            // 
            // btnFigurasX
            // 
            this.btnFigurasX.BackColor = System.Drawing.Color.Wheat;
            this.btnFigurasX.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFigurasX.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFigurasX.Location = new System.Drawing.Point(309, 94);
            this.btnFigurasX.Name = "btnFigurasX";
            this.btnFigurasX.Size = new System.Drawing.Size(120, 23);
            this.btnFigurasX.TabIndex = 41;
            this.btnFigurasX.Text = "Figuras Equis";
            this.btnFigurasX.UseVisualStyleBackColor = false;
            this.btnFigurasX.Click += new System.EventHandler(this.btnFigurasX_Click);
            // 
            // btnFiguras2
            // 
            this.btnFiguras2.BackColor = System.Drawing.Color.Wheat;
            this.btnFiguras2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFiguras2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFiguras2.Location = new System.Drawing.Point(430, 94);
            this.btnFiguras2.Name = "btnFiguras2";
            this.btnFiguras2.Size = new System.Drawing.Size(120, 23);
            this.btnFiguras2.TabIndex = 42;
            this.btnFiguras2.Text = "Figuras Doses";
            this.btnFiguras2.UseVisualStyleBackColor = false;
            this.btnFiguras2.Click += new System.EventHandler(this.btnFiguras2_Click);
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlAyuda1.Location = new System.Drawing.Point(588, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 38;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // std2
            // 
            this.std2.BackColor = System.Drawing.Color.Wheat;
            this.std2.Location = new System.Drawing.Point(56, 67);
            this.std2.Maximo = 15;
            this.std2.Minimo = 0;
            this.std2.Name = "std2";
            this.std2.Size = new System.Drawing.Size(563, 16);
            this.std2.TabIndex = 12;
            this.std2.Valores = "";
            // 
            // stdX
            // 
            this.stdX.BackColor = System.Drawing.Color.Wheat;
            this.stdX.Location = new System.Drawing.Point(56, 50);
            this.stdX.Maximo = 15;
            this.stdX.Minimo = 0;
            this.stdX.Name = "stdX";
            this.stdX.Size = new System.Drawing.Size(563, 16);
            this.stdX.TabIndex = 11;
            this.stdX.Valores = "";
            // 
            // std1
            // 
            this.std1.BackColor = System.Drawing.Color.Wheat;
            this.std1.Location = new System.Drawing.Point(56, 33);
            this.std1.Maximo = 15;
            this.std1.Minimo = 0;
            this.std1.Name = "std1";
            this.std1.Size = new System.Drawing.Size(563, 16);
            this.std1.TabIndex = 10;
            this.std1.Valores = "";
            // 
            // stdVar
            // 
            this.stdVar.BackColor = System.Drawing.Color.Wheat;
            this.stdVar.Location = new System.Drawing.Point(56, 16);
            this.stdVar.Maximo = 15;
            this.stdVar.Minimo = 0;
            this.stdVar.Name = "stdVar";
            this.stdVar.Size = new System.Drawing.Size(563, 16);
            this.stdVar.TabIndex = 9;
            this.stdVar.Valores = "";
            // 
            // SignosSeguidosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(616, 178);
            this.ControlBox = false;
            this.Controls.Add(this.btnFiguras2);
            this.Controls.Add(this.btnFigurasX);
            this.Controls.Add(this.btnFiguras1);
            this.Controls.Add(this.btnFigurasV);
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.std2);
            this.Controls.Add(this.stdX);
            this.Controls.Add(this.std1);
            this.Controls.Add(this.stdVar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SignosSeguidosFrm";
            this.Text = "Signos Seguidos";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        protected void ActualizarDatos()
        {
            string todosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";
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
                    filtro.SetNoVariantes( stdVar.Valores );
                }
                else
                {
                    filtro.SetNoVariantes( todosValores );
                }
				
                if( std1.Valores != "" )
                {
                    filtro.SetNoUnos( std1.Valores );
                }
                else
                {
                    filtro.SetNoUnos( todosValores );
                }
				
                if( stdX.Valores != "" )
                {
                    filtro.SetNoEquis( stdX.Valores );
                }
                else
                {
                    filtro.SetNoEquis( todosValores );
                }
				
                if( std2.Valores != "" )
                {
                    filtro.SetNoDoses( std2.Valores );
                }
                else
                {
                    filtro.SetNoDoses( todosValores );
                }

                if (this.figurasV != null)
                {
                    if (this.figurasV.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtro.FigurasV = this.figurasV;
                    }
                }
                if (this.figuras1 != null)
                {
                    if (this.figuras1.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtro.Figuras1 = this.figuras1;
                    }
                }
                if (this.figurasX != null)
                {
                    if (this.figurasX.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtro.FigurasX = this.figurasX;
                    }
                }
                if (this.figuras2 != null)
                {
                    if (this.figuras2.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtro.Figuras2 = this.figuras2;
                    }
                }
            }
            else
            {
                filtro.IsActive = false;
                filtro.ContieneDatos = false;
            }
        }
        protected FiltroSignosSeguidos ObtenerFiltroTemporal()
        {
            FiltroSignosSeguidos filtroTemp = new FiltroSignosSeguidos();
            string todosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";
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
                    filtroTemp.SetNoVariantes(stdVar.Valores);
                }
                else
                {
                    filtroTemp.SetNoVariantes(todosValores);
                }

                if (std1.Valores != "")
                {
                    filtroTemp.SetNoUnos(std1.Valores);
                }
                else
                {
                    filtroTemp.SetNoUnos(todosValores);
                }

                if (stdX.Valores != "")
                {
                    filtroTemp.SetNoEquis(stdX.Valores);
                }
                else
                {
                    filtroTemp.SetNoEquis(todosValores);
                }

                if (std2.Valores != "")
                {
                    filtroTemp.SetNoDoses(std2.Valores);
                }
                else
                {
                    filtroTemp.SetNoDoses(todosValores);
                }

                if (this.figurasV != null)
                {
                    if (this.figurasV.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtroTemp.FigurasV = this.figurasV;
                    }
                }
                if (this.figuras1 != null)
                {
                    if (this.figuras1.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtroTemp.Figuras1 = this.figuras1;
                    }
                }
                if (this.figurasX != null)
                {
                    if (this.figurasX.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtroTemp.FigurasX = this.figurasX;
                    }
                }
                if (this.figuras2 != null)
                {
                    if (this.figuras2.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtroTemp.Figuras2 = this.figuras2;
                    }
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
            ActualizarDatos();			
            //cerrar ventana
            grupo.ActivaFiltro(filtro);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, System.EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, System.EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Signos seguidos(*.seg)|*.seg|Signos seguidos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, System.EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Signos seguidos(*.seg)|*.seg|Signos seguidos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo )==true)
            {
                Grupo g=archComb.LeeCondicion();
                filtro=(FiltroSignosSeguidos)g.GetFiltro("SignosSeguidos");
                MarcarValores();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, System.EventArgs e)
        {
            ActualizarDatos();
            filtro=(FiltroSignosSeguidos)grupo.GetFiltro("SignosSeguidos");
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroSignosSeguidos();
            MarcarValores();
        }

        private void menuCondiciones1_BCopiar(object sender, System.EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.seg";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, System.EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.seg";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable

            if(formHelper.ExisteFicheroTemporal("tmp.seg"))
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }
        private void CerrarVentana()
        {
            this.Close();
        }
        protected void IndicarCondicionFiguras()
        {
            if (this.figurasV != null)
            {
                if (this.figurasV.Count > 0)
                {
                    btnFigurasV.BackColor = Color.LightGreen;
                }
                else
                {
                    btnFigurasV.BackColor = Color.Wheat;
                }
            }
            else
            {
                btnFigurasV.BackColor = Color.Wheat;
            }

            if (this.figuras1 != null)
            {
                if (this.figuras1.Count > 0)
                {
                    btnFiguras1.BackColor = Color.LightGreen;
                }
                else
                {
                    btnFiguras1.BackColor = Color.Wheat;
                }
            }
            else
            {
                btnFiguras1.BackColor = Color.Wheat;
            }

            if (this.figurasX != null)
            {
                if (this.figurasX.Count > 0)
                {
                    btnFigurasX.BackColor = Color.LightGreen;
                }
                else
                {
                    btnFigurasX.BackColor = Color.Wheat;
                }
            }
            else
            {
                btnFigurasX.BackColor = Color.Wheat;
            }

            if (this.figuras2 != null)
            {
                if (this.figuras2.Count > 0)
                {
                    btnFiguras2.BackColor = Color.LightGreen;
                }
                else
                {
                    btnFiguras2.BackColor = Color.Wheat;
                }
            }
            else
            {
                btnFiguras2.BackColor = Color.Wheat;
            }
        }
        private void btnFigurasV_Click(object sender, EventArgs e)
        {
            if (this.figurasV == null)
            {
                this.figurasV = new List<long>();
            }
            FigurasFiltrosFrm figuras = new FigurasFiltrosFrm(this.figurasV, 10, new FiltroSignosSeguidos());
            figuras.ShowDialog();
            IndicarCondicionFiguras();
        }

        private void btnFiguras1_Click(object sender, EventArgs e)
        {
            if (this.figuras1 == null)
            {
                this.figuras1 = new List<long>();
            }
            FigurasFiltrosFrm figuras = new FigurasFiltrosFrm(this.figuras1, 10, new FiltroSignosSeguidos());
            figuras.ShowDialog();
            IndicarCondicionFiguras();
        }

        private void btnFigurasX_Click(object sender, EventArgs e)
        {
            if (this.figurasX == null)
            {
                this.figurasX = new List<long>();
            }
            FigurasFiltrosFrm figuras = new FigurasFiltrosFrm(this.figurasX, 10, new FiltroSignosSeguidos());
            figuras.ShowDialog();
            IndicarCondicionFiguras();
        }

        private void btnFiguras2_Click(object sender, EventArgs e)
        {
            if (this.figuras2 == null)
            {
                this.figuras2 = new List<long>();
            }
            FigurasFiltrosFrm figuras = new FigurasFiltrosFrm(this.figuras2, 10, new FiltroSignosSeguidos());
            figuras.ShowDialog();
            IndicarCondicionFiguras();
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroSignosSeguidos filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Free1X2.UI.Estadisticas.VisorEstadisticas visor = new Free1X2.UI.Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
