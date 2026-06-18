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
using System.Collections.Generic;
using Free1X2.Utils;

namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for RelacionGE1.
	/// </summary>
	public class RelacionGE1
	{
		private string strGEquipos = "";
		private string strSumaVictorias = "";
		private string strSumaEmpates = "";
		private string strSumaDerrotas = "";
		private string strSumaPuntos = "";

		private int[] geRelacionados;
		private bool[] sumaVictorias;
		private bool[] sumaEmpates;
		private bool[] sumaDerrotas;
		private bool[] sumaPuntos;
		

		//el controlador es el objeto parent que contiene a este objeto
		protected ControladorRelacionesGE1 controlador;

		protected List<GrupoEquipos> gruposEquipos;

	    public bool Analiza()
		{
		//	bool relacionValida = true;

			if(	strSumaVictorias != "" || strSumaEmpates != "" ||
				strSumaDerrotas != "" || strSumaPuntos != "" )
			{
				return AnalizaSumas();
			}			

			return 	true;
		}

		public string Analiza(int numRelacion)
		{
			string txt="";
			if(	strSumaVictorias != "" || strSumaEmpates != "" ||
				strSumaDerrotas != "" || strSumaPuntos != "" )
			{
				txt=AnalizaSumas(numRelacion);
			}
			return txt;
		}

		protected bool AnalizaSumas()
		{
			int sumaVictoriasTemp = 0;
			int sumaEmpatesTemp = 0;
			int sumaDerrotasTemp = 0;
			int sumaPuntosTemp = 0;

		    for(int i = 0; i < geRelacionados.Length; i++)
			{
				GrupoEquipos ge = gruposEquipos[ geRelacionados[i]-1 ];
				sumaVictoriasTemp += ge.NoVictorias;
				sumaEmpatesTemp += ge.NoEmpates;
				sumaDerrotasTemp += ge.NoDerrotas;
				sumaPuntosTemp += ge.NoSumaPuntos;
			}

			if(strSumaPuntos != "")
			{
				if(sumaPuntos.Length < sumaPuntosTemp+1 || sumaPuntos[ sumaPuntosTemp ] == false)
				{
					return false;
				}			
			}

			if(strSumaVictorias != "")
			{
				if(sumaVictorias.Length < sumaVictoriasTemp+1 || sumaVictorias[ sumaVictoriasTemp ] == false)
				{
					return false;
				}				
			}

			if(strSumaEmpates != "")
			{
				if(sumaEmpates.Length < sumaEmpatesTemp+1 || sumaEmpates[ sumaEmpatesTemp ] == false)
				{
					return false;
				}				
			}

			if(strSumaDerrotas != "")
			{
				if(sumaDerrotas.Length < sumaDerrotasTemp+1 || sumaDerrotas[ sumaDerrotasTemp ] == false)
				{
					return false;
				}				
			}			
			
			return true;		
		}

		protected string AnalizaSumas(int numRelacion)
		{
			string txt="";
			int sumaVictoriasTemp = 0;
			int sumaEmpatesTemp = 0;
			int sumaDerrotasTemp = 0;
			int sumaPuntosTemp = 0;

		    for(int i = 0; i < geRelacionados.Length; i++)
			{
				GrupoEquipos ge = gruposEquipos[ geRelacionados[i]-1 ];
				sumaVictoriasTemp += ge.NoVictorias;
				sumaEmpatesTemp += ge.NoEmpates;
				sumaDerrotasTemp += ge.NoDerrotas;
				sumaPuntosTemp += ge.NoSumaPuntos;
			}

			if(strSumaPuntos != "")
			{
				if(sumaPuntos.Length < sumaPuntosTemp+1 || sumaPuntos[ sumaPuntosTemp ] == false)
					txt+="Fallo en Suma de Puntos de la relación "+numRelacion+".  ("+sumaPuntosTemp+")#";
			}

			if(strSumaVictorias != "")
			{
				if(sumaVictorias.Length < sumaVictoriasTemp+1 || sumaVictorias[ sumaVictoriasTemp ] == false)
					txt+="Fallo en Nº de Victorias de la relación "+numRelacion+".  ("+sumaVictoriasTemp+")#";
			}

			if(strSumaEmpates != "")
			{
				if(sumaEmpates.Length < sumaEmpatesTemp+1 || sumaEmpates[ sumaEmpatesTemp ] == false)
					txt+="Fallo en Nº de Empates de la relación "+numRelacion+".  ("+sumaEmpatesTemp+")#";
			}

			if(strSumaDerrotas != "")
			{
				if(sumaDerrotas.Length < sumaDerrotasTemp+1 || sumaDerrotas[ sumaDerrotasTemp ] == false)
					txt+="Fallo en Nº de Derrotas de la relación "+numRelacion+".  ("+sumaDerrotasTemp+")#";
			}			
			return txt;		
		}

		public ControladorRelacionesGE1 Controlador
		{
			set
			{ 
				controlador = value;
				gruposEquipos = controlador.GruposEquipos;
			} 
		}

		public string GruposEquipos
		{
			get{ return strGEquipos; }
			set
			{ 
				strGEquipos = value;

				RangosHelper rangosHelper = new RangosHelper();
				geRelacionados = rangosHelper.ObtenIntArray( strGEquipos );
			}
		}

		public string SumaVictorias
		{
			get{ return strSumaVictorias;}
			set
			{ 
				strSumaVictorias = value;
			
				if(strSumaVictorias != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					sumaVictorias = rangosHelper.ObtenBoolArray(strSumaVictorias);
				}
				else
				{
					sumaVictorias = null;
				}			
			}		
		}

		public string SumaEmpates
		{
			get{ return strSumaEmpates;}
			set
			{ 
				strSumaEmpates = value;
			
				if(strSumaEmpates != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					sumaEmpates = rangosHelper.ObtenBoolArray(strSumaEmpates);
				}
				else
				{
					sumaEmpates = null;
				}			
			}		
		}

		public string SumaDerrotas
		{
			get{ return strSumaDerrotas;}
			set
			{ 
				strSumaDerrotas = value;
			
				if(strSumaDerrotas != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					sumaDerrotas = rangosHelper.ObtenBoolArray(strSumaDerrotas);
				}
				else
				{
					sumaDerrotas = null;
				}			
			}		
		}

		public string SumaPuntos
		{
			get{ return strSumaPuntos;}
			set
			{ 
				strSumaPuntos = value;
			
				if(strSumaPuntos != "")
				{
					RangosHelper rangosHelper = new RangosHelper();
					sumaPuntos = rangosHelper.ObtenBoolArray(strSumaPuntos);
				}
				else
				{
					sumaPuntos = null;
				}			
			}		
		}
	}
}
