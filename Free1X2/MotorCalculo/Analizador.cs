// created on 10/08/2003 at 17:05
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.com
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
using System.Windows.Forms;

using Free1X2.EntradaSalida;
using Free1X2.UI;
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
    public class Analizador
    {
        private GeneradorColumnas gc;

        private string[] pronosticos;
        private long pronosticoBase;

        private string archColumnasBase = "";
        private string completarCon = "";
        private int noColsAnalizadas;
        private int noColsAceptadas;
        private int numPartidos = VariablesGlobales.NumeroPartidos;
        protected Analisis.ContenedorAnalisisGlobal contenedorAnalisisGlobal;

        private ControladorGrupos ctrlGrupos;

        private bool guardarCols;
        private bool analizarCols;
        private IArchivoColumnas archivoCols;
        private ControladorIfThen ifThen;

        //variable usada para llamar a DoEvents()
        private DateTime dt1 = DateTime.Now;

        public Analizador()
        {
            ctrlGrupos = new ControladorGrupos();
            GrupoPartidos grupoPartidos = new GrupoPartidos();
            grupoPartidos.CtrlGrupos = ctrlGrupos;
            ctrlGrupos.GruposPartidos = grupoPartidos;

            //crea grupo boleto base grupo = 0			
            Grupo grupo = new Grupo();
            grupo.EsGrupoBase = true;
            ctrlGrupos.GruposPartidos.AddGrupo(grupo);
         }
        public Analizador(int numeroPartidos)
        {
            ctrlGrupos = new ControladorGrupos();
            GrupoPartidos grupoPartidos = new GrupoPartidos();
            grupoPartidos.CtrlGrupos = ctrlGrupos;
            ctrlGrupos.GruposPartidos = grupoPartidos;

            //crea grupo boleto base grupo = 0			
            Grupo grupo = new Grupo();
            grupo.EsGrupoBase = true;
            ctrlGrupos.GruposPartidos.AddGrupo(grupo);

            numPartidos = numeroPartidos;
        }

        public void AnalizaColumna(long columna)
        {
            bool columnaValida = false;


            if (compruebaPronostico(columna))
            {
                ctrlGrupos.RecalcularControladorGrupos();

                columnaValida = ctrlGrupos.AnalizaColumna(columna);
            }

            // Analiza el controlador If-Then
            if (IfThen != null)
            {
                if (!IfThen.EsVacio && IfThen.EsActivo)
                {
                    columnaValida = IfThen.CompruebaPronostico(columna, GruposPartidos);
                }
            }

            if (DateTime.Now.Subtract(dt1).Milliseconds > 800)

            {
                //Permitir que el programa procese eventos
                Application.DoEvents();
                dt1 = DateTime.Now;
            }
            
            // Aumentamos el contador
            noColsAnalizadas++;

            if (columnaValida)
            {
                noColsAceptadas++;
                if (guardarCols)
                {
                    archivoCols.GuardarCols(UtilColumnas.ConvLongToStr(columna));
                }

            }
        }
        public void AnalizaColumna(long columna, bool analizar)
        {
            bool columnaValida = true;

            ctrlGrupos.RecalcularControladorGrupos();

            columnaValida = ctrlGrupos.AnalizaColumna(columna,ContenedorAnalisisColumnasGlobal);
            
            // Analiza el controlador If-Then
            if (IfThen != null)
            {
                if (!IfThen.EsVacio && IfThen.EsActivo)
                {
                    if (!IfThen.CompruebaPronostico(columna, GruposPartidos)) columnaValida = false;
                }
            }

            //si 0.8 segundos transcurridos
            if (DateTime.Now.Subtract(dt1).Milliseconds > 800)
            {
                //Permitir que el programa procese eventos
                Application.DoEvents();
                dt1 = DateTime.Now;
            }

            // Aumentamos el contador
            noColsAnalizadas++;

            if (columnaValida)
            {
                ContenedorAnalisisColumnasGlobal.AnalisisGrupos.IncrementarContador(columna);

                noColsAceptadas++;
            }
        }
        public bool compruebaPronostico(long columna)
        {
            return (pronosticoBase & columna) == columna;
        }

        public bool compruebaPronostico(string columna)
        {
            for (int i = 0; i < columna.Length; i++)
            {
                string signo = columna.Substring(i, 1);
                string pr = Pronosticos[i];
                if (pr.IndexOf(signo) < 0) return false;
            }
            return true;
        }

        public void AnalizaCombinacion(string archColsResul)
        {
            guardarCols = true;
            archivoCols = new ArchivoColumnasTexto(archColsResul);
            // Si se graba el archivo NO se analizan las columnas
            AnalizaCombinacion(false);

            archivoCols.Cerrar();

            archivoCols = null;
            guardarCols = false;

        }

        public void AnalizaCombinacion(int _numPartidos)
        {
            numPartidos = _numPartidos;
            AnalizaCombinacion(true);
        }
        protected FiltroSimetrias UsaSimetrias()
        {
            //Sólo miramos si usa Simetrías en el boleto base
            Grupo grupoTemp = GruposPartidos[0];
            FiltroSimetrias filtroSim = (FiltroSimetrias) grupoTemp.GetFiltro("Simetrias");
            return filtroSim;
        }
        protected FiltroDiferencias UsaDiferencias()
        {
            //Sólo miramos si usa Diferencias en el boleto base
            Grupo grupoTemp = GruposPartidos[0];
            FiltroDiferencias filtroRep = (FiltroDiferencias)grupoTemp.GetFiltro("Diferencias");
            return filtroRep;
        }
        protected FiltroValoracionSignos UsaValoraciones()
        {
            //Sólo miramos si usa Val en el boleto base
            Grupo grupoTemp = GruposPartidos[0];
            FiltroValoracionSignos filtroVal = (FiltroValoracionSignos)grupoTemp.GetFiltro("ValoracionSignos");
            return filtroVal;
        }
        protected FiltroColProbables UsaCPs()
        {
            //Sólo miramos si usa CPs en el boleto base
            Grupo grupoTemp = GruposPartidos[0];
            FiltroColProbables filtroCPs = (FiltroColProbables)grupoTemp.GetFiltro(Filtro.ColProbables.ToString());
            return filtroCPs;
        }
        protected List<int> ObtenerArrayAciertos(string valores)
        {
            List<int> aciertos = new List<int>();
            if (valores != "")
            {
                string[] aciertosTemp = valores.Split('#');
                for (int i = 0; i < aciertosTemp.Length; i++)
                {
                    if (aciertosTemp[i].LastIndexOf('-') == -1)
                    {
                        //Es un acierto individual

                        aciertos.Add((Convert.ToInt32(aciertosTemp[i])));
                    }
                    else
                    {
                        string[] aciertosIntervalo = aciertosTemp[i].Split('-');
                        for (int j = (int)Math.Round(Convert.ToDouble(aciertosIntervalo[0]), 0); j <= (int)Math.Round(Convert.ToDouble(aciertosIntervalo[1]), 0); j++)
                        {
                            aciertos.Add(j);
                        }
                    }
                }
            }
            aciertos.Sort();
            return aciertos;
        }
        public void AnalizaCombinacion(bool analizarColumnas)
        {
            // Marca si deseamos análisis
            analizarCols = analizarColumnas;

            // Si se pide análisis, inicializa el Contenedor
            if (analizarCols)
            {
                InicializarContenedorAnalisis(false);
            }
            else
            {
                ContenedorAnalisisColumnasGlobal = null;
            }

            InicializaContadores();
            InicializaParametros();
            InicializaPronosticoBase();

            if (!archColumnasBase.Equals(""))
            {
                //usa columnas desde archivo.
                gc = new GeneradorColumnas(archColumnasBase);
            }
            else
            {
                //usa pronosticos de la combinacion base.				
                gc = new GeneradorColumnas(pronosticos);
            }

            gc.AnalizadorColumnas = this;
            if (analizarCols)
            {
                gc.GenerarColumnas(analizarCols, false);
            }
            else
            {
                gc.GenerarColumnas();
            }
            if (ContenedorAnalisisColumnasGlobal != null && noColsAceptadas > 0)
            {
                // Muestra la pantalla de análisis

                VisorAnalisisColumnasFrm visor = new VisorAnalisisColumnasFrm(ContenedorAnalisisColumnasGlobal, GruposPartidos[0]);
                Application.DoEvents();
                visor.Show();
                
            }
        }
        public void AnalizaCombinacion(bool analizarColumnas, bool esArchivo)
        {
            // Marca si deseamos análisis
            analizarCols = analizarColumnas;

            // Si se pide análisis, inicializa el Contenedor
            if (analizarCols)
            {
                InicializarContenedorAnalisis(esArchivo);
            }
            else
            {
                ContenedorAnalisisColumnasGlobal = null;
            }

            InicializaContadores();
            InicializaParametros();
            InicializaPronosticoBase();

            if (!archColumnasBase.Equals(""))
            {
                //usa columnas desde archivo.
                gc = new GeneradorColumnas(archColumnasBase);
            }
            else
            {
                //usa pronosticos de la combinacion base.				
                gc = new GeneradorColumnas(pronosticos);
            }

            gc.AnalizadorColumnas = this;
            if (analizarCols)
            {
                gc.GenerarColumnas(analizarCols,true);
            }
            else
            {
                gc.GenerarColumnas();
            }
            if (ContenedorAnalisisColumnasGlobal != null && noColsAceptadas > 0)
            {
                // Muestra la pantalla de análisis
                VisorAnalisisColumnasFrm visor = new VisorAnalisisColumnasFrm(ContenedorAnalisisColumnasGlobal, GruposPartidos[0]);
                visor.Show();
            }
        }

        protected void InicializarContenedorAnalisis(bool esArchivo)
        {
            if (!esArchivo)
            {
                Grupo grupoTemp = GruposPartidos[0];
                ContenedorAnalisisColumnasGlobal = new Analisis.ContenedorAnalisisGlobal(CtrlGrupos.GruposPartidos.Count, CtrlGrupos.ControlesGrupos.Count, VariablesGlobales.NumeroPartidos);
                FiltroSimetrias filtroSim = UsaSimetrias();
                FiltroDiferencias filtroRep = UsaDiferencias();
                FiltroValoracionSignos filtroVal = UsaValoraciones();
                FiltroColProbables filtroCPs = UsaCPs();
                FiltroFormatosSignos filtroFS = (FiltroFormatosSignos)grupoTemp.GetFiltro("FormatosSignos");
                ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaSimetrias = filtroSim.IsActive;
                ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaDiferencias = filtroRep.IsActive;
                ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaValoraciones = filtroVal.IsActive;
                ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaCPs = filtroCPs.IsActive;
                ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaFormatos = filtroFS.IsActive;

                if (ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaSimetrias)
                {
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.NoAciertosSimetrias = filtroSim.ArrayAciertos;
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.InicializarSimetrias();
                }
                if (ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaDiferencias)
                {
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.ContenedorDiferencias = new List<Analisis.ContenedorDiferencias>();

                    for (int i = 0; i < filtroRep.Diferencias.Count; i++)
                    {
                        Analisis.ContenedorDiferencias cont = new Analisis.ContenedorDiferencias(filtroRep.Diferencias[i].PartidosSimetricos.Count);
                        ContenedorAnalisisColumnasGlobal.AnalisisGrupos.ContenedorDiferencias.Add(cont);

                    }
                }
                if (ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaValoraciones)
                {
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.NoValoresValoracionGlobal = ObtenerArrayAciertos(filtroVal.ValorGlobal);
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.NoValoresValoracionUnos = ObtenerArrayAciertos(filtroVal.ValorUnos);
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.NoValoresValoracionEquis = ObtenerArrayAciertos(filtroVal.ValorEquis);
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.NoValoresValoracionDoses = ObtenerArrayAciertos(filtroVal.ValorDoses);

                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.InicializarValoraciones();
                }
                if (ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaCPs)
                {
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.InicializarCPs(filtroCPs.ColProbables);
                }
                if (ContenedorAnalisisColumnasGlobal.AnalisisGrupos.UsaFormatos)
                {
                    ContenedorAnalisisColumnasGlobal.AnalisisGrupos.InicializaFormatosSignos(ObtenerFormatosSignosAAnalizar(filtroFS.FormatosSignos));
                }
                ContenedorAnalisisColumnasGlobal.EsAnalisisExterno = false;
            }
            else
            {
                ContenedorAnalisisColumnasGlobal = new Analisis.ContenedorAnalisisGlobal(CtrlGrupos.GruposPartidos.Count, CtrlGrupos.ControlesGrupos.Count, numPartidos);
                ContenedorAnalisisColumnasGlobal.EsAnalisisExterno = true;
            }
        }
        protected void InicializaPronosticoBase()
        {
            pronosticoBase = UtilColumnas.ConvStrToLong(Pronosticos);
        }
        protected List<int> ObtenerFormatosSignosAAnalizar(List<FormatosSignos> gruposFormatos)
        {
            List<int> noFormatos = new List<int>();
            for (int i = 0; i < gruposFormatos.Count; i++)
            {
                FormatosSignos f = gruposFormatos[i];
                noFormatos.Add(f.LineasFormatos.Count);
            }
            return noFormatos;
        }

        protected void InicializaParametros()
        {
            AConfiguracion aConfig = new AConfiguracion(Application.StartupPath);

            //inicializa parametros de condiciones establecidas en parametros.free1x2

            for (int i = 0; i < ctrlGrupos.GruposPartidos.Count; i++)
            {
                Grupo grupo = ctrlGrupos.GruposPartidos[i];
                grupo.ActualizaParametrosFiltros(aConfig);
            }
        }

        public void SetPronostico(int noPartido, string strPronostico)
        {
            if (pronosticos == null)
            {
                //should get number of matches from congig file.
                pronosticos = new string[numPartidos];
            }

            pronosticos[noPartido] = strPronostico;
        }

        public void PararAnalisis()
        {

            if (gc != null)
            {
                gc.PararGeneracionCols();
            }
        }

        private void InicializaContadores()
        {
            noColsAnalizadas = 0;
            noColsAceptadas = 0;
        }

        private bool hayDatos()
        {
            if (ArchivoColumnasBase.Length > 0) return true;
            if (CtrlGrupos.ControlesGrupos.Count > 1) return true;
            if (GruposPartidos.Count > 1)
                return true;
            if (GruposPartidos[0].EsActivo) return true;
            if (IfThen != null)
            {
                if (!IfThen.EsVacio) return true;
            }
            for (int i = 0; i < pronosticos.Length; i++)
            {
                if (pronosticos[i] != "1,X,2") return true;
            }
            return false;
        }

        public int ColsAnalizadas
        {
            get { return noColsAnalizadas; }
        }

        public int ColsAceptadas
        {
            get { return noColsAceptadas; }
        }

        public GrupoPartidos GruposPartidos
        {
            get { return ctrlGrupos.GruposPartidos; }
            set
            {
                ctrlGrupos.GruposPartidos = value;
                //Poner referencia a controlador de grupos en grupos de partidos.
                ctrlGrupos.GruposPartidos.CtrlGrupos = ctrlGrupos;
            }
        }

        public ControladorGrupos CtrlGrupos
        {
            get { return ctrlGrupos; }
            set { ctrlGrupos = value; }
        }

        public string[] Pronosticos
        {
            get { return pronosticos; }
            set { pronosticos = value; }
        }

        public string ArchivoColumnasBase
        {
            get { return archColumnasBase; }
            set { archColumnasBase = value; }
        }

        public ControladorIfThen IfThen
        {
            get { return ifThen; }
            set { ifThen = value; }
        }
        public string CompletarCon
        {
            get { return completarCon; }
            set { completarCon = value; }
        }
        public bool HayDatos
        {
            get { return hayDatos(); }
        }
        public Analisis.ContenedorAnalisisGlobal ContenedorAnalisisColumnasGlobal
        {
            get { return contenedorAnalisisGlobal; }
            set { contenedorAnalisisGlobal = value; }
        }
    }
}
