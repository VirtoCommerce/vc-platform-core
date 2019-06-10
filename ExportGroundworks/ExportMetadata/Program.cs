using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace ExportMetadata
{
    class Program
    {
        static void Main(string[] args)
        {
            var etm = ExportedTypeMetadata.GetFromType<Price>();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
