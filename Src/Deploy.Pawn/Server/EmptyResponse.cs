namespace Deploy.Pawn.Server
{
    public class EmptyResponse : Response
    {
        public EmptyResponse(int statusCode = 204)
        {
            StatusCode = statusCode;

        }
    }
}