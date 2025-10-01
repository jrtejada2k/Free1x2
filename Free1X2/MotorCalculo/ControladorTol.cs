// created on 09/03/2004 at 22:53
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
	public class ControladorTol
	{
		protected List<ToleranciaFiltros> tolerancias;	
		
		protected string strFallosPermitidos = "";
		protected bool[] fallosPermitidos;

		
		public ControladorTol()
		{
            tolerancias = new List<ToleranciaFiltros>();
		}
		
		public void PonerTolerancia(ToleranciaFiltros tol)
		{
			tolerancias.Add( tol );		
		}

		protected void InicializarFallos()
		{
		
			string separador = EncuentraSeparador( strFallosPermitidos );

			if( separador == "," )
			{
				string[] tempCP = strFallosPermitidos.Split( ',' );
					
				int capacidadArray = Convert.ToInt32(tempCP[tempCP.Length -1]) + 2;
				fallosPermitidos = new bool[ capacidadArray ];
				
				for(int i = 0; i < tempCP.Length; i++)
				{
					fallosPermitidos[ Convert.ToInt32(tempCP[i]) ] = true;				
				}	
			}
			else if( separador == "-")
			{
				string[] tempCP = strFallosPermitidos.Split( '-' );

				int CP1 = Convert.ToInt32(tempCP[0]);
				int CP2 = Convert.ToInt32(tempCP[1]);

				int noCPNuevos = (CP2 - CP1) + 1;

				fallosPermitidos = new bool[CP2 + 2];

				for(int i = 0; i < noCPNuevos; i++ )
				{
					fallosPermitidos[ CP1 + i ] = true;			
				}
			}
			else
			{					
				fallosPermitidos = new bool[ Convert.ToInt32( strFallosPermitidos ) + 2];
				fallosPermitidos[ Convert.ToInt32( strFallosPermitidos ) ] = true;
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

		public bool SonFallosPermitidosValidos(int noFallos)
		{
			bool sonFallosValidos = true;


			if(fallosPermitidos.Length <= noFallos)
			{
				//se sale de rango, por lo tanto condicion fallada
				sonFallosValidos = false;
			}	
			else if(fallosPermitidos[ noFallos ] == false)
			{
				sonFallosValidos = false;
			}	
		
			return sonFallosValidos;
		}

		public string FallosPermitidos
		{
			get{ return strFallosPermitidos; }
			set
			{ 
				strFallosPermitidos = value; 

				if(strFallosPermitidos != "")
				{
					InicializarFallos();
				}
			}		
		}

        public List<ToleranciaFiltros> Tolerancias
		{
			get { return tolerancias; }
			set { tolerancias = value; }
		}		
	}
}
