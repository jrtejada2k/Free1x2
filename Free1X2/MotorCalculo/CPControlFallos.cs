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
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for CPControlFallos.
	/// </summary>
	public class CPControlFallos
	{
		private string strColumnas = "";
		private string strTolerancias = "";
		private string strAciertos = "";
		private int noToleranciasAcumulados;

		private int[] columnasRelacionadas;
		private bool[] aciertosPermitidos;

		//el controlador es el objeto parent que contiene a este objeto
		protected ControladorCPControlFallos controlador;

		protected List<ColumnaProbable> columnasProbables;

	    public bool Analiza()
		{
			bool controlValido = true;

			noToleranciasAcumulados = 0;

	        for(int i = 0; i < columnasRelacionadas.Length; i++  )
	        {
	            ColumnaProbable cp = columnasProbables[ columnasRelacionadas[i]-1 ];
	            noToleranciasAcumulados += cp.ObtenNoAciertosTolerancias( strTolerancias );
	        }

	        //si se sale del rango condicion fallada.
			if(aciertosPermitidos.Length -1 < noToleranciasAcumulados)
			{
				controlValido = false;
			}			
			else if(aciertosPermitidos[ noToleranciasAcumulados ] == false)
			{
				controlValido = false;			
			}				

			return controlValido;
		}

		protected void InicializarColumnas()
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

		protected void InicializarAciertos()
		{
			string separador = EncuentraSeparador( strAciertos );
			
			if( separador == "," )
			{
				string[] tempAciertos = strAciertos.Split( ',' );
				
				//Crear array de maximo numero de aciertos + 2
				int noAciertosMax = Convert.ToInt32( tempAciertos[tempAciertos.Length -1] );
				noAciertosMax += 2;
			
				aciertosPermitidos = new bool[ noAciertosMax ];
			
				for(int i = 0; i < tempAciertos.Length; i++)
				{
					aciertosPermitidos[ Convert.ToInt32(tempAciertos[i]) ] = true;				
				}			
				
			}
			else if( separador == "-")
			{
				string[] tempAciertos = strAciertos.Split( '-' );
				
				//Crear array de maximo numero de aciertos + 2
				int noAciertosMax = Convert.ToInt32( tempAciertos[1] );
				noAciertosMax += 2;
				
				aciertosPermitidos = new bool[ noAciertosMax ];
				
				int grupo1 = Convert.ToInt32(tempAciertos[0]);
				int grupo2 = Convert.ToInt32(tempAciertos[1]);
								
				int noGruposNuevos = (grupo2 - grupo1) + 1;				
				
				for(int i = 0; i < noGruposNuevos; i++ )
				{
					aciertosPermitidos[ grupo1 + i ] = true;				
				}				
				
			}
			else
			{
				//solo se permite un acierto: "1"
				//Crear array de maximo numero de aciertos + 2
				
				int noAciertos = Convert.ToInt32( strAciertos );
				int noAciertosMax = noAciertos + 2;
				
				aciertosPermitidos = new bool[ noAciertosMax ];
				aciertosPermitidos[ noAciertos ] = true;								
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
				InicializarColumnas();
			}
		}

		public string Aciertos
		{
			get { return strAciertos; }
			set 
			{ 
				strAciertos = value; 
				InicializarAciertos();
			}
		}

		public int ToleranciasAcumuladas
		{
			get { return noToleranciasAcumulados; }
			set { noToleranciasAcumulados = value; }
		}

		public string Tolerancias
		{
			get { return strTolerancias; }
			set { strTolerancias = value.ToUpper(); }
		}

		public ControladorCPControlFallos Controlador
		{
			set
			{ 
				controlador = value;
				columnasProbables = controlador.ColumnasProbables;
			} 
		}
	}
}
