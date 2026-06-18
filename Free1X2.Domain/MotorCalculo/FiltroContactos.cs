// Free1X2 · WinUI 3 — WIN3
// created on 16/08/2004 at 17:54
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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
	public class FiltroContactos: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        List<long> figuras;
        List<long> figurasLong = new List<long>();

	    //no. Contactos
		private int Num1X;
		private int Num12;
		private int NumX2;
		private int Num11;
		private int NumXX;
		private int Num22;
		private int Num1V;
		private int NumXV;
		private int Num2V;
		private int NumVV;
		
		//condiciones 
		private int noPartidos;
		//standard
		private bool[] cont1X;
		private bool[] cont12;
		private bool[] contX2;

		private bool[] cont11;
		private bool[] contXX;
		private bool[] cont22;
		
		private bool[] cont1V;
		private bool[] contXV;
		private bool[] cont2V;
		private bool[] contVV;
				   
		//tolerancias
		private char[] cont1XTol;
		private char[] cont12Tol;
		private char[] contX2Tol;
		
		private char[] cont11Tol;
		private char[] contXXTol;	
		private char[] cont22Tol;
		
		private char[] cont1VTol;
		private char[] contXVTol;
		private char[] cont2VTol;
		private char[] contVVTol;	
		
		public FiltroContactos()
		{
			//Este valor podria ser configurable
			noPartidos = VariablesGlobales.NumeroPartidos;

			cont1X = new bool[noPartidos+1];
			cont12 = new bool[noPartidos+1];
			contX2 = new bool[noPartidos+1];
			cont11 = new bool[noPartidos+1];
			contXX = new bool[noPartidos+1];

			cont22 = new bool[noPartidos+1];
			cont1V = new bool[noPartidos+1];
			contXV = new bool[noPartidos+1];
			cont2V = new bool[noPartidos+1];
			contVV = new bool[noPartidos+1];

			cont1XTol = new char[noPartidos+1];
			cont12Tol = new char[noPartidos+1];
			contX2Tol = new char[noPartidos+1];
			cont11Tol = new char[noPartidos+1];
			contXXTol = new char[noPartidos+1];

			cont22Tol = new char[noPartidos+1];
			cont1VTol = new char[noPartidos+1];
			contXVTol = new char[noPartidos+1];
			cont2VTol = new char[noPartidos+1];
			contVVTol = new char[noPartidos+1];			
		}

		public void Inicializa()
		{
			InicializaMatrices();
			InicializaContadores();
		}

		private void InicializaMatrices()
		{
			for( int i = 0; i < noPartidos+1; i++ )
			{
				cont1X[i] = false;	
				cont12[i] = false;	
				contX2[i] = false;	
				cont11[i] = false;	
				contXX[i] = false;	

				cont22[i] = false;	
				cont1V[i] = false;	
				contXV[i] = false;	
				cont2V[i] = false;	
				contVV[i] = false;
		
				cont1XTol[i] = ' ';
				cont12Tol[i] = ' ';
				contX2Tol[i] = ' ';
				cont11Tol[i] = ' ';
				contXXTol[i] = ' ';

				cont22Tol[i] = ' ';
				cont1VTol[i] = ' ';
				contXVTol[i] = ' ';
				cont2VTol[i] = ' ';
				contVVTol[i] = ' ';
			}
		}
		
		private void InicializaContadores()
		{
			Num1X = 0;
			Num12 = 0;
			NumX2 = 0;
			Num11 = 0;
			NumXX = 0;

			Num22 = 0;
			Num1V = 0;
			NumXV = 0;
			Num2V = 0;
			NumVV = 0;
		}
		
		public void AnalizaColumna(long columna)
		{
			InicializaContadores();
            long colSinAsteriscos = 0;
            byte signo1;
		    while (columna != 0)
            {
                signo1 = (byte)(columna & 7);
                if (signo1 != 0) colSinAsteriscos = colSinAsteriscos << 3 | signo1;
                columna >>= 3;
            }
            signo1 = (byte)(colSinAsteriscos & 7);

            while (colSinAsteriscos != 0)
            {
                colSinAsteriscos >>= 3;
                byte signo2 = (byte)(colSinAsteriscos & 7);
                if (signo2 > 0)
                {
                    switch (signo1 | signo2)
                    {
                        case 6: //1X
                            Num1X++;
                            break;
                        case 5: //12
                            Num12++;
                            break;
                        case 3: //X2
                            NumX2++;
                            break;
                        case 4: //11
                            Num11++;
                            break;
                        case 2: //XX
                            NumXX++;
                            break;
                        case 1: //22
                            Num22++;
                            break;
                    }
                    signo1 = signo2;
                }
            }
            Num1V = Num1X + Num12;
            NumVV = NumXX + NumX2 + Num22;
            Num2V = Num22 + NumX2;
            NumXV = NumXX + NumX2;
        }
		
		private bool EsColumnaValida()
		{
			bool esValida = true;
						
			if( cont1X[Num1X] == false ||
				cont12[Num12] == false ||
				contX2[NumX2] == false ||
				cont11[Num11] == false ||
				contXX[NumXX] == false ||

				cont22[Num22] == false ||
				cont1V[Num1V] == false ||
				contXV[NumXV] == false ||
				cont2V[Num2V] == false ||
				contVV[NumVV] == false )
			{
				esValida = false;
			}
			return  esValida;
		}	

		private void EsColumnaValida(ref string[] arrayFallos)
		{
			string texto="";
			if( cont11[Num11] == false )
				texto+="Fallo en contactos 11  ("+Num11+")#";
			if( cont11Tol[Num11]  != ' ' )
				texto+="!Tolerancia en contactos 11  ("+Num11+")#";
			if( cont1X[Num1X] == false )
				texto+="Fallo en contactos 1X  ("+Num1X+")#";
			if( cont1XTol[Num1X]  != ' ' )
				texto+="!Tolerancia en contactos 1X  ("+Num1X+")#";
			if( cont12[Num12] == false )
				texto+="Fallo en contactos 12  ("+Num12+")#";
			if( cont12Tol[Num12]  != ' ' )
				texto+="!Tolerancia en contactos 12  ("+Num12+")#";
			if( cont1V[Num1V] == false )
				texto+="Fallo en contactos 1V  ("+Num1V+")#";
			if( cont1VTol[Num1V]  != ' ' )
				texto+="!Tolerancia en contactos 1V  ("+Num1V+")#";
			if( contXX[NumXX] == false )
				texto+="Fallo en contactos XX  ("+NumXX+")#";
			if( contXXTol[NumXX]  != ' ' )
				texto+="!Tolerancia en contactos XX  ("+NumXX+")#";
			if( contX2[NumX2] == false )
				texto+="Fallo en contactos X2  ("+NumX2+")#";
			if( contX2Tol[NumX2]  != ' ' )
				texto+="!Tolerancia en contactos X2  ("+NumX2+")#";
			if( contXV[NumXV] == false )
				texto+="Fallo en contactos XV  ("+NumXV+")#";
			if( contXVTol[NumXV]  != ' ' )
				texto+="!Tolerancia en contactos XV  ("+NumXV+")#";
			if( cont22[Num22] == false )
				texto+="Fallo en contactos 22  ("+Num22+")#";
			if( cont22Tol[Num22]  != ' ' )
				texto+="!Tolerancia en contactos 22  ("+Num22+")#";
			if( cont2V[Num2V] == false )
				texto+="Fallo en contactos 2V  ("+Num2V+")#";
			if( cont2VTol[Num2V]  != ' ' )
				texto+="!Tolerancia en contactos 2V  ("+Num2V+")#";
			if( contVV[NumVV] == false )
				texto+="Fallo en contactos VV  ("+NumVV+")#";
			if( contVVTol[NumVV]  != ' ' )
				texto+="!Tolerancia en contactos VV  ("+NumVV+")#";
            if (UsaFiguras())
            {
                if (!Figuras.Contains(FiguraContactos.Figura))
                {
                    texto += "Fallo en Figuras  (" + Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(FiguraContactos.Figura) + ")#";
                }
            }
			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
		}

		private bool CumpleCondiciones()
		{
			bool esValida = false;
						
			//si acertamos la columna...
			if( EsColumnaValida())
			{	
				esValida = true;
                if (UsaFiguras())
                {
                    esValida = AnalizarFiguras();
                }

			}
			return esValida;
		}

        protected bool AnalizarFiguras()
        {
            return Figuras.Contains(FiguraContactos.Figura);
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
			AnalizaColumna(columna);
			EsColumnaValida(ref arrayFallos);
			return arrayFallos;
		}

		public Filtro NombreFiltro
		{
			get{ return Filtro.Contactos; }
		}
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			int noAciertosTols = 0;
			
			//formato letrasTolerancias es: "ABC", "A", "CD", etc...
			foreach( char letra in letrasTolerancias)
			{
				if( cont1XTol[ Num1X ] == letra)
				{					
					noAciertosTols++;		
				}
				if( cont12Tol[ Num12 ] == letra)
				{					
					noAciertosTols++;		
				}
				if( contX2Tol[ NumX2 ] == letra)
				{					
					noAciertosTols++;		
				}
				if( cont11Tol[ Num11 ] == letra)
				{					
					noAciertosTols++;		
				}
				if( contXXTol[ NumXX ] == letra)
				{					
					noAciertosTols++;		
				}
				if( cont22Tol[ Num22 ] == letra)
				{					
					noAciertosTols++;		
				}
				if( cont1VTol[ Num1V ] == letra)
				{					
					noAciertosTols++;		
				}
				if( contXVTol[ NumXV ] == letra)
				{					
					noAciertosTols++;		
				}
				if( cont2VTol[ Num2V ] == letra)
				{					
					noAciertosTols++;		
				}
				if( contVVTol[ NumVV ] == letra)
				{					
					noAciertosTols++;		
				}
			}			
			return noAciertosTols;
		}
		
		#endregion 
			
		
		public void ReinicializaValores()
		{
			for( int i = 0; i < cont1X.Length; i++ )
			{
				cont1X[i] = false;	
				cont12[i] = false;	
				contX2[i] = false;	
				cont11[i] = false;	
				contXX[i] = false;	

				cont22[i] = false;	
				cont1V[i] = false;	
				contXV[i] = false;	
				cont2V[i] = false;	
				contVV[i] = false;
		
				cont1XTol[i] = ' ';
				cont12Tol[i] = ' ';
				contX2Tol[i] = ' ';
				cont11Tol[i] = ' ';
				contXXTol[i] = ' ';

				cont22Tol[i] = ' ';
				cont1VTol[i] = ' ';
				contXVTol[i] = ' ';
				cont2VTol[i] = ' ';
				contVVTol[i] = ' ';
			}
		}
				
		public void SetNum1X(string valores)
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
				
				cont1X[valorNumerico] = true;
				cont1XTol[valorNumerico] = letraTol[0];
			}		
		}
	
		public void SetNumX2(string valores)
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
				
				contX2[valorNumerico] = true;
				contX2Tol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNum11(string valores)
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
				
				cont11[valorNumerico] = true;
				cont11Tol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNumXX(string valores)
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
				
				contXX[valorNumerico] = true;
				contXXTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNum12(string valores)
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
				
				cont12[valorNumerico] = true;
				cont12Tol[valorNumerico] = letraTol[0];
			}		
		}
		
		public string GetNum1X()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in cont1X )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (cont1XTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNumXX()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in contXX )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (contXXTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNumX2()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in contX2 )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (contX2Tol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNum12()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in cont12 )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (cont12Tol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNum11()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in cont11 )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (cont11Tol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}
		
		public void SetNum22(string valores)
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
				
				cont22[valorNumerico] = true;
				cont22Tol[valorNumerico] = letraTol[0];
			}		
		}
	
		public void SetNumXV(string valores)
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
				
				contXV[valorNumerico] = true;
				contXVTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNum2V(string valores)
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
				
				cont2V[valorNumerico] = true;
				cont2VTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNumVV(string valores)
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
				
				contVV[valorNumerico] = true;
				contVVTol[valorNumerico] = letraTol[0];
			}		
		}

		public void SetNum1V(string valores)
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
				
				cont1V[valorNumerico] = true;
				cont1VTol[valorNumerico] = letraTol[0];
			}		
		}
		
		public string GetNum22()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in cont22 )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (cont22Tol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNumXV()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in contXV )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (contXVTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNum2V()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in cont2V )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (cont2VTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNumVV()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in contVV )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (contVVTol[counter].ToString()).Trim();
				}
				
				counter++;
			}
			
			return valores;
		}

		public string GetNum1V()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in cont1V )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (cont1VTol[counter].ToString()).Trim();
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
		    if( nombre.Equals( Filtro.Contactos.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public void LlenarTodosValores()
		{
			// Pone como True todos aquellos valores que no tengan ningún valor asignado
			string todosValores="";
			for(int i=0;i<VariablesGlobales.NumeroPartidos;i++)
			{
				todosValores+=","+i;
			}
			todosValores=todosValores.Substring(1);
			if(hayValores(cont11)==false) SetNum11(todosValores);
			if(hayValores(cont12)==false) SetNum12(todosValores);
			if(hayValores(cont1V)==false) SetNum1V(todosValores);
			if(hayValores(cont1X)==false) SetNum1X(todosValores);
			if(hayValores(cont22)==false) SetNum22(todosValores);
			if(hayValores(cont2V)==false) SetNum2V(todosValores);
			if(hayValores(contVV)==false) SetNumVV(todosValores);
			if(hayValores(contX2)==false) SetNumX2(todosValores);
			if(hayValores(contXV)==false) SetNumXV(todosValores);
			if(hayValores(contXX)==false) SetNumXX(todosValores);
		}
			
		private bool hayValores(bool[] propiedad)
		{
			for(int i=0;i<propiedad.Length;i++)
			{
				if(propiedad[i]) return true;
			}
			return false;
		}
        protected FiguraCondicion ObtenerFiguraLongB()
        {
            FiguraCondicion figura = new FiguraCondicion();
            //Al comienzo de la figura vamos a meter un marcador, para que acepte los 0

            long temporal = 1;
            int[] valores = { Num1X, Num12, NumX2, Num11, NumXX, Num22, Num1V, NumXV, Num2V, NumVV };
            for (int i = 0; i < valores.Length; i++)
            {
                temporal <<= 4;
                temporal |= (uint)valores[i];
            }
            figura.Figura = temporal;
            return figura;
        }
        public FiguraCondicion ObtenerFiguraLong()
        {
            FiguraCondicion figura = new FiguraCondicion();
            long temporal = 0;
            List<int> valores = new List<int>();
            valores.Add(Num1X);
            valores.Add(Num12);
            valores.Add(NumX2);
            valores.Add(Num11);
            valores.Add(NumXX);
            valores.Add(Num22);
            valores.Add(Num1V);
            valores.Add(NumXV);
            valores.Add(Num2V);
            valores.Add(NumVV);

            valores.Sort();

            for (int i = valores.Count - 1; i >= 0; i--)
            {
                if (valores[i] > 0)
                {
                    temporal <<= 4;
                    temporal |= (uint)valores[i];
                }
            }
            figura.Figura = temporal;
            return figura;
        }

        public int NoContactos1X
		{
			get{ return Num1X ; }
		}
        public int NoContactos12
        {
            get { return Num12; }
        }
        public int NoContactos11
        {
            get { return Num11; }
        }
        public int NoContactos1V
        {
            get { return Num1V; }
        }
        public int NoContactos22
        {
            get { return Num22; }
        }
        public int NoContactos2V
        {
            get { return Num2V; }
        }
        public int NoContactosVV
        {
            get { return NumVV; }
        }
        public int NoContactosX2
        {
            get { return NumX2; }
        }
        public int NoContactosXV
        {
            get { return NumXV; }
        }
        public int NoContactosXX
        {
            get { return NumXX; }
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
        public List<long> FigurasLong
        {
            get
            {
                return figurasLong;
            }
            set
            {
                figurasLong = value;
            }
        }

        public FiguraCondicion FiguraContactos
        {
            get { return ObtenerFiguraLong(); }
        }
        public bool AnalisisActivo
        {
            get { return VariablesGlobales.AnalizarContactos; }
        }
        #endregion
    }
}

