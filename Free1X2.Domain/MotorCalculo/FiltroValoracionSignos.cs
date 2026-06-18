// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 xfsf
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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
	/// Summary description for FiltroValoracionSignos.
	/// </summary>
	public class FiltroValoracionSignos:IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        List<long> figuras;

		private string tipoValoracion = "suma";

		private double[] valores1;
		private double[] valoresX;
		private double[] valores2;

		private double valoracion;
		private double valoracionUnos;
		private double valoracionEquis;
		private double valoracionDoses;
		private string valorGlobal = "";
		private string valorUnos = "";
		private string valorEquis = "";
		private string valorDoses = "";
		private RangosOpciones rangosGlobales;
		private RangosOpciones rangosUnos;
		private RangosOpciones rangosEquis;
		private RangosOpciones rangosDoses;
		
		public FiltroValoracionSignos()
		{
			valores1 = new double[VariablesGlobales.NumeroPartidos];
            valoresX = new double[VariablesGlobales.NumeroPartidos];
            valores2 = new double[VariablesGlobales.NumeroPartidos];

			rangosGlobales = new RangosOpciones();
			rangosUnos = new RangosOpciones();
			rangosEquis = new RangosOpciones();
			rangosDoses = new RangosOpciones();

            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
			{
				valores1[i]= 0;
				valoresX[i]= 0;
				valores2[i]= 0;
			}
		}

		private void InicializaContadores()
		{
			valoracion = 0;		
			valoracionUnos = 0;
			valoracionEquis = 0;
			valoracionDoses = 0;

		}

		private void AnalizaColumna(long columna)
		{
			switch(tipoValoracion)
			{
				case "suma":
					AnalizaPorSumas(columna);
					break;
				case "multiplo":
					AnalizaPorMultiplos(columna);
					break;
			}		
		}
        private void AnalizaPorSumas(long columna)
        {
            valoracion = 0;
            valoracionUnos = 0;
            valoracionEquis = 0;
            valoracionDoses = 0;
            int i = 0;
            while(columna !=0)
            {
                 switch (columna & 7)
                 {
                     case 4: //1
                        valoracion += valores1[i];
                        valoracionUnos += valores1[i];
                         break;
                     case 2://X
                        valoracion += valoresX[i];
                        valoracionEquis += valoresX[i];
                         break;
                     case 1: //2
                        valoracion += valores2[i];
                        valoracionDoses += valores2[i];
                        break;
                }
                columna >>= 3;
                i++;
            }
        }
        private void AnalizaPorMultiplos(long columna)
        {
            valoracion = 1;
            valoracionUnos = 1;
            valoracionEquis = 1;
            valoracionDoses = 1;
            int i = 0;
            while(columna !=0)
            {
                 switch (columna & 7)
                 {
                     case 4: //1
                        valoracion *= valores1[i] * 0.03420425138;
                        valoracionUnos *= valores1[i] * 0.03420425138;
                         break;
                     case 2: //X
                        valoracion *= valoresX[i] * 0.03420425138;
                        valoracionEquis *= valoresX[i] * 0.03420425138;
                         break;
                     case 1: //2
                        valoracion *= valores2[i] * 0.03420425138;
                        valoracionDoses *= valores2[i] * 0.03420425138;
                        break;
                }
                columna >>= 3;
                i++;
            }

            if (valoracion == 1) valoracion = 0;
            if (valoracionUnos == 1) valoracionUnos = 0;
            if (valoracionEquis == 1) valoracionEquis = 0;
            if (valoracionDoses == 1) valoracionDoses = 0;
        }

		private bool EsColumnaValida()
		{
		    if (rangosGlobales.ValorEnRangoValido(valoracion) && rangosUnos.ValorEnRangoValido(valoracionUnos) && rangosEquis.ValorEnRangoValido(valoracionEquis) && rangosDoses.ValorEnRangoValido(valoracionDoses))
			{
				return true;
			}
		    return false;
		}

	    private void EsColumnaValida(ref string[] arrayFallos)
		{
			// El parámetro nulo sólo es para diferenciar del otro método y no se usa
			string texto="";
			if (!rangosGlobales.ValorEnRangoValido(valoracion))
				texto+="Fallo en valoración global  ("+valoracion+")#";
			if (!rangosUnos.ValorEnRangoValido(valoracionUnos))
				texto+="Fallo en valoración de 1  ("+valoracionUnos+")#";
			if (!rangosEquis.ValorEnRangoValido(valoracionEquis))
				texto+="Fallo en valoración de X  ("+valoracionEquis+")#";
			if (!rangosDoses.ValorEnRangoValido(valoracionDoses))
				texto+="Fallo en valoración de 2  ("+valoracionDoses+")#";
			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
		}

		private bool CumpleCondiciones()
		{
		    return EsColumnaValida();
		}

	    #region metodos interface IFiltro
		public bool Analizar(long columna)
		{
			InicializaContadores();
			AnalizaColumna(columna);
			return CumpleCondiciones();
		}
		
		public string[] AnalizarFallos(long columna)
		{
			InicializaContadores();
			AnalizaColumna(columna);
			string[] arrayFallos =null;
			EsColumnaValida(ref arrayFallos);
			return arrayFallos;
		}
		
		public Filtro NombreFiltro
		{
			get{ return Filtro.ValoracionSignos; }
		}
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			//este filtro no usa las tolerancias
			int noAciertosTols = 0;				
		
			return noAciertosTols;
		}
		
		#endregion 

		public void ReinicializaValores()
		{
            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
			{
				valores1[i]= 0;
				valoresX[i]= 0;
				valores2[i]= 0;
			}							
		}

		protected void CalcularRangos(string valores)
		{
			rangosGlobales.ReinicializarRangos();
			if (valores !="") rangosGlobales.PonerRangos( valores );		
		}
		protected void CalcularRangosUnos(string valores)
		{
			rangosUnos.ReinicializarRangos();
			if (valores !="") rangosUnos.PonerRangos( valores );		
		}
		protected void CalcularRangosEquis(string valores)
		{
			rangosEquis.ReinicializarRangos();
			if (valores !="") rangosEquis.PonerRangos( valores );		
		}
		protected void CalcularRangosDoses(string valores)
		{
			rangosDoses.ReinicializarRangos();
			if (valores !="") rangosDoses.PonerRangos( valores );		
		}

		public string ValorGlobal
		{
			get{ return valorGlobal; }
			set
			{
				valorGlobal = value; 
				CalcularRangos( valorGlobal );
			}
		}
		public string ValorUnos
		{
			get{ return valorUnos; }
			set
			{
				valorUnos = value; 
				CalcularRangosUnos( valorUnos );
			}
		}
		public string ValorEquis
		{
			get{ return valorEquis; }
			set
			{
				valorEquis = value; 
				CalcularRangosEquis( valorEquis );
			}
		}
		public string ValorDoses
		{
			get{ return valorDoses; }
			set
			{
				valorDoses = value; 
				CalcularRangosDoses( valorDoses );
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
		

		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.ValoracionSignos.ToString() ) )
			{
				return true;
			}
		    return false;
		}


	    public string TipoValoracion
		{
			get{ return tipoValoracion; }	
			set{ tipoValoracion = value; }
		}


		public double[] Valores1
		{
			get{ return valores1; }
			set{ valores1 = value; }
		}


		public double[] ValoresX
		{
			get{ return valoresX; }
			set{ valoresX = value; }
		}


		public double[] Valores2
		{
			get{ return valores2; }
			set{ valores2 = value; }
		}


		public double ValoracionResultado
		{
			get{ return valoracion; }
		}
        public double ValoracionUnos
        {
            get { return valoracionUnos; }
        }
        public double ValoracionEquis
        {
            get { return valoracionEquis; }
        }
        public double ValoracionDoses
        {
            get { return valoracionDoses; }
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
            get { return VariablesGlobales.AnalizarValoracion; }
        }
        #endregion
    }
}

