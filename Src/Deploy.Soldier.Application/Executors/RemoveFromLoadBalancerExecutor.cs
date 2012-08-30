using System.IO;
using System.Linq;
using Deploy.Soldier.Application.Infrastructure.Executers;
using Deploy.Tasks.Websites;
using Microsoft.Web.Administration;

namespace Deploy.Soldier.Application.Executors
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