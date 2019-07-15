using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.OrdersModule.Web.Authorization
{
    /// <summary>
    /// Restrict access rights to orders where user is assigned as responsible
    /// </summary>
    public sealed class OnlyOrderResponsibleScope : PermissionScope
    {
        public OnlyOrderResponsibleScope()
        {
            Scope = "{{userId}}";
            Label = "none";
        }
    }
}
