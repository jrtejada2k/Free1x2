// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
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
using System;
using System.Collections.Generic;
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for ControladorCPControlFallos.
	/// </summary>
	public class ControladorCPControlFallos
	{
		protected List<CPControlFallos> controlesFallos;
		protected List<ColumnaProbable> columnasProbables;

		protected string strFallosPermitidos = "";
		protected bool[] fallosPermitidos;

		public ControladorCPControlFallos()
		{
			controlesFallos = new List<CPControlFallos>();
		}

		public bool Analiza()
		{
			bool columnaValida = true;
			CPControlFallos ctrlFallos;
			if(strFallosPermitidos == "")
			{
				for(int i = 0; i < controlesFallos.Count; i++)
				{
					ctrlFallos = controlesFallos[i];
					columnaValida = ctrlFallos.Analiza();
					if (columnaValida == false)
						break;
				}		
			}
			else
			{
				int noFallos = 0;
				for(int i = 0; i < controlesFallos.Count; i++)
				{
					ctrlFallos = controlesFallos[i];					
					if (ctrlFallos.Analiza() == false)
					{
						noFallos++;
					}
				}	
				if(fallosPermitidos.Length < noFallos + 1 || fallosPermitidos[noFallos] == false)
				{
					columnaValida = false;
				}			
			}
			return columnaValida;
		}

		public void Analiza(ref string txt, bool tieneFallos)
		{
			// Si no hay fallos anteriores no hace falta comprobar el control de fallos
			//if(tieneFallos==false) return;
			// Convertimos el argumento en cadena vacía para evitar errores
			txt="";
		    CPControlFallos ctrlFallos;
			if(strFallosPermitidos == "")
			{
				for(int i = 0; i < controlesFallos.Count; i++)
				{
					ctrlFallos = controlesFallos[i];
					bool columnaValida = ctrlFallos.Analiza();
					if (columnaValida == false)
						txt+="Fallo en " + Convert.ToString(i+1)+"ª linea de Tolerancias Locales  ("+ctrlFallos.ToleranciasAcumuladas+")#";
					else
					{
							txt+="*"+Convert.ToString(i+1)+"ª linea de Tolerancias Locales aceptada.#";
					}
				}
			}
			else
			{
				int noFallos = 0;
				for(int i = 0; i < controlesFallos.Count; i++)
				{
					ctrlFallos = controlesFallos[i];					
					if (ctrlFallos.Analiza() == false)
					{
						noFallos++;
					}
				}
				if(fallosPermitidos.Length < noFallos + 1 || fallosPermitidos[noFallos] == false)
					txt+="Fallo en Fallos de Controles permitidos  ("+noFallos+")#";			}
		}

		protected void InicializarFallos()
		{
			RangosHelper rangosHelper = new RangosHelper();
			fallosPermitidos = rangosHelper.ObtenBoolArray(strFallosPermitidos);
		}

		public List<CPControlFallos> ControlesFallos
		{
			get { return controlesFallos; }
			set 
			{ 
				controlesFallos = value; 
				
				//añadir referencia a este objeto "padre"
				foreach(CPControlFallos ctrlFallos in controlesFallos)
				{
					ctrlFallos.Controlador = this;				
				}			
			}
		}

		public List<ColumnaProbable> ColumnasProbables
		{
			get{ return columnasProbables; }
			set{ columnasProbables = value; }
		}

		public string FallosPermitidos
		{
			get{ return strFallosPermitidos; }
			set
			{ 
				strFallosPermitidos = value; 

				if(strFallosPermitidos != "")
				{
					InicializarFallos();
				}
				else
				{
					fallosPermitidos = null;
				}
			}		
		}
	}
}
