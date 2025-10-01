namespace Free1X2.UI.Controls
{
    partial class VisorCumplimiento
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
            this.lblCumplimiento = new System.Windows.Forms.Label();
            this.lblArchivo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCumplimiento
            // 
            this.lblCumplimiento.BackColor = System.Drawing.Color.Snow;
            this.lblCumplimiento.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCumplimiento.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCumplimiento.Location = new System.Drawing.Point(100, 0);
            this.lblCumplimiento.Name = "lblCumplimiento";
            this.lblCumplimiento.Size = new System.Drawing.Size(99, 18);
            this.lblCumplimiento.TabIndex = 3;
            this.lblCumplimiento.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblArchivo
            // 
            this.lblArchivo.BackColor = System.Drawing.Color.Snow;
            this.lblArchivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblArchivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArchivo.Location = new System.Drawing.Point(0, 0);
            this.lblArchivo.Name = "lblArchivo";
            this.lblArchivo.Size = new System.Drawing.Size(99, 18);
            this.lblArchivo.TabIndex = 2;
            this.lblArchivo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // VisorCumplimiento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCumplimiento);
            this.Controls.Add(this.lblArchivo);
            this.Name = "VisorCumplimiento";
            this.Size = new System.Drawing.Size(200, 19);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCumplimiento;
        private System.Windows.Forms.Label lblArchivo;
    }
}
