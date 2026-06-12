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

using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for ControladorRelacionesGE1.
	/// </summary>
	public class ControladorRelacionesGE1
	{
		protected List<RelacionGE1> relaciones;
		protected List<GrupoEquipos> gruposEquipos;

		public ControladorRelacionesGE1()
		{
			relaciones = new List<RelacionGE1>();
		}

		public bool Analiza()
		{
		    for(int i = 0; i < relaciones.Count; i++)
			{
                if (!relaciones[i].Analiza())
				{
					return false;
				}
			}
			return true;		
		}

		public string AnalizaRelaciones()
		{
			string txt="";
		    for(int i = 0; i < relaciones.Count; i++)
		    {
		        RelacionGE1 rel = relaciones[i];
		        txt=rel.Analiza(i+1);
		    }
		    return txt;		
		}

		public List<RelacionGE1> Relaciones
		{
			get { return relaciones; }
			set 
			{ 
				relaciones = value; 
				
				//añadir referencia a este objeto "padre"
				foreach(RelacionGE1 rel in relaciones)
				{
					rel.Controlador = this;				
				}			
			}
		}	

		public List<GrupoEquipos> GruposEquipos
		{
			get{ return gruposEquipos; }
			set{ gruposEquipos = value; }
		}
	}
}
