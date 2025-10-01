// created on 15/07/2004 at 20:30
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Collections;
using Free1X2.EntradaSalida;

namespace Free1X2.Utils
{
	/// <summary>
	/// Summary description for Porcentajes.
	/// </summary>
	public class Porcentajes
	{
        public double[,] valores = new double[VariablesGlobales.NumeroPartidos, 3];
		private string _Temporada="";
		private string _Jornada="";
		public bool JornadaEncontrada=true;
		public short FormatoFichero;
		private char sep=' ';
		private string[] aux;
		private string lineatexto;
		private ArrayList Jornadas;
		private	string[] ValorsJornada;		
		private string mNombreFichero;
		private ArrayList _DescripcionValoracion;
		private string _DescripcionBuscada="";

		public Porcentajes()
		{

		}
		public Porcentajes( double[,] p)
		{
			valores=p;
		}
		public Porcentajes( string NombreFichero)
		{
			mNombreFichero=NombreFichero;
			Leer ();
		}
		public string Jornada
		{
			set{_Jornada = value;}
		}
		public string Temporada
		{

			set{_Temporada = value;}
		}
		public string NombreFichero
		{
			set{mNombreFichero = value;}
		}
		public ArrayList DescripcionValoracion
		{
			get{return _DescripcionValoracion;}
		}
		public string DescripcionBuscada
		{
			set{_DescripcionBuscada = value;}
		}
		
		public void Leer( )
		{
			StreamReader srv = new StreamReader(mNombreFichero);
			FormatoFichero=TestFichero (srv);
			srv.Close();
			srv = new StreamReader(mNombreFichero);

			switch (FormatoFichero)
			{
				case 1: LeeUnValorPorFila(srv);break;
				case 3: LeeTresValoresPorFila(srv); break;
				case 45:
				case 42: Lee42ValoresPorFila(srv); break;
				case 46:
				case 43: JornadaEncontrada=Lee43ValoresPorFila(srv);break;
				case 47:
				case 44: JornadaEncontrada=Lee44ValoresPorFila(srv);break;
				default: MessageBox.Show ("Fichero Incorrecto"); break;
			}

		}
		private void LeeUnValorPorFila(StreamReader srv)
		{
		    lineatexto = null;
			try
			{
                for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
				{
					valores[i,0] = Convert.ToDouble(srv.ReadLine().Replace(".",","));
					valores[i,1] = Convert.ToDouble(srv.ReadLine().Replace(".",","));
					valores[i,2] = Convert.ToDouble(srv.ReadLine().Replace(".",","));
				}
			}
			catch 
			{
				MessageBox.Show("El fichero seleccionado no es de valoraciones o está corrupto","Error fichero incorrecto",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation );
			}
			srv.Close();
		}

		private void LeeTresValoresPorFila(StreamReader srv)
		{
		    int i = 0;
			while(srv.Peek() > -1)
			{
			    lineatexto=LeerLinia(srv);
				string[] partes = lineatexto.Split(sep);
				if (partes.Length ==3)
				{
					try
					{
						valores[i,0] = Convert.ToDouble(partes[0].Replace (".",",").Trim());
						valores[i,1] = Convert.ToDouble(partes[1].Replace (".",",").Trim());
						valores[i,2] = Convert.ToDouble(partes[2].Replace (".",",").Trim());
					}
					catch
					{
                        if (i != VariablesGlobales.NumeroPartidos)
						{
							MessageBox.Show("El fichero seleccionado no es de valoraciones o está corrupto","Error fichero incorrecto",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation );
							break;
						}
					}
				}
				i++;
			}
			srv.Close();
		}
		private void Lee42ValoresPorFila(StreamReader srv)
		{
			int indice=0;
			while(srv.Peek() > -1)
			{
			    lineatexto=LeerLinia(srv);
				string[] partes = lineatexto.Split(sep);
				if (partes.Length ==42 || partes.Length ==45)
				{
					try
					{
						for(int i=0;i<42;i+=3)
						{
							valores[indice,0] = Convert.ToDouble(partes[i].Replace (".",",").Trim());
							valores[indice,1] = Convert.ToDouble(partes[1+i].Replace (".",",").Trim());
							valores[indice,2] = Convert.ToDouble(partes[2+i].Replace (".",",").Trim());
							indice++;
						}
					}
					catch
					{
						MessageBox.Show("El fichero seleccionado no es de valoraciones o está corrupto","Error fichero incorrecto",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation );
						break;
					}
				}
			}
			srv.Close();
		}
		private bool Lee43ValoresPorFila(StreamReader srv)
		{
		    bool encontrado=false;
			int indice=0;
			_DescripcionValoracion= new ArrayList();
			while(srv.Peek() > -1)
			{
			    lineatexto=LeerLinia(srv);
				string[] partes = lineatexto.Split(sep);
				if (partes.Length ==43 || partes.Length ==46)
				{
					_DescripcionValoracion.Add (partes[0]);
					try
					{
						if(partes[0]==_DescripcionBuscada)
						{
							for(int i=1;i<43;i+=3)
							{
								valores[indice,0] = Convert.ToDouble(partes[i].Replace (".",",").Trim());
								valores[indice,1] = Convert.ToDouble(partes[1+i].Replace (".",",").Trim());
								valores[indice,2] = Convert.ToDouble(partes[2+i].Replace (".",",").Trim());
								indice++;
							}
							encontrado=true;
						}
					}
					catch
					{
						MessageBox.Show("El fichero seleccionado no es de valoraciones o está corrupto","Error fichero incorrecto",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation );
						break;
					}
				}
			}
			srv.Close();
			return encontrado;
		}
		private bool Lee44ValoresPorFila(StreamReader srv)
		{
		    bool encontrado=false;

			int indice=0;
			while(srv.Peek() > -1)
			{
			    lineatexto=LeerLinia(srv);
				string[] partes = lineatexto.Split(sep);
				if (partes.Length ==44 || partes.Length ==47)
				{
					try
					{
						if(partes[0]==_Temporada && partes[1]==_Jornada)
						{
							for(int i=2;i<44;i+=3)
							{
								valores[indice,0] = Convert.ToDouble(partes[i].Replace (".",",").Trim());
								valores[indice,1] = Convert.ToDouble(partes[1+i].Replace (".",",").Trim());
								valores[indice,2] = Convert.ToDouble(partes[2+i].Replace (".",",").Trim());
								indice++;
							}
							encontrado=true;
						}
					}
					catch
					{
						MessageBox.Show("El fichero seleccionado no es de valoraciones o está corrupto","Error fichero incorrecto",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation );
						break;
					}
				}
			}
			srv.Close();
			return encontrado;
		}
		private short TestSeparador ()
		{
			aux=lineatexto.Split(sep);
			if (aux.Length ==3) 
			{
				if (EsNumero(aux[0]) && EsNumero(aux[1]) && EsNumero(aux[2]))
				{
					return 3;
				}
			}
			if (aux.Length ==42 || aux.Length ==45) 
			{
				short retorno=42;
				for(int i=0;i<42;i++)
				{
					if (!EsNumero(aux[i])) {retorno=2; break;}
				}
				return retorno;
			}
			if (aux.Length ==43 || aux.Length ==46) 
			{
				short retorno=43;
				for(int i=1;i<43;i++)
				{
					if (!EsNumero(aux[i])) {retorno=2; break;}
				}
				return retorno;
			}
			if (aux.Length ==44  || aux.Length ==47) 
			{
				short retorno=44;
				for(int i=2;i<44;i++)
				{
					if (!EsNumero(aux[i])) {retorno=2; break;}
				}
				return retorno;
			}
			return 2;
		}

		private short TestFichero (StreamReader srv)
		{
			
			lineatexto=LeerLinia(srv);
			
			// ---------------------------------------
			//	Probamos con separador ","
			// ---------------------------------------
			sep=',';
			if (TestSeparador()!=2) return TestSeparador();
			// ---------------------------------------
			//	Probamos con separador " "
			// ---------------------------------------
			sep=' ';
			if (TestSeparador()!=2) return TestSeparador();
			// ---------------------------------------
			//	Probamos con separador Tabulador
			// ---------------------------------------
			sep=(char)9;
			if (TestSeparador()!=2) return TestSeparador();
			if (EsNumero(lineatexto))return 1;
		    return 2;
		}

		private string LeerLinia(StreamReader srv)
		{
			lineatexto="";
			if (srv.Peek() > -1)
			{
				lineatexto=srv.ReadLine().Trim();
				lineatexto.Replace ((char)9,' ');
				lineatexto.Replace ("  ", " ");
			}
			return lineatexto;
		}

		public bool EsNumero (string Valor)
		{
			try {Convert.ToDouble(Valor);return true;} 
			catch {return false;}
		}
		public float[,] ValoresNeperianos()
		{
			float[,] valoresN = ValoresBase100();
            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
			{
				for(int j=0;j<3;j++)
				{
					valoresN[i,j]=(float)Math.Log (valoresN[i,j]);
				}
			}
			return valoresN;
		}
		public float[,] ValoresBase100()
		{
            float[,] valoresB100 = new float[VariablesGlobales.NumeroPartidos, 3];
		    for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
		    {
		        float factor = (float)(valores[i,0]+valores[i,1]+valores[i,2]);
		        for(int j=0;j<3;j++)
				{
					valoresB100[i,j]=(float)(valores[i,j]/factor);
				}
		    }
		    return valoresB100;
		}
		private void Guardar(string nombreArchivo)
		{
			int i,j;
		    String linea;
		
			StreamWriter sw = File.CreateText( nombreArchivo );				
			
			switch (FormatoFichero)
			{
				case 3:
                    for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
					{
						linea="";
						for (j=0;j<3;j++)
						{	
							linea += valores[i,j].ToString().Replace (",",".") ;
							if(j<2) linea += sep;
						}
						sw.WriteLine(linea );
					}
					break;
				case 1:
                    for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
					{
						for (j=0;j<3;j++)
						{	
							sw.WriteLine(valores[i,j].ToString());
						}
					}
					break;
				case 42:
					linea="";
                    for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
					{
						for (j=0;j<3;j++)
						{	
							linea += valores[i,j].ToString().Replace (",",".") + sep ;
						}
					}
					sw.WriteLine(linea );
					break;

				default:
					break;
			}
			sw.Close ();
		}
		public void GuardarValoraciones(string nombreArchivo,short formato3valoresPorFila, char separador, double[] valores1, double[] valoresX, double[] valores2) 
		{
			int i;

            for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
			{
				valores[i,0]=valores1[i];
				valores[i,1]=valoresX[i];
				valores[i,2]=valores2[i];
			}
			sep=separador;
			FormatoFichero=formato3valoresPorFila;
			Guardar (nombreArchivo);
		}
		public void GuardarValoraciones(string nombreArchivo,short formato3valoresPorFila, char separador, double[,] valores1X2) 
		{
			valores = valores1X2;
			sep=separador;
			FormatoFichero=formato3valoresPorFila;
			Guardar (nombreArchivo);
		}
		public void GuardarValoraciones(string nombreArchivo,char separador, double[,] valores1X2, string pTemporada, string pJornada) 
		{
			valores = valores1X2;
			_Temporada=pTemporada;
			_Jornada=pJornada;
			sep=separador;
			if(File.Exists (nombreArchivo))
			{
				StreamReader srv = new StreamReader(nombreArchivo);
				FormatoFichero=TestFichero (srv);
				srv.Close();
			}
			else
			{
				FormatoFichero =44;
			}
			if (FormatoFichero ==44)
			{
				//---Guardar datos del L.A.E. de la Jornada ------
				Jornadas =new ArrayList();
		
				bool JornadaYaExiste =false;

                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(nombreArchivo);
				StringBuilder linea= new StringBuilder ("");
				if(File.Exists (nombreArchivo))
				{
					while( comBaseCols.SiguienteColumna() )
					{
						linea.Remove (0,linea.Length );
						linea.Append (comBaseCols.LeeColumnaSinComas());
						ValorsJornada=  linea.ToString ().Split (sep);
						if(ValorsJornada[0]==_Temporada  && ValorsJornada[1]==_Jornada) 
						{
							JornadaYaExiste=true;
							linea.Remove (0,linea.Length );
							linea.Append ( MontaLinea(valores1X2,sep));
						}
						Jornadas.Add (linea.ToString());
					}
				}
				if(!JornadaYaExiste)
				{
					linea.Remove (0,linea.Length );
					linea.Append ( MontaLinea(valores1X2,sep));
					Jornadas.Add (linea.ToString());
				}
				comBaseCols.Cerrar();
                IArchivoColumnas comCols = new ArchivoColumnasTexto(nombreArchivo);
				foreach(string str in Jornadas)
				{
					comCols.GuardarColsComa(str);
				}
				comCols.Cerrar();	
			}
			else
			{
				MessageBox.Show ("El fichero no es un fichero de Valoraciones históricas");
			}
		}
		private string MontaLinea(double[,]valores1X2, char separador)
		{
			StringBuilder linea= new StringBuilder (_Temporada);

			linea.Append( separador);
			linea.Append( _Jornada);

            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
			{
				for (int x=0;x<3;x++)
				{
					linea.Append( separador);
					linea.Append( valores1X2[i,x].ToString ().Replace (".",","));
				}
			}	
			return linea.ToString ();
		}
		public void PonerPorcentajesEnElPortapapeles()
		{
			const char separador = (char) 9;
			const char LF = (char) 10;
			const char NL = (char) 13;

			string cadena= valores[0,0].ToString ()+ separador + valores[0,1]+ separador + valores[0,2];
            for (int i = 1; i < VariablesGlobales.NumeroPartidos; i++)
			{
				cadena = cadena + NL+LF + valores[i,0]+ separador + valores[i,1]+ separador + valores[i,2];
			}

			Clipboard.SetDataObject (cadena, true);
		}
		public bool RecuperarPorcentajesDelPortapapeles()
		{
		    bool ret =false;

			// Declares an IDataObject to hold the data returned from the clipboard.
			// Retrieves the data from the clipboard.
			IDataObject iData = Clipboard.GetDataObject();
 
			// Determines whether the data is in a format you can use.
		    if (iData != null)
		        if (iData.GetDataPresent(DataFormats.Text))
		        {
		            // Yes it is, so display it in a text box.
		            string cadena = (String) iData.GetData(DataFormats.Text);
		            mNombreFichero = Path.GetTempFileName();

		            StreamWriter sw = File.CreateText(mNombreFichero);

		            sw.Write(cadena);
		            sw.Close();
		            Leer();
		            ret = true;
		        }
		    return ret;
		}
	}
}

