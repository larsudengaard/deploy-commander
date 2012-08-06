using System.IO;
using System.Linq;
using Deploy.Soldier.Application.Infrastructure.Executers;
using Deploy.Tasks.Websites;
using Microsoft.Web.Administration;

namespace Deploy.Soldier.Application.Executors
{
    public class AddToLoadBalancerExecutor : ManageWebsiteExecutor<AddToLoadBalancer, AddToLoadBalancer.Result>
    {
        protected override AddToLoadBalancer.Result ManageWebsite(AddToLoadBalancer task, Site website)
        {
            var applicationRoot = website.Applications.Single(a => a.Path == "/");
            var virtualRoot = applicationRoot.VirtualDirectories.Single(v => v.Path == "/");
            string physicalPath = virtualRoot.PhysicalPath;

            using (File.Create(physicalPath + "\\" + task.WatchdogFilename));
            return new AddToLoadBalancer.Result();
        }
    }
}