using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Services;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class SubscriptionBuilderImpl : ISubscriptionBuilder
    {
        private Subscription _subscription;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly ISettingsManager _settingsManager;
        private readonly IUniqueNumberGenerator _uniqueNumberGenerator;
        private readonly IStoreService _storeService;
        public SubscriptionBuilderImpl(IPaymentPlanService paymentPlanService, ISettingsManager settingsManager, IStoreService storeService, IUniqueNumberGenerator uniqueNumberGenerator)
        {
            _paymentPlanService = paymentPlanService;
            _settingsManager = settingsManager;
            _uniqueNumberGenerator = uniqueNumberGenerator;
            _storeService = storeService;
        }

        #region ISubscriptionBuilder Members
        public virtual Subscription Subscription
        {
            get
            {
                return _subscription;
            }
        }

        public virtual async Task<ISubscriptionBuilder> ActualizeAsync()
        {
            if (!Subscription.IsCancelled)
            {
                //Calculate balance from linked orders
                if (!Subscription.CustomerOrders.IsNullOrEmpty())
                {
                    Subscription.Balance = 0m;
                    var allNotCanceledOrders = Subscription.CustomerOrders.Where(x => !x.IsCancelled);
                    var ordersGrandTotal = allNotCanceledOrders.Sum(x => Math.Round(x.Total, 2, MidpointRounding.AwayFromZero));
                    var paidPaymentStatuses = new[] { PaymentStatus.Authorized, PaymentStatus.Paid };
                    var paidTotal = allNotCanceledOrders.SelectMany(x => x.InPayments).Where(x => !x.IsCancelled && paidPaymentStatuses.Contains(x.PaymentStatus)).Sum(x => x.Sum);

                    Subscription.Balance = ordersGrandTotal - paidTotal;
                }

                //Evaluate current subscription status
                Subscription.SubscriptionStatus = SubscriptionStatus.Active;
                var now = DateTime.UtcNow;
                if (Subscription.TrialSart != null)
                {
                    Subscription.SubscriptionStatus = SubscriptionStatus.Trialing;
                    if (Subscription.TrialEnd != null && now >= Subscription.TrialEnd)
                    {
                        Subscription.SubscriptionStatus = SubscriptionStatus.Active;
                    }
                }

                if (Subscription.SubscriptionStatus == SubscriptionStatus.Unpaid)
                {
                    var delay = await _settingsManager.GetValueAsync("Subscription.PastDue.Delay", 7);
                    //WORKAROUND: because  dont have time when subscription becomes unpaid we are use last modified timestamps
                    if (Subscription.ModifiedDate.Value.AddDays(delay) > now)
                    {
                        Subscription.SubscriptionStatus = SubscriptionStatus.PastDue;
                    }
                }

                if (Subscription.SubscriptionStatus != SubscriptionStatus.Trialing && Subscription.Balance > 0)
                {
                    Subscription.SubscriptionStatus = SubscriptionStatus.Unpaid;
                }

                if (Subscription.EndDate.HasValue && now >= Subscription.EndDate)
                {
                    CancelSubscription("Completed with time expiration");
                }

            }
            return this;
        }

        public virtual async Task<Subscription> TryCreateSubscriptionFromOrderAsync(CustomerOrder order)
        {
            Subscription retVal = null;
            PaymentPlan paymentPlan = null;
            if (!string.IsNullOrEmpty(order.ShoppingCartId))
            {
                //Retrieve payment plan with id as the same order original shopping cart id
                paymentPlan = (await _paymentPlanService.GetByIdsAsync(new[] { order.ShoppingCartId })).FirstOrDefault();
            }
            if (paymentPlan == null)
            {
                //Try to create subscription if order line item with have defined PaymentPlan
                //TODO: On the right must also be taken into account when the situation in the order contains items with several different plans
                paymentPlan = (await _paymentPlanService.GetByIdsAsync(order.Items.Select(x => x.ProductId).ToArray())).FirstOrDefault();
            }

            //Generate numbers for new subscriptions
            var store = await _storeService.GetByIdAsync(order.StoreId, StoreResponseGroup.StoreInfo.ToString());
            var numberTemplate = store.Settings.GetSettingValue("Subscription.SubscriptionNewNumberTemplate", "SU{0:yyMMdd}-{1:D5}");

            if (paymentPlan != null)
            {
                var now = DateTime.UtcNow;
                //There need to make "prototype" for future orders which will be created by subscription schedule information
                retVal = AbstractTypeFactory<Subscription>.TryCreateInstance<Subscription>();
                retVal.StoreId = order.StoreId;
                retVal.Number = _uniqueNumberGenerator.GenerateNumber(numberTemplate);
                retVal.CustomerOrderPrototype = CloneCustomerOrder(order);
                //Need to prevent subscription creation for prototype order in CreateSubscriptionHandler
                retVal.CustomerOrderPrototype.Number = retVal.Number;
                retVal.CustomerOrderPrototype.IsPrototype = true;
                retVal.CustomerId = order.CustomerId;
                retVal.CustomerName = order.CustomerName;
                retVal.Interval = paymentPlan.Interval;
                retVal.IntervalCount = paymentPlan.IntervalCount;
                retVal.StartDate = now;
                retVal.CurrentPeriodStart = now;
                retVal.TrialPeriodDays = paymentPlan.TrialPeriodDays;
                retVal.SubscriptionStatus = SubscriptionStatus.Active;
                retVal.CurrentPeriodEnd = GetPeriodEnd(now, paymentPlan.Interval, paymentPlan.IntervalCount);
                if (retVal.TrialPeriodDays > 0)
                {
                    retVal.TrialSart = now;
                    retVal.TrialEnd = GetPeriodEnd(now, PaymentInterval.Days, retVal.TrialPeriodDays);
                    //For trial need to shift start and end period  
                    retVal.CurrentPeriodStart = retVal.TrialEnd;
                    retVal.CurrentPeriodEnd = GetPeriodEnd(retVal.TrialEnd.Value, paymentPlan.Interval, paymentPlan.IntervalCount);
                }

                retVal.CustomerOrders = new List<CustomerOrder>
                {
                    order
                };
            }
            return retVal;
        }

        public virtual ISubscriptionBuilder TakeSubscription(Subscription subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            _subscription = subscription;
            return this;
        }

        public virtual async Task<CustomerOrder> TryToCreateRecurrentOrderAsync(bool forceCreation = false)
        {
            CustomerOrder retVal = null;
            if (!Subscription.IsCancelled)
            {
                var now = DateTime.UtcNow;
                if (forceCreation || now >= Subscription.CurrentPeriodEnd)
                {
                    Subscription.CurrentPeriodStart = now;
                    Subscription.CurrentPeriodEnd = GetPeriodEnd(now, Subscription.Interval, Subscription.IntervalCount);

                    retVal = CloneCustomerOrder(Subscription.CustomerOrderPrototype);
                    retVal.Status = "New";
                    retVal.IsPrototype = false;
                    retVal.SubscriptionId = Subscription.Id;
                    retVal.SubscriptionNumber = Subscription.Number;
                    foreach (var payment in retVal.InPayments)
                    {
                        payment.PaymentStatus = PaymentStatus.New;
                    }
                    foreach (var shipment in retVal.Shipments)
                    {
                        shipment.Status = "New";
                    }

                    if (_subscription.CustomerOrders == null)
                        _subscription.CustomerOrders = new List<CustomerOrder>();

                    _subscription.CustomerOrders.Add(retVal);
                    await ActualizeAsync();
                }
            }
            return retVal;
        }


        public virtual ISubscriptionBuilder CancelSubscription(string reason)
        {
            if (!Subscription.IsCancelled)
            {
                Subscription.IsCancelled = true;
                Subscription.CancelReason = reason;
                Subscription.CancelledDate = DateTime.UtcNow;
                Subscription.SubscriptionStatus = SubscriptionStatus.Cancelled;
            }
            return this;
        }

        #endregion


        protected virtual CustomerOrder CloneCustomerOrder(CustomerOrder order)
        {
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var serializationSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ObjectCreationHandling = ObjectCreationHandling.Replace };

            var retVal = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(order, serializationSettings), order.GetType(), serializationSettings) as CustomerOrder;

            //Reset all tracking numbers and ids
            foreach (var entity in retVal.GetFlatObjectsListWithInterface<IEntity>())
            {
                entity.Id = null;
                var operation = entity as IOperation;
                if (operation != null)
                {
                    operation.Number = null;
                    operation.Status = null;
                }
            }
            //Reset all audit info
            foreach (var auditableEntity in retVal.GetFlatObjectsListWithInterface<IAuditable>())
            {
                auditableEntity.CreatedBy = null;
                auditableEntity.CreatedDate = default(DateTime);
                auditableEntity.ModifiedBy = null;
                auditableEntity.ModifiedDate = null;
            }
            return retVal;
        }

        private DateTime GetPeriodEnd(DateTime periodStart, PaymentInterval interval, int intervalCount)
        {
            var retVal = periodStart;
            if (interval == PaymentInterval.Days)
            {
                retVal = retVal.AddDays(Math.Max(1, intervalCount));
            }
            else if (interval == PaymentInterval.Months)
            {
                retVal = retVal.AddMonths(Math.Max(1, intervalCount));
            }
            else if (interval == PaymentInterval.Weeks)
            {
                retVal = retVal.AddYears(Math.Max(1, intervalCount));
            }
            else if (interval == PaymentInterval.Weeks)
            {
                retVal = retVal.AddDays(7 * Math.Max(1, intervalCount));
            }
            return retVal;
        }
    }
}
