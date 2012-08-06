namespace Deploy.Tasks
{
    public interface IResponse
    {
        bool Success { get; }
        string ErrorMessage { get; }
        IResult Result { get; }
    }
}