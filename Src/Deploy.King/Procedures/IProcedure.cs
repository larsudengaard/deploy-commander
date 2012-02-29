using Deploy.King.Builds;
using Deploy.King.Procedures.Arguments;

namespace Deploy.King.Procedures
{
    public interface IProcedure<TArguments> : IProcedure where TArguments : IProcedureArguments
    {
    }

    public interface IProcedure
    {
        bool Perform(Build build, IProcedureArguments arguments);
        string Name { get; }
    }
}