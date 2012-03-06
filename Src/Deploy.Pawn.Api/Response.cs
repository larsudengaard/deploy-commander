using System;
using Newtonsoft.Json;

namespace Deploy.Pawn.Api
{
    public class Response<T> : IResponse 
        where T : IResult
    {
        [JsonConstructor]
        public Response(T result, bool success, string errorMessage, string stackTrace)
        {
            Result = result;
            Success = success;
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
        }

        public Response(T result, bool success, Exception exception)
        {
            Result = result;
            Success = success;

            if (exception != null)
            {
                ErrorMessage = exception.Message;
                StackTrace = exception.StackTrace;
            }
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public string StackTrace { get; private set; }
        public T Result { get; private set; }

        IResult IResponse.Result
        {
            get { return Result; }
        }
    }
}