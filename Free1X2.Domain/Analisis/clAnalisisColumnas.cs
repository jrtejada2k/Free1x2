using System;

namespace Free1X2.Analisis
{
	/// <summary>
	/// Descripción breve de clAnalisisColumnas.
	/// </summary>
	public class clAnalisisColumnas
	{
		private Filtro filtro;
		private int maxAciertos=14;
		private int[,] aciertos;
		private string[] nombreCondicion;
		private int numCols;

		public clAnalisisColumnas(Filtro _filtro)
		{
			inicializa(_filtro, 14);
		}

		public clAnalisisColumnas(Filtro _filtro, int _numPartidos)
		{
			inicializa(_filtro, _numPartidos);
		}

		private void inicializa(Filtro _filtro, int numPartidos)
		{
			filtro=_filtro;
			switch(filtro)
			{
				case Filtro.NoVariantes:
					maxAciertos=numPartidos;
					nombreCondicion=new string[] {"Cantidad de Variantes", "Cantidad de X", "Cantidad de 2"};
					aciertos=new int[MaxCondiciones, maxAciertos+1];
					break;
				case Filtro.Contactos:
					maxAciertos=numPartidos-1;
					nombreCondicion=new string[] {"1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"};
					aciertos=new int[MaxCondiciones, maxAciertos+1];
					break;
				case Filtro.Distancias:
					maxAciertos=numPartidos-1;
					nombreCondicion=new string[] {"Distancia de Variantes", "Distancia de 1", "Distancia de X", "Distancia de 2"};
					aciertos=new int[MaxCondiciones, maxAciertos+1];
					break;
				case Filtro.NoInterrupciones:
					maxAciertos=numPartidos-1;
					nombreCondicion=new string[] {"Interrupciones Globales", "Interrupciones de Variantes", "Interrupciones de 1", "Interrupciones de X", "Interrupciones de 2", "Interrupciones Seguidas Globales", "Interrupciones Seguidas de Variantes", "Interrupciones Seguidas de 1", "Interrupciones Seguidas de X", "Interrupciones Seguidas de 2"};
					aciertos=new int[MaxCondiciones, maxAciertos+1];
					break;
				case Filtro.PesosNumericos:
					maxAciertos=9;
					nombreCondicion=new string[] {"Peso Numérico Global", "Peso Numérico de Variantes", "Peso Numérico de 1", "Peso Numérico de X", "Peso Numérico de 2"};
					aciertos=new int[MaxCondiciones, maxAciertos+1];
					break;
				case Filtro.SignosSeguidos:
					maxAciertos=numPartidos;
					nombreCondicion=new string[] {"Variantes Seguidas", "1 Seguidos", "X Seguidos", "2 Seguidos"};
					aciertos=new int[MaxCondiciones, maxAciertos+1];
					break;
			}
		}

		public Filtro NombreFiltro
		{
			get{ return filtro;}
		}

		public int NumCols
		{
			get{ return numCols;}
			set{ numCols=value;}
		}

		public int MaxAciertos
		{
			get{ return maxAciertos;}
		}

		public int MaxCondiciones
		{
			get{ return nombreCondicion.Length;}
		}

		public string[] NombreCondicion
		{
			get{ return nombreCondicion;}
		}
	
		public int[,] Aciertos
		{
			get{ return aciertos;}
		}

		public double Porcentaje(int numCondicion, int numValor)
		{
			double n=0;
			if(numCols>0)
			{
				double cols=Convert.ToDouble(aciertos[numCondicion,numValor]*100);
				double totCols=Convert.ToDouble(numCols);
				n=cols/totCols;
			}
			return n;
		}

	}
}
