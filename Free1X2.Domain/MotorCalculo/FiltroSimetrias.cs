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
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for FiltroSimetrias.
	/// </summary>
	public class FiltroSimetrias : IFiltro
	{
		private bool isActive;
	    protected List<Simetria> arraySimetrias = new List<Simetria>();
		protected List<int> arrayAciertos = new List<int>();
        protected List<long> figuras;
		protected string aciertosString = "";
        protected int aciertos;

	    public List<Simetria> ArraySimetrias
		{
			get {return arraySimetrias;}
			set {arraySimetrias = value;}
		}
		public List<int> ArrayAciertos
		{
			get {return arrayAciertos;}
			set {arrayAciertos = value;}
		}
		public string Aciertos
		{
			get {return aciertosString;}
			set {aciertosString = value;}
		}
        public int AciertosSimetria
        {
            get { return aciertos; }
            set { aciertos = value; }
        }

		#region IFiltro Members

		public bool Analizar(string columna)
		{
            long columnaLong = UtilColumnas.ConvStrToLong(columna);
			
			return Analizar(columnaLong);
		}
		public bool Analizar(long columna)
		{
		    aciertos = 0;
            for (int i = 0; i < ArraySimetrias.Count; i++)
            {
                Simetria simetria = ArraySimetrias[i];

                //Tomamos el primer signo
                int signo1 = UtilColumnas.ObtenerSignoInt(columna, simetria.PartidosSimetricos[0]);
                bool esValida = true;
                for (int j = 1; j < simetria.PartidosSimetricos.Count; j++)
                {
                    if (signo1 != UtilColumnas.ObtenerSignoInt(columna, simetria.PartidosSimetricos[j]))
                    {
                        esValida = false;
                        break;
                    }
                }

                if (esValida)
                {
                    aciertos++;
                }
            }
		    return ArrayAciertos.IndexOf(aciertos) != -1;
		}
        public string AnalizarFallosComb(long columna)
        {
            bool esValida = false;
            int ac = 0;
            string texto = "";
            for (int i = 0; i < ArraySimetrias.Count; i++)
            {
                Simetria simetria = ArraySimetrias[i];
                if (simetria.PartidosSimetricos.Count > 0)
                {
                    //Tomamos el primer signo
                    esValida = true;
                    int signo = UtilColumnas.ObtenerSignoInt(columna, simetria.PartidosSimetricos[0]);

                    for (int j = 1; j < simetria.PartidosSimetricos.Count; j++)
                    {
                        int signo2 = UtilColumnas.ObtenerSignoInt(columna, simetria.PartidosSimetricos[j]);

                        if (signo != signo2)
                        {
                            esValida = false;
                            break;
                        }
                    }
                }
                if (esValida)
                {
                    ac++;
                }
            }
            if (!ArrayAciertos.Contains(ac))
            {
                texto = "Fallo en Número de Aciertos (" + ac + ")";
            }
            return texto;
        }

		public string[] AnalizarFallos(long columna)
		{
            string[] arrayFallos = null;
		    string texto = AnalizarFallosComb(columna);
            if (texto.Length > 0)
            {
                arrayFallos = texto.Split('#');
            }
            return arrayFallos;
		}

		public bool IsActive
		{
			get{ return isActive; } 
			set{ isActive = value; }
		}

		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.Simetrias.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public Filtro NombreFiltro
		{
			get
			{
				return Filtro.Simetrias;
			}
		}

		public bool ContieneDatos
		{
			get
			{
			    if(ArraySimetrias.Count == 0)
				{
					ArrayAciertos.Clear();
				    return false;
				}
			    return true;
			}
		    set
			{ }
		}

		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			
			return 0;
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
            get { return VariablesGlobales.AnalizarSimetrias; }
        }

        #endregion
    }
}
