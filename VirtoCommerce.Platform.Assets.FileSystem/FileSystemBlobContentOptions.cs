namespace VirtoCommerce.Platform.Assets.FileSystem
{
    public class FileSystemBlobContentOptions
    {
        /// <summary>
        /// The root folder where the files are stored
        /// </summary>
        public string RootPath { get; set; }
        /// <summary>
        /// The content part which uses to  generate the absolute public URL for direct access to files stored in the file system
        /// Example: with 'assets' value will be generated resulting http://localhost:8906/assets url
        /// </summary>
        public string PublicPath { get; set; } = "assets";
    }
}
