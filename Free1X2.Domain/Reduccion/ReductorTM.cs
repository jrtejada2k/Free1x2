// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using Free1X2.EntradaSalida;

namespace Free1X2.Reduccion
{
	/// <summary>
	/// Descripción breve de ReductorTM.
	/// </summary>
	public class ReductorTM: Base, IReduccion
	{
		// P-12: tabla de potencias de 3. Antes Base3aBase10 llamaba a Math.Pow por
		// cada dígito y convertía el resultado double con Convert.ToInt16.
		private static readonly int[] pot3 = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969 };

		// P-16: List<string> en vez de ArrayList (sin boxing ni ToString() por columna).
		private List<string> columnasBaseDisponibles;
		private string archivoEntrada;
		private readonly string[] columnas=new string[243];
		private readonly int[,] diferencias=new int[243,243];
		private int[,] codigosColumnas;
		// P-16: listas de adyacencia como List<int> en vez de strings CSV. Antes cada
		// vecino costaba una concatenación de strings al construirlas y un
		// Split/Convert/Replace/IndexOf al recorrerlas, con O(n²) de reasignaciones.
		private List<int>[] reduceA;
		private int[] reduceCols;
	    readonly List<string> reductoras=new List<string>();
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
				// P-12: se comparan caracteres en vez de crear dos strings de 1 carácter
				// por posición. Eran 243×243×5 ≈ 300 000 pares de Substring + comparación
				// de strings en cada construcción del ReductorTM. Comparar los char es
				// exactamente la misma condición (igualdad ordinal de un solo carácter).
				string colI = columnas[i];
				for(int j=0;j<243;j++)
				{
					int dif = 0;
					if(i!=j)
					{
						string colJ = columnas[j];
						for(int p=0;p<colI.Length;p++)
						{
							if(colI[p]!=colJ[p]) dif++;
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
		        // P-12: dígito por indexación y potencia por tabla, sin Substring ni
		        // Math.Pow. Se conserva el fallo con caracteres no numéricos, que antes
		        // producía Convert.ToInt16(string) => FormatException.
		        int pos = numero[i] - '0';
		        if(pos<0 || pos>9) throw new FormatException("Carácter no numérico en base 3: '" + numero[i] + "'");
		        b10+=pos*pot3[numero.Length-1-i];
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
		    columnasBaseDisponibles = new List<string>();
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
				columna=columnasBaseDisponibles[i].Replace("X","0");
				codigosColumnas[i,0]=Convert.ToInt16(Base3aBase10(columna.Substring(0,5)));
				codigosColumnas[i,1]=Convert.ToInt16(Base3aBase10(columna.Substring(5,5)));
				codigosColumnas[i,2]=Convert.ToInt16(Base3aBase10(columna.Substring(10)));
			}
			// Busca las diferencias entre las columnas
			// P-16: una lista por columna en vez de un string CSV. El orden de inserción
			// de los vecinos es exactamente el de antes (mismo doble bucle).
			reduceA=new List<int>[noColumnasIniciales];
			for(int i=0;i<noColumnasIniciales;i++) reduceA[i]=new List<int>();
			reduceCols=new int[noColumnasIniciales];
			for(int i=0;i<noColumnasIniciales;i++)
			{
				// P-16: los 3 códigos de la columna i se leen una vez, no en cada j.
				int codI0=codigosColumnas[i,0];
				int codI1=codigosColumnas[i,1];
				int codI2=codigosColumnas[i,2];
				List<int> vecinosI=reduceA[i];
				for(int j=i;j<noColumnasIniciales;j++)
				{
					if(i<=j)
					{
						int dif = 0;
						if(i!=j)
						{
							dif+=diferencias[codI0,codigosColumnas[j,0]];
							if(dif<=diferencia)
							{
								dif+=diferencias[codI1,codigosColumnas[j,1]];
								if(dif<=diferencia)
								{
									dif+=diferencias[codI2,codigosColumnas[j,2]];
								}
							}
						}
						if(dif<=diferencia)
						{
							vecinosI.Add(j);
							reduceCols[i]++;
							if(i!=j)
							{
								reduceA[j].Add(i);
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

		/// <summary>
		/// P-16 · Misma selección y mismo orden de salida que el original, sin strings.
		/// Cambios: (a) el máximo y el número de columnas ya procesadas se calculan en UNA
		/// pasada por reduceCols en vez de copiar + Array.Sort + Array.Reverse +
		/// Array.IndexOf en cada vuelta del while; (b) las listas de adyacencia son
		/// List&lt;int&gt;, con lo que desaparecen Split/Convert/Replace/IndexOf por vecino;
		/// (c) en la rama mayor==1 el índice del siguiente 1 se busca con un cursor que
		/// avanza, porque las posiciones anteriores acaban de ponerse a 0 (equivale
		/// exactamente a repetir Array.IndexOf(reduceCols, 1)).
		/// </summary>
		protected override void Reduce(int nivelReduccion, int maxCol, int percent)
		{
			int ticks=0;
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
			    // Equivale a ordenar de mayor a menor y leer [0] (el máximo) y la posición
			    // del primer 0, que en ese orden cae justo tras los valores positivos.
			    mayor = reduceCols[0];
			    int positivos = 0;
			    bool hayCero = false;
			    for (int k = 0; k < reduceCols.Length; k++)
			    {
			        int v = reduceCols[k];
			        if (v > mayor) mayor = v;
			        if (v > 0) positivos++;
			        else if (v == 0) hayCero = true;
			    }
			    int menor = hayCero ? positivos : -1;
			    if (menor > 0) noColumnasProcesadas = reduceCols.Length - menor;
			    if (mayor == 0)
			        break;
			    if (mayor == 1)
			    {
			        // Estas columnas sólo se reducen a sí mismas y se añaden diréctamente a la reducción
			        int cursor = 0;
			        for (int i = 0; i < menor; i++)
			        {
			            while (reduceCols[cursor] != 1) cursor++;
			            numCol = cursor;
			            reductoras.Add(columnasBaseDisponibles[numCol]);
			            noColumnasFinales++;
			            reduceA[numCol].Clear();
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
			        List<int> colsReductoras = reduceA[numCol];
			        if (colsReductoras.Count == 0) continue;
			        // Una vez encontrada la columna la añadimos a las reductoras, limpiamos sus reductoras
			        // y ponemos el contador a 0.
			        // El original seguía recorriendo una COPIA del texto tras vaciar
			        // reduceA[numCol]; aquí se sustituye la lista por una nueva vacía (en vez
			        // de Clear()) para no tocar la que se está recorriendo.
			        reduceA[numCol] = new List<int>();
			        reduceCols[numCol] = 0;
			        reductoras.Add(columnasBaseDisponibles[numCol]);
			        noColumnasFinales++;
			        // Recorremos la matriz de reducidas por la anterior, cada una de ellas la ponemos a 0
			        // volvemos a recorrer sus reducidas
			        for (int i = 0; i < colsReductoras.Count; i++)
			        {
			            ticks++;
			            if (ticks == 500)
			            {
			                ticks = 0;
			                Free1X2.Abstractions.UiPump.Pump();
			                if (salida) break;
			            }
			            int numCol2 = colsReductoras[i];
			            // Fidelidad: el original hacía Convert.ToInt16 sobre el índice, que
			            // desborda por encima de 32767 columnas. Se conserva ese límite.
			            if (numCol2 > short.MaxValue) throw new OverflowException("El valor era demasiado grande para un Int16.");
			            if (numCol != numCol2)
			            {
			                List<int> colsReductoras2 = reduceA[numCol2];
			                if (colsReductoras2.Count == 0) continue;
			                reduceCols[numCol2] = 0;
			                reduceA[numCol2] = new List<int>();
			                // A estas columnas reducidas, le quitamos la columna anterior de la lista y restamos
			                // su contador en 1.
			                for (int j = 0; j < colsReductoras2.Count; j++)
			                {
			                    int numCol3 = colsReductoras2[j];
			                    if (numCol3 > short.MaxValue) throw new OverflowException("El valor era demasiado grande para un Int16.");
			                    if (numCol3 != numCol2 && numCol3 != numCol)
			                    {
			                        borrar[0] = numCol;
			                        borrar[1] = numCol2;
			                        List<int> vecinos3 = reduceA[numCol3];
			                        // Elimina las columnas previas. Las listas no tienen
			                        // repetidos (cada par (i,j) se añade una sola vez), luego
			                        // Remove equivale al Replace de un único ",X," del original
			                        // y al decremento único de su contador.
			                        for (int b = 0; b < borrar.Length; b++)
			                        {
			                            if (vecinos3.Remove(borrar[b]))
			                            {
			                                reduceCols[numCol3]--;
			                            }
			                        }
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
				comReducCols.GuardarCols(reductoras[nr]);
			}	
			comReducCols.Cerrar();	
		}

	}
}
