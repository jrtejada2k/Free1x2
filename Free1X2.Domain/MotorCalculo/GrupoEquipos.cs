// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
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

using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for GrupoEquipos.
	/// </summary>
	public class GrupoEquipos
	{
		private char[] pronosticos = new char[VariablesGlobales.NumeroPartidos];
        private long longPronosticos;
        private long longPronosticosTemp;

		int noSumaPuntos;
		int noVictorias;
		int noEmpates;
		int noDerrotas;

		private string strSumaPuntos = "";
		private string strVictorias = "";
		private string strEmpates = "";
		private string strDerrotas = "";

		private bool[] sumaPuntos;
		private bool[] victorias;
		private bool[] empates;
		private bool[] derrotas;

	    public bool Analizar(long columna)
		{			
			InicializaContadores();
			AnalizaColumna( columna );
			return CumpleCondiciones();
		}
        public void CalcularLongPronosticos()
        {
            longPronosticosTemp = LongPronosticos;
        }

	    public string Analizar(long columna, int numGrupo)
		{			
			InicializaContadores();
			AnalizaColumna( columna );
			return esColumnaValida(numGrupo);
		}

		private void InicializaContadores()
		{
			noSumaPuntos = 0;
			noVictorias = 0;
			noEmpates = 0;
			noDerrotas = 0;
		}



        private void AnalizaColumna(long columna)
        {
            //Hacemos una copia de longPronosticos para protegerla de escritura
            long longPronosticos2 = longPronosticos;

            while (columna != 0)
            {
                byte cPronostico = (byte)(longPronosticos2 & 7);

                //1 = eq. casa
                //2 = eq. fuera
                //3 = eqs casa y fuera
                //7 = ninguno
                if (cPronostico != 7)
                {
                    switch (columna & 7)
                    {
                        case 4: //'1' Es un signo 1
                            {
                                if (cPronostico == 1)//'2' Gana el local, seleccionado visitante
                                {
                                    noDerrotas++;
                                }
                                else if (cPronostico == 2)//'3' Gana el local, seleccionado el local
                                {
                                    noSumaPuntos += 3;
                                    noVictorias++;
                                }
                                else if (cPronostico == 3)//gana el local, seleccionados los Dos
                                {
                                    noSumaPuntos += 3;
                                    noVictorias++;
                                    noDerrotas++;
                                }
                                break;
                            }
                        case 1://'2' Es un signo 2
                            {
                                if (cPronostico == 1)//'2' Gana el visitante, seleccionado visitante
                                {
                                    noVictorias++;
                                    noSumaPuntos += 3;
                                }
                                else if (cPronostico == 2)//'3' Gana el visitante, seleccionado local
                                {
                                    noDerrotas++;
                                }
                                else if (cPronostico == 3)//gana el visitante, seleccionados los Dos
                                {
                                    noSumaPuntos += 3;
                                    noVictorias++;
                                    noDerrotas++;
                                }
                                break;
                            }
                        case 2://'X' Es un signo X
                            {
                               if (cPronostico == 1 || cPronostico == 2)
                                {
                                    noSumaPuntos++;
                                    noEmpates++;
                                }
                                else if (cPronostico == 3)//'3' Empate, seleccionados los Dos
                                {
                                    noSumaPuntos += 2;
                                    noEmpates += 2;
                                }
                                break;
                            }
                    }
                }
                columna >>= 3;
                longPronosticos2 >>= 3;
            }
        }
		private bool CumpleCondiciones()
		{
			bool cumpleCondicion = false;
			if( esColumnaValida() )
			{
				cumpleCondicion = true;
			}
			return cumpleCondicion;
		}

		private bool esColumnaValida()
		{
			bool esValida = true;
			if(strSumaPuntos != "")
			{
				if(sumaPuntos.Length < noSumaPuntos+1 || sumaPuntos[ noSumaPuntos ] == false )
				{
					esValida = false;
				}			
			}
			if( esValida && strVictorias != "")
			{
				if(victorias.Length < noVictorias+1 || victorias[ noVictorias ] == false )
				{
					esValida = false;
				}				
			}
			if( esValida && strEmpates != "")
			{
				if(empates.Length < noEmpates+1 || empates[ noEmpates ] == false )
				{
					esValida = false;
				}			
			}
			if( esValida && strDerrotas != "")
			{
				if( derrotas.Length < noDerrotas+1 || derrotas[ noDerrotas ] == false )
				{
					esValida = false;
				}				
			}			
			return esValida;
		}		
				
		private string esColumnaValida(int numGrupo)
		{
			string txt="";
			if(strSumaPuntos != "")
			{
				if(sumaPuntos.Length < noSumaPuntos+1 || sumaPuntos[ noSumaPuntos ] == false )
					txt+="Fallo en Suma de Puntos del grupo "+numGrupo+".  ("+noSumaPuntos+")#";
			}
			if( strVictorias != "")
			{
				if(victorias.Length < noVictorias+1 || victorias[ noVictorias ] == false )
					txt+="Fallo en Nº de Victorias del grupo "+numGrupo+".  ("+noVictorias+")#";
			}
			if( strEmpates != "")
			{
				if(empates.Length < noEmpates+1 || empates[ noEmpates ] == false )
					txt+="Fallo en Nº de Empates del grupo "+numGrupo+".  ("+noEmpates+")#";
			}
			if( strDerrotas != "")
			{
				if( derrotas.Length < noDerrotas+1 || derrotas[ noDerrotas ] == false )
					txt+="Fallo en Nº de Derrotas del grupo "+numGrupo+".  ("+noDerrotas+")#";
			}
			return txt;
		}		
				
		public string SumaPuntos
		{
			get{ return strSumaPuntos;}
			set
			{ 
				strSumaPuntos = value;
				
				if(strSumaPuntos != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					sumaPuntos = rangosHelper.ObtenBoolArray(strSumaPuntos);
				}
				else
				{
					sumaPuntos = null;
				}			
			}			
		}

		public string Victorias
		{
			get{ return strVictorias;}
			set
			{ 
				strVictorias = value;
				
				if(strVictorias != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					victorias = rangosHelper.ObtenBoolArray(strVictorias);
				}
				else
				{
					victorias = null;
				}			
			}			
		}

		public string Empates
		{
			get{ return strEmpates;}
			set
			{ 
				strEmpates = value;
				
				if(strEmpates != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					empates = rangosHelper.ObtenBoolArray(strEmpates);
				}
				else
				{
					empates = null;
				}			
			}			
		}

		public string Derrotas
		{
			get{ return strDerrotas;}
			set
			{ 
				strDerrotas = value;
				
				if(strDerrotas != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					derrotas = rangosHelper.ObtenBoolArray(strDerrotas);
				}
				else
				{
					derrotas = null;
				}			
			}			
		}
		public char[] Pronosticos
		{
			get{ return pronosticos; }
			set
            { 
                pronosticos = value;
            }	
		}
        public long LongPronosticos
        {
            get
            {
                longPronosticos = 0;
                //001 = 1 visitante
                //010 = 2 local
                //011 = 3 ambos
                //111 = 7 ninguno
                for (int i = pronosticos.Length - 1; i > -1; i--)
                {
                    longPronosticos <<= 3;
                    longPronosticos |= (uint)"*213***0".IndexOf(pronosticos[i]);
                }
                
                return longPronosticos;
            }
            set
            {
                longPronosticos = value;

            }
        }

            public int NoSumaPuntos
		{
			get{ return noSumaPuntos;}
		}

		public int NoVictorias
		{
			get{ return noVictorias;}
		}

		public int NoEmpates
		{
			get{ return noEmpates;}
		}

		public int NoDerrotas
		{
			get{ return noDerrotas;}
		}
	}
}

