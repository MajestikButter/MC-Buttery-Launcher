using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MC_Buttery_Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Logger logger = new("./data/logs", "Log__" + DateTime.Now.ToString("dd-MM-yyyy__HH-mm-ss") + ".txt");

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + "\n\nPlus submit a bug report issue on GitHub with the log file found at\n" + logger.logFile, "An unhandled exception occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            logger.Error(e.Exception.Message + "\n" + e.Exception.StackTrace);
            e.Handled = true;
        }
    }
}
