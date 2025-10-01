namespace Free1X2.UI.Controls
{
    partial class CtrlFormato123
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
            this.txtAciertosMax1 = new System.Windows.Forms.TextBox();
            this.txtAciertosMin1 = new System.Windows.Forms.TextBox();
            this.txtFormato1 = new System.Windows.Forms.TextBox();
            this.lblNumeroFormato = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtAciertosMax1
            // 
            this.txtAciertosMax1.BackColor = System.Drawing.Color.White;
            this.txtAciertosMax1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAciertosMax1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAciertosMax1.Location = new System.Drawing.Point(177, 1);
            this.txtAciertosMax1.MaxLength = 2;
            this.txtAciertosMax1.Name = "txtAciertosMax1";
            this.txtAciertosMax1.Size = new System.Drawing.Size(24, 13);
            this.txtAciertosMax1.TabIndex = 8;
            this.txtAciertosMax1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAciertosMin1
            // 
            this.txtAciertosMin1.BackColor = System.Drawing.Color.White;
            this.txtAciertosMin1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAciertosMin1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAciertosMin1.Location = new System.Drawing.Point(145, 1);
            this.txtAciertosMin1.MaxLength = 2;
            this.txtAciertosMin1.Name = "txtAciertosMin1";
            this.txtAciertosMin1.Size = new System.Drawing.Size(24, 13);
            this.txtAciertosMin1.TabIndex = 7;
            this.txtAciertosMin1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtFormato1
            // 
            this.txtFormato1.BackColor = System.Drawing.Color.White;
            this.txtFormato1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFormato1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFormato1.Location = new System.Drawing.Point(33, 1);
            this.txtFormato1.MaxLength = 14;
            this.txtFormato1.Name = "txtFormato1";
            this.txtFormato1.Size = new System.Drawing.Size(100, 13);
            this.txtFormato1.TabIndex = 6;
            // 
            // lblNumeroFormato
            // 
            this.lblNumeroFormato.Location = new System.Drawing.Point(5, 1);
            this.lblNumeroFormato.Name = "lblNumeroFormato";
            this.lblNumeroFormato.Size = new System.Drawing.Size(30, 13);
            this.lblNumeroFormato.TabIndex = 5;
            this.lblNumeroFormato.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CtrlFormato123
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtAciertosMax1);
            this.Controls.Add(this.txtAciertosMin1);
            this.Controls.Add(this.txtFormato1);
            this.Controls.Add(this.lblNumeroFormato);
            this.Name = "CtrlFormato123";
            this.Size = new System.Drawing.Size(207, 15);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAciertosMax1 = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.TextBox txtAciertosMin1 = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.TextBox txtFormato1 = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.Label lblNumeroFormato = new System.Windows.Forms.Label();
    }
}
