using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Summary description for SignoBoletoBase.
	/// </summary>
	public class SignoBoletoBase : UserControl
	{
		private Label lblSigno;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components;

		private Color colorFondo = Color.Wheat;
		private bool isEnabled = true;
		private bool isChecked;

		public SignoBoletoBase()
		{
			InitializeComponent();
		}

		public bool IsEnabled
		{
			get{ return isEnabled;}
			set{ isEnabled = value;}
		}

		public bool IsChecked
		{
			get{ return isChecked;}
			set
			{ 
				isChecked = value;

				if(isChecked)
				{
					lblSigno.BackColor = Color.LimeGreen;
				}
				else
				{
					lblSigno.BackColor = colorFondo;
				}
			
			}
		}

		public Color ColorFondo
		{
			get{ return colorFondo;}
			set{ colorFondo = value;}		
		}

		public string Pronostico
		{
			get{ return lblSigno.Text; }
			set{ lblSigno.Text = value; }
		}

		void LblSignoClick(object sender, System.EventArgs e)
		{
			if(isEnabled)
			{
				if( lblSigno.BackColor == colorFondo )
				{
					lblSigno.BackColor = Color.LimeGreen;
					isChecked = true;
				}
				else
				{
					lblSigno.BackColor = colorFondo;
					isChecked = false;
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
			this.lblSigno = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblSigno
			// 
			this.lblSigno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblSigno.Location = new System.Drawing.Point(0, 0);
			this.lblSigno.Name = "lblSigno";
			this.lblSigno.Size = new System.Drawing.Size(18, 18);
			this.lblSigno.TabIndex = 0;
			this.lblSigno.Text = "X";
			this.lblSigno.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblSigno.Click += new System.EventHandler(this.LblSignoClick);
			// 
			// SignoBoletoBase
			// 
			this.Controls.Add(this.lblSigno);
			this.Name = "SignoBoletoBase";
			this.Size = new System.Drawing.Size(18, 18);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
