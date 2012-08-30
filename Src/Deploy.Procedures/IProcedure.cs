using Deploy.Procedures.Arguments;
using Deploy.Procedures.Builds;

namespace Deploy.Procedures
{
    public interface IProcedure
    {
        bool Perform(IProject project, Build build);
        string Name { get; }
    }
}