using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Game.Core
{
    public class PlayerProfileSystem : ManagedSystem
    {
        public string SaveDataDirectory => Path.Combine(Application.persistentDataPath, "SaveData");
        public string ProfilesDirectory => Path.Combine(SaveDataDirectory, "Profiles");
        public string CurrentProfileGameSavesDirectory => Path.Combine(ProfilesDirectory, CurrentProfile.ID, "GameSaves");
        public string CurrentProfileDirectory => Path.Combine(ProfilesDirectory, CurrentProfile.ID);
        public string CurrentProfileDataPath => Path.Combine(CurrentProfileDirectory, "ProfileData.json");

        public PlayerProfile CurrentProfile { get; private set; }
        public List<PlayerProfile> AllProfiles = new();

        public event Action OnProfileChange;

        private void Start()
        {
            LoadAllProfiles();
        }

        private void LoadAllProfiles()
        {
            AllProfiles.Clear();
            string[] profileDirectories = Directory.GetDirectories(FileUtility.GetOrCreateDirectory(ProfilesDirectory));
            foreach (var directory in profileDirectories)
            {
                // Logger.LogDebug($"Checking directory: {directory}.");
                PlayerProfile profile = LoadProfileFromDirectory(directory);
                if (profile != null)
                {
                    AllProfiles.Add(profile);
                }
            }
            if (AllProfiles.Count == 0)
            {
                CreateDefaultProfile();
            }
            else
            {
                AllProfiles.Sort((a, b) => b.LastSelectedTime.CompareTo(a.LastSelectedTime));
                SelectProfile(AllProfiles[0]);
            }
        }

        private PlayerProfile LoadProfileFromDirectory(string directory)
        {
            return null;
            // Redacted code using third party solution
        }

        public void SelectProfile(PlayerProfile profile)
        {
            if (AllProfiles.Contains(profile))
            {
                CurrentProfile = profile;
                profile.LastSelectedTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                // Logger.LogDebug($"Selected [{profile.Name}] player profile. Last selected time: [{profile.LastSelectedTime}]");
                SaveCurrentProfile();
                OnProfileChange?.Invoke();
                Logger.LogImportant($"Selected [{profile.Name}] player profile.");
            }
        }

        public void SaveCurrentProfile()
        {
            SaveProfile(CurrentProfile);
        }

        public void SaveProfile(PlayerProfile profile)
        {
            // Redacted code using third party solution
        }

        private void CreateDefaultProfile()
        {
            Logger.LogImportant($"Creating default player profile");
            var defaultProfile = new PlayerProfile
            {
                Name = "Player1",
                ID = Guid.NewGuid().ToString(),
                LastSelectedTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds()
            };
            SaveProfile(defaultProfile);
            SelectProfile(defaultProfile);
        }
    }

    [Serializable]
    public class PlayerProfile
    {
        public string Name;
        public string ID;
        public bool CompletedTutorial;
        public long LastSelectedTime;
    }
}
