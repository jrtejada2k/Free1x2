using System;
using System.Drawing;
using Free1X2.MotorCalculo;
using Free1X2.UI.Controls.Boleto;

namespace Free1X2.UI.Controls
{
    /// <summary>
    /// Descripción breve de ControlBoleto.
    /// </summary>
    public class ControlBoleto : System.Windows.Forms.UserControl
    {
        public string[] columna;
        public int[] aciertos;
        public int boletos;
        public int apuestas;
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ControlBoleto()
        {
            InitializeComponent();
            PrepararBoleto();
        }

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ControlBoleto
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Name = "ControlBoleto";
            this.Size = new System.Drawing.Size(372, 349);
            this.ResumeLayout(false);

        }
        #endregion

        private void PrepararBoleto()
        {
            int x = 0;
            int y = 0;
            for (int i = 1; i <= 8; i++)
            {
                ControlColumnaBoleto colBol = new ControlColumnaBoleto(16);
                colBol.Name = "col_" + i;
                colBol.Location = new Point(x, y);
                colBol.NumColumna = i;

                Controls.Add(colBol);
                x += colBol.Width + 2;
            }
        }

        public void LimpiarColumna(int numColumna)
        {
            string nombre = "col_" + numColumna;

            ControlColumnaBoleto ap = (ControlColumnaBoleto)Controls[numColumna - 1];

            if (ap.Name == nombre)
            {
                for (int j = 1; j < 16; j++)
                {
                    ap.LimpiarApuesta(j);
                    ap.NumColumna = 0;
                    ap.Aciertos = 0;
                }
            }
        }

        private void LimpiarBoleto()
        {
            for (int i = 0; i < 8; i++)
            {
                LimpiarColumna(i + 1);
            }
        }	

        public void LlenarColumna(int numColumna, string sColumna, int numColumnaEnCombinacion, int ac)
        {
            ControlColumnaBoleto ap = (ControlColumnaBoleto)Controls[numColumna - 1];

            for (int j = 0; j < sColumna.Length; j++)
            {
                ap.LlenarApuesta(j + 1, sColumna[j]);
                ap.NumColumna = numColumnaEnCombinacion;
                ap.Aciertos = ac;
                ap.Visible = true;

            }
            int noPartidos = sColumna.Length;
            for (int k = 16; k > noPartidos; k--)
            {
                ap.OcultarApuesta(k);
            }
        }

        public void LlenarBoleto(int boleto)
        {
            // Primero, limpia el boleto.
            LimpiarBoleto();

            int numColumna = (boleto*8)+1;
            for (int i = 0; i < 8; i++)
            {
                if ((numColumna + i - 1) < apuestas)
                {
                    LlenarColumna(i + 1, columna[numColumna + i - 1], numColumna + i, 0);
                }
                else
                {
                    LlenarColumna(i + 1, "", 0, 0);
                }
            }
        }

        public void CreaMatriz(int capacidad)
        {
            aciertos = new int[capacidad];
            columna = new string[capacidad];
            if((capacidad%8)==0)
            {
                boletos=Convert.ToInt32(capacidad/8);
            }
            else
            {
                boletos=Convert.ToInt32((capacidad/8)+0.5000001);
            }
            apuestas=capacidad;
        }

        public void OrdenarMatrizColumnas(OrdenarMatriz orden,TipoOrden tipo)
        {
            int pos;
            int j=0;
            int i;
            int[] matrizTmp=new int[columna.Length];
            string[] matrizSalida=new string[columna.Length];
            if(orden==OrdenarMatriz.Signo)
            {
                Array.Sort(columna);
                if(tipo==TipoOrden.desc)
                {
                    Array.Reverse(columna);
                }
                return;
            }
            FiltroNoVariantes fVar=new FiltroNoVariantes();
            FiltroInterrupciones fInt=new FiltroInterrupciones();
            FiltroSignosSeguidos fSeg=new FiltroSignosSeguidos();
            for(i=0;i<columna.Length;i++)
            {
                switch(orden)
                {
                    case OrdenarMatriz.Variantes:
                        {
                            fVar.AnalizaColumna(columna[i]);
                            matrizTmp[i]=fVar.NoVariantes;
                            break;
                        }
                    case OrdenarMatriz.Equis:
                        {
                            fVar.AnalizaColumna(columna[i]);
                            matrizTmp[i]=fVar.NoEquis;
                            break;
                        }
                    case OrdenarMatriz.Doses:
                        {
                            fVar.AnalizaColumna(columna[i]);
                            matrizTmp[i]=fVar.NoDoses;
                            break;
                        }
                    case OrdenarMatriz.SignosSeguidos:
                        {
                            //fSeg.AnalizaColumna(columna[i]);
                            //matrizTmp[i]=fSeg.NoSeguidos;
                            break;
                        }
                    case OrdenarMatriz.Interrupciones:
                        {
                            fInt.AnalizaColumna(columna[i]);
                            matrizTmp[i]=fInt.NoInterrupciones;
                            break;
                        }
                }
            }
            // Ahora que tenemos los valores los vamos a buscar y
            // rellenamos la nueva matriz de forma ordenada.
            if(tipo==TipoOrden.asc)
            {
                for (i = 0; i <= columna[0].Length; i++)
                {
                    pos=Array.IndexOf(matrizTmp,i);
                    while(pos>=0)
                    {
                        matrizSalida[j]=columna[pos];
                        j++;
                       
                        pos = Array.IndexOf(matrizTmp, i, pos + 1);
                        
                    }
                }
            }
            else
            {
                for(i=0;i<=columna[0].Length;i++)
                {
                    pos = Array.IndexOf(matrizTmp, columna[0].Length - i);

                    while(pos>=0)
                    {
                        matrizSalida[j]=columna[pos];
                        j++;
                        pos = Array.IndexOf(matrizTmp, columna[0].Length - i, pos + 1);
                    }
                }
            }
            Array.Copy(matrizSalida,columna,matrizSalida.Length);
        }
    }
}
