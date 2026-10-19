// Free1X2 · WinUI 3 — WIN3
// created on 18/10/2003 at 17:01
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
// Copyright (C) 2003 Joan Duatis - duatis [at] coac [dot] net
// Copyright (C) 2007 Morrison - morrison [dot] ne [at] gmail [dot] com

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
using System.Runtime.ExceptionServices;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Analisis
{
	public class Analizador
	{

        private IArchivoColumnas comBaseCols;
        private IArchivoColumnas comCols;
		
		private ArrayList comColsIter=new ArrayList();
		// P-11: InicializarNumeroDePartidos() reasigna Bits al conocer el nº de
		// partidos de los ficheros comparados; no hace falta el BitArray de 4,78 M aquí.
		private BitArray Bits;
        private int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907, 43046721 };

        private int premioMinimo = 10;

		private ProbabilidadesPremios probabilidadesPremios;
		


        private int[] premios = new int[17];
        private int[] premiosTemp = new int[17];

        private int indices;
        private int noPartidosA;
        private int noPartidosB;

		public Analizador(int minimoPremio)
		{
			probabilidadesPremios = new ProbabilidadesPremios();
            premioMinimo = minimoPremio;
		}
        private void InicializarNumeroDePartidos()
        {
            indices = pot[noPartidosA];
            Bits = new BitArray(indices);
        }
        private void InicializaTemporales()
        {
            // Menor (F3): antes se asignaba un int[17] por CADA columna base comparada.
            // El array es privado y se reescribe por completo, así que limpiarlo in situ
            // da el mismo estado inicial (todo 0).
            Array.Clear(premiosTemp, 0, premiosTemp.Length);
        }
		public void ComparaCombinaciones(string combFileBase, string combFile)
		{
			InicializaArchivosCols( combFileBase, combFile);

			probabilidadesPremios.CalculaProbabilidades();
		}	
		
		protected void InicializaArchivosCols(string combFileBase, string combFile)
		{
            comBaseCols = new ArchivoColumnasTexto(combFileBase);
            comCols = new ArchivoColumnasTexto(combFile);

            noPartidosA = comBaseCols.ObtenNumSignos();
            noPartidosB = comCols.ObtenNumSignos();

            if (noPartidosB != noPartidosA)
            {
                Free1X2.Abstractions.UserDialogs.ShowError("Los Dos archivos tienen un número distinto de signos");
                return;
            }
            InicializarNumeroDePartidos();

			Bits.SetAll (false);

			int noColumnasBase = 0;
			int noColumnasHijas = 0;
			int Num;

			ConvertidorDeBases col= new ConvertidorDeBases();

			
			//marcamos las columnas de la combinacion hija
			while( comCols.SiguienteColumna() )
			{
				Bits[col.ConvColumnaANumero (comCols.LeeColumnaSinComas())]=true;
				noColumnasHijas++;
			}
			// Cerrado al acabar de leer (antes quedaba abierto hasta el finalizador: en Windows, bloqueado).
			comCols.Cerrar();

			
			// #11: la madre se lee y se convierte en orden, en un hilo, como antes: un error de lectura,
			// de conversión o de índice salta en la misma línea, tras sumar lo de las anteriores.
			var nums = new List<int>();
			Exception error = null;
			try
			{
				while( comBaseCols.SiguienteColumna() )
				{
					Num=col.ConvColumnaANumero (comBaseCols.LeeColumnaSinComas());
					_ = Bits[Num];
					nums.Add(Num);
				}
			}
			catch (Exception e)
			{
				error = e;
			}
			comBaseCols.Cerrar();

			// Y cada columna de la madre se busca sola, por tramos en varios núcleos (solo lee Bits).
			// Se suman enteros: da lo mismo en cualquier orden.
			int[] tramos = Paralelo.Tramos(nums.Count);
			var partes = Paralelo.EnParalelo(tramos.Length - 1, k =>
			{
				var v = new Vecindad(this);
				for (int i = tramos[k]; i < tramos[k + 1]; i++) v.Columna(nums[i]);
				return v;
			});
			foreach (var v in partes)
			{
				for (int i = 0; i < premios.Length; i++)
				{
					premios[i] += v.Premios[i];
					if (v.Directos[i] != 0) probabilidadesPremios.AddPremioDirecto(i, v.Directos[i]);
				}
			}
			if (error != null) ExceptionDispatchInfo.Capture(error).Throw();
			noColumnasBase = nums.Count;
			probabilidadesPremios.NoColsComb = noColumnasHijas;
			probabilidadesPremios.NoColsCombBase = noColumnasBase;
		}

		/// <summary>
		/// Lo que cuenta un tramo de columnas de la madre: sus premios, sus premios directos (una vez por
		/// columna y categoría) y el estado de la búsqueda de vecinas, que antes eran campos del Analizador.
		/// </summary>
		private sealed class Vecindad
		{
			private readonly Analizador a;
			public readonly int[] Premios = new int[17];
			public readonly int[] Directos = new int[17];
			private readonly int[] temp = new int[17];
			private short Profundidad;

			public Vecindad(Analizador a) { this.a = a; }

			public void Columna(int Num)
			{
				Array.Clear(temp, 0, temp.Length);
				if (a.Bits[Num])
				{
					ObtenPremios(0);
				}
				else
				{
					Profundidad=0;
					EncontrarDistantes1(Num, 0, a.noPartidosA - a.premioMinimo);
				}
				for (int i = 0; i < temp.Length; i++)
				{
					if (temp[i] != 0) Directos[i]++;
				}
			}

			private void ObtenPremios(int diferencias)
			{
				Premios[a.noPartidosA - diferencias]++;
				temp[a.noPartidosA - diferencias]++;
			}

			private void EncontrarDistantes1(int IndiceInicial, short PosicionInicial,int pProfundidad)
			{
				short Partido,z;
				int SigIni, Indice;
				int[] pot = a.pot;

				//--encontramos las apuestas que se diferencian en un solo signo ----
				Profundidad++;
				for (Partido = PosicionInicial; Partido<a.noPartidosA; Partido++)
				{
					SigIni = ((IndiceInicial / pot[Partido]) % 3);//signo inicial del partido
					for (z = 0; z<3; z++)
					{
						if (z == SigIni) continue;
						Indice = IndiceInicial + pot[Partido] * (z - SigIni);
						if (a.Bits[Indice] )
						{
							ObtenPremios(Profundidad);
						}
						else
						{
							if (Profundidad < pProfundidad)
							{EncontrarDistantes1 ( Indice, (short)(Partido + 1), pProfundidad);}
						}
					}
				}
				Profundidad--;
			}
		}

		protected void IteraCols()
		{
		    int noColumnasBase = 0;
			Comparador com = new Comparador();

		    while( comBaseCols.SiguienteColumna() )
			{
				string columnaComBase = comBaseCols.LeeColumnaSinComas();
				com.col1 =columnaComBase;
                InicializaTemporales();
				
				for(int i = 0; i < comColsIter.Count; i++)
				{
				    byte[] columnaCompara = (byte[]) comColsIter[i];

				    ObtenPremios( com.CalculaDiferencias (columnaCompara ));
				}

			    ObtenPremiosDirectos();
				
				noColumnasBase++;
			}
			
			probabilidadesPremios.NoColsComb = comColsIter.Count;
			probabilidadesPremios.NoColsCombBase = noColumnasBase;			
		}
		
		protected void ObtenPremiosDirectos()
		{
            for (int i = 0; i < premiosTemp.Length; i++)
            {
                if (premiosTemp[i] != 0)
                {
                    probabilidadesPremios.AddPremioDirecto(i, 1);
                }
            }
		}

        protected void ObtenPremios(int diferencias)
		{	

            premios[noPartidosA - diferencias]++;
            premiosTemp[noPartidosA - diferencias]++;
		
		}
		
		public double ObtenProbabilidadPremios(int categoriaPremio)
		{
			return probabilidadesPremios.ObtenProbabilidadPremioDirecto( categoriaPremio );
		}
		
	}
}
