using System.Linq;
using Deploy.Soldier.Application.Infrastructure.Executers;
using Deploy.Tasks.Websites;
using Microsoft.Web.Administration;

namespace Deploy.Soldier.Application.Executors
{
    public class ChangePhysicalPathOnWebsiteExecutor : ManageWebsiteExecutor<ChangePhysicalPathOnWebsite, ChangePhysicalPathOnWebsite.Result>
    {
        protected override ChangePhysicalPathOnWebsite.Result ManageWebsite(ChangePhysicalPathOnWebsite command, Site website)
        {
            var applicationRoot = website.Applications.Single(a => a.Path == "/");
            var virtualRoot = applicationRoot.VirtualDirectories.Single(v => v.Path == "/");
            string oldPhysicalPath = virtualRoot.PhysicalPath;
            virtualRoot.SetAttributeValue("physicalPath", command.NewPath);

            return new ChangePhysicalPathOnWebsite.Result
            {
                OldPath = oldPhysicalPath
            };
        }
    }
}