using System;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Descripción breve de CtrPorcentaje.
	/// </summary>
	public class CtrPorcentaje : UserControl
	{
		private int valor;
		private int columnas;
		private double porcentaje;
		private bool seleccionado;
		private vistaAnalisis vista=vistaAnalisis.Columnas;
		private Panel panel1;
		private Label lblNum;
		private Label lblPct;
		private ToolTip toolTip1;
		// Eventos
		public event EventHandler Pulsado;
		protected void OnPulsado(EventArgs e)
		{
			if (Pulsado != null)
				Pulsado(this, e);
		}

		private System.ComponentModel.IContainer components;

		public CtrPorcentaje()
		{
			InitializeComponent();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }


		/// <summary> 
		/// Limpiar los recursos que se estén utilizando.
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

		#region Código generado por el Diseñador de componentes
		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblNum = new System.Windows.Forms.Label();
			this.lblPct = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.lblNum);
			this.panel1.Controls.Add(this.lblPct);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(40, 26);
			this.panel1.TabIndex = 50;
			// 
			// lblNum
			// 
			this.lblNum.BackColor = System.Drawing.Color.PapayaWhip;
			this.lblNum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.lblNum.ForeColor = System.Drawing.Color.DimGray;
			this.lblNum.Location = new System.Drawing.Point(-1, -1);
			this.lblNum.Name = "lblNum";
			this.lblNum.Size = new System.Drawing.Size(40, 12);
			this.lblNum.TabIndex = 51;
			this.lblNum.Text = "0";
			this.lblNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblNum.Click += new System.EventHandler(this.CtrPorcentaje_Click);
			// 
			// lblPct
			// 
			this.lblPct.BackColor = System.Drawing.Color.PapayaWhip;
			this.lblPct.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F);
			this.lblPct.Location = new System.Drawing.Point(0, 10);
			this.lblPct.Name = "lblPct";
			this.lblPct.Size = new System.Drawing.Size(40, 14);
			this.lblPct.TabIndex = 50;
			this.lblPct.Text = "0";
			this.lblPct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblPct.Click += new System.EventHandler(this.CtrPorcentaje_Click);
			// 
			// CtrPorcentaje
			// 
			this.Controls.Add(this.panel1);
			this.Name = "CtrPorcentaje";
			this.Size = new System.Drawing.Size(40, 26);
			this.Click += new System.EventHandler(this.CtrPorcentaje_Click);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void CtrPorcentaje_Click(object sender, EventArgs e)
		{
			Seleccionado=!seleccionado;
			OnPulsado(EventArgs.Empty);
		}

		public int Valor
		{
			get { return valor; }
			set
			{
				valor = value;
				lblNum.Text=valor.ToString();
			}
		}
		
		public int Columnas
		{
			get { return columnas; }
			set
			{
				columnas = value;
				mostrar();
			}
		}
		
		public double Porcentaje
		{
			get { return porcentaje; }
			set
			{
				porcentaje = value;
				mostrar();
			}
		}
		
		public vistaAnalisis Vista
		{
			get { return vista; }
			set
			{
				vista = value;
				mostrar();
			}
		}
		
		public bool Seleccionado
		{
			get { return seleccionado; }
			set
			{
				seleccionado = value;
				Color color;
				if(seleccionado)
					color=Color.LightGreen;
				else
					color=Color.PapayaWhip;
				lblNum.BackColor=color;
				lblPct.BackColor=color;
			}
		}
		
		private void mostrar()
		{
			if(vista==vistaAnalisis.Porcentaje)
			{
				lblPct.Text=porcentaje.ToString("0.00")+"%";
				toolTip1.SetToolTip(lblPct, columnas+ " columnas");
				toolTip1.SetToolTip(lblNum, porcentaje.ToString("0.00")+"%");
			}
			else
			{
				lblPct.Text=columnas.ToString();
				toolTip1.SetToolTip(lblPct, porcentaje.ToString("0.00")+"%");
				toolTip1.SetToolTip(lblNum, porcentaje.ToString("0.00")+"%");
			}
		}

	}
}
