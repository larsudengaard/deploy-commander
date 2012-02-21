using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Handlers
{
    public class ChangePsysicalPathOnWebsiteHandler : ManageWebsiteHandler<ChangePsysicalPathOnWebsite>
    {
        protected override Result ManageWebsite(ChangePsysicalPathOnWebsite command, Site website)
        {
            website.SetAttributeValue("physicalPath", command.NewPath);
            return new Result
            {
                Success = true,
                Message = string.Format("Website '{0}' was stopped.", website.Name)
            };
        }
    }
}