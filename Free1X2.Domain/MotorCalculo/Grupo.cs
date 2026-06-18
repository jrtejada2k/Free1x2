// Free1X2 · WinUI 3 — WIN3
// created on 18/08/2003 at 23:01
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
// Copyright (C) 2008 Morrison - morrison [dot] ne [at] gmail [dot] com
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
using Free1X2.Analisis;
using Free1X2.EntradaSalida;

namespace Free1X2.MotorCalculo
{
	public class Grupo
	{		
        private List<IFiltro> filtros;
        
		private ControladorTol ctrlTolerancias;

		private bool esGrupoBase;
		private string nombreGrupo="";
		public bool[] partidosActivos;
		private bool todosPartidosActivos;
		
        protected int numPartidos=VariablesGlobales.NumeroPartidos;
        protected long mascara = 7;

		protected bool reCalcularGrupo = true;
		protected bool grupoValidoMemoria = true;

        protected Dictionary<long, bool> bits;
        IArchivoColumnas aCol;

        protected string archivoFiltro = "";

		public Grupo()
		{
			InicializaPartidos();
			InicializaFiltros();
			ctrlTolerancias = new ControladorTol();
			configuraBoleto();
		}
		
		void configuraBoleto()
		{
			// Obtiene el no de partidos del boleto y los separadores
			string[] separador=null;
			AConfiguracion aConfig = new AConfiguracion( System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) );
			aConfig.ObtenNumPartidos(ref numPartidos, ref separador);
		}

		protected void InicializaPartidos()
		{
			partidosActivos = new bool[numPartidos];
		}
        public void CalcularMascara()
        {
            byte partido;
            long mascaraTemp = 0;
            for (int i = partidosActivos.Length - 1; i > -1; i--)
            {
                if (!partidosActivos[i])
                {
                    partido = 0;
                    mascaraTemp <<= 3;
                    mascaraTemp |= partido;
                }
                else
                {
                    partido = 7;
                    mascaraTemp <<= 3;
                    mascaraTemp |= partido;
                }
            }
            mascara = mascaraTemp;
        }
		protected void InicializaFiltros()
		{
            filtros = new List<IFiltro>();

		    IFiltro filtro = new FiltroNoVariantes();
			filtros.Add( filtro );
                       
			filtro = new FiltroValoracionSignos();
			filtros.Add( filtro );
			
			filtro = new FiltroSignosSeguidos();
			filtros.Add( filtro );
            
			filtro = new FiltroInterrupciones();
			filtros.Add( filtro );
			
			filtro = new FiltroDibujos();
			filtros.Add( filtro );
			
			filtro = new FiltroPesosNumericos();
			filtros.Add( filtro );
            
			filtro = new FiltroDistancias();
			filtros.Add( filtro );
            
			filtro = new FiltroGruposEquipos();
			filtros.Add( filtro );								
            
			filtro = new FiltroContactos();
			filtros.Add( filtro );		
	        
			filtro = new FiltroFormatosSignos();
			filtros.Add( filtro );
            
			filtro = new FiltroColProbables();
			filtros.Add( filtro );

            filtro = new FiltroFormatos123();
            filtros.Add(filtro);

            filtro = new FiltroSimetrias();
            filtros.Add(filtro);

            filtro = new FiltroDiferencias();
            filtros.Add(filtro);
            
			//anadir mas filtros aqui...		
		
		}

		public void ActualizaParametrosFiltros(AConfiguracion aConfig)
		{
			//este metodo actualiza los parametros de los filtros
			//especificados en el archivo de configuracion.
			InicializaPuntosCP( aConfig );		
		}

        protected void InicializaPuntosCP(AConfiguracion aConfig)
        {
            int puntosFijos = 0;
            int puntosDobles = 0;
            int puntosTriples = 0;

            aConfig.ObtenPuntosCP(ref puntosFijos, ref puntosDobles, ref puntosTriples);

            FiltroColProbables fCP = (FiltroColProbables) GetFiltro(Filtro.ColProbables.ToString());
            fCP.InicializaPuntosCP(puntosFijos, puntosDobles, puntosTriples);

        }

	    public long ColumnaGrupo(long columna )
		{
			  return columna & mascara;
		}

		public bool AnalizaColumna( long columna )
		{
		    bool columnaValida = true;
            long nuevaCol = columna;
            if (reCalcularGrupo)
            {
                if (!todosPartidosActivos)
                {
                    nuevaCol = ColumnaGrupo(columna);
                }

                for (int i = 0; i < filtros.Count; i++)
                {
                    IFiltro filtro = filtros[i];

                    if (filtro.IsActive)
                    {
                        if (!filtro.Analizar(nuevaCol))
                        {
                            //do not check any more filters
                            columnaValida = false;
                            break;
                        }
                    }
                }

                //comprueba tolerancias
                if (columnaValida)
                {
                    if (ctrlTolerancias.FallosPermitidos == "")
                    {
                        columnaValida = SonTolGrupoValidas();
                    }
                    else
                    {
                        columnaValida = SonTolGrupoValidasConFallos();
                    }

                    if (UsaFiltroParcial && columnaValida)
                    {
                        if (bits == null)
                        {
                            aCol = new ArchivoColumnasTexto(ArchivoFiltroGrupo);

                            InicializarColumnasFiltro(aCol.LeerTodasCols(false));
                        }
                        columnaValida = AnalizaFiltroColumnas(nuevaCol);
                    }
                }

                grupoValidoMemoria = columnaValida;
                reCalcularGrupo = false;
            }

            else
            {
                columnaValida = grupoValidoMemoria;
            }

			return columnaValida;		
		}
        public bool AnalizaColumna(long columna, ContenedorAnalisis contenedor)
        {
            bool columnaValida = true;

                if (reCalcularGrupo)
                {
                    long nuevaCol = ColumnaGrupo(columna);
                    if (nuevaCol == 0) nuevaCol = columna;

                    for (int i = 0; i < filtros.Count; i++)
                    {
                        IFiltro filtro = filtros[i];
                        if (filtro.AnalisisActivo)
                        {
                            if (filtro.IsActive)
                            {
                                columnaValida = filtro.Analizar(nuevaCol);

                                if (columnaValida == false)
                                {
                                    //do not check any more filters
                                    break;
                                }
                            }
                            else if (!filtro.IsActive)
                            {

                                filtro.Analizar(nuevaCol);
                            }
                            if (!contenedor.FiltrosTemp.Contains(filtro))
                            {
                                if (EsGrupoBase)
                                {
                                    contenedor.FiltrosTemp.Add(filtro);
                                }
                            }
                        }
                        else
                        {
                            if (filtro.IsActive)
                            {
                                columnaValida = filtro.Analizar(nuevaCol);

                                if (!columnaValida)
                                {
                                    //do not check any more filters
                                    break;
                                }
                            }
                        }
                    }

                    //comprueba tolerancias
                    if (columnaValida)
                    {
                        if (ctrlTolerancias.FallosPermitidos == "")
                        {
                            columnaValida = SonTolGrupoValidas();
                        }
                        else
                        {
                            columnaValida = SonTolGrupoValidasConFallos();
                        }
                    }

                    grupoValidoMemoria = columnaValida;
                    reCalcularGrupo = false;
                }
                else
                {
                    columnaValida = grupoValidoMemoria;
                }

            return columnaValida;
        }

		public bool AnalizaToleranciasGrupo( long columna )
		{
		    if (ctrlTolerancias.FallosPermitidos == "")
            {
                return SonTolGrupoValidas();
            }
		    return SonTolGrupoValidasConFallos();
		}

        protected bool AnalizaFiltroColumnas(long columna)
        {
            return bits.ContainsKey(columna);            
        }

        protected void InicializarColumnasFiltro(string[] cols)
        {
            bits = new Dictionary<long,bool>();
            for (int i = 0; i < cols.Length; i++)
            {
                long l = Utils.UtilColumnas.ConvStrToLong(cols[i], partidosActivos);

                if(!bits.ContainsKey(l))
                {
                    {
                        bits.Add(l, true);
                    }
                }
            }
        }
        protected bool SonTolGrupoValidasConFallos()
		{
            //poner contadores de tolerancias a 0
			foreach(ToleranciaFiltros tol in ctrlTolerancias.Tolerancias)
			{
				tol.ReinicializaNoAciertosAcumulados();					
			}	

			//contar numero de aciertos
			for(int i = 0; i < filtros.Count; i++)
			{
			    IFiltro filtro = filtros[i];

			    if(filtro.IsActive)
				{
					foreach(ToleranciaFiltros tol in ctrlTolerancias.Tolerancias)
					{
					    int noAciertosTemp = filtro.ObtenNoAciertosTolerancias( tol.LetrasTol );
					    tol.SumaAciertosAcumulados( noAciertosTemp );
					}
				}
			}

            int noControlesFallados = 0;

			//comprobar numero de aciertos validos
			foreach(ToleranciaFiltros tol in ctrlTolerancias.Tolerancias)
			{					
				if(!tol.CumpleTolerancia() )
				{
					noControlesFallados++;
				}					
			}		
			return  ctrlTolerancias.SonFallosPermitidosValidos( noControlesFallados );
		}

		protected bool SonTolGrupoValidas()
		{
		    bool columnaValida = true;
			
			//poner contadores de tolerancias a 0
			foreach(ToleranciaFiltros tol in ctrlTolerancias.Tolerancias)
			{
				tol.ReinicializaNoAciertosAcumulados();					
			}	

			//contar numero de aciertos
			for(int i = 0; i < filtros.Count; i++)
			{
			    IFiltro filtro = filtros[i];

			    if(filtro.IsActive)
				{
					foreach(ToleranciaFiltros tol in ctrlTolerancias.Tolerancias)
					{
					    int noAciertosTemp = filtro.ObtenNoAciertosTolerancias( tol.LetrasTol );
					    tol.SumaAciertosAcumulados( noAciertosTemp );
					}
				}
			}

		    //comprobar numero de aciertos validos
			foreach(ToleranciaFiltros tol in ctrlTolerancias.Tolerancias)
			{					
				if(!tol.CumpleTolerancia() )
				{
					columnaValida = false;
					break;
				}					
			}		
		
			return columnaValida;
		}

        protected long ObtenColumnaGrupo(long columna)
        {
             return columna & mascara;
        }
		
		public IFiltro GetFiltro( string nombre)
		{
			
			IFiltro filtro = null;
						
			for(int i = 0; i < filtros.Count; i++)
			{
				filtro = filtros[i];
				
				if( filtro.EsNombreFiltro( nombre ) )
				{
					break;				
				}			
			}
			
			return filtro;		
		}
		
		public string ObtenPartidosActivos()
		{
			string noPartidos = "";

		    for(int i=0; i < partidosActivos.Length ; i++)
			{
				if( partidosActivos[i] )
				{
					int partidoNumero = i + 1;
					
					if( noPartidos != "" )
					{
						noPartidos += ",";
					}
					
					noPartidos += partidoNumero.ToString();
				}
			}			
			
			return noPartidos;
		}
        public bool HayPartidosActivos()
        {
            bool hayPartidos = false;
            
            for (int i = 0; i < partidosActivos.Length; i++)
            {
                if (partidosActivos[i])
                {
                    hayPartidos = true;
                    break;
                }
            }

            return hayPartidos;
        }

		public void PonerPartidosActivos( string noPartidosActivos )
		{
			string[] arrayPartidosActivos = noPartidosActivos.Split(',');
            			
			//comprobar si todos los partidos estan activos
			if( arrayPartidosActivos.Length == numPartidos )
			{
				todosPartidosActivos = true;
			}
			else
			{
				todosPartidosActivos = false;
			}

		    foreach(string strPartido in arrayPartidosActivos)
		    {
		        int noPartido = Convert.ToInt32(strPartido);

		        if (noPartido <= partidosActivos.Length)
                {
                    partidosActivos[noPartido - 1] = true;
                }
		    }
		}

		public void ActivaFiltro(Grupo grupo, string nombreFiltro, bool valor)
		{
			int i;
			// Obtiene el grupo de referencia
			IFiltro filtro=null;
			for(i=0;i<grupo.Filtros.Count;i++)
			{
				filtro = grupo.Filtros[i];
				if(filtro.NombreFiltro.ToString()==nombreFiltro)
					break;
			}
		    for(i=0;i<Filtros.Count;i++)
		    {
		        IFiltro filtroTmp = Filtros[i];
		        if(filtroTmp.NombreFiltro==filtro.NombreFiltro)
					break;
		    }
		    filtros.RemoveAt(i);
			filtro.IsActive=valor;
			filtro.ContieneDatos=valor;
			filtros.Insert(i,filtro);
		}

		public void ActivaFiltro(IFiltro filtro)
		{
			int i;

		    for(i=0;i<Filtros.Count;i++)
		    {
		        IFiltro filtroTmp = Filtros[i];
		        if (filtroTmp.NombreFiltro == filtro.NombreFiltro)
                {
                    break;
                }
		    }
		    filtros.RemoveAt(i);
			filtros.Insert(i,filtro);
		}

		private bool esActivo()
		{
		    for(int i=0;i<filtros.Count;i++)
		    {
		        IFiltro filtroTmp = filtros[i];
		        if(filtroTmp.IsActive) return true;
		    }
		    return false;
		}

        public List<IFiltro> Filtros
		{
			get{ return filtros;}
		}
		
		public ControladorTol ControladorTolerancias
		{
			get { return ctrlTolerancias; }
			set { ctrlTolerancias = value; }
		}
		
		public bool[] Partidos
		{
			get{ return partidosActivos; }
			set
            {
                partidosActivos = value;
                CalcularMascara();
            }
		}

		public bool EsGrupoBase
		{
			get{ return esGrupoBase; }
			set
			{ 
				esGrupoBase = value;
				
				//si es grupo base todos los partidos estan activados
				if( esGrupoBase )
				{
					todosPartidosActivos = true;
				}
			}
		}
		
		public string NombreGrupo
		{
			get { return nombreGrupo; }
			set { nombreGrupo = value; }
		}

		public bool EsActivo
		{
			get { return esActivo(); }
		}

		public void RecalcularGrupo()
		{
			reCalcularGrupo = true;
		}
        public void ReinicializaVariablesFiltroParcial()
        {
            //Establecemos bits y aCol a null para recalcular
            //Necesario cuando cambia por ejemplo el número de partidos involucrados

            bits = null;
            aCol = null;
            
        }
        public string ArchivoFiltroGrupo
        {
            get { return archivoFiltro; }
            set { archivoFiltro = value; }
        }

        public bool UsaFiltroParcial
        {
            get
            {
                if (ArchivoFiltroGrupo == "")
                {
                    return false;
                }
                return true;
            }
        }
	}
}
