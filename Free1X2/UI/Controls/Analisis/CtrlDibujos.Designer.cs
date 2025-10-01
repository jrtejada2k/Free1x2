namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlDibujos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlDibujos));
            this.btnMarcarCondicion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMarcarCondicion
            // 
            this.btnMarcarCondicion.BackColor = System.Drawing.Color.NavajoWhite;
            this.btnMarcarCondicion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnMarcarCondicion.Image = ((System.Drawing.Image)(resources.GetObject("btnMarcarCondicion.Image")));
            this.btnMarcarCondicion.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMarcarCondicion.Location = new System.Drawing.Point(596, 268);
            this.btnMarcarCondicion.Name = "btnMarcarCondicion";
            this.btnMarcarCondicion.Size = new System.Drawing.Size(162, 23);
            this.btnMarcarCondicion.TabIndex = 37;
            this.btnMarcarCondicion.Text = "Marcar Condición";
            this.btnMarcarCondicion.UseVisualStyleBackColor = false;
            this.btnMarcarCondicion.Click += new System.EventHandler(this.btnMarcarCondicion_Click);
            // 
            // CtrlDibujos
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.btnMarcarCondicion);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CtrlDibujos";
            this.Size = new System.Drawing.Size(790, 393);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMarcarCondicion;
    }
}
