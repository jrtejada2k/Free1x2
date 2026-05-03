using System;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class AgregarEquipoFrm : Form
    {
        ListBox lista1, lista2, lista2B, listaInt;
        public AgregarEquipoFrm(ListBox lb1, ListBox lb2, ListBox lb2B, ListBox lbInt)
        {
            InitializeComponent();
            lista1 = lb1;
            lista2 = lb2;
            lista2B = lb2B;
            listaInt = lbInt;
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        private void btnNuevoEquipo_Click(object sender, EventArgs e)
        {
            if (txtEquipo.Text != "")
            {
                if (rdbPrimera.Checked)
                {
                    if (!lista1.Items.Contains(txtEquipo.Text))
                    {
                        lista1.Items.Add(txtEquipo.Text);
                    }
                }
                else if (rdbSegunda.Checked)
                {
                    if (!lista2.Items.Contains(txtEquipo.Text))
                    {
                        lista2.Items.Add(txtEquipo.Text);
                    }
                }
                else if(rdbSegundaB.Checked)
                {
                    if (!lista2B.Items.Contains(txtEquipo.Text))
                    {
                        lista2B.Items.Add(txtEquipo.Text);
                    }
                }
                else
                {
                    if (!listaInt.Items.Contains(txtEquipo.Text))
                    {
                        listaInt.Items.Add(txtEquipo.Text);
                    }
                }
            }
            Close();
        }
    }
}