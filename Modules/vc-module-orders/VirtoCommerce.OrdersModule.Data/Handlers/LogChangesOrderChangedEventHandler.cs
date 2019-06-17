using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoCompare;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using Address = VirtoCommerce.OrdersModule.Core.Model.Address;

namespace VirtoCommerce.OrdersModule.Data.Handlers
{
    public class LogChangesOrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        private readonly IMemberService _memberService;
        private readonly IChangeLogService _changeLogService;
        private static readonly string[] _observedProperties;

        static LogChangesOrderChangedEventHandler()
        {
            var operationPropNames = ReflectionUtility.GetPropertyNames<IOperation>(x => x.Status, x => x.Comment, x => x.Currency, x => x.Number, x => x.IsApproved);
            var orderPropNames = ReflectionUtility.GetPropertyNames<CustomerOrder>(x => x.DiscountAmount, x => x.Total, x => x.Fee, x => x.Number, x => x.TaxPercentRate, x => x.TaxTotal, x => x.TaxType);
            var shipmentPropNames = ReflectionUtility.GetPropertyNames<Shipment>(x => x.DiscountAmount, x => x.Total, x => x.Fee, x => x.Number, x => x.TaxPercentRate, x => x.TaxTotal, x => x.TaxType, x => x.Height, x => x.Length, x => x.MeasureUnit, x => x.Price, x => x.ShipmentMethodCode, x => x.ShipmentMethodOption, x => x.Weight, x => x.WeightUnit, x => x.Width);
            var paymentPropNames = ReflectionUtility.GetPropertyNames<PaymentIn>(x => x.DiscountAmount, x => x.Total, x => x.Number, x => x.TaxPercentRate, x => x.TaxTotal, x => x.TaxType, x => x.OuterId, x => x.AuthorizedDate, x => x.CapturedDate, x => x.Price, x => x.GatewayCode, x => x.IncomingDate, x => x.Purpose, x => x.VoidedDate);

            _observedProperties = operationPropNames.Concat(orderPropNames).Concat(shipmentPropNames).Concat(paymentPropNames).Distinct().ToArray();
        }

        public LogChangesOrderChangedEventHandler(IChangeLogService changeLogService, IMemberService memberService)
        {
            _changeLogService = changeLogService;
            _memberService = memberService;
        }

        public virtual async Task Handle(OrderChangedEvent message)
        {
            var operationLogs = new List<OperationLog>();
            foreach (var changedEntry in message.ChangedEntries.Where(x => x.EntryState == EntryState.Modified))
            {
                var originalOperations = changedEntry.OldEntry.GetFlatObjectsListWithInterface<IOperation>().Distinct();
                var modifiedOperations = changedEntry.NewEntry.GetFlatObjectsListWithInterface<IOperation>().Distinct();

                modifiedOperations.ToList().CompareTo(originalOperations.ToList(), EqualityComparer<IOperation>.Default,
                                                     (state, modified, original) => operationLogs.AddRange(GetChangedEntryOperationLogsAsync(new GenericChangedEntry<IOperation>(modified, original, state)).GetAwaiter().GetResult()));
            }
            if (!operationLogs.IsNullOrEmpty())
            {
                await _changeLogService.SaveChangesAsync(operationLogs.ToArray());
            }
        }

        protected virtual async Task<IEnumerable<OperationLog>> GetChangedEntryOperationLogsAsync(GenericChangedEntry<IOperation> changedEntry)
        {
            var result = new List<string>();

            if (changedEntry.EntryState == EntryState.Modified)
            {
                var diff = Comparer.Compare(changedEntry.OldEntry, changedEntry.NewEntry);

                var shipment = changedEntry.OldEntry as Shipment;
                if (shipment != null)
                {
                    result.AddRange(GetShipmentChanges(shipment, changedEntry.NewEntry as Shipment));
                    diff.AddRange(Comparer.Compare(shipment, changedEntry.NewEntry as Shipment));
                }

                var payment = changedEntry.OldEntry as PaymentIn;
                if (payment != null)
                {
                    result.AddRange(GetPaymentChanges(payment, changedEntry.NewEntry as PaymentIn));
                    diff.AddRange(Comparer.Compare(payment, changedEntry.NewEntry as PaymentIn));
                }

                var order = changedEntry.OldEntry as CustomerOrder;
                if (order != null)
                {
                    result.AddRange(await GetCustomerOrderChangesAsync(order, changedEntry.NewEntry as CustomerOrder));
                    diff.AddRange(Comparer.Compare(order, changedEntry.NewEntry as CustomerOrder));
                }
                var observedDifferences = diff.Join(_observedProperties, x => x.Name.ToLowerInvariant(), x => x.ToLowerInvariant(), (x, y) => x).ToArray();
                foreach (var difference in observedDifferences.Distinct(new DifferenceComparer()))
                {
                    result.Add($"The {changedEntry.OldEntry.OperationType} {changedEntry.NewEntry.Number} property '{difference.Name}' changed from '{difference.OldValue}' to  '{difference.NewValue}'");
                }
            }
            else if (changedEntry.EntryState == EntryState.Deleted)
            {
                result.Add($"The {changedEntry.NewEntry.OperationType} {changedEntry.NewEntry.Number} deleted");
            }
            else if (changedEntry.EntryState == EntryState.Added)
            {
                result.Add($"The new {changedEntry.NewEntry.OperationType} {changedEntry.NewEntry.Number} added");
            }
            return result.Select(x => GetLogRecord(changedEntry.NewEntry, x));
        }

        protected virtual async Task<IEnumerable<string>> GetCustomerOrderChangesAsync(CustomerOrder originalOrder, CustomerOrder modifiedOrder)
        {
            var result = new List<string>();
            if (originalOrder.EmployeeId != modifiedOrder.EmployeeId)
            {
                var employeeName = "none";
                if (!string.IsNullOrEmpty(modifiedOrder.EmployeeId))
                {
                    var employee = await _memberService.GetByIdAsync(modifiedOrder.EmployeeId) as Employee;
                    employeeName = employee != null ? employee.FullName : employeeName;
                }
                result.Add($"Order employee was changed  to '{employeeName}'");
            }
            result.AddRange(GetAddressChanges(originalOrder, originalOrder.Addresses, modifiedOrder.Addresses));
            return result.ToArray();
        }

        protected virtual IEnumerable<string> GetShipmentChanges(Shipment originalShipment, Shipment modifiedShipment)
        {
            var retVal = new List<string>();
            retVal.AddRange(GetAddressChanges(originalShipment, new[] { originalShipment.DeliveryAddress }, new[] { modifiedShipment.DeliveryAddress }));
            return retVal.ToArray();
        }

        protected virtual IEnumerable<string> GetPaymentChanges(PaymentIn payment, PaymentIn modifiedPayment)
        {
            var result = new List<string>();
            result.AddRange(GetAddressChanges(payment, new[] { payment.BillingAddress }, new[] { modifiedPayment.BillingAddress }));
            return result;
        }

        protected virtual IEnumerable<string> GetAddressChanges(IOperation operation, IEnumerable<Address> originalAddress, IEnumerable<Address> modifiedAddress)
        {
            var result = new List<string>();
            modifiedAddress.Where(x => x != null).ToList().CompareTo(originalAddress.Where(x => x != null).ToList(), EqualityComparer<Address>.Default,
                                      (state, source, target) =>
                                      {
                                          if (state == EntryState.Added)
                                          {
                                              result.Add($"The address '{StringifyAddress(target)}' for {operation.OperationType} {operation.Number} added");
                                          }
                                          else if (state == EntryState.Deleted)
                                          {
                                              result.Add($"The address '{StringifyAddress(target)}' for {operation.OperationType} {operation.Number} deleted");
                                          }
                                      });
            return result;
        }

        protected virtual string StringifyAddress(Address address)
        {
            var result = "";
            if (address != null)
            {
                return string.Join(", ", typeof(Address).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                   .OrderBy(p => p.Name)
                                   .Select(p => p.GetValue(address))
                                   .Where(x => x != null));
            }
            return result;
        }

        protected virtual OperationLog GetLogRecord(IOperation operation, string template)
        {
            var result = new OperationLog
            {
                ObjectId = operation.Id,
                ObjectType = operation.GetType().Name,
                OperationType = EntryState.Modified,
                Detail = template
            };
            return result;

        }
    }

    internal class DifferenceComparer : EqualityComparer<Difference>
    {
        public override bool Equals(Difference x, Difference y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public override int GetHashCode(Difference obj)
        {
            var result = string.Join(":", obj.Name, obj.NewValue, obj.OldValue);
            return result.GetHashCode();
        }
    }
}

