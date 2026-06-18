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
using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{	
	public class FValoracionData: FiltroDatosBase
	{
		private FiltroValoracionSignos _filtro;

	    public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroValoracionSignos)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;
			_filtro.TipoValoracion = xmlCondicionDatos.Attributes["tipoValoracion"].Value;
			_filtro.ValorGlobal = xmlCondicionDatos.Attributes["valorGlobal"].Value;
			_filtro.ValorUnos = xmlCondicionDatos.Attributes["valorUnos"].Value;
			_filtro.ValorEquis = xmlCondicionDatos.Attributes["valorEquis"].Value;
			_filtro.ValorDoses = xmlCondicionDatos.Attributes["valorDoses"].Value;

            double[] valores1 = new double[xmlCondicionDatos.ChildNodes.Count];
            double[] valoresX = new double[xmlCondicionDatos.ChildNodes.Count];
			double[] valores2 = new double[xmlCondicionDatos.ChildNodes.Count];

	        int counter = 0;
			
			foreach(XmlNode xmlValoracion in xmlCondicionDatos.ChildNodes)
			{			
				string[] valoresPartido = ((xmlValoracion.InnerText).Trim()).Split(',');
				
				valores1[counter] = Convert.ToDouble(valoresPartido[0].Replace (".",","));
				valoresX[counter] = Convert.ToDouble(valoresPartido[1].Replace (".",","));
				valores2[counter] = Convert.ToDouble(valoresPartido[2].Replace (".",","));

				counter++;
			}

			_filtro.Valores1 = valores1;
			_filtro.ValoresX = valoresX;
			_filtro.Valores2 = valores2;
		}

		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			_filtro = (FiltroValoracionSignos)filtro;
			
			if( _filtro.ContieneDatos )
			{
				xmlWriter.WriteStartElement("condicion");
				xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
				xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());
				xmlWriter.WriteAttributeString("tipoValoracion", _filtro.TipoValoracion);
				xmlWriter.WriteAttributeString("valorGlobal", _filtro.ValorGlobal);
				xmlWriter.WriteAttributeString("valorUnos", _filtro.ValorUnos);
				xmlWriter.WriteAttributeString("valorEquis", _filtro.ValorEquis);
				xmlWriter.WriteAttributeString("valorDoses", _filtro.ValorDoses);
								
				EscribirValoracionesFiltro(xmlWriter);
								
				xmlWriter.WriteEndElement();
			}
		}

		protected void EscribirValoracionesFiltro(XmlTextWriter xmlWriter)
		{
			
			double[] valores1 = _filtro.Valores1;
			double[] valoresX = _filtro.ValoresX;
			double[] valores2 = _filtro.Valores2;

		    for(int i = 0; i < valores1.Length; i++)
			{
				string strValores = valores1[i].ToString().Replace (",",".");
				strValores += "," + valoresX[i].ToString().Replace (",",".");
				strValores += "," + valores2[i].ToString().Replace (",",".");

				xmlWriter.WriteStartElement("valores");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteString( strValores );
				xmlWriter.WriteEndElement();
			}			
		}

	}
}
