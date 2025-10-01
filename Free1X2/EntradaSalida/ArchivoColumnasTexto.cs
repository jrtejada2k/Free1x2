// created on 25/08/2003 at 18:00
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
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
using System.IO;
using System.Text;
using Free1X2.Utils;

namespace Free1X2.EntradaSalida
{
	public class ArchivoColumnasTexto : IArchivoColumnas
	{
		
		private string nombreArchivo = "";

		private StreamWriter sw;
		private StreamReader sr;
		int numPartidos;
		private int numColumnas;

		public int NumColumnas
		{
			get{return numColumnas;}
		}
        public int NumPartidos
        {
            set { numPartidos = value; }
        }
		public ArchivoColumnasTexto( string fileName )
		{
			nombreArchivo = fileName;
			numColumnas=(int)ObtenNumCols();		
		}
        public ArchivoColumnasTexto(string fileName, int noPartidos)
        {
            NumPartidos = noPartidos;
            nombreArchivo = fileName;
            numColumnas = (int)ObtenNumCols();
        }
		public void GuardarCols( string columna )
		{
			if( sw == null )
			{
				numColumnas = 0;
				sw = File.CreateText( nombreArchivo );				
			}

			numColumnas++;			
			sw.WriteLine( columna.Replace(",", "") );
		}	
		
		public void GuardarCols( int columna )
		{
			ConvertidorDeBases conv=new ConvertidorDeBases();
			string colTexto=conv.ConvNumAColumna(columna);
			GuardarCols(colTexto);
		}
        public void GuardarCols(long columna)
        {
            //Implementar
        }
		public void GuardarColsComa( string columna )
		{
			if( sw == null )
			{
				numColumnas = 0;
				sw = File.CreateText( nombreArchivo );				
			}

			numColumnas++;
			sw.WriteLine( columna);
		}	
		
		public void GuardarTodasCols(string[] columnas)
		{
			GuardarTodasCols(columnas, false);
		}

		public void GuardarTodasCols(int[] columnas)
		{
			GuardarTodasCols(columnas, false);
		}

		public void GuardarTodasCols(BitArray columnas)
		{
			GuardarTodasCols(columnas, false);
		}

		public void GuardarTodasCols(string[] columnas, bool conComa)
		{
			if(conComa)
			{
				for(int i=0; i<columnas.Length; i++)
				{
					GuardarColsComa(columnas[i]);
				}
			}
			else
			{
				for(int i=0; i<columnas.Length; i++)
				{
					GuardarCols(columnas[i]);
				}
			}
		}

		public void GuardarTodasCols(int[] columnas, bool conComa)
		{
			ConvertidorDeBases conv=new ConvertidorDeBases();
			if(conComa)
			{
				for(int i=0; i<columnas.Length; i++)
				{
					GuardarColsComa(conv.ConvNumAColumna(columnas[i]));
				}
			}
			else
			{
				for(int i=0; i<columnas.Length; i++)
				{
					GuardarCols(conv.ConvNumAColumna(columnas[i]));
				}
			}
		}

        public void GuardarTodasCols(BitArray columnas, bool conComa)
        {
            ConvertidorDeBases conv;
            if (numPartidos != 0)
            {
                conv = new ConvertidorDeBases((byte)numPartidos);
            }
            else
            {
                conv = new ConvertidorDeBases();
            }
            if (conComa)
            {
                for (int i = 0; i < columnas.Length; i++)
                {
                    if (columnas[i]) GuardarColsComa(conv.ConvNumAColumna(i));
                }
            }
            else
            {
                for (int i = 0; i < columnas.Length; i++)
                {
                    if (columnas[i]) GuardarCols(conv.ConvNumAColumna(i));
                }
            }
        }

		public bool SiguienteColumna()
		{
			bool tieneSiguiente = false;
			
			if( sr == null )
			{
				sr = new StreamReader( nombreArchivo );
			}
			
			if( sr.Peek() >= 0 )
			{
				tieneSiguiente = true;
			}
			
			return tieneSiguiente;		
		}
		
		public string LeeColumna()
		{
		    string columna = sr.ReadLine();
			
			if( !columna.Equals("") )
			{
				columna = DarFormatoColumna( columna );
			}			
			
			return columna.ToUpper();
		}
		
		public string LeeColumnaSinComas()
		{
		    return sr.ReadLine().Trim().ToUpper();
		}

	    public string[] LeerTodasCols(bool conFormato)
		{
            List<string> lista = new List<string>();
			while(SiguienteColumna())
			{
				lista.Add(LeeColumnaSinComas());
			}
			string[] columnas=new string[lista.Count];
			if(conFormato)
			{
				for(int i=0; i<lista.Count; i++)
				{
					columnas[i]=DarFormatoColumna(lista[i]);
				}
			}
			else
			{
				for(int i=0; i<lista.Count; i++)
				{
					columnas[i]=lista[i];
				}
			}
			return columnas;
		}
        public string[] LeerTodasColsSeparadasPorComas()
        {
            List<string> lista = new List<string>();
            while (SiguienteColumna())
            {
                lista.Add(LeeColumnaSinComas());
            }
            string[] columnas = new string[lista.Count];
            for (int i = 0; i < lista.Count; i++)
            {
                columnas[i] = lista[i].Replace(",", "");
            }

            return columnas;
        }

		public int[] LeerTodasColsANumero()
		{
			ConvertidorDeBases conv=new ConvertidorDeBases();
            List<string> lista = new List<string>();
            
			while(SiguienteColumna())
			{
				lista.Add(LeeColumnaSinComas());
			}
			int[] columnas=new int[lista.Count];
			for(int i=0; i<lista.Count; i++)
			{
				columnas[i]=conv.ConvColumnaANumero(lista[i]);
			}
			return columnas;
		}

		public BitArray LeerTodasColsABitArray(int numeroPartidos)
		{
			ConvertidorDeBases conv=new ConvertidorDeBases();
			int max=Convert.ToInt32(Math.Pow(3, numeroPartidos));
			BitArray matriz=new BitArray(max,false);
		    while(SiguienteColumna())
			{
				string columna = LeeColumnaSinComas();
				int num = conv.ConvColumnaANumero(columna, numeroPartidos);
				matriz[num]=true;
			}
			return matriz;
		}

		public string DarFormatoColumna(string lineaArchivo)
		{
			StringBuilder columnaBuilder = new StringBuilder();
			
			if(lineaArchivo.IndexOf(',') == -1)
			{
				//add commas to line
				
				for(int i = 0; i < lineaArchivo.Length ;i++)
				{
					if( i == 0)
					{
						columnaBuilder.Append( lineaArchivo[i] );
					}
					else
					{
						columnaBuilder.Append( "," + lineaArchivo[i] );
					}				
				}		
			}
		
			return columnaBuilder.ToString();
		}

		public long ObtenNumCols()
		{
			// Obtenemos la longitud de la columna para conocer las apuestas del filtro
		    long longitudColumnas=0;
			
			FileInfo archivo=new FileInfo(nombreArchivo);
			
			if(archivo.Exists)
			{
				if(SiguienteColumna())
				{
					string temp = LeeColumnaSinComas();
					int longitudLinea = temp.Length+2;
				
					longitudColumnas=(archivo.Length/longitudLinea);
				}
				// Reinicia el streamReader ya que hemos leído la 1ª columna
				sr.Close();
				sr = null;

				//no hace falta reabrirlo, se abrira solo en caso de hacer falta asi que simplemente
				//cerramos el streamreader en la linea anterior
				//sr = new StreamReader( nombreArchivo );
			}
			
			return longitudColumnas;
		}

        public int ObtenNumSignos()
        {
            // Obtenemos la longitud de la columna para conocer las apuestas del filtro
            int longitudLinea = 0;

            FileInfo archivo = new FileInfo(nombreArchivo);

            if (archivo.Exists)
            {
                if (SiguienteColumna())
                {
                    string temp = LeeColumnaSinComas();
                    longitudLinea = temp.Length;
                }
                // Reinicia el streamReader ya que hemos leído la 1ª columna
                sr.Close();
                sr = null;

                //no hace falta reabrirlo, se abrira solo en caso de hacer falta asi que simplemente
                //cerramos el streamreader en la linea anterior
                //sr = new StreamReader( nombreArchivo );
            }

            return longitudLinea;
        }


		public void Cerrar()
		{
			if( sw != null )
			{
				sw.Close();
			}
			
			if(sr != null)
			{
				sr.Close();
			}
			
		}
		
		//destructor
		~ ArchivoColumnasTexto() 
		{
		   Cerrar();
		}

		
	}	
}
