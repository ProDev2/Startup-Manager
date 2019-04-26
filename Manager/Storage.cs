using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;

namespace Manager
{
    public class Storage<V>
    {
        private string FilePath;
        private Dictionary<string, V> Map;

        public string StorageFile
        {
            get
            {
                return FilePath;
            }
        }

        public Dictionary<string, V> StorageMap
        {
            get
            {
                return Map;
            }
        }

        public Storage(string filePath) : this(filePath, false)
        {
        }

        public Storage(string filePath, bool isRelative)
        {
            if (isRelative)
            {
                filePath = FileHelper.GetByRelativePath(filePath);
            }

            this.FilePath = filePath;

            Map = new Dictionary<string, V>();
            Load();
        }

        public void Clear()
        {
            if (Map != null)
            {
                lock (Map)
                {
                    Map.Clear();
                    Save();
                }
            }
        }

        public void Put(string key, V value)
        {
            if (key == null) return;

            if (Map != null)
            {
                lock (Map)
                {
                    Load();

                    if (Map.ContainsKey(key))
                    {
                        Map[key] = value;
                    } else
                    {
                        Map.Add(key, value);
                    }

                    Save();
                }
            }
        }

        public V Get(string key)
        {
            if (key == null) return default(V);

            if (Map != null)
            {
                lock (Map)
                {
                    Load();

                    Map.TryGetValue(key, out V value);
                    return value;
                }
            }
            return default(V);
        }

        public void Remove(string key)
        {
            if (key == null) return;

            if (Map != null)
            {
                lock (Map)
                {
                    Load();

                    Map.Remove(key);

                    Save();
                }
            }
        }

        public void RemoveValue(V value)
        {
            if (value == null) return;

            if (Map != null)
            {
                lock (Map)
                {
                    Load();

                    foreach (var item in Map.Where(kvp => kvp.Value != null && kvp.Value.Equals(value)).ToList())
                    {
                        Map.Remove(item.Key);
                    }

                    Save();
                }
            }
        }

        public void Save()
        {
            if (FilePath == null) return;

            string code = SaveAsCode();
            if (code == null) return;

            File.WriteAllText(FilePath, code);
        }

        public void Load()
        {
            if (FilePath == null) return;

            if (!File.Exists(FilePath)) return;

            string code = File.ReadAllText(FilePath);
            if (code == null) return;

            LoadFromCode(code);
        }

        public void LoadFromCode(String code)
        {
            if (code == null)
                return;
            if (Map == null)
                Map = new Dictionary<string, V>();

            try
            {
                lock (Map)
                {
                    var settings = new JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
                    settings.Formatting = Formatting.Indented;

                    JsonConvert.PopulateObject(code, Map, settings);
                }
            }
            catch (Exception e)
            {
            }
        }

        public string SaveAsCode()
        {
            if (Map == null)
                Map = new Dictionary<string, V>();

            try
            {
                lock (Map)
                {
                    var settings = new JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
                    settings.Formatting = Formatting.Indented;

                    return JsonConvert.SerializeObject(Map, settings);
                }
            } catch (Exception e)
            {
            }
            return null;
        }

        public class AllFieldsContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(p => base.CreateProperty(p, memberSerialization))
                    .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(f => base.CreateProperty(f, memberSerialization)))
                    .ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props;
            }
        }
    }
}
