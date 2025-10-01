// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2008 EquipoFree
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

namespace Free1X2.Utils
{
	/// <summary>
	/// Summary description for Pral.
	/// </summary>
	public class Pral
	{
	    public static string Normaliza (string columna) 
		{
			string chval = "12X";
	        columna=columna.ToUpper();
			string xcol = "";

			for (int nr=0; nr<columna.Length; nr++)
			{
			    char ch = columna[nr];
			    if (chval.IndexOf(ch)>=0) xcol+=ch;
			}

	        return xcol;
		} 
	}
}
