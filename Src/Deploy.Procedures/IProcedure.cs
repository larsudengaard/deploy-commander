using Deploy.Procedures.Arguments;
using Deploy.Procedures.Builds;

namespace Deploy.Procedures
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