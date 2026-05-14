using System.IO;
using System.IO.Compression;

namespace Confirmit.CATI.Core.Export
{
    public class Packaging
    {
        /// <summary>
        /// Creates a package zip file containing specified content file.
        /// Single file for now.
        /// </summary>
        /// <param name="contentFileName">The name of created file in the package</param>
        /// <param name="contentString">The content string</param>
        public string CreatePackage(string contentFileName, string contentString)
        {
            var packagePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using (var zipToOpen = new FileStream(packagePath, FileMode.Create))
            {
                using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    ZipArchiveEntry entry = archive.CreateEntry(contentFileName);
                    using (var writer = new StreamWriter(entry.Open()))
                    {
                        writer.Write(contentString);
                    }
                }
            }

            return packagePath;
        }
    }
}