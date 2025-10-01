namespace Free1X2.UI
{
    partial class Compresor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Compresor));
            this.lblNombreArchivo = new System.Windows.Forms.Label();
            this.btnAbreArchivo = new System.Windows.Forms.Button();
            this.lblArchivoSalida = new System.Windows.Forms.Label();
            this.btnAbreArchivoComp = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbNivel = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNombreArchivo
            // 
            this.lblNombreArchivo.BackColor = System.Drawing.Color.White;
            this.lblNombreArchivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNombreArchivo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreArchivo.Location = new System.Drawing.Point(116, 19);
            this.lblNombreArchivo.Name = "lblNombreArchivo";
            this.lblNombreArchivo.Size = new System.Drawing.Size(196, 21);
            this.lblNombreArchivo.TabIndex = 139;
            this.lblNombreArchivo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAbreArchivo
            // 
            this.btnAbreArchivo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbreArchivo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbreArchivo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbreArchivo.Image = ((System.Drawing.Image)(resources.GetObject("btnAbreArchivo.Image")));
            this.btnAbreArchivo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbreArchivo.Location = new System.Drawing.Point(10, 19);
            this.btnAbreArchivo.Name = "btnAbreArchivo";
            this.btnAbreArchivo.Size = new System.Drawing.Size(100, 21);
            this.btnAbreArchivo.TabIndex = 138;
            this.btnAbreArchivo.Text = "Archivo...";
            this.btnAbreArchivo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbreArchivo.UseVisualStyleBackColor = false;
            this.btnAbreArchivo.Click += new System.EventHandler(this.btnAbreArchivo_Click);
            // 
            // lblArchivoSalida
            // 
            this.lblArchivoSalida.BackColor = System.Drawing.Color.White;
            this.lblArchivoSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblArchivoSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArchivoSalida.Location = new System.Drawing.Point(116, 19);
            this.lblArchivoSalida.Name = "lblArchivoSalida";
            this.lblArchivoSalida.Size = new System.Drawing.Size(196, 21);
            this.lblArchivoSalida.TabIndex = 141;
            this.lblArchivoSalida.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAbreArchivoComp
            // 
            this.btnAbreArchivoComp.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbreArchivoComp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbreArchivoComp.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbreArchivoComp.Image = ((System.Drawing.Image)(resources.GetObject("btnAbreArchivoComp.Image")));
            this.btnAbreArchivoComp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbreArchivoComp.Location = new System.Drawing.Point(10, 19);
            this.btnAbreArchivoComp.Name = "btnAbreArchivoComp";
            this.btnAbreArchivoComp.Size = new System.Drawing.Size(100, 21);
            this.btnAbreArchivoComp.TabIndex = 140;
            this.btnAbreArchivoComp.Text = "Archivo...";
            this.btnAbreArchivoComp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbreArchivoComp.UseVisualStyleBackColor = false;
            this.btnAbreArchivoComp.Click += new System.EventHandler(this.btnAbreArchivoComp_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbbNivel);
            this.groupBox1.Controls.Add(this.btnAbreArchivo);
            this.groupBox1.Controls.Add(this.lblNombreArchivo);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 71);
            this.groupBox1.TabIndex = 144;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Comprimir";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 21);
            this.label1.TabIndex = 141;
            this.label1.Text = "Nivel de Compresión:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbbNivel
            // 
            this.cbbNivel.FormattingEnabled = true;
            this.cbbNivel.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.cbbNivel.Location = new System.Drawing.Point(161, 43);
            this.cbbNivel.Name = "cbbNivel";
            this.cbbNivel.Size = new System.Drawing.Size(38, 21);
            this.cbbNivel.TabIndex = 140;
            this.cbbNivel.Text = "5";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAbreArchivoComp);
            this.groupBox2.Controls.Add(this.lblArchivoSalida);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(320, 71);
            this.groupBox2.TabIndex = 145;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Descomprimir";
            // 
            // lblEstado
            // 
            this.lblEstado.Location = new System.Drawing.Point(13, 156);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(302, 60);
            this.lblEstado.TabIndex = 146;
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCerrar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.Image = ((System.Drawing.Image)(resources.GetObject("btnCerrar.Image")));
            this.btnCerrar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrar.Location = new System.Drawing.Point(114, 221);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 25);
            this.btnCerrar.TabIndex = 147;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // Compresor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(326, 259);
            this.ControlBox = false;
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Compresor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Compresor  *.z3q";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNombreArchivo;
        private System.Windows.Forms.Button btnAbreArchivo;
        private System.Windows.Forms.Label lblArchivoSalida;
        private System.Windows.Forms.Button btnAbreArchivoComp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbNivel;
    }
}