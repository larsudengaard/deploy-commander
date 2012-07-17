using System.Collections.Generic;
using Deploy.King.Projects;
using Deploy.Procedures.Builds;

namespace Deploy.King.Host.Models.Projects
{
    public class ProjectWithBuildsModel
    {
        readonly Project project;
        readonly List<Build> builds;

        public ProjectWithBuildsModel(Project project, List<Build> builds)
        {
            this.project = project;
            this.builds = builds;
        }

        public Project Project
        {
            get { return project; }
        }

        public IEnumerable<Build> Builds
        {
            get { return builds; }
        }
    }
}