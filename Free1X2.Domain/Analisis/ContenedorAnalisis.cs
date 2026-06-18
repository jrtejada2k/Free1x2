// Free1X2 · WinUI 3 — WIN3
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
using Free1X2.MotorCalculo;

namespace Free1X2.Analisis
{
    
    public class ContenedorAnalisis
    {
        #region Variables Generales
        //Variables generales
        protected bool usaSimetrias;
        protected bool usaValoraciones;
        protected bool usaCPs;
        protected bool usaFormatos;
        protected bool usaDiferencias = true;

        List<IFiltro> filtrosTemp = new List<IFiltro>();


        #endregion

        //Variables Específicas
        #region VX2
        int[,] vx2;
        
        #endregion
        #region SSeguidos
        int[,] seguidos;
        SortedList<long, int> sortedFigurasV1X2_V = new SortedList<long, int>();
        SortedList<long, int> sortedFigurasV1X2_1 = new SortedList<long, int>();
        SortedList<long, int> sortedFigurasV1X2_X = new SortedList<long, int>();
        SortedList<long, int> sortedFigurasV1X2_2 = new SortedList<long, int>();

        
        #endregion
        #region Contactos
        int[,] contactos;
        SortedList<long, int> sortedFigurasContactos = new SortedList<long, int>();

        #endregion
        #region Dibujos
        int[,] dibujos; 
        #endregion
        #region Distancias
        int[,] distancias; 
        #endregion
        #region Interrupciones
        int[,] interrupciones; 
        #endregion
        #region Pesos
        int[,] pesos;
        SortedList<long, int> sortedFigurasPesos = new SortedList<long, int>();

        #endregion
        #region Valoración
        List<int> valoracionGlobal = new List<int>();
        List<int> valoracionUnos = new List<int>();
        List<int> valoracionEquis = new List<int>();
        List<int> valoracionDoses = new List<int>();
        string tipoValoracion = "";
        protected List<int> novaloresValoracionGlobal = new List<int>();
        protected List<int> novaloresValoracionUnos = new List<int>();
        protected List<int> novaloresValoracionEquis = new List<int>();
        protected List<int> novaloresValoracionDoses = new List<int>(); 
        #endregion
        #region Simetrías
        protected List<int> noAciertosSimetrias = new List<int>();
        List<ContenedorSimetrias> contenedorSim = new List<ContenedorSimetrias>(); 
        #endregion
        #region Diferencias
        List<ContenedorDiferencias> contenedorRep = new List<ContenedorDiferencias>();
        
        #endregion       
        protected int[] aciertosFormatosSignos;
        protected SortedList<int, int> aciertosGlobalesFormatos = new SortedList<int, int>();
        protected List<ContenedorColumnasProbables> contColumnasProbables = new List<ContenedorColumnasProbables>();
        List<ContenedorFormatos> contenedorFormatosSignos = new List<ContenedorFormatos>();

        public ContenedorAnalisis(int numPartidos)
        {
            Inicializa(numPartidos);
        }

        public void IncrementarContador(long columna)
        {
            for (int i = 0; i < FiltrosTemp.Count; i++)
            {
                IFiltro _filtro = FiltrosTemp[i];
                if (_filtro.AnalisisActivo)
                {
                    switch (_filtro.NombreFiltro)
                    {
                        case Filtro.NoVariantes:
                            FiltroNoVariantes filtroVX2 = (FiltroNoVariantes)_filtro;
                            VX2[0, filtroVX2.NoEquis + filtroVX2.NoDoses]++;
                            VX2[1, filtroVX2.NoEquis]++;
                            VX2[2, filtroVX2.NoDoses]++;
                            break;
                        case Filtro.Contactos:
                                FiltroContactos filtroCont = (FiltroContactos)_filtro;
                                contactos[0, filtroCont.NoContactos1X]++;
                                contactos[1, filtroCont.NoContactos12]++;
                                contactos[2, filtroCont.NoContactosX2]++;

                                contactos[3, filtroCont.NoContactos11]++;
                                contactos[4, filtroCont.NoContactosXX]++;
                                contactos[5, filtroCont.NoContactos22]++;

                                contactos[6, filtroCont.NoContactos1V]++;
                                contactos[7, filtroCont.NoContactosXV]++;
                                contactos[8, filtroCont.NoContactos2V]++;
                                contactos[9, filtroCont.NoContactosVV]++;
                                if (VariablesGlobales.AnalizarFigurasContactos)
                                {
                                    FiguraCondicion figura = filtroCont.ObtenerFiguraLong();
                                    if (sortedFigurasContactos.ContainsKey(figura.Figura))
                                    {
                                        sortedFigurasContactos[figura.Figura]++;
                                    }
                                    else
                                    {
                                        sortedFigurasContactos.Add(figura.Figura, 1);
                                    }
                                }
                            
                            break;
                        case Filtro.Distancias:
                            FiltroDistancias filtroDist = (FiltroDistancias)_filtro;
                            distancias[0, filtroDist.NoDistanciasVariantes]++;
                            distancias[1, filtroDist.NoDistanciasUnos]++;
                            distancias[2, filtroDist.NoDistanciasEquis]++;
                            distancias[3, filtroDist.NoDistanciasDoses]++;
                            break;
                        case Filtro.NoInterrupciones:
                            FiltroInterrupciones filtroInt = (FiltroInterrupciones)_filtro;
                            Interrupciones[0, filtroInt.NoInterrupciones]++;
                            Interrupciones[1, filtroInt.NoInterrupcionesVariantes]++;
                            Interrupciones[2, filtroInt.NoInterrupcionesUnos]++;
                            Interrupciones[3, filtroInt.NoInterrupcionesEquis]++;
                            Interrupciones[4, filtroInt.NoInterrupcionesDoses]++;

                            Interrupciones[5, filtroInt.NoInterrupcionesGlobalesSeguidas]++;
                            Interrupciones[6, filtroInt.NoInterrupcionesVariantesSeguidas]++;
                            Interrupciones[7, filtroInt.NoInterrupcionesUnosSeguidos]++;
                            Interrupciones[8, filtroInt.NoInterrupcionesEquisSeguidas]++;
                            Interrupciones[9, filtroInt.NoInterrupcionesDosesSeguidas]++;

                            break;
                        case Filtro.PesosNumericos:
                            FiltroPesosNumericos filtroPesos = (FiltroPesosNumericos)_filtro;
                            Pesos[0, filtroPesos.PesoGlobal]++;
                            Pesos[1, filtroPesos.PesoVariantes]++;
                            Pesos[2, filtroPesos.PesoUnos]++;
                            Pesos[3, filtroPesos.PesoEquis]++;
                            Pesos[4, filtroPesos.PesoDoses]++;
                            if (VariablesGlobales.AnalizarFigurasPesos)
                            {
                                filtroPesos.ObtenerFiguraLong();

                                long figura = filtroPesos.FiguraPesos.Figura;                                                              

                                if (sortedFigurasPesos.ContainsKey(figura))
                                {
                                    sortedFigurasPesos[figura]++;
                                }
                                else
                                {
                                    sortedFigurasPesos.Add(figura, 1);
                                }
                            }
                            break;
                        case Filtro.SignosSeguidos:
                            FiltroSignosSeguidos filtroSeguidos = (FiltroSignosSeguidos)_filtro;
                            Seguidos[0, filtroSeguidos.NoVariantesSeguidas]++;
                            Seguidos[1, filtroSeguidos.NoUnosSeguidos]++;
                            Seguidos[2, filtroSeguidos.NoEquisSeguidas]++;
                            Seguidos[3, filtroSeguidos.NoDosesSeguidos]++;
                            if (VariablesGlobales.AnalizarFigurasV1X2)
                            {
                                #region Indicar FigurasV1X2
                                long figura = filtroSeguidos.ObtenFiguraV(columna);
                                 
                                if (sortedFigurasV1X2_V.ContainsKey(figura))
                                {
                                    sortedFigurasV1X2_V[figura]++;
                                }
                                else
                                {
                                    sortedFigurasV1X2_V.Add(figura, 1);
                                }

                                figura = filtroSeguidos.ObtenFigura1(columna);
                                if (sortedFigurasV1X2_1.ContainsKey(figura))
                                {
                                    sortedFigurasV1X2_1[figura]++;
                                }
                                else
                                {
                                    sortedFigurasV1X2_1.Add(figura, 1);
                                }


                                figura = filtroSeguidos.ObtenFiguraX(columna);
                                if (sortedFigurasV1X2_X.ContainsKey(figura))
                                {
                                    sortedFigurasV1X2_X[figura]++;
                                }
                                else
                                {
                                    sortedFigurasV1X2_X.Add(figura, 1);
                                }


                                figura = filtroSeguidos.ObtenFigura2(columna);
                                if (sortedFigurasV1X2_2.ContainsKey(figura))
                                {
                                    sortedFigurasV1X2_2[figura]++;
                                }
                                else
                                {
                                    sortedFigurasV1X2_2.Add(figura, 1);
                                }

                                #endregion
                            }
                            break;
                        case Filtro.Dibujos:
                            FiltroDibujos filtroDib = (FiltroDibujos)_filtro;
                            Dibujos[filtroDib.NoEquisCol, filtroDib.NoDosesCol]++;
                            break;
                        case Filtro.Simetrias:
                            FiltroSimetrias filtroSim = (FiltroSimetrias)_filtro;
                            IncrementarColumnasSimetrias(filtroSim.AciertosSimetria);
                            break;
                        case Filtro.ValoracionSignos:
                            FiltroValoracionSignos filtroVal = (FiltroValoracionSignos)_filtro;
                            if (filtroVal.IsActive)
                            {
                                IncrementarColumnasValoracion((float)filtroVal.ValoracionResultado, (float)filtroVal.ValoracionUnos, (float)filtroVal.ValoracionEquis, (float)filtroVal.ValoracionDoses);
                                TipoValoracion = filtroVal.TipoValoracion;
                            }
                            break;
                        case Filtro.ColProbables:
                            
                            FiltroColProbables filtroCPs = (FiltroColProbables)_filtro;
                            if (filtroCPs.IsActive)
                            {
                                for (int j = 0; j < filtroCPs.ColProbables.Count; j++)
                                {
                                    ColumnaProbable cp = filtroCPs.ColProbables[j];
                                    ContColumnasProbables[j].IncrementarAC(cp.NoAC);
                                    ContColumnasProbables[j].IncrementarACS(cp.NoACS);
                                    ContColumnasProbables[j].IncrementarFS(cp.NoFS);
                                }
                            }
                            break;
                        case Filtro.FormatosSignos:
                            FiltroFormatosSignos filtroFS = (FiltroFormatosSignos)_filtro;
                            if (filtroFS.IsActive)
                            {
                                //El filtro contiene un arrayList que contiene los grupos de formatos llamada FormatosSignos
                                //este arraylist contiene objetos FormatosSignos que contiene un arrayList
                                //llamado lineasFormatos que contiene los formatos
                                //Hay que calcular las lineas y globales para cada grupo de formatos.
                                //Convertir el arrayList FormatosSignos en List<FormatosSignos>
                                for (int j = 0; j < filtroFS.FormatosSignos.Count; j++)
                                {
                                    FormatosSignos fS = filtroFS.FormatosSignos[j];
                                    contenedorFormatosSignos[j].AciertosFormatosSignos[fS.NoLineasValidas]++;
                                    IncrementarAciertosGlobalesFormatos(contenedorFormatosSignos[j].AciertosGlobalesFormatos, fS.NoAparicionesGlobal);
                                }
                            }
                            break;

                        case Filtro.Diferencias:
                            FiltroDiferencias filtroRep = (FiltroDiferencias)_filtro;
                            if (filtroRep.IsActive)
                            {
                                for (int j = 0; j < filtroRep.Diferencias.Count; j++)
                                {
                                    Diferencia repet = filtroRep.Diferencias[j];
                                    if (repet.AnalizaVX2Dib)
                                    {
                                        contenedorRep[j].Variantes[repet.NoVariantes]++;
                                        contenedorRep[j].Equis[repet.NoEquis]++;
                                        contenedorRep[j].Doses[repet.NoDoses]++;
                                        contenedorRep[j].Dibujos[repet.NoDibujos]++;

                                    }
                                    if (repet.AnalizaInterrupciones)
                                    {
                                        contenedorRep[j].Interrupciones[repet.NoInterrupciones]++;
                                    }
                                    if (repet.AnalizaFormatos)
                                    {
                                        contenedorRep[j].Formatos[repet.NoFormatos]++;
                                    }
                                }
                                
                            }
                            break;
                    }
                }
            }
            FiltrosTemp.Clear();
           
        }
        private void Inicializa(int numPartidos)
        {
            vx2 = new int[3, numPartidos + 1];

            distancias = new int[4, numPartidos];
            interrupciones = new int[10, numPartidos + 1];
            pesos = new int[5,10];
           
            int maxAciertos = numPartidos + 1;
            
            seguidos = new int[4, numPartidos + 1];
            dibujos = new int[maxAciertos, numPartidos + 1];
            contactos = new int[10, numPartidos];
        }
        public void InicializaFormatosSignos(List<int> noFormatos)
        {
            for (int i = 0; i < noFormatos.Count; i++)
            {
                ContenedorFormatos cont = new ContenedorFormatos();
                cont.AciertosFormatosSignos = new int[noFormatos[i] + 1];
                cont.AciertosGlobalesFormatos.Add(0, 0);
                contenedorFormatosSignos.Add(cont);
            }
        }
        public void InicializarSimetrias()
        {
            for (int i = 0; i < NoAciertosSimetrias.Count; i++)
            {
                ContenedorSimetrias contSim = new ContenedorSimetrias();
                contSim.Aciertos = NoAciertosSimetrias[i];
                contenedorSim.Add(contSim);
            }
        }
        public void InicializarValoraciones()
        {
            for (int i = 0; i < novaloresValoracionGlobal[novaloresValoracionGlobal.Count - 1] + 1; i++)
            {
                valoracionGlobal.Add(0);
            }
            for (int i = 0; i < novaloresValoracionUnos[novaloresValoracionUnos.Count - 1] + 1; i++)
            {
                valoracionUnos.Add(0);
            }
            for (int i = 0; i < novaloresValoracionEquis[novaloresValoracionEquis.Count - 1] + 1; i++)
            {
                valoracionEquis.Add(0);
            }
            for (int i = 0; i < novaloresValoracionDoses[novaloresValoracionDoses.Count - 1] + 1; i++)
            {
                valoracionDoses.Add(0);
            }
        }
        public void InicializarCPs(List<ColumnaProbable> cps)
        {
            for (int i = 0; i < cps.Count; i++)
            {
                ContenedorColumnasProbables contenedorCps = new ContenedorColumnasProbables(cps[i].Pronosticos.Length);
                ContColumnasProbables.Add(contenedorCps);
            }
        }
        public void IncrementarColumnasSimetrias(int aciertos)
        {
            for (int i = 0; i < contenedorSim.Count; i++)
            {
                ContenedorSimetrias contSim = contenedorSim[i];
                if (contSim.Aciertos == aciertos)
                {
                    contSim.Columnas++;
                    break;
                }
            }
        }
        public void IncrementarColumnasValoracion(float valorGlobal, float valorUnos, float valorEquis, float valorDoses)
        {
            int keyGlobal = (int)Math.Round(valorGlobal,0);
            int keyUnos = (int)Math.Round(valorUnos, 0);
            int keyEquis = (int)Math.Round(valorEquis, 0);
            int keyDoses = (int)Math.Round(valorDoses, 0);

            valoracionGlobal[keyGlobal]++;
            valoracionUnos[keyUnos]++;
            valoracionEquis[keyEquis]++;
            valoracionDoses[keyDoses]++;

        }
        protected void IncrementarAciertosGlobalesFormatos(SortedList<int,int> sortedFormatos, int aciertos)
        {
            if (sortedFormatos.Keys.Contains(aciertos))
            {
                sortedFormatos[aciertos]++;
            }
            else
            {
                sortedFormatos.Add(aciertos, 1);
            }
        }
        #region Condiciones Comunes
        //Condiciones Comunes
        public int[,] VX2
        {
            get
            {
                return vx2;
            }
            set
            {
                vx2 = value;
            }
        }
        public int[,] Distancias
        {
            get
            {
                return distancias;
            }
            set
            {
                distancias = value;
            }
        }
        public int[,] Interrupciones
        {
            get
            {
                return interrupciones;
            }
            set
            {
                interrupciones = value;
            }
        }
        public int[,] Pesos
        {
            get
            {
                return pesos;
            }
            set
            {
                pesos = value;
            }
        }
        public int[,] Seguidos
        {
            get
            {
                return seguidos;
            }
            set
            {
                seguidos = value;
            }
        }
        public int[,] Dibujos
        {
            get
            {
                return dibujos;
            }
            set
            {
                dibujos = value;
            }
        } 
        #endregion

        //Condiciones Especiales
        #region Generales
        public bool UsaSimetrias
        {
            get { return usaSimetrias; }
            set { usaSimetrias = value; }
        }
        public bool UsaDiferencias
        {
            get { return usaDiferencias; }
            set { usaDiferencias = value; }
        }
        public bool UsaValoraciones
        {
            get { return usaValoraciones; }
            set { usaValoraciones = value; }
        }
        public bool UsaCPs
        {
            get { return usaCPs; }
            set { usaCPs = value; }
        }
        public bool UsaFormatos
        {
            get { return usaFormatos; }
            set { usaFormatos = value; }
        }
        public List<IFiltro> FiltrosTemp
        {
            get { return filtrosTemp; }
            set { filtrosTemp = value; }
        }

        #endregion
        #region Simetrías
        public List<int> NoAciertosSimetrias
        {
            get { return noAciertosSimetrias; }
            set { noAciertosSimetrias = value; }
        }
        public List<ContenedorSimetrias> ContenedorSim
        {
            get { return contenedorSim; }
            set { contenedorSim = value; }
        } 
        #endregion
        #region Valoración
        public List<int> NoValoresValoracionGlobal
        {
            get { return novaloresValoracionGlobal; }
            set { novaloresValoracionGlobal = value; }
        }
        public List<int> NoValoresValoracionUnos
        {
            get { return novaloresValoracionUnos; }
            set { novaloresValoracionUnos = value; }
        }
        public List<int> NoValoresValoracionEquis
        {
            get { return novaloresValoracionEquis; }
            set { novaloresValoracionEquis = value; }
        }
        public List<int> NoValoresValoracionDoses
        {
            get { return novaloresValoracionDoses; }
            set { novaloresValoracionDoses = value; }
        }
        public List<int> ValoracionesGlobales
        {
            get { return valoracionGlobal; }
        }
        public List<int> ValoracionesUnos
        {
            get { return valoracionUnos; }
        }
        public List<int> ValoracionesEquis
        {
            get { return valoracionEquis; }
        }
        public List<int> ValoracionesDoses
        {
            get { return valoracionDoses; }
        }
        public string TipoValoracion
        {
            get { return tipoValoracion; }
            set { tipoValoracion = value; }
        } 
        #endregion
        #region Contactos
        public int[,] Contactos
        {
            get
            {
                return contactos;
            }
            set
            {
                contactos = value;
            }
        }

        public SortedList<long, int> SortedFigurasContactos
        {
            get { return sortedFigurasContactos; }
            set { sortedFigurasContactos = value; }
        }
        public SortedList<long, int> SortedFigurasV1X2_V
        {
            get { return sortedFigurasV1X2_V; }
            set { sortedFigurasV1X2_V = value; }
        }
        public SortedList<long, int> SortedFigurasV1X2_1
        {
            get { return sortedFigurasV1X2_1; }
            set { sortedFigurasV1X2_1 = value; }
        }
        public SortedList<long, int> SortedFigurasV1X2_X
        {
            get { return sortedFigurasV1X2_X; }
            set { sortedFigurasV1X2_X = value; }
        }
        public SortedList<long, int> SortedFigurasV1X2_2
        {
            get { return sortedFigurasV1X2_2; }
            set { sortedFigurasV1X2_2 = value; }
        }
        public SortedList<long, int> SortedFigurasPesos
        {
            get { return sortedFigurasPesos; }
            set { sortedFigurasPesos = value; }
        }
        #endregion 
        #region CPs
        public List<ContenedorColumnasProbables> ContColumnasProbables
        {
            get { return contColumnasProbables ; }
            set { contColumnasProbables = value; }
        } 
        #endregion
        public int[] AciertosFormatosSignos
        {
            get { return aciertosFormatosSignos; }
            set { aciertosFormatosSignos = value; }
        }
        public List<ContenedorFormatos> ContenedorFormatosSignos
        {
            get { return contenedorFormatosSignos; }
            set { contenedorFormatosSignos = value; }
        }

        public List<ContenedorDiferencias> ContenedorDiferencias
        {
            get { return contenedorRep; }
            set { contenedorRep = value; }
        }
    }
}
