using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Free1X2.UI
{
    public class FormulariosHelper
    {
        public FormulariosHelper()
        {
        }
        public string ObtenerTodosValores()
        {
            string valores = "";
            for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                valores += i.ToString() + ",";
            }
            return valores.Substring(0,valores.Length - 1);
        }
        public void Redimensionar(Control c)
        {
            if (VariablesGlobales.NumeroPartidos > 14)
            {
                c.Width += 60;                
            }
        }
        public void Traducir(Form f)
        {
          /*  if (VariablesGlobales.Idioma != "es-ES")
            {
                if (VariablesGlobales.DiccionarioIdioma.ContainsKey(f.Text.ToUpper()))
                {
                    f.Text = VariablesGlobales.DiccionarioIdioma[f.Text.ToUpper()];
                }

                for (int i = 0; i < f.Controls.Count; i++)
                {
                    try
                    {
                        Control c = f.Controls[i];
                        if (c.Controls.Count > 0)
                        {
                            TraducirControl(c);
                        }
                        if (VariablesGlobales.DiccionarioIdioma.ContainsKey(c.Text.ToUpper()))
                        {
                            c.Text = VariablesGlobales.DiccionarioIdioma[c.Text.ToUpper()];
                        }
                    }
                    catch
                    {
                    }
                }
            }
           * */
        }
        private void TraducirControl(Control c)
        {
            if (VariablesGlobales.DiccionarioIdioma.ContainsKey(c.Text.ToUpper()))
            {
                c.Text = VariablesGlobales.DiccionarioIdioma[c.Text.ToUpper()];
            }
            if (c.Controls.Count > 0)
            {
                for (int i = 0; i < c.Controls.Count; i++)
                {
                    TraducirControl(c.Controls[i]);
                }
            }
            if (c.GetType().ToString() == "System.Windows.Forms.MenuStrip")
            {
                MenuStrip mS = (MenuStrip)c;
                TraducirMenuStrip(mS);                
            }
        }
        private void TraducirMenuStrip(MenuStrip mS)
        {
            for (int i = 0; i < mS.Items.Count; i++)
            {
                ToolStripMenuItem tsmi = (ToolStripMenuItem)mS.Items[i];

                if (VariablesGlobales.DiccionarioIdioma.ContainsKey(tsmi.Text.ToUpper()))
                {
                    tsmi.Text = VariablesGlobales.DiccionarioIdioma[tsmi.Text.ToUpper()];
                }

                if (tsmi.DropDownItems.Count > 0)
                {
                    TraducirToolStripMenuItem(tsmi);
                }


            }
        }
        private void TraducirToolStripMenuItem(ToolStripMenuItem t)
        {
            for (int i = 0; i < t.DropDownItems.Count; i++)
            {
                ToolStripDropDownItem it = (ToolStripDropDownItem)t.DropDownItems[i];
                if (VariablesGlobales.DiccionarioIdioma.ContainsKey(it.Text.ToUpper()))
                {
                    it.Text = VariablesGlobales.DiccionarioIdioma[it.Text.ToUpper()];
                }
                if (it.DropDownItems.Count > 0)
                {
                    for (int j = 0; j < it.DropDownItems.Count; j++)
                    {
                        if (VariablesGlobales.DiccionarioIdioma.ContainsKey(it.DropDownItems[j].Text.ToUpper()))
                        {
                            it.DropDownItems[j].Text = VariablesGlobales.DiccionarioIdioma[it.DropDownItems[j].Text.ToUpper()];
                        }
                    }
                }

            }
        }
        public void CambiarFondoBoton(Button boton)
        {
            if (boton.Enabled)
                boton.BackColor = System.Drawing.Color.LightSalmon;
            else
                boton.BackColor = System.Drawing.Color.Silver;
        }

        public bool ExisteFicheroTemporal(string fichero)
        {
            // Borra los posibles archivos temporales
            DirectoryInfo d = new DirectoryInfo(Application.StartupPath + "/Temp/");
            FileInfo[] f = d.GetFiles(fichero);

            return f.Length > 0;
        }
    }
}
