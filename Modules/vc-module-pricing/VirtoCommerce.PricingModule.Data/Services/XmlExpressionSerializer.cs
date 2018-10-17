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
            var expression = serializer.DeserializeText(serializedExpression);

            // TODO: how to convert Expression to Expression<T> to Compile() it?
            //var result = expression.Compile();
            //return result;

            throw new NotImplementedException();
        }


        private static ExpressionSerializer GetSerializer()
        {
            return new ExpressionSerializer(new XmlSerializer());
        }
    }
}
