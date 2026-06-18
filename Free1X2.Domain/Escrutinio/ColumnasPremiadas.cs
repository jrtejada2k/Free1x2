namespace Free1X2.Escrutinio
{
	/// <summary>
	/// Descripción breve de ColumnasPremiadas.
	/// </summary>
	public class ColumnasPremiadas
	{
	    private string columna;
		private string fichero;
		private int jornada;
		private int premio;
		private int noColumna;
		private int noBoleto;

		public string Columna
		{
			get{return columna;}
			set{columna=value;}
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

		public int NoColumna
		{
			get{return noColumna;}
			set{noColumna=value;}
		}

		public int NoBoleto
		{
			get{return noBoleto;}
			set{noBoleto=value;}
		}

	}
}
