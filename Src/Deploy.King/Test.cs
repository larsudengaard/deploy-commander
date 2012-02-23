using System;
using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King
{
    public class Test
    {
        readonly PawnClient client;

        public Test()
        {
            client = new PawnClient();
        }

        public void DoIt()
        {
            var migratorPackageResponse = client.ExecuteTask(new Package
            {
                FileData = File.ReadAllBytes(@"C:\packages\Energy10.Migrator.zip"), PackageName = "Energy10.Migrator." + DateTime.Now.ToString("ddMMyyyy.hhmmss")
            });

            var migrationCheckResponse = client.ExecuteTask(new RunExecutable
            {
                ExecutablePath = migratorPackageResponse.Result.PackagePath + @"\Energy10.Migrator.exe", 
                Arguments = "-listMissingMigrations"
            });

            if (!migrationCheckResponse.Result.Message.Contains("None"))
                throw new Exception("Migrations not to be run: " + migrationCheckResponse.Result.Message);

            string webVersion = DateTime.Now.ToString("ddMMyyyy.hhmmss");
            var webPackageResponse = client.ExecuteTask(new Package
            {
                FileData = File.ReadAllBytes(@"C:\packages\Energy10.Web.zip"), PackageName = "Energy10.Web." + webVersion
            });

            var copyFolderResponse = client.ExecuteTask(new CopyFolder
            {
                SourcePath = webPackageResponse.Result.PackagePath,
                DestinationPath = @"C:\Websites\www.energy10.dk\wwwroot_" + webVersion
            });

            var changePhysicalPathResponse = client.ExecuteTask(new ChangePsysicalPathOnWebsite
            {
                NewPath = @"C:\Websites\www.energy10.dk\wwwroot_" + webVersion, WebsiteName = "energy10"
            });

            client.ExecuteCommands(new DeleteFolder
            {
                Path = changePhysicalPathResponse.Result.OldPath
            });
        }
    }
}
