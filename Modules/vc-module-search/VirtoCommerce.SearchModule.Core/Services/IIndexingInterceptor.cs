using System;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
{
    /// <summary>
    /// Allows intercepting indexing operations.
    /// These operations can either be scheduled jobs or indexing actions from the user.
    /// This allows us to handle the entire batch of operations as on logical unit, 
    /// which might come in handy when you store different document types in a the same physical index.
    /// </summary>
    public interface IIndexingInterceptor
    {
        void OnBegin(IndexingOptions[] options);

        void OnEnd(IndexingOptions[] options, Exception exception = null);
    }
}
