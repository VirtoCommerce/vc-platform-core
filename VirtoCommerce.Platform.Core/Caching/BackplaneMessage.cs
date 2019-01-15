namespace VirtoCommerce.Platform.Core.Caching
{
    /// <summary>
    /// Defines the possible actions of the backplane message.
    /// </summary>
    public enum BackplaneAction : byte
    {
        /// <summary>
        /// Default value is invalid to ensure we are not getting wrong results.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The changed action.
        /// <see cref="CacheItemChangedEventAction"/>
        /// </summary>
        Changed,

        /// <summary>
        /// The clear action.
        /// </summary>
        Clear,

        /// <summary>
        /// The clear region action.
        /// </summary>
        ClearRegion,

        /// <summary>
        /// If the cache item has been removed.
        /// </summary>
        Removed
    }

    /// <summary>
    /// The enum defines the actual operation used to change the value in the cache.
    /// </summary>
    public enum CacheItemChangedEventAction : byte
    {
        /// <summary>
        /// Default value is invalid to ensure we are not getting wrong results.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// If Put was used to change the value.
        /// </summary>
        Put,

        /// <summary>
        /// If Add was used to change the value.
        /// </summary>
        Add,

        /// <summary>
        /// If Update was used to change the value.
        /// </summary>
        Update
    }

    public class BackplaneMessage
    {
        public BackplaneMessage()
        {

        }

        public BackplaneMessage(byte[] owner, BackplaneAction action)
        {
            OwnerIdentity = owner;
            Action = action;
        }

        public BackplaneMessage(byte[] owner, BackplaneAction action, string key)
            : this(owner, action)
        {
            Key = key;
        }

        public BackplaneMessage(byte[] owner, BackplaneAction action, string key, string region)
            : this(owner, action, key)
        {
            Region = region;
        }

        public BackplaneMessage(byte[] owner, BackplaneAction action, string key, CacheItemChangedEventAction changeAction)
            : this(owner, action, key)
        {
            ChangeAction = changeAction;
        }

        public BackplaneMessage(byte[] owner, BackplaneAction action, string key, string region, CacheItemChangedEventAction changeAction)
            : this(owner, action, key, region)
        {
            ChangeAction = changeAction;
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public BackplaneAction Action { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the owner identity.
        /// </summary>
        /// <value>The owner identity.</value>
        public byte[] OwnerIdentity { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>The region.</value>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the cache action.
        /// </summary>
        public CacheItemChangedEventAction ChangeAction { get; set; }
    }
}
