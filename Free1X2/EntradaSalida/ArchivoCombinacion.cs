// created on 18/08/2003 at 21:15
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.net.uk
// Copyright (C) 2004 xfsf
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

using System.Collections.Generic;
using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	public class ArchivoCombinacion
	{
		private XmlDocument combFile;
		private string combFileName;
		private string[] pronosticos;
		private string[] equipos;
		private GrupoPartidos grupoPartidos;
		private ControladorGrupos ctrlGrupos;
		private string archivoColumnasFiltro;
		private ControladorIfThen ifThen;
		
		private XmlTextWriter xmlWriter;
		
		public void AbrirArchivoCombinacion(string fileName)
		{
			combFileName = fileName;
			combFile = new XmlDocument();
			XmlTextReader reader = new XmlTextReader( fileName );
			combFile.Load( reader );	
			reader.Close();
		}

		public string[] LeeEquipos()
		{
		    XmlNodeList XMLEquipos = combFile.GetElementsByTagName( "equipos" );
			XmlNode equiposElement = XMLEquipos[0];
			string[] strEquipo = new string[ equiposElement.ChildNodes.Count ];
			XmlNode equipo;
			for(int i = 0; i < equiposElement.ChildNodes.Count; i++ )
			{
				equipo = equiposElement.ChildNodes[ i ];
				strEquipo[ i ] = equipo.Attributes["equipos"].Value;
			}			
			return strEquipo;					
		}

		public string LeeFiltroColumnas()
		{
		    XmlNodeList XMLFiltroColumnas = combFile.GetElementsByTagName( "filtroColumnas" );

			if(XMLFiltroColumnas.Count > 0)
			{
				return XMLFiltroColumnas[0].Attributes["nombreArchivo"].Value;			
			}

			return "";
		}

		public string[] LeePronosticos()
		{
		    XmlNodeList XMLPronosticos = combFile.GetElementsByTagName( "columnaBase" );
			XmlNode pronosticosElement = XMLPronosticos[0];
			string[] strPronostico = new string[ pronosticosElement.ChildNodes.Count ];
		    for(int i = 0; i < pronosticosElement.ChildNodes.Count; i++ )
		    {
		        XmlNode pronostico = pronosticosElement.ChildNodes[ i ];
		        strPronostico[ i ] = pronostico.Attributes["pronostico"].Value;
		    }
		    return strPronostico;					
		}
		
		public ControladorIfThen CargaIfThen()
		{
			ArchivoCondiciones conds=new ArchivoCondiciones();
			conds.AbrirArchivoCombinacion(NombreArchivo);
			ControladorIfThen controladorIfThen=conds.LeeIfThen();
			return controladorIfThen;
		}

		public void CargaControladorGrupos( ControladorGrupos controlGrupos )
		{
			LeeGrupos( controlGrupos );
			LeeControlesGrupos( controlGrupos );
			LeeControlesConjuntos( controlGrupos );
		}

		public void CargaControladorGruposVacio( ControladorGrupos controlGrupos )
		{
			LeeGruposVacio( controlGrupos );
		}

		public void LeeControlesConjuntos( ControladorGrupos controlGrupos )
		{
			ControlConjuntos cConj;

		    XmlNodeList xmlControlConjuntos = combFile.GetElementsByTagName( "controlConjuntos" );
			
			if(xmlControlConjuntos.Count > 0)
			{
				XmlNode controlesConj = xmlControlConjuntos[0];

				for(int i = 0; i < controlesConj.ChildNodes.Count; i++)
				{
					if(controlesConj.ChildNodes[i].NodeType != XmlNodeType.Comment)
					{										
						XmlNode xmlControlConjNode = controlesConj.ChildNodes[i];
					
						if( i == 0) //control conj base
						{
							cConj = controlGrupos.ControlesConjuntos[ 0 ];
							cConj.PonerCtrlGruposControlados( xmlControlConjNode.Attributes["conjuntos"].Value );
							cConj.PonerFallosPermitidos( xmlControlConjNode.Attributes["fallos"].Value ); 						
						}
						else
						{					
							cConj = new ControlConjuntos();
					
							cConj.PonerCtrlGruposControlados( xmlControlConjNode.Attributes["conjuntos"].Value );
							cConj.PonerFallosPermitidos( xmlControlConjNode.Attributes["fallos"].Value ); 
						
							controlGrupos.ControlesConjuntos.Add( cConj );
							controlGrupos.UsaControlConjuntos = true;
						}
					}
				}
			}
			
		}
		
		public void LeeGrupos(ControladorGrupos controlGrupos)
		{
			GrupoPartidos gPartidos = new GrupoPartidos();
			gPartidos.CtrlGrupos = controlGrupos;
			controlGrupos.GruposPartidos = gPartidos;

		    XmlNodeList xmlGruposPartidos = combFile.GetElementsByTagName( "gruposPartidos" );
			
			XmlNode gruposPartidos = xmlGruposPartidos[0];
			
			for(int i = 0; i < gruposPartidos.ChildNodes.Count; i++)
			{
				if(gruposPartidos.ChildNodes[i].NodeType != XmlNodeType.Comment)
				{
					Grupo grupo = GetGrupoFromXmlNode( gruposPartidos.ChildNodes[i] );
					try
					{
						grupo.NombreGrupo= gruposPartidos.ChildNodes[i].Attributes["nombre"].Value;
					}
					catch
                    {
                    }
					gPartidos.AddGrupo( grupo );
				}
			}				
		}
		
		public void LeeGruposVacio(ControladorGrupos controlGrupos)
		{
			GrupoPartidos gPartidos = new GrupoPartidos();
			gPartidos.CtrlGrupos = controlGrupos;
			controlGrupos.GruposPartidos = gPartidos;
			Grupo grupo=new Grupo();
			gPartidos.AddGrupo( grupo );
		}
		
		public void LeeControlesGrupos(ControladorGrupos controlGrupos)
		{
			ControlGrupos cg;

		    XmlNodeList xmlControlGrupos = combFile.GetElementsByTagName( "controlGrupos" );
			
			XmlNode controlesGrupos = xmlControlGrupos[0];
			
			for(int i = 0; i < controlesGrupos.ChildNodes.Count; i++)
			{
				if(controlesGrupos.ChildNodes[i].NodeType != XmlNodeType.Comment)
				{					
					XmlNode xmlControlGruposNode = controlesGrupos.ChildNodes[i];
					
					if( i == 0) //control grupo base
					{
						cg = controlGrupos.ControlesGrupos[ 0 ];
						cg.PonerGruposControlados( xmlControlGruposNode.Attributes["grupos"].Value );
						cg.PonerFallosPermitidos( xmlControlGruposNode.Attributes["fallos"].Value ); 
						cg.UsaControlGrupos = false;
					}
					else
					{					
						cg = new ControlGrupos();
					
						cg.PonerGruposControlados( xmlControlGruposNode.Attributes["grupos"].Value );
						cg.PonerFallosPermitidos( xmlControlGruposNode.Attributes["fallos"].Value ); 
						
						controlGrupos.AddControlGrupo( cg );
					}
				}
			}		
		}
					
		protected Grupo GetGrupoFromXmlNode(XmlNode xmlGrupo)
		{
			Grupo grupo = new Grupo();			
						
			XmlNode xmlNode;
			
			for(int i = 0; i < xmlGrupo.ChildNodes.Count; i++)
			{
                if (i == 0)
                {
                    grupo.EsGrupoBase = true;
                }
                else
                {
                    grupo.EsGrupoBase = false;
                }
				if(xmlGrupo.ChildNodes[i].NodeType != XmlNodeType.Comment)
				{
					xmlNode = xmlGrupo.ChildNodes[i];
					
					if( xmlNode.Name == "condicion" )
					{
						PonerCondicionesGrupo( grupo, xmlNode );						
					}
					else if( xmlNode.Name == "partidosActivos" )
					{
						if(xmlNode.Attributes["noPartidos"].Value != "")
						{
							grupo.PonerPartidosActivos(  xmlNode.Attributes["noPartidos"].Value );
						}
					}		
					else if ( xmlNode.Name == "tolerancias" )
					{
						PonerToleranciasGrupo( grupo, xmlNode );					
					}
                    else if (xmlNode.Name == "filtroParcial")
                    {
                        grupo.ArchivoFiltroGrupo = xmlNode.Attributes["archivo"].Value;
                        grupo.ReinicializaVariablesFiltroParcial();
                    }
				}
			}
			
			return grupo;
		}
		
		protected void PonerToleranciasGrupo(Grupo grupo, XmlNode xmlTols)
		{
		    List<ToleranciaFiltros> tolsArray = new List<ToleranciaFiltros>();

			string noFallosTol = "";

		    for(int i = 0; i < xmlTols.ChildNodes.Count; i++)
		    {
		        XmlNode xmlNode = xmlTols.ChildNodes[i];

		        if( xmlNode.Name == "control" )
				{
					ToleranciaFiltros tol = new ToleranciaFiltros();
					tol.Aciertos = xmlNode.Attributes["noAciertos"].Value;
					tol.LetrasTol = xmlNode.Attributes["letras"].Value;

					tolsArray.Add( tol );
				}
				else if( xmlNode.Name == "controlFallos")
				{
					noFallosTol = xmlNode.Attributes["noFallos"].Value;
				}
		    }

		    grupo.ControladorTolerancias.Tolerancias = tolsArray;
			grupo.ControladorTolerancias.FallosPermitidos = noFallosTol;
						
		}
		
		protected void PonerCondicionesGrupo(Grupo grupo, XmlNode xmlCondicion)
		{
		    string nombreFiltro = xmlCondicion.Attributes["id"].Value;
			IFiltro filtro = grupo.GetFiltro( nombreFiltro );
			DatosFiltroHelper.PonerCondicionesFiltro( filtro, xmlCondicion );
		}
		
		public void GuardaArchivo()
		{
					
			xmlWriter = new XmlTextWriter(combFileName, null);
			xmlWriter.Formatting = Formatting.Indented;
			
			InicializaArchivo();
			
			EscribeVersionArchivo();
			EscribeEquipos();
			EscribeColumnaBase();
			EscribeArchFiltroColumnas();
			EscribeGrupoPartidos();
			EscribeControlesGrupos();
			EscribeControlesConjuntos();
			EscribeIfThen();
			FinalizaArchivo();
			
			xmlWriter.Close();
		}	
		
		protected void InicializaArchivo()
		{
			xmlWriter.WriteStartElement("xml");		
		}
		
		protected void FinalizaArchivo()
		{			
			xmlWriter.WriteEndElement();
		}
		
		protected void EscribeVersionArchivo()
		{
			xmlWriter.WriteStartElement("file");
			xmlWriter.WriteAttributeString("version", "0.1");
			xmlWriter.WriteEndElement();	
		}

		protected void EscribeEquipos()
		{
			xmlWriter.WriteStartElement("equipos");
			
			for(int i = 0; i < equipos.Length; i++ )
			{
				xmlWriter.WriteStartElement("partido");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("equipos", equipos[i]);
				xmlWriter.WriteEndElement();
			}			
			xmlWriter.WriteEndElement();		
		}

		protected void EscribeColumnaBase()
		{
			xmlWriter.WriteStartElement("columnaBase");
			
			for(int i = 0; i < pronosticos.Length; i++ )
			{
				xmlWriter.WriteStartElement("partido");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("pronostico", pronosticos[i]);
				xmlWriter.WriteEndElement();
			}			
			xmlWriter.WriteEndElement();		
		}

		protected void EscribeArchFiltroColumnas()
		{
			xmlWriter.WriteStartElement("filtroColumnas");
			xmlWriter.WriteAttributeString("nombreArchivo", archivoColumnasFiltro);
			xmlWriter.WriteEndElement();		
		}
		
		protected void EscribeGrupoPartidos()
		{
			xmlWriter.WriteStartElement("gruposPartidos");
			xmlWriter.WriteComment(" grupo 0 es el boleto base ");

		    for(int i = 0; i < grupoPartidos.Count; i++)
			{
				xmlWriter.WriteStartElement("grupo");
				xmlWriter.WriteAttributeString("id", i.ToString());
				
				Grupo grupo = grupoPartidos[i];
				xmlWriter.WriteAttributeString("nombre", grupo.NombreGrupo);

				EscribirPartidosActivos( grupo );
                if ((!grupo.EsGrupoBase) && (!(grupo.NombreGrupo == "0")))
                {
                    EscribirFiltroParcial(grupo);
                }
				DatosFiltroHelper.EscribirCondicionesFiltros(grupo, xmlWriter);		
				
				//escribir control de tolerancias
				EscribirToleranciasGrupo( grupo );				
				
				xmlWriter.WriteEndElement();
			}		
			
			xmlWriter.WriteEndElement();			
		}
		
		public void EscribirPartidosActivos(Grupo grupo)
		{		
			xmlWriter.WriteStartElement("partidosActivos");
			xmlWriter.WriteAttributeString("noPartidos", grupo.ObtenPartidosActivos() );
			xmlWriter.WriteEndElement();			
		}
        public void EscribirFiltroParcial(Grupo grupo)
        {
            xmlWriter.WriteStartElement("filtroParcial");
            xmlWriter.WriteAttributeString("archivo", grupo.ArchivoFiltroGrupo);
            xmlWriter.WriteEndElement();
        }
		public void EscribirToleranciasGrupo(Grupo grupo)
		{			
			xmlWriter.WriteStartElement("tolerancias");

		    for (int i = 0; i < grupo.ControladorTolerancias.Tolerancias.Count; i++)
			{
				ToleranciaFiltros tol = grupo.ControladorTolerancias.Tolerancias[i];
				
				xmlWriter.WriteStartElement("control");
				xmlWriter.WriteAttributeString("id", i.ToString() );
				xmlWriter.WriteAttributeString("letras", tol.LetrasTol );
				xmlWriter.WriteAttributeString("noAciertos", tol.Aciertos );
				
				xmlWriter.WriteEndElement();
			}			

			//tolerancias de fallos
			xmlWriter.WriteStartElement("controlFallos");
			xmlWriter.WriteAttributeString("noFallos", grupo.ControladorTolerancias.FallosPermitidos );
			xmlWriter.WriteEndElement(); 
			
			xmlWriter.WriteEndElement(); //tolerancias
		}
		

		protected void EscribeControlesConjuntos()
		{
			if(ctrlGrupos.ControlesConjuntos.Count > 1)
			{
				xmlWriter.WriteStartElement("controlConjuntos");

			    for(int i = 0; i < ctrlGrupos.ControlesConjuntos.Count; i++)
				{
					ControlConjuntos cConj = ctrlGrupos.ControlesConjuntos[i];
				
					xmlWriter.WriteStartElement("control");
				
					xmlWriter.WriteAttributeString("id", i.ToString() );
					xmlWriter.WriteAttributeString("conjuntos", cConj.ObtenCtrlGruposControladosStr() );
					xmlWriter.WriteAttributeString("fallos", cConj.ObtenFallosPermitidosStr() );				
				
					xmlWriter.WriteEndElement();			
				}
			
				xmlWriter.WriteEndElement();	
			}
		}

		protected void EscribeControlesGrupos()
		{
			xmlWriter.WriteStartElement("controlGrupos");

		    for(int i = 0; i < ctrlGrupos.ControlesGrupos.Count; i++)
			{
				ControlGrupos cg = ctrlGrupos.ControlesGrupos[i];
				
				xmlWriter.WriteStartElement("control");
				
				xmlWriter.WriteAttributeString("id", i.ToString() );
				xmlWriter.WriteAttributeString("grupos", cg.ObtenGruposControlados() );
				xmlWriter.WriteAttributeString("fallos", cg.ObtenFallosPermitidos() );				
				
				xmlWriter.WriteEndElement();			
			}
			
			xmlWriter.WriteEndElement();			
		}
		
		protected void EscribeIfThen()
		{
			if(IfThen==null) return;
		    xmlWriter.WriteStartElement("If-Then");
			// Condiciones
			xmlWriter.WriteStartElement("Condiciones");		// CONDICIONES
			for(int i=0;i<IfThen.ControlesCondiciones.Count;i++)
			{
				CondicionIfThen cond = (CondicionIfThen)IfThen.ControlesCondiciones[i];
				xmlWriter.WriteStartElement("Condicion");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("If",cond.CondIf);
				xmlWriter.WriteAttributeString("Then",cond.CondThen);
				xmlWriter.WriteEndElement();	// Condiciones
			}
			if(IfThen.ControlesCondiciones.Count>0)
			{
				xmlWriter.WriteStartElement("Rangos");
				xmlWriter.WriteAttributeString("valor", IfThen.RangoAciertoCondiciones);
				xmlWriter.WriteEndElement();	// Rangos
			}
			xmlWriter.WriteEndElement();		// CONDICIONES
			// Grupos
			xmlWriter.WriteStartElement("Grupos");	// GRUPOS
			for(int i=0;i<IfThen.ControlesGrupos.Count;i++)
			{
				GrupoIfThen gr = (GrupoIfThen)IfThen.ControlesGrupos[i];
				xmlWriter.WriteStartElement("Grupo");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("if", gr.NumGrupoIf.ToString());
				xmlWriter.WriteAttributeString("then", gr.NumGrupoThen.ToString());
				xmlWriter.WriteAttributeString("no-if", gr.NoIf.ToString());
				xmlWriter.WriteAttributeString("no-then", gr.NoThen.ToString());
				xmlWriter.WriteEndElement();	// Grupos
			}
			if(IfThen.ControlesGrupos.Count>0)
			{
				xmlWriter.WriteStartElement("Rangos");
				xmlWriter.WriteAttributeString("valor", IfThen.RangoAciertoGrupos);
				xmlWriter.WriteEndElement();	// Rangos
			}
			xmlWriter.WriteEndElement();	// GRUPOS
			xmlWriter.WriteEndElement();	// Control de Condiciones
		}

		public string NombreArchivo
		{
			get{ return combFileName; }
			set{ combFileName = value; }
		
		}

		public string[] Equipos
		{
			set{ equipos = value; }
		}

		public string[] Pronosticos
		{
			set{ pronosticos = value; }
		}
		
		public GrupoPartidos Grupos
		{
			set{ grupoPartidos = value; }		
		}
		
		public ControladorGrupos CtrlGrupos
		{
			set{ ctrlGrupos = value; }
		}

		public string ArchivoColumnasFiltro
		{
			set{ archivoColumnasFiltro = value; }
		}

		public ControladorIfThen IfThen
		{
			get{ return ifThen; }
			set{ ifThen= value; }
		}
	}	
}
