// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis xfsf 
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
namespace Free1X2.Utils
{
	public class Comparador2 {
		private readonly int[,] comtab = new int[2187,2187];
		private int c7p, c7u, n7p, n7u;
		
        public Comparador2() {
			comtab[0,0]=7;
			for (int nr=0; nr<2186;nr++) {
				string ax1 = n2s(nr);
				for (int nr2=nr+1; nr2<2187; nr2++) {
					string ax2 = n2s(nr2);
					int na = neq(ax1, ax2);
					comtab[nr,nr2]=na;
					comtab[nr2,nr]=na;
					comtab[nr2,nr2]=7;
				}
			}
		}
		public string n2s(int nx1, int nx2) { return n2s(nx1)+n2s(nx2); }
		public int neq(int nx1, int nx2) { 
			return comtab[c7p,nx1]+comtab[c7u,nx2]; 
		}
		public int neq(int colnum) {
			n7p = colnum/2187; n7u = colnum%2187;
			return comtab[c7p,n7p]+comtab[c7u,n7u];
		}
		public void s2n(string col, ref int nx1, ref int nx2) {
			nx1 = s2n(col.Substring(0,7));
			nx2 = s2n(col.Substring(7,7));
		} 
		public void e1c(int nx1, int nx2) {
			c7p = nx1; c7u = nx2;
		}
		public void e1c(int colnum) {
			c7p = colnum/2187; c7u = colnum%2187;
		}
		public string n1s(int nx) {
			string ax = ""; int nx2;
			for (int nr=0; nr<14; nr++) {
				nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}		
		public int s1n(string columna) {
			int nx1=0, nx2=0;
			s2n(columna, ref nx1, ref nx2);
			return (nx1*2187+nx2);
		}
		private string n2s(int nx) {
			string ax = ""; int nx2;
			for (int nr=0; nr<7; nr++) {
				nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}
		private int s2n(string ax) {
			int nx = 0;
			for (int nr=0; nr<ax.Length; nr++) {
				nx *= 3;
				string ch = ax.Substring(nr,1);
				if (ch=="1") nx+=1;
				else if (ch=="2") nx+=2;
			}
			return nx;
		}
		private int neq(string ax1, string ax2) {
			int na = 0;
			for (int nr=0; nr<7; nr++) {
				if (ax1[nr]==ax2[nr]) na++;
			}
			return na;
		}
	}
}
