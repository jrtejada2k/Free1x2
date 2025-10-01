using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
    public class Diferencia
    {
        private List<bool[]> partidosSimetricos = new List<bool[]>();
        private bool[] acV, acX, acDoses, acDib, acInt, acFormatos;
        private bool[] resultadosV = new bool[17];
        private bool[] resultadosX = new bool[17];
        private bool[] resultadosDoses = new bool[17];
        private bool[] resultadosInt = new bool[17];
        private bool[,] resultadosDib = new bool[17, 17];
        private Dictionary<long, bool> formatos = new Dictionary<long, bool>();
        private bool analizaVX2Dib = true;
        private bool analizaFormatos = true;
        private bool analizaInterrupciones = true;

        int ResV;
        int ResX;
        int ResDoses;
        int ResDib;
        int ResInt;
        int ResFormatos;

        FiltroInterrupciones fI = new FiltroInterrupciones();
        FiltroNoVariantes f = new FiltroNoVariantes();

        public bool Analizar(long col)
        {

            //Arreglar para noperder tiempo analizando loq ue no se pide.
            ResV = 0;
            ResX = 0;
            ResDoses = 0;
            ResDib = 0;
            ResInt = 0;
            ResFormatos = 0;

            resultadosV = new bool[17];
            resultadosX = new bool[17];
            resultadosDoses = new bool[17];
            resultadosInt = new bool[17];
            resultadosDib = new bool[17, 17];
            formatos.Clear();

            for (int i = 0; i < partidosSimetricos.Count; i++)
            {              
                long columna = Utils.UtilColumnas.IgnorarPartidos(partidosSimetricos[i], col);
                if (AnalizaFormatos)
                {
                    //Formatos
                    if (!formatos.ContainsKey(columna))
                    {
                        ResFormatos++;
                        formatos.Add(columna, true);
                    }
                }
                if (AnalizaInterrupciones)
                {
                    
                    fI.AnalizaColumna(columna);
                    if(!resultadosInt[fI.NoInterrupciones])
                    {
                        ResInt++;
                        resultadosInt[fI.NoInterrupciones] = true;
                    }
                }
                if (AnalizaVX2Dib)
                {
                    //Variantes, Equis y Doses, Dibujos
                    
                    f.AnalizaColumna((columna));
                    if (!resultadosV[f.NoVariantes])
                    {
                        ResV++;
                        resultadosV[f.NoVariantes]=true;
                    }


                    if (!resultadosX[f.NoEquis])
                    {
                        ResX++;
                        resultadosX[f.NoEquis]=true;
                    }

                    if (!resultadosDoses[f.NoDoses])
                    {
                        ResDoses++;
                        resultadosDoses[f.NoDoses]=true;
                    }

                    if (!resultadosDib[f.NoEquis, f.NoDoses])
                    {
                        ResDib++;
                        resultadosDib[f.NoEquis, f.NoDoses] = true;
                    }
                }

            }
            bool esValida = true;
            if (AnalizaInterrupciones)
            {
                esValida = AcInt[ResInt];
            }
            if (AnalizaFormatos && esValida)
            {
                esValida = AcFormatos[ResFormatos];
            }
            if (AnalizaVX2Dib && esValida)
            {
                esValida = AcV[ResV] && AcX[ResX] && AcDoses[ResDoses] && AcDib[ResDib];
            }
            return esValida;
        }
        public string AnalizarFallos(long col)
        {
            string texto = "";

            ResV = 0;
            ResX = 0;
            ResDoses = 0;
            ResDib = 0;
            ResInt = 0;
            ResFormatos = 0;

            resultadosV = new bool[17];
            resultadosX = new bool[17];
            resultadosDoses = new bool[17];
            resultadosInt = new bool[17];
            resultadosDib = new bool[17, 17];
            formatos.Clear();

            for (int i = 0; i < partidosSimetricos.Count; i++)
            {
                long columna = Utils.UtilColumnas.IgnorarPartidos(partidosSimetricos[i], col);
                if (AnalizaFormatos)
                {
                    //Formatos
                    if (!formatos.ContainsKey(columna))
                    {
                        ResFormatos++;
                        formatos.Add(columna, true);
                    }
                }
                if (AnalizaInterrupciones)
                {

                    fI.AnalizaColumna(columna);
                    if (!resultadosInt[fI.NoInterrupciones])
                    {
                        ResInt++;
                        resultadosInt[fI.NoInterrupciones] = true;
                    }

                }
                if (AnalizaVX2Dib)
                {
                    //Variantes, Equis y Doses, Dibujos


                    f.AnalizaColumna((columna));
                    if (!resultadosV[f.NoVariantes])
                    {
                        ResV++;
                        resultadosV[f.NoVariantes] = true;
                    }


                    if (!resultadosX[f.NoEquis])
                    {
                        ResX++;
                        resultadosX[f.NoEquis] = true;
                    }

                    if (!resultadosDoses[f.NoDoses])
                    {
                        ResDoses++;
                        resultadosDoses[f.NoDoses] = true;
                    }

                    if (!resultadosDib[f.NoEquis, f.NoDoses])
                    {
                        ResDib++;
                        resultadosDib[f.NoEquis, f.NoDoses] = true;
                    }
                }

            }
            if (!AcV[ResV]) texto += "Fallo en cantidad de Variantes" + ResV +")#";
            if (!AcX[ResX]) texto += "Fallo en cantidad de Equis" + ResX + ")#";
            if (!AcDoses[ResDoses]) texto += "Fallo en cantidad de Doses" + ResDoses + ")#";
            if (!AcDib[ResDib]) texto += "Fallo en cantidad de Dibujos" + ResDib + ")#";
            if (!AcFormatos[ResFormatos]) texto += "Fallo en cantidad de Formatos" + ResFormatos + ")#";
            if (!AcInt[ResInt]) texto += "Fallo en cantidad de Interrupciones" + ResInt + ")#";


            return texto;
        }

        public void AñadirPartidosSimetricos(bool[] pS)
        {
            partidosSimetricos.Add(pS);
        }
        public bool[] AcV
        {
            get { return acV; }
            set { acV = value; }
        }
        public bool[] AcX
        {
            get { return acX; }
            set { acX = value; }
        }
        public bool[] AcDoses
        {
            get { return acDoses; }
            set { acDoses = value; }
        }
        public bool[] AcDib
        {
            get { return acDib; }
            set { acDib = value; }
        }
        public bool[] AcInt
        {
            get { return acInt; }
            set { acInt = value; }
        }
        public bool[] AcFormatos
        {
            get { return acFormatos; }
            set { acFormatos = value; }
        }
        public List<bool[]> PartidosSimetricos
        {
            get { return partidosSimetricos; }
            set { partidosSimetricos = value; }
        }
        public bool AnalizaVX2Dib
        {
            get { return analizaVX2Dib; }
            set { analizaVX2Dib = value; }
        }
        public bool AnalizaFormatos
        {
            get { return analizaFormatos; }
            set { analizaFormatos = value; }
        }
        public bool AnalizaInterrupciones
        {
            get { return analizaInterrupciones; }
            set { analizaInterrupciones = value; }
        }
        public int NoVariantes
        {
            get { return ResV; }
        }
        public int NoEquis
        {
            get { return ResX; }
        }
        public int NoDoses
        {
            get { return ResDoses; }
        }
        public int NoDibujos
        {
            get { return ResDib; }
        }
        public int NoInterrupciones
        {
            get { return ResInt; }
        }
        public int NoFormatos
        {
            get { return ResFormatos; }
        }
    }
}
