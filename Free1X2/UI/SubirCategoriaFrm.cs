using System;
using System.IO;
using System.Windows.Forms;
using Free1X2.Utils ;

using Free1X2.SubirCategoria;
using Free1X2.EntradaSalida;

namespace Free1X2.UI 
{
	class SubirCategoriaFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.TextBox textBoxOut;
		private System.Windows.Forms.TextBox textBoxIn;
		private System.Windows.Forms.Button btnGrabar;
		private System.Windows.Forms.TextBox textBoxCount;
		private System.Windows.Forms.CheckBox checkBox13;
		private System.Windows.Forms.CheckBox checkBox10;
		private System.Windows.Forms.Button btnFileIn;
		private System.Windows.Forms.CheckBox checkBox9;
		private System.Windows.Forms.CheckBox checkBox7;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.Button btnCalcular;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox14;
		private System.Windows.Forms.Button btnFileOut;
		private System.Windows.Forms.CheckBox checkBox12;
		private System.Windows.Forms.CheckBox checkBox11;
		private System.Windows.Forms.CheckBox checkBox8;
		
		private Calculos SubeCat;
		private string archivoBase = "";
		private string archivoExternas="";
		private int seguidos;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkNiv_1;
		private System.Windows.Forms.CheckBox chkNiv_2;
		private System.Windows.Forms.CheckBox chkNiv_4;
		private System.Windows.Forms.CheckBox chkNiv_3;
		private System.Windows.Forms.CheckBox chkNiv_8;
		private System.Windows.Forms.CheckBox chkNiv_7;
		private System.Windows.Forms.CheckBox chkNiv_6;
		private System.Windows.Forms.CheckBox chkNiv_5;
		private System.Windows.Forms.CheckBox chkNiv_9;
		private System.Windows.Forms.CheckBox chkNiv_14;
		private System.Windows.Forms.CheckBox chkNiv_13;
		private System.Windows.Forms.CheckBox chkNiv_12;
		private System.Windows.Forms.CheckBox chkNiv_11;
		private System.Windows.Forms.CheckBox chkNiv_10;
		private System.Windows.Forms.CheckBox chkNiv_0;
		private string archivoFinal = "";
		private System.Windows.Forms.Label lbl14;
		private System.Windows.Forms.Label lbl13;
		private System.Windows.Forms.Label lbl12;
		private System.Windows.Forms.Label lbl9;
		private System.Windows.Forms.Label lbl10;
		private System.Windows.Forms.Label lbl11;
		private System.Windows.Forms.Label lbl5;
		private System.Windows.Forms.Label lbl6;
		private System.Windows.Forms.Label lbl7;
		private System.Windows.Forms.Label lbl8;
		private System.Windows.Forms.Label lbl4;
		private System.Windows.Forms.Label lbl1;
		private System.Windows.Forms.Label lbl2;
		private System.Windows.Forms.Label lbl3;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox txSeguidos;
		private int NumPartidos=14;
		private System.Windows.Forms.TextBox txCombinacionExterna;
		private System.Windows.Forms.Button btFileExternas;
		private ValidadorCadenas Valida= new ValidadorCadenas();
        private CheckBox chkNiv_16;
        private CheckBox chkNiv_15;
	    private Label lbl15;
        private Label lbl16;
        private CheckBox checkBox16;
        private CheckBox checkBox15;
        int noSignos;
        Label[] labels;
        CheckBox[] checksPartidos;
        CheckBox[] checksNiveles;

	    public SubirCategoriaFrm()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);

            labels = new Label[] { lbl1, lbl2, lbl3, lbl4, lbl5, lbl6, lbl7, lbl8, lbl9, lbl10, lbl11, lbl12, lbl13, lbl14, lbl15, lbl16 };
            checksPartidos = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9, checkBox10, checkBox11, checkBox12, checkBox13, checkBox14, checkBox15, checkBox16 };
            checksNiveles = new CheckBox[] { chkNiv_0, chkNiv_1, chkNiv_2, chkNiv_3, chkNiv_4, chkNiv_5, chkNiv_6, chkNiv_7, chkNiv_8, chkNiv_9, chkNiv_10, chkNiv_11, chkNiv_12, chkNiv_13, chkNiv_14, chkNiv_15, chkNiv_16 };
            DeshabilitarTodo();
		}
        private void AdaptarInterfaz(int noPartidos)
        {
            DeshabilitarTodo();
            for (int i = 0; i < noPartidos; i++)
            {
                labels[i].Enabled = true;
                checksPartidos[i].Enabled = true;
                checksPartidos[i].Checked = true;
            }
            AdaptarNiveles();
        }
        private void AdaptarNiveles()
        {
            DeshabilitarNiveles();
            checksNiveles[0].Enabled = true;
            int nivel = 1;
            for (int i = 0; i < checksPartidos.Length; i++)
            {
                if (checksPartidos[i].Checked)
                {
                    checksNiveles[nivel].Enabled = true;
                    nivel++;
                }
            }
        }
        private void DeshabilitarTodo()
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Enabled = false;
                checksPartidos[i].Enabled = false;
                checksPartidos[i].Checked = false;
            }
            for (int i = 0; i < checksNiveles.Length; i++)
            {
                checksNiveles[i].Enabled = false;
                checksNiveles[i].Checked = false;
            }
        }
        private void DeshabilitarNiveles()
        {
            for (int i = 0; i < checksNiveles.Length; i++)
            {
                checksNiveles[i].Enabled = false;
                checksNiveles[i].Checked = false;
            }
        }
		private bool[] calcpartidos() 
		{
			bool[] involucrados = new bool[checksPartidos.Length];
            for (int i = 0; i < involucrados.Length; i++)
            {
                involucrados[i] = checksPartidos[i].Checked;
            }
			return involucrados;
		}
	
		
		
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubirCategoriaFrm));
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.checkBox14 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox13 = new System.Windows.Forms.CheckBox();
            this.textBoxCount = new System.Windows.Forms.TextBox();
            this.textBoxIn = new System.Windows.Forms.TextBox();
            this.textBoxOut = new System.Windows.Forms.TextBox();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.lbl15 = new System.Windows.Forms.Label();
            this.lbl16 = new System.Windows.Forms.Label();
            this.checkBox16 = new System.Windows.Forms.CheckBox();
            this.checkBox15 = new System.Windows.Forms.CheckBox();
            this.txCombinacionExterna = new System.Windows.Forms.TextBox();
            this.btFileExternas = new System.Windows.Forms.Button();
            this.txSeguidos = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.lbl4 = new System.Windows.Forms.Label();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lbl3 = new System.Windows.Forms.Label();
            this.lbl8 = new System.Windows.Forms.Label();
            this.lbl5 = new System.Windows.Forms.Label();
            this.lbl6 = new System.Windows.Forms.Label();
            this.lbl7 = new System.Windows.Forms.Label();
            this.lbl9 = new System.Windows.Forms.Label();
            this.lbl10 = new System.Windows.Forms.Label();
            this.lbl11 = new System.Windows.Forms.Label();
            this.lbl12 = new System.Windows.Forms.Label();
            this.lbl13 = new System.Windows.Forms.Label();
            this.lbl14 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkNiv_16 = new System.Windows.Forms.CheckBox();
            this.chkNiv_15 = new System.Windows.Forms.CheckBox();
            this.chkNiv_0 = new System.Windows.Forms.CheckBox();
            this.chkNiv_14 = new System.Windows.Forms.CheckBox();
            this.chkNiv_13 = new System.Windows.Forms.CheckBox();
            this.chkNiv_12 = new System.Windows.Forms.CheckBox();
            this.chkNiv_11 = new System.Windows.Forms.CheckBox();
            this.chkNiv_10 = new System.Windows.Forms.CheckBox();
            this.chkNiv_9 = new System.Windows.Forms.CheckBox();
            this.chkNiv_8 = new System.Windows.Forms.CheckBox();
            this.chkNiv_7 = new System.Windows.Forms.CheckBox();
            this.chkNiv_6 = new System.Windows.Forms.CheckBox();
            this.chkNiv_5 = new System.Windows.Forms.CheckBox();
            this.chkNiv_4 = new System.Windows.Forms.CheckBox();
            this.chkNiv_3 = new System.Windows.Forms.CheckBox();
            this.chkNiv_2 = new System.Windows.Forms.CheckBox();
            this.chkNiv_1 = new System.Windows.Forms.CheckBox();
            this.btnGrabar = new System.Windows.Forms.Button();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.btnFileOut = new System.Windows.Forms.Button();
            this.btnFileIn = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox8
            // 
            this.checkBox8.Checked = true;
            this.checkBox8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox8.Location = new System.Drawing.Point(235, 48);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(16, 16);
            this.checkBox8.TabIndex = 11;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox11
            // 
            this.checkBox11.Checked = true;
            this.checkBox11.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox11.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox11.Location = new System.Drawing.Point(326, 48);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(16, 16);
            this.checkBox11.TabIndex = 12;
            this.checkBox11.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox12
            // 
            this.checkBox12.Checked = true;
            this.checkBox12.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox12.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox12.Location = new System.Drawing.Point(359, 48);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(16, 16);
            this.checkBox12.TabIndex = 15;
            this.checkBox12.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox14
            // 
            this.checkBox14.Checked = true;
            this.checkBox14.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox14.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox14.Location = new System.Drawing.Point(417, 48);
            this.checkBox14.Name = "checkBox14";
            this.checkBox14.Size = new System.Drawing.Size(16, 16);
            this.checkBox14.TabIndex = 16;
            this.checkBox14.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox3.Location = new System.Drawing.Point(86, 48);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(16, 16);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox2.Location = new System.Drawing.Point(57, 48);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(16, 16);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.Checked = true;
            this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox5.Location = new System.Drawing.Point(148, 48);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(16, 16);
            this.checkBox5.TabIndex = 10;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox4.Location = new System.Drawing.Point(115, 48);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(16, 16);
            this.checkBox4.TabIndex = 7;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox6
            // 
            this.checkBox6.Checked = true;
            this.checkBox6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox6.Location = new System.Drawing.Point(177, 48);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(16, 16);
            this.checkBox6.TabIndex = 9;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox1.Location = new System.Drawing.Point(28, 48);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(16, 16);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox7
            // 
            this.checkBox7.Checked = true;
            this.checkBox7.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox7.Location = new System.Drawing.Point(206, 48);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(16, 16);
            this.checkBox7.TabIndex = 8;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox9
            // 
            this.checkBox9.Checked = true;
            this.checkBox9.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox9.Location = new System.Drawing.Point(268, 48);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(16, 16);
            this.checkBox9.TabIndex = 14;
            this.checkBox9.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox10
            // 
            this.checkBox10.Checked = true;
            this.checkBox10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox10.Location = new System.Drawing.Point(297, 48);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(16, 16);
            this.checkBox10.TabIndex = 13;
            this.checkBox10.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox13
            // 
            this.checkBox13.Checked = true;
            this.checkBox13.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox13.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox13.Location = new System.Drawing.Point(388, 48);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(16, 16);
            this.checkBox13.TabIndex = 17;
            this.checkBox13.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // textBoxCount
            // 
            this.textBoxCount.BackColor = System.Drawing.Color.LemonChiffon;
            this.textBoxCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCount.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCount.Location = new System.Drawing.Point(192, 280);
            this.textBoxCount.Name = "textBoxCount";
            this.textBoxCount.ReadOnly = true;
            this.textBoxCount.Size = new System.Drawing.Size(120, 22);
            this.textBoxCount.TabIndex = 24;
            this.textBoxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxIn
            // 
            this.textBoxIn.BackColor = System.Drawing.Color.LemonChiffon;
            this.textBoxIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxIn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxIn.Location = new System.Drawing.Point(137, 20);
            this.textBoxIn.Name = "textBoxIn";
            this.textBoxIn.ReadOnly = true;
            this.textBoxIn.Size = new System.Drawing.Size(376, 23);
            this.textBoxIn.TabIndex = 22;
            // 
            // textBoxOut
            // 
            this.textBoxOut.BackColor = System.Drawing.Color.LemonChiffon;
            this.textBoxOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOut.Enabled = false;
            this.textBoxOut.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOut.Location = new System.Drawing.Point(137, 44);
            this.textBoxOut.Name = "textBoxOut";
            this.textBoxOut.ReadOnly = true;
            this.textBoxOut.Size = new System.Drawing.Size(376, 23);
            this.textBoxOut.TabIndex = 23;
            // 
            // groupBox
            // 
            this.groupBox.BackColor = System.Drawing.Color.Bisque;
            this.groupBox.Controls.Add(this.lbl15);
            this.groupBox.Controls.Add(this.lbl16);
            this.groupBox.Controls.Add(this.checkBox16);
            this.groupBox.Controls.Add(this.checkBox15);
            this.groupBox.Controls.Add(this.txCombinacionExterna);
            this.groupBox.Controls.Add(this.btFileExternas);
            this.groupBox.Controls.Add(this.txSeguidos);
            this.groupBox.Controls.Add(this.label15);
            this.groupBox.Controls.Add(this.lbl4);
            this.groupBox.Controls.Add(this.lbl1);
            this.groupBox.Controls.Add(this.lbl2);
            this.groupBox.Controls.Add(this.lbl3);
            this.groupBox.Controls.Add(this.lbl8);
            this.groupBox.Controls.Add(this.lbl5);
            this.groupBox.Controls.Add(this.lbl6);
            this.groupBox.Controls.Add(this.lbl7);
            this.groupBox.Controls.Add(this.lbl9);
            this.groupBox.Controls.Add(this.lbl10);
            this.groupBox.Controls.Add(this.lbl11);
            this.groupBox.Controls.Add(this.lbl12);
            this.groupBox.Controls.Add(this.lbl13);
            this.groupBox.Controls.Add(this.lbl14);
            this.groupBox.Controls.Add(this.label);
            this.groupBox.Controls.Add(this.checkBox8);
            this.groupBox.Controls.Add(this.checkBox1);
            this.groupBox.Controls.Add(this.checkBox2);
            this.groupBox.Controls.Add(this.checkBox3);
            this.groupBox.Controls.Add(this.checkBox4);
            this.groupBox.Controls.Add(this.checkBox7);
            this.groupBox.Controls.Add(this.checkBox6);
            this.groupBox.Controls.Add(this.checkBox5);
            this.groupBox.Controls.Add(this.checkBox11);
            this.groupBox.Controls.Add(this.checkBox10);
            this.groupBox.Controls.Add(this.checkBox9);
            this.groupBox.Controls.Add(this.checkBox12);
            this.groupBox.Controls.Add(this.checkBox14);
            this.groupBox.Controls.Add(this.checkBox13);
            this.groupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox.Location = new System.Drawing.Point(16, 96);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(504, 168);
            this.groupBox.TabIndex = 19;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Partidos Involucrados";
            // 
            // lbl15
            // 
            this.lbl15.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl15.Location = new System.Drawing.Point(442, 28);
            this.lbl15.Name = "lbl15";
            this.lbl15.Size = new System.Drawing.Size(26, 16);
            this.lbl15.TabIndex = 40;
            this.lbl15.Text = "15";
            this.lbl15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl16
            // 
            this.lbl16.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl16.Location = new System.Drawing.Point(471, 28);
            this.lbl16.Name = "lbl16";
            this.lbl16.Size = new System.Drawing.Size(26, 16);
            this.lbl16.TabIndex = 39;
            this.lbl16.Text = "16";
            this.lbl16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox16
            // 
            this.checkBox16.Checked = true;
            this.checkBox16.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox16.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox16.Location = new System.Drawing.Point(479, 48);
            this.checkBox16.Name = "checkBox16";
            this.checkBox16.Size = new System.Drawing.Size(16, 16);
            this.checkBox16.TabIndex = 37;
            this.checkBox16.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox15
            // 
            this.checkBox15.Checked = true;
            this.checkBox15.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox15.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.checkBox15.Location = new System.Drawing.Point(450, 48);
            this.checkBox15.Name = "checkBox15";
            this.checkBox15.Size = new System.Drawing.Size(16, 16);
            this.checkBox15.TabIndex = 38;
            this.checkBox15.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // txCombinacionExterna
            // 
            this.txCombinacionExterna.BackColor = System.Drawing.Color.LemonChiffon;
            this.txCombinacionExterna.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txCombinacionExterna.Enabled = false;
            this.txCombinacionExterna.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txCombinacionExterna.Location = new System.Drawing.Point(131, 136);
            this.txCombinacionExterna.Name = "txCombinacionExterna";
            this.txCombinacionExterna.ReadOnly = true;
            this.txCombinacionExterna.Size = new System.Drawing.Size(364, 23);
            this.txCombinacionExterna.TabIndex = 36;
            // 
            // btFileExternas
            // 
            this.btFileExternas.BackColor = System.Drawing.Color.DarkSalmon;
            this.btFileExternas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btFileExternas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btFileExternas.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btFileExternas.Image = ((System.Drawing.Image)(resources.GetObject("btFileExternas.Image")));
            this.btFileExternas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btFileExternas.Location = new System.Drawing.Point(10, 136);
            this.btFileExternas.Name = "btFileExternas";
            this.btFileExternas.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btFileExternas.Size = new System.Drawing.Size(120, 23);
            this.btFileExternas.TabIndex = 35;
            this.btFileExternas.Text = "Usar combinación";
            this.btFileExternas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btFileExternas.UseVisualStyleBackColor = false;
            this.btFileExternas.Click += new System.EventHandler(this.btFileExternas_Click);
            // 
            // txSeguidos
            // 
            this.txSeguidos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txSeguidos.Location = new System.Drawing.Point(241, 96);
            this.txSeguidos.Name = "txSeguidos";
            this.txSeguidos.Size = new System.Drawing.Size(24, 21);
            this.txSeguidos.TabIndex = 34;
            this.txSeguidos.Text = "0";
            this.txSeguidos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txSeguidos.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txSeguidos_KeyPress);
            this.txSeguidos.TextChanged += new System.EventHandler(this.txSeguidos_TextChanged);
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Location = new System.Drawing.Point(168, 96);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 21);
            this.label15.TabIndex = 33;
            this.label15.Text = "Seguidos";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl4
            // 
            this.lbl4.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl4.Location = new System.Drawing.Point(107, 28);
            this.lbl4.Name = "lbl4";
            this.lbl4.Size = new System.Drawing.Size(26, 16);
            this.lbl4.TabIndex = 32;
            this.lbl4.Text = "4";
            this.lbl4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl1
            // 
            this.lbl1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl1.Location = new System.Drawing.Point(20, 28);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(26, 16);
            this.lbl1.TabIndex = 31;
            this.lbl1.Text = "1";
            this.lbl1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl2
            // 
            this.lbl2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl2.Location = new System.Drawing.Point(49, 28);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(26, 16);
            this.lbl2.TabIndex = 30;
            this.lbl2.Text = "2";
            this.lbl2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl3
            // 
            this.lbl3.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl3.Location = new System.Drawing.Point(78, 28);
            this.lbl3.Name = "lbl3";
            this.lbl3.Size = new System.Drawing.Size(26, 16);
            this.lbl3.TabIndex = 29;
            this.lbl3.Text = "3";
            this.lbl3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl8
            // 
            this.lbl8.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl8.Location = new System.Drawing.Point(227, 28);
            this.lbl8.Name = "lbl8";
            this.lbl8.Size = new System.Drawing.Size(26, 16);
            this.lbl8.TabIndex = 28;
            this.lbl8.Text = "8";
            this.lbl8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl5
            // 
            this.lbl5.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl5.Location = new System.Drawing.Point(140, 28);
            this.lbl5.Name = "lbl5";
            this.lbl5.Size = new System.Drawing.Size(26, 16);
            this.lbl5.TabIndex = 27;
            this.lbl5.Text = "5";
            this.lbl5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl6
            // 
            this.lbl6.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl6.Location = new System.Drawing.Point(169, 28);
            this.lbl6.Name = "lbl6";
            this.lbl6.Size = new System.Drawing.Size(26, 16);
            this.lbl6.TabIndex = 26;
            this.lbl6.Text = "6";
            this.lbl6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl7
            // 
            this.lbl7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl7.Location = new System.Drawing.Point(198, 28);
            this.lbl7.Name = "lbl7";
            this.lbl7.Size = new System.Drawing.Size(26, 16);
            this.lbl7.TabIndex = 25;
            this.lbl7.Text = "7";
            this.lbl7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl9
            // 
            this.lbl9.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl9.Location = new System.Drawing.Point(260, 28);
            this.lbl9.Name = "lbl9";
            this.lbl9.Size = new System.Drawing.Size(26, 16);
            this.lbl9.TabIndex = 24;
            this.lbl9.Text = "9";
            this.lbl9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl10
            // 
            this.lbl10.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl10.Location = new System.Drawing.Point(289, 28);
            this.lbl10.Name = "lbl10";
            this.lbl10.Size = new System.Drawing.Size(26, 16);
            this.lbl10.TabIndex = 23;
            this.lbl10.Text = "10";
            this.lbl10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl11
            // 
            this.lbl11.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl11.Location = new System.Drawing.Point(318, 28);
            this.lbl11.Name = "lbl11";
            this.lbl11.Size = new System.Drawing.Size(26, 16);
            this.lbl11.TabIndex = 22;
            this.lbl11.Text = "11";
            this.lbl11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl12
            // 
            this.lbl12.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl12.Location = new System.Drawing.Point(351, 28);
            this.lbl12.Name = "lbl12";
            this.lbl12.Size = new System.Drawing.Size(26, 16);
            this.lbl12.TabIndex = 21;
            this.lbl12.Text = "12";
            this.lbl12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl13
            // 
            this.lbl13.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl13.Location = new System.Drawing.Point(380, 28);
            this.lbl13.Name = "lbl13";
            this.lbl13.Size = new System.Drawing.Size(26, 16);
            this.lbl13.TabIndex = 20;
            this.lbl13.Text = "13";
            this.lbl13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl14
            // 
            this.lbl14.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbl14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl14.Location = new System.Drawing.Point(409, 28);
            this.lbl14.Name = "lbl14";
            this.lbl14.Size = new System.Drawing.Size(26, 16);
            this.lbl14.TabIndex = 19;
            this.lbl14.Text = "14";
            this.lbl14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(64, 72);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(320, 16);
            this.label.TabIndex = 18;
            this.label.Text = "(desmarcando alguno, se arriesga la garantía)";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Bisque;
            this.groupBox1.Controls.Add(this.chkNiv_16);
            this.groupBox1.Controls.Add(this.chkNiv_15);
            this.groupBox1.Controls.Add(this.chkNiv_0);
            this.groupBox1.Controls.Add(this.chkNiv_14);
            this.groupBox1.Controls.Add(this.chkNiv_13);
            this.groupBox1.Controls.Add(this.chkNiv_12);
            this.groupBox1.Controls.Add(this.chkNiv_11);
            this.groupBox1.Controls.Add(this.chkNiv_10);
            this.groupBox1.Controls.Add(this.chkNiv_9);
            this.groupBox1.Controls.Add(this.chkNiv_8);
            this.groupBox1.Controls.Add(this.chkNiv_7);
            this.groupBox1.Controls.Add(this.chkNiv_6);
            this.groupBox1.Controls.Add(this.chkNiv_5);
            this.groupBox1.Controls.Add(this.chkNiv_4);
            this.groupBox1.Controls.Add(this.chkNiv_3);
            this.groupBox1.Controls.Add(this.chkNiv_2);
            this.groupBox1.Controls.Add(this.chkNiv_1);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(526, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(72, 325);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Niveles";
            // 
            // chkNiv_16
            // 
            this.chkNiv_16.Enabled = false;
            this.chkNiv_16.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_16.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_16.Location = new System.Drawing.Point(16, 24);
            this.chkNiv_16.Name = "chkNiv_16";
            this.chkNiv_16.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_16.TabIndex = 21;
            this.chkNiv_16.Text = "16";
            // 
            // chkNiv_15
            // 
            this.chkNiv_15.Enabled = false;
            this.chkNiv_15.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_15.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_15.Location = new System.Drawing.Point(16, 40);
            this.chkNiv_15.Name = "chkNiv_15";
            this.chkNiv_15.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_15.TabIndex = 20;
            this.chkNiv_15.Text = "15";
            // 
            // chkNiv_0
            // 
            this.chkNiv_0.Enabled = false;
            this.chkNiv_0.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_0.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_0.Location = new System.Drawing.Point(16, 280);
            this.chkNiv_0.Name = "chkNiv_0";
            this.chkNiv_0.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_0.TabIndex = 19;
            this.chkNiv_0.Text = "0";
            // 
            // chkNiv_14
            // 
            this.chkNiv_14.Enabled = false;
            this.chkNiv_14.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_14.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_14.Location = new System.Drawing.Point(16, 56);
            this.chkNiv_14.Name = "chkNiv_14";
            this.chkNiv_14.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_14.TabIndex = 18;
            this.chkNiv_14.Text = "14";
            // 
            // chkNiv_13
            // 
            this.chkNiv_13.Enabled = false;
            this.chkNiv_13.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_13.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_13.Location = new System.Drawing.Point(16, 72);
            this.chkNiv_13.Name = "chkNiv_13";
            this.chkNiv_13.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_13.TabIndex = 17;
            this.chkNiv_13.Text = "13";
            // 
            // chkNiv_12
            // 
            this.chkNiv_12.Enabled = false;
            this.chkNiv_12.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_12.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_12.Location = new System.Drawing.Point(16, 88);
            this.chkNiv_12.Name = "chkNiv_12";
            this.chkNiv_12.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_12.TabIndex = 16;
            this.chkNiv_12.Text = "12";
            // 
            // chkNiv_11
            // 
            this.chkNiv_11.Enabled = false;
            this.chkNiv_11.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_11.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_11.Location = new System.Drawing.Point(16, 104);
            this.chkNiv_11.Name = "chkNiv_11";
            this.chkNiv_11.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_11.TabIndex = 15;
            this.chkNiv_11.Text = "11";
            // 
            // chkNiv_10
            // 
            this.chkNiv_10.Enabled = false;
            this.chkNiv_10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_10.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_10.Location = new System.Drawing.Point(16, 120);
            this.chkNiv_10.Name = "chkNiv_10";
            this.chkNiv_10.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_10.TabIndex = 14;
            this.chkNiv_10.Text = "10";
            // 
            // chkNiv_9
            // 
            this.chkNiv_9.Enabled = false;
            this.chkNiv_9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_9.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_9.Location = new System.Drawing.Point(16, 136);
            this.chkNiv_9.Name = "chkNiv_9";
            this.chkNiv_9.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_9.TabIndex = 13;
            this.chkNiv_9.Text = "9";
            // 
            // chkNiv_8
            // 
            this.chkNiv_8.Enabled = false;
            this.chkNiv_8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_8.Location = new System.Drawing.Point(16, 152);
            this.chkNiv_8.Name = "chkNiv_8";
            this.chkNiv_8.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_8.TabIndex = 12;
            this.chkNiv_8.Text = "8";
            // 
            // chkNiv_7
            // 
            this.chkNiv_7.Enabled = false;
            this.chkNiv_7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_7.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_7.Location = new System.Drawing.Point(16, 168);
            this.chkNiv_7.Name = "chkNiv_7";
            this.chkNiv_7.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_7.TabIndex = 11;
            this.chkNiv_7.Text = "7";
            // 
            // chkNiv_6
            // 
            this.chkNiv_6.Enabled = false;
            this.chkNiv_6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_6.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_6.Location = new System.Drawing.Point(16, 184);
            this.chkNiv_6.Name = "chkNiv_6";
            this.chkNiv_6.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_6.TabIndex = 10;
            this.chkNiv_6.Text = "6";
            // 
            // chkNiv_5
            // 
            this.chkNiv_5.Enabled = false;
            this.chkNiv_5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_5.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_5.Location = new System.Drawing.Point(16, 200);
            this.chkNiv_5.Name = "chkNiv_5";
            this.chkNiv_5.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_5.TabIndex = 9;
            this.chkNiv_5.Text = "5";
            // 
            // chkNiv_4
            // 
            this.chkNiv_4.Enabled = false;
            this.chkNiv_4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_4.Location = new System.Drawing.Point(16, 216);
            this.chkNiv_4.Name = "chkNiv_4";
            this.chkNiv_4.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_4.TabIndex = 8;
            this.chkNiv_4.Text = "4";
            // 
            // chkNiv_3
            // 
            this.chkNiv_3.Enabled = false;
            this.chkNiv_3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_3.Location = new System.Drawing.Point(16, 232);
            this.chkNiv_3.Name = "chkNiv_3";
            this.chkNiv_3.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_3.TabIndex = 7;
            this.chkNiv_3.Text = "3";
            // 
            // chkNiv_2
            // 
            this.chkNiv_2.Enabled = false;
            this.chkNiv_2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_2.Location = new System.Drawing.Point(16, 248);
            this.chkNiv_2.Name = "chkNiv_2";
            this.chkNiv_2.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_2.TabIndex = 6;
            this.chkNiv_2.Text = "2";
            // 
            // chkNiv_1
            // 
            this.chkNiv_1.Checked = true;
            this.chkNiv_1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNiv_1.Enabled = false;
            this.chkNiv_1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkNiv_1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNiv_1.Location = new System.Drawing.Point(16, 264);
            this.chkNiv_1.Name = "chkNiv_1";
            this.chkNiv_1.Size = new System.Drawing.Size(40, 20);
            this.chkNiv_1.TabIndex = 5;
            this.chkNiv_1.Text = "1";
            // 
            // btnGrabar
            // 
            this.btnGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGrabar.Enabled = false;
            this.btnGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrabar.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrabar.Image = ((System.Drawing.Image)(resources.GetObject("btnGrabar.Image")));
            this.btnGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGrabar.Location = new System.Drawing.Point(336, 280);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(120, 32);
            this.btnGrabar.TabIndex = 21;
            this.btnGrabar.Text = "Grabar";
            this.btnGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGrabar.UseVisualStyleBackColor = false;
            this.btnGrabar.Click += new System.EventHandler(this.BtnGrabarClick);
            // 
            // btnCalcular
            // 
            this.btnCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCalcular.Enabled = false;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalcular.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcular.Image = ((System.Drawing.Image)(resources.GetObject("btnCalcular.Image")));
            this.btnCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCalcular.Location = new System.Drawing.Point(48, 280);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(120, 32);
            this.btnCalcular.TabIndex = 20;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.BtnCalcularClick);
            // 
            // btnFileOut
            // 
            this.btnFileOut.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFileOut.Enabled = false;
            this.btnFileOut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileOut.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileOut.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btnFileOut.Image = ((System.Drawing.Image)(resources.GetObject("btnFileOut.Image")));
            this.btnFileOut.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileOut.Location = new System.Drawing.Point(16, 44);
            this.btnFileOut.Name = "btnFileOut";
            this.btnFileOut.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFileOut.Size = new System.Drawing.Size(120, 23);
            this.btnFileOut.TabIndex = 2;
            this.btnFileOut.Text = "Salida (txt)";
            this.btnFileOut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileOut.UseVisualStyleBackColor = false;
            this.btnFileOut.Click += new System.EventHandler(this.btnFileOutClick);
            // 
            // btnFileIn
            // 
            this.btnFileIn.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnFileIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileIn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileIn.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btnFileIn.Image = ((System.Drawing.Image)(resources.GetObject("btnFileIn.Image")));
            this.btnFileIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileIn.Location = new System.Drawing.Point(16, 20);
            this.btnFileIn.Name = "btnFileIn";
            this.btnFileIn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFileIn.Size = new System.Drawing.Size(120, 23);
            this.btnFileIn.TabIndex = 0;
            this.btnFileIn.Text = "Origen (txt)";
            this.btnFileIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileIn.UseVisualStyleBackColor = false;
            this.btnFileIn.Click += new System.EventHandler(this.BtnFileInClick);
            // 
            // SubirCategoriaFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(610, 360);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxCount);
            this.Controls.Add(this.textBoxOut);
            this.Controls.Add(this.textBoxIn);
            this.Controls.Add(this.btnGrabar);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.btnFileOut);
            this.Controls.Add(this.btnFileIn);
            this.Controls.Add(this.groupBox);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubirCategoriaFrm";
            this.Text = "Subir Categoría";
            this.Load += new System.EventHandler(this.SubirCategoriaFrmLoad);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		void BtnFileInClick(object sender, System.EventArgs e)
		{
            DeshabilitarTodo();
			btnFileIn.Enabled = false;
			OpenFileDialog abreFileIn = new OpenFileDialog();
			abreFileIn.InitialDirectory = "Columnas\\" ;
			abreFileIn.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			
			if(abreFileIn.ShowDialog() == DialogResult.OK)
			{
				archivoBase = abreFileIn.FileName;
				textBoxIn.Text = Path.GetFileName(archivoBase);
                IArchivoColumnas aCol = new ArchivoColumnasTexto(archivoBase);
                this.noSignos = aCol.ObtenNumSignos();
                this.NumPartidos = this.noSignos;
                aCol.Cerrar();
			}
			
			if(archivoBase != "")
			{			
				textBoxCount.Text = "leyendo..."; 
							
				SubeCat = new Calculos( archivoBase );
				
				textBoxCount.Text = "I=" + SubeCat.NoColumnas;
				btnFileOut.Enabled = true;
				textBoxOut.Enabled = true;							
			}
			
			btnFileIn.Enabled = true;
            AdaptarInterfaz(this.NumPartidos);
		}

		void btnFileOutClick(object sender, System.EventArgs e)
		{
			SaveFileDialog abreFileOut = new SaveFileDialog();
			abreFileOut.InitialDirectory = "Columnas\\" ;
			abreFileOut.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			
			if(abreFileOut.ShowDialog() == DialogResult.OK)
			{
			    archivoFinal = abreFileOut.FileName;
				textBoxOut.Text = Path.GetFileName(archivoFinal);
				btnCalcular.Enabled = true;
			}			
		}

		void BtnCalcularClick(object sender, System.EventArgs e) {
            bool[] niveles = new bool[checksNiveles.Length];
            for (int i = 0; i < niveles.Length; i++)
            {
                niveles[i] = checksNiveles[i].Checked;
            }
			if(HayAlgunNivelSeleccionado ())
			{
				seguidos=Convert.ToInt16 (txSeguidos.Text) ;

				btnCalcular.Enabled = false;
				btnGrabar.Enabled = false;
				groupBox.Enabled = false;
				SubeCat.Calcular(calcpartidos(),niveles,seguidos, archivoExternas, noSignos);
				textBoxCount.Text = "F="+SubeCat.NoColumnas;
				groupBox.Enabled = true;
				btnGrabar.Enabled = true;
				btnCalcular.Enabled = true;
			}
			else
			{
				MessageBox.Show ("No se ha seleccionado ningún nivel");
			}
		}
		bool HayAlgunNivelSeleccionado()
		{
			bool res=false;
			for(int i=0; i<checksNiveles.Length;i++)
			{
                if (checksNiveles[i].Checked)
                {
                    res = true;
                    break;
                }
            
			}
			return res;
		}

		void BtnGrabarClick(object sender, System.EventArgs e) {
			btnGrabar.Enabled = false;
			btnCalcular.Enabled = false;
			btnFileOut.Enabled = false;
			textBoxOut.Enabled = false;
			textBoxCount.Text = "grabando...";
			SubeCat.Grabar(textBoxOut.Text);
			textBoxCount.Text = "G="+SubeCat.NoColumnas;
			textBoxOut.Enabled = true;
			btnFileOut.Enabled = true;
			btnCalcular.Enabled = true;
			btnGrabar.Enabled = true;
		}
		
		void SubirCategoriaFrmLoad(object sender, System.EventArgs e)
		{
			
		}

	    private void checkBox_CheckedChanged(object sender, System.EventArgs e)
		{
            AdaptarNiveles();
		}

		private void txSeguidos_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumeros, sender, e);
		}

		private void txSeguidos_TextChanged(object sender, System.EventArgs e)
		{
			if (txSeguidos.Text !="")
			{
				seguidos =Convert.ToInt16 (txSeguidos.Text);
				if (seguidos > NumPartidos )
				{
					seguidos = NumPartidos;
					txSeguidos.Text=NumPartidos.ToString ();
				}
			}
		}

		private void btFileExternas_Click(object sender, EventArgs e)
		{
			OpenFileDialog abreFileEx = new OpenFileDialog();
			abreFileEx.InitialDirectory = "Columnas\\" ;
			abreFileEx.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			
			if(abreFileEx.ShowDialog() == DialogResult.OK)
			{
				archivoExternas= abreFileEx.FileName;
				txCombinacionExterna.Text = Path.GetFileName(archivoExternas);
			}
			else
			{
				archivoExternas="";
				txCombinacionExterna.Text = "";
			}
		}

	}			
}
