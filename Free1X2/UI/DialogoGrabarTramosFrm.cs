using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for DialogoGrabarTramosFrm.
	/// </summary>
	public class DialogoGrabarTramosFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btGrabar;
		private System.Windows.Forms.TextBox txColumnaInicial;
		private System.Windows.Forms.TextBox txColumnaFinal;
		private System.Windows.Forms.TextBox txPaso;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public int ColumnaInicial;
		public int ColumnaFinal;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txNunCols;
		public int Paso;
		public int NumColsPorPaso;

		public DialogoGrabarTramosFrm(int columnainicial, int columnafinal)
		{
			InitializeComponent();
			txColumnaInicial.Text =columnainicial.ToString ();
			txColumnaFinal.Text =columnafinal.ToString ();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogoGrabarTramosFrm));
            this.txColumnaInicial = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txColumnaFinal = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txPaso = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btGrabar = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txNunCols = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txColumnaInicial
            // 
            this.txColumnaInicial.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txColumnaInicial.Location = new System.Drawing.Point(133, 9);
            this.txColumnaInicial.Name = "txColumnaInicial";
            this.txColumnaInicial.Size = new System.Drawing.Size(129, 21);
            this.txColumnaInicial.TabIndex = 0;
            this.txColumnaInicial.Text = "0";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.LemonChiffon;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Columna inicial";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.LemonChiffon;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "Columna final";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txColumnaFinal
            // 
            this.txColumnaFinal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txColumnaFinal.Location = new System.Drawing.Point(133, 31);
            this.txColumnaFinal.Name = "txColumnaFinal";
            this.txColumnaFinal.Size = new System.Drawing.Size(129, 21);
            this.txColumnaFinal.TabIndex = 2;
            this.txColumnaFinal.Text = "0";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.LemonChiffon;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "Grabar";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPaso
            // 
            this.txPaso.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPaso.Location = new System.Drawing.Point(163, 66);
            this.txPaso.Name = "txPaso";
            this.txPaso.Size = new System.Drawing.Size(34, 21);
            this.txPaso.TabIndex = 4;
            this.txPaso.Text = "1";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.LemonChiffon;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(198, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 21);
            this.label4.TabIndex = 6;
            this.label4.Text = "columnas";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btGrabar
            // 
            this.btGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGrabar.Image = ((System.Drawing.Image)(resources.GetObject("btGrabar.Image")));
            this.btGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btGrabar.Location = new System.Drawing.Point(86, 102);
            this.btGrabar.Name = "btGrabar";
            this.btGrabar.Size = new System.Drawing.Size(100, 32);
            this.btGrabar.TabIndex = 7;
            this.btGrabar.Text = "Aceptar";
            this.btGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGrabar.UseVisualStyleBackColor = false;
            this.btGrabar.Click += new System.EventHandler(this.btGrabar_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.LemonChiffon;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(103, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 21);
            this.label5.TabIndex = 8;
            this.label5.Text = "de cada";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txNunCols
            // 
            this.txNunCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNunCols.Location = new System.Drawing.Point(68, 66);
            this.txNunCols.Name = "txNunCols";
            this.txNunCols.Size = new System.Drawing.Size(34, 21);
            this.txNunCols.TabIndex = 9;
            this.txNunCols.Text = "1";
            // 
            // DialogoGrabarTramosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(272, 144);
            this.Controls.Add(this.txNunCols);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btGrabar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txPaso);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txColumnaFinal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txColumnaInicial);
            this.Name = "DialogoGrabarTramosFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Grabar Tramos";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btGrabar_Click(object sender, System.EventArgs e)
		{
			ColumnaInicial=Convert.ToInt32 (txColumnaInicial.Text);
			ColumnaFinal=Convert.ToInt32 (txColumnaFinal.Text);
			NumColsPorPaso =Convert.ToInt32 (this.txNunCols.Text);
			Paso=Convert.ToInt32 (txPaso.Text);
			Close ();
		}
	}
}

