using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Mods;
using UnityEngine;

namespace WorldMazesExtension
{
    /// <summary>
    /// Version Helper extension for obtaining the Mod's version, using a namespace dedicated to each mod.
    /// </summary>
    public static class WorldMazesExtension
    {
        private static string _version;
        private static FieldInfo LoadedMods;
        private static Dictionary<string, string> _modVersions = new();
        public static string Version(this Component ModSource) => _version ??= (Application.isEditor ? "UnityEditor" : GetVersion(ModSource));

        private static string GetVersion(object ModSource)
        {
            if (ModSource is not ModSource modSource)
                return "Unavailable";
            // Return the version if it has already been grabbed
            string id = modSource.ModName;
            if (_modVersions.ContainsKey(id))
                return _modVersions[id];
            // Cache the field for the loadedMods Dictionary
            if (LoadedMods == null)
                LoadedMods = typeof(ModManager).GetField("loadedMods", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            // Search through loadedMods for the mod attached to this assembly
            // Each key in loadedMods and InstalledModInfos is the path to the mod
            var key = ((Dictionary<string, Mod>)LoadedMods.GetValue(ModManager.Instance)).FirstOrDefault(x => x.Value.ModID == modSource.ModName).Key ?? "";
            // Use the resulting folder to locate the ModInfo and obtain the mod's version
            var modInfo = key != "" ? ModManager.Instance.InstalledModInfos[key] : new ModInfo();
            _modVersions.Add(id, modInfo.Version ?? "Unavailable");
            return _modVersions[id];
        }
    }
}