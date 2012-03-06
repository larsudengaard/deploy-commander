using System.IO;
using System.Linq;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure.Executers;
using Microsoft.Web.Administration;

namespace Deploy.Pawn.Executors
{
    public class RemoveFromLoadBalancerExecutor : ManageWebsiteExecutor<RemoveFromLoadBalancer, RemoveFromLoadBalancer.Result>
    {
        protected override RemoveFromLoadBalancer.Result ManageWebsite(RemoveFromLoadBalancer task, Site website)
        {
            var applicationRoot = website.Applications.Single(a => a.Path == "/");
            var virtualRoot = applicationRoot.VirtualDirectories.Single(v => v.Path == "/");
            string physicalPath = virtualRoot.PhysicalPath;

            File.Delete(physicalPath + "\\" + task.WatchdogFilename);
            return new RemoveFromLoadBalancer.Result();
        }
    }
}