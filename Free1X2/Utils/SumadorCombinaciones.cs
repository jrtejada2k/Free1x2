// created on 07/12/2003 at 21:14
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2003 Joan Duatis - duatis@coac.net
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

using System.Collections;
using Free1X2.EntradaSalida;

namespace Free1X2.Utils
{
	
	public class SumadorCombinaciones
	{
		private string archivoCols1 = "";
		private string archivoCols2 = "";
		private string archivoColsFinal = "";

	    private string mensajeFinCalculo = "";

        private IArchivoColumnas archivoBaseCols;
        private IArchivoColumnas archivoFinalCols;
		
		private int noColumnasFinal;
		private int noColsRepetidas;

        private readonly BitArray Bits = new BitArray(14348907, false);

        private readonly int noPartidos;

        public SumadorCombinaciones(int noSignos)
        {
            noPartidos = noSignos;
        }
		
		public void Calcula( AlgebraCombTipo tipoCalculo )
		{
			
			switch( tipoCalculo )
			{
				case AlgebraCombTipo.EliminaRepetidas:
					EliminaColumnasRepetidas();
					break;
				case AlgebraCombTipo.SumaEliminaRepetidas:
					SumaCombinacionesSinRepetir();
					break;
				case AlgebraCombTipo.SumaSoloComunes:
					SumaCombinacionesSoloRepetidas();
					break;
				case AlgebraCombTipo.RestaSegunda:
					Resta2Combinaciones();
					break;				
			}
			
			CrearMensajeFinCalculo( tipoCalculo );
		
		}

		protected void Resta2Combinaciones()
		{
			int Num;
			noColumnasFinal = 0;
			noColsRepetidas = 0;
			Bits.SetAll (false);

            ConvertidorDeBases conv = new ConvertidorDeBases((byte)noPartidos);
			
			archivoBaseCols = InicializaArchivoCols( archivoCols1 ); // lee el primer archivo
			while( archivoBaseCols.SiguienteColumna() )	{ 
				Num = conv.ConvColumnaANumero  (archivoBaseCols.LeeColumnaSinComas());
				if (Bits[Num]==false)
				{
					Bits[Num]=true;
					noColumnasFinal++;
				}
				else
				{
					noColsRepetidas++;
				}

		
			}
			archivoBaseCols.Cerrar();	//cerrar primer archivo y abrir segundo
			
			archivoBaseCols = InicializaArchivoCols( archivoCols2 );
			while( archivoBaseCols.SiguienteColumna() ) {
				Num = conv.ConvColumnaANumero  (archivoBaseCols.LeeColumnaSinComas());
				if (Bits[Num])
				{
					Bits[Num]=false;
					noColumnasFinal--;
					noColsRepetidas++;
				}
			}
			archivoBaseCols.Cerrar();

            IArchivoColumnas Cols = new ArchivoColumnasTexto(archivoColsFinal);

			for (int nr = 0; nr < Bits.Count; nr++) 
			{
                if (Bits[nr])
                {
                    Cols.GuardarCols(conv.ConvNumAColumna(nr));
                }
			}		
			Cols.Cerrar();	
		}

		protected void SumaCombinacionesSinRepetir()
		{
			Bits.SetAll (false);

            ConvertidorDeBases conv = new ConvertidorDeBases((byte)noPartidos);
			
			archivoBaseCols = InicializaArchivoCols( archivoCols1 );
			archivoFinalCols = InicializaArchivoCols( archivoColsFinal );
			
			int Num;
			noColumnasFinal = 0;
			noColsRepetidas = 0;
			string columna;
			
			//lee todas las columnas del primer archivo 
			//y activa su bit
			while( archivoBaseCols.SiguienteColumna() )
			{
				columna=archivoBaseCols.LeeColumnaSinComas();
				Num = conv.ConvColumnaANumero  (columna);
				//No poner posibles columnas repetidas!
				if (Bits[Num]==false)
				{
					Bits[Num]=true;
					archivoFinalCols.GuardarCols( columna);

					noColumnasFinal++;
				}
				else
				{
					//columna repetida
					noColsRepetidas++;
				}
			}
			
			//cerrar primer archivo y abrir segundo
			archivoBaseCols.Cerrar();
			
			archivoBaseCols = InicializaArchivoCols( archivoCols2 );
			
			while( archivoBaseCols.SiguienteColumna() )
			{
				columna=archivoBaseCols.LeeColumnaSinComas();
				Num = conv.ConvColumnaANumero  (columna);
								
				//si columna no está marcada, columna no repetida
				if (Bits[Num]==false)
				{
					Bits[Num]=true;
					
					//grabar columna a archivo
					archivoFinalCols.GuardarCols( columna);				
					
					noColumnasFinal++;

				}
				else
				{
					//columna repetida
					noColsRepetidas++;
				}
			}			
			
			//limpieza general de objetos para liberar memoria
			archivoBaseCols.Cerrar();
			archivoFinalCols.Cerrar();
		}

		protected void SumaCombinacionesSoloRepetidas()
		{
			Bits.SetAll (false);

            ConvertidorDeBases conv = new ConvertidorDeBases((byte)noPartidos);
						
			archivoBaseCols = InicializaArchivoCols( archivoCols1 );		
			archivoFinalCols = InicializaArchivoCols( archivoColsFinal );
			int Num;
			noColumnasFinal = 0;
			noColsRepetidas = 0;

		    //lee todas las columnas del primer archivo 
			//y activa su bit
			while( archivoBaseCols.SiguienteColumna() )
			{
				Num = conv.ConvColumnaANumero  (archivoBaseCols.LeeColumnaSinComas());
				
				//activar el bit para futura referencia
				//No poner posibles columnas repetidas!
				if(Bits[Num]==false)
				{
					Bits[Num]=true;
				}
			}
			
			//cerrar primer archivo y abrir segundo
			archivoBaseCols.Cerrar();
			
			archivoBaseCols = InicializaArchivoCols( archivoCols2 );
			
			while( archivoBaseCols.SiguienteColumna() )
			{
				string columna = archivoBaseCols.LeeColumnaSinComas();
				Num = conv.ConvColumnaANumero  (columna);

								
				//si columna está marcada,  guardar columna repetida
				if( Bits[Num])
				{									
					//grabar columna a archivo
					archivoFinalCols.GuardarCols(columna);				
					
					noColumnasFinal++;
					noColsRepetidas++;
				}		
			}	
			
			//limpieza general de objetos para liberar memoria
			archivoBaseCols.Cerrar();
			archivoFinalCols.Cerrar();
		}
		
		protected void EliminaColumnasRepetidas()
		{		
			Bits.SetAll (false);
            ConvertidorDeBases conv = new ConvertidorDeBases((byte)noPartidos);

			archivoBaseCols = InicializaArchivoCols( archivoCols1 );		
			archivoFinalCols = InicializaArchivoCols( archivoColsFinal );

		    noColumnasFinal = 0;
			noColsRepetidas = 0;

		    while( archivoBaseCols.SiguienteColumna() )
			{
				string columna = archivoBaseCols.LeeColumnaSinComas();
				int Num = conv.ConvColumnaANumero  (columna);
								
				//si columna no está marcada, columna no repetida
				if( !Bits[Num] )
				{
					Bits[Num]=true;
					
					//grabar columna a archivo
					archivoFinalCols.GuardarCols( columna );				
					
					noColumnasFinal++;
				}
				else
				{
					//columna repetida
					noColsRepetidas++;
				}

			}		
		
			//limpieza general de objetos para liberar memoria
			archivoBaseCols.Cerrar();
			archivoFinalCols.Cerrar();
		}
		
		protected void CrearMensajeFinCalculo( AlgebraCombTipo tipoCalculo )
		{
			switch( tipoCalculo )
			{
				case AlgebraCombTipo.EliminaRepetidas:
					mensajeFinCalculo = "Nº columnas final: " + noColumnasFinal + "\n";
					mensajeFinCalculo += "Nº col. repetidas: " + noColsRepetidas;
					break;
				case AlgebraCombTipo.SumaEliminaRepetidas:
					mensajeFinCalculo = "Nº columnas final: " + noColumnasFinal + "\n";
					mensajeFinCalculo += "Nº col. repetidas: " + noColsRepetidas;
					break;
				case AlgebraCombTipo.SumaSoloComunes:
					mensajeFinCalculo = "Nº columnas final: " + noColumnasFinal;
					break;	
				case AlgebraCombTipo.RestaSegunda:
					mensajeFinCalculo = "Nº columnas final: " + noColumnasFinal;
					break;			
			}		
		}

        protected ArchivoColumnasTexto InicializaArchivoCols(string combFile)
		{
            return new ArchivoColumnasTexto(combFile);			
		}
		
		public string ArchivoCols1
		{
			get{ return archivoCols1; }
			set{ archivoCols1 = value; }
		}
		
		public string ArchivoCols2
		{
			get{ return archivoCols2; }
			set{ archivoCols2 = value; }
		}
		
		public string ArchivoColsFinal
		{
			get{ return archivoColsFinal; }
			set{ archivoColsFinal = value; }
		}
		
		public string MensajeFinCalculo
		{
			get { return mensajeFinCalculo;}		
		}

	}	
}
