using System.Windows.Forms;

namespace Free1X2.UI
{
    public partial class AyudaFrm : Form
    {
        public AyudaFrm()
        {
            InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }

        private static void linkManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com/DocWK/index.php?title=Inicio");
        }

        private static void linkArticulos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com/Free1X2/Como.aspx");
        }

        private static void linkForo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com/foros/index.php");
        }

        private static void linkFAQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com/DocWK/index.php?title=FAQ");
        }

        private static void linkRecursos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com/Free1X2/RecursosFree1X2.aspx");
        }

        private void lnkNotificaciones_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NotificacionesFrm notificacionesFrm = new NotificacionesFrm();
            notificacionesFrm.ShowDialog();
        }

        private void lnkFacebook_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.facebook.com/Free1x2#/pages/free1x2/175884348193?ref=sgm");
        }


    }
}