﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Principal;
using System.Security.AccessControl;
using SymbolicLinkSupport;
using Windows.System;
using Newtonsoft.Json;

namespace MC_Buttery_Launcher
{
    public partial class MainWindow : Window
    {
        public static readonly string ReleasePath = Environment.GetEnvironmentVariable("LocalAppData") + "/Packages/Microsoft.MinecraftUWP_8wekyb3d8bbwe/LocalState/games/com.mojang";
        public static readonly string PreviewPath = Environment.GetEnvironmentVariable("LocalAppData") + "/Packages/Microsoft.MinecraftWindowsBeta_8wekyb3d8bbwe/LocalState/games/com.mojang";
        public static readonly string DefaultReleaseProfilePath = Directory.GetCurrentDirectory().Replace(@"\", "/") + "/data/profiles/release/default";
        public static readonly string DefaultPreviewProfilePath = Directory.GetCurrentDirectory().Replace(@"\", "/") + "/data/profiles/preview/default";
        public static readonly string ReleasePackageName = "Microsoft.MinecraftUWP_8wekyb3d8bbwe";
        public static readonly string PreviewPackageName = "Microsoft.MinecraftWindowsBeta_8wekyb3d8bbwe";
        public static readonly SecurityIdentifier ReleaseSecurityId = new("S-1-15-2-1958404141-86561845-1752920682-3514627264-368642714-62675701-733520436");
        public static readonly SecurityIdentifier PreviewSecurityId = new("S-1-15-3-424268864-5579737-879501358-346833251-474568803-887069379-4040235476");
        public static readonly Dictionary<string, WindowSize> DefaultWindowSizes = new()
        {
            { "MainWindow", new WindowSize(440, 820) },
            { "SettingsWindow", new WindowSize(80, 375) },
            { "EditProfile", new WindowSize(225, 690) }
        };

        readonly List<Profile> PreviewProfiles = new();
        readonly List<Profile> ReleaseProfiles = new();

        public readonly Settings Settings;

        public MainWindow()
        {
            InitializeComponent();

            VerifyPaths();
            FetchProfiles();

            VerifySettingsFile();
            string settingsStr = File.ReadAllText("./data/settings.json");
            Settings = JsonConvert.DeserializeObject<Settings>(settingsStr)!;
            VerifySettings();

            WindowSize size = Settings.windowSizes["MainWindow"];
            Height = size.height;
            Width = size.width;

            RefreshProfiles();
            SetupProfileLists();
        }

        private static void VerifyPaths()
        {
            VerifyDataPath();
            VerifyProfilePath();
        }

        private static bool VerifyPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return false;
            }
            return true;
        }

        private static void VerifySettingsFile()
        {
            if (!File.Exists("./data/settings.json"))
            {
                Dictionary<string, WindowSize> windowSizes = new();
                foreach (var windowSize in DefaultWindowSizes)
                {
                    windowSizes.Add(windowSize.Key, windowSize.Value.Clone());
                }
                File.WriteAllText("./data/settings.json", JsonConvert.SerializeObject(new Settings(null, null, false, windowSizes)));
            }
        }
        private void VerifySettings()
        {
            #pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (Settings.keepOpen == null) Settings.keepOpen = true;
            #pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

            if (Settings.selectedPreview == null || PreviewProfiles.FindIndex((v) => v.uuid == Settings.selectedPreview) < 0)
            {
                Settings.selectedPreview = PreviewProfiles[0].uuid;
            }
            if (Settings.selectedRelease == null || ReleaseProfiles.FindIndex((v) => v.uuid == Settings.selectedRelease) < 0)
            {
                Settings.selectedRelease = ReleaseProfiles[0].uuid;
            }
            if (Settings.windowSizes == null)
            {
                Dictionary<string, WindowSize> windowSizes = new();
                foreach (var windowSize in DefaultWindowSizes)
                {
                    windowSizes.Add(windowSize.Key, windowSize.Value.Clone());
                }
                Settings.windowSizes = windowSizes;
            }
        }

        private int GetSelectedProfileIndex(bool isRelease) {
            if (isRelease)
            {
                int index = ReleaseProfiles.FindIndex((v) => v.uuid == Settings.selectedRelease);
                if (index < 0) return 0;
                return index;
            }
            else
            {
                int index = PreviewProfiles.FindIndex((v) => v.uuid == Settings.selectedPreview);
                if (index < 0) return 0;
                return index;
            }
        }

        private static bool VerifyDataPath()
        {
            return VerifyPath("./data");
        }

        private static bool VerifyProfilePath()
        {
            return VerifyPath("./data/profiles");
        }

        public void RefreshProfiles()
        {
            FetchProfiles();
            ReleaseProfileList.Items.Refresh();
            ReleaseProfileList.SelectedIndex = GetSelectedProfileIndex(true);
            PreviewProfileList.Items.Refresh();
            PreviewProfileList.SelectedIndex = GetSelectedProfileIndex(false);
        }

        private void SetupProfileLists()
        {
            ReleaseProfileList.ItemsSource = ReleaseProfiles;
            ReleaseProfileList.SelectedIndex = ReleaseProfiles.FindIndex(p => p.uuid == Settings.selectedRelease);

            PreviewProfileList.ItemsSource = PreviewProfiles;
            PreviewProfileList.SelectedIndex = PreviewProfiles.FindIndex(p => p.uuid == Settings.selectedPreview);
        }

        private void FetchProfiles()
        {
            Profile releaseProfile = new(true, "Default Release", DefaultReleaseProfilePath);
            Profile previewProfile = new(false, "Default Preview", DefaultPreviewProfilePath);
            if (!File.Exists("./data/profiles.json"))
            {
                List<Profile> profiles = new();

                SetReleaseProfile(releaseProfile);
                SetPreviewProfile(previewProfile);

                profiles.Add(releaseProfile);
                profiles.Add(previewProfile);

                File.WriteAllText("./data/profiles.json", JsonConvert.SerializeObject(profiles, Formatting.Indented));
            };

            string profileStr = File.ReadAllText("./data/profiles.json");
            List<Profile> Profiles = JsonConvert.DeserializeObject<List<Profile>>(profileStr) ?? new();

            ReleaseProfiles.Clear();
            PreviewProfiles.Clear();
            foreach (Profile profile in Profiles)
            {
                profile.path = profile.path.Replace(@"\\", "/");
                if (profile.isRelease) ReleaseProfiles.Add(profile);
                else PreviewProfiles.Add(profile);
            }
            if (ReleaseProfiles.Count <= 0)
            {
                ReleaseProfiles.Add(releaseProfile);
                releaseProfile.Save();
            }
            if (PreviewProfiles.Count <= 0)
            {
                PreviewProfiles.Add(previewProfile);
                previewProfile.Save();
            }
        }
        private static bool IsSymbolic(string path)
        {
            return DirectoryInfoExtensions.IsSymbolicLink(new DirectoryInfo(path));
        }

        private static void TryMoveFolder(string path, int tryCount = 0)
        {
            if (tryCount > 100) throw new Exception("Attempted to move folder more than 100 times");
            if (tryCount > 0)
            {
                if (!Directory.Exists(path + ".copy_" + tryCount)) {
                    Directory.Move(path, path + ".copy_" + tryCount);
                    return;
                }
                TryMoveFolder(path, tryCount + 1);
                return;
            }
            if (!Directory.Exists(path + ".copy"))
            {
                Directory.Move(path, path + ".copy");
                return;
            }
            TryMoveFolder(path, tryCount + 1);
        }

        private static void SymLinkPath(string fromPath, string toPath, SecurityIdentifier secId)
        {
            VerifyPath(toPath);

            SecurityIdentifier owner = WindowsIdentity.GetCurrent().User!;

            DirectoryInfo fromInfo = new(fromPath);
            DirectoryInfo toInfo = new(toPath);

            string[] pathSplit = fromPath.Split("/".ToCharArray()[0]);
            string folderPath = string.Join("/", pathSplit.Take(pathSplit.Length - 1));
            if (Directory.Exists(fromPath))
            {
                if (!IsSymbolic(fromPath))
                {
                    TryMoveFolder(fromPath);
                }
                else
                {
                    Directory.Delete(fromPath, false);
                };
            }
            else if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            DirectoryInfoExtensions.CreateSymbolicLink(toInfo, fromPath);

            FileSystemAccessRule accessRule = new(
                secId,
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
                AccessControlType.Allow
            );

            DirectorySecurity fromSec = fromInfo.GetAccessControl();
            fromSec.SetOwner(owner);
            fromSec.AddAccessRule(accessRule);
            fromInfo.SetAccessControl(fromSec);

            DirectorySecurity toSec = toInfo.GetAccessControl();
            toSec.SetOwner(owner);
            toSec.AddAccessRule(accessRule);
            toInfo.SetAccessControl(toSec);
        }

        private static void SwitchProfile(string mojangPath, Profile profile, SecurityIdentifier secId)
        {
            SymLinkPath(mojangPath, profile.path, secId);
            foreach (var folder in profile.subfolders)
            {
                SymLinkPath(profile.path + "/" + folder.Key, folder.Value, secId);
            }
        }

        private void SetReleaseProfile(Profile profile)
        {
            SwitchProfile(ReleasePath, profile, ReleaseSecurityId);
            Settings.selectedRelease = profile.uuid;
            Settings.Save();
        }

        private void SetPreviewProfile(Profile profile)
        {
            SwitchProfile(PreviewPath, profile, PreviewSecurityId);
            Settings.selectedPreview = profile.uuid;
            Settings.Save();
        }

        // Event Handlers

        private async void LaunchRelease()
        {
            Profile profile = (Profile)ReleaseProfileList.SelectedItem;
            SetReleaseProfile(profile);

            var pkg = await AppDiagnosticInfo.RequestInfoForPackageAsync(ReleasePackageName);
            if (pkg.Count <= 0) return;
            await pkg[0].LaunchAsync();
            if (!Settings.keepOpen) Application.Current.Shutdown();
        }
        private void ReleaseLaunchClick(object sender, RoutedEventArgs e)
        {
            LaunchRelease();
        }

        private async void LaunchPreview()
        {
            Profile profile = (Profile)PreviewProfileList.SelectedItem;
            SetPreviewProfile(profile);

            var pkg = await AppDiagnosticInfo.RequestInfoForPackageAsync(PreviewPackageName);
            if (pkg.Count <= 0) return;
            await pkg[0].LaunchAsync();
            if (!Settings.keepOpen) Application.Current.Shutdown();
        }
        private void PreviewLaunchClick(object sender, RoutedEventArgs e)
        {
            LaunchPreview();
        }

        private void ReleaseProfileClick(object sender, RoutedEventArgs e)
        {
            Profile profile = (Profile)ReleaseProfileList.SelectedItem;
            SetReleaseProfile(profile);
        }

        private void PreviewProfileClick(object sender, RoutedEventArgs e)
        {
            Profile profile = (Profile)PreviewProfileList.SelectedItem;
            SetPreviewProfile(profile);
        }

        private void EditProfileUI(Profile profile)
        {
            new EditProfile(profile, this).ShowDialog();
        }

        private void NewReleaseProfileClick(object sender, RoutedEventArgs e)
        {
            EditProfileUI(new Profile(true));
        }

        private void NewPreviewProfileClick(object sender, RoutedEventArgs e)
        {
            EditProfileUI(new Profile(false));
        }

        private void ReleaseEditClick(object sender, RoutedEventArgs e)
        {
            EditProfileUI((Profile)ReleaseProfileList.SelectedItem);
        }

        private void PreviewEditClick(object sender, RoutedEventArgs e)
        {
            EditProfileUI((Profile)PreviewProfileList.SelectedItem);
        }

        private void ReleaseRemoveClick(object sender, RoutedEventArgs e)
        {
            Profile selected = (Profile)ReleaseProfileList.SelectedItem;
            selected.Remove();
            RefreshProfiles();
        }

        private void PreviewRemoveClick(object sender, RoutedEventArgs e)
        {
            Profile selected = (Profile)PreviewProfileList.SelectedItem;
            selected.Remove();
            RefreshProfiles();
        }

        private void ReleaseProfileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LaunchRelease();
        }

        private void ReleaseProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReleaseProfileList.SelectedIndex < 0) ReleaseProfiles.FindIndex(p => p.uuid == Settings.selectedRelease);
        }

        private void PreviewProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PreviewProfileList.SelectedIndex < 0) PreviewProfiles.FindIndex(p => p.uuid == Settings.selectedPreview);
        }

        private void PreviewProfileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LaunchPreview();
        }

        private void SettingsUI()
        {
            new SettingsWindow(Settings, this).ShowDialog();
        }
        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsUI();
        }

        private void OnClose(object? sender, EventArgs e)
        {
            Debug.WriteLine("Closing MainWindow");
            Settings.windowSizes["MainWindow"] = new WindowSize(Height, Width);

            Settings.Save();
        }
    }
}
