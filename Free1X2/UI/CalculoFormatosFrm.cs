using System;
using System.IO;
using System.Windows.Forms;
using Free1X2.Utils;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Description of CalculoFormatosFrm.	
	/// </summary>
    public class CalculoFormatosFrm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnFileOut;
        private System.Windows.Forms.Label label1;

        private int Num1X = 0;
        private int Num12 = 0;
        private int NumX2 = 0;
        private int Num11 = 0;
        private int NumXX = 0;
        private int Num22 = 0;
        private int Num1V = 0;
        private int NumXV = 0;
        private int Num2V = 0;
        private int NumVV = 0;

        private int[] ANum1X = new int[50];
        private int[] ANum12 = new int[50];
        private int[] ANumX2 = new int[50];
        private int[] ANum11 = new int[50];
        private int[] ANumXX = new int[50];
        private int[] ANum22 = new int[50];
        private int[] ANum1V = new int[50];
        private int[] ANumXV = new int[50];
        private int[] ANum2V = new int[50];
        private int[] ANumVV = new int[50];



        public CalculoFormatosFrm()
        {
            InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        //mis var
        private string archivoFinal = "";
        #region Windows Forms Designer generated code
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalculoFormatosFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.btnFileOut = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleName = "Label1";
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "Columna";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnFileOut
            // 
            this.btnFileOut.BackColor = System.Drawing.Color.LightSalmon;
            this.btnFileOut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileOut.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileOut.Image = ((System.Drawing.Image)(resources.GetObject("btnFileOut.Image")));
            this.btnFileOut.Location = new System.Drawing.Point(11, 9);
            this.btnFileOut.Name = "btnFileOut";
            this.btnFileOut.Size = new System.Drawing.Size(24, 22);
            this.btnFileOut.TabIndex = 6;
            this.btnFileOut.UseVisualStyleBackColor = false;
            this.btnFileOut.Click += new System.EventHandler(this.BtnFileOutClick);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(121, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(144, 21);
            this.textBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkSalmon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(78, 88);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(136, 32);
            this.button2.TabIndex = 3;
            this.button2.Text = "Sacar Formato";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.Button2Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(41, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 22);
            this.label2.TabIndex = 7;
            this.label2.Text = "Falta Fichero Salida";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CalculoFormatosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(292, 130);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnFileOut);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "CalculoFormatosFrm";
            this.Text = "Saca Formatos (Indeciso)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion



        void Button2Click(object sender, System.EventArgs e)
        {
            String columna;
            string ParActual;
            int[] ParesNum = new int[9];
            string[] Pares = new string[9];

            // Una col solo puede tener:
            // En trios 12 de los 27 posibles y en cuartetos 11 de los 81

            string TrioActual;
            int[] TriosNum = new int[12];
            string[] Trios = new string[12];

            string CuartetoActual;
            int[] CuartetosNum = new int[11];
            string[] Cuartetos = new string[11];

            string QuintetoActual;
            int[] QuintetosNum = new int[10];
            string[] Quintetos = new string[10];


            string temp;


            bool HayOtroPar = false;
            int i, j;

            j = 1;

            columna = textBox1.Text;

            if (archivoFinal == "")
            {
                string msg = "Falta seleccionar archivo de salida";
                string cab = "Error Output ";
                MessageBox.Show(msg, cab, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (columna.Length != 14)
                {
                    //Esto no debe de hacerse aqui, pues estamos en el "motor de calulo"
                    //Lo apropiado es lanzar una Exception con el mensaje, y capturar la
                    //posible exception en un try{}catch{} en el interfaz de usuario.

                    string msg = "numero de caracteres \n\ren la columna distinto de 14";
                    string cab = "error de longitud";
                    MessageBox.Show(msg, cab, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //************************
                //Pares
                //************************

                for (i = 0; columna.Length >= 1; i++)
                {
                    HayOtroPar = false;
                    if (columna.Length >= 2)
                    {
                        ParActual = columna.Substring(0, 2);

                        //Miramos si ya existe el par q estamos tratando

                        for (j = 0; j < 9; j++)
                        {
                            if (Pares[j] == ParActual)
                            {
                                HayOtroPar = true;
                                ParesNum[j] += 1;
                            }

                        }

                        if (HayOtroPar == false)
                        {
                            for (j = 0; j < 9; j++)
                            {
                                if (ParesNum[j] == 0)
                                {
                                    Pares[j] = ParActual;
                                    ParesNum[j] += 1;
                                    break;
                                }
                            }
                        }
                    }

                    //**************************	
                    //Trios
                    //**************************

                    HayOtroPar = false;
                    if (columna.Length >= 3)
                    {
                        TrioActual = columna.Substring(0, 3);
                        //Miramos si ya existe el trio q estamos tratando


                        for (j = 0; j < 12; j++)
                        {
                            if (Trios[j] == TrioActual)
                            {
                                HayOtroPar = true;
                                TriosNum[j] += 1;
                            }

                        }

                        if (HayOtroPar == false)
                        {
                            for (j = 0; j < 12; j++)
                            {
                                if (TriosNum[j] == 0)
                                {
                                    Trios[j] = TrioActual;
                                    TriosNum[j] += 1;
                                    break;
                                }
                            }
                        }
                    }

                    //**************************	
                    //Cuartetos
                    //**************************

                    HayOtroPar = false;
                    if (columna.Length >= 4)
                    {
                        CuartetoActual = columna.Substring(0, 4);
                        //Miramos si ya existe el cuarteto q estamos tratando


                        for (j = 0; j < 11; j++)
                        {
                            if (Cuartetos[j] == CuartetoActual)
                            {
                                HayOtroPar = true;
                                CuartetosNum[j] += 1;
                            }

                        }

                        if (HayOtroPar == false)
                        {
                            for (j = 0; j < 11; j++)
                            {
                                if (CuartetosNum[j] == 0)
                                {
                                    Cuartetos[j] = CuartetoActual;
                                    CuartetosNum[j] += 1;
                                    break;
                                }
                            }
                        }
                    }


                    //**************************	
                    // Quintetos
                    //**************************

                    HayOtroPar = false;
                    if (columna.Length >= 5)
                    {
                        QuintetoActual = columna.Substring(0, 5);
                        //Miramos si ya existe el quinteto q estamos tratando


                        for (j = 0; j < 10; j++)
                        {
                            if (Quintetos[j] == QuintetoActual)
                            {
                                HayOtroPar = true;
                                QuintetosNum[j] += 1;
                            }

                        }

                        if (HayOtroPar == false)
                        {
                            for (j = 0; j < 10; j++)
                            {
                                if (QuintetosNum[j] == 0)
                                {
                                    Quintetos[j] = QuintetoActual;
                                    QuintetosNum[j] += 1;
                                    break;
                                }
                            }
                        }
                    }



                    //Acorto la columna inicial en 1 por la iz
                    columna = columna.Substring(1);

                }


                // abrir fichero
                StreamWriter sw = new StreamWriter(archivoFinal);

                temp = "* Pares que aparecen";
                sw.WriteLine(temp);

                for (j = 0; j < 9; j++)
                {
                    if (ParesNum[j] != 0)
                    {
                        temp = Pares[j] + "-" + ParesNum[j].ToString();
                        sw.WriteLine(temp);
                        if (j == 8) sw.WriteLine("Pares distintos = " + (j + 1));
                    }
                    else
                    {
                        sw.WriteLine("Pares distintos = " + j);
                        break;
                    }


                }

                temp = "* Trios que aparecen";
                sw.WriteLine(temp);
                for (j = 0; j < 12; j++)
                {
                    if (TriosNum[j] != 0)
                    {
                        temp = Trios[j] + "-" + TriosNum[j].ToString();
                        sw.WriteLine(temp);
                        if (j == 11) sw.WriteLine("Trios distintos = " + (j + 1));
                    }
                    else
                    {
                        sw.WriteLine("Trios distintos = " + j);
                        break;
                    }


                }

                temp = "* Cuartetos que aparecen";
                sw.WriteLine(temp);
                for (j = 0; j < 11; j++)
                {
                    if (CuartetosNum[j] != 0)
                    {
                        temp = Cuartetos[j] + "-" + CuartetosNum[j].ToString();
                        sw.WriteLine(temp);
                        if (j == 10) sw.WriteLine("Cuartetos distintos = " + (j + 1));
                    }
                    else
                    {
                        sw.WriteLine("Cuartetos distintos = " + j);
                        break;
                    }

                }

                temp = "* Quintetos que aparecen";
                sw.WriteLine(temp);
                for (j = 0; j < 10; j++)
                {
                    if (QuintetosNum[j] != 0)
                    {
                        temp = Quintetos[j] + "-" + QuintetosNum[j].ToString();
                        sw.WriteLine(temp);
                        if (j == 9) sw.WriteLine("Quintetos distintos = " + (j + 1));
                    }
                    else
                    {
                        sw.WriteLine("Quintetos distintos = " + j);
                        break;
                    }

                }

                AnalizaColumna(textBox1.Text);

                temp = "* Contactos";
                sw.WriteLine(temp);


                temp = "1X" + "-" + Num1X.ToString();
                sw.WriteLine(temp);
                temp = "12" + "-" + Num12.ToString();
                sw.WriteLine(temp);
                temp = "X2" + "-" + NumX2.ToString();
                sw.WriteLine(temp);
                temp = "11" + "-" + Num11.ToString();
                sw.WriteLine(temp);
                temp = "XX" + "-" + NumXX.ToString();
                sw.WriteLine(temp);
                temp = "22" + "-" + Num22.ToString();
                sw.WriteLine(temp);
                temp = "1V" + "-" + Num1V.ToString();
                sw.WriteLine(temp);
                temp = "XV" + "-" + NumXV.ToString();
                sw.WriteLine(temp);
                temp = "2V" + "-" + Num2V.ToString();
                sw.WriteLine(temp);
                temp = "VV" + "-" + NumVV.ToString();
                sw.WriteLine(temp);

                sw.Close();
                MessageBox.Show("Ya");
            }

        }


        void BtnFileOutClick(object sender, System.EventArgs e)
        {
            SaveFileDialog abreFileOut = new SaveFileDialog();
            abreFileOut.InitialDirectory = Application.StartupPath + "/Columnas/";
            abreFileOut.Filter = "Informe(*.txt)|*.txt|Todos los archivos (*.*)|*.*";

            if (abreFileOut.ShowDialog() == DialogResult.OK)
            {
                archivoFinal = abreFileOut.FileName;
                label2.Text = Path.GetFileName(archivoFinal);
            }
        }

        private void InicializaContadores()
        {
            Num1X = 0;
            Num12 = 0;
            NumX2 = 0;
            Num11 = 0;
            NumXX = 0;

            Num22 = 0;
            Num1V = 0;
            NumXV = 0;
            Num2V = 0;
            NumVV = 0;
        }

        public void AnalizaColumna(string columna)
        {
            InicializaContadores();

            string columnaTemp = columna.Replace("*", "");

            for (int i = 0; i < (columnaTemp.Length - 1); i++)
            {
                switch (columnaTemp.Substring(i, 2))
                {
                    case "1X":
                    case "X1":
                    case "1x":
                    case "x1":
                        Num1X++;
                        Num1V++;
                        break;
                    case "12":
                    case "21":
                        Num12++;
                        Num1V++;
                        break;
                    case "X2":
                    case "2X":
                    case "x2":
                    case "2x":
                        NumX2++;
                        NumVV++;
                        NumXV++;
                        Num2V++;
                        break;
                    case "11":
                        Num11++;
                        break;
                    case "XX":
                    case "xx":
                        NumXX++;
                        NumVV++;
                        NumXV++;
                        break;
                    case "22":
                        Num22++;
                        NumVV++;
                        Num2V++;
                        break;
                }
            }
        }
    }
}
