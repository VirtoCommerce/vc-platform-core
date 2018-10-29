using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Utilities
{
    /// <summary>
    /// Common operations for different implementations of the <see cref="IOperation"/> interface.
    /// </summary>
    public static class OperationUtilities
    {
        /// <summary>
        /// Builds a list of all child operations related to the given operation.
        /// Child operations are collected from properties of given operation using reflection.
        /// </summary>
        /// <param name="operation">The requested operation.</param>
        /// <returns>List of all child operations of the <paramref name="operation"/>.</returns>
        public static IEnumerable<IOperation> GetAllChildOperations(IOperation operation)
        {
            var retVal = new List<IOperation>();
            var objectType = operation.GetType();

            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var childOperations = properties.Where(x => x.PropertyType.GetInterface(typeof(IOperation).Name) != null)
                .Select(x => (IOperation)x.GetValue(operation)).Where(x => x != null).ToList();

            foreach (var childOperation in childOperations)
            {
                retVal.Add(childOperation);
            }

            //Handle collection and arrays
            var collections = properties.Where(p => p.GetIndexParameters().Length == 0)
                .Select(x => x.GetValue(operation, null))
                .Where(x => x is IEnumerable && !(x is string))
                .Cast<IEnumerable>();

            foreach (var collection in collections)
            {
                foreach (var childOperation in collection.OfType<IOperation>())
                {
                    retVal.Add(childOperation);
                }
            }

            return retVal;
        }
    }
}
