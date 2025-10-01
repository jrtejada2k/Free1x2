namespace Free1X2.UI
{
    partial class ExportadorCPsFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportadorCPsFrm));
            this.btnExportarClm = new System.Windows.Forms.Button();
            this.btnExportarSimples = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExportarClm
            // 
            this.btnExportarClm.BackColor = System.Drawing.Color.LightSalmon;
            this.btnExportarClm.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExportarClm.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarClm.Image = ((System.Drawing.Image)(resources.GetObject("btnExportarClm.Image")));
            this.btnExportarClm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportarClm.Location = new System.Drawing.Point(9, 37);
            this.btnExportarClm.Name = "btnExportarClm";
            this.btnExportarClm.Size = new System.Drawing.Size(302, 24);
            this.btnExportarClm.TabIndex = 33;
            this.btnExportarClm.Text = "Exportar CPs con Ac, Acs, Fs (*.clm)";
            this.btnExportarClm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportarClm.UseVisualStyleBackColor = false;
            this.btnExportarClm.Click += new System.EventHandler(this.btnExportarClm_Click);
            // 
            // btnExportarSimples
            // 
            this.btnExportarSimples.BackColor = System.Drawing.Color.LightSalmon;
            this.btnExportarSimples.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExportarSimples.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarSimples.Image = ((System.Drawing.Image)(resources.GetObject("btnExportarSimples.Image")));
            this.btnExportarSimples.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportarSimples.Location = new System.Drawing.Point(9, 12);
            this.btnExportarSimples.Name = "btnExportarSimples";
            this.btnExportarSimples.Size = new System.Drawing.Size(302, 24);
            this.btnExportarSimples.TabIndex = 32;
            this.btnExportarSimples.Text = "Exportar CPs Simples (sólo columnas *.txt)";
            this.btnExportarSimples.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportarSimples.UseVisualStyleBackColor = false;
            this.btnExportarSimples.Click += new System.EventHandler(this.btnExportarSimples_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(221, 67);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(90, 24);
            this.btnCancelar.TabIndex = 31;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // ExportadorCPsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(326, 101);
            this.Controls.Add(this.btnExportarClm);
            this.Controls.Add(this.btnExportarSimples);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportadorCPsFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Exportador de Columnas Probables";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExportarClm;
        private System.Windows.Forms.Button btnExportarSimples;
        private System.Windows.Forms.Button btnCancelar;
    }
}