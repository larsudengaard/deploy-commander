using System;
using Deploy.Soldier.Application.Infrastructure.Executers;
using Deploy.Tasks.Websites;
using Microsoft.Web.Administration;

namespace Deploy.Soldier.Application.Executors
{
    public class StartWebsiteExecutor : ManageWebsiteExecutor<StartWebsite, StartWebsite.Result>
    {
        protected override StartWebsite.Result ManageWebsite(StartWebsite command, Site website)
        {
            website.Start();
            if (website.State != ObjectState.Started)
            {
                throw new InvalidOperationException(string.Format("Website '{0}' cannot be started.", command.WebsiteName));
            }

            return new StartWebsite.Result
            {
                Message = string.Format("Website '{0}' was started.", command.WebsiteName)
            };
        }
    }
}