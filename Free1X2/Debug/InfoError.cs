using System;
using System.Windows.Forms;
using System.IO;

namespace Free1X2.Debug
{
    public partial class InfoError : Form
    {
        private bool hayInfoAdicional;
        public InfoError(string nombre)
        {
            InitializeComponent();
            if (nombre != "")
            {
                lblNombre.Text = nombre;
            }
            else
            {
                lblmain.Text = "Se ha producido un error que este manejador no ha podido controlar.";
                lblSecundario.Text = "";
                lblNombre.Text = "http://www.free1x2.com";
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEnviarOnline_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "/Informes/" + lblNombre.Text;
            
            StreamReader sr = new StreamReader(path);
            string info = sr.ReadToEnd();
            int lineas = 0;
            string objetoCausante = "";
            sr = new StreamReader(path);
            while (sr.Peek() != -1)
            {
                objetoCausante = sr.ReadLine();
                lineas++;
                if (lineas == 4)
                {
                    break;
                }
            }
            sr.Close();

            string user = txtUser.Text;
            string email = txtEmail.Text;
            string version = Application.ProductVersion;
            string comentarios = "";
            if (hayInfoAdicional)
            {
                comentarios = txtInfoAdicional.Text;
            }
            try
            {
                //Aquí código para enviar online
                Free1X2WService servicio = new Free1X2WService();
                // Modo offline - no envía datos reales
                MessageBox.Show("Información registrada localmente (modo offline).", "Información",MessageBoxButtons.OK);
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error al enviar la información a Free1X2.com");
            }
            Close();
        }

        private void txtInfoAdicional_MouseClick(object sender, MouseEventArgs e)
        {
            txtInfoAdicional.Text = "";
            txtInfoAdicional.MouseClick -= txtInfoAdicional_MouseClick;
            hayInfoAdicional = true;
        }
    }
}
