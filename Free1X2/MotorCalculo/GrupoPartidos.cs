// created on 18/08/2003 at 23:00
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
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
using System.Collections;

namespace Free1X2.MotorCalculo
{
	public class GrupoPartidos: IEnumerable
	{
		private List<Grupo> grupos;
		
		//ctrlGrupos es el objeto que contiene todos los controles de grupo
		private ControladorGrupos ctrlGrupos; 
				
		public GrupoPartidos()
		{
            grupos = new List<Grupo>();			
		}
		
		public void AddGrupo(Grupo grupo)
		{
            grupo.CalcularMascara();
            grupos.Add( grupo );
            int noGrupoNuevo = grupos.Count-1;
			
			//poner grupo en control de grupos libres.			
			ControlGrupos cg = ctrlGrupos.ControlesGrupos[0];
			cg.PonerGrupo( noGrupoNuevo );
		}	
		
		public void InsertarGrupo(Grupo grupo, int posicion)
		{
			grupos.Insert(posicion, grupo );
			//poner grupo en control de grupos libres.			
			ControlGrupos cg = ctrlGrupos.ControlesGrupos[0];
			cg.PonerGrupo( posicion );
		}	
		
		public void BorrarGrupo(int posicion)
		{
			grupos.RemoveAt(posicion);
		}

        public int Count
		{
			get{ return grupos.Count; }
		}
		
		public Grupo this[int index]
		{
			get {  return grupos[index]; }		
		}
		
		public ControladorGrupos CtrlGrupos
		{
			get{ return ctrlGrupos; }
			set{ ctrlGrupos = value; }		
		}
		
		// Implement the IEnumerable interface 
   		public IEnumerator GetEnumerator() 
   		{
      		return grupos.GetEnumerator();
      	}		
	}
}
