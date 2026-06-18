// Free1X2 · WinUI 3 — WIN3
// created on 28/07/2004 at 16:47
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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
	public class FiltroDistancias: IFiltro
	{
		const long MascUnos = 5270498306774157604;
        const long MascEquis = 2635249153387078802;
        const long MascDoses = 1317624576693539401;
        const long MascVariantes = 3952873730080618203;
        List<long> figuras;

        private bool isActive;
		private bool contieneDatos;
		
		//no. Distancias
		private int noIntGlobal;
		private int distVar;
		private int distUnos;
		private int distEquis;
		private int distDoses;

		//condiciones 
		private int noPartidos;
		//standard
		private bool[] intGlobal;
		private bool[] intVar;
		private bool[] int1;
		private bool[] intX;
		private bool[] int2;

		//tolerancias
		private char[] intGlobalTol;
		private char[] intVarTol;
		private char[] int1Tol;
		private char[] intXTol;
		private char[] int2Tol;	
	
	
		public FiltroDistancias()
		{
			//Este valor podria ser configurable
			noPartidos = VariablesGlobales.NumeroPartidos;

			intGlobal = new bool[noPartidos+1];
			intVar = new bool[noPartidos+1];
			int1 = new bool[noPartidos+1];
			intX = new bool[noPartidos+1];
			int2 = new bool[noPartidos+1];

			intGlobalTol = new char[noPartidos+1];
			intVarTol = new char[noPartidos+1];
			int1Tol = new char[noPartidos+1];
			intXTol = new char[noPartidos+1];
			int2Tol = new char[noPartidos+1];

		
			for( int i = 0; i < noPartidos+1; i++ )
			{
				intGlobal[i] = false;	
				intVar[i] = false;	
				int1[i] = false;	
				intX[i] = false;	
				int2[i] = false;	

				intGlobalTol[i] = ' ';
				intVarTol[i] = ' ';
				int1Tol[i] = ' ';
				intXTol[i] = ' ';
				int2Tol[i] = ' ';
			}		
		}		
		
		private void InicializaContadores()
		{
			distVar = 0;
			distUnos = 0;
			distEquis = 0;
			distDoses = 0;
		}
		
		public void AnalizaColumna(long columna)
		{
            distUnos = Distancia(columna & MascUnos);
            distEquis = Distancia(columna & MascEquis);
            distDoses = Distancia(columna & MascDoses);
            distVar = Distancia(columna & MascVariantes);
		}
        public int Distancia(long columna)
        {
            int act, dist = 0;
            if (columna != 0)
            {
                while ((columna & 7) == 0) columna >>= 3;
                do
                {
                    columna >>= 3;
                    act = 0;
                    while ((columna & 7) == 0 && columna != 0)
                    {
                        act++;
                        columna >>= 3;
                    }
                    if (columna != 0) dist = Math.Max(act + 1, dist);
                } while (columna != 0);
            }
            return dist;
        }

		private bool EsColumnaValida()
		{
			/*bool esValida = true;
						
			if( !intVar[distVar] ||	
				!int1[distUnos] ||	
				!intX[distEquis] ||	
				!int2[distDoses] )
			{
				esValida = false;
			}
			return  esValida;*/

            return (intVar[distVar] && int1[distUnos] && intX[distEquis] && int2[distDoses]);
		}

		private void EsColumnaValida(ref string[] arrayFallos)
		{
			string texto="";
			if( intVar[distVar] == false )
				texto+="Fallo en distancia de variantes  ("+distVar+")#";
			if( intVarTol[distVar]  != ' ' )
				texto+="!Tolerancia en distancia de variantes  ("+distVar+")#";
			if( int1[distUnos] == false )
				texto+="Fallo en distancia de unos  ("+distUnos+")#";
			if( int1Tol[distUnos]  != ' ' )
				texto+="!Tolerancia en distancia de unos  ("+distUnos+")#";
			if( intX[distEquis] == false )
				texto+="Fallo en distancia de equis  ("+distEquis+")#";
			if( intXTol[distEquis]  != ' ' )
				texto+="!Tolerancia en distancia de equis  ("+distEquis+")#";
			if( int2[distDoses] == false )
				texto+="Fallo en distancia de doses  ("+distDoses+")#";
			if( int2Tol[distDoses]  != ' ' )
				texto+="!Tolerancia en distancia de doses  ("+distDoses+")#";
			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
		}
				
		private bool CumpleCondiciones()
		{
			/*bool esValida = false;
						
			//si acertamos la columna...
			if( EsColumnaValida())
			{	
				esValida = true;						
			}
			return esValida;*/
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
			get{ return Filtro.Distancias; }
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
				if( intVarTol[ distVar ] == letra)
				{					
					noAciertosTols++;		
				}
				if( int1Tol[ distUnos ] == letra)
				{					
					noAciertosTols++;		
				}
				if( intXTol[ distEquis ] == letra)
				{					
					noAciertosTols++;		
				}
				if( int2Tol[ distDoses ] == letra)
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

				intGlobalTol[i] = ' ';
				intVarTol[i] = ' ';
				int1Tol[i] = ' ';
				intXTol[i] = ' ';
				int2Tol[i] = ' ';
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
	
		public void SetdistUnos(string valores)
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

		public void SetdistEquis(string valores)
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

		public void SetdistDoses(string valores)
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

		public void SetdistVar(string valores)
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
		    if( nombre.Equals( Filtro.Distancias.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public int NoDistancias
		{
			get{ return noIntGlobal ; }
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

		public void LlenarTodosValores()
		{
			// Pone como True todos aquellos valores que no tengan ningún valor asignado
			string todosValores="";
			for(int i=0;i<VariablesGlobales.NumeroPartidos;i++)
			{
				todosValores+=","+i;
			}
			todosValores=todosValores.Substring(1);
			if(hayValores(int1)==false) SetdistUnos(todosValores);
			if(hayValores(intX)==false) SetdistEquis(todosValores);
			if(hayValores(int2)==false) SetdistDoses(todosValores);
			if(hayValores(intVar)==false) SetdistVar(todosValores);
		}

		private bool hayValores(bool[] propiedad)
		{
			for(int i=0;i<propiedad.Length;i++)
			{
				if(propiedad[i]==true) return true;
			}
			return false;
		}

        public int NoDistanciasUnos
        {
            get { return distUnos; }
        }
        public int NoDistanciasEquis
        {
            get { return distEquis; }
        }
        public int NoDistanciasDoses
        {
            get { return distDoses; }
        }
        public int NoDistanciasVariantes
        {
            get { return distVar; }
        }
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
            get { return VariablesGlobales.AnalizarDistancias; }
        }
	}
}

