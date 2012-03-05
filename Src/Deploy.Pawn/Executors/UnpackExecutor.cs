using System;
using System.IO;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;
using Deploy.Utilities;

namespace Deploy.Pawn.Executors
{
    public class UnpackExecutor : TaskExecutor<Unpack, Unpack.Result>
    {
        protected override Unpack.Result Execute(Unpack task)
        {
            throw new NotImplementedException("WHAAT");
            var packagePath = string.Format(@"C:\temp\packages\{0}\", task.PackageName);
            
            using (var memoryStream = new MemoryStream(task.FileData))
            {
                Zip.Extract(memoryStream, packagePath);
            }

            return new Unpack.Result
            {
                PackagePath = packagePath,
            };
        }
    }
}