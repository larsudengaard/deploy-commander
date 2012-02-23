using System;
using System.Linq;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Infrastructure
{
    public abstract class ManageWebsiteExecuter<T, TResult> : TaskExecuter<T, TResult> 
        where T : ManageWebsiteTask<TResult>
        where TResult : IResult
    {
        protected override TResult Handle(T task)
        {
            var server = new ServerManager();
            Site website = server.Sites.SingleOrDefault(x => x.Name == task.WebsiteName);
            if (website == null)
            {
                throw new MissingWebsiteException(string.Format("Website '{0}' does not exist on this server.", task.WebsiteName));
            }

            return ManageWebsite(task, website);
        }

        protected abstract TResult ManageWebsite(T task, Site website);

        public class MissingWebsiteException : Exception
        {
            public MissingWebsiteException(string message)
                : base(message)
            {
            }
        }
    }
}