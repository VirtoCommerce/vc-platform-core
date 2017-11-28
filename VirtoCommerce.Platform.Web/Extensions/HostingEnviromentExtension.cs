using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace VirtoCommerce.Platform.Web.Extensions
{
    public static class HostingEnviromentExtension
    {
        public static string MapPath(this IHostingEnvironment hostEnv, string path)
        {
            var result = hostEnv.WebRootPath;

            if (path.StartsWith("~/"))
            {
                result = System.IO.Path.Combine(result, path.Replace("~/", string.Empty));
            }
            else if (Path.IsPathRooted(path))
            {
                result = path;
            }

            return result;
        }
    }
}
