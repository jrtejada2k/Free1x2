// Free1X2 · WinUI 3 — WIN3
// created on 01/02/2004 at 10:54
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

namespace Free1X2.Reduccion
{
	public abstract class Base
	{
		protected int noColumnasFinales;
		protected int noColumnasProcesadas;
		protected int noColumnasIniciales;
		protected bool salida;

		public int NoColumnasIniciales
		{
			get{ return noColumnasIniciales;}			
		}
		
		public int NoColumnasFinales
		{
			get { return noColumnasFinales;	}
		}
		
		public int NoColumnasProcesadas
		{
			get { return noColumnasProcesadas;}
		}

		abstract public void ComienzaReduccion(string archivoEntrada, string sal, int nivelReduccion, int maxCol, int percent);
		abstract protected void EntradaDeDatos(string archivoEntrada);
		abstract protected void Reduce(int nivelReduccion, int maxCol, int percent);
		abstract protected void GrabacionDeReductoras(string archivoEntrada, int nivelReduccion);
		public void Cancelar() { salida = true; }
	}
}
