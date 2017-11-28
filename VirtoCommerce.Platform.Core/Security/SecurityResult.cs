using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class SecurityResult : ValueObject
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
    }

    public class UserLockedResult : ValueObject
    {
        public bool Locked { get; set; }

        public UserLockedResult(bool locked)
        {
            Locked = locked;
        }
    }
}
