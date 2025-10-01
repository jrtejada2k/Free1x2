// created on 06/01/2005 at 21:21
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.using System;


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for DialogoFiltrarPorLimitesFrm.
	/// </summary>
	public class DialogoFiltrarPorLimitesFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Button btCancelar;
		private System.Windows.Forms.Button btAceptar;
		private System.Windows.Forms.TextBox lblextremo9;
		private System.Windows.Forms.TextBox lblextremo9d;
		private System.Windows.Forms.TextBox lblextremo10d;
		private System.Windows.Forms.TextBox lblextremo10;
		private System.Windows.Forms.TextBox lblextremo11;
		private System.Windows.Forms.TextBox lblextremo12d;
		private System.Windows.Forms.TextBox lblextremo12;
		private System.Windows.Forms.TextBox lblextremo13d;
		private System.Windows.Forms.TextBox lblextremo13;
		private System.Windows.Forms.TextBox lblextremo14d;
		private System.Windows.Forms.TextBox lblextremo14;
		private System.Windows.Forms.TextBox lblextremo15d;
		private System.Windows.Forms.TextBox lblextremo15;
		private System.Windows.Forms.TextBox lblextremo16d;
		private System.Windows.Forms.TextBox lblextremo16;
		private System.Windows.Forms.TextBox txdif9;
		private System.Windows.Forms.TextBox txdif9d;
		private System.Windows.Forms.TextBox txdif10d;
		private System.Windows.Forms.TextBox txdif10;
		private System.Windows.Forms.TextBox txdif11d;
		private System.Windows.Forms.TextBox txdif11;
		private System.Windows.Forms.TextBox txdif12d;
		private System.Windows.Forms.TextBox txdif12;
		private System.Windows.Forms.TextBox txdif13d;
		private System.Windows.Forms.TextBox txdif13;
		private System.Windows.Forms.TextBox txdif14d;
		private System.Windows.Forms.TextBox txdif14;
		private System.Windows.Forms.TextBox txdif15d;
		private System.Windows.Forms.TextBox txdif15;
		private System.Windows.Forms.TextBox txdif16d;
		private System.Windows.Forms.TextBox txdif16;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.TextBox txdif17d;
		private System.Windows.Forms.TextBox txdif17;
		private System.Windows.Forms.TextBox lblextremo17;
		private System.Windows.Forms.Label label42;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		public int [,] extremos = new int [10,4];
		public bool ValoresAceptados = false;
		private System.Windows.Forms.TextBox lblextremo11d;
		private System.Windows.Forms.TextBox lblextremo17d;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.Label label44;
		private System.Windows.Forms.TextBox txdif18d;
		private System.Windows.Forms.TextBox txdif18;
		private System.Windows.Forms.TextBox lblextremo18d;
		private System.Windows.Forms.TextBox lblextremo18;
		private System.Windows.Forms.Label label45;
		private System.ComponentModel.Container components = null;

		public DialogoFiltrarPorLimitesFrm(int[,] pextremos)
		{
			InitializeComponent();

			extremos=pextremos;
			CoherenciarExtremos();
			lblextremo9.Text =extremos [0,0].ToString ();
			lblextremo9d.Text =extremos [0,1].ToString ();

			lblextremo10.Text =(extremos [1,0]+1).ToString ();
			lblextremo10d.Text =extremos [1,1].ToString ();

			lblextremo11.Text =(extremos [2,0]+1).ToString ();
			lblextremo11d.Text =extremos [2,1].ToString ();

			lblextremo12.Text =(extremos [3,0]+1).ToString ();
			lblextremo12d.Text =extremos [3,1].ToString ();

			lblextremo13.Text =(extremos [4,0]+1).ToString ();
			lblextremo13d.Text =extremos [4,1].ToString ();

			lblextremo14.Text =(extremos [5,0]+1).ToString ();
			lblextremo14d.Text =extremos [5,1].ToString ();

			lblextremo15.Text =(extremos [6,0]+1).ToString ();
			lblextremo15d.Text =extremos [6,1].ToString ();

			lblextremo16.Text =(extremos [7,0]+1).ToString ();
			lblextremo16d.Text =extremos [7,1].ToString ();

			lblextremo17.Text =(extremos [8,0]+1).ToString ();
			lblextremo17d.Text =extremos [8,1].ToString ();

			lblextremo18.Text =(extremos [9,0]+1).ToString ();
			lblextremo18d.Text =extremos [9,1].ToString ();

            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogoFiltrarPorLimitesFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.lblextremo9 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblextremo9d = new System.Windows.Forms.TextBox();
            this.txdif9 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txdif9d = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txdif10d = new System.Windows.Forms.TextBox();
            this.txdif10 = new System.Windows.Forms.TextBox();
            this.lblextremo10d = new System.Windows.Forms.TextBox();
            this.lblextremo10 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txdif11d = new System.Windows.Forms.TextBox();
            this.txdif11 = new System.Windows.Forms.TextBox();
            this.lblextremo11d = new System.Windows.Forms.TextBox();
            this.lblextremo11 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txdif12d = new System.Windows.Forms.TextBox();
            this.txdif12 = new System.Windows.Forms.TextBox();
            this.lblextremo12d = new System.Windows.Forms.TextBox();
            this.lblextremo12 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txdif13d = new System.Windows.Forms.TextBox();
            this.txdif13 = new System.Windows.Forms.TextBox();
            this.lblextremo13d = new System.Windows.Forms.TextBox();
            this.lblextremo13 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.txdif14d = new System.Windows.Forms.TextBox();
            this.txdif14 = new System.Windows.Forms.TextBox();
            this.lblextremo14d = new System.Windows.Forms.TextBox();
            this.lblextremo14 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.txdif15d = new System.Windows.Forms.TextBox();
            this.txdif15 = new System.Windows.Forms.TextBox();
            this.lblextremo15d = new System.Windows.Forms.TextBox();
            this.lblextremo15 = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txdif16d = new System.Windows.Forms.TextBox();
            this.txdif16 = new System.Windows.Forms.TextBox();
            this.lblextremo16d = new System.Windows.Forms.TextBox();
            this.lblextremo16 = new System.Windows.Forms.TextBox();
            this.btCancelar = new System.Windows.Forms.Button();
            this.btAceptar = new System.Windows.Forms.Button();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.txdif17d = new System.Windows.Forms.TextBox();
            this.txdif17 = new System.Windows.Forms.TextBox();
            this.lblextremo17d = new System.Windows.Forms.TextBox();
            this.lblextremo17 = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.txdif18d = new System.Windows.Forms.TextBox();
            this.txdif18 = new System.Windows.Forms.TextBox();
            this.lblextremo18d = new System.Windows.Forms.TextBox();
            this.lblextremo18 = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(138, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "De la posición ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblextremo9
            // 
            this.lblextremo9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo9.Location = new System.Drawing.Point(138, 34);
            this.lblextremo9.Name = "lblextremo9";
            this.lblextremo9.Size = new System.Drawing.Size(100, 21);
            this.lblextremo9.TabIndex = 1;
            this.lblextremo9.Text = "0";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(239, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "a la posición";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblextremo9d
            // 
            this.lblextremo9d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo9d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo9d.Location = new System.Drawing.Point(239, 34);
            this.lblextremo9d.Name = "lblextremo9d";
            this.lblextremo9d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo9d.TabIndex = 3;
            this.lblextremo9d.Text = "0";
            // 
            // txdif9
            // 
            this.txdif9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif9.Location = new System.Drawing.Point(340, 34);
            this.txdif9.Name = "txdif9";
            this.txdif9.Size = new System.Drawing.Size(24, 21);
            this.txdif9.TabIndex = 5;
            this.txdif9.Text = "0";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(340, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(153, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "eliminar columnas con";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif9d
            // 
            this.txdif9d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif9d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif9d.Location = new System.Drawing.Point(379, 34);
            this.txdif9d.Name = "txdif9d";
            this.txdif9d.Size = new System.Drawing.Size(24, 21);
            this.txdif9d.TabIndex = 6;
            this.txdif9d.Text = "4";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(365, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 21);
            this.label4.TabIndex = 7;
            this.label4.Text = "a";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(25, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 21);
            this.label6.TabIndex = 9;
            this.label6.Text = "Rango de aciertos";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(25, 34);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 21);
            this.label7.TabIndex = 10;
            this.label7.Text = "< 10";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(25, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 21);
            this.label8.TabIndex = 17;
            this.label8.Text = "de 10 a < 11";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(365, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 21);
            this.label10.TabIndex = 15;
            this.label10.Text = "a";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif10d
            // 
            this.txdif10d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif10d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif10d.Location = new System.Drawing.Point(379, 56);
            this.txdif10d.Name = "txdif10d";
            this.txdif10d.Size = new System.Drawing.Size(24, 21);
            this.txdif10d.TabIndex = 14;
            this.txdif10d.Text = "3";
            // 
            // txdif10
            // 
            this.txdif10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif10.Location = new System.Drawing.Point(340, 56);
            this.txdif10.Name = "txdif10";
            this.txdif10.Size = new System.Drawing.Size(24, 21);
            this.txdif10.TabIndex = 13;
            this.txdif10.Text = "1";
            // 
            // lblextremo10d
            // 
            this.lblextremo10d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo10d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo10d.Location = new System.Drawing.Point(239, 56);
            this.lblextremo10d.Name = "lblextremo10d";
            this.lblextremo10d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo10d.TabIndex = 12;
            this.lblextremo10d.Text = "0";
            // 
            // lblextremo10
            // 
            this.lblextremo10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo10.Location = new System.Drawing.Point(138, 56);
            this.lblextremo10.Name = "lblextremo10";
            this.lblextremo10.Size = new System.Drawing.Size(100, 21);
            this.lblextremo10.TabIndex = 11;
            this.lblextremo10.Text = "0";
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(25, 78);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 21);
            this.label11.TabIndex = 24;
            this.label11.Text = "de 11 a < 12";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(365, 78);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(12, 21);
            this.label13.TabIndex = 22;
            this.label13.Text = "a";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif11d
            // 
            this.txdif11d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif11d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif11d.Location = new System.Drawing.Point(379, 78);
            this.txdif11d.Name = "txdif11d";
            this.txdif11d.Size = new System.Drawing.Size(24, 21);
            this.txdif11d.TabIndex = 21;
            this.txdif11d.Text = "2";
            // 
            // txdif11
            // 
            this.txdif11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif11.Location = new System.Drawing.Point(340, 78);
            this.txdif11.Name = "txdif11";
            this.txdif11.Size = new System.Drawing.Size(24, 21);
            this.txdif11.TabIndex = 20;
            this.txdif11.Text = "1";
            // 
            // lblextremo11d
            // 
            this.lblextremo11d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo11d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo11d.Location = new System.Drawing.Point(239, 78);
            this.lblextremo11d.Name = "lblextremo11d";
            this.lblextremo11d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo11d.TabIndex = 19;
            this.lblextremo11d.Text = "0";
            // 
            // lblextremo11
            // 
            this.lblextremo11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo11.Location = new System.Drawing.Point(138, 78);
            this.lblextremo11.Name = "lblextremo11";
            this.lblextremo11.Size = new System.Drawing.Size(100, 21);
            this.lblextremo11.TabIndex = 18;
            this.lblextremo11.Text = "0";
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(25, 100);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 21);
            this.label14.TabIndex = 31;
            this.label14.Text = "de 12 a < 13";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(365, 100);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(12, 21);
            this.label16.TabIndex = 29;
            this.label16.Text = "a";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif12d
            // 
            this.txdif12d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif12d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif12d.Location = new System.Drawing.Point(379, 100);
            this.txdif12d.Name = "txdif12d";
            this.txdif12d.Size = new System.Drawing.Size(24, 21);
            this.txdif12d.TabIndex = 28;
            this.txdif12d.Text = "1";
            // 
            // txdif12
            // 
            this.txdif12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif12.Location = new System.Drawing.Point(340, 100);
            this.txdif12.Name = "txdif12";
            this.txdif12.Size = new System.Drawing.Size(24, 21);
            this.txdif12.TabIndex = 27;
            this.txdif12.Text = "1";
            // 
            // lblextremo12d
            // 
            this.lblextremo12d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo12d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo12d.Location = new System.Drawing.Point(239, 100);
            this.lblextremo12d.Name = "lblextremo12d";
            this.lblextremo12d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo12d.TabIndex = 26;
            this.lblextremo12d.Text = "0";
            // 
            // lblextremo12
            // 
            this.lblextremo12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo12.Location = new System.Drawing.Point(138, 100);
            this.lblextremo12.Name = "lblextremo12";
            this.lblextremo12.Size = new System.Drawing.Size(100, 21);
            this.lblextremo12.TabIndex = 25;
            this.lblextremo12.Text = "0";
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(25, 166);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(112, 21);
            this.label17.TabIndex = 38;
            this.label17.Text = "de < 13 a 12";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(365, 122);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(12, 21);
            this.label19.TabIndex = 36;
            this.label19.Text = "a";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif13d
            // 
            this.txdif13d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif13d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif13d.Location = new System.Drawing.Point(379, 122);
            this.txdif13d.Name = "txdif13d";
            this.txdif13d.Size = new System.Drawing.Size(24, 21);
            this.txdif13d.TabIndex = 35;
            this.txdif13d.Text = "0";
            // 
            // txdif13
            // 
            this.txdif13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif13.Location = new System.Drawing.Point(340, 122);
            this.txdif13.Name = "txdif13";
            this.txdif13.Size = new System.Drawing.Size(24, 21);
            this.txdif13.TabIndex = 34;
            this.txdif13.Text = "1";
            // 
            // lblextremo13d
            // 
            this.lblextremo13d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo13d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo13d.Location = new System.Drawing.Point(239, 122);
            this.lblextremo13d.Name = "lblextremo13d";
            this.lblextremo13d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo13d.TabIndex = 33;
            this.lblextremo13d.Text = "0";
            // 
            // lblextremo13
            // 
            this.lblextremo13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo13.Location = new System.Drawing.Point(138, 122);
            this.lblextremo13.Name = "lblextremo13";
            this.lblextremo13.Size = new System.Drawing.Size(100, 21);
            this.lblextremo13.TabIndex = 32;
            this.lblextremo13.Text = "0";
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(25, 188);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(112, 21);
            this.label20.TabIndex = 45;
            this.label20.Text = "de < 12 a 11";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(365, 144);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(12, 21);
            this.label22.TabIndex = 43;
            this.label22.Text = "a";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif14d
            // 
            this.txdif14d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif14d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif14d.Location = new System.Drawing.Point(379, 144);
            this.txdif14d.Name = "txdif14d";
            this.txdif14d.Size = new System.Drawing.Size(24, 21);
            this.txdif14d.TabIndex = 42;
            this.txdif14d.Text = "0";
            // 
            // txdif14
            // 
            this.txdif14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif14.Location = new System.Drawing.Point(340, 144);
            this.txdif14.Name = "txdif14";
            this.txdif14.Size = new System.Drawing.Size(24, 21);
            this.txdif14.TabIndex = 41;
            this.txdif14.Text = "1";
            // 
            // lblextremo14d
            // 
            this.lblextremo14d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo14d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo14d.Location = new System.Drawing.Point(239, 144);
            this.lblextremo14d.Name = "lblextremo14d";
            this.lblextremo14d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo14d.TabIndex = 40;
            this.lblextremo14d.Text = "0";
            // 
            // lblextremo14
            // 
            this.lblextremo14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo14.Location = new System.Drawing.Point(138, 144);
            this.lblextremo14.Name = "lblextremo14";
            this.lblextremo14.Size = new System.Drawing.Size(100, 21);
            this.lblextremo14.TabIndex = 39;
            this.lblextremo14.Text = "0";
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(25, 210);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(112, 21);
            this.label23.TabIndex = 52;
            this.label23.Text = "de < 11 a 10";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(406, 166);
            this.label24.Name = "label24";
            this.label24.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label24.Size = new System.Drawing.Size(87, 21);
            this.label24.TabIndex = 51;
            this.label24.Text = "diferencias";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(365, 166);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(12, 21);
            this.label25.TabIndex = 50;
            this.label25.Text = "a";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif15d
            // 
            this.txdif15d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif15d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif15d.Location = new System.Drawing.Point(379, 166);
            this.txdif15d.Name = "txdif15d";
            this.txdif15d.Size = new System.Drawing.Size(24, 21);
            this.txdif15d.TabIndex = 49;
            this.txdif15d.Text = "1";
            // 
            // txdif15
            // 
            this.txdif15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif15.Location = new System.Drawing.Point(340, 166);
            this.txdif15.Name = "txdif15";
            this.txdif15.Size = new System.Drawing.Size(24, 21);
            this.txdif15.TabIndex = 48;
            this.txdif15.Text = "1";
            // 
            // lblextremo15d
            // 
            this.lblextremo15d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo15d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo15d.Location = new System.Drawing.Point(239, 166);
            this.lblextremo15d.Name = "lblextremo15d";
            this.lblextremo15d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo15d.TabIndex = 47;
            this.lblextremo15d.Text = "0";
            // 
            // lblextremo15
            // 
            this.lblextremo15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo15.Location = new System.Drawing.Point(138, 166);
            this.lblextremo15.Name = "lblextremo15";
            this.lblextremo15.Size = new System.Drawing.Size(100, 21);
            this.lblextremo15.TabIndex = 46;
            this.lblextremo15.Text = "0";
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label26.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(25, 232);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(112, 21);
            this.label26.TabIndex = 59;
            this.label26.Text = "< 10";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label27.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(406, 188);
            this.label27.Name = "label27";
            this.label27.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label27.Size = new System.Drawing.Size(87, 21);
            this.label27.TabIndex = 58;
            this.label27.Text = "diferencias";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label28
            // 
            this.label28.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(365, 188);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(12, 21);
            this.label28.TabIndex = 57;
            this.label28.Text = "a";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif16d
            // 
            this.txdif16d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif16d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif16d.Location = new System.Drawing.Point(379, 188);
            this.txdif16d.Name = "txdif16d";
            this.txdif16d.Size = new System.Drawing.Size(24, 21);
            this.txdif16d.TabIndex = 56;
            this.txdif16d.Text = "2";
            // 
            // txdif16
            // 
            this.txdif16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif16.Location = new System.Drawing.Point(340, 188);
            this.txdif16.Name = "txdif16";
            this.txdif16.Size = new System.Drawing.Size(24, 21);
            this.txdif16.TabIndex = 55;
            this.txdif16.Text = "1";
            // 
            // lblextremo16d
            // 
            this.lblextremo16d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo16d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo16d.Location = new System.Drawing.Point(239, 188);
            this.lblextremo16d.Name = "lblextremo16d";
            this.lblextremo16d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo16d.TabIndex = 54;
            this.lblextremo16d.Text = "0";
            // 
            // lblextremo16
            // 
            this.lblextremo16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo16.Location = new System.Drawing.Point(138, 188);
            this.lblextremo16.Name = "lblextremo16";
            this.lblextremo16.Size = new System.Drawing.Size(100, 21);
            this.lblextremo16.TabIndex = 53;
            this.lblextremo16.Text = "0";
            // 
            // btCancelar
            // 
            this.btCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(269, 274);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(100, 32);
            this.btCancelar.TabIndex = 60;
            this.btCancelar.Text = "&Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            this.btCancelar.Click += new System.EventHandler(this.button1_Click);
            // 
            // btAceptar
            // 
            this.btAceptar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAceptar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAceptar.Image = ((System.Drawing.Image)(resources.GetObject("btAceptar.Image")));
            this.btAceptar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAceptar.Location = new System.Drawing.Point(149, 274);
            this.btAceptar.Name = "btAceptar";
            this.btAceptar.Size = new System.Drawing.Size(100, 32);
            this.btAceptar.TabIndex = 61;
            this.btAceptar.Text = "&Aceptar";
            this.btAceptar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAceptar.UseVisualStyleBackColor = false;
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label29.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(406, 78);
            this.label29.Name = "label29";
            this.label29.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label29.Size = new System.Drawing.Size(87, 21);
            this.label29.TabIndex = 23;
            this.label29.Text = "diferencias";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label30.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(406, 100);
            this.label30.Name = "label30";
            this.label30.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label30.Size = new System.Drawing.Size(87, 21);
            this.label30.TabIndex = 30;
            this.label30.Text = "diferencias";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(406, 144);
            this.label31.Name = "label31";
            this.label31.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label31.Size = new System.Drawing.Size(87, 21);
            this.label31.TabIndex = 44;
            this.label31.Text = "diferencias";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label32
            // 
            this.label32.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(406, 34);
            this.label32.Name = "label32";
            this.label32.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label32.Size = new System.Drawing.Size(87, 21);
            this.label32.TabIndex = 8;
            this.label32.Text = "diferencias";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label33
            // 
            this.label33.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(406, 122);
            this.label33.Name = "label33";
            this.label33.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label33.Size = new System.Drawing.Size(87, 21);
            this.label33.TabIndex = 37;
            this.label33.Text = "diferencias";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label34
            // 
            this.label34.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(406, 56);
            this.label34.Name = "label34";
            this.label34.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label34.Size = new System.Drawing.Size(87, 21);
            this.label34.TabIndex = 16;
            this.label34.Text = "diferencias";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label40
            // 
            this.label40.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label40.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(406, 210);
            this.label40.Name = "label40";
            this.label40.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label40.Size = new System.Drawing.Size(87, 21);
            this.label40.TabIndex = 67;
            this.label40.Text = "diferencias";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label41
            // 
            this.label41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(365, 210);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(12, 21);
            this.label41.TabIndex = 66;
            this.label41.Text = "a";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif17d
            // 
            this.txdif17d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif17d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif17d.Location = new System.Drawing.Point(379, 210);
            this.txdif17d.Name = "txdif17d";
            this.txdif17d.Size = new System.Drawing.Size(24, 21);
            this.txdif17d.TabIndex = 65;
            this.txdif17d.Text = "3";
            // 
            // txdif17
            // 
            this.txdif17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif17.Location = new System.Drawing.Point(340, 210);
            this.txdif17.Name = "txdif17";
            this.txdif17.Size = new System.Drawing.Size(24, 21);
            this.txdif17.TabIndex = 64;
            this.txdif17.Text = "1";
            // 
            // lblextremo17d
            // 
            this.lblextremo17d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo17d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo17d.Location = new System.Drawing.Point(239, 210);
            this.lblextremo17d.Name = "lblextremo17d";
            this.lblextremo17d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo17d.TabIndex = 63;
            this.lblextremo17d.Text = "0";
            // 
            // lblextremo17
            // 
            this.lblextremo17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo17.Location = new System.Drawing.Point(138, 210);
            this.lblextremo17.Name = "lblextremo17";
            this.lblextremo17.Size = new System.Drawing.Size(100, 21);
            this.lblextremo17.TabIndex = 62;
            this.lblextremo17.Text = "0";
            // 
            // label42
            // 
            this.label42.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label42.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(25, 144);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(112, 21);
            this.label42.TabIndex = 68;
            this.label42.Text = "de <14 a 13";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label43
            // 
            this.label43.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(406, 232);
            this.label43.Name = "label43";
            this.label43.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label43.Size = new System.Drawing.Size(87, 21);
            this.label43.TabIndex = 75;
            this.label43.Text = "diferencias";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.Location = new System.Drawing.Point(365, 232);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(12, 21);
            this.label44.TabIndex = 74;
            this.label44.Text = "a";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txdif18d
            // 
            this.txdif18d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif18d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif18d.Location = new System.Drawing.Point(379, 232);
            this.txdif18d.Name = "txdif18d";
            this.txdif18d.Size = new System.Drawing.Size(24, 21);
            this.txdif18d.TabIndex = 73;
            this.txdif18d.Text = "4";
            // 
            // txdif18
            // 
            this.txdif18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txdif18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txdif18.Location = new System.Drawing.Point(340, 232);
            this.txdif18.Name = "txdif18";
            this.txdif18.Size = new System.Drawing.Size(24, 21);
            this.txdif18.TabIndex = 72;
            this.txdif18.Text = "0";
            // 
            // lblextremo18d
            // 
            this.lblextremo18d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo18d.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo18d.Location = new System.Drawing.Point(239, 232);
            this.lblextremo18d.Name = "lblextremo18d";
            this.lblextremo18d.Size = new System.Drawing.Size(100, 21);
            this.lblextremo18d.TabIndex = 71;
            this.lblextremo18d.Text = "0";
            // 
            // lblextremo18
            // 
            this.lblextremo18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblextremo18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblextremo18.Location = new System.Drawing.Point(138, 232);
            this.lblextremo18.Name = "lblextremo18";
            this.lblextremo18.Size = new System.Drawing.Size(100, 21);
            this.lblextremo18.TabIndex = 70;
            this.lblextremo18.Text = "0";
            // 
            // label45
            // 
            this.label45.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label45.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(25, 122);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(112, 21);
            this.label45.TabIndex = 69;
            this.label45.Text = "de 13 a < 14";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DialogoFiltrarPorLimitesFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(518, 328);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.label44);
            this.Controls.Add(this.txdif18d);
            this.Controls.Add(this.txdif18);
            this.Controls.Add(this.lblextremo18d);
            this.Controls.Add(this.lblextremo18);
            this.Controls.Add(this.label45);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.txdif17d);
            this.Controls.Add(this.txdif17);
            this.Controls.Add(this.lblextremo17d);
            this.Controls.Add(this.lblextremo17);
            this.Controls.Add(this.btAceptar);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.txdif16d);
            this.Controls.Add(this.txdif16);
            this.Controls.Add(this.lblextremo16d);
            this.Controls.Add(this.lblextremo16);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.txdif15d);
            this.Controls.Add(this.txdif15);
            this.Controls.Add(this.lblextremo15d);
            this.Controls.Add(this.lblextremo15);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txdif14d);
            this.Controls.Add(this.txdif14);
            this.Controls.Add(this.lblextremo14d);
            this.Controls.Add(this.lblextremo14);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txdif13d);
            this.Controls.Add(this.txdif13);
            this.Controls.Add(this.lblextremo13d);
            this.Controls.Add(this.lblextremo13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txdif12d);
            this.Controls.Add(this.txdif12);
            this.Controls.Add(this.lblextremo12d);
            this.Controls.Add(this.lblextremo12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txdif11d);
            this.Controls.Add(this.txdif11);
            this.Controls.Add(this.lblextremo11d);
            this.Controls.Add(this.lblextremo11);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txdif10d);
            this.Controls.Add(this.txdif10);
            this.Controls.Add(this.lblextremo10d);
            this.Controls.Add(this.lblextremo10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txdif9d);
            this.Controls.Add(this.txdif9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblextremo9d);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblextremo9);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label34);
            this.Name = "DialogoFiltrarPorLimitesFrm";
            this.Text = "DialogoFiltrarPorLimitesFrm";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close ();
		}
		private void CoherenciarExtremos()
		{
			for (int i =0;i<9;i++)
			{
				if(extremos[i+1,0]<extremos[i,0]+1)extremos[i+1,0]=extremos[i,0]+1;
			}
		}
		private void PonerTextoEnVariables()
		{
			extremos [0,0]=Convert.ToInt32 (lblextremo9.Text);
			extremos [0,1]=Convert.ToInt32 (lblextremo9d.Text);
			extremos [0,2]=Convert.ToInt32 (txdif9.Text);
			extremos [0,3]=Convert.ToInt32 (txdif9d.Text);

			extremos [1,0]=Convert.ToInt32 (lblextremo10.Text);
			extremos [1,1]=Convert.ToInt32 (lblextremo10d.Text);
			extremos [1,2]=Convert.ToInt32 (txdif10.Text);
			extremos [1,3]=Convert.ToInt32 (txdif10d.Text);

			extremos [2,0]=Convert.ToInt32 (lblextremo11.Text);
			extremos [2,1]=Convert.ToInt32 (lblextremo11d.Text);
			extremos [2,2]=Convert.ToInt32 (txdif11.Text);
			extremos [2,3]=Convert.ToInt32 (txdif11d.Text);

			extremos [3,0]=Convert.ToInt32 (lblextremo12.Text);
			extremos [3,1]=Convert.ToInt32 (lblextremo12d.Text);
			extremos [3,2]=Convert.ToInt32 (txdif12.Text);
			extremos [3,3]=Convert.ToInt32 (txdif12d.Text);

			extremos [4,0]=Convert.ToInt32 (lblextremo13.Text);
			extremos [4,1]=Convert.ToInt32 (lblextremo13d.Text);
			extremos [4,2]=Convert.ToInt32 (txdif13.Text);
			extremos [4,3]=Convert.ToInt32 (txdif13d.Text);

			extremos [5,0]=Convert.ToInt32 (lblextremo14.Text);
			extremos [5,1]=Convert.ToInt32 (lblextremo14d.Text);
			extremos [5,2]=Convert.ToInt32 (txdif14.Text);
			extremos [5,3]=Convert.ToInt32 (txdif14d.Text);

			extremos [6,0]=Convert.ToInt32 (lblextremo15.Text);
			extremos [6,1]=Convert.ToInt32 (lblextremo15d.Text);
			extremos [6,2]=Convert.ToInt32 (txdif15.Text);
			extremos [6,3]=Convert.ToInt32 (txdif15d.Text);

			extremos [7,0]=Convert.ToInt32 (lblextremo16.Text);
			extremos [7,1]=Convert.ToInt32 (lblextremo16d.Text);
			extremos [7,2]=Convert.ToInt32 (txdif16.Text);
			extremos [7,3]=Convert.ToInt32 (txdif16d.Text);

			extremos [8,0]=Convert.ToInt32 (lblextremo17.Text);
			extremos [8,1]=Convert.ToInt32 (lblextremo17d.Text);
			extremos [8,2]=Convert.ToInt32 (txdif17.Text);
			extremos [8,3]=Convert.ToInt32 (txdif17d.Text);

			extremos [9,0]=Convert.ToInt32 (lblextremo17.Text);
			extremos [9,1]=Convert.ToInt32 (lblextremo17d.Text);
			extremos [9,2]=Convert.ToInt32 (txdif17.Text);
			extremos [9,3]=Convert.ToInt32 (txdif17d.Text);

		}

		private void btAceptar_Click(object sender, System.EventArgs e)
		{
			PonerTextoEnVariables();
			CoherenciarExtremos();
			ValoresAceptados=true;
			this.Close ();
		}

	}
}
