using Deploy.King.Procedures;
using Deploy.King.Procedures.Arguments;

namespace Deploy.King
{
    public interface IDeployProcedureFactory
    {
        IDeployProcedure CreateFor(IProcedureArguments arguments);
    }
}