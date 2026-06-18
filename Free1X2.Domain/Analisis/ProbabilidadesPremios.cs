// Free1X2 · WinUI 3 — WIN3
// created on 20/10/2003 at 20:52
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

namespace Free1X2.Analisis
{
	public class ProbabilidadesPremios
	{
		private int noColumnasBase;
		private int noColumnasComb;

        private int[] premios = new int[17];
        private double[] probabilidades = new double[17];
		
		public ProbabilidadesPremios()
		{
			InicializaContadores();		
		}
		
		protected void InicializaContadores()
		{
            for (int i = 0; i < premios.Length; i++)
            {
                premios[i] = 0;
                probabilidades[i] = 0;
            }
		}

        public void AddPremioDirecto(int categoriaPremio, int noPremios)
		{
            premios[categoriaPremio] += noPremios;		
		}		
		
		public void CalculaProbabilidades()
		{
            for (int i = 0; i < probabilidades.Length; i++)
            {
                probabilidades[i] = (premios[i] * 100.0) / noColumnasBase;
            }
		}
		
		public int NoColsCombBase
		{
			get{ return noColumnasBase; }
			set{ noColumnasBase = value; }		
		}
		
		public int NoColsComb
		{
			get{ return noColumnasComb; }
			set{ noColumnasComb = value; }		
		}
		
		public double ObtenProbabilidadPremioDirecto(int categoriaPremio)
		{
            return probabilidades[categoriaPremio];
		}
		
		
	}
}
