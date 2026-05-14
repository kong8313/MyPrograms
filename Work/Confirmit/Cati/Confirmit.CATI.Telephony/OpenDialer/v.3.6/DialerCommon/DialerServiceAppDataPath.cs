using System;
using System.IO;
using System.Reflection;

namespace DialerCommon
{
    public static class DialerServiceAppDataPath
    {
        /// <summary>
        /// Gets the full path to the file containing required info.
        /// </summary>
        /// <returns>The full path to the file.</returns>
        public static string GetServiceAppDataPath()
        {
            string path = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).ToUpper();
            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar)); // path above bin directory
            path = path + Path.DirectorySeparatorChar + "App_Data" + Path.DirectorySeparatorChar;
            return path;
        }
    }
}