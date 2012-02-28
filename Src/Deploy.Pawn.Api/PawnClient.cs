using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Deploy.Pawn.Api.Tasks;
using Newtonsoft.Json;

namespace Deploy.Pawn.Api
{
    public class PawnClient
    {
        readonly string clientUrl;

        public PawnClient(string clientUrl)
        {
            clientUrl = clientUrl.EndsWith("/") ? clientUrl.Remove(clientUrl.Length - 1) : clientUrl;
            this.clientUrl = !clientUrl.EndsWith("/service") ? clientUrl + "/service" : "";
        }

        public Response<TResult> ExecuteTask<TResult>(ITask<TResult> task) where TResult : IResult
         {
             var serializer = new JsonSerializer
             {
                 TypeNameHandling = TypeNameHandling.All
             };
             var sb = new StringBuilder();
             serializer.Serialize(new StringWriter(sb), task);
             var bytes = Encoding.UTF8.GetBytes(HttpUtility.UrlEncode(sb.ToString()));

             var request = WebRequest.Create(clientUrl);
             request.ContentType = "application/x-www-form-urlencoded";
             request.Method = "POST";
             request.ContentLength = bytes.Length;

             using (var requestStream = request.GetRequestStream())
             {
                 requestStream.Write(bytes, 0, bytes.Length);
             }

             WebResponse response = request.GetResponse();
             return serializer.Deserialize<Response<TResult>>(new JsonTextReader(new StreamReader(response.GetResponseStream())));
         }
    }
}