using System.Linq;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Handlers
{
    public abstract class ManageWebsiteHandler<T> : CommandHandler<T> where T : ManageWebsiteCommand
    {
        protected override Result Handle(T command)
        {
            var server = new ServerManager();
            Site website = server.Sites.SingleOrDefault(x => x.Name == command.WebsiteName);
            if (website == null)
            {
                return new Result
                {
                    Success = false,
                    Message = string.Format("Website '{0}' does not exist on this server.", command.WebsiteName)
                };
            }

            return ManageWebsite(command, website);
        }

        protected abstract Result ManageWebsite(T command, Site website);
    }
}