using System;
using System.IO;
using Deploy.King.Builds;
using Deploy.King.Messaging;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures.Energy10
{
    public class DeployWithMigrations : DeployBase<ArgumentsForDeployEnergy10WithMigrations>
    {
        public DeployWithMigrations(IMessenger messenger) : base(messenger)
        {
        }

        public override bool Perform(Build build, ArgumentsForDeployEnergy10WithMigrations arguments)
        {
            if (arguments.DeployWeb2)
                throw new NotImplementedException();

            var ravendbClient = new PawnClient(arguments.RavenDBPawnHostname);
            string migratorExecutablePath;
            var missingMigrations = GetMissingMigrations(ravendbClient, build, arguments, out migratorExecutablePath);

            if (missingMigrations.StartsWith("None"))
            {
                Messenger.Publish("No migrations found. Please run DeployEnerg10 procedure instead.");
                return false;
            }

            var webClient = new PawnClient(arguments.Web1PawnHostname);
            ExecuteTask(webClient, new StopWebsite
            {
                WebsiteName = arguments.WebsiteName
            });

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

            DeployWebServer(webClient, build, arguments);

            ExecuteTask(webClient, new StartWebsite { WebsiteName = arguments.WebsiteName });

            return true;
        }
    }
}