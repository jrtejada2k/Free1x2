using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Free1X2.MotorCalculo;

namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAgrupacion : UserControl
    {
        string[] agrupaciones;
        public CtrlAgrupacion(string[] lista)
        {
            InitializeComponent();
            this.agrupaciones = lista;
            LlenarAgrupaciones();
        }
        protected void LlenarAgrupaciones()
        {
            //Montar un Dataset
            DataSet dsAgrupaciones = new DataSet();
            DataTable dtAgrupaciones = new DataTable("Agrupaciones");

            DataColumn dtcAgrupaciones = new DataColumn();
            dtcAgrupaciones.DataType = System.Type.GetType("System.String");
            dtcAgrupaciones.ColumnName = "Número";
            dtAgrupaciones.Columns.Add(dtcAgrupaciones);

            dtcAgrupaciones = new DataColumn();
            dtcAgrupaciones.DataType = System.Type.GetType("System.String");
            dtcAgrupaciones.ColumnName = "Elementos";
            dtAgrupaciones.Columns.Add(dtcAgrupaciones);

            dtcAgrupaciones = new DataColumn();
            dtcAgrupaciones.DataType = System.Type.GetType("System.String");
            dtcAgrupaciones.ColumnName = "Aciertos";
            dtAgrupaciones.Columns.Add(dtcAgrupaciones);

            for (int i = 0; i < agrupaciones.Length; i++)
            {
                DataRow dtRow = dtAgrupaciones.NewRow();
                string[] valores = agrupaciones[i].Split('+');
                dtRow["Número"] = valores[0];
                dtRow["Elementos"] = valores[1];
                dtRow["Aciertos"] = valores[2];

                dtAgrupaciones.Rows.Add(dtRow);
            }
            dsAgrupaciones.Tables.Add(dtAgrupaciones);


            dataGridView1.DataSource = dsAgrupaciones.Tables["Agrupaciones"];
        }

    }
}
