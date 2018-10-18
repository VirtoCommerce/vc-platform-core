using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule2.Web.Model
{
    public class CustomerOrder2Entity : CustomerOrderEntity
    {
        public CustomerOrder2Entity()
        {
            Invoices = new NullCollection<InvoiceEntity>();
        }
        public string NewField { get; set; }
        public virtual ObservableCollection<InvoiceEntity> Invoices { get; set; }

        
        public override OrderOperation ToModel(OrderOperation operation)
        {
            var order2 = operation as CustomerOrder2;

            if (order2 != null)
            {
                order2.Invoices = this.Invoices.Select(x => x.ToModel(new Invoice())).OfType<Invoice>().ToList();
            }

            base.ToModel(operation);

            return operation;
        }

        public override OperationEntity FromModel(OrderOperation operation, PrimaryKeyResolvingMap pkMap)
        {
            var order2 = operation as CustomerOrder2;
            if (order2 != null)
            {
                if (order2.Invoices != null)
                {
                    this.Invoices = new ObservableCollection<InvoiceEntity>(order2.Invoices.Select(x => new InvoiceEntity().FromModel(x, pkMap)).OfType<InvoiceEntity>());
                }
            }

            base.FromModel(operation, pkMap);

            return this;
        }

        public override void Patch(OperationEntity operation)
        {
            var target = operation as CustomerOrder2Entity;
            if (target != null)
            {
                target.NewField = this.NewField;
                if (!this.Invoices.IsNullCollection())
                {
                    this.Invoices.Patch(target.Invoices, (sourceInvoice, targetInvoice) => sourceInvoice.Patch(targetInvoice));
                }
            }

            base.Patch(operation);
        }

    }
}
