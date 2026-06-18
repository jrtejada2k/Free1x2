// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
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

namespace Free1X2.Analisis
{
    public class ContenedorFormatos
    {
        protected int[] aciertosFormatosSignos;
        protected SortedList<int, int> aciertosGlobalesFormatos = new SortedList<int, int>();

        public int[] AciertosFormatosSignos
        {
            get { return aciertosFormatosSignos; }
            set { aciertosFormatosSignos = value; }
        }
        public SortedList<int, int> AciertosGlobalesFormatos
        {
            get { return aciertosGlobalesFormatos; }
            set { aciertosGlobalesFormatos = value; }
        }
    }
}
