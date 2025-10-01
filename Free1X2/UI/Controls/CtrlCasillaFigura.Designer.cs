namespace Free1X2.UI.Controls
{
    partial class CtrlCasillaFigura
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
            this.txtCasilla = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtCasilla
            // 
            this.txtCasilla.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCasilla.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCasilla.Location = new System.Drawing.Point(0, 0);
            this.txtCasilla.Name = "txtCasilla";
            this.txtCasilla.Size = new System.Drawing.Size(110, 18);
            this.txtCasilla.TabIndex = 0;
            // 
            // CtrlCasillaFigura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtCasilla);
            this.Name = "CtrlCasillaFigura";
            this.Size = new System.Drawing.Size(110, 18);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtCasilla;

    }
}
