// Free1X2 · WinUI 3 — WIN3
// Copyright (C) 2005 Luis Fernandez - luifer [at] onetel [dot] com
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

using System.Collections.Generic;


namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for FiltroFormatos.
	/// </summary>
	public class FiltroFormatosSignos: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        List<long> figuras;
        protected int noLineasValidas;
        protected int noAparicionesGlobal;

		private List<FormatosSignos> arrayFormatos;

		public FiltroFormatosSignos()
		{
			arrayFormatos = new List<FormatosSignos>();				
		}

		public bool Analizar(long columna)
		{			
			bool columnaValida = true;

		    long colSinAsteriscos = 0;
            while (columna != 0)
            {
                byte signo1 = (byte)(columna & 7);
                if (signo1 != 0) colSinAsteriscos = colSinAsteriscos << 3 | signo1;
                columna >>= 3;
            }
			foreach( FormatosSignos formatos in arrayFormatos )
			{
                columnaValida = formatos.Analizar(colSinAsteriscos);
                
				if( columnaValida == false )
				{
					//do not check any more groups
					break;
				}
                noLineasValidas = formatos.NoLineasValidas;
                noAparicionesGlobal = formatos.NoAparicionesGlobal;
			}
		    
			return columnaValida;
		}
		
		public string[] AnalizarFallos(long columna)
		{
			string[] arrayFallos=null;
			int numGrupo=0;
            long colSinAsteriscos = 0;
		    string texto="";
			//elimina asteriscos de partidos no seleccionados en grupo
            while (columna != 0)
            {
                byte signo1 = (byte)(columna & 7);
                if (signo1 != 0) colSinAsteriscos = colSinAsteriscos << 3 | signo1;
                columna >>= 3;
            }

			foreach( FormatosSignos formatos in arrayFormatos )
			{
				numGrupo++;
				string txt = formatos.Analizar ( colSinAsteriscos, numGrupo );
				texto+=txt;
			}
			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
			return arrayFallos;
		}

		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			//este filtro no usa tolerancias.
			return 0;
		}

		public Filtro NombreFiltro
		{
			get{ return Filtro.FormatosSignos; }
		}

		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.FormatosSignos.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public List<FormatosSignos> FormatosSignos
		{
			get{ return arrayFormatos; }
			set
			{ 
				arrayFormatos = value; 	
											
				//comprobar si ArrayList contiene valores
				if(arrayFormatos != null && arrayFormatos.Count > 0)
				{
					contieneDatos = true;											
				}
				else
				{
					contieneDatos = false;
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
            get { return VariablesGlobales.AnalizarFormatos; }
        }
        public int NoLineasValidas
        {
            get { return noLineasValidas; }
            set { noLineasValidas = value; }
        }
        public int NoAparicionesGlobal
        {
            get { return noAparicionesGlobal; }
            set { noAparicionesGlobal = value; }
        }
	}
}

