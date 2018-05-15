using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Web.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.NotificationsModule.Web.ExportImport
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

    public sealed class NotificationsExportImportManager : ISupportExportImportModule
    {
        private const string _manifestZipEntryName = "Manifest.json";
        private const string _notificationsZipEntryName = "NotificationsEntries.json";
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationService _notificationService;
        private const int _batchSize = 50;

        public NotificationsExportImportManager(INotificationSearchService notificationSearchService, INotificationService notificationService)
        {
            _notificationSearchService = notificationSearchService;
            _notificationService = notificationService;
        }

        public void DoExport(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback,
            CancellationToken cancellationToken)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, System.Text.Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                progressInfo.Description = "Notifications exporting...";
                progressCallback(progressInfo);

                var notificationsResult = _notificationSearchService.SearchNotifications(new NotificationSearchCriteria { Take = Int32.MaxValue });
                writer.WritePropertyName("NotificationsTotalCount");
                writer.WriteValue(notificationsResult.TotalCount);

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                writer.WritePropertyName("Notifications");
                writer.WriteStartArray();
                for (var i = 0; i < notificationsResult.TotalCount; i += _batchSize)
                {
                    var searchResponse = _notificationSearchService.SearchNotifications(new NotificationSearchCriteria { Skip = i, Take = _batchSize });

                    var notifications = _notificationService.GetByIdsAsync(searchResponse.Results.Select(n => n.Id).ToArray()).GetAwaiter().GetResult();
                    foreach (var notification in notifications)
                    {
                        GetJsonSerializer().Serialize(writer, notification);
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

        public void DoImport(Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

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

                            if (cancellationToken.IsCancellationRequested)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                            }
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
                                    var notification = GetJsonSerializer().Deserialize<Notification>(reader);
                                    notifications.Add(notification);
                                    notificationsCount++;

                                    reader.Read();
                                }

                                if (cancellationToken.IsCancellationRequested)
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                }

                                if (notificationsCount % _batchSize == 0 || reader.TokenType == JsonToken.EndArray)
                                {
                                    _notificationService.SaveChangesAsync(notifications.ToArray()).GetAwaiter().GetResult();
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

        private static JsonSerializer GetJsonSerializer()
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = { new PolymorphicJsonConverter() }
            };
            return serializer;
        }
    }
}
