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

namespace Free1X2.Analisis
{
    public class ContenedorAnalisisGlobal
    {
        protected ContenedorAnalisis analisisGrupos;
        int[] columnasPorFallosDeGrupos;
        int[] columnasPorFallosDeConjuntos;
        private bool esAnalisisExterno;
        protected int noGruposComb;
        protected int noConjuntosComb;

        protected int noPartidosComb;
        public ContenedorAnalisisGlobal(int noGrupos, int noConjuntos, int partidos)
        {
            noPartidosComb = partidos;
            noGruposComb = noGrupos;
            noConjuntosComb = noConjuntos;
            Inicializar();
            columnasPorFallosDeConjuntos = new int[noConjuntos];
            columnasPorFallosDeGrupos = new int[noGrupos];
        }
        protected void Inicializar()
        {
                AnalisisGrupos = new ContenedorAnalisis(noPartidosComb);
        }
        public ContenedorAnalisis AnalisisGrupos
        {
            get { return analisisGrupos; }
            set { analisisGrupos = value; }
        }
        public void VaciarInformacion()
        {
                AnalisisGrupos.FiltrosTemp.Clear();

        }
        public int[] ColumnasPorFallosDeGrupos
        {
            get { return columnasPorFallosDeGrupos; }
            set { columnasPorFallosDeGrupos = value; }
        }
        public int[] ColumnasPorFallosDeConjuntos
        {
            get { return columnasPorFallosDeConjuntos; }
            set { columnasPorFallosDeConjuntos = value; }
        }
        public int NoGrupos
        {
            get { return noGruposComb; }
        }
        public int NoConjuntos
        {
            get { return noConjuntosComb; }
        }
        public bool EsAnalisisExterno
        {
            get { return esAnalisisExterno; }
            set { esAnalisisExterno = value;}
        }

    }
}
