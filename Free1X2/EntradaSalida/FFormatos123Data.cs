using System;
using System.Xml;
using System.Collections.Generic;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	/// <summary>
	/// Summary description for FFormatos123Data.
	/// </summary>
	public class FFormatos123Data : FiltroDatosBase
	{
		private FiltroFormatos123 _filtro;


		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroFormatos123)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;

            List<Formato123> arrayFormatos = new List<Formato123>();
		    double[,] valoracion = new double[14,3];

		    for( int i = 0; i < xmlCondicionDatos.ChildNodes.Count; i++)
			{
				XmlNode xmlSubCondicion1 = xmlCondicionDatos.ChildNodes[i];
				
				string aciertosTmp=xmlSubCondicion1.Attributes["aciertos"].Value;
				if(aciertosTmp.Length==0) aciertosTmp="0";
                _filtro.AciertosFiltro = aciertosTmp;
                _filtro.PasoFijo = Convert.ToBoolean(xmlSubCondicion1.Attributes["repeticiones"].Value);

				string[] valores = xmlSubCondicion1.Attributes["valoracion"].Value.Split('/');
				
				int partido=0;
				int signo = 0;
				for(int l=0; l<valores.Length;l++)
				{
					valoracion[partido,signo] = Convert.ToDouble(valores[l]);
					signo++;
					if(signo == 3)
					{
						signo = 0;
						partido++;
					}
				}

			    for( int j = 0; j < xmlSubCondicion1.ChildNodes.Count; j++)
				{
					XmlNode xmlSubCond2 = xmlSubCondicion1.ChildNodes[j];

					Formato123 formato = new Formato123();
					formato.Formato = xmlSubCond2.Attributes["formato"].Value;
					formato.AciertosMin = Convert.ToInt32(xmlSubCond2.Attributes["aciertosMin"].Value);
					formato.AciertosMax = Convert.ToInt32(xmlSubCond2.Attributes["aciertosMax"].Value);

					arrayFormatos.Add(formato);
				}
			}		

			_filtro.ArrayFormatos = arrayFormatos;
            _filtro.ArrayAciertos = ObtenerArrayAciertos(_filtro.AciertosFiltro);
			_filtro.Valoracion = valoracion;
            _filtro.ValoracionTranformada = TransformarValoracion(_filtro.Valores);
		}
        protected byte[,] TransformarValoracion(double[,] valoracion)
        {
            byte[,] valoresTransformados = new byte[14, 3];
            for (int i = 0; i < 14; i++)
            {
                double[] valor = { valoracion[i, 0], valoracion[i, 1], valoracion[i, 2] };
                if ((valor[0] >= valor[1]) && (valor[0] >= valor[2]))
                {
                    if (valor[1] >= valor[2])
                    {
                        valoresTransformados[i, 0] = 4; //"1";
                        valoresTransformados[i, 1] = 2; //"2";
                        valoresTransformados[i, 2] = 1; //"3";
                    }
                    else if (valor[2] > valor[1])
                    {
                        valoresTransformados[i, 0] = 4; //"1";
                        valoresTransformados[i, 1] = 1; //"3";
                        valoresTransformados[i, 2] = 2; //"2";
                    }
                }
                else if ((valor[1] > valor[0]) && (valor[1] >= valor[2]))
                {
                    if (valor[0] >= valor[2])
                    {
                        valoresTransformados[i, 0] = 2; // "2";
                        valoresTransformados[i, 1] = 4; // "1";
                        valoresTransformados[i, 2] = 1; // "3";
                    }
                    else
                    {
                        valoresTransformados[i, 0] = 1; // "3";
                        valoresTransformados[i, 1] = 4; // "1";
                        valoresTransformados[i, 2] = 2; // "2";
                    }
                }
                else if ((valor[2] > valor[0]) && (valor[2] > valor[1]))
                {
                    if (valor[0] >= valor[1])
                    {
                        valoresTransformados[i, 0] = 2; // "2";
                        valoresTransformados[i, 1] = 1; // "3";
                        valoresTransformados[i, 2] = 4; // "1";

                    }
                    else
                    {
                        valoresTransformados[i, 0] = 1; //"3";
                        valoresTransformados[i, 1] = 2; //"2";
                        valoresTransformados[i, 2] = 4; //"1";
                    }
                }
            } return valoresTransformados;
        }


		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{			
			_filtro = (FiltroFormatos123)filtro;
			
			if( _filtro.ContieneDatos )
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
			for(int i = 0; i < 1; i++) //Cuando se habiliten más grupos activar
			{

				xmlWriter.WriteStartElement("grupoformato");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("aciertos", ObtenerStringAciertos(_filtro.ArrayAciertos));
                xmlWriter.WriteAttributeString("repeticiones", _filtro.PasoFijo.ToString());
                xmlWriter.WriteAttributeString("valoracion", ObtenerStringPorcentajes(_filtro.Valoracion));
				for(int j = 0; j < _filtro.ArrayFormatos.Count; j++)
				{
					Formato123 formato = _filtro.ArrayFormatos[j];
					xmlWriter.WriteStartElement("formatos");
					xmlWriter.WriteAttributeString("id", j.ToString());

					xmlWriter.WriteAttributeString("formato", formato.Formato);
					xmlWriter.WriteAttributeString("aciertosMin", formato.AciertosMin.ToString());
					xmlWriter.WriteAttributeString("aciertosMax", formato.AciertosMax.ToString());

					xmlWriter.WriteEndElement();
				}
				
				xmlWriter.WriteEndElement();
			}				
		}


        protected string ObtenerStringAciertos(List<int> aciertos)
		{
			string salida = "";
			for(int i = 0; i < aciertos.Count; i++)
			{
				salida += aciertos[i].ToString();
				if(i < aciertos.Count - 1)
				{
					salida += ",";
				}
			}
			return salida;
		}
		protected string ObtenerStringPorcentajes(double[,] porcentajes)
		{
			string salida = "";
			for(int i = 0; i < 14; i++)
			{
				for(int j=0; j < 3; j++)
				{
					salida += porcentajes[i,j].ToString();
					if((i < 13)||(j<2))
					{
						salida += "/";
					}
				}
			}
			return salida;
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


	}
}
