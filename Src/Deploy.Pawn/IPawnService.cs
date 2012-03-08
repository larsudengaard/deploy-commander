using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn
{
    public interface IPawnService
    {
        IResponse Execute(ITask task);
    }
}