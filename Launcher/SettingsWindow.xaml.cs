using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MC_Buttery_Launcher
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        readonly Settings Settings;
        readonly MainWindow MainWindow;

        public SettingsWindow(Settings settings, MainWindow mainWindow)
        {
            InitializeComponent();

            Settings = settings;
            MainWindow = mainWindow;

            KeepOpenCheckBox.IsChecked = settings.keepOpen;

            WindowSize size = Settings.windowSizes["SettingsWindow"];
            Height = size.height;
            Width = size.width;
        }

        private void OnClose(object? sender, EventArgs e)
        {
            Debug.WriteLine("Closing SettingsWindow");
            Settings.windowSizes["SettingsWindow"] = new WindowSize(Height, Width);


            Settings.keepOpen = KeepOpenCheckBox.IsChecked ?? false;

            Debug.WriteLine("Saving settings");
            Debug.WriteLine(JsonConvert.SerializeObject(Settings));

            Settings.Save();
        }
    }
}
