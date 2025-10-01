namespace Free1X2.UI
{
    partial class VerBoletosEnEditorFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerBoletosEnEditorFrm));
            this.txtBoletos = new System.Windows.Forms.TextBox();
            this.bImpDirec = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtBoletos
            // 
            this.txtBoletos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoletos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoletos.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoletos.Location = new System.Drawing.Point(12, 13);
            this.txtBoletos.Multiline = true;
            this.txtBoletos.Name = "txtBoletos";
            this.txtBoletos.ReadOnly = true;
            this.txtBoletos.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBoletos.Size = new System.Drawing.Size(274, 246);
            this.txtBoletos.TabIndex = 0;
            // 
            // bImpDirec
            // 
            this.bImpDirec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bImpDirec.BackColor = System.Drawing.Color.DarkSalmon;
            this.bImpDirec.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bImpDirec.Font = new System.Drawing.Font("Verdana", 7F);
            this.bImpDirec.Image = ((System.Drawing.Image)(resources.GetObject("bImpDirec.Image")));
            this.bImpDirec.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bImpDirec.Location = new System.Drawing.Point(168, 261);
            this.bImpDirec.Name = "bImpDirec";
            this.bImpDirec.Size = new System.Drawing.Size(118, 30);
            this.bImpDirec.TabIndex = 11;
            this.bImpDirec.Text = "Imprimir";
            this.bImpDirec.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bImpDirec.UseVisualStyleBackColor = false;
            this.bImpDirec.Click += new System.EventHandler(this.bImpDirec_Click);
            // 
            // VerBoletosEnEditorFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(299, 295);
            this.Controls.Add(this.bImpDirec);
            this.Controls.Add(this.txtBoletos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "VerBoletosEnEditorFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ver Boletos En Editor de Texto";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoletos;
        private System.Windows.Forms.Button bImpDirec;
    }
}