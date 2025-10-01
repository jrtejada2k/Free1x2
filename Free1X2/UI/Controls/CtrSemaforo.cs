using System;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	public class CtrSemaforo : UserControl
	{
		public enum NombreEstado
		{
			Neutro,
			Rojo,
			Amarillo,
			Verde
		}

		public enum Luces
		{
			Dos=2,
			Tres=3
		}

		private Panel panel1;
		private Button btnVerde;
		private Button btnAmarillo;
		private Button btnRojo;
		protected NombreEstado estado;
		protected int numLuces;
		protected alignment alineacion;

		public event EventHandler BotonPulsado;
		protected void OnBotonPulsado(EventArgs e)
		{
			if (BotonPulsado != null)
				BotonPulsado(this, e);
		}
        
		public NombreEstado Estado
		{
			get{ return estado; }
			set
			{
				estado = value;
				CambiaColor();
			}
		}

		public Luces NumLuces
		{
			get{ return (Luces)numLuces; }
			set
			{
				numLuces = (int)value;
				CambiarEstado();
			}
		}

		public alignment Alineacion
		{
			get{ return alineacion; }
			set
			{
				alineacion = value;
				CambiarEstado();
			}
		}
        
		// Variable del diseñador requerida.
		private System.ComponentModel.Container components;

		public CtrSemaforo()
		{
			InitializeComponent();
			if(NumLuces.Equals(null)) NumLuces=Luces.Tres;
			if(Alineacion.Equals(null)) Alineacion=alignment.Vertical;
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

		#region Component Designer generated code
		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnVerde = new System.Windows.Forms.Button();
            this.btnAmarillo = new System.Windows.Forms.Button();
            this.btnRojo = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.btnVerde);
            this.panel1.Controls.Add(this.btnAmarillo);
            this.panel1.Controls.Add(this.btnRojo);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(38, 17);
            this.panel1.TabIndex = 3;
            // 
            // btnVerde
            // 
            this.btnVerde.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(255)))), ((int)(((byte)(220)))));
            this.btnVerde.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.btnVerde.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVerde.Location = new System.Drawing.Point(26, 3);
            this.btnVerde.Name = "btnVerde";
            this.btnVerde.Size = new System.Drawing.Size(10, 10);
            this.btnVerde.TabIndex = 5;
            this.btnVerde.TabStop = false;
            this.btnVerde.UseVisualStyleBackColor = false;
            this.btnVerde.Click += new System.EventHandler(this.btnVerde_Click);
            // 
            // btnAmarillo
            // 
            this.btnAmarillo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(220)))));
            this.btnAmarillo.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.btnAmarillo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAmarillo.Location = new System.Drawing.Point(14, 3);
            this.btnAmarillo.Name = "btnAmarillo";
            this.btnAmarillo.Size = new System.Drawing.Size(10, 10);
            this.btnAmarillo.TabIndex = 4;
            this.btnAmarillo.TabStop = false;
            this.btnAmarillo.UseVisualStyleBackColor = false;
            this.btnAmarillo.Click += new System.EventHandler(this.btnAmarillo_Click);
            // 
            // btnRojo
            // 
            this.btnRojo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnRojo.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.btnRojo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRojo.Location = new System.Drawing.Point(2, 3);
            this.btnRojo.Name = "btnRojo";
            this.btnRojo.Size = new System.Drawing.Size(10, 10);
            this.btnRojo.TabIndex = 3;
            this.btnRojo.TabStop = false;
            this.btnRojo.UseVisualStyleBackColor = false;
            this.btnRojo.Click += new System.EventHandler(this.btnRojo_Click);
            // 
            // CtrSemaforo
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Name = "CtrSemaforo";
            this.Size = new System.Drawing.Size(38, 17);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void btnRojo_Click(object sender, EventArgs e)
		{
			Estado=NombreEstado.Rojo;
			OnBotonPulsado(EventArgs.Empty);
		}

		private void btnAmarillo_Click(object sender, EventArgs e)
		{
			Estado=NombreEstado.Amarillo;
			OnBotonPulsado(EventArgs.Empty);
		}

		private void btnVerde_Click(object sender, EventArgs e)
		{
			Estado=NombreEstado.Verde;
			OnBotonPulsado(EventArgs.Empty);
		}

		private void CambiarEstado()
		{
			switch(Alineacion)
			{
				case alignment.Horizontal:
					Height=17;
					panel1.Height=17;
					switch(NumLuces)
					{
						case Luces.Dos :
							btnRojo.Location=new Point(2,2);
							btnAmarillo.Visible=false;
							btnVerde.Location=new Point(16,2);
							Width =32;
							panel1.Width =32;
							break;
						case Luces.Tres :
							btnRojo.Location=new Point(2,2);
							btnAmarillo.Visible=true;
							btnAmarillo.Location=new Point(16,2);
							btnVerde.Location=new Point(30,2);
							Width =46;
							panel1.Width =46;
							break;
					}
					break;
				case alignment.Vertical:
					Width =18;
					panel1.Width =18;
					switch(NumLuces)
					{
						case Luces.Dos :
							btnRojo.Location=new Point(2,2);
							btnAmarillo.Visible=false;
							btnVerde.Location=new Point(2,16);
							Height=32;
							panel1.Height=32;
							break;
						case Luces.Tres :
							btnRojo.Location=new Point(2,2);
							btnAmarillo.Visible=true;
							btnAmarillo.Location=new Point(2,16);
							btnVerde.Location=new Point(2,30);
							Height=46;
							panel1.Height=46;
							break;
					}
					break;
			}
		}

		// Funciones
		private void CambiaColor()
		{
			switch (Estado)
			{
				case NombreEstado.Neutro:
					btnRojo.BackColor=Color.FromArgb(255,220,220);
					btnRojo.Enabled=true;
                    btnRojo.FlatStyle = FlatStyle.Flat;
					btnAmarillo.BackColor=Color.FromArgb(255,255,220);
					btnAmarillo.Enabled=true;
                    btnAmarillo.FlatStyle = FlatStyle.Flat;
					btnVerde.BackColor=Color.FromArgb(220,255,220);
					btnVerde.Enabled=false;
                    btnVerde.FlatStyle = FlatStyle.Flat;
					break;
				case NombreEstado.Rojo :
					btnRojo.BackColor=Color.Red;
					btnRojo.Enabled=false;
                    btnRojo.FlatStyle = FlatStyle.Flat;
					btnAmarillo.BackColor=Color.FromArgb(255,255,220);
					btnAmarillo.Enabled=true;
                    btnAmarillo.FlatStyle = FlatStyle.Flat;
					btnVerde.BackColor=Color.FromArgb(220,255,220);
					btnVerde.Enabled=true;
                    btnVerde.FlatStyle = FlatStyle.Flat;
					break;
				case NombreEstado.Amarillo :
					if(NumLuces==Luces.Tres )
					{
						btnRojo.BackColor=Color.FromArgb(255,220,220);
						btnRojo.Enabled=true;
                        btnRojo.FlatStyle = FlatStyle.Flat;
						btnAmarillo.BackColor=Color.Yellow;
						btnAmarillo.Enabled=false;
                        btnAmarillo.FlatStyle = FlatStyle.Flat;
						btnVerde.BackColor=Color.FromArgb(220,255,220);
						btnVerde.Enabled=true;
                        btnVerde.FlatStyle = FlatStyle.Flat;
					}
					break;
				case NombreEstado.Verde :
					btnRojo.BackColor=Color.FromArgb(255,220,220);
					btnRojo.Enabled=true;
                    btnRojo.FlatStyle = FlatStyle.Flat;
					btnAmarillo.BackColor=Color.FromArgb(255,255,220);
					btnAmarillo.Enabled=true;
                    btnAmarillo.FlatStyle = FlatStyle.Flat;
					btnVerde.BackColor=Color.Lime;
					btnVerde.Enabled=false;
                    btnVerde.FlatStyle = FlatStyle.Flat;
					break;
			}
		}


	}
}
