// Free1X2 · WinUI 3 — WIN3
// created on 23/08/2003 at 14:21
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

using System;
using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	public class FNoVariantesData: FiltroDatosBase
	{
		private FiltroNoVariantes _filtro;
		
		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			
			_filtro = (FiltroNoVariantes)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;

		    for( int i = 0; i < xmlCondicionDatos.ChildNodes.Count; i++)
		    {
		        XmlNode xmlSubCondicion1 = xmlCondicionDatos.ChildNodes[i];

		        PonerValoresStandard( xmlSubCondicion1 );
		    }
		}
		
		protected void PonerValoresStandard(XmlNode xmlValoresStandard)
		{
		    for(int i = 0; i < xmlValoresStandard.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlValoresStandard.ChildNodes[i];
				
				string valoresTipoDesc = xmlSubCondicion.Attributes["tipo"].Value;

                string valores = xmlSubCondicion.Attributes["values"].Value;
				
				if( valoresTipoDesc.Equals("var") )
				{
					_filtro.SetNoVariantes( valores );
				}
				else if( valoresTipoDesc.Equals("equis") )
				{
					_filtro.SetNoEquis( valores );
				}
				else if( valoresTipoDesc.Equals("doses") )
				{
					_filtro.SetNoDoses( valores );
				}			
			}			
		}
			
		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			_filtro = (FiltroNoVariantes)filtro;
			
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
			xmlWriter.WriteStartElement("standard");
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "var");
			xmlWriter.WriteAttributeString("values", _filtro.GetVariantes());
			xmlWriter.WriteEndElement();
			
			//X
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "equis");
			xmlWriter.WriteAttributeString("values", _filtro.GetEquis());
			xmlWriter.WriteEndElement();
			
			//2
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "doses");
			xmlWriter.WriteAttributeString("values", _filtro.GetDoses());
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteEndElement();
			
		}		
	}
}
