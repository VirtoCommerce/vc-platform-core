using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ImageToolsModule.Core.Security
{
    public static class SecurityConstants
    {
        public static class Thumbnail
        {
            public const string Access = "thumbnail:access",
                 Create = "thumbnail:create",
                 Delete = "thumbnail:delete",
                 Update = "thumbnail:update",
                 Read = "thumbnail:read";
        }
    }
}
