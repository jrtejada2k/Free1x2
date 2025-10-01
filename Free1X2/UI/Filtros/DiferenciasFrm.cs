using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.UI.Controls;
using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo.Estadisticas;

namespace Free1X2.UI.Filtros
{
    public partial class DiferenciasFrm : Form
    {
        int controlesAAñadir = 20;
        int conjunto = 1;
        int indice;
        FiltroDiferencias filtro;
        protected List<Diferencia> arrayDiferencias;
        private Grupo grupo;
        private MainForm parentFrm;

        public DiferenciasFrm(Grupo grupo, MainForm form)
        {
            InitializeComponent();
            LlenarControles(controlesAAñadir);
            this.grupo = grupo;
            filtro = (FiltroDiferencias)grupo.GetFiltro(Filtro.Diferencias.ToString());
            parentFrm = form;
            MarcarValores();
            AdaptarControlesDesplazamiento();
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Debe especificar en cada línea un grupo de partidos\nseparados por \",\" o por \"-\". \nMarcar en las casillas de la derecha para \ncada concepto la cantidad o el intervalo de valores DISTINTOS \nentre los grupos especificados";

        }

        public MainForm FormPadre
        {
            get { return parentFrm; }
        }

        protected void LlenarControles(int noControles)
        {
            int posicionY = 0;
            int noControl = 1;
            for (int i = 0; i < noControles; i++)
            {
                CtrlSimetria ctrlSimetria = new CtrlSimetria(noControl);
                if (i == noControles - 1)
                {
                    controlesAAñadir++;
                    ctrlSimetria.TxtSimetria.TextChanged += Añadir_Enter;

                    int indicePre = i - 1;
                    if (indicePre >= 0)
                    {
                        CtrlSimetria ctrlSimetriaPre = (CtrlSimetria)cctrl.Controls[indicePre];
                        ctrlSimetriaPre.TxtSimetria.TextChanged -= Añadir_Enter;
                    }
                }
                AñadirControl(ctrlSimetria, 0, posicionY);
                posicionY += 14;
                noControl++;
            }
        }
        protected void AñadirFromList(List<string> lista)
        {
            int index = 0;
            for (int i = 0; i < cctrl.Controls.Count; i++)
            {
                CtrlSimetria ctrl = (CtrlSimetria)cctrl.Controls[i];
                if (ctrl.TxtSimetria.Text == "")
                {
                    if (lista.Count > index)
                    {
                        ctrl.TxtSimetria.Text = lista[index];
                        index++;
                    }
                }

            }            
        }
        private void AñadirControl(CtrlSimetria ctrlSimetria, int posicionX, int posicionY)
        {
            ctrlSimetria.Location = new Point(posicionX, posicionY);
            cctrl.Controls.Add(ctrlSimetria);
            cctrl.AutoScroll = true;
        }
        protected void LimpiarPantalla()
        {
            for (int i = 0; i < cctrl.Controls.Count; i++)
            {

                CtrlSimetria ctrlSim = (CtrlSimetria)cctrl.Controls[i];
                ctrlSim.TxtSimetria.Text = "";
            }
            txtV.Text = "";
            txtX.Text = "";
            txtDoses.Text = "";
            txtDib.Text = "";
            txtInt.Text = "";
            txtFormatos.Text = "";

        }
        protected void MarcarValores()
        {
            string nombreFiltro = Filtro.Diferencias.ToString();
            filtro = (FiltroDiferencias)grupo.GetFiltro(nombreFiltro);
            if (filtro.ContieneDatos)
            {
                MarcarValores(filtro.Diferencias[0]);
            }
            else
            {
                LimpiarPantalla();
            }
            conjunto = 1;
        }

        protected void MarcarValores(Diferencia rep)
        {
            LimpiarPantalla();
           
            for (int i = 0; i < rep.PartidosSimetricos.Count; i++)
            {
                CtrlSimetria ctrlSim = (CtrlSimetria)cctrl.Controls[i];
                ctrlSim.TxtSimetria.Text = Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(rep.PartidosSimetricos[i]);
            }
            if (rep.AnalizaVX2Dib)
            {
                txtV.Text = Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcV);
                txtX.Text = Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcX);
                txtDoses.Text = Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcDoses);
                txtDib.Text = Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcDib);
            }
            if (rep.AnalizaInterrupciones)
            {
                txtInt.Text = Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcInt);
            }
            if (rep.AnalizaFormatos)
            {
                txtFormatos.Text = Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(rep.AcFormatos);
            }
        }
        private void Añadir_Enter(object sender, EventArgs e)
        {
            //Obtener el último control, su número y su posicion
            CtrlSimetria ctrlSim = (CtrlSimetria)cctrl.Controls[cctrl.Controls.Count - 1];

            int posicionY = ctrlSim.Location.Y;
            int noControl = Convert.ToInt32(ctrlSim.LblNum.Text);
            //Eliminar el evento en el último control
            ctrlSim.TxtSimetria.TextChanged -= Añadir_Enter;
            CtrlSimetria ctrlSimetria = new CtrlSimetria(noControl + 1);
            ctrlSimetria.TxtSimetria.TextChanged += Añadir_Enter;

            AñadirControl(ctrlSimetria, 0, posicionY + 15);
        }
        private bool[] ActivarTodos(int longitud)
        {
            bool[] array = new bool[longitud];

            for (int i = 1; i < array.Length; i++)
            {
                array[i] = true;
            }
            return array;
        }
        private Diferencia CreaDiferencia()
        {
            try
            {
                Diferencia rep = new Diferencia();
                for (int i = 0; i < cctrl.Controls.Count; i++)
                {
                    CtrlSimetria ctrl = (CtrlSimetria)cctrl.Controls[i];
                    if (ctrl.TxtSimetria.Text != "")
                    {
                        
                        bool[] partidos = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(ctrl.TxtSimetria.Text, VariablesGlobales.NumeroPartidos + 1);


                        rep.AñadirPartidosSimetricos(partidos);
                    }
                }
                rep.AcV = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtV.Text, rep.PartidosSimetricos.Count + 1);
                rep.AcX = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtX.Text, rep.PartidosSimetricos.Count + 1);
                rep.AcDoses = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtDoses.Text, rep.PartidosSimetricos.Count + 1);
                rep.AcDib = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtDib.Text, rep.PartidosSimetricos.Count + 1);
                rep.AcInt = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtInt.Text, rep.PartidosSimetricos.Count + 1);
                rep.AcFormatos = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(txtFormatos.Text, rep.PartidosSimetricos.Count + 1);

                if (rep.PartidosSimetricos.Count > 0)
                {
                    if (rep.AcV == null && rep.AcX == null && rep.AcDoses == null && rep.AcDib == null)
                    {
                        rep.AnalizaVX2Dib = false;
                    }
                    else
                    {
                        rep.AnalizaVX2Dib = true;
                    }
                    if (rep.AcV == null)
                    {
                        rep.AcV = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                    }
                    if (rep.AcX == null)
                    {
                        rep.AcX = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                    }
                    if (rep.AcDoses == null)
                    {
                        rep.AcDoses = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                    }
                    if (rep.AcDib == null)
                    {
                        rep.AcDib = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                    }
                    if (rep.AcInt == null)
                    {
                        rep.AcInt = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                        rep.AnalizaInterrupciones = false;
                    }
                    else
                    {
                        rep.AnalizaInterrupciones = true;
                    }

                    if (rep.AcFormatos == null)
                    {
                        rep.AcFormatos = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                        rep.AnalizaFormatos = false;
                    }
                    else
                    {
                        rep.AnalizaFormatos = true;
                    }
                }
                else
                {
                    rep = null;
                }
                
                return rep;
            }
            catch
            {
                return null;
            }
        }
        private void ActualizarDatos()
        {
            //Determina si hay que guardar o actualizar la Diferencia en pantalla
            if (filtro.Diferencias.Count > indice)
            {
                Diferencia s = CreaDiferencia();
                if (s != null)
                {
                    filtro.Diferencias[indice] = s;
                }
            }
            else
            {
                //La añadimos
                Diferencia s = CreaDiferencia();
                if (s != null)
                {
                    filtro.Diferencias.Add(s);
                }
                AdaptarControlesDesplazamiento();
            }
        }
        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            //Cada línea introducida será un grupo de partidos. Los grupos de partidos
            //forman una Diferencia
            string nombreFiltro = Filtro.Diferencias.ToString();
            filtro = (FiltroDiferencias)grupo.GetFiltro(nombreFiltro);
            ActualizarDatos();
            filtro.IsActive = filtro.ContieneDatos;
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
            CerrarVentana();


        }


        private void CerrarVentana()
        {
            Close();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }
        private void AdaptarControlesDesplazamiento()
        {
            if (conjunto > 1)
            {
                btnPrevCP.Enabled = true;
            }
            else
            {
                btnPrevCP.Enabled = false;
            }
            lblNoSim.Text = conjunto + "/" + filtro.Diferencias.Count;

        }
        private void btnNextCP_Click(object sender, EventArgs e)
        {
            //Hay que guardar la Diferencia en pantalla
            if (filtro.Diferencias.Count > indice)
            {
                Diferencia s = CreaDiferencia();
                if (s != null)
                {
                    filtro.Diferencias[indice] = s;
                }
                conjunto++;
                indice++;
                if (filtro.Diferencias.Count > indice)
                {
                    //Hay más conjuntos después del mostrado

                    MarcarValores(filtro.Diferencias[indice]);
                }
                else
                {
                    //Se está creando una nueva simetria
                    LimpiarPantalla();
                }

            }
            else
            {
                Diferencia s = CreaDiferencia();
                if (s!=null)
                {
                    filtro.Diferencias.Add(s);
                
                    indice++;
                    conjunto++;
                    LimpiarPantalla();
                }
            }
            

            

            AdaptarControlesDesplazamiento();
        }

        private void btnPrevCP_Click(object sender, EventArgs e)
        {
            if (indice > 0)
            {
                //Cuando vamos atrás lo primero es intentar guardar
                if (filtro.Diferencias.Count > indice)
                {
                    Diferencia s = CreaDiferencia();
                    if (s != null)
                    {
                        filtro.Diferencias[indice] = s;
                   

                        conjunto--;
                        indice--;
                    }
                    MarcarValores(filtro.Diferencias[indice]);
                }
                else
                {
                    //El usuario se ha arrepentido
                    conjunto--;
                    indice--;
                    MarcarValores(filtro.Diferencias[indice]);
                }
            }
                      

            AdaptarControlesDesplazamiento();
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            // Crea un fichero temporal
            string nombreFichero = Application.StartupPath + "/Temp/" + "tmp.rep";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled = true;
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            ActualizarDatos();
            FiltroDiferencias filtroTemp = filtro;

            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {

            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\";
            saveDialog.Filter = "Diferencias(*.dif)|*.dif|Diferencias(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if (saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }
        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo = nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            ActualizarDatos();
            if (filtro.ContieneDatos)
            {
                if (MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?", "Abrir condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            filtro = new FiltroDiferencias();
            if (filtro != null)
            {
                grupo.ActivaFiltro(filtro);
                LimpiarPantalla();
            }
            string nombreFichero = Application.StartupPath + "/Temp/" + "tmp.rep";
            abrir(nombreFichero);
        }
        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            FormulariosHelper mf = new FormulariosHelper();
            if (mf.ExisteFicheroTemporal("tmp.simII"))
                menuCondiciones1.BotonPegarEnabled = true;
            else
                menuCondiciones1.BotonPegarEnabled = false;
        }
        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            filtro = (FiltroDiferencias)grupo.GetFiltro("Diferencias");
            if (filtro.ContieneDatos)
            {
                if (MessageBox.Show("¿Borrar los datos del filtro?", "Borrar condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            filtro.Diferencias.Clear();
            indice = 0;
            conjunto = 1;
            AdaptarControlesDesplazamiento();
            LimpiarPantalla();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            ActualizarDatos();
            if (filtro.ContieneDatos)
            {
                if (MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?", "Abrir condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            filtro = new FiltroDiferencias();
            grupo.ActivaFiltro(filtro);
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones/";
            abreCombDialog.Filter = "Diferencias(*.dif)|*.dif|Diferencias(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if (abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }
        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if (archComb.AbrirArchivoCombinacion(nombreArchivo))
            {
                Grupo g = archComb.LeeCondicion();
                filtro = (FiltroDiferencias)g.GetFiltro("Diferencias");
                if (filtro.ContieneDatos)
                {
                    grupo.ActivaFiltro(filtro);
                    MarcarValores();
                    AdaptarControlesDesplazamiento();
                }
            }
        }

        private void btnEliminaActual_Click(object sender, EventArgs e)
        {
            if (filtro.Diferencias.Count > indice)
            {
                filtro.Diferencias.RemoveAt(indice);
                if (indice > 0)
                {
                    indice--;
                }                
            }
            else
            {
                LimpiarPantalla();
            }
            MarcarValores();
            AdaptarControlesDesplazamiento();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<string> cadenas = new List<string>();

            switch (cbbAtajos.Text)
            {
                case "Dúos":
                    cadenas = Utils.UtilidadesEntradasValores.ObtenerGruposDeValores(2, chkPasoFijo.Checked);
                    break;
                case "Tríos":
                    cadenas = Utils.UtilidadesEntradasValores.ObtenerGruposDeValores(3, chkPasoFijo.Checked);
                    break;
                case "Cuartetos":
                    cadenas = Utils.UtilidadesEntradasValores.ObtenerGruposDeValores(4, chkPasoFijo.Checked);
                    break;
                case "Quintetos":
                    cadenas = Utils.UtilidadesEntradasValores.ObtenerGruposDeValores(5, chkPasoFijo.Checked);
                    break;
                case "Sextetos":
                    cadenas = Utils.UtilidadesEntradasValores.ObtenerGruposDeValores(6, chkPasoFijo.Checked);
                    break;
                case "Septetos":
                    cadenas = Utils.UtilidadesEntradasValores.ObtenerGruposDeValores(7, chkPasoFijo.Checked);
                    break;
                case "Octetos":
                    cadenas = Utils.UtilidadesEntradasValores.ObtenerGruposDeValores(8, chkPasoFijo.Checked);
                    break;

            }
            AñadirFromList(cadenas);
        }
    }
}
