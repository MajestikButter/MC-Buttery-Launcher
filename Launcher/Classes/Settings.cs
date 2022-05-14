using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MC_Buttery_Launcher
{
    public class Settings 
    {
        public string? selectedRelease;
        public string? selectedPreview;
        public bool keepOpen;
        public Dictionary<string, WindowSize> windowSizes;

        public Settings(string? selectedRelease, string? selectedPreview, bool keepOpen, Dictionary<string, WindowSize> windowSizes)
        {
            this.selectedRelease = selectedRelease;
            this.selectedPreview = selectedPreview;
            this.keepOpen = keepOpen;
            this.windowSizes = windowSizes;
        }

        public void Save()
        {
            File.WriteAllText("./data/settings.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}