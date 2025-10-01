namespace Free1X2.UI
{
    partial class ConfiguracionAnalisisFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfiguracionAnalisisFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.chkTodo = new System.Windows.Forms.CheckBox();
            this.chkVX2 = new System.Windows.Forms.CheckBox();
            this.chkSeguidos = new System.Windows.Forms.CheckBox();
            this.chkInterrupciones = new System.Windows.Forms.CheckBox();
            this.chkDibujos = new System.Windows.Forms.CheckBox();
            this.chkFormatos = new System.Windows.Forms.CheckBox();
            this.chkGruposEquipos = new System.Windows.Forms.CheckBox();
            this.chkContactos = new System.Windows.Forms.CheckBox();
            this.chkDistancias = new System.Windows.Forms.CheckBox();
            this.chkCPs = new System.Windows.Forms.CheckBox();
            this.chkValoracion = new System.Windows.Forms.CheckBox();
            this.chkPesos = new System.Windows.Forms.CheckBox();
            this.chkSimetrias = new System.Windows.Forms.CheckBox();
            this.chkFormatos123 = new System.Windows.Forms.CheckBox();
            this.chkFigContactos = new System.Windows.Forms.CheckBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.chkControlGrupos = new System.Windows.Forms.CheckBox();
            this.chkControlConjuntos = new System.Windows.Forms.CheckBox();
            this.btnSalir = new System.Windows.Forms.Button();
            this.chkNada = new System.Windows.Forms.CheckBox();
            this.chkFigurasV1X2 = new System.Windows.Forms.CheckBox();
            this.chkFigPesos = new System.Windows.Forms.CheckBox();
            this.chkDiferencias = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Analizar";
            // 
            // chkTodo
            // 
            this.chkTodo.AutoSize = true;
            this.chkTodo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTodo.Location = new System.Drawing.Point(15, 35);
            this.chkTodo.Name = "chkTodo";
            this.chkTodo.Size = new System.Drawing.Size(54, 17);
            this.chkTodo.TabIndex = 1;
            this.chkTodo.Text = "Todo";
            this.chkTodo.UseVisualStyleBackColor = true;
            this.chkTodo.CheckedChanged += new System.EventHandler(this.chkTodo_CheckedChanged);
            // 
            // chkVX2
            // 
            this.chkVX2.AutoSize = true;
            this.chkVX2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVX2.Location = new System.Drawing.Point(15, 60);
            this.chkVX2.Name = "chkVX2";
            this.chkVX2.Size = new System.Drawing.Size(49, 17);
            this.chkVX2.TabIndex = 2;
            this.chkVX2.Text = "VX2";
            this.chkVX2.UseVisualStyleBackColor = true;
            this.chkVX2.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkSeguidos
            // 
            this.chkSeguidos.AutoSize = true;
            this.chkSeguidos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSeguidos.Location = new System.Drawing.Point(15, 79);
            this.chkSeguidos.Name = "chkSeguidos";
            this.chkSeguidos.Size = new System.Drawing.Size(120, 17);
            this.chkSeguidos.TabIndex = 3;
            this.chkSeguidos.Text = "Signos Seguidos";
            this.chkSeguidos.UseVisualStyleBackColor = true;
            this.chkSeguidos.CheckedChanged += new System.EventHandler(this.chkSeguidos_CheckedChanged);
            // 
            // chkInterrupciones
            // 
            this.chkInterrupciones.AutoSize = true;
            this.chkInterrupciones.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkInterrupciones.Location = new System.Drawing.Point(15, 136);
            this.chkInterrupciones.Name = "chkInterrupciones";
            this.chkInterrupciones.Size = new System.Drawing.Size(109, 17);
            this.chkInterrupciones.TabIndex = 5;
            this.chkInterrupciones.Text = "Interrupciones";
            this.chkInterrupciones.UseVisualStyleBackColor = true;
            this.chkInterrupciones.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkDibujos
            // 
            this.chkDibujos.AutoSize = true;
            this.chkDibujos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDibujos.Location = new System.Drawing.Point(15, 117);
            this.chkDibujos.Name = "chkDibujos";
            this.chkDibujos.Size = new System.Drawing.Size(69, 17);
            this.chkDibujos.TabIndex = 4;
            this.chkDibujos.Text = "Dibujos";
            this.chkDibujos.UseVisualStyleBackColor = true;
            this.chkDibujos.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkFormatos
            // 
            this.chkFormatos.AutoSize = true;
            this.chkFormatos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFormatos.Location = new System.Drawing.Point(15, 174);
            this.chkFormatos.Name = "chkFormatos";
            this.chkFormatos.Size = new System.Drawing.Size(79, 17);
            this.chkFormatos.TabIndex = 7;
            this.chkFormatos.Text = "Formatos";
            this.chkFormatos.UseVisualStyleBackColor = true;
            this.chkFormatos.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkGruposEquipos
            // 
            this.chkGruposEquipos.AutoSize = true;
            this.chkGruposEquipos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkGruposEquipos.Location = new System.Drawing.Point(15, 155);
            this.chkGruposEquipos.Name = "chkGruposEquipos";
            this.chkGruposEquipos.Size = new System.Drawing.Size(115, 17);
            this.chkGruposEquipos.TabIndex = 6;
            this.chkGruposEquipos.Text = "Grupos Equipos";
            this.chkGruposEquipos.UseVisualStyleBackColor = true;
            this.chkGruposEquipos.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkContactos
            // 
            this.chkContactos.AutoSize = true;
            this.chkContactos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkContactos.Location = new System.Drawing.Point(15, 326);
            this.chkContactos.Name = "chkContactos";
            this.chkContactos.Size = new System.Drawing.Size(83, 17);
            this.chkContactos.TabIndex = 13;
            this.chkContactos.Text = "Contactos";
            this.chkContactos.UseVisualStyleBackColor = true;
            this.chkContactos.CheckedChanged += new System.EventHandler(this.chkContactos_CheckedChanged);
            // 
            // chkDistancias
            // 
            this.chkDistancias.AutoSize = true;
            this.chkDistancias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDistancias.Location = new System.Drawing.Point(15, 307);
            this.chkDistancias.Name = "chkDistancias";
            this.chkDistancias.Size = new System.Drawing.Size(84, 17);
            this.chkDistancias.TabIndex = 12;
            this.chkDistancias.Text = "Distancias";
            this.chkDistancias.UseVisualStyleBackColor = true;
            this.chkDistancias.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkCPs
            // 
            this.chkCPs.AutoSize = true;
            this.chkCPs.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCPs.Location = new System.Drawing.Point(15, 288);
            this.chkCPs.Name = "chkCPs";
            this.chkCPs.Size = new System.Drawing.Size(143, 17);
            this.chkCPs.TabIndex = 11;
            this.chkCPs.Text = "Columnas Probables";
            this.chkCPs.UseVisualStyleBackColor = true;
            this.chkCPs.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkValoracion
            // 
            this.chkValoracion.AutoSize = true;
            this.chkValoracion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkValoracion.Location = new System.Drawing.Point(15, 269);
            this.chkValoracion.Name = "chkValoracion";
            this.chkValoracion.Size = new System.Drawing.Size(86, 17);
            this.chkValoracion.TabIndex = 10;
            this.chkValoracion.Text = "Valoración";
            this.chkValoracion.UseVisualStyleBackColor = true;
            this.chkValoracion.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkPesos
            // 
            this.chkPesos.AutoSize = true;
            this.chkPesos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPesos.Location = new System.Drawing.Point(15, 231);
            this.chkPesos.Name = "chkPesos";
            this.chkPesos.Size = new System.Drawing.Size(123, 17);
            this.chkPesos.TabIndex = 9;
            this.chkPesos.Text = "Pesos Numéricos";
            this.chkPesos.UseVisualStyleBackColor = true;
            this.chkPesos.CheckedChanged += new System.EventHandler(this.chkPesos_CheckedChanged);
            // 
            // chkSimetrias
            // 
            this.chkSimetrias.AutoSize = true;
            this.chkSimetrias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSimetrias.Location = new System.Drawing.Point(15, 193);
            this.chkSimetrias.Name = "chkSimetrias";
            this.chkSimetrias.Size = new System.Drawing.Size(80, 17);
            this.chkSimetrias.TabIndex = 8;
            this.chkSimetrias.Text = "Simetrías";
            this.chkSimetrias.UseVisualStyleBackColor = true;
            this.chkSimetrias.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkFormatos123
            // 
            this.chkFormatos123.AutoSize = true;
            this.chkFormatos123.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFormatos123.Location = new System.Drawing.Point(15, 364);
            this.chkFormatos123.Name = "chkFormatos123";
            this.chkFormatos123.Size = new System.Drawing.Size(104, 17);
            this.chkFormatos123.TabIndex = 14;
            this.chkFormatos123.Text = "Formatos 123";
            this.chkFormatos123.UseVisualStyleBackColor = true;
            this.chkFormatos123.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkFigContactos
            // 
            this.chkFigContactos.AutoSize = true;
            this.chkFigContactos.Enabled = false;
            this.chkFigContactos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFigContactos.Location = new System.Drawing.Point(34, 345);
            this.chkFigContactos.Name = "chkFigContactos";
            this.chkFigContactos.Size = new System.Drawing.Size(146, 17);
            this.chkFigContactos.TabIndex = 15;
            this.chkFigContactos.Text = "Figuras de Contactos";
            this.chkFigContactos.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.Image")));
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Location = new System.Drawing.Point(26, 424);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(93, 32);
            this.btnGuardar.TabIndex = 26;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // chkControlGrupos
            // 
            this.chkControlGrupos.AutoSize = true;
            this.chkControlGrupos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkControlGrupos.Location = new System.Drawing.Point(15, 383);
            this.chkControlGrupos.Name = "chkControlGrupos";
            this.chkControlGrupos.Size = new System.Drawing.Size(131, 17);
            this.chkControlGrupos.TabIndex = 27;
            this.chkControlGrupos.Text = "Control de Grupos";
            this.chkControlGrupos.UseVisualStyleBackColor = true;
            this.chkControlGrupos.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkControlConjuntos
            // 
            this.chkControlConjuntos.AutoSize = true;
            this.chkControlConjuntos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkControlConjuntos.Location = new System.Drawing.Point(15, 402);
            this.chkControlConjuntos.Name = "chkControlConjuntos";
            this.chkControlConjuntos.Size = new System.Drawing.Size(148, 17);
            this.chkControlConjuntos.TabIndex = 28;
            this.chkControlConjuntos.Text = "Control de Conjuntos";
            this.chkControlConjuntos.UseVisualStyleBackColor = true;
            this.chkControlConjuntos.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSalir.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.Image")));
            this.btnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.Location = new System.Drawing.Point(120, 424);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(93, 32);
            this.btnSalir.TabIndex = 29;
            this.btnSalir.Text = "Salir";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // chkNada
            // 
            this.chkNada.AutoSize = true;
            this.chkNada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNada.Location = new System.Drawing.Point(85, 35);
            this.chkNada.Name = "chkNada";
            this.chkNada.Size = new System.Drawing.Size(55, 17);
            this.chkNada.TabIndex = 30;
            this.chkNada.Text = "Nada";
            this.chkNada.UseVisualStyleBackColor = true;
            this.chkNada.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkFigurasV1X2
            // 
            this.chkFigurasV1X2.AutoSize = true;
            this.chkFigurasV1X2.Enabled = false;
            this.chkFigurasV1X2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFigurasV1X2.Location = new System.Drawing.Point(34, 98);
            this.chkFigurasV1X2.Name = "chkFigurasV1X2";
            this.chkFigurasV1X2.Size = new System.Drawing.Size(101, 17);
            this.chkFigurasV1X2.TabIndex = 31;
            this.chkFigurasV1X2.Text = "Figuras V1X2";
            this.chkFigurasV1X2.UseVisualStyleBackColor = true;
            // 
            // chkFigPesos
            // 
            this.chkFigPesos.AutoSize = true;
            this.chkFigPesos.Enabled = false;
            this.chkFigPesos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFigPesos.Location = new System.Drawing.Point(34, 250);
            this.chkFigPesos.Name = "chkFigPesos";
            this.chkFigPesos.Size = new System.Drawing.Size(186, 17);
            this.chkFigPesos.TabIndex = 32;
            this.chkFigPesos.Text = "Figuras de Pesos Numéricos";
            this.chkFigPesos.UseVisualStyleBackColor = true;
            // 
            // chkDiferencias
            // 
            this.chkDiferencias.AutoSize = true;
            this.chkDiferencias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDiferencias.Location = new System.Drawing.Point(15, 212);
            this.chkDiferencias.Name = "chkDiferencias";
            this.chkDiferencias.Size = new System.Drawing.Size(90, 17);
            this.chkDiferencias.TabIndex = 33;
            this.chkDiferencias.Text = "Diferencias";
            this.chkDiferencias.UseVisualStyleBackColor = true;
            // 
            // ConfiguracionAnalisisFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(239, 476);
            this.ControlBox = false;
            this.Controls.Add(this.chkDiferencias);
            this.Controls.Add(this.chkFigPesos);
            this.Controls.Add(this.chkFigurasV1X2);
            this.Controls.Add(this.chkNada);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.chkControlConjuntos);
            this.Controls.Add(this.chkControlGrupos);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.chkFigContactos);
            this.Controls.Add(this.chkFormatos123);
            this.Controls.Add(this.chkContactos);
            this.Controls.Add(this.chkDistancias);
            this.Controls.Add(this.chkCPs);
            this.Controls.Add(this.chkValoracion);
            this.Controls.Add(this.chkPesos);
            this.Controls.Add(this.chkSimetrias);
            this.Controls.Add(this.chkFormatos);
            this.Controls.Add(this.chkGruposEquipos);
            this.Controls.Add(this.chkInterrupciones);
            this.Controls.Add(this.chkDibujos);
            this.Controls.Add(this.chkSeguidos);
            this.Controls.Add(this.chkVX2);
            this.Controls.Add(this.chkTodo);
            this.Controls.Add(this.label1);
            this.Name = "ConfiguracionAnalisisFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Configurar Análisis";
            this.Load += new System.EventHandler(this.ConfiguracionAnalisisFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkTodo;
        private System.Windows.Forms.CheckBox chkVX2;
        private System.Windows.Forms.CheckBox chkSeguidos;
        private System.Windows.Forms.CheckBox chkInterrupciones;
        private System.Windows.Forms.CheckBox chkDibujos;
        private System.Windows.Forms.CheckBox chkFormatos;
        private System.Windows.Forms.CheckBox chkGruposEquipos;
        private System.Windows.Forms.CheckBox chkContactos;
        private System.Windows.Forms.CheckBox chkDistancias;
        private System.Windows.Forms.CheckBox chkCPs;
        private System.Windows.Forms.CheckBox chkValoracion;
        private System.Windows.Forms.CheckBox chkPesos;
        private System.Windows.Forms.CheckBox chkSimetrias;
        private System.Windows.Forms.CheckBox chkFormatos123;
        private System.Windows.Forms.CheckBox chkFigContactos;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.CheckBox chkControlGrupos;
        private System.Windows.Forms.CheckBox chkControlConjuntos;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.CheckBox chkNada;
        private System.Windows.Forms.CheckBox chkFigurasV1X2;
        private System.Windows.Forms.CheckBox chkFigPesos;
        private System.Windows.Forms.CheckBox chkDiferencias;
    }
}