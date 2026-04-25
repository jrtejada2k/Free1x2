using System;
using System.ComponentModel;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Descripción breve de barraBotones.
	/// </summary>
	public class ctrBarraBotones : UserControl
	{
		const int anchoBoton=24;
		private ToolBar toolBar1;
		private ToolBarButton sep1;
		private ToolBarButton sep2;
		private ToolBarButton btnAbrir;
		private ToolBarButton btnAyuda;
		private ToolBarButton btnBorrar;
		private ToolBarButton btnBuscar;
		private ToolBarButton btnCancelar;
		private ToolBarButton btnCerrar;
		private ToolBarButton btnCopiar;
		private ToolBarButton btnCortar;
		private ToolBarButton btnDeshacer;
		private ToolBarButton btnGuardar;
		private ToolBarButton btnImprimir;
		private ToolBarButton btnNuevo;
		private ToolBarButton btnPegar;
		private ToolBarButton btnPrevio;
		private ToolBarButton btnPunteo;
		private ToolBarButton btnRehacer;
		private ToolBarButton btnSalir;
		private ToolBarButton btnOk;
		private IContainer components;
		private ImageList _imageList1;

		public event EventHandler BSalir;
		public event EventHandler BNuevo;
		public event EventHandler BAbrir;
		public event EventHandler BGuardar;
		public event EventHandler BCerrar;
		public event EventHandler BPrevio;
		public event EventHandler BImprimir;
		public event EventHandler BBorrar;
		public event EventHandler BPunteo;
		public event EventHandler BOk;
		public event EventHandler BCancelar;
		public event EventHandler BBuscar;
		public event EventHandler BCopiar;
		public event EventHandler BCortar;
		public event EventHandler BPegar;
		public event EventHandler BDeshacer;
		public event EventHandler BRehacer;
		public event EventHandler BAyuda;

		protected bool botonAbrir=true;
		protected bool botonAyuda=true;
		protected bool botonBorrar=true;
		protected bool botonBuscar=true;
		protected bool botonCancelar=true;
		protected bool botonCerrar=true;
		protected bool botonCopiar=true;
		protected bool botonCortar=true;
		protected bool botonDeshacer=true;
		protected bool botonGuardar=true;
		protected bool botonImprimir=true;
		protected bool botonNuevo=true;
		protected bool botonPegar=true;
		protected bool botonPrevio=true;
		protected bool botonPunteo=true;
		protected bool botonRehacer=true;
		protected bool botonSalir=true;
		protected bool botonOk=true;
		protected int numBotones=18;
		protected alignment alineacion=alignment.Horizontal;

		public enum alignment
		{
			Vertical,
			Horizontal
		}
	
		public int NumBotones
		{
			get{ return numBotones; }
			set{ numBotones = value;}
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

		public bool BotonAbrir
		{
			get{ return botonAbrir; }
			set
			{
				botonAbrir = value;
				btnAbrir.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonAyuda
		{
			get{ return botonAyuda; }
			set
			{
				botonAyuda = value;
				btnAyuda.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonBorrar
		{
			get{ return botonBorrar; }
			set
			{
				botonBorrar = value;
				btnBorrar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonBuscar
		{
			get{ return botonBuscar; }
			set
			{
				botonBuscar = value;
				btnBuscar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonCancelar
		{
			get{ return botonCancelar; }
			set
			{
				botonCancelar = value;
				btnCancelar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonCerrar
		{
			get{ return botonCerrar; }
			set
			{
				botonCerrar = value;
				btnCerrar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonCopiar
		{
			get{ return botonCopiar; }
			set
			{
				botonCopiar = value;
				btnCopiar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonCortar
		{
			get{ return botonCortar; }
			set
			{
				botonCortar = value;
				btnCortar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonDeshacer
		{
			get{ return botonDeshacer; }
			set
			{
				botonDeshacer = value;
				btnDeshacer.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonGuardar
		{
			get{ return botonGuardar; }
			set
			{
				botonGuardar = value;
				btnGuardar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonImprimir
		{
			get{ return botonImprimir; }
			set
			{
				botonImprimir = value;
				btnImprimir.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonNuevo
		{
			get{ return botonNuevo; }
			set
			{
				botonNuevo = value;
				btnNuevo.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonPegar
		{
			get{ return botonPegar; }
			set
			{
				botonPegar = value;
				btnPegar.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonPrevio
		{
			get{ return botonPrevio; }
			set
			{
				botonPrevio = value;
				btnPrevio.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonPunteo
		{
			get{ return botonPunteo; }
			set
			{
				botonPunteo = value;
				btnPunteo.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonRehacer
		{
			get{ return botonRehacer; }
			set
			{
				botonRehacer = value;
				btnRehacer.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonSalir
		{
			get{ return botonSalir; }
			set
			{
				botonSalir = value;
				btnSalir.Visible = value;
				ContarBotones(value);
			}
		}
		public bool BotonOk
		{
			get{ return botonOk; }
			set
			{
				botonOk = value;
				btnOk.Visible = value;
				ContarBotones(value);
			}
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
			Redibujar();
		}

		private void Redibujar()
		{
			if(Alineacion==alignment.Horizontal)
			{
				toolBar1.Dock=DockStyle.Top;
				Width=8+(NumBotones*anchoBoton);
				toolBar1.Width=8+(NumBotones*anchoBoton);
				Height=anchoBoton+2;
				toolBar1.Height=anchoBoton+2;
			}
			else
			{
				toolBar1.Dock=DockStyle.Left;
				Height=8+(NumBotones*anchoBoton);
				toolBar1.Height=8+(NumBotones*anchoBoton);
				Width=anchoBoton+2;
				toolBar1.Width=anchoBoton+2;
			}
		}

		protected void OnBSalir(EventArgs e)
		{
			if (BSalir != null)
				BSalir(this, e);
		}
		protected void OnBNuevo(EventArgs e)
		{
			if (BNuevo != null)
				BNuevo(this, e);
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
		protected void OnBCerrar(EventArgs e)
		{
			if (BCerrar != null)
				BCerrar(this, e);
		}
		protected void OnBPrevio(EventArgs e)
		{
			if (BPrevio != null)
				BPrevio(this, e);
		}
		protected void OnBImprimir(EventArgs e)
		{
			if (BImprimir != null)
				BImprimir(this, e);
		}
		protected void OnBBorrar(EventArgs e)
		{
			if (BBorrar != null)
				BBorrar(this, e);
		}
		protected void OnBPunteo(EventArgs e)
		{
			if (BPunteo != null)
				BPunteo(this, e);
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
		protected void OnBBuscar(EventArgs e)
		{
			if (BBuscar != null)
				BBuscar(this, e);
		}
		protected void OnBCopiar(EventArgs e)
		{
			if (BCopiar != null)
				BCopiar(this, e);
		}
		protected void OnBCortar(EventArgs e)
		{
			if (BCortar != null)
				BCortar(this, e);
		}
		protected void OnBPegar(EventArgs e)
		{
			if (BPegar != null)
				BPegar(this, e);
		}
		protected void OnBDeshacer(EventArgs e)
		{
			if (BDeshacer != null)
				BDeshacer(this, e);
		}
		protected void OnBRehacer(EventArgs e)
		{
			if (BRehacer != null)
				BRehacer(this, e);
		}
		protected void OnBAyuda(EventArgs e)
		{
			if (BAyuda != null)
				BAyuda(this, e);
		}


		public ctrBarraBotones()
		{
			InitializeComponent();
			Console.Write(Application.StartupPath);
			cargarImagenes();

		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrBarraBotones));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.sep1 = new System.Windows.Forms.ToolBarButton();
            this.btnSalir = new System.Windows.Forms.ToolBarButton();
            this.btnNuevo = new System.Windows.Forms.ToolBarButton();
            this.btnAbrir = new System.Windows.Forms.ToolBarButton();
            this.btnGuardar = new System.Windows.Forms.ToolBarButton();
            this.btnCerrar = new System.Windows.Forms.ToolBarButton();
            this.btnPrevio = new System.Windows.Forms.ToolBarButton();
            this.btnImprimir = new System.Windows.Forms.ToolBarButton();
            this.btnBorrar = new System.Windows.Forms.ToolBarButton();
            this.btnPunteo = new System.Windows.Forms.ToolBarButton();
            this.btnOk = new System.Windows.Forms.ToolBarButton();
            this.btnCancelar = new System.Windows.Forms.ToolBarButton();
            this.btnBuscar = new System.Windows.Forms.ToolBarButton();
            this.btnCopiar = new System.Windows.Forms.ToolBarButton();
            this.btnCortar = new System.Windows.Forms.ToolBarButton();
            this.btnPegar = new System.Windows.Forms.ToolBarButton();
            this.btnDeshacer = new System.Windows.Forms.ToolBarButton();
            this.btnRehacer = new System.Windows.Forms.ToolBarButton();
            this.btnAyuda = new System.Windows.Forms.ToolBarButton();
            this.sep2 = new System.Windows.Forms.ToolBarButton();
            this._imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.sep1,
            this.btnSalir,
            this.btnNuevo,
            this.btnAbrir,
            this.btnGuardar,
            this.btnCerrar,
            this.btnPrevio,
            this.btnImprimir,
            this.btnBorrar,
            this.btnPunteo,
            this.btnOk,
            this.btnCancelar,
            this.btnBuscar,
            this.btnCopiar,
            this.btnCortar,
            this.btnPegar,
            this.btnDeshacer,
            this.btnRehacer,
            this.btnAyuda,
            this.sep2});
            this.toolBar1.Divider = false;
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(440, 26);
            this.toolBar1.TabIndex = 1;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btnSalir
            // 
            this.btnSalir.ImageIndex = 0;
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.ToolTipText = "Salir";
            // 
            // btnNuevo
            // 
            this.btnNuevo.ImageIndex = 1;
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.ToolTipText = "Nuevo";
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
            // btnCerrar
            // 
            this.btnCerrar.ImageIndex = 4;
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.ToolTipText = "Cerrar";
            // 
            // btnPrevio
            // 
            this.btnPrevio.ImageIndex = 5;
            this.btnPrevio.Name = "btnPrevio";
            this.btnPrevio.ToolTipText = "Vista previa";
            // 
            // btnImprimir
            // 
            this.btnImprimir.ImageIndex = 6;
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.ToolTipText = "Imprimir";
            // 
            // btnBorrar
            // 
            this.btnBorrar.ImageIndex = 7;
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.ToolTipText = "Borrar";
            // 
            // btnPunteo
            // 
            this.btnPunteo.ImageIndex = 8;
            this.btnPunteo.Name = "btnPunteo";
            this.btnPunteo.ToolTipText = "Punteo";
            // 
            // btnOk
            // 
            this.btnOk.ImageIndex = 9;
            this.btnOk.Name = "btnOk";
            this.btnOk.ToolTipText = "Ok";
            // 
            // btnCancelar
            // 
            this.btnCancelar.ImageIndex = 10;
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.ToolTipText = "Cancelar";
            // 
            // btnBuscar
            // 
            this.btnBuscar.ImageIndex = 11;
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.ToolTipText = "Buscar";
            // 
            // btnCopiar
            // 
            this.btnCopiar.ImageIndex = 12;
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.ToolTipText = "Copiar";
            // 
            // btnCortar
            // 
            this.btnCortar.ImageIndex = 13;
            this.btnCortar.Name = "btnCortar";
            this.btnCortar.ToolTipText = "Cortar";
            // 
            // btnPegar
            // 
            this.btnPegar.ImageIndex = 14;
            this.btnPegar.Name = "btnPegar";
            this.btnPegar.ToolTipText = "Pegar";
            // 
            // btnDeshacer
            // 
            this.btnDeshacer.ImageIndex = 15;
            this.btnDeshacer.Name = "btnDeshacer";
            this.btnDeshacer.ToolTipText = "Deshacer";
            // 
            // btnRehacer
            // 
            this.btnRehacer.ImageIndex = 16;
            this.btnRehacer.Name = "btnRehacer";
            this.btnRehacer.ToolTipText = "Rehacer";
            // 
            // btnAyuda
            // 
            this.btnAyuda.ImageIndex = 17;
            this.btnAyuda.Name = "btnAyuda";
            this.btnAyuda.ToolTipText = "Ayuda";
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // _imageList1
            // 
            this._imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList1.ImageStream")));
            this._imageList1.TransparentColor = System.Drawing.Color.LightSalmon;
            this._imageList1.Images.SetKeyName(0, "salir.gif");
            this._imageList1.Images.SetKeyName(1, "nuevo.gif");
            this._imageList1.Images.SetKeyName(2, "abrir.gif");
            this._imageList1.Images.SetKeyName(3, "archivo.gif");
            this._imageList1.Images.SetKeyName(4, "Cerrar.bmp");
            this._imageList1.Images.SetKeyName(5, "IrIzquierda.gif");
            this._imageList1.Images.SetKeyName(6, "imprimir.gif");
            this._imageList1.Images.SetKeyName(7, "papelera.gif");
            this._imageList1.Images.SetKeyName(8, "Punteo.bmp");
            this._imageList1.Images.SetKeyName(9, "ok.gif");
            this._imageList1.Images.SetKeyName(10, "cancelar.gif");
            this._imageList1.Images.SetKeyName(11, "analisisSignos.gif");
            this._imageList1.Images.SetKeyName(12, "Copiar.gif");
            this._imageList1.Images.SetKeyName(13, "Cortar.bmp");
            this._imageList1.Images.SetKeyName(14, "pegar.gif");
            this._imageList1.Images.SetKeyName(15, "IrInicio.gif");
            this._imageList1.Images.SetKeyName(16, "IrFinal.gif");
            this._imageList1.Images.SetKeyName(17, "ayuda.gif");
            // 
            // ctrBarraBotones
            // 
            this.Controls.Add(this.toolBar1);
            this.Name = "ctrBarraBotones";
            this.Size = new System.Drawing.Size(440, 32);
            this.Load += new System.EventHandler(this.ctrBarraBotones_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ctrBarraBotones_Load(object sender, EventArgs e)
		{
			Redibujar();
		}

		private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			switch (e.Button.ImageIndex)
			{
				case 0:
					OnBSalir(EventArgs.Empty);
					break;
				case 1:
					OnBNuevo(EventArgs.Empty);
					break;
				case 2:
					OnBAbrir(EventArgs.Empty);
					break;
				case 3:
					OnBGuardar(EventArgs.Empty);
					break;
				case 4:
					OnBCerrar(EventArgs.Empty);
					break;
				case 5:
					OnBPrevio(EventArgs.Empty);
					break;
				case 6:
					OnBImprimir(EventArgs.Empty);
					break;
				case 7:
					OnBBorrar(EventArgs.Empty);
					break;
				case 8:
					OnBPunteo(EventArgs.Empty);
					break;
				case 9:
					OnBOk(EventArgs.Empty);
					break;
				case 10:
					OnBCancelar(EventArgs.Empty);
					break;
				case 11:
					OnBBuscar(EventArgs.Empty);
					break;
				case 12:
					OnBCopiar(EventArgs.Empty);
					break;
				case 13:
					OnBCortar(EventArgs.Empty);
					break;
				case 14:
					OnBPegar(EventArgs.Empty);
					break;
				case 15:
					OnBDeshacer(EventArgs.Empty);
					break;
				case 16:
					OnBRehacer(EventArgs.Empty);
					break;
				case 17:
					OnBAyuda(EventArgs.Empty);
					break;
			}
		}

		private void cargarImagenes()
		{
			toolBar1.ImageList=_imageList1;			
		}
	}
}
