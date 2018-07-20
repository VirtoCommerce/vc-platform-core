using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.CustomerModule.Data.Model
{
	public class NoteEntity : AuditableEntity
	{

		[StringLength(128)]
		public string AuthorName { get; set; }

		[StringLength(128)]
		public string ModifierName { get; set; }

		[StringLength(128)]
		public string Title { get; set; }

		public string Body { get; set; }

		public bool IsSticky { get; set; }

		#region Navigation Properties
		public string MemberId { get; set; }
        public virtual MemberEntity Member { get; set; }

        #endregion


        public virtual Note ToModel(Note note)
        {
            if (note == null)
                throw new ArgumentNullException("note");

            note.Id = Id;
            note.CreatedBy = CreatedBy;
            note.CreatedDate = CreatedDate;
            note.ModifiedBy = ModifiedBy;
            note.ModifiedDate = ModifiedDate;

            note.Body = this.Body;
            note.Title = this.Title;
            return note;
        }

        public virtual NoteEntity FromModel(Note note)
        {
            if (note == null)
                throw new ArgumentNullException("note");

            Id = note.Id;
            CreatedBy = note.CreatedBy;
            CreatedDate = note.CreatedDate;
            ModifiedBy = note.ModifiedBy;
            ModifiedDate = note.ModifiedDate;

            this.AuthorName = note.CreatedBy;
            this.ModifierName = note.ModifiedBy;
            return this;
        }

        public virtual void Patch(NoteEntity target)
        {
            target.Body = this.Body;
            target.Title = this.Title;
        }
	}
}
