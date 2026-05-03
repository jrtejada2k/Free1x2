using System;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	public class GeneradorOptions : UserControl
	{
		private TextBox val_X;
		private TextBox val_1;
		private TextBox val_2;
		private Label numPartido;
		public GeneradorOptions()
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
			get{ return Convert.ToInt32(numPartido.Text); }
			set{ numPartido.Text = (value).ToString();}
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

		void InitializeComponent() {
            this.val_2 = new System.Windows.Forms.TextBox();
            this.val_1 = new System.Windows.Forms.TextBox();
            this.val_X = new System.Windows.Forms.TextBox();
            this.numPartido = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // val_2
            // 
            this.val_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.val_2.Location = new System.Drawing.Point(116, 0);
            this.val_2.MaxLength = 3;
            this.val_2.Name = "val_2";
            this.val_2.Size = new System.Drawing.Size(30, 20);
            this.val_2.TabIndex = 3;
            this.val_2.Text = "0";
            this.val_2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // val_1
            // 
            this.val_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.val_1.Location = new System.Drawing.Point(44, 0);
            this.val_1.MaxLength = 3;
            this.val_1.Name = "val_1";
            this.val_1.Size = new System.Drawing.Size(30, 20);
            this.val_1.TabIndex = 1;
            this.val_1.Text = "0";
            this.val_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // val_X
            // 
            this.val_X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.val_X.Location = new System.Drawing.Point(80, 0);
            this.val_X.MaxLength = 3;
            this.val_X.Name = "val_X";
            this.val_X.Size = new System.Drawing.Size(30, 20);
            this.val_X.TabIndex = 2;
            this.val_X.Text = "0";
            this.val_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // numPartido
            // 
            this.numPartido.Location = new System.Drawing.Point(8, 4);
            this.numPartido.Name = "numPartido";
            this.numPartido.Size = new System.Drawing.Size(24, 16);
            this.numPartido.TabIndex = 4;
            this.numPartido.Text = "0";
            // 
            // GeneradorOptions
            // 
            this.Controls.Add(this.numPartido);
            this.Controls.Add(this.val_2);
            this.Controls.Add(this.val_X);
            this.Controls.Add(this.val_1);
            this.Name = "GeneradorOptions";
            this.Size = new System.Drawing.Size(160, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
