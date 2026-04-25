// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 xfsf
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
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Collections.Generic;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI {
	public class ImprimirBoletoFrm : Form {
		private TextBox tbmgizq;
		private Button bRconfig;
		private GroupBox groupBox1;
		private Button bLeer;
		private Button bImpDirec;
		private Label label1;
		private TextBox tbmgsup;
		private TextBox tbmaxbol;
		private PictureBox pictureBox1;
		private Label label2;
		private TextBox tbminbol;
		private Label lfile;
		private Label label6;
		private Label label5;
		private Label label4;
		private Label lcols;
        private Label lblImpresora;
        private Button btnVerImpresoras;
        private System.ComponentModel.IContainer components;
		private Button bSconfig;
        private ImageList imageListBoletos;
        private PictureBox pictureBox2;
        private Panel panel1;
        ControladorImpresion controlador = new ControladorImpresion();

		public ImprimirBoletoFrm() {
			InitializeComponent();

		    // Advertising disabled for performance

			if (!GetConfig()) SetConfig();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


	    // Advertising system removed for performance optimization

	    private List<string> cols = new List<string>();
		private string columna; char ch;
		private int nwcol, maxbol, minbol, actubol;
		private int mincol, maxcol;
		private float mgx, mgy;
		private float cx, cy, inc;
		private Font myFont;
		
		private bool GetConfig() {
		    string path  = Application.StartupPath+"/Impresion/imprebol.cfg";
			StreamReader sr;
			try {
				sr = new StreamReader(path);
			}
			catch { return false; }
			string tmp = sr.ReadLine();
			string[] aux = tmp.Split('='); tbmgsup.Text=aux[1];
			tmp=sr.ReadLine();
			aux = tmp.Split('='); tbmgizq.Text=aux[1];
            tmp = sr.ReadLine();
            aux = tmp.Split('='); lblImpresora.Text = aux[1];
            tmp = sr.ReadLine();
            aux = tmp.Split('=');
            GirarBoleto(aux[1]);
			sr.Close();
			return false;
		}
        protected void GirarBoleto(string girar)
        {
            switch (girar)
            {
                case "si":
                    pictureBox1.Image = imageListBoletos.Images[1];
                    break;
                default:
                    pictureBox1.Image = imageListBoletos.Images[0];
                    break;                    
            }
        }
		private void SetConfig() {
		    string path = Application.StartupPath+"/Impresion/imprebol.cfg";
			StreamWriter sw = new StreamWriter(path);
			sw.WriteLine("margen superior="+tbmgsup.Text);
			sw.WriteLine("margen izquierdo="+tbmgizq.Text);
            sw.WriteLine("modelo=" + lblImpresora.Text);
            string girar;
            if(controlador.Rotar)
            {
                girar = "si";
            }
            else
            {
                girar = "no";
            }
            sw.WriteLine("girar=" + girar);
            sw.Close();
		}
        private void LeerCols()
        {
            lcols.Text = lfile.Text = "-";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath + "/Columnas/";
            ofd.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                cols.Clear();
                string filein = Path.GetFileName(ofd.FileName);
                IArchivoColumnas ac = new ArchivoColumnasTexto(ofd.FileName);
                while (ac.SiguienteColumna())
                {
                    cols.Add(ac.LeeColumnaSinComas());
                }
                ac.Cerrar();
                lfile.Text = filein; lcols.Text = "" + cols.Count;
            }
            int boletos = (cols.Count - 1) / 8 + 1;
            tbminbol.Text = "1";
            tbmaxbol.Text = boletos.ToString();
        }
		private void Preparar() {
            if ((cols.Count > 0)&&(cols[0] != null))
            {
                mgy = (float)Convert.ToDouble(tbmgsup.Text);
                mgx = (float)Convert.ToDouble(tbmgizq.Text);
                minbol = Convert.ToInt32(tbminbol.Text); if (minbol < 1) minbol = 1;
                maxbol = Convert.ToInt32(tbmaxbol.Text); if (maxbol < minbol) maxbol = minbol;
                maxcol = maxbol * 8; if (maxcol > cols.Count) maxcol = cols.Count;
                mincol = (minbol - 1) * 8; if (mincol == maxcol) mincol--;
                actubol = minbol;
                nwcol = mincol;
                try
                {
                    myFont = new Font("Arial", 14);

                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += Imprimir;

                    pd.Print();

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado el archivo", "Error");
            }
		}
		
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImprimirBoletoFrm));
            this.bSconfig = new System.Windows.Forms.Button();
            this.lcols = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lfile = new System.Windows.Forms.Label();
            this.tbminbol = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbmaxbol = new System.Windows.Forms.TextBox();
            this.tbmgsup = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bImpDirec = new System.Windows.Forms.Button();
            this.bLeer = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bRconfig = new System.Windows.Forms.Button();
            this.tbmgizq = new System.Windows.Forms.TextBox();
            this.lblImpresora = new System.Windows.Forms.Label();
            this.btnVerImpresoras = new System.Windows.Forms.Button();
            this.imageListBoletos = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bSconfig
            // 
            this.bSconfig.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSconfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSconfig.Font = new System.Drawing.Font("Verdana", 7F);
            this.bSconfig.Image = ((System.Drawing.Image)(resources.GetObject("bSconfig.Image")));
            this.bSconfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSconfig.Location = new System.Drawing.Point(11, 112);
            this.bSconfig.Name = "bSconfig";
            this.bSconfig.Size = new System.Drawing.Size(169, 32);
            this.bSconfig.TabIndex = 18;
            this.bSconfig.Text = "Salvar configuración";
            this.bSconfig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSconfig.UseVisualStyleBackColor = false;
            this.bSconfig.Click += new System.EventHandler(this.BSconfigClick);
            // 
            // lcols
            // 
            this.lcols.BackColor = System.Drawing.SystemColors.Info;
            this.lcols.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lcols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcols.Location = new System.Drawing.Point(123, 46);
            this.lcols.Name = "lcols";
            this.lcols.Size = new System.Drawing.Size(259, 22);
            this.lcols.TabIndex = 23;
            this.lcols.Text = "Núm. columnas";
            this.lcols.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Maroon;
            this.label4.Location = new System.Drawing.Point(18, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 21);
            this.label4.TabIndex = 11;
            this.label4.Text = "Desde el boleto";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Maroon;
            this.label5.Location = new System.Drawing.Point(18, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 21);
            this.label5.TabIndex = 13;
            this.label5.Text = "Hasta el boleto";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Maroon;
            this.label6.Location = new System.Drawing.Point(21, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(192, 16);
            this.label6.TabIndex = 22;
            this.label6.Text = "Posición impresión";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lfile
            // 
            this.lfile.BackColor = System.Drawing.SystemColors.Info;
            this.lfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lfile.Location = new System.Drawing.Point(123, 24);
            this.lfile.Name = "lfile";
            this.lfile.Size = new System.Drawing.Size(259, 21);
            this.lfile.TabIndex = 21;
            this.lfile.Text = "Fichero entrada";
            this.lfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbminbol
            // 
            this.tbminbol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbminbol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbminbol.Location = new System.Drawing.Point(145, 80);
            this.tbminbol.Name = "tbminbol";
            this.tbminbol.Size = new System.Drawing.Size(64, 21);
            this.tbminbol.TabIndex = 12;
            this.tbminbol.Text = "1";
            this.tbminbol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "Margen superior";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(20, 29);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(188, 286);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // tbmaxbol
            // 
            this.tbmaxbol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbmaxbol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbmaxbol.Location = new System.Drawing.Point(145, 102);
            this.tbmaxbol.Name = "tbmaxbol";
            this.tbmaxbol.Size = new System.Drawing.Size(64, 21);
            this.tbmaxbol.TabIndex = 14;
            this.tbmaxbol.Text = "1";
            this.tbmaxbol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbmgsup
            // 
            this.tbmgsup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbmgsup.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbmgsup.Location = new System.Drawing.Point(132, 17);
            this.tbmgsup.Name = "tbmgsup";
            this.tbmgsup.Size = new System.Drawing.Size(48, 21);
            this.tbmgsup.TabIndex = 4;
            this.tbmgsup.Text = "100";
            this.tbmgsup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Margen izquierdo";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bImpDirec
            // 
            this.bImpDirec.BackColor = System.Drawing.Color.DarkSalmon;
            this.bImpDirec.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bImpDirec.Font = new System.Drawing.Font("Verdana", 7F);
            this.bImpDirec.Image = ((System.Drawing.Image)(resources.GetObject("bImpDirec.Image")));
            this.bImpDirec.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bImpDirec.Location = new System.Drawing.Point(246, 80);
            this.bImpDirec.Name = "bImpDirec";
            this.bImpDirec.Size = new System.Drawing.Size(118, 43);
            this.bImpDirec.TabIndex = 10;
            this.bImpDirec.Text = "Imprimir";
            this.bImpDirec.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bImpDirec.UseVisualStyleBackColor = false;
            this.bImpDirec.Click += new System.EventHandler(this.BImpDirecClick);
            // 
            // bLeer
            // 
            this.bLeer.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bLeer.Font = new System.Drawing.Font("Verdana", 7F);
            this.bLeer.Image = ((System.Drawing.Image)(resources.GetObject("bLeer.Image")));
            this.bLeer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeer.Location = new System.Drawing.Point(6, 24);
            this.bLeer.Name = "bLeer";
            this.bLeer.Size = new System.Drawing.Size(116, 44);
            this.bLeer.TabIndex = 14;
            this.bLeer.Text = "Leer columnas";
            this.bLeer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeer.UseVisualStyleBackColor = false;
            this.bLeer.Click += new System.EventHandler(this.BLeerClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbmaxbol);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lcols);
            this.groupBox1.Controls.Add(this.tbminbol);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lfile);
            this.groupBox1.Controls.Add(this.bImpDirec);
            this.groupBox1.Controls.Add(this.bLeer);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(20, 322);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 140);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccionar";
            // 
            // bRconfig
            // 
            this.bRconfig.BackColor = System.Drawing.Color.DarkSalmon;
            this.bRconfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bRconfig.Font = new System.Drawing.Font("Verdana", 7F);
            this.bRconfig.Image = ((System.Drawing.Image)(resources.GetObject("bRconfig.Image")));
            this.bRconfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bRconfig.Location = new System.Drawing.Point(11, 79);
            this.bRconfig.Name = "bRconfig";
            this.bRconfig.Size = new System.Drawing.Size(169, 32);
            this.bRconfig.TabIndex = 19;
            this.bRconfig.Text = "Recuperar configuración";
            this.bRconfig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bRconfig.UseVisualStyleBackColor = false;
            this.bRconfig.Click += new System.EventHandler(this.BRconfigClick);
            // 
            // tbmgizq
            // 
            this.tbmgizq.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbmgizq.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbmgizq.Location = new System.Drawing.Point(132, 42);
            this.tbmgizq.Name = "tbmgizq";
            this.tbmgizq.Size = new System.Drawing.Size(48, 21);
            this.tbmgizq.TabIndex = 5;
            this.tbmgizq.Text = "30";
            this.tbmgizq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblImpresora
            // 
            this.lblImpresora.BackColor = System.Drawing.SystemColors.Info;
            this.lblImpresora.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblImpresora.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImpresora.Location = new System.Drawing.Point(11, 244);
            this.lblImpresora.Name = "lblImpresora";
            this.lblImpresora.Size = new System.Drawing.Size(169, 25);
            this.lblImpresora.TabIndex = 24;
            this.lblImpresora.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnVerImpresoras
            // 
            this.btnVerImpresoras.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnVerImpresoras.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVerImpresoras.Font = new System.Drawing.Font("Verdana", 7F);
            this.btnVerImpresoras.Image = ((System.Drawing.Image)(resources.GetObject("btnVerImpresoras.Image")));
            this.btnVerImpresoras.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVerImpresoras.Location = new System.Drawing.Point(11, 206);
            this.btnVerImpresoras.Name = "btnVerImpresoras";
            this.btnVerImpresoras.Size = new System.Drawing.Size(169, 37);
            this.btnVerImpresoras.TabIndex = 25;
            this.btnVerImpresoras.Text = "Impresoras Conocidas";
            this.btnVerImpresoras.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVerImpresoras.UseVisualStyleBackColor = false;
            this.btnVerImpresoras.Click += new System.EventHandler(this.btnVerImpresoras_Click);
            // 
            // imageListBoletos
            // 
            this.imageListBoletos.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListBoletos.ImageStream")));
            this.imageListBoletos.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListBoletos.Images.SetKeyName(0, "boletoT.jpg");
            this.imageListBoletos.Images.SetKeyName(1, "boletoT2.jpg");
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(41, 472);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(352, 24);
            this.pictureBox2.TabIndex = 26;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblImpresora);
            this.panel1.Controls.Add(this.bRconfig);
            this.panel1.Controls.Add(this.btnVerImpresoras);
            this.panel1.Controls.Add(this.bSconfig);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbmgizq);
            this.panel1.Controls.Add(this.tbmgsup);
            this.panel1.Location = new System.Drawing.Point(223, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 286);
            this.panel1.TabIndex = 27;
            // 
            // ImprimirBoletoFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(435, 514);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImprimirBoletoFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Impresión de boletos";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		
		void BImpDirecClick(object sender, EventArgs e) {
			Preparar();
		}
        void Imprimir(object sender, PrintPageEventArgs ev)
        {
            if (cols.Count > 0)
            {
                char p15;
                SolidBrush myBrush = new SolidBrush(Color.Black);
                ev.Graphics.DrawString(lfile.Text, myFont, myBrush, mgx, mgy);
                ev.Graphics.DrawString("Boleto " + actubol, myFont, myBrush, mgx, mgy + 25);

                columna = cols[nwcol];
                if (columna.Length > 14) p15 = columna[14]; else p15 = '1';
                for (int nw8 = 0; nw8 < 8; nw8++)
                {
                    columna = cols[nwcol++];
                    for (int pa = 0; pa < 14; pa++)
                    {
                        ch = columna[pa];
                        cx = mgx - 29 + (14 - pa) * 19.685F;
                        if (ch == '1') inc = 0;
                        else if (ch == 'X') inc = 14.764F;
                        else inc = 29.528F;
                        cy = mgy + 83 + nw8 * 44.291F + inc;
                        ev.Graphics.FillRectangle(myBrush, cx, cy, 6, 4);
                    }
                    if (nwcol == cols.Count) break;
                    if (nwcol + 2 == cols.Count && nw8 == 6) break;
                }
                cx = mgx - 30;
                if (p15 == '1') cy = mgy + 245;
                else if (p15 == '2') cy = mgy + 305;
                else cy = mgy + 275;
                ev.Graphics.FillRectangle(myBrush, cx, cy, 6, 4);
                actubol++;
                if (nwcol < maxcol) ev.HasMorePages = true;
                else ev.HasMorePages = false;
            }
            else
            {
                MessageBox.Show("No se ha seleccionado el archivo", "Error");
            }
        }
		void BLeerClick(object sender, EventArgs e) { LeerCols(); }
		void BSconfigClick(object sender, EventArgs e) { SetConfig(); }
		void BRconfigClick(object sender, EventArgs e) { GetConfig(); }

        private void btnVerImpresoras_Click(object sender, EventArgs e)
        {
            ListaImpresoras listaImp = new ListaImpresoras(controlador);
            listaImp.ShowDialog();
            if ((controlador != null)&&(controlador.Modelo != null))
            {
                tbmgsup.Text = controlador.MargenSuperior.ToString();
                tbmgizq.Text = controlador.MargenIzquierda.ToString();
                lblImpresora.Text = controlador.Modelo;
                if (controlador.Rotar)
                {
                    pictureBox1.Image = imageListBoletos.Images[1];
                }
                else
                {
                    pictureBox1.Image = imageListBoletos.Images[0];
                }
            }
            else
            {
                CargarDefault();
            }

        }
        protected void CargarDefault()
        {
            GetConfig();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.quinielista.com/archivos.asp?r=1276");
        }
	}
}
