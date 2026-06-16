// created on 17/01/2004 at 11:58
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2006 Toni Moreno
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
using System.Data;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.MotorCalculo;

namespace Free1X2.Escrutinio
{
	public class EscrutadorComb
	{
		private string archivoCols = "";
		private bool pararEscrutinio;
		protected ArrayList listaPremiadas=new ArrayList();
		private int[] premiosAceptados;
		protected bool añadirAGanadoras;
		private int[] premiosTotales;
		private int[] premiosCombinacion;
		private ListaPremiadasComb[] listaColumnasPremiadas;
		private ArrayList listaEscrutadasConPremio;
		private DataSet escrutinioDS;
		private byte profundidad;
		private Hashtable aciertos = new Hashtable();
		int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };

	    public EscrutadorComb(string listaPremios)
		{
			premiosAceptados = LeerPremios(listaPremios);
			premiosTotales=new int[premiosAceptados.Length];
			premiosCombinacion=new int[premiosAceptados.Length];
		}

		public EscrutadorComb(int[] listaPremios)
		{
			premiosAceptados=listaPremios;
			premiosTotales=new int[premiosAceptados.Length];
			premiosCombinacion=new int[premiosAceptados.Length];
		}

		public ArrayList ListaEscrutadasConPremio
		{
			get{return listaEscrutadasConPremio;}
			set{listaEscrutadasConPremio=value;}
		}

		public ListaPremiadasComb[] ListaColumnasPremiadas
		{
			get{return listaColumnasPremiadas;}
			set{listaColumnasPremiadas=value;}
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

		public int[] PremiosCombinacion
		{
			get{return premiosCombinacion;}
			set{premiosCombinacion=value;}
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

		public void EscrutarCombinacion(int jornada)
		{
		    Analizador analizador=new Analizador();
			ArchivoCombinacion archComb = new ArchivoCombinacion();
			archComb.AbrirArchivoCombinacion( archivoCols );
			archComb.CargaControladorGrupos( analizador.CtrlGrupos );
			string[] miListaPronosticos = archComb.LeePronosticos();
		    for(int i=0;i<miListaPronosticos.Length;i++)
		    {
		        string tmp = miListaPronosticos[i];
		        miListaPronosticos[i]=tmp.Replace(",","");
		    }
		    Analisis.AnalizadorColumnaCombinacion a=new Analisis.AnalizadorColumnaCombinacion();
		    long[] columnasTmp=new long[aciertos.Count];
            aciertos.Keys.CopyTo(columnasTmp,0);
		    for (int i = 0; i < columnasTmp.Length; i++)
			{
			    bool columnaOk = a.AnalizaColumna(columnasTmp[i], analizador, miListaPronosticos);
			    if (columnaOk)
				{
                    int premio = (int)aciertos[columnasTmp[i]];
				    int orden = Array.IndexOf(premiosAceptados, premio);
					if (orden >= 0) premiosTotales[orden]++;
					if (añadirAGanadoras)
					{
						ColumnasPremiadasComb cmb = new ColumnasPremiadasComb ();
                        cmb.Columna = columnasTmp[i];
						cmb.Fichero = archivoCols;
						cmb.Jornada = jornada;
                        cmb.Premio = premio;
						listaEscrutadasConPremio.Add ( cmb );
					}
				}
			}
		}

		public void PararEscrutinio ()
		{
			if(!pararEscrutinio)
			{
				pararEscrutinio = true;
			}
		}


		public void PonerPremios(int[] premios, int noColumna, string columna)
		{
			DataRow row = escrutinioDS.Tables["Resultados"].NewRow();
			row["LineaID"] = noColumna.ToString();
			row["Columna"] = columna;
			row["Seleccionado"] = false;

			for(int i = 1; i < premios.Length; i++)
			{
				row["P" + i] = premios[i].ToString();
				premiosTotales[i]+=premios[i];
			}
			escrutinioDS.Tables["Resultados"].Rows.Add( row );
		}

		public void PonerPremios(int[] premios, int noColumna, string columna, string archivo)
		{
			DataRow row = escrutinioDS.Tables["Resultados"].NewRow();
			row["LineaID"] = noColumna.ToString();
			row["Columna"] = columna;
			row["Archivo"] = archivo;
			row["Seleccionado"] = false;
			for(int i = 0; i < premios.Length; i++)
			{
				row["P" + i] = premios[i].ToString();
				premiosTotales[i]+=premios[i];
			}
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
			escrutinioDS.Tables["Resultados"].Rows.Add( row );
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

		private void columnasCon1SignoDiferente ( long indiceInicial, int posicionInicial, byte ultimoPremio, int premioActual )
		{
		    //'--encontramos las columnas que se diferencian en un solo signo ----
			for (int partido = posicionInicial; partido < VariablesGlobales.NumeroPartidos; partido++)
			{
			    long sigIni = (indiceInicial / pot[partido]) % 3;
			    for (int z = 0; z < 3; z++)
				{
					if (z == sigIni) continue;
					int indice = (int)(indiceInicial + pot[partido] * (z - sigIni));
                    if (aciertos.ContainsKey(indice))
                    {
                        if ((int)aciertos[indice] < premioActual) aciertos[indice] = premioActual;
                    }
                    else
                        aciertos.Add(indice, premioActual);
                    if (premioActual > ultimoPremio)
					{
                        columnasCon1SignoDiferente(indice, partido + 1, ultimoPremio, premioActual-1);
					}
				}
			}
		}
		public void ObtenerPosiblesPremios ( string columna, int[] categorias )
		{
			// Pone a 0 los contadores
			aciertos = new Hashtable();
			ConvertidorDeBases c = new ConvertidorDeBases ();
			Array.Sort ( categorias );
			Array.Reverse ( categorias );
			profundidad = (byte)categorias[0];
			long col = c.ConvColumnaANumero ( columna );
			if (categorias[0] == VariablesGlobales.NumeroPartidos)
			{
				aciertos[col] = (byte)categorias[0];
				profundidad--;
			}
			columnasCon1SignoDiferente ( col, 0, (byte)categorias[categorias.Length - 1], profundidad );
		}
	}
}
