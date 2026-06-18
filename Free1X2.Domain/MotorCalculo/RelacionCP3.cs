// Free1X2 · WinUI 3 — WIN3
// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
// Idea original de Jose Carlos de Nova (ABDON) 
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

using System;
using System.Collections.Generic;

namespace Free1X2.MotorCalculo
{
    public class RelacionCP3
    {
        #region Propiedades Privadas
        private bool[] noSandwichsPermitidos;
        private bool[] noEscalerasAscPermitidas;
        private bool[] noEscalerasDescPermitidas;
        private bool[] noEscalerasTotalesPermitidas;
        private List<AgrupacionColumnas> agrupacionesSolapadasPermitidas;
        private List<AgrupacionColumnas> agrupacionesPasoFijoPermitidas;
        private string concepto;

        //Textos

        private string noSandwichsPermitidosString;
        private string noEscalerasAscPermitidasString;
        private string noEscalerasDescPermitidasString;
        private string noEscalerasTotalesPermitidasString;
        private string[] agrupacionesSolapadasPermitidasString;
        private string[] agrupacionesPasoFijoPermitidasString;
        private string columnasImplicadasString;
        private string conceptoString;

        private int[,] agrupacionesPasoFijo;
        private int[,] agrupacionesSolapadas;
        private List<EscaleraAciertos> escaleras;
        private int[] serieAciertos;
        private List<SandwichAciertos> sandwichs;
        private List<ColumnaProbable> columnas; 
        #endregion

        public RelacionCP3(List<ColumnaProbable> cps)
        {
            columnas = cps;
            ObtenerSerieAciertos();
        }
        public RelacionCP3(int[] serie)
        {
            serieAciertos = serie;
        }
        public RelacionCP3()
        {

        }
        #region Métodos de Análisis de Cálculo
        public bool Analiza()
        {
            if(AnalizaEscaleras())
            {
                if(AnalizaSandwichs())
                {
                    if(AnalizaAgrupacionesPasoFijo())
                    {
                        return AnalizaAgrupacionesSolapadas();
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        private bool AnalizaAgrupacionesPasoFijo()
        {
            bool esValida = true;
            if (AgrupacionesPasoFijoPermitidas != null)
            {
                for (int i = 0; i < AgrupacionesPasoFijoPermitidas.Count; i++)
                {
                    AgrupacionColumnas agrup = AgrupacionesPasoFijoPermitidas[i];
                    int noAgrup = 0;
                    if (AgrupacionesPasoFijo.GetLength(0) > agrup.NoElementos)
                    {
                        for (int j = 0; j < agrup.NoAciertos.Count; j++)
                        {
                            noAgrup += AgrupacionesPasoFijo[agrup.NoElementos, agrup.NoAciertos[j]];
                        }
                    }
                    esValida = agrup.Numero.Contains(noAgrup);
                    if (!esValida)
                    {
                        break;
                    }
                }
            }
            return esValida;
        }
        private bool AnalizaAgrupacionesSolapadas()
        {
            bool esValida = true;
            if (AgrupacionesSolapadasPermitidas != null)
            {
                for (int i = 0; i < AgrupacionesSolapadasPermitidas.Count; i++)
                {
                    AgrupacionColumnas agrup = AgrupacionesSolapadasPermitidas[i];
                    int noAgrup = 0;
                    if (AgrupacionesSolapadas.GetLength(0) > agrup.NoElementos)
                    {
                        for (int j = 0; j < agrup.NoAciertos.Count; j++)
                        {
                            noAgrup += AgrupacionesSolapadas[agrup.NoElementos, agrup.NoAciertos[j]];
                        }
                    }
                    if(!agrup.Numero.Contains(noAgrup))
                    {
                        break;
                    }
                }                
            }
            return esValida;
        }

        private bool AnalizaEscaleras()
        {
            if (NumeroEscalerasASCPermitidas != null)
            {
                if (!noEscalerasAscPermitidas[NumeroDeEscalerasAscendentes])
                {
                    return false;
                }
            }

            if (NumeroEscalerasDESCPermitidas != null)
            {
                if (!noEscalerasDescPermitidas[NumeroDeEscalerasDescendentes])
                {
                    return false;
                }
            }

            if (NumeroEscalerasTotalesPermitidas != null)
            {
                if (!noEscalerasTotalesPermitidas[NumeroDeEscaleras])
                {
                    return false;
                }
            }
            return true;

        }
        private bool AnalizaSandwichs()
        {
            if (NumeroSandwichsPermitidos != null)
            {
                return noSandwichsPermitidos[Sandwichs.Count];
            }
            return true;
        }
        #endregion

        public string Analiza(int numRel)
        {
            string txt = "";
            if (AnalizaEscaleras())
            {
                if (AnalizaSandwichs())
                {
                    if (AnalizaAgrupacionesPasoFijo())
                    {
                        if (!AnalizaAgrupacionesSolapadas())
                        {
                            txt += "Fallo en Agrupaciones Solapadas#";
                        }
                    }
                    else
                    {
                        txt += "Fallo en Agrupaciones Paso Fijo#";
                    }
                }
                else
                {
                    txt += "Fallo en Sandwichs (" + Sandwichs.Count + ")#";
                }
            }
            else
            {
                txt += "Fallo en Escaleras (" + NumeroDeEscaleras + ")#";
            }
            return txt;
        }

        public void ActualizaValores()
        {
            serieAciertos = null;
            ObtenerSerieAciertos();
            agrupacionesPasoFijo = null;
            agrupacionesSolapadas = null;
            escaleras = null;
            sandwichs = null;
        }
        

        #region Métodos Privados
        private int ObtenAciertosMinimos()
        {
            int minimo = 0;
            if (SerieAciertos.Length > 0)
            {
                minimo = SerieAciertos[0];
                for (int i = 0; i < SerieAciertos.Length; i++)
                {
                    int valor = SerieAciertos[i];
                    if (valor < minimo)
                    {
                        minimo = valor;
                    }
                }
            }
            return minimo;
        }
        private int[] ObtenerSerieAciertos()
        {
            if (serieAciertos == null)
            {
                serieAciertos = new int[columnas.Count];
                switch (Concepto)
                {
                    case "AC":
                        for (int i = 0; i < columnas.Count; i++)
                        {
                            serieAciertos[i] =columnas[i].NoAC;
                        }
                        break;
                    case "ACS":
                        for (int i = 0; i < columnas.Count; i++)
                        {
                            serieAciertos[i] = columnas[i].NoACS;
                        }
                        break;
                    case "FS":
                        for (int i = 0; i < columnas.Count; i++)
                        {
                            serieAciertos[i] = columnas[i].NoFS;
                        }
                        break;
                }
        }
            return serieAciertos;
        }
        private int ObtenAciertosMaximos()
        {
            int maximo = 0;
            if (SerieAciertos.Length > 0)
            {
                maximo = SerieAciertos[0];
                for (int i = 0; i < SerieAciertos.Length; i++)
                {
                    int valor = SerieAciertos[i];
                    if (valor > maximo)
                    {
                        maximo = valor;
                    }
                }
            }
            return maximo;
        }
        private int ObtenSumaTotalDeAciertos()
        {
            int suma = 0;
            if (SerieAciertos.Length > 0)
            {
                for (int i = 0; i < SerieAciertos.Length; i++)
                {
                    suma += SerieAciertos[i];
                }
            }
            return suma;
        }
        private int[,] ObtenerAgrupaciones()
        {
            //Vamos a crear un int[] que almacene las apariciones de cada agrupación
            //Lo máximo que puede aparecer es una agrupación de la longitud de valores
            if (agrupacionesPasoFijo == null)
            {
                agrupacionesPasoFijo = new int[1, 15];
                int agrupaciones = 1;
                if (SerieAciertos.Length > 0)
                {
                    int actual = SerieAciertos[0];

                    for (int i = 1; i < SerieAciertos.Length; i++)
                    {
                        int numero = SerieAciertos[i];
                        if (numero == actual)
                        {
                            agrupaciones++;
                        }
                        else
                        {
                            if (agrupaciones >= agrupacionesPasoFijo.GetLength(0))
                            {
                                int[,] copia = new int[agrupaciones + 1,15];
                                for(int j = 0; j < agrupacionesPasoFijo.GetLength(0);j++)
                                {
                                    for(int k = 0; k < agrupacionesPasoFijo.GetLength(1); k++)
                                    {
                                        copia[j,k] = agrupacionesPasoFijo[j,k];
                                    }
                                }
                                agrupacionesPasoFijo = copia;
                            }
                            agrupacionesPasoFijo[agrupaciones, actual]++;
                            agrupaciones = 1;

                        }
                        actual = numero;
                    }
                    if (agrupaciones >= agrupacionesPasoFijo.GetLength(0))
                    {
                        int[,] copia = new int[agrupaciones + 1, 15];
                        for (int j = 0; j < agrupacionesPasoFijo.GetLength(0); j++)
                        {
                            for (int k = 0; k < agrupacionesPasoFijo.GetLength(1); k++)
                            {
                                copia[j, k] = agrupacionesPasoFijo[j, k];
                            }
                        }
                        agrupacionesPasoFijo = copia;
                        
                    }
                    agrupacionesPasoFijo[agrupaciones, actual]++;
                }
            }

            return agrupacionesPasoFijo;
        }
        private int[,] ObtenerAgrupacionesSolapadas()
        {
            if (agrupacionesSolapadas == null)
            {
                agrupacionesSolapadas = (int[,])AgrupacionesPasoFijo.Clone();
                //Vamos a crear un int[] que almacene las apariciones de cada agrupación
                //Lo máximo que puede aparecer es una agrupación de la longitud de valores

                if (agrupacionesPasoFijo.Length > 0)
                {
                    int suma = 1;

                    for (int i = 0; i < agrupacionesPasoFijo.GetLength(0); i++)
                    {
                        //i es el concepto
                        for (int j = 0; j < agrupacionesPasoFijo.GetLength(1); j++)
                        {
                            // son los aciertos
                            if (agrupacionesPasoFijo[i, j] > 0)
                            {
                                for (int k = i - 1; k > 1; k--)
                                {
                                    suma++;
                                    agrupacionesSolapadas[k, j] += suma;

                                }
                                suma = 1;
                            }
                        }
                    }
                }
            }
            return agrupacionesSolapadas;
        }
        private List<EscaleraAciertos> ObtenerEscaleras()
        {
            if (escaleras == null)
            {
                List<int> Temporales = new List<int>();
                escaleras = new List<EscaleraAciertos>();
                int minimoEscalones = 3;
                
                if (SerieAciertos.Length > 0)
                {
                    int escalon = serieAciertos[0];
                    Temporales.Add(escalon);
                    for (int i = 1; i < serieAciertos.Length; i++)
                    {
                        if ((Math.Abs(serieAciertos[i] - escalon) == 1) && (!Temporales.Contains(serieAciertos[i])))
                        {
                            Temporales.Add(serieAciertos[i]);
                        }
                        else
                        {

                            if (Temporales.Count < minimoEscalones)
                            {
                                Temporales = new List<int>();
                            }
                            else
                            {
                                //Guardar escalera
                                EscaleraAciertos esc = new EscaleraAciertos();
                                esc.Escalones = Temporales;

                                escaleras.Add(esc);
                                Temporales = new List<int>();
                            }
                            Temporales.Add(serieAciertos[i]);
                        }
                        escalon = serieAciertos[i];
                    }
                }
                if (Temporales.Count >= minimoEscalones)
                {
                    EscaleraAciertos esc = new EscaleraAciertos();
                    esc.Escalones = Temporales;
                    escaleras.Add(esc);
                }
            }
            return escaleras;
        }
        private List<SandwichAciertos> ObtenerSandwichs()
        {
            if (sandwichs == null)
            {
                //Un sandwich tiene 4 capas...
                int capa = 1;
                List<int> Temporales = new List<int>();
                sandwichs = new List<SandwichAciertos>();

                if (SerieAciertos.Length > 0)
                {
                    for (int i = 0; i < SerieAciertos.Length; i++)
                    {
                        switch (capa)
                        {
                            case 1:
                            
                                Temporales.Add(SerieAciertos[i]);
                                capa++;
                                break;
                            case 2:
                                //Debe ser distinto al anterior
                                if (SerieAciertos[i] != Temporales[0])
                                {
                                    Temporales.Add(SerieAciertos[i]);
                                    capa++;
                                }
                                else
                                {
                                    //Deja este y borra el anterior
                                    Temporales.RemoveAt(0);
                                    Temporales.Add(SerieAciertos[i]);
                                }
                                break;
                            case 3:
                                if (SerieAciertos[i] == Temporales[1])
                                {
                                    Temporales.Add(SerieAciertos[i]);
                                    capa++;
                                }
                                else
                                {
                                    //No vale
                                    Temporales = new List<int>();
                                    Temporales.Add(SerieAciertos[i-1]);
                                    Temporales.Add(serieAciertos[i]);
                                }
                                break;
                            case 4:
                                if (SerieAciertos[i] == Temporales[0])
                                {
                                    Temporales.Add(SerieAciertos[i]);
                                    capa++;
                                }
                                else
                                {
                                    //No vale
                                    Temporales = new List<int>();
                                    Temporales.Add(SerieAciertos[i - 1]);
                                    Temporales.Add(SerieAciertos[i]);
                                    capa = 3;
                                }
                                break;
                            default:
                                //Aquí hacemos la comprobación
                                if (Temporales.Count == 4)
                                {
                                    SandwichAciertos sand = new SandwichAciertos();
                                    sand.Sandwich = Temporales;
                                    sandwichs.Add(sand);

                                }
                                Temporales = new List<int>();
                                capa = 1;
                                break;
                        }
                    }

                    if (Temporales.Count == 4)
                    {
                        SandwichAciertos sand = new SandwichAciertos();
                        sand.Sandwich = Temporales;
                        sandwichs.Add(sand);

                    }

                }
            }
            return sandwichs;
        }
        private int ObtenerNumeroEscalerasAscendentes()
        {
            int numAsc = 0;
            if (Escaleras.Count > 0)
            {
                for (int i = 0; i < Escaleras.Count; i++)
                {
                    if (Escaleras[i].Orientacion == OrientacionEscalera.Ascendente)
                    {
                        numAsc++;
                    }
                }

            }
            return numAsc;
        } 
        #endregion

        #region Propiedades Públicas
        public int[,] AgrupacionesPasoFijo
        {
            get { return ObtenerAgrupaciones(); }
        }
        public int[,] AgrupacionesSolapadas
        {
            get { return ObtenerAgrupacionesSolapadas(); }
        }
        public int[] SerieAciertos
        {
            //De una columna con las ganadoras
            get { return ObtenerSerieAciertos(); }
            set { serieAciertos = value; }
        }

        public int MinimoAciertos
        {
            get { return ObtenAciertosMinimos(); }
        }
        public int MaximoAciertos
        {
            get { return ObtenAciertosMaximos(); }
        }
        public int SumaTotalDeAciertos
        {
            get
            {
                return ObtenSumaTotalDeAciertos();
            }
        }
        public List<EscaleraAciertos> Escaleras
        {
            get { return ObtenerEscaleras(); }
        }
        public List<SandwichAciertos> Sandwichs
        {
            get { return ObtenerSandwichs(); }
        }
        public int NumeroDeEscaleras
        {
            get { return Escaleras.Count; }
        }
        public int NumeroDeEscalerasAscendentes
        {
            get { return ObtenerNumeroEscalerasAscendentes(); }
        }
        public int NumeroDeEscalerasDescendentes
        {
            get { return NumeroDeEscaleras - NumeroDeEscalerasAscendentes; }
        }
        public bool[] NumeroEscalerasASCPermitidas
        {
            get { return noEscalerasAscPermitidas; }
            set { noEscalerasAscPermitidas = value; }
        }
        public bool[] NumeroEscalerasDESCPermitidas
        {
            get { return noEscalerasDescPermitidas; }
            set { noEscalerasDescPermitidas = value; }
        }
        public bool[] NumeroEscalerasTotalesPermitidas
        {
            get { return noEscalerasTotalesPermitidas; }
            set { noEscalerasTotalesPermitidas = value; }
        }
        public bool[] NumeroSandwichsPermitidos
        {
            get { return noSandwichsPermitidos; }
            set { noSandwichsPermitidos = value; }
        }
        public List<AgrupacionColumnas> AgrupacionesPasoFijoPermitidas
        {
            get { return agrupacionesPasoFijoPermitidas; }
            set { agrupacionesPasoFijoPermitidas = value; }
        }
        public List<AgrupacionColumnas> AgrupacionesSolapadasPermitidas
        {
            get { return agrupacionesSolapadasPermitidas; }
            set { agrupacionesSolapadasPermitidas = value; }
        }
        public string Concepto
        {
            get { return concepto; }
            set { concepto = value; }
        }
        public List<ColumnaProbable> Columnas
        {
            get { return columnas; }
            set { columnas = value; }
        }
        #endregion

        #region Textos
        //Textos
        public string NumeroEscalerasASCPermitidasString
        {
            get { return noEscalerasAscPermitidasString; }
            set { noEscalerasAscPermitidasString = value; }
        }
        public string NumeroEscalerasDESCPermitidasString
        {
            get { return noEscalerasDescPermitidasString; }
            set { noEscalerasDescPermitidasString = value; }
        }
        public string NumeroEscalerasTotalesPermitidasString
        {
            get { return noEscalerasTotalesPermitidasString; }
            set { noEscalerasTotalesPermitidasString = value; }
        }
        public string NumeroSandwichsPermitidosString
        {
            get { return noSandwichsPermitidosString; }
            set { noSandwichsPermitidosString = value; }
        }
        public string[] AgrupacionesPasoFijoPermitidasString
        {
            get { return agrupacionesPasoFijoPermitidasString; }
            set { agrupacionesPasoFijoPermitidasString = value; }
        }
        public string[] AgrupacionesSolapadasPermitidasString
        {
            get { return agrupacionesSolapadasPermitidasString; }
            set { agrupacionesSolapadasPermitidasString = value; }
        }
        public string ColumnasImplicadasString
        {
            get { return columnasImplicadasString; }
            set { columnasImplicadasString = value; }
        }
        public string ConceptoString
        {
            get { return conceptoString; }
            set { conceptoString = value; }
        } 
        #endregion

    }
}
