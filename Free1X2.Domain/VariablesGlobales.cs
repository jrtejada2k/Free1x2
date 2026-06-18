
using System;
using System.Collections.Generic;
using Free1X2.EntradaSalida;

namespace Free1X2
{
	/// <summary>
	/// Descripción breve de VariablesGlobales.
	/// </summary>
	public class VariablesGlobales
	{
		private static int numPartidos;
		private static int puntosFijos;
		private static int puntosDobles;
		private static int puntosTriples;
		private static int desplazamiento;
		private static string[] separador;

        private static double precioApuesta;
        private static double porcentaje14;
        private static double recaudacion;
        private static string moneda;

        private static bool mostrarTsFree;
        private static bool mostrarTsFiltros;
        private static bool mostrarTsCombinacion;
        private static bool mostrarTsOperaciones;
        private static bool mostrarTsArchivo;
        private static bool mostrarTsUtilidades;

        private static bool analizarTodo;
        private static bool analizarVX2;
        private static bool analizarSeguidos;
        private static bool analizarDibujos;
        private static bool analizarInterrupciones;
        private static bool analizarFormatos;
        private static bool analizarFormatos123;
        private static bool analizarContactos;
        private static bool analizarFigurasContactos;
        private static bool analizarFigurasV1X2;
        private static bool analizarFigurasPesos;
        private static bool analizarSimetrias;
        private static bool analizarSimetriasII;
        private static bool analizarGruposEquipos;
        private static bool analizarPesos;
        private static bool analizarValoracion;
        private static bool analizarDistancias;
        private static bool analizarCPs;
        private static bool analizarControlGrupos;
        private static bool analizarControlConjuntos;

        private static bool actualizarAlInicio;

        private static bool mostrarConfirmacionSalir;
        private static DateTime fechaUltimaComprobacionNotificaciones;
        private static Dictionary<string, string> diccionarioIdioma = new Dictionary<string, string>();
        private static string idioma = "";
        private static bool inicializado = false;
        
		static VariablesGlobales()
		{
			try
			{
				InicializarVariables();
			}
			catch (Exception ex)
			{
				// If initialization fails, set default values
				SetDefaultValues();
				System.Diagnostics.Debug.WriteLine($"VariablesGlobales initialization failed: {ex.Message}");
			}
		}

		protected static void InicializarVariables()
		{
			if (inicializado) return;
			
			try
			{
				// Try different possible paths for the config file
				string startupPath = GetConfigPath();
				
				// Abre el archivo de configuracion
				AConfiguracion ac = new AConfiguracion(startupPath);
				//Idioma
				ac.ObtenInfoIdioma(ref idioma);
				// Puntos CPs
				ac.ObtenPuntosCP(ref puntosFijos, ref puntosDobles, ref puntosTriples);
				// Desplazamiento
				ac.ObtenDesplazamiento(ref desplazamiento);
				// Configuración del Boleto
				ac.ObtenNumPartidos(ref numPartidos, ref separador);
				//Configuracion de las barras de herramientas
				ac.ObtenToolBarsVisibles(ref mostrarTsFree, ref mostrarTsFiltros, ref mostrarTsCombinacion, ref mostrarTsOperaciones, ref mostrarTsArchivo, ref mostrarTsUtilidades);
				//Configuración del actualizador
				ac.ObtenConfiguracionActualizador(ref actualizarAlInicio);
				//Configuración del análisis
				ac.ObtenConfiguracionAnalisis( ref analizarTodo,  ref analizarVX2, ref analizarSeguidos, ref analizarFigurasV1X2, ref analizarInterrupciones, ref analizarDibujos, ref analizarSimetrias, ref analizarFormatos,ref analizarFormatos123, ref analizarDistancias, ref analizarContactos, ref analizarFigurasContactos, ref analizarPesos, ref analizarFigurasPesos, ref analizarValoracion, ref analizarCPs, ref analizarGruposEquipos, ref analizarControlGrupos, ref analizarControlConjuntos, ref analizarSimetriasII);
				//ONLAE
				ac.ObtenValoresLAE(ref precioApuesta, ref porcentaje14, ref recaudacion, ref moneda);
				//Idioma
				ac.ObtenIdioma(ref diccionarioIdioma, idioma);
				//Advertencia al salir
				ac.ObtenConfiguracionAdvertenciaSalir(ref mostrarConfirmacionSalir);
				//Notificaciones
				ac.ObtenFechaUltimaComprobacionNotificaciones(ref fechaUltimaComprobacionNotificaciones);
				
				inicializado = true;
			}
			catch (Exception ex)
			{
				SetDefaultValues();
				System.Diagnostics.Debug.WriteLine($"Configuration loading failed: {ex.Message}");
			}
		}
		
		private static string GetConfigPath()
		{
			// Try multiple possible paths for the configuration file
			string[] possiblePaths = {
				System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
				System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar),
				Environment.CurrentDirectory,
				System.IO.Path.Combine(Environment.CurrentDirectory, "bin"),
				System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])
			};
			
			foreach (string path in possiblePaths)
			{
				try
				{
					if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(System.IO.Path.Combine(path, "parametros.free1x2")))
					{
						return path;
					}
				}
				catch
				{
					// Continue to next path
				}
			}
			
			// Fallback to application startup path
			return System.AppContext.BaseDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar) ?? Environment.CurrentDirectory;
		}
		
		private static void SetDefaultValues()
		{
			// Set safe default values if configuration loading fails
			numPartidos = 14;
			puntosFijos = 10;
			puntosDobles = 15;
			puntosTriples = 20;
			desplazamiento = 0;
			separador = new string[] { "1", "X", "2" };
			precioApuesta = 0.75;
			porcentaje14 = 0.55;
			recaudacion = 1000000;
			moneda = "€";
			idioma = "es-ES";
			mostrarTsFree = true;
			mostrarTsFiltros = true;
			mostrarTsCombinacion = true;
			mostrarTsOperaciones = true;
			mostrarTsArchivo = true;
			mostrarTsUtilidades = true;
			analizarTodo = true;
			actualizarAlInicio = false;
			mostrarConfirmacionSalir = true;
			fechaUltimaComprobacionNotificaciones = DateTime.Now;
			diccionarioIdioma = new Dictionary<string, string>();
			inicializado = true;
		}

		public static void ReinicializarVariables() 
		{ 
			InicializarVariables(); 
		}

        public static double PrecioApuesta
        {
            get { return precioApuesta; }
        }
        public static double Porcentaje14
        {
            get { return porcentaje14; }
        }
        public static double Recaudacion
        {
            get { return recaudacion; }
        }
        public static string Moneda
        {
            get { return moneda; }
        }

		public static int NumeroPartidos 
		{ 
			get { return numPartidos; } 
		} 

		public static int PuntosFijos 
		{ 
			get { return puntosFijos; } 
		} 

		public static int PuntosDobles 
		{ 
			get { return puntosDobles; } 
		} 

		public static int PuntosTriples 
		{ 
			get { return puntosTriples; } 
		} 

		public static int Desplazamiento 
		{ 
			get { return desplazamiento; } 
		} 

		public static string[] Separador 
		{ 
			get { return separador; } 
		}
        public static bool MostrarTsFree
        {
            get { return mostrarTsFree; }
        }
        public static bool MostrarTsFiltros
        {
            get { return mostrarTsFiltros; }
        }
        public static bool MostrarTsCombinacion
        {
            get { return mostrarTsCombinacion; }
        }
        public static bool MostrarTsOperaciones
        {
            get { return mostrarTsOperaciones; }
        }
        public static bool MostrarTsArchivo
        {
            get { return mostrarTsArchivo; }
        }
        public static bool MostrarTsUtilidades
        {
            get { return mostrarTsUtilidades; }
        }
        public static bool ActualizarAlInicio
        {
            get { return actualizarAlInicio; }
        }
        public static bool PedirConfirmacionAlSalir
        {
            get { return mostrarConfirmacionSalir; }
        }
        //Propiedades que configuran el análisis

        public static bool AnalizarTodo
        {
            get { return analizarTodo; }
        }
        public static bool AnalizarVX2
        {
            get { return analizarVX2; }
        }
        public static bool AnalizarSeguidos
        {
            get { return analizarSeguidos; }
        }
        public static bool AnalizarDibujos
        {
            get { return analizarDibujos; }
        }
        public static bool AnalizarInterrupciones
        {
            get { return analizarInterrupciones; }
        }
        public static bool AnalizarSimetrias
        {
            get { return analizarSimetrias; }
        }
        public static bool AnalizarSimetriasII
        {
            get { return analizarSimetriasII; }
        }
        public static bool AnalizarContactos
        {
            get { return analizarContactos; }
        }
        public static bool AnalizarFigurasContactos
        {
            get { return analizarFigurasContactos; }
        }
        public static bool AnalizarFigurasV1X2
        {
            get { return analizarFigurasV1X2; }
        }
        public static bool AnalizarFigurasPesos
        {
            get { return analizarFigurasPesos; }
        }
        public static bool AnalizarCPs
        {
            get { return analizarCPs; }
        }
        public static bool AnalizarValoracion
        {
            get { return analizarValoracion; }
        }
        public static bool AnalizarFormatos
        {
            get { return analizarFormatos; }
        }
        public static bool AnalizarFormatos123
        {
            get { return analizarFormatos123; }
        }
        public static bool AnalizarGruposEquipos
        {
            get { return analizarGruposEquipos; }
        }
        public static bool AnalizarPesos
        {
            get { return analizarPesos; }
        }
        public static bool AnalizarDistancias
        {
            get { return analizarDistancias; }
        }
        public static bool AnalizarControlGrupos
        {
            get { return analizarControlGrupos; }
        }
        public static bool AnalizarControlConjuntos
        {
            get { return analizarControlConjuntos; }
        }
        public static string Idioma
        {
            get { return idioma; }
        }
        public static Dictionary<string, string> DiccionarioIdioma
        {
            get { return diccionarioIdioma; }
        }
 
        public static string SistemaOperativo
        {
            get { return Environment.OSVersion.ToString(); }
        }
        public static bool FuncionaBajoMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }
        public static DateTime FechaUltimaComprobacionNotificaciones
        {
            get
            {
                return fechaUltimaComprobacionNotificaciones;
            }
        }
	}
}
