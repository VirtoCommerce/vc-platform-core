using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class IndexDocument : Entity
    {
        public IndexDocument(string id)
        {
            Id = id;
        }

        public IList<IndexDocumentField> Fields { get; set; } = new List<IndexDocumentField>();

        public virtual void Merge(IndexDocument doc)
        {
            foreach (var field in doc.Fields)
            {
                Add(field);
            }
        }

        public virtual void Add(IndexDocumentField field)
        {
            var existingField = Fields.FirstOrDefault(f => f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

            if (existingField != null)
            {
                existingField.Merge(field);
            }
            else
            {
                Fields.Add(field);
            }
        }
    }
}
