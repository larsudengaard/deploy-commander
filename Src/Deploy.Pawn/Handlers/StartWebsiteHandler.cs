using System;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Handlers
{
    public class StartWebsiteExecuter : ManageWebsiteExecuter<StartWebsite, StartWebsite.Result>
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