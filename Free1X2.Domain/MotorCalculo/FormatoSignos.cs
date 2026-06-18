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

using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for FormatosSignosLinea.
	/// </summary>
	public class FormatoSignos
	{
		protected string formato = "";
        protected long _Longformato;
        protected long _ValMin;
        protected string rangoAparicion = "";
		protected bool[] arrayRangos;
		protected int noApariciones;

	    private void InicializaContadores()
		{
			noApariciones = 0;
		}

		public bool Analiza( long columna )
		{
			InicializaContadores();	
			AnalizaColumna(columna);		
			return CumpleCondiciones();
		}
        protected void AnalizaColumna(long columna)
        {
            while (columna != 0 && columna >= _ValMin)
            {
                if ((columna & _Longformato) == columna) noApariciones++;
                columna >>= 3;
            }
        }

		private bool CumpleCondiciones()
		{
			bool cumpleCondicion = false;

			if( (arrayRangos.Length > noApariciones ) 
				&& arrayRangos[noApariciones])
			{
				cumpleCondicion = true;
			}			

			return cumpleCondicion;
		}

		public string Formato
		{
			get{ return formato;}
			set{ 
                formato = value.ToUpper();
                _Longformato = ConvFormatoToLong(formato);
                _ValMin = (long)1 << ((formato.Length - 1) * 3);
            } 
		}

        private long ConvFormatoToLong(string f)
        {
            long res = ~0;
            for (int i = 0;i < f.Length; i++)
            {
                res = res << 3 | (uint)" 2XV1  *".IndexOf (f[i]);
            }
            return res;
        }


        public string RangoAparicion
		{
			get{ return rangoAparicion;}
			set
			{ 
				rangoAparicion = value;

				if(rangoAparicion != "")
				{				
					RangosHelper rangos = new RangosHelper();
					arrayRangos = rangos.ObtenBoolArray( rangoAparicion );
				}
			}
		}
		public int NoApariciones
		{
			get{ return noApariciones; }
		}
	}
}

