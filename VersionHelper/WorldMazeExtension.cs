using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace WorldMazesExtension
{
    public static class WorldMazeExtension
    {
        private static string GetVersion;
        public static string Version(this Type type)
        {
            if (GetVersion != null)
                return GetVersion;
            // Building with Unity forces the assembly name to be the ModID in ModConfig
            var assemblyName = type.Assembly.GetName().Name;
            var key = "";
            foreach (KeyValuePair<string, Mod> mod in (Dictionary<string, Mod>)typeof(ModManager).GetField("loadedMods", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ModManager.Instance))
            {
                if (mod.Value.ModID == assemblyName)
                {
                    // Each key in loadedMods is the path to the mod.
                    key = mod.Key;
                    break;
                }
            }
            // The game does not store version information, it is only used by Steam.
            // As such the version information must be read from the json directly.
            var json = File.ReadAllText(Directory.GetFiles(key, "modInfo.json").FirstOrDefault());
            GetVersion = JsonConvert.DeserializeObject<ModInfo>(json).Version;
            return GetVersion;
        }
    }

    public class ModInfo
    {
        public string Version;
    }
}
