// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 xfsf
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
using System.Collections.Generic;
using System.Windows.Forms;
using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.Utils;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for ValoracionFrm.
    /// </summary>
    public class ValoracionFrm : Form
    {
        private Label label16;
        private System.ComponentModel.IContainer components;
		
        private Grupo grupo;
        FiltroValoracionSignos filtro;

        private double[] valores1 = new double[VariablesGlobales.NumeroPartidos];
        private double[] valoresX = new double[VariablesGlobales.NumeroPartidos];
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radTipoVal_Suma;
        private System.Windows.Forms.RadioButton radTipoVal_Multiplo;
        private double[] valores2 = new double[VariablesGlobales.NumeroPartidos];
        private ValidadorCadenas Valida= new ValidadorCadenas();
        private TextBox valGlobal;
        private Label label18;
        private Label label19;
        private Label label20;
        private TextBox TxMinMaxSumas;
        private TextBox TxMinMaxProductos;
        private Label LblMinMax;
        private Label label22;
        private TextBox TxVal1;
        private TextBox TxValX;
        private TextBox TxVal2;
        private ToolTip toolTip1;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private TextBox txResultado;
        private Label label21;
        private Label label17;
        private Button btnCalculoVal;
        private TextBox txtColumna;
        private Button btnBuscarLimites;
        private Button btnColsBase;
        private TextBox txtColBase;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        private Controls.ControlPorcentajes controlPorcentajes1;
        private Controls.ctrlAyuda ctrlAyuda1;
        protected FormulariosHelper formHelper = new FormulariosHelper();


        private string tipoValoracion = "suma";

        public ValoracionFrm(Grupo grupo)
        {
            InitializeComponent();
            this.grupo = grupo;
            string nombreFiltro = Filtro.ValoracionSignos.ToString();
            filtro = (FiltroValoracionSignos)grupo.GetFiltro( nombreFiltro );
            InicializarPantalla();
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Debe especificar los porcentajes para cada partido\ny acotar el rango mínimo y máximo para los valores\nGlobal, 1, X y 2";
            txtColumna.MaxLength = VariablesGlobales.NumeroPartidos;
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        protected void InicializarPantalla()
        {
            if(filtro.ContieneDatos)
            {
                if(filtro.TipoValoracion == "suma")
                {
                    radTipoVal_Suma.Checked = true;
                    tipoValoracion = "suma";
                }
                else
                {
                    radTipoVal_Multiplo.Checked = true;
                    tipoValoracion = "multiplo";
                }

                PonerValoracionPantalla(filtro.Valores1, filtro.ValoresX, filtro.Valores2 );
                valGlobal.Text = filtro.ValorGlobal;
                TxVal1.Text = filtro.ValorUnos ;
                TxValX.Text = filtro.ValorEquis ;
                TxVal2.Text = filtro.ValorDoses ;
            }
        }
		
        protected string[] ObtenerColsBase()
        {
            string[] columna=new string[3];
            string tmp;
            int i, j;

            //REINICIALIZA VALORES
            valores1 = new double[VariablesGlobales.NumeroPartidos];
            valoresX = new double[VariablesGlobales.NumeroPartidos];
            valores2 = new double[VariablesGlobales.NumeroPartidos];
            for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                valores1[i]=controlPorcentajes1.Valores [i,0];
                valoresX[i]=controlPorcentajes1.Valores [i,1];
                valores2[i]=controlPorcentajes1.Valores [i,2];
            }

            for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                tmp=ordenarValoracionPartido(valores1[i],valoresX[i],valores2[i]);
                for (j=0;j<3;j++)
                {
                    columna[j]+=tmp.Substring(j,1);
                }
            }
            return columna;
        }

        protected string ordenarValoracionPartido(double v1, double vX, double v2)
        {
            string mayor;
            string medio;
            string menor;

            //Nota: El ordenamiento siempre favorece el signo mas casero en caso de que 
            //los valores sean indeticos entre alguno de los signos.

            if((v1 >= v2) && (v1 >= vX))
            {
                mayor="1";

                if(vX >= v2)
                {
                    medio="X";
                    menor="2";
                }
                else
                {
                    medio="2";
                    menor="X";
                }
            }
            else if(vX >= v2)
            {
                mayor="X";

                if(v1 >= v2)
                {
                    medio="1";
                    menor="2";
                }
                else
                {
                    medio="2";
                    menor="1";
                }
            }
            else
            {
                mayor="2";

                if(vX > v1)
                {
                    medio="X";
                    menor="1";
                }
                else
                {
                    medio="1";
                    menor="X";
                }
            }

            return mayor + medio + menor;
        }
		
        protected void PrepararValores()
        {
            double sumaValorMin = 0;
            double sumaValorMax = 0;
			
            double sumaValor1Max = 0;
            double sumaValorXMax = 0;
            double sumaValor2Max = 0;

            double ProductoValorMin = 1;
            double ProductoValorMax = 1;
			
            double ProductoValor1Max = 1;
            double ProductoValorXMax = 1;
            double ProductoValor2Max = 1;

            int i;

            //REINICIALIZA VALORES
            valores1 = new double[VariablesGlobales.NumeroPartidos];
            valoresX = new double[VariablesGlobales.NumeroPartidos];
            valores2 = new double[VariablesGlobales.NumeroPartidos];
            for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                valores1[i]=controlPorcentajes1.Valores [i,0];
                valoresX[i]=controlPorcentajes1.Valores [i,1];
                valores2[i]=controlPorcentajes1.Valores [i,2];
            }

            for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                //globales
                sumaValorMax += ObtenValorMaximo(valores1[i], valoresX[i], valores2[i]);
                sumaValorMin += ObtenValorMinimo(valores1[i], valoresX[i], valores2[i]);
                if((valores1[i]+ valoresX[i]+ valores2[i]) >0) 
                {
                    ProductoValorMax *= ObtenValorMaximo(valores1[i], valoresX[i], valores2[i])*0.03420425138;
                    ProductoValorMin *= ObtenValorMinimo(valores1[i], valoresX[i], valores2[i])*0.03420425138;
                }

                //por sumas
                sumaValor1Max += valores1[i];
                sumaValorXMax += valoresX[i];
                sumaValor2Max += valores2[i];

                //por multimplos solo valores mayores de 29.236 producen aumento de la valoración
                if(valores1[i]>29.236) ProductoValor1Max *= valores1[i]*0.03420425138;
                if(valoresX[i]>29.236) ProductoValorXMax *= valoresX[i]*0.03420425138;
                if(valores2[i]>29.236) ProductoValor2Max *= valores2[i]*0.03420425138;

            }
	
            ProductoValorMax=Math.Round(ProductoValorMax+0.000999999,3);
            ProductoValorMin=Math.Round(ProductoValorMin-0.000999999,3);
            if (ProductoValorMin<0) ProductoValorMin=0;
            TxMinMaxSumas.Text = sumaValorMin + "-" + sumaValorMax;
            TxMinMaxProductos.Text =  ProductoValorMin + "-" + ProductoValorMax;

            if(valGlobal.Text == "")
            {
                if (radTipoVal_Suma.Checked)
                {
                    valGlobal.Text = sumaValorMin + "-" + sumaValorMax;
                }
                else
                {
                    valGlobal.Text =  ProductoValorMin + "-" + ProductoValorMax;
                }
            }		

            if(TxVal1.Text == "")
            {
                if (radTipoVal_Suma.Checked)
                {
                    TxVal1.Text = "0-" + sumaValor1Max;					
                }
                else
                {
                    TxVal1.Text = "0-" + Math.Round (ProductoValor1Max+0.000999999,3);	
					
                }
            }	

            if(TxValX.Text == "")
            {
                if (radTipoVal_Suma.Checked)
                {					
                    TxValX.Text = "0-" + sumaValorXMax;					
                }
                else
                {					
                    TxValX.Text = "0-" + Math.Round (ProductoValorXMax+0.000999999,3);					
                }
            }	

            if(TxVal2.Text == "")
            {
                if (radTipoVal_Suma.Checked)
                {
                    TxVal2.Text = "0-" + sumaValor2Max;
                }
                else
                {
                    TxVal2.Text = "0-" + Math.Round (ProductoValor2Max+0.000999999,3);
                }
            }	
        }


        protected double ObtenValorMaximo(double valor1, double valor2, double valor3)
        {
            double valorMaximo = 0;
            valorMaximo = valor1;
            if(valor2 > valorMaximo)
            {
                valorMaximo = valor2;
            }
            if( valor3 > valorMaximo)
            {
                valorMaximo = valor3;
            }
            return valorMaximo;
        }

        protected double ObtenValorMinimo(double valor1, double valor2, double valor3)
        {
            double valorMinimo = valor1;
            if(valor2 < valorMinimo)
            {
                valorMinimo = valor2;
            }
            if( valor3 < valorMinimo)
            {
                valorMinimo = valor3;
            }
            return valorMinimo;
        }

        protected void PonerCondicionesFiltro()
        {
            filtro.ReinicializaValores();
			
            filtro.TipoValoracion = tipoValoracion;
            filtro.Valores1 = valores1;
            filtro.ValoresX = valoresX;
            filtro.Valores2 = valores2;
            filtro.ValorGlobal = valGlobal.Text;
            filtro.ValorUnos = TxVal1.Text;
            filtro.ValorEquis  = TxValX.Text;
            filtro.ValorDoses = TxVal2.Text;
            if(filtro.ContieneDatos == false)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                filtro.IsActive = true;				
            }
            filtro.ContieneDatos = true;
        }

        protected void PonerValoracionPantalla(double[] v1, double[] vX, double[] v2)
        {
            double[,] v = new double[VariablesGlobales.NumeroPartidos, 3];

            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                v[i,0]=v1[i];
                v[i,1]=vX[i];
                v[i,2]=v2[i];
            }
            controlPorcentajes1.Valores = v;
        }

        protected void ReiniciarValoracionPantalla()
        {

            TxMinMaxSumas.Text="";
            TxMinMaxProductos.Text="";
        }

        protected void CalcularValoracionColumna()
        {
            if (txtColumna.Text == "" || txtColumna.Text.Length < VariablesGlobales.NumeroPartidos)
            {
                MessageBox.Show("Introducir columna de " + VariablesGlobales.NumeroPartidos + " signos para el calculo.", "Error de calculo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                PrepararValores();
            }
            else
            {
                PrepararValores();

                filtro.Valores1 = valores1;
                filtro.ValoresX = valoresX;
                filtro.Valores2 = valores2;
                filtro.TipoValoracion = tipoValoracion;

                filtro.Analizar( UtilColumnas.ConvStrToLong(txtColumna.Text) );
                txResultado.Text = filtro.ValoracionResultado.ToString("r");
            }	
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValoracionFrm));
            this.valGlobal = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.LblMinMax = new System.Windows.Forms.Label();
            this.TxMinMaxProductos = new System.Windows.Forms.TextBox();
            this.TxMinMaxSumas = new System.Windows.Forms.TextBox();
            this.radTipoVal_Multiplo = new System.Windows.Forms.RadioButton();
            this.radTipoVal_Suma = new System.Windows.Forms.RadioButton();
            this.label18 = new System.Windows.Forms.Label();
            this.TxVal1 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.TxValX = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.TxVal2 = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtColBase = new System.Windows.Forms.TextBox();
            this.btnColsBase = new System.Windows.Forms.Button();
            this.btnBuscarLimites = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txResultado = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnCalculoVal = new System.Windows.Forms.Button();
            this.txtColumna = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.controlPorcentajes1 = new Free1X2.UI.Controls.ControlPorcentajes();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // valGlobal
            // 
            this.valGlobal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valGlobal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valGlobal.Location = new System.Drawing.Point(225, 111);
            this.valGlobal.Name = "valGlobal";
            this.valGlobal.Size = new System.Drawing.Size(300, 21);
            this.valGlobal.TabIndex = 59;
            this.toolTip1.SetToolTip(this.valGlobal, "Usar # como separador rango (Ejemplo: 100-200#200-600)");
            this.valGlobal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoDeRangos_KeyPress);
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(176, 111);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 21);
            this.label16.TabIndex = 61;
            this.label16.Text = "Global";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                          | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Bisque;
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.LblMinMax);
            this.groupBox1.Controls.Add(this.TxMinMaxProductos);
            this.groupBox1.Controls.Add(this.TxMinMaxSumas);
            this.groupBox1.Controls.Add(this.radTipoVal_Multiplo);
            this.groupBox1.Controls.Add(this.radTipoVal_Suma);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(179, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(346, 72);
            this.groupBox1.TabIndex = 63;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tipo Valoración";
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.Black;
            this.label22.Location = new System.Drawing.Point(159, 38);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(64, 21);
            this.label22.TabIndex = 6;
            this.label22.Text = "Mín-Máx";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblMinMax
            // 
            this.LblMinMax.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblMinMax.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblMinMax.ForeColor = System.Drawing.Color.Black;
            this.LblMinMax.Location = new System.Drawing.Point(159, 16);
            this.LblMinMax.Name = "LblMinMax";
            this.LblMinMax.Size = new System.Drawing.Size(64, 21);
            this.LblMinMax.TabIndex = 5;
            this.LblMinMax.Text = "Mín-Máx";
            this.LblMinMax.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxMinMaxProductos
            // 
            this.TxMinMaxProductos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                                  | System.Windows.Forms.AnchorStyles.Right)));
            this.TxMinMaxProductos.BackColor = System.Drawing.SystemColors.Info;
            this.TxMinMaxProductos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxMinMaxProductos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxMinMaxProductos.ForeColor = System.Drawing.Color.Black;
            this.TxMinMaxProductos.Location = new System.Drawing.Point(226, 38);
            this.TxMinMaxProductos.MaxLength = 3;
            this.TxMinMaxProductos.Name = "TxMinMaxProductos";
            this.TxMinMaxProductos.ReadOnly = true;
            this.TxMinMaxProductos.Size = new System.Drawing.Size(114, 21);
            this.TxMinMaxProductos.TabIndex = 3;
            // 
            // TxMinMaxSumas
            // 
            this.TxMinMaxSumas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                              | System.Windows.Forms.AnchorStyles.Right)));
            this.TxMinMaxSumas.BackColor = System.Drawing.SystemColors.Info;
            this.TxMinMaxSumas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxMinMaxSumas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxMinMaxSumas.ForeColor = System.Drawing.Color.Black;
            this.TxMinMaxSumas.Location = new System.Drawing.Point(226, 16);
            this.TxMinMaxSumas.MaxLength = 3;
            this.TxMinMaxSumas.Name = "TxMinMaxSumas";
            this.TxMinMaxSumas.ReadOnly = true;
            this.TxMinMaxSumas.Size = new System.Drawing.Size(114, 21);
            this.TxMinMaxSumas.TabIndex = 2;
            // 
            // radTipoVal_Multiplo
            // 
            this.radTipoVal_Multiplo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radTipoVal_Multiplo.ForeColor = System.Drawing.Color.Black;
            this.radTipoVal_Multiplo.Location = new System.Drawing.Point(8, 40);
            this.radTipoVal_Multiplo.Name = "radTipoVal_Multiplo";
            this.radTipoVal_Multiplo.Size = new System.Drawing.Size(143, 21);
            this.radTipoVal_Multiplo.TabIndex = 1;
            this.radTipoVal_Multiplo.Text = "Por productos x 3E7";
            this.radTipoVal_Multiplo.Click += new System.EventHandler(this.radTipoVal_Clicked);
            this.radTipoVal_Multiplo.CheckedChanged += new System.EventHandler(this.radTipoVal_Multiplo_CheckedChanged);
            // 
            // radTipoVal_Suma
            // 
            this.radTipoVal_Suma.Checked = true;
            this.radTipoVal_Suma.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radTipoVal_Suma.ForeColor = System.Drawing.Color.Black;
            this.radTipoVal_Suma.Location = new System.Drawing.Point(8, 16);
            this.radTipoVal_Suma.Name = "radTipoVal_Suma";
            this.radTipoVal_Suma.Size = new System.Drawing.Size(104, 20);
            this.radTipoVal_Suma.TabIndex = 0;
            this.radTipoVal_Suma.TabStop = true;
            this.radTipoVal_Suma.Text = "Por sumas";
            this.radTipoVal_Suma.Click += new System.EventHandler(this.radTipoVal_Clicked);
            this.radTipoVal_Suma.CheckedChanged += new System.EventHandler(this.radTipoVal_Suma_CheckedChanged);
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(176, 133);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(43, 21);
            this.label18.TabIndex = 67;
            this.label18.Text = "1";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxVal1
            // 
            this.TxVal1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxVal1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxVal1.Location = new System.Drawing.Point(225, 133);
            this.TxVal1.Name = "TxVal1";
            this.TxVal1.Size = new System.Drawing.Size(300, 21);
            this.TxVal1.TabIndex = 66;
            this.toolTip1.SetToolTip(this.TxVal1, "Usar # como separador rango (Ejemplo: 100-200#200-600)");
            this.TxVal1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoDeRangos_KeyPress);
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(176, 155);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(43, 21);
            this.label19.TabIndex = 69;
            this.label19.Text = "X";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxValX
            // 
            this.TxValX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxValX.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxValX.Location = new System.Drawing.Point(225, 155);
            this.TxValX.Name = "TxValX";
            this.TxValX.Size = new System.Drawing.Size(300, 21);
            this.TxValX.TabIndex = 68;
            this.toolTip1.SetToolTip(this.TxValX, "Usar # como separador rango (Ejemplo: 100-200#200-600)");
            this.TxValX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoDeRangos_KeyPress);
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(176, 177);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(43, 21);
            this.label20.TabIndex = 71;
            this.label20.Text = "2";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxVal2
            // 
            this.TxVal2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxVal2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxVal2.Location = new System.Drawing.Point(225, 177);
            this.TxVal2.Name = "TxVal2";
            this.TxVal2.Size = new System.Drawing.Size(300, 21);
            this.TxVal2.TabIndex = 70;
            this.toolTip1.SetToolTip(this.TxVal2, "Usar # como separador rango (Ejemplo: 100-200#200-600)");
            this.TxVal2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoDeRangos_KeyPress);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtColBase);
            this.groupBox3.Controls.Add(this.btnColsBase);
            this.groupBox3.Controls.Add(this.btnBuscarLimites);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(176, 204);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(349, 221);
            this.groupBox3.TabIndex = 572;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Utilidades";
            // 
            // txtColBase
            // 
            this.txtColBase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColBase.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtColBase.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColBase.ForeColor = System.Drawing.Color.Black;
            this.txtColBase.Location = new System.Drawing.Point(145, 148);
            this.txtColBase.Multiline = true;
            this.txtColBase.Name = "txtColBase";
            this.txtColBase.ReadOnly = true;
            this.txtColBase.Size = new System.Drawing.Size(180, 56);
            this.txtColBase.TabIndex = 573;
            this.txtColBase.TabStop = false;
            this.txtColBase.Visible = false;
            // 
            // btnColsBase
            // 
            this.btnColsBase.BackColor = System.Drawing.Color.LightSalmon;
            this.btnColsBase.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnColsBase.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColsBase.ForeColor = System.Drawing.Color.Black;
            this.btnColsBase.Image = ((System.Drawing.Image)(resources.GetObject("btnColsBase.Image")));
            this.btnColsBase.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnColsBase.Location = new System.Drawing.Point(24, 177);
            this.btnColsBase.Name = "btnColsBase";
            this.btnColsBase.Size = new System.Drawing.Size(120, 24);
            this.btnColsBase.TabIndex = 572;
            this.btnColsBase.Text = "Columnas base";
            this.btnColsBase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnColsBase.UseVisualStyleBackColor = false;
            this.btnColsBase.Click += new System.EventHandler(this.btnColsBase_Click);
            // 
            // btnBuscarLimites
            // 
            this.btnBuscarLimites.BackColor = System.Drawing.Color.LightSalmon;
            this.btnBuscarLimites.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscarLimites.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarLimites.ForeColor = System.Drawing.Color.Black;
            this.btnBuscarLimites.Image = ((System.Drawing.Image)(resources.GetObject("btnBuscarLimites.Image")));
            this.btnBuscarLimites.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscarLimites.Location = new System.Drawing.Point(24, 151);
            this.btnBuscarLimites.Name = "btnBuscarLimites";
            this.btnBuscarLimites.Size = new System.Drawing.Size(120, 24);
            this.btnBuscarLimites.TabIndex = 571;
            this.btnBuscarLimites.Text = "Buscar límite";
            this.btnBuscarLimites.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBuscarLimites.UseVisualStyleBackColor = false;
            this.btnBuscarLimites.Click += new System.EventHandler(this.btnBuscarLimites_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                          | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Bisque;
            this.groupBox2.Controls.Add(this.txResultado);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.btnCalculoVal);
            this.groupBox2.Controls.Add(this.txtColumna);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(20, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(309, 109);
            this.groupBox2.TabIndex = 65;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Calcular Valoración";
            // 
            // txResultado
            // 
            this.txResultado.BackColor = System.Drawing.SystemColors.Info;
            this.txResultado.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txResultado.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txResultado.ForeColor = System.Drawing.Color.Black;
            this.txResultado.Location = new System.Drawing.Point(75, 55);
            this.txResultado.MaxLength = 20;
            this.txResultado.Name = "txResultado";
            this.txResultado.ReadOnly = true;
            this.txResultado.Size = new System.Drawing.Size(128, 21);
            this.txResultado.TabIndex = 5;
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.Black;
            this.label21.Location = new System.Drawing.Point(10, 33);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(64, 21);
            this.label21.TabIndex = 4;
            this.label21.Text = "Columna:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(10, 56);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(64, 20);
            this.label17.TabIndex = 2;
            this.label17.Text = "Valor:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCalculoVal
            // 
            this.btnCalculoVal.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCalculoVal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalculoVal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalculoVal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCalculoVal.Image = ((System.Drawing.Image)(resources.GetObject("btnCalculoVal.Image")));
            this.btnCalculoVal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCalculoVal.Location = new System.Drawing.Point(211, 52);
            this.btnCalculoVal.Name = "btnCalculoVal";
            this.btnCalculoVal.Size = new System.Drawing.Size(80, 24);
            this.btnCalculoVal.TabIndex = 1;
            this.btnCalculoVal.Text = "Calcula";
            this.btnCalculoVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCalculoVal.UseVisualStyleBackColor = false;
            this.btnCalculoVal.Click += new System.EventHandler(this.btnCalculoVal_Click);
            // 
            // txtColumna
            // 
            this.txtColumna.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                           | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColumna.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColumna.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtColumna.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColumna.ForeColor = System.Drawing.Color.Black;
            this.txtColumna.Location = new System.Drawing.Point(75, 33);
            this.txtColumna.MaxLength = 0;
            this.txtColumna.Name = "txtColumna";
            this.txtColumna.Size = new System.Drawing.Size(128, 21);
            this.txtColumna.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 449);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(540, 48);
            this.panel1.TabIndex = 573;
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
            this.menuCondiciones1.Location = new System.Drawing.Point(232, 8);
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
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(517, 2);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 575;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // controlPorcentajes1
            // 
            this.controlPorcentajes1.archivoPorcentajes = null;
            this.controlPorcentajes1.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajes1.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajes1.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajes1.Jornada = "01";
            this.controlPorcentajes1.Location = new System.Drawing.Point(8, 36);
            this.controlPorcentajes1.Name = "controlPorcentajes1";
            this.controlPorcentajes1.ReadOnly = false;
            this.controlPorcentajes1.Size = new System.Drawing.Size(160, 412);
            this.controlPorcentajes1.TabIndex = 574;
            this.controlPorcentajes1.Temporada = "2004/2005";
            // 
            // ValoracionFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(540, 497);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.controlPorcentajes1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.TxVal2);
            this.Controls.Add(this.TxValX);
            this.Controls.Add(this.TxVal1);
            this.Controls.Add(this.valGlobal);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label16);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(512, 400);
            this.Name = "ValoracionFrm";
            this.Text = "Valoración";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void InicializaRangos()
        {
            valGlobal.Text ="";
            TxVal1.Text ="";
            TxVal2.Text ="";
            TxValX.Text ="";
        }


        private void radTipoVal_Clicked(object sender, EventArgs e)
        {

            if(radTipoVal_Suma.Checked)
            {
                tipoValoracion = "suma";
            }
            else
            {
                tipoValoracion = "multiplo";
            }
        }

        private void btnCalculoVal_Click(object sender, EventArgs e)
        {
            CalcularValoracionColumna();
        }

        private void textBoxgenericoDeRangos_KeyPress(object sender, KeyPressEventArgs e)
        {
            Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosYsignosEsp, sender, e);
        }

        private void radTipoVal_Multiplo_CheckedChanged(object sender, EventArgs e)
        {
            InicializaRangos();
        }

        private void radTipoVal_Suma_CheckedChanged(object sender, EventArgs e)
        {
            InicializaRangos();
        }

        private void btnBuscarLimites_Click(object sender, EventArgs e)
        {
            BuscaLimsFrm morrisonLim = new BuscaLimsFrm();
            morrisonLim.ShowDialog();
        }

        private void btnColsBase_Click(object sender, EventArgs e)
        {
            string[] columna=ObtenerColsBase();
            txtColBase.Text=columna[0]+"\r\n"+columna[1]+"\r\n"+columna[2];
            txtColBase.Visible=true;
        }

        private void ActualizarDatos()
        {
            PrepararValores();
            PonerCondicionesFiltro();
        }
        private FiltroValoracionSignos ObtenerFiltroTemporal()
        {
            PrepararValores();

            FiltroValoracionSignos filtroTemp = new FiltroValoracionSignos();
            filtroTemp.ReinicializaValores();

            filtroTemp.TipoValoracion = tipoValoracion;
            filtroTemp.Valores1 = valores1;
            filtroTemp.ValoresX = valoresX;
            filtroTemp.Valores2 = valores2;
            filtroTemp.ValorGlobal = valGlobal.Text;
            filtroTemp.ValorUnos = TxVal1.Text;
            filtroTemp.ValorEquis = TxValX.Text;
            filtroTemp.ValorDoses = TxVal2.Text;
            if (filtroTemp.ContieneDatos == false)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                filtroTemp.IsActive = true;
            }
            filtroTemp.ContieneDatos = true;

            return filtroTemp;
        }

        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            ActualizarDatos();
            grupo.ActivaFiltro(filtro);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Valoración de Signos(*.valor)|*.valor|Valoración de Signos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Valoración de Signos(*.valor)|*.valor|Valoración de Signos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo ))
            {
                Grupo g=archComb.LeeCondicion();
                filtro=(FiltroValoracionSignos)g.GetFiltro("ValoracionSignos");
                InicializarPantalla();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroValoracionSignos();
            // cambia el flag temporalmente
            filtro.ContieneDatos=true;
            InicializarPantalla();
            filtro.ContieneDatos=false;
            ReiniciarValoracionPantalla();
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.valor";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.valor";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.valor"))
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }
        private void CerrarVentana()
        {
            Close();
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroValoracionSignos filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }

    }
}
