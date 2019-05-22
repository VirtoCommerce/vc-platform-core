using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;

namespace VirtoCommerce.NotificationsModule.Data.ExportImport
{
    public sealed class NotificationsExportImport
    {
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationService _notificationService;
        private const int _batchSize = 50;
        private readonly JsonSerializer _jsonSerializer;

        public NotificationsExportImport(INotificationSearchService notificationSearchService, INotificationService notificationService, JsonSerializer jsonSerializer)
        {
            _notificationSearchService = notificationSearchService;
            _notificationService = notificationService;
            _jsonSerializer = jsonSerializer;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Notifications are started to export";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Notifications");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchCriteria = AbstractTypeFactory<NotificationSearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;
                    searchCriteria.ResponseGroup = NotificationResponseGroup.Full.ToString();
                    searchCriteria.IsActive = true;
                    var searchResult = await _notificationSearchService.SearchNotificationsAsync(searchCriteria);
                    return (GenericSearchResult<Notification>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} notifications have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "Notifications")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<EmailNotification>(_jsonSerializer, _batchSize, items => _notificationService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } notifications have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}
