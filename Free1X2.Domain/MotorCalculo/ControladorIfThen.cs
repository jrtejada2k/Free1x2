// created on 23/06/2005
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2005 Toni Moreno  toni@moreno-csa.com
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
using System.Collections;
using System.Collections.Generic;
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	public class ControladorIfThen
	{
		private ArrayList controlesCondiciones;
		private ArrayList controlesGrupos;
		//private CondicionIfThen condicion;
		//private GrupoIfThen grupo;
		private string rangoAciertoCondiciones;
		private string rangoAciertoGrupos;
		private bool[] aciertoCondiciones;
		private bool[] aciertoGrupos;
		private bool activo=true;
	
		public ControladorIfThen()
		{
			controlesGrupos = new ArrayList();	
			controlesCondiciones = new ArrayList();	
		}
		
		public ArrayList ControlesGrupos
		{
			get { return controlesGrupos; }
			set { controlesGrupos = value; }
		}

		public ArrayList ControlesCondiciones
		{
			get { return controlesCondiciones; }
			set { controlesCondiciones = value; }
		}

		public string RangoAciertoCondiciones
		{
			get { return rangoAciertoCondiciones; }
			set 
			{
				int[] aciertosTmp;
				rangoAciertoCondiciones = value;
				RangosHelper rangos=new RangosHelper();
				aciertosTmp=rangos.ObtenIntArray(rangoAciertoCondiciones );
				aciertoCondiciones=new bool[ControlesCondiciones.Count+1];
				for(int i=0;i<aciertosTmp.Length;i++)
				{
					aciertoCondiciones[aciertosTmp[i]]=true;
				}
			}
		}

		public string RangoAciertoGrupos
		{
			get { return rangoAciertoGrupos; }
			set 
			{
				int[] aciertosTmp;
				rangoAciertoGrupos = value;
				RangosHelper rangos=new RangosHelper();
				aciertosTmp=rangos.ObtenIntArray(rangoAciertoGrupos );
				aciertoGrupos=new bool[ControlesGrupos.Count+1];
				for(int i=0;i<aciertosTmp.Length;i++)
				{
					aciertoGrupos[aciertosTmp[i]]=true;
				}
			}
		}

		public bool[] AciertoCondiciones
		{
			get { return aciertoCondiciones; }
		}

		public bool[] AciertoGrupos
		{
			get{ return aciertoGrupos; }
		}

		public bool EsActivo
		{
			get { return activo; }
			set { activo=value;}
		}

		public bool EsVacio
		{
			get 
			{
				bool vacio=true;
				if(ControlesCondiciones.Count>0 || ControlesGrupos.Count>0) vacio=false;
				return vacio;
			}
		}

		public bool AnalizaColumna( string columna )
		{
		    return true;
		}
		
		public void AddGrupos( GrupoIfThen grupos)
		{
			controlesGrupos.Add( grupos);	
		}
		
		public void AddCondiciones( CondicionIfThen condicion)
		{
			controlesCondiciones.Add( condicion);	
		}
		
		public void QuitaGrupos(int indice)
		{
			controlesGrupos.RemoveAt(indice);
		}

		public void QuitaCondiciones(int indice)
		{
			controlesCondiciones.RemoveAt(indice);
		}

		public void LimpiaGrupos()
		{
			controlesGrupos.Clear();
		}

		public void LimpiaCondiciones()
		{
			controlesCondiciones.Clear();
		}

		public void LimpiaTodo()
		{
			LimpiaGrupos();
			LimpiaCondiciones();
		}

		public string getLineaTexto(IFiltro filtro)
		{
			string txt="";
			if(filtro.NombreFiltro==Filtro.NoVariantes)
			{
				FiltroNoVariantes f=(FiltroNoVariantes)filtro;
				if(f.GetVariantes().Length>0)
					txt="Cantidad de Variantes: "+f.GetVariantes();
				else if(f.GetEquis().Length>0)
					txt="Cantidad de X: "+f.GetEquis();
				else if(f.GetDoses().Length>0)
					txt="Cantidad de 2: "+f.GetDoses();
			}
			else if(filtro.NombreFiltro==Filtro.Dibujos)
			{
				FiltroDibujos f=(FiltroDibujos)filtro;
				txt="Dibujo: "+f.GetDibujos();
			}
			else if(filtro.NombreFiltro==Filtro.SignosSeguidos)
			{
				FiltroSignosSeguidos f=(FiltroSignosSeguidos)filtro;
				if(f.GetVariantes().Length>0)
					txt="Signos Seguidos de Variantes: "+f.GetVariantes();
				else if(f.GetUnos().Length>0)
					txt="Signos Seguidos de 1: "+f.GetUnos();
				else if(f.GetEquis().Length>0)
					txt="Signos Seguidos de X: "+f.GetEquis();
				else if(f.GetDoses().Length>0)
					txt="Signos Seguidos de 2: "+f.GetDoses();
			}
			else if(filtro.NombreFiltro==Filtro.NoInterrupciones)
			{
				FiltroInterrupciones f= (FiltroInterrupciones)filtro;
				if(f.GetIntGlobales().Length>0)
					txt="Interrupciones Globales: "+f.GetIntGlobales();
				else if(f.GetIntVar().Length>0)
					txt="Interrupciones de Variantes: "+f.GetIntVar();
				else if(f.GetInt1().Length>0)
					txt="Interrupciones de 1: "+f.GetInt1();
				else if(f.GetIntX().Length>0)
					txt="Interrupciones de X: "+f.GetIntX();
				else if(f.GetInt2().Length>0)
					txt="Interrupciones de 2: "+f.GetInt2();
				else if(f.GetIntGlobalSeg().Length>0)
					txt="Interrupciones Seguidas Globales: "+f.GetIntGlobalSeg();
				else if(f.GetIntVarSeg().Length>0)
					txt="Interrupciones Seguidas de Variantes: "+f.GetIntVarSeg();
				else if(f.GetInt1Seg().Length>0)
					txt="Interrupciones Seguidas de 1: "+f.GetInt1Seg();
				else if(f.GetIntXSeg().Length>0)
					txt="Interrupciones Seguidas de X: "+f.GetIntXSeg();
				else if(f.GetInt2Seg().Length>0)
					txt="Interrupciones Seguidas de 2: "+f.GetInt2Seg();
			}
			else if(filtro.NombreFiltro==Filtro.PesosNumericos )
			{
				FiltroPesosNumericos f=(FiltroPesosNumericos)filtro;
				if(f.GetPNGlobal().Length>0)
					txt="Peso Numérico global: "+f.GetPNGlobal();
				else if(f.GetPNVariantes().Length>0)
					txt="Peso Numérico de Variantes: "+f.GetPNVariantes();
				else if(f.GetPNUnos().Length>0)
					txt="Peso Numérico de 1: "+f.GetPNUnos();
				else if(f.GetPNEquis().Length>0)
					txt="Peso Numérico de X: "+f.GetPNEquis();
				else if(f.GetPNDoses().Length>0)
					txt="Peso Numérico de 2: "+f.GetPNDoses();
			}
			else if(filtro.NombreFiltro==Filtro.Distancias)
			{
				FiltroDistancias f=(FiltroDistancias)filtro;
				if(f.GetIntVar().Length>0)
					txt="Distancia de Variantes: "+f.GetIntVar();
				else if(f.GetInt1().Length>0)
					txt="Distancia de 1: "+f.GetInt1();
				else if(f.GetIntX().Length>0)
					txt="Distancia de X: "+f.GetIntX();
				else if(f.GetInt2().Length>0)
					txt="Distancia de 2: "+f.GetInt2();
			}
			else if(filtro.NombreFiltro==Filtro.Contactos)
			{
				FiltroContactos f=(FiltroContactos)filtro;
				if(f.GetNum11().Length>0)
					txt="Contactos de 11: "+f.GetNum11();
				if(f.GetNumXX().Length>0)
					txt="Contactos de XX: "+f.GetNumXX();
				if(f.GetNum22().Length>0)
					txt="Contactos de 22: "+f.GetNum22();
				if(f.GetNumVV().Length>0)
					txt="Contactos de VV: "+f.GetNumVV();
				if(f.GetNum1X().Length>0)
					txt="Contactos de 1X: "+f.GetNum1X();
				if(f.GetNum12().Length>0)
					txt="Contactos de 12: "+f.GetNum12();
				if(f.GetNum1V().Length>0)
					txt="Contactos de 1V: "+f.GetNum1V();
				if(f.GetNumX2().Length>0)
					txt="Contactos de X2: "+f.GetNumX2();
				if(f.GetNumXV().Length>0)
					txt="Contactos de XV: "+f.GetNumXV();
				if(f.GetNum2V().Length>0)
					txt="Contactos de 2V: "+f.GetNum2V();
			}
			return txt;
		}

		public string getLineaTexto(Grupo grupo, Analizador analizador)
		{
			int i;
			Grupo gr;
			for(i=1;i<=analizador.GruposPartidos.Count;i++)
			{
				gr=analizador.GruposPartidos[i];
				if(gr.NombreGrupo==grupo.NombreGrupo) break;
			}
			return i+" - "+grupo.NombreGrupo;
		}

		public bool CompruebaPronostico(long columna, GrupoPartidos gruposPartidos)
		{
			CondicionIfThen cond;
			GrupoIfThen gr;
			int fallos=0;
			for(int i=0;i<ControlesCondiciones.Count;i++)
			{
				cond=(CondicionIfThen)ControlesCondiciones[i];
				if(cond.CompruebaPronostico(columna)==false) fallos++;
			}
			if(ControlesCondiciones.Count>0)
				if (AciertoCondiciones[ControlesCondiciones.Count-fallos]==false) return false;
			fallos=0;
			for(int i=0;i<ControlesGrupos.Count;i++)
			{
				gr=(GrupoIfThen)ControlesGrupos[i];
				if(gr.CompruebaPronostico(columna, gruposPartidos)==false) fallos++;
			}
			if(ControlesGrupos.Count>0)
				return AciertoGrupos[ControlesGrupos.Count-fallos];
		    return true;
		}

		public string[] CompruebaErrores(long columna)
		{
			// Devuelve un array de errores de condiciones
			CondicionIfThen cond;
			List<string> listaErrores=new List<string>();
			for(int i=0;i<ControlesCondiciones.Count;i++)
			{
				cond=(CondicionIfThen)ControlesCondiciones[i];
				if(cond.CompruebaPronostico(columna)==false) listaErrores.Add("!Fallo en condición relacionada "+Convert.ToString(i+1));
			}
			int n=listaErrores.Count;
			if(n>0)
			{
				if(AciertoCondiciones[ControlesCondiciones.Count-n]==false)
					listaErrores.Add("Rango de aciertos de condiciones fallado");
				else
					listaErrores.Add("*Rango de aciertos de condiciones acertado");
				n++;
			}
			string[] errores=new string[n];
			for(int i=0;i<n;i++)
			{
				errores[i]=listaErrores[i];
			}
			return errores;
		}
	
		public string[] CompruebaErrores(long columna, GrupoPartidos gruposPartidos)
		{
			// Devuelve un array de errores de condiciones
			GrupoIfThen gr;
			ArrayList listaErrores=new ArrayList();
			for(int i=0;i<ControlesGrupos.Count;i++)
			{
				gr=(GrupoIfThen)ControlesGrupos[i];
				if(gr.CompruebaPronostico(columna, gruposPartidos)==false) listaErrores.Add("!Fallo en grupo relacionado "+Convert.ToString(i+1));
			}
			int n=listaErrores.Count;
			if(n>0)
			{
				if(AciertoGrupos[ControlesGrupos.Count-n]==false)
					listaErrores.Add("Rango de aciertos de grupos fallado");
				else
					listaErrores.Add("*Rango de aciertos de grupos acertado");
				n++;
			}
			string[] errores=new string[n];
			for(int i=0;i<n;i++)
			{
				errores[i]=(string)listaErrores[i];
			}
			return errores;
		}
	
	}
}



