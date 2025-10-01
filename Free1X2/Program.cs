using System;
using System.Threading;
using System.Windows.Forms;
using Free1X2.UI;

namespace Free1X2
{
    /// <summary>
    /// Application entry point using the original main form
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                InitializeApplication();
                RunApplication(args);
            }
            catch (Exception ex)
            {
                HandleStartupException(ex);
            }
        }

        private static void InitializeApplication()
        {
            // Enable visual styles and modern rendering
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Enable DPI awareness for modern displays
            try
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
            }
            catch
            {
                // SetHighDpiMode might not be available in all .NET versions
            }
            
            // Set up global exception handling
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void RunApplication(string[] args)
        {
            // Create and run the original main form
            using (var mainForm = new MainForm())
            {
                // Handle command line arguments if provided
                ProcessCommandLineArguments(mainForm, args);
                
                // Run the application
                Application.Run(mainForm);
            }
        }

        private static void ProcessCommandLineArguments(MainForm mainForm, string[] args)
        {
            if (args != null && args.Length > 0)
            {
                // Handle file associations or command line parameters
                foreach (string arg in args)
                {
                    if (System.IO.File.Exists(arg))
                    {
                        // Auto-load file if it exists
                        // mainForm.LoadFile(arg);
                        break;
                    }
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "Application Thread Exception");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception, "Unhandled Domain Exception");
            }
        }

        private static void HandleException(Exception exception, string context)
        {
            try
            {
                // Create error report with original error handling logic
                string nombreSeg = "Informe_" + DateTime.Now.ToShortDateString().Replace('/', '-') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '-') + ".txt";
                
                var manejador = new Free1X2.Debug.ManejadorExcepciones();
                manejador.GuardarInformeErrorATxt(exception, nombreSeg); 

                var infoError = new Free1X2.Debug.InfoError(nombreSeg);
                infoError.ShowDialog();
            }
            catch
            {
                try
                {
                    var infoError = new Free1X2.Debug.InfoError("");
                    infoError.ShowDialog();
                }
                catch
                {
                    Application.Exit();
                }
            }
        }

        private static void HandleStartupException(Exception exception)
        {
            string message = $"Error crítico durante el inicio de Free1X2:\n\n" +
                           $"{exception.Message}\n\n" +
                           $"La aplicación no puede continuar.";

            MessageBox.Show(
                message,
                "Error de Inicio",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            Environment.Exit(1);
        }
    }
}