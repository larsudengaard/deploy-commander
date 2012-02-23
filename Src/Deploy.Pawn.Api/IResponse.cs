namespace Deploy.Pawn.Api
{
    public interface IResponse
    {
        bool Success { get; }
        string ErrorMessage { get; }
        IResult Result { get; }
    }
}