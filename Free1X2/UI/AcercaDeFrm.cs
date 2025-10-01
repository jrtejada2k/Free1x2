// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.


using System;
using System.Windows.Forms;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de AcercaDeFrm.
	/// </summary>
	public class AcercaDeFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLicencia;
		private System.Windows.Forms.LinkLabel linkDescarga;
		private System.Windows.Forms.LinkLabel linkForo;
		private System.Windows.Forms.LinkLabel linkGPL;
		private System.Windows.Forms.LinkLabel linkOccidente;
		private System.Windows.Forms.PictureBox imgLogo;
        private LinkLabel linkManual;
        private LinkLabel linkLabel1;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AcercaDeFrm()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AcercaDeFrm));
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.linkLicencia = new System.Windows.Forms.LinkLabel();
            this.linkDescarga = new System.Windows.Forms.LinkLabel();
            this.linkForo = new System.Windows.Forms.LinkLabel();
            this.linkGPL = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.linkOccidente = new System.Windows.Forms.LinkLabel();
            this.linkManual = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // imgLogo
            // 
            this.imgLogo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.imgLogo.Image = ((System.Drawing.Image)(resources.GetObject("imgLogo.Image")));
            this.imgLogo.Location = new System.Drawing.Point(16, 96);
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.Size = new System.Drawing.Size(114, 114);
            this.imgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgLogo.TabIndex = 0;
            this.imgLogo.TabStop = false;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(152, 80);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(70, 13);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "Versión ....";
            // 
            // linkLicencia
            // 
            this.linkLicencia.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLicencia.Location = new System.Drawing.Point(152, 109);
            this.linkLicencia.Name = "linkLicencia";
            this.linkLicencia.Size = new System.Drawing.Size(152, 16);
            this.linkLicencia.TabIndex = 2;
            this.linkLicencia.TabStop = true;
            this.linkLicencia.Text = "Información de licencia";
            this.linkLicencia.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLicencia_LinkClicked);
            // 
            // linkDescarga
            // 
            this.linkDescarga.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkDescarga.Location = new System.Drawing.Point(152, 154);
            this.linkDescarga.Name = "linkDescarga";
            this.linkDescarga.Size = new System.Drawing.Size(85, 16);
            this.linkDescarga.TabIndex = 3;
            this.linkDescarga.TabStop = true;
            this.linkDescarga.Text = "Descarga";
            this.linkDescarga.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDescarga_LinkClicked);
            // 
            // linkForo
            // 
            this.linkForo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkForo.Location = new System.Drawing.Point(152, 198);
            this.linkForo.Name = "linkForo";
            this.linkForo.Size = new System.Drawing.Size(152, 22);
            this.linkForo.TabIndex = 4;
            this.linkForo.TabStop = true;
            this.linkForo.Text = "Foro sobre el programa";
            this.linkForo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkForo_LinkClicked);
            // 
            // linkGPL
            // 
            this.linkGPL.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkGPL.Location = new System.Drawing.Point(152, 133);
            this.linkGPL.Name = "linkGPL";
            this.linkGPL.Size = new System.Drawing.Size(152, 16);
            this.linkGPL.TabIndex = 5;
            this.linkGPL.TabStop = true;
            this.linkGPL.Text = "Sobre el software libre";
            this.linkGPL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGPL_LinkClicked);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Firebrick;
            this.label1.Location = new System.Drawing.Point(0, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 56);
            this.label1.TabIndex = 6;
            this.label1.Text = "El primer programa de quinielas libre y gratuito";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkOccidente
            // 
            this.linkOccidente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.linkOccidente.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkOccidente.ForeColor = System.Drawing.Color.Firebrick;
            this.linkOccidente.LinkColor = System.Drawing.Color.Firebrick;
            this.linkOccidente.Location = new System.Drawing.Point(56, 296);
            this.linkOccidente.Name = "linkOccidente";
            this.linkOccidente.Size = new System.Drawing.Size(200, 24);
            this.linkOccidente.TabIndex = 8;
            this.linkOccidente.TabStop = true;
            this.linkOccidente.Text = "www.free1X2.com";
            this.linkOccidente.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.linkOccidente.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkOccidente_LinkClicked);
            // 
            // linkManual
            // 
            this.linkManual.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkManual.Location = new System.Drawing.Point(152, 176);
            this.linkManual.Name = "linkManual";
            this.linkManual.Size = new System.Drawing.Size(56, 22);
            this.linkManual.TabIndex = 9;
            this.linkManual.TabStop = true;
            this.linkManual.Text = "Manual";
            this.linkManual.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkManual_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.linkLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.ForeColor = System.Drawing.Color.Firebrick;
            this.linkLabel1.LinkColor = System.Drawing.Color.Firebrick;
            this.linkLabel1.Location = new System.Drawing.Point(123, 242);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(74, 19);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Créditos";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // AcercaDeFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(320, 347);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.linkManual);
            this.Controls.Add(this.linkOccidente);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkGPL);
            this.Controls.Add(this.linkForo);
            this.Controls.Add(this.linkDescarga);
            this.Controls.Add(this.linkLicencia);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.imgLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AcercaDeFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Acerca de";
            this.Load += new System.EventHandler(this.AcercaDeFrm_Load);
            this.Click += new System.EventHandler(this.AcercaDeFrm_Click);
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void AcercaDeFrm_Load(object sender, System.EventArgs e)
		{
			lblVersion.Text="Versión "+Application.ProductVersion + " Rarotonga";
		}

		private void linkLicencia_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(Application.StartupPath+"/Documentacion/licencia.txt");
		}

		private void linkGPL_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
            System.Diagnostics.Process.Start(Application.StartupPath + "/Documentacion/GPL.txt");
		}

		private void linkDescarga_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.free1x2.com/Descargas/Descargas.aspx");
		}

		private void linkForo_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.free1x2.com/foros/index.php");
		}

		private void linkOccidente_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.free1x2.com");
		}

        private void linkManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com/DocWK");
        }

        private void AcercaDeFrm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreditosFrm c = new CreditosFrm();
            c.ShowDialog();
        }

	}
}
