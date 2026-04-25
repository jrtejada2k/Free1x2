// created on 11/11/2003 at 23:10
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
using System.Data;

using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.UI.Controls;
using Free1X2.EntradaSalida;
using System.IO;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    public class ColProbablesFrm : Form
    {
        #region Propiedades privadas
        private Label label;
        private GroupBox groupBox;
        private OptionNumTol0_14 optACS;
        private Button btnNextCP;
        private Label lblNoCP;
        private Button btnPrevCP;
        private OptionNumTol0_14 optAC;
        private OptionNumTol0_14 optFS;
        private Button btnImportar;
        private Label label3;
        private Label label2;
        private Button btnCopiarValores;

        private Grupo grupo;
        private List<ColumnaProbable> grupoCP;
        private int cpPantalla = 0;
        private List<RelacionCP1> arrayRelaciones1;
        private List<RelacionCP2> arrayRelaciones2;
        private List<RelacionCP3> arrayRelaciones3;
        private int relCP1Pantalla = 0;
        private OptionNumsHoriz optACTol;
        private OptionNumsHoriz optACSTol;
        private OptionNumsHoriz optFSTol;
        private Label label1;
        private Label label5;
        private Label label6;
        private Label tol1;
        private Label tol2;
        private Label tol3;
        private Label tol0;
        private Label label4;
        private GroupBox groupBox1;
        private TabControl tabControl1;
        private TabPage tabColumnas;
        private TabPage tabRelaciones1;

        private FiltroColProbables filtroCP;
        private Button btnCopiarCols;
        private Button btnEliminarActual;


        private TabPage tabControlFallos;
        private DataGrid dgControlFallos;

        protected List<CPControlFallos> arrayControlesFallo;
        private Label label21;
        private TextBox txtPuntos;
        private Button btnPuntuacion;
        private Label label22;
        private TextBox txtFallosCtrl;
        private TextBox txtColumnas;
        private TextBox txtCuantasCP;
        private TextBox txtNoAciertos;
        private TextBox txtSumaAC;
        private TextBox txtRecorridoAC;
        private Label label23;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label lblNoRel1;
        private Button btnPrevRel1;
        private Button btnNextRel1;
        private Button btnEliminarRel1;
        private Label label28;
        private Button btnNext3CP;
        private Button btnFinCP;
        private Button btnPrev3CP;
        private Button btnInicioCP;
        private MenuCondiciones menuCondiciones1;
        private ctrlAyuda ctrlAyuda1;
        protected FormulariosHelper formHelper = new FormulariosHelper();

        private TabPage tabRelaciones2;
        private Label label29;
        private TextBox txtColsA;
        private Label label30;
        private Label label31;
        private ComboBox cbbMasMenos;
        private TextBox txtColsB;
        private Label label32;
        private ComboBox cbbConcepto;
        private TextBox txtNumAc;
        private GroupBox groupBox5;
        private GroupBox groupBox4;
        private Button btnRel2Atras;
        private Button btnRel2Adelante;
        private Label lblRel2Paginacion;
        protected int indiceNavRel2 = 1; //Es uno porque es la de la pantalla, que se guardara o no
        protected int indiceNavRel3 = 1;
        protected int paginaPantallaRelaciones2 = 1;
        protected int paginaPantallaRelaciones3 = 1;
        private Label label33;
        private GroupBox groupBox6;
        private Label label34;
        private Label label35;
        private TextBox txtColsA2;
        private ComboBox cbbConcepto2;
        private Label label36;
        private TextBox txtNumAc2;
        private Label label37;
        private TextBox txtColsB2;
        private ComboBox cbbMasMenos2;
        private Label label38;
        private Button btnEliminaRel2;
        private MainForm parentFrm;
        private TabPage tabRelaciones3;
        private Label label44;
        private ComboBox cbbConceptoRel3;
        private GroupBox groupBox11;
        private DataGrid dgAgrpacionesSolapadas;
        private GroupBox groupBox10;
        private DataGrid dgAgrupacionesPasoFijo;
        private GroupBox groupBox7;
        private Label label46;
        private TextBox txtNoSandwichs;
        private Label label40;
        private TextBox txtColsImplicadas;
        private GroupBox groupBox9;
        private Label label43;
        private TextBox txtNoEscalerasDesc;
        private Label label41;
        private TextBox txtNoEscalerasAsc;
        private Label label42;
        private TextBox txtNoEscaleras;
        private GroupBox groupBox8;
        private Button btnAtrasRelacion3;
        private Button btnAdelanteRelacion3;
        private Label lblRel3Paginacion;

        //Relaciones III

        private Button btnEliminaRel3;
        private GroupBox groupBox12;
        private Button btnBorraPronosticos;
        private Button btnExportador;
        private Button btnATriple;

        #endregion

        public ColProbablesFrm(Grupo grupo, MainForm frm)
        {
            InitializeComponent();
            AñadirPartidos();
            parentFrm = frm;
            this.grupo = grupo;

            //columnas probables tab
            InicializaDatos();
            //relaciones 1 tab
            InicializaDatosRelacionesCP();

            //control de fallos tab
            InicializaGridControlFallos();
            InicializaDatosControlFallos();

            compruebaPegar();
            //activa/desactiva botones de movimiento
            ActivarBotones(0);
            ActualizaDatosPantalla(0);
            ctrlAyuda1.TextoAyuda = "Una Columna Probable es un segundo\npronóstico sobre un grupo de partidos,\nexpresando la posibilidad de que se\nden ciertos resultados";
            AdaptarControlesDesplazamientoRelaciones2();
            AdaptarControlesDesplazamientoRelaciones3();
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

       
        protected void AñadirPartidos()
        {
            int noP;
            try
            {
                noP = VariablesGlobales.NumeroPartidos;
            }
            catch
            {
                noP = 14;
            }

            int x = 24;
            int y = 10;
            for (int i = 0; i < noP; i++)
            {
                Prono1X2 prono = new Prono1X2();
                Label label = new Label();
                //Prono
                prono.Name = "prono" + Convert.ToString(i + 1);
                prono.Location = new System.Drawing.Point(x, y);
                //Label
                label.BackColor = System.Drawing.Color.Bisque;
                label.Size = new System.Drawing.Size(20, 20);
                label.Text = Convert.ToString(i + 1);
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                label.Location = new System.Drawing.Point(x - label.Size.Width - 1, y);

                tabColumnas.Controls.Add(prono);
                tabColumnas.Controls.Add(label);

                y += prono.Height + 1;
            }
            if (noP > 14)
            {
                tabControl1.Size = new System.Drawing.Size(Size.Width, y + 30);
                Size = new System.Drawing.Size(Size.Width, y + 120);
                menuCondiciones1.Location = new System.Drawing.Point(menuCondiciones1.Location.X, y + 40);

            }
        }

        public MainForm FormPadre
        {
            get { return parentFrm; }
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.cps"))
                menuCondiciones1.BotonPegarEnabled = true;
            else
                menuCondiciones1.BotonPegarEnabled = false;
        }

        #region columnas
        protected void InicializaDatos()
        {
            string nombreFiltro = Filtro.ColProbables.ToString();
            filtroCP = (FiltroColProbables)grupo.GetFiltro(nombreFiltro);

            //grupoCP = filtroCP.ColProbables;
            grupoCP = ObtenCopiaCP(filtroCP);
            ActualizaDatosPantalla(cpPantalla);
        }

        protected List<ColumnaProbable> ObtenCopiaCP(FiltroColProbables filtro)
        {
            List<ColumnaProbable> arrayCP_copia = new List<ColumnaProbable>();

            foreach (ColumnaProbable cp in filtro.ColProbables)
            {
                ColumnaProbable cp_copia = new ColumnaProbable();

                cp_copia.Pronosticos = cp.Pronosticos;
                cp_copia.SetNoAciertos(cp.GetAciertos());
                cp_copia.SetNoAciertosSeguidos(cp.GetAciertosSeguidos());
                cp_copia.SetNoFallosSeguidos(cp.GetFallosSeguidos());
                cp_copia.SetPuntos(cp.GetPuntos());

                if (cp.ToleranciaLocalActiva)
                {
                    cp_copia.SetACTol(cp.GetACTol());
                    cp_copia.SetACSTol(cp.GetACSTol());
                    cp_copia.SetFSTol(cp.GetFSTol());
                    cp_copia.SetTolerancias(cp.GetTolerancias());
                }

                arrayCP_copia.Add(cp_copia);
            }

            return arrayCP_copia;
        }

        public void CambiaCPSelecionado()
        {
            CambiaCPSelecionado(cpPantalla);
        }

        protected void ActualizaDatosPantalla(int noCP)
        {
            ColumnaProbable cp;
            if (grupoCP.Count > 0)
            {
                cp = grupoCP[noCP];
                lblNoCP.Text = (noCP + 1) + "/" + grupoCP.Count;
            }
            else
            {
                cp = new ColumnaProbable();
                lblNoCP.Text = "1/1";
            }
            string[] pronosticos = cp.Pronosticos;
            Prono1X2 prono;
            for (int i = 0; i < tabColumnas.Controls.Count; i++)
            {
                prono = tabColumnas.Controls[i] as Prono1X2;
                if (prono != null)
                {
                    int valor = Convert.ToInt32(prono.Name.Substring(5));
                    prono.Pronostico = pronosticos[valor - 1];
                }
            }

            optAC.Valores = cp.GetAciertos();
            optACS.Valores = cp.GetAciertosSeguidos();
            optFS.Valores = cp.GetFallosSeguidos();

            txtPuntos.Text = cp.GetPuntos();

            optACTol.Valores = cp.GetACTol();
            optACSTol.Valores = cp.GetACSTol();
            optFSTol.Valores = cp.GetFSTol();

            ToleranciaValores = cp.GetTolerancias();
        }
        protected Prono1X2 BuscarControlProno1X2(int partido)
        {
            Prono1X2 prono = new Prono1X2();
            string nombre = "prono" + partido;
            for (int i = 0; i < tabColumnas.Controls.Count; i++)
            {
                prono = tabColumnas.Controls[i] as Prono1X2;
                if (prono != null)
                {
                    if (prono.Name == nombre)
                    {
                        break;
                    }
                }
            }
            return prono;
        }

        protected void BorrarCP(int noCP)
        {
            grupoCP.RemoveAt(noCP);

        }

        protected bool NecesitaBorrarUltimaCP()
        {
            bool borrar = false;

            ColumnaProbable cp = grupoCP[grupoCP.Count - 1];

            string[] prono = cp.Pronosticos;

            if (prono[0].Equals("") && prono[1].Equals("") &&
                prono[2].Equals("") && prono[3].Equals("") &&
                prono[4].Equals("") && prono[5].Equals("") &&
                prono[6].Equals("") && prono[7].Equals("") &&
                prono[8].Equals("") && prono[9].Equals("") &&
                prono[10].Equals("") && prono[11].Equals("") &&
                prono[12].Equals("") && prono[13].Equals(""))
            {
                borrar = true;
            }

            return borrar;
        }
        protected bool NecesitaBorrarUltimaCPTemporal(List<ColumnaProbable> cols)
        {
            bool borrar = false;

            ColumnaProbable cp = cols[cols.Count - 1];

            string[] prono = cp.Pronosticos;

            if (prono[0].Equals("") && prono[1].Equals("") &&
                prono[2].Equals("") && prono[3].Equals("") &&
                prono[4].Equals("") && prono[5].Equals("") &&
                prono[6].Equals("") && prono[7].Equals("") &&
                prono[8].Equals("") && prono[9].Equals("") &&
                prono[10].Equals("") && prono[11].Equals("") &&
                prono[12].Equals("") && prono[13].Equals(""))
            {
                borrar = true;
            }

            return borrar;
        }

        protected void GuardarDatos()
        {
            DataSet miDataset = (DataSet)dgControlFallos.DataSource;
            miDataset.AcceptChanges();
            GuardarCPActual();

            if (grupoCP.Count > 0)
            {
                //borrar ultima CP si no contiene datos
                if (NecesitaBorrarUltimaCP())
                {
                    BorrarCP(grupoCP.Count - 1);

                }
            }

            if (filtroCP.ContieneDatos == false && grupoCP.Count > 0)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                filtroCP.ContieneDatos = true;
                filtroCP.IsActive = true;
            }

            //guardar copia actualizada de CP en filtro
            filtroCP.ColProbables = grupoCP;
        }

        protected void GuardarCPActual()
        {
            ColumnaProbable cp;

            if (cpPantalla < grupoCP.Count)
            {
                cp = grupoCP[cpPantalla];
                GuardaDatosCP(cp);
            }
            else if (TieneColumnaDatos())
            {
                //existen datos en pantalla que necesitan se poner en nueva CP

                //crear CP y poner en grupo
                cp = new ColumnaProbable();
                grupoCP.Add(cp);

                GuardaDatosCP(cp);
            }
        }

        protected bool NecesitaGuardarDatosTol()
        {
            bool necesitaGuardar = true;

            if (optACTol.Valores == "" && optACSTol.Valores == ""
                && optFSTol.Valores == "")
            {
                necesitaGuardar = false;
            }

            return necesitaGuardar;
        }

        protected void GuardaDatosCP(ColumnaProbable cp)
        {

            //borra antiguos valores
            cp.ReinicializaValores();

            string[] pronosticos = cp.Pronosticos;
            for (int i = 0; i < pronosticos.Length; i++)
            {
                Prono1X2 prono = BuscarControlProno1X2(i + 1);
                pronosticos[i] = prono.Pronostico;
            }

            cp.Pronosticos = pronosticos;

            string todosValores = formHelper.ObtenerTodosValores();

            //guardar valores si cp tiene pronosticos
            if (TieneColumnaDatos())
            {
                cp.SetPuntos(txtPuntos.Text);

                if (optAC.Valores != "")
                {
                    cp.SetNoAciertos(optAC.Valores);
                }
                else
                {
                    cp.SetNoAciertos(todosValores);
                }

                if (optACS.Valores != "")
                {
                    cp.SetNoAciertosSeguidos(optACS.Valores);
                }
                else
                {
                    cp.SetNoAciertosSeguidos(todosValores);
                }

                if (optFS.Valores != "")
                {
                    cp.SetNoFallosSeguidos(optFS.Valores);
                }
                else
                {
                    cp.SetNoFallosSeguidos(todosValores);
                }

                if (NecesitaGuardarDatosTol())
                {
                    if (optACTol.Valores == "")
                    {
                        cp.SetACTol(todosValores);
                    }
                    else
                    {
                        cp.SetACTol(optACTol.Valores);
                    }

                    if (optACSTol.Valores == "")
                    {
                        cp.SetACSTol(todosValores);
                    }
                    else
                    {
                        cp.SetACSTol(optACSTol.Valores);
                    }

                    if (optFSTol.Valores == "")
                    {
                        cp.SetFSTol(todosValores);
                    }
                    else
                    {
                        cp.SetFSTol(optFSTol.Valores);
                    }

                    if (ToleranciaValores == "")
                    {
                        cp.SetTolerancias("0");
                    }
                    else
                    {
                        cp.SetTolerancias(ToleranciaValores);
                    }
                }
            }
            else
            {
                grupoCP.RemoveAt(cpPantalla);
            }
        }

        protected bool NecesitaGuardarDatos()
        {
            bool necesitaGuardar = true;

            if (optAC.Valores == "" && optACS.Valores == "" && optFS.Valores == "")
            {
                necesitaGuardar = false;
            }

            return necesitaGuardar;
        }

        protected bool TieneColumnaDatos()
        {
            bool contieneValores = false;

            if (cpPantalla < grupoCP.Count)
            {
                for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
                {
                    Prono1X2 p = BuscarControlProno1X2(i + 1);
                    if (p != null)
                    {
                        if (p.Pronostico != "")
                        {
                            contieneValores = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                //No se ha guardado ninguna columna, comprobar la primera que es la
                //de la pantalla. Debe haber npartidos, recorrerlos
                for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
                {
                    Prono1X2 p = BuscarControlProno1X2(i + 1);
                    if (p != null)
                    {
                        if (p.Pronostico != "")
                        {
                            contieneValores = true;
                            break;
                        }
                    }
                }
            }

            return contieneValores;
        }

        protected void CambiaCPSelecionado(int noCP)
        {
            // Para los desplazamientos en grupos de 3, vigilamos que 
            // exista la CP. Si no va a la primera o última.
            if (noCP < 0) noCP = 0;
            if (noCP > (grupoCP.Count)) noCP = grupoCP.Count;
            //primero guardar datos de pantalla
            GuardarCPActual();


            cpPantalla = noCP;

            //crear CP si no existe 			
            if (grupoCP.Count < noCP + 1)
            {
                ColumnaProbable cp = new ColumnaProbable();
                grupoCP.Add(cp);
            }
            //activa/desactiva botones de movimiento
            ActivarBotones(noCP);
            ActualizaDatosPantalla(noCP);
        }

        private void ActivarBotones(int noCP)
        {
            //activa/desactiva los botones de movimiento
            // Anterior
            if (noCP == 0)
                btnPrevCP.Enabled = false;
            else
                btnPrevCP.Enabled = true;
            // 3 atrás
            if (noCP <= (VariablesGlobales.Desplazamiento - 1) || grupoCP.Count <= VariablesGlobales.Desplazamiento)
                btnPrev3CP.Enabled = false;
            else
                btnPrev3CP.Enabled = true;
            // 3 delante
            if ((noCP + VariablesGlobales.Desplazamiento) < grupoCP.Count || grupoCP.Count < VariablesGlobales.Desplazamiento)
                btnNext3CP.Enabled = true;
            else
                btnNext3CP.Enabled = false;
            // Primero y último
            if (grupoCP.Count <= 1)
            {
                btnInicioCP.Enabled = false;
                btnFinCP.Enabled = false;
            }
            else
            {
                btnInicioCP.Enabled = true;
                btnFinCP.Enabled = true;
            }
        }

        protected void ImportaColumnas()
        {
            List<ColumnaProbable> grupoCPtmp = new List<ColumnaProbable>();
            List<ColumnaProbable> grupoCP2 = ObtenCopiaCP(filtroCP);
            if (grupoCP.Count > 0)
            {
                //borrar ultima CP si no contiene datos
                if (NecesitaBorrarUltimaCP())
                {
                    BorrarCP(grupoCP.Count - 1);
                }
            }

            //Inicializar grupoCPtmp

            ImportadorCPsFrm importador = new ImportadorCPsFrm(grupoCPtmp);
            importador.ShowDialog();
            if (grupoCPtmp.Count > 0)
            {
                // Si el nº de columnas en pantalla coincide con el nº de columnas
                // importadas, preguntamos si quiere sustituir las CPs (manteniendo
                // los rangos de aciertos).
                bool sustituirCPs = false;
                bool AgregarCPs = false;
                if (grupoCPtmp.Count == grupoCP.Count && grupoCP.Count > 0)
                {
                    if (MessageBox.Show("¿Sustituir las columnas existentes por las importadas (manteniendo los rangos de aciertos y tolerancias)?", "Importar columnas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        sustituirCPs = true;
                }
                if (sustituirCPs)
                {
                    for (int ncol = 0; ncol < grupoCP.Count; ncol++)
                    {
                        grupoCP2.Add(grupoCP[ncol]);
                    }
                    grupoCP.Clear();
                    for (int ncol = 0; ncol < grupoCPtmp.Count; ncol++)
                    {
                        ColumnaProbable cp = grupoCP2[ncol];
                        ColumnaProbable cpTmp = grupoCPtmp[ncol];
                        cp.Pronosticos = cpTmp.Pronosticos;
                        grupoCP.Add(cp);
                    }
                }
                else
                {
                    if (grupoCP.Count > 0)
                    {
                        if (MessageBox.Show("¿Añadir las columnas importadas al final de las existentes? \r\n(Si se selecciona No, se sustituirán TODAS las columnas y sus rangos por las columnas importadas)", "Importar columnas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            AgregarCPs = true;
                    }
                    if (AgregarCPs)
                    {
                        for (int ncol = 0; ncol < grupoCPtmp.Count; ncol++)
                        {
                            grupoCP.Add(grupoCPtmp[ncol]);
                        }
                    }
                    else
                    {
                        grupoCP.Clear();
                        for (int ncol = 0; ncol < grupoCPtmp.Count; ncol++)
                        {
                            grupoCP.Add(grupoCPtmp[ncol]);
                        }
                    }
                }
            }
            ActualizaDatosPantalla(cpPantalla);
            //activa/desactiva botones de movimiento
            ActivarBotones(cpPantalla);
        }

        protected void ExportaColumnas()
        {
            if (NecesitaGuardarDatos())
            {
                GuardarCPActual();
            }
            FiltroColProbables filtroTemp = new FiltroColProbables();
            filtroTemp.ColProbables = grupoCP;
            List<ColumnaProbable> grupoCP2 = filtroTemp.ColProbables;

            ExportadorCPsFrm exportador = new ExportadorCPsFrm(grupoCP2);
            exportador.ShowDialog();
        }
        protected void CopiaValoresCP()
        {
            if (grupoCP.Count == 0) return;
            string valoresAC = optAC.Valores;
            string valoresACS = optACS.Valores;
            string valoresFS = optFS.Valores;

            string todosValores = formHelper.ObtenerTodosValores();

            if (valoresAC == "")
            {
                valoresAC = todosValores;
            }

            if (valoresACS == "")
            {
                valoresACS = todosValores;
            }

            if (valoresFS == "")
            {
                valoresFS = todosValores;
            }
            // Selecciona el rango de CPs a copiar
            CopiarDatosCPFrm f = new CopiarDatosCPFrm(cpPantalla + 1, grupoCP.Count);
            f.ShowDialog();
            int min = f.Desde - 1;
            if (min < 0) return;
            int max = f.Hasta;
            ColumnaProbable cp;
            for (int i = min; i < max; i++)
            {
                cp = grupoCP[i];
                cp.ReinicializaValores();
                cp.SetNoAciertos(valoresAC);
                cp.SetNoAciertosSeguidos(valoresACS);
                cp.SetNoFallosSeguidos(valoresFS);
            }
            // Repite la operación para la columna actual si no se ha hecho ya.
            if (cpPantalla < min || cpPantalla > max)
            {
                cp = grupoCP[cpPantalla];
                cp.ReinicializaValores();
                cp.SetNoAciertos(valoresAC);
                cp.SetNoAciertosSeguidos(valoresACS);
                cp.SetNoFallosSeguidos(valoresFS);
            }

            ActualizaDatosPantalla(cpPantalla);
        }
        private void BorraPronosticosColumnaPantalla()
        {
            if (grupoCP.Count > cpPantalla)
            {
                ColumnaProbable cp = grupoCP[cpPantalla];

                cp.BorraPronosticos();
            }

            ActualizaDatosPantalla(cpPantalla);
        }

        private void PonerTodosATriple()
        {
            if (grupoCP.Count > cpPantalla)
            {
                ColumnaProbable cp = grupoCP[cpPantalla];

                cp.TodosATriple();
            }
            else
            {
                ColumnaProbable cp = new ColumnaProbable();
                grupoCP.Add(cp);

                cp = grupoCP[cpPantalla];

                cp.TodosATriple();
            }

            ActualizaDatosPantalla(cpPantalla);
        }


        protected string ToleranciaValores
        {
            get
            {
                string valores = "";

                if (tol0.BackColor == System.Drawing.Color.LightGreen)
                {
                    valores = "0";
                }

                if (tol1.BackColor == System.Drawing.Color.LightGreen)
                {
                    if (!valores.Equals(""))
                    {
                        valores += ",";
                    }
                    valores += "1";
                }
                if (tol2.BackColor == System.Drawing.Color.LightGreen)
                {
                    if (!valores.Equals(""))
                    {
                        valores += ",";
                    }
                    valores += "2";
                }
                if (tol3.BackColor == System.Drawing.Color.LightGreen)
                {
                    if (!valores.Equals(""))
                    {
                        valores += ",";
                    }
                    valores += "3";
                }

                return valores;
            }
            set
            {
                string valores = value;
                string[] valArray = valores.Split(',');

                tol0.BackColor = System.Drawing.Color.Wheat;
                tol1.BackColor = System.Drawing.Color.Wheat;
                tol2.BackColor = System.Drawing.Color.Wheat;
                tol3.BackColor = System.Drawing.Color.Wheat;

                foreach (string val in valArray)
                {
                    switch (val)
                    {
                        case "0":
                            tol0.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "1":
                            tol1.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "2":
                            tol2.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "3":
                            tol3.BackColor = System.Drawing.Color.LightGreen;
                            break;
                    }
                }

            }//end set

        } //end property

        #endregion

        #region tabRelaciones

        protected void InicializaDatosRelacionesCP()
        {
            arrayRelaciones1 = new List<RelacionCP1>();
            arrayRelaciones2 = new List<RelacionCP2>();
            arrayRelaciones3 = new List<RelacionCP3>();
            MostrarPrimeraRelacion2();
            MostrarPrimeraRelacion3();
            RelacionCP1 rel;

            //carga datos
            List<RelacionCP1> relacionesCP = filtroCP.RelacionesCP1.Relaciones;

            //guardar copia en UI 
            for (int i = 0; i < relacionesCP.Count; i++)
            {
                rel = new RelacionCP1();
                RelacionCP1 relGuardada = relacionesCP[i];

                rel.Columnas = relGuardada.Columnas;
                rel.SumaAciertos = relGuardada.SumaAciertos;
                rel.Recorridos = relGuardada.Recorridos;
                rel.CantidadCP = relGuardada.CantidadCP;
                rel.CuantosAC = relGuardada.CuantosAC;

                arrayRelaciones1.Add(rel);
            }

            ActualizaDatosPantRel1(relCP1Pantalla);

        }

        protected void ActualizaDatosPantRel1(int relCP1)
        {

            if (arrayRelaciones1.Count > 0)
            {
                RelacionCP1 rel = arrayRelaciones1[relCP1];
                txtColumnas.Text = rel.Columnas;
                txtRecorridoAC.Text = rel.Recorridos;
                txtSumaAC.Text = rel.SumaAciertos;
                txtCuantasCP.Text = rel.CantidadCP;
                txtNoAciertos.Text = rel.CuantosAC;
                lblNoRel1.Text = (relCP1 + 1) + "/" + arrayRelaciones1.Count;
            }
            else
            {
                txtColumnas.Text = "";
                txtRecorridoAC.Text = "";
                txtSumaAC.Text = "";
                txtCuantasCP.Text = "";
                txtNoAciertos.Text = "";
                lblNoRel1.Text = "1/1";
            }

        }

        protected bool TieneRelacion1Datos()
        {
            bool contieneDatos = true;

            if (txtColumnas.Text == "")
            {
                contieneDatos = false;
            }
            else if (txtSumaAC.Text == "" &&
                     txtRecorridoAC.Text == "" &&
                     txtCuantasCP.Text == "")
            {
                contieneDatos = false;
            }

            return contieneDatos;
        }

        public void CambiaRelCP1Selecionado()
        {
            CambiaRelCP1Selecionado(relCP1Pantalla);
        }

        protected void CambiaRelCP1Selecionado(int relCP1)
        {
            //primero guardar datos de pantalla
            GuardarRelCP1Actual();
            relCP1Pantalla = relCP1;

            //crear rel1 si no existe 

            if (arrayRelaciones1.Count < relCP1 + 1)
            {
                RelacionCP1 rel = new RelacionCP1();
                arrayRelaciones1.Add(rel);
            }


            //activa/desactiva boton "atras" si estamos en la primera relacion
            if (relCP1 == 0)
            {
                btnPrevRel1.Enabled = false;
            }
            else
            {
                btnPrevRel1.Enabled = true;
            }

            ActualizaDatosPantRel1(relCP1Pantalla);
        }

        protected void GuardarRelCP1Actual()
        {
            RelacionCP1 rel;

            if (relCP1Pantalla < arrayRelaciones1.Count)
            {
                rel = arrayRelaciones1[relCP1Pantalla];
                GuardaDatosRel1(rel);
            }
            else if (TieneRelacion1Datos())
            {
                //existen datos en pantalla que se necesitan poner en nueva rel
                rel = new RelacionCP1();
                arrayRelaciones1.Add(rel);

                GuardaDatosRel1(rel);
            }
        }


        protected void GuardarDatosRelacionesCP1()
        {
            GuardarRelCP1Actual();

            if (arrayRelaciones1.Count > 0)
            {
                //borrar ultima relacion si no contiene datos
                if (NecesitaBorrarUltimaRel1())
                {
                    BorrarRel1(arrayRelaciones1.Count - 1);
                }
            }

            List<RelacionCP1> relacionesCPFinal = new List<RelacionCP1>();

            for (int i = 0; i < arrayRelaciones1.Count; i++)
            {
                RelacionCP1 rel = arrayRelaciones1[i];

                if (rel.Columnas != "")
                {
                    relacionesCPFinal.Add(rel);
                }
            }

            filtroCP.RelacionesCP1.Relaciones = relacionesCPFinal;

        }
        protected void GuardaDatosRel1(RelacionCP1 rel)
        {
            if (TieneRelacion1Datos())
            {
                rel.Columnas = txtColumnas.Text;
                rel.SumaAciertos = txtSumaAC.Text;
                rel.Recorridos = txtRecorridoAC.Text;
                rel.CantidadCP = txtCuantasCP.Text;
                rel.CuantosAC = txtNoAciertos.Text;
            }
        }

        protected void BorrarRel1(int noRelCP1)
        {
            arrayRelaciones1.RemoveAt(noRelCP1);

        }

        protected bool NecesitaBorrarUltimaRel1()
        {
            bool borrar = false;

            RelacionCP1 rel = arrayRelaciones1[arrayRelaciones1.Count - 1];

            if (rel.Columnas == "")
            {
                borrar = true;
            }
            else if (rel.SumaAciertos == "" &&
                     rel.Recorridos == "" &&
                     rel.CantidadCP == "")
            {
                borrar = true;
            }

            return borrar;
        }


        #endregion

        #region tabControlFallos

        protected void InicializaGridControlFallos()
        {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = "CPControlFallos";
            tableStyle.ColumnHeadersVisible = true;

            //Columnas
            DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
            cs.MappingName = "Columnas";
            cs.HeaderText = "Columnas";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            //Tolerancias
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Tolerancias";
            cs.HeaderText = "Tolerancias";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            //Aciertos
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Aciertos";
            cs.HeaderText = "Aciertos";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            dgControlFallos.TableStyles.Add(tableStyle);

        }


        protected void InicializaDatosControlFallos()
        {
            arrayControlesFallo = new List<CPControlFallos>();

            CPControlFallos ctrFallos;
            CPControlFallos ctrFallosGuardada;

            //carga datos
            txtFallosCtrl.Text = filtroCP.ControlFallosCP.FallosPermitidos;
            List<CPControlFallos> controlesFallo = filtroCP.ControlFallosCP.ControlesFallos;

            for (int i = 0; i < controlesFallo.Count; i++)
            {
                ctrFallosGuardada = controlesFallo[i];

                ctrFallos = new CPControlFallos();

                ctrFallos.Columnas = ctrFallosGuardada.Columnas;
                ctrFallos.Aciertos = ctrFallosGuardada.Aciertos;
                ctrFallos.Tolerancias = ctrFallosGuardada.Tolerancias;

                arrayControlesFallo.Add(ctrFallos);
            }

            //si hay menos de 50 filas crear...
            if (arrayControlesFallo.Count < 50)
            {
                //crea lineas en blanco
                int noLineas = 50 - arrayControlesFallo.Count;

                for (int i = 0; i < 50; i++)
                {
                    ctrFallos = new CPControlFallos();
                    arrayControlesFallo.Add(ctrFallos);
                }
            }

            DataSet miDataset = ObtenDataSetControlFallos(arrayControlesFallo);
            dgControlFallos.SetDataBinding(miDataset, "CPControlFallos");
        }

        protected DataSet ObtenDataSetControlFallos(List<CPControlFallos> controlesFallo)
        {
            DataTable myDataTable = new DataTable("CPControlFallos");

            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = System.Type.GetType("System.String");
            myDataColumn.ColumnName = "Columnas";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = System.Type.GetType("System.String");
            myDataColumn.ColumnName = "Tolerancias";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = System.Type.GetType("System.String");
            myDataColumn.ColumnName = "Aciertos";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            DataSet myDataSet = new DataSet();
            // Add the new DataTable to the DataSet.
            myDataSet.Tables.Add(myDataTable);


            //meter datos en el dataset
            for (int i = 0; i < controlesFallo.Count; i++)
            {
                DataRow myDataRow = myDataTable.NewRow();
                myDataRow["Columnas"] = ((CPControlFallos)controlesFallo[i]).Columnas;
                myDataRow["Tolerancias"] = ((CPControlFallos)controlesFallo[i]).Tolerancias;
                myDataRow["Aciertos"] = ((CPControlFallos)controlesFallo[i]).Aciertos;

                myDataTable.Rows.Add(myDataRow);
            }

            return myDataSet;
        }


        protected List<CPControlFallos> ObtenArrayControlFallos(DataSet datosDS)
        {
            List<CPControlFallos> arrayListDatos = new List<CPControlFallos>();

            foreach (DataRow row in datosDS.Tables["CPControlFallos"].Rows)
            {
                if (row["Columnas"].ToString() != "")
                {
                    CPControlFallos ctrFallos = new CPControlFallos();

                    ctrFallos.Columnas = row["Columnas"].ToString();
                    ctrFallos.Tolerancias = row["Tolerancias"].ToString();
                    ctrFallos.Aciertos = row["Aciertos"].ToString();

                    arrayListDatos.Add(ctrFallos);
                }
            }

            return arrayListDatos;
        }


        protected void GuardarDatosControlFallos()
        {
            DataSet miDataset = (DataSet)dgControlFallos.DataSource;

            arrayControlesFallo = ObtenArrayControlFallos(miDataset);

            List<CPControlFallos> arrayControlesFalloFinal = new List<CPControlFallos>();

            for (int i = 0; i < arrayControlesFallo.Count; i++)
            {
                CPControlFallos ctrFallos = arrayControlesFallo[i];

                if (ctrFallos.Columnas != "")
                {
                    arrayControlesFalloFinal.Add(ctrFallos);
                }
            }

            filtroCP.ControlFallosCP.ControlesFallos = arrayControlesFalloFinal;
            filtroCP.ControlFallosCP.FallosPermitidos = txtFallosCtrl.Text;
        }

        #endregion


        void BtnImportarClick(object sender, EventArgs e)
        {
            ImportaColumnas();
        }

        void BtnCopiarValoresClick(object sender, EventArgs e)
        {
            CopiaValoresCP();
        }

        #region designer
        void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColProbablesFrm));
            this.btnCopiarValores = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnImportar = new System.Windows.Forms.Button();
            this.btnPrevCP = new System.Windows.Forms.Button();
            this.lblNoCP = new System.Windows.Forms.Label();
            this.btnNextCP = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.btnPrev3CP = new System.Windows.Forms.Button();
            this.btnInicioCP = new System.Windows.Forms.Button();
            this.btnFinCP = new System.Windows.Forms.Button();
            this.btnNext3CP = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tol1 = new System.Windows.Forms.Label();
            this.tol2 = new System.Windows.Forms.Label();
            this.tol3 = new System.Windows.Forms.Label();
            this.tol0 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.optACTol = new Free1X2.UI.Controls.OptionNumsHoriz();
            this.optACSTol = new Free1X2.UI.Controls.OptionNumsHoriz();
            this.optFSTol = new Free1X2.UI.Controls.OptionNumsHoriz();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabColumnas = new System.Windows.Forms.TabPage();
            this.btnExportador = new System.Windows.Forms.Button();
            this.btnATriple = new System.Windows.Forms.Button();
            this.btnBorraPronosticos = new System.Windows.Forms.Button();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.btnPuntuacion = new System.Windows.Forms.Button();
            this.txtPuntos = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.btnEliminarActual = new System.Windows.Forms.Button();
            this.btnCopiarCols = new System.Windows.Forms.Button();
            this.optACS = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.optFS = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.optAC = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.tabRelaciones1 = new System.Windows.Forms.TabPage();
            this.btnEliminarRel1 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnPrevRel1 = new System.Windows.Forms.Button();
            this.btnNextRel1 = new System.Windows.Forms.Button();
            this.lblNoRel1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.txtCuantasCP = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtNoAciertos = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtRecorridoAC = new System.Windows.Forms.TextBox();
            this.txtSumaAC = new System.Windows.Forms.TextBox();
            this.txtColumnas = new System.Windows.Forms.TextBox();
            this.tabRelaciones2 = new System.Windows.Forms.TabPage();
            this.btnEliminaRel2 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.txtColsA2 = new System.Windows.Forms.TextBox();
            this.cbbConcepto2 = new System.Windows.Forms.ComboBox();
            this.label36 = new System.Windows.Forms.Label();
            this.txtNumAc2 = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.txtColsB2 = new System.Windows.Forms.TextBox();
            this.cbbMasMenos2 = new System.Windows.Forms.ComboBox();
            this.label38 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.txtColsA = new System.Windows.Forms.TextBox();
            this.cbbConcepto = new System.Windows.Forms.ComboBox();
            this.label30 = new System.Windows.Forms.Label();
            this.txtNumAc = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.txtColsB = new System.Windows.Forms.TextBox();
            this.cbbMasMenos = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnRel2Atras = new System.Windows.Forms.Button();
            this.btnRel2Adelante = new System.Windows.Forms.Button();
            this.lblRel2Paginacion = new System.Windows.Forms.Label();
            this.tabRelaciones3 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txtColsImplicadas = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.cbbConceptoRel3 = new System.Windows.Forms.ComboBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.dgAgrpacionesSolapadas = new System.Windows.Forms.DataGrid();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.dgAgrupacionesPasoFijo = new System.Windows.Forms.DataGrid();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label46 = new System.Windows.Forms.Label();
            this.txtNoSandwichs = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label43 = new System.Windows.Forms.Label();
            this.txtNoEscalerasDesc = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.txtNoEscalerasAsc = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.txtNoEscaleras = new System.Windows.Forms.TextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnAtrasRelacion3 = new System.Windows.Forms.Button();
            this.btnEliminaRel3 = new System.Windows.Forms.Button();
            this.btnAdelanteRelacion3 = new System.Windows.Forms.Button();
            this.lblRel3Paginacion = new System.Windows.Forms.Label();
            this.tabControlFallos = new System.Windows.Forms.TabPage();
            this.txtFallosCtrl = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.dgControlFallos = new System.Windows.Forms.DataGrid();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.groupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabColumnas.SuspendLayout();
            this.tabRelaciones1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabRelaciones2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabRelaciones3.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAgrpacionesSolapadas)).BeginInit();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAgrupacionesPasoFijo)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabControlFallos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgControlFallos)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCopiarValores
            // 
            this.btnCopiarValores.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCopiarValores.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopiarValores.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiarValores.Image = ((System.Drawing.Image)(resources.GetObject("btnCopiarValores.Image")));
            this.btnCopiarValores.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopiarValores.Location = new System.Drawing.Point(608, 170);
            this.btnCopiarValores.Name = "btnCopiarValores";
            this.btnCopiarValores.Size = new System.Drawing.Size(134, 24);
            this.btnCopiarValores.TabIndex = 11;
            this.btnCopiarValores.Text = "Copiar datos";
            this.btnCopiarValores.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCopiarValores.UseVisualStyleBackColor = false;
            this.btnCopiarValores.Click += new System.EventHandler(this.BtnCopiarValoresClick);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(96, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ac. Seg";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(96, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Fallos Seg";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnImportar
            // 
            this.btnImportar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnImportar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnImportar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportar.Image = ((System.Drawing.Image)(resources.GetObject("btnImportar.Image")));
            this.btnImportar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportar.Location = new System.Drawing.Point(608, 120);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(134, 24);
            this.btnImportar.TabIndex = 10;
            this.btnImportar.Text = "Importador CPs";
            this.btnImportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportar.UseVisualStyleBackColor = false;
            this.btnImportar.Click += new System.EventHandler(this.BtnImportarClick);
            // 
            // btnPrevCP
            // 
            this.btnPrevCP.BackColor = System.Drawing.Color.Silver;
            this.btnPrevCP.Enabled = false;
            this.btnPrevCP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrevCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnPrevCP.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevCP.Image")));
            this.btnPrevCP.Location = new System.Drawing.Point(56, 16);
            this.btnPrevCP.Name = "btnPrevCP";
            this.btnPrevCP.Size = new System.Drawing.Size(24, 23);
            this.btnPrevCP.TabIndex = 6;
            this.btnPrevCP.UseVisualStyleBackColor = false;
            this.btnPrevCP.Click += new System.EventHandler(this.BtnPrevCPClick);
            this.btnPrevCP.EnabledChanged += new System.EventHandler(this.btnPrevCP_EnabledChanged);
            // 
            // lblNoCP
            // 
            this.lblNoCP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblNoCP.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoCP.ForeColor = System.Drawing.Color.Black;
            this.lblNoCP.Location = new System.Drawing.Point(80, 16);
            this.lblNoCP.Name = "lblNoCP";
            this.lblNoCP.Size = new System.Drawing.Size(96, 23);
            this.lblNoCP.TabIndex = 8;
            this.lblNoCP.Text = "0/0";
            this.lblNoCP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNextCP
            // 
            this.btnNextCP.BackColor = System.Drawing.Color.LightSalmon;
            this.btnNextCP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNextCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnNextCP.Image = ((System.Drawing.Image)(resources.GetObject("btnNextCP.Image")));
            this.btnNextCP.Location = new System.Drawing.Point(176, 16);
            this.btnNextCP.Name = "btnNextCP";
            this.btnNextCP.Size = new System.Drawing.Size(24, 23);
            this.btnNextCP.TabIndex = 7;
            this.btnNextCP.UseVisualStyleBackColor = false;
            this.btnNextCP.Click += new System.EventHandler(this.BtnNextCPClick);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.btnPrev3CP);
            this.groupBox.Controls.Add(this.btnInicioCP);
            this.groupBox.Controls.Add(this.btnFinCP);
            this.groupBox.Controls.Add(this.btnNext3CP);
            this.groupBox.Controls.Add(this.lblNoCP);
            this.groupBox.Controls.Add(this.btnPrevCP);
            this.groupBox.Controls.Add(this.btnNextCP);
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox.Location = new System.Drawing.Point(96, 8);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(256, 48);
            this.groupBox.TabIndex = 9;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "CP";
            // 
            // btnPrev3CP
            // 
            this.btnPrev3CP.BackColor = System.Drawing.Color.Silver;
            this.btnPrev3CP.Enabled = false;
            this.btnPrev3CP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrev3CP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnPrev3CP.Image = ((System.Drawing.Image)(resources.GetObject("btnPrev3CP.Image")));
            this.btnPrev3CP.Location = new System.Drawing.Point(31, 16);
            this.btnPrev3CP.Name = "btnPrev3CP";
            this.btnPrev3CP.Size = new System.Drawing.Size(24, 23);
            this.btnPrev3CP.TabIndex = 12;
            this.btnPrev3CP.UseVisualStyleBackColor = false;
            this.btnPrev3CP.Click += new System.EventHandler(this.btnPrev3CP_Click);
            this.btnPrev3CP.EnabledChanged += new System.EventHandler(this.btnPrev3CP_EnabledChanged);
            // 
            // btnInicioCP
            // 
            this.btnInicioCP.BackColor = System.Drawing.Color.Silver;
            this.btnInicioCP.Enabled = false;
            this.btnInicioCP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInicioCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnInicioCP.Image = ((System.Drawing.Image)(resources.GetObject("btnInicioCP.Image")));
            this.btnInicioCP.Location = new System.Drawing.Point(6, 16);
            this.btnInicioCP.Name = "btnInicioCP";
            this.btnInicioCP.Size = new System.Drawing.Size(24, 23);
            this.btnInicioCP.TabIndex = 11;
            this.btnInicioCP.UseVisualStyleBackColor = false;
            this.btnInicioCP.Click += new System.EventHandler(this.btnInicioCP_Click);
            this.btnInicioCP.EnabledChanged += new System.EventHandler(this.btnInicioCP_EnabledChanged);
            // 
            // btnFinCP
            // 
            this.btnFinCP.BackColor = System.Drawing.Color.Silver;
            this.btnFinCP.Enabled = false;
            this.btnFinCP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFinCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnFinCP.Image = ((System.Drawing.Image)(resources.GetObject("btnFinCP.Image")));
            this.btnFinCP.Location = new System.Drawing.Point(226, 16);
            this.btnFinCP.Name = "btnFinCP";
            this.btnFinCP.Size = new System.Drawing.Size(24, 23);
            this.btnFinCP.TabIndex = 10;
            this.btnFinCP.UseVisualStyleBackColor = false;
            this.btnFinCP.Click += new System.EventHandler(this.btnFinCP_Click);
            this.btnFinCP.EnabledChanged += new System.EventHandler(this.btnFinCP_EnabledChanged);
            // 
            // btnNext3CP
            // 
            this.btnNext3CP.BackColor = System.Drawing.Color.Silver;
            this.btnNext3CP.Enabled = false;
            this.btnNext3CP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNext3CP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.btnNext3CP.Image = ((System.Drawing.Image)(resources.GetObject("btnNext3CP.Image")));
            this.btnNext3CP.Location = new System.Drawing.Point(201, 16);
            this.btnNext3CP.Name = "btnNext3CP";
            this.btnNext3CP.Size = new System.Drawing.Size(24, 23);
            this.btnNext3CP.TabIndex = 9;
            this.btnNext3CP.UseVisualStyleBackColor = false;
            this.btnNext3CP.Click += new System.EventHandler(this.btnNext3CP_Click);
            this.btnNext3CP.EnabledChanged += new System.EventHandler(this.btnNext3CP_EnabledChanged);
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(96, 64);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(64, 16);
            this.label.TabIndex = 1;
            this.label.Text = "Aciertos";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 16;
            this.label1.Text = "Aciertos";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(16, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 17;
            this.label5.Text = "Ac. Seg";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(16, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 16);
            this.label6.TabIndex = 18;
            this.label6.Text = "Fallos Seg";
            // 
            // tol1
            // 
            this.tol1.BackColor = System.Drawing.Color.Wheat;
            this.tol1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tol1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tol1.ForeColor = System.Drawing.Color.Black;
            this.tol1.Location = new System.Drawing.Point(158, 90);
            this.tol1.Name = "tol1";
            this.tol1.Size = new System.Drawing.Size(16, 16);
            this.tol1.TabIndex = 20;
            this.tol1.Text = "1";
            this.tol1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tol1.Click += new System.EventHandler(this.tol1_Click);
            // 
            // tol2
            // 
            this.tol2.BackColor = System.Drawing.Color.Wheat;
            this.tol2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tol2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tol2.ForeColor = System.Drawing.Color.Black;
            this.tol2.Location = new System.Drawing.Point(175, 90);
            this.tol2.Name = "tol2";
            this.tol2.Size = new System.Drawing.Size(16, 16);
            this.tol2.TabIndex = 21;
            this.tol2.Text = "2";
            this.tol2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tol2.Click += new System.EventHandler(this.tol2_Click);
            // 
            // tol3
            // 
            this.tol3.BackColor = System.Drawing.Color.Wheat;
            this.tol3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tol3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tol3.ForeColor = System.Drawing.Color.Black;
            this.tol3.Location = new System.Drawing.Point(192, 90);
            this.tol3.Name = "tol3";
            this.tol3.Size = new System.Drawing.Size(16, 16);
            this.tol3.TabIndex = 22;
            this.tol3.Text = "3";
            this.tol3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tol3.Click += new System.EventHandler(this.tol3_Click);
            // 
            // tol0
            // 
            this.tol0.BackColor = System.Drawing.Color.Wheat;
            this.tol0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tol0.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tol0.ForeColor = System.Drawing.Color.Black;
            this.tol0.Location = new System.Drawing.Point(141, 90);
            this.tol0.Name = "tol0";
            this.tol0.Size = new System.Drawing.Size(16, 16);
            this.tol0.TabIndex = 23;
            this.tol0.Text = "0";
            this.tol0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tol0.Click += new System.EventHandler(this.tol0_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(16, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 16);
            this.label4.TabIndex = 24;
            this.label4.Text = "Núm Tolerancias";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.optACTol);
            this.groupBox1.Controls.Add(this.optACSTol);
            this.groupBox1.Controls.Add(this.optFSTol);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tol2);
            this.groupBox1.Controls.Add(this.tol3);
            this.groupBox1.Controls.Add(this.tol0);
            this.groupBox1.Controls.Add(this.tol1);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(96, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 120);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tolerancias locales";
            // 
            // optACTol
            // 
            this.optACTol.BackColor = System.Drawing.Color.Bisque;
            this.optACTol.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optACTol.ForeColor = System.Drawing.Color.Black;
            this.optACTol.Location = new System.Drawing.Point(80, 24);
            this.optACTol.Name = "optACTol";
            this.optACTol.Size = new System.Drawing.Size(420, 16);
            this.optACTol.TabIndex = 13;
            this.optACTol.Valores = "";
            // 
            // optACSTol
            // 
            this.optACSTol.BackColor = System.Drawing.Color.Bisque;
            this.optACSTol.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optACSTol.ForeColor = System.Drawing.Color.Black;
            this.optACSTol.Location = new System.Drawing.Point(80, 41);
            this.optACSTol.Name = "optACSTol";
            this.optACSTol.Size = new System.Drawing.Size(420, 16);
            this.optACSTol.TabIndex = 14;
            this.optACSTol.Valores = "";
            // 
            // optFSTol
            // 
            this.optFSTol.BackColor = System.Drawing.Color.Bisque;
            this.optFSTol.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optFSTol.ForeColor = System.Drawing.Color.Black;
            this.optFSTol.Location = new System.Drawing.Point(80, 58);
            this.optFSTol.Name = "optFSTol";
            this.optFSTol.Size = new System.Drawing.Size(420, 16);
            this.optFSTol.TabIndex = 15;
            this.optFSTol.Valores = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabColumnas);
            this.tabControl1.Controls.Add(this.tabRelaciones1);
            this.tabControl1.Controls.Add(this.tabRelaciones2);
            this.tabControl1.Controls.Add(this.tabRelaciones3);
            this.tabControl1.Controls.Add(this.tabControlFallos);
            this.tabControl1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(785, 370);
            this.tabControl1.TabIndex = 26;
            // 
            // tabColumnas
            // 
            this.tabColumnas.BackColor = System.Drawing.Color.Bisque;
            this.tabColumnas.Controls.Add(this.btnExportador);
            this.tabColumnas.Controls.Add(this.btnATriple);
            this.tabColumnas.Controls.Add(this.btnBorraPronosticos);
            this.tabColumnas.Controls.Add(this.ctrlAyuda1);
            this.tabColumnas.Controls.Add(this.btnPuntuacion);
            this.tabColumnas.Controls.Add(this.txtPuntos);
            this.tabColumnas.Controls.Add(this.label21);
            this.tabColumnas.Controls.Add(this.btnEliminarActual);
            this.tabColumnas.Controls.Add(this.btnCopiarCols);
            this.tabColumnas.Controls.Add(this.groupBox);
            this.tabColumnas.Controls.Add(this.btnCopiarValores);
            this.tabColumnas.Controls.Add(this.btnImportar);
            this.tabColumnas.Controls.Add(this.optACS);
            this.tabColumnas.Controls.Add(this.label);
            this.tabColumnas.Controls.Add(this.label2);
            this.tabColumnas.Controls.Add(this.label3);
            this.tabColumnas.Controls.Add(this.optFS);
            this.tabColumnas.Controls.Add(this.optAC);
            this.tabColumnas.Controls.Add(this.groupBox1);
            this.tabColumnas.Location = new System.Drawing.Point(4, 22);
            this.tabColumnas.Name = "tabColumnas";
            this.tabColumnas.Size = new System.Drawing.Size(777, 344);
            this.tabColumnas.TabIndex = 0;
            this.tabColumnas.Text = "Columnas";
            this.tabColumnas.UseVisualStyleBackColor = true;
            this.tabColumnas.Click += new System.EventHandler(this.tabColumnas_Click);
            // 
            // btnExportador
            // 
            this.btnExportador.BackColor = System.Drawing.Color.LightSalmon;
            this.btnExportador.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExportador.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportador.Image = ((System.Drawing.Image)(resources.GetObject("btnExportador.Image")));
            this.btnExportador.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportador.Location = new System.Drawing.Point(608, 145);
            this.btnExportador.Name = "btnExportador";
            this.btnExportador.Size = new System.Drawing.Size(134, 24);
            this.btnExportador.TabIndex = 48;
            this.btnExportador.Text = "Exportador CPs";
            this.btnExportador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportador.UseVisualStyleBackColor = false;
            this.btnExportador.Click += new System.EventHandler(this.btnExportador_Click);
            // 
            // btnATriple
            // 
            this.btnATriple.BackColor = System.Drawing.Color.LightSalmon;
            this.btnATriple.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnATriple.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnATriple.Image = ((System.Drawing.Image)(resources.GetObject("btnATriple.Image")));
            this.btnATriple.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnATriple.Location = new System.Drawing.Point(96, 270);
            this.btnATriple.Name = "btnATriple";
            this.btnATriple.Size = new System.Drawing.Size(135, 23);
            this.btnATriple.TabIndex = 47;
            this.btnATriple.Text = "Todos a Triple";
            this.btnATriple.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnATriple.UseVisualStyleBackColor = false;
            this.btnATriple.Click += new System.EventHandler(this.btnATriple_Click);
            // 
            // btnBorraPronosticos
            // 
            this.btnBorraPronosticos.BackColor = System.Drawing.Color.LightSalmon;
            this.btnBorraPronosticos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBorraPronosticos.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBorraPronosticos.Image = ((System.Drawing.Image)(resources.GetObject("btnBorraPronosticos.Image")));
            this.btnBorraPronosticos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorraPronosticos.Location = new System.Drawing.Point(96, 246);
            this.btnBorraPronosticos.Name = "btnBorraPronosticos";
            this.btnBorraPronosticos.Size = new System.Drawing.Size(135, 23);
            this.btnBorraPronosticos.TabIndex = 46;
            this.btnBorraPronosticos.Text = "Borrar Pronósticos";
            this.btnBorraPronosticos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBorraPronosticos.UseVisualStyleBackColor = false;
            this.btnBorraPronosticos.Click += new System.EventHandler(this.button1_Click);
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(750, 3);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(16, 16);
            this.ctrlAyuda1.TabIndex = 45;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // btnPuntuacion
            // 
            this.btnPuntuacion.BackColor = System.Drawing.Color.LightSalmon;
            this.btnPuntuacion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPuntuacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPuntuacion.Image = ((System.Drawing.Image)(resources.GetObject("btnPuntuacion.Image")));
            this.btnPuntuacion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPuntuacion.Location = new System.Drawing.Point(557, 24);
            this.btnPuntuacion.Name = "btnPuntuacion";
            this.btnPuntuacion.Size = new System.Drawing.Size(166, 24);
            this.btnPuntuacion.TabIndex = 44;
            this.btnPuntuacion.Text = "Cambiar Puntuación";
            this.btnPuntuacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPuntuacion.UseVisualStyleBackColor = false;
            this.btnPuntuacion.Click += new System.EventHandler(this.btnPuntuacion_Click);
            // 
            // txtPuntos
            // 
            this.txtPuntos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPuntos.Location = new System.Drawing.Point(423, 24);
            this.txtPuntos.Name = "txtPuntos";
            this.txtPuntos.Size = new System.Drawing.Size(104, 21);
            this.txtPuntos.TabIndex = 43;
            // 
            // label21
            // 
            this.label21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(367, 24);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(48, 21);
            this.label21.TabIndex = 42;
            this.label21.Text = "Puntos";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnEliminarActual
            // 
            this.btnEliminarActual.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminarActual.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminarActual.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarActual.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminarActual.Image")));
            this.btnEliminarActual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminarActual.Location = new System.Drawing.Point(608, 220);
            this.btnEliminarActual.Name = "btnEliminarActual";
            this.btnEliminarActual.Size = new System.Drawing.Size(134, 24);
            this.btnEliminarActual.TabIndex = 27;
            this.btnEliminarActual.Text = "Eliminar actual";
            this.btnEliminarActual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminarActual.UseVisualStyleBackColor = false;
            this.btnEliminarActual.Click += new System.EventHandler(this.btnEliminarActual_Click);
            // 
            // btnCopiarCols
            // 
            this.btnCopiarCols.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCopiarCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopiarCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiarCols.Image = ((System.Drawing.Image)(resources.GetObject("btnCopiarCols.Image")));
            this.btnCopiarCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopiarCols.Location = new System.Drawing.Point(608, 195);
            this.btnCopiarCols.Name = "btnCopiarCols";
            this.btnCopiarCols.Size = new System.Drawing.Size(134, 24);
            this.btnCopiarCols.TabIndex = 26;
            this.btnCopiarCols.Text = "Copiar Columnas";
            this.btnCopiarCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCopiarCols.UseVisualStyleBackColor = false;
            this.btnCopiarCols.Click += new System.EventHandler(this.btnCopiarCols_Click);
            // 
            // optACS
            // 
            this.optACS.BackColor = System.Drawing.Color.Wheat;
            this.optACS.Location = new System.Drawing.Point(160, 81);
            this.optACS.Maximo = 15;
            this.optACS.Minimo = 0;
            this.optACS.Name = "optACS";
            this.optACS.Size = new System.Drawing.Size(563, 16);
            this.optACS.TabIndex = 0;
            this.optACS.Valores = "";
            // 
            // optFS
            // 
            this.optFS.BackColor = System.Drawing.Color.Wheat;
            this.optFS.Location = new System.Drawing.Point(160, 98);
            this.optFS.Maximo = 15;
            this.optFS.Minimo = 0;
            this.optFS.Name = "optFS";
            this.optFS.Size = new System.Drawing.Size(563, 16);
            this.optFS.TabIndex = 0;
            this.optFS.Valores = "";
            // 
            // optAC
            // 
            this.optAC.BackColor = System.Drawing.Color.Wheat;
            this.optAC.Location = new System.Drawing.Point(160, 64);
            this.optAC.Maximo = 15;
            this.optAC.Minimo = 0;
            this.optAC.Name = "optAC";
            this.optAC.Size = new System.Drawing.Size(563, 16);
            this.optAC.TabIndex = 0;
            this.optAC.Valores = "";
            // 
            // tabRelaciones1
            // 
            this.tabRelaciones1.BackColor = System.Drawing.Color.Bisque;
            this.tabRelaciones1.Controls.Add(this.btnEliminarRel1);
            this.tabRelaciones1.Controls.Add(this.groupBox3);
            this.tabRelaciones1.Controls.Add(this.groupBox2);
            this.tabRelaciones1.Controls.Add(this.label27);
            this.tabRelaciones1.Controls.Add(this.label26);
            this.tabRelaciones1.Controls.Add(this.label23);
            this.tabRelaciones1.Controls.Add(this.txtRecorridoAC);
            this.tabRelaciones1.Controls.Add(this.txtSumaAC);
            this.tabRelaciones1.Controls.Add(this.txtColumnas);
            this.tabRelaciones1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabRelaciones1.Location = new System.Drawing.Point(4, 22);
            this.tabRelaciones1.Name = "tabRelaciones1";
            this.tabRelaciones1.Size = new System.Drawing.Size(777, 344);
            this.tabRelaciones1.TabIndex = 1;
            this.tabRelaciones1.Text = "Relaciones I";
            this.tabRelaciones1.UseVisualStyleBackColor = true;
            // 
            // btnEliminarRel1
            // 
            this.btnEliminarRel1.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminarRel1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminarRel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarRel1.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminarRel1.Image")));
            this.btnEliminarRel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminarRel1.Location = new System.Drawing.Point(192, 32);
            this.btnEliminarRel1.Name = "btnEliminarRel1";
            this.btnEliminarRel1.Size = new System.Drawing.Size(124, 23);
            this.btnEliminarRel1.TabIndex = 13;
            this.btnEliminarRel1.Text = "Eliminar Actual";
            this.btnEliminarRel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminarRel1.UseVisualStyleBackColor = false;
            this.btnEliminarRel1.Click += new System.EventHandler(this.btnEliminarRel1_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnPrevRel1);
            this.groupBox3.Controls.Add(this.btnNextRel1);
            this.groupBox3.Controls.Add(this.lblNoRel1);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(16, 16);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(160, 48);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Relaciones";
            // 
            // btnPrevRel1
            // 
            this.btnPrevRel1.BackColor = System.Drawing.Color.Silver;
            this.btnPrevRel1.Enabled = false;
            this.btnPrevRel1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrevRel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevRel1.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevRel1.Image")));
            this.btnPrevRel1.Location = new System.Drawing.Point(16, 16);
            this.btnPrevRel1.Name = "btnPrevRel1";
            this.btnPrevRel1.Size = new System.Drawing.Size(24, 23);
            this.btnPrevRel1.TabIndex = 13;
            this.btnPrevRel1.UseVisualStyleBackColor = false;
            this.btnPrevRel1.Click += new System.EventHandler(this.btnPrevRel1_Click);
            this.btnPrevRel1.EnabledChanged += new System.EventHandler(this.btnPrevRel1_EnabledChanged);
            // 
            // btnNextRel1
            // 
            this.btnNextRel1.BackColor = System.Drawing.Color.LightSalmon;
            this.btnNextRel1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNextRel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextRel1.Image = ((System.Drawing.Image)(resources.GetObject("btnNextRel1.Image")));
            this.btnNextRel1.Location = new System.Drawing.Point(118, 16);
            this.btnNextRel1.Name = "btnNextRel1";
            this.btnNextRel1.Size = new System.Drawing.Size(24, 23);
            this.btnNextRel1.TabIndex = 14;
            this.btnNextRel1.UseVisualStyleBackColor = false;
            this.btnNextRel1.Click += new System.EventHandler(this.btnNextRel1_Click);
            // 
            // lblNoRel1
            // 
            this.lblNoRel1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblNoRel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoRel1.ForeColor = System.Drawing.Color.Black;
            this.lblNoRel1.Location = new System.Drawing.Point(48, 16);
            this.lblNoRel1.Name = "lblNoRel1";
            this.lblNoRel1.Size = new System.Drawing.Size(64, 23);
            this.lblNoRel1.TabIndex = 13;
            this.lblNoRel1.Text = "0/0";
            this.lblNoRel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label28);
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.txtCuantasCP);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.txtNoAciertos);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(16, 104);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(330, 104);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // label28
            // 
            this.label28.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(14, 16);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(272, 23);
            this.label28.TabIndex = 9;
            this.label28.Text = "Introducir datos en ambas casillas";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(14, 48);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(136, 20);
            this.label24.TabIndex = 7;
            this.label24.Text = "¿Cuántas Columnas?";
            // 
            // txtCuantasCP
            // 
            this.txtCuantasCP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCuantasCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCuantasCP.Location = new System.Drawing.Point(159, 48);
            this.txtCuantasCP.Name = "txtCuantasCP";
            this.txtCuantasCP.Size = new System.Drawing.Size(160, 20);
            this.txtCuantasCP.TabIndex = 2;
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(12, 69);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(126, 20);
            this.label25.TabIndex = 8;
            this.label25.Text = "Número de Aciertos";
            // 
            // txtNoAciertos
            // 
            this.txtNoAciertos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNoAciertos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoAciertos.Location = new System.Drawing.Point(159, 69);
            this.txtNoAciertos.Name = "txtNoAciertos";
            this.txtNoAciertos.Size = new System.Drawing.Size(160, 20);
            this.txtNoAciertos.TabIndex = 3;
            // 
            // label27
            // 
            this.label27.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(24, 240);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(100, 21);
            this.label27.TabIndex = 10;
            this.label27.Text = "Recorrido";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label26
            // 
            this.label26.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(23, 216);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(100, 21);
            this.label26.TabIndex = 9;
            this.label26.Text = "Suma Aciertos";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label23
            // 
            this.label23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(16, 80);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(92, 21);
            this.label23.TabIndex = 6;
            this.label23.Text = "Columnas";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtRecorridoAC
            // 
            this.txtRecorridoAC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRecorridoAC.Location = new System.Drawing.Point(156, 238);
            this.txtRecorridoAC.Name = "txtRecorridoAC";
            this.txtRecorridoAC.Size = new System.Drawing.Size(160, 21);
            this.txtRecorridoAC.TabIndex = 5;
            // 
            // txtSumaAC
            // 
            this.txtSumaAC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSumaAC.Location = new System.Drawing.Point(156, 216);
            this.txtSumaAC.Name = "txtSumaAC";
            this.txtSumaAC.Size = new System.Drawing.Size(160, 21);
            this.txtSumaAC.TabIndex = 4;
            // 
            // txtColumnas
            // 
            this.txtColumnas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColumnas.Location = new System.Drawing.Point(114, 80);
            this.txtColumnas.Name = "txtColumnas";
            this.txtColumnas.Size = new System.Drawing.Size(232, 21);
            this.txtColumnas.TabIndex = 1;
            // 
            // tabRelaciones2
            // 
            this.tabRelaciones2.BackColor = System.Drawing.Color.Bisque;
            this.tabRelaciones2.Controls.Add(this.btnEliminaRel2);
            this.tabRelaciones2.Controls.Add(this.groupBox6);
            this.tabRelaciones2.Controls.Add(this.groupBox5);
            this.tabRelaciones2.Controls.Add(this.groupBox4);
            this.tabRelaciones2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tabRelaciones2.Location = new System.Drawing.Point(4, 22);
            this.tabRelaciones2.Name = "tabRelaciones2";
            this.tabRelaciones2.Padding = new System.Windows.Forms.Padding(3);
            this.tabRelaciones2.Size = new System.Drawing.Size(777, 344);
            this.tabRelaciones2.TabIndex = 3;
            this.tabRelaciones2.Text = "Relaciones II";
            // 
            // btnEliminaRel2
            // 
            this.btnEliminaRel2.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminaRel2.FlatAppearance.BorderSize = 0;
            this.btnEliminaRel2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminaRel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminaRel2.ForeColor = System.Drawing.Color.Black;
            this.btnEliminaRel2.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminaRel2.Image")));
            this.btnEliminaRel2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminaRel2.Location = new System.Drawing.Point(237, 22);
            this.btnEliminaRel2.Name = "btnEliminaRel2";
            this.btnEliminaRel2.Size = new System.Drawing.Size(133, 23);
            this.btnEliminaRel2.TabIndex = 21;
            this.btnEliminaRel2.Text = "Eliminar Actual";
            this.btnEliminaRel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminaRel2.UseVisualStyleBackColor = false;
            this.btnEliminaRel2.Click += new System.EventHandler(this.btnEliminaRel2_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label34);
            this.groupBox6.Controls.Add(this.label35);
            this.groupBox6.Controls.Add(this.txtColsA2);
            this.groupBox6.Controls.Add(this.cbbConcepto2);
            this.groupBox6.Controls.Add(this.label36);
            this.groupBox6.Controls.Add(this.txtNumAc2);
            this.groupBox6.Controls.Add(this.label37);
            this.groupBox6.Controls.Add(this.txtColsB2);
            this.groupBox6.Controls.Add(this.cbbMasMenos2);
            this.groupBox6.Controls.Add(this.label38);
            this.groupBox6.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox6.Location = new System.Drawing.Point(11, 162);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(707, 100);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Relaciones II - AC/ACS/FS individuales";
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Verdana", 7F);
            this.label34.ForeColor = System.Drawing.Color.Black;
            this.label34.Location = new System.Drawing.Point(13, 70);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(619, 20);
            this.label34.TabIndex = 18;
            this.label34.Text = "Los aciertos se refieren a los AC, ACS o FS INDIVIDUALES de CADA columna";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label35
            // 
            this.label35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ForeColor = System.Drawing.Color.Black;
            this.label35.Location = new System.Drawing.Point(13, 27);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(85, 20);
            this.label35.TabIndex = 9;
            this.label35.Text = "Las columnas";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtColsA2
            // 
            this.txtColsA2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColsA2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColsA2.ForeColor = System.Drawing.Color.Black;
            this.txtColsA2.Location = new System.Drawing.Point(101, 27);
            this.txtColsA2.Name = "txtColsA2";
            this.txtColsA2.Size = new System.Drawing.Size(106, 21);
            this.txtColsA2.TabIndex = 8;
            // 
            // cbbConcepto2
            // 
            this.cbbConcepto2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbbConcepto2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbConcepto2.ForeColor = System.Drawing.Color.Black;
            this.cbbConcepto2.FormattingEnabled = true;
            this.cbbConcepto2.Items.AddRange(new object[] {
            "AC",
            "ACS",
            "FS"});
            this.cbbConcepto2.Location = new System.Drawing.Point(336, 27);
            this.cbbConcepto2.Name = "cbbConcepto2";
            this.cbbConcepto2.Size = new System.Drawing.Size(45, 21);
            this.cbbConcepto2.TabIndex = 17;
            this.cbbConcepto2.Text = "AC";
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Verdana", 7F);
            this.label36.ForeColor = System.Drawing.Color.Black;
            this.label36.Location = new System.Drawing.Point(13, 50);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(619, 20);
            this.label36.TabIndex = 10;
            this.label36.Text = "(Introducir intervalo o columnas individuales: a-b ó a,c,d)";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNumAc2
            // 
            this.txtNumAc2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNumAc2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumAc2.ForeColor = System.Drawing.Color.Black;
            this.txtNumAc2.Location = new System.Drawing.Point(266, 27);
            this.txtNumAc2.Name = "txtNumAc2";
            this.txtNumAc2.Size = new System.Drawing.Size(66, 21);
            this.txtNumAc2.TabIndex = 16;
            // 
            // label37
            // 
            this.label37.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ForeColor = System.Drawing.Color.Black;
            this.label37.Location = new System.Drawing.Point(209, 27);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(55, 20);
            this.label37.TabIndex = 11;
            this.label37.Text = "tendrán";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtColsB2
            // 
            this.txtColsB2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColsB2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColsB2.ForeColor = System.Drawing.Color.Black;
            this.txtColsB2.Location = new System.Drawing.Point(568, 27);
            this.txtColsB2.Name = "txtColsB2";
            this.txtColsB2.Size = new System.Drawing.Size(106, 21);
            this.txtColsB2.TabIndex = 14;
            // 
            // cbbMasMenos2
            // 
            this.cbbMasMenos2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbbMasMenos2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbMasMenos2.ForeColor = System.Drawing.Color.Black;
            this.cbbMasMenos2.FormattingEnabled = true;
            this.cbbMasMenos2.Items.AddRange(new object[] {
            "Más",
            "Menos"});
            this.cbbMasMenos2.Location = new System.Drawing.Point(382, 27);
            this.cbbMasMenos2.Name = "cbbMasMenos2";
            this.cbbMasMenos2.Size = new System.Drawing.Size(59, 21);
            this.cbbMasMenos2.TabIndex = 12;
            this.cbbMasMenos2.Text = "Más";
            // 
            // label38
            // 
            this.label38.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.ForeColor = System.Drawing.Color.Black;
            this.label38.Location = new System.Drawing.Point(453, 27);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(112, 20);
            this.label38.TabIndex = 13;
            this.label38.Text = "que las columnas";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label33);
            this.groupBox5.Controls.Add(this.label29);
            this.groupBox5.Controls.Add(this.txtColsA);
            this.groupBox5.Controls.Add(this.cbbConcepto);
            this.groupBox5.Controls.Add(this.label30);
            this.groupBox5.Controls.Add(this.txtNumAc);
            this.groupBox5.Controls.Add(this.label31);
            this.groupBox5.Controls.Add(this.txtColsB);
            this.groupBox5.Controls.Add(this.cbbMasMenos);
            this.groupBox5.Controls.Add(this.label32);
            this.groupBox5.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox5.Location = new System.Drawing.Point(11, 56);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(707, 100);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Relaciones II - Sumas de AC/ACS/FS";
            // 
            // label33
            // 
            this.label33.Font = new System.Drawing.Font("Verdana", 7F);
            this.label33.ForeColor = System.Drawing.Color.Black;
            this.label33.Location = new System.Drawing.Point(13, 70);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(619, 20);
            this.label33.TabIndex = 18;
            this.label33.Text = "Los aciertos se refieren a la SUMA de AC, ACS o FS entre todas las columnas";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label29
            // 
            this.label29.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.Color.Black;
            this.label29.Location = new System.Drawing.Point(13, 25);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(85, 20);
            this.label29.TabIndex = 9;
            this.label29.Text = "Las columnas";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtColsA
            // 
            this.txtColsA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColsA.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColsA.ForeColor = System.Drawing.Color.Black;
            this.txtColsA.Location = new System.Drawing.Point(101, 25);
            this.txtColsA.Name = "txtColsA";
            this.txtColsA.Size = new System.Drawing.Size(106, 21);
            this.txtColsA.TabIndex = 8;
            // 
            // cbbConcepto
            // 
            this.cbbConcepto.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbbConcepto.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbConcepto.ForeColor = System.Drawing.Color.Black;
            this.cbbConcepto.FormattingEnabled = true;
            this.cbbConcepto.Items.AddRange(new object[] {
            "AC",
            "ACS",
            "FS"});
            this.cbbConcepto.Location = new System.Drawing.Point(336, 25);
            this.cbbConcepto.Name = "cbbConcepto";
            this.cbbConcepto.Size = new System.Drawing.Size(45, 21);
            this.cbbConcepto.TabIndex = 17;
            this.cbbConcepto.Text = "AC";
            // 
            // label30
            // 
            this.label30.Font = new System.Drawing.Font("Verdana", 7F);
            this.label30.ForeColor = System.Drawing.Color.Black;
            this.label30.Location = new System.Drawing.Point(13, 50);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(619, 20);
            this.label30.TabIndex = 10;
            this.label30.Text = "(Introducir intervalo o columnas individuales: a-b ó a,c,d)";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNumAc
            // 
            this.txtNumAc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNumAc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumAc.ForeColor = System.Drawing.Color.Black;
            this.txtNumAc.Location = new System.Drawing.Point(266, 25);
            this.txtNumAc.Name = "txtNumAc";
            this.txtNumAc.Size = new System.Drawing.Size(66, 21);
            this.txtNumAc.TabIndex = 16;
            // 
            // label31
            // 
            this.label31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.Black;
            this.label31.Location = new System.Drawing.Point(209, 25);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(55, 20);
            this.label31.TabIndex = 11;
            this.label31.Text = "tendrán";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtColsB
            // 
            this.txtColsB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColsB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColsB.ForeColor = System.Drawing.Color.Black;
            this.txtColsB.Location = new System.Drawing.Point(568, 25);
            this.txtColsB.Name = "txtColsB";
            this.txtColsB.Size = new System.Drawing.Size(106, 21);
            this.txtColsB.TabIndex = 14;
            // 
            // cbbMasMenos
            // 
            this.cbbMasMenos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbbMasMenos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbMasMenos.ForeColor = System.Drawing.Color.Black;
            this.cbbMasMenos.FormattingEnabled = true;
            this.cbbMasMenos.Items.AddRange(new object[] {
            "Más",
            "Menos"});
            this.cbbMasMenos.Location = new System.Drawing.Point(382, 25);
            this.cbbMasMenos.Name = "cbbMasMenos";
            this.cbbMasMenos.Size = new System.Drawing.Size(59, 21);
            this.cbbMasMenos.TabIndex = 12;
            this.cbbMasMenos.Text = "Más";
            // 
            // label32
            // 
            this.label32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ForeColor = System.Drawing.Color.Black;
            this.label32.Location = new System.Drawing.Point(453, 25);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(112, 20);
            this.label32.TabIndex = 13;
            this.label32.Text = "que las columnas";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnRel2Atras);
            this.groupBox4.Controls.Add(this.btnRel2Adelante);
            this.groupBox4.Controls.Add(this.lblRel2Paginacion);
            this.groupBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox4.Location = new System.Drawing.Point(11, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(137, 48);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Actual / Guardadas";
            // 
            // btnRel2Atras
            // 
            this.btnRel2Atras.BackColor = System.Drawing.Color.LightSalmon;
            this.btnRel2Atras.Enabled = false;
            this.btnRel2Atras.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRel2Atras.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRel2Atras.ForeColor = System.Drawing.Color.Black;
            this.btnRel2Atras.Image = ((System.Drawing.Image)(resources.GetObject("btnRel2Atras.Image")));
            this.btnRel2Atras.Location = new System.Drawing.Point(16, 16);
            this.btnRel2Atras.Name = "btnRel2Atras";
            this.btnRel2Atras.Size = new System.Drawing.Size(24, 23);
            this.btnRel2Atras.TabIndex = 13;
            this.btnRel2Atras.UseVisualStyleBackColor = false;
            this.btnRel2Atras.Click += new System.EventHandler(this.btnRel2Atras_Click);
            // 
            // btnRel2Adelante
            // 
            this.btnRel2Adelante.BackColor = System.Drawing.Color.LightSalmon;
            this.btnRel2Adelante.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRel2Adelante.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRel2Adelante.ForeColor = System.Drawing.Color.Black;
            this.btnRel2Adelante.Image = ((System.Drawing.Image)(resources.GetObject("btnRel2Adelante.Image")));
            this.btnRel2Adelante.Location = new System.Drawing.Point(81, 16);
            this.btnRel2Adelante.Name = "btnRel2Adelante";
            this.btnRel2Adelante.Size = new System.Drawing.Size(24, 23);
            this.btnRel2Adelante.TabIndex = 14;
            this.btnRel2Adelante.UseVisualStyleBackColor = false;
            this.btnRel2Adelante.Click += new System.EventHandler(this.btnRel2Adelante_Click);
            // 
            // lblRel2Paginacion
            // 
            this.lblRel2Paginacion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblRel2Paginacion.ForeColor = System.Drawing.Color.Black;
            this.lblRel2Paginacion.Location = new System.Drawing.Point(41, 15);
            this.lblRel2Paginacion.Name = "lblRel2Paginacion";
            this.lblRel2Paginacion.Size = new System.Drawing.Size(39, 24);
            this.lblRel2Paginacion.TabIndex = 13;
            this.lblRel2Paginacion.Text = "1 / 1";
            this.lblRel2Paginacion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabRelaciones3
            // 
            this.tabRelaciones3.BackColor = System.Drawing.Color.Bisque;
            this.tabRelaciones3.Controls.Add(this.groupBox12);
            this.tabRelaciones3.Controls.Add(this.groupBox11);
            this.tabRelaciones3.Controls.Add(this.groupBox10);
            this.tabRelaciones3.Controls.Add(this.groupBox7);
            this.tabRelaciones3.Controls.Add(this.groupBox9);
            this.tabRelaciones3.Controls.Add(this.groupBox8);
            this.tabRelaciones3.Location = new System.Drawing.Point(4, 22);
            this.tabRelaciones3.Name = "tabRelaciones3";
            this.tabRelaciones3.Size = new System.Drawing.Size(777, 344);
            this.tabRelaciones3.TabIndex = 4;
            this.tabRelaciones3.Text = "Relaciones III";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label40);
            this.groupBox12.Controls.Add(this.txtColsImplicadas);
            this.groupBox12.Controls.Add(this.label44);
            this.groupBox12.Controls.Add(this.cbbConceptoRel3);
            this.groupBox12.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox12.Location = new System.Drawing.Point(11, 57);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(105, 245);
            this.groupBox12.TabIndex = 29;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Columnas";
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ForeColor = System.Drawing.Color.Black;
            this.label40.Location = new System.Drawing.Point(20, 46);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(65, 20);
            this.label40.TabIndex = 9;
            this.label40.Text = "Columnas";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtColsImplicadas
            // 
            this.txtColsImplicadas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColsImplicadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColsImplicadas.ForeColor = System.Drawing.Color.Black;
            this.txtColsImplicadas.Location = new System.Drawing.Point(14, 69);
            this.txtColsImplicadas.Name = "txtColsImplicadas";
            this.txtColsImplicadas.Size = new System.Drawing.Size(76, 21);
            this.txtColsImplicadas.TabIndex = 8;
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ForeColor = System.Drawing.Color.Black;
            this.label44.Location = new System.Drawing.Point(20, 111);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(65, 20);
            this.label44.TabIndex = 27;
            this.label44.Text = "Concepto";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbbConceptoRel3
            // 
            this.cbbConceptoRel3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbbConceptoRel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbConceptoRel3.ForeColor = System.Drawing.Color.Black;
            this.cbbConceptoRel3.FormattingEnabled = true;
            this.cbbConceptoRel3.Items.AddRange(new object[] {
            "AC",
            "ACS",
            "FS"});
            this.cbbConceptoRel3.Location = new System.Drawing.Point(30, 134);
            this.cbbConceptoRel3.Name = "cbbConceptoRel3";
            this.cbbConceptoRel3.Size = new System.Drawing.Size(45, 21);
            this.cbbConceptoRel3.TabIndex = 26;
            this.cbbConceptoRel3.Text = "AC";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.dgAgrpacionesSolapadas);
            this.groupBox11.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox11.Location = new System.Drawing.Point(325, 157);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(272, 145);
            this.groupBox11.TabIndex = 25;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Agrupaciones de Aciertos Solapadas";
            // 
            // dgAgrpacionesSolapadas
            // 
            this.dgAgrpacionesSolapadas.AlternatingBackColor = System.Drawing.Color.OldLace;
            this.dgAgrpacionesSolapadas.BackColor = System.Drawing.Color.OldLace;
            this.dgAgrpacionesSolapadas.BackgroundColor = System.Drawing.Color.Bisque;
            this.dgAgrpacionesSolapadas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgAgrpacionesSolapadas.CaptionBackColor = System.Drawing.Color.SaddleBrown;
            this.dgAgrpacionesSolapadas.CaptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.dgAgrpacionesSolapadas.CaptionForeColor = System.Drawing.Color.OldLace;
            this.dgAgrpacionesSolapadas.CaptionVisible = false;
            this.dgAgrpacionesSolapadas.DataMember = "";
            this.dgAgrpacionesSolapadas.FlatMode = true;
            this.dgAgrpacionesSolapadas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.dgAgrpacionesSolapadas.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.dgAgrpacionesSolapadas.GridLineColor = System.Drawing.Color.Bisque;
            this.dgAgrpacionesSolapadas.HeaderBackColor = System.Drawing.Color.Wheat;
            this.dgAgrpacionesSolapadas.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 7.25F);
            this.dgAgrpacionesSolapadas.HeaderForeColor = System.Drawing.Color.SaddleBrown;
            this.dgAgrpacionesSolapadas.LinkColor = System.Drawing.Color.DarkSlateBlue;
            this.dgAgrpacionesSolapadas.Location = new System.Drawing.Point(6, 19);
            this.dgAgrpacionesSolapadas.Name = "dgAgrpacionesSolapadas";
            this.dgAgrpacionesSolapadas.ParentRowsBackColor = System.Drawing.Color.OldLace;
            this.dgAgrpacionesSolapadas.ParentRowsForeColor = System.Drawing.Color.DarkSlateGray;
            this.dgAgrpacionesSolapadas.PreferredColumnWidth = 69;
            this.dgAgrpacionesSolapadas.SelectionBackColor = System.Drawing.Color.SlateGray;
            this.dgAgrpacionesSolapadas.SelectionForeColor = System.Drawing.Color.White;
            this.dgAgrpacionesSolapadas.Size = new System.Drawing.Size(260, 117);
            this.dgAgrpacionesSolapadas.TabIndex = 1;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.dgAgrupacionesPasoFijo);
            this.groupBox10.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox10.Location = new System.Drawing.Point(325, 6);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(272, 145);
            this.groupBox10.TabIndex = 24;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Agrupaciones de Aciertos Paso Fijo";
            // 
            // dgAgrupacionesPasoFijo
            // 
            this.dgAgrupacionesPasoFijo.AlternatingBackColor = System.Drawing.Color.OldLace;
            this.dgAgrupacionesPasoFijo.BackColor = System.Drawing.Color.OldLace;
            this.dgAgrupacionesPasoFijo.BackgroundColor = System.Drawing.Color.Bisque;
            this.dgAgrupacionesPasoFijo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgAgrupacionesPasoFijo.CaptionBackColor = System.Drawing.Color.SaddleBrown;
            this.dgAgrupacionesPasoFijo.CaptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.dgAgrupacionesPasoFijo.CaptionForeColor = System.Drawing.Color.OldLace;
            this.dgAgrupacionesPasoFijo.CaptionVisible = false;
            this.dgAgrupacionesPasoFijo.DataMember = "";
            this.dgAgrupacionesPasoFijo.FlatMode = true;
            this.dgAgrupacionesPasoFijo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.dgAgrupacionesPasoFijo.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.dgAgrupacionesPasoFijo.GridLineColor = System.Drawing.Color.Bisque;
            this.dgAgrupacionesPasoFijo.HeaderBackColor = System.Drawing.Color.Wheat;
            this.dgAgrupacionesPasoFijo.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 7.25F);
            this.dgAgrupacionesPasoFijo.HeaderForeColor = System.Drawing.Color.SaddleBrown;
            this.dgAgrupacionesPasoFijo.LinkColor = System.Drawing.Color.DarkSlateBlue;
            this.dgAgrupacionesPasoFijo.Location = new System.Drawing.Point(6, 19);
            this.dgAgrupacionesPasoFijo.Name = "dgAgrupacionesPasoFijo";
            this.dgAgrupacionesPasoFijo.ParentRowsBackColor = System.Drawing.Color.OldLace;
            this.dgAgrupacionesPasoFijo.ParentRowsForeColor = System.Drawing.Color.DarkSlateGray;
            this.dgAgrupacionesPasoFijo.PreferredColumnWidth = 69;
            this.dgAgrupacionesPasoFijo.SelectionBackColor = System.Drawing.Color.SlateGray;
            this.dgAgrupacionesPasoFijo.SelectionForeColor = System.Drawing.Color.White;
            this.dgAgrupacionesPasoFijo.Size = new System.Drawing.Size(260, 117);
            this.dgAgrupacionesPasoFijo.TabIndex = 2;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label46);
            this.groupBox7.Controls.Add(this.txtNoSandwichs);
            this.groupBox7.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox7.Location = new System.Drawing.Point(127, 57);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(192, 94);
            this.groupBox7.TabIndex = 23;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Sandwichs";
            // 
            // label46
            // 
            this.label46.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.ForeColor = System.Drawing.Color.Black;
            this.label46.Location = new System.Drawing.Point(34, 37);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(52, 21);
            this.label46.TabIndex = 9;
            this.label46.Text = "Total";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNoSandwichs
            // 
            this.txtNoSandwichs.AcceptsReturn = true;
            this.txtNoSandwichs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNoSandwichs.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoSandwichs.ForeColor = System.Drawing.Color.Black;
            this.txtNoSandwichs.Location = new System.Drawing.Point(92, 37);
            this.txtNoSandwichs.Name = "txtNoSandwichs";
            this.txtNoSandwichs.Size = new System.Drawing.Size(95, 21);
            this.txtNoSandwichs.TabIndex = 8;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label43);
            this.groupBox9.Controls.Add(this.txtNoEscalerasDesc);
            this.groupBox9.Controls.Add(this.label41);
            this.groupBox9.Controls.Add(this.txtNoEscalerasAsc);
            this.groupBox9.Controls.Add(this.label42);
            this.groupBox9.Controls.Add(this.txtNoEscaleras);
            this.groupBox9.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox9.Location = new System.Drawing.Point(127, 157);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(192, 145);
            this.groupBox9.TabIndex = 22;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Escaleras";
            // 
            // label43
            // 
            this.label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.ForeColor = System.Drawing.Color.Black;
            this.label43.Location = new System.Drawing.Point(3, 86);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(83, 21);
            this.label43.TabIndex = 13;
            this.label43.Text = "Descendentes";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNoEscalerasDesc
            // 
            this.txtNoEscalerasDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNoEscalerasDesc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoEscalerasDesc.ForeColor = System.Drawing.Color.Black;
            this.txtNoEscalerasDesc.Location = new System.Drawing.Point(92, 86);
            this.txtNoEscalerasDesc.Name = "txtNoEscalerasDesc";
            this.txtNoEscalerasDesc.Size = new System.Drawing.Size(95, 21);
            this.txtNoEscalerasDesc.TabIndex = 12;
            // 
            // label41
            // 
            this.label41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ForeColor = System.Drawing.Color.Black;
            this.label41.Location = new System.Drawing.Point(3, 62);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(83, 21);
            this.label41.TabIndex = 11;
            this.label41.Text = "Ascendentes";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNoEscalerasAsc
            // 
            this.txtNoEscalerasAsc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNoEscalerasAsc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoEscalerasAsc.ForeColor = System.Drawing.Color.Black;
            this.txtNoEscalerasAsc.Location = new System.Drawing.Point(92, 62);
            this.txtNoEscalerasAsc.Name = "txtNoEscalerasAsc";
            this.txtNoEscalerasAsc.Size = new System.Drawing.Size(95, 21);
            this.txtNoEscalerasAsc.TabIndex = 10;
            // 
            // label42
            // 
            this.label42.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.ForeColor = System.Drawing.Color.Black;
            this.label42.Location = new System.Drawing.Point(3, 38);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(83, 21);
            this.label42.TabIndex = 9;
            this.label42.Text = "Total";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNoEscaleras
            // 
            this.txtNoEscaleras.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNoEscaleras.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoEscaleras.ForeColor = System.Drawing.Color.Black;
            this.txtNoEscaleras.Location = new System.Drawing.Point(92, 38);
            this.txtNoEscaleras.Name = "txtNoEscaleras";
            this.txtNoEscaleras.Size = new System.Drawing.Size(95, 21);
            this.txtNoEscaleras.TabIndex = 8;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.btnAtrasRelacion3);
            this.groupBox8.Controls.Add(this.btnEliminaRel3);
            this.groupBox8.Controls.Add(this.btnAdelanteRelacion3);
            this.groupBox8.Controls.Add(this.lblRel3Paginacion);
            this.groupBox8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox8.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox8.Location = new System.Drawing.Point(11, 6);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(308, 48);
            this.groupBox8.TabIndex = 21;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Actual / Guardadas";
            // 
            // btnAtrasRelacion3
            // 
            this.btnAtrasRelacion3.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAtrasRelacion3.Enabled = false;
            this.btnAtrasRelacion3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAtrasRelacion3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAtrasRelacion3.Image = ((System.Drawing.Image)(resources.GetObject("btnAtrasRelacion3.Image")));
            this.btnAtrasRelacion3.Location = new System.Drawing.Point(30, 16);
            this.btnAtrasRelacion3.Name = "btnAtrasRelacion3";
            this.btnAtrasRelacion3.Size = new System.Drawing.Size(24, 23);
            this.btnAtrasRelacion3.TabIndex = 13;
            this.btnAtrasRelacion3.UseVisualStyleBackColor = false;
            this.btnAtrasRelacion3.Click += new System.EventHandler(this.btnAtrasRelacion3_Click);
            // 
            // btnEliminaRel3
            // 
            this.btnEliminaRel3.BackColor = System.Drawing.Color.LightSalmon;
            this.btnEliminaRel3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminaRel3.ForeColor = System.Drawing.Color.Black;
            this.btnEliminaRel3.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminaRel3.Image")));
            this.btnEliminaRel3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEliminaRel3.Location = new System.Drawing.Point(146, 15);
            this.btnEliminaRel3.Name = "btnEliminaRel3";
            this.btnEliminaRel3.Size = new System.Drawing.Size(133, 24);
            this.btnEliminaRel3.TabIndex = 28;
            this.btnEliminaRel3.Text = "Eliminar Actual";
            this.btnEliminaRel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminaRel3.UseVisualStyleBackColor = false;
            this.btnEliminaRel3.Click += new System.EventHandler(this.btnEliminaRel3_Click);
            // 
            // btnAdelanteRelacion3
            // 
            this.btnAdelanteRelacion3.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAdelanteRelacion3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAdelanteRelacion3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdelanteRelacion3.Image = ((System.Drawing.Image)(resources.GetObject("btnAdelanteRelacion3.Image")));
            this.btnAdelanteRelacion3.Location = new System.Drawing.Point(95, 16);
            this.btnAdelanteRelacion3.Name = "btnAdelanteRelacion3";
            this.btnAdelanteRelacion3.Size = new System.Drawing.Size(24, 23);
            this.btnAdelanteRelacion3.TabIndex = 14;
            this.btnAdelanteRelacion3.UseVisualStyleBackColor = false;
            this.btnAdelanteRelacion3.Click += new System.EventHandler(this.btnAdelanteRelacion3_Click);
            // 
            // lblRel3Paginacion
            // 
            this.lblRel3Paginacion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblRel3Paginacion.ForeColor = System.Drawing.Color.Black;
            this.lblRel3Paginacion.Location = new System.Drawing.Point(55, 15);
            this.lblRel3Paginacion.Name = "lblRel3Paginacion";
            this.lblRel3Paginacion.Size = new System.Drawing.Size(39, 24);
            this.lblRel3Paginacion.TabIndex = 13;
            this.lblRel3Paginacion.Text = "1 / 1";
            this.lblRel3Paginacion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControlFallos
            // 
            this.tabControlFallos.BackColor = System.Drawing.Color.Bisque;
            this.tabControlFallos.Controls.Add(this.txtFallosCtrl);
            this.tabControlFallos.Controls.Add(this.label22);
            this.tabControlFallos.Controls.Add(this.dgControlFallos);
            this.tabControlFallos.Location = new System.Drawing.Point(4, 22);
            this.tabControlFallos.Name = "tabControlFallos";
            this.tabControlFallos.Size = new System.Drawing.Size(777, 344);
            this.tabControlFallos.TabIndex = 2;
            this.tabControlFallos.Text = "Control Fallos";
            // 
            // txtFallosCtrl
            // 
            this.txtFallosCtrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFallosCtrl.Location = new System.Drawing.Point(387, 51);
            this.txtFallosCtrl.Name = "txtFallosCtrl";
            this.txtFallosCtrl.Size = new System.Drawing.Size(181, 21);
            this.txtFallosCtrl.TabIndex = 2;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(384, 32);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(184, 16);
            this.label22.TabIndex = 1;
            this.label22.Text = "Número de Fallos de Controles";
            // 
            // dgControlFallos
            // 
            this.dgControlFallos.BackgroundColor = System.Drawing.Color.Bisque;
            this.dgControlFallos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgControlFallos.CaptionVisible = false;
            this.dgControlFallos.DataMember = "";
            this.dgControlFallos.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgControlFallos.Location = new System.Drawing.Point(0, 0);
            this.dgControlFallos.Name = "dgControlFallos";
            this.dgControlFallos.Size = new System.Drawing.Size(356, 280);
            this.dgControlFallos.TabIndex = 0;
            this.dgControlFallos.Leave += new System.EventHandler(this.dgControlFallos_Leave);
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
            this.menuCondiciones1.Location = new System.Drawing.Point(440, 376);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 27;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // ColProbablesFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(784, 419);
            this.ControlBox = false;
            this.Controls.Add(this.menuCondiciones1);
            this.Controls.Add(this.tabControl1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "ColProbablesFrm";
            this.Text = "Columnas Probables";
            this.groupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabColumnas.ResumeLayout(false);
            this.tabColumnas.PerformLayout();
            this.tabRelaciones1.ResumeLayout(false);
            this.tabRelaciones1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabRelaciones2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tabRelaciones3.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAgrpacionesSolapadas)).EndInit();
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAgrupacionesPasoFijo)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.tabControlFallos.ResumeLayout(false);
            this.tabControlFallos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgControlFallos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void tol0_Click(object sender, EventArgs e)
        {
            if (tol0.BackColor == System.Drawing.Color.Wheat)
            {
                tol0.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                tol0.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void tol1_Click(object sender, EventArgs e)
        {
            if (tol1.BackColor == System.Drawing.Color.Wheat)
            {
                tol1.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                tol1.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void tol2_Click(object sender, EventArgs e)
        {
            if (tol2.BackColor == System.Drawing.Color.Wheat)
            {
                tol2.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                tol2.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void tol3_Click(object sender, EventArgs e)
        {
            if (tol3.BackColor == System.Drawing.Color.Wheat)
            {
                tol3.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                tol3.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void btnCopiarCols_Click(object sender, System.EventArgs e)
        {
            CambiaCPSelecionado(cpPantalla);

            //borrar ultima CP si no contiene datos
            if (NecesitaBorrarUltimaCP())
            {
                BorrarCP(grupoCP.Count - 1);

            }

            CopiarCPFrm copiaCols = new CopiarCPFrm(grupoCP, this);
            copiaCols.ShowDialog();
            //activa/desactiva botones de movimiento
            ActivarBotones(cpPantalla);
        }

        private void btnEliminarActual_Click(object sender, System.EventArgs e)
        {
            //si es la primera columna
            if (cpPantalla == 0)
            {
                //solo borrar si la CP ya esta guardada en memoria
                if (grupoCP.Count > 0)
                {
                    BorrarCP(cpPantalla);
                }
            }
            else
            {
                BorrarCP(cpPantalla);
                cpPantalla = cpPantalla - 1;
            }

            if (grupoCP.Count == 0)
            {
                //Pueden existir datos en pantalla que tenemos que borrar.
                //Inicializar CP y asignar. Al no cambiar de CP en pantalla,
                //los datos de esta columna en blanco apareceran en pantalla.
                ColumnaProbable cp = new ColumnaProbable();
                grupoCP.Add(cp);
            }

            ActualizaDatosPantalla(cpPantalla);
            //activa/desactiva botones de movimiento
            ActivarBotones(cpPantalla);
        }

        private void tabColumnas_Click(object sender, System.EventArgs e)
        {

        }

        private void btnPuntuacion_Click(object sender, System.EventArgs e)
        {
            CambioPuntosFrm cmbPuntosFrm = new CambioPuntosFrm();
            cmbPuntosFrm.ShowDialog();
        }

        private void btnPrevRel1_Click(object sender, System.EventArgs e)
        {
            CambiaRelCP1Selecionado(relCP1Pantalla - 1);
        }

        private void btnNextRel1_Click(object sender, System.EventArgs e)
        {
            if (TieneRelacion1Datos())
            {
                CambiaRelCP1Selecionado(relCP1Pantalla + 1);
            }
        }

        private void btnEliminarRel1_Click(object sender, System.EventArgs e)
        {
            //si es la primera relacion
            if (relCP1Pantalla == 0)
            {
                //solo borrar si la relacion ya esta guardada en memoria
                if (arrayRelaciones1.Count > 0)
                {
                    BorrarRel1(relCP1Pantalla);
                }
            }
            else
            {
                BorrarRel1(relCP1Pantalla);
                relCP1Pantalla = relCP1Pantalla - 1;
            }

            if (arrayRelaciones1.Count == 0)
            {
                //Pueden existir datos en pantalla que tenemos que borrar.
                //Inicializar rel y asignar. Al no cambiar de rel en pantalla,
                //los datos de esta rel en blanco apareceran en pantalla.
                RelacionCP1 rel = new RelacionCP1();
                arrayRelaciones1.Add(rel);
            }

            ActualizaDatosPantRel1(relCP1Pantalla);
        }

        private void btnPrevCP_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnPrevCP);
        }

        private void btnPrevRel1_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnPrevRel1);
        }

        private void GuardarFiltro()
        {
            GuardarDatos();
            GuardarDatosRelacionesCP1();
            GuardarDatosRelacionesCP2();
            GuardarDatosRelacionesCP3();
            GuardarDatosControlFallos();
        }

        private FiltroColProbables ObtenerFiltroTemporal()
        {
            FiltroColProbables filtroTemp = new FiltroColProbables();
            List<ColumnaProbable> grupoCPTemporal = new List<ColumnaProbable>();
            List<RelacionCP1> arrayRelaciones1Temporal = new List<RelacionCP1>();
            List<RelacionCP2> arrayRelaciones2Temporal = new List<RelacionCP2>();
            List<RelacionCP3> arrayRelaciones3Temporal = new List<RelacionCP3>();

            grupoCPTemporal.AddRange(grupoCP);
            arrayRelaciones1Temporal.AddRange(arrayRelaciones1);
            arrayRelaciones2Temporal.AddRange(arrayRelaciones2);
            arrayRelaciones3Temporal.AddRange(arrayRelaciones3);

            #region Datos
            

            DataSet miDataset = (DataSet)dgControlFallos.DataSource;
            miDataset.AcceptChanges();

            ColumnaProbable cp;

            if (cpPantalla < grupoCPTemporal.Count)
            {
                cp = grupoCPTemporal[cpPantalla];
                GuardaDatosCP(cp);
            }
            else if (TieneColumnaDatos())
            {
                //existen datos en pantalla que necesitan se poner en nueva CP

                //crear CP y poner en grupo
                cp = new ColumnaProbable();
                grupoCPTemporal.Add(cp);

                GuardaDatosCP(cp);
            }

            if (grupoCPTemporal.Count > 0)
            {
                //borrar ultima CP si no contiene datos
                if (NecesitaBorrarUltimaCPTemporal(grupoCPTemporal))
                {
                    grupoCPTemporal.RemoveAt(grupoCPTemporal.Count - 1);

                }
            }

            if (filtroTemp.ContieneDatos == false && grupoCPTemporal.Count > 0)
            {
                //primera vez guardando datos. 
                //Activar condicion.
                filtroTemp.ContieneDatos = true;
                filtroTemp.IsActive = true;
            }

            //guardar copia actualizada de CP en filtro
            filtroTemp.ColProbables = grupoCPTemporal; 
            #endregion

            #region Relaciones 1
            RelacionCP1 rel;

            if (relCP1Pantalla < arrayRelaciones1Temporal.Count)
            {
                rel = (RelacionCP1)arrayRelaciones1Temporal[relCP1Pantalla];
                GuardaDatosRel1(rel);
            }
            else if (TieneRelacion1Datos())
            {
                //existen datos en pantalla que se necesitan poner en nueva rel
                rel = new RelacionCP1();
                arrayRelaciones1Temporal.Add(rel);

                GuardaDatosRel1(rel);
            }

            if (arrayRelaciones1.Count > 0)
            {
                //borrar ultima relacion si no contiene datos
                if (NecesitaBorrarUltimaRel1())
                {
                    BorrarRel1(arrayRelaciones1.Count - 1);
                }
            }

            List<RelacionCP1> relacionesCPFinal = new List<RelacionCP1>();

            for (int i = 0; i < arrayRelaciones1Temporal.Count; i++)
            {
                rel = (RelacionCP1)arrayRelaciones1Temporal[i];

                if (rel.Columnas != "")
                {
                    relacionesCPFinal.Add(rel);
                }
            }

            filtroTemp.RelacionesCP1.Relaciones = relacionesCPFinal; 
            #endregion

            #region Relaciones 2
            RelacionCP2 rel2 = new RelacionCP2();
            rel2.Aciertos = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(txtNumAc.Text);
            rel2.ColumnasA = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsA.Text);
            rel2.ColumnasB = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsB.Text);
            rel2.Concepto = cbbConcepto.Text;
            rel2.Cantidad = cbbMasMenos.Text;
            rel2.StrAciertos = txtNumAc.Text;
            rel2.StrColsA = txtColsA.Text;
            rel2.StrColsB = txtColsB.Text;

            rel2.Aciertos2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(txtNumAc2.Text);
            rel2.ColumnasA2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsA2.Text);
            rel2.ColumnasB2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsB2.Text);
            rel2.Concepto2 = cbbConcepto2.Text;
            rel2.Cantidad2 = cbbMasMenos2.Text;
            rel2.StrAciertos2 = txtNumAc2.Text;
            rel2.StrColsA2 = txtColsA2.Text;
            rel2.StrColsB2 = txtColsB2.Text;

            rel2.ColumnasProbables = grupoCPTemporal;

            if (ComprobarRelacion2(rel2))
            {
                if (arrayRelaciones2Temporal.Count >= indiceNavRel2)
                {
                    arrayRelaciones2Temporal[indiceNavRel2 - 1] = rel2;
                }
                else
                {
                    arrayRelaciones2Temporal.Add(rel2);
                }
            }

            filtroTemp.RelacionesCP2.ColumnasProbables = grupoCPTemporal;
            filtroTemp.RelacionesCP2.Relaciones2 = arrayRelaciones2Temporal; 
            #endregion

            #region Relaciones 3
            List<ColumnaProbable> lista = ObtenerColumnasImplicadasRelCP3();
            RelacionCP3 rel3 = new RelacionCP3();

            rel3.Concepto = cbbConceptoRel3.Text;
            rel3.ConceptoString = cbbConceptoRel3.Text;

            rel3.Columnas = lista;

            int longitudEscaleras = lista.Count / 3;
            int longitudSandwichs = lista.Count / 4;

            rel3.ColumnasImplicadasString = txtColsImplicadas.Text;

            rel3.NumeroEscalerasASCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoEscalerasAsc.Text, longitudEscaleras);
            rel3.NumeroEscalerasASCPermitidasString = txtNoEscalerasAsc.Text;

            rel3.NumeroEscalerasDESCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoEscalerasDesc.Text, longitudEscaleras);
            rel3.NumeroEscalerasDESCPermitidasString = txtNoEscalerasDesc.Text;

            rel3.NumeroEscalerasTotalesPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoEscaleras.Text, longitudEscaleras);
            rel3.NumeroEscalerasTotalesPermitidasString = txtNoEscaleras.Text;

            rel3.NumeroSandwichsPermitidos = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoSandwichs.Text, longitudSandwichs);
            rel3.NumeroSandwichsPermitidosString = txtNoSandwichs.Text;

            DataSet dataSetPasoFijo = (DataSet)dgAgrupacionesPasoFijo.DataSource;

            rel3.AgrupacionesPasoFijoPermitidas = ObtenArrayAgrupacionesPasoFijo(dataSetPasoFijo);
            rel3.AgrupacionesPasoFijoPermitidasString = ObtenTextoAgrupacionesPasoFijo(dataSetPasoFijo);

            DataSet dataSetSolapadas = (DataSet)dgAgrpacionesSolapadas.DataSource;

            rel3.AgrupacionesSolapadasPermitidas = ObtenArrayAgrupacionesSolapadas(dataSetSolapadas);
            rel3.AgrupacionesSolapadasPermitidasString = ObtenTextoAgrupacionesSolapadas(dataSetSolapadas);


            if (ComprobarRelacion3(rel3))
            {
                if (arrayRelaciones3Temporal.Count >= indiceNavRel3)
                {
                    arrayRelaciones3Temporal[indiceNavRel3 - 1] = rel3;
                }
                else
                {
                    arrayRelaciones3Temporal.Add(rel3);
                }
            }

            filtroTemp.RelacionesCP3.Relaciones = arrayRelaciones3Temporal; 
            #endregion

            #region Control Fallos
            DataSet miDataset2 = (DataSet)dgControlFallos.DataSource;

            List<CPControlFallos> arrayControlesFalloTemp = ObtenArrayControlFallos(miDataset2);

            List<CPControlFallos> arrayControlesFalloFinal = new List<CPControlFallos>();

            for (int i = 0; i < arrayControlesFalloTemp.Count; i++)
            {
                CPControlFallos ctrFallos = arrayControlesFalloTemp[i];

                if (ctrFallos.Columnas != "")
                {
                    arrayControlesFalloFinal.Add(ctrFallos);
                }
            }

            filtroTemp.ControlFallosCP.ControlesFallos = arrayControlesFalloFinal;
            filtroTemp.ControlFallosCP.FallosPermitidos = txtFallosCtrl.Text; 
            #endregion

            return filtroTemp;
        }

        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            GuardarFiltro();
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtroCP);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        void BtnNextCPClick(object sender, EventArgs e)
        {
            //cambiar a siguiente columna solo si actual contiene datos
            if (TieneColumnaDatos())
            {
                CambiaCPSelecionado(cpPantalla + 1);
            }
        }

        void BtnPrevCPClick(object sender, EventArgs e)
        {
            CambiaCPSelecionado(cpPantalla - 1);
        }

        private void btnPrev3CP_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnPrev3CP);
        }

        private void btnNext3CP_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnNext3CP);
        }

        private void btnNext3CP_Click(object sender, EventArgs e)
        {
            //cambiar a siguiente columna solo si actual contiene datos
            if (TieneColumnaDatos())
            {
                CambiaCPSelecionado(cpPantalla + VariablesGlobales.Desplazamiento);
            }
        }

        private void btnPrev3CP_Click(object sender, EventArgs e)
        {
            //cambiar a siguiente columna solo si actual contiene datos
            if (TieneColumnaDatos())
            {
                CambiaCPSelecionado(cpPantalla - VariablesGlobales.Desplazamiento);
            }
        }

        private void btnFinCP_Click(object sender, EventArgs e)
        {
            //cambiar a siguiente columna solo si actual contiene datos
            if (TieneColumnaDatos())
            {
                CambiaCPSelecionado(grupoCP.Count - 1);
            }
        }

        private void btnInicioCP_Click(object sender, EventArgs e)
        {
            //cambiar a siguiente columna solo si actual contiene datos
            if (TieneColumnaDatos())
            {
                CambiaCPSelecionado(0);
            }
        }

        private void btnFinCP_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnFinCP);
        }

        private void btnInicioCP_EnabledChanged(object sender, EventArgs e)
        {
            formHelper.CambiarFondoBoton(btnInicioCP);
        }

        private void dgControlFallos_Leave(object sender, EventArgs e)
        {
            DataSet miDataset = (DataSet)dgControlFallos.DataSource;
            miDataset.AcceptChanges();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            if (filtroCP.ContieneDatos)
            {
                if (MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?", "Abrir condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\";
            abreCombDialog.Filter = "Columnas probables(*.cps)|*.cps|Columnas probables(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if (abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if (archComb.AbrirArchivoCombinacion(nombreArchivo))
            {
                grupo = archComb.LeeCondicion();
                InicializaDatos();
                InicializaDatosControlFallos();
                InicializaDatosRelacionesCP();
            }
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            GuardarFiltro();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\";
            saveDialog.Filter = "Columnas probables(*.cps)|*.cps|Columnas probables(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if (saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo = nombreArchivo;
            archComb.GuardaArchivo(filtroCP);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Borrar los datos del filtro?", "Borrar condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            filtroCP = new FiltroColProbables();
            grupoCP = ObtenCopiaCP(filtroCP);
            ActualizaDatosPantalla(0);
            InicializaDatosRelacionesCP();
            InicializaDatosControlFallos();
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            GuardarFiltro();
            // Crea un fichero temporal
            string nombreFichero = Application.StartupPath + "/Temp/" + "tmp.cont";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled = true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            if (filtroCP.ContieneDatos)
            {
                if (MessageBox.Show("El filtro ya tiene datos introducidos. ¿Pegar igualmente?", "Pegar condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            string nombreFichero = Application.StartupPath + "/Temp/" + "tmp.cont";
            abrir(nombreFichero);
        }
        private void CerrarVentana()
        {
            Close();
        }
        protected void ReinicializarValoresRelaciones2()
        {
            txtColsA.Text = "";
            txtColsB.Text = "";
            txtNumAc.Text = "";
            cbbConcepto.Text = "AC";
            cbbMasMenos.Text = "Más";

            txtColsA2.Text = "";
            txtColsB2.Text = "";
            txtNumAc2.Text = "";
            cbbConcepto2.Text = "AC";
            cbbMasMenos2.Text = "Más";

            AdaptarControlesDesplazamientoRelaciones2();
        }
        protected void ReinicializarValoresRelaciones3()
        {
            txtColsImplicadas.Text = "";
            txtNoSandwichs.Text = "";
            txtNoEscaleras.Text = "";
            txtNoEscalerasAsc.Text = "";
            txtNoEscalerasDesc.Text = "";
            cbbConceptoRel3.Text = "AC";

            //Llenar los datagrids
            InicializaDatosAgrupacionesPasoFijo(new string[1]);
            InicializaDatosAgrupacionesSolapadas(new string[1]);

            AdaptarControlesDesplazamientoRelaciones3();
        }

        protected void EliminaRelacion2EnPantalla()
        {
            if (arrayRelaciones2.Count > 0)
            {
                if (arrayRelaciones2.Count >= indiceNavRel2)
                {
                    arrayRelaciones2.RemoveAt(indiceNavRel2 - 1);
                    MostrarRelacion2();
                }
            }
            else
            {
                ReinicializarValoresRelaciones2();
            }
        }
        protected void EliminaRelacion3EnPantalla()
        {
            if (arrayRelaciones3.Count > 0)
            {
                if (arrayRelaciones3.Count >= indiceNavRel3)
                {
                    arrayRelaciones3.RemoveAt(indiceNavRel3 - 1);

                    MostrarRelacion3();
                }
            }
            else
            {
                ReinicializarValoresRelaciones3();
            }
        }

        protected void MostrarRelacion2()
        {
            if (arrayRelaciones2.Count >= indiceNavRel2)
            {
                RelacionCP2 rel = arrayRelaciones2[indiceNavRel2 - 1];
                txtColsA.Text = rel.StrColsA;
                txtColsB.Text = rel.StrColsB;
                txtNumAc.Text = rel.StrAciertos;
                cbbConcepto.Text = rel.Concepto;
                cbbMasMenos.Text = rel.Cantidad;

                txtColsA2.Text = rel.StrColsA2;
                txtColsB2.Text = rel.StrColsB2;
                txtNumAc2.Text = rel.StrAciertos2;
                cbbConcepto2.Text = rel.Concepto2;
                cbbMasMenos2.Text = rel.Cantidad2;
            }
            else
            {
                ReinicializarValoresRelaciones2();
            }
        }
        protected void MostrarPrimeraRelacion2()
        {
            arrayRelaciones2 = filtroCP.RelacionesCP2.Relaciones2;
            if (arrayRelaciones2.Count > 0)
            {
                RelacionCP2 rel = arrayRelaciones2[0];
                txtColsA.Text = rel.StrColsA;
                txtColsB.Text = rel.StrColsB;
                txtNumAc.Text = rel.StrAciertos;
                cbbConcepto.Text = rel.Concepto;
                cbbMasMenos.Text = rel.Cantidad;

                txtColsA2.Text = rel.StrColsA2;
                txtColsB2.Text = rel.StrColsB2;
                txtNumAc2.Text = rel.StrAciertos2;
                cbbConcepto2.Text = rel.Concepto2;
                cbbMasMenos2.Text = rel.Cantidad2;
            }
            else
            {
                ReinicializarValoresRelaciones2();
            }
        }

        protected void MostrarRelacion3()
        {
            if (arrayRelaciones3.Count >= indiceNavRel3)
            {
                RelacionCP3 rel = arrayRelaciones3[indiceNavRel3 - 1];
                txtColsImplicadas.Text = rel.ColumnasImplicadasString;
                txtNoSandwichs.Text = rel.NumeroSandwichsPermitidosString;
                txtNoEscaleras.Text = rel.NumeroEscalerasTotalesPermitidasString;
                txtNoEscalerasAsc.Text = rel.NumeroEscalerasASCPermitidasString;
                txtNoEscalerasDesc.Text = rel.NumeroEscalerasDESCPermitidasString;
                cbbConceptoRel3.Text = rel.ConceptoString;

                //Llenar los datagrids
                InicializaDatosAgrupacionesPasoFijo(rel.AgrupacionesPasoFijoPermitidasString);
                InicializaDatosAgrupacionesSolapadas(rel.AgrupacionesSolapadasPermitidasString);
            }
            else
            {
                ReinicializarValoresRelaciones3();
            }
        }
        protected void MostrarPrimeraRelacion3()
        {
            arrayRelaciones3 = filtroCP.RelacionesCP3.Relaciones;
            if (arrayRelaciones3.Count > 0)
            {
                RelacionCP3 rel = arrayRelaciones3[0];
                txtColsImplicadas.Text = rel.ColumnasImplicadasString;
                txtNoSandwichs.Text = rel.NumeroSandwichsPermitidosString;
                txtNoEscaleras.Text = rel.NumeroEscalerasTotalesPermitidasString;
                txtNoEscalerasAsc.Text = rel.NumeroEscalerasASCPermitidasString;
                txtNoEscalerasDesc.Text = rel.NumeroEscalerasDESCPermitidasString;
                cbbConceptoRel3.Text = rel.ConceptoString;

                //Llenar los datagrids
                InicializaDatosAgrupacionesPasoFijo(rel.AgrupacionesPasoFijoPermitidasString);
                InicializaDatosAgrupacionesSolapadas(rel.AgrupacionesSolapadasPermitidasString);
            }
            else
            {
                ReinicializarValoresRelaciones3();
            }
        }

        protected void GuardarDatosRelacionesCP2()
        {
            GuardarRelacion2EnPantalla();
            
            filtroCP.RelacionesCP2.ColumnasProbables = grupoCP;
            filtroCP.RelacionesCP2.Relaciones2 = arrayRelaciones2;
        }
        protected void GuardarDatosRelacionesCP3()
        {
            GuardarRelacion3EnPantalla();

            filtroCP.RelacionesCP3.Relaciones = arrayRelaciones3;
        }

        protected void GuardarRelacion2(RelacionCP2 rel)
        {
            if (arrayRelaciones2.Count >= indiceNavRel2)
            {
                arrayRelaciones2[indiceNavRel2 - 1] = rel;
            }
            else
            {
                arrayRelaciones2.Add(rel);
            }
        }
        protected void GuardarRelacion3(RelacionCP3 rel)
        {
            if (arrayRelaciones3.Count >= indiceNavRel3)
            {
                arrayRelaciones3[indiceNavRel3 - 1] = rel;
            }
            else
            {
                arrayRelaciones3.Add(rel);
            }
        }

        
        protected bool ComprobarRelacion2(RelacionCP2 rel)
        {
            if ((rel.ColumnasA.Count == 0 || rel.ColumnasB.Count == 0 || rel.Aciertos.Count == 0)&&
                (rel.ColumnasA2.Count == 0 || rel.ColumnasB2.Count == 0 || rel.Aciertos2.Count == 0))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected bool ComprobarRelacion3(RelacionCP3 rel)
        {
            if (rel.AgrupacionesPasoFijoPermitidas == null && rel.AgrupacionesSolapadasPermitidas == null &&
                rel.ColumnasImplicadasString == "" && rel.NumeroEscalerasASCPermitidas == null && rel.NumeroEscalerasDESCPermitidas == null &&
                rel.NumeroEscalerasTotalesPermitidas == null && rel.NumeroSandwichsPermitidos == null)
            {
                return false;
            }
            return true;
        }


        protected void GuardarRelacion2EnPantalla()
        {
            RelacionCP2 rel = new RelacionCP2();
            rel.Aciertos = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(txtNumAc.Text);
            rel.ColumnasA = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsA.Text);
            rel.ColumnasB = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsB.Text);
            rel.Concepto = cbbConcepto.Text;
            rel.Cantidad = cbbMasMenos.Text;
            rel.StrAciertos = txtNumAc.Text;
            rel.StrColsA = txtColsA.Text;
            rel.StrColsB = txtColsB.Text;

            rel.Aciertos2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(txtNumAc2.Text);
            rel.ColumnasA2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsA2.Text);
            rel.ColumnasB2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsB2.Text);
            rel.Concepto2 = cbbConcepto2.Text;
            rel.Cantidad2 = cbbMasMenos2.Text;
            rel.StrAciertos2 = txtNumAc2.Text;
            rel.StrColsA2 = txtColsA2.Text;
            rel.StrColsB2 = txtColsB2.Text;

            rel.ColumnasProbables = grupoCP;

            if (ComprobarRelacion2(rel))
            {
                GuardarRelacion2(rel);
            }
        }
        protected void GuardarRelacion3EnPantalla()
        {
            List<ColumnaProbable> lista = ObtenerColumnasImplicadasRelCP3();
            RelacionCP3 rel = new RelacionCP3();

            rel.Concepto = cbbConceptoRel3.Text;
            rel.ConceptoString = cbbConceptoRel3.Text;

            rel.Columnas = lista;

            int longitudEscaleras = lista.Count / 3;
            int longitudSandwichs = lista.Count / 4;

            rel.ColumnasImplicadasString = txtColsImplicadas.Text;

            rel.NumeroEscalerasASCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoEscalerasAsc.Text, longitudEscaleras);
            rel.NumeroEscalerasASCPermitidasString = txtNoEscalerasAsc.Text;

            rel.NumeroEscalerasDESCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoEscalerasDesc.Text, longitudEscaleras);
            rel.NumeroEscalerasDESCPermitidasString = txtNoEscalerasDesc.Text;

            rel.NumeroEscalerasTotalesPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoEscaleras.Text, longitudEscaleras);
            rel.NumeroEscalerasTotalesPermitidasString = txtNoEscaleras.Text;

            rel.NumeroSandwichsPermitidos = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtNoSandwichs.Text, longitudSandwichs);
            rel.NumeroSandwichsPermitidosString = txtNoSandwichs.Text;

            DataSet dataSetPasoFijo = (DataSet)dgAgrupacionesPasoFijo.DataSource;

            rel.AgrupacionesPasoFijoPermitidas = ObtenArrayAgrupacionesPasoFijo(dataSetPasoFijo);
            rel.AgrupacionesPasoFijoPermitidasString = ObtenTextoAgrupacionesPasoFijo(dataSetPasoFijo);

            DataSet dataSetSolapadas = (DataSet)dgAgrpacionesSolapadas.DataSource;

            rel.AgrupacionesSolapadasPermitidas = ObtenArrayAgrupacionesSolapadas(dataSetSolapadas);
            rel.AgrupacionesSolapadasPermitidasString = ObtenTextoAgrupacionesSolapadas(dataSetSolapadas);


            if (ComprobarRelacion3(rel))
            {
                GuardarRelacion3(rel);
            }

        }

        private void btnRel2Adelante_Click(object sender, EventArgs e)
        {
            GuardarRelacion2EnPantalla();
            
            indiceNavRel2 += 1;
            paginaPantallaRelaciones2++;
            MostrarRelacion2();
            AdaptarControlesDesplazamientoRelaciones2();
        }

        private void btnRel2Atras_Click(object sender, EventArgs e)
        {
            GuardarRelacion2EnPantalla();
            
            indiceNavRel2--;
            paginaPantallaRelaciones2--;
            AdaptarControlesDesplazamientoRelaciones2();
            MostrarRelacion2();
        }

        protected void AdaptarControlesDesplazamientoRelaciones2()
        {
            if (indiceNavRel2 == 1)
            {
                //Desactiva izq
                btnRel2Atras.Enabled = false;
                btnRel2Adelante.Enabled = true;
            }
            else
            {
                btnRel2Atras.Enabled = true;
                btnRel2Adelante.Enabled = true;
            }

            lblRel2Paginacion.Text = Convert.ToString(indiceNavRel2) + " / " + arrayRelaciones2.Count;
        }
        protected void AdaptarControlesDesplazamientoRelaciones3()
        {
            if (indiceNavRel3 == 1)
            {
                //Desactiva izq
                btnAtrasRelacion3.Enabled = false;
                btnAdelanteRelacion3.Enabled = true;
            }
            else
            {
                btnAtrasRelacion3.Enabled = true;
                btnAdelanteRelacion3.Enabled = true;
            }

            lblRel3Paginacion.Text = Convert.ToString(indiceNavRel3) + " / " + arrayRelaciones3.Count;
        }

        private void btnEliminaRel2_Click(object sender, EventArgs e)
        {
            EliminaRelacion2EnPantalla();
            AdaptarControlesDesplazamientoRelaciones2();
        }

        //Relaciones III
        protected void InicializaDatosAgrupacionesSolapadas(string[] entradas)
        {
            DataSet miDataset = ObtenDataSetAgrupacionesSolapadas(entradas);
            dgAgrpacionesSolapadas.SetDataBinding(miDataset, "Agrupaciones Solapadas");
        }
        protected void InicializaDatosAgrupacionesPasoFijo(string[] entradas)
        {
            DataSet miDataset = ObtenDataSetAgrupacionesPasoFijo(entradas);
            dgAgrupacionesPasoFijo.SetDataBinding(miDataset, "Agrupaciones Paso Fijo");
        }

        protected void InicializaGridAgrupacionesSolapadas()
        {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = "Agrpaciones Solapadas";
            tableStyle.ColumnHeadersVisible = true;

            //Número
            DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
            cs.MappingName = "Número";
            cs.HeaderText = "Número";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            //Elementos
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Elementos";
            cs.HeaderText = "Elementos";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            //Aciertos
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Aciertos";
            cs.HeaderText = "Aciertos";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            dgAgrpacionesSolapadas.TableStyles.Add(tableStyle);

        }
        protected void InicializaGridAgrupacionesPasoFijo()
        {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = "Agrpaciones Paso Fijo";
            tableStyle.ColumnHeadersVisible = true;

            //Número
            DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
            cs.MappingName = "Número";
            cs.HeaderText = "Número";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            //Elementos
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Elementos";
            cs.HeaderText = "Elementos";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            //Aciertos
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Aciertos";
            cs.HeaderText = "Aciertos";
            cs.Width = 100;
            tableStyle.GridColumnStyles.Add(cs);

            dgAgrupacionesPasoFijo.TableStyles.Add(tableStyle);

        }

        protected DataSet ObtenDataSetAgrupacionesSolapadas(string[] entradas)
        {
            DataTable myDataTable = new DataTable("Agrupaciones Solapadas");

            DataRow myDataRow;

            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = System.Type.GetType("System.String");
            myDataColumn.ColumnName = "Número";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = System.Type.GetType("System.String");
            myDataColumn.ColumnName = "Elementos";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = System.Type.GetType("System.String");
            myDataColumn.ColumnName = "Aciertos";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            DataSet myDataSet = new DataSet();

            myDataSet.Tables.Add(myDataTable);

            if (entradas != null)
            {
                //meter datos en el dataset
                for (int i = 0; i < entradas.Length; i++)
                {
                    if (entradas[i] != null)
                    {
                        string[] partes = entradas[i].Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                        if (partes.Length == 3)
                        {
                            myDataRow = myDataTable.NewRow();
                            myDataRow["Número"] = partes[0];
                            myDataRow["Elementos"] = partes[1];
                            myDataRow["Aciertos"] = partes[2];

                            myDataTable.Rows.Add(myDataRow);

                        }
                    }
                }
            }

            if (myDataTable.Rows.Count < 50)
            {
                //crea lineas en blanco
                int noLineas = 50 - myDataTable.Rows.Count;

                for (int i = 0; i < noLineas; i++)
                {
                    myDataRow = myDataTable.NewRow();
                    myDataRow["Número"] = "";
                    myDataRow["Elementos"] = "";
                    myDataRow["Aciertos"] = "";

                    myDataTable.Rows.Add(myDataRow);
                }
            }

            return myDataSet;
        }
        protected DataSet ObtenDataSetAgrupacionesPasoFijo(string[] entradas)
        {
            DataTable myDataTable = new DataTable("Agrupaciones Paso Fijo");

            DataRow myDataRow;

            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Número";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Elementos";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Aciertos";
            myDataColumn.DefaultValue = "";
            myDataTable.Columns.Add(myDataColumn);

            DataSet myDataSet = new DataSet();

            myDataSet.Tables.Add(myDataTable);

            if (entradas != null)
            {
                //meter datos en el dataset
                for (int i = 0; i < entradas.Length; i++)
                {
                    if (entradas[i] != null)
                    {
                        string[] partes = entradas[i].Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                        if (partes.Length == 3)
                        {
                            myDataRow = myDataTable.NewRow();
                            myDataRow["Número"] = partes[0];
                            myDataRow["Elementos"] = partes[1];
                            myDataRow["Aciertos"] = partes[2];

                            myDataTable.Rows.Add(myDataRow);
                        }
                    }
                }
            }

            if (myDataTable.Rows.Count < 50)
            {
                //crea lineas en blanco
                int noLineas = 50 - myDataTable.Rows.Count;

                for (int i = 0; i < noLineas; i++)
                {
                    myDataRow = myDataTable.NewRow();
                    myDataRow["Número"] = "";
                    myDataRow["Elementos"] = "";
                    myDataRow["Aciertos"] = "";

                    myDataTable.Rows.Add(myDataRow);
                }
            }

            return myDataSet;
        }

        protected List<ColumnaProbable> ObtenerColumnasImplicadasRelCP3()
        {
            List<ColumnaProbable> lista = new List<ColumnaProbable>();
            List<int> indices = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtColsImplicadas.Text);
            for (int i = 0; i < indices.Count; i++)
            {
                lista.Add(grupoCP[i]);
            }
            return lista;
        }


        protected List<AgrupacionColumnas> ObtenArrayAgrupacionesPasoFijo(DataSet datosDS)
        {
            int maximoAgrupaciones = grupoCP.Count + 1;

            List<AgrupacionColumnas> agrupaciones = new List<AgrupacionColumnas>();
            //elementos, aciertos, numero

            foreach (DataRow row in datosDS.Tables["Agrupaciones Paso Fijo"].Rows)
            {
                if (row["Número"].ToString() != "" && row["Elementos"].ToString() != "" && row["Aciertos"].ToString() != "")
                {
                   
                    List<int> valores = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(row["Número"].ToString());
                    int elementos = Convert.ToInt32(row["Elementos"].ToString());
                    List<int> ac = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(row["Aciertos"].ToString());

                    
                    if (elementos < maximoAgrupaciones && ac.Count <= 15)
                    {
                        AgrupacionColumnas agrup = new AgrupacionColumnas(valores, elementos, ac);
                        agrupaciones.Add(agrup);
                    }
                }
            }
            if (agrupaciones.Count == 0)
            {
                agrupaciones = null;
            }
            return agrupaciones;
        }
        protected List<AgrupacionColumnas> ObtenArrayAgrupacionesSolapadas(DataSet datosDS)
        {
            int maximoAgrupaciones = grupoCP.Count + 1;

            List<AgrupacionColumnas> agrupaciones = new List<AgrupacionColumnas>();
            //elementos, aciertos, numero

            foreach (DataRow row in datosDS.Tables["Agrupaciones Solapadas"].Rows)
            {
                if (row["Número"].ToString() != "" && row["Elementos"].ToString() != "" && row["Aciertos"].ToString() != "")
                {

                    List<int> valores = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(row["Número"].ToString());
                    int elementos = Convert.ToInt32(row["Elementos"].ToString());
                    List<int> ac = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(row["Aciertos"].ToString());


                    if (elementos < maximoAgrupaciones && ac.Count <= 15)
                    {
                        AgrupacionColumnas agrup = new AgrupacionColumnas(valores, elementos, ac);
                        agrupaciones.Add(agrup);
                    }
                }
            }
            if (agrupaciones.Count == 0)
            {
                agrupaciones = null;
            }
            return agrupaciones;
        }
        protected string[] ObtenTextoAgrupacionesSolapadas(DataSet datosDS)
        {
            string[] datos = new string[datosDS.Tables["Agrupaciones Solapadas"].Rows.Count];
            int contador = 0;
            foreach (DataRow row in datosDS.Tables["Agrupaciones Solapadas"].Rows)
            {
                if (row["Número"].ToString() != "")
                {
                    datos[contador] = row["Número"] + "+" + row["Elementos"] + "+" + row["Aciertos"];
                    contador++;
                }
            }
            if (contador == 0)
            {
                datos = null;
            }
            return datos;
        }
        protected string[] ObtenTextoAgrupacionesPasoFijo(DataSet datosDS)
        {
            string[] datos = new string[datosDS.Tables["Agrupaciones Paso Fijo"].Rows.Count];
            int contador = 0;
            foreach (DataRow row in datosDS.Tables["Agrupaciones Paso Fijo"].Rows)
            {
                if (row["Número"].ToString() != "")
                {
                    datos[contador] = row["Número"] + "+" + row["Elementos"] + "+" + row["Aciertos"];
                    contador++;
                }
            }
            if (contador == 0)
            {
                datos = null;
            }
            return datos;
        }

        private void btnAtrasRelacion3_Click(object sender, EventArgs e)
        {
            GuardarRelacion3EnPantalla();

            indiceNavRel3--;
            paginaPantallaRelaciones3--;
            AdaptarControlesDesplazamientoRelaciones3();
            MostrarRelacion3();
        }

        private void btnAdelanteRelacion3_Click(object sender, EventArgs e)
        {         
            GuardarRelacion3EnPantalla();

            indiceNavRel3++;
            paginaPantallaRelaciones3++;
            MostrarRelacion3();
            AdaptarControlesDesplazamientoRelaciones3();
        }

        private void btnEliminaRel3_Click(object sender, EventArgs e)
        {
            EliminaRelacion3EnPantalla();
            AdaptarControlesDesplazamientoRelaciones3();
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroColProbables filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);
            
            visor.ShowDialog();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            BorraPronosticosColumnaPantalla();
        }

        private void btnATriple_Click(object sender, EventArgs e)
        {
            PonerTodosATriple();
        }

        private void btnExportador_Click(object sender, EventArgs e)
        {
            ExportaColumnas();
        }

    }
}
