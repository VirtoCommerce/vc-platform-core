using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.JsonConverters;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.NotificationsModule.Data.ExportImport
{
    public sealed class NotificationsExportEntries
    {
        public bool IsNotEmpty => Notifications.Any() || NotificationTemplates.Any();

        public ICollection<Notification> Notifications { get; set; }
        public ICollection<NotificationTemplate> NotificationTemplates { get; set; }
        public ICollection<NotificationMessage> NotificationMessages { get; set; }

        public ICollection<EmailAttachment> EmailAttachments { get; set; }
        public ICollection<EmailNotification> EmailNotifications { get; set; }
        public ICollection<EmailNotificationMessage> EmailNotificationMessages { get; set; }
        public ICollection<EmailNotificationTemplate> EmailNotificationTemplates { get; set; }
        //todo SMS


    }

    public sealed class NotificationsExportImportManager : IExportSupport, IImportSupport
    {
        private const string _manifestZipEntryName = "Manifest.json";
        private const string _notificationsZipEntryName = "NotificationsEntries.json";
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationService _notificationService;
        private const int _batchSize = 50;
        private readonly JsonSerializer _serializer;

        public NotificationsExportImportManager(INotificationSearchService notificationSearchService, INotificationService notificationService)
        {
            _notificationSearchService = notificationSearchService;
            _notificationService = notificationService;

            _serializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new PolymorphicJsonConverter() },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, System.Text.Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                progressInfo.Description = "Notifications exporting...";
                progressCallback(progressInfo);

                var notificationsResult = await _notificationSearchService.SearchNotificationsAsync(new NotificationSearchCriteria { Take = Int32.MaxValue, ResponseGroup = NotificationResponseGroup.Default.ToString() });
                writer.WritePropertyName("NotificationsTotalCount");
                writer.WriteValue(notificationsResult.TotalCount);

                writer.WritePropertyName("Notifications");
                writer.WriteStartArray();
                for (var i = 0; i < notificationsResult.TotalCount; i += _batchSize)
                {
                    var searchResponse = await _notificationSearchService.SearchNotificationsAsync(new NotificationSearchCriteria { Skip = i, Take = _batchSize, ResponseGroup = NotificationResponseGroup.Full.ToString() });

                    foreach (var notification in searchResponse.Results)
                    {
                        _serializer.Serialize(writer, notification);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(notificationsResult.TotalCount, i + _batchSize) } of { notificationsResult.TotalCount } notifications exported";
                    progressCallback(progressInfo);
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();
            var notificationsTotalCount = 0;

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "NotificationsTotalCount")
                        {
                            notificationsTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "Notifications")
                        {
                            reader.Read();
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var notifications = new List<Notification>();
                                var notificationsCount = 0;

                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var notification = _serializer.Deserialize<Notification>(reader);
                                    notifications.Add(notification);
                                    notificationsCount++;

                                    reader.Read();
                                }
                                cancellationToken.ThrowIfCancellationRequested();

                                if (notificationsCount % _batchSize == 0 || reader.TokenType == JsonToken.EndArray)
                                {
                                    await _notificationService.SaveChangesAsync(notifications.ToArray());
                                    notifications.Clear();

                                    if (notificationsTotalCount > 0)
                                    {
                                        progressInfo.Description = $"{ notificationsCount } of { notificationsTotalCount } notifications imported";
                                    }
                                    else
                                    {
                                        progressInfo.Description = $"{ notificationsCount } notifications imported";
                                    }

                                    progressCallback(progressInfo);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
