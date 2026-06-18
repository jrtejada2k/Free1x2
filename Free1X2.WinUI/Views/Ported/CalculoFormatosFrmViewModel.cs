using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada de "CalculoFormatosFrm" (WinForms legacy: Free1X2.UI.CalculoFormatosFrm).
    /// Propósito legacy: dada una columna de 14 signos (1/X/2), extrae los formatos que aparecen
    /// (pares, tríos, cuartetos, quintetos) y los contactos, y vuelca el informe a un fichero de texto.
    /// La lógica (pares/tríos/cuartetos/quintetos + contactos) es la del form legacy, replicada
    /// íntegramente aquí (no depende del motor de cálculo, sólo de System.IO).
    /// </summary>
    public partial class CalculoFormatosFrmViewModel : ObservableObject
    {
        // Contadores de contactos (legacy: campos Num1X..NumVV).
        private int Num1X, Num12, NumX2, Num11, NumXX, Num22, Num1V, NumXV, Num2V, NumVV;

        // Entrada principal: la columna de 14 signos. En el legacy era textBox1 (CharacterCasing.Upper, 14 chars).
        [ObservableProperty]
        private string _columna = "";

        // Ruta del fichero de salida seleccionado. En el legacy era el campo 'archivoFinal'.
        [ObservableProperty]
        private string _archivoSalida = "";

        // Texto mostrado en el label de estado del fichero (legacy label2, por defecto "Falta Fichero Salida").
        [ObservableProperty]
        private string _ficheroSalidaTexto = "Falta Fichero Salida";

        // Informe textual generado (resultado del cálculo). En el legacy se escribía directo al StreamWriter.
        [ObservableProperty]
        private string _informe = "";

        /// <summary>
        /// Legacy: BtnFileOutClick -> SaveFileDialog (filtro "Informe(*.txt)").
        /// </summary>
        [RelayCommand]
        private async Task SeleccionarFicheroSalidaAsync()
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "Informe",
            };
            picker.FileTypeChoices.Add("Informe", new System.Collections.Generic.List<string> { ".txt" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                ArchivoSalida = file.Path;
                FicheroSalidaTexto = Path.GetFileName(ArchivoSalida);
            }
        }

        /// <summary>
        /// Legacy: Button2Click + AnalizaColumna/InicializaContadores.
        /// Valida longitud 14, calcula pares/tríos/cuartetos/quintetos/contactos y vuelca al fichero.
        /// </summary>
        [RelayCommand]
        private void SacarFormato()
        {
            // Legacy: si archivoFinal == "" -> aviso "Falta seleccionar archivo de salida".
            if (string.IsNullOrEmpty(ArchivoSalida))
            {
                AppServices.MostrarError("Falta seleccionar archivo de salida");
                return;
            }

            // Legacy: si longitud != 14 -> aviso de longitud (no aborta, igual que el legacy).
            if (Columna.Length != 14)
            {
                AppServices.MostrarError("numero de caracteres en la columna distinto de 14");
            }

            string columna = Columna;

            // Una col solo puede tener: en tríos 12 de los 27 y en cuartetos 11 de los 81.
            int[] ParesNum = new int[9];
            string[] Pares = new string[9];
            int[] TriosNum = new int[12];
            string[] Trios = new string[12];
            int[] CuartetosNum = new int[11];
            string[] Cuartetos = new string[11];
            int[] QuintetosNum = new int[10];
            string[] Quintetos = new string[10];

            int j;
            for (int i = 0; columna.Length >= 1; i++)
            {
                // ----- Pares -----
                bool HayOtroPar = false;
                if (columna.Length >= 2)
                {
                    string ParActual = columna.Substring(0, 2);
                    for (j = 0; j < 9; j++)
                    {
                        if (Pares[j] == ParActual) { HayOtroPar = true; ParesNum[j] += 1; }
                    }
                    if (!HayOtroPar)
                    {
                        for (j = 0; j < 9; j++)
                        {
                            if (ParesNum[j] == 0) { Pares[j] = ParActual; ParesNum[j] += 1; break; }
                        }
                    }
                }

                // ----- Tríos -----
                HayOtroPar = false;
                if (columna.Length >= 3)
                {
                    string TrioActual = columna.Substring(0, 3);
                    for (j = 0; j < 12; j++)
                    {
                        if (Trios[j] == TrioActual) { HayOtroPar = true; TriosNum[j] += 1; }
                    }
                    if (!HayOtroPar)
                    {
                        for (j = 0; j < 12; j++)
                        {
                            if (TriosNum[j] == 0) { Trios[j] = TrioActual; TriosNum[j] += 1; break; }
                        }
                    }
                }

                // ----- Cuartetos -----
                HayOtroPar = false;
                if (columna.Length >= 4)
                {
                    string CuartetoActual = columna.Substring(0, 4);
                    for (j = 0; j < 11; j++)
                    {
                        if (Cuartetos[j] == CuartetoActual) { HayOtroPar = true; CuartetosNum[j] += 1; }
                    }
                    if (!HayOtroPar)
                    {
                        for (j = 0; j < 11; j++)
                        {
                            if (CuartetosNum[j] == 0) { Cuartetos[j] = CuartetoActual; CuartetosNum[j] += 1; break; }
                        }
                    }
                }

                // ----- Quintetos -----
                HayOtroPar = false;
                if (columna.Length >= 5)
                {
                    string QuintetoActual = columna.Substring(0, 5);
                    for (j = 0; j < 10; j++)
                    {
                        if (Quintetos[j] == QuintetoActual) { HayOtroPar = true; QuintetosNum[j] += 1; }
                    }
                    if (!HayOtroPar)
                    {
                        for (j = 0; j < 10; j++)
                        {
                            if (QuintetosNum[j] == 0) { Quintetos[j] = QuintetoActual; QuintetosNum[j] += 1; break; }
                        }
                    }
                }

                // Acorta la columna inicial en 1 por la izquierda.
                columna = columna.Substring(1);
            }

            // Construye el informe (legacy: StreamWriter sw.WriteLine(...)).
            var sb = new StringBuilder();
            sb.AppendLine("* Pares que aparecen");
            for (j = 0; j < 9; j++)
            {
                if (ParesNum[j] != 0)
                {
                    sb.AppendLine(Pares[j] + "-" + ParesNum[j].ToString());
                    if (j == 8) sb.AppendLine("Pares distintos = " + (j + 1));
                }
                else { sb.AppendLine("Pares distintos = " + j); break; }
            }

            sb.AppendLine("* Trios que aparecen");
            for (j = 0; j < 12; j++)
            {
                if (TriosNum[j] != 0)
                {
                    sb.AppendLine(Trios[j] + "-" + TriosNum[j].ToString());
                    if (j == 11) sb.AppendLine("Trios distintos = " + (j + 1));
                }
                else { sb.AppendLine("Trios distintos = " + j); break; }
            }

            sb.AppendLine("* Cuartetos que aparecen");
            for (j = 0; j < 11; j++)
            {
                if (CuartetosNum[j] != 0)
                {
                    sb.AppendLine(Cuartetos[j] + "-" + CuartetosNum[j].ToString());
                    if (j == 10) sb.AppendLine("Cuartetos distintos = " + (j + 1));
                }
                else { sb.AppendLine("Cuartetos distintos = " + j); break; }
            }

            sb.AppendLine("* Quintetos que aparecen");
            for (j = 0; j < 10; j++)
            {
                if (QuintetosNum[j] != 0)
                {
                    sb.AppendLine(Quintetos[j] + "-" + QuintetosNum[j].ToString());
                    if (j == 9) sb.AppendLine("Quintetos distintos = " + (j + 1));
                }
                else { sb.AppendLine("Quintetos distintos = " + j); break; }
            }

            // Contactos (legacy: AnalizaColumna(textBox1.Text)).
            AnalizaColumna(Columna);
            sb.AppendLine("* Contactos");
            sb.AppendLine("1X" + "-" + Num1X.ToString());
            sb.AppendLine("12" + "-" + Num12.ToString());
            sb.AppendLine("X2" + "-" + NumX2.ToString());
            sb.AppendLine("11" + "-" + Num11.ToString());
            sb.AppendLine("XX" + "-" + NumXX.ToString());
            sb.AppendLine("22" + "-" + Num22.ToString());
            sb.AppendLine("1V" + "-" + Num1V.ToString());
            sb.AppendLine("XV" + "-" + NumXV.ToString());
            sb.AppendLine("2V" + "-" + Num2V.ToString());
            sb.AppendLine("VV" + "-" + NumVV.ToString());

            string informe = sb.ToString();
            Informe = informe;

            try
            {
                File.WriteAllText(ArchivoSalida, informe);
                AppServices.MostrarInfo("Ya");
            }
            catch (System.Exception ex)
            {
                AppServices.MostrarError("No se ha podido guardar el informe: " + ex.Message);
            }
        }

        // Legacy: InicializaContadores().
        private void InicializaContadores()
        {
            Num1X = Num12 = NumX2 = Num11 = NumXX = 0;
            Num22 = Num1V = NumXV = Num2V = NumVV = 0;
        }

        // Legacy: AnalizaColumna(string columna) — cuenta contactos por pares consecutivos.
        public void AnalizaColumna(string columna)
        {
            InicializaContadores();
            string columnaTemp = columna.Replace("*", "");
            for (int i = 0; i < (columnaTemp.Length - 1); i++)
            {
                switch (columnaTemp.Substring(i, 2))
                {
                    case "1X":
                    case "X1":
                    case "1x":
                    case "x1":
                        Num1X++; Num1V++; break;
                    case "12":
                    case "21":
                        Num12++; Num1V++; break;
                    case "X2":
                    case "2X":
                    case "x2":
                    case "2x":
                        NumX2++; NumVV++; NumXV++; Num2V++; break;
                    case "11":
                        Num11++; break;
                    case "XX":
                    case "xx":
                        NumXX++; NumVV++; NumXV++; break;
                    case "22":
                        Num22++; NumVV++; Num2V++; break;
                }
            }
        }
    }
}
