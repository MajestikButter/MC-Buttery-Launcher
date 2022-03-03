using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MC_Buttery_Launcher
{
    public class Profile
    {
        public bool isRelease;
#pragma warning disable IDE1006 // Naming Styles
        public string name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        public string path;
        public string uuid;
        public Dictionary<string, string> subfolders;

        public Profile(bool isRelease, string name = "New Profile", string? path = null, string? uuid = null, Dictionary<string, string>? subfolders = null)
        {
            this.isRelease = isRelease;
            this.name = name;
            this.path = path ?? (isRelease ? MainWindow.DefaultReleaseProfilePath : MainWindow.DefaultPreviewProfilePath);
            this.uuid = uuid ?? Guid.NewGuid().ToString();
            this.subfolders = subfolders ?? new();
        }

        public void Save()
        {
            string profileStr = File.ReadAllText("./data/profiles.json");
            List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(profileStr)!;
            Profile? profile = profiles.Find(profile => profile.uuid == uuid);
            if (profile == null)
            {
                profiles.Add(this);
            }
            else
            {
                profile.name = name;
                profile.path = path;
            }
            File.WriteAllText("./data/profiles.json", JsonConvert.SerializeObject(profiles, Formatting.Indented));
        }

        public void Remove()
        {
            string profileStr = File.ReadAllText("./data/profiles.json");
            List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(profileStr)!;
            Profile? profile = profiles.Find(profile => profile.uuid == uuid);
            if (profile != null) profiles.Remove(profile);
            File.WriteAllText("./data/profiles.json", JsonConvert.SerializeObject(profiles, Formatting.Indented));
        }
    }
}