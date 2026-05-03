using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Free1X2.MotorCalculo;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class ListaImpresoras : Form
    {
        ControladorImpresion cont;
        List<ControladorImpresion> arrayImpresoras;
        public ListaImpresoras(ControladorImpresion controlador)
        {
            InitializeComponent();
            cont = controlador;
            try
            {
                CargarListaImpresoras();
            }
            catch
            {
                lblMensaje.Text = "No se han encontrado impresoras";
            }
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

        protected void CargarListaImpresoras()
        {
            listBox1.Items.Clear();
            ControladoresImpresion lista = new ControladoresImpresion();
            arrayImpresoras = lista.Impresoras;
            if (arrayImpresoras.Count > 0)
            {
                for (int i = 0; i < arrayImpresoras.Count; i++)
                {
                    listBox1.Items.Add(arrayImpresoras[i].Modelo);
                }
            }
            else
            {
                lblMensaje.Text = "No hay impresoras";
                cont = null;
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            ControladorImpresion controladorImp = BuscarImpresora(listBox1.SelectedItem.ToString());
            cont.Modelo = controladorImp.Modelo;
            cont.MargenSuperior = controladorImp.MargenSuperior;
            cont.MargenIzquierda = controladorImp.MargenIzquierda;
            cont.Rotar = controladorImp.Rotar;
            Close();
        }
        protected ControladorImpresion BuscarImpresora(string modelo)
        {
            ControladorImpresion control = new ControladorImpresion();
            for (int i = 0; i < arrayImpresoras.Count; i++)
            {
                control = arrayImpresoras[i];
                if (control.Modelo == modelo)
                {
                    break;
                }
            }
            return control;
        }
    }
}