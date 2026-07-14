using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VuelosCRSA
{
    internal static class Program
    {
        // Código de idioma seleccionado: "es" (Español) o "en" (Inglés)
        public static string SelectedLanguage = "es";
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Mostrar diálogo de selección de idioma al iniciar
            try
            {
                using (var lf = new LanguageForm())
                {
                    var dr = lf.ShowDialog();
                    // LanguageForm establece Program.SelectedLanguage cuando se elige.
                    // Si el diálogo se cancela, mantener "es" por defecto.
                    if (dr != System.Windows.Forms.DialogResult.OK)
                    {
                        SelectedLanguage = "es";
                    }
                }
            }
            catch
            {
                // Si no se puede mostrar el formulario, usar Español por defecto
                SelectedLanguage = "es";
            }

            Application.Run(new Form1());
        }
    }
}
