using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using Serialize.Linq.Extensions;
using Serialize.Linq.Serializers;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Serialization;

namespace VirtoCommerce.PricingModule.Data.Services
{
    public class XmlExpressionSerializer : IExpressionSerializer
    {
        public string SerializeExpression(Expression expression)
        {
            return expression.ToXml();
        }

        public T DeserializeExpression<T>(string serializedExpression)
        {
            var serializer = GetSerializer();
            var rawExpression = serializer.DeserializeText(serializedExpression);

            var typedExpression = (Expression<T>)rawExpression;
            var result = typedExpression.Compile();
            return result;
        }


        private static ExpressionSerializer GetSerializer()
        {
            return new ExpressionSerializer(new XmlSerializer());
        }
    }
}
