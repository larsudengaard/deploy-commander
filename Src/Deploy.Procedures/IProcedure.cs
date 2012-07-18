using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Procedures.Arguments;
using Deploy.Procedures.Builds;

namespace Deploy.Procedures
{
    public interface IProcedure
    {
        bool Perform(Build build, IProject project);
        string Name { get; }
        TResult ExecuteTask<TResult>(PawnClient client, ITask<TResult> task) where TResult : IResult;
    }
}