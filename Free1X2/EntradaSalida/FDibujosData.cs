// created on 14/09/2003 at 19:35
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
using System.Collections;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	public class FDibujosData: FiltroDatosBase
	{
		private FiltroDibujos _filtro;
		
		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroDibujos)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;
			
			ArrayList dibujos = new ArrayList();
			
			foreach(XmlNode xmlDib in xmlCondicionDatos.ChildNodes)
			{			
				dibujos.Add( xmlDib.InnerText );
			}
			
			_filtro.Dibujos = dibujos;			
			
		}
		
		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			_filtro = (FiltroDibujos)filtro;
			
			if( _filtro.ContieneDatos )
			{
				xmlWriter.WriteStartElement("condicion");
				xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
				xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());
				
				EscribirValores(xmlWriter);			
				
				xmlWriter.WriteEndElement(); //para <condicion>
			}		
			
		}
		
		protected void EscribirValores(XmlTextWriter xmlWriter)
		{
			ArrayList dibujos = _filtro.Dibujos;
			
			foreach(string strDib in dibujos)
			{
				xmlWriter.WriteStartElement("dibujo");
				xmlWriter.WriteString( strDib );
				xmlWriter.WriteEndElement();			
			}			
		}
		
	}
}
