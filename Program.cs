
using System;
using System.Windows.Forms;
using MunicipalServicesApp.Forms;

namespace MunicipalServicesApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainMenuForm());
        }
    }
}
