// Free1X2 · WinUI 3 — WIN3
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

using System;
using System.Collections.Generic;
using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	public class ArchivoCondiciones
	{
		private XmlDocument combFile;
		private string combFileName;
	    private XmlTextWriter xmlWriter;
		
		public bool AbrirArchivoCombinacion(string fileName)
		{
			combFileName = fileName;
			combFile = new XmlDocument();
			var reader = new XmlTextReader( fileName );
			try
			{
				combFile.Load( reader );
			}
			catch
			{
				 return false;

			}
			reader.Close();
			return true;
		}
					
		public void LeeGrupos(ref Grupo miGrupo)
		{
			Grupo grupo=null;

		    XmlNodeList xmlGruposPartidos = combFile.GetElementsByTagName( "gruposPartidos" );
			
			XmlNode gruposPartidos = xmlGruposPartidos[0];
			
			for(int i = 0; i < gruposPartidos.ChildNodes.Count; i++)
			{
				if(gruposPartidos.ChildNodes[i].NodeType != XmlNodeType.Comment)
				{
					grupo = GetGrupoFromXmlNode( gruposPartidos.ChildNodes[i] );
					try
					{
						grupo.NombreGrupo= gruposPartidos.ChildNodes[i].Attributes["nombre"].Value;
					}
					catch
					{}
				}
			}
			miGrupo=grupo;
		}
		
		public Grupo LeeCondicion()
		{
		    XmlNodeList xmlGruposPartidos = combFile.GetElementsByTagName( "Condiciones" );
			XmlNode gruposPartidos = xmlGruposPartidos[0];
			Grupo grupo = GetGrupoFromXmlNode( gruposPartidos);
			return grupo;
		}

		public ControladorIfThen LeeIfThen()
		{
			var ifThen=new ControladorIfThen();
		    XmlNode condiciones=null;
			XmlNode condicion;
			XmlNode grupos=null;

		    XmlNodeList xmlRelaciones = combFile.GetElementsByTagName( "If-Then" );
			XmlNode nodoPrincipal = xmlRelaciones[0];
			if(nodoPrincipal==null) return null;
			xmlRelaciones = nodoPrincipal.ChildNodes;
			for(int i=0;i<xmlRelaciones.Count;i++)
			{
				if(xmlRelaciones[i].Name=="Condiciones")
					condiciones=xmlRelaciones[i];
				else if(xmlRelaciones[i].Name=="Grupos")
					grupos=xmlRelaciones[i];
			}
			if(condiciones!=null)
			{
			    for(int i=0;i<condiciones.ChildNodes.Count;i++)
				{
					condicion=condiciones.ChildNodes[i];
					if(i==condiciones.ChildNodes.Count-1)
					{
						// Lee los rangos
						ifThen.RangoAciertoCondiciones=condicion.Attributes["valor"].Value;
					}
					else
					{
						// Lee las condiciones
						CondicionIfThen cond = new CondicionIfThen();
						cond.CondIf =condicion.Attributes["If"].Value;
						cond.CondThen =condicion.Attributes["Then"].Value;
						ifThen.AddCondiciones(cond);
					}
				}
			}
			if(grupos!=null)
			{
			    for(int i=0;i<grupos.ChildNodes.Count;i++)
			    {
			        XmlNode grupo = grupos.ChildNodes[i];
			        if(i==grupos.ChildNodes.Count-1)
					{
						// Lee los rangos
						ifThen.RangoAciertoGrupos=grupo.Attributes["valor"].Value;
					}
					else
					{
						// Lee las condiciones
						GrupoIfThen gr = new GrupoIfThen();
						gr.NumGrupoIf=Convert.ToInt16(grupo.Attributes["if"].Value);
						gr.NumGrupoThen=Convert.ToInt16(grupo.Attributes["then"].Value);
						gr.NoIf=Convert.ToBoolean(grupo.Attributes["no-if"].Value);
						gr.NoThen=Convert.ToBoolean(grupo.Attributes["no-then"].Value);
						ifThen.AddGrupos(gr);
					}
			    }
			}
			return ifThen;
		}

	    protected Grupo GetGrupoFromXmlNode(XmlNode xmlGrupo)
		{
			Grupo grupo = new Grupo();

		    for(int i = 0; i < xmlGrupo.ChildNodes.Count; i++)
			{
				if(xmlGrupo.ChildNodes[i].NodeType != XmlNodeType.Comment)
				{
				    XmlNode xmlNode = xmlGrupo.ChildNodes[i];

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
				}
			}
			
			return grupo;
		}
		
		protected IFiltro GetFiltroFromXmlNode(XmlNode xmlGrupo)
		{
			IFiltro filtro=null;
			XmlNode xmlNode;
			for(int i = 0; i < xmlGrupo.ChildNodes.Count; i++)
			{
				if(xmlGrupo.ChildNodes[i].NodeType != XmlNodeType.Comment)
				{
					xmlNode = xmlGrupo.ChildNodes[i];
					if( xmlNode.Name == "condicion" )
						filtro=ObtenFiltro(xmlNode );
				}
			}
			return filtro;
		}
		
		protected void PonerToleranciasGrupo(Grupo grupo, XmlNode xmlTols)
		{
			ToleranciaFiltros tol;
            List<ToleranciaFiltros> tolsArray = new List<ToleranciaFiltros>();

			string noFallosTol = "";


		    for(int i = 0; i < xmlTols.ChildNodes.Count; i++)
		    {
		        XmlNode xmlNode = xmlTols.ChildNodes[i];

		        if( xmlNode.Name == "control" )
				{
					tol = new ToleranciaFiltros();
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
		
		protected void AddCondicionesGrupo(Grupo grupo, XmlNode xmlCondicion)
		{
			var grTemp=new Grupo();
		    string nombreFiltro = xmlCondicion.Attributes["id"].Value;
			IFiltro filtro = grTemp.GetFiltro( nombreFiltro );
			DatosFiltroHelper.PonerCondicionesFiltro( filtro, xmlCondicion );
			grupo.Filtros.Add(filtro);
		}
		
		protected IFiltro ObtenFiltro(XmlNode xmlCondicion)
		{
			Grupo grTemp=new Grupo();
		    string nombreFiltro = xmlCondicion.Attributes["id"].Value;
			IFiltro filtro = grTemp.GetFiltro( nombreFiltro );
			DatosFiltroHelper.PonerCondicionesFiltro( filtro, xmlCondicion );
			return filtro;
		}
		
		public void GuardaArchivo(Grupo grupo)
		{
			xmlWriter = new XmlTextWriter(combFileName, null);
			xmlWriter.Formatting = Formatting.Indented;
			InicializaArchivo();
			EscribeVersionArchivo();
			EscribeGrupoPartidos(grupo);
			FinalizaArchivo();
			xmlWriter.Close();
		}

		public void GuardaArchivo(IFiltro filtro)
		{
			xmlWriter = new XmlTextWriter(combFileName, null);
			xmlWriter.Formatting = Formatting.Indented;
			InicializaArchivo();
			EscribeVersionArchivo();
			EscribeTagCondicion();
			EscribeCondicion(filtro);
			xmlWriter.WriteEndElement();			
			FinalizaArchivo();
			xmlWriter.Close();
		}

		public void GuardaArchivo(ControladorIfThen ifThen)
		{
			xmlWriter = new XmlTextWriter(combFileName, null);
			xmlWriter.Formatting = Formatting.Indented;
			InicializaArchivo();
			EscribeVersionArchivo();
			EscribeIfThen(ifThen);
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

		protected void EscribeGrupoPartidos(Grupo grupo)
		{
			xmlWriter.WriteStartElement("gruposPartidos");
			xmlWriter.WriteStartElement("grupo");
			xmlWriter.WriteAttributeString("id", "0");
			xmlWriter.WriteAttributeString("nombre", grupo.NombreGrupo);
			EscribirPartidosActivos( grupo );
			DatosFiltroHelper.EscribirCondicionesFiltros(grupo, xmlWriter);		
			//escribir control de tolerancias
			EscribirToleranciasGrupo( grupo );				
			xmlWriter.WriteEndElement();			
		}
		
		protected void EscribeTagCondicion()
		{
			xmlWriter.WriteStartElement("Condiciones");
		}

		protected void EscribeCondicion(IFiltro filtro)
		{
			Grupo grupo = new Grupo();
			grupo.Filtros.Clear();
			grupo.Filtros.Add(filtro);
			DatosFiltroHelper.EscribirCondicionesFiltros(grupo, xmlWriter);
		}
		
		protected void EscribeIfThen(ControladorIfThen ifThen)
		{
			if(ifThen==null) return;
			CondicionIfThen cond;
			GrupoIfThen gr;
			xmlWriter.WriteStartElement("If-Then");
			// Condiciones
			xmlWriter.WriteStartElement("Condiciones");		// CONDICIONES
			for(int i=0;i<ifThen.ControlesCondiciones.Count;i++)
			{
				cond=(CondicionIfThen)ifThen.ControlesCondiciones[i];
				xmlWriter.WriteStartElement("Condicion");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("If",cond.CondIf);
				xmlWriter.WriteAttributeString("Then",cond.CondThen);
				xmlWriter.WriteEndElement();	// Condiciones
			}
			if(ifThen.ControlesCondiciones.Count>0)
			{
				xmlWriter.WriteStartElement("Rangos");
				xmlWriter.WriteAttributeString("valor", ifThen.RangoAciertoCondiciones);
				xmlWriter.WriteEndElement();	// Rangos
			}
			xmlWriter.WriteEndElement();		// CONDICIONES
			// Grupos
			xmlWriter.WriteStartElement("Grupos");	// GRUPOS
			for(int i=0;i<ifThen.ControlesGrupos.Count;i++)
			{
				gr=(GrupoIfThen)ifThen.ControlesGrupos[i];
				xmlWriter.WriteStartElement("Grupo");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("if", gr.NumGrupoIf.ToString());
				xmlWriter.WriteAttributeString("then", gr.NumGrupoThen.ToString());
				xmlWriter.WriteAttributeString("no-if", gr.NoIf.ToString());
				xmlWriter.WriteAttributeString("no-then", gr.NoThen.ToString());
				xmlWriter.WriteEndElement();	// Grupos
			}
			if(ifThen.ControlesGrupos.Count>0)
			{
				xmlWriter.WriteStartElement("Rangos");
				xmlWriter.WriteAttributeString("valor", ifThen.RangoAciertoGrupos);
				xmlWriter.WriteEndElement();	// Rangos
			}
			xmlWriter.WriteEndElement();	// GRUPOS
			xmlWriter.WriteEndElement();	// Control de Condiciones
		}

		public void EscribirPartidosActivos(Grupo grupo)
		{		
			xmlWriter.WriteStartElement("partidosActivos");
			xmlWriter.WriteAttributeString("noPartidos", grupo.ObtenPartidosActivos() );
			xmlWriter.WriteEndElement();
		}
		
		public void EscribirToleranciasGrupo(Grupo grupo)
		{			
			xmlWriter.WriteStartElement("tolerancias");			
			
			ToleranciaFiltros tol;
			
			for (int i = 0; i < grupo.ControladorTolerancias.Tolerancias.Count; i++)
			{
				tol = grupo.ControladorTolerancias.Tolerancias[i];
				
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

		public string NombreArchivo
		{
			get{ return combFileName; }
			set{ combFileName = value; }
		}
	}	
}
