namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlAnalisisSimetrias
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
            this.lblNoAciertos = new System.Windows.Forms.Label();
            this.lblColumnas = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNoAciertos
            // 
            this.lblNoAciertos.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblNoAciertos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoAciertos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoAciertos.Location = new System.Drawing.Point(0, 0);
            this.lblNoAciertos.Name = "lblNoAciertos";
            this.lblNoAciertos.Size = new System.Drawing.Size(66, 18);
            this.lblNoAciertos.TabIndex = 14;
            this.lblNoAciertos.Text = "Aciertos";
            this.lblNoAciertos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblColumnas
            // 
            this.lblColumnas.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblColumnas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColumnas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColumnas.Location = new System.Drawing.Point(0, 19);
            this.lblColumnas.Name = "lblColumnas";
            this.lblColumnas.Size = new System.Drawing.Size(66, 18);
            this.lblColumnas.TabIndex = 13;
            this.lblColumnas.Text = "Columnas";
            this.lblColumnas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CtrlAnalisisSimetrias
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.lblNoAciertos);
            this.Controls.Add(this.lblColumnas);
            this.Name = "CtrlAnalisisSimetrias";
            this.Size = new System.Drawing.Size(790, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNoAciertos;
        private System.Windows.Forms.Label lblColumnas;
    }
}
