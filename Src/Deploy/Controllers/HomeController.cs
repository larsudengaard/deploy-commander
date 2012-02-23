using System;
using System.Web.Mvc;
using Deploy.King;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

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
            var result = client.ExecuteTask(new StartWebsite
            {
                WebsiteName = "ravendb"
            });

            ViewBag.Result = result;
            return View("Index");
        }

        public ActionResult Test()
        {
            var test = new Test();
            test.DoIt();

            return View("Index");
        }

        public ActionResult Stop()
        {
            var client = new PawnClient();
            var result = client.ExecuteTask(new StopWebsite
            {
                WebsiteName = "ravendb"
            });
            
            ViewBag.Result = result;
            return View("Index");
        }

        public ActionResult RunExecutable()
        {
            var client = new PawnClient();
            var result = client.ExecuteTask(new RunExecutable
            {
                ExecutablePath = @"C:\workspace\Energy10\Src\Energy10.Migrator\bin\Debug\Energy10.Migrator.exe",
                Arguments = "-listMissingMigrations"
            });

            ViewBag.Result = result;
            return View("Index");
        }

        public ActionResult Package()
        {
            var client = new PawnClient();
            var result = client.ExecuteTask(new Package
            {
                PackageName = @"Energy10.Web.10101010",
                FileData = System.IO.File.ReadAllBytes(@"C:\test.zip")
            });

            ViewBag.Result = result;
            return View("Index");
        }
    }
}
