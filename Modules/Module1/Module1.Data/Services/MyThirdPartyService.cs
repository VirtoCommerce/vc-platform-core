using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Module1.Abstractions;

namespace Module1.Data.Services
{
    public class ThirdPartyServiceImpl : IThirdPartyService
    {
        public string CallThirdPartyMethodFromAnotherProject(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            using (var inStream = new MemoryStream(bytes))
            using (var outStream = CreateToMemoryStream(inStream, "TestZipEntry"))
            {
                return $"Zipped message UTF-8 decoded string is:{Encoding.UTF8.GetString(outStream.ToArray())}";
            }
        }

        // From https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples#create-a-zip-fromto-a-memory-stream-or-byte-array
        private MemoryStream CreateToMemoryStream(MemoryStream memStreamIn, string zipEntryName)
        {
            var outputMemStream = new MemoryStream();
            var zipStream = new ZipOutputStream(outputMemStream);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            var newEntry = new ZipEntry(zipEntryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);

            StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
            zipStream.Close();          // Must finish the ZipOutputStream before using outputMemStream.

            outputMemStream.Position = 0;
            return outputMemStream;
        }
    }
}
