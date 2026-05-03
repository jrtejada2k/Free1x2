// created on 02/09/2003 at 20:45
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
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
using System.Collections.Generic;
using System.Drawing;

using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo.Estadisticas;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    public class NoVariantesFrm : Form
    {
        private Label label3;
        private Label label2;
        private Controls.OptionNumTol0_14 std2;
        private Label label;
        private Controls.OptionNumTol0_14 stdVar;
        private Controls.OptionNumTol0_14 stdX;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        private FiltroNoVariantes filtro;
        private Controls.ctrlAyuda ctrlAyuda1;
        protected FormulariosHelper formHelper = new FormulariosHelper();
        private Grupo grupo;
		
        public NoVariantesFrm( Grupo grupo )
        {
            InitializeComponent();
            this.grupo = grupo;
            string nombreFiltro = Filtro.NoVariantes.ToString();
            filtro = (FiltroNoVariantes)grupo.GetFiltro( nombreFiltro );
            MarcarValores();
            compruebaPegar();
            formHelper.Redimensionar(this);
            ctrlAyuda1.TextoAyuda = "Especificar la cantidad de Variantes, X y 2";
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
            stdX.Valores = filtro.GetEquis();
            std2.Valores = filtro.GetDoses();			
        }
		
        protected bool NecesitaGuardarDatos()
        {
            bool necesitaGuardar = true;
			
            if(stdVar.Valores == "" && stdX.Valores == "" && std2.Valores == "" )
            {
                necesitaGuardar = false;
            }		
            return necesitaGuardar;		
        }
		
				
		
		
        void InitializeComponent() {
            this.label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.std2 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdX = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdVar = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(16, 9);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(32, 17);
            this.label.TabIndex = 5;
            this.label.Text = "Var";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "X";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "2";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(611, 48);
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
            this.menuCondiciones1.Location = new System.Drawing.Point(264, 8);
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
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(585, -1);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 38;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // std2
            // 
            this.std2.BackColor = System.Drawing.Color.Wheat;
            this.std2.Location = new System.Drawing.Point(48, 43);
            this.std2.Maximo = 15;
            this.std2.Minimo = 0;
            this.std2.Name = "std2";
            this.std2.Size = new System.Drawing.Size(563, 17);
            this.std2.TabIndex = 7;
            this.std2.Valores = "";
            // 
            // stdX
            // 
            this.stdX.BackColor = System.Drawing.Color.Wheat;
            this.stdX.Location = new System.Drawing.Point(48, 26);
            this.stdX.Maximo = 15;
            this.stdX.Minimo = 0;
            this.stdX.Name = "stdX";
            this.stdX.Size = new System.Drawing.Size(563, 17);
            this.stdX.TabIndex = 6;
            this.stdX.Valores = "";
            // 
            // stdVar
            // 
            this.stdVar.BackColor = System.Drawing.Color.Wheat;
            this.stdVar.Location = new System.Drawing.Point(48, 9);
            this.stdVar.Maximo = 15;
            this.stdVar.Minimo = 0;
            this.stdVar.Name = "stdVar";
            this.stdVar.Size = new System.Drawing.Size(563, 17);
            this.stdVar.TabIndex = 0;
            this.stdVar.Valores = "";
            // 
            // NoVariantesFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(611, 133);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.std2);
            this.Controls.Add(this.stdX);
            this.Controls.Add(this.label);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.stdVar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NoVariantesFrm";
            this.Text = "Número de Variantes";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        protected void ActualizarDatos()
        {
            string todosValores = Utils.UtilidadesEntradasValores.ObtenerTodosValores();
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
            }
            else
            {
                filtro.IsActive = false;
                filtro.ContieneDatos = false;
            }
        }
        protected FiltroNoVariantes ObtenerFiltroTemporal()
        {
            FiltroNoVariantes filtroTemp = new FiltroNoVariantes();
            string todosValores = Utils.UtilidadesEntradasValores.ObtenerTodosValores();
            filtroTemp.ReinicializaValores();

            if (NecesitaGuardarDatos())
            {
                if (filtroTemp.ContieneDatos == false)
                {
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
            ActualizarDatos();
            grupo.ActivaFiltro(filtro);
            //cerrar ventana
            CerrarVentana();		
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = Application.StartupPath + "/Condiciones/";
            abreCombDialog.Filter = "Cantidad de signos V, X y 2(*.vx2)|*.vx2|Cantidad de signos V, X y 2(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Application.StartupPath + "/Condiciones/";
            saveDialog.Filter = "Cantidad de signos V, X y 2(*.vx2)|*.vx2|Cantidad de signos V, X y 2(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo ))
            {
                Grupo g=archComb.LeeCondicion();
                filtro=(FiltroNoVariantes)g.GetFiltro("NoVariantes");
                MarcarValores();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            ActualizarDatos();
            filtro=(FiltroNoVariantes)grupo.GetFiltro("NoVariantes");
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroNoVariantes();
            MarcarValores();
        }
        private void CerrarVentana()
        {
            Close();
        }
        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.vx2";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.vx2";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.vx2"))
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroNoVariantes filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }

    }
}
