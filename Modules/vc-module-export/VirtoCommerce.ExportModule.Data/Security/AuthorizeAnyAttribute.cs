using System;
using Microsoft.AspNetCore.Authorization;

namespace VirtoCommerce.ExportModule.Data.Security
{
    /// <summary>
    /// Defines "OR"-combined policies to check
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeAnyAttribute : AuthorizeAttribute
    {
        public string[] Policies { get; }

        public AuthorizeAnyAttribute(params string[] policies)
        {
            Policies = policies;
        }
    }
}
