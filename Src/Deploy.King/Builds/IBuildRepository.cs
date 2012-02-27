using System;
using System.Collections.Generic;
using System.IO;
using Deploy.King.Procedures;
using Deploy.King.Procedures.Arguments;

namespace Deploy.King.Builds
{
    public interface IBuildRepository
    {
        Build GetBuild(string id);
        IEnumerable<Build> GetBuildsFor(IProcedureArguments arguments);
        byte[] GetPackage(Build build);
    }
}