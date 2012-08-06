using System.IO;
using Deploy.Soldier.Application.Infrastructure;
using Deploy.Tasks.Filesystem;
using Deploy.Utilities;

namespace Deploy.Soldier.Application.Executors
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