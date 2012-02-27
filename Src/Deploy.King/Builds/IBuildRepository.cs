using System;
using System.Collections.Generic;
using Deploy.King.Procedures;

namespace Deploy.King.Builds
{
    public interface IBuildRepository
    {
        Build GetBuild(string id);
        IEnumerable<Build> GetBuildsFor<TProcedure>() where TProcedure : IProcedure;
        IEnumerable<Build> GetBuildsFor(Type procedureType);
        IEnumerable<Build> GetBuildsFor(IProcedure procedure);
    }
}