using System;
using System.Web.Mvc;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;

namespace Deploy.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Start()
        {
            var client = new PawnClient();
            Result[] results = client.ExecuteCommands(new StartWebsite
            {
                WebsiteName = "ravendb"
            });

            ViewBag.Results = results;
            return View("Index");
        }

        public ActionResult Stop()
        {
            var client = new PawnClient();
            Result[] results = client.ExecuteCommands(new StopWebsite
            {
                WebsiteName = "ravendb"
            });

            ViewBag.Results = results;
            return View("Index");
        }

        public ActionResult RunExecutable()
        {
            var client = new PawnClient();
            Result[] results = client.ExecuteCommands(new RunExecutable
            {
                ExecutablePath = @"C:\workspace\Energy10\Src\Energy10.Migrator\bin\Debug\Energy10.Migrator.exe",
                Arguments = "-listMissingMigrations"
            });

            ViewBag.Results = results;
            return View("Index");
        }

        public ActionResult Package()
        {
            var client = new PawnClient();
            Result[] results = client.ExecuteCommands(new Package
            {
                PackageName = @"Energy10.Web.10101010",
                FileData = System.IO.File.ReadAllBytes(@"C:\test.zip")
            });

            ViewBag.Results = results;
            return View("Index");
        }
    }
}
