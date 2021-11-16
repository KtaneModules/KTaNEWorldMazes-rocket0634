using System.Collections.Generic;
using System.Linq;
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
            return _modVersions.ContainsKey(key) ? _modVersions[key] : GetVersionFromGame(key, ModSource);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetVersionFromGame(string id, object source)
        {
            // This is here as FirstOrDefault appears to throw a type exception if I try to compare Mod.ModID to string id.
            // This curiously does not happen when it is compared to ModSource.ModName.
            // Note that the project can still be built and ran regardless of the thrown error.
            ModSource modSource = source as ModSource;
            // Cache the field for the loadedMods Dictionary
            LoadedMods ??= typeof(ModManager).GetField("loadedMods", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
            // Search through loadedMods for the mod attached to this assembly
            // Each key in loadedMods and InstalledModInfos is the path to the mod
            var key = ((Dictionary<string, Mod>)LoadedMods.GetValue(ModManager.Instance)).FirstOrDefault(x => x.Value.ModID == modSource.ModName).Key ?? "";
            // Use the resulting folder to locate the ModInfo and obtain the mod's version
            var modInfo = key != "" ? ModManager.Instance.InstalledModInfos[key] : new ModInfo();
            _modVersions.Add(id, modInfo.Version ?? "Not found");
            return _modVersions[id];
        }
    }
}