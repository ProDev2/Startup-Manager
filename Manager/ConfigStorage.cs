using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class ConfigStorage : Storage<string>
    {
        private static ConfigStorage Storage;

        public static ConfigStorage Global
        {
            get
            {
                if (Storage == null)
                    Storage = new ConfigStorage();

                return Storage;
            }
        }

        public ConfigStorage() : base(Config.RelativeConfigStoragePath, true)
        {
        }

        //Static access methods
        public static long GetUpdateInterval()
        {
            long defaultInterval = Config.ConfigDefaultIntervalValue;
            string stringDefaultInterval = defaultInterval.ToString();

            string value = GetValue(Config.ConfigIntervalKey, stringDefaultInterval);
            if (value == null) return default(long);

            try
            {
                return long.Parse(value);
            }
            catch (Exception e)
            {
            }

            try
            {
                return (long) int.Parse(value);
            }
            catch (Exception e)
            {
            }
            return default(long);
        }

        public static void SetUpdateInterval(long updateInterval)
        {
            SetValue(Config.ConfigIntervalKey, updateInterval.ToString());
        }

        public static bool GetBool(string key, bool defValue)
        {
            string value = GetValue(key, defValue.ToString());
            if (value == null) return default(bool);

            try
            {
                return bool.Parse(value);
            }
            catch (Exception e)
            {
            }
            return default(bool);
        }

        public static void SetBool(string key, bool value)
        {
            SetValue(key, value.ToString());
        }

        public static string GetValue(string key, string defValue)
        {
            string value = GetValue(key);
            if (value != null)
                return value;
            else
            {
                SetValue(key, defValue);

                return defValue;
            }
        }

        public static string GetValue(string key)
        {
            ConfigStorage storage = ConfigStorage.Global;
            if (storage == null) return default(String);

            return storage.Get(key);
        }

        public static void SetValue(string key, string value)
        {
            ConfigStorage storage = ConfigStorage.Global;
            if (storage == null) return;

            storage.Put(key, value);
        }
    }
}
