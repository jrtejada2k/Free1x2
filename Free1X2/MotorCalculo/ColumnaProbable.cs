// created on 11/11/2003 at 20:14
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
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
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	public class ColumnaProbable
	{
        private byte[] pronosticos = new byte[VariablesGlobales.NumeroPartidos + 1];		
		
		//condiciones 
		private int noPartidos;
		
		private bool[] AC;
		private bool[] ACS;
		private bool[] FS;

		private string strPuntos = "";
		private bool[] puntos;
		
		//tolerancias globales
		private char[] ACTol;
		private char[] ACSTol;
		private char[] FSTol;
        		
		//para el calculo de cada columna
		private int noAC;
		private int noACS;
		private int noFS;
		private int noPuntos;

		private int noPuntosFijos = 6;
		private int noPuntosDobles = 3;
		private int noPuntosTriples = 1;

		//tolerancias locales
		private bool tolLocalActiva;
		private bool[] ACTolLocal;
		private bool[] ACSTolLocal;
		private bool[] FSTolLocal;

		private bool[] tolLocales = new bool[4];
		
		private int noFallosActual;
		private bool falloACBase;
		private bool falloACSBase;
		private bool falloFSBase;
		
		public ColumnaProbable()
		{
			//Este valor podria ser configurable
            noPartidos = VariablesGlobales.NumeroPartidos;
									
			AC =  new bool[ noPartidos+1 ];
			ACS = new bool[ noPartidos+1 ];
			FS =  new bool[ noPartidos+1 ];

			ACTol = new char[noPartidos+1];
			ACSTol = new char[noPartidos+1];
			FSTol = new char[noPartidos+1];

			ACTolLocal =  new bool[ noPartidos+1 ];
			ACSTolLocal = new bool[ noPartidos+1 ];
			FSTolLocal =  new bool[ noPartidos+1 ];
			
			for( int i = 0; i < noPartidos+1; i++ )
			{
				AC[i] = false;
				ACS[i] = false;
				FS[i] = false;

				ACTol[i] = ' ';
				ACSTol[i] = ' ';
				FSTol[i] = ' ';

				ACTolLocal[i] = false;
				ACSTolLocal[i] = false;
				FSTolLocal[i] = false;
			}

			tolLocales[0] = false;
			tolLocales[1] = false;
			tolLocales[2] = false;
			tolLocales[3] = false;
		}
		
		public void ReinicializaValores()
		{
			for( int i = 0; i < noPartidos+1; i++ )
			{
				AC[i] = false;
				ACS[i] = false;
				FS[i] = false;

				ACTol[i] = ' ';
				ACSTol[i] = ' ';
				FSTol[i] = ' ';

				ACTolLocal[i] = false;
				ACSTolLocal[i] = false;
				FSTolLocal[i] = false;
			}		
			
			tolLocalActiva = false;

			tolLocales[0] = false;
			tolLocales[1] = false;
			tolLocales[2] = false;
			tolLocales[3] = false;
		}
		
		private bool CumpleCondiciones()
		{
			
			bool cumplePuntos = true;
            bool cumpleCondicion = esColumnaValida();
	
			if(strPuntos != "")
			{
				if(puntos.Length < noPuntos + 1 || puntos[noPuntos] == false )
				{
					return false;
				}				
			}
			if( tolLocalActiva && cumplePuntos )
			{
				return SonToleranciasValidas();
			}
			return cumpleCondicion;
		}
		
		private string CumpleCondiciones(int numCol)
		{
		    string txt = esColumnaValida(numCol);
			if(strPuntos != "")
			{
				if(puntos.Length < noPuntos + 1 || puntos[noPuntos] == false )
					txt+="Fallo en Puntuación de la columna "+numCol+"  ("+noPuntos+")#";
			}
			return txt;
		}

		private bool esColumnaValida()
		{
			bool esValida = true;
			if( !AC[ noAC ] )
			{
				esValida = false;
				falloACBase = true;
			}
			
			if( !ACS[ noACS ] )
			{
				esValida = false;
				falloACSBase = true;
			}
			
			if( !FS[ noFS ] )
			{
				esValida = false;
				falloFSBase = true;
			}
			
			return esValida;
		}

		private string esColumnaValida(int numCol)
		{
			string txt="";
			if( !AC[ noAC ] )
			{
				txt+="Fallo en Aciertos en la columna "+numCol+"  ("+noAC+")#";
				falloACBase = true;
			}
			if( ACTol[ noAC ] !=' ' )
				txt+="!Tolerancias en Aciertos en la columna "+numCol+"  ("+noAC+")#";
			
			if( !ACS[ noACS ]  )
			{
				txt+="Fallo en Aciertos Seguidos en la columna "+numCol+"  ("+noACS+")#";
				falloACSBase = true;
			}
			if( ACSTol[ noACS ] !=' ' )
				txt+="!Tolerancias en Aciertos Seguidos en la columna "+numCol+"  ("+noACS+")#";
			
			if( !FS[ noFS ]  )
			{
				txt+="Fallo en Fallos Seguidos en la columna "+numCol+"  ("+noFS+")#";
				falloFSBase = true;
			}
			if( FSTol[ noFS ] !=' ' )
				txt+="!Tolerancias en Fallos Seguidos en la columna "+numCol+"  ("+noFS+")#";			
			
			return txt;
		}

        private bool SonToleranciasValidas()
        {
            noFallosActual = 0;

            //calcular noFallosActual total
            if (falloACBase)
            {
                if (ACTolLocal[noAC])
                {
                    noFallosActual++;
                }
                else
                {
                    return false;
                }
            }

            if (falloACSBase)
            {
                if (ACSTolLocal[noACS])
                {
                    noFallosActual++;
                }
                else
                {
                    return false;
                }
            }

            if (falloFSBase)
            {
                if (FSTolLocal[noFS])
                {
                    noFallosActual++;
                }
                else
                {
                    return false;
                }
            }

            return tolLocales[noFallosActual];
        }

	    private void InicializaContadores()
		{
			noAC = 0;
			noACS = 0;
			noFS = 0;		
			noPuntos = 0;
				
			falloACBase = false;
			falloACSBase = false;
			falloFSBase = false;
		}
		
		private void AnalizaColumna(long columna)
		{			
			int tempNoACS = 0;
			int tempNoFS = 0;

		    for (int nr2 = 0; nr2 < VariablesGlobales.NumeroPartidos; nr2++) 
			{
				if(pronosticos[nr2] != 0)
				{
                    if ((pronosticos[nr2] & (int)(columna & 7)) > 0) 
					{
						//pronostico acertado
						noAC++;
						
						tempNoACS++;
						tempNoFS = 0;	

						if( pronosticos[nr2] == 6 || //1x 
							pronosticos[nr2] == 5 || //12
							pronosticos[nr2] == 3 )  //x2
						{
							noPuntos +=  noPuntosDobles;						
						}
						else if(pronosticos[nr2] == 7) //1x2
						{
							noPuntos +=  noPuntosTriples;
						}
						else //signo fijo
						{
							noPuntos +=  noPuntosFijos;						
						}
					}
					else
					{
						//pronostico fallado
						tempNoFS++;					
						tempNoACS = 0;
					}

					//actualizar noACS y noFS si valores temporales son mayores
					if( noACS < tempNoACS )
					{
						noACS = tempNoACS;
					}
					
					if( noFS < tempNoFS )
					{
						noFS = tempNoFS;
					}
				}
                columna >>= 3;
			}				
		}
        public void BorraPronosticos()
        {
            Pronosticos = new string[VariablesGlobales.NumeroPartidos + 1]; 
        }
        public void TodosATriple()
        {
            string[] p = new string[VariablesGlobales.NumeroPartidos + 1];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = "1X2";
            }
            Pronosticos = p;
        }
        public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			int noAciertosTols = 0;
			
			//formato letrasTolerancias es: "ABC", "A", "CD", etc...
			
			foreach( char letra in letrasTolerancias)
			{
				if( ACTol[ noAC ] == letra)
				{					
					noAciertosTols++;		
				}
				if( ACSTol[ noACS ] == letra)
				{					
					noAciertosTols++;		
				}
				if( FSTol[ noFS ] == letra)
				{					
					noAciertosTols++;		
				}				
			}			
		
			return noAciertosTols;
		}
		
		public bool Analizar(long columna)
		{
			InicializaContadores();
			AnalizaColumna( columna );		
			return CumpleCondiciones();
		}		
		
		public string Analizar(long columna, int numCol)
		{
			InicializaContadores();
			AnalizaColumna( columna );		
			return CumpleCondiciones(numCol);
		}		
		
		public void SetNoAciertos(string valores)
		{
			string[] strValores = valores.Split(',');
						
			int valorNumerico;
			string letraTol;
			
			foreach(string str in strValores)
			{								
				if(Char.IsLetter(str[str.Length-1]))
				{
					//valor en tolerancias
					valorNumerico = Convert.ToInt32( str.Substring(0,str.Length-1) );
					letraTol = str.Substring(str.Length-1,1);
				}
				else
				{
					valorNumerico = Convert.ToInt32(str);
					letraTol = " ";
				}
				
				AC[valorNumerico] = true;
				ACTol[valorNumerico] = letraTol[0];
			}
		}			
		
		public void SetNoAciertosSeguidos(string valores)
		{
			string[] strValores = valores.Split(',');
						
			int valorNumerico;
			string letraTol;
			
			foreach(string str in strValores)
			{								
				if(Char.IsLetter(str[str.Length-1]))
				{
					//valor en tolerancias
					valorNumerico = Convert.ToInt32( str.Substring(0,str.Length-1) );
					letraTol = str.Substring(str.Length-1,1);
				}
				else
				{
					valorNumerico = Convert.ToInt32(str);
					letraTol = " ";
				}
				
				ACS[valorNumerico] = true;
				ACSTol[valorNumerico] = letraTol[0];
			}	
		}		
				
		public void SetNoFallosSeguidos(string valores)
		{
			string[] strValores = valores.Split(',');
						
			int valorNumerico;
			string letraTol;
			
			foreach(string str in strValores)
			{								
				if(Char.IsLetter(str[str.Length-1]))
				{
					//valor en tolerancias
					valorNumerico = Convert.ToInt32( str.Substring(0,str.Length-1) );
					letraTol = str.Substring(str.Length-1,1);
				}
				else
				{
					valorNumerico = Convert.ToInt32(str);
					letraTol = " ";
				}
				
				FS[valorNumerico] = true;
				FSTol[valorNumerico] = letraTol[0];
			}	
		}

		public void SetPuntos(string valorPuntos)
		{
			strPuntos = valorPuntos;
			
			if(strPuntos != "")
			{
				RangosHelper rangosHelper = new RangosHelper();
				puntos = rangosHelper.ObtenBoolArray(valorPuntos);		
			}
			else
			{
				puntos = null;	
			}
		}
		

		public void SetACTol(string valores)
		{
			tolLocalActiva = true;
			string[] strValores = valores.Split(',');
			
			foreach(string str in strValores)
			{
				ACTolLocal[Convert.ToInt32(str)] = true;
			}		
		}

		public void SetACSTol(string valores)
		{
			string[] strValores = valores.Split(',');
			tolLocalActiva = true;
			
			foreach(string str in strValores)
			{
				ACSTolLocal[Convert.ToInt32(str)] = true;
			}		
		}

		public void SetFSTol(string valores)
		{
			string[] strValores = valores.Split(',');
			tolLocalActiva = true;
			
			foreach(string str in strValores)
			{
				FSTolLocal[Convert.ToInt32(str)] = true;
			}		
		}

		public void SetTolerancias(string valores)
		{
			string[] strValores = valores.Split(',');
			
			foreach(string str in strValores)
			{
				tolLocales[Convert.ToInt32(str)] = true;
			}			
		}

		public string GetTolerancias()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in tolLocales )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}

		public string GetACTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in ACTolLocal )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}

		public string GetACSTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in ACSTolLocal )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}

		public string GetFSTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in FSTolLocal )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}

		public string GetAciertos()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in AC )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (ACTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetAciertosSeguidos()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in ACS )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (ACSTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetFallosSeguidos()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in FS )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (FSTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}

		public string GetPuntos()
		{
			return strPuntos;
		}
        public string PronosticosString
        {
            get
            {
                string valor = "";
                for (int z = 0; z < Pronosticos.Length; z++)
                {
                    
                     valor += Pronosticos[z];
                     valor += ",";
                    
                }
                return valor.Remove(valor.Length - 1);
            }
        }
		public string[] Pronosticos
		{
			get 
			{
                string[] tempPronos = new string[VariablesGlobales.NumeroPartidos];
				
				for(int i= 0; i < VariablesGlobales.NumeroPartidos; i++)
				{
					tempPronos[i] = "";
				}

                for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
				{
                    tempPronos[i] = UtilColumnas.ConvPartidoByteToStr(pronosticos[i]);
				}			
			
				return tempPronos; 
			}
			set 
			{ 				
				string[] tempPronos = value;
				
                for(int i = 0; i < tempPronos.Length; i++)
				{
                    pronosticos[i] = UtilColumnas.ConvPartidoStrToByte(tempPronos[i]);                    					
				}					
			
			}
		}

		public int NoAC
		{
			get{ return noAC; }
		}
        public int NoACS
        {
            get { return noACS; }
        }
        public int NoFS
        {
            get { return noFS; }
        }

		public bool ToleranciaLocalActiva
		{
			get{ return tolLocalActiva; }
			set{ tolLocalActiva = value; }
		}

		public void InicializaPuntosCP(int puntosFijos, int puntosDobles, int puntosTriples)
		{
			noPuntosFijos = puntosFijos;
			noPuntosDobles = puntosDobles;
			noPuntosTriples = puntosTriples;

		}
	}
}
