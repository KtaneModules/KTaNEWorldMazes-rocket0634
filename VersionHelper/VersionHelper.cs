using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Assets.Scripts.Mods;
using UnityEngine;

namespace WorldMazesExtension
{
    /// <summary>
    /// Version Helper extension for obtaining the Mod's version, using a namespace dedicated to each mod.
    /// </summary>
    public static class VersionHelper
    {
        private static FieldInfo LoadedMods;
        private static Dictionary<string, string> _modVersions = new();
        public static string Version(this Component ModSource) => Application.isEditor ? "UnityEditor" : GetVersion(ModSource);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetVersion(object ModSource)
        {
            string key = ModSource is not ModSource modSource ? default : modSource.ModName;
            // Return Unavailable if the object is not a ModSource or if its ModName is null
            if (key == null)
                return "Unavailable";
            // Return the version if it has already been grabbed.
            return _modVersions.ContainsKey(key) ? _modVersions[key] : GetVersionFromGame(key);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetVersionFromGame(string id)
        {
            // Cache the field for the loadedMods Dictionary
            LoadedMods ??= typeof(ModManager).GetField("loadedMods", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            // Search through loadedMods for the mod attached to this assembly
            // Each key in loadedMods and InstalledModInfos is the path to the mod
            string key = "";
            foreach (var entry in (Dictionary<string, Mod>)LoadedMods.GetValue(ModManager.Instance))
            {
                if (entry.Value.ModID == id)
                {
                    key = entry.Key;
                    break;
                }
            }
            // Use the resulting folder to locate the ModInfo and obtain the mod's version
            if (!ModManager.Instance.InstalledModInfos.TryGetValue(key, out ModInfo modInfo))
                modInfo = default;
            _modVersions.Add(id, modInfo.Version ?? "Not found");
            return _modVersions[id];
        }
    }
}