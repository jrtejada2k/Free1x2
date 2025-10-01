// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
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

using System;
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for Simetria.
	/// </summary>
	public class Simetria
	{
        protected List<int> partidosSimetricos = new List<int>();
		protected string partidos;
		public Simetria(string partidosEntrada)
		{
			ObtenerPartidosSimetricos(partidosEntrada);
			Partidos = partidosEntrada;
		}
		public List<int> PartidosSimetricos
		{
			get {return partidosSimetricos;}
			set {partidosSimetricos = value;}
		}
		public string Partidos
		{
			get {return partidos;}
			set {partidos = value;}
		}
		protected void ObtenerPartidosSimetricos(string parts)
		{
			string[] partes = parts.Split(',');
			for(int i = 0; i < partes.Length; i++)
			{
				int partidoSimetrico = Convert.ToInt32(partes[i]);
				PartidosSimetricos.Add(partidoSimetrico);
			}
		}
	}
}
