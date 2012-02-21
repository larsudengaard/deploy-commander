using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Web;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;
using Deploy.Pawn.Server;
using Newtonsoft.Json;

namespace Deploy.Pawn
{
    public class ServiceController
    {
        private readonly ICommandHandlerFactory commandHandlerFactory;
        private HttpServer server;
        private IDisposable handler;

        public ServiceController(ICommandHandlerFactory commandHandlerFactory)
        {
            this.commandHandlerFactory = commandHandlerFactory;
        }

        public void Start()
        {
            server = new HttpServer("http://*:5555/");
            handler = server
                    .Where(ctx => ctx.Request.RawUrl.EndsWith("/service"))
                    .Subscribe(ExecuteHandlers);
        }

        private void ExecuteHandlers(RequestContext requestContext)
        {
            Request request = requestContext.Request;

            var buffer = new byte[request.ContentLength];
            request.InputStream.Read(buffer, 0, request.ContentLength);
            string inputString = Encoding.UTF8.GetString(buffer);
            inputString = HttpUtility.UrlDecode(inputString);

            var serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.All;
            var commands = serializer.Deserialize<ICommand[]>(new JsonTextReader(new StringReader(inputString)));
            
            var results = new List<Result>();
            foreach (var command in commands)
            {
                ICommandHandler commandHandler = commandHandlerFactory.CreateHandlerFor(command);
                results.Add(commandHandler.Handle(command));
            }

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), results.ToArray());
            requestContext.Respond(new StringResponse(sb.ToString()));
        }

        public void Stop()
        {
            handler.Dispose();
            server.Dispose();
        }
    }
}