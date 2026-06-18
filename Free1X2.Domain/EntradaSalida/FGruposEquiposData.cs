// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
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
using System.Xml;
using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	/// <summary>
	/// Summary description for FGruposEquiposData.
	/// </summary>
	public class FGruposEquiposData: FiltroDatosBase
	{
		private FiltroGruposEquipos _filtro;

		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroGruposEquipos)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;
			
			List<GrupoEquipos> arrayGE = new List<GrupoEquipos>();

		    List<RelacionGE1> arrayRelaciones1 = new List<RelacionGE1>();

		    foreach(XmlNode xmlNode in xmlCondicionDatos.ChildNodes)
			{	
				
				switch(xmlNode.Name.ToUpper())
				{
					case "GRUPOEQUIPOS":
						GrupoEquipos ge = ObtenGrupoEquipos( xmlNode );
						arrayGE.Add( ge );
						break;

					case "RELACION1":
						RelacionGE1 rel1 = ObtenRelacionGE1( xmlNode );
						arrayRelaciones1.Add( rel1 );
						break;									
				}
			}			

			_filtro.GruposEquipos = arrayGE;	
			_filtro.RelacionesGE1.Relaciones = arrayRelaciones1;
            

		}		
		protected RelacionGE1 ObtenRelacionGE1( XmlNode xmlNode )
		{
		    RelacionGE1 rel = new RelacionGE1();

			for(int i = 0; i < xmlNode.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlNode.ChildNodes[i];
				string valoresTipoDesc = xmlSubCondicion.Name;

				switch(valoresTipoDesc)
				{
					case "ge":
						rel.GruposEquipos = xmlSubCondicion.Attributes["values"].Value;
						break;
					case "victorias":
						rel.SumaVictorias = xmlSubCondicion.Attributes["values"].Value;
						break;					
					case "empates":
						rel.SumaEmpates = xmlSubCondicion.Attributes["values"].Value;
						break;	
					case "derrotas":
						rel.SumaDerrotas = xmlSubCondicion.Attributes["values"].Value;
						break;	
					case "puntos":
						rel.SumaPuntos = xmlSubCondicion.Attributes["values"].Value;
						break;	
				}
			}		
		
			return rel;
		}

		protected GrupoEquipos ObtenGrupoEquipos( XmlNode xmlNode )
		{
			GrupoEquipos ge = new GrupoEquipos();

		    for(int i = 0; i < xmlNode.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlNode.ChildNodes[i];
				string valoresTipoDesc = xmlSubCondicion.Name;

				switch(valoresTipoDesc)
				{
					case "equipos":
						ge.Pronosticos = ObtenEquipos(xmlSubCondicion.Attributes["value"].Value);						
						break;
					case "victorias":
						ge.Victorias = xmlSubCondicion.Attributes["value"].Value;
						break;
					case "empates":
						ge.Empates = xmlSubCondicion.Attributes["value"].Value;
						break;
					case "derrotas":
						ge.Derrotas = xmlSubCondicion.Attributes["value"].Value;
						break;
					case "sumaPuntos":
						ge.SumaPuntos = xmlSubCondicion.Attributes["value"].Value;
						break;
				}
			}
            ge.CalcularLongPronosticos();
			return ge;
		
		}


		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			
			_filtro = (FiltroGruposEquipos)filtro;
			
			if( _filtro.ContieneDatos )
			{
				xmlWriter.WriteStartElement("condicion");
				xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
				xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());

				//escribir comentarios de ayuda para modificacion manual
				xmlWriter.WriteComment(" Para cada partido:");
				xmlWriter.WriteComment(" 0 => ningun equipo seleccionado");
				xmlWriter.WriteComment(" 1 => solo equipo casa seleccionado");
				xmlWriter.WriteComment(" 2 => solo equipo fuera seleccionado");
				xmlWriter.WriteComment(" 3 => equipos casa y fuera seleccionados");
				
				EscribirGE(xmlWriter);
				EscribirRelGE1(xmlWriter);
								
				xmlWriter.WriteEndElement(); //para <condicion>
			}		
			
		}

		protected void EscribirGE(XmlTextWriter xmlWriter)
		{
			List<GrupoEquipos> arrayGE = _filtro.GruposEquipos;

		    for(int i = 0; i < arrayGE.Count; i++)
			{
				GrupoEquipos ge = arrayGE[i];
				
				xmlWriter.WriteStartElement("grupoEquipos");
				xmlWriter.WriteAttributeString("id", i.ToString());

				
				EscribirEquipos( xmlWriter, ge );
				
				//Victorias
				xmlWriter.WriteStartElement("victorias");
				xmlWriter.WriteAttributeString("value", ge.Victorias);
				xmlWriter.WriteEndElement();
				//Empate
				xmlWriter.WriteStartElement("empates");
				xmlWriter.WriteAttributeString("value", ge.Empates);
				xmlWriter.WriteEndElement();
				//derrotas
				xmlWriter.WriteStartElement("derrotas");
				xmlWriter.WriteAttributeString("value", ge.Derrotas);
				xmlWriter.WriteEndElement();
				//sumaPuntos
				xmlWriter.WriteStartElement("sumaPuntos");
				xmlWriter.WriteAttributeString("value", ge.SumaPuntos);
				xmlWriter.WriteEndElement();
				
				xmlWriter.WriteEndElement(); //</grupoEquipos>
			}	
		
		}

		protected void EscribirEquipos(XmlTextWriter xmlWriter, GrupoEquipos ge)
		{
			char[] equipos = ge.Pronosticos;
			string strEquipos = "";

			for(int i = 0; i < equipos.Length; i++)
			{
				strEquipos +=  equipos[i];			
			}

			xmlWriter.WriteStartElement("equipos");
			xmlWriter.WriteAttributeString("value", strEquipos);
			xmlWriter.WriteEndElement();		
		}


		protected void EscribirRelGE1(XmlTextWriter xmlWriter)
		{
			List<RelacionGE1> arrayRelaciones1 = _filtro.RelacionesGE1.Relaciones;

		    for(int i = 0; i < arrayRelaciones1.Count; i++)
			{
				RelacionGE1 rel1 = arrayRelaciones1[i];

				xmlWriter.WriteStartElement("Relacion1");
				xmlWriter.WriteAttributeString("id", i.ToString());

				//ge
				xmlWriter.WriteStartElement("ge");
				xmlWriter.WriteAttributeString("values", rel1.GruposEquipos);
				xmlWriter.WriteEndElement();
				//Victorias
				xmlWriter.WriteStartElement("victorias");
				xmlWriter.WriteAttributeString("values", rel1.SumaVictorias);
				xmlWriter.WriteEndElement();
				//Empate
				xmlWriter.WriteStartElement("empates");
				xmlWriter.WriteAttributeString("values", rel1.SumaEmpates);
				xmlWriter.WriteEndElement();
				//derrotas
				xmlWriter.WriteStartElement("derrotas");
				xmlWriter.WriteAttributeString("values", rel1.SumaDerrotas);
				xmlWriter.WriteEndElement();
				//sumaPuntos
				xmlWriter.WriteStartElement("puntos");
				xmlWriter.WriteAttributeString("values", rel1.SumaPuntos);
				xmlWriter.WriteEndElement();

				xmlWriter.WriteEndElement();
			}		
		}

		protected char[] ObtenEquipos(string strEquipos)
		{
			char[] equipos = new char[strEquipos.Length];

			for(int i = 0; i < strEquipos.Length; i++)
			{
				equipos[i] = strEquipos[i];
			}

			return equipos;		
		}
		
	}
}
