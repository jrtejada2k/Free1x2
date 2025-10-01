// created on 23/08/2003 at 13:52
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
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

using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	public class DatosFiltroHelper
	{
	
		public static void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			IFiltroDatos filtroDatos = null;
			
			switch(filtro.NombreFiltro)
			{
				case Filtro.NoVariantes:
					filtroDatos = new FNoVariantesData();
					break;
				case Filtro.SignosSeguidos:
					filtroDatos = new FSignosSeguidosData();
					break;
				case Filtro.Dibujos:
					filtroDatos = new FDibujosData();
					break;
				case Filtro.ColProbables:
					filtroDatos = new FColProbablesData();
					break;
				case Filtro.PesosNumericos:
					filtroDatos = new FPesosNumericosData();
					break;
				case Filtro.ValoracionSignos:
					filtroDatos = new FValoracionData();
					break;
				case Filtro.NoInterrupciones:
					filtroDatos = new FInterrupcionesData();
					break;
				case Filtro.Distancias:
					filtroDatos = new FDistanciasData();
					break;
				case Filtro.GruposEquipos:
					filtroDatos = new FGruposEquiposData();
					break;
				case Filtro.Contactos:
					filtroDatos = new FContactosData();
					break;
				case Filtro.FormatosSignos:
					filtroDatos = new FFormatosSignosData();
					break;
                case Filtro.Formatos123:
                    filtroDatos = new FFormatos123Data();
                    break;

                case Filtro.Simetrias:
                    filtroDatos = new FSimetriasData();
                    break;
                case Filtro.Diferencias:
                    filtroDatos = new FDiferenciasData();
                    break;
			}

		    if (filtroDatos != null) filtroDatos.PonerCondicionesFiltro( filtro, xmlCondicionDatos );
		}
						
		public static void EscribirCondicionesFiltros(Grupo grupo, XmlTextWriter xmlWriter)
		{
			IFiltroDatos filtroDatos = null;
			
			foreach(IFiltro filtro in grupo.Filtros)
			{
			    switch(filtro.NombreFiltro)
				{
					case Filtro.NoVariantes:
						filtroDatos = new FNoVariantesData();
						break;
					case Filtro.SignosSeguidos:
						filtroDatos = new FSignosSeguidosData();
						break;	
					case Filtro.Dibujos:
						filtroDatos = new FDibujosData();
						break;
					case Filtro.ColProbables:
						filtroDatos = new FColProbablesData();
						break;
					case Filtro.PesosNumericos:
						filtroDatos = new FPesosNumericosData();
						break;
					case Filtro.ValoracionSignos:
						filtroDatos = new FValoracionData();
						break;
					case Filtro.NoInterrupciones:
						filtroDatos = new FInterrupcionesData();
						break;
					case Filtro.Distancias:
						filtroDatos = new FDistanciasData();
						break;
					case Filtro.GruposEquipos:
						filtroDatos = new FGruposEquiposData();
						break;
					case Filtro.Contactos:
						filtroDatos = new FContactosData();
						break;
					case Filtro.FormatosSignos:
						filtroDatos = new FFormatosSignosData();
						break;
                    case Filtro.Formatos123:
                        filtroDatos = new FFormatos123Data();
                        break;

                    case Filtro.Simetrias:
                        filtroDatos = new FSimetriasData();
                        break;
                    case Filtro.Diferencias:
                        filtroDatos = new FDiferenciasData();
                        break;
				}
			    if (filtroDatos != null) filtroDatos.EscribirCondicionesFiltros(filtro, xmlWriter);
			}
		}
	}
}
