using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Manager
{
    public static class RegisterHelper
    {
        public static string RedirectPath;

        public static RegistryKey OpenPathKey(bool openReadonly)
        {
            try
            {
                RegistryKey space = GetRegistrySpace();
                string path = GetRegistryPath();

                if (space != null && path != null)
                {
                    return space.OpenSubKey(path, !openReadonly);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public static bool HasEntry()
        {
            RegistryKey pathKey = OpenPathKey(true);
            if (pathKey == null)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            string registryKey = GetRegistryKey();
            if (registryKey == null || registryKey.Length <= 0)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            try
            {
                bool hasEntry = pathKey.GetValue(registryKey) != null;
                try { pathKey.Close(); } catch (Exception e) { }
                return hasEntry;
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try { pathKey.Close(); } catch (Exception e) { }
            return false;
        }

        public static bool HasEntryWithCurrentPath()
        {
            RegistryKey pathKey = OpenPathKey(true);
            if (pathKey == null)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            string registryKey = GetRegistryKey();
            if (registryKey == null || registryKey.Length <= 0)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            string currentPath = GetCurrentPath();
            if (currentPath == null)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            try
            {
                object entryValue = pathKey.GetValue(registryKey);
                if (entryValue == null)
                {
                    try { pathKey.Close(); } catch (Exception e) { }
                    return false;
                }

                if (entryValue is string[])
                {
                    foreach (string value in entryValue as string[])
                    {
                        if (value != null && value.Length > 0)
                        {
                            string valuePath = ValueToPath(value);
                            if (valuePath != null && valuePath.Length > 0 && Path.GetFullPath(valuePath).Equals(currentPath))
                            {
                                try { pathKey.Close(); } catch (Exception e) { }
                                return true;
                            }
                        }
                    }
                }

                if (entryValue is string)
                {
                    string value = entryValue as string;
                    if (value != null && value.Length > 0)
                    {
                        string valuePath = ValueToPath(value);
                        if (valuePath != null && valuePath.Length > 0 && Path.GetFullPath(valuePath).Equals(currentPath))
                        {
                            try { pathKey.Close(); } catch (Exception e) { }
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try { pathKey.Close(); } catch (Exception e) { }
            return false;
        }

        public static bool PutEntry()
        {
            RegistryKey pathKey = OpenPathKey(false);
            if (pathKey == null)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            string registryKey = GetRegistryKey();
            if (registryKey == null || registryKey.Length <= 0)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            string currentPath = GetCurrentPath();
            if (currentPath == null)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            try
            {
                string valuePath = PathToValue(currentPath);
                pathKey.SetValue(registryKey, valuePath);

                try { pathKey.Close(); } catch (Exception e) { }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try { pathKey.Close(); } catch (Exception e) { }
            return false;
        }

        public static bool RemoveEntry()
        {
            RegistryKey pathKey = OpenPathKey(false);
            if (pathKey == null)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            string registryKey = GetRegistryKey();
            if (registryKey == null || registryKey.Length <= 0)
            {
                try { pathKey.Close(); } catch (Exception e) { }
                return false;
            }

            try
            {
                pathKey.DeleteValue(registryKey);

                try { pathKey.Close(); } catch (Exception e) { }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try { pathKey.Close(); } catch (Exception e) { }
            return false;
        }

        public static string ValueToPath(string value)
        {
            if (value == null) return null;
            if (value != null && value.Length > 0)
                value = value.Trim();
            if (value != null && value.Length > 0 && value.StartsWith("\""))
                value = value.Remove(0, 1);
            if (value != null && value.Length > 0 && value.EndsWith("\""))
                value = value.Remove(value.Length - 1, 1);
            return value;
        }

        public static string PathToValue(string path)
        {
            if (path == null) return null;
            if (path != null && path.Length > 0)
                path = path.Trim();
            if (path.Length <= 0)
                return "";
            return "\"" + path + "\"";
        }

        public static RegistryKey GetRegistrySpace()
        {
            return Config.RegistrySpace;
        }

        public static string GetRegistryPath()
        {
            return Config.RegistryFolder;
        }

        public static string GetRegistryKey()
        {
            string key = null;

            if (key == null || key.Length <= 0)
                key = Config.RegistryKey;

            if (key == null || key.Length <= 0) return null;

            key = key.Trim();
            key = key.Replace(" ", "_");

            if (key == null || key.Length <= 0)
                return null;
            return key;
        }

        private static string GetCurrentPath()
        {
            string path = FileHelper.GetPath();

            try
            {
                if (RedirectPath != null && RedirectPath.Length > 0 && File.Exists(RedirectPath))
                    path = RedirectPath;
            } catch (Exception e)
            {
            }

            if (path == null || path.Length <= 0 || !File.Exists(path))
                path = FileHelper.GetPath();

            if (path == null || path.Length <= 0 || !File.Exists(path))
            {
                try
                {
                    Process process = Process.GetCurrentProcess();
                    if (process != null)
                        path = process.MainModule.FileName;
                } catch (Exception e)
                {
                }
            }

            if (path == null || path.Length <= 0 || !File.Exists(path))
                return null;

            try
            {
                return Path.GetFullPath(path);
            } catch (Exception e)
            {
                return path;
            }
        }

        public static void SetRedirectPath(string redirectPath, bool update)
        {
            if (redirectPath == null || redirectPath.Length <= 0)
                redirectPath = "";

            string currentPath = GetCurrentPath();

            RedirectPath = redirectPath;

            bool changed = false;
            try
            {
                changed = !currentPath.Equals(Path.GetFullPath(redirectPath));
            } catch (Exception e)
            {
            }

            if (changed && update)
                PutEntry();
        }
    }
}
