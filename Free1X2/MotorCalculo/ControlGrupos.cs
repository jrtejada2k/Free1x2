// created on 22/11/2003 at 15:16
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
// 
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
using Free1X2.Analisis;

namespace Free1X2.MotorCalculo
{
	public class ControlGrupos
	{
		private bool usaControlGrupos;
		private bool[] fallosPermitidos;
		private int[] gruposControlados;
		
		private string strFallosPermitidos = "";
		private string strGruposControlados = "";
				
		private int noFallosEnAnalisis;
		private int noGruposControladosTemp;
		
		//ctrlGrupos es el objeto que contiene todos los controles de grupo
		private ControladorGrupos ctrlGrupos;

		protected bool reCalcularControlGrupo = true;
		protected bool controlGrupoValidoMemoria = true;

	    public bool AnalizaColumna(long columna)
        {
            if (usaControlGrupos)
            {
                return AnalizaGruposConControlFallos(columna);
            }
            return AnalizaGruposSinControlFallos(columna);
        }	
		
        public bool AnalizaColumna(long columna, ContenedorAnalisisGlobal contenedor)
        {
            if (usaControlGrupos)
            {
                return AnalizaGruposConControlFallos(columna, contenedor);
            }
            return AnalizaGruposSinControlFallos(columna, contenedor);
        }

		protected bool AnalizaGruposConControlFallos( long columna )
		{
			bool columnaValida;

			if(reCalcularControlGrupo)
			{			
				//bool esGrupoValido = true;
			
				ReinicializaControl();
			
				for(int i = 0; i < gruposControlados.Length; i++)
				{
					Grupo grupo = ctrlGrupos.GruposPartidos[ gruposControlados[i] ];
				
					ControlaFallo( grupo.AnalizaColumna( columna ) );				
				}
			
				columnaValida = fallosPermitidos[noFallosEnAnalisis];
				

				controlGrupoValidoMemoria = columnaValida;
				reCalcularControlGrupo = false;						
			}
			else
			{
				columnaValida = controlGrupoValidoMemoria;
			}
		
			return columnaValida;
		}
		
		protected bool AnalizaGruposSinControlFallos( long columna )
		{
									
			for(int i = 0; i < gruposControlados.Length; i++)
			{
			    if (!ctrlGrupos.GruposPartidos[gruposControlados[i]].AnalizaColumna(columna))
                {
                    return false;
                }
			}		
			return true;
		}

        protected bool AnalizaGruposConControlFallos(long columna, ContenedorAnalisisGlobal contenedor)
        {
            bool columnaValida = true;

            if (reCalcularControlGrupo)
            {
                ReinicializaControl();
                Grupo BBase = ctrlGrupos.GruposPartidos[0];
                BBase.EsGrupoBase = true;

                for (int i = 0; i < gruposControlados.Length; i++)
                {
                    Grupo grupo = ctrlGrupos.GruposPartidos[gruposControlados[i]];
                   // grupo.CalcularMascara();
                    bool esGrupoValido = grupo.AnalizaColumna(columna);

                    ControlaFallo(esGrupoValido);
                }

                if (fallosPermitidos[noFallosEnAnalisis] == false)
                {
                    columnaValida = false;
                    contenedor.VaciarInformacion();
                }
                else
                {
                    contenedor.ColumnasPorFallosDeGrupos[noFallosEnAnalisis]++;
                }

                controlGrupoValidoMemoria = columnaValida;
                reCalcularControlGrupo = false;
            }
            else
            {
                columnaValida = controlGrupoValidoMemoria;
            }

            return columnaValida;
        }

        protected bool AnalizaGruposSinControlFallos(long columna, ContenedorAnalisisGlobal contenedor)
        {
            Grupo BBase = ctrlGrupos.GruposPartidos[0];
            BBase.EsGrupoBase = true;


            for (int i = 0; i < gruposControlados.Length; i++)
            {
                Grupo grupo = ctrlGrupos.GruposPartidos[gruposControlados[i]];

                if(!grupo.AnalizaColumna(columna, contenedor.AnalisisGrupos))
                {
                    return false;
                }
            }
            return true;
        }


		protected void ControlaFallo( bool esGrupoValido )
		{
			if( !esGrupoValido )
			{
				noFallosEnAnalisis++;			
			}
			
			noGruposControladosTemp++;
		}
				
		public string ObtenGruposControlados()
		{
			return strGruposControlados;
		}
		
		public void PonerGruposControlados(string gruposControl)
		{
			strGruposControlados = gruposControl;
            string gruposParseados = Utils.UtilidadesEntradasValores.ObtenerValoresSeparadosPorComas(gruposControl);
            string[] arrayGrupos = gruposParseados.Split(',');
            gruposControlados = new int[arrayGrupos.Length];
            try
            {             
                for (int i = 0; i < arrayGrupos.Length; i++)
                {
                   gruposControlados[i] = Convert.ToInt32(arrayGrupos[i]);	
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Hay errores en los valores introducidos", "Error");
                return;
            }		
		}
		
		public void PonerFallosPermitidos(string fallosgrupos)
		{
			strFallosPermitidos = fallosgrupos;

            //Parsear la entrada para recibir un string separado por comas
            if (fallosgrupos != "")
            {
                string fallosParseados = Utils.UtilidadesEntradasValores.ObtenerValoresSeparadosPorComas(fallosgrupos);
                string[] arrayFallos = fallosParseados.Split(',');
                //inicializar array con fallos totales posibles
                //el numero maximo corresponde al numero de grupos totales que existen
                fallosPermitidos = new bool[gruposControlados.Length + 1];

                //tenemos los valores separados por comas. Inicializar array
                try
                {
                    for (int i = 0; i < arrayFallos.Length; i++)
                    {
                        fallosPermitidos[Convert.ToInt32(arrayFallos[i])] = true;
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Hay errores en los valores introducidos", "Error");
                    return;
                }
                usaControlGrupos = true;
            }
            else
            {
                usaControlGrupos = false;
            }			
		
		}
		
		public bool ContieneGrupo(int noGrupo)
		{
			bool contieneGrupo = false;
			
			
			for(int i = 0; i < gruposControlados.Length; i++)
			{
				if( gruposControlados[i] == noGrupo)
				{
					contieneGrupo = true;
					break;
				}			
			}
		
			return contieneGrupo;
		}
		
		public string ObtenFallosPermitidos()
		{
			return strFallosPermitidos;
		}
		
		protected void ReinicializaControl()
		{
			noFallosEnAnalisis = 0;
			noGruposControladosTemp = 0;
		}
		
		public void PonerGrupo(int noGrupo)
		{
			
			if( noGrupo == 0)
			{
				strGruposControlados = "0";			
			}
			else 
			{
				if( strGruposControlados != "")
				{
					strGruposControlados += ",";
				}
				
				strGruposControlados += noGrupo.ToString();
			}	
			
			PonerGruposControlados( strGruposControlados );
		
		}

		public void RecalcularControlGrupos()
		{
			reCalcularControlGrupo = true;

		    for(int i = 0; i < gruposControlados.Length; i++)
		    {
                ctrlGrupos.GruposPartidos[gruposControlados[i]].RecalcularGrupo();
		    }
		}
		
		public ControladorGrupos CtrlGrupos
		{
			get{ return ctrlGrupos; }
			set{ ctrlGrupos = value; }		
		}
				
		public int[] GruposControlados
		{
			get{ return gruposControlados; }
			set{ gruposControlados = value; }	
		}
		
		public bool UsaControlGrupos
		{
			get { return usaControlGrupos; }
			set { usaControlGrupos = value; }
		}
		
		public bool[] FallosPermitidos
		{
			get{ return fallosPermitidos; }
			set{ fallosPermitidos = value; }		
		}
		
		public int NoGruposFallados
		{
			get{ return noFallosEnAnalisis; }
			set{ noFallosEnAnalisis = value; }	
		}	
	}
}
