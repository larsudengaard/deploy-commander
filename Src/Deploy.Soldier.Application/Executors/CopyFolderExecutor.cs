using System.IO;
using Deploy.Soldier.Application.Infrastructure;
using Deploy.Tasks.Filesystem;

namespace Deploy.Soldier.Application.Executors
{
    public class CopyFolderExecutor : TaskExecutor<CopyFolder, CopyFolder.Result>
    {
        protected override CopyFolder.Result Execute(CopyFolder task)
        {
            var sourceDirectory = new DirectoryInfo(task.SourcePath);
            if (!sourceDirectory.Exists)
            {
                return new CopyFolder.Result
                {
                    Success = false,
                    Message = "SourcePath does not exists."
                };
            }

            var destinationDirectory = new DirectoryInfo(task.DestinationPath);
            if (destinationDirectory.Exists)
            {
                if (destinationDirectory.GetFiles().Length > 0)
                {
                    return new CopyFolder.Result
                    {
                        Success = false,
                        Message = "DestinationPath is not empty."
                    };
                }
            }
            else
            {
                destinationDirectory.Create();
            }

            Copy(sourceDirectory, destinationDirectory);
            return new CopyFolder.Result
            {
                Success = true,
                Message = "Files copied successfully",
            };
        }

        static void Copy(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(destination.ToString(), fi.Name), true);
            }

            foreach (DirectoryInfo sourceSubDir in source.GetDirectories())
            {
                DirectoryInfo destinationSubDir =
                    destination.CreateSubdirectory(sourceSubDir.Name);
                Copy(sourceSubDir, destinationSubDir);
            }
        }
    }
}