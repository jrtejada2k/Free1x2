// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
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

namespace Free1X2.MotorCalculo
{
    public class ControladoresImpresion
    {
        private XmlDocument configFile;
        private string archConfig = "";

        public ControladoresImpresion()
        {
            InicializaParametros(System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
        }
        protected void InicializaParametros(string directorioInicio)
        {
            archConfig = directorioInicio + @"/Impresion/impresoras.cfg";

            configFile = new XmlDocument();

            XmlTextReader reader = new XmlTextReader(archConfig);

            configFile.Load(reader);

            reader.Close();
        }
        public List<ControladorImpresion> ObtenListaImpresorasSoportadas()
        {
            List<ControladorImpresion> impresoras = new List<ControladorImpresion>();
            XmlNodeList XMLImpresoras = configFile.GetElementsByTagName("impresoras");
            XmlNode xmlListaImpresoras = XMLImpresoras[0];
            for (int i = 0; i < xmlListaImpresoras.ChildNodes.Count; i++)
            {               
                ControladorImpresion controlador = new ControladorImpresion();
                controlador.Modelo = xmlListaImpresoras.ChildNodes[i].Attributes["modelo"].Value;
                controlador.MargenSuperior = Convert.ToInt32(xmlListaImpresoras.ChildNodes[i].Attributes["ms"].Value);
                controlador.MargenIzquierda = Convert.ToInt32(xmlListaImpresoras.ChildNodes[i].Attributes["mi"].Value);
                controlador.Rotar = Convert.ToBoolean(xmlListaImpresoras.ChildNodes[i].Attributes["girar"].Value);
                if(!impresoras.Contains(controlador))
                {
                    impresoras.Add(controlador);
                }
            }
            return impresoras;
        }
        public List<ControladorImpresion> Impresoras
        {
            get 
            {
                return ObtenListaImpresorasSoportadas();              
            }
        }
    }
}
