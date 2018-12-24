namespace VirtoCommerce.Platform.Web.Infrastructure
{
    public class Authorization
    {
        public string RefreshTokenLifeTime { get; set; }

        public string AccessTokenLifeTime { get; set; }

        public string Authority { get; set; }

        public string LimitedCookiePermissions { get; set; }
    }
}
