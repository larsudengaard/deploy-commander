using System;
using System.IO;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;
using Deploy.Utilities;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Deploy.Pawn.Executors
{
    public class PackageExecutor : TaskExecutor<Package, Package.Result>
    {
        protected override Package.Result Execute(Package task)
        {
            var packagePath = string.Format(@"C:\temp\packages\{0}\", task.PackageName);
            
            using (var memoryStream = new MemoryStream(task.FileData))
            {
                Zip.Extract(memoryStream, packagePath);
            }

            return new Package.Result
            {
                PackagePath = packagePath,
            };
        }
    }
}