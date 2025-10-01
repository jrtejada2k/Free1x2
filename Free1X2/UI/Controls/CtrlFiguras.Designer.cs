namespace Free1X2.UI.Controls
{
    partial class CtrlFiguras
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
            this.contControl = new System.Windows.Forms.ContainerControl();
            this.SuspendLayout();
            // 
            // contControl
            // 
            this.contControl.AutoScroll = true;
            this.contControl.BackColor = System.Drawing.Color.Bisque;
            this.contControl.Location = new System.Drawing.Point(2, 3);
            this.contControl.Name = "contControl";
            this.contControl.Size = new System.Drawing.Size(161, 221);
            this.contControl.TabIndex = 0;
            // 
            // CtrlFiguras
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.contControl);
            this.Name = "CtrlFiguras";
            this.Size = new System.Drawing.Size(166, 228);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContainerControl contControl;
    }
}
