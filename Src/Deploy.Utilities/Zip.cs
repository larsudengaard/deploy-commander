using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Deploy.Utilities
{
    public static class Zip
    {
        public class File
        {
            public string Filename { get; set; }
            public byte[] Data { get; set; }
        }

        public static IEnumerable<File> Extract(Stream fileStream)
        {
            var filesInPackage = new List<File>();
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
                    
                    var buffer = new byte[4096];		// 4K is optimum
                    Stream zipStream = zipFile.GetInputStream(zipEntry);
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        StreamUtils.Copy(zipStream, memoryStream, buffer);
                        filesInPackage.Add(new File
                        {
                            Data = memoryStream.ToArray(),
                            Filename = zipEntry.Name
                        });
                    }
                }

                return filesInPackage;
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

        public static void Extract(Stream fileStream, string outputFolder)
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
                    using (FileStream streamWriter = System.IO.File.Create(fullZipToPath))
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