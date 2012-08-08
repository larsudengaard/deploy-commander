using System;
using System.IO;
using Deploy.Soldier.Application.Infrastructure;
using Deploy.Tasks.Filesystem;

namespace Deploy.Soldier.Application.Executors
{
    public class DeleteFolderExecutor : TaskExecutor<DeleteFolder, DeleteFolder.Result>
    {
        protected override DeleteFolder.Result Execute(DeleteFolder task)
        {
            if (task.Path.Length <= 3)
                throw new InvalidOperationException("WTF?! Trying to delete drive?");

            if (task.Path.ToLower().Contains("Windows"))
                throw new InvalidOperationException("WTF?! Trying to delete windows installation?");

            if (Directory.Exists(task.Path))
            {
                Directory.Delete(task.Path, true);
            }
            
            return new DeleteFolder.Result();
        }
    }
}