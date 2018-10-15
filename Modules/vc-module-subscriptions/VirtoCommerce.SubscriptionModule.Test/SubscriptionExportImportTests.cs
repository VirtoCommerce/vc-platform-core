using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Web.ExportImport;
using Xunit;

namespace VirtoCommerce.SubscriptionModule.Test
{
    public class SubscriptionExportImportTests
    {
        private const int ExpectedBatchSize = 20;

        private readonly IList<Subscription> TestSubscriptions = new List<Subscription>()
        {
            new Subscription
            {
                StoreId = "Electronics",
                CustomerId = "88c0ce58-49c9-4ff5-af0d-10f19d0e38bc",
                CustomerName = "alex@mail.com",
                Balance = 1157.3900m,
                Number = "SU170116-00100",
                Interval = PaymentInterval.Months,
                IntervalCount = 1,
                TrialPeriodDays = 0,
                SubscriptionStatus = SubscriptionStatus.Unpaid,
                CustomerOrderPrototypeId = "cc55ce11091446d5ab966364c4cb24a3",
                StartDate = DateTime.Parse("2017-01-16T15:15:05.21"),
                CurrentPeriodStart = DateTime.Parse("2017-01-16T15:15:05.21"),
                CurrentPeriodEnd = DateTime.Parse("2017-02-16T15:15:05.21"),
                IsCancelled = false,
                CreatedDate = DateTime.Parse("2017-01-16T15:15:06.54"),
                ModifiedDate = DateTime.Parse("2018-10-01T05:05:03.757"),
                CreatedBy = "alex@mail.com",
                ModifiedBy = "unknown",
                Id = "d52055595dbe43c181f00c192bcfcb5e"
            },
            new Subscription
            {
                StoreId = "Electronics",
                CustomerId = "88c0ce58-49c9-4ff5-af0d-10f19d0e38bc",
                CustomerName = "alex@mail.com",
                Balance = 290.9400m,
                Number = "SU170116-00000",
                Interval = PaymentInterval.Months,
                IntervalCount = 1,
                TrialPeriodDays = 10,
                SubscriptionStatus = SubscriptionStatus.Unpaid,
                CustomerOrderPrototypeId = "ecbf96fc665e48bbad257bcacf4b90a3",
                StartDate = DateTime.Parse("2017-01-16T15:11:54.613"),
                TrialSart = DateTime.Parse("2017-01-16T15:11:54.613"),
                TrialEnd = DateTime.Parse("2017-01-26T15:11:54.613"),
                CurrentPeriodStart = DateTime.Parse("2017-01-26T15:11:54.613"),
                CurrentPeriodEnd = DateTime.Parse("2017-02-26T15:11:54.613"),
                IsCancelled = false,
                CreatedDate = DateTime.Parse("2017-01-16T15:11:55.463"),
                ModifiedDate = DateTime.Parse("2018-10-01T05:05:03.757"),
                CreatedBy = "alex@mail.com",
                ModifiedBy = "unknown",
                Id = "75c80743dd0044ef9a306b7f58be64da"
            }
        };

        private readonly IList<PaymentPlan> TestPaymentPlans = new List<PaymentPlan>
        {
            new PaymentPlan
            {
                Interval = PaymentInterval.Months,
                IntervalCount = 1,
                TrialPeriodDays = 0,
                CreatedDate = DateTime.Parse("2017-01-16T15:14:58.093"),
                ModifiedDate = DateTime.Parse("2017-01-16T15:14:58.093"),
                CreatedBy = "alex@mail.com",
                ModifiedBy = "alex@mail.com",
                Id = "ceb9b71524664fbc8017bb412dbc48e8"
            },
            new PaymentPlan
            {
                Interval = PaymentInterval.Months,
                IntervalCount = 1,
                TrialPeriodDays = 0,
                CreatedDate = DateTime.Parse("2017-01-16T14:33:37.697"),
                ModifiedDate = DateTime.Parse("2017-01-16T14:33:37.697"),
                CreatedBy = "alex@mail.com",
                ModifiedBy = "alex@mail.com",
                Id = "3afde7c22e0a49f9b80deeff3b65670c"
            },
            new PaymentPlan
            {
                Interval = PaymentInterval.Months,
                IntervalCount = 1,
                TrialPeriodDays = 0,
                CreatedDate = DateTime.Parse("2017-01-16T13:30:47.023"),
                ModifiedDate = DateTime.Parse("2017-01-16T13:30:47.023"),
                CreatedBy = "alex@mail.com",
                ModifiedBy = "alex@mail.com",
                Id = "2ddc62ef321c44aba27a7a99efb1086d"
            },
            new PaymentPlan
            {
                Interval = PaymentInterval.Months,
                IntervalCount = 1,
                TrialPeriodDays = 10,
                CreatedDate = DateTime.Parse("2017-01-16T10:26:05.47"),
                ModifiedDate = DateTime.Parse("2017-01-16T10:48:51.497"),
                CreatedBy = "admin",
                ModifiedBy = "admin",
                Id = "1486f5a1a25f48a999189c081792a379"
            }
        };

        private readonly Mock<ISubscriptionService> _subscriptionService;
        private readonly Mock<ISubscriptionSearchService> _subscriptionSearchService;
        private readonly Mock<IPaymentPlanService> _paymentPlanService;
        private readonly Mock<IPaymentPlanSearchService> _paymentPlanSearchService;
        private readonly Mock<ICancellationToken> _cancellationToken;
        private readonly SubscriptionExportImport _subscriptionExportImport;

        public SubscriptionExportImportTests()
        {
            _subscriptionService = new Mock<ISubscriptionService>();
            _subscriptionSearchService = new Mock<ISubscriptionSearchService>();
            _paymentPlanService = new Mock<IPaymentPlanService>();
            _paymentPlanSearchService = new Mock<IPaymentPlanSearchService>();

            _subscriptionExportImport = new SubscriptionExportImport(_subscriptionService.Object, _subscriptionSearchService.Object,
                _paymentPlanSearchService.Object, _paymentPlanService.Object);

            _cancellationToken = new Mock<ICancellationToken>();
        }

        private static void IgnoreProgressInfo(ExportImportProgressInfo progressInfo)
        {
        }

        private static Stream ReadEmbeddedResourceAsync(string filePath)
        {
            var currentAssembly = typeof(SubscriptionExportImportTests).Assembly;
            var resourcePath = $"{currentAssembly.GetName().Name}.{filePath}";

            return currentAssembly.GetManifestResourceStream(resourcePath);
        }

        [Fact]
        public async Task TestDataExport()
        {
            // Arrange
            var firstSubscriptionCriteria = new SubscriptionSearchCriteria
            {
                Take = 0,
                ResponseGroup = SubscriptionResponseGroup.Default.ToString()
            };
            var firstSubscriptionResult = new GenericSearchResult<Subscription>
            {
                TotalCount = TestSubscriptions.Count
            };
            _subscriptionSearchService
                .Setup(subscriptionSearchService => subscriptionSearchService.SearchSubscriptionsAsync(firstSubscriptionCriteria))
                .ReturnsAsync(firstSubscriptionResult);

            var secondSubscriptionCriteria = new SubscriptionSearchCriteria
            {
                Skip = 0,
                Take = ExpectedBatchSize,
                ResponseGroup = SubscriptionResponseGroup.Default.ToString()
            };
            var secondSubscriptionResult = new GenericSearchResult<Subscription>
            {
                TotalCount = TestSubscriptions.Count,
                Results = TestSubscriptions
            };
            _subscriptionSearchService
                .Setup(subscriptionSearchService => subscriptionSearchService.SearchSubscriptionsAsync(secondSubscriptionCriteria))
                .ReturnsAsync(secondSubscriptionResult);

            var firstPaymentPlanCriteria = new PaymentPlanSearchCriteria
            {
                Take = 0,
            };
            var firstPaymentPlanResult = new GenericSearchResult<PaymentPlan>
            {
                TotalCount = TestPaymentPlans.Count
            };
            _paymentPlanSearchService.Setup(service => service.SearchPlansAsync(firstPaymentPlanCriteria))
                .ReturnsAsync(firstPaymentPlanResult);

            var secondPaymentPlanCriteria = new PaymentPlanSearchCriteria
            {
                Skip = 0,
                Take = ExpectedBatchSize
            };
            var secondPaymentPlanResult = new GenericSearchResult<PaymentPlan>
            {
                TotalCount = TestPaymentPlans.Count,
                Results = TestPaymentPlans
            };
            _paymentPlanSearchService.Setup(service => service.SearchPlansAsync(secondPaymentPlanCriteria))
                .ReturnsAsync(secondPaymentPlanResult);

            string expectedJson;
            using (var resourceStream = ReadEmbeddedResourceAsync("Resources.SerializedSubscriptionData.json"))
            using (var textReader = new StreamReader(resourceStream))
            {
                expectedJson = await textReader.ReadToEndAsync();
            }

            // Act
            string actualJson;

            using (var targetStream = new MemoryStream())
            using (var textReader = new StreamReader(targetStream))
            {
                await _subscriptionExportImport.DoExportAsync(targetStream, IgnoreProgressInfo, _cancellationToken.Object);

                targetStream.Seek(0, SeekOrigin.Begin);
                actualJson = await textReader.ReadToEndAsync();
            }

            // Assert
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task TestDataImport()
        {
            // Arrange
            Subscription[] actualSubscriptions = null;
            _subscriptionService.Setup(service => service.SaveSubscriptionsAsync(It.IsAny<Subscription[]>()))
                .Callback<Subscription[]>(subscriptions => actualSubscriptions = subscriptions)
                .Returns(Task.CompletedTask);

            PaymentPlan[] actualPaymentPlans = null;
            _paymentPlanService.Setup(service => service.SavePlansAsync(It.IsAny<PaymentPlan[]>()))
                .Callback<PaymentPlan[]>(paymentPlans => actualPaymentPlans = paymentPlans)
                .Returns(Task.CompletedTask);

            // Act
            using (var resourceStream = ReadEmbeddedResourceAsync("Resources.SerializedSubscriptionData.json"))
            {
                await _subscriptionExportImport.DoImportAsync(resourceStream, IgnoreProgressInfo, _cancellationToken.Object);
            }

            // Assert
            Assert.Equal(TestSubscriptions, actualSubscriptions);
            Assert.Equal(TestPaymentPlans, actualPaymentPlans);
        }
    }
}
