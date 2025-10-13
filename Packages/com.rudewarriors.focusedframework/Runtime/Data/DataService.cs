using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace RudeWarriors.Framework.Core
{
    // ---------- Interfaces ----------
    public interface IDataStore
    {
        void Save<T>(string key, T value);
        bool TryLoad<T>(string key, out T value);
        void Delete(string key);
        void Clear();
    }

    public interface IDataService
    {
        void SavePersistent<T>(string key, T data);
        bool TryLoadPersistent<T>(string key, out T data);

        void SaveRuntime<T>(string key, T data);
        bool TryLoadRuntime<T>(string key, out T data);
    }

    // ---------- Implementations ----------
    /// <summary>
    /// Simple in-memory key/value store for temporary runtime data.
    /// </summary>
    public class MemoryStore : IDataStore
    {
        private readonly Dictionary<string, object> _cache = new();

        public void Save<T>(string key, T value) => _cache[key] = value;

        public bool TryLoad<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out var obj) && obj is T cast)
            {
                value = cast;
                return true;
            }
            value = default;
            return false;
        }

        public void Delete(string key) => _cache.Remove(key);

        public void Clear() => _cache.Clear();
    }

    /// <summary>
    /// File-based store using JSON serialization under Application.persistentDataPath.
    /// </summary>
    public class JsonFileStore : IDataStore
    {
        private readonly string _rootPath;

        public JsonFileStore(string subPath = "data")
        {
            _rootPath = Path.Combine(Application.persistentDataPath, subPath);
            Directory.CreateDirectory(_rootPath);
        }

        private string GetPath(string key) => Path.Combine(_rootPath, key + ".json");

        public void Save<T>(string key, T value)
        {
            string path = GetPath(key);
            string json = JsonUtility.ToJson(value, true);
            File.WriteAllText(path, json);
        }

        public bool TryLoad<T>(string key, out T value)
        {
            string path = GetPath(key);
            if (!File.Exists(path))
            {
                value = default;
                return false;
            }

            string json = File.ReadAllText(path);
            value = JsonUtility.FromJson<T>(json);
            return true;
        }

        public void Delete(string key)
        {
            string path = GetPath(key);
            if (File.Exists(path))
                File.Delete(path);
        }

        public void Clear()
        {
            if (Directory.Exists(_rootPath))
                Directory.Delete(_rootPath, true);

            Directory.CreateDirectory(_rootPath);
        }
    }

    // ---------- Orchestrator ----------
    /// <summary>
    /// High-level data access layer combining runtime and persistent stores.
    /// Register this in the ServiceLocator for global access.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IDataStore _persistent;
        private readonly IDataStore _runtime;

        public DataService()
        {
            _persistent = new JsonFileStore();
            _runtime = new MemoryStore();
        }

        public void SavePersistent<T>(string key, T data) => _persistent.Save(key, data);
        public bool TryLoadPersistent<T>(string key, out T data) => _persistent.TryLoad(key, out data);

        public void SaveRuntime<T>(string key, T data) => _runtime.Save(key, data);
        public bool TryLoadRuntime<T>(string key, out T data) => _runtime.TryLoad(key, out data);
    }
}
