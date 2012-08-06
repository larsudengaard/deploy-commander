using System.IO;
using System.Text;
using System.Web.Mvc;
using Deploy.Soldier.Application;
using Deploy.Soldier.Infrastructure;
using Deploy.Tasks;
using Newtonsoft.Json;

namespace Deploy.Soldier.Controllers
{
    public class ServiceController : Controller
    {
        readonly IPawnService pawnService;

        public ServiceController(IPawnService pawnService)
        {
            this.pawnService = pawnService;
        }

        [AuthorizeWithSecretKey]
        public ActionResult Index()
        {
            var serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.All;
            var task = serializer.Deserialize<ITask>(new JsonTextReader(new StreamReader(Request.InputStream)));

            var response = pawnService.Execute(task);

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), response);
            
            return new ContentResult
            {
                ContentType = "application/json",
                Content = sb.ToString()
            };
        }
    }
}