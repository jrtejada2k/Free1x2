using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Free1X2.Utils;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Summary description for ControlPorcentajes.
	/// </summary>
	public class ControlPorcentajes : UserControl
	{
		private MyDataGrid dataGridPorcentajes;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private ArrayList Valors;
		private string _archivoPorcentajes;
		private short _FormatoFicheroValoraciones;
		private bool _ReadOnly;
		private ctrBarraBotones ctrBarraBotones1;
		private ComboBox comboValoraciones;
		public event EventHandler Modificado;
		private string _Jornada="01";
		private string _Temporada = "2004/2005";

	
		private class pct
		{
			private double[] _v = new double [3];
			public pct()
			{			
				_v[0]=50;			
				_v[1]=30;			
				_v[2]=20;			
			}
			public pct(double v1, double vx, double v2)
			{			
				_v[0]=v1;			
				_v[1]=vx;			
				_v[2]=v2;			
			}
			public double[] porcentajes
			{
				get{ return _v; }
				set{_v = value;}
			}
			public double Uno
			{
				get{ return _v[0]; }
				set{_v[0] = value;}
			}
			public double Equis
			{
				get{ return _v[1]; }
				set{_v[1] = value;}
			}
			public double Dos
			{
				get{ return _v[2]; }
				set{_v[2] = value;}
			}
		}


			private System.ComponentModel.Container components = null;

			public ControlPorcentajes()
            {     
                //Establecemos un noPartidos por defecto para el diseñador
                int noPartidos;
                //Modificamos este valor para que acepte el número de partidos de la conf
                try
                {
                    noPartidos = VariablesGlobales.NumeroPartidos;
                }
                catch
                {
                    //Aplicar valor por defecto
                    noPartidos = 14;
                }
				InitializeComponent();

				Valors =new ArrayList();
                for (int i = 0; i < noPartidos; i++)
                {
                    Valors.Add(new pct());
                }
				InicializaGridPorcentajes();
				GridDataBind();
			}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

        public ControlPorcentajes(int partidos)
        {
            //Este método debería usarse en las utilidades en las que el número de partidos 
            //depende del archivo que se maneje
           
            InitializeComponent();

            Valors = new ArrayList();
            for (int i = 0; i < partidos; i++)
            {
                Valors.Add(new pct());
            }
            InicializaGridPorcentajes();
            GridDataBind();
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

			public double[,] Valores
			{
				get
				{ 
					double[,] p = new double[Valors.Count,3];
					int cont =0;
					foreach (pct i in Valors)
					{
						p[cont,0]=i.Uno;
						p[cont,1]=i.Equis;
						p[cont,2]=i.Dos;
						cont++;
					}
					return p;
				}
				set
				{
					double[,] p = value;
					Valors =new ArrayList();
                    for (int i = 0; i < p.GetLength(0); i++)
					{
						Valors.Add (new pct(p[i,0],p[i,1],p[i,2]));
					}
					GridDataBind();
				}
			}
		public bool ReadOnly
		{
			get
			{ 
				return _ReadOnly;
			}
			set
			{
				_ReadOnly=value;
				dataGridPorcentajes.Enabled=(value==false);
				ctrBarraBotones1.BotonAbrir=(value==false);
				ctrBarraBotones1.BotonPegar=(value==false);
			}
		}
		public short FormatoFicheroValoraciones
		{
			get	{ return _FormatoFicheroValoraciones;}
			set	{_FormatoFicheroValoraciones =value;}
		}		
		public string archivoPorcentajes
		{
			get	{ return _archivoPorcentajes;}
			set	{_archivoPorcentajes =value;}
		}
		public string CaptionText
		{
			get	{ return dataGridPorcentajes.CaptionText;}
			set	{dataGridPorcentajes.CaptionText =value;}
		}
		public string Jornada
		{
			get	{ return _Jornada;}
			set	{_Jornada =value;}
		}
		public string Temporada
		{
			get	{ return _Temporada;}
			set	{_Temporada =value;	}
		}
			protected void InicializaGridPorcentajes()
			{			
				DataGridTableStyle tableStyle = new DataGridTableStyle();
				tableStyle.MappingName = "ArrayList";
				tableStyle.ColumnHeadersVisible = true;

			    //		unos
				DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
				cs.MappingName = "Uno";
				cs.HeaderText = "  1";
				cs.Width = 40;
				tableStyle.GridColumnStyles.Add(cs);

				//		equis
				cs = new DataGridTextBoxColumn();
				cs.MappingName = "Equis";
				cs.HeaderText = "  X";
				cs.Width = 40;
				tableStyle.GridColumnStyles.Add(cs);

				//		doses
				cs = new DataGridTextBoxColumn();
				cs.MappingName = "Dos";
				cs.HeaderText = "  2";
				cs.Width = 40;
				tableStyle.GridColumnStyles.Add(cs);

				dataGridPorcentajes.TableStyles.Add(tableStyle);
			}
			protected void GridDataBind()
			{
				dataGridPorcentajes.DataSource = Valors;	
				dataGridPorcentajes.Refresh ();
			}
        private void dataGridPorcentajes_Paint(object sender, PaintEventArgs e)
        {
            int row = 0;//TopRow(); 
            int yDelta = dataGridPorcentajes.GetCellBounds(row, 0).Height + 1;
            int y = dataGridPorcentajes.GetCellBounds(row, 0).Top + 2;
            CurrencyManager cm = (CurrencyManager)BindingContext[dataGridPorcentajes.DataSource, dataGridPorcentajes.DataMember];
            int pos = 13;
            double nv;
            SolidBrush Brocha;
            Font f = new Font(dataGridPorcentajes.Font.Name, dataGridPorcentajes.Font.Size, FontStyle.Bold);

            while (y < dataGridPorcentajes.Height - yDelta && row < cm.Count)
            {
                string text = string.Format("{0}", row + 1);
                if (row > 8) pos = 9;
                nv = (double)dataGridPorcentajes[row, 0] + (double)dataGridPorcentajes[row, 1] + (double)dataGridPorcentajes[row, 2];
                Brocha = new SolidBrush((nv < 99 || nv > 101) ? Color.Red : Color.Black);
                e.Graphics.DrawString(text, f, Brocha, pos, y);
                y += yDelta;
                row++;
            }

            Brocha = new SolidBrush(Color.FromArgb(80, 255, 255, 128));
            int x = 120 + dataGridPorcentajes.GetCellBounds(4, 0).Left;
            // dataGridPorcentajes.GetCellBounds(0,0).
            try
            {
                y = dataGridPorcentajes.GetCellBounds(4, 0).Top;
                int y2 = dataGridPorcentajes.GetCellBounds(8, 0).Top - y;
                e.Graphics.FillRectangle(Brocha, new Rectangle(1, y, x, y2));
            }
            catch
            {
            }
            try
            {
                y = dataGridPorcentajes.GetCellBounds(11, 0).Top;
                int y2 = (dataGridPorcentajes.GetCellBounds(13, 0).Top + 16) - y;
                e.Graphics.FillRectangle(Brocha, new Rectangle(1, y, x, y2));


            }
            catch
            {
            }
            Brocha.Dispose();
        }
		public class MyDataGrid : DataGrid
		{
			protected override void OnMouseMove(MouseEventArgs e)
			{
				HitTestInfo hti = HitTest(new Point(e.X, e.Y));
				if(hti.Type == HitTestType.RowResize) 
				{
					return; //no baseclass call
				}
				base.OnMouseMove(e);
			}
		}

		#region Component Designer generated code
			/// <summary> 
			/// Required method for Designer support - do not modify 
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
                this.comboValoraciones = new System.Windows.Forms.ComboBox();
                this.ctrBarraBotones1 = new Free1X2.UI.Controls.ctrBarraBotones();
                this.dataGridPorcentajes = new Free1X2.UI.Controls.ControlPorcentajes.MyDataGrid();
                ((System.ComponentModel.ISupportInitialize)(this.dataGridPorcentajes)).BeginInit();
                this.SuspendLayout();
                // 
                // comboValoraciones
                // 
                this.comboValoraciones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                this.comboValoraciones.Location = new System.Drawing.Point(0, 0);
                this.comboValoraciones.MaxDropDownItems = 16;
                this.comboValoraciones.Name = "comboValoraciones";
                this.comboValoraciones.Size = new System.Drawing.Size(160, 21);
                this.comboValoraciones.TabIndex = 278;
                this.comboValoraciones.Visible = false;
                this.comboValoraciones.SelectedIndexChanged += new System.EventHandler(this.comboValoraciones_SelectedIndexChanged);
                // 
                // ctrBarraBotones1
                // 
                this.ctrBarraBotones1.Alineacion = Free1X2.UI.Controls.ctrBarraBotones.alignment.Horizontal;
                this.ctrBarraBotones1.BotonAbrir = true;
                this.ctrBarraBotones1.BotonAyuda = false;
                this.ctrBarraBotones1.BotonBorrar = false;
                this.ctrBarraBotones1.BotonBuscar = false;
                this.ctrBarraBotones1.BotonCancelar = false;
                this.ctrBarraBotones1.BotonCerrar = false;
                this.ctrBarraBotones1.BotonCopiar = true;
                this.ctrBarraBotones1.BotonCortar = false;
                this.ctrBarraBotones1.BotonDeshacer = false;
                this.ctrBarraBotones1.BotonGuardar = true;
                this.ctrBarraBotones1.BotonImprimir = false;
                this.ctrBarraBotones1.BotonNuevo = false;
                this.ctrBarraBotones1.BotonOk = false;
                this.ctrBarraBotones1.BotonPegar = true;
                this.ctrBarraBotones1.BotonPrevio = false;
                this.ctrBarraBotones1.BotonPunteo = false;
                this.ctrBarraBotones1.BotonRehacer = false;
                this.ctrBarraBotones1.BotonSalir = false;
                this.ctrBarraBotones1.Location = new System.Drawing.Point(28, 24);
                this.ctrBarraBotones1.Name = "ctrBarraBotones1";
                this.ctrBarraBotones1.NumBotones = 4;
                this.ctrBarraBotones1.Size = new System.Drawing.Size(104, 26);
                this.ctrBarraBotones1.TabIndex = 1;
                this.ctrBarraBotones1.BGuardar += new System.EventHandler(this.ctrBarraBotones1_BGuardar);
                this.ctrBarraBotones1.BAbrir += new System.EventHandler(this.ctrBarraBotones1_BAbrir);
                this.ctrBarraBotones1.BPegar += new System.EventHandler(this.ctrBarraBotones1_BPegar);
                this.ctrBarraBotones1.BCopiar += new System.EventHandler(this.ctrBarraBotones1_BCopiar);
                // 
                // dataGridPorcentajes
                // 
                this.dataGridPorcentajes.BackgroundColor = System.Drawing.Color.Bisque;
                this.dataGridPorcentajes.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.dataGridPorcentajes.CaptionBackColor = System.Drawing.Color.DarkSalmon;
                this.dataGridPorcentajes.CaptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.dataGridPorcentajes.CaptionText = "  P O R C E N T A J E S";
                this.dataGridPorcentajes.DataMember = "";
                this.dataGridPorcentajes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
                this.dataGridPorcentajes.Location = new System.Drawing.Point(0, 52);
                this.dataGridPorcentajes.Name = "dataGridPorcentajes";
                this.dataGridPorcentajes.Size = new System.Drawing.Size(160, 335);
                this.dataGridPorcentajes.TabIndex = 0;
                this.dataGridPorcentajes.Paint += new System.Windows.Forms.PaintEventHandler(this.dataGridPorcentajes_Paint);
                this.dataGridPorcentajes.CurrentCellChanged += new System.EventHandler(this.dataGridPorcentajes_CurrentCellChanged);
                // 
                // ControlPorcentajes
                // 
                this.BackColor = System.Drawing.Color.Bisque;
                this.Controls.Add(this.comboValoraciones);
                this.Controls.Add(this.ctrBarraBotones1);
                this.Controls.Add(this.dataGridPorcentajes);
                this.Name = "ControlPorcentajes";
                this.Size = new System.Drawing.Size(160, 417);
                ((System.ComponentModel.ISupportInitialize)(this.dataGridPorcentajes)).EndInit();
                this.ResumeLayout(false);

			}
		#endregion

		private void ctrBarraBotones1_BAbrir(object sender, EventArgs e)
		{
			OpenFileDialog abreValIn = new OpenFileDialog();
			abreValIn.InitialDirectory = "Combinaciones\\" ;
			abreValIn.Filter = "A.Valoracion(*.txt)|*.txt|Todos los archivos(*.*)|*.*";
			if(abreValIn.ShowDialog() == DialogResult.OK) 
			{
			    _archivoPorcentajes=abreValIn.FileName;
				Porcentajes Pct = new Porcentajes(abreValIn.FileName);
				FormatoFicheroValoraciones=Pct.FormatoFichero ;
				switch (_FormatoFicheroValoraciones )
				{
					case 1:
					case 3:
					case 42:
						Valores=Pct.valores;
						comboValoraciones.Items.Clear();
						comboValoraciones.Visible =false;
						break;
					case 43:
						comboValoraciones.Visible =true;
						comboValoraciones.Items.Clear();
						comboValoraciones.Items.AddRange (Pct.DescripcionValoracion.ToArray()  ); 
						comboValoraciones.SelectedIndex = 0;
						break;
					case 44:
						comboValoraciones.Items.Clear();
						comboValoraciones.Visible =false;
						Pct.NombreFichero =_archivoPorcentajes ;//ArchivoHistoricoDeValoraciones;
						Pct.Temporada =_Temporada;
						Pct.Jornada =_Jornada;
						Pct.Leer ();
						Valores=Pct.valores;
						break;
				}
				OnModificado(EventArgs.Empty);
			}
		}
		public void Refresca()
		{
			Porcentajes Pct = new Porcentajes(_archivoPorcentajes);
			switch (Pct.FormatoFichero)
			{
				case 1:
				case 3:
				case 42:
					Valores=Pct.valores;
					comboValoraciones.Items.Clear();
					comboValoraciones.Visible =false;
					break;
				case 43:
					comboValoraciones.Visible =true;
					comboValoraciones.Items.Clear();
					comboValoraciones.Items.AddRange (Pct.DescripcionValoracion.ToArray()); 
					comboValoraciones.SelectedIndex = 0;
					break;
				case 44:
					comboValoraciones.Items.Clear();
					comboValoraciones.Visible =false;
					Pct.NombreFichero =_archivoPorcentajes ;//ArchivoHistoricoDeValoraciones;
					Pct.Temporada =_Temporada;
					Pct.Jornada =_Jornada;
					Pct.Leer ();
					Valores=Pct.valores;
					break;
			}
		}

		private void comboValoraciones_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(comboValoraciones.SelectedItem !=null)
			{
				string Descripcion = comboValoraciones.SelectedItem.ToString() ;
				Porcentajes Pct = new Porcentajes();
				Pct.NombreFichero =archivoPorcentajes;
				Pct.DescripcionBuscada = Descripcion;
				Pct.Leer ();
				Valores =Pct.valores;
				OnModificado(EventArgs.Empty);
			}
		}

		private void ctrBarraBotones1_BGuardar(object sender, System.EventArgs e)
		{
		    GuardarValoracionFrm valFrm = new GuardarValoracionFrm(Valores);
		    valFrm.Show();
		}

	    private void ctrBarraBotones1_BCopiar(object sender, EventArgs e)
		{
			Porcentajes Pct = new Porcentajes(Valores);
			Pct.PonerPorcentajesEnElPortapapeles();
		}

		private void ctrBarraBotones1_BPegar(object sender, EventArgs e)
		{
			Porcentajes Pct = new Porcentajes(Valores);
			if (Pct.RecuperarPorcentajesDelPortapapeles ())
			{
				Valores=Pct.valores;
				_FormatoFicheroValoraciones=Pct.FormatoFichero ;
				comboValoraciones.SelectedIndex = -1;
				OnModificado(EventArgs.Empty);
			}
		}
		protected void OnModificado(EventArgs e)
		{
			if (Modificado != null)
				Modificado(this, e);
		}

		private void dataGridPorcentajes_CurrentCellChanged(object sender, EventArgs e)
		{
			OnModificado(EventArgs.Empty);
		}
	}
}

