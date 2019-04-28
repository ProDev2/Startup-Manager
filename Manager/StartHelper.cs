using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public static class StartHelper
    {
        private static bool Initialized;

        public static bool IsInitialized()
        {
            return Initialized;
        }

        public static void Initialize()
        {
            Initialize(false);
        }

        public static void Initialize(bool reinitialize)
        {
            if (Initialized && !reinitialize) return;
            Initialized = true;

            try
            {
                RunInitialization();
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void RunInitialization()
        {
            //Run all initialization processes
            InitializeRegistryEntry();
        }

        private static void InitializeRegistryEntry()
        {
            //Initialize registry entry if needed
            try
            {
                ConfigStorage configStorage = ConfigStorage.Global;
                if (configStorage == null) return;

                bool setRegistryEntry = configStorage.GetBool(Config.ConfigEnableRegistryEntryKey, Config.ConfigDefaultEnableRegistryEntryValue);

                if (setRegistryEntry)
                {
                    bool hasEntry = RegisterHelper.HasEntryWithCurrentPath();
                    if (!hasEntry)
                        RegisterHelper.PutEntry();
                }

                if (!setRegistryEntry)
                {
                    bool hasEntry = RegisterHelper.HasEntry();
                    if (hasEntry)
                        RegisterHelper.RemoveEntry();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
