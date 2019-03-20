using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.MarketingModule.Web.ExportImport
{
    public sealed class CsvCouponImporter
    {
        public CsvCouponImporter(ICouponService couponService)
        {
            _couponService = couponService;
        }

        private readonly ICouponService _couponService;
        private const int ChunkSize = 500;

        public async Task DoImportAsync(Stream inputStream, string delimiter, string promotionId, DateTime? expirationDate, Action<ExportImportProgressInfo> progressCallback)
        {
            var coupons = new List<Coupon>();

            var progressInfo = new ExportImportProgressInfo
            {
                Description = "Reading coupons from CSV..."
            };
            progressCallback(progressInfo);

            using (var reader = new CsvReader(new StreamReader(inputStream)))
            {
                reader.Configuration.Delimiter = delimiter;
                reader.Configuration.HasHeaderRecord = false;
                while (reader.Read())
                {
                    coupons.Add(new Coupon
                    {
                        Code = reader.GetField<string>(0),
                        MaxUsesNumber = reader.GetField<int>(1),
                        PromotionId = promotionId,
                        ExpirationDate = expirationDate
                    });
                }
            }

            var chunksCount = (int)Math.Ceiling((double)coupons.Count / ChunkSize);
            for (var i = 0; i < chunksCount; i++)
            {
                progressInfo.Description = string.Format("Importing {0} of {1} coupons...", (i + 1) * ChunkSize, coupons.Count);
                progressCallback(progressInfo);
                var chunk = coupons.Skip(i * ChunkSize).Take(ChunkSize);
                await _couponService.SaveCouponsAsync(chunk.ToArray());
            }
            progressInfo.Description = "Coupons import is finished.";
            progressCallback(progressInfo);

        }
    }
}
