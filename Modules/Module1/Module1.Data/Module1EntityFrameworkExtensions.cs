using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Module1.Data
{
    public static class Module1EntityFrameworkExtensions
    {
        public static DbContextOptionsBuilder UseModule1(this DbContextOptionsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.ReplaceService<IModelCustomizer, PlatformDbContextCustomizer>();
        }
    }
}
