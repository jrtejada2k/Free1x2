namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlCasilla
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
            this.lblCasilla = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblCasilla
            // 
            this.lblCasilla.BackColor = System.Drawing.Color.White;
            this.lblCasilla.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCasilla.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCasilla.Location = new System.Drawing.Point(0, 0);
            this.lblCasilla.Name = "lblCasilla";
            this.lblCasilla.Size = new System.Drawing.Size(45, 18);
            this.lblCasilla.TabIndex = 1;
            this.lblCasilla.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CtrlCasilla
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lblCasilla);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CtrlCasilla";
            this.Size = new System.Drawing.Size(45, 18);
            this.ResumeLayout(false);

        }

        #endregion

       private System.Windows.Forms.Label lblCasilla;


    }
}
