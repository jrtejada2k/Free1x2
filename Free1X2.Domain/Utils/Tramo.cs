// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
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

namespace Free1X2.Utils
{
	/// <summary>
	/// Summary description for Tramo.
	/// </summary>
	public class Tramo
	{
		private int _NumTramo;
		private int _ValorIzquierda;
		private int _ValorDerecha;
		private int _NumColumnasTramo;
		private double _ProbAcumulada;
		private int[] _Aciertos= new int[5];
		private int _ColumnasPremiadas;
		private double _PremiosAcumulados;
		private double _Balance;
		private double[] premios= new double[5];


		public Tramo(double[] ImportePremios)
		{			
			premios=ImportePremios;			
		}
		public void PonerAciertos (int[] Aciertos)
		{
			_Aciertos=Aciertos;
		}
		public int NumeroDeTramo
		{
			get{ return _NumTramo; }
			set{_NumTramo = value;}
		}
		public void AddTramo (Tramo tr)
		{
			_Aciertos[0]+=tr.P14;
			_Aciertos[1]+=tr.P13;
			_Aciertos[2]+=tr.P12;
			_Aciertos[3]+=tr.P11;
			_Aciertos[4]+=tr.P10;
			_ColumnasPremiadas +=tr.ColumnasPremiadas ;
			_PremiosAcumulados +=tr.TotalImportePremios;
			_Balance +=tr.Balance ;
		}
		public int ValorIzquierda
		{
			get{ return _ValorIzquierda; }
			set{_ValorIzquierda = value;}
		}
		public int ValorDerecha
		{
			get{ return _ValorDerecha; }
			set{_ValorDerecha = value;}
		}
		public int NumColumnasTramo
		{
			get{ return _NumColumnasTramo; }
			set{_NumColumnasTramo = value;}
		}
		public double ProbAcumulada
		{
			get{ return _ProbAcumulada; }
			set{_ProbAcumulada = value;}
		}
		public int P14
		{
			get{ return _Aciertos[0]; }
		}
		public int P13
		{
			get{ return _Aciertos[1]; }
		}
		public int P12
		{
			get{ return _Aciertos[2]; }
		}
		public int P11
		{
			get{ return _Aciertos[3]; }
		}
		public int P10
		{
			get{ return _Aciertos[4]; }
		}

		public int ColumnasPremiadas
		{
			get{ return _ColumnasPremiadas; }
			set{_ColumnasPremiadas = value;}
		}
		public double TotalImportePremios
		{
			get	{return _PremiosAcumulados;}
		}
		public void CalculaTotalImportePremios()
		{
			double PrAcum=0;
			for (int i=0;i<5;i++){PrAcum += _Aciertos[i]*premios[i];}
			_PremiosAcumulados=PrAcum;
			_Balance=PrAcum - _NumColumnasTramo * 0.5;
		}
		public double Balance
		{
			get	{return _Balance;}
		}
	}
}