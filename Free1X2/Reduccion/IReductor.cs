// created on - 05/02/2004 8:45:09
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

namespace Free1X2.Reduccion
{
	public interface IReduccion 
	{
		int NoColumnasIniciales { get; }
		int NoColumnasFinales { get; }
		int NoColumnasProcesadas { get; }
		
		void Cancelar();
		void ComienzaReduccion(string archivoEntrada, string sal, int nivelReduccion, int maxCol, int percent);
		void Inicializa(string entrada, int nivelReduccion);
	}
}
