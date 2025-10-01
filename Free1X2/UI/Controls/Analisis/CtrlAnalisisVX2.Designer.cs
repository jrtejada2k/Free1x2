namespace Free1X2.UI.Controls.Analisis
{
    partial class CtrlAnalisisVX2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlAnalisisVX2));
            this.lblVariantes = new System.Windows.Forms.Label();
            this.lblEquis = new System.Windows.Forms.Label();
            this.lblDoses = new System.Windows.Forms.Label();
            this.lblNoVX2 = new System.Windows.Forms.Label();
            this.btnMarcarCondicion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblVariantes
            // 
            this.lblVariantes.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblVariantes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVariantes.Location = new System.Drawing.Point(0, 19);
            this.lblVariantes.Name = "lblVariantes";
            this.lblVariantes.Size = new System.Drawing.Size(44, 18);
            this.lblVariantes.TabIndex = 0;
            this.lblVariantes.Text = "V";
            this.lblVariantes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVariantes.Click += new System.EventHandler(this.lblVariantes_Click);
            // 
            // lblEquis
            // 
            this.lblEquis.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblEquis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEquis.Location = new System.Drawing.Point(0, 38);
            this.lblEquis.Name = "lblEquis";
            this.lblEquis.Size = new System.Drawing.Size(44, 18);
            this.lblEquis.TabIndex = 1;
            this.lblEquis.Text = "X";
            this.lblEquis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEquis.Click += new System.EventHandler(this.lblEquis_Click);
            // 
            // lblDoses
            // 
            this.lblDoses.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblDoses.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDoses.Location = new System.Drawing.Point(0, 57);
            this.lblDoses.Name = "lblDoses";
            this.lblDoses.Size = new System.Drawing.Size(44, 18);
            this.lblDoses.TabIndex = 2;
            this.lblDoses.Text = "2";
            this.lblDoses.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDoses.Click += new System.EventHandler(this.lblDoses_Click);
            // 
            // lblNoVX2
            // 
            this.lblNoVX2.BackColor = System.Drawing.Color.NavajoWhite;
            this.lblNoVX2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoVX2.Location = new System.Drawing.Point(0, 0);
            this.lblNoVX2.Name = "lblNoVX2";
            this.lblNoVX2.Size = new System.Drawing.Size(44, 18);
            this.lblNoVX2.TabIndex = 3;
            this.lblNoVX2.Text = "Nº";
            this.lblNoVX2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMarcarCondicion
            // 
            this.btnMarcarCondicion.BackColor = System.Drawing.Color.NavajoWhite;
            this.btnMarcarCondicion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnMarcarCondicion.Image = ((System.Drawing.Image)(resources.GetObject("btnMarcarCondicion.Image")));
            this.btnMarcarCondicion.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMarcarCondicion.Location = new System.Drawing.Point(0, 82);
            this.btnMarcarCondicion.Name = "btnMarcarCondicion";
            this.btnMarcarCondicion.Size = new System.Drawing.Size(162, 23);
            this.btnMarcarCondicion.TabIndex = 36;
            this.btnMarcarCondicion.Text = "Marcar Condición";
            this.btnMarcarCondicion.UseVisualStyleBackColor = false;
            this.btnMarcarCondicion.Click += new System.EventHandler(this.btnMarcarCondicion_Click);
            // 
            // CtrlAnalisisVX2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.btnMarcarCondicion);
            this.Controls.Add(this.lblNoVX2);
            this.Controls.Add(this.lblDoses);
            this.Controls.Add(this.lblEquis);
            this.Controls.Add(this.lblVariantes);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CtrlAnalisisVX2";
            this.Size = new System.Drawing.Size(839, 225);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblVariantes;
        private System.Windows.Forms.Label lblEquis;
        private System.Windows.Forms.Label lblDoses;
        private System.Windows.Forms.Label lblNoVX2;
        private System.Windows.Forms.Button btnMarcarCondicion;
    }
}
