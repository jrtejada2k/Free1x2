// Free1X2 · WinUI 3 — WIN3
// created on 11/11/2003 at 19:52
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
// Copyright (C) 2008 Morrison - morrison [dot] ne [at] gmail [dot] com
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
	public class FiltroColProbables: IFiltro
	{
		private bool isActive;
		private bool contieneDatos;
        protected List<long> figuras;
		private List<ColumnaProbable> colProbables;

		private ControladorRelacionesCP1 controlRelaciones1;
        private ControladorRelacionesCP2 controlRelaciones2;
        private ControladorRelacionesCP3 controlRelaciones3;
		private ControladorCPControlFallos controlFallosCP;

        
		
		public FiltroColProbables()
		{
			colProbables = new List<ColumnaProbable>();
			
			controlRelaciones1 = new ControladorRelacionesCP1();					
			controlRelaciones1.ColumnasProbables = colProbables;
			
			controlFallosCP = new ControladorCPControlFallos();
			controlFallosCP.ColumnasProbables = colProbables;

            controlRelaciones2 = new ControladorRelacionesCP2();
            controlRelaciones2.ColumnasProbables = colProbables;

            controlRelaciones3 = new ControladorRelacionesCP3();
		}

        public bool Analizar(long columna)
        {
           // foreach (ColumnaProbable cp in colProbables)
            for(int i = 0; i < colProbables.Count; i++)
            {
                if(!colProbables[i].Analizar(columna))
                {
                    //do not check any more groups
                    return false;
                }
            }

            if (!controlFallosCP.Analiza())
            {
                return false;
            }
            //comprobar relaciones de columnas...
            if (!controlRelaciones1.Analiza())
            {
                return false;
            }

            //Analizar Relaciones 2 sólo si hay alguna definida
            if (this.controlRelaciones2.Relaciones2.Count > 0)
            {
                if (!AnalizarRelaciones2())
                {
                    return false;
                }
            }

            //Analizar Relaciones 2 sólo si hay alguna definida
            if (this.controlRelaciones3.Relaciones.Count > 0)
            {
                return AnalizarRelaciones3();
            }

            return true;
        }
        protected bool AnalizarRelaciones2()
        {
            bool esValida = false;
            for (int i = 0; i < this.controlRelaciones2.Relaciones2.Count; i++)
            {
                esValida = this.controlRelaciones2.Relaciones2[i].Analizar();
                if (!esValida)
                {
                    break;
                }
            }
            return esValida;
        }
        protected bool AnalizarRelaciones3()
        {
            bool esValida = false;
            esValida = this.controlRelaciones3.AnalizaRelaciones();


            return esValida;
        }
        public string[] AnalizarFallos(long columna)
		{
			string[] arrayFallos=null;
			string txt="";
			string texto="";
			int numCol=0;
			foreach( ColumnaProbable cp in colProbables )
			{
				numCol++;
				txt = cp.Analizar( columna, numCol);
				texto+=txt;
			}
			// Control de fallos
			controlFallosCP.Analiza(ref txt, texto.Length>0);
			texto+=txt;
			
			//comprobar relaciones de columnas...
			controlRelaciones1.Analiza(ref txt);
			texto+=txt;

            //comprobar relaciones II
            controlRelaciones2.Analiza(ref txt);
            texto += txt;

            //comprobar relaciones III
            controlRelaciones3.Analiza(ref txt);
            texto += txt;

			if(texto.Length>0)
			{
				texto=texto.Substring(0,texto.Length-1);
				arrayFallos=texto.Split('#');
			}
			return arrayFallos;
		}

		public void InicializaPuntosCP(int puntosFijos, int puntosDobles, int puntosTriples)
		{
		
			foreach(ColumnaProbable cp in colProbables)
			{
				cp.InicializaPuntosCP(puntosFijos, puntosDobles, puntosTriples);
			}
		}
		
		public List<ColumnaProbable> ColProbables
		{
			get{ return colProbables; }
			set
			{ 
				colProbables = value; 

				//añadir referencia a columnas probables
				controlRelaciones1.ColumnasProbables = colProbables;
				controlFallosCP.ColumnasProbables = colProbables;
                controlRelaciones2.ColumnasProbables = colProbables;
				
				//comprobar si ArrayList contiene valores
				if(colProbables != null && colProbables.Count > 0)
				{
					contieneDatos = true;											
				}
				else
				{
					contieneDatos = false;
				}
			}
		}

		public ControladorRelacionesCP1 RelacionesCP1
		{
			get{ return controlRelaciones1; }
			set{ controlRelaciones1 = value; }		
		}
        public ControladorRelacionesCP2 RelacionesCP2
        {
            get { return controlRelaciones2; }
            set { controlRelaciones2 = value; }
        }
        public ControladorRelacionesCP3 RelacionesCP3
        {
            get { return controlRelaciones3; }
            set { controlRelaciones3 = value; }
        }
		public ControladorCPControlFallos ControlFallosCP
		{
			get{ return controlFallosCP; }
			set{ controlFallosCP = value; }		
		}
		
		public Filtro NombreFiltro
		{
			get{ return Filtro.ColProbables; }
		}
		
		public bool EsNombreFiltro(string nombre)
		{
			
			if( nombre.Equals( Filtro.ColProbables.ToString() ) )
			{
				return true;
			}
			else
			{
				return false;	
			}
			
		}
		
		public int ObtenNoAciertosTolerancias(string letrasTolerancias)
		{
			int noTolerancias = 0;

			foreach( ColumnaProbable cp in colProbables )
			{
				noTolerancias += cp.ObtenNoAciertosTolerancias( letrasTolerancias );
			}			
			
			return noTolerancias;
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

        #region Miembros de IFiltro


        public bool UsaFiguras()
        {
            if ((this.Figuras == null) || (this.Figuras.Count == 0))
            {
                return false;
            }
            else
            {
                return true;
            }
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
            get { return VariablesGlobales.AnalizarCPs; }
        }

        #endregion
    }
}
