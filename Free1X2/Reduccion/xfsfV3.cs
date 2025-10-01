// created on 20/01/2004 at 23:04
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 xfsf
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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

using System;
using System.Windows.Forms;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Reduccion
{
	public class XfsfV3 : Base, IReduccion
	{ 
		protected byte[,] columnas = new byte[4782969,3];
		private int[] flags = new int[4782969];
		
		public override void ComienzaReduccion(string archivoEntrada,string sal, int nivelReduccion, int maxCol, int percent)
		{
			//lee columnas fichero columnas madre
			EntradaDeDatos(archivoEntrada);
			//reduce columnas
			Reduce(nivelReduccion, maxCol, percent);
			//grabar columnas reducidas
			GrabacionDeReductoras(sal, nivelReduccion);
		}

		protected override void Reduce(int nivelReduccion, int maxCol, int percent)
		{
			int nr, nr3, nextcol, nv, min;
			Comparador col = new Comparador ();
			//Comparador com = new Comparador ();
			//inicializar datos
			if (maxCol==0) maxCol = noColumnasIniciales;
			for (nr=0; nr<noColumnasIniciales; nr++) 
			{
				flags[nr]++;
				col.byteCol1 [0]=columnas[nr,0];
				col.byteCol1 [1]=columnas[nr,1];
				col.byteCol1 [2]=columnas[nr,2];
				for (nr3=nr+1; nr3<noColumnasIniciales; nr3++) 
				{
					col.byteCol2 [0]=columnas[nr3,0];
					col.byteCol2 [1]=columnas[nr3,1];
					col.byteCol2 [2]=columnas[nr3,2];

					if (col.CalculaCoincidencias ()>=nivelReduccion)
					{
						flags[nr]++;
						flags[nr3]++;
					}
				}
				 
				noColumnasProcesadas = nr-noColumnasIniciales; // esto es solo para no desanimar al usuario
				Application.DoEvents();	
				if (salida) break;
			}
			noColumnasProcesadas=0;
			while (true) 
			{
				//permitimos que el programa ejecute los eventos correspondientes
				Application.DoEvents();	
				if (salida) break;
				nextcol=(-1); min=(-1);	nv=(-1);		// seleccion de la maxima reductora. 0 es posible
				for (nr=0; nr<noColumnasIniciales; nr++) 
				{
					nv=flags[nr];
					if (nv>20000000) continue;
					if (nv>min) { min=nv; nextcol=nr; }
				}
				if (nextcol<0) break;
				col.byteCol1 [0]=columnas[nextcol,0];
				col.byteCol1 [1]=columnas[nextcol,1];
				col.byteCol1 [2]=columnas[nextcol,2];
				flags[nextcol] = 40000000;	// marcando columna reductora
				noColumnasFinales++; noColumnasProcesadas++;
				for (nr=0; nr<noColumnasIniciales; nr++) 
				{	// busca las reducidas
					if (flags[nr]>20000000) continue;

					col.byteCol2 [0]=columnas[nr,0];
					col.byteCol2 [1]=columnas[nr,1];
					col.byteCol2 [2]=columnas[nr,2];

					if (col.CalculaCoincidencias ()>=nivelReduccion) 
					{	
						flags[nr] = 30000000;					// marcando columna reducida
						noColumnasProcesadas++;
						for (nr3=0; nr3<noColumnasIniciales; nr3++) 
						{	// disminuye las influenciadas
							if (flags[nr3]>20000000) continue;
							if (col.CalculaCoincidenciasConCol2 (columnas[nr3,0],columnas[nr3,1],columnas[nr3,2]) >=nivelReduccion) if (flags[nr3]>0) flags[nr3]--;
						}
					}
				}
				int ncent = noColumnasProcesadas*100/noColumnasIniciales;
				if (noColumnasFinales==maxCol || ncent>=percent) break;
			}
		}

		protected override void EntradaDeDatos(string archivoEntrada) 
		{
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			Comparador col= new Comparador();
			noColumnasIniciales = 0;
			while( comBaseCols.SiguienteColumna() )
			{
				col.ConvColumnaA3Bytes (comBaseCols.LeeColumnaSinComas(),ref columnas[noColumnasIniciales,0],ref columnas[noColumnasIniciales,1],ref columnas[noColumnasIniciales,2]);
				flags[noColumnasIniciales++]=0;	
				
			}
			comBaseCols.Cerrar();						
		}
		
		protected override void GrabacionDeReductoras(string archivoSalida, int nivelReduccion)
		{						
			//archivo final de reducidas
			Comparador col =new Comparador();
            IArchivoColumnas comReducCols = new ArchivoColumnasTexto(archivoSalida);
			for (int nr=0; nr<noColumnasIniciales; nr++)
			{
				if (flags[nr]>30000000)
				{					
					comReducCols.GuardarCols( col.Conv3BytesAColumna(columnas[nr,0],columnas[nr,1],columnas[nr,2]) );
				}
			}		
			comReducCols.Cerrar();	
		}

		public void Inicializa(string entrada, int nivelReduccion)
		{
		}
	}
}

