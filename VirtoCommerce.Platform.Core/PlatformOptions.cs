namespace VirtoCommerce.Platform.Core
{
    public class PlatformOptions
    {
        public string DemoCredentials { get; set; }
        public string DemoResetTime { get; set; }
        public string ActivationUrl { get; set; }
        public string LicenseFilePath { get; set; }
        public string LocalUploadFolderPath { get; set; } = "App_Data/Uploads";
        public string LicensePublicKeyPath { get; set; }
        public string LicensePrivateKeyPath { get; set; }
    }
}
