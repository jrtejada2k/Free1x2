// Free1X2 · WinUI 3 — WIN3
using Free1X2.Utils;

namespace Free1X2.Escrutinio
{
	/// <summary>
	/// Descripción breve de ColumnasPremiadas.
	/// </summary>
	public class ColumnasPremiadasComb
	{
	    private string columnaTexto;
		private long columna;
		private string fichero;
		private int jornada;
		private int premio;
		private ConvertidorDeBases conv = new ConvertidorDeBases ();

		public string ColumnaTexto
		{
			get { return columnaTexto; }
			set
			{
				columnaTexto = value;
				columna = conv.ConvColumnaANumero ( columnaTexto );
			}
		}

		public long Columna
		{
			get { return columna; }
			set
			{
				columna = value;
				columnaTexto = conv.ConvNumAColumna ( columna );
			}
		}

		public string Fichero
		{
			get{return fichero;}
			set{fichero=value;}
		}

		public int Jornada
		{
			get{return jornada;}
			set{jornada=value;}
		}

		public int Premio
		{
			get{return premio;}
			set{premio=value;}
		}

	}
}
