using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Common
{
    /// <summary>
    /// Helper class used for resolving model object primary keys when it presisted in persistent infrastructure
    /// Used in model to db model converters
    /// </summary>
    public class PrimaryKeyResolvingMap 
    {
        private Dictionary<IEntity, IEntity> _resolvingMap = new Dictionary<IEntity, IEntity>();
   
        public void AddPair(IEntity transientEntity, IEntity persistentEntity)
        {
            _resolvingMap[transientEntity] = persistentEntity;
        }

        public void ResolvePrimaryKeys()
        {
            foreach(var pair in _resolvingMap)
            {
                if(string.IsNullOrEmpty(pair.Key.Id) && !string.IsNullOrEmpty(pair.Value.Id))
                {
                    pair.Key.Id = pair.Value.Id;
                }
            }
        }
    }
}
