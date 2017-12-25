using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.Licensing;

namespace VirtoCommerce.Platform.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DemoOptions _demoOptions;
        private readonly IHostingEnvironment _hostEnv;
        public IndexModel(IOptions<DemoOptions> demoOptions, IOptions<WebAnalyticsOptions> webAnalyticsOptions, IHostingEnvironment hostEnv)
        {
            _demoOptions = demoOptions.Value;
            WebAnalyticsOptions = webAnalyticsOptions.Value;
            _hostEnv = hostEnv;
        
        }

        public WebAnalyticsOptions WebAnalyticsOptions { get; set; }
        public string PlatformVersion { get; set; }
        public string DemoCredentials { get; set; }
        public string DemoResetTime { get; set; }
        public string License { get; set; }


        public void OnGet()
        {
            PlatformVersion = Core.Common.PlatformVersion.CurrentVersion.ToString();
            DemoCredentials = _demoOptions.DemoCredentials;
            DemoResetTime = _demoOptions.DemoResetTime;

            var license = LoadLicense();
            if (license != null)
            {
                License = JsonConvert.SerializeObject(license, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            }

            if (!string.IsNullOrEmpty(DemoResetTime))
            {
                TimeSpan timeSpan;
                if (TimeSpan.TryParse(DemoResetTime, out timeSpan))
                {
                    var now = DateTime.UtcNow;
                    var resetTime = new DateTime(now.Year, now.Month, now.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, DateTimeKind.Utc);

                    if (resetTime < now)
                    {
                        resetTime = resetTime.AddDays(1);
                    }

                    DemoResetTime = JsonConvert.SerializeObject(resetTime).Replace("\"", "'");
                }
            }
        }

        private License LoadLicense()
        {
            License license = null;

            var licenseFilePath = _hostEnv.MapPath("~/App_Data/VirtoCommerce.lic");
            if (System.IO.File.Exists(licenseFilePath))
            {
                var rawLicense = System.IO.File.ReadAllText(licenseFilePath);
                license = VirtoCommerce.Platform.Web.Licensing.License.Parse(rawLicense);

                if (license != null)
                {
                    license.RawLicense = null;
                }
            }

            return license;
        }
    }
}
