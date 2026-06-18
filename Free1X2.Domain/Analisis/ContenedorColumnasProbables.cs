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

namespace Free1X2.Analisis
{
    public class ContenedorColumnasProbables
    {		//condiciones 
        private int noPartidos;


        //para el calculo de cada columna
        private int[] noAC;
        private int[] noACS;
        private int[] noFS;

        public ContenedorColumnasProbables(int partidos)
        {
            noPartidos = partidos;
            Inicializar();
        }
        public void IncrementarAC(int aciertos)
        {
            NoAC[aciertos]++;
        }
        public void IncrementarACS(int aciertos)
        {
            NoACS[aciertos]++;
        }
        public void IncrementarFS(int aciertos)
        {
            NoFS[aciertos]++;
        }
        protected void Inicializar()
        {
            NoAC = new int[noPartidos + 1];
            NoACS = new int[noPartidos +1];
            NoFS = new int[noPartidos + 1];
        }
        public int[] NoAC
        {
            get { return noAC; }
            set { noAC = value; }
        }
        public int[] NoACS
        {
            get { return noACS; }
            set { noACS = value; }
        }
        public int[] NoFS
        {
            get { return noFS; }
            set { noFS = value; }
        }
    }
}
