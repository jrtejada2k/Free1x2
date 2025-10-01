namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlAnalisisDistancias
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblNo = new System.Windows.Forms.Label();
            this.lblEquis = new System.Windows.Forms.Label();
            this.lblUnos = new System.Windows.Forms.Label();
            this.lblVariantes = new System.Windows.Forms.Label();
            this.lblDoses = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNo
            // 
            this.lblNo.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNo.Location = new System.Drawing.Point(0, 0);
            this.lblNo.Name = "lblNo";
            this.lblNo.Size = new System.Drawing.Size(44, 18);
            this.lblNo.TabIndex = 7;
            this.lblNo.Text = "Nº";
            this.lblNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEquis
            // 
            this.lblEquis.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblEquis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEquis.Location = new System.Drawing.Point(0, 57);
            this.lblEquis.Name = "lblEquis";
            this.lblEquis.Size = new System.Drawing.Size(44, 18);
            this.lblEquis.TabIndex = 6;
            this.lblEquis.Text = "X";
            this.lblEquis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUnos
            // 
            this.lblUnos.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblUnos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblUnos.Location = new System.Drawing.Point(0, 38);
            this.lblUnos.Name = "lblUnos";
            this.lblUnos.Size = new System.Drawing.Size(44, 18);
            this.lblUnos.TabIndex = 5;
            this.lblUnos.Text = "1";
            this.lblUnos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVariantes
            // 
            this.lblVariantes.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblVariantes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVariantes.Location = new System.Drawing.Point(0, 19);
            this.lblVariantes.Name = "lblVariantes";
            this.lblVariantes.Size = new System.Drawing.Size(44, 18);
            this.lblVariantes.TabIndex = 4;
            this.lblVariantes.Text = "V";
            this.lblVariantes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDoses
            // 
            this.lblDoses.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblDoses.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDoses.Location = new System.Drawing.Point(0, 76);
            this.lblDoses.Name = "lblDoses";
            this.lblDoses.Size = new System.Drawing.Size(44, 18);
            this.lblDoses.TabIndex = 8;
            this.lblDoses.Text = "2";
            this.lblDoses.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CtrlAnalisisDistancias
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.lblDoses);
            this.Controls.Add(this.lblNo);
            this.Controls.Add(this.lblEquis);
            this.Controls.Add(this.lblUnos);
            this.Controls.Add(this.lblVariantes);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CtrlAnalisisDistancias";
            this.Size = new System.Drawing.Size(790, 113);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNo;
        private System.Windows.Forms.Label lblEquis;
        private System.Windows.Forms.Label lblUnos;
        private System.Windows.Forms.Label lblVariantes;
        private System.Windows.Forms.Label lblDoses;
    }
}
