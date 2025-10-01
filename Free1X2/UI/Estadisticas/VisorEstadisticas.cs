using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Free1X2.MotorCalculo.Estadisticas;

namespace Free1X2.UI.Estadisticas
{
    public partial class VisorEstadisticas : Form
    {
        List<Estadistica> stats;
        public VisorEstadisticas(List<Estadistica> est)
        {
            InitializeComponent();

            stats = est;
            LlenarEstadisticas();
        }

        protected void LlenarEstadisticas()
        {
            //Montar un Dataset
            DataSet dsEst = new DataSet();
            DataTable dtEst = new DataTable("Estadísticas");

            DataColumn dtcEst = new DataColumn();
            dtcEst.DataType = Type.GetType("System.String");
            dtcEst.ColumnName = "Archivo";
            dtEst.Columns.Add(dtcEst);
            
            dtcEst = new DataColumn();
            dtcEst.DataType = Type.GetType("System.String");
            dtcEst.ColumnName = "Cumplimiento";
            dtEst.Columns.Add(dtcEst);

            for (int i = 0; i < stats.Count; i++)
            {
                DataRow dtRow = dtEst.NewRow();

                Estadistica estadistica = stats[i];

                dtRow["Archivo"] = estadistica.Archivo;
                dtRow["Cumplimiento"] = estadistica.Cumplimiento + "%";

                dtEst.Rows.Add(dtRow);
            }
            dsEst.Tables.Add(dtEst);


            dgEstadisticas.DataSource = dsEst.Tables["Estadísticas"];
        }

    }
}
