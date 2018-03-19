using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Tests.Model
{
    public class CustomerOrder : AuditableEntity
    {
        public string Number { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public decimal ShippingTotal { get; set; }
        public decimal Total { get; set; }
        public decimal FeeTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }

        public IEnumerable<Shipment> Shipments { get; set; }
        public IEnumerable<LineItem> Items { get; set; }
    }
}
