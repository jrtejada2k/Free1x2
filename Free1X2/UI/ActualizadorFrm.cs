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
using System.Windows.Forms;

// using Free1X2.SVC_Actualizador; // TODO: Replace with modern HTTP client

namespace Free1X2.UI
{
    public partial class ActualizadorFrm : Form
    {
        protected string estado = "Listo";
        
        public ActualizadorFrm()
        {
            InitializeComponent();
            estado = "Listo";
            IniciarTimer();
            MostrarMensaje("Versión Instalada: " + Application.ProductVersion + "\n\n\nPreguntando a Free1X2.com");
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        protected void ComprobarVersion()
        {            
            Application.DoEvents();
            Free1X2WService actualizador = new Free1X2WService();
            try
            {
                string versionDisponible = actualizador.UltimaVersionFree1X2();
                try
                {
                    string versionActual = Application.ProductVersion;

                    string[] versiones = versionDisponible.Split('*');
                    if (versiones.Length == 2)
                    {
                        //Ha recogido una versión kamikaze y una Standard
                        string[] versionA = versiones[0].Split('-');
                        string[] versionB = versiones[1].Split('-');


                        if ((Convert.ToDouble(NormalizarVersion(versionA[0])) > Convert.ToDouble(NormalizarVersion(versionActual))) ||
                            (Convert.ToDouble(NormalizarVersion(versionB[0])) > Convert.ToDouble(NormalizarVersion(versionActual))))
                        {
                            string mensaje = "Hay nuevas versiones de Free1X2 disponibles:\n\n" + versionA[0] + " " + versionA[1] + "\n";
                            mensaje += versionB[0] + " " + versionB[1] + "\n";
                            MostrarMensaje(mensaje);
                            linkDescarga.Visible = true;
                            lnkSalir.Visible = true;
                            //Dejamos abierto el formulario
                            estado = "Nuevas Versiones Disponibles";
                        }
                        else
                        {
                            string mensaje = "Ultimas Versiones Publicadas:\n\n" + versionA[0] + " " + versionA[1] + "\n";
                            mensaje += versionB[0] + " " + versionB[1] + "\n\n";
                            mensaje += "Tienes la última versión";
                            MostrarMensaje(mensaje);
                            //Hay que cerrar el formulario
                            timer1.Interval = 4 * 1000;
                            estado = "Finalizado";
                        }
                    }
                    else
                    {
                        //Sólo ha recogido una version
                        string[] version = versionDisponible.Split('-');
                        if (Convert.ToDouble(NormalizarVersion(version[0])) > Convert.ToDouble(NormalizarVersion(versionActual)))
                        {
                            MostrarMensaje("Hay una nueva versión de Free1X2 disponible:\n\n" + version[0] + " " + version[1]);
                            linkDescarga.Visible = true;
                            lnkSalir.Visible = true;
                        }
                        else
                        {
                            MostrarMensaje("Versión Disponible: " + version[0] + " " + version[1] + "\n Versión Instalada: " + versionActual + "\n" + "Tienes la última versión");
                        }
                    }
                }
                catch
                {
                    MostrarMensaje(versionDisponible);
                }
            }
            catch
            {
                MessageBox.Show("Ha sido imposible establecer una conexión. Comprueba que estás conectado a internet", "Error", MessageBoxButtons.OK);
                this.Close();
            }
            
        }
        protected string NormalizarVersion(string numVersion)
        {
            string[] valores = numVersion.Split('.');
            int diferencia = 4 - valores.Length;
            for (int i = 0; i < diferencia; i++)
            {
                numVersion += ".0";
            }
            return numVersion;
        }
        protected void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
        private void linkDescarga_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.free1x2.com/Free1X2/Index.aspx");
                Close();
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error al intentar\nconectar con www.free1x2.com. Pruebe más tarde", "Error de Conexión", MessageBoxButtons.OK);
                return;
            }
        }

        protected void IniciarTimer()
        {
            timer1.Interval = 2 * 1000;
            timer1.Start();
        }
        protected void ReiniciarTimer()
        {
            timer1.Stop();
            timer1.Interval = 2 * 1000;
            timer1.Start();
        }
        protected void PararTimer()
        {
            timer1.Stop();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            bool hayConexion = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            
            switch (estado)
                {
                    case "Listo":
                        estado = "Comprobando Actualizaciones";
                        toolStripStatus.Text = estado;
                        ReiniciarTimer();
                        break;
                    case "Comprobando Actualizaciones":
                        if (hayConexion)
                        {
                            ComprobarVersion();
                            toolStripStatus.Text = estado;
                            ReiniciarTimer();
                        }
                        else
                        {
                            MostrarMensaje("Imposible conectar con Free1X2.com.\nCompruebe la conexión");
                            estado = "Finalizado";
                        }
                        break;
                    case "Nuevas Versiones Disponibles":
                        timer1.Enabled = false;
                        break;
                    case "Finalizado":
                        estado = "Cerrando";
                        toolStripStatus.Text = "Cerrando...";
                        break;
                    case "Cerrando":
                        Close();
                        break;
                }
            
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }
    }
}
