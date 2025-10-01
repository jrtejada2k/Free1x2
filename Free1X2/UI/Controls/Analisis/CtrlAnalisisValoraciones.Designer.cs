namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlAnalisisValoraciones
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlAnalisisValoraciones));
            this.ContControl = new System.Windows.Forms.ContainerControl();
            this.lblUnos = new System.Windows.Forms.Label();
            this.lblEquis = new System.Windows.Forms.Label();
            this.lblDoses = new System.Windows.Forms.Label();
            this.lblGlobal = new System.Windows.Forms.Label();
            this.lblTipo = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ContControl
            // 
            this.ContControl.AutoScroll = true;
            this.ContControl.BackColor = System.Drawing.Color.Bisque;
            this.ContControl.ImeMode = System.Windows.Forms.ImeMode.On;
            this.ContControl.Location = new System.Drawing.Point(37, 67);
            this.ContControl.Name = "ContControl";
            this.ContControl.Size = new System.Drawing.Size(535, 254);
            this.ContControl.TabIndex = 1;
            // 
            // lblUnos
            // 
            this.lblUnos.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblUnos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblUnos.Location = new System.Drawing.Point(213, 44);
            this.lblUnos.Name = "lblUnos";
            this.lblUnos.Size = new System.Drawing.Size(91, 20);
            this.lblUnos.TabIndex = 5;
            this.lblUnos.Text = "Unos";
            this.lblUnos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEquis
            // 
            this.lblEquis.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblEquis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEquis.Location = new System.Drawing.Point(346, 44);
            this.lblEquis.Name = "lblEquis";
            this.lblEquis.Size = new System.Drawing.Size(91, 20);
            this.lblEquis.TabIndex = 6;
            this.lblEquis.Text = "Equis";
            this.lblEquis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDoses
            // 
            this.lblDoses.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblDoses.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDoses.Location = new System.Drawing.Point(479, 44);
            this.lblDoses.Name = "lblDoses";
            this.lblDoses.Size = new System.Drawing.Size(91, 20);
            this.lblDoses.TabIndex = 7;
            this.lblDoses.Text = "Doses";
            this.lblDoses.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGlobal
            // 
            this.lblGlobal.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblGlobal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGlobal.Location = new System.Drawing.Point(80, 44);
            this.lblGlobal.Name = "lblGlobal";
            this.lblGlobal.Size = new System.Drawing.Size(91, 20);
            this.lblGlobal.TabIndex = 5;
            this.lblGlobal.Text = "Global";
            this.lblGlobal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTipo
            // 
            this.lblTipo.AutoSize = true;
            this.lblTipo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipo.Location = new System.Drawing.Point(63, 13);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(0, 15);
            this.lblTipo.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Bisque;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(589, 44);
            this.label9.Name = "label9";
            this.label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label9.Size = new System.Drawing.Size(144, 219);
            this.label9.TabIndex = 10;
            this.label9.Text = resources.GetString("label9.Text");
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CtrlAnalisisValoraciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblTipo);
            this.Controls.Add(this.lblDoses);
            this.Controls.Add(this.lblGlobal);
            this.Controls.Add(this.lblEquis);
            this.Controls.Add(this.ContControl);
            this.Controls.Add(this.lblUnos);
            this.Name = "CtrlAnalisisValoraciones";
            this.Size = new System.Drawing.Size(768, 324);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContainerControl ContControl;
        private System.Windows.Forms.Label lblDoses;
        private System.Windows.Forms.Label lblEquis;
        private System.Windows.Forms.Label lblUnos;
        private System.Windows.Forms.Label lblGlobal;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.Label label9;
    }
}
