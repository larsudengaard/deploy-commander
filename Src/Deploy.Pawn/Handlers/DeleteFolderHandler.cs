using System;
using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;

namespace Deploy.Pawn.Handlers
{
    public class DeleteFolderHandler : CommandHandler<DeleteFolder>
    {
        protected override Result Handle(DeleteFolder command)
        {
            if (command.Path.Length <= 3)
                throw new InvalidOperationException("WTF?! Trying to delete drive?");

            if (command.Path.ToLower().Contains("Windows"))
                throw new InvalidOperationException("WTF?! Trying to delete windows installation?");

            Directory.Delete(command.Path, true);
            return new Result
            {
                Success = true,
                Message = "Deleted folder: " + command.Path
            };
        }
    }
}