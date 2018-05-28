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
      

        public static License Parse(string rawLicense)
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
                        if (ValidateSignature(data, signature))
                        {
                            result = JsonConvert.DeserializeObject<License>(data);
                            result.RawLicense = rawLicense;
                        }
                    }
                }
            }

            return result;
        }


        private static bool ValidateSignature(string data, string signature)
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
                    Modulus = Convert.FromBase64String("uYgtG8GG6fZ4jZdaL6LF4f2vmmTHNr0H/m+Bfo4vNhOYDlUTOv89FVQ3xE0DPhZ2uQ6Q/AN9KausQz2VbdfUn0Ge/jcHNsdE+9SBdllzgvCr/2sUlCKcpiEIBC9AXnAd7lKFSHiS61cVLo24+8aowoeGsAAO3djqN2xP+4Co9CMywKscLSPUMOJWHMuXAr3+pjamYaqwe3/iv5VA/8ff0evVyqhE/8fIixm9Ti7OhPNwYRDmTKP+t4DRZlp4R46g4v43tg4Q9FYaGKRCuxAdbbEsTYhFzHzv/CcUoFzYF0x3lyW5mfqad5y+LhsWPiHGDrd+xWXq9Nho1glNZ0sGYQ=="),
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

        
    }
}
