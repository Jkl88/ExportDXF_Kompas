using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportDXF_Kompas
{
    static class Program
    {
        static MainForm mainForm;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]        
        static void Main()
        {
            Application.ApplicationExit += OnAppExit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            Application.Run(mainForm);
        }

        private static void OnAppExit(object sender, EventArgs e)
        {
           var kompas = mainForm.kompas;
           if (kompas.startKompas) kompas.getApp().Quit();
        }
    }
}
