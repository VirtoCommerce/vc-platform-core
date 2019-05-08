using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.Platform.Core.ChangeLog
{
    public interface IChangesLogEventHandler : IEventHandler<ChangesLogEvent<IChangesLog>>
    {
    }
}
