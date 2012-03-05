namespace Deploy.Pawn.Api
{
    public class Response<T> : IResponse 
        where T : IResult
    {
        public Response(T result, bool success, string errorMessage, string stacktrace)
        {
            Stacktrace = stacktrace;
            Result = result;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public string Stacktrace { get; private set; }
        public T Result { get; private set; }

        IResult IResponse.Result
        {
            get { return Result; }
        }
    }
}