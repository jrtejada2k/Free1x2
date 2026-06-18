// Free1X2 · WinUI 3 — WIN3
// created on 23/06/2005
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2005 Toni Moreno  toni [at] moreno-csa [dot] com
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

namespace Free1X2.MotorCalculo
{
	public class GrupoIfThen
	{
		private Grupo grupoIf;
		private Grupo grupoThen;
		private int numGrupoIf;
		private int numGrupoThen;
		private bool noIf;
		private bool noThen;

	    public Grupo GrupoIf
		{
			get{return grupoIf;}
			set{grupoIf=value;}
		}

		public Grupo GrupoThen
		{
			get{return grupoThen;}
			set{grupoThen=value;}
		}
		
		public int NumGrupoIf
		{
			get{return numGrupoIf;}
			set{numGrupoIf=value;}
		}

		public int NumGrupoThen
		{
			get{return numGrupoThen;}
			set{numGrupoThen=value;}
		}
		
		public bool NoIf
		{
			get{return noIf;}
			set{noIf=value;}
		}
		
		public bool NoThen
		{
			get{return noThen;}
			set{noThen=value;}
		}

		public bool CompruebaPronostico(long columna, GrupoPartidos gruposPartidos)
		{
		    Grupo grupo = gruposPartidos[NumGrupoIf];
			bool esValido = grupo.AnalizaColumna(columna);
			if(NoIf) esValido=!esValido;
			// Si la condición If se falla, la condición no se evalúa y la
			// columna es correcta.
			if(esValido==false) return true;

			grupo=gruposPartidos[NumGrupoThen];
			esValido=grupo.AnalizaColumna(columna);
			if(NoThen) esValido=!esValido;
			if(esValido==false) return false;
		    return true;
		}
	}
}
