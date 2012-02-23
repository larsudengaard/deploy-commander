using System;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Handlers
{
    public class StopWebsiteExecuter : ManageWebsiteExecuter<StopWebsite, StopWebsite.Result>
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