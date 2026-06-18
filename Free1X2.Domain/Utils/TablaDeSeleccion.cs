// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2008 EquipoFree
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
	/// Summary description for TablaDeSeleccion.
	/// </summary>
	public class TablaDeSeleccion
	{		
		private string _Concepto="";
		private double _Min;
		private double _Max;
		private bool _chec;
	
		public TablaDeSeleccion(string pConcepto, double pMin, double pMax)
		{
			_Concepto = pConcepto;
			_Min=pMin;
			_Max=pMax;
		}
		public string Concepto
		{
			get{ return _Concepto;}
		}
		public bool Checked
		{
			get{ return _chec; }
			set{_chec = value;}
		}
		public double Minimo
		{
			get{ return _Min; }
			set{Checked=true;_Min = value;}
		}

		public double Maximo
		{
			get{ return _Max; }
			set{Checked=true;_Max = value;}
		}
	}
}
