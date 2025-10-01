namespace Free1X2.UI
{
    partial class ListadoCondicionesFrm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListadoCondicionesFrm));
            this.treeVwCondiciones = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnExpandir = new System.Windows.Forms.Button();
            this.btnColapsar = new System.Windows.Forms.Button();
            this.Exportar = new System.Windows.Forms.Button();
            this.ExportarHtml = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeVwCondiciones
            // 
            this.treeVwCondiciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeVwCondiciones.BackColor = System.Drawing.Color.Bisque;
            this.treeVwCondiciones.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeVwCondiciones.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeVwCondiciones.ForeColor = System.Drawing.Color.Black;
            this.treeVwCondiciones.ImageIndex = 0;
            this.treeVwCondiciones.ImageList = this.imageList1;
            this.treeVwCondiciones.ItemHeight = 20;
            this.treeVwCondiciones.Location = new System.Drawing.Point(12, 12);
            this.treeVwCondiciones.Name = "treeVwCondiciones";
            this.treeVwCondiciones.SelectedImageIndex = 0;
            this.treeVwCondiciones.Size = new System.Drawing.Size(480, 292);
            this.treeVwCondiciones.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ordenar.gif");
            this.imageList1.Images.SetKeyName(1, "pencil.gif");
            this.imageList1.Images.SetKeyName(2, "filtro.gif");
            this.imageList1.Images.SetKeyName(3, "grupos.gif");
            this.imageList1.Images.SetKeyName(4, "controlGrupos.gif");
            this.imageList1.Images.SetKeyName(5, "IfThen.gif");
            this.imageList1.Images.SetKeyName(6, "abrir.gif");
            this.imageList1.Images.SetKeyName(7, "gruposEquipos.gif");
            this.imageList1.Images.SetKeyName(8, "ok.gif");
            this.imageList1.Images.SetKeyName(9, "VX2.gif");
            this.imageList1.Images.SetKeyName(10, "seguidos.gif");
            this.imageList1.Images.SetKeyName(11, "Dibujos.gif");
            this.imageList1.Images.SetKeyName(12, "CPs.gif");
            this.imageList1.Images.SetKeyName(13, "interrupciones.gif");
            this.imageList1.Images.SetKeyName(14, "pesos.gif");
            this.imageList1.Images.SetKeyName(15, "valoraciones.gif");
            this.imageList1.Images.SetKeyName(16, "distancias.gif");
            this.imageList1.Images.SetKeyName(17, "contactos.gif");
            this.imageList1.Images.SetKeyName(18, "formatos.gif");
            this.imageList1.Images.SetKeyName(19, "formatos123.gif");
            this.imageList1.Images.SetKeyName(20, "simetrias.gif");
            this.imageList1.Images.SetKeyName(21, "gruposEquipos.gif");
            this.imageList1.Images.SetKeyName(22, "exclamacionBlanco.bmp");
            this.imageList1.Images.SetKeyName(23, "diferencias.gif");
            // 
            // btnExpandir
            // 
            this.btnExpandir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExpandir.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnExpandir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExpandir.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExpandir.Location = new System.Drawing.Point(498, 13);
            this.btnExpandir.Name = "btnExpandir";
            this.btnExpandir.Size = new System.Drawing.Size(128, 32);
            this.btnExpandir.TabIndex = 1;
            this.btnExpandir.Text = "Expandir Todo";
            this.btnExpandir.UseVisualStyleBackColor = false;
            this.btnExpandir.Click += new System.EventHandler(this.btnExpandir_Click);
            // 
            // btnColapsar
            // 
            this.btnColapsar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColapsar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnColapsar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnColapsar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColapsar.Location = new System.Drawing.Point(498, 46);
            this.btnColapsar.Name = "btnColapsar";
            this.btnColapsar.Size = new System.Drawing.Size(128, 32);
            this.btnColapsar.TabIndex = 2;
            this.btnColapsar.Text = "Colapsar Todo";
            this.btnColapsar.UseVisualStyleBackColor = false;
            this.btnColapsar.Click += new System.EventHandler(this.btnColapsar_Click);
            // 
            // Exportar
            // 
            this.Exportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Exportar.BackColor = System.Drawing.Color.DarkSalmon;
            this.Exportar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Exportar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Exportar.Location = new System.Drawing.Point(498, 79);
            this.Exportar.Name = "Exportar";
            this.Exportar.Size = new System.Drawing.Size(128, 32);
            this.Exportar.TabIndex = 3;
            this.Exportar.Text = "Exportar a Texto";
            this.Exportar.UseVisualStyleBackColor = false;
            this.Exportar.Click += new System.EventHandler(this.Exportar_Click);
            // 
            // ExportarHtml
            // 
            this.ExportarHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportarHtml.BackColor = System.Drawing.Color.DarkSalmon;
            this.ExportarHtml.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ExportarHtml.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExportarHtml.Location = new System.Drawing.Point(498, 112);
            this.ExportarHtml.Name = "ExportarHtml";
            this.ExportarHtml.Size = new System.Drawing.Size(128, 32);
            this.ExportarHtml.TabIndex = 4;
            this.ExportarHtml.Text = "Exportar a Html";
            this.ExportarHtml.UseVisualStyleBackColor = false;
            this.ExportarHtml.Click += new System.EventHandler(this.ExportarHtml_Click);
            // 
            // ListadoCondicionesFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(632, 316);
            this.Controls.Add(this.ExportarHtml);
            this.Controls.Add(this.Exportar);
            this.Controls.Add(this.btnColapsar);
            this.Controls.Add(this.btnExpandir);
            this.Controls.Add(this.treeVwCondiciones);
            this.Name = "ListadoCondicionesFrm";
            this.Text = "Listado de Condiciones";
            this.Load += new System.EventHandler(this.ListadoCondicionesFrm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeVwCondiciones;
        private System.Windows.Forms.Button btnExpandir;
        private System.Windows.Forms.Button btnColapsar;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button Exportar;
        private System.Windows.Forms.Button ExportarHtml;
    }
}