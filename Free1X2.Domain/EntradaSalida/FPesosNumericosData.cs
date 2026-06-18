// Free1X2 · WinUI 3 — WIN3
// created on 25/01/2004 at 16:16
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

using System;
using System.Xml;
using System.Collections.Generic;
using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	public class FPesosNumericosData: FiltroDatosBase
	{
		private FiltroPesosNumericos _filtro;
		
		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			
			_filtro = (FiltroPesosNumericos)filtro;
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
					
					case "tolerancias":
						PonerValoresTolerancias( xmlSubCondicion1 );
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
			    int[] valores = GetValores( xmlSubCondicion.Attributes["values"].Value );
				string valoresTipoDesc = xmlSubCondicion.Attributes["tipo"].Value;
				
				//si valores contiene algun valor...
				if( valores != null )
				{
					if( valoresTipoDesc.Equals("global") )
					{
						_filtro.SetPNGlobal( valores );
					}
					else if( valoresTipoDesc.Equals("variantes") )
					{
						_filtro.SetPNVar( valores );
					}
					else if( valoresTipoDesc.Equals("unos") )
					{
						_filtro.SetPNUnos( valores );
					}
					else if( valoresTipoDesc.Equals("equis") )
					{
						_filtro.SetPNEquis( valores );
					}
					else if( valoresTipoDesc.Equals("doses") )
					{
						_filtro.SetPNDoses( valores );
					}						
				}
			}	
			
		}
		
		protected void PonerValoresTolerancias(XmlNode xmlValoresTolerancias)
		{
		    int[] valores = GetValores( xmlValoresTolerancias.Attributes["noTolerancias"].Value);
			_filtro.PonerTolerancia( valores );
						
			for(int i = 0; i < xmlValoresTolerancias.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlValoresTolerancias.ChildNodes[i];
				
			    valores = GetValores( xmlSubCondicion.Attributes["values"].Value );
				string valoresTipoDesc = xmlSubCondicion.Attributes["tipo"].Value;
				
				//si valores contiene algun valor...
				if( valores != null )
				{
					if( valoresTipoDesc.Equals("global") )
					{
						_filtro.SetPNGlobalTol( valores );
					}
					else if( valoresTipoDesc.Equals("variantes") )
					{
						_filtro.SetPNVarTol( valores );
					}
					else if( valoresTipoDesc.Equals("unos") )
					{
						_filtro.SetPNUnosTol( valores );
					}
					else if( valoresTipoDesc.Equals("equis") )
					{
						_filtro.SetPNEquisTol( valores );
					}
					else if( valoresTipoDesc.Equals("doses") )
					{
						_filtro.SetPNDosesTol( valores );
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

                string cadena = xmlSubCondicion.Attributes["value"].Value;
                if (cadena != null)
                {
                    valores.Add(Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(cadena));
                }
            }
            _filtro.Figuras = valores;

        }		

		
		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			
			_filtro = (FiltroPesosNumericos)filtro;
			
			if( _filtro.ContieneDatos )
			{
				xmlWriter.WriteStartElement("condicion");
				xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
				xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());
				
				EscribirValoresStandardFiltro(xmlWriter);
				EscribirValoresToleranciasFiltro(xmlWriter);
                EscribirValoresFigurasFiltro(xmlWriter);
				xmlWriter.WriteEndElement();
			}
			
		}
		
		protected void EscribirValoresStandardFiltro(XmlTextWriter xmlWriter)
		{
			
			xmlWriter.WriteStartElement("standard");
			
			//global
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "global");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNGlobal());
			xmlWriter.WriteEndElement();
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "variantes");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNVariantes());
			xmlWriter.WriteEndElement();
			
			//1
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "unos");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNUnos());
			xmlWriter.WriteEndElement();
			
			//X
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "equis");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNEquis());
			xmlWriter.WriteEndElement();
			
			//2
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "doses");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNDoses());
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteEndElement();			
		}
		
		protected void EscribirValoresToleranciasFiltro(XmlTextWriter xmlWriter)
		{
			
			xmlWriter.WriteStartElement("tolerancias");
			xmlWriter.WriteAttributeString("noTolerancias", _filtro.GetTolerancias());
			
			//Global
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "global");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNGlobalTol());
			xmlWriter.WriteEndElement();
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "variantes");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNVariantesTol());
			xmlWriter.WriteEndElement();
			
			//1
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "unos");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNUnosTol());
			xmlWriter.WriteEndElement();
			
			//X
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "equis");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNEquisTol());
			xmlWriter.WriteEndElement();
			
			//2
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "doses");
			xmlWriter.WriteAttributeString("values", _filtro.GetPNDosesTol());
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
