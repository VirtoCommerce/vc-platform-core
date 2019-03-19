using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace VirtoCommerce.ContentModule.Web.Extensions
{
    //TODO: Move to shared library 
    public static class HostingEnviromentExtension
    {

        public static string MapPath(this IHostingEnvironment hostEnv, string path)
        {
            var result = hostEnv.WebRootPath;

            if (path.StartsWith("~/"))
            {
                result = Path.Combine(result, path.Replace("~/", string.Empty).Replace("/", "\\"));
            }
            else if (Path.IsPathRooted(path))
            {
                result = path;
            }

            return result;
        }
    }
}
