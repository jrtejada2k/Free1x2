using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System;

using Free1X2.Utils;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for TramificarGraficasFrm.
	/// </summary>
	public class TramificarGraficasFrm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private ArrayList Tramos=null;
		private double escalaX;
		private double escalaY;
		Grafico grafica = null;
		Point[] Puntos= null;
		bool primeraVez=true;
		private System.Windows.Forms.ToolBar toolBarGraficas;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		private System.Windows.Forms.ToolBarButton toolBarButton8;
		private System.Windows.Forms.ToolBarButton toolBarButton9;
		private System.Windows.Forms.ImageList iListGraficas;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.Panel panel9;
		private System.Windows.Forms.Panel panel10;
		private System.Windows.Forms.ToolBarButton toolBarButton10;
		private System.Windows.Forms.ToolBarButton toolBarButton11;
		private System.Windows.Forms.ToolBarButton toolBarButton12;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.ComponentModel.IContainer components;

		public TramificarGraficasFrm(ArrayList pTramos)
		{
			InitializeComponent();
			Tramos=pTramos;
			WindowState = FormWindowState.Maximized;

			Puntos= new Point [Tramos.Count];
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TramificarGraficasFrm));
            this.toolBarGraficas = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton7 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton8 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton9 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton12 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton10 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton11 = new System.Windows.Forms.ToolBarButton();
            this.iListGraficas = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolBarGraficas
            // 
            this.toolBarGraficas.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3,
            this.toolBarButton4,
            this.toolBarButton5,
            this.toolBarButton6,
            this.toolBarButton7,
            this.toolBarButton8,
            this.toolBarButton9,
            this.toolBarButton12,
            this.toolBarButton10,
            this.toolBarButton11});
            this.toolBarGraficas.ButtonSize = new System.Drawing.Size(40, 24);
            this.toolBarGraficas.DropDownArrows = true;
            this.toolBarGraficas.ImageList = this.iListGraficas;
            this.toolBarGraficas.Location = new System.Drawing.Point(0, 0);
            this.toolBarGraficas.Name = "toolBarGraficas";
            this.toolBarGraficas.ShowToolTips = true;
            this.toolBarGraficas.Size = new System.Drawing.Size(652, 28);
            this.toolBarGraficas.TabIndex = 6;
            this.toolBarGraficas.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
            this.toolBarGraficas.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarGraficas_ButtonClick);
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.ImageIndex = 0;
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Text = "Pr";
            this.toolBarButton1.ToolTipText = "Gráfica de probabilidades";
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.ImageIndex = 0;
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.Text = "14";
            this.toolBarButton2.ToolTipText = "Gráfica de los 14 aciertos";
            // 
            // toolBarButton3
            // 
            this.toolBarButton3.ImageIndex = 0;
            this.toolBarButton3.Name = "toolBarButton3";
            this.toolBarButton3.Text = "13";
            this.toolBarButton3.ToolTipText = "Gráfica de los 13 aciertos";
            // 
            // toolBarButton4
            // 
            this.toolBarButton4.ImageIndex = 0;
            this.toolBarButton4.Name = "toolBarButton4";
            this.toolBarButton4.Text = "12";
            this.toolBarButton4.ToolTipText = "Gráfica de los 12 aciertos";
            // 
            // toolBarButton5
            // 
            this.toolBarButton5.ImageIndex = 0;
            this.toolBarButton5.Name = "toolBarButton5";
            this.toolBarButton5.Text = "11";
            this.toolBarButton5.ToolTipText = "Gráfica de los 11 aciertos";
            // 
            // toolBarButton6
            // 
            this.toolBarButton6.ImageIndex = 0;
            this.toolBarButton6.Name = "toolBarButton6";
            this.toolBarButton6.Text = "10";
            this.toolBarButton6.ToolTipText = "Gráfica de los 10 aciertos";
            // 
            // toolBarButton7
            // 
            this.toolBarButton7.ImageIndex = 0;
            this.toolBarButton7.Name = "toolBarButton7";
            this.toolBarButton7.Text = "Nº";
            this.toolBarButton7.ToolTipText = "Gráfica del nº de aciertos";
            // 
            // toolBarButton8
            // 
            this.toolBarButton8.ImageIndex = 0;
            this.toolBarButton8.Name = "toolBarButton8";
            this.toolBarButton8.Text = "";
            this.toolBarButton8.ToolTipText = "Grafica del importe de premios";
            // 
            // toolBarButton9
            // 
            this.toolBarButton9.ImageIndex = 0;
            this.toolBarButton9.Name = "toolBarButton9";
            this.toolBarButton9.Text = "+/-";
            this.toolBarButton9.ToolTipText = "Grafica del balance de ingresos - gastos";
            // 
            // toolBarButton12
            // 
            this.toolBarButton12.Name = "toolBarButton12";
            this.toolBarButton12.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // toolBarButton10
            // 
            this.toolBarButton10.ImageIndex = 1;
            this.toolBarButton10.Name = "toolBarButton10";
            this.toolBarButton10.ToolTipText = "Borra la imagen";
            // 
            // toolBarButton11
            // 
            this.toolBarButton11.ImageIndex = 2;
            this.toolBarButton11.Name = "toolBarButton11";
            this.toolBarButton11.ToolTipText = "Copia la imagen en el clipboard";
            // 
            // iListGraficas
            // 
            this.iListGraficas.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iListGraficas.ImageStream")));
            this.iListGraficas.TransparentColor = System.Drawing.Color.Transparent;
            this.iListGraficas.Images.SetKeyName(0, "grafico.gif");
            this.iListGraficas.Images.SetKeyName(1, "nuevo.gif");
            this.iListGraficas.Images.SetKeyName(2, "Copiar.gif");
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(0, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(40, 8);
            this.panel2.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.DodgerBlue;
            this.panel3.Location = new System.Drawing.Point(40, 28);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(40, 8);
            this.panel3.TabIndex = 8;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Brown;
            this.panel4.Location = new System.Drawing.Point(80, 28);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(40, 8);
            this.panel4.TabIndex = 9;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Cyan;
            this.panel5.Location = new System.Drawing.Point(120, 28);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(40, 8);
            this.panel5.TabIndex = 10;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Green;
            this.panel6.Location = new System.Drawing.Point(160, 28);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(40, 8);
            this.panel6.TabIndex = 11;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Red;
            this.panel7.Location = new System.Drawing.Point(200, 28);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(40, 8);
            this.panel7.TabIndex = 12;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.CadetBlue;
            this.panel8.Location = new System.Drawing.Point(240, 28);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(40, 8);
            this.panel8.TabIndex = 13;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.ForestGreen;
            this.panel9.Location = new System.Drawing.Point(280, 28);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(40, 8);
            this.panel9.TabIndex = 14;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.Orange;
            this.panel10.Location = new System.Drawing.Point(320, 28);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(40, 8);
            this.panel10.TabIndex = 15;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Beige;
            this.pictureBox1.Location = new System.Drawing.Point(12, 40);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(628, 368);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // TramificarGraficasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(652, 422);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolBarGraficas);
            this.Name = "TramificarGraficasFrm";
            this.Text = "Gráficas de los resultados del análisis de tramos";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		private void btGrafica_Click(object sender, System.EventArgs e)
		{
			DibujaProbabilidad();
		}

		private void DibujaProbabilidad()
		{
			int i;
			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (35+tr.ProbAcumulada)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.DibujaCurva (new Pen(Color.Black));
		}

		private void DibujaP10()
		{
			int i;
			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.P10)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.Red));
		}

		private void DibujaP11()
		{
			int i;

			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.P11)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.Green ));
		}

		private void DibujaP12()
		{
			int i;

			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.P12)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.Cyan));
		}

		private void DibujaP13()
		{
			int i;

			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.P13)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.Brown));
		}

		private void DibujaP14()
		{
			int i;

			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.P14)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.Blue));
		}

		private void DibujaColumnasPremiadas()
		{
			int i;

			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.ColumnasPremiadas)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.CadetBlue ));
		}

		private void DibujaTotalImportePremios()
		{
			int i;

			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.TotalImportePremios)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.ForestGreen));
		}

		private void DibujaBalance()
		{
			int i;

			foreach (Tramo tr in Tramos)
			{
				i=tr.NumeroDeTramo-1;
				Puntos.SetValue(new Point (i , (int) (tr.Balance)), i);
			}
			grafica= new Grafico(Puntos,pictureBox1);
			grafica.EscalaX =escalaX;
			grafica.EscalaY =escalaY;
			grafica.DibujaCurva (new Pen(Color.Orange));
		}

		private void CopiarImagenEnClipboard()
		{
    		Clipboard.SetDataObject(pictureBox1.Image ,true);
		}

		private void LimpiarImagen()
		{
			Bitmap B = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics gr = Graphics.FromImage(B);
			gr.Clear(Color.Beige);
			pictureBox1.Image = B;
			gr.Dispose ();
			primeraVez=true;
			escalaX=0;
			escalaY=0;
		}

		private void toolBarGraficas_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(primeraVez) LimpiarImagen();

			switch (toolBarGraficas.Buttons.IndexOf(e.Button))
			{
				case 0: DibujaProbabilidad ();	break;
				case 1: DibujaP14 ();break;
				case 2: DibujaP13 ();break;
				case 3: DibujaP12 ();break;
				case 4: DibujaP11 ();break;
				case 5: DibujaP10 ();break;
				case 6: DibujaColumnasPremiadas ();break;
				case 7: DibujaTotalImportePremios ();break;
				case 8: DibujaBalance ();break;
				case 10: LimpiarImagen();break;
				case 11: CopiarImagenEnClipboard(); break;
				default: break;
			}
			if(primeraVez && toolBarGraficas.Buttons.IndexOf(e.Button)< 9)
			{
				grafica= new Grafico(Puntos,pictureBox1);
				grafica.DibujaGrid (50);
				grafica.DibujarEjes ();
				escalaX=grafica.EscalaX ;
				escalaY=grafica.EscalaY ;
				primeraVez=false;
			}
		}
	}
}
