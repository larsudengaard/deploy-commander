using System;
using System.Collections.Generic;
using System.IO;
using Deploy.King.Builds;
using Deploy.King.Messaging;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Package = Deploy.King.Builds.Package;

namespace Deploy.King.Procedures
{
    public class DeployEnergy10 : Procedure<ArgumentsForDeployEnergy10>
    {
        public DeployEnergy10(IMessenger messenger) : base(messenger)
        {
        }

        public override bool Perform(Build build, ArgumentsForDeployEnergy10 arguments)
        {
            if (arguments.DeployWeb2)
                throw new NotImplementedException();

            var migratorPackage = GetPackage(build, "Energy10.Migrator");
            if (!IsMigrationsMigrationsRun(migratorPackage, arguments))
            {
                return false;
            }

            var webPackage = GetPackage(build, "Energy10.Web");
            DeployWebServer(arguments.Web1PawnHostname, build, webPackage, arguments);

            return true;
        }

        void DeployWebServer(string webServerClientUrl, Build build, Package webPackage, ArgumentsForDeployEnergy10 arguments)
        {
            var client = new PawnClient(webServerClientUrl);
            var webPackageResult = ExecuteTask(client, new Unpack
            {
                FileData = File.ReadAllBytes(webPackage.Path),
                PackageName = webPackage.Name
            });

            string webPhysicalPath = arguments.WebsitePhysicalPath + "wwwroot_" + build.Number;
            ExecuteTask(client, new CopyFolder
            {
                SourcePath = webPackageResult.PackagePath,
                DestinationPath = webPhysicalPath
            });
            Messenger.Publish(string.Format("Copied folder from {0} to {1}", webPackageResult.PackagePath, webPhysicalPath));

            var changePhysicalPathResult = ExecuteTask(client, new ChangePhysicalPathOnWebsite
            {
                NewPath = webPhysicalPath,
                WebsiteName = arguments.WebsiteName
            });
            Messenger.Publish(string.Format("Changed physicalPath from {0} to {1} on website {2}", webPhysicalPath, changePhysicalPathResult.OldPath, arguments.WebsiteName));

            ExecuteTask(client, new DeleteFolder
            {
                Path = changePhysicalPathResult.OldPath
            });
            Messenger.Publish(string.Format("Deleted folder {0}", changePhysicalPathResult.OldPath));
        }

        bool IsMigrationsMigrationsRun(Package migratorPackage, ArgumentsForDeployEnergy10 arguments)
        {
            var client = new PawnClient(arguments.RavenDBPawnHostname);
            var migratorPackageResult = ExecuteTask(client, new Unpack
            {
                FileData = File.ReadAllBytes(migratorPackage.Path),
                PackageName = migratorPackage.Name
            });

            var migrationCheckResult = ExecuteTask(client, new RunExecutable
            {
                ExecutablePath = migratorPackageResult.PackagePath + @"\Energy10.Migrator.exe",
                Arguments = "-listMissingMigrations"
            });

            return migrationCheckResult.Message.StartsWith("None");
        }
    }
}