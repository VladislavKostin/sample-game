using UnityEngine;
using System.IO;

namespace Game.Core
{
    public static class FileUtility
    {
        public static string GetOrCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        // public static string GetOrCreateSaveDataDirectory()
        // {
        //     return GetOrCreateDirectory(SaveDataDirectory);
        // }

        // public static string GetOrCreateProfileDirectory()
        // {
        //     return GetOrCreateDirectory(ProfilesDirectory);
        // }

        // public static string GetOrCreateGameSaveDirectory()
        // {
        //     return GetOrCreateDirectory(ProfilesDirectory);
        // }
    }
}
