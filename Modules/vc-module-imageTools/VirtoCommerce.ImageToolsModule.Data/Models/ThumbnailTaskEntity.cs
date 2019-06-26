using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Models
{
    public class ThumbnailTaskEntity : AuditableEntity
    {
        public ThumbnailTaskEntity()
        {
            ThumbnailTaskOptions = new NullCollection<ThumbnailTaskOptionEntity>();
        }

        [Required]
        [StringLength(1024)]
        public string Name { get; set; }

        [Required]
        [StringLength(2048)]
        public string WorkPath { get; set; }

        public DateTime? LastRun { get; set; }

        public ObservableCollection<ThumbnailTaskOptionEntity> ThumbnailTaskOptions { get; set; }

        public virtual ThumbnailTaskEntity FromModel(ThumbnailTask task, PrimaryKeyResolvingMap pkMap)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            pkMap.AddPair(task, this);

            Id = task.Id;
            Name = task.Name;
            LastRun = task.LastRun;
            WorkPath = task.WorkPath;
            CreatedBy = task.CreatedBy;
            CreatedDate = task.CreatedDate;
            ModifiedBy = task.ModifiedBy;
            ModifiedDate = task.ModifiedDate;

            if (task.ThumbnailOptions != null)
            {
                ThumbnailTaskOptions = new ObservableCollection<ThumbnailTaskOptionEntity>(task.ThumbnailOptions.Select(x => FromModel(x, task)));
            }

            return this;
        }

        public virtual ThumbnailTaskOptionEntity FromModel(ThumbnailOption option, ThumbnailTask task)
        {
            var result = new ThumbnailTaskOptionEntity
            {
                ThumbnailTaskId = task.Id,
                ThumbnailOptionId = option.Id
            };
            return result;
        }

        public virtual ThumbnailTask ToModel(ThumbnailTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            task.Id = Id;
            task.CreatedBy = CreatedBy;
            task.CreatedDate = CreatedDate;
            task.LastRun = LastRun;
            task.ModifiedBy = ModifiedBy;
            task.ModifiedDate = ModifiedDate;
            task.Name = Name;
            task.WorkPath = WorkPath;

            task.ThumbnailOptions = ThumbnailTaskOptions.Select(o => o.ThumbnailOption.ToModel(AbstractTypeFactory<ThumbnailOption>.TryCreateInstance())).ToArray();

            return task;
        }

        public virtual void Patch(ThumbnailTaskEntity target)
        {
            target.LastRun = LastRun;
            target.Name = Name;
            target.WorkPath = WorkPath;

            if (!ThumbnailTaskOptions.IsNullCollection())
            {
                var comparer = AnonymousComparer.Create((ThumbnailTaskOptionEntity x) => x.ThumbnailOptionId);
                ThumbnailTaskOptions.Patch(target.ThumbnailTaskOptions, comparer, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }
        }
    }
}
