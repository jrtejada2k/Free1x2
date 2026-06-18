// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
	public class FiltroSignosSeguidos: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        protected List<long> figuras;
        protected List<long> figurasV = new List<long>();
        protected List<long> figuras1 = new List<long>();
        protected List<long> figurasX = new List<long>();
        protected List<long> figuras2 = new List<long>();
	
        
        //contadores columna
		private int noVarSeguidosCol;
		private int noUnosSeguidosCol;
		private int noEquisSeguidosCol;
		private int noDosesSeguidosCol;
		
		//condiciones 
		private int noPartidos;
		//standard
		private bool[] variantes;
		private bool[] unos;
		private bool[] equis;
		private bool[] doses;
		//tolerancias
		private char[] variantesTol;
		private char[] unosTol;
		private char[] equisTol;
		private char[] dosesTol;
				

	    public FiltroSignosSeguidos()
		{
			//Este valor podria ser configurable
            noPartidos = VariablesGlobales.NumeroPartidos;
			
			variantes = new bool[noPartidos+1];
			unos = new bool[noPartidos+1];
			equis = new bool[noPartidos+1];
			doses = new bool[noPartidos+1];
			
			variantesTol = new char[noPartidos+1];
			unosTol = new char[noPartidos+1];
			equisTol = new char[noPartidos+1];
			dosesTol = new char[noPartidos+1];
			
			for( int i = 0; i < noPartidos+1; i++ )
			{
				variantes[i] = false;
				unos[i] = false;
				equis[i] = false;

				doses[i] = false;
				
				variantesTol[i]  = ' ';
				unosTol[i]  = ' ';
				equisTol[i]  = ' ';
				dosesTol[i]  = ' ';
			}		
		}
		
		
		private void InicializaContadores()
		{
			noVarSeguidosCol = 0;
			noUnosSeguidosCol = 0;
			noEquisSeguidosCol = 0;
			noDosesSeguidosCol = 0;		
		}
		
		private bool EsColumnaValida()
		{
            return (variantes[noVarSeguidosCol] && unos[ noUnosSeguidosCol ] && equis[ noEquisSeguidosCol ] && doses[ noDosesSeguidosCol ]);
		}		
		
		private void EsColumnaValida(ref string[] arrayFallos,long columna)
		{
			string texto="";
			if( variantes[ noVarSeguidosCol ] == false)
				texto+="Fallo en variantes seguidas  ("+noVarSeguidosCol+")#";
			if( variantesTol[ noVarSeguidosCol ] !=' ')
				texto+="!Tolerancia  en variantes seguidas  ("+noVarSeguidosCol+")#";
			if( unos[ noUnosSeguidosCol ] == false)
				texto+="Fallo en unos seguidos  ("+noUnosSeguidosCol+")#";
			if( unosTol[ noUnosSeguidosCol ] !=' ')
				texto+="!Tolerancia en unos seguidos  ("+noUnosSeguidosCol+")#";
			if( equis[ noEquisSeguidosCol ] == false )
				texto+="Fallo en equis seguidas  ("+noEquisSeguidosCol+")#";
			if( equisTol[ noEquisSeguidosCol ] !=' ')
				texto+="!Tolerancia en equis seguidas  ("+noEquisSeguidosCol+")#";
			if( doses[ noDosesSeguidosCol ] == false )
				texto+="Fallo en doses seguidos  ("+noDosesSeguidosCol+")#";
			if( dosesTol[ noDosesSeguidosCol ] !=' ')
				texto+="!Tolerancia en doses seguidos  ("+noDosesSeguidosCol+")#";
            if (!AnalizaFigurasV(columna))
            {
                long fig = ObtenFiguraV(columna);
                string figTxt = "";
                if (fig != 0) figTxt = Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(fig);
                else figTxt = "0";
                texto += "Fallo en Figuras de Variantes  (" + figTxt + ")#";
            }
            if (!AnalizaFiguras1(columna))
            {
                long fig = ObtenFigura1(columna);
                string figTxt;
                if (fig != 0) figTxt = Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(fig);
                else figTxt = "0";
                texto += "Fallo en Figuras de Unos  (" + figTxt + ")#";
            }
            if (!AnalizaFigurasX(columna))
            {
                long fig = ObtenFiguraX(columna);
                string figTxt = "";
                if (fig != 0) figTxt = Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(fig);
                else figTxt = "0";
                texto += "Fallo en Figuras de Equis  (" + figTxt + ")#";
            } 
            if (!AnalizaFiguras2(columna))
            {
                long fig = ObtenFigura2(columna);
                string figTxt = "";
                if (fig != 0) figTxt = Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(fig);
                else figTxt = "0";
                texto += "Fallo en Figuras de Doses  (" + figTxt + ")#";
            }
            if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
		}

        private bool CumpleCondiciones(long columna)
        {
            if (EsColumnaValida())
            {
                return AnalizaFiguras(columna);
            }
            return false;
        }


	    private void AnalizaColumna(long columna)
        {
            int tempVarSeguidas = 0;
            int temp1Seguidas = 0;
            int tempXSeguidas = 0;
            int temp2Seguidas = 0;

            while (columna !=0)
            {
                switch (columna & 7)
                {
                    case 4: //'1'
                        temp1Seguidas++;

                        tempVarSeguidas = 0;
                        tempXSeguidas = 0;
                        temp2Seguidas = 0;
                        break;
                    case 2: //'X'
                        tempXSeguidas++;
                        tempVarSeguidas++;

                        temp1Seguidas = 0;
                        temp2Seguidas = 0;
                        break;
                    case 1: //'2'
                        temp2Seguidas++;
                        tempVarSeguidas++;

                        temp1Seguidas = 0;
                        tempXSeguidas = 0;
                        break;
                }

                //update var seguidas if temp is greater
                if (noVarSeguidosCol < tempVarSeguidas)
                {
                    noVarSeguidosCol = tempVarSeguidas;
                }

                if (noUnosSeguidosCol < temp1Seguidas)
                {
                    noUnosSeguidosCol = temp1Seguidas;
                }

                if (noEquisSeguidosCol < tempXSeguidas)
                {
                    noEquisSeguidosCol = tempXSeguidas;
                }

                if (noDosesSeguidosCol < temp2Seguidas)
                {
                    noDosesSeguidosCol = temp2Seguidas;
                }
                columna >>= 3;
            }
        }		
		#region interface methods

		public bool Analizar(long columna)
		{
			InicializaContadores();
			AnalizaColumna(columna);
			return CumpleCondiciones(columna);
		}
        #region Métodos Obtención de Figuras
        public long ObtenFiguraV(long columna)
        {
            long temporal = 0;
            columna &= 120632132875995;
            List<int> valores = new List<int>();
            int cuantas = 0;

            while (columna != 0)
            {
                if ((columna & 7) != 0)
                {
                    cuantas++;
                }
                else
                {
                    if (cuantas > 0)
                    {
                        valores.Add(cuantas);
                        cuantas = 0;
                    }
                }
                columna >>= 3;
            }
            if (cuantas > 0)
            {
                valores.Add(cuantas);
            }           
            valores.Sort();
            for (int i = valores.Count - 1; i >= 0; i--)
            {
                temporal <<= 4;
                temporal |= (uint)valores[i];
            }
            return temporal;
        }
        public long ObtenFigura1(long columna)
        {
            long temporal = 0;
            columna &= 160842843834660;
            List<int> valores = new List<int>();
            int cuantas = 0;

            while (columna != 0)
            {
                if ((columna & 7) != 0)
                {
                    cuantas++;
                }
                else
                {
                    if (cuantas > 0)
                    {
                        valores.Add(cuantas);
                        cuantas = 0;
                    }
                }
                columna >>= 3;
            }
            if (cuantas > 0)
            {
                valores.Add(cuantas);
            }
            valores.Sort();
            for (int i = valores.Count - 1; i >= 0; i--)
            {
                temporal <<= 4;
                temporal |= (uint)valores[i];
            }
            return temporal;
        }
        public long ObtenFiguraX(long columna)
        {
            long temporal = 0;
            columna &= 80421421917330;
            List<int> valores = new List<int>();
            int cuantas = 0;

            while (columna != 0)
            {
                if ((columna & 7) != 0)
                {
                    cuantas++;
                }
                else
                {
                    if (cuantas > 0)
                    {
                        valores.Add(cuantas);
                        cuantas = 0;
                    }
                }
                columna >>= 3;
            }
            if (cuantas > 0)
            {
                valores.Add(cuantas);
            }
            valores.Sort();
            for (int i = valores.Count - 1; i >= 0; i--)
            {
                temporal <<= 4;
                temporal |= (uint)valores[i];
            }
            return temporal;
        }
        public long ObtenFigura2(long columna)
        {
            long temporal = 0;
            columna &= 40210710958665;
            List<int> valores = new List<int>();
            int cuantas = 0;

            while (columna != 0)
            {
                if ((columna & 7) != 0)
                {
                    cuantas++;
                }
                else
                {
                    if (cuantas > 0)
                    {
                        valores.Add(cuantas);
                        cuantas = 0;
                    }
                }
                columna >>= 3;
            }
            if (cuantas > 0)
            {
                valores.Add(cuantas);
            }
            valores.Sort();
            for (int i = valores.Count - 1; i >= 0; i--)
            {
                temporal <<= 4;
                temporal |= (uint)valores[i];
            }
            return temporal;
        } 
        #endregion

        public string[] AnalizarFallos(long columna)
		{
			string[] arrayFallos=null;
			InicializaContadores();
			AnalizaColumna(columna);
			EsColumnaValida(ref arrayFallos, columna);
			return arrayFallos;
		}


		public Filtro NombreFiltro
		{
			get{ return Filtro.SignosSeguidos; }
		}
		
		public bool IsActive
		{
			get{ return isActive; } 
			set{ isActive = value; }
		}
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			int noAciertosTols = 0;
			
			//formato letrasTolerancias es: "ABC", "A", "CD", etc...
			
			foreach( char letra in letrasTolerancias)
			{
				if( variantesTol[ noVarSeguidosCol ] == letra)
				{					
					noAciertosTols++;		
				}
				if( unosTol[ noUnosSeguidosCol ] == letra)
				{					
					noAciertosTols++;		
				}
				if( equisTol[ noEquisSeguidosCol ] == letra)
				{					
					noAciertosTols++;		
				}
				if( dosesTol[ noDosesSeguidosCol ] == letra)
				{					
					noAciertosTols++;		
				}				
				
			}			
		
			return noAciertosTols;
		}
		
		#endregion
        #region Métodos de análisis de figuras
        protected bool AnalizaFigurasV(long columna)
        {
            if (FigurasV.Count > 0)
            {
                return figurasV.Contains(ObtenFiguraV(columna));
            }
            return true;
        }

	    protected bool AnalizaFiguras1(long columna)
	    {
	        if (Figuras1.Count > 0)
            {
                return Figuras1.Contains(ObtenFigura1(columna));
            }
	        return true;
	    }

	    protected bool AnalizaFigurasX(long columna)
	    {
	        if (FigurasX.Count > 0)
            {
                return FigurasX.Contains(ObtenFiguraX(columna));
            }
	        return true;
	    }

	    protected bool AnalizaFiguras2(long columna)
	    {
	        if (Figuras2.Count > 0)
            {
                return Figuras2.Contains(ObtenFigura2(columna));
            }
	        return true;
	    }

	    protected bool AnalizaFiguras(long columna)
	    {
	        if (UsaFiguras())
            {
                return (AnalizaFigurasV(columna) && AnalizaFiguras1(columna) && AnalizaFigurasX(columna) && AnalizaFiguras2(columna));
            }
	        return true;
	    }

	    #endregion
        public void ReinicializaValores()
		{
			for( int i = 0; i < variantes.Length; i++ )
			{
				variantes[i] = false;
				unos[i] = false;
				equis[i] = false;
				doses[i] = false;
				
				variantesTol[i] = ' ';
				unosTol[i] = ' ';
				equisTol[i] = ' ';
				dosesTol[i] = ' ';
			}			
		}
				
		
		public void SetNoVariantes(string valores)
		{
			string[] strValores = valores.Split(',');
			
			int valorNumerico;
			string letraTol;
			
			foreach(string str in strValores)
			{
				if(Char.IsLetter(str[str.Length-1]))
				{
					//valor en tolerancias
					valorNumerico = Convert.ToInt32( str.Substring(0,str.Length-1) );
					letraTol = str.Substring(str.Length-1,1);
				}
				else
				{
					valorNumerico = Convert.ToInt32(str);
					letraTol = " ";
				}
				
				variantes[valorNumerico] = true;
				variantesTol[valorNumerico] = letraTol[0];
			}			
		}
				
		public void SetNoUnos(string valores)
		{
			string[] strValores = valores.Split(',');
			
			int valorNumerico;
			string letraTol;
			
			foreach(string str in strValores)
			{
				if(Char.IsLetter(str[str.Length-1]))
				{
					//valor en tolerancias
					valorNumerico = Convert.ToInt32( str.Substring(0,str.Length-1) );
					letraTol = str.Substring(str.Length-1,1);
				}
				else
				{
					valorNumerico = Convert.ToInt32(str);
					letraTol = " ";
				}
				
				unos[valorNumerico] = true;
				unosTol[valorNumerico] = letraTol[0];
			}			
		}				
		
		public void SetNoEquis(string valores)
		{
			string[] strValores = valores.Split(',');
						
			int valorNumerico;
			string letraTol;
			
			foreach(string str in strValores)
			{							
				if(Char.IsLetter(str[str.Length-1]))
				{
					//valor en tolerancias
					valorNumerico = Convert.ToInt32( str.Substring(0,str.Length-1) );
					letraTol = str.Substring(str.Length-1,1);
				}
				else
				{
					valorNumerico = Convert.ToInt32(str);
					letraTol = " ";
				}
				
				equis[valorNumerico] = true;
				equisTol[valorNumerico] = letraTol[0];
			}		
		}		
				
		public void SetNoDoses(string valores)
		{
			string[] strValores = valores.Split(',');
			
			int valorNumerico;
			string letraTol;
			
			foreach(string str in strValores)
			{							
				if(Char.IsLetter(str[str.Length-1]))
				{
					//valor en tolerancias
					valorNumerico = Convert.ToInt32( str.Substring(0,str.Length-1) );
					letraTol = str.Substring(str.Length-1,1);
				}
				else
				{
					valorNumerico = Convert.ToInt32(str);
					letraTol = " ";
				}
				
				doses[valorNumerico] = true;
				dosesTol[valorNumerico] = letraTol[0];
			}			
		}		
		
		public string GetVariantes()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in variantes )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (variantesTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
				
		public string GetUnos()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in unos )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (unosTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
				
		public string GetEquis()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in equis )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (equisTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
				
		public string GetDoses()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in doses )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter+ (dosesTol[counter].ToString()).Trim();
				}
				
				counter++;
			}	
		
			return valores;
		}
			
		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.SignosSeguidos.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public void LlenarTodosValores()
		{
			// Pone como True todos aquellos valores que no tengan ningún valor asignado
			string todosValores="";
			for(int i=0;i<=VariablesGlobales.NumeroPartidos;i++)
			{
				todosValores+=","+i;
			}
			todosValores=todosValores.Substring(1);
			if(hayValores(unos)==false) SetNoUnos(todosValores);
			if(hayValores(variantes)==false) SetNoVariantes(todosValores);
			if(hayValores(equis)==false) SetNoEquis(todosValores);
			if(hayValores(doses)==false) SetNoDoses(todosValores);
		}
			
		private bool hayValores(bool[] propiedad)
		{
			for(int i=0;i<propiedad.Length;i++)
			{
				if(propiedad[i]) return true;
			}
			return false;
		}
			
		public bool ContieneDatos
		{
			get { return contieneDatos; }
			set { contieneDatos = value; }		
		}

        public int NoVariantesSeguidas
        {
            get { return noVarSeguidosCol; }
        }
        public int NoUnosSeguidos
        {
            get { return noUnosSeguidosCol; }
        }
        public int NoEquisSeguidas
        {
            get { return noEquisSeguidosCol; }
        }
        public int NoDosesSeguidos
        {
            get { return noDosesSeguidosCol; }
        }
        public List<long> FigurasV
        {
            get { return figurasV; }
            set { figurasV = value; }
        }
        public List<long> Figuras1
        {
            get { return figuras1; }
            set { figuras1 = value; }
        }
        public List<long> FigurasX
        {
            get { return figurasX; }
            set { figurasX = value; }
        }
        public List<long> Figuras2
        {
            get { return figuras2; }
            set { figuras2 = value; }
        }


        #region Miembros de IFiltro


        public bool UsaFiguras()
        {
            return (FigurasV.Count != 0 || Figuras1.Count != 0 || FigurasX.Count != 0 || Figuras2.Count != 0);         
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
        #endregion

        #region Miembros de IFiltro


        public bool AnalisisActivo
        {
            get { return VariablesGlobales.AnalizarSeguidos; }
        }

        #endregion
    }
}

