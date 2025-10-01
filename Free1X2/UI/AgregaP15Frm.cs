// created on 28/8/2005
// Modificado 11-4-06
// Free1X2 : Programa de quinielas "libre"
//
// Copyright (C) 2005 Indeciso JJchild@lycos.co.uk
// (Idea de eliminar repetidas sacada de codigo Luis Fernandez)
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
using System.IO;
using System.Collections;
using Free1X2.EntradaSalida;
using System.Windows.Forms;
using Free1X2.Utils;


namespace Free1X2.UI
{
	/// <summary>
	/// Description of AgregaP15Frm.	
	/// </summary>
	public class AgregaP15Frm : Form
	{
		private Label label12;
		private TextBox textBox4;
		private Button button1;
		private TextBox textBox5;
		private TextBox textBox3;
		private CheckBox checkBox4;
		private TextBox textBox6;
		private TextBox textBox1;
		private Button btnFileIn;
		private TextBox textBox2;
		private Label label10;
		private Label label11;
		private CheckBox checkBox3;
		private CheckBox checkBox1;
		private Button btnFileOut;
		private Label label3;
		private Label label2;
		private Label label1;
		private CheckBox checkBox2;
		private Label label7;
		private Label label6;
		private Label label5;
		private Label label4;
		private Label label9;
		private Label label8;		
		private int noColumnasFinal;
		private int noColsRepetidas;

        private BitArray Bits = new BitArray(4782969, false);		
		
		public AgregaP15Frm()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		private string archivoFinal = "";
		private string archivoTemp = "";			
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgregaP15Frm));
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnFileOut = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btnFileIn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(292, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 21);
            this.label8.TabIndex = 23;
            this.label8.Text = "Si no P15 =";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(40, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 16);
            this.label9.TabIndex = 25;
            this.label9.Text = "Elimina partido 15";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Cornsilk;
            this.label4.Location = new System.Drawing.Point(0, 296);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(400, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Preparado";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(40, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 21);
            this.label5.TabIndex = 17;
            this.label5.Text = "Si partido";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(134, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 21);
            this.label6.TabIndex = 19;
            this.label6.Text = "=";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(174, 168);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 21);
            this.label7.TabIndex = 21;
            this.label7.Text = "entonces P15 =";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox2
            // 
            this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.Location = new System.Drawing.Point(24, 172);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(16, 16);
            this.checkBox2.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(142, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 32);
            this.label1.TabIndex = 10;
            this.label1.Text = "Falta fichero entrada";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(142, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(240, 32);
            this.label2.TabIndex = 7;
            this.label2.Text = "Falta fichero salida";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(40, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 21);
            this.label3.TabIndex = 12;
            this.label3.Text = "Partido 15 fijo a";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFileOut
            // 
            this.btnFileOut.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFileOut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileOut.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileOut.Image = ((System.Drawing.Image)(resources.GetObject("btnFileOut.Image")));
            this.btnFileOut.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileOut.Location = new System.Drawing.Point(8, 48);
            this.btnFileOut.Name = "btnFileOut";
            this.btnFileOut.Size = new System.Drawing.Size(128, 32);
            this.btnFileOut.TabIndex = 6;
            this.btnFileOut.Text = "Fichero salida";
            this.btnFileOut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileOut.UseVisualStyleBackColor = false;
            this.btnFileOut.Click += new System.EventHandler(this.BtnFileOutClick);
            // 
            // checkBox1
            // 
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(24, 138);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(16, 16);
            this.checkBox1.TabIndex = 15;
            // 
            // checkBox3
            // 
            this.checkBox3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3.Location = new System.Drawing.Point(24, 104);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(16, 16);
            this.checkBox3.TabIndex = 27;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(321, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 16);
            this.label11.TabIndex = 29;
            this.label11.Text = "0";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(168, 104);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(147, 16);
            this.label10.TabIndex = 28;
            this.label10.Text = "Nº Columnas Repetidas:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(112, 168);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(16, 21);
            this.textBox2.TabIndex = 18;
            // 
            // btnFileIn
            // 
            this.btnFileIn.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFileIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileIn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileIn.Image = ((System.Drawing.Image)(resources.GetObject("btnFileIn.Image")));
            this.btnFileIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileIn.Location = new System.Drawing.Point(8, 8);
            this.btnFileIn.Name = "btnFileIn";
            this.btnFileIn.Size = new System.Drawing.Size(128, 32);
            this.btnFileIn.TabIndex = 9;
            this.btnFileIn.Text = "Fichero entrada";
            this.btnFileIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileIn.UseVisualStyleBackColor = false;
            this.btnFileIn.Click += new System.EventHandler(this.BtnFileInClick);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(151, 133);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(16, 21);
            this.textBox1.TabIndex = 13;
            // 
            // textBox6
            // 
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(362, 206);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(24, 21);
            this.textBox6.TabIndex = 32;
            // 
            // checkBox4
            // 
            this.checkBox4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox4.Location = new System.Drawing.Point(24, 206);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(16, 24);
            this.checkBox4.TabIndex = 30;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(152, 168);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(16, 21);
            this.textBox3.TabIndex = 20;
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox5.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(370, 168);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(16, 21);
            this.textBox5.TabIndex = 24;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(291, 248);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 32);
            this.button1.TabIndex = 8;
            this.button1.Text = "Calcular";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // textBox4
            // 
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox4.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(274, 168);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(16, 21);
            this.textBox4.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(40, 206);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(315, 21);
            this.label12.TabIndex = 31;
            this.label12.Text = "Resultado partido 15 igual que resultado del partido";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AgregaP15Frm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(400, 310);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFileIn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnFileOut);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AgregaP15Frm";
            this.Text = "Añade P15 (Indeciso)";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


        protected ArchivoColumnasTexto InicializaArchivoCols(string combFile)
		{
            return new ArchivoColumnasTexto(combFile);			
		}
				
		void BtnFileOutClick(object sender, EventArgs e)
		{
			SaveFileDialog abreFileOut = new SaveFileDialog();
			abreFileOut.InitialDirectory = Application.StartupPath + "/Columnas/" ;
			abreFileOut.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			
			if(abreFileOut.ShowDialog() == DialogResult.OK)
			{
				archivoFinal = abreFileOut.FileName;
				label2.Text = Path.GetFileName(archivoFinal);
			}			
		}
		
		void BtnFileInClick(object sender, EventArgs e)
		{
			OpenFileDialog abreTemp = new OpenFileDialog();
			abreTemp.InitialDirectory = Application.StartupPath + "/Columnas/" ;
			abreTemp.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(abreTemp.ShowDialog() == DialogResult.OK)
		    {
		    	archivoTemp = abreTemp.FileName;
		    	label1.Text = Path.GetFileName(archivoTemp);
    		}
		}
        private string ComprobarEntradas()
        {
            string error = "";

            if ((archivoFinal == "") || (archivoTemp == ""))
            {
                error = "Uno de los archivos necesarios no ha sido especificado\n";
            }
            else
            {
                if (archivoFinal == archivoTemp)
                {
                    error = "El archivo de salida debe ser distinto al de entrada\n";
                }
            }

            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked)
            {
                error += "No se ha especificado qué hacer con el Pleno al 15";
            }
            return error;
        }
		
		void Button1Click(object sender, EventArgs e)
		{
			string columna;
			int tmp;

		    int i;

		    int ContX;
			int Cont2;
			
			string [] Buffer1 = new string [8];
			string [] BufferX = new string [8];
			string [] Buffer2 = new string [8];

		    noColumnasFinal = 0;
			noColsRepetidas = 0;
			label11.Text = "0";
			
			for ( i=0;i<8;i++)
			{
				Buffer1[i] = "";
				BufferX[i] = "";
				Buffer2[i] = "";
			}
			
			int Cont1 = ContX = Cont2 = 0;
            string error = ComprobarEntradas();

			if (error != "")
			{
				MessageBox.Show (error,"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
			label4.Text = "Calculando";
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoTemp);

            IArchivoColumnas sw = new ArchivoColumnasTexto(archivoFinal);
			Bits.SetAll (false);
			ConvertidorDeBases conv= new ConvertidorDeBases();
			while( comBaseCols.SiguienteColumna() )
				{
					columna= comBaseCols.LeeColumnaSinComas();
					
	    	    	// abrir fichero
				
					if (checkBox1.Checked) 
					{
						sw.GuardarCols(columna + textBox1.Text);
					}
					else
		    		{
		    		    if (checkBox2.Checked) 
					    {
		    			    tmp = Int32.Parse(textBox2.Text);
		    			    tmp = tmp -1;
		    			    char tmp2 = char.Parse (textBox3.Text);
		    			    if (columna[tmp] == tmp2)
		    			    {
		    				    columna = columna + textBox4.Text;
		    			    }
						    else
						    {
							    columna = columna + textBox5.Text;
						    }
						    switch (columna.Substring (14,1))
						    {
						    case "1":
							    Buffer1[Cont1] = columna;
							    Cont1++;
							    break;
						    case "x":
						    case "X":							
							    BufferX[ContX] = columna;
							    ContX++;
							    break;
						    case "2":
							    Buffer2[Cont2] = columna;
							    Cont2++;
							    break;
						    }
						    if (Cont1== 8)
						    {
							    for ( i=0;i<8;i++)
							    {
								    sw.GuardarCols(Buffer1[i]);
							    }
							    Cont1=0;
						    }
						    else if (ContX== 8)
						    {
							    for ( i=0;i<8;i++)
							    {
								    sw.GuardarCols(BufferX[i]);
							    }
							    ContX=0;
						    }
						    else if (Cont2== 8)
						    {
							    for ( i=0;i<8;i++)
							    {
								    sw.GuardarCols(Buffer2[i]);
							    }
							    Cont2=0;
						    }
		    		    }
		    		    if (checkBox3.Checked) 
					    {
    		    			
		    			    columna = columna.Substring (0,14);
		    			    int Num = conv.ConvColumnaANumero  (columna);
		    			    //si columna no está marcada, columna no repetida
						    if( Bits[Num]==false )
						    {
							    Bits[Num]=true;
    					
							    //grabar columna a archivo
							    sw.GuardarCols( columna );
    					
							    noColumnasFinal++;
						    }
						    else
						    {
							    //columna repetida
							    noColsRepetidas++;
						    }
		    		    }
		    		    if (checkBox4.Checked) 
					    {
		    			    tmp = Int32.Parse(textBox6.Text);
		    			    tmp = tmp -1;
    		    			
						    columna = columna + columna[tmp];
    						
						    switch (columna.Substring (14,1))
						    {
						    case "1":
							    Buffer1[Cont1] = columna;
							    Cont1++;
							    break;
						    case "x":
						    case "X":							
							    BufferX[ContX] = columna;
							    ContX++;
							    break;
						    case "2":
							    Buffer2[Cont2] = columna;
							    Cont2++;
							    break;
						    }
						    if (Cont1== 8)
						    {
							    for ( i=0;i<8;i++)
							    {
								    sw.GuardarCols(Buffer1[i]);
							    }
							    Cont1=0;
						    }
						    else if (ContX== 8)
						    {
							    for ( i=0;i<8;i++)
							    {
								    sw.GuardarCols(BufferX[i]);
							    }
							    ContX=0;
						    }
						    else if (Cont2== 8)
						    {
							    for ( i=0;i<8;i++)
							    {
								    sw.GuardarCols(Buffer2[i]);
							    }
							    Cont2=0;
						    }
		    		    }
		   			}
				}
                comBaseCols.Cerrar();
								
				if (Cont1 != 0)
						{
							for ( i=0;i<Cont1;i++)
							{
								sw.GuardarCols(Buffer1[i]);
							}
							
						}
				
				if (ContX != 0)
						{
							for ( i=0;i<ContX;i++)
							{
								sw.GuardarCols(BufferX [i]);
							}
							
						}
				if (Cont2 != 0)
						{
							for ( i=0;i<Cont2;i++)
							{
								sw.GuardarCols(Buffer2[i]);
							}
						}
					
				
				sw.Cerrar();
				label11.Text = noColsRepetidas.ToString ();
				label4.Text = "Terminado";
		    }
		}
		
	}
}

