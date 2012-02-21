namespace Deploy.Pawn.Handlers
{
    public class Executable : PackageCommandHandler<PackagePayload>
    {
        protected override void Run()
        {
            Execute();
        }
    }
}