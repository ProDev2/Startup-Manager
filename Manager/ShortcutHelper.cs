using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class ShortcutHelper
    {
        private static readonly string Extension = ".lnk";

        public string RedirectPath;
        public string RedirectTargetPath;

        public bool SaveShortcut()
        {
            string path = GetPath();

            string targetPath = GetTargetPath();
            string targetParentPath = targetPath != null ? Directory.GetParent(targetPath).FullName : null;

            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = shell.CreateShortcut(path) as IWshShortcut;
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = targetParentPath;
                shortcut.Save();
            }
            catch (Exception e)
            {
            }

            return path != null && System.IO.File.Exists(path);
        }

        public bool RemoveShortcut()
        {
            string path = GetPath();
            string targetPath = GetTargetPath();

            try
            {
                if (Path.GetExtension(path).Equals(Extension))
                    System.IO.File.Delete(path);
            }
            catch (Exception e)
            {
            }

            return path != null && !System.IO.File.Exists(path);
        }

        private string GetName()
        {
            string name = null;

            if (name == null || name.Length <= 0)
            {
                try
                {
                    Process process = Process.GetCurrentProcess();
                    if (process != null)
                    {
                        string processPath = process.MainModule.FileName;
                        string processName = Path.GetFileName(processPath);
                        if (processName != null && processName.Length > 0)
                            name = processName;
                    }
                }
                catch (Exception e)
                {
                }
            }

            if (name == null || name.Length <= 0)
            {
                try
                {
                    string path = Assembly.GetExecutingAssembly().Location;
                    string fileName = Path.GetFileName(path);
                    if (fileName != null && fileName.Length > 0)
                        name = fileName;
                }
                catch (Exception e)
                {
                }
            }

            return name;
        }

        public string GetPath()
        {
            string path = null;

            try
            {
                if (RedirectPath != null && RedirectPath.Length > 0)
                    path = RedirectPath;
            }
            catch (Exception e)
            {
            }

            if (path == null || path.Length <= 0)
            {
                string name = GetName();
                if (name != null && name.Length > 0)
                {
                    path = FileHelper.GetByRelativePath(name);
                }
            }

            if (path == null || path.Length <= 0 || !System.IO.File.Exists(path))
                return null;

            try
            {
                path = Path.ChangeExtension(path, Extension);
            }
            catch (Exception e)
            {
            }

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception e)
            {
                return path;
            }
        }

        public void SetRedirectPath(string redirectPath, bool update)
        {
            if (redirectPath == null || redirectPath.Length <= 0)
                redirectPath = "";

            string currentPath = GetPath();

            RedirectPath = redirectPath;

            bool changed = false;
            try
            {
                changed = !currentPath.Equals(Path.GetFullPath(redirectPath));
            }
            catch (Exception e)
            {
            }

            if (changed && update)
                SaveShortcut();
        }

        public string GetTargetPath()
        {
            string path = null;

            try
            {
                if (RedirectTargetPath != null && RedirectTargetPath.Length > 0 && System.IO.File.Exists(RedirectTargetPath))
                    path = RedirectTargetPath;
            }
            catch (Exception e)
            {
            }

            if (path == null || path.Length <= 0 || !System.IO.File.Exists(path))
                path = FileHelper.GetPath();

            if (path == null || path.Length <= 0 || !System.IO.File.Exists(path))
            {
                try
                {
                    Process process = Process.GetCurrentProcess();
                    if (process != null)
                        path = process.MainModule.FileName;
                }
                catch (Exception e)
                {
                }
            }

            if (path == null || path.Length <= 0 || !System.IO.File.Exists(path))
                return null;

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception e)
            {
                return path;
            }
        }

        public void SetRedirectTargetPath(string redirectTargetPath, bool update)
        {
            if (redirectTargetPath == null || redirectTargetPath.Length <= 0)
                redirectTargetPath = "";

            string currentPath = GetTargetPath();

            RedirectTargetPath = redirectTargetPath;

            bool changed = false;
            try
            {
                changed = !currentPath.Equals(Path.GetFullPath(redirectTargetPath));
            }
            catch (Exception e)
            {
            }

            if (changed && update)
                SaveShortcut();
        }
    }
}