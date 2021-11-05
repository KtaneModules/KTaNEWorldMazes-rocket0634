using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Mods;
using UnityEngine;

namespace WorldMazesExtension
{
    public static class WorldMazesExtension
    {
        private static string _version;
        public static string Version(this Component ModSource) => _version ??= (Application.isEditor ? "UnityEditor" : GetVersion(ModSource));

        private static string GetVersion(object ModSource)
        {
            if (ModSource is not ModSource modSource)
                return "Unavailable";
            // Search through loadedMods for the mod attached to this assembly
            // Each key in loadedMods and InstalledModInfos is the path to the mod
            var key = ((Dictionary<string, Mod>)typeof(ModManager).GetField("loadedMods", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ModManager.Instance)).FirstOrDefault(x => x.Value.ModID == modSource.ModName).Key ?? "";
            // Use the resulting folder to locate the ModInfo and obtain the mod's version
            var modInfo = key != "" ? ModManager.Instance.InstalledModInfos[key] : new ModInfo();
            _version = modInfo.Version ?? "Unavailable";
            return _version;
        }
    }
}
