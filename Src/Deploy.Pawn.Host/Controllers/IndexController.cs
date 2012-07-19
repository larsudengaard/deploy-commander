using System;
using System.Web.Mvc;

namespace Deploy.Pawn.Host.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            return new ContentResult
            {
                ContentType = "text/plain",
                Content = string.Format("{0:HH}00 HOURS, Huah! ({0:HH:mm:ss})", DateTime.Now)
            };
        }
    }
}