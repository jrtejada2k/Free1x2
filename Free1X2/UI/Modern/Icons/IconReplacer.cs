using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Free1X2.UI.Modern.Theming;

namespace Free1X2.UI.Modern.Icons
{
    /// <summary>
    /// Replaces legacy embedded toolbar/button images with modern Segoe Fluent Icons,
    /// matched by control Name. No-ops if the Fluent font is unavailable, leaving
    /// legacy icons intact.
    /// </summary>
    public static class IconReplacer
    {
        private const int ToolStripIconSize = 16;
        private const int ButtonIconSize    = 20;

        // control.Name → Segoe Fluent glyph
        private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
        {
            // ===== tsFree =====
            { "bSalir",                  SegoeIcons.Glyph.PowerButton    },
            { "bConfig",                 SegoeIcons.Glyph.Settings       },
            { "bConfAnalisis",           SegoeIcons.Glyph.DeveloperTools },
            { "bAyuda",                  SegoeIcons.Glyph.Help           },
            { "bAcercaDe",               SegoeIcons.Glyph.Info           },

            // ===== tsArchivo =====
            { "bGuardarEquipos",         SegoeIcons.Glyph.Save           },
            { "bNuevo",                  SegoeIcons.Glyph.NewFile        },
            { "bObtenerBoletos",         SegoeIcons.Glyph.CloudDownload  },
            { "bAbrirCombinacion",       SegoeIcons.Glyph.OpenFile       },
            { "bGuardarCombinacion",     SegoeIcons.Glyph.Save           },
            { "bGuardarCombinacionComo", SegoeIcons.Glyph.SaveAs         },
            { "bBorrarTemporales",       SegoeIcons.Glyph.Delete         },
            { "bAbrirEquipos",           SegoeIcons.Glyph.FolderOpen     },
            { "bBorrarInformes",         SegoeIcons.Glyph.Delete         },
            { "bGestorEquipos",          SegoeIcons.Glyph.People         },

            // ===== tsCombinacion =====
            { "bCalcular",               SegoeIcons.Glyph.Calculator     },
            { "bCalcularM",              SegoeIcons.Glyph.Sigma          },
            { "bVerBoletos",             SegoeIcons.Glyph.View           },
            { "bImprimirBoletos",        SegoeIcons.Glyph.Print          },
            { "bReducir",                SegoeIcons.Glyph.BackToWindow   },
            { "bEscrutinio",             SegoeIcons.Glyph.Search         },
            { "bEscrutarComb",           SegoeIcons.Glyph.DocumentApproval },
            { "bAnalisisColumnas",       SegoeIcons.Glyph.BarChartVertical },
            { "bAnalisisFallos",         SegoeIcons.Glyph.Error          },
            { "bAnalisisGrafico",        SegoeIcons.Glyph.BarChart4      },
            { "bAnalisisSignos",         SegoeIcons.Glyph.BulletedList   },
            { "bProbabilidades",         SegoeIcons.Glyph.Equalizer      },
            { "bEstadisticas",           SegoeIcons.Glyph.LineChart      },
            { "bP15",                    SegoeIcons.Glyph.Add            },

            // ===== tsOperaciones =====
            { "bAlgebra",                SegoeIcons.Glyph.Calculator     },
            { "bTransposición",          SegoeIcons.Glyph.Switch         },
            { "bMultiplicacion",         SegoeIcons.Glyph.Sigma          },
            { "bFraccionador",           SegoeIcons.Glyph.Crop           },
            { "bRotacion",               SegoeIcons.Glyph.Rotate         },

            // ===== tsUtilidades =====
            { "bSubeCategoria",          SegoeIcons.Glyph.Up             },
            { "bModificadorPct",         SegoeIcons.Glyph.Equalizer      },
            { "bGeneradorCPs",           SegoeIcons.Glyph.Tools          },
            { "bDiferenciasColumnas",    SegoeIcons.Glyph.Diff           },
            { "bProbabilidad",           SegoeIcons.Glyph.Sort           },
            { "bSelectorJuanM",          SegoeIcons.Glyph.Contact        },
            { "bSelectorMarioSan",       SegoeIcons.Glyph.Contact2       },
            { "bRentabilidad",           SegoeIcons.Glyph.Bank           },
            { "bColumnasGEPT",           SegoeIcons.Glyph.ViewAll        },
            { "bTramificar",             SegoeIcons.Glyph.ColumnRightTwo },
            { "bPremiadas",              SegoeIcons.Glyph.Trophy         },
            { "bEstimacion",             SegoeIcons.Glyph.Calculator     },
            { "bBancoPruebas",           SegoeIcons.Glyph.Tools          },
            { "bImportExport",           SegoeIcons.Glyph.Sync           },
            { "bAnalisisGrupos",         SegoeIcons.Glyph.People         },
            { "bRedPerfectas",           SegoeIcons.Glyph.BackToWindow   },
            { "bDependenciaLineal",      SegoeIcons.Glyph.LineChart      },

            // ===== tsFiltros =====
            { "bCombinarFiltros",        SegoeIcons.Glyph.Filter         },
            { "bDiferenciasFiltros",     SegoeIcons.Glyph.Diff           },
            { "bFiltroCoincidencias",    SegoeIcons.Glyph.FilterChecked  },
            { "bFiltroAidomnou",         SegoeIcons.Glyph.Filter         },
            { "bFiltroPim",              SegoeIcons.Glyph.Filter         },

            // ===== Condiciones panel (large buttons) =====
            { "btnNoVariantes",          SegoeIcons.Glyph.Permissions    },
            { "btnSignosSeguidos",       SegoeIcons.Glyph.BulletedList   },
            { "btnPesosNum",             SegoeIcons.Glyph.Calculator     },
            { "btnValoracion",           SegoeIcons.Glyph.Star           },
            { "btnDibujos",              SegoeIcons.Glyph.Brush          },
            { "btnIterrupciones",        SegoeIcons.Glyph.Pause          },
            { "btnCP",                   SegoeIcons.Glyph.Tablet         },
            { "btnDistancias",           SegoeIcons.Glyph.Switch         },
            { "btnGruposEquipos",        SegoeIcons.Glyph.People         },
            { "btnFormatos",             SegoeIcons.Glyph.BulletedList   },
            { "btnContactos",            SegoeIcons.Glyph.ContactInfo    },
            { "btnFormatos123",          SegoeIcons.Glyph.Sigma          },
            { "btnSimetrias",            SegoeIcons.Glyph.MirrorImage    },
            { "btnDiferencias",          SegoeIcons.Glyph.Diff           },
            { "btnIfThen",               SegoeIcons.Glyph.Branch         },
            { "btnTolGrupo",             SegoeIcons.Glyph.Tools          },

            // ===== Filtro / Filtro Parcial panels =====
            { "btnAddFiltroCols",        SegoeIcons.Glyph.Add            },
            { "btnAbreFiltroParcial",    SegoeIcons.Glyph.FolderOpen     },

            // ===== Group navigation (bottom strip) =====
            { "btnGrupoInicio",          SegoeIcons.Glyph.First          },
            { "btnGrupoPrev",            SegoeIcons.Glyph.Back           },
            { "btnGrupoSiguiente",       SegoeIcons.Glyph.Forward        },
            { "btnGrupoFin",             SegoeIcons.Glyph.Last           },
            { "btnGrupoPrevM",           SegoeIcons.Glyph.Back           },
            { "btnGrupoSiguienteM",      SegoeIcons.Glyph.Forward        },
            { "btnControlGrupos",        SegoeIcons.Glyph.People         },
            { "btnInsertarGrupo",        SegoeIcons.Glyph.Add            },
            { "btnBorrarGrupo",          SegoeIcons.Glyph.Remove         },
            { "btnCopiarGrupo",          SegoeIcons.Glyph.Copy           },
            { "btnPegarGrupo",           SegoeIcons.Glyph.Paste          },
            { "btnGuardarGrupo",         SegoeIcons.Glyph.Save           },
            { "btnAbrirGrupo",           SegoeIcons.Glyph.OpenFile       },
        };

        /// <summary>
        /// Walk the control tree and swap legacy images on any ToolStripItem or Button
        /// whose Name appears in <see cref="Map"/>. Safe to call multiple times.
        /// </summary>
        public static void Apply(Control root)
        {
            if (root == null || !SegoeIcons.IsAvailable) return;
            ApplyRecursive(root);
        }

        private static void ApplyRecursive(Control c)
        {
            if (c is ToolStrip ts)
            {
                foreach (ToolStripItem item in ts.Items)
                    ReplaceItem(item);
            }
            else if (c is MenuStrip ms)
            {
                foreach (ToolStripItem item in ms.Items)
                    ReplaceItem(item);
            }
            else if (c is Button btn && btn.Image != null)
            {
                if (Map.TryGetValue(btn.Name, out string glyph))
                    btn.Image = SegoeIcons.Render(glyph, ButtonIconSize, ModernTheme.Colors.Text);
            }

            foreach (Control child in c.Controls)
                ApplyRecursive(child);
        }

        private static void ReplaceItem(ToolStripItem item)
        {
            if (Map.TryGetValue(item.Name, out string glyph))
                item.Image = SegoeIcons.Render(glyph, ToolStripIconSize, ModernTheme.Colors.Text);

            if (item is ToolStripDropDownItem dd)
            {
                foreach (ToolStripItem child in dd.DropDownItems)
                    ReplaceItem(child);
            }
        }
    }
}
