// created on 17/01/2004 at 11:58
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2006 Toni Moreno
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
using Free1X2.MotorCalculo;

namespace Free1X2.Analisis
{
	/// <summary>
	/// Núcleo puro del análisis de columnas de combinación (sin UI). Extraído de
	/// <c>Free1X2.Analisis.AnalisisCombinacion.AnalizaColumna</c> (WinForms) para que
	/// el escrutinio de combinaciones (<c>Free1X2.Escrutinio.EscrutadorComb</c>) pueda
	/// vivir en el dominio sin depender de TreeView/Form. La lógica numérica es idéntica
	/// al original; solo se ha separado de la parte de presentación (AnalizarCombinacion).
	/// </summary>
	public class AnalizadorColumnaCombinacion
	{
		private bool[] grupos;
		private bool[] conjuntos;
		private bool[] conjuntos2;

		public bool AnalizaColumna(long columna, MotorCalculo.Analizador analizador, string[] pronosticosBase)
		{
			string[] evaluacionFiltro;
			IFiltro filtro;
			grupos = new bool[analizador.GruposPartidos.Count];

			for (int i = 0; i < analizador.GruposPartidos.Count; i++)
			{
				Grupo g = analizador.GruposPartidos[i];
				// Si es el boleto base, comprueba el pronóstico.
				if (i == 0)
				{
					string[] evaluacionPronosticos = evaluarPronosticos(columna, pronosticosBase);
					if (evaluacionPronosticos != null) return false;
				}
				// Si el grupo no contiene todos los partidos, obtiene la columna a analizar
				long columnaAAnalizar = g.ColumnaGrupo(columna);
				for (int f = 0; f < g.Filtros.Count; f++)
				{
					filtro = g.Filtros[f];
					if (filtro.IsActive)
					{
						if (!filtro.Analizar(columnaAAnalizar)) return false;
					}
				}
				if (g.ControladorTolerancias.Tolerancias.Count > 0)
				{
					if (!g.AnalizaToleranciasGrupo(columnaAAnalizar)) return false;
				}
			}
			// Analiza el control de grupos
			conjuntos = new bool[analizador.CtrlGrupos.ControlesGrupos.Count];

			if (conjuntos.Length > 1)
			{
				for (int i = 1; i < conjuntos.Length; i++)
				{
					ControlGrupos cg = analizador.CtrlGrupos.ControlesGrupos[i];
					if (!AnalizaFallosGrupos(cg)) return false;
				}

				// Comprueba el control de conjuntos
				conjuntos2 = new bool[analizador.CtrlGrupos.ControlesConjuntos.Count];
				if (conjuntos2.Length > 1)
				{
					for (int i = 1; i < analizador.CtrlGrupos.ControlesConjuntos.Count; i++)
					{
						ControlConjuntos cc = analizador.CtrlGrupos.ControlesConjuntos[i];
						if (!AnalizaFallosConjuntos(cc)) return false;
					}
				}
			}

			// Analiza el controlador If-Then
			if (analizador.IfThen != null)
			{
				if (!analizador.IfThen.EsVacio && analizador.IfThen.EsActivo)
				{
					if (analizador.IfThen.ControlesCondiciones.Count > 0)
					{
						evaluacionFiltro = analizador.IfThen.CompruebaErrores(columna);
						if (evaluacionFiltro.Length > 0) return false;
					}
					if (analizador.IfThen.ControlesGrupos.Count > 0)
					{
						evaluacionFiltro = analizador.IfThen.CompruebaErrores(columna, analizador.GruposPartidos);
						if (evaluacionFiltro.Length > 0) return false;
					}
				}
			}

			return true;
		}

		private string[] evaluarPronosticos(long cg, string[] pronosticos)
		{
			string[] arrayFallos=null;
            int partido = 0;
            string fallos = "";
            while (cg != 0)
            {
                int signo = (int)cg & 7;
                int pronostico = Utils.UtilColumnas.ConvPartidoStrToByte(pronosticos[partido]);
                if ((signo & pronostico) != signo)
                {
                    fallos = "#" + Convert.ToString(partido + 1) + fallos;
                }
                cg >>= 3;
                partido++;
            }
		    if (fallos.Length >= 2)
		    {
		        string fallosDef = fallos.Substring(1, fallos.Length - 1);
		        arrayFallos = fallosDef.Split('#');
		    }
		    return arrayFallos;
		}

		private bool AnalizaFallosGrupos(ControlGrupos c)
		{
			int numFallos=0;
		    for(int i=0;i<c.GruposControlados.Length ;i++)
		    {
		        int numGrupo = c.GruposControlados[i];
		        if(grupos[numGrupo]==false) numFallos++;
		    }
		    if(c.FallosPermitidos[numFallos]) return true;
			return false;
		}

		private bool AnalizaFallosConjuntos(ControlConjuntos c)
		{
			int numFallos=0;
		    bool[] permitidos=c.ObtenFallosPermitidos();
			int[] conj=c.ObtenCtrolGruposConjunto();
			for(int i=0;i<conj.Length ;i++)
			{
			    int numConjunto = conj[i];
			    if(conjuntos[numConjunto]==false) numFallos++;
			}
		    if(permitidos[numFallos]) return true;
			return false;
		}
	}
}
