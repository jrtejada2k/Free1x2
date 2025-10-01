// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;
using Free1X2.UI.Controls;

namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for GruposEquiposFrm.
    /// </summary>
    public class GruposEquiposFrm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;


        private Grupo grupo;
        private FiltroGruposEquipos filtroGE;
        private List<GrupoEquipos> arrayGE;
        private List<RelacionGE1> arrayRelaciones1;
        private Label label43;
        private Label label44;
        private Label label45;
        private Label label46;
        private GroupBox groupBox1;
        private TextBox txtVictorias;
        private TextBox txtEmpates;
        private TextBox txtDerrotas;
        private TextBox txtSumaPuntos;
        private Label lblNoGruposEq;
        private Button btnPrev;
        private Button btnNext;
        private Button btnEliminarGrupo;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox2;
        private Button btnPrevRel;
        private Button btnNexRel;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label19;
        private TextBox txtGERel;
        private TextBox txtSumaVictoriasRel;
        private TextBox txtSumaEmpatesRel;
        private TextBox txtSumaDerrotasRel;
        private TextBox txtSumaPuntosRel;
        private Label label20;
        private int noGEPantalla;
        private Label lblNoRel1;
        private MenuCondiciones menuCondiciones1;
        private int relGE1Pantalla;
        private ctrlAyuda ctrlAyuda1;
        private ctrlAyuda ctrlAyuda2;
        private Button btnEliminaGERel;
        private MainForm parentFrm;
        protected FormulariosHelper formHelper = new FormulariosHelper();

        public GruposEquiposFrm(Grupo grupo, MainForm frm)
        {
            InitializeComponent();
            AñadirCasillas();
            parentFrm=frm;
            this.grupo = grupo;
            LlenaEquipos();
            InicializaDatos();
            InicializaDatosRelacionesGE();
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Marcar los equipos elegidos y\nespecificar las victorias/empates/derrotas\ny/o la suma de puntos que conseguirán";
        }
        protected void AñadirCasillas()
        {
            int x = 20;
            int y = 72;
            for (int i = 1; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                Label l2 = new Label();
                l2.Text = i.ToString();
                l2.Size = new Size(20, 16);
                l2.Location = new Point(x, y);
                l2.Name = "PartidoNumero" + i;
                l2.TextAlign = ContentAlignment.MiddleCenter;
                tabPage1.Controls.Add(l2);
                x += l2.Size.Width + 1;
                Label l = new Label();
                l.Name = "lblCasa" + i;
                l.Location = new Point(x, y);
                l.Size = new Size(100, 16);
                l.Click += Equipo_Click;
                l.BorderStyle = BorderStyle.FixedSingle;

                tabPage1.Controls.Add(l);

                x += l.Size.Width + 1;

                l = new Label();
                l.Name = "lblFuera" + i;
                l.Location = new Point(x, y);
                l.Size = new Size(100, 16);
                l.Click += Equipo_Click;
                l.BorderStyle = BorderStyle.FixedSingle;


                tabPage1.Controls.Add(l);

                y += l.Size.Height + 1;
                x -= l.Size.Width + 1 + l2.Size.Width + 1;
            }
        }

        #region GE

        public MainForm FormPadre
        {
            get{ return parentFrm; }
        }

        protected void InicializaDatos()
        {
            string nombreFiltro = Filtro.GruposEquipos.ToString();
            filtroGE = (FiltroGruposEquipos)grupo.GetFiltro( nombreFiltro );
            arrayGE = ObtenCopiaArrayGE( filtroGE );
            ActualizaDatosPantalla( noGEPantalla );
        }

        private int obtenNumPartido(Label lbl)
        {
            int numPartido=0;
            // El control es un label. Comprobamos nombre
            int pos = lbl.Name.IndexOf("lblCasa");
            if(pos>=0)
                numPartido=Convert.ToInt16(lbl.Name.Substring(7));
            else
            {
                // Comprobamos nombre si es partido fuera (en ese caso, ponemos el nº en negativo
                pos=lbl.Name.IndexOf("lblFuera");
                if(pos>=0)
                    numPartido=0-Convert.ToInt16(lbl.Name.Substring(8));
            }
            return numPartido;
        }

        private void LlenaEquipos()
        {
            for(int i = 0; i < tabPage1.Controls.Count; i++)
            {
                Label l = tabPage1.Controls[i] as Label;
                if(l != null)
                {
                    int numPartido = obtenNumPartido(l);
                    if(numPartido!=0)
                        l.Text=getEquipo(numPartido);
                }
            }
        }

        private string getEquipo(int numPartido)
        {
            // Si el nº de partido es positivo, se trata del equipo de casa, si no, el de fuera
            string equipo;
            PartidoBoleto p=FormPadre.pronosticos.BuscarControl(Math.Abs(numPartido));
            if(numPartido>0)
                equipo=p.EquipoCasa;
            else
                equipo=p.EquipoFuera;
            return equipo;
        }

        protected List<GrupoEquipos> ObtenCopiaArrayGE(FiltroGruposEquipos filtro)
        {
            List<GrupoEquipos> arrayGE_copia = new List<GrupoEquipos>();

            foreach( GrupoEquipos ge in filtro.GruposEquipos)
            {
                GrupoEquipos ge_copia = new GrupoEquipos();

                ge_copia.Pronosticos = ge.Pronosticos;
                ge_copia.SumaPuntos = ge.SumaPuntos;
                ge_copia.Victorias = ge.Victorias;
                ge_copia.Empates = ge.Empates;
                ge_copia.Derrotas = ge.Derrotas;

                arrayGE_copia.Add( ge_copia );
            }		
		
            return arrayGE_copia;
        }

        protected void ActualizaDatosPantalla( int noGE )
        {
            GrupoEquipos ge;
            if( arrayGE.Count > 0 )
            {
                ge = arrayGE[noGE];
                lblNoGruposEq.Text = (noGE + 1) + "/" + arrayGE.Count;
            }
            else
            {
                ge =new GrupoEquipos();
                lblNoGruposEq.Text = "1/1";
            }
            PonerEquiposSeleccionados(ge.Pronosticos);
            txtSumaPuntos.Text = ge.SumaPuntos;
            txtVictorias.Text = ge.Victorias;
            txtEmpates.Text = ge.Empates;
            txtDerrotas.Text = ge.Derrotas;
        }

        protected void CambiaGESelecionado()
        {
            CambiaGESelecionado( noGEPantalla );		
        }

        protected void CambiaGESelecionado( int noGE )
        {
            //primero guardar datos de pantalla
            GuardarGEActual();

            noGEPantalla = noGE;

            //crear ge si no existe 			
            if( arrayGE.Count < noGE + 1 )
            {
                GrupoEquipos ge = new GrupoEquipos();
                arrayGE.Add( ge );		
            }

            //activa/desactiva boton "atras" si estamos en la primera columna
            if( noGE == 0 )
            {
                btnPrev.Enabled = false;
            }
            else
            {
                btnPrev.Enabled = true;	
            }
			
            ActualizaDatosPantalla( noGE );
		
        }

        protected void GuardarGEActual()
        {
            GrupoEquipos ge;
			
            if( noGEPantalla  < arrayGE.Count)
            {
                ge = arrayGE[noGEPantalla];
                GuardaDatosGE( ge );
            }
            else if( TieneGEDatos() )
            {
                ge = new GrupoEquipos();
                arrayGE.Add( ge );

                GuardaDatosGE( ge );
            }		
			
        }
        private bool SonEntradasValidas()
        {
            //Las entradas deben ser numéricas o vacías... pero no pueden ser todas vacias...
            //
            bool sonValidas = false;
            if (txtVictorias.Text != "" || txtEmpates.Text != "" || txtDerrotas.Text != "" || txtSumaPuntos.Text != "")
            {
                if (Utils.UtilidadesEntradasValores.SonTodosNumeros(txtVictorias.Text) &&
                    Utils.UtilidadesEntradasValores.SonTodosNumeros(txtEmpates.Text) &&
                    Utils.UtilidadesEntradasValores.SonTodosNumeros(txtDerrotas.Text) &&
                    Utils.UtilidadesEntradasValores.SonTodosNumeros(txtSumaPuntos.Text))
                {
                    sonValidas = true;
                }
            }
            return sonValidas;
        }
        private bool SonEntradasValidasRelaciones()
        {
            //Las entradas deben ser numéricas o vacías... pero no pueden ser todas vacias...
            bool sonValidas = false;
            if (txtSumaVictoriasRel.Text != "" || txtSumaEmpatesRel.Text != "" || txtSumaDerrotasRel.Text != "" || txtSumaPuntosRel.Text != "")
            {
                if (Utils.UtilidadesEntradasValores.SonTodosNumeros(txtSumaVictoriasRel.Text) &&
                    Utils.UtilidadesEntradasValores.SonTodosNumeros(txtSumaEmpatesRel.Text) &&
                    Utils.UtilidadesEntradasValores.SonTodosNumeros(txtSumaDerrotasRel.Text) &&
                    Utils.UtilidadesEntradasValores.SonTodosNumeros(txtSumaPuntosRel.Text))
                {
                    sonValidas = true;
                }
            }
            return sonValidas;
        }
        private bool HayPronosticosActivos(GrupoEquipos ge)
        {
            bool hayPronosticos = false;
            for (int i = 0; i < ge.Pronosticos.Length; i++)
            {
                if (ge.Pronosticos[i] != '\0' && ge.Pronosticos[i] != '0')
                {
                    hayPronosticos = true;
                    break;
                }
            }
            return hayPronosticos;
        }
        protected void GuardaDatosGE(GrupoEquipos ge)
        {
            ge.Pronosticos = ObtenEquiposSeleccionados();
            ge.CalcularLongPronosticos();
            if (HayPronosticosActivos(ge))
            {
                if (SonEntradasValidas())
                {
                    ge.Victorias = txtVictorias.Text;
                    ge.Empates = txtEmpates.Text;
                    ge.Derrotas = txtDerrotas.Text;
                    ge.SumaPuntos = txtSumaPuntos.Text;
                }
                else
                {
                    MessageBox.Show("Hay errores en la entrada de datos", "Error");
                }
            }
        }

        protected void GuardarDatos()
        {
            GuardarGEActual();

            if (arrayGE.Count > 0)
            {
                //borrar ultima CP si no contiene datos
                if (NecesitaBorrarUltimoGE())
                {
                    BorrarGE(arrayGE.Count - 1);
                }
            }
            else
            {
                filtroGE.IsActive = false;
            }

            if (!filtroGE.ContieneDatos)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                if (arrayGE.Count > 0)
                {
                    filtroGE.ContieneDatos = true;
                    filtroGE.IsActive = true;
                }
                else
                {
                    filtroGE.IsActive = false;
                }
            }
            else
            {
                filtroGE.ContieneDatos = true;
                filtroGE.IsActive = true;
            }
            
            //guardar copia actualizada de ge en filtro

            for (int i = 0; i < arrayGE.Count; i++)
            {
                GrupoEquipos geq = arrayGE[i];
                geq.CalcularLongPronosticos();
            }
            filtroGE.GruposEquipos = arrayGE;		
        }

        private FiltroGruposEquipos ObtenerFiltroTemporal()
        {
            FiltroGruposEquipos filtroTemp = new FiltroGruposEquipos();
            List<GrupoEquipos> arrayGETemporal = new List<GrupoEquipos>();
            ArrayList arrayRelaciones1Temporal = new ArrayList();
            arrayGETemporal.AddRange(arrayGE);
            arrayRelaciones1Temporal.AddRange(arrayRelaciones1);

            GrupoEquipos ge;

            if (noGEPantalla < arrayGETemporal.Count)
            {
                ge = arrayGETemporal[noGEPantalla];
                GuardaDatosGE(ge);
            }
            else if (TieneGEDatos())
            {
                ge = new GrupoEquipos();
                arrayGETemporal.Add(ge);

                GuardaDatosGE(ge);
            }

            if (arrayGETemporal.Count > 0)
            {
                //borrar ultima CP si no contiene datos
                if (NecesitaBorrarUltimoGETemporal(arrayGETemporal))
                {
                    arrayGE.RemoveAt( arrayGETemporal.Count - 1);
                }
            }

            if (filtroTemp.ContieneDatos == false && arrayGETemporal.Count > 0)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                filtroTemp.ContieneDatos = true;
                filtroTemp.IsActive = true;
            }

            //guardar copia actualizada de ge en filtro

            for (int i = 0; i < arrayGETemporal.Count; i++)
            {
                GrupoEquipos geq = arrayGETemporal[i];
                geq.CalcularLongPronosticos();
            }
            filtroTemp.GruposEquipos = arrayGETemporal;


            RelacionGE1 rel2;

            if (relGE1Pantalla < arrayRelaciones1Temporal.Count)
            {
                rel2 = (RelacionGE1)arrayRelaciones1Temporal[relGE1Pantalla];
                GuardaDatosRel1(rel2);
            }
            else if (TieneRelacion1Datos())
            {
                //existen datos en pantalla que se necesitan poner en nueva rel
                rel2 = new RelacionGE1();
                arrayRelaciones1Temporal.Add(rel2);

                GuardaDatosRel1(rel2);
            }

            if (arrayRelaciones1Temporal.Count > 0)
            {
                //borrar ultima relacion si no contiene datos
                if (NecesitaBorrarUltimaRel1Temporal(arrayRelaciones1Temporal))
                {
                    arrayRelaciones1Temporal.RemoveAt(arrayRelaciones1Temporal.Count - 1);
                }
            }

            List<RelacionGE1> relacionesGEFinal = new List<RelacionGE1>();

            for (int i = 0; i < arrayRelaciones1Temporal.Count; i++)
            {
                RelacionGE1 rel = (RelacionGE1)arrayRelaciones1Temporal[i];

                if (rel.GruposEquipos != "")
                {
                    relacionesGEFinal.Add(rel);
                }
            }

            filtroTemp.RelacionesGE1.Relaciones = relacionesGEFinal;
            return filtroTemp;
        }

        protected void LimpiaEquiposPantalla()
        {
            for (int i = 0; i < tabPage1.Controls.Count; i++)
            {
                Label label = tabPage1.Controls[i] as Label;
                if (label != null)
                {
                    if ((label.Name.Remove(label.Name.Length - 1) == "lblCasa") ||
                        (label.Name.Remove(label.Name.Length - 1) == "lblFuera") ||
                        (label.Name.Remove(label.Name.Length - 2) == "lblCasa") ||
                        (label.Name.Remove(label.Name.Length - 2) == "lblFuera"))
                    {
                        int partido = Math.Abs(obtenNumPartido(label));
                        if (partido == 1 || partido == 2 || partido == 3 || partido == 4 || partido == 9 || partido == 10 || partido == 11)
                        {
                            label.BackColor = Color.LemonChiffon;
                        }
                        else
                        {
                            label.BackColor = Color.AntiqueWhite;
                        }
                    }
                }
            }
        }

        protected Label ObtenerLabel(string name)
        {
            Label label = new Label();
            for (int i = 0; i < tabPage1.Controls.Count; i++)
            {
                label = tabPage1.Controls[i] as Label;
                if (label != null)
                {
                    if (label.Name == name)
                    {
                        break;
                    }
                }
            }
            return label;
        }


        protected void PonerEquiposSeleccionados(char[] c)
        {
            LimpiaEquiposPantalla();
            for (int i = 0; i < c.Length; i++)
            {
                Label l;
                int partido = i + 1;
                if (c[i] == '1')
                {
                    l = ObtenerLabel("lblCasa" + Convert.ToString(i + 1));
                    if (partido == 1 || partido == 2 || partido == 3 || partido == 4 || partido == 9 || partido == 10 || partido == 11)
                    {
                        l.BackColor = Color.PaleTurquoise;
                    }
                    else
                    {
                        l.BackColor = Color.Yellow;
                    }
                }
                else if (c[i] == '2')
                {
                    l = ObtenerLabel("lblFuera" + Convert.ToString(i + 1));
                    if (partido == 1 || partido == 2 || partido == 3 || partido == 4 || partido == 9 || partido == 10 || partido == 11)
                    {
                        l.BackColor = Color.PaleTurquoise;
                    }
                    else
                    {
                        l.BackColor = Color.Yellow;
                    }
                }
                else if (c[i] == '3')
                {
                    l = ObtenerLabel("lblCasa" + Convert.ToString(i + 1));
                    if (partido == 1 || partido == 2 || partido == 3 || partido == 4 || partido == 9 || partido == 10 || partido == 11)
                    {
                        l.BackColor = Color.PaleTurquoise;
                    }
                    else
                    {
                        l.BackColor = Color.Yellow;
                    }
                    l = ObtenerLabel("lblFuera" + Convert.ToString(i + 1));
                    if (partido == 1 || partido == 2 || partido == 3 || partido == 4 || partido == 9 || partido == 10 || partido == 11)
                    {
                        l.BackColor = Color.PaleTurquoise;
                    }
                    else
                    {
                        l.BackColor = Color.Yellow;
                    }
                }
            }
        }

        protected char[] ObtenEquiposSeleccionados()
        {
            char[] c = new char[VariablesGlobales.NumeroPartidos];

            int n = 48;
            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                Label l = ObtenerLabel("lblCasa" + Convert.ToString(i + 1));
                Label l2 = ObtenerLabel("lblFuera" + Convert.ToString(i + 1));
                if ((l.BackColor == Color.PaleTurquoise) || (l.BackColor == Color.Yellow))
                {
                    n++;
                }
                if ((l2.BackColor == Color.PaleTurquoise) || (l2.BackColor == Color.Yellow))
                {
                    n += 2;
                }
                c[i] = Convert.ToChar(n);

                n = 48;
            }
            return c;
        }

        protected bool TieneGEDatos()
        {
            bool contieneValores = false;
            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                Label l = ObtenerLabel("lblCasa" + Convert.ToString(i + 1));
                Label l2 = ObtenerLabel("lblFuera" + Convert.ToString(i + 1));
                if ((l.BackColor == Color.PaleTurquoise) || (l2.BackColor == Color.PaleTurquoise) || (l.BackColor == Color.Yellow) || (l2.BackColor == Color.Yellow))
                {
                    contieneValores = true;
                    break;
                }
            }
            return contieneValores;
        }

        protected bool NecesitaBorrarUltimoGE()
        {
            bool borrar = true;
			
            GrupoEquipos ge = arrayGE[arrayGE.Count-1];
			
            char[] c = ge.Pronosticos;

            for(int i = 0; i < c.Length; i++)
            {
                if(c[i] == '1' || c[i] == '2' || c[i] == '3')
                {
                    borrar = false;
                    break;
                }			
            }		            			
            return borrar;		
        }
        protected bool NecesitaBorrarUltimoGETemporal(List<GrupoEquipos> arrayGETemporal)
        {
            bool borrar = true;

            GrupoEquipos ge = arrayGETemporal[arrayGETemporal.Count - 1];

            char[] c = ge.Pronosticos;

            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == '1' || c[i] == '2' || c[i] == '3')
                {
                    borrar = false;
                    break;
                }
            }
            return borrar;
        }

        protected void BorrarGE(int noGE)
        {
            arrayGE.RemoveAt( noGE );
        }


        #endregion

        #region GERelacion1

        protected void InicializaDatosRelacionesGE()
        {
            arrayRelaciones1 = new List<RelacionGE1>();

            //carga datos
            List<RelacionGE1> relacionesGE = filtroGE.RelacionesGE1.Relaciones;
			
            //guardar copia en UI 
            for(int i = 0; i < relacionesGE.Count; i++)
            {
                RelacionGE1 rel = new RelacionGE1();
                RelacionGE1 relGuardada = relacionesGE[i];
								
                rel.GruposEquipos = relGuardada.GruposEquipos;
                rel.SumaVictorias = relGuardada.SumaVictorias;
                rel.SumaEmpates = relGuardada.SumaEmpates;
                rel.SumaDerrotas = relGuardada.SumaDerrotas;
                rel.SumaPuntos = relGuardada.SumaPuntos;
				
                arrayRelaciones1.Add( rel );	
            }	
	
            ActualizaDatosPantRel1( relGE1Pantalla );
		
        }

        protected void ActualizaDatosPantRel1( int relGE1 )
        {
            RelacionGE1 rel;
            if(arrayRelaciones1.Count > 0)
            {
                rel = arrayRelaciones1[ relGE1 ]; 
                lblNoRel1.Text = (relGE1 + 1) + "/" + arrayRelaciones1.Count;
            }
            else
            {
                rel =new RelacionGE1();
                lblNoRel1.Text = "1/1";
            }
            txtGERel.Text = rel.GruposEquipos;
            txtSumaVictoriasRel.Text = rel.SumaVictorias;
            txtSumaEmpatesRel.Text = rel.SumaEmpates;
            txtSumaDerrotasRel.Text = rel.SumaDerrotas;
            txtSumaPuntosRel.Text = rel.SumaPuntos;
        }

        protected bool TieneRelacion1Datos()
        {
            bool contieneDatos = true;

            if(	txtGERel.Text == "")
            {
                contieneDatos = false;
            }
            else if (	txtSumaVictoriasRel.Text == "" && 
                     	txtSumaEmpatesRel.Text == "" &&
                     	txtSumaDerrotasRel.Text == "" &&
                     	txtSumaPuntosRel.Text == "")
            {
                contieneDatos = false;
            }

            return contieneDatos;
        }

        protected void CambiaRelGE1Selecionado(int relGE1)
        {
            //primero guardar datos de pantalla
            GuardarRelGE1Actual();
            relGE1Pantalla = relGE1;

            //crear rel1 si no existe 

            if( arrayRelaciones1.Count < relGE1 + 1 )
            {
                RelacionGE1 rel = new RelacionGE1(); 
                arrayRelaciones1.Add( rel );		
            }			
						
			
            //activa/desactiva boton "atras" si estamos en la primera relacion
            if( relGE1 == 0 )
            {
                btnPrevRel.Enabled = false;
            }
            else
            {
                btnPrevRel.Enabled = true;	
            }				

            ActualizaDatosPantRel1( relGE1Pantalla );
        }

        protected void GuardarRelGE1Actual()
        {
            RelacionGE1 rel;

            if( relGE1Pantalla  < arrayRelaciones1.Count)
            {
                rel = arrayRelaciones1[ relGE1Pantalla ];
                GuardaDatosRel1( rel );
            }
            else if( TieneRelacion1Datos() && SonEntradasValidasRelaciones() )
            {
                //existen datos en pantalla que se necesitan poner en nueva rel
                rel = new RelacionGE1();
                arrayRelaciones1.Add( rel );

                GuardaDatosRel1( rel );			
            }		
        }

        protected void GuardaDatosRel1( RelacionGE1 rel )
        {		
            if( TieneRelacion1Datos() && SonEntradasValidasRelaciones() )
            {
                rel.GruposEquipos =txtGERel.Text;
                rel.SumaVictorias = txtSumaVictoriasRel.Text;
                rel.SumaEmpates = txtSumaEmpatesRel.Text;
                rel.SumaDerrotas = txtSumaDerrotasRel.Text;
                rel.SumaPuntos = txtSumaPuntosRel.Text;				
            }			
        }

        protected void GuardarDatosRelacionesGE1()
        {
            GuardarRelGE1Actual();

            if( arrayRelaciones1.Count > 0 )
            {	
                //borrar ultima relacion si no contiene datos
                if( NecesitaBorrarUltimaRel1() )
                {
                    BorrarRel1( arrayRelaciones1.Count - 1 );				
                }
            }

            List<RelacionGE1> relacionesGEFinal = new List<RelacionGE1>();
			
            for(int i = 0; i < arrayRelaciones1.Count; i++)
            {
                RelacionGE1 rel = arrayRelaciones1[i];

                if(rel.GruposEquipos != "")
                {
                    relacionesGEFinal.Add( rel );
                }
            }

            filtroGE.RelacionesGE1.Relaciones = relacionesGEFinal;
		
        }

        protected bool NecesitaBorrarUltimaRel1()
        {
            bool borrar = false;

            RelacionGE1 rel = arrayRelaciones1[arrayRelaciones1.Count -1];

            if(rel.GruposEquipos == "")
            {
                borrar = true;
            }
            else if(rel.SumaVictorias == "" && 
                    rel.SumaEmpates == "" &&
                    rel.SumaDerrotas == "" &&
                    rel.SumaPuntos == "")
            {
                borrar = true;
            }
			
            return borrar;
        }
        protected bool NecesitaBorrarUltimaRel1Temporal(ArrayList arrayRelaciones1Temporal)
        {
            bool borrar = false;

            RelacionGE1 rel = (RelacionGE1)arrayRelaciones1Temporal[arrayRelaciones1Temporal.Count - 1];

            if (rel.GruposEquipos == "")
            {
                borrar = true;
            }
            else if (rel.SumaVictorias == "" &&
                     rel.SumaEmpates == "" &&
                     rel.SumaDerrotas == "" &&
                     rel.SumaPuntos == "")
            {
                borrar = true;
            }

            return borrar;
        }

        protected void BorrarRel1( int noRelGE1 )
        {
            arrayRelaciones1.RemoveAt( noRelGE1 );
		
        }


        #endregion

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GruposEquiposFrm));
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.lblNoGruposEq = new System.Windows.Forms.Label();
            this.txtVictorias = new System.Windows.Forms.TextBox();
            this.txtEmpates = new System.Windows.Forms.TextBox();
            this.txtDerrotas = new System.Windows.Forms.TextBox();
            this.txtSumaPuntos = new System.Windows.Forms.TextBox();
            this.btnEliminarGrupo = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnEliminaGERel = new System.Windows.Forms.Button();
            this.ctrlAyuda2 = new Free1X2.UI.Controls.ctrlAyuda();
            this.label20 = new System.Windows.Forms.Label();
            this.txtSumaPuntosRel = new System.Windows.Forms.TextBox();
            this.txtSumaDerrotasRel = new System.Windows.Forms.TextBox();
            this.txtSumaEmpatesRel = new System.Windows.Forms.TextBox();
            this.txtSumaVictoriasRel = new System.Windows.Forms.TextBox();
            this.txtGERel = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnNexRel = new System.Windows.Forms.Button();
            this.lblNoRel1 = new System.Windows.Forms.Label();
            this.btnPrevRel = new System.Windows.Forms.Button();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label43
            // 
            this.label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(280, 96);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(56, 21);
            this.label43.TabIndex = 42;
            this.label43.Text = "Victorias";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.Location = new System.Drawing.Point(280, 118);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(56, 21);
            this.label44.TabIndex = 43;
            this.label44.Text = "Empates";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label45
            // 
            this.label45.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(266, 140);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(70, 21);
            this.label45.TabIndex = 44;
            this.label45.Text = "Derrotas";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label46
            // 
            this.label46.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.Location = new System.Drawing.Point(245, 162);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(91, 21);
            this.label46.TabIndex = 45;
            this.label46.Text = "Suma Puntos";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnPrev);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Controls.Add(this.lblNoGruposEq);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(32, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 48);
            this.groupBox1.TabIndex = 46;
            this.groupBox1.TabStop = false;
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.Color.Silver;
            this.btnPrev.Enabled = false;
            this.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrev.Image = ((System.Drawing.Image)(resources.GetObject("btnPrev.Image")));
            this.btnPrev.Location = new System.Drawing.Point(13, 16);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(24, 23);
            this.btnPrev.TabIndex = 47;
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            this.btnPrev.EnabledChanged += new System.EventHandler(this.btnPrev_EnabledChanged);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.LightSalmon;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
            this.btnNext.Location = new System.Drawing.Point(145, 16);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(24, 23);
            this.btnNext.TabIndex = 47;
            this.btnNext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // lblNoGruposEq
            // 
            this.lblNoGruposEq.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblNoGruposEq.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoGruposEq.Location = new System.Drawing.Point(39, 16);
            this.lblNoGruposEq.Name = "lblNoGruposEq";
            this.lblNoGruposEq.Size = new System.Drawing.Size(104, 23);
            this.lblNoGruposEq.TabIndex = 53;
            this.lblNoGruposEq.Text = "0/0";
            this.lblNoGruposEq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtVictorias
            // 
            this.txtVictorias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVictorias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVictorias.Location = new System.Drawing.Point(352, 96);
            this.txtVictorias.Name = "txtVictorias";
            this.txtVictorias.Size = new System.Drawing.Size(128, 21);
            this.txtVictorias.TabIndex = 47;
            // 
            // txtEmpates
            // 
            this.txtEmpates.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpates.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmpates.Location = new System.Drawing.Point(352, 118);
            this.txtEmpates.Name = "txtEmpates";
            this.txtEmpates.Size = new System.Drawing.Size(128, 21);
            this.txtEmpates.TabIndex = 48;
            // 
            // txtDerrotas
            // 
            this.txtDerrotas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDerrotas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDerrotas.Location = new System.Drawing.Point(352, 140);
            this.txtDerrotas.Name = "txtDerrotas";
            this.txtDerrotas.Size = new System.Drawing.Size(128, 21);
            this.txtDerrotas.TabIndex = 49;
            // 
            // txtSumaPuntos
            // 
            this.txtSumaPuntos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSumaPuntos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSumaPuntos.Location = new System.Drawing.Point(352, 162);
            this.txtSumaPuntos.Name = "txtSumaPuntos";
            this.txtSumaPuntos.Size = new System.Drawing.Size(128, 21);
            this.txtSumaPuntos.TabIndex = 50;
            // 
            // btnEliminarGrupo
            // 
            this.btnEliminarGrupo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminarGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminarGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarGrupo.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminarGrupo.Image")));
            this.btnEliminarGrupo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminarGrupo.Location = new System.Drawing.Point(296, 24);
            this.btnEliminarGrupo.Name = "btnEliminarGrupo";
            this.btnEliminarGrupo.Size = new System.Drawing.Size(144, 23);
            this.btnEliminarGrupo.TabIndex = 53;
            this.btnEliminarGrupo.Text = "Eliminar Actual";
            this.btnEliminarGrupo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminarGrupo.UseVisualStyleBackColor = false;
            this.btnEliminarGrupo.Click += new System.EventHandler(this.btnEliminarGrupo_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(496, 391);
            this.tabControl1.TabIndex = 54;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Bisque;
            this.tabPage1.Controls.Add(this.ctrlAyuda1);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.btnEliminarGrupo);
            this.tabPage1.Controls.Add(this.txtVictorias);
            this.tabPage1.Controls.Add(this.txtEmpates);
            this.tabPage1.Controls.Add(this.txtDerrotas);
            this.tabPage1.Controls.Add(this.txtSumaPuntos);
            this.tabPage1.Controls.Add(this.label43);
            this.tabPage1.Controls.Add(this.label44);
            this.tabPage1.Controls.Add(this.label45);
            this.tabPage1.Controls.Add(this.label46);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(488, 365);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Grupos Equipos";
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(460, 3);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 54;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Bisque;
            this.tabPage2.Controls.Add(this.btnEliminaGERel);
            this.tabPage2.Controls.Add(this.ctrlAyuda2);
            this.tabPage2.Controls.Add(this.label20);
            this.tabPage2.Controls.Add(this.txtSumaPuntosRel);
            this.tabPage2.Controls.Add(this.txtSumaDerrotasRel);
            this.tabPage2.Controls.Add(this.txtSumaEmpatesRel);
            this.tabPage2.Controls.Add(this.txtSumaVictoriasRel);
            this.tabPage2.Controls.Add(this.txtGERel);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(488, 365);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Relaciones";
            // 
            // btnEliminaGERel
            // 
            this.btnEliminaGERel.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminaGERel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminaGERel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminaGERel.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminaGERel.Image")));
            this.btnEliminaGERel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminaGERel.Location = new System.Drawing.Point(296, 24);
            this.btnEliminaGERel.Name = "btnEliminaGERel";
            this.btnEliminaGERel.Size = new System.Drawing.Size(144, 23);
            this.btnEliminaGERel.TabIndex = 54;
            this.btnEliminaGERel.Text = "Eliminar Actual";
            this.btnEliminaGERel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminaGERel.UseVisualStyleBackColor = false;
            this.btnEliminaGERel.Click += new System.EventHandler(this.btnEliminaGERel_Click);
            // 
            // ctrlAyuda2
            // 
            this.ctrlAyuda2.Location = new System.Drawing.Point(460, 3);
            this.ctrlAyuda2.Name = "ctrlAyuda2";
            this.ctrlAyuda2.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda2.TabIndex = 12;
            this.ctrlAyuda2.TextoAyuda = "";
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(29, 194);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(100, 21);
            this.label20.TabIndex = 11;
            this.label20.Text = "Suma Puntos";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSumaPuntosRel
            // 
            this.txtSumaPuntosRel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSumaPuntosRel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSumaPuntosRel.Location = new System.Drawing.Point(134, 194);
            this.txtSumaPuntosRel.Name = "txtSumaPuntosRel";
            this.txtSumaPuntosRel.Size = new System.Drawing.Size(100, 21);
            this.txtSumaPuntosRel.TabIndex = 10;
            // 
            // txtSumaDerrotasRel
            // 
            this.txtSumaDerrotasRel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSumaDerrotasRel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSumaDerrotasRel.Location = new System.Drawing.Point(134, 172);
            this.txtSumaDerrotasRel.Name = "txtSumaDerrotasRel";
            this.txtSumaDerrotasRel.Size = new System.Drawing.Size(100, 21);
            this.txtSumaDerrotasRel.TabIndex = 9;
            // 
            // txtSumaEmpatesRel
            // 
            this.txtSumaEmpatesRel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSumaEmpatesRel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSumaEmpatesRel.Location = new System.Drawing.Point(134, 150);
            this.txtSumaEmpatesRel.Name = "txtSumaEmpatesRel";
            this.txtSumaEmpatesRel.Size = new System.Drawing.Size(100, 21);
            this.txtSumaEmpatesRel.TabIndex = 8;
            // 
            // txtSumaVictoriasRel
            // 
            this.txtSumaVictoriasRel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSumaVictoriasRel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSumaVictoriasRel.Location = new System.Drawing.Point(134, 128);
            this.txtSumaVictoriasRel.Name = "txtSumaVictoriasRel";
            this.txtSumaVictoriasRel.Size = new System.Drawing.Size(100, 21);
            this.txtSumaVictoriasRel.TabIndex = 7;
            // 
            // txtGERel
            // 
            this.txtGERel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGERel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGERel.Location = new System.Drawing.Point(134, 88);
            this.txtGERel.Name = "txtGERel";
            this.txtGERel.Size = new System.Drawing.Size(100, 21);
            this.txtGERel.TabIndex = 6;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(29, 172);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(100, 21);
            this.label19.TabIndex = 4;
            this.label19.Text = "Suma Derrotas";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            this.label18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(29, 150);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(100, 21);
            this.label18.TabIndex = 3;
            this.label18.Text = "Suma Empates";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(29, 128);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 21);
            this.label17.TabIndex = 2;
            this.label17.Text = "Suma Victorias";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(29, 88);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 21);
            this.label16.TabIndex = 1;
            this.label16.Text = "Grupos Equipos";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnNexRel);
            this.groupBox2.Controls.Add(this.lblNoRel1);
            this.groupBox2.Controls.Add(this.btnPrevRel);
            this.groupBox2.Location = new System.Drawing.Point(32, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(182, 48);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // btnNexRel
            // 
            this.btnNexRel.BackColor = System.Drawing.Color.LightSalmon;
            this.btnNexRel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNexRel.Image = ((System.Drawing.Image)(resources.GetObject("btnNexRel.Image")));
            this.btnNexRel.Location = new System.Drawing.Point(145, 16);
            this.btnNexRel.Name = "btnNexRel";
            this.btnNexRel.Size = new System.Drawing.Size(24, 23);
            this.btnNexRel.TabIndex = 2;
            this.btnNexRel.UseVisualStyleBackColor = false;
            this.btnNexRel.Click += new System.EventHandler(this.btnNexRel_Click);
            // 
            // lblNoRel1
            // 
            this.lblNoRel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoRel1.Location = new System.Drawing.Point(39, 16);
            this.lblNoRel1.Name = "lblNoRel1";
            this.lblNoRel1.Size = new System.Drawing.Size(104, 23);
            this.lblNoRel1.TabIndex = 1;
            this.lblNoRel1.Text = "0/0";
            this.lblNoRel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnPrevRel
            // 
            this.btnPrevRel.BackColor = System.Drawing.Color.Silver;
            this.btnPrevRel.Enabled = false;
            this.btnPrevRel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrevRel.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevRel.Image")));
            this.btnPrevRel.Location = new System.Drawing.Point(13, 16);
            this.btnPrevRel.Name = "btnPrevRel";
            this.btnPrevRel.Size = new System.Drawing.Size(24, 23);
            this.btnPrevRel.TabIndex = 0;
            this.btnPrevRel.UseVisualStyleBackColor = false;
            this.btnPrevRel.Click += new System.EventHandler(this.btnPrevRel_Click);
            this.btnPrevRel.EnabledChanged += new System.EventHandler(this.btnPrevRel_EnabledChanged);
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
            this.menuCondiciones1.Location = new System.Drawing.Point(178, 397);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 55;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // GruposEquiposFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(496, 445);
            this.ControlBox = false;
            this.Controls.Add(this.menuCondiciones1);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GruposEquiposFrm";
            this.Text = "Grupos de Equipos";
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void Equipo_Click(object sender, EventArgs e)
        {
            Label lblEquipo = (Label)sender;
            int numPartido=Math.Abs(obtenNumPartido(lblEquipo));
            Color colorBase;
            Color colorNuevo;
            switch(numPartido)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 9:
                case 10:
                case 11:
                    colorBase=Color.LemonChiffon;
                    colorNuevo=Color.PaleTurquoise;
                    break;
                default:
                    colorBase=Color.AntiqueWhite;
                    colorNuevo=Color.Yellow;
                    break;
            }

            if(lblEquipo.BackColor == colorBase)
                lblEquipo.BackColor = colorNuevo;
            else
                lblEquipo.BackColor = colorBase;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            CambiaGESelecionado( noGEPantalla - 1 );
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if( TieneGEDatos() )
            {
                CambiaGESelecionado( noGEPantalla + 1 );
            }
        }

        private void btnEliminarGrupo_Click(object sender, EventArgs e)
        {
            //si es la primera columna
            if(noGEPantalla == 0)
            {
                //solo borrar si la CP ya esta guardada en memoria
                if(arrayGE.Count > 0)
                {
                    BorrarGE( noGEPantalla );
                }							
            }
            else
            {
                BorrarGE( noGEPantalla );
                noGEPantalla = noGEPantalla -1;			
            }	
		
            if(arrayGE.Count == 0)
            {
                //Pueden existir datos en pantalla que tenemos que borrar.
                //Inicializar ge y asignar. Al no cambiar de CP en pantalla,
                //los datos de esta columna en blanco apareceran en pantalla.
                GrupoEquipos ge = new GrupoEquipos();
                arrayGE.Add( ge );			
            }
			
            ActualizaDatosPantalla( noGEPantalla );
        }

        private void btnPrevRel_Click(object sender, EventArgs e)
        {
            CambiaRelGE1Selecionado( relGE1Pantalla - 1 );
        }

        private void btnNexRel_Click(object sender, EventArgs e)
        {
            if( TieneRelacion1Datos() )
            {
                CambiaRelGE1Selecionado( relGE1Pantalla + 1 );
            }
        }

        private void btnEliminaGERel_Click(object sender, EventArgs e)
        {
            //si es la primera relacion
            if(relGE1Pantalla == 0)
            {
                //solo borrar si la relacion ya esta guardada en memoria
                if(arrayRelaciones1.Count > 0)
                {
                    BorrarRel1( relGE1Pantalla );
                }							
            }
            else
            {
                BorrarRel1( relGE1Pantalla );
                relGE1Pantalla = relGE1Pantalla -1;			
            }	
		
            if(arrayRelaciones1.Count == 0)
            {
                //Pueden existir datos en pantalla que tenemos que borrar.
                //Inicializar rel y asignar. Al no cambiar de rel en pantalla,
                //los datos de esta rel en blanco apareceran en pantalla.
                RelacionGE1 rel = new RelacionGE1();				
                arrayRelaciones1.Add( rel );			
            }
			
            ActualizaDatosPantRel1( relGE1Pantalla );
        }

        private void btnPrev_EnabledChanged(object sender, EventArgs e)
        {
            FormulariosHelper f=new FormulariosHelper();
            f.CambiarFondoBoton(btnPrev);
        }

        private void btnPrevRel_EnabledChanged(object sender, EventArgs e)
        {
            FormulariosHelper f=new FormulariosHelper();
            f.CambiarFondoBoton(btnPrevRel);
        }

        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            GuardarDatos();
            GuardarDatosRelacionesGE1();
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtroGE);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            GuardarDatos();
            if(filtroGE.GruposEquipos.Count>0)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Grupos de equipos(*.geq)|*.geq|Grupos de equipos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            GuardarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Grupos de equipos(*.geq)|*.geq|Grupos de equipos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo ))
            {
                grupo=archComb.LeeCondicion();
                filtroGE=(FiltroGruposEquipos)grupo.GetFiltro("GruposEquipos");
                arrayGE = ObtenCopiaArrayGE( filtroGE );
                noGEPantalla=0;
                relGE1Pantalla = 0;
                ActualizaDatosPantalla( noGEPantalla );
                InicializaDatosRelacionesGE();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            if(filtroGE.GruposEquipos.Count>0)
            {
                filtroGE.ContieneDatos=true;
                filtroGE.IsActive=true;
            }
            archComb.GuardaArchivo(filtroGE);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            GuardarDatos();
            if(filtroGE.ContieneDatos)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtroGE=new FiltroGruposEquipos();
            arrayGE = ObtenCopiaArrayGE( filtroGE );
            ActualizaDatosPantalla( 0 );
            InicializaDatosRelacionesGE();
            ActualizaDatosPantRel1( 0 );
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            GuardarDatos();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.geq";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            GuardarDatos();
            if(filtroGE.GruposEquipos.Count>0)			
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.geq";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if(formHelper.ExisteFicheroTemporal("tmp.geq"))
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
            FiltroGruposEquipos filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
