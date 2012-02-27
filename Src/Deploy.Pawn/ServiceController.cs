using System;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Web;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;
using Deploy.Pawn.Server;
using Deploy.Utilities;
using Newtonsoft.Json;

namespace Deploy.Pawn
{
    public class ServiceController
    {
        private readonly ITaskExecuterFactory taskExecuterFactory;
        private HttpServer server;
        private IDisposable handler;

        public ServiceController(ITaskExecuterFactory taskExecuterFactory)
        {
            this.taskExecuterFactory = taskExecuterFactory;
        }

        public void Start()
        {
            server = new HttpServer(string.Format("http://*:{0}/", AppSettings.GetInteger("HttpServerPort")));
            handler = server
                    .Where(ctx => ctx.Request.RawUrl.EndsWith("/service"))
                    .Subscribe(ExecuteHandlers);
        }

        private void ExecuteHandlers(RequestContext requestContext)
        {
            Request request = requestContext.Request;

            string inputString = new StreamReader(request.InputStream).ReadToEnd();
            inputString = HttpUtility.UrlDecode(inputString);

            var serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.All;
            var task = serializer.Deserialize<ITask>(new JsonTextReader(new StringReader(inputString)));

            bool successful = true;
            string errorMessage = null;
            IResult result = null;
            try
            {
                ITaskExecutor taskExecutor = taskExecuterFactory.CreateExecuterFor(task);
                result = taskExecutor.Execute(task);
            }
            catch(Exception e)
            {
                successful = false;
                errorMessage = e.Message;
            }

            var sb = new StringBuilder();
            var response = CreateResponse(result, successful, errorMessage);
            serializer.Serialize(new StringWriter(sb), response);
            requestContext.Respond(new StringResponse(sb.ToString()));
        }

        IResponse CreateResponse(IResult result, bool successful, string errorMessage)
        {
            Type responseType = typeof (Response<>).MakeGenericType(result.GetType());
            return (IResponse) Activator.CreateInstance(responseType, result, successful, errorMessage);
        }

        public void Stop()
        {
            handler.Dispose();
            server.Dispose();
        }
    }
}