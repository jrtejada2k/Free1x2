using System;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Descripción breve de barraBotones.
	/// </summary>
	public class MenuCondiciones : UserControl
	{
		const int anchoBoton=32;
		const int anchoSeparador=10;
		private ToolBar toolBar1;
		private ToolBarButton sep1;
		private ToolBarButton sep2;
		private ToolBarButton btnAbrir;
		private ToolBarButton btnBorrar;
		private ToolBarButton btnCancelar;
		private ToolBarButton btnCopiar;
		private ToolBarButton btnGuardar;
		private ToolBarButton btnPegar;
		private ToolBarButton btnOk;
		private ToolBarButton sep3;
		private ToolBarButton sep4;
		private ToolBarButton btnEstadisticas;
		private ToolBarButton sep5;
		private System.ComponentModel.IContainer components=null;

		public event EventHandler BOk;
		public event EventHandler BCancelar;
		public event EventHandler BAbrir;
		public event EventHandler BGuardar;
		public event EventHandler BBorrar;
		public event EventHandler BCopiar;
		public event EventHandler BPegar;
		public event EventHandler BEstadisticas;
		protected int numBotones;
        private ImageList imageList1;

		protected alignment alineacion=alignment.Horizontal;
	
		public MenuCondiciones()
		{
			// Llamada necesaria para el Diseñador de formularios Windows.Forms.
			InitializeComponent();
           
			cargarImagenes();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }


		public int NumBotones
		{
			get{ return numBotones; }
			set
			{
				numBotones = value;
				Redibujar();
			}
		}

		public alignment Alineacion
		{
			get{ return alineacion; }
			set
			{
				alineacion = value;
				Redibujar();
			}
		}

		// Botones visibles
		public bool BotonAbrir
		{
			get{ return btnAbrir.Visible; }
			set
			{
				btnAbrir.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonBorrar
		{
			get{ return btnBorrar.Visible; }
			set
			{
				btnBorrar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonCancelar
		{
			get{ return btnCancelar.Visible; }
			set
			{
				btnCancelar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonCopiar
		{
			get{ return btnCopiar.Visible; }
			set
			{
				btnCopiar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonGuardar
		{
			get{ return btnGuardar.Visible; }
			set
			{
				btnGuardar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonPegar
		{
			get{ return btnPegar.Visible; }
			set
			{
				btnPegar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonOk
		{
			get{ return btnOk.Visible; }
			set
			{
				btnOk.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonEstadisticas
		{
			get{ return btnEstadisticas.Visible; }
			set
			{
				btnEstadisticas.Visible = value;
				ContarBotones(value);
			}
		}

		// Botones habilitados
		public bool BotonAbrirEnabled
		{
			get{ return btnAbrir.Enabled; }
			set{ btnAbrir.Enabled = value; }
		}
		public bool BotonBorrarEnabled
		{
			get{ return btnBorrar.Enabled; }
			set{ btnBorrar.Enabled = value; }
		}
		public bool BotonCancelarEnabled
		{
			get{ return btnCancelar.Enabled; }
			set{ btnCancelar.Enabled = value; }
		}
		public bool BotonCopiarEnabled
		{
			get{ return btnCopiar.Enabled; }
			set{ btnCopiar.Enabled = value; }
		}
		public bool BotonGuardarEnabled
		{
			get{ return btnGuardar.Enabled; }
			set{ btnGuardar.Enabled = value; }
		}
		public bool BotonPegarEnabled
		{
			get{ return btnPegar.Enabled; }
			set{ btnPegar.Enabled = value; }
		}
		public bool BotonOkEnabled
		{
			get{ return btnOk.Enabled; }
			set{ btnOk.Enabled = value; }
		}
		public bool BotonEstadisticasEnabled
		{
			get{ return btnEstadisticas.Enabled; }
			set{ btnEstadisticas.Enabled = value; }
		}

		private void ContarBotones(bool valor)
		{
			if(valor)
			{
				NumBotones++;
			}
			else
			{
				NumBotones--;
			}
		}

		private void Redibujar()
		{
			if(Alineacion==alignment.Horizontal)
			{
				toolBar1.Dock=DockStyle.Top;
				Width=(5*anchoSeparador)+(NumBotones*anchoBoton);
				toolBar1.Width=(5*anchoSeparador)+(NumBotones*anchoBoton);
				Height=anchoBoton+4;
				toolBar1.Height=anchoBoton+4;
			}
			else
			{
				toolBar1.Dock=DockStyle.Left;
				Height=(10*anchoSeparador)+(NumBotones*anchoBoton);
				toolBar1.Height=(10*anchoSeparador)+(NumBotones*anchoBoton);
				Width=anchoBoton+4;
				toolBar1.Width=anchoBoton+4;
			}
		}

		protected void OnBAbrir(EventArgs e)
		{
			if (BAbrir != null)
				BAbrir(this, e);
		}
		protected void OnBGuardar(EventArgs e)
		{
			if (BGuardar != null)
				BGuardar(this, e);
		}
		protected void OnBBorrar(EventArgs e)
		{
			if (BBorrar != null)
				BBorrar(this, e);
		}
		protected void OnBOk(EventArgs e)
		{
			if (BOk != null)
				BOk(this, e);
		}
		protected void OnBCancelar(EventArgs e)
		{
			if (BCancelar != null)
				BCancelar(this, e);
		}
		protected void OnBCopiar(EventArgs e)
		{
			if (BCopiar != null)
				BCopiar(this, e);
		}
		protected void OnBEstadisticas(EventArgs e)
		{
			if (BEstadisticas != null)
				BEstadisticas(this, e);
		}
		protected void OnBPegar(EventArgs e)
		{
			if (BPegar != null)
				BPegar(this, e);
		}

		/// <summary> 
		/// Limpiar los recursos que se estén utilizando.
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

		#region Código generado por el Diseñador de componentes
		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuCondiciones));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.sep1 = new System.Windows.Forms.ToolBarButton();
            this.btnOk = new System.Windows.Forms.ToolBarButton();
            this.btnCancelar = new System.Windows.Forms.ToolBarButton();
            this.sep2 = new System.Windows.Forms.ToolBarButton();
            this.btnAbrir = new System.Windows.Forms.ToolBarButton();
            this.btnGuardar = new System.Windows.Forms.ToolBarButton();
            this.btnBorrar = new System.Windows.Forms.ToolBarButton();
            this.sep3 = new System.Windows.Forms.ToolBarButton();
            this.btnCopiar = new System.Windows.Forms.ToolBarButton();
            this.btnPegar = new System.Windows.Forms.ToolBarButton();
            this.sep4 = new System.Windows.Forms.ToolBarButton();
            this.btnEstadisticas = new System.Windows.Forms.ToolBarButton();
            this.sep5 = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.sep1,
            this.btnOk,
            this.btnCancelar,
            this.sep2,
            this.btnAbrir,
            this.btnGuardar,
            this.btnBorrar,
            this.sep3,
            this.btnCopiar,
            this.btnPegar,
            this.sep4,
            this.btnEstadisticas,
            this.sep5});
            this.toolBar1.ButtonSize = new System.Drawing.Size(18, 18);
            this.toolBar1.Divider = false;
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.MaximumSize = new System.Drawing.Size(237, 36);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(237, 26);
            this.toolBar1.TabIndex = 1;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btnOk
            // 
            this.btnOk.ImageIndex = 0;
            this.btnOk.Name = "btnOk";
            this.btnOk.ToolTipText = "Ok";
            // 
            // btnCancelar
            // 
            this.btnCancelar.ImageIndex = 1;
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.ToolTipText = "Cancelar";
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btnAbrir
            // 
            this.btnAbrir.ImageIndex = 2;
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.ToolTipText = "Abrir";
            // 
            // btnGuardar
            // 
            this.btnGuardar.ImageIndex = 3;
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.ToolTipText = "Guardar";
            // 
            // btnBorrar
            // 
            this.btnBorrar.ImageIndex = 7;
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.ToolTipText = "Borrar";
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btnCopiar
            // 
            this.btnCopiar.ImageIndex = 4;
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.ToolTipText = "Copiar";
            // 
            // btnPegar
            // 
            this.btnPegar.ImageIndex = 5;
            this.btnPegar.Name = "btnPegar";
            this.btnPegar.ToolTipText = "Pegar";
            // 
            // sep4
            // 
            this.sep4.Name = "sep4";
            this.sep4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btnEstadisticas
            // 
            this.btnEstadisticas.ImageIndex = 8;
            this.btnEstadisticas.Name = "btnEstadisticas";
            this.btnEstadisticas.ToolTipText = "Estadísticas";
            // 
            // sep5
            // 
            this.sep5.Name = "sep5";
            this.sep5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ok.gif");
            this.imageList1.Images.SetKeyName(1, "cancelar.gif");
            this.imageList1.Images.SetKeyName(2, "abrir.gif");
            this.imageList1.Images.SetKeyName(3, "archivo.gif");
            this.imageList1.Images.SetKeyName(4, "papelera.gif");
            this.imageList1.Images.SetKeyName(5, "Copiar.gif");
            this.imageList1.Images.SetKeyName(6, "pegar.gif");
            this.imageList1.Images.SetKeyName(7, "analisis.gif");
            // 
            // MenuCondiciones
            // 
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.toolBar1);
            this.Name = "MenuCondiciones";
            this.Size = new System.Drawing.Size(240, 36);
            this.Load += new System.EventHandler(this.MenuCondiciones_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void MenuCondiciones_Load(object sender, EventArgs e)
		{
			int n=0;
			// Comprueba los botones activados
			if(BotonAbrir) n++;
			if(BotonBorrar) n++;
			if(BotonCancelar) n++;
			if(BotonCopiar) n++;
			if(BotonGuardar) n++;
			if(BotonPegar) n++;
			if(BotonOk) n++;
			if(BotonEstadisticas) n++;
			NumBotones=n;
			Redibujar();
		}

		private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			switch (e.Button.ToolTipText)
			{
				case "Abrir":
					OnBAbrir(EventArgs.Empty);
					break;
				case "Guardar":
					OnBGuardar(EventArgs.Empty);
					break;
				case "Borrar":
					OnBBorrar(EventArgs.Empty);
					break;
				case "Ok":
					OnBOk(EventArgs.Empty);
					break;
				case "Cancelar":
					OnBCancelar(EventArgs.Empty);
					break;
				case "Copiar":
					OnBCopiar(EventArgs.Empty);
					break;
				case "Pegar":
					OnBPegar(EventArgs.Empty);
					break;
				case "Estadísticas":
					OnBEstadisticas(EventArgs.Empty);
					break;
			}
		}

		private void cargarImagenes()
		{
			try
			{
				imageList1.ImageSize=new Size(16,16);

				toolBar1.ImageList=imageList1;
				btnOk.ImageIndex=0;
				btnCancelar.ImageIndex=1;
				btnAbrir.ImageIndex=2;
				btnGuardar.ImageIndex=3;
				btnBorrar.ImageIndex=4;
				btnCopiar.ImageIndex=5;
				btnPegar.ImageIndex=6;
				btnEstadisticas.ImageIndex=7;
			}
			catch{}
		}

	}
}
