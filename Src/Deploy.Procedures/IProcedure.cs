using Deploy.Procedures.Arguments;
using Deploy.Procedures.Builds;

namespace Deploy.Procedures
{
    public interface IProcedure
    {
        bool Perform(Build build, IProject project);
        string Name { get; }
    }
}