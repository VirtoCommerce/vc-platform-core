using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Smidge.Models;

namespace VirtoCommerce.Platform.Web.Utility
{
    public class WebFileFolder
    {
        private readonly IHostingEnvironment _env;
        private readonly string _path;

        public WebFileFolder(IHostingEnvironment env, string path)
        {
            _env = env;
            _path = path;
        }

        public T[] AllWebFiles<T>(string pattern, SearchOption search) where T : IWebFile, new()
        {
            var fsPath = _path.Replace("~", _env.WebRootPath);
            return Directory.GetFiles(fsPath, pattern, search)
                .Select(f => new T
                {
                    FilePath = f.Replace(_env.WebRootPath, "~").Replace("\\", "/")
                }).ToArray();
        }
    }
}
