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
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for ControladorRelacionesCP1.
	/// </summary>
	public class ControladorRelacionesCP1
	{
		protected List<RelacionCP1> relaciones;
		protected List<ColumnaProbable> columnasProbables;
		

		public ControladorRelacionesCP1()
		{
			relaciones = new List<RelacionCP1>();
		}

		public void PonerRelacion(RelacionCP1 rel)
		{
			relaciones.Add( rel );	
			rel.Controlador = this;
		}

		public bool Analiza()
		{
			bool columnaValida = true;

		    for(int i = 0; i < relaciones.Count; i++)
			{
				RelacionCP1 rel = relaciones[i];
				columnaValida = rel.Analiza();

				if (columnaValida == false)
				{
					break;
				}
			}

			return columnaValida;		
		}

		public void Analiza(ref string txt)
		{
			txt="";
		    for(int i = 0; i < relaciones.Count; i++)
		    {
		        RelacionCP1 rel = relaciones[i];
		        txt+= rel.Analiza(i+1);
		    }
		}

		public List<RelacionCP1> Relaciones
		{
			get { return relaciones; }
			set 
			{ 
				relaciones = value; 
				
				//añadir referencia a este objeto "padre"
				foreach(RelacionCP1 rel in relaciones)
				{
					rel.Controlador = this;				
				}			
			}
		}	

		public List<ColumnaProbable> ColumnasProbables
		{
			get{ return columnasProbables; }
			set{ columnasProbables = value; }
		}
		
	}
}
