using System;
using System.IO;
using Deploy.King.Builds;
using Deploy.King.Messaging;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures
{
    public class DeployEnergy10WithMigrations : Procedure<ArgumentsForDeployEnergy10WithMigrations>
    {
        readonly IMessenger messenger;

        public DeployEnergy10WithMigrations(IMessenger messenger) : base(messenger)
        {
            this.messenger = messenger;
        }

        public override bool Perform(Build build, ArgumentsForDeployEnergy10WithMigrations arguments)
        {
            if (arguments.DeployWeb2)
                throw new NotImplementedException();

            var migratorPackage = GetPackage(build, "Energy10.Migrator");
            var ravenClient = new PawnClient(arguments.RavenDBPawnHostname);
            var migratorPackageResult = ExecuteTask(ravenClient, new Unpack
            {
                FileData = File.ReadAllBytes(migratorPackage.Path),
                PackageName = migratorPackage.Name
            });

            var migrationCheckResult = ExecuteTask(ravenClient, new RunExecutable
            {
                ExecutablePath = migratorPackageResult.PackagePath + @"\Energy10.Migrator.exe",
                Arguments = "-listMissingMigrations"
            });

            if (migrationCheckResult.Message.StartsWith("None"))
            {
                Messenger.Publish("No migrations found. Please run DeployEnerg10 procedure instead.");
                return false;
            }
            Messenger.Publish("Found migrations: " + migrationCheckResult.Message);

            var webClient = new PawnClient(arguments.Web1PawnHostname);
            ExecuteTask(webClient, new StopWebsite
            {
                WebsiteName = arguments.WebsiteName
            });

            var migrationResult = ExecuteTask(ravenClient, new RunExecutable
            {
                ExecutablePath = migratorPackageResult.PackagePath + @"\Energy10.Migrator.exe",
                Arguments = "-migrate"
            });

            if (!migrationResult.Success)
                return false;

            var webPackage = build.GetPackage("Energy10.Web");
            var webPackageResponse = webClient.ExecuteTask(new Unpack
            {
                FileData = File.ReadAllBytes(webPackage.Path),
                PackageName = webPackage.Name
            });

            string webPhysicalPath = arguments.WebsitePhysicalPath + "wwwroot_" + build.Number;
            var copyFolderResponse = webClient.ExecuteTask(new CopyFolder
            {
                SourcePath = webPackageResponse.Result.PackagePath,
                DestinationPath = webPhysicalPath
            });

            var changePhysicalPathResponse = webClient.ExecuteTask(new ChangePhysicalPathOnWebsite
            {
                NewPath = webPhysicalPath,
                WebsiteName = arguments.WebsiteName
            });

            webClient.ExecuteTask(new DeleteFolder
            {
                Path = changePhysicalPathResponse.Result.OldPath
            });

            webClient.ExecuteTask(new StartWebsite
            {
                WebsiteName = arguments.WebsiteName
            });

            return true;
        }
    }
}