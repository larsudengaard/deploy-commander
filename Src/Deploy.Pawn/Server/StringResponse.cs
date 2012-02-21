using System.Text;

namespace Deploy.Pawn.Server
{
    public class StringResponse : Response
    {
        public StringResponse(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            WriteStream = s => s.Write(bytes, 0 , bytes.Length);
        }
    }
}