// Free1X2 · WinUI 3 — WIN3
// created on 27/06/2004 at 23:04
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
// Copyright (C) 2004 xfsf
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections;
using System.Collections.Generic;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Reduccion
{
	public class JDCDobleContador : Base, IReduccion
	{ 
		// P-07: List<int> en vez de ArrayList (ver JDC.cs). Sort() sobre List<int>
		// produce el mismo orden ascendente que ArrayList.Sort() con Comparer.Default:
		// los enteros iguales son indistinguibles, así que la secuencia es idéntica.
		private List<int> columnas;
		// P-11: se asignaban aquí ~38 MB en el LOH (5 BitArray de 4 782 969 +
		// int[4 782 969]) que InicializarNumeroDePartidos() reasigna en cuanto conoce
		// el nº de partidos del fichero. Ahora se crean solo allí.
		private BitArray Bits;
		private BitArray Reducida;
		private BitArray MenorDistHamming;
	    private BitArray Reductora;
		private BitArray BitsExternos;
		private int[] flags;
		// P-21: contador "columnas pobres que reduce cada columna", indexado por VALOR (0..3^n-1),
		// no por posición. Antes se recalculaba para TODAS las columnas en cada pasada del while
		// (O(final × n × vecindario)); ahora se dispersa desde cada columna pobre a su vecindario
		// (relación simétrica → mismo recuento) con coste O(final × pobres × vecindario).
		// 'pobresTocados' lista los valores incrementados, para ponerlos a 0 tras cada pasada sin
		// barrer los 3^n enteros.
		private int[] poorN;
		private System.Collections.Generic.List<int> pobresTocados;
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
            // P-11: aquí había un `new BitArray(indices);` cuyo resultado se descartaba
            // (asignación perdida). Eliminarlo no cambia ningún resultado.
            Bits = new BitArray(indices);
            Reductora = new BitArray(indices);
            BitsExternos = new BitArray(indices);

            flags = new int[indices];
            poorN = new int[indices];
            pobresTocados = new System.Collections.Generic.List<int>();
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
			int nr, nextcol, primera, ultima;

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
			
			// P-21: el contador por posición n[] se sustituye por poorN[] (por valor). //nª bordes que reduce cada columna
		    int ncent;

			noColumnasProcesadas=0;
			max=(-1);
			primera=0;
			
			ultima = noColumnasIniciales+numExternas;

			//---------Seleccionamos los reductores-------------------------------------
			while (true) 
			{
				// #10: las tres pasadas de cada vuelta, en varios núcleos por tramos de 'columnas' (solo
				// leen), con el mismo resultado que en un hilo: el máximo y el mínimo no dependen del
				// orden; las pobres se dispersan en su orden (DispersaColumnaPobre solo suma en poorN), y
				// la elegida es la primera con el mayor (pobres, valor), como el bucle.
				int a=primera, b=ultima;
				int[] tramosA=Paralelo.Tramos(noColumnasIniciales-a);
				int[] tramos=Paralelo.Tramos(b-a);
				int Rmayor = 0;
				
				//calculo del valor de reduccion maximo y minimo
				var extremos=Paralelo.EnParalelo<(int mx, int mn)>(tramosA.Length-1, k =>
				{
					int mx=0, mn=100000;
					for (int i=a+tramosA[k]; i<a+tramosA[k+1]; i++)
					{
						int v=(int)columnas[i];
						if (Reducida[v]==false)
						{
							mx=Math.Max(mx,flags[v]);
							if (flags[v]>1) mn=Math.Min(mn,flags[v]);
						}
					}
					return (mx, mn);
				});
				max=0;
				Rmin=100000;
				foreach (var e in extremos) { max=Math.Max(max,e.mx); Rmin=Math.Min(Rmin,e.mn); }
				if (max==0) break;
				// P-21: dispersa desde cada columna POBRE (flags==Rmin, viva) a su vecindario de
				// Hamming. poorN[v] queda idéntico a n[v] del doble bucle original (vecindad simétrica),
				// con coste O(pobres × vecindario) por pasada en vez de O(n × vecindario).
				// Se buscan en paralelo y se dispersan en un hilo, en su orden.
				int rmin=Rmin;
				var pobres=Paralelo.EnParalelo(tramos.Length-1, k =>
				{
					var p=new List<int>();
					for (int i=a+tramos[k]; i<a+tramos[k+1]; i++)
					{
						int v=(int)columnas[i];
						if (Bits[v] && Reducida[v]==false && flags[v]>1 && flags[v]==rmin) p.Add(v);
					}
					return p;
				});
				foreach (var lista in pobres)
					foreach (int valP in lista) DispersaColumnaPobre(valP, 1, (short)(14-nivelReduccion));
				//permitimos que el programa ejecute los eventos correspondientes
				Free1X2.Abstractions.UiPump.Pump();	
				if (salida) break;

			    // seleccion de la maxima reductora que elimina a mas columnas pobres: en cada tramo, la
				// primera con el mayor (pobres, valor); se juntan en orden con la misma regla (gana la
				// mayor y, en empate, la del tramo anterior).
				int maxV=max;
				var mejores=Paralelo.EnParalelo<(int col, int nnr, int valor)>(tramos.Length-1, k =>
				{
					int col=-1, mn=-1, mayor=0;
					for (int i=a+tramos[k]; i<a+tramos[k+1]; i++)
					{
						int v=(int)columnas[i];
						int f=flags[v];
						// n[nr] original = poorN (pobres que reduce) + 1 si su propio flags==max.
						int nn=poorN[v] + (f==maxV ? 1 : 0);
						if (Reductora[v]) continue;
						if (nn>mn) { mn=nn; col=i; mayor=f; }
						if (nn==mn && f>mayor) { col=i; mayor=f; }
					}
					return (col, mn, mayor);
				});
				nextcol=(-1); min=(-1);
				foreach (var m in mejores)
					if (m.col>=0 && (m.nnr>min || (m.nnr==min && m.valor>Rmayor))) { nextcol=m.col; min=m.nnr; Rmayor=m.valor; }
				// P-21: repone a 0 sólo los valores tocados por la dispersión de esta pasada.
				for (int t=0; t<pobresTocados.Count; t++) poorN[pobresTocados[t]]=0;
				pobresTocados.Clear();
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


		// P-21: dispersa +1 al contador poorN de cada columna del vecindario de Hamming (distancia
		// ≤ 14-nivel) de una columna POBRE. Recorre EXACTAMENTE los mismos índices que el antiguo
		// CalculaNumColumnasPobresQueReduce (mismas posiciones crecientes, misma profundidad), de
		// modo que poorN[v] acaba contando cuántas columnas pobres tienen a v como vecina = el valor
		// que el original calculaba columna a columna. Sin Pump() dentro: era el bucle más caliente
		// (se invocaba por cada columna en cada pasada) y bombeaba la UI miles de millones de veces;
		// el bombeo ya ocurre una vez por pasada en Reduce.
		private void DispersaColumnaPobre(int IndiceInicial, short PosicionInicial, short pProfundidad)
		{
			short Partido, z;
			int SigIni, Indice;

			nivelProf++;
			for (Partido = PosicionInicial; Partido < noPartidos + 1; Partido++)
			{
				SigIni = ((IndiceInicial / pot[Partido - 1]) % 3);
				for (z = 0; z < 3; z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido - 1] * (z - SigIni);
					// Solo la primera vez: con repeticiones la lista llegaba a cientos de millones de entradas.
					// Al empezar cada pasada todo poorN está a 0, así que 0 = aún no tocada.
					if (poorN[Indice] == 0) pobresTocados.Add(Indice);
					poorN[Indice]++;
					if (nivelProf < pProfundidad) DispersaColumnaPobre(Indice, (short)(Partido + 1), pProfundidad);
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
		    columnas = new List<int>();
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

