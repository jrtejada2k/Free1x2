// Free1X2 · WinUI 3 — WIN3
// created on 17/11/2003 at 20:46
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
	public class FColProbablesData: FiltroDatosBase
	{
		private FiltroColProbables _filtro;
		
		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			
			_filtro = (FiltroColProbables)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;
			
			List<ColumnaProbable> arrayCP = new List<ColumnaProbable>();

		    List<RelacionCP1> arrayRelaciones1 = new List<RelacionCP1>();

		    List<RelacionCP2> arrayRelaciones2 = new List<RelacionCP2>();

		    List<RelacionCP3> arrayRelaciones3 = new List<RelacionCP3>();

		    List<CPControlFallos> arrayFallosCP = new List<CPControlFallos>();

		    string controlFallos = "";
			
			foreach(XmlNode xmlNode in xmlCondicionDatos.ChildNodes)
			{	
				
				switch(xmlNode.Name.ToUpper())
				{
					case "CP":
						ColumnaProbable cp = ObtenCP( xmlNode );
						arrayCP.Add( cp );
						break;

					case "RELACION1":
						RelacionCP1 rel1 = ObtenRelacionCP1( xmlNode );
						arrayRelaciones1.Add( rel1 );
						break;

					case "FALLOSCP":
						CPControlFallos cpCtrlFallos = ObtenCPCrtlFallos( xmlNode );
                        arrayFallosCP.Add( cpCtrlFallos );
						break;

					case "CONTROLFALLOS":
						controlFallos = xmlNode.Attributes["noFallos"].Value;
						break;	
				
                    case "RELACION2":
                        RelacionCP2 rel2 = ObtenRelacionCP2(xmlNode);
                        rel2.ColumnasProbables = arrayCP;
                        arrayRelaciones2.Add(rel2);
                        break;
                    case "RELACION3":
                        RelacionCP3 rel3 = ObtenRelacionCP3(xmlNode,arrayCP);
                        arrayRelaciones3.Add(rel3);
                        break;
				}
			}
			
			_filtro.ColProbables = arrayCP;			
			_filtro.RelacionesCP1.Relaciones = arrayRelaciones1;
            _filtro.RelacionesCP2.Relaciones2 = arrayRelaciones2;
            _filtro.RelacionesCP2.ColumnasProbables = _filtro.ColProbables;
            _filtro.RelacionesCP3.Relaciones = arrayRelaciones3;
			_filtro.ControlFallosCP.ControlesFallos = arrayFallosCP;
			_filtro.ControlFallosCP.FallosPermitidos = controlFallos;
			
		}
		
		protected ColumnaProbable ObtenCP(XmlNode xmlCP)
		{
		    ColumnaProbable cp = new ColumnaProbable();
			
			for(int i = 0; i < xmlCP.ChildNodes.Count; i++ )
			{	
				XmlNode xmlSubCondicion = xmlCP.ChildNodes[i];
				string valoresTipoDesc = xmlSubCondicion.Name;

				switch(valoresTipoDesc)
				{
					case "pronostico":
						cp.Pronosticos = ObtenPronosticosCP(xmlSubCondicion.Attributes["values"].Value);
						break;
					case "standard":
						PonerValoresStandard( xmlSubCondicion, cp );
						break;					
					case "tolerancias":
						PonerValoresTolerancias( xmlSubCondicion, cp );
						break;						
				}				
			}
			
			return cp;		
		}

		protected RelacionCP1 ObtenRelacionCP1( XmlNode xmlNode )
		{
		    RelacionCP1 rel = new RelacionCP1();

			for(int i = 0; i < xmlNode.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlNode.ChildNodes[i];
				string valoresTipoDesc = xmlSubCondicion.Name;

				switch(valoresTipoDesc)
				{
					case "columnas":
						rel.Columnas = xmlSubCondicion.Attributes["values"].Value;
						break;

					case "sumas":
						rel.SumaAciertos = xmlSubCondicion.Attributes["values"].Value;
						break;
					
					case "recorridos":
						rel.Recorridos = xmlSubCondicion.Attributes["values"].Value;
						break;	
					case "gruposCP":
						rel.CantidadCP = xmlSubCondicion.Attributes["cantidadCP"].Value;
						rel.CuantosAC  = xmlSubCondicion.Attributes["noAC"].Value;
						break;
				}
			}		
		
			return rel;
		}
        protected RelacionCP2 ObtenRelacionCP2(XmlNode xmlNode)
        {
            RelacionCP2 rel = new RelacionCP2();

            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                XmlNode xmlSubCondicion = xmlNode.ChildNodes[i];
                string valoresTipoDesc = xmlSubCondicion.Name;

                switch (valoresTipoDesc)
                {
                    case "columnasA":
                        rel.StrColsA = xmlSubCondicion.Attributes["values"].Value;
                        rel.ColumnasA = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(rel.StrColsA);
                        break;
                    case "columnasB":
                        rel.StrColsB = xmlSubCondicion.Attributes["values"].Value;
                        rel.ColumnasB = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(rel.StrColsB);
                        break;
                    case "aciertos":
                        rel.StrAciertos = xmlSubCondicion.Attributes["values"].Value;
                        rel.Aciertos = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(rel.StrAciertos);
                        break;
                    case "concepto":
                        rel.Concepto = xmlSubCondicion.Attributes["value"].Value;
                        break;
                    case "cantidad":
                        rel.Cantidad = xmlSubCondicion.Attributes["value"].Value;
                        break;

                    case "columnasA2":
                        rel.StrColsA2 = xmlSubCondicion.Attributes["values"].Value;
                        rel.ColumnasA2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(rel.StrColsA2);
                        break;
                    case "columnasB2":
                        rel.StrColsB2 = xmlSubCondicion.Attributes["values"].Value;
                        rel.ColumnasB2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(rel.StrColsB2);
                        break;
                    case "aciertos2":
                        rel.StrAciertos2 = xmlSubCondicion.Attributes["values"].Value;
                        rel.Aciertos2 = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(rel.StrAciertos2);
                        break;
                    case "concepto2":
                        rel.Concepto2 = xmlSubCondicion.Attributes["value"].Value;
                        break;
                    case "cantidad2":
                        rel.Cantidad2 = xmlSubCondicion.Attributes["value"].Value;
                        break;
                }
            }
            return rel;
        }
        protected RelacionCP3 ObtenRelacionCP3(XmlNode xmlNode, List<ColumnaProbable> grupoCP)
        {
            RelacionCP3 rel = new RelacionCP3();
            List<ColumnaProbable> lista = null;
            int longitudEscaleras = grupoCP.Count / 3;
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                XmlNode xmlSubCondicion = xmlNode.ChildNodes[i];
                string valoresTipoDesc = xmlSubCondicion.Name;

                
                
                switch (valoresTipoDesc)
                {
                    case "columnas":
                        string cols = xmlSubCondicion.Attributes["values"].Value;
                        lista = ObtenerColumnasImplicadasRelCP3(cols,grupoCP);
                        rel.Columnas = lista;
                        rel.ColumnasImplicadasString = cols;
                        break;
                    case "sandwichs":
                        if (lista != null)
                        {
                            int longitudSandwichs = lista.Count / 4;
                            rel.NumeroSandwichsPermitidosString = xmlSubCondicion.Attributes["values"].Value;
                            rel.NumeroSandwichsPermitidos = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(rel.NumeroSandwichsPermitidosString,longitudSandwichs);
                        }
                        break;
                    case "escalerasAsc":
                        rel.NumeroEscalerasASCPermitidasString = xmlSubCondicion.Attributes["values"].Value;
                        rel.NumeroEscalerasASCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(rel.NumeroEscalerasASCPermitidasString, longitudEscaleras); 
                        break;
                    case "escalerasDesc":
                        rel.NumeroEscalerasDESCPermitidasString = xmlSubCondicion.Attributes["values"].Value;
                        rel.NumeroEscalerasDESCPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(rel.NumeroEscalerasDESCPermitidasString, longitudEscaleras); 
                        break;
                    case "escaleras":
                        rel.NumeroEscalerasTotalesPermitidasString = xmlSubCondicion.Attributes["values"].Value;
                        rel.NumeroEscalerasTotalesPermitidas = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(rel.NumeroEscalerasTotalesPermitidasString, longitudEscaleras); 
                        break;
                    case "concepto":
                        rel.Concepto = xmlSubCondicion.Attributes["values"].Value;
                        rel.ConceptoString = xmlSubCondicion.Attributes["values"].Value;
                        break;
                    case "agrupacionesPasoFijo":
                        rel.AgrupacionesPasoFijoPermitidasString = xmlSubCondicion.Attributes["values"].Value.Split('#');
                        rel.AgrupacionesPasoFijoPermitidas = ObtenArrayAgrupaciones(rel.AgrupacionesPasoFijoPermitidasString, grupoCP.Count + 1);
                        break;
                    case "agrupacionesSolapadas":
                        rel.AgrupacionesSolapadasPermitidasString = xmlSubCondicion.Attributes["values"].Value.Split('#');
                        rel.AgrupacionesSolapadasPermitidas = ObtenArrayAgrupaciones(rel.AgrupacionesSolapadasPermitidasString, grupoCP.Count + 1);
                        break;
                }
            }
            
            return rel;
        }		

		protected CPControlFallos ObtenCPCrtlFallos( XmlNode xmlNode )
		{
			CPControlFallos cpCtrlFallos = new CPControlFallos();

		    for(int i = 0; i < xmlNode.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion = xmlNode.ChildNodes[i];
				string valoresTipoDesc = xmlSubCondicion.Name;

				switch(valoresTipoDesc)
				{
					case "columnas":
						cpCtrlFallos.Columnas = xmlSubCondicion.Attributes["values"].Value;
						break;

					case "tolerancias":
						cpCtrlFallos.Tolerancias = xmlSubCondicion.Attributes["values"].Value;
						break;
					
					case "aciertos":
						cpCtrlFallos.Aciertos = xmlSubCondicion.Attributes["values"].Value;
						break;	
				}
			}	

			return cpCtrlFallos;
		}

		protected void PonerValoresStandard( XmlNode xmlSubCondicion, ColumnaProbable cp )
		{
		    for(int i = 0; i < xmlSubCondicion.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion1 = xmlSubCondicion.ChildNodes[i];
				string tipoCondicion = xmlSubCondicion1.Name;

				if( tipoCondicion.Equals("aciertos") )
				{
					cp.SetNoAciertos( xmlSubCondicion1.Attributes["values"].Value );
				}
				else if( tipoCondicion.Equals("aciertosSeguidos") )
				{
					cp.SetNoAciertosSeguidos( xmlSubCondicion1.Attributes["values"].Value  );
				}
				else if( tipoCondicion.Equals("fallosSeguidos") )
				{
					cp.SetNoFallosSeguidos(  xmlSubCondicion1.Attributes["values"].Value  );
				}
				else if( tipoCondicion.Equals("puntos") )
				{
					cp.SetPuntos( xmlSubCondicion1.Attributes["values"].Value );
				}				
			}		
		}

		protected void PonerValoresTolerancias( XmlNode xmlSubCondicion, ColumnaProbable cp )
		{
		    cp.SetTolerancias( xmlSubCondicion.Attributes["noTolerancias"].Value );

			for(int i = 0; i < xmlSubCondicion.ChildNodes.Count; i++ )
			{
				XmlNode xmlSubCondicion1 = xmlSubCondicion.ChildNodes[i];
				string tipoCondicion = xmlSubCondicion1.Name;

				if( tipoCondicion.Equals("aciertosTol") )
				{
					cp.SetACTol( xmlSubCondicion1.Attributes["values"].Value );
				}
				else if( tipoCondicion.Equals("aciertosSeguidosTol") )
				{
					cp.SetACSTol( xmlSubCondicion1.Attributes["values"].Value  );
				}
				else if( tipoCondicion.Equals("fallosSeguidosTol") )
				{
					cp.SetFSTol(  xmlSubCondicion1.Attributes["values"].Value  );
				}
			}			
		}
		
		protected string[] ObtenPronosticosCP( string pronostico )
		{
			//pronostico es de la forma: 1X,,,X,1X2,,X2,,,
			
			string[] arrayPronosticos = pronostico.Split(',');
			
			//convertir de 1X2 a 1,X,2

		    for(int i = 0; i < arrayPronosticos.Length; i++)
			{
				string temp = arrayPronosticos[i];
				string newProno = "";
				
				foreach(char charSigno in temp)
				{
					newProno += charSigno + ",";				
				}
				
				//quitar ultima coma si string no esta vacio
				if( newProno != "" )
				{
					newProno = newProno.Remove(newProno.Length - 1, 1);
				}
				arrayPronosticos[i] = newProno;
			}	
			
			return arrayPronosticos;
		
		}
		
		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{
			
			_filtro = (FiltroColProbables)filtro;
			
			if( _filtro.ContieneDatos )
			{
				xmlWriter.WriteStartElement("condicion");
				xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
				xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());
				
				EscribirValoresCP(xmlWriter);
				EscribirValoresRelaciones1(xmlWriter);
                EscribirValoresRelaciones2(xmlWriter);
				EscribirValoresCPCtrlFallos(xmlWriter);
                EscribirValoresRelaciones3(xmlWriter);
				
				xmlWriter.WriteEndElement(); //para <condicion>
			}		
			
		}
		
		protected void EscribirValoresCP(XmlTextWriter xmlWriter)
		{
			List<ColumnaProbable> arrayCP = _filtro.ColProbables;

		    for(int i = 0; i < arrayCP.Count; i++)
			{
				ColumnaProbable cp = arrayCP[i];
				
				xmlWriter.WriteStartElement("cp");
				xmlWriter.WriteAttributeString("id", i.ToString());
				
				EscribirPronosticoCP( xmlWriter, cp );
				EscribirValoresEstandard( xmlWriter, cp );

				if(cp.ToleranciaLocalActiva)
				{
					EscribirValoresTolerancias( xmlWriter, cp );
				}
				
				xmlWriter.WriteEndElement(); //</cp>
			}
			
		}

		protected void EscribirValoresRelaciones1(XmlTextWriter xmlWriter)
		{
			List<RelacionCP1> relacionesCP1 = _filtro.RelacionesCP1.Relaciones;

		    for(int i = 0; i < relacionesCP1.Count; i++)
			{
				RelacionCP1 rel = relacionesCP1[i];
				
				xmlWriter.WriteStartElement("relacion1");
				xmlWriter.WriteAttributeString("id", i.ToString());

				//columnas
				xmlWriter.WriteStartElement("columnas");
				xmlWriter.WriteAttributeString("values", rel.Columnas);
				xmlWriter.WriteEndElement();

				//sumas
				xmlWriter.WriteStartElement("sumas");
				xmlWriter.WriteAttributeString("values", rel.SumaAciertos);
				xmlWriter.WriteEndElement();

				//recorridos
				xmlWriter.WriteStartElement("recorridos");
				xmlWriter.WriteAttributeString("values", rel.Recorridos);
				xmlWriter.WriteEndElement();
				
				//gruposCP
				xmlWriter.WriteStartElement("gruposCP");
				xmlWriter.WriteAttributeString("cantidadCP", rel.CantidadCP);
				xmlWriter.WriteAttributeString("noAC", rel.CuantosAC);
				xmlWriter.WriteEndElement();

				xmlWriter.WriteEndElement(); //</relacion1>
			}
		}
        protected void EscribirValoresRelaciones2(XmlTextWriter xmlWriter)
        {
            List<RelacionCP2> relacionesCP2 = _filtro.RelacionesCP2.Relaciones2;

            for (int i = 0; i < relacionesCP2.Count; i++)
            {
                RelacionCP2 rel = relacionesCP2[i];

                xmlWriter.WriteStartElement("relacion2");
                xmlWriter.WriteAttributeString("id", i.ToString());

                //columnasA
                xmlWriter.WriteStartElement("columnasA");
                xmlWriter.WriteAttributeString("values", rel.StrColsA);
                xmlWriter.WriteEndElement();

                //columnasB
                xmlWriter.WriteStartElement("columnasB");
                xmlWriter.WriteAttributeString("values", rel.StrColsB);
                xmlWriter.WriteEndElement();

                //aciertos
                xmlWriter.WriteStartElement("aciertos");
                xmlWriter.WriteAttributeString("values", rel.StrAciertos);
                xmlWriter.WriteEndElement();

                //concepto
                xmlWriter.WriteStartElement("concepto");
                xmlWriter.WriteAttributeString("value", rel.Concepto);
                xmlWriter.WriteEndElement();

                //cantidad
                xmlWriter.WriteStartElement("cantidad");
                xmlWriter.WriteAttributeString("value", rel.Cantidad);
                xmlWriter.WriteEndElement();

                //columnasA2
                xmlWriter.WriteStartElement("columnasA2");
                xmlWriter.WriteAttributeString("values", rel.StrColsA2);
                xmlWriter.WriteEndElement();

                //columnasB2
                xmlWriter.WriteStartElement("columnasB2");
                xmlWriter.WriteAttributeString("values", rel.StrColsB2);
                xmlWriter.WriteEndElement();

                //aciertos2
                xmlWriter.WriteStartElement("aciertos2");
                xmlWriter.WriteAttributeString("values", rel.StrAciertos2);
                xmlWriter.WriteEndElement();

                //concepto2
                xmlWriter.WriteStartElement("concepto2");
                xmlWriter.WriteAttributeString("value", rel.Concepto2);
                xmlWriter.WriteEndElement();

                //cantidad2
                xmlWriter.WriteStartElement("cantidad2");
                xmlWriter.WriteAttributeString("value", rel.Cantidad2);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }
        }
        protected void EscribirValoresRelaciones3(XmlTextWriter xmlWriter)
        {
            List<RelacionCP3> relacionesCP3 = _filtro.RelacionesCP3.Relaciones;

            for (int i = 0; i < relacionesCP3.Count; i++)
            {
                RelacionCP3 rel = relacionesCP3[i];

                xmlWriter.WriteStartElement("relacion3");
                xmlWriter.WriteAttributeString("id", i.ToString());

                //columnas
                xmlWriter.WriteStartElement("columnas");
                xmlWriter.WriteAttributeString("values", rel.ColumnasImplicadasString);
                xmlWriter.WriteEndElement();

                //sandwichs
                xmlWriter.WriteStartElement("sandwichs");
                xmlWriter.WriteAttributeString("values", rel.NumeroSandwichsPermitidosString);
                xmlWriter.WriteEndElement();

                //escalerasAsc
                xmlWriter.WriteStartElement("escalerasAsc");
                xmlWriter.WriteAttributeString("values", rel.NumeroEscalerasASCPermitidasString);
                xmlWriter.WriteEndElement();

                //escalerasDesc
                xmlWriter.WriteStartElement("escalerasDesc");
                xmlWriter.WriteAttributeString("values", rel.NumeroEscalerasDESCPermitidasString);
                xmlWriter.WriteEndElement();

                //escaleras
                xmlWriter.WriteStartElement("escaleras");
                xmlWriter.WriteAttributeString("values", rel.NumeroEscalerasTotalesPermitidasString);
                xmlWriter.WriteEndElement();

                //concepto
                xmlWriter.WriteStartElement("concepto");
                xmlWriter.WriteAttributeString("values", rel.ConceptoString);
                xmlWriter.WriteEndElement();

                //agrupacionesPasoFijo
                xmlWriter.WriteStartElement("agrupacionesPasoFijo");
                string agrupacionesPF = "";
                if (rel.AgrupacionesPasoFijoPermitidasString != null)
                {
                    for (int j = 0; j < rel.AgrupacionesPasoFijoPermitidasString.Length; j++)
                    {
                        if (rel.AgrupacionesPasoFijoPermitidasString[j] != null)
                        {
                            agrupacionesPF += rel.AgrupacionesPasoFijoPermitidasString[j] + "#";
                        }
                    }

                    xmlWriter.WriteAttributeString("values", agrupacionesPF.Substring(0, agrupacionesPF.Length - 1));

                }
                else
                {
                    xmlWriter.WriteAttributeString("values", "");
                }
                    
                xmlWriter.WriteEndElement();

                //agrupacionesSolapadas
                xmlWriter.WriteStartElement("agrupacionesSolapadas");
                if (rel.AgrupacionesSolapadasPermitidasString != null)
                {
                    string agrupacionesS = "";
                    for (int j = 0; j < rel.AgrupacionesSolapadasPermitidasString.Length; j++)
                    {
                        if (rel.AgrupacionesSolapadasPermitidasString[j] != null)
                        {
                            agrupacionesS += rel.AgrupacionesSolapadasPermitidasString[j] + "#";
                        }
                    }
                    xmlWriter.WriteAttributeString("values", agrupacionesS.Substring(0, agrupacionesS.Length - 1));
                }
                else
                {
                    xmlWriter.WriteAttributeString("values", "");
                } 
                xmlWriter.WriteEndElement();


                xmlWriter.WriteEndElement();
            }
        }

		protected void EscribirValoresCPCtrlFallos(XmlTextWriter xmlWriter)
		{
			List<CPControlFallos> arrayFallosCP = _filtro.ControlFallosCP.ControlesFallos;

		    for(int i = 0; i < arrayFallosCP.Count; i++)
			{
				CPControlFallos cpCtrlFallos = arrayFallosCP[i];

				xmlWriter.WriteStartElement("fallosCP");
				xmlWriter.WriteAttributeString("id", i.ToString());

				//columnas
				xmlWriter.WriteStartElement("columnas");
				xmlWriter.WriteAttributeString("values", cpCtrlFallos.Columnas);
				xmlWriter.WriteEndElement();

				//sumas
				xmlWriter.WriteStartElement("tolerancias");
				xmlWriter.WriteAttributeString("values", cpCtrlFallos.Tolerancias);
				xmlWriter.WriteEndElement();

				//recorridos
				xmlWriter.WriteStartElement("aciertos");
				xmlWriter.WriteAttributeString("values", cpCtrlFallos.Aciertos);
				xmlWriter.WriteEndElement();

				xmlWriter.WriteEndElement(); //</fallosCP>
			}		

			xmlWriter.WriteStartElement("controlFallos");
			xmlWriter.WriteAttributeString("noFallos", _filtro.ControlFallosCP.FallosPermitidos);
			xmlWriter.WriteEndElement(); //</controlFallos>
		}
		
		protected void EscribirPronosticoCP(XmlTextWriter xmlWriter, ColumnaProbable cp)
		{
			string[] pronosticos = cp.Pronosticos;
		    string strAllPronosticos = "";

		    foreach(string strProno in pronosticos)
		    {
		        //eliminar comas del pronostico.
		        string temp = strProno.Replace(",", "");

		        strAllPronosticos += temp + ",";
		    }

		    //quitar ultima coma.
			strAllPronosticos = strAllPronosticos.Remove(strAllPronosticos.Length - 1, 1);
			
			xmlWriter.WriteStartElement("pronostico");
			xmlWriter.WriteAttributeString("values", strAllPronosticos);
			xmlWriter.WriteEndElement(); //</Pronostico>	
		}
		
		protected void EscribirValoresEstandard(XmlTextWriter xmlWriter, ColumnaProbable cp)
		{
			xmlWriter.WriteStartElement("standard");

			//aciertos
			xmlWriter.WriteStartElement("aciertos");
			xmlWriter.WriteAttributeString("values", cp.GetAciertos() );
			xmlWriter.WriteEndElement(); //</aciertos>	
			
			//aciertos seguidos
			xmlWriter.WriteStartElement("aciertosSeguidos");
			xmlWriter.WriteAttributeString("values", cp.GetAciertosSeguidos() );
			xmlWriter.WriteEndElement(); //</aciertosSeguidos>
			
			//fallos seguidos
			xmlWriter.WriteStartElement("fallosSeguidos");
			xmlWriter.WriteAttributeString("values", cp.GetFallosSeguidos() );
			xmlWriter.WriteEndElement(); //</fallosSeguidos>	

			//puntos
			xmlWriter.WriteStartElement("puntos");
			xmlWriter.WriteAttributeString("values", cp.GetPuntos() );
			xmlWriter.WriteEndElement(); //</puntos>	
	
			

			xmlWriter.WriteEndElement(); 
		}

		protected void EscribirValoresTolerancias(XmlTextWriter xmlWriter, ColumnaProbable cp)
		{
			xmlWriter.WriteStartElement("tolerancias");
			xmlWriter.WriteAttributeString("noTolerancias", cp.GetTolerancias() );

			//tolerancias aciertos
			xmlWriter.WriteStartElement("aciertosTol");
			xmlWriter.WriteAttributeString("values", cp.GetACTol() );
			xmlWriter.WriteEndElement(); 
			
			//tolerancias aciertos seguidos
			xmlWriter.WriteStartElement("aciertosSeguidosTol");
			xmlWriter.WriteAttributeString("values", cp.GetACSTol() );
			xmlWriter.WriteEndElement(); 
			
			//tolerancias fallos seguidos
			xmlWriter.WriteStartElement("fallosSeguidosTol");
			xmlWriter.WriteAttributeString("values", cp.GetFSTol() );
			xmlWriter.WriteEndElement(); 
			
			xmlWriter.WriteEndElement(); 
		
		}

        protected List<ColumnaProbable> ObtenerColumnasImplicadasRelCP3(string columnas, List<ColumnaProbable> grupoCP)
        {
            List<ColumnaProbable> lista = new List<ColumnaProbable>();
            List<int> indices = Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(columnas);
            for (int i = 0; i < indices.Count; i++)
            {
                lista.Add(grupoCP[i]);
            }
            return lista;
        }
        protected List<AgrupacionColumnas> ObtenArrayAgrupaciones(string[] datos, int maximoAgrupaciones)
        {
            List<AgrupacionColumnas> agrupaciones = new List<AgrupacionColumnas>();
            //elementos, aciertos, numero

            for(int i = 0; i < datos.Length; i++)
            {
                string[] agrupacion = datos[i].Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                if (agrupacion.Length == 3)
                {
                    List<int> valores = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(agrupacion[0]);
                    int elementos = Convert.ToInt32(agrupacion[1]);
                    List<int> ac = Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(agrupacion[2]);


                    if (elementos < maximoAgrupaciones && ac.Count <= 15)
                    {
                        AgrupacionColumnas agrup = new AgrupacionColumnas(valores, elementos, ac);
                        agrupaciones.Add(agrup);
                    }
                }
            }
            if (agrupaciones.Count == 0)
            {
                agrupaciones = null;
            }
            return agrupaciones;
        }

	}
}

