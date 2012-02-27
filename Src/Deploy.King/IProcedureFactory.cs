using Deploy.King.Procedures;
using Deploy.King.Procedures.Arguments;

namespace Deploy.King
{
    public interface IProcedureFactory
    {
        IProcedure CreateFor(IProcedureArguments arguments);
    }
}