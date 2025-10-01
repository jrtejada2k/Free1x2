using System;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace Free1X2.UI
{
    public partial class GestorEquiposFrm : Form
    {
        private readonly string archivoEquiposPrimera = Application.StartupPath + "/Equipos/equipos1.dat";
        private readonly string archivoEquiposSegunda = Application.StartupPath + "/Equipos/equipos2.dat";
        private readonly string archivoEquiposSegundaB = Application.StartupPath + "/Equipos/equipos2b.dat";
        private readonly string archivoEquiposInt = Application.StartupPath + "/Equipos/equiposInt.dat";

        public GestorEquiposFrm()
        {
            InitializeComponent();
            CargaEquipos(lbEquipos1, archivoEquiposPrimera);
            CargaEquipos(lbEquipos2, archivoEquiposSegunda);
            CargaEquipos(lbEquipos2B, archivoEquiposSegundaB);
            CargaEquipos(lbEquiposInt, archivoEquiposInt);
        }

        private void CargaEquipos(ListBox equipos, string archivoEquipos)
        {
            equipos.Items.Clear();
            
            using (var sr = new StreamReader(archivoEquipos, Encoding.Default))
            {
                while (sr.Peek() != -1)
                {
                    equipos.Items.Add(sr.ReadLine());
                }

                sr.Close();
            }
        }

        private void btnASegunda_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbEquipos1.SelectedItems.Count; i++)
            {
                if (!lbEquipos2.Items.Contains(lbEquipos1.SelectedItems[i]))
                {
                    lbEquipos2.Items.Add(lbEquipos1.SelectedItems[i]);
                }
                lbEquipos1.Items.Remove(lbEquipos1.SelectedItems[i]);
            }
        }

        private void btnAPrimera_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbEquipos2.SelectedItems.Count; i++)
            {
                if (!lbEquipos1.Items.Contains(lbEquipos2.SelectedItems[i]))
                {
                    lbEquipos1.Items.Add(lbEquipos2.SelectedItems[i]);
                }
                lbEquipos2.Items.Remove(lbEquipos2.SelectedItems[i]);
            }
        }

        private void GuardarEquipos(string archivoEquipos, ListBox equipos)
        {
            using (var sw = new StreamWriter(archivoEquipos, false, Encoding.Default))
            {
                for (int i = 0; i < equipos.Items.Count; i++)
                {
                    sw.WriteLine(equipos.Items[i]);
                }
                sw.Close();
            }
        }

        private void GuardarEquiposTodasCategorias()
        {
            GuardarEquipos(archivoEquiposPrimera, lbEquipos1);
            GuardarEquipos(archivoEquiposSegunda, lbEquipos2);
            GuardarEquipos(archivoEquiposSegundaB, lbEquipos2B);
            GuardarEquipos(archivoEquiposInt, lbEquiposInt);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarEquiposTodasCategorias();
        }

        private void btnASegundaB_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbEquipos2.SelectedItems.Count; i++)
            {
                if (!lbEquipos2B.Items.Contains(lbEquipos2.SelectedItems[i]))
                {
                    lbEquipos2B.Items.Add(lbEquipos2.SelectedItems[i]);
                }
                lbEquipos2.Items.Remove(lbEquipos2.SelectedItems[i]);
            }
        }

        private void btnASegundaSube_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbEquipos2B.SelectedItems.Count; i++)
            {
                if (!lbEquipos2.Items.Contains(lbEquipos2B.SelectedItems[i]))
                {
                    lbEquipos2.Items.Add(lbEquipos2B.SelectedItems[i]);
                }
                lbEquipos2B.Items.Remove(lbEquipos2B.SelectedItems[i]);
            }
        }

        private void btnNuevoEquipo_Click(object sender, EventArgs e)
        {
            var agregarEquipo = new AgregarEquipoFrm(lbEquipos1, lbEquipos2, lbEquipos2B, lbEquiposInt);
            agregarEquipo.ShowDialog();
        }

        private void btnEliminaDe1_Click(object sender, EventArgs e)
        {
            lbEquipos1.Items.Remove(lbEquipos1.SelectedItem);
        }

        private void btnEliminaDe2_Click(object sender, EventArgs e)
        {
            lbEquipos2.Items.Remove(lbEquipos2.SelectedItem);
        }

        private void btnEliminaDe2B_Click(object sender, EventArgs e)
        {
            lbEquipos2B.Items.Remove(lbEquipos2B.SelectedItem);
        }

        private void btnEliminaDeInt_Click(object sender, EventArgs e)
        {
            lbEquiposInt.Items.Remove(lbEquiposInt.SelectedItem);
        }
    }
}