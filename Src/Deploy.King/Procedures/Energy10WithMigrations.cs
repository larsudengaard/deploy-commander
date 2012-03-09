using System.Threading;
using Deploy.King.Builds;
using Deploy.King.Messaging;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures
{
    public class Energy10WithMigrations : Energy10Base<ArgumentsForEnergy10WithMigrations>
    {
        public Energy10WithMigrations(IMessenger messenger) : base(messenger)
        {
        }

        public override bool Perform(Build build, ArgumentsForEnergy10WithMigrations arguments)
        {
            var ravendbClient = new PawnClient(arguments.RavenDBPawnHostname);
            string migratorExecutablePath;
            var missingMigrations = GetMissingMigrations(ravendbClient, build, arguments, out migratorExecutablePath);

            if (missingMigrations.StartsWith("None"))
            {
                Messenger.Publish("No migrations found. Please run DeployEnerg10 procedure instead.");
                return false;
            }

            PawnClient web1Client = new PawnClient(arguments.Web1PawnHostname);
            PawnClient web2Client = null;
            if (arguments.DeployWeb2)
                web2Client = new PawnClient(arguments.Web2PawnHostname);

            ExecuteTask(web1Client, new RemoveFromLoadBalancer
            {
                WatchdogFilename = "loadbalancer",
                WebsiteName = arguments.WebsiteName
            });

            if (arguments.DeployWeb2)
                ExecuteTask(web2Client, new RemoveFromLoadBalancer
                {
                    WatchdogFilename = "loadbalancer",
                    WebsiteName = arguments.WebsiteName
                });

            // Give loadbalancer time to see sites are remove from balancing
            Messenger.Publish("Waiting 3 seconds for loadbalancer to react");
            Thread.Sleep(3000);

            Messenger.Publish("Migrations which will be run: " + missingMigrations);
            var migrationResult = ExecuteTask(ravendbClient, new RunExecutable
            {
                ExecutablePath = migratorExecutablePath,
                Arguments = "-migrate"
            });

            Messenger.Publish(migrationResult.Message);
            if (!migrationResult.Success)
            {
                Messenger.Publish("Migrations was not succesfull: " + migrationResult.Message);
                return false;
            }

            DeployWebServer(web1Client, build, arguments);
            if (arguments.DeployWeb2)
                DeployWebServer(web2Client, build, arguments);

            ExecuteTask(web1Client, new AddToLoadBalancer
            {
                WatchdogFilename = "loadbalancer", 
                WebsiteName = arguments.WebsiteName
            });

            if (arguments.DeployWeb2)
                ExecuteTask(web1Client, new AddToLoadBalancer
                {
                    WatchdogFilename = "loadbalancer",
                    WebsiteName = arguments.WebsiteName
                });

            return true;
        }
    }
}