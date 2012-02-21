using System;
using System.Collections.Generic;
using System.IO;

namespace Deploy.Pawn.Server
{
    public class Response
    {
        public Response()
        {
            WriteStream = s => { };
            StatusCode = 200;
            Headers = new Dictionary<string, string>{{"Content-Type", "text/html"}};
        }

        public int StatusCode { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public Action<Stream> WriteStream { get; set; }
    }
}