// created on 13/09/2003 at 12:16
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
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

using System.Collections;
using System.Collections.Generic;
using System;

namespace Free1X2.MotorCalculo
{
	public class FiltroDibujos: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        List<long> figuras;

		private int noEquisCol;
		private int noDosesCol;
		
		private ArrayList dibujos = new ArrayList();
		//private Hashtable dibujosHashtable  = new Hashtable();
		private bool[,] dibs = new bool[17,17];
		private void InicializaContadores()
		{
			noEquisCol = 0;
			noDosesCol = 0;			
		}
						
        private void AnalizaColumna(long columna)
        {
            noEquisCol = Utils.UtilColumnas.ContarBitsA1(columna & 80421421917330);
            noDosesCol = Utils.UtilColumnas.ContarBitsA1(columna & 40210710958665);
        }	

		private bool CumpleCondiciones()
		{
            return dibs[noEquisCol, noDosesCol];
		}
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			//este filtro no usa tolerancias.
			return 0;
		}
		
		public bool Analizar(long columna)
		{
			InicializaContadores();
			AnalizaColumna(columna);
			return CumpleCondiciones();
		}
		
		public string[] AnalizarFallos(long columna)
		{
			string[] arrayFallos=null;
			InicializaContadores();
			AnalizaColumna(columna);
			if(CumpleCondiciones()==false)
			{
				string dibujoCol = noEquisCol + "+" + noDosesCol;
				arrayFallos=new string[1];
				arrayFallos[0]="Fallo en número de dibujos  ("+dibujoCol+")";
			}
			return arrayFallos;
		}

		public Filtro NombreFiltro
		{
			get{ return Filtro.Dibujos; }
		}
		
		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.Dibujos.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public string GetDibujos()
		{
			string valores = "";
			int i;
			
			for(i=0;i<Dibujos.Count;i++)
			{
				if( !valores.Equals("") )
				{
					valores += ",";
				}
			    valores += (string) Dibujos[i];
			    valores.Trim();
			}
			return valores;
		}
					
		public ArrayList Dibujos
		{
			get{ return dibujos; }
			set
			{ 
				dibujos = value; 
				
				//comprobar ArrayList contiene valores
				if(dibujos != null && dibujos.Count > 0)
				{
					contieneDatos = true;	
					
					//dibujosHashtable.Clear();
                    dibs = new bool[17, 17];			
					foreach(string str in dibujos)
					{
						//dibujosHashtable.Add( str, str );
	                    string[] valores = str.Split('+');
                        dibs[Convert.ToInt32(valores[0]), Convert.ToInt32(valores[1])] = true;
					}
				}				
			}
		}
		
		public bool IsActive
		{
			get{ return isActive; } 
			set{ isActive = value; }
		}
		
		public bool ContieneDatos
		{
			get { return contieneDatos; }
			set { contieneDatos = value; }		
		}
        public int NoEquisCol
        {
            get { return noEquisCol; }
        }
        public int NoDosesCol
        {
            get { return noDosesCol; }
        }

        #region Miembros de IFiltro


        public bool UsaFiguras()
        {
            if ((Figuras == null) || (Figuras.Count == 0))
            {
                return false;
            }
            return true;
        }

	    public List<long> Figuras
        {
            get
            {
                return figuras;
            }
            set
            {
                figuras = value;
            }
        }
        public bool AnalisisActivo
        {
            get { return VariablesGlobales.AnalizarDibujos; }
        }
        #endregion
    }
}

