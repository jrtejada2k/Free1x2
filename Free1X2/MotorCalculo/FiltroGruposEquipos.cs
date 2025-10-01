// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 xfsf
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
using System.Collections.Generic;


namespace Free1X2.MotorCalculo
{
	/// <summary>
	/// Summary description for FiltroGruposEquipos.
	/// </summary>
	public class FiltroGruposEquipos: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        List<long> figuras;

		private List<GrupoEquipos> gEquipos;
		private ControladorRelacionesGE1 controlRelaciones1;

		public FiltroGruposEquipos()
		{
		    gEquipos = new List<GrupoEquipos>();	
			controlRelaciones1 = new ControladorRelacionesGE1();
			controlRelaciones1.GruposEquipos = gEquipos;
		}

		public bool Analizar(long columna)
		{
			//foreach( GrupoEquipos ge in gEquipos )
            for(int i = 0; i < gEquipos.Count; i++)
			{
                if (!gEquipos[i].Analizar(columna))
				{
					return false;
				}
			}
            return controlRelaciones1.Analiza();
		}

		public string[] AnalizarFallos(long columna)
		{
			string txt;
			string texto="";
			string[] arrayFallos=null;

			int numGrupo=0;
			foreach( GrupoEquipos ge in gEquipos )
			{
				numGrupo++;
				txt=ge.Analizar( columna, numGrupo);
				texto+=txt;
			}
			txt=controlRelaciones1.AnalizaRelaciones();
			texto+=txt;
			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
			return arrayFallos;
		}

		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			//este filtro no usa tolerancias.
			return 0;
		}

		public List<GrupoEquipos> GruposEquipos
		{
			get{ return gEquipos; }
			set
			{ 
				gEquipos = value; 	
			
				//añadir referencia a relaciones
				controlRelaciones1.GruposEquipos = gEquipos;
				
				//comprobar si ArrayList contiene valores
				if(gEquipos != null && gEquipos.Count > 0)
				{
					contieneDatos = true;											
				}
				else
				{
					contieneDatos = false;
				}
			}
		}

		public ControladorRelacionesGE1 RelacionesGE1
		{
			get{ return controlRelaciones1; }
			set{ controlRelaciones1 = value; }		
		}

		public Filtro NombreFiltro
		{
			get{ return Filtro.GruposEquipos; }
		}

		public bool EsNombreFiltro(string nombre)
		{
		    if( nombre.Equals( Filtro.GruposEquipos.ToString() ) )
			{
				return true;
			}
		    return false;
		}

	    public bool IsActive
		{
			get{ return isActive; } 
			set{ isActive = value; }
		}
		
		public bool ContieneDatos
		{
			get { return contieneDatos; }
			set { contieneDatos = value; }		
		}
        public bool UsaFiguras()
        {
            if ((Figuras == null) || (Figuras.Count == 0))
            {
                return false;
            }
            return true;
        }

	    public List<long> Figuras
        {
            get
            {
                return figuras;
            }
            set
            {
                figuras = value;
            }
        }
        public bool AnalisisActivo
        {
            get { return VariablesGlobales.AnalizarGruposEquipos; }
        }
	}
}

