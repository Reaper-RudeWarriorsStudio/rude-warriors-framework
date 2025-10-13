using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    [Serializable]
    public class SaveProfile
    {
        public string ProfileName;
        public DateTime LastPlayed;
        public Dictionary<string, string> Data = new(); // key = data key, value = JSON

        public SaveProfile(string name)
        {
            ProfileName = name;
            LastPlayed = DateTime.Now;
        }
    }

    public interface ISaveProfileManager
    {
        SaveProfile CurrentProfile { get; }
        List<SaveProfile> GetAllProfiles();
        void CreateProfile(string name);
        void LoadProfile(string name);
        void DeleteProfile(string name);
        void Save(string key, object data);
        T Load<T>(string key);
        void SaveCurrentProfile();
    }

    /// <summary>
    /// Multi-slot save system built on top of DataService.
    /// Manages user profiles, stored as JSON files under persistentDataPath.
    /// </summary>
    public class SaveProfileManager : MonoBehaviour, ISaveProfileManager
    {
        private readonly string _profilesPath = Path.Combine(Application.persistentDataPath, "profiles");
        private IDataService _dataService;
        private SaveProfile _currentProfile;
        private List<SaveProfile> _allProfiles = new();

        public SaveProfile CurrentProfile => _currentProfile;

        private void Awake()
        {
            Directory.CreateDirectory(_profilesPath);
            _dataService = ServiceLocator.Get<IDataService>();
            LoadAllProfiles();
        }

        public List<SaveProfile> GetAllProfiles() => _allProfiles;

        public void CreateProfile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Debug.LogWarning("[SaveProfileManager] Invalid profile name.");
                return;
            }

            string path = GetProfilePath(name);
            if (File.Exists(path))
            {
                Debug.LogWarning($"[SaveProfileManager] Profile '{name}' already exists.");
                return;
            }

            _currentProfile = new SaveProfile(name);
            SaveCurrentProfile();
            LoadAllProfiles();
        }

        public void LoadProfile(string name)
        {
            string path = GetProfilePath(name);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SaveProfileManager] Profile '{name}' not found.");
                return;
            }

            string json = File.ReadAllText(path);
            _currentProfile = JsonUtility.FromJson<SaveProfile>(json);
            _currentProfile.LastPlayed = DateTime.Now;
        }

        public void DeleteProfile(string name)
        {
            string path = GetProfilePath(name);
            if (File.Exists(path))
                File.Delete(path);

            if (_currentProfile != null && _currentProfile.ProfileName == name)
                _currentProfile = null;

            LoadAllProfiles();
        }

        public void Save(string key, object data)
        {
            if (_currentProfile == null)
            {
                Debug.LogWarning("[SaveProfileManager] No profile loaded.");
                return;
            }

            string json = JsonUtility.ToJson(data);
            _currentProfile.Data[key] = json;
            SaveCurrentProfile();
        }

        public T Load<T>(string key)
        {
            if (_currentProfile == null)
            {
                Debug.LogWarning("[SaveProfileManager] No profile loaded.");
                return default;
            }

            if (_currentProfile.Data.TryGetValue(key, out var json))
                return JsonUtility.FromJson<T>(json);

            Debug.LogWarning($"[SaveProfileManager] Key '{key}' not found in profile '{_currentProfile.ProfileName}'.");
            return default;
        }

        public void SaveCurrentProfile()
        {
            if (_currentProfile == null) return;

            _currentProfile.LastPlayed = DateTime.Now;
            string path = GetProfilePath(_currentProfile.ProfileName);
            string json = JsonUtility.ToJson(_currentProfile, true);
            File.WriteAllText(path, json);
        }

        private void LoadAllProfiles()
        {
            _allProfiles.Clear();

            foreach (string file in Directory.GetFiles(_profilesPath, "*.json"))
            {
                string json = File.ReadAllText(file);
                var profile = JsonUtility.FromJson<SaveProfile>(json);
                _allProfiles.Add(profile);
            }
        }

        private string GetProfilePath(string name)
        {
            return Path.Combine(_profilesPath, $"{name}.json");
        }
    }
}
