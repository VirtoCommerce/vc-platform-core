using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    /// <summary>
    /// Stores relations between contacts and organizations.
    /// </summary>
    public class MemberRelationEntity : Entity
    {
        /// <summary>
        /// Gets or sets the ancestor sequence. A number to indicate whether the ancestor is the parent, 
        /// grandparent, great grandparent, and so on for the descendant. 
        /// 1 means parent, 2 means grand parent, and so on. 
        /// For the Top Organization, it does not have a parent, so its sequence is 0.
        /// </summary>
        /// <value>
        /// The ancestor sequence.
        /// </value>
        [Required]
        public int AncestorSequence { get; set; }

        [StringLength(64)]
        public string RelationType { get; set; }

        #region NavigationProperties

        /// <summary>
        /// Gets or sets the ancestor member id.
        /// </summary>
        /// <value>
        /// The ancestor id.
        /// </value>
        [ForeignKey("Ancestor")]
        [Required]
        [StringLength(128)]
        public string AncestorId { get; set; }
        public virtual MemberEntity Ancestor { get; set; }

        /// <summary>
        /// Gets or sets the descendant member id.
        /// </summary>
        /// <value>
        /// The descendant id.
        /// </value>
        [StringLength(128, ErrorMessage = "Only 128 characters allowed.")]
        public string DescendantId { get; set; }
        public virtual MemberEntity Descendant { get; set; }

        #endregion
    }
}
