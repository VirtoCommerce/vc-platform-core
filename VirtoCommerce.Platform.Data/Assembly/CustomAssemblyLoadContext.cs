using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;
using System.Reflection;

namespace VirtoCommerce.Platform.Data.Assembly
{
    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(String unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override System.Reflection.Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}
