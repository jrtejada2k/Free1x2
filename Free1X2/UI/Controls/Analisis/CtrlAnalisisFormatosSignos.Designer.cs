namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlAnalisisFormatosSignos
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
            this.lblNoLineas = new System.Windows.Forms.Label();
            this.lblGlobal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNoLineas
            // 
            this.lblNoLineas.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblNoLineas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoLineas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoLineas.Location = new System.Drawing.Point(0, 0);
            this.lblNoLineas.Name = "lblNoLineas";
            this.lblNoLineas.Size = new System.Drawing.Size(66, 18);
            this.lblNoLineas.TabIndex = 16;
            this.lblNoLineas.Text = "Líneas";
            this.lblNoLineas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGlobal
            // 
            this.lblGlobal.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblGlobal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGlobal.Location = new System.Drawing.Point(0, 63);
            this.lblGlobal.Name = "lblGlobal";
            this.lblGlobal.Size = new System.Drawing.Size(66, 18);
            this.lblGlobal.TabIndex = 15;
            this.lblGlobal.Text = "Global";
            this.lblGlobal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CtrlAnalisisFormatosSignos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.lblNoLineas);
            this.Controls.Add(this.lblGlobal);
            this.Name = "CtrlAnalisisFormatosSignos";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(790, 226);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNoLineas;
        private System.Windows.Forms.Label lblGlobal;
    }
}
