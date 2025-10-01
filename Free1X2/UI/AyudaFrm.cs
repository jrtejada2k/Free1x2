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
            MessageBox.Show("Online help disabled for offline operation.\\nPlease refer to local documentation.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void linkArticulos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Online help disabled for offline operation.\\nPlease refer to local documentation.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void linkForo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Online help disabled for offline operation.\\nPlease refer to local documentation.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void linkFAQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com/DocWK/index.php?title=FAQ");
        }

        private static void linkRecursos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Online help disabled for offline operation.\\nPlease refer to local documentation.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lnkNotificaciones_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Notifications system disabled for performance.", "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lnkFacebook_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Online help disabled for offline operation.\\nPlease refer to local documentation.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


    }
}