using System;
using System.Runtime.Serialization;

namespace VirtoCommerce.SearchModule.Core.Exceptions
{
    [Serializable]
    public class SearchException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchException"/> class.
        /// </summary>
        public SearchException()
        {
        }

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
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected SearchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
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
