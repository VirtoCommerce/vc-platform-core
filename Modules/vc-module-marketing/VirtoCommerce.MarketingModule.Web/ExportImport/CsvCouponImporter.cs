using System;
using System.IO;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.MarketingModule.Web.ExportImport
{
    public sealed class CsvCouponImporter
    {
        private readonly ICouponService _couponService;
        private const int ChunkSize = 500;

        public CsvCouponImporter(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public void DoImport(Stream inputStream, string delimiter, string promotionId, DateTime? expirationDate, Action<ExportImportProgressInfo> progressCallback)
        {
            //TODO
            //var coupons = new List<Coupon>();

            //var progressInfo = new ExportImportProgressInfo
            //{
            //    Description = "Reading coupons from CSV..."
            //};
            //progressCallback(progressInfo);

            //using (var reader = new CsvReader(new StreamReader(inputStream)))
            //{
            //    reader.Configuration.Delimiter = delimiter;
            //    reader.Configuration.HasHeaderRecord = false;
            //    while (reader.Read())
            //    {
            //        coupons.Add(new Coupon
            //        {
            //            Code = reader.GetField<string>(0),
            //            MaxUsesNumber = reader.GetField<int>(1),
            //            PromotionId = promotionId,
            //            ExpirationDate = expirationDate
            //        });
            //    }
            //}

            //var chunksCount = (int)Math.Ceiling((double)coupons.Count / ChunkSize);
            //for (var i = 0; i < chunksCount; i++)
            //{
            //    progressInfo.Description = string.Format("Importing {0} of {1} coupons...", (i + 1) * ChunkSize, coupons.Count);
            //    progressCallback(progressInfo);
            //    var chunk = coupons.Skip(i * ChunkSize).Take(ChunkSize);
            //    _couponService.SaveCoupons(chunk.ToArray());
            //}
            //progressInfo.Description = "Coupons import is finished.";
            //progressCallback(progressInfo);

        }
    }
}
