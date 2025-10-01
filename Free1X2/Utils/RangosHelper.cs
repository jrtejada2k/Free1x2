// created on 20/06/2004 at 18:00
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
using System.Collections;

namespace Free1X2.Utils
{
	/// <summary>
	/// Summary description for RangosOpciones.
	/// </summary>
	public class RangosHelper
	{
		private ArrayList tempBoolArray;

	    public int[] ObtenIntArray(string valores)
		{
			//valores puede ser del tipo 1,3,4,5 o 1,2-5,6 o 2-5

			//crea grupos de valores continuos... 1 o 1-4
			string[] valoresContArray = valores.Split(',');

			tempBoolArray = InicializarIntArray( valoresContArray );
		
			return (int[])tempBoolArray.ToArray(typeof(int));
		}

		protected ArrayList InicializarIntArray(string[] valores)
		{
			ArrayList array = new ArrayList();

			string[] tempArray;

			foreach(string str in valores)
			{
				tempArray = str.Split( '-' );

				if(tempArray.Length == 2)
				{
					int valor1 = Convert.ToInt32(tempArray[0]);
					int valor2 = Convert.ToInt32(tempArray[1]);

					int noValoresNuevos = (valor2 - valor1) + 1;

					for(int i = 0; i < noValoresNuevos; i++ )
					{
						array.Add(valor1 + i);				
					}				
				}
				else
				{
					int valor1 = Convert.ToInt32(tempArray[0]);
					array.Add( valor1 );	
				}
			}	

			return array;
		}

		public bool[] ObtenBoolArray(string valores)
		{
			//valores puede ser del tipo 1,3,4,5 o 1,2-5,6 o 2-5			

			//crea grupos de valores continuos... 1 o 1-4
			string[] valoresContArray = valores.Split(',');

			tempBoolArray = InicializarBoolArray( valoresContArray );

			foreach(string str in valoresContArray)
			{	
				PonerRangos(str);				
			}
			
			return (bool[])tempBoolArray.ToArray(typeof(bool));		
		}

		protected ArrayList InicializarBoolArray(string[] valores)
		{
			ArrayList array = new ArrayList();
		    int intMax = 0;

			foreach(string str in valores)
			{
			    string[] strTemp = str.Split('-');

			    foreach(string str2 in strTemp)
			    {
			        int intTemp = Convert.ToInt32(str2);

			        if(intTemp > intMax)
					{
						intMax = intTemp;
					}
			    }
			}

		    bool[] boolArray = new bool[intMax + 2];
			array.AddRange( boolArray );

			return array;		
		}

		protected void PonerRangos(string strRango)
		{
			if(strRango.IndexOf("-") > -1)
			{
				string[] tempValor = strRango.Split('-');

				int val1 = Convert.ToInt32(tempValor[0]);
				int val2 = Convert.ToInt32(tempValor[1]);

				int noValNuevos = (val2 - val1) + 1;

				for(int i = 0; i < noValNuevos; i++ )
				{							
					tempBoolArray[val1 + i]= true;
				}			
			}
			else
			{
				//un solo digito:
				
				tempBoolArray[Convert.ToInt32(strRango)] = true;
			}					
		}		
	}
}
