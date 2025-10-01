// created on 25/11/2003 at 22:14
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
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
using System.Windows.Forms;
using System.Collections.Generic;

using Free1X2.MotorCalculo;

namespace Free1X2.UI.Filtros
{
    public class ControlGruposFrm : Form
    {
        private Label label2;
        private TextBox txtFallos;
        private Button btnCancelar;
        private Label label;
        private TextBox txtGrupos;
        private Label lblControlNo;
        private Button btnOK;
        private GroupBox groupBox;
        private Button btnNext;
        private Button btnPrev;
		
        private List<ControlGrupos> controlesGrupos;
        private List<ControlConjuntos> controlesConjuntos;
        private ControladorGrupos ctrlGrupos;
        private Button btnEliminarControl;
        private GroupBox groupBox1;
        private Button btnPrevConj;
        private Button btnNextConj;
        private Button btnEliminarCtrlConjunto;
        private Label lblConjuntoNo;
        private Label label1;
        private Label label3;
        private TextBox textConjuntos;
        private TextBox textFallosConj;
        protected FormulariosHelper formHelper = new FormulariosHelper();

        //cg = 0 es el base con todos los grupos sueltos
        //que no entran en control de fallos
        private int cgPantalla = 1; 

        //cConj = 0 es el base con todos los conjuntos sueltos
        //que no entran en control de fallos
        private int cConjPantalla = 1; 
		
        public ControlGruposFrm( ControladorGrupos ctrlGrupos )
        {			
            InitializeComponent();
            this.ctrlGrupos = ctrlGrupos;
            InicializaDatos();
        }
		
        protected void InicializaDatos()
        {			
            controlesGrupos = ObtenCopiaControlesGrupos( ctrlGrupos );
            controlesConjuntos = ObtenCopiaControlesConjuntos( ctrlGrupos );
            ActualizaDatosPantalla( cgPantalla );	
            ActualizaDatosPantallaConj( cConjPantalla );
        }

        protected List<ControlConjuntos> ObtenCopiaControlesConjuntos(ControladorGrupos ctrlGrupos)
        {
            List<ControlConjuntos> controlesConj_copia = new List<ControlConjuntos>();

            ControlConjuntos cConj_copia;

            foreach( ControlConjuntos cConj in ctrlGrupos.ControlesConjuntos)
            {
                cConj_copia = new ControlConjuntos();

                cConj_copia.PonerCtrlGruposControlados(cConj.ObtenCtrlGruposControladosStr());
                cConj_copia.PonerFallosPermitidos(cConj.ObtenFallosPermitidosStr());

                controlesConj_copia.Add( cConj_copia );
            }

            return controlesConj_copia;
        }

        protected List<ControlGrupos> ObtenCopiaControlesGrupos(ControladorGrupos ctrlGrupos)
        {
            List<ControlGrupos> controlesGrupos_copia = new List<ControlGrupos>();
			
            ControlGrupos cg_copia;
			
            foreach( ControlGrupos cg in ctrlGrupos.ControlesGrupos)
            {
                cg_copia = new ControlGrupos();
								
                cg_copia.PonerGruposControlados( cg.ObtenGruposControlados() );
                cg_copia.PonerFallosPermitidos( cg.ObtenFallosPermitidos() );
                cg_copia.CtrlGrupos = cg.CtrlGrupos;
                cg_copia.UsaControlGrupos = cg.UsaControlGrupos;
				
                controlesGrupos_copia.Add( cg_copia );
            }						
			
            return controlesGrupos_copia;			
        }


        #region controlGrupos
		
        protected void ActualizaDatosPantalla( int noCG )
        {
            //control 0 siempre existe por ser el control base.
            //y no entra en control de grupos
            if( controlesGrupos.Count > 1 )
            {
                ControlGrupos cg = controlesGrupos[ noCG ];
				
                txtGrupos.Text = cg.ObtenGruposControlados();
                txtFallos.Text = cg.ObtenFallosPermitidos();
				
                lblControlNo.Text = (noCG) + "/" + (controlesGrupos.Count-1);
            }
            else
            {
                txtGrupos.Text = "";
                txtFallos.Text = "";
                lblControlNo.Text = "1/1";
            }
        }
		
        protected bool TieneDatosControl()
        {
            bool tieneDatos = true;
			
            if(txtGrupos.Text == "" ||  txtFallos.Text == "")
            {
                tieneDatos = false;			
            }
		
            return tieneDatos;
        }
		
        protected void CambiaCGSelecionado(int noCG)
        {
            //primero guardar datos de pantalla
            GuardarCGActual();
			
            cgPantalla = noCG;	
			
            //crear CG si no existe 			
            if( controlesGrupos.Count < noCG + 1 )
            {
                ControlGrupos cg = new ControlGrupos();
                cg.CtrlGrupos = ctrlGrupos;
                controlesGrupos.Add( cg );		
            }
			
            //activa/desactiva boton "atras" si estamos en la primera columna
            if( noCG == 1 )
            {
                btnPrev.Enabled = false;
            }
            else
            {
                btnPrev.Enabled = true;	
            }
			
            ActualizaDatosPantalla( noCG );
		
        }
		
        protected void GuardarCGActual()
        {
            ControlGrupos cg;
						
            if( cgPantalla  < controlesGrupos.Count)
            {
                if( TieneDatosControl() )
                {
                    cg = controlesGrupos[ cgPantalla ];
                    GuardaDatosCG( cg );
                }
				
            }
            else if( TieneDatosControl() )
            {
                //existen datos en pantalla que necesitan se poner en nuevo control
				
                //crear CP y poner en grupo
                cg = new ControlGrupos();
                cg.CtrlGrupos = ctrlGrupos;
                controlesGrupos.Add( cg );
								
                GuardaDatosCG( cg );	
            }
		
        }
		
        protected void GuardaDatosCG( ControlGrupos cg )
        {
            cg.PonerGruposControlados( txtGrupos.Text );
            cg.PonerFallosPermitidos( txtFallos.Text );		
        }
		
        protected bool NecesitaBorrarUltimaCG()
        {
            bool necesitaBorrar = false;
			
            //controlgrupo 0 es la base. No borrar!
            if(controlesGrupos.Count > 1)
            {
                ControlGrupos cg = controlesGrupos[ controlesGrupos.Count-1 ];
							
                if(cg.ObtenGruposControlados() == "" || cg.ObtenFallosPermitidos() == "")
                {
                    necesitaBorrar = true;			
                }
            }
            return necesitaBorrar;
        }
		
        protected void BorrarCG( int noCG )
        {
            controlesGrupos.RemoveAt( noCG );
        }
		
        protected void GuardarDatos()
        {
            GuardarCGActual();
			
            if( controlesGrupos.Count > 0 )
            {	
                //borrar ultima CG si no contiene datos
                if( NecesitaBorrarUltimaCG() )
                {
                    BorrarCG( controlesGrupos.Count - 1 );				
                }
            }
			
            //buscar grupos "sueltos" que no esten controlados
            //y ponerlos en controlesgrupos[0]
            GuardarGruposLibres();
			
            //poner controles grupos en el controlador de grupos del analizador
            ctrlGrupos.ControlesGrupos = controlesGrupos;
		
        }
		
        protected void GuardarGruposLibres()
        {
            int noGruposTotal = ctrlGrupos.GruposPartidos.Count;
            ControlGrupos cg;

            string strGruposLibres = "0";
			
            //grupo 0 siempre es libre por ser grupo base
            for(int noGrupo = 1; noGrupo < noGruposTotal; noGrupo++)
            {				
                bool contieneGrupo = false;
				
                for(int i = 1; i < controlesGrupos.Count; i++)
                {
                    cg = controlesGrupos[ i ];
                    contieneGrupo = cg.ContieneGrupo( noGrupo );
					
                    if( contieneGrupo)
                    {
                        break;
                    }				
                }
				
                if( !contieneGrupo)
                {
                    strGruposLibres += "," + noGrupo;					
                }					
            }
			
            //poner grupos libres en controlgrupos base.
            cg = controlesGrupos[ 0 ];
            cg.PonerGruposControlados( strGruposLibres );
            cg.UsaControlGrupos = false;
        }


        #endregion

        #region controles Conjuntos
        protected void ActualizaDatosPantallaConj( int noConj )
        {			
            if( controlesConjuntos.Count > 1 )
            {
                ControlConjuntos cConj = controlesConjuntos[ noConj ];
				
                textConjuntos.Text = cConj.ObtenCtrlGruposControladosStr();
                textFallosConj.Text = cConj.ObtenFallosPermitidosStr();
				
                lblConjuntoNo.Text = (noConj) + "/" + (controlesConjuntos.Count-1);
            }
            else
            {
                textConjuntos.Text = "";
                textFallosConj.Text = "";
                lblConjuntoNo.Text = "1/1";
            }
        }


        protected bool TieneDatosControlConj()
        {
            bool tieneDatos = true;
			
            if(textConjuntos.Text == "" || textFallosConj.Text == "")
            {
                tieneDatos = false;			
            }
		
            return tieneDatos;		
        }
		

        protected void CambiaCConjSelecionado(int noConj)
        {
            //primero guardar datos de pantalla
            GuardarCConjActual();
			
            cConjPantalla = noConj;	
			
            //crear CConj si no existe 			
            if( controlesConjuntos.Count < noConj + 1 )
            {
                ControlConjuntos cConj = new ControlConjuntos();
                controlesConjuntos.Add( cConj );		
            }
			
            //activa/desactiva boton "atras" si estamos en la primera columna
            if( noConj == 1 )
            {
                btnPrevConj.Enabled = false;
            }
            else
            {
                btnPrevConj.Enabled = true;	
            }
			
            ActualizaDatosPantallaConj( noConj );
		
        }


        protected void GuardarCConjActual()
        {
            ControlConjuntos cConj;
						
            if( cConjPantalla  < controlesConjuntos.Count)
            {
                if( TieneDatosControlConj() )
                {
                    cConj = controlesConjuntos[ cConjPantalla ];
                    GuardaDatosCConj( cConj );
                }
				
            }
            else if( TieneDatosControlConj() )
            {
                //existen datos en pantalla que se necesitan poner en nuevo control
								
                cConj = new ControlConjuntos();
                controlesConjuntos.Add( cConj );
								
                GuardaDatosCConj( cConj );	
            }		
        }


        protected void BorrarCConj( int noConj )
        {
            controlesConjuntos.RemoveAt( noConj );
        }


        protected void GuardaDatosCConj( ControlConjuntos cConj )
        {
            cConj.PonerCtrlGruposControlados( textConjuntos.Text );
            cConj.PonerFallosPermitidos( textFallosConj.Text );			
        }


        protected bool NecesitaBorrarUltimaCConj()
        {
            bool necesitaBorrar = false;
			
            //controlgrupo 0 es la base. No borrar!
            if(controlesConjuntos.Count > 1)
            {
                ControlConjuntos cConj = controlesConjuntos[ controlesConjuntos.Count-1 ];
							
                if(cConj.ObtenCtrlGruposControladosStr() == "" || cConj.ObtenFallosPermitidosStr() == "")
                {
                    necesitaBorrar = true;			
                }
            }
            return necesitaBorrar;
        }


        protected void GuardarDatosCConj()
        {
            GuardarCConjActual();
			
            if( controlesConjuntos.Count > 0 )
            {	
                //borrar ultima CG si no contiene datos
                if( NecesitaBorrarUltimaCConj() )
                {
                    BorrarCConj( controlesConjuntos.Count - 1 );				
                }
            }
			
            //buscar grupos "sueltos" 
            GuardarGruposLibresCConj();
			
            //poner controles en el controlador de conjuntos del analizador
            ctrlGrupos.ControlesConjuntos = controlesConjuntos;					
        }

        protected void GuardarGruposLibresCConj()
        {
            int noConjuntosTotal = ctrlGrupos.ControlesGrupos.Count;
            ControlConjuntos cConj;

            string strConjuntosLibres = "0";
			
            //conjunto 0 siempre es libre
            for(int noConjunto = 1; noConjunto < noConjuntosTotal; noConjunto++)
            {				
                bool contieneConjunto = false;
				
                for(int i = 1; i < controlesConjuntos.Count; i++)
                {
                    cConj = controlesConjuntos[ i ];
                    contieneConjunto = cConj.ContieneConjunto( noConjunto );
					
                    if( contieneConjunto)
                    {
                        break;
                    }				
                }
				
                if( !contieneConjunto)
                {
                    strConjuntosLibres += "," + noConjunto;					
                }					
            }
			
            //poner grupos libres en controlgrupos base.
            cConj = controlesConjuntos[ 0 ];
            cConj.PonerCtrlGruposControlados( strConjuntosLibres );		
            cConj.PonerFallosPermitidos("0");
        }

        #endregion
			
		
		
        void BtnCancelarClick(object sender, EventArgs e)
        {
            Close();	
        }
		
        void BtnOKClick(object sender, EventArgs e)
        {
            GuardarDatos();
            GuardarDatosCConj();
            Close();
        }
		
        void BtnPrevClick(object sender, EventArgs e)
        {
            CambiaCGSelecionado( cgPantalla - 1 );
        }
		
        void BtnNextClick(object sender, EventArgs e)
        {
            //cambiar a siguiente control solo si actual contiene datos
            if( TieneDatosControl() )
            {
                CambiaCGSelecionado( cgPantalla + 1 );
            }
        }
		
        void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlGruposFrm));
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.lblControlNo = new System.Windows.Forms.Label();
            this.btnEliminarControl = new System.Windows.Forms.Button();
            this.txtFallos = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtGrupos = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textFallosConj = new System.Windows.Forms.TextBox();
            this.textConjuntos = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblConjuntoNo = new System.Windows.Forms.Label();
            this.btnEliminarCtrlConjunto = new System.Windows.Forms.Button();
            this.btnNextConj = new System.Windows.Forms.Button();
            this.btnPrevConj = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.Color.Silver;
            this.btnPrev.Enabled = false;
            this.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrev.Image = ((System.Drawing.Image)(resources.GetObject("btnPrev.Image")));
            this.btnPrev.Location = new System.Drawing.Point(16, 24);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(24, 23);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.BtnPrevClick);
            this.btnPrev.EnabledChanged += new System.EventHandler(this.btnPrev_EnabledChanged);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.LightSalmon;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
            this.btnNext.Location = new System.Drawing.Point(128, 24);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(24, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.BtnNextClick);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.lblControlNo);
            this.groupBox.Controls.Add(this.btnNext);
            this.groupBox.Controls.Add(this.btnPrev);
            this.groupBox.Controls.Add(this.btnEliminarControl);
            this.groupBox.Controls.Add(this.txtFallos);
            this.groupBox.Controls.Add(this.label2);
            this.groupBox.Controls.Add(this.txtGrupos);
            this.groupBox.Controls.Add(this.label);
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.Location = new System.Drawing.Point(16, 8);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(304, 144);
            this.groupBox.TabIndex = 4;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Control Grupos";
            // 
            // lblControlNo
            // 
            this.lblControlNo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControlNo.Location = new System.Drawing.Point(48, 24);
            this.lblControlNo.Name = "lblControlNo";
            this.lblControlNo.Size = new System.Drawing.Size(72, 23);
            this.lblControlNo.TabIndex = 2;
            this.lblControlNo.Text = "0/0";
            this.lblControlNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnEliminarControl
            // 
            this.btnEliminarControl.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminarControl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminarControl.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarControl.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminarControl.Image")));
            this.btnEliminarControl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminarControl.Location = new System.Drawing.Point(168, 24);
            this.btnEliminarControl.Name = "btnEliminarControl";
            this.btnEliminarControl.Size = new System.Drawing.Size(112, 24);
            this.btnEliminarControl.TabIndex = 7;
            this.btnEliminarControl.Text = "Eliminar Actual";
            this.btnEliminarControl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminarControl.UseVisualStyleBackColor = false;
            this.btnEliminarControl.Click += new System.EventHandler(this.btnEliminarControl_Click);
            // 
            // txtFallos
            // 
            this.txtFallos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFallos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFallos.Location = new System.Drawing.Point(80, 104);
            this.txtFallos.Name = "txtFallos";
            this.txtFallos.Size = new System.Drawing.Size(200, 21);
            this.txtFallos.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(26, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "Fallos";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGrupos
            // 
            this.txtGrupos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGrupos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGrupos.Location = new System.Drawing.Point(80, 72);
            this.txtGrupos.Name = "txtGrupos";
            this.txtGrupos.Size = new System.Drawing.Size(200, 21);
            this.txtGrupos.TabIndex = 0;
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(10, 72);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(64, 21);
            this.label.TabIndex = 2;
            this.label.Text = "Grupos";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.LightSalmon;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(144, 320);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 24);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.BtnOKClick);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelar.Image")));
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(232, 320);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(80, 24);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelarClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textFallosConj);
            this.groupBox1.Controls.Add(this.textConjuntos);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblConjuntoNo);
            this.groupBox1.Controls.Add(this.btnEliminarCtrlConjunto);
            this.groupBox1.Controls.Add(this.btnNextConj);
            this.groupBox1.Controls.Add(this.btnPrevConj);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(16, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 144);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Control Conjuntos";
            // 
            // textFallosConj
            // 
            this.textFallosConj.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textFallosConj.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textFallosConj.Location = new System.Drawing.Point(88, 96);
            this.textFallosConj.Name = "textFallosConj";
            this.textFallosConj.Size = new System.Drawing.Size(192, 21);
            this.textFallosConj.TabIndex = 7;
            // 
            // textConjuntos
            // 
            this.textConjuntos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textConjuntos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textConjuntos.Location = new System.Drawing.Point(88, 64);
            this.textConjuntos.Name = "textConjuntos";
            this.textConjuntos.Size = new System.Drawing.Size(192, 21);
            this.textConjuntos.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "Fallos";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "Conjuntos";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblConjuntoNo
            // 
            this.lblConjuntoNo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConjuntoNo.Location = new System.Drawing.Point(48, 24);
            this.lblConjuntoNo.Name = "lblConjuntoNo";
            this.lblConjuntoNo.Size = new System.Drawing.Size(72, 23);
            this.lblConjuntoNo.TabIndex = 3;
            this.lblConjuntoNo.Text = "0/0";
            this.lblConjuntoNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnEliminarCtrlConjunto
            // 
            this.btnEliminarCtrlConjunto.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminarCtrlConjunto.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminarCtrlConjunto.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarCtrlConjunto.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminarCtrlConjunto.Image")));
            this.btnEliminarCtrlConjunto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminarCtrlConjunto.Location = new System.Drawing.Point(168, 24);
            this.btnEliminarCtrlConjunto.Name = "btnEliminarCtrlConjunto";
            this.btnEliminarCtrlConjunto.Size = new System.Drawing.Size(112, 24);
            this.btnEliminarCtrlConjunto.TabIndex = 2;
            this.btnEliminarCtrlConjunto.Text = "Eliminar Actual";
            this.btnEliminarCtrlConjunto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminarCtrlConjunto.UseVisualStyleBackColor = false;
            this.btnEliminarCtrlConjunto.Click += new System.EventHandler(this.btnEliminarCtrlConjunto_Click);
            // 
            // btnNextConj
            // 
            this.btnNextConj.BackColor = System.Drawing.Color.LightSalmon;
            this.btnNextConj.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNextConj.Image = ((System.Drawing.Image)(resources.GetObject("btnNextConj.Image")));
            this.btnNextConj.Location = new System.Drawing.Point(128, 24);
            this.btnNextConj.Name = "btnNextConj";
            this.btnNextConj.Size = new System.Drawing.Size(24, 23);
            this.btnNextConj.TabIndex = 1;
            this.btnNextConj.UseVisualStyleBackColor = false;
            this.btnNextConj.Click += new System.EventHandler(this.btnNextConj_Click);
            // 
            // btnPrevConj
            // 
            this.btnPrevConj.BackColor = System.Drawing.Color.Silver;
            this.btnPrevConj.Enabled = false;
            this.btnPrevConj.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrevConj.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevConj.Image")));
            this.btnPrevConj.Location = new System.Drawing.Point(16, 24);
            this.btnPrevConj.Name = "btnPrevConj";
            this.btnPrevConj.Size = new System.Drawing.Size(24, 23);
            this.btnPrevConj.TabIndex = 0;
            this.btnPrevConj.UseVisualStyleBackColor = false;
            this.btnPrevConj.Click += new System.EventHandler(this.btnPrevConj_Click);
            this.btnPrevConj.EnabledChanged += new System.EventHandler(this.btnPrevConj_EnabledChanged);
            // 
            // ControlGruposFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(336, 358);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlGruposFrm";
            this.Text = "Control de Grupos";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }


        private void btnEliminarControl_Click(object sender, EventArgs e)
        {
            if(cgPantalla == 1)
            {
                //solo borrar si la CP ya esta guardada en memoria
                if(controlesGrupos.Count > 1)
                {
                    BorrarCG( cgPantalla );
                }							
            }
            else
            {
                BorrarCG( cgPantalla );
                cgPantalla = cgPantalla -1;			
            }	
			
            if( cgPantalla == 1 )
            {
                btnPrev.Enabled = false;
            }
            else
            {
                btnPrev.Enabled = true;	
            }

            //CambiaCGSelecionado(cgPantalla);
            ActualizaDatosPantalla( cgPantalla );
        }

        private void btnPrevConj_Click(object sender, EventArgs e)
        {
            CambiaCConjSelecionado( cConjPantalla - 1 );
        }

        private void btnNextConj_Click(object sender, EventArgs e)
        {
            //cambiar a siguiente control solo si actual contiene datos
            if( TieneDatosControlConj() )
            {
                CambiaCConjSelecionado( cConjPantalla + 1 );
            }
        }

        private void btnEliminarCtrlConjunto_Click(object sender, EventArgs e)
        {
            if(cConjPantalla == 1)
            {
                //solo borrar si la CP ya esta guardada en memoria
                if(controlesConjuntos.Count > 1)
                {
                    BorrarCConj( cConjPantalla );
                }							
            }
            else
            {
                BorrarCConj( cConjPantalla );
                cConjPantalla = cConjPantalla -1;			
            }	
			
            if( cConjPantalla == 1 )
            {
                btnPrevConj.Enabled = false;
            }
            else
            {
                btnPrevConj.Enabled = true;	
            }

            ActualizaDatosPantallaConj( cConjPantalla );		
        }

        private void btnPrev_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnPrev);
        }

        private void btnPrevConj_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnPrevConj);
        }	
		
    }
}