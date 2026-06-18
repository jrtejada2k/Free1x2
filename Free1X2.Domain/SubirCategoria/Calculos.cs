// Free1X2 · WinUI 3 — WIN3
// created on 23/01/2004 at 15:42
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 xfsf
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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
using Free1X2.Utils ;

namespace Free1X2.SubirCategoria
{
	public class Calculos 
	{
		private int noColumnas;
		private ArrayList columnas;
		private int Profundidad;
		private bool[] _nivel;
		private bool[] _involucrados;
        private BitArray Bits = new BitArray(14348907, false);
        private BitArray ColumnasExternas = new BitArray(14348907, false);
		private BitArray PartidoInvariante = new BitArray(14,false);
        private int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907 };
		private int numInvolucrados;
		private int[] ListaInvolucrados = new int[29];
	    private int ProfMaxima=15;
		private ConvertidorDeBases conv =new ConvertidorDeBases();
        int numPartidos;
		public Calculos()
		{
			Profundidad=0;
		}

		public Calculos (string archivoEntrada) 
		{
		    columnas = new ArrayList();
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
            int noP = comBaseCols.ObtenNumSignos();

			Profundidad=0;

			ConvertidorDeBases col= new ConvertidorDeBases();

		    noColumnas = 0;
			while( comBaseCols.SiguienteColumna() )
			{
			    int Num = col.ConvColumnaANumero  (comBaseCols.LeeColumnaSinComas(),noP);
			    columnas.Add(Num);
			}
		    noColumnas=columnas.Count;
			comBaseCols.Cerrar();
		}

		public void Grabar(string archivoSalida)
		{
            IArchivoColumnas Cols = new ArchivoColumnasTexto(archivoSalida, numPartidos);
			Cols.GuardarTodasCols(Bits);

			Cols.Cerrar();	
		}
		public void SubirUnaCategoria (int IndiceInicial, int PosicionInicial, int pProfundidad)
		{
			int SigIni;
			int Partido;
			int z;
			int Indice;
			Profundidad ++;
			//'--encontramos las columnas que se diferencian en un solo signo ----
			for (Partido = PosicionInicial;Partido<14; Partido++)
			{
				if(_involucrados[Partido]==false) continue;
				SigIni = (IndiceInicial / pot[Partido]) % 3;
				for (z = 0;z<3;z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					if (Profundidad < ProfMaxima)
					{
						SubirUnaCategoria( Indice, Partido + 1, pProfundidad);
					}
					if (_nivel[Profundidad])
					{	
						if (Bits[Indice]==false)
						{
							if(ColumnasExternas[Indice])
							{
								Bits[Indice]=true;
								noColumnas++;
							}
						}
					}
				}
			}
			Profundidad --;
		}

		public void Calcular(bool[] involucrados, bool[] niveles, int seguidos, string FicheroExternas, int noPartidos) 
		{
            PartidoInvariante = new BitArray(noPartidos, false);
            numPartidos = noPartidos;
            conv = new ConvertidorDeBases((byte)noPartidos);
			int i;
		    int n;
		    LeerExternas(FicheroExternas);

			_nivel=niveles;
			_involucrados=involucrados;
		    numInvolucrados=0;
			ProfMaxima=noPartidos + 1;
            //OJO
			for(i=0; i<involucrados.Length;i++)
			{
				if(involucrados[i])	ListaInvolucrados[numInvolucrados++]=i;
			}
			//añadimos los primero al final para hacerlo circular
			for(i=0;i<seguidos+1;i++)
			{
				ListaInvolucrados[numInvolucrados+i]=ListaInvolucrados[i];
			}
			while(_nivel[--ProfMaxima]==false){}
			noColumnas=0;
			Bits.SetAll (false);
			for (i=0; i<columnas.Count ;i++)
			{
				if(_nivel[0]) 
				{
					if (Bits[(int)columnas[i]]==false)
					{
						if(ColumnasExternas[(int)columnas[i]])
						{
							Bits[(int)columnas[i]]=true;
							noColumnas++;
						}
					}
				}
				//Generamos las columnas con los cambios seguidos

			    for (n=0;n<numInvolucrados ;n++)
				{
					PartidoInvariante.SetAll (false);
					PartidoInvariante[ListaInvolucrados [n]]=true;
					SubirUnaCategoriaSeguidos ((int) columnas[i], n, 0, seguidos);
				}
			}	
		}
		private void LeerExternas(string FicheroExternas)
		{
			ColumnasExternas.SetAll (false);
			if(FicheroExternas=="")
			{
				ColumnasExternas.SetAll (true);
			}
			else
			{
                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(FicheroExternas);
			
				while( comBaseCols.SiguienteColumna() )
				{
					ColumnasExternas[conv.ConvColumnaANumero  (comBaseCols.LeeColumnaSinComas())] = true;
				}
				comBaseCols.Cerrar();
			}
		}

		public void SubirUnaCategoriaSeguidos (int IndiceInicial, int InvolucradoInicial, int pProfundidad, int pProfMaxima)
		{
		    int i,z;

		    Profundidad ++;
			i= InvolucradoInicial;
			//'--encontramos las columnas que se diferencian en un solo signo ----
			int Partido = ListaInvolucrados[i];
			PartidoInvariante[Partido]=true;
			int SigIni = (IndiceInicial / pot[Partido]) % 3;
			for (z = 0;z<3;z++)
			{
				if (z == SigIni) continue;
				int Indice = IndiceInicial + pot[Partido] * (z - SigIni);
				if (Profundidad < pProfMaxima)
				{
					SubirUnaCategoriaSeguidos( Indice, i+1, pProfundidad, pProfMaxima);
				}
				else
				{
					SubirUnaCategoriaExceptoSeguidos( Indice, 0, pProfundidad );
				
					if (_nivel[Profundidad])
					{	
						if (Bits[Indice]==false)
						{
							if(ColumnasExternas[Indice])
							{
								Bits[Indice]=true;
								noColumnas++;
							}
						}
					}
				}
			}
			Profundidad --;
		}
		public void SubirUnaCategoriaExceptoSeguidos (int IndiceInicial, int PosicionInicial, int pProfundidad)
		{
		    int Partido;
			int z;
		    Profundidad ++;
			//'--encontramos las columnas que se diferencian en un solo signo ----
			for (Partido = PosicionInicial;Partido<numPartidos; Partido++)
			{
				if(_involucrados[Partido]==false) continue;
				if(PartidoInvariante[Partido]) continue;
				int SigIni = (IndiceInicial / pot[Partido]) % 3;
				for (z = 0;z<3;z++)
				{
					if (z == SigIni) continue;
					int Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					if (Profundidad < ProfMaxima)
					{
						SubirUnaCategoriaExceptoSeguidos( Indice, Partido + 1, pProfundidad);
					}
					if (_nivel[Profundidad])
					{	
						if (Bits[Indice]==false)
						{
							if(ColumnasExternas[Indice])
							{
								Bits[Indice]=true;
								noColumnas++;
							}
						}
					}
				}
			}
			Profundidad --;
		}
		public int NoColumnas 
		{
			get { return noColumnas; }
			set { noColumnas = value; }
		}
	}
}

