// created on 08/02/2004 at 11:18
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Basado en algoritmo original de JLPM
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

using System.Collections;

using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Reduccion
{
	public class JLPM : Base, IReduccion
	{
		protected ArrayList columnasBaseDisponibles;
		private ArrayList colsReduccion;

		private UtilColumnas utilCols = new UtilColumnas();
		
		public override void ComienzaReduccion(string archivoEntrada, string sal, int nivelReduccion, int maxCol, int percent)
		{
			//lee columnas fichero columnas madre
			EntradaDeDatos(archivoEntrada);

			//de momento el codigo solo reduce al 100% 
			Reduce(nivelReduccion, 0, 100);
			
			//grabar columnas reducidas
			GrabacionDeReductoras(sal, nivelReduccion);
		}
		
		protected override void Reduce(int nivelReduccion, int maxCol, int percent)
		{
			//array de almacenamiento columnas finales de la reduccion
			colsReduccion = new ArrayList();
			
			// Variables de trabajo...
		    int acu = 0;
						
			string columna = "";
						
			// Apuestas totales iniciales...
			int ebet = columnasBaseDisponibles.Count;						
			
			string[] ty = (string[])columnasBaseDisponibles.ToArray(columna.GetType());
			
			int[] g0 = new int[ebet]; 
			bool[] g1 = new bool[ebet];
			
			int noCoincidencias = nivelReduccion;
			
			// Averiguamos la apuesta mínima a marcar...
			int ret = QC(ty, ty[0], noCoincidencias);
			
			for(int i = 0; i < ebet; i++)
			{
				g0[i] = ret;
				g1[i] = true;
			}
			
			// Inicio del proceso...
			for(;;)
			{
				//permitimos que el programa ejecute los eventos correspondientes
				Free1X2.Abstractions.UiPump.Pump();
				
				// Obtenemos la apuesta mas alta...
				int res = 0;
				
				for (int i = 0; i < ebet; i++)
				{
					if (g0[i] >= 0 && g0[i] > res)
					{
						res = g0[i];
						ret = i;
					}
				}
				
				//si la apuesta mas alta es 0 salimos terminamos la reduccion...
				if (res == 0)
				{
					break;
				}		
				
				// Con la apuesta mas alta la comparamos para extraer a todas las apuestas que anula...
				for (int i = 0; i < ebet; i++)
				{
					if (g1[i] && utilCols.ObtenNoCoincidencias(ty[i], ty[ret]) >= noCoincidencias)
					{
						for (int j=0; j < ebet; j++)
						{
							if (g0[j] > 0 && utilCols.ObtenNoCoincidencias(ty[i], ty[j]) >= noCoincidencias) 
							{
								g0[j]--;
							}
						}
						
						g1[i] = false;
					}
				}
				
				// Con la mejor apuesta conseguida se almacena y pasamos a la siguiente...
				colsReduccion.Add( ty[ret] );
				noColumnasFinales++;
				
				acu += res;
				
				if (acu == ebet)
				{
					break;
				}			
			}			

		}
		
		protected int QC( string[] combinacion, string columna, int limit )
		{
			int c1 = 0;
			
			for (int i = 0; i < combinacion.Length; i++) 
			{
				if( utilCols.ObtenNoCoincidencias( columna, combinacion[ i ] ) >= limit) 
				{
					c1++;
				}
			}
			
			return c1;
		}	
		
		protected override void EntradaDeDatos(string archivoEntrada) 
		{
		    columnasBaseDisponibles = new ArrayList();

            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			
			//carga todas las columnas en array
			while( comBaseCols.SiguienteColumna() )
			{
			    string columna = comBaseCols.LeeColumnaSinComas();
			    columnasBaseDisponibles.Add( columna );
			}

		    comBaseCols.Cerrar();
			
			noColumnasIniciales =  columnasBaseDisponibles.Count;	
			
		}
		
		protected override void GrabacionDeReductoras(string archivoSalida, int nivelReduccion)
		{
			//archivo final de reducidas			
            IArchivoColumnas comReducCols = new ArchivoColumnasTexto(archivoSalida);
			
			for (int nr = 0; nr < colsReduccion.Count; nr++)
			{
				comReducCols.GuardarCols( (string)colsReduccion[nr] );
			}		
			
			comReducCols.Cerrar();				
		}	

		public void Inicializa(string entrada, int nivelReduccion)
		{
		}

	}
}
