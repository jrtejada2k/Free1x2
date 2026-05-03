// created on 28/02/2004 at 14:43
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Toni moreno
//	Idea original: Luiz Carlos Duarte
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
namespace Free1X2.UI.Controls
{
	public class ModificadorOptions : UserControl
	{
		private TextBox val_X;
		private TextBox val_1;
		private TextBox val_2;
		private CheckBox chckActive;
		public ModificadorOptions()
		{
			InitializeComponent();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

						
		public int NumeroPartido
		{
			get{ return Convert.ToInt32(chckActive.Text); }
			set{ chckActive.Text = (value).ToString();}
		}
		
		public string Valor_1
		{
			get{ return val_1.Text;}
			set{ val_1.Text = value;}		
		}
		
		public string Valor_X
		{
			get{ return val_X.Text;}
			set{ val_X.Text = value;}		
		}
		
		public string Valor_2
		{
			get{ return val_2.Text;}
			set{ val_2.Text = value;}		
		}
		
		public bool PartidoActivo
		{
			get{ return chckActive.Checked; }
			set{ chckActive.Checked = value; }
		}

		void InitializeComponent() {
            this.chckActive = new System.Windows.Forms.CheckBox();
            this.val_2 = new System.Windows.Forms.TextBox();
            this.val_1 = new System.Windows.Forms.TextBox();
            this.val_X = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chckActive
            // 
            this.chckActive.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chckActive.Location = new System.Drawing.Point(7, 0);
            this.chckActive.Name = "chckActive";
            this.chckActive.Size = new System.Drawing.Size(44, 20);
            this.chckActive.TabIndex = 0;
            this.chckActive.Text = "14";
            this.chckActive.CheckedChanged += new System.EventHandler(this.ChckActiveCheckedChanged);
            // 
            // val_2
            // 
            this.val_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.val_2.Enabled = false;
            this.val_2.Location = new System.Drawing.Point(112, 0);
            this.val_2.MaxLength = 3;
            this.val_2.Name = "val_2";
            this.val_2.Size = new System.Drawing.Size(30, 20);
            this.val_2.TabIndex = 3;
            this.val_2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // val_1
            // 
            this.val_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.val_1.Enabled = false;
            this.val_1.Location = new System.Drawing.Point(50, 0);
            this.val_1.MaxLength = 3;
            this.val_1.Name = "val_1";
            this.val_1.Size = new System.Drawing.Size(30, 20);
            this.val_1.TabIndex = 1;
            this.val_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // val_X
            // 
            this.val_X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.val_X.Enabled = false;
            this.val_X.Location = new System.Drawing.Point(81, 0);
            this.val_X.MaxLength = 3;
            this.val_X.Name = "val_X";
            this.val_X.Size = new System.Drawing.Size(30, 20);
            this.val_X.TabIndex = 2;
            this.val_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ModificadorOptions
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.val_2);
            this.Controls.Add(this.val_X);
            this.Controls.Add(this.val_1);
            this.Controls.Add(this.chckActive);
            this.Name = "ModificadorOptions";
            this.Size = new System.Drawing.Size(148, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		void ChckActiveCheckedChanged(object sender, EventArgs e)
		{
			if(chckActive.Checked)
			{
				val_1.Enabled = true;
				val_X.Enabled = true;
				val_2.Enabled = true;
			}
			else
			{
				val_1.Enabled = false;
				val_X.Enabled = false;
				val_2.Enabled = false;
			}
		}

		
	}
}
