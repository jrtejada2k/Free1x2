using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Free1X2.Analisis;
using Free1X2.MotorCalculo;
using Free1X2.UI.Controls.Analisis;
using Free1X2.EntradaSalida;

namespace Free1X2.UI
{
    public partial class VisorAnalisisColumnasAbdonFrm : Form
    {
        int noColumna;
        int noColumnaGanadora;
        string modo = "Global";
        List<InformeColumnasABDON> InformePorColumnas;
        List<InformeColumnasABDON> InformePorGanadoras;
        List<long> Columnas;

        List<int>[,] ContenedorAgrupacionesPasoFijoGlobal;
        List<int>[,] ContenedorAgrupacionesSolapadasGlobal;
        List<int>[,] ContenedorAgrupacionesPasoFijo;
        List<int>[,] ContenedorAgrupacionesSolapadas;

        public VisorAnalisisColumnasAbdonFrm(List<InformeColumnasABDON> informePorCols, List<InformeColumnasABDON> informePorGans, List<long> columnas)
        {
            InitializeComponent();
            InformePorColumnas = informePorCols;
            InformePorGanadoras = informePorGans;
            Columnas = columnas;
            ContenedorAgrupacionesPasoFijoGlobal = new List<int>[Columnas.Count + 1, 15];
            ContenedorAgrupacionesSolapadasGlobal = new List<int>[Columnas.Count + 1, 15];
            ContenedorAgrupacionesPasoFijo = new List<int>[Columnas.Count + 1, 15];
            ContenedorAgrupacionesSolapadas = new List<int>[Columnas.Count + 1, 15];

            MostrarInformePorCols();
            MostrarInformePorGans();

            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
      
        protected void MostrarInformePorCols()
        {
            if ((InformePorColumnas != null) && (InformePorColumnas.Count > 0))
            {
                lblColumna.Text = Convert.ToString(noColumna + 1) + " / " + InformePorColumnas.Count;
                ObtenInfoPorCols();
            }
            else
            {
                MostrarMensaje("El informe está vacío");
            }
            AdaptarControlesDesplazamientoPorCols();
        }
      
        protected void MostrarMensaje(string txt)
        {
            MessageBox.Show(txt);
        }
     
        protected void MostrarInformePorGans()
        {

            if ((InformePorGanadoras != null) && (InformePorGanadoras.Count > 0))
            {
                if (modo == "Individual")
                {
                    lblGanadora.Text = Convert.ToString(noColumnaGanadora + 1) + " / " + InformePorGanadoras.Count;
                    ObtenInfoPorGans();
                }
                else
                {
                    ObtenInfoPorGansGlobal();
                }
            }
            else
            {
                MostrarMensaje("El informe está vacío");
            }
            AdaptarControlesDesplazamientoPorGans();
        }

        protected void ObtenInfoPorCols()
        {
            if (InformePorColumnas.Count > 0)
            {
                InformeColumnasABDON inf = InformePorColumnas[noColumna];
                if (noColumna < InformePorColumnas.Count)
                {
                    lblAciertosMin.Text = inf.MinimoAciertos.ToString();
                    lblAciertosMax.Text = inf.MaximoAciertos.ToString();
                }
            }
            else
            {
                lblAciertosMin.Text = "-";
                lblAciertosMax.Text = "-";
            }
        }
       
        protected void ObtenInfoPorGans()
        {
            if (InformePorGanadoras.Count > 0)
            {
                if (noColumnaGanadora < InformePorGanadoras.Count)
                {

                    MostrarAgrupacionesPasoFijo(0);

                    MostrarAgrupacionesSolapadas(0);

                    MostrarSumaTotalDeAciertos();

                    MostrarEscaleras();

                    MostrarSandwichs();
                }
            }
            else
            {
                MostrarMensaje("El informe está vacío");
            }

            //Habilitar controles que se usan en individual

            txtSumaAciertosOpcional.Visible = true;
            lblSumaAcOpcional.Visible = true;
            btnSumaOpcional.Visible = true;
            lblSumaOpcional.Visible = true;
            lblInfoIndividual.Visible = true;
            lblGanadora.Visible = true;
            btnAdelanteGanadoras.Visible = true;
            btnAtrasGanadoras.Visible = true;
            btnGenerarCondicion.Visible = false;
        }

        protected void ObtenInfoPorGansGlobal()
        {
            if (InformePorGanadoras.Count > 0)
            {

                MostrarAgrupacionesPasoFijoGlobal(0);

                MostrarAgrupacionesSolapadasGlobal(0);

                MostrarEscalerasGlobal();

                MostrarSandwichsGlobal();

                MostrarSumaAciertosGlobal();

            }
            else
            {
                MostrarMensaje("El informe está vacío");
            }

            //Inhabilitar controles que no se usan en global

            txtSumaAciertosOpcional.Visible = false;
            lblSumaAcOpcional.Visible = false;
            btnSumaOpcional.Visible = false;
            lblSumaOpcional.Visible = false;
            lblInfoIndividual.Visible = false;
            lblGanadora.Visible = false;
            btnAdelanteGanadoras.Visible = false;
            btnAtrasGanadoras.Visible = false;
            btnGenerarCondicion.Visible = true;
        }

        protected int MostrarAgrupacionesPasoFijo(int tipo)
        {
            Controls.RemoveByKey("CtrlAgrupacionesPasoFijo");
            int noAgrup = ObtenerListaAgrupacionesPasoFijo(tipo);
            string[] lista = ObtenTextoAgrupaciones(ContenedorAgrupacionesPasoFijo);
            if (lista.Length > 0)
            {
                CtrlAgrupacion ctrl = new CtrlAgrupacion(lista);
                ctrl.Name = "CtrlAgrupacionesPasoFijo";
                ctrl.Location = new Point(23, 142);
                Controls.Add(ctrl);
            }
            return noAgrup;
        }
       
        protected int MostrarAgrupacionesSolapadas(int tipo)
        {
            Controls.RemoveByKey("CtrlAgrupacionesSolapadas");
            int noAgrup = ObtenerListaAgrupacionesSolapadas(tipo);
            string[] lista = ObtenTextoAgrupaciones(ContenedorAgrupacionesSolapadas);
            if (lista.Length > 0)
            {
                CtrlAgrupacion ctrl = new CtrlAgrupacion(lista);
                ctrl.Name = "CtrlAgrupacionesSolapadas";
                ctrl.Location = new Point(359, 143);
                Controls.Add(ctrl);
            }
            return noAgrup;
        }
       
        private void MostrarSumaTotalDeAciertos()
        {
            if (InformePorGanadoras.Count > 0)
            {
                InformeColumnasABDON inf = InformePorGanadoras[noColumnaGanadora];
                lblSumaTotalAciertos.Text = inf.SumaTotalDeAciertos.ToString();
            }
        }
       
        private void MostrarSumaOpcionalDeAciertos(List<int> lista)
        {
            int suma = 0;
            if (InformePorGanadoras.Count > 0)
            {
                InformeColumnasABDON inf = InformePorGanadoras[noColumnaGanadora];

                for (int i = 0; i < inf.SerieAciertos.Length; i++)
                {
                    if (lista.Contains(i))
                    {
                        suma += inf.SerieAciertos[i];
                    }
                }
            }
            lblSumaOpcional.Text = suma.ToString();
        }

        private void MostrarSandwichs()
        {
            if (InformePorGanadoras.Count > 0)
            {
                lblNumSandwichs.Text = InformePorGanadoras[noColumnaGanadora].Sandwichs.Count.ToString();
            }
        }
        private void MostrarEscaleras()
        {
            if (InformePorGanadoras.Count > 0)
            {
                InformeColumnasABDON inf = InformePorGanadoras[noColumnaGanadora];

                lblNumEscalerasDesc.Text = inf.NumeroDeEscalerasDescendentes.ToString();
                lblNumEscalerasAsc.Text = inf.NumeroDeEscalerasAscendentes.ToString();
                lblNumEscalerasTotales.Text = inf.NumeroDeEscaleras.ToString();
            }
        }
            
        

        private void MostrarEscalerasGlobal()
        {
            int minASC = 0;
            int minDESC = 0;
            int minTOT = 0;
            int maxASC = 0;
            int maxDESC = 0;
            int maxTOT = 0;

            if (InformePorGanadoras.Count > 0)
            {
                minASC = InformePorGanadoras[0].NumeroDeEscalerasAscendentes;
                maxASC = InformePorGanadoras[0].NumeroDeEscalerasAscendentes;
                minDESC = InformePorGanadoras[0].NumeroDeEscalerasDescendentes;
                maxDESC = InformePorGanadoras[0].NumeroDeEscalerasDescendentes;
                minTOT = InformePorGanadoras[0].NumeroDeEscaleras;
                maxTOT = InformePorGanadoras[0].NumeroDeEscaleras;

                for (int i = 0; i < InformePorGanadoras.Count; i++)
                {
                    InformeColumnasABDON inf = InformePorGanadoras[i];

                    if (inf.NumeroDeEscalerasAscendentes < minASC)
                    {
                        minASC = inf.NumeroDeEscalerasAscendentes;
                    }
                    if (inf.NumeroDeEscalerasAscendentes > maxASC)
                    {
                        maxASC = inf.NumeroDeEscalerasAscendentes;
                    } 

                    if (inf.NumeroDeEscalerasDescendentes < minDESC)
                    {
                        minDESC = inf.NumeroDeEscalerasDescendentes;
                    }
                    if (inf.NumeroDeEscalerasDescendentes > maxDESC)
                    {
                        maxDESC = inf.NumeroDeEscalerasDescendentes;
                    }

                    if (inf.NumeroDeEscaleras < minTOT)
                    {
                        minTOT = inf.NumeroDeEscaleras;
                    } 
                    if (inf.NumeroDeEscaleras > maxTOT)
                    {
                        maxTOT = inf.NumeroDeEscaleras;
                    }

                }
            }

            lblNumEscalerasAsc.Text = minASC + "-" + maxASC;
            lblNumEscalerasDesc.Text = minDESC + "-" + maxDESC;
            lblNumEscalerasTotales.Text = minTOT + "-" + maxTOT;


        }
        private void MostrarSandwichsGlobal()
        {
            int min = 0;
            int max = 0;

            if (InformePorGanadoras.Count > 0)
            {
                min = InformePorGanadoras[0].Sandwichs.Count;
                max = InformePorGanadoras[0].Sandwichs.Count;

                for (int i = 0; i < InformePorGanadoras.Count; i++)
                {
                    InformeColumnasABDON inf = InformePorGanadoras[i];
                    if (inf.Sandwichs.Count > max)
                    {
                        max = inf.Sandwichs.Count;
                    }
                    if (inf.Sandwichs.Count < min)
                    {
                        min = inf.Sandwichs.Count;
                    }
                }
            }
            lblNumSandwichs.Text = min + "-" + max;

        }
        private void MostrarSumaAciertosGlobal()
        {
            List<int> suma = new List<int>();

            if (InformePorGanadoras.Count > 0)
            {
                for (int i = 0; i < InformePorGanadoras.Count; i++)
                {
                    InformeColumnasABDON inf = InformePorGanadoras[i];
                    suma.Add(inf.SumaTotalDeAciertos);
                }
            }
            suma.Sort();
            lblSumaTotalAciertos.Text = suma[0] + "-" + suma[suma.Count - 1];

        }


        protected int[] MostrarAgrupacionesPasoFijoGlobal(int tipo)
        {
            Controls.RemoveByKey("CtrlAgrupacionesPasoFijo");
            int[] noAgrup = ObtenerListaAgrupacionesPasoFijoGlobal(tipo);
            string[] lista = ObtenTextoAgrupaciones(ContenedorAgrupacionesPasoFijoGlobal);
            if (lista.Length > 0)
            {
                CtrlAgrupacion ctrl = new CtrlAgrupacion(lista);
                ctrl.Name = "CtrlAgrupacionesPasoFijo";
                ctrl.Location = new Point(23, 142);
                Controls.Add(ctrl);
            }
            return noAgrup;
        }
        protected int[] MostrarAgrupacionesSolapadasGlobal(int tipo)
        {
            Controls.RemoveByKey("CtrlAgrupacionesSolapadas");
            int[] noAgrup = ObtenerListaAgrupacionesSolapadasGlobal(tipo);
            string[] lista = ObtenTextoAgrupaciones(ContenedorAgrupacionesSolapadasGlobal);
            if (lista.Length > 0)
            {
                CtrlAgrupacion ctrl = new CtrlAgrupacion(lista);
                ctrl.Name = "CtrlAgrupacionesSolapadas";
                ctrl.Location = new Point(359, 143);
                Controls.Add(ctrl);
            }
            return noAgrup;
        }

        private void AdaptarControlesDesplazamientoPorCols()
        {
            if (InformePorColumnas.Count > 0)
            {
                if (noColumna == 0)
                {
                    btnAtras.Enabled = false;
                }
                else
                {
                    btnAtras.Enabled = true;
                }

                if (noColumna < InformePorColumnas.Count - 1)
                {
                    btnAdelante.Enabled = true;
                }
                else
                {
                    btnAdelante.Enabled = false;
                }
            }
            else
            {
                btnAdelante.Enabled = false;
                btnAtras.Enabled = false;
            }
        }
        
        private void AdaptarControlesDesplazamientoPorGans()
        {
            if (InformePorGanadoras.Count > 0)
            {
                if (noColumnaGanadora == 0)
                {
                    btnAtrasGanadoras.Enabled = false;
                }
                else
                {
                    btnAtrasGanadoras.Enabled = true;
                }

                if (noColumnaGanadora < InformePorGanadoras.Count - 1)
                {
                    btnAdelanteGanadoras.Enabled = true;
                }
                else
                {
                    btnAdelanteGanadoras.Enabled = false;
                }
            }
            else
            {
                btnAdelanteGanadoras.Enabled = false;
                btnAtrasGanadoras.Enabled = false;
            }
        }

        private void btnAdelante_Click(object sender, EventArgs e)
        {
            if (noColumna < InformePorColumnas.Count)
            {
                noColumna++;
                MostrarInformePorCols();
            }
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            if (noColumna > 0)
            {
                noColumna--;
                MostrarInformePorCols();
            }
        }

        private void btnAdelanteGanadoras_Click(object sender, EventArgs e)
        {
            if (noColumnaGanadora < InformePorGanadoras.Count)
            {
                noColumnaGanadora++;
                ContenedorAgrupacionesSolapadas = new List<int>[Columnas.Count + 1, 15];
                ContenedorAgrupacionesPasoFijo = new List<int>[Columnas.Count + 1, 15];

                MostrarInformePorGans();
            }
        }

        private void btnAtrasGanadoras_Click(object sender, EventArgs e)
        {
            if (noColumnaGanadora > 0)
            {
                noColumnaGanadora--;
                ContenedorAgrupacionesSolapadas = new List<int>[Columnas.Count + 1, 15];
                ContenedorAgrupacionesPasoFijo = new List<int>[Columnas.Count + 1, 15];
                MostrarInformePorGans();
            }
        }

        private void btnMostrarAgrupaciones_Click(object sender, EventArgs e)
        {
            //Mostraremos las agrupaciones sólo que se nos indiquen
            int tipo;
            try
            {
                tipo = Convert.ToInt32(txtTipoAgrupacion.Text);
            }
            catch
            {
                tipo = 0;
            }
            txtTipoAgrupacion.Text = tipo.ToString();

            if (modo == "Individual")
            {
                ContenedorAgrupacionesPasoFijo = new List<int>[Columnas.Count + 1, 15];
                int noTotal = MostrarAgrupacionesPasoFijo(tipo);

                lblNumElementosPasoFijo.Text = txtTipoAgrupacion.Text;
                lblHayPasoFijo.Text = noTotal.ToString();
            }
            else
            {
                ContenedorAgrupacionesPasoFijoGlobal = new List<int>[Columnas.Count + 1, 15];
                int[] rangos = MostrarAgrupacionesPasoFijoGlobal(tipo);
                lblNumElementosPasoFijo.Text = txtTipoAgrupacion.Text;
                lblHayPasoFijo.Text = rangos[0] + "-" + rangos[1];
            }
            
        }

        private void btnMostrarAgrupacionesSolapadas_Click(object sender, EventArgs e)
        {
            //Mostraremos las agrupaciones sólo que se nos indiquen
            int tipo;
            try
            {
                tipo = Convert.ToInt32(txtTipoAgrupacionSolapada.Text);
            }
            catch
            {
                tipo = 0;
            }
            txtTipoAgrupacionSolapada.Text = tipo.ToString();

            if (modo == "Individual")
            {
                ContenedorAgrupacionesSolapadas = new List<int>[Columnas.Count + 1, 15];
                int noTotal = MostrarAgrupacionesSolapadas(tipo);
                lblNumElementosSolapadas.Text = txtTipoAgrupacionSolapada.Text;
                lblHaySolapadas.Text = noTotal.ToString();
            }
            else
            {
                ContenedorAgrupacionesSolapadasGlobal = new List<int>[Columnas.Count + 1, 15];
                int[] rangos = MostrarAgrupacionesSolapadasGlobal(tipo);
                lblNumElementosSolapadas.Text = txtTipoAgrupacionSolapada.Text;
                lblHaySolapadas.Text = rangos[0] + "-" + rangos[1];
            }
        }

        private void btnSumaOpcional_Click(object sender, EventArgs e)
        {
            if (txtSumaAciertosOpcional.Text != null)
            {
                List<int> columnas = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(txtSumaAciertosOpcional.Text);
                MostrarSumaOpcionalDeAciertos(columnas);
            }
        }
        private void GenerarCondicion()
        {
            //Siempre valores globales
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\";
            saveDialog.Filter = "Columnas probables(*.cps)|*.cps|Columnas probables(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                //Construir un filtro
                string todosValores = Utils.UtilidadesEntradasValores.ObtenerTodosValores();
                List<ColumnaProbable> cols = new List<ColumnaProbable>();
                FiltroColProbables filtro = new FiltroColProbables();

                //Inicializar columnas
                for (int i = 0; i < Columnas.Count; i++)
                {
                    int min = InformePorColumnas[i].MinimoAciertos;
                    int max = InformePorColumnas[i].MaximoAciertos;

                    string pronosticos = Utils.UtilColumnas.ObtenerStringFromLongApuestaMultiple(Columnas[i]);
                    ColumnaProbable cp = new ColumnaProbable();
                    cp.Pronosticos = pronosticos.Split(',');
                    cp.SetNoAciertos(Utils.UtilidadesEntradasValores.ObtenerTodosValores(min,max));
                    cp.SetNoAciertosSeguidos(todosValores);
                    cp.SetNoFallosSeguidos(todosValores);

                    cp.SetPuntos("");

                    filtro.ColProbables.Add(cp);
                    cols.Add(cp);
                }
                //Inicializar Relacion 1

                RelacionCP1 rel1 = new RelacionCP1();
                rel1.Columnas = "1-" + Columnas.Count;
                rel1.SumaAciertos = lblSumaTotalAciertos.Text;
                rel1.Recorridos = "";
                rel1.CantidadCP = "";
                rel1.CuantosAC = "";

                filtro.RelacionesCP1.Relaciones.Add(rel1);

                //Inicializar Relacion 3

                RelacionCP3 rel = new RelacionCP3();
                rel.ColumnasImplicadasString = "1-" + Columnas.Count;
                rel.Columnas = cols;
                rel.Concepto = "AC";
                rel.ConceptoString = "AC";
                int longitudEsc = cols.Count / 3;
                int longitudSandwichs = cols.Count / 4;

                rel.NumeroEscalerasASCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(lblNumEscalerasAsc.Text, longitudEsc);
                rel.NumeroEscalerasASCPermitidasString = lblNumEscalerasAsc.Text;

                rel.NumeroEscalerasDESCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(lblNumEscalerasDesc.Text, longitudEsc);
                rel.NumeroEscalerasDESCPermitidasString = lblNumEscalerasDesc.Text;

                rel.NumeroEscalerasTotalesPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(lblNumEscalerasTotales.Text, longitudEsc);
                rel.NumeroEscalerasTotalesPermitidasString = lblNumEscalerasTotales.Text;

                rel.NumeroSandwichsPermitidos = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(lblNumSandwichs.Text, longitudSandwichs);
                rel.NumeroSandwichsPermitidosString = lblNumSandwichs.Text;

                rel.AgrupacionesPasoFijoPermitidasString = ObtenTextoAgrupaciones(ContenedorAgrupacionesPasoFijoGlobal);

                rel.AgrupacionesSolapadasPermitidasString = ObtenTextoAgrupaciones(ContenedorAgrupacionesSolapadasGlobal);

                filtro.RelacionesCP3.Relaciones.Add(rel);
                filtro.ContieneDatos = true;
                guardar(saveDialog.FileName, filtro);
            }
            MessageBox.Show("Filtro guardado");

        }
        private void btnGenerarCondicion_Click(object sender, EventArgs e)
        {
            string txt = "Atención, se va a generar el filtro, asegurese ";
            txt += "de que las Agrupaciones Paso Fijo y Solapadas muestran ";
            txt += "los datos que desea. \nSi quiere que se guarden todos los ";
            txt += "datos, por favor, marque '0' en el campo 'Mostrar sólo ";
            txt += "agrupaciones de x elementos'";

            if (MessageBox.Show(txt, "Atención", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                GenerarCondicion();
            }
        }

        protected string[] ObtenTextoAgrupaciones(List<int>[,] lista)
        {
            List<string> temp = new List<string>();
            for (int i = 1; i < lista.GetLength(0); i++)
            {
                for (int j = 0; j < lista.GetLength(1); j++)
                {
                    if (lista[i, j] != null)
                    {
                        if (lista[i, j].Count > 1)
                        {
                            lista[i, j].Sort();
                            string fila = lista[i, j][0] + "-" + lista[i, j][lista[i, j].Count - 1] + "+" + i + "+" + j;
                            temp.Add(fila);
                        }
                        else
                        {
                            if (lista[i, j][0] != 0)
                            {
                                string fila = lista[i, j][0] + "+" + i + "+" + j;
                                temp.Add(fila);
                            }
                        }
                    }
                }
            }

            string[] datos = new string[temp.Count];
            for (int i = 0; i < temp.Count; i++)
            {
                datos[i] = temp[i];
            }
            return datos;
        }

        protected int ObtenerListaAgrupacionesPasoFijo(int tipo)
        {
            int noAgrup = 0;
            if (InformePorGanadoras.Count > 0)
            {
                InformeColumnasABDON inf = InformePorGanadoras[noColumnaGanadora];

                for (int j = 0; j < inf.AgrupacionesPasoFijo.GetLength(0); j++)
                {
                    if (j == tipo || tipo == 0)
                    {
                        for (int k = 0; k < inf.AgrupacionesPasoFijo.GetLength(1); k++)
                        {
                            if (ContenedorAgrupacionesPasoFijo[j, k] == null)
                            {
                                ContenedorAgrupacionesPasoFijo[j, k] = new List<int>();
                            }
                            int num = inf.AgrupacionesPasoFijo[j, k];
                            if (!ContenedorAgrupacionesPasoFijo[j, k].Contains(num))
                            {
                                noAgrup += num;
                                ContenedorAgrupacionesPasoFijo[j, k].Add(num);

                            }
                        }
                    }
                }
            }
            return noAgrup;
        }
        protected int ObtenerListaAgrupacionesSolapadas(int tipo)
        {
            int noAgrup = 0;
            if (InformePorGanadoras.Count > 0)
            {
                InformeColumnasABDON inf = InformePorGanadoras[noColumnaGanadora];

                for (int j = 0; j < inf.AgrupacionesSolapadas.GetLength(0); j++)
                {
                    if (j == tipo || tipo == 0)
                    {
                        for (int k = 0; k < inf.AgrupacionesSolapadas.GetLength(1); k++)
                        {
                            if (ContenedorAgrupacionesSolapadas[j, k] == null)
                            {
                                ContenedorAgrupacionesSolapadas[j, k] = new List<int>();
                            }
                            int num = inf.AgrupacionesSolapadas[j, k];
                            if (!ContenedorAgrupacionesSolapadas[j, k].Contains(num))
                            {
                                noAgrup += num;
                                ContenedorAgrupacionesSolapadas[j, k].Add(num);
                            }
                        }
                    }
                }
            }
            return noAgrup;
        }
       
        protected int[] ObtenerListaAgrupacionesPasoFijoGlobal(int tipo)
        {
            int noAgrup = 0;
            List<int> valores = new List<int>();
            if (InformePorGanadoras.Count > 0)
            {
                for (int i = 0; i < InformePorGanadoras.Count; i++)
                {
                    InformeColumnasABDON inf = InformePorGanadoras[i];

                    for (int j = 1; j < inf.AgrupacionesPasoFijo.GetLength(0); j++)
                    {
                        if (j == tipo || tipo == 0)
                        {
                            for (int k = 0; k < inf.AgrupacionesPasoFijo.GetLength(1); k++)
                            {
                                if (ContenedorAgrupacionesPasoFijoGlobal[j, k] == null)
                                {
                                    ContenedorAgrupacionesPasoFijoGlobal[j, k] = new List<int>();
                                }
                                int num = inf.AgrupacionesPasoFijo[j, k];
                                noAgrup += num;
                                if (!ContenedorAgrupacionesPasoFijoGlobal[j, k].Contains(num))
                                {
                                    ContenedorAgrupacionesPasoFijoGlobal[j, k].Add(num);
                                }
                            }
                        }
                    }
                    for (int j = inf.AgrupacionesPasoFijo.GetLength(0); j < ContenedorAgrupacionesPasoFijoGlobal.GetLength(0); j++)
                    {

                        for (int k = 0; k < inf.AgrupacionesPasoFijo.GetLength(1); k++)
                        {
                            if (ContenedorAgrupacionesPasoFijoGlobal[j, k] == null)
                            {
                                ContenedorAgrupacionesPasoFijoGlobal[j, k] = new List<int>();
                                ContenedorAgrupacionesPasoFijoGlobal[j, k].Add(0);
                            }
                            else
                            {
                                if (ContenedorAgrupacionesPasoFijoGlobal[j, k].IndexOf(0) == -1)
                                {
                                    ContenedorAgrupacionesPasoFijoGlobal[j, k].Add(0);
                                }
                            }
                        }
                    }
                    valores.Add(noAgrup);
                    noAgrup = 0;
                }
            }
            return Utils.UtilidadesEntradasValores.ObtenerRangos(valores);
        }
        protected int[] ObtenerListaAgrupacionesSolapadasGlobal(int tipo)
        {
            int noAgrup = 0;
            List<int> valores = new List<int>();
            if (InformePorGanadoras.Count > 0)
            {
                for (int i = 0; i < InformePorGanadoras.Count; i++)
                {
                    InformeColumnasABDON inf = InformePorGanadoras[i];

                    for (int j = 1; j < inf.AgrupacionesSolapadas.GetLength(0); j++)
                    {
                        if (j == tipo || tipo == 0)
                        {
                            for (int k = 0; k < inf.AgrupacionesSolapadas.GetLength(1); k++)
                            {
                                if (ContenedorAgrupacionesSolapadasGlobal[j, k] == null)
                                {
                                    ContenedorAgrupacionesSolapadasGlobal[j, k] = new List<int>();
                                }
                                int num = inf.AgrupacionesSolapadas[j, k];
                                noAgrup += num;
                                if (!ContenedorAgrupacionesSolapadasGlobal[j, k].Contains(num))
                                {
                                    ContenedorAgrupacionesSolapadasGlobal[j, k].Add(num);
                                }
                            }
                        }
                    }

                    for (int j = inf.AgrupacionesSolapadas.GetLength(0); j < ContenedorAgrupacionesSolapadasGlobal.GetLength(0); j++)
                    {

                        for (int k = 0; k < inf.AgrupacionesSolapadas.GetLength(1); k++)
                        {
                            if (ContenedorAgrupacionesSolapadasGlobal[j, k] == null)
                            {
                                ContenedorAgrupacionesSolapadasGlobal[j, k] = new List<int>();
                            }
                            if (!ContenedorAgrupacionesSolapadasGlobal[j, k].Contains(0))
                            {
                                ContenedorAgrupacionesSolapadasGlobal[j, k].Add(0);
                            }
                        }
                    }

                    valores.Add(noAgrup);
                    noAgrup = 0;
                }
            }
            return Utils.UtilidadesEntradasValores.ObtenerRangos(valores);
        }

        private void guardar(string nombreArchivo, FiltroColProbables filtro)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo = nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }

        private void btnModo_Click(object sender, EventArgs e)
        {
            switch (modo)
            {
                case "Individual":
                    modo = "Global";
                    btnModo.Text = "Cambiar a modo Individual";
                    break;
                case "Global":
                    modo = "Individual";
                    btnModo.Text = "Cambiar a modo global";

                    break;
            }
            lblHayPasoFijo.Text = "";
            lblHaySolapadas.Text = "";
            lblNumElementosPasoFijo.Text = "";
            lblNumElementosSolapadas.Text = "";
            txtTipoAgrupacion.Text = "";
            txtTipoAgrupacionSolapada.Text = "";
            MostrarInformePorGans();
        }

        private void VisorAnalisisColumnasAbdonFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose(true);
        }
    }
}
