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
        public static readonly Logger logger = new("./data/logs", "Log__" + DateTime.Now.ToString("dd-MM-yyyy__HH-mm-ss") + ".txt");

        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler((sender, ev) => {
                Exception e = (Exception)ev.ExceptionObject;
                Exception(e.Message, e.StackTrace);
            });
        }
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception(e.Exception.Message, e.Exception.StackTrace);
            e.Handled = true;
        }
        private static void Exception (string message, string? stack)
        {
            MessageBox.Show(message + "\n\nPlease submit a bug report issue on GitHub with the log file found at\n" + logger.logFile, "An unhandled exception occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            logger.Error(message + "\n" + (stack ?? ""));
        }
    }
}
