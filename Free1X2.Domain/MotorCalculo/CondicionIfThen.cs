// Free1X2 · WinUI 3 — WIN3
// created on 23/06/2005
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2005 Toni Moreno  toni [at] moreno-csa [dot] com
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

namespace Free1X2.MotorCalculo
{
	public class CondicionIfThen
	{
		private IFiltro filtroIf;
		private IFiltro filtroThen;
		private string condIf;
		private string condThen;
		private bool noIf;
		private bool noThen;

	    public IFiltro FiltroIf
		{
			get{return filtroIf;}
			set{filtroIf=value;}
		}

		public IFiltro FiltroThen
		{
			get{return filtroThen;}
			set{filtroThen=value;}
		}

		public string CondIf
		{
			get{return condIf;}
			set
			{
				condIf=value;
				noIf = (condIf.IndexOf ( "(NO)" ) >= 0);
			}
		}

		public string CondThen
		{
			get{return condThen;}
			set
			{
				condThen=value;
				noThen = (condThen.IndexOf ( "(NO)" ) >= 0);
			}
		}

		public bool NoIf
		{
			get{return noIf;}
			set{noIf=value;}
		}
		
		public bool NoThen
		{
			get{return noThen;}
			set{noThen=value;}
		}

		public IFiltro getFiltro(string texto)
		{
			IFiltro f=null;
			string nombreCond;
			string valor;
			int posicion;
			// Comprueba los tipos de filtro
			if(texto.IndexOf("Cantidad")>=0)
			{
				posicion=texto.LastIndexOf(" ");
			    valor=texto.Substring(posicion).Trim();
				FiltroNoVariantes f2=new FiltroNoVariantes();
				if(texto.IndexOf("Variantes")>=0)
					f2.SetNoVariantes(Convert.ToInt16(valor),true);
				else if(texto.IndexOf("X")>=0)
					f2.SetNoEquis(Convert.ToInt16(valor),true);
				else if(texto.IndexOf("2")>=0)
					f2.SetNoDoses(Convert.ToInt16(valor),true);
				// Llena todos los valores antes de asignar el del filtro.
				f2.LlenarTodosValores();
				f=f2;
			}
			else if(texto.IndexOf("Dibujo")>=0)
			{
				posicion=texto.LastIndexOf(" ");
				valor=texto.Substring(posicion).Trim();
				FiltroDibujos f2=new FiltroDibujos();
				f2.Dibujos.Add(valor);
				f=f2;
			}
			else if(texto.IndexOf("Signos Seguidos")>=0)
			{
				posicion=texto.LastIndexOf(" ");
				nombreCond=texto.Substring(0,posicion-1);
				valor=texto.Substring(posicion).Trim();
				FiltroSignosSeguidos f2=new FiltroSignosSeguidos();
				if(nombreCond.IndexOf("Variantes")>=0)
					f2.SetNoVariantes(valor);
				else if(nombreCond.IndexOf("1")>=0)
					f2.SetNoUnos(valor);
				else if(nombreCond.IndexOf("X")>=0)
					f2.SetNoEquis(valor);
				else if(nombreCond.IndexOf("2")>=0)
						f2.SetNoDoses(valor);
				// Llena todos los valores antes de asignar el del filtro.
				f2.LlenarTodosValores();
				f=f2;
			}
			else if(texto.IndexOf("Interrupciones")>=0)
			{
				posicion=texto.LastIndexOf(" ");
				nombreCond=texto.Substring(0,posicion-1);
				valor=texto.Substring(posicion).Trim();
				FiltroInterrupciones f2=new FiltroInterrupciones();
				if(nombreCond.IndexOf("Seguidas")>=0)
				{
					if(nombreCond.IndexOf("Globales")>=0)
						f2.SetNoIntGlobalSeg(valor);
					else if(nombreCond.IndexOf("Variantes")>=0)
						f2.SetNoIntVarSeg(valor);
					else if(nombreCond.IndexOf("1")>=0)
						f2.SetNoInt1Seg(valor);
					else if(nombreCond.IndexOf("X")>=0)
						f2.SetNoIntXSeg(valor);
					else if(nombreCond.IndexOf("2")>=0)
						f2.SetNoInt2Seg(valor);
				}
				else
				{
					if(nombreCond.IndexOf("Globales")>=0)
						f2.SetNoIntGlobales(valor);
					else if(nombreCond.IndexOf("Variantes")>=0)
						f2.SetNoIntVar(valor);
					else if(nombreCond.IndexOf("1")>=0)
						f2.SetNoInt1(valor);
					else if(nombreCond.IndexOf("X")>=0)
						f2.SetNoIntX(valor);
					else if(nombreCond.IndexOf("2")>=0)
						f2.SetNoInt2(valor);
				}
				// Llena todos los valores antes de asignar el del filtro.
				f2.LlenarTodosValores();
				f=f2;
			}
			else if(texto.IndexOf("Peso numérico")>=0)
			{
				posicion=texto.LastIndexOf(" ");
				nombreCond=texto.Substring(0,posicion-1);
				valor=texto.Substring(posicion).Trim();
				FiltroPesosNumericos f2=new FiltroPesosNumericos();
				if(nombreCond.IndexOf("Global")>=0)
					f2.SetPNGlobal(valor);
				else if(nombreCond.IndexOf("Variantes")>=0)
					f2.SetPNVar(valor);
				else if(nombreCond.IndexOf("1")>=0)
					f2.SetPNUnos(valor);
				else if(nombreCond.IndexOf("X")>=0)
					f2.SetPNEquis(valor);
				else if(nombreCond.IndexOf("2")>=0)
					f2.SetPNDoses(valor);
				// Llena todos los valores antes de asignar el del filtro.
				f2.LlenarTodosValores();
				f=f2;
			}
			else if(texto.IndexOf("Distancia")>=0)
			{
				posicion=texto.LastIndexOf(" ");
				nombreCond=texto.Substring(0,posicion-1);
				valor=texto.Substring(posicion).Trim();
				FiltroDistancias f2=new FiltroDistancias();
				if(nombreCond.IndexOf("Variantes")>=0)
					f2.SetdistVar(valor);
				else if(nombreCond.IndexOf("1")>=0)
					f2.SetdistUnos(valor);
				else if(nombreCond.IndexOf("X")>=0)
					f2.SetdistEquis(valor);
				else if(nombreCond.IndexOf("2")>=0)
					f2.SetdistDoses(valor);
				// Llena todos los valores antes de asignar el del filtro.
				f2.LlenarTodosValores();
				f=f2;
			}
			else if(texto.IndexOf("Contactos")>=0)
			{
				posicion=texto.LastIndexOf(" ");
				nombreCond=texto.Substring(0,posicion-1);
				valor=texto.Substring(posicion).Trim();
				FiltroContactos f2=new FiltroContactos();
				if(nombreCond.IndexOf("1X")>=0)
					f2.SetNum1X(valor);
				else if(nombreCond.IndexOf("12")>=0)
					f2.SetNum12(valor);
				else if(nombreCond.IndexOf("X2")>=0)
					f2.SetNumX2(valor);
				else if(nombreCond.IndexOf("11")>=0)
					f2.SetNum11(valor);
				else if(nombreCond.IndexOf("XX")>=0)
					f2.SetNumXX(valor);
				else if(nombreCond.IndexOf("22")>=0)
					f2.SetNum22(valor);
				else if(nombreCond.IndexOf("1V")>=0)
					f2.SetNum1V(valor);
				else if(nombreCond.IndexOf("XV")>=0)
					f2.SetNumXV(valor);
				else if(nombreCond.IndexOf("2V")>=0)
					f2.SetNum2V(valor);
				else if(nombreCond.IndexOf("VV")>=0)
					f2.SetNumVV(valor);
				// Llena todos los valores antes de asignar el del filtro.
				f2.LlenarTodosValores();
				f=f2;
			}
			f.IsActive=true;
			f.ContieneDatos=true;
			return f;
		}

		public bool CompruebaPronostico(long columna)
		{
		    IFiltro filtro = getFiltro(CondIf);
			bool esValido = filtro.Analizar(columna);
			if(NoIf) esValido=!esValido;
			// Si la condición If se falla, la condición no se evalúa y la
			// columna es correcta.
			if(esValido==false) return true;

			filtro=getFiltro(CondThen);
			esValido=filtro.Analizar(columna);
			if(NoThen) esValido=!esValido;
			if(esValido==false) return false;
		    return true;
		}

	}
}
