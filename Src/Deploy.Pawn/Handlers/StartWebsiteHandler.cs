using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Handlers
{
    public class StartWebsiteHandler : ManageWebsiteHandler<StartWebsite>
    {
        protected override Result ManageWebsite(StartWebsite command, Site website)
        {
            website.Start();
            if (website.State != ObjectState.Started)
            {
                return new Result
                {
                    Success = false,
                    Message = string.Format("Website '{0}' cannot be started.", command.WebsiteName)
                };
            }

            return new Result
            {
                Success = true,
                Message = string.Format("Website '{0}' was started.", command.WebsiteName)
            };
        }
    }
}