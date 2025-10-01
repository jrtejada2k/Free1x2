using System;
using System.Collections.Generic;
using System.Xml;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
    public class FDiferenciasData : FiltroDatosBase
    {
        private FiltroDiferencias _filtro;

        public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
        {
            _filtro = (FiltroDiferencias)filtro;
            _filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
            _filtro.ContieneDatos = true;

            List<Diferencia> arrayRepet = new List<Diferencia>();

            for (int i = 0; i < xmlCondicionDatos.ChildNodes.Count; i++)
            {
                Diferencia rep = new Diferencia();
                rep.PartidosSimetricos = new List<bool[]>();
                XmlNode xmlSubCondicion1 = xmlCondicionDatos.ChildNodes[i];
                for (int j = 0; j < xmlSubCondicion1.ChildNodes.Count; j++)
                {
                    XmlNode xmlSubCond2 = xmlSubCondicion1.ChildNodes[j];
                    switch (xmlSubCond2.Name)
                    {
                        case "aciertos":
                            rep.AcV = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(xmlSubCond2.Attributes["aciertosV"].Value, rep.PartidosSimetricos.Count + 1);
                            rep.AcX = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(xmlSubCond2.Attributes["aciertosX"].Value, rep.PartidosSimetricos.Count + 1);
                            rep.AcDoses = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(xmlSubCond2.Attributes["aciertos2"].Value, rep.PartidosSimetricos.Count + 1);
                            rep.AcDib = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(xmlSubCond2.Attributes["aciertosDib"].Value, rep.PartidosSimetricos.Count + 1);
                            rep.AcInt = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(xmlSubCond2.Attributes["aciertosInt"].Value, rep.PartidosSimetricos.Count + 1);
                            rep.AcFormatos = Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(xmlSubCond2.Attributes["aciertosFormatos"].Value, rep.PartidosSimetricos.Count + 1);

                            if (rep.AcV == null && rep.AcX == null && rep.AcDoses == null && rep.AcDib == null)
                            {
                                rep.AnalizaVX2Dib = false;
                            }
                            else
                            {
                                rep.AnalizaVX2Dib = true;
                            }
                            if (rep.AcInt == null)
                            {
                                rep.AcInt = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                                rep.AnalizaInterrupciones = false;
                            }
                            else
                            {
                                rep.AnalizaInterrupciones = true;
                            }

                            if (rep.AcFormatos == null)
                            {
                                rep.AcFormatos = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                                rep.AnalizaFormatos = false;
                            }
                            else
                            {
                                rep.AnalizaFormatos = true;
                            }
                            
                            if (rep.AcV == null)
                            {
                                rep.AcV = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                            }
                            if (rep.AcX == null)
                            {
                                rep.AcX = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                            }
                            if (rep.AcDoses == null)
                            {
                                rep.AcDoses = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                            }
                            if (rep.AcDib == null)
                            {
                                rep.AcDib = ActivarTodos(rep.PartidosSimetricos.Count + 1);
                            }

                            break;
                        case "partidos":
                          rep.AñadirPartidosSimetricos(Utils.UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(xmlSubCond2.Attributes["valores"].Value));
                            break;
                    }
                }


                


                arrayRepet.Add(rep);
            }
            _filtro.Diferencias = arrayRepet;
        }
        private bool[] ActivarTodos(int longitud)
        {
            bool[] array = new bool[longitud];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = true;
            }
            return array;
        }


        public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
        {
            _filtro = (FiltroDiferencias)filtro;

            if (_filtro.ContieneDatos)
            {
                xmlWriter.WriteStartElement("condicion");
                xmlWriter.WriteAttributeString("id", _filtro.NombreFiltro.ToString());
                xmlWriter.WriteAttributeString("activa", _filtro.IsActive.ToString());

                EscribirValoresStandardFiltro(xmlWriter);

                xmlWriter.WriteEndElement();
            }
        }
        protected void EscribirValoresStandardFiltro(XmlTextWriter xmlWriter)
        {
            for (int i = 0; i < _filtro.Diferencias.Count; i++)
            {
                Diferencia sim = _filtro.Diferencias[i];
                xmlWriter.WriteStartElement("grupoDiferencias");
                xmlWriter.WriteAttributeString("id", i.ToString());

                for (int j = 0; j < sim.PartidosSimetricos.Count; j++)
                {
                    bool[] partidos = sim.PartidosSimetricos[j];
                    xmlWriter.WriteStartElement("partidos");
                    xmlWriter.WriteAttributeString("id", j.ToString());

                    xmlWriter.WriteAttributeString("valores", Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(partidos));

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteStartElement("aciertos");
                if (sim.AnalizaVX2Dib)
                {
                    xmlWriter.WriteAttributeString("aciertosV", Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(sim.AcV));
                    xmlWriter.WriteAttributeString("aciertosX", Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(sim.AcX));
                    xmlWriter.WriteAttributeString("aciertos2", Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(sim.AcDoses));
                    xmlWriter.WriteAttributeString("aciertosDib", Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(sim.AcDib));
                }
                else
                {
                    xmlWriter.WriteAttributeString("aciertosV", "");
                    xmlWriter.WriteAttributeString("aciertosX", "");
                    xmlWriter.WriteAttributeString("aciertos2", "");
                    xmlWriter.WriteAttributeString("aciertosDib", "");
                }
                if (sim.AnalizaInterrupciones)
                {
                    xmlWriter.WriteAttributeString("aciertosInt", Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(sim.AcInt));
                }
                else
                {
                    xmlWriter.WriteAttributeString("aciertosInt", "");

                }

                if (sim.AnalizaFormatos)
                {
                    xmlWriter.WriteAttributeString("aciertosFormatos", Utils.UtilidadesEntradasValores.ObtenerTextoFromBool(sim.AcFormatos));
                }
                else
                {
                    xmlWriter.WriteAttributeString("aciertosFormatos", "");

                }
                xmlWriter.WriteEndElement();


                xmlWriter.WriteEndElement();
            }
        }
        protected List<int> ObtenerArrayAciertos(string aciertos)
        {
            List<int> arrayA = new List<int>();
            string[] aciertosTemp = aciertos.Split(',');
            for (int i = 0; i < aciertosTemp.Length; i++)
            {
                int a = Convert.ToInt32(aciertosTemp[i]);
                arrayA.Add(a);
            }
            return arrayA;
        }
        protected string ObtenerStringAciertos(bool[] aciertos)
        {
            string salida = "";
            for (int i = 0; i < aciertos.Length; i++)
            {
                if (aciertos[i])
                {
                    salida += i.ToString();
                    if (i < aciertos.Length - 1)
                    {
                        salida += ",";
                    }
                }
            }
            return salida.Substring(0, salida.Length - 1);
        }
    }
}
