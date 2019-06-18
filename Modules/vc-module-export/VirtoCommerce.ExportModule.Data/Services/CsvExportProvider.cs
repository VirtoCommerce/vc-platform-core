using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public sealed class CsvExportProvider : IExportProvider
    {
        private static class DynamicMethodGenerator
        {
            public delegate object GetMethodDelegate(object source);

            /// <summary>
            /// Generate dynamic method to call get-accessor of property
            /// </summary>
            /// <param name="type">Type of object to access</param>
            /// <param name="propertyInfo"><see cref="PropertyInfo"/> for object property</param>
            /// <returns></returns>
            public static GetMethodDelegate CreateDynamicMethod(Type type, PropertyInfo propertyInfo)
            {
                var getMethodInfo = propertyInfo.GetGetMethod(true);
                var dynamicGet = CreateGetDynamicMethod(type);
                var getGenerator = dynamicGet.GetILGenerator();

                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Call, getMethodInfo);
                BoxIfNeeded(getMethodInfo.ReturnType, getGenerator);
                getGenerator.Emit(OpCodes.Ret);

                return (GetMethodDelegate)dynamicGet.CreateDelegate(typeof(GetMethodDelegate));
            }

            public static DynamicMethod CreateGetDynamicMethod(Type type)
            {
                return new DynamicMethod("DynamicGet", typeof(object), new Type[] { typeof(object) }, type, true);
            }

            public static void BoxIfNeeded(Type type, ILGenerator generator)
            {
                if (type.IsValueType)
                {
                    generator.Emit(OpCodes.Box, type);
                }
            }
        }

        public string TypeName => nameof(CsvExportProvider);
        public IExportProviderConfiguration Configuration { get; }
        public ExportedTypeMetadata Metadata { get; set; }

        private readonly StreamWriter _streamWriter;

        private readonly Dictionary<MemberInfo, DynamicMethodGenerator.GetMethodDelegate> cacheGetMethods = new Dictionary<MemberInfo, DynamicMethodGenerator.GetMethodDelegate>();

        public CsvExportProvider(Stream stream, IExportProviderConfiguration exportProviderConfiguration)
        {
            _streamWriter = new StreamWriter(stream);
            Configuration = exportProviderConfiguration;
        }

        /// <summary>
        /// Returns property value using precached dynamic method.
        /// It's about 15x faster than <see cref="PropertyInfo.GetValue(object)"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private object GetPropertyValue(object obj, MemberInfo memberInfo)
        {
            // Checks cache for dynamic method then add it if still not in
            if (!cacheGetMethods.ContainsKey(memberInfo))
            {
                lock (cacheGetMethods)
                {
                    if (!cacheGetMethods.ContainsKey(memberInfo))
                    {
                        var getMethod = DynamicMethodGenerator.CreateDynamicMethod(obj.GetType(), (PropertyInfo)memberInfo);
                        cacheGetMethods.Add(memberInfo, getMethod);
                    }
                }
            }
            return cacheGetMethods[memberInfo](obj);
        }

        public void WriteMetadata(ExportedTypeMetadata metadata)
        {
            var firstProperty = true;
            foreach (var propertyInfo in metadata.PropertiesInfo)
            {
                if (!firstProperty) _streamWriter.Write(";");
                _streamWriter.Write(@"""");
                _streamWriter.Write(string.IsNullOrEmpty(propertyInfo.ExportName) ? propertyInfo.Name : propertyInfo.ExportName);
                _streamWriter.Write(@"""");
                firstProperty = false;
            }
            _streamWriter.WriteLine();
        }

        public void WriteRecord(object objectToRecord)
        {
            var firstProperty = true;
            foreach (var propertyInfo in Metadata.PropertiesInfo)
            {
                if (!firstProperty) _streamWriter.Write(";");
                _streamWriter.Write(@"""");
                // Write value in culture invariant format. Convert is faster than string.format
                _streamWriter.Write(Convert.ToString(GetPropertyValue(objectToRecord, propertyInfo.MemberInfo), CultureInfo.InvariantCulture).Replace(@"""", @""""""));
                _streamWriter.Write(@"""");
                firstProperty = false;
            }
            _streamWriter.WriteLine();
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }
    }
}
