using System;
using System.Linq;
using Deploy.Tasks;
using Deploy.Tasks.Websites;
using Microsoft.Web.Administration;

namespace Deploy.Soldier.Application.Infrastructure.Executers
{
    public abstract class ManageWebsiteExecutor<T, TResult> : TaskExecutor<T, TResult> 
        where T : ManageWebsiteTask<TResult>
        where TResult : IResult
    {
        protected override TResult Execute(T task)
        {
            var serverManager = new ServerManager();
            Site website = serverManager.Sites.SingleOrDefault(x => x.Name == task.WebsiteName);
            if (website == null)
            {
                throw new MissingWebsiteException(string.Format("Website '{0}' does not exist on this server.", task.WebsiteName));
            }

            TResult result = ManageWebsite(task, website);
            serverManager.CommitChanges();

            return result;
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