namespace Deploy.Pawn.Api
{
    public class Response<T> : IResponse 
        where T : IResult
    {
        protected Response()
        {
        }

        public Response(T result, bool success, string errorMessage)
        {
            Result = result;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public T Result { get; private set; }

        IResult IResponse.Result
        {
            get { return Result; }
        }
    }
}