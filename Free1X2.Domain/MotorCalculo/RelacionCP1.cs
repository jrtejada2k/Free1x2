// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
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
using System.Collections.Generic;
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for RelacionCP1.
	/// </summary>
	public class RelacionCP1
	{
		private string strColumnas = "";
		private string strSumas = "";
		private string strRecorrido = "";
		
		//grupos de CP
		private string strCantidadCP = "";
		private string strCuantosAC = "";

		//variable apra almacenar que CP hacen cumplir 
		//la condicion de grupos de CP
		// P-17: antes era un string CSV que se reconstruía por concatenación y se
		// re-parseaba con Split + Convert.ToInt32 en CADA columna analizada. Ahora es una
		// lista de enteros reutilizada; el orden de inserción es el mismo que tenía el CSV.
		private readonly List<int> listaCPs = new List<int>();
		// P-17: buffers reutilizables de AnalizaGruposColumnas (antes un int[15], un
		// string[15] y 15 strings vacías por columna).
		private readonly int[] noAciertosPorCP = new int[15];
		private readonly List<int>[] cpsPorNoAciertos = CrearBuffersCPs();

		private static List<int>[] CrearBuffersCPs()
		{
			List<int>[] buffers = new List<int>[15];
			for(int i = 0; i < 15; i++) buffers[i] = new List<int>();
			return buffers;
		}

		private int[] columnasRelacionadas;
		//private RangosOpciones rangosSumas;
		
		private bool[] sumas;
		private bool[] recorridos;

		private bool[] cantidadCP;
		private bool[] cuantosAC;
		
		//el controlador es el objeto parent que contiene a este objeto
		protected ControladorRelacionesCP1 controlador;

		protected List<ColumnaProbable> columnasProbables;
		public int noColConMismosAC;			
		public int recorrido;
		private int sumaTemp;

	    public bool Analiza()
		{
			bool relacionValida = true;
			InicializaValores();
			if(strCantidadCP != "")
			{
				relacionValida = AnalizaGruposColumnas();
			}
			if(relacionValida && strSumas != "")
			{
				relacionValida = AnalizaSumas();
			}
			if(relacionValida && strRecorrido != "")
			{
				relacionValida = AnalizaRecorridos();
			}		
            return 	relacionValida;
		}

		public string Analiza(int numRel)
		{
			string txt="";
			bool relacionValida = true;
			InicializaValores();
			if(strCantidadCP != "")
			{
				relacionValida = AnalizaGruposColumnas();
				if(relacionValida ==false)
					txt+="Fallo en Cantidad de CPs de la Relación de Columnas " + numRel+"  ("+noColConMismosAC+")#";
			}
			if(relacionValida && strSumas != "")
			{
				relacionValida = AnalizaSumas();
				if(relacionValida ==false)
					txt+="Fallo en Suma de Aciertos de la Relación de Columnas " + numRel+"  ("+sumaTemp+")#";
			}
			if(relacionValida && strRecorrido != "")
			{
				relacionValida = AnalizaRecorridos();
				if(relacionValida ==false)
					txt+="Fallo en Recorrido de la Relación de Columnas " + numRel+"  ("+recorrido+")#";
			}
			return 	txt;
		}

		protected void InicializaValores()
		{
			listaCPs.Clear();
			sumaTemp=0;
			noColConMismosAC=0;
		}

		protected bool AnalizaSumas()
		{
			bool relacionValida = true;
			ColumnaProbable cp;

			//listaCPs tendra valores si se han analizado los grupos de columnas
			if(listaCPs.Count == 0)
			{
				for(int i = 0; i < columnasRelacionadas.Length; i++)
				{
					cp = columnasProbables[ columnasRelacionadas[i]-1 ];
					sumaTemp += cp.NoAC;
				}
			}
			else
			{
			    // P-17: se recorre la lista de enteros, sin Split ni Convert.ToInt32.
			    for(int i = 0; i< listaCPs.Count; i++ )
				{
					int noCP = listaCPs[i];
					
					cp = columnasProbables[ columnasRelacionadas[ noCP ]-1 ];
					sumaTemp += cp.NoAC;
				}			
			}
			
			if(sumas.Length < sumaTemp+1)
			{
				//se sale de rango, por lo tanto condicion fallada
				relacionValida = false;
			}	
			else if(sumas[ sumaTemp ] == false)
			{
				relacionValida = false;
			}			
			
			return relacionValida;
		}

		protected bool AnalizaRecorridos()
		{
			bool relacionValida = true;

			int maxAC = 0;
			int minAC = 14;

			ColumnaProbable cp;
			int tempNoAC;

			//listaCPs tendra valores si se han analizado los grupos de columnas
			if(listaCPs.Count == 0)
			{
				for(int i = 0; i < columnasRelacionadas.Length; i++)
				{
					cp = columnasProbables[ columnasRelacionadas[i]-1 ];
				
					tempNoAC = cp.NoAC;

					if(tempNoAC < minAC)
					{
						minAC = tempNoAC;
					}

					if(tempNoAC > maxAC)
					{
						maxAC = tempNoAC;
					}
				}
			}
			else
			{
			    // P-17: se recorre la lista de enteros, sin Split ni Convert.ToInt32.
			    for(int i = 0; i < listaCPs.Count; i++ )
				{
					int noCP = listaCPs[i];
				
					cp = columnasProbables[ columnasRelacionadas[ noCP ]-1 ];
				
					tempNoAC = cp.NoAC;

					if(tempNoAC < minAC)
					{
						minAC = tempNoAC;
					}

					if(tempNoAC > maxAC)
					{
						maxAC = tempNoAC;
					}
				}			
			}

			recorrido = maxAC - minAC;

			if(recorridos[ recorrido ] == false)
			{
				relacionValida = false;
			}

			return relacionValida;		
		}
		
		/// <summary>
		/// P-17 · Agrupa los CP relacionados por su número de aciertos. Misma semántica y
		/// mismo orden que la versión con strings CSV: para cada número de aciertos
		/// aceptado (en orden creciente) se añaden las posiciones de ese grupo en el orden
		/// en que se recorrieron. Los buffers son de instancia y se limpian en cada llamada,
		/// en vez de asignar un int[15] + un string[15] + 15 strings vacías por columna.
		/// </summary>
		protected bool AnalizaGruposColumnas()
		{
			bool relacionValida = false;
			Array.Clear(noAciertosPorCP, 0, noAciertosPorCP.Length);
			for(int i = 0; i < 15; i++)
			{
				cpsPorNoAciertos[i].Clear();
			}

		    for(int i = 0; i < columnasRelacionadas.Length; i++)
			{
				ColumnaProbable cp = columnasProbables[ columnasRelacionadas[i]-1 ];
				
				noAciertosPorCP[ cp.NoAC ]++;
				cpsPorNoAciertos[cp.NoAC ].Add(i);
			}

		    for(int i = 0; i < cuantosAC.Length; i++)
			{
				if(cuantosAC[i])
				{					
					noColConMismosAC += noAciertosPorCP[i];	

					List<int> grupo = cpsPorNoAciertos[i];
					for(int j = 0; j < grupo.Count; j++)
					{
						listaCPs.Add(grupo[j]);
					}
				}			
			}	
		
			if(noColConMismosAC < cantidadCP.Length && cantidadCP[noColConMismosAC])
			{
				relacionValida = true;						
			}
		
			return relacionValida;
		}

		protected void InicializarColumnasRelacionadas()
		{
			string separador = EncuentraSeparador( strColumnas );
			
			if( separador == "," )
			{
				string[] tempCP = strColumnas.Split( ',' );
				
				columnasRelacionadas = new int[ tempCP.Length ];

				for(int i = 0; i < tempCP.Length; i++)
				{
					columnasRelacionadas[ i ] = Convert.ToInt32( tempCP[ i ] );				
				}	
			}
			else if( separador == "-")
			{
				string[] tempCP = strColumnas.Split( '-' );

				int CP1 = Convert.ToInt32(tempCP[0]);
				int CP2 = Convert.ToInt32(tempCP[1]);

				int noCPNuevos = (CP2 - CP1) + 1;

				columnasRelacionadas = new int[ noCPNuevos ];

				for(int i = 0; i < noCPNuevos; i++ )
				{
					columnasRelacionadas[ i ] = CP1 + i;				
				}
			}
			else
			{
				//gruposControl solo contiene un grupo.
				columnasRelacionadas = new int[ 1 ];
				columnasRelacionadas[ 0 ] = Convert.ToInt32( strColumnas );
			}			
		}

		protected void InicializarSumas()
		{						
			if(strSumas != "")
			{
				string separador = EncuentraSeparador( strSumas );
			
				if( separador == "," )
				{
					string[] tempCP = strSumas.Split( ',' );
					
					int capacidadArray = Convert.ToInt32(tempCP[tempCP.Length -1]) + 2;
					sumas = new bool[ capacidadArray ];
				
					for(int i = 0; i < tempCP.Length; i++)
					{
						sumas[ Convert.ToInt32(tempCP[i]) ] = true;				
					}	
				}
				else if( separador == "-")
				{
					string[] tempCP = strSumas.Split( '-' );

					int CP1 = Convert.ToInt32(tempCP[0]);
					int CP2 = Convert.ToInt32(tempCP[1]);

					int noCPNuevos = (CP2 - CP1) + 1;

					sumas = new bool[CP2 + 2];

					for(int i = 0; i < noCPNuevos; i++ )
					{
						sumas[ CP1 + i ] = true;				
					}
				}
				else
				{					
					sumas = new bool[ Convert.ToInt32( strSumas ) + 2];
					sumas[ Convert.ToInt32( strSumas ) ] = true;
				}
			}
		}

		protected void InicializarRecorridos()
		{
			recorridos = new bool[15];		

			if(strRecorrido != "")
			{

				string separador = EncuentraSeparador( strRecorrido );
			
				if( separador == "," )
				{
					string[] tempCP = strRecorrido.Split( ',' );
				
					for(int i = 0; i < tempCP.Length; i++)
					{
						recorridos[ Convert.ToInt32(tempCP[i]) ] = true;				
					}	
				}
				else if( separador == "-")
				{
					string[] tempCP = strRecorrido.Split( '-' );

					int CP1 = Convert.ToInt32(tempCP[0]);
					int CP2 = Convert.ToInt32(tempCP[1]);

					int noCPNuevos = (CP2 - CP1) + 1;

					for(int i = 0; i < noCPNuevos; i++ )
					{
						recorridos[ CP1 + i ] = true;				
					}
				}
				else
				{
					//gruposControl solo contiene un grupo.
				
					recorridos[ Convert.ToInt32( strRecorrido ) ] = true;
				}
			}
		
		}

		protected string EncuentraSeparador( string values )
		{
			string separador = "";
			
			foreach(char c in values)
			{
				switch( c )
				{
					case ',':
						separador = ",";
						break;
					case '-':
						separador = "-";
						break;
				}
				
				if( separador != "" )
				{
					break;
				}
			}
			
			return separador;
		}

		public string Columnas
		{
			get{ return strColumnas; }
			set
			{ 
				strColumnas = value;
				InicializarColumnasRelacionadas();
			}
		}

		public string SumaAciertos
		{
			get{ return strSumas; }
			set
			{ 
				strSumas = value; 
				InicializarSumas();				
			}
		}

		public string Recorridos
		{
			get{ return strRecorrido; }
			set
			{ 
				strRecorrido = value; 
				InicializarRecorridos();				
			}
		}

		public string CantidadCP
		{
			get{ return strCantidadCP;}
			set
			{ 
				strCantidadCP = value;
			
				if(strCantidadCP != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					cantidadCP = rangosHelper.ObtenBoolArray(strCantidadCP);
				}
				else
				{
					cantidadCP = null;
				}
			
			}		
		}
		
		public string CuantosAC
		{
			get{ return strCuantosAC;}
			set
			{ 
				strCuantosAC = value;
				
				if(strCuantosAC != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					cuantosAC = rangosHelper.ObtenBoolArray(strCuantosAC);
				}
				else
				{
					cuantosAC = null;
				}
			
			}			
		}

		public ControladorRelacionesCP1 Controlador
		{
			set
			{ 
				controlador = value;
				columnasProbables = controlador.ColumnasProbables;
			} 
		}

	}
}
