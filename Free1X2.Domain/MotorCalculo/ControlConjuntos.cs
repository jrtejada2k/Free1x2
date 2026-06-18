// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
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

using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for ControlConjuntos.
	/// </summary>
	public class ControlConjuntos
	{
		private bool[] fallosPermitidos;
		private int[] ctrlGruposControlados;

		private string strFallosPermitidos = "";
		private string strCtrlGruposControlados = "";

	    public void PonerFallosPermitidos(string fallosgrupos)
		{
			strFallosPermitidos = fallosgrupos;
            
            fallosPermitidos = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(fallosgrupos,ctrlGruposControlados.Length);
		}

		public void PonerCtrlGruposControlados(string controlesGrupos)
		{
			strCtrlGruposControlados = controlesGrupos;

			RangosHelper rangos = new RangosHelper();

			ctrlGruposControlados = rangos.ObtenIntArray(controlesGrupos);		
		}

		public string ObtenCtrlGruposControladosStr()
		{
			return strCtrlGruposControlados;
		}

		public string ObtenFallosPermitidosStr()
		{
			return strFallosPermitidos;
		}

		public int[] ObtenCtrolGruposConjunto()
		{
			return ctrlGruposControlados;
		}

		public bool[] ObtenFallosPermitidos()
		{
			return fallosPermitidos;
		}

		public bool ContieneConjunto(int noConj)
		{
			bool contieneConj = false;					
			
			for(int i = 0; i < ctrlGruposControlados.Length; i++)
			{
				if( ctrlGruposControlados[i] == noConj)
				{
					contieneConj = true;
					break;
				}			
			}

			return contieneConj;
		}
	}
}
