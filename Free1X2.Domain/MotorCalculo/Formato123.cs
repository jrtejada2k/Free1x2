// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison [dot] ne [at] gmail [dot] com
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

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for Formato123.
	/// </summary>
	public class Formato123
	{
		string formato = "";
		int aciertosMax, aciertosMin;
		// P-05: caché del formato ya convertido a long. Antes
		// FiltroFormatos123.CumpleCondicionesPasoLibre/PasoFijo llamaban a
		// ConvStrToLong(formato.Formato) por CADA columna y por CADA formato,
		// con un Substring por carácter. Solo depende del texto del formato.
		long formatoLong;
		bool formatoLongCalculado;

	    public string Formato
		{
			get {return formato;}
			set
			{
				formato = value;
				formatoLongCalculado = false;
			}
		}

		/// <summary>
		/// P-05 · El formato convertido a long, calculado una sola vez.
		/// Réplica exacta del ConvStrToLong privado de FiltroFormatos123.
		/// </summary>
		public long FormatoLong
		{
			get
			{
				if(!formatoLongCalculado)
				{
					formatoLong = ConvStrToLong(formato);
					formatoLongCalculado = true;
				}
				return formatoLong;
			}
		}

		private static long ConvStrToLong(string s)
		{
			string signos = "321";
			long res=0;
			for(int i=0;i<s.Length;i++)
			{
				res =(res <<=3) ^ (1<<signos.IndexOf (s.Substring (i,1)));
			}
			return res;
		}
		public int AciertosMax
		{
			get {return aciertosMax;}
			set {aciertosMax = value;}
		}
		public int AciertosMin
		{
			get {return aciertosMin;}
			set {aciertosMin = value;}
		}
	}
}
