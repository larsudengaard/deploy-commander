using System;

namespace Deploy.Pawn.Api
{
    public class Response<T> : IResponse 
        where T : IResult
    {
        public Response(T result, bool success, Exception exception)
        {
            Result = result;
            Success = success;

            if (exception != null)
            {
                ErrorMessage = exception.Message;
                Stacktrace = exception.StackTrace;
            }
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