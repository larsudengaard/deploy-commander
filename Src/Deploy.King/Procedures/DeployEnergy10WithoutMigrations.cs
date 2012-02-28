using System.Collections.Generic;
using System.IO;
using Deploy.King.Builds;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Package = Deploy.King.Builds.Package;

namespace Deploy.King.Procedures
{
    public class DeployEnergy10WithoutMigrations : Procedure<ArgumentsForDeployEnergy10WithoutMigrations>
    {
        public override bool Perform(Build build, ArgumentsForDeployEnergy10WithoutMigrations arguments)
        {
            var migratorPackage = build.GetPackage("Energy10.Migrator");
            if (!IsMigrationsMigrationsRun(migratorPackage, arguments))
            {
                return false;
            }

            var webPackage = build.GetPackage("Energy10.Web");
            DeployWebServer(arguments.Web1PawnHostname, build, webPackage, arguments);

            if (arguments.DeployWeb2)
            {
                DeployWebServer(arguments.Web2PawnHostname, build, webPackage, arguments);
            }

            return true;
        }

        void DeployWebServer(string webServerClientUrl, Build build, Package webPackage, ArgumentsForDeployEnergy10WithoutMigrations arguments)
        {
            var client = new PawnClient(webServerClientUrl);
            var webPackageResponse = client.ExecuteTask(new Unpack
            {
                FileData = File.ReadAllBytes(webPackage.Path),
                PackageName = webPackage.Name
            });

            string webPhysicalPath = arguments.WebsitePhysicalPath + webPackage.Name + build.Number;
            var copyFolderResponse = client.ExecuteTask(new CopyFolder
            {
                SourcePath = webPackageResponse.Result.PackagePath,
                DestinationPath = webPhysicalPath
            });

            var changePhysicalPathResponse = client.ExecuteTask(new ChangePhysicalPathOnWebsite
            {
                NewPath = webPhysicalPath,
                WebsiteName = arguments.WebsiteName
            });

            client.ExecuteTask(new DeleteFolder
            {
                Path = changePhysicalPathResponse.Result.OldPath
            });
        }

        bool IsMigrationsMigrationsRun(Package migratorPackage, ArgumentsForDeployEnergy10WithoutMigrations arguments)
        {
            var client = new PawnClient(arguments.RavenDBPawnHostname);
            var migratorPackageResponse = client.ExecuteTask(new Unpack
            {
                FileData = File.ReadAllBytes(migratorPackage.Path),
                PackageName = migratorPackage.Name
            });

            var migrationCheckResponse = client.ExecuteTask(new RunExecutable
            {
                ExecutablePath = migratorPackageResponse.Result.PackagePath + @"\Energy10.Migrator.exe",
                Arguments = "-listMissingMigrations"
            });

            return migrationCheckResponse.Result.Message.Contains("None");
        }
    }
}