// created on 22/06/2004 at 17:44
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 XFSF
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

namespace Free1X2.MotorCalculo
{
	public class FiltroInterrupciones: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        List<long> figuras;

		//no. interrupciones
		private int noIntGlobal;
		private int noIntVar;
		private int noInt1;
		private int noIntX;
		private int noInt2;

		//no interrupciones seguidas
		private int noIntGlobalSeg;
		private int noIntVarSeg;
		private int noInt1Seg;
		private int noIntXSeg;
		private int noInt2Seg;
		
		//condiciones 
		private int noPartidos;
		//standard
		private bool[] intGlobal;
		private bool[] intVar;
		private bool[] int1;
		private bool[] intX;
		private bool[] int2;

		private bool[] intGlobalSeg;
		private bool[] intVarSeg;
		private bool[] int1Seg;
		private bool[] intXSeg;
		private bool[] int2Seg;
				   
		//tolerancias
		private char[] intGlobalTol;
		private char[] intVarTol;
		private char[] int1Tol;
		private char[] intXTol;
		private char[] int2Tol;	
	
		private char[] intGlobalSegTol;
		private char[] intVarSegTol;
		private char[] int1SegTol;
		private char[] intXSegTol;
		private char[] int2SegTol;	
		
		public FiltroInterrupciones()
		{
			//Este valor podria ser configurable
            noPartidos = VariablesGlobales.NumeroPartidos;

			intGlobal = new bool[noPartidos+1];
			intVar = new bool[noPartidos+1];
			int1 = new bool[noPartidos+1];
			intX = new bool[noPartidos+1];
			int2 = new bool[noPartidos+1];

			intGlobalSeg = new bool[noPartidos+1];
			intVarSeg = new bool[noPartidos+1];
			int1Seg = new bool[noPartidos+1];
			intXSeg = new bool[noPartidos+1];
			int2Seg = new bool[noPartidos+1];

			intGlobalTol = new char[noPartidos+1];
			intVarTol = new char[noPartidos+1];
			int1Tol = new char[noPartidos+1];
			intXTol = new char[noPartidos+1];
			int2Tol = new char[noPartidos+1];

			intGlobalSegTol = new char[noPartidos+1];
			intVarSegTol = new char[noPartidos+1];
			int1SegTol = new char[noPartidos+1];
			intXSegTol = new char[noPartidos+1];
			int2SegTol = new char[noPartidos+1];
			
			for( int i = 0; i < noPartidos+1; i++ )
			{
				intGlobal[i] = false;	
				intVar[i] = false;	
				int1[i] = false;	
				intX[i] = false;	
				int2[i] = false;	

				intGlobalSeg[i] = false;	
				intVarSeg[i] = false;	
				int1Seg[i] = false;	
				intXSeg[i] = false;	
				int2Seg[i] = false;
		
				intGlobalTol[i] = ' ';
				intVarTol[i] = ' ';
				int1Tol[i] = ' ';
				intXTol[i] = ' ';
				int2Tol[i] = ' ';

				intGlobalSegTol[i] = ' ';
				intVarSegTol[i] = ' ';
				int1SegTol[i] = ' ';
				intXSegTol[i] = ' ';
				int2SegTol[i] = ' ';
			}
			
		}		
		
		private void InicializaContadores()
		{
			noIntGlobal = 0;
			noIntVar = 0;
			noInt1 = 0;
			noIntX = 0;
			noInt2 = 0;

			noIntGlobalSeg = 0;
			noIntVarSeg = 0;
			noInt1Seg = 0;
			noIntXSeg = 0;
			noInt2Seg = 0;
		}
		
		public void AnalizaColumna(string columna)
		{
			InicializaContadores();

			string columnaTemp = columna.Replace("*","");
						
			int nvs, nus, nxs, nds;
		    int ngs = nvs = nus = nxs = nds = 0;
			char ant = '0'; 

			for (int i=0; i < columnaTemp.Length; i++) 
			{
				char act = columnaTemp[i];
				if (act==ant) 
				{
					if (ngs>noIntGlobalSeg) noIntGlobalSeg=ngs;
					if (nus>noInt1Seg) noInt1Seg=nus;
					if (nds>noInt2Seg) noInt2Seg=nds;
					if (nxs>noIntXSeg) noIntXSeg=nxs;
					if (nvs>noIntVarSeg) noIntVarSeg=nvs;
					ngs = nus = nds = nxs = nvs = 0;
				}
				else 
				{
					if (i > 0 ) 
					{
						noIntGlobal++; ngs++;
						if (ant=='1') 
						{
							if (act=='2') 
							{ 
								noInt1++; nus++;
								if (nxs>noIntXSeg) noIntXSeg=nxs;
								nxs = 0;
							}
							else 
							{ 
								noInt1++; nus++; 
								if (nds>noInt2Seg) noInt2Seg=nds;
								nds = 0;
							}
						}
						else if (ant=='2') 
						{
							if (act=='1') 
							{
								noInt2++; noIntVar++; nds++; nvs++;
								if (nxs>noIntXSeg) noIntXSeg=nxs;
								nxs = 0;
							}
							else 
							{
								noInt2++; nds++;
								if (nus>noInt1Seg) noInt1Seg=nus; 
								if (nvs>noIntVarSeg) noIntVarSeg=nvs; 
								nus = nvs=0;
							}
						}
						else 
						{
							if (act=='1') 
							{
								noIntX++; noIntVar++; nxs++; nvs++;
								if (nds>noInt2Seg) noInt2Seg=nds;
								nds = 0;
							}
							else 
							{
								noIntX++; nxs++;
								if (nus>noInt1Seg) noInt1Seg=nus; 
								if (nvs>noIntVarSeg) noIntVarSeg=nvs; 
								nus = nvs=0;
							}
						}
					}
				}
				ant = act;
				
			}
			if (ngs>noIntGlobalSeg) noIntGlobalSeg=ngs;
			if (nus>noInt1Seg) noInt1Seg=nus;
			if (nds>noInt2Seg) noInt2Seg=nds;
			if (nxs>noIntXSeg) noIntXSeg=nxs;
			if (nvs>noIntVarSeg) noIntVarSeg=nvs;					
		}

        public void AnalizaColumna(long columna)
        {
            InicializaContadores();

            byte nvs, nus, nxs, nds;
            byte ant=0;
            byte ngs = nvs = nus = nxs = nds = 0;            

            //seguimos con el resto de signos
            while (columna != 0)
            {
                while (ant == 0)
                {
                    ant = (byte)(columna & 7);
                    columna >>= 3;
                }
                byte signo1 = (byte)(columna & 7);
                if (signo1 == 0)
                {
                    columna >>= 3;
                    continue; //omitimos los asteriscos
                }
                byte act = signo1;
                if (act == ant)
                {
                    if (ngs > noIntGlobalSeg) noIntGlobalSeg = ngs;
                    if (nus > noInt1Seg) noInt1Seg = nus;
                    if (nds > noInt2Seg) noInt2Seg = nds;
                    if (nxs > noIntXSeg) noIntXSeg = nxs;
                    if (nvs > noIntVarSeg) noIntVarSeg = nvs;
                    ngs = nus = nds = nxs = nvs = 0;
                }
                else
                {
                    noIntGlobal++; ngs++;
                    switch (ant)
                    {
                        case 4:
                            switch (act)
                            {
                                case 1:
                                    noInt1++;
                                    nus++;
                                    if (nxs > noIntXSeg) noIntXSeg = nxs;
                                    nxs = 0;
                                    break;
                                default:
                                    noInt1++;
                                    nus++;
                                    if (nds > noInt2Seg) noInt2Seg = nds;
                                    nds = 0;
                                    break;
                            }
                            break;
                        case 1:
                            switch (act)
                            {
                                case 4:
                                    noInt2++;
                                    noIntVar++;
                                    nds++;
                                    nvs++;
                                    if (nxs > noIntXSeg) noIntXSeg = nxs;
                                    nxs = 0;
                                    break;
                                default:
                                    noInt2++;
                                    nds++;
                                    if (nus > noInt1Seg) noInt1Seg = nus;
                                    if (nvs > noIntVarSeg) noIntVarSeg = nvs;
                                    nus = nvs = 0;
                                    break;
                            }
                            break;
                        default:
                            switch (act)
                            {
                                case 4:
                                    noIntX++;
                                    noIntVar++;
                                    nxs++;
                                    nvs++;
                                    if (nds > noInt2Seg) noInt2Seg = nds;
                                    nds = 0;
                                    break;
                                default:
                                    noIntX++;
                                    nxs++;
                                    if (nus > noInt1Seg) noInt1Seg = nus;
                                    if (nvs > noIntVarSeg) noIntVarSeg = nvs;
                                    nus = nvs = 0;
                                    break;
                            }
                            break;
                    }
                }
                ant = act;
                columna >>= 3;
            }
            if (ngs > noIntGlobalSeg) noIntGlobalSeg = ngs;
            if (nus > noInt1Seg) noInt1Seg = nus;
            if (nds > noInt2Seg) noInt2Seg = nds;
            if (nxs > noIntXSeg) noIntXSeg = nxs;
            if (nvs > noIntVarSeg) noIntVarSeg = nvs;
        }
        private bool EsColumnaValida()
		{
            if( intGlobal[ noIntGlobal ] == false ||
				intVar[noIntVar] == false ||	
				int1[noInt1] == false ||	
				intX[noIntX] == false ||	
				int2[noInt2] == false ||

				intGlobalSeg[ noIntGlobalSeg ] == false ||
				intVarSeg[noIntVarSeg] == false ||	
				int1Seg[noInt1Seg] == false ||	
				intXSeg[noIntXSeg] == false ||	
				int2Seg[noInt2Seg] == false )
			{
				return false;
			}
			return  true;
		}	
				
		private void EsColumnaValida(ref string[] arrayFallos)
		{
			string texto="";

			if( intGlobal[ noIntGlobal ] == false)
				texto+="Fallo en número de interrupciones  ("+noIntGlobal+")#";
			if( intGlobalTol[ noIntGlobal ]  != ' ')
				texto+="!Tolerancia en número de interrupciones  ("+noIntGlobal+")#";
			if( intVar[noIntVar] == false)
				texto+="Fallo en interrupciones de variantes  ("+noIntVar+")#";
			if( intVarTol[noIntVar]  != ' ')
				texto+="!Tolerancia en interrupciones de variantes  ("+noIntVar+")#";
			if( int1[noInt1] == false)
				texto+="Fallo en interrupciones de unos  ("+noInt1+")#";
			if( int1Tol[noInt1]  != ' ')
				texto+="!Tolerancia en interrupciones de unos  ("+noInt1+")#";
			if( intX[noIntX] == false)
				texto+="Fallo en interrupciones de equis  ("+noIntX+")#";
			if( intXTol[noIntX]  != ' ')
				texto+="!Tolerancia en interrupciones de equis  ("+noIntX+")#";
			if( int2[noInt2] == false)
				texto+="Fallo en interrupciones de doses  ("+noInt2+")#";
			if( int2Tol[noInt2]  != ' ')
				texto+="!Tolerancia en interrupciones de doses  ("+noInt2+")#";
			if( intGlobalSeg[ noIntGlobalSeg ] == false)
				texto+="Fallo en número de interrupciones seguidas  ("+noIntGlobalSeg+")#";
			if( intGlobalSegTol[ noIntGlobalSeg ]  != ' ')
				texto+="!Tolerancia en número de interrupciones seguidas  ("+noIntGlobalSeg+")#";
			if( intVarSeg[noIntVarSeg] == false)
				texto+="Fallo en interrupciones de variantes seguidas  ("+noIntVarSeg+")#";
			if( intVarSegTol[noIntVarSeg]  != ' ')
				texto+="!Tolerancia en interrupciones de variantes seguidas  ("+noIntVarSeg+")#";
			if( int1Seg[noInt1Seg] == false)
				texto+="Fallo en interrupciones de unos seguidas  ("+noInt1Seg+")#";
			if( int1SegTol[noInt1Seg]  != ' ')
				texto+="!Tolerancia en interrupciones de unos seguidas  ("+noInt1Seg+")#";
			if( intXSeg[noIntXSeg] == false)
				texto+="Fallo en interrupciones de equis seguidas  ("+noIntXSeg+")#";
			if( intXSegTol[noIntXSeg]  != ' ')
				texto+="!Tolerancia en interrupciones de equis seguidas  ("+noIntXSeg+")#";
			if( int2Seg[noInt2Seg] == false )
				texto+="Fallo en interrupciones de doses seguidas  ("+noInt2Seg+")#";
			if( int2SegTol[noInt2Seg]  != ' ' )
				texto+="!Tolerancia en interrupciones de doses seguidas  ("+noInt2Seg+")#";

			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
		}	
				
		private bool CumpleCondiciones()
		{
		    return EsColumnaValida();
		}
		
		#region metodos interface IFiltro
		public bool Analizar(long columna)
		{
			InicializaContadores();
			AnalizaColumna(columna);
			return CumpleCondiciones();
		}
		
		public string[] AnalizarFallos(long columna)
		{
			string[] arrayFallos=null;
			InicializaContadores();
            AnalizaColumna(columna);
			EsColumnaValida(ref arrayFallos);
			return arrayFallos;
		}

		public Filtro NombreFiltro
		{
			get{ return Filtro.NoInterrupciones; }
		}
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			int noAciertosTols = 0;
			
			//formato letrasTolerancias es: "ABC", "A", "CD", etc...
			
			foreach( char letra in letrasTolerancias)
			{
				if( intGlobalTol[ noIntGlobal ] == letra)
				{					
					noAciertosTols++;		
				}
				if( intVarTol[ noIntVar ] == letra)
				{					
					noAciertosTols++;		
				}
				if( int1Tol[ noInt1 ] == letra)
				{					
					noAciertosTols++;		
				}
				if( intXTol[ noIntX ] == letra)
				{					
					noAciertosTols++;		
				}
				if( int2Tol[ noInt2 ] == letra)
				{					
					noAciertosTols++;		
				}

				//int seguidas
				if( intGlobalSegTol[ noIntGlobalSeg ] == letra)
				{					
					noAciertosTols++;		
				}
				if( intVarSegTol[ noIntVarSeg ] == letra)
				{					
					noAciertosTols++;		
				}
				if( int1SegTol[ noInt1Seg ] == letra)
				{					
					noAciertosTols++;		
				}
				if( intXSegTol[ noIntXSeg ] == letra)
				{					
					noAciertosTols++;		
				}
				if( int2SegTol[ noInt2Seg ] == letra)
				{					
					noAciertosTols++;		
				}
			}			
			return noAciertosTols;
		}
		
		#endregion 
			
		
		public void ReinicializaValores()
		{
			for( int i = 0; i < intGlobal.Length; i++ )
			{
				intGlobal[i] = false;	
				intVar[i] = false;	
				int1[i] = false;	
				intX[i] = false;	
				int2[i] = false;	

				intGlobalSeg[i] = false;	
				intVarSeg[i] = false;	
				int1Seg[i] = false;	
				intXSeg[i] = false;	
				int2Seg[i] = false;
		
				intGlobalTol[i] = ' ';
				intVarTol[i] = ' ';
				int1Tol[i] = ' ';
				intXTol[i] = ' ';
				int2Tol[i] = ' ';

				intGlobalSegTol[i] = ' ';
				intVarSegTol[i] = ' ';
				int1SegTol[i] = ' ';
				intXSegTol[i] = ' ';
				int2SegTol[i] = ' ';
			}
		}
				
		public void SetNoIntGlobales(string valores)
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
				
				intGlobal[valorNumerico] = true;
				intGlobalTol[valorNumerico] = letraTol[0];
			}		
		}
	
		public void SetNoInt1(string valores)
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
				
				int1[valorNumerico] = true;
				int1Tol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNoIntX(string valores)
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
				
				intX[valorNumerico] = true;
				intXTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNoInt2(string valores)
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
				
				int2[valorNumerico] = true;
				int2Tol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNoIntVar(string valores)
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
				
				intVar[valorNumerico] = true;
				intVarTol[valorNumerico] = letraTol[0];
			}		
		}
		
		public string GetIntGlobales()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in intGlobal )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (intGlobalTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetInt1()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in int1 )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (int1Tol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetIntX()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in intX )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (intXTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetInt2()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in int2 )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (int2Tol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetIntVar()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in intVar )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (intVarTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}
		
		public void SetNoIntGlobalSeg(string valores)
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
				
				intGlobalSeg[valorNumerico] = true;
				intGlobalSegTol[valorNumerico] = letraTol[0];
			}		
		}
	
		public void SetNoInt1Seg(string valores)
		{
			string[] strValores = valores.Split(',');
			
			int valorNumerico;
			string letraTol ;
			
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
				
				int1Seg[valorNumerico] = true;
				int1SegTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNoIntXSeg(string valores)
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
				
				intXSeg[valorNumerico] = true;
				intXSegTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNoInt2Seg(string valores)
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
				
				int2Seg[valorNumerico] = true;
				int2SegTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNoIntVarSeg(string valores)
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
				
				intVarSeg[valorNumerico] = true;
				intVarSegTol[valorNumerico] = letraTol[0];
			}		
		}
		
		public string GetIntGlobalSeg()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in intGlobalSeg )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (intGlobalSegTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetInt1Seg()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in int1Seg )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (int1SegTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetIntXSeg()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in intXSeg )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (intXSegTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetInt2Seg()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in int2Seg )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (int2SegTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetIntVarSeg()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in intVarSeg )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (intVarSegTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

					
		public bool IsActive
		{
			get{ return isActive; } 
			set{ isActive = value; }
		}
		
		public bool ContieneDatos
		{
			get { return contieneDatos; }
			set { contieneDatos = value; }		
		}
		
		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.NoInterrupciones.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public void LlenarTodosValores()
		{
			// Pone como True todos aquellos valores que no tengan ningún valor asignado
			string todosValores="";
			for(int i=0;i<=VariablesGlobales.NumeroPartidos;i++)
			{
				todosValores+=","+i;
			}
			todosValores=todosValores.Substring(1);
			if(hayValores(intGlobal)==false) SetNoIntGlobales(todosValores);
			if(hayValores(intVar)==false) SetNoIntVar(todosValores);
			if(hayValores(int1)==false) SetNoInt1(todosValores);
			if(hayValores(intX)==false) SetNoIntX(todosValores);
			if(hayValores(int2)==false) SetNoInt2(todosValores);
			if(hayValores(intGlobalSeg)==false) SetNoIntGlobalSeg(todosValores);
			if(hayValores(intVarSeg)==false) SetNoIntVarSeg(todosValores);
			if(hayValores(int1Seg)==false) SetNoInt1Seg(todosValores);
			if(hayValores(intXSeg)==false) SetNoIntXSeg(todosValores);
			if(hayValores(int2Seg)==false) SetNoInt2Seg(todosValores);
		}
			
		private bool hayValores(bool[] propiedad)
		{
			for(int i=0;i<propiedad.Length;i++)
			{
				if(propiedad[i]==true) return true;
			}
			return false;
		}
			
		public int NoInterrupciones
		{
			get{ return noIntGlobal ; }
		}
        public int NoInterrupcionesGlobalesSeguidas
        {
            get { return noIntGlobalSeg; }
        }
        public int NoInterrupcionesUnos
        {
            get { return noInt1; }
        }
        public int NoInterrupcionesUnosSeguidos
        {
            get { return noInt1Seg; }
        }
        public int NoInterrupcionesEquis
        {
            get { return noIntX; }
        }
        public int NoInterrupcionesEquisSeguidas
        {
            get { return noIntXSeg; }
        }
        public int NoInterrupcionesDoses
        {
            get { return noInt2; }
        }
        public int NoInterrupcionesDosesSeguidas
        {
            get { return noInt2Seg; }
        }
        public int NoInterrupcionesVariantes
        {
            get { return noIntVar; }
        }
        public int NoInterrupcionesVariantesSeguidas
        {
            get { return noIntVarSeg; }
        }

        #region Miembros de IFiltro


        public bool UsaFiguras()
        {
            if ((Figuras == null) || (Figuras.Count == 0))
            {
                return false;
            }
            return true;
        }

	    public List<long> Figuras
        {
            get
            {
                return figuras;
            }
            set
            {
                figuras = value;
            }
        }
        public bool AnalisisActivo
        {
            get { return VariablesGlobales.AnalizarInterrupciones; }
        }
        #endregion
    }
}

