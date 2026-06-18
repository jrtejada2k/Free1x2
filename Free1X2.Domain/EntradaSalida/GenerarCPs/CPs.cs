// Free1X2 · WinUI 3 — WIN3
using System;
using System.Data;

namespace Free1X2.EntradaSalida.GenerarCPs
{
	/// <summary>
	/// Descripción breve de CPsBL.
	/// </summary>
	public class CPs
	{
		public CPs()
		{
		}

		public string[,] CrearCPs(DataView miDataView, Valoracion[] valor)
		{
			int max=0;
			int j,k;
			int desde, hasta;
			int tipo;
			bool vacio;
			max=miDataView.Count;
			// Nombres de las columnas del dataview
			int c_Desde=1;
			int c_Hasta=2;
			int c_ForzarFijos=3;
			int c_NumFijos=4;
			int c_ForzarDobles=5;
			int c_NumDobles=6;
			int c_ForzarTriples=7;
			string[,] columnas=new string[VariablesGlobales.NumeroPartidos,max];
			string[] columnasTmp=new string[VariablesGlobales.NumeroPartidos];

			// Recorremos el dataset
			for (j=0;j<max;j++)
			{
				// Primero miramos que tipo de columnas buscamos.
				// Si son sólo de fijos/dobles, las "mandamos" a otro método
				// Usamos el flag tipo para usar el switch con la cláusula Default
				tipo=3;
				if(miDataView[j][c_ForzarFijos].ToString().Length>0) if(Convert.ToBoolean(miDataView[j][c_ForzarFijos])==true) tipo=1;
				if(miDataView[j][c_ForzarDobles].ToString().Length>0) if(Convert.ToBoolean(miDataView[j][c_ForzarDobles])==true) tipo=2;
				// Leemos los parámetros
				desde=Convert.ToInt16(miDataView[j][c_Desde]);
				hasta=Convert.ToInt16(miDataView[j][c_Hasta]);
				switch (tipo)
				{
					case 1:
						columnasTmp=ObtenerFijos(Convert.ToInt16(miDataView[j][c_NumFijos]),valor);
						for(k=0;k<columnasTmp.Length;k++)
						{
							columnas[k,j]=columnasTmp[k];
						}
						break;
					case 2:
						columnasTmp=ObtenerDobles(Convert.ToInt16(miDataView[j][c_NumDobles]),valor);
						for(k=0;k<columnasTmp.Length;k++)
						{
							columnas[k,j]=columnasTmp[k];
						}
						break;
					default:
						for(k=0;k<valor.Length;k++)
						{
							columnas[k,j]="";
							if(valor[k].Uno>=desde && valor[k].Uno<=hasta)
							{
								columnas[k,j]+="1";
							}
							if(valor[k].Equis>=desde && valor[k].Equis<=hasta)
							{
								columnas[k,j]+="X";
							}
							if(valor[k].Dos>=desde && valor[k].Dos<=hasta)
							{
								columnas[k,j]+="2";
							}
							vacio=false;
							if(miDataView[j][c_ForzarTriples].ToString().Length>0) vacio=Convert.ToBoolean(miDataView[j][c_ForzarTriples]);
							if((columnas[k,j].Length==0) && (vacio==true))
							{
								columnas[k,j]="1X2";
							}
						}
						break;
				}
			}
			return columnas;
		}

		public string[] ObtenerFijos(int max, Valoracion[] valor)
		{
			string[] signos=new String[VariablesGlobales.NumeroPartidos];
			if(max==VariablesGlobales.NumeroPartidos)
			{
				signos=ObtenerTodosFijos(valor);
				return signos;
			}
			// Utilizamos una variable intermedia
			Valoracion[] valorTmp=new Valoracion[VariablesGlobales.NumeroPartidos];
            for (int i = 0; i < valorTmp.Length; i++)
            {
                valorTmp[i] = new Valoracion();
                valorTmp[i].Uno = valor[i].Uno;
                valorTmp[i].Equis = valor[i].Equis;
                valorTmp[i].Dos = valor[i].Dos;
            }
			double[] matrizOrdenada=OrdenarValoracion(valorTmp);
			Array.Reverse(matrizOrdenada);
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < valorTmp.Length; j++)
                {
                    if (valorTmp[j].Uno == matrizOrdenada[i])
                    {
                        signos[j] = "1";
                        valorTmp[j].Uno = 0;
                        break;
                    }
                    if (valorTmp[j].Equis == matrizOrdenada[i])
                    {
                        signos[j] = "X";
                        valorTmp[j].Equis = 0;
                        break;
                    }
                    if (valorTmp[j].Dos == matrizOrdenada[i])
                    {
                        signos[j] = "2";
                        valorTmp[j].Dos = 0;
                        break;
                    }
                }
            }
			return signos;
		}

		public string[] ObtenerTodosFijos(Valoracion[] valor)
		{
			string[] signos=new String[VariablesGlobales.NumeroPartidos];
            for (int i = 0; i < valor.Length; i++)
            {
                if (valor[i].Uno > valor[i].Equis && valor[i].Uno > valor[i].Dos) signos[i] = "1";
                if (valor[i].Equis > valor[i].Uno && valor[i].Equis > valor[i].Dos) signos[i] = "X";
                if (valor[i].Dos > valor[i].Equis && valor[i].Dos > valor[i].Uno) signos[i] = "2";
            }
			return signos;
		}

		public string[] ObtenerTodosDobles(Valoracion[] valor)
		{
			string[] signos=new String[VariablesGlobales.NumeroPartidos];
            for (int i = 0; i < valor.Length; i++)
            {
                if (valor[i].Uno < valor[i].Equis && valor[i].Uno < valor[i].Dos) signos[i] = "X2";
                if (valor[i].Equis < valor[i].Uno && valor[i].Equis < valor[i].Dos) signos[i] = "12";
                if (valor[i].Dos < valor[i].Equis && valor[i].Dos < valor[i].Uno) signos[i] = "1X";
            }
			return signos;
		}

		public string[] ObtenerDobles(int max, Valoracion[] valor)
		{
			string[] signos=new String[VariablesGlobales.NumeroPartidos];
            if (max == VariablesGlobales.NumeroPartidos)
            {
                signos = ObtenerTodosDobles(valor);
                return signos;
            }
			// Utilizamos una variable intermedia
			Valoracion[] valorTmp=new Valoracion[VariablesGlobales.NumeroPartidos];
            for (int i = 0; i < valorTmp.Length; i++)
            {
                valorTmp[i] = new Valoracion();
                valorTmp[i].Uno = valor[i].Uno;
                valorTmp[i].Equis = valor[i].Equis;
                valorTmp[i].Dos = valor[i].Dos;
            }
			double[] matrizOrdenada=OrdenarValoracion(valorTmp);
			for(int i=0;i<max;i++)
			{
                for (int j = 0; j < VariablesGlobales.NumeroPartidos; j++)
                {
                    if (valorTmp[j].Uno == matrizOrdenada[i])
                    {
                        signos[j] = "X2";
                        valorTmp[j].Uno = 99.99;
                        break;
                    }
                    if (valorTmp[j].Equis == matrizOrdenada[i])
                    {
                        signos[j] = "12";
                        valorTmp[j].Equis = 99.99;
                        break;
                    }
                    if (valorTmp[j].Dos == matrizOrdenada[i])
                    {
                        signos[j] = "1X";
                        valorTmp[j].Dos = 99.99;
                        break;
                    }
                }
			}
			return signos;
		}

        public double[] OrdenarValoracion(Valoracion[] valor)
        {
            double[] matrizOrdenada = new double[VariablesGlobales.NumeroPartidos * 3];
            for (int i = 0; i < valor.Length; i++)
            {
                matrizOrdenada[i * 3] = valor[i].Uno;
                matrizOrdenada[(i * 3) + 1] = valor[i].Equis;
                matrizOrdenada[(i * 3) + 2] = valor[i].Dos;
            }
            Array.Sort(matrizOrdenada);
            return matrizOrdenada;
        }

	}
}
