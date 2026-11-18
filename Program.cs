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
        static void Main(string[] args)
        {
            Application.ApplicationExit += OnAppExit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
                Application.Run(new MainForm(""));
            else
                Application.Run(new MainForm(args[0]));
        }

        private static void OnAppExit(object sender, EventArgs e)
        {
           var kompas = mainForm.kompas;
           if (kompas.startKompas) kompas.getApp().Quit();
        }
    }
}
