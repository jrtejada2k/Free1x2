// Free1X2 · WinUI 3 — WIN3
// created on 17/01/2004 at 11:58
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
// Copyright (C) 2008 Morrison - morrison [dot] ne [at] gmail [dot] com
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
using System.Data;
using System.IO;
using Free1X2.EntradaSalida;

namespace Free1X2.Escrutinio
{
	public class Escrutador
	{
		private string archivoCols = "";
		private string[] columnas;
		private int limiteCol;
		private bool pararEscrutinio;
		protected ArrayList listaPremiadas=new ArrayList();
		private ColumnasPremiadas colPremiada;
		private int[] premiosAceptados;
		private int numJornada=1;
		protected bool añadirAGanadoras;
		private int[] premios;
		private int[] premiosTotales;

		private DataSet escrutinioDS;						
		
		public Escrutador(string listaPremios)
		{
			int maxCol=Convert.ToInt32(Math.Pow(3,VariablesGlobales.NumeroPartidos));
			columnas = new string[maxCol];
			premiosAceptados = LeerPremios(listaPremios);
			premiosTotales=new int[VariablesGlobales.NumeroPartidos+1];
		}

		public Escrutador(int[] listaPremios)
		{
			int maxCol=Convert.ToInt32(Math.Pow(3,VariablesGlobales.NumeroPartidos));
			columnas = new string[maxCol];
			premiosAceptados=listaPremios;
			premiosTotales=new int[VariablesGlobales.NumeroPartidos+1];
		}

        public Escrutador()
        {
        }

		public int[] Premios
		{
			get{return premiosAceptados;}
			set{premiosAceptados=value;}
		}

		public int[] PremiosTotales
		{
			get{return premiosTotales;}
			set{premiosTotales=value;}
		}

		protected int[] LeerPremios(string listaPremios)
		{
			string[] arrayPremios=listaPremios.Split(',');
			int[] arrayPremiosInt=new int[arrayPremios.Length];
			for(int i=0;i<arrayPremios.Length;i++)
			{
				arrayPremiosInt[i]=Convert.ToInt16(arrayPremios[i]);
			}
			return arrayPremiosInt;
		}

		public void EscrutaCombConColumna( string columnaGanadora, DataSet escDS )
		{
            //Este método no tiene ninguna referencia
			numJornada=1;
			escrutinioDS = escDS;
			premios = EscrutaColumna( columnaGanadora );
			PonerPremios( premios, 1, columnaGanadora);
		}

		public void EscrutaCombConColumna( string columnaGanadora, DataSet escDS, string archivo)
		{
			numJornada=1;
			escrutinioDS = escDS;
			premios = EscrutaColumna( columnaGanadora );
			PonerPremios( premios, 1, columnaGanadora, archivo);
		}

        public int EscrutaApuestaMultiple(long apuesta, long ganadora)
        {
            long res = (ganadora & apuesta);
            return Utils.UtilColumnas.ContarBitsA1(res);
        }
        public void PararEscrutinio()
		{
			if(!pararEscrutinio)
			{
				pararEscrutinio = true;
			}
		}
		
		public void EscrutaCombConTemporada( string archivoRef, DataSet escDS, string archivo )
		{
			//esta variable pararEscrutinio se puede poner a true desde fuera 
			//de este metodo para parar el escrutinio
			pararEscrutinio = false;
			escrutinioDS = escDS;
			LeeArchivoTemporada( archivoRef );	
			premios = null;
			
			for(int i = 0; i < limiteCol; i++)
			{
				numJornada=i+1;
				if(pararEscrutinio)
				{
					break;
				}
				Free1X2.Abstractions.UiPump.Pump();
				premios = EscrutaColumna( columnas[i] );				
				PonerPremios( premios, i+1, columnas[i], archivo);				
			}
		}
		
		protected void LeeArchivoTemporada( string archivoRef )
		{
		    string combFile = archivoRef;
            IArchivoColumnas archComb = InicializaArchivoCols(combFile);
			
			limiteCol = 0;
			
			while(archComb.SiguienteColumna() ) 
			{
				string columna = archComb.LeeColumnaSinComas();
				string colTst = columna.Replace('x','X');	
				columnas[limiteCol++] = colTst;
			}
			archComb.Cerrar();
		}	
		
		protected int[] EscrutaColumna( string columnaGanadora ) 
		{
			int[]nc = new int[VariablesGlobales.NumeroPartidos+1]; //numero de aciertos por categoria
		    int numCol=0;	// Orden de la columna
            IArchivoColumnas archComb = InicializaArchivoCols(archivoCols);
			columnaGanadora = columnaGanadora.Replace('x','X');
            

			while(archComb.SiguienteColumna() ) 
			{
                int maxSignos;
				numCol++;
				string columna = archComb.LeeColumnaSinComas();
				string colTst = columna.Replace('x','X');	
				int na = 0;
                if (columna.Length <= columnaGanadora.Length)
                {
                    maxSignos = columna.Length;
                }
                else
                {
                    maxSignos = columnaGanadora.Length;
                }
				for (int nr=0; nr<maxSignos; nr++) 
				{
					char ch = columnaGanadora[nr]; 
					char ch2 = colTst[nr];
					if (ch == '*' || ch == ch2)
					{
						na++;
					}
                   
				}

				if(añadirAGanadoras)
				{
					AñadeAGanadoras(colTst, numCol, na);
				}

				nc[na]++;			
			}
			
			archComb.Cerrar();
			return nc;
		}	

		private void AñadeAGanadoras(string col, int numCol, int numAciertos)
		{
			// Si la columna no está en el rango de premios, se vuelve
			if(Array.IndexOf(Premios,numAciertos) >= 0)
			{			
				colPremiada = new ColumnasPremiadas();
				colPremiada.Columna = col;
				colPremiada.Fichero = archivoCols;
				colPremiada.Jornada = numJornada;
				colPremiada.NoColumna = numCol;
				colPremiada.Premio = numAciertos;
				colPremiada.NoBoleto = ObtenBoleto(numCol);
				listaPremiadas.Add(colPremiada);
			}
		}

		protected int ObtenBoleto(int numCol)
		{
			int n=numCol/8;
			if(numCol%8>0) n++;
			return n;
		}

		public void PonerPremios(int[] prems, int noColumna, string columna)
		{
			DataRow row = escrutinioDS.Tables["Resultados"].NewRow();
			row["LineaID"] = noColumna.ToString();
			row["Columna"] = columna;
			row["Seleccionado"] = false;

			for(int i = 1; i < prems.Length; i++)
			{				
				row["P" + i] = prems[i].ToString();
				premiosTotales[i]+=prems[i];
			}	
			escrutinioDS.Tables["Resultados"].Rows.Add( row );
		}
		
		public void PonerPremios(int[] prems, int noColumna, string columna, string archivo)
		{
            int totalAciertos = 0;
			DataRow row = escrutinioDS.Tables["Resultados"].NewRow();
			row["LineaID"] = noColumna.ToString();
			row["Columna"] = columna;
			row["Archivo"] = Path.GetFileName(archivo);
			row["Seleccionado"] = false;
			for(int i = 0; i < prems.Length; i++)
			{				
				row["P" + i] = prems[i].ToString();
				premiosTotales[i]+=prems[i];
                totalAciertos += prems[i] * i;
			}	
            //Aquí hay que obtener el total de aciertos por jornada
            row["Ac. Totales"] = totalAciertos.ToString();
			escrutinioDS.Tables["Resultados"].Rows.Add( row );
		}
		
		public void AñadirPremiosGlobales(int[] premiosGlobales)
		{
			DataRow row = escrutinioDS.Tables["Resultados"].NewRow();
			row["LineaID"] = 0;
			row["Archivo"] = "-------------";
			row["Columna"] = "----- TOTALES:";
			row["Seleccionado"] = false;

			for(int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
			{				
				row["P" + i] = premiosGlobales[i].ToString(); 
			}
            row["Ac. Totales"] = " ";
			escrutinioDS.Tables["Resultados"].Rows.Add( row );
		}

        protected ArchivoColumnasTexto InicializaArchivoCols(string combFile)
		{
            return new ArchivoColumnasTexto(combFile);
		}

		public string ArchivoColumnas
		{
			get{ return archivoCols; }
			set{ archivoCols = value; }		
		}

		public ArrayList ListaPremiadas
		{
			get{ return listaPremiadas;}
			set{ listaPremiadas = value; }		
		}

		public bool AñadirAGanadoras
		{
			get{ return añadirAGanadoras; }
			set{ añadirAGanadoras = value; }
		}
		
	}
}
