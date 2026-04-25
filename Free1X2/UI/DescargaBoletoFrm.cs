using System;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class DescargaBoletoFrm : Form
    {
        string boleto = "";
        private MainForm f;
        public DescargaBoletoFrm( MainForm form)
        {
            InitializeComponent();
            InicializarComboBoxes();
            f = form;
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

        private void InicializarComboBoxes()
        {
            for (int i = 1; i <= 60; i++)
            {
                cbbJornada.Items.Add(i.ToString());
            }
            for (int i = 2005; i <= 2010; i++)
            {
                cbbTemporada.Items.Add(i + "-" + Convert.ToString(i+1));
            }

            if (DateTime.Now.Month <= 6)
            {
                cbbTemporada.Text = Convert.ToString(DateTime.Now.Year - 1) + "-" + DateTime.Now.Year;
            }
            else
            {
                cbbTemporada.Text = DateTime.Now.Year + "-" + Convert.ToString(DateTime.Now.Year + 1);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            Free1X2WService fWS = new Free1X2WService();
            string[] partes = cbbTemporada.Text.Split('-');
            if (partes.Length == 2)
            {
                boleto = fWS.ObtenerBoleto(Convert.ToInt32(cbbJornada.Text), partes[0]);

                if (boleto == "")
                {
                    lblMensaje.Text = "El Boleto elegido no está disponible";
                }
                else
                {
                    f.BoletoOnline = boleto;
                    Close();
                }
            }
            else
            {
                lblMensaje.Text = "La Temporada elegida no es correcta";
            }
        }
    }
}
