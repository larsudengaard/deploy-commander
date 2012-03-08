using System.IO;
using System.Text;
using System.Web.Mvc;
using Deploy.Pawn.Api.Tasks;
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

        public ActionResult Service(string data)
        {
            var serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.All;
            var task = serializer.Deserialize<ITask>(new JsonTextReader(new StringReader(data)));

            var response = pawnService.Execute(task);

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), response);
            
            //Console.WriteLine(string.Format("{0}: Task complete", DateTime.Now));

            return new ContentResult
            {
                ContentType = "application/json",
                Content = sb.ToString()
            };
        }
    }
}