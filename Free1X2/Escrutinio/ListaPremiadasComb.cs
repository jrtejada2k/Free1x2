using Free1X2.Utils;

namespace Free1X2.Escrutinio
{
	/// <summary>
	/// Descripción breve de ColumnasPremiadas.
	/// </summary>
	public class ListaPremiadasComb
	{
		private string columnaTexto;
		private long columna;
		private int premio;
		private ConvertidorDeBases conv = new ConvertidorDeBases ();

		public ListaPremiadasComb(string _columna, int _premio)
		{
			columnaTexto = _columna;
			columna=conv.ConvColumnaANumero(_columna);
			premio=_premio;
		}

		public ListaPremiadasComb ( long _columna, int _premio )
		{
			columnaTexto = conv.ConvNumAColumna(_columna);
			columna = _columna;
			premio = _premio;
		}

		public ListaPremiadasComb ()
		{
		}
		
		public string ColumnaTexto
		{
			get{return columnaTexto;}
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

		public int Premio
		{
			get{return premio;}
			set{premio=value;}
		}

	}
}
