using System;
using System.IO;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;

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
            var result = client.ExecuteCommands(new Package
            {
                FileData = File.ReadAllBytes(@"C:\packages\Energy10.Migrator.zip"), PackageName = "Energy10.Migrator." + DateTime.Now.ToString("ddMMyyyy.hhmmss")
            })[0];

            Result migrationCheckResult = client.ExecuteCommands(new RunExecutable
            {
                ExecutablePath = result.Message + @"\Energy10.Migrator.exe", 
                Arguments = "-listMissingMigrations"
            })[0];

            if (!migrationCheckResult.Message.Contains("None"))
                throw new Exception("Migrations not to be run: " + migrationCheckResult);

            string webVersion = DateTime.Now.ToString("ddMMyyyy.hhmmss");
            Result webPackageResult = client.ExecuteCommands(new Package
            {
                FileData = File.ReadAllBytes(@"C:\packages\Energy10.Web.zip"), PackageName = "Energy10.Web." + webVersion
            })[0];

            Result[] results = client.ExecuteCommands(new CopyFolder
            {
                SourcePath = webPackageResult.Message, DestinationPath = @"C:\Websites\www.energy10.dk\wwwroot_" + webVersion
            }, new ChangePsysicalPathOnWebsite
            {
                NewPath = @"C:\Websites\www.energy10.dk\wwwroot_" + webVersion, WebsiteName = "energy10"
            });

            client.ExecuteCommands(new DeleteFolder
            {
                Path = results[1].Message
            });
        }
    }
}
