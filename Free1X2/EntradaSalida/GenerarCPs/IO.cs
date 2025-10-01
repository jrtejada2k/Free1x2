using System;
using System.IO;
using System.Text;

namespace Free1X2.EntradaSalida.GenerarCPs
{
	/// <summary>
	/// Descripción breve de IO.
	/// </summary>
	public class IO
	{
		public IO( string fileName )
		{
			nombreArchivo = fileName;					
		}
		
		private string nombreArchivo = "";
		
		private StreamWriter sw;
		private StreamReader sr;
				
		public void GuardarTexto( string texto )
		{
			if( sw == null )
			{
				sw = File.CreateText( nombreArchivo );				
			}
			
			sw.WriteLine( texto);
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

		public string LeeColumna()
		{
		    if( sr == null )
			{
				sr = new StreamReader( nombreArchivo );
			}
			return sr.ReadLine();
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

		public string[] ColumnaAPartidos(string columna)
		{
			string[] matrizColumnas=new string[14];
			int n,n2=-1;
			// Llenamos la matriz
			for(int i=0;i<14;i++)
			{
				n=columna.IndexOf(",",n2+1);
				if(n>=0)
				{
					matrizColumnas[i]=columna.Substring(n2+1,columna.Length-n);
					n2=n;
				}
				else
				{
					if(n2==columna.Length-1)
					{
						matrizColumnas[i]="";
					}
					else
					{
						matrizColumnas[i]=columna.Substring(n2+1,columna.Length-n2-1);
					}
				}
			}
			// Eliminamos los finales de linea
			for(int i=0;i<14;i++)
			{
				n=matrizColumnas[i].IndexOf(",");
				if(n>=0)
				{
					matrizColumnas[i]=matrizColumnas[i].Substring(0,n);
				}
			}
			return matrizColumnas;
		}

		public double ContarValoracion(Valoracion valorPartido, string signo)
		{
			double resultado=0;
			switch(signo)
			{
				case "1":
					resultado=valorPartido.Uno;
					break;
				case "X":
					resultado=valorPartido.Equis;
					break;
				case "2":
					resultado=valorPartido.Dos;
					break;
			}
			return resultado;
		}

		public Valoracion LeeLineaValoracion()
		{
			Valoracion valor = new Valoracion();
		    string separador=", ";
		    if( sr == null )
			{
				sr = new StreamReader( nombreArchivo );
			}
			string linea = sr.ReadLine();
			int n = linea.IndexOf(separador);
			if(n==-1)
			{
				separador=",";
				n=linea.IndexOf(separador);
			}
			string numero = linea.Substring(0,n);
		    numero.Replace(".",",");
			valor.Uno=Convert.ToDouble(numero);
			int n2 = linea.IndexOf(separador,n+1);
			numero=linea.Substring(n+2,n2-n-2);
			numero.Replace(".",",");
			valor.Equis=Convert.ToDouble(numero);
			numero.Replace(".",",");
			numero=linea.Substring(n2+2,linea.Length-n2-2);
			valor.Dos=Convert.ToDouble(numero);
			return valor;
		}

		~ IO() 
		{
			Cerrar();
		}

	}
}
