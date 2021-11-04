using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Mods;

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
            // Search through loadedMods for the mod attached to this assembly
            foreach (KeyValuePair<string, Mod> mod in (Dictionary<string, Mod>)typeof(ModManager).GetField("loadedMods", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ModManager.Instance))
                if (mod.Value.ModID == assemblyName)
                {
                    // Each key in loadedMods and InstalledModInfos is the path to the mod.
                    key = mod.Key;
                    break;
                }
            // Use the resulting folder to locate the ModInfo and obtain the mod's version
            var modInfo = key != "" ? ModManager.Instance.InstalledModInfos[key] : new ModInfo();
            GetVersion = modInfo.Version ?? "unavailable";
            return GetVersion;
        }
    }
}
