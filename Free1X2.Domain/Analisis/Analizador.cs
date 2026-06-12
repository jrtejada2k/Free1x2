// created on 18/10/2003 at 17:01
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2003 Joan Duatis - duatis@coac.net
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com

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

namespace Free1X2.Analisis
{
	public class Analizador
	{

        private IArchivoColumnas comBaseCols;
        private IArchivoColumnas comCols;
		
		private ArrayList comColsIter=new ArrayList();
		private BitArray Bits = new BitArray(4782969,false);
        private int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907, 43046721 };

        private int premioMinimo = 10;
		private short Profundidad;

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
            premiosTemp = new int[17];
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

			
			while( comBaseCols.SiguienteColumna() )
			{
                InicializaTemporales();
	
				Num=col.ConvColumnaANumero (comBaseCols.LeeColumnaSinComas());
				if (Bits[Num])
				{
					ObtenPremios(0);
				}
				else
				{
					Profundidad=0;
					EncontrarDistantes1(Num, 0, noPartidosA - premioMinimo);
				}
				ObtenPremiosDirectos();
				noColumnasBase++;
			}
			probabilidadesPremios.NoColsComb = noColumnasHijas;
			probabilidadesPremios.NoColsCombBase = noColumnasBase;			
		}

		private void EncontrarDistantes1(int IndiceInicial, short PosicionInicial,int pProfundidad)
		{
			short Partido,z;
			int SigIni, Indice;

			//--encontramos las apuestas que se diferencian en un solo signo ----
			Profundidad++;
			for (Partido = PosicionInicial; Partido<noPartidosA; Partido++)
			{
				SigIni = ((IndiceInicial / pot[Partido]) % 3);//signo inicial del partido
				for (z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					if (Bits[Indice] ) 
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
