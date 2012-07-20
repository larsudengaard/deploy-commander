using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Procedures;
using Deploy.Procedures.Messaging;
using Newtonsoft.Json;

namespace Deploy.King.Executor
{
    public class PawnClient : IPawnClient
    {
        readonly IMessenger messenger;
        readonly string authenticationSecretKey;
        readonly bool allowNonTrustedPawnCertificate;
        readonly string clientUrl;

        public PawnClient(IMessenger messenger, string clientUrl, string authenticationSecretKey, bool allowNonTrustedPawnCertificate = false)
        {
            this.messenger = messenger;
            this.authenticationSecretKey = authenticationSecretKey;
            this.allowNonTrustedPawnCertificate = allowNonTrustedPawnCertificate;
            clientUrl = clientUrl.EndsWith("/") ? clientUrl.Remove(clientUrl.Length - 1) : clientUrl;
            this.clientUrl = !clientUrl.EndsWith("/service") ? clientUrl + "/service" : "";
        }

        public string ClientUrl
        {
            get { return clientUrl; }
        }

        TResult IPawnClient.ExecuteTask<TResult>(ITask<TResult> task)
        {
            var message = string.Format("{0}: Task {1}", ClientUrl, task.GetType().Name);

            var properties = task.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            foreach (var property in properties)
            {
                message += string.Format("\n{0}: {1}", property.Name, property.GetValue(task, null));
            }

            messenger.Publish(message);

            var response = ExecuteTask(task);
            if (!response.Success)
            {
                messenger.Publish(string.Format("Task error: {0}\n{1}", response.ErrorMessage, response.StackTrace));
                throw new TaskFailedException(task);
            }

            return response.Result;
        }

        public Response<TResult> ExecuteTask<TResult>(ITask<TResult> task) where TResult : IResult
        {
            var serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), task);
            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());

            WebRequest request = WebRequest.Create(ClientUrl);
            request.Timeout = (int) TimeSpan.FromMinutes(30).TotalMilliseconds;
            request.Headers.Add("pawnKey", authenticationSecretKey);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            
            if (allowNonTrustedPawnCertificate)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            WebResponse response = request.GetResponse();
            return serializer.Deserialize<Response<TResult>>(new JsonTextReader(new StreamReader(response.GetResponseStream())));
        }
    }
}