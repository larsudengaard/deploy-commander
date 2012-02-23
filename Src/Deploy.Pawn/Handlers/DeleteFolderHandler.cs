using System;
using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn.Handlers
{
    public class DeleteFolderExecuter : TaskExecuter<DeleteFolder, Result>
    {
        protected override Result Handle(DeleteFolder task)
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