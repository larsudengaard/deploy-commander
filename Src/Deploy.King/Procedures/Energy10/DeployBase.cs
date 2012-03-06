using System;
using System.IO;
using System.Threading;
using Deploy.King.Builds;
using Deploy.King.Messaging;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures.Energy10
{
    public abstract class DeployBase<TArguments> : Procedure<TArguments> where TArguments : IDeployEnergy10BaseArguments
    {
        protected DeployBase(IMessenger messenger) : base(messenger)
        {
        }

        protected string GetMissingMigrations(PawnClient ravendbClient, Build build, TArguments arguments, out string migratorExecutablePath)
        {
            var migratorPackage = build.GetPackage("Energy10.Migrator");
            var migratorPackageResult = ExecuteTask(ravendbClient, new Unpack
            {
                FileData = File.ReadAllBytes(migratorPackage.Path),
                PackageName = migratorPackage.Name
            });

            migratorExecutablePath = migratorPackageResult.PackagePath + @"\Energy10.Migrator.exe";
            var migrationCheckResult = ExecuteTask(ravendbClient, new RunExecutable
            {
                ExecutablePath = migratorExecutablePath,
                Arguments = "-listMissingMigrations"
            });

            return migrationCheckResult.Message;
        }

        protected void DeployWebServer(PawnClient webserverClient, Build build, TArguments arguments)
        {
            var webPackage = build.GetPackage("Energy10.Web");
            var webPackageResult = ExecuteTask(webserverClient, new Unpack
            {
                FileData = File.ReadAllBytes(webPackage.Path),
                PackageName = webPackage.Name
            });

            var websiteName = arguments.WebsiteName;
            var websitePhysicalPath = arguments.WebsitePhysicalPath;

            string webPhysicalPath = websitePhysicalPath + "wwwroot_" + build.Number;
            ExecuteTask(webserverClient, new CopyFolder
            {
                SourcePath = webPackageResult.PackagePath,
                DestinationPath = webPhysicalPath
            });
            Messenger.Publish(string.Format("Copied folder from {0} to {1}", webPackageResult.PackagePath, webPhysicalPath));

            var changePhysicalPathResult = ExecuteTask(webserverClient, new ChangePhysicalPathOnWebsite
            {
                NewPath = webPhysicalPath,
                WebsiteName = websiteName
            });
            Messenger.Publish(string.Format("Changed physicalPath from {0} to {1} on website {2}", webPhysicalPath, changePhysicalPathResult.OldPath, websiteName));

            if (changePhysicalPathResult.OldPath != webPhysicalPath)
            {
                DeleteFolder.Result result = null;
                var retryCount = 5;
                do
                {
                    try
                    {
                        result = ExecuteTask(webserverClient, new DeleteFolder
                        {
                            Path = changePhysicalPathResult.OldPath
                        });
                    }
                    catch (TaskFailedException)
                    {
                        retryCount--;
                        Thread.Sleep(500);
                    }
                } while (result == null && retryCount != 0);

                Messenger.Publish(string.Format("Deleted folder {0}", changePhysicalPathResult.OldPath));
            }
        }
    }
}