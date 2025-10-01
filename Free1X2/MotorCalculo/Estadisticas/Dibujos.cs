// created on 25/04/2004 at 11:56
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 xfsf
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

namespace Free1X2.MotorCalculo.Estadisticas {
	public class Dibujos {
	    public int[] Procesar(string columna) {
			int n2;
	        int[] ind = new int[5];
			int nx = n2=0; 
			for (int nr2=0; nr2<14; nr2++)
			{
			    char ch = columna[nr2];
			    if (ch=='1') {}
				else if (ch=='2') n2++;
				else nx++;
			}
	        ind[3]=nx; ind[4]=n2;
			return ind;
		}

	}
}
