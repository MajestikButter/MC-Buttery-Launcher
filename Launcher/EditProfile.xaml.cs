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
        }

        private void Confirm()
        {

            Profile.name = NameTextBox.Text;
            Profile.path = PathTextBox.Text;

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
    }
}
