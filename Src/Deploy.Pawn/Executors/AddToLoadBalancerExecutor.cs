using System.IO;
using System.Linq;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure.Executers;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Executors
{
    public class AddToLoadBalancerExecutor : ManageWebsiteExecutor<AddToLoadBalancer, AddToLoadBalancer.Result>
    {
        protected override AddToLoadBalancer.Result ManageWebsite(AddToLoadBalancer task, Site website)
        {
            var applicationRoot = website.Applications.Single(a => a.Path == "/");
            var virtualRoot = applicationRoot.VirtualDirectories.Single(v => v.Path == "/");
            string physicalPath = virtualRoot.PhysicalPath;

            File.Create(physicalPath + task.WatchdogFilename);
            return new AddToLoadBalancer.Result();
        }
    }
}