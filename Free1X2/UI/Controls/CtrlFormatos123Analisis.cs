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

using Free1X2.MotorCalculo;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Summary description for CtrlFormatos123Analisis.
	/// </summary>
	public class CtrlFormatos123Analisis : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox txtFormato;
		protected Formato123 formato;
		protected int apariciones;
		private System.Windows.Forms.TextBox txtApariciones;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CtrlFormatos123Analisis()
		{
			InitializeComponent();

		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

		public Formato123 Formato
		{
			get {return formato;}
			set {formato = value;}
		}
		public int Apariciones
		{
			get {return apariciones;}
			set {apariciones = value;}
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

		protected void MostrarFormato()
		{
			txtFormato.Text = Formato.Formato;
			txtApariciones.Text = Apariciones.ToString();
		}
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.txtFormato = new System.Windows.Forms.TextBox();
            this.txtApariciones = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtFormato
            // 
            this.txtFormato.BackColor = System.Drawing.Color.White;
            this.txtFormato.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFormato.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFormato.Location = new System.Drawing.Point(1, 1);
            this.txtFormato.MaxLength = 14;
            this.txtFormato.Name = "txtFormato";
            this.txtFormato.ReadOnly = true;
            this.txtFormato.Size = new System.Drawing.Size(100, 13);
            this.txtFormato.TabIndex = 3;
            // 
            // txtApariciones
            // 
            this.txtApariciones.BackColor = System.Drawing.Color.White;
            this.txtApariciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtApariciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtApariciones.Location = new System.Drawing.Point(107, 1);
            this.txtApariciones.MaxLength = 2;
            this.txtApariciones.Name = "txtApariciones";
            this.txtApariciones.ReadOnly = true;
            this.txtApariciones.Size = new System.Drawing.Size(24, 13);
            this.txtApariciones.TabIndex = 4;
            this.txtApariciones.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CtrlFormatos123Analisis
            // 
            this.Controls.Add(this.txtApariciones);
            this.Controls.Add(this.txtFormato);
            this.Name = "CtrlFormatos123Analisis";
            this.Size = new System.Drawing.Size(138, 16);
            this.Load += new System.EventHandler(this.CtrlFormatos123Analisis_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void CtrlFormatos123Analisis_Load(object sender, System.EventArgs e)
		{
			if(Formato != null)
			{
				MostrarFormato();
			}
		}


	}
}
