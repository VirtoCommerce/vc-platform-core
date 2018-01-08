using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.Platform.Data.PushNotifications
{
    public class PushNotificationHub : Hub, IPushNotificationManager
    {
        private readonly List<PushNotification> _innerList;
        private object _lockObject = new object();

        public PushNotificationSearchResult SearchNotifies(string userId, PushNotificationSearchCriteria criteria)
        {
            var query = _innerList.OrderByDescending(x => x.Created).Where(x => x.Creator == userId).AsQueryable();
            if (criteria.Ids != null && criteria.Ids.Any())
            {
                query = query.Where(x => criteria.Ids.Contains(x.Id));
            }
            if (criteria.OnlyNew)
            {
                query = query.Where(x => x.IsNew);
            }
            if (criteria.StartDate != null)
            {
                query = query.Where(x => x.Created >= criteria.StartDate);
            }
            if (criteria.EndDate != null)
            {
                query = query.Where(x => x.Created <= criteria.EndDate);
            }

            if (criteria.OrderBy != null)
            {
                var parts = criteria.OrderBy.Split(':');
                if (parts.Any())
                {
                    var fieldName = parts[0];
                    if (parts.Length > 1 && parts[1].EqualsInvariant("desc"))
                    {
                        query = query.OrderByDescending(fieldName);
                    }
                    else
                    {
                        query = query.OrderBy(fieldName);
                    }
                }
            }

            var retVal = new PushNotificationSearchResult
            {
                TotalCount = query.Count(),
                NewCount = query.Count(x => x.IsNew),
                NotifyEvents = query.Skip(criteria.Start).Take(criteria.Count).ToList()
            };

            return retVal;
        }
    
        public void Upsert(PushNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            lock (_lockObject)
            {
                var alreadyExistNotify = _innerList.FirstOrDefault(x => x.Id == notification.Id);
                if (alreadyExistNotify != null)
                {
                    _innerList.Remove(alreadyExistNotify);
                    _innerList.Add(notification);
                }
                else
                {
                    var lastEvent = _innerList.OrderByDescending(x => x.Created).FirstOrDefault();
                    if (lastEvent != null && lastEvent.ItHasSameContent(notification))
                    {
                        lastEvent.IsNew = true;
                        lastEvent.RepeatCount++;
                        lastEvent.Created = DateTime.UtcNow;
                    }
                    else
                    {
                        _innerList.Add(notification);
                    }
                }
            }

            Send(notification);
        }

        public Task Send(PushNotification message)
        {
            return Clients.All.InvokeAsync("Send", message);
        }

    }

}
