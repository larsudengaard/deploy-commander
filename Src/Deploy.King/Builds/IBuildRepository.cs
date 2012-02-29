using System.Collections.Generic;
using Deploy.King.Procedures.Arguments;

namespace Deploy.King.Builds
{
    public interface IBuildRepository
    {
        Build GetBuild(string id);
        IEnumerable<Build> GetBuildsFor(IProcedureArguments arguments);
        string GetPackage(Build build);
        BuildInformation GetBuildInformation(string build);
    }
}