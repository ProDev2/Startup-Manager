using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public static class FileHelper
    {
        public static string GetPath()
        {
            try
            {
                return Directory.GetCurrentDirectory();
            } catch (Exception e)
            {
            }

            return null;
        }

        public static string GetByRelativePath(string relativePath)
        {
            string path = GetPath();
            if (path == null) return null;

            try
            {
                path = Path.GetFullPath(path);

                return Path.Combine(path, relativePath);
            } catch (Exception e)
            {
            }
            return null;
        }
    }
}
