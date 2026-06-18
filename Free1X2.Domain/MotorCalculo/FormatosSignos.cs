// Free1X2 · WinUI 3 — WIN3
// Copyright (C) 2005 Luis Fernandez - luifer [at] onetel [dot] com
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
using System.Collections.Generic;
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for FormatosSignos.
	/// </summary>
	public class FormatosSignos
	{
		private List<FormatoSignos> lineasFormatos = new List<FormatoSignos>();
		private string strLineas = "";
		private string strGlobal = "";
        protected int noLineasValidas;
        protected int noAparicionesGlobal;
		private bool[] lineas;
		private bool[] global;


	    public bool Analizar(long columna)
		{			
			bool columnaValida = true;
	        noLineasValidas = 0;
			noAparicionesGlobal = 0;

			for(int i=0; i< lineasFormatos.Count; i++)
			{
				FormatoSignos formato = lineasFormatos[i];
				columnaValida = formato.Analiza( columna );
				noAparicionesGlobal +=  formato.NoApariciones;
				if(!columnaValida)
				{
					if(strLineas == "" && strGlobal == "")
					{
						break;
					}
				}
				else
				{
					noLineasValidas++;
				}
			}			
		
			if(strLineas != "" || strGlobal != "")
			{
				bool esValida = true;
				if(strLineas != "")
				{
                    if (!(lineas.Length > noLineasValidas && lineas[noLineasValidas]))
                    {
                        esValida = false;
                    }
				}

				if(strGlobal != "" && esValida)
				{
                    if (!(global.Length > noAparicionesGlobal &&
                          global[noAparicionesGlobal]))
                    {

                        esValida = false;
                    }
				}
				columnaValida = esValida;
			}			
			return columnaValida;					
		}		

		public string Analizar(long columna, int numGrupo)
		{
			string texto="";
		    noLineasValidas = 0;
			noAparicionesGlobal = 0;

			for(int i=0; i< lineasFormatos.Count; i++)
			{
				FormatoSignos formato = lineasFormatos[i];
				bool columnaValida = formato.Analiza( columna);
				if(columnaValida==false)
					texto+="Fallo en Linea nº "+Convert.ToString((i+1))+" ("+ formato.Formato + ")" + " en grupo "+numGrupo+"  ("+formato.NoApariciones+")#";
				noAparicionesGlobal +=  formato.NoApariciones;
				if(!columnaValida)
				{
					if(strLineas == "" && strGlobal == "")
					{
						break;
					}
				}
				else
				{
					noLineasValidas++;
				}
			}
		    if(strLineas != "" || strGlobal != "")
			{
			    if(strLineas != "")
				{
					if(!(lineas.Length > noLineasValidas && lineas[noLineasValidas]))
						texto+="Fallo en Líneas en grupo "+numGrupo+"  ("+noLineasValidas+")#";
				}

				if(strGlobal != "")
				{
					if(	!(global.Length > noAparicionesGlobal && global[noAparicionesGlobal]))
						texto+="Fallo en Global en grupo "+numGrupo+"  ("+noAparicionesGlobal+")#";
				}
			}			
			return texto;					
		}		

		public List<FormatoSignos> LineasFormatos
		{
			get{ return lineasFormatos;}
			set{ lineasFormatos = value;}
		}

		public string Lineas
		{
			get{ return strLineas;}
			set
			{ 
				strLineas = value;

				if(strLineas != "")
				{
					RangosHelper rangos = new RangosHelper();
					lineas = rangos.ObtenBoolArray(strLineas);
				}
			}
		}

		public string Global
		{
			get{ return strGlobal;}
			set
			{ 
				strGlobal = value;
				
				if(strGlobal != "")
				{
					RangosHelper rangos = new RangosHelper();
					global = rangos.ObtenBoolArray(strGlobal);
				}
			
			}
		}
        public int NoLineasValidas
        {
            get { return noLineasValidas; }
            set { noLineasValidas = value; }
        }
        public int NoAparicionesGlobal
        {
            get { return noAparicionesGlobal; }
            set { noAparicionesGlobal = value; }
        }
	}
}

