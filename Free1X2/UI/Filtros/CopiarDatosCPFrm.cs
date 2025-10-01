using System;
using System.Windows.Forms;

namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Descripción breve de CopiarDatosCPFrm.
    /// </summary>
    public class CopiarDatosCPFrm : Form
    {
        private Button btnCancelar;
        private Button btnCopiar;
        private Label label1;
        private NumericUpDown udMin;
        private Label label2;
        private Label label3;
        private NumericUpDown udMax;
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.Container components;

        public CopiarDatosCPFrm(long desde, long max)
        {
            InitializeComponent();
            udMin.Minimum=1;
            udMin.Maximum=max;
            udMin.Value=desde;
            udMax.Minimum=desde;
            udMax.Maximum=max;
            udMax.Value=max;
        }

        public int Desde
        {
            get{return Convert.ToInt16(udMin.Value);}
            set{udMin.Value=value;}
        }
        public int Hasta
        {
            get{return Convert.ToInt16(udMax.Value);}
            set{udMax.Value=value;}
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

        #region Código generado por el Diseñador de Windows Forms
        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopiarDatosCPFrm));
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnCopiar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.udMin = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.udMax = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.udMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMax)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(144, 80);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(80, 24);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnCopiar
            // 
            this.btnCopiar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCopiar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopiar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiar.Image = ((System.Drawing.Image)(resources.GetObject("btnCopiar.Image")));
            this.btnCopiar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopiar.Location = new System.Drawing.Point(48, 80);
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.Size = new System.Drawing.Size(80, 24);
            this.btnCopiar.TabIndex = 6;
            this.btnCopiar.Text = "Copiar";
            this.btnCopiar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCopiar.UseVisualStyleBackColor = false;
            this.btnCopiar.Click += new System.EventHandler(this.btnCopiar_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Columnas:";
            // 
            // udMin
            // 
            this.udMin.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udMin.Location = new System.Drawing.Point(64, 40);
            this.udMin.Name = "udMin";
            this.udMin.Size = new System.Drawing.Size(72, 21);
            this.udMin.TabIndex = 8;
            this.udMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udMin.ValueChanged += new System.EventHandler(this.udMin_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "desde";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(140, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "hasta";
            // 
            // udMax
            // 
            this.udMax.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udMax.Location = new System.Drawing.Point(184, 40);
            this.udMax.Name = "udMax";
            this.udMax.Size = new System.Drawing.Size(72, 21);
            this.udMax.TabIndex = 11;
            this.udMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udMax.ValueChanged += new System.EventHandler(this.udMax_ValueChanged);
            // 
            // CopiarDatosCPFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(272, 118);
            this.ControlBox = false;
            this.Controls.Add(this.udMax);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.udMin);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCopiar);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopiarDatosCPFrm";
            this.Text = "Copiar Datos";
            ((System.ComponentModel.ISupportInitialize)(this.udMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            udMin.Minimum=-1;
            Desde=-1;
            Close();
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void udMin_ValueChanged(object sender, EventArgs e)
        {
            udMax.Minimum=udMin.Value;
        }

        private void udMax_ValueChanged(object sender, EventArgs e)
        {
            udMin.Maximum=udMax.Value;
        }

    }
}
