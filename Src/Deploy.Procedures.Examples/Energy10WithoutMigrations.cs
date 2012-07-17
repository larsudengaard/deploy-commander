using Deploy.Pawn.Api;
using Deploy.Procedures.Builds;
using Deploy.Procedures.Examples.Arguments;
using Deploy.Procedures.Messaging;

namespace Deploy.Procedures.Examples
{
    public class Energy10WithoutMigrations : Energy10Base<ArgumentsForEnergy10WithoutMigrations>
    {
        public Energy10WithoutMigrations(IMessenger messenger) : base(messenger)
        {
        }

        public override bool Perform(Build build, ArgumentsForEnergy10WithoutMigrations arguments)
        {
            string migratorExecutablePath;
            string missingMigrations = GetMissingMigrations(new PawnClient(arguments.RavenDBPawnHostname, "kehi52page", true), build, arguments, out migratorExecutablePath);
            if (!missingMigrations.StartsWith("None"))
            {
                Messenger.Publish(string.Format("Migrations need to be run: {0}. Please run DeployEnerg10WithMigrations procedure instead.", missingMigrations));
                return false;
            }

            DeployWebServer(new PawnClient(arguments.Web1PawnHostname, "kehi52page", true), build, arguments);
            
            if (arguments.DeployWeb2)
                DeployWebServer(new PawnClient(arguments.Web2PawnHostname, "kehi52page", true), build, arguments);

            return true;
        }
    }
}