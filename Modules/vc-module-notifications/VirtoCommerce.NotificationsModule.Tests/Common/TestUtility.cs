using System;
using System.IO;

namespace VirtoCommerce.NotificationsModule.Tests.Common
{
    public class TestUtility
    {
        public static string MapPath(string path)
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            if (baseDirectory.IndexOf(@"\bin\", StringComparison.Ordinal) != -1)
            {
                baseDirectory = baseDirectory.Remove(baseDirectory.IndexOf(@"\bin\", StringComparison.Ordinal));
            }
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public static string GetStringByPath(string path)
        {
            string fullPath = MapPath(path);

            string content = File.ReadAllText(fullPath);

            return content;
        }
    }
}
