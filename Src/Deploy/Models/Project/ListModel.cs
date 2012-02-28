using System;
using System.Collections.Generic;
using System.Linq;

namespace Deploy.Models.Project
{
    public class ListModel
    {
        readonly List<Build> builds;

        public ListModel(King.Projects.Project project, string procedureType, List<Build> builds)
        {
            this.builds = builds;
            Id = project.Id;
            Name = project.Name;
            ProcedureType = procedureType;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string ProcedureType { get; set; }

        public IEnumerable<Build> Builds
        {
            get { return builds; }
        }

        public class Build
        {
            public Build(King.Builds.Build build)
            {
                Id = build.Id;
                Number = build.Number;
                StartDate = build.StartDate;
            }

            public string Id { get; set; }
            public int Number { get; set; }
            public DateTime StartDate { get; set; }
        }
    }
}