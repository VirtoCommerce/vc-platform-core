using System.ComponentModel.DataAnnotations;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Model
{
    public class MenuLinkEntity : AuditableEntity
    {
        [Required]
        [StringLength(1024)]
        public string Title { get; set; }
        [Required]
        [StringLength(2048)]
        public string Url { get; set; }
        [Required]
        public int Priority { get; set; }
        [StringLength(254)]
        public string AssociatedObjectType { get; set; }
        [StringLength(254)]
        public string AssociatedObjectName { get; set; }
        [StringLength(128)]
        public string AssociatedObjectId { get; set; }
        public virtual MenuLinkListEntity MenuLinkList { get; set; }
        public string MenuLinkListId { get; set; }


        public void Patch(MenuLinkEntity target)
        {
            Title = target.Title;
            Url = target.Url;
            Priority = target.Priority;

            AssociatedObjectId = target.AssociatedObjectId;
            AssociatedObjectName = target.AssociatedObjectName;
            AssociatedObjectType = target.AssociatedObjectType;
        }
    }
}
