// created on 24/01/2004 at 15:59
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    public class PesosNumFrm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label label;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdDoses;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdVariantes;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdGlobal;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdUnos;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdGlobalTol;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdUnosTol;
        private System.Windows.Forms.Label opt4;
        private System.Windows.Forms.Label opt5;
        private System.Windows.Forms.Label opt2;
        private System.Windows.Forms.Label opt3;
        private System.Windows.Forms.Label opt0;
        private System.Windows.Forms.Label opt1;
        private System.Windows.Forms.Label label9;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdEquis;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdDosesTol;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdVariantesTol;
        private Free1X2.UI.Controls.OptionNumsHoriz0_9 stdEquisTol;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
        private Free1X2.UI.Controls.MenuCondiciones menuCondiciones1;
        private FiltroPesosNumericos filtro;
        private Free1X2.UI.Controls.ctrlAyuda ctrlAyuda1;
        private Label label1;
        private Label lblFig311;
        private Label lblFig32;
        private Label lblFig2111;
        private Label lblFig221;
        private Label lblFig11111;
        private Grupo grupo;
        private Label label33;
        protected List<long> figuras = new List<long>();
        FormulariosHelper fHelper = new FormulariosHelper();

        public PesosNumFrm(Grupo grupo)
        {
            InitializeComponent();
            this.grupo = grupo;
            string nombreFiltro = Filtro.PesosNumericos.ToString();
            filtro = (FiltroPesosNumericos)grupo.GetFiltro( nombreFiltro );
            MarcarValores();
            compruebaPegar();
            this.ctrlAyuda1.TextoAyuda = "El Peso Numérico de una columna\nes una representación numérica de la columna.\nTambién se puede expresar el Peso Numérico\nde Variantes, 1, X y 2";
            fHelper.Traducir(this);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
        protected void MarcarValores()
        {
            stdGlobal.Valores = filtro.GetPNGlobal();
            stdVariantes.Valores = filtro.GetPNVariantes();
            stdUnos.Valores = filtro.GetPNUnos();
            stdEquis.Valores = filtro.GetPNEquis();
            stdDoses.Valores = filtro.GetPNDoses();
			
            stdGlobalTol.Valores = filtro.GetPNGlobalTol();
            stdVariantesTol.Valores = filtro.GetPNVariantesTol();
            stdUnosTol.Valores = filtro.GetPNUnosTol();
            stdEquisTol.Valores = filtro.GetPNEquisTol();
            stdDosesTol.Valores = filtro.GetPNDosesTol();
			
            this.ToleranciaValores = filtro.GetTolerancias();
            this.figuras = filtro.Figuras;

            MarcarFigurasSeleccionadas();
        }

        protected void MarcarFigurasSeleccionadas()
        {
            long fig = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText("3-2");
            if (filtro.Figuras.Contains(fig)) this.lblFig32.BackColor = System.Drawing.Color.LightGreen;
            else this.lblFig32.BackColor = System.Drawing.Color.Wheat;

            fig = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText("3-1-1");
            if (filtro.Figuras.Contains(fig)) this.lblFig311.BackColor = System.Drawing.Color.LightGreen;
            else this.lblFig311.BackColor = System.Drawing.Color.Wheat;
            
            fig = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText("2-2-1");
            if (filtro.Figuras.Contains(fig)) this.lblFig221.BackColor = System.Drawing.Color.LightGreen;
            else this.lblFig221.BackColor = System.Drawing.Color.Wheat;
            
            fig = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText("2-1-1-1");
            if (filtro.Figuras.Contains(fig)) this.lblFig2111.BackColor = System.Drawing.Color.LightGreen;
            else this.lblFig2111.BackColor = System.Drawing.Color.Wheat;

            fig = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText("1-1-1-1-1");
            if (filtro.Figuras.Contains(fig)) this.lblFig11111.BackColor = System.Drawing.Color.LightGreen;
            else this.lblFig11111.BackColor = System.Drawing.Color.Wheat;
        }

        protected void ActualizarDatos()
        {
            string todosValores = fHelper.ObtenerTodosValores(); 
            filtro.ReinicializaValores();
            if( NecesitaGuardarDatos() )
            {
                if(filtro.ContieneDatos == false)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtro.IsActive = true;				
                }
				
                filtro.ContieneDatos = true;
				
                if( stdGlobal.Valores != "" )
                {
                    filtro.SetPNGlobal( stdGlobal.Valores );
                }
                else
                {
                    filtro.SetPNGlobal( todosValores );
                }
				
                if( stdVariantes.Valores != "" )
                {
                    filtro.SetPNVar( stdVariantes.Valores );
                }
                else
                {
                    filtro.SetPNVar( todosValores );
                }
				
                if( stdUnos.Valores != "" )
                {
                    filtro.SetPNUnos( stdUnos.Valores );
                }
                else
                {
                    filtro.SetPNUnos( todosValores );
                }
				
                if( stdEquis.Valores != "" )
                {
                    filtro.SetPNEquis( stdEquis.Valores );
                }
                else
                {
                    filtro.SetPNEquis( todosValores );
                }
				
                if( stdDoses.Valores != "" )
                {
                    filtro.SetPNDoses( stdDoses.Valores );
                }
                else
                {
                    filtro.SetPNDoses( todosValores );
                }
				
                //guardar tolerancias
                if( NecesitaGuardarDatosTol() )
                {
                    if( stdGlobalTol.Valores != "" )
                    {
                        filtro.SetPNGlobalTol( stdGlobalTol.Valores );
                    }
                    else
                    {
                        filtro.SetPNGlobalTol( "" );
                    }
					
                    if( stdVariantesTol.Valores != "" )
                    {
                        filtro.SetPNVarTol( stdVariantesTol.Valores );
                    }
                    else
                    {
                        filtro.SetPNVarTol( "" );
                    }
					
                    if( stdUnosTol.Valores != "" )
                    {
                        filtro.SetPNUnosTol( stdUnosTol.Valores );
                    }
                    else
                    {
                        filtro.SetPNUnosTol( "" );
                    }
					
                    if( stdEquisTol.Valores != "" )
                    {
                        filtro.SetPNEquisTol( stdEquisTol.Valores );
                    }
                    else
                    {
                        filtro.SetPNEquisTol( "" );
                    }				
					
                    if( stdDosesTol.Valores != "" )
                    {
                        filtro.SetPNDosesTol( stdDosesTol.Valores );
                    }
                    else
                    {
                        filtro.SetPNDosesTol( "" );
                    }
					
                    if( this.ToleranciaValores != "" )
                    {
                        filtro.PonerTolerancia( this.ToleranciaValores );
                    }
                    else
                    {
                        filtro.PonerTolerancia( "0" );
                    }	

                }
                else //no hay tolerancias marcadas
                {
                    filtro.PonerTolerancia( "0" );				
                }
				
                //Guardar las figuras
                filtro.Figuras = this.figuras;
            }
            else
            {
                filtro.IsActive = false;
                filtro.ContieneDatos = false;
            }
        }
        protected FiltroPesosNumericos ObtenerFiltroTemporal()
        {
            FiltroPesosNumericos filtroTemp = new FiltroPesosNumericos();
            string todosValores = fHelper.ObtenerTodosValores(); 
            filtroTemp.ReinicializaValores();
            if (NecesitaGuardarDatos())
            {
                if (filtroTemp.ContieneDatos == false)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtroTemp.IsActive = true;
                }

                filtroTemp.ContieneDatos = true;

                if (stdGlobal.Valores != "")
                {
                    filtroTemp.SetPNGlobal(stdGlobal.Valores);
                }
                else
                {
                    filtroTemp.SetPNGlobal(todosValores);
                }

                if (stdVariantes.Valores != "")
                {
                    filtroTemp.SetPNVar(stdVariantes.Valores);
                }
                else
                {
                    filtroTemp.SetPNVar(todosValores);
                }

                if (stdUnos.Valores != "")
                {
                    filtroTemp.SetPNUnos(stdUnos.Valores);
                }
                else
                {
                    filtroTemp.SetPNUnos(todosValores);
                }

                if (stdEquis.Valores != "")
                {
                    filtroTemp.SetPNEquis(stdEquis.Valores);
                }
                else
                {
                    filtroTemp.SetPNEquis(todosValores);
                }

                if (stdDoses.Valores != "")
                {
                    filtroTemp.SetPNDoses(stdDoses.Valores);
                }
                else
                {
                    filtroTemp.SetPNDoses(todosValores);
                }

                //guardar tolerancias
                if (NecesitaGuardarDatosTol())
                {
                    if (stdGlobalTol.Valores != "")
                    {
                        filtroTemp.SetPNGlobalTol(stdGlobalTol.Valores);
                    }
                    else
                    {
                        filtroTemp.SetPNGlobalTol("");
                    }

                    if (stdVariantesTol.Valores != "")
                    {
                        filtroTemp.SetPNVarTol(stdVariantesTol.Valores);
                    }
                    else
                    {
                        filtroTemp.SetPNVarTol("");
                    }

                    if (stdUnosTol.Valores != "")
                    {
                        filtroTemp.SetPNUnosTol(stdUnosTol.Valores);
                    }
                    else
                    {
                        filtroTemp.SetPNUnosTol("");
                    }

                    if (stdEquisTol.Valores != "")
                    {
                        filtroTemp.SetPNEquisTol(stdEquisTol.Valores);
                    }
                    else
                    {
                        filtroTemp.SetPNEquisTol("");
                    }

                    if (stdDosesTol.Valores != "")
                    {
                        filtroTemp.SetPNDosesTol(stdDosesTol.Valores);
                    }
                    else
                    {
                        filtroTemp.SetPNDosesTol("");
                    }

                    if (this.ToleranciaValores != "")
                    {
                        filtroTemp.PonerTolerancia(this.ToleranciaValores);
                    }
                    else
                    {
                        filtroTemp.PonerTolerancia("0");
                    }

                }
                else //no hay tolerancias marcadas
                {
                    filtroTemp.PonerTolerancia("0");
                }

                //Guardar las figuras
                filtroTemp.Figuras = this.figuras;
            }
            else
            {
                filtroTemp.IsActive = false;
                filtroTemp.ContieneDatos = false;
            }
            return filtroTemp;
        }

        protected bool NecesitaGuardarDatos()
        {
            bool necesitaGuardar = true;
			
            if(stdGlobal.Valores == "" && stdVariantes.Valores == "" && stdUnos.Valores == ""
               && stdEquis.Valores == "" && stdDoses.Valores == "" && this.figuras.Count == 0)
            {
                necesitaGuardar = false;
            }
            return necesitaGuardar;
        }
		
        protected bool NecesitaGuardarDatosTol()
        {
            bool necesitaGuardar = true;//	   
			
            if(stdGlobalTol.Valores == "" && stdVariantesTol.Valores == "" 
               && stdUnosTol.Valores == "" && stdEquisTol.Valores == "" 
               && stdDosesTol.Valores == "" )
            {
                necesitaGuardar = false;
            }
			
            return necesitaGuardar;
        }
		
        protected string ToleranciaValores
        {
            get
            {
                string valores = "";
				
                if(opt0.BackColor == System.Drawing.Color.LightGreen)
                {
                    valores = "0";
                }
				
                if(opt1.BackColor == System.Drawing.Color.LightGreen)
                {
                    if( !valores.Equals("") )
                    {
                        valores += ",";
                    }
                    valores += "1";
                }
                if(opt2.BackColor == System.Drawing.Color.LightGreen)
                {
                    if( !valores.Equals("") )
                    {
                        valores += ",";
                    }
                    valores += "2";
                }
                if(opt3.BackColor == System.Drawing.Color.LightGreen)
                {
                    if( !valores.Equals("") )
                    {
                        valores += ",";
                    }
                    valores += "3";
                }
                if(opt4.BackColor == System.Drawing.Color.LightGreen)
                {
                    if( !valores.Equals("") )
                    {
                        valores += ",";
                    }
                    valores += "4";
                }
                if(opt5.BackColor == System.Drawing.Color.LightGreen)
                {
                    if( !valores.Equals("") )
                    {
                        valores += ",";
                    }
                    valores += "5";
                }
				
                return valores;			
            }
            set
            {
                string valores = value;
                string[] valArray = valores.Split(',');
				
                foreach(string val in valArray)
                {
                    switch( val )
                    {
                        case "0":
                            opt0.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "1":
                            opt1.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "2":
                            opt2.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "3":
                            opt3.BackColor = System.Drawing.Color.LightGreen;
                            break;	
                        case "4":
                            opt4.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "5":
                            opt5.BackColor = System.Drawing.Color.LightGreen;
                            break;
                    }				
                }			
			
            }		
        } 
		
        void InitializeComponent() {
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.opt1 = new System.Windows.Forms.Label();
            this.opt0 = new System.Windows.Forms.Label();
            this.opt3 = new System.Windows.Forms.Label();
            this.opt2 = new System.Windows.Forms.Label();
            this.opt5 = new System.Windows.Forms.Label();
            this.opt4 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFig311 = new System.Windows.Forms.Label();
            this.lblFig32 = new System.Windows.Forms.Label();
            this.lblFig2111 = new System.Windows.Forms.Label();
            this.lblFig221 = new System.Windows.Forms.Label();
            this.lblFig11111 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.stdGlobal = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdUnos = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdVariantes = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdEquis = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdDoses = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdGlobalTol = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdVariantesTol = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdUnosTol = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdEquisTol = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.stdDosesTol = new Free1X2.UI.Controls.OptionNumsHoriz0_9();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(40, 149);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 16);
            this.label8.TabIndex = 14;
            this.label8.Text = "Var";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(48, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "X";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(48, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "2";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(32, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "Tolerancias";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(32, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "Global";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(40, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Var";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(48, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "1";
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(32, 232);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(111, 23);
            this.label14.TabIndex = 24;
            this.label14.Text = "Núm Tolerancias:";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(48, 200);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 16);
            this.label11.TabIndex = 17;
            this.label11.Text = "2";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(48, 183);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 16);
            this.label10.TabIndex = 16;
            this.label10.Text = "X";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(48, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(16, 16);
            this.label9.TabIndex = 15;
            this.label9.Text = "1";
            // 
            // opt1
            // 
            this.opt1.BackColor = System.Drawing.Color.Wheat;
            this.opt1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt1.Location = new System.Drawing.Point(166, 232);
            this.opt1.Name = "opt1";
            this.opt1.Size = new System.Drawing.Size(16, 16);
            this.opt1.TabIndex = 19;
            this.opt1.Text = "1";
            this.opt1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt1.Click += new System.EventHandler(this.Opt1Click);
            // 
            // opt0
            // 
            this.opt0.BackColor = System.Drawing.Color.Wheat;
            this.opt0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt0.Location = new System.Drawing.Point(149, 232);
            this.opt0.Name = "opt0";
            this.opt0.Size = new System.Drawing.Size(16, 16);
            this.opt0.TabIndex = 18;
            this.opt0.Text = "0";
            this.opt0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt0.Click += new System.EventHandler(this.Opt0Click);
            // 
            // opt3
            // 
            this.opt3.BackColor = System.Drawing.Color.Wheat;
            this.opt3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt3.Location = new System.Drawing.Point(200, 232);
            this.opt3.Name = "opt3";
            this.opt3.Size = new System.Drawing.Size(16, 16);
            this.opt3.TabIndex = 21;
            this.opt3.Text = "3";
            this.opt3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt3.Click += new System.EventHandler(this.Opt3Click);
            // 
            // opt2
            // 
            this.opt2.BackColor = System.Drawing.Color.Wheat;
            this.opt2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt2.Location = new System.Drawing.Point(183, 232);
            this.opt2.Name = "opt2";
            this.opt2.Size = new System.Drawing.Size(16, 16);
            this.opt2.TabIndex = 20;
            this.opt2.Text = "2";
            this.opt2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt2.Click += new System.EventHandler(this.Opt2Click);
            // 
            // opt5
            // 
            this.opt5.BackColor = System.Drawing.Color.Wheat;
            this.opt5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt5.Location = new System.Drawing.Point(234, 232);
            this.opt5.Name = "opt5";
            this.opt5.Size = new System.Drawing.Size(16, 16);
            this.opt5.TabIndex = 23;
            this.opt5.Text = "5";
            this.opt5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt5.Click += new System.EventHandler(this.Opt5Click);
            // 
            // opt4
            // 
            this.opt4.BackColor = System.Drawing.Color.Wheat;
            this.opt4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt4.Location = new System.Drawing.Point(217, 232);
            this.opt4.Name = "opt4";
            this.opt4.Size = new System.Drawing.Size(16, 16);
            this.opt4.TabIndex = 22;
            this.opt4.Text = "4";
            this.opt4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt4.Click += new System.EventHandler(this.Opt4Click);
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(32, 16);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(48, 16);
            this.label.TabIndex = 2;
            this.label.Text = "Global";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 270);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 48);
            this.panel1.TabIndex = 37;
            // 
            // menuCondiciones1
            // 
            this.menuCondiciones1.Alineacion = Free1X2.alignment.Horizontal;
            this.menuCondiciones1.AutoSize = true;
            this.menuCondiciones1.BackColor = System.Drawing.Color.Bisque;
            this.menuCondiciones1.BotonAbrir = true;
            this.menuCondiciones1.BotonAbrirEnabled = true;
            this.menuCondiciones1.BotonBorrar = true;
            this.menuCondiciones1.BotonBorrarEnabled = true;
            this.menuCondiciones1.BotonCancelar = true;
            this.menuCondiciones1.BotonCancelarEnabled = true;
            this.menuCondiciones1.BotonCopiar = true;
            this.menuCondiciones1.BotonCopiarEnabled = true;
            this.menuCondiciones1.BotonEstadisticas = true;
            this.menuCondiciones1.BotonEstadisticasEnabled = true;
            this.menuCondiciones1.BotonGuardar = true;
            this.menuCondiciones1.BotonGuardarEnabled = true;
            this.menuCondiciones1.BotonOk = true;
            this.menuCondiciones1.BotonOkEnabled = true;
            this.menuCondiciones1.BotonPegar = true;
            this.menuCondiciones1.BotonPegarEnabled = false;
            this.menuCondiciones1.Location = new System.Drawing.Point(65, 9);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 3;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(271, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 16);
            this.label1.TabIndex = 39;
            this.label1.Text = "Figuras";
            // 
            // lblFig311
            // 
            this.lblFig311.BackColor = System.Drawing.Color.Wheat;
            this.lblFig311.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFig311.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFig311.Location = new System.Drawing.Point(271, 51);
            this.lblFig311.Name = "lblFig311";
            this.lblFig311.Size = new System.Drawing.Size(66, 16);
            this.lblFig311.TabIndex = 43;
            this.lblFig311.Text = "3-1-1";
            this.lblFig311.Click += new System.EventHandler(this.lblFig311_Click);
            // 
            // lblFig32
            // 
            this.lblFig32.BackColor = System.Drawing.Color.Wheat;
            this.lblFig32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFig32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFig32.Location = new System.Drawing.Point(271, 34);
            this.lblFig32.Name = "lblFig32";
            this.lblFig32.Size = new System.Drawing.Size(66, 16);
            this.lblFig32.TabIndex = 42;
            this.lblFig32.Text = "3-2";
            this.lblFig32.Click += new System.EventHandler(this.lblFig32_Click);
            // 
            // lblFig2111
            // 
            this.lblFig2111.BackColor = System.Drawing.Color.Wheat;
            this.lblFig2111.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFig2111.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFig2111.Location = new System.Drawing.Point(271, 85);
            this.lblFig2111.Name = "lblFig2111";
            this.lblFig2111.Size = new System.Drawing.Size(66, 16);
            this.lblFig2111.TabIndex = 45;
            this.lblFig2111.Text = "2-1-1-1";
            this.lblFig2111.Click += new System.EventHandler(this.lblFig2111_Click);
            // 
            // lblFig221
            // 
            this.lblFig221.BackColor = System.Drawing.Color.Wheat;
            this.lblFig221.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFig221.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFig221.Location = new System.Drawing.Point(271, 68);
            this.lblFig221.Name = "lblFig221";
            this.lblFig221.Size = new System.Drawing.Size(66, 16);
            this.lblFig221.TabIndex = 44;
            this.lblFig221.Text = "2-2-1";
            this.lblFig221.Click += new System.EventHandler(this.lblFig221_Click);
            // 
            // lblFig11111
            // 
            this.lblFig11111.BackColor = System.Drawing.Color.Wheat;
            this.lblFig11111.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFig11111.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFig11111.Location = new System.Drawing.Point(271, 102);
            this.lblFig11111.Name = "lblFig11111";
            this.lblFig11111.Size = new System.Drawing.Size(66, 16);
            this.lblFig11111.TabIndex = 46;
            this.lblFig11111.Text = "1-1-1-1-1";
            this.lblFig11111.Click += new System.EventHandler(this.lblFig11111_Click);
            // 
            // label33
            // 
            this.label33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(254, 132);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(95, 103);
            this.label33.TabIndex = 47;
            this.label33.Text = "No se incluyen las Figuras 5 y 4-1, que darían 0 columnas";
            this.label33.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(350, 1);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 38;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // stdGlobal
            // 
            this.stdGlobal.BackColor = System.Drawing.Color.Wheat;
            this.stdGlobal.Location = new System.Drawing.Point(80, 16);
            this.stdGlobal.Name = "stdGlobal";
            this.stdGlobal.Size = new System.Drawing.Size(170, 16);
            this.stdGlobal.TabIndex = 0;
            this.stdGlobal.Valores = "";
            // 
            // stdUnos
            // 
            this.stdUnos.BackColor = System.Drawing.Color.Wheat;
            this.stdUnos.Location = new System.Drawing.Point(80, 50);
            this.stdUnos.Name = "stdUnos";
            this.stdUnos.Size = new System.Drawing.Size(170, 16);
            this.stdUnos.TabIndex = 0;
            this.stdUnos.Valores = "";
            // 
            // stdVariantes
            // 
            this.stdVariantes.BackColor = System.Drawing.Color.Wheat;
            this.stdVariantes.Location = new System.Drawing.Point(80, 33);
            this.stdVariantes.Name = "stdVariantes";
            this.stdVariantes.Size = new System.Drawing.Size(170, 16);
            this.stdVariantes.TabIndex = 0;
            this.stdVariantes.Valores = "";
            // 
            // stdEquis
            // 
            this.stdEquis.BackColor = System.Drawing.Color.Wheat;
            this.stdEquis.Location = new System.Drawing.Point(80, 67);
            this.stdEquis.Name = "stdEquis";
            this.stdEquis.Size = new System.Drawing.Size(170, 16);
            this.stdEquis.TabIndex = 0;
            this.stdEquis.Valores = "";
            // 
            // stdDoses
            // 
            this.stdDoses.BackColor = System.Drawing.Color.Wheat;
            this.stdDoses.Location = new System.Drawing.Point(80, 84);
            this.stdDoses.Name = "stdDoses";
            this.stdDoses.Size = new System.Drawing.Size(170, 16);
            this.stdDoses.TabIndex = 0;
            this.stdDoses.Valores = "";
            // 
            // stdGlobalTol
            // 
            this.stdGlobalTol.BackColor = System.Drawing.Color.Wheat;
            this.stdGlobalTol.Location = new System.Drawing.Point(80, 132);
            this.stdGlobalTol.Name = "stdGlobalTol";
            this.stdGlobalTol.Size = new System.Drawing.Size(170, 16);
            this.stdGlobalTol.TabIndex = 7;
            this.stdGlobalTol.Valores = "";
            // 
            // stdVariantesTol
            // 
            this.stdVariantesTol.BackColor = System.Drawing.Color.Wheat;
            this.stdVariantesTol.Location = new System.Drawing.Point(80, 149);
            this.stdVariantesTol.Name = "stdVariantesTol";
            this.stdVariantesTol.Size = new System.Drawing.Size(170, 16);
            this.stdVariantesTol.TabIndex = 8;
            this.stdVariantesTol.Valores = "";
            // 
            // stdUnosTol
            // 
            this.stdUnosTol.BackColor = System.Drawing.Color.Wheat;
            this.stdUnosTol.Location = new System.Drawing.Point(80, 166);
            this.stdUnosTol.Name = "stdUnosTol";
            this.stdUnosTol.Size = new System.Drawing.Size(170, 16);
            this.stdUnosTol.TabIndex = 9;
            this.stdUnosTol.Valores = "";
            // 
            // stdEquisTol
            // 
            this.stdEquisTol.BackColor = System.Drawing.Color.Wheat;
            this.stdEquisTol.Location = new System.Drawing.Point(80, 183);
            this.stdEquisTol.Name = "stdEquisTol";
            this.stdEquisTol.Size = new System.Drawing.Size(170, 16);
            this.stdEquisTol.TabIndex = 10;
            this.stdEquisTol.Valores = "";
            // 
            // stdDosesTol
            // 
            this.stdDosesTol.BackColor = System.Drawing.Color.Wheat;
            this.stdDosesTol.Location = new System.Drawing.Point(80, 200);
            this.stdDosesTol.Name = "stdDosesTol";
            this.stdDosesTol.Size = new System.Drawing.Size(170, 16);
            this.stdDosesTol.TabIndex = 11;
            this.stdDosesTol.Valores = "";
            // 
            // PesosNumFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(375, 318);
            this.ControlBox = false;
            this.Controls.Add(this.label33);
            this.Controls.Add(this.lblFig11111);
            this.Controls.Add(this.lblFig2111);
            this.Controls.Add(this.lblFig221);
            this.Controls.Add(this.lblFig311);
            this.Controls.Add(this.lblFig32);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.opt5);
            this.Controls.Add(this.opt4);
            this.Controls.Add(this.opt3);
            this.Controls.Add(this.opt2);
            this.Controls.Add(this.opt1);
            this.Controls.Add(this.opt0);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.stdGlobal);
            this.Controls.Add(this.stdUnos);
            this.Controls.Add(this.stdVariantes);
            this.Controls.Add(this.stdEquis);
            this.Controls.Add(this.stdDoses);
            this.Controls.Add(this.stdGlobalTol);
            this.Controls.Add(this.stdVariantesTol);
            this.Controls.Add(this.stdUnosTol);
            this.Controls.Add(this.stdEquisTol);
            this.Controls.Add(this.stdDosesTol);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PesosNumFrm";
            this.Text = "Pesos Numéricos";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        void Opt0Click(object sender, System.EventArgs e)
        {
            if( this.opt0.BackColor == System.Drawing.Color.Wheat )
            {
                this.opt0.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                this.opt0.BackColor = System.Drawing.Color.Wheat;
            }
        }
		
        void Opt1Click(object sender, System.EventArgs e)
        {
            if( this.opt1.BackColor == System.Drawing.Color.Wheat )
            {
                this.opt1.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                this.opt1.BackColor = System.Drawing.Color.Wheat;
            }
        }
		
        void Opt2Click(object sender, System.EventArgs e)
        {
            if( this.opt2.BackColor == System.Drawing.Color.Wheat )
            {
                this.opt2.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                this.opt2.BackColor = System.Drawing.Color.Wheat;
            }
        }
		
        void Opt3Click(object sender, System.EventArgs e)
        {
            if( this.opt3.BackColor == System.Drawing.Color.Wheat )
            {
                this.opt3.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                this.opt3.BackColor = System.Drawing.Color.Wheat;
            }
        }
		
        void Opt4Click(object sender, System.EventArgs e)
        {
            if( this.opt4.BackColor == System.Drawing.Color.Wheat )
            {
                this.opt4.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                this.opt4.BackColor = System.Drawing.Color.Wheat;
            }
        }
		
        void Opt5Click(object sender, System.EventArgs e)
        {
            if( this.opt5.BackColor == System.Drawing.Color.Wheat )
            {
                this.opt5.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                this.opt5.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void menuCondiciones1_BOk(object sender, System.EventArgs e)
        {
            ActualizarDatos();			
            //cerrar ventana
            grupo.ActivaFiltro(filtro);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, System.EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, System.EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Pesos Numéricos(*.pes)|*.pes|Pesos Numéricos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, System.EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Pesos Numéricos(*.pes)|*.pes|Pesos Numéricos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo )==true)
            {
                Grupo g=archComb.LeeCondicion();
                filtro=(FiltroPesosNumericos)g.GetFiltro("PesosNumericos");
                MarcarValores();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, System.EventArgs e)
        {
            ActualizarDatos();
            filtro=(FiltroPesosNumericos)grupo.GetFiltro("PesosNumericos");
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroPesosNumericos();
            MarcarValores();
        }

        private void menuCondiciones1_BCopiar(object sender, System.EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.pes";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, System.EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.pes";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            FormulariosHelper mf = new FormulariosHelper();
            if(mf.ExisteFicheroTemporal("tmp.pes")==true)
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }
        private void CerrarVentana()
        {
            this.Close();
        }

        #region Selección de Figuras
        protected void MarcarFigura(Label l)
        {
            if (l.BackColor == System.Drawing.Color.Wheat)
            {
                //Está desactivada, activarla
                long f = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(l.Text);
                if (!this.figuras.Contains(f))
                {
                    this.figuras.Add(f);
                }
                l.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                //Está activada, desactivarla
                long f = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(l.Text);
                if (this.figuras.Contains(f))
                {
                    this.figuras.Remove(f);
                }
                l.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void lblFig32_Click(object sender, EventArgs e)
        {
            MarcarFigura(lblFig32);
        }

        private void lblFig311_Click(object sender, EventArgs e)
        {
            MarcarFigura(lblFig311);
        }

        private void lblFig221_Click(object sender, EventArgs e)
        {
            MarcarFigura(lblFig221);
        }

        private void lblFig2111_Click(object sender, EventArgs e)
        {
            MarcarFigura(lblFig2111);
        }

        private void lblFig11111_Click(object sender, EventArgs e)
        {
            MarcarFigura(lblFig11111);
        } 
        #endregion

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroPesosNumericos filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Free1X2.UI.Estadisticas.VisorEstadisticas visor = new Free1X2.UI.Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
