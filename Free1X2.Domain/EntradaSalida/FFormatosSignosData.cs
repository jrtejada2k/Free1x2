// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2005 Luis Fernandez - luifer@onetel.com
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
	/// Summary description for FFormatosSignosData.
	/// </summary>
	public class FFormatosSignosData: FiltroDatosBase
	{
		private FiltroFormatosSignos _filtro;

	    public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroFormatosSignos)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;

			List<FormatosSignos> arrayFormatos = new List<FormatosSignos>();

		    XmlNode xmlSubCondicion1;
			
			for( int i = 0; i < xmlCondicionDatos.ChildNodes.Count; i++)
			{
				xmlSubCondicion1 = xmlCondicionDatos.ChildNodes[i];
				
				FormatosSignos formatos = new FormatosSignos();
				formatos.Lineas = xmlSubCondicion1.Attributes["lineas"].Value;
				formatos.Global = xmlSubCondicion1.Attributes["global"].Value;

				List<FormatoSignos> lineas = new List<FormatoSignos>();

			    for( int j = 0; j < xmlSubCondicion1.ChildNodes.Count; j++)
				{
					XmlNode xmlSubCond2 = xmlSubCondicion1.ChildNodes[j];

					FormatoSignos lineaFormato = new FormatoSignos();
					lineaFormato.Formato = xmlSubCond2.Attributes["formato"].Value;
					lineaFormato.RangoAparicion = xmlSubCond2.Attributes["rango"].Value;
					
					lineas.Add(lineaFormato);
				}

				formatos.LineasFormatos = lineas;

				arrayFormatos.Add(formatos);
			}		

			_filtro.FormatosSignos = arrayFormatos;		
		}

		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{			
			_filtro = (FiltroFormatosSignos)filtro;
			
			if( _filtro.ContieneDatos )
			{
				xmlWriter.WriteStartElement("condicion");
				xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
				xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());
				
				EscribirValoresStandardFiltro(xmlWriter);
								
				xmlWriter.WriteEndElement();
			}				
		}

		protected void EscribirValoresStandardFiltro(XmlTextWriter xmlWriter)
		{
			List<FormatosSignos> arrayFormatos = _filtro.FormatosSignos;


		    for(int i = 0; i < arrayFormatos.Count; i++)
			{
				FormatosSignos formatos = arrayFormatos[i];

				xmlWriter.WriteStartElement("grupoFormato");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("lineas", formatos.Lineas);
				xmlWriter.WriteAttributeString("global", formatos.Global);

				List<FormatoSignos> lineas = formatos.LineasFormatos;

				for(int j = 0; j < lineas.Count; j++)
				{
					FormatoSignos lineaFormato = lineas[j];

					xmlWriter.WriteStartElement("lineaFormato");
					xmlWriter.WriteAttributeString("id", j.ToString());

					xmlWriter.WriteAttributeString("formato", lineaFormato.Formato);
					xmlWriter.WriteAttributeString("rango", lineaFormato.RangoAparicion);
					xmlWriter.WriteEndElement();
				}				

				xmlWriter.WriteEndElement(); //grupoFormato
			}						
		}
	}
}
