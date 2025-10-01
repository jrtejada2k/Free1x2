using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.Comunicacion;
using Free1X2.EntradaSalida;
// using Free1X2.SVC_Actualizador; // TODO: Replace with modern HTTP client

namespace Free1X2.UI
{
    public partial class NotificacionesFrm : Form
    {
        int idSeleccionado = 0;
        List<Notificacion> notificaciones = new List<Notificacion>();
        public NotificacionesFrm()
        {
            InitializeComponent();
            ObtenerNotificaciones();
            MostrarMensajes();
            tvMensajes.ExpandAll();
        }
        private void ObtenerNotificaciones()
        {
            GestorNotificaciones gestor = new GestorNotificaciones();
            notificaciones = gestor.ObtenerNotificaciones();          
        }
        private void MostrarMensajes()
        {
            tvMensajes.Nodes.Clear();
            lblRemitente.Text = "";
            lblTitulo.Text = "";
            webBrowser1.DocumentText = "";
            TreeNode nodoMain = new TreeNode();

            nodoMain.Name = "Principal";

            for (int i = 0; i < notificaciones.Count; i++)
            {
                if (!notificaciones[i].Borrada)
                {
                    TreeNode nodo = new TreeNode("#" + notificaciones[i].IdNotificacion.ToString() + "  " + notificaciones[i].Titulo + " - Remitente: " + notificaciones[i].Remitente);
                    nodo.ContextMenuStrip = contextMenu;
                    nodo.Name = notificaciones[i].IdNotificacion.ToString();
                    if (!notificaciones[i].Leida)
                    {
                        nodo.NodeFont = new Font("Verdana", 8, FontStyle.Italic);
                    }
                    nodoMain.Nodes.Add(nodo);
                }
            }
            if (nodoMain.Nodes.Count > 0)
            {
                nodoMain.Text = "Notificaciones";
            }
            else
            {
                nodoMain = new TreeNode("No hay Notificaciones");
            }
            tvMensajes.Nodes.Add(nodoMain);
        }
        private Notificacion ObtenerNotificacion(int id)
        {
            for (int i = 0; i < notificaciones.Count; i++)
            {
                if (notificaciones[i].IdNotificacion == id)
                {
                    return notificaciones[i];
                }
            }
            return null;
        }
        private void EliminarNotificacion(int id)
        {
            for (int i = 0; i < notificaciones.Count; i++)
            {
                if (notificaciones[i].IdNotificacion == id)
                {
                    notificaciones[i].Borrada = true;
                    notificaciones[i].Leida = true;
                    break;
                }
            }
            SalvarEstadoNotificaciones();
            tvMensajes.ExpandAll();
        }
        private void CargarMensaje(Notificacion not)
        {
            string salto = Environment.NewLine;
            lblTitulo.Text = not.Titulo;
            lblRemitente.Text = "Remitente: " + not.Remitente;
            string htmlCode = "<html>";
            htmlCode += "<head>";
            htmlCode += "<title>";
            htmlCode += not.Titulo;
            htmlCode += "</title>";
            htmlCode += "</head>";
            htmlCode += "<body>";
            htmlCode += "<font face = \"Verdana\">";
            htmlCode += salto + not.Contenido;
            htmlCode += "</font>";
            htmlCode += "</body>";
            htmlCode += "</html>";


            webBrowser1.DocumentText = htmlCode;

            not.Leida = true;

            SalvarEstadoNotificaciones();
        }
        private void tvMensajes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (tvMensajes.SelectedNode.Name != "Principal")
                {

                    idSeleccionado = Convert.ToInt32(tvMensajes.SelectedNode.Name);

                    Notificacion not = ObtenerNotificacion(idSeleccionado);

                    if (not != null)
                    {
                        tvMensajes.SelectedNode.NodeFont = tvMensajes.Nodes[0].NodeFont;
                        CargarMensaje(not);
                    }
                }
            }
            catch
            {
            }
        }

        
        private void ActualizaFechaUltimaComprobacionNotificaciones()
        {
            AConfiguracion aConf = new AConfiguracion(Application.StartupPath);
            aConf.GuardarFechaUltimaComprobacionNotificaciones(DateTime.Now);
        }
        private void Eliminar_Click(object sender, EventArgs e)
        {
            EliminarNotificacion(idSeleccionado);
            MostrarMensajes();
        }

        private void tvMensajes_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Name != "Principal")
            {
                tvMensajes.SelectedNode = e.Node;
            }
        }
        private void SalvarEstadoNotificaciones()
        {
            AConfiguracion aConf = new AConfiguracion(Application.StartupPath);
            aConf.GuardarEstadoComunicaciones(notificaciones);
        }
        private void NotificacionesFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SalvarEstadoNotificaciones();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            ObtenerNotificaciones();
            MostrarMensajes();
            tvMensajes.ExpandAll();
        }

    }
}
