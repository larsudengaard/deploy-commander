using Deploy.Procedures;
using Deploy.Procedures.Arguments;

namespace Deploy.King
{
    public interface IProcedureFactory
    {
        IProcedure CreateFor(IProcedureArguments arguments);
    }
}