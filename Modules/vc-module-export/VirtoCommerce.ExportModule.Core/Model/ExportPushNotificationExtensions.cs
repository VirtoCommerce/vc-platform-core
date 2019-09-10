namespace VirtoCommerce.ExportModule.Core.Model
{
    public static class ExportPushNotificationExtensions
    {
        public static void Patch(this ExportPushNotification target, ExportProgressInfo source)
        {
            target.Description = source.Description;
            target.Errors = source.Errors;
            target.ProcessedCount = source.ProcessedCount;
            target.TotalCount = source.TotalCount;
        }
    }
}
