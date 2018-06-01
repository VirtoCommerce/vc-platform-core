using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace VirtoCommerce.Platform.Web.Licensing
{
    public sealed class License
    {
        private static readonly string _hashAlgorithmName = HashAlgorithmName.SHA256.Name;

        public string Type { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string RawLicense { get; set; }
      

        public static License Parse(string rawLicense, string publicKeyPath)
        {
            License result = null;

            if (!string.IsNullOrEmpty(rawLicense))
            {
                using (var reader = new StringReader(rawLicense))
                {
                    var data = reader.ReadLine();
                    var signature = reader.ReadLine();

                    if (data != null && signature != null)
                    {
                        if (ValidateSignature(data, signature, publicKeyPath))
                        {
                            result = JsonConvert.DeserializeObject<License>(data);
                            result.RawLicense = rawLicense;
                        }
                    }
                }
            }

            return result;
        }


        private static bool ValidateSignature(string data, string signature, string publicKeyPath)
        {
            bool result;
            byte[] dataHash;

            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var algorithm = SHA256.Create())
            {
                dataHash = algorithm.ComputeHash(dataBytes);
            }

            var signatureBytes = Convert.FromBase64String(signature);

            try
            {
                var rsaParam = new RSAParameters()
                {
                    Modulus = Convert.FromBase64String(ReadFileWithKey(publicKeyPath)),
                    Exponent = Convert.FromBase64String("AQAB")
                };

                using (var rsa = new RSACryptoServiceProvider())
                {
                    // Import public key
                    rsa.ImportParameters(rsaParam);

                    // Create signature verifier with the rsa key
                    var signatureDeformatter = new RSAPKCS1SignatureDeformatter(rsa);

                    // Set the hash algorithm to SHA256.
                    signatureDeformatter.SetHashAlgorithm(_hashAlgorithmName);

                    result = signatureDeformatter.VerifySignature(dataHash, signatureBytes);
                }
            }
            catch (FormatException)
            {
                result = false;
            }

            return result;
        }

        private static string ReadFileWithKey(string path)
        {
            string fileContent;

            using (var streamReader = File.OpenText(path))
            {
                fileContent = streamReader.ReadToEnd();
            }

            return fileContent;
        }

        
    }
}
