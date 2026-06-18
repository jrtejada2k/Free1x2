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
using System;
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for RangosOpciones.
	/// </summary>
	public class RangosOpciones
	{
	    private List<Rango> rangos = new List<Rango>();

	    public void PonerRangos(string valores)
		{
			//separar los diferentes rangos
			string[] valoresArray = valores.Split('#');

	        foreach(string rangoValores in valoresArray)
			{
				string[] rangoArray = rangoValores.Split('-');
			
				Rango rango = new Rango();

				if(rangoArray.Length == 1)
				{
					//solo se ha especificado un valor, asi que maximo y minimo es identico
					rango.Min = Convert.ToDouble(rangoArray[0]);
					rango.Max = Convert.ToDouble(rangoArray[0]);
				}
				else
				{
					rango.Min = Convert.ToDouble(rangoArray[0]);
					rango.Max = Convert.ToDouble(rangoArray[1]);				
				}				

				rangos.Add(rango);
			}
		
		}

		public bool ValorEnRangoValido(double valor)
		{
			bool valorValido = false;

		    for(int i = 0; i < rangos.Count; i++)
		    {
		        Rango rango = rangos[i];

		        if( valor <= rango.Max && valor >= rango.Min )
				{
					valorValido = true;
					break;
				}
		    }

		    return valorValido;
		}

		public void ReinicializarRangos()
		{
			rangos.Clear();		
		}

		public int Count
		{
			get{ return rangos.Count; }
		}

		protected class Rango
		{
			double min;
			double max;

			public double Min
			{
				get{ return min; }
				set{ min = value; }
			}

			public double Max
			{
				get{ return max; }
				set{ max = value; }
			}
		
		
		}
	}
}
