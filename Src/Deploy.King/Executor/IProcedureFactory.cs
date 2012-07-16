using Deploy.Procedures;
using Deploy.Procedures.Arguments;

namespace Deploy.King.Executor
{
    public interface IProcedureFactory
    {
        IProcedure CreateFor(IProcedureArguments arguments);
    }
}