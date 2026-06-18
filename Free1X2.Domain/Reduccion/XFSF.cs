// Free1X2 · WinUI 3 — WIN3
// created on 20/01/2004 at 23:04
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// 
// modified on 24/06/2004 at 3:03
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
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Reduccion
{
	public class XFSF : Base, IReduccion
	{
		protected byte[,] columnas = new byte[4782969,3];
		private int[] flags = new int[4782969];

		public override void ComienzaReduccion(string archivoEntrada, string sal, int nivelReduccion, int maxCol, int percent)
		{
			//lee columnas fichero columnas madre
			EntradaDeDatos(archivoEntrada);
			//reduce columnas
			Reduce(nivelReduccion, maxCol, percent);
			//grabar columnas reducidas
			GrabacionDeReductoras(sal, nivelReduccion);
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
		
		protected override void Reduce(int nivelReduccion, int maxCol, int percent) {
			//inicializar datos
			int nextcol = noColumnasIniciales*4/10; 		// seleccion de primera columna arbitraria. Prevista mejora.
			int nfin = maxCol;
			if (nfin==0) nfin = noColumnasIniciales;
			Comparador com= new Comparador();
			
			while (true) {
				//permitimos que el programa ejecute los eventos correspondientes
				Free1X2.Abstractions.UiPump.Pump();
				if (salida) break;	
				flags[nextcol] = 999999999;		// marca de columna reductora
				com.byteCol2 [0]=columnas[nextcol,0];
				com.byteCol2 [1]=columnas[nextcol,1];
				com.byteCol2 [2]=columnas[nextcol,2];
				nfin--; noColumnasProcesadas++; noColumnasFinales++; 
				int min = 999999999, ind = 0;
				for (int nr=0; nr<noColumnasIniciales; nr++) {
					if (flags[nr]>999999997) continue;
					int na = 0;
					na=com.CalculaCoincidenciasConCol2 (columnas[nr,0],columnas[nr,1],columnas[nr,2]);
					if (na >=nivelReduccion) {
						flags[nr]=999999998;	// marca de columna reducida
						noColumnasProcesadas++;
					}
					else {
						flags[nr]+=na;
						if (flags[nr]<min) {
							ind = nr; 
							min = flags[nr];
						}
					}
				}
				int ncent = noColumnasProcesadas*100/noColumnasIniciales;
				if (min>999999997 || nfin==0 || ncent>=percent) break;
				nextcol = ind;
			}	
		}
		
		protected override void GrabacionDeReductoras(string archivoSalida, int nivelReduccion)
		{						
			Comparador col =new Comparador();

            IArchivoColumnas comReducCols = new ArchivoColumnasTexto(archivoSalida);
			for (int nr=0; nr<noColumnasIniciales; nr++)
			{
				if (flags[nr]==999999999)
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
