using System;
using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;

namespace Deploy.Pawn.Executors
{
    public class DeleteFolderExecutor : TaskExecutor<DeleteFolder, Result>
    {
        protected override Result Execute(DeleteFolder task)
        {
            if (task.Path.Length <= 3)
                throw new InvalidOperationException("WTF?! Trying to delete drive?");

            if (task.Path.ToLower().Contains("Windows"))
                throw new InvalidOperationException("WTF?! Trying to delete windows installation?");

            Directory.Delete(task.Path, true);
            return new Result
            {
                Success = true,
                Message = "Deleted folder: " + task.Path
            };
        }
    }
}