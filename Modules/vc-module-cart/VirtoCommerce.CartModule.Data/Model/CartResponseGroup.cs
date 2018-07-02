using System;

namespace VirtoCommerce.CartModule.Data.Model
{
    [Flags]
    public enum CartResponseGroup
    {
        Default = 0,
        //WithTemplates = 1,
        //WithAttachments = 2,
        //WithRecipients = 4,
        Full = 7
    }
}
