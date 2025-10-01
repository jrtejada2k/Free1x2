// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// 
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Free1X2.MotorCalculo;

namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for CopiarCPFrm.
    /// </summary>
    public class CopiarCPFrm : Form
    {
        private Label label1;
        private TextBox txtCP;
        private Button btnCancelar;
        private Button btnCopiar;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;

        private List<ColumnaProbable> grupoCP;
        private Label label2;

        private ColProbablesFrm parentForm;
        private ListBox cmbGrupo;
        private Button btnCrearGrupos;
        private int noGrupoPantalla;
			

        public CopiarCPFrm(List<ColumnaProbable> grupoCP, ColProbablesFrm frm)
        {
            InitializeComponent();
            this.grupoCP = grupoCP;	
            parentForm = frm;

            InicializarGruposDropDown();
			
        }

        protected void InicializarGruposDropDown()
        {
            //accede al formulario principal
            MainForm mainFrm = parentForm.FormPadre;
            //obten numero de grupos
            int noGrupos = mainFrm.MotorCalculo.GruposPartidos.Count;

            for(int i = 0; i < noGrupos; i++)
            {
                if(i==0)
                {
                    cmbGrupo.Items.Add("Boleto Base");
                }
                else
                {
                    cmbGrupo.Items.Add(i);
                }
            }

            noGrupoPantalla = mainFrm.NoGrupoPantalla;

            cmbGrupo.SelectedIndex = noGrupoPantalla;
        }

        protected void CopiarColumnas()
        {
            for(int numGrupo=0; numGrupo<cmbGrupo.Items.Count; numGrupo++)
            {
                //averiguar a que grupo hay que copiar...
                if(cmbGrupo.GetSelected(numGrupo))
                {
                    Grupo grupo = ObtenGrupo( numGrupo);
                    string nombreFiltro = Filtro.ColProbables.ToString();
                    FiltroColProbables filtroCP = (FiltroColProbables)grupo.GetFiltro( nombreFiltro );

                    int[] indexCP = ObtenCP( txtCP.Text );

                    if(IndicesValidos(indexCP))
                    {
                        for(int i = 0; i < indexCP.Length; i++)
                        {
                            ColumnaProbable cp = grupoCP[ indexCP[i]-1 ];

                            ColumnaProbable cp_copia = new ColumnaProbable();
                            cp_copia.Pronosticos = cp.Pronosticos;
                            cp_copia.SetNoAciertos( cp.GetAciertos() );
                            cp_copia.SetNoAciertosSeguidos( cp.GetAciertosSeguidos() );
                            cp_copia.SetNoFallosSeguidos( cp.GetFallosSeguidos() );

                            if( cp.ToleranciaLocalActiva )
                            {
                                cp_copia.SetACTol( cp.GetACTol() );
                                cp_copia.SetACSTol( cp.GetACSTol() );
                                cp_copia.SetFSTol( cp.GetFSTol() );
                                cp_copia.SetTolerancias( cp.GetTolerancias() );
                            }

                            if(noGrupoPantalla == cmbGrupo.SelectedIndex)
                            {
                                grupoCP.Add( cp_copia );
                            }
                            else
                            {
                                filtroCP.ColProbables.Add( cp_copia );
                                filtroCP.ContieneDatos=true;
                                filtroCP.IsActive=true;
								
                            }
                        }

                        parentForm.CambiaCPSelecionado();				
                    }
                }
            }
            Close();		
        }

        protected Grupo ObtenGrupo(int noGrupo)
        {
            return parentForm.FormPadre.MotorCalculo.GruposPartidos[ noGrupo ];					
        }

        protected bool IndicesValidos(int[] indexCP)
        {
            bool indicesValidos = true;

            int indiceMaximo = grupoCP.Count;

            for(int i = 0; i < indexCP.Length; i++)
            {
                if(indexCP[i] > indiceMaximo)
                {
                    indicesValidos = false;
                    break;
                }
            }
			
            return indicesValidos;		
        }

        protected int[] ObtenCP( string valores)
        {
			

            string separador = EncuentraSeparador(valores);

            int[] indexCP;

            if( separador == ",")
            {
                string[] strIndexCP = valores.Split(',');
				
                indexCP = new int[strIndexCP.Length];

                for(int i =0; i < strIndexCP.Length; i++)
                {
                    indexCP[i] = Convert.ToInt32( strIndexCP[i] );
                }
            }
            else if(separador == "-")
            {
                string[] tempIndex = valores.Split('-');
                int indexMin = Convert.ToInt32(tempIndex[0]);
                int indexMax = Convert.ToInt32(tempIndex[1]);

                int noIndexes = (indexMax - indexMin) + 1;

                indexCP = new int[ noIndexes ];

                for(int i = 0; i < indexCP.Length; i++ )
                {
                    indexCP[i] = indexMin + i;
                }
            }
            else
            {
                indexCP = new int[1];
                indexCP[0] = Convert.ToInt32( valores );
            }

            return indexCP;		
        }

        protected string EncuentraSeparador( string values )
        {
            string separador = "";
			
            foreach(char c in values)
            {
                switch( c )
                {
                    case ',':
                        separador = ",";
                        break;
                    case '-':
                        separador = "-";
                        break;
                }
				
                if( separador != "" )
                {
                    //salir del bucle
                    break;
                }
            }
			
            return separador;
        }

        /// <summary>
        /// Clean up any resources being used.
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopiarCPFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtCP = new System.Windows.Forms.TextBox();
            this.btnCopiar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbGrupo = new System.Windows.Forms.ListBox();
            this.btnCrearGrupos = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Columnas:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCP
            // 
            this.txtCP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCP.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCP.Location = new System.Drawing.Point(113, 24);
            this.txtCP.Name = "txtCP";
            this.txtCP.Size = new System.Drawing.Size(128, 21);
            this.txtCP.TabIndex = 1;
            // 
            // btnCopiar
            // 
            this.btnCopiar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCopiar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiar.Image = ((System.Drawing.Image)(resources.GetObject("btnCopiar.Image")));
            this.btnCopiar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopiar.Location = new System.Drawing.Point(47, 232);
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.Size = new System.Drawing.Size(80, 24);
            this.btnCopiar.TabIndex = 2;
            this.btnCopiar.Text = "Copiar";
            this.btnCopiar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCopiar.UseVisualStyleBackColor = false;
            this.btnCopiar.Click += new System.EventHandler(this.btnCopiar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(135, 232);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(80, 24);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 21);
            this.label2.TabIndex = 5;
            this.label2.Text = "A Grupo(s):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbGrupo
            // 
            this.cmbGrupo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cmbGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbGrupo.Location = new System.Drawing.Point(113, 56);
            this.cmbGrupo.Name = "cmbGrupo";
            this.cmbGrupo.ScrollAlwaysVisible = true;
            this.cmbGrupo.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.cmbGrupo.Size = new System.Drawing.Size(128, 119);
            this.cmbGrupo.TabIndex = 6;
            // 
            // btnCrearGrupos
            // 
            this.btnCrearGrupos.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCrearGrupos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCrearGrupos.Image = ((System.Drawing.Image)(resources.GetObject("btnCrearGrupos.Image")));
            this.btnCrearGrupos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCrearGrupos.Location = new System.Drawing.Point(87, 184);
            this.btnCrearGrupos.Name = "btnCrearGrupos";
            this.btnCrearGrupos.Size = new System.Drawing.Size(88, 24);
            this.btnCrearGrupos.TabIndex = 7;
            this.btnCrearGrupos.Text = "Crear grupos";
            this.btnCrearGrupos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCrearGrupos.UseVisualStyleBackColor = false;
            this.btnCrearGrupos.Click += new System.EventHandler(this.btnCrearGrupos_Click);
            // 
            // CopiarCPFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(263, 270);
            this.ControlBox = false;
            this.Controls.Add(this.btnCrearGrupos);
            this.Controls.Add(this.cmbGrupo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnCopiar);
            this.Controls.Add(this.txtCP);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopiarCPFrm";
            this.Text = "Copiar Columnas";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            CopiarColumnas();
        }

        private void btnCrearGrupos_Click(object sender, EventArgs e)
        {
            //accede al formulario principal
            MainForm mainFrm = parentForm.FormPadre;
            CrearGruposFrm f=new CrearGruposFrm();
            f.ShowDialog();
            int gruposNuevos = Convert.ToInt16(f.udNumGrupos.Value);
            int gruposActuales = mainFrm.analizador.GruposPartidos.Count;
            for(int i=0;i<gruposNuevos;i++)
            {
                Grupo grupo = new Grupo();
                grupo.PonerPartidosActivos("1,2,3,4,5,6,7,8,9,10,11,12,13,14");
                mainFrm.analizador.GruposPartidos.AddGrupo( grupo );
                mainFrm.ActualizarGruposPronostico();
                cmbGrupo.Items.Add(gruposActuales.ToString());
                gruposActuales++;
            }
        }

    }
}