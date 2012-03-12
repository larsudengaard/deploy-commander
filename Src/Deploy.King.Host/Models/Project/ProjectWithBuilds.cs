using System;
using System.Collections.Generic;
using Deploy.King.Builds;

namespace Deploy.King.Host.Models.Project
{
    public class ProjectWithBuilds
    {
        readonly Projects.Project project;
        readonly List<Build> builds;

        public ProjectWithBuilds(Projects.Project project, List<Build> builds, string procedureType)
        {
            this.project = project;
            this.builds = builds;
            ProcedureType = procedureType;
        }

        public string ProcedureType { get; set; }

        public Projects.Project Project
        {
            get { return project; }
        }

        public IEnumerable<Build> Builds
        {
            get { return builds; }
        }
    }
}