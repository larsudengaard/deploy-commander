using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Deploy.Pawn.Api.Commands;
using Newtonsoft.Json;

namespace Deploy.Pawn.Api
{
    public class PawnClient
    {
         public Result[] ExecuteCommands(params ICommand[] commands)
         {
             var serializer = new JsonSerializer
             {
                 TypeNameHandling = TypeNameHandling.All
             };

             var sb = new StringBuilder();
             serializer.Serialize(new StringWriter(sb), commands);
             var bytes = Encoding.UTF8.GetBytes(HttpUtility.UrlEncode(sb.ToString()));

             var request = WebRequest.Create("http://ipv4.fiddler:5555/service");
             request.ContentType = "application/x-www-form-urlencoded";
             request.Method = "POST";
             request.ContentLength = bytes.Length;

             using (var requestStream = request.GetRequestStream())
             {
                 requestStream.Write(bytes, 0, bytes.Length);
                 requestStream.Flush();
             }

             WebResponse response = request.GetResponse();
             return serializer.Deserialize<Result[]>(new JsonTextReader(new StreamReader(response.GetResponseStream())));
         }
    }
}