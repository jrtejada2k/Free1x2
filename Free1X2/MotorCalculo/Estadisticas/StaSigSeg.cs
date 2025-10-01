// created on 02/05/2004 at 8:44
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
	public class StaSigSeg {
	    public int[] Procesar(string columna) {
			int mx, m2, mv;
			int nx, n2, nv;
	        int[] ind = new int[5];
			int m1 = mx=m2=mv=0;
			int n1 = nx=n2=nv=0;
			char ch0 = columna[0]; 
			if (ch0=='1') n1++;
			else if (ch0=='2') n2++;
			else nx++;
			if (ch0!='1') nv++;
			for (int nr=1; nr<14; nr++) {
				char ch = columna[nr];
				if (ch==ch0) {
					if (ch=='1') n1++;
					else if (ch=='2') n2++;
					else nx++;
					if (ch!='1') nv++;
				}
				else {
					if (ch0=='1') {
						if (n1>m1) m1=n1;
						n1=0; nv=1;
						if (ch=='2') n2=1;
						else nx=1;
					}
					else if (ch0=='2') {
						if (n2>m2) m2=n2;
						n2=0;
						if (ch=='1') {
							if (nv>mv) mv=nv;
							n1=1; nv=0;
						}
						else { nx=1; nv++; }
					}
					else {
						if (nx>mx) mx=nx;
						nx=0;
						if (ch=='2') { n2=1; nv++; }
						else {
							if (nv>mv) mv=nv;
							n1=1; nv=0;
						}
					}
				}
				ch0 = ch; 
			}
			if (n1>m1) m1=n1;
			if (nx>mx) mx=nx;
			if (n2>m2) m2=n2;
			if (nv>mv) mv=nv;
			ind[1]=mv; ind[2]=m1; ind[3]=mx; ind[4]=m2;
			return ind;
		}

	}
}
