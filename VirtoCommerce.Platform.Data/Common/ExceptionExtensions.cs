using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Data.Common
{
    public static class ExceptionExtensions
    {
        public static void ThrowFaultException(this Exception ex)
        {
            throw new Exception(ex.ExpandExceptionMessage());
        }

        public static string ExpandExceptionMessage(this Exception ex)
        {
            var builder = new StringBuilder();

            string separator = Environment.NewLine;
            var exception = ex;

            while (exception != null)
            {
                if (builder.Length > 0)
                {
                    builder.Append(separator);
                }

                builder.Append(exception.Message);

                exception = exception.InnerException;
            }

            return builder.ToString();
        }
    }
}
