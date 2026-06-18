// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
    public class RelacionCP2
    {
        protected List<int> columnasA = new List<int>();
        protected List<int> columnasB = new List<int>();

        protected List<int> columnasA2 = new List<int>();
        protected List<int> columnasB2 = new List<int>();

        protected List<int> arrayAciertos = new List<int>();
        protected List<int> arrayAciertos2 = new List<int>();

        protected string concepto = ""; // AC, ACS, FS
        protected string cantidad = ""; // Más, Menos

        protected string concepto2 = ""; // AC, ACS, FS
        protected string cantidad2 = ""; // Más, Menos

        protected int AC_A, AC_B, ACS_A, ACS_B, FS_A, FS_B;
        protected List<ColumnaProbable> columnasProbables;

        protected string strColsA, strColsB, strAciertos;

        protected string strColsA2, strColsB2, strAciertos2;

        protected bool AnalizaGlobales()
        {
            bool esValida = true;
            ReinicializarAciertos();
            ObtenerAciertosCPsA(ColumnasA);
            ObtenerAciertosCPsB(ColumnasB);
            switch (Cantidad)
            {
                case "Más":
                    switch (Concepto)
                    {
                        case "AC":
                           esValida = Aciertos.Contains(AC_A - AC_B);
                            
                            break;
                        case "ACS":
                           esValida = Aciertos.Contains(ACS_A - ACS_B);
                            
                            break;
                        case "FS":
                            esValida = Aciertos.Contains(FS_A - FS_B);
                            
                            break;
                    }
                    break;
                case "Menos":
                    switch (Concepto)
                    {
                        case "AC":
                            esValida = Aciertos.Contains(AC_B - AC_A);
                            
                            break;
                        case "ACS":
                            esValida = Aciertos.Contains(ACS_B - ACS_A);
                            
                            break;
                        case "FS":
                            esValida = Aciertos.Contains(FS_B - FS_A);
                            
                            break;
                    }
                    break;

            }
            return esValida;
        }
        protected bool AnalizaIndividuales()
        {
            bool esValida = true;
            //Será válida si todas las columnas de A cumplen la condición
            //respecto a todas las columnas de B

            for (int i = 0; i < ColumnasA2.Count; i++)
            {
                if (!esValida)
                {
                    break;
                }
                ColumnaProbable cp = ColumnasProbables[ColumnasA2[i]];

                for (int j = 0; j < ColumnasB2.Count; j++)
                {
                    ColumnaProbable cp2 = ColumnasProbables[ColumnasB2[j]];
                    int aciertos = 0;
                    switch (Cantidad2)
                    {
                        case "Más":
                            switch (Concepto2)
                            {
                                case "AC":
                                    aciertos = cp.NoAC - cp2.NoAC;
                                    break;
                                case "ACS":
                                    aciertos = cp.NoACS - cp2.NoACS;
                                    break;
                                case "FS":
                                    aciertos = cp.NoFS - cp2.NoFS;
                                    break;
                            }
                            break;
                        case "Menos":
                            switch (Concepto2)
                            {
                                case "AC":
                                    aciertos = cp2.NoAC - cp.NoAC;
                                    break;
                                case "ACS":
                                    aciertos = cp2.NoACS - cp.NoACS;
                                    break;
                                case "FS":
                                    aciertos = cp2.NoFS - cp.NoFS;
                                    break;
                            }
                            break;
                    }
                    if (Aciertos2.IndexOf(aciertos)== -1)
                    {
                        esValida = false;
                        break;
                    }
                }
            }
            
            return esValida;
        }

        public bool Analizar()
        {
            bool esValida = true;
            //Analizar sólo lo que esté activo
            if (ActivaGlobales)
            {
                esValida = AnalizaGlobales();
            }

            if (ActivaIndividuales && esValida)
            {
                esValida = AnalizaIndividuales();
            }
            return esValida;
        }

        public string Analiza(int numRel)
        {
            string txt = "";
            if(ActivaGlobales)
            {
                if (!AnalizaGlobales())
                {
                    txt += "Fallo en Relaciones II - Globales#";
                }
            }
            if(ActivaIndividuales)
            {
                if (!AnalizaIndividuales())
                {
                    txt += "Fallo en Relaciones II - Individuales#";  
                }
            }
            return txt;
        }

        protected void ReinicializarAciertos()
        {
            AC_A = AC_B = 0;
            ACS_A = ACS_B = 0;
            FS_A = FS_B = 0;
        }
        protected void ObtenerAciertosCPsA(List<int> Cols)
        {            
            for (int i = 0; i < Cols.Count; i++)
            {
                ColumnaProbable cp = columnasProbables[Cols[i]];
                AC_A += cp.NoAC;
                ACS_A += cp.NoACS;
                FS_A += cp.NoFS;
            }
        }
        protected void ObtenerAciertosCPsB(List<int> Cols)
        {
            for (int i = 0; i < Cols.Count; i++)
            {
                ColumnaProbable cp = columnasProbables[Cols[i]];
                AC_B += cp.NoAC;
                ACS_B += cp.NoACS;
                FS_B += cp.NoFS;
            }
        }


        #region Propiedades
        public List<int> ColumnasA
        {
            get { return columnasA; }
            set { columnasA = value; }
        }
        public List<int> ColumnasB
        {
            get { return columnasB; }
            set { columnasB = value; }
        }
        public List<int> Aciertos
        {
            get { return arrayAciertos; }
            set { arrayAciertos = value; }
        }
        public string Concepto
        {
            get { return concepto; }
            set { concepto = value; }
        }
        public string Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

        public List<int> ColumnasA2
        {
            get { return columnasA2; }
            set { columnasA2 = value; }
        }
        public List<int> ColumnasB2
        {
            get { return columnasB2; }
            set { columnasB2 = value; }
        }
        public List<int> Aciertos2
        {
            get { return arrayAciertos2; }
            set { arrayAciertos2 = value; }
        }
        public string Concepto2
        {
            get { return concepto2; }
            set { concepto2 = value; }
        }
        public string Cantidad2
        {
            get { return cantidad2; }
            set { cantidad2 = value; }
        }

        public List<ColumnaProbable> ColumnasProbables
        {
            get { return columnasProbables; }
            set { columnasProbables = value; }
        }
        public string StrColsA
        {
            get { return strColsA; }
            set { strColsA = value; }
        }
        public string StrColsB
        {
            get { return strColsB; }
            set { strColsB = value; }
        }
        public string StrAciertos
        {
            get { return strAciertos; }
            set { strAciertos = value; }
        }

        public string StrColsA2
        {
            get { return strColsA2; }
            set { strColsA2 = value; }
        }
        public string StrColsB2
        {
            get { return strColsB2; }
            set { strColsB2 = value; }
        }
        public string StrAciertos2
        {
            get { return strAciertos2; }
            set { strAciertos2 = value; }
        }

        protected bool ActivaGlobales
        {
            get
            {
                return (ColumnasA.Count > 0 && ColumnasB.Count > 0 && Aciertos.Count > 0);
            }
        }

        protected bool ActivaIndividuales
        {
            get
            {
                return (ColumnasA2.Count > 0 && ColumnasB2.Count > 0 && Aciertos2.Count > 0);
            }
        }

        #endregion
    }
}
