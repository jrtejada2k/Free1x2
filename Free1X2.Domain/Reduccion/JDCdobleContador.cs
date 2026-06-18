// Free1X2 · WinUI 3 — WIN3
// created on 27/06/2004 at 23:04
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
using System.Collections;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Reduccion
{
	public class JDCDobleContador : Base, IReduccion
	{ 
		private ArrayList columnas;
		private BitArray Bits = new BitArray(4782969,false);
		private BitArray Reducida = new BitArray(4782969,false);
		private BitArray MenorDistHamming = new BitArray(4782969,false);
	    private BitArray Reductora = new BitArray(4782969,false);
		private BitArray BitsExternos = new BitArray(4782969,false);
		private int[] flags = new int[4782969];
		private short Profundidad;
		private short nivelProf;
        private int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907, 43046721 };
        private int min;
		private int numExternas;
		private bool AdmiteExternas;
		private int max, Rmin;
		private int indices;
        private int noPartidos;

	    public JDCDobleContador (bool pAdmitirExternas)
		{
			AdmiteExternas=pAdmitirExternas;
		}

        private void InicializarNumeroDePartidos()
        {
            indices = pot[noPartidos];
            Reducida = new BitArray(indices);
            MenorDistHamming = new BitArray(indices);
            new BitArray(indices);
            Bits = new BitArray(indices);
            Reductora = new BitArray(indices);
            BitsExternos = new BitArray(indices);

            flags = new int[indices];
        }

		public override void ComienzaReduccion(string archivoEntrada, string sal, int nivelReduccion, int maxCol, int percent)
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
			//este método utiliza columnas en formato numerico: un nº entre 0 y 3^14-1)
			//inicializar datos
			int nr, nextcol, nv, primera, ultima;

			Profundidad=0;
			numExternas=0;
			
			
			if (maxCol==0) maxCol = noColumnasIniciales;

			//--------calculamos las columnas que reduce cada una-----------------------
			for (nr=0; nr<noColumnasIniciales ; nr++) 
			{	
				if(Reducida [(int)columnas[nr]]==false)
				{
                    CalculaLasQueReduce((int)columnas[nr], 1, (short)(noPartidos - nivelReduccion), 1, false);
				}
				else
				{
                    CalculaLasQueReduce((int)columnas[nr], 1, (short)(noPartidos - nivelReduccion), -1, false);
				}
				noColumnasProcesadas = nr-noColumnasIniciales; // esto es solo para no desanimar al usuario
				Free1X2.Abstractions.UiPump.Pump();
				if (salida) break;	
			}
			
			int[] n = new int[noColumnasIniciales+numExternas]; //nª bordes que reduce cada columna
		    int ncent;

			noColumnasProcesadas=0;
			max=(-1);
			primera=0;
			
			ultima = noColumnasIniciales+numExternas;

			//---------Seleccionamos los reductores-------------------------------------
			while (true) 
			{
				max=0;
				Rmin=100000;
				int Rmayor = 0;
				
				//calculo del valor de reduccion maximo y minimo
				for (nr=primera; nr<noColumnasIniciales; nr++) 
				{
					if(Reducida[(int) columnas[nr]] == false) max=Math.Max (max,flags[(int) columnas[nr]]);
					if(Reducida[(int) columnas[nr]] == false && flags[(int) columnas[nr]] > 1) Rmin=Math.Min (Rmin,flags[(int) columnas[nr]]);
				}
				if (max==0) break;
				//calculamos el numero de bordes que reduce cada columna
				for (nr=primera; nr<ultima; nr++) 
				{
					n[nr]=0;
					
					CalculaNumColumnasPobresQueReduce ((int)columnas[nr], 1, (short)(14-nivelReduccion),ref n[nr]);
					if (flags[(int) columnas[nr]]==max) n[nr]++;
				}
				//permitimos que el programa ejecute los eventos correspondientes
				Free1X2.Abstractions.UiPump.Pump();	
				if (salida) break;
				nextcol=(-1); min=(-1);

			    // seleccion de la maxima reductora que elimina a mas columnas pobres
				for (nr=primera; nr<ultima; nr++) 
				{
					nv=flags[(int)columnas[nr]];
					
					if(Reductora[(int)columnas[nr]]) continue;
					if (n[nr]>min) { min=n[nr]; nextcol=nr; Rmayor=nv;}
					if (n[nr]==min && nv>Rmayor) {nextcol=nr; Rmayor=nv;}
				}
				if (nextcol<0) break;
				max=min;
				
				
				//estrechamos el intervalo para rebajar el tiempo de búsqueda
				while (Reducida[(int)columnas[primera]])
				{
					primera++;
				}
				while (Reducida[(int)columnas[ultima-1]])
				{
					ultima--;
				}

				// marcando columna reductora
				if(Bits[(int)columnas[nextcol]] && Reducida [(int)columnas[nextcol]]==false) noColumnasProcesadas++;
				Reductora[(int)columnas[nextcol]]=true;
				Reducida [(int)columnas[nextcol]]=true;
				noColumnasFinales++;
			
				//marcamos las reducidas y rebajamos las influenciadas por ellas
                MarcarReducidas((int)columnas[nextcol], 1, (short)(noPartidos - nivelReduccion));
							
				ncent = noColumnasProcesadas*100/(noColumnasIniciales);
				if (noColumnasFinales==maxCol || ncent>=percent) break;
			}
						
		}


		private void CalculaNumColumnasPobresQueReduce (int IndiceInicial, short PosicionInicial, short pProfundidad, ref int contador)
		{
			short Partido,z;
			int SigIni, Indice;

			//--encontramos las apuestas que se diferencian en un solo signo ----
			nivelProf++;

            for (Partido = PosicionInicial; Partido < noPartidos + 1; Partido++)
			{
                Free1X2.Abstractions.UiPump.Pump();
				SigIni = ((IndiceInicial / pot[Partido - 1]) % 3);
				for (z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido - 1] * (z - SigIni);
					if (Bits [Indice])
					{
						if (Reducida[Indice]==false && flags[Indice] > 1) 
						{
							if (flags[Indice]==Rmin) contador++;
						}
					}
					if (nivelProf < pProfundidad) CalculaNumColumnasPobresQueReduce (Indice, (short)(Partido + 1), pProfundidad, ref contador);
				}
			}
			nivelProf--;
		}
	
		private void CalculaLasQueReduce (int IndiceInicial, short PosicionInicial, short pProfundidad, int delta, bool PonerHamming)
		{
			short Partido,z;
			int SigIni, Indice;

			//--encontramos las apuestas que se diferencian en un solo signo ----
			nivelProf++;

            for (Partido = PosicionInicial; Partido < noPartidos + 1; Partido++)
			{
				SigIni = ((IndiceInicial / pot[Partido - 1]) % 3);
				for (z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido - 1] * (z - SigIni);

					switch (Bits[Indice])
					{
						case false:
							if (AdmiteExternas)
							{
								if(BitsExternos[Indice]==false)
								{
									numExternas++;
									BitsExternos[Indice]=true;
									columnas.Add (Indice);
								}
								flags[Indice]+=delta;	
							}
							break;
						case true:
							if(Reductora[Indice]==false) flags[Indice]+=delta;
							break;
					}
					if(PonerHamming) MenorDistHamming[Indice]=true;
					if (nivelProf < pProfundidad) CalculaLasQueReduce (Indice, (short)(Partido + 1), pProfundidad, delta, PonerHamming);
				}
			}
			nivelProf--;
		}

		private void MarcarReducidas (int IndiceInicial, short PosicionInicial, short pProfundidad)
		{
			short Partido,z;
			int SigIni, Indice;

			//--encontramos las apuestas que se diferencian en un solo signo ----
			Profundidad++;
            for (Partido = PosicionInicial; Partido < noPartidos; Partido++)
			{
				SigIni = ((IndiceInicial / pot[Partido - 1]) % 3);//signo inicial del partido
				for (z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido - 1] * (z - SigIni);
					if (Reducida [Indice]) flags[Indice]--;

					if (Bits[Indice])
					{						
						if (Reducida [Indice]==false)
						{
							Reducida [Indice]=true;
							MenorDistHamming[Indice]=true;
							flags[Indice]-=2;
							if (Bits[Indice]) noColumnasProcesadas++;
							//disminuimos el contador de las que la reducen
							CalculaLasQueReduce ( Indice, 1,pProfundidad, -1, true);
						}
					}
					if (Profundidad < pProfundidad)
					{MarcarReducidas ( Indice, (short)(Partido + 1), pProfundidad);}
				}
			}
			Profundidad--;
		}

	    protected override void EntradaDeDatos(string archivoEntrada) 
		{
		    columnas = new ArrayList();
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
            noPartidos = comBaseCols.ObtenNumSignos();
            InicializarNumeroDePartidos();
			Bits.SetAll (false);

			ConvertidorDeBases col= new ConvertidorDeBases();

	        noColumnasIniciales = 0;
			while( comBaseCols.SiguienteColumna() )
			{
				int Num = col.ConvColumnaANumero  (comBaseCols.LeeColumnaSinComas());

				flags[Num]=1;	
				columnas.Add(Num);
				Bits[Num]=true;
				
			}
			noColumnasIniciales=columnas.Count;
			comBaseCols.Cerrar();	
			columnas.Sort() ;		
		}
		
		protected override void GrabacionDeReductoras(string archivoSalida, int nivelReduccion)
		{						
			//archivo final de reducidas

			ConvertidorDeBases col =new ConvertidorDeBases();
            IArchivoColumnas comReducCols = new ArchivoColumnasTexto(archivoSalida);

			for (int nr=0; nr<noColumnasIniciales+numExternas; nr++) 
			{
				if (Reductora[(int)columnas[nr]])
				{					
					comReducCols.GuardarCols(  col.ConvNumAColumna((int)columnas[nr]) );
				}
			}		
			comReducCols.Cerrar();	
		}

		public void Inicializa(string entrada, int nivelReduccion)
		{
		}

	}
}

