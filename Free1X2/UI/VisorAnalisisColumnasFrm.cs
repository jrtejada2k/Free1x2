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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.MotorCalculo;
using Free1X2.Analisis;

namespace Free1X2.UI
{
    public partial class VisorAnalisisColumnasFrm : Form
    {
        ContenedorAnalisisGlobal contenedorAnalisis;
        protected int noGrupos;
        protected int grupoActual;
        FiltroContactos filtroContactos;
        FiltroNoVariantes filtroVariantes;
        FiltroSignosSeguidos filtroSeguidos;
        FiltroPesosNumericos filtroPesos;
        FiltroDibujos filtroDibujos;
        FiltroInterrupciones filtroInterrupciones;
        public VisorAnalisisColumnasFrm(ContenedorAnalisisGlobal contenedor, Grupo grupo)
        {
            InitializeComponent();
            contenedorAnalisis = contenedor;
            noGrupos = contenedor.NoGrupos;
            filtroContactos = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
            filtroVariantes = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
            filtroSeguidos = (FiltroSignosSeguidos)grupo.GetFiltro(Filtro.SignosSeguidos.ToString());
            filtroPesos = (FiltroPesosNumericos)grupo.GetFiltro(Filtro.PesosNumericos.ToString());
            filtroDibujos = (FiltroDibujos)grupo.GetFiltro(Filtro.Dibujos.ToString());
            filtroInterrupciones = (FiltroInterrupciones)grupo.GetFiltro(Filtro.NoInterrupciones.ToString());

            grupoActual = 0;
            MostrarDatos(0);

            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);

        }
        protected void MostrarDatos(int grupo)
        {
            if (VariablesGlobales.AnalizarVX2)
            {
                AñadirVX2();
            }
            else
            {
               BorrarTab("VX2");
            }


            if (VariablesGlobales.AnalizarSeguidos)
            {
                AñadirSSeguidos();
            }
            else
            {
                BorrarTab("Signos Seguidos");
            }


            if (VariablesGlobales.AnalizarDibujos)
            {
                AñadirDibujos();
            }
            else
            {
                BorrarTab("Dibujos");
            }


            if (VariablesGlobales.AnalizarInterrupciones)
            {
                AñadirInterrupciones();
            }


            else
            {
                BorrarTab("Interrupciones");
            }
            if (VariablesGlobales.AnalizarContactos)
            {
                AñadirContactos();
            }


            else
            {
                BorrarTab("Contactos");
            }


            if (VariablesGlobales.AnalizarPesos)
            {
                AñadirPesos();
            }
            else
            {
                BorrarTab("Pesos Numéricos");
            }


            if (VariablesGlobales.AnalizarDistancias)
            {
                AñadirDistancias();
            }
            else
            {
                BorrarTab("Distancias");
            }


            if (VariablesGlobales.AnalizarControlGrupos)
            {
                AñadirControlGrupos();
            }
            else
            {
                BorrarTab("Control Grupos");
            }


            if (VariablesGlobales.AnalizarControlConjuntos)
            {
                AñadirControlConjuntos();
            }
            else
            {
                BorrarTab("Control Conjuntos");
            }


            if (VariablesGlobales.AnalizarSimetrias)
            {
                AñadirSimetrias();
            }
            else
            {
                BorrarTab("Simetrías");
            }

            if (VariablesGlobales.AnalizarSimetriasII)
            {
                AñadirSimetriasII();
            }
            else
            {
                BorrarTab("Diferencias");
            }
            if (VariablesGlobales.AnalizarValoracion)
            {
                AñadirValoraciones();
            }
            else
            {
                BorrarTab("Valoración");
            }


            if (VariablesGlobales.AnalizarCPs)
            {
            AñadirCPs();
            }
            else
            {
            BorrarTab("Columnas Probables");
            }

            if (VariablesGlobales.AnalizarFormatos)
            {
                AñadirFormatos();
            }
            else
            {
                BorrarTab("Formatos");
            }
            //Condiciones aún no contempladas
            
            BorrarTab("Formatos123");
            BorrarTab("Grupos Equipos");
        }
        protected void AñadirVX2()
        {
            //Añadir Variantes
            TabPage pagina = ObtenerPagina("VX2");
            int[,] valores = contenedorAnalisis.AnalisisGrupos.VX2;
            Controls.Analisis.CtrlAnalisisVX2 ctrlVX2 = new Controls.Analisis.CtrlAnalisisVX2(valores,filtroVariantes, contenedorAnalisis.EsAnalisisExterno);
            ctrlVX2.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlVX2);
        }
        protected void AñadirSSeguidos()
        {
            //Añadir SSeguidos
            TabPage pagina = ObtenerPagina("Signos Seguidos");
            int[,] valores = contenedorAnalisis.AnalisisGrupos.Seguidos;
            Controls.Analisis.CtrlAnalisisSSeguidos ctrlSS = new Controls.Analisis.CtrlAnalisisSSeguidos(valores, ObtenerFigurasV1X2_VFromSortedList(), ObtenerFigurasV1X2_1FromSortedList(), ObtenerFigurasV1X2_XFromSortedList(), ObtenerFigurasV1X2_2FromSortedList(), filtroSeguidos, contenedorAnalisis.EsAnalisisExterno);
            ctrlSS.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlSS);
        }
        protected void AñadirDibujos()
        {
            //Añadir SSeguidos
            TabPage pagina = ObtenerPagina("Dibujos");
            int[,] valores = contenedorAnalisis.AnalisisGrupos.Dibujos;
            Controls.Analisis.CtrlDibujos ctrlD = new Controls.Analisis.CtrlDibujos(valores, filtroDibujos, contenedorAnalisis.EsAnalisisExterno);
            ctrlD.Location = new Point(1, 0);
            pagina.Controls.Add(ctrlD);
        }
        protected void AñadirInterrupciones()
        {
            TabPage pagina = ObtenerPagina("Interrupciones");
            int[,] valores = contenedorAnalisis.AnalisisGrupos.Interrupciones;
            Controls.Analisis.CtrlAnalisisInterrupciones ctrlInt = new Controls.Analisis.CtrlAnalisisInterrupciones(valores, filtroInterrupciones, contenedorAnalisis.EsAnalisisExterno);
            ctrlInt.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlInt);
        }
        protected void AñadirContactos()
        {
            TabPage pagina = ObtenerPagina("Contactos");
            int[,] valores = contenedorAnalisis.AnalisisGrupos.Contactos;
            Controls.Analisis.CtrlAnalisisContactos ctrlCont = new Controls.Analisis.CtrlAnalisisContactos(valores, ObtenerFigurasContactosFromSortedList(), filtroContactos, contenedorAnalisis.EsAnalisisExterno);
            ctrlCont.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlCont);
        }
        protected void AñadirPesos()
        {
            TabPage pagina = ObtenerPagina("Pesos Numéricos");
            int[,] valores = contenedorAnalisis.AnalisisGrupos.Pesos;
            Controls.Analisis.CtrlAnalisisPesos ctrlPesos = new Controls.Analisis.CtrlAnalisisPesos(valores, ObtenerFigurasPesosFromSortedList(), filtroPesos, contenedorAnalisis.EsAnalisisExterno);
            ctrlPesos.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlPesos);
        }
        protected void AñadirDistancias()
        {
            TabPage pagina = ObtenerPagina("Distancias");
            int[,] valores = contenedorAnalisis.AnalisisGrupos.Distancias;
            Controls.Analisis.CtrlAnalisisDistancias ctrlDist = new Controls.Analisis.CtrlAnalisisDistancias(valores, contenedorAnalisis.EsAnalisisExterno);
            ctrlDist.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlDist);
        }
        protected void AñadirControlGrupos()
        {
            TabPage pagina = ObtenerPagina("Control Grupos");
            
            int[] valores = contenedorAnalisis.ColumnasPorFallosDeGrupos;
            Controls.Analisis.CtrlAnalisisControlGrupos ctrlGrupos = new Controls.Analisis.CtrlAnalisisControlGrupos(valores, contenedorAnalisis.EsAnalisisExterno);
            ctrlGrupos.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlGrupos);
        }
        protected void AñadirControlConjuntos()
        {
            TabPage pagina = ObtenerPagina("Control Conjuntos");

            int[] valores = contenedorAnalisis.ColumnasPorFallosDeConjuntos;
            Controls.Analisis.CtrlAnalisisControlConjuntos ctrlConj = new Controls.Analisis.CtrlAnalisisControlConjuntos(valores, contenedorAnalisis.EsAnalisisExterno);
            ctrlConj.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlConj);
        }
        protected void AñadirSimetrias()
        {
            TabPage pagina = ObtenerPagina("Simetrías");

            List<ContenedorSimetrias> contenedor = contenedorAnalisis.AnalisisGrupos.ContenedorSim;
            if (contenedor.Count == 0)
            {
                lblNombreCondicionSimetrias.Text = "Simetrías: No hay datos para analizar";
            }
            Controls.Analisis.CtrlAnalisisSimetrias ctrlSim = new Controls.Analisis.CtrlAnalisisSimetrias(contenedor, contenedorAnalisis.EsAnalisisExterno);
            ctrlSim.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlSim);
        }
        protected void AñadirSimetriasII()
        {
            TabPage pagina = ObtenerPagina("Diferencias");

            List<ContenedorDiferencias> contenedor = contenedorAnalisis.AnalisisGrupos.ContenedorDiferencias;
            if (contenedor.Count == 0)
            {
                lblNombreCondicionSimetriasII.Text = "Diferencias: No hay datos para analizar";
            }
            Controls.Analisis.CtrlAnalisisDiferencias ctrlRep = new Controls.Analisis.CtrlAnalisisDiferencias(contenedor, contenedorAnalisis.EsAnalisisExterno);
            ctrlRep.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlRep);
        }

        protected void AñadirFormatos()
        {
            ContenedorAnalisis contenedor = contenedorAnalisis.AnalisisGrupos;
            TabPage pagina = ObtenerPagina("Formatos");
            if (!contenedor.UsaFormatos)
            {
                lblNombreCondicionFormatos.Text = "Formatos: No hay datos para analizar";
            }
            TabControl tControl = new TabControl();
            tControl.Width = 800;
            tControl.Height = 500;
            tControl.Appearance = TabAppearance.FlatButtons;
            
            for (int i = 0; i < contenedor.ContenedorFormatosSignos.Count; i++)
            {
                string nombre = "Grupo " + Convert.ToString(i + 1);
                TabPage pgn = new TabPage(nombre);
                Controls.Analisis.CtrlAnalisisFormatosSignos ctrlFS = new Controls.Analisis.CtrlAnalisisFormatosSignos(contenedor.ContenedorFormatosSignos[i].AciertosGlobalesFormatos, contenedor.ContenedorFormatosSignos[i].AciertosFormatosSignos, contenedorAnalisis.EsAnalisisExterno);
                ctrlFS.Location = new Point(1, 35);
                pgn.Controls.Add(ctrlFS);
                pgn.AutoScroll = true;
                pgn.BackColor = Color.Bisque;
                tControl.TabPages.Add(pgn);
                tControl.Location = new Point(1, 35);
                pagina.Controls.Add(tControl);
            }
            


        }
        protected void AñadirValoraciones()
        {
            TabPage pagina = ObtenerPagina("Valoración");

            ContenedorAnalisis contenedor = contenedorAnalisis.AnalisisGrupos;

            if (!contenedor.UsaValoraciones)
            {
                lblNombreCondicionValoracion.Text = "Valoración: No hay datos para analizar";
            }
            Controls.Analisis.CtrlAnalisisValoraciones ctrlVal = new Controls.Analisis.CtrlAnalisisValoraciones(contenedor, contenedorAnalisis.EsAnalisisExterno);
            ctrlVal.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlVal);
        }
        protected void AñadirCPs()
        {
            TabPage pagina = ObtenerPagina("Columnas Probables");

            ContenedorAnalisis contenedor = contenedorAnalisis.AnalisisGrupos;

            if (!contenedor.UsaCPs)
            {
                lblNombreCondicionCPs.Text = "Columnas probables: No hay datos para analizar";
            }
            Controls.Analisis.CtrlAnalisisCPs ctrlCps = new Controls.Analisis.CtrlAnalisisCPs(contenedor, contenedorAnalisis.EsAnalisisExterno);
            ctrlCps.Location = new Point(1, 35);
            pagina.Controls.Add(ctrlCps);
        }
        protected TabPage ObtenerPagina(string texto)
        {
            TabPage pagina = new TabPage();
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                pagina = tabControl1.TabPages[i];
                if (pagina.Text == texto)
                {
                    break;
                }
            }
            return pagina;
        }
        protected List<FiguraCondicion> ObtenerFigurasContactosFromSortedList()
        {
            List<FiguraCondicion> arrayF = new List<FiguraCondicion>();
            FiguraCondicion fig = new FiguraCondicion();
            for (int i = 0; i < contenedorAnalisis.AnalisisGrupos.SortedFigurasContactos.Count; i++)
            {
                long a = contenedorAnalisis.AnalisisGrupos.SortedFigurasContactos.Keys[i];
                int b = contenedorAnalisis.AnalisisGrupos.SortedFigurasContactos.Values[i];
                fig.Figura = a;
                fig.Apariciones = b;

                arrayF.Add(fig);
                fig = new FiguraCondicion();
            }
            arrayF.Sort(new FigurasComparer());
            return arrayF;
        }
        protected List<FiguraCondicion> ObtenerFigurasV1X2_VFromSortedList()
        {
            List<FiguraCondicion> arrayF = new List<FiguraCondicion>();
            FiguraCondicion fig = new FiguraCondicion();
            for (int i = 0; i < contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_V.Count; i++)
            {
                long a = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_V.Keys[i];
                int b = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_V.Values[i];
                fig.Figura = a;
                fig.Apariciones = b;

                arrayF.Add(fig);
                fig = new FiguraCondicion();
            }
            arrayF.Sort(new FigurasComparer());
            return arrayF;
        }
        protected List<FiguraCondicion> ObtenerFigurasV1X2_1FromSortedList()
        {
            List<FiguraCondicion> arrayF = new List<FiguraCondicion>();
            FiguraCondicion fig = new FiguraCondicion();
            for (int i = 0; i < contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_1.Count; i++)
            {
                long a = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_1.Keys[i];
                int b = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_1.Values[i];
                fig.Figura = a;
                fig.Apariciones = b;

                arrayF.Add(fig);
                fig = new FiguraCondicion();
            }
            arrayF.Sort(new FigurasComparer());
            return arrayF;
        }
        protected List<FiguraCondicion> ObtenerFigurasV1X2_XFromSortedList()
        {
            List<FiguraCondicion> arrayF = new List<FiguraCondicion>();
            FiguraCondicion fig = new FiguraCondicion();
            for (int i = 0; i < contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_X.Count; i++)
            {
                long a = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_X.Keys[i];
                int b = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_X.Values[i];
                fig.Figura = a;
                fig.Apariciones = b;

                arrayF.Add(fig);
                fig = new FiguraCondicion();
            }
            arrayF.Sort(new FigurasComparer());
            return arrayF;
        }
        protected List<FiguraCondicion> ObtenerFigurasV1X2_2FromSortedList()
        {
            List<FiguraCondicion> arrayF = new List<FiguraCondicion>();
            FiguraCondicion fig = new FiguraCondicion();
            for (int i = 0; i < contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_2.Count; i++)
            {
                long a = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_2.Keys[i];
                int b = contenedorAnalisis.AnalisisGrupos.SortedFigurasV1X2_2.Values[i];
                fig.Figura = a;
                fig.Apariciones = b;

                arrayF.Add(fig);
                fig = new FiguraCondicion();
            }
            arrayF.Sort(new FigurasComparer());
            return arrayF;
        }
        protected List<FiguraCondicion> ObtenerFigurasPesosFromSortedList()
        {
            List<FiguraCondicion> arrayF = new List<FiguraCondicion>();
            FiguraCondicion fig = new FiguraCondicion();
            for (int i = 0; i < contenedorAnalisis.AnalisisGrupos.SortedFigurasPesos.Count; i++)
            {
                long a = contenedorAnalisis.AnalisisGrupos.SortedFigurasPesos.Keys[i];
                int b = contenedorAnalisis.AnalisisGrupos.SortedFigurasPesos.Values[i];
                fig.Figura = a;
                fig.Apariciones = b;

                arrayF.Add(fig);
                fig = new FiguraCondicion();
            }
            arrayF.Sort(new FigurasComparer());
            return arrayF;
        }
        protected void BorrarTab(string textoTab)
        {
                TabPage pagina = ObtenerPagina(textoTab);
                tabControl1.TabPages.Remove(pagina);
        }


    }
}
