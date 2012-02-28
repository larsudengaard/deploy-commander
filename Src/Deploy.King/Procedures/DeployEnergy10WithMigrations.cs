using System;
using System.IO;
using Deploy.King.Builds;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures
{
    public class DeployEnergy10WithMigrations : Procedure<ArgumentsForDeployEnergy10WithMigrations>
    {
        public override bool Perform(Build build, ArgumentsForDeployEnergy10WithMigrations arguments)
        {
            if (arguments.DeployWeb2)
                throw new NotImplementedException();

            var migratorPackage = build.GetPackage("Energy10.Migrator");
            var ravenClient = new PawnClient(arguments.RavenDBPawnHostname);
            var migratorPackageResponse = ravenClient.ExecuteTask(new Unpack
            {
                FileData = File.ReadAllBytes(migratorPackage.Path),
                PackageName = migratorPackage.Name
            });

            var migrationCheckResponse = ravenClient.ExecuteTask(new RunExecutable
            {
                ExecutablePath = migratorPackageResponse.Result.PackagePath + @"\Energy10.Migrator.exe",
                Arguments = "-listMissingMigrations"
            });

            if (migrationCheckResponse.Result.Message.StartsWith("None"))
            {
                return false;
            }

            var webClient = new PawnClient(arguments.Web1PawnHostname);
            webClient.ExecuteTask(new StopWebsite
            {
                WebsiteName = arguments.WebsiteName
            });

            var migrationResponse = ravenClient.ExecuteTask(new RunExecutable
            {
                ExecutablePath = migratorPackageResponse.Result.PackagePath + @"\Energy10.Migrator.exe",
                Arguments = "-migrate"
            });

            if (!migrationResponse.Success)
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