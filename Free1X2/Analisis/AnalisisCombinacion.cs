using System;
using System.Windows.Forms;
using Free1X2.UI;
using Free1X2.MotorCalculo;

namespace Free1X2.Analisis
{
	/// <summary>
	/// Descripción breve de AnalisisCombinacion.
	/// </summary>
	public class AnalisisCombinacion
	{
	    private bool[] grupos;
		private bool[] conjuntos;
		private bool[] conjuntos2;
		private System.Drawing.Color rojo=System.Drawing.Color.Red;
		private System.Drawing.Color naranja=System.Drawing.Color.DarkOrange;
		private System.Drawing.Color verde=System.Drawing.Color.Green;
		private System.Drawing.Color negro=System.Drawing.Color.Black;

		public void AnalizarCombinacion(string nombreCombinacion, long columna, MotorCalculo.Analizador analizador, string[] pronosticosBase)
		{
			string texto;
		    bool hayFallos=false;
		    Grupo g;
		    TreeNode nodoPrincipal;
			TreeNode nodoSecundario;
			TreeNode nodoSecundarioHijo;
			TreeNode nodeTemp;
		    TreeNodeCollection coleccion;
			TreeNodeCollection coleccionSecundaria;
			string[] evaluacionFiltro;
			IFiltro filtro;

			//obten solo nombre archivo sin directorio. ej: filtro.txt
			//nombreCombinacion = nombreCombinacion.Substring( nombreCombinacion.LastIndexOf("\\") + 1 );
            nombreCombinacion = System.IO.Path.GetFileName(nombreCombinacion);
			AnalizarCombinacionFrm f2=new AnalizarCombinacionFrm();
			// imageIndex: 0=fallo, 1=acierto, 2=tolerancia
			f2.Text="Análisis de fallos: "+nombreCombinacion;
			// Se añade el nodo de combinación
			TreeNode nodoBase = new TreeNode("Combinación",1,1);
			f2.treeView1.Nodes.Add(nodoBase);
			TreeNodeCollection coleccionBase = nodoBase.Nodes;

			grupos=new bool[analizador.GruposPartidos.Count];
			for(int i=0;i<analizador.GruposPartidos.Count;i++)
			{
			    g=analizador.GruposPartidos[i];
				texto="Grupo "+i;
				if(i==0)
					texto="Boleto base";
				else
				{
					if(g.NombreGrupo.Length>0)
						texto+=" ("+g.NombreGrupo+")";
				}
                nodoPrincipal = new TreeNode(texto, 1, 1);
				coleccionBase.Add(nodoPrincipal);
				coleccion=nodoPrincipal.Nodes;
				// Si es el boleto base, comprueba el pronóstico.
				if(i==0)
				{
					nodoSecundario=new TreeNode("Pronóstico de partidos");
					nodoSecundario.Checked=true;
					string[] evaluacionPronosticos=evaluarPronosticos(columna, pronosticosBase);
					if( evaluacionPronosticos !=null)
					{
						nodoSecundario.ForeColor=rojo;
						nodoSecundario.ImageIndex=0;
						nodoSecundario.SelectedImageIndex=0;
                        nodoBase.ForeColor = rojo;
                        nodoBase.ImageIndex = 0;
                        nodoBase.SelectedImageIndex = 0;
                        nodoPrincipal.ForeColor = rojo;
                        nodoPrincipal.ImageIndex = 0;
                        nodoPrincipal.SelectedImageIndex = 0;
						coleccion.Add(nodoSecundario);
						coleccionSecundaria=nodoSecundario.Nodes;
						for(int j=0;j<evaluacionPronosticos.Length;j++)
						{
							int n = Convert.ToInt16(evaluacionPronosticos[j]);
							nodoSecundarioHijo=new TreeNode("Partido "+n+ "  ("+pronosticosBase[n-1]+")");
							nodoSecundarioHijo.ForeColor=rojo;
							coleccionSecundaria.Add(nodoSecundarioHijo);
						}
					}
					else
					{
						nodoSecundario.ImageIndex=1;
						nodoSecundario.SelectedImageIndex=1;
						coleccion.Add(nodoSecundario);
					}
				}
				// Si el grupo no contiene todos los partidos, obtiene la columna a analizar
				long columnaAAnalizar = g.ColumnaGrupo(columna);
			    for(int f=0;f<g.Filtros.Count;f++)
				{
					filtro = g.Filtros[f];
					if(filtro.IsActive)
					{
						nodoSecundario=new TreeNode(buscarFiltro(filtro.NombreFiltro),1,1);
						nodoSecundario.Checked=true;
						evaluacionFiltro = filtro.AnalizarFallos(columnaAAnalizar);
						coleccion.Add(nodoSecundario);
						if(evaluacionFiltro!=null)
						{
							nodoSecundario.Checked=true;
							nodoSecundario.ImageIndex=1;
							nodoSecundario.SelectedImageIndex=1;
							nodoPrincipal.ImageIndex=1;
							nodoPrincipal.SelectedImageIndex=1;
							coleccionSecundaria=nodoSecundario.Nodes;
							for(int j=0;j<evaluacionFiltro.Length;j++)
							{
								// Si comienza por "*" NO hay fallo
								if(evaluacionFiltro[j].Substring(0,1)=="*")
								{
									nodoSecundarioHijo=new TreeNode(evaluacionFiltro[j].Substring(1));
									nodoSecundarioHijo.ForeColor=verde;
									nodoSecundarioHijo.ImageIndex=1;
									nodoSecundarioHijo.SelectedImageIndex=1;
									// Si el filtro tiene tolerancias locales y está en naranja, cambia a verde
									if(filtro.NombreFiltro==Filtro.PesosNumericos || filtro.NombreFiltro==Filtro.ColProbables)
									{
										if(nodoSecundario.ForeColor==naranja)
										{
											nodoSecundario.ForeColor=verde;
											nodoSecundario.ImageIndex=1;
											nodoSecundario.SelectedImageIndex=1;
										}
									}
								}
								else if(evaluacionFiltro[j].Substring(0,1)=="!")
								{
									nodoSecundarioHijo=new TreeNode(evaluacionFiltro[j].Substring(1));
									nodoSecundarioHijo.ForeColor=naranja;
									nodoSecundarioHijo.ImageIndex=2;
									nodoSecundarioHijo.SelectedImageIndex=2;
									// Si no hay fallo anterior, cambia el TreeNode padre
									if(nodoSecundario.ForeColor!=rojo)
									{
										nodoSecundario.ForeColor=naranja;
										nodoSecundario.ImageIndex=2;
										nodoSecundario.SelectedImageIndex=2;
									}
								}
								else
								{
									nodoSecundarioHijo=new TreeNode(evaluacionFiltro[j]);
									nodoSecundarioHijo.ForeColor=rojo;
									// Cambia el color e iconos de los nodos padres
									nodoSecundario.ForeColor=rojo;
									nodoSecundario.ImageIndex=0;
									nodoSecundario.SelectedImageIndex=0;
									nodoPrincipal.ForeColor=rojo;
									nodoPrincipal.ImageIndex=0;
									nodoPrincipal.SelectedImageIndex=0;
                                    nodoBase.ForeColor = rojo;
                                    nodoBase.ImageIndex = 0;
                                    nodoBase.SelectedImageIndex = 0;
								}
								coleccionSecundaria.Add(nodoSecundarioHijo);
							}
						}
					}
				}
				if((g.ControladorTolerancias.Tolerancias.Count>0)&&!g.EsGrupoBase)
				{
                    TreeNode nodoTolerancias = new TreeNode("Tolerancias Globales");
                    if (g.AnalizaToleranciasGrupo(columnaAAnalizar))
                    {
                        nodoPrincipal.ForeColor = verde;
                        nodoPrincipal.ImageIndex = 1;
                        nodoPrincipal.SelectedImageIndex = 1;

                        nodoTolerancias.ForeColor = verde;
                        nodoTolerancias.ImageIndex = 1;
                        nodoTolerancias.SelectedImageIndex = 1;
                    }
                    else
                    {
                        nodoPrincipal.ForeColor = rojo;
                        nodoPrincipal.ImageIndex = 0;
                        nodoPrincipal.SelectedImageIndex = 0;

                        nodoTolerancias.ForeColor = rojo;
                        nodoTolerancias.ImageIndex = 0;
                        nodoTolerancias.SelectedImageIndex = 0;
                        nodoTolerancias.Text += " - Fallo en tolerancias Globales";

                        nodoPrincipal.Nodes.Add(nodoTolerancias);

                    }
				}
				if(nodoPrincipal.ForeColor==rojo)
				{
					grupos[i]=false;
					hayFallos=true;
				}
				else
					grupos[i]=true;
			}
			// Si hay fallos en la combinacion, cambia el color e iconos del nodo
			if(hayFallos)
			{
				nodoBase.ForeColor=rojo;
				nodoBase.ImageIndex=0;
				nodoBase.SelectedImageIndex=0;
			}

			// Analiza el control de grupos
			int imagen;
			System.Drawing.Color color;
			conjuntos=new bool[analizador.CtrlGrupos.ControlesGrupos.Count];

			if(conjuntos.Length>1)
			{
				ControlGrupos cg;
				nodoBase=new TreeNode("Control de grupos",1,1);
				f2.treeView1.Nodes.Add(nodoBase);
				coleccionBase=nodoBase.Nodes;
				hayFallos=false;
				for(int i=1;i<conjuntos.Length;i++)
				{
				    cg=analizador.CtrlGrupos.ControlesGrupos[i];
					texto="Control de Grupos "+i;
					if(AnalizaFallosGrupos(cg)==false)
					{
						conjuntos[i]=false;
						hayFallos=true;
						imagen=0;
						color=rojo;
					}
					else
					{
						conjuntos[i]=true;
						imagen=1;
						color=negro;
						// Si el control de errores permite los fallos producidos
						// ponemos en ambar los nodos de los grupos afectados SI
						// estaban en rojo.
						for(int j=0;j<cg.GruposControlados.Length;j++)
						{
							nodeTemp=f2.treeView1.Nodes[0].Nodes[cg.GruposControlados[j]];
							if(nodeTemp.ForeColor==rojo)
							{
								nodeTemp.ForeColor=naranja;
								nodeTemp.ImageIndex=2;
								nodeTemp.SelectedImageIndex=2;
							}
						}
					}
					nodoPrincipal=new TreeNode(texto,imagen,imagen);
					nodoPrincipal.ForeColor=color;
					coleccionBase.Add(nodoPrincipal);
				}
				if(hayFallos)
				{
					nodoBase.ForeColor=rojo;
					nodoBase.ImageIndex=0;
					nodoBase.SelectedImageIndex=0;
				}
				else
				{
					// Si el boleto base NO está fallado y se aciertan los controles
					// de grupos, se cambia el estilo del nodo de combinación a ambar
					if(grupos[0])
					{
						f2.treeView1.Nodes[0].ForeColor=naranja;
						f2.treeView1.Nodes[0].ImageIndex=2;
						f2.treeView1.Nodes[0].SelectedImageIndex=2;
					}
				}

				// Comprueba el control de conjuntos
				conjuntos2=new bool[analizador.CtrlGrupos.ControlesConjuntos.Count];
				if(conjuntos2.Length>1)
				{
				    nodoBase=new TreeNode("Control de conjuntos",1,1);
					f2.treeView1.Nodes.Add(nodoBase);
					coleccionBase=nodoBase.Nodes;
					hayFallos=false;
					for(int i=1;i<analizador.CtrlGrupos.ControlesConjuntos.Count;i++)
					{
					    ControlConjuntos cc = analizador.CtrlGrupos.ControlesConjuntos[i];
						texto="Control de Conjuntos "+i;
						if(AnalizaFallosConjuntos(cc)==false)
						{
							conjuntos2[i]=false;
							hayFallos=true;
							imagen=0;
							color=rojo;
						}
						else
						{
							conjuntos2[i]=true;
							imagen=1;
							color=negro;
							// Si el control de errores permite los fallos producidos
							// ponemos en ambar los nodos de los grupos afectados SI
							// estaban en rojo.
							int[] conj=cc.ObtenCtrolGruposConjunto();
							for(int j=0;j<conj.Length;j++)
							{
								nodeTemp=f2.treeView1.Nodes[1].Nodes[conj[j]-1];
								if(nodeTemp.ForeColor==rojo)
								{
									nodeTemp.ForeColor=naranja;
									nodeTemp.ImageIndex=2;
									nodeTemp.SelectedImageIndex=2;
								}
							}
						}
						nodoPrincipal=new TreeNode(texto,imagen,imagen);
						nodoPrincipal.ForeColor=color;
						coleccionBase.Add(nodoPrincipal);
					}
					if(hayFallos)
					{
						nodoBase.ForeColor=rojo;
						nodoBase.ImageIndex=0;
						nodoBase.SelectedImageIndex=0;
					}
					else
					{
						// Si el boleto base NO está fallado y se aciertan los controles
						// de grupos, se cambia el estilo del nodo de combinación a ambar
						if(grupos[0])
						{
							f2.treeView1.Nodes[0].ForeColor=naranja;
							f2.treeView1.Nodes[0].ImageIndex=2;
							f2.treeView1.Nodes[0].SelectedImageIndex=2;
						}
						f2.treeView1.Nodes[1].ForeColor=naranja;
						f2.treeView1.Nodes[1].ImageIndex=2;
						f2.treeView1.Nodes[1].SelectedImageIndex=2;
					}
				}
			}

			// Analiza el controlador If-Then
			if(analizador.IfThen!=null)
			{
				if(analizador.IfThen.EsVacio==false && analizador.IfThen.EsActivo)
				{
					nodoBase=new TreeNode("Condiciones relacionadas",1,1);
					f2.treeView1.Nodes.Add(nodoBase);
					coleccionBase=nodoBase.Nodes;
					if(analizador.IfThen.ControlesCondiciones.Count>0)
					{
						nodoPrincipal=new TreeNode("Condiciones sencillas",1,1);
						coleccionBase.Add(nodoPrincipal);
						coleccionSecundaria=nodoPrincipal.Nodes;
						evaluacionFiltro=analizador.IfThen.CompruebaErrores(columna);
						hayFallos=false;
						for(int numErrores=0;numErrores<evaluacionFiltro.Length;numErrores++)
						{
							string car=evaluacionFiltro[numErrores].Substring(0,1);
							switch(car)
							{
								case "*":
									// No hay Error
									color=verde;
									imagen=1;
									texto=evaluacionFiltro[numErrores].Substring(1);
									break;
								case "!":
									// Fallo pendiente de tolerancias. Posible error
									color=naranja;
									imagen=2;
									texto=evaluacionFiltro[numErrores].Substring(1);
									break;
								default:
									// Hay fallo
									hayFallos=true;
									color=rojo;
									imagen=0;
									texto=evaluacionFiltro[numErrores];
									break;
							}
							nodoSecundario=new TreeNode(texto,imagen,imagen);
							nodoSecundario.ForeColor=color;
							coleccionSecundaria.Add(nodoSecundario);
						}
						if(hayFallos)
						{
							nodoPrincipal.ForeColor=rojo;
							nodoPrincipal.ImageIndex=0;
							nodoPrincipal.SelectedImageIndex=0;
							nodoBase.ForeColor=rojo;
							nodoBase.ImageIndex=0;
							nodoBase.SelectedImageIndex=0;
						}
					}
					if(analizador.IfThen.ControlesGrupos.Count>0)
					{
						nodoPrincipal=new TreeNode("Grupos relacionados",1,1);
						coleccionBase.Add(nodoPrincipal);
						coleccionSecundaria=nodoPrincipal.Nodes;
						evaluacionFiltro=analizador.IfThen.CompruebaErrores(columna, analizador.GruposPartidos);
						for(int numErrores=0;numErrores<evaluacionFiltro.Length;numErrores++)
						{
							string car=evaluacionFiltro[numErrores].Substring(0,1);
							switch(car)
							{
								case "*":
									// No hay Error
									color=verde;
									imagen=1;
									texto=evaluacionFiltro[numErrores].Substring(1);
									break;
								case "!":
									// Fallo pendiente de tolerancias. Posible error
									color=naranja;
									imagen=2;
									texto=evaluacionFiltro[numErrores].Substring(1);
									break;
								default:
									// Hay fallo
									hayFallos=true;
									color=rojo;
									imagen=0;
									texto=evaluacionFiltro[numErrores];
									break;
							}
							nodoSecundario=new TreeNode(texto,imagen,imagen);
							nodoSecundario.ForeColor=color;
							coleccionSecundaria.Add(nodoSecundario);
						}
						if(hayFallos)
						{
							nodoPrincipal.ForeColor=rojo;
							nodoPrincipal.ImageIndex=0;
							nodoPrincipal.SelectedImageIndex=0;
							nodoBase.ForeColor=rojo;
							nodoBase.ImageIndex=0;
							nodoBase.SelectedImageIndex=0;
						}
					}
				}
			}
			f2.treeView1.SelectedNode =f2.treeView1.Nodes[0];
			f2.StartPosition=FormStartPosition.CenterScreen;
			f2.ShowDialog();
		}

        public bool AnalizaColumna(long columna, MotorCalculo.Analizador analizador, string[] pronosticosBase)
        {
            string[] evaluacionFiltro;
            IFiltro filtro;
            grupos = new bool[analizador.GruposPartidos.Count];

            for (int i = 0; i < analizador.GruposPartidos.Count; i++)
            {
                Grupo g = analizador.GruposPartidos[i];
                // Si es el boleto base, comprueba el pronóstico.
                if (i == 0)
                {
                    string[] evaluacionPronosticos = evaluarPronosticos(columna, pronosticosBase);
                    if (evaluacionPronosticos != null) return false;
                }
                // Si el grupo no contiene todos los partidos, obtiene la columna a analizar
                long columnaAAnalizar = g.ColumnaGrupo(columna);
                for (int f = 0; f < g.Filtros.Count; f++)
                {
                    filtro = g.Filtros[f];
                    if (filtro.IsActive)
                    {
                        if (!filtro.Analizar(columnaAAnalizar)) return false;
                    }
                }
                if (g.ControladorTolerancias.Tolerancias.Count > 0)
                {
                    if (!g.AnalizaToleranciasGrupo(columnaAAnalizar)) return false;
                }
            }
            // Analiza el control de grupos
            conjuntos = new bool[analizador.CtrlGrupos.ControlesGrupos.Count];

            if (conjuntos.Length > 1)
            {
                for (int i = 1; i < conjuntos.Length; i++)
                {
                    ControlGrupos cg = analizador.CtrlGrupos.ControlesGrupos[i];
                    if (!AnalizaFallosGrupos(cg)) return false;
                }

                // Comprueba el control de conjuntos
                conjuntos2 = new bool[analizador.CtrlGrupos.ControlesConjuntos.Count];
                if (conjuntos2.Length > 1)
                {
                    for (int i = 1; i < analizador.CtrlGrupos.ControlesConjuntos.Count; i++)
                    {
                        ControlConjuntos cc = analizador.CtrlGrupos.ControlesConjuntos[i];
                        if (!AnalizaFallosConjuntos(cc)) return false;
                    }
                }
            }

            // Analiza el controlador If-Then
            if (analizador.IfThen != null)
            {
                if (!analizador.IfThen.EsVacio && analizador.IfThen.EsActivo)
                {
                    if (analizador.IfThen.ControlesCondiciones.Count > 0)
                    {
                        evaluacionFiltro = analizador.IfThen.CompruebaErrores(columna);
                        if (evaluacionFiltro.Length > 0) return false;
                    }
                    if (analizador.IfThen.ControlesGrupos.Count > 0)
                    {
                        evaluacionFiltro = analizador.IfThen.CompruebaErrores(columna, analizador.GruposPartidos);
                        if (evaluacionFiltro.Length > 0) return false;
                    }
                }
            }

            return true;
        }



		private string[] evaluarPronosticos(long cg, string[] pronosticos)
		{
			string[] arrayFallos=null;
            int partido = 0;
            string fallos = "";
            while (cg != 0)
            {
                int signo = (int)cg & 7;
                int pronostico = Utils.UtilColumnas.ConvPartidoStrToByte(pronosticos[partido]);
                if ((signo & pronostico) != signo)
                {
                    fallos = "#" + Convert.ToString(partido + 1) + fallos;
                }
                cg >>= 3;
                partido++;
            }
		    if (fallos.Length >= 2)
		    {
		        string fallosDef = fallos.Substring(1, fallos.Length - 1);
		        arrayFallos = fallosDef.Split('#');
		    }
		    return arrayFallos;
		}

		private string buscarFiltro (Filtro nombreFiltro)
		{
			string nombre;
			switch(nombreFiltro)
			{
				case Filtro.ColProbables:
					nombre="Columnas probables";
					break;
				case Filtro.FormatosSignos:
					nombre="Formatos de Signos";
					break;
				case Filtro.GruposEquipos:
					nombre="Grupos de Equipos";
					break;
				case Filtro.NoInterrupciones:
					nombre="Interrupciones";
					break;
				case Filtro.NoVariantes:
					nombre="Variantes, X y 2";
					break;
				case Filtro.PesosNumericos:
					nombre="Pesos Numericos";
					break;
				case Filtro.SignosSeguidos:
					nombre="Signos Seguidos";
					break;
				case Filtro.ValoracionSignos:
					nombre="Valoracion de Signos";
					break;
				default:
					nombre=nombreFiltro.ToString();
					break;
			}
			return nombre;
		}

		private bool AnalizaFallosGrupos(ControlGrupos c)
		{
			int numFallos=0;
		    for(int i=0;i<c.GruposControlados.Length ;i++)
		    {
		        int numGrupo = c.GruposControlados[i];
		        if(grupos[numGrupo]==false) numFallos++;
		    }
		    if(c.FallosPermitidos[numFallos]) return true;
			return false;
		}

		private bool AnalizaFallosConjuntos(ControlConjuntos c)
		{
			int numFallos=0;
		    bool[] permitidos=c.ObtenFallosPermitidos();
			int[] conj=c.ObtenCtrolGruposConjunto();
			for(int i=0;i<conj.Length ;i++)
			{
			    int numConjunto = conj[i];
			    if(conjuntos[numConjunto]==false) numFallos++;
			}
		    if(permitidos[numFallos]) return true;
			return false;
		}

	}
}
