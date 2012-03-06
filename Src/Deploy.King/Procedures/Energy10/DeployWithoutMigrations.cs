using System;
using System.IO;
using Deploy.King.Builds;
using Deploy.King.Messaging;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures.Energy10
{
    public class DeployWithoutMigrations : DeployBase<ArgumentsForDeployEnergy10>
    {
        public DeployWithoutMigrations(IMessenger messenger) : base(messenger)
        {
        }

        public override bool Perform(Build build, ArgumentsForDeployEnergy10 arguments)
        {
            string migratorExecutablePath;
            string missingMigrations = GetMissingMigrations(new PawnClient(arguments.RavenDBPawnHostname), build, arguments, out migratorExecutablePath);
            if (!missingMigrations.StartsWith("None"))
            {
                Messenger.Publish(string.Format("Migrations need to be run: {0}. Please run DeployEnerg10WithMigrations procedure instead.", missingMigrations));
                return false;
            }

            DeployWebServer(new PawnClient(arguments.Web1PawnHostname), build, arguments);
            
            if (arguments.DeployWeb2)
                DeployWebServer(new PawnClient(arguments.Web2PawnHostname), build, arguments);

            return true;
        }
    }
}