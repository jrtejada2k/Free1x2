using System;
using System.Windows.Forms;

using Free1X2.EntradaSalida;

namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for CambioPuntosFrm.
    /// </summary>
    public class CambioPuntosFrm : System.Windows.Forms.Form
    {
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtValorFijos;
        private TextBox txtValorDobles;
        private TextBox txtValorTriples;
        private Button btnCancel;
        private Button btnOK;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public CambioPuntosFrm()
        {
            InitializeComponent();
            LeerPuntos();
        }

        protected void LeerPuntos()
        {
            int valorFijos = 0;
            int valorDobles = 0;
            int valorTriples = 0;

            AConfiguracion aConfig = new AConfiguracion( Application.StartupPath );
            aConfig.ObtenPuntosCP(ref valorFijos, ref valorDobles, ref valorTriples);

            txtValorFijos.Text = valorFijos.ToString();
            txtValorDobles.Text = valorDobles.ToString();
            txtValorTriples.Text = valorTriples.ToString();
        }


        protected void GuardarPuntos()
        {		
            int valorFijos = Convert.ToInt32(txtValorFijos.Text);
            int valorDobles = Convert.ToInt32(txtValorDobles.Text);
            int valorTriples = Convert.ToInt32(txtValorTriples.Text);

            AConfiguracion aConfig = new AConfiguracion( Application.StartupPath );
            aConfig.GuardarPuntosCP(valorFijos, valorDobles, valorTriples);

            Close();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CambioPuntosFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtValorFijos = new System.Windows.Forms.TextBox();
            this.txtValorDobles = new System.Windows.Forms.TextBox();
            this.txtValorTriples = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Valor Fijos:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Valor Dobles:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(27, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Valor Triples:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtValorFijos
            // 
            this.txtValorFijos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValorFijos.Location = new System.Drawing.Point(148, 24);
            this.txtValorFijos.Name = "txtValorFijos";
            this.txtValorFijos.Size = new System.Drawing.Size(48, 21);
            this.txtValorFijos.TabIndex = 3;
            // 
            // txtValorDobles
            // 
            this.txtValorDobles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValorDobles.Location = new System.Drawing.Point(148, 48);
            this.txtValorDobles.Name = "txtValorDobles";
            this.txtValorDobles.Size = new System.Drawing.Size(48, 21);
            this.txtValorDobles.TabIndex = 4;
            // 
            // txtValorTriples
            // 
            this.txtValorTriples.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValorTriples.Location = new System.Drawing.Point(148, 72);
            this.txtValorTriples.Name = "txtValorTriples";
            this.txtValorTriples.Size = new System.Drawing.Size(48, 21);
            this.txtValorTriples.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(121, 112);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 24);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.LightSalmon;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(27, 112);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 24);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // CambioPuntosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(236, 149);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtValorTriples);
            this.Controls.Add(this.txtValorDobles);
            this.Controls.Add(this.txtValorFijos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CambioPuntosFrm";
            this.Text = "Cambiar Puntuación";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GuardarPuntos();
        }

    }
}
