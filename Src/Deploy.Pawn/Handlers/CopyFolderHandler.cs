using System;
using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn.Handlers
{
    public class CopyFolderExecuter : TaskExecuter<CopyFolder, Result>
    {
        protected override Result Handle(CopyFolder task)
        {
            var sourceDirectory = new DirectoryInfo(task.SourcePath);
            if (!sourceDirectory.Exists)
            {
                return new Result
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
                    return new Result
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
            return new Result
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