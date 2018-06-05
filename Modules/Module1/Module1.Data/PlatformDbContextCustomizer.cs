using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Module1.Data
{
    public class PlatformDbContextCustomizer : RelationalModelCustomizer
    {
        public PlatformDbContextCustomizer(ModelCustomizerDependencies dependencies)
            : base(dependencies)
        {
        }

        public override void Customize(ModelBuilder builder, DbContext context)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            builder.Entity<SettingEntity2>(entity =>
            {
                entity.ToTable("PlatformSetting");
            });

            base.Customize(builder, context);
        }
    }
}
