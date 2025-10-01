using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Descripción breve de Pronosticos.
	/// </summary>
	public class Pronosticos : UserControl
	{
		private Label lblTitulo;
		private PartidoBoleto partidoBoleto1;
		private PartidoBoleto partidoBoleto2;
		private PartidoBoleto partidoBoleto3;
		private PartidoBoleto partidoBoleto4;
		private PartidoBoleto partidoBoleto5;
		private PartidoBoleto partidoBoleto6;
		private PartidoBoleto partidoBoleto7;
		private PartidoBoleto partidoBoleto8;
		private PartidoBoleto partidoBoleto9;
		private PartidoBoleto partidoBoleto10;
		private PartidoBoleto partidoBoleto11;
		private PartidoBoleto partidoBoleto12;
		private PartidoBoleto partidoBoleto13;
		private PartidoBoleto partidoBoleto14;
		private PartidoBoleto partidoBoleto16;
		private PartidoBoleto partidoBoleto15;
		public TextBox txtNombre;

	    private int grupoPantalla;
		private int numPartidos=14;
        public List<bool[]> grupoPronosticos = new List<bool[]>();
		private string[] partBol;
		protected string[]equipos1;
		protected string[]equipos2;
		protected string appStartPath = "";
		protected int numPartidosActivos;
		protected bool guardarGrupo = true;
		private string[] pronosticos;
        private PartidoBoleto[] partidosBoleto;
		
		/// <summary> 
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components;

		public Pronosticos()
		{
            try
            {
                numPartidos = VariablesGlobales.NumeroPartidos;
            }
            catch
            {
                numPartidos = 14;
            }
			// Llamada necesaria para el Diseñador de formularios Windows.Forms.
			InitializeComponent();
			cargaControl();
            cargarEventosBoleto();
		}
        private void cargarEventosBoleto()
        {
            for (int i = 0; i < partidosBoleto.Length; i++ )
            {
                partidosBoleto[i].lblNumPartido.Click += PartidoBoleto_Click;
            }
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.partidoBoleto16 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto15 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto12 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto13 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto14 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto9 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto10 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto11 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto5 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto6 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto7 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto8 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto3 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto4 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto2 = new Free1X2.UI.Controls.PartidoBoleto();
            this.partidoBoleto1 = new Free1X2.UI.Controls.PartidoBoleto();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.DarkSalmon;
            this.lblTitulo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.Maroon;
            this.lblTitulo.Location = new System.Drawing.Point(2, 1);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(342, 24);
            this.lblTitulo.TabIndex = 61;
            this.lblTitulo.Text = "Grupo";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitulo.DoubleClick += new System.EventHandler(this.lblTitulo_DoubleClick);
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.Ivory;
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.txtNombre.Location = new System.Drawing.Point(2, 26);
            this.txtNombre.Multiline = true;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(342, 18);
            this.txtNombre.TabIndex = 76;
            // 
            // partidoBoleto16
            // 
            this.partidoBoleto16.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto16.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto16.EquipoCasa = "";
            this.partidoBoleto16.EquipoFuera = "";
            this.partidoBoleto16.IsEnabled = true;
            this.partidoBoleto16.Location = new System.Drawing.Point(2, 360);
            this.partidoBoleto16.Name = "partidoBoleto16";
            this.partidoBoleto16.NumPartido = 16;
            this.partidoBoleto16.Pronostico = "";
            this.partidoBoleto16.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto16.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto16.TabIndex = 78;
            // 
            // partidoBoleto15
            // 
            this.partidoBoleto15.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto15.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto15.EquipoCasa = "";
            this.partidoBoleto15.EquipoFuera = "";
            this.partidoBoleto15.IsEnabled = true;
            this.partidoBoleto15.Location = new System.Drawing.Point(2, 339);
            this.partidoBoleto15.Name = "partidoBoleto15";
            this.partidoBoleto15.NumPartido = 15;
            this.partidoBoleto15.Pronostico = "";
            this.partidoBoleto15.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto15.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto15.TabIndex = 77;
            // 
            // partidoBoleto12
            // 
            this.partidoBoleto12.BackColor = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto12.ColorBase = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto12.EquipoCasa = "";
            this.partidoBoleto12.EquipoFuera = "";
            this.partidoBoleto12.IsEnabled = true;
            this.partidoBoleto12.Location = new System.Drawing.Point(2, 276);
            this.partidoBoleto12.Name = "partidoBoleto12";
            this.partidoBoleto12.NumPartido = 12;
            this.partidoBoleto12.Pronostico = "";
            this.partidoBoleto12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto12.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto12.TabIndex = 75;
            // 
            // partidoBoleto13
            // 
            this.partidoBoleto13.BackColor = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto13.ColorBase = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto13.EquipoCasa = "";
            this.partidoBoleto13.EquipoFuera = "";
            this.partidoBoleto13.IsEnabled = true;
            this.partidoBoleto13.Location = new System.Drawing.Point(2, 297);
            this.partidoBoleto13.Name = "partidoBoleto13";
            this.partidoBoleto13.NumPartido = 13;
            this.partidoBoleto13.Pronostico = "";
            this.partidoBoleto13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto13.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto13.TabIndex = 74;
            // 
            // partidoBoleto14
            // 
            this.partidoBoleto14.BackColor = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto14.ColorBase = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto14.EquipoCasa = "";
            this.partidoBoleto14.EquipoFuera = "";
            this.partidoBoleto14.IsEnabled = true;
            this.partidoBoleto14.Location = new System.Drawing.Point(2, 318);
            this.partidoBoleto14.Name = "partidoBoleto14";
            this.partidoBoleto14.NumPartido = 14;
            this.partidoBoleto14.Pronostico = "";
            this.partidoBoleto14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto14.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto14.TabIndex = 73;
            // 
            // partidoBoleto9
            // 
            this.partidoBoleto9.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto9.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto9.EquipoCasa = "";
            this.partidoBoleto9.EquipoFuera = "";
            this.partidoBoleto9.IsEnabled = true;
            this.partidoBoleto9.Location = new System.Drawing.Point(2, 213);
            this.partidoBoleto9.Name = "partidoBoleto9";
            this.partidoBoleto9.NumPartido = 9;
            this.partidoBoleto9.Pronostico = "";
            this.partidoBoleto9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto9.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto9.TabIndex = 72;
            // 
            // partidoBoleto10
            // 
            this.partidoBoleto10.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto10.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto10.EquipoCasa = "";
            this.partidoBoleto10.EquipoFuera = "";
            this.partidoBoleto10.IsEnabled = true;
            this.partidoBoleto10.Location = new System.Drawing.Point(2, 234);
            this.partidoBoleto10.Name = "partidoBoleto10";
            this.partidoBoleto10.NumPartido = 10;
            this.partidoBoleto10.Pronostico = "";
            this.partidoBoleto10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto10.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto10.TabIndex = 71;
            // 
            // partidoBoleto11
            // 
            this.partidoBoleto11.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto11.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto11.EquipoCasa = "";
            this.partidoBoleto11.EquipoFuera = "";
            this.partidoBoleto11.IsEnabled = true;
            this.partidoBoleto11.Location = new System.Drawing.Point(2, 255);
            this.partidoBoleto11.Name = "partidoBoleto11";
            this.partidoBoleto11.NumPartido = 11;
            this.partidoBoleto11.Pronostico = "";
            this.partidoBoleto11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto11.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto11.TabIndex = 70;
            // 
            // partidoBoleto5
            // 
            this.partidoBoleto5.BackColor = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto5.ColorBase = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto5.EquipoCasa = "";
            this.partidoBoleto5.EquipoFuera = "";
            this.partidoBoleto5.IsEnabled = true;
            this.partidoBoleto5.Location = new System.Drawing.Point(2, 129);
            this.partidoBoleto5.Name = "partidoBoleto5";
            this.partidoBoleto5.NumPartido = 5;
            this.partidoBoleto5.Pronostico = "";
            this.partidoBoleto5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto5.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto5.TabIndex = 69;
            // 
            // partidoBoleto6
            // 
            this.partidoBoleto6.BackColor = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto6.ColorBase = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto6.EquipoCasa = "";
            this.partidoBoleto6.EquipoFuera = "";
            this.partidoBoleto6.IsEnabled = true;
            this.partidoBoleto6.Location = new System.Drawing.Point(2, 150);
            this.partidoBoleto6.Name = "partidoBoleto6";
            this.partidoBoleto6.NumPartido = 6;
            this.partidoBoleto6.Pronostico = "";
            this.partidoBoleto6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto6.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto6.TabIndex = 68;
            // 
            // partidoBoleto7
            // 
            this.partidoBoleto7.BackColor = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto7.ColorBase = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto7.EquipoCasa = "";
            this.partidoBoleto7.EquipoFuera = "";
            this.partidoBoleto7.IsEnabled = true;
            this.partidoBoleto7.Location = new System.Drawing.Point(2, 171);
            this.partidoBoleto7.Name = "partidoBoleto7";
            this.partidoBoleto7.NumPartido = 7;
            this.partidoBoleto7.Pronostico = "";
            this.partidoBoleto7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto7.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto7.TabIndex = 67;
            // 
            // partidoBoleto8
            // 
            this.partidoBoleto8.BackColor = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto8.ColorBase = System.Drawing.Color.AntiqueWhite;
            this.partidoBoleto8.EquipoCasa = "";
            this.partidoBoleto8.EquipoFuera = "";
            this.partidoBoleto8.IsEnabled = true;
            this.partidoBoleto8.Location = new System.Drawing.Point(2, 192);
            this.partidoBoleto8.Name = "partidoBoleto8";
            this.partidoBoleto8.NumPartido = 8;
            this.partidoBoleto8.Pronostico = "";
            this.partidoBoleto8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto8.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto8.TabIndex = 66;
            // 
            // partidoBoleto3
            // 
            this.partidoBoleto3.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto3.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto3.EquipoCasa = "";
            this.partidoBoleto3.EquipoFuera = "";
            this.partidoBoleto3.IsEnabled = true;
            this.partidoBoleto3.Location = new System.Drawing.Point(2, 87);
            this.partidoBoleto3.Name = "partidoBoleto3";
            this.partidoBoleto3.NumPartido = 3;
            this.partidoBoleto3.Pronostico = "";
            this.partidoBoleto3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto3.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto3.TabIndex = 65;
            // 
            // partidoBoleto4
            // 
            this.partidoBoleto4.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto4.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto4.EquipoCasa = "";
            this.partidoBoleto4.EquipoFuera = "";
            this.partidoBoleto4.IsEnabled = true;
            this.partidoBoleto4.Location = new System.Drawing.Point(2, 108);
            this.partidoBoleto4.Name = "partidoBoleto4";
            this.partidoBoleto4.NumPartido = 4;
            this.partidoBoleto4.Pronostico = "";
            this.partidoBoleto4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto4.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto4.TabIndex = 64;
            // 
            // partidoBoleto2
            // 
            this.partidoBoleto2.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto2.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto2.EquipoCasa = "";
            this.partidoBoleto2.EquipoFuera = "";
            this.partidoBoleto2.IsEnabled = true;
            this.partidoBoleto2.Location = new System.Drawing.Point(2, 66);
            this.partidoBoleto2.Name = "partidoBoleto2";
            this.partidoBoleto2.NumPartido = 2;
            this.partidoBoleto2.Pronostico = "";
            this.partidoBoleto2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto2.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto2.TabIndex = 63;
            // 
            // partidoBoleto1
            // 
            this.partidoBoleto1.BackColor = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto1.ColorBase = System.Drawing.Color.LemonChiffon;
            this.partidoBoleto1.EquipoCasa = "";
            this.partidoBoleto1.EquipoFuera = "";
            this.partidoBoleto1.IsEnabled = true;
            this.partidoBoleto1.Location = new System.Drawing.Point(2, 45);
            this.partidoBoleto1.Name = "partidoBoleto1";
            this.partidoBoleto1.NumPartido = 1;
            this.partidoBoleto1.Pronostico = "";
            this.partidoBoleto1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.partidoBoleto1.Size = new System.Drawing.Size(342, 20);
            this.partidoBoleto1.TabIndex = 62;
            this.partidoBoleto1.Click += new System.EventHandler(this.PartidoBoleto_Click);
            // 
            // Pronosticos
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.partidoBoleto16);
            this.Controls.Add(this.partidoBoleto15);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.partidoBoleto12);
            this.Controls.Add(this.partidoBoleto13);
            this.Controls.Add(this.partidoBoleto14);
            this.Controls.Add(this.partidoBoleto9);
            this.Controls.Add(this.partidoBoleto10);
            this.Controls.Add(this.partidoBoleto11);
            this.Controls.Add(this.partidoBoleto5);
            this.Controls.Add(this.partidoBoleto6);
            this.Controls.Add(this.partidoBoleto7);
            this.Controls.Add(this.partidoBoleto8);
            this.Controls.Add(this.partidoBoleto3);
            this.Controls.Add(this.partidoBoleto4);
            this.Controls.Add(this.partidoBoleto2);
            this.Controls.Add(this.partidoBoleto1);
            this.Controls.Add(this.lblTitulo);
            this.Name = "Pronosticos";
            this.Size = new System.Drawing.Size(347, 389);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public string NombreGrupo
		{
			get{ return txtNombre.Text;}
			set{ txtNombre.Text=value;}
		}

		public bool GuardarGrupo
		{
			get{ return guardarGrupo;}
			set{ guardarGrupo=value;}
		}

		public string[] ListaPronosticos
		{
			get{ return pronosticos;}
			set{ pronosticos=value;}
		}

		public int NumPartidos
		{
			get
            {
                try
                {
                    numPartidos = VariablesGlobales.NumeroPartidos;
                }
                catch
                {
                    numPartidos = 16;
                }
                return numPartidos;
            }
			set
			{
				numPartidos=value;
				InicializarBoleto(value);
			}
		}

		public int NumPartidosActivos
		{
			get
			{
			    numPartidosActivos=0;
				for(int i=0;i<NumPartidos;i++)
				{
				    PartidoBoleto p = BuscarControl(i+1);
				    if(p.IsEnabled) numPartidosActivos++;
				}
			    return numPartidosActivos;
			}
		}

		private void cargaControl()
		{
			InicializaPartidosGrupos();
			InicializaPronosticos();
			partBol = new string[NumPartidos];
			pronosticos = new string[NumPartidos];
		}

		public string[] DevolverEquipos() 
		{
		    for (int i = 0; i < partidosBoleto.Length; i++)
		    {
		        PartidoBoleto p = partidosBoleto[i];

		        // Comprueba que no se asigne más allá de la longitud de la matriz
                if (p.NumPartido <= partBol.Length)
                {
                    partBol[p.NumPartido - 1] = p.EquipoCasa + "-" + p.EquipoFuera;
                }
		    }
		    return partBol;
		}

		public void ComprobarPartidosActivos()
		{
			MainForm f=(MainForm)ParentForm;
			if(f!=null)
			{
                f.groupBox.Enabled = !(NumPartidosActivos == 0);
			}
		}

		public void LeerBoletoBase() 
		{
		    OpenFileDialog abreFileIn = new OpenFileDialog();
			abreFileIn.InitialDirectory = "Combinaciones\\" ;
			abreFileIn.Filter = "Equipos(*.txt)|*.txt|Todos los archivos(*.*)|*.*" ;
			
			if(abreFileIn.ShowDialog() == DialogResult.OK)
			{
				string archivoBoleto = abreFileIn.FileName;
				
				StreamReader sr = new StreamReader(archivoBoleto, System.Text.Encoding.Default);
			    for (int nr=0; nr<NumPartidos; nr++) 
				{
					string partido = sr.ReadLine();
					if (partido==null) partido = "? - ?";
					partBol[nr] = partido;
				}
				sr.Close();
				SetEquipos(partBol);
			}			
		}
        public void LeerBoletoBase(string boleto)
        {
            string[] partidos = boleto.Split('#');
            int longitud;
            if (partidos.Length > partBol.Length)
            {
                longitud = partBol.Length;
            }
            else
            {
                longitud = partidos.Length;
            }
            try
            {
                for (int nr = 0; nr < longitud; nr++)
                {
                    partBol[nr] = partidos[nr].Replace(',','-');
                }
                SetEquipos(partBol);
            }
            catch
            {
            }
            
        }

		public void CrearArchivoBoleto()
		{
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.InitialDirectory = "Combinaciones\\" ;
			saveDialog.Filter = "Equipos(*.txt)|*.txt|Todos los archivos(*.*)|*.*" ;

			string[] equipos = DevolverEquipos();

		    if(saveDialog.ShowDialog() == DialogResult.OK)
			{	
				string nombreArchivoComb = saveDialog.FileName;

				StreamWriter sw = new StreamWriter(nombreArchivoComb, false, System.Text.Encoding.Default);
				

				foreach(string str in equipos)
				{
					sw.WriteLine(str);
				}

				sw.Close();			
			}		
		}

		public void SetEquipos( string[] partBol) 
		{
		    try
            {
                for (int i = 0; i < partidosBoleto.Length; i++)
                {
                    PartidoBoleto p = partidosBoleto[i];
                    // Comprueba que no se asigne más allá de la longitud de la matriz
                    if (p.NumPartido <= partBol.Length)
                    {
                        string[] partidos = partBol[p.NumPartido - 1].Split('-');
                        p.EquipoCasa = partidos[0].ToUpper();
                        p.EquipoFuera = partidos[1].ToUpper();
                    }
                }
            }
            catch
            {
                SetEquiposVacio();
                MessageBox.Show("El archivo de Boleto no es válido", "Error",MessageBoxButtons.OK,MessageBoxIcon.Warning );                
            }
		}
		
		public void SetEquiposVacio() 
		{
            for (int i = 0; i < partidosBoleto.Length; i++)
            {
                partidosBoleto[i].EquipoCasa = "";
                partidosBoleto[i].EquipoFuera = "";
            }
		}
		
		protected void InicializaPartidosGrupos()
		{
            partidosBoleto = new PartidoBoleto[] { partidoBoleto1, partidoBoleto2, partidoBoleto3, partidoBoleto4, partidoBoleto5, partidoBoleto6, partidoBoleto7, partidoBoleto8, partidoBoleto9, partidoBoleto10, partidoBoleto11, partidoBoleto12, partidoBoleto13, partidoBoleto14,partidoBoleto15, partidoBoleto16 };

			bool[] partidosGrupo = new bool[NumPartidos];
            for (int i = 0; i < NumPartidos; i++)
            {
                partidosGrupo[i] = true;
            }
			//poner partidos grupo base
			CrearGrupo( partidosGrupo );
		}

        protected int CrearGrupo(bool[] partidos)
        {
            grupoPronosticos.Add(partidos);
            int noGrupoPronostico = grupoPronosticos.Count - 1;
            if (noGrupoPronostico == 0)
            {
                grupoPantalla = 0;
            }
            return noGrupoPronostico;
        }

        public void ActivaTodosPartidos(bool valor)
        {
            if (valor)
            {
                ComprobarPartidosActivos();
            }

            for (int i = 0; i < partidosBoleto.Length; i++)
            {
                partidosBoleto[i].IsEnabled = valor;
            }
        }
				
		public int GrupoPantalla
		{
			get{ return grupoPantalla; }
            set
            {
                if (grupoPantalla < grupoPronosticos.Count && GuardarGrupo)
                    GuardaPartidosGrupoActivo();
                grupoPantalla = value;
                CompruebaGrupoExiste();
                CambiaGrupo();
                GuardarGrupo = true;
            }
		}

		public bool[] ObtenPartidosGrupo( int indexGrupo )
		{		
			//guardar partidos grupo en pantalla por si han cambiado.
			GuardaPartidosGrupoActivo();
			return grupoPronosticos[ indexGrupo ];		
		}
		
		public void ObtenPronosticos()
		{
			//Lee los pronósticos
		    for(int i=0;i<NumPartidos;i++)
		    {
		        PartidoBoleto p = BuscarControl(i+1);
		        pronosticos[i]=p.Pronostico;
		    }
		}
		
		public void BorrarPartidosGrupoActivo()
		{
			grupoPronosticos.Clear();				
		}
		
		public void PonerPartidosGrupoActivo( bool[] partidos )
		{
			CrearGrupo( partidos );		
		}

        protected void GuardaPartidosGrupoActivo()
        {
            bool[] partidos = grupoPronosticos[grupoPantalla];

            for (int i = 0; i < NumPartidos; i++)
            {
                if (partidosBoleto[i].BackColor != partidosBoleto[i].ColorBase)
                {
                    partidos[i] = false;
                }
                else
                {
                    partidos[i] = true;
                }
            }
        }
					
		protected void CompruebaGrupoExiste()
		{		
			if( (grupoPantalla + 1)  > grupoPronosticos.Count )			
			{
				bool[] partidosGrupo = new bool[NumPartidos];
				for(int i=0;i<NumPartidos;i++)
				{
					partidosGrupo[i] = false;
				}
				grupoPronosticos.Add( partidosGrupo );
			}		
		}

		protected void CambiaGrupo()
		{
			string txt="";
		    if( grupoPantalla == 0 )
			{
			    ActivaPronosticos(true);
				ActivaTodosPartidos(true);
				lblTitulo.Text = "Boleto Base";
				if(grupoPronosticos.Count >1)
				{
					int grupos = grupoPronosticos.Count -1;
					txt="(+" + grupos + " grupos)";
					txtNombre.Text=txt;
				}
				txtNombre.Enabled=false;
				txtNombre.Text=txt;
				txtNombre.Tag="G";
			}
			else
			{
			    ActivaPartidos( ObtenPartidosDesdeGrupo( grupoPantalla ) );
				lblTitulo.Text = ObtenTitulo();
				txtNombre.Enabled=true;
				txtNombre.Tag="";
			}
		}

		public void ActivaPartidos( bool[] partidos )
		{
			//desactiva primero todos los partidos
			ActivaTodosPartidos(false);
            for (int i = 0; i < partidosBoleto.Length; i++)
            {
                if (partidosBoleto[i].NumPartido <= partidos.Length)
                {
                    if (partidos[partidosBoleto[i].NumPartido - 1])
                    {
                        partidosBoleto[i].IsEnabled = true;
                    }
                }
            }
		}		
		
		protected bool[] ObtenPartidosDesdeGrupo( int noGrupo )
		{
			return grupoPronosticos[ noGrupo ];					
		}

		public string ObtenTitulo()
		{		
			int numeroGruposPartidos = grupoPronosticos.Count -1;
			return "Grupo " + grupoPantalla + "/" + numeroGruposPartidos;
		}
        public void Reiniciar14Triples()
        {
            InicializaPronosticos();
        }

        protected void InicializaPronosticos()
        {
            for (int i = 0; i < partidosBoleto.Length; i++)
            {
                partidosBoleto[i].Pronostico = "1X2";
            }
        }

        public void ActivaPronosticos(bool valor)
        {
            for (int i = 0; i < partidosBoleto.Length; i++)
            {
                partidosBoleto[i].IsEnabled = valor;
            }
        }
				
		private void lblTitulo_DoubleClick(object sender, EventArgs e)
		{
            if (grupoPantalla != 0)
            {
                //activar/desactivar todos dependiendo de si hay partidos activos
                bool[] partidos = ObtenPartidosGrupo(grupoPantalla);
                bool activarPartidos = true;
                for (int i = 0; i < partidos.Length; i++)
                {
                    if (partidos[i])
                    {
                        activarPartidos = false;
                        break;
                    }
                }

                ActivaTodosPartidos(activarPartidos);
                ComprobarPartidosActivos();
            }
            else
            {
                InicializaPronosticos();
            }
		}

		public string this[int index]
		{
			get
			{
			    return GetPronostico(index);
			}
		    set
			{
				SetPronostico( index, value );
			}
		}

		protected void SetPronostico( int index, string strPronostico )
		{
			// Buscamos el partido cuyo nº coincida con el índice
            for (int i = 0; i < partidosBoleto.Length; i++)
            {
                if (index == partidosBoleto[i].NumPartido)
                {
                    // Este es nuestro partido!
                    partidosBoleto[i].Pronostico = strPronostico;
                }
            }
		}

		protected string GetPronostico(int index)
		{
			string strPronostico = "";
			// Buscamos el partido cuyo nº coincida con el índice

            string tmp = BuscarControl(index).Pronostico;
			for(int i=0;i<tmp.Length-1;i++)
			{
				strPronostico+=tmp.Substring(i,1)+",";
			}
            try
            {
                strPronostico += tmp.Substring(tmp.Length - 1, 1);
            }
            catch
            {
                strPronostico = "";
            }
			return strPronostico;	
		}

		public PartidoBoleto BuscarControl(int numPartido)
		{
		    for (int i = 0; i < partidosBoleto.Length; i++)
            {
                if (numPartido == partidosBoleto[i].NumPartido)
                {
                    // Este es nuestro partido!
                    return partidosBoleto[i];
                }
            }
			return null;
		}

		private void InicializarBoleto(int nPartidos)
		{
			// Ponemos un separador estandar para 16 partidos.
			// Este método sólo debiera ser llamado al modificar el nº de partidos
			// de forma interna. Para hacerlo desde otro módulo, hay que llamar al método
			// incluyendo el separador.
			string[] separador={"5","9","13"};
			InicializarBoleto(nPartidos, separador);
		}

		public void InicializarBoleto(int nPartidos, string[] separador)
		{
			numPartidos=nPartidos;
			partBol = new string[NumPartidos];
			int numColor=0;
			Color[] colores={Color.LemonChiffon, Color.AntiqueWhite};
			// Establecemos la posición de los controles
			// El primer partido está en una posición fija.
			PartidoBoleto p;
			PartidoBoleto pAnt = BuscarControl(1);
			for(int i=2;i<=16;i++)
			{
			    p=BuscarControl(i);
				p.Location=new Point(2,pAnt.Location.Y+21);
				pAnt=p;
			}
			// Cambiamos el color base tras cada separador.
			for(int j=0;j<separador.Length;j++)
			{
				numColor--;
				numColor=Math.Abs(numColor);
				for(int i=Convert.ToInt16(separador[j]);i<=16;i++)
				{
				    p=BuscarControl(i);
					p.ColorBase=colores[numColor];
					p.BackColor=p.ColorBase;
				}
			}
			// Ponemos como no visibles los controles innecesarios
			for(int i=NumPartidos+1;i<=16;i++)
			{
			    p=BuscarControl(i);
				p.Visible=false;
			}
			// Establecemos la altura del control pronóstico
		    p=BuscarControl(NumPartidos);
			Size=new Size(Width,p.Location.Y+22);
		}

        private void PartidoBoleto_Click(object sender, EventArgs e)
        {
            if (GrupoPantalla != 0)
            {
                ComprobarPartidosActivos();
            }
        }
	}
}
