using System;
using Deploy.Soldier.Application.Infrastructure.Executers;
using Deploy.Tasks.Websites;
using Microsoft.Web.Administration;

namespace Deploy.Soldier.Application.Executors
{
    public class StopWebsiteExecutor : ManageWebsiteExecutor<StopWebsite, StopWebsite.Result>
    {
        protected override StopWebsite.Result ManageWebsite(StopWebsite command, Site website)
        {
            website.Stop();
            if (website.State != ObjectState.Stopped)
            {
                throw new InvalidOperationException(string.Format("Website '{0}' cannot be stopped.", website.Name));
            }

            return new StopWebsite.Result
            {
                Message = string.Format("Website '{0}' was stopped.", website.Name)
            };
        }
    }
}