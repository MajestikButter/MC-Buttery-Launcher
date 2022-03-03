using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MC_Buttery_Launcher
{
    public class Settings 
    {
        public string selectedRelease;
        public string selectedPreview;
        public bool keepOpen;

        public Settings(string selectedRelease, string selectedPreview, bool keepOpen)
        {
            this.selectedRelease = selectedRelease;
            this.selectedPreview = selectedPreview;
            this.keepOpen = keepOpen;
        }

        public void Save()
        {
            File.WriteAllText("./data/settings.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}