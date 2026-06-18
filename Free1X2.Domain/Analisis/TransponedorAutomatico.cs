// Free1X2 · WinUI 3 — WIN3
// created on 06/12/2004 at 14:00
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Joan Duatis - duatis [at] coac [dot] net
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.using System;

using System;

namespace Free1X2.Analisis
{
	/// <summary>
	/// Summary description for Trasponedor.
	/// </summary>
	public class TransponedorAutomatico
	{
		private double[,] valoraciones = new double[14,3];
		private double[,] frecuencias = new double[14,3];
		private double[,] pmediaV = new double[14,2];
		private double[,] pmediaF = new double[14,2];

		public TransponedorAutomatico(double [,] v, double [,] f)
		{
			valoraciones=ValoresBase100(v);
			frecuencias=ValoresBase100(f);
			CalculaPMediaPartidos (valoraciones,ref pmediaV);
			CalculaPMediaPartidos (frecuencias,ref pmediaF);
			OrdenarPMedias (ref pmediaV);
			OrdenarPMedias (ref pmediaF);

		}
		public int [] Trasposicion()
		{
			int[] resultado = new int [14];
			for (int i=0;i<14;i++)
			{
				resultado[(int)pmediaF[i,1]]=(int) pmediaV[i,1];
			}
			return resultado;
		}
		private void CalculaPMediaPartidos (double [,] porcentaje, ref double [,] ProbMedia)
		{
			for (int i =0;i<14;i++)
			{
				ProbMedia[i,0]=Math.Pow (porcentaje[i,0],2)+Math.Pow (porcentaje[i,1],2)+Math.Pow (porcentaje[i,2],2);
				ProbMedia[i,1]=i;
			}
		}
		private double[,] ValoresBase100(double[,] valores)
		{
			double[,] valoresB100 = new double [14,3];
		    for (int i=0;i<14;i++)
		    {
		        double factor = valores[i,0]+valores[i,1]+valores[i,2];
		        for(int j=0;j<3;j++)valoresB100[i,j]=valores[i,j]/factor;
		    }
		    return valoresB100;
		}
		private void OrdenarPMedias( ref double [,] ProbMedia)
		{
		    for (int x=0;x<13;x++)
			{
				for (int y=x+1;y<14;y++)
				{
					if (ProbMedia[y,0]>ProbMedia[x,0])
					{
						double aux = ProbMedia [x,0];
						double aux2 = ProbMedia [x,1];
						ProbMedia [x,0]=ProbMedia [y,0];
						ProbMedia [x,1]=ProbMedia [y,1];
						ProbMedia [y,0]=aux;
						ProbMedia [y,1]=aux2;
					}
				}
			}
		}
	}
}
