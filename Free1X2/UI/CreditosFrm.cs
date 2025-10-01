using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Free1X2.UI
{
    public partial class CreditosFrm : Form
    {       
        int yTope = -69;
        public CreditosFrm()
        {
            InitializeComponent();
            timer1.Start();
            lblFantasma.Focus();
        }

        private void Mover()
        {
            panel2.Location = new Point(panel2.Location.X, panel2.Location.Y - 1);
            Application.DoEvents();
            lblFantasma.Focus();
        }

        private void CreditosFrm_Load(object sender, EventArgs e)
        {
            lblFantasma.Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (panel2.Location.Y >= yTope)
            {
                Mover();
            }
            else
            {
                timer1.Stop();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:lfernandezrodriguez@gmail.com");
        }

        private void linkLabel1_MouseEnter(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void linkLabel1_MouseLeave(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void lnkEquipoFree_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com");
        }

        private void lnkBtesters_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.free1x2.com");
        }

        private void lnkJvallespin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:jvallespin@gmail.com");
        }

        private void lnkIndeciso_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:jjchild@lycos.co.uk");
        }

        private void lnkJoan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:duatis@coac.net");
        }

        private void lnkXfsf_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:segura33@ono.com");
        }

        private void lnkTM_Click(object sender, EventArgs e)
        {

        }

        private void lnkTM_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:toni@moreno-csa.com");
        }

        private void lnkArdoc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:ardock@gmail.com");
        }

        private void lnkArdoc_MouseEnter(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void lnkArdoc_MouseLeave(object sender, EventArgs e)
        {
            timer1.Start();
        }
        
    }
}