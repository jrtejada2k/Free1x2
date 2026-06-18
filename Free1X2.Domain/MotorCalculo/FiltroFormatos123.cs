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

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for FiltroFormatos123.
	/// </summary>
	public class FiltroFormatos123 : IFiltro
	{
		private bool isActive;
	    protected List<Formato123> arrayFormatos = new List<Formato123>();
		protected List<int> arrayAciertos = new List<int>();
        protected byte[,] valoresTransformados = new byte[VariablesGlobales.NumeroPartidos, 3];
        protected double[,] valores = new double[VariablesGlobales.NumeroPartidos, 3];
		protected bool pasoFijo;
        protected string aciertosFiltro;
		int aciertosMax;
		int aciertosMin;
        List<long> figuras;

	    public double[,] Valores
        {
            get { return valores; }
            set { valores = value; }
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
        public string AciertosFiltro
        {
            get { return aciertosFiltro; }
            set { aciertosFiltro = value; }
        }
        public List<Formato123> ArrayFormatos
		{
			get {return arrayFormatos;}
			set {arrayFormatos = value;}
		}
        public List<int> ArrayAciertos
		{
			get {return arrayAciertos;}
			set {arrayAciertos = value;}
		}

		public bool PasoFijo
		{
			get {return pasoFijo;}
			set{pasoFijo = value;}
		}
		public bool NecesitaGuardar()
		{
		    if((ArrayFormatos.Count == 0)&&(ArrayAciertos.Count == 0))
			{
				return false;
			}
		    return true;
		}

	    public byte[,] ValoracionTranformada
		{
			get {return valoresTransformados;}
			set {valoresTransformados = value;}
		}
		public double[,] Valoracion
		{
			get {return valores;}
			set {valores = value;}
		}
		public string Col1X2ToCol123(string columna)
		{
			return TraducirColumna(columna.ToUpper(), TransformarValoracion(Valores) );
		}
		protected bool AnalizarColumna(long columna)
		{
		    if(PasoFijo)
			{
				return CumpleCondicionesPasoFijo(TraducirLongColumnaALong123(columna));
			}
		    return CumpleCondicionesPasoLibre(TraducirLongColumnaALong123(columna));
		}

	    protected string AnalizarFallosComb(long columna)
	    {
	        if (PasoFijo)
            {
                return AnalizarFallosCombPasoFijo(TraducirLongColumnaALong123(columna));
            }
	        return AnalizarFallosCombPasoLibre(TraducirLongColumnaALong123(columna));
	    }

	    protected bool CumpleCondicionesPasoLibre(long columnaFormato)
		{
			long columnaFormatoTemp = columnaFormato;
	        int aciertosGlobales = 0;


	        for(int i = 0; i < ArrayFormatos.Count; i++)
			{
				Formato123 formato = ArrayFormatos[i];
				int aciertos = 0;
				long formato123 = ConvStrToLong(formato.Formato);
				
				while (columnaFormatoTemp != 0 )
				{
					if (((columnaFormatoTemp) & formato123) == formato123)
					{
						aciertos++;
					}
					columnaFormatoTemp >>= 3;
				} 

				if((aciertos >= formato.AciertosMin)&&(aciertos <= formato.AciertosMax)&&(aciertosGlobales <= AciertosMax))
				{
                    //Columna en límites, es un acierto global
                    aciertosGlobales++;
				}
				else
				{
                    if (aciertosGlobales > AciertosMax)
                    {
                        //Se ha pasado de aciertos, hacemos un break y salimos del for
                        //y pasamos a la siguiente columna
                        break;
                    }

				}
				columnaFormatoTemp = columnaFormato;
			}
			return ArrayAciertos.Contains(aciertosGlobales);
			
		}
		
		protected bool CumpleCondicionesPasoFijo(long columnaFormato)
		{
			long columnaFormatoTemp = columnaFormato;
			int aciertosGlobales = 0;
		    for(int i = 0; i < ArrayFormatos.Count; i++)
			{
				//Formato123 formato = ArrayFormatos[i];
                long formato123 = ConvStrToLong(ArrayFormatos[i].Formato);
				while (columnaFormatoTemp != 0 )
				{
					if ((columnaFormatoTemp & formato123) == formato123)
					{
						aciertosGlobales++;
						break;
					}
					columnaFormatoTemp >>= 3;
				}
				columnaFormatoTemp = columnaFormato;
			}

			return ArrayAciertos.Contains(aciertosGlobales);
		}
        protected string AnalizarFallosCombPasoLibre(long columnaFormato)
        {
            long columnaFormatoTemp = columnaFormato;
            int aciertosGlobales = 0;
            string texto = "";


            for (int i = 0; i < ArrayFormatos.Count; i++)
            {
                Formato123 formato = ArrayFormatos[i];
                int aciertos = 0;
                long formato123 = ConvStrToLong(formato.Formato);

                while (columnaFormatoTemp != 0)
                {
                    if (((columnaFormatoTemp) & formato123) == formato123)
                    {
                        aciertos++;
                    }
                    columnaFormatoTemp >>= 3;
                }

                if ((aciertos >= formato.AciertosMin) && (aciertos <= formato.AciertosMax) && (aciertosGlobales <= AciertosMax))
                {
                    //Columna en límites, es un acierto global
                    aciertosGlobales++;
                }
                else
                {
                    texto = "Columna Fuera de Límites (Formato " + formato.Formato + " Apariciones: " + aciertos+")";
                    if (aciertosGlobales > AciertosMax)
                    {
                        //Se ha pasado de aciertos, hacemos un break y salimos del for
                        //y pasamos a la siguiente columna
                        break;
                    }

                }
                columnaFormatoTemp = columnaFormato;
            }
            if (!ArrayAciertos.Contains(aciertosGlobales))
            {
                return "Fallo en Líneas Acertadas (" + aciertosGlobales + ")";
            }
            return texto;
        }

        protected string AnalizarFallosCombPasoFijo(long columnaFormato)
        {
            long columnaFormatoTemp = columnaFormato;
            int aciertosGlobales = 0;
            string texto = "";

            for (int i = 0; i < ArrayFormatos.Count; i++)
            {
                Formato123 formato = ArrayFormatos[i];
                long formato123 = ConvStrToLong(formato.Formato);
                while (columnaFormatoTemp != 0)
                {
                    if (((columnaFormatoTemp) & formato123) == formato123)
                    {
                        aciertosGlobales++;
                        break;
                    }
                    columnaFormatoTemp >>= 3;
                }
                columnaFormatoTemp = columnaFormato;
            }

            if (!ArrayAciertos.Contains(aciertosGlobales))
            {
                texto = "Fallo en Aciertos Globales (" + aciertosGlobales+")";
            }
            return texto;
        }
		private long ConvStrToLong  (string s)
		{
			string signos = "321";
			long res=0;
			for(int i=0;i<s.Length;i++)
			{
				res =(res <<=3) ^ (1<<signos.IndexOf (s.Substring (i,1)));
			}
			return res;
		}
        protected long TraducirLongColumnaALong123(long columna)
        {
            long columnaAFormato = 0;
            int i = 0;
            while ((columna != 0) && (i < VariablesGlobales.NumeroPartidos))
            {
                switch (columna & 7)
                {
                    case 4: //'1'
                        columnaAFormato = columnaAFormato << 3 | ValoracionTranformada[i, 0];
                        break;
                    case 2: //'X'
                        columnaAFormato = columnaAFormato << 3 | ValoracionTranformada[i, 1];
                        break;
                    case 1: //'2'
                        columnaAFormato = columnaAFormato << 3 | ValoracionTranformada[i, 2];
                        break;
                }
                columna >>= 3;
                i++;
            }
            return columnaAFormato;
        }
		protected string TraducirColumna(string columna)
		{
			string columnaAFormato = "";
			for(int i = 0; i < columna.Length; i++)
			{
				if(columna[i] == '1')
				{
					columnaAFormato += ValoracionTranformada[i,0];
				}
				else if(columna[i] == 'X')
				{
					columnaAFormato += ValoracionTranformada[i,1];
				}
				else
				{
					columnaAFormato += ValoracionTranformada[i,2];
				}
			}
			
			return columnaAFormato;
		}
        protected string TraducirColumna(string columna, string[,] val)
        {
            string columnaAFormato = "";
            for (int i = 0; i < columna.Length; i++)
            {
                if (columna[i] == '1')
                {
                    columnaAFormato += val[i, 0];
                }
                else if (columna[i] == 'X')
                {
                    columnaAFormato += val[i, 1];
                }
                else
                {
                    columnaAFormato += val[i, 2];
                }
            }

            return columnaAFormato;
        }
        protected string[,] TransformarValoracion(double[,] valoracion)
        {
            string[,] valoresTransformados = new string[VariablesGlobales.NumeroPartidos, 3];
            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                double[] valor = { valoracion[i, 0], valoracion[i, 1], valoracion[i, 2] };
                if ((valor[0] >= valor[1]) && (valor[0] >= valor[2]))
                {
                    if (valor[1] >= valor[2])
                    {
                        valoresTransformados[i, 0] = "1";
                        valoresTransformados[i, 1] = "2";
                        valoresTransformados[i, 2] = "3";
                    }
                    else if (valor[2] > valor[1])
                    {
                        valoresTransformados[i, 0] = "1";
                        valoresTransformados[i, 1] = "3";
                        valoresTransformados[i, 2] = "2";
                    }
                }
                else if ((valor[1] > valor[0]) && (valor[1] >= valor[2]))
                {
                    if (valor[0] >= valor[2])
                    {
                        valoresTransformados[i, 0] = "2";
                        valoresTransformados[i, 1] = "1";
                        valoresTransformados[i, 2] = "3";
                    }
                    else
                    {
                        valoresTransformados[i, 0] = "3";
                        valoresTransformados[i, 1] = "1";
                        valoresTransformados[i, 2] = "2";
                    }
                }
                else if ((valor[2] > valor[0]) && (valor[2] > valor[1]))
                {
                    if (valor[0] >= valor[1])
                    {
                        valoresTransformados[i, 0] = "2";
                        valoresTransformados[i, 1] = "3";
                        valoresTransformados[i, 2] = "1";

                    }
                    else
                    {
                        valoresTransformados[i, 0] = "3";
                        valoresTransformados[i, 1] = "2";
                        valoresTransformados[i, 2] = "1";
                    }
                }
            } return valoresTransformados;
        }



		#region IFiltro Members

		public bool Analizar(long columna)
		{
			return AnalizarColumna(columna);
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
		    if( nombre.Equals( Filtro.Formatos123.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public Filtro NombreFiltro
		{
			get
			{
				return Filtro.Formatos123;
			}
		}

		public bool ContieneDatos
		{
			get { return NecesitaGuardar(); }
			set { }
		}

		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			//Pasando de tolerancias por ahora... ;)
			return 0;
		}

		#endregion

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
            get { return VariablesGlobales.AnalizarFormatos123; }
        }
        #endregion
    }
}

