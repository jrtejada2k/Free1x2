// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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
    public class ControladorRelacionesCP2
    {
        private List<RelacionCP2> relaciones2 = new List<RelacionCP2>();
        protected List<ColumnaProbable> columnasProbables;

        public bool Analizar()
        {
            for (int i = 0; i < relaciones2.Count; i++)
            {
                if (!relaciones2[i].Analizar())
                {
                    return false;
                }
            }
            
            return true;
        }
        public void Analiza(ref string txt)
        {
            txt = "";
            for (int i = 0; i < Relaciones2.Count; i++)
            {
                RelacionCP2 rel = Relaciones2[i];
                txt += rel.Analiza(i + 1);
            }
        }
        public List<RelacionCP2> Relaciones2
        {
            get { return relaciones2; }
            set { relaciones2 = value; }
        }
        public List<ColumnaProbable> ColumnasProbables
        {
            get { return columnasProbables; }
            set { columnasProbables = value; }
        }
    }
}
