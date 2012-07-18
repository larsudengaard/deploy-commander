using System.Collections.Generic;
using Deploy.King.Projects;
using Deploy.Procedures.Builds;

namespace Deploy.King.Host.Models.Projects
{
    public class ProjectWithBuildsModel
    {
        readonly Project project;
        readonly List<Build> builds;
        readonly string error;

        public ProjectWithBuildsModel(Project project, List<Build> builds, string error)
        {
            this.error = error;
            this.project = project;
            this.builds = builds;
        }

        public ProjectWithBuildsModel(Project project, List<Build> builds)
            : this(project, builds, null)
        {
        }

        public string Error
        {
            get { return error; }
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