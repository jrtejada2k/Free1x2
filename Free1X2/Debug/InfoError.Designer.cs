namespace Free1X2.Debug
{
    partial class InfoError
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoError));
            this.label1 = new System.Windows.Forms.Label();
            this.lblmain = new System.Windows.Forms.Label();
            this.lblSecundario = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnEnviarOnline = new System.Windows.Forms.Button();
            this.txtInfoAdicional = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(12, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(647, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Se ha producido un error inesperado";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblmain
            // 
            this.lblmain.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblmain.Location = new System.Drawing.Point(13, 40);
            this.lblmain.Name = "lblmain";
            this.lblmain.Size = new System.Drawing.Size(646, 130);
            this.lblmain.TabIndex = 1;
            this.lblmain.Text = resources.GetString("lblmain.Text");
            this.lblmain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSecundario
            // 
            this.lblSecundario.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSecundario.Location = new System.Drawing.Point(13, 170);
            this.lblSecundario.Name = "lblSecundario";
            this.lblSecundario.Size = new System.Drawing.Size(646, 33);
            this.lblSecundario.TabIndex = 4;
            this.lblSecundario.Text = "El informe se ha guardado en la carpeta Informes con el nombre:";
            this.lblSecundario.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNombre
            // 
            this.lblNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombre.ForeColor = System.Drawing.Color.Maroon;
            this.lblNombre.Location = new System.Drawing.Point(14, 203);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(645, 42);
            this.lblNombre.TabIndex = 5;
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCerrar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrar.Location = new System.Drawing.Point(146, 370);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(181, 23);
            this.btnCerrar.TabIndex = 9;
            this.btnCerrar.Text = "Cerrar sin Enviar Nada";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnEnviarOnline
            // 
            this.btnEnviarOnline.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnEnviarOnline.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEnviarOnline.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnviarOnline.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEnviarOnline.Location = new System.Drawing.Point(333, 370);
            this.btnEnviarOnline.Name = "btnEnviarOnline";
            this.btnEnviarOnline.Size = new System.Drawing.Size(140, 23);
            this.btnEnviarOnline.TabIndex = 10;
            this.btnEnviarOnline.Text = "Enviar Online";
            this.btnEnviarOnline.UseVisualStyleBackColor = false;
            this.btnEnviarOnline.Click += new System.EventHandler(this.btnEnviarOnline_Click);
            // 
            // txtInfoAdicional
            // 
            this.txtInfoAdicional.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInfoAdicional.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInfoAdicional.Location = new System.Drawing.Point(16, 250);
            this.txtInfoAdicional.Multiline = true;
            this.txtInfoAdicional.Name = "txtInfoAdicional";
            this.txtInfoAdicional.Size = new System.Drawing.Size(458, 115);
            this.txtInfoAdicional.TabIndex = 11;
            this.txtInfoAdicional.Text = "Escribe aquí toda Información importante que quieras añadir y que crees que nos p" +
                "uede ayudar a solucionar el problema";
            this.txtInfoAdicional.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtInfoAdicional_MouseClick);
            // 
            // txtEmail
            // 
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Location = new System.Drawing.Point(480, 274);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(179, 20);
            this.txtEmail.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(480, 250);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 24);
            this.label2.TabIndex = 13;
            this.label2.Text = "Tu email (opcional)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(480, 312);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 30);
            this.label3.TabIndex = 15;
            this.label3.Text = "Nombre de usuario (opcional)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtUser
            // 
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.Location = new System.Drawing.Point(480, 345);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(179, 20);
            this.txtUser.TabIndex = 14;
            // 
            // InfoError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(671, 401);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtInfoAdicional);
            this.Controls.Add(this.btnEnviarOnline);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.lblSecundario);
            this.Controls.Add(this.lblmain);
            this.Controls.Add(this.label1);
            this.Name = "InfoError";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manejador de Errores de Free1X2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblmain;
        private System.Windows.Forms.Label lblSecundario;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnEnviarOnline;
        private System.Windows.Forms.TextBox txtInfoAdicional;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUser;
    }
}