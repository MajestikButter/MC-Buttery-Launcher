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
using Ookii.Dialogs.Wpf;
using System.Diagnostics;

namespace MC_Buttery_Launcher
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        readonly Profile Profile;
        readonly MainWindow MainWindow;
        public EditProfile(Profile Profile, MainWindow MainWindow)
        {
            InitializeComponent();
            this.Profile = Profile;
            this.MainWindow = MainWindow;
            NameTextBox.Text = Profile.name;
            PathTextBox.Text = Profile.path;
            SubfoldersTextBox.Text = SubfoldersToString();

            WindowSize size = MainWindow.Settings.windowSizes["EditProfile"];
            Height = size.height;
            Width = size.width;
        }

        private string SubfoldersToString()
        {
            return string.Join("\n", Profile.subfolders.Select(x => x.Key + " = " + x.Value));
        }

        private Dictionary<string, string> SubfoldersFromString()
        {
            string[] definitions = SubfoldersTextBox.Text.Split('\n');
            Dictionary<string, string> subfolders = new();
            foreach (string definition in definitions)
            {
                string[] keyValuePair = definition.Trim().Split("=");
                if (keyValuePair.Length == 2)
                {
                    subfolders.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
                }
            }
            return subfolders;
        }

        private void Confirm()
        {

            Profile.name = NameTextBox.Text;
            Profile.path = PathTextBox.Text;
            Profile.subfolders = SubfoldersFromString();

            Profile.Save();
            MainWindow.RefreshProfiles();
            Close();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            Confirm();
        }

        private void PathClick(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();
            dialog.SelectedPath = PathTextBox.Text;
            if (dialog.ShowDialog() ?? false)
            {
                PathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Confirm();
            }
        }

        private void OnClose(object? sender, EventArgs e)
        {
            Debug.WriteLine("Closing EditProfile");
            MainWindow.Settings.windowSizes["EditProfile"] = new WindowSize(Height, Width);
        }
    }
}
