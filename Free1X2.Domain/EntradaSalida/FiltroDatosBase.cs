// Free1X2 · WinUI 3 — WIN3
// created on 23/08/2003 at 14:19
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
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
using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	public abstract class FiltroDatosBase: IFiltroDatos
	{
		public abstract void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos);
	
		public abstract void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter);
		
		public int[] GetValores(string strValores)
		{	
			int[] valores = null;
			
			if( strValores != "" )
			{
				string[] strValues = strValores.Split(',');
				
				valores = new int[ strValues.Length ];
				
				for(int i = 0; i < strValues.Length; i++)
				{
					valores[i] = Convert.ToInt32( strValues[i] );	
				}
			}
			
			return valores;		
		}
					
	}
	
	
}
