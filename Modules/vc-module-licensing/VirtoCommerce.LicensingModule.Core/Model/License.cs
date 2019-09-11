using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Core.Model
{
    public class License : AuditableEntity, IHasChangesHistory, ICloneable
    {
        public string Type { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ActivationCode { get; set; }

        #region IHasChangesHistory Members
        public ICollection<OperationLog> OperationsLog { get; set; }
        #endregion
        
        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as License;

            result.OperationsLog = OperationsLog?.Select(x => x.Clone()).OfType<OperationLog>().ToList();

            return result;
        }

        #endregion
    }
}
