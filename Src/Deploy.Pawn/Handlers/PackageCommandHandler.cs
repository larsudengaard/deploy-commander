using System;
using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Deploy.Pawn.Handlers
{
    public class PackageHandler : CommandHandler<Package>
    {
        protected override Result Handle(Package command)
        {
            var packageName = Path.GetFileNameWithoutExtension(command.PackageName);
            var packagePath = string.Format(@"C:\temp\packages\{0}\", packageName);
            
            using (var memoryStream = new MemoryStream(command.FileData))
            {
                ExtractZipFile(memoryStream, packagePath + command.PackageName);
            }

            return new Result
            {
                Success = true,
                Message = packagePath,
            };
        }

        public void ExtractZipFile(Stream fileStream, string outputFolder)
        {
            ZipFile zipFile = null;
            try
            {
                
                zipFile = new ZipFile(fileStream);
                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;			// Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];		// 4K is optimum
                    Stream zipStream = zipFile.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outputFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zipFile.Close(); // Ensure we release resources
                }
            }
        }
    }
}