using System;
using System.Collections;
using Free1X2.EntradaSalida;

namespace Free1X2.Reduccion
{
	/// <summary>
	/// Descripción breve de ReductorTM.
	/// </summary>
	public class ReductorTM: Base, IReduccion
	{
		private ArrayList columnasBaseDisponibles;
		private string archivoEntrada;
		private readonly string[] columnas=new string[243];
		private readonly int[,] diferencias=new int[243,243];
		private int[,] codigosColumnas;
		private string[] reduceA;
		private int[] reduceCols;
	    readonly ArrayList reductoras=new ArrayList();
		private bool matrizOk;
		int diferencia;

		public ReductorTM()
		{
			CrearMatriz();
		}

		private void CrearMatriz()
		{
		    for(int i=0;i<243;i++)
			{
				columnas[i]=Base10aBase3(i.ToString(), 5);
			}
			for(int i=0;i<243;i++)
			{
				for(int j=0;j<243;j++)
				{
					int dif = 0;
					if(i!=j)
					{
						for(int p=0;p<columnas[i].Length;p++)
						{
							if(columnas[i].Substring(p,1)!=columnas[j].Substring(p,1)) dif++;
						}
					}
					diferencias[i,j]=dif;
				}
			}
		}

		private string Base3aBase10(string numero)
		{
			int b10=0;
		    for(int i=0;i<numero.Length;i++)
		    {
		        int pos = Convert.ToInt16(numero.Substring(i,1));
		        b10+=pos*Convert.ToInt16(Math.Pow(3,numero.Length-1-i));
		    }
		    return b10.ToString();
		}

		private string Base10aBase3(string numero, int longitud)
		{
			string sal="";
			int num=Convert.ToInt16(numero);
		    while(num>=3)
			{
				int resto = num%3;
				num/=3;
				sal=resto+sal;
			}
			sal=num+sal;
			for(int i=sal.Length;i<longitud;i++)
			{
				sal="0"+sal;
			}
			return sal;
		}

		public void Inicializa(string entrada, int nivelReduccion)
		{
			diferencia=14-nivelReduccion;
			if(matrizOk) inicializaVariables();
			if(!entrada.Equals("")) EntradaDeDatos(entrada);
		}

		private void inicializaVariables()
		{
			columnasBaseDisponibles.Clear();
			codigosColumnas=null;
			reduceA=null;
			reduceCols=null;
			reductoras.Clear();
			matrizOk=false;
		}

		protected override void EntradaDeDatos(string entrada)
		{
			archivoEntrada=entrada;
			int ticks=0;
			string columna;
		    columnasBaseDisponibles = new ArrayList();
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			//carga todas las columnas en array
			while( comBaseCols.SiguienteColumna() )
			{
				columna = comBaseCols.LeeColumnaSinComas();	
				columnasBaseDisponibles.Add( columna );
			}
			comBaseCols.Cerrar();
			noColumnasIniciales =  columnasBaseDisponibles.Count;
			// Obtiene las columnas con 3 diferencias
			codigosColumnas=new int[noColumnasIniciales,3];
			for(int i=0;i<noColumnasIniciales;i++)
			{
				columna=columnasBaseDisponibles[i].ToString().Replace("X","0");
				codigosColumnas[i,0]=Convert.ToInt16(Base3aBase10(columna.Substring(0,5)));
				codigosColumnas[i,1]=Convert.ToInt16(Base3aBase10(columna.Substring(5,5)));
				codigosColumnas[i,2]=Convert.ToInt16(Base3aBase10(columna.Substring(10)));
			}
			// Busca las diferencias entre las columnas
			reduceA=new string[noColumnasIniciales];
			reduceCols=new int[noColumnasIniciales];
			for(int i=0;i<noColumnasIniciales;i++)
			{
				for(int j=i;j<noColumnasIniciales;j++)
				{
					if(i<=j)
					{
						int dif = 0;
						if(i!=j)
						{
							for(int k=0;k<3;k++)
							{
								dif+=diferencias[codigosColumnas[i,k],codigosColumnas[j,k]];
								if(dif>diferencia) break;
							}
						}
						if(dif<=diferencia)
						{
							reduceA[i]+=j+",";
							reduceCols[i]++;
							if(i!=j)
							{
								reduceA[j]+=i+",";
								reduceCols[j]++;
							}
						}
					}
					ticks++;
					if(ticks==500)
					{
						ticks=0;
						Free1X2.Abstractions.UiPump.Pump();
						if (salida) break;
					}
				}
			}
			matrizOk=true;
		}

		public override void ComienzaReduccion(string entrada, string sal, int nivelReduccion, int maxCol, int percent)
		{
			if(matrizOk==false) EntradaDeDatos(entrada);
			Reduce(nivelReduccion, maxCol, percent);
			GrabacionDeReductoras(sal, nivelReduccion);
		}

		protected override void Reduce(int nivelReduccion, int maxCol, int percent)
		{
			int ticks=0;
			string[] colsReductoras;
			string[] colsReductoras2;
			string txtColsReductoras;
			string txtColsReductoras2;
			string txtColsReductoras3;
			int[] matrizTemporal=new int[reduceCols.Length];
		    int mayor = -1;
		    if(matrizOk==false) EntradaDeDatos(archivoEntrada);
			noColumnasFinales=0;
			noColumnasProcesadas=0;
			int[] borrar=new int[2];
			reductoras.Clear();

			while(mayor!=0)
			{
			    int numCol;
			    // Buscamos el máximo de columnas reducidas por otra.
			    reduceCols.CopyTo(matrizTemporal, 0);
			    Array.Sort(matrizTemporal);
			    Array.Reverse(matrizTemporal);
			    mayor = matrizTemporal[0];
			    int menor = Array.IndexOf(matrizTemporal, 0);
			    if (menor > 0) noColumnasProcesadas = matrizTemporal.Length - menor;
			    if (mayor == 0)
			        break;
			    if (mayor == 1)
			    {
			        // Estas columnas sólo se reducen a sí mismas y se añaden diréctamente a la reducción
			        for (int i = 0; i < menor; i++)
			        {
			            numCol = Array.IndexOf(reduceCols, mayor);
			            reductoras.Add(columnasBaseDisponibles[numCol]);
			            noColumnasFinales++;
			            reduceA[numCol] = "";
			            reduceCols[numCol] = 0;
			            ticks++;
			            if (ticks == 500)
			            {
			                ticks = 0;
			                Free1X2.Abstractions.UiPump.Pump();
			                if (salida) break;
			            }
			        }
			    }
			    else
			    {
			        numCol = Array.IndexOf(reduceCols, mayor);
			        // Buscamos la columna que más columnas reduce.
			        txtColsReductoras = reduceA[numCol];
			        if (txtColsReductoras.Length == 0) continue;
			        txtColsReductoras = txtColsReductoras.Substring(0, txtColsReductoras.Length - 1);
			        colsReductoras = txtColsReductoras.Split(',');
			        // Una vez encontrada la columna la añadimos a las reductoras, limpiamos sus reductoras
			        // y ponemos el contador a 0.
			        reduceA[numCol] = "";
			        reduceCols[numCol] = 0;
			        reductoras.Add(columnasBaseDisponibles[numCol]);
			        noColumnasFinales++;
			        // Recorremos la matriz de reducidas por la anterior, cada una de ellas la ponemos a 0
			        // volvemos a recorrer sus reducidas
			        for (int i = 0; i < colsReductoras.Length; i++)
			        {
			            ticks++;
			            if (ticks == 500)
			            {
			                ticks = 0;
			                Free1X2.Abstractions.UiPump.Pump();
			                if (salida) break;
			            }
			            int numCol2 = Convert.ToInt16(colsReductoras[i]);
			            if (numCol != numCol2)
			            {
			                txtColsReductoras2 = reduceA[numCol2];
			                if (txtColsReductoras2.Length == 0) continue;
			                txtColsReductoras2 = txtColsReductoras2.Substring(0, txtColsReductoras2.Length - 1);
			                colsReductoras2 = txtColsReductoras2.Split(',');
			                reduceCols[numCol2] = 0;
			                reduceA[numCol2] = "";
			                // A estas columnas reducidas, le quitamos la columna anterior de la lista y restamos
			                // su contador en 1.
			                for (int j = 0; j < colsReductoras2.Length; j++)
			                {
			                    int numCol3 = Convert.ToInt16(colsReductoras2[j]);
			                    if (numCol3 != numCol2 && numCol3 != numCol)
			                    {
			                        borrar[0] = numCol;
			                        borrar[1] = numCol2;
			                        txtColsReductoras3 = "," + reduceA[numCol3];
			                        // Elimina las columnas previas
			                        for (int b = 0; b < borrar.Length; b++)
			                        {
			                            string tmp = "," + borrar[b] + ",";
			                            int pos = txtColsReductoras3.IndexOf(tmp);
			                            if (pos >= 0)
			                            {
			                                txtColsReductoras3 = txtColsReductoras3.Replace(tmp, ",");
			                                reduceCols[numCol3]--;
			                            }
			                        }
			                        reduceA[numCol3] = txtColsReductoras3.Substring(1);
			                    }
			                }
			            }
			        }
			    }
			}
		    matrizOk=false;
			noColumnasProcesadas=noColumnasIniciales;
		}

	    protected override void GrabacionDeReductoras(string archivoSalida, int nivelReduccion)
		{
            IArchivoColumnas comReducCols = new ArchivoColumnasTexto(archivoSalida);
			for (int nr=0; nr<reductoras.Count; nr++) 
			{
				comReducCols.GuardarCols(reductoras[nr].ToString());
			}	
			comReducCols.Cerrar();	
		}

	}
}
