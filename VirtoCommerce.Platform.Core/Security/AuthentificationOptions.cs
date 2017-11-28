using System;

namespace VirtoCommerce.Platform.Core.Security
{
    public class AuthenticationOptions 
    {
        public bool CookiesEnabled { get; set; }
        public TimeSpan CookiesValidateInterval { get; set; }

        public bool BearerTokensEnabled { get; set; }
        public TimeSpan BearerTokensExpireTimeSpan { get; set; }

        public bool HmacEnabled { get; set; }
        public TimeSpan HmacSignatureValidityPeriod { get; set; }

        public bool ApiKeysEnabled { get; set; }
        public string ApiKeysHttpHeaderName { get; set; }
        public string ApiKeysQueryStringParameterName { get; set; }
    }
}
