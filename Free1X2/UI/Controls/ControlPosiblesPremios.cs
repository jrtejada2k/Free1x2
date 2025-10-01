using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Summary description for ControlPosiblesPremios.
	/// </summary>
	public class ControlPosiblesPremios : UserControl
	{
		private Label lblP14C1;
		private Label lblP13C1;
		private Label lblP12C1;
		private Label lblP11C1;
		private Label lblP10C1;
		private Label lblP9C1;
		private Label lblP8C1;
		private Label lblP7C1;
		private Label lblP6C1;
		private Label lblP5C1;
		private Label lblP4C1;
		private Label lblP3C1;
		private Label lblP2C1;
		private Label lblP1C1;
		private Label lblP1;
        private Label lblP15C1;
        private Label lblP16C1;
		//protected string columnaGanadora;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ControlPosiblesPremios(string columna, string columnaGanadora)
		{
			InitializeComponent();
            string aciertos = columna.Substring(columna.Length - 2, 2);

            Label[] labels = { lblP1C1, lblP2C1, lblP3C1, lblP4C1, lblP5C1, lblP6C1, lblP7C1, lblP8C1, lblP9C1, lblP10C1, lblP11C1, lblP12C1, lblP13C1, lblP14C1, lblP15C1, lblP16C1 };
            for (int i = 0; i < columnaGanadora.Length; i++)
            {
                labels[i].Text = columna[i].ToString();
                labels[i].Visible = true;
            }
			lblP1.Text = aciertos;


			for(int i=0; i < columnaGanadora.Length; i++)
			{
                
				if(labels[i].Text != columnaGanadora[i].ToString())
				{
					labels[i].BackColor = System.Drawing.Color.Red;
				}
			}
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
            this.lblP14C1 = new System.Windows.Forms.Label();
            this.lblP13C1 = new System.Windows.Forms.Label();
            this.lblP12C1 = new System.Windows.Forms.Label();
            this.lblP11C1 = new System.Windows.Forms.Label();
            this.lblP10C1 = new System.Windows.Forms.Label();
            this.lblP9C1 = new System.Windows.Forms.Label();
            this.lblP8C1 = new System.Windows.Forms.Label();
            this.lblP7C1 = new System.Windows.Forms.Label();
            this.lblP6C1 = new System.Windows.Forms.Label();
            this.lblP5C1 = new System.Windows.Forms.Label();
            this.lblP4C1 = new System.Windows.Forms.Label();
            this.lblP3C1 = new System.Windows.Forms.Label();
            this.lblP2C1 = new System.Windows.Forms.Label();
            this.lblP1C1 = new System.Windows.Forms.Label();
            this.lblP1 = new System.Windows.Forms.Label();
            this.lblP15C1 = new System.Windows.Forms.Label();
            this.lblP16C1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblP14C1
            // 
            this.lblP14C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C1.Location = new System.Drawing.Point(0, 368);
            this.lblP14C1.Name = "lblP14C1";
            this.lblP14C1.Size = new System.Drawing.Size(20, 20);
            this.lblP14C1.TabIndex = 27;
            this.lblP14C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP14C1.Visible = false;
            // 
            // lblP13C1
            // 
            this.lblP13C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C1.Location = new System.Drawing.Point(0, 344);
            this.lblP13C1.Name = "lblP13C1";
            this.lblP13C1.Size = new System.Drawing.Size(20, 20);
            this.lblP13C1.TabIndex = 26;
            this.lblP13C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP13C1.Visible = false;
            // 
            // lblP12C1
            // 
            this.lblP12C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C1.Location = new System.Drawing.Point(0, 320);
            this.lblP12C1.Name = "lblP12C1";
            this.lblP12C1.Size = new System.Drawing.Size(20, 20);
            this.lblP12C1.TabIndex = 25;
            this.lblP12C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP12C1.Visible = false;
            // 
            // lblP11C1
            // 
            this.lblP11C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C1.Location = new System.Drawing.Point(0, 288);
            this.lblP11C1.Name = "lblP11C1";
            this.lblP11C1.Size = new System.Drawing.Size(20, 20);
            this.lblP11C1.TabIndex = 24;
            this.lblP11C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP11C1.Visible = false;
            // 
            // lblP10C1
            // 
            this.lblP10C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C1.Location = new System.Drawing.Point(0, 264);
            this.lblP10C1.Name = "lblP10C1";
            this.lblP10C1.Size = new System.Drawing.Size(20, 20);
            this.lblP10C1.TabIndex = 23;
            this.lblP10C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP10C1.Visible = false;
            // 
            // lblP9C1
            // 
            this.lblP9C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C1.Location = new System.Drawing.Point(0, 240);
            this.lblP9C1.Name = "lblP9C1";
            this.lblP9C1.Size = new System.Drawing.Size(20, 20);
            this.lblP9C1.TabIndex = 22;
            this.lblP9C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP9C1.Visible = false;
            // 
            // lblP8C1
            // 
            this.lblP8C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C1.Location = new System.Drawing.Point(0, 208);
            this.lblP8C1.Name = "lblP8C1";
            this.lblP8C1.Size = new System.Drawing.Size(20, 20);
            this.lblP8C1.TabIndex = 21;
            this.lblP8C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP8C1.Visible = false;
            // 
            // lblP7C1
            // 
            this.lblP7C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C1.Location = new System.Drawing.Point(0, 184);
            this.lblP7C1.Name = "lblP7C1";
            this.lblP7C1.Size = new System.Drawing.Size(20, 20);
            this.lblP7C1.TabIndex = 20;
            this.lblP7C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP7C1.Visible = false;
            // 
            // lblP6C1
            // 
            this.lblP6C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C1.Location = new System.Drawing.Point(0, 160);
            this.lblP6C1.Name = "lblP6C1";
            this.lblP6C1.Size = new System.Drawing.Size(20, 20);
            this.lblP6C1.TabIndex = 19;
            this.lblP6C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP6C1.Visible = false;
            // 
            // lblP5C1
            // 
            this.lblP5C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C1.Location = new System.Drawing.Point(0, 136);
            this.lblP5C1.Name = "lblP5C1";
            this.lblP5C1.Size = new System.Drawing.Size(20, 20);
            this.lblP5C1.TabIndex = 18;
            this.lblP5C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP5C1.Visible = false;
            // 
            // lblP4C1
            // 
            this.lblP4C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C1.Location = new System.Drawing.Point(0, 104);
            this.lblP4C1.Name = "lblP4C1";
            this.lblP4C1.Size = new System.Drawing.Size(20, 20);
            this.lblP4C1.TabIndex = 17;
            this.lblP4C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP4C1.Visible = false;
            // 
            // lblP3C1
            // 
            this.lblP3C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C1.Location = new System.Drawing.Point(0, 80);
            this.lblP3C1.Name = "lblP3C1";
            this.lblP3C1.Size = new System.Drawing.Size(20, 20);
            this.lblP3C1.TabIndex = 16;
            this.lblP3C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP3C1.Visible = false;
            // 
            // lblP2C1
            // 
            this.lblP2C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C1.Location = new System.Drawing.Point(0, 56);
            this.lblP2C1.Name = "lblP2C1";
            this.lblP2C1.Size = new System.Drawing.Size(20, 20);
            this.lblP2C1.TabIndex = 15;
            this.lblP2C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP2C1.Visible = false;
            // 
            // lblP1C1
            // 
            this.lblP1C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C1.Location = new System.Drawing.Point(0, 32);
            this.lblP1C1.Name = "lblP1C1";
            this.lblP1C1.Size = new System.Drawing.Size(20, 20);
            this.lblP1C1.TabIndex = 14;
            this.lblP1C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP1C1.Visible = false;
            // 
            // lblP1
            // 
            this.lblP1.BackColor = System.Drawing.Color.White;
            this.lblP1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1.Location = new System.Drawing.Point(0, 0);
            this.lblP1.Name = "lblP1";
            this.lblP1.Size = new System.Drawing.Size(20, 20);
            this.lblP1.TabIndex = 113;
            this.lblP1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C1
            // 
            this.lblP15C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C1.Location = new System.Drawing.Point(0, 400);
            this.lblP15C1.Name = "lblP15C1";
            this.lblP15C1.Size = new System.Drawing.Size(20, 20);
            this.lblP15C1.TabIndex = 114;
            this.lblP15C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP15C1.Visible = false;
            // 
            // lblP16C1
            // 
            this.lblP16C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C1.Location = new System.Drawing.Point(0, 424);
            this.lblP16C1.Name = "lblP16C1";
            this.lblP16C1.Size = new System.Drawing.Size(20, 20);
            this.lblP16C1.TabIndex = 115;
            this.lblP16C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblP16C1.Visible = false;
            // 
            // ControlPosiblesPremios
            // 
            this.Controls.Add(this.lblP15C1);
            this.Controls.Add(this.lblP16C1);
            this.Controls.Add(this.lblP1C1);
            this.Controls.Add(this.lblP2C1);
            this.Controls.Add(this.lblP3C1);
            this.Controls.Add(this.lblP4C1);
            this.Controls.Add(this.lblP5C1);
            this.Controls.Add(this.lblP6C1);
            this.Controls.Add(this.lblP7C1);
            this.Controls.Add(this.lblP8C1);
            this.Controls.Add(this.lblP9C1);
            this.Controls.Add(this.lblP10C1);
            this.Controls.Add(this.lblP11C1);
            this.Controls.Add(this.lblP12C1);
            this.Controls.Add(this.lblP13C1);
            this.Controls.Add(this.lblP14C1);
            this.Controls.Add(this.lblP1);
            this.Name = "ControlPosiblesPremios";
            this.Size = new System.Drawing.Size(20, 445);
            this.ResumeLayout(false);

		}
		#endregion

	}
}
