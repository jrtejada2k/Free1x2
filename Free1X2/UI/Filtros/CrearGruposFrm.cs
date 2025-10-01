using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Descripción breve de CopiarDatosCPFrm.
    /// </summary>
    public class CrearGruposFrm : Form
    {
        private Button btnCancelar;
        private Label label1;
        private TextBox txtNumGrupos;
        public NumericUpDown udNumGrupos;
        private Button btnCrear;
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private Container components;

        public CrearGruposFrm()
        {
            InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrearGruposFrm));
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnCrear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNumGrupos = new System.Windows.Forms.TextBox();
            this.udNumGrupos = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.udNumGrupos)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(144, 57);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(80, 24);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnCrear
            // 
            this.btnCrear.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCrear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCrear.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrear.Image = ((System.Drawing.Image)(resources.GetObject("btnCrear.Image")));
            this.btnCrear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCrear.Location = new System.Drawing.Point(48, 57);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(80, 24);
            this.btnCrear.TabIndex = 6;
            this.btnCrear.Text = "Crear";
            this.btnCrear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCrear.UseVisualStyleBackColor = false;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "¿Cuántos grupos nuevos?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtNumGrupos
            // 
            this.txtNumGrupos.Location = new System.Drawing.Point(176, 40);
            this.txtNumGrupos.Name = "txtNumGrupos";
            this.txtNumGrupos.Size = new System.Drawing.Size(56, 20);
            this.txtNumGrupos.TabIndex = 8;
            this.txtNumGrupos.Text = "textBox1";
            // 
            // udNumGrupos
            // 
            this.udNumGrupos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udNumGrupos.Location = new System.Drawing.Point(189, 9);
            this.udNumGrupos.Name = "udNumGrupos";
            this.udNumGrupos.Size = new System.Drawing.Size(64, 21);
            this.udNumGrupos.TabIndex = 8;
            this.udNumGrupos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udNumGrupos.ValueChanged += new System.EventHandler(this.udNumGrupos_ValueChanged);
            // 
            // CrearGruposFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(272, 99);
            this.ControlBox = false;
            this.Controls.Add(this.udNumGrupos);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CrearGruposFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crear nuevos grupos";
            ((System.ComponentModel.ISupportInitialize)(this.udNumGrupos)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            udNumGrupos.Value=0;
            Close();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void udNumGrupos_ValueChanged(object sender, EventArgs e)
        {
            if(udNumGrupos.Value>0)
                btnCrear.Enabled=true;
            else
                btnCrear.Enabled=false;
        }

    }
}
