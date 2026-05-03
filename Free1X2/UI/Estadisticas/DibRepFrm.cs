// created on 28/04/2004 at 8:41
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 xfsf
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
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Estadisticas {
	public class DibRepFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label lt0006;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label153;
		private System.Windows.Forms.Label lt0313;
		private System.Windows.Forms.Label lt0114;
		private System.Windows.Forms.Label lncol;
		private System.Windows.Forms.Label lt0012;
		private System.Windows.Forms.Label lt0013;
		private System.Windows.Forms.Label lt0111;
		private System.Windows.Forms.Label lt0110;
		private System.Windows.Forms.Label lt0113;
		private System.Windows.Forms.Label lt0112;
		private System.Windows.Forms.Label lt0409;
		private System.Windows.Forms.Label lt0408;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label85;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label136;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lt0314;
		private System.Windows.Forms.Label lt0405;
		private System.Windows.Forms.Label lt0312;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lt0406;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lt0410;
		private System.Windows.Forms.Label lt0411;
		private System.Windows.Forms.Label lt0412;
		private System.Windows.Forms.RadioButton rbCols;
		private System.Windows.Forms.Label lt0414;
		private System.Windows.Forms.Label lt0009;
		private System.Windows.Forms.Label lt0008;
		private System.Windows.Forms.Label lt0403;
		private System.Windows.Forms.Label lt0402;
		private System.Windows.Forms.Label lt0005;
		private System.Windows.Forms.Label lt0004;
		private System.Windows.Forms.Label lt0007;
		private System.Windows.Forms.Label lt0311;
		private System.Windows.Forms.Label lt0001;
		private System.Windows.Forms.Label lt0000;
		private System.Windows.Forms.Label lt0003;
		private System.Windows.Forms.Label lt0002;
		private System.Windows.Forms.Label label119;
		private System.Windows.Forms.Label lt0208;
		private System.Windows.Forms.Label lt0207;
		private System.Windows.Forms.Label lt0206;
		private System.Windows.Forms.Label lt0205;
		private System.Windows.Forms.Label lt0204;
		private System.Windows.Forms.Label lt0203;
		private System.Windows.Forms.Label lt0202;
		private System.Windows.Forms.Label lt0201;
		private System.Windows.Forms.Label lt0014;
		private System.Windows.Forms.Label lt0209;
		private System.Windows.Forms.Label lt0010;
		private System.Windows.Forms.Label lt0011;
		private System.Windows.Forms.RadioButton rbPercent;
		private System.Windows.Forms.Label lt0401;
		private System.Windows.Forms.Label lt0400;
		private System.Windows.Forms.Label lt0200;
		private System.Windows.Forms.Label lt0404;
		private System.Windows.Forms.Label lt0407;
		private System.Windows.Forms.Label lt0310;
		private System.Windows.Forms.Label lt0214;
		private System.Windows.Forms.Label lt0212;
		private System.Windows.Forms.Label lt0213;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Label lt0108;
		private System.Windows.Forms.Label lt0109;
		private System.Windows.Forms.Label lt0104;
		private System.Windows.Forms.Label lt0105;
		private System.Windows.Forms.Label lt0106;
		private System.Windows.Forms.Label lt0107;
		private System.Windows.Forms.Label lt0100;
		private System.Windows.Forms.Label lt0101;
		private System.Windows.Forms.Label lt0102;
		private System.Windows.Forms.Label lt0103;
		private System.Windows.Forms.Label lt0210;
		private System.Windows.Forms.Label lt0211;
		private System.Windows.Forms.Label lt0413;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lt0308;
		private System.Windows.Forms.Label lt0309;
		private System.Windows.Forms.Label lt0306;
		private System.Windows.Forms.Label lt0307;
		private System.Windows.Forms.Label lt0304;
		private System.Windows.Forms.Label lt0305;
		private System.Windows.Forms.Label lt0302;
		private System.Windows.Forms.Label lt0303;
		private System.Windows.Forms.Label lt0300;
		private System.Windows.Forms.Label lt0301;
		private int[,] rsl;
		private int numcol;
		public DibRepFrm(int[,] ofparent, int ncol)
		{
			InitializeComponent();
			rsl = ofparent; numcol = ncol;
			PintaPantalla();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


		
		
		void InitializeComponent() {
            this.lt0301 = new System.Windows.Forms.Label();
            this.lt0300 = new System.Windows.Forms.Label();
            this.lt0303 = new System.Windows.Forms.Label();
            this.lt0302 = new System.Windows.Forms.Label();
            this.lt0305 = new System.Windows.Forms.Label();
            this.lt0304 = new System.Windows.Forms.Label();
            this.lt0307 = new System.Windows.Forms.Label();
            this.lt0306 = new System.Windows.Forms.Label();
            this.lt0309 = new System.Windows.Forms.Label();
            this.lt0308 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lt0413 = new System.Windows.Forms.Label();
            this.lt0211 = new System.Windows.Forms.Label();
            this.lt0210 = new System.Windows.Forms.Label();
            this.lt0103 = new System.Windows.Forms.Label();
            this.lt0102 = new System.Windows.Forms.Label();
            this.lt0101 = new System.Windows.Forms.Label();
            this.lt0100 = new System.Windows.Forms.Label();
            this.lt0107 = new System.Windows.Forms.Label();
            this.lt0106 = new System.Windows.Forms.Label();
            this.lt0105 = new System.Windows.Forms.Label();
            this.lt0104 = new System.Windows.Forms.Label();
            this.lt0109 = new System.Windows.Forms.Label();
            this.lt0108 = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.rbCols = new System.Windows.Forms.RadioButton();
            this.rbPercent = new System.Windows.Forms.RadioButton();
            this.lt0213 = new System.Windows.Forms.Label();
            this.lt0212 = new System.Windows.Forms.Label();
            this.lt0214 = new System.Windows.Forms.Label();
            this.lt0310 = new System.Windows.Forms.Label();
            this.lt0407 = new System.Windows.Forms.Label();
            this.lt0404 = new System.Windows.Forms.Label();
            this.lt0200 = new System.Windows.Forms.Label();
            this.lt0400 = new System.Windows.Forms.Label();
            this.lt0401 = new System.Windows.Forms.Label();
            this.lt0011 = new System.Windows.Forms.Label();
            this.lt0010 = new System.Windows.Forms.Label();
            this.lt0209 = new System.Windows.Forms.Label();
            this.lt0014 = new System.Windows.Forms.Label();
            this.lt0201 = new System.Windows.Forms.Label();
            this.lt0202 = new System.Windows.Forms.Label();
            this.lt0203 = new System.Windows.Forms.Label();
            this.lt0204 = new System.Windows.Forms.Label();
            this.lt0205 = new System.Windows.Forms.Label();
            this.lt0206 = new System.Windows.Forms.Label();
            this.lt0207 = new System.Windows.Forms.Label();
            this.lt0208 = new System.Windows.Forms.Label();
            this.label119 = new System.Windows.Forms.Label();
            this.lt0002 = new System.Windows.Forms.Label();
            this.lt0003 = new System.Windows.Forms.Label();
            this.lt0000 = new System.Windows.Forms.Label();
            this.lt0001 = new System.Windows.Forms.Label();
            this.lt0311 = new System.Windows.Forms.Label();
            this.lt0007 = new System.Windows.Forms.Label();
            this.lt0004 = new System.Windows.Forms.Label();
            this.lt0005 = new System.Windows.Forms.Label();
            this.lt0402 = new System.Windows.Forms.Label();
            this.lt0403 = new System.Windows.Forms.Label();
            this.lt0008 = new System.Windows.Forms.Label();
            this.lt0009 = new System.Windows.Forms.Label();
            this.lt0414 = new System.Windows.Forms.Label();
            this.lt0412 = new System.Windows.Forms.Label();
            this.lt0411 = new System.Windows.Forms.Label();
            this.lt0410 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lt0406 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lt0312 = new System.Windows.Forms.Label();
            this.lt0405 = new System.Windows.Forms.Label();
            this.lt0314 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label136 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lt0408 = new System.Windows.Forms.Label();
            this.lt0409 = new System.Windows.Forms.Label();
            this.lt0112 = new System.Windows.Forms.Label();
            this.lt0113 = new System.Windows.Forms.Label();
            this.lt0110 = new System.Windows.Forms.Label();
            this.lt0111 = new System.Windows.Forms.Label();
            this.lt0013 = new System.Windows.Forms.Label();
            this.lt0012 = new System.Windows.Forms.Label();
            this.lncol = new System.Windows.Forms.Label();
            this.lt0114 = new System.Windows.Forms.Label();
            this.lt0313 = new System.Windows.Forms.Label();
            this.label153 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lt0006 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // lt0301
            // 
            this.lt0301.BackColor = System.Drawing.Color.Bisque;
            this.lt0301.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0301.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0301.Location = new System.Drawing.Point(152, 120);
            this.lt0301.Name = "lt0301";
            this.lt0301.Size = new System.Drawing.Size(48, 16);
            this.lt0301.TabIndex = 122;
            this.lt0301.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0300
            // 
            this.lt0300.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0300.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0300.Location = new System.Drawing.Point(104, 120);
            this.lt0300.Name = "lt0300";
            this.lt0300.Size = new System.Drawing.Size(48, 16);
            this.lt0300.TabIndex = 123;
            this.lt0300.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0303
            // 
            this.lt0303.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0303.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0303.Location = new System.Drawing.Point(248, 120);
            this.lt0303.Name = "lt0303";
            this.lt0303.Size = new System.Drawing.Size(48, 16);
            this.lt0303.TabIndex = 120;
            this.lt0303.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0302
            // 
            this.lt0302.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0302.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0302.Location = new System.Drawing.Point(200, 120);
            this.lt0302.Name = "lt0302";
            this.lt0302.Size = new System.Drawing.Size(48, 16);
            this.lt0302.TabIndex = 121;
            this.lt0302.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0305
            // 
            this.lt0305.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0305.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0305.Location = new System.Drawing.Point(344, 120);
            this.lt0305.Name = "lt0305";
            this.lt0305.Size = new System.Drawing.Size(48, 16);
            this.lt0305.TabIndex = 130;
            this.lt0305.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0304
            // 
            this.lt0304.BackColor = System.Drawing.Color.Bisque;
            this.lt0304.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0304.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0304.Location = new System.Drawing.Point(296, 120);
            this.lt0304.Name = "lt0304";
            this.lt0304.Size = new System.Drawing.Size(48, 16);
            this.lt0304.TabIndex = 125;
            this.lt0304.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0307
            // 
            this.lt0307.BackColor = System.Drawing.Color.Bisque;
            this.lt0307.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0307.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0307.Location = new System.Drawing.Point(440, 120);
            this.lt0307.Name = "lt0307";
            this.lt0307.Size = new System.Drawing.Size(48, 16);
            this.lt0307.TabIndex = 128;
            this.lt0307.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0306
            // 
            this.lt0306.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0306.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0306.Location = new System.Drawing.Point(392, 120);
            this.lt0306.Name = "lt0306";
            this.lt0306.Size = new System.Drawing.Size(48, 16);
            this.lt0306.TabIndex = 129;
            this.lt0306.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0309
            // 
            this.lt0309.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0309.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0309.Location = new System.Drawing.Point(536, 120);
            this.lt0309.Name = "lt0309";
            this.lt0309.Size = new System.Drawing.Size(48, 16);
            this.lt0309.TabIndex = 126;
            this.lt0309.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0308
            // 
            this.lt0308.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0308.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0308.Location = new System.Drawing.Point(488, 120);
            this.lt0308.Name = "lt0308";
            this.lt0308.Size = new System.Drawing.Size(48, 16);
            this.lt0308.TabIndex = 127;
            this.lt0308.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(400, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 16);
            this.label8.TabIndex = 10;
            this.label8.Text = "6";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(160, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "1";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0413
            // 
            this.lt0413.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0413.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0413.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0413.Location = new System.Drawing.Point(728, 144);
            this.lt0413.Name = "lt0413";
            this.lt0413.Size = new System.Drawing.Size(48, 16);
            this.lt0413.TabIndex = 222;
            this.lt0413.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0211
            // 
            this.lt0211.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0211.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0211.Location = new System.Drawing.Point(632, 96);
            this.lt0211.Name = "lt0211";
            this.lt0211.Size = new System.Drawing.Size(48, 16);
            this.lt0211.TabIndex = 100;
            this.lt0211.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0210
            // 
            this.lt0210.BackColor = System.Drawing.Color.Bisque;
            this.lt0210.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0210.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0210.Location = new System.Drawing.Point(584, 96);
            this.lt0210.Name = "lt0210";
            this.lt0210.Size = new System.Drawing.Size(48, 16);
            this.lt0210.TabIndex = 101;
            this.lt0210.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0103
            // 
            this.lt0103.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0103.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0103.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0103.Location = new System.Drawing.Point(248, 72);
            this.lt0103.Name = "lt0103";
            this.lt0103.Size = new System.Drawing.Size(48, 16);
            this.lt0103.TabIndex = 69;
            this.lt0103.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0102
            // 
            this.lt0102.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0102.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0102.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0102.Location = new System.Drawing.Point(200, 72);
            this.lt0102.Name = "lt0102";
            this.lt0102.Size = new System.Drawing.Size(48, 16);
            this.lt0102.TabIndex = 70;
            this.lt0102.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0101
            // 
            this.lt0101.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0101.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0101.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0101.Location = new System.Drawing.Point(152, 72);
            this.lt0101.Name = "lt0101";
            this.lt0101.Size = new System.Drawing.Size(48, 16);
            this.lt0101.TabIndex = 71;
            this.lt0101.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0100
            // 
            this.lt0100.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0100.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0100.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0100.Location = new System.Drawing.Point(104, 72);
            this.lt0100.Name = "lt0100";
            this.lt0100.Size = new System.Drawing.Size(48, 16);
            this.lt0100.TabIndex = 72;
            this.lt0100.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0107
            // 
            this.lt0107.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0107.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0107.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0107.Location = new System.Drawing.Point(440, 72);
            this.lt0107.Name = "lt0107";
            this.lt0107.Size = new System.Drawing.Size(48, 16);
            this.lt0107.TabIndex = 77;
            this.lt0107.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0106
            // 
            this.lt0106.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0106.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0106.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0106.Location = new System.Drawing.Point(392, 72);
            this.lt0106.Name = "lt0106";
            this.lt0106.Size = new System.Drawing.Size(48, 16);
            this.lt0106.TabIndex = 78;
            this.lt0106.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0105
            // 
            this.lt0105.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0105.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0105.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0105.Location = new System.Drawing.Point(344, 72);
            this.lt0105.Name = "lt0105";
            this.lt0105.Size = new System.Drawing.Size(48, 16);
            this.lt0105.TabIndex = 79;
            this.lt0105.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0104
            // 
            this.lt0104.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0104.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0104.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0104.Location = new System.Drawing.Point(296, 72);
            this.lt0104.Name = "lt0104";
            this.lt0104.Size = new System.Drawing.Size(48, 16);
            this.lt0104.TabIndex = 74;
            this.lt0104.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0109
            // 
            this.lt0109.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0109.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0109.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0109.Location = new System.Drawing.Point(536, 72);
            this.lt0109.Name = "lt0109";
            this.lt0109.Size = new System.Drawing.Size(48, 16);
            this.lt0109.TabIndex = 75;
            this.lt0109.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0108
            // 
            this.lt0108.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0108.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0108.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0108.Location = new System.Drawing.Point(488, 72);
            this.lt0108.Name = "lt0108";
            this.lt0108.Size = new System.Drawing.Size(48, 16);
            this.lt0108.TabIndex = 76;
            this.lt0108.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.rbCols);
            this.groupBox.Controls.Add(this.rbPercent);
            this.groupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.Location = new System.Drawing.Point(616, 176);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(128, 80);
            this.groupBox.TabIndex = 291;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "mostrar";
            // 
            // rbCols
            // 
            this.rbCols.Location = new System.Drawing.Point(16, 48);
            this.rbCols.Name = "rbCols";
            this.rbCols.Size = new System.Drawing.Size(96, 16);
            this.rbCols.TabIndex = 1;
            this.rbCols.Text = "columnas";
            this.rbCols.CheckedChanged += new System.EventHandler(this.RbColsCheckedChanged);
            // 
            // rbPercent
            // 
            this.rbPercent.Checked = true;
            this.rbPercent.Location = new System.Drawing.Point(16, 24);
            this.rbPercent.Name = "rbPercent";
            this.rbPercent.Size = new System.Drawing.Size(96, 16);
            this.rbPercent.TabIndex = 0;
            this.rbPercent.TabStop = true;
            this.rbPercent.Text = "porcentajes";
            // 
            // lt0213
            // 
            this.lt0213.BackColor = System.Drawing.Color.Bisque;
            this.lt0213.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0213.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0213.Location = new System.Drawing.Point(728, 96);
            this.lt0213.Name = "lt0213";
            this.lt0213.Size = new System.Drawing.Size(48, 16);
            this.lt0213.TabIndex = 241;
            this.lt0213.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0212
            // 
            this.lt0212.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0212.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0212.Location = new System.Drawing.Point(680, 96);
            this.lt0212.Name = "lt0212";
            this.lt0212.Size = new System.Drawing.Size(48, 16);
            this.lt0212.TabIndex = 99;
            this.lt0212.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0214
            // 
            this.lt0214.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0214.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0214.Location = new System.Drawing.Point(776, 96);
            this.lt0214.Name = "lt0214";
            this.lt0214.Size = new System.Drawing.Size(48, 16);
            this.lt0214.TabIndex = 55;
            this.lt0214.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0310
            // 
            this.lt0310.BackColor = System.Drawing.Color.Bisque;
            this.lt0310.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0310.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0310.Location = new System.Drawing.Point(584, 120);
            this.lt0310.Name = "lt0310";
            this.lt0310.Size = new System.Drawing.Size(48, 16);
            this.lt0310.TabIndex = 135;
            this.lt0310.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0407
            // 
            this.lt0407.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0407.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0407.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0407.Location = new System.Drawing.Point(440, 144);
            this.lt0407.Name = "lt0407";
            this.lt0407.Size = new System.Drawing.Size(48, 16);
            this.lt0407.TabIndex = 111;
            this.lt0407.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0404
            // 
            this.lt0404.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0404.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0404.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0404.Location = new System.Drawing.Point(296, 144);
            this.lt0404.Name = "lt0404";
            this.lt0404.Size = new System.Drawing.Size(48, 16);
            this.lt0404.TabIndex = 108;
            this.lt0404.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0200
            // 
            this.lt0200.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0200.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0200.Location = new System.Drawing.Point(104, 96);
            this.lt0200.Name = "lt0200";
            this.lt0200.Size = new System.Drawing.Size(48, 16);
            this.lt0200.TabIndex = 89;
            this.lt0200.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0400
            // 
            this.lt0400.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0400.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0400.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0400.Location = new System.Drawing.Point(104, 144);
            this.lt0400.Name = "lt0400";
            this.lt0400.Size = new System.Drawing.Size(48, 16);
            this.lt0400.TabIndex = 106;
            this.lt0400.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0401
            // 
            this.lt0401.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0401.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0401.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0401.Location = new System.Drawing.Point(152, 144);
            this.lt0401.Name = "lt0401";
            this.lt0401.Size = new System.Drawing.Size(48, 16);
            this.lt0401.TabIndex = 105;
            this.lt0401.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0011
            // 
            this.lt0011.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0011.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0011.Location = new System.Drawing.Point(632, 48);
            this.lt0011.Name = "lt0011";
            this.lt0011.Size = new System.Drawing.Size(48, 16);
            this.lt0011.TabIndex = 32;
            this.lt0011.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0010
            // 
            this.lt0010.BackColor = System.Drawing.Color.Bisque;
            this.lt0010.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0010.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0010.Location = new System.Drawing.Point(584, 48);
            this.lt0010.Name = "lt0010";
            this.lt0010.Size = new System.Drawing.Size(48, 16);
            this.lt0010.TabIndex = 33;
            this.lt0010.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0209
            // 
            this.lt0209.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0209.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0209.Location = new System.Drawing.Point(536, 96);
            this.lt0209.Name = "lt0209";
            this.lt0209.Size = new System.Drawing.Size(48, 16);
            this.lt0209.TabIndex = 92;
            this.lt0209.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0014
            // 
            this.lt0014.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0014.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0014.Location = new System.Drawing.Point(776, 48);
            this.lt0014.Name = "lt0014";
            this.lt0014.Size = new System.Drawing.Size(48, 16);
            this.lt0014.TabIndex = 29;
            this.lt0014.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0201
            // 
            this.lt0201.BackColor = System.Drawing.Color.Bisque;
            this.lt0201.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0201.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0201.Location = new System.Drawing.Point(152, 96);
            this.lt0201.Name = "lt0201";
            this.lt0201.Size = new System.Drawing.Size(48, 16);
            this.lt0201.TabIndex = 88;
            this.lt0201.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0202
            // 
            this.lt0202.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0202.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0202.Location = new System.Drawing.Point(200, 96);
            this.lt0202.Name = "lt0202";
            this.lt0202.Size = new System.Drawing.Size(48, 16);
            this.lt0202.TabIndex = 87;
            this.lt0202.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0203
            // 
            this.lt0203.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0203.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0203.Location = new System.Drawing.Point(248, 96);
            this.lt0203.Name = "lt0203";
            this.lt0203.Size = new System.Drawing.Size(48, 16);
            this.lt0203.TabIndex = 86;
            this.lt0203.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0204
            // 
            this.lt0204.BackColor = System.Drawing.Color.Bisque;
            this.lt0204.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0204.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0204.Location = new System.Drawing.Point(296, 96);
            this.lt0204.Name = "lt0204";
            this.lt0204.Size = new System.Drawing.Size(48, 16);
            this.lt0204.TabIndex = 91;
            this.lt0204.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0205
            // 
            this.lt0205.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0205.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0205.Location = new System.Drawing.Point(344, 96);
            this.lt0205.Name = "lt0205";
            this.lt0205.Size = new System.Drawing.Size(48, 16);
            this.lt0205.TabIndex = 96;
            this.lt0205.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0206
            // 
            this.lt0206.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0206.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0206.Location = new System.Drawing.Point(392, 96);
            this.lt0206.Name = "lt0206";
            this.lt0206.Size = new System.Drawing.Size(48, 16);
            this.lt0206.TabIndex = 95;
            this.lt0206.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0207
            // 
            this.lt0207.BackColor = System.Drawing.Color.Bisque;
            this.lt0207.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0207.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0207.Location = new System.Drawing.Point(440, 96);
            this.lt0207.Name = "lt0207";
            this.lt0207.Size = new System.Drawing.Size(48, 16);
            this.lt0207.TabIndex = 94;
            this.lt0207.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0208
            // 
            this.lt0208.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0208.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0208.Location = new System.Drawing.Point(488, 96);
            this.lt0208.Name = "lt0208";
            this.lt0208.Size = new System.Drawing.Size(48, 16);
            this.lt0208.TabIndex = 93;
            this.lt0208.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label119
            // 
            this.label119.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label119.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label119.Location = new System.Drawing.Point(16, 120);
            this.label119.Name = "label119";
            this.label119.Size = new System.Drawing.Size(80, 16);
            this.label119.TabIndex = 119;
            this.label119.Text = "cant. 2";
            this.label119.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0002
            // 
            this.lt0002.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0002.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0002.Location = new System.Drawing.Point(200, 48);
            this.lt0002.Name = "lt0002";
            this.lt0002.Size = new System.Drawing.Size(48, 16);
            this.lt0002.TabIndex = 19;
            this.lt0002.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0003
            // 
            this.lt0003.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0003.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0003.Location = new System.Drawing.Point(248, 48);
            this.lt0003.Name = "lt0003";
            this.lt0003.Size = new System.Drawing.Size(48, 16);
            this.lt0003.TabIndex = 18;
            this.lt0003.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0000
            // 
            this.lt0000.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0000.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0000.Location = new System.Drawing.Point(104, 48);
            this.lt0000.Name = "lt0000";
            this.lt0000.Size = new System.Drawing.Size(48, 16);
            this.lt0000.TabIndex = 21;
            this.lt0000.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0001
            // 
            this.lt0001.BackColor = System.Drawing.Color.Bisque;
            this.lt0001.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0001.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0001.Location = new System.Drawing.Point(152, 48);
            this.lt0001.Name = "lt0001";
            this.lt0001.Size = new System.Drawing.Size(48, 16);
            this.lt0001.TabIndex = 20;
            this.lt0001.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0311
            // 
            this.lt0311.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0311.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0311.Location = new System.Drawing.Point(632, 120);
            this.lt0311.Name = "lt0311";
            this.lt0311.Size = new System.Drawing.Size(48, 16);
            this.lt0311.TabIndex = 134;
            this.lt0311.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0007
            // 
            this.lt0007.BackColor = System.Drawing.Color.Bisque;
            this.lt0007.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0007.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0007.Location = new System.Drawing.Point(440, 48);
            this.lt0007.Name = "lt0007";
            this.lt0007.Size = new System.Drawing.Size(48, 16);
            this.lt0007.TabIndex = 26;
            this.lt0007.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0004
            // 
            this.lt0004.BackColor = System.Drawing.Color.Bisque;
            this.lt0004.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0004.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0004.Location = new System.Drawing.Point(296, 48);
            this.lt0004.Name = "lt0004";
            this.lt0004.Size = new System.Drawing.Size(48, 16);
            this.lt0004.TabIndex = 23;
            this.lt0004.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0005
            // 
            this.lt0005.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0005.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0005.Location = new System.Drawing.Point(344, 48);
            this.lt0005.Name = "lt0005";
            this.lt0005.Size = new System.Drawing.Size(48, 16);
            this.lt0005.TabIndex = 28;
            this.lt0005.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0402
            // 
            this.lt0402.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0402.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0402.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0402.Location = new System.Drawing.Point(200, 144);
            this.lt0402.Name = "lt0402";
            this.lt0402.Size = new System.Drawing.Size(48, 16);
            this.lt0402.TabIndex = 104;
            this.lt0402.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0403
            // 
            this.lt0403.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0403.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0403.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0403.Location = new System.Drawing.Point(248, 144);
            this.lt0403.Name = "lt0403";
            this.lt0403.Size = new System.Drawing.Size(48, 16);
            this.lt0403.TabIndex = 103;
            this.lt0403.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0008
            // 
            this.lt0008.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0008.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0008.Location = new System.Drawing.Point(488, 48);
            this.lt0008.Name = "lt0008";
            this.lt0008.Size = new System.Drawing.Size(48, 16);
            this.lt0008.TabIndex = 25;
            this.lt0008.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0009
            // 
            this.lt0009.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0009.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0009.Location = new System.Drawing.Point(536, 48);
            this.lt0009.Name = "lt0009";
            this.lt0009.Size = new System.Drawing.Size(48, 16);
            this.lt0009.TabIndex = 24;
            this.lt0009.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0414
            // 
            this.lt0414.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0414.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0414.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0414.Location = new System.Drawing.Point(776, 144);
            this.lt0414.Name = "lt0414";
            this.lt0414.Size = new System.Drawing.Size(48, 16);
            this.lt0414.TabIndex = 222;
            this.lt0414.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0412
            // 
            this.lt0412.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0412.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0412.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0412.Location = new System.Drawing.Point(680, 144);
            this.lt0412.Name = "lt0412";
            this.lt0412.Size = new System.Drawing.Size(48, 16);
            this.lt0412.TabIndex = 227;
            this.lt0412.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0411
            // 
            this.lt0411.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0411.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0411.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0411.Location = new System.Drawing.Point(632, 144);
            this.lt0411.Name = "lt0411";
            this.lt0411.Size = new System.Drawing.Size(48, 16);
            this.lt0411.TabIndex = 276;
            this.lt0411.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0410
            // 
            this.lt0410.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0410.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0410.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0410.Location = new System.Drawing.Point(584, 144);
            this.lt0410.Name = "lt0410";
            this.lt0410.Size = new System.Drawing.Size(48, 16);
            this.lt0410.TabIndex = 118;
            this.lt0410.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(256, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "3";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0406
            // 
            this.lt0406.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0406.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0406.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0406.Location = new System.Drawing.Point(392, 144);
            this.lt0406.Name = "lt0406";
            this.lt0406.Size = new System.Drawing.Size(48, 16);
            this.lt0406.TabIndex = 112;
            this.lt0406.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(448, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 16);
            this.label9.TabIndex = 9;
            this.label9.Text = "7";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0312
            // 
            this.lt0312.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0312.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0312.Location = new System.Drawing.Point(680, 120);
            this.lt0312.Name = "lt0312";
            this.lt0312.Size = new System.Drawing.Size(48, 16);
            this.lt0312.TabIndex = 240;
            this.lt0312.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0405
            // 
            this.lt0405.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0405.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0405.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0405.Location = new System.Drawing.Point(344, 144);
            this.lt0405.Name = "lt0405";
            this.lt0405.Size = new System.Drawing.Size(48, 16);
            this.lt0405.TabIndex = 113;
            this.lt0405.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0314
            // 
            this.lt0314.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0314.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0314.Location = new System.Drawing.Point(776, 120);
            this.lt0314.Name = "lt0314";
            this.lt0314.Size = new System.Drawing.Size(48, 16);
            this.lt0314.TabIndex = 256;
            this.lt0314.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(112, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "0";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(544, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 16);
            this.label11.TabIndex = 7;
            this.label11.Text = "9";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(352, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 16);
            this.label7.TabIndex = 11;
            this.label7.Text = "5";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label136
            // 
            this.label136.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label136.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label136.Location = new System.Drawing.Point(16, 144);
            this.label136.Name = "label136";
            this.label136.Size = new System.Drawing.Size(80, 16);
            this.label136.TabIndex = 102;
            this.label136.Text = "cant. V";
            this.label136.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(304, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 16);
            this.label12.TabIndex = 6;
            this.label12.Text = "4";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label85
            // 
            this.label85.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label85.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label85.Location = new System.Drawing.Point(16, 72);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(80, 16);
            this.label85.TabIndex = 68;
            this.label85.Text = "cant. 1";
            this.label85.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(208, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "2";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0408
            // 
            this.lt0408.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0408.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0408.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0408.Location = new System.Drawing.Point(488, 144);
            this.lt0408.Name = "lt0408";
            this.lt0408.Size = new System.Drawing.Size(48, 16);
            this.lt0408.TabIndex = 110;
            this.lt0408.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0409
            // 
            this.lt0409.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0409.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0409.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0409.Location = new System.Drawing.Point(536, 144);
            this.lt0409.Name = "lt0409";
            this.lt0409.Size = new System.Drawing.Size(48, 16);
            this.lt0409.TabIndex = 109;
            this.lt0409.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0112
            // 
            this.lt0112.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0112.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0112.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0112.Location = new System.Drawing.Point(680, 72);
            this.lt0112.Name = "lt0112";
            this.lt0112.Size = new System.Drawing.Size(48, 16);
            this.lt0112.TabIndex = 82;
            this.lt0112.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0113
            // 
            this.lt0113.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0113.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0113.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0113.Location = new System.Drawing.Point(728, 72);
            this.lt0113.Name = "lt0113";
            this.lt0113.Size = new System.Drawing.Size(48, 16);
            this.lt0113.TabIndex = 81;
            this.lt0113.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0110
            // 
            this.lt0110.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0110.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0110.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0110.Location = new System.Drawing.Point(584, 72);
            this.lt0110.Name = "lt0110";
            this.lt0110.Size = new System.Drawing.Size(48, 16);
            this.lt0110.TabIndex = 84;
            this.lt0110.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0111
            // 
            this.lt0111.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0111.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0111.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0111.Location = new System.Drawing.Point(632, 72);
            this.lt0111.Name = "lt0111";
            this.lt0111.Size = new System.Drawing.Size(48, 16);
            this.lt0111.TabIndex = 83;
            this.lt0111.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0013
            // 
            this.lt0013.BackColor = System.Drawing.Color.Bisque;
            this.lt0013.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0013.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0013.Location = new System.Drawing.Point(728, 48);
            this.lt0013.Name = "lt0013";
            this.lt0013.Size = new System.Drawing.Size(48, 16);
            this.lt0013.TabIndex = 30;
            this.lt0013.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0012
            // 
            this.lt0012.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0012.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0012.Location = new System.Drawing.Point(680, 48);
            this.lt0012.Name = "lt0012";
            this.lt0012.Size = new System.Drawing.Size(48, 16);
            this.lt0012.TabIndex = 31;
            this.lt0012.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lncol
            // 
            this.lncol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lncol.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lncol.Location = new System.Drawing.Point(16, 16);
            this.lncol.Name = "lncol";
            this.lncol.Size = new System.Drawing.Size(64, 24);
            this.lncol.TabIndex = 4;
            this.lncol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0114
            // 
            this.lt0114.BackColor = System.Drawing.Color.NavajoWhite;
            this.lt0114.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0114.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0114.Location = new System.Drawing.Point(776, 72);
            this.lt0114.Name = "lt0114";
            this.lt0114.Size = new System.Drawing.Size(48, 16);
            this.lt0114.TabIndex = 275;
            this.lt0114.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0313
            // 
            this.lt0313.BackColor = System.Drawing.Color.Bisque;
            this.lt0313.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0313.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0313.Location = new System.Drawing.Point(728, 120);
            this.lt0313.Name = "lt0313";
            this.lt0313.Size = new System.Drawing.Size(48, 16);
            this.lt0313.TabIndex = 241;
            this.lt0313.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label153
            // 
            this.label153.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label153.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label153.Location = new System.Drawing.Point(16, 96);
            this.label153.Name = "label153";
            this.label153.Size = new System.Drawing.Size(80, 16);
            this.label153.TabIndex = 85;
            this.label153.Text = "cant. X";
            this.label153.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(688, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 16);
            this.label15.TabIndex = 14;
            this.label15.Text = "12";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(640, 24);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 16);
            this.label14.TabIndex = 15;
            this.label14.Text = "11";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(784, 24);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 16);
            this.label17.TabIndex = 12;
            this.label17.Text = "14";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(736, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 16);
            this.label16.TabIndex = 13;
            this.label16.Text = "13";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(496, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 16);
            this.label10.TabIndex = 8;
            this.label10.Text = "8";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(592, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 16);
            this.label13.TabIndex = 16;
            this.label13.Text = "10";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lt0006
            // 
            this.lt0006.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lt0006.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lt0006.Location = new System.Drawing.Point(392, 48);
            this.lt0006.Name = "lt0006";
            this.lt0006.Size = new System.Drawing.Size(48, 16);
            this.lt0006.TabIndex = 27;
            this.lt0006.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label34
            // 
            this.label34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(16, 48);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(80, 16);
            this.label34.TabIndex = 17;
            this.label34.Text = "posiciones";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DibRepFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(840, 274);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.lt0411);
            this.Controls.Add(this.lt0114);
            this.Controls.Add(this.lt0314);
            this.Controls.Add(this.lt0213);
            this.Controls.Add(this.lt0312);
            this.Controls.Add(this.lt0412);
            this.Controls.Add(this.lt0413);
            this.Controls.Add(this.lt0310);
            this.Controls.Add(this.lt0311);
            this.Controls.Add(this.lt0305);
            this.Controls.Add(this.lt0306);
            this.Controls.Add(this.lt0307);
            this.Controls.Add(this.lt0308);
            this.Controls.Add(this.lt0309);
            this.Controls.Add(this.lt0304);
            this.Controls.Add(this.lt0300);
            this.Controls.Add(this.lt0301);
            this.Controls.Add(this.lt0302);
            this.Controls.Add(this.lt0303);
            this.Controls.Add(this.label119);
            this.Controls.Add(this.lt0410);
            this.Controls.Add(this.lt0405);
            this.Controls.Add(this.lt0406);
            this.Controls.Add(this.lt0407);
            this.Controls.Add(this.lt0408);
            this.Controls.Add(this.lt0409);
            this.Controls.Add(this.lt0404);
            this.Controls.Add(this.lt0400);
            this.Controls.Add(this.lt0401);
            this.Controls.Add(this.lt0402);
            this.Controls.Add(this.lt0403);
            this.Controls.Add(this.label136);
            this.Controls.Add(this.lt0210);
            this.Controls.Add(this.lt0211);
            this.Controls.Add(this.lt0212);
            this.Controls.Add(this.lt0205);
            this.Controls.Add(this.lt0206);
            this.Controls.Add(this.lt0207);
            this.Controls.Add(this.lt0208);
            this.Controls.Add(this.lt0209);
            this.Controls.Add(this.lt0204);
            this.Controls.Add(this.lt0200);
            this.Controls.Add(this.lt0201);
            this.Controls.Add(this.lt0202);
            this.Controls.Add(this.lt0203);
            this.Controls.Add(this.label153);
            this.Controls.Add(this.lt0110);
            this.Controls.Add(this.lt0111);
            this.Controls.Add(this.lt0112);
            this.Controls.Add(this.lt0113);
            this.Controls.Add(this.lt0105);
            this.Controls.Add(this.lt0106);
            this.Controls.Add(this.lt0107);
            this.Controls.Add(this.lt0108);
            this.Controls.Add(this.lt0109);
            this.Controls.Add(this.lt0104);
            this.Controls.Add(this.lt0100);
            this.Controls.Add(this.lt0101);
            this.Controls.Add(this.lt0102);
            this.Controls.Add(this.lt0103);
            this.Controls.Add(this.label85);
            this.Controls.Add(this.lt0010);
            this.Controls.Add(this.lt0011);
            this.Controls.Add(this.lt0012);
            this.Controls.Add(this.lt0013);
            this.Controls.Add(this.lt0014);
            this.Controls.Add(this.lt0005);
            this.Controls.Add(this.lt0006);
            this.Controls.Add(this.lt0007);
            this.Controls.Add(this.lt0008);
            this.Controls.Add(this.lt0009);
            this.Controls.Add(this.lt0004);
            this.Controls.Add(this.lt0000);
            this.Controls.Add(this.lt0001);
            this.Controls.Add(this.lt0002);
            this.Controls.Add(this.lt0003);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lt0214);
            this.Controls.Add(this.lt0414);
            this.Controls.Add(this.lt0313);
            this.Controls.Add(this.lncol);
            this.Name = "DibRepFrm";
            this.Text = "coincidencias";
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		void RbColsCheckedChanged(object sender, System.EventArgs e) { PintaPantalla(); }

		private void PintaPantalla() {
			lncol.Text = ""+numcol;
			if (rbPercent.Checked) Porcentajes();
			else PintaColumnas();
		}
		private void Porcentajes() {
			lt0000.Text=""+(rsl[0,0]*10000/numcol)/1E2;
			lt0001.Text=""+(rsl[0,1]*10000/numcol)/1E2;
			lt0002.Text=""+(rsl[0,2]*10000/numcol)/1E2;
			lt0003.Text=""+(rsl[0,3]*10000/numcol)/1E2;
			lt0004.Text=""+(rsl[0,4]*10000/numcol)/1E2;
			lt0005.Text=""+(rsl[0,5]*10000/numcol)/1E2;
			lt0006.Text=""+(rsl[0,6]*10000/numcol)/1E2;
			lt0007.Text=""+(rsl[0,7]*10000/numcol)/1E2;
			lt0008.Text=""+(rsl[0,8]*10000/numcol)/1E2;
			lt0009.Text=""+(rsl[0,9]*10000/numcol)/1E2;
			lt0010.Text=""+(rsl[0,10]*10000/numcol)/1E2;
			lt0011.Text=""+(rsl[0,11]*10000/numcol)/1E2;
			lt0012.Text=""+(rsl[0,12]*10000/numcol)/1E2;
			lt0013.Text=""+(rsl[0,13]*10000/numcol)/1E2;
			lt0014.Text=""+(rsl[0,14]*10000/numcol)/1E2;
			lt0100.Text=""+(rsl[1,0]*10000/numcol)/1E2;
			lt0101.Text=""+(rsl[1,1]*10000/numcol)/1E2;
			lt0102.Text=""+(rsl[1,2]*10000/numcol)/1E2;
			lt0103.Text=""+(rsl[1,3]*10000/numcol)/1E2;
			lt0104.Text=""+(rsl[1,4]*10000/numcol)/1E2;
			lt0105.Text=""+(rsl[1,5]*10000/numcol)/1E2;
			lt0106.Text=""+(rsl[1,6]*10000/numcol)/1E2;
			lt0107.Text=""+(rsl[1,7]*10000/numcol)/1E2;
			lt0108.Text=""+(rsl[1,8]*10000/numcol)/1E2;
			lt0109.Text=""+(rsl[1,9]*10000/numcol)/1E2;
			lt0110.Text=""+(rsl[1,10]*10000/numcol)/1E2;
			lt0111.Text=""+(rsl[1,11]*10000/numcol)/1E2;
			lt0112.Text=""+(rsl[1,12]*10000/numcol)/1E2;
			lt0113.Text=""+(rsl[1,13]*10000/numcol)/1E2;
			lt0114.Text=""+(rsl[1,14]*10000/numcol)/1E2;
			lt0200.Text=""+(rsl[2,0]*10000/numcol)/1E2;
			lt0201.Text=""+(rsl[2,1]*10000/numcol)/1E2;
			lt0202.Text=""+(rsl[2,2]*10000/numcol)/1E2;
			lt0203.Text=""+(rsl[2,3]*10000/numcol)/1E2;
			lt0204.Text=""+(rsl[2,4]*10000/numcol)/1E2;
			lt0205.Text=""+(rsl[2,5]*10000/numcol)/1E2;
			lt0206.Text=""+(rsl[2,6]*10000/numcol)/1E2;
			lt0207.Text=""+(rsl[2,7]*10000/numcol)/1E2;
			lt0208.Text=""+(rsl[2,8]*10000/numcol)/1E2;
			lt0209.Text=""+(rsl[2,9]*10000/numcol)/1E2;
			lt0210.Text=""+(rsl[2,10]*10000/numcol)/1E2;
			lt0211.Text=""+(rsl[2,11]*10000/numcol)/1E2;
			lt0212.Text=""+(rsl[2,12]*10000/numcol)/1E2;
			lt0213.Text=""+(rsl[2,13]*10000/numcol)/1E2;
			lt0214.Text=""+(rsl[2,14]*10000/numcol)/1E2;
			lt0300.Text=""+(rsl[3,0]*10000/numcol)/1E2;
			lt0301.Text=""+(rsl[3,1]*10000/numcol)/1E2;
			lt0302.Text=""+(rsl[3,2]*10000/numcol)/1E2;
			lt0303.Text=""+(rsl[3,3]*10000/numcol)/1E2;
			lt0304.Text=""+(rsl[3,4]*10000/numcol)/1E2;
			lt0305.Text=""+(rsl[3,5]*10000/numcol)/1E2;
			lt0306.Text=""+(rsl[3,6]*10000/numcol)/1E2;
			lt0307.Text=""+(rsl[3,7]*10000/numcol)/1E2;
			lt0308.Text=""+(rsl[3,8]*10000/numcol)/1E2;
			lt0309.Text=""+(rsl[3,9]*10000/numcol)/1E2;
			lt0310.Text=""+(rsl[3,10]*10000/numcol)/1E2;
			lt0311.Text=""+(rsl[3,11]*10000/numcol)/1E2;
			lt0312.Text=""+(rsl[3,12]*10000/numcol)/1E2;
			lt0313.Text=""+(rsl[3,13]*10000/numcol)/1E2;
			lt0314.Text=""+(rsl[3,14]*10000/numcol)/1E2;
			lt0400.Text=""+(rsl[4,0]*10000/numcol)/1E2;
			lt0401.Text=""+(rsl[4,1]*10000/numcol)/1E2;
			lt0402.Text=""+(rsl[4,2]*10000/numcol)/1E2;
			lt0403.Text=""+(rsl[4,3]*10000/numcol)/1E2;
			lt0404.Text=""+(rsl[4,4]*10000/numcol)/1E2;
			lt0405.Text=""+(rsl[4,5]*10000/numcol)/1E2;
			lt0406.Text=""+(rsl[4,6]*10000/numcol)/1E2;
			lt0407.Text=""+(rsl[4,7]*10000/numcol)/1E2;
			lt0408.Text=""+(rsl[4,8]*10000/numcol)/1E2;
			lt0409.Text=""+(rsl[4,9]*10000/numcol)/1E2;
			lt0410.Text=""+(rsl[4,10]*10000/numcol)/1E2;
			lt0411.Text=""+(rsl[4,11]*10000/numcol)/1E2;
			lt0412.Text=""+(rsl[4,12]*10000/numcol)/1E2;
			lt0413.Text=""+(rsl[4,13]*10000/numcol)/1E2;
			lt0414.Text=""+(rsl[4,14]*10000/numcol)/1E2;
		}
		private void PintaColumnas() {
			lt0000.Text=""+rsl[0,0];
			lt0001.Text=""+rsl[0,1];
			lt0002.Text=""+rsl[0,2];
			lt0003.Text=""+rsl[0,3];
			lt0004.Text=""+rsl[0,4];
			lt0005.Text=""+rsl[0,5];
			lt0006.Text=""+rsl[0,6];
			lt0007.Text=""+rsl[0,7];
			lt0008.Text=""+rsl[0,8];
			lt0009.Text=""+rsl[0,9];
			lt0010.Text=""+rsl[0,10];
			lt0011.Text=""+rsl[0,11];
			lt0012.Text=""+rsl[0,12];
			lt0013.Text=""+rsl[0,13];
			lt0014.Text=""+rsl[0,14];
			lt0100.Text=""+rsl[1,0];
			lt0101.Text=""+rsl[1,1];
			lt0102.Text=""+rsl[1,2];
			lt0103.Text=""+rsl[1,3];
			lt0104.Text=""+rsl[1,4];
			lt0105.Text=""+rsl[1,5];
			lt0106.Text=""+rsl[1,6];
			lt0107.Text=""+rsl[1,7];
			lt0108.Text=""+rsl[1,8];
			lt0109.Text=""+rsl[1,9];
			lt0110.Text=""+rsl[1,10];
			lt0111.Text=""+rsl[1,11];
			lt0112.Text=""+rsl[1,12];
			lt0113.Text=""+rsl[1,13];
			lt0114.Text=""+rsl[1,14];
			lt0200.Text=""+rsl[2,0];
			lt0201.Text=""+rsl[2,1];
			lt0202.Text=""+rsl[2,2];
			lt0203.Text=""+rsl[2,3];
			lt0204.Text=""+rsl[2,4];
			lt0205.Text=""+rsl[2,5];
			lt0206.Text=""+rsl[2,6];
			lt0207.Text=""+rsl[2,7];
			lt0208.Text=""+rsl[2,8];
			lt0209.Text=""+rsl[2,9];
			lt0210.Text=""+rsl[2,10];
			lt0211.Text=""+rsl[2,11];
			lt0212.Text=""+rsl[2,12];
			lt0213.Text=""+rsl[2,13];
			lt0214.Text=""+rsl[2,14];
			lt0300.Text=""+rsl[3,0];
			lt0301.Text=""+rsl[3,1];
			lt0302.Text=""+rsl[3,2];
			lt0303.Text=""+rsl[3,3];
			lt0304.Text=""+rsl[3,4];
			lt0305.Text=""+rsl[3,5];
			lt0306.Text=""+rsl[3,6];
			lt0307.Text=""+rsl[3,7];
			lt0308.Text=""+rsl[3,8];
			lt0309.Text=""+rsl[3,9];
			lt0310.Text=""+rsl[3,10];
			lt0311.Text=""+rsl[3,11];
			lt0312.Text=""+rsl[3,12];
			lt0313.Text=""+rsl[3,13];
			lt0314.Text=""+rsl[3,14];
			lt0400.Text=""+rsl[4,0];
			lt0401.Text=""+rsl[4,1];
			lt0402.Text=""+rsl[4,2];
			lt0403.Text=""+rsl[4,3];
			lt0404.Text=""+rsl[4,4];
			lt0405.Text=""+rsl[4,5];
			lt0406.Text=""+rsl[4,6];
			lt0407.Text=""+rsl[4,7];
			lt0408.Text=""+rsl[4,8];
			lt0409.Text=""+rsl[4,9];
			lt0410.Text=""+rsl[4,10];
			lt0411.Text=""+rsl[4,11];
			lt0412.Text=""+rsl[4,12];
			lt0413.Text=""+rsl[4,13];
			lt0414.Text=""+rsl[4,14];
		}		

	}
}
