namespace Free1X2.UI.Controls
{
    partial class ctrlAyuda
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlAyuda));
            this.pctAyuda = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pctAyuda)).BeginInit();
            this.SuspendLayout();
            // 
            // pctAyuda
            // 
            this.pctAyuda.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pctAyuda.Image = ((System.Drawing.Image)(resources.GetObject("pctAyuda.Image")));
            this.pctAyuda.Location = new System.Drawing.Point(-1, -2);
            this.pctAyuda.Name = "pctAyuda";
            this.pctAyuda.Size = new System.Drawing.Size(15, 15);
            this.pctAyuda.TabIndex = 170;
            this.pctAyuda.TabStop = false;
            this.pctAyuda.MouseHover += new System.EventHandler(this.pctAyuda_MouseHover);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 50;
            this.toolTip1.AutoPopDelay = 50000;
            this.toolTip1.BackColor = System.Drawing.Color.Bisque;
            this.toolTip1.InitialDelay = 50;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.OwnerDraw = true;
            this.toolTip1.ReshowDelay = 10;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Ayuda Específica";
            // 
            // ctrlAyuda
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pctAyuda);
            this.Name = "ctrlAyuda";
            this.Size = new System.Drawing.Size(16, 16);
            this.Load += new System.EventHandler(this.ctrlAyuda_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pctAyuda)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        protected System.Windows.Forms.PictureBox pctAyuda;
    }
}
