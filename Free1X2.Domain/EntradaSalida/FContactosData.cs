// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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

using System;
using System.Collections.Generic;
using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	/// <summary>
	/// Summary description for FContactosData.
	/// </summary>
	public class FContactosData: FiltroDatosBase
	{

		private FiltroContactos _filtro;

		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroContactos)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;

		    for( int i = 0; i < xmlCondicionDatos.ChildNodes.Count; i++)
		    {
		        XmlNode xmlSubCondicion1 = xmlCondicionDatos.ChildNodes[i];

		        switch( xmlSubCondicion1.Name )
				{
					case "standard":
						PonerValoresStandard( xmlSubCondicion1 );
						break;
                    case "figuras":
                        PonerValoresFiguras(xmlSubCondicion1);
                        break;
				}
		    }
		}

		protected void PonerValoresStandard(XmlNode xmlValoresStandard)
		{
		    for(int i = 0; i < xmlValoresStandard.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlValoresStandard.ChildNodes[i];
				
				//reset variable

			    string valores = xmlSubCondicion.Attributes["values"].Value;
				
				string valoresTipoDesc = xmlSubCondicion.Attributes["tipo"].Value;
				
				//si valores contiene algun valor...
				if( valores != null )
				{				

					if( valoresTipoDesc.Equals("1X") )
					{
						_filtro.SetNum1X( valores );
					}
					if( valoresTipoDesc.Equals("12") )
					{
						_filtro.SetNum12( valores );
					}
					else if( valoresTipoDesc.Equals("X2") )
					{
						_filtro.SetNumX2( valores );
					}
					else if( valoresTipoDesc.Equals("11") )
					{
						_filtro.SetNum11( valores );
					}
					else if( valoresTipoDesc.Equals("XX") )
					{
						_filtro.SetNumXX( valores );
					}	
					
					//int seguidos
					else if( valoresTipoDesc.Equals("22") )
					{
						_filtro.SetNum22( valores );
					}
					if( valoresTipoDesc.Equals("1V") )
					{
						_filtro.SetNum1V( valores );
					}
					else if( valoresTipoDesc.Equals("XV") )
					{
						_filtro.SetNumXV( valores );
					}
					else if( valoresTipoDesc.Equals("2V") )
					{
						_filtro.SetNum2V( valores );
					}
					else if( valoresTipoDesc.Equals("VV") )
					{
						_filtro.SetNumVV( valores );
					}					
				}				
			}
		}
        protected void PonerValoresFiguras(XmlNode xmlValoresFiguras)
        {
            List<long> valores = new List<long>();

            for (int i = 0; i < xmlValoresFiguras.ChildNodes.Count; i++)
            {
                XmlNode xmlSubCondicion = xmlValoresFiguras.ChildNodes[i];

                //reset variable

                string cadena = xmlSubCondicion.Attributes["value"].Value;
                if (cadena != null)
                {
                    long figura = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(cadena);
                    valores.Add(figura);
                }
            }
            _filtro.Figuras = valores;

        }		

		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			_filtro = (FiltroContactos)filtro;
			
			if( _filtro.ContieneDatos )
			{
				xmlWriter.WriteStartElement("condicion");
				xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
				xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());
				
				EscribirValoresStandardFiltro(xmlWriter);
                EscribirValoresFigurasFiltro(xmlWriter);				
				xmlWriter.WriteEndElement();
			}
		}
		
		protected void EscribirValoresStandardFiltro(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("standard");

			//global
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "1X");
			xmlWriter.WriteAttributeString("values", _filtro.GetNum1X());
			xmlWriter.WriteEndElement();
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "12");
			xmlWriter.WriteAttributeString("values", _filtro.GetNum12());
			xmlWriter.WriteEndElement();
			
			//1
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "X2");
			xmlWriter.WriteAttributeString("values", _filtro.GetNumX2());
			xmlWriter.WriteEndElement();
			
			//X
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "11");
			xmlWriter.WriteAttributeString("values", _filtro.GetNum11());
			xmlWriter.WriteEndElement();
			
			//2
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "XX");
			xmlWriter.WriteAttributeString("values", _filtro.GetNumXX());
			xmlWriter.WriteEndElement();

			//seguidas
			//global
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "22");
			xmlWriter.WriteAttributeString("values", _filtro.GetNum22());
			xmlWriter.WriteEndElement();
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "1V");
			xmlWriter.WriteAttributeString("values", _filtro.GetNum1V());
			xmlWriter.WriteEndElement();
			
			//1
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "XV");
			xmlWriter.WriteAttributeString("values", _filtro.GetNumXV());
			xmlWriter.WriteEndElement();
			
			//X
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "2V");
			xmlWriter.WriteAttributeString("values", _filtro.GetNum2V());
			xmlWriter.WriteEndElement();
			
			//2
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "VV");
			xmlWriter.WriteAttributeString("values", _filtro.GetNumVV());
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteEndElement();
			
		}
        protected void EscribirValoresFigurasFiltro(XmlTextWriter xmlWriter)
        {
            if ((_filtro.Figuras != null) && (_filtro.Figuras.Count > 0))
            {
                xmlWriter.WriteStartElement("figuras");
                for (int i = 0; i < _filtro.Figuras.Count; i++)
                {
                    xmlWriter.WriteStartElement("figura");
                    xmlWriter.WriteAttributeString("value", Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(_filtro.Figuras[i]));
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

        }

	}
}
