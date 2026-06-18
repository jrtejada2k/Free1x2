// Free1X2 · WinUI 3 — WIN3
// created on 23/08/2003 at 14:22
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
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
	public class FSignosSeguidosData: FiltroDatosBase
	{
		private FiltroSignosSeguidos _filtro;
		
		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroSignosSeguidos)filtro;
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
                    case "figurasVariantes":
                        PonerValoresFigurasVariantes(xmlSubCondicion1);
                        break;
                    case "figurasUnos":
                        PonerValoresFigurasUnos(xmlSubCondicion1);
                        break;
                    case "figurasEquis":
                        PonerValoresFigurasEquis(xmlSubCondicion1);
                        break;
                    case "figurasDoses":
                        PonerValoresFigurasDoses(xmlSubCondicion1);
                        break;
				}
		    }
		}
		
		protected void PonerValoresStandard(XmlNode xmlValoresStandard)
		{
		    for(int i = 0; i < xmlValoresStandard.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlValoresStandard.ChildNodes[i];
				
			    string valores = xmlSubCondicion.Attributes["values"].Value;
				
				string valoresTipoDesc = xmlSubCondicion.Attributes["tipo"].Value;
				
				//si valores contiene algun valor...
				if( valores != null )
				{
					if( valoresTipoDesc.Equals("var") )
					{
						_filtro.SetNoVariantes( valores );
					}
					else if( valoresTipoDesc.Equals("unos") )
					{
						_filtro.SetNoUnos( valores );
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
		}

        protected void PonerValoresFigurasVariantes(XmlNode xmlValoresFiguras)
        {
            List<long> valores = new List<long>();

            for (int i = 0; i < xmlValoresFiguras.ChildNodes.Count; i++)
            {
                XmlNode xmlSubCondicion = xmlValoresFiguras.ChildNodes[i];

                string cadena = xmlSubCondicion.Attributes["value"].Value;
                if (cadena != null)
                {
                    valores.Add(ObtenerFiguraFromText(cadena));
                }
            }
            _filtro.FigurasV = valores;

        }
        protected void PonerValoresFigurasUnos(XmlNode xmlValoresFiguras)
        {
            List<long> valores = new List<long>();

            for (int i = 0; i < xmlValoresFiguras.ChildNodes.Count; i++)
            {
                XmlNode xmlSubCondicion = xmlValoresFiguras.ChildNodes[i];

                string cadena = xmlSubCondicion.Attributes["value"].Value;
                if (cadena != null)
                {
                    valores.Add(ObtenerFiguraFromText(cadena));
                }
            }
            _filtro.Figuras1 = valores;

        }
        protected void PonerValoresFigurasEquis(XmlNode xmlValoresFiguras)
        {
            List<long> valores = new List<long>();

            for (int i = 0; i < xmlValoresFiguras.ChildNodes.Count; i++)
            {
                XmlNode xmlSubCondicion = xmlValoresFiguras.ChildNodes[i];

                string cadena = xmlSubCondicion.Attributes["value"].Value;
                if (cadena != null)
                {
                    valores.Add(ObtenerFiguraFromText(cadena));
                }

            }
            _filtro.FigurasX = valores;

        }
        protected void PonerValoresFigurasDoses(XmlNode xmlValoresFiguras)
        {
            List<long> valores = new List<long>();

            for (int i = 0; i < xmlValoresFiguras.ChildNodes.Count; i++)
            {
                XmlNode xmlSubCondicion = xmlValoresFiguras.ChildNodes[i];

                string cadena = xmlSubCondicion.Attributes["value"].Value;
                if (cadena != null)
                {
                    valores.Add(ObtenerFiguraFromText(cadena));
                }
            }
            _filtro.Figuras2 = valores;

        }		

		
		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			_filtro = (FiltroSignosSeguidos)filtro;
			
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
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "var");
			xmlWriter.WriteAttributeString("values", _filtro.GetVariantes());
			xmlWriter.WriteEndElement();
			
			//1
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "unos");
			xmlWriter.WriteAttributeString("values", _filtro.GetUnos());
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
        protected void EscribirValoresFigurasFiltro(XmlTextWriter xmlWriter)
        {
            EscribirValoresFigurasVariantesFiltro(xmlWriter);
            EscribirValoresFigurasUnosFiltro(xmlWriter);
            EscribirValoresFigurasEquisFiltro(xmlWriter);
            EscribirValoresFigurasDosesFiltro(xmlWriter);
        }
        protected void EscribirValoresFigurasVariantesFiltro(XmlTextWriter xmlWriter)
        {
            if ((_filtro.FigurasV != null) && (_filtro.FigurasV.Count > 0))
            {
                xmlWriter.WriteStartElement("figurasVariantes");
                for (int i = 0; i < _filtro.FigurasV.Count; i++)
                {
                    xmlWriter.WriteStartElement("figura");
                    xmlWriter.WriteAttributeString("value", ObtenerTextoFromLong(_filtro.FigurasV[i]));
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

        }
        protected void EscribirValoresFigurasUnosFiltro(XmlTextWriter xmlWriter)
        {
            if ((_filtro.Figuras1 != null) && (_filtro.Figuras1.Count > 0))
            {
                xmlWriter.WriteStartElement("figurasUnos");
                for (int i = 0; i < _filtro.Figuras1.Count; i++)
                {
                    xmlWriter.WriteStartElement("figura");
                    xmlWriter.WriteAttributeString("value", ObtenerTextoFromLong(_filtro.Figuras1[i]));
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

        }
        protected void EscribirValoresFigurasEquisFiltro(XmlTextWriter xmlWriter)
        {
            if ((_filtro.FigurasX != null) && (_filtro.FigurasX.Count > 0))
            {
                xmlWriter.WriteStartElement("figurasEquis");
                for (int i = 0; i < _filtro.FigurasX.Count; i++)
                {
                    xmlWriter.WriteStartElement("figura");
                    xmlWriter.WriteAttributeString("value", ObtenerTextoFromLong(_filtro.FigurasX[i]));
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

        }
        protected void EscribirValoresFigurasDosesFiltro(XmlTextWriter xmlWriter)
        {
            if ((_filtro.Figuras2 != null) && (_filtro.Figuras2.Count > 0))
            {
                xmlWriter.WriteStartElement("figurasDoses");
                for (int i = 0; i < _filtro.Figuras2.Count; i++)
                {
                    xmlWriter.WriteStartElement("figura");
                    xmlWriter.WriteAttributeString("value", ObtenerTextoFromLong(_filtro.Figuras2[i]));
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

        }

        
        public long ObtenerFiguraFromText(string texto)
        {
            long figuraTemp = 0;
            string[] valores = texto.Split('-');
            for (int i = 0; i < valores.Length; i++)
            {
                figuraTemp <<= 4;
                figuraTemp |= (uint)Convert.ToInt32(valores[i]);
            }

            return figuraTemp;
        }
        public string ObtenerTextoFromLong(long fig)
        {
            return Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(fig);
        }
		
	}
}
