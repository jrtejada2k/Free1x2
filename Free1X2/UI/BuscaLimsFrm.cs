// project created on 08/04/2004 at 10:38
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using Free1X2.UI.Controls;
using Free1X2.Utils;

namespace Free1X2.UI
{
	class BuscaLimsFrm : Form
	{
		private TextBox tbpmin;
		private Label lb13;
		private Label lb10;
		private Label lb11;
		private Label lb14;
		private Label lTime;
		private Label lb09;
		private Label lb08;
		private GroupBox groupBox1;
		private TextBox tbsmax;
		private Button bCalcular;
		private Label lb02;
		private Label lb01;
		private Label lb07;
		private Label lb06;
		private Label lb05;
		private Label lb04;
		private Label label1;
		private valors valors1;
		private TextBox tblac;
		private Label label14;
		private TextBox tbsmin;
		private Label lColProc;
		private Label label3;
		private Label label2;
		private TextBox tbpmax;
		private Label lColAdm;
		private Label label5;
		private Label label4;
		private Label lb03;
		private Label lb12;
        private int noPartidos;
        private Label lbl16;
        private Label lbl15;
        private double[,] dv = new double[14, 3];
        long columnaBase;
        double min = -1;
        double max;
        double minProductos = -1;
        double maxProductos;
        BitArray rangos = new BitArray(VariablesGlobales.NumeroPartidos + 1, false);
		public BuscaLimsFrm()
        {
            try
            {
                noPartidos = VariablesGlobales.NumeroPartidos;

            }
            catch
            {
                noPartidos = 14;
            }
            dv = new double[noPartidos, 3];
			InitializeComponent();
			myTimer = new Timer();
       		myTimer.Interval = 3000;
   		    myTimer.Tick += CalculoColumnas;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}

		
		private int nca, ncv;
		private string tmp;
	    private Timer myTimer;
		private DateTime time0, time9;		

        private void ObtenerRangos()
        {
            rangos = new BitArray(VariablesGlobales.NumeroPartidos + 1, false);
            List<int> aciertosPermitidos = UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(tblac.Text);
            for (int i = 0; i < aciertosPermitidos.Count; i++)
            {
                rangos[aciertosPermitidos[i]] = true;
            }
        }
        private void Calcular() 
		{
			bCalcular.Enabled = false;
			time0 = DateTime.Now;
			myTimer.Start();
			RecuperaPantalla();
			CalculoCB();
            ObtenerRangos();
			//Busqueda();
            AnalizarColumnas("", 0);
			myTimer.Stop();
			Refrescar();
			bCalcular.Enabled = true;
		}
		private void RecuperaPantalla() {
			dv = valors1.RetVals();
			string[] aux = tblac.Text.Split('-');
			if (aux.Length!=2 && aux.Length !=1)
            {
                MessageBox.Show("error en entrada de rango");
			}
		}
		private void CalculoCB() {
            string temp = "";
            Label[] labs = new Label[] { lb01, lb02, lb03, lb04, lb05, lb06, lb07, lb08, lb09, lb10, lb11, lb12, lb13, lb14, lbl15, lbl16 };
            for (int i = 0; i < dv.GetLength(0); i++)
            {
                if (dv[i, 0] >= dv[i, 1] && dv[i, 0] >= dv[0, 2]) labs[i].Text = "1";
                else if (dv[i, 2] >= dv[i, 0] && dv[i, 2] >= dv[i, 1]) labs[i].Text = "2";
                else labs[i].Text = "X";

                temp += labs[i].Text;

                labs[i].Visible = true;
            }
            columnaBase = UtilColumnas.ConvStrToLong(temp);
		}
        private void ObtenerValoracion(string col)
        {
            double valor = 0;
            double valorProductos = 1;
            for (int i = 0; i < col.Length; i++)
            {
                switch (col[i])
                {
                    case '1':
                        valor += dv[i, 0];
                        valorProductos *= dv[i, 0] * 0.03420425138;
                        break;
                    case 'X':
                        valor += dv[i, 1];
                        valorProductos *= dv[i, 1] * 0.03420425138;
                        break;
                    case '2':
                        valor += dv[i, 2];
                        valorProductos *= dv[i, 2] * 0.03420425138;
                        break;
                }
            }
            if ((min > valor)||(min == -1))
            {
                Application.DoEvents();
                min = valor;
            }

            if (max < valor)
            {
                Application.DoEvents();
                max = valor;
            }

            if ((minProductos > valorProductos) || (minProductos == -1))
            {
                Application.DoEvents();
                minProductos = valorProductos;
            }

            if (maxProductos < valorProductos)
            {
                Application.DoEvents();
                maxProductos = valorProductos;
            }
        }
        protected void AnalizarColumnas(string preString, int partidoNo)
        {
            string[] signos = { "1", "X", "2" };

            for (int i = 0; i < signos.Length; i++)
            {
                string newPreString = preString + signos[i];

                if (partidoNo < noPartidos - 1)
                {
                    AnalizarColumnas(newPreString, partidoNo + 1);
                }
                else
                {
                    nca++;
                    long columnaGenerada = UtilColumnas.ConvStrToLong(newPreString);
                    long resultado = columnaBase & columnaGenerada;
                    int aciertos = UtilColumnas.ContarBitsA1(resultado);
                    if (rangos[aciertos])
                    {
                        ncv++;
                        ObtenerValoracion(newPreString);
                    }
                }
            }
        }

		private void Refrescar() {
            if (min > -1)
            {
                tbsmin.Text = min.ToString();
            }
            if (max > 0)
            {
                tbsmax.Text = max.ToString();
            }
            if (minProductos > -1)
            {
                tbpmin.Text = minProductos.ToString();
            }
            if (maxProductos > 0)
            {
                tbpmax.Text = maxProductos.ToString();
            }
			lColProc.Text = ""+nca;
			lColAdm.Text = ""+ncv;
			time9 = DateTime.Now;
			tmp = (time9-time0)+"00000000000";
			lTime.Text = tmp.Substring(0,11);
		}

		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuscaLimsFrm));
            this.lb12 = new System.Windows.Forms.Label();
            this.lb03 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lColAdm = new System.Windows.Forms.Label();
            this.tbpmax = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lColProc = new System.Windows.Forms.Label();
            this.tbsmin = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tblac = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lb04 = new System.Windows.Forms.Label();
            this.lb05 = new System.Windows.Forms.Label();
            this.lb06 = new System.Windows.Forms.Label();
            this.lb07 = new System.Windows.Forms.Label();
            this.lb01 = new System.Windows.Forms.Label();
            this.lb02 = new System.Windows.Forms.Label();
            this.bCalcular = new System.Windows.Forms.Button();
            this.tbsmax = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbpmin = new System.Windows.Forms.TextBox();
            this.lb08 = new System.Windows.Forms.Label();
            this.lb09 = new System.Windows.Forms.Label();
            this.lTime = new System.Windows.Forms.Label();
            this.lb14 = new System.Windows.Forms.Label();
            this.lb11 = new System.Windows.Forms.Label();
            this.lb10 = new System.Windows.Forms.Label();
            this.lb13 = new System.Windows.Forms.Label();
            this.valors1 = new Free1X2.UI.Controls.valors();
            this.lbl16 = new System.Windows.Forms.Label();
            this.lbl15 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb12
            // 
            this.lb12.BackColor = System.Drawing.SystemColors.Info;
            this.lb12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb12.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb12.Location = new System.Drawing.Point(193, 255);
            this.lb12.Name = "lb12";
            this.lb12.Size = new System.Drawing.Size(24, 16);
            this.lb12.TabIndex = 207;
            this.lb12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb12.Visible = false;
            // 
            // lb03
            // 
            this.lb03.BackColor = System.Drawing.SystemColors.Info;
            this.lb03.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb03.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb03.Location = new System.Drawing.Point(193, 90);
            this.lb03.Name = "lb03";
            this.lb03.Size = new System.Drawing.Size(24, 16);
            this.lb03.TabIndex = 200;
            this.lb03.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb03.Visible = false;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(96, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 16);
            this.label4.TabIndex = 204;
            this.label4.Text = "Mín";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(248, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 21);
            this.label5.TabIndex = 215;
            this.label5.Text = "Rango";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lColAdm
            // 
            this.lColAdm.BackColor = System.Drawing.SystemColors.Info;
            this.lColAdm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lColAdm.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lColAdm.Location = new System.Drawing.Point(304, 215);
            this.lColAdm.Name = "lColAdm";
            this.lColAdm.Size = new System.Drawing.Size(112, 24);
            this.lColAdm.TabIndex = 217;
            this.lColAdm.Text = "Admitidas";
            this.lColAdm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbpmax
            // 
            this.tbpmax.BackColor = System.Drawing.SystemColors.Info;
            this.tbpmax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbpmax.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbpmax.Location = new System.Drawing.Point(153, 72);
            this.tbpmax.Name = "tbpmax";
            this.tbpmax.Size = new System.Drawing.Size(56, 21);
            this.tbpmax.TabIndex = 209;
            this.tbpmax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 202;
            this.label2.Text = "Productos";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(153, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 16);
            this.label3.TabIndex = 205;
            this.label3.Text = "Máx";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lColProc
            // 
            this.lColProc.BackColor = System.Drawing.SystemColors.Info;
            this.lColProc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lColProc.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lColProc.Location = new System.Drawing.Point(304, 190);
            this.lColProc.Name = "lColProc";
            this.lColProc.Size = new System.Drawing.Size(112, 24);
            this.lColProc.TabIndex = 218;
            this.lColProc.Text = "Procesadas";
            this.lColProc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbsmin
            // 
            this.tbsmin.BackColor = System.Drawing.SystemColors.Info;
            this.tbsmin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbsmin.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbsmin.Location = new System.Drawing.Point(96, 48);
            this.tbsmin.Name = "tbsmin";
            this.tbsmin.Size = new System.Drawing.Size(56, 21);
            this.tbsmin.TabIndex = 206;
            this.tbsmin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(189, 34);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 16);
            this.label14.TabIndex = 212;
            this.label14.Text = "C.B.";
            // 
            // tblac
            // 
            this.tblac.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblac.Location = new System.Drawing.Point(305, 40);
            this.tblac.Name = "tblac";
            this.tblac.Size = new System.Drawing.Size(64, 21);
            this.tblac.TabIndex = 214;
            this.tblac.Text = "4-9";
            this.tblac.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(32, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 203;
            this.label1.Text = "Sumas";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb04
            // 
            this.lb04.BackColor = System.Drawing.SystemColors.Info;
            this.lb04.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb04.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb04.Location = new System.Drawing.Point(193, 107);
            this.lb04.Name = "lb04";
            this.lb04.Size = new System.Drawing.Size(24, 16);
            this.lb04.TabIndex = 199;
            this.lb04.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb04.Visible = false;
            // 
            // lb05
            // 
            this.lb05.BackColor = System.Drawing.SystemColors.Info;
            this.lb05.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb05.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb05.Location = new System.Drawing.Point(193, 128);
            this.lb05.Name = "lb05";
            this.lb05.Size = new System.Drawing.Size(24, 16);
            this.lb05.TabIndex = 202;
            this.lb05.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb05.Visible = false;
            // 
            // lb06
            // 
            this.lb06.BackColor = System.Drawing.SystemColors.Info;
            this.lb06.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb06.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb06.Location = new System.Drawing.Point(193, 145);
            this.lb06.Name = "lb06";
            this.lb06.Size = new System.Drawing.Size(24, 16);
            this.lb06.TabIndex = 205;
            this.lb06.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb06.Visible = false;
            // 
            // lb07
            // 
            this.lb07.BackColor = System.Drawing.SystemColors.Info;
            this.lb07.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb07.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb07.Location = new System.Drawing.Point(193, 162);
            this.lb07.Name = "lb07";
            this.lb07.Size = new System.Drawing.Size(24, 16);
            this.lb07.TabIndex = 204;
            this.lb07.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb07.Visible = false;
            // 
            // lb01
            // 
            this.lb01.BackColor = System.Drawing.SystemColors.Info;
            this.lb01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb01.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb01.Location = new System.Drawing.Point(193, 56);
            this.lb01.Name = "lb01";
            this.lb01.Size = new System.Drawing.Size(24, 16);
            this.lb01.TabIndex = 198;
            this.lb01.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb01.Visible = false;
            // 
            // lb02
            // 
            this.lb02.BackColor = System.Drawing.SystemColors.Info;
            this.lb02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb02.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb02.Location = new System.Drawing.Point(193, 73);
            this.lb02.Name = "lb02";
            this.lb02.Size = new System.Drawing.Size(24, 16);
            this.lb02.TabIndex = 201;
            this.lb02.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb02.Visible = false;
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(296, 144);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(128, 32);
            this.bCalcular.TabIndex = 196;
            this.bCalcular.Text = "Calcular Límites";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // tbsmax
            // 
            this.tbsmax.BackColor = System.Drawing.SystemColors.Info;
            this.tbsmax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbsmax.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbsmax.Location = new System.Drawing.Point(153, 48);
            this.tbsmax.Name = "tbsmax";
            this.tbsmax.Size = new System.Drawing.Size(56, 21);
            this.tbsmax.TabIndex = 207;
            this.tbsmax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbpmax);
            this.groupBox1.Controls.Add(this.tbpmin);
            this.groupBox1.Controls.Add(this.tbsmax);
            this.groupBox1.Controls.Add(this.tbsmin);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(224, 360);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(224, 112);
            this.groupBox1.TabIndex = 197;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resultados";
            // 
            // tbpmin
            // 
            this.tbpmin.BackColor = System.Drawing.SystemColors.Info;
            this.tbpmin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbpmin.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbpmin.Location = new System.Drawing.Point(96, 72);
            this.tbpmin.Name = "tbpmin";
            this.tbpmin.Size = new System.Drawing.Size(56, 21);
            this.tbpmin.TabIndex = 208;
            this.tbpmin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lb08
            // 
            this.lb08.BackColor = System.Drawing.SystemColors.Info;
            this.lb08.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb08.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb08.Location = new System.Drawing.Point(193, 179);
            this.lb08.Name = "lb08";
            this.lb08.Size = new System.Drawing.Size(24, 16);
            this.lb08.TabIndex = 203;
            this.lb08.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb08.Visible = false;
            // 
            // lb09
            // 
            this.lb09.BackColor = System.Drawing.SystemColors.Info;
            this.lb09.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb09.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb09.Location = new System.Drawing.Point(193, 200);
            this.lb09.Name = "lb09";
            this.lb09.Size = new System.Drawing.Size(24, 16);
            this.lb09.TabIndex = 206;
            this.lb09.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb09.Visible = false;
            // 
            // lTime
            // 
            this.lTime.BackColor = System.Drawing.SystemColors.Info;
            this.lTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lTime.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTime.Location = new System.Drawing.Point(304, 240);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(112, 24);
            this.lTime.TabIndex = 216;
            this.lTime.Text = "Tiempo";
            this.lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb14
            // 
            this.lb14.BackColor = System.Drawing.SystemColors.Info;
            this.lb14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb14.Location = new System.Drawing.Point(193, 289);
            this.lb14.Name = "lb14";
            this.lb14.Size = new System.Drawing.Size(24, 16);
            this.lb14.TabIndex = 213;
            this.lb14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb14.Visible = false;
            // 
            // lb11
            // 
            this.lb11.BackColor = System.Drawing.SystemColors.Info;
            this.lb11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb11.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb11.Location = new System.Drawing.Point(193, 234);
            this.lb11.Name = "lb11";
            this.lb11.Size = new System.Drawing.Size(24, 16);
            this.lb11.TabIndex = 208;
            this.lb11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb11.Visible = false;
            // 
            // lb10
            // 
            this.lb10.BackColor = System.Drawing.SystemColors.Info;
            this.lb10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb10.Location = new System.Drawing.Point(193, 217);
            this.lb10.Name = "lb10";
            this.lb10.Size = new System.Drawing.Size(24, 16);
            this.lb10.TabIndex = 209;
            this.lb10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb10.Visible = false;
            // 
            // lb13
            // 
            this.lb13.BackColor = System.Drawing.SystemColors.Info;
            this.lb13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb13.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb13.Location = new System.Drawing.Point(193, 272);
            this.lb13.Name = "lb13";
            this.lb13.Size = new System.Drawing.Size(24, 16);
            this.lb13.TabIndex = 210;
            this.lb13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb13.Visible = false;
            // 
            // valors1
            // 
            this.valors1.BackColor = System.Drawing.Color.Bisque;
            this.valors1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valors1.Location = new System.Drawing.Point(2, 8);
            this.valors1.Name = "valors1";
            this.valors1.Size = new System.Drawing.Size(184, 493);
            this.valors1.TabIndex = 219;
            // 
            // lbl16
            // 
            this.lbl16.BackColor = System.Drawing.SystemColors.Info;
            this.lbl16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl16.Location = new System.Drawing.Point(193, 327);
            this.lbl16.Name = "lbl16";
            this.lbl16.Size = new System.Drawing.Size(24, 16);
            this.lbl16.TabIndex = 223;
            this.lbl16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl16.Visible = false;
            // 
            // lbl15
            // 
            this.lbl15.BackColor = System.Drawing.SystemColors.Info;
            this.lbl15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl15.Location = new System.Drawing.Point(193, 310);
            this.lbl15.Name = "lbl15";
            this.lbl15.Size = new System.Drawing.Size(24, 16);
            this.lbl15.TabIndex = 222;
            this.lbl15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl15.Visible = false;
            // 
            // BuscaLimsFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(464, 513);
            this.Controls.Add(this.lbl16);
            this.Controls.Add(this.lbl15);
            this.Controls.Add(this.valors1);
            this.Controls.Add(this.lColProc);
            this.Controls.Add(this.lColAdm);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tblac);
            this.Controls.Add(this.lb14);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.lb13);
            this.Controls.Add(this.lb10);
            this.Controls.Add(this.lb11);
            this.Controls.Add(this.lb12);
            this.Controls.Add(this.lb09);
            this.Controls.Add(this.lb06);
            this.Controls.Add(this.lb07);
            this.Controls.Add(this.lb08);
            this.Controls.Add(this.lb05);
            this.Controls.Add(this.lb02);
            this.Controls.Add(this.lb03);
            this.Controls.Add(this.lb04);
            this.Controls.Add(this.lb01);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bCalcular);
            this.Name = "BuscaLimsFrm";
            this.Text = "Buscador límites";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		void BCalcularClick(object sender, EventArgs e) { Calcular(); }
		void CalculoColumnas(object sender, EventArgs e) { Refrescar(); }

	}			
}
