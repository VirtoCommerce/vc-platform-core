using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public class ViewableEntityConverter<T> where T : IEntity
    {
        private readonly Func<T, string> _nameGetter;
        private readonly Func<T, string> _codeGetter;
        private readonly Func<T, string> _imageUrlGetter;
        private readonly Func<T, string> _parentGetter;

        public ViewableEntityConverter(Func<T, string> nameGetter,
            Func<T, string> codeGetter,
            Func<T, string> imageUrlGetter,
            Func<T, string> parentGetter)
        {
            _nameGetter = nameGetter;
            _codeGetter = codeGetter;
            _imageUrlGetter = imageUrlGetter;
            _parentGetter = parentGetter;
        }

        public virtual ViewableEntity ToViewableEntity(T entity)
        {
            return new ViewableEntity()
            {
                Id = entity.Id,
                Name = _nameGetter(entity),
                Code = _codeGetter(entity),
                ImageUrl = _imageUrlGetter(entity),
                Parent = _parentGetter(entity),
                Type = typeof(T).Name,
            };
        }
    }
}
