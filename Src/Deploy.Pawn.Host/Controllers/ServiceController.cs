using System.IO;
using System.Text;
using System.Web.Mvc;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Host.Infrastructure;
using Newtonsoft.Json;

namespace Deploy.Pawn.Host.Controllers
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