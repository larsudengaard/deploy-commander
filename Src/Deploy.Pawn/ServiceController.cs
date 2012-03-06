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
using System.Linq;

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
            Console.WriteLine(string.Format("{0}: Received task", DateTime.Now));
            Request request = requestContext.Request;

            string inputString = new StreamReader(request.InputStream).ReadToEnd();
            inputString = HttpUtility.UrlDecode(inputString);

            var serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.All;
            var task = serializer.Deserialize<ITask>(new JsonTextReader(new StringReader(inputString)));

            bool successful = true;
            Exception exception = null;
            IResult result = null;
            try
            {
                ITaskExecutor taskExecutor = taskExecuterFactory.CreateExecuterFor(task);
                Console.WriteLine(string.Format("{0}: Executing {1}", DateTime.Now, task.GetType().Name));
                result = taskExecutor.Execute(task);
            }
            catch(Exception e)
            {
                Console.WriteLine(string.Format("{0}: Error in {1}, {2} ({3})", DateTime.Now, task.GetType().Name, e.GetType().Name, e.Message));
                successful = false;
                exception = e;
            }

            var resultType = task.GetType().GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ITask<>)).GetGenericArguments()[0];

            var sb = new StringBuilder();
            var response = CreateResponse(resultType, result, successful, exception);
            serializer.Serialize(new StringWriter(sb), response);
            requestContext.Respond(new StringResponse(sb.ToString()));
            Console.WriteLine(string.Format("{0}: Task complete", DateTime.Now));
        }

        IResponse CreateResponse(Type resultType, IResult result, bool successful, Exception e)
        {
            Type responseType = typeof(Response<>).MakeGenericType(resultType);
            return (IResponse) Activator.CreateInstance(responseType, result, successful, e);
        }

        public void Stop()
        {
            handler.Dispose();
            server.Dispose();
        }
    }
}