// Free1X2 · WinUI 3 — WIN3
// created on 10/08/2003 at 17:44
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004-2006 Luis Fernandez - luis_fernandez10@excite.com
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
	public class FiltroNoVariantes: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
		//contadores columna
		private int noVariantesCol;
		private int noEquisCol;
		private int noDosesCol;
        List<long> figuras;

		//condiciones 
		private int noPartidos;
		//standard
		private bool[] variantes;
		private bool[] equis;
		private bool[] doses;
		//tolerancias
		private char[] variantesTol;
		private char[] equisTol;
		private char[] dosesTol;
		
		public FiltroNoVariantes()
		{
			//Este valor podria ser configurable
			noPartidos = VariablesGlobales.NumeroPartidos;
			
			variantes = new bool[noPartidos+1];
			equis = new bool[noPartidos+1];
			doses = new bool[noPartidos+1];
			
			variantesTol = new char[noPartidos+1];
			equisTol = new char[noPartidos+1];
			dosesTol = new char[noPartidos+1];
			
			for( int i = 0; i < noPartidos+1; i++ )
			{
				variantes[i] = false;
				equis[i] = false;
				doses[i] = false;
				
				variantesTol[i] = ' ';
				equisTol[i] = ' ';
				dosesTol[i] = ' ';
			}
			
		}		
		
		private void InicializaContadores()
		{
			noVariantesCol = 0;
			noEquisCol = 0;
			noDosesCol = 0;					
		}
		
		public void AnalizaColumna(string columna)
		{
			InicializaContadores();
			for(int i = 0; i < columna.Length ; i++)
			{
				switch( columna[i] )
				{
                    case 'X':
					case 'x':
						noVariantesCol++;
						noEquisCol++;					
						break;
					
					case '2':
						noVariantesCol++;
						noDosesCol++;
						break;
				}	
				
			}		
				
		}

        public void AnalizaColumna(long columna)
        {
            //mascara: activados solo los x: 
            //010010010010010010010010010010010010010010 
            noEquisCol = UtilColumnas.ContarBitsA1(columna & 80421421917330);

            //mascara: activados solo los 2: 
            //001001001001001001001001001001001001001001 
            noDosesCol = UtilColumnas.ContarBitsA1(columna & 5026338869833);
            
            //mascara: activados los X2: 
            //011011011011011011011011011011011011011011
            //noVariantesCol = UtilColumnas.ContarBitsA1(columna & 1884877076187);            
            noVariantesCol = noEquisCol + noDosesCol;
        }

        private bool esColumnaValida()
        {
            return (variantes[noVariantesCol] &&
                equis[noEquisCol] &&
                doses[noDosesCol]);
        }	

		private void esColumnaValida(ref string[] arrayFallos)
		{
			string texto="";
			if( variantes[ noVariantesCol ] == false)
				texto+="Fallo en número de variantes  ("+noVariantesCol+")#";
			if(variantesTol[ noVariantesCol ]!=' ')
				texto+="!Tolerancia en número de variantes  ("+noVariantesCol+")#";
			if( equis[ noEquisCol ] == false)
				texto+="Fallo en número de equis  ("+noEquisCol+")#";
			if(equisTol[ noEquisCol ] != ' ')
				texto+="!Tolerancia en número de equis  ("+noEquisCol+")#";
			if( doses[ noDosesCol ] == false)
				texto+="Fallo en número de doses  ("+noDosesCol+")#";
			if(dosesTol[ noDosesCol ] != ' ')
				texto+="!Tolerancia en número de doses  ("+noDosesCol+")#";
			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
		}

		private bool CumpleCondiciones()
		{
            return esColumnaValida();
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
			esColumnaValida(ref arrayFallos);
			return arrayFallos;
		}

		public Filtro NombreFiltro
		{
			get{ return Filtro.NoVariantes; }
		}
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			int noAciertosTols = 0;
			
			//formato letrasTolerancias es: "ABC", "A", "CD", etc...
			
			foreach( char letra in letrasTolerancias)
			{
				if( variantesTol[ noVariantesCol ] == letra)
				{					
					noAciertosTols++;		
				}
				if( equisTol[ noEquisCol ] == letra)
				{					
					noAciertosTols++;		
				}
				if( dosesTol[ noDosesCol ] == letra)
				{					
					noAciertosTols++;		
				}				
			}			
		
			return noAciertosTols;
		}
		
		#endregion 
			
		
		public void ReinicializaValores()
		{
			for( int i = 0; i < variantes.Length; i++ )
			{
				variantes[i] = false;
				equis[i] = false;
				doses[i] = false;
				
				variantesTol[i] = ' ';
				equisTol[i] = ' ';
				dosesTol[i] = ' ';
			}
		}
				
		public void SetNoVariantes(int noVariantes, bool active)
		{
			variantes[noVariantes] = active; 
		}
		
		public void SetNoVariantes(int[] noVariantes)
		{			
			foreach(int i in noVariantes)
			{
				SetNoVariantes(i, true);
			}			
		}
		
		public void SetNoVariantes(string valores)
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
				
				variantes[valorNumerico] = true;
				variantesTol[valorNumerico] = letraTol[0];
			}		
		}		
		
		public void SetNoEquis(int noEquis, bool active)
		{
			equis[noEquis] = active; 
		}
		
		public void SetNoEquis(int[] noEquis)
		{			
			foreach(int i in noEquis)
			{
				SetNoEquis(i, true);
			}			
		}
		
		public void SetNoEquis(string valores)
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
				
				equis[valorNumerico] = true;
				equisTol[valorNumerico] = letraTol[0];
			}	
		}		
		
		public void SetNoDoses(int noDoses, bool active)
		{
			doses[noDoses] = active; 
		}
		
		public void SetNoDoses(int[] noDoses)
		{			
			foreach(int i in noDoses)
			{
				SetNoDoses(i, true);
			}			
		}
		
		public void SetNoDoses(string valores)
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
				
				doses[valorNumerico] = true;
				dosesTol[valorNumerico] = letraTol[0];
			}		
		}		
		
		public string GetVariantes()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in variantes )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (variantesTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
			
		public string GetEquis()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in equis )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (equisTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
			
		public string GetDoses()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in doses )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (dosesTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
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
			if(hayValores(variantes)==false) SetNoVariantes(todosValores);
			if(hayValores(equis)==false) SetNoEquis(todosValores);
			if(hayValores(doses)==false) SetNoDoses(todosValores);
		}

		private bool hayValores(bool[] propiedad)
		{
			for(int i=0;i<propiedad.Length;i++)
			{
				if(propiedad[i]) return true;
			}
			return false;
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
		    if( nombre.Equals( Filtro.NoVariantes.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public int NoVariantes
		{
			get{ return noVariantesCol ; }
		} 

		public int NoEquis
		{
			get{ return noEquisCol ; }
		} 

		public int NoDoses
		{
			get{ return noDosesCol ; }
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
            get { return VariablesGlobales.AnalizarVX2; }
        }
        #endregion
    }
}
