using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Deploy.King.Host.Infrastructure
{
    public class JsonNetResult : JsonResult
    {
        readonly JsonSerializer serializer;

        public JsonNetResult()
        {
            serializer = new JsonSerializer();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType)
                                       ? ContentType
                                       : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data == null)
                return;

            var sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), Data);
            response.Write(sb.ToString());
            //serializer.Serialize(new StreamWriter(response.OutputStream), Data);
        }
    }
}