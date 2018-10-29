using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Core.Services
{
    public interface IPaymentPlanService
    {
        Task<PaymentPlan[]> GetByIdsAsync(string[] planIds, string responseGroup = null);
        Task SavePlansAsync(PaymentPlan[] plans);
        Task DeleteAsync(string[] ids);
    }
}
