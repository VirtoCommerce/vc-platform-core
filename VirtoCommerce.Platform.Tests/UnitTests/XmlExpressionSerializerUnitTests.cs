using System;
using System.Linq.Expressions;
using VirtoCommerce.Platform.Data.Serialization;
using Xunit;

namespace VirtoCommerce.Platform.Tests.UnitTests
{
    public class XmlExpressionSerializerUnitTests
    {
        [Fact]
        public void SerializeDeserialize()
        {
            var serializer = new XmlExpressionSerializer();
            Expression<Func<Promotion, bool>> isTeenAgerExpr = s => s.Age > 12 && s.Age < 20;
            var stringExpr = serializer.SerializeExpression(isTeenAgerExpr);

            var func = serializer.DeserializeExpression<Func<Promotion, bool>>(stringExpr);
        }
    }

    public class Promotion
    {
        public int Age { get; set; }
    }
}
