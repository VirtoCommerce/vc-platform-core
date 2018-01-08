namespace VirtoCommerce.Platform.Web.Model
{
    public class ResetPasswordConfirmRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
