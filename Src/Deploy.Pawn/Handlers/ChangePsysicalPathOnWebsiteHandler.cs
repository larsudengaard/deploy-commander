using System.Linq;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Handlers
{
    public class ChangePsysicalPathOnWebsiteExecuter : ManageWebsiteExecuter<ChangePsysicalPathOnWebsite, ChangePsysicalPathOnWebsite.Result>
    {
        protected override ChangePsysicalPathOnWebsite.Result ManageWebsite(ChangePsysicalPathOnWebsite command, Site website)
        {
            var applicationRoot = website.Applications.Single(a => a.Path == "/");
            var virtualRoot = applicationRoot.VirtualDirectories.Single(v => v.Path == "/");
            string oldPhysicalPath = virtualRoot.PhysicalPath;
            virtualRoot.SetAttributeValue("physicalPath", command.NewPath);

            return new ChangePsysicalPathOnWebsite.Result()
            {
                OldPath = oldPhysicalPath
            };
        }
    }
}