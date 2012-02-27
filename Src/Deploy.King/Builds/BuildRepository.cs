using System;
using System.Collections.Generic;
using Deploy.King.Procedures;
using Deploy.King.Utilities;

namespace Deploy.King.Builds
{
    public class BuildRepository : IBuildRepository
    {
        public Build GetBuild(string id)
        {
            return new Build { Id = id, Received = DateTime.Now.AddHours(-25) };
        }

        public IEnumerable<Build> GetBuildsFor(IProcedure procedure)
        {
            return GetBuildsFor(procedure.GetType());
        }

        public IEnumerable<Build> GetBuildsFor<TProcedure>() where TProcedure : IProcedure
        {
            return GetBuildsFor(typeof (TProcedure));
        }

        public IEnumerable<Build> GetBuildsFor(Type procedureType)
        {
            if (!procedureType.IsA<IProcedure>())
                throw new ArgumentException("Must be type of IProcedure", "procedureType");

            return new List<Build>
            {
                new Build {Id = "0ef55b85", Received = DateTime.Now.AddHours(-25)},
                new Build {Id = "5d09d9cc", Received = DateTime.Now.AddHours(-47)},
            };
        }
    }
}