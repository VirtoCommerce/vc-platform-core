using System;
using System.Runtime.Serialization;
using VirtoCommerce.Platform.Core.Exceptions;

namespace VirtoCommerce.SearchModule.Core.Exceptions
{
    [Serializable]
    public class SearchException : PlatformException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SearchException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SearchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
