namespace Free1X2.UI
{
    partial class ImportadorCPsFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportadorCPsFrm));
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnImportarSimples = new System.Windows.Forms.Button();
            this.btnImportarClm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(224, 67);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(90, 24);
            this.btnCancelar.TabIndex = 28;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnImportarSimples
            // 
            this.btnImportarSimples.BackColor = System.Drawing.Color.LightSalmon;
            this.btnImportarSimples.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnImportarSimples.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportarSimples.Image = ((System.Drawing.Image)(resources.GetObject("btnImportarSimples.Image")));
            this.btnImportarSimples.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportarSimples.Location = new System.Drawing.Point(12, 12);
            this.btnImportarSimples.Name = "btnImportarSimples";
            this.btnImportarSimples.Size = new System.Drawing.Size(302, 24);
            this.btnImportarSimples.TabIndex = 29;
            this.btnImportarSimples.Text = "Importar CPs Simples (sólo columnas *.txt)";
            this.btnImportarSimples.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportarSimples.UseVisualStyleBackColor = false;
            this.btnImportarSimples.Click += new System.EventHandler(this.btnImportarSimples_Click);
            // 
            // btnImportarClm
            // 
            this.btnImportarClm.BackColor = System.Drawing.Color.LightSalmon;
            this.btnImportarClm.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnImportarClm.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportarClm.Image = ((System.Drawing.Image)(resources.GetObject("btnImportarClm.Image")));
            this.btnImportarClm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportarClm.Location = new System.Drawing.Point(12, 37);
            this.btnImportarClm.Name = "btnImportarClm";
            this.btnImportarClm.Size = new System.Drawing.Size(302, 24);
            this.btnImportarClm.TabIndex = 30;
            this.btnImportarClm.Text = "Importar CPs con Ac, Acs, Fs (*.clm)";
            this.btnImportarClm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportarClm.UseVisualStyleBackColor = false;
            this.btnImportarClm.Click += new System.EventHandler(this.btnImportarClm_Click);
            // 
            // ImportadorCPsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(326, 100);
            this.ControlBox = false;
            this.Controls.Add(this.btnImportarClm);
            this.Controls.Add(this.btnImportarSimples);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportadorCPsFrm";
            this.Text = "Importador de Columnas Probables";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnImportarSimples;
        private System.Windows.Forms.Button btnImportarClm;
    }
}