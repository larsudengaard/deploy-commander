using System.IO;
using System.Net;
using System.Text;
using Deploy.Pawn.Api.Tasks;
using Deploy.Utilities;
using Newtonsoft.Json;

namespace Deploy.Pawn.Api
{
    public class PawnClient
    {
        readonly string clientUrl;
        public const string AuthenticationKeyHeaderName = "pawnKey";

        public PawnClient(string clientUrl)
        {
            clientUrl = clientUrl.EndsWith("/") ? clientUrl.Remove(clientUrl.Length - 1) : clientUrl;
            this.clientUrl = !clientUrl.EndsWith("/service") ? clientUrl + "/service" : "";
        }

        public string ClientUrl
        {
            get { return clientUrl; }
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
            request.Headers.Add(AuthenticationKeyHeaderName, AppSettings.GetString("AuthenticationSecretKey"));
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            
            if (AppSettings.GetBoolean("AllowNonTrustedPawnCertificate", false))
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            WebResponse response = request.GetResponse();
            return serializer.Deserialize<Response<TResult>>(new JsonTextReader(new StreamReader(response.GetResponseStream())));
        }
    }
}