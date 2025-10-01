// created on 25/08/2003 at 17:47
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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

using System.Collections;

namespace Free1X2.EntradaSalida
{
	public interface IArchivoColumnas
	{
		int NumColumnas{get;}
        int NumPartidos { set;}
		void Cerrar();
		string DarFormatoColumna(string lineaArchivo);
		void GuardarCols( string columna );
		void GuardarCols( int columna );
        void GuardarCols(long columna);
		void GuardarColsComa( string columna );
		void GuardarTodasCols(string[] columnas);
        void GuardarTodasCols(string[] columnas, bool conComa);
		void GuardarTodasCols(int[] columnas);
        void GuardarTodasCols(int[] columnas, bool conComa);
		void GuardarTodasCols(BitArray columnas);
        void GuardarTodasCols(BitArray columnas, bool conComa);
		bool SiguienteColumna();
		string LeeColumna();
		string LeeColumnaSinComas();
		string[] LeerTodasCols(bool conFormato);
		int[] LeerTodasColsANumero();
        string[] LeerTodasColsSeparadasPorComas();
		BitArray LeerTodasColsABitArray(int numeroPartidos);
		long ObtenNumCols();
        int ObtenNumSignos();
	}	
}
