namespace Free1X2.UI
{
    partial class EstucolFrm
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

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EstucolFrm));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAbreArchivoGanadoras = new System.Windows.Forms.Button();
            this.lblArchivoGanadoras = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAbreArchivoReducidas = new System.Windows.Forms.Button();
            this.lblNombreArchivoReducidas = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdbAgrupacionC = new System.Windows.Forms.RadioButton();
            this.rdbAgrupacionB = new System.Windows.Forms.RadioButton();
            this.rdbAgrupacionA = new System.Windows.Forms.RadioButton();
            this.btnComenzar = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblEstado = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAbreArchivoGanadoras);
            this.groupBox2.Controls.Add(this.lblArchivoGanadoras);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(10, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 63);
            this.groupBox2.TabIndex = 147;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Entrada Columnas Ganadoras";
            // 
            // btnAbreArchivoGanadoras
            // 
            this.btnAbreArchivoGanadoras.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbreArchivoGanadoras.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbreArchivoGanadoras.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbreArchivoGanadoras.Image = ((System.Drawing.Image)(resources.GetObject("btnAbreArchivoGanadoras.Image")));
            this.btnAbreArchivoGanadoras.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbreArchivoGanadoras.Location = new System.Drawing.Point(10, 19);
            this.btnAbreArchivoGanadoras.Name = "btnAbreArchivoGanadoras";
            this.btnAbreArchivoGanadoras.Size = new System.Drawing.Size(100, 25);
            this.btnAbreArchivoGanadoras.TabIndex = 140;
            this.btnAbreArchivoGanadoras.Text = "Archivo...";
            this.btnAbreArchivoGanadoras.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbreArchivoGanadoras.UseVisualStyleBackColor = false;
            this.btnAbreArchivoGanadoras.Click += new System.EventHandler(this.btnAbreArchivoGanadoras_Click);
            // 
            // lblArchivoGanadoras
            // 
            this.lblArchivoGanadoras.BackColor = System.Drawing.Color.White;
            this.lblArchivoGanadoras.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblArchivoGanadoras.Location = new System.Drawing.Point(116, 19);
            this.lblArchivoGanadoras.Name = "lblArchivoGanadoras";
            this.lblArchivoGanadoras.Size = new System.Drawing.Size(120, 25);
            this.lblArchivoGanadoras.TabIndex = 141;
            this.lblArchivoGanadoras.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAbreArchivoReducidas);
            this.groupBox1.Controls.Add(this.lblNombreArchivoReducidas);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 63);
            this.groupBox1.TabIndex = 146;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Entrada Columnas";
            // 
            // btnAbreArchivoReducidas
            // 
            this.btnAbreArchivoReducidas.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbreArchivoReducidas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbreArchivoReducidas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbreArchivoReducidas.Image = ((System.Drawing.Image)(resources.GetObject("btnAbreArchivoReducidas.Image")));
            this.btnAbreArchivoReducidas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbreArchivoReducidas.Location = new System.Drawing.Point(10, 19);
            this.btnAbreArchivoReducidas.Name = "btnAbreArchivoReducidas";
            this.btnAbreArchivoReducidas.Size = new System.Drawing.Size(100, 25);
            this.btnAbreArchivoReducidas.TabIndex = 138;
            this.btnAbreArchivoReducidas.Text = "Archivo...";
            this.btnAbreArchivoReducidas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbreArchivoReducidas.UseVisualStyleBackColor = false;
            this.btnAbreArchivoReducidas.Click += new System.EventHandler(this.btnAbreArchivoReducidas_Click);
            // 
            // lblNombreArchivoReducidas
            // 
            this.lblNombreArchivoReducidas.BackColor = System.Drawing.Color.White;
            this.lblNombreArchivoReducidas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNombreArchivoReducidas.Location = new System.Drawing.Point(116, 19);
            this.lblNombreArchivoReducidas.Name = "lblNombreArchivoReducidas";
            this.lblNombreArchivoReducidas.Size = new System.Drawing.Size(120, 25);
            this.lblNombreArchivoReducidas.TabIndex = 139;
            this.lblNombreArchivoReducidas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdbAgrupacionC);
            this.groupBox3.Controls.Add(this.rdbAgrupacionB);
            this.groupBox3.Controls.Add(this.rdbAgrupacionA);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(267, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(151, 125);
            this.groupBox3.TabIndex = 148;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Agrupación de Columnas";
            // 
            // rdbAgrupacionC
            // 
            this.rdbAgrupacionC.AutoSize = true;
            this.rdbAgrupacionC.Location = new System.Drawing.Point(39, 79);
            this.rdbAgrupacionC.Name = "rdbAgrupacionC";
            this.rdbAgrupacionC.Size = new System.Drawing.Size(86, 17);
            this.rdbAgrupacionC.TabIndex = 150;
            this.rdbAgrupacionC.Text = "1,3 - 2,4...";
            this.rdbAgrupacionC.UseVisualStyleBackColor = true;
            // 
            // rdbAgrupacionB
            // 
            this.rdbAgrupacionB.AutoSize = true;
            this.rdbAgrupacionB.Location = new System.Drawing.Point(39, 54);
            this.rdbAgrupacionB.Name = "rdbAgrupacionB";
            this.rdbAgrupacionB.Size = new System.Drawing.Size(86, 17);
            this.rdbAgrupacionB.TabIndex = 149;
            this.rdbAgrupacionB.Text = "1,2 - 2,3...";
            this.rdbAgrupacionB.UseVisualStyleBackColor = true;
            // 
            // rdbAgrupacionA
            // 
            this.rdbAgrupacionA.AutoSize = true;
            this.rdbAgrupacionA.Checked = true;
            this.rdbAgrupacionA.Location = new System.Drawing.Point(39, 29);
            this.rdbAgrupacionA.Name = "rdbAgrupacionA";
            this.rdbAgrupacionA.Size = new System.Drawing.Size(86, 17);
            this.rdbAgrupacionA.TabIndex = 0;
            this.rdbAgrupacionA.TabStop = true;
            this.rdbAgrupacionA.Text = "1,2 - 3,4...";
            this.rdbAgrupacionA.UseVisualStyleBackColor = true;
            // 
            // btnComenzar
            // 
            this.btnComenzar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnComenzar.Enabled = false;
            this.btnComenzar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnComenzar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComenzar.Image = ((System.Drawing.Image)(resources.GetObject("btnComenzar.Image")));
            this.btnComenzar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnComenzar.Location = new System.Drawing.Point(145, 157);
            this.btnComenzar.Name = "btnComenzar";
            this.btnComenzar.Size = new System.Drawing.Size(139, 25);
            this.btnComenzar.TabIndex = 149;
            this.btnComenzar.Text = "Obtener Informe";
            this.btnComenzar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnComenzar.UseVisualStyleBackColor = false;
            this.btnComenzar.Click += new System.EventHandler(this.btnComenzar_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstado});
            this.statusStrip1.Location = new System.Drawing.Point(0, 190);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(428, 22);
            this.statusStrip1.TabIndex = 150;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblEstado
            // 
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(29, 17);
            this.lblEstado.Text = "Listo";
            // 
            // EstucolFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(428, 212);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnComenzar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EstucolFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "EstuCol - Generador / Analizador de Columnas Probables";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EstucolFrm_FormClosed);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAbreArchivoGanadoras;
        private System.Windows.Forms.Label lblArchivoGanadoras;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAbreArchivoReducidas;
        private System.Windows.Forms.Label lblNombreArchivoReducidas;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdbAgrupacionB;
        private System.Windows.Forms.RadioButton rdbAgrupacionA;
        private System.Windows.Forms.RadioButton rdbAgrupacionC;
        private System.Windows.Forms.Button btnComenzar;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblEstado;
    }
}