// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 Luis Fernandez - luifer@onetel.com
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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace Free1X2.EntradaSalida
{
	/// <summary>
	/// Summary description for AConfiguracion.
	/// </summary>
	public class AConfiguracion
	{
		private XmlDocument configFile;
		private string archConfig = "";

		public AConfiguracion(string directorioInicio)
		{
			InicializaParametros(directorioInicio);			
		}
		
		public AConfiguracion()
		{
			InicializaParametros(System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
		}

		protected void InicializaParametros(string directorioInicio)
		{
			archConfig = directorioInicio + @"/parametros.free1x2";

			configFile = new XmlDocument();

			XmlTextReader reader = new XmlTextReader( directorioInicio + @"/parametros.free1x2" );
			
			configFile.Load( reader );	
			
			reader.Close();		
		}

		public void ObtenPuntosCP(ref int puntosFijos, ref int puntosDobles, ref int puntosTriples)
		{
		    XmlNodeList XMLConfigCP = configFile.GetElementsByTagName( "CP" );

			XmlNode xmlPuntosCP = XMLConfigCP[0].ChildNodes[0];

			puntosFijos = Convert.ToInt32(xmlPuntosCP.Attributes["fijos"].Value);
			puntosDobles = Convert.ToInt32(xmlPuntosCP.Attributes["dobles"].Value);
			puntosTriples = Convert.ToInt32(xmlPuntosCP.Attributes["triples"].Value);		
		}
        public void ObtenToolBarsVisibles(ref bool tsFree, ref bool tsFiltros, ref bool tsCombinacion, ref bool tsOperaciones, ref bool tsArchivo, ref bool tsUtilidades)
        {
            // Lectura ROBUSTA: si el nodo <toolbars> falta, o un atributo tsX falta / no
            // es parseable, se devuelve TRUE para ese grupo. La visibilidad por defecto de
            // las SEIS barras es "mostrar" (idéntico a VariablesGlobales.SetDefaultValues),
            // de modo que un parametros.free1x2 viejo o corrupto nunca pueda dejar oculta
            // una barra (p.ej. Filtros) de forma permanente al arrancar. El usuario sigue
            // pudiendo ocultarlas desde Ver -> Barras de Herramientas.
            tsFree = tsFiltros = tsCombinacion = tsOperaciones = tsArchivo = tsUtilidades = true;

            XmlNodeList XMLConfigToolBars = configFile.GetElementsByTagName("toolbars");
            if (XMLConfigToolBars.Count == 0 || XMLConfigToolBars[0].ChildNodes.Count == 0) return;

            XmlNode xmlToolBars = XMLConfigToolBars[0].ChildNodes[0];

            tsFree = LeerBoolAtributo(xmlToolBars, "tsFree");
            tsFiltros = LeerBoolAtributo(xmlToolBars, "tsFiltros");
            tsCombinacion = LeerBoolAtributo(xmlToolBars, "tsCombinacion");
            tsOperaciones = LeerBoolAtributo(xmlToolBars, "tsOperaciones");
            tsArchivo = LeerBoolAtributo(xmlToolBars, "tsArchivo");
            tsUtilidades = LeerBoolAtributo(xmlToolBars, "tsUtilidades");
        }

        // Devuelve el valor booleano de un atributo; si falta o no es parseable, TRUE
        // (la barra se muestra por defecto). Usado por ObtenToolBarsVisibles.
        private static bool LeerBoolAtributo(XmlNode nodo, string nombre)
        {
            var attr = nodo?.Attributes?[nombre];
            if (attr == null) return true;
            return bool.TryParse(attr.Value, out bool v) ? v : true;
        }
        public void ObtenConfiguracionActualizador(ref bool actualizarAlInicio)
        {
            XmlNodeList XMLConfigActualizador = configFile.GetElementsByTagName("actualizador");

            XmlNode xmlActualizador = XMLConfigActualizador[0].ChildNodes[0];

            actualizarAlInicio = Convert.ToBoolean(xmlActualizador.Attributes["actualizarAlInicio"].Value);

        }
        public void ObtenConfiguracionAdvertenciaSalir(ref bool mostrarConfirmacionSalir)
        {
            XmlNodeList XMLConfigActualizador = configFile.GetElementsByTagName("salir");

            XmlNode xmlActualizador = XMLConfigActualizador[0].ChildNodes[0];

            mostrarConfirmacionSalir = Convert.ToBoolean(xmlActualizador.Attributes["confirmacionSalir"].Value);

        }
        public void ObtenConfiguracionAnalisis(ref bool aTodo, ref bool aVX2, ref bool aSeguidos, ref bool aFigurasV1X2, ref bool aInterrupciones, ref bool aDibujos, ref bool aSimetrias, ref bool aFormatos, ref bool aFormatos123, ref bool aDistancias, ref bool aContactos, ref bool aFigurasContactos, ref bool aPesos, ref bool aFigurasPesos, ref bool aValoracion, ref bool aCPs, ref bool aGruposEquipos, ref bool aControlGrupos, ref bool aControlConjuntos, ref bool aDiferencias)
        {
            XmlNodeList XMLConfigAnalisis = configFile.GetElementsByTagName("analisis");

            XmlNode xmlAnalisis = XMLConfigAnalisis[0].ChildNodes[0];

            aTodo = Convert.ToBoolean(xmlAnalisis.Attributes["aTodo"].Value);
            aVX2 = Convert.ToBoolean(xmlAnalisis.Attributes["aVX2"].Value);
            aSeguidos = Convert.ToBoolean(xmlAnalisis.Attributes["aSeguidos"].Value);
            aFigurasV1X2 = Convert.ToBoolean(xmlAnalisis.Attributes["aFigurasV1X2"].Value);
            aInterrupciones = Convert.ToBoolean(xmlAnalisis.Attributes["aInterrupciones"].Value);
            aSimetrias = Convert.ToBoolean(xmlAnalisis.Attributes["aSimetrias"].Value);
            aFormatos = Convert.ToBoolean(xmlAnalisis.Attributes["aFormatos"].Value);
            aFormatos123 = Convert.ToBoolean(xmlAnalisis.Attributes["aFormatos123"].Value);
            aDistancias = Convert.ToBoolean(xmlAnalisis.Attributes["aDistancias"].Value);
            aContactos = Convert.ToBoolean(xmlAnalisis.Attributes["aContactos"].Value);
            aFigurasContactos = Convert.ToBoolean(xmlAnalisis.Attributes["aFigurasContactos"].Value);
            aPesos = Convert.ToBoolean(xmlAnalisis.Attributes["aPesos"].Value);
            aFigurasPesos = Convert.ToBoolean(xmlAnalisis.Attributes["aFigurasPesos"].Value);
            aValoracion = Convert.ToBoolean(xmlAnalisis.Attributes["aValoracion"].Value);
            aCPs = Convert.ToBoolean(xmlAnalisis.Attributes["aCPs"].Value);
            aDibujos = Convert.ToBoolean(xmlAnalisis.Attributes["aDibujos"].Value);
            aGruposEquipos = Convert.ToBoolean(xmlAnalisis.Attributes["aGruposEquipos"].Value);
            aControlGrupos = Convert.ToBoolean(xmlAnalisis.Attributes["aControlGrupos"].Value);
            aControlConjuntos = Convert.ToBoolean(xmlAnalisis.Attributes["aControlConjuntos"].Value);
            aDiferencias = Convert.ToBoolean(xmlAnalisis.Attributes["aDiferencias"].Value);
        }

        public void ObtenNumPartidos(ref int numPartidos, ref string[] separador)
		{
            XmlNodeList XMLConfigP = configFile.GetElementsByTagName( "partidos" );
			XmlNode xmlPartidos = XMLConfigP[0].ChildNodes[0];
			numPartidos = Convert.ToInt16(xmlPartidos.Attributes["numero"].Value);
			string tmp = Convert.ToString(xmlPartidos.Attributes["separador"].Value);
			separador=tmp.Split(',');
		}
		public void ObtenNumPartidos(ref int numPartidos, ref string separador)
		{
		    XmlNodeList XMLConfigP = configFile.GetElementsByTagName( "partidos" );
			XmlNode xmlPartidos = XMLConfigP[0].ChildNodes[0];
			numPartidos = Convert.ToInt16(xmlPartidos.Attributes["numero"].Value);
			separador = Convert.ToString(xmlPartidos.Attributes["separador"].Value);
		}
		public void ObtenDesplazamiento(ref int desplazamiento)
		{
		    XmlNodeList XMLConfigP = configFile.GetElementsByTagName( "desplazamiento" );
			XmlNode xmlPartidos;
			try
			{
				xmlPartidos = XMLConfigP[0].ChildNodes[0];
				desplazamiento = Convert.ToInt16(xmlPartidos.Attributes["valor"].Value);
			}
			catch{}
		}

        public void ObtenInfoIdioma(ref string idioma)
        {
            XmlNodeList XMLConfigIdioma = configFile.GetElementsByTagName("idioma");
            XmlNode xmlIdioma = XMLConfigIdioma[0].ChildNodes[0];
            idioma = xmlIdioma.Attributes["valor"].Value;
        }
        public void ObtenIdioma(ref Dictionary<string, string> archivoIdioma, string idioma)
        {
            ArchivoIdioma aIdioma = new ArchivoIdioma();
            archivoIdioma = aIdioma.ObtenerIdioma(idioma);
        }
        public void ObtenEstadoNotificaciones(ref SortedList<int, string> notificaciones)
        {
            StreamReader sr = new StreamReader(System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) + "/Comunicaciones/comunicaciones.free1x2");
            notificaciones.Clear();

            while (sr.Peek() != -1)
            {
                string[] info = sr.ReadLine().Split('#');
                if (!notificaciones.ContainsKey(Convert.ToInt32(info[0])))
                {
                    notificaciones.Add(Convert.ToInt32(info[0]), info[1] + "#" + info[2]);
                }
            }
            sr.Close();
        }
        public void ObtenFechaUltimaComprobacionNotificaciones(ref DateTime fecha)
        {
            XmlNodeList XMLConfigP = configFile.GetElementsByTagName("notificaciones");
            XmlNode xmlFecha;
            try
            {
                xmlFecha = XMLConfigP[0].ChildNodes[0];
                fecha = Convert.ToDateTime(xmlFecha.Attributes["valor"].Value);
            }
            catch { }
        }
		public void GuardarPuntosCP(int puntosFijos, int puntosDobles, int puntosTriples)
		{
		    XmlNodeList XMLConfigCP = configFile.GetElementsByTagName( "CP" );
			XmlNode xmlPuntosCP = XMLConfigCP[0].ChildNodes[0];
			xmlPuntosCP.Attributes["fijos"].Value = puntosFijos.ToString();
			xmlPuntosCP.Attributes["dobles"].Value = puntosDobles.ToString();
			xmlPuntosCP.Attributes["triples"].Value = puntosTriples.ToString();
			//guardar a disco...
			XmlTextWriter writer = new XmlTextWriter(archConfig, null);
			writer.Formatting = Formatting.Indented;
			configFile.Save(writer);	
			writer.Close();
		}
        public void GuardarToolBarsVisibles(bool tsFree, bool tsFiltros, bool tsCombinacion, bool tsOperaciones, bool tsArchivo, bool tsUtilidades)
        {
            XmlNodeList XMLConfigToolBars = configFile.GetElementsByTagName("toolbars");

            XmlNode xmlToolBars = XMLConfigToolBars[0].ChildNodes[0];

            xmlToolBars.Attributes["tsFree"].Value = tsFree.ToString();
            xmlToolBars.Attributes["tsFiltros"].Value = tsFiltros.ToString();
            xmlToolBars.Attributes["tsCombinacion"].Value = tsCombinacion.ToString();
            xmlToolBars.Attributes["tsOperaciones"].Value = tsOperaciones.ToString();
            xmlToolBars.Attributes["tsArchivo"].Value = tsArchivo.ToString();
            xmlToolBars.Attributes["tsUtilidades"].Value = tsUtilidades.ToString();
            //guardar a disco...
            XmlTextWriter writer = new XmlTextWriter(archConfig, null);
            writer.Formatting = Formatting.Indented;
            configFile.Save(writer);
            writer.Close();
        }
        public void GuardarConfiguracionActualizador(bool actualizarAlInicio)
        {
            XmlNodeList XMLConfigActualizador = configFile.GetElementsByTagName("actualizador");

            XmlNode xmlActualizador= XMLConfigActualizador[0].ChildNodes[0];

            xmlActualizador.Attributes["actualizarAlInicio"].Value = actualizarAlInicio.ToString();

            //guardar a disco...
            XmlTextWriter writer = new XmlTextWriter(archConfig, null);
            writer.Formatting = Formatting.Indented;
            configFile.Save(writer);
            writer.Close();
        }
        public void GuardarConfiguracionAnalisis(bool aTodo, bool aVX2, bool aSeguidos, bool aFigurasV1X2, bool aInterrupciones, bool aDibujos, bool aSimetrias, bool aFormatos, bool aFormatos123, bool aDistancias, bool aContactos, bool aFigurasContactos, bool aPesos, bool aFigurasPesos, bool aValoracion, bool aCPs, bool aGruposEquipos, bool aControlGrupos, bool aControlConjuntos, bool aDiferencias)
        {
            XmlNodeList XMLConfigAnalisis = configFile.GetElementsByTagName("analisis");

            XmlNode xmlAnalisis = XMLConfigAnalisis[0].ChildNodes[0];

            xmlAnalisis.Attributes["aTodo"].Value = aTodo.ToString();
            xmlAnalisis.Attributes["aVX2"].Value = aVX2.ToString();
            xmlAnalisis.Attributes["aSeguidos"].Value = aSeguidos.ToString();
            xmlAnalisis.Attributes["aFigurasV1X2"].Value = aFigurasV1X2.ToString();
            xmlAnalisis.Attributes["aDibujos"].Value = aDibujos.ToString();
            xmlAnalisis.Attributes["aInterrupciones"].Value = aInterrupciones.ToString();
            xmlAnalisis.Attributes["aFormatos"].Value = aFormatos.ToString();
            xmlAnalisis.Attributes["aFormatos123"].Value = aFormatos123.ToString();
            xmlAnalisis.Attributes["aSimetrias"].Value = aSimetrias.ToString();
            xmlAnalisis.Attributes["aContactos"].Value = aContactos.ToString();
            xmlAnalisis.Attributes["aFigurasContactos"].Value = aFigurasContactos.ToString();
            xmlAnalisis.Attributes["aPesos"].Value = aPesos.ToString();
            xmlAnalisis.Attributes["aFigurasPesos"].Value = aFigurasPesos.ToString();
            xmlAnalisis.Attributes["aCPs"].Value = aCPs.ToString();
            xmlAnalisis.Attributes["aValoracion"].Value = aValoracion.ToString();
            xmlAnalisis.Attributes["aGruposEquipos"].Value = aGruposEquipos.ToString();
            xmlAnalisis.Attributes["aDistancias"].Value = aDistancias.ToString();
            xmlAnalisis.Attributes["aControlGrupos"].Value = aControlGrupos.ToString();
            xmlAnalisis.Attributes["aControlConjuntos"].Value = aControlConjuntos.ToString();
            xmlAnalisis.Attributes["aDiferencias"].Value = aDiferencias.ToString();

            //guardar a disco...
            XmlTextWriter writer = new XmlTextWriter(archConfig, null);
            writer.Formatting = Formatting.Indented;
            configFile.Save(writer);
            writer.Close();
        }

		public void GuardarConfiguracionBoleto(int numPartidos, string separador)
		{
		    XmlNodeList XMLConfigBol = configFile.GetElementsByTagName( "partidos" );
			XmlNode xmlPuntosBol = XMLConfigBol[0].ChildNodes[0];
			xmlPuntosBol.Attributes["numero"].Value = numPartidos.ToString();
			xmlPuntosBol.Attributes["separador"].Value = separador;
			//guardar a disco...
			XmlTextWriter writer = new XmlTextWriter(archConfig, null);
			writer.Formatting = Formatting.Indented;
			configFile.Save(writer);	
			writer.Close();
		}

		public void GuardarDesplazamiento(string num)
		{
		    XmlNodeList XMLConfigBol = configFile.GetElementsByTagName("desplazamiento");
		    try
			{
				XmlNode xmlPuntosBol= XMLConfigBol[0].ChildNodes[0];
				xmlPuntosBol.Attributes["valor"].Value = num;
				//guardar a disco...
				XmlTextWriter writer = new XmlTextWriter(archConfig, null);
				writer.Formatting = Formatting.Indented;
				configFile.Save(writer);	
				writer.Close();
			}
			catch{}
		}
        public void GuardarFechaUltimaComprobacionNotificaciones(DateTime fecha)
        {
            XmlNodeList XMLFecha = configFile.GetElementsByTagName("notificaciones");
            try
            {
                XmlNode xmlFecha = XMLFecha[0].ChildNodes[0];
                xmlFecha.Attributes["valor"].Value = fecha.ToShortDateString();
                //guardar a disco...
                XmlTextWriter writer = new XmlTextWriter(archConfig, null);
                writer.Formatting = Formatting.Indented;
                configFile.Save(writer);
                writer.Close();
            }
            catch { }
        }

	    public void GuardarIdioma(string idioma)
        {
            XmlNodeList XMLConfigIdioma = configFile.GetElementsByTagName("idioma");
            try
            {
                XmlNode xmlIdioma = XMLConfigIdioma[0].ChildNodes[0];
                xmlIdioma.Attributes["valor"].Value = idioma;
                //guardar a disco...
                XmlTextWriter writer = new XmlTextWriter(archConfig, null);
                writer.Formatting = Formatting.Indented;
                configFile.Save(writer);
                writer.Close();
            }
            catch { }
        }
        // Notification system removed for performance

        public void GuardarValoresSeparadorJB(string[] valores)
		{
			string texto="";
			for(int i=0;i<valores.Length;i++)
			{
				texto+=valores[i];
				if(i<(valores.Length-1)) texto+=",";
			}
		    XmlNodeList XMLConfigJB = configFile.GetElementsByTagName( "utilidades" );
			XmlNode xmlPuntosJB = XMLConfigJB[0].ChildNodes[0];
			xmlPuntosJB.Attributes["valores"].Value = texto;
			//guardar a disco...
			XmlTextWriter writer = new XmlTextWriter(archConfig, null);
			writer.Formatting = Formatting.Indented;
			configFile.Save(writer);	
			writer.Close();
		}

		public void GuardarValoresLAE(double PrecioApuesta, double PorcentajeDestinadoAl14, double Recaudacion, string simboloMoneda)
		{
		    XmlNodeList XMLConfigLAE = configFile.GetElementsByTagName( "LAE" );

			XmlNode xmlValoresLAE = XMLConfigLAE[0].ChildNodes[0];

            xmlValoresLAE.Attributes["PrecioApuesta"].Value = PrecioApuesta.ToString("0.00", new System.Globalization.CultureInfo("es-ES"));
            xmlValoresLAE.Attributes["PorcentajeDestinadoAl14"].Value = PorcentajeDestinadoAl14.ToString(new System.Globalization.CultureInfo("es-ES"));
            xmlValoresLAE.Attributes["Recaudacion"].Value = Recaudacion.ToString(new System.Globalization.CultureInfo("es-ES"));
            xmlValoresLAE.Attributes["Moneda"].Value = simboloMoneda;

			//guardar a disco...

			XmlTextWriter writer = new XmlTextWriter(archConfig, null);
			writer.Formatting = Formatting.Indented;
			configFile.Save(writer);	
			writer.Close();

		}

        public void GuardarConfiguracionAdvertenciaSalir(bool mostrarConfirmacionSalir)
        {
            XmlNodeList XMLConfigActualizador = configFile.GetElementsByTagName("salir");

            XmlNode xmlActualizador = XMLConfigActualizador[0].ChildNodes[0];

            xmlActualizador.Attributes["confirmacionSalir"].Value = mostrarConfirmacionSalir.ToString();

            //guardar a disco...
            XmlTextWriter writer = new XmlTextWriter(archConfig, null);
            writer.Formatting = Formatting.Indented;
            configFile.Save(writer);
            writer.Close();
        }

		public void ObtenValoresLAE(ref double PrecioApuesta, ref double PorcentajeDestinadoAl14, ref double Recaudacion, ref string simboloMoneda)
		{
		    XmlNodeList XMLConfigLAE = configFile.GetElementsByTagName( "LAE" );
			XmlNode xmlValoresLAE = XMLConfigLAE[0].ChildNodes[0];

            PrecioApuesta = float.Parse(xmlValoresLAE.Attributes["PrecioApuesta"].Value, new System.Globalization.CultureInfo("es-ES"));

            PorcentajeDestinadoAl14 = float.Parse(xmlValoresLAE.Attributes["PorcentajeDestinadoAl14"].Value, new System.Globalization.CultureInfo("es-ES"));
            Recaudacion = float.Parse(xmlValoresLAE.Attributes["Recaudacion"].Value, new System.Globalization.CultureInfo("es-ES"));
            
            simboloMoneda = xmlValoresLAE.Attributes["Moneda"].Value;
		}

		public void ObtenValoresLAE(ref string PrecioApuesta, ref string PorcentajeDestinadoAl14, ref string Recaudacion, ref string simboloMoneda)
		{
		    XmlNodeList XMLConfigLAE = configFile.GetElementsByTagName( "LAE" );
			XmlNode xmlValoresLAE = XMLConfigLAE[0].ChildNodes[0];

			PrecioApuesta = xmlValoresLAE.Attributes["PrecioApuesta"].Value;
			PorcentajeDestinadoAl14 = xmlValoresLAE.Attributes["PorcentajeDestinadoAl14"].Value;
			Recaudacion = xmlValoresLAE.Attributes["Recaudacion"].Value;
            simboloMoneda = xmlValoresLAE.Attributes["Moneda"].Value;
		}

		public string[] ObtenEquipos(int division)
		{
            List<string> equiposArray = new List<string>();

            equiposArray.Add("");

		    XmlNodeList XMLConfigEquipos = configFile.GetElementsByTagName( "equipos" );

		    for(int i = 0; i < XMLConfigEquipos[0].ChildNodes.Count; i++)
			{
				XmlNode xmlEquipo = XMLConfigEquipos[0].ChildNodes[i];
				int div = Convert.ToInt32(xmlEquipo.Attributes["division"].Value);

				if(division == div)
				{
					equiposArray.Add(xmlEquipo.InnerText);
				}			
			}

			 return (string[])equiposArray.ToArray();
		}

		public string ObtenValoresUtilSeparadorJB()
		{
		    XmlNodeList XMLConfig = configFile.GetElementsByTagName( "separadorJB" );
			
			return XMLConfig[0].Attributes["valores"].Value;
		}
	}
}
