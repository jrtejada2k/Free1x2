using System;
using System.Xml;
using System.Collections.Generic;

using Free1X2.MotorCalculo;

namespace Free1X2.EntradaSalida
{
	/// <summary>
	/// Summary description for FSimetriasData.
	/// </summary>
	public class FSimetriasData : FiltroDatosBase
	{
		private FiltroSimetrias _filtro;
		public override void PonerCondicionesFiltro(IFiltro filtro, XmlNode xmlCondicionDatos)
		{
			_filtro = (FiltroSimetrias)filtro;
			_filtro.IsActive = Convert.ToBoolean(xmlCondicionDatos.Attributes["activa"].Value);
			_filtro.ContieneDatos = true;

			List<Simetria> arraySimetrias = new List<Simetria>();

		    for( int i = 0; i < xmlCondicionDatos.ChildNodes.Count; i++)
			{
				XmlNode xmlSubCondicion1 = xmlCondicionDatos.ChildNodes[i];
				
				string aciertosTmp=xmlSubCondicion1.Attributes["aciertos"].Value;
				if(aciertosTmp.Length==0) aciertosTmp="0";
				
				_filtro.Aciertos = aciertosTmp;


			    for( int j = 0; j < xmlSubCondicion1.ChildNodes.Count; j++)
				{
					XmlNode xmlSubCond2 = xmlSubCondicion1.ChildNodes[j];

					Simetria simetria = new Simetria(xmlSubCond2.Attributes["simetria"].Value);

					arraySimetrias.Add(simetria);
				}
			}		

			_filtro.ArraySimetrias = arraySimetrias;
            _filtro.ArrayAciertos = ObtenerArrayAciertos(_filtro.Aciertos);
		}


		public override void EscribirCondicionesFiltros(IFiltro filtro, XmlTextWriter xmlWriter)
		{			
			_filtro = (FiltroSimetrias)filtro;
			
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

				xmlWriter.WriteStartElement("gruposimetrias");
				xmlWriter.WriteAttributeString("id", i.ToString());
				xmlWriter.WriteAttributeString("aciertos", ObtenerStringAciertos(_filtro.ArrayAciertos));
				for(int j = 0; j < _filtro.ArraySimetrias.Count; j++)
				{
				    xmlWriter.WriteStartElement("simetrias");
					xmlWriter.WriteAttributeString("id", j.ToString());

                    xmlWriter.WriteAttributeString("simetria", _filtro.ArraySimetrias[j].Partidos);

					xmlWriter.WriteEndElement();
				}
				
				xmlWriter.WriteEndElement();
			}				
		}
        protected List<int> ObtenerArrayAciertos(string aciertos)
        {
            List<int> arrayA = new List<int>();
            string[] aciertosTemp = aciertos.Split(',');
            for (int i = 0; i < aciertosTemp.Length; i++)
            {
                arrayA.Add(Convert.ToInt32(aciertosTemp[i]));
            }
            return arrayA;
        }
        protected string ObtenerStringAciertos(List<int> aciertos)
        {
            string salida = "";
            for (int i = 0; i < aciertos.Count; i++)
            {
                salida += aciertos[i].ToString();
                if (i < aciertos.Count - 1)
                {
                    salida += ",";
                }
            }
            return salida;
        }
	}
}
