using System;
using System.Windows.Forms;
using System.IO;
using System.Collections;

using Free1X2.UI.Controls;
using Free1X2.EntradaSalida;

namespace Free1X2.UI 
{
	public class ModificadorFrm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Button btnGrabar;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.RadioButton radioDistribucion1;
        private System.Windows.Forms.RadioButton radioDistribucion2;
		private System.Windows.Forms.Button abrir;
		private System.Windows.Forms.CheckBox checkOrdenar;
		private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.RadioButton radioDistribucion3;
		private System.Windows.Forms.TextBox fichero;
		
		protected int opcion;
		protected int apuestas;
        protected int[,] frecuencia = new int[14, 3];
		protected string[] columnas;
        private GroupBox gbPartidos;
        private int noPartidos = 14;
		public ModificadorFrm()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
            PonerControles();
		}
        private void ReinicializarVariables()
        {
            frecuencia = new int[noPartidos, 3];
            PonerControles();
        }
        private void PonerControles()
        {
            this.gbPartidos.Controls.Clear();
            int x = 4;
            int y = 15;
            for (int i = 1; i <= noPartidos; i++)
            {
                ModificadorOptions m = new ModificadorOptions();
                m.Name = "partido" + i.ToString();
                m.NumeroPartido = i;
                m.Location = new System.Drawing.Point(x, y);
                this.gbPartidos.Controls.Add(m);
                y += m.Size.Height + 1;
            }
        }
        protected void PonerPorcentajes(string nombreArchivo)
		{
            IArchivoColumnas archivo = new ArchivoColumnasTexto(nombreArchivo);
			// Calcula las apuestas que contiene el fichero
			apuestas=Convert.ToInt32(archivo.ObtenNumCols());
			// Abre el fichero y pasa las columnas a un array
			LeeFichero(nombreArchivo);
			AccionModificador(AccionControlModificador.Escribir);
		}
		
		protected void AccionModificador(AccionControlModificador accion)
		{
			int numeroPartido=0;
            for (int i = 0; i < this.gbPartidos.Controls.Count; i++)
            {
                ModificadorOptions partidoGenerico = (ModificadorOptions)this.gbPartidos.Controls[i];

                numeroPartido = partidoGenerico.NumeroPartido - 1;
                switch (accion)
                {
                    case AccionControlModificador.Modificar:
                        if (partidoGenerico.PartidoActivo)
                        {
                            ModificarFrecuencia(partidoGenerico, numeroPartido);
                        }
                        break;
                    case AccionControlModificador.Escribir:
                        partidoGenerico.Valor_1 = Convert.ToString(frecuencia[numeroPartido, 0]);
                        partidoGenerico.Valor_X = Convert.ToString(frecuencia[numeroPartido, 1]);
                        partidoGenerico.Valor_2 = Convert.ToString(frecuencia[numeroPartido, 2]);
                        break;
                }
            }
		}
				
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModificadorFrm));
            this.fichero = new System.Windows.Forms.TextBox();
            this.radioDistribucion3 = new System.Windows.Forms.RadioButton();
            this.btnOk = new System.Windows.Forms.Button();
            this.checkOrdenar = new System.Windows.Forms.CheckBox();
            this.abrir = new System.Windows.Forms.Button();
            this.radioDistribucion2 = new System.Windows.Forms.RadioButton();
            this.radioDistribucion1 = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnGrabar = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.gbPartidos = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fichero
            // 
            this.fichero.BackColor = System.Drawing.SystemColors.Control;
            this.fichero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fichero.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fichero.Location = new System.Drawing.Point(16, 7);
            this.fichero.Multiline = true;
            this.fichero.Name = "fichero";
            this.fichero.ReadOnly = true;
            this.fichero.Size = new System.Drawing.Size(352, 30);
            this.fichero.TabIndex = 0;
            // 
            // radioDistribucion3
            // 
            this.radioDistribucion3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDistribucion3.Location = new System.Drawing.Point(29, 77);
            this.radioDistribucion3.Name = "radioDistribucion3";
            this.radioDistribucion3.Size = new System.Drawing.Size(125, 30);
            this.radioDistribucion3.TabIndex = 2;
            this.radioDistribucion3.Text = "Ordenado";
            this.radioDistribucion3.CheckedChanged += new System.EventHandler(this.radioDistribucion3_CheckedChanged);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOk.Enabled = false;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(240, 213);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(128, 29);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "Ok";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.BtnOkClick);
            // 
            // checkOrdenar
            // 
            this.checkOrdenar.Checked = true;
            this.checkOrdenar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkOrdenar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkOrdenar.Location = new System.Drawing.Point(29, 120);
            this.checkOrdenar.Name = "checkOrdenar";
            this.checkOrdenar.Size = new System.Drawing.Size(125, 29);
            this.checkOrdenar.TabIndex = 3;
            this.checkOrdenar.Text = "Ordenar signos";
            // 
            // abrir
            // 
            this.abrir.BackColor = System.Drawing.Color.LightSalmon;
            this.abrir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.abrir.Image = ((System.Drawing.Image)(resources.GetObject("abrir.Image")));
            this.abrir.Location = new System.Drawing.Point(369, 7);
            this.abrir.Name = "abrir";
            this.abrir.Size = new System.Drawing.Size(24, 30);
            this.abrir.TabIndex = 1;
            this.abrir.UseVisualStyleBackColor = false;
            this.abrir.Click += new System.EventHandler(this.AbrirClick);
            // 
            // radioDistribucion2
            // 
            this.radioDistribucion2.Checked = true;
            this.radioDistribucion2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDistribucion2.Location = new System.Drawing.Point(29, 48);
            this.radioDistribucion2.Name = "radioDistribucion2";
            this.radioDistribucion2.Size = new System.Drawing.Size(117, 26);
            this.radioDistribucion2.TabIndex = 1;
            this.radioDistribucion2.TabStop = true;
            this.radioDistribucion2.Text = "Proporcional";
            this.radioDistribucion2.CheckedChanged += new System.EventHandler(this.radioDistribucion2_CheckedChanged);
            // 
            // radioDistribucion1
            // 
            this.radioDistribucion1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDistribucion1.Location = new System.Drawing.Point(29, 17);
            this.radioDistribucion1.Name = "radioDistribucion1";
            this.radioDistribucion1.Size = new System.Drawing.Size(117, 28);
            this.radioDistribucion1.TabIndex = 0;
            this.radioDistribucion1.Text = "Aleatorio";
            this.radioDistribucion1.CheckedChanged += new System.EventHandler(this.radioDistribucion1_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(240, 273);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 29);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // btnGrabar
            // 
            this.btnGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrabar.Image = ((System.Drawing.Image)(resources.GetObject("btnGrabar.Image")));
            this.btnGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGrabar.Location = new System.Drawing.Point(240, 243);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(128, 29);
            this.btnGrabar.TabIndex = 18;
            this.btnGrabar.Text = "Grabar";
            this.btnGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGrabar.UseVisualStyleBackColor = false;
            this.btnGrabar.Click += new System.EventHandler(this.BtnGrabarClick);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.checkOrdenar);
            this.groupBox.Controls.Add(this.radioDistribucion3);
            this.groupBox.Controls.Add(this.radioDistribucion2);
            this.groupBox.Controls.Add(this.radioDistribucion1);
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox.Location = new System.Drawing.Point(222, 44);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(171, 158);
            this.groupBox.TabIndex = 16;
            this.groupBox.TabStop = false;
            this.groupBox.Text = " Distribución de los signos ";
            // 
            // gbPartidos
            // 
            this.gbPartidos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPartidos.ForeColor = System.Drawing.Color.Maroon;
            this.gbPartidos.Location = new System.Drawing.Point(16, 44);
            this.gbPartidos.Name = "gbPartidos";
            this.gbPartidos.Size = new System.Drawing.Size(200, 416);
            this.gbPartidos.TabIndex = 22;
            this.gbPartidos.TabStop = false;
            this.gbPartidos.Text = "Partidos";
            // 
            // ModificadorFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(405, 472);
            this.Controls.Add(this.gbPartidos);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnGrabar);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.abrir);
            this.Controls.Add(this.fichero);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModificadorFrm";
            this.Text = "Modificador de columnas";
            this.Load += new System.EventHandler(this.ModificadorFrmLoad);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}		
		void AbrirClick(object sender, System.EventArgs e)
		{
			string archivoEntrada;
			OpenFileDialog abreFicheroDialog = new OpenFileDialog();

			abreFicheroDialog.InitialDirectory = "Columnas\\" ;
			abreFicheroDialog.Filter = "Columnas(*.txt)|*.txt|Todos (*.*)|*.*" ;
			if(abreFicheroDialog.ShowDialog() == DialogResult.OK)
		    {		
    	        //Averiguamos el número de partidos
                IArchivoColumnas aCol = new ArchivoColumnasTexto(abreFicheroDialog.FileName);
                noPartidos = aCol.ObtenNumSignos();
                ReinicializarVariables();

		    	archivoEntrada = abreFicheroDialog.FileName;		    	
		    	fichero.Text = Path.GetFileName(archivoEntrada);
				btnOk.Enabled = true;
				PonerPorcentajes(abreFicheroDialog.FileName);
				// Habilitamos los controles.
				ModificadorOptions partidoGenerico;
				for(int i = 0; i < this.Controls.Count; i++)
				{
					partidoGenerico = this.Controls[i] as ModificadorOptions;
					if(partidoGenerico != null)
					{
						partidoGenerico.Enabled=true;
					}
				}
		
			}
		}
		
		void ModificadorFrmLoad(object sender, System.EventArgs e)
		{
			opcion=2;
		}
		

		private void radioDistribucion2_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioDistribucion2.Checked==true)
			{
				opcion=2;
				checkOrdenar.Enabled=true;
			}
			else
			{
				checkOrdenar.Enabled=false;
			}
		}
		
		private void radioDistribucion1_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioDistribucion1.Checked==true)
			{
				opcion=1;
			}
		}

		private void radioDistribucion3_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioDistribucion3.Checked==true)
			{
				opcion=3;
				checkOrdenar.Enabled=true;
			}
			else
			{
				checkOrdenar.Enabled=false;
			}
		}
		void BtnGrabarClick(object sender, System.EventArgs e)
		{
			// Graba el archivo.
			string tmp=fichero.Text;
			string nuevoFichero;
			int posicion=0;
			posicion=tmp.IndexOf('.');
			nuevoFichero=tmp.Substring(0,posicion) + "_modificado.txt";
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.FileName=nuevoFichero;
			saveDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(saveDialog.ShowDialog() == DialogResult.OK)
			{
				GuardaFichero( saveDialog.FileName);
				MessageBox.Show("Se ha grabado el archivo.","Guardar columnas",MessageBoxButtons.OK ,MessageBoxIcon.Information );
			}
		}
		
		void BtnCancelClick(object sender, System.EventArgs e)
		{
			this.Close();
		}
		
		void ModificarFrecuencia(ModificadorOptions partido, int numPartido)
		{
			char[] signo;
			signo=new char[3];
			double pctSuma;
			double[] pct=new double[3];
			int[] inicial=new int[3];
			int[] total=new int[3];
			int[] grupo=new int[3];
			int numColumna;
			int aleatorio,i;
			string temporal;
			// Lee el valor del campo
			if (partido.Valor_1.Length==0 || partido.Valor_X.Length==0 || partido.Valor_2.Length==0)
			{
				MessageBox.Show("La valoración de algún signo del partido " + (numPartido+1) + " está vacía.", "ATENCION!!", MessageBoxButtons.OK , MessageBoxIcon.Hand , MessageBoxDefaultButton.Button1);
				return;
			}
			pct[0]=Convert.ToDouble (partido.Valor_1);
			pct[1]=Convert.ToDouble (partido.Valor_X);
			pct[2]=Convert.ToDouble (partido.Valor_2);
			// Comprueba que sume 100, si no, convierte a %
			pctSuma=pct[0]+pct[1]+pct[2];
			if (pctSuma!=100)
			{
				if(MessageBox.Show("La valoración del partido " + (numPartido+1) + " no suma 100. ¿Continuar?", "ATENCION!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)==DialogResult.Yes )
				{
					pct[0]*=100/pctSuma;
					pct[1]*=100/pctSuma;
					pct[2]*=100/pctSuma;
				}
				else
				{
					return;
				}
			}
			// Establece el máximo de columnas con cada signo
			inicial[0]=Convert.ToInt32 (Convert.ToDouble (apuestas)*pct[0]/100);
			inicial[1]=Convert.ToInt32 (Convert.ToDouble (apuestas)*pct[1]/100);
			inicial[2]=Convert.ToInt32 (Convert.ToDouble (apuestas)*pct[2]/100);
			// Previene que la suma sea diferente al nº de apuestas.
			while ((inicial[0]+inicial[1]+inicial[2])<apuestas)
			{
				inicial[0]++;
			}
			while ((inicial[0]+inicial[1]+inicial[2])>apuestas)
			{
				inicial[2]--;
			}
			// ordena los signos según su aparición.
			// Lo hacemos antes de comprobar si queremos ordenación porque luego
			// necesitaremos saber qué signo tiene menor porcentaje.
			temporal=ordenar(pct);
			// Establece el orden de los signos según si deseamos aparición o no
			if (checkOrdenar.Checked==true && opcion>1)
			{
				// cambia el orden de los signos
				signo[0]=temporal[0];
				signo[1]=temporal[1];
				signo[2]=temporal[2];
				// asigna el total según el signo ordenado
				for (i=0;i<3;i++)
				{
					switch(signo[i])
					{
						case '1':
							total[i]=inicial[0];
							break;
						case 'X':
							total[i]=inicial[1];
							break;
						case '2':
							total[i]=inicial[2];
							break;
					}
				}
			}
			else
			{
				// no se ordena
				signo[0]='1';
				signo[1]='X';
				signo[2]='2';
				// asigna el nº de apuestas
				for (i=0;i<3;i++)
				{
					total[i]=inicial[i];
				}
			}


			switch (opcion)
			{
				case 1:
					signo[0]='1';
					signo[1]='X';
					signo[2]='2';
					for (numColumna=0;numColumna<apuestas;numColumna++)
					{
					calcularAleatorio:
						aleatorio=Convert.ToInt32( Math.Pow(System.DateTime.Now.Millisecond,3)) % 3;
						if(total[aleatorio]==0)
						{
							goto calcularAleatorio;
						}
						columnas[numColumna]=cambiarSigno(columnas[numColumna],numPartido,signo[aleatorio]);
						total[aleatorio]--;
					}
					break;
				case 2:
					// buscamos el partido de menor aparición para establecer la relación
					// por unidad respecto a los demás partidos.
					for (i=0;i<3;i++)
					{
						if (total[2]==0)
						{
							if (total[1]==0)
							{
								grupo[i]=total[i];
							}
							else
							{
								grupo[i]=total[i]/total[1];
							}
						}
						else
						{
							grupo[i]=total[i]/total[2];
						}
					}
					procesarGrupos(total,grupo,signo,numPartido);
					break;
				case 3:
					for (i=0;i<3;i++)
					{
						grupo[i]=total[i];
					}
					procesarGrupos(total,grupo,signo,numPartido);
					break;
			}
		
			for (i=0;i<3;i++)
			{
				frecuencia[numPartido,i]=Convert.ToInt32(pct[i]);
			}
			partido.PartidoActivo=false;
		}

		protected void procesarGrupos( int[] total,int[] grupo,char[] signo, int numPartido)
		{
			int[] quedan=new int[3];
			int procesados=0;
			int i=0;
			while (procesados<apuestas)
			{
				for (i=0;i<3;i++)
				{
					quedan[i]=grupo[i];
					if (quedan[i]>total[i])
					{
						quedan[i]=total[i];
					}
					while (quedan[i]>0 && total[i]>0)
					{
						columnas[procesados]=cambiarSigno(columnas[procesados],numPartido,signo[i]);
						quedan[i]--;
						total[i]--;
						procesados++;
					}
				}
			}

		}

		protected string cambiarSigno(string columna,int partido,char signo)
		{
			string nuevoSigno="";
			string nuevaColumna="";
			nuevoSigno=Convert.ToString(signo);
			for(int i=0;i<noPartidos;i++)
			{
				if (i==partido)
				{
					nuevaColumna+=nuevoSigno;
				}
				else
				{
					nuevaColumna+=columna[i];
				}
			}
			return nuevaColumna;
		}

		protected string ordenar(double [] porcentaje)
		{
			// Ordena los 3 signos de mayor a menor y los devuelve en una cadena de texto
			// Se puede mejorar mediante una hashtable o una matriz multidimensional (signo,procentaje) y
			// simplemente ejecutar la orden matriz.sort pero aunque es mucho código es de rápida ejecución.
			char [] orden;
			orden=new Char[3];
			string ordenado;
			if ((porcentaje[0]>=porcentaje[1]) && (porcentaje[0]>=porcentaje[2]))
			{
				orden[0]='1';
				if (porcentaje[1]>=porcentaje[2])
				{ 
					orden[1]='X';
					orden[2]='2';
				}
				else
				{ 
					orden[1]='2';
					orden[2]='X';
				}
			}
			if ((porcentaje[1]>=porcentaje[0]) && (porcentaje[1]>=porcentaje[2]))
			{
				orden[0]='X';
				if (porcentaje[0]>=porcentaje[2])
				{ 
					orden[1]='1';
					orden[2]='2';
				}
				else
				{ 
					orden[1]='2';
					orden[2]='1';
				}
			}
			if ((porcentaje[2]>=porcentaje[1]) && (porcentaje[2]>=porcentaje[0]))
			{
				orden[0]='2';
				if (porcentaje[0]>=porcentaje[1])
				{ 
					orden[1]='1';
					orden[2]='X';
				}
				else
				{ 
					orden[1]='X';
					orden[2]='1';
				}
			}
			ordenado=String.Concat(orden[0],orden[1],orden[2]);
			return ordenado;
		}

		void BtnOkClick(object sender, System.EventArgs e)
		{
			AccionModificador(AccionControlModificador.Modificar);	// Modifica
			AccionModificador(AccionControlModificador.Escribir);	// Reescribe las frecuencias
			if(MessageBox.Show("¿Grabar el fichero con los cambios?", "Proceso concluido", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)==DialogResult.Yes )
			{
				BtnGrabarClick(sender,e);
			}
		}

		protected void LeeFichero( string archivoEntrada )
		{
			//Interfaz IArchivoColumnas y objeto AColumnas estan
			//definidos en Free1X2.EntradaSalida
			long contador=0;
			int i,j;
			char signo;
            IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoEntrada);						
			
			//reinicializar variables
			columnas=new string [apuestas];
			frecuencia=new int [noPartidos,3];

			while(archComb.SiguienteColumna() ) 
			{
				columnas[contador]= archComb.LeeColumnaSinComas();
				for(i=0;i<noPartidos;i++)
				{
					signo=(columnas[contador])[i];
					switch(signo)
					{
						case '1':
							frecuencia[i,0]++;
							break;
						case 'X':
							frecuencia[i,1]++;
							break;
						case '2':
							frecuencia[i,2]++;
							break;
					}
				}
				contador++;
			}
			archComb.Cerrar();
			// pasa la frecuencia a porcentajes
			for(i=0;i<noPartidos;i++)
			{
				for(j=0;j<3;j++)
				{
					frecuencia[i,j]=Convert.ToInt32( (frecuencia[i,j]*100/Convert.ToDouble (apuestas))+0.4);
				}
				// Comprueba que sumen 100.
				while ((frecuencia[i,0]+frecuencia[i,1]+frecuencia[i,2])<100)
				{
					frecuencia[i,0]++;
				}
				while ((frecuencia[i,0]+frecuencia[i,1]+frecuencia[i,2])>100)
				{
                    if (frecuencia[i, 2] > 0)
                    {
                        frecuencia[i, 2]--;
                    }
                    else
                    {
                        if (frecuencia[i, 1] > 0)
                        {
                            frecuencia[i, 1]--;
                        }
                    }
				}
			}
		}
		
		protected void GuardaFichero(string archivoSalida)
		{
            IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoSalida);
			archComb.GuardarTodasCols(columnas);
			archComb.Cerrar();			
		}

	}
}
