namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlAnalisisControlGrupos
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
            this.lblColumnas = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNo
            // 
            this.lblNo.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNo.Location = new System.Drawing.Point(0, 0);
            this.lblNo.Name = "lblNo";
            this.lblNo.Size = new System.Drawing.Size(67, 18);
            this.lblNo.TabIndex = 9;
            this.lblNo.Text = "Fallos";
            this.lblNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblColumnas
            // 
            this.lblColumnas.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblColumnas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColumnas.Location = new System.Drawing.Point(0, 20);
            this.lblColumnas.Name = "lblColumnas";
            this.lblColumnas.Size = new System.Drawing.Size(67, 18);
            this.lblColumnas.TabIndex = 8;
            this.lblColumnas.Text = "Columnas";
            this.lblColumnas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CtrlAnalisisControlGrupos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.lblNo);
            this.Controls.Add(this.lblColumnas);
            this.Name = "CtrlAnalisisControlGrupos";
            this.Size = new System.Drawing.Size(790, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNo;
        private System.Windows.Forms.Label lblColumnas;
    }
}
