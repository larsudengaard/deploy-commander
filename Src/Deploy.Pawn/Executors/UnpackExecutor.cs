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
            var packagePath = AppSettings.GetPath("UnpackDirectory") + task.PackageName + "\\";
            if (Directory.Exists(packagePath))
            {
                Directory.Delete(packagePath, true);
            }

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