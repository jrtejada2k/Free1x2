using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{

	public class GeneradorColumnas
	{
        private long columnaBase;
        private long columnaInicial;
		private string archColsBase = "";
				
		private int noPartidos;

		private bool generacionEnMarcha;
		
		private Analizador analizador;
								
		public GeneradorColumnas(string[] Pronosticos)
		{				
			noPartidos = Pronosticos.Length;
            columnaBase = UtilColumnas.ConvStrToLong(Pronosticos);
            InicializarColumnaInicial();
		}
		
		public GeneradorColumnas(string archivoColsBase)
		{
            archColsBase = archivoColsBase;
            columnaBase = 281474976710655;
            IArchivoColumnas aCol = new ArchivoColumnasTexto(archivoColsBase);
            
            InicializarColumnaInicial(aCol.ObtenNumSignos());
            aCol.Cerrar();
		}
        protected void InicializarColumnaInicial()
        {
            switch (VariablesGlobales.NumeroPartidos)
            {
                case 14:
                    columnaInicial = 2513169434916;
                    break;
                case 15:
                    columnaInicial = 20105355479332;
                    break;
                case 16:
                    columnaInicial = 160842843834660;
                    break;
                case 13:
                    columnaInicial = 314146179364;
                    break;
                case 12:
                    columnaInicial = 39268272420;
                    break;
                case 11:
                    columnaInicial = 4908534052;
                    break;
                case 10:
                    columnaInicial = 613566756;
                    break;
                case 9:
                    columnaInicial = 76695844;
                    break;
                case 8:
                    columnaInicial = 9586980;
                    break;
                case 7:
                    columnaInicial = 1198372;
                    break;
                case 6:
                    columnaInicial = 149796;
                    break;
                case 5:
                    columnaInicial = 18724;
                    break;
                case 4:
                    columnaInicial = 2340;
                    break;
                case 3:
                    columnaInicial = 292;
                    break;
                case 2:
                    columnaInicial = 36;
                    break;
                case 1:
                    columnaInicial = 4;
                    break;


            }
        }
        protected void InicializarColumnaInicial(int numPartidos)
        {
            switch (numPartidos)
            {
                case 14:
                    columnaInicial = 2513169434916;
                    break;
                case 15:
                    columnaInicial = 20105355479332;
                    break;
                case 16:
                    columnaInicial = 160842843834660;
                    break;
                case 13:
                    columnaInicial = 314146179364;
                    break;
                case 12:
                    columnaInicial = 39268272420;
                    break;
                case 11:
                    columnaInicial = 4908534052;
                    break;
                case 10:
                    columnaInicial = 613566756;
                    break;
                case 9:
                    columnaInicial = 76695844;
                    break;
                case 8:
                    columnaInicial = 9586980;
                    break;
                case 7:
                    columnaInicial = 1198372;
                    break;
                case 6:
                    columnaInicial = 149796;
                    break;
                case 5:
                    columnaInicial = 18724;
                    break;
                case 4:
                    columnaInicial = 2340;
                    break;
                case 3:
                    columnaInicial = 292;
                    break;
                case 2:
                    columnaInicial = 36;
                    break;
                case 1:
                    columnaInicial = 4;
                    break;


            }
        }

		public void GenerarColumnas()
		{
			generacionEnMarcha = true;	
			
			if( !archColsBase.Equals("") )
			{
				//usa archivo columnas
				UsaColumnasArchivo();
			}
			else
			{
				//comenzamos pasando una columna de NPartidos 1.
                AnalizaNuevaColumna(columnaInicial);
                
                //genera todas las columnas que se diferencian en un signo de la columna
                //de tod NPartidos 1
                GeneraColumnas(columnaInicial, 0, 1);
			}

			generacionEnMarcha = false;	
		}
        public void GenerarColumnas(bool generandoAnalisis, bool esAnalisisExterno)
        {
            generacionEnMarcha = true;

            if (!archColsBase.Equals(""))
            {
                if (esAnalisisExterno)
                {
                    UsaColumnasArchivoExterno(generandoAnalisis);
                }
                else
                {
                    //usa archivo columnas
                    UsaColumnasArchivo(generandoAnalisis);
                }
            }
            else
            {
                //comenzamos pasando una columna de 14 1.
                AnalizaNuevaColumna(columnaInicial,generandoAnalisis);

                //genera todas las columnas que se diferencian en un signo de la columna
                //de tod 14 1
                GeneraColumnas(columnaInicial, 0, 1,generandoAnalisis);
            }

            generacionEnMarcha = false;
        }

        protected void GeneraColumnas(long valorInicial, byte posicionInicial, byte profundidad)
        {
			if(!generacionEnMarcha) return;

            long mascara = (long)7 << 3 * (posicionInicial);

            for (byte partido = posicionInicial; partido < noPartidos; partido++)
            {
                long nuevoLongColumna = valorInicial & ~mascara ^ (long)1 << ((partido) * 3 + 1);
                
                AnalizaNuevaColumna(nuevoLongColumna);

                if (profundidad < noPartidos)
                {
                    GeneraColumnas(nuevoLongColumna, (byte)(partido + 1), (byte)(profundidad + 1));
                }

                nuevoLongColumna = valorInicial & ~mascara ^ (long)1 << ((partido) * 3);

                AnalizaNuevaColumna(nuevoLongColumna);

                if (profundidad < noPartidos)
                {
                    GeneraColumnas(nuevoLongColumna, (byte)(partido + 1), (byte)(profundidad + 1));
                }

                mascara <<= 3;
            } 

        }
        protected void GeneraColumnas(long valorInicial, byte posicionInicial, byte profundidad,bool generandoAnalisis)
        {
            if (generacionEnMarcha == false) return;
            byte partido;
            long mascara = (long)7 << 3 * (posicionInicial);

            for (partido = posicionInicial; partido < noPartidos; partido++)
            {
                long nuevoLongColumna = valorInicial & ~mascara ^ (long)1 << ((partido) * 3 + 1);

                AnalizaNuevaColumna(nuevoLongColumna,generandoAnalisis);

                if (profundidad < noPartidos)
                {
                    GeneraColumnas(nuevoLongColumna, (byte)(partido + 1), (byte)(profundidad + 1),generandoAnalisis);
                }

                nuevoLongColumna = valorInicial & ~mascara ^ (long)1 << ((partido) * 3);

                AnalizaNuevaColumna(nuevoLongColumna,generandoAnalisis);

                if (profundidad < noPartidos)
                {
                    GeneraColumnas(nuevoLongColumna, (byte)(partido + 1), (byte)(profundidad + 1),generandoAnalisis);
                }

                mascara <<= 3;
            }

        }
	
		protected void UsaColumnasArchivoB()
		{
		    IArchivoColumnas archivo = new ArchivoColumnasTexto(archColsBase);
			
			while(generacionEnMarcha && archivo.SiguienteColumna() )
			{
			    //mandar la columna al analizador para realizar calculos
                AnalizaNuevaColumna(archivo.LeeColumnaSinComas().ToUpper());
			}

		    archivo.Cerrar();		
		}
        protected void UsaColumnasArchivo()
        {
            long columna;

            IArchivoColumnas archivo = new ArchivoColumnasTexto(archColsBase);

            while (generacionEnMarcha && archivo.SiguienteColumna())
            {
                string columnaFiltro = archivo.LeeColumnaSinComas();
                if (columnaFiltro.Length > VariablesGlobales.NumeroPartidos)
                {
                    //El filtro tiene más de los partidos admitidos
                    columna = UtilColumnas.ConvStrToLong(columnaFiltro.Substring(0, VariablesGlobales.NumeroPartidos));

                }
                else if (columnaFiltro.Length < VariablesGlobales.NumeroPartidos)
                {
                    columna = UtilColumnas.ConvStrToLong(columnaFiltro, analizador.CompletarCon);
                }
                else
                {
                    columna = UtilColumnas.ConvStrToLong(columnaFiltro);
                }
                //mandar la columna al analizador para realizar calculos
                AnalizaNuevaColumna(columna);
            }
            archivo.Cerrar();
        }
        protected void UsaColumnasArchivo(bool generandoAnalisis)
        {
            long columna;

            IArchivoColumnas archivo = new ArchivoColumnasTexto(archColsBase);

            while (generacionEnMarcha && archivo.SiguienteColumna())
            {
                string columnaFiltro = archivo.LeeColumnaSinComas();
                if (columnaFiltro.Length > VariablesGlobales.NumeroPartidos)
                {
                    //El filtro tiene más de los partidos admitidos
                    columna = UtilColumnas.ConvStrToLong(columnaFiltro.Substring(0, VariablesGlobales.NumeroPartidos));

                }
                else
                {
                    columna = UtilColumnas.ConvStrToLong(columnaFiltro);
                }

                //mandar la columna al analizador para realizar calculos
                AnalizaNuevaColumna(columna, true);
            }
            archivo.Cerrar();
        }
        protected void UsaColumnasArchivoExterno(bool generandoAnalisis)
        {
            //Análisis externo es cuando se usa alguna de las herramientas para analizar archivo, ignora VariablesGolbales.NumPartidos

            IArchivoColumnas archivo = new ArchivoColumnasTexto(archColsBase);

            while (generacionEnMarcha && archivo.SiguienteColumna())
            {
                AnalizaNuevaColumna(UtilColumnas.ConvStrToLong(archivo.LeeColumnaSinComas()), true);
            }
            archivo.Cerrar();
        }

        public void PararGeneracionCols()
		{
			generacionEnMarcha = false;		
		}

        protected void AnalizaNuevaColumna(string col)
        {
            //Borrar
        }
		
		protected void AnalizaNuevaColumna( long columna, bool analizar )
		{
            //comprobamos si la columna esta contenida en el pronostico base.
            if ((columnaBase & columna) == columna)
            {
                analizador.AnalizaColumna(columna,analizar);
            }
		}

        protected void AnalizaNuevaColumna(long columna)
        {
            //comprobamos si la columna esta contenida en el pronostico base.
            if ((columnaBase & columna) == columna)
            {
                analizador.AnalizaColumna(columna);
            }
        }
		
		public Analizador AnalizadorColumnas
		{
			set { analizador = value; }
		}
				
	}
}
