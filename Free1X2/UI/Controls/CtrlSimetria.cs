// Free1X2 : Programa de quinielas "libre"
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
using System.ComponentModel;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Summary description for CtrlSimetria.
	/// </summary>
	public class CtrlSimetria : UserControl
	{
		private TextBox txtSimetria;
        private Label lblNum;
        private IContainer components;
        private int click = 1;
		public CtrlSimetria(int num)
		{
			InitializeComponent();
			lblNum.Text = num.ToString();
		}
		public Label LblNum
		{
			get {return lblNum;}
		}
		public TextBox TxtSimetria
		{
			get {return txtSimetria;}
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.txtSimetria = new System.Windows.Forms.TextBox();
            this.lblNum = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSimetria
            // 
            this.txtSimetria.BackColor = System.Drawing.Color.White;
            this.txtSimetria.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSimetria.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSimetria.Location = new System.Drawing.Point(37, 1);
            this.txtSimetria.MaxLength = 32;
            this.txtSimetria.Name = "txtSimetria";
            this.txtSimetria.Size = new System.Drawing.Size(160, 14);
            this.txtSimetria.TabIndex = 82;
            // 
            // lblNum
            // 
            this.lblNum.Location = new System.Drawing.Point(3, 1);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(30, 14);
            this.lblNum.TabIndex = 81;
            this.lblNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNum.Click += new System.EventHandler(this.lblNum_Click);
            // 
            // CtrlSimetria
            // 
            this.Controls.Add(this.txtSimetria);
            this.Controls.Add(this.lblNum);
            this.Name = "CtrlSimetria";
            this.Size = new System.Drawing.Size(200, 16);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void lblNum_Click(object sender, EventArgs e)
        {
            if (click < VariablesGlobales.NumeroPartidos)
            {
                click++;
            }
            else
            {
                click = 2;
            }
            if (click > 1)
            {
                TxtSimetria.Text = "1-" + click;
            }
        }

    }
}
