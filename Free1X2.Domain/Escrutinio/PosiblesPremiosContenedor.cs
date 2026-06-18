// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;

namespace Free1X2.Escrutinio
{
	/// <summary>
	/// Summary description for PosiblesPremiosContenedor.
	/// </summary>
	public class PosiblesPremiosContenedor
	{
		protected string colGanadora;

        protected List<string> col16 = new List<string>();
        protected List<string> col15 = new List<string>();
        protected List<string> col14 = new List<string>();
        protected List<string> col13 = new List<string>();
        protected List<string> col12 = new List<string>();
        protected List<string> col11 = new List<string>();
        protected List<string> col10 = new List<string>();
		protected double valorTotal;

	    public string ColGanadora
		{
			get {return colGanadora;}
			set {colGanadora = value;}
		}
        public List<string> Col16
        {
            get { return col16; }
            set { col16 = value; }
        }
        public List<string> Col15
        {
            get { return col15; }
            set { col15 = value; }
        }
		public List<string> Col14
		{
			get {return col14;}
			set {col14 = value;}
		}
		public List<string> Col13
		{
			get {return col13;}
			set {col13 = value;}
		}
		public List<string> Col12
		{
			get {return col12;}
			set {col12 = value;}
		}
		public List<string> Col11
		{
			get {return col11;}
			set {col11 = value;}
		}
		public List<string> Col10
		{
			get {return col10;}
			set {col10 = value;}
		}
		public double ValorTotal
		{
			get
			{
				return valorTotal;
			}
		}
		public void CalcularValorTotal()
		{
			double valor = Col10.Count * 0.00001;
			if(Col11.Count > 0)
			{
				valor += Col11.Count * 1;
			}
			if(Col12.Count > 0)
			{
				valor += Col12.Count * 500;
			}
			if(Col13.Count > 0)
			{
				valor += Col13.Count + 200000;
			}
			if(Col14.Count > 0)
			{
				valor += Col14.Count + 6000000;
			}
            if (Col15.Count > 0)
            {
                valor += Col15.Count + 600000000;
            }
            if (Col16.Count > 0)
            {
                valor += Col16.Count + 600000000000;
            }
			valorTotal = valor;
		}

	}
}
