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
	/// <summary>
	/// Summary description for FInterrupcionesData.
	/// </summary>
	public class FInterrupcionesData: FiltroDatosBase
	{

		private FiltroInterrupciones _filtro;

		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroInterrupciones)filtro;
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
				}
		    }
		}

		protected void PonerValoresStandard(XmlNode xmlValoresStandard)
		{
		    string valores;

			//este flag se usa porque al pasar de la version 0.23
			//a la 0.24 se añadieron las int seguidas, y tenemos que
			//activarlas todas si esamos abriendo un fichero de la 0.23
            bool activadosIntSeguidas = false;
			
			for(int i = 0; i < xmlValoresStandard.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlValoresStandard.ChildNodes[i];

			    valores = xmlSubCondicion.Attributes["values"].Value;
				
				string valoresTipoDesc = xmlSubCondicion.Attributes["tipo"].Value;
				
				//si valores contiene algun valor...
				if( valores != null )
				{				

					if( valoresTipoDesc.Equals("global") )
					{
						_filtro.SetNoIntGlobales( valores );
					}
					if( valoresTipoDesc.Equals("var") )
					{
						_filtro.SetNoIntVar( valores );
					}
					else if( valoresTipoDesc.Equals("unos") )
					{
						_filtro.SetNoInt1( valores );
					}
					else if( valoresTipoDesc.Equals("equis") )
					{
						_filtro.SetNoIntX( valores );
					}
					else if( valoresTipoDesc.Equals("doses") )
					{
						_filtro.SetNoInt2( valores );
					}	
					
					//int seguidos
					else if( valoresTipoDesc.Equals("globalSeg") )
					{
						_filtro.SetNoIntGlobalSeg( valores );
						activadosIntSeguidas = true;
					}
					if( valoresTipoDesc.Equals("varSeg") )
					{
						_filtro.SetNoIntVarSeg( valores );
						activadosIntSeguidas = true;
					}
					else if( valoresTipoDesc.Equals("unosSeg") )
					{
						_filtro.SetNoInt1Seg( valores );
						activadosIntSeguidas = true;
					}
					else if( valoresTipoDesc.Equals("equisSeg") )
					{
						_filtro.SetNoIntXSeg( valores );
						activadosIntSeguidas = true;
					}
					else if( valoresTipoDesc.Equals("dosesSeg") )
					{
						_filtro.SetNoInt2Seg( valores );
						activadosIntSeguidas = true;
					}					
				}				
			}
	
			if(!activadosIntSeguidas)
			{
				valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";
				_filtro.SetNoIntGlobalSeg( valores );
				_filtro.SetNoIntVarSeg( valores );
				_filtro.SetNoInt1Seg( valores );
				_filtro.SetNoIntXSeg( valores );
				_filtro.SetNoInt2Seg( valores );
			}
		}		
		
		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			_filtro = (FiltroInterrupciones)filtro;
			
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

			//global
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "global");
			xmlWriter.WriteAttributeString("values", _filtro.GetIntGlobales());
			xmlWriter.WriteEndElement();
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "var");
			xmlWriter.WriteAttributeString("values", _filtro.GetIntVar());
			xmlWriter.WriteEndElement();
			
			//1
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "unos");
			xmlWriter.WriteAttributeString("values", _filtro.GetInt1());
			xmlWriter.WriteEndElement();
			
			//X
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "equis");
			xmlWriter.WriteAttributeString("values", _filtro.GetIntX());
			xmlWriter.WriteEndElement();
			
			//2
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "doses");
			xmlWriter.WriteAttributeString("values", _filtro.GetInt2());
			xmlWriter.WriteEndElement();

			//seguidas
			//global
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "globalSeg");
			xmlWriter.WriteAttributeString("values", _filtro.GetIntGlobalSeg());
			xmlWriter.WriteEndElement();
			
			//var
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "varSeg");
			xmlWriter.WriteAttributeString("values", _filtro.GetIntVarSeg());
			xmlWriter.WriteEndElement();
			
			//1
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "unosSeg");
			xmlWriter.WriteAttributeString("values", _filtro.GetInt1Seg());
			xmlWriter.WriteEndElement();
			
			//X
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "equisSeg");
			xmlWriter.WriteAttributeString("values", _filtro.GetIntXSeg());
			xmlWriter.WriteEndElement();
			
			//2
			xmlWriter.WriteStartElement("valores");
			xmlWriter.WriteAttributeString("tipo", "dosesSeg");
			xmlWriter.WriteAttributeString("values", _filtro.GetInt2Seg());
			xmlWriter.WriteEndElement();
			
			xmlWriter.WriteEndElement();
			
		}

		
	}
}
