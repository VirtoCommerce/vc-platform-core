using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Tools.Models;
using Xunit;

namespace VirtoCommerce.Tools.Tests
{
    [Trait("Category", "CI")]
    public class UrlBuilderSingleStoreSingleLanguage
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderSingleStoreSingleLanguage()
        {
            _context = new UrlBuilderContext
            {
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        DefaultLanguage = "en-US",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                        }),
                    },
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_VirtualPathIsNull_Expect_Null()
        {
            var result = _builder.BuildStoreUrl(_context, null);
            Assert.Null(result);
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsNull_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, null);
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsAny_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, "en-US");
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_VirtualRoot()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_CurrentStoreAndSelectedLanguage_Expect_VirtualRoot()
        {
            var store = _context.CurrentStore;
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/", result);
        }
    }

    [Trait("Category", "CI")]
    public class UrlBuilderSingleStoreMultipleLanguages
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderSingleStoreMultipleLanguages()
        {
            _context = new UrlBuilderContext
            {
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        DefaultLanguage = "ru-RU",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                            "ru-RU",
                            "lt-LT",
                        }),
                    },
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsNull_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, null);
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsAny_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, "en-US");
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_CurrentLanguage()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("~/en-US/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_DefaultLanguage()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/ru-RU/", result);
        }

        [Fact]
        public void When_CurrentStoreAndSelectedLanguage_Expect_SelectedLanguage()
        {
            var store = _context.CurrentStore;
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/lt-LT/", result);
        }
    }

    [Trait("Category", "CI")]
    public class UrlBuilderMultipleStoresSingleLanguage
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderMultipleStoresSingleLanguage()
        {
            _context = new UrlBuilderContext
            {
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        DefaultLanguage = "en-US",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                        }),
                    },
                    new Store
                    {
                        Id = "Store2",
                        DefaultLanguage = "de-DE",
                        Languages = new List<string>(new[]
                        {
                            "de-DE",
                        }),
                    }
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsNull_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, null);
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsAny_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, "en-US");
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_CurrentStoreAndNoLanguage()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("~/Store1/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_CurrentStoreAndNoLanguage()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store1/", result);
        }

        [Fact]
        public void When_SelectedStoreAndUnknownLanguage_Expect_SelectedStoreAndNoLanguage()
        {
            var store = _context.AllStores.Last();
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/", result);
        }

        [Fact]
        public void When_SelectedStoreAndSelectedLanguage_Expect_SelectedStoreAndNoLanguage()
        {
            var store = _context.AllStores.Last();
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/", result);
        }
    }

    [Trait("Category", "CI")]
    public class UrlBuilderMultipleStoresMultipleLanguages
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderMultipleStoresMultipleLanguages()
        {
            _context = new UrlBuilderContext
            {
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        DefaultLanguage = "ru-RU",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                            "ru-RU",
                            "lt-LT",
                        }),
                    },
                    new Store
                    {
                        Id = "Store2",
                        DefaultLanguage = "es-ES",
                        Languages = new List<string>(new[]
                        {
                            "de-DE",
                            "es-ES",
                            "fr-FR",
                        }),
                    }
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsNull_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, null);
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsAny_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, "en-US");
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_CurrentStoreAndCurrentLanguage()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("~/Store1/en-US/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_CurrentStoreAndDefaultLanguage()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store1/ru-RU/", result);
        }

        [Fact]
        public void When_CurrentStoreAndSelectedLanguage_Expect_CurrentStoreAndSelectedLanguage()
        {
            var store = _context.CurrentStore;
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store1/lt-LT/", result);
        }

        [Fact]
        public void When_SelectedStoreAndUnknownLanguage_Expect_SelectedStoreAndDefaultLanguage()
        {
            var store = _context.AllStores.Last();
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/es-ES/", result);
        }

        [Fact]
        public void When_SelectedStoreAndSelectedLanguage_Expect_SelectedStoreAndSelectedLanguage()
        {
            var store = _context.AllStores.Last();
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/fr-FR/", result);
        }
    }

    [Trait("Category", "CI")]
    public class UrlBuilderStoreWithUrlsInsecureRequest
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderStoreWithUrlsInsecureRequest()
        {
            _context = new UrlBuilderContext
            {
                CurrentUrl = "http://localhost/insecure1/path",
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        Url = "http://localhost/insecure1",
                        SecureUrl = "https://localhost/secure1",
                        DefaultLanguage = "ru-RU",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                            "ru-RU",
                            "lt-LT",
                        }),
                    },
                    new Store
                    {
                        Id = "Store2",
                        DefaultLanguage = "es-ES",
                        Languages = new List<string>(new[]
                        {
                            "de-DE",
                            "es-ES",
                            "fr-FR",
                        }),
                    }
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsNull_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, null);
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsAny_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, "en-US");
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_InsecureUrlAndCurrentLanguage()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("http://localhost/insecure1/en-US/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_InsecureUrlAndDefaultLanguage()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("http://localhost/insecure1/ru-RU/", result);
        }

        [Fact]
        public void When_CurrentStoreAndSelectedLanguage_Expect_InsecureUrlAndSelectedLanguage()
        {
            var store = _context.CurrentStore;
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("http://localhost/insecure1/lt-LT/", result);
        }

        [Fact]
        public void When_SelectedStoreAndUnknownLanguage_Expect_SelectedStoreAndDefaultLanguage()
        {
            var store = _context.AllStores.Last();
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/es-ES/", result);
        }

        [Fact]
        public void When_SelectedStoreAndSelectedLanguage_Expect_SelectedStoreAndSelectedLanguage()
        {
            var store = _context.AllStores.Last();
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/fr-FR/", result);
        }
    }

    [Trait("Category", "CI")]
    public class UrlBuilderStoreWithUrlsSecureRequest
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderStoreWithUrlsSecureRequest()
        {
            _context = new UrlBuilderContext
            {
                CurrentUrl = "https://localhost/secure1/path",
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        Url = "http://localhost/insecure1",
                        SecureUrl = "https://localhost/secure1",
                        DefaultLanguage = "ru-RU",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                            "ru-RU",
                            "lt-LT",
                        }),
                    },
                    new Store
                    {
                        Id = "Store2",
                        DefaultLanguage = "es-ES",
                        Languages = new List<string>(new[]
                        {
                            "de-DE",
                            "es-ES",
                            "fr-FR",
                        }),
                    }
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsNull_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, null);
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_StoreIsNullAndLanguageIsAny_Expect_VirtualRoot()
        {
            var result = _builder.BuildStoreUrl(_context, "/", null, "en-US");
            Assert.Equal("~/", result);
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_SecureUrlAndCurrentLanguage()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("https://localhost/secure1/en-US/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_SecureUrlAndDefaultLanguage()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("https://localhost/secure1/ru-RU/", result);
        }

        [Fact]
        public void When_CurrentStoreAndSelectedLanguage_Expect_SecureUrlAndSelectedLanguage()
        {
            var store = _context.CurrentStore;
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("https://localhost/secure1/lt-LT/", result);
        }

        [Fact]
        public void When_SelectedStoreAndUnknownLanguage_Expect_SelectedStoreAndDefaultLanguage()
        {
            var store = _context.AllStores.Last();
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/es-ES/", result);
        }

        [Fact]
        public void When_SelectedStoreAndSelectedLanguage_Expect_SelectedStoreAndSelectedLanguage()
        {
            var store = _context.AllStores.Last();
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("~/Store2/fr-FR/", result);
        }
    }

    [Trait("Category", "CI")]
    public class UrlBuilderStoreWithUrlsRequestFromDifferentStore
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderStoreWithUrlsRequestFromDifferentStore()
        {
            _context = new UrlBuilderContext
            {
                CurrentUrl = "https://localhost/secure2/path",
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        Url = "http://localhost/insecure1",
                        SecureUrl = "https://localhost/secure1",
                        DefaultLanguage = "ru-RU",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                            "ru-RU",
                            "lt-LT",
                        }),
                    },
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_InsecureUrlAndCurrentLanguage()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("http://localhost/insecure1/en-US/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_InsecureUrlAndDefaultLanguage()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("http://localhost/insecure1/ru-RU/", result);
        }

        [Fact]
        public void When_CurrentStoreAndSelectedLanguage_Expect_InsecureUrlAndSelectedLanguage()
        {
            var store = _context.CurrentStore;
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("http://localhost/insecure1/lt-LT/", result);
        }
    }

    [Trait("Category", "CI")]
    public class UrlBuilderStoreWithUrlsNoRequest
    {
        private readonly UrlBuilderContext _context;
        private readonly IUrlBuilder _builder;

        public UrlBuilderStoreWithUrlsNoRequest()
        {
            _context = new UrlBuilderContext
            {
                AllStores = new[]
                {
                    new Store
                    {
                        Id = "Store1",
                        Url = "http://localhost/insecure1",
                        SecureUrl = "https://localhost/secure1",
                        DefaultLanguage = "ru-RU",
                        Languages = new List<string>(new[]
                        {
                            "en-US",
                            "ru-RU",
                            "lt-LT",
                        }),
                    },
                }
            };

            _context.CurrentStore = _context.AllStores.First();
            _context.CurrentLanguage = _context.CurrentStore.Languages.First();

            _builder = new UrlBuilder();
        }

        [Fact]
        public void When_CurrentStoreAndCurrentLanguage_Expect_InsecureUrlAndCurrentLanguage()
        {
            var result = _builder.BuildStoreUrl(_context, "/");
            Assert.Equal("http://localhost/insecure1/en-US/", result);
        }

        [Fact]
        public void When_AbsoluteUrl_Expect_AbsoluteUrl()
        {
            var result = _builder.BuildStoreUrl(_context, "http://domain/path");
            Assert.Equal("http://domain/path", result);
        }

        [Fact]
        public void When_CurrentStoreAndUnknownLanguage_Expect_InsecureUrlAndDefaultLanguage()
        {
            var store = _context.CurrentStore;
            var language = "ja-JP";

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("http://localhost/insecure1/ru-RU/", result);
        }

        [Fact]
        public void When_CurrentStoreAndSelectedLanguage_Expect_InsecureUrlAndSelectedLanguage()
        {
            var store = _context.CurrentStore;
            var language = store.Languages.Last();

            var result = _builder.BuildStoreUrl(_context, "/", store, language);
            Assert.Equal("http://localhost/insecure1/lt-LT/", result);
        }
    }
}
