using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class NoteEntity : AuditableEntity, IHasOuterId
    {
        [StringLength(128)]
        public string AuthorName { get; set; }

        [StringLength(128)]
        public string ModifierName { get; set; }

        [StringLength(128)]
        public string Title { get; set; }

        public string Body { get; set; }

        public bool IsSticky { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public string MemberId { get; set; }
        public virtual MemberEntity Member { get; set; }

        #endregion

        public virtual Note ToModel(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            note.Id = Id;
            note.CreatedBy = CreatedBy;
            note.CreatedDate = CreatedDate;
            note.ModifiedBy = ModifiedBy;
            note.ModifiedDate = ModifiedDate;
            note.OuterId = OuterId;

            note.Body = Body;
            note.Title = Title;

            return note;
        }

        public virtual NoteEntity FromModel(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            Id = note.Id;
            CreatedBy = note.CreatedBy;
            CreatedDate = note.CreatedDate;
            ModifiedBy = note.ModifiedBy;
            ModifiedDate = note.ModifiedDate;
            OuterId = note.OuterId;

            AuthorName = note.CreatedBy;
            ModifierName = note.ModifiedBy;

            return this;
        }

        public virtual void Patch(NoteEntity target)
        {
            target.Body = Body;
            target.Title = Title;
        }
    }
}
