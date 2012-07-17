using System.Collections.Generic;
using Deploy.Procedures.Arguments;

namespace Deploy.Procedures.Builds
{
    public interface IBuildRepository
    {
        Build GetBuild(string id);
        IEnumerable<Build> GetBuildsFor(IProcedureArguments arguments);
        Package GetPackage(Build build, string packageName);
        BuildInformation GetBuildInformation(string build);
    }
}