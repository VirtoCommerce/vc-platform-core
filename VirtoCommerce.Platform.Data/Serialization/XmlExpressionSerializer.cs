using System.Linq.Expressions;
using Serialize.Linq.Serializers;
using VirtoCommerce.Platform.Core.Serialization;

namespace VirtoCommerce.Platform.Data.Serialization
{
    public class XmlExpressionSerializer : IExpressionSerializer
    {
        public string SerializeExpression(Expression expression)
        {
            return GetSerializer().SerializeText(expression);
        }

        public T DeserializeExpression<T>(string serializedExpression)
        {
            var serializer = GetSerializer();
            var expression = serializer.DeserializeText(serializedExpression);
            var node = serializer.Convert(expression).ToExpression<T>();
            return node.Compile();
        }

        private ExpressionSerializer GetSerializer()
        {
            return new ExpressionSerializer(new XmlSerializer());
        }
    }
}
