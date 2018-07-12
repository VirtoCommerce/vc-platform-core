using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.OrderModule.Core.Model;
using VirtoCommerce.OrderModule.Core.Services;
using VirtoCommerce.OrderModule.Core.Model.Search;

namespace VirtoCommerce.OrderModule.Web.ExportImport
{
    public sealed class BackupObject
    {
        public BackupObject()
        {
            CustomerOrders = new List<CustomerOrder>();
        }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
    }

    public sealed class OrderExportImport
    {
        private readonly ICustomerOrderSearchService _customerOrderSearchService;
        private readonly ICustomerOrderService _customerOrderService;

        public OrderExportImport(ICustomerOrderSearchService customerOrderSearchService, ICustomerOrderService customerOrderService)
        {
            _customerOrderSearchService = customerOrderSearchService;
            _customerOrderService = customerOrderService;
        }

        public void DoExport(Stream backupStream, Action<ExportImportProgressInfo> progressCallback)
        {
            
        }

        public void DoImport(Stream backupStream, Action<ExportImportProgressInfo> progressCallback)
        {
            
        }

        
    }
}
