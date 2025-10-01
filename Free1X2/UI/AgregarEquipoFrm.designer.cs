namespace Free1X2.UI
{
    partial class AgregarEquipoFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtEquipo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rdbPrimera = new System.Windows.Forms.RadioButton();
            this.rdbSegunda = new System.Windows.Forms.RadioButton();
            this.rdbSegundaB = new System.Windows.Forms.RadioButton();
            this.btnNuevoEquipo = new System.Windows.Forms.Button();
            this.rdbInt = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txtEquipo
            // 
            this.txtEquipo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEquipo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEquipo.ForeColor = System.Drawing.Color.Maroon;
            this.txtEquipo.Location = new System.Drawing.Point(80, 6);
            this.txtEquipo.Name = "txtEquipo";
            this.txtEquipo.Size = new System.Drawing.Size(107, 21);
            this.txtEquipo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rdbPrimera
            // 
            this.rdbPrimera.AutoSize = true;
            this.rdbPrimera.Checked = true;
            this.rdbPrimera.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbPrimera.Location = new System.Drawing.Point(16, 32);
            this.rdbPrimera.Name = "rdbPrimera";
            this.rdbPrimera.Size = new System.Drawing.Size(38, 17);
            this.rdbPrimera.TabIndex = 2;
            this.rdbPrimera.TabStop = true;
            this.rdbPrimera.Text = "1ª";
            this.rdbPrimera.UseVisualStyleBackColor = true;
            // 
            // rdbSegunda
            // 
            this.rdbSegunda.AutoSize = true;
            this.rdbSegunda.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbSegunda.Location = new System.Drawing.Point(58, 32);
            this.rdbSegunda.Name = "rdbSegunda";
            this.rdbSegunda.Size = new System.Drawing.Size(38, 17);
            this.rdbSegunda.TabIndex = 3;
            this.rdbSegunda.Text = "2ª";
            this.rdbSegunda.UseVisualStyleBackColor = true;
            // 
            // rdbSegundaB
            // 
            this.rdbSegundaB.AutoSize = true;
            this.rdbSegundaB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbSegundaB.Location = new System.Drawing.Point(100, 32);
            this.rdbSegundaB.Name = "rdbSegundaB";
            this.rdbSegundaB.Size = new System.Drawing.Size(46, 17);
            this.rdbSegundaB.TabIndex = 4;
            this.rdbSegundaB.Text = "2ªB";
            this.rdbSegundaB.UseVisualStyleBackColor = true;
            // 
            // btnNuevoEquipo
            // 
            this.btnNuevoEquipo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNuevoEquipo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevoEquipo.Location = new System.Drawing.Point(66, 55);
            this.btnNuevoEquipo.Name = "btnNuevoEquipo";
            this.btnNuevoEquipo.Size = new System.Drawing.Size(74, 23);
            this.btnNuevoEquipo.TabIndex = 12;
            this.btnNuevoEquipo.Text = "Añadir";
            this.btnNuevoEquipo.UseVisualStyleBackColor = true;
            this.btnNuevoEquipo.Click += new System.EventHandler(this.btnNuevoEquipo_Click);
            // 
            // rdbInt
            // 
            this.rdbInt.AutoSize = true;
            this.rdbInt.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdbInt.Location = new System.Drawing.Point(150, 32);
            this.rdbInt.Name = "rdbInt";
            this.rdbInt.Size = new System.Drawing.Size(41, 17);
            this.rdbInt.TabIndex = 13;
            this.rdbInt.Text = "Int";
            this.rdbInt.UseVisualStyleBackColor = true;
            // 
            // AgregarEquipoFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(206, 86);
            this.Controls.Add(this.rdbInt);
            this.Controls.Add(this.btnNuevoEquipo);
            this.Controls.Add(this.rdbSegundaB);
            this.Controls.Add(this.rdbSegunda);
            this.Controls.Add(this.rdbPrimera);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEquipo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AgregarEquipoFrm";
            this.Text = "Añadir Equipo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEquipo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdbPrimera;
        private System.Windows.Forms.RadioButton rdbSegunda;
        private System.Windows.Forms.RadioButton rdbSegundaB;
        private System.Windows.Forms.Button btnNuevoEquipo;
        private System.Windows.Forms.RadioButton rdbInt;
    }
}