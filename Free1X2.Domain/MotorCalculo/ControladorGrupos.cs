// Free1X2 · WinUI 3 — WIN3
// created on 24/11/2003 at 19:59
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer [at] onetel [dot] net [dot] uk
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

using Free1X2.Analisis;

namespace Free1X2.MotorCalculo
{
	public class ControladorGrupos
	{
		private List<ControlGrupos> controlesGrupos;
        private List<ControlConjuntos> controlesConjuntos;

		private GrupoPartidos gruposPartidos;

		private bool usaControlConjuntos;

		protected bool reCalcularControladorGrupo = true;
		protected bool controladorGrupoValidoMemoria = true;
	
		public ControladorGrupos()
		{
			controlesGrupos = new List<ControlGrupos>();
            controlesConjuntos = new List<ControlConjuntos>();	
			
			//este primer ctrlGrupo contiene los grupos que no entran en control de fallos.
			//Poner inicialmente el grupo 0 que es el boleto base.
			ControlGrupos ctrlGrupo = new ControlGrupos();
						
			int[] gruposSeleccionados = new int[1];
			gruposSeleccionados[0] = 0; //grupo/boleto base
			
			ctrlGrupo.GruposControlados = gruposSeleccionados;
			ctrlGrupo.UsaControlGrupos = false;
			
			AddControlGrupo( ctrlGrupo );	
			
			//control de conjuntos de conjuntos "libres"
			ControlConjuntos cConj = new ControlConjuntos();
			//inicializar con el grupo 0 que es el base y siempre sera un grupo "libre"
			cConj.PonerCtrlGruposControlados("0");
			cConj.PonerFallosPermitidos("0");

			controlesConjuntos.Add( cConj );
		}

        public bool AnalizaColumna(long columna)
        {
            //bool columnaValida;

            if (reCalcularControladorGrupo)
            {
                reCalcularControladorGrupo = false;
                if (usaControlConjuntos)
                {
                    controladorGrupoValidoMemoria = AnalizaColumnaConConjuntos(columna);
                }
                else
                {
                    controladorGrupoValidoMemoria = AnalizaColumnaSinConjuntos(columna);
                }
            }
            return controladorGrupoValidoMemoria;
        }

		public bool AnalizaColumnaSinConjuntos( long columna )
		{
            for (int i = 0; i < controlesGrupos.Count; i++)
            {
                if (!controlesGrupos[i].AnalizaColumna(columna))
                {
                    return false;
                }
            }

            return true;
		}
		public bool AnalizaColumnaConConjuntos( long columna )
		{
			bool columnaValida = true;

			ControlGrupos ctrlGrupo;

		    bool grupoValido;

		    //primero analiza los grupos libres...
			ControlConjuntos ctrlConjuntos = controlesConjuntos[ 0 ];

			int[] gruposConjunto = ctrlConjuntos.ObtenCtrolGruposConjunto();
			bool[] fallosPermitidos;

			for(int j = 0; j < gruposConjunto.Length; j++)
			{
				ctrlGrupo = controlesGrupos[ gruposConjunto[j] ];
				grupoValido = ctrlGrupo.AnalizaColumna( columna );

				if(!grupoValido)
				{
					columnaValida = false;
					break;
				}
			}
			
			//si los grupos libres estas acertados, analiza el resto...
			if(columnaValida)
			{
				for(int i = 1; i < controlesConjuntos.Count; i++)
				{
					ctrlConjuntos = controlesConjuntos[ i ];
				
					gruposConjunto = ctrlConjuntos.ObtenCtrolGruposConjunto();
					fallosPermitidos = ctrlConjuntos.ObtenFallosPermitidos();

					int noCtrlGruposFallados = 0;

					for(int j = 0; j < gruposConjunto.Length; j++)
					{
						ctrlGrupo = controlesGrupos[ gruposConjunto[j] ];
						grupoValido = ctrlGrupo.AnalizaColumna( columna );

						if(!grupoValido)
						{
							noCtrlGruposFallados++;
						}
					}

					if(fallosPermitidos.Length < noCtrlGruposFallados+1 || fallosPermitidos[noCtrlGruposFallados] == false)
					{
						columnaValida = false;
						break;
					}
				}
			}

			return columnaValida;
		}

        public bool AnalizaColumna(long columna, ContenedorAnalisisGlobal contenedor)
        {
            bool columnaValida;

            if (reCalcularControladorGrupo)
            {
                if (usaControlConjuntos)
                {
                    columnaValida = AnalizaColumnaConConjuntos(columna, contenedor);
                }
                else
                {
                    columnaValida = AnalizaColumnaSinConjuntos(columna, contenedor);
                }

                controladorGrupoValidoMemoria = columnaValida;
                reCalcularControladorGrupo = false;
            }
            else
            {
                columnaValida = controladorGrupoValidoMemoria;
            }

            return columnaValida;
        }

        public bool AnalizaColumnaSinConjuntos(long columna, ContenedorAnalisisGlobal contenedor)
        {
            bool columnaValida = true;

            for (int i = 0; i < controlesGrupos.Count; i++)
            {
                ControlGrupos ctrlGrupo = controlesGrupos[i];

                    columnaValida = ctrlGrupo.AnalizaColumna(columna, contenedor);
                    

                if (columnaValida == false)
                {
                    //do not check more groups
                    break;
                }
            }

            return columnaValida;
        }
        public bool AnalizaColumnaConConjuntos(long columna, ContenedorAnalisisGlobal contenedor)
        {
            bool columnaValida = true;

            ControlGrupos ctrlGrupo;

            bool grupoValido;
            int noCtrlGruposFallados;

            //primero analiza los grupos libres...
            ControlConjuntos ctrlConjuntos = controlesConjuntos[0];

            int[] gruposConjunto = ctrlConjuntos.ObtenCtrolGruposConjunto();

            for (int j = 0; j < gruposConjunto.Length; j++)
            {
                ctrlGrupo = controlesGrupos[gruposConjunto[j]];
                if (j == 0)
                {
                    grupoValido = ctrlGrupo.AnalizaColumna(columna, contenedor);
                }
                else
                {
                    contenedor.VaciarInformacion();
                    grupoValido = ctrlGrupo.AnalizaColumna(columna,contenedor);
                }

                if (!grupoValido)
                {
                    columnaValida = false;
                    break;
                }
            }

            //si los grupos libres estas acertados, analiza el resto...
            if (columnaValida)
            {
                for (int i = 1; i < controlesConjuntos.Count; i++)
                {
                    ctrlConjuntos = controlesConjuntos[i];

                    gruposConjunto = ctrlConjuntos.ObtenCtrolGruposConjunto();
                    bool[] fallosPermitidos = ctrlConjuntos.ObtenFallosPermitidos();

                    noCtrlGruposFallados = 0;

                    for (int j = 0; j < gruposConjunto.Length; j++)
                    {
                        ctrlGrupo = controlesGrupos[gruposConjunto[j]];
                        grupoValido = ctrlGrupo.AnalizaColumna(columna);

                        if (!grupoValido)
                        {
                            noCtrlGruposFallados++;
                        }
                    }

                    if (fallosPermitidos.Length < noCtrlGruposFallados + 1 || fallosPermitidos[noCtrlGruposFallados] == false)
                    {
                        columnaValida = false;
                        break;
                    }
                    contenedor.ColumnasPorFallosDeConjuntos[noCtrlGruposFallados]++;
                    contenedor.ColumnasPorFallosDeGrupos[noCtrlGruposFallados]++;
                }
            }

            return columnaValida;
        }
        public void RecalcularControladorGrupos()
		{
			reCalcularControladorGrupo = true;

		    for(int i = 0; i < controlesGrupos.Count; i++)
		    {
                controlesGrupos[i].RecalcularControlGrupos();
		    }
		}
		
		public void AddControlGrupo( ControlGrupos ctrlGrupo )
		{
			ctrlGrupo.CtrlGrupos = this;
			controlesGrupos.Add( ctrlGrupo );	
		}
		
		public GrupoPartidos GruposPartidos
		{
			get { return gruposPartidos; }
			set { gruposPartidos = value; }
		}
		
		public List<ControlGrupos> ControlesGrupos
		{
			get { return controlesGrupos; }
			set { controlesGrupos = value; }
		}

        public List<ControlConjuntos> ControlesConjuntos
		{
			get { return controlesConjuntos; }
			set 
			{ 
				controlesConjuntos = value; 
				if(controlesConjuntos.Count > 1)
				{
					usaControlConjuntos = true;
				}
			}
		}

		public bool UsaControlConjuntos
		{
			get { return usaControlConjuntos; }
			set { usaControlConjuntos = value; }
		}
	
	}
}



