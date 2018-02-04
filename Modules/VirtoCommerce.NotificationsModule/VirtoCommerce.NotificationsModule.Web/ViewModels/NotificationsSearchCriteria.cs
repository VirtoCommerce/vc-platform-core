using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.NotificationsModule.Web.ViewModels
{
    public class NotificationsSearchCriteria
    {
        public NotificationsSearchCriteria()
        {
            Take = 20;
        }

        public string Keyword { get; set; }

        /// <summary>
        /// Sorting expression property1:asc;property2:desc
        /// </summary>
        public string Sort { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}
