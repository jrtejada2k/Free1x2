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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.ExceptionServices;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.Escrutinio
{
	public class Escrutador
	{
		private string archivoCols = "";
		// P-11: antes los dos constructores asignaban un string[3^14] (~38 MB de
		// referencias en el LOH) que solo usa el escrutinio de temporada, y solo
		// hasta `limiteCol` posiciones. Una List<string> crece a lo que haga falta.
		private readonly List<string> columnas = new List<string>();
		private int limiteCol;
		private bool pararEscrutinio;
		protected ArrayList listaPremiadas=new ArrayList();
		private int[] premiosAceptados;
		private int numJornada=1;
		protected bool añadirAGanadoras;
		private int[] premios;
		private int[] premiosTotales;

		private DataSet escrutinioDS;						
		
		public Escrutador(string listaPremios)
		{
			premiosAceptados = LeerPremios(listaPremios);
			premiosTotales=new int[VariablesGlobales.NumeroPartidos+1];
		}

		public Escrutador(int[] listaPremios)
		{
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
		
		/// <param name="porJornada">#5: recibe las premiadas de cada ganadora, que salen de la lista,
		/// para llevarlas a disco (contra 1.000 ganadoras, 14 triples dan 19 millones). Sin él, todo
		/// sigue como antes.</param>
		public void EscrutaCombConTemporada( string archivoRef, DataSet escDS, string archivo, Action<ArrayList> porJornada = null )
		{
			//esta variable pararEscrutinio se puede poner a true desde fuera 
			//de este metodo para parar el escrutinio
			pararEscrutinio = false;
			escrutinioDS = escDS;
			LeeArchivoTemporada( archivoRef );
			premios = null;
			// #9: el fichero de columnas se lee una vez, no una por ganadora (con 1.000, 1.000 lecturas).
			Preparadas cols = null;

			for(int i = 0; i < limiteCol; i++)
			{
				numJornada=i+1;
				if(pararEscrutinio)
				{
					break;
				}
				Free1X2.Abstractions.UiPump.Pump();
				cols ??= LeerColumnas();
				premios = EscrutaColumna( columnas[i], cols );
				PonerPremios( premios, i+1, columnas[i], archivo);				
				porJornada?.Invoke(SacarPremiadas());
			}
		}

		/// <summary>Devuelve las premiadas acumuladas y vacía la lista (#5).</summary>
		public ArrayList SacarPremiadas()
		{
			ArrayList p = listaPremiadas;
			listaPremiadas = new ArrayList();
			return p;
		}
		
		protected void LeeArchivoTemporada( string archivoRef )
		{
		    string combFile = archivoRef;
            IArchivoColumnas archComb = InicializaArchivoCols(combFile);
			
			limiteCol = 0;
			// P-11: la lista se vacía en cada lectura, igual que antes se reescribía
			// el array desde el índice 0 con limiteCol puesto a 0.
			columnas.Clear();
			
			while(archComb.SiguienteColumna() ) 
			{
				string columna = archComb.LeeColumnaSinComas();
				string colTst = columna.Replace('x','X');	
				columnas.Add(colTst);
				limiteCol++;
			}
			archComb.Cerrar();
		}	
		
		protected int[] EscrutaColumna( string columnaGanadora )
		{
			return EscrutaColumna( columnaGanadora, LeerColumnas() );
		}

		/// <summary>
		/// Las columnas del fichero (<c>LeeColumnaSinComas</c> con <c>Replace('x','X')</c>) en un solo bloque:
		/// la i va de Inicios[i] a Inicios[i+1]. Un string por columna eran 4,8 millones de objetos con 14 triples.
		/// ponytail: todo el fichero en memoria (14 triples: ~140 MB); el tope son unos 140 millones de
		/// columnas (2^31 signos). Leer por trozos si alguna vez hace falta más.
		/// </summary>
		private sealed class Preparadas
		{
			public char[] Signos;
			public int[] Inicios;
			public int Count => Inicios.Length - 1;
		}

		private Preparadas LeerColumnas()
		{
			IArchivoColumnas archComb = InicializaArchivoCols(archivoCols);
			var signos = new char[Math.Max(1 << 16, archComb.NumColumnas * 15)];
			int n = 0;
			var inicios = new List<int> { 0 };
			while(archComb.SiguienteColumna() )
			{
				string colTst = archComb.LeeColumnaSinComas().Replace('x','X');
				if (n + colTst.Length > signos.Length) Array.Resize(ref signos, Math.Max(signos.Length * 2, n + colTst.Length));
				colTst.CopyTo(0, signos, n, colTst.Length);
				n += colTst.Length;
				inicios.Add(n);
			}
			archComb.Cerrar();
			return new Preparadas { Signos = signos, Inicios = inicios.ToArray() };
		}

		/// <summary>Lo que cuenta un tramo de columnas.</summary>
		private sealed class Parcial
		{
			public int[] Nc;
			public List<ColumnasPremiadas> Premiadas;
			public Exception Error;
		}

		/// <summary>
		/// #9: cada tramo de columnas cuenta sus aciertos y apunta sus premiadas por su cuenta, en varios
		/// núcleos, y se juntan en orden: lo mismo que en un hilo. Un tramo para en su primer error y el
		/// error salta tras juntar lo de antes, como en un hilo (p. ej. 15 aciertos: fuera de rango).
		/// </summary>
		private int[] EscrutaColumna( string columnaGanadora, Preparadas cols )
		{
			string g = columnaGanadora.Replace('x','X');
			int[] tramos = Paralelo.Tramos(cols.Count);
			var partes = Paralelo.EnParalelo(tramos.Length - 1, k => Contar(tramos[k], tramos[k + 1], cols, g));
			int[]nc = new int[VariablesGlobales.NumeroPartidos+1]; //numero de aciertos por categoria
			foreach (var p in partes)
			{
				for (int i = 0; i < nc.Length; i++) nc[i] += p.Nc[i];
				if (p.Premiadas != null) listaPremiadas.AddRange(p.Premiadas);
				if (p.Error != null) ExceptionDispatchInfo.Capture(p.Error).Throw();
			}
			return nc;
		}

		private Parcial Contar( int desde, int hasta, Preparadas cols, string columnaGanadora )
		{
			var p = new Parcial { Nc = new int[VariablesGlobales.NumeroPartidos+1] };
			if (añadirAGanadoras) p.Premiadas = new List<ColumnasPremiadas>();
			char[] s = cols.Signos;
			try
			{
				for (int i = desde; i < hasta; i++)
				{
					int numCol = i + 1;	// Orden de la columna
					int a = cols.Inicios[i];
					int largo = cols.Inicios[i + 1] - a;
					int maxSignos = largo <= columnaGanadora.Length ? largo : columnaGanadora.Length;
					int na = 0;
					for (int nr=0; nr<maxSignos; nr++)
					{
						char ch = columnaGanadora[nr];
						if (ch == '*' || ch == s[a + nr])
						{
							na++;
						}
					}

					// Si la columna no está en el rango de premios, no se apunta
					if(añadirAGanadoras && Array.IndexOf(Premios,na) >= 0)
					{
						p.Premiadas.Add(new ColumnasPremiadas
						{
							Columna = new string(s, a, largo),
							Fichero = archivoCols,
							Jornada = numJornada,
							NoColumna = numCol,
							Premio = na,
							NoBoleto = ObtenBoleto(numCol),
						});
					}

					p.Nc[na]++;
				}
			}
			catch (Exception e)
			{
				p.Error = e;
			}
			return p;
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
