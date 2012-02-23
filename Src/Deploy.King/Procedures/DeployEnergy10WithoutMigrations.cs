using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures
{
    public class DeployEnergy10WithoutMigrations : IDeployProcedure<DeployEnergy10WithoutMigrations.Arguments>
    {
        public bool Perform(DeployPackage package, Arguments arguments)
        {
            var migratorPackage = package.GetChildPackage("Energy10.Migrator");
            if (!IsMigrationsMigrationsRun(migratorPackage, arguments))
            {
                return false;
            }

            var webPackage = package.GetChildPackage("Energy10.Web");
            DeployWebServer(arguments.Web1PawnHostname, webPackage, arguments);

            if (arguments.DeployWeb2)
            {
                DeployWebServer(arguments.Web2PawnHostname, webPackage, arguments);
            }

            return true;
        }

        void DeployWebServer(string webServerClientUrl, ChildPackage webPackage, Arguments arguments)
        {
            var client = new PawnClient(webServerClientUrl);
            var webPackageResponse = client.ExecuteTask(new Package
            {
                FileData = File.ReadAllBytes(webPackage.Path),
                PackageName = webPackage.Fullname
            });

            string webPhysicalPath = arguments.WebsitePhysicalPath + webPackage.Fullname;
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

        bool IsMigrationsMigrationsRun(ChildPackage migratorPackage, Arguments arguments)
        {
            var client = new PawnClient(arguments.RavenDBPawnHostname);
            var migratorPackageResponse = client.ExecuteTask(new Package
            {
                FileData = File.ReadAllBytes(migratorPackage.Path),
                PackageName = migratorPackage.Fullname
            });

            var migrationCheckResponse = client.ExecuteTask(new RunExecutable
            {
                ExecutablePath = migratorPackageResponse.Result.PackagePath + @"\Energy10.Migrator.exe",
                Arguments = "-listMissingMigrations"
            });

            return migrationCheckResponse.Result.Message.Contains("None");
        }

        public class Arguments
        {
            public string Web1PawnHostname { get; set; }
            public string WebsitePhysicalPath { get; set; }
            public string WebsiteName { get; set; }

            public bool DeployWeb2 { get; set; }
            public string Web2PawnHostname { get; set; }

            public string RavenDBPawnHostname { get; set; }
        }
    }
}