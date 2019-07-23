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

        public ViewableEntityConverter(Func<T, string> nameGetter,
            Func<T, string> codeGetter,
            Func<T, string> imageUrlGetter)
        {
            _nameGetter = nameGetter;
            _codeGetter = codeGetter;
            _imageUrlGetter = imageUrlGetter;
        }

        public virtual ViewableEntity ToViewableEntity(T entity)
        {
            return new ViewableEntity()
            {
                Id = entity.Id,
                Name = _nameGetter(entity),
                Code = _codeGetter(entity),
                ImageUrl = _imageUrlGetter(entity),
            };
        }
    }
}
