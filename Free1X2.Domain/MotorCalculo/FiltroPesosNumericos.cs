// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
	public class FiltroPesosNumericos: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        List<long> figuras = new List<long>();

        FiguraCondicion figuraPesos;

		private int pesoGlobal;
		private int pesoUnos;
		private int pesoVar;
		private int pesoEquis;
		private int pesoDoses;
		
		//arrays con indices a comparar
		private int[] indicesUnos;
		private int[] indicesEquis;
		private int[] indicesDoses;		
		
		private int noPartidos;
		//condiciones:
		//standard
		private bool[] pnGlobal;
		private bool[] pnVariantes;
		private bool[] pnUnos;
		private bool[] pnEquis;
		private bool[] pnDoses;
		//tolerancias
		private bool[] pnGlobalTol;
		private bool[] pnVariantesTol;
		private bool[] pnUnosTol;
		private bool[] pnEquisTol;
		private bool[] pnDosesTol;
				
		private bool[] tolerancias = new bool[6];
		
		private int noFallosActual;
		private bool falloPNGlobalBase;
		private bool falloPNVarBase;
		private bool falloPNUnosBase;
		private bool falloPNEquisBase;
		private bool falloPNDosesBase;

		public FiltroPesosNumericos()
		{
			noPartidos = VariablesGlobales.NumeroPartidos;
			
			//inicializar valores indices a comparar
			indicesUnos = new int[] { 7, 5, 3, 1, 8, 6, 4, 2, 9, 7, 5, 3, 1, 8, 6, 4 };
            indicesEquis = new int[] { 5, 1, 6, 2, 7, 3, 8, 4, 9, 5, 1, 6, 2, 7, 3, 8 };
            indicesDoses = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7 };
			
			//inicializa condiciones
			pnGlobal = new bool[noPartidos+1];
			pnVariantes = new bool[noPartidos+1];
			pnUnos = new bool[noPartidos+1];
			pnEquis = new bool[noPartidos+1];
			pnDoses = new bool[noPartidos+1];
			
			pnGlobalTol = new bool[noPartidos+1];
			pnVariantesTol = new bool[noPartidos+1];
			pnUnosTol = new bool[noPartidos+1];
			pnEquisTol = new bool[noPartidos+1];
			pnDosesTol = new bool[noPartidos+1];
			
			for( int i = 0; i < noPartidos+1; i++ )
			{
				pnGlobal[i] = false;
				pnVariantes[i] = false;
				pnUnos[i] = false;
				pnEquis[i] = false;
				pnDoses[i] = false;
				
				pnGlobalTol[i] = false;
				pnVariantesTol[i] = false;
				pnUnosTol[i] = false;
				pnEquisTol[i] = false;
				pnDosesTol[i] = false;
			}
			
			tolerancias[0] = false;
			tolerancias[1] = false;
			tolerancias[2] = false;
			tolerancias[3] = false;
			tolerancias[4] = false;
			tolerancias[5] = false;			
		}
		
		private void InicializaContadores()
		{
			pesoGlobal = 0;
			pesoUnos = 0;
			pesoVar = 0;
			pesoEquis = 0;
			pesoDoses = 0;		
			
			noFallosActual = 0;
			falloPNGlobalBase = false;
			falloPNVarBase = false;
			falloPNUnosBase = false;
			falloPNEquisBase = false;
			falloPNDosesBase = false;
		}
        private void AnalizaColumna(long columna)
        {

            int tempSuma1 = 0;
            int tempSumaX = 0;
            int tempSuma2 = 0;

            int indicePartido = 0;

            //nota: Si un grupo contiene 7 partidos, los pesos numericos
            //de este grupo se calculan agrupando todos los partidos consecutivamente,
            //por ejemplo, si partidos de un grupo a jugar son los partidos: 1,3,5,7,9,11,13
            //los PN se calcularian como si esos partidos estuviesen el las posiciones: 1,2,3,4,5,6,7

            while (columna !=0)
            {
                switch (columna & 7)
                {
                    case 4: //'1'
                        tempSuma1 += indicesUnos[indicePartido];
                        indicePartido++;
                        break;
                    case 2: //'X'
                        tempSumaX += indicesEquis[indicePartido];
                        indicePartido++;
                        break;
                    case 1: //'2'
                        tempSuma2 += indicesDoses[indicePartido];
                        indicePartido++;
                        break;
                }
                columna >>= 3;
            }

            //PN 1:
            pesoUnos = CalculaPeso(tempSuma1);

            //PN X:
            pesoEquis = CalculaPeso(tempSumaX);

            //PN 2:
            pesoDoses = CalculaPeso(tempSuma2);

            //PN V:
            int tempInt = pesoEquis + pesoDoses;
            pesoVar = CalculaPeso(tempInt);

            //PN G:
            tempInt = pesoVar + pesoUnos;
            pesoGlobal = CalculaPeso(tempInt);
        }
        protected bool AnalizaFiguras()
        {
            if (figuras.Count == 5)
            {
                return true;
            }
            ObtenerFiguraLong(); //Esto inicializa la figura
            return Figuras.Contains(figuraPesos.Figura);
        }

        public void ObtenerFiguraLong()
        {
            figuraPesos = new FiguraCondicion();

            int[] figTemp = new int[10]; // Para 14 el máximo es 9

            figTemp[PesoUnos]++;
            figTemp[PesoEquis]++;
            figTemp[PesoDoses]++;
            figTemp[PesoVariantes]++;
            figTemp[PesoGlobal]++;

            //Recorrer figTemp

            int inicial = 1;
            int numValores = 0;
            for (int i = 0; i < figTemp.Length; i++)
            {
                if (figTemp[i] != 0)
                {
                    numValores++;
                    inicial *= figTemp[i];
                }
            }
            long l = 0;
            switch (inicial)
            {
                case 1:
                    l = 69905;
                    break;
                case 2:
                    l = 8465;
                    break;
                case 3:
                    l = 785;
                    break;
                case 4:
                    l = 545;
                    break;
                case 6:
                    l = 50;
                    break;

            }
            figuraPesos.Figura = l;
        }

		protected int CalculaPeso(int valorSuma)
		{
		    int numeroTemp = valorSuma;

		    while( numeroTemp > 9)
			{
				int num1 = numeroTemp/10;
				int num2 = numeroTemp - num1*10;
				
				numeroTemp = num1 + num2;
			}			
			
			//int peso = numeroTemp;

            return numeroTemp;
		}
		
		private bool esColumnaValida()
		{
			bool esValida = true;
						
			if( pnGlobal[ pesoGlobal ] == false )
			{
				falloPNGlobalBase = true;
				esValida = false;
			}
			
			if( pnVariantes[ pesoVar ] == false )
			{
				falloPNVarBase = true;
				esValida = false;
			}
			
			if( pnUnos[ pesoUnos ] == false )
			{
				falloPNUnosBase = true;
				esValida = false;
			}
			
			if( pnEquis[ pesoEquis ] == false )
			{
				falloPNEquisBase = true;
				esValida = false;
			}
			
			if( pnDoses[ pesoDoses ] == false )
			{
				falloPNDosesBase = true;
				esValida = false;
			}		
		
			return  esValida;
		}
		
		private void esColumnaValida(ref string[] arrayFallos, ref string[] arrayFallosFiguras)
		{
			string texto="";
            string textoFiguras = "";
			if( pnGlobal[ pesoGlobal ] == false )
			{
				falloPNGlobalBase = true;
				texto+="Fallo en peso global  ("+pesoGlobal+")#";
			}
			if( pnVariantes[ pesoVar ] == false )
			{
				falloPNVarBase = true;
				texto+="Fallo en peso de variantes  ("+pesoVar+")#";
			}
			if( pnUnos[ pesoUnos ] == false )
			{
				falloPNUnosBase = true;
				texto+="Fallo en peso de unos  ("+pesoUnos+")#";
			}
			if( pnEquis[ pesoEquis ] == false )
			{
				falloPNEquisBase = true;
				texto+="Fallo en peso de equis  ("+pesoEquis+")#";
			}
			if( pnDoses[ pesoDoses ] == false )
			{
				falloPNDosesBase = true;
				texto+="Fallo en peso de doses  ("+pesoDoses+")#";
			}

            //Figuras
            if (UsaFiguras())
            {
                if (!AnalizaFiguras())
                {
                    textoFiguras += "Fallo en Figuras  (" + Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong( FiguraPesos.Figura) + ")#";
                }
            }

			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
            if (textoFiguras.Length > 0)
            {
                textoFiguras = textoFiguras.Substring(0, textoFiguras.Length - 1);
                arrayFallosFiguras = textoFiguras.Split('#');
            }
		}
		
		private bool SonToleranciasValidas()
		{
		    //calcular noFallosActual total
			if( falloPNGlobalBase )
			{
				if( pnGlobalTol[ pesoGlobal ] )
				{
					noFallosActual++;
				}
				else
				{
					return false;
				}
			}
			
			if( falloPNVarBase )
			{
				if( pnVariantesTol[ pesoVar ] )
				{
					noFallosActual++;
				}
				else
				{
					return false;
				}
			}
			
			if( falloPNUnosBase )
			{
				if( pnUnosTol[ pesoUnos ] )
				{
					noFallosActual++;
				}
				else
				{
					return false;
				}
			}
			
			if( falloPNEquisBase )
			{
				if( pnEquisTol[ pesoEquis ] )
				{
					noFallosActual++;
				}
				else
				{
					return false;
				}
			}
			
			if( falloPNDosesBase )
			{
				if( pnDosesTol[ pesoDoses ] )
				{
					noFallosActual++;
				}
				else
				{
					return false;
				}
			}

            return tolerancias[noFallosActual];
		}
		
		private bool CumpleCondiciones()
		{
			bool esValida = false;
			
			//si acertamos la columna...
			if( esColumnaValida())
			{	
				//si permitimos no fallar
				if( tolerancias[0] )
				{
					 esValida = true;
				}				
			}
			
			//mirar tolerancias si columna no es valida
			if(!esValida)
			{
				esValida = SonToleranciasValidas();
			}
            if (esValida)
            {
                if (UsaFiguras())
                {
                    esValida = AnalizaFiguras();
                }
            }
			
			return esValida;
		}

        private void CumpleCondiciones(ref string[] arrayFallos, ref string[] arrayFallosFiguras)
		{
			esColumnaValida(ref arrayFallos, ref arrayFallosFiguras);
			//mirar tolerancias si columna no es valida
			if(arrayFallos!=null)
			{               
				if(SonToleranciasValidas())
				{
					string[] arrayFallos2=new string[arrayFallos.Length+1];
					for(int i=0;i<arrayFallos.Length;i++)
					{
						// Ponemos un * para indicar que NO es un fallo y sólo observaciones de que no se ha fallado
						arrayFallos2[i]="!"+arrayFallos[i];
					}
					arrayFallos2[arrayFallos.Length]="*"+noFallosActual+" tolerancias aceptadas.";
					arrayFallos=arrayFallos2;
				}
			}
		}
		
		public bool Analizar(long columna)
		{
			InicializaContadores();
			AnalizaColumna(columna);
			return CumpleCondiciones();			
		}
		
		public string[] AnalizarFallos(long columna)
		{
			string[] arrayFallos=null;
            string[] arrayFallosFiguras = null;
			InicializaContadores();
			AnalizaColumna(columna);
			CumpleCondiciones(ref arrayFallos, ref arrayFallosFiguras);
            string[] arrayDefinitiva = null;
            if (arrayFallos == null)
            {
                if (arrayFallosFiguras != null)
                {
                    arrayDefinitiva = new string[arrayFallosFiguras.Length];
                    arrayFallosFiguras.CopyTo(arrayDefinitiva, 0);
                }
            }
            else
            {
                if (arrayFallosFiguras != null)
                {
                    arrayDefinitiva = new string[arrayFallosFiguras.Length + arrayFallos.Length];
                    arrayFallos.CopyTo(arrayDefinitiva, 0);
                    arrayFallosFiguras.CopyTo(arrayDefinitiva, arrayFallos.Length);
                }
                else
                {
                    arrayDefinitiva = arrayFallos;
                }
            }

			return arrayDefinitiva;
		}

		public void ReinicializaValores()
		{
			for( int i = 0; i < pnGlobal.Length; i++ )
			{
				pnGlobal[i] = false;
				pnVariantes[i] = false;
				pnUnos[i] = false;
				pnEquis[i] = false;
				pnDoses[i] = false;
				
				pnGlobalTol[i] = false;
				pnVariantesTol[i] = false;
				pnUnosTol[i] = false;
				pnEquisTol[i] = false;
				pnDosesTol[i] = false;				
			}
			
			tolerancias[0] = false;
			tolerancias[1] = false;
			tolerancias[2] = false;
			tolerancias[3] = false;
			tolerancias[4] = false;
			tolerancias[5] = false;
					
		}		
		
		public void PonerTolerancia(int noTolerancia, bool activa)
		{		
			tolerancias[ noTolerancia ] = activa;		
		}
		
		public void PonerTolerancia(int[] noTolerancia)
		{			
			foreach(int i in noTolerancia)
			{
				PonerTolerancia(i, true);
			}			
		}
		
		public void PonerTolerancia(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				PonerTolerancia(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNGlobal(int iPNGlobal, bool active)
		{
			pnGlobal[iPNGlobal] = active; 
		}
		
		public void SetPNGlobal(int[] iPNGlobal)
		{			
			foreach(int i in iPNGlobal)
			{
				SetPNGlobal(i, true);
			}			
		}
		
		public void SetPNGlobal(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNGlobal(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNGlobalTol(int iPNGlobalTol, bool active)
		{
			pnGlobalTol[iPNGlobalTol] = active; 
		}
		
		public void SetPNGlobalTol(int[] iPNGlobalTol)
		{			
			foreach(int i in iPNGlobalTol)
			{
				SetPNGlobalTol(i, true);
			}			
		}
		
		public void SetPNGlobalTol(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNGlobalTol(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNVar(int iPNVar, bool active)
		{
			pnVariantes[iPNVar] = active; 
		}
		
		public void SetPNVar(int[] iPNVar)
		{			
			foreach(int i in iPNVar)
			{
				SetPNVar(i, true);
			}			
		}
		
		public void SetPNVar(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNVar(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNVarTol(int iPNVarTol, bool active)
		{
			pnVariantesTol[iPNVarTol] = active; 
		}
		
		public void SetPNVarTol(int[] iPNVarTol)
		{			
			foreach(int i in iPNVarTol)
			{
				SetPNVarTol(i, true);
			}			
		}
		
		public void SetPNVarTol(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNVarTol(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNUnos(int iPNUnos, bool active)
		{
			pnUnos[iPNUnos] = active; 
		}
		
		public void SetPNUnos(int[] iPNUnos)
		{			
			foreach(int i in iPNUnos)
			{
				SetPNUnos(i, true);
			}			
		}
		
		public void SetPNUnos(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNUnos(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNUnosTol(int iPNUnosTol, bool active)
		{
			pnUnosTol[iPNUnosTol] = active; 
		}
		
		public void SetPNUnosTol(int[] iPNUnosTol)
		{			
			foreach(int i in iPNUnosTol)
			{
				SetPNUnosTol(i, true);
			}			
		}
		
		public void SetPNUnosTol(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNUnosTol(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNEquis(int iPNEquis, bool active)
		{
			pnEquis[iPNEquis] = active; 
		}
		
		public void SetPNEquis(int[] iPNEquis)
		{			
			foreach(int i in iPNEquis)
			{
				SetPNEquis(i, true);
			}			
		}
		
		public void SetPNEquis(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNEquis(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNEquisTol(int iPNEquisTol, bool active)
		{
			pnEquisTol[iPNEquisTol] = active; 
		}
		
		public void SetPNEquisTol(int[] iPNEquisTol)
		{			
			foreach(int i in iPNEquisTol)
			{
				SetPNEquisTol(i, true);
			}			
		}
		
		public void SetPNEquisTol(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNEquisTol(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNDoses(int iPNDoses, bool active)
		{
			pnDoses[iPNDoses] = active; 
		}
		
		public void SetPNDoses(int[] iPNDoses)
		{			
			foreach(int i in iPNDoses)
			{
				SetPNDoses(i, true);
			}			
		}
		
		public void SetPNDoses(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNDoses(Convert.ToInt32(str), true);
			}		
		}
		
		public void SetPNDosesTol(int iPNDosesTol, bool active)
		{
			pnDosesTol[iPNDosesTol] = active; 
		}
		
		public void SetPNDosesTol(int[] iPNDosesTol)
		{			
			foreach(int i in iPNDosesTol)
			{
				SetPNDosesTol(i, true);
			}			
		}
		
		public void SetPNDosesTol(string valores)
		{
			string[] strValores = valores.Split(',');
			if(valores.Length==0) return;
			foreach(string str in strValores)
			{
				SetPNDosesTol(Convert.ToInt32(str), true);
			}		
		}
		
		public string GetPNGlobal()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnGlobal )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNGlobalTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnGlobalTol )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNVariantes()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnVariantes )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNVariantesTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnVariantesTol )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNUnos()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnUnos )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNUnosTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnUnosTol )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNEquis()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnEquis )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNEquisTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnEquisTol )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNDoses()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnDoses )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetPNDosesTol()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in pnDosesTol )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public string GetTolerancias()
		{
			string valores = "";
			int counter = 0;
			
			foreach( bool isTrue in tolerancias )
			{
				if( isTrue )
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
				
					valores += counter.ToString();
				}
				
				counter++;
			}	
		
			return valores;
		}
		
		public Filtro NombreFiltro
		{
			get{ return Filtro.PesosNumericos; }
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
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			//este filtro no usa tolerancias de grupo.
			return 0;
		}
		
		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.PesosNumericos.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public void LlenarTodosValores()
		{
			// Pone como True todos aquellos valores que no tengan ningún valor asignado
			string todosValores="";
			for(int i=0;i<10;i++)
			{
				todosValores+=","+i;
			}
			todosValores=todosValores.Substring(1);
			if(hayValores(pnGlobal)==false) SetPNGlobal(todosValores);
			if(hayValores(pnVariantes)==false) SetPNVar(todosValores);
			if(hayValores(pnUnos)==false) SetPNUnos(todosValores);
			if(hayValores(pnEquis)==false) SetPNEquis(todosValores);
			if(hayValores(pnDoses)==false) SetPNDoses(todosValores);
			if(hayValores(pnGlobalTol)==false) SetPNGlobalTol(todosValores);
			if(hayValores(pnVariantesTol)==false) SetPNVarTol(todosValores);
			if(hayValores(pnUnosTol)==false) SetPNUnosTol(todosValores);
			if(hayValores(pnEquisTol)==false) SetPNEquisTol(todosValores);
			if(hayValores(pnDosesTol)==false) SetPNDosesTol(todosValores);
		}

        private bool hayValores(bool[] propiedad)
        {
            for (int i = 0; i < propiedad.Length; i++)
            {
                if (propiedad[i]) return true;
            }
            return false;
        }

	    public int PesoGlobal
        {
            get { return pesoGlobal; }
        }
        public int PesoUnos
        {
            get { return pesoUnos; }
        }
        public int PesoEquis
        {
            get { return pesoEquis; }
        }
        public int PesoDoses
        {
            get { return pesoDoses; }
        }
        public int PesoVariantes
        {
            get { return pesoVar; }
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


        #endregion

        #region Miembros de IFiltro


        public bool AnalisisActivo
        {
            get { return VariablesGlobales.AnalizarPesos; }
        }
        public FiguraCondicion FiguraPesos
        {
            get { return figuraPesos; }
        }

        #endregion
    }
}

