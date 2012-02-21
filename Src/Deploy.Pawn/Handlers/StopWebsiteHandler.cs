using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Handlers
{
    public class StopWebsiteHandler : ManageWebsiteHandler<StopWebsite>
    {
        protected override Result ManageWebsite(StopWebsite command, Site website)
        {
            website.Stop();
            if (website.State != ObjectState.Stopped)
            {
                return new Result
                {
                    Success = false,
                    Message = string.Format("Website '{0}' cannot be stopped.", website.Name)
                };
            }

            return new Result
            {
                Success = true,
                Message = string.Format("Website '{0}' was stopped.", website.Name)
            };
        }
    }
}