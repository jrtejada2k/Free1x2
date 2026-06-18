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

using System.Collections;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Reduccion
{
	public class Redu1305Xfsf : Base, IReduccion
	{ 
		private BitArray validas = new BitArray(4782969);
	    private int[] flags = new int[4782969];
		private short Profundidad;
        private readonly int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907, 43046721};
		private int numExternas;
		private readonly bool AdmiteExternas;
        private int indices;
        private int noPartidos;
		public Redu1305Xfsf (bool pAdmitirExternas)
		{
			AdmiteExternas=pAdmitirExternas;
		}
        private void InicializarNumeroDePartidos()
        {
            indices = pot[noPartidos];
            validas = new BitArray(indices);
            flags = new int[indices];
        }
		public override void ComienzaReduccion(string archivoEntrada,string sal, int nivelReduccion, int maxCol, int percent)
		{
			//lee columnas fichero columnas madre
			EntradaDeDatos(archivoEntrada);
			//reduce columnas
			if(AdmiteExternas)
			{
				ReduceConExternas(nivelReduccion, maxCol, percent);
			}
			else
			{
				Reduce(nivelReduccion, maxCol, percent);
			}
			//grabar columnas reducidas
			GrabacionDeReductoras(sal, nivelReduccion);
		}

		private void ReduceConExternas(int nivelReduccion, int maxCol, int percent)
		{
			int nsel, primera=0;

			salida = false;
			noColumnasProcesadas=noColumnasFinales=0; 
			if (maxCol==0) maxCol = noColumnasIniciales;

            for (nsel = primera; nsel < indices; nsel++) if (flags[nsel] == 1) break;   // selecció primera
			while (true) 
			{
				Free1X2.Abstractions.UiPump.Pump(); 
				if (salida) break;
				validas[nsel]=true; 
				noColumnasFinales++; 
				flags[nsel]=(-1); 
				noColumnasProcesadas++;
				MarcarReducidas(nsel,0,(short) (VariablesGlobales.NumeroPartidos-nivelReduccion));
                for (nsel = primera; nsel < indices; nsel++) 
				{  	
					if (flags[nsel]==1) {primera=nsel+1; break;}
				}
                if (nsel == indices) break; 
				int ncent = noColumnasProcesadas*100/(noColumnasIniciales);
				if (noColumnasFinales>=maxCol || ncent>=percent) break;

			}
            for (int nr = 0; nr < indices; nr++) 
			{
				if (flags[nr]==2)
				{
					Free1X2.Abstractions.UiPump.Pump(); 
					if (salida) break;
					validas[nr]=true; 
					noColumnasFinales++; 
					flags[nr]=(-1); 
					noColumnasProcesadas++;
					Marcar1(nr,0,(short) (VariablesGlobales.NumeroPartidos -nivelReduccion));	
					int ncent = noColumnasProcesadas*100/(noColumnasIniciales);
					if (noColumnasFinales>=maxCol || ncent>=percent) break;

				}
			}
		}
		protected override void Reduce(int nivelReduccion, int maxCol, int percent)
		{
			int nsel, primera=0;

			salida = false;
			noColumnasProcesadas=noColumnasFinales=0; 
			if (maxCol==0) maxCol = noColumnasIniciales;

            for (nsel = primera; nsel < indices; nsel++) if (flags[nsel] == 1) break;   // selecció primera
			while (true) 
			{
				Free1X2.Abstractions.UiPump.Pump(); 
				if (salida) break;
				validas[nsel]=true; 
				noColumnasFinales++; 
				flags[nsel]=(-1); 
				noColumnasProcesadas++;
				MarcarReducidas(nsel,0,(short) (VariablesGlobales.NumeroPartidos-nivelReduccion));
                for (nsel = primera; nsel < indices; nsel++) 
				{  	
					if (flags[nsel]==1) {primera=nsel+1; break;}
				}
                if (nsel == indices) break; 
				int ncent = noColumnasProcesadas*100/(noColumnasIniciales);
				if (noColumnasFinales>=maxCol || ncent>=percent) break;

			}
            for (int nr = 0; nr < indices; nr++) 
			{
				if (flags[nr]==2)
				{
					Free1X2.Abstractions.UiPump.Pump(); 
					if (salida) break;
					validas[nr]=true; 
					noColumnasFinales++; 
					flags[nr]=(-1); 
					noColumnasProcesadas++;
					Marcar1(nr,0,(short) (VariablesGlobales.NumeroPartidos-nivelReduccion));	
					int ncent = noColumnasProcesadas*100/(noColumnasIniciales);
					if (noColumnasFinales>=maxCol || ncent>=percent) break;

				}
			}
		}
	
		private void MarcarReducidas (int IndiceInicial, short PosicionInicial, short pProfundidad)
		{
			short Partido,z;
			int SigIni, Indice;

			//--encontramos las apuestas que se diferencian en un solo signo ----
			Profundidad++;
            for (Partido = PosicionInicial; Partido < noPartidos; Partido++)
			{
				SigIni = ((IndiceInicial / pot[Partido]) % 3);//signo inicial del partido
				for (z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					//si la nueva columna forma parte de la combinación
					if (flags[Indice]>0) { flags[Indice]=(-1); noColumnasProcesadas++; }
					if (Profundidad < pProfundidad)
					{MarcarReducidas ( Indice, (short)(Partido + 1), pProfundidad);}
					else
					{MarcarInfluenciadas ( Indice, (short)(Partido + 1), (short) (2*pProfundidad));}
				}
			}
			Profundidad--;
		}
		private void Marcar1 (int IndiceInicial, short PosicionInicial, short pProfundidad)
		{
			short Partido,z;
			int SigIni, Indice;

			//--encontramos las apuestas que se diferencian en un solo signo ----
			Profundidad++;
			for (Partido = PosicionInicial; Partido<noPartidos; Partido++)
			{
				SigIni = ((IndiceInicial / pot[Partido]) % 3);//signo inicial del partido
				for (z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					//si la nueva columna forma parte de la combinación
					if (flags[Indice]>0) { flags[Indice]=(-1); noColumnasProcesadas++; }
					if (Profundidad < pProfundidad)
					{Marcar1 ( Indice, (short)(Partido + 1), pProfundidad);}
				}
			}
			Profundidad--;
		}
		private void MarcarInfluenciadas (int IndiceInicial, short PosicionInicial, short pProfundidad)
		{
		    //--encontramos las apuestas que se diferencian en un solo signo ----
			Profundidad++;
            for (short Partido = PosicionInicial; Partido < noPartidos; Partido++)
            {
                int SigIni = ((IndiceInicial / pot[Partido]) % 3);
                for (short z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					int Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					//si la nueva columna forma parte de la combinación
					if (flags[Indice]>0) flags[Indice]=2;
					if (Profundidad < pProfundidad)
					{MarcarInfluenciadas ( Indice, (short)(Partido + 1), pProfundidad);}
				}
            }
		    Profundidad--;
		}

		protected override void EntradaDeDatos(string archivoEntrada) 
		{
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			ConvertidorDeBases col= new ConvertidorDeBases();
            noPartidos = comBaseCols.ObtenNumSignos();
            InicializarNumeroDePartidos();
			noColumnasIniciales = 0;
			
			while( comBaseCols.SiguienteColumna() )
			{
				flags[col.ConvColumnaANumero  (comBaseCols.LeeColumnaSinComas())]=1;	
				noColumnasIniciales++;
			}
			comBaseCols.Cerrar();						
		}
		
		protected override void GrabacionDeReductoras(string archivoSalida, int nivelReduccion)
		{						
			//archivo final de reducidas

			ConvertidorDeBases col =new ConvertidorDeBases();
            IArchivoColumnas comReducCols = new ArchivoColumnasTexto(archivoSalida);

            for (int nr = 0; nr < indices + numExternas; nr++) 
			{if (validas[nr]) comReducCols.GuardarCols(col.ConvNumAColumna(nr));}	
			comReducCols.Cerrar();	
		}

		public void Inicializa(string entrada, int nivelReduccion)
		{
		}
	}
}

