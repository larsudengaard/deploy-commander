using System.Collections.Generic;
using Deploy.Procedures.Arguments;

namespace Deploy.Procedures.Builds
{
    public interface IBuildRepository
    {
        Build GetBuild(string id);
        IEnumerable<Build> GetBuildsFor(IProcedureArguments arguments);
        string GetPackage(Build build);
        BuildInformation GetBuildInformation(string build);
    }
}