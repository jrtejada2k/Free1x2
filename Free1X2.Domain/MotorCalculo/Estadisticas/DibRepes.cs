// Free1X2 · WinUI 3 — WIN3
// created on 30/04/2004 at 8:19
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
	public class DibRepes {
		private string old = "00000000000000";

	    public int[] Procesar(string columna) {
			int[] ind = new int[5];
			int n1, nx, n2;
	        int np = n1=nx=n2=0; 
			for (int nr=0; nr<14; nr++) {
				char ch = columna[nr];
				if (ch==old[nr]) np++;
				if (ch=='1') n1++;
				else if (ch=='2') n2++;
				else nx++; 
			}
			ind[0]=np; ind[2]=n1; ind[3]=nx; ind[4]=n2;
			old=columna;
			return ind;
		}

	}
}
