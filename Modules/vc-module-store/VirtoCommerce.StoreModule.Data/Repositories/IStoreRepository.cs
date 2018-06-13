using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Data.Model;
using dataModel = VirtoCommerce.StoreModule.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Repositories
{
	public interface IStoreRepository : IRepository
	{
        IQueryable<StoreEntity> Stores { get; }
        IQueryable<StorePaymentMethodEntity> StorePaymentMethods { get; }
        IQueryable<StoreShippingMethodEntity> StoreShippingMethods { get; }
        IQueryable<StoreTaxProviderEntity> StoreTaxProviders { get; }

        dataModel.StoreEntity[] GetStoresByIds(string[] ids);
    }
}
