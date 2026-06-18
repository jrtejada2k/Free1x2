// Free1X2 · WinUI 3 — WIN3
// created on 13/03/2004 at 14:58
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

namespace Free1X2.MotorCalculo
{
	public class ToleranciaFiltros
	{
		protected string letrasTol = "";
		
		protected string strAciertos = "";
		private bool[] aciertosPermitidos;
		
		protected int noAciertosAcumulados;

	    public void ReinicializaNoAciertosAcumulados()
		{
			noAciertosAcumulados = 0;
		}
		
		public void SumaAciertosAcumulados( int noAciertos )
		{
			noAciertosAcumulados += noAciertos;
		}
		
		public bool CumpleTolerancia()
		{
			bool toleranciaValida = true;
			
			//comprobar que numero de aciertos no se salga del rango controlado
			//si se sale la tolerancia esta fallada directamente
			if(aciertosPermitidos.Length -1 < noAciertosAcumulados)
			{
				toleranciaValida = false;
			}			
			else if(aciertosPermitidos[ noAciertosAcumulados ] == false)
			{
				toleranciaValida = false;			
			}			
			
			return toleranciaValida;			
		}
		
		protected void InicializaArrayAciertos()
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
					//salir del bucle
					break;
				}
			}
			
			return separador;
		}

		
		public int NoAciertosAcumulados
		{
			get{ return noAciertosAcumulados; }
		}
		
		public string LetrasTol
		{
			get{ return letrasTol; }
			set{ letrasTol = value.ToUpper(); }
		}
		
		public string Aciertos
		{
			get{ return strAciertos; }
			set
			{
					strAciertos = value;
					InicializaArrayAciertos();
			}
		}

	}
}
