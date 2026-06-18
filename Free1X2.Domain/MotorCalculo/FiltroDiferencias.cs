// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
    public class FiltroDiferencias : IFiltro
    {
        protected List<Diferencia> diferencia;
        protected bool contieneDatos;
        protected bool isActive = false;

        public FiltroDiferencias()
        {
            diferencia = new List<Diferencia>();
        }

        public List<Diferencia> Diferencias
        {
            get { return diferencia; }
            set { diferencia = value; }
        }

        #region Miembros de IFiltro

        public bool Analizar(long columna)
        {
            for (int i = 0; i < Diferencias.Count; i++)
            {
                if (!Diferencias[i].Analizar(columna))
                {
                    return false;
                }
            }
            return true;
        }

        public string[] AnalizarFallos(long columna)
        {
            string[] arrayFallos = null;
            string texto = "";
            for (int i = 0; i < Diferencias.Count; i++)
            {
                Diferencia dif = Diferencias[i];
                if(!dif.Analizar(columna))               
                {
                    texto += "Fallos en Diferencias " + Convert.ToString(i + 1) + ": ";
                    if(dif.AnalizaVX2Dib)
                    {
                        if (!dif.AcV[dif.NoVariantes])
                        {
                            texto += "Variantes (" + dif.NoVariantes + ") ";
                        }
                        if (!dif.AcX[dif.NoEquis])
                        {
                            texto += "Equis (" + dif.NoEquis + ") ";
                        }
                        if (!dif.AcDoses[dif.NoDoses])
                        {
                            texto += "Doses (" + dif.NoDoses + ") ";
                        } 
                        if (!dif.AcDib[dif.NoDibujos])
                        {
                            texto += "Dibujos (" + dif.NoDibujos + ") ";
                        }
                    }
                    if (dif.AnalizaInterrupciones)
                    {
                        if (!dif.AcInt[dif.NoInterrupciones]) texto += "Interrupciones (" + dif.NoInterrupciones + ") ";
                    }
                    if (dif.AnalizaFormatos)
                    {
                        if (!dif.AcFormatos[dif.NoFormatos]) texto += "Formatos (" + dif.NoFormatos + ")#";
                    }
                }
            }
            if (texto.Length > 0)
            {
                arrayFallos = texto.Split('#');
            }
            return arrayFallos;
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value;}
        }

        public bool EsNombreFiltro(string nombre)
        {
            if (nombre.Equals(Filtro.Diferencias.ToString()))
            {
                return true;
            }
            return false;
        }

        public Filtro NombreFiltro
        {
            get
            {
                return Filtro.Diferencias;
            }
        }

        public bool ContieneDatos
        {
            get
            {
                if (Diferencias.Count == 0)
                {
                    contieneDatos = false;
                    return false;
                }
                return contieneDatos = true;
            }
            set
            {
                contieneDatos = value;
            }
        }

        public int ObtenNoAciertosTolerancias(string letrasTolerancias)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool UsaFiguras()
        {
            return false;
        }

        public List<long> Figuras
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool AnalisisActivo
        {
            get { return VariablesGlobales.AnalizarSimetriasII; }
        }

        #endregion
    }
}
